#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	MessageSimulationEditorLogic
作    者:	HappLI
描    述:	消息模拟逻辑
*********************************************************************/
using Framework.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class MessageSimulationEditorLogic : ISessionCallback
    {
        int m_nServerIndex = 0;
        List<ServerList.Item> m_vServers = new List<ServerList.Item>();
        List<string> m_vServerList = new List<string>();
        SimulationNetWork m_NetWork;

        class MidReqInfo
        {
            public int reqMid;
            public System.Type reqMessage;
            public int resMid;
            public System.Type resMessage;

            public System.Object reqMessageInstance;
            public System.Object resMessageInstance;
        }

        Dictionary<int, MidReqInfo> m_vReqMids = new Dictionary<int, MidReqInfo>();
        Dictionary<int, System.Type> m_vResMids = new Dictionary<int, System.Type>();

        Vector2 m_LogMessage = Vector2.zero;
        Vector2 m_Inspector = Vector2.zero;
        MidReqInfo m_ReqSimulateMessage = null;
        List<string> m_vReciveMessages = new List<string>();
        List<string> m_vReqMidPops = new List<string>();

        private ESessionType m_eSessionType = ESessionType.Tcp;
        string m_strHost = "http://192.168.98.157:8083";
        //-----------------------------------------------------
        public void OnEnable()
        {
            m_NetWork = new SimulationNetWork();
            m_NetWork.Start();
            m_NetWork.RegisterSessionCallback(this);
            m_vServerList.Clear();

            CheckMIDs();
        }
        //-----------------------------------------------------
        public void OnDisable()
        {     
            Clear();
        }
        //-----------------------------------------------------
        public void Clear()
        {
            m_vReciveMessages.Clear();
            m_NetWork.Disconnect(ESessionType.Count);
            m_ReqSimulateMessage = null;
        }
        //-----------------------------------------------------
        void CheckMIDs()
        {
            m_ReqSimulateMessage = null;
            m_vReqMids.Clear();
            m_vResMids.Clear();
            m_vReqMidPops.Clear();
            if (MessageBuilder.MessageTypeEnumType == null)
            {
                return;
            }

            System.Type midType = MessageBuilder.MessageTypeEnumType;
            foreach (Enum v in Enum.GetValues(midType))
            {
               string enumName = v.ToString();
                string fullname = "Proto3.";
                bool bReq = false;
                bool bRes = false;
                if (enumName.EndsWith("Req"))
                {
                    bReq = true;
                    fullname += enumName + "uest";
                }
                else if (enumName.EndsWith("Res"))
                {
                    fullname += enumName + "ponse";
                    bRes = true;
                }
                if(bRes || bReq)
                {
                    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "MainScripts")
                        {
                            System.Type type = assembly.GetType(fullname);
                            if (type != null && type.GetInterface("Google.Protobuf.IMessage") != null)
                            {
                                if (bReq)
                                {
                                    MidReqInfo info = new MidReqInfo();
                                    info.reqMid = Convert.ToInt32(v);
                                    info.reqMessage = type;
                                    string resName = enumName.Substring(0, enumName.Length - 3) + "Res";
                                    if (Enum.IsDefined(midType, resName))
                                    {
                                        info.resMid = Convert.ToInt32(Enum.Parse(midType, resName));
                                    }
                                    else info.resMid = 0;
                                    info.resMessage = null;
                                    m_vReqMids[info.reqMid] = info;
                                }
                                else if (bRes)
                                {
                                    int midCode = Convert.ToInt32(v);
                                    m_vResMids[midCode] = type;
                                    m_NetWork.Register(midCode, OnRevicePacketMsg);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            foreach(var db in m_vReqMids)
            {
                if(db.Value.resMid!=0)
                {
                    m_vResMids.TryGetValue(db.Value.resMid, out db.Value.resMessage);
                }
                string key = db.Key.ToString();
                m_vReqMidPops.Add(key.Substring(0, 1).ToUpper() + "/" + key);
            }
        }
        //-----------------------------------------------------
        public void ReqList()
        {
            m_vServers.Clear();
            if (string.IsNullOrEmpty(m_strHost))
                return;
            m_NetWork.ReqHttp(m_strHost, OnServerListBack);
        }
        //-----------------------------------------------------
        public void Connect(string ip, int port)
        {
            m_NetWork.Disconnect(ESessionType.Count);
            m_NetWork.Connect(m_eSessionType,ip, port);
        }
        //-----------------------------------------------------
        public void Update(float fFrameTime)
        {
            if (fFrameTime >= 0.03333) fFrameTime = 0.03333f;
            m_NetWork.LateUpdate(fFrameTime);
        }
        //-----------------------------------------------------
        public void OnEvent(Event evt)
        {

        }
        //-----------------------------------------------------
        public void OnGUI()
        {
        }
        //-----------------------------------------------------
        public void DrawPreview(Rect rc)
        {
            
        }
        //-----------------------------------------------------
        public void OnSceneGUI()
        {
        }
        //-----------------------------------------------------
        void DrawMessagePropery(System.Object pointer, System.Type type)
        {
            Color color = GUI.color;
            GUI.color = Color.red;
            EditorGUILayout.LabelField(type.ToString());
            GUI.color = color;
            PropertyInfo[] props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            if (props == null) return;
            for(int i = 0; i < props.Length; ++i)
            {
                System.Type PropertyType = props[i].PropertyType;
                if (props[i].GetCustomAttribute<global::System.Diagnostics.DebuggerNonUserCodeAttribute>() == null) continue;

                if (PropertyType == typeof(bool))
                {
                    bool val = (bool)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.Toggle(props[i].Name, val));
                }
                else if (PropertyType == typeof(byte))
                {
                    byte val = (byte)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (byte)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (PropertyType == typeof(short))
                {
                    short val = (short)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (short)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (PropertyType == typeof(ushort))
                {
                    ushort val = (ushort)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (ushort)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (PropertyType == typeof(int))
                {
                    int val = (int)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (int)EditorGUILayout.IntField(props[i].Name, (int)val));
                }
                else if (PropertyType == typeof(uint))
                {
                    uint val = (uint)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (uint)EditorGUILayout.IntField(props[i].Name, (int)val));
                }
                else if (PropertyType == typeof(float))
                {
                    float val = (float)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.FloatField(props[i].Name, val));
                }
                else if (PropertyType == typeof(double))
                {
                    double val = (double)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.DoubleField(props[i].Name, val));
                }
                else if (PropertyType == typeof(long))
                {
                    long val = (long)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.LongField(props[i].Name, val));
                }
                else if (PropertyType == typeof(ulong))
                {
                    ulong val = (ulong)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (ulong)EditorGUILayout.LongField(props[i].Name, (long)val));
                }
                else if (PropertyType == typeof(string))
                {
                    string val = (string)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.TextField(props[i].Name, val));
                }
                else if (PropertyType.IsEnum)
                {
                    Enum val = (Enum)Enum.ToObject(PropertyType, props[i].GetValue(pointer));
                    val = Framework.ED.InspectorDrawUtil.PopEnum(props[i].Name, val, PropertyType);
                    props[i].SetValue(pointer,val);
                }
                else if (PropertyType.IsGenericType && PropertyType.FullName.Contains("Google.Protobuf.Collections.RepeatedField"))
                {
                    System.Type[] argvsType = PropertyType.GetGenericArguments();
                    System.Collections.IList val = (System.Collections.IList)props[i].GetValue(pointer);
                    int cnt = EditorGUILayout.IntField(props[i].Name+" Size", val.Count);
                    EditorGUI.indentLevel++;
                    if (cnt!= val.Count)
                    {
                        while(val.Count> cnt)
                        {
                            val.RemoveAt(val.Count-1);
                        }
                        for(int k = val.Count; k < cnt; ++k)
                        {
                            val.Add(System.Activator.CreateInstance(argvsType[0]));
                        }
                    }
                    for (int j = 0; j < val.Count; ++j)
                    {
                        EditorGUILayout.LabelField("Element[" + j + "]");
                        EditorGUI.indentLevel++;
                        DrawProperty(val[j], argvsType[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                    if (props[i].GetSetMethod() == null)
                    {
                        FieldInfo fieldInfo = type.GetField(props[i].Name + "_", BindingFlags.NonPublic| BindingFlags.Instance| BindingFlags.IgnoreCase);
                        if (fieldInfo != null)
                            fieldInfo.SetValue(pointer, val);
                    }
                    else
                        props[i].SetValue(pointer, val);
                }
            }
        }
        //-----------------------------------------------------
        void DrawProperty(System.Object pointer, System.Type type)
        {
            PropertyInfo[] props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            for (int i = 0; i < props.Length; ++i)
            {
                if (props[i].GetCustomAttribute<global::System.Diagnostics.DebuggerNonUserCodeAttribute>() == null) continue;

                if (props[i].PropertyType == typeof(bool))
                {
                    bool val = (bool)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.Toggle(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(byte))
                {
                    byte val = (byte)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (byte)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(short))
                {
                    short val = (short)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (short)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(ushort))
                {
                    ushort val = (ushort)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (ushort)EditorGUILayout.IntField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(int))
                {
                    int val = (int)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (int)EditorGUILayout.IntField(props[i].Name, (int)val));
                }
                else if (props[i].PropertyType == typeof(uint))
                {
                    uint val = (uint)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (uint)EditorGUILayout.IntField(props[i].Name, (int)val));
                }
                else if (props[i].PropertyType == typeof(float))
                {
                    float val = (float)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.FloatField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(double))
                {
                    double val = (double)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.DoubleField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(long))
                {
                    long val = (long)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.LongField(props[i].Name, val));
                }
                else if (props[i].PropertyType == typeof(ulong))
                {
                    ulong val = (ulong)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, (ulong)EditorGUILayout.LongField(props[i].Name, (long)val));
                }
                else if (props[i].PropertyType == typeof(string))
                {
                    string val = (string)props[i].GetValue(pointer);
                    props[i].SetValue(pointer, EditorGUILayout.TextField(props[i].Name, val));
                }
                else if (props[i].PropertyType.IsEnum)
                {
                    Enum val = (Enum)Enum.ToObject(props[i].PropertyType, props[i].GetValue(pointer));
                    val = Framework.ED.InspectorDrawUtil.PopEnum(props[i].Name, val, props[i].PropertyType);
                    props[i].SetValue(pointer, val);
                }
                else if (props[i].PropertyType == typeof(System.Collections.IList))
                {
                    System.Type[] argvsType = props[i].PropertyType.GetGenericArguments();
                    System.Collections.IList val = (System.Collections.IList)props[i].GetValue(pointer);
                    int cnt = EditorGUILayout.IntField(props[i].Name + " Size", val.Count);
                    EditorGUI.indentLevel++;
                    if (cnt != val.Count)
                    {
                        while (val.Count > cnt)
                        {
                            val.RemoveAt(val.Count - 1);
                        }
                        for (int k = val.Count; k < cnt; ++k)
                        {
                            val.Add(System.Activator.CreateInstance(argvsType[0]));
                        }
                    }
                    for (int j = 0; j < val.Count; ++j)
                    {
                        EditorGUILayout.LabelField("Element[" + j + "]");
                        EditorGUI.indentLevel++;
                        DrawProperty(val[j], argvsType[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                    props[i].SetValue(pointer, val);
                }
            }
        }
        //-----------------------------------------------------
        public void OnDrawInspecPanel(Vector2 size)
        {
            m_Inspector = GUILayout.BeginScrollView(m_Inspector);
            if (m_ReqSimulateMessage!=null)
            {
                if(m_ReqSimulateMessage.reqMessageInstance == null)
                    m_ReqSimulateMessage.reqMessageInstance = System.Activator.CreateInstance(m_ReqSimulateMessage.reqMessage);
                EditorGUI.BeginDisabledGroup(m_NetWork.GetSessionState(m_eSessionType) != ESessionState.Connected);
                DrawMessagePropery(m_ReqSimulateMessage.reqMessageInstance,m_ReqSimulateMessage.reqMessage);
                EditorGUI.EndDisabledGroup();
                if(GUILayout.Button("发送请求"))
                {
                    m_NetWork.SendMessage(m_eSessionType, (int)m_ReqSimulateMessage.reqMid, (Google.Protobuf.IMessage)m_ReqSimulateMessage.reqMessageInstance);
                }
                if (m_ReqSimulateMessage.resMessage != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    if (m_ReqSimulateMessage.resMessageInstance == null)
                        m_ReqSimulateMessage.resMessageInstance = System.Activator.CreateInstance(m_ReqSimulateMessage.resMessage);
                    DrawMessagePropery(m_ReqSimulateMessage.resMessageInstance, m_ReqSimulateMessage.resMessage);
                    EditorGUI.EndDisabledGroup();
                }
            }
            GUILayout.EndScrollView();
        }
        //-----------------------------------------------------
        public void OnDrawLayerPanel(Vector2 size)
        {
            EditorGUILayout.BeginHorizontal();
            m_strHost = EditorGUILayout.TextField("服务器", m_strHost);
            if(GUILayout.Button("请求服务器列表"))
            {
                ReqList();
            }
            EditorGUILayout.EndHorizontal();

            m_nServerIndex = EditorGUILayout.Popup("服务器列表", m_nServerIndex, m_vServerList.ToArray());
            EditorGUI.BeginDisabledGroup(m_nServerIndex <0 || m_nServerIndex>= m_NetWork.GetServerList().Count);
            if (GUILayout.Button("连接服务器"))
            {
                m_NetWork.Connect(m_eSessionType, m_NetWork.GetServerList()[m_nServerIndex].ip, int.Parse(m_NetWork.GetServerList()[m_nServerIndex].port));
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(m_NetWork.GetSessionState(m_eSessionType) != ESessionState.Connected);
            int index = -1;
            if(m_ReqSimulateMessage!=null)
            {
                bool bIndex = false;
                int findex = -1;
                foreach (var db in m_vReqMids)
                {
                    findex++;
                    if (m_ReqSimulateMessage == db.Value)
                    {
                        bIndex = true;
                        break;
                    }
                }
                if(bIndex) index = findex;
            }
            index = EditorGUILayout.Popup("请求消息列表", index, m_vReqMidPops.ToArray());
            if(index>=0&& index < m_vReqMids.Count)
            {
                m_ReqSimulateMessage = m_vReqMids.ElementAt(index).Value;
            }
            EditorGUI.EndDisabledGroup();

            //             Rect rect = GUILayoutUtility.GetLastRect();
            // GUILayout.BeginArea(new Rect(0, rect.y, size.x, size.y- rect.y));
            GUILayout.BeginArea(new Rect(0, 101, size.x, size.y - 101));
            GUILayout.Label("日志");
            if (GUILayout.Button("清除"))
            {
                m_vReciveMessages.Clear();
            }
            m_LogMessage = GUILayout.BeginScrollView(m_LogMessage);
            {
                for(int i = 0; i < m_vReciveMessages.Count; ++i)
                    GUILayout.TextField(m_vReciveMessages[i]);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        //-----------------------------------------------------
        public void OnSessionState(AServerSession session, ESessionState eState)
        {

        }
        //-----------------------------------------------------
        void OnRevicePacketMsg(PacketBase msg)
        {
            if (msg.GetMsg() == null) return;
            Debug.Log(msg.GetMsg().ToString());
            if (m_vReciveMessages.Count >= 50)
                m_vReciveMessages.RemoveAt(0);
            m_vReciveMessages.Add(msg.GetMsg().ToString());
            if(m_ReqSimulateMessage!=null)
            {
                if((int)m_ReqSimulateMessage.resMid== msg.GetCode())
                {
                    m_ReqSimulateMessage.resMessageInstance = msg.GetMsg();
                }
            }
        }
        //-----------------------------------------------------
        void OnServerListBack(string error)
        {
            if (error.CompareTo("Fail") == 0) return;
            m_vServerList.Clear();
            for (int i =0; i< m_NetWork.GetServerList().Count; ++i)
            {
                m_vServerList.Add(m_NetWork.GetServerList()[i].name + "-" + m_NetWork.GetServerList()[i].ip + ":" + m_NetWork.GetServerList()[i].port);
            }
        }
    }
}
#endif