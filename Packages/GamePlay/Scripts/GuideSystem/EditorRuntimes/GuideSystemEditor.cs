/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideSystemEditor
作    者:	
描    述:	引导编辑器
*********************************************************************/
#if UNITY_EDITOR
using Framework.Cutscene.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Framework.Guide.Editor
{
    public partial class GuideSystemEditor : EditorWindow, IGuideSystemCallback
    {
        struct MenuData
        {
            public EControllType ctlType;
            public Vector2 mousePos;
            public MenuData(EControllType type, Vector2 pos)
            {
                ctlType = type;
                mousePos = pos;
            }
        }
        public enum EControllType
        {
            New,
            Open,
            OpenCreateSearch,
            Save,

            ReName,
            Copy,
            Parse,
            Delete,
            UnDo,


            OpenCurrent,
            Stop,
            Test,
            Recode,
            Unrecode,

            Expand,
            UnExpand,

            CustomAgent,

            LinkFile,
            Commit,
            Editor,
            Document,
            Count,
        }
        static string[] CONTROLL_NAMES = new string[] 
        { "新建(ctl+n)", "打开(ctl+o)", "创建节点", "保存(ctl+s)", "重新命名", "复制(ctl+c)", "粘贴(ctl+v)", "删除(del)", "回退(ctl+z)", 
            "打开当前引导", "关闭当前引导", "测试", "录制(ctl+r)", "取消录制",
            "全部展开", "全部收起", "自定义", "关联文件", "提交", "编辑设置项", "说明文档" };

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
        public static GuideSystemEditor current;

        GuideEditorLogic m_pLogic = null;
        public GuideEditorLogic logic
        {
            get { return m_pLogic; }
        }
        public static GuideSystemEditor Instance = null;

        private double m_PreviousTime;
        private float m_fDeltaTime;

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


        public static System.Type GamePlugin = null;
        public static MethodInfo EventBuildByTypeMethod = null;
        public static MethodInfo EventDrawMethod = null;
        public static MethodInfo EventPopMethod = null;
        public static MethodInfo EventNameGet = null;

        public static Dictionary<int, NodeAttr> NodeTypes = new Dictionary<int, NodeAttr>();


        public static List<string> ExportTypesPop = new List<string>();
        public static List<int> ExportTypes = new List<int>();
        public static Dictionary<int, NodeAttr> StepTypes = new Dictionary<int, NodeAttr>();

        public static List<string> ExportTriggerTypesPop = new List<string>();
        public static List<int> ExportTriggerTypes = new List<int>();
        public static Dictionary<int, NodeAttr> TriggerTypes = new Dictionary<int, NodeAttr>();

        public static List<string> ExportExcudeTypesPop = new List<string>();
        public static List<int> ExportExcudeTypes = new List<int>();
        public static Dictionary<int, NodeAttr> ExcudeTypes = new Dictionary<int, NodeAttr>();

        public static Dictionary<int, MethodInfo> NodeSceneDraws = new Dictionary<int, MethodInfo>();
        public static Dictionary<int, List<MethodInfo>> NodeMenuCalls = new Dictionary<int, List<MethodInfo>>();

        public static List<string> DisplayTypesPop = new List<string>();
        public static Dictionary<string, GuideDisplayTypeAttribute> DisplayTypes = new Dictionary<string, GuideDisplayTypeAttribute>();
        public static Dictionary<System.Type, GuideDisplayTypeAttribute> TypeDisplayTypes = new Dictionary<System.Type, GuideDisplayTypeAttribute>();
        public static bool IsRecodeMode = false;

        private GameObject m_pRecodeModeGO = null;

        public System.Reflection.MethodInfo OnNodeEditorPreview = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewEnable = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewDisable = null;
        public System.Reflection.MethodInfo OnNodeEditorPreviewVisible = null;
        public System.Reflection.MethodInfo OnCustomRecodeMethod = null;
        //-----------------------------------------------------
        [MenuItem("Tools/引导编辑器 _F1")]
        private static void OpenTool()
        {
            if (Instance != null) return;
            if (Instance == null)
                Instance = EditorWindow.GetWindow<GuideSystemEditor>();
            Instance.titleContent = new GUIContent("引导编辑器");
            Instance.wantsMouseMove = true;
            Instance.Show();
        }
        //------------------------------------------------------

        public string strFile = "";
        //------------------------------------------------------
        protected void OnDisable()
        {
            CustomAgentEditor.CloseEditor();
            m_pLogic.OnDisable();
            Instance = null;
            SceneView.duringSceneGui -= OnSceneFunc;
            GuideSystem.getInstance().UnRegister(this);

            if (OnNodeEditorPreviewDisable != null)
                OnNodeEditorPreviewDisable.Invoke(null, null);

            if (m_pRecodeModeGO)
            {
                if (Application.isPlaying) GameObject.Destroy(m_pRecodeModeGO);
                else GameObject.DestroyImmediate(m_pRecodeModeGO);
                m_pRecodeModeGO = null;
            }
            IsRecodeMode = false;
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
            if (GamePlugin != null)
            {
                GuideSystem.getInstance().Register(this);
            }
            string[] guideDatas = AssetDatabase.FindAssets("t:GuideDatas");
            if (guideDatas != null && guideDatas.Length > 0)
            {
                m_pGuideCsv = AssetDatabase.LoadAssetAtPath<GuideDatas>(AssetDatabase.GUIDToAssetPath(guideDatas[0]));
            }
            if (m_pGuideCsv!=null)
            {
                GuideSystem.getInstance().Register(this);
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
            if (bPop) GuideSystemEditor.Instance.ShowNotification(new GUIContent("保存完成"));
        }
        //------------------------------------------------------
        public string GetSaveFilePath()
        {
            return GuidePreferences.GetSettings().dataSavePath;
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
                menu.AddItem(new GUIContent(CONTROLL_NAMES[(int)i]), false, CreateCtlAction, new MenuData(i, mousePos));
            }
            if(m_pLogic.SelectonCache!=null && m_pLogic.SelectonCache.Count ==1 && m_pLogic.SelectonCache[0].bindNode!=null)
            {
                if (NodeMenuCalls.TryGetValue(m_pLogic.SelectonCache[0].bindNode.GetEnumType(), out var menus) && menus.Count>0)
                {
                    menu.AddSeparator("");
                    foreach(var db in menus)
                    {
                        menu.AddItem(new GUIContent(db.GetCustomAttribute<GuideNodeMenuAttribute>().DisplayName), false, (node) => {
                            db.Invoke(null, new object[] { node });
                        }, m_pLogic.SelectonCache[0].bindNode);
                    }
                }
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
        public bool CanUseTag(ushort tag)
        {
            if(tag<=0 || tag>= 65535)
            {
                EditorUtility.DisplayDialog("提示", "tag 值只能在1到65535区间范围", "好的");
                return false;
            }
            foreach(var db in m_pGuideCsv.allDatas)
            {
                if (db.Value.Tag == 0xffff)
                    continue;
                if(db.Value.Tag == tag)
                {
                    EditorUtility.DisplayDialog("提示", "tag=" + tag + " 以被" + db.Value.Name + "使用,请更换", "好的");
                    return false;
                }
            }
            return true;
        }
        //------------------------------------------------------
        public void OnEvent(Event evt)
        {
            if(evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.Z && evt.control)
                {
                    CreateCtlAction(EControllType.UnDo);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.S && evt.control)
                {
                    CreateCtlAction(EControllType.Save);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.O && evt.control)
                {
                    CreateCtlAction(EControllType.Open);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.C && evt.control)
                {
                    CreateCtlAction(EControllType.Copy);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.V && evt.control)
                {
                    CreateCtlAction(EControllType.Parse);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.N && evt.control)
                {
                    CreateCtlAction(EControllType.New);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.R && evt.control)
                {
                    if(IsRecodeMode)
                    {
                        CreateCtlAction(EControllType.Unrecode);
                    }
                    else 
                        CreateCtlAction(EControllType.Recode);
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Delete)
                {
                    CreateCtlAction(EControllType.Delete);
                    evt.Use();
                }
                if (evt.keyCode == KeyCode.Escape)
                {
                    m_pDataSearcher.Close();
                    evt.Use();
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

            this.Repaint();
            m_PreviousTime = EditorApplication.timeSinceStartup;
        }
        //------------------------------------------------------
        internal void AddRecodeClickStep(GuideGuid guide, string widgetTag, int listIndex)
        {
            StepParam stepParam = new StepParam();
            if (StepTypes.TryGetValue((int)GuideStepType.ClickUI, out stepParam.Data))
            {
                bool bCtl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                if (bCtl && !m_pLogic.HasStepNodeWidgetGuide(guide.guid))
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
                                            field.SetValue(ports[j], guide.guid);
                                    }
                                    else if ( stepParam.Data.argvs[j].attr.argvName == "widgetTag")
                                    {
                                        if (!string.IsNullOrEmpty(widgetTag))
                                        {
                                            var field = ports[j].GetType().GetField("strValue", BindingFlags.Instance | BindingFlags.NonPublic);
                                            if (field != null)
                                                field.SetValue(ports[j], widgetTag);
                                        }           
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "bMask")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                            field.SetValue(ports[j], 1);
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "maskColor")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                            field.SetValue(ports[j], 0);
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "index")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                            field.SetValue(ports[j], listIndex+1);
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "RayTest")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                        {
                                            int RayTest = 1;
                                            if(guide.transform is RectTransform)
                                            {
                                                RectTransform rectTrans = guide.transform as RectTransform;
                                                Camera uiCamera = null;
                                                var canvas = Canvas.FindObjectsOfType<Canvas>();
                                                for (int k = 0; k < canvas.Length; ++k)
                                                {
                                                    if (canvas[k].worldCamera != null)
                                                    {
                                                        uiCamera = canvas[k].worldCamera;
                                                        break;
                                                    }
                                                }
                                                if (uiCamera != null)
                                                {
                                                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTrans.position);
                                                    PointerEventData pTestEventData = new PointerEventData(EventSystem.current);
                                                    pTestEventData.position = screenPos;
                                                    var rayTestResults = new List<RaycastResult>(4);
                                                    EventSystem.current.RaycastAll(pTestEventData, rayTestResults);
                                                    bool bHit = false;
                                                    for (int k = 0; k < rayTestResults.Count; ++k)
                                                    {
                                                        if (rayTestResults[k].gameObject == guide.gameObject)
                                                        {
                                                            bHit = true;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            var parent = rayTestResults[k].gameObject.transform;
                                                            while (parent != null)
                                                            {
                                                                if (parent == guide.transform)
                                                                {
                                                                    bHit = true;
                                                                    break;
                                                                }
                                                                parent = parent.parent;
                                                            }
                                                            if (bHit) break;
                                                        }
                                                    }
                                                    RayTest = bHit ? 1 : 0;
                                                }
                                            }


                                            field.SetValue(ports[j], RayTest);
                                        }
                                    }
                                }
                            }
                            lastNode.linkOutPort.baseNode.Links.Clear();
                            lastNode.linkOutPort.baseNode.Links.Add(newNode);
                            this.ShowNotification(new GUIContent("成功录制了控件\"" + guide.name + "\""), 3);
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
        internal void AddRecodeClickZoom(Vector3 worldPos)
        {
            StepParam stepParam = new StepParam();
            if (StepTypes.TryGetValue((int)GuideStepType.ClickZoom, out stepParam.Data))
            {
                bool bCtl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool bShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); 
                if (bCtl && bShift)
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
                                    if (stepParam.Data.argvs[j].attr.argvName == "startPosX")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null) field.SetValue(ports[j], (int)(worldPos.x * 1000));
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "startPosY")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null) field.SetValue(ports[j], (int)(worldPos.y * 1000));
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "startPosZ")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null) field.SetValue(ports[j], (int)(worldPos.z*1000));
                                    }
                                    else if (stepParam.Data.argvs[j].attr.argvName == "IsStart3DPos")
                                    {
                                        var field = ports[j].GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (field != null)
                                            field.SetValue(ports[j], 1);
                                    }
                                }
                            }
                            lastNode.linkOutPort.baseNode.Links.Clear();
                            lastNode.linkOutPort.baseNode.Links.Add(newNode);
                            this.ShowNotification(new GUIContent("成功录制了3D位置区域点击\"" + worldPos + "\""), 3);
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
            GuideSystemEditor[] windows = Resources.FindObjectsOfTypeAll<GuideSystemEditor>();
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
        static void AddDisplayAttr(System.Type type, string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) displayName = type.Name;
            if(!TypeDisplayTypes.ContainsKey(type))
            {
                var attr = new GuideDisplayTypeAttribute(type, displayName);
                if (attr.callDisplayType == null) attr.callDisplayType = attr.displayType;
                TypeDisplayTypes[type] = attr;
                DisplayTypes[displayName] = attr;
                DisplayTypesPop.Add(displayName);
            }
        }
        //------------------------------------------------------
        public static void InitDisplayAttr()
        {
            DisplayTypesPop.Clear();
            DisplayTypes.Clear();
            TypeDisplayTypes.Clear();
            AddDisplayAttr(typeof(int), "Int");
            AddDisplayAttr(typeof(float), "Float");
            AddDisplayAttr(typeof(bool), "Bool");
            AddDisplayAttr(typeof(string), "String");
            AddDisplayAttr(typeof(Color), "Color");
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type enumType = types[i];
                    if (enumType.IsDefined(typeof(GuideDisplayTypeAttribute)))
                    {
                        GuideDisplayTypeAttribute[] attrs = (GuideDisplayTypeAttribute[])enumType.GetCustomAttributes<GuideDisplayTypeAttribute>();
                        for (int k = 0; k < attrs.Length; ++k)
                        {
                            var attr = attrs[k];
                            string displayName = attr.popDisplayName;
                            if (string.IsNullOrEmpty(displayName)) displayName = enumType.Name;
                            if (attr.displayType == null) attr.displayType = enumType;
                            attr.callDisplayType = enumType;
                            if (!TypeDisplayTypes.ContainsKey(attr.displayType))
                            {
                                TypeDisplayTypes[attr.displayType] = attr;
                                DisplayTypes[displayName] = attr;
                                DisplayTypesPop.Add(displayName);
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        void CheckAssemblyAI()
        {
            OnNodeEditorPreview = null;
            OnNodeEditorPreviewEnable = null;
            OnNodeEditorPreviewDisable = null;
            OnNodeEditorPreviewVisible = null;
            NodeSceneDraws.Clear();
            NodeMenuCalls.Clear();

            OnCustomRecodeMethod = null;

            InitDisplayAttr();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type enumType = types[i];
                    if(enumType.IsDefined(typeof(GuideEditorPreviewAttribute)))
                    {
                        var method = enumType.GetCustomAttribute<GuideEditorPreviewAttribute>();
                        if(method.type>0)
                        {
                            if(!string.IsNullOrEmpty(method.CallMethod))
                            {
                                var drawSceneMethod = enumType.GetMethod(method.CallMethod);
                                if (drawSceneMethod != null)
                                    NodeSceneDraws[method.type] = drawSceneMethod;
                            }
                            var methods = enumType.GetMethods();
                            foreach(var func in methods)
                            {
                                if(func.IsDefined(typeof(GuideNodeMenuAttribute)))
                                {
                                    var nodeMenu = func.GetCustomAttribute<GuideNodeMenuAttribute>();
                                    if (func != null && func.GetParameters() != null && func.GetParameters().Length == 1 &&
                                        func.GetParameters()[0].ParameterType == typeof(BaseNode))
                                    {
                                        if (!NodeMenuCalls.TryGetValue(method.type, out var menuList))
                                        {
                                            menuList = new List<MethodInfo>();
                                            NodeMenuCalls[method.type] = menuList;
                                        }
                                        menuList.Add(func);
                                    }
                                }
                            }
                        }
                    }

                    if(OnCustomRecodeMethod == null && enumType.IsDefined(typeof(GuideCustomRecodeAttribute)))
                    {
                        OnCustomRecodeMethod = enumType.GetMethod(enumType.GetCustomAttribute<GuideCustomRecodeAttribute>().callMethod, BindingFlags.Instance| BindingFlags.Static| BindingFlags.Public| BindingFlags.NonPublic);
                    }

                    if (enumType.IsDefined(typeof(GuideDisplayTypeAttribute)))
                    {
                        GuideDisplayTypeAttribute[] attrs = (GuideDisplayTypeAttribute[])enumType.GetCustomAttributes<GuideDisplayTypeAttribute>();
                        for(int k =0; k < attrs.Length; ++k)
                        {               
                            var attr = attrs[k];
                            string displayName = attr.popDisplayName;
                            if(string.IsNullOrEmpty(displayName)) displayName = enumType.Name;
                            if(attr.displayType == null) attr.displayType = enumType;
                            attr.callDisplayType = enumType;
                            if (!TypeDisplayTypes.ContainsKey(attr.displayType))
                            {
                                TypeDisplayTypes[attr.displayType] = attr;
                                DisplayTypes[displayName] = attr;
                                DisplayTypesPop.Add(displayName);
                            }
                        }
                    }
                }
                if (OnNodeEditorPreview != null) break;
            }
            CustomAgentUtil.RefreshEditorData();

            if(OnNodeEditorPreview == null)
            {
                OnNodeEditorPreview =  typeof(GuideNodePreviewEditor).GetMethod("OnEditorPreview");
                OnNodeEditorPreviewEnable = typeof(GuideNodePreviewEditor).GetMethod("OnEnable");
                OnNodeEditorPreviewDisable = typeof(GuideNodePreviewEditor).GetMethod("OnDisable");
                OnNodeEditorPreviewVisible = typeof(GuideNodePreviewEditor).GetMethod("OnVisible");
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
        public void OnGuideGroupStatus(BaseNode node, bool bDoing, bool bCheckRecord)
        {
            if (!bDoing) m_pLogic.OnStopDoingGuide();
        }
        //------------------------------------------------------
        public void CreateCtlAction(object var)
        {
            EControllType ctl = EControllType.New;
            Vector2 mousePos = Vector2.zero;
            if(var is MenuData)
            {
                MenuData menu = (MenuData)var;
                ctl = menu.ctlType;
                mousePos = menu.mousePos;
            }
            else
                ctl = (EControllType)var;
            switch (ctl)
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
                            GuideSystem.getInstance().datas = m_pGuideCsv.allDatas;
                            GuideSystem.getInstance().RefreshTriggers();
                        m_pLogic.SyncCurGroup();
                        }
                        break;
                    case EControllType.OpenCreateSearch:
                        {
                            FuncContextMenu(mousePos);
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
                        this.ShowNotification(new GUIContent("目前功能暂时不支持，敬请期待"), 1.0f);
                        //     m_pLogic.UnRedo();
                    }
                        break;
                    case EControllType.LinkFile:
                        {
                            OpenPathInExplorer(GetSaveFilePath());
                            if (m_pLogic.GetCurGroup() != null && !string.IsNullOrEmpty(m_pLogic.GetCurGroup().strFile))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_pLogic.GetCurGroup().strFile));
                            }
                        }
                        break;
                    case EControllType.OpenCurrent:
                        {
                            if (GuideSystem.getInstance().bDoing)
                            {
                                m_pLogic.Load(GuideSystem.getInstance().DoingTriggerNode.guideGroup);
                                m_pLogic.ExcudeNode(GuideSystem.getInstance().DoingSeqNode);
                            }
                            else
                                EditorUtility.DisplayDialog("提示", "当前没有正在执行的引导", "好的");
                        }
                        break;
                    case EControllType.Test:
                        {
                            m_pLogic.Save();
                            GuideSystem.getInstance().datas = m_pGuideCsv.allDatas;
                            GuideSystem.getInstance().RefreshTriggers();
                            m_pLogic.TestGuide();
                        }
                        break;
                    case EControllType.Stop:
                        {
                            //    OpenPathInExplorer(Data.DataManager.getInstance().Guide.strFilePath);
                            m_pLogic.OverGuide();
                        }
                        break;
                    case EControllType.CustomAgent:
                        {
                            CustomAgentEditor.Open();
                        }
                        break;
                    case EControllType.Commit:
                        {
                        string saveToPath = Framework.Guide.Editor.GuidePreferences.GetSettings().dataSavePath;
                        GuideEditorUtil.CommitGit(saveToPath);
                        //    if (m_pGuideCsv != null) m_pGuideCsv.CommitServer();
                        }
                        break;
                    case EControllType.Editor:
                        {
                            SettingsService.OpenUserPreferences("Preferences/GuideSystemEditor");
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
                            if (!m_pLogic.HasTriggerNode())
                            {
                                this.ShowNotification(new GUIContent("请先创建触发节点"), 2);
                                return;
                            }
                            if (m_pRecodeModeGO == null)
                            {
                                m_pRecodeModeGO = new GameObject("GuideRecodeMode");
                                m_pRecodeModeGO.hideFlags = HideFlags.HideAndDontSave;
                                m_pRecodeModeGO.AddComponent<GudieRecodeMode>();
                            }
                            IsRecodeMode = true;
                            titleContent = new GUIContent("引导编辑器-录制模式");
                        this.ShowNotification(new GUIContent("进入录制模式\r\n1.请按住control键，然后鼠标在Game 视图中点击ui，即可录制\r\n1.请按住control键+shift键，然后鼠标在Game 视图中点击3D物件，即可录制3D坐标"), 3);
                    }
                    break;
                    case EControllType.Unrecode:
                        {
                            if(IsRecodeMode)
                                this.ShowNotification(new GUIContent("退出录制模式"), 1);
                            IsRecodeMode = false;
                            titleContent = new GUIContent("引导编辑器");
                    }
                    break;
                case EControllType.Document:
                    {
                        Application.OpenURL("https://docs.qq.com/doc/DTFFNT3JsRXRTcENY");
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

        public IUserData OnGuideBuildEvent(string strCmd)
        {
            if (GamePlugin != null)
            {
                MethodInfo meth = GamePlugin.GetMethod("OnGuideBuildEvent");
                if (meth != null)
                {
                    return (IUserData)meth.Invoke(null, new object[] { strCmd });
                }
            }
            return null;
        }
        //------------------------------------------------------
        public bool OnTriggerGuideEvent(IUserData pEvent, IUserData pTrigger)
        {
            return false;
        }
        //------------------------------------------------------
        public bool OnGuideCheckSign(Framework.Guide.BaseNode pNode, CallbackParam param)
        {
            return false;
        }
        //------------------------------------------------------
        public bool OnGuideSuccssedListener(Framework.Guide.BaseNode pNode)
        {
            return true;
        }
        //------------------------------------------------------
        public void OnGuideExecuteNode(BaseNode pNode)
        {
            m_pLogic.ExcudeNode(pNode);
        }
        //------------------------------------------------------
        public void OnGuideNodeAutoNext(BaseNode pNode)
        {

        }
        //------------------------------------------------------
        public string OnGuideLanguageContent(string contentId)
        {
            return contentId;
        }
        //------------------------------------------------------
        public bool OnGuideLoadAsset(UnityEngine.Object pCall, string file, bool bAsync)
        {
            return false;
        }
        //------------------------------------------------------
        public bool OnGuideUnLoadAsset(string file, UnityEngine.Object callback)
        {
            return false;
        }
    }
}
#endif
