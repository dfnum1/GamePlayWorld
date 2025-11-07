/********************************************************************
生成日期:	
类    名: 	PositionTween
作    者:	zdq
描    述:	位置平滑过渡
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.UI;
using Framework.Base;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.RtgTween
{
    public class PositionTweenController
    {
        static Dictionary<int, PositionTween> ms_Tweens = new Dictionary<int, PositionTween>();
        public static void OnAdd(PositionTween tween)
        {
            if (tween == null) return;
            ms_Tweens[tween.GetID()] = tween;
        }
        public static void OnRemove(PositionTween tween)
        {
            if (tween == null) return;
            if (ms_Tweens.ContainsKey(tween.GetID()))
            {
                ms_Tweens.Remove(tween.GetID());
            }
        }
        public static PositionTween Find(int id)
        {
            PositionTween tween;
            if (ms_Tweens.TryGetValue(id, out tween))
                return tween;
            return null;
        }
        public static bool IsPlaying(int id)
        {
            PositionTween tween;
            if (ms_Tweens.TryGetValue(id, out tween))
            {
                return tween.isEnable;
            }
            return false;
        }
    }


    public class PositionTween : MonoBehaviour, IUserData
    {
        public static Action<IUserData> OnGlobalTweenEffectCompleted;
        public float Duration = 1f;
        public AnimationCurve SpeedCurve;
        public Vector3 StartPos;
        public Vector3 EndPos;

        public Vector3 StartTan = Vector3.zero;
        public Vector3 EndTan = Vector3.zero;

        public bool isLoop = true;
        public bool isPingPong = false;

        public bool bAutoDestroy = false;

        [Header("是否开启动画播放")]
        public bool isEnable = false;

        public Action OnCompleteAction;

        float m_Timer=0;
        float m_SpeedTimer=0;

        [Header("是否使用本地坐标")]
        public bool isLocalPos = true;

        public bool IfTransformConvertUI = false;

        public int ID;

        [Header("一开始就播放动画")]
        public bool PlayOnAwake = false;

        [SerializeField]
        private Framework.Core.AInstanceAble m_pParentAble = null;
        public Framework.Core.AInstanceAble parentAble
        {
            get { return m_pParentAble; }
            set { m_pParentAble = value; }
        }

        private Transform m_pToTarget = null;
        private void Awake()
        {
            if (PlayOnAwake)
            {
                isEnable = true;
                Play(StartPos, EndPos, Duration);
            }
        }

        public int GetID()
        {
            if (ID == 0) return GetInstanceID();
            return ID;
        }

        public void Replay()
        {
            isEnable = true;
            transform.localPosition = StartPos;
            Play(StartPos, EndPos, Duration);
        }

        public void Play(Vector3 startPos, Vector3 endPos, float duration)
        {
            StartPos = startPos;
            EndPos = endPos;
            this.Duration = duration;

            isEnable = true;
            m_Timer = 0f;
            m_SpeedTimer =0;
            PositionTweenController.OnAdd(this);
        }

        public void Play(Transform Start, Transform End, float duration = -1)
        {
            if (End == null) return;
            m_Timer = 0;
            m_SpeedTimer =0;
            if (duration > 0) this.Duration = duration;
            isEnable = true;
            if (Start != null)
            {
                if(isLocalPos)  StartPos = Start.localPosition;
                else StartPos = Start.position;
            }
            else
            {
                if (isLocalPos) StartPos = transform.localPosition;
                else StartPos = transform.position;
            }
            m_pToTarget = End;
        }

        public void Stop()
        {
            isEnable = false;
            m_Timer = 0f;
            m_SpeedTimer =0;
            m_pToTarget = null;
        }
        public void OnComplete()
        {
            if (OnGlobalTweenEffectCompleted != null)
                OnGlobalTweenEffectCompleted(this);
            OnCompleteAction?.Invoke();

            if (bAutoDestroy)
            {
                if (m_pParentAble != null) m_pParentAble.RecyleDestroy();
                else GameObject.Destroy(this.gameObject);
                m_pToTarget = null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnable == false)
            {
                return;
            }
            if (Duration <= 0)
            {
                isEnable = false;
                return;
            }
            float timeScale = 1;
            if(SpeedCurve!=null)
            {
                m_SpeedTimer += Time.deltaTime;
                timeScale = SpeedCurve.Evaluate(m_SpeedTimer);
            }
            m_Timer += Time.deltaTime*timeScale;

            if (m_Timer >= Duration && isLoop == false)
            {
                isEnable = false;
                m_Timer = 0f;
                m_SpeedTimer=0;
                OnComplete();
                return;
            }
            if(m_Timer >= Duration && isPingPong)
            {
                var temp = EndPos;
                EndPos = StartPos;
                StartPos = temp;
            }
            if (isLoop)
            {
                if (m_Timer == Duration)
                {

                }
                else
                {
                    m_Timer %= Duration;
                }
            }

            if (isLocalPos)
            {
                if (m_pToTarget)
                {
                    if (IfTransformConvertUI)
                    {
                        Vector3 worldUIPos = Vector3.zero;
                        if (AUIManager.getInstance()!=null && AUIManager.getInstance().ConvertWorldPosToUIPos(m_pToTarget.position, true, ref worldUIPos))
                        {
                            transform.position = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, worldUIPos + EndTan, worldUIPos);
                        }
                        else
                        {
                            m_Timer = 0f;
                            m_SpeedTimer=0;
                            OnComplete();
                        }
                    }
                    else
                        transform.localPosition = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, m_pToTarget.localPosition + EndTan, m_pToTarget.localPosition);
                }
                else
                    transform.localPosition = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, EndPos + EndTan, EndPos);
            }
            else
            {
                if (m_pToTarget)
                {
                    if(IfTransformConvertUI)
                    {
                         Vector3 worldUIPos = Vector3.zero;
                        if (AUIManager.getInstance() != null && AUIManager.getInstance().ConvertWorldPosToUIPos(m_pToTarget.position, false, ref worldUIPos))
                        {
                            transform.position = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, worldUIPos + EndTan, worldUIPos);
                        }
                        else
                        {
                            m_Timer = 0f;
                            m_SpeedTimer=0;
                            OnComplete();
                        }
                    }
                    else
                        transform.position = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, m_pToTarget.position + EndTan, m_pToTarget.position);
                }
                else
                    transform.position = BezierUtility.Bezier4(m_Timer / Duration, StartPos, StartPos + StartTan, EndPos + EndTan, EndPos);
            }

        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            PositionTweenController.OnRemove(this);
        }
        //------------------------------------------------------
        public void Destroy()
        {
            Stop();
            PositionTweenController.OnRemove(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PositionTween), true)]
    [CanEditMultipleObjects]
    public class PositionTweenEditor : Editor
    {
        //private void OnEnable()
        //{
        //    PositionTween pt = target as PositionTween;
        //    if (pt.parentAble == null)
        //    {
        //        pt.parentAble = pt.GetComponent<Framework.Core.AInstanceAble>();
        //        if(pt.parentAble == null)
        //            pt.parentAble = pt.GetComponentInParent<Framework.Core.AInstanceAble>();
        //    }
        //}
        //private void OnDisable()
        //{
        //    PositionTween pt = target as PositionTween;
        //    if (pt!=null && pt.parentAble == null)
        //    {
        //        pt.parentAble = pt.GetComponent<Framework.Core.AInstanceAble>();
        //        if (pt.parentAble == null)
        //            pt.parentAble = pt.GetComponentInParent<Framework.Core.AInstanceAble>();
        //    }
        //}
        private void OnSceneGUI()
        {
            PositionTween effector = target as PositionTween;

            Color col = Handles.color;

            SerializedProperty StartPos = serializedObject.FindProperty("StartPos");
            SerializedProperty EndPos = serializedObject.FindProperty("EndPos");

            {

                Handles.color = Color.green;
                StartPos.vector3Value = Handles.DoPositionHandle(StartPos.vector3Value, Quaternion.identity);
                effector.StartTan = Handles.DoPositionHandle(effector.StartTan + StartPos.vector3Value, Quaternion.identity) - StartPos.vector3Value;
                Handles.DrawLine(StartPos.vector3Value, StartPos.vector3Value + effector.StartTan);
                Handles.ArrowHandleCap(0, StartPos.vector3Value + effector.StartTan, Quaternion.LookRotation(effector.StartTan), 0.1f, EventType.Repaint);

                Handles.color = Color.red;
                EndPos.vector3Value = Handles.DoPositionHandle(EndPos.vector3Value, Quaternion.identity);
                effector.EndTan = Handles.DoPositionHandle(effector.EndTan + EndPos.vector3Value, Quaternion.identity) - EndPos.vector3Value;
                Handles.DrawLine(EndPos.vector3Value, EndPos.vector3Value + effector.EndTan);
                Handles.ArrowHandleCap(0, EndPos.vector3Value + effector.EndTan, Quaternion.LookRotation(effector.EndTan), 0.1f, EventType.Repaint);
            }

            float duration = effector.Duration;
            if (duration > 0)
            {
                Handles.color = Color.yellow;
                Vector3 prePos = Vector3.zero;
                float time = 0f;
                Vector3 m1 = StartPos.vector3Value + effector.StartTan;
                Vector3 m2 = EndPos.vector3Value + effector.EndTan;
                float fDelta = duration / 30f;
                while (time <= duration)
                {
                    Vector3 pos = BezierUtility.Bezier4(Mathf.Clamp01(time / duration), StartPos.vector3Value, m1, m2, EndPos.vector3Value);
                    UnityEditor.Handles.color = Color.yellow;
                    UnityEditor.Handles.DrawLine(prePos, pos);
                    prePos = pos;
                    time += fDelta;
                }
            }


            Handles.color = col;
        }
    }
#endif
}