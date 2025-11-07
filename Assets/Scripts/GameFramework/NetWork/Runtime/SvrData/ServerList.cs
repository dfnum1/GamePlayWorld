/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	ServerList
作    者:	HappLI
描    述:   服务器列表
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using Framework.Core;

namespace Framework.Net
{
    public enum EServerType
    {
        Login = 1,
    }
    public class ServerList
    {
        public delegate void OnServerListEvent(string error, IUserData userParam);

        [System.Serializable]
        public struct Item
        {
            public int id;
            public int type;
            public string name;
            public string ip;
            public string port;
        }

        [System.Serializable]
        private class Server
        {
            public bool canHotUp = false;
            public string hotAddress;
            public string uploadAddress;
            public string matchAddress;
            public string loginNotice;
            public string[] loginNoticeArray;

            public List<Item> servers = null;
            public void Clear()
            {
                canHotUp = false;
                hotAddress = null;
                uploadAddress = null;
                if(servers != null) servers.Clear();
            }
        }

        private static bool ms_bCanHotUp = false;
        private static string ms_strHotAddress = null;
        private static string ms_strUploadAddress = null;

        private static string ms_strLoginListAddress = null;
        private static string ms_strLoginAddress = null;
        private static string ms_strHttpMatchAddress = null;
        private static string ms_strMatchAddress = null;
        private static string ms_strLoginNotice = null;
        private static string[] ms_strLoginNoticeArray = null;
        private static Dictionary<EServerType, List<Item>> ms_vServers = new Dictionary<EServerType, List<Item>>();

        private static OnServerListEvent m_ServerListCallback = null;
        private static IUserData m_CallbackVariable = null;

        static void Clear()
        {
            ms_bCanHotUp = false;
            ms_strHotAddress = null;
            ms_strUploadAddress = null;
            ms_strLoginAddress = null;
            ms_strLoginListAddress = null;
            ms_strMatchAddress = null;
            ms_strHttpMatchAddress = null;
            ms_strLoginNotice = null;
            ms_vServers.Clear();
        }

        public static void ReqList(OnServerListEvent callback = null, IUserData userParam = null)
        {
            m_CallbackVariable = userParam;
            m_ServerListCallback = callback;
            Clear();
            if (VersionData.offline)
            {
                string serverUrl = VersionData.GetServerUrl();// version check url
                ms_strHotAddress = serverUrl;
                OnServerList("offline");
            }
            else
            {
                string serverUrl = VersionData.GetServerUrl();
                ms_strLoginAddress = serverUrl;
                Framework.Base.Logger.Info("serverUrl:" + serverUrl);
                if (string.IsNullOrEmpty(serverUrl))
                {
                    //ms_strLoginAddress = "http://192.168.6.142:35530/"; // 本地
                    ms_strLoginAddress = "http://192.168.100.210:35520/"; // 内网 
                    //  ms_strLoginAddress = "http://192.144.198.50:35520/"; // 外网 
                }
                if (!ms_strLoginAddress.EndsWith("/")) ms_strLoginAddress += "/";
                ms_strLoginListAddress = ms_strLoginAddress + "login/server_list";
                ms_strHttpMatchAddress = ms_strLoginAddress + "match/server";
                Net.WebHandler.ReqHttp(ms_strLoginListAddress, SystemInfo.deviceUniqueIdentifier, OnServerList);
            }
        }
        //------------------------------------------------------
        public static void RefreshServerList(OnServerListEvent callback = null, IUserData userParam = null)
        {
            if (string.IsNullOrEmpty(ms_strLoginListAddress))
                return;
            m_ServerListCallback = callback;
            Net.WebHandler.ReqHttp(ms_strLoginListAddress, OnServerList);
        }
        //------------------------------------------------------
        static void OnServerList(string strValue)
        {
            if (strValue.CompareTo("Fail") == 0)
            {
                if (m_ServerListCallback != null)
                    m_ServerListCallback(strValue, m_CallbackVariable);
                m_ServerListCallback = null;
                m_CallbackVariable = null;
                return;
            }
            try
            {
                if(!string.IsNullOrEmpty(strValue))
                {
                    Server server = JsonUtility.FromJson<Server>(strValue);
                    ms_bCanHotUp = server.canHotUp;
                    ms_strUploadAddress = server.uploadAddress;
                    ms_strHotAddress = server.hotAddress;
                    ms_strLoginNotice = server.loginNotice;
                    ms_strLoginNoticeArray = server.loginNoticeArray;
                    //ms_strLoginNoticeArray = new string[] {"标题1","内容1", "标题2", "内容2", "标题3", "内容3" };
                    ms_strMatchAddress = null;
                    if (IsValidIPPort(server.matchAddress))
                        ms_strMatchAddress = server.matchAddress;
                    for (int i = 0; i < server.servers.Count; ++i)
                    {
                        List<Item> items;
                        if (ms_vServers.TryGetValue((EServerType)server.servers[i].type, out items))
                        {
                            items.Clear();
                        }
                    }
                    for (int i =0; i < server.servers.Count; ++i)
                    {
                        List<Item> items;
                        if (ms_vServers.TryGetValue((EServerType)server.servers[i].type, out items))
                        {
                            items.Add(server.servers[i]);
                        }
                        else
                        {
                            items = new List<Item>();
                            items.Add(server.servers[i]);
                            ms_vServers.Add((EServerType)server.servers[i].type, items);
                        }
                    }
                }
            }
            catch
            {

            }
            
            if (m_ServerListCallback != null)
                m_ServerListCallback(strValue, m_CallbackVariable);
            m_ServerListCallback = null;
            m_CallbackVariable = null;
        }
        //------------------------------------------------------
        public static List<Item> GetServers(EServerType serverType)
        {
            List<Item> vItems;
            if (ms_vServers.TryGetValue(serverType, out vItems)) return vItems;
            return null;
        }
        //------------------------------------------------------
        public static string GetLoginAddress()
        {
            return ms_strLoginAddress;
        }
        //------------------------------------------------------
        public static void ReqMatchServer(System.Action<string> onCallback)
        {
            if (string.IsNullOrEmpty(ms_strHttpMatchAddress))
            {
                if (onCallback != null) onCallback(null);
                return;
            }
            Net.WebHandler.ReqHttp(ms_strHttpMatchAddress, (string address)=> {
                if (string.IsNullOrEmpty(address) || string.Compare(address,"Fail") ==0 || !IsValidIPPort(address))
                {
                    if (onCallback != null) onCallback(null);
                    return;
                }
                else
                {
                    ms_strMatchAddress = address;
                    if (onCallback != null) onCallback(ms_strMatchAddress);
                }
            });
        }
        //------------------------------------------------------
        public static string GetHotAddress()
        {
            return ms_strHotAddress;
        }
        //------------------------------------------------------
        public static bool GetVersionIsUpdate()
        {
            return ms_bCanHotUp;
        }
        //------------------------------------------------------
        public static string GetLoginNotice()
        {
            return ms_strLoginNotice;
        }
        //------------------------------------------------------
        public static string[] GetLoginNoticeArray()
        {
            return ms_strLoginNoticeArray;
        }
        //------------------------------------------------------
        private static bool IsValidIPPort(string address)
        {
            if (string.IsNullOrEmpty(address)) return false;
            int ipPort = address.LastIndexOf(":");
            if (ipPort <= 0)
            {
                return false;
            }
            int port = 0;
            if (!int.TryParse(address.Substring(ipPort + 1, address.Length - ipPort - 1), out port))
            {
                return false;
            }
            return true;
        }
    }
}

