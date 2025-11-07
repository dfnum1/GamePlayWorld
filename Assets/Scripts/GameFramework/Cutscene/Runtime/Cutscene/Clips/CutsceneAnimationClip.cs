#if USE_CUTSCENE
/********************************************************************
生成日期:	06:30:2025
类    名: 	CutsceneAnimationClip
作    者:	HappLI
描    述:	CutsceneAnimationClip
*********************************************************************/
using Framework.Base;
using Framework.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.Cutscene.Runtime
{
    [System.Serializable, CutsceneClip("AnimationClip")]
    public class CutsceneAnimationClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp           baseProp;
        [Display("动作剪辑"),StringViewGUI(typeof(AnimationClip))] public string action;
        [Display("Layer")] public int layer;
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new AnimationDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eAnimation;
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
    //相机移动驱动逻辑
    //-----------------------------------------------------
    public class AnimationDriver : ACutsceneDriver
    {
        System.Collections.Generic.List<ICutsceneObject> m_vObjects;
        string m_strLoadName = null;
        Asset m_pAsset = null;
        //-----------------------------------------------------
        public override void OnDestroy()
        {
            if (m_vObjects != null)
            {
                UnityEngine.Pool.ListPool<ICutsceneObject>.Release(m_vObjects);
                m_vObjects = null;
            }
            if (m_pAsset != null) m_pAsset.Release();
            m_pAsset = null;
        }
        //-----------------------------------------------------
        void CheckLoad(CutsceneAnimationClip clipData)
        {
            if (m_pAsset == null || clipData.action.CompareTo(m_strLoadName) != 0)
            {
                if (m_pAsset != null) m_pAsset.Release();
                m_pAsset = null;
                LoadAsset(clipData.action, false);
                m_strLoadName = clipData.action;
            }
        }
        //-----------------------------------------------------
        public override void OnLoadAsset(Asset pAsset)
        {
            m_pAsset = pAsset;
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            m_vObjects = pTrack.GetBindAllCutsceneObject(m_vObjects);
            CheckLoad(clip.clip.Cast<CutsceneAnimationClip>());
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            if(clip.CanRestore())
            {
                if (m_vObjects != null)
                {
                    AnimationClip animationClip = null;
                    if (m_pAsset!=null)
                    {
                        animationClip = m_pAsset.GetOrigin<AnimationClip>();
                    }
                    var clipData = clip.clip.Cast<CutsceneAnimationClip>();
                    foreach (var db in m_vObjects)
                    {
                        if (db.StopAction(this, animationClip, clipData.layer))
                        {
                            continue;
                        }
                        db.SetParamHold(false);
                    }

                }
            }
            if(m_vObjects!=null)
            {
                UnityEngine.Pool.ListPool<ICutsceneObject>.Release(m_vObjects);
                m_vObjects = null;
            }

            return true;
        }
        //-----------------------------------------------------
        public override bool OnFrameClip(CutsceneTrack pTrack, FrameData frameData)
        {
            var clipData = frameData.clip.Cast<CutsceneAnimationClip>();
#if UNITY_EDITOR
            CheckLoad(clipData);
#endif
            if (m_pAsset == null)
                return true;
            var animationClip = m_pAsset.GetOrigin<AnimationClip>();
            if (animationClip == null)
                return true;
            if (m_vObjects != null && m_vObjects.Count>0)
            {
                foreach (var db in m_vObjects)
                {
                    db.SetParamHold(true);
                    if (db.PlayAction(this, animationClip, clipData.layer, frameData.subTime))
                    {
                        continue;
                    }
                    var obj = db.GetUniyObject();
                    if (obj == null) continue;
                    if (!(obj is GameObject)) continue;
                    GameObject pGo = obj as GameObject;
                    animationClip.SampleAnimation(pGo, frameData.subTime);
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