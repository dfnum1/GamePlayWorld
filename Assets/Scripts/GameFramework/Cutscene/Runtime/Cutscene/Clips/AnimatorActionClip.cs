#if USE_CUTSCENE

/********************************************************************
生成日期:	06:30:2025
类    名: 	AnimatorActionClip
作    者:	HappLI
描    述:	Animator动作剪辑
*********************************************************************/
using Framework.Base;
using UnityEngine;

namespace Framework.Cutscene.Runtime
{
    [System.Serializable, CutsceneClip("Animator状态动作Clip")]
    public class AnimatorActionClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp           baseProp;
        [Display("动作层")] public int                      layer;
        [Display("动作名"),RowFieldInspector] public string action;
        [Display("动作起始偏移")] public float actionOffset = 0.0f; //动作起始偏移
        [Display("动作速度")] public float playSpeed = 1.0f;                                    
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new AnimatorActionDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eAnimatorAction;
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
#if UNITY_EDITOR
        //-----------------------------------------------------
        public AnimatorActionClip OnDrawFieldLineRow(System.Object pOwner, System.Reflection.FieldInfo fieldInfo)
        {
            if(fieldInfo.Name == "action")
            {
                if(baseProp.ownerTrackObject!=null && GUILayout.Button("选择动作"))
                {
                    var binder = baseProp.ownerTrackObject.GetBindLastCutsceneObject();
                    var provider = ScriptableObject.CreateInstance<Editor.AnimatorActionSearchProvider>();
                    provider.onSelectAction = (selectedAction, length, loop) =>
                    {
                        fieldInfo.SetValue(pOwner, selectedAction);
                        if (!loop)
                        {
                            this.baseProp.duration = length;
                        }
                        //if (state.motion.isLooping)
                        //{
                        //    this.baseProp.repeatCnt = 0; // 0表示无限循环
                        //    this.baseProp.endEdgeType = EClipEdgeType.Repeat;
                        //}
                        //else
                        //{
                        //    this.baseProp.repeatCnt = 1;
                        //    this.baseProp.endEdgeType = EClipEdgeType.None;
                        //}
                    };
                    provider.SetAnimator(binder);

                    // 弹出搜索窗口，位置可根据需要调整
                    UnityEditor.Experimental.GraphView.SearchWindow.Open(new UnityEditor.Experimental.GraphView.SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
                }
            }
            return this;
        }
#endif
    }
    //-----------------------------------------------------
    //相机移动驱动逻辑
    //-----------------------------------------------------
    public class AnimatorActionDriver : ACutsceneDriver
    {
        System.Collections.Generic.List<ICutsceneObject> m_vObjects;
        //-----------------------------------------------------
        public override void OnDestroy()
        {
            if (m_vObjects != null)
            {
                UnityEngine.Pool.ListPool<ICutsceneObject>.Release(m_vObjects);
                m_vObjects = null;
            }
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            m_vObjects = pTrack.GetBindAllCutsceneObject(m_vObjects);
            if(m_vObjects!=null)
            {
                var clipData = clip.clip.Cast<AnimatorActionClip>();
                foreach (var db in m_vObjects)
                {
                    db.SetParamHold(true);
                    if (db.PlayAction(clipData.action, clipData.layer, clipData.actionOffset))
                    {
                        continue;
                    }
                    var animator = db.GetAnimator();
                    if (animator == null) continue;
                    // animator.Play(clipData.action, clipData.layer);
                    animator.speed = clipData.playSpeed; // 设置播放速度
                    animator.CrossFade(clipData.action, Mathf.Max(0, clipData.GetBlend(true)), clipData.layer, clipData.actionOffset);
                }
            }
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            if (m_vObjects != null)
            {
                foreach (var db in m_vObjects)
                    db.SetParamHold(false);

                UnityEngine.Pool.ListPool<ICutsceneObject>.Release(m_vObjects);
                m_vObjects = null;
            }
            return true;
        }
        //-----------------------------------------------------
        public override bool OnFrameClip(CutsceneTrack pTrack, FrameData frameData)
        {
     //       if (!IsEditorMode() && frameData.eStatus == EPlayableStatus.Pause)
     //           return true;

            if (m_vObjects != null && m_vObjects.Count>0)
            {
                var clipData = frameData.clip.Cast<AnimatorActionClip>();
                if (frameData.isBlending)
                {
                    if (clipData.GetBlend(true) < 0)
                    {
                        // blend out,so
                        return true;
                    }
                }
                foreach (var db in m_vObjects)
                {
                    db.SetParamHold(true);
                    if (db.PlayAction(clipData.action, clipData.layer, frameData.subTime+clipData.actionOffset))
                    {
                        continue;
                    }
                    var animator = db.GetAnimator();
                    if (animator == null) continue;
                    if (frameData.eStatus == EPlayableStatus.Pause)
                        animator.speed = 0;
                    else
                        animator.speed = clipData.playSpeed;  // 设置播放速度
                    float blendTime = Mathf.Max(0, clipData.GetBlend(true)); // 融合时间，可根据你的需求调整
                    if (frameData.subTime <= blendTime)
                    {
                        animator.CrossFadeInFixedTime(clipData.action, blendTime, clipData.layer, frameData.subTime + clipData.actionOffset);
                    }
                    else
                        animator.PlayInFixedTime(clipData.action, clipData.layer, frameData.subTime + clipData.actionOffset);
#if UNITY_EDITOR
                    if (frameData.eStatus != EPlayableStatus.Start && IsEditorMode())
                    {
                        animator.speed = 1;
                    }
#endif
                    animator.Update(0);
#if UNITY_EDITOR
                    if (frameData.eStatus != EPlayableStatus.Start && IsEditorMode())
                    {
                        animator.speed = 0;
                    }
#endif
                }
            }
            else
            {
                m_vObjects = pTrack.GetBindAllCutsceneObject(m_vObjects);
            }
            return true;
        }
    }
}
#endif