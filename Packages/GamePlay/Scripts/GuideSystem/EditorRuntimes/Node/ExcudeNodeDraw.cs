/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ExcudeNodeDraw
作    者:	
描    述:	执行器节点绘制
*********************************************************************/
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Framework.Guide;
namespace Framework.Guide.Editor
{
    public class ExcudeNodeDraw 
    {
        public static void Draw(GraphNode pGraph, ExcudeNode pNode)
        {
//             int selIndex = GuideSystemEditor.Instance.ExportExcudeTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideSystemEditor.Instance.ExportExcudeTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideSystemEditor.Instance.ExportExcudeTypes.Count)
//             {
//                 int type = GuideSystemEditor.Instance.ExportExcudeTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideSystemEditor.NodeAttr nodeAttr;
            if (!GuideSystemEditor.ExcudeTypes.TryGetValue(pNode.type, out nodeAttr))
                return;

            pGraph.bLinkOut = true;
            pGraph.bLinkIn = true;
            Rect rect = GUILayoutUtility.GetLastRect();
            pGraph.linkInPort.baseNode = pGraph;
            pGraph.linkInPort.direction = EPortIO.LinkIn;
            GraphNode.LinkField(new Vector2(rect.x - 10, 8), pGraph.linkInPort);

            pGraph.linkOutPort.baseNode = pGraph;
            pGraph.linkOutPort.direction = EPortIO.LinkOut;
            GraphNode.LinkField(new Vector2(rect.width + 10, 8), pGraph.linkOutPort);

            pNode.CheckPorts();

            pNode.bExpand = EditorGUILayout.Foldout(pNode.bExpand, "");
            if (!pNode.bExpand)
                return;

            pNode.bFireCheck = EditorGUILayout.Toggle(new GUIContent("触发检测","勾选后，可将本次触发器的触发条件前置判断，如果满足，则触发该引导"), pNode.bFireCheck);

           
            for (int i = 0; i < pNode._Ports.Count; ++i)
            {
                SlotPort port = pNode._Ports[i].GetEditor<SlotPort>(pGraph.bindNode.Guid);
                port.baseNode = pGraph;
                port.port = pNode._Ports[i];
                port.index = i;
                int bBit = 0;
                EArgvFalg falg = EArgvFalg.None;
                System.Type displayType = null;
                string strLabel = "槽[" + (i + 1) + "]";
                if (i >= 0 && i < nodeAttr.argvs.Count)
                {
                    strLabel = nodeAttr.argvs[i].attr.DisplayName;
                    displayType = nodeAttr.argvs[i].attr.displayType;
                    bBit = (int)nodeAttr.argvs[i].bBit;
                    port.port.SetFlag( nodeAttr.argvs[i].attr.Flag);
                    falg = nodeAttr.argvs[i].attr.Flag;
                    if (!string.IsNullOrEmpty(nodeAttr.argvs[i].attr.strTips))
                        port.SetTips(nodeAttr.argvs[i].attr.strTips);
                    else
                        port.SetTips(strLabel);

                    port.SetAttribute(nodeAttr.argvs[i].attr);
                }
                if (EArgvFalg.Get == falg || EArgvFalg.GetAndPort == falg)
                    port.direction = EPortIO.Out;
                else
                    port.direction = EPortIO.In;

                if (displayType == null)
                {
                    displayType = PortUtil.GetDisplayType(port.port.GetGuid());
                    if (displayType != null) bBit = PortUtil.GetDisplayTypeBit(port.port.GetGuid());
                }
                pNode._Ports[i].bindType = displayType;
                pNode._Ports[i].enumDisplayType = bBit;
                pGraph.DrawPort(port, new GUIContent(strLabel, port.GetTips()), displayType, falg == EArgvFalg.All || falg == EArgvFalg.Get || falg == EArgvFalg.PortAll || falg == EArgvFalg.SetAndPort, (EBitGuiType)bBit, falg);
                pGraph.Port.Add(port);
                PortUtil.SetDisplayType(port.port.GetGuid(), displayType, bBit);
            }

            //if (GUILayout.Button("执行前事件" + "[" + pNode.GetBeginEvents().Count + "]"))
            //{
            //    GuideSystemEditor.Instance.OpenEventInspector(pNode.GetBeginEvents());
            //}
            //if (GUILayout.Button("执行后事件" + "[" + pNode.GetEndEvents().Count + "]"))
            //{
            //    GuideSystemEditor.Instance.OpenEventInspector(pNode.GetEndEvents());
            //}
        }
    }
}
#endif