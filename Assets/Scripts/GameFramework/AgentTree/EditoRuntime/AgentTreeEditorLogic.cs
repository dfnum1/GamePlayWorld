/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	AgentTreeEditor
作    者:	HappLI
描    述:	可视化编程编辑器
*********************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeEditorLogic
    {
        public enum EActivityType{ Idle, HoldNode, DragNode, HoldGrid, DragGrid, DragLink }
        public static EActivityType currentActivity = EActivityType.Idle;

        private int topPadding { get { return 19/*isDocked() ? 19 : 22*/; } }
        private Func<bool> isDocked
        {
            get
            {
                if (_isDocked == null)
                {
                    BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                    MethodInfo isDockedMethod = typeof(AgentTreeEditor).GetProperty("docked", fullBinding).GetGetMethod(true);
                    _isDocked = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), this, isDockedMethod);
                }
                return _isDocked;
            }
        }
        private Func<bool> _isDocked;

        bool m_bDeleteSelected = false;
        public static bool isPanning { get; private set; }

        private IGraphNode hoveredNode = null;
        private int hoveredPortGuid = -1;
        private int draggedOutputGuid = -1;
        private int draggedInputGuid = -1;
        private int draggedOutputTargetGuid = -1;
        private List<Vector2> draggedOutputReroutes = new List<Vector2>();
        private ReroutePort hoveredReroute = new ReroutePort();
        private List<ReroutePort> selectedReroutes = new List<ReroutePort>();

        public HashSet<int> lastportConnectionPoints = new HashSet<int>();
        public Dictionary<int, Rect> portConnectionPoints { get { return m_portConnectionPoints; } }
        private Dictionary<int, Rect> m_portConnectionPoints = new Dictionary<int, Rect>();
        private HashSet<System.Int64> m_vConnectionsSets = new HashSet<System.Int64>();
        public bool IsDraggingOutPort { get { return draggedOutputGuid != -1; } }
        public bool IsDraggingInPort { get { return draggedInputGuid != -1; } }
        public bool IsHoveringPort { get { return hoveredPortGuid != -1; } }
        public bool IsHoveringNode { get { return hoveredNode !=null; } }
        public bool IsHoveringReroute { get { return hoveredReroute.port != null; } }

        private Vector2 dragBoxStart;
        private Rect selectionBox;
        private bool isDoubleClick = false;


        public Vector2 panOffset { get { return _panOffset; } set { _panOffset = value; Repaint(); } }
        private Vector2 _panOffset;
        public float zoom { get { return _zoom; } set { _zoom = Mathf.Clamp(value, AgentTreePreferences.GetSettings().minZoom, AgentTreePreferences.GetSettings().maxZoom); Repaint(); } }
        private float _zoom = 1;

        private Matrix4x4 prevGuiMatrix;

        Dictionary<int, double> m_vExcudingNodes = new Dictionary<int, double>();
        AgentTree m_pExcudingAT = null;
        float m_fCheckExcudingGap = 0;
        AgentTree m_pAT = null;
        AAgentTreeData m_pATData = null;
        public AAgentTreeData ATData
        {
            get { return m_pATData; }
        }
        UnityEngine.Object m_pSrcPrefab = null;
        Dictionary<int, Variable> m_vNewVariables = new Dictionary<int, Variable>();

        public static Vector2[] dragOffset;
        protected List<IGraphNode> m_SelectionCache;
        public List<IGraphNode> SelectonCache
        {
            get { return m_SelectionCache; }
        }

        public static int MAX_CATCH_DATASS = 10;
        public Stack<AgentTreeCoreData> m_vStackDatas = new Stack<AgentTreeCoreData>();

        public static AAgentTreeData pCopyATData = null;
        public static List<IGraphNode> CopySelectionCathes = null;

        private ReroutePort[] m_preBoxSelectionReroute;
        private IGraphNode[] m_preBoxSelection;

        private List<GraphNode> m_culledNodes;
        int m_nTaskEnterID = -100;
        List<Task> m_vEnterTasks = new List<Task>();
        Dictionary<int, GraphNode> m_vActioNodes = new Dictionary<int, GraphNode>();

        Dictionary<int, RefPortNode> m_vRefPorts = new Dictionary<int, RefPortNode>();
        Dictionary<long, TransferDot> m_vTransferDots = new Dictionary<long, TransferDot>();

        Dictionary<int, StructData> m_vVariableStructs = new Dictionary<int, StructData>();
        List<StructData> m_vStructDatas = new List<StructData>();
        public List<StructData> StrcutDatas
        {
            get { return m_vStructDatas; }
        }
        public Dictionary<int, StructData> VariableStrcuts
        {
            get { return m_vVariableStructs; }
        }
        public Dictionary<int, RefPortNode> RefPorts
        {
            get { return m_vRefPorts; }
        }
        public Dictionary<int, GraphNode> ActionNodes
        {
            get { return m_vActioNodes; }
        }
        public Dictionary<IGraphNode, Vector2> nodeSizes { get { return m_nodeSizes; } }
        private Dictionary<IGraphNode, Vector2> m_nodeSizes = new Dictionary<IGraphNode, Vector2>();

        AgentTreeEditor m_pEditor;
        public AgentTreeEditor Editor { get { return m_pEditor; } }
        AgentTreeStructInspector m_pStructInspector = new AgentTreeStructInspector();
        AgentTreeFinderInspector m_pFinderInspector = new AgentTreeFinderInspector();
        AgentTreeReplaceSearcher m_pSearcher = new AgentTreeReplaceSearcher();
        AgentTreeSearcherAPI m_pSearcherAPIs = new AgentTreeSearcherAPI();

        public bool m_bOpenFinder = false;
        public bool m_bOpenStruct = false;

        struct TempCopyParseData
        {
            public int type;//0-nodelink 1-portlink
            public int oldData;
        }
        public void OnEnable(AgentTreeEditor pEditor)
        {
            m_pEditor = pEditor;
            m_bDeleteSelected = false;
        }
        //------------------------------------------------------
        public void OnDisable()
        {
            if (AgentTreePreferences.GetSettings().autoSave)
                Save(false);
            m_pExcudingAT = null;
            m_bDeleteSelected = false;
        }
        //------------------------------------------------------
        public void ShowNotification(string strMsg, float time)
        {
            if (m_pEditor == null) return;
            m_pEditor.ShowNotification(new GUIContent(strMsg), time);
        }
        //------------------------------------------------------
        public void OnGUI()
        {
            Event evt = Event.current;
            Matrix4x4 m = GUI.matrix;

            Controls();
            OnDraw();

            if (m_pATData == null) return;
            m_pFinderInspector.OnDraw(m_pEditor.position);
            m_pStructInspector.OnDraw(m_pEditor.position);
            m_pSearcher.OnDraw();
            m_pSearcherAPIs.OnDraw();

            GUI.matrix = m;

            if(Event.current.type == EventType.MouseUp)
            {
                if (!m_pSearcher.IsMouseIn(Event.current.mousePosition))
                    m_pSearcher.Close();
                if (!m_pSearcherAPIs.IsMouseIn(Event.current.mousePosition))
                    m_pSearcherAPIs.Close();
            }
        }
        //------------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {
            if(m_SelectionCache!=null && m_SelectionCache.Count>0)
            {
                foreach (var db in m_SelectionCache)
                {
                    db.OnSceneGUI(sceneView);
                }
            }
        }
        //------------------------------------------------------
        public void OpenSearcher(Vector2 mousePos, System.Type hashClassType = null)
        {
            m_pSearcher.Open(this, hashClassType, new Rect(mousePos.x, mousePos.y, 350,400));
        }
        //------------------------------------------------------
        public void OpenSearcher(GraphNode pNode, Vector2 mousePos)
        {
            m_pSearcher.Open(pNode,new Rect(mousePos.x, mousePos.y, 350, 400));
        }
        //------------------------------------------------------
        public void CloseSearcher()
        {
            m_pSearcher.Close();
            m_pSearcherAPIs.Close();
        }
        //------------------------------------------------------
        public AgentTree GetAgentTree()
        {
            return m_pAT;
        }
        //------------------------------------------------------
        void OnChangeData()
        {
            RecodeDo();
        }
        //------------------------------------------------------
        void RecodeDo()
        {
            if (m_pExcudingAT != null) return;
            if (m_pATData == null) return;
            if (m_vStackDatas.Count >= 20)
                m_vStackDatas.Pop();
            Save(false);
            m_vStackDatas.Push(m_pATData.Data.Clone());
        }
        //------------------------------------------------------
        public void UnRedo()
        {
            if (m_pExcudingAT != null) return;
            if (m_pATData == null) return;
            if (m_vStackDatas.Count > 0)
            {
                AgentTreeCoreData pData = m_vStackDatas.Pop();
                m_pATData.Data = pData;
                Reload(true);
            }
        }
        //------------------------------------------------------
        public GraphNode GetGraphNode(int guid)
        {
            GraphNode pNode;
            if (m_vActioNodes.TryGetValue(guid, out pNode))
                return pNode;
            return null;
        }
        //------------------------------------------------------
        public void Save(bool bSaveAs = false)
        {
            CheckExcuding();
            if (m_pATData == null || m_pExcudingAT != null) return;
            if(Application.isPlaying)
            {
                m_pEditor.ShowNotification(new GUIContent("运行模式下不支持保存"));
                return;
            }
            HashSet<Variable> variabels = new HashSet<Variable>();
            HashSet<Task> vTasks = new HashSet<Task>();
            m_vEnterTasks.Clear();
            List<ExcudeNode> vNodes = new List<ExcudeNode>();
            List<GraphNode> vTaskNode = new List<GraphNode>();
               
            foreach (var db in m_vActioNodes)
            {
                if (db.Value.BindNode != null)
                {
                    db.Value.BindNode.bSaved = false;
                    List<Variable> vars = db.Value.BindNode.GetAllVariable();
                    foreach (var vardb in vars)
                    {
                        if(vardb!=null)
                            variabels.Add(vardb);
                    }

                    List<int> NextActionIDs = new List<int>();
                    List<ExcudeNode> NextActions = new List<ExcudeNode>();
                    for (int i = 0; i < db.Value.NextLinks.Count; ++i)
                    {
                        if (db.Value.NextLinks[i].BindNode != null && db.Value.NextLinks[i].BindNode != db.Value.BindNode)
                        {
                            NextActionIDs.Add(db.Value.NextLinks[i].BindNode.GUID);
                            NextActions.Add(db.Value.NextLinks[i].BindNode);
                        }
                    }
                    db.Value.BindNode.nextActionsID = NextActionIDs.ToArray();
                    db.Value.BindNode.nextActions = NextActions.ToArray();

                    if (db.Value.BindNode.outArgvs != null)
                    {
                        for (int i = 0; i < db.Value.BindNode.outArgvs.Length; ++i)
                        {
                            if (db.Value.BindNode.outArgvs[i].variable != null)
                            {
                                VariableDelegate varDelegate = db.Value.BindNode.outArgvs[i].variable as VariableDelegate;
                                if(varDelegate!=null)
                                {
                                    for(int j =0; j < varDelegate.OutLink.linkNodes.Count; ++j)
                                    {
                                        if(varDelegate.OutLink.linkNodes[j].BindNode.GetExcudeHash() == (int)EActionType.DelegateCallback)
                                        {
                                            varDelegate.OutLink.linkNodes[j].BindNode.SetCustomValue(db.Value.BindNode.GUID);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (db.Value.BindNode is ActionNode)
                    {
                        ActionNode bindAction = db.Value.BindNode as ActionNode;
                        if (bindAction.condition != null)
                        {
                            for (int i = 0; i < db.Value.OutConditionLinks.Count; ++i)
                            {
                                if (db.Value.OutConditionLinks[i].partalNode != null)
                                {
                                    if (db.Value.OutConditionLinks[i].partalNode.Actions == null)
                                        db.Value.OutConditionLinks[i].partalNode.Actions = new List<ExcudeNode>();
                                    db.Value.OutConditionLinks[i].partalNode.Actions.Clear();
                                    if (db.Value.OutConditionLinks[i].linkNodes != null)
                                    {
                                        for (int j = 0; j < db.Value.OutConditionLinks[i].linkNodes.Count; ++j)
                                        {
                                            if (db.Value.OutConditionLinks[i].linkNodes[j] != db.Value && db.Value.OutConditionLinks[i].linkNodes[j] != null)
                                            {
                                                db.Value.OutConditionLinks[i].partalNode.Actions.Add(db.Value.OutConditionLinks[i].linkNodes[j].BindNode);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    

                    db.Value.BindNode.Save();
                    vNodes.Add(db.Value.BindNode);
                }
                else if(db.Value.EnterTask != null)
                {
                    if(db.Value.NextLinks.Count>0 && db.Value.NextLinks[0].BindNode!=null)
                    {
                        db.Value.EnterTask.EnterNode.Action = db.Value.NextLinks[0].BindNode;
                        db.Value.EnterTask.EnterNode.ActionGUID = db.Value.NextLinks[0].BindNode.GUID;
                    }
                    else
                    {
                        db.Value.EnterTask.EnterNode.Action = null;
                        db.Value.EnterTask.EnterNode.ActionGUID = -1;
                    }
                    if (db.Value.EnterTask.EnterNode.Param!=null && db.Value.EnterTask.EnterNode.Param.variable != null)
                        variabels.Add(db.Value.EnterTask.EnterNode.Param.variable);
                    if(!m_vEnterTasks.Contains(db.Value.EnterTask))
                        m_vEnterTasks.Add(db.Value.EnterTask);
                }
            }

            m_vVariableStructs.Clear();
            for (int i = 0; i < m_vStructDatas.Count; ++i)
            {
                if (m_vStructDatas[i].runtimeVars != null)
                {
                    if (m_vStructDatas[i].variables == null)
                        m_vStructDatas[i].variables = new List<int>();
                    m_vStructDatas[i].variables.Clear();
                    for (int j = 0; j < m_vStructDatas[i].runtimeVars.Count; ++j)
                    {
                        m_vStructDatas[i].runtimeVars[j].Save();
                        m_vStructDatas[i].runtimeVars[j].SetFlag(EFlag.Locked, true);
                        variabels.Add(m_vStructDatas[i].runtimeVars[j]);

                        m_vStructDatas[i].variables.Add(m_vStructDatas[i].runtimeVars[j].GUID);

                        m_vVariableStructs[m_vStructDatas[i].runtimeVars[j].GUID] = m_vStructDatas[i];
                    }
                }
            }


            VariableSerializes Locals = new VariableSerializes();
            int maxGuid = 0;
            foreach (var db in variabels)
            {
                db.Save();
                Locals.AddVariable(db);
                maxGuid = Mathf.Max(maxGuid, db.GUID);
            }
            AgentTreeManager.AdjustMaxGUID(++maxGuid);
            foreach (var db in m_vEnterTasks)
            {
                db.EnterNode.Save();
            }
            List<APINode> APINodes = new List<APINode>();
            List<ActionNode> ActionNodes = new List<ActionNode>();
            for (int i = 0; i < vNodes.Count; ++i)
            {
                if (vNodes[i] is ActionNode) ActionNodes.Add((ActionNode)vNodes[i]);
                else if (vNodes[i] is APINode) APINodes.Add((APINode)vNodes[i]);
            }


            if (bSaveAs)
            {
                string strPath = EditorUtility.SaveFilePanel("另存为", Application.dataPath, "NEW", "asset");
                strPath = strPath.Replace("\\", "/");
                strPath = strPath.Replace(Application.dataPath, "Assets");
                if (string.IsNullOrEmpty(strPath))
                {
                    EditorUtility.DisplayDialog("提示", "请选择有效的目录", "好的");
                    return;
                }
                string strName = System.IO.Path.GetFileNameWithoutExtension(strPath);
                if (string.IsNullOrEmpty(strName) || m_pATData.name.CompareTo(strName) == 0)
                {
                    EditorUtility.DisplayDialog("提示", "无效的名字", "好的");
                    return;
                }

                AAgentTreeData newAT = AAgentTreeData.CreateInstance();
                if (newAT.Data == null) newAT.Data = new AgentTreeCoreData();
                newAT.name = strName;
                newAT.Data.Locals = Locals;
                newAT.Data.Tasks = new List<Task>(m_vEnterTasks.ToArray());
                newAT.Data.vNodes = ActionNodes;
                newAT.Data.vATApis = APINodes;
                newAT.Data.StructDatas = new List<StructData>();
                for (int i = 0; i < m_vStructDatas.Count; ++i)
                {
                    if (m_vStructDatas[i].variables.Count <= 0) continue;
                    StructData stData = new StructData();
                    stData.structName = m_vStructDatas[i].structName;
                    stData.variables = new List<int>(m_vStructDatas[i].variables.ToArray());
                    newAT.Data.StructDatas.Add(stData);
                }
                newAT.Data.RefPorts = new List<RefPort>();
                foreach (var db in m_vRefPorts)
                {
                    db.Value.BindNode.Save(variabels);
                    if(db.Value.BindNode.IsValid())
                        newAT.Data.RefPorts.Add(db.Value.BindNode);
                }

                newAT.Data.transferDots = new List<TransferDot>();
                foreach(var db in m_vTransferDots)
                {
                    db.Value.key = db.Key;
                    newAT.Data.transferDots.Add(db.Value);
                }

                AgentTreeEditorUtils.SetAssetIcon(newAT, AgentTreePreferences.GetSettings()._iconTexture);
                EditorUtility.SetDirty(newAT);
            }
            else
            {
                if (m_pATData.Data == null)
                    m_pATData.Data = new AgentTreeCoreData();

                m_pATData.Data.Locals = Locals;
                m_pATData.Data.vNodes = ActionNodes;
                m_pATData.Data.vATApis = APINodes;
                m_pATData.Data.Tasks = new List<Task>(m_vEnterTasks.ToArray());
                m_pATData.Data.StructDatas = new List<StructData>();
                for(int i = 0; i < m_vStructDatas.Count; ++i)
                {
                    if (m_vStructDatas[i].variables.Count <= 0) continue;
                    StructData stData = new StructData();
                    stData.structName = m_vStructDatas[i].structName;
                    stData.variables = new List<int>(m_vStructDatas[i].variables.ToArray());
                    m_pATData.Data.StructDatas.Add(stData);
                }
                m_pATData.Data.RefPorts = new List<RefPort>();
                foreach (var db in m_vRefPorts)
                {
                    db.Value.BindNode.Save(variabels);
                    if (db.Value.BindNode.IsValid())
                        m_pATData.Data.RefPorts.Add(db.Value.BindNode);
                }
                m_pATData.Data.transferDots = new List<TransferDot>();
                foreach (var db in m_vTransferDots)
                {
                    db.Value.key = db.Key;
                    m_pATData.Data.transferDots.Add(db.Value);
                }
                EditorUtility.SetDirty(m_pATData);
            }

            if(m_pSrcPrefab != null)
            {
                EditorUtility.SetDirty(m_pSrcPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        //------------------------------------------------------
        public void PopMemberVariables(System.Object port)
        {
            if (m_pATData == null) return;
            MousePosAndData menuPop = (MousePosAndData)port;
            ArgvPort argvPort = menuPop.bindData as ArgvPort;
            if (argvPort == null || argvPort.variable == null) return;
            GenericMenu contextMenu = new GenericMenu();
            foreach(var db in m_vStructDatas)
            {
                for(int i =0; i < db.runtimeVars.Count; ++i)
                {
                    if (db.runtimeVars[i].GetType() != argvPort.variable.GetType())
                        continue;
                    StrcutPopMenuData structData = new StrcutPopMenuData();
                    structData.port = menuPop.bindData;
                    structData.structData = db;
                    structData.structMemeber = db.runtimeVars[i];
                    contextMenu.AddItem(new GUIContent(db.structName + "/" + db.runtimeVars[i].strName), false, OnPortSelectMemeber, structData);
                }
            }
            contextMenu.DropDown(new Rect(menuPop.mousePos, Vector2.zero));
        }
        //------------------------------------------------------
        public void OpenAPISearcher(Vector2 mouse, System.Object data)
        {
            m_pSearcherAPIs.Open(this, new Rect(mouse.x, mouse.y, 350, 400), data);
        }
        //------------------------------------------------------
        public void OnPortSelectMemeber(System.Object data)
        {
            StrcutPopMenuData menuData = (StrcutPopMenuData)data;
            if (menuData.port == null) return;
            ArgvPort argvPort = menuData.port as ArgvPort;
            if (argvPort == null) return;
            argvPort.ClearConnections();
            argvPort.port.variable = menuData.structMemeber;
            argvPort.port.guid = menuData.structMemeber.GUID;
        }
        //------------------------------------------------------
        public void UpdateStrcuts()
        {
            m_vVariableStructs.Clear();
            for (int i = 0; i < m_vStructDatas.Count; ++i)
            {
                if (m_vStructDatas[i].runtimeVars != null)
                {
                    for (int j = 0; j < m_vStructDatas[i].runtimeVars.Count; ++j)
                    {
                        m_vVariableStructs[m_vStructDatas[i].runtimeVars[j].GUID] = m_vStructDatas[i];
                    }
                }
            }
        }
        //------------------------------------------------------
        public void Update(float fTime)
        {
            m_fCheckExcudingGap -= fTime;
            if (m_fCheckExcudingGap <=0)
            {
                CheckExcuding();
            }
            if(m_bOpenFinder)
            {
                m_pFinderInspector.Open(this);
                m_bOpenFinder = false;
            }
            m_pFinderInspector.Update(m_pEditor.position, fTime);
            if(m_bOpenStruct)
            {
                m_pStructInspector.Open(this);
                m_bOpenStruct = false;
            }
            m_pStructInspector.Update(m_pEditor.position, fTime);
            m_pSearcher.Update(fTime);
            m_pSearcherAPIs.Update(fTime);
            if (m_bDeleteSelected)
            {
                RemoveSelectedNodes();
                m_bDeleteSelected = false;
            }
            double time = EditorApplication.timeSinceStartup;
            foreach (var db in m_vExcudingNodes)
            {
                if(time-db.Value > 0.5f)
                {
                    m_vExcudingNodes.Remove(db.Key);
                    break;
                }
            }
        }
        //------------------------------------------------------
        void CheckExcuding()
        {
            m_fCheckExcudingGap = 5;

            if (m_pExcudingAT != null && AgentTreeManager.getInstance().Agents != null && !AgentTreeManager.getInstance().Agents.ContainsKey(m_pExcudingAT.Guid))
            {
                m_pExcudingAT = null;
            }
        }
        //------------------------------------------------------
        public void OnExcudeAction(AgentTreeTask pTask, ActionNode pNode)
        {
            if (m_pATData == null) return;
            if (pNode ==null || pTask == null || pTask.taskData == null) return;
            m_pExcudingAT = pTask.pAT;
            Task task = pTask.taskData;
            if (m_pATData.Data.Tasks.Contains(task) )
            {
                m_vExcudingNodes[pNode.GUID] = EditorApplication.timeSinceStartup;
                if (pTask.taskData.EnterNode != null)
                    m_vExcudingNodes[pTask.taskData.EnterNode.GUID] = EditorApplication.timeSinceStartup;
            }
        }
        //------------------------------------------------------
        private bool IsExcudingNode(int guid)
        {
            if (m_pATData == null) return false;
//             if(m_pExcudingAT == null)
//             {
//                 foreach (var db in AgentTreeManager.getInstance().Agents)
//                 {
//                     if(db.Value.pSoAT == m_pATData || db.Value.CoreData == m_pATData.Data)
//                     {
//                         m_pExcudingAT = db.Value;
//                         break;
//                     }
//                 }
//             }
            if (m_pExcudingAT == null) return false;
            if (m_vExcudingNodes.ContainsKey(guid))
            {
               // m_vExcudingNodes[guid] = EditorApplication.timeSinceStartup;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void Controls()
        {
            m_pEditor.wantsMouseMove = true;
            Event e = Event.current;
            bool bInSecondView = m_pSearcher.IsMouseIn(e.mousePosition) || m_pSearcherAPIs.IsMouseIn(e.mousePosition) || m_pFinderInspector.IsMouseIn(e.mousePosition) || m_pStructInspector.IsMouseIn(e.mousePosition);
            switch (e.type)
            {
                case EventType.MouseMove:
                    break;
                case EventType.ScrollWheel:
                    if(!bInSecondView)
                    {
                        float oldZoom = zoom;
                        if (e.delta.y > 0) zoom += 0.1f * zoom;
                        else zoom -= 0.1f * zoom;
                        if (AgentTreePreferences.GetSettings().zoomToMouse) panOffset += (1 - oldZoom / zoom) * (WindowToGridPosition(e.mousePosition) + panOffset);
                    }
                    break;
                case EventType.MouseDrag:
                //    m_pSearcher.Close();
                    if(!bInSecondView)
                    {
                        if (e.button == 0)
                        {
                            IPortNode draggedOutputPort = PortUtil.GetPort(draggedOutputGuid);
                            if (draggedOutputPort != null)
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
                                bool gridSnap = AgentTreePreferences.GetSettings().gridSnap;
                                if (e.control) gridSnap = !gridSnap;

                                Vector2 mousePos = WindowToGridPosition(e.mousePosition);
                                // Move selected nodes with offset
                                for (int i = 0; i < m_SelectionCache.Count; ++i)
                                {
                                    IGraphNode pNode = m_SelectionCache[i];
                                    if (pNode != null)
                                    {
                                        IGraphNode node = m_SelectionCache[i];
                                        Vector2 initial = node.GetPosition();
                                        node.SetPosition(mousePos + dragOffset[i]);
                                        if (gridSnap)
                                        {
                                            Vector2 pos = node.GetPosition();
                                            pos.x = (Mathf.Round((pos.x + 8) / 16) * 16) - 8;
                                            pos.y = (Mathf.Round((pos.y + 8) / 16) * 16) - 8;
                                        }

                                        // Offset portConnectionPoints instantly if a node is dragged so they aren't delayed by a frame.
                                        if(node is GraphNode)
                                        {
                                            GraphNode graphNode = node as GraphNode;
                                            Vector2 offset = node.GetPosition() - initial;
                                            if (offset.sqrMagnitude > 0)
                                            {
                                                foreach (ArgvPort output in graphNode.Outputs)
                                                {
                                                    Rect rect;
                                                    if (portConnectionPoints.TryGetValue(output.GetGUID(), out rect))
                                                    {
                                                        rect.position += offset;
                                                        portConnectionPoints[output.GetGUID()] = rect;
                                                    }
                                                }

                                                foreach (ArgvPort input in graphNode.Inputs)
                                                {
                                                    Rect rect;
                                                    if (portConnectionPoints.TryGetValue(input.GetGUID(), out rect))
                                                    {
                                                        rect.position += offset;
                                                        portConnectionPoints[input.GetGUID()] = rect;
                                                    }
                                                }
                                            }
                                        }
                                        
                                    }
                                }
                                // Move selected reroutes with offset
                                for (int i = 0; i < selectedReroutes.Count; i++)
                                {
                                    Vector2 pos = mousePos + dragOffset[Selection.objects.Length + i];
                                    if (gridSnap)
                                    {
                                        pos.x = (Mathf.Round(pos.x / 16) * 16);
                                        pos.y = (Mathf.Round(pos.y / 16) * 16);
                                    }
                                    selectedReroutes[i].SetPoint(pos);
                                }
                                Repaint();
                            }
                            else if (currentActivity == EActivityType.HoldGrid)
                            {
                                currentActivity = EActivityType.DragGrid;
                                if (!Event.current.control) m_SelectionCache.Clear();
                                m_preBoxSelection = m_SelectionCache.ToArray();
                                m_preBoxSelectionReroute = selectedReroutes.ToArray();
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
                    }
                    break;
                case EventType.MouseDown:
               //     if (!m_pSearcher.IsMouseIn(e.mousePosition))
               //         m_pSearcher.Close();
                    Repaint();
                    if (!bInSecondView)
                    {
                        if (e.button == 0)
                        {
                            draggedOutputReroutes.Clear();

                            IPortNode hoveredPort = PortUtil.GetPort(hoveredPortGuid);
                            if (hoveredPort != null)
                            {
                                if (hoveredPort.IsOutput())
                                {
                                    draggedInputGuid = -1;
                                    draggedOutputGuid = hoveredPortGuid;
                                }
                                else if(hoveredPort.IsInput())
                                {
                                    draggedOutputGuid = -1;
                                    draggedInputGuid = hoveredPortGuid;
                                }
                                //   else
                                //   {
                                //  hoveredPort.VerifyConnections();
                                //  if (hoveredPort.IsConnected())
                                //   {
                                //     GraphNode node = hoveredPort.GetNode();
                                //     APortNode output = hoveredPort.Connection();
                                //     int outputConnectionIndex = output.GetConnectionIndex(hoveredPort);
                                //     draggedOutputReroutes = output.GetReroutePoints(outputConnectionIndex);
                                //      hoveredPort.Disconnect(output);
                                //      draggedOutputGuid = output.GetGUID();
                                //        draggedOutputTargetGuid = hoveredPortGuid;
                                //       m_pEditor.OnUpdateNode(node);
                                //   }
                                //  }
                            }
                            else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                            {
                                // If mousedown on node header, select or deselect
                                if (!m_SelectionCache.Contains(hoveredNode))
                                {
                                    SelectNode(hoveredNode, e.control || e.shift);
                                    if (!e.control && !e.shift) selectedReroutes.Clear();
                                }
                                else if (e.control || e.shift) DeselectNode(hoveredNode);

                                // Cache double click state, but only act on it in MouseUp - Except ClickCount only works in mouseDown.
                                isDoubleClick = (e.clickCount == 2);

                                e.Use();
                                currentActivity = EActivityType.HoldNode;
                            }
                            else if (IsHoveringReroute)
                            {
                                // If reroute isn't selected
                                if (!selectedReroutes.Contains(hoveredReroute))
                                {
                                    // Add it
                                    if (e.control || e.shift) selectedReroutes.Add(hoveredReroute);
                                    // Select it
                                    else
                                    {
                                        selectedReroutes = new List<ReroutePort>() { hoveredReroute };
                                    }

                                }
                                // Deselect
                                else if (e.control || e.shift) selectedReroutes.Remove(hoveredReroute);
                                e.Use();
                                currentActivity = EActivityType.HoldNode;
                            }
                            // If mousedown on grid background, deselect all
                            else if (!IsHoveringNode)
                            {
                                currentActivity = EActivityType.HoldGrid;
                                if (!e.control && !e.shift)
                                {
                                    selectedReroutes.Clear();
                                }
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        if (!bInSecondView)
                        {
                            //Port drag release
                            if (IsDraggingOutPort)
                            {
                                //If connection is valid, save it
                                IPortNode draggedOutputTarget = PortUtil.GetPort(draggedOutputTargetGuid);
                                IPortNode draggedOutput = PortUtil.GetPort(draggedOutputGuid);
                                if (draggedOutputTarget != null && draggedOutput != null)
                                {
                                    if (draggedOutputTarget.GetNode() != draggedOutput.GetNode())
                                    {
                                        if (draggedOutput is ArgvPort && draggedOutputTarget is ArgvPort)
                                        {
                                            ArgvPort source = draggedOutput as ArgvPort;
                                            ArgvPort target = draggedOutputTarget as ArgvPort;
                                            if (target.variable.GetType() == source.variable.GetType() && target.direction != source.direction)
                                            {
                                                bool bCanLink = true;
                                                if ((source.variable is VariableUser))
                                                {
                                                    VariableUser srcClass = source.variable as VariableUser;
                                                    VariableUser dstClass = target.variable as VariableUser;
                                                    if (dstClass.hashCode != 0 && srcClass.hashCode != 0 && srcClass.hashCode != dstClass.hashCode)
                                                    {
                                                        Type srcType = null;
                                                        Type dstType = null;
                                                        if (AgentTreeUtl.ExportClasses.TryGetValue(srcClass.hashCode, out srcType) &&
                                                           AgentTreeUtl.ExportClasses.TryGetValue(dstClass.hashCode, out dstType))
                                                        {
                                                            ATExportNodeAttrData exportNode;
                                                            if (!dstType.IsInterface && !srcType.IsInterface &&
                                                                AgentTreeEditorUtils.HasCommonInheritBase(srcType, dstType) &&
                                                                AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)EActionType.CastVariable, out exportNode))
                                                            {
                                                                Rect srcRect, dstRect;
                                                                if (m_portConnectionPoints.TryGetValue(source.GetGUID(), out srcRect) && m_portConnectionPoints.TryGetValue(target.GetGUID(), out dstRect))
                                                                {
                                                                    AgentTreeEditor.ActionParam param = new AgentTreeEditor.ActionParam();
                                                                    param.mousePos = GridToWindowPosition((srcRect.position + dstRect.position) / 2);
                                                                    param.gridPos = (srcRect.position + dstRect.position) / 2;
                                                                    param.Data = exportNode;
                                                                    GraphNode pCastNode = CreateActionNode(param);
                                                                    if (pCastNode != null)
                                                                    {
                                                                        if (pCastNode.BindNode != null)
                                                                        {
                                                                            pCastNode.BindNode.ClearArgv();
                                                                            pCastNode.BindNode.AddInArgv(source.variable);
                                                                            pCastNode.BindNode.AddOutArgv(target.variable);
                                                                            pCastNode.BindNode.Save();

                                                                            pCastNode.NextLinks.Add(target.GetNode());
                                                                            target.GetNode().PrevLinks.Add(pCastNode);

                                                                            source.GetNode().NextLinks.Add(pCastNode);
                                                                            pCastNode.PrevLinks.Add(source.GetNode());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (dstType.IsInterface && !srcType.IsInterface && AgentTreeEditorUtils.HasCommonInheritBase(srcType, dstType))
                                                            bCanLink = true;
                                                        else
                                                            bCanLink = false;
                                                    }
                                                }
                                                if (bCanLink)
                                                {
                                                    ArgvPort outPort = null;
                                                    ArgvPort inPort = null;
                                                    if (target.direction == EPortIO.Out)
                                                    {
                                                        outPort = target;
                                                        inPort = source;
                                                    }
                                                    else
                                                    {
                                                        outPort = source;
                                                        inPort = target;
                                                    }

                                                    if (!outPort.variable.IsFlag(EFlag.Const))
                                                    {
                                                        if (outPort.baseNode.EnterTask != null)
                                                        {
                                                            OnChangeData();
                                                            if (inPort.variable == outPort.variable)
                                                            {
                                                                inPort.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(outPort.variable.GetType());
                                                                inPort.variable.SetClassHashCode(outPort.variable.GetClassHashCode());
                                                                inPort.variable = inPort.port.variable;
                                                            }
                                                            else
                                                                inPort.variable = outPort.variable;
                                                        }
                                                        else
                                                        {
                                                            OnChangeData();
                                                            if (inPort.port.variable == outPort.variable)
                                                            {
                                                                inPort.port.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(outPort.variable.GetType());
                                                                inPort.port.variable.SetClassHashCode(outPort.variable.GetClassHashCode());
                                                                inPort.variable = inPort.port.variable;
                                                            }
                                                            else
                                                            {
                                                                if (inPort.port.HasDummy(outPort.baseNode.GetGUID()))
                                                                    inPort.port.RemoveDummy(outPort.baseNode.GetGUID());
                                                                else
                                                                    inPort.port.AddDummy(outPort.baseNode.GetGUID(), outPort.variable);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (draggedOutput is LinkPort && draggedOutputTarget is LinkPort)
                                        {
                                            LinkPort output = draggedOutput as LinkPort;
                                            LinkPort input = draggedOutputTarget as LinkPort;
                                            if (input.direction != output.direction)
                                            {
                                                OnChangeData();
                                                GraphNode outputNode = output.GetNode();
                                                if (output is ConditionLinkPort)
                                                {
                                                    ConditionLinkPort condPort = output as ConditionLinkPort;
                                                    if (!condPort.linkNodes.Contains(draggedOutputTarget.GetNode()))
                                                        condPort.linkNodes.Add(draggedOutputTarget.GetNode());
                                                    else
                                                        condPort.linkNodes.Remove(draggedOutputTarget.GetNode());
                                                }
                                                else if (output is DelegateLinkPort)
                                                {
                                                    DelegateLinkPort condPort = output as DelegateLinkPort;
                                                    if (!condPort.linkNodes.Contains(draggedOutputTarget.GetNode()))
                                                    {
                                                        if (draggedOutputTarget.GetNode().BindNode.GetExcudeHash() == (int)EActionType.DelegateCallback)
                                                        {
                                                            draggedOutputTarget.GetNode().BindNode.SetCustomValue(condPort.GetNode().GetGUID());
                                                        }
                                                        condPort.linkNodes.Add(draggedOutputTarget.GetNode());
                                                    }
                                                    else
                                                    {
                                                        if (draggedOutputTarget.GetNode().BindNode.GetExcudeHash() == (int)EActionType.DelegateCallback)
                                                            draggedOutputTarget.GetNode().BindNode.SetCustomValue(0);
                                                        condPort.linkNodes.Remove(draggedOutputTarget.GetNode());
                                                    }
                                                }
                                                else
                                                {
                                                    if (outputNode.EnterTask != null)
                                                    {
                                                        GraphNode inputNode = draggedOutputTarget.GetNode();
                                                        outputNode.NextLinks.Clear();
                                                        outputNode.NextLinks.Add(inputNode);
                                                        inputNode.PrevLinks.Add(outputNode);
                                                    }
                                                    else
                                                    {
                                                        GraphNode inputNode = draggedOutputTarget.GetNode();
                                                        if (outputNode.NextLinks.Contains(inputNode))
                                                        {
                                                            outputNode.NextLinks.Remove(inputNode);
                                                            inputNode.PrevLinks.Remove(outputNode);
                                                        }
                                                        else
                                                        {
                                                            outputNode.NextLinks.Add(inputNode);
                                                            inputNode.PrevLinks.Add(outputNode);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //GraphNode node = draggedOutputTarget.GetNode();
                                    //if (graph.nodes.Count != 0) draggedOutput.Connect(draggedOutputTarget);

                                    //// ConnectionIndex can be -1 if the connection is removed instantly after creation
                                    //int connectionIndex = draggedOutput.GetConnectionIndex(draggedOutputTarget);
                                    //if (connectionIndex != -1)
                                    //{
                                    //    draggedOutput.GetReroutePoints(connectionIndex).AddRange(draggedOutputReroutes);
                                    //    if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(node);
                                    //    EditorUtility.SetDirty(graph);
                                    //}
                                }
                                else
                                {
                                    if(draggedOutput!=null && draggedOutput is ArgvPort)
                                    {
                                        ArgvPort outPort = draggedOutput as ArgvPort;
                                        if(outPort.variable is VariableUser)
                                        {
                                            int hashCode = ((VariableUser)outPort.variable).GetClassHashCode();
                                            if(AgentTreeUtl.ExportClasses.TryGetValue(hashCode, out var classType))
                                            {
                                                m_pEditor.ShowClassMethod(e.mousePosition, outPort, classType);
                                            }
                                        }
                                    }
                                }
                                //Release dragged connection
                                draggedOutputGuid = -1;
                                draggedInputGuid = -1;
                                draggedOutputTargetGuid = -1;
                                if (m_pATData != null) EditorUtility.SetDirty(m_pATData);
                                if (AgentTreePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                            }
                            else if(IsDraggingInPort)
                            {
                                if (!bInSecondView)
                                {
                                    if(hoveredNode!=null && hoveredNode is RefPortNode)
                                    {
                                        RefPortNode refNode = hoveredNode as RefPortNode;
                                        ArgvPort draggedInput = PortUtil.GetPort(draggedInputGuid) as ArgvPort;
                                        if(draggedInput!=null)
                                        {
                                            if(draggedInput.port.pRefPort!=null)
                                            {
                                                draggedInput.port.pRefPort = null;
                                                draggedInput.port.refPortID = -1;
                                            }
                                            else if (refNode.BindNode.GetVariable() != null && draggedInput.port.variable != null && draggedInput.port.variable.GetType() == refNode.BindNode.GetVariable().GetType())
                                            {
                                                draggedInput.port.refPortID = refNode.BindNode.id;
                                                draggedInput.port.pRefPort = refNode.BindNode;
                                            }
                                        }
              
                                    }
                                    else
                                    {
                                        IPortNode draggedInput = PortUtil.GetPort(draggedInputGuid);
                                        if (draggedInput != null)
                                        {
                                            ArgvPort tempPort = draggedInput as ArgvPort;
                                            if(tempPort!=null)
                                            {
                                                if(tempPort.portalNode != null)
                                                {
                                                    tempPort.port.refPortID = -1;
                                                    tempPort.port.pRefPort = null;
                                                }
                                                else
                                                    m_pEditor.ShowRefPortContextMenu(draggedInput);
                                            }
                                        }
                                    }

                                    draggedInputGuid = -1;
                                }
                            }
                            else if (currentActivity == EActivityType.DragNode)
                            {
                                if (m_pATData != null) EditorUtility.SetDirty(m_pATData);
                                if (AgentTreePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                            }
                            else if (!IsHoveringNode)
                            {
                                // If click outside node, release field focus
                                if (!isPanning)
                                {
                                    //    EditorGUI.FocusTextInControl(null);
                                    //     EditorGUIUtility.editingTextField = false;
                                }
                                if (AgentTreePreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                            }

                            // If click node header, select it.
                            if (currentActivity == EActivityType.HoldNode && !(e.control || e.shift))
                            {
                                selectedReroutes.Clear();
                                SelectNode(hoveredNode, false);

                                // Double click to center node
                                if (isDoubleClick)
                                {
                                    CenterView(hoveredNode);
                                }
                            }

                            // If click reroute, select it.
                            if (IsHoveringReroute && !(e.control || e.shift))
                            {
                                selectedReroutes = new List<ReroutePort>() { hoveredReroute };
                            }
                        }

                        Repaint();
                        currentActivity = EActivityType.Idle;
                    }
                    else if (e.button == 1 || e.button == 2)
                    {
                        if (!isPanning)
                        {
                            if (IsDraggingOutPort)
                            {
                                draggedOutputReroutes.Add(WindowToGridPosition(e.mousePosition));
                            }
                            else if (currentActivity == EActivityType.DragNode && Selection.activeObject == null && selectedReroutes.Count == 1)
                            {
                                selectedReroutes[0].InsertPoint(selectedReroutes[0].GetPoint());
                                selectedReroutes[0] = new ReroutePort(selectedReroutes[0].port, selectedReroutes[0].connectionIndex, selectedReroutes[0].pointIndex + 1);
                            }
                            else if (IsHoveringReroute)
                            {
                                if(!bInSecondView)
                                    m_pEditor.ShowRerouteContextMenu(hoveredReroute);
                            }
                            else if (IsHoveringPort)
                            {
                                if (!bInSecondView)
                                    m_pEditor.ShowPortContextMenu(PortUtil.GetPort(hoveredPortGuid));
                            }
                            else if (IsHoveringNode && IsHoveringTitle(hoveredNode))
                            {
                                if (!m_SelectionCache.Contains(hoveredNode)) SelectNode(hoveredNode, false);
                                if(e.button == 1) m_pEditor.FuncContextMenu(e.mousePosition);
                                else m_pEditor.BaseFuncContextMenu(e.mousePosition);
                                e.Use(); // Fixes copy/paste context menu appearing in Unity 5.6.6f2 - doesn't occur in 2018.3.2f1 Probably needs to be used in other places.
                            }
                            else if (!IsHoveringNode)
                            {
                                if (!bInSecondView)
                                {
                                    if (e.button == 1) m_pEditor.FuncContextMenu(e.mousePosition);
                                    else m_pEditor.BaseFuncContextMenu(e.mousePosition);
                                }
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
                        m_pSearcherAPIs.Close();
                        m_pSearcher.Close();
                        m_SelectionCache.Clear();
                        m_pFinderInspector.Close();
                        m_pStructInspector.Close();
                    }
                    else if(e.keyCode == KeyCode.F2)
                    {
                        m_bOpenFinder = true;
                    }
                    else if (e.keyCode == KeyCode.F3)
                    {
                        m_bOpenStruct = true;
                    }
                    else if(e.keyCode == KeyCode.Z && e.control)
                    {
                        this.UnRedo();
                        e.Use();
                    }
                    if (e.keyCode == KeyCode.Delete)
                    {
                        m_bDeleteSelected = true;// RemoveSelectedNodes();
                    }
                    //                     if (IsMac())
                    //                     {
                    //                         if (e.keyCode == KeyCode.Return) RenameSelectedNode();
                    //                     }
                    //                     else
                    //                     {
                    //                         if (e.keyCode == KeyCode.F2) RenameSelectedNode();
                    //                     }
                    break;
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                    //                     if (e.commandName == "SoftDelete")
                    //                     {
                    //                         if (e.type == EventType.ExecuteCommand) RemoveSelectedNodes();
                    //                         e.Use();
                    //                     }
                    if (IsMac() && e.commandName == "Delete")
                    {
                        if (e.type == EventType.ExecuteCommand) m_bDeleteSelected = true;// RemoveSelectedNodes();
                        e.Use();
                    }
//                     else if (e.commandName == "Duplicate")
//                     {
//                         if (e.type == EventType.ExecuteCommand) DuplicateSelectedNodes();
//                         e.Use();
//                     }
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
        public void CenterView(IGraphNode pNode)
        {
            Vector2 nodeDimension = nodeSizes.ContainsKey(pNode) ? nodeSizes[pNode] / 2 : Vector2.zero;
            panOffset = -pNode.GetPosition() - nodeDimension;
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
        private void RecalculateDragOffsets(Event current)
        {
            dragOffset = new Vector2[m_SelectionCache.Count + selectedReroutes.Count];
            // Selected nodes
            for (int i = 0; i < m_SelectionCache.Count; i++)
            {
                IGraphNode grap = m_SelectionCache[i];
                if (grap != null)
                {
                    dragOffset[i] = grap.GetPosition() - WindowToGridPosition(current.mousePosition);
                }
            }

            // Selected reroutes
            for (int i = 0; i < selectedReroutes.Count; i++)
            {
                dragOffset[m_SelectionCache.Count + i] = selectedReroutes[i].GetPoint() - WindowToGridPosition(current.mousePosition);
            }
        }
        //------------------------------------------------------
        public void ClearTempData()
        {
            m_vExcudingNodes.Clear();
            hoveredNode = null;
            hoveredPortGuid = -1;
            draggedOutputGuid = -1;
            draggedInputGuid = -1;
            draggedOutputTargetGuid = -1;
            draggedOutputReroutes.Clear();
            selectedReroutes.Clear();
            lastportConnectionPoints.Clear();
            m_portConnectionPoints.Clear();
            m_vConnectionsSets.Clear();

            isDoubleClick = false;

            m_vNewVariables.Clear();
            PortUtil.Clear();

            dragOffset = null;
            if (m_SelectionCache != null) m_SelectionCache.Clear();

            pCopyATData = null;

            m_preBoxSelectionReroute = null;
            m_preBoxSelection = null;
            if (m_culledNodes != null) m_culledNodes.Clear();
            m_nTaskEnterID = -100;
            m_vEnterTasks.Clear();
            m_vActioNodes.Clear();
            m_nodeSizes.Clear();
            m_vStructDatas.Clear();
            m_vVariableStructs.Clear();

            m_pExcudingAT = null;
            m_pSrcPrefab = null;
        }
        //------------------------------------------------------
        public void LoadAT(string strPath, UnityEngine.Object srcPrefab = null)
        {
            AAgentTreeData pAT = AAgentTreeData.CreateInstance();

            LoadAT(pAT, srcPrefab);
        }
        //------------------------------------------------------
        public void LoadAT(AgentTreeCoreData pData, UnityEngine.Object srcPrefab = null)
        {
            AAgentTreeData pAT = AAgentTreeData.CreateInstance();
            pAT.hideFlags |= HideFlags.DontSave;
            pAT.Data = pData;

            LoadAT(pAT, srcPrefab);
        }
        //------------------------------------------------------
        public void LoadAT(AAgentTreeData pAT, UnityEngine.Object srcPrefab = null, bool bPop = true)
        {
            AgentTreeEditorUtils.SetAssetIcon(pAT, AgentTreePreferences.GetSettings()._iconTexture);
            if (bPop)
            {
                if (pAT == null)
                {
                    EditorUtility.DisplayDialog("提示", "不是一个有效的图形脚本数据", "好的");
                    return;
                }

                if (m_pATData != null)
                {
                    if (EditorUtility.DisplayDialog("提示", "是否保存当前再加载？", "保存", "取消"))
                    {
                        Save();
                    }
                }
            }


            ClearTempData();

            m_pSrcPrefab = srcPrefab;
            m_pATData = pAT;
            Reload(false);
        }
        //------------------------------------------------------
        void Reload(bool bClearTemp)
        {
            if(bClearTemp) ClearTempData();
            Dictionary<int ,AgentTree> vAgents = AgentTreeManager.getInstance().Agents;
            if (m_pAT != null)
            {
                if (!vAgents.ContainsKey(m_pAT.Guid))
                {
                    m_pAT.Clear();
                }
                m_pAT = null;
            }
            foreach (var db in vAgents)
            {
                if(db.Value.CoreData == m_pATData.Data)
                {
                    m_pAT = db.Value;
                    break;
                }
            }
            bool bNew = false;
            if (m_pAT == null)
            {
                bNew = true;
                m_pAT = new AgentTree();
            }
            if (m_pATData.Data != null && m_pATData.Data.Tasks != null)
            {
                for (int i = 0; i < m_pATData.Data.Tasks.Count; ++i)
                {
                    if(m_vEnterTasks.Contains(m_pATData.Data.Tasks[i])) continue;
                    m_vEnterTasks.Add(m_pATData.Data.Tasks[i]);
                }
                for (int i = 0; i < m_pATData.Data.vNodes.Count; ++i)
                {
                    GraphNode grap = new GraphNode(m_pEditor, m_pATData, m_pATData.Data.vNodes[i]);
                    grap.BindNode = m_pATData.Data.vNodes[i];
                    m_vActioNodes.Add(m_pATData.Data.vNodes[i].GUID, grap);
                }
                for (int i = 0; i < m_pATData.Data.vATApis.Count; ++i)
                {
                    GraphNode grap = new GraphNode(m_pEditor, m_pATData, m_pATData.Data.vATApis[i]);
                    grap.BindNode = m_pATData.Data.vATApis[i];
                    m_vActioNodes.Add(m_pATData.Data.vATApis[i].GUID, grap);
                }
            }
            m_pAT.onGetVariable = GetNewVariable;
            if(bNew) m_pAT.Init(null, m_pATData);
            foreach (var db in m_vActioNodes)
            {
                db.Value.Init();
            }

            foreach (var db in m_vActioNodes)
            {
                if (db.Value.BindNode == null) continue;

                if(db.Value.BindNode.inArgvs != null)
                {
                    for(int i =0; i < db.Value.BindNode.inArgvs.Length; ++i)
                    {
                        if(db.Value.BindNode.inArgvs[i].variable !=null)
                        {
                            VariableDelegate varDelegate = db.Value.BindNode.inArgvs[i].variable as VariableDelegate;
                            if(varDelegate!=null)
                            {
                                varDelegate.OutLink.ClearConnections();
                                if(varDelegate.Actions != null)
                                {
                                    for (int j = 0; j < varDelegate.Actions.Length; ++j)
                                    {
                                        GraphNode callAct;
                                        if (m_vActioNodes.TryGetValue(varDelegate.Actions[j], out callAct) && varDelegate.OutLink.linkNodes.IndexOf(callAct) < 0)
                                        {
                                            if(callAct.BindNode.GetExcudeHash() == (int)EActionType.DelegateCallback)
                                            {
                                                callAct.BindNode.SetCustomValue(db.Value.BindNode.GUID);
                                            }
                                            varDelegate.OutLink.linkNodes.Add(callAct);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (db.Value.BindNode.nextActionsID != null)
                {
                    for (int i = 0; i < db.Value.BindNode.nextActionsID.Length; ++i)
                    {
                        GraphNode link = null;
                        if (m_vActioNodes.TryGetValue(db.Value.BindNode.nextActionsID[i], out link))
                        {
                            db.Value.NextLinks.Add(link);
                            link.PrevLinks.Add(db.Value);
                        }
                    }
                }
                if(db.Value.BindNode is ActionNode)
                {
                    ActionNode actionNode = db.Value.BindNode as ActionNode;
                    if (actionNode.condition != null && actionNode.condition.portals != null)
                    {
                        for (int i = 0; i < actionNode.condition.portals.Count; ++i)
                        {
                            ConditionLinkPort link = new ConditionLinkPort();
                            link.baseNode = db.Value;
                            link.partalNode = actionNode.condition.portals[i];
                            link.index = i + 1;
                            link.direction = EPortIO.Out;
                            if (link.partalNode.Actions != null)
                            {
                                for (int j = 0; j < link.partalNode.Actions.Count; ++j)
                                {
                                    GraphNode pPorNode;
                                    if (m_vActioNodes.TryGetValue(link.partalNode.Actions[j].GUID, out pPorNode) && pPorNode != null)
                                    {
                                        link.linkNodes.Add(pPorNode);
                                    }
                                }
                            }
                            db.Value.OutConditionLinks.Add(link);
                        }
                    }
                }


            }

            for (int i = 0; i < m_vEnterTasks.Count; ++i)
            {
                GraphNode grap = new GraphNode(m_pEditor, m_pATData, null);
                grap.bEnterRoot = true;
                grap.EnterTask = m_vEnterTasks[i];
                grap.BindNode = null;
                grap.TaskID = m_nTaskEnterID--;
                m_vEnterTasks[i].EnterNode.pEditor = m_pEditor;
                GraphNode link = null;
                if (m_vActioNodes.TryGetValue(m_vEnterTasks[i].EnterNode.ActionGUID, out link))
                {
                    grap.NextLinks.Add(link);
                    link.PrevLinks.Add(grap);
                }
                m_vActioNodes.Add(grap.TaskID, grap);
            }

            m_vStructDatas.Clear();
            if (m_pATData.Data == null)
                m_pATData.Data = new AgentTreeCoreData();
            if(m_pATData.Data.StructDatas != null)
            {
                for (int i = 0; i < m_pATData.Data.StructDatas.Count; ++i)
                {
                    if (m_pATData.Data.StructDatas[i].variables == null || m_pATData.Data.StructDatas[i].variables.Count <= 0)
                        continue;
                    StructData stData = new StructData();
                    stData.structName = m_pATData.Data.StructDatas[i].structName;
                    stData.variables = new List<int>(m_pATData.Data.StructDatas[i].variables.ToArray());
                    stData.runtimeVars = new List<Variable>();
                    for (int j = 0; j < stData.variables.Count; ++j)
                    {
                        Variable var = m_pAT.GetVariable<Variable>(stData.variables[j]);
                        if (var != null)
                        {
                            var.SetFlag(EFlag.Locked, true);
                            stData.runtimeVars.Add(var);
                            m_vVariableStructs[stData.variables[j]] = stData;
                        }
                    }
                    if (stData.runtimeVars.Count > 0)
                        m_vStructDatas.Add(stData);
                }
            }
            

            m_vRefPorts.Clear();
            if(m_pATData.Data.RefPorts!=null)
            {
                for (int i = 0; i < m_pATData.Data.RefPorts.Count; ++i)
                {
                    m_pATData.Data.RefPorts[i].Init(m_pAT);
                    
                    m_vRefPorts.Add(m_pATData.Data.RefPorts[i].id, new RefPortNode(m_pATData, m_pATData.Data.RefPorts[i]));
                }
            }

            m_vTransferDots.Clear();
            if(m_pATData.Data.transferDots!=null)
            {
                for (int i = 0; i < m_pATData.Data.transferDots.Count; ++i)
                {
                    var dot = m_pATData.Data.transferDots[i];
                    m_vTransferDots.Add(dot.key, dot);
                }
            }
        }
        //------------------------------------------------------
        public void AdjustVariableGUID()
        {
            if (m_pATData == null) return;

            int maxGuid = 0;
            foreach (var db in m_vActioNodes)
            {
                if (db.Value.BindNode != null)
                {
                    List<Variable> vList = db.Value.BindNode.GetAllVariable();
                    for(int i = 0; i < vList.Count; ++i)
                        maxGuid = Math.Max(maxGuid, vList[i].GUID);
                }
                if(db.Value.EnterTask != null && db.Value.EnterTask.EnterNode.Param != null && db.Value.EnterTask.EnterNode.Param.variable!=null)
                    maxGuid = Math.Max(maxGuid, db.Value.EnterTask.EnterNode.Param.variable.GUID);
            }
            foreach (var db in m_vStructDatas)
            {
                for(int i =0; i < db.runtimeVars.Count; ++i)
                    maxGuid = Math.Max(maxGuid, db.runtimeVars[i].GUID);
            }
            AgentTreeManager.AdjustMaxGUID(++maxGuid, true);
        }
        //------------------------------------------------------
        public void NewAT()
        {
            if (m_pATData != null)
            {
                if (EditorUtility.DisplayDialog("提示", "是否保存当前再加载？", "保存", "取消"))
                {
                    Save();
                }
            }
            ClearTempData();
            m_pATData = null;
            CheckATData();
        }
        //------------------------------------------------------
        bool CheckATData()
        {
            if (m_pATData == null)
            {
                Dictionary<int, AgentTree> vAgents = AgentTreeManager.getInstance().Agents;
                if (m_pAT != null)
                {
                    if (!vAgents.ContainsKey(m_pAT.Guid))
                    {
                        m_pAT.Clear();
                    }
                    m_pAT = null;
                }
                bool bNewAt = false;
                if (m_pAT == null)
                {
                    m_pAT = new AgentTree();
                    bNewAt = true;
                }
                string strPath = EditorUtility.SaveFilePanel("创建AT", Application.dataPath, "NEW", "asset");
                strPath = strPath.Replace("\\", "/");
                strPath = strPath.Replace(Application.dataPath, "Assets");
                if (string.IsNullOrEmpty(strPath))
                {
                    EditorUtility.DisplayDialog("提示", "请选择有效的目录", "好的");
                    return false;
                }
                string strName = System.IO.Path.GetFileNameWithoutExtension(strPath);
                if (string.IsNullOrEmpty(strName))
                {
                    EditorUtility.DisplayDialog("提示", "无效的名字", "好的");
                    return false;
                }

                m_pATData = AAgentTreeData.CreateInstance();
                if (m_pATData == null)
                    return false;
                m_pATData.name = strName;
                if (m_pATData.Data == null)
                    m_pATData.Data = new AgentTreeCoreData();
                m_pATData.Data.Locals = new VariableSerializes();
                m_pATData.Data.Tasks = new List<Task>();

                m_pATData.Data.bEnable = true;
                if(bNewAt) m_pAT.Init(null, m_pATData);
                UnityEditor.AssetDatabase.CreateAsset(m_pATData, strPath);
                AgentTreeEditorUtils.SetAssetIcon(m_pATData, AgentTreePreferences.GetSettings()._iconTexture);

                return true;
            }
            return true;
        }
        //------------------------------------------------------
        public void CreateRefPortNode(ArgvPort port, Variable variable)
        {
            int maxId = 0;
            foreach(var db in m_vRefPorts)
            {
                maxId = Mathf.Max(maxId, db.Key);
            }
            Rect fromRect;
            if (!m_portConnectionPoints.TryGetValue(port.GetGUID(), out fromRect))
            {
                fromRect = port.rect;
            }

            RefPort refPort = new RefPort();
            refPort.id = ++maxId;
            refPort.rect = fromRect;
            refPort.rect.position -= new Vector2(110,0);
            refPort.rect.width = 100;
            refPort.rect.height = 80;
            refPort.SetVariable(variable);
            refPort.varGuid = variable.GUID;

            port.port.refPortID = refPort.id;
            port.port.pRefPort = refPort;
            RefPortNode refPortNode = new RefPortNode(m_pATData, refPort);
            m_vRefPorts.Add(refPort.id, refPortNode);
        }
        //------------------------------------------------------
        public GraphNode CreateActionNode(AgentTreeEditor.ActionParam param)
        {
            if (!CheckATData()) return null;

            if (m_pATData.Data == null)
                m_pATData.Data = new AgentTreeCoreData();

            int guid= -1;
            foreach(var db in m_vActioNodes)
            {
                guid = Mathf.Max(db.Key, guid);
            }
            guid++;

            OnChangeData();

            AdjustVariableGUID();

            ExcudeNode pNode = null;
            if (param.Data is ATAPINodeAttrData)
            {
                APINode pActionNode = new APINode();
                pActionNode.pEditor = m_pEditor;
                pActionNode.excudeType = param.Data.actionID;
                pNode = pActionNode;
            }
            else if (param.Data is ATExportNodeAttrData)
            {
                ActionNode pActionNode = new ActionNode();
                pActionNode.pEditor = m_pEditor;
                pActionNode.actionType = (EActionType)param.Data.actionID;
                pNode = pActionNode;
            }
            if (pNode == null) return null;
            pNode.pEditor = m_pEditor;
            pNode.strName = param.Data.DisplayName;
            pNode.GUID = guid;
            pNode.rect = new Rect(param.gridPos.x, param.gridPos.y, 200, 10);

            GraphNode grap = new GraphNode(m_pEditor, m_pATData, pNode);
            grap.BindNode = pNode;
            m_vActioNodes.Add(guid, grap);
            m_pAT.onGetVariable = GetNewVariable;

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public GraphNode CreateAPINode(AgentTreeEditor.APIParam param)
        {
            if (!CheckATData()) return null;

            if (m_pATData.Data == null)
                m_pATData.Data = new AgentTreeCoreData();

            int guid = -1;
            foreach (var db in m_vActioNodes)
            {
                guid = Mathf.Max(db.Key, guid);
            }
            guid++;

            OnChangeData();

            AdjustVariableGUID();

            APINode pNode = new APINode();
            pNode.excudeType = 0;
            pNode.GUID = guid;
            pNode.rect = new Rect(param.gridPos.x, param.gridPos.y, 200, 10);

            GraphNode grap = new GraphNode(m_pEditor, m_pATData, pNode);
            grap.BindNode = pNode;
            m_vActioNodes.Add(guid, grap);
            m_pAT.onGetVariable = GetNewVariable;

            this.Repaint();
            Debug.ClearDeveloperConsole();
            return grap;
        }
        //------------------------------------------------------
        public GraphNode CreateTaskNode(ETaskType type, Vector2 gridPos)
        {
            if (!CheckATData()) return null;
            if(type != ETaskType.Custom)
            {
                if (type <= ETaskType.Exit)
                {
                    for (int i = 0; i < m_vEnterTasks.Count; ++i)
                    {
                        if (m_vEnterTasks[i].type == type) return null;
                    }
                }
            }
            OnChangeData();

            AdjustVariableGUID();

            Task task = new Task();
            task.type = type;
            task.EnterNode = new EnterNode();
            m_vEnterTasks.Add(task);

            GraphNode grap = new GraphNode(m_pEditor, m_pATData, null);
            grap.bEnterRoot = true;
            grap.EnterTask = task;
            grap.BindNode = null;
            grap.TaskID = m_nTaskEnterID--;
            task.EnterNode.pEditor = m_pEditor;
            task.EnterNode.rect = new Rect(gridPos.x, gridPos.y, 100, 100);
            m_vActioNodes.Add(grap.TaskID, grap);

            return grap;
        }
        //------------------------------------------------------
        public void OnNewVariable(Variable pVar)
        {
            m_vNewVariables[pVar.GUID] = pVar;
            AgentTreeManager.AdjustMaxGUID(pVar.GUID);
        }
        //------------------------------------------------------
        Variable GetNewVariable(int guid)
        {
            Variable pOut;
            if (m_vNewVariables.TryGetValue(guid, out pOut))
                return pOut;
            return null;
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
            if(!EditorUtility.DisplayDialog("提示", "是否确认删除所有选择的节点", "删除", "再想想"))
            {
                return;
            }
            OnChangeData();
            // We need to delete reroutes starting at the highest point index to avoid shifting indices
            selectedReroutes = selectedReroutes.OrderByDescending(x => x.pointIndex).ToList();
            for (int i = 0; i < selectedReroutes.Count; i++)
            {
                selectedReroutes[i].RemovePoint();
            }
            selectedReroutes.Clear();
            foreach (IGraphNode item in m_SelectionCache)
            {
                if(item is GraphNode)
                {
                    GraphNode graphNode = item as GraphNode;
                    m_vActioNodes.Remove(item.GetGUID());
                    foreach (var db in m_vActioNodes)
                    {
                        db.Value.NextLinks.Remove(graphNode);
                        graphNode.PrevLinks.Remove(db.Value);
                        for (int i = 0; i < db.Value.OutConditionLinks.Count; ++i)
                        {
                            db.Value.OutConditionLinks[i].linkNodes.Remove(graphNode);
                        }
                        for (int i = 0; i < db.Value.OutDelegateLinks.Count; ++i)
                        {
                            db.Value.OutDelegateLinks[i].linkNodes.Remove(graphNode);
                        }
                    }
                }
                else if (item is RefPortNode)
                {
                    RefPortNode graphNode = item as RefPortNode;
                    m_vRefPorts.Remove(item.GetGUID());
                    foreach(var db in PortUtil.allPortPositions)
                    {
                        if(db.Value is ArgvPort)
                        {
                            ArgvPort argvPort = db.Value as ArgvPort;
                            if(argvPort != null && argvPort.port.pRefPort == graphNode.BindNode)
                            {
                                argvPort.port.pRefPort = null;
                                argvPort.port.refPortID = -1;
                            }
                        }
                    }
                }
            }
            m_SelectionCache.Clear();
            m_preBoxSelection = null;
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
                    RenamePopup.Show(node.BindNode, size.x);
                }
                else
                {
                    RenamePopup.Show(node.BindNode);
                }
            }
        }
        //------------------------------------------------------
        public void ParseNode()
        {
            if (CopySelectionCathes == null || CopySelectionCathes.Count <= 0) return;
            AdjustVariableGUID();
            List<GraphNode> vNews = new List<GraphNode>();


            Dictionary<int, int> vOldNodeLink = new Dictionary<int, int>();
            Dictionary<int, int> vOldPortLink = new Dictionary<int, int>();
            for(int i = 0; i < CopySelectionCathes.Count; ++i)
            {
                if(CopySelectionCathes[i] is GraphNode)
                {
                    GraphNode copyNode = CopySelectionCathes[i] as GraphNode;
                    if (copyNode.EnterTask != null)
                    {
                        if (copyNode.EnterTask.type >= ETaskType.Tick)
                        {
                            GraphNode pGrah = CreateTaskNode(copyNode.EnterTask.type, copyNode.GetPosition() + new Vector2(20, -30));
                            if (pGrah != null)
                            {
                                pGrah.EnterTask.Copy(copyNode.EnterTask, false);
                                pGrah.SetPosition(copyNode.GetPosition() + new Vector2(20, -30));
                                pGrah.EnterTask.EnterNode.Param = null;
                                pGrah.EnterTask.EnterNode.Action = null;
                                pGrah.EnterTask.EnterNode.ActionGUID = -1;
                                if (copyNode.EnterTask.EnterNode.Param != null &&
                                    copyNode.EnterTask.EnterNode.Param.variable != null)
                                {
                                    pGrah.EnterTask.EnterNode.Param = new Port(null);
                                    pGrah.EnterTask.EnterNode.Param.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(copyNode.EnterTask.EnterNode.Param.variable.GetType());
                                }
                                vNews.Add(pGrah);
                            }
                        }
                    }
                    else if (copyNode.BindNode != null)
                    {
                        ATEditorAttrData attrNode = copyNode.BindNode.GetEditAttrData<ATEditorAttrData>();
                        if (attrNode != null)
                        {
                            GraphNode pGrah = CreateActionNode(new AgentTreeEditor.ActionParam()
                            {
                                gridPos = copyNode.GetPosition() + new Vector2(20, -30),
                                Data = attrNode
                            });
                            if (pGrah != null)
                            {
                                ActionNode srcCopyAction = null;
                                if (copyNode.BindNode is ActionNode)
                                {
                                    srcCopyAction = copyNode.BindNode as ActionNode;
                                }

                                ActionNode actionNode = null;
                                pGrah.BindNode.Copy(copyNode.BindNode, false);
                                pGrah.BindNode.nextActions = null;
                                pGrah.BindNode.nextActionsID = null;
                                pGrah.BindNode.inArgvs = null;
                                pGrah.BindNode.outArgvs = null;
                                pGrah.BindNode.ClearArgv();
                                if (pGrah.BindNode is ActionNode)
                                {
                                    actionNode = (pGrah.BindNode as ActionNode);
                                    actionNode.condition = null;
                                }
                                pGrah.SetPosition(copyNode.GetPosition() + new Vector2(20, -30));

                                if (copyNode.BindNode.inArgvs != null)
                                {
                                    for (int a = 0; a < copyNode.BindNode.GetInArgvCount(); ++a)
                                    {
                                        var db = copyNode.BindNode.GetInVariable(a);
                                        Variable newVar = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(db.GetType());
                                        newVar.Copy(db, false);
                                        pGrah.BindNode.AddInArgv(newVar);
                                    }
                                }
                                if (copyNode.BindNode.outArgvs != null)
                                {
                                    for (int a = 0; a < copyNode.BindNode.GetOutArgvCount(); ++a)
                                    {
                                        var db = copyNode.BindNode.GetOutVariable(a);
                                        Variable newVar = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(db.GetType());
                                        newVar.Copy(db, false);
                                        pGrah.BindNode.AddOutArgv(newVar);
                                    }
                                }
                                if (actionNode != null && srcCopyAction != null && srcCopyAction.condition != null && srcCopyAction.condition.portals != null)
                                {
                                    actionNode.condition = new Condition();
                                    actionNode.condition.portals = new List<PortalNode>();
                                    foreach (var db in srcCopyAction.condition.portals)
                                    {
                                        PortalNode portal = new PortalNode();
                                        portal.opType = db.opType;
                                        if (db.argv != null && db.argv.variable != null)
                                        {
                                            portal.argv = new Port(AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(db.argv.variable.GetType()));
                                            portal.argv.Copy(db.argv);
                                        }
                                        if (db.compare != null && db.compare.variable != null)
                                        {
                                            portal.compare = new Port(AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(db.compare.variable.GetType()));
                                            portal.compare.Copy(db.compare);
                                        }
                                        actionNode.condition.portals.Add(portal);
                                    }
                                }
                                pGrah.BindNode.Save();
                                vNews.Add(pGrah);
                            }
                        }
                    }
                }
                
            }
            CopySelectionCathes.Clear();

            if(vNews.Count>0)
            {
                m_SelectionCache = new List<IGraphNode>(vNews.ToArray());
            }
            
            GUI.FocusControl("");
        }
        //------------------------------------------------------
        public void SelectNode(IGraphNode node, bool add)
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
        }
        //------------------------------------------------------
        public void DeselectNode(IGraphNode node)
        {
            bool bInclude = m_SelectionCache.Contains(node);
            m_SelectionCache.Remove(node);
        }
        //------------------------------------------------------
        public bool IsHoveringTitle(IGraphNode node, bool bSubLink = true)
        {
            Vector2 mousePos = Event.current.mousePosition;
            if(node is GraphNode)
            {
                //Get node position
                Vector2 nodePos = GridToWindowPosition(node.GetPosition());
                if (bSubLink)
                    nodePos.x += 32;
                float width;
                Vector2 size;
                if (nodeSizes.TryGetValue(node, out size)) width = size.x;
                else width = 200;
                Rect windowRect;
                if (bSubLink) windowRect = new Rect(nodePos, new Vector2((width - 64) / zoom, 30 / zoom));
                else windowRect = new Rect(nodePos, new Vector2(width / zoom, 30 / zoom));
                return windowRect.Contains(mousePos);
            }
            else
            {
                Vector2 nodePos = GridToWindowPosition(node.GetPosition());
                Rect windowRect = new Rect(nodePos, new Vector2(node.GetWidth(),node.GetHeight()));
                return windowRect.Contains(mousePos);
            }
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