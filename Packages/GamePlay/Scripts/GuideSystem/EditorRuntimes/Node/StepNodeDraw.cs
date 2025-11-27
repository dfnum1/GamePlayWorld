/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	StepNodeDraw
作    者:	
描    述:	步骤器绘制逻辑
*********************************************************************/
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    //------------------------------------------------------
    public class StepNodeDraw 
    {
        public static void Draw(GraphNode pGraph, StepNode pNode)
        {
//             int selIndex = GuideSystemEditor.Instance.ExportTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideSystemEditor.Instance.ExportTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideSystemEditor.Instance.ExportTypes.Count)
//             {
//                 int type = GuideSystemEditor.Instance.ExportTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideSystemEditor.NodeAttr nodeAttr;
            if (!GuideSystemEditor.StepTypes.TryGetValue(pNode.type, out nodeAttr))
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

            //       pNode.bFireCheck = EditorGUILayout.Toggle("触发检测", pNode.bFireCheck);
            pNode.bOption = EditorGUILayout.Toggle("非强制", pNode.bOption);
            //if(pNode.guideGroup!=null)
            //{
            //    if (pNode.guideGroup.Tag >= 0 && pNode.guideGroup.Tag < 65535)
            //        pNode.bMaster = EditorGUILayout.Toggle(new GUIContent("关键步骤", "如果勾选了关键步骤，则业务层需要发送服务器记录"), pNode.bMaster);
            //}
            pNode.fDeltaTime = EditorGUILayout.FloatField("延时(s)", pNode.fDeltaTime);

            {
                float labelWidthBack = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80;
                pNode.bAutoSignCheck = EditorGUILayout.Toggle(new GUIContent("自动检测信号","如果满足条件后，自动跳转到下一步"), pNode.bAutoSignCheck);
                EditorGUIUtility.labelWidth = labelWidthBack;
            }

            pNode.fDeltaSignTime = EditorGUILayout.FloatField("交互延时(s)", pNode.fDeltaSignTime);

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
                    if(!string.IsNullOrEmpty(nodeAttr.argvs[i].attr.strTips))
                        port.SetTips(nodeAttr.argvs[i].attr.strTips);
                    else
                        port.SetTips(strLabel);
                    falg = nodeAttr.argvs[i].attr.Flag;

                    port.SetAttribute(nodeAttr.argvs[i].attr);
                }
                if(EArgvFalg.Get == falg || EArgvFalg.GetAndPort == falg)
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
                pGraph.DrawPort(port, new GUIContent(strLabel, port.GetTips()), displayType, true, (EBitGuiType)bBit, falg);
                PortUtil.SetDisplayType(port.port.GetGuid(), displayType, bBit);
                pGraph.Port.Add(port);
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;
            pNode.bSuccessedListenerBreak = EditorGUILayout.Toggle("信号检测不成立时", pNode.bSuccessedListenerBreak);
            EditorGUIUtility.labelWidth = labelWidth;
            if (pNode.bSuccessedListenerBreak)
            {
                Rect auto_rect = GUILayoutUtility.GetLastRect();
                ExternPort externPort = pGraph.GetExternPort(20);
                if (externPort == null)
                {
                    externPort = new ExternPort();
                    pGraph.vExternPorts.Add(externPort);
                }
                externPort.baseNode = pGraph;
                externPort.direction = EPortIO.LinkOut;
                externPort.externID = 20;
                externPort.reqNodeType = null;
                externPort.portRect = new Vector2(auto_rect.width + 10, auto_rect.y);
            }
            else
            {
                pGraph.RemoveExternPort(20);
            }

            pNode.bAutoNext = EditorGUILayout.Toggle(new GUIContent("自动跳转","勾选后，不管是否满足步骤条件，都自动跳转到对应节点"), pNode.bAutoNext);
            if (pNode.bAutoNext)
            {
                pNode.fAutoTime = EditorGUILayout.FloatField("自动倒计时", pNode.fAutoTime);
            }
            if (pNode.IsAutoNext())
            {
                EditorGUILayout.LabelField("自动跳转节点");
                Rect auto_rect = GUILayoutUtility.GetLastRect();
                ExternPort externPort = pGraph.GetExternPort(10);
                if (externPort == null)
                {
                    externPort = new ExternPort();
                    pGraph.vExternPorts.Add(externPort);
                }
                externPort.baseNode = pGraph;
                externPort.direction = EPortIO.LinkOut;
                externPort.externID = 10;
                externPort.reqNodeType = typeof(ExcudeNode);
                externPort.portRect = new Vector2(auto_rect.width + 10, auto_rect.y);
            }
            else
            {
                pGraph.RemoveExternPort(10);
            }

            //if (GUILayout.Button("执行前事件" + "[" + pNode.GetBeginEvents().Count + "]"))
            //{
            //    GuideSystemEditor.Instance.OpenEventInspector(pNode.GetBeginEvents());
            //}
            //if (GUILayout.Button("执行后事件" + "[" + pNode.GetEndEvents().Count + "]"))
            //{
            //    GuideSystemEditor.Instance.OpenEventInspector(pNode.GetEndEvents());
            //}
            for (int i = 0; i < pGraph.vExternPorts.Count; ++i)
            {
                GraphNode.LinkField(pGraph.vExternPorts[i].portRect, pGraph.vExternPorts[i]);
            }
        }
    }
}
#endif