/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	CustomRectMask2D
作    者:	Happli
描    述:	自定义裁剪
*********************************************************************/
using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport]
    [ExecuteAlways]
    public class CustomRectMask2D : RectMask2D
    {
#if !UNITY_2019_4_OR_NEWER
        [SerializeField]
        private Vector4 m_Padding = new Vector4();

        /// <summary>
        /// Padding to be applied to the masking
        /// X = Left
        /// Y = Bottom
        /// Z = Right
        /// W = Top
        /// </summary>
        public Vector4 padding
        {
            get { return m_Padding; }
            set
            {
                m_Padding = value;
                MaskUtilities.Notify2DMaskStateChanged(this);
            }
        }
#endif
        public bool AutoPlay = true;
        public AnimationCurve offsetXCurve;
        public AnimationCurve offsetYCurve;
        public float startX;
        public float startY;
        public float widthFactor;
        public float heightFactor;

        RectTransform m_pRect;

        bool m_bPlayed = false;
        float m_fMaxTime = 0;
        float m_fDelta = 0;
        //------------------------------------------------------
        protected override void Awake()
        {
            if(AutoPlay)
            {
                Play();
            }
        }
        //------------------------------------------------------
        public void Play()
        {
            GetMaxTime();
            if (m_fMaxTime > 0) m_bPlayed = true;
        }
        //------------------------------------------------------
        public void Stop()
        {
            m_fDelta = 0;
            m_bPlayed = false;
        }
        //------------------------------------------------------
        public void SetDelta(float fDelta)
        {
            m_fDelta = fDelta;
        }
        //------------------------------------------------------
        private void Update()
        {
            if(m_bPlayed)
                ForceUpdate(Time.deltaTime);
        }
        //------------------------------------------------------
        public void ForceUpdate(float fDelta)
        {
            float offsetX =0;
            float offsetY = 0;
            if(m_fMaxTime>0)
            {
                m_fDelta += fDelta;
                if (offsetXCurve != null) offsetX = offsetXCurve.Evaluate(m_fDelta);
                if (offsetYCurve != null) offsetY = offsetYCurve.Evaluate(m_fDelta);
                if (m_fDelta >= m_fMaxTime)
                {
                    m_fDelta = 0;
                }
            }
            if (m_pRect == null)
                m_pRect = rectTransform;
            if (m_pRect == null) return;
            padding = new Vector4(startX+ offsetX, m_pRect.sizeDelta.y - startY- offsetY - m_pRect.sizeDelta.y * heightFactor, m_pRect.sizeDelta.x - startX- offsetX - m_pRect.sizeDelta.x * widthFactor, startY+ offsetY);
        }
        //------------------------------------------------------
        public float GetMaxTime()
        {
#if !UNITY_EDITOR
            if (m_fMaxTime >= 0) return m_fMaxTime;
#endif
            m_fMaxTime = 0;
            m_fMaxTime = Mathf.Max(m_fMaxTime, BaseUtil.GetCurveMaxTime(offsetXCurve));
            m_fMaxTime = Mathf.Max(m_fMaxTime, BaseUtil.GetCurveMaxTime(offsetYCurve));
            return m_fMaxTime;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomRectMask2D), true)]
    [CanEditMultipleObjects]
    public class CustomRectMask2DEditor : UnityEditor.UI.RectMask2DEditor
    {
        bool m_bPlaying = false;
        float m_fTimer = 0;
        Framework.ED.EditorTimer m_pTimer = new Framework.ED.EditorTimer();
#if UNITY_2019_4_OR_NEWER
        //------------------------------------------------------
        protected override void OnEnable()
        {
            EditorApplication.update += Update;
            base.OnEnable();
        }
        //------------------------------------------------------
        void OnDisabled()
        {
            EditorApplication.update -= Update;
        }
#endif
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            CustomRectMask2D image = target as CustomRectMask2D;
            if(image.GetMaxTime()>0)
                m_fTimer = EditorGUILayout.Slider("预览", m_fTimer, 0, image.GetMaxTime());

            RectTransform rect = image.transform as RectTransform;
            image.startX = EditorGUILayout.FloatField("startX", image.startX);
            image.startY = EditorGUILayout.FloatField("startY", image.startY);
            image.offsetXCurve = EditorGUILayout.CurveField("offsetXCurve", image.offsetXCurve);
            image.offsetYCurve = EditorGUILayout.CurveField("offsetYCurve", image.offsetYCurve);
            image.widthFactor = EditorGUILayout.FloatField("widthFactor", image.widthFactor);
            image.heightFactor = EditorGUILayout.FloatField("heightFactor", image.heightFactor);

            if(EditorGUI.EndChangeCheck())
            {
                image.SetDelta(m_fTimer);
                image.ForceUpdate(0);
                Vector2 size = rect.sizeDelta;
                rect.sizeDelta = size * 0.999f;
                rect.sizeDelta = size;

                serializedObject.ApplyModifiedProperties();

                SceneView.RepaintAll();
            }
            if (GUILayout.Button("播放"))
            {
                m_fTimer = 0;
                image.SetDelta(0);
                m_bPlaying = true;
            }
            if (GUILayout.Button("停止"))
            {
                m_fTimer = 0;
                image.SetDelta(0);
                m_bPlaying = false;
            }
        }
        //------------------------------------------------------
        private void OnSceneGUI()
        {
            CustomRectMask2D image = target as CustomRectMask2D;
            GUILayout.BeginArea(new Rect(0, 0, 500, 50));
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            m_fTimer = GUILayout.HorizontalSlider(m_fTimer, 0, image.GetMaxTime());
            if (GUILayout.Button("播放", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
            {
                m_fTimer = 0;
                image.SetDelta(0);
                m_bPlaying = true;
            }

            if (GUILayout.Button("停止", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
            {
                m_fTimer = 0;
                image.SetDelta(0);
                image.ForceUpdate(0);
                m_bPlaying = false;
            }
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                image.SetDelta(m_fTimer);
                image.ForceUpdate(0);
                Framework.ED.EditorUtil.RepaintPlayModeView();
            }
            GUILayout.EndArea();

        }
        //------------------------------------------------------
        void Update()
        {
            m_pTimer.Update();
            if(m_bPlaying)
            {
                CustomRectMask2D image = target as CustomRectMask2D;
                m_fTimer += m_pTimer.deltaTime;
                image.SetDelta(m_fTimer);
                image.ForceUpdate(0);
                Framework.ED.EditorUtil.RepaintPlayModeView();
            }
        }
    }
#endif
}