#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderPass
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public abstract class APostRenderPass : ScriptableRenderPass
    {
        protected string m_ProfilerTag;
        protected CustomSettings m_Setting;
        List<ShaderTagId> m_ShaderTagIdList = null;
        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        //------------------------------------------------------
        protected APostRenderPass()
        {

        } 
        //------------------------------------------------------
        public abstract EPostPassType GetPostPassType();
        //------------------------------------------------------
        public bool IsActived()
        {
            return m_Setting!=null && m_Setting.isEnabled();
        }
        //------------------------------------------------------
        public CustomSettings GetSetting()
        {
            return m_Setting;
        }
        //------------------------------------------------------
        protected void OnInit(CustomSettings setting)
        {
            m_Setting = setting;
            if(m_Setting!=null)
            {
                m_ProfilerTag = m_Setting.passName;
            }
            m_ShaderTagIdList = null;
            //m_FilteringSettings = new FilteringSettings(new RenderQueueRange(m_Setting.lowerRenderQueue, m_Setting.upperRenderQueue), m_Setting.layerMask);
            //m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            //if (m_Setting.ShaderTags!=null)
            //{
            //    m_ShaderTagIdList = new List<ShaderTagId>(m_Setting.ShaderTags.Length);
            //    for (int i = 0; i < m_Setting.ShaderTags.Length; ++i)
            //    {
            //        if(string.IsNullOrEmpty(m_Setting.ShaderTags[i])) continue;
            //        m_ShaderTagIdList.Add(new ShaderTagId(m_Setting.ShaderTags[i]));
            //    }
            //}
            URPPostWorker.OnCreateRenderPass(this);
        }
        //------------------------------------------------------
        public void SetProfilerTag(string Name)
        {
            m_ProfilerTag = Name;
        }
        //------------------------------------------------------
        public virtual void Setup(RenderTargetIdentifier src)
        {
            URPPostWorker.OnRenderPassSetup(this, src);
        }
        //------------------------------------------------------
        protected void OnExecute(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cullingMask == 0)
                return;

            if (m_ShaderTagIdList != null)
            {
                var sortFlags = (m_Setting.IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
            }
            URPPostWorker.OnRenderPassExecude(this, cmd, context, ref renderingData);
        }
        //------------------------------------------------------
        public override void FrameCleanup(CommandBuffer cmd)
        {
            URPPostWorker.OnRenderPassFrameCleanup(this, cmd);
            base.FrameCleanup(cmd);
        }
    }
}
#endif