#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	SkillSystem
作    者:	HappLI
描    述:	技能系统
*********************************************************************/
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
#endif

namespace Framework.Core
{
    public enum ESkillType
    {
        eInitiative=0, //主动技
        ePassivity =1 //被动技
    }
    public class SkillSystem : TypeObject
    {
        private Actor m_pOwner = null;

        private LinkedList<Skill> m_vInitiatives;
        private LinkedList<Skill> m_vPassivitys;

        private LinkedList<Skill> m_vTodoList = null;

        List<AWorldNode> m_vLockTargets = null;
        private bool m_bAutoSkill = true;
        Skill m_pCurrentSkill = null;
        byte m_nNoActionSkillClear = 0;
        Skill m_pNoActionCurrentSkill = null;
        //-----------------------------------------------------
        public SkillSystem()
        {
            m_pOwner = null;
            m_bAutoSkill = true;
            m_vTodoList = new LinkedList<Skill>();
        }
        //-----------------------------------------------------
        public AFramework GetGameModule()
        {
            if (m_pOwner != null)
                return m_pOwner.GetGameModule();
            return null;
        }
        //-----------------------------------------------------
        public void SetActor(Actor pActor)
        {
            m_pOwner = pActor;
        }
        //-----------------------------------------------------
        public Actor GetActor()
        {
            return m_pOwner;
        }
        //-----------------------------------------------------
        public long GetCurrentTime()
        {
            return m_pOwner.GetGameModule().GetRunTime();
        }
        //-----------------------------------------------------
        public List<AWorldNode> GetLockTargets(bool isEmptyReLock = true)
        {
            return m_vLockTargets;
        }
        //-----------------------------------------------------
        public void AddLockTarget(AWorldNode pNode, bool bClear = false)
        {
            if (m_vLockTargets == null) m_vLockTargets = new List<AWorldNode>(2);
            if (bClear) m_vLockTargets.Clear();
            if (!m_vLockTargets.Contains(pNode))
                m_vLockTargets.Add(pNode);
        }
        //-----------------------------------------------------
        public void ClearLockTargets()
        {
            if (m_vLockTargets != null)
                m_vLockTargets.Clear();
        }
        //-----------------------------------------------------
        public void AddSkill(Skill pSkill, ESkillType eType)
        {
            if (pSkill == null) return;
            pSkill.SetSkillSystem(this);
            if (eType == ESkillType.eInitiative)
            {
                if (m_vInitiatives == null) m_vInitiatives = new LinkedList<Skill>();
                if (!m_vInitiatives.Contains(pSkill))
                    m_vInitiatives.AddLast(pSkill);
            }
            else
            {
                if (m_vPassivitys == null) m_vPassivitys = new LinkedList<Skill>();
                if (!m_vPassivitys.Contains(pSkill))
                    m_vPassivitys.AddLast(pSkill);
            }
            pSkill.OnInit();
        }
        //--------------------------------------------------------
        internal void OnActionEndState(ActorAction pState)
        {
            if(m_pCurrentSkill!=null)
            {
                if(m_pCurrentSkill.GetActionType() == pState.type && m_pCurrentSkill.GetActionTag() == pState.actionTag)
                {
                    m_pCurrentSkill = null;
                }
            }
        }
        //--------------------------------------------------------
        internal void OnActionStartState(ActorAction pState)
        {
        }
        //-----------------------------------------------------
        public Skill GetCurrentSkill(bool checkNoAction =true)
        {
            if (m_pCurrentSkill != null) return m_pCurrentSkill;
            if(checkNoAction) return m_pNoActionCurrentSkill;
            return null;
        }
        //-----------------------------------------------------
        public bool DoSkill(Skill pSkill)
        {
            if (pSkill == null)
                return false;
            var pAction = m_pOwner.GetAction(pSkill.GetActionType(), pSkill.GetActionTag());
            bool bFind = pSkill.DoLockTarget();
            if (bFind)
            {
                if (pAction != null) m_pOwner.StartActionState(pAction, pStateParam:pSkill);
                m_pOwner.OnStartState(pSkill);
                pSkill.SetUsed();
                if (pAction != null) m_pCurrentSkill = pSkill;
                else
                {
                    m_nNoActionSkillClear = 0;
                    m_pNoActionCurrentSkill = pSkill;
                }
            }
            return bFind;
        }
        //-----------------------------------------------------
        public void DoSkill()
        {
            if (m_vTodoList == null)
                return;
            Skill doSkill = null;
            uint priority = 0;
            ActorAction pAction = null;
            for (var node = m_vTodoList.First; node != null; node = node.Next)
            {
                Skill skill = node.Value;
                var action = m_pOwner.GetAction(skill.GetActionType(), skill.GetActionTag());
                if (action == null)
                {
                    DoSkill(skill);
                    continue;
                }
                if (action.priority >= m_pOwner.GetCurrentPlayActionStatePriority(action.layer))
                {
                    if (action != null && action.priority >= priority)
                    {
                        priority = action.priority;
                        doSkill = skill;
                        pAction = action;
                    }
                }
            }
            if (doSkill != null)
            {
                DoSkill(doSkill);
            }
            m_vTodoList.Clear();
        }
        //-----------------------------------------------------
        public void Update(FFloat fFrame)
        {
            if(m_vInitiatives!=null)
            {
                for(var node = m_vInitiatives.First; node !=null; node = node.Next)
                {
                    Skill skill = node.Value;
                    skill.Update(fFrame);
                    if (!skill.IsActived())
                        continue;
                    if (skill.CanTrigger() && !m_vTodoList.Contains(skill))
                    {
                        //! 先直接使用，且跳出检测
                        //     m_pOwner.GetActorGraph()
                        //      m_pOwner.StartSkill();
                        m_vTodoList.AddLast(skill);
                    }
                }
            }
            if (m_vPassivitys != null)
            {
                for (var node = m_vPassivitys.First; node != null; node = node.Next)
                {
                    Skill skill = node.Value;
                    skill.Update(fFrame);
                    if (!skill.IsActived())
                        continue;
                    if (skill.CanTrigger() && !m_vTodoList.Contains(skill))
                    {
                        //! 先直接使用，且跳出检测
                        m_vTodoList.AddLast(skill);
                    }
                }
            }

            if(m_bAutoSkill)
            {
                DoSkill();
            }

            if(m_pNoActionCurrentSkill!=null)
            {
                m_nNoActionSkillClear++;
                if(m_nNoActionSkillClear>2)
                {
                    m_pNoActionCurrentSkill = null;
                    m_nNoActionSkillClear = 0;
                }
            }
        }
        //-----------------------------------------------------
        public void EnableAutoSkill(bool bAuto)
        {
            m_bAutoSkill = bAuto;
        }
        //-----------------------------------------------------
        public override void Destroy()
        {
            if (m_vInitiatives != null)
            {
                for (var node = m_vInitiatives.First; node != null; node = node.Next)
                {
                    node.Value.Destroy();
                }
                m_vInitiatives.Clear();
            }
            if (m_vPassivitys != null)
            {
                for (var node = m_vPassivitys.First; node != null; node = node.Next)
                {
                    node.Value.Destroy();
                }
                m_vPassivitys.Clear();
            }
            if (m_vTodoList != null) m_vTodoList.Clear();
            m_pOwner = null;
            if (m_vLockTargets != null) m_vLockTargets.Clear();
            m_bAutoSkill = true;
            m_pCurrentSkill = null;
            m_nNoActionSkillClear = 0;
            m_pNoActionCurrentSkill = null;
        }
    }
}
#endif