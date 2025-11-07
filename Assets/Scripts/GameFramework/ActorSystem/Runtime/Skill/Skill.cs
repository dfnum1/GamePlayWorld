#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	Skill
作    者:	HappLI
描    述:	技能系统-单个技能
*********************************************************************/
using Framework.Data;
using System.Collections.Generic;
using UnityEngine;


#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
#endif

namespace Framework.Core
{
    public abstract class Skill : AActorStateInfo
    {
        protected uint        m_nSkillID = 0;
        protected uint        m_nLevel = 0;

        protected BaseData    m_pConfigData = null;

        protected long        m_LastTriggerTime = 0;
        protected int         m_nTriggerCount  =0;

        protected bool        m_bActived = false;
        protected SkillSystem m_pOwner = null;
        //-----------------------------------------------------
        public Skill()
        {
            m_pOwner = null;
        }
        //-----------------------------------------------------
        public void SetConfigData(BaseData pConfigData)
        {
            if (m_pConfigData == pConfigData)
                return;
            m_pConfigData = pConfigData;
            OnConfigData();
        }
        //-----------------------------------------------------
        protected virtual void OnConfigData() { }
        //-----------------------------------------------------
        public AFramework GetGameModule()
        {
            if (m_pOwner != null)
                return m_pOwner.GetGameModule();
            return null;
        }
        //-----------------------------------------------------
        public Actor GetActor()
        {
            if (m_pOwner != null)
                return m_pOwner.GetActor();
            return null;
        }
        //-----------------------------------------------------
        internal void SetSkillSystem(SkillSystem pSystem)
        {
            m_pOwner = pSystem;
        }
        //-----------------------------------------------------
        public virtual void OnInit()
        {
            m_LastTriggerTime = 0;
            m_nTriggerCount = 0;
        }
        //-----------------------------------------------------
        public void Update(FFloat fFrame)
        {
            if (!m_bActived)
                return;
            OnUpdate(fFrame);
        }
        //-----------------------------------------------------
        public bool IsActived()
        {
            return m_bActived;
        }
        //-----------------------------------------------------
        public void SetActived(bool bActive)
        {
            m_bActived = bActive;
        }
        //-----------------------------------------------------
        public virtual void SetUsed()
        {
            m_nTriggerCount++;
            m_LastTriggerTime = GetGameModule().GetRunTime();
        }
        //-----------------------------------------------------
        public float GetRuntimeNormalCD()
        {
            if (GetConfigCD() <= 0)
                return 0;
            long nCurTime = GetGameModule().GetRunTime();
            long nDelta = nCurTime - m_LastTriggerTime;
            if (nDelta < 0) return 0;
            if (nDelta >= GetConfigCD()) return 0;
            return Mathf.Clamp01(nDelta / GetConfigCD());
        }
        //-----------------------------------------------------
        public float GetRuntimeCD()
        {
            if (GetConfigCD() <= 0)
                return 0;
            long nCurTime = GetGameModule().GetRunTime();
            long nDelta = nCurTime - m_LastTriggerTime;
            if (nDelta < 0) nDelta = 0;
            if (nDelta >= GetConfigCD()) return 0;
            float fCD = nDelta / 1000.0f;
            if (fCD < 0) fCD = 0;
            return fCD;
        }
        //-----------------------------------------------------
        public void SetLevel(uint nLevel)
        {
            m_nLevel = nLevel;
        }
        //-----------------------------------------------------
        public BaseData GetConfigData()
        {
            return m_pConfigData;
        }
        //-----------------------------------------------------
        public bool CanTrigger()
        {
            if (!m_bActived || m_nLevel<=0) return false;
            if (GetRuntimeCD() > 0) return false;
            return CheckCanTrigger();
        }
        //-----------------------------------------------------
        public override void AddLockTarget(AWorldNode pNode, bool bClear = false)
        {
            m_pOwner.AddLockTarget(pNode,bClear);
        }
        //-----------------------------------------------------
        public override void ClearLockTargets()
        {
            m_pOwner.ClearLockTargets();
        }
        //------------------------------------------------------
        public override List<AWorldNode> GetLockTargets( bool isEmptyReLock = true)
        {
            return m_pOwner.GetLockTargets(isEmptyReLock);
        }
        //-----------------------------------------------------
        public abstract bool DoLockTarget();
        //-----------------------------------------------------
        protected virtual void OnUpdate(FFloat fFrame) { }
        //-----------------------------------------------------
        protected abstract bool CheckCanTrigger();
        //-----------------------------------------------------
        public abstract long GetConfigCD();
        //-----------------------------------------------------
        public abstract EActionStateType GetActionType();
        //-----------------------------------------------------
        public abstract uint GetActionTag();
        //-----------------------------------------------------
        public abstract int GetAttrFormulaType();
        //-----------------------------------------------------
        public override void Destroy()
        {
            m_pOwner = null;
            m_pConfigData = null;
            m_bActived = false;
            m_nSkillID = 0;
            m_LastTriggerTime = 0;
            m_nTriggerCount = 0;
            m_nLevel = 0;
        }
    }
}
#endif