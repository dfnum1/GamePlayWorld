/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	HitFrameActor
作    者:	HappLI
描    述:	
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;
using Framework.Base;
#if USE_SERVER
using System.Threading;
#endif

namespace Framework.Core
{
    public enum EHitType
    {
        [PluginDisplay("未知")]
        Unknown = 0,
        [PluginDisplay("爆炸")]
        Explode = 1,
        [PluginDisplay("弹射")]
        Bound = 2,
        [PluginDisplay("贯穿")]
        MutiHit = 3,
    }
    //------------------------------------------------------
    public enum EVolumeType : byte
    {
        Target = 0,
        Attack = 1,
        AttackInvert = 2,
        PartTarget = 3,
        Count,
    }
    //------------------------------------------------------
    public enum EVolumeFlag : int
    {
        Target = 1 << (int)EVolumeType.Target,
        Attack = 1 << (int)EVolumeType.Attack,
        AttackInvert = 1 << (int)EVolumeType.AttackInvert,
    }
    //------------------------------------------------------
    public struct HitFrameActor
    {
        public AWorldNode attack_ptr;
        public AWorldNode target_ptr;
        public AttackFrameParameter frameParameter;
        public StateParam attack_state_param;
        public StateParam target_state_param;
   //     public AFrameClip attack_frame;
   //      public AFrameClip target_frame;
        public Vector3 hit_position;
        public Vector3 hit_direction;
        public uint damage_id;
        public ushort damage_level;
        public IUserData skill_data;
        public byte projectileClassify;
        public int damage_power;
        public uint hit_body_part;
        public EHitType hitType;
        public int mul_hit_cnt;
        public IUserData hitType_take_data;
        public int attacker_target_count;
        public HitFrameActor(uint damage_id, AWorldNode attacker, AWorldNode targeter, AttackFrameParameter frameParameter, Vector3 hit_position, Vector3 hit_direction, byte projectileClassify = 0, 
            StateParam attack_state_param = null, StateParam target_state_param = null/*,
            AFrameClip attack_frame = null, AFrameClip target_frame = null*/)
        {
            this.damage_id = damage_id;
            this.projectileClassify = 0;
            this.damage_level = 1;
            this.hit_position = hit_position;
            this.hit_direction = hit_direction;
            this.attack_state_param = attack_state_param;
            this.target_state_param = target_state_param;
            this.damage_power = 1;
            this.attack_ptr = attacker;
            this.target_ptr = targeter;
         //   this.attack_frame = attack_frame;
         //   this.target_frame = target_frame;
            this.skill_data = null;
            this.hitType = EHitType.Unknown;
            this.hitType_take_data = null;
            this.attacker_target_count = 0;
            this.hit_body_part = 0;
            this.mul_hit_cnt = 1;
            this.frameParameter = frameParameter;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (attack_ptr != null ? attack_ptr.GetInstanceID() : 0);
            hash = hash * 31 + (target_ptr != null ? target_ptr.GetInstanceID() : 0);
            hash = hash * 31 + damage_id.GetHashCode();
            return hash;
        }
        public bool Equals(HitFrameActor other)
        {
            return attack_ptr == other.attack_ptr && target_ptr == other.target_ptr && damage_id == other.damage_id;
        }

        public static HitFrameActor DEFAULT = new HitFrameActor(0, null, null, null, Vector3.zero, Vector3.zero);
    }
#if USE_ACTORSYSTEM
    public class AttackFrameUtil
    {
        //------------------------------------------------------
        public static void CU_ProjectileAttackFrameInsector(ProjectileNode pProjectile, Actor pTarget, ActorAction targetAction, EVolumeType hitVolumeType, ref HashSet<HitFrameActor> vHitFrames, ref bool bPenetrableHit )
        {
            FVector3 vMin1, vMax1;
            FMatrix4x4 targetWorld = pTarget.GetMatrix();
            vMin1 = pTarget.GetBounds().GetCenter();
            vMax1 = pTarget.GetBounds().GetHalf();
            if (pProjectile.IsIntersecition(targetWorld, vMin1, vMax1))
            {
                HitFrameActor hit = new HitFrameActor(pProjectile.GetDamageID(), pProjectile, pTarget, pProjectile.GetProjectileData(), pProjectile.GetFrameHitPosition(pTarget), pProjectile.GetDirection(),
                    pProjectile.GetClassify(), pProjectile.GetStateParam());
                hit.damage_power += pProjectile.GetDamagePower();
                hit.hitType = pProjectile.IsBoundProjectile() ? EHitType.Bound : EHitType.Unknown;
                if (hit.hitType == EHitType.Bound)
                    hit.hitType_take_data = new Variable1() { intVal = pProjectile.GetCfgBoundCount() - pProjectile.GetRemainBoundCount() };
                else if (pProjectile.IsPenetrable())
                {
                    hit.hitType = EHitType.MutiHit;
                    hit.hitType_take_data = new Variable1() { intVal = pProjectile.GetCfgHitCount() - pProjectile.GetRemainHitCount() };
                }
                hit.attacker_target_count = pProjectile.GetTargetHitCount() + vHitFrames.Count;
                vHitFrames.Add(hit);
                pProjectile.RecodeHit(pTarget.GetInstanceID(), 0.1f);
            }
        }
        //------------------------------------------------------
        public static void CU_PlayOnHitActorActions(Actor pAttacker, uint dwSkillLevel, AttackFrameParameter pAttackFrame, EVolumeType eAttackVolume,
            FFloat fBaseAttackOnHitActionRate, FFloat fAppendAttackOnHitActionRate,
        FVector3 vAttackPosition, FVector3 vAttackDirection, 
        ref List<HitFrameActor> attack_data_array, bool bUseAttackData)
        {
//            ActionState pCurrentActionState = pAttacker.GetCurrentActionState();
//            if (pAttackFrame == null || pCurrentActionState == null)
//            {
//                return;
//            }
//            List<ActionFrame> catchFrames = pAttacker.GetGameModule().shareParams.catchActionFrames;

//            for (int i = 0; i < attack_data_array.Count; i++)
//            {
//                ActionState pAction = null;
//                ActionFrame pTargetFrame = null;
//                ActorAttackData attack_data = attack_data_array[i];
//                Actor pActor = attack_data.target_ptr as Actor;
//                if (pActor == null)
//                    continue;

//                //! get target action
//                pAction = pActor.GetCurrentActionState();
//                //! get target frame
//                if (pAction != null)
//                {
//                    if (eAttackVolume == EVolumeType.Attack || eAttackVolume == EVolumeType.AttackInvert)
//                    {
//                        pTargetFrame = pAction.GetFirstFrameByType(EVolumeType.Target);
//                    }

//                    if (pTargetFrame == null)
//                    {
//                        pAction.GetCurrentFrameArray(catchFrames);
//                        if (catchFrames.Count > 0)
//                            pTargetFrame = catchFrames[0];
//                        catchFrames.Clear();
//                    }
//                }

//                bool bOnHitAction = true;


////                 if (CU_IsHit(pAttacker, pActor))
////                 {
////                     bOnHitAction = false;
////                 }
////                 else
//                {
//                    if (fBaseAttackOnHitActionRate < 1)
//                        bOnHitAction = pActor.GetGameModule().CheckerRandom(Mathf.Max(0, fBaseAttackOnHitActionRate));
//                    else bOnHitAction = true;
//                }
//                if (pAttackFrame.damage != 0)
//                {
//                    attack_data.damage_id = pAttackFrame.damage;
//                    if (pAttacker != null) attack_data.damage_level = pAttacker.GetActorParameter().GetLevel();
//                }

//                attack_data.target_id = pActor.GetInstanceID();
//                if (attack_data.hit_position.sqrMagnitude <= 0)
//                    attack_data.hit_position = pActor.GetPosition();
//                attack_data.ground_type = pActor.GetGroundType();

//                if (pAction == null)
//                {
//                    attack_data_array[i] = attack_data;
//                    continue;
//                }
//                attack_data.state_param = pAction.GetStateParam();
//                attack_data.action_speed = pAction.GetSpeed();
//                attack_data.state_delta = pAction.GetDelta();
//                attack_data.last_state_delta = pAction.GetLastDelta();
//                attack_data.stuck_limit = pAction.GetStuckLimit();
//                if (attack_data.state_param != null && attack_data.state_param is Skill)
//                    attack_data.damage_level = (attack_data.state_param as Skill).skillLevel;

//                if (pTargetFrame == null)
//                {
//                    attack_data_array[i] = attack_data;
//                    continue;
//                }

//                //                 if (pTargetFrame.IsPartTargetFrame())
//                //                     attack_data.part_damage_rate = pActor.GetActorParameter().GetPartDamageRate(pTargetFrame.id);
//                //                 else attack_data.part_damage_rate = 1;
//                //                 attack_data.frame_id = pTargetFrame.id;

//                bool bIsBackHit = false;
//                bool bGroundHit = true;
//                int dwOnHitPropertyID = 0;
//                uint nHitStateTypeAndTag = 0;
//                FFloat fDuration = FFloat.zero;
//                byte target_hit_flag = pAttackFrame.target_hit_flag;
//                FVector3 vAttackerToTargetDirection = FVector3.zero;

//                if (bOnHitAction)
//                {
//                    if (pAttackFrame.target_direction_postion)
//                    {
//                        vAttackerToTargetDirection = vAttackPosition - pActor.GetPosition();
//                        vAttackerToTargetDirection.y = 0;
//                        vAttackerToTargetDirection.Normalize();
//                        bIsBackHit = FVector3.Dot(vAttackerToTargetDirection, pActor.GetDirection()) < 0;
//                    }
//                    else
//                    {
//                        bIsBackHit = FVector3.Dot(vAttackDirection, pActor.GetDirection()) > 0;
//                    }
//                }

//                if (pTargetFrame != null && bOnHitAction)
//                {
//                    if ((pActor.GetGroundType() == EActorGroundType.Air) || pTargetFrame.air)
//                    {
//                        if (bIsBackHit)
//                        {
//                            nHitStateTypeAndTag = pAttackFrame.target_action_hit_air_back;
//                            dwOnHitPropertyID = (int)pAttackFrame.target_property_hit_air_back;
//                        }
//                        else
//                        {
//                            nHitStateTypeAndTag = pAttackFrame.target_action_hit_air;
//                            dwOnHitPropertyID = (int)pAttackFrame.target_property_hit_air;
//                        }
//                        bGroundHit = false;
//                    }
//                    else
//                    {
//                        if (bIsBackHit)
//                        {
//                            nHitStateTypeAndTag = pAttackFrame.target_action_hit_ground_back;
//                            dwOnHitPropertyID = (int)pAttackFrame.target_property_hit_ground_back;
//                        }
//                        else
//                        {
//                            nHitStateTypeAndTag = pAttackFrame.target_action_hit_ground;
//                            dwOnHitPropertyID = (int)pAttackFrame.target_property_hit_ground;
//                        }
//                        fDuration = (FFloat)pAttackFrame.target_duration_hit;
//                    }
//                }

//                if (pActor.IsHardBody())
//                    nHitStateTypeAndTag = 0;

//                FFloat fShakeTime = FFloat.zero;
//                if (nHitStateTypeAndTag != 0)
//                {
//                    if (pActor.IsPetRide())
//                    {
//                        //! actor riding pet
//                        //ActionState pState = pActor.GetActionStateGraph().GetSingleActionState(EActionStateType.RideHurt);
//                        //if (pState!=null)
//                        //{
//                        //    pActor.SetGroundType(EActorGroundType.Ground);
//                        //    float fActionSpeed = pState.GetDuration() / fDuration;
//                        //    pActor.StartActionState(pState, 0f, fActionSpeed, 0f, true, false, true);
//                        //    pState.Stuck(pAttackFrame.stuck_time_hit, 0);
//                        //    pActor.GetPetRiding().GetCurrentActionState().Stuck(pAttackFrame.stuck_time_hit, 0);
//                        //    pActor.ResetMomentum();
//                        //    fShakeTime = pAttackFrame.stuck_time_hit;
//                        //}
//                    }
//                    else
//                    {
//                        ActionStateManager.GetActionTypeAndTag(nHitStateTypeAndTag, out var eHitStateType, out var hitTag);
//                        //! play target on_hit action
//                        ActionState pState = pActor.GetActionStateGraph().GetActionState(eHitStateType, hitTag, true, true);
//                        if (pState != null && pState.GetActionStateType() != EActionStateType.Idle)
//                        {
//                            ActionStateProperty pProperty = pCurrentActionState.FindActionStateProperty(dwOnHitPropertyID);
//                            FFloat fVerSpeed = FFloat.zero;
//                            if (pProperty != null && pProperty.propertyData.physic.bUseVerSpeed)
//                                fVerSpeed = pProperty.propertyData.physic.fVerSpeed;

//                            bool bAirHit = pActor.GetGroundType() == EActorGroundType.Air ? true : fVerSpeed > 0f;

//                            if (pState.IsGroundOnHitType() && bAirHit)
//                            {
//                                dwOnHitPropertyID = bIsBackHit ? (int)pAttackFrame.target_property_hit_ground_back : (int)pAttackFrame.target_property_hit_ground - 1;
//                                pProperty = pCurrentActionState.FindActionStateProperty(dwOnHitPropertyID);
//                                bAirHit = false;
//                            }

//                            if (pAttackFrame.target_direction_postion)
//                            {
//                                if (bIsBackHit)
//                                    pActor.SetDirection(-vAttackerToTargetDirection);
//                                else
//                                    pActor.SetDirection(vAttackerToTargetDirection);
//                            }
//                            else if (pAttacker != null)
//                            {
//                                if (bIsBackHit)
//                                    pActor.SetDirection(pAttacker.GetDirection());
//                                else
//                                    pActor.SetDirection(-pAttacker.GetDirection());
//                            }
//                            bool bForcePlay = (target_hit_flag & ((int)ETargetHitFlag.ForceHitAction)) != 0;
//                            FFloat fActionSpeed = FFloat.one;
//                            if (bForcePlay || pActor.CanDoHitAction())
//                            {
//                                pActor.SetGroundType(bAirHit ? EActorGroundType.Air : EActorGroundType.Ground);
//                                if (fDuration > 0) fActionSpeed = (bAirHit) ? FFloat.one : pState.GetDuration() / fDuration;
//                                pActor.StartActionState(pState, 0f, fActionSpeed, bForcePlay, false, true);
//                                pState.SetActionStateProperty(pProperty, pAttacker);
//                                pState.Stuck((FFloat)pAttackFrame.stuck_time_hit, 0);
//                            }
//                            fShakeTime = (FFloat)pAttackFrame.stuck_time_hit;

//                            attack_data.action_id_on_hit = pState.GetCore().id;
//                            attack_data.action_speed_on_hit = fActionSpeed;
//                        }
//                    }
//                    attack_data.hard_frame = false;
//                }
//                else
//                {
//                    //! hard body
//                    pAction = pActor.GetCurrentActionState();
//                    fShakeTime = (FFloat)(pAttackFrame.stuck_time_hit + 0.15f);
//                    attack_data.hard_frame = true;

//                    if (pAction != null && bOnHitAction && bGroundHit && !pActor.IsHardBody())
//                    {
//                        ActionStateProperty pProperty = pCurrentActionState.FindActionStateProperty(dwOnHitPropertyID);
//                        FFloat fVerSpeed = FFloat.zero;
//                        if (pProperty != null)
//                        {
//                            if (pProperty.propertyData.physic.bUseVerSpeed) fVerSpeed = pProperty.propertyData.physic.fVerSpeed;
//                        }

//                        if (pAttackFrame.target_direction_postion)
//                        {
//                            if (bIsBackHit)
//                                pActor.SetDirection(-vAttackerToTargetDirection);
//                            else
//                                pActor.SetDirection(vAttackerToTargetDirection);
//                        }
//                        else if (pAttacker != null)
//                        {
//                            if (bIsBackHit)
//                                pActor.SetDirection(pAttacker.GetDirection());
//                            else
//                                pActor.SetDirection(-pAttacker.GetDirection());
//                        }

//                        pAction.SetActionStateProperty(pProperty, pAttacker);
//                        pAction.Stuck((FFloat)pAttackFrame.stuck_time_hit, 0);
//                    }
//                }
//                attack_data_array[i] = attack_data;
//            }
        }
    }
#endif
}
