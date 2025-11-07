#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    [System.Serializable]
    public class CustomSettings
    {
        [SerializeField]
        bool bEnabled = true;
        public LayerMask layerMask = -1;
        public string passName = "";

        public bool IsOpaque = true;
        public string[] ShaderTags;

        public virtual bool isEnabled()
        {
            return bEnabled;
        }
    }

    [System.Serializable]
    public class PassSettings : CustomSettings
    {
        public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public abstract class APostRenderFeature : ScriptableRendererFeature
    {
        protected void OnCreatePass(APostRenderPass renderPass)
        {

        }
    }
}
#endif