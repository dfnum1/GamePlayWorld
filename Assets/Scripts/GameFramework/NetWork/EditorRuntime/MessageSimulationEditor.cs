#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	MessageSimulationEditor
作    者:	HappLI
描    述:	消息模拟编辑器
*********************************************************************/
using Framework.ED;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class MessageSimulationEditor : EditorWindow
    {
        static float GapTop = 1f;
        static float GapBottom = 1;
        Rect m_LayerSize = new Rect();
        Rect m_InspecSize = new Rect();
        Vector2 m_layerPanelScrollPos = Vector2.zero;
        Vector2 m_inspectorPanelScrollPos = Vector2.zero;

        EditorTimer m_pTimer = new EditorTimer();
        public static MessageSimulationEditor Instance { protected set; get; }

        MessageSimulationEditorLogic m_Logic = null;
        //-----------------------------------------------------
        [MenuItem("Tools/消息模拟器")]
        private static void StartEditor()
        {
            if (Instance == null)
                Instance = EditorWindow.GetWindow<MessageSimulationEditor>();
            Instance.titleContent = new GUIContent("消息模拟器");
        }
        //-----------------------------------------------------
        static public void OnSceneFunc(SceneView sceneView)
        {
            Instance.OnSceneGUI(sceneView);
        }
        //-----------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
            if (m_Logic != null)
                m_Logic.OnSceneGUI();
        }
        //-----------------------------------------------------
        public void SetEditorTarget(UnityEngine.Object value, bool bInspectorOPen = true)
        {
            if (value == null) return;
        }
        //-----------------------------------------------------
        private void RefreshChidAnimator(Transform root, bool bClear = true)
        {
            
        }
        //-----------------------------------------------------
        private void BuildObjectClips()
        {
           
        }
        //-----------------------------------------------------
        protected void OnDisable()
        {
            Instance = null;

            m_Logic.OnDisable();
        }
        //-----------------------------------------------------
        protected void OnEnable()
        {
            Instance = this;
            base.minSize = new Vector2(850f, 320f);
            m_Logic = new MessageSimulationEditorLogic();
            m_Logic.OnEnable();
        }
        //-----------------------------------------------------
        public void OnDraw(int controllerId, Camera camera)
        {

        }
        //-----------------------------------------------------
        protected void Update()
        {
            m_pTimer.Update();
            m_Logic.Update(m_pTimer.deltaTime);
            this.Repaint();
        }
        //-----------------------------------------------------
        private void OnGUI()
        {

            float width = position.width / 2;
            m_LayerSize = new Rect(0, GapTop, width, position.height - GapTop - GapBottom);
            m_InspecSize = new Rect(m_LayerSize.xMax+5, GapTop, position.width- (m_LayerSize.xMax + 5), position.height - GapTop - GapBottom);

            GUILayout.BeginHorizontal();

            DrawLayerPanel();
            DrawInspectorPanel();

            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.MaxHeight(GapBottom) });


            GUILayout.EndHorizontal();

            if (Event.current != null)
                OnEvent(Event.current);

            GUILayout.EndHorizontal();

            this.Repaint();
        }
        //-----------------------------------------------------
        private void OnEvent(Event evt)
        {
            m_Logic.OnEvent(evt);
        }
        //-----------------------------------------------------
        public void OnReLoadAssetData()
        {

        }
        //-----------------------------------------------------
        private void OnReLoad()
        {
            OnReLoadAssetData();
        }
        //-----------------------------------------------------
        public void OnDestroy()
        {
            m_Logic.Clear();
        }
        //-----------------------------------------------------
        private void DrawLayerPanel()
        {
            GUILayout.BeginArea(m_LayerSize);
            GUILayout.BeginVertical();

            float LinkHeight = 20f;
            this.m_layerPanelScrollPos = GUILayout.BeginScrollView(this.m_layerPanelScrollPos, new GUILayoutOption[] { GUILayout.Height(m_LayerSize.height- LinkHeight) });
            m_Logic.OnDrawLayerPanel(m_LayerSize.size-new Vector2(0, LinkHeight));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        private void DrawInspectorPanel()
        {
            GUILayout.BeginArea(m_InspecSize);

            GUILayout.BeginVertical("Box");

            this.m_inspectorPanelScrollPos = GUILayout.BeginScrollView(this.m_inspectorPanelScrollPos, new GUILayoutOption[] { GUILayout.Height(m_InspecSize.height) });
            m_Logic.OnDrawInspecPanel(m_InspecSize.size);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
#endif
