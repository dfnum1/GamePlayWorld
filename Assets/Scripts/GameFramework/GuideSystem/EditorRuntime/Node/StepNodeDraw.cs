#if UNITY_EDITOR && USE_GUIDESYSTEM
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.Guide
{
    public class StepNodeDraw 
    {
        public static void Draw(GraphNode pGraph, StepNode pNode)
        {
//             int selIndex = GuideEditor.Instance.ExportTypes.IndexOf(pNode.type);
//             selIndex = EditorGUILayout.Popup("类型", selIndex, GuideEditor.Instance.ExportTypesPop.ToArray());
//             if (selIndex >= 0 && selIndex < GuideEditor.Instance.ExportTypes.Count)
//             {
//                 int type = GuideEditor.Instance.ExportTypes[selIndex];
//                 pNode.type = type;
//             }
            GuideEditor.NodeAttr nodeAttr;
            if (!GuideEditor.Instance.StepTypes.TryGetValue(pNode.type, out nodeAttr))
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
            pNode.fDeltaTime = EditorGUILayout.FloatField("延时(s)", pNode.fDeltaTime);

            pNode.bAutoNext = EditorGUILayout.Toggle("自动跳转", pNode.bAutoNext);
            if(pNode.bAutoNext)
                pNode.fAutoTime = EditorGUILayout.FloatField("自动倒计时", pNode.fAutoTime);

            pNode.bAutoSignCheck = EditorGUILayout.Toggle("信号检测", pNode.bAutoSignCheck);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;
            pNode.bSuccessedListenerBreak = EditorGUILayout.Toggle("执行成功后中断监听", pNode.bSuccessedListenerBreak);
            EditorGUIUtility.labelWidth = labelWidth;
            if (pNode.bSuccessedListenerBreak)
            {
                Rect auto_rect = GUILayoutUtility.GetLastRect();
                ExternPort externPort = pGraph.GetExternPort(20);
                if(externPort == null)
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
                pNode._Ports[i].bindType = displayType;
                pNode._Ports[i].enumDisplayType = bBit;
                pGraph.DrawPort(port, strLabel, displayType, true, (EBitGuiType)bBit, falg);
                pGraph.Port.Add(port);
            }

            if(GUILayout.Button("执行前事件" + "[" + pNode.GetBeginEvents().Count + "]"))
            {
                GuideEditor.Instance.OpenEventInspector(pNode.GetBeginEvents());
            }
            if (GUILayout.Button("执行后事件" + "[" + pNode.GetEndEvents().Count + "]"))
            {
                GuideEditor.Instance.OpenEventInspector(pNode.GetEndEvents());
            }
            if (pNode.IsAutoNext())
            {
                EditorGUILayout.LabelField("结束后执行器");
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
            for (int i = 0; i < pGraph.vExternPorts.Count; ++i)
            {
                GraphNode.LinkField(pGraph.vExternPorts[i].portRect, pGraph.vExternPorts[i]);
            }
        }
    }
}
#endif