#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP 自定义渲染
*********************************************************************/
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public class CustomRenderPass : APostRenderPass
    {
        EPostPassType m_ePostType = EPostPassType.ForceCustomOpaque;
        public CustomRenderPass(CustomSettings param, EPostPassType postType)
        {
            m_ePostType = postType;
            OnInit(param);
        }
        //------------------------------------------------------
        public override EPostPassType GetPostPassType()
        {
            return m_ePostType;
        }
        //------------------------------------------------------
        public override void Setup(RenderTargetIdentifier src)
        {
        }
        //------------------------------------------------------
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_Setting.isEnabled()) return;
            if (renderingData.cameraData.camera.cullingMask == 0)
                return;

            var cmd = CommandBufferPool.Get(m_ProfilerTag);
            cmd.Clear();
            OnExecute(cmd, context, ref renderingData);
            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }
    }
}
#endif