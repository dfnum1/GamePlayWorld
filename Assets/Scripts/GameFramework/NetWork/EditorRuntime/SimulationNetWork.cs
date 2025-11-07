#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	MessageSimulationEditorLogic
作    者:	HappLI
描    述:	消息模拟逻辑
*********************************************************************/
using Framework.Net;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.ED
{
    public class SimulationNetWork : ANetWork, ISessionCallback
    {
        class ReqData : System.IComparable<ReqData>
        {
            public float reqSleep;
            public byte repeatTimes;
            public System.Action<string> onCallback;
            public string url;

            public UnityEngine.Networking.UnityWebRequest request;

            public int CompareTo(ReqData other)
            {
                return url.CompareTo(other.url);
            }
        }
        List<ReqData> m_vCallback = new List<ReqData>();

        [System.Serializable]
        public struct SvrItem
        {
            public int id;
            public string name;
            public string ip;
            public string port;
        }
        class Server
        {
            public List<SvrItem> list = new List<SvrItem>();
        }
        List<SvrItem> m_ServerList = new List<SvrItem>();

        System.Reflection.MethodInfo m_getMessageCodeMethod = null;
        System.Reflection.MethodInfo m_newBuilderMethod = null;
        System.Reflection.MethodInfo m_newMessageByCodeMethod = null;
        //------------------------------------------------------
        protected override void OnInit()
        {
            base.OnInit();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                bool bFind = false;
                Type[] types = assembly.GetTypes();
                foreach(var tp in types)
                {
                    if(tp.IsDefined(typeof(PacketBuilderCallAttribute),false))
                    {
                        var methods = tp.GetMethods();
                        for(int i =0; i < methods.Length; ++i)
                        {
                            if(m_getMessageCodeMethod == null)
                            {
                                if (methods[i].ReturnType == typeof(int))
                                {
                                    var methodParams = methods[i].GetParameters();
                                    if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(System.Type))
                                    {
                                        m_getMessageCodeMethod = methods[i];
                                    }
                                }
                            }

                            if (methods[i].ReturnType == typeof(Google.Protobuf.IMessage))
                            {
                                var methodParams = methods[i].GetParameters();
                                if (m_newMessageByCodeMethod == null)
                                {
                                    if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(int))
                                    {
                                        m_newMessageByCodeMethod = methods[i];
                                    }
                                }
                                if (m_newBuilderMethod == null)
                                {
                                    if (methodParams.Length == 4 && 
                                        methodParams[0].ParameterType == typeof(int) &&
                                        methodParams[1].ParameterType == typeof(byte[]) &&
                                        methodParams[2].ParameterType == typeof(int) &&
                                        methodParams[3].ParameterType == typeof(int))
                                    {
                                        m_newBuilderMethod = methods[i];
                                    }
                                }
                            }

                            if (m_newBuilderMethod != null && m_newMessageByCodeMethod != null && m_getMessageCodeMethod != null)
                                break;
                        }
                        bFind = true;
                        break;
                    }
                }
                if (bFind) break;
            }
        }
        //------------------------------------------------------
        public List<SvrItem> GetServerList()
        {
            return m_ServerList;
        }
        //------------------------------------------------------
        void OnServerList(string strValue)
        {
            if(strValue.CompareTo("Fail") == 0)
            {
                EditorUtility.DisplayDialog("提示", "请求服务器列表失败", "Ok");
            }
            try
            {
                string[] arr = strValue.Split('|');

                if (arr.Length >= 1)
                {
                    strValue = "{ \"list\": " + arr[0] + "}";
                    Server studentList = JsonUtility.FromJson<Server>(strValue);
                    m_ServerList = studentList.list;
                }
            }
            catch
            {
                EditorUtility.DisplayDialog("提示", "解析失败", "Ok");
            }

        }
        //------------------------------------------------------
        public override int GetNetProtoMessageCode(Type type)
        {
            if (m_getMessageCodeMethod == null) return 0;
            return (int)m_getMessageCodeMethod.Invoke(null, new object[] { type });
        }
        //------------------------------------------------------
        public override IMessage CreateMessageByCode(int code)
        {
            if (m_newMessageByCodeMethod == null) return null;
            var msg = m_newMessageByCodeMethod.Invoke(null, new object[] { code });
            if (msg == null || msg.GetType().GetInterface(typeof(IMessage).FullName) == null)
                return null;
            return msg as IMessage;
        }
        //------------------------------------------------------
        public override IMessage CreateMessageByCode(int code, byte[] pDatas, int nLenth, int offset = 0)
        {
            if (m_newBuilderMethod == null) return null;
            var msg = m_newBuilderMethod.Invoke(null, new object[] { code, pDatas, offset, nLenth });
            if (msg == null || msg.GetType().GetInterface(typeof(IMessage).FullName) == null)
                return null;
            return msg as IMessage;
        }
    }
}
#endif