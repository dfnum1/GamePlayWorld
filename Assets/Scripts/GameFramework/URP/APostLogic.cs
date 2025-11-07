#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderPass
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public abstract class APostLogic
    {
        int m_nHashCode = 0;
        protected APostRenderPass m_RenderPass;
        //------------------------------------------------------
        public virtual void Awake()
        {

        }
        //------------------------------------------------------
        public void SetCode(int hashCode)
        {
            m_nHashCode = hashCode;
        }
        //------------------------------------------------------
        public int GetCode()
        {
            return m_nHashCode;
        }
        //------------------------------------------------------
        public void SetPass(APostRenderPass pass)
        {
            m_RenderPass = pass;
        }
        //------------------------------------------------------
        public virtual void Destroy()
        {
            m_RenderPass = null;
        }
        //------------------------------------------------------
        public virtual void ClearReset()
        {

        }
        //------------------------------------------------------
        public virtual void Setup(APostRenderPass pass, RenderTargetIdentifier source)
        {
        }
        //------------------------------------------------------
        public virtual void Update(float fFrame)
        {

        }
        //------------------------------------------------------
        public virtual void Excude(APostRenderPass pass, CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {

        }
        //------------------------------------------------------
        public virtual void FrameCleanup(APostRenderPass pass, CommandBuffer cmd)
        {

        }
    }
}
#endif