#if UNITY_EDITOR && USE_GUIDESYSTEM
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.Guide
{
    public class TriggerNodeDraw 
    {
        public static void Draw(GraphNode pGraph, TriggerNode pNode)
        {
//             int selIndex = GuideEditor.Instance.ExportTriggerTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideEditor.Instance.ExportTriggerTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideEditor.Instance.ExportTriggerTypes.Count)
//             {
//                 int type = GuideEditor.Instance.ExportTriggerTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideEditor.NodeAttr nodeAttr;
            if (!GuideEditor.Instance.TriggerTypes.TryGetValue(pNode.type, out nodeAttr))
                return;

            pGraph.bLinkOut = true;
            pGraph.bLinkIn = false;
            Rect rect = GUILayoutUtility.GetLastRect();
            pGraph.linkOutPort.baseNode = pGraph;
            pGraph.linkOutPort.direction = EPortIO.LinkOut;
            GraphNode.LinkField(new Vector2(rect.width + 10, 8), pGraph.linkOutPort);

            pNode.CheckPorts();
            pNode.bExpand = EditorGUILayout.Foldout(pNode.bExpand, "");
            if (!pNode.bExpand)
                return;

            pNode.priority = EditorGUILayout.IntField("优先级", pNode.priority);
            pNode.bFireCheck = true;
            for (int i = 0; i < pNode._Ports.Count; ++i)
            {
                SlotPort port = pNode._Ports[i].GetEditor<SlotPort>(pGraph.bindNode.Guid);
                port.baseNode = pGraph;
                port.port = pNode._Ports[i];
                port.direction = EPortIO.Out;
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
                    if (!string.IsNullOrEmpty(nodeAttr.argvs[i].attr.strTips))
                        port.SetTips(nodeAttr.argvs[i].attr.strTips);
                    else
                        port.SetTips(strLabel);
                    falg = nodeAttr.argvs[i].attr.Flag;

                    port.SetAttribute(nodeAttr.argvs[i].attr);
                }
                pNode._Ports[i].bindType = displayType;
                pNode._Ports[i].enumDisplayType = bBit;
                pGraph.DrawPort(port, strLabel, displayType, false, (EBitGuiType)bBit, falg);
                pGraph.Port.Add(port);
            }
            if (GUILayout.Button("事件列表"+ "[" + (pNode.vEvents!=null?pNode.vEvents.Count.ToString():"0") + "]"))
            {
                if (pNode.vEvents == null) pNode.vEvents = new List<IUserData>();
                GuideEditor.Instance.OpenEventInspector(pNode.vEvents);
            }
        }
    }
}
#endif