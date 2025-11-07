#if UNITY_EDITOR && USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	GuideEditorLogic
作    者:	HappLI
描    述:	可视化编程编辑器
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.Guide
{
    public partial class GuideEditorLogic
    {
        public enum EActivityType{ Idle, HoldNode, DragNode, HoldGrid, DragGrid, DragLink }
        public static EActivityType currentActivity = EActivityType.Idle;

        public int topPadding { get { return 19/*isDocked() ? 19 : 22*/; } }
        private Func<bool> isDocked
        {
            get
            {
                if (_isDocked == null)
                {
                    BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                    MethodInfo isDockedMethod = typeof(GuideEditor).GetProperty("docked", fullBinding).GetGetMethod(true);
                    _isDocked = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), this, isDockedMethod);
                }
                return _isDocked;
            }
        }
        private Func<bool> _isDocked;

        static int ms_nPortGUID = 0;
        bool m_bDeleteSelected = false;
        public static bool isPanning { get; private set; }

        private GraphNode hoveredNode = null;
        private int hoveredPortGuid = -1;
        private int draggedOutputGuid = -1;
        private int draggedOutputTargetGuid = -1;
        private List<Vector2> draggedOutputReroutes = new List<Vector2>();

        public HashSet<int> lastportConnectionPoints = new HashSet<int>();
        public Dictionary<int, Rect> portConnectionPoints { get { return m_portConnectionPoints; } }
        private Dictionary<int, Rect> m_portConnectionPoints = new Dictionary<int, Rect>();
        private HashSet<System.Int64> m_vConnectionsSets = new HashSet<System.Int64>();
        public bool IsDraggingPort { get { return draggedOutputGuid != -1; } }
        public bool IsHoveringPort { get { return hoveredPortGuid != -1; } }
        public bool IsHoveringNode { get { return hoveredNode !=null; } }

        private Vector2 dragBoxStart;
        private Rect selectionBox;
        private bool isDoubleClick = false;


        public Vector2 panOffset { get { return _panOffset; } set { _panOffset = value; Repaint(); } }
        private Vector2 _panOffset;
        public float zoom { get { return _zoom; } set { _zoom = Mathf.Clamp(value, GuidePreferences.GetSettings().minZoom, GuidePreferences.GetSettings().maxZoom); Repaint(); } }
        private float _zoom = 1;

        private Matrix4x4 prevGuiMatrix;

        public static GuideGroup pCurrentGroup = null;
        Dictionary<int, double> m_vExcudingNodes = new Dictionary<int, double>();
        float m_fCheckExcudingGap = 0;
        GuideGroup m_pGroupData = null;
        UnityEngine.Object m_pSrcPrefab = null;

        public static Vector2[] dragOffset;
        protected List<GraphNode> m_SelectionCache;
        public List<GraphNode> SelectonCache
        {
            get { return m_SelectionCache; }
        }

        bool m_bParseDo = false;
        public List<GraphNode> CopySelectionCathes = null;


        private GraphNode[] m_preBoxSelection;

        private List<GraphNode> m_culledNodes;
        Dictionary<int, GraphNode> m_vActioNodes = new Dictionary<int, GraphNode>();
        public Dictionary<GraphNode, Vector2> nodeSizes { get { return m_nodeSizes; } }
        private Dictionary<GraphNode, Vector2> m_nodeSizes = new Dictionary<GraphNode, Vector2>();

        GuideEditor m_pEditor;
        NodeSearcher m_pSearcher = new NodeSearcher();
        EventInspector m_pEventInspector = new EventInspector();

        GUIStyle m_pNameTileStyle = null;

        public void OnEnable(GuideEditor pEditor)
        {
            m_pEditor = pEditor;
            m_bDeleteSelected = false;
            pCurrentGroup = null;
        }
        //------------------------------------------------------
        public void OnDisable()
        {
            if (GuidePreferences.GetSettings().autoSave)
                Save(false);
            m_bDeleteSelected = false;
            pCurrentGroup = null;
        }
        //------------------------------------------------------
        public void OnGUI()
        {
            if (m_pNameTileStyle == null)
            {
                m_pNameTileStyle = new GUIStyle();
                m_pNameTileStyle.alignment = TextAnchor.MiddleCenter;
                m_pNameTileStyle.fontSize = 20;
                m_pNameTileStyle.fontStyle = FontStyle.Bold;
                m_pNameTileStyle.normal.textColor = Color.white;
            }
            Event evt = Event.current;
            Matrix4x4 m = GUI.matrix;
            Controls();
            OnDraw();

            if (m_pGroupData == null) return;

            if (m_pEventInspector != null) m_pEventInspector.OnDraw(m_pEditor.position);
            m_pSearcher.OnDraw();
            GUI.matrix = m;

            if (Event.current.type == EventType.MouseUp)
            {
                if (!m_pSearcher.IsMouseIn(Event.current.mousePosition))
                    m_pSearcher.Close();
            }

            if (m_pGroupData != null)
            {
                GUI.Label(new Rect(0, topPadding * zoom, m_pEditor.position.width, 50), "当前引导组：" + m_pGroupData.Name + "[" + m_pGroupData.Guid + "]", m_pNameTileStyle);

                GUILayout.BeginArea(new Rect(m_pEditor.position.width - 250, topPadding * zoom, 250, 50));
                GUILayout.BeginHorizontal();
                float label = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50;
                m_pGroupData.Tag = (ushort)EditorGUILayout.IntField(new GUIContent("Tag","Tag 不能超65535"), m_pGroupData.Tag);
                EditorGUIUtility.labelWidth = label;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(m_pEditor.position.width - 250, topPadding * zoom + 40, 250, 80));
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 100;
                m_pGroupData.bRemoveOver = EditorGUILayout.Toggle("结束后移除", m_pGroupData.bRemoveOver);
                m_pGroupData.bChangeStateBreak = EditorGUILayout.Toggle(new GUIContent("允许状态切换中断", "若勾选移除，则在触发结束后，本次运行游戏内不再触发"), m_pGroupData.bChangeStateBreak);
                EditorGUIUtility.labelWidth = label;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
        //------------------------------------------------------
        public void ExpandNodes(bool bExpand)
        {
            foreach (var db in m_vActioNodes)
            {
                if (db.Value.bindNode != null)
                    db.Value.bindNode.bExpand = bExpand;
            }
        }
        //------------------------------------------------------
        public bool HasStepNodeWidgetGuide(int guide)
        {
            foreach (var db in m_vActioNodes)
            {
                if (db.Value.bindNode != null && db.Value.bindNode is StepNode)
                {
                    var step = db.Value.bindNode as StepNode;
                    GuideEditor.NodeAttr nodeAttr;
                    if (GuideEditor.Instance.StepTypes.TryGetValue(step.type, out nodeAttr))
                    {
                        var ports = step.GetArgvPorts();
                        if (nodeAttr.argvs.Count == ports.Count)
                        {
                            for (int j = 0; j < nodeAttr.argvs.Count; ++j)
                            {
                                if (nodeAttr.argvs[j].attr.displayType == typeof(GuideGuid))
                                {
                                    var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (field != null)
                                    {
                                        if (guide.ToString().CompareTo(field.GetValue(ports[j]).ToString()) == 0)
                                            return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool HasTriggerNode()
        {
            foreach (var db in m_vActioNodes)
            {
                if (db.Value.bindNode is TriggerNode)
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {
            if(m_SelectionCache!=null && m_SelectionCache.Count>0)
            {
                foreach (var db in m_SelectionCache)
                {
                    if (db != null)
                    {
                        db.OnSceneGUI(sceneView);
                    }
                }
            }
        }
        //------------------------------------------------------
        public void OpenInspector(List<IUserData> vEvents)
        {
            m_pEventInspector.Open(vEvents);
        }
        //------------------------------------------------------
        public void OpenSearcher(Vector2 mousePos)
        {
            m_pSearcher.Open(new Rect(mousePos.x, mousePos.y, 350,400));
        }
        //------------------------------------------------------
        public void CloseSearcher()
        {
            m_pSearcher.Close();
        }
        //------------------------------------------------------
        void OnChangeData()
        {
            RecodeDo();
        }
        //------------------------------------------------------
        void RecodeDo()
        {

        }
        //------------------------------------------------------
        public void UnRedo()
        {
        }
        //------------------------------------------------------
        public void Save(bool bSaveAs = false)
        {
            CheckExcuding();
            if (m_pGroupData == null)
            {
                GuideEditor.Instance.ShowNotification(new GUIContent("m_pGroupData = null,保存失败"));
                return;
            }

            List<StepNode> vSteps = new List<StepNode>();
            List<GuideOperate> vConditions = new List<GuideOperate>();
            List<TriggerNode> vTriggerNode = new List<TriggerNode>();
            List<ExcudeNode> vExcudes = new List<ExcudeNode>();
            List<ArgvPort> vPorts = new List<ArgvPort>();
            foreach(var db in m_vActioNodes)
            {
                if(db.Value.bindNode is StepNode)
                {
                    StepNode step = db.Value.bindNode as StepNode;
                    vSteps.Add(step);

                    step.pAutoExcudeNode = null;
                    step.pSuccessedListenerBreakNode = null;
                    if (step.bAutoNext)
                    {
                        ExternPort externPort = db.Value.GetExternPort(10);
                        if (externPort!=null && externPort.vLinks.Count > 0 && externPort.vLinks[0] != null)
                        {
                            //autonext
                            step.pAutoExcudeNode = db.Value.vExternPorts[0].vLinks[0].bindNode as ExcudeNode;
                        }
                    }
                    if(step.bSuccessedListenerBreak)
                    {
                        ExternPort externBreakPort = db.Value.GetExternPort(20);
                        if (externBreakPort != null && externBreakPort.vLinks.Count > 0 && externBreakPort.vLinks[0] != null)
                        {
                            // successed listener break
                            step.pSuccessedListenerBreakNode = db.Value.vExternPorts[0].vLinks[0].bindNode;
                        }
                    }

                    if (step.vOps == null) step.vOps = new List<GuideOperate>();
                    step.vOps.Clear();
                    step.pNext = null;
                    for (int i = 0; i < db.Value.Links.Count; ++i)
                    {
                        if (db.Value.Links[i].bindNode is GuideOperate)
                            step.vOps.Add(db.Value.Links[i].bindNode as GuideOperate);
                        else
                        {
                            SeqNode seqNod = db.Value.Links[i].bindNode as SeqNode;
                            if (seqNod != null)
                            {
                                step.pNext = seqNod;
                                step.nextGuid = seqNod.Guid;
                            }
                        }
                    }
                }
                else if (db.Value.bindNode is GuideOperate)
                {
                    GuideOperate cond = db.Value.bindNode as GuideOperate;
                    vConditions.Add(cond);

                    cond.pNext = null;
                    if(db.Value.Links.Count>0)
                    {
                        cond.pNext = db.Value.Links[0].bindNode;
                    }
                }
                else if (db.Value.bindNode is TriggerNode)
                {
                    TriggerNode trigger = db.Value.bindNode as TriggerNode;
                    vTriggerNode.Add(trigger);

                    if (trigger.vOps == null) trigger.vOps = new List<GuideOperate>();
                    trigger.vOps.Clear();
                    trigger.pNext = null;
                    for (int i = 0; i < db.Value.Links.Count; ++i)
                    {
                        if (db.Value.Links[i].bindNode is GuideOperate)
                            trigger.vOps.Add(db.Value.Links[i].bindNode as GuideOperate);
                        else
                            trigger.pNext = db.Value.Links[i].bindNode as SeqNode;
                    }
                }
                else if (db.Value.bindNode is ExcudeNode)
                {
                    ExcudeNode trigger = db.Value.bindNode as ExcudeNode;
                    vExcudes.Add(trigger);

                    if (trigger.vOps == null) trigger.vOps = new List<GuideOperate>();
                    trigger.vOps.Clear();
                    trigger.pNext = null;
                    for (int i = 0; i < db.Value.Links.Count; ++i)
                    {
                        if (db.Value.Links[i].bindNode is GuideOperate)
                            trigger.vOps.Add(db.Value.Links[i].bindNode as GuideOperate);
                        else
                            trigger.pNext = db.Value.Links[i].bindNode as SeqNode;
                    }
                }
            }

            int guid = 0;
            List<BaseNode> vNodes = new List<BaseNode>();
            foreach (var db in vSteps)
            {
                if(db.GetArgvPorts()!=null)
                {
                    foreach(var port in db.GetArgvPorts())
                    {
                        if (port.guid == 0) port.guid = BuildPortGUID();
                        vPorts.Add(port);
                    }
                }
                vNodes.Add(db);
            }
            foreach (var db in vConditions)
            {
                if (db.GetArgvPorts() != null)
                {
                    foreach (var port in db.GetArgvPorts())
                    {
                        if (port.guid == 0) port.guid = BuildPortGUID();
                        vPorts.Add(port);
                    }
                }
                vNodes.Add(db);
            }
            foreach (var db in vTriggerNode)
            {
                if (db.GetArgvPorts() != null)
                {
                    foreach (var port in db.GetArgvPorts())
                    {
                        if (port.guid == 0) port.guid = BuildPortGUID();
                        vPorts.Add(port);
                    }
                }
                vNodes.Add(db);
            }
            foreach (var db in vExcudes)
            {
                if (db.GetArgvPorts() != null)
                {
                    foreach (var port in db.GetArgvPorts())
                    {
                        if (port.guid == 0) port.guid = BuildPortGUID();
                        vPorts.Add(port);
                    }
                }
                vNodes.Add(db);
            }

            m_pGroupData.vSteps = vSteps;
            m_pGroupData.vOperates = vConditions;
            m_pGroupData.vTriggers = vTriggerNode;
            m_pGroupData.vExcudes = vExcudes;
            m_pGroupData.vPorts = vPorts;
            m_pGroupData.BuildMapping();
            foreach (var db in vNodes)
            {
                db.Save();
            }
            m_pGroupData.Save();
            m_pGroupData.Init(true);
            if (m_pSrcPrefab != null)
            {
                EditorUtility.SetDirty(m_pSrcPrefab);
            }

            m_pEditor.Save();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        //------------------------------------------------------
        public void Update(float fTime)
        {
            m_fCheckExcudingGap -= fTime;
            if (m_fCheckExcudingGap <=0)
            {
                CheckExcuding();
            }
            m_pEventInspector.Update(fTime);
            m_pSearcher.Update(fTime);
            if(m_bDeleteSelected)
            {
                RemoveSelectedNodes();
                m_bDeleteSelected = false;
            }
            double time = EditorApplication.timeSinceStartup;
            foreach (var db in m_vExcudingNodes)
            {
                if (db.Value < 0) continue;
                if(time-db.Value > 1f)
                {
                    m_vExcudingNodes.Remove(db.Key);
                    break;
                }
            }
            if (m_bParseDo)
            {
                m_bParseDo = false;
                DoParse();
            }
        }
        //------------------------------------------------------
        public void ExcudeNode(BaseNode pNode)
        {
            if (pNode.guideGroup != m_pGroupData) return;
            if (m_vExcudingNodes == null) m_vExcudingNodes = new Dictionary<int, double>();
            if(m_vExcudingNodes.Count>0)
            {
                int[] sKeys = new int[m_vExcudingNodes.Values.Count];
                m_vExcudingNodes.Keys.CopyTo(sKeys, 0);
                for (int i = 0; i < sKeys.Length; ++i)
                {
                    if (m_vExcudingNodes[sKeys[i]] < 0)
                        m_vExcudingNodes[sKeys[i]] = EditorApplication.timeSinceStartup;
                }
            }

            if(pNode is StepNode)
                m_vExcudingNodes[pNode.Guid] = -1;
            else
                m_vExcudingNodes[pNode.Guid] = EditorApplication.timeSinceStartup;
        }
        //------------------------------------------------------
        void CheckExcuding()
        {
            m_fCheckExcudingGap = 5;
        }
        //------------------------------------------------------
        private bool IsExcudingNode(int guid)
        {
            if (m_pGroupData == null) return false;
            if (m_vExcudingNodes.ContainsKey(guid)) return true;
            return false;
        }
        //------------------------------------------------------
        public void Controls()
        {
            m_pEditor.wantsMouseMove = true;
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseMove:
                    break;
                case EventType.ScrollWheel:
                    if (!m_pSearcher.IsMouseIn(e.mousePosition))
                    {
                        float oldZoom = zoom;
                        if (e.delta.y > 0) zoom += 0.1f * zoom;
                        else zoom -= 0.1f * zoom;
                        if (GuidePreferences.GetSettings().zoomToMouse) panOffset += (1 - oldZoom / zoom) * (WindowToGridPosition(e.mousePosition) + panOffset);
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        IPortNode draggedOutputPort = PortUtil.GetPort(draggedOutputGuid);
                        if (draggedOutputPort!=null)
                        {
                            IPortNode hoveredPort = PortUtil.GetPort(hoveredPortGuid);
                            if (hoveredPort != null && draggedOutputPort.CanConnectTo(hoveredPort))
                            {
                          //      if (!draggedOutputPort.IsConnectedTo(hoveredPort))
                                {
                                    draggedOutputTargetGuid = hoveredPortGuid;
                                }
                            }
                            else
                            {
                                draggedOutputTargetGuid = -1;
                            }
                            Repaint();
                        }
                        else if (currentActivity == EActivityType.HoldNode)
                        {
                            RecalculateDragOffsets(e);
                            currentActivity = EActivityType.DragNode;
                            Repaint();
                        }
                        if (currentActivity == EActivityType.DragNode)
                        {
                            // Holding ctrl inverts grid snap
                            bool gridSnap = GuidePreferences.GetSettings().gridSnap;
                            if (e.control) gridSnap = !gridSnap;

                            Vector2 mousePos = WindowToGridPosition(e.mousePosition);
                            // Move selected nodes with offset
                            for(int i = 0; i <m_SelectionCache.Count; ++i)
                            {
                                GraphNode pNode = m_SelectionCache[i];
                                if(pNode!=null)
                                {
                                    GraphNode node = m_SelectionCache[i];
                                    Vector2 initial = node.GetPosition();
                                    node.SetPosition(mousePos + dragOffset[i]);
                                    if (gridSnap)
                                    {
                                        Vector2 pos = node.GetPosition();
                                        pos.x = (Mathf.Round((pos.x + 8) / 16) * 16) - 8;
                                        pos.y = (Mathf.Round((pos.y + 8) / 16) * 16) - 8;
                                    }

                                    // Offset portConnectionPoints instantly if a node is dragged so they aren't delayed by a frame.
                                    Vector2 offset = node.GetPosition() - initial;
                                    if (offset.sqrMagnitude > 0)
                                    {
                                        foreach (IPortNode output in node.Port)
                                        {
                                            Rect rect;
                                            if (portConnectionPoints.TryGetValue(output.GetGUID(), out rect))
                                            {
                                                rect.position += offset;
                                                portConnectionPoints[output.GetGUID()] = rect;
                                            }
                                        }
                                    }
                                }
                            }
                            Repaint();
                        }
                        else if (currentActivity == EActivityType.HoldGrid)
                        {
                            currentActivity = EActivityType.DragGrid;
                            if (!Event.current.control) m_SelectionCache.Clear();
                            m_preBoxSelection = m_SelectionCache.ToArray();
                            dragBoxStart = WindowToGridPosition(e.mousePosition);
                            Repaint();
                        }
                        else if (currentActivity == EActivityType.DragGrid)
                        {
                            Vector2 boxStartPos = GridToWindowPosition(dragBoxStart);
                            Vector2 boxSize = e.mousePosition - boxStartPos;
                            if (boxSize.x < 0) { boxStartPos.x += boxSize.x; boxSize.x = Mathf.Abs(boxSize.x); }
                            if (boxSize.y < 0) { boxStartPos.y += boxSize.y; boxSize.y = Mathf.Abs(boxSize.y); }
                            selectionBox = new Rect(boxStartPos, boxSize);
                            Repaint();
                        }
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        panOffset += e.delta * zoom;
                        isPanning = true;
                    }
                    break;
                case EventType.MouseDown:
                    Repaint();
                    if (e.button == 0)
                    {
                        draggedOutputReroutes.Clear();

                        IPortNode hoveredPort = PortUtil.GetPort(hoveredPortGuid);
                        if (hoveredPort != null)
                        {
                            if (hoveredPort.IsOutput())
                            {
                                draggedOutputGuid = hoveredPortGuid;
                            }
                        }
                        else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                        {
                            // If mousedown on node header, select or deselect
                            if (!m_SelectionCache.Contains(hoveredNode))
                            {
                                SelectNode(hoveredNode, e.control || e.shift);
                            }
                            else if (e.control || e.shift) DeselectNode(hoveredNode);

                            // Cache double click state, but only act on it in MouseUp - Except ClickCount only works in mouseDown.
                            isDoubleClick = (e.clickCount == 2);

                            e.Use();
                            currentActivity = EActivityType.HoldNode;
                        }
                        // If mousedown on grid background, deselect all
                        else if (!IsHoveringNode)
                        {
                            currentActivity = EActivityType.HoldGrid;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        //Port drag release
                        if (IsDraggingPort)
                        {
                            //If connection is valid, save it
                            IPortNode draggedOutputTarget = PortUtil.GetPort(draggedOutputTargetGuid);
                            IPortNode draggedOutput = PortUtil.GetPort(draggedOutputGuid);
                            if (draggedOutputTarget != null && draggedOutput != null)
                            {
                                if(draggedOutputTarget.GetNode() != draggedOutput.GetNode())
                                {
                                    if (draggedOutputTarget.GetNode() != draggedOutput.GetNode())
                                    {
                                        IPortNode outPort, inPort;
                                        if (draggedOutputTarget.IsOutput())
                                        {
                                            outPort = draggedOutputTarget;
                                            inPort = draggedOutput;
                                        }
                                        else
                                        {
                                            inPort = draggedOutputTarget;
                                            outPort = draggedOutput;
                                        }
                                        if (inPort is SlotPort && outPort is SlotPort)
                                        {
                                            if (outPort.GetPort() is VarPort)
                                            {
                                                VarPort varPort = outPort.GetPort() as VarPort;
                                                if (varPort.ContainsDummyArgv(inPort.GetNode().GetGUID()))
                                                {
                                                    if (varPort.dummyMaps[inPort.GetNode().GetGUID()] != inPort.GetPort())
                                                    {
                                                        varPort.dummyMaps[inPort.GetNode().GetGUID()] = inPort.GetPort() as ArgvPort;
                                                    }
                                                    else
                                                        varPort.DelDummyArgv(outPort.GetNode().GetGUID());
                                                }
                                                else
                                                {
                                                    varPort.AddDummyArgv(inPort.GetNode().GetGUID(), inPort.GetPort() as ArgvPort);
                                                }
                                            }
                                            else if (inPort.GetPort() is VarPort)
                                            {
                                                VarPort varPort = inPort.GetPort() as VarPort;
                                                if (varPort.ContainsDummyArgv(outPort.GetNode().GetGUID()))
                                                {
                                                    if(varPort.dummyMaps[outPort.GetNode().GetGUID()] != outPort.GetPort())
                                                    {
                                                        varPort.dummyMaps[outPort.GetNode().GetGUID()] = outPort.GetPort() as ArgvPort;
                                                    }
                                                    else
                                                        varPort.DelDummyArgv(outPort.GetNode().GetGUID());
                                                }
                                                else
                                                {
                                                    varPort.AddDummyArgv(outPort.GetNode().GetGUID(), outPort.GetPort() as ArgvPort);
                                                }
                                            }
                                            else if( inPort.GetPort() is ArgvPort && outPort.GetPort() is ArgvPort )
                                            {
                                                if ((inPort as SlotPort).port == (outPort as SlotPort).port)
                                                {
                                                    (inPort as SlotPort).GetNode().bindNode.GetArgvPorts()[((inPort as SlotPort).index)] = new ArgvPort() { guid = GuideEditorLogic.BuildPortGUID() };
                                                }
                                                else
                                                    (inPort as SlotPort).GetNode().bindNode.GetArgvPorts()[((inPort as SlotPort).index)] = (outPort as SlotPort).port as ArgvPort;
                                            }
                                        }
                                        else if (inPort is LinkPort && outPort is LinkPort)
                                        {
                                            if(outPort is ExternPort)
                                            {
                                                if((outPort as ExternPort).reqNodeType ==null || inPort.GetNode().bindNode.GetType() == (outPort as ExternPort).reqNodeType )
                                                {
                                                    if ((outPort as ExternPort).vLinks.Contains(inPort.GetNode()))
                                                    {
                                                        (outPort as ExternPort).vLinks.Remove(inPort.GetNode());
                                                    }
                                                    else
                                                    {
                                                        (outPort as ExternPort).vLinks.Clear();
                                                        (outPort as ExternPort).vLinks.Add(inPort.GetNode());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (outPort.GetNode().Links.Contains(inPort.GetNode()))
                                                {
                                                    outPort.GetNode().Links.Remove(inPort.GetNode());
                                                }
                                                else
                                                {
                                                    if ((inPort.GetNode().bindNode is GuideOperate))
                                                    {
                                                        for(int j = 0; j < outPort.GetNode().Links.Count; ++j)
                                                        {
                                                            if(outPort.GetNode().Links[j].bindNode is SeqNode)
                                                            {
                                                                outPort.GetNode().Links.Clear();
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        outPort.GetNode().Links.Clear();
                                                    }
                                                    outPort.GetNode().Links.Add(inPort.GetNode());
                                                }
                                            }
       
                                        }
                                    }
                                }
                            }
                            //Release dragged connection
                            draggedOutputGuid = -1;
                            draggedOutputTargetGuid = -1;
                            if (m_pSrcPrefab != null) EditorUtility.SetDirty(m_pSrcPrefab);
                            if (GuidePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }
                        else if (currentActivity == EActivityType.DragNode)
                        {
                            if(m_pSrcPrefab != null) EditorUtility.SetDirty(m_pSrcPrefab);
                            if (GuidePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }
                        else if (!IsHoveringNode)
                        {
                            // If click outside node, release field focus
                            if (!isPanning)
                            {
                            //    EditorGUI.FocusTextInControl(null);
                           //     EditorGUIUtility.editingTextField = false;
                            }
                            if (GuidePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                        }

                        // If click node header, select it.
                        if (currentActivity == EActivityType.HoldNode && !(e.control || e.shift))
                        {
                            SelectNode(hoveredNode, false);

                            // Double click to center node
                            if (isDoubleClick)
                            {
                                Vector2 nodeDimension = nodeSizes.ContainsKey(hoveredNode) ? nodeSizes[hoveredNode] / 2 : Vector2.zero;
                                panOffset = -hoveredNode.GetPosition() - nodeDimension;
                            }
                        }

                        Repaint();
                        currentActivity = EActivityType.Idle;
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        if (!isPanning)
                        {
                            if (IsDraggingPort)
                            {
                                draggedOutputReroutes.Add(WindowToGridPosition(e.mousePosition));
                            }
                            else if (IsHoveringPort)
                            {
                                m_pEditor.ShowPortContextMenu(PortUtil.GetPort(hoveredPortGuid));
                            }
                            else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                            {
                                if (!m_SelectionCache.Contains(hoveredNode)) SelectNode(hoveredNode, false);
                                if (e.button == 1)
                                {
                                    if (e.control || m_pGroupData == null)
                                        m_pEditor.BaseFuncContextMenu(e.mousePosition);
                                    else
                                        m_pEditor.FuncContextMenu(e.mousePosition);
                                }
                                else m_pEditor.BaseFuncContextMenu(e.mousePosition);
                                e.Use();// Fixes copy/paste context menu appearing in Unity 5.6.6f2 - doesn't occur in 2018.3.2f1 Probably needs to be used in other places.
                            }
                            else if (!IsHoveringNode)
                            {
                                if (e.button == 1)
                                {
                                    if (e.control || m_pGroupData == null)
                                        m_pEditor.BaseFuncContextMenu(e.mousePosition);
                                    else
                                        m_pEditor.FuncContextMenu(e.mousePosition);
                                }
                                else m_pEditor.BaseFuncContextMenu(e.mousePosition);
                            }
                        }
                        isPanning = false;
                    }
                    // Reset DoubleClick
                    isDoubleClick = false;
                    break;
                case EventType.KeyDown:
                    if (EditorGUIUtility.editingTextField) break;
                    else if (e.keyCode == KeyCode.F) Home();
                    else if (e.keyCode == KeyCode.Escape)
                    {
                        m_pSearcher.Close();
                        m_SelectionCache.Clear();
                        m_pEventInspector.Close();
                    }
                    if (e.keyCode == KeyCode.Delete)
                    {
                        m_bDeleteSelected = true;
                    }
                    break;
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    if (IsMac() && e.commandName == "Delete")
                    {
                        if (e.type == EventType.ExecuteCommand) m_bDeleteSelected = true;// RemoveSelectedNodes();
                        e.Use();
                    }
                    Repaint();
                    break;
                case EventType.Ignore:
                    // If release mouse outside window
                    if (e.rawType == EventType.MouseUp && currentActivity == EActivityType.DragGrid)
                    {
                        Repaint();
                        currentActivity = EActivityType.Idle;
                    }
                    break;
            }
        }
        //------------------------------------------------------
        public bool IsMac()
        {
#if UNITY_2017_1_OR_NEWER
            return SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
#else
            return SystemInfo.operatingSystem.StartsWith("Mac");
#endif
        }
        //------------------------------------------------------
        public void TestGuide(BaseNode pStartNode = null)
        {
            if (m_pGroupData == null)
            {
                GuideEditor.Instance.ShowNotification(new GUIContent("m_pGroupData = null,测试失败"));
                return;
            }
            GuideSystem.getInstance().Enable(true);
            GuideSystem.getInstance().OverGuide();
            GuideSystem.getInstance().AddGuide(m_pGroupData, true);
            if (pStartNode == null && m_SelectionCache !=null && m_SelectionCache.Count>0)
            {
                for(int i = 0; i < m_SelectionCache.Count; ++i)
                {
                    if(!(m_SelectionCache[i].bindNode is TriggerNode))
                    {
                        pStartNode = m_SelectionCache[i].bindNode;
                        break;
                    }
                }
            }
            m_pGroupData.Init(true);
            GuideSystem.getInstance().SetTestFlag(true);
            var uiMgr = UI.AUIManager.getInstance();
            GuideSystem.getInstance().DoGuide(m_pGroupData.Guid, -1, pStartNode);
        }
        //------------------------------------------------------
        public void OverGuide()
        {
            GuideSystem.getInstance().OverGuide();
        }
        //------------------------------------------------------
        public void OnStopDoingGuide()
        {
            if (m_vExcudingNodes !=null && m_vExcudingNodes.Count > 0)
            {
                int[] sKeys = new int[m_vExcudingNodes.Values.Count];
                m_vExcudingNodes.Keys.CopyTo(sKeys, 0);
                for (int i = 0; i < sKeys.Length; ++i)
                {
                    if (m_vExcudingNodes[sKeys[i]] < 0)
                        m_vExcudingNodes[sKeys[i]] = EditorApplication.timeSinceStartup;
                }
            }
            pCurrentGroup = null;
        }
        //------------------------------------------------------
        private void RecalculateDragOffsets(Event current)
        {
            dragOffset = new Vector2[m_SelectionCache.Count];
            // Selected nodes
            for (int i = 0; i < m_SelectionCache.Count; i++)
            {
                GraphNode grap = m_SelectionCache[i];
                if (grap != null)
                {
                    dragOffset[i] = grap.GetPosition() - WindowToGridPosition(current.mousePosition);
                }
            }
        }
        //------------------------------------------------------
        public void ClearTempData()
        {
            m_vExcudingNodes.Clear();
            hoveredNode = null;
            hoveredPortGuid = -1;
            draggedOutputGuid = -1;
            draggedOutputTargetGuid = -1;
            draggedOutputReroutes.Clear();
            lastportConnectionPoints.Clear();
            m_portConnectionPoints.Clear();
            m_vConnectionsSets.Clear();

            isDoubleClick = false;

            PortUtil.Clear();

            dragOffset = null;
            if (m_SelectionCache != null) m_SelectionCache.Clear();

            m_preBoxSelection = null;
            if (m_culledNodes != null) m_culledNodes.Clear();
            m_vActioNodes.Clear();
            m_nodeSizes.Clear();

            m_pSrcPrefab = null;
        }
        //------------------------------------------------------
        public void NewGuide()
        {
            GuideGroup pGroup = GuideEditor.Instance.NewGuide();
            if (pGroup == null) return;
            LoadGroup(pGroup.Guid);
        }
        //------------------------------------------------------
        public void LoadGroup(int id, UnityEngine.Object srcPrefab = null)
        {
            GuideGroup pData;
            if(GuideSystem.getInstance().datas.TryGetValue(id, out pData))
                Load(pData, srcPrefab, true);
            else
                EditorUtility.DisplayDialog("提示", "不存在改ID(" + id + ") 的引导组", "请确认");
        }
        //------------------------------------------------------
        public void Load(GuideGroup pGroup, UnityEngine.Object srcPrefab = null, bool bPop = true)
        {
            if(bPop)
            {
                if (pGroup == null)
                {
                    EditorUtility.DisplayDialog("提示", "不是一个有效的AI脚本数据", "好的");
                    return;
                }

                if (m_pGroupData != null)
                {
                    if (EditorUtility.DisplayDialog("提示", "是否保存当前再加载？", "保存", "取消"))
                    {
                        Save();
                    }
                }
            }


            ClearTempData();

            m_pSrcPrefab = srcPrefab;
            m_pGroupData = pGroup;
            Reload(false);
        }
        //------------------------------------------------------
        void Reload(bool bClearTemp)
        {
            if(bClearTemp) ClearTempData();
            if (m_pGroupData != null)
            {
                m_pGroupData.Init();
            }
            if(m_pGroupData.vTriggers != null)
            {
                foreach(var db in m_pGroupData.vTriggers)
                {
                    GraphNode node = new GraphNode(m_pGroupData, db);
                    m_vActioNodes.Add(db.Guid, node);
                }
            }
            if (m_pGroupData.vExcudes != null)
            {
                foreach (var db in m_pGroupData.vExcudes)
                {
                    GraphNode node = new GraphNode(m_pGroupData, db);
                    m_vActioNodes.Add(db.Guid, node);
                }
            }
            if (m_pGroupData.vSteps != null)
            {
                foreach (var db in m_pGroupData.vSteps)
                {
                    GraphNode node = new GraphNode(m_pGroupData, db);
                    m_vActioNodes.Add(db.Guid, node);
                }
            }
            if (m_pGroupData.vOperates != null)
            {
                foreach (var db in m_pGroupData.vOperates)
                {
                    GraphNode node = new GraphNode(m_pGroupData, db);
                    m_vActioNodes.Add(db.Guid, node);
                }
            }

            //! step excude mode
            if (m_pGroupData.vSteps != null)
            {
                foreach (var db in m_pGroupData.vSteps)
                {
                    GraphNode node = m_vActioNodes[db.Guid];
                    GraphNode excudeNode = null;
                    if(db.pAutoExcudeNode != null && m_vActioNodes.TryGetValue(db.pAutoExcudeNode.Guid,  out excudeNode))
                    {
                        ExternPort exterPort = node.GetExternPort(10);
                        if(exterPort == null)
                        {
                            exterPort = new ExternPort();
                            exterPort.externID = 10;
                            node.vExternPorts.Add(exterPort);
                        }
                        exterPort.vLinks.Clear();
                        exterPort.vLinks.Add(excudeNode);
                    }
                    GraphNode breakNode = null;
                    if (db.pSuccessedListenerBreakNode != null && m_vActioNodes.TryGetValue(db.pSuccessedListenerBreakNode.Guid, out breakNode))
                    {
                        ExternPort exterPort = node.GetExternPort(20);
                        if (exterPort == null)
                        {
                            exterPort = new ExternPort();
                            exterPort.externID = 20;
                            node.vExternPorts.Add(exterPort);
                        }
                        exterPort.vLinks.Clear();
                        exterPort.vLinks.Add(breakNode);
                    }
                }
            }
            //节点连接关系设置
            foreach (var db in m_vActioNodes)
            {
                if(db.Value.bindNode is SeqNode)
                {
                    SeqNode seqNode = db.Value.bindNode as SeqNode;
                    foreach(var ops in seqNode.vOps)
                    {
                        GraphNode excudeNode = null;
                        if (m_vActioNodes.TryGetValue(ops.Guid, out excudeNode))
                        {
                            db.Value.Links.Add(excudeNode);
                        }
                    }
                    GraphNode next;
                    if (m_vActioNodes.TryGetValue(seqNode.nextGuid, out next))
                    {
                        db.Value.Links.Add(next);
                    }
                }
                else if(db.Value.bindNode is GuideOperate)
                {
                    GuideOperate guideOperate = db.Value.bindNode as GuideOperate;
                    GraphNode next;
                    if (m_vActioNodes.TryGetValue(guideOperate.NextNode, out next))
                    {
                        db.Value.Links.Add(next);
                    }
                }
            }

            ms_nPortGUID = 0;
            if(m_pGroupData.vPorts!=null)
            {
                foreach (var db in m_pGroupData.vPorts)
                {
                    ms_nPortGUID = Mathf.Max(ms_nPortGUID, db.guid);
                }
            }

            if (m_vActioNodes.Count>0)
            {
                panOffset = -m_vActioNodes.ElementAt(0).Value.GetPosition() - new Vector2(m_vActioNodes.ElementAt(0).Value.GetWidth(), m_vActioNodes.ElementAt(0).Value.GetHeight())/2;
            }
        }
        //------------------------------------------------------
        public int BuildGroupGUID()
        {
            int guid = 0;
            foreach (var db in GuideSystem.getInstance().datas)
            {
                guid = Mathf.Max(guid, db.Key);
            }
            guid++;
            return guid;
        }
        //------------------------------------------------------
        public int BuildNodeGUID()
        {
            if (m_pGroupData == null) return 0;
            int guide = m_pGroupData.BuildStepGUID();
            foreach (var db in m_vActioNodes)
            {
                guide = Mathf.Max(guide, db.Key);
            }
            return ++guide;
        }
        //------------------------------------------------------
        public static int BuildPortGUID()
        {
            return ++ms_nPortGUID;
        }
        //------------------------------------------------------
        public GraphNode CreateTriggerNode(GuideEditor.TriggerParam param)
        {
            if (m_pGroupData == null) return null;

            OnChangeData();

            int guid = BuildNodeGUID();
            if (guid == 0) return null;

            TriggerNode pNode = new TriggerNode();
            pNode.Guid = guid;
            pNode.type = param.Data.type;
            pNode.posX = (int)param.gridPos.x;
            pNode.posY = (int)param.gridPos.y;
            pNode.Name = param.Data.strShortName;

            GraphNode grap = new GraphNode(m_pGroupData, pNode);
            m_vActioNodes.Add(guid, grap);

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public GraphNode CreateExcudeNode(GuideEditor.ExcudeParam param)
        {
            if (m_pGroupData == null) return null;

            OnChangeData();

            int guid = BuildNodeGUID();
            if (guid == 0) return null;

            ExcudeNode pNode = new ExcudeNode();
            pNode.Guid = guid;
            pNode.type = param.Data.type;
            pNode.posX = (int)param.gridPos.x;
            pNode.posY = (int)param.gridPos.y;
            pNode.Name = param.Data.strShortName;

            GraphNode grap = new GraphNode(m_pGroupData, pNode);
            m_vActioNodes.Add(guid, grap);

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public GraphNode FindLastNodeAndNoNextLink()
        {
            for(int i = m_vActioNodes.Count-1; i >=0; --i)
            {
                var node = m_vActioNodes.ElementAt(i).Value;
                if (node.bLinkOut && node.linkOutPort.baseNode !=null && 
                    (node.linkOutPort.baseNode.Links == null || node.linkOutPort.baseNode.Links.Count <= 0))
                    return node;
            }
            return null;
        }
        //------------------------------------------------------
        public GraphNode CreateStepNode(GuideEditor.StepParam param)
        {
            if (m_pGroupData == null) return null;

            OnChangeData();

            int guid = BuildNodeGUID();
            if (guid == 0) return null;

            StepNode pNode = new StepNode();
            pNode.Guid = guid;
            pNode.type = param.Data.type;
            pNode.posX = (int)param.gridPos.x;
            pNode.posY = (int)param.gridPos.y;
            pNode.Name = param.Data.strShortName;

            GraphNode grap = new GraphNode(m_pGroupData, pNode);
            m_vActioNodes.Add(guid, grap);

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public GraphNode CreateOpNode(GuideEditor.OpParam param)
        {
            if (m_pGroupData == null) return null;

            OnChangeData();

            int guid = BuildNodeGUID();
            if (guid == 0) return null;

            GuideOperate pNode = new GuideOperate();
            pNode.Guid = guid;
            pNode.posX = (int)param.gridPos.x;
            pNode.posY = (int)param.gridPos.y;
            pNode.Name = guid.ToString();

            GraphNode grap = new GraphNode(m_pGroupData, pNode);
            m_vActioNodes.Add(guid, grap);

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public void Home()
        {
            zoom = 2;
            panOffset = Vector2.zero;
        }
        //------------------------------------------------------
        public void Repaint()
        {
            m_pEditor.Repaint();
        }
        //------------------------------------------------------
        public void RemoveSelectedNodes()
        {
            if(m_SelectionCache.Count>0)
            {
                if (!EditorUtility.DisplayDialog("提示", "是否确认删除所有选择的节点", "删除", "再想想"))
                {
                    return;
                }
                OnChangeData();
                foreach (GraphNode item in m_SelectionCache)
                {
                    m_vActioNodes.Remove(item.GetGUID());
                    foreach (var db in m_vActioNodes)
                    {
                        db.Value.Links.Remove(item);
                    }
                }
            }
            else if(m_pGroupData!=null)
            {
                if (!EditorUtility.DisplayDialog("提示", "是否确认删除该组引导", "删除", "再想想"))
                {
                    return;
                }
                GuideSystem.getInstance().datas.Remove(m_pGroupData.Guid);
                m_pGroupData = null;
                ClearTempData();
            }
            m_SelectionCache.Clear();
            m_preBoxSelection = null;
            m_bDeleteSelected = false;
        }
        //------------------------------------------------------
        public void DuplicateSelectedNodes()
        {
            //GraphNode[] newNodes = new GraphNode[m_SelectionCache.Count];
            //Dictionary<GraphNode, GraphNode> substitutes = new Dictionary<GraphNode, GraphNode>();
            //for (int i = 0; i < m_SelectionCache.Count; i++)
            //{
            //    if (m_SelectionCache[i].BindNode!=null)
            //    {
            //        GraphNode srcNode = m_SelectionCache[i];
            //        GraphNode newNode = graphEditor.CopyNode(srcNode);
            //        substitutes.Add(srcNode, newNode);
            //        newNode.position = srcNode.position + new Vector2(30, 30);
            //        newNodes[i] = newNode;
            //    }
            //}

            //// Walk through the selected nodes again, recreate connections, using the new nodes
            //for (int i = 0; i < Selection.objects.Length; i++)
            //{
            //    if (Selection.objects[i] is XNode.Node)
            //    {
            //        XNode.Node srcNode = Selection.objects[i] as XNode.Node;
            //        if (srcNode.graph != graph) continue; // ignore nodes selected in another graph
            //        foreach (XNode.NodePort port in srcNode.Ports)
            //        {
            //            for (int c = 0; c < port.ConnectionCount; c++)
            //            {
            //                XNode.NodePort inputPort = port.direction == XNode.NodePort.IO.Input ? port : port.GetConnection(c);
            //                XNode.NodePort outputPort = port.direction == XNode.NodePort.IO.Output ? port : port.GetConnection(c);

            //                XNode.Node newNodeIn, newNodeOut;
            //                if (substitutes.TryGetValue(inputPort.node, out newNodeIn) && substitutes.TryGetValue(outputPort.node, out newNodeOut))
            //                {
            //                    newNodeIn.UpdateStaticPorts();
            //                    newNodeOut.UpdateStaticPorts();
            //                    inputPort = newNodeIn.GetInputPort(inputPort.fieldName);
            //                    outputPort = newNodeOut.GetOutputPort(outputPort.fieldName);
            //                }
            //                if (!inputPort.IsConnectedTo(outputPort)) inputPort.Connect(outputPort);
            //            }
            //        }
            //    }
            //}
            //Selection.objects = newNodes;
        }
        //------------------------------------------------------
        public void RenameSelectedNode()
        {
            if (m_SelectionCache.Count == 1 && m_SelectionCache[0] is GraphNode)
            {
                GraphNode node = m_SelectionCache[0] as GraphNode;
                Vector2 size;
                if (nodeSizes.TryGetValue(node, out size))
                {
                    RenamePopup.Show(node.bindNode, node.bindNode.Name, size.x);
                }
                else
                {
                    RenamePopup.Show(node.bindNode, node.bindNode.Name);
                }
            }
            else
            {
                RenamePopup.Show(m_pGroupData, m_pGroupData.Name);
            }
        }
        //------------------------------------------------------
        public void ParseNode()
        {
            m_bParseDo = true;
        }
        //------------------------------------------------------
        void DoParse()
        {
            m_bParseDo = false;
            if (m_pGroupData == null) return;
            if (CopySelectionCathes == null || CopySelectionCathes.Count <= 0) return;
            try
            {
                Save();
                Vector2Int mouseOffset = new Vector2Int(20, -30);
                List<TriggerNode> vTriggerNodes = new List<TriggerNode>();
                List<ExcudeNode> vExcudeNodes = new List<ExcudeNode>();
                List<StepNode> vStepNodes = new List<StepNode>();
                List<GuideOperate> vOperateNodes = new List<GuideOperate>();
                int curMaxGUID = BuildNodeGUID()+1;
                Dictionary<int, BaseNode> vOldNewNodes = new Dictionary<int, BaseNode>();
                Dictionary<int, BaseNode> vNewNodes = new Dictionary<int, BaseNode>();
                Dictionary<int, ArgvPort> vOldNewArgvPorts = new Dictionary<int, ArgvPort>();
                foreach (var db in CopySelectionCathes)
                {
                    if(db.bindNode!=null)
                    {
                        BaseNode copyNode = (BaseNode)JsonUtility.FromJson(JsonUtility.ToJson(db.bindNode), db.bindNode.GetType());
                        copyNode.Guid = curMaxGUID++;
                        vOldNewNodes.Add(db.bindNode.Guid, copyNode);
                        vNewNodes[copyNode.Guid] = copyNode;
                        if (copyNode is TriggerNode)
                            vTriggerNodes.Add(copyNode as TriggerNode);
                        else if (copyNode is ExcudeNode)
                            vExcudeNodes.Add(copyNode as ExcudeNode);
                        else if (copyNode is GuideOperate)
                            vOperateNodes.Add(copyNode as GuideOperate);

                        if (copyNode is StepNode)
                            vStepNodes.Add(copyNode as StepNode);

                        List<ArgvPort> _CopyPorts = null;
                        List<ArgvPort> vPorts = db.bindNode.GetArgvPorts();
                        if(vPorts!=null)
                        {
                            _CopyPorts = new List<ArgvPort>();
                            for (int i = 0; i < vPorts.Count; ++i)
                            {
                                ArgvPort port= (ArgvPort)JsonUtility.FromJson(JsonUtility.ToJson(vPorts[i]), vPorts[i].GetType());
                                port.guid = BuildPortGUID();
                                _CopyPorts.Add(port);
                                vOldNewArgvPorts[vPorts[i].guid] = port;
                            }
                        }
                        copyNode.SetArgvPorts(_CopyPorts);
                    }
                }

                Dictionary<int, GraphNode> vNewGraphNodes = new Dictionary<int, GraphNode>();
                foreach (var db in vNewNodes)
                {
                    GraphNode node = new GraphNode(m_pGroupData, db.Value);
                    node.bindNode.posX += mouseOffset.x;
                    node.bindNode.posY += mouseOffset.y;
                    m_vActioNodes.Add(db.Value.Guid, node);
                    vNewGraphNodes[db.Value.Guid] = node;
                }

                foreach (var db in vNewNodes)
                {
                    if (db.Value is TriggerNode)
                    {
                        TriggerNode triNode = db.Value as TriggerNode;

                    }
                    else if (db.Value is ExcudeNode)
                    {
                        ExcudeNode stepNode = db.Value as ExcudeNode;

                    }
                    else if (db.Value is GuideOperate)
                    {
                        GuideOperate opNode = db.Value as GuideOperate;
                        opNode._Ports = new List<VarPort>();
                        if (opNode.Vars != null)
                        {
                            for(int i =0; i < opNode.Vars.Length; ++i)
                            {
                                if (opNode.Vars[i].dummyMaps != null) opNode.Vars[i].dummyMaps.Clear();
                                if (opNode.Vars[i].dummys == null) continue;
                                List<DummyPort> dummp = new List<DummyPort>();
                                for(int j = 0; j < opNode.Vars[i].dummys.Length; ++j)
                                {
                                    int guid = opNode.Vars[i].dummys[j].argvGuid;
                                    int nodeGuid = opNode.Vars[i].dummys[j].nodeGuid;
                                    if (vOldNewNodes.ContainsKey(nodeGuid) && vOldNewArgvPorts.ContainsKey(guid))
                                    {
                                        DummyPort dumPort = opNode.Vars[i].dummys[j];
                                        dumPort.nodeGuid = vOldNewNodes[nodeGuid].Guid;
                                        dumPort.argvGuid = vOldNewArgvPorts[guid].guid;
                                        dummp.Add(dumPort);
                                        opNode.Vars[i].AddDummyArgv(dumPort.nodeGuid, vOldNewArgvPorts[guid]);
                                    }
                                }
                                opNode.Vars[i].dummys = dummp.ToArray();
                                opNode._Ports.Add(opNode.Vars[i]);
                            }
                        }
                        opNode.pNext = null;
                        if (opNode.NextNode != 0 && vOldNewNodes.ContainsKey(opNode.NextNode))
                        {
                            opNode.pNext = vOldNewNodes[opNode.NextNode];
                            opNode.NextNode = vOldNewNodes[opNode.NextNode].Guid;
                        }
                        else
                            opNode.NextNode = -1;
                    }

                    if (db.Value is StepNode)
                    {
                        StepNode stepNode = db.Value as StepNode;
                        stepNode.pAutoExcudeNode = null;
                        if (vOldNewNodes.ContainsKey(stepNode.autoExcudeNodeGuid) && vOldNewNodes[stepNode.autoExcudeNodeGuid] is ExcudeNode)
                        {
                            stepNode.pAutoExcudeNode = vOldNewNodes[stepNode.autoExcudeNodeGuid] as ExcudeNode;
                            stepNode.autoExcudeNodeGuid = vOldNewNodes[stepNode.autoExcudeNodeGuid].Guid;
                        }
                        else
                            stepNode.autoExcudeNodeGuid = -1;

                        stepNode.pSuccessedListenerBreakNode = null;
                        if (vOldNewNodes.ContainsKey(stepNode.nSuccessedBreakSkipTo) && vOldNewNodes[stepNode.nSuccessedBreakSkipTo]!=null)
                        {
                            stepNode.pSuccessedListenerBreakNode = vOldNewNodes[stepNode.nSuccessedBreakSkipTo];
                            stepNode.nSuccessedBreakSkipTo = vOldNewNodes[stepNode.nSuccessedBreakSkipTo].Guid;
                        }
                        else
                            stepNode.nSuccessedBreakSkipTo = -1;
                    }
                    if (db.Value is SeqNode)
                    {
                        SeqNode stepNode = db.Value as SeqNode;
                        if (stepNode.nextGuid != 0)
                        {
                            stepNode.pNext = null;
                            if (vOldNewNodes.ContainsKey(stepNode.nextGuid) && vOldNewNodes[stepNode.nextGuid] is SeqNode)
                            {
                                stepNode.pNext = vOldNewNodes[stepNode.nextGuid] as SeqNode;
                                stepNode.nextGuid = vOldNewNodes[stepNode.nextGuid].Guid;
                            }
                            else
                                stepNode.nextGuid = -1;
                        }
                        else
                            stepNode.pNext = null;

                        stepNode.vOps = new List<GuideOperate>();
                        if (stepNode.Ops != null)
                        {
                            List<int> vOps = new List<int>();
                            for (int i = 0; i < stepNode.Ops.Length; ++i)
                            {
                                if (vOldNewNodes.ContainsKey(stepNode.Ops[i]) && vOldNewNodes[stepNode.Ops[i]] is GuideOperate)
                                {
                                    vOps.Add(vOldNewNodes[stepNode.Ops[i]].Guid);
                                    stepNode.vOps.Add(vOldNewNodes[stepNode.Ops[i]] as GuideOperate);
                                }
                            }
                            stepNode.Ops = vOps.ToArray();
                        }
                        else
                            stepNode.vOps = new List<GuideOperate>();
                    }
                }

                //! step excude mode
                if (vStepNodes != null)
                {
                    foreach (var db in vStepNodes)
                    {
                        GraphNode node = m_vActioNodes[db.Guid];
                        GraphNode excudeNode = null;
                        if (db.pAutoExcudeNode != null && m_vActioNodes.TryGetValue(db.pAutoExcudeNode.Guid, out excudeNode))
                        {
                            ExternPort externPort = node.GetExternPort(10);
                            if(externPort == null)
                            {
                                externPort = new ExternPort();
                                externPort.externID = 10;
                                node.vExternPorts.Add(externPort);
                            }
                            externPort.vLinks.Clear();
                            externPort.vLinks.Add(excudeNode);
                        }

                        if (db.pSuccessedListenerBreakNode != null && m_vActioNodes.TryGetValue(db.pSuccessedListenerBreakNode.Guid, out excudeNode))
                        {
                            ExternPort externPort = node.GetExternPort(20);
                            if (externPort == null)
                            {
                                externPort = new ExternPort();
                                externPort.externID = 20;
                                node.vExternPorts.Add(externPort);
                            }
                            externPort.vLinks.Clear();
                            externPort.vLinks.Add(excudeNode);
                        }
                    }
                }

                SelectonCache.Clear();
                //节点连接关系设置
                foreach (var db in vNewGraphNodes)
                {
                    if (db.Value.bindNode is GuideOperate)
                    {
                        GuideOperate guideOperate = db.Value.bindNode as GuideOperate;
                        GraphNode next;
                        if (m_vActioNodes.TryGetValue(guideOperate.NextNode, out next))
                        {
                            db.Value.Links.Add(next);
                        }
                    }
                    else if (db.Value.bindNode is SeqNode)
                    {
                        SeqNode seqNode = db.Value.bindNode as SeqNode;
                        if(seqNode.vOps!=null)
                        {
                            foreach (var ops in seqNode.vOps)
                            {
                                GraphNode excudeNode = null;
                                if (m_vActioNodes.TryGetValue(ops.Guid, out excudeNode))
                                {
                                    db.Value.Links.Add(excudeNode);
                                }
                            }
                        }

                        GraphNode next;
                        if (m_vActioNodes.TryGetValue(seqNode.nextGuid, out next))
                        {
                            db.Value.Links.Add(next);
                        }
                    }

                    SelectonCache.Add(db.Value);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.StackTrace);
            }
            CopySelectionCathes.Clear();
        }
        //------------------------------------------------------
        public void SelectNode(GraphNode node, bool add)
        {
            if (add)
            {
                m_SelectionCache.Add(node);
            }
            else
            {
                m_SelectionCache.Clear();
                m_SelectionCache.Add(node);
            }
     //       if(Event.current.shift)
     //           m_pInspector.Open(m_SelectionCache);
        }
        //------------------------------------------------------
        public void DeselectNode(GraphNode node)
        {
            bool bInclude = m_SelectionCache.Contains(node);
            m_SelectionCache.Remove(node);
       //     if(bInclude && m_SelectionCache.Count<=0)
      //      {
       //         m_pInspector.Close();
      //      }
        }
        //------------------------------------------------------
        bool IsHoveringTitle(GraphNode node, bool bSubLink = true)
        {
            Vector2 mousePos = Event.current.mousePosition;
            //Get node position
            Vector2 nodePos = GridToWindowPosition(node.GetPosition());
            if(bSubLink)
                nodePos.x += 32;
            float width;
            Vector2 size;
            if (nodeSizes.TryGetValue(node, out size)) width = size.x;
            else width = 200;
            Rect windowRect;
            if(bSubLink) windowRect = new Rect(nodePos, new Vector2((width-64) / zoom, 30 / zoom));
            else windowRect = new Rect(nodePos, new Vector2(width / zoom, 30 / zoom));
            return windowRect.Contains(mousePos);
        }
        //------------------------------------------------------
        public Vector2 WindowToGridPosition(Vector2 windowPosition)
        {
            return (windowPosition - (m_pEditor.position.size * 0.5f) - (panOffset / zoom)) * zoom;
        }
        //------------------------------------------------------
        public Vector2 GridToWindowPosition(Vector2 gridPosition)
        {
            return (m_pEditor.position.size * 0.5f) + (panOffset / zoom) + (gridPosition / zoom);
        }
        //------------------------------------------------------
        public Rect GridToWindowRectNoClipped(Rect gridRect)
        {
            gridRect.position = GridToWindowPositionNoClipped(gridRect.position);
            return gridRect;
        }
        //------------------------------------------------------
        public Rect GridToWindowRect(Rect gridRect)
        {
            gridRect.position = GridToWindowPosition(gridRect.position);
            gridRect.size /= zoom;
            return gridRect;
        }
        //------------------------------------------------------
        public Vector2 GridToWindowPositionNoClipped(Vector2 gridPosition)
        {
            Vector2 center = m_pEditor.position.size * 0.5f;
            // UI Sharpness complete fix - Round final offset not panOffset
            float xOffset = Mathf.Round(center.x * zoom + (panOffset.x + gridPosition.x));
            float yOffset = Mathf.Round(center.y * zoom + (panOffset.y + gridPosition.y));
            return new Vector2(xOffset, yOffset);
        }
    }
}
#endif