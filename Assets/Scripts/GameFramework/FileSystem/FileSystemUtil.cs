/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	FileSystem
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Core
{
    public partial class FileSystemUtil
    {
        public static string AssetPath 
        {
            get 
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return Application.dataPath;
                return fileSystem.GetAssetPath(); 
            } 
        }
        public static string StreamPath
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return Application.streamingAssetsPath;
                return fileSystem.GetStreamPath();
            }
        }
        public static string StreamRawPath
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return Application.streamingAssetsPath + "/raws/";
                return fileSystem.GetStreamRawPath();
            }
        }
        public static string StreamBinaryPath
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return Application.dataPath + "/Binarys/";
                return fileSystem.GetStreamBinaryPath();
            }
        }
        public static string PersistenDataPath
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return Application.dataPath + "/../Local/";
                return fileSystem.GetPersistenDataPath();
            }
        }
        public static string UpdateDataPath
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return null;
                return fileSystem.GetUpdateDataPath();
            }
        }
        public static string LocalUpdateFile
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return null;
                return fileSystem.GetLocalUpdateFile();
            }
        }
        public static string LocalVersionFile
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return null;
                return fileSystem.GetLocalVersionFile();
            }
        }
        public static string Version
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return "1.0.0";
                return fileSystem.GetVersion();
            }
        }
        public static string PlublishVersion
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return "1.0.0";
                return fileSystem.GetPlublishVersion();
            }
        }
        public static void Build(EFileSystemType eType, string basePkgName = "base_pkg", string srrVersion = "1.0.0", short base_pack_cnt = 0, string strEncryptKey = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.Build(eType, basePkgName, srrVersion, base_pack_cnt, strEncryptKey);
        }
        public static void SetCapability(int oneFrameCost = 300, int maxInstanceCnt = 30, int destroyDelayTime = 60)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.SetCapability(oneFrameCost, maxInstanceCnt, destroyDelayTime);
        }

        public static void SetInstanceDelayDestroyParam(int destroyDelayTime)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.SetInstanceDelayDestroyParam(destroyDelayTime);
        }
        public static void EnableDebug(bool bDebug)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.EnableDebug(bDebug);
        }
        public static bool isEnableDebug()
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return false;
            return fileSystem.isEnableDebug();
        }

        public static void Update(float fFrame, bool bCostCheck = true)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.Update(fFrame, bCostCheck);
        }
        public static AFileSystem GetFileSystem()
        {
            if (AFramework.mainFramework == null)
            {
#if UNITY_EDITOR
            if (AFramework.editorFramework != null)
                return AFramework.editorFramework.FileSystem;

#endif
                return null;
            }
            return AFramework.mainFramework.FileSystem;
        }

        public static EFileSystemType GetStreamType()
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return EFileSystemType.AssetData;
            return fileSystem.GetStreamType();
        }
        public static void RefreshLocalUpdate()
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.RefreshLocalUpdate();
        }
        public static void Free(float fDelta = 0)
        {
               AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.Free(fDelta);
        }
#if !USE_SERVER
        public static Asset LoadAsset(string strFile, bool bAsync = false, bool bImmediately = false, bool bPremanent = false, System.Type assignAssetType = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.LoadAsset(strFile, bAsync, bImmediately, bPremanent, assignAssetType);
        }
        public static AssetOperiaon LoadScene(string strFile, string strSceneName, ELoadSceneMode mode = ELoadSceneMode.Single, bool bAysnc = false)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.LoadScene(strFile, strSceneName, mode, bAysnc);
        }

        public static void DeSpawnInstance(AInstanceAble pAble, int nCheckMax = 2)
        {
            if (pAble == null) return;
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.DeSpawnInstance(pAble, nCheckMax);
        }

        public static bool IsEditorMode
        {
            get
            {
#if UNITY_EDITOR
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return false;
                return fileSystem.IsEditorMode();
#else
                return false;
#endif
            }
#if UNITY_EDITOR
            set
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return;
                fileSystem.SetEditorMode(value);
            }
#endif
        }
        public static Transform PoolRoot
        {
            get
            {
                AFileSystem fileSystem = GetFileSystem();
                if (fileSystem == null) return null;
                return fileSystem.GetPoolRoot();
            }
        }
        public static void OnAssetRelease(Asset asset)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.OnAssetRelease(asset);
        }
        public static void OnAssetGrab(Asset asset)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.OnAssetGrab(asset);
        }
        public static void OnAssetDispose(Asset asset)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.OnAssetDispose(asset);
        }
        public static void OnAssetCallback(Asset asset)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.OnAssetCallback(asset);
        }
        public static void AddReleaseAsset(Asset asset)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.AddReleaseAsset(asset);
        }

        public static AssetOperiaon AsyncReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.AsyncReadFile(strFile, onCallback);
        }
        public static AssetOperiaon ReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.ReadFile(strFile, onCallback);
        }

        public static AssetOperiaon LoadCustomFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.LoadCustomFile(strFile, onCallback);
        }

        public static Package.EnterData FindEnterData(string abName)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.FindEnterData(abName);
        }
        public static void AddFile(string strAbName, string strAbPath, string md5, string[] dependAbs)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.AddFile(strAbName, strAbPath, md5, dependAbs);
        }

        public static Asset FindAndLoadAsset(string strFile, bool bAsync = false, bool bImmediately = false, bool bPremanent = false, System.Type assignAssetType= null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.FindAndLoadAsset(strFile, bAsync, bImmediately, bPremanent, assignAssetType);
        }
        public static InstanceOperiaon SpawnInstance(string strFile, bool bAsync = true)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.SpawnInstance(strFile, bAsync);
        }
        public static bool SpawnInstance(InstanceOperiaon pOp)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return false;
            return fileSystem.SpawnInstance(pOp);
        }
        //------------------------------------------------------
        public static InstanceOperiaon SpawnInstance(GameObject pAsset, bool bAsync = false)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.SpawnInstance(pAsset, bAsync);
        }
        //------------------------------------------------------
        public static void PreSpawnInstance(string strFile, bool bAsync = true, bool bFrontQueue = true)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.PreSpawnInstance(strFile, bAsync, bFrontQueue);
        }
        //------------------------------------------------------
        public static void PreSpawnInstance(GameObject pPrefab, bool bFrontQueue = true)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.PreSpawnInstance(pPrefab, bFrontQueue);
        }
        //------------------------------------------------------
        public static void PreDeSpawnInstance(string strFile, int cnt=1)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.PreDeSpawnInstance(strFile, cnt);
        }
        //------------------------------------------------------
        public static void PreDeSpawnInstance(GameObject pPrefab, int cnt = 1)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return;
            fileSystem.PreDeSpawnInstance(pPrefab, cnt);
        }
        //------------------------------------------------------
        public static int GetPreSpawnStats(string strFile)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return 0;
            return fileSystem.GetPreSpawnStats(strFile);
        }
        public static int GetPreSpawnStats(GameObject pPrefab)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return 0;
            return fileSystem.GetPreSpawnStats(pPrefab);
        }
        public static bool IsInLoadQueue(string strFile)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return false;
            return fileSystem.IsInLoadQueue(strFile);
        }
        //-------------------------------------------------
        public static int StatsInstanceCount(string strFile)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return 0;
            return fileSystem.StatsInstanceCount(strFile);
        }
        //-------------------------------------------------
        public static int StatsInstanceCount(int gui)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return 0;
            return fileSystem.StatsInstanceCount(gui);
        }
        //------------------------------------------------- 
        public static AssetBundle LoadAssetBunlde(AFileSystem fileSystem, string abPath, PackageStream pStream = null)
        {
            if (fileSystem == null) fileSystem = GetFileSystem();
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(abPath);
                request.timeout = 5;
                var op = request.SendWebRequest();
                //error
                //while (!op.isDone)
                //{
                //    if (request.result == UnityWebRequest.Result.Success)
                //        break;
                //    if (request.result == UnityWebRequest.Result.ConnectionError ||
                //        request.result == UnityWebRequest.Result.ProtocolError ||
                //        request.result == UnityWebRequest.Result.DataProcessingError)
                //    {
                //        Debug.LogError(abPath + " loaded error:" + request.result);
                //        return null;
                //    }
                //}
                return DownloadHandlerAssetBundle.GetContent(request);
            }
            else
            {
                if (fileSystem.eType == EFileSystemType.EncrptyPak)
                {
                    if (fileSystem.isEnableDebug()) Debug.Log(abPath);
                    if (pStream == null)
                    {
                        pStream = new PackageStream(fileSystem, abPath);
                    }
                    return AssetBundle.LoadFromStream(pStream, 0, fileSystem.GetSreamReadBufferSize());
                }
#if UNITY_5_1
                return = AssetBundle.CreateFromFile(abPath);
#else
                return AssetBundle.LoadFromFile(abPath);
#endif
            }
        }
        //------------------------------------------------- 
        public static IEnumerator CoroutineLoadAssetBunlde(AFileSystem fileSystem, string abPath, System.Action<AssetBundle> callback, PackageStream pStream = null)
        {
            if (fileSystem == null) fileSystem = GetFileSystem();
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(abPath);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError ||
                                        request.result == UnityWebRequest.Result.ProtocolError ||
                                        request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(abPath + " loaded error:" + request.result);
                    if (callback != null) callback(null);
                     yield break;
                }
                if (callback != null) callback(DownloadHandlerAssetBundle.GetContent(request));
                yield break;
            }
            else
            {
                if (fileSystem.eType == EFileSystemType.EncrptyPak)
                {
                    if (fileSystem.isEnableDebug()) Debug.Log(abPath);
                    if (pStream == null)
                    {
                        pStream = new PackageStream(fileSystem, abPath);
                    }
                    var bundleLoadRequest = AssetBundle.LoadFromStreamAsync(pStream, 0, fileSystem.GetSreamReadBufferSize());
                    yield return bundleLoadRequest;
                    if (callback != null) callback(bundleLoadRequest.assetBundle);
                    yield break;
                }
#if UNITY_5_1
                //yield return AssetBundle.CreateFromFile(abPath);
#else
                var requestAb = AssetBundle.LoadFromFileAsync(abPath);
                yield return requestAb;
                if (callback != null) callback(requestAb.assetBundle);
                yield break;
#endif
            }
        }
        //-------------------------------------------------
        static IEnumerator CoroutineWebLoadAsssetBundle(UnityWebRequest request)
        {
            yield return request.SendWebRequest();
        }
        //-------------------------------------------------
        public static void LoadAssetBundle(AFileSystem fileSystem, AssetBundleInfo ab)
        {
            if (ab == null) return;
            ab.bReqing = false;
            if (ab.asyncRequest != null) return;
            if (ab.assetbundle == null)
            {
                if (fileSystem == null) fileSystem = GetFileSystem();
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(ab.abPath);
                    ab.asyncRequest = request.SendWebRequest();
                }
                else
                {
                    if (fileSystem.eType == EFileSystemType.EncrptyPak)
                    {
                        if (fileSystem.isEnableDebug()) Debug.Log(ab.abName);
                        if (ab.packageStream == null)
                            ab.packageStream = new PackageStream(fileSystem, ab.abName);
                        ab.assetbundle = AssetBundle.LoadFromStream(ab.packageStream, 0, fileSystem.GetSreamReadBufferSize());
                    }
                    else
                    {
                        ab.assetbundle = LoadAssetBunlde(fileSystem, ab.abPath);
                    }
                }

            }
        }
        //-------------------------------------------------
        public static void AsyncLoadAssetBundle(AFileSystem fileSystem, AssetBundleInfo ab)
        {
            if (ab == null) return;
            ab.bReqing = false;
            if (ab.assetbundle) return;
            if (fileSystem == null) fileSystem = GetFileSystem();
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (ab.asyncRequest == null)
                {
                    UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(ab.abPath);
                    ab.asyncRequest = request.SendWebRequest();
                }
            }
            else
            {
                if (fileSystem.eType == EFileSystemType.EncrptyPak)
                {
                    if (ab.asyncRequest == null)
                    {
                        if (ab.packageStream == null)
                            ab.packageStream = new PackageStream(fileSystem, ab.abName);
                        ab.asyncRequest = AssetBundle.LoadFromStreamAsync(ab.packageStream, 0, fileSystem.GetSreamReadBufferSize());
                    }
                    return;
                }

                if (ab.asyncRequest == null)
                {
                    ab.asyncRequest = AssetBundle.LoadFromFileAsync(ab.abPath);
                }
            }

        }
        //--------------------------------------------------------
        public static bool FileExist(string strFile)
        {
            return File.Exists(strFile);
        }
        //--------------------------------------------------------
        public static string ReadFileIfExist(string strFile)
        {
            if (File.Exists(strFile))
                return System.IO.File.ReadAllText(strFile);
            return null;
        }
#else
        public static Asset LoadAsset(string strFile, System.Type objType = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.LoadAsset(strFile, objType);
        }
        public static T ReadFile<T>(string strFile) where T : ExternEngine.Object, new()
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.ReadFile<T>(strFile);
        }
        //-------------------------------------------------
        public static ExternEngine.Object ReadFile(string strFile, System.Type objType)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.ReadFile(strFile, objType);
        }
        //-------------------------------------------------
        public static AssetOperiaon ReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            AFileSystem fileSystem = GetFileSystem();
            if (fileSystem == null) return null;
            return fileSystem.ReadFile(strFile, onCallback);
        }
#endif
        //------------------------------------------------------
        public static long ConverVersion(string version)
        {
            long result = 0;
            long.TryParse(version.Replace(".", ""), out result);
            return result;
        }
    }
}

