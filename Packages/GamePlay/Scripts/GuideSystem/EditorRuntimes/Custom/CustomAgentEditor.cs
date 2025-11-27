/********************************************************************
生成日期:	06:30:2025
类    名: 	CustomAgentEditor
作    者:	HappLI
描    述:	引导自定义步骤、执行器、触发器 编辑窗口
*********************************************************************/
#if UNITY_EDITOR
using Framework.Guide.Editor;
using Framework.Guide;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;
using static Framework.Guide.Editor.AssetTree;
using static UnityEditor.UIElements.ToolbarMenu;

namespace Framework.Cutscene.Editor
{
    public class CustomAgentEditor : EditorWindow
    {
        public class AgentItem : AssetTree.ItemData
        {
            public ETab Tab;
            public GuideCustomAgent.AgentUnit unit;
            public AgentItem(GuideCustomAgent.AgentUnit unit, ETab tab)
            {
                this.Tab = tab;
                this.unit = unit;
                name = unit.name + "[" + unit.customType + "]";
            }
            public override Color itemColor()
            {
                if (unit.customType <= 0 || string.IsNullOrEmpty(unit.name))
                    return Color.red;
                return Color.white;
            }
        }
        public enum ETab
        {
            CustomTrigger,
            CustomStep,
            CustomExecute,
            None,
        }
        string m_strNewName = "";
        ETab m_eTab = ETab.CustomTrigger;
        AssetTree m_pTriggerTreeview;
        AssetTree m_pStepTreeview;
        AssetTree m_pExecuteTreeview;
        AgentItem m_pSelect = null;

        bool m_bExpandInput = false;
        bool m_bExpandOutput = false;
        Vector2 m_scoll = Vector2.zero;
        HashSet<uint> m_vTriggerTypes = new HashSet<uint>();
        HashSet<string> m_vTriggerNames = new HashSet<string>();
        List<GuideCustomAgent.AgentUnit> m_vTriggers = new List<GuideCustomAgent.AgentUnit>();
        HashSet<uint> m_vStepTypes = new HashSet<uint>();
        HashSet<string> m_vStepNames = new HashSet<string>();
        List<GuideCustomAgent.AgentUnit> m_vSteps = new List<GuideCustomAgent.AgentUnit>();

        HashSet<uint> m_vExecuteTypes = new HashSet<uint>();
        HashSet<string> m_vExecuteNames = new HashSet<string>();
        List<GuideCustomAgent.AgentUnit> m_vExecutes = new List<GuideCustomAgent.AgentUnit>();

        float m_fRightWidth = 0;

        //--------------------------------------------------------
        static CustomAgentEditor ms_pInstnace = null;
        public static void Open()
        {
            if (ms_pInstnace != null)
            {
                ms_pInstnace.Focus();
                return;
            }
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
                return;
            }
            CustomAgentEditor window = EditorWindow.GetWindow<CustomAgentEditor>();
            window.titleContent = new GUIContent("自定义行为参数编辑器");
        }
        //--------------------------------------------------------
        public static void CloseEditor()
        {
            if (ms_pInstnace != null)
            {
                ms_pInstnace.Close();
                ms_pInstnace = null;
            }
        }
        //--------------------------------------------------------
        void OnEnable()
        {
            ms_pInstnace = this;
            this.minSize = new Vector2(800, 400);
            CustomAgentUtil.Init(true);
            m_pTriggerTreeview = new AssetTree(new string[] { "类型", "名称" });
            m_pStepTreeview = new AssetTree(new string[] { "类型", "名称" });
            m_pExecuteTreeview = new AssetTree(new string[] { "类型", "名称" });
            m_pTriggerTreeview.OnSelectChange += OnSelectChange;
            m_pStepTreeview.OnSelectChange += OnSelectChange;
            m_pExecuteTreeview.OnSelectChange += OnSelectChange;
            m_pTriggerTreeview.OnItemDoubleClick += OnSelectChange;
            m_pStepTreeview.OnItemDoubleClick += OnSelectChange;
            m_pExecuteTreeview.OnItemDoubleClick += OnSelectChange;
            m_pTriggerTreeview.OnCellDraw += OnDrawItem;
            m_pStepTreeview.OnCellDraw += OnDrawItem;
            m_pExecuteTreeview.OnCellDraw += OnDrawItem;
            m_vTriggers = new List<GuideCustomAgent.AgentUnit>(CustomAgentUtil.GetTriggerList());
            m_vSteps = new List<GuideCustomAgent.AgentUnit>(CustomAgentUtil.GetStepList());
            m_vExecutes = new List<GuideCustomAgent.AgentUnit>(CustomAgentUtil.GetExecuteList());
            Refresh();
        }
        //--------------------------------------------------------
        void OnDisable()
        {
            ms_pInstnace = null;
        }
        //--------------------------------------------------------
        void Refresh(ETab tab = ETab.None)
        {
            if(tab == ETab.None || tab == ETab.CustomTrigger)
            {
                m_vTriggerTypes.Clear();
                m_vTriggerNames.Clear();
                m_pTriggerTreeview.BeginTreeData();
                for (int i = 0; i < m_vTriggers.Count; ++i)
                {
                    var item = m_vTriggers[i];
                    m_vTriggerTypes.Add(item.customType);
                    m_vTriggerNames.Add(item.name);
                    m_pTriggerTreeview.AddData(new AgentItem(item, ETab.CustomTrigger) { id = (int)item.customType });
                }
                m_pTriggerTreeview.EndTreeData();
            }
            if (tab == ETab.None || tab == ETab.CustomStep)
            {
                m_vStepTypes.Clear();
                m_vStepNames.Clear();
                m_pStepTreeview.BeginTreeData();
                for (int i = 0; i < m_vSteps.Count; ++i)
                {
                    var item = m_vSteps[i];
                    m_vStepTypes.Add(item.customType);
                    m_vStepNames.Add(item.name);
                    m_pStepTreeview.AddData(new AgentItem(item, ETab.CustomStep) { id = (int)item.customType });
                }
                m_pStepTreeview.EndTreeData();
            }
            if (tab == ETab.None || tab == ETab.CustomExecute)
            {
                m_vExecuteTypes.Clear();
                m_vExecuteNames.Clear();
                m_pExecuteTreeview.BeginTreeData();
                for (int i = 0; i < m_vExecutes.Count; ++i)
                {
                    var item = m_vExecutes[i];
                    m_vExecuteTypes.Add(item.customType);
                    m_vExecuteNames.Add(item.name);
                    m_pExecuteTreeview.AddData(new AgentItem(item, ETab.CustomStep) { id = (int)item.customType });
                }
                m_pExecuteTreeview.EndTreeData();
            }
        }
        //--------------------------------------------------------
        void OnSelectChange(AssetTree.ItemData item)
        {
            if (item == null)
                return;
            m_pSelect = item as AgentItem;
        }
        //--------------------------------------------------------
        bool OnDrawItem(Rect r, TreeItemData t, int c, bool s, bool f)
        {
            var agentItem = t.data as AgentItem;
            agentItem.name = agentItem.unit.name + "[" + agentItem.unit.customType + "]";
            if (c == 0)
            {
                EditorGUI.LabelField(r, agentItem.unit.customType.ToString());
            }
            else if (c == 1)
            {
                EditorGUI.LabelField(r, agentItem.unit.name);
            }
            return true;
        }
        //--------------------------------------------------------
        void OnGUI()
        {
            float leftWidth = 300 - 1;
            GUILayout.BeginArea(new Rect(0, 0, leftWidth, 25));
            m_eTab = (ETab)GUILayout.Toolbar((int)m_eTab, new string[] { "触发器", "步骤器", "执行器" });
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 30, leftWidth, position.height - 30));
            if (m_eTab == ETab.CustomTrigger)
            {
                if (m_pTriggerTreeview != null)
                {
                    m_pTriggerTreeview.GetColumn(1).width = leftWidth* 2.0f / 3.0f;
                    m_pTriggerTreeview.GetColumn(0).width = leftWidth  / 3.0f;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("搜索", GUILayout.Width(30));
                    m_pTriggerTreeview.searchString = EditorGUILayout.TextField(m_pTriggerTreeview.searchString);
                    EditorGUILayout.EndHorizontal();
                    m_pTriggerTreeview.OnGUI(new Rect(0, 20, leftWidth, position.height - 80));
                }
            }
            else if (m_eTab == ETab.CustomStep)
            {
                if (m_pStepTreeview != null)
                {
                    m_pStepTreeview.GetColumn(1).width = leftWidth * 2.0f / 3.0f;
                    m_pStepTreeview.GetColumn(0).width = leftWidth / 3.0f;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("搜索", GUILayout.Width(30));
                    m_pStepTreeview.searchString = EditorGUILayout.TextField(m_pStepTreeview.searchString);
                    EditorGUILayout.EndHorizontal();
                    m_pStepTreeview.OnGUI(new Rect(0, 20, leftWidth, position.height - 80));
                }
            }
            else if (m_eTab == ETab.CustomExecute)
            {
                if (m_pExecuteTreeview != null)
                {
                    m_pExecuteTreeview.GetColumn(1).width = leftWidth * 2.0f / 3.0f;
                    m_pExecuteTreeview.GetColumn(0).width = leftWidth / 3.0f;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("搜索", GUILayout.Width(30));
                    m_pExecuteTreeview.searchString = EditorGUILayout.TextField(m_pExecuteTreeview.searchString);
                    EditorGUILayout.EndHorizontal();
                    m_pExecuteTreeview.OnGUI(new Rect(0, 20, leftWidth, position.height - 80));
                }
            }
            GUILayout.EndArea();

      //      UIDrawUtils.DrawColorLine(new Vector3(leftWidth, position.height - 81), new Vector3(position.width, position.height - 81), Color.white);
            GUILayout.BeginArea(new Rect(0, position.height-30, leftWidth, 30));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", new GUILayoutOption[] { GUILayout.Width(leftWidth / 2) }))
            {
                CustomAgentUtil.RefreshData(m_vTriggers, m_vSteps,m_vExecutes);
            }
            if (GUILayout.Button("提交", new GUILayoutOption[] { GUILayout.Width(leftWidth / 2) }))
            {
               // CustomAgentUtil.CommitGit();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            m_fRightWidth = position.width - leftWidth - 1;
            //     UIDrawUtils.DrawColorLine(new Vector3(rightWidth,0), new Vector3(rightWidth, position.height), Color.white);
            GUILayout.BeginArea(new Rect(leftWidth + 1, 5, m_fRightWidth, position.height-5));
            if (m_eTab == ETab.CustomTrigger)
            {
                DrawCustomTriggers();
            }
            else if (m_eTab == ETab.CustomStep)
            {
                DrawCustomSteps();
            }
            else if (m_eTab == ETab.CustomExecute)
            {
                DrawCustomExecutes();
            }
            DrawSelect();
            GUILayout.EndArea();

            // UIDrawUtils.DrawColorLine(new Vector3(rightWidth, 26), new Vector3(position.width, 26), Color.white);
        }
        //--------------------------------------------------------
        void DrawSelect()
        {
            float posX = position.width / 2;
            if (m_eTab == ETab.CustomTrigger)
            {
                EditorGUILayout.HelpBox("触发器：为业务层，需要进行业务层调用。一个引导的起始触发节点", MessageType.Info);
                EditorGUILayout.HelpBox("触发器自定义类型必须在:" + (int)GuideTriggerDef.CustomBegin + " - " + ((int)GuideTriggerDef.CustomEnd-1) +  " 区间内", MessageType.Warning);
                if(m_pSelect!=null)
                {
                    DrawAgentUnit(m_pSelect.unit, m_vTriggers, m_vTriggerTypes, m_vTriggerNames, (int)GuideTriggerDef.CustomBegin, (int)GuideTriggerDef.CustomEnd);
                    m_scoll = EditorGUILayout.BeginScrollView(m_scoll, new GUILayoutOption[] { GUILayout.MaxHeight(position.height - 100) });
                    if (m_pSelect != null)
                    {
                        {
                            GUILayout.BeginHorizontal();
                            m_bExpandInput = EditorGUILayout.Foldout(m_bExpandInput, "输入参数");
                            if (GUILayout.Button("添加", GUILayout.Width(50)))
                            {
                                var list = m_pSelect.unit.inputs == null ? new List<GuideCustomAgent.AgentArgv>() : new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(m_pSelect.unit.inputs);
                                list.Add(new GuideCustomAgent.AgentArgv() { name = "NewParam", Flag = EArgvFalg.PortAll, canEdit = true });
                                m_pSelect.unit.inputs = list.ToArray();
                                m_bExpandInput = true;
                            }
                            GUILayout.EndHorizontal();
                            if (m_bExpandInput)
                                m_pSelect.unit.inputs = DrawAgentUnitParams(m_pSelect.unit, m_pSelect.unit.inputs);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.BeginArea(new Rect(2, position.height - 30, position.width / 2 - 10, 30));
                    if (GUILayout.Button("删除触发器", new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 10) }))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确定删除自定义触发器?", "删除", "再想想"))
                        {
                            m_vTriggers.Remove(m_pSelect.unit);
                            m_pSelect = null;
                            Refresh(ETab.CustomTrigger);
                        }
                    }
                    GUILayout.EndArea();
                }
                
            }
            else if (m_eTab == ETab.CustomStep)
            {
                EditorGUILayout.HelpBox("步骤器：只有执行成功、满足对应的条件后，才会执行下一节点。步骤器，需要实现:\r\n1.需要实现OnGuideExecuteNode 的回调逻辑.\r\n2.需要实现OnGuideCheckSign信号检测结果逻辑，如果返回true，则会执行下一步.\r\n3.需要实现OnGuideNodeAutoNext回调函数，如果有自动执行的逻辑，这需要实现对应的业务行为", MessageType.Info);
                EditorGUILayout.HelpBox("步骤器自定义类型必须从:" + (int)GuideStepType.CustomBegin + " - " + ((int)GuideStepType.CustomEnd-1) + " 区间内", MessageType.Warning);
                if (m_pSelect != null)
                {
                    DrawAgentUnit(m_pSelect.unit, m_vSteps, m_vStepTypes, m_vStepNames, (int)GuideStepType.CustomBegin, (int)GuideStepType.CustomEnd);
                    m_scoll = EditorGUILayout.BeginScrollView(m_scoll, new GUILayoutOption[] { GUILayout.MaxHeight(position.height - 100) });
                    {
                        GUILayout.BeginHorizontal();
                        m_bExpandInput = EditorGUILayout.Foldout(m_bExpandInput, "输入参数");
                        if (GUILayout.Button("添加", GUILayout.Width(50)))
                        {
                            var list = m_pSelect.unit.inputs == null ? new List<GuideCustomAgent.AgentArgv>() : new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(m_pSelect.unit.inputs);
                            list.Add(new GuideCustomAgent.AgentArgv() { name = "NewParam", Flag = EArgvFalg.All });
                            m_pSelect.unit.inputs = list.ToArray();
                            m_bExpandInput = true;
                        }
                        GUILayout.EndHorizontal();
                        if (m_bExpandInput) m_pSelect.unit.inputs = DrawAgentUnitParams(m_pSelect.unit, m_pSelect.unit.inputs);
                    }
                    GUILayout.Space(5);
                    {
                        GUILayout.BeginHorizontal();
                        m_bExpandOutput = EditorGUILayout.Foldout(m_bExpandOutput, "输出参数");
                        if (GUILayout.Button("添加", GUILayout.Width(50)))
                        {
                            var list = m_pSelect.unit.outputs == null ? new List<GuideCustomAgent.AgentArgv>() : new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(m_pSelect.unit.outputs);
                            list.Add(new GuideCustomAgent.AgentArgv() { name = "NewParam", Flag = EArgvFalg.GetAndPort, canEdit = false });
                            m_pSelect.unit.outputs = list.ToArray();
                            m_bExpandOutput = true;
                        }
                        GUILayout.EndHorizontal();
                        if (m_bExpandOutput) m_pSelect.unit.outputs = DrawAgentUnitParams(m_pSelect.unit, m_pSelect.unit.outputs);
                    }
                    EditorGUILayout.EndScrollView();

                    GUILayout.BeginArea(new Rect(2, position.height - 30, position.width / 2 - 10, 30));
                    if (GUILayout.Button("删除步骤器", new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 10) }))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确定删除改自定义步骤器?", "删除", "再想想"))
                        {
                            m_vSteps.Remove(m_pSelect.unit);
                            m_pSelect = null;
                            Refresh(ETab.CustomStep);
                        }
                    }
                    GUILayout.EndArea();
                }
            }
            else if (m_eTab == ETab.CustomExecute)
            {
                EditorGUILayout.HelpBox("执行器：为调用业务逻辑，需要返回计算的值.\r\n比如获取背包物品个数，返回值为物品个数。\r\n1.需要实现OnGuideExecuteNode 回调函数，执行对应的业务逻辑行为，然后将对应的返回值设置给port(pNode._Ports[xxx].fillValue=xxx)", MessageType.Info);
                EditorGUILayout.HelpBox("执行器自定义类型必须从:" + (int)GuideExcudeType.CustomBegin + " - " + ((int)(GuideExcudeType.CustomEnd)-1) + " 区间内", MessageType.Warning);
                if (m_pSelect != null)
                {
                    DrawAgentUnit(m_pSelect.unit, m_vExecutes, m_vExecuteTypes, m_vExecuteNames, (int)GuideExcudeType.CustomBegin, (int)GuideExcudeType.CustomEnd);
                    m_scoll = EditorGUILayout.BeginScrollView(m_scoll, new GUILayoutOption[] { GUILayout.MaxHeight(position.height - 100) });
                    {
                        GUILayout.BeginHorizontal();
                        m_bExpandInput = EditorGUILayout.Foldout(m_bExpandInput, "输入参数");
                        if (GUILayout.Button("添加", GUILayout.Width(50)))
                        {
                            var list = m_pSelect.unit.inputs == null ? new List<GuideCustomAgent.AgentArgv>() : new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(m_pSelect.unit.inputs);
                            list.Add(new GuideCustomAgent.AgentArgv() { name = "NewParam", Flag = EArgvFalg.All });
                            m_pSelect.unit.inputs = list.ToArray();
                            m_bExpandInput = true;
                        }
                        GUILayout.EndHorizontal();
                        if (m_bExpandInput) m_pSelect.unit.inputs = DrawAgentUnitParams(m_pSelect.unit, m_pSelect.unit.inputs);
                    }
                    GUILayout.Space(5);
                    {
                        GUILayout.BeginHorizontal();
                        m_bExpandOutput = EditorGUILayout.Foldout(m_bExpandOutput, "输出参数");
                        if (GUILayout.Button("添加", GUILayout.Width(50)))
                        {
                            var list = m_pSelect.unit.outputs == null ? new List<GuideCustomAgent.AgentArgv>() : new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(m_pSelect.unit.outputs);
                            list.Add(new GuideCustomAgent.AgentArgv() { name = "NewParam", Flag = EArgvFalg.GetAndPort, canEdit = false });
                            m_pSelect.unit.outputs = list.ToArray();
                            m_bExpandOutput = true;
                        }
                        GUILayout.EndHorizontal();
                        if (m_bExpandOutput) m_pSelect.unit.outputs = DrawAgentUnitParams(m_pSelect.unit, m_pSelect.unit.outputs);
                    }
                    EditorGUILayout.EndScrollView();

                    GUILayout.BeginArea(new Rect(2, position.height - 30, position.width / 2 - 10, 30));
                    if (GUILayout.Button("删除执行器", new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 10) }))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确定删除改自定义执行器?", "删除", "再想想"))
                        {
                            m_vExecutes.Remove(m_pSelect.unit);
                            m_pSelect = null;
                            Refresh(ETab.CustomExecute);
                        }
                    }
                    GUILayout.EndArea();
                }
            }
        }
        //--------------------------------------------------------
        void DrawCustomTriggers()
        {
            GUILayout.BeginHorizontal();
            m_strNewName = EditorGUILayout.TextField(m_strNewName);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_strNewName) || m_vTriggerNames.Contains(m_strNewName));
            if (GUILayout.Button("新增触发器", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                GuideCustomAgent.AgentUnit unit = new GuideCustomAgent.AgentUnit();
                unit.name = m_strNewName;
                unit.customType = 0; // 默认类型为0
                m_vTriggers.Add(unit);
                Refresh(ETab.CustomTrigger);
                m_strNewName = "";
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
        //--------------------------------------------------------
        void DrawCustomSteps()
        {
            GUILayout.BeginHorizontal();
            m_strNewName = EditorGUILayout.TextField(m_strNewName);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_strNewName) || m_vStepNames.Contains(m_strNewName));
            if (GUILayout.Button("新增步骤器", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                GuideCustomAgent.AgentUnit unit = new GuideCustomAgent.AgentUnit();
                unit.name = m_strNewName;
                unit.customType = 0; // 默认类型为0
                m_vSteps.Add(unit);
                Refresh(ETab.CustomStep);
                m_strNewName = "";
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
        //--------------------------------------------------------
        void DrawCustomExecutes()
        {
            GUILayout.BeginHorizontal();
            m_strNewName = EditorGUILayout.TextField(m_strNewName);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_strNewName) || m_vExecuteNames.Contains(m_strNewName));
            if (GUILayout.Button("新增执行器", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                GuideCustomAgent.AgentUnit unit = new GuideCustomAgent.AgentUnit();
                unit.name = m_strNewName;
                unit.customType = 0; // 默认类型为0
                m_vExecutes.Add(unit);
                Refresh(ETab.CustomExecute);
                m_strNewName = "";
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
        //--------------------------------------------------------
        void DrawAgentUnit(GuideCustomAgent.AgentUnit unit, List<GuideCustomAgent.AgentUnit> vList, HashSet<uint> vTypes, HashSet<string> vNames, int beginType, int endType)
        {
            string curName = unit.name;
            curName = EditorGUILayout.DelayedTextField("事件名称",unit.name);
            if(curName!= unit.name && !string.IsNullOrEmpty(curName))
            {
                if(!vNames.Contains(curName))
                {
                    vNames.Remove(unit.name);
                    vNames.Add(curName);
                    unit.name = curName;
                }
            }
            uint curType = unit.customType;
            curType = (uint)EditorGUILayout.IntField("事件类型",(int)unit.customType);
            if (curType != unit.customType && curType>0)
            {
                if(!vTypes.Contains(curType) && !CustomAgentUtil.HasAgent(curType) && curType >= beginType && curType < endType)
                {
                    vTypes.Remove(unit.customType);
                    vTypes.Add(curType);
                    unit.customType = curType;
                }
            }
        }
        //--------------------------------------------------------
        GuideCustomAgent.AgentArgv[] DrawAgentUnitParams(GuideCustomAgent.AgentUnit unit, GuideCustomAgent.AgentArgv[] vParams)
        {
            if (vParams == null)
                return vParams;

            float width = m_fRightWidth - 20;
            float controlWidth = 100;
            float editColWidth = 70;
            float displayWidth = 100;
            float headWidth = (width - controlWidth- editColWidth- displayWidth) / 3;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("参数名", GUILayout.Width(headWidth));          
            GUILayout.Label("Port定义", GUILayout.Width(headWidth));
            GUILayout.Label("显示规则", GUILayout.Width(displayWidth));
            GUILayout.Label(new GUIContent("位标志","当显示规则为枚举时，才有位标志编辑"), GUILayout.Width(headWidth));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < vParams.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                // 参数名
                vParams[i].name = EditorGUILayout.TextField(vParams[i].name, GUILayout.Width(headWidth));
                
                vParams[i].Flag =(EArgvFalg)EditorGUILayout.EnumPopup(vParams[i].Flag, new GUILayoutOption[] { GUILayout.Width(headWidth) });
                
                int displayIndex = EditorGUILayout.Popup(GuideSystemEditor.DisplayTypesPop.IndexOf(vParams[i].displayType), GuideSystemEditor.DisplayTypesPop.ToArray(), new GUILayoutOption[] { GUILayout.Width(displayWidth) });
                if(displayIndex>=0 && displayIndex < GuideSystemEditor.DisplayTypesPop.Count)
                    vParams[i].displayType = GuideSystemEditor.DisplayTypesPop[displayIndex];
                else
                {
                    vParams[i].displayType = null;
                }
                bool isEnum = false;
                if(vParams[i].displayType!=null && GuideSystemEditor.DisplayTypes.TryGetValue(vParams[i].displayType, out var attrType))
                {
                    if(attrType.displayType.IsEnum)
                    {
                        isEnum = true;
                    }
                }
                if (!isEnum)
                {
                    vParams[i].bBit = EBitGuiType.None;
                }
                vParams[i].bBit = (EBitGuiType)EditorGUILayout.EnumPopup(vParams[i].bBit, new GUILayoutOption[] { GUILayout.Width(headWidth) });

                // 上移
                GUI.enabled = i > 0;
                if (GUILayout.Button("↑", GUILayout.Width(30)))
                {
                    var temp = vParams[i - 1];
                    vParams[i - 1] = vParams[i];
                    vParams[i] = temp;
                    unit.inputs = vParams;
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                // 下移
                GUI.enabled = i < vParams.Length - 1;
                if (GUILayout.Button("↓", GUILayout.Width(30)))
                {
                    var temp = vParams[i + 1];
                    vParams[i + 1] = vParams[i];
                    vParams[i] = temp;
                    unit.inputs = vParams;
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                GUI.enabled = true;

                // 删除
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if(EditorUtility.DisplayDialog("提示", "是否删除改变量参数?","删除", "再想想"))
                    {
                        var list = new System.Collections.Generic.List<GuideCustomAgent.AgentArgv>(vParams);
                        list.RemoveAt(i);
                        vParams = list.ToArray();
                        EditorGUILayout.EndHorizontal();
                    }
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            return vParams;
        }
    }
}

#endif