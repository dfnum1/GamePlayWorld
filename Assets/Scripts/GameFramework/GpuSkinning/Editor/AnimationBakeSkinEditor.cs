#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static Framework.ED.EditorUtil;

namespace Framework.Plugin
{
    public class AnimationBakeSkinEditor : EditorWindow
    {
        public static AnimationBakeSkinEditor Instance { protected set; get; }

        public AnimationBakeEditorLogic m_Logic;

        public static float LeftWidth = 300;
        public static float RightWidth = 300f;
        public static float GapTop = 50f;
        public static float GapBottom = 5f;

        ED.EditorTimer m_pEditorTimer = new ED.EditorTimer();

        Vector2 m_inspectorPanelScrollPos;
        Vector2 m_layerPanelScrollPos;

        string m_BackupScene = "";
        public Rect previewRect = new Rect(0, 0, 0, 0);

        private bool m_bInspectorOpen = false;
        string m_strMsgHelp = "";
        //-----------------------------------------------------
        [MenuItem("Tools/角色动画烘焙器")]
        public static void Init()
        {
            if (AnimationBakeSkinEditor.Instance == null)
            {
                AnimationBakeSkinEditor window = EditorWindow.GetWindow<AnimationBakeSkinEditor>();
                window.titleContent = new GUIContent("角色动画烘焙器");
                window.Show();
            }
        }
        //-----------------------------------------------------
        public static void Open(GameObject pTarget, bool bInspectorOpen = true)
        {
            Init();
            AnimationBakeSkinEditor.Instance.SetEditorTarget(pTarget, bInspectorOpen);
        }
        //-----------------------------------------------------
        void OnDisable()
        {
            if (m_Logic != null) m_Logic.OnDisable();
            if (!Application.isPlaying && m_BackupScene.Length > 0)
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(m_BackupScene);
        }
        //-----------------------------------------------------
        void OnEnable()
        {
            Instance = this;
            base.minSize = new Vector2(850f, 320f);

            m_Logic = new AnimationBakeEditorLogic();

            if (m_Logic != null)
                m_Logic.OnEnable();

            m_strMsgHelp = "按住F2 切换模型合并和烘焙\n";
        }
        //-----------------------------------------------------
        void SetEditorTarget(UnityEngine.Object target, bool bInspectorOPen = true)
        {
            if (target == null) return;
            m_Logic.SetTarget(target);
            m_bInspectorOpen = bInspectorOPen;
        }
        //-----------------------------------------------------
        private void OnSelectionChange()
        {

        }
        //-----------------------------------------------------
        void Update()
        {
            m_pEditorTimer.Update();
            if (m_Logic != null)
            {
                m_Logic.Update(m_pEditorTimer.deltaTime);
            }
            this.Repaint();
        }
        //-----------------------------------------------------
        private void DrawHelpBox()
        {
            Color color = GUI.color;
            GUI.color = Color.green;
            EditorGUILayout.HelpBox(m_strMsgHelp, MessageType.None);
            GUI.color = color;
        }
        //-----------------------------------------------------
        private void OnGUI()
        {
            if (m_Logic == null)
            {
                this.Reset();
                base.ShowNotification(new GUIContent("Init Failed."));
                return;
            }

            DrawToolPanel();
            DrawLayerPanel();
            DrawInspectorPanel();

            if (m_Logic != null && Event.current != null)
            {
                m_Logic.OnEvent(Event.current);
            }
        }
        //-----------------------------------------------------
        static public void OnSceneFunc(SceneView sceneView)
        {
        }
        //-----------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {

        }
        //-----------------------------------------------------
        private void OnInspectorUpdate()
        {
            base.Repaint();
        }
        //-----------------------------------------------------
        private void Reset()
        {

        }
        //-----------------------------------------------------
        private void OnReLoadAssetData()
        {
        }
        //-----------------------------------------------------
        private void OnReLoad()
        {
            if (m_Logic != null)
                m_Logic.SaveData();
        }
        //-----------------------------------------------------
        public void OnDestroy()
        {

        }
        //-----------------------------------------------------
        private void DrawLayerPanel()
        {
            float bottomHeight = 60; // 60 + GapBottom;
            GUILayout.BeginArea(new Rect(0, GapTop, LeftWidth, position.height - bottomHeight));
            {
                GUILayout.BeginVertical();
                DrawHelpBox();
                m_Logic.OnDrawLayerPanel();
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        private void DrawPreview()
        {
        }
        //-----------------------------------------------------
        private void DrawInspectorPanel()
        {
            GUILayout.BeginArea(new Rect(LeftWidth + 5, GapTop, position.width - LeftWidth - 5, position.height));
            {
                GUILayout.BeginVertical();
                {
                    this.m_inspectorPanelScrollPos = GUILayout.BeginScrollView(this.m_inspectorPanelScrollPos, new GUILayoutOption[0]);
                    m_Logic.OnDrawInspectorPanel();
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        private void DrawToolPanel()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, GapTop));
            {
                GUILayout.BeginHorizontal();
                {
                    GUI.SetNextControlName("ReLoad_CT");
                    if (GUILayout.Button("ReLoad", new GUILayoutOption[] { GUILayout.Width(80f), GUILayout.Height(45f) }))
                    {
                        OnReLoad();
                    }
                    if (!m_bInspectorOpen)
                    {
                        //                 if (GUILayout.Button("SavePrefab", new GUILayoutOption[] { GUILayout.Width(80f), GUILayout.Height(45f) }))
                        //                 {
                        //                     if (m_Logic != null)
                        //                         m_Logic.SaveToPrefab();
                        //                 }
                    }
                    else
                    {
                        if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(80f), GUILayout.Height(45f) }))
                        {
                            if (m_Logic != null)
                                m_Logic.SaveData();
                        }
                    }
                    if (GUILayout.Button("Close", new GUILayoutOption[] { GUILayout.Width(80f), GUILayout.Height(45f) }))
                    {
                        if (m_Logic != null)
                            m_Logic.SaveData();

                        base.Close();
                    }


                    if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                    {
                        GUI.SetNextControlName("ReLoad_CT");
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }

}
#endif