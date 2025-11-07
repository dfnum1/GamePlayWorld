/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Package
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Core
{
    public class Package : System.IComparable<Package>
    {
        [System.Serializable]
        public struct UpdateItem
        {
            [System.NonSerialized]
            public string remotePath;
            public string abName;
            public string md5;
            public long size;
            public string[] depends;
        }

        [System.Serializable]
        public struct UpdateData : IUserData
        {
            public long totalSize;
            public string rootDir;
            public string version;
            public UpdateItem[] datas;
            public UpdateItem[] rawDatas;
            public static int CompareVersion(UpdateData lhs, UpdateData rhs)
            {
                long ver1 = 0;
                long ver2 = 0;
                long.TryParse(lhs.version, out ver1);
                long.TryParse(rhs.version, out ver2);
                return ver1.CompareTo(ver2);
            }
            public bool IsExist(string abName)
            {
                if (datas == null) return false;
                for (int i = 0; i < datas.Length; ++i)
                {
                    if (datas[i].abName.CompareTo(abName) == 0) return true;
                }
                return false;
            }

            public bool IsRawExist(string rawName)
            {
                if (rawDatas == null) return false;
                for (int i = 0; i < datas.Length; ++i)
                {
                    if (rawDatas[i].abName.CompareTo(rawName) == 0) return true;
                }
                return false;
            }
#if UNITY_EDITOR
            public void RemoveItem(string abName)
            {
                if (datas == null) return;
                bool bDirty = false;
                List<UpdateItem> items = new List<UpdateItem>(datas);
                for (int i = 0; i < items.Count;)
                {
                    if (items[i].abName.CompareTo(abName) == 0)
                    {
                        items.RemoveAt(i);
                        bDirty = true;
                    }
                    else
                        ++i;
                }
                if (bDirty)
                {
                    datas = items.ToArray();
                }
            }
            public void RemoveRawItem(string rawName)
            {
                if (datas == null) return;
                bool bDirty = false;
                List<UpdateItem> items = new List<UpdateItem>(rawDatas);
                for (int i = 0; i < items.Count;)
                {
                    if (items[i].abName.CompareTo(rawName) == 0)
                    {
                        items.RemoveAt(i);
                        bDirty = true;
                    }
                    else
                        ++i;
                }
                if (bDirty)
                {
                    rawDatas = items.ToArray();
                }
            }
#endif
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }
        //------------------------------------------------------
        [System.Serializable]
        public struct UpdateFiles
        {
            public string version;
            public List<UpdateData> datas;
            public AbMapping mapping;
            public void Sort()
            {
                if (datas == null || datas.Count <= 1) return;
                Base.SortUtility.QuickSort<UpdateData>(ref datas, UpdateData.CompareVersion);
            }
        }
        //------------------------------------------------------
        [System.Serializable]
        public struct PakUpdateFiles
        {
            [System.Serializable]
            public struct PakData
            {
                public string remotePath;
                public string md5;
                public long size;
            }
            public string version;
            public int versionCode;
            public List<PakData> datas;
        }
        //------------------------------------------------------
        protected AFileSystem m_pFileSystem;
        public class EnterData
        {
            public string strABPath = null;
            public string md5= null;
            public string[] dependencies = null;

            public bool bBase = true;
            public bool isValid()
            {
                return !string.IsNullOrEmpty(strABPath);
            }
        }
        protected string m_strVersion = null;
        protected Dictionary<string, EnterData> m_vFiles = null;
        protected PackageStream m_pStream = null;
        public Package(AFileSystem fileSystem)
        {
            m_pFileSystem = fileSystem;
            m_strVersion = null;
            m_vFiles = null;
        }
        //------------------------------------------------------
        public IEnumerator CoroutineInit(string strFile, string strPkgName, string strVersion = null, System.Action<bool> callback = null)
        {
            m_strVersion = strVersion;
            if (string.IsNullOrEmpty(strFile) || string.IsNullOrEmpty(strPkgName))
            {
                if (callback != null) callback(false);
                yield break;
            }
            if (FileSystemUtil.GetStreamType() == EFileSystemType.EncrptyPak)
            {
                if (m_pStream == null)
                    m_pStream = new PackageStream(m_pFileSystem, strPkgName);
            }
#if !USE_SERVER
            yield return FileSystemUtil.CoroutineLoadAssetBunlde(m_pFileSystem, strPkgName,(ab)=> {
                bool bResult = InitedPackge(ab, strFile, strPkgName, strVersion);
                if (callback != null) callback(bResult);
            }, m_pStream);
#else
             if (callback != null) callback(true);
            yield break;
#endif
        }
        //------------------------------------------------------
        public bool Init(string strFile, string strPkgName, string strVersion = null)
        {
            m_strVersion = strVersion;
            if (string.IsNullOrEmpty(strFile) || string.IsNullOrEmpty(strPkgName))
            {
                return false;
            }
            if (FileSystemUtil.GetStreamType() == EFileSystemType.EncrptyPak)
            {
                if (m_pStream == null)
                    m_pStream = new PackageStream(m_pFileSystem, strPkgName);
            }
#if !USE_SERVER
            AssetBundle assetData = FileSystemUtil.LoadAssetBunlde(m_pFileSystem, strPkgName, m_pStream);
            return InitedPackge(assetData, strFile, strPkgName, strVersion);
#else
            return m_vFiles.Count > 0;
#endif
        }
        //------------------------------------------------------
        bool InitedPackge( AssetBundle assetData, string strFile, string strPkgName, string strVersion = null)
        {
#if !USE_SERVER
            if (assetData == null)
            {
                Debug.LogError("package[" + strPkgName + "]  load failed!!");
                return false;
            }
            AssetBundleManifest manifest = assetData.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest != null)
            {
                if (strFile[strFile.Length - 1] != '/') strFile += '/';
                string[] names = manifest.GetAllAssetBundles();
                if (m_vFiles == null)
                    m_vFiles = new Dictionary<string, EnterData>(names.Length);
                m_vFiles.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    EnterData enter = new EnterData();
                    enter.strABPath = BaseUtil.stringBuilder.Append(strFile).Append(names[i]).ToString();
                    enter.md5 = manifest.GetAssetBundleHash(names[i]).ToString();
                    enter.dependencies = manifest.GetDirectDependencies(names[i]);
                    enter.bBase = true;
                    m_vFiles.Add(names[i], enter);
                    //         Framework.Plugin.Logger.Info(enter.strABPath);
                }
            }
            else
                Debug.LogError("package: " + strFile + "  load manifest failed!!");

            assetData.Unload(true);
            UnityEngine.Debug.Log("inited package[" + strPkgName + "]!");
#endif
            return m_vFiles.Count > 0;
        }
        //------------------------------------------------------
        public string GetVersion()
        {
            return m_strVersion;
        }
        //------------------------------------------------------
        public bool RefreshLocalUpdate(string packagePath, bool bInitPackage = true)
        {
#if !USE_SERVER
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return true;
            string txt = FileSystemUtil.ReadFileIfExist(m_pFileSystem.GetLocalUpdateFile());
            if (!string.IsNullOrEmpty(txt))
            {
                try
                {
                    Debug.Log("RefreshLocalUpdate-Begin");
                    AbMapping abMapping = null;
                    string strCode = txt;
                    if (m_pFileSystem.eType == EFileSystemType.EncrptyPak)
                    {
                        PakUpdateFiles updataData = JsonUtility.FromJson<PakUpdateFiles>(strCode);
                        Debug.Log("RefreshLocalUpdate Pak Cnt:" + updataData.datas.Count);
                        string strPakFile;
                        for (int i = 0; i < updataData.datas.Count; ++i)
                        {
                            strPakFile = BaseUtil.stringBuilder.Append(packagePath).Append(updataData.datas[i].remotePath).ToString();
                            if (File.Exists(strPakFile))
                            {
                                Debug.Log("LoadPackage:" + strPakFile);
                                m_pFileSystem.LoadPackage(strPakFile);
                            }
                            else
                                Debug.Log("Package:" + strPakFile + "   not exits");
                        }
                        Debug.Log("All Update Package Loaded");
                        if (bInitPackage)
                        {
                            int dataSize = 0;
                            byte[] buffDatas = m_pFileSystem.ReadFile("mapping.txt", true, ref dataSize);
                            if (dataSize > 0)
                            {
                                m_pFileSystem.MergeMappingAB(JsonUtility.FromJson<AbMapping>(System.Text.Encoding.UTF8.GetString(buffDatas, 0, dataSize)), true);
                            }

                            Init(m_pFileSystem.StreamPackagesPath, BaseUtil.stringBuilder.Append(m_pFileSystem.StreamPackagesPath).Append("base_pkg").ToString(), FileSystemUtil.PlublishVersion);
                        }
                    }
                    else
                    {
                        UpdateFiles updataData = JsonUtility.FromJson<UpdateFiles>(strCode);
                        if (updataData.datas != null)
                        {

                            long curVersion = FileSystemUtil.ConverVersion(m_strVersion);
                            updataData.Sort();
                            for (int i = 0; i < updataData.datas.Count; ++i)
                            {
                                if (updataData.datas[i].datas == null) continue;
                                if (FileSystemUtil.ConverVersion(updataData.datas[i].version) < curVersion)
                                    continue;
                                for (int j = 0; j < updataData.datas[i].datas.Length; ++j)
                                {
                                    UpdateItem item = updataData.datas[i].datas[j];
                                    EnterData pData = FileSystemUtil.FindEnterData(item.abName);
                                    string filePath = BaseUtil.stringBuilder.Append(FileSystemUtil.UpdateDataPath).Append(item.abName).ToString();
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        FileSystemUtil.AddFile(item.abName, filePath, item.md5, item.depends);
                                    }
                                }
                            }
                        }
                        abMapping = updataData.mapping;
                    }
                    if (abMapping != null)
                    {
                        m_pFileSystem.MergeMappingAB(abMapping);
                    }
                    Debug.Log("RefreshLocalUpdate-End");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("update files parse error");
                    Debug.LogException(ex);
                    return false;
                }
            }
#endif
            return true;
        }
        //------------------------------------------------------
        public bool GetAbNameAndPath(string strAssetFile, bool bAbName, out string abName, out string abPath, out string[] depends)
        {
            if (bAbName)
            {
                abName = strAssetFile;
                EnterData outEnter;
                if (m_vFiles.TryGetValue(abName, out outEnter))
                {
                    abPath = outEnter.strABPath;
                    depends = outEnter.dependencies;
                    return true;
                }
                else
                {
                    abPath = null;
                    abName = null;
                    depends = null;
                    return false;
                }
            }
            else
            {
                if (m_pFileSystem == null)
                {
                    abPath = null;
                    abName = null;
                    depends = null;
                    return false;
                }
                EnterData outEnter;
                abName = m_pFileSystem.GetFileAbName(strAssetFile);
                if(string.IsNullOrEmpty(abName))
                {
                    abName = strAssetFile.Replace("\\", "/").ToLower(); //gc 416b + 0.5kb
                    int index = abName.LastIndexOf('/');
                    if (index >= 0)
                    {
                        abName = abName.Substring(0, index);//gc 366b
                        if (string.IsNullOrEmpty(abName))
                        {
                            abPath = null;
                            abName = null;
                            depends = null;
                            return false;
                        }
                    }
                    if (m_vFiles.TryGetValue(abName, out outEnter))
                    {
                        abPath = outEnter.strABPath;
                        depends = outEnter.dependencies;
                        return true;
                    }
                    abName = BaseUtil.stringBuilder.Append(abName).Append(".pkg").ToString(); //gc 406b
                    if (m_vFiles.TryGetValue(abName, out outEnter))
                    {
                        abPath = outEnter.strABPath;
                        depends = outEnter.dependencies;
                        return true;
                    }
                }
                if(string.IsNullOrEmpty(abName))
                {
                    abPath = null;
                    abName = null;
                    depends = null;
                    return false;
                }
                if (m_vFiles.TryGetValue(abName, out outEnter))
                {
                    abPath = outEnter.strABPath;
                    depends = outEnter.dependencies;
                    return true;
                }
                abName = null;
            }
            abPath = null;
            abName = null;
            depends = null;
            return false;
        }
        //------------------------------------------------------
        public EnterData FinData(string abName)
        {
            EnterData outEnter;
            if (m_vFiles.TryGetValue(abName, out outEnter))
            {
                return outEnter;
            }
            return null;
        }
        //------------------------------------------------------
        public EnterData AddFile(string strAbName, string strAbPath, string md5, string[] dependAbs)
        {
            if (string.IsNullOrEmpty(strAbName) || string.IsNullOrEmpty(strAbPath)) return null;
            EnterData enter = null;
            if(!m_vFiles.TryGetValue(strAbName, out enter))
            {
                enter = new EnterData();
                m_vFiles.Add(strAbName, enter);
            }
            enter.strABPath = strAbPath;
            enter.md5 = md5;
            enter.dependencies = dependAbs;
            enter.bBase = false;
            return enter;
        }
        //------------------------------------------------------
        public string[] GetDependencies(string strAssetFile, bool bAbName=true)
        {
            if (bAbName)
            {
                EnterData outEnter;
                if (m_vFiles.TryGetValue(strAssetFile, out outEnter))
                {
                    return outEnter.dependencies;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string abName = strAssetFile.Replace("\\", "/").ToLower();
                abName = abName.Substring(0, abName.LastIndexOf('/'));
                EnterData outEnter;
                if (m_vFiles.TryGetValue(abName, out outEnter))
                {
                    return outEnter.dependencies;
                }
                abName = BaseUtil.stringBuilder.Append(abName).Append(".pkg").ToString();
                if (m_vFiles.TryGetValue(abName, out outEnter))
                {
                    return outEnter.dependencies;
                }
            }
            return null;
        }
        //------------------------------------------------------
        public int CompareTo(Package other)
        {
            string ver1 = m_strVersion;
            string ver2 = other.m_strVersion;
            if (string.IsNullOrEmpty(ver1))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(ver2))
            {
                return -1;
            }
            long v1 = FileSystemUtil.ConverVersion(ver1);
            long v2 = FileSystemUtil.ConverVersion(ver2);
            if (v1 < v2) return -1;
            if (v1 > v2) return 1;
            return 0;
        }
    }
}

