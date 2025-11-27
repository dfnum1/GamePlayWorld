/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	OpNodeDraw
作    者:	
描    述:	操作器绘制逻辑
*********************************************************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Framework.Guide.Editor
{
    public class OpNodeDraw 
    {
        public static void Draw(GraphNode pGraph, GuideOperate pNode)
        {
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

            for (int i = 0; i < pNode._Ports.Count; ++i)
            {
                SlotPort port = pNode._Ports[i].GetEditor<SlotPort>(pGraph.bindNode.Guid);
                port.baseNode = pGraph;
                port.port = pNode._Ports[i];
                port.direction = EPortIO.In;
                port.index = i;

                System.Type disp = null;
                EBitGuiType bit = EBitGuiType.None;
                if(pNode._Ports[i].dummyMaps!=null)
                {
                    foreach (var db in pNode._Ports[i].dummyMaps)
                    {
                        if(db.Value.bindType!=null)
                        {
                            disp = db.Value.bindType;
                            bit = (EBitGuiType)db.Value.enumDisplayType;
                        }
                        if(db.Value.enumDisplayType != 0)
                        {
                            bit = (EBitGuiType)db.Value.enumDisplayType;
                        }
                    }
                }

                if (disp != null && bit != EBitGuiType.None)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 20;

                    pNode._Ports[i].type = (EOpType)GraphNode.PopEnum(pNode._Ports[i].type, new GUIContent("IF"), typeof(EOpType), EBitGuiType.None, new GUILayoutOption[] { GUILayout.Width(60) }, disp);
                    Rect view = GUILayoutUtility.GetLastRect();
                    EditorGUIUtility.labelWidth = pGraph.GetWidth() - 50;
                    bool bString = false;
                    if (disp !=null && GuideSystemEditor.TypeDisplayTypes.TryGetValue(disp, out var displayAttr))
                    {
                        bString = displayAttr.bStrValue;
                    }
                    object ret = null;
                    if(disp == typeof(string) || bString)
                        ret = pGraph.DrawProperty(new GUIContent(""), pNode._Ports[i], pNode._Ports[i].value, "strValue", disp, bit);
                    else
                        ret = pGraph.DrawProperty(new GUIContent(""), pNode._Ports[i], pNode._Ports[i].strValue, "value", disp, bit);
                    if (ret == null)
                        pNode._Ports[i].value = EditorGUILayout.IntField(pNode._Ports[i].value);
                    else
                    {
                        if (disp == typeof(string) || bString)
                        {
                            if(ret.ToString()!="-")
                                pNode._Ports[i].strValue = ret.ToString();
                        }
                        else
                            pNode._Ports[i].value = System.Convert.ToInt32(ret);
                    }

                    pGraph.DrawPort(port, view);
                    EditorGUIUtility.labelWidth = 100;
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    string strLabel = "IF";

                    pGraph.DrawPort(port, new GUIContent(strLabel), disp, true, EBitGuiType.None, EArgvFalg.PortAll, 20);
                    if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(20) }))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确认删除改条目", "删除", "取消"))
                        {
                            pNode._Ports.RemoveAt(i);
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                }


                if (i + 1 < pNode._Ports.Count)
                {
                    bool bOr = (pNode.Relation & (1 << i)) != 0;
                    ERelationType type = bOr ? ERelationType.Or : ERelationType.And;
                    type = (ERelationType)GraphNode.PopEnum(type, null, typeof(ERelationType), EBitGuiType.None, new GUILayoutOption[] { GUILayout.Width(pGraph.GetWidth() - 50) });
                    if (type == ERelationType.And) pNode.Relation &= (byte)(~(1 << i));
                    else pNode.Relation |= (byte)((1 << i));
                }
                pGraph.Port.Add(port);
            }
            if (GUILayout.Button("添加"))
            {
                pNode._Ports.Add(new VarPort());
            }
        }
    }
}
#endif