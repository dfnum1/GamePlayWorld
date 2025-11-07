/********************************************************************
生成日期:	11:10:2022   13:56
类    名: 	JniPlugin
作    者:	HappLI
描    述:	File jni file ios/android coreplus
*********************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

namespace Framework.Core
{
    public class JniPlugin
    {
        static JniPlugin m_Instance;
        AndroidJavaObject m_JniHelper;
        protected static AndroidJavaObject s_ActivityContext = null;

        public JniPlugin()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (s_ActivityContext == null)
            {
                AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                if (activityClass != null)
                {
                    s_ActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    m_JniHelper = new AndroidJavaObject("com.unity3d.plugin.PluginUnity");
                    if(m_JniHelper!=null)
                    {
                        m_JniHelper.Call("Initialise", s_ActivityContext);
                    }
                    else
                        Debug.Log("JniPlugin-Failed!");
                }
                else
                    Debug.Log("JniPlugin-Failed!");
            }
#endif
            m_Instance = this;
        }
        public static AndroidJavaObject GetCurrentActivityContext()
        {
            return s_ActivityContext;
        }
        public static byte[] ReadFile(string path, bool bAbs, ref int dataSize)
		{
            if (m_Instance == null) return null;
            byte[] bytes = null;
            dataSize = 0;
            string strFullPath = bAbs?path:BaseUtil.stringBuilder.Append(FileSystemUtil.StreamRawPath).Append(path).ToString();
            if (File.Exists(strFullPath))
            {
                FileStream stream = File.Open(strFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataSize = (int)stream.Length;
                bytes = new byte[dataSize];
                stream.Read(bytes, 0, dataSize);
                return bytes;
            }

            bytes = GameDelegate.ReadFile(strFullPath, true, ref dataSize);
            if (dataSize > 0 && bytes!=null)
            {
                return bytes;
            }
            return null;
        }

        public static string ReadFileAllText(string path, bool bAbs)
        {
            string strFullPath = bAbs ? path : BaseUtil.stringBuilder.Append(FileSystemUtil.StreamRawPath).Append(path).ToString();
            if (File.Exists(strFullPath))
            {
                return System.IO.File.ReadAllText(strFullPath);
            }
            if(FileSystemUtil.GetStreamType() == EFileSystemType.EncrptyPak)
            {
                int dataSize = 0;
                byte[] bytes = GameDelegate.ReadFile(strFullPath, true, ref dataSize);
                if (dataSize > 0 && bytes != null)
                    return System.BitConverter.ToString(bytes, 0, dataSize);
                return null;
            }
            else
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                if (m_Instance != null)
                    return m_Instance.m_JniHelper.Call<string>("ReadFileAllText", strFullPath);
#endif
                return null;
            }

        }

        public static byte[] ReadFileAllBytes(string path, ref int dataSize, bool bAbs)
        {
            string strFullPath = bAbs ? path : BaseUtil.stringBuilder.Append(FileSystemUtil.StreamRawPath).Append(path).ToString();
            dataSize = 0;
            byte[] bytes = null;
            if (File.Exists(strFullPath))
            {
                bytes = System.IO.File.ReadAllBytes(strFullPath);
                dataSize = bytes.Length;
                return bytes;
            }
            bytes = GameDelegate.ReadFile(strFullPath, true, ref dataSize);
            if (dataSize > 0 && bytes != null)
                return bytes;
//             if (m_Instance != null)
//                 return m_Instance.m_JniHelper.Call<byte[]>("ReadFile", strFullPath,0,-1);
            return null;
        }

        public static bool ExistFile(string path, bool bAbs)
        {
            string strFullPath = bAbs ? path : BaseUtil.stringBuilder.Append(FileSystemUtil.StreamRawPath).Append(path).ToString();
            return GameDelegate.FileExist(strFullPath, true);
        }

        static string ms_strDevice = null;
        public static string GetUDID()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
			if(m_Instance != null && m_Instance.m_JniHelper != null)
			    ms_strDevice = m_Instance.m_JniHelper.Call<string>("GetUDID");
#elif !UNITY_EDITOR && UNITY_IOS
            ms_strDevice = _GetUUID();
#endif
            if (string.IsNullOrEmpty(ms_strDevice))
                ms_strDevice = SystemInfo.deviceUniqueIdentifier;
            return ms_strDevice;
        }

        public static bool CopyTextToClipboard(string text)
        {
            if (m_Instance == null) return false;
#if !UNITY_EDITOR && UNITY_ANDROID
            if (m_Instance.m_JniHelper == null)
                return false;
            m_Instance.m_JniHelper.Call("CopyTextToClipboard", "Label", text);
#elif !UNITY_EDITOR && UNITY_IOS
            _copyTextToClipboard(text);
#elif UNITY_EDITOR || UNITY_STANDALONE
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
#endif
            return true;
        }

        public static string GetNetowrkState()
        {
            if (m_Instance == null) return null;
#if !UNITY_EDITOR && UNITY_ANDROID
            if (m_Instance.m_JniHelper == null)
                return null;
            return m_Instance.m_JniHelper.Call<string>("GetNetowrkState");
#elif !UNITY_EDITOR && UNITY_IOS
            return _GetCurrentNetworkState();
#elif UNITY_EDITOR || UNITY_STANDALONE
            return null;
#else
            return null;
#endif
        }

        public static long GetRemainMemory()
        {
            if (m_Instance == null) return -1;
#if !UNITY_EDITOR && UNITY_ANDROID
            if (m_Instance.m_JniHelper == null)
                return -1;
            return m_Instance.m_JniHelper.Call<long>("GetRemainMemory");
#elif !UNITY_EDITOR && UNITY_IOS
            return _GetRemainMemory();
#elif UNITY_EDITOR
            return SystemInfo.systemMemorySize;
#else
            return -1;
#endif
        }

        public static bool ShowAlertDialog(string title, string content, string sureBtnName, string cancelBtnName, GameObject pListener)
        {
            if (m_Instance == null) return false;
#if !UNITY_EDITOR && UNITY_ANDROID
            if (m_Instance.m_JniHelper == null)
                return false;
            m_Instance.m_JniHelper.Call("ShowAlertDialog", title, content, sureBtnName, cancelBtnName, pListener?pListener.name:"");
            return true;
#elif !UNITY_EDITOR && UNITY_IOS
            iOSNativeAlert.ShowDialog(title,content, pListener, sureBtnName, cancelBtnName);
            return true;
#else
            return false;
#endif
        }

        public static void GetIPType(string serverIp, string serverPorts, out string newServerIp, out System.Net.Sockets.AddressFamily mIPType)
        {
#if !UNITY_EDITOR && UNITY_IOS
            mIPType = System.Net.Sockets.AddressFamily.InterNetwork;
            newServerIp = serverIp;
            try
            {
                string mIPv6 = getIPv6(serverIp, serverPorts);
                if (!string.IsNullOrEmpty(mIPv6))
                {
                    string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                    if (m_StrTemp != null && m_StrTemp.Length >= 2)
                    {
                        string IPType = m_StrTemp[1];
                        if (IPType == "ipv6")
                        {
                            newServerIp = m_StrTemp[0];
                            mIPType = System.Net.Sockets.AddressFamily.InterNetworkV6;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("GetIPv6 error:" + e);
            }
#else
            newServerIp = serverIp;
            mIPType = System.Net.Sockets.AddressFamily.InterNetwork;
#endif
        }
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _copyTextToClipboard(string text);

        [DllImport("__Internal")]
        private static extern string _GetCurrentNetworkState();
        
        [DllImport("__Internal")]
        private static extern long _GetRemainMemory();

        [DllImport("__Internal")]
        private static extern string getIPv6(string mHost, string mPort);

        [DllImport("__Internal")]
        private static extern string _GetUUID();

        public static class iOSNativeAlert
        {
            public delegate void AlertDelegate(string str);
            [DllImport("__Internal")]

            private static extern void _showDialog(
                string title,
                string msg,
                string actionFirst,
                string actionSecond,
                string actionThird,
                AlertDelegate onCompletion);

            static GameObject pListener;

            [AOT.MonoPInvokeCallback(typeof(AlertDelegate))]
            private static void OnCompletionCallback(string str)
            {
                if (pListener != null) pListener.SendMessage("OnAlertDlgCallback", str);
                pListener = null;
            }

            public static void ShowDialog(
                string title,
                string msg,
                GameObject onCompletion,
                string actionFirst = "Ok",
                string actionSecond = null,
                string actionThird = null)
            {
                pListener = onCompletion;
                _showDialog(title, msg, actionFirst, actionSecond, actionThird, OnCompletionCallback);
            }
        }
#elif UNITY_EDITOR && !UNITY_EDITOR_OSX
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            //系统内存总量
            public ulong dwTotalPhys;
            //系统可用内存
            public ulong dwAvailPhys;
            public ulong dwTotalPageFile;
            public ulong dwAvailPageFile;
            public ulong dwTotalVirtual;
            public ulong dwAvailVirtual;
        }
        [DllImport("kernel32")]
        static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
        static MEMORY_INFO GetMemoryStatus()
        {
            MEMORY_INFO MemInfo = new MEMORY_INFO();
            GlobalMemoryStatus(ref MemInfo);
            return MemInfo;
        }
#endif
    }
}