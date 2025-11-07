#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	VignettePassFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    [System.Serializable]
    public class VignettePassSettings : PassSettings
    {        
        public Material blit;

        [Tooltip("Sets the vignette center point (screen center is [0.5, 0.5]).")]
        public Vector2 center = new Vector2(0.5f, 0.5f);
        [Tooltip("Vignette color.")]
        public Color color = new Color(0f, 0f, 0f, 1f);
        [Range(0f, 1f), Tooltip("Amount of vignetting on screen.")]
        public float intensity = 0;
        [Range(0.01f, 1f), Tooltip("Smoothness of the vignette borders.")]
        public float smoothness = 0.2f;
        [Range(0f, 1f), Tooltip("Lower values will make a square-ish vignette.")]
        public float roundness = 1f;
        [Tooltip("Set to true to mark the vignette to be perfectly round. False will make its shape dependent on the current aspect ratio.")]
        public bool rounded = false;
        [Range(0f, 1f), Tooltip("Mask opacity.")]
        public float opacity = 1f;
        public Material blitMat
        {
            get
            {
                return blit;
            }
        }

        public override bool isEnabled()
        {
            return base.isEnabled() && blitMat != null;
        }
    }
    public class VignettePassFeature : APostRenderFeature
    {
        VignetteRenderPass m_ScriptablePass;
        public VignettePassSettings settings = new VignettePassSettings();
        //------------------------------------------------------
        public override void Create()
        {
            this.name = settings.passName;
            m_ScriptablePass = new VignetteRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.renderEvent;
        }
        //------------------------------------------------------
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            if (!settings.isEnabled()) return;
            if (!URPPostWorker.IsEnabledPass(EPostPassType.Vignette))
                return;
            if ((renderingData.cameraData.camera.cullingMask & settings.layerMask.value) == 0) return;
            var src = renderer.cameraColorTarget;
            m_ScriptablePass.Setup(src);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
#endif