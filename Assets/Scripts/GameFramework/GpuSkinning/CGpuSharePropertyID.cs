#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    internal class CGpuSharePropertyID
    {
        public static MaterialPropertyBlock materialBlock = null;

        public static int shaderPorpID_MainTex = -1;
        public static int shaderPropID_GPUSkinning_TextureMatrix = -1;
        public static int shaderPropID_GPUSkinning_TextureSize_NumPixelsPerFrame = -1;
        public static int shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation = -1;
        public static int shaderPropID_GPUSkinning_SKINS = -1;
        public static int shaderPropID_GPUSkinning_SKIN_PixelSegmentation = -1;


        public static void Init()
        {
            if(materialBlock == null)
            {
                materialBlock = new MaterialPropertyBlock();
                shaderPropID_GPUSkinning_SKINS = Shader.PropertyToID("_SKIN_UVS");
                shaderPropID_GPUSkinning_TextureMatrix = Shader.PropertyToID("_GPUSkinning_TextureMatrix");
                shaderPropID_GPUSkinning_TextureSize_NumPixelsPerFrame = Shader.PropertyToID("_GPUSkinning_TextureSize_NumPixelsPerFrame");
                shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation = Shader.PropertyToID("_GPUSkinning_FrameIndex_PixelSegmentation");
                shaderPropID_GPUSkinning_SKIN_PixelSegmentation = Shader.PropertyToID("_GPUSkinning_SKIN_PixelSegmentation");
                shaderPorpID_MainTex = Shader.PropertyToID("_DiffuseTex");

            }
        }
    }
}
#endif