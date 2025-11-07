/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Core;
using Framework.RtgTween;
using UnityEngine;

namespace Framework.UI
{
    public enum ELogicController : byte
    {
        Widget = 0,
        UICamera,
        GameCamera,
    }
    //------------------------------------------------------
    public abstract class UIBaseParameter
    {
        public ELogicController eControllerType = ELogicController.Widget;
        public bool isFullPath = false;
        public bool bFirstParent = true;
        public string strControllerName = "";
        public int controllerTag = 0;
#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bDeling = false;
        [System.NonSerialized]
        public bool bExpand = false;
        [System.NonSerialized]
        public bool expandPropertys = false;
        [System.NonSerialized]
        public EditorData controllRect = new EditorData();

        [System.NonSerialized]
        public Backuper m_Backuper = new Backuper();
        [System.NonSerialized]
        UnityEngine.Object m_pEditController = null;
        public UnityEngine.Object pEditController
        {
            get { return m_pEditController; }
            set
            {
                if (m_pEditController != value)
                {
                    m_Backuper.SetController(value);
                    m_pEditController = value;
                    if (m_pEditController)
                    {
                        TransformRef tranRef = null;
                        Transform transform = pEditController as Transform;
                        if (transform)
                        {
                            tranRef = transform.GetComponent<TransformRef>();
                        }
                        controllerTag = 0;
                        strControllerName = m_pEditController.name;
                        if (tranRef != null)
                        {
                            strControllerName = tranRef.strSymbole != null ? tranRef.strSymbole : tranRef.name;
                            controllerTag = tranRef.GUID;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            if (pEditController == null) return null;
            Transform transform = pEditController as Transform;
            if (transform) return transform;
            GameObject go = pEditController as GameObject;
            if (go) return go.transform;

            Behaviour behavior = pEditController as Behaviour;
            if (behavior) return behavior.transform;
            return null;
        }
        //------------------------------------------------------
        public RectTransform GetRectTransform()
        {
            return pEditController as RectTransform;
        }
        //------------------------------------------------------
        public UnityEngine.UI.Graphic GetUIGraphic()
        {
            UnityEngine.UI.Graphic ui = pEditController as UnityEngine.UI.Graphic;
            if (ui == null)
            {
                Transform trans = pEditController as Transform;
                if (trans != null) ui = trans.GetComponent<UnityEngine.UI.Graphic>();
            }
            return ui;
        }
        //------------------------------------------------------
        public UnityEngine.CanvasGroup GetCanvasGroup()
        {
            UnityEngine.CanvasGroup ui = pEditController as UnityEngine.CanvasGroup;
            if (ui == null)
            {
                Transform trans = pEditController as Transform;
                if (trans != null) ui = trans.GetComponent<UnityEngine.CanvasGroup>();
            }
            return ui;
        }
        //------------------------------------------------------
        public Camera GetCamera()
        {
            if (pEditController == null) return null;
            Camera transform = pEditController as Camera;
            if (transform) return transform;
            return null;
        }

        //------------------------------------------------------
        public abstract void MoveGapTime(float fTime);
#endif
        //------------------------------------------------------
        public abstract float GetTrackDuration();
        public abstract float GetTrackFirst();
        public abstract float GetTimelineLength();
    }
    //------------------------------------------------------
    [System.Serializable]
    public class UIAnimatorParameter:UIBaseParameter
    {
        public UIAnimatorElementType type;
        public RtgTweenerProperty from;
        public RtgTweenerProperty to;

        public float time;
        public EEaseType transition;
        public EQuationType equation;
        public bool pingpong;

        public float delay;
        public int loop;
        public int delay_times;

        public bool bLocal;
        public bool bOffset = true;
        public bool bRecover;

        public float finalValue;
        public float initialValue;
        public AnimationCurve lerpCurve;

        public UIAnimatorEvent[] EventList = null;

        //------------------------------------------------------
        public override float GetTimelineLength()
        {
            return time + delay;
        }
        //------------------------------------------------------
        public override float GetTrackDuration()
        {
            return time;
        }
        //------------------------------------------------------
        public override float GetTrackFirst()
        {
            return delay;
        }
#if UNITY_EDITOR
        public override void MoveGapTime(float fTime)
        {
            delay += fTime;
            if (delay < 0) delay = 0;
        }
#endif
    }

    [System.Serializable]
    public class UITargetBindTrackParameter : UIBaseParameter
    {
        [System.Serializable]
        public struct SplineData : IQuickSort<SplineData>
        {
            public float time;
            public List<float> propertys;
#if UNITY_EDITOR
            [System.NonSerialized]
            public float editorTime;
#endif
            //-----------------------------------------------------
            public int CompareTo(int type, SplineData other)
            {
                if (time < other.time) return -1;
                if (time > other.time) return 1;
                return 0;
            }
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }

        public int dataDeclare  = Core.DeclareKit.InTag_OutTag_Pos_Euler;
        public List<SplineData> datas = null;

        //------------------------------------------------------
        public bool HasProperty(Core.eDeclareType type)
        {
            return Core.DeclareKit.HasDeclare(type, dataDeclare);
        }
        //------------------------------------------------------
        public override float GetTrackDuration()
        {
            if (datas == null || datas.Count<=0) return 0;
#if UNITY_EDITOR
            float fTime = 0;
            for(int i = 0; i < datas.Count; ++i)
            {
                fTime = Mathf.Max(datas[i].time, fTime);
            }
            return fTime - GetTrackFirst();
#else
            return datas[datas.Count - 1].time - datas[0].time;
#endif
        }
        //------------------------------------------------------
        public override float GetTrackFirst()
        {
            if (datas == null || datas.Count <= 0) return 0;
#if UNITY_EDITOR
            float fTime = datas[0].time;
            for (int i = 0; i < datas.Count; ++i)
            {
                fTime = Mathf.Min(datas[i].time, fTime);
            }
            return fTime;
#else
            return datas[0].time;
#endif
        }
        //------------------------------------------------------
        public override float GetTimelineLength()
        {
            return GetTrackFirst() + GetTrackDuration();
        }
#if UNITY_EDITOR
        public override void MoveGapTime(float fTime)
        {
            if (datas == null || datas.Count <= 0) return;
            for (int i = 0; i < datas.Count; ++i)
            {
                SplineData data = datas[i];
                data.time += fTime;
                data.editorTime = data.time;
                datas[i] = data;
            }
        }
#endif
        //------------------------------------------------------
        public void SetFloat(Core.eDeclareType type, int index, float val)
        {
            if (datas == null || index < 0 || index >= datas.Count || datas[index].propertys == null) return;
            Core.DeclareKit.SetFloat(type, dataDeclare, datas[index].propertys, val);
        }
        //------------------------------------------------------
        public bool GetFloat(Core.eDeclareType type, int index, ref float val)
        {
            if (datas == null || index < 0 || index >= datas.Count || datas[index].propertys == null) return false;
            return Core.DeclareKit.GetFloat(type, dataDeclare, datas[index].propertys, ref val);
        }
        //------------------------------------------------------
        public void SetVector3(Core.eDeclareType type, int index, Vector3 vec3)
        {
            if (datas == null || index < 0 || index >= datas.Count || datas[index].propertys == null) return;
            Core.DeclareKit.SetVector3(type, dataDeclare, datas[index].propertys, vec3);
        }
        //------------------------------------------------------
        public bool GetVector3(Core.eDeclareType type, int index, ref Vector3 vec3)
        {
            if (datas == null || index < 0 || index >= datas.Count || datas[index].propertys == null) return false;
            return Core.DeclareKit.GetVector3(type, dataDeclare, datas[index].propertys, ref vec3);
        }
    }

    [System.Serializable]
    public class UIAnimatorGroupData
    {
        public int nID=0;
        public string desc = "";
        public float fSpeed = 1;

        public UIAnimatorParameter[] Parameters;
        public UITargetBindTrackParameter[] BindTracks;

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bExpand = false;
        //------------------------------------------------------
        public void Recovert()
        {
            if (Parameters == null) return;
            for (int i = 0; i < Parameters.Length; ++i)
            {
                Parameters[i].m_Backuper.Recovert();
            }
        }
        //------------------------------------------------------
        public float GetTimelineLength()
        {
            float len = 0;
            if(Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; ++i)
                {
                    len = Mathf.Max(len, Parameters[i].GetTimelineLength());
                }
            }
            if (BindTracks != null)
            {
                for (int i = 0; i < BindTracks.Length; ++i)
                {
                    len = Mathf.Max(len, BindTracks[i].GetTimelineLength());
                }
            }
            return len;
        }
#endif
        //------------------------------------------------------
        public float GetTrackLength()
        {
            float len = 0;
            if(Parameters!=null)
            {
                for (int i = 0; i < Parameters.Length; ++i)
                {
                    len = Mathf.Max(len, Parameters[i].GetTrackDuration() + Parameters[i].delay);
                }
            }
            if (BindTracks != null)
            {
                for (int i = 0; i < BindTracks.Length; ++i)
                {
                    len = Mathf.Max(len, BindTracks[i].GetTrackDuration() + BindTracks[i].GetTrackFirst());
                }
            }
            return len;
        }
    }
    //------------------------------------------------------
    public class UIAnimatorData
    {
        public UIAnimatorGroupData[] datas;
    }
}

