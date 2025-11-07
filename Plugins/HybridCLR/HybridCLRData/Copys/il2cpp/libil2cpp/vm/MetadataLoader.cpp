#include "il2cpp-config.h"
#include "MetadataLoader.h"
#include "os/File.h"
#include "os/Mutex.h"
#include "utils/MemoryMappedFile.h"
#include "utils/PathUtils.h"
#include "utils/Runtime.h"
#include "utils/Logging.h"


#if IL2CPP_TARGET_ANDROID && IL2CPP_TINY_DEBUGGER && !IL2CPP_TINY_FROM_IL2CPP_BUILDER
#include <stdlib.h>
extern "C"
{
    void* loadAsset(const char* path, int *size, void* (*alloc)(size_t));
}
#elif IL2CPP_TARGET_JAVASCRIPT && IL2CPP_TINY_DEBUGGER && !IL2CPP_TINY_FROM_IL2CPP_BUILDER
extern void* g_MetadataForWebTinyDebugger;
#endif

namespace MetadataProcess
{
	static const int COMMON_ENCRYPT_ARRAY[] = { 23,12,63,65 };
	class Decrypt
	{
	private:
		static void meta_decode(int* v, const int* k)
		{
			unsigned int n=32, sum, y=v[0], z=v[1],
				delta=0x9e3779b9 ;
			sum=delta<<5 ;
			/* start cycle */
			while (n-->0) {
				z-= (y<<4)+k[2] ^ y+sum ^ (y>>5)+k[3] ;
				y-= (z<<4)+k[0] ^ z+sum ^ (z>>5)+k[1] ;
				sum-=delta ;  }
			/* end cycle */
			v[0]=y ; v[1]=z ;
		}

		static void meta_decode_byte(char* v, const int* k, int p)
		{
			char y[] = "Unity$tea";
			*v = *v^(char)(k[p%4]%0xFF)^y[p];
		}		
	public:
		static bool isEncrypted;
		static void meta_decode_buffer(char* in_buffer, unsigned int in_size, int cipherRemains)
		{
			char *p;
			unsigned int remain = in_size % 8;
			unsigned int align_size = in_size - remain;
			for (p = in_buffer; p < in_buffer + align_size; p += 8)
			{
				meta_decode( (int*)p, COMMON_ENCRYPT_ARRAY);
			}
			if( remain > 0 && cipherRemains )
			{
				for (p = in_buffer + align_size; p < in_buffer + in_size; p += 1)
				{
					meta_decode_byte( p, COMMON_ENCRYPT_ARRAY, --remain );	
				}
			}
		}
	};
}
bool MetadataProcess::Decrypt::isEncrypted = false;

void* il2cpp::vm::MetadataLoader::LoadMetadataFile(const char* fileName)
{
#if IL2CPP_TARGET_ANDROID && IL2CPP_TINY_DEBUGGER && !IL2CPP_TINY_FROM_IL2CPP_BUILDER
    std::string resourcesDirectory = utils::PathUtils::Combine(utils::StringView<char>("Data"), utils::StringView<char>("Metadata"));

    std::string resourceFilePath = utils::PathUtils::Combine(resourcesDirectory, utils::StringView<char>(fileName, strlen(fileName)));

    int size = 0;
    void* fileBuffer = loadAsset(resourceFilePath.c_str(), &size, malloc);
	if(size>4)
	{
		char* tempBuff = (char*)fileBuffer;
		char h0 = *((char*)(tempBuff+0));
		char h1 = *((char*)(tempBuff+1));
		char h2 = *((char*)(tempBuff+2));
		char h3 = *((char*)(tempBuff+3));
		if(h0=='e' && h1 == 'n' && h2 == 'c' && h3=='r')
		{
			MetadataProcess::Decrypt::isEncrypted = true;
			utils::Logging::Write("INFO: %s is Encrypted!", resourceFilePath.c_str());
			tempBuff += sizeof(char)*4;
			MetadataProcess::Decrypt::meta_decode_buffer(tempBuff,size-4,1);
			utils::Logging::Write("INFO: %s Decode Succeed!", resourceFilePath.c_str());
			return (void*)tempBuff;
		}		
	}
	return fileBuffer;
#elif IL2CPP_TARGET_JAVASCRIPT && IL2CPP_TINY_DEBUGGER && !IL2CPP_TINY_FROM_IL2CPP_BUILDER
    return g_MetadataForWebTinyDebugger;
#else
    std::string resourcesDirectory = utils::PathUtils::Combine(utils::Runtime::GetDataDir(), utils::StringView<char>("Metadata"));

    std::string resourceFilePath = utils::PathUtils::Combine(resourcesDirectory, utils::StringView<char>(fileName, strlen(fileName)));

    int error = 0;
    os::FileHandle* handle = os::File::Open(resourceFilePath, kFileModeOpen, kFileAccessRead, kFileShareRead, kFileOptionsNone, &error);
    if (error != 0)
    {
        utils::Logging::Write("ERROR: Could not open %s", resourceFilePath.c_str());
        return NULL;
    }

    void* fileBuffer = utils::MemoryMappedFile::Map(handle);
	int fileSize = os::File::GetLength(handle, &error);
    if (error != 0)
    {
        os::File::Close(handle, &error);
        utils::Logging::Write("ERROR: getFileLength error %s", resourceFilePath.c_str());
        return NULL;
    }

    os::File::Close(handle, &error);
    if (error != 0)
    {
        utils::MemoryMappedFile::Unmap(fileBuffer);
        fileBuffer = NULL;
        return NULL;
    }
	if(fileSize>4)
	{
		char* tempBuff = (char*)fileBuffer;
		char h0 = *((char*)(tempBuff+0));
		char h1 = *((char*)(tempBuff+1));
		char h2 = *((char*)(tempBuff+2));
		char h3 = *((char*)(tempBuff+3));
		if(h0=='e' && h1 == 'n' && h2 == 'c' && h3=='r')
		{
			MetadataProcess::Decrypt::isEncrypted = true;
			utils::Logging::Write("INFO: %s is Encrypted!", resourceFilePath.c_str());
			tempBuff += sizeof(char)*4;
			char* fileOutput = (char*)malloc(fileSize-4);
            memcpy(fileOutput, tempBuff, fileSize-4);
			MetadataProcess::Decrypt::meta_decode_buffer(fileOutput,fileSize-4,1);
			utils::Logging::Write("INFO: %s Decode Succeed!", resourceFilePath.c_str());
			il2cpp::utils::MemoryMappedFile::Unmap(fileBuffer);
			fileBuffer = NULL;
			return (void*)fileOutput;
		}
	}
    return fileBuffer;
#endif
}

void il2cpp::vm::MetadataLoader::UnloadMetadataFile(void* fileBuffer)
{
	if(MetadataProcess::Decrypt::isEncrypted)
	{
		free(fileBuffer);
		return;
	}
#if IL2CPP_TARGET_ANDROID && IL2CPP_TINY_DEBUGGER && !IL2CPP_DEBUGGER_TESTS
	free(fileBuffer);
#else
	bool success = il2cpp::utils::MemoryMappedFile::Unmap(fileBuffer);
	NO_UNUSED_WARNING(success);
	IL2CPP_ASSERT(success);		
#endif
}
