#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    [System.Serializable]
    public class ScreenPassSettings : PassSettings
    {        
        public Material blit;
#if UNITY_EDITOR
        [System.NonSerialized]
        Material m_BlitCopy;
#endif
        public Material blitMat
        {
            get
            {
#if UNITY_EDITOR
                if (blit == null) return null;
                if (m_BlitCopy == null) m_BlitCopy = new Material(blit);
                return m_BlitCopy;
#else
                return blit;
#endif
            }
        }

        public override bool isEnabled()
        {
            return base.isEnabled() && blitMat != null;
        }
    }
    public class ScreenPassFeature : APostRenderFeature
    {
        ScreenRenderPass m_ScriptablePass;
        public ScreenPassSettings settings = new ScreenPassSettings();
        //------------------------------------------------------
        public override void Create()
        {
            this.name = settings.passName;
            m_ScriptablePass = new ScreenRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.renderEvent;
        }
        //------------------------------------------------------
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!settings.isEnabled()) return;
            if (!URPPostWorker.IsEnabledPass(EPostPassType.Screen))
                return;
            if (renderingData.cameraData.isSceneViewCamera) return;
            if ((renderingData.cameraData.camera.cullingMask & settings.layerMask.value) == 0) return;
            var src = renderer.cameraColorTarget;
            m_ScriptablePass.Setup(src);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
#endif