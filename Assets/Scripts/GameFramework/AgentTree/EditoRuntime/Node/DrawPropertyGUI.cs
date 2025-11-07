#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
namespace Framework.Plugin.AT
{
	public class DrawPropertyGUI
	{
        //------------------------------------------------------
        public static System.Type PopVariableType(System.Type varType, string strLabel = "类型", GUILayoutOption[] options = null)
        {
            EVariableType varEnumType = VariableSerializes.GetVariableType(varType);
            AgentTreeEditorUtils.vPopVariables.Clear();
            foreach (EVariableType v in Enum.GetValues(typeof(EVariableType)))
            {
                AgentTreeEditorUtils.vPopVariables.Add(v.ToString());
            }
            int index = AgentTreeEditorUtils.vPopVariables.IndexOf(varEnumType.ToString());
            if (string.IsNullOrEmpty(strLabel))
            {
                index = EditorGUILayout.Popup(index, AgentTreeEditorUtils.vPopVariables.ToArray(), options);
            }
            else
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50;
                index = EditorGUILayout.Popup(strLabel, index, AgentTreeEditorUtils.vPopVariables.ToArray(), options);
                EditorGUIUtility.labelWidth = labelWidth;
            }
            if (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count)
            {
                index = (int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]);
                varType = VariableSerializes.GetVariableType((EVariableType)index);
            }
            return varType;
        }
        //------------------------------------------------------
        public static void DrawVariable(GraphNode pNode, IPortNode port, IPortNode pSamePort = null, bool bEdit = true, float labelWidth = 100, float widthOffse=0 )
		{
            float labelWidthBack = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            if (port is ArgvPort)
            {
                ArgvPort argv = (ArgvPort)port;
                if (argv.variable != null)
                {
                    DrawUIParam.Current.asignType  = argv.alignType;
                    DrawUIParam.Current.displayType = argv.displayType;
                    if(argv.displayType == null && argv.alignType == typeof(System.Type))
                    {
                        argv.displayType = argv.alignType;
                        DrawUIParam.Current.displayType = typeof(System.Type);
                    }
                    string addtiveName = "";
                    if(argv.variable.IsFlag(EFlag.Const)) addtiveName = "(常量)";
                    if (pNode.Editor != null)
                    {
                        StructData structData;
                        if (pNode.Editor.GetVariableStructs().TryGetValue(argv.variable.GUID, out structData))
                        {
                            addtiveName = "(结构[" + structData.structName + "]成员)";
                        }
                    }
                    DrawUIParam.Current.strDefaultName = port.GetDefaultName();

                    string label = (!string.IsNullOrEmpty(argv.variable.strName) ? argv.variable.strName : DrawUIParam.Current.strDefaultName);
                    if (argv.variable != null) argv.variable.strName = label;
                    label += addtiveName;
                    DrawUIParam.Current.offsetWidth = AgentTreeEditorResources.styles.nodeBody.CalcSize(new GUIContent(label)).x;
                    DrawUIParam.Current.size = new Vector2(pNode.GetWidth()+ widthOffse, 30);
                    DrawUIParam.Current.bEdit = bEdit;
                    Rect rect = (pSamePort ==null)? argv.variable.OnGUI(DrawUIParam.Current) : pSamePort.GetViewRect();
                    Vector2 titleCalc = pNode.offsetSize;
                    titleCalc.x = Mathf.Max(titleCalc.x, DrawUIParam.Current.offsetWidth/2);
                    pNode.offsetSize = titleCalc;
                    DrawUIParam.Current.offsetWidth = 0;
                    if (argv.IsInput())
                    {
                        Vector2 position = rect.position - new Vector2(16, 0);
                        GraphNode.PortField(position, argv);
                    }
                    if (argv.IsOutput())
                    {
                        Vector2 position = rect.position + new Vector2(rect.width, 0);
                        GraphNode.PortField(position, argv);
                    }
                    if(argv.variable!=null && argv.variable is VariableDelegate)
                    {
                        VariableDelegate varDelegate = (VariableDelegate)argv.variable;
                        varDelegate.OutLink.delegateVar = varDelegate;
                        pNode.OutDelegateLinks.Add(varDelegate.OutLink);

                        varDelegate.OutLink.baseNode = pNode;
                        varDelegate.OutLink.index = 1;
                        varDelegate.OutLink.direction = EPortIO.Out;
                        varDelegate.OutLink.rect = rect;
                        GraphNode.LinkField(new Vector2(rect.width + 14, rect.y + 2), varDelegate.OutLink);
                    }
                    port.SetViewRect(rect);
                }
            }
            EditorGUIUtility.labelWidth = labelWidthBack;
        }
	}
}
#endif
