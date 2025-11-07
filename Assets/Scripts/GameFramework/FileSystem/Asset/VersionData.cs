/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	VersionData
作    者:	HappLI
描    述:	版本数据
*********************************************************************/

using UnityEngine;

namespace Framework.Core
{
    public class VersionData
    {
        //------------------------------------------------------
        [System.Serializable]
        public struct Server
        {
            public string name;
            public string url;
        }
        //------------------------------------------------------
        [System.Serializable]
        public struct Config
        {
            public string version;
            public short packCount;
            public string assetbundleEncryptKey;
            public short defaultLanguage;
            public bool sceneStreamAB;
            public bool offline;
            public bool popPrivacy;
            public string channel;
            public Server[] serverUrls;

            public bool Parse(byte[] bytes, int size)
            {
                if (bytes == null) return false;
                try
                {
                    GameDelegate.DecodeBuffer(ref bytes, size, 1);
                    Data.BinaryUtil util = new Data.BinaryUtil();
                    util.Set(bytes, size);
                    int magic = 'v' | ('e' << 8) | ('r' << 16) | ('0' << 24);
                    if (util.ToInt32() != magic)
                        return false;
                    util.ToByte();//version
                    version = util.ToString();
                    packCount = util.ToShort();
                    assetbundleEncryptKey = util.ToString();
                    defaultLanguage = util.ToShort();
                    sceneStreamAB = util.ToBool();
                    offline = util.ToBool();
                    popPrivacy = util.ToBool();
                    channel = util.ToString();
                    int svrCnt = (int)util.ToByte();
                    if (svrCnt <= 0) return false;
                    serverUrls = new Server[svrCnt];
                    for (int i = 0; i < svrCnt; ++i)
                    {
                        Server item = new Server();
                        item.url = util.ToString();
                        item.name = util.ToString();
                        serverUrls[i] = item;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log("version data parser error:" + ex.ToString());
                    return false;
                }
               
                return true;
            }
#if UNITY_EDITOR
            public void Save(string strFile)
            {
                Data.BinaryUtil util = new Data.BinaryUtil();
                util.WriteInt32('v' | ('e' << 8) | ('r' << 16) | ('0' << 24));
                util.WriteByte(0);
                util.WriteString(version);
                util.WriteShort(packCount);
                util.WriteString(assetbundleEncryptKey);
                util.WriteShort(defaultLanguage);
                util.WriteBool(sceneStreamAB);
                util.WriteBool(offline);
                util.WriteBool(popPrivacy);
                util.WriteString(channel);
                if (serverUrls !=null)
                {
                    util.WriteByte((byte)serverUrls.Length);
                    for (int i = 0; i < serverUrls.Length; ++i)
                    {
                        util.WriteString(serverUrls[i].url);
                        util.WriteString(serverUrls[i].name);
                    }
                }
                else
                {
                    util.WriteByte(0);
                }
                byte[] bytes =  util.GetBuffer();
                GameDelegate.EncryptBuffer(ref bytes, util.GetCur(), 1);
                util.SaveTo(strFile);
            }
#endif
        }
        //------------------------------------------------------
        public static bool sceneStreamAB = false;
        public static string version = "1.0.0";
        public static short base_pack_cnt = 0;
        public static Server[] serverUrls = null;
        public static string serverVersion = "1.0.0";
        public static string assetbundleEncryptKey = null;
        public static short defaultLanguage = -1;
        public static bool offline = true;
        public static bool popPrivacy = false;
        public static string channel = "";
        //------------------------------------------------------
        public static string GetServerUrl()
        {
            if (serverUrls == null || serverUrls.Length <=0) return null;
            for(int i =0; i < serverUrls.Length; ++i)
            {
                if (!string.IsNullOrEmpty(serverUrls[i].url)) return serverUrls[i].url;
            }
            return null;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        static bool ParserVersion(string strContext)
        {
            try
            {
                Config config = JsonUtility.FromJson<Config>(strContext);
                SetConfig(config);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("版本文件解析失败:" + ex.StackTrace);
                return false;
            }
        }
#endif
        //------------------------------------------------------
        static void SetConfig(Config config)
        {
            VersionData.sceneStreamAB = config.sceneStreamAB;
            VersionData.offline = config.offline;
            VersionData.popPrivacy = config.popPrivacy;
            VersionData.version = config.version;
            VersionData.serverUrls = config.serverUrls;
            VersionData.base_pack_cnt = config.packCount;
            VersionData.defaultLanguage = config.defaultLanguage;
            VersionData.channel = config.channel;
            if (string.IsNullOrEmpty(VersionData.channel)) VersionData.channel = "";
            if (string.IsNullOrEmpty(config.assetbundleEncryptKey))
                VersionData.assetbundleEncryptKey = null;
            else
                VersionData.assetbundleEncryptKey = config.assetbundleEncryptKey;
        }
        //------------------------------------------------------
        public static bool Parser(EFileSystemType streamType, string strFile)
        {
#if UNITY_EDITOR
            if (streamType > EFileSystemType.AssetData)
            {
                string versionFile = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + "/base_pkg/version.ver";
                if (System.IO.File.Exists(versionFile))
                {
                    if (ParserVersion(System.IO.File.ReadAllText(versionFile)))
                        return true;
                }
            }
#endif
            TextAsset pVer = Resources.Load<TextAsset>("version");
            if (pVer != null)
            {
                Config cfg = new Config();
                if (!cfg.Parse(pVer.bytes, pVer.bytes.Length))
                    return false;
                SetConfig(cfg);
                return true;
            }
//             else
//             {
//                 int dataSize = 0;
//                 byte[] buffDatas = GameDelegate.ReadFile("version.txt", true, ref dataSize);
//                 if(dataSize>0)
//                 {
//                     Config cfg = new Config();/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                     if (!cfg.Parse(buffDatas, dataSize))
//                         return false;
//                     SetConfig(cfg);
//                     return true;
//                 }
//             }
            return false;
        }
    }
}
