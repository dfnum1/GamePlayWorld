/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	EventSystemTrigger
作    者:	HappLI
描    述:	
*********************************************************************/

using UnityEngine;
using System.Collections.Generic;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
#if USE_SERVER
using GameObject = ExternEngine.GameObject;
using Transform = ExternEngine.Transform;
#endif

namespace Framework.Core
{
    public class EventSystem : AModule
    {
        public delegate void OnEventDelegage(BaseEvent param, ref uint useFlag);
        EventPool m_EventPool = new EventPool();
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_EventPool = new EventPool();
        }
        //------------------------------------------------------
        public static void LogTo(BaseEvent param, Actor pActor = null)
        {
#if UNITY_EDITOR
            if (!Base.ConfigUtil.bEventTriggerDebug) return;
            if (pActor != null)
                Base.Logger.Info("TriggerEvent:" + pActor.ToString());
            Base.Logger.Info("type:" + param.GetEventType().ToString() + "   param:" + param.ToString());
#endif
        }
        //------------------------------------------------------
        private Actor m_pTriggerActor = null;
        public OnEventDelegage OnEventCallback = null;
        public FVector3 TriggerEventPos = FVector3.zero;
        public FVector3 TriggerEventRealPos = FVector3.zero;
        public FVector3 TriggerActorDir = FVector3.right;
        public bool bCalcAxisOffset = false;
        public int nTriggerActorID = 0;
        public int nAttackGroup = 0;
        public uint nAttackGroupFiler = 0xffffffff;
        public AInstanceAble pInstnaceAble = null;
        public GameObject pGameObject = null;
        public IInstanceSpawner InstanceSpawner = null;
        public bool bUsedEventOffset = true;
        protected FVector3 m_EventOffset = FVector3.zero;
        protected uint m_nUsedFlag = 0xffffffff;
        public StateParam TriggerActorActionStateParam = null;
        protected AWorldNode m_pParentNode = null;
        public AWorldNode pParentNode
        {
            get { return m_pParentNode; }
            set
            {
                m_pParentNode = value;
            }
        }

        protected AWorldNode m_pTargetNode = null;
        public AWorldNode pTargetNode
        {
            get { return m_pTargetNode; }
            set
            {
                m_pTargetNode = value;
            }
        }

        protected IUserData m_ATuserData = null;
        public IUserData ATuserData
        {
            get { return m_ATuserData; }
            set
            {
                m_ATuserData = value;
                if (m_ATuserData != null && m_ATuserData is Actor)
                    m_pTriggerActor = m_ATuserData as Actor;
                if(m_pTriggerActor!=null)
                {
                }
                AWorldNode pNode = m_ATuserData as AWorldNode;
                if (pNode != null)
                {
                    nTriggerActorID = pNode.GetInstanceID();
                    nAttackGroup = pNode.GetAttackGroup();
                    nAttackGroupFiler = (uint)(~(1 << pNode.GetAttackGroup()));

                    TriggerEventPos = pNode.GetPosition();
                    TriggerEventRealPos = pNode.GetPosition();
                    TriggerActorDir = pNode.GetDirection();
                }

                if (m_ATuserData != null && m_ATuserData is StateParam)
                {
                    this.TriggerActorActionStateParam = m_ATuserData as StateParam;
                    if (m_ATuserData != null && m_ATuserData is HitDamageParam)
                    {
                        var hitDmg = m_ATuserData as HitDamageParam;
                        m_ATuserData = hitDmg.GetAttacter();
                        if (this.m_pTriggerActor == null)
                        {
                            this.m_pTriggerActor = hitDmg.GetAttacter() as Actor;
                            m_nUsedFlag |= (uint)EEventParamFlag.UsedActor;
                            this.m_pTriggerActor = hitDmg.GetAttacter() as Actor;
                            nTriggerActorID = m_pTriggerActor.GetInstanceID();
                            nAttackGroup = m_pTriggerActor.GetAttackGroup();
                            nAttackGroupFiler = (uint)(~(1 << m_pTriggerActor.GetAttackGroup()));

                            TriggerEventPos = m_pTriggerActor.GetPosition();
                            TriggerEventRealPos = m_pTriggerActor.GetPosition();
                            TriggerActorDir = m_pTriggerActor.GetDirection();
                        }
                        if(this.m_pParentNode == null)
                        {
                            m_pParentNode = hitDmg.GetParentAttacker();
                        }
                        if(this.m_pTargetNode == null)
                        {
                            this.m_pTargetNode = hitDmg.GetTarget();
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public uint TriggerActionTag
        {
            get
            {
                return 0xffffffff;
            }
        }
        //------------------------------------------------------
        public void SetTriggerActor(Actor pActor)
        {
            m_pTriggerActor = pActor;
        }
        //------------------------------------------------------
        protected Actor GetTriggerActor()
        {
            if ((m_nUsedFlag & (int)EEventParamFlag.UsedActor) != 0) return m_pTriggerActor;
            return null;
        }
        //------------------------------------------------------
        protected Transform GetTriggerTransform()
        {
#if !USE_SERVER
            AWorldNode pNode = GetTriggerNode();
            if (pNode != null && pNode.GetObjectAble() != null) return pNode.GetObjectAble().GetTransorm();
            AInstanceAble pAble = GetTriggerInstance();
            if (pAble != null) return pAble.GetTransorm();
            if(pGameObject) return pGameObject.transform;
#endif
            return null;
        }
        //------------------------------------------------------
        protected AWorldNode GetTriggerNode()
        {
            if ((m_nUsedFlag & (int)EEventParamFlag.UsedActor) != 0) return m_ATuserData as AWorldNode;
            return null;
        }
        //------------------------------------------------------
        protected AInstanceAble GetTriggerInstance()
        {
            if ((m_nUsedFlag & (int)EEventParamFlag.UsedInstance) != 0) return pInstnaceAble;
            return null;
        }
        //------------------------------------------------------
        public virtual void Begin()
        {
            nTriggerActorID = 0;
            TriggerActorActionStateParam = null;
            m_pTriggerActor = null;
            TriggerEventPos = FVector3.zero;
            TriggerEventRealPos = FVector3.zero;
            nAttackGroup = 0;
            nAttackGroupFiler = 0xffffffff;
            pInstnaceAble = null;
            ATuserData = null;
            m_pParentNode = null;
            m_pTargetNode = null;
            pGameObject = null;
            InstanceSpawner = null;
            OnEventCallback = null;
            m_nUsedFlag = 0xffffffff;
            bCalcAxisOffset = false;
            bUsedEventOffset = true;
            m_EventOffset = FVector3.zero;
        }
        //------------------------------------------------------
        public virtual void End()
        {
            nTriggerActorID = 0;
            TriggerActorActionStateParam = null;
            m_pTriggerActor = null;
            TriggerEventPos = FVector3.zero;
            TriggerEventRealPos = FVector3.zero;
            nAttackGroup = 0;
            nAttackGroupFiler = 0xffffffff;
            pInstnaceAble = null;
            ATuserData = null;
            m_pParentNode = null;
            pGameObject = null;
            InstanceSpawner = null;
            OnEventCallback = null;
            m_nUsedFlag = 0xffffffff;
            bCalcAxisOffset = false;
            m_EventOffset = FVector3.zero;
            bUsedEventOffset = true;
        }
        //------------------------------------------------------
        public virtual void OnStopEvent(AWorldNode pTrigger, BaseEvent param)
        {

        }
        //------------------------------------------------------
        public virtual void CollectPreload(BaseEvent param, List<string> vFiles, HashSet<string> vAssets)
        {

        }
        //------------------------------------------------------
        public virtual bool CanTrigger(BaseEvent param)
        {
            return true;
        }
        //------------------------------------------------------
        protected virtual void PrepareEventDatas(BaseEvent pEvent)
        {

        }
        //------------------------------------------------------
        public void OnTriggerEvent(BaseEvent pEvent)
        {
            if (pEvent == null ) return;
            pEvent.BeginEvent();
            if (!CanTrigger(pEvent))
            {
                pEvent.EndEvent();
                return;
            }

            m_nUsedFlag = 0xffffffff;
            if (OnEventCallback != null) OnEventCallback(pEvent, ref m_nUsedFlag);
            PrepareEventDatas(pEvent);

            if (bUsedEventOffset)
            {
                m_EventOffset = pEvent.GetOffset();
                if (bCalcAxisOffset && m_EventOffset.sqrMagnitude > 0)
                {
                    m_EventOffset = FVector3.zero;
                    FVector3 vRight = FVector3.Cross(TriggerActorDir, FVector3.up);
                    m_EventOffset += TriggerActorDir * pEvent.GetOffset().z;
                    m_EventOffset += vRight * pEvent.GetOffset().x;
                    m_EventOffset.y = (FFloat)pEvent.GetOffset().y;
                }
            }
            else m_EventOffset = FVector3.zero;
            
            pEvent.OnExecute(this);

            pEvent.EndEvent();
            LogTo(pEvent, GetTriggerActor());
        }
        //------------------------------------------------------
        public BaseEvent MallocEvent(int eventType)
        {
            BaseEvent pEvent = m_EventPool.MallocEvent(eventType);
            if (pEvent == null)
                GetFramework().OnMallocEvent(eventType);
            return pEvent;
        }
        //------------------------------------------------------
        public void FreeEvent(BaseEvent pEvent)
        {
            m_EventPool.FreeEvent(pEvent);
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            End();
        }
    }
}

