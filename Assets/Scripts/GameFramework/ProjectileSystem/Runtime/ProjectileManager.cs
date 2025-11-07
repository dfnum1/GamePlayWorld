/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ProjectileManager
作    者:	HappLI
描    述:	飞行道具管理器
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
#if USE_SERVER
using Transform = ExternEngine.Transform;
#endif

namespace Framework.Core
{
    public interface ProjectileManagerCB
    {
        void OnLaunchProjectile(ProjectileNode pProjectile);
		void OnProjectileHit(ProjectileNode pProjectile, HitFrameActor hitedActor, bool bHitScene, bool bExplode);
		void OnStopProjectile(ProjectileNode pProjectile);
        void OnDelayStopProjectile(ProjectileNode pProjectile);
        void OnProjectileUpdate(ProjectileNode pProjectile);
    }
    //------------------------------------------------------
    public class ProjectileManager : AModule
    {
        struct BoundProjectile
        {
            public ProjectileNode projectile;
            public int bound_count;
            public byte attackGroup;
            public AWorldNode pTarget;
            public AWorldNode hitActor;
        }
        public enum EEventType
        {
            Step,
            Attacker,
            Over,
            Hit,
        }
        struct Event
        {
            public AWorldNode node_ptr;
            public AWorldNode target_ptr;
            public EEventType type;
            public ProjectileNode pProjectile;
            public int eventId;

            public Vector3 runtime_pos;
            public Vector3 runtime_dir;
            public Vector3 runtime_hit;

            public StateParam stateParam;

            public Event(EEventType type, ProjectileNode pProjectile, AWorldNode node_ptr, AWorldNode target_ptr, int eventId, Vector3 runtime_hit, StateParam stateParam)
            {
                this.eventId = eventId;
                this.pProjectile = pProjectile;
                this.type = type;
                this.node_ptr = node_ptr;
                this.target_ptr = target_ptr;
                runtime_pos = pProjectile.GetPosition();
                runtime_dir = pProjectile.GetDirection();
                this.runtime_hit = runtime_hit;
                this.stateParam = stateParam;
            }
        }

        private Dictionary<int, ProjectileNode> m_mRunningProjectile;
        private HashSet<ProjectileManagerCB> m_vProjectileManagerCB;

        Dictionary<uint, ProjectileData> m_vDatas = null;

        private List<BoundProjectile> m_vPrepareBounds = new List<BoundProjectile>(4);

        private List<Event> m_vTickEventTemps = new List<Event>(4);

        private List<ProjectileNode> m_CatchTemp = null;

        List<ProjectileNode> m_vRangeExplodeProjectiles = null;
        List<ProjectileNode> m_vCounteractProjectiles = null;

        //------------------------------------------------------
        ~ProjectileManager()
        {
            m_CatchTemp = null;
            m_vPrepareBounds = null;
            m_mRunningProjectile = null;
            m_vProjectileManagerCB = null;
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_CatchTemp = new List<ProjectileNode>(16);
            if (m_vTickEventTemps == null) m_vTickEventTemps = new List<Event>(4);
            m_vTickEventTemps.Clear();
             m_mRunningProjectile = new Dictionary<int, ProjectileNode>(16);
            m_vProjectileManagerCB = new HashSet<ProjectileManagerCB>();
            if (m_vPrepareBounds == null) m_vPrepareBounds = new List<BoundProjectile>(4);
        }
        //------------------------------------------------------
        public void RefreshDatas()
        {
            ProjectileDatas projecitleDatas = GetFramework().GetBindData<ProjectileDatas>();
            if (projecitleDatas == null)
                return;
            if (m_vDatas == null) m_vDatas = new Dictionary<uint, ProjectileData>();
            projecitleDatas.GetDatas(m_vDatas);
        }
        //------------------------------------------------------
        public ProjectileData GetProjectileData(uint nId)
        {
            if (m_vDatas == null)
                return null;
            ProjectileData outData;
            if (m_vDatas.TryGetValue(nId, out outData))
                return outData;
            return null;
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            m_vPrepareBounds.Clear();
            m_vTickEventTemps.Clear();
            StopAllProjectiles();
            m_vProjectileManagerCB.Clear();
            if (m_vCounteractProjectiles != null) m_vCounteractProjectiles.Clear();
            if (m_vRangeExplodeProjectiles != null) m_vRangeExplodeProjectiles.Clear();
        }
        //------------------------------------------------------
        public void Reset()
        {
            OnDestroy();
        }
        //------------------------------------------------------
        public void StopProjectileByOwner(AWorldNode pNode, float fLaucherTime)
        {
            if (pNode == null) return;
            foreach(var db in m_mRunningProjectile)
            {
                ProjectileNode pProjectile = db.Value;
                if(pProjectile.GetOwnerActor() == pNode)
                {
                    if(pProjectile.GetDelayTime()>0 || pProjectile.GetRunningTime() <= fLaucherTime)
                    {
                        pProjectile.SetRemainLifeTime(0);
                    }
                }
            }
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrameTime)
        {
            for (int i = 0; i < m_vPrepareBounds.Count; ++i)
            {
                DoBoundLaunchProjectile(m_vPrepareBounds[i]);
            }
            m_vPrepareBounds.Clear();

            if (m_vCounteractProjectiles == null) m_vCounteractProjectiles = new List<ProjectileNode>(4);
            m_vCounteractProjectiles.Clear();
            if (m_vRangeExplodeProjectiles == null) m_vRangeExplodeProjectiles = new List<ProjectileNode>(4);
            m_vRangeExplodeProjectiles.Clear();

            ProjectileNode pProjectile = null;
            foreach (var db in m_mRunningProjectile)
            {
                pProjectile = db.Value;
                if(pProjectile.IsDestroy() || pProjectile.IsKilled())
                {
                    m_CatchTemp.Add(pProjectile);
                    continue;
                }
                if(pProjectile.CheckStoped(this,fFrameTime))
                {
                    m_CatchTemp.Add(pProjectile);
                    continue;
                }

                if(pProjectile.CheckIntersection(this, out var bCounteract, out var bExplode, out var bHitScene))
                {
                    if (bCounteract)
                        m_vCounteractProjectiles.Add(pProjectile);
                    if (bExplode)
                        m_vRangeExplodeProjectiles.Add(pProjectile);
                 //   DoProjectileHit(pProjectile, pAttacker, 0, ref m_AttackDataArray, false, bHitScene, pProjectile.IsPenetrable() ? pProjectile.GetRemainHitCount() : 1);
                }
            }

            for (int i = 0; i < m_vTickEventTemps.Count; ++i)
            {
                Event evt = m_vTickEventTemps[i];
                switch(evt.type)
                {
                    case EEventType.Attacker:
                        {
                            DoAttackEvent(evt);
                        }
                        break;
                    case EEventType.Hit:
                        {
                            DoHitEvent(evt);
                        }
                        break;
                    case EEventType.Step:
                        {
                            DoStepEvent(evt);
                        }
                        break;
                    case EEventType.Over:
                        {
                            DoOverEvent(evt);
                        }
                        break;
                }
            }
            m_vTickEventTemps.Clear();

            for (int i = 0; i < m_CatchTemp.Count; ++i)
            {
                StopProjectile(m_CatchTemp[i]);
            }
            m_CatchTemp.Clear();
        }
        //------------------------------------------------------
        internal void DoCallback(ProjectileNode projectile)
        {
            if (m_vProjectileManagerCB == null) return;
            foreach (var cb in m_vProjectileManagerCB)
                cb.OnProjectileUpdate(projectile);
        }
        //------------------------------------------------------
        internal void AddEvent(EEventType type, ProjectileNode pProjectile)
        {
            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null)
                return;
            m_vTickEventTemps.Add(new Event(type, pProjectile, pProjectile.GetOwnerActor(), null, projectileData.StepEventID, pProjectile.GetPosition(), pProjectile.GetStateParam()));
        }
        //------------------------------------------------------
        public void TrackCheck(AWorldNode pTargetNode, FVector3 vPosition, ProjectileData pData, Transform pTrackTransform, ref Transform pTrackSlot, ref int damage_power, ref uint track_frame_id, ref uint track_body_id, ref FVector3 trackOffset)
        {
            damage_power = 0;
            track_frame_id = 0xffffffff;
            track_body_id = 0xffffffff;
            pTrackSlot = null;
            trackOffset = FVector3.zero;
            if (pData == null) return;
            if (ProjectileData.IsTrack(pData.type))
            {
                if (pTrackTransform == null)
                {
                    Actor pTargetActor = pTargetNode as Actor;
                    if (pTrackSlot == null && pData.track_target_slot != null && pData.track_target_slot.Length > 0)
                    {
                        if (pTargetNode is Actor)
                        {
                            Actor actorTarget = pTargetNode as Actor;
                            Transform slot = pTargetNode != null ? actorTarget.GetEventSlot(pData.track_target_slot[m_pFramework.GetRamdom(0, pData.track_target_slot.Length)], ref trackOffset) : null;
                            if (slot != null)
                                pTrackSlot = slot;
                        }
                        else if (pTargetNode.IsObjected())
                        {
#if !USE_SERVER
                            pTrackSlot = pTargetNode.GetObjectAble().GetTransorm();
#endif
                        }
                    }
                }
                else
                    pTrackSlot = pTrackTransform;
            }
        }
        //------------------------------------------------------
        public int LaunchProjectile(uint dwProjectileTableID, AWorldNode pOwnerActor, StateParam stateParam,
        FVector3 vPosition, FVector3 vDirection, AWorldNode targetNode = null, int dwAssignedID = 0,
        float fDelta =0, Transform pTrackTransform = null, List<IUserData> vResults = null)
        {
            if (pOwnerActor == null) return 0;
            ProjectileData pData = GetProjectileData(dwProjectileTableID);
            if (pData != null) return 0;
            return LaunchProjectile(pData, pOwnerActor, stateParam, vPosition, vDirection, targetNode, dwAssignedID, fDelta, pTrackTransform, vResults);
        }
        //------------------------------------------------------
        public int LaunchProjectile(ProjectileData pData, AWorldNode pOwnerActor, StateParam stateParam,
        FVector3 vPosition, FVector3 vDirection, AWorldNode targetNode = null, int dwAssignedID = 0,
        float fDelta = 0, Transform pTrackTransform = null, List<IUserData> vResults = null)
        {
            if (pOwnerActor == null || pData == null) return 0;
            int dwID = 0;
            uint dwProjectileTableID = pData.id;
#if UNITY_EDITOR
            if (pData.life_time <= 0)
            {
                Debug.LogError("飞行道具" + pData.id + " 没生命时长!");
                return dwID;
            }
#endif
#if UNITY_EDITOR
            if (pData.hit_count <= 0)
            {
                Debug.LogError("飞行道具" + pData.id + " 没有攻击次数!");
                return dwID;
            }
#endif
            if (targetNode != null && (targetNode.IsFlag(EWorldNodeFlag.Killed) || targetNode.IsDestroy()))
            {
#if UNITY_EDITOR
                Debug.LogWarning("飞行道具" + pData.id + " 没发射成功，因为目标已阵亡!");
#endif
                return dwID;
            }
            if (pData.speedLows != null && pData.speedLows.Length > 1) dwAssignedID = 0;

            FVector3 up = FVector3.up;

            int damage_power = 0;
            uint track_frame_id = 0xffffffff;
            uint track_body_id = 0xffffffff;
            Transform pTrackSlot = null;
            FVector3 trackOffset = FVector3.zero;
            TrackCheck(targetNode, vPosition, pData, pTrackTransform, ref pTrackSlot, ref damage_power, ref track_frame_id, ref track_body_id, ref trackOffset);
            if (pData.type != EProjectileType.TrackPath && pData.speedLows != null && pData.speedLows.Length > 0)
            {
                if (pData.speedLows.Length > 1) dwAssignedID = 0;
                for (int i = 0; i < pData.speedLows.Length; ++i)
                {
                    ProjectileNode pProjectile = GetFramework().gameWorld.CreateNode(EActorType.Projectile, pData, (int)dwAssignedID) as ProjectileNode;
                    pProjectile.Reset();
                    pProjectile.SetData(pData, pOwnerActor, targetNode, vPosition, vDirection, damage_power, track_frame_id, track_body_id);
                    pProjectile.SetSpeed(vPosition, vDirection, i, pOwnerActor.IsFlag(EWorldNodeFlag.Facing2D));
                    pProjectile.SetTrack(pTrackSlot, trackOffset);
                    pProjectile.SetDelayTime((FFloat)(fDelta + pData.launch_delay + 0.0666f));
                    pProjectile.SetStateParam(stateParam);

                    m_mRunningProjectile.Add(pProjectile.GetInstanceID(), pProjectile);
                    if (vResults != null) vResults.Add(pProjectile);

                    pProjectile.SetVisible(true);
                    pProjectile.SetActived(true);
                    pProjectile.EnableLogic(true);
                    pProjectile.SetSpatial(true);
                    pProjectile.SetCollectAble(false);

                    //! call back
                    OnLaunchProjectileCallback(pProjectile);
                }
            }
            else if (pData.life_time > 0)
            {
                if (pData.type == EProjectileType.TrackPath)
                {
                    if (!pData.IsValidTrackPath())
                    {
                        return dwID;
                    }
                }

                ProjectileNode pProjectile = GetFramework().gameWorld.CreateNode(EActorType.Projectile, pData) as ProjectileNode;
                pProjectile.Reset();

                pProjectile.SetData(pData, pOwnerActor, targetNode, vPosition, vDirection, damage_power, track_frame_id, track_body_id);
                pProjectile.SetStateParam(stateParam);
                pProjectile.SetDelayTime((FFloat)(fDelta + pData.launch_delay + 0.0666f));
                pProjectile.SetTrack(pTrackSlot, trackOffset);

                if (pData.type == EProjectileType.TrackPath)
                {
                    pProjectile.BuildTrackPathKeyframe(vPosition, pProjectile.GetTargetPosition());
                }

                m_mRunningProjectile.Add(pProjectile.GetInstanceID(), pProjectile);
                if (vResults != null) vResults.Add(pProjectile);

                pProjectile.SetVisible(true);
                pProjectile.SetActived(true);
                pProjectile.EnableLogic(true);
                pProjectile.SetSpatial(true);
                pProjectile.SetCollectAble(false);

                //! call back
                OnLaunchProjectileCallback(pProjectile);
            }
            return dwID;
        }
        //------------------------------------------------------
        uint DoBoundLaunchProjectile(BoundProjectile bound)
        {
            uint dwID = 0xffffffff;
            var csvData = bound.projectile.GetProjectileData();
            if (csvData == null)
                return dwID;
            if (bound.pTarget == null) return dwID;
            if (bound.pTarget.IsFlag(EWorldNodeFlag.Killed)) return dwID;
            float dist = (bound.pTarget.GetPosition() - bound.projectile.GetPosition()).sqrMagnitude;
            if (dist <= 1) return dwID;
            FVector3 up = FVector3.up;

            int damage_power = 0;
            uint track_frame_id = 0xffffffff;
            uint track_body_id = 0xffffffff;
            Transform pTrackSlot = null;
            FVector3 trackOffset = FVector3.zero;
            FVector3 vPosition = bound.projectile.GetPosition();
            FVector3 vDirection = (bound.pTarget.GetPosition() - bound.projectile.GetPosition()).normalized;
            TrackCheck(bound.pTarget, vPosition, bound.projectile.GetProjectileData(), null, ref pTrackSlot, ref damage_power, ref track_frame_id, ref track_body_id, ref trackOffset);
            if(pTrackSlot!=null) vDirection = (pTrackSlot.position - bound.projectile.GetPosition()).normalized;
            if (csvData.speedLows != null && csvData.speedLows.Length > 0)
            {
                for (int i = 0; i < csvData.speedLows.Length; ++i)
                {
                    ProjectileNode pProjectile = GetFramework().gameWorld.CreateNode(EActorType.Projectile, csvData) as ProjectileNode;
                    pProjectile.Reset();
                    pProjectile.SetData(csvData, bound.projectile.GetOwnerActor(), bound.pTarget, vPosition, vDirection, damage_power, track_frame_id, track_body_id);
                    pProjectile.SetBoundSpeed(vPosition, vDirection, i);
                    pProjectile.SetTrack(pTrackSlot, trackOffset);
                    pProjectile.SetOffsetEulerAngle(bound.projectile.GetOffsetEulerAngle());
                    pProjectile.SetStateParam(bound.projectile.GetStateParam());
                    pProjectile.SetBindOwnerSlot(bound.projectile.GetBindOwnerSlot());
                    pProjectile.SetRemainHitCount(bound.projectile.GetRemainHitCount()+1);
                    if (bound.hitActor != null) pProjectile.RecodeHit(bound.hitActor.GetInstanceID(), 10);
                    pProjectile.CopyBoundeds(bound.projectile);

                    pProjectile.SetBoundStartActor(bound.hitActor);
                    pProjectile.SetBoundProjectile(true);
                    pProjectile.SetRemainBoundCount(bound.projectile.GetRemainBoundCount());
                    pProjectile.SetDamagePower(bound.projectile.GetDamagePower());

                    if (bound.attackGroup !=0xff) pProjectile.SetAttackGroup(bound.attackGroup);
                    if (bound.projectile.IsBoundInvert())
                    {
                        if (bound.bound_count%2==0)
                        {
                            if (csvData.bound_damage_id > 0)
                            {
                                pProjectile.SetDamageId((uint)csvData.bound_damage_id);
                            }
                            else pProjectile.SetDamageId((uint)csvData.damage);
                        }
                        else
                        {
                            pProjectile.SetDamageId((uint)csvData.damage);
                        }
                    }
                    else
                    {
                        if (csvData.bound_damage_id > 0)
                        {
                            pProjectile.SetDamageId((uint)csvData.bound_damage_id);
                        }
                    }
                    if (bound.bound_count>=0 && (csvData.bound_flag & (int)EBoundFlag.BoundDamageAdd) != 0)
                        pProjectile.SetDamageId(pProjectile.GetDamageID() + (uint)bound.bound_count);


                    m_mRunningProjectile.Add(pProjectile.GetInstanceID(), pProjectile);

                    pProjectile.SetVisible(true);
                    pProjectile.SetActived(true);
                    pProjectile.EnableLogic(true);
                    pProjectile.SetSpatial(true);
                    pProjectile.SetCollectAble(false);
                    //! call back
                    OnLaunchProjectileCallback(pProjectile);
                }
            }
            else if (csvData.life_time > 0)
            {
                ProjectileNode pProjectile = GetFramework().gameWorld.CreateNode(EActorType.Projectile, csvData) as ProjectileNode;
                pProjectile.Reset();

                pProjectile.SetData(csvData, bound.projectile.GetOwnerActor(), bound.pTarget, vPosition, vDirection, damage_power, track_frame_id, track_body_id);
                pProjectile.SetStateParam(bound.projectile.GetStateParam());
                pProjectile.SetOffsetEulerAngle(bound.projectile.GetOffsetEulerAngle());
                pProjectile.SetTrack(pTrackSlot, trackOffset);
                pProjectile.SetBindOwnerSlot(bound.projectile.GetBindOwnerSlot());
                pProjectile.SetRemainHitCount(bound.projectile.GetRemainHitCount());
                pProjectile.SetRemainBoundCount(bound.projectile.GetRemainBoundCount());
                pProjectile.SetBoundStartActor(bound.hitActor);
                pProjectile.SetBoundProjectile(true);
                pProjectile.SetDamagePower(bound.projectile.GetDamagePower());
                if (bound.hitActor != null) pProjectile.RecodeHit(bound.hitActor.GetInstanceID(), 10);
                pProjectile.CopyBoundeds(bound.projectile);

                if (bound.attackGroup != 0xff) pProjectile.SetAttackGroup(bound.attackGroup);
                if (bound.projectile.IsBoundInvert())
                {
                    if (bound.bound_count % 2 == 0)
                    {
                        if (csvData.bound_damage_id > 0)
                        {
                            pProjectile.SetDamageId((uint)csvData.bound_damage_id);
                        }
                        else pProjectile.SetDamageId( (uint)csvData.damage);
                        
                    }
                    else
                    {
                        pProjectile.SetDamageId((uint)csvData.damage);
                    }
                }
                else
                {
                    if (csvData.bound_damage_id > 0)
                    {
                        pProjectile.SetDamageId((uint)csvData.bound_damage_id);
                    }
                }
				if (bound.bound_count>=0 && (csvData.bound_flag & (int)EBoundFlag.BoundDamageAdd) != 0)
                	pProjectile.SetDamageId(pProjectile.GetDamageID() + (uint)bound.bound_count);
                m_mRunningProjectile.Add(pProjectile.GetInstanceID(), pProjectile);

                pProjectile.SetVisible(true);
                pProjectile.SetActived(true);
                pProjectile.EnableLogic(true);
                pProjectile.SetSpatial(true);
                pProjectile.SetCollectAble(false);
                //! call back
                OnLaunchProjectileCallback(pProjectile);
            }
            return dwID;
        }
        //------------------------------------------------------
        void OnLaunchProjectileCallback(ProjectileNode pProjectile)
        {
            foreach (var db in m_vProjectileManagerCB)
                db.OnLaunchProjectile(pProjectile);
#if !USE_SERVER
            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null)
                return;

            //! waring test
            if (pProjectile.TestFinalDropPos())
            {
                InstanceOperiaon pOp = FileSystemUtil.SpawnInstance(projectileData.waring_effect, true);
                if (pOp != null)
                {
                    pOp.SetByParent(RootsHandler.ParticlesRoot);
                    pOp.OnCallback = pProjectile.OnSpawnWarningCallback;
                    pOp.OnSign = pProjectile.OnSpawnSign;
                }
                //    pProjectile.pWaringCallback = pOp;
            }

            if (!string.IsNullOrEmpty(projectileData.effect))
            {
                InstanceOperiaon pOp = FileSystemUtil.SpawnInstance(projectileData.effect, true);
                if (pOp != null)
                {
                    pOp.SetByParent(RootsHandler.ParticlesRoot);
                    pOp.OnCallback = pProjectile.OnSpawnCallback;
                    pOp.OnSign = pProjectile.OnSpawnSign;
                }
                //    pProjectile.pCallback = pOp;
            }
            if (projectileData.sound_launch_id > 0)
            {
                AudioManager.PlayID(projectileData.sound_launch_id);
            }
            else if (!string.IsNullOrEmpty(projectileData.sound_launch))
            {
                AudioManager.PlayEffect(projectileData.sound_launch);
            }
#endif
        }
        //------------------------------------------------------
        public uint BoundLaunchProjectile(ProjectileNode projectile, AWorldNode pTarget, AWorldNode pHitActor, int boundCnt, byte boundAttackGroup=0xff, bool bImmdeate = true)
        {
            uint dwID = 0xffffffff;
            if (pTarget == null) return dwID;
            if (projectile.GetRemainBoundCount() <= 0) return dwID;
            float dist = (pTarget.GetPosition() - projectile.GetPosition()).sqrMagnitude;
            if (dist <= 1) return dwID;

       //     projectile.delta = 0.1f;
            projectile.SetRemainLifeTime(0.1f);
            projectile.AddBounded(pTarget);

            BoundProjectile bound = new BoundProjectile();
            bound.projectile = projectile;
            bound.pTarget = pTarget;
            bound.hitActor = pHitActor;
            bound.bound_count = boundCnt;
            bound.attackGroup = boundAttackGroup;
            if (bImmdeate)
            {
                return DoBoundLaunchProjectile(bound);
            }
            else
            {
                m_vPrepareBounds.Add(bound);
            }
            return dwID;
        }
        //------------------------------------------------------
        public ProjectileNode FindProjectile(int nLaunchID)
        {
            ProjectileNode pProjectile = null;
            if (m_mRunningProjectile.TryGetValue(nLaunchID, out pProjectile))
                return pProjectile;
            return null;
        }
        //------------------------------------------------------
        public Dictionary<int, ProjectileNode> GetRunningProjectile()
        {
            return m_mRunningProjectile;
        }
        //------------------------------------------------------
        public void DoProjectileHit(ProjectileNode pProjectile, AWorldNode pAttacker, AWorldNode hitActor, int hitCnt, bool bHitScene, bool bExplode = false)
        {
            pProjectile.SubHitCount(hitCnt);
            pProjectile.ResetHitStepDelta();

            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null)
                return;

            if (projectileData.HitEventID>0)
                m_vTickEventTemps.Add(new Event(EEventType.Hit, pProjectile, hitActor, null, projectileData.HitEventID, pProjectile.GetPosition(), pProjectile.GetStateParam()));
            if(hitActor!=null)
            {
                if (pAttacker != null)
                    pAttacker = pProjectile.GetOwnerActor();
                if(projectileData.AttackEventID>0)
                    m_vTickEventTemps.Add(new Event(EEventType.Attacker, pProjectile, pAttacker, hitActor, projectileData.AttackEventID, pProjectile.GetPosition(), pProjectile.GetStateParam()));
            }

            HitFrameActor attackData = HitFrameActor.DEFAULT;
            attackData.mul_hit_cnt = 1;
            attackData.attack_ptr = pAttacker;
            attackData.target_ptr = hitActor;
            OnProjectileHitCallback(pProjectile, attackData, bHitScene, bExplode);

            if (pProjectile.IsTrackEnd() || bHitScene)
                pProjectile.SetRemainLifeTime(0);
        }
        //------------------------------------------------------
        public void DoProjectileHit(ProjectileNode pProjectile, AWorldNode pAttacker, uint dwSkillLevel, ref HashSet<HitFrameActor> attack_data_array, bool bUseAttackData, bool bHitScene, int hitCount = 1, bool bExplode = false)
        {
            pProjectile.SubHitCount(hitCount);
            pProjectile.ResetHitStepDelta();
            float fAppendOnHitActionRate = 0f;
          //  AttackFrameUtil.CU_PlayOnHitActorActions(pAttacker as Actor, dwSkillLevel, pProjectile.projectile, EVolumeType.Attack, fAppendOnHitActionRate,
          //      pProjectile.position, pProjectile.direction, ref attack_data_array, bUseAttackData);

            if (pAttacker == null)
                pAttacker = pProjectile.GetOwnerActor();

            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null)
                return;

            foreach(var attackArray in attack_data_array )
            {
                if (projectileData.HitEventID>0)
                    m_vTickEventTemps.Add(new Event(EEventType.Hit, pProjectile, attackArray.target_ptr, null, projectileData.HitEventID, attackArray.hit_position, pProjectile.GetStateParam()));

                if (projectileData.AttackEventID > 0)
                    m_vTickEventTemps.Add(new Event(EEventType.Attacker, pProjectile, pAttacker, attackArray.target_ptr, projectileData.AttackEventID, pProjectile.GetPosition(), pProjectile.GetStateParam()));

                //! call back
                OnProjectileHitCallback(pProjectile, attackArray, bHitScene, bExplode);
            }

            if (pProjectile.IsTrackEnd() || bHitScene)
                pProjectile.SetRemainLifeTime(0);
        }
        //------------------------------------------------------
        public void DoProjectileHit(int dwLaunchID, AWorldNode pAttacker, uint dwSkillLevel, ref HashSet<HitFrameActor> attack_data_array, bool bUseAttackData, bool bHitScene)
        {
            ProjectileNode pProjectile;
            if (m_mRunningProjectile.TryGetValue(dwLaunchID, out pProjectile))
                DoProjectileHit(pProjectile, pAttacker, dwSkillLevel, ref attack_data_array, bUseAttackData, bHitScene);
        }
        //------------------------------------------------------
        void OnProjectileHitCallback(ProjectileNode pProjectile, HitFrameActor attackData, bool bHitScene, bool bExplode)
        {
            foreach (var db in m_vProjectileManagerCB)
            {
                db.OnProjectileHit(pProjectile, attackData, bHitScene, bExplode);
            }
            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null)
                return;
            if(pProjectile.IsBoundProjectile())
            {
                if (projectileData.bound_hit_sound_id > 0)
                {
                    AudioManager.PlayID(projectileData.bound_hit_sound_id);
                }
                else if (!string.IsNullOrEmpty(projectileData.bound_hit_sound))
                {
                    AudioManager.PlayEffect(projectileData.bound_hit_sound);
                }
                else if (projectileData.sound_hit_id > 0)
                {
                    AudioManager.PlayID(projectileData.sound_hit_id);
                }
                else if (!string.IsNullOrEmpty(projectileData.sound_hit))
                {
                    AudioManager.PlayEffect(projectileData.sound_hit);
                }
            }
            else
            {
                if (projectileData.sound_hit_id > 0)
                {
                    AudioManager.PlayID(projectileData.sound_hit_id);
                }
                else if (!string.IsNullOrEmpty(projectileData.sound_hit))
                {
                    AudioManager.PlayEffect(projectileData.sound_hit);
                }
            }
        }
        //------------------------------------------------------
        public void DoProjectileCounteract(ProjectileNode pProjectile)
        {
            pProjectile.SubHitCount(1);
        }
        //------------------------------------------------------
        void DoStepEvent(Event pEvt)
        {
            if (pEvt.node_ptr == null || pEvt.eventId<=0)
                return;
            if (m_pFramework == null) return;
            m_pFramework.eventSystem.Begin();
            m_pFramework.eventSystem.ATuserData = pEvt.node_ptr;
            m_pFramework.eventSystem.pParentNode = pEvt.pProjectile.GetOwnerActor();
            m_pFramework.eventSystem.bCalcAxisOffset = true;
            m_pFramework.eventSystem.TriggerEventPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerEventRealPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerActorActionStateParam = pEvt.stateParam;
            m_pFramework.eventSystem.TriggerActorDir = pEvt.node_ptr.GetDirection();
            m_pFramework.eventSystem.OnEventCallback += OnOverEventCallback;
            m_pFramework.OnTriggerEvent(pEvt.eventId, false);
            m_pFramework.eventSystem.End();
        }
        //------------------------------------------------------
        void DoHitEvent(Event pEvt)
        {
            if (pEvt.node_ptr == null || pEvt.eventId <= 0) return;
            if (m_pFramework == null) return;
            m_pFramework.eventSystem.Begin();
            m_pFramework.eventSystem.ATuserData = pEvt.node_ptr;
            m_pFramework.eventSystem.pTargetNode = pEvt.target_ptr;
            m_pFramework.eventSystem.pParentNode = pEvt.pProjectile.GetOwnerActor();
            m_pFramework.eventSystem.bCalcAxisOffset = true;
            m_pFramework.eventSystem.TriggerEventPos = pEvt.runtime_hit;
            m_pFramework.eventSystem.TriggerEventRealPos = pEvt.runtime_hit;
            m_pFramework.eventSystem.TriggerActorActionStateParam = pEvt.stateParam;
            m_pFramework.eventSystem.TriggerActorDir = pEvt.node_ptr.GetDirection();
            m_pFramework.eventSystem.OnEventCallback += OnOverEventCallback;
            m_pFramework.OnTriggerEvent(pEvt.eventId, false);
            m_pFramework.eventSystem.End();
        }
        //------------------------------------------------------
        void DoAttackEvent(Event pEvt)
        {
            if (pEvt.node_ptr == null || pEvt.eventId <= 0) return;
            if (m_pFramework == null) return;
            m_pFramework.eventSystem.Begin();
            m_pFramework.eventSystem.ATuserData = pEvt.node_ptr;
            m_pFramework.eventSystem.pTargetNode = pEvt.target_ptr;
            m_pFramework.eventSystem.pParentNode = pEvt.pProjectile.GetOwnerActor();
            m_pFramework.eventSystem.bCalcAxisOffset = true;
            m_pFramework.eventSystem.TriggerEventPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerEventRealPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerActorActionStateParam = pEvt.stateParam;
            m_pFramework.eventSystem.TriggerActorDir = pEvt.node_ptr.GetDirection();
            m_pFramework.eventSystem.OnEventCallback += OnOverEventCallback;
            m_pFramework.OnTriggerEvent(pEvt.eventId, false);
            m_pFramework.eventSystem.End();
        }
        //------------------------------------------------------
        void DoOverEvent(Event pEvt)
        {
            if (pEvt.node_ptr == null || pEvt.eventId <= 0) return;
            if (m_pFramework == null) return;
            m_pFramework.eventSystem.Begin();
            m_pFramework.eventSystem.ATuserData = pEvt.node_ptr;
            m_pFramework.eventSystem.pTargetNode = pEvt.target_ptr;
            m_pFramework.eventSystem.pParentNode = pEvt.pProjectile.GetOwnerActor();
            m_pFramework.eventSystem.bCalcAxisOffset = true;
            m_pFramework.eventSystem.TriggerEventPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerEventRealPos = pEvt.runtime_pos;
            m_pFramework.eventSystem.TriggerActorActionStateParam = pEvt.stateParam;
            m_pFramework.eventSystem.TriggerActorDir = pEvt.node_ptr.GetDirection();
            m_pFramework.eventSystem.OnEventCallback += OnOverEventCallback;
            m_pFramework.OnTriggerEvent(pEvt.eventId, false);
            m_pFramework.eventSystem.End();
        }
        //------------------------------------------------------
        void OnOverEventCallback(BaseEvent param, ref uint usedFlag)
        {
        }
        //------------------------------------------------------
        public void StopProjectile(ProjectileNode pProjectile, bool bRemoved = true)
        {
            var projectileData = pProjectile.GetProjectileData();
            if (projectileData == null) return;
            if(projectileData.OverEventID>0)
                m_vTickEventTemps.Add(new Event(EEventType.Over, pProjectile, pProjectile.GetOwnerActor(), null, projectileData.OverEventID, pProjectile.GetPosition(), pProjectile.GetStateParam()));
            foreach (var db in m_vProjectileManagerCB)
                db.OnStopProjectile(pProjectile);

            if (bRemoved)
            {
                m_mRunningProjectile.Remove(pProjectile.GetInstanceID());
            }
            pProjectile.Reset();
            pProjectile.SetDestroy();
        }
        //------------------------------------------------------
        public void StopProjectile(int dwLaunchID, bool bRemoved = true)
        {
            ProjectileNode pProjectile;
            if (m_mRunningProjectile.TryGetValue(dwLaunchID, out pProjectile))
                StopProjectile(pProjectile, bRemoved);
        }
        //------------------------------------------------------
        public void StopAllProjectiles()
        {
            m_vTickEventTemps.Clear();
            foreach (var db in m_mRunningProjectile)
            {
                foreach (var cb in m_vProjectileManagerCB)
                    cb.OnStopProjectile(db.Value);

                db.Value.Reset();
                db.Value.SetDestroy();
            }
            m_CatchTemp.Clear();
            m_mRunningProjectile.Clear();
        }
        //------------------------------------------------------
        public void DelayStopProjectile(ProjectileNode pProjectile, FFloat fDelayDuration)
        {
            pProjectile.SetDelayStopDelta(fDelayDuration);
            pProjectile.SetDelayStopDuration(fDelayDuration);

            //! call back
            foreach (var db in m_vProjectileManagerCB)
                db.OnDelayStopProjectile(pProjectile);

            var vHoldRoles = pProjectile.GetHoldRoles();
            if (vHoldRoles != null)
            {
                Actor target;
                for (int i = 0; i < vHoldRoles.Count; ++i)
                {
                    target = vHoldRoles[i];
                    target.SetSpeedXZ(Vector3.zero);
                }
            }
        }
        //------------------------------------------------------
        public void RegisterCallBack(ProjectileManagerCB pCallBack)
        {
            m_vProjectileManagerCB.Add(pCallBack);
        }
        //------------------------------------------------------
        public void UnregisterCallBack(ProjectileManagerCB pCallBack)
        {
            m_vProjectileManagerCB.Remove(pCallBack);
        }
    }
}

