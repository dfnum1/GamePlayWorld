/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	WaringParticleController
作    者:	HappLI
描    述:	预警特效
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public class WaringParticleController : ParticleController
    {
        public string propertyName = null;
        public bool bBlock = true;
        public bool bShared = true;

        private float m_fTime = 0;

        public override void OnPoolReady()
        {
            base.OnPoolReady();
        }
        //------------------------------------------------------
        public override void OnPoolStart()
        {
            base.OnPoolStart();
            m_fTime = 0;
        }
        //------------------------------------------------------
        protected override void LateUpdate()
        {
            if (m_fLiftTime > 0)
            {
                m_fTime += Time.deltaTime;
                SetFloat(propertyName, Mathf.Clamp01(m_fTime / m_fLiftTime), bBlock, bBlock);
            }
            base.LateUpdate();
        }
    }
}
