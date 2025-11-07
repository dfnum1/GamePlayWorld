
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	Actor
作    者:	HappLI
描    述:	
*********************************************************************/
#if USE_ACTORSYSTEM
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;
using Framework.Plugin.AT;
#if USE_CUTSCENE
using Framework.Cutscene.Runtime;
using Framework.CutsceneAT.Runtime;
using UnityEngine.UIElements;
using Framework.Base;
#endif
namespace Framework.Core
{
    [ATExportMono("World/Actor", EGlobalType.None, true, "", marcoDefine = "USE_ACTORSYSTEM")]
    public class Actor : AWorldNode
    {
        public System.Action<AActorStateInfo, EActorStateStatus> OnStateInfoCallback;
        ActorParameter m_pActorParameter;
        EActorGroundType m_eLastGroundType = EActorGroundType.Ground;
        EActorGroundType m_eGroundType = EActorGroundType.Ground;
        Dictionary<System.Type, AActorAgent> m_mTypeAgents = null;
        List<AActorAgent> m_vAgents = null;
        ActorGraph m_pGraph = null;
        SkillSystem m_pSkillSystem = null;

        protected bool m_bTurning = false;
        protected FVector2 m_fTurningDelta = FVector2.zero;
        FQuaternion m_OriginRotation;
        FQuaternion m_TargetRotation;
        FQuaternion m_CurRotation;

        private int m_nRVOTestCnt = 0;
        private byte m_nRVOColliderAdjust = 0;
        protected FFloat m_fRVOPushForce;
        protected FFloat m_fRVORadius;

        protected FVector3 m_Position = FVector3.zero;
        protected FVector3 m_PositionOffset = FVector3.zero;
        protected FVector3 m_Speed = FVector3.zero;
        protected FVector3 m_RVOVelocity = FVector3.zero;
        protected FFloat m_fGravity = 0.0f;
        protected bool m_bUseGravity = true;
        protected FFloat m_fFraction = 0.0f;
        private FFloat m_fStepHeight = ConstDef.STEP_HEIGHT_LOWER;
        private FFloat m_fDurationHit = 0;

        byte m_nLastCollisionFlag = 0;
        byte m_nCollisionFlag = 0;

        private Framework.Plugin.AT.AgentTree m_pAT = null;
        //--------------------------------------------------------
        public Framework.Plugin.AT.AgentTree AT
        {
            get { return m_pAT; }
        }
        //--------------------------------------------------------
        public Actor(AFramework pGame) : base(pGame)
        {
            Reset();
        }
        //--------------------------------------------------------
        protected virtual void Reset()
        {
            m_Position = FVector3.zero;
            m_PositionOffset = FVector3.zero;
            m_Speed = FVector3.zero;
            m_RVOVelocity = FVector3.zero;
            m_fTurningDelta = FVector2.zero;
            m_bTurning = false;
            m_OriginRotation = FQuaternion.identity;
            m_TargetRotation = FQuaternion.identity;
            m_CurRotation = FQuaternion.identity;

            m_eGroundType = EActorGroundType.Ground;
            m_eLastGroundType = m_eGroundType;

            m_nRVOColliderAdjust = 0;
            m_nRVOTestCnt = 0;
            m_fRVOPushForce = ExternEngine.FFloat.one;
            m_fRVORadius = ExternEngine.FFloat.one;

            m_bUseGravity = true;
            m_fGravity = Base.ConstDef.GTRAVITY_VALUE;
            m_fStepHeight = ConstDef.STEP_HEIGHT_LOWER;
            m_fFraction = 0;
            m_nCollisionFlag = m_nLastCollisionFlag;

            m_fDurationHit = 0;

            OnStateInfoCallback = null;
        }
        //--------------------------------------------------------
        protected override void InnerCreated()
        {
            if (m_pActorParameter == null) m_pActorParameter = new ActorParameter(this);
            if (m_pSkillSystem == null) m_pSkillSystem = TypeInstancePool.Malloc<SkillSystem>();
            m_pSkillSystem.SetActor(this);
            GetAgent<ActorGraphicAgent>(true);
            Reset();
        }
        //--------------------------------------------------------
        public Framework.Plugin.AT.AgentTree GetAgentTree()
        {
            return m_pAT;
        }
        //--------------------------------------------------------
        public ActorParameter GetActorParameter()
        {
            if (m_pActorParameter == null) m_pActorParameter = new ActorParameter(this);
            return m_pActorParameter;
        }
        //--------------------------------------------------------
        public ActorGraphData GetGraphData()
        {
            return GetActorGraph().GetGraphData();
        }
        //--------------------------------------------------------
        public ActorGraph GetActorGraph()
        {
            if (m_pGraph == null)
            {
                m_pGraph = TypeInstancePool.Malloc<ActorGraph>();
                m_pGraph.Init(this);
                m_pGraph.AddStartActionCallback(OnActionStartState);
                m_pGraph.AddEndActionCallback(OnActionEndState);
            }
            return m_pGraph;
        }
        //--------------------------------------------------------
        public bool LoadActorGraph(string actorGraph)
        {
            return GetActorGraph().LoadGraph(actorGraph, OnLoadActorGraph);
        }
        //--------------------------------------------------------
        void OnLoadActorGraph(ActorGraphData graphData)
        {
            GetAgent<ActorGraphicAgent>(true).OnLoadActorGraphData(graphData);
        }
        //--------------------------------------------------------
        protected override void Construction()
        {
            OnStateInfoCallback = null;
            base.Construction();
        }
        //--------------------------------------------------------
        public void AddCallback(IActorAttrDirtyCallback callback)
        {
            GetActorParameter().AddCallback(callback);
        }
        //--------------------------------------------------------
        public void RemoveCallback(IActorAttrDirtyCallback callback) 
        {
            GetActorParameter().RemoveCallback(callback);
        }
        //--------------------------------------------------------
        public void SetAttrs(byte[] attiTypes, FFloat[] values) 
        {
            GetActorParameter().SetAttrs(attiTypes, values);
        }
        //--------------------------------------------------------
        public void SetAttrs(byte[] attiTypes, int[] values)
        {
            GetActorParameter().SetAttrs(attiTypes, values);
        }
        //--------------------------------------------------------
        public void SetAttr(byte type, FFloat value)
        {
            GetActorParameter().SetAttr(type, value);
        }
        //--------------------------------------------------------
        public FFloat GetAttr(byte type, float defVal = 0) 
        {
            return GetActorParameter().GetAttr(type, defVal);
        }
        //--------------------------------------------------------
        public void RemoveAttr(byte type)
        {
            GetActorParameter().RemoveAttr(type);
        }
        //--------------------------------------------------------
        public void AppendAttrs(byte[] attiTypes, FFloat[] values)
        {
            GetActorParameter().AppendAttrs(attiTypes, values);
        }
        //--------------------------------------------------------
        public void AppendAttr(byte type, FFloat value) 
        {
             GetActorParameter().AppendAttr(type, value);
        }
        //--------------------------------------------------------
        public void SubAttrs(byte[] attiTypes, FFloat[] values)
        {
            GetActorParameter().SubAttrs(attiTypes, values);
        }
        //--------------------------------------------------------
        public void SubAttr(byte type, FFloat value, bool bLowerZero = false) 
        {
            GetActorParameter().SubAttr(type, value, bLowerZero);
        }
        //--------------------------------------------------------
        public void ClearAttrs()
        {
            GetActorParameter().ClearAttrs();
        }
        //--------------------------------------------------------
        public ActorAction GetAction(EActionStateType eType, uint nTag)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return null;
            return pAgent.GetActorAction(eType, nTag);
        }
        //--------------------------------------------------------
        public ActorAction GetAction(string actionName)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return null;
            return pAgent.GetActorAction(actionName);
        }
        //--------------------------------------------------------
        public uint GetCurrentPlayActionStatePriority(uint layer)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return 0;

            return pAgent.GetCurrentPlayActionStatePriority(layer);
        }
        //--------------------------------------------------------
        public ActorAction GetCurrentPlayActionState(uint layer=0)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return null;

            return pAgent.GetCurrentPlayActionState(layer);
        }
        //--------------------------------------------------------
        public override StateParam GetStateParam()
        {
            if (m_pSkillSystem != null) return m_pSkillSystem.GetCurrentSkill();
            return null;
        }
        //--------------------------------------------------------
        public bool IsInAction(EActionStateType eType)
        {
            if(m_pGraph!=null)
            {
                if (m_pGraph.IsInAction(eType))
                    return true;
            }
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return false;
            return pAgent.IsInAction(eType);
        }
        //--------------------------------------------------------
        public SkillSystem GetSkillSystem()
        {
            if (m_pSkillSystem == null) m_pSkillSystem = TypeInstancePool.Malloc<SkillSystem>();
            m_pSkillSystem.SetActor(this);
            return m_pSkillSystem;
        }
        //--------------------------------------------------------
        public bool IsAttacking()
        {
            if (m_pSkillSystem == null) return false;
            return m_pSkillSystem.GetCurrentSkill(false) != null;
        }
        //--------------------------------------------------------
        public bool IsHiting()
        {
            return m_fDurationHit > 0.0f;
        }
        //--------------------------------------------------------
        public void SetHitDuration(FFloat fDuration)
        {
            m_fDurationHit = fDuration;
        }
        //--------------------------------------------------------
        [ATMethod]
        public void StartActionState(EActionStateType eType, uint nTag =0, bool bForce = false, StateParam pStateParam = null)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            var action = pAgent.GetActorAction(eType, nTag);
            if (action == null)
                return;
            if(action.GetPlayCutscene()!=null)
            {
                GetActorGraph().Play(action.GetPlayCutscene(), action, pStateParam);
                return;
            }
            pAgent.PlayAnimation(eType, nTag, bForce);
        }
        //--------------------------------------------------------
        public void StartActionState(uint nActionTypeAndTag, bool bForce = false, StateParam pStateParam = null)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            var action = pAgent.GetActorAction(nActionTypeAndTag);
            if (action == null)
                return;
            if (action.GetPlayCutscene() != null)
            {
                GetActorGraph().Play(action.GetPlayCutscene(), action);
                return;
            }
            pAgent.PlayAnimation(action, 0.1f, bForce);
        }
        //--------------------------------------------------------
        [ATMethod]
        public void StartActionState(ActorAction pAction, float blendTime = 0.1f, bool bForce = false, StateParam pStateParam = null)
        {
            if (pAction == null)
                return;
            if (pAction.GetPlayCutscene() != null)
            {
                GetActorGraph().Play(pAction.GetPlayCutscene(), pAction, pStateParam);
                return;
            }
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            pAgent.PlayAnimation(pAction, blendTime, bForce);
        }
        //--------------------------------------------------------
        [ATMethod]
        public void StopActionState(EActionStateType eType, uint nTag =0)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            var action = pAgent.GetActorAction(eType, nTag);
            if (action!=null && action.GetPlayCutscene() != null)
            {
                GetActorGraph().Stop(action);
                return;
            }

            pAgent.StopAnimation(eType, nTag);
        }
        //--------------------------------------------------------
        void OnActionEndState(ActorAction pState)
        {
            if (m_pSkillSystem != null)
                m_pSkillSystem.OnActionEndState(pState);
        }
        //--------------------------------------------------------
        void OnActionStartState(ActorAction pState)
        {
            if (m_pSkillSystem != null)
                m_pSkillSystem.OnActionStartState(pState);
        }
        //--------------------------------------------------------
        [ATMethod]
        public void RemoveActionState(EActionStateType eType, uint nTag = 0)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            pAgent.RemoveActionState(eType, nTag);
        }
        //--------------------------------------------------------
        [ATMethod]
        public void SetIdleType(EActionStateType eType, uint tag = 0)
        {
            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
            if (null == pAgent)
                return;
            pAgent.SetIdleType(eType, tag);
        }
        //------------------------------------------------------
        internal void OnStartState(AActorStateInfo stateInfo)
        {
            if (OnStateInfoCallback == null) return;
            OnStateInfoCallback(stateInfo, EActorStateStatus.eBegin);
        }
        //------------------------------------------------------
        internal void OnTickingState(AActorStateInfo stateInfo)
        {
            if (OnStateInfoCallback == null) return;
            OnStateInfoCallback(stateInfo, EActorStateStatus.eTicking);
        }
        //------------------------------------------------------
        internal void OnEndState(AActorStateInfo stateInfo)
        {
            if (OnStateInfoCallback == null) return;
            OnStateInfoCallback(stateInfo, EActorStateStatus.eEnd);
        }
        //------------------------------------------------------
        public override FFloat GetPhysicRadius()
        {
            return m_fRVORadius * m_Transform.GetScaleMag();
        }
        //------------------------------------------------------
        public void SetRVOTest(int testCnt)
        {
            m_nRVOTestCnt = testCnt;
        }
        //------------------------------------------------------
        public FFloat GetProjectAppendSpeed()
        {
            return 0.0f;
        }
        //------------------------------------------------------
        public bool HasSpeedXZ()
        {
#if USE_FIXEDMATH
            return FMath.Abs(m_Speed.x) > 0.1f || FMath.Abs(m_Speed.z) > 0.1f;
#else
            return System.Math.Abs(m_Speed.x) > 0.1f || System.Math.Abs(m_Speed.z) > 0.1f;
#endif
        }
        //-------------------------------------------------
        internal override void SetRVOVelocity(FVector3 speed)
        {
            m_RVOVelocity = speed;
        }
        //-------------------------------------------------
        internal override FVector3 GetRVOVelocity()
        {
            return m_RVOVelocity;
        }
        //------------------------------------------------------
        public void SetFarction(FFloat fFraction)
        {
            m_fFraction = fFraction;
        }
        //------------------------------------------------------
        public FFloat GetFarction()
        {
            return m_fFraction;
        }
        //------------------------------------------------------
        public void SetGravity(FFloat fGravity)
        {
            m_fGravity = fGravity;
        }
        //------------------------------------------------------
        public FFloat GetGravity()
        {
            return m_fGravity;
        }
        //------------------------------------------------------
        public void EnableGravity(bool bEnable)
        {
            m_bUseGravity = bEnable;
        }
        //------------------------------------------------------
        public override FVector3 GetSpeed() { return m_Speed; }
        //------------------------------------------------------
        public override void SetSpeed(FVector3 vSpeed)
        {
            m_Speed = vSpeed;
        }
        //------------------------------------------------------
        public override void SetSpeedXZ(FVector3 vSpeed)
        {
            m_Speed.x = vSpeed.x;
            m_Speed.z = vSpeed.z;
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public void SetSpeedXZ(ExternEngine.FFloat fSpeedX, ExternEngine.FFloat fSpeedZ)
        {
            m_Speed.x = fSpeedX;
            m_Speed.z = fSpeedZ;
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public void SetSpeedY(ExternEngine.FFloat fSpeed)
        {
            m_Speed.y = fSpeed;
        }
        //------------------------------------------------------
        public void SetCollisionFlag(byte dwCollisionFlag)
        {
            if (m_nCollisionFlag == dwCollisionFlag)
                return;
            m_nLastCollisionFlag = m_nCollisionFlag;
            m_nCollisionFlag = dwCollisionFlag;
        }
        //------------------------------------------------------
        public bool IsCollisionFlag(ECollisionFlag flag)
        {
            return (m_nCollisionFlag & (byte)flag) != 0;
        }
        //------------------------------------------------------
        public bool IsLastCollisionFlag(ECollisionFlag flag)
        {
            return (m_nLastCollisionFlag & (byte)flag) != 0;
        }
        //------------------------------------------------------
        public bool IsGroundLanding()
        {
            return !IsLastCollisionFlag(ECollisionFlag.DOWN) && IsCollisionFlag(ECollisionFlag.DOWN);
        }
        //------------------------------------------------------
        public void EnableCollisionFlag(ECollisionFlag flag, bool bEnable, bool bClear = false)
        {
            byte curFlag = m_nCollisionFlag;
            if (bClear) curFlag = 0;
            if (bEnable) curFlag |= (byte)flag;
            else curFlag &= (byte)(~(byte)flag);
            SetCollisionFlag(curFlag);
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public override void SetPosition(FVector3 vPos)
        {
            m_Position = vPos;
        }
        //------------------------------------------------------
        [ATMethod]
        public void SetPositionOffset(FVector3 vPos, bool bAmount = true)
        {
            if (bAmount) m_PositionOffset += vPos;
            else m_PositionOffset = vPos;
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetFinalPosition()
        {
            return m_Position;
        }
        //------------------------------------------------------
        public override void SetFinalPosition(FVector3 vPos)
        {
            m_Position = vPos;
            UpdateFinalPosition(0);
        }
        //------------------------------------------------------
        [ATMethod]
        public void SetEulerAngleImmediately(FVector3 vEulerAngle)
        {
            m_bTurning = false;
            m_TargetRotation.eulerAngles = vEulerAngle;
            m_OriginRotation.eulerAngles = vEulerAngle;
            m_CurRotation = m_OriginRotation;
        }
        //------------------------------------------------------
        public override void SetEulerAngle(FVector3 vEulerAngle)
        {
            bool bFacing = IsFlag(EWorldNodeFlag.Facing2D);
            if (bFacing)
            {
                SetDirection(BaseUtil.EulersAngleToDirection(vEulerAngle));
                return;
            }
            m_bTurning = GetTurnTime() > 0;
            if (m_bTurning)
            {
                m_OriginRotation = m_CurRotation;
                m_TargetRotation.eulerAngles = vEulerAngle;
                m_fTurningDelta.x = 0.0f;
                m_fTurningDelta.y = GetTurnTime();
            }
            else
            {
                m_TargetRotation.eulerAngles = vEulerAngle;
                m_OriginRotation.eulerAngles = vEulerAngle;
                m_CurRotation = m_OriginRotation;
            }
        }
        //------------------------------------------------------
        public void SetDirectionImmediately(FVector3 vDir)
        {
            m_bTurning = false;
            BaseUtil.CU_GetQuaternionFromDirection(vDir, GetUp(), ref m_TargetRotation);
            m_OriginRotation = m_TargetRotation;
            m_CurRotation = m_OriginRotation;
        }
        //------------------------------------------------------
        public override void SetDirection(FVector3 vDir)
        {
            SetDirection(vDir, 0);
        }
        //------------------------------------------------------
        [ATMethod]
        public void SetDirection(FVector3 vDir, FFloat turnTime, bool replaceTurnTime = true)
        {
            if (vDir.sqrMagnitude <= 0) return;

            bool bFacing = IsFlag(EWorldNodeFlag.Facing2D);
            if (bFacing)
            {
#if USE_FIXEDMATH
                FFloat dotVal = FMath.Dot(vDir, FVector3.right);
#else
                FFloat dotVal = Vector3.Dot(vDir, FVector3.right);
#endif
                if (dotVal <= ExternEngine.FFloat.zero)
                    vDir = FVector3.back;
                else
                    vDir = FVector3.forward;
            }

            if (vDir.sqrMagnitude > 0) vDir = vDir.normalized;
            if (GetFinalDirection() == vDir) return;

            m_bTurning = GetTurnTime() > ExternEngine.FFloat.zero || turnTime > ExternEngine.FFloat.zero;
            BaseUtil.CU_GetQuaternionFromDirection(vDir, GetFinalUp(), ref m_TargetRotation);
            if (m_bTurning || turnTime > 0)
            {
                m_OriginRotation = m_CurRotation;
                if (replaceTurnTime || m_fTurningDelta.y <= ExternEngine.FFloat.zero)
                {
                    m_fTurningDelta.x = ExternEngine.FFloat.zero;
                    if (turnTime > ExternEngine.FFloat.zero) m_fTurningDelta.y = turnTime;
                    else m_fTurningDelta.y = GetTurnTime();
                }

                m_bTurning = true;
            }
            else
            {
                BaseUtil.CU_GetQuaternionFromDirection(vDir, GetFinalUp(), ref m_TargetRotation);
                m_OriginRotation = m_TargetRotation;
                m_CurRotation = m_OriginRotation;
            }
        }
        //------------------------------------------------------
        [ATMethod]
        public bool IsTurnning()
        {
            return m_bTurning;
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetFinalDirection()
        {
            return m_TargetRotation * Vector3.forward;
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetFinalEulerAngle()
        {
            return m_TargetRotation.eulerAngles;
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetFinalRight()
        {
            return m_TargetRotation * FVector3.right;
        }
        //------------------------------------------------------
        public override void SetUp(FVector3 vUp)
        {
            if (vUp.sqrMagnitude <= 0) return;
            if (vUp.sqrMagnitude > 0) vUp = vUp.normalized;
            SetUp(vUp, ExternEngine.FFloat.zero);
        }
        //------------------------------------------------------
        [ATMethod]
        public void SetUp(FVector3 vUp, FFloat turnTime, bool replaceTurnTime = true)
        {
            if (vUp.sqrMagnitude <= 0) return;
            if (vUp.sqrMagnitude > 0) vUp = vUp.normalized;

            if (GetFinalUp() == vUp) return;

            m_bTurning = GetTurnTime() > ExternEngine.FFloat.zero || turnTime > ExternEngine.FFloat.zero;
            BaseUtil.CU_GetQuaternionFromDirection(GetFinalDirection(), vUp, ref m_TargetRotation);
            if (m_bTurning || turnTime > 0)
            {
                m_OriginRotation = m_CurRotation;
                if (replaceTurnTime || m_fTurningDelta.y <= ExternEngine.FFloat.zero)
                {
                    m_fTurningDelta.x = ExternEngine.FFloat.zero;
                    if (turnTime > ExternEngine.FFloat.zero) m_fTurningDelta.y = turnTime;
                    else m_fTurningDelta.y = GetTurnTime();
                }
                m_bTurning = true;
            }
            else
            {
                BaseUtil.CU_GetQuaternionFromDirection(GetFinalDirection(), vUp, ref m_TargetRotation);
                m_OriginRotation = m_TargetRotation;
                m_CurRotation = m_OriginRotation;
            }
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetFinalUp()
        {
            return m_TargetRotation * Vector3.up;
        }
        //--------------------------------------------------------
        protected override void OnInnerSpawnObject(IUserData userData)
        {
            GetActorGraph();
            if (m_vAgents != null)
            {
                for (int i = 0; i < m_vAgents.Count; ++i)
                {
                    m_vAgents[i].LoadedAble(userData);
                }
            }
            if (m_pAT != null)
            {
                Framework.Plugin.AT.AgentTreeManager.getInstance().UnloadAT(m_pAT);
                m_pAT = null;
            }
            if(m_pObjectAble!=null)
            {
                ActorComponent actorComponent = m_pObjectAble as ActorComponent;
                if (actorComponent != null)
                {
                    if(actorComponent.ActionGraphData!=null)
                    {
                        GetActorGraph().LoadActorGraph(actorComponent.ActionGraphData, OnLoadActorGraph);
                    }
                    m_pAT = Framework.Plugin.AT.AgentTreeManager.getInstance().LoadAT(actorComponent.ATData);
                    if (m_pAT != null)
                    {
                        m_pAT.AddOwnerClass(this);
                        if (IsLogicEnable() && IsActived() && !IsDestroy())
                        {
                            m_pAT.Enable(true);
                            m_pAT.Enter();
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------
        protected override void OnFlagDirty(EWorldNodeFlag flag, bool IsUsed)
        {
            if(flag == EWorldNodeFlag.Logic)
            {
                if (m_pAT != null) m_pAT.Enable(IsUsed);
            }
            else if(flag == EWorldNodeFlag.Killed)
            {
                StartActionState(EActionStateType.Die,0, true);
            }
        }
        //--------------------------------------------------------
        public T GetAgent<T>(bool bAutoCreate= false) where T : AActorAgent, new()
        {
            if (m_mTypeAgents!=null && m_mTypeAgents.TryGetValue(typeof(T), out var agent))
                return agent as T;

            T pAgent = null;
            if(bAutoCreate)
            {
                pAgent = AddAgent<T>();
                pAgent.Init();
                if (m_pObjectAble != null)
                {
                    pAgent.LoadedAble(m_pObjectAble);
                }
            }
            return pAgent;
        }
        //--------------------------------------------------------
        public void AddAgent(AActorAgent pAgent)
        {
            if (pAgent == null)
                return;
            if (m_vAgents == null)
            {
                m_vAgents = new List<AActorAgent>(2);
                m_mTypeAgents = new Dictionary<System.Type, AActorAgent>(2);
            }
            else
            {
                if (m_vAgents.Contains(pAgent)) return;
            }
            pAgent.SetActor(this);
            m_vAgents.Add(pAgent);
            m_mTypeAgents[pAgent.GetType()] = pAgent;
        }
        //--------------------------------------------------------
        public T AddAgent<T>() where T : AActorAgent, new()
        {
            if (m_vAgents == null)
            {
                m_vAgents = new List<AActorAgent>(2);
                m_mTypeAgents = new Dictionary<System.Type, AActorAgent>(2);
            }
            else
            {
                if (m_mTypeAgents.TryGetValue(typeof(T), out var pAgentThis))
                    return pAgentThis as T;
            }
            T pAgent = new T();
            pAgent.SetActor(this);
            m_vAgents.Add(pAgent);
            m_mTypeAgents[pAgent.GetType()] = pAgent;
            return pAgent;
        }
        //--------------------------------------------------------
        public void DelAgent(AActorAgent pAgent, bool bAutoDestroy = true)
        {
            if (pAgent == null) return;
            if(m_vAgents != null)
            {
                m_vAgents.Remove(pAgent);
            }
            pAgent.SetActor(null);
            if (bAutoDestroy) pAgent.Destroy();
        }
        //--------------------------------------------------------
        public bool IsOnHitAction()
        {
            return false;
        }
        //--------------------------------------------------------
        public bool IsInvincible()
        {
            return false;
        }
        //------------------------------------------------------
        public void SetGroundType(EActorGroundType eGroundType)
        {
            if (m_eGroundType != eGroundType)
            {
                m_eLastGroundType = m_eGroundType;
                m_eGroundType = eGroundType;
            }
        }
        //--------------------------------------------------------
        protected override void InnerUpdate(ExternEngine.FFloat fFrame)
        {
            if (m_pGraph != null)
            {
                m_pGraph.Update(fFrame);
            }
            if (m_pSkillSystem != null)
                m_pSkillSystem.Update(fFrame);
            if (m_vAgents != null)
            {
                for (int i = 0; i < m_vAgents.Count; ++i)
                    m_vAgents[i].Update(fFrame);
            }
            if (IsCanLogic())
            {
                m_pActorParameter.Update(fFrame);
                if (m_fDurationHit > 0)
                {
                    m_fDurationHit -= fFrame;
                }
            }

            UpdatePosition(fFrame);
            UpdateGroundCollisionFlag();

            UpdateFinalPosition(fFrame);

            UpdateGroundType();
        }
        //------------------------------------------------------
        private void UpdateGroundCollisionFlag()
        {
            FVector3 checkPos = m_Position;
            FVector3 hitPoint = FVector3.zero;
            FVector3 hitNormal = FVector3.up;
            checkPos += hitNormal * ConstDef.TERRAIN_HIT_DISTANCE;
            var result = TerrainManager.GetHeight(
                GetGameModule(),
                checkPos,
                FVector3.down,
                ref hitPoint,
                ref hitNormal,
                ConstDef.TERRAIN_HIT_DISTANCE,
                m_fStepHeight
            );
            if(result != EPhyTerrain.None)
            {
                SetTerrain(hitPoint);
            }

            if(result == EPhyTerrain.Hit)
            {
                FFloat offsetHeight = BaseUtil.PointDistancePlane(GetFinalUp(), hitPoint, m_Position);
                if(offsetHeight  <= m_fStepHeight)
                {
                    m_Position = hitPoint;
                    SetGroundType(EActorGroundType.Ground);
                    if (!IsCollisionFlag(ECollisionFlag.DOWN))
                    {
                        EnableCollisionFlag(ECollisionFlag.DOWN, true, true);
                    }
                }
                else if(offsetHeight >= Mathf.Max(m_fStepHeight+0.05f, ConstDef.JUMP_HEIGHT_LOWER))
                {
                    SetGroundType(EActorGroundType.Air);
                    if(!IsCollisionFlag(ECollisionFlag.UP))
                    {
                        EnableCollisionFlag(ECollisionFlag.UP, true,true);
                    }
                }
                else
                {
                    SetGroundType(EActorGroundType.Ground);
                    if (!IsCollisionFlag(ECollisionFlag.DOWN))
                    {
                        EnableCollisionFlag(ECollisionFlag.DOWN, true, true);
                    }
                }
            }
            else if(result == EPhyTerrain.UnHitBelow)
            {
                if (!IsCollisionFlag(ECollisionFlag.DOWN))
                {
                    EnableCollisionFlag(ECollisionFlag.DOWN, true, true);
                }
                SetGroundType(EActorGroundType.Ground);
                m_Position = hitPoint;
            }
        }
        //------------------------------------------------------
        void UpdateGroundType()
        {
            if(m_eGroundType != m_eLastGroundType)
            {
                switch(m_eGroundType)
                {
                    case EActorGroundType.Ground:
                        {
                            if (m_eLastGroundType == EActorGroundType.Air)
                                StartActionState(EActionStateType.Jump, (int)EActionStateTag.End);
                            else
                            {
                                StartActionState(EActionStateType.Idle);
                            }
                        }
                        break;
                    case EActorGroundType.Air:
                        {
                            StartActionState(EActionStateType.Jump, (int)EActionStateTag.Start);
                        }
                        break;
                    default:
                        break;
                }

                if (m_pAT != null)
                {
                    OnGourndATData ground = new OnGourndATData();
                    ground.lastType = m_eLastGroundType;
                    ground.curType = m_eGroundType;
                    m_pAT.ExecuteAPI(ground);
                }
                m_eLastGroundType = m_eGroundType;
            }
        }
        //------------------------------------------------------
        public static FFloat APPLY_FRACTION(FFloat speed, FFloat fraction, FFloat time)
        {
#if USE_FIXEDMATH
            if (FMath.Abs(fraction) > 0.01f && FMath.Abs(speed) > 0.01f)
#else
            if (System.Math.Abs(fraction) > 0.01f && System.Math.Abs(speed) > 0.01f)
#endif
            {
                FFloat temp_speed = speed;
                if (temp_speed > 0f)
                    temp_speed -= fraction * time;
                else
                    temp_speed += fraction * time;
                if (temp_speed * speed < 0f)
                    speed = ExternEngine.FFloat.zero;
                else
                    speed = temp_speed;
            }
            return speed;
        }
        //--------------------------------------------------------
        void UpdatePosition(ExternEngine.FFloat fTime)
        {
            if (m_bTurning)
            {
                m_fTurningDelta.x += fTime;
                if (m_fTurningDelta.y > 0 && m_fTurningDelta.x < m_fTurningDelta.y)
                {
#if USE_FIXEDMATH
                    FFloat fFactor = FMath.Clamp01(m_fTurningDelta.x / m_fTurningDelta.y);
#else
                    FFloat fFactor = Mathf.Clamp01(m_fTurningDelta.x / m_fTurningDelta.y);
#endif
                    m_CurRotation = FQuaternion.Lerp(m_OriginRotation, m_TargetRotation, fFactor);
                }
                else
                {
                    m_fTurningDelta = FVector2.zero;
                    m_bTurning = false;
                    m_OriginRotation = m_TargetRotation;
                    m_CurRotation = m_TargetRotation;
                }
            }
            if (!IsFlag(EWorldNodeFlag.Logic))
                return;
            if (!IsKilled() )
            {
                FVector3 pushSpeed = FVector3.zero;
                if (IsEnableRVO() && m_fRVOPushForce > 0)
                {
                    pushSpeed = GetWorld().ComputerNewVelocity(this, new FVector3(m_Speed.x, 0.0f, m_Speed.z), m_fRVOPushForce, Base.ConfigUtil.timeHorizon, Base.ConfigUtil.timeHorizonObst, out var isColission);
                }

                if (m_bUseGravity)
                {
                    m_Speed.y -= m_fGravity * fTime;// * fTime * 0.5f;
                    if(m_Speed.y < 0 && m_eGroundType == EActorGroundType.Ground)
                    {
                        FFloat offsetHeight = BaseUtil.PointDistancePlane(GetFinalUp(), GetTerrain(), m_Position);
                        if(offsetHeight<=0) m_Speed.y = 0;
                    }
                }

                //! apply fraction
                m_Speed.x = APPLY_FRACTION(m_Speed.x, m_fFraction, fTime);
                m_Speed.z = APPLY_FRACTION(m_Speed.z, m_fFraction, fTime);
                m_PositionOffset.x = (m_Speed.x + pushSpeed.x) * fTime * GetRunSpeed();
                m_PositionOffset.z = (m_Speed.z + pushSpeed.z) * fTime * GetRunSpeed();
                m_PositionOffset.y = (m_Speed.y + pushSpeed.y) * fTime;

                /*
                if (GetPhysicRadius() > 0)
                {
                    FVector3 tempPos = m_Position + m_PositionOffset;
                    if (TerrainManager.IsPhysicCapsuleHit(GetGameModule(),
                        tempPos + m_fStepHeight * GetFinalUp(),
                        tempPos + m_pActorParameter.GetModelHeight() * GetFinalUp(),
                        GetPhysicRadius(), GetObjectAble()))
                    {
                        m_PositionOffset = FVector3.zero;
                    }
                }*/
            }
        }
        //------------------------------------------------------
        public void UpdateFinalPosition(FFloat fTime)
        {
            FVector3 lastPos = m_Position;
            m_Position += m_PositionOffset;
            if (IsTerrainBelow(m_Position))
            {
                m_Position.y = 0;
            }
            TerrainLayers.LimitInWorldZooms(GetGameModule(), ref m_Position);


            m_Transform.SetPosition(m_Position);
            m_Transform.SetEulerAngle(m_CurRotation.eulerAngles);
            m_PositionOffset = Vector3.zero;
        }
        //------------------------------------------------------
        public Transform GetEventSlot(string strSlot, ref FVector3 offset)
        {
            offset = FVector3.zero;
            if (string.IsNullOrEmpty(strSlot)) return null;
            if (!IsObjected()) return null;
#if !USE_SERVER
            if (strSlot.CompareTo("Root") == 0) return GetObjectAble().GetTransorm();
#endif
            if (strSlot.CompareTo("RootTop") == 0)
            {
                offset.y += m_pActorParameter.GetModelHeight();
#if !USE_SERVER
                return GetObjectAble().GetTransorm();
#endif
            }

            ActorComponent actorComp = m_pObjectAble as ActorComponent;
            if (actorComp == null)
                return GetObjectAble().GetTransorm();


            var pSlot = actorComp.GetSlot(strSlot, out var slotOffset);
            if (pSlot)
                offset = pSlot.transform.forward * slotOffset.z + pSlot.transform.right * slotOffset.x + pSlot.transform.up * slotOffset.y;
            else
            {
                pSlot = GetObjectAble().GetTransorm();
                offset = GetDirection() * offset.z + GetRight() * offset.x + GetUp() * offset.y;
            }
            return pSlot;
        }
        //------------------------------------------------------
        public override FMatrix4x4 GetEventBindSlot(string strSlot, int bindSlot)
        {
            if (bindSlot == 0 || !IsObjected()) return GetMatrix();
            if (string.IsNullOrEmpty(strSlot)) return GetMatrix();
            if (strSlot.CompareTo("Root") == 0) return GetMatrix();
            if (strSlot.CompareTo("RootTop") == 0)
            {
                FMatrix4x4 temp = GetMatrix();
                BaseUtil.OffsetPosition(ref temp, new Vector3(0, m_pActorParameter.GetModelHeight(), 0));
                return temp;
            }
            FMatrix4x4 matrix = GetMatrix();

            ActorComponent actorComp = m_pObjectAble as ActorComponent;
            if (actorComp == null)
                return matrix;

            var tranform = actorComp.GetTransorm();
            if (tranform)
            {
                if ((bindSlot & (int)ESlotBindBit.Rotation) != 0)
                {
                    matrix = tranform.localToWorldMatrix;
                    if ((bindSlot & (int)ESlotBindBit.Position) == 0) BaseUtil.UpdatePosition(ref matrix, GetPosition());
                    if ((bindSlot & (int)ESlotBindBit.Scale) == 0) BaseUtil.UpdateScale(ref matrix, GetScale());
                }
                else
                {
                    if ((bindSlot & (int)ESlotBindBit.Position) != 0) BaseUtil.UpdatePosition(ref matrix, tranform.position);
                    if ((bindSlot & (int)ESlotBindBit.Scale) != 0) BaseUtil.UpdateScale(ref matrix, tranform.localScale);
                }

            }

            return matrix;
        }
        //--------------------------------------------------------
        public override uint GetConfigID()
        {
            return m_pActorParameter.GetConfigID();
        }
        //--------------------------------------------------------
        public override uint GetElementFlags()
        {
            return 0;
        }
        //--------------------------------------------------------
        public override void SetElementFlags(uint flags)
        {

        }
        //------------------------------------------------------
        public override bool IsRunAlongPath()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return false;
            return agent.IsRunningAlongPathPoint();
        }
        //------------------------------------------------------
        public override bool IsRunAlongPathPlaying()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return false;
            return agent.IsRunningAlongPathPlaying();
        }
        //------------------------------------------------------
        public override bool IsLocalRunAlongPath()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return false;
            return agent.IsLocalRunAlongPath();
        }
        //------------------------------------------------------
        public override void PauseRunAlongPathPoint()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return;
            agent.PauseRunAlongPathPoint();
        }
        //------------------------------------------------------
        public override void ResumeRunAlongPathPoint()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return;
            agent.ResumeRunAlongPathPoint();
        }
        //------------------------------------------------------
        public override void AppendRunAlongPathByTimeStep(FVector3 point, FFloat fTime, int nInsertIndex = -1)
        {
            var agent = GetAgent<RunAlongPathAgent>(true);
            if (agent == null) return;
            agent.AppendRunAlongPathByTimeStep(point, fTime, nInsertIndex);
        }
        //------------------------------------------------------
        public FFloat RunAlongPathPoint(List<RunPoint> vPoints, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            var agent = GetAgent<RunAlongPathAgent>(true);
            return agent.RunAlongPathPoint(vPoints, fSpeed, bEnsureSucceed, bUpdateDirection, bLocalRun);
        }
        //------------------------------------------------------
        public override FFloat RunAlongPathPoint(List<FVector3> vPoints, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            var agent = GetAgent<RunAlongPathAgent>(true);
            return agent.RunAlongPathPoint(vPoints, fSpeed, bEnsureSucceed, bUpdateDirection, bLocalRun);
        }
        //------------------------------------------------------
        public override FFloat RunAlongPathPoint(FVector3 toPos, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            var agent = GetAgent<RunAlongPathAgent>(true);
            return agent.RunAlongPathPoint(GetPosition(), toPos, fSpeed, bEnsureSucceed, bUpdateDirection, bLocalRun);
        }
        //------------------------------------------------------
        public override FFloat RunAlongPathPoint(FVector3 srcPos, FVector3 toPos, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            var agent = GetAgent<RunAlongPathAgent>(true);
            return agent.RunAlongPathPoint(srcPos, toPos, fSpeed, bEnsureSucceed, bUpdateDirection, bLocalRun);
        }
        //------------------------------------------------------
        public override void StopRunAlongPathPoint()
        {
            var agent = GetAgent<RunAlongPathAgent>(false);
            if (agent == null) return;
            agent.StopRunAlongPathPoint();
        }
        //------------------------------------------------------
#if USE_CUTSCENE
        //--------------------------------------------------------
        public override bool SetParameter(EParamType type, CutsceneParam paramData)
        {
            switch(type)
            {
                case EParamType.ePlayAction:
                    {
                        var animationClip = paramData.ToUnityObject<AnimationClip>();
                        if(animationClip!=null)
                        {
                            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
                            if (null == pAgent)
                                return false;
                            pAgent.PlayAnimation(paramData.userData, animationClip, paramData.ToInt(0));
                            pAgent.SetActionTime(paramData.userData, paramData.ToFloat(1));
                        }
                        else
                        {
                            string actionName = paramData.ToString();
                            if (string.IsNullOrEmpty(actionName))
                            {
                                EActionStateType eType = (EActionStateType)paramData.ToInt(0);
                                uint tag = (uint)paramData.ToInt(1);
                                StartActionState(eType, tag, false);
                            }
                            else
                            {
                                ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
                                if (null == pAgent)
                                    return false;
                                pAgent.PlayAnimation(actionName);
                                pAgent.SetActionTime(actionName,paramData.ToFloat(0));
                            }
                        }
                    }
                    return true;
                case EParamType.eStopAction:
                    {
                        string actionName = paramData.ToString();
                        var animationClip = paramData.ToUnityObject<AnimationClip>();
                        if (animationClip != null)
                        {
                            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
                            if (null == pAgent)
                                return false;
                            pAgent.StopAnimation(paramData.userData);
                        }
                        else if (string.IsNullOrEmpty(actionName))
                        {
                            EActionStateType eType = (EActionStateType)paramData.ToInt(0);
                            uint tag = (uint)paramData.ToInt(1);
                            StopActionState(eType, tag);
                        }
                        else
                        {
                            ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
                            if (null == pAgent)
                                return false;
                            pAgent.StopAnimation(actionName);
                        }
                    }
                    return true;
                case EParamType.eActionSpeed:
                    {
                        ActorGraphicAgent pAgent = GetAgent<ActorGraphicAgent>();
                        if (null == pAgent)
                            return false;
                        if (paramData.userData != null)
                        {
                            pAgent.SetActionSpeed(paramData.userData,paramData.ToFloat(1));
                        }
                        else if (string.IsNullOrEmpty(paramData.strData))
                        {
                            pAgent.SetActionSpeed(paramData.strData, paramData.ToFloat(1));
                        }
                        else
                            pAgent.SetActionSpeed(paramData.ToFloat());
                    }
                    return true;
            }
            return base.SetParameter(type, paramData);
        }
#endif
        //--------------------------------------------------------
        protected override void OnDestroy()
        {
            OnStateInfoCallback = null;
            if (m_pGraph != null)
            {
                m_pGraph.Free();
                m_pGraph = null;
            }
            if (m_pSkillSystem != null)
            {
                m_pSkillSystem.Free();
                m_pSkillSystem = null;
            }
            if (m_vAgents != null)
            {
                for (int i = 0; i < m_vAgents.Count; ++i)
                    m_vAgents[i].Free();
                m_vAgents.Clear();
            }
            if (m_pActorParameter != null)
            {
                m_pActorParameter.Free();
                m_pActorParameter = null;
            }

            if (m_pAT != null)
            {
                m_pAT.Exit();
                m_pAT.Enable(false);
                Framework.Plugin.AT.AgentTreeManager.getInstance().UnloadAT(m_pAT);
                m_pAT = null;
            }
        }
    }
}
#else
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using Framework.Core;
using Framework.Plugin.AT;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{

    [ATExportMono("World/Actor", EGlobalType.None, true, "", marcoDefine = "USE_ACTORSYSTEM")]
    public class Actor : AWorldNode
    {
        public Actor(AFramework pGame) : base(pGame) { }

        public override uint GetConfigID() { return 0; }
        public override uint GetElementFlags() { return 0; }
        public override void SetElementFlags(uint flags) { }
        protected override void OnDestroy() { }
        public void SetIdleType(EActionStateType state) { }
        public void StartActionState(EActionStateType state, int tag = 0) { }
        public void AddCallback(IActorAttrDirtyCallback callback) { }
        public void RemoveCallback(IActorAttrDirtyCallback callback) { }
        public void SetAttrs(byte[] attiTypes, int[] values) { }
        public void SetAttr(byte type, int value) { }
        //--------------------------------------------------------
        public int GetAttr(byte type, int defVal = 0) { return 0; }
        public void RemoveAttr(byte type) { }
        //--------------------------------------------------------
        public void AppendAttrs(byte[] attiTypes, int[] values) { }
        //--------------------------------------------------------
        public void AppendAttr(byte type, int value) { }
        //--------------------------------------------------------
        public void SubAttrs(byte[] attiTypes, int[] values) { }
        //--------------------------------------------------------
        public void SubAttr(byte type, int value, bool bLowerZero = false) { }
        //--------------------------------------------------------
        public void ClearAttrs() { }
        public Transform GetEventSlot(string strSlot, ref FVector3 offset) { return null; }
        public FFloat GetProjectAppendSpeed() { return 0.0f; }
        public bool IsInvincible() { return false; }
    }
}
#endif
