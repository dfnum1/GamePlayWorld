#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public class CustomPassFeature : APostRenderFeature
    {
        CustomRenderPass m_OpaquePass;
        CustomRenderPass m_TransparentPass;

        EPostPassType m_CustomOpaque;
        EPostPassType m_CustomTransparent;
        public CustomSettings customSetting;
        //------------------------------------------------------
        public override void Create()
        {
            this.name = "CustomMain";
            m_CustomOpaque = EPostPassType.ForceCustomOpaque;
            m_CustomTransparent = EPostPassType.ForceCustomTransparent;

            m_OpaquePass = new CustomRenderPass(customSetting, m_CustomOpaque);
            m_OpaquePass.SetProfilerTag("CustomMainOpaques");
            m_OpaquePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

            m_TransparentPass = new CustomRenderPass(customSetting, m_CustomTransparent);
            m_TransparentPass.SetProfilerTag("CustomMainTransparents");
            m_TransparentPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }
        //------------------------------------------------------
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (customSetting.isEnabled())
            {
                if (URPPostWorker.IsEnabledPass(m_CustomOpaque))
                {
                    if ((renderingData.cameraData.camera.cullingMask & customSetting.layerMask.value) != 0)
                    {
                        var src = renderer.cameraColorTarget;
                        m_OpaquePass.Setup(src);
                        renderer.EnqueuePass(m_OpaquePass);
                    }
                }
                if (URPPostWorker.IsEnabledPass(m_CustomTransparent))
                {
                    if ((renderingData.cameraData.camera.cullingMask & customSetting.layerMask.value) != 0)
                    {
                        var src = renderer.cameraColorTarget;
                        m_TransparentPass.Setup(src);
                        renderer.EnqueuePass(m_TransparentPass);
                    }
                }

            }
        }
    }
}
#endif