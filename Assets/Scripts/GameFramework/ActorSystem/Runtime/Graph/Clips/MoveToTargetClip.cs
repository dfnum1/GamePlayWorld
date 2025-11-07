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
    [System.Serializable, CutsceneClip("Actor/移动到锁定目标", typeof(Core.Actor))]
    public class MoveToTargetClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp       baseProp;
        [Display("面朝目标")] public bool faceTo = true;
        [Display("目标朝向偏移")] public bool           bDirOffset = true;
        [Display("位置偏移")] public Vector3        posOffset; //跟随偏移量
        [Display("角度偏移")] public Vector3        rotOffset; //跟随角度偏移量
        [Display("速度曲线")] public AnimationCurve speedCurve;
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new MoveToTargetDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EActorCutsceneClipType.eMoveToLockTarget;
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
        //-----------------------------------------------------
#if UNITY_EDITOR
        [System.NonSerialized] Actor m_pSelfTarget = null;
        [System.NonSerialized] AWorldNode m_pLockTarget = null;
        [AddInspector]
        public void OnEditor()
        {
            UnityEditor.EditorGUILayout.LabelField("---------------------------------------------------------------------");
            System.Collections.Generic.List<AWorldNode> lockTargets = null;
            if (baseProp.ownerTrackObject != null)
            {
                var binder = baseProp.ownerTrackObject.GetCutscene().GetBindData() as Actor;
                if(binder!=null)
                {
                    m_pSelfTarget = binder;
                    lockTargets = binder.GetSkillSystem().GetLockTargets();
                    if (lockTargets != null && lockTargets.Count > 0)
                        m_pLockTarget = lockTargets[0];
                }
            }
            if(m_pSelfTarget == null)
            {
                UnityEditor.EditorGUILayout.HelpBox("当前没有绑定对象", UnityEditor.MessageType.Warning);
                return;
            }
            UnityEditor.EditorGUILayout.ObjectField("当前绑定对象", m_pSelfTarget.GetObjectAble(), typeof(AInstanceAble), true);

            if (m_pLockTarget!=null)
            {
                UnityEditor.EditorGUILayout.ObjectField("锁定目标", m_pLockTarget.GetObjectAble(), typeof(AInstanceAble), true);
            }
            if(m_pLockTarget == null)
            {
                UnityEditor.EditorGUILayout.HelpBox("请设置锁定的编辑目标", UnityEditor.MessageType.Warning);
                return;
            }
            if (GUILayout.Button("设置偏移参数"))
            {
                if (m_pLockTarget != null && m_pSelfTarget != null)
                {
                    posOffset = m_pSelfTarget.GetPosition() - m_pLockTarget.GetPosition();
                    rotOffset = (Quaternion.Inverse(Quaternion.Euler(m_pLockTarget.GetEulerAngle())) * Quaternion.Euler(m_pSelfTarget.GetEulerAngle())).eulerAngles;
                }
            }
        }
#endif
    }
    //-----------------------------------------------------
    //MoveToTargetDriver
    //-----------------------------------------------------
    public class MoveToTargetDriver : ACutsceneDriver
    {
        FVector3 m_ActorPosition = FVector3.zero;
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
                m_ActorPosition = m_pSelf.GetPosition();
            }
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            CheckSelf(pTrack);
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            if(m_pSelf!=null)
            {
                if(clip.CanRestore())
                    m_pSelf.SetPosition(m_ActorPosition);
                m_pSelf = null;
            }
            return true;
        }
        //-----------------------------------------------------
        public override bool OnFrameClip(CutsceneTrack pTrack, FrameData frameData)
        {
            CheckSelf(pTrack);
            if (m_pSelf == null) return true;

            MoveToTargetClip moveTo = frameData.clip.Cast<MoveToTargetClip>();

            float normalTime = frameData.subTime / moveTo.GetDuration();
            if (BaseUtil.IsValidCurve(moveTo.speedCurve))
                normalTime = moveTo.speedCurve.Evaluate(normalTime);

            var lockTargets = m_pSelf.GetSkillSystem().GetLockTargets();
            if (lockTargets == null || lockTargets.Count<=0) return true;
            FVector3 targetPos = lockTargets[0].GetPosition();
            if (moveTo.bDirOffset)
            {
                targetPos += lockTargets[0].GetDirection()* moveTo.posOffset.z;
                targetPos += lockTargets[0].GetUp() * moveTo.posOffset.y;
                targetPos += lockTargets[0].GetRight() * moveTo.posOffset.x;
            }
            m_pSelf.SetFinalPosition(FVector3.Lerp(m_ActorPosition, targetPos, normalTime));
            if (moveTo.faceTo)
            {
                Vector3 diff = targetPos - m_ActorPosition;
                diff.y = 0;
                m_pSelf.SetDirection(diff);
            }
            return true;
        }
    }
}
#endif