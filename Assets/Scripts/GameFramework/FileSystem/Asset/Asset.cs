/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Asset
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
#if USE_SERVER
using ExternEngine;
using Object = ExternEngine.Object;
#else
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
#endif
namespace Framework.Core
{
#if !USE_SERVER
    public interface IAssetObject
    {
        bool IsValid();
        void Destroy();
    }
    //------------------------------------------------------
    static class RecycleAssetsOperiaonPool
    {
        public static ObjectSetPool<AssetOperiaon> ASSET_OP_POOLS = new ObjectSetPool<AssetOperiaon>(128);
    }
    public enum ELoadSceneMode
    {
        Single = 0,
        Additive = 1
    }

    public class AssetOperiaon : YieldInstruction, IUserData
    {
        public string strFile = null;
#if UNITY_EDITOR
        internal string strRawFile = null;
#endif
        public bool bPermanent = false;

        public bool bScene = false;
        public ELoadSceneMode loadMode = ELoadSceneMode.Single;
        public string sceneName = null;

        int m_nDelay = 0;
        public void SetDelay(float fDelay)
        {
            m_nDelay = (int)(fDelay*1000);
        }
        public int GetDelayMS()
        {
            return m_nDelay;
        }
        internal void UpdateDelay(int oneMS)
        {
            m_nDelay -= oneMS;
        }

        private bool m_bFreed = false;

        public System.Type assingType = null;
        public Asset pAsset = null;
        public VariableBuffer pBufferAsset;
        public Action<AssetOperiaon> OnCallback = null;

        private IUserData m_userData = null;
        public IUserData userData1 = null;
        public IUserData userData2 = null;
        public IUserData userData3 = null;

        public IUserData userData
        {
            get
            {
                return m_userData;
            }
            set
            {
                m_userData = value;
            }
        }
        public void Clear()
        {
            strFile = null;
#if UNITY_EDITOR
            strRawFile = null;
#endif
            bPermanent = false;
            pAsset = null;
            OnCallback = null;

            bScene = false;
            sceneName = null;
            loadMode = ELoadSceneMode.Single;

            userData = null;
            userData1 = null;
            userData2 = null;
            userData3 = null;
            pBufferAsset.Destroy();

            m_nDelay = 0;
            m_bFreed = true;

            assingType = null;
        }

        public void Destroy()
        {
        }

        public bool isDone
        {
            get
            {
                return pAsset != null && pAsset.Status >= Asset.EStatus.Loaded;
            }
        }

        public bool IsMe(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            return this.strFile.CompareTo(path) == 0;
        }

        public bool isValid()
        {
            return !m_bFreed && pAsset != null && pAsset.IsValid() && pAsset.Status == Asset.EStatus.Loaded;
        }
        public bool isFreed
        {
            get { return m_bFreed; }
        }
        public UnityEngine.Object GetOrigin()
        {
            if (pAsset == null) return null;
            return pAsset.GetOrigin();
        }
        public T GetOrigin<T>() where T : UnityEngine.Object
        {
            if (pAsset == null) return null;
            return pAsset.GetOrigin<T>();
        }


        public static Action<AssetOperiaon, bool, bool> OnProcessCB = null;
        public void Refresh(bool bAsync = false)
        {
            if (OnProcessCB != null) OnProcessCB(this, bAsync, true);
        }

        public float GetProgress()
        {
            if (pAsset != null) return pAsset.GetProgress();
            return 0;
        }

        public void Release(float fDelta = 60)
        {
            if (pAsset != null) pAsset.Release(fDelta);
            pAsset = null;
            if (pBufferAsset.IsValid()) pBufferAsset.Destroy();
            assingType = null;
            Clear();
        }

        public static void Free(AssetOperiaon op)
        {
            op.Clear();
            op.m_bFreed = true;
            RecycleAssetsOperiaonPool.ASSET_OP_POOLS.Release(op);
        }
        public static AssetOperiaon Malloc()
        {
            AssetOperiaon op = RecycleAssetsOperiaonPool.ASSET_OP_POOLS.Get();
            op.m_bFreed = false;
            return op;
        }
    }
    //------------------------------------------------------
    public class Asset : System.IComparable<Asset>, IUserData
    {
        public enum EStatus
        {
            None,
            Loading,
            Loaded,
            Failed,
        }
        public string Path = null;
#if UNITY_EDITOR
        internal string RawPath = null;
#endif
        public EStatus Status = EStatus.None;

        protected AssetBundleRequest m_AsyncReqLoad = null;
        protected AssetBundleRequest m_AsyncReqLoadOfType = null;
        private AssetBundleInfo m_ABInfo = null;
#if UNITY_EDITOR
        public AssetBundleInfo assetbundle
        {
            get { return m_ABInfo; }
        }
#endif

        protected bool callbacking = false;
        protected List<AssetOperiaon> Callbacks = null;

        protected AsyncOperation m_AsyncReq = null;
        public bool bScene = false;
        public ELoadSceneMode loadMode = ELoadSceneMode.Single;
        public string sceneName = null;

        public bool bImdmediately = false;
        public bool bAsync = false;

        private bool m_bDoLoadOriAsseted = false;
        private bool m_bDoLoadTypeAsseted = false;
        protected UnityEngine.Object m_ObjectAsset = null;
        protected UnityEngine.Object m_ObjectAssetOfType = null;

        protected System.Type m_AssignAssetType = null;

        protected bool bInited = false;
        private int m_nRef = 0;
        private float m_fTime = 0;

        public static Asset NULL = new Asset(null) { };

        private AFileSystem m_pFileSystem;
        public Asset(AFileSystem fileSystem)
        {
            m_pFileSystem = fileSystem;

            m_ABInfo = null;
            Path = null;
#if UNITY_EDITOR
            RawPath = null;
#endif
            Status = EStatus.None;
            Callbacks = null;
            callbacking = false;
            bInited = false;
            m_AsyncReqLoad = null;
            m_AsyncReqLoadOfType = null;
            m_nRef = 0;

            bScene = false;
            loadMode = ELoadSceneMode.Single;
            sceneName = null;
            m_AsyncReq = null;

            bAsync = false;
            bImdmediately = false;
            m_fTime = -1;

            m_bDoLoadTypeAsseted = false;
            m_bDoLoadOriAsseted = false;
            m_ObjectAsset = null;
            m_ObjectAssetOfType = null;
            m_AssignAssetType = null;
        }
        //------------------------------------------------------
        public virtual bool IsValid()
        {
            return m_ObjectAsset != null || m_ObjectAssetOfType != null;
        }
        //------------------------------------------------------
        public virtual int GetInstanceID()
        {
            if (m_ObjectAsset == null) return 0;
            return m_ObjectAsset.GetInstanceID();
        }
        //------------------------------------------------------
        public UnityEngine.Object GetOrigin()
        {
            if (m_ObjectAsset == null) return m_ObjectAssetOfType;
            return m_ObjectAsset;
        }
        //------------------------------------------------------
        public T GetOrigin<T>() where T : UnityEngine.Object
        {
            if (m_ObjectAssetOfType && m_ObjectAssetOfType is T)
                return m_ObjectAssetOfType as T;
            return m_ObjectAsset as T;
        }
        //------------------------------------------------------
        public float GetProgress()
        {
            float abProgress = 1f;// ABInfo!=null?ABInfo.GetProgress():1;
            if (bScene)
            {
                if (bAsync)
                {
                    if (m_AsyncReq != null) return (abProgress + m_AsyncReq.progress) * 0.5f;
                    else return abProgress;
                }
            }
            else
            {
                if (bAsync)
                {
                    if (m_AsyncReqLoad != null) return (abProgress + m_AsyncReqLoad.progress) * 0.5f;
                    else if (m_AsyncReqLoadOfType != null) return (abProgress + m_AsyncReqLoadOfType.progress) * 0.5f;
                    else return abProgress;
                }
            }
            return ((Status == EStatus.Loaded) ? 1 : 0 + abProgress) * 0.5f;
        }
        //------------------------------------------------------
        public int RefCnt
        {
            get { return m_nRef; }
        }
        //------------------------------------------------------
        public void Release(float fDelta = 60)
        {
            m_nRef--;
            m_pFileSystem.OnAssetRelease(this);
            if (m_nRef <= 0)
            {
                m_nRef = 0;
                m_fTime = fDelta;
                if (fDelta <= 0)
                {
                    Dispose();
                }
                else
                {
                    m_pFileSystem.AddReleaseAsset(this);
                }
            }
        }
        //------------------------------------------------------
        public void Grab()
        {
            m_nRef++;
            m_pFileSystem.OnAssetGrab(this);
        }
        //------------------------------------------------------
        public bool CheckDispose(bool bForce = false)
        {
            if(bForce)
            {
                Dispose(true);
                return true;
            }
            if (m_nRef > 0)
            {
                m_fTime = -1;
                return true;
            }
            m_fTime -= Time.deltaTime;
            if (m_fTime > 0) return false;

            if (m_fTime <= 0)
                Dispose(!bScene);

            return m_fTime <= 0;
        }
        //------------------------------------------------------
        protected void Dispose(bool bUnlod = true)
        {
            Status = EStatus.None;
            m_pFileSystem.OnAssetDispose(this);
            if (Callbacks != null)
            {
                for (int i = 0; i < Callbacks.Count; ++i)
                {
                    AssetOperiaon.Free(Callbacks[i]);
                }
            }
            Callbacks = null;
            if (m_ObjectAsset)
            {
                if (!(m_ObjectAsset is GameObject) &&
                    !(m_ObjectAsset is Component))
                    Resources.UnloadAsset(m_ObjectAsset);
            }
            if (m_ObjectAssetOfType)
            {
                if (!(m_ObjectAssetOfType is GameObject) &&
                    !(m_ObjectAssetOfType is Component))
                    Resources.UnloadAsset(m_ObjectAssetOfType);
            }
            m_ObjectAssetOfType = null;

            m_bDoLoadTypeAsseted = false;
            m_bDoLoadOriAsseted = false;
            if (m_ABInfo != null) m_ABInfo.Unload(bUnlod, Path);
            m_ABInfo = null;
            bInited = false;
            m_AsyncReqLoad = null;
            m_AsyncReqLoadOfType = null;
            m_fTime = -1;
            m_nRef = 0;

            bAsync = false;
            bImdmediately = false;

            bScene = false;
            loadMode = ELoadSceneMode.Single;
            sceneName = null;
            m_AsyncReq = null;
            m_AssignAssetType = null;
        }
        //------------------------------------------------------
        public int CompareTo(Asset other)
        {
            if (Path.CompareTo(other.Path) == 0) return 0;
            return 1;
        }
        //------------------------------------------------------
        public void SetAssignAssetType(System.Type assignType)
        {
            if (bScene) return;
            //if (m_AssignAssetType == assignType) return;
            //if (assignType != null)
            //{
            //    if (m_ObjectAssetOfType)
            //    {
            //        m_bDoLoadTypeAsseted = false;
            //        if (!(m_ObjectAssetOfType is GameObject) &&
            //            !(m_ObjectAssetOfType is Component))
            //            Resources.UnloadAsset(m_ObjectAssetOfType);
            //        m_ObjectAssetOfType = null;
            //    }
            //    m_AssignAssetType = assignType;
            //}
            //else m_bDoLoadTypeAsseted = true;
        }
        //------------------------------------------------------
        public void AddCallback(AssetOperiaon pOp)
        {
            if (Callbacks == null) Callbacks = new List<AssetOperiaon>(8);
            if(!Callbacks.Contains(pOp)) Callbacks.Add(pOp);
        }
        //------------------------------------------------------
        public void DoCallback()
        {
            if (Callbacks == null || Callbacks.Count <= 0) return;
            if (callbacking) return;
            callbacking = true;
            int preCnt = m_nRef;
            for (int i = 0; i < Callbacks.Count; ++i)
            {
                if (Callbacks[i].isFreed) continue;
                Callbacks[i].pAsset = this;
                if (Callbacks[i].OnCallback != null)
                    Callbacks[i].OnCallback(Callbacks[i]);

                if (preCnt > m_nRef || Callbacks == null || Status == EStatus.None) break;

                if (Callbacks[i] != null)
                {
                    //  Callbacks[i].Clear();
                    AssetOperiaon.Free(Callbacks[i]);
                }
            }
            if (Callbacks != null) Callbacks.Clear();
            callbacking = false;
            m_pFileSystem.OnAssetCallback(this);
        }
        //------------------------------------------------------
        public bool Check()
        {
            if (Status == EStatus.Loaded || Status == EStatus.Failed)
            {
                if(Status == EStatus.Loaded)
                {
                    if (m_ObjectAsset == null)
                    {
                        m_bDoLoadOriAsseted = false;
                        Status = EStatus.Loading;
                        return false;
                    }
                }
                DoCallback();
                return true;
            }
            if (Status == EStatus.None)
            {
                //! do loading
                Status = EStatus.Loading;
                return DoLoading(bAsync, bImdmediately);
            }
            else if (Status == EStatus.Loading)
            {
                return DoLoading(bAsync, bImdmediately);
            }
            return true;
        }
        //------------------------------------------------------
        protected bool DoLoading(bool bAsync, bool bImmediately = false)
        {
            if (m_pFileSystem == null)
            {
                return false;
            }
            EFileSystemType eType = m_pFileSystem.GetStreamType();
            if (eType == EFileSystemType.AssetBundle || eType == EFileSystemType.EncrptyPak)
            {
                if (bScene && string.IsNullOrEmpty(Path))
                {
                    if (m_pFileSystem.IsStreamSceneAB())
                    {
                        if (m_ABInfo == null)
                        {
                            m_ABInfo = m_pFileSystem.TryGetAssetInfo(Path);
                            if (m_ABInfo == null)
                            {
                                Status = EStatus.Failed;
                                m_bDoLoadTypeAsseted = true;
                                m_bDoLoadOriAsseted = true;
                                DoCallback();
                                return true;
                            }
                        }
                        m_pFileSystem.BuildDependsDepth(m_ABInfo);
                        if (!bInited)
                        {
                            //! inited and build use ref
                            bInited = true;
                            if (bAsync)
                                m_ABInfo.AsyncRequest(Path, bImmediately);
                            else
                                m_ABInfo.LoadAndUsed(Path, bImmediately);
                        }
                        bool bLoaed = false;
                        if (bAsync)
                            bLoaed = m_ABInfo.CheckAsyncLoaded();
                        else
                            bLoaed = m_ABInfo.CheckLoaded();
                        if (!bLoaed)
                        {
                            return false;
                        }
                        if (m_ABInfo.assetbundle == null)
                        {
                            if (m_pFileSystem.isEnableDebug()) Debug.Log("DoLoading Loaded Failed:" + Path);
                            Status = EStatus.Failed;
                            m_bDoLoadTypeAsseted = true;
                            m_bDoLoadOriAsseted = true;
                            DoCallback();
                            return true;
                        }
                    }
                    if (bAsync)
                    {
                        if (m_AsyncReq == null)
                            m_AsyncReq = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                        else
                        {
                            if (m_AsyncReq.isDone)
                            {
                                Status = EStatus.Loaded;
                                m_bDoLoadTypeAsseted = true;
                                m_bDoLoadOriAsseted = true;
                                DoCallback();
                                if (!m_pFileSystem.IsStreamSceneAB())
                                {
                                    //! 解绑场景ab 的引用关系，交给unity 自行的卸载机制卸载
                                    if (m_ABInfo != null) m_ABInfo.Unload(false, Path);
                                    m_ABInfo = null;
                                }
                                m_AsyncReq = null;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                        Status = EStatus.Loaded;
                        m_bDoLoadTypeAsseted = true;
                        m_bDoLoadOriAsseted = true;
                        DoCallback();
                        //! 解绑场景ab 的引用关系，交给unity 自行的卸载机制卸载
                        if (m_ABInfo != null) m_ABInfo.Unload(false, Path);
                        m_ABInfo = null;
                        return true;
                    }
                    return false;
                }

                if(m_ABInfo == null) m_ABInfo = m_pFileSystem.TryGetAssetInfo(Path);
                if (m_ABInfo == null)
                {
                    Status = EStatus.Failed;
                    m_bDoLoadTypeAsseted = true;
                    m_bDoLoadOriAsseted = true;
                    DoCallback();
                    return true;
                }
                else
                {
                    m_pFileSystem.BuildDependsDepth(m_ABInfo);
                    if (!bInited)
                    {
                        //! inited and build use ref
                        bInited = true;
                        if (bAsync)
                            m_ABInfo.AsyncRequest(Path, bImmediately);
                        else
                            m_ABInfo.LoadAndUsed(Path, bImmediately);
                    }
                    //else
                    {
                        bool bLoaed = false;
                        if (bAsync)
                            bLoaed = m_ABInfo.CheckAsyncLoaded();
                        else
                            bLoaed = m_ABInfo.CheckLoaded();

                        if (bLoaed)
                        {
                            if (m_ABInfo.assetbundle == null)
                            {
                                if (m_pFileSystem.isEnableDebug()) Debug.Log("DoLoading Loaded Failed:" + Path);
                                Status = EStatus.Failed;
                                m_bDoLoadTypeAsseted = true;
                                m_bDoLoadOriAsseted = true;
                                DoCallback();
                                return true;
                            }
                            else
                            {
                                if (m_pFileSystem.isEnableDebug()) Debug.Log("DoLoading Loaded Succeed:" + Path);
                                if (bAsync)
                                {
                                    if (bScene)
                                    {
                                        if (m_AsyncReq == null)
                                            m_AsyncReq = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                                        else
                                        {
                                            if (m_AsyncReq.isDone)
                                            {
                                                Status = EStatus.Loaded;
                                                m_bDoLoadTypeAsseted = true;
                                                m_bDoLoadOriAsseted = true;
                                                DoCallback();

                                                //! 解绑场景ab 的引用关系，交给unity 自行的卸载机制卸载
                                                if (m_ABInfo != null) m_ABInfo.Unload(false, Path);
                                                m_ABInfo = null;
                                                m_AsyncReq = null;
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!m_bDoLoadOriAsseted)
                                        {
                                            if (m_AsyncReqLoad == null)
                                                m_AsyncReqLoad = m_ABInfo.assetbundle.LoadAssetAsync(Path);
                                            if (m_AsyncReqLoad.isDone)
                                            {
                                                m_ObjectAsset = m_AsyncReqLoad.asset;

                                                if (m_ObjectAssetOfType == null)
                                                {
                                                    if (m_ObjectAsset != null && m_ObjectAsset is Texture)
                                                    {
                                                        m_AssignAssetType = typeof(Sprite);
                                                        m_bDoLoadTypeAsseted = false;
                                                    }
                                                    else m_bDoLoadTypeAsseted = true;
                                                }
                                                else m_bDoLoadTypeAsseted = true;
                                                m_AsyncReqLoad = null;
                                                m_bDoLoadOriAsseted = true;
                                            }
                                        }

                                        if (!m_bDoLoadTypeAsseted)
                                        {
                                            if (m_AssignAssetType != null)
                                            {
                                                if(m_ObjectAssetOfType==null)
                                                {
                                                    if (m_AsyncReqLoadOfType == null)
                                                        m_AsyncReqLoadOfType = m_ABInfo.assetbundle.LoadAssetAsync(Path, m_AssignAssetType);
                                                    if (m_AsyncReqLoadOfType.isDone)
                                                    {
                                                        m_ObjectAssetOfType = m_AsyncReqLoadOfType.asset;
                                                        m_AsyncReqLoadOfType = null;
                                                        m_bDoLoadTypeAsseted = true;
                                                    }
                                                }
                                                else m_bDoLoadTypeAsseted = true;
                                            }
                                            else m_bDoLoadTypeAsseted = true;
                                        }
                                        if (m_bDoLoadOriAsseted && m_bDoLoadTypeAsseted)
                                        {
                                            Status = (m_ObjectAsset || m_ObjectAssetOfType) ? EStatus.Loaded : EStatus.Failed;
                                            DoCallback();
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (bScene)
                                    {
                                        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                                        Status = EStatus.Loaded;
                                        m_bDoLoadOriAsseted = true;
                                        m_bDoLoadTypeAsseted = true;
                                        DoCallback();
                                        //! 解绑场景ab 的引用关系，交给unity 自行的卸载机制卸载
                                        if (m_ABInfo != null) m_ABInfo.Unload(false, Path);
                                        m_ABInfo = null;
                                    }
                                    else
                                    {
//                                         if (!m_bDoLoadTypeAsseted)
//                                         {
//                                             if (m_AssignAssetType != null)
//                                             {
//                                                 if (m_ObjectAssetOfType == null)
//                                                     m_ObjectAssetOfType = m_ABInfo.assetbundle.LoadAsset(Path, m_AssignAssetType);
//                                                 m_bDoLoadTypeAsseted = true;
//                                             }
//                                         }
                                        if (!m_bDoLoadOriAsseted)
                                        {
                                            if (m_ObjectAsset == null) m_ObjectAsset = m_ABInfo.assetbundle.LoadAsset(Path);
                                            if(m_ObjectAssetOfType == null)
                                            {
                                                if (m_ObjectAsset != null && m_ObjectAsset is Texture)
                                                {
                                                    m_AssignAssetType = typeof(Sprite);
                                                    m_ObjectAssetOfType = m_ABInfo.assetbundle.LoadAsset(Path, m_AssignAssetType);
                                                }
                                            }
            
                                            m_bDoLoadTypeAsseted = true;
                                            m_bDoLoadOriAsseted = true;
                                        }
                                        Status = (m_ObjectAsset || m_ObjectAssetOfType) ? EStatus.Loaded : EStatus.Failed;
                                        DoCallback();
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            else
            {
#if UNITY_EDITOR
                if (bScene)
                {
                    if (bAsync)
                    {
                        if (m_AsyncReq == null)
                            m_AsyncReq = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                        else
                        {
                            if (m_AsyncReq.isDone)
                            {
                                Status = EStatus.Loaded;
                                m_bDoLoadOriAsseted = true;
                                DoCallback();
                                m_AsyncReq = null;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, (UnityEngine.SceneManagement.LoadSceneMode)loadMode);
                        Status = EStatus.Loaded;
                        m_bDoLoadOriAsseted = true;
                        DoCallback();
                        return true;
                    }
                }
                else
                {
                    try
                    {
                        if(!m_bDoLoadOriAsseted)
                        {
                            m_bDoLoadOriAsseted = true;
                            if (m_ObjectAsset == null) m_ObjectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(RawPath);
                        }
                        if(!m_bDoLoadTypeAsseted)
                        {
                            if (m_ObjectAsset != null && m_ObjectAsset is Texture)
                                m_AssignAssetType = typeof(Sprite);
                            if (m_AssignAssetType != null)
                            {
                                m_bDoLoadTypeAsseted = true;
                                if (m_ObjectAssetOfType == null)
                                    m_ObjectAssetOfType = UnityEditor.AssetDatabase.LoadAssetAtPath(RawPath, m_AssignAssetType);
                            }
                        }
                        Status = (m_ObjectAsset || m_ObjectAssetOfType) ? EStatus.Loaded : EStatus.Failed;
                        DoCallback();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Path:" + RawPath);
                        Debug.LogError(ex.ToString());
                    }
                    return true;
                }
#else
                Framework.Plugin.Logger.Error("UnSupport FileSystem Type...");
                Debug.Break();
#endif
                return false;
            }
        }
        //------------------------------------------------------
        public virtual void Destroy() 
        {
            Release();
        }
    }
}
#else
    public enum ELoadSceneMode
    {
        Single = 0,
        Additive = 1
    }
    public class Asset : System.IDisposable
    {
        ExternEngine.Object m_pObject;
        public string Path = null;
        public bool bScene = false;
        public string sceneName = null;
        public bool bImdmediately = true;
        public bool bAsync = false;
        public ELoadSceneMode loadMode = ELoadSceneMode.Single;

        public enum EStatus
        {
            None,
            Loading,
            Loaded,
            Failed,

        }
        public EStatus Status = EStatus.None;

        protected List<AssetOperiaon> Callbacks = null;
        private int m_nRef = 0;
        private AFileSystem m_pFileSystem = null;
        //------------------------------------------------------
        public void Grab() { m_nRef++; }
        //------------------------------------------------------
        public void Release(float delta = 0)
        {
            m_nRef--;
            if(m_nRef<=0)
            {
                Dispose();
            }
        }
        //------------------------------------------------------
        public int RefCnt
        {
            get { return m_nRef; }
        }
        //------------------------------------------------------
        public void SetAssignAssetType(System.Type type)
        {

        }
        public bool IsValid() { return m_pObject != null; }
        public float GetProgress() { return 1; }

        public ExternEngine.Object GetOrigin()
        {
            return m_pObject;
        }
        public T GetOrigin<T>() where T : ExternEngine.Object
        {
            return m_pObject as T;
        }
        //-------------------------------------------------
        public void SetObject(ExternEngine.Object pObj)
        {
            m_pObject = pObj;
        }
        //-------------------------------------------------
        public void AddCallback(AssetOperiaon pOp)
        {
            if (Callbacks == null) Callbacks = new List<AssetOperiaon>(8);
            Callbacks.Add(pOp);
        }
        //-------------------------------------------------
        public void Dispose()
        {
            if (m_pObject != null) m_pObject.Dispose();
             m_pObject = null;
            bScene = false;
            sceneName = null;
            bImdmediately = true;
            bAsync = false;
            m_nRef = 0;
            loadMode = ELoadSceneMode.Single;
            Status = EStatus.None;
        }
        //-------------------------------------------------
        public bool CheckDispose()
        {
            if (m_nRef > 0)
            {
                return false;
            }
            Dispose();
            return true;
        }
        //-------------------------------------------------
        public bool Check()
        {
            return true;
        }
        //-------------------------------------------------
        public Asset(AFileSystem pFileSystem)
        {
            m_pFileSystem = pFileSystem;
        }
        //-------------------------------------------------
        public int GetInstanceID() { return m_pObject.GetInstanceID(); }
    }
    //------------------------------------------------------
    static class RecycleAssetsOperiaonPool
    {
        public static ObjectSetPool<AssetOperiaon> ASSET_OP_POOLS = new ObjectSetPool<AssetOperiaon>(128);
    }
    //-------------------------------------------------
    public class AssetOperiaon
    {
        public Asset pAsset = null;
        public string strFile = null;
        public bool bScene = false;
        public ELoadSceneMode loadMode = ELoadSceneMode.Single;
        public string sceneName = null;
        public bool bPermanent = false;

        public System.Type assingType = null;
        int m_nDelay = 0;
        public void SetDelay(float fDelay)
        {
            m_nDelay = (int)(fDelay * 1000);
        }
        public int GetDelayMS()
        {
            return m_nDelay;
        }
        internal void UpdateDelay(int oneMS)
        {
            m_nDelay -= oneMS;
        }

        public static Action<AssetOperiaon, bool, bool> OnProcessCB = null;
        public System.Action<AssetOperiaon> OnCallback = null;

        public VariablePoolAble userData = null;
        public VariablePoolAble userData1 = null;
        public void Clear()
        {
            strFile = null;
            pAsset = null;
            OnCallback = null;

            bScene = false;
            sceneName = null;
            loadMode = ELoadSceneMode.Single;

            userData = null;
            userData1 = null;

            assingType = null;
            m_nDelay = 0;
            bPermanent = false;
        }

        public void Destroy()
        {
        }

        public bool isDone
        {
            get
            {
                return pAsset != null;
            }
        }
        public bool isValid() { return pAsset != null && pAsset.IsValid(); }
        public ExternEngine.Object GetOrigin()
        {
            if (pAsset == null) return null;
            return pAsset.GetOrigin();
        }
        public T GetOrigin<T>() where T : Object
        {
            if (pAsset == null) return null;
            return pAsset.GetOrigin<T>();
        }
        public static void Free(AssetOperiaon op)
        {
            RecycleAssetsOperiaonPool.ASSET_OP_POOLS.Release(op);
        }
        public static AssetOperiaon Malloc()
        {
            return RecycleAssetsOperiaonPool.ASSET_OP_POOLS.Get();
        }
    }
}
#endif