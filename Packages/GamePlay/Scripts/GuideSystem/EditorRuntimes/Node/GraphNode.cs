/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GraphNode
作    者:	
描    述:	基础绘制节点
*********************************************************************/
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    public class GraphNode 
    {
        public BaseNode bindNode;
        public GuideGroup groupData;

        public List<IPortNode> Port = new List<IPortNode>();

        public GraphNode Parent = null;
        public List<GraphNode> Links = new List<GraphNode>();

        public bool bLinkIn = true;
        public LinkPort linkInPort = new LinkPort();

        public bool bLinkOut = false;
        public LinkPort linkOutPort = new LinkPort();

        public Vector2 CalcSize = Vector2.zero;
        public float widthOffset = 0;

        public List<ExternPort> vExternPorts = new List<ExternPort>();

        public GraphNode(GuideGroup asset, BaseNode pNode)
        {
            if (asset == null) return;
            groupData = asset;
            bindNode = pNode;
        }
        //------------------------------------------------------
        public ExternPort GetExternPort(int externId)
        {
            for(int i = 0; i < vExternPorts.Count; ++i)
            {
                if (vExternPorts[i].externID == externId) return vExternPorts[i];
            }
            return null;
        }
        //------------------------------------------------------
        public void RemoveExternPort(int externId)
        {
            for (int i = 0; i < vExternPorts.Count; ++i)
            {
                if (vExternPorts[i].externID == externId)
                {
                    vExternPorts.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public IPortNode ContainsPort(IPort port)
        {
            for(int i=0;i < Port.Count; ++i)
            {
                if (Port[i].GetPort() == port) return Port[i];
            }
            return null;
        }
        //------------------------------------------------------
        public void SetPosition(Vector2 pos)
        {
            bindNode.posX = (int)pos.x;
            bindNode.posY = (int)pos.y;
        }
        //------------------------------------------------------
        public Vector2 GetPosition()
        {
            return new Vector2(bindNode.posX, bindNode.posY);
        }
        //------------------------------------------------------
        public float GetWidth()
        {
            return Mathf.Max(200, CalcSize.x);
        }
        //------------------------------------------------------
        public float GetHeight()
        {
            return Mathf.Max(CalcSize.y, 30);
        }
        //------------------------------------------------------
        public int GetGUID()
        {
            return bindNode.Guid;
        }
        //------------------------------------------------------
        public Color GetTint()
        {
            return GuidePreferences.GetSettings().nodeBgColor;
        }
        //------------------------------------------------------
        public void OnHeaderGUI()
        {
            string tileName = bindNode.Name;
            if (bindNode is StepNode)
            {
                tileName ="步骤[" + bindNode.Name + "]";
            }
            else if (bindNode is TriggerNode)
            {
                tileName = "触发器[" + bindNode.Name + "]";
            }
            else if (bindNode is GuideOperate)
            {
                tileName = "操作器[" + bindNode.Name + "]";
            }
            else if (bindNode is ExcudeNode)
            {
                tileName = "执行器[" + bindNode.Name + "]";
            }
            if (GuideSystem.getInstance().vTrackingNodes.Contains(bindNode))
                tileName += "¤";
            CalcSize.x = GuideEditorResources.styles.nodeHeader.CalcSize(new GUIContent(tileName)).x+80+ widthOffset;
            GUILayout.Label(tileName, GuideEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        //------------------------------------------------------
        public void OnBodyGUI()
        {
            if (bindNode == null || groupData == null || Event.current.type == EventType.ScrollWheel) return;

            Port.Clear();
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            try
            {
                if (bindNode is StepNode)
                {
                    StepNodeDraw.Draw(this, bindNode as StepNode);
                }
                else if (bindNode is TriggerNode)
                {
                    TriggerNodeDraw.Draw(this, bindNode as TriggerNode);
                }
                else if (bindNode is GuideOperate)
                {
                    OpNodeDraw.Draw(this, bindNode as GuideOperate);
                }
                else if (bindNode is ExcudeNode)
                {
                    ExcudeNodeDraw.Draw(this, bindNode as ExcudeNode);
                }
            }
            catch
            {

            }

            EditorGUIUtility.labelWidth = labelWidth;
        }
        //------------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {
            if (GuideSystemEditor.Instance.OnNodeEditorPreview == null)
            {
                return;
            }
            if (bindNode == null)
            {
                GuideSystemEditor.Instance.OnNodeEditorPreviewVisible?.Invoke(null, new object[] { false });
                return;
            }
            SeqNode stepNode = bindNode as SeqNode;
            if (stepNode == null)
            {
                GuideSystemEditor.Instance.OnNodeEditorPreviewVisible?.Invoke(null, new object[] { false });
                return;
            }
            if (stepNode.GetEnumType() <= 0)
            {
                GuideSystemEditor.Instance.OnNodeEditorPreviewVisible?.Invoke(null, new object[] { false });
                return;
            }
            GuideSystemEditor.NodeAttr nodeAttr;
            if (!GuideSystemEditor.NodeTypes.TryGetValue(stepNode.GetEnumType(), out nodeAttr))
            {
                GuideSystemEditor.Instance.OnNodeEditorPreviewVisible?.Invoke(null, new object[] { false });
                return;
            }
            if(nodeAttr.previewEditor)
            {
                GuideSystemEditor.Instance.OnNodeEditorPreview.Invoke(null,new object[]{ bindNode });

                if (GuideSystemEditor.NodeSceneDraws.TryGetValue(bindNode.GetEnumType(), out var drawScene) && drawScene != null)
                {
                    var vParams = drawScene.GetParameters();
                    if (vParams.Length == 2)
                    {
                        if (vParams[0].ParameterType == typeof(BaseNode) && vParams[1].ParameterType == typeof(SceneView))
                        {
                            drawScene.Invoke(null, new object[] { bindNode, sceneView });
                        }
                        if (vParams[1].ParameterType == typeof(BaseNode) && vParams[0].ParameterType == typeof(SceneView))
                        {
                            drawScene.Invoke(null, new object[] { bindNode, sceneView });
                        }
                    }
                }
            }
            else
                GuideSystemEditor.Instance.OnNodeEditorPreviewVisible?.Invoke(null, new object[] { false });
        }
        //------------------------------------------------------
        bool IsUnityObject(System.Type baseType)
        {
            if (baseType == null) return false;
            if (baseType.IsSubclassOf(typeof(UnityEngine.Object))) return true;
            System.Type temp = baseType;
            while (temp != null)
            {
                if (temp.IsSubclassOf(typeof(UnityEngine.Object))) return true;
                temp = temp.BaseType;
            }
            return false;
        }
        //------------------------------------------------------
        public object DrawProperty(GUIContent strLabel, object pOwnerData, object pValue, string valueFiledName, System.Type displayType, EBitGuiType bBit = EBitGuiType.None, GUILayoutOption[] layoutOps = null)
        {
            if (displayType == null) return null;
            if (displayType.IsEnum)
            {
                if (pValue.GetType() == typeof(System.Int32))
                {
                    int val = (int)pValue;
                    return Convert.ToInt32(PopEnum((System.Enum)System.Enum.ToObject(displayType, val), strLabel, displayType, bBit));
                }
            }
            if (displayType == typeof(bool))
            {
                int val = (int)pValue;
                if (string.IsNullOrEmpty(strLabel.text))
                    return EditorGUILayout.Toggle(val != 0) ? 1 : 0;
                else
                    return EditorGUILayout.Toggle(strLabel, val != 0) ? 1 : 0;
            }
            if (displayType == typeof(float))
            {
                int val = (int)pValue;
                if (string.IsNullOrEmpty(strLabel.text))
                    return (int)(EditorGUILayout.FloatField(val*0.001f)*1000);
                else
                    return (int)(EditorGUILayout.FloatField(strLabel,val * 0.001f) * 1000);
            }
            if (displayType == typeof(Color))
            {
                int val = (int)pValue;
                if (val == 0) val = -939524096;//0xC8000000
                Color col = new Color(
                ((val & 0x00ff0000) >> 16) / 255f,
                ((val & 0x0000ff00) >> 8) / 255f,
                ((val & 0x000000ff)) / 255f,
                ((val & 0xff000000) >> 24) / 255f);
                if (string.IsNullOrEmpty(strLabel.text))
                    col = EditorGUILayout.ColorField(col);
                else
                    col = EditorGUILayout.ColorField(strLabel, col);

                return (int)(((uint)(col.a * 255f) << 24) | ((uint)(col.r * 255f) << 16) | ((uint)(col.g * 255f) << 8) | (uint)(col.b * 255f));
            }
            if (displayType == typeof(UnityEngine.Vector2))
            {
                int val = (int)pValue;
                if (val == 0) val = 0;
                float x = Math.Clamp((int)(val >> 16),-65535,65535)*0.01f;
                float y = Math.Clamp((int)(val & 0xffff), -65535, 65535) * 0.01f;
                Vector2 col = new Vector2(x,y);
                if (string.IsNullOrEmpty(strLabel.text))
                    col = EditorGUILayout.Vector2Field("",col);
                else
                    col = EditorGUILayout.Vector2Field(strLabel, col);
                return (int)(((int)Mathf.Clamp((int)(col.x * 100f),-65535,65535) << 16) | (int)((int)Mathf.Clamp((int)(col.y * 100f), -65535, 65535)));
            }

            if (displayType!=null && GuideSystemEditor.TypeDisplayTypes.TryGetValue(displayType, out var displayAttr))
            {
                if (displayAttr.Draw(this, strLabel, pOwnerData, valueFiledName))
                    return "-";//上层不做任何赋值

                if(pValue.GetType() == typeof(int))
                {
                    int nValue = (int)pValue;
                    if (displayAttr.Draw(this, strLabel, ref nValue))
                        return nValue;
                }
                else if (pValue.GetType() == typeof(string))
                {
                    string strValue = (string)pValue;
                    if (displayAttr.Draw(this,strLabel, ref strValue))
                        return strValue;
                }
            }
            if (IsUnityObject(displayType))
            {
                if (pValue != null && pValue.GetType() == typeof(string))
                {
                    string strFile = (string)pValue;
                    UnityEngine.Object pRet = null;
                    if (string.IsNullOrEmpty(strLabel.text))
                        pRet = EditorGUILayout.ObjectField(AssetDatabase.LoadMainAssetAtPath(strFile), displayType, true);
                    else
                        pRet = EditorGUILayout.ObjectField(strLabel, AssetDatabase.LoadMainAssetAtPath(strFile), displayType, true);
                    if (pRet != null) return AssetDatabase.GetAssetPath(pRet);
                    return "";
                }
            }
            else
            {
                if (pValue != null && pValue.GetType() == typeof(string))
                {
                    string strFile = (string)pValue;
                    strFile = EditorGUILayout.TextField(strLabel, strFile);
                    return strFile;
                }
            }
            return null;
        }
        //------------------------------------------------------
        static List<string> EnumPops = new List<string>();
        static List<Enum> EnumValuePops = new List<Enum>();
        public static object PopEnum(Enum val, GUIContent strDisplayName, System.Type enumType, EBitGuiType type = EBitGuiType.None, GUILayoutOption[] op = null, System.Type displayType = null)
        {
            EnumPops.Clear();
            EnumValuePops.Clear();
            int index = -1;
            if (type == EBitGuiType.None)
            {
                foreach (Enum v in Enum.GetValues(enumType))
                {
                    FieldInfo fi = enumType.GetField(v.ToString());
                    string strTemName = v.ToString();
                    if (fi != null && fi.IsDefined(typeof(GuideDisableAttribute)))
                    {
                        var disType = fi.GetCustomAttribute<GuideDisableAttribute>().disableType;
                        if(disType!=null)
                        {
                            if (displayType != null)
                            {
                                if (disType == displayType)
                                    continue;
                            }
                        }
                        else
                            continue;
                    }
                    if (fi != null && fi.IsDefined(typeof(GuideDisplayAttribute)))
                    {
                        strTemName = fi.GetCustomAttribute<GuideDisplayAttribute>().displayName;
                    }
                    if (fi != null && fi.IsDefined(typeof(InspectorNameAttribute)))
                    {
                        strTemName = fi.GetCustomAttribute<InspectorNameAttribute>().displayName;
                    }
                    EnumPops.Add(strTemName);
                    EnumValuePops.Add(v);
                    if (v.ToString().CompareTo(val.ToString()) == 0)
                        index = EnumPops.Count - 1;
                }

                if (strDisplayName == null || string.IsNullOrEmpty(strDisplayName.text))
                    index = EditorGUILayout.Popup(index, EnumPops.ToArray(), op);
                else
                {
                    float labelWidthBack = EditorGUIUtility.labelWidth;
                    if (!string.IsNullOrEmpty(strDisplayName.text))
                    {
                        EditorGUIUtility.labelWidth = GUI.skin.textField.CalcSize(strDisplayName).x;
                    }
                    index = EditorGUILayout.Popup(strDisplayName, index, EnumPops.ToArray(), op);
                    EditorGUIUtility.labelWidth = labelWidthBack;
                }
                if (index >= 0 && index < EnumValuePops.Count)
                {
                    val = EnumValuePops[index];
                }
            }
            else
            {
                int flag = Convert.ToInt32(val);
                if (!string.IsNullOrEmpty(strDisplayName.text))
                {
                    float labelWidthBack = EditorGUIUtility.labelWidth;
                    if (!string.IsNullOrEmpty(strDisplayName.text))
                    {
                        EditorGUIUtility.labelWidth = GUI.skin.textField.CalcSize(strDisplayName).x + 5;
                    }
                    EditorGUILayout.LabelField(strDisplayName);
                    EditorGUIUtility.labelWidth = labelWidthBack;
                }
                EditorGUI.indentLevel++;
                foreach (Enum v in Enum.GetValues(enumType))
                {
                    int flagValue = Convert.ToInt32(v);
                    if (type == EBitGuiType.Offset) flagValue = 1 << flagValue;

                    FieldInfo fi = enumType.GetField(v.ToString());
                    string strTemName = v.ToString();
                    if (fi != null && fi.IsDefined(typeof(GuideDisableAttribute)))
                    {
                        var disType = fi.GetCustomAttribute<GuideDisableAttribute>().disableType;
                        if (disType != null)
                        {
                            if (displayType != null)
                            {
                                if (disType == displayType)
                                    continue;
                            }
                        }
                        else
                            continue;
                    }
                    if (fi != null && fi.IsDefined(typeof(GuideDisplayAttribute)))
                    {
                        strTemName = fi.GetCustomAttribute<GuideDisplayAttribute>().displayName;
                    }
                    bool bToggle = EditorGUILayout.Toggle(strTemName, (flag & flagValue) != 0);
                    if (bToggle)
                    {
                        flag |= (int)flagValue;
                    }
                    else flag &= (int)(~flagValue);
                }
                EditorGUI.indentLevel--;
                return flag;
            }
            return val;
        }
        //------------------------------------------------------
        public void DrawPort(IPortNode portNode, GUIContent strLabel, System.Type displayType = null, bool bEdit = true, EBitGuiType bBit = EBitGuiType.None, EArgvFalg bPort = EArgvFalg.PortAll, float newLabelWidth = 84)
        {
            bool bDraw = false;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = newLabelWidth;
            IPort port = portNode.GetPort();

            Rect view = Rect.zero;
            EditorGUI.BeginDisabledGroup(!bEdit);
            if (port is ArgvPort)
            {
                ArgvPort argv = port as ArgvPort;

                bool bString = false;
                if (displayType!=null&& GuideSystemEditor.TypeDisplayTypes.TryGetValue(displayType, out var displayAttr))
                {
                    bString = displayAttr.bStrValue; 
                }

                object retObj = null;
                if (displayType == typeof(String) || bString)
                {
                    FieldInfo field = argv.GetType().GetField("strValue", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        var strObj = field.GetValue(argv);
                        string strVal = "";
                        if (strObj != null) strVal = strObj.ToString();
                        retObj = DrawProperty(strLabel, argv, strVal, "strValue", displayType, bBit);
                        if(retObj != null && retObj.ToString() != "-")
                        {
                            field.SetValue(port, retObj.ToString());
                        }
                    }
                }
                else
                {
                    FieldInfo field = argv.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        int value = (int)field.GetValue(argv);
                        retObj = DrawProperty(strLabel, argv, value, "value", displayType, bBit);
                        if (retObj != null && int.TryParse(retObj.ToString(), out var intVal))
                        {
                            field.SetValue(port, intVal);
                        }
                    }
                }
                if (retObj == null)
                {
                    if ((portNode.GetAttribute() != null && portNode.GetAttribute() is GuideStrArgvAttribute) ||
                        (portNode.GetAttribute() != null && displayType == typeof(String)))
                    {
                        FieldInfo field = argv.GetType().GetField("strValue", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            var fieldObj = field.GetValue(argv);
                            string strValue = "";
                            if (fieldObj != null) strValue = fieldObj.ToString();
                            strValue = EditorGUILayout.TextField(strLabel, strValue);
                            field.SetValue(argv, strValue);
                        }
                    }
                    else
                    {
                        FieldInfo field = argv.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            int value = (int)field.GetValue(argv);
                            object ret = DrawProperty(strLabel, port, value, "value", displayType, bBit);
                            if (ret == null)
                                value = EditorGUILayout.IntField(strLabel, value);
                            else
                            {
                                if(int.TryParse(ret.ToString(), out var portVal))
                                    value = portVal;
                            }
                            field.SetValue(argv, value);
                        }
                    }
                }

                bDraw = true;
                view = GUILayoutUtility.GetLastRect();
            }
            else if (port is VarPort)
            {
                VarPort varPort = port as VarPort;
                varPort.type = (EOpType)PopEnum(varPort.type, strLabel, typeof(EOpType), EBitGuiType.None, new GUILayoutOption[] { GUILayout.Width(60) }, displayType);
                view = GUILayoutUtility.GetLastRect();
                bool bString = false;
                if (displayType!=null && GuideSystemEditor.TypeDisplayTypes.TryGetValue(displayType, out var displayAttr))
                {
                    bString = displayAttr.bStrValue;
                }

                object retObj = null;
                if (displayType == typeof(String) || bString)
                {
                    if (varPort.strValue == null) varPort.strValue = "";
                    retObj = DrawProperty(new GUIContent(""), port, varPort.strValue, "strValue", displayType, bBit);
                }
                else
                    retObj = DrawProperty(new GUIContent(""), port, varPort.value, "value", displayType, bBit);


                if (retObj == null)
                    varPort.value = EditorGUILayout.IntField(varPort.value);
                else
                {
                    if (displayType == typeof(String))
                    {
                        if(retObj.ToString()!= "-")
                            varPort.strValue = retObj.ToString();
                    }
                    else
                    {
                        if(int.TryParse(retObj.ToString(), out var portVal))
                            varPort.value = portVal;
                    }
                }
                bDraw = true;
            }
            EditorGUI.EndDisabledGroup();
            if (bDraw)
            {
                if(((int)bPort & (int)EArgvFalg.GetAndPort)!=0 || ((int)bPort & (int)EArgvFalg.SetAndPort) != 0)
                    DrawPort(portNode, view);
            }


            EditorGUIUtility.labelWidth = labelWidth;
        }
        //------------------------------------------------------
        public void DrawPort(IPortNode portNode, Rect view )
        {
            Vector2 position = view.position;
            if (portNode.IsInput())
            {
                position -= new Vector2(16, 0);
            }
            if (portNode.IsOutput())
            {
                position += new Vector2(view.width, 0);
            }
            GraphNode.PortField(position, portNode);
            portNode.SetViewRect(view);
        }
        //------------------------------------------------------
        public static void DrawLinkHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
    //        GUI.DrawTexture(rect, AgentTreeEditorResources.linkOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, GuideEditorResources.linkOuter);
            GUI.color = col;
        }
        //------------------------------------------------------
        public static void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, GuideEditorResources.dotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, GuideEditorResources.dot);
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
    }
}
#endif