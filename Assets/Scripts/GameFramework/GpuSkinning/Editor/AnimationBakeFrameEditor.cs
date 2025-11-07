#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.Plugin
{
    public class AnimationBakeFrameEditor : EditorWindow
    {
        public static Action OnEditorEnd = null;
        private static System.Type realType;
        private static PropertyInfo s_property_handleWireMaterial;

        private static void InitType()
        {
            if (realType == null)
            {
                realType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.HandleUtility");
                s_property_handleWireMaterial = realType.GetProperty("handleWireMaterial", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }
        }

        static Material handleWireMaterial
        {
            get
            {
                InitType();
                return (s_property_handleWireMaterial.GetValue(null, null) as Material);
            }
        }

        public class FrameIndex
        {
            public int frameTime;
        }
        public static AnimationBakeFrameEditor Instance { protected set; get; }

        public static float LeftWidth = 350f;
        public static float RightWidth = 0;
        public static float GapTop = 0f;
        public static float GapBottom = 50f;

        private static int timelineHash = "timelinecontrol".GetHashCode();
        private int m_hotEventKey;
        private float m_tempPreviewPlaybackTime;
        public float m_fMaxKeyTime = 60f;
        public float m_fCurKeyTime = 0f;
        private float m_fPlayTime = 0f;
        private double m_PreviousTime;
        public float m_fDeltaTime = 0.02f;
        public double fixedDeltaTime = 0.02;

        Vector2 m_inspectorPanelScrollPos;
        Vector2 m_layerPanelScrollPos;

        string m_BackupScene = "";
        public Rect previewRect = new Rect(0, 0, 0, 0);

        public FrameIndex m_SelectFrame = null;
        List<FrameIndex> m_vFrames = new List<FrameIndex>();

        Animator m_pAnimator = null;
        GameObject m_Target = null;
        UnityEngine.Object m_Asset = null;
        AnimationClip m_CurClip = null;
        List<string> popClip = new List<string>();
        List<AnimationClip> m_vClips = new List<AnimationClip>();
        SkinMeshBakerData m_Data;
        int m_preCull = 0;
        //-----------------------------------------------------
        public static void Show(UnityEngine.Object target, AnimatorController controller, SkinMeshBakerData uvData)
        {
            if (target == null) return;
            if (AnimationBakeFrameEditor.Instance == null)
            {
                AnimationBakeFrameEditor window = EditorWindow.GetWindow<AnimationBakeFrameEditor>();
                window.title = "烘焙数据指定帧";
            }
            Instance.SetTarget(target, controller, uvData);
        }
        //-----------------------------------------------------
        protected void OnDisable()
        {
            if (m_CurClip != null)
            {
                SkinMeshBakerData.BakeFrame frame = m_Data.getFrame(m_CurClip.name);
                frame.frams.Clear();
                for (int i = 0; i < m_vFrames.Count; ++i)
                {
                    frame.frams.Add(m_vFrames[i].frameTime);
                }
            }

            m_vClips.Clear();
            m_Data = null;
            BakeSkinUtil.DestroyObject(m_Target);

            if (OnEditorEnd != null) OnEditorEnd();
        }
        //-----------------------------------------------------
        public void SetTarget(UnityEngine.Object target, AnimatorController controller, SkinMeshBakerData uvData)
        {
            m_Asset = target;
            m_vClips.Clear();
            m_Data = uvData;
            BakeSkinUtil.DestroyObject(m_Target);
            m_Target = GameObject.Instantiate<GameObject>(target as GameObject);

            m_pAnimator = m_Target.GetComponent<Animator>();
            if (m_pAnimator == null) m_pAnimator = m_Target.AddComponent<Animator>();
            m_pAnimator.runtimeAnimatorController = controller;

            AnimationClip[] clips = m_pAnimator.runtimeAnimatorController.animationClips;
            if (clips != null)
            {
                m_vClips = new List<AnimationClip>(clips);
                popClip = new List<string>();
                for (int i = 0; i < m_vClips.Count; ++i)
                {
                    popClip.Add(m_vClips[i].name);

                    if (m_Data != null)
                    {
                        m_Data.getFrame(m_vClips[i].name, true);
                    }
                }
            }
        }
        //-----------------------------------------------------
        protected void OnEnable()
        {
            Instance = this;
            base.minSize = new Vector2(850f, 320f);
        }
        //-----------------------------------------------------
        private void OnSelectionChange()
        {

        }
        //-----------------------------------------------------
        private void Update()
        {
            m_fDeltaTime = (float)((EditorApplication.timeSinceStartup - m_PreviousTime) * 0.8f);
            this.Repaint();
            m_PreviousTime = EditorApplication.timeSinceStartup;

            if (m_pAnimator && m_CurClip)
            {
                m_CurClip.SampleAnimation(m_Target, m_fPlayTime);
            }
        }
        //-----------------------------------------------------
        private void OnGUI()
        {
            if (m_Target == null)
            {
                this.Reset();
                base.ShowNotification(new GUIContent("Init Failed."));
                return;
            }


            base.RemoveNotification();
          //  GUILayout.BeginHorizontal();
          //  EditorGUI.BeginChangeCheck();

            DrawLayerPanel();
            DrawInspectorPanel();

            m_fPlayTime = this.DrawTimelinePanel(m_fPlayTime);
            m_fPlayTime = Mathf.RoundToInt(m_fPlayTime * m_fMaxKeyTime) / m_fMaxKeyTime;

            if (Event.current != null)
            {
                if (Event.current.type == EventType.KeyUp)
                {
                    int frame = Mathf.RoundToInt(m_fPlayTime * m_fMaxKeyTime);
                    if (Event.current.keyCode == KeyCode.K)
                    {
                        bool bHas = false;
                        for (int i = 0; i < m_vFrames.Count; ++i)
                        {
                            if (m_vFrames[i].frameTime == frame)
                            {
                                bHas = true;
                                m_vFrames.RemoveAt(i);
                                break;
                            }
                        }
                        if (!bHas)
                            m_vFrames.Add(new FrameIndex() { frameTime = frame });
                    }
                }
            }
        }
        //-----------------------------------------------------
        private void Reset()
        {

        }
        //-----------------------------------------------------
        public void OnDestroy()
        {
        }
        //-----------------------------------------------------
        private void DrawLayerPanel()
        {
            GUILayout.BeginArea(new Rect(0, GapTop, LeftWidth, previewRect.height));
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(LeftWidth), GUILayout.Height(previewRect.height) });

            this.m_layerPanelScrollPos = GUILayout.BeginScrollView(this.m_layerPanelScrollPos, new GUILayoutOption[0]);

            GUILayout.BeginVertical();

            int index = m_CurClip != null ? m_vClips.IndexOf(m_CurClip) : -1;
            index = EditorGUILayout.Popup("动画", index, popClip.ToArray());
            if (index >= 0 && index < m_vClips.Count)
            {
                if (m_CurClip != m_vClips[index])
                {
                    if (m_CurClip != null)
                    {
                        SkinMeshBakerData.BakeFrame bake = m_Data.getFrame(m_CurClip.name);
                        if (bake != null)
                        {
                            bake.frams.Clear();
                            for (int i = 0; i < m_vFrames.Count; ++i)
                            {
                                bake.frams.Add(m_vFrames[i].frameTime);
                            }
                        }

                    }

                    m_CurClip = m_vClips[index];
                    m_fPlayTime = 0;
                    m_fMaxKeyTime = (int)Mathf.ClosestPowerOfTwo((int)(m_CurClip.frameRate * m_CurClip.length));

                    m_vFrames.Clear();
                    if (m_CurClip != null)
                    {
                        SkinMeshBakerData.BakeFrame bake = m_Data.getFrame(m_CurClip.name);
                        if (bake != null)
                        {
                            for (int i = 0; i < bake.frams.Count; ++i)
                            {
                                m_vFrames.Add(new FrameIndex() { frameTime = bake.frams[i] });
                            }
                        }

                    }
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        private void DrawPreview()
        {

        }
        //-----------------------------------------------------
        private void DrawInspectorPanel()
        {
            GUILayout.BeginArea(new Rect(position.width - RightWidth, GapTop, RightWidth, previewRect.height));

            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            this.m_inspectorPanelScrollPos = GUILayout.BeginScrollView(this.m_inspectorPanelScrollPos, new GUILayoutOption[0]);

            //TODO
            // ....

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        private float DrawTimelinePanel(float time)
        {
            Rect position = new Rect(10f, this.position.height - GapBottom, this.position.width - 80, GapBottom);// GUILayoutUtility.GetRect(500f, 10000f, (float)50f, (float)50f);
            int controlID = GUIUtility.GetControlID(timelineHash, FocusType.Passive, position);
            Rect rect2 = new Rect((position.x + (position.width * time)) - 5f, position.y + 2f, 10f, 20f);
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.MouseDown:
                    if (rect2.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseMove:
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float introduced10 = Mathf.Clamp(current.mousePosition.x, position.x, position.x + position.width);
                        time = (introduced10 - position.x) / position.width;
                        current.Use();

                        //drag preivew
                    }
                    break;

                default:
                    if (type == EventType.Repaint)
                    {
                        Rect rect = new Rect(position.x, position.y + 10f, position.width, 1.5f);
                        this.DrawTimeLine(rect, time, m_fMaxKeyTime, 5, 1);
                        Color backupColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        GUI.skin.horizontalSliderThumb.Draw(rect2, new GUIContent(), controlID);
                        GUI.backgroundColor = backupColor;
                    }
                    break;
            }

            for (int i = 0; i < m_vFrames.Count; ++i)
            {
                this.DrawEvents(position, m_vFrames[i]);
            }

            return time;
        }

        //-----------------------------------------------------
        private void DrawEventKey(Rect rect, FrameIndex key)
        {
            float normalizedTime = key.frameTime / m_fMaxKeyTime;
            Rect position = new Rect((rect.x + (rect.width * normalizedTime)) - 3f, rect.y + 25f, 6f, 18f);
            int hashCode = key.GetHashCode();
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (!position.Contains(current.mousePosition))
                    {
                        break;
                    }
                    this.m_hotEventKey = hashCode;
                    this.m_tempPreviewPlaybackTime = normalizedTime;
                    this.SelectEvent(key);
                    if (current.clickCount > 1)
                    {

                    }
                    current.Use();
                    return;

                case EventType.MouseUp:
                    if (this.m_hotEventKey == hashCode)
                    {
                        this.m_hotEventKey = 0;
                        if (current.button == 1)
                        {

                        }
                        current.Use();
                    }
                    break;

                case EventType.MouseMove:
                    break;

                case EventType.MouseDrag:
                    if (this.m_hotEventKey != hashCode)
                    {
                        break;
                    }
                    if (current.button == 0)
                    {
                        float num3 = Mathf.Clamp(current.mousePosition.x, rect.x, rect.x + rect.width);
                        key.frameTime = (int)((num3 - rect.x) / rect.width);
                        this.m_tempPreviewPlaybackTime = key.frameTime;

                        this.SelectEvent(key);
                    }
                    current.Use();
                    return;

                case EventType.Repaint:
                    {
                        Color color = GUI.color;
                        //   if (this.m_CurKey == key)
                        //   {
                        //       GUI.color = Color.red;
                        //   }
                        //   else
                        {
                            GUI.color = Color.green;
                        }
                        GUI.skin.button.Draw(position, new GUIContent(), hashCode);
                        GUI.color = color;
                        if ((this.m_hotEventKey == hashCode) || ((this.m_hotEventKey == 0) && position.Contains(current.mousePosition)))
                        {
                            string text = string.Format("{0}", key.frameTime);
                            Vector2 vector = EditorStyles.largeLabel.CalcSize(new GUIContent(text));
                            Rect rect3 = new Rect((rect.x + (rect.width * normalizedTime)) - (vector.x / 2f), rect.y - GapBottom * 0.5f, vector.x, vector.y);
                            EditorStyles.largeLabel.Draw(rect3, new GUIContent(text), hashCode);
                            return;
                        }
                        break;
                    }
                default:
                    return;
            }
        }
        //-----------------------------------------------------
        private void SelectEvent(FrameIndex keyTime)
        {
            m_SelectFrame = keyTime;
            if (m_SelectFrame == null) return;
            m_fCurKeyTime = m_SelectFrame.frameTime;
        }
        //-----------------------------------------------------
        private void DrawEvents(Rect rect, FrameIndex key)
        {
            float normalizedTime = key.frameTime / m_fMaxKeyTime;
            Rect position = new Rect((rect.x + (rect.width * normalizedTime)) - 3f, rect.y + 25f, 6f, 18f);
            int hashCode = key.GetHashCode();
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (!position.Contains(current.mousePosition))
                    {
                        break;
                    }
                    this.m_hotEventKey = hashCode;
                    this.m_tempPreviewPlaybackTime = normalizedTime;
                    this.SelectEvent(key);
                    if (current.clickCount > 1)
                    {

                    }
                    current.Use();
                    return;

                case EventType.MouseUp:
                    if (this.m_hotEventKey == hashCode)
                    {
                        this.m_hotEventKey = 0;
                        if (current.button == 1)
                        {

                        }
                        current.Use();
                    }
                    break;

                case EventType.MouseMove:
                    break;

                case EventType.MouseDrag:
                    if (this.m_hotEventKey != hashCode)
                    {
                        break;
                    }
                    if (current.button == 0)
                    {
                        float num3 = Mathf.Clamp(current.mousePosition.x, rect.x, rect.x + rect.width);
                        key.frameTime = (int)(((num3 - rect.x) / rect.width));
                        this.m_tempPreviewPlaybackTime = key.frameTime;

                        this.SelectEvent(key);
                    }
                    current.Use();
                    return;

                case EventType.Repaint:
                    {
                        Color color = GUI.color;
                        if (this.m_SelectFrame == key)
                        {
                            GUI.color = Color.yellow;
                        }
                        else
                        {
                            GUI.color = Color.blue;
                        }
                        GUI.skin.button.Draw(position, new GUIContent(), hashCode);
                        GUI.color = color;
                        if ((this.m_hotEventKey == hashCode) || ((this.m_hotEventKey == 0) && position.Contains(current.mousePosition)))
                        {
                            string text = string.Format("{0}", key.frameTime);
                            Vector2 vector = EditorStyles.largeLabel.CalcSize(new GUIContent(text));
                            Rect rect3 = new Rect((rect.x + (rect.width * normalizedTime)) - (vector.x / 2f), rect.y - GapBottom * 0.5f, vector.x, vector.y);
                            EditorStyles.largeLabel.Draw(rect3, new GUIContent(text), hashCode);
                            return;
                        }
                        break;
                    }
                default:
                    return;
            }
        }
        //-----------------------------------------------------
        private void DrawTimeLine(Rect rect, float currentFrame, float maxTime = 100f, int gapBig = 10, int gapSmall = 5)
        {
            if (Event.current.type == EventType.Repaint)
            {
                handleWireMaterial?.SetPass(0);
                Color c = new Color(1f, 0f, 0f, 0.75f);
                GL.Color(c);
                GL.Begin(1);

                GL.Vertex3(rect.x, rect.y, 0f);
                GL.Vertex3(rect.x + rect.width, rect.y, 0f);
                GL.Vertex3(rect.x, rect.y + 25f, 0f);
                GL.Vertex3(rect.x + rect.width, rect.y + 25f, 0f);
                for (int i = 0; i <= maxTime; i++)
                {
                    if ((i % gapBig) == 0)
                    {
                        GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y, 0f);
                        GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y + 15f, 0f);
                    }
                    else if ((i % gapSmall) == 0)
                    {
                        GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y, 0f);
                        GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y + 10f, 0f);
                    }
                    //else
                    //{
                    //    GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y, 0f);
                    //    GL.Vertex3(rect.x + ((rect.width * i) / maxTime), rect.y + 5f, 0f);
                    //}
                }
                c = new Color(1f, 0f, 0f, 1f);
                GL.Color(c);
                GL.Vertex3(rect.x + (rect.width * currentFrame), rect.y, 0f);
                GL.Vertex3(rect.x + (rect.width * currentFrame), rect.y + 20f, 0f);
                GL.End();

                for (int i = 0; i <= maxTime; i++)
                {
                    if ((i % gapSmall) == 0 || (i % gapBig) == 0 || i == maxTime)
                    {
                        if (i >= maxTime)
                            GUI.Label(new Rect(rect.x + ((rect.width * i) / maxTime) - 50, rect.y, 50, 20), i.ToString());
                        else
                            GUI.Label(new Rect(rect.x + ((rect.width * i) / maxTime), rect.y, 50, 20), i.ToString());
                    }
                }
            }
        }
    }
}


#endif