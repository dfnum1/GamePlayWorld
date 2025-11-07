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
    public class ScreenRenderPass : APostRenderPass
    {
        ScreenPassSettings m_ScreenSetting;
        public ScreenRenderPass(PassSettings param)
        {
            this.renderPassEvent = param.renderEvent;
            m_ScreenSetting = param as ScreenPassSettings;
            OnInit(param);
        }
        //------------------------------------------------------
        public override EPostPassType GetPostPassType()
        {
            return EPostPassType.Screen;
        }
        //------------------------------------------------------
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!IsActived() || m_ScreenSetting.blitMat == null) return;
            var cmd = CommandBufferPool.Get(m_ProfilerTag);
            OnExecute(cmd, context, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif