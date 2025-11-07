#if UNITY_EDITOR && USE_GUIDESYSTEM
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.Guide
{
    public class ExcudeNodeDraw 
    {
        public static void Draw(GraphNode pGraph, ExcudeNode pNode)
        {
//             int selIndex = GuideEditor.Instance.ExportExcudeTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideEditor.Instance.ExportExcudeTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideEditor.Instance.ExportExcudeTypes.Count)
//             {
//                 int type = GuideEditor.Instance.ExportExcudeTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideEditor.NodeAttr nodeAttr;
            if (!GuideEditor.Instance.ExcudeTypes.TryGetValue(pNode.type, out nodeAttr))
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

            pNode.bFireCheck = EditorGUILayout.Toggle("触发检测", pNode.bFireCheck);

           
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
                pNode._Ports[i].bindType = displayType;
                pNode._Ports[i].enumDisplayType = bBit;
                pGraph.DrawPort(port, strLabel, displayType, falg == EArgvFalg.All || falg == EArgvFalg.Get || falg == EArgvFalg.PortAll || falg == EArgvFalg.SetAndPort, (EBitGuiType)bBit, falg);
                pGraph.Port.Add(port);
            }

            if (GUILayout.Button("执行前事件" + "[" + pNode.GetBeginEvents().Count + "]"))
            {
                GuideEditor.Instance.OpenEventInspector(pNode.GetBeginEvents());
            }
            if (GUILayout.Button("执行后事件" + "[" + pNode.GetEndEvents().Count + "]"))
            {
                GuideEditor.Instance.OpenEventInspector(pNode.GetEndEvents());
            }
        }
    }
}
#endif