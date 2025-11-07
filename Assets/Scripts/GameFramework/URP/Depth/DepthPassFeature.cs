#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace Framework.URP
{
    public class DepthPassFeature : APostRenderFeature
    {
   //     static int m_nToggleRef = 0;
        static DepthPassFeature ms_pInstance = null;
        public PassSettings settings = new PassSettings();
        DepthOnlyPass m_DepthPrepass;
        RenderTargetHandle m_DepthTexture;
        //------------------------------------------------------
        public override void Create()
        {
            ms_pInstance = this;
            m_DepthTexture.Init("_CameraDepthTexture");
            m_DepthPrepass = new DepthOnlyPass(UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPrePasses, RenderQueueRange.opaque, settings.layerMask);
        }
        //------------------------------------------------------
        protected bool IsEnable()
        {
            return settings.isEnabled() /*&& m_nToggleRef>0 */&& URPPostWorker.IsEnabledPass(EPostPassType.DepthPass);
        }
//         //------------------------------------------------------
//         public static void ToggleRef(bool bEnable)
//         {
//             if (bEnable) m_nToggleRef++;
//             else
//             {
//                 m_nToggleRef--;
//                 if (m_nToggleRef < 0) m_nToggleRef = 0;
//             }
//             OnTogglePass(m_nToggleRef>0);
//         }
        //------------------------------------------------------
        internal static void OnTogglePass(bool bEnable)
        {
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            {
                var renderingAsset= (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
                renderingAsset.supportsCameraDepthTexture = bEnable;
            }
        }
        //------------------------------------------------------
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!IsEnable() || renderingData.cameraData.isSceneViewCamera) return;
#if UNITY_2020_1_OR_NEWER
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
#endif
            int layerMask = settings.layerMask.value;
            if ((renderingData.cameraData.camera.cullingMask & layerMask) == 0) return;
#if UNITY_2022_1_OR_NEWER
#else
            m_DepthPrepass.Setup(renderingData.cameraData.cameraTargetDescriptor, m_DepthTexture);
#endif
            renderer.EnqueuePass(m_DepthPrepass);
        }
    }
}
#endif