#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Framework.Plugin.AT
{
	public class GraphNodeGUI
	{
        public static List<string> EnumPops = new List<string>();
        public static List<Enum> EnumValuePops = new List<Enum>();
        public static HashSet<string> EnumFilers = new HashSet<string>();
        //------------------------------------------------------
        public static object PopEnum<T>(string label, T enumValue)
        {
            EnumPops.Clear();
            EnumValuePops.Clear();
            int index = -1;
            Type enumType = typeof(T);
            foreach (Enum v in Enum.GetValues(enumType))
            {
                FieldInfo fi = enumType.GetField(v.ToString());
                string strTemName = v.ToString();
                if (fi != null && fi.IsDefined(typeof(ATFieldAttribute)))
                {
                    strTemName = fi.GetCustomAttribute<ATFieldAttribute>().DisplayName;
                }
                EnumPops.Add(strTemName);
                EnumValuePops.Add(v);
                if (v.ToString().CompareTo(enumValue.ToString()) == 0)
                    index = EnumPops.Count - 1;
            }
            index = EditorGUILayout.Popup(label, index, EnumPops.ToArray());
            if (index >= 0 && index < EnumValuePops.Count)
            {
                return EnumValuePops[index];
            }
            return enumValue;
        }
        //------------------------------------------------------
        public static bool DrawProperty(GraphNode pNode)
		{
            if(pNode.BindNode is APINode)
            {
                return DrawAPINode(pNode);
            }
            else if (pNode.BindNode is ActionNode)
            {
                switch ((EActionType)pNode.BindNode.GetExcudeHash())
                {
                    case EActionType.NewVariable: return DrawNewVariable(pNode);
                    case EActionType.FieldVariable: return DrawFieldVariable(pNode);
                    case EActionType.CastVariable: return DrawCastVariable(pNode);
                    case EActionType.ATFunction: return DrawATFunction(pNode);
                    case EActionType.DelegateCallback: return DrawDelegateCallback(pNode);
                    case EActionType.Condition_IfElse: return DrawCondition(pNode);
                    case EActionType.Condition_IfAnd: return DrawIfAndOrCondition(pNode);
                    case EActionType.Condition_IfOr: return DrawIfAndOrCondition(pNode);
                    case EActionType.Condition_Switch: return DrawCondition(pNode);
                    case EActionType.Condition_Where: return DrawCondition(pNode);
                    case EActionType.Condition_Parallel: return DrawCondition(pNode);
                    case EActionType.Condition_Sync: return DrawCondition(pNode);
                    case EActionType.Condition_FrameDo: return DrawCondition(pNode);
                }
            }
            
            System.Reflection.MethodInfo call = AgentTreeEditorUtils.GetDrawProperyMethod();
            if (call != null)
                return (bool)call.Invoke(null, new object[] { pNode });
            return false;
        }
        //------------------------------------------------------
        static bool DrawAPINode(GraphNode pNode)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;


            APINode pApiNode = pNode.BindNode as APINode;
            int preType = pApiNode.excudeType;

            pApiNode.strName = EditorGUILayout.TextField("Name", pApiNode.strName);

            EditorGUIUtility.labelWidth = labelWidth;

            ATExportNodeAttrData attrData = pNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
            if (attrData == null || attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
//                 pNode.InLink.baseNode = pNode;
//                 pNode.InLink.direction = EPortIO.In;
//                 GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);

                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;

            VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();

            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = (EPortIO)0;// EPortIO.In;
                port.index = i;
                if (port.variable!=null && string.IsNullOrEmpty(port.variable.strName) )
                    port.variable.strName = "变量[" + (i+1) + "]";

                Rect rect;
                GUILayout.BeginHorizontal();
                DrawPropertyGUI.DrawVariable(pNode, port, null, false, 50,-pNode.GetWidth()/2);
                System.Type retVarType = DrawPropertyGUI.PopVariableType(port.variable.GetType(), null, new GUILayoutOption[] { GUILayout.Width(40) });
                if(retVarType != port.variable.GetType())
                {
                    port.variable = pVarsFactor.NewVariableByType(retVarType, port.variable.GUID);
                }
                if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    if (EditorUtility.DisplayDialog("提示", "移除该变量?", "移除", "取消"))
                    {
                        pNode.BindNode.DelInPort(i);
                        break;
                    }
                }
                if (i>0 && GUILayout.Button("↑", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    pNode.BindNode.SwapInArgv(i, i-1);
                }
                if (i < pNode.BindNode.GetInArgvCount()-1 && GUILayout.Button("↓", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    pNode.BindNode.SwapInArgv(i, i + 1);
                }
                rect = GUILayoutUtility.GetLastRect();
                GUILayout.EndHorizontal();

                ArgvPort portDummy = pNode.BindNode.GetInEditorPort<ArgvPort>(i,1);
                portDummy.baseNode = pNode;
                portDummy.port = pNode.BindNode.GetInPort(i);
                portDummy.direction = EPortIO.Out;
                portDummy.index = i;
                portDummy.inputToOutput = true;

                GraphNode.PortField(rect.position + new Vector2(rect.width, 0), portDummy);

                pNode.Inputs.Add(port);
                pNode.Inputs.Add(portDummy);

            }
            if (GUILayout.Button("添加参数"))
            {
                pNode.Editor?.AdjustMaxGuid();
                Variable pvar = null;
                pvar = pVarsFactor.NewVariableByType(typeof(VariableInt));
                pvar.strName = "";
                pNode.BindNode.AddInArgv(pvar);
                pNode.BindNode.Save();
            }

            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                //     port.variable = pNode.BindNode.GetOutVariable(i);
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.index = i;
                if (port.variable != null && string.IsNullOrEmpty(port.variable.strName))
                    port.variable.strName = "返回值[" + (i+1) + "]";

                GUILayout.BeginHorizontal();
                DrawPropertyGUI.DrawVariable(pNode, port, null, false, 70, -pNode.GetWidth() / 2);
                System.Type retVarType = DrawPropertyGUI.PopVariableType(port.variable.GetType(), null, new GUILayoutOption[] { GUILayout.Width(40) });
                if (retVarType != port.variable.GetType())
                {
                    port.variable = pVarsFactor.NewVariableByType(retVarType, port.variable.GUID);
                }
                if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    if (EditorUtility.DisplayDialog("提示", "移除该变量?", "移除", "取消"))
                    {
                        pNode.BindNode.DelOutPort(i);
                        break;
                    }
                }
                if (i > 0 && GUILayout.Button("↑", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    pNode.BindNode.SwapOutArgv(i, i - 1);
                }
                if (i < pNode.BindNode.GetOutArgvCount() - 1 && GUILayout.Button("↓", new GUILayoutOption[] { GUILayout.Width(20) }))
                {
                    pNode.BindNode.SwapOutArgv(i, i + 1);
                }
                GUILayout.EndHorizontal();
                pNode.Outputs.Add(port);
            }
            return true;
        }
        //------------------------------------------------------
        static bool DrawFieldVariable(GraphNode pNode)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;

            bool bChanged = false;
            if (!bChanged && (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length !=1)) bChanged = true;

            ATExportNodeAttrData exportNodeAttr = null;
            List<ATMethodReturnAttribute> OutArgvs = null;
            if (!bChanged)
            {
                VariableUser user = pNode.BindNode.GetInVariableByIndex<VariableUser>(0);
                if (user == null )
                {
                    bChanged = true;
                }
                if(!bChanged)
                {
                    int index = -1;
                    for (int i = 0; i < AgentTreeEditorUtils.AssemblyATData.exportFieldMethods.Count; ++i)
                    {
                        if (AgentTreeEditorUtils.AssemblyATData.exportFieldMethods[i].classHashCode == user.hashCode)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        pNode.BindNode.ClearOutArgv();
                        pNode.BindNode.Save();
                    }

                    int newhashCode = index;
                    index = UnityEditor.EditorGUILayout.Popup("入口", index, AgentTreeEditorUtils.AssemblyATData.exportFieldMethodsNames.ToArray());
                    if (index >=0 && index < AgentTreeEditorUtils.AssemblyATData.exportFieldMethods.Count)
                    {
                        exportNodeAttr = AgentTreeEditorUtils.AssemblyATData.exportFieldMethods[index];
                        user.hashCode = exportNodeAttr.classHashCode;
                        pNode.BindNode.SetCustomValue(exportNodeAttr.methodAttr.ActionType);
                        OutArgvs = exportNodeAttr.OutArgvs;
                        if (user.hashCode != 0 && OutArgvs.Count>0)
                        {
                            bool bChangeRet = false;
                            if (pNode.BindNode.GetOutArgvCount() <= 0) bChangeRet = true;
                            if(!bChangeRet)
                            {
                                if(OutArgvs[0].ReturnType != pNode.BindNode.GetOutVariable(0).GetType())
                                {
                                    bChangeRet = true;
                                }
                            }
                            if(bChangeRet)
                            {
                                pNode.Editor?.AdjustMaxGuid();
                                VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                                Variable pvar = null;
                                pvar = pVarsFactor.NewVariableByType(OutArgvs[0].ReturnType);
                                pvar.strName = "";
                                pNode.BindNode.AddOutArgv(pvar);
                                pNode.BindNode.Save();
                            }
                        }
                    }
                }
            }


            if (bChanged)
            {
                VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                pNode.BindNode.ClearArgv();
                {
                    pNode.Editor?.AdjustMaxGuid();
                    Variable pvar = null;
                    pvar = pVarsFactor.NewVariable<Plugin.AT.VariableUser>();
                    pvar.strName = "";
                    pNode.BindNode.AddInArgv(pvar);
                }

                pNode.BindNode.Save();
            }

            EditorGUIUtility.labelWidth = labelWidth;

            ATExportNodeAttrData attrData = pNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
            if (attrData ==null || attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                pNode.InLink.baseNode = pNode;
                pNode.InLink.direction = EPortIO.In;
                GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;
            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
         //       port.variable = pNode.BindNode.GetInVariable(i);
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                if (exportNodeAttr != null)
                    port.alignType = exportNodeAttr.classType;

                bool bShowEdit = true;
                if (attrData != null && i < attrData.InArgvs.Count)
                {
                    port.SetDefaultName(attrData.InArgvs[i].DisplayName);
                    port.alignType = attrData.InArgvs[i].AlignType;
                    port.displayType = attrData.InArgvs[i].DisplayType;
                    bShowEdit = attrData.InArgvs[i].bShowEdit;
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (bShowEdit) DrawPropertyGUI.DrawVariable(pNode, port,null, false);
                pNode.Inputs.Add(port);
            }

            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port =pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
           //     port.variable = pNode.BindNode.GetOutVariable(i);
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.index = i;
                bool bShowEdit = true;
                if (OutArgvs != null && i < OutArgvs.Count)
                {
                    port.SetDefaultName(OutArgvs[i].Name);
                    port.alignType = OutArgvs[i].AlignType;
                    port.displayType = OutArgvs[i].DisplayType;
                    bShowEdit = OutArgvs[i].bShowEdit;
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (bShowEdit)
                {
                    if (pNode.BindNode.HasInArgv(port.variable))
                        DrawPropertyGUI.DrawVariable(pNode, port, pNode.Inputs[pNode.BindNode.IndexofInArgv(port.variable)], false);
                    else DrawPropertyGUI.DrawVariable(pNode, port, null, false);
                }
                pNode.Outputs.Add(port);
            }
            return true;
        }
        //------------------------------------------------------
        public static bool DrawCastVariable(GraphNode pNode)
        {
            bool bChanged = false;
            if (!bChanged && (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length != 1)) bChanged = true;
            if (!bChanged && (pNode.BindNode.outArgvs == null || pNode.BindNode.outArgvs.Length != 1)) bChanged = true;
            if (bChanged)
            {
                VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                pNode.BindNode.ClearArgv();
                {
                    pNode.Editor?.AdjustMaxGuid();
                    Variable pvar = null;
                    pvar = pVarsFactor.NewVariable<Plugin.AT.VariableUser>();
                    pvar.strName = "";
                    pNode.BindNode.AddInArgv(pvar);
                }
                {
                    Variable pvar = null;
                    pvar = pVarsFactor.NewVariable<Plugin.AT.VariableUser>();
                    pvar.strName = "";
                    pNode.BindNode.AddOutArgv(pvar);
                }
                pNode.BindNode.Save();
            }
            ATExportNodeAttrData attrData = pNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
            if (attrData == null || attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                pNode.InLink.baseNode = pNode;
                pNode.InLink.direction = EPortIO.In;
                GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;
            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
         //       port.variable = pNode.BindNode.GetInVariable(i);
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                bool bShowEdit = true;
                if (attrData != null && i < attrData.InArgvs.Count)
                {
                    port.SetDefaultName(attrData.InArgvs[i].DisplayName);
                    port.alignType = attrData.InArgvs[i].AlignType;
                    port.displayType = attrData.InArgvs[i].DisplayType;
                    bShowEdit = attrData.InArgvs[i].bShowEdit;
                    if(port.alignType == null && port.port.dummyMap!=null && port.port.dummyMap.Count>0)
                    {
                        Variable var = port.port.dummyMap.ElementAt(0).Value;
                        if(var != null && var.GetClassHashCode()!=0)
                        {
                            AgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);
                        }
                    }
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (bShowEdit) DrawPropertyGUI.DrawVariable(pNode, port);
                pNode.Inputs.Add(port);
            }
            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
          //      port.variable = pNode.BindNode.GetOutVariable(i);
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.index = i;
                bool bShowEdit = true;
                if (attrData != null && i < attrData.OutArgvs.Count)
                {
                    port.SetDefaultName(attrData.OutArgvs[i].Name);
                    port.alignType = attrData.OutArgvs[i].AlignType;
                    port.displayType = attrData.OutArgvs[i].DisplayType;
                    bShowEdit = attrData.OutArgvs[i].bShowEdit;
                    if (port.alignType == null && port.port.dummyMap != null && port.port.dummyMap.Count > 0)
                    {
                        Variable var = port.port.dummyMap.ElementAt(0).Value;
                        if (var != null && var.GetClassHashCode() != 0)
                        {
                            AgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);
                        }
                    }
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (bShowEdit)
                {
                    if (pNode.BindNode.HasInArgv(port.variable))
                        DrawPropertyGUI.DrawVariable(pNode, port, pNode.Inputs[pNode.BindNode.IndexofInArgv(port.variable)]);
                    else DrawPropertyGUI.DrawVariable(pNode, port);
                }
                pNode.Outputs.Add(port);
            }
            return true;
        }
        //------------------------------------------------------
        public static bool DrawNewVariable(GraphNode pNode)
        {
            bool bChanged = false;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;
            long preValue = pNode.BindNode.GetCustomValue();
            ATExportNodeAttrData exportData = pNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
            AgentTreeEditorUtils.vPopVariables.Clear();
            if (pNode.Editor != null && exportData!=null && exportData.betweenVar != null)
            {
                int index = -1;
                if (!exportData.betweenVar.bAll && exportData.betweenVar.end > exportData.betweenVar.begin && exportData.betweenVar.begin > EVariableType.Null)
                {
                    for (EVariableType i = exportData.betweenVar.begin; i <= exportData.betweenVar.end; ++i)
                    {
                        if (preValue == (int)i) index = AgentTreeEditorUtils.vPopVariables.Count;
                        AgentTreeEditorUtils.vPopVariables.Add(i.ToString());
                    }
                }
                else if (exportData.betweenVar.bAll)
                {
                    foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
                    {
                        if (preValue == (int)v) index = AgentTreeEditorUtils.vPopVariables.Count;
                        AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
                    }
                }
                index = EditorGUILayout.Popup("类型", index, AgentTreeEditorUtils.vPopVariables.ToArray());
                if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));
                else pNode.BindNode.SetCustomValue((int)exportData.betweenVar.begin);
            }
            else
            {
                foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
                {
                    AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
                }
                int index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());
                index = EditorGUILayout.Popup("类型", index, AgentTreeEditorUtils.vPopVariables.ToArray());
                if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));
                else pNode.BindNode.SetCustomValue( 1);
            }
            bChanged = preValue != pNode.BindNode.GetCustomValue();
            EditorGUIUtility.labelWidth = labelWidth;
            AgentTreeEditorUtils.vPopVariables.Clear();
            if (!bChanged && (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length != 1)) bChanged = true;
            if (bChanged && pNode.BindNode.GetCustomValue() > 0)
            {
                VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                pNode.Editor?.AdjustMaxGuid();
                pNode.BindNode.ClearArgv();
                {
                    Variable pvar = null;
                    pvar = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType((EVariableType)pNode.BindNode.GetCustomValue()));
                    pvar.strName = "";
                    pNode.BindNode.AddInArgv(pvar);
                    pNode.BindNode.AddOutArgv(pvar);
                }
                pNode.BindNode.Save();
            }
            ATExportNodeAttrData attrData = pNode.Editor?.GetActionNodeAttr((int)pNode.BindNode.GetExcudeHash());
            if (attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                pNode.InLink.baseNode = pNode;
                pNode.InLink.direction = EPortIO.In;
                GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;
            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                port.variable.SetFlag(EFlag.Declaration, true);
                bool bShowEdit = true;
                if (attrData != null && i < attrData.InArgvs.Count)
                {
                    port.SetDefaultName(attrData.InArgvs[i].DisplayName);
                    port.alignType = attrData.InArgvs[i].AlignType;
                    port.displayType = attrData.InArgvs[i].DisplayType;
                    bShowEdit = attrData.InArgvs[i].bShowEdit;
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
                if (bShowEdit) DrawPropertyGUI.DrawVariable(pNode, port);
                if (port.variable != null) pNode.Inputs.Add(port);
            }
            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.variable.SetFlag(EFlag.Declaration, true);
                port.index = i;
                bool bShowEdit = true;
                if (attrData != null && i < attrData.OutArgvs.Count)
                {
                    port.SetDefaultName(attrData.OutArgvs[i].Name);
                    port.alignType = attrData.OutArgvs[i].AlignType;
                    port.displayType = attrData.OutArgvs[i].DisplayType;
                    bShowEdit = attrData.OutArgvs[i].bShowEdit;
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
                    int checkIndex = pNode.BindNode.IndexofInArgv(port.variable);
                    if (checkIndex != -1 && checkIndex < pNode.Inputs.Count)
                        DrawPropertyGUI.DrawVariable(pNode, port, pNode.Inputs[checkIndex]);
                    else DrawPropertyGUI.DrawVariable(pNode, port);
                }
                if (port.variable != null) pNode.Outputs.Add(port);
            }
            return true;
        }
        //------------------------------------------------------
        static bool DrawATFunction(GraphNode pNode)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;
            long nAPIFunc = pNode.BindNode.GetCustomValue();
            APINode apiNode = pNode.Editor?.GetAPINode(nAPIFunc);
            if (GUILayout.Button(apiNode != null ? apiNode.strName : "None"))
            {
                pNode.Editor?.ATFuncContextMenu(Event.current.mousePosition, pNode);
            }
            pNode.bLink = true;
            Rect rect = GUILayoutUtility.GetLastRect();
            pNode.InLink.baseNode = pNode;
            pNode.InLink.direction = EPortIO.In;
            GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
            pNode.OutLink.baseNode = pNode;
            pNode.OutLink.direction = EPortIO.Out;
            GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);

            if(apiNode != null)
            {
                bool bChange = false;

                int oldLen = pNode.BindNode.inArgvs != null ? pNode.BindNode.inArgvs.Length : 0;
                int newLen = apiNode.inArgvs != null ? apiNode.inArgvs.Length : 0;
                if (newLen!= oldLen)
                {
                    bChange = true;
                }
                if(!bChange)
                {
                    oldLen = pNode.BindNode.outArgvs != null ? pNode.BindNode.outArgvs.Length : 0;
                    newLen = apiNode.outArgvs != null ? apiNode.outArgvs.Length : 0;
                    if (newLen != oldLen)
                    {
                        bChange = true;
                    }
                }
  
                if(!bChange)
                {
                    if(apiNode.inArgvs!=null)
                    {
                        for (int i = 0; i < apiNode.inArgvs.Length; ++i)
                        {
                            if (apiNode.inArgvs[i].variable.GetType() != pNode.BindNode.inArgvs[i].variable.GetType())
                            {
                                bChange = true;
                                break;
                            }
                        }
                    }

                }
                if (!bChange)
                {
                    if(apiNode.outArgvs!=null)
                    {
                        for (int i = 0; i < apiNode.outArgvs.Length; ++i)
                        {
                            if (apiNode.outArgvs[i].variable.GetType() != pNode.BindNode.outArgvs[i].variable.GetType())
                            {
                                bChange = true;
                                break;
                            }
                        }
                    }

                }
                if (bChange)
                {
                    List<Port> vInTemp = (pNode.BindNode.inArgvs != null) ? new List<Port>(pNode.BindNode.inArgvs) : null;
                    List<Port> vOutTemp = (pNode.BindNode.outArgvs != null) ? new List<Port>(pNode.BindNode.outArgvs) : null;
                    VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                    pNode.Editor?.AdjustMaxGuid();
                    pNode.BindNode.ClearArgv();
                    for (int i =0; i < apiNode.inArgvs.Length; ++i)
                    {
                        Port pvar = AgentTreeEditorUtils.BuildOriVariableCommonNew(apiNode.inArgvs[i].variable.GetType(), pVarsFactor, vInTemp, i);
                        pvar.variable.strName = "变量[" + (i+1) + "]";
                        pvar.variable.SetFlag(EFlag.Override, true);
                        pNode.BindNode.AddInPort(pvar);
                    }
                    for (int i = 0; i < apiNode.outArgvs.Length; ++i)
                    {
                        Port pvar = AgentTreeEditorUtils.BuildOriVariableCommonNew(apiNode.outArgvs[i].variable.GetType(), pVarsFactor, vOutTemp, i);
                        pvar.variable.strName = "返回值[" + (i + 1) + "]";
                        pNode.BindNode.AddOutPort(pvar);
                    }
                    pNode.BindNode.Save();
                }
            }


            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                port.variable.SetFlag(EFlag.Declaration, true);
                DrawPropertyGUI.DrawVariable(pNode, port,null, port.port.dummyMap ==null || port.port.dummyMap.Count <= 0);
                if (port.variable != null) pNode.Inputs.Add(port);
            }
            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.variable.SetFlag(EFlag.Declaration, true);
                port.index = i;
                DrawPropertyGUI.DrawVariable(pNode, port, null, false);
                if (port.variable != null) pNode.Outputs.Add(port);
            }
            return true;
        }
        //------------------------------------------------------
        static bool DrawDelegateCallback(GraphNode pNode)
        {
          //  bool bChanged = false;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;

            GraphNode grapNode;
            if (pNode.Editor != null && pNode.Editor.GetGraphNodes().TryGetValue((int)pNode.BindNode.GetCustomValue(), out grapNode))
            {
                int index = grapNode.BindNode.IndexofInArgvLink(pNode.BindNode);

                ATExportNodeAttrData attrData = grapNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
                if(attrData!=null && attrData.monoFuncAttr!=null && attrData.monoFuncAttr.DecleType!=null && index >= 0 && index < attrData.InArgvs.Count)
                {
                    var attr = attrData.InArgvs[index];
                    if(attr.ArgvType == typeof(VariableDelegate))
                    {
                        if(attr.vDelegateArgvs == null)
                        {
                            MemberInfo methodInfo = null;
                            Dictionary<string, int> vMethodNames = new Dictionary<string, int>();
                            var members = attrData.monoFuncAttr.DecleType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                            for (int i = 0; i < members.Length; ++i)
                            {
                                if (members[i].IsDefined(typeof(ATMethodAttribute)) || members[i].IsDefined(typeof(ATFieldAttribute)))
                                {
                                    int memberCnt = 0;
                                    if (!vMethodNames.TryGetValue(members[i].Name, out memberCnt))
                                    {
                                        memberCnt = 1;
                                        vMethodNames[members[i].Name] = memberCnt;
                                    }
                                    else
                                        vMethodNames[members[i].Name] = ++memberCnt;
                                    AgentTreeAutoBuildCodeGUI.ExportMethodInfo exportMth = new AgentTreeAutoBuildCodeGUI.ExportMethodInfo();
                                    exportMth.info = members[i];
                                    if (memberCnt <= 1) exportMth.memberName = members[i].Name;
                                    else exportMth.memberName = members[i].Name + "_" + (memberCnt - 1);
                                    int guid = AgentTreeAutoBuildCodeGUI.BuildMonoFuncID(attrData.monoFuncAttr.DecleType, exportMth);
                                    if (guid == attrData.monoFuncAttr.guid)
                                    {
                                        methodInfo = members[i];
                                        break;
                                    }
                                }
                            }
                            if (methodInfo != null && methodInfo is MethodInfo)
                            {
                                MethodInfo method = methodInfo as MethodInfo;
                                if(method.IsStatic)
                                {
                                    ParameterInfo[] paramInfos = method.GetParameters();
                                    if (paramInfos != null && index >= 0 && index < paramInfos.Length)
                                        attr.CheckDelegateArgvs(paramInfos[index]);
                                }
                                else
                                {
                                    index -= 1;
                                    ParameterInfo[] paramInfos = method.GetParameters();
                                    if (paramInfos != null && index >= 0 && index < paramInfos.Length)
                                        attr.CheckDelegateArgvs(paramInfos[index]);
                                }
                            }
                        }
                        if(attr.vDelegateArgvs != null)
                        {
                            if(pNode.BindNode.outArgvs ==null || attr.vDelegateArgvs.Count != pNode.BindNode.outArgvs.Length)
                            {
                                List<Port> vInTemp = (pNode.BindNode.inArgvs != null) ? new List<Port>(pNode.BindNode.inArgvs) : null;
                                List<Port> vOutTemp = (pNode.BindNode.outArgvs != null) ? new List<Port>(pNode.BindNode.outArgvs) : null;
                                VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                                pNode.Editor?.AdjustMaxGuid();

                                pNode.BindNode.ClearArgv();
                                for(int k =0; k < attr.vDelegateArgvs.Count; ++k)
                                {
                                    Port pvar = null;
                                    pvar = AgentTreeEditorUtils.BuildOriVariableCommonNew(attr.vDelegateArgvs[k].ArgvType, pVarsFactor, vInTemp, k);
                                    if (attr.vDelegateArgvs[k].ArgvType == typeof(VariableUser))
                                        pvar.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(attr.vDelegateArgvs[k].AlignType));
                                    else if (attr.vDelegateArgvs[k].ArgvType == typeof(VariableMonoScript))
                                        pvar.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(attr.vDelegateArgvs[k].AlignType));
                                    pNode.BindNode.AddOutPort(pvar);
                                }
                                pNode.BindNode.Save();
                            }
                        }
                        else
                        {
                            if (pNode.BindNode.outArgvs != null && pNode.BindNode.outArgvs.Length > 0)
                            {
                                pNode.BindNode.ClearArgv();
                                pNode.BindNode.Save();
                            }
                        }
                    }
                }
            }
            else
            {
                if (pNode.BindNode.outArgvs != null && pNode.BindNode.outArgvs.Length > 0)
                {
                    pNode.BindNode.ClearArgv();
                    pNode.BindNode.Save();
                }
            }

            pNode.bLink = true;
            Rect rect = GUILayoutUtility.GetLastRect();
            pNode.InLink.baseNode = pNode;
            pNode.InLink.direction = EPortIO.In;
            GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
            pNode.OutLink.baseNode = pNode;
            pNode.OutLink.direction = EPortIO.Out;
            GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);

            Type DisplayType1 = null;
            Type AlignType1 = null;
            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                //     port.variable = pNode.BindNode.GetInVariable(i);
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                if (port.alignType == null) port.alignType = AlignType1;
                if (port.displayType == null) port.displayType = DisplayType1;
                DrawPropertyGUI.DrawVariable(pNode, port);
                pNode.Inputs.Add(port);
            }
            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
                //   port.variable = pNode.BindNode.GetOutVariable(i);
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.index = i;
                DrawPropertyGUI.DrawVariable(pNode, port);
                pNode.Outputs.Add(port);
            }

            return true;
        }
        //------------------------------------------------------
        static bool DrawCondition(GraphNode pNode)
        {
            bool bChanged = false;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;
            long preValue = pNode.BindNode.GetCustomValue();
            ATExportNodeAttrData exportData = null;
            AgentTreeEditorUtils.vPopVariables.Clear();

            ActionNode pAction = pNode.BindNode as ActionNode;
            if (pAction!=null && pAction.condition == null)
            {
                pAction.condition = new Condition();
            }

            bool hasPortalPort = true;
            if(pNode.BindNode.GetExcudeHash() == (int)EActionType.Condition_IfAnd || pNode.BindNode.GetExcudeHash() == (int)EActionType.Condition_IfOr)
            {
                hasPortalPort = false;
            }

            if(pNode.BindNode.GetExcudeHash() != (int)EActionType.Condition_FrameDo)
            {
                if (pNode.Editor != null && AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)pNode.BindNode.GetExcudeHash(), out exportData) && exportData.betweenVar != null)
                {
                    int index = -1;
                    if (!exportData.betweenVar.bAll && exportData.betweenVar.end > exportData.betweenVar.begin && exportData.betweenVar.begin > EVariableType.Null)
                    {
                        for (EVariableType i = exportData.betweenVar.begin; i <= exportData.betweenVar.end; ++i)
                        {
                            if (preValue == (int)i) index = AgentTreeEditorUtils.vPopVariables.Count;
                            AgentTreeEditorUtils.vPopVariables.Add(i.ToString());
                        }
                    }
                    else if (exportData.betweenVar.bAll)
                    {
                        foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
                        {
                            if (preValue == (int)v) index = AgentTreeEditorUtils.vPopVariables.Count;
                            AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
                        }
                    }
                    index = EditorGUILayout.Popup("类型", index, AgentTreeEditorUtils.vPopVariables.ToArray());
                    if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue( (int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));
                    else pNode.BindNode.SetCustomValue((int)exportData.betweenVar.begin);
                }
                else
                {
                    foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
                    {
                        AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
                    }
                    int index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());
                    index = EditorGUILayout.Popup("类型", index, AgentTreeEditorUtils.vPopVariables.ToArray());
                    if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue(  (int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));
                    else pNode.BindNode.SetCustomValue( 1);
                }
            }
            else
            {
                pNode.BindNode.SetCustomValue( (int)EVariableType.Float);
            }
            
            bChanged = preValue != pNode.BindNode.GetCustomValue();
            VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
            EditorGUIUtility.labelWidth = labelWidth;
            AgentTreeEditorUtils.vPopVariables.Clear();
            if (!bChanged && (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length != 1)) bChanged = true;
            if (pNode.BindNode.GetExcudeHash() == (int)EActionType.Condition_FrameDo)
            {
                if (!bChanged && (pNode.BindNode.outArgvs == null || pNode.BindNode.outArgvs.Length != 1)) bChanged = true;
            }
            if (bChanged && pNode.BindNode.GetCustomValue() > 0)
            {
                pNode.BindNode.ClearArgv();
                if (pNode.BindNode.GetExcudeHash() == (int)EActionType.Condition_FrameDo)
                {
                    pNode.Editor?.AdjustMaxGuid();
                    {
                        Variable pvar = null;
                        pvar = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType((EVariableType)pNode.BindNode.GetCustomValue()));
                        pvar.strName = "秒";
                        pNode.BindNode.AddInArgv(pvar);
                    }
                    {
                        Variable pvar = null;
                        pvar = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType((EVariableType)pNode.BindNode.GetCustomValue()));
                        pvar.strName = "当前秒";
                        pNode.BindNode.AddOutArgv(pvar);
                    }
                }
                else
                {
                    pNode.Editor?.AdjustMaxGuid();
                    {
                        Variable pvar = null;
                        pvar = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType((EVariableType)pNode.BindNode.GetCustomValue()));
                        pvar.strName = "参数1";
                        pNode.BindNode.AddInArgv(pvar);
                    }
                }
                pNode.BindNode.Save();
            }
            ATExportNodeAttrData attrData = pNode.Editor?.GetActionNodeAttr((int)pNode.BindNode.GetExcudeHash());
            if (attrData == null || attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                pNode.InLink.baseNode = pNode;
                pNode.InLink.direction = EPortIO.In;
                GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;

            if (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length <= 0) return true;
            Variable pInArgv = pNode.BindNode.GetInVariable(0);
            Type DisplayType1 = null;
            Type AlignType1 = null;
            ArgvPort enterPort = pNode.BindNode.GetInEditorPort<ArgvPort>(0);
            if (enterPort!=null && enterPort.port!=null && enterPort.port.dummyMap != null && pNode.Editor != null)
            {
                foreach (var db in enterPort.port.dummyMap)
                {
                    GraphNode pBindNode = pNode.Editor?.GetGraphNode(db.Key);
                    if (pBindNode != null && pBindNode.BindNode != null)
                    {
                        Port dummPort = pBindNode.BindNode.GetOutPort(pBindNode.BindNode.IndexofOutArgv(db.Value));
                        if (dummPort != null && dummPort.Editorer != null && dummPort.Editorer is ArgvPort)
                        {
                            if (AlignType1 == null)
                                AlignType1 = (dummPort.Editorer as ArgvPort).alignType;
                            if (DisplayType1 == null)
                                DisplayType1 = (dummPort.Editorer as ArgvPort).displayType;
                        }
                    }
                    if (AlignType1 != null && DisplayType1 != null) break;
                }
            }
            for (int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
           //     port.variable = pNode.BindNode.GetInVariable(i);
                port.port = pNode.BindNode.GetInPort(i);
                port.direction = EPortIO.In;
                port.index = i;
                bool bShowEdit = true;
                if (attrData != null && i < attrData.InArgvs.Count)
                {
                    port.SetDefaultName(attrData.InArgvs[i].DisplayName);
                    port.alignType = attrData.InArgvs[i].AlignType;
                    port.displayType = attrData.InArgvs[i].DisplayType;
                    bShowEdit = attrData.InArgvs[i].bShowEdit;
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (port.alignType == null) port.alignType = AlignType1;
                if (port.displayType == null) port.displayType = DisplayType1;
                if (bShowEdit) DrawPropertyGUI.DrawVariable(pNode, port);
                pNode.Inputs.Add(port);
            }
            for (int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)
            {
                ArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);
                port.baseNode = pNode;
             //   port.variable = pNode.BindNode.GetOutVariable(i);
                port.port = pNode.BindNode.GetOutPort(i);
                port.direction = EPortIO.Out;
                port.index = i;
                bool bShowEdit = true;
                if (attrData != null && i < attrData.OutArgvs.Count)
                {
                    port.SetDefaultName(attrData.OutArgvs[i].Name);
                    port.alignType = attrData.OutArgvs[i].AlignType;
                    port.displayType = attrData.OutArgvs[i].DisplayType;
                    bShowEdit = attrData.OutArgvs[i].bShowEdit;
                    if (!bShowEdit && port.alignType != null)
                    {
                        port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));
                    }
                }
                if (bShowEdit)
                {
                    if (pNode.BindNode.HasInArgv(port.variable))
                        DrawPropertyGUI.DrawVariable(pNode, port, pNode.Inputs[pNode.BindNode.IndexofInArgv(port.variable)]);
                    else DrawPropertyGUI.DrawVariable(pNode, port);
                }
                pNode.Outputs.Add(port);
            }

            if(pAction!=null)
            {
                string strLabel = "";
                switch ((EActionType)pNode.BindNode.GetExcudeHash())
                {
                    case EActionType.Condition_IfElse:
                        {
                            strLabel = "条件{0}";
                        }
                        break;
                    case EActionType.Condition_IfAnd:
                        {
                            strLabel = "且{0}";
                        }
                        break;
                    case EActionType.Condition_IfOr:
                        {
                            strLabel = "或{0}";
                        }
                        break;
                    case EActionType.Condition_Switch:
                        {
                            strLabel = "信号{0}";
                        }
                        break;
                    case EActionType.Condition_Where:
                        {
                            strLabel = "当{0}";
                        }
                        break;
                    case EActionType.Condition_Parallel:
                        {
                            strLabel = "并行{0}";
                        }
                        break;
                    case EActionType.Condition_Sync:
                        {
                            strLabel = "同步{0}";
                        }
                        break;
                    case EActionType.Condition_FrameDo:
                        {
                            strLabel = "帧执行{0}";
                        }
                        break;
                }
                if (pNode.BindNode.GetExcudeHash() == (int)EActionType.Condition_Where)
                {
                    if (pAction.condition.portals != null && pAction.condition.portals.Count > 1)
                    {
                        pAction.condition.portals.RemoveRange(1, pAction.condition.portals.Count - 1);
                    }
                }
                if (pAction.condition.portals != null)
                {
                    if (pNode.OutConditionLinks.Count != pAction.condition.portals.Count)
                    {
                        pNode.OutConditionLinks = new List<ConditionLinkPort>(pAction.condition.portals.Count);
                        for (int i = 0; i < pAction.condition.portals.Count; ++i)
                        {
                            pNode.OutConditionLinks.Add(new ConditionLinkPort());
                        }
                    }
                    Color color = GUI.color;
                    for (int i = 0; i < pAction.condition.portals.Count; ++i)
                    {
                        PortalNode pN = pAction.condition.portals[i];
                        if (pAction.actionType == EActionType.Condition_Switch)
                        {
                            pN.opType = ECondOpType.ValueEqual;
                        }
                        AgentTreeUtl.BeginHorizontal();
                        GUI.color = Color.red;
                        if (GUILayout.Button(string.Format(strLabel, i + 1)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "是否删除?", "是", "否"))
                            {
                                pAction.condition.portals.RemoveAt(i);
                                pNode.OutConditionLinks.RemoveAt(i);
                                break;
                            }
                        }
                        GUI.color = color;

                        if (hasPortalPort)
                        {
                            ConditionLinkPort link = pNode.OutConditionLinks[i];
                            link.baseNode = pNode;
                            link.partalNode = pN;
                            link.index = i + 1;
                            link.direction = EPortIO.Out;
                            Rect rect = GUILayoutUtility.GetLastRect();
                            GraphNode.LinkField(new Vector2(rect.width + 14, rect.y + 2), link);
                        }

                        AgentTreeUtl.EndHorizontal();

                        bool bHasArgv = true;
                        EditorGUIUtility.labelWidth = 50;
                        if (pAction.actionType == EActionType.Condition_Sync || pAction.actionType == EActionType.Condition_FrameDo)
                        {
                            bHasArgv = false;
                        }
                        else
                            pN.opType = (ECondOpType)PopEnum<ECondOpType>("操作", pN.opType);
                        EditorGUIUtility.labelWidth = labelWidth;

                        if (bHasArgv)
                        {
                            if (pN.argv == null || pN.argv.GetVariable(null) == null || pN.argv.GetVariable(null).GetType() != pInArgv.GetType())
                            {
                                pNode.Editor?.AdjustMaxGuid();
                                if (pN.argv == null) pN.argv = new Port(null);
                                pN.argv.variable = pVarsFactor.NewVariableByType(pInArgv.GetType());
                            }

                            {
                                pN.argv.variable.strName = "参数2";
                                ArgvPort port = pN.argv.GetEditorer<ArgvPort>();
                                port.baseNode = pNode;
                                //       port.variable = pN.argv.GetVariable();
                                port.port = pN.argv;
                                port.direction = EPortIO.In;
                                port.index = 1;
                                port.portalNode = pN;
                                port.displayType = DisplayType1;
                                port.alignType = AlignType1;
                                DrawPropertyGUI.DrawVariable(pNode, port);
                                pNode.Inputs.Add(port);
                            }
                        }
                        else
                        {
                            pN.argv = null;
                        }

                        if (pN.opType == ECondOpType.InView || pN.opType == ECondOpType.OutView || pN.opType == ECondOpType.PositionDistance)
                        {
                            if (pN.compare == null || pN.compare.variable == null || pN.compare.variable.GetType() != typeof(VariableFloat))
                            {
                                pN.compare.variable = pVarsFactor.NewVariableByType(typeof(VariableFloat));
                                pN.compare.variable.strName = "比较值";
                            }
                        }
                        else
                            pN.compare = null;

                        if (pN.compare != null && pN.compare.variable != null)
                        {
                            ArgvPort port = pN.compare.GetEditorer<ArgvPort>();
                            port.baseNode = pNode;
                            //          port.variable = pN.compare.variable;
                            port.port = pN.compare;
                            port.direction = EPortIO.In;
                            port.index = 2;
                            port.portalNode = pN;
                            port.displayType = DisplayType1;
                            port.alignType = AlignType1;
                            DrawPropertyGUI.DrawVariable(pNode, port);
                            pNode.Inputs.Add(port);
                        }
                    }
                }

                if (pAction.actionType == EActionType.Condition_Where && pAction.condition.portals != null && pAction.condition.portals.Count > 0)
                {

                }
                else
                {
                    if (GUILayout.Button("添加语句"))
                    {
                        if (pAction.condition.portals == null)
                            pAction.condition.portals = new List<PortalNode>();
                        pAction.condition.portals.Add(new PortalNode());
                        pNode.OutConditionLinks.Add(new ConditionLinkPort());
                    }
                }
            }

            

            return true;
        }
        //------------------------------------------------------
        static bool DrawIfAndOrCondition(GraphNode pNode)
        {
            bool bChanged = false;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;
            long preValue = pNode.BindNode.GetCustomValue();
           // ATExportNodeAttrData exportData = null;
            AgentTreeEditorUtils.vPopVariables.Clear();

            ActionNode pAction = pNode.BindNode as ActionNode;
            if (pAction != null && pAction.condition == null)
            {
                pAction.condition = new Condition();
            }

            bChanged = preValue != pNode.BindNode.GetCustomValue();
            VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
            EditorGUIUtility.labelWidth = labelWidth;
            AgentTreeEditorUtils.vPopVariables.Clear();
            if (!bChanged && pAction!=null && pAction.condition.portals!=null && pAction.condition.portals.Count>0 && (pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length != pAction.condition.portals.Count)) bChanged = true;
            if (bChanged)
            {
                List<Port> vPort = pNode.BindNode.inArgvs!=null? new List<Port>(pNode.BindNode.inArgvs.ToArray()):new List<Port>();
                pNode.BindNode.ClearArgv();
                pNode.Editor?.AdjustMaxGuid();
                for(int i =0; i < pAction.condition.portals.Count; ++i)
                {
                    PortalNode pN = pAction.condition.portals[i];
                    if (pN.argv == null || pN.argv.GetVariable(null) == null)
                    {
                        pNode.Editor?.AdjustMaxGuid();
                        if (pN.argv == null) pN.argv = new Port(null);
                        pN.argv.variable = pVarsFactor.NewVariableByType(typeof(Plugin.AT.VariableBool));
                        pN.argv.variable.strName = "参数2";
                    }
                    Variable pvar = null;
                    pvar = pVarsFactor.NewVariableByType(pN.argv.variable.GetType());
                    pvar.strName = "参数1";
                    pNode.BindNode.AddInArgv(pvar);
                    if(i<vPort.Count)
                    {
                        vPort[i].variable = pvar;
                        pNode.BindNode.inArgvs[i] = vPort[i];
                    }
                }
                pNode.BindNode.Save();
            }
            ATExportNodeAttrData attrData = pNode.Editor?.GetActionNodeAttr((int)pNode.BindNode.GetExcudeHash());
            if (attrData == null || attrData.nolinkAttr == null)
            {
                pNode.bLink = true;
                Rect rect = GUILayoutUtility.GetLastRect();
                pNode.InLink.baseNode = pNode;
                pNode.InLink.direction = EPortIO.In;
                GraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);
                pNode.OutLink.baseNode = pNode;
                pNode.OutLink.direction = EPortIO.Out;
                GraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);
            }
            else pNode.bLink = false;
           
            if(pAction!=null)
            {
                string strLabel = "";
                switch ((EActionType)pNode.BindNode.GetExcudeHash())
                {
                    case EActionType.Condition_IfElse:
                        {
                            strLabel = "条件{0}";
                        }
                        break;
                    case EActionType.Condition_IfAnd:
                        {
                            strLabel = "且{0}";
                        }
                        break;
                    case EActionType.Condition_IfOr:
                        {
                            strLabel = "或{0}";
                        }
                        break;
                    case EActionType.Condition_Switch:
                        {
                            strLabel = "信号{0}";
                        }
                        break;
                    case EActionType.Condition_Where:
                        {
                            strLabel = "当{0}";
                        }
                        break;
                    case EActionType.Condition_Parallel:
                        {
                            strLabel = "并行{0}";
                        }
                        break;
                    case EActionType.Condition_Sync:
                        {
                            strLabel = "同步{0}";
                        }
                        break;
                    case EActionType.Condition_FrameDo:
                        {
                            strLabel = "帧执行{0}";
                        }
                        break;
                }
                if (pAction.condition.portals != null)
                {
                    foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
                    {
                        AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
                    }
                    Color color = GUI.color;
                    for (int i = 0; i < pAction.condition.portals.Count; ++i)
                    {
                        PortalNode pN = pAction.condition.portals[i];

                        Variable pInArgv = pNode.BindNode.GetInVariable(i);
                        Type DisplayType1 = null;
                        Type AlignType1 = null;
                        ArgvPort enterPort = pNode.BindNode.GetInEditorPort<ArgvPort>(i);
                        if (enterPort != null && enterPort.port != null && enterPort.port.dummyMap != null && pNode.Editor != null)
                        {
                            foreach (var db in enterPort.port.dummyMap)
                            {
                                GraphNode pBindNode = pNode.Editor.GetGraphNode(db.Key);
                                if (pBindNode != null && pBindNode.BindNode != null)
                                {
                                    Port dummPort = pBindNode.BindNode.GetOutPort(pBindNode.BindNode.IndexofOutArgv(db.Value));
                                    if (dummPort != null && dummPort.Editorer != null && dummPort.Editorer is ArgvPort)
                                    {
                                        if (AlignType1 == null)
                                            AlignType1 = (dummPort.Editorer as ArgvPort).alignType;
                                        if (DisplayType1 == null)
                                            DisplayType1 = (dummPort.Editorer as ArgvPort).displayType;
                                    }
                                }
                                if (AlignType1 != null && DisplayType1 != null) break;
                            }
                        }

                        AgentTreeUtl.BeginHorizontal();
                        GUI.color = Color.red;
                        if (GUILayout.Button(string.Format(strLabel, i + 1)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "是否删除?", "是", "否"))
                            {
                                pAction.condition.portals.RemoveAt(i);
                                pNode.OutConditionLinks.RemoveAt(i);
                                break;
                            }
                        }
                        GUI.color = color;
                        AgentTreeUtl.EndHorizontal();

                        preValue = (int)VariableSerializes.GetVariableType(pInArgv.GetType());
                        int index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());
                        EditorGUIUtility.labelWidth = 50;
                        index = EditorGUILayout.Popup("类型", index, AgentTreeEditorUtils.vPopVariables.ToArray());
                        EditorGUIUtility.labelWidth = labelWidth;
                        if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count)
                        {
                            index = (int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]);
                            if (index != preValue)
                            {
                                pNode.BindNode.inArgvs[i].variable = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType((EVariableType)index), pNode.BindNode.inArgvs[i].variable.GUID);
                            }
                        }

                        {
                            if (pN.argv.GetVariable(null).GetType() != pInArgv.GetType())
                            {
                                pNode.Editor?.AdjustMaxGuid();
                                if (pN.argv == null) pN.argv = new Port(null);
                                pN.argv.variable = pVarsFactor.NewVariableByType(pInArgv.GetType());
                            }

                            {
                                enterPort.baseNode = pNode;
                                //     port.variable = pNode.BindNode.GetInVariable(i);
                                enterPort.port = pNode.BindNode.GetInPort(i);
                                enterPort.direction = EPortIO.In;
                                enterPort.index = i;
                                bool bShowEdit = true;
                                if (attrData != null && i < attrData.InArgvs.Count)
                                {
                                    enterPort.SetDefaultName(attrData.InArgvs[i].DisplayName);
                                    enterPort.alignType = attrData.InArgvs[i].AlignType;
                                    enterPort.displayType = attrData.InArgvs[i].DisplayType;
                                    bShowEdit = attrData.InArgvs[i].bShowEdit;
                                    if (!bShowEdit && enterPort.alignType != null)
                                    {
                                        enterPort.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(enterPort.alignType));
                                    }
                                }
                                if (enterPort.alignType == null) enterPort.alignType = AlignType1;
                                if (enterPort.displayType == null) enterPort.displayType = DisplayType1;
                                if (bShowEdit) DrawPropertyGUI.DrawVariable(pNode, enterPort);
                                pNode.Inputs.Add(enterPort);
                            }

                            EditorGUIUtility.labelWidth = 50;
                            pN.opType = (ECondOpType)PopEnum<ECondOpType>("操作", pN.opType);
                            EditorGUIUtility.labelWidth = labelWidth;


                            {
                                ArgvPort port = pN.argv.GetEditorer<ArgvPort>();
                                port.baseNode = pNode;
                                //       port.variable = pN.argv.GetVariable();
                                port.port = pN.argv;
                                port.direction = EPortIO.In;
                                port.index = 1;
                                port.portalNode = pN;
                                port.displayType = DisplayType1;
                                port.alignType = AlignType1;
                                DrawPropertyGUI.DrawVariable(pNode, port);
                                pNode.Inputs.Add(port);
                            }
                        }
                        pN.compare = null;

                        if (pN.compare != null && pN.compare.variable != null)
                        {
                            ArgvPort port = pN.compare.GetEditorer<ArgvPort>();
                            port.baseNode = pNode;
                            //          port.variable = pN.compare.variable;
                            port.port = pN.compare;
                            port.direction = EPortIO.In;
                            port.index = 2;
                            port.portalNode = pN;
                            port.displayType = DisplayType1;
                            port.alignType = AlignType1;
                            DrawPropertyGUI.DrawVariable(pNode, port);
                            pNode.Inputs.Add(port);
                        }
                    }
                }

                if (GUILayout.Button("添加语句"))
                {
                    if (pAction.condition.portals == null)
                        pAction.condition.portals = new List<PortalNode>();

                    PortalNode pN = new PortalNode();
                    pNode.Editor?.AdjustMaxGuid();
                    if (pN.argv == null) pN.argv = new Port(null);
                    pN.argv.variable = pVarsFactor.NewVariableByType(typeof(VariableBool));
                    pAction.condition.portals.Add(pN);
                }
            }
            

            return true;
        }
    }
}
#endif
