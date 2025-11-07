using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Base;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.RtgTween
{
    public abstract class ATweenEffecter : Framework.Core.AInstanceAble
    {
        public bool bAutoDestroy = true;
        public float fDuration = 0;
        public AnimationCurve speedCured;
        [SerializeField]
        public Vector3 StartPos = Vector3.zero;
        [SerializeField]
        public Vector3 EndPos = Vector3.zero;


        public bool bLocal = true;
        public float maxScaleTime = 1.2f;

        public Vector3 StartTangle = Vector3.right;
        public Vector3 EndTangle = Vector3.up;

        public AnimationCurve sacleX;
        public AnimationCurve sacleY;
        public AnimationCurve sacleZ;

        public AnimationCurve alpha;

        [System.NonSerialized]
        public System.Action<IUserData> OnTweenFinishCB = null;
        [System.NonSerialized]
        public IUserData Param = null;


        protected float m_fTime = 0;
        protected Vector3 m_StartPos = Vector3.zero;
        protected Vector3 m_RuntimeStartPos = Vector3.zero;
        protected Vector3 m_RuntimeEndPos = Vector3.zero;
        protected Vector3 m_RuntimeStartEndOffset = Vector3.zero;
#if UNITY_EDITOR
        protected bool m_bEditor = false;
#endif
        protected bool m_bPlaying = false;

        private float m_fSpeedDuration = 0;
        protected float m_fSpeed = 1;

        protected override void Awake()
        {
            base.Awake();
            m_RuntimeStartPos = StartPos;
            m_RuntimeEndPos = EndPos;
            m_RuntimeStartEndOffset = EndPos - StartPos;
            m_fTime = 0;
            m_fSpeedDuration = 0;
            m_fSpeed = 1;
            m_bPlaying = false;
        }
        //------------------------------------------------------
        public void SetRuntimeStartPos(Vector3 vPos, bool bAmount = false)
        {
            if (bAmount)
            {
                m_RuntimeStartPos += vPos;
                m_RuntimeEndPos += vPos;
            }
            else
            {
                m_RuntimeStartPos = vPos;
                m_RuntimeEndPos = vPos + m_RuntimeStartEndOffset;
            }
        }
        //------------------------------------------------------
        public void SetRuntimeEndPos(Vector3 vPos, bool bAmount = false)
        {
            if (bAmount) m_RuntimeEndPos += vPos;
            else m_RuntimeEndPos = vPos;
        }
        //------------------------------------------------------
        public virtual void Play(Vector3 startPos, bool bEditor = false, float fromTime = 0)
        {
            if (m_pTransform == null) m_pTransform = transform;

#if UNITY_EDITOR
            m_bEditor = bEditor;
            if (m_bEditor)
            {
                if (m_pTransform != null && (m_pTransform.parent == null || m_pTransform.parent.GetComponent<Canvas>() == null))
                {
                    Transform uiRoot = null;
                    Canvas root = GameObject.FindObjectOfType<Canvas>();
                    if (root != null)
                    {
                        uiRoot = root.transform;
                        m_pTransform.SetParent(root.transform);
                    }
                    else
                    {
                        GameObject rootCanvs = new GameObject("Canvas");
                        rootCanvs.AddComponent<Canvas>();
                        uiRoot = rootCanvs.transform;
                    }
                    if (uiRoot != null)
                        m_pTransform.SetParent(uiRoot);
                }
            }
#endif
            if (fDuration <= 0) return;
            m_StartPos = startPos;
#if UNITY_EDITOR
            if (bLocal && !m_bEditor)
                m_pTransform.localPosition = startPos;
            else
                m_pTransform.position = startPos;
#else
            if (bLocal)
                m_pTransform.localPosition = startPos;
            else
                m_pTransform.position = startPos;
#endif
            m_fTime = fromTime;
            m_bPlaying = true;
            m_fSpeedDuration = 0;
            m_fSpeed = 1;
        }
        //------------------------------------------------------
        public virtual void Play(Vector3 startPos, Vector3 endPos, bool bEditor = false, float fromTime = 0)
        {
#if UNITY_EDITOR
            m_bEditor = bEditor;
#endif
            if (m_pTransform == null) m_pTransform = transform;
            if (fDuration <= 0) return;
            m_RuntimeStartPos = startPos;
            m_StartPos = Vector3.zero;
            m_RuntimeEndPos = endPos;
            m_fTime = fromTime;
            m_bPlaying = true;
            m_fSpeedDuration = 0;
            m_fSpeed = 1;
        }
        //------------------------------------------------------
        public virtual void Stop(bool bDestroyCheck = true)
        {
            m_bPlaying = false;
            m_fTime = -1;
            m_RuntimeEndPos = EndPos;
            m_RuntimeStartPos = StartPos;
            m_fSpeedDuration = 0;
            m_fSpeed = 1;
            if (!bDestroyCheck) return;

            if (bAutoDestroy)
            {
#if UNITY_EDITOR
                if (m_bEditor) return;
                FileSystemUtil.DeSpawnInstance(this, 2);
#else
               FileSystemUtil.DeSpawnInstance(this,2);
#endif
                SetUnActive();
            }

            if (OnTweenFinishCB != null)
            {
                OnTweenFinishCB(Param);
                OnTweenFinishCB = null;
                Param = null;
            }
        }
        //------------------------------------------------------
        public bool bEnd()
        {
            return m_fTime == -1 || fDuration <= 0 || IsRecyled();
        }
        //------------------------------------------------------
        public virtual bool IsPlaying()
        {
            return m_bPlaying && fDuration > 0 && m_fTime < fDuration;
        }
        //------------------------------------------------------
        void Update()
        {
            float fDelta = Time.deltaTime;
            if (Time.timeScale > maxScaleTime) fDelta = Time.fixedDeltaTime * maxScaleTime;
            ForceUpdate(fDelta);
        }
        //------------------------------------------------------
        protected void UpdateSpeed(float fDelta)
        {
            if (speedCured != null && BaseUtil.IsValidCurve(speedCured))
            {
                m_fSpeedDuration += fDelta;
                m_fSpeed = speedCured.Evaluate(m_fSpeedDuration);
                if (Mathf.Abs(m_fSpeed) <= 0.0001f)
                {
                    if (m_fSpeed > 0) m_fSpeed = 0.01f;
                    else m_fSpeed = -0.01f;
                }
            }
            else m_fSpeed = 1;
        }
        //------------------------------------------------------
        public virtual void ForceUpdate(float fFrameTime)
        {
            if (!m_bPlaying) return;
            if (m_fTime < 0 || fDuration <= 0 || m_pTransform == null) return;
            UpdateSpeed(fFrameTime);
            m_fTime += fFrameTime * m_fSpeed;

            Vector3 m1 = m_RuntimeStartPos + StartTangle;
            Vector3 m2 = m_RuntimeEndPos + EndTangle;
            float normalTime = m_fTime / fDuration;
#if UNITY_EDITOR
            if (bLocal && !m_bEditor)

                m_pTransform.localPosition = m_StartPos + BezierUtility.Bezier4(normalTime, m_RuntimeStartPos, m1, m2, m_RuntimeEndPos);
            else
                m_pTransform.position = BezierUtility.Bezier4(normalTime, m_RuntimeStartPos, m1, m2, m_RuntimeEndPos);
#else
            if (bLocal)
                m_pTransform.localPosition = m_StartPos + BezierUtility.Bezier4(normalTime, m_RuntimeStartPos, m1, m2, m_RuntimeEndPos);
            else
                m_pTransform.position = BezierUtility.Bezier4(normalTime, m_RuntimeStartPos, m1, m2, m_RuntimeEndPos);

#endif
            Vector3 scale = Vector3.zero;
            if (sacleX != null && sacleX.length > 0) scale.x += sacleX.Evaluate(normalTime);
            else scale.x = 1;
            if (sacleY != null && sacleY.length > 0) scale.y += sacleY.Evaluate(normalTime);
            else scale.y = 1;
            if (sacleZ != null && sacleZ.length > 0) scale.z += sacleZ.Evaluate(normalTime);
            else scale.z = 1;
            m_pTransform.localScale = scale;

            if (m_fTime >= fDuration)
            {
                Stop();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ATweenEffecter), true)]
    [CanEditMultipleObjects]
    public class ATweenEffecterEditor : Editor
    {
        Framework.ED.EditorTimer m_pTimer = new Framework.ED.EditorTimer();
        static AnimationCurve m_pTempCurve = null;
        private void OnEnable()
        {
            EditorApplication.update += OnPreviewUpdate;
        }
        private void OnDisable()
        {
            EditorApplication.update -= OnPreviewUpdate;
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ATweenEffecter effector = target as ATweenEffecter;

            if (target.GetType().IsSubclassOf(typeof(ATweenEffecter)))
            {
                System.Reflection.FieldInfo[] fields = effector.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                for (int i = 0; i < fields.Length; ++i)
                {
                    if (fields[i].IsNotSerialized) continue;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(fields[i].Name), true);
                }
            }


            if (effector.alpha == null || effector.alpha.length <= 0)
            {
                effector.alpha = new AnimationCurve();
                effector.alpha.AddKey(0, 0);
                effector.alpha.AddKey(1, 1);
            }
            effector.maxScaleTime = UnityEditor.EditorGUILayout.Slider("时间缩放最大倍数限制", effector.maxScaleTime, 1, 2);
            effector.bLocal = UnityEditor.EditorGUILayout.Toggle("相对位置", effector.bLocal);
            effector.bAutoDestroy = UnityEditor.EditorGUILayout.Toggle("自动销毁", effector.bAutoDestroy);

            effector.fDuration = UnityEditor.EditorGUILayout.FloatField("时长", effector.fDuration);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedCured"), new GUIContent("速度曲线"), true);

            effector.alpha = UnityEditor.EditorGUILayout.CurveField("透明度", effector.alpha);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartPos"), new GUIContent("模拟起点"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EndPos"), new GUIContent("模拟终点"), true);

            effector.StartTangle = EditorGUILayout.Vector3Field("起点切角", effector.StartTangle);
            effector.EndTangle = EditorGUILayout.Vector3Field("终点切角", effector.EndTangle);

            GUILayout.BeginHorizontal();
            effector.sacleX = UnityEditor.EditorGUILayout.CurveField("X轴缩放曲线", effector.sacleX);
            if (GUILayout.Button("复制"))
            {
                m_pTempCurve = effector.sacleX;
            }
            if (m_pTempCurve != null && m_pTempCurve != effector.sacleX && GUILayout.Button("黏贴"))
            {
                effector.sacleX.keys = m_pTempCurve.keys;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            effector.sacleY = UnityEditor.EditorGUILayout.CurveField("Y轴缩放曲线", effector.sacleY);
            if (GUILayout.Button("复制"))
            {
                m_pTempCurve = effector.sacleY;
            }
            if (m_pTempCurve != null && m_pTempCurve != effector.sacleY && GUILayout.Button("黏贴"))
            {
                effector.sacleY.keys = m_pTempCurve.keys;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            effector.sacleZ = UnityEditor.EditorGUILayout.CurveField("Z轴缩放曲线", effector.sacleZ);
            if (GUILayout.Button("复制"))
            {
                m_pTempCurve = effector.sacleZ;
            }
            if (m_pTempCurve != null && m_pTempCurve != effector.sacleZ && GUILayout.Button("黏贴"))
            {
                effector.sacleZ.keys = m_pTempCurve.keys;
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            if (GUILayout.Button("预览"))
            {
                effector.Play(Vector3.zero, true);
            }
        }

        private void OnSceneGUI()
        {
            ATweenEffecter effector = target as ATweenEffecter;

            Color col = Handles.color;

            if (effector.alpha == null || effector.alpha.length <= 0) return;
            //SerializedProperty StartPos = effector.StartPos;
            //SerializedProperty EndPos = serializedObject.FindProperty("EndPos");

            {

                Handles.color = Color.green;
                effector.StartPos = Handles.DoPositionHandle(effector.StartPos, Quaternion.identity);
                effector.StartTangle = Handles.DoPositionHandle(effector.StartTangle + effector.StartPos, Quaternion.identity) - effector.StartPos;
                Handles.DrawLine(effector.StartPos, effector.StartPos + effector.StartTangle);
                Handles.ArrowHandleCap(0, effector.StartPos + effector.StartTangle, Quaternion.LookRotation(effector.StartTangle), 0.1f, EventType.Repaint);

                Handles.color = Color.red;
                effector.EndPos = Handles.DoPositionHandle(effector.EndPos, Quaternion.identity);
                effector.EndTangle = Handles.DoPositionHandle(effector.EndTangle + effector.EndPos, Quaternion.identity) - effector.EndPos;
                Handles.DrawLine(effector.EndPos, effector.EndPos + effector.EndTangle);
                Handles.ArrowHandleCap(0, effector.EndPos + effector.EndTangle, Quaternion.LookRotation(effector.EndTangle), 0.1f, EventType.Repaint);
            }

            float duration = effector.fDuration;
            if (duration > 0)
            {
                float fSpeed = 1;
                float fSpeedDuration = 0;

                int stackLoop = 1000;
                Handles.color = Color.yellow;
                Vector3 prePos = Vector3.zero;
                float time = 0f;
                Vector3 m1 = effector.StartPos + effector.StartTangle;
                Vector3 m2 = effector.EndPos + effector.EndTangle;
                float fDelta = duration / 30f;
                while (time <= duration && stackLoop > 0)
                {
                    if (effector.speedCured != null && BaseUtil.IsValidCurve(effector.speedCured))
                    {
                        fSpeedDuration += fDelta;
                        fSpeed = effector.speedCured.Evaluate(fSpeedDuration);
                    }
                    Vector3 pos = BezierUtility.Bezier4(Mathf.Clamp01(time / duration), effector.StartPos, m1, m2, effector.EndPos);
                    if ((pos - prePos).sqrMagnitude > 0.1f)
                    {
                        UnityEditor.Handles.color = Color.yellow;
                        UnityEditor.Handles.DrawLine(prePos, pos);
                        prePos = pos;
                    }

                    time += fDelta * fSpeed;
                    stackLoop--;
                }
            }


            Handles.color = col;
        }
        //------------------------------------------------------
        void OnPreviewUpdate()
        {
            if (Application.isPlaying)
                return;
            m_pTimer.Update();
            ATweenEffecter effector = target as ATweenEffecter;
            if (effector.IsPlaying())
                effector.ForceUpdate(m_pTimer.deltaTime);
        }
    }
#endif
}
