/********************************************************************
生成日期:	06:30:2025
类    名: 	CutsceneEditor
作    者:	HappLI
描    述:	过场动作编辑器
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Framework.War.Editor
{
    public class WarWorldEditor : EditorWindowBase
    {
        private const float EDGE_SNAP_OFFSET = 1;
        enum EDragEdge
        {
            None = 0,
            Asset,
            Inspector,
            Timeline,
        }
        struct SplitData
        {
            public float rate;
            public bool bOpen;
        }

        Rect m_PreviewRect;

        float m_fToolSize = 25;
        Rect m_InspectorRect;
        Vector2 m_InspectorScoller;
        Rect m_TimelineRect;
        Vector2 m_TimelineScoller;
        Rect m_AssetRect;
        Vector2 m_AssetScoller;

        EDragEdge m_eDragEdge = EDragEdge.None;

        SplitData m_ViewLeftRate;
        SplitData m_ViewRightRate;
        SplitData m_ViewHeightRate;

        GUIStyle m_TileStyle;
        string m_lastContentMd5 = null;
        //--------------------------------------------------------
        [MenuItem("Tools/战争世界编辑器")]
        public static void Open()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
                return;
            }
            WarWorldEditor window = EditorWindow.GetWindow<WarWorldEditor>();
            window.titleContent = new GUIContent("战争世界编辑器");
        }
        //--------------------------------------------------------
        protected override void OnInnerEnable()
        {
            this.minSize = new Vector2(600, 400);
            this.wantsMouseMove = true;
            this.wantsMouseEnterLeaveWindow = true;
            this.autoRepaintOnSceneChange = true;
            this.wantsLessLayoutEvents = true;

            m_ViewLeftRate.bOpen = true;
            m_ViewLeftRate.rate = 0.25f;
            m_ViewRightRate.bOpen = true;
            m_ViewRightRate.rate = 0.75f;
            m_ViewHeightRate.bOpen = true;
            m_ViewHeightRate.rate = 0.5f;

            m_TileStyle = new GUIStyle();
            m_TileStyle.fontSize = 20;
            m_TileStyle.normal.textColor = Color.white;
            m_TileStyle.alignment = TextAnchor.MiddleCenter;
        }
        //--------------------------------------------------------
        protected override void OnInnerDisable()
        {
        }
        //--------------------------------------------------------
        protected override void OnInnerUpdate()
        {
            RefreshLayout();
        }
        //--------------------------------------------------------
        public Rect InspectorRect
        {
            get { return m_InspectorRect; }
        }
        //--------------------------------------------------------
        public Rect AssetRect
        {
            get { return m_AssetRect; }
        }
        //--------------------------------------------------------
        public Rect PreviewRect
        {
            get { return m_PreviewRect; }
        }
        //--------------------------------------------------------
        public Rect TimelineRect
        {
            get { return m_TimelineRect; }
        }
        //--------------------------------------------------------
        public GUIStyle TileStyle
        {
            get { return m_TileStyle; }
        }
        //--------------------------------------------------------
        public override void OnChangeSelect(object pObject)
        {
            if (m_pCurrentObj != pObject)
            {
            }
            base.OnChangeSelect(pObject);

            m_lastContentMd5 = null;
        }
        //--------------------------------------------------------
        public override void SaveChanges()
        {
            base.SaveChanges();
            m_lastContentMd5 = null;
        }
        //--------------------------------------------------------
        protected override void OnInnerGUI()
        {
            ProcessDragEdge(Event.current);
        }
        //--------------------------------------------------------
        protected override void OnAfterInnerGUI()
        {
            //! top split toobar line
            UIDrawUtils.DrawColorLine(new Vector2(0, m_fToolSize), new Vector2(this.position.width, m_fToolSize), Color.grey);

            //! asset line
            {
                Color lineColor = Color.grey;
                if (CheckEdgeDrag(EDragEdge.Asset, new Rect(m_AssetRect.xMin, m_AssetRect.yMin, m_AssetRect.width, 10)))
                    lineColor = Color.yellow;
                UIDrawUtils.DrawColorLine(new Vector2(m_AssetRect.xMax, m_AssetRect.yMin), new Vector2(m_AssetRect.xMax, m_AssetRect.yMax), lineColor);
            }
            //! inspector
            {
                Color lineColor = Color.grey;
                if (CheckEdgeDrag(EDragEdge.Inspector, new Rect(m_InspectorRect.xMin, m_InspectorRect.yMin, m_InspectorRect.width, 10)))
                    lineColor = Color.yellow;
                UIDrawUtils.DrawColorLine(new Vector2(m_InspectorRect.xMin + 0.01f, m_InspectorRect.yMin), new Vector2(m_InspectorRect.xMin + 0.01f, m_InspectorRect.yMax), lineColor);
            }
            //! timeline split line
            {
                Color lineColor = Color.grey;
                if (CheckEdgeDrag(EDragEdge.Timeline, new Rect(m_TimelineRect.xMax - 300, m_TimelineRect.yMin, m_TimelineRect.width, 10)))
                    lineColor = Color.yellow;
                UIDrawUtils.DrawColorLine(new Vector2(m_TimelineRect.xMin, m_TimelineRect.yMin + 0.01f), new Vector2(m_TimelineRect.xMax, m_TimelineRect.yMin + 0.01f), lineColor);
            }

            DrawControllBtn();
        }
        //--------------------------------------------------------
        void DrawControllBtn()
        {
            //! asset line
            {
                if (m_ViewLeftRate.bOpen)
                {
                    var btnRect = new Rect(m_AssetRect.xMax - 21, m_AssetRect.y + 1, 20, 20);
                    if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("StepLeftButton").image) || CheckBtnRectClick(btnRect))
                    {
                        m_ViewLeftRate.bOpen = false;
                    }
                }
                else
                {
                    var btnRect = new Rect(-1, m_AssetRect.y + 1, 20, 20);
                    if (GUI.Button(btnRect, (EditorGUIUtility.TrIconContent("StepButton").image)) || CheckBtnRectClick(btnRect))
                        m_ViewLeftRate.bOpen = true;
                }
            }
            //! inspector
            {
                if (m_ViewRightRate.bOpen)
                {
                    Rect btnRect = new Rect(m_InspectorRect.x, m_InspectorRect.y + 1, 20, 20);
                    if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("StepButton").image) || CheckBtnRectClick(btnRect))
                        m_ViewRightRate.bOpen = false;

                }
                else
                {
                    Rect btnRect = new Rect(position.width - 18, m_InspectorRect.y + 1, 20, 20);
                    if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("StepLeftButton").image) || CheckBtnRectClick(btnRect))
                        m_ViewRightRate.bOpen = true;
                }
            }
            //! timline
            {
                if (m_ViewHeightRate.bOpen)
                {
                    Rect btnRect = new Rect(m_TimelineRect.xMax - 40, m_TimelineRect.y + 1, 20, 20);
                    if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("StepButton").image) || CheckBtnRectClick(btnRect))
                        m_ViewHeightRate.bOpen = false;

                }
                else
                {
                    Rect btnRect = new Rect(m_TimelineRect.xMax - 40, m_TimelineRect.y + 1, 20, 20);
                    Rect dragRect = new Rect(m_TimelineRect.xMax - 350, m_TimelineRect.y + 1, 350, 20);
                    if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("StepLeftButton").image) || CheckBtnRectClick(dragRect))
                        m_ViewHeightRate.bOpen = true;
                }
            }
        }
        //--------------------------------------------------------
        bool CheckBtnRectClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                return true;
            }
            return false;
        }
        //--------------------------------------------------------
        bool CheckEdgeDrag(EDragEdge type, Rect region)
        {
            if (m_eDragEdge == type) return true;
            if (region.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDrag)
                    m_eDragEdge = type;
                return true;
            }
            return false;
        }
        //--------------------------------------------------------
        void ProcessDragEdge(Event evt)
        {
            if (m_eDragEdge == EDragEdge.None) return;
            if (evt.type == EventType.MouseUp)
            {
                m_eDragEdge = EDragEdge.None;
                return;
            }
            if (evt.button == 0)
            {
                if (evt.type != EventType.MouseDrag)
                    return;

                switch (m_eDragEdge)
                {
                    case EDragEdge.Asset:
                        {
                            m_ViewLeftRate.rate += Event.current.delta.x / this.position.width;
                            evt.Use();
                        }
                        break;
                    case EDragEdge.Inspector:
                        {
                            m_ViewRightRate.rate += Event.current.delta.x / this.position.width;
                            evt.Use();
                        }
                        break;
                    case EDragEdge.Timeline:
                        {
                            m_ViewHeightRate.rate += Event.current.delta.y / this.position.height;
                            evt.Use();
                        }
                        break;
                }
            }
        }
        //--------------------------------------------------------
        void RefreshLayout()
        {
            m_ViewLeftRate.rate = Mathf.Clamp(m_ViewLeftRate.rate, 0.1f, 0.5f);
            float leftRate = m_ViewLeftRate.bOpen ? m_ViewLeftRate.rate : 0;

            m_ViewRightRate.rate = Mathf.Clamp(m_ViewRightRate.rate, 0.55f, 0.8f);
            float rightRate = m_ViewRightRate.bOpen ? m_ViewRightRate.rate : 0;

            m_ViewHeightRate.rate = Mathf.Clamp(m_ViewHeightRate.rate, 0.25f, 0.9f);
            float heightRate = m_ViewHeightRate.bOpen ? m_ViewHeightRate.rate : 0;


            m_AssetRect.y = m_InspectorRect.y = m_fToolSize;
            m_AssetRect.width = this.position.width * leftRate;
            if (!m_ViewLeftRate.bOpen) m_AssetRect.width = 20;

            m_TimelineRect.y = this.position.height * heightRate;
            if (!m_ViewHeightRate.bOpen) m_TimelineRect.y = this.position.height - 20;
            m_TimelineRect.height = this.position.height - m_TimelineRect.y;
            if (!m_ViewHeightRate.bOpen) m_InspectorRect.height = 20;
            m_TimelineRect.width = this.position.width;

            if (m_ViewRightRate.bOpen)
                m_InspectorRect.x = this.position.width * rightRate;
            else
                m_InspectorRect.x = this.position.width;
            m_InspectorRect.width = this.position.width - m_InspectorRect.x;
            if (!m_ViewRightRate.bOpen) m_InspectorRect.width = 20;
            m_InspectorRect.height = this.position.height - m_TimelineRect.height - m_fToolSize;
            m_AssetRect.height = m_InspectorRect.height;

            m_PreviewRect.x = m_AssetRect.xMax;
            m_PreviewRect.y = m_AssetRect.y;
            m_PreviewRect.width = this.position.width - m_AssetRect.width - m_InspectorRect.width;
            m_PreviewRect.height = this.position.height - m_TimelineRect.height - m_AssetRect.y;

        }
        //-----------------------------------------------------
        protected override void OnInnerEvent(Event evt)
        {
        }
    }
}

#endif