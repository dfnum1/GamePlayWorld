/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	TriggerNodeDraw
作    者:	
描    述:	触发器绘制逻辑
*********************************************************************/
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    public class TriggerNodeDraw 
    {
        public static void Draw(GraphNode pGraph, TriggerNode pNode)
        {
//             int selIndex = GuideSystemEditor.Instance.ExportTriggerTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideSystemEditor.Instance.ExportTriggerTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideSystemEditor.Instance.ExportTriggerTypes.Count)
//             {
//                 int type = GuideSystemEditor.Instance.ExportTriggerTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideSystemEditor.NodeAttr nodeAttr;
            if (!GuideSystemEditor.TriggerTypes.TryGetValue(pNode.type, out nodeAttr))
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
                if (displayType == null)
                {
                    displayType = PortUtil.GetDisplayType(port.port.GetGuid());
                    if (displayType != null) bBit = PortUtil.GetDisplayTypeBit(port.port.GetGuid());
                }

                pNode._Ports[i].bindType = displayType;
                pNode._Ports[i].enumDisplayType = bBit;
                pGraph.DrawPort(port, new GUIContent(strLabel, port.GetTips()), displayType, false, (EBitGuiType)bBit, falg);
                pGraph.Port.Add(port);
                PortUtil.SetDisplayType(port.port.GetGuid(), displayType, bBit);
            }
            //if (GUILayout.Button("事件列表"+ "[" + (pNode.vEvents!=null?pNode.vEvents.Count.ToString():"0") + "]"))
            //{
            //    if (pNode.vEvents == null) pNode.vEvents = new List<IUserData>();
            //    GuideSystemEditor.Instance.OpenEventInspector(pNode.vEvents);
            //}
        }
    }
}
#endif