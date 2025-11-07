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
    public class VignetteRenderPass : APostRenderPass
    {
        static class Uniforms
        {
            internal static readonly int _Vignette_Color = Shader.PropertyToID("_Vignette_Color");
            internal static readonly int _Vignette_Center = Shader.PropertyToID("_Vignette_Center");
            internal static readonly int _Vignette_Settings = Shader.PropertyToID("_Vignette_Settings");
            internal static readonly int _Vignette_Mask = Shader.PropertyToID("_Vignette_Mask");
        }

        VignettePassSettings m_ScreenSetting;
        public VignetteRenderPass(PassSettings param)
        {
            this.renderPassEvent = param.renderEvent;
            m_ScreenSetting = param as VignettePassSettings;
            OnInit(param);
        }
        //------------------------------------------------------
        public override EPostPassType GetPostPassType()
        {
            return EPostPassType.Vignette;
        }
        //------------------------------------------------------
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!IsActived() || m_ScreenSetting.blitMat == null) return;
            var cmd = CommandBufferPool.Get(m_ProfilerTag);

            m_ScreenSetting.color.a = m_ScreenSetting.opacity;
            m_ScreenSetting.blitMat.SetColor(Uniforms._Vignette_Color, m_ScreenSetting.color);
            m_ScreenSetting.blitMat.SetVector(Uniforms._Vignette_Center, m_ScreenSetting.center);
            float roundness = (1f - m_ScreenSetting.roundness) * 6f + m_ScreenSetting.roundness;
            m_ScreenSetting.blitMat.SetVector(Uniforms._Vignette_Settings, new Vector4(m_ScreenSetting.intensity * 3f, m_ScreenSetting.smoothness * 5f, roundness, m_ScreenSetting.rounded ? 1f : 0f));

            Camera camera = renderingData.cameraData.camera;
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_ScreenSetting.blitMat);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            OnExecute(cmd, context, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif