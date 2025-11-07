
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Asset
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    //------------------------------------------------------
    static class RecycleOperiaonPool
    {
        public static ObjectSetPool<InstanceOperiaon> INSTANCE_OP_POOLS = new ObjectSetPool<InstanceOperiaon>(128);
    }
    public class InstanceOperiaon
    {
        public enum EFlag : byte
        {
            Used = 1 << 0,
            Scene = 1 << 1,
            Async = 1 << 2,
            LoadAssetReqed = 1 << 3,
            Preload = 1 << 4,
        }

        public string strFile = null;
        public Asset pAsset = null;
        public GameObject pAssetPrefab = null;

        public AInstanceAble pPoolAble = null;

        public Transform pByParent = null;
        public bool followByParentLayer = false;

        public Action<InstanceOperiaon> OnSign = null;
        public Action<InstanceOperiaon> OnCallback = null;

        private byte m_nFlag = 0;

        public int limitCheckCnt = -1;

        public UnityEngine.Object userPointer = null;

        public IUserData userData0 = null;
        public IUserData userData1 = null;
        public IUserData userData2 = null;
        public IUserData userData3 = null;
        //------------------------------------------------------
        public static InstanceOperiaon Malloc()
        {
            return RecycleOperiaonPool.INSTANCE_OP_POOLS.Get();
        }
        //------------------------------------------------------
        public static void Free(InstanceOperiaon instOp)
        {
            instOp.Clear();
            RecycleOperiaonPool.INSTANCE_OP_POOLS.Release(instOp);
        }
        //------------------------------------------------------
        public void Clear()
        {
            Earse();
            m_nFlag = 0;
        }
        //------------------------------------------------------
        public void Release()
        {
            if(pPoolAble!=null) pPoolAble.Destroy();
        }
        //------------------------------------------------------
        public void Earse()
        {
            pAssetPrefab = null;
            strFile = null;
            pAsset = null;
            OnSign = null;
            OnCallback = null;
            userPointer = null;
            userData0 = null;
            userData1 = null;
            userData2 = null;
            userData3 = null;

            pPoolAble = null;

            pByParent = null;

            limitCheckCnt = -1;
            followByParentLayer = false;
        }
        //------------------------------------------------------
        public string GetFile()
        {
            return strFile;
        }
        //------------------------------------------------------
        public void SetLimitCheckCnt(int cnt)
        {
            limitCheckCnt = cnt;
        }
        //------------------------------------------------------
        public AInstanceAble GetAble() { return pPoolAble; }
        //------------------------------------------------------
        public void Destroy() { }
        //------------------------------------------------------
        public bool IsUsed() { return bUsed; }
        public void SetUsed(bool used) { bUsed = used; }
        public bool IsAsync() { return bAsync; }
        public void SetAsync(bool isAsync) { bAsync = isAsync; }

        public bool IsScene() { return bScene; }
        public void SetScene(bool isScene) { bScene = isScene; }

        public bool IsPreload() { return isPreload; }
        public void SetIsPreload(bool bPreload) { isPreload = bPreload; }
        //------------------------------------------------------
        public bool bUsed
        {
            get { return (m_nFlag & (int)EFlag.Used) != 0; }
            set { SetFlag(EFlag.Used, value); }
        }
        //------------------------------------------------------
        public bool bScene
        {
            get { return (m_nFlag & (int)EFlag.Scene) != 0; }
            set { SetFlag(EFlag.Scene, value); }
        }
        //------------------------------------------------------
        public bool bAsync
        {
            get { return (m_nFlag & (int)EFlag.Async) != 0; }
            set { SetFlag(EFlag.Async, value); }
        }
        //------------------------------------------------------
        internal bool bLoadAssetReqed
        {
            get { return (m_nFlag & (int)EFlag.LoadAssetReqed) != 0; }
            set { SetFlag(EFlag.LoadAssetReqed, value); }
        }
        //------------------------------------------------------
        public bool isPreload
        {
            get { return (m_nFlag & (int)EFlag.Preload) != 0; }
            set { SetFlag(EFlag.Preload, value); }
        }
        //------------------------------------------------------
        public bool isDone
        {
            get
            {
                return isInvalid;
            }
        }
        //------------------------------------------------------
        void SetFlag(EFlag flag, bool bSet)
        {
            if (bSet) m_nFlag |= (byte)flag;
            else
            {
                m_nFlag &= (byte)(~(byte)flag);
            }
        }
        //------------------------------------------------------
        public void Refresh()
        {
            if (OnRefreshCall != null) OnRefreshCall(this, false, true);
        }
        //------------------------------------------------------
        public bool isInvalid
        {
            get
            {
                return (string.IsNullOrEmpty(strFile) && pAssetPrefab == null) || this == DEFAULT;
            }
        }
        //------------------------------------------------------
        public void SetByParent(Transform pParent)
        {
            pByParent = pParent;
        }
        //------------------------------------------------------
        public Transform GetByParent() { return pByParent; }
        public void SetUserData(int index, IUserData userData)
        {
            switch (index)
            {
                case 0: userData0 = userData; break;
                case 1: userData1 = userData; break;
                case 2: userData2 = userData; break;
                case 3: userData3 = userData; break;
            }
        }
        //------------------------------------------------------
        public T GetUserData<T>(int index) where T : IUserData
        {
            switch (index)
            {
                case 0: if (userData0 is T) return (T)userData0; break;
                case 1: if (userData1 is T) return (T)userData1; break;
                case 2: if (userData2 is T) return (T)userData2; break;
                case 3: if (userData3 is T) return (T)userData3; break;
            }
            return default;
        }
        //------------------------------------------------------
        public bool HasData<T>(int index) where T : IUserData
        {
            switch (index)
            {
                case 0: return userData0 != null && userData0 is T;
                case 1: return userData1 != null && userData1 is T;
                case 2: return userData2 != null && userData2 is T;
                case 3: return userData3 != null && userData3 is T;
            }
            return false;
        }
        //------------------------------------------------------
        public bool HasData(int index)
        {
            switch (index)
            {
                case 0: return userData0 != null;
                case 1: return userData1 != null;
                case 2: return userData2 != null;
                case 3: return userData3 != null;
            }
            return false;
        }
        //------------------------------------------------------
        public delegate int OnRefreshDelegate(InstanceOperiaon pOp, bool bRemove, bool bRefreshImmde);
        public static OnRefreshDelegate OnRefreshCall = null;
        public static InstanceOperiaon DEFAULT = new InstanceOperiaon()
        {
            pAsset = null,
            pAssetPrefab = null,
            strFile = null,
            OnSign = null,
            OnCallback = null,
            pPoolAble = null,
            bUsed = false,
            bScene = false,
            userPointer = null,
            userData0 = null,
            userData1 = null,
            userData2 = null,
            userData3 = null,
            m_nFlag = 0,
        };
    }
    //------------------------------------------------------
    public class InstancePools
    {
#if !USE_SERVER
        LinkedList<InstanceOperiaon> m_vReqs = null;
        Dictionary<string, List<AInstanceAble>> m_vPreInstance = null;
        Dictionary<GameObject, List<AInstanceAble>> m_vPreInstanceByPrefab = null;
        Dictionary<string, int> m_vStats = null;
        Dictionary<int, int> m_vStatsByInstanceID = null;

        HashSet<AInstanceAble> m_vDestroying = null;
        private Transform m_pPooRoot = null;

        float m_fDestroyDeltaTime = 60;
        long m_lCostOneFrameTime = 300;
        int m_nMaxInstanceCnt = 100;
        bool m_bHeadLock = false;
        bool m_bShutdown = false;

        bool m_bEnableCoroutine = false;
        bool m_bCotouting = false;
        AFramework m_pFramework;
        public InstancePools(AFramework pFramework, bool bCoroutine, int oneFrameCost = 300, int maxInstanceCnt = 30)
        {
            m_bShutdown = false;
            m_bEnableCoroutine = bCoroutine;
            m_pFramework = pFramework;
            m_vDestroying = new HashSet<AInstanceAble>(64);
            m_vPreInstance = new Dictionary<string, List<AInstanceAble>>(256);
            m_vPreInstanceByPrefab = new Dictionary<GameObject, List<AInstanceAble>>(128);
            m_vStats = new Dictionary<string, int>(128);
            m_vStatsByInstanceID = new Dictionary<int, int>(128);
            m_vReqs = new LinkedList<InstanceOperiaon>();
            SetCapability(oneFrameCost, maxInstanceCnt);
            m_bHeadLock = false;
            InstanceOperiaon.OnRefreshCall += RefreshInstanceOperation;
            AInstanceAble.OnDestroyLinster += OnDestroyInstanceAbleLinster;
            AInstanceAble.OnRealDestroyLinster += OnRealDestroyInstanceAbleLinster;
            AInstanceAble.OnRecyleLinster += OnRecyleInstanceAbleLinster;
            AInstanceAble.OnPoolStartLinster += OnPoolStartInstanceAbleLinster;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                m_pPooRoot = (new GameObject("PoolRoot")).transform;
                m_pPooRoot.position = Vector3.one * 9000;
                m_pPooRoot.localScale = Vector3.zero;
               // GameObject.DontDestroyOnLoad(m_pPooRoot.gameObject);
               // m_pPooRoot.gameObject.SetActive(false);
            }
#else
             m_pPooRoot = (new GameObject("PoolRoot")).transform;
             m_pPooRoot.position = Vector3.one * 9000;
              m_pPooRoot.localScale = Vector3.zero;
         //    GameObject.DontDestroyOnLoad(m_pPooRoot.gameObject);
          //   m_pPooRoot.gameObject.SetActive(false);
#endif
            if (pFramework.gameStartup != null)
            {
                if (pFramework.gameStartup.GetTransform())
                {
                    m_pPooRoot.SetParent(pFramework.gameStartup.GetTransform());
                }
                else
                {
                    if (!pFramework.gameStartup.IsEditor())
                        GameObject.DontDestroyOnLoad(m_pPooRoot.gameObject);
                }
            }
        }
        //------------------------------------------------------
        ~InstancePools()
        {
            m_vDestroying = null;
            m_vPreInstance = null;
            m_vPreInstanceByPrefab = null;
            m_vReqs = null;
            m_vStats = null;
            m_vStatsByInstanceID = null;
            InstanceOperiaon.OnRefreshCall -= RefreshInstanceOperation;
            AInstanceAble.OnDestroyLinster -= OnDestroyInstanceAbleLinster;
            AInstanceAble.OnRealDestroyLinster -= OnRealDestroyInstanceAbleLinster;
            AInstanceAble.OnRecyleLinster -= OnRecyleInstanceAbleLinster;
            AInstanceAble.OnPoolStartLinster -= OnPoolStartInstanceAbleLinster;
        }
        //------------------------------------------------------
        public void SetCapability(int oneFrameCost = 300, int maxInstanceCnt = 30, int destroyDelayTime = 60)
        {
            m_nMaxInstanceCnt = maxInstanceCnt;
            m_lCostOneFrameTime = oneFrameCost;
            SetDelayDestroyParam(destroyDelayTime);
        }
        //------------------------------------------------------
        public void SetDelayDestroyParam(int destroyDelayTime)
        {
            if (destroyDelayTime > 0) m_fDestroyDeltaTime = destroyDelayTime;
        }
        //------------------------------------------------------
        internal Transform GetPoolRoot()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return m_pPooRoot;
#endif
            if (m_pPooRoot == null)
            {
                m_pPooRoot = (new GameObject("PoolRoot")).transform;
                m_pPooRoot.position = Vector3.one * 9000;
                m_pPooRoot.localScale = Vector3.zero;
                GameObject.DontDestroyOnLoad(m_pPooRoot.gameObject);
                //m_pPooRoot.gameObject.SetActive(false);

                if (m_pFramework!=null && m_pFramework.gameStartup != null)
                {
                    if (m_pFramework.gameStartup.GetTransform())
                    {
                        m_pPooRoot.SetParent(m_pFramework.gameStartup.GetTransform());
                    }
                    else
                    {
                        if (!m_pFramework.gameStartup.IsEditor())
                            GameObject.DontDestroyOnLoad(m_pPooRoot.gameObject);
                    }
                }
            }
            return m_pPooRoot;
        }
        //------------------------------------------------------
        public void PreSpawn(List<string> vAssets, bool bAsync = true, bool bFrontQueue = true)
        {
            if (vAssets == null) return;
            for (int i = 0; i < vAssets.Count; ++i)
                PreSpawn(vAssets[i], bAsync, bFrontQueue);
        }
        //------------------------------------------------------
        public void PreSpawn(string strFile, bool bAsync = true, bool bFrontQueue = true)
        {
            if (string.IsNullOrEmpty(strFile)) return;
            InstanceOperiaon pOp = InstanceOperiaon.Malloc();
            pOp.strFile = strFile;
            pOp.OnCallback = null;
            pOp.OnSign = null;
            pOp.bAsync = bAsync;
            pOp.pAssetPrefab = null;
            pOp.isPreload = true;
            if (bFrontQueue) m_vReqs.AddFirst(pOp);
            else m_vReqs.AddLast(pOp);
            m_bHeadLock = true;
            CheckCoroutine();
        }
        //------------------------------------------------------
        public void PreSpawn(GameObject pPrefab, bool bFrontQueue = true)
        {
            if (pPrefab == null) return;
            InstanceOperiaon pOp = InstanceOperiaon.Malloc();
            pOp.strFile = null;
            pOp.OnCallback = null;
            pOp.OnSign = null;
            pOp.bAsync = false;
            pOp.pAssetPrefab = pPrefab;
            pOp.isPreload = true;
            if (bFrontQueue) m_vReqs.AddFirst(pOp);
            else m_vReqs.AddLast(pOp);
            m_bHeadLock = true;
            CheckCoroutine();
        }
        //------------------------------------------------------
        public void PreSpawn(List<GameObject> vPrefabs, bool bFrontQueue = true)
        {
            for (int i = 0; i < vPrefabs.Count; ++i)
            {
                PreSpawn(vPrefabs[i], bFrontQueue);
            }
        }
        //------------------------------------------------------
        public void PreDeSpawnInstance(string strFile, int cnt =1)
        {
            if (string.IsNullOrEmpty(strFile)) return;
            if(cnt <=0)
            {
                InstanceOperiaon pOp;
                for (var node = m_vReqs.First; node != null;)
                {
                    var next = node.Next;
                    pOp = node.Value;
                    if (strFile.CompareTo(pOp.strFile) == 0)
                    {
                        InstanceOperiaon.Free(pOp);
                        m_vReqs.Remove(node);
                    }
                    node = next;
                }
                List<AInstanceAble> vInstance;
                if(m_vPreInstance.TryGetValue(strFile, out vInstance))
                {
                    for(int i =0; i < vInstance.Count; ++i)
                    {
                        if(vInstance[i]) vInstance[i].RecyleDestroy();
                    }
                    vInstance.Clear();
                }
            }
            else
            {
                int statCnt = 0;
                InstanceOperiaon pOp;
                for (var node = m_vReqs.First; node != null;)
                {
                    var next = node.Next;
                    pOp = node.Value;
                    if (strFile.CompareTo(pOp.strFile) == 0)
                    {
                        InstanceOperiaon.Free(pOp);
                        m_vReqs.Remove(node);
                        statCnt++;
                        if (statCnt >= cnt) break;
                    }
                    node = next;
                }
                if(statCnt < cnt)
                {
                    List<AInstanceAble> vInstance;
                    if (m_vPreInstance.TryGetValue(strFile, out vInstance))
                    {
                        for (int i = 0; i < vInstance.Count;)
                        {
                            if (vInstance[i])
                            {
                                vInstance[i].RecyleDestroy();
                                vInstance.RemoveAt(i);
                                statCnt++;
                                if (statCnt >= cnt) break;
                            }
                            else
                            {
                                vInstance.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void PreDeSpawnInstance(GameObject pPrefab, int cnt = 1)
        {
            if (pPrefab == null) return;
            if (cnt <= 0)
            {
                InstanceOperiaon pOp;
                for (var node = m_vReqs.First; node != null;)
                {
                    var next = node.Next;
                    pOp = node.Value;
                    if (pOp.pAssetPrefab == pPrefab)
                    {
                        InstanceOperiaon.Free(pOp);
                        m_vReqs.Remove(node);
                    }
                    node = next;
                }
                List<AInstanceAble> vInstance;
                if (m_vPreInstanceByPrefab.TryGetValue(pPrefab, out vInstance))
                {
                    for (int i = 0; i < vInstance.Count; ++i)
                    {
                        if (vInstance[i]) vInstance[i].RecyleDestroy();
                    }
                    vInstance.Clear();
                }
            }
            else
            {
                int statCnt = 0;
                InstanceOperiaon pOp;
                for (var node = m_vReqs.First; node != null;)
                {
                    var next = node.Next;
                    pOp = node.Value;
                    if (pOp.pAssetPrefab == pPrefab)
                    {
                        InstanceOperiaon.Free(pOp);
                        m_vReqs.Remove(node);
                        statCnt++;
                        if (statCnt >= cnt) break;
                    }
                    node = next;
                }
                if (statCnt < cnt)
                {
                    List<AInstanceAble> vInstance;
                    if (m_vPreInstanceByPrefab.TryGetValue(pPrefab, out vInstance))
                    {
                        for (int i = 0; i < vInstance.Count;)
                        {
                            if (vInstance[i])
                            {
                                vInstance[i].RecyleDestroy();
                                vInstance.RemoveAt(i);
                                statCnt++;
                                if (statCnt >= cnt) break;
                            }
                            else
                            {
                                vInstance.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public int GetPreSpawnStats(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return 0;
            int cnt = 0;
            for (var node = m_vReqs.First; node != null; node = node.Next)
            {
                if (node.Value.strFile != null && node.Value.strFile.CompareTo(strFile) == 0)
                {
                    cnt++;
                }
            }
            List<AInstanceAble> vPools;
            if (m_vPreInstance.TryGetValue(strFile, out vPools))
                cnt += vPools.Count;
            return cnt;
        }
        //------------------------------------------------------
        public int GetPreSpawnStats(GameObject pPrefab)
        {
            if (pPrefab == null) return 0;
            int cnt = 0;
            for (var node = m_vReqs.First; node != null; node = node.Next)
            {
                if (node.Value.pPoolAble == pPrefab)
                {
                    cnt++;
                }
            }
            List<AInstanceAble> vPools;
            if (m_vPreInstanceByPrefab.TryGetValue(pPrefab, out vPools))
                cnt += vPools.Count;
            return cnt;
        }
        //------------------------------------------------------
        public InstanceOperiaon Spawn(string strFile, bool bAsync = true, System.Action<InstanceOperiaon> OnCallback = null, System.Action<InstanceOperiaon> OnSign = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            if (!AInstancesLimit.CanInstnace(strFile)) return null;

            InstanceOperiaon pOp = InstanceOperiaon.Malloc();
            pOp.strFile = strFile;
            pOp.OnCallback = OnCallback;
            pOp.OnSign = OnSign;
            pOp.bAsync = bAsync;
            pOp.pAssetPrefab = null;
            pOp.isPreload = false;
            m_vReqs.AddLast(pOp);
            m_bHeadLock = true;

            CheckCoroutine();
            return pOp;
        }
        //------------------------------------------------------
        public bool Spawn(InstanceOperiaon pInstanceOp)
        {
            if (string.IsNullOrEmpty(pInstanceOp.strFile)) return false;
            if (!AInstancesLimit.CanInstnace(pInstanceOp.strFile)) return false;
            m_vReqs.AddLast(pInstanceOp);
            m_bHeadLock = true;

            CheckCoroutine();
            return true;
        }
        //------------------------------------------------------
        public InstanceOperiaon Spawn(GameObject pAsset, bool bAsync = false, System.Action<InstanceOperiaon> OnCallback = null, System.Action<InstanceOperiaon> OnSign = null)
        {
            if (pAsset == null) return null;
            InstanceOperiaon pOp = InstanceOperiaon.Malloc();
            pOp.strFile = null;
            pOp.bAsync = bAsync;
            pOp.pAssetPrefab = pAsset;
            pOp.OnCallback = OnCallback;
            pOp.OnSign = OnSign;
            pOp.isPreload = false;
            m_vReqs.AddLast(pOp);
            m_bHeadLock = true;

            CheckCoroutine();
            return pOp;
        }
        //------------------------------------------------------
        public void DeSpawn(AInstanceAble pAble, int nCheckMax = 4)
        {
            InnerDeSpawn(pAble, nCheckMax);
        }
        //------------------------------------------------------
        public void InnerDeSpawn(AInstanceAble pAble, int nCheckMax = 4)
        {
            if (pAble == null) return;
            if (!pAble.CanRecyle())
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(pAble.gameObject);
                else
                    GameObject.DestroyImmediate(pAble.gameObject);
#else
                 GameObject.Destroy(pAble.gameObject);
#endif
                return;
            }
            if (pAble.IsRecyled()) return;
            if (nCheckMax <= 32) nCheckMax = 32;
            pAble.OnRecyle();
            bool bRecycle = false;
            List<AInstanceAble> vPools;
            if (pAble.Prefab)
            {
#if UNITY_EDITOR
                if (FileSystemUtil.IsEditorMode)
                {
                    if (Application.isPlaying)
                        GameObject.Destroy(pAble.gameObject);
                    else
                        GameObject.DestroyImmediate(pAble.gameObject);
                    return;
                }
#endif
                if (m_vPreInstanceByPrefab.TryGetValue(pAble.Prefab, out vPools))
                {
                    if (vPools == null) vPools = new List<AInstanceAble>(32);
                    if (vPools.Count < nCheckMax)
                    {
                        vPools.Add(pAble);
                        pAble.SetParent(GetPoolRoot());
                        bRecycle = true;
                    }
                }
                else
                {
                    vPools = new List<AInstanceAble>(32);
                    if (vPools.Count < nCheckMax)
                    {
                        vPools.Add(pAble);
                        pAble.SetParent(GetPoolRoot());
                        bRecycle = true;
                    }
                    m_vPreInstanceByPrefab.Add(pAble.Prefab, vPools);
                }
                if (!bRecycle)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        GameObject.Destroy(pAble.gameObject);
                    else
                        GameObject.DestroyImmediate(pAble.gameObject);
#else
                 GameObject.Destroy(pAble.gameObject);
#endif

                }
                pAble = null;

                return;
            }
            if (string.IsNullOrEmpty(pAble.AssetFile))
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(pAble.gameObject);
                else
                    GameObject.DestroyImmediate(pAble.gameObject);
#else
                 GameObject.Destroy(pAble.gameObject);
#endif
                return;
            }
#if UNITY_EDITOR
            if (FileSystemUtil.IsEditorMode)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(pAble.gameObject);
                else
                    GameObject.DestroyImmediate(pAble.gameObject);
                return;
            }
#endif

            if (m_vPreInstance.TryGetValue(pAble.AssetFile, out vPools))
            {
                if (vPools == null) vPools = new List<AInstanceAble>(32);
                if (vPools.Count < nCheckMax)
                {
                    pAble.SetParent(GetPoolRoot());
                    vPools.Add(pAble);
                    bRecycle = true;
                }
            }
            else
            {
                vPools = new List<AInstanceAble>(32);
                if (vPools.Count < nCheckMax)
                {
                    pAble.SetParent(GetPoolRoot());
                    vPools.Add(pAble);
                    bRecycle = true;
                }
                m_vPreInstance.Add(pAble.AssetFile, vPools);
            }
            if (!bRecycle)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(pAble.gameObject);
                else
                    GameObject.DestroyImmediate(pAble.gameObject);
#else
                 GameObject.Destroy(pAble.gameObject);
#endif

            }
            else
            {
                m_vDestroying.Add(pAble);
            }
            pAble = null;
        }
        //------------------------------------------------------
        public int GetReqCount()
        {
            return m_vReqs.Count;
        }
        //------------------------------------------------------
        public void EnableCoroutines(bool bCoroutines)
        {
            m_bEnableCoroutine = bCoroutines;
            CheckCoroutine();
        }
        //------------------------------------------------------
        void CheckCoroutine()
        {
            if (!m_bEnableCoroutine)
                return;
            if (m_bCotouting)
                return;
            if (m_vReqs.Count > 0 && m_pFramework != null)
            {
                m_bCotouting = true;
                m_pFramework.BeginCoroutine(CoroutineUpdate());
            }
        }
        //------------------------------------------------------
        IEnumerator CoroutineUpdate()
        {
            m_bCotouting = true;
            yield return CoroutineSystem.waitForEndOfFrame;
            int index = 0;
            for (var node = m_vReqs.First; node != null;)
            {
                var next = node.Next;
                var mulCalbback = node.Value;
                int ret = RefreshInstanceOperation(mulCalbback, false);
                if (ret != 0)
                {
                    index++;
                    m_vReqs.Remove(node);
                    if (index >= m_nMaxInstanceCnt)
                        break;
                }
                node = next;
            }
            m_bCotouting = false;
            yield break;
        }
        //------------------------------------------------------
        public float Update(float fFrameTime, DateTime nowTime, bool bCheckCost)
        {
            if (!m_bEnableCoroutine || m_pFramework == null)
            {
                int maxCnt = 0;
                m_bHeadLock = false;
                for (var node = m_vReqs.First; node != null;)
                {
                    var next = node.Next;
                    var mulCalbback = node.Value;
                    if (m_bHeadLock) break;

                    int ret = RefreshInstanceOperation(mulCalbback, false);

                    if (ret != 0)
                    {
                        m_vReqs.Remove(node);
                        if (ret == 2) maxCnt++;
                    }
                    node = next;

                    if (bCheckCost && (DateTime.Now - nowTime).TotalMilliseconds >= m_lCostOneFrameTime)
                    {
                        return Mathf.Max(m_lCostOneFrameTime, (long)(DateTime.Now - nowTime).TotalMilliseconds - m_lCostOneFrameTime) * 0.001f;
                    }
                    if (bCheckCost && maxCnt >= m_nMaxInstanceCnt)
                    {
                        return 0.01f;
                    }
                    if (m_bHeadLock) break;
                }
            }
            float fTime = Time.time;
            AInstanceAble pAble;
            foreach (var db in m_vDestroying)
            {
                pAble = db;
                if ((fTime - db.lastUseTime) >= m_fDestroyDeltaTime)
                {
                    pAble.RealDestroy();
                    break;
                }
            }
            CheckCoroutine();
            return 0;
        }
        //-------------------------------------------------
        void InnerInstance(InstanceOperiaon pOp)
        {
            pOp.bUsed = true;
            if (pOp.OnSign != null)
                pOp.OnSign(pOp);

            if (!pOp.bUsed)
            {
                pOp.pPoolAble = null;
                if (pOp.OnCallback != null)
                    pOp.OnCallback(pOp);
                return;
            }

            List<AInstanceAble> vPools;
            if (pOp.pAssetPrefab != null)
            {
                if (!pOp.isPreload && m_vPreInstanceByPrefab.TryGetValue(pOp.pAssetPrefab, out vPools) && vPools.Count > 0)
                {
                    AInstanceAble pInsAble = vPools[0];
                    m_vDestroying.Remove(pInsAble);
                    vPools.RemoveAt(0);
                    if (pOp.pByParent) pInsAble.SetParent(pOp.pByParent, pOp.followByParentLayer);
                    else pInsAble.SetParent(null);
                    pOp.pPoolAble = pInsAble;
                    if (pInsAble) pInsAble.OnPoolReady();
                    if (pOp.OnCallback != null) pOp.OnCallback(pOp);
                    if (pInsAble) pInsAble.OnPoolStart();
                }
                else
                {
                    GameObject pObj = GameObject.Instantiate<GameObject>(pOp.pAssetPrefab);
                    AInstanceAble pPoolAble = pObj.GetComponent<AInstanceAble>();
                    if (pPoolAble == null)
                        pPoolAble = pObj.AddComponent<AInstanceAble>();
                    else pPoolAble.enabled = true;
                    pOp.pPoolAble = pPoolAble;
                    if (pOp.pByParent) pPoolAble.SetParent(pOp.pByParent, pOp.followByParentLayer);
                    pPoolAble.SetBindPrefab(pOp.pAssetPrefab);
                    if (pPoolAble) pPoolAble.OnPoolReady();
                    if (pOp.OnCallback != null) pOp.OnCallback(pOp);
                    if (pOp.isPreload)
                    {
                        if (!m_vPreInstanceByPrefab.TryGetValue(pOp.pAssetPrefab, out vPools))
                        {
                            vPools = new List<AInstanceAble>(2);
                            m_vPreInstanceByPrefab.Add(pOp.pAssetPrefab, vPools);
                        }
                        pPoolAble.lastUseTime = Time.time;
                        vPools.Add(pPoolAble);
                        pPoolAble.SetParent(GetPoolRoot());
                        m_vDestroying.Add(pPoolAble);
                    }
                    else
                    {
                        if (pPoolAble) pPoolAble.OnPoolStart();
                    }
                }
                return;
            }

            if (!pOp.isPreload && m_vPreInstance.TryGetValue(pOp.strFile, out vPools) && vPools.Count > 0)
            {
                AInstanceAble pInsAble = vPools[0];
                m_vDestroying.Remove(pInsAble);
                vPools.RemoveAt(0);
                if (pOp.pByParent) pInsAble.SetParent(pOp.pByParent, pOp.followByParentLayer);
                else pInsAble.SetParent(null);
                pInsAble.enabled = true;
                pOp.pPoolAble = pInsAble;
                if (pInsAble) pInsAble.OnPoolReady();
                if (pOp.OnCallback != null) pOp.OnCallback(pOp);
                if (pInsAble) pInsAble.OnPoolStart();
            }
            else
            {
                GameObject pObj = GameObject.Instantiate<GameObject>(pOp.pAsset.GetOrigin() as GameObject);
                AInstanceAble pPoolAble = pObj.GetComponent<AInstanceAble>();
                if (pPoolAble == null)
                    pPoolAble = pObj.AddComponent<AInstanceAble>();
                else pPoolAble.enabled = true;
                pOp.pPoolAble = pPoolAble;
                if (pOp.pByParent) pPoolAble.SetParent(pOp.pByParent, pOp.followByParentLayer);
                pPoolAble.SetBindAsset(pOp.pAsset);
                if (pPoolAble) pPoolAble.OnPoolReady();
                if (pOp.OnCallback != null) pOp.OnCallback(pOp);
                if (pOp.isPreload)
                {
                    if (!m_vPreInstance.TryGetValue(pOp.strFile, out vPools))
                    {
                        vPools = new List<AInstanceAble>(2);
                        m_vPreInstance.Add(pOp.strFile, vPools);
                    }
                    pPoolAble.lastUseTime = Time.time;
                    vPools.Add(pPoolAble);
                    pPoolAble.SetParent(GetPoolRoot());
                    m_vDestroying.Add(pPoolAble);
                }
                else
                {
                    if (pPoolAble) pPoolAble.OnPoolStart();
                }
            }
        }
        //-------------------------------------------------
        int RefreshInstanceOperation(InstanceOperiaon pOp, bool bAutoRemoveList = false, bool bRefreshImmde = false)
        {
            if (pOp.isInvalid)
            {
                if (bAutoRemoveList)
                {
                    InstanceOperiaon.Free(pOp);
                    m_vReqs.Remove(pOp);
                }
                return 1;
            }

            if (pOp.limitCheckCnt > 0)
            {
                int cnt = 0;
                if (!string.IsNullOrEmpty(pOp.strFile))
                    cnt = StatsInstanceCount(pOp.strFile, false);
                else if (pOp.pAssetPrefab)
                    cnt = StatsInstanceCount(pOp.pAssetPrefab.GetInstanceID(), false);
                if (cnt > pOp.limitCheckCnt)
                {
                    if (bAutoRemoveList)
                    {
                        InstanceOperiaon.Free(pOp);
                        m_vReqs.Remove(pOp);
                    }
                    return 1;
                }
            }

            bool bReqStep = true;
            if (!pOp.isPreload)
            {
                List<AInstanceAble> vPools;
                if (pOp.pAssetPrefab && m_vPreInstanceByPrefab.TryGetValue(pOp.pAssetPrefab, out vPools) && vPools.Count > 0)
                {
                    bReqStep = false;
                }
                else if (!string.IsNullOrEmpty(pOp.strFile) && m_vPreInstance.TryGetValue(pOp.strFile, out vPools) && vPools.Count > 0)
                {
                    bReqStep = false;
                }
            }

            if (!bReqStep || pOp.pAssetPrefab)
            {
                InnerInstance(pOp);

                if (!pOp.bUsed)
                {
                    if (pOp.pPoolAble != null)
                        DeSpawn(pOp.pPoolAble, 4);
                }
                InstanceOperiaon.Free(pOp);
                if (bAutoRemoveList) m_vReqs.Remove(pOp);
                return 2;
            }

            if (!pOp.bLoadAssetReqed)
            {
                pOp.bUsed = true;
                if (pOp.OnSign != null)
                    pOp.OnSign(pOp);

                if (!pOp.bUsed)
                {
                    pOp.pPoolAble = null;
                    if (pOp.OnCallback != null)
                        pOp.OnCallback(pOp);

                    InstanceOperiaon.Free(pOp);
                    if (bAutoRemoveList) m_vReqs.Remove(pOp);
                    return 1;
                }

                pOp.pAsset = FileSystemUtil.FindAndLoadAsset(pOp.strFile, pOp.bAsync, bRefreshImmde);
                pOp.bLoadAssetReqed = true;
            }

            if (pOp.pAsset == null)
            {
                if (pOp.OnCallback != null)
                    pOp.OnCallback(pOp);

                InstanceOperiaon.Free(pOp);
                if (bAutoRemoveList) m_vReqs.Remove(pOp);
                return 1;
            }
            else if (pOp.pAsset.Status == Asset.EStatus.Loaded)
            {
                if (pOp.pAsset.GetOrigin() != null && pOp.pAsset.GetOrigin() is GameObject)
                {
                    InnerInstance(pOp);
                    if (!pOp.bUsed)
                    {
                        if (pOp.pPoolAble != null)
                            DeSpawn(pOp.pPoolAble, 4);
                    }
                }
                else
                {
                    if (pOp.OnCallback != null)
                        pOp.OnCallback(pOp);
                }
                InstanceOperiaon.Free(pOp);
                if (bAutoRemoveList) m_vReqs.Remove(pOp);
                return 2;
            }
            else if (pOp.pAsset.Status == Asset.EStatus.Failed)
            {
                Debug.LogWarning("Asset Load[" + pOp.pAsset.Path + "] Failed");
                if (pOp.OnCallback != null)
                    pOp.OnCallback(pOp);

                InstanceOperiaon.Free(pOp);
                if (bAutoRemoveList) m_vReqs.Remove(pOp);
                return 2;
            }

            return 0;
        }
        //-------------------------------------------------
        public int StatsInstanceCount(string strFile, bool bIncludeReq = true)
        {
            if (string.IsNullOrEmpty(strFile)) return 0;
            int cnt = 0;
            if (!m_vStats.TryGetValue(strFile, out cnt))
                cnt = 0;
            if (bIncludeReq)
            {
                for (var node = m_vReqs.First; node != null; node = node.Next)
                {
                    if (strFile.CompareTo(node.Value.strFile) == 0)
                        cnt++;
                }
            }

            return cnt;
        }
        //-------------------------------------------------
        public int StatsInstanceCount(int guid, bool bIncludeReq = true)
        {
            if (guid == 0) return 0;
            int cnt = 0;
            if (!m_vStatsByInstanceID.TryGetValue(guid, out cnt))
                cnt = 0;
            if (bIncludeReq)
            {
                for (var node = m_vReqs.First; node != null; node = node.Next)
                {
                    if ((node.Value.pAssetPrefab != null && node.Value.pAssetPrefab.GetInstanceID() == guid))
                        cnt++;
                }
            }

            return cnt;
        }
        //------------------------------------------------------
        void StatsInstance(AInstanceAble pAble, bool bAdd)
        {
            if (pAble == null) return;
            int InstanceID = pAble.InstanceID;
            if (InstanceID != 0)
            {
                if (bAdd)
                {
                    int stat;
                    if (!m_vStatsByInstanceID.TryGetValue(InstanceID, out stat))
                        m_vStatsByInstanceID[InstanceID] = 1;
                    else
                        m_vStatsByInstanceID[InstanceID] = 1 + stat;
                }
                else
                {
                    int stat;
                    if (m_vStatsByInstanceID.TryGetValue(InstanceID, out stat) && stat > 0)
                        m_vStatsByInstanceID[InstanceID] = stat - 1;
                }
            }
            string strFile = pAble.AssetFile;
            if (!string.IsNullOrEmpty(strFile))
            {
                if (bAdd)
                {
                    int stat;
                    if (!m_vStats.TryGetValue(strFile, out stat))
                        m_vStats[strFile] = 1;
                    else
                        m_vStats[strFile] = 1 + stat;
                }
                else
                {
                    int stat;
                    if (m_vStats.TryGetValue(strFile, out stat) && stat > 0)
                        m_vStats[strFile] = stat - 1;
                }
            }
        }
        //------------------------------------------------------
        void OnRealDestroyInstanceAbleLinster(string strFile, GameObject pAsset, AInstanceAble pAble)
        {
            if (m_bShutdown)
                return;
            if (string.IsNullOrEmpty(strFile) && pAsset == null)
                StatsInstance(pAble, false);
            m_vDestroying.Remove(pAble);
            if (pAsset != null)
            {
                List<AInstanceAble> vPool;
                if (m_vPreInstanceByPrefab.TryGetValue(pAsset, out vPool))
                {
                    vPool.Remove(pAble);
                }
                return;
            }
            if (!string.IsNullOrEmpty(strFile))
            {
                List<AInstanceAble> vPool;
                if (m_vPreInstance.TryGetValue(strFile, out vPool))
                {
                    vPool.Remove(pAble);
                }
                return;
            }
        }
        //------------------------------------------------------
        void OnDestroyInstanceAbleLinster(string strFile, GameObject pAsset, AInstanceAble pAble)
        {
            if (m_bShutdown)
                return;
            m_vDestroying.Remove(pAble);
            StatsInstance(pAble, false);
        }
        //------------------------------------------------------
        void OnPoolStartInstanceAbleLinster(string strFile, GameObject pAsset, AInstanceAble pAble)
        {
            if (m_bShutdown)
                return;
            StatsInstance(pAble, true);
        }
        //------------------------------------------------------
        void OnRecyleInstanceAbleLinster(string strFile, GameObject pAsset, AInstanceAble pAble)
        {
            if (m_bShutdown)
                return;
            StatsInstance(pAble, false);
        }
        //------------------------------------------------------
        public void ClearAllPreSpawn(float fDelay)
        {
            if(fDelay<=0)
            {
                foreach (var db in m_vPreInstance)
                {
                    for (int i = 0; i < db.Value.Count; ++i)
                    {
                        if (db.Value[i])
                        {
                            db.Value[i].RealDestroy();
                        }
                    }
                    db.Value.Clear();
                }
                m_vPreInstance.Clear();

                foreach (var db in m_vPreInstanceByPrefab)
                {
                    for (int i = 0; i < db.Value.Count; ++i)
                    {
                        if (db.Value[i])
                        {
                            db.Value[i].RealDestroy();
                        }
                    }
                    db.Value.Clear();
                }
                m_vPreInstanceByPrefab.Clear();
            }
            else
            {
                foreach (var db in m_vPreInstance)
                {
                    for (int i = 0; i < db.Value.Count; ++i)
                    {
                        if (db.Value[i])
                        {
                            m_vDestroying.Add(db.Value[i]);
                        }
                    }
                }

                foreach (var db in m_vPreInstanceByPrefab)
                {
                    for (int i = 0; i < db.Value.Count; ++i)
                    {
                        if (db.Value[i])
                        {
                            m_vDestroying.Add(db.Value[i]);
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            Free(0);

            m_vStats.Clear();
            m_vStatsByInstanceID.Clear();

            ClearReqing();
        }
        //------------------------------------------------------
        public void ClearReqing()
        {
            for (var node = m_vReqs.First; node != null; node = node.Next)
            {
                InstanceOperiaon.Free(node.Value);
            }
            m_vReqs.Clear();
        }
        //------------------------------------------------------
        public void Free(float fDelay)
        {
            ClearAllPreSpawn(fDelay);

            if(fDelay<=0)
            {
                foreach (var db in m_vDestroying)
                {
                    db.RealDestroy();
                }
                m_vDestroying.Clear();
            }
        }
        //------------------------------------------------------
        public void Shutdown()
        {
            if (!m_bShutdown)
                return;

            m_bShutdown = true;
            if (m_pPooRoot)
            {
                int count = m_pPooRoot.childCount;
                for (int i = 0; i < count; ++i)
                {
                    GameObject.Destroy(m_pPooRoot.GetChild(i).gameObject);
                }
                m_pPooRoot.DetachChildren();
                
                GameObject.DestroyImmediate(m_pPooRoot.gameObject);
            }
            m_vReqs.Clear();
            m_vPreInstance.Clear();
            m_vPreInstanceByPrefab.Clear();
            m_vDestroying.Clear();

            m_vStats.Clear();
            m_vStatsByInstanceID.Clear();
        }
#else
        public InstancePools(Module.AFrameworkBase pFramework, bool bCoroutine, int oneFrameCost = 300, int maxInstanceCnt = 30) { }
        public void SetCapability(int oneFrameCost = 300, int maxInstanceCnt = 30, int destroyDelayTime = 60) { }
        public void EnableCoroutines(bool bCoroutines) { }
        public void Clear() { }
        public void Free(float fDelta = 0) { }
        public int GetReqCount() { return 0; }
        public void ClearReqing() { }
        public void SetDelayDestroyParam(int destroyDelayTime){}
        public void Shutdown() { }
        public int StatsInstanceCount(string strFile) { return 0; }
        public int StatsInstanceCount(int guid) { return 0; }
        public float Update(float fFrameTime, DateTime nowTime, bool bCheckCost) { return 0; }
#endif
    }
}
