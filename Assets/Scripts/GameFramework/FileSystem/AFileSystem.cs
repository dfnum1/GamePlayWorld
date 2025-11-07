/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	FileSystem
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
namespace Framework.Core
{
    public enum EFileSystemType
    {
        AssetData = 0,
        AssetBundle,
        EncrptyPak,
    }
    //------------------------------------------------------
    public interface IFileSystemCallBack
    {
        void OnAssetCallback(Asset asset);
        void OnAssetGrab(Asset asset);
        void OnAssetRelease(Asset asset);
        void OnAssetDispose(Asset asset);
    };
    //------------------------------------------------------
    [System.Serializable]
    public class AbMapping
    {
        [System.Serializable]
        public struct FileAbName
        {
            public string file;
            public string abName;
        }
        public FileAbName[] mapping;
    }
    //------------------------------------------------------
    public abstract class AFileSystem : AModule, IUserData
    {
        protected string m_strAssetPath = "";
        protected string m_strStreamPath = "";
        protected string m_strStreamPackagesPath = "";
        protected string m_strStreamRawPath = "";
        protected string m_strStreamBinaryPath = "";
        protected string m_strPersistentDataPath = "";
        protected string m_strUpdateDataPath = "";
        protected string m_strLocalUpdateFile = "";
        protected string m_strLocalUpdateVersionFile = "";
        protected string m_strVersion = "1.0.0";
        protected string m_strPublishVersion = "1.0.0";
        protected string m_strPackageSuffix = "pak";
        protected bool m_bEnableDebug = false;
        protected bool m_bStreamSceneAB = false;
        public string AssetPath { get { return m_strAssetPath; } }
        public string StreamPath { get { return m_strStreamPath; } }
        public string StreamRawPath { get { return m_strStreamRawPath; } }
        public string StreamBinaryPath { get { return m_strStreamBinaryPath; } }
        public string StreamPackagesPath { get { return m_strStreamPackagesPath; } }
        public string PersistenDataPath { get { return m_strPersistentDataPath; } }
        public string UpdateDataPath { get { return m_strUpdateDataPath; } }
        public string LocalUpdateFile { get { return m_strLocalUpdateFile; } }
        public string LocalVersionFile { get { return m_strLocalUpdateVersionFile; } }
        public string Version { get { return m_strVersion; } }
        public string PlublishVersion { get { return m_strPublishVersion; } }

        protected EFileSystemType m_eType = EFileSystemType.AssetData;
        public EFileSystemType eType { get { return m_eType; } }

        protected Dictionary<string, Asset>   m_vResources;
        protected Dictionary<string, Asset>   m_vPermanentResources;
        protected LinkedList<Asset>                 m_vReleasePool;
        protected LinkedList<Asset>                 m_vCheckingPool;

        protected LinkedList<AssetOperiaon> m_vReqAssetOps = null;
        protected LinkedList<AssetOperiaon> m_vAsyncReqAssetOps = null;

        protected uint m_nSreamReadBufferSize = 512 * 1024;
        protected int m_nDealCreateBundleMaxCnt = 100;
        protected LinkedList<AssetBundleInfo> m_vReqCreateBundleQueue = null;
        protected LinkedList<AssetBundleInfo> m_vReleaseAssetBundlePool;
        protected HashSet<string> m_vCheckABDeeps = null;
        protected Dictionary<string, AssetBundleInfo> m_vAssetBundles = null;
        protected Dictionary<string, string> m_vFileABMapping = null;
        protected LinkedList<AssetBundleInfo> m_vUnloadAssetBunds = null;
        protected LinkedList<AssetOperiaon> m_vCustomFileOps = null;

        protected private float m_fWatingSecond = 0;
        protected private long m_lOneFrameCost = 300;
        protected InstancePools m_pInstancePool = null;

        protected bool m_bEnableCoroutines = true;
        private bool m_bCoroutining = false;
#if UNITY_EDITOR
        private IEnumerator m_editorCoroutine;
#endif

        protected int m_nPackageCnt = 0;
        //  List<Package> m_vPackages = null;
        protected Package m_pPackages = null;

        protected List<IFileSystemCallBack> m_vCallback = null;
        IGame m_pGameMain = null;
#if UNITY_EDITOR
        private List<string> m_vSearchPaths = new List<string>(8);
        private Dictionary<string, List<string>> m_vOtherDynamicPaths = new Dictionary<string, List<string>>(16);
        public static bool bEditorMode = false;
        public Dictionary<string, AssetBundleInfo> assetbunds
        {
            get { return m_vAssetBundles; }
        }
        public Dictionary<string, Asset> resources
        {
            get { return m_vResources; }
        }
        public Dictionary<string, Asset> permanentResources
        {
            get { return m_vPermanentResources; }
        }
        Dictionary<string, string> m_vCheckFilePathLowerUpper = new Dictionary<string, string>(16);
        public bool IsEditorMode()
        {
            return bEditorMode;
        }
        public void SetEditorMode(bool bEditor)
        {
            bEditorMode = bEditor;
        }
#endif
        //------------------------------------------------------
        protected override void OnInit()
        {
            m_pGameMain = GetFramework().gameStartup;
#if UNITY_EDITOR
            m_vCheckFilePathLowerUpper.Clear();
            m_vSearchPaths.Clear();
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath = Application.streamingAssetsPath;
            m_strStreamRawPath = Application.streamingAssetsPath + "/raws/";
            m_strStreamBinaryPath = Application.dataPath + "/../Binarys/";
#elif UNITY_STANDALONE
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath = Application.streamingAssetsPath + "/packages/";
            m_strStreamRawPath = Application.streamingAssetsPath + "/packages/raws/";
            m_strStreamBinaryPath = m_strStreamRawPath + "Binarys/";
#elif UNITY_ANDROID
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath = Application.streamingAssetsPath + "/packages/";
            m_strStreamRawPath = "packages/raws/";
            m_strStreamBinaryPath = m_strStreamRawPath + "Binarys/";
#elif UNITY_IOS
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath =  Application.streamingAssetsPath + "/packages/";
            m_strStreamRawPath = Application.streamingAssetsPath + "/packages/raws/";
            m_strStreamBinaryPath = m_strStreamRawPath + "Binarys/";
#elif UNITY_WEBGL
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath =  Application.streamingAssetsPath + "/packages/";
            m_strStreamRawPath = Application.streamingAssetsPath + "/packages/raws/";
            m_strStreamBinaryPath = m_strStreamRawPath + "Binarys/";
#else
            m_strStreamPath = Application.streamingAssetsPath;
            m_strStreamPackagesPath = Application.streamingAssetsPath + "/packages/";
            m_strStreamRawPath = Application.streamingAssetsPath + "/raws/";
            m_strStreamBinaryPath = Application.dataPath + "/../Binarys/";
#endif
#if UNITY_EDITOR
            bEditorMode = false;
#endif
            m_strAssetPath = Application.dataPath;
            m_strPersistentDataPath = Application.persistentDataPath + "/";
#if UNITY_EDITOR
            m_strPersistentDataPath = Application.dataPath + "/../Local/";
#endif
            m_strUpdateDataPath = BaseUtil.stringBuilder.Append(m_strPersistentDataPath).Append("updates/").ToString();
            m_strLocalUpdateVersionFile = BaseUtil.stringBuilder.Append(m_strPersistentDataPath).Append("localversion.txt").ToString();
            m_strLocalUpdateFile = BaseUtil.stringBuilder.Append(m_strUpdateDataPath).Append("updates.json").ToString();

            
            m_vResources = new Dictionary<string, Asset>(512);
            m_vPermanentResources = new Dictionary<string, Asset>(512);
            m_vReleasePool = new LinkedList<Asset>();
            m_vCheckingPool = new LinkedList<Asset>();

            m_vReqAssetOps = new LinkedList<AssetOperiaon>();
            m_vAsyncReqAssetOps = new LinkedList<AssetOperiaon>();
            m_vCustomFileOps = new LinkedList<AssetOperiaon>();

            m_vCheckABDeeps = null;
            //  m_vPackages = new List<Package>(4);
            m_vReqCreateBundleQueue = new LinkedList<AssetBundleInfo>();
            m_vReleaseAssetBundlePool = new LinkedList<AssetBundleInfo>();
            m_vAssetBundles = new Dictionary<string, AssetBundleInfo>(512);

            m_vCallback = new List<IFileSystemCallBack>(2);


            m_nSreamReadBufferSize = 32 * 1024;
            if (m_eType == EFileSystemType.EncrptyPak)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                m_strStreamPackagesPath = "packages/";
                m_strStreamRawPath = "raws/";
                m_strStreamBinaryPath = "raws/Binarys/";
#elif UNITY_IOS
                m_strStreamRawPath = "raws/";
                m_strStreamBinaryPath = "raws/Binarys/";
#elif UNITY_WEBGL && !UNITY_EDITOR
                m_strStreamRawPath = "raws/";
                m_strStreamBinaryPath = "raws/Binarys/";
#elif UNITY_EDITOR
                m_strStreamPackagesPath = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + "/defualt/encrpty_packages/";
#endif
            }

            AssetOperiaon.OnProcessCB = OnProcessOp;

            OnPreBuild();
#if UNITY_EDITOR
            if (!Application.isPlaying) m_bEnableCoroutines = false;
#endif
        }
        //------------------------------------------------------
        protected abstract void OnPreBuild();
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_pInstancePool = new InstancePools(m_pFramework, m_bEnableCoroutines);
            OnInnerAwake();
        }
        //------------------------------------------------------
        protected abstract void OnInnerAwake();
        //------------------------------------------------------
        public EFileSystemType GetStreamType()
        {
            return m_eType;
        }
        //------------------------------------------------------
        public string GetAssetPath() { return AssetPath; }
        //------------------------------------------------------
        public string GetStreamPath() { return StreamPath; }
        //------------------------------------------------------
        public string GetStreamRawPath() { return StreamRawPath; }
        //------------------------------------------------------
        public string GetStreamBinaryPath() { return StreamBinaryPath; }
        //------------------------------------------------------
        public string GetPersistenDataPath() { return PersistenDataPath; }
        //------------------------------------------------------
        public string GetUpdateDataPath() { return UpdateDataPath; }
        //------------------------------------------------------
        public string GetLocalUpdateFile() { return LocalUpdateFile; }
        //------------------------------------------------------
        public string GetLocalVersionFile() { return LocalVersionFile; }
        //------------------------------------------------------
        public string GetVersion() { return Version; }
        //------------------------------------------------------
        public string GetPlublishVersion() { return PlublishVersion; }
        //------------------------------------------------------
        public bool IsStreamSceneAB() { return m_bStreamSceneAB; }
        //------------------------------------------------------
        public void Build(EFileSystemType eType, string basePkgName="base_pkg", string srrVersion = "1.0.0", short base_pack_cnt = 0, string strEncryptKey=null, bool bStreamSceneAB=false)
        {
            m_bEnableDebug = false;
            m_bStreamSceneAB = bStreamSceneAB;
#if !UNITY_EDITOR
            if (base_pack_cnt>0 && eType > EFileSystemType.AssetData)
            {
                eType = EFileSystemType.EncrptyPak;
            }
#endif
            m_strPublishVersion = srrVersion;
            m_strVersion = srrVersion;
            m_eType = eType;
            if(m_vCallback!=null) m_vCallback.Clear();

            m_nPackageCnt = base_pack_cnt;

            Debug.Log("FileSystemType:" + eType.ToString());
#if !USE_SERVER
            Debug.Log("platform:" + Application.platform);
#endif
            //! load local packages
            if (m_eType == EFileSystemType.AssetBundle)
            {
#if UNITY_2019_4_2
                AssetBundle.SetAssetBundleDecryptKey(strEncryptKey);
#endif
                string pkgPath = m_strStreamPackagesPath;
#if UNITY_EDITOR
                AssetBundle.UnloadAllAssetBundles(true);
              //  m_strStreamPackagesPath = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget +"/" + srrVersion + "/base_pkg/";
             //   if (!Directory.Exists(m_strStreamPackagesPath))
            //    {
                    m_strStreamPackagesPath = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + "/default/base_pkg/";
             //   }
                pkgPath = m_strStreamPackagesPath;
#endif
                if (System.IO.File.Exists(LocalVersionFile))
                {
                    m_strVersion = System.IO.File.ReadAllText(LocalVersionFile).Trim();
                }
                if (string.IsNullOrEmpty(m_strVersion))
                    m_strVersion = srrVersion;

                if(!Directory.Exists(m_strUpdateDataPath))
                    Directory.CreateDirectory(m_strUpdateDataPath);
            }
            else if (m_eType == EFileSystemType.EncrptyPak)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                m_strStreamPackagesPath = "packages/";
                m_strStreamRawPath = "raws/";
                m_strStreamBinaryPath = "raws/Binarys/";
#elif UNITY_IOS
                m_strStreamRawPath = "raws/";
                m_strStreamBinaryPath = "raws/Binarys/";     
#endif

#if UNITY_EDITOR
            //    m_strStreamPackagesPath = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + "/" + srrVersion + "/encrpty_packages/";
             //   if (!Directory.Exists(m_strStreamPackagesPath))
             //   {
                    m_strStreamPackagesPath = Application.dataPath + "/../Publishs/Packages/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + "/default/encrpty_packages/";
             //   }
#endif
                if (System.IO.File.Exists(LocalVersionFile))
                {
                    m_strVersion = System.IO.File.ReadAllText(LocalVersionFile).Trim();
                }
                if (string.IsNullOrEmpty(m_strVersion))
                    m_strVersion = srrVersion;

             //   base_pack_cnt = 47;
            //    m_nPackageCnt = base_pack_cnt;
                if (!Directory.Exists(m_strUpdateDataPath))
                    Directory.CreateDirectory(m_strUpdateDataPath);
            }
        }
        //------------------------------------------------------
        public void EnableCoroutines(bool bCoroutines)
        {
#if !USE_SERVER
            m_bEnableCoroutines = bCoroutines;
            if (m_pInstancePool != null)
                m_pInstancePool.EnableCoroutines(bCoroutines);
                
            CheckStartCoroutinesLoad();
#endif
        }
        //------------------------------------------------------
        public IEnumerator CoroutineInitPackages(System.Action<bool> callback)
        {
            if (m_eType < EFileSystemType.AssetBundle)
            {
                if (callback != null) callback(true);
                 yield break;
            }

            m_pPackages = new Package(this);
            if (m_eType == EFileSystemType.EncrptyPak)
            {
                EnableCatchHandle(true);
                DeleteAllPackages();
                string strFile;
                for (int i = 1; i <= m_nPackageCnt; ++i)
                {
                    //strFile = Framework.Core.BaseUtil.stringBuilder.Append(m_strUpdateDataPath).Append("package_").Append(i).Append(".").Append(m_strPackageSuffix).ToString();
                    //if (!File.Exists(strFile))
                    //{
                    strFile = BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("package_").Append(i).Append(".").Append(m_strPackageSuffix).ToString();
                    //}
                    LoadPackage(strFile);
                }
                RefreshLocalUpdate(false);
            }

            m_vFileABMapping = null;
            try
            {
                if (m_eType == EFileSystemType.EncrptyPak)
                {
                    int dataSize = 0;
                    byte[] buffDatas = ReadFile("mapping.txt", true, ref dataSize);
                    if (dataSize > 0)
                    {
                        MergeMappingAB(JsonUtility.FromJson<AbMapping>(System.Text.Encoding.UTF8.GetString(buffDatas, 0, dataSize)), true);
                    }
                }
                else
                {
                    TextAsset mappingAsset = Resources.Load<TextAsset>("mapping");
                    if (mappingAsset)
                    {
                        MergeMappingAB(JsonUtility.FromJson<AbMapping>(mappingAsset.text), true);
                        Resources.UnloadAsset(mappingAsset);
                    }
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        Debug.Log("plaftorm is web, not support load local localmapping file");
                    }
                    else
                    {
                        string localMapping = BaseUtil.stringBuilder.Append(m_strPersistentDataPath).Append("localmapping.txt").ToString();
#if UNITY_EDITOR
                    localMapping = BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("mapping.txt").ToString();
#endif
                        var mappingTxt = FileSystemUtil.ReadFileIfExist(localMapping);
                        if (!string.IsNullOrEmpty(mappingTxt))
                        {
                            MergeMappingAB(JsonUtility.FromJson<AbMapping>(mappingTxt), true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            // base_pkg
            if (m_eType == EFileSystemType.EncrptyPak)
            {
                yield return m_pPackages.CoroutineInit(m_strStreamPackagesPath, "base_pkg", m_strPublishVersion,(result)=> {
                    if (!result)
                    {
                        m_pPackages = null;
                        Debug.LogError("包解析失败!");
                    }
                    if (callback != null) callback(result);
                });
            }
            else
            {
                yield return m_pPackages.CoroutineInit(m_strStreamPackagesPath, BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("base_pkg").ToString(), m_strPublishVersion,(result)=> {
                    if(!result)
                    {
                        m_pPackages = null;
                        Debug.LogError("包解析失败!");
                    }
                    if (callback != null) callback(result);
                });
            }
        }
        //------------------------------------------------------
        public void InitPackages()
        {
            if (m_eType < EFileSystemType.AssetBundle)
            {
                return;
            }

            m_pPackages = new Package(this);
            if (m_eType == EFileSystemType.EncrptyPak)
            {
                EnableCatchHandle(true);
                DeleteAllPackages();
                string strFile;
                for (int i = 1; i <= m_nPackageCnt; ++i)
                {
                    //strFile = Framework.Core.BaseUtil.stringBuilder.Append(m_strUpdateDataPath).Append("package_").Append(i).Append(".").Append(m_strPackageSuffix).ToString();
                    //if (!File.Exists(strFile))
                    //{
                    strFile = BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("package_").Append(i).Append(".").Append(m_strPackageSuffix).ToString();
                    //}
                    LoadPackage(strFile);
                }
                RefreshLocalUpdate(false);
            }

            m_vFileABMapping = null;
            try
            {
                if (m_eType == EFileSystemType.EncrptyPak)
                {
                    int dataSize = 0;
                    byte[] buffDatas = ReadFile("mapping.txt", true, ref dataSize);
                    if (dataSize > 0)
                    {
                        MergeMappingAB(JsonUtility.FromJson<AbMapping>(System.Text.Encoding.UTF8.GetString(buffDatas, 0, dataSize)), true);
                    }
                }
                else
                {
                    TextAsset mappingAsset = Resources.Load<TextAsset>("mapping");
                    if (mappingAsset)
                    {
                        MergeMappingAB(JsonUtility.FromJson<AbMapping>(mappingAsset.text), true);
                        Resources.UnloadAsset(mappingAsset);
                    }
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        Debug.Log("plaftorm is web, not support load local localmapping file");
                    }
                    else
                    {
                        string localMapping = BaseUtil.stringBuilder.Append(m_strPersistentDataPath).Append("localmapping.txt").ToString();
#if UNITY_EDITOR
                    localMapping = BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("mapping.txt").ToString();
#endif
                        var mappingTxt = FileSystemUtil.ReadFileIfExist(localMapping);
                        if (!string.IsNullOrEmpty(mappingTxt))
                        {
                            MergeMappingAB(JsonUtility.FromJson<AbMapping>(mappingTxt), true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            // base_pkg
            if (m_eType == EFileSystemType.EncrptyPak)
            {
                if (!m_pPackages.Init(m_strStreamPackagesPath, "base_pkg", m_strPublishVersion))
                {
                    m_pPackages = null;
                    Debug.LogError("包解析失败!");
                }
            }
            else
            {
                if (!m_pPackages.Init(m_strStreamPackagesPath, BaseUtil.stringBuilder.Append(m_strStreamPackagesPath).Append("base_pkg").ToString(), m_strPublishVersion))
                {
                    m_pPackages = null;
                    Debug.LogError("包解析失败!");
                }
            }
        }
        //------------------------------------------------------
        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            if(m_pGameMain != null)
            {
                return m_pGameMain.BeginCoroutine(coroutine);
            }
            else
            {
                return GetFramework().BeginCoroutine(coroutine);
            }
        }
        //------------------------------------------------------
        public void MergeMappingAB(AbMapping abMapping, bool bClear = false)
        {
            if (abMapping != null && abMapping.mapping != null && abMapping.mapping.Length > 0)
            {
                if(m_vFileABMapping == null) m_vFileABMapping = new Dictionary<string, string>(abMapping.mapping.Length);
                if (bClear) m_vFileABMapping.Clear();
                for (int i = 0; i < abMapping.mapping.Length; ++i)
                {
                    if (string.IsNullOrEmpty(abMapping.mapping[i].file) || string.IsNullOrEmpty(abMapping.mapping[i].abName)) continue;
                    m_vFileABMapping[abMapping.mapping[i].file] = abMapping.mapping[i].abName;
                }
            }
        }
        //------------------------------------------------------
        public void SetCapability(int oneFrameCost = 300, int maxInstanceCnt = 30, int destroyDelayTime = 60)
        {
            m_lOneFrameCost = oneFrameCost;
            if (m_pInstancePool == null) return;
            m_pInstancePool.SetCapability(oneFrameCost, maxInstanceCnt, destroyDelayTime);
        }
        //------------------------------------------------------
        public void SetInstanceDelayDestroyParam(int destroyDelayTime)
        {
            if (m_pInstancePool != null) m_pInstancePool.SetDelayDestroyParam(destroyDelayTime);
        }
        //------------------------------------------------------
        public void SetSreamReadBufferSize(uint sreamReadBufferSize)
        {
            m_nSreamReadBufferSize = sreamReadBufferSize;
        }
        //------------------------------------------------------
        public uint GetSreamReadBufferSize()
        {
            return m_nSreamReadBufferSize;
        }
        //------------------------------------------------------
        public void EnableDebug(bool bDebug)
        {
            m_bEnableDebug = bDebug;
        }
        //------------------------------------------------------
        public bool isEnableDebug()
        {
            return m_bEnableDebug;
        }
        //------------------------------------------------------
        public void RegisterCallback(IFileSystemCallBack callback)
        {
            if (m_vCallback == null) m_vCallback = new List<IFileSystemCallBack>();
            if (m_vCallback.Contains(callback)) return;
            m_vCallback.Add(callback);
        }
        //------------------------------------------------------
        public void UnRegisterCallback(IFileSystemCallBack callback)
        {
            if (m_vCallback == null) return;
            m_vCallback.Remove(callback);
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_fWatingSecond = 0;
            if(m_vReqAssetOps!=null)
            {
                var node = m_vReqAssetOps.First;
                while (node != null)
                {
                    var next = node.Next;
                    node.Value.Clear();
                    AssetOperiaon.Free(node.Value);
                    node = next;
                }
                m_vReqAssetOps.Clear();
            }

            if(m_vAsyncReqAssetOps!=null)
            {
                var node = m_vAsyncReqAssetOps.First;
                while (node != null)
                {
                    var next = node.Next;
                    node.Value.Clear();
                    AssetOperiaon.Free(node.Value);
                    node = next;
                }
                m_vAsyncReqAssetOps.Clear();
            }

            if(m_pInstancePool!=null) m_pInstancePool.Clear();

            Free(0);
        }
        //------------------------------------------------------
        public int GetCurLoadingCount()
        {
            if(m_pInstancePool!=null)
                return m_vCheckingPool.Count + m_pInstancePool.GetReqCount();
            return m_vCheckingPool.Count;
        }
        //------------------------------------------------------
        public void Free(float fDelta = 0)
        {
            if(m_pInstancePool!=null) m_pInstancePool.Free(fDelta);
            //! release asset
            if(m_vResources!=null)
            {
                foreach (var db in m_vResources)
                {
                    if (db.Value.RefCnt > 0)
                    {
                        continue;
                    }
                    if (m_vCheckingPool.Contains(db.Value)) continue;
                    m_vReleasePool.Remove(db.Value);
                    db.Value.Release(fDelta);
                }
            }
        }
        //------------------------------------------------------
        public void ClearReqing(bool bIncludeInstance = true)
        {
            m_vReqAssetOps.Clear();
            m_vAsyncReqAssetOps.Clear();
            if(bIncludeInstance && m_pInstancePool!=null) m_pInstancePool.ClearReqing();
        }
        //------------------------------------------------------
        void Shutdown()
        {
            //! release asset
            for (var node = m_vReleasePool.First; node != null; node = node.Next)
            {
                node.Value.CheckDispose(true);
            }
            m_vReqAssetOps.Clear();
            m_vAsyncReqAssetOps.Clear();
            m_vReleasePool.Clear();
            m_vReleaseAssetBundlePool.Clear();
            m_vResources.Clear();
            if(m_pInstancePool!=null) m_pInstancePool.Shutdown();

            if(m_vCallback!=null) m_vCallback.Clear();
            OnShutdown();
        }
        //-------------------------------------------------
        protected abstract void OnShutdown();
        //-------------------------------------------------
        public int StatsInstanceCount(string strFile)
        {
            if (m_pInstancePool == null) return 0;
            return m_pInstancePool.StatsInstanceCount(strFile);
        }
        //-------------------------------------------------
        public int StatsInstanceCount(int guid)
        {
            if (m_pInstancePool == null) return 0;
            return m_pInstancePool.StatsInstanceCount(guid);
        }
        //-------------------------------------------------
        public void AddReleaseAsset(Asset pAsset)
        {
            m_vReleasePool.AddLast(pAsset);
        }
#if !USE_SERVER
        //------------------------------------------------------
        public void ReleaseAsyncRequest(AssetBundleInfo assetAb)
        {
            if (assetAb == null || assetAb.asyncRequest == null) return;
            m_vReleaseAssetBundlePool.AddLast(assetAb);
        }
        //------------------------------------------------------
        public void ReqCreateBundle(AssetBundleInfo ReqAB, bool bImmediately=false)
        {
            ReqAB.bReqing = true;
            if (bImmediately)
            {
                if (ReqAB.bAsync || Application.platform == RuntimePlatform.WebGLPlayer)
                    FileSystemUtil.AsyncLoadAssetBundle(this,ReqAB);
                else
                    FileSystemUtil.LoadAssetBundle(this,ReqAB);
                return;
            }
            m_vReqCreateBundleQueue.AddFirst(ReqAB);
            CheckStartCoroutinesLoad();
        }
        //------------------------------------------------------
        public AssetOperiaon AsyncReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            string strRawFile = BuildAssetFilePath(strFile);
#if UNITY_EDITOR
            CheckDynamicAble(strRawFile);
            string areadyPath;
            if (m_vCheckFilePathLowerUpper.TryGetValue(strRawFile.ToLower(), out areadyPath))
            {
                if (areadyPath.CompareTo(strRawFile) != 0)
                {
                    UnityEngine.Debug.LogError(strRawFile + "\r\n" + areadyPath + "\r\n资源路径存在大小写问题!,请及时修复");
                    UnityEngine.Debug.Break();
                }
            }
            else
                m_vCheckFilePathLowerUpper[strRawFile.ToLower()] = strRawFile;
#endif
            AssetOperiaon op = AssetOperiaon.Malloc();
            op.OnCallback = onCallback;
#if UNITY_EDITOR
            op.strRawFile = strRawFile;
#endif
            op.strFile = strFile;
            m_vAsyncReqAssetOps.AddFirst(op);
            CheckStartCoroutinesLoad();
            return op;
        }
        //------------------------------------------------------
        public AssetOperiaon ReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            string strRawFile = BuildAssetFilePath(strFile);
#if UNITY_EDITOR
            CheckDynamicAble(strRawFile);
            string areadyPath;
            if (m_vCheckFilePathLowerUpper.TryGetValue(strRawFile.ToLower(), out areadyPath))
            {
                if (areadyPath.CompareTo(strRawFile) != 0)
                {
                    UnityEngine.Debug.LogError(strRawFile + "\r\n" + areadyPath + "\r\n资源路径存在大小写问题!,请及时修复");
                    UnityEngine.Debug.Break();
                }
            }
            else
                m_vCheckFilePathLowerUpper[strRawFile.ToLower()] = strRawFile;
#endif

            AssetOperiaon op = AssetOperiaon.Malloc();
#if UNITY_EDITOR
            op.strRawFile = strRawFile;
#endif
            op.strFile = strFile;
            op.OnCallback = onCallback;
            m_vReqAssetOps.AddFirst(op);
            CheckStartCoroutinesLoad();
            return op;
        }
        //------------------------------------------------------
        public AssetOperiaon LoadCustomFile(string strFile, System.Action<AssetOperiaon> onCallback = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            string strRawFile = BuildAssetFilePath(strFile);
            for (var node = m_vCustomFileOps.First; node != null; node = node.Next)
            {
                if (node.Value.strFile == strFile)
                {
                    if (onCallback != null) node.Value.OnCallback += onCallback;
                    return node.Value;
                }
            }

            AssetOperiaon op = AssetOperiaon.Malloc();
            op.OnCallback = onCallback;
#if UNITY_EDITOR
            op.strRawFile = strRawFile;
#endif
            op.strFile = strFile;
            op.bScene = false;
            m_vCustomFileOps.AddLast(op);
            CheckStartCoroutinesLoad();
            return op;
        }
        //------------------------------------------------------
        public bool IsInLoadQueue(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return false;
            for (var node = m_vAsyncReqAssetOps.First; node != null; node = node.Next)
            {
                if (strFile.CompareTo(node.Value.strFile) == 0)
                    return true;
            }
            for (var node = m_vReqAssetOps.First; node != null; node = node.Next)
            {
                if (strFile.CompareTo(node.Value.strFile) == 0)
                    return true;
            }
            for (var node = m_vCheckingPool.First; node != null; node = node.Next)
            {
                if (strFile.CompareTo(node.Value.Path) == 0)
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public AssetOperiaon LoadScene(string strFile, string strSceneName, ELoadSceneMode mode = ELoadSceneMode.Single, bool bAysnc = false)
        {
            if (string.IsNullOrEmpty(strSceneName)) return null;
            string strRawFile = BuildAssetFilePath(strFile);
            AssetOperiaon op = AssetOperiaon.Malloc();
            op.strFile = strFile;
#if UNITY_EDITOR
            op.strRawFile = strRawFile;
#endif
            op.bScene = true;
            op.loadMode = mode;
            op.sceneName = strSceneName;
            if (bAysnc) m_vAsyncReqAssetOps.AddFirst(op);
            else m_vReqAssetOps.AddFirst(op);
            CheckStartCoroutinesLoad();
            return op;
        }
        //------------------------------------------------------
        public void ClearAllPreSpawn(float fDelay)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.ClearAllPreSpawn(fDelay);
        }
        //------------------------------------------------------
        public int GetPreSpawnStats(string strFile)
        {
            if (m_pInstancePool == null) return 0;
            return m_pInstancePool.GetPreSpawnStats(strFile);
        }
        //------------------------------------------------------
        public int GetPreSpawnStats(GameObject pPrefab)
        {
            if (m_pInstancePool == null) return 0;
            return m_pInstancePool.GetPreSpawnStats(pPrefab);
        }
        //------------------------------------------------------
        public void PreSpawnInstance(string strFile, bool bAsync = true, bool bFrontQueue = true)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.PreSpawn(strFile, bAsync, bFrontQueue);
        }
        //------------------------------------------------------
        public void PreSpawnInstance(GameObject pPrefab, bool bFrontQueue = true)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.PreSpawn(pPrefab, bFrontQueue);
        }
        //------------------------------------------------------
        public void PreDeSpawnInstance(string strFile, int cnt =1)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.PreDeSpawnInstance(strFile, cnt);
        }
        //------------------------------------------------------
        public void PreDeSpawnInstance(GameObject pPrefab, int cnt = 1)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.PreDeSpawnInstance(pPrefab, cnt);
        }
        //------------------------------------------------------
        public InstanceOperiaon SpawnInstance(string strFile, bool bAsync = true)
        {
            if (m_pInstancePool == null) return null;
            return m_pInstancePool.Spawn(strFile, bAsync);
        }
        //------------------------------------------------------
        public bool SpawnInstance(InstanceOperiaon pOp)
        {
            if (m_pInstancePool == null) return false;
            return m_pInstancePool.Spawn(pOp);
        }
        //------------------------------------------------------
        public InstanceOperiaon SpawnInstance(GameObject pAsset, bool bAsync = false)
        {
            if (pAsset == null || m_pInstancePool == null) return null;
            return m_pInstancePool.Spawn(pAsset, bAsync);
        }
        //------------------------------------------------------
        public void DeSpawnInstance(AInstanceAble pAble, int nCheckMax=2)
        {
            if (m_pInstancePool == null) return;
            m_pInstancePool.DeSpawn(pAble, nCheckMax);
        }
        //------------------------------------------------------
        private void CheckStartCoroutinesLoad()
        {
            if (!m_bEnableCoroutines) return;
            if (m_bCoroutining) return;
            if(m_vReqCreateBundleQueue.Count > 0 || m_vReqAssetOps.Count > 0 || m_vAsyncReqAssetOps.Count > 0 || m_vCheckingPool.Count>0 || m_vCustomFileOps.Count>0)
            {
                m_bCoroutining = true;
#if UNITY_EDITOR
                if (!Application.isPlaying || !AFramework.isStartup)
                {
                    m_editorCoroutine = CoroutinesUpdateLoad();
                    EditorApplication.update += EditorCoroutineUpdate;
                    return;
                }
#endif
                StartCoroutine(CoroutinesUpdateLoad());
            }
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        private void EditorCoroutineUpdate()
        {
            if (m_editorCoroutine == null || !m_editorCoroutine.MoveNext())
            {
                EditorApplication.update -= EditorCoroutineUpdate;
                m_bCoroutining = false;
                m_editorCoroutine = null;
            }
        }
#endif
        //------------------------------------------------------
        private IEnumerator CoroutinesUpdateLoad()
        {
            m_bCoroutining = true;
            yield return CoroutineSystem.waitForEndOfFrame;
            int DoCnt = 0;
            while (m_vReqCreateBundleQueue.Count > 0)
            {
                AssetBundleInfo reqInfo = m_vReqCreateBundleQueue.First.Value;
                m_vReqCreateBundleQueue.RemoveFirst();
                if (reqInfo.bAsync || Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    FileSystemUtil.AsyncLoadAssetBundle(this,reqInfo);
                    yield return reqInfo.asyncRequest;
                }
                else
                {
                    FileSystemUtil.LoadAssetBundle(this,reqInfo);
                }
                DoCnt++;
                if(DoCnt >= m_nDealCreateBundleMaxCnt)
                {
                    break;
                }
            }
            ProcessReq(ref m_vReqAssetOps, false, m_nDealCreateBundleMaxCnt);
            ProcessReq(ref m_vAsyncReqAssetOps, true, m_nDealCreateBundleMaxCnt);
           
            CheckLodingPool(true, DateTime.Now);

            if (m_vCustomFileOps.Count > 0)
            {
                var reqInfo = m_vCustomFileOps.First.Value;
                m_vCustomFileOps.RemoveFirst();
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    var request = UnityWebRequest.Get(reqInfo.strFile);
                    yield return request.SendWebRequest();
                    if (reqInfo.OnCallback != null)
                    {
                        byte[] buffers = request.downloadHandler.data;
                        reqInfo.pBufferAsset = new VariableBuffer(buffers, buffers != null ? buffers.Length : 0);
                        reqInfo.OnCallback(reqInfo);
                    }
                }
                else
                {
                    int dataSize = 0;
                    byte[] dataBuffer = ReadFile(reqInfo.strFile, true, ref dataSize);
                    if (reqInfo.OnCallback != null)
                    {
                        reqInfo.pBufferAsset = new VariableBuffer(dataBuffer, dataSize);
                        reqInfo.OnCallback(reqInfo);
                    }
                }
                AssetOperiaon.Free(reqInfo);
            }
            m_bCoroutining = false;
        }
#endif
        //------------------------------------------------------
        public void InnerAssetGrab(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return;
            Asset assetRef;
            if (m_vResources.TryGetValue(strFile, out assetRef))
            {
                assetRef.Grab();
            }
        }
        //------------------------------------------------------
        public void InnerAssetRelease(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return;
            Asset assetRef;
            if(m_vResources.TryGetValue(strFile, out assetRef))
            {
                assetRef.Release();
            }
        }
        //------------------------------------------------------
        public AssetOperiaon BuildAssetOp(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            AssetOperiaon op = AssetOperiaon.Malloc();
            op.strFile = strFile;
#if UNITY_EDITOR
            op.strRawFile = BuildAssetFilePath(strFile);
#endif
            return op;
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            Update(fFrame, true);
        }
        //------------------------------------------------------
        public void Update(float fFrame, bool bCostCheck = true)
        {
            if (fFrame <= 0) fFrame = 0.0333f;
            if (m_fWatingSecond > 0)
            {
                m_fWatingSecond -= fFrame;

                CheckStartCoroutinesLoad();
                return;
            }
            m_fWatingSecond = Mathf.Min(0.5f, InnerUpdate(fFrame, bCostCheck));

            if(m_vUnloadAssetBunds!=null)
            {
                long curTime = GetFramework().GetRunTime();
                for (var node = m_vUnloadAssetBunds.First; node != null;)
                {
                    var next = node.Next;
                    AssetBundleInfo ab = node.Value;
                    if(ab.assetbundle == null || ab.lastTime<=0)
                    {
                        m_vUnloadAssetBunds.Remove(node);
                        node = next;
                        continue;
                    }
                    if (curTime - ab.lastTime >= 10000)
                    {
                        if (ab.refCnt <= 0)
                            ab.Unload(true);
                        else
                            ab.Unload(false);
                        m_vUnloadAssetBunds.Remove(node);
                        node = next;
                        continue;
                    }
                    node = next;
                }
            }
            CheckStartCoroutinesLoad();
        }
        //------------------------------------------------------
        float InnerUpdate(float fFrame, bool bCostCheck = true)
        {
            DateTime nowTime = DateTime.Now;
            int maxCnt = 0;
            Asset pCheck = null;

            float fCheckingCostTime = 0.0f;
#if !USE_SERVER
            if (!m_bEnableCoroutines || m_pFramework==null)
            {
                while (m_vReqCreateBundleQueue.Count > 0)
                {
                    AssetBundleInfo reqInfo = m_vReqCreateBundleQueue.First.Value;
                    m_vReqCreateBundleQueue.RemoveFirst();
                    if (reqInfo.bAsync || Application.platform == RuntimePlatform.WebGLPlayer)
                        FileSystemUtil.AsyncLoadAssetBundle(this, reqInfo);
                    else
                        FileSystemUtil.LoadAssetBundle(this, reqInfo);
                    maxCnt++;
                    if (bCostCheck)
                    {
                        if ((DateTime.Now - nowTime).TotalMilliseconds >= m_lOneFrameCost)
                        {
                            return Mathf.Max(m_lOneFrameCost, (long)(DateTime.Now - nowTime).TotalMilliseconds - m_lOneFrameCost) * 0.001f;
                        }
                        if (maxCnt >= m_nDealCreateBundleMaxCnt)
                        {
                            return 0.01f;
                        }
                    }

                    if (m_vReqCreateBundleQueue.Count <= 0) break;
                }

                ProcessReq(ref m_vReqAssetOps, false, m_nDealCreateBundleMaxCnt);
                ProcessReq(ref m_vAsyncReqAssetOps, true, m_nDealCreateBundleMaxCnt);

                fCheckingCostTime = CheckLodingPool(bCostCheck, nowTime);
            }
#else
            ProcessReq(ref m_vReqAssetOps, false, m_nDealCreateBundleMaxCnt);
            ProcessReq(ref m_vAsyncReqAssetOps, true, m_nDealCreateBundleMaxCnt);
            fCheckingCostTime = CheckLodingPool(bCostCheck, nowTime);
#endif
            float fInstanceCostTime = 0;
            if(m_pInstancePool!=null) fInstanceCostTime = m_pInstancePool.Update(fFrame, nowTime, bCostCheck);

            //! release asset
            for (var node = m_vReleasePool.First; node != null;)
            {
                var next = node.Next;
                pCheck = node.Value;
                if (pCheck.CheckDispose())
                {
                    m_vReleasePool.Remove(node);
                }
                node = next;
            }

            AssetBundleInfo pCheckAb = null;
            for (var node = m_vReleaseAssetBundlePool.First; node != null;)
            {
                var next = node.Next;
                pCheckAb = node.Value;
                if (pCheck.CheckDispose())
                {
                    m_vReleaseAssetBundlePool.Remove(node);
                }
                node = next;
            }
            return Math.Max(fCheckingCostTime, fInstanceCostTime);
        }
        //------------------------------------------------------
        float CheckLodingPool(bool bCostCheck, DateTime nowTime)
        {
            for (var node = m_vCheckingPool.First; node != null;)
            {
                var next = node.Next;
                var pCheck = node.Value;
                if (pCheck.Check())
                    m_vCheckingPool.Remove(node);

                node = next;

                if (bCostCheck && (DateTime.Now - nowTime).TotalMilliseconds >= m_lOneFrameCost)
                {
                    return Mathf.Max(m_lOneFrameCost, (long)(DateTime.Now - nowTime).TotalMilliseconds - m_lOneFrameCost) * 0.001f;
                }
            }
            return 0;
        }
        //------------------------------------------------------
        void ProcessReq(ref LinkedList<AssetOperiaon> vAssetOp, bool bAsync,int maxDeal=-1)
        {
            if (vAssetOp == null) return;
            if(maxDeal <=0 || maxDeal >= vAssetOp.Count)
            {
                int fDelta = ((GetFramework() != null) ? (int)(GetFramework().GetDeltaTime() * 1000) : 33);
                for (var node = vAssetOp.First; node != null;)
                {
                    var next = node.Next;
                    var op = node.Value;
                    if(op.GetDelayMS() <= 0)
                    {
                        vAssetOp.Remove(node);
                        if ((!op.bScene && string.IsNullOrEmpty(op.strFile)) ||
                            (op.bScene && string.IsNullOrEmpty(op.sceneName)))
                        {
                            if (op.OnCallback != null)
                            {
                                op.OnCallback(op);
                                AssetOperiaon.Free(op);
                            }
                            node = next;
                            continue;
                        }

                        ProcessOp(op, bAsync, false);
                    }
                    else
                    {
                        op.UpdateDelay(fDelta);
                    }
                    node = next;
                }
            }
            else
            {
                int dealCount = maxDeal;
                int fDelta = ((GetFramework()!=null)?(int)(GetFramework().GetDeltaTime()*1000):33);
                for (var node = vAssetOp.First; node != null && dealCount>=0;)
                {
                    var next = node.Next;
                    var op = node.Value;
                    if (op.GetDelayMS() <=0)
                    {
                        vAssetOp.Remove(node);
                        if ((!op.bScene && string.IsNullOrEmpty(op.strFile)) || (op.bScene && string.IsNullOrEmpty(op.sceneName)))
                        {
                            if (op.OnCallback != null)
                            {
                                op.OnCallback(op);
                                AssetOperiaon.Free(op);
                            }
                            node = next;
                            continue;
                        }
                        if (ProcessOp(op, bAsync, false))
                        {
                            dealCount--;
                        }
                    }
                    else
                    {
                        op.UpdateDelay(fDelta);
                    }
                    node = next;
                }
            }
        }
#if !USE_SERVER
        //------------------------------------------------------
        public Asset FindAndLoadAsset(string strFile, bool bAsync = false, bool bImmediately = false, bool bPremanent = false, System.Type assignAssetType = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
#if UNITY_EDITOR
            string strRawFile = BuildAssetFilePath(strFile);
            CheckDynamicAble(strRawFile);
#endif
            Asset pOutAsset = null;
            if (m_vPermanentResources.TryGetValue(strFile, out pOutAsset) ||
                m_vResources.TryGetValue(strFile, out pOutAsset))
            {
                if(pOutAsset.Status != Asset.EStatus.None)
                    return pOutAsset;
            }
            return LoadAsset(strFile, bAsync, bImmediately, bPremanent, assignAssetType);
        }
        //------------------------------------------------------
        public Asset LoadAsset(string strFile, bool bAsync = false, bool bImmediately = false, bool bPremanent = false, System.Type assignAssetType = null, AssetOperiaon onCallback = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            string strRawFile = BuildAssetFilePath(strFile);
#if UNITY_EDITOR
            CheckDynamicAble(strRawFile);
#endif
            Asset pOutAsset = null;
            if (m_vPermanentResources.TryGetValue(strFile, out pOutAsset) ||
                m_vResources.TryGetValue(strFile, out pOutAsset))
            {
                pOutAsset.bScene = false;
                pOutAsset.sceneName = null;
                pOutAsset.bImdmediately = bImmediately;
                pOutAsset.bAsync = bAsync;
                pOutAsset.SetAssignAssetType(assignAssetType);
                if(onCallback!=null) pOutAsset.AddCallback(onCallback);

                if (bImmediately)
                {
                    if (!pOutAsset.Check())
                    {
                        m_vCheckingPool.AddLast(pOutAsset);
                    }
                }
                else
                    m_vCheckingPool.AddLast(pOutAsset);
                m_vReleasePool.Remove(pOutAsset);
                return pOutAsset;
            }

            pOutAsset = new Asset(this);
            pOutAsset.Path = strFile;
#if UNITY_EDITOR
            pOutAsset.RawPath = strRawFile;
#endif
            pOutAsset.SetAssignAssetType(assignAssetType);
            if (bPremanent)
                m_vPermanentResources.Add(strFile, pOutAsset);
            else
                m_vResources.Add(strFile, pOutAsset);

            pOutAsset.bScene = false;
            pOutAsset.sceneName = null;

            pOutAsset.bImdmediately = bImmediately;
            pOutAsset.bAsync = bAsync;
            if (onCallback != null) pOutAsset.AddCallback(onCallback);
            if (bImmediately)
            {
                if (!pOutAsset.Check())
                {
                    m_vCheckingPool.AddLast(pOutAsset);
                }
            }
            else
                m_vCheckingPool.AddLast(pOutAsset);
            return pOutAsset;
        }
#endif
        //------------------------------------------------------
        void OnProcessOp(AssetOperiaon pOp, bool bAsync = false, bool bImmediately = false)
        {
            ProcessOp(pOp, bAsync, bImmediately);
        }
        //------------------------------------------------------
        bool ProcessOp(AssetOperiaon pOp, bool bAsync = false, bool bImmediately = false)
        {
            string strKey = pOp.strFile;
            if (pOp.bScene) strKey = pOp.sceneName;
            if (string.IsNullOrEmpty(strKey))
                return true;
            Asset pOutAsset = null;
            if (m_vPermanentResources.TryGetValue(strKey, out pOutAsset) ||
                m_vResources.TryGetValue(strKey, out pOutAsset))
            {
                pOp.pAsset = pOutAsset;
                pOutAsset.AddCallback(pOp);
                pOutAsset.bScene = pOp.bScene;
                pOutAsset.sceneName = pOp.sceneName;
                pOutAsset.loadMode = pOp.loadMode;

                pOutAsset.bImdmediately = bImmediately;
                pOutAsset.bAsync = bAsync;

                pOutAsset.SetAssignAssetType(pOp.assingType);

                m_vReleasePool.Remove(pOutAsset);
                if (!pOutAsset.Check())
                {
                    m_vCheckingPool.AddLast(pOutAsset);
                }
                if (pOutAsset.Status == Asset.EStatus.Loaded || pOutAsset.Status == Asset.EStatus.Failed) return false;
                return true;
            }

            pOutAsset = new Asset(this);
            pOutAsset.Path = pOp.strFile;
#if UNITY_EDITOR
            pOutAsset.RawPath = pOp.strRawFile;
#endif
            pOutAsset.SetAssignAssetType(pOp.assingType);
            if (pOp.bPermanent)
                m_vPermanentResources.Add(strKey, pOutAsset);
            else
                m_vResources.Add(strKey, pOutAsset);

            pOp.pAsset = pOutAsset;
            pOutAsset.AddCallback(pOp);
            pOutAsset.bScene = pOp.bScene;
            pOutAsset.sceneName = pOp.sceneName;
            pOutAsset.loadMode = pOp.loadMode;

            pOutAsset.bImdmediately = bImmediately;
            pOutAsset.bAsync = bAsync;
            if (!pOutAsset.Check())
            {
                m_vCheckingPool.AddLast(pOutAsset);
            }
            return true;
        }
        //------------------------------------------------------
        public Package GetPackages()
        {
            return m_pPackages;
        }
        //------------------------------------------------------
        public bool RefreshLocalUpdate(bool bRefreshPackage = true)
        {
            if (m_pPackages == null) return true;
            return m_pPackages.RefreshLocalUpdate(m_strUpdateDataPath, bRefreshPackage);
        }
        //------------------------------------------------------
        public Package.EnterData AddFile(string strAbName, string strAbPath, string md5, string[] dependAbs)
        {
            if (m_pPackages == null) return null;
            return m_pPackages.AddFile(strAbName, strAbPath, md5, dependAbs);
        }
        //------------------------------------------------------
        Package GetAbNameAndPath(string strAssetFile, bool bAbName, out string abName, out string abPath, out string[] depends)
        {
            abName = null;
            abPath = null;
            depends = null;
            if (m_pPackages == null) return null;

            if (m_pPackages.GetAbNameAndPath(strAssetFile, bAbName, out abName, out abPath, out depends))
                return m_pPackages;
// 
//             for (int i = 0; i < m_vPackages.Count; ++i)
//             {
//                 if (m_vPackages[i].GetAbNameAndPath(strAssetFile, bAbName, out abName, out abPath, out depends))
//                     return m_vPackages[i];
//             }
            return null;
        }
        //------------------------------------------------------
        public string GetFileAbName(string fileName)
        {
            if (m_vFileABMapping == null) return null;
            string abName = null;
            if (m_vFileABMapping.TryGetValue(fileName, out abName))
                return abName;

            return null;
        }
        //------------------------------------------------------
        public Package.EnterData FindEnterData(string abName)
        {
            if (m_pPackages == null) return null;
            return m_pPackages.FinData(abName);
            //for (int i = 0; i < m_vPackages.Count; ++i)
            //{
            //    enterData = m_vPackages[i].FinData(abName);
            //    if (enterData.isValid()) return enterData;
            //}
            //return null;
        }
#if !USE_SERVER
        //------------------------------------------------------
        public Transform GetPoolRoot()
        {
            if (m_pInstancePool == null) return null;
            return m_pInstancePool.GetPoolRoot();
        }
        //------------------------------------------------------
        public AssetBundleInfo TryGetAssetInfo(string file, bool bAbName = false)
        {
            if (string.IsNullOrEmpty(file)) return null;
            string abName;
            string abPath;
            string[] depends;
            Package pkg = GetAbNameAndPath(file, bAbName, out abName, out abPath, out depends);
            if (pkg == null) return null; 

            AssetBundleInfo abInfo;

            if (!m_vAssetBundles.TryGetValue(abName, out abInfo))
            {
                abInfo = new AssetBundleInfo(this);
                abInfo.abName = abName;
                abInfo.abPath = abPath;
                abInfo.package = pkg;
                abInfo.bReqing = true;
                abInfo.dependAbNames = depends;
                if (AInstancesLimit.IsPermanentAB(abName))abInfo.setPermanent(true);
                else abInfo.setPermanent(false);
                m_vAssetBundles.Add(abName, abInfo);
            }
            return abInfo;
        }
        //-------------------------------------------------
        public void BuildDependsDepth(AssetBundleInfo assetbundle, bool bInitBeing = true)
        {
            if (assetbundle == null) return;
            if (assetbundle == null || assetbundle.initedDeps) return;
            assetbundle.initedDeps = true;
            if (isEnableDebug()) Debug.Log("BuildDependsDepth AB:" + assetbundle.abName);

            if (m_vCheckABDeeps == null) m_vCheckABDeeps = new HashSet<string>();
            if (bInitBeing) m_vCheckABDeeps.Clear();
            else
            {
                if (m_vCheckABDeeps.Contains(assetbundle.abName))
                    return;
            }
            m_vCheckABDeeps.Add(assetbundle.abName);

            if (assetbundle.package != null)
            {
                string[] depnames = assetbundle.dependAbNames;
                if (depnames != null && depnames.Length > 0)
                {
                    if (assetbundle.depends == null)
                        assetbundle.depends = new List<AssetBundleInfo>(depnames.Length);
                    for (int i = 0; i < depnames.Length; ++i)
                    {
                        if (m_vCheckABDeeps.Contains(depnames[i]))
                            continue;

                        AssetBundleInfo depAsset = TryGetAssetInfo(depnames[i], true);
                        if (depAsset != null)
                        {
                            BuildDependsDepth(depAsset, false);
                            m_vCheckABDeeps.Add(depnames[i]);
                            assetbundle.depends.Add(depAsset);
                        }
                    }
                }
            }

            if (bInitBeing) m_vCheckABDeeps.Clear();
        }
        //------------------------------------------------------
        public void OnAssetCallback(Asset asset)
        {
            for(int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnAssetCallback(asset);
            }
        }
        //------------------------------------------------------
        public void OnAssetGrab(Asset asset)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnAssetGrab(asset);
            }
        }
        //------------------------------------------------------
        public void OnAssetRelease(Asset asset)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnAssetRelease(asset);
            }
        }
        //------------------------------------------------------
        public void OnAssetDispose(Asset asset)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnAssetDispose(asset);
            }
        }
        //-------------------------------------------------
        public void OnAssetBundleLoadAsset(Asset asset, AssetBundleInfo assetAb)
        {
            if (m_eType != EFileSystemType.EncrptyPak) return;
            if (asset == null || assetAb == null) return;
            if (m_vUnloadAssetBunds == null) m_vUnloadAssetBunds = new LinkedList<AssetBundleInfo>();
            assetAb.lastTime = (int)GetFramework().GetRunTime();
            m_vUnloadAssetBunds.AddFirst(assetAb);
        }
#else
        public abstract T ReadFile<T>(string strFile) where T : ExternEngine.Object, new();
        public abstract ExternEngine.Object ReadFile(string strFile, System.Type objType);
        public abstract AssetOperiaon ReadFile(string strFile, System.Action<AssetOperiaon> onCallback = null);
        public abstract Asset LoadAsset(string strFile, System.Type objType = null);
#endif
        //------------------------------------------------------
        public abstract void EnableCatchHandle(bool bFileCatch, int catchCnt=64);
        //------------------------------------------------------
        public abstract void DeleteAllPackages();
        //------------------------------------------------------
        public abstract System.IntPtr LoadPackage(string strPakFile);
        //------------------------------------------------------
        public abstract void UnloadPackage(string strPakFile);
        //------------------------------------------------------
        public abstract int ReadBuffer(string strFile, byte[] buffer, int dataSize, int bufferOffset, int offsetRead, bool bAbs);
        //------------------------------------------------------
        public abstract int GetFileSize(string strFile, bool bAbs);
        public abstract byte[] ReadFile(string strFile, bool bAbs, ref int dataSize);
        //------------------------------------------------- 
        protected override void OnDestroy()
        {
            Clear();
            Free();
            m_vCallback.Clear();

            Shutdown();
        }
        //------------------------------------------------------
        public string BuildAssetFilePath(string strFile)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(strFile)) return strFile;
            if (File.Exists(strFile)) return strFile;
            foreach (var path in m_vSearchPaths)
            {
                string fullPath = path + strFile;
                if (File.Exists(fullPath)) return fullPath;
            }
            return strFile;
#else
            return strFile; 
#endif
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        protected void AddSearchPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (m_vSearchPaths.Contains(path)) return;
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path += "/";
            m_vSearchPaths.Add(path);
        }
        //------------------------------------------------------
        protected void AddDynamicPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (m_vOtherDynamicPaths.ContainsKey(path)) return;

            int index = path.IndexOf("/xxx/");
            List<string> vSplit = new List<string>();
            if (index > 0)
            {
                int after = index + "/xxx/".Length;
                vSplit.Add(path.Substring(0, index)+"/");
                vSplit.Add("/"+path.Substring(after, path.Length - after));
                m_vOtherDynamicPaths[path] = vSplit;
            }
            else vSplit.Add(path);
            m_vOtherDynamicPaths[path] = vSplit;
        }
        //------------------------------------------------------
        public void CheckDynamicAble(string strFile)
        {
            foreach (var db in m_vOtherDynamicPaths)
            {
                if(db.Value.Count>0)
                {
                    bool bCan = true;
                    for(int i =0; i < db.Value.Count; ++i)
                    {
                        if(!strFile.Contains(db.Value[i]))
                        {
                            bCan = false;
                            break;
                        }
                    }
                    if (bCan)
                        return;
                }
            }
            if (strFile.Contains("/DatasRef/"))
            {
                Debug.LogError(strFile + "  不支持动态加载");
                Debug.Break();
              //  UnityEditor.EditorUtility.DisplayDialog("提示", strFile + "\r\n不支持动态加载", "好的");
            }
        }
#endif
    }
}

