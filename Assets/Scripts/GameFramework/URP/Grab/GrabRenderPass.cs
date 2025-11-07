#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP 自定义渲染
*********************************************************************/
using Framework.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public class GrabRenderPass : APostRenderPass
    {
        GrabPassSettings m_GrabSetting;
        private int m_nGrabRT;
        private bool m_bActived = false;
        private bool m_bAutoBlur = false;

        float m_RenderScale = -1;
        RenderTargetIdentifier m_Source;
        RenderTexture m_pGrabRT = null;
        UnityEngine.Experimental.Rendering.GraphicsFormat m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm;

        private static readonly ShaderTagId shaderTag = new ShaderTagId("GrabPass");
        public GrabRenderPass(PassSettings param)
        {
            this.renderPassEvent = param.renderEvent;
            m_GrabSetting = param as GrabPassSettings;
            m_bActived = false;
            m_bAutoBlur = false;
            OnInit(param);

             m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
#if UNITY_ANDROID && !UNITY_EDITOR
            if(SystemInfo.IsFormatSupported( UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_ETC_UNorm, UnityEngine.Experimental.Rendering.FormatUsage.Render) )
            {
                m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_ETC_UNorm;
            }
            else if (SystemInfo.IsFormatSupported(UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_ETC2_UNorm, UnityEngine.Experimental.Rendering.FormatUsage.Render))
            {
                m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_ETC2_UNorm;
            }
#elif UNITY_IOS
            if(SystemInfo.IsFormatSupported( UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_PVRTC_4Bpp_UNorm, UnityEngine.Experimental.Rendering.FormatUsage.Render) )
            {
                m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.RGB_PVRTC_4Bpp_UNorm;
            }
#elif UNITY_EDITOR
            if (SystemInfo.IsFormatSupported(UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm, UnityEngine.Experimental.Rendering.FormatUsage.Render))
            {
                m_GraphicFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm;
            }
#endif
            //   BlitMat = new Material(Shader.Find("Hidden/Universal Render Pipeline/Blit"));
        }
        //------------------------------------------------------
        public override EPostPassType GetPostPassType()
        {
            return EPostPassType.Grab;
        }
        //------------------------------------------------------
        public RenderTexture GetRenderRT()
        {
            return m_pGrabRT;
        }
        //------------------------------------------------------
        public void SetActive(bool bActive, float renderScale = -1, bool bAutoBlur = false)
        {
            m_bAutoBlur = bAutoBlur;
            if (m_bActived == bActive) return;
            m_bActived = bActive;
            if (m_bActived)
            {
                m_RenderScale = renderScale;
                if (m_RenderScale < 0) m_RenderScale = m_GrabSetting.renderScale;
            }
            else
            {
                //! set delay release
                Core.TimerManager.RegisterTimer(AFramework.mainFramework, "GrabPassRelease", OnTimerTick, 2000, 1, false, false, null,true);
            //    OnTimerTick(0, null);
            }
        }
        ////------------------------------------------------------
        //public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        //{
        //    RenderTextureDescriptor descriptor = cameraTextureDescriptor;
        //    descriptor.depthBufferBits = 0;
        //    descriptor.width = (int)(descriptor.width * m_GrabSetting.renderScale);
        //    descriptor.height = (int)(descriptor.height * m_GrabSetting.renderScale);
        //    cmd.GetTemporaryRT(m_nGrabRT.id, descriptor, FilterMode.Bilinear);
        //
        //    cmd.SetGlobalTexture(m_GrabSetting.textureID, m_nGrabRT.Identifier());
        //    m_nGrabWidth = descriptor.width;
        //    m_nGrabHeight = descriptor.height;
        //}
        //------------------------------------------------------
        public override void Setup(RenderTargetIdentifier src)
        {
            base.Setup(src);
            m_Source = src;
        }
        //------------------------------------------------------
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_Setting.isEnabled() || !m_bActived || m_RenderScale<0) return;

            if (renderingData.cameraData.isSceneViewCamera ) return;
#if UNITY_2020_1_OR_NEWER
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
#endif

            CommandBuffer cmd = CommandBufferPool.Get("GrabPass");
            // 
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 0;
            desc.depthBufferBits = 0;
            int width = (int)(desc.width * m_RenderScale);
            int height = (int)(desc.height * m_RenderScale);
            if (m_pGrabRT == null || m_pGrabRT.width != width || m_pGrabRT.height != height)
            {
                if(m_pGrabRT!=null) RenderTexture.ReleaseTemporary(m_pGrabRT);
                m_pGrabRT = RenderTexture.GetTemporary(width, height, 0, m_GraphicFormat);
            }

            if(GrabPassFeature.GrabStat>=0)
            {
                // m_nGrabRT = Shader.PropertyToID(m_GrabSetting.textureID);
                // cmd.ReleaseTemporaryRT(m_nGrabRT);

                //cmd.GetTemporaryRT(m_nGrabRT, width, height, 0, FilterMode.Bilinear, m_GraphicFormat);

                if(GrabPassFeature.PassMaterial)
                {
                    cmd.Blit(m_Source, m_pGrabRT, GrabPassFeature.PassMaterial);
                }
                else
                {
                    if (m_bAutoBlur && m_GrabSetting.blurMat)
                        cmd.Blit(m_Source, m_pGrabRT, m_GrabSetting.blurMat, 0);
                    else
                        cmd.Blit(m_Source, m_pGrabRT);
                }

                Shader.SetGlobalTexture(m_GrabSetting.textureID, GetRenderRT());
                context.ExecuteCommandBuffer(cmd);
            }

     //       cmd.ReleaseTemporaryRT(m_nGrabRT);
            CommandBufferPool.Release(cmd);

            GrabPassFeature.GrabStatMaskFlag &= ~renderingData.cameraData.camera.cullingMask;
            if (GrabPassFeature.GrabStat > 0)
            {
                if (GrabPassFeature.GrabStatMaskFlag == 0)
                {
               //     ED.EditorHelp.SaveToPng(GetRenderRT(), "D://Test.png");
                    GrabPassFeature.GrabStat--;
                }
                if (GrabPassFeature.GrabStat <= 0) GrabPassFeature.GrabStat = -1;
            }
        }
        //------------------------------------------------------
        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
//             if (!m_Setting.isEnabled() || !m_bActived)
//             {
//                 cmd.ReleaseTemporaryRT(m_nGrabRT);
//                 return;
//             }
        }
        //------------------------------------------------------
        bool OnTimerTick(int hHandle, IUserData param)
        {
            if(!m_bActived)
            {
                if (m_pGrabRT) RenderTexture.ReleaseTemporary(m_pGrabRT);
                m_pGrabRT = null;
             //   Shader.SetGlobalTexture(m_GrabSetting.textureID, Data.GlobalDefaultResources.GrayTexture);
                GrabPassFeature.CullingMask = 0;
                GrabPassFeature.GrabStat = -1;
            }
            return false;
        }
    }
}
#endif