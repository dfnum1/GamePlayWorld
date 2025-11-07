#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeEditor : EditorWindow, IAgentTreeEditor
    {
        public enum EControllType
        {
            New,
            Open,
            Save,
            SaveAs,
            Copy,
            Parse,
            Delete,
            OpenStruct,
            Finder,
            UnDo,
            Expand,
            FuncLocation,
            BreakPoint,
            Editor,
            Count,
        }
        static string[] CONTROLL_NAMES = new string[] { "新建(ctl+n)", "打开(ctl+o)", "保存(ctl+s)", "另存为(shift+ctl+s)", "复制(ctl+c)", "粘贴(ctl+v)", "删除(del)", "结构体成员", "查找", "回退(ctl+z)", "展开", "函数定位", "断点", "编辑设置项" };

        public interface BaseParam
        {
        }
        public struct EnterParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public ETaskType type;
        }

        public struct APIParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
        }

        public struct ActionParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public ATEditorAttrData Data;
        }

        public struct VarialePop : BaseParam
        {
            public Variable Data;
            public ArgvPort Port;
        }
        public static AgentTreeEditor current;

        AgentTreeEditorLogic m_pLogic = null;
        public AgentTreeEditorLogic logic
        {
            get { return m_pLogic; }
        }

        private double m_PreviousTime;
        private float m_fDeltaTime;

        private bool m_bPopStrcutMenus = false;
        private System.Object m_PopStrcutMenuCallbackData = null;
        private bool m_bOpenSearch = false;
        object m_OpenAPISearchData = null;
        private Vector2 m_OpenMousePos = Vector2.zero;


        public static Dictionary<int, APINode> GlobalAPIs = new Dictionary<int, APINode>();
        public static List<string> GlobalAPIPops = new List<string>();
        public static List<APINode> GlobalAPILists = new List<APINode>();

        public List<string> ExportAPIPopNamess = new List<string>();

        AgentTreeSearcherVariable m_PopVariable = new AgentTreeSearcherVariable();
        //-----------------------------------------------------
        [MenuItem("Tools/AT/图形脚本")]
        private static void OpenTool()
        {
            AgentTreeEditor Instance = EditorWindow.GetWindow<AgentTreeEditor>();
            Instance.titleContent = new GUIContent("可视化编程编辑器");
            Instance.wantsMouseMove = true;
            Instance.Show();
        }
        //------------------------------------------------------
        public static AgentTreeEditor Editor(AAgentTreeData pATDAta, UnityEngine.Object srcPrefab = null)
        {
            var opendInstance = FindOpened(pATDAta.Data);
            if (opendInstance != null)
                return opendInstance;
            var Instance = EditorWindow.GetWindow<AgentTreeEditor>();
            string name = "可视化编程编辑器";
            if (srcPrefab != null)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(srcPrefab);
                if (prefab != null)
                {
                    name += "[" + prefab.name + "]";
                }
                else
                    name += "[" + srcPrefab.name + "]";
            }
            if (pATDAta != null) name += "[" + pATDAta.name + "]";
            Instance.titleContent = new GUIContent(name);
            Instance.wantsMouseMove = true;
            Instance.Show();
            if (Instance.m_pLogic != null)
                Instance.m_pLogic.LoadAT(pATDAta, srcPrefab);
            return Instance;
        }
        //------------------------------------------------------
        public static AgentTreeEditor Editor(AgentTreeCoreData pATDAta, UnityEngine.Object srcPrefab = null)
        {
            var opendInstance = FindOpened(pATDAta);
            if (opendInstance != null)
                return opendInstance;
            var Instance = EditorWindow.GetWindow<AgentTreeEditor>();
            string name = "可视化编程编辑器";
            if (srcPrefab != null)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(srcPrefab);
                if(prefab!=null)
                {
                    name += "[" + prefab.name + "]";
                }
                else
                    name += "[" + srcPrefab.name + "]";
            }
            Instance.titleContent = new GUIContent(name);
            Instance.wantsMouseMove = true;
            Instance.Show();

            if (Instance.m_pLogic != null)
                Instance.m_pLogic.LoadAT(pATDAta, srcPrefab);
            return Instance;
        }
        //------------------------------------------------------
        static AgentTreeEditor FindOpened(AgentTreeCoreData bindData)
        {
            AgentTreeEditor[] windows = Resources.FindObjectsOfTypeAll<AgentTreeEditor>();
            for (int i = 0; i < windows.Length; i++)
            {
                if (windows[i] != null && windows[i].GetATCoreData() == bindData)
                {
                    windows[i].Focus();
                    return windows[i];
                }
            }
            return null;
        }
        //------------------------------------------------------

        public string strFile = "";
        //------------------------------------------------------
        protected void OnDisable()
        {
            m_pLogic.OnDisable();
            SceneView.duringSceneGui -= OnSceneFunc;
            AgentTreeManager.OnExcudeAction -= OnExcudeAction;
        }
        //-----------------------------------------------------
        protected void OnEnable()
        {
            base.minSize = new Vector2(850f, 320f);
            m_pLogic = new AgentTreeEditorLogic();
            m_pLogic.OnEnable(this);
            CheckAssemblyAT();

            VariableFactory.OnNewVariable = m_pLogic.OnNewVariable;

            AgentTreeManager.OnExcudeAction += OnExcudeAction;

            m_bOpenSearch = false;
            m_OpenAPISearchData = null;
            m_OpenMousePos = Vector2.zero;
            SceneView.duringSceneGui += OnSceneFunc;
        }
        //------------------------------------------------------
        public AgentTreeCoreData GetATCoreData()
        {
            if (m_pLogic.ATData == null)
                return null;
            return m_pLogic.ATData.Data;
        }
        //------------------------------------------------------
        public Dictionary<int, GraphNode> GetGraphNodes()
        {
            return m_pLogic.ActionNodes;
        }
        //------------------------------------------------------
        public GraphNode GetGraphNode(int guid)
        {
            return m_pLogic.GetGraphNode(guid);
        }
        //------------------------------------------------------
        public Dictionary<int, StructData> GetVariableStructs()
        {
            return m_pLogic.VariableStrcuts;
        }
        //------------------------------------------------------
        public void AdjustMaxGuid()
        {
            if (m_pLogic != null) m_pLogic.AdjustVariableGUID();
        }
        //------------------------------------------------------
        public AgentTree GetCurrentAgentTree()
        {
            if (m_pLogic != null) return m_pLogic.GetAgentTree();
            return null;
        }
        //------------------------------------------------------
        private void OnGUI()
        {
            AgentTreeUtl.BeginClip();

            if (m_PopVariable.IsOpen())
                EditorGUI.BeginDisabledGroup(true);
            m_pLogic.OnGUI();

            if (m_PopVariable.IsOpen())
                EditorGUI.EndDisabledGroup();
            m_PopVariable.OnDraw();
            OnEvent(Event.current);
            AgentTreeUtl.EndClip();

            if (Event.current.type == EventType.MouseUp)
            {
                if (!m_PopVariable.IsMouseIn(Event.current.mousePosition))
                    m_PopVariable.Close();
            }
        }
        //-----------------------------------------------------
        public void OnSceneFunc(SceneView sceneView)
        {
            OnSceneGUI(sceneView);
        }
        //-----------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
            if (m_pLogic != null)
                m_pLogic.OnSceneGUI(sceneView);
        }
        //------------------------------------------------------
        public void OnEvent(Event evt)
        {
            if(evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.S && evt.control && evt.shift)
                {
                    CreateCtlAction(EControllType.SaveAs);
                }
                if (evt.keyCode == KeyCode.Z && evt.control)
                {
                    CreateCtlAction(EControllType.UnDo);
                }
                else if (evt.keyCode == KeyCode.S && evt.control)
                {
                    CreateCtlAction(EControllType.Save);
                }
                else if (evt.keyCode == KeyCode.O && evt.control)
                {
                    CreateCtlAction(EControllType.Open);
                }
                else if (evt.keyCode == KeyCode.N && evt.control)
                {
                    CreateCtlAction(EControllType.SaveAs);
                }
                else if (evt.keyCode == KeyCode.C && evt.control)
                {
                    CreateCtlAction(EControllType.Copy);
                }
                else if (evt.keyCode == KeyCode.V && evt.control)
                {
                    CreateCtlAction(EControllType.Parse);
                }
                else if (evt.keyCode == KeyCode.Delete)
                {
                    CreateCtlAction(EControllType.Delete);
                }

                if (evt.keyCode == KeyCode.Escape)
                {
                    AgentTreeUtl.pCopyCurve = null;
                    m_PopVariable.Close();
                }
            }
        }
        //-----------------------------------------------------
        private void Update()
        {
            m_fDeltaTime = (float)((EditorApplication.timeSinceStartup - m_PreviousTime) * 0.8f);
            if (m_pLogic != null)
            {
                m_pLogic.Update(m_fDeltaTime);
            }

            if(m_bOpenSearch)
            {
                m_pLogic.OpenSearcher(m_OpenMousePos);
                m_bOpenSearch = false;
            }
            if(m_bPopStrcutMenus)
            {
                if(m_pLogic!=null) m_pLogic.PopMemberVariables(m_PopStrcutMenuCallbackData);
                m_PopStrcutMenuCallbackData = null;
                m_bPopStrcutMenus = false;
            }
            if(m_OpenAPISearchData!=null)
            {
                m_pLogic.OpenAPISearcher(m_OpenMousePos, m_OpenAPISearchData);
                m_OpenAPISearchData = null;
            }
            this.Repaint();
            m_PreviousTime = EditorApplication.timeSinceStartup;
        }
        //------------------------------------------------------
        public Texture2D GetGridTexture()
        {
            return AgentTreePreferences.GetSettings().gridTexture;
        }
        //------------------------------------------------------
        public Texture2D GetSecondaryGridTexture()
        {
            return AgentTreePreferences.GetSettings().crossTexture;
        }
        //------------------------------------------------------
        public virtual GUIStyle GetBodyStyle()
        {
            return AgentTreeEditorResources.styles.nodeBody;
        }
        //------------------------------------------------------
        public static void RepaintAll()
        {
            AgentTreeEditor[] windows = Resources.FindObjectsOfTypeAll<AgentTreeEditor>();
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Repaint();
            }
        }
        //------------------------------------------------------
        public void OnUpdateNode(GraphNode pNode)
        {

        }
        //------------------------------------------------------
        public ATExportNodeAttrData GetActionNodeAttr(int actionType)
        {
            ATExportNodeAttrData pAtt;
            if (AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue(actionType, out pAtt))
                return pAtt;
            return null;
        }
        //------------------------------------------------------
        void CheckAssemblyAT()
        {
            AgentTreeEditorUtils.CheckAssemblyAT();
        }
        //------------------------------------------------------
        public void BaseFuncContextMenu(Vector2 mousePos)
        {
            GenericMenu menu = new GenericMenu();

            for (EControllType i = EControllType.New; i < EControllType.Count; ++i)
            {
                if (i == EControllType.Parse && (AgentTreeEditorLogic.pCopyATData != null || AgentTreeEditorLogic.CopySelectionCathes != null))
                {
                    menu.AddDisabledItem(new GUIContent(CONTROLL_NAMES[(int)i]));
                }
                else if(i == EControllType.FuncLocation && m_pLogic.SelectonCache!=null && m_pLogic.SelectonCache.Count == 1)
                {
                    if(m_pLogic.SelectonCache[0] is GraphNode)
                    {
                        if (((GraphNode)m_pLogic.SelectonCache[0]).BindNode != null)
                        {
                            menu.AddItem(new GUIContent(CONTROLL_NAMES[(int)i]), false, CreateCtlAction, i);
                        }
                    }

                }
                else if (i == EControllType.BreakPoint && m_pLogic.SelectonCache != null && m_pLogic.SelectonCache.Count == 1)
                {
                    if (m_pLogic.SelectonCache[0] is GraphNode)
                    {
                        if (((GraphNode)m_pLogic.SelectonCache[0]).BindNode != null)
                        {
                            menu.AddItem(new GUIContent(CONTROLL_NAMES[(int)i]), false, CreateCtlAction, i);
                        }
                    }
   
                }
                else
                    menu.AddItem(new GUIContent(CONTROLL_NAMES[(int)i]), false, CreateCtlAction, i);
            }
            menu.DropDown(new Rect(mousePos, Vector2.zero));
            menu.ShowAsContext();

            m_pLogic.CloseSearcher();
        }
        //------------------------------------------------------
        public void FuncContextMenu(Vector2 mousePos)
        {
            m_OpenMousePos = mousePos;
            m_bOpenSearch = true;
          //  m_pLogic.OpenSearcher(mousePos);
            if(m_pLogic.SelectonCache!=null && m_pLogic.SelectonCache.Count == 1 && m_pLogic.SelectonCache[0] is GraphNode)
            {
                if (m_pLogic.IsHoveringTitle(m_pLogic.SelectonCache[0] as GraphNode))
                    m_pLogic.OpenSearcher(m_pLogic.SelectonCache[0] as GraphNode, mousePos);
            }
        }
        //------------------------------------------------------
        public void ATFuncContextMenu(Vector2 mousePos, System.Object graphNode)
        {
            m_OpenMousePos = mousePos;
            m_OpenAPISearchData = graphNode;
        }
        //------------------------------------------------------
        public APINode GetAPINode(long guidKey)
        {
            int guid = (int)(guidKey >> 32);
            int owner = (int)(guidKey & 0xffffffff);
            if(owner ==0)
            {
                //! global
                APINode apiNode;
                if (GlobalAPIs.TryGetValue(guid, out apiNode))
                    return apiNode;
                return null;
            }

            GraphNode graphNode;
            if (m_pLogic.ActionNodes.TryGetValue(guid, out graphNode))
                return graphNode.BindNode as APINode;
            return null;
        }
        //------------------------------------------------------
        public void OnAPINodeSelect(object var)
        {
            if (var is AgentTreeSearcherAPI.ItemEvent)
            {
                AgentTreeSearcherAPI.ItemEvent param = (AgentTreeSearcherAPI.ItemEvent)var;
                if (param.bindData is GraphNode)
                {
                    (param.bindData as GraphNode).SetAPINode(param.apiNode, param.atGUID);
                }
            }
        }
        //------------------------------------------------------
        public void ShowRerouteContextMenu(ReroutePort reroute)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Remove"), false, () => reroute.RemovePoint());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }
        //------------------------------------------------------
        public void ShowRefPortContextMenu(IPortNode port)
        {
            if(port is ArgvPort)
            {
                m_PopVariable.OnSelectedCall = OnRefPortSelected;
                m_PopVariable.argvPort = port as ArgvPort;
                m_PopVariable.vIngores.Add(m_PopVariable.argvPort.variable);
                m_PopVariable.filterVariable = m_PopVariable.argvPort.variable.GetType();
                m_PopVariable.Open(m_pLogic, new Rect(position.width / 2, position.height / 2, 350, 400));
            }
        }
        //------------------------------------------------------
        public void ShowClassMethod(Vector2 mousePos, ArgvPort port, System.Type classType)
        {
            m_pLogic.OpenSearcher(mousePos, classType);
        }
        //------------------------------------------------------
        void OnRefPortSelected(ArgvPort port, BaseParam variable )
        {
            AgentTreeEditor.VarialePop param = (AgentTreeEditor.VarialePop)variable;
            if(param.Data!=null)
            {
                m_pLogic.CreateRefPortNode(port, param.Data);
            }
        }
        //------------------------------------------------------
        /// <summary> Show right-click context menu for hovered port </summary>
        public void ShowPortContextMenu(IPortNode hoveredPort)
        {
            if (hoveredPort == null) return;
            MousePosAndData mouseAndData = new MousePosAndData();
            mouseAndData.mousePos = Event.current.mousePosition;
            mouseAndData.bindData = hoveredPort;

            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Clear Connections"), false, () => hoveredPort.ClearConnections());
            contextMenu.AddItem(new GUIContent("重命名"), false, () => hoveredPort.ReName());
            contextMenu.AddItem(new GUIContent("常量标签"), false, () => hoveredPort.SetConstFlag());
            contextMenu.AddItem(new GUIContent("等价值"), false, () => hoveredPort.Equivalence());
            contextMenu.AddItem(new GUIContent("成员变量"), false, PopMemberVariables, mouseAndData);
            contextMenu.AddItem(new GUIContent("应用"), false, () => hoveredPort.ApplayToDef());
            if(hoveredPort.IsLocal())
                contextMenu.AddItem(new GUIContent("设为自身变量"), false, () => hoveredPort.SetLocal(false));
            else
                contextMenu.AddItem(new GUIContent("设为局部变量"), false, () => hoveredPort.SetLocal(true));

            if(hoveredPort.GetATNode()!=null && hoveredPort.GetATNode() is ActionNode)
            {
                ActionNode actionNode = hoveredPort.GetATNode() as ActionNode;
                if(actionNode.actionType == EActionType.ATFunction)
                {
                    contextMenu.AddItem(new GUIContent("覆盖"), false, () => hoveredPort.SetFlag( EFlag.Override, hoveredPort.IsFlag( EFlag.Override) ));
                }
            }

            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }
        //------------------------------------------------------
        public void VaribleReName(Variable variable)
        {
            RenamePopup.Show(variable);
        }
        //------------------------------------------------------
        public void PopMemberVariables(System.Object port)
        {
            m_PopStrcutMenuCallbackData = port;
            m_bPopStrcutMenus = true;
        }
        //------------------------------------------------------
        public void PopEquivalence(ArgvPort port)
        {
            if (port == null) return;
            m_PopVariable.filterVariable = port.variable.GetType();
            m_PopVariable.argvPort = port;
            m_PopVariable.Open(m_pLogic, new Rect(position.width/2, position.height/2, 350, 400));
        }
        //------------------------------------------------------
        public void EquivalenceVariable(object val)
        {
            ArgvPort port = ((VarialePop)val).Port;
            Variable toVariable = ((VarialePop)val).Data;
            if (toVariable == null) return;
            if(port !=null && port.variable.GetType() == toVariable.GetType())
            {
                port.variable = toVariable;
            }
        }
        //------------------------------------------------------
        public void CreateActionNode(object val)
        {
            ActionParam param = (ActionParam)val;
            m_pLogic.CreateActionNode(param);
        }
        //------------------------------------------------------
        public void CreateTaskNode(object val)
        {
            EnterParam param = (EnterParam)val;
            m_pLogic.CreateTaskNode(param.type, param.gridPos);
        }
        //------------------------------------------------------
        public void CreateAPINode(object val)
        {
            APIParam param = (APIParam)val;
            m_pLogic.CreateAPINode(param);
        }
        //------------------------------------------------------
        public void CreateCtlAction(object var)
        {
            EControllType ctl = (EControllType)var;
            switch(ctl)
            {
                case EControllType.Copy:
                    {
                        if(m_pLogic.SelectonCache!=null)
                        {
                            if (AgentTreeEditorLogic.CopySelectionCathes == null) AgentTreeEditorLogic.CopySelectionCathes = new List<IGraphNode>();
                            AgentTreeEditorLogic.CopySelectionCathes.Clear();
                            for (int i = 0; i < m_pLogic.SelectonCache.Count; ++i)
                            {
                                AgentTreeEditorLogic.CopySelectionCathes.Add(m_pLogic.SelectonCache[i]);
                            }
                        }
                    }
                    break;
                case EControllType.Parse:
                    {
                        m_pLogic.ParseNode();
                    }
                    break;
                case EControllType.New:
                    {
                        m_pLogic.NewAT();
                    }
                    break;
                case EControllType.Open:
                    {
                        string strPath = EditorUtility.OpenFilePanel("打开图形脚本", Application.dataPath, "asset");
                        strPath = strPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        if (string.IsNullOrEmpty(strPath))
                        {
                            EditorUtility.DisplayDialog("提示", "请选择一个有效目录文件", "好的");
                            return;
                        }
                        m_pLogic.LoadAT(strPath);
                    }
                    break;
                case EControllType.Save:
                    {
                        m_pLogic.Save();
                    }
                    break;
                case EControllType.SaveAs:
                    {
                        m_pLogic.Save(true);
                    }
                    break;
                case EControllType.Delete:
                    {
                        m_pLogic.RemoveSelectedNodes();
                    }
                    break;
                case EControllType.OpenStruct:
                    {
                        m_pLogic.m_bOpenStruct = true;
                    }
                    break;
                case EControllType.Finder:
                    {
                        m_pLogic.m_bOpenFinder = true;
                    }
                    break;
                case EControllType.UnDo:
                    {
                   //     m_pLogic.UnRedo();
                    }
                    break;
                case EControllType.Expand:
                    {
//                         if(m_pLogic.SelectonCache!=null)
//                         {
//                             for(int i = 0; i < m_pLogic.SelectonCache.Count; ++i)
//                             {
//                                 m_pLogic.SelectonCache[i].SetExpand(!m_pLogic.SelectonCache[i].IsExpand());
//                             }
//                         }
                    }
                    break;
                case EControllType.Editor:
                    {
                        SettingsService.OpenUserPreferences("Preferences/GameFramework/ATEditor");
                    }
                    break;
                case EControllType.FuncLocation:
                    {
                        if (m_pLogic.SelectonCache != null && m_pLogic.SelectonCache.Count>0 && m_pLogic.SelectonCache[0] is GraphNode && ((GraphNode)m_pLogic.SelectonCache[0]).BindNode != null)
                        {
                            ExcudeNode bindNode = ((GraphNode)m_pLogic.SelectonCache[0]).BindNode;
                            if (bindNode is ActionNode)
                            {
                                ActionNode bindAction = (ActionNode)bindNode;
                                ATExportNodeAttrData exportNode;
                                if (AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)bindAction.actionType, out exportNode))
                                {
                                    //Debug.Log(exportNode.classFullName);//Plugin.AT.AgentTree_UIManager
                                    var classNames = exportNode.classFullName.Split('_');
                                    string className = "";
                                    string filePath = "";
                                    if (classNames.Length > 1)
                                    {
                                        className = classNames[1];
                                        //Debug.Log(className);
                                    }
                                    var scripts = AssetDatabase.FindAssets("t:Script " + className, new string[] { "Assets/Scripts" });
                                    if (scripts.Length > 0)
                                    {
                                        filePath = AssetDatabase.GUIDToAssetPath(scripts[0]);
                                    }
                                    //foreach (var item in scripts)
                                    //{
                                    //    Debug.Log(AssetDatabase.GUIDToAssetPath(item));
                                    //}

                                    filePath = Application.dataPath + "/../" + filePath;

                                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 0, 0);
                                }
                            }
                            
                        }
                    }
                    break;
                case EControllType.BreakPoint:
                    {
                        if (m_pLogic.SelectonCache != null && m_pLogic.SelectonCache.Count > 0 && m_pLogic.SelectonCache[0] is GraphNode && ((GraphNode)m_pLogic.SelectonCache[0]).BindNode != null)
                        {
                            ExcudeNode bindNode = ((GraphNode)m_pLogic.SelectonCache[0]).BindNode;
                            bindNode.bBreakPoint = !bindNode.bBreakPoint;
                        }
                    }
                    break;
            }
        }
        //------------------------------------------------------
        public void OnExcudeAction(AgentTreeTask pTask, ActionNode pNode)
        {
            if(m_pLogic!=null)
            {
                m_pLogic.OnExcudeAction(pTask, pNode);
            }
        }
        //------------------------------------------------------
        public bool OnFillCustomVariable(AgentTreeTask pTask, Variable pCustomVariable, IUserData pData)
        {
            return false;
        }
        //------------------------------------------------------
        public int GetClassHashCode(Type type)
        {
            return 0;
        }
        //------------------------------------------------------
        public Type GetHashCodeClass(int hasCode)
        {
            return null;
        }
        //------------------------------------------------------
        public int GetParentHashCode(int hasCode)
        {
            return 0;
        }
    }
}
#endif
