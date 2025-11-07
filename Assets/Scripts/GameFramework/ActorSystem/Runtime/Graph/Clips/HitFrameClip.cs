#if USE_CUTSCENE
/********************************************************************
生成日期:	06:30:2025
类    名: 	MoveToClip
作    者:	HappLI
描    述:	跟随剪辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.Cutscene.Editor;
using Framework.ED;
#endif
using UnityEngine;
using Framework.Base;
using Framework.Core;

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif

namespace Framework.Cutscene.Runtime
{
    [System.Serializable, CutsceneClip("Actor/判定框", typeof(Core.Actor))]
    public class HitFrameClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp       baseProp;
        public AttackFrameParameter frameParams;
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new HitFrameDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EActorCutsceneClipType.eHitFrame;
        }
        //-----------------------------------------------------
        public float GetDuration()
        {
            return baseProp.duration;
        }
        //-----------------------------------------------------
        public EClipEdgeType GetEndEdgeType()
        {
            return baseProp.endEdgeType;
        }
        //-----------------------------------------------------
        public string GetName()
        {
            return baseProp.name;
        }
        //-----------------------------------------------------
        public ushort GetRepeatCount()
        {
            return baseProp.repeatCnt;
        }
        //-----------------------------------------------------
        public float GetTime()
        {
            return baseProp.time;
        }
        //-----------------------------------------------------
        public float GetBlend(bool bIn)
        {
            return baseProp.GetBlend(bIn);
        }
    }
    //-----------------------------------------------------
    //HitFrameDriver
    //-----------------------------------------------------
    public class HitFrameDriver : ACutsceneDriver
    {
        Actor m_pSelf = null;
        //-----------------------------------------------------
        public override void OnDestroy()
        {
            m_pSelf = null;
        }
        //-----------------------------------------------------
        void CheckSelf(CutsceneTrack pTrack)
        {
            if (m_pSelf == null)
            {
                var pBinder = pTrack.GetBindLastCutsceneObject();
                if (pBinder == null)
                    return;

                Actor pActor = pBinder as Actor;
                if (pActor == null) return;
                m_pSelf = pActor;
            }
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            CheckSelf(pTrack);
            var lockTargets = m_pSelf.GetSkillSystem().GetLockTargets();
            if (lockTargets == null || lockTargets.Count <= 0) return true;
            HitFrameClip hitParam = clip.clip.Cast<HitFrameClip>();
            for (int i = 0; i < lockTargets.Count; ++i)
            {
                var target = lockTargets[i];
                //  AttackFrameUtil.CU_PlayOnHitActorActions(m_pSelf, 1, hitParam, EVolumeType.Attack, 1, 1, m_pSelf.GetPosition(), m_pSelf.GetDirection());
                HitFrameActor hitFrame = new HitFrameActor(0, m_pSelf, target, hitParam.frameParams, target.GetPosition(), m_pSelf.GetDirection(),0, m_pSelf.GetStateParam(), target.GetStateParam());
                GetFramework().OnHitFrameDamage(hitFrame);
                FVector3 speed = FVector3.zero;
                speed += m_pSelf.GetDirection() * hitParam.frameParams.hit_back_speed.z;
                speed += m_pSelf.GetUp() * hitParam.frameParams.hit_back_speed.y;
                speed += m_pSelf.GetRight() * hitParam.frameParams.hit_back_speed.x;
                target.SetSpeed(speed);
                //! play hit effect
                if (hitFrame.frameParameter.sound_hit_id > 0)
                {
                    AudioManager.PlayID(hitFrame.frameParameter.sound_hit_id);
                }
                else if (!string.IsNullOrEmpty(hitFrame.frameParameter.sound_hit))
                {
                    AudioManager.PlayEffect(hitFrame.frameParameter.sound_hit);
                }
                if (!string.IsNullOrEmpty(hitFrame.frameParameter.target_effect_hit))
                {
                    FVector3 offset = hitFrame.hit_position - hitFrame.target_ptr.GetPosition() + hitFrame.frameParameter.target_effect_hit_offset;
                    target.AddPart(hitFrame.frameParameter.target_effect_hit, offset, Vector3.zero, 1, hitFrame.frameParameter.effect_hit_slot);
                }
                Actor pTargetActor = target as Actor;
                if (hitFrame.frameParameter.target_action_hit!=0)
                {
                    if (target is Actor)
                    {
                        if (pTargetActor != null)
                        {
                            pTargetActor.StartActionState(hitFrame.frameParameter.target_action_hit, false);
                        }
                    }
                }
                if (pTargetActor != null)
                {
                    pTargetActor.SetHitDuration(Mathf.Max(hitFrame.frameParameter.target_duration_hit, hitParam.GetDuration()));
                    if (hitFrame.frameParameter.hit_back_fraction>=0) pTargetActor.SetFarction(hitFrame.frameParameter.hit_back_fraction);
                    if(hitFrame.frameParameter.hit_back_gravity >=0) pTargetActor.SetGravity(hitFrame.frameParameter.hit_back_gravity);
                }
            }
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            m_pSelf = null;
            return true;
        }
    }
}
#endif