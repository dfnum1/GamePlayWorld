#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public class GraphNode : IGraphNode
    {
        ExcudeNode m_BindNode;
        public ExcudeNode BindNode
        {
            get { return m_BindNode; }
            set
            {
                m_BindNode = value;
                if (value != null)
                {
                    value.pEditor = m_pEditor;
                    value.bindGraphNode = this;
                }
            }
        }
        public AAgentTreeData ATData;

        public int TaskID = 0;
        public Task EnterTask;

        public List<ArgvPort> Inputs = new List<ArgvPort>();
        public List<ArgvPort> Outputs = new List<ArgvPort>();

        public bool bExpand = true;

        public bool bLink = true;
        public LinkPort InLink = new LinkPort();
        public LinkPort OutLink = new LinkPort();

        public List<ConditionLinkPort> OutConditionLinks = new List<ConditionLinkPort>();

        public List<DelegateLinkPort> OutDelegateLinks = new List<DelegateLinkPort>();

        public bool bEnterRoot = false;

        public Vector2 offsetSize = Vector2.zero;

        public HashSet<GraphNode> PrevLinks = new HashSet<GraphNode>();
        public List<GraphNode> NextLinks = new List<GraphNode>();

        private EVariableType m_CustomVariableType = EVariableType.Null;

        IAgentTreeEditor m_pEditor;
        public IAgentTreeEditor Editor
        {
            get { return m_pEditor; }
        }
        public GraphNode(IAgentTreeEditor pEditor, AAgentTreeData asset, ExcudeNode pNode)
        {
            m_pEditor = pEditor;
            if (asset == null) return;
            ATData = asset;
            //    GUID = asset.GUID;
            //  Name = asset.Name;
            //  Port = asset.Port;
            BindNode = pNode;
            if(pNode!=null) pNode.pEditor = pEditor;

            InLink.baseNode = this;
            InLink.direction = EPortIO.In;

            OutLink.baseNode = this;
            OutLink.direction = EPortIO.Out;

            OutConditionLinks.Clear();
            OutDelegateLinks.Clear();
        }
        public void Init()
        {
            m_CustomVariableType = EVariableType.Null;
            if (EnterTask != null && EnterTask.EnterNode.Param != null && EnterTask.EnterNode.Param.variable != null)
            {
                m_CustomVariableType = VariableSerializes.GetVariableType(EnterTask.EnterNode.Param.variable.GetType());
            }
            InLink.baseNode = this;
            InLink.direction = EPortIO.In;

            OutLink.baseNode = this;
            OutLink.direction = EPortIO.Out;

          //  OutConditionLinks.Clear();
        }
        //------------------------------------------------------
        public void SetPosition(Vector2 pos)
        {
            if (EnterTask != null)
            {
                EnterTask.EnterNode.rect.position = pos;
                return;
            }
            BindNode.rect.x = pos.x;
            BindNode.rect.y = pos.y;
        }
        //------------------------------------------------------
        public Vector2 GetPosition()
        {
            if (EnterTask != null) return new Vector2(EnterTask.EnterNode.rect.x, EnterTask.EnterNode.rect.y);
            return new Vector2(BindNode.rect.x, BindNode.rect.y);
        }
        //------------------------------------------------------
        public float GetWidth()
        {
            if (EnterTask != null)
            {
                if (EnterTask.type == ETaskType.Custom)
                    return Mathf.Max(200, EnterTask.EnterNode.rect.width) + offsetSize.x;
                else if (EnterTask.type == ETaskType.KeyInput)
                    return Mathf.Max(150, EnterTask.EnterNode.rect.width) + offsetSize.x;
                else if (EnterTask.type == ETaskType.MouseInput)
                    return Mathf.Max(150, EnterTask.EnterNode.rect.width)+ offsetSize.x;
                else
                    return Mathf.Max(100, EnterTask.EnterNode.rect.width)+ offsetSize.x;
            }
            if(BindNode!=null && BindNode is APINode)
            {
                return Mathf.Max(250, BindNode.rect.width)+ offsetSize.x;
            }
            return Mathf.Max(200, BindNode.rect.width) + offsetSize.x;
        }
        //------------------------------------------------------
        public string ToTitleTips()
        {
            if (BindNode == null) return null;
            return BindNode.ToTitleTips();
        }
        //------------------------------------------------------
        public bool IsExpand()
        {
            if (EnterTask != null) return true;
            return bExpand;
        }
        //------------------------------------------------------
        public void SetExpand(bool bexpand)
        {
            if (EnterTask == null)
            {
                bExpand = bexpand;
            }
            for(int i =0; i < NextLinks.Count; ++i)
            {
                if (NextLinks[i] != null) NextLinks[i].SetExpand(bexpand);
            }
        }
        //------------------------------------------------------
        public float GetHeight()
        {
            if (EnterTask != null) return Mathf.Max(100, EnterTask.EnterNode.rect.height + offsetSize.y);
            return Mathf.Max(30, BindNode.rect.height + offsetSize.y);
        }
        //------------------------------------------------------
        public int GetGUID()
        {
            if (EnterTask != null) return TaskID;
            if (BindNode == null) return 0;
            return BindNode.GUID;
        }
        //------------------------------------------------------
        public Color GetTint()
        {
            return AgentTreePreferences.GetSettings().nodeBgColor;
        }
        //------------------------------------------------------
        public string GetDesc()
        {
            string desc = "未知";
            if (EnterTask != null)
            {
                desc = EnterTask.type.ToString();
                if (EnterTask.EnterNode != null && EnterTask.EnterNode.CustomName != null)
                {
                    if(!string.IsNullOrEmpty(EnterTask.EnterNode.CustomName)) desc += "::" + EnterTask.EnterNode.CustomName;
                    if (EnterTask.EnterNode.CustomGO) desc += "[" + EnterTask.EnterNode.CustomGO.name + "]";
                    if (EnterTask.EnterNode.CustomID>0) desc += "[" + EnterTask.EnterNode.CustomID + "]";
                }
            }
            else
            {
                desc = BindNode.strName;
            }
            return desc;
        }
        //------------------------------------------------------
        internal void SetAPINode(APINode apiNode, int atGuid)
        {
            if (BindNode == null || !(BindNode is ActionNode)) return;
            ActionNode actionNode = BindNode as ActionNode;
            if(actionNode.actionType == EActionType.ATFunction)
            {
                long key = ((long)apiNode.GUID) << 32 | (long)atGuid;
                actionNode.SetCustomValue(key);
            }
        }
        //------------------------------------------------------
        public void OnHeaderGUI()
        {
            if (EnterTask != null)
            {
                GUILayout.Label(EnterTask.type.ToString(), AgentTreeEditorResources.styles.nodeHeader, GUILayout.Height(30));
                bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                OutLink.baseNode = this;
                OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), OutLink);
            }
            else
            {
                Vector2 size = AgentTreeEditorResources.styles.nodeHeader.CalcSize(new GUIContent(BindNode.strName));
                BindNode.rect.width =  size.x+80 + offsetSize.x;
               
                GUILayout.Label(BindNode.strName, AgentTreeEditorResources.styles.nodeHeader, GUILayout.Height(30));
                if(BindNode.bBreakPoint)
                {
                    Rect rect = GUILayoutUtility.GetLastRect();
                    GUI.DrawTexture(new Rect(rect.xMax - 15, 0, 32, 32), AgentTreeEditorResources.breakPoint);
                }
            }
            if (BindNode is APINode)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.DrawTexture(new Rect(new Vector2(rect.xMin-8, 0), new Vector2(32, 32)), AgentTreeEditorResources.function);
            }
        }
        //------------------------------------------------------
        public void OnBodyGUI()
        {
            Inputs.Clear();
            Outputs.Clear();
            OutDelegateLinks.Clear();
            if (EnterTask != null)
            {
                if (EnterTask.type == ETaskType.Start) GUILayout.Box(AgentTreeEditorResources.enterStart);
                else if (EnterTask.type == ETaskType.Exit) GUILayout.Box(AgentTreeEditorResources.enterExit);
                else if (EnterTask.type == ETaskType.Tick) GUILayout.Box(AgentTreeEditorResources.enterTick);
                else if (EnterTask.type == ETaskType.KeyInput)
                {
                    AgentTreeUtl.BeginHorizontal();
                    GUILayout.Space((GetWidth() - 64) / 4);
                    GUILayout.Box(AgentTreeEditorResources.keyInput);
                    AgentTreeUtl.EndHorizontal();
                }
                else if (EnterTask.type == ETaskType.MouseInput)
                {
                    AgentTreeUtl.BeginHorizontal();
                    GUILayout.Space((GetWidth() - 64) / 4);
                    GUILayout.Box(AgentTreeEditorResources.mouseInput);
                    AgentTreeUtl.EndHorizontal();
                }
                else if (EnterTask.type == ETaskType.Custom)
                {
                    AgentTreeUtl.BeginHorizontal();
                    GUILayout.Space((GetWidth() - 64) / 4);
                    GUILayout.Box(AgentTreeEditorResources.enterCustom);
                    AgentTreeUtl.EndHorizontal();
                }
                if (EnterTask.type == ETaskType.Custom)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 50;
                    int index = -1;
                    for (int i = 0; i < AgentTreeEditorUtils.AssemblyATData.exportEventTypes.Count; ++i)
                    {
                        if (AgentTreeEditorUtils.AssemblyATData.exportEventTypes[i].enumValueInt == EnterTask.EnterNode.EnterType)
                        {
                            index = i;
                            break;
                        }
                    }
                    System.Type idType = null;
                    index = EditorGUILayout.Popup("事件类型", index, AgentTreeEditorUtils.AssemblyATData.exportEventTypeNames.ToArray());
                    if (index >= 0 && index < AgentTreeEditorUtils.AssemblyATData.exportEventTypes.Count)
                    {
                        idType = AgentTreeEditorUtils.AssemblyATData.exportEventTypes[index].idCustomType;
                        EnterTask.EnterNode.EnterType = AgentTreeEditorUtils.AssemblyATData.exportEventTypes[index].enumValueInt;
                    }
                    if (index != -1)
                    {
                        object retValue = AgentTreeUtl.DrawProperty("标识ID", EnterTask.EnterNode.CustomID, idType);
                        if (retValue != null)
                            EnterTask.EnterNode.CustomID = System.Convert.ToInt32(retValue);
                        else
                            EnterTask.EnterNode.CustomID = EditorGUILayout.IntField("标识ID", EnterTask.EnterNode.CustomID);
                        EnterTask.EnterNode.CustomName = EditorGUILayout.TextField("标识名", EnterTask.EnterNode.CustomName);
                        EnterTask.EnterNode.CustomGO = EditorGUILayout.ObjectField("对象", EnterTask.EnterNode.CustomGO, typeof(GameObject), true) as GameObject;
                        if (m_CustomVariableType == EVariableType.Null && EnterTask.EnterNode.Param != null && EnterTask.EnterNode.Param.variable != null)
                        {
                            m_CustomVariableType = VariableSerializes.GetVariableType(EnterTask.EnterNode.Param.variable.GetType());
                        }

                        m_CustomVariableType = (EVariableType)EditorGUILayout.EnumPopup("参数类型", m_CustomVariableType);
                        System.Type varType = VariableSerializes.GetVariableType(m_CustomVariableType);
                        if (varType != null &&
                            (EnterTask.EnterNode.Param == null ||
                            EnterTask.EnterNode.Param.variable == null ||
                            EnterTask.EnterNode.Param.variable.GetType() != varType))
                        {
                            if (EnterTask.EnterNode.Param == null)
                                EnterTask.EnterNode.Param = new Port(null);
                            EnterTask.EnterNode.pEditor.AdjustMaxGuid();
                            EnterTask.EnterNode.Param.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(varType);
                        }
                        if (EnterTask.EnterNode.Param != null && EnterTask.EnterNode.Param.variable != null)
                        {
                            ArgvPort port = EnterTask.EnterNode.Param.GetEditorer<ArgvPort>();
                            port.baseNode = this;
                            port.port = EnterTask.EnterNode.Param;
                            port.direction = EPortIO.Out;
                            port.index = -100;
                            DrawPropertyGUI.DrawVariable(this, port);
                            Outputs.Add(port);
                        }
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
                else if (EnterTask.type == ETaskType.KeyInput)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 30;

                    EnterTask.EnterNode.EnterType = (ushort)(KeyCode)EditorGUILayout.EnumPopup("按键", (KeyCode)EnterTask.EnterNode.EnterType);
                    if (
                        (EnterTask.EnterNode.Param == null ||
                        EnterTask.EnterNode.Param.variable == null ||
                        EnterTask.EnterNode.Param.variable.GetType() != typeof(VariableBool)))
                    {
                        if (EnterTask.EnterNode.Param == null)
                            EnterTask.EnterNode.Param = new Port(null);
                        EnterTask.EnterNode.pEditor.AdjustMaxGuid();
                        EnterTask.EnterNode.Param.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(typeof(VariableBool));
                    }
                    if (EnterTask.EnterNode.Param != null && EnterTask.EnterNode.Param.variable != null)
                    {
                        ArgvPort port = EnterTask.EnterNode.Param.GetEditorer<ArgvPort>();
                        port.baseNode = this;
                        port.port = EnterTask.EnterNode.Param;
                        port.direction = EPortIO.Out;
                        port.index = -100;
                        DrawPropertyGUI.DrawVariable(this, port);
                        Outputs.Add(port);
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
                else if (EnterTask.type == ETaskType.MouseInput)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 30;

                    EnterTask.EnterNode.EnterType = (ushort)EditorGUILayout.IntField("触控Id", EnterTask.EnterNode.EnterType);
                    if (
                        (EnterTask.EnterNode.Param == null ||
                        EnterTask.EnterNode.Param.variable == null ||
                        EnterTask.EnterNode.Param.variable.GetType() != typeof(VariableUser)))
                    {
                        if (EnterTask.EnterNode.Param == null)
                            EnterTask.EnterNode.Param = new Port(null);
                        EnterTask.EnterNode.pEditor.AdjustMaxGuid();
                        EnterTask.EnterNode.Param.variable = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(typeof(VariableUser));
                    }
                    if (EnterTask.EnterNode.Param != null && EnterTask.EnterNode.Param.variable != null)
                    {
                        var userData = EnterTask.EnterNode.Param.variable as VariableUser;
                        userData.hashCode = -1;//ATMouseData hashid

                        ArgvPort port = EnterTask.EnterNode.Param.GetEditorer<ArgvPort>();
                        port.baseNode = this;
                        port.port = EnterTask.EnterNode.Param;
                        port.direction = EPortIO.Out;
                        port.index = -100;
                        DrawPropertyGUI.DrawVariable(this, port);
                        Outputs.Add(port);
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
            }
            else
                GraphNodeGUI.DrawProperty(this);
        }
        //------------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {

        }
        //------------------------------------------------------
        public static void DrawLinkHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
    //        GUI.DrawTexture(rect, AgentTreeEditorResources.linkOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, AgentTreeEditorResources.linkOuter);
            GUI.color = col;
        }
        //------------------------------------------------------
        public static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, AgentTreeEditorResources.dotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, AgentTreeEditorResources.dot);
            GUI.color = col;
        }
        //------------------------------------------------------
        public static void PortField(Vector2 position, IPortNode port)
        {
            if (port == null) return;

            Rect rect = new Rect(position, new Vector2(16, 16));

            Color backgroundColor = new Color32(90, 97, 105, 255);
            Color col = port.GetColor();
            DrawPortHandle(rect, backgroundColor, col);
            port.SetRect(rect);
            PortUtil.allPortPositions[port.GetGUID()] = port;
        }
        //------------------------------------------------------
        public static void LinkField(Vector2 position, IPortNode port)
        {
            if (port == null) return;

            Rect rect = new Rect(position, new Vector2(16, 16));

            Color backgroundColor = new Color32(90, 97, 105, 255);
            Color col = port.GetColor();
            DrawLinkHandle(rect, backgroundColor, col);
            port.SetRect(rect);
            PortUtil.allPortPositions[port.GetGUID()] = port;
        }
        //------------------------------------------------------
        public void DrawArgvInPort(int index)
        {
            if (BindNode == null || index<0 || index>= BindNode.GetInArgvCount())
                return;
            ATExportNodeAttrData attrData = m_pEditor.GetActionNodeAttr((int)BindNode.GetExcudeHash());

            ArgvPort port = BindNode.GetInEditorPort<ArgvPort>(index);
            port.baseNode = this;
            port.port = BindNode.GetInPort(index);
            port.direction = EPortIO.In;
            port.index = index;
            bool bShowEdit = true;
            if (attrData != null && index < attrData.InArgvs.Count)
            {
                port.SetDefaultName(attrData.InArgvs[index].DisplayName);
                port.alignType = attrData.InArgvs[index].AlignType;
                port.displayType = attrData.InArgvs[index].DisplayType;
                bShowEdit = attrData.InArgvs[index].bShowEdit;
                if (port.alignType == null && port.port.dummyMap != null && port.port.dummyMap.Count > 0)
                {
                    Variable var = port.port.dummyMap.ElementAt(0).Value;
                    if (var != null && var.GetClassHashCode() != 0)
                        AgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);
                }
                if (!bShowEdit && port.alignType != null)
                {
                    if (port.variable != null) port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                }
            }
            if (bShowEdit) DrawPropertyGUI.DrawVariable(this, port);
            if (port.variable != null) this.Inputs.Add(port);
        }
        //------------------------------------------------------
        public void DrawArgvOutPort(int index)
        {
            if (BindNode == null || index < 0 || index >= BindNode.GetOutArgvCount())
                return;
            ATExportNodeAttrData attrData = m_pEditor.GetActionNodeAttr((int)BindNode.GetExcudeHash());

            ArgvPort port = BindNode.GetOutEditorPort<ArgvPort>(index);
            port.baseNode = this;
            port.port = BindNode.GetOutPort(index);
            port.direction = EPortIO.Out;
            port.index = index;
            bool bShowEdit = true;
            if (attrData != null && index < attrData.OutArgvs.Count)
            {
                port.SetDefaultName(attrData.OutArgvs[index].Name);
                port.alignType = attrData.OutArgvs[index].AlignType;
                port.displayType = attrData.OutArgvs[index].DisplayType;
                bShowEdit = attrData.OutArgvs[index].bShowEdit;
                if (port.alignType == null && port.port.dummyMap != null && port.port.dummyMap.Count > 0)
                {
                    Variable var = port.port.dummyMap.ElementAt(0).Value;
                    if (var != null && var.GetClassHashCode() != 0)
                        AgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);
                }
                if (!bShowEdit && port.alignType != null)
                {
                    if (port.variable != null) port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                }
            }
            if (bShowEdit)
            {
                int checkIndex = BindNode.IndexofInArgv(port.variable);
                if (checkIndex != -1 && checkIndex < Inputs.Count)
                    DrawPropertyGUI.DrawVariable(this, port, Inputs[checkIndex]);
                else DrawPropertyGUI.DrawVariable(this, port);
            }
            if (port.variable != null) this.Outputs.Add(port);
        }
    }
}
#endif