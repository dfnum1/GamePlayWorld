//------------------------------------------------------
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Projectile
作    者:	HappLI
描    述:	飞行道具
*********************************************************************/

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using Framework.Base;
using System;
using System.Collections.Generic;
using UnityEngine;
using Codice.CM.Client.Differences;
using Framework.Cutscene.Runtime;
#if USE_SERVER
using AudioClip = ExternEngine.AudioClip;
using Transform = ExternEngine.Transform;
#endif

namespace Framework.Core
{
    //------------------------------------------------------
    public class ProjectileNode : AWorldNode
    {
        private ProjectileData m_ProjecileData;

        //! run time data
        private FFloat m_fDelayTime = 0;
        private FVector3 m_InitPosition;
  //      private FVector3 m_LastPosition;
  //      private FVector3 m_Position;
        private FVector3 m_TargetPosition;
        private FVector3 m_InitDirection;
        private Vector3 m_SelfRotate;
   //     private FVector3 m_Direction;
        private FVector3 m_OffsetEulerAngle;
        private FVector3 m_Speed;
        private FVector3 m_ExternSpeed;
        private FVector3 m_Acceleration;
        private FVector3 m_MaxSpeed;
        private FFloat m_fTrackSpeedLerp;
        private FFloat m_fRunningTime;
        private StateParam m_pStateParam;
        private uint m_nInheritFlag;
        private FFloat m_fRemainLifeTime;
        private FFloat m_fLifeTime;
        private int m_nMaxOneFrameHit;
        private int m_nRemainHitCount;
        private FFloat m_fHitStepDelta;
        private FFloat m_fDelayStopDuration;
        private FFloat m_fDelayStopDelta;


        private int m_nDamagePower = 0;
        private uint m_nDamageId = 0;
        private FFloat m_TotalDamage = 0;

        private bool m_HasSpeed = true;

        private int m_nBounceTypeBounceCount = 0;

        public bool m_IsBoundProjectile = false;
        public AWorldNode m_pBoundStartActor = null;
        public int m_nRemainBoundCount = -1;

        private float m_fEventStepDetla = 0;

        private AWorldNode m_pOwnerActor = null;
        private string m_strBindOwnerSlot = null;
        private FVector3 m_StartBindOffset = FVector3.zero;

        private AWorldNode m_pTargetNode = null;

        private FVector3 m_BornOffset = FVector3.zero;

        private uint m_nTrackFrameId;
        private uint m_nTrackBodyPart;
        private FVector3 m_TrackPoint = FVector3.zero;
        private Transform m_pTrackTransform = null;
        private FVector3 m_TrackOffset = FVector3.zero;
        private bool m_bTrackEnd;
        private bool m_bTrackBegin = true;

        private float m_fWaringDuration = 0;
        private bool m_HasDropPoint = false;
        private Vector3 m_FinalDropPosition = Vector3.zero;

        private uint m_nLaunchFlag = 0;

        private List<Actor> m_vHoldRoles = null;
        private Dictionary<int, int> m_on_hit_actors = new Dictionary<int, int>(2);
        private HashSet<int> m_bounded_list = null;

        private List<ProjectileKeyframe> m_vTrackKeyframes = null;
        //------------------------------------------------------
        public ProjectileNode(AFramework pGameMoudle) : base(pGameMoudle)
        {

        }
        //------------------------------------------------------
        public ProjectileData GetProjectileData() => m_ProjecileData;
        //------------------------------------------------------
        public int GetCfgBoundCount()
        {
            if (m_ProjecileData == null) return 0;
            return m_ProjecileData.bound_count;
        }
        //------------------------------------------------------
        public int GetCfgHitCount()
        {
            if (m_ProjecileData == null) return 0;
            return m_ProjecileData.hit_count;
        }
        //------------------------------------------------------
        public bool IsPenetrable()
        {
            if (m_ProjecileData == null) return false;
            return m_ProjecileData.penetrable;
        }
        //------------------------------------------------------
        public FFloat GetDelayTime() => m_fDelayTime;
        //------------------------------------------------------
        public void SetDelayTime(FFloat value) => m_fDelayTime = value;
        //------------------------------------------------------
        public FVector3 GetInitPosition() => m_InitPosition;
        //------------------------------------------------------
        public void SetInitPosition(FVector3 value) => m_InitPosition = value;
        //------------------------------------------------------
        public FVector3 GetTargetPosition() => m_TargetPosition;
        //------------------------------------------------------
        public void SetTargetPosition(FVector3 value) => m_TargetPosition = value;
        //------------------------------------------------------
        public FVector3 GetInitDirection() => m_InitDirection;
        //------------------------------------------------------
        public void SetInitDirection(FVector3 value) => m_InitDirection = value;
        //------------------------------------------------------
        public Vector3 GetSelfRotate() => m_SelfRotate;
        //------------------------------------------------------
        public void SetSelfRotate(Vector3 value) => m_SelfRotate = value;
        //------------------------------------------------------
        public FVector3 GetOffsetEulerAngle() => m_OffsetEulerAngle;
        //------------------------------------------------------
        public void SetOffsetEulerAngle(FVector3 value) => m_OffsetEulerAngle = value;
        //------------------------------------------------------
        public FVector3 GetExternSpeed() => m_ExternSpeed;
        //------------------------------------------------------
        public void SetExternSpeed(FVector3 value) => m_ExternSpeed = value;
        //------------------------------------------------------
        public FFloat GetRunningTime() => m_fRunningTime;
        //------------------------------------------------------
        public void SetStateParam(StateParam value) => m_pStateParam = value;
        //------------------------------------------------------
        public uint GetInheritFlag() => m_nInheritFlag;
        //------------------------------------------------------
        public void SetInheritFlag(uint value) => m_nInheritFlag = value;
        //------------------------------------------------------
        public FFloat GetRemainLifeTime() => m_fRemainLifeTime;
        //------------------------------------------------------
        public void SetRemainLifeTime(FFloat value) => m_fRemainLifeTime = value;
        //------------------------------------------------------
        public void SubHitCount(int hitCout)
        {
            m_nRemainHitCount -= hitCout;
            if (m_nRemainHitCount < 0) m_nRemainHitCount = 0;
        }
        //------------------------------------------------------
        public void ResetHitStepDelta()
        {
            if (m_ProjecileData == null)
                return;
            m_fHitStepDelta = m_ProjecileData.hit_step;
        }
        //------------------------------------------------------
        public FFloat GetLifeTime() => m_fLifeTime;
        //------------------------------------------------------
        public void SetLifeTime(FFloat value) => m_fLifeTime = value;
        //------------------------------------------------------
        public int GetMaxOneFrameHit() => m_nMaxOneFrameHit;
        //------------------------------------------------------
        public void SetMaxOneFrameHit(int value) => m_nMaxOneFrameHit = value;
        //------------------------------------------------------
        public int GetRemainHitCount() => m_nRemainHitCount;
        //------------------------------------------------------
        public void SetRemainHitCount(int value) => m_nRemainHitCount = value;
        //------------------------------------------------------
        public FFloat GetHitStepDelta() => m_fHitStepDelta;
        //------------------------------------------------------
        public void SetHitStepDelta(FFloat value) => m_fHitStepDelta = value;
        //------------------------------------------------------
        public FFloat GetDelayStopDuration() => m_fDelayStopDuration;
        //------------------------------------------------------
        public void SetDelayStopDuration(FFloat value) => m_fDelayStopDuration = value;
        //------------------------------------------------------
        public FFloat GetDelayStopDelta() => m_fDelayStopDelta;
        //------------------------------------------------------
        public void SetDelayStopDelta(FFloat value) => m_fDelayStopDelta = value;
        //------------------------------------------------------
        public int GetDamagePower() => m_nDamagePower;
        //------------------------------------------------------
        public void SetDamagePower(int value) => m_nDamagePower = value;
        //------------------------------------------------------
        public void SetDamageId(uint value) => m_nDamageId = value;
        //------------------------------------------------------
        public bool HasSpeed() => m_HasSpeed;
        //------------------------------------------------------
        public int GetBounceTypeBounceCount() => m_nBounceTypeBounceCount;
        //------------------------------------------------------
        public void SetBounceTypeBounceCount(int value) => m_nBounceTypeBounceCount = value;
        //------------------------------------------------------
        public bool IsBoundProjectile() => m_IsBoundProjectile;
        //------------------------------------------------------
        public void SetBoundProjectile(bool bBound)
        {
            m_IsBoundProjectile = bBound;
        }
        //------------------------------------------------------
        public AWorldNode GetBoundStartActor() => m_pBoundStartActor;
        //------------------------------------------------------
        public void SetBoundStartActor(AWorldNode value) => m_pBoundStartActor = value;
        //------------------------------------------------------
        public int GetRemainBoundCount() => m_nRemainBoundCount;
        //------------------------------------------------------
        public void SetRemainBoundCount(int value) => m_nRemainBoundCount = value;
        //------------------------------------------------------
        public AWorldNode GetOwnerActor() => m_pOwnerActor;
        //------------------------------------------------------
        public void SetOwnerActor(AWorldNode value) => m_pOwnerActor = value;
        //------------------------------------------------------
        public string GetBindOwnerSlot() => m_strBindOwnerSlot;
        //------------------------------------------------------
        public void SetBindOwnerSlot(string value) => m_strBindOwnerSlot = value;
        //------------------------------------------------------
        public FVector3 GetStartBindOffset() => m_StartBindOffset;
        //------------------------------------------------------
        public void SetStartBindOffset(FVector3 value) => m_StartBindOffset = value;
        //------------------------------------------------------
        public AWorldNode GetTargetNode() => m_pTargetNode;
        //------------------------------------------------------
        public void SetTargetNode(AWorldNode value) => m_pTargetNode = value;
        //------------------------------------------------------
        public uint GetTrackFrameId() => m_nTrackFrameId;
        //------------------------------------------------------
        public void SetTrackFrameId(uint value) => m_nTrackFrameId = value;
        //------------------------------------------------------
        public uint GetTrackBodyPart() => m_nTrackBodyPart;
        //------------------------------------------------------
        public void SetTrackBodyPart(uint value) => m_nTrackBodyPart = value;
        //------------------------------------------------------
        public FVector3 GetTrackPoint() => m_TrackPoint;
        //------------------------------------------------------
        public void SetTrackPoint(FVector3 value) => m_TrackPoint = value;
        //------------------------------------------------------
        public Transform GetTrackTransform() => m_pTrackTransform;
        //------------------------------------------------------
        public void SetTrackTransform(Transform value) => m_pTrackTransform = value;
        //------------------------------------------------------
        public FVector3 GetTrackOffset() => m_TrackOffset;
        //------------------------------------------------------
        public void SetTrackOffset(FVector3 value) => m_TrackOffset = value;
        //------------------------------------------------------
        public bool IsTrackEnd() => m_bTrackEnd;
        //------------------------------------------------------
        public bool IsTrackBegin() => m_bTrackBegin;
        //------------------------------------------------------
        public float GetWaringDuration() => m_fWaringDuration;
        //------------------------------------------------------
        public void SetWaringDuration(float value) => m_fWaringDuration = value;
        //------------------------------------------------------
        public bool HasDropPoint() => m_HasDropPoint;
        //------------------------------------------------------
        public Vector3 GetFinalDropPosition() => m_FinalDropPosition;
        //------------------------------------------------------
        public uint GetLaunchFlag() => m_nLaunchFlag;
        //------------------------------------------------------
        public void SetLaunchFlag(uint value) => m_nLaunchFlag = value;
        //------------------------------------------------------
        public List<Actor> GetHoldRoles() => m_vHoldRoles;
        //------------------------------------------------------
        public void AddHoldRole(Actor value)
        {
            if (value == null)
                return;
            if (m_vHoldRoles == null) m_vHoldRoles = new List<Actor>(2);
            if(!m_vHoldRoles.Contains(value)) m_vHoldRoles.Add(value);
        }
        //------------------------------------------------------
        public void ClearHoldRoles()
        {
            if (m_vHoldRoles != null)
            {
                Actor target;
                for (int i = 0; i < m_vHoldRoles.Count; ++i)
                {
                    target = m_vHoldRoles[i];
                    target.SetSpeedXZ(Vector3.zero);
                }
                m_vHoldRoles.Clear();
            }
        }
        //------------------------------------------------------
        public bool CanSceneTest()
        {
            if (m_ProjecileData == null) return false;
            if (m_ProjecileData.type == EProjectileType.Bounce) return false;
            return !m_ProjecileData.unSceneTest;
        }
        //------------------------------------------------------
        public uint GetDamageID(bool bCheckState =true)
        {
            if (m_nDamageId == 0 && bCheckState)
            {
                if (m_pStateParam != null) return (uint)(m_pStateParam.GetDamageID());
            }
            return (uint)m_nDamageId;
        }
        //------------------------------------------------------
        public uint GetExplodeDamageID()
        {
            if (m_ProjecileData == null) return GetDamageID();
            uint id = m_ProjecileData.explode_damage_id;
            if (id == 0) return GetDamageID();
            return (uint)id;
        }
        //------------------------------------------------------
        protected override void OnFlagDirty(EWorldNodeFlag flag, bool bSet)
        {
            if (flag == EWorldNodeFlag.Killed || flag == EWorldNodeFlag.Destroy)
            {
                if (bSet) m_fRemainLifeTime = 0;
                if (IsLaunchFlaged(ELaunchFlag.DieKeep))
                {
                    AInstanceAble pAble = m_pObjectAble;
                    m_pObjectAble = null;
                    if (pAble != null)
                        pAble.DelayDestroy(5);
                }
            }
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            if (m_vHoldRoles != null) m_vHoldRoles.Clear();
            m_fRemainLifeTime = 0;
        }
        //------------------------------------------------------
        public override FFloat GetPhysicRadius()
        {
            return 0.0f;
        }
        //------------------------------------------------------
        public bool IsLaunchFlaged(ELaunchFlag flag)
        {
            return (m_nLaunchFlag & (int)flag) != 0;
        }
        //------------------------------------------------------
        public override FVector3 GetSpeed()
        {
            return m_Speed;
        }
        //------------------------------------------------------
        public override void SetSpeed(FVector3 speed)
        {
            m_Speed = speed;
        }
        //------------------------------------------------------
        public FVector3 GetAcceleration()
        {
            return m_Acceleration;
        }
        //------------------------------------------------------
        public void SetAcceleration(FVector3 acc)
        {
            m_Acceleration = acc;
        }
        //------------------------------------------------------
        public void ResetTrackStates()
        {
            m_bTrackBegin = false;
            m_bTrackEnd = false;
        }
        //------------------------------------------------------
        public virtual void Reset()
        {
            m_ProjecileData = null;

            m_fDelayTime = 0.0f;
            m_fEventStepDetla = 0.0f;
            m_InitPosition = FVector3.zero;
    //        m_LastPosition = FVector3.zero;
    //        m_Position = FVector3.zero;
            m_TargetPosition = FVector3.zero;
            m_InitDirection = FVector3.forward;
            m_SelfRotate = Vector3.zero;
     //       m_Direction = FVector3.zero;
            m_OffsetEulerAngle = FVector3.zero;
            m_Speed = FVector3.zero;
            m_ExternSpeed = FVector3.zero;
            m_Acceleration = FVector3.zero;
            m_MaxSpeed = FVector3.zero;
            m_fRunningTime = 0.0f;
            m_pOwnerActor = null;
            m_strBindOwnerSlot = null;
            m_StartBindOffset = FVector3.zero;
            m_pStateParam = null;
            m_nInheritFlag = 0xffffffff;
            m_fRemainLifeTime = 0.0f;
            m_nMaxOneFrameHit = 1;
            m_nRemainHitCount = 0;
            m_fHitStepDelta = 0.0f;
            m_fDelayStopDuration = 0.0f;
            m_fDelayStopDelta = 0.0f;
            if (m_on_hit_actors != null) m_on_hit_actors.Clear();

            m_fTrackSpeedLerp = 0.0f;
            m_nTrackFrameId = 0xfffffff;
            m_nTrackBodyPart = 0xfffffff;
            m_pTrackTransform = null;
            m_TrackPoint = Vector3.zero;
            m_TrackOffset = Vector3.zero;
            ResetTrackStates();

             m_nDamageId = 0;
            m_TotalDamage = 0;

            m_nBounceTypeBounceCount = 0;

            if (m_bounded_list != null) m_bounded_list.Clear();
            m_nRemainBoundCount = -1;
            m_pBoundStartActor = null;
            m_IsBoundProjectile = false;

            m_fWaringDuration = 0;
            m_HasDropPoint = false;
            m_FinalDropPosition = Vector3.zero;

            m_HasSpeed = false;

            m_BornOffset = FVector3.zero;

            m_nLaunchFlag = 0;

            if (m_vHoldRoles != null)
            {
                Actor target;
                for (int i = 0; i < m_vHoldRoles.Count; ++i)
                {
                    target = m_vHoldRoles[i];
                    target.SetSpeedXZ(Vector3.zero);
                }
                m_vHoldRoles.Clear();
            }

            if (m_vTrackKeyframes != null) m_vTrackKeyframes.Clear();
        }
        //------------------------------------------------------
        protected override void OnInnerSpawnObject(IUserData user)
        {
            if (m_pObjectAble != null && m_pObjectAble is ParticleController)
            {
                ParticleController parCtl = ((ParticleController)m_pObjectAble);
                parCtl.EnableTrailEmitting(m_fDelayTime <= 0);
                parCtl.SetOwner(this);
                if (m_fDelayTime > 0) parCtl.Pause();
                else parCtl.Resume();
            }
        }
        //------------------------------------------------------
        public void SetBounceTypeCount(int bounceCount)
        {
            m_nBounceTypeBounceCount = bounceCount;
        }
        //------------------------------------------------------
        public void SetBornOffset(FVector3 offset)
        {
            m_BornOffset = offset;
        }
        //------------------------------------------------------
        public void SetData(ProjectileData pData, AWorldNode pOwnerActor, AWorldNode m_pTargetNode, FVector3 vPosition, FVector3 vDirection, int nDamagePower, uint m_nTrackFrameId, uint track_body_id)
        {
            if ((pData.launch_flag & (int)ELaunchFlag.DirectionLuanch) == 0)
            {
                FFloat single = FVector3.Dot(vDirection, FVector3.forward) >= 0 ? 1.0f : -1.0f;
                vDirection = FVector3.forward * single;
            }

            m_pBoundStartActor = null;
            m_IsBoundProjectile = false;
            m_ProjecileData = pData;
            m_Transform.SetScale(FVector3.one * m_ProjecileData.scale);
            m_nRemainBoundCount = pData.bound_count;
            m_fWaringDuration = pData.waring_duration;
            m_ProjecileData = pData;
            m_pOwnerActor = pOwnerActor;
            if (m_pOwnerActor != null) SetAttackGroup(m_pOwnerActor.GetAttackGroup());
            else SetAttackGroup(0xff);
            m_nMaxOneFrameHit = Mathf.Max(1, m_ProjecileData.max_oneframe_hit);
            m_fRemainLifeTime = (FFloat)m_ProjecileData.life_time;
            m_fLifeTime = (FFloat)m_ProjecileData.life_time;
            m_nRemainHitCount = m_ProjecileData.hit_count;
            m_fRunningTime = 0.0f;
            m_fHitStepDelta = 0.0f;
            m_InitPosition = vPosition;
            SetPosition(m_InitPosition);
            m_Transform.SetLastPosition(m_InitPosition);
            //       m_LastPosition = m_InitPosition;
            //       m_Position = m_InitPosition;
            m_TargetPosition = FVector3.zero;
            m_bTrackEnd = false;
            m_bTrackBegin = pData.speedLerp.y <= 0;
            this.m_nTrackFrameId = m_nTrackFrameId;
            this.m_nTrackBodyPart = track_body_id;
            this.m_nDamagePower = nDamagePower;
            this.m_nDamageId = m_ProjecileData.damage;
            this.m_pTargetNode = m_pTargetNode;
            this.m_nLaunchFlag = m_ProjecileData.launch_flag;
            m_Speed = FVector3.zero;
            m_ExternSpeed = FVector3.zero;
            m_Acceleration = FVector3.zero;
            m_InitDirection = vDirection;
            FVector3 dir = FVector3.forward;
            if (m_Speed.sqrMagnitude <= 0.001f)
                dir = vDirection;
            else
                dir = m_Speed;
            m_SelfRotate.x = GetGameModule().GetRamdom(pData.minRotate.x, pData.maxRotate.x);
            m_SelfRotate.y = GetGameModule().GetRamdom(pData.minRotate.y, pData.maxRotate.y);
            m_SelfRotate.z = GetGameModule().GetRamdom(pData.minRotate.z, pData.maxRotate.z);
            dir.Normalize();
            SetDirection(dir);

            if (!pData.bImmedate && pData.fEventStepGap > 0)
                m_fEventStepDetla = pData.fEventStepGap;

            m_BoundBox.Set(m_ProjecileData.aabb_min, m_ProjecileData.aabb_max);

            if (m_vTrackKeyframes != null) m_vTrackKeyframes.Clear();

            m_nBounceTypeBounceCount = 0;
            if (pData.type == EProjectileType.Bounce)
                SetBounceTypeCount((int)pData.speedLerp.x);

            this.OnCreated();
        }
        //------------------------------------------------------
        public void SetTrack(Transform pTrackSlot, FVector3 m_TrackOffset)
        {
            if (ProjectileData.IsTrack(m_ProjecileData.type) || m_ProjecileData.type == EProjectileType.TrackPath)
            {
                m_pTrackTransform = pTrackSlot;
                this.m_TrackOffset = m_TrackOffset;
                this.m_fTrackSpeedLerp = (FFloat)m_ProjecileData.speedLerp.x;
                if (m_pTrackTransform)
                    m_TrackPoint = m_pTrackTransform.position;
                else if (m_pTargetNode != null)
                    m_TrackPoint = m_pTargetNode.GetPosition();
            }
            UpdateTrackPosition();
        }
        //------------------------------------------------------
        public bool UpdateTrackPosition()
        {
            bool bTrackDo = false;
            if (ProjectileData.IsTrack(m_ProjecileData.type) || m_ProjecileData.type == EProjectileType.TrackPath || m_IsBoundProjectile)
            {
                if (m_ProjecileData.type == EProjectileType.TrackPoint)
                {
                    m_TargetPosition = m_TrackPoint;
                    if (m_pTrackTransform)
                        m_TargetPosition.z = (FFloat)m_pTrackTransform.position.z;
                    else if (m_pTargetNode != null)
                        m_TargetPosition.z = m_pTargetNode.GetPosition().z;

                    m_TargetPosition += m_ProjecileData.track_target_offset + m_TrackOffset;
                    bTrackDo = true;
                }
                else
                {
                    if (m_pTargetNode != null && !m_pTargetNode.IsFlag(EWorldNodeFlag.Killed) && !m_pTargetNode.IsDestroy())
                    {
                        m_TargetPosition = m_pTargetNode.GetPosition();
                        if (m_pTrackTransform != null)
                        {
                            m_TargetPosition = m_pTrackTransform.position + m_ProjecileData.track_target_offset + m_TrackOffset;
                        }
                    }
                    bTrackDo = true;
                }
            }
            return bTrackDo;
        }
        //------------------------------------------------------
        public void SetSpeed(FVector3 vPosition, FVector3 vDirection, int index, bool isFace2D = false)
        {
            if (m_ProjecileData.type == EProjectileType.Trap)
            {
                m_InitDirection = vDirection;
                vDirection.Normalize();
                SetDirection(vDirection);
                m_HasSpeed = false;
                return;
            }
            if ((m_ProjecileData.launch_flag & (int)ELaunchFlag.DirectionLuanch) == 0)
            {
                FFloat single = FVector3.Dot(vDirection, FVector3.forward) >= 0 ? 1.0f : -1.0f;
                vDirection = FVector3.forward * single;
            }
            if (m_ProjecileData.type == EProjectileType.TrackPath)
            {
                vDirection.Normalize();
                this.m_Speed.x = GetGameModule().GetRamdom(m_ProjecileData.speedLerp.x, m_ProjecileData.speedLerp.y);
                m_HasSpeed = m_Speed.sqrMagnitude > 0.1f;
                return;
            }

            FVector3 up = FVector3.up;
            FVector3 vRight = FVector3.right;
            if (isFace2D) vRight = -FVector3.Cross(vDirection, up);
            else vRight = FVector3.Cross(vDirection, up);
            if (index >= 0 && m_ProjecileData.speedLows != null && index < m_ProjecileData.speedLows.Length)
            {
                FVector3 randomSpeed = m_ProjecileData.speedLows[index];
                FVector3 randomAcceleration = m_ProjecileData.accelerations[index];
                if (m_ProjecileData.speedUppers != null && index < m_ProjecileData.speedUppers.Length)
                {
                    randomSpeed.x = GetGameModule().GetRamdom(m_ProjecileData.speedLows[index].x, m_ProjecileData.speedUppers[index].x);
                    randomSpeed.y = GetGameModule().GetRamdom(m_ProjecileData.speedLows[index].y, m_ProjecileData.speedUppers[index].y);
                    randomSpeed.z = GetGameModule().GetRamdom(m_ProjecileData.speedLows[index].z, m_ProjecileData.speedUppers[index].z);
                }
                m_Speed = vDirection * randomSpeed.z + up * randomSpeed.y + vRight * randomSpeed.x;
                m_Acceleration = vDirection * randomAcceleration.z + up * randomAcceleration.y + vRight * randomAcceleration.x;
                if (m_ProjecileData.speedMaxs != null && index < m_ProjecileData.speedMaxs.Length)
                    m_MaxSpeed = m_ProjecileData.speedMaxs[index];
                else m_MaxSpeed = FVector3.zero;
                m_InitDirection = vDirection;
                SetDirection(vDirection);
            }
            m_HasSpeed = m_Speed.sqrMagnitude > 0.1f;
        }
        //------------------------------------------------------
        public void SetBoundSpeed(FVector3 vPosition, FVector3 vDirection, int index)
        {
            if (m_ProjecileData.bound_speed.sqrMagnitude <= 0)
            {
                SetSpeed(vPosition, vDirection, index);
                return;
            }

            if (m_ProjecileData.type == EProjectileType.Trap)
            {
                m_InitDirection = vDirection;
                vDirection.Normalize();
                SetDirection(vDirection);
                m_HasSpeed = false;
                return;
            }
            if (m_ProjecileData.type == EProjectileType.TrackPath)
            {
                vDirection.Normalize();
                m_Speed = vDirection * GetGameModule().GetRamdom(m_ProjecileData.speedLerp.x, m_ProjecileData.speedLerp.y);
                m_HasSpeed = m_Speed.sqrMagnitude > 0.1f;
                return;
            }

            FVector3 up = FVector3.up;
            FVector3 vRight = FVector3.Cross(vDirection, up);
            FVector3 randomSpeed = m_ProjecileData.bound_speed;
            m_Speed = vDirection * randomSpeed.z + up * randomSpeed.y + vRight * randomSpeed.x;
            m_Acceleration = FVector3.zero;
            m_MaxSpeed = FVector3.zero;
            m_InitDirection = vDirection;
            SetDirection(vDirection);
            m_HasSpeed = m_Speed.sqrMagnitude > 0.1f;
        }
        //------------------------------------------------------
        public void SetRuntimeTime(FFloat time)
        {
            this.m_fRunningTime = time;
        }
        //------------------------------------------------------
        protected override void InnerUpdate(ExternEngine.FFloat fFrameTime)
        {
            if (m_pGame.IsPause() || m_pGame.IsLogicLock())
                fFrameTime = 0;

            this.m_fRunningTime += fFrameTime;
            if (m_fDelayTime > 0)
            {
                m_fDelayTime -= fFrameTime;
                if (m_fDelayTime <= 0)
                {
                    if (m_pObjectAble != null && m_pObjectAble is ParticleController)
                    {
                        ParticleController parCtl = ((ParticleController)m_pObjectAble);
                        parCtl.EnableTrailEmitting(true);
                        parCtl.Play();
                    }
                    SetVisible(true);
                }
            }
            if (m_ProjecileData == null)
            {
                m_fRemainLifeTime = 0;
                m_nRemainHitCount = 0;
                return;
            }

            FFloat frameSpeed = 1.0f;
            //    if (m_fHitStepDelta < 0.01f)
            switch (m_ProjecileData.bornType)
            {
                case EProjecitleBornType.FollowTrigger:
                    {
                        if (m_pOwnerActor == null || m_pOwnerActor.IsFlag(EWorldNodeFlag.Killed) || m_pOwnerActor.IsDestroy())
                        {
                            m_fRemainLifeTime = 0;
                        }
                        else
                        {
                            FVector3 pos = m_pOwnerActor.GetPosition();
                            if (m_pTrackTransform != null)
                            {
                                pos = m_pTrackTransform.position;
                            }
                            SetPosition(pos + m_BornOffset);
                        }
                    }
                    break;
                case EProjecitleBornType.FollowTarget:
                    break;
                case EProjecitleBornType.StartTargetPos:
                    break;
                case EProjecitleBornType.StartTriggerPos:
                    break;
                case EProjecitleBornType.FollowThenTargetPos:
                    break;
                default:
                    {
                        FVector3 curSpeed = FVector3.zero;
                        if (m_ProjecileData.type == EProjectileType.TrackPath)
                        {
                            FFloat fFrameSpeed = m_Speed.z;
                            FVector3 position = FVector3.zero;
                            if (m_fDelayTime <= 0 && EvaluateTrackPath(m_fLifeTime - m_fRemainLifeTime, ref position, ref frameSpeed))
                            {
                                m_Speed.z = Mathf.Max(1.0f * 0.1f, fFrameSpeed);
                                SetPosition(position);
                                FVector3 temp = (position - GetLastPosition());
                                if (temp.sqrMagnitude > 0) SetDirection(temp.normalized);
                            }
                            m_Speed.y = Mathf.Lerp(m_Speed.y, m_Speed.z, fFrameTime * 10);
                            fFrameTime *= m_Speed.y;
                        }
                        else if (m_ProjecileData.type == EProjectileType.Bounce)
                        {
                            if (m_fDelayTime <= 0)
                            {
                                m_Speed.y -= fFrameTime * ConstDef.GTRAVITY_VALUE;
                                if (IsTerrainBelow())
                                {
                                    if (m_Speed.y < 0.0f)
                                    {
                                        if (m_nBounceTypeBounceCount > 0)
                                        {
                                            m_nBounceTypeBounceCount--;
                                            if (m_nBounceTypeBounceCount < 0)
                                            {
                                                m_fRemainLifeTime = 0.01f;
                                                m_nRemainHitCount = 0;
                                            }
                                        }

                                        m_Speed.x = m_Speed.x * m_ProjecileData.speedLerp.y;
                                        m_Speed.z = m_Speed.z * m_ProjecileData.speedLerp.y;
                                        m_Speed.y = -m_Speed.y * m_ProjecileData.speedLerp.y;
                                    }
                                }


                                m_Speed.x += m_Acceleration.x * fFrameTime;
                                m_Speed.y += m_Acceleration.y * fFrameTime;
                                m_Speed.z += m_Acceleration.z * fFrameTime;
                                curSpeed = m_Speed;
                            }

                            if (m_MaxSpeed.x > 0) curSpeed.x = Math.Min(curSpeed.x, m_MaxSpeed.x);
                            if (m_MaxSpeed.y > 0) curSpeed.y = Math.Min(curSpeed.y, m_MaxSpeed.y);
                            if (m_MaxSpeed.z > 0) curSpeed.z = Math.Min(curSpeed.z, m_MaxSpeed.z);
                        }
                        else
                        {
                            if (m_fDelayTime <= 0)
                            {
                                m_Speed.x += m_Acceleration.x * fFrameTime;
                                m_Speed.y += m_Acceleration.y * fFrameTime;
                                m_Speed.z += m_Acceleration.z * fFrameTime;
                                curSpeed = m_Speed;
                            }

                            if (m_MaxSpeed.x > 0) curSpeed.x = Math.Min(curSpeed.x, m_MaxSpeed.x);
                            if (m_MaxSpeed.y > 0) curSpeed.y = Math.Min(curSpeed.y, m_MaxSpeed.y);
                            if (m_MaxSpeed.z > 0) curSpeed.z = Math.Min(curSpeed.z, m_MaxSpeed.z);

                            if (m_HasSpeed)
                            {
                                if (m_fDelayTime > 0 || m_ProjecileData.externLogicSpeed)
                                {
                                    FVector3 runSpeed = m_pGame.GetExternLogicAppendSpeed();
                                    runSpeed.y = 0.0f;
                                    runSpeed.x = 0.0f;
                                    curSpeed += runSpeed;
                                }
                                if (m_fDelayTime <= 0)
                                    curSpeed += m_ExternSpeed;
                                if (m_pOwnerActor != null && m_pOwnerActor is Actor)
                                {
                                    Actor owner = m_pOwnerActor as Actor;
                                    curSpeed += GetDirection() * owner.GetProjectAppendSpeed();
                                }
                            }

                            if (!m_bTrackBegin && (GetPosition() - m_InitPosition).sqrMagnitude >= m_ProjecileData.speedLerp.y * m_ProjecileData.speedLerp.y)
                                m_bTrackBegin = true;
                            if (!m_bTrackBegin && m_IsBoundProjectile) m_bTrackBegin = true;

                            if (UpdateTrackPosition())
                            {
                                FVector3 targetPos = m_TargetPosition;
                                FVector3 targetDir = (targetPos - GetPosition()).normalized;
                                FFloat sqrtDiff = targetDir.sqrMagnitude;
                                if (sqrtDiff > 0.1f)
                                {
                                    if (m_bTrackBegin && m_fDelayTime <= 0)
                                    {
                                        FFloat magnite = curSpeed.magnitude;
                                        if (m_fLifeTime > 0)
                                        {
                                            FFloat scaleLerp = 0.0f;
                                            if (m_bTrackEnd) scaleLerp = 1.0f;
                                            else scaleLerp = (1.0f - Mathf.Clamp01(m_fRemainLifeTime / m_fLifeTime));
                                            if (GetPosition().z >= targetPos.z) m_fTrackSpeedLerp += fFrameTime * 10;
                                            if (m_ProjecileData.speedLerp.x > 0) curSpeed = FVector3.Lerp(curSpeed, targetDir * magnite, scaleLerp * m_ProjecileData.speedLerp.x);
                                            else curSpeed = FVector3.Lerp(curSpeed, targetDir * magnite, scaleLerp);
                                        }
                                        else
                                            curSpeed = targetDir * magnite;

                                        FFloat fSqrGap = Math.Max(1.0f, curSpeed.sqrMagnitude * fFrameTime * fFrameTime);
                                        if (!m_bTrackEnd && (targetPos - GetPosition()).sqrMagnitude <= fSqrGap)
                                        {
                                            m_fRemainLifeTime = fFrameTime * 2;
                                            m_bTrackEnd = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!m_bTrackEnd)
                                    {
                                        m_fRemainLifeTime = fFrameTime * 2;
                                        m_bTrackEnd = true;
                                    }
                                }

                                m_TargetPosition = targetPos;
                            }
                            if (curSpeed.sqrMagnitude > 0.01f)
                                SetDirection(curSpeed.normalized);
                            else
                                SetDirection(m_InitDirection);
                        }
                        FVector3 runtime_position = GetPosition();
                        if (m_fDelayTime > 0)
                        {
                            if (m_pOwnerActor == null) runtime_position = m_InitPosition;
                        }

                        runtime_position += curSpeed * fFrameTime;
                        if (m_fDelayTime > 0)
                        {
                            if (m_pOwnerActor != null)
                            {
                                runtime_position = m_pOwnerActor.GetPosition();
                                //! update m_Position
                                if (!string.IsNullOrEmpty(this.m_strBindOwnerSlot) && this.m_pOwnerActor != null)
                                {
                                    var bindSlot = this.m_pOwnerActor.FindBindSlot(this.m_strBindOwnerSlot);
                                    if (bindSlot != null)
                                    {
                                        FVector3 vPosition = bindSlot.position;
                                        if (this.m_StartBindOffset.sqrMagnitude > 0)
                                        {
                                            vPosition += bindSlot.forward * this.m_StartBindOffset.z;
                                            vPosition += bindSlot.right * this.m_StartBindOffset.x;
                                            vPosition += bindSlot.up * this.m_StartBindOffset.y;
                                        }
                                        runtime_position = vPosition;
                                    }
                                }
                            }
                        }
                        SetPosition(runtime_position);

                        if ((m_ProjecileData.bound_flag & (int)EBoundFlag.PhysicReflectBound) != 0 && m_nRemainBoundCount > 0)
                        {
                            //! check bound
                            RaycastHit hit;
                            if (curSpeed.sqrMagnitude > 0 && TerrainManager.Raycast(GetGameModule(), GetPosition(), GetDirection(), curSpeed.magnitude * fFrameTime, out hit))
                            {
                                m_Speed = Vector3.Reflect((hit.point - GetPosition()).normalized, hit.normal) * curSpeed.magnitude;
                                this.m_nRemainBoundCount--;
                            }
                        }
                    }
                    break;
            }
            if (m_fDelayTime <= 0)
            {
                m_fRemainLifeTime -= fFrameTime;
                m_fRemainLifeTime = Math.Max(0.0f, m_fRemainLifeTime);
            }
            if (m_fDelayTime <= 0)
            {
                m_fHitStepDelta -= fFrameTime;
                m_fHitStepDelta = Math.Max(0.0f, m_fHitStepDelta);
            }
            if (m_pGame != null && m_pGame.projectileManager != null)
                m_pGame.projectileManager.DoCallback(this);
        }
        //------------------------------------------------------
        internal bool CheckStoped(ProjectileManager mgr, FFloat fFrameTime)
        {
            if (m_ProjecileData == null)
                return false;
            if (m_ProjecileData.fEventStepGap > 0)
            {
                m_fEventStepDetla -= fFrameTime;
                if (m_fEventStepDetla <= 0)
                {
                    mgr.AddEvent(ProjectileManager.EEventType.Step, this);

                    m_fEventStepDetla = m_ProjecileData.fEventStepGap;
                }
            }
            if (((m_fRemainLifeTime <= 0.01f) || (m_nRemainHitCount <= 0)))
            {
                if (m_fDelayStopDuration > 0.01f)
                {
                    m_fDelayStopDuration -= fFrameTime;
                    if (m_fDelayStopDuration < 0) m_fDelayStopDuration = 0;
                    return false;
                }

                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool CheckIntersection(ProjectileManager pMgr,out bool isCounteract, out bool isRangeExplode, out bool isHitScene)
        {
            isCounteract = false;
            isRangeExplode = false;
            isHitScene = false;
            if (m_fDelayTime > 0)
                return false;
            if (m_fHitStepDelta > 0.01f || m_nRemainHitCount <= 0)
                return false;
            if (m_ProjecileData == null)
                return false;

            var cacheHits = GetGameModule().shareParams.hitFrameActorCaches;
            var cacheWorldNodes = GetGameModule().shareParams.catchNodeList;
            cacheWorldNodes.Clear();
            cacheHits.Clear();

            WorldBoundBox projBound = this.GetBounds();

            bool bCheckActors = true;
            if (ProjectileData.IsTrack(m_ProjecileData.type))
            {
                bCheckActors = (m_nLaunchFlag & (int)ELaunchFlag.TrackIngoreOtherCollision) == 0;
            }

            if (ProjectileData.IsTrack(m_ProjecileData.type) && m_ProjecileData.type != EProjectileType.TrackPoint)
            {
                if (m_bTrackEnd && m_pTargetNode != null && m_pTargetNode.IsCanLogic()
                        && !IsHited(m_pTargetNode.GetInstanceID(), false))
                {
                    m_fRemainLifeTime = 0;
                    bool bTrackCheck = CanHit(m_pTargetNode);
                    if (bTrackCheck)
                    {
                        bool bFrameHit = true;
                        if (bFrameHit)
                        {
                            StateParam state_param = m_pTargetNode.GetStateParam();
                            HitFrameActor hit = new HitFrameActor(GetDamageID(),this, m_pTargetNode, m_ProjecileData, GetFrameHitPosition(), GetDirection(),
                                m_ProjecileData.classify, m_pStateParam, state_param);
                            hit.damage_power += GetDamagePower();
                            hit.hit_body_part = m_nTrackBodyPart;
                            hit.hitType = IsBoundProjectile() ? EHitType.Bound : EHitType.Unknown;
                            if (hit.hitType == EHitType.Bound)
                                hit.hitType_take_data = new Variable1() { intVal = m_ProjecileData.bound_count - m_nRemainBoundCount };
                            else if (IsPenetrable())
                            {
                                hit.hitType = EHitType.MutiHit;
                                hit.hitType_take_data = new Variable1() { intVal = m_ProjecileData.hit_count - m_nRemainHitCount };
                            }
                            hit.attacker_target_count = GetTargetHitCount();
                            if (IsBoundProjectile()) hit.attacker_target_count++;
                            cacheHits.Add(hit);
                            RecodeHit(m_pTargetNode.GetInstanceID(), 0.1f);
                        }
                        else if (!bCheckActors)
                        {
                            cacheWorldNodes.Add(m_pTargetNode);
                        }
                    }
                    else if (!bCheckActors)
                    {
                        cacheWorldNodes.Add(m_pTargetNode);
                    }
                }
                else if (!bCheckActors && m_pTargetNode != null)
                {
                    cacheWorldNodes.Add(m_pTargetNode);
                }
            }
            bool bPenetrableHit = false;
            bool bProjectHit = false;
            bool bCheck = false;

            if (bCheckActors)
            {
                GetGameModule().gameWorld.CalcNeighbors(this, Mathf.Max(5, projBound.GetBoundSize() * 1.5f), ref cacheWorldNodes);
            }
            if (cacheWorldNodes.Count > 0)
            {
                AWorldNode pTarget;
                for (int c = 0; c < cacheWorldNodes.Count; ++c)
                {
                    if (m_nRemainHitCount <= 0 || m_nRemainHitCount <= cacheHits.Count) break;
                    pTarget = cacheWorldNodes[c];
                    if (pTarget == this || pTarget.GetActorType() == EActorType.Projectile) continue;
                    bCheck = CanHit(pTarget);
                    if (!bCheck)
                        continue;

                    if (IsPenetrable())
                    {
                        if (IsHited(pTarget.GetInstanceID(), false))
                        {
                            bCheck = false;
                        }
                    }
                    if (bCheck)
                    {
                        if (m_nTrackBodyPart != 0xffffffff)
                        {
                            Actor actorTarget = pTarget as Actor;
#if USE_ACTORSYSTEM
                            if (actorTarget != null)
                                AttackFrameUtil.CU_ProjectileAttackFrameInsector(this, actorTarget, actorTarget.GetCurrentPlayActionState(), EVolumeType.PartTarget, ref cacheHits, ref bPenetrableHit);
#endif
                        }
                        else if (m_nTrackBodyPart == 0xffffffff && cacheHits.Count < m_nMaxOneFrameHit)
                        {
                            Actor actorTarget = pTarget as Actor;
#if USE_ACTORSYSTEM
                            if (actorTarget != null)
                            {
                                AttackFrameUtil.CU_ProjectileAttackFrameInsector(this, actorTarget, actorTarget.GetCurrentPlayActionState(), EVolumeType.Target, ref cacheHits, ref bPenetrableHit);
                            }
#endif         
                            {
                                Matrix4x4 targetWorld = pTarget.GetMatrix();
                                var vMin1 = pTarget.GetBounds().GetCenter();
                                var vMax1 = pTarget.GetBounds().GetHalf();
                                if (IsIntersecition(targetWorld, vMin1, vMax1))
                                {
                                    HitFrameActor hit = new HitFrameActor(GetDamageID(),this, pTarget, m_ProjecileData, GetFrameHitPosition(pTarget), GetDirection(),
                                        GetClassify(),m_pStateParam);
                                    hit.damage_power += GetDamagePower();
                                    hit.hit_body_part = m_nTrackBodyPart;
                                    hit.hitType = IsBoundProjectile() ? EHitType.Bound : EHitType.Unknown;
                                    if (hit.hitType == EHitType.Bound)
                                        hit.hitType_take_data = new Variable1() { intVal = m_ProjecileData.bound_count - m_nRemainBoundCount };
                                    else if (IsPenetrable())
                                    {
                                        hit.hitType = EHitType.MutiHit;
                                        hit.hitType_take_data = new Variable1() { intVal = m_ProjecileData.hit_count - m_nRemainHitCount };
                                    }
                                    hit.attacker_target_count = GetTargetHitCount() + cacheHits.Count;
                                    if (IsBoundProjectile()) hit.attacker_target_count++;
                                    cacheHits.Add(hit);
                                    RecodeHit(pTarget.GetInstanceID(), 0.1f);
                                }
                            }
                        }
                    }
                    if (cacheHits.Count >= m_nMaxOneFrameHit) break;
                }
            }


            if (cacheHits.Count > 0)
            {
                bProjectHit = true;
            }
            else if (CanSceneTest())
            {
                FVector3 hitPos;
                FVector3 hitUp;
                if (TerrainManager.IsTerrainBelowProj(GetGameModule(), GetPosition(), out hitPos))
                {
                    SetPosition(hitPos);
                    isHitScene = true;
                    bProjectHit = true;
                }
                else if (TerrainManager.IsPhysicHit(GetGameModule(), GetPosition(), GetUp(), out hitPos, out hitUp, 0.1f))
                {
                    SetPosition(hitPos);
                    isHitScene = true;
                    bProjectHit = true;
                }
            }

            if (bProjectHit)
            {
                if (m_ProjecileData.explode_range > 0.01f)
                {
                    isRangeExplode = true;
                }
                AWorldNode pAttacker = m_pOwnerActor;
                if(pAttacker != null)
                {
                    foreach (var hitData in cacheHits)
                    {
                        if (hitData.target_ptr != null)
                        {
                            for (int j = 0; j < hitData.mul_hit_cnt; ++j)
                            {
                                m_TotalDamage += GetGameModule().OnHitFrameDamage(hitData);
                            }
                        }
                    }

                    pMgr.DoProjectileHit(this, pAttacker, 0, ref cacheHits, false, isHitScene, bPenetrableHit ? m_nRemainHitCount : 1);
                }
            }
            if (!IsCanLogic() && m_pOwnerActor != null && m_ProjecileData.counteract)
            {
                isCounteract = true;
            }
            return cacheHits.Count>0;
        }
        //------------------------------------------------------
        internal void OnSpawnWarningCallback(InstanceOperiaon instanceOp)
        {

        }
        //------------------------------------------------------
        public bool TestFinalDropPos()
        {
            m_HasDropPoint = false;
            m_FinalDropPosition = Vector3.zero;
            if (m_ProjecileData.type != EProjectileType.Projectile || m_ProjecileData.life_time <= 0 || m_ProjecileData.waring_duration <= 0) return false;
            if (string.IsNullOrEmpty(m_ProjecileData.waring_effect)) return false;
            if (this.m_Acceleration.y > 0.01f)
            {
                m_HasDropPoint = true;
                m_FinalDropPosition = FVector3.zero;
                return m_HasDropPoint;
            }

            Vector3 max_speed = this.m_MaxSpeed;
            Vector3 curSpeed = this.m_Speed;
            Vector3 curAcceleration = this.m_Acceleration;
            float fDuration = 0;
            Vector3 duration_speed = this.m_Speed;
            Vector3 curPos = this.GetPosition();
            float fDelta = Mathf.Max(m_pGame.TimeDelta, m_ProjecileData.life_time / 10);
            while (fDuration <= m_ProjecileData.life_time)
            {
                duration_speed.x += m_Acceleration.x * fDelta;
                duration_speed.y += m_Acceleration.y * fDelta;
                duration_speed.z += m_Acceleration.z * fDelta;

                if (max_speed.x > 0) duration_speed.x = Mathf.Min(duration_speed.x, max_speed.x);
                if (max_speed.y > 0) duration_speed.y = Mathf.Min(duration_speed.y, max_speed.y);
                if (max_speed.z > 0) duration_speed.z = Mathf.Min(duration_speed.z, max_speed.z);

                curPos = curPos + duration_speed * fDelta;
                if (curPos.y < 0)
                {
                    m_FinalDropPosition = curPos - this.GetPosition();
                    m_HasDropPoint = true;
                    break;
                }
                fDuration += fDelta;
            }
            return m_HasDropPoint;
        }
        //------------------------------------------------------
        public FVector3 GetFrameHitPosition(AWorldNode pTarget = null)
        {
            if (pTarget != null)
            {
                Transform pSlot = pTarget.FindBindSlot(string.IsNullOrEmpty(m_ProjecileData.effect_hit_slot) ? "F_chest01" : m_ProjecileData.effect_hit_slot);
                if (pSlot != null) return pSlot.position + pSlot.forward * m_ProjecileData.target_effect_hit_offset.z + pSlot.right * m_ProjecileData.target_effect_hit_offset.x + pSlot.up * m_ProjecileData.target_effect_hit_offset.y;
                return pTarget.GetPosition() + pTarget.GetDirection() * m_ProjecileData.target_effect_hit_offset.z + pTarget.GetRight() * m_ProjecileData.target_effect_hit_offset.x + pTarget.GetUp() * m_ProjecileData.target_effect_hit_offset.y;
            }
            if (m_pTargetNode != null)
            {
                Transform pSlot = m_pTargetNode.FindBindSlot(string.IsNullOrEmpty(m_ProjecileData.effect_hit_slot) ? "F_chest01" : m_ProjecileData.effect_hit_slot);
                if (pSlot != null) return pSlot.position + m_ProjecileData.target_effect_hit_offset;
                return m_pTargetNode.GetPosition() + m_ProjecileData.target_effect_hit_offset;
            }
            if (m_HasSpeed) return GetPosition() + m_ProjecileData.target_effect_hit_offset;
            return GetPosition() + m_ProjecileData.target_effect_hit_offset;
        }
        //------------------------------------------------------
        public FVector3 GetFrameExplodePosition(AWorldNode pTarget = null)
        {
            if (pTarget != null)
            {
                return pTarget.GetPosition() + m_ProjecileData.explode_effect_offset;
            }
            if (m_HasSpeed) return GetPosition() + m_ProjecileData.explode_effect_offset;
            return GetPosition() + m_ProjecileData.explode_effect_offset;
        }
        //------------------------------------------------------
        public int GetTargetHitCount()
        {
            if (m_on_hit_actors == null) return 0;
            return m_on_hit_actors.Count;
        }
        //------------------------------------------------------
        public void RecodeHit(int nFlagID, float fDelay = 0)
        {
            if (m_on_hit_actors == null) m_on_hit_actors = new Dictionary<int, int>(4);
            m_on_hit_actors[nFlagID] = (int)(m_pGame.GetRunTime() + (int)(fDelay * 1000));
        }
        //------------------------------------------------------
        public override bool IsIntersecition(AWorldNode pNode)
        {
            if (m_ProjecileData == null || pNode == null) return false;
            WorldBoundBox bound = pNode.GetBounds();
            if (m_ProjecileData.collisionType == EProjectileCollisionType.CAPSULE)
            {
                if (Base.IntersetionUtil.CU_SphereAABBInstersection(GetPosition(), (FFloat)m_ProjecileData.aabb_min.x, pNode.GetPosition(), FVector3.one * 0.01f))
                    return true;
            }
            else
            {
                if (Base.IntersetionUtil.CU_LineOBBIntersection(GetShareFrameParams().intersetionParam, GetLastPosition(), GetPosition(), bound.GetCenter(), bound.GetHalf(), pNode.GetMatrix()) ||
                    Base.IntersetionUtil.CU_OBBOBBIntersection(GetShareFrameParams().intersetionParam, bound.GetCenter(), bound.GetHalf(), pNode.GetMatrix(), GetBounds().GetCenter(), GetBounds().GetHalf(), GetMatrix()))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public override bool IsIntersecition(FMatrix4x4 mtTrans, FVector3 vCenter, FVector3 vHalf)
        {
            if (m_ProjecileData == null) return false;
            if (m_ProjecileData.collisionType == EProjectileCollisionType.CAPSULE)
            {
                FVector3 vTransCenter = mtTrans.MultiplyPoint(vCenter);
                if (Base.IntersetionUtil.CU_SphereAABBInstersection(GetPosition(), (FFloat)m_ProjecileData.aabb_min.x, vTransCenter - vHalf, vTransCenter+vHalf))
                    return true;
            }
            else
            {
                if (this.m_HasSpeed && Base.IntersetionUtil.CU_LineOBBIntersection(GetShareFrameParams().intersetionParam, GetLastPosition(), GetPosition(), vCenter, vHalf, mtTrans))
                {
                    return true;
                }
                if (Base.IntersetionUtil.CU_OBBOBBIntersection(GetShareFrameParams().intersetionParam, vCenter, vHalf, mtTrans, GetBounds().GetCenter(), GetBounds().GetHalf(), GetMatrix()))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public override bool IsIntersecition(FMatrix4x4 mtTrans, FFloat radius)
        {
            if (m_ProjecileData == null) return false;
            if (m_ProjecileData.collisionType == EProjectileCollisionType.CAPSULE)
            {
                FVector3 vTransCenter = BaseUtil.GetPosition(mtTrans);
                if (Base.IntersetionUtil.CU_SphereSphereInstersection(GetPosition(), (FFloat)m_ProjecileData.aabb_min.x, vTransCenter, radius))
                    return true;
            }
            else
            {
                FVector3 vTransCenter = BaseUtil.GetPosition(mtTrans);
                if (this.m_HasSpeed && Base.IntersetionUtil.CU_LineSphereIntersection(GetShareFrameParams().intersetionParam, GetLastPosition(), GetPosition(), vTransCenter, radius))
                {
                    return true;
                }
                if (Base.IntersetionUtil.CU_SphereAABBInstersection(vTransCenter, radius, GetBounds().GetMin(true), GetBounds().GetMax(true)))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsHited(int nFlag, bool bCheckTime = true)
        {
            if (m_on_hit_actors == null) return false;
            int lastTime = 0;
            if (m_on_hit_actors.TryGetValue(nFlag, out lastTime))
            {
                if (IsBoundInvert() && m_IsBoundProjectile)
                {
                    return true;
                }
                else
                {
                    if (bCheckTime)
                        return (m_pGame.GetRunTime() - lastTime) < GetHitStep();
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public void CopyHits(ProjectileNode pProj)
        {
            if (pProj.m_on_hit_actors == null) return;
            if (m_on_hit_actors == null) m_on_hit_actors = new Dictionary<int, int>(4);
            foreach (var db in pProj.m_on_hit_actors)
            {
                m_on_hit_actors[db.Key] = db.Value;
            }
        }
        //------------------------------------------------------
        public bool IsBounded(AWorldNode pNode)
        {
            if (m_bounded_list == null) return false;
            int id = (int)pNode.GetActorType() * 10 + pNode.GetInstanceID();
            return m_bounded_list.Contains(id);
        }
        //------------------------------------------------------
        public void CopyBoundeds(ProjectileNode pProj)
        {
            if (pProj.m_bounded_list == null) return;
            if (m_bounded_list == null) m_bounded_list = new HashSet<int>(2);
            foreach (var db in pProj.m_bounded_list)
            {
                m_bounded_list.Add(db);
            }
        }
        //------------------------------------------------------
        public void AddBounded(AWorldNode pNode)
        {
            if (m_bounded_list == null) m_bounded_list = new HashSet<int>(2);
            int id = (int)pNode.GetActorType() * 10 + pNode.GetInstanceID();
            m_bounded_list.Add(id);
        }
        //------------------------------------------------------
        public bool IsBoundInvert()
        {
            if (m_ProjecileData == null) return false;
            return (m_ProjecileData.bound_flag & (int)EBoundFlag.BoundInversion) != 0;
        }
        //------------------------------------------------------
        int GetHitStep()
        {
            return m_ProjecileData.hit_step > 0 ? (int)(m_ProjecileData.hit_step * 1000) : 500;
        }
        //------------------------------------------------------
        public bool CanHit(AWorldNode pNode)
        {
            if (!pNode.IsCanLogic()) return false;

            bool isFriend = GetAttackGroup() == pNode.GetAttackGroup();
            Actor pActor = pNode as Actor;
            if (pActor != null)
            {
                if (pActor.IsInvincible()) return false;
            }
            if (m_IsBoundProjectile && IsBoundInvert())
            {
                if (m_on_hit_actors != null && m_on_hit_actors.ContainsKey(pNode.GetInstanceID()))
                    return false;
            }
            return true;
        }
        //------------------------------------------------------
        public override byte GetClassify()
        {
            if (m_pOwnerActor != null)
                return m_pOwnerActor.GetClassify();
            return 0;
        }
        //------------------------------------------------------
        public override uint GetConfigID()
        {
            if (m_ProjecileData == null) return 0;
            return m_ProjecileData.id;
        }
        //------------------------------------------------------
        public override uint GetElementFlags()
        {
            if (m_pOwnerActor != null)
                return m_pOwnerActor.GetElementFlags();
            return 0;
        }
        //------------------------------------------------------
        public override void SetElementFlags(uint flags)
        {

        }
        //------------------------------------------------------
        public override FFloat GetTimeSpeed()
        {
            if (m_pOwnerActor != null) return m_pOwnerActor.GetTimeSpeed();
            return base.GetTimeSpeed();
        }
        //------------------------------------------------------
        public void BuildTrackPathKeyframe(FVector3 vStart, FVector3 vEnd, float fSpeed = 0)
        {
            if (m_ProjecileData == null) return;
            if (m_ProjecileData.IsValidTrackPath())
            {
                if (fSpeed == 0)
                    this.m_Speed.x = GetGameModule().GetRamdom(m_ProjecileData.speedLerp.x, m_ProjecileData.speedLerp.y);
                else
                    this.m_Speed.x = fSpeed;
                if (this.m_Speed.x > 0)
                {
                    m_HasSpeed = m_Speed.sqrMagnitude > 0.1f;
                    if (m_vTrackKeyframes == null) m_vTrackKeyframes = new List<ProjectileKeyframe>();
                    m_vTrackKeyframes.Clear();

                    EProjectileParabolicType parabolicType = (EProjectileParabolicType)(int)m_ProjecileData.accelerations[0].y;
                    FFloat distance = (vEnd - vStart).magnitude;
                    if (distance <= 0) return;

                    if ((m_ProjecileData.launch_flag & (int)ELaunchFlag.DirectionLuanch) == 0)
                    {
                        m_InitDirection = (vEnd - vStart).normalized;
                    }
                    FQuaternion vRot = FQuaternion.LookRotation(m_InitDirection);


                    FFloat fPathLength = 0.0f;
                    if (parabolicType == EProjectileParabolicType.StartEnd)
                    {
                        if (m_ProjecileData.speedMaxs.Length == 2)
                        {
                            fPathLength = distance;
#if USE_FIXEDMATH
                            ProjectileKeyframe keyFrame = new ProjectileKeyframe(0, m_ProjecileData.accelerations[0].x, BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedMaxs[0], vRot) + vStart, FVector3.zero, BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedUppers[0], vRot));
                            m_vTrackKeyframes.Add(keyFrame);

                            keyFrame = new ProjectileKeyframe(fPathLength / m_Speed.x, m_ProjecileData.accelerations[1].x, BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedMaxs[1], vRot) + vEnd, BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedLows[1], vRot), FVector3.zero);
                            m_vTrackKeyframes.Add(keyFrame);
#else
                            ProjectileKeyframe keyFrame = new ProjectileKeyframe(0, m_ProjecileData.accelerations[0].x, BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedMaxs[0], vRot) + vStart, FVector3.zero, BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedUppers[0], vRot));
                            m_vTrackKeyframes.Add(keyFrame);

                            keyFrame = new ProjectileKeyframe(fPathLength / m_Speed.x, m_ProjecileData.accelerations[1].x, BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedMaxs[1], vRot) + vEnd, BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedLows[1], vRot), FVector3.zero);
                            m_vTrackKeyframes.Add(keyFrame);
#endif
                            m_fRemainLifeTime = (fPathLength + 2f) / m_Speed.x;
                            m_fLifeTime = m_fRemainLifeTime;
                            m_Speed.y = 1.0f;
                            m_Speed.z = 1.0f;
                        }
                    }
                    else
                    {
                        ProjectileKeyframe keyFrame = new ProjectileKeyframe(0, 1.0f, vStart, FVector3.zero, FVector3.zero);
                        FVector3 tempPos = vStart;
                        m_vTrackKeyframes.Add(keyFrame);
                        FFloat fPathSegmentLength = 0.0f;
                        for (int i = 0; i < m_ProjecileData.speedMaxs.Length; ++i)
                        {
                            FVector3 start = tempPos;
#if USE_FIXEDMATH
                            FVector3 pt = BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedMaxs[i], vRot);
#else
                            FVector3 pt = BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedMaxs[i], vRot);
#endif
                            FVector3 end = vStart + pt;
                            fPathSegmentLength = (end - start).magnitude;
                            if (fPathSegmentLength <= 0) continue;

                            fPathLength += fPathSegmentLength;
                            FFloat fTime = fPathLength / this.m_Speed.x;
#if USE_FIXEDMATH
                            FVector3 inTan = BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedLows[i], vRot);
                            FVector3 outTan = BaseUtil.FRoateAround(FVector3.zero, m_ProjecileData.speedUppers[i], vRot);
#else
                            FVector3 inTan = BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedLows[i], vRot);
                            FVector3 outTan = BaseUtil.RoateAround(FVector3.zero, m_ProjecileData.speedUppers[i], vRot);
#endif
                            keyFrame = new ProjectileKeyframe(fTime, m_ProjecileData.accelerations[i].x, end, inTan, outTan);
                            m_vTrackKeyframes.Add(keyFrame);
                            tempPos = end;
                        }
                        if (parabolicType == EProjectileParabolicType.TrackPathLinkEnd)
                        {
                            fPathSegmentLength = (vEnd - tempPos).magnitude;
                            if (fPathSegmentLength > 0)
                            {
                                fPathLength += fPathSegmentLength;
                                FFloat fTime = fPathLength / this.m_Speed.x;

                                keyFrame = new ProjectileKeyframe(fTime, m_vTrackKeyframes[m_vTrackKeyframes.Count - 1].time, vEnd, FVector3.zero, FVector3.zero);
                                m_vTrackKeyframes.Add(keyFrame);
                            }
                        }
                        m_fRemainLifeTime = (fPathLength + 2f) / m_Speed.x;
                        m_fLifeTime = m_fRemainLifeTime;
                        m_Speed.y = 1.0f;
                        m_Speed.z = 1.0f;
                    }
                }
            }
            m_HasSpeed = m_Speed.sqrMagnitude > 0;
        }
        //------------------------------------------------------
        public bool EvaluateTrackPath(FFloat fTime, ref FVector3 vPosition, ref FFloat fSpeed)
        {
            fSpeed = 1.0f;
            if (m_ProjecileData == null || m_ProjecileData.type != EProjectileType.TrackPath || m_fLifeTime <= 0 || m_vTrackKeyframes == null) return false;
            if (m_vTrackKeyframes.Count <= 0) return false;

            if (this.m_pTargetNode != null && IsLaunchFlaged(ELaunchFlag.RefreshEndPoint))
            {
                ProjectileKeyframe keyFrame = m_vTrackKeyframes[m_vTrackKeyframes.Count - 1];
                if (IsLaunchFlaged(ELaunchFlag.RefreshEndPoint))
                {
                    FVector3 targetPos = m_pTargetNode.GetPosition();
                    if (UpdateTrackPosition())
                    {
                        targetPos = m_TargetPosition;
                    }
                    keyFrame.point = targetPos;
                    m_vTrackKeyframes[m_vTrackKeyframes.Count - 1] = keyFrame;
                }
            }

            if (fTime <= m_vTrackKeyframes[0].time)
            {
                vPosition = m_vTrackKeyframes[0].point;
                return true;
            }
            if (fTime >= m_vTrackKeyframes[m_vTrackKeyframes.Count - 1].time)
            {
                vPosition = m_vTrackKeyframes[m_vTrackKeyframes.Count - 1].point;
                return true;
            }

            int __len = m_vTrackKeyframes.Count;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (fTime < m_vTrackKeyframes[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(m_vTrackKeyframes.Count - 1, __first);

            if (lhs < 0 || lhs >= m_vTrackKeyframes.Count || rhs < 0 || rhs >= m_vTrackKeyframes.Count)
                return false;

            ProjectileKeyframe lhsKey = m_vTrackKeyframes[lhs];
            ProjectileKeyframe rhsKey = m_vTrackKeyframes[rhs];

            FFloat dx = rhsKey.time - lhsKey.time;
            FVector3 m1 = FVector3.zero, m2 = FVector3.zero;
            FFloat t;
            if (dx != 0f)
            {
                t = (fTime - lhsKey.time) / dx;
            }
            else
                t = 0;

            m1 = lhsKey.point + lhsKey.outTan;
            m2 = rhsKey.point + rhsKey.inTan;
#if USE_FIXEDMATH
            vPosition = BezierUtility.FBezier4(t, lhsKey.point, m1, m2, rhsKey.point);
            fSpeed = ExternEngine.FMath.Lerp(lhsKey.m_Speed, rhsKey.m_Speed, t);
#else
            vPosition = BezierUtility.Bezier4(t, lhsKey.point, m1, m2, rhsKey.point);
            fSpeed = Mathf.Lerp(lhsKey.speed, rhsKey.speed, t);
#endif

            return true;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override void DrawDebug()
        {
            UnityEditor.Handles.Label(GetPosition(), GetActorType().ToString() + ":" + GetInstanceID().ToString() + " config:" + GetConfigID());
            if (m_ProjecileData == null) return;
            if (m_ProjecileData.collisionType == EProjectileCollisionType.BOX)
                ED.EditorUtil.DrawBoundingBox(m_BoundBox.GetCenter(), m_BoundBox.GetHalf(), GetMatrix(), ED.EditorUtil.GetVolumeToColor(EVolumeType.Target), true);
            else
                Gizmos.DrawWireSphere(GetPosition(), m_ProjecileData.minRotate.x);
            if (m_ProjecileData != null && m_ProjecileData.explode_range > 0)
            {
                ED.EditorUtil.DrawBoundingBox(GetPosition(), Vector3.one * m_ProjecileData.explode_range, Matrix4x4.identity, Color.cyan, true);
            }
            DrawTrackPath(Color.blue);
        }
        //------------------------------------------------------
        public void DrawTrackPath(Color color)
        {
            if (m_vTrackKeyframes == null || m_vTrackKeyframes.Count <= 0) return;
            Vector3 prePos = Vector3.zero;
            float fMax = 0;
            for (int i = 0; i < m_vTrackKeyframes.Count; ++i) fMax = Mathf.Max(fMax, m_vTrackKeyframes[i].time);
            Color bak = UnityEditor.Handles.color;
            UnityEditor.Handles.color = color;
            prePos = m_vTrackKeyframes[0].point;
            float time = 0f;
            while (time < fMax)
            {
                FVector3 pos = Vector3.zero;
                FFloat m_Speed = 1.0f;
                EvaluateTrackPath(time, ref pos, ref m_Speed);

                UnityEditor.Handles.DrawLine(prePos + GetPosition(), pos + GetPosition());

                prePos = pos;
                time += 0.01f;
            }
            UnityEditor.Handles.color = bak;
        }
#endif
    }
}

