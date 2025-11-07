#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	IPostPassRender
作    者:	HappLI
描    述:	URP
*********************************************************************/

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public interface IPostPassRender : IBasePostPassRender
    {
        void OnFrameCleanup(CommandBuffer cmd);
        void OnSetup(RenderTargetIdentifier source);
        void OnRenderPassExecute(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData);
    }    
}
#endif