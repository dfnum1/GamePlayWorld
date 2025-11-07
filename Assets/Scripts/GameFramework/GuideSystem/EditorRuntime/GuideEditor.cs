#if UNITY_EDITOR && USE_GUIDESYSTEM
using Framework.Data;
using Framework.Plugin.AT;
using Framework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace Framework.Plugin.Guide
{
    public partial class GuideEditor : EditorWindow, IGuideSystemCallback
    {
        public enum EControllType
        {
            New,
            Open,
            Save,
            ReName,
            Copy,
            Parse,
            Delete,
            UnDo,
            LinkFile,
            OpenCurrent,
            Test,
            Expand,
            UnExpand,
            Stop,

            Recode,
            Unrecode,
            
            ExportCode,
            Commit,
            Editor,
            Count,
        }
        static string[] CONTROLL_NAMES = new string[] { "新建(ctl+n)", "打开(ctl+1)", "保存(ctl+s)", "重新命名", "复制(ctl+c)", "粘贴(ctl+v)", "删除(del)", "回退(ctl+z)", "关联文件", "打开当前引导", "测试", "全部展开", "全部收起", "关闭当前引导", "录制", "取消录制", "导出代码", "提交", "编辑设置项" };

        public interface BaseParam
        {

        }
        public struct OpParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public string strName;
        }
        public struct TriggerParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public NodeAttr Data;
        }
        public struct ExcudeParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public NodeAttr Data;
        }
        public struct StepParam : BaseParam
        {
            public Vector2 mousePos;
            public Vector2 gridPos;
            public NodeAttr Data;
        }
        public struct DataParam : BaseParam
        {
            public GuideGroup Data;
        }
        public static GuideEditor current;

        GuideEditorLogic m_pLogic = null;
        public GuideEditorLogic logic
        {
            get { return m_pLogic; }
        }
        public static GuideEditor Instance = null;

        private double m_PreviousTime;
        private float m_fDeltaTime;
        public float deltaTime
        {
            get { return m_fDeltaTime; }
        }

        private bool m_bOpenSearch = false;
        private Vector2 m_OpenMousePos = Vector2.zero;
        private bool m_bOpenGuideSearcher = false;
        DataSearcher m_pDataSearcher = new DataSearcher();

        public class NodeAttr
        {
            public class ArgvAttr
            {
                public EBitGuiType bBit = EBitGuiType.None;
                public GuideArgvAttribute attr = null;
            }
            public bool previewEditor = false;
            public int type;
            public string strExportName;
            public string strShortName;
            public string strName;
            public string strQueueName;

            public List<ArgvAttr> argvs = new List<ArgvAttr>();
        }
        protected GuideDatas m_pGuideCsv = null;


        public static MethodInfo EventBuildByTypeMethod = null;
        public static MethodInfo EventDrawMethod = null;
        public static MethodInfo EventPopMethod = null;
        public static MethodInfo EventNameGet = null;

        public static bool IsRecodeMode = false;

        public Dictionary<int, NodeAttr> NodeTypes = new Dictionary<int, NodeAttr>();


        public List<string> ExportTypesPop = new List<string>();
        public List<int> ExportTypes = new List<int>();
        public Dictionary<int, NodeAttr> StepTypes = new Dictionary<int, NodeAttr>();

        public List<string> ExportTriggerTypesPop = new List<string>();
        public List<int> ExportTriggerTypes = new List<int>();
        public Dictionary<int, NodeAttr> TriggerTypes = new Dictionary<int, NodeAttr>();

        public List<string> ExportExcudeTypesPop = new List<string>();
        public List<int> ExportExcudeTypes = new List<int>();
        public Dictionary<int, NodeAttr> ExcudeTypes = new Dictionary<int, NodeAttr>();

        public System.Reflection.MethodInfo OnNodeEditorPreview = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewEnable = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewDisable = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewVisible = null;

        private int m_nLastSelectGOId = 0;
        //-----------------------------------------------------
        [MenuItem("Tools/引导编辑器 _F3")]
        private static void OpenTool()
        {
            if (Instance != null) return;
            if (Instance == null)
                Instance = EditorWindow.GetWindow<GuideEditor>();
            Instance.titleContent = new GUIContent("引导编辑器");
            Instance.wantsMouseMove = true;
            Instance.Show();
        }
        //------------------------------------------------------

        public string strFile = "";
        //------------------------------------------------------
        protected void OnDisable()
        {
            m_pLogic.OnDisable();
            Instance = null;
            SceneView.duringSceneGui -= OnSceneFunc;
            GuideSystem.getInstance().UnRegister(this);

            if (OnNodeEditorPreviewDisable != null)
                OnNodeEditorPreviewDisable.Invoke(null, null);
        }
        //-----------------------------------------------------
        protected void OnEnable()
        {
            Instance = this;
            IsRecodeMode = false;
            base.minSize = new Vector2(850f, 320f);
            m_pLogic = new GuideEditorLogic();
            m_pLogic.OnEnable(this);
            CheckAssemblyAI();

            m_bOpenGuideSearcher = false;
            m_bOpenSearch = false;
            m_OpenMousePos = Vector2.zero;
            SceneView.duringSceneGui += OnSceneFunc;
            GuideSystem.getInstance().Register(this);
            string[] guideDatas = AssetDatabase.FindAssets("t:GuideDatas");
            if (guideDatas != null && guideDatas.Length > 0)
            {
                m_pGuideCsv = AssetDatabase.LoadAssetAtPath<GuideDatas>(AssetDatabase.GUIDToAssetPath(guideDatas[0]));
            }
            if (m_pGuideCsv!=null)
            {
                GuideSystem.getInstance().Register(this);
                if (Core.AFramework.mainFramework == null || m_pGuideCsv.datas == null || m_pGuideCsv.datas.Length <= 0)
                    m_pGuideCsv.Mapping();
                m_pGuideCsv.Init(true);
                GuideSystem.getInstance().datas = m_pGuideCsv.allDatas;
            }

            hideFlags = HideFlags.DontSave;
            if (OnNodeEditorPreviewEnable != null)
                OnNodeEditorPreviewEnable.Invoke(null, null);
        }
        //------------------------------------------------------
        public void Save(bool bPop=true, bool bSaveAll = false)
        {
            if (m_pGuideCsv == null)
            {
                ShowNotification(new GUIContent("保存失败"));
                return;
            }
            m_pGuideCsv.Save(bSaveAll);
            if (bPop) Framework.Plugin.Guide.GuideEditor.Instance.ShowNotification(new GUIContent("保存完成"));
        }
        //------------------------------------------------------
        public string GetSaveFilePath()
        {
            if (m_pGuideCsv == null) return null;
            return m_pGuideCsv.GetSaveRoot();
        }
        //------------------------------------------------------
        public GuideGroup NewGuide()
        {
            if (m_pGuideCsv == null) return null;
            return m_pGuideCsv.New();
        }
        //------------------------------------------------------
        private void OnGUI()
        {
            GuideEditorLogic.BeginClip();

            EditorGUI.BeginDisabledGroup(m_pDataSearcher.IsOpen());
            m_pLogic.OnGUI();
            EditorGUI.EndDisabledGroup();

            m_pDataSearcher.OnDraw();
            OnEvent(Event.current);
            GuideEditorLogic.EndClip();
        }
        //-----------------------------------------------------
        static public void OnSceneFunc(SceneView sceneView)
        {
            Instance.OnSceneGUI(sceneView);
        }
        //-----------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
            if (m_pLogic != null)
                m_pLogic.OnSceneGUI(sceneView);
        }
        //------------------------------------------------------
        public void BaseFuncContextMenu(Vector2 mousePos)
        {
            GenericMenu menu = new GenericMenu();

            for (EControllType i = EControllType.New; i < EControllType.Count; ++i)
            {
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
        }
        //------------------------------------------------------
        public void ShowPortContextMenu(IPortNode hoveredPort)
        {
            if (hoveredPort == null) return;
            GenericMenu contextMenu = new GenericMenu();
             contextMenu.AddItem(new GUIContent("Clear Connections"), false, () => hoveredPort.ClearConnections());
//             contextMenu.AddItem(new GUIContent("常量标签"), false, () => hoveredPort.SetConstFlag());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }
        //------------------------------------------------------
        public void OnEvent(Event evt)
        {
            if(evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.Z && evt.control)
                {
                    CreateCtlAction(EControllType.UnDo);
                }
                else if (evt.keyCode == KeyCode.S && evt.control)
                {
                    CreateCtlAction(EControllType.Save);
                }
                else if (evt.keyCode == KeyCode.Alpha1 && evt.control)
                {
                    CreateCtlAction(EControllType.Open);
                }
                else if (evt.keyCode == KeyCode.C && evt.control)
                {
                    CreateCtlAction(EControllType.Copy);
                }
                else if (evt.keyCode == KeyCode.V && evt.control)
                {
                    CreateCtlAction(EControllType.Parse);
                }
                else if (evt.keyCode == KeyCode.N && evt.control)
                {
                    CreateCtlAction(EControllType.New);
                }
                else if (evt.keyCode == KeyCode.Delete)
                {
                    CreateCtlAction(EControllType.Delete);
                }
                if (evt.keyCode == KeyCode.Escape)
                    m_pDataSearcher.Close();
            }
        }
        //-----------------------------------------------------
        private void Update()
        {
            m_fDeltaTime = (float)((EditorApplication.timeSinceStartup - m_PreviousTime) * 0.8f);
            if (m_pLogic != null)
            {
                m_pLogic.Update(m_fDeltaTime);

                if (m_bOpenGuideSearcher)
                {
                    m_pDataSearcher.Open(new Rect(0,20, 350, position.height));
                    m_bOpenSearch = false;
                    m_bOpenGuideSearcher = false;
                }
                if (m_bOpenSearch)
                {
                    m_pLogic.OpenSearcher(m_OpenMousePos);
                    m_bOpenSearch = false;
                }
            }

            if(IsRecodeMode)
            {
                var eventCut = UnityEngine.EventSystems.EventSystem.current;
                if(eventCut)
                {
                    bool isOver = eventCut.IsPointerOverGameObject();
                    if (isOver && Input.GetMouseButtonDown(0))
                    {
                        var curSelect = eventCut.currentSelectedGameObject;
                        if(curSelect == null)
                        {
                            PointerEventData pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);

                            pointerEventData.position = Input.mousePosition;

                            List<RaycastResult> results = ListPool<RaycastResult>.Get();
                            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, results);
                            int count = results.Count;

                            var components = ListPool<Component>.Get();
                            for (int i = 0; i < results.Count; ++i)
                            {
                                components.Clear();
                                results[i].gameObject.GetComponents(components);
                                var componentsCount = components.Count;
                                for (var j = 0; j < componentsCount; j++)
                                {
                                    var behavor = components[j] as Behaviour;
                                    if (behavor && behavor.isActiveAndEnabled && components[j] is IPointerClickHandler)
                                    {
                                        curSelect = components[j].gameObject;
                                        break;
                                    }
                                }
                                if (curSelect) break;
                            }
                            ListPool<Component>.Release(components);

                            ListPool<RaycastResult>.Release(results);
                        }
                        

                        if (curSelect && curSelect.GetInstanceID() != m_nLastSelectGOId)
                        {
                            //! 点击了一个按钮
                            GuideGuid guide = curSelect.GetComponent<GuideGuid>();
                            if (guide == null) guide = curSelect.AddComponent<GuideGuid>();
                            if (guide.Guid == 0)
                            {
                                guide.Guid = GuideGuidUtl.GeneratorGUID(guide);
                                GuideGuidUtl.SetDirtyPrefab(guide);
                            }
                            GuideGuidUtl.OnAdd(guide,false);

                            EventTriggerListener eventTrigger = curSelect.GetComponent<EventTriggerListener>();
                            if (eventTrigger == null) eventTrigger = curSelect.AddComponent<EventTriggerListener>();
                            eventTrigger.SetGuideGuid(guide);

                            AddRecodeClickStep(guide);
                        }
                        m_nLastSelectGOId = curSelect ? curSelect.GetInstanceID():-1 ;
                    }
                }

            }

            this.Repaint();
            m_PreviousTime = EditorApplication.timeSinceStartup;
        }
        //------------------------------------------------------
        void AddRecodeClickStep(GuideGuid guide)
        {
            StepParam stepParam = new StepParam();
            if (StepTypes.TryGetValue(100, out stepParam.Data))
            {
                bool bCtl =  Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
                if (!bCtl && !m_pLogic.HasStepNodeWidgetGuide(guide.Guid))
                {
                    var lastNode = m_pLogic.FindLastNodeAndNoNextLink();
                    if (lastNode != null)
                    {
                        stepParam.gridPos = lastNode.GetPosition() + Vector2.right * (50 + lastNode.GetWidth());
                        GraphNode newNode = m_pLogic.CreateStepNode(stepParam);
                        if (newNode != null)
                        {
                            newNode.bindNode.CheckPorts();
                            var ports = newNode.bindNode.GetArgvPorts();
                            if (stepParam.Data.argvs.Count == ports.Count)
                            {
                                for (int j = 0; j < stepParam.Data.argvs.Count; ++j)
                                {
                                    if (stepParam.Data.argvs[j].attr.displayType == typeof(GuideGuid))
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                            field.SetValue(ports[j], guide.Guid);
                                    }
                                }
                            }
                            lastNode.linkOutPort.baseNode.Links.Clear();
                            lastNode.linkOutPort.baseNode.Links.Add(newNode);
                            this.ShowNotification(new GUIContent("成功录制了控件\""+ guide .name+ "\"" ), 3);
                        }
                    }
                }
                else
                    this.ShowNotification(new GUIContent("该控件点击已录制！！！"), 3);
            }
            else
            {
                Debug.LogError("找不到步骤类型");
            }
        }
        //------------------------------------------------------
        public Texture2D GetGridTexture()
        {
            return GuidePreferences.GetSettings().gridTexture;
        }
        //------------------------------------------------------
        public Texture2D GetSecondaryGridTexture()
        {
            return GuidePreferences.GetSettings().crossTexture;
        }
        //------------------------------------------------------
        public virtual GUIStyle GetBodyStyle()
        {
            return GuideEditorResources.styles.nodeBody;
        }
        //------------------------------------------------------
        public static void RepaintAll()
        {
            GuideEditor[] windows = Resources.FindObjectsOfTypeAll<GuideEditor>();
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Repaint();
            }
        }
        //------------------------------------------------------
        public void OpenEventInspector(List<IUserData> vEvents)
        {
            if (m_pLogic == null) return;
            m_pLogic.OpenInspector(vEvents);
        }
        //------------------------------------------------------
        public void OnUpdateNode(GraphNode pNode)
        {

        }
        //------------------------------------------------------
        void CheckAssemblyAI()
        {
            OnNodeEditorPreview = null;
            OnNodeEditorPreviewEnable = null;
            OnNodeEditorPreviewDisable = null;
            OnNodeEditorPreviewVisible = null;
            StepTypes.Clear();
            ExportTypesPop.Clear();
            ExportTypes.Clear();
            TriggerTypes.Clear();
            ExportTriggerTypesPop.Clear();
            ExportTriggerTypes.Clear();
            ExcudeTypes.Clear();
            ExportExcudeTypesPop.Clear();
            ExportExcudeTypes.Clear();
            NodeTypes.Clear();

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type enumType = types[i];
                    if(OnNodeEditorPreview==null && enumType.IsDefined(typeof(GuideEditorPreviewAttribute)))
                    {
                        OnNodeEditorPreview = enumType.GetMethod(enumType.GetCustomAttribute<GuideEditorPreviewAttribute>().CallMethod);
                        OnNodeEditorPreviewEnable = enumType.GetMethod("OnEnable");
                        OnNodeEditorPreviewDisable = enumType.GetMethod("OnDisable");
                        OnNodeEditorPreviewVisible = enumType.GetMethod("OnVisible");
                    }
                    if (enumType.IsDefined(typeof(Core.EventDrawAttribute), false))
                    {
                        EventDrawMethod = enumType.GetMethod(enumType.GetCustomAttribute<Core.EventDrawAttribute>().strMethod);
                        continue;
                    }
                    if (enumType.IsDefined(typeof(Core.EventPopClassAttribute), false))
                    {
                        EventPopMethod = enumType.GetMethod(enumType.GetCustomAttribute<Core.EventPopClassAttribute>().strMethod);
                        EventNameGet = enumType.GetMethod(enumType.GetCustomAttribute<Core.EventPopClassAttribute>().strEventName);
                        continue;
                    }
                    if (enumType.IsDefined(typeof(Core.EventCoreAttribute), false))
                    {
                        EventBuildByTypeMethod = enumType.GetMethod(enumType.GetCustomAttribute<Core.EventCoreAttribute>().strMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

                        continue;
                    }
                    if (enumType.IsEnum && enumType.IsDefined(typeof(GuideExportAttribute), false))
                    {
                        foreach (var v in Enum.GetValues(enumType))
                        {
                            string strName = Enum.GetName(enumType, v);
                            FieldInfo fi = enumType.GetField(strName);
                            int flagValue = (int)v;

                            if(!fi.IsDefined(typeof(GuideStepAttribute)) && !fi.IsDefined(typeof(GuideTriggerAttribute)) && !fi.IsDefined(typeof(GuideExcudeAttribute)))
                                continue;

                            bool bPreviewEditor = false;
                            string strDeclName = "";
                            if (fi.IsDefined(typeof(GuideStepAttribute)))
                            {
                                bPreviewEditor = fi.GetCustomAttribute<GuideStepAttribute>().bEditorPreview;
                                strDeclName = fi.GetCustomAttribute<GuideStepAttribute>().DisplayName;
                            }
                            else if (fi.IsDefined(typeof(GuideExcudeAttribute)))
                            {
                                bPreviewEditor = fi.GetCustomAttribute<GuideExcudeAttribute>().bEditorPreview;
                                strDeclName = fi.GetCustomAttribute<GuideExcudeAttribute>().DisplayName;
                            }
                            else
                                strDeclName = fi.GetCustomAttribute<GuideTriggerAttribute>().DisplayName;

                            {
                                NodeAttr node = new NodeAttr();
                                node.type = flagValue;
                                node.previewEditor = bPreviewEditor;

                                node.strShortName = strDeclName;
                                if (string.IsNullOrEmpty(node.strShortName))
                                    node.strShortName = strName;


                                node.strExportName = enumType.GetCustomAttribute<GuideExportAttribute>().strDisplay;
                                if (string.IsNullOrEmpty(node.strExportName))
                                    node.strExportName = enumType.Name;

                                node.strName = node.strExportName + "/" + node.strShortName;
                                node.strQueueName = node.strName + strName;

                                //    node.bCanAny = faaiDeclar.CanAny;
                                GuideArgvAttribute[] argvs = (GuideArgvAttribute[])fi.GetCustomAttributes(typeof(GuideArgvAttribute));

                                node.argvs = new List<NodeAttr.ArgvAttr>();
                                if (argvs != null && argvs.Length > 0)
                                {
                                    for (int a = 0; a < argvs.Length; ++a)
                                    {
                                        NodeAttr.ArgvAttr attr = new NodeAttr.ArgvAttr();
                                        attr.attr = argvs[a];
                                        attr.bBit = attr.attr.bBit;
                                        node.argvs.Add(attr);
                                    }
                                }

                                if(fi.IsDefined(typeof(GuideStepAttribute)))
                                {
                                    node.previewEditor = bPreviewEditor;
                                    StepTypes[flagValue] = node;

                                    ExportTypesPop.Add(node.strShortName);
                                    ExportTypes.Add(flagValue);
                                    NodeTypes[flagValue] = node;
                                }
                                else if (fi.IsDefined(typeof(GuideTriggerAttribute)))
                                {
                                    TriggerTypes[flagValue] = node;

                                    ExportTriggerTypesPop.Add(node.strShortName);
                                    ExportTriggerTypes.Add(flagValue);
                                }
                                else
                                {
                                    ExcudeTypes[flagValue] = node;

                                    ExportExcudeTypesPop.Add(node.strShortName);
                                    ExportExcudeTypes.Add(flagValue);
                                    NodeTypes[flagValue] = node;
                                }
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void LoadData(object val)
        {
            DataParam param = (DataParam)val;
            m_pLogic.Load(param.Data, null, true);
        }
        //------------------------------------------------------
        public void CreateStepNode(object val)
        {
            StepParam param = (StepParam)val;
            m_pLogic.CreateStepNode(param);
        }
        //------------------------------------------------------
        public void CreateTriggerNode(object val)
        {
            TriggerParam param = (TriggerParam)val;
            m_pLogic.CreateTriggerNode(param);
        }
        //------------------------------------------------------
        public void CreateExcudeNode(object val)
        {
            ExcudeParam param = (ExcudeParam)val;
            m_pLogic.CreateExcudeNode(param);
        }
        //------------------------------------------------------
        public void CreateConditionNode(object val)
        {
            OpParam param = (OpParam)val;
            m_pLogic.CreateOpNode(param);
        }
        //------------------------------------------------------
        public void OnGuideStatus(int guid, bool bDoing)
        {
            if (!bDoing) m_pLogic.OnStopDoingGuide();
        }
        //------------------------------------------------------
        public void CreateCtlAction(object var)
        {
            EControllType ctl = (EControllType)var;
            switch(ctl)
            {
                case EControllType.Copy:
                    {
                        if (m_pLogic.SelectonCache != null)
                        {
                            if (m_pLogic.CopySelectionCathes == null) m_pLogic.CopySelectionCathes = new List<GraphNode>();
                            m_pLogic.CopySelectionCathes.Clear();
                            for (int i = 0; i < m_pLogic.SelectonCache.Count; ++i)
                            {
                                m_pLogic.CopySelectionCathes.Add(m_pLogic.SelectonCache[i]);
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
                        m_pLogic.NewGuide();
                    }
                    break;
                case EControllType.Open:
                    {
                        m_bOpenGuideSearcher = true;
                    }
                    break;
                case EControllType.Save:
                    {
                        m_pLogic.Save();
                    }
                    break;
                case EControllType.Delete:
                    {
                        m_pLogic.RemoveSelectedNodes();
                    }
                    break;
                case EControllType.ReName:
                    {
                        m_pLogic.RenameSelectedNode();
                    }
                    break;
                case EControllType.UnDo:
                    {
                   //     m_pLogic.UnRedo();
                    }
                    break;
                case EControllType.LinkFile:
                    {
                        OpenPathInExplorer(GetSaveFilePath());
                    }
                    break;
                case EControllType.OpenCurrent:
                    {
                        if (GuideSystem.getInstance().bDoing)
                        {
                            m_pLogic.Load(GuideSystem.getInstance().DoingTriggerNode.guideGroup );
                            m_pLogic.ExcudeNode(GuideSystem.getInstance().DoingSeqNode);
                        }
                        else
                            EditorUtility.DisplayDialog("提示", "当前没有正在执行的引导", "好的");
                    }
                    break;
                case EControllType.Test:
                    {
                        m_pLogic.TestGuide();
                    }
                    break;
                case EControllType.Stop:
                    {
                        //    OpenPathInExplorer(Data.DataManager.getInstance().Guide.strFilePath);
                        m_pLogic.OverGuide();
                    }
                    break;
                case EControllType.ExportCode:
                    {
                        GuideAutoCode.AutoCode();
                    }
                    break;
                case EControllType.Commit:
                    {
                        if(m_pGuideCsv!=null) m_pGuideCsv.CommitServer();
                    }
                    break;
                case EControllType.Editor:
                    {
                        SettingsService.OpenUserPreferences("Preferences/GameFramework/GuideEditor");
                    }
                    break;
                case EControllType.Expand:
                    {
                        m_pLogic.ExpandNodes(true);
                    }
                    break;
                case EControllType.UnExpand:
                    {
                        m_pLogic.ExpandNodes(false);
                    }
                    break;
                case EControllType.Recode:
                    {
                        if(!m_pLogic.HasTriggerNode())
                        {
                            this.ShowNotification(new GUIContent("请先创建触发节点"), 2);
                            return;
                        }
                        IsRecodeMode = true;
                        titleContent = new GUIContent("引导编辑器-录制模式");
                        m_nLastSelectGOId = -1;
                        if(UnityEngine.EventSystems.EventSystem.current && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
                            m_nLastSelectGOId = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetInstanceID();
                    }
                    break;
                case EControllType.Unrecode:
                    {
                        IsRecodeMode = false;
                        titleContent = new GUIContent("引导编辑器");
                        m_nLastSelectGOId = -1;
                    }
                    break;
            }
        }
        //------------------------------------------------------
        static void OpenPathInExplorer(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            System.Diagnostics.Process[] prpgress = System.Diagnostics.Process.GetProcesses();

            string args = "";
            if (!path.Contains(":/") && !path.Contains(":\\"))
            {
                if ((path[0] == '/') || (path[0] == '\\'))
                    path = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) + path;
                else
                    path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + path;
            }

            args = path.Replace(":/", ":\\");
            args = args.Replace("/", "\\");
            if (path.Contains("."))
            {
                args = string.Format("/Select, \"{0}\"", args);
            }
            else
            {
                if (args[args.Length - 1] != '\\')
                {
                    args += "\\";
                }
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("Explorer.exe", args);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("open", path));
#endif
        }
        //------------------------------------------------------
        public IUserData OnGuideBuildEvent(string strCmd)
        {
            return Core.BuildEventUtl.BuildEvent(null, strCmd);
        }
        //------------------------------------------------------
        public bool OnGuideTriggerEvent(IUserData pEvent, IUserData pTrigger)
        {
            return false;
        }
        //------------------------------------------------------
        public bool OnGuideSign(Framework.Plugin.Guide.BaseNode pNode, CallbackParam param)
        {
            return false;
        }
        //------------------------------------------------------
        public bool OnGuideSuccssedListener(Framework.Plugin.Guide.BaseNode pNode)
        {
            return true;
        }
        //------------------------------------------------------
        public void OnGuideNode(BaseNode pNode)
        {
            m_pLogic.ExcudeNode(pNode);
        }
        //------------------------------------------------------
        public void OnGuideNodeAutoNext(BaseNode pNode)
        {

        }
    }
}
#endif
