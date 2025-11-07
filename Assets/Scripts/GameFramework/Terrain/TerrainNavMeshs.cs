/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TerrainNavMeshs
作    者:	HappLI
描    述:	导航
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FVector3 = UnityEngine.Vector3;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Framework.Core
{
    public class TerrainNavMeshs
    {
        AModule m_pModule;
        struct NavData
        {
            public NavMeshData navMeshData;
            public NavMeshDataInstance navInstance;

            public UnityEngine.AsyncOperation asyncOp;
        }
        int m_nAutoGUID = 0;
        Dictionary<int, NavData> m_vInstances = new Dictionary<int, NavData>();
        internal void Awake(AModule moduble)
        {
            m_nAutoGUID = 0;
            m_pModule = moduble;
        }
        //--------------------------------------------------------
        internal void Start()
        {
            m_nAutoGUID = 0;
            m_pModule.GetFramework().BeginCoroutine(AsyncUpdateNavMesh());
        }
        //--------------------------------------------------------
        internal void Update(float fFrame)
        {

        }
        //--------------------------------------------------------
        internal void JobUpdate(float fFrame)
        {

        }
        //--------------------------------------------------------
        IEnumerator AsyncUpdateNavMesh()
        {
            while(true)
            {
                foreach (var db in m_vInstances)
                {
                }
                yield return null;
            }
        }
        //------------------------------------------------------
        public bool HasNavMesh()
        {
            return m_vInstances.Count > 0;
        }
        //--------------------------------------------------------
        internal void Clear()
        {
            foreach (var db in m_vInstances)
            {
                db.Value.navInstance.Remove();
            }
            m_vInstances.Clear();
            if (m_nAutoGUID >= int.MaxValue-1) m_nAutoGUID = 0;
        }
        //------------------------------------------------------
        public int AddNavMesh(Vector3 pos, Quaternion rotation, NavMeshData pData)
        {
            foreach (var db in m_vInstances)
            {
                if (db.Value.navMeshData == pData)
                    return db.Key;
            }

            NavData nav = new NavData();
            nav.navMeshData = pData;
            nav.navInstance = NavMesh.AddNavMeshData(pData, pos, rotation);
            m_vInstances.Add(++m_nAutoGUID, nav);
            return m_nAutoGUID;
        }
        //------------------------------------------------------
        public void RemoveNavMesh(int nGuid)
        {
            if (nGuid < 0)
                return;
            if(m_vInstances.TryGetValue(nGuid, out var navData))
            {
                if(navData.navInstance.valid)
                {
                    navData.navInstance.Remove();
                }
                m_vInstances.Remove(nGuid);
            }
        }
        //------------------------------------------------------
        public static int BuildNavMesh(AFramework framework, Vector3 pos, Quaternion rotation)
        {
            var navMeshs = TerrainManager.GetTerrainNavMeshs(framework);
            if (navMeshs == null) return -1;

            NavMeshData navMesh = new NavMeshData();
            return navMeshs.AddNavMesh(pos, rotation, navMesh);
        }
        //------------------------------------------------------
        public static int AddNavMesh(AFramework framework, Vector3 pos, Quaternion rotation, NavMeshData pData)
        {
            var navMeshs = TerrainManager.GetTerrainNavMeshs(framework);
            if (navMeshs == null) return -1;

            return navMeshs.AddNavMesh(pos, rotation, pData);
        }
        //------------------------------------------------------
        public static void RemoveNavMesh(AFramework framework, int nGuid)
        {
            var navMeshs = TerrainManager.GetTerrainNavMeshs(framework);
            if (navMeshs == null) return;
            navMeshs.RemoveNavMesh(nGuid);
        }
        //------------------------------------------------------
        public static bool SampleNavPoition(FVector3 position, out FVector3 navPos, float maxDistance = 100)
        {
            UnityEngine.AI.NavMeshHit NavHit;
            navPos = position;
            if (UnityEngine.AI.NavMesh.SamplePosition(position, out NavHit, maxDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                navPos = NavHit.position;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool UpdateNavMesh(AFramework framework, int index, List<NavMeshBuildSource> vSources, UnityEngine.Bounds bounds, bool bAsync = true)
        {
            var navMeshs = TerrainManager.GetTerrainNavMeshs(framework);
            if (navMeshs == null) return false;

            if (!navMeshs.m_vInstances.TryGetValue(index, out var navMesh))
                return false;

            var defaultBuildSettings = NavMesh.GetSettingsByID(0);

            return UpdateNavMesh(framework, index, defaultBuildSettings, vSources, bounds, bAsync);
        }
        //------------------------------------------------------
        public static bool UpdateNavMesh(AFramework framework, int index, NavMeshBuildSettings setting, List<NavMeshBuildSource> vSources, UnityEngine.Bounds bounds, bool bAsync = true)
        {
            var navMeshs = TerrainManager.GetTerrainNavMeshs(framework);
            if (navMeshs == null) return false;

            if (!navMeshs.m_vInstances.TryGetValue(index, out var navMesh))
                return false;

            if (bAsync)
            {
                navMesh.asyncOp = NavMeshBuilder.UpdateNavMeshDataAsync(navMesh.navMeshData, setting, vSources, bounds);
                navMeshs.m_vInstances[index] = navMesh;
            }
            else
            {
                NavMeshBuilder.UpdateNavMeshData(navMesh.navMeshData, setting, vSources, bounds);
            }
            return true;
        }
    }
    //--------------------------------------------------------
    public struct NavPathSearch : Framework.Core.IPathSearcher
    {
        public AFramework m_pFramework;
        public enum EStatus
        {
            None = 0,
            Finished = 1,
            NoReach = 2,
            FinishedNoReach = 3,
        }

        Vector3 m_vStart;
        Vector3 m_vEnd;

        int m_nAgentTypeID;
        int m_nAreaFlags;

        IUserData m_pUserData;
        IUserData m_pUserData1;
        System.Action<IPathSearcher> m_onCallback;

        EStatus m_Status;
        //------------------------------------------------------
        public void SetFramework(AFramework framework)
        {
            m_nAgentTypeID = 0;
            m_nAreaFlags = NavMesh.AllAreas;
            m_pFramework = framework;
        }
        //------------------------------------------------------
        public bool IsValid()
        {
            return m_onCallback != null && (m_vStart - m_vEnd).sqrMagnitude > 0;
        }
        //------------------------------------------------------
        public void SetCallback(System.Action<IPathSearcher> callback)
        {
            m_onCallback = callback;
        }
        //------------------------------------------------------
        public Vector3 GetStart() { return m_vStart; }
        //------------------------------------------------------
        public void SetStart(Vector3 point)
        {
            m_vStart = point;
        }
        //------------------------------------------------------
        public Vector3 GetEnd() { return m_vEnd; }

        //------------------------------------------------------
        public void SetEnd(Vector3 point)
        {
            m_vEnd = point;
        }
        //------------------------------------------------------
        public void SetAreaFlags(int flags = (int)NavMesh.AllAreas)
        {
            m_nAreaFlags = flags;
        }
        //------------------------------------------------------
        public void SetAngentTypeID(int nAngentTypeID)
        {
            m_nAgentTypeID = nAngentTypeID;
        }
        //------------------------------------------------------
        public void SetUserData(IUserData userData)
        {
            m_pUserData = userData;
        }
        //------------------------------------------------------
        public IUserData GetUserData()
        {
            return m_pUserData;
        }
        //------------------------------------------------------
        public void SetUserData1(IUserData userData)
        {
            m_pUserData1 = userData;
        }
        //------------------------------------------------------
        public IUserData GetUserData1()
        {
            return m_pUserData1;
        }
        //------------------------------------------------------
        public void StartSearch()
        {
            if (m_pFramework == null)
            {
                m_Status = EStatus.Finished;
                return;
            }
            ClearStatus();
            if (BaseUtil.Equal(m_vStart, m_vEnd))
            {
                m_Status = EStatus.Finished;
                return;
            }
            m_Status = EStatus.Finished;
        }
        //------------------------------------------------------
        public void GetRunPath(ref List<RunPoint> path)
        {
            if (path == null) return;

            var navPath = new NavMeshPath();
            var filter = new NavMeshQueryFilter();
            filter.areaMask = m_nAreaFlags;
            filter.agentTypeID = m_nAgentTypeID;
            NavMesh.CalculatePath(m_vStart, m_vEnd, filter, navPath);
            if(navPath.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < navPath.corners.Length; ++i)
                {
                    path.Add(new RunPoint(navPath.corners[i]));
                }
            }
        }
        //------------------------------------------------------
        public void GetPathList(ref List<Vector3> path)
        {
            if (path == null ) return;
            var navPath = new NavMeshPath();
            var filter = new NavMeshQueryFilter();
            filter.areaMask = m_nAreaFlags;
            filter.agentTypeID = m_nAgentTypeID;
            NavMesh.CalculatePath(m_vStart, m_vEnd, filter, navPath);
            if (navPath.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < navPath.corners.Length; ++i)
                {
                    path.Add(navPath.corners[i]);
                }
            }
        }
        //------------------------------------------------------
        public void GetGridPathList(ref List<Vector3Int> vPaths)
        {
            if (vPaths == null) return;
            var navPath = new NavMeshPath();
            var filter = new NavMeshQueryFilter();
            filter.areaMask = m_nAreaFlags;
            filter.agentTypeID = m_nAgentTypeID;
            NavMesh.CalculatePath(m_vStart, m_vEnd, filter, navPath);
            if (navPath.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < navPath.corners.Length; ++i)
                {
                    var point = navPath.corners[i];
                    vPaths.Add(new Vector3Int((int)point.x, (int)point.y, (int)point.z));
                }
            }
        }
        //------------------------------------------------------
        void ClearStatus()
        {
            m_Status = EStatus.None;
        }
        //------------------------------------------------------
        public void DoCallback()
        {
            if (m_onCallback != null) m_onCallback(this);
            Destroy();
        }
        //------------------------------------------------------
        public void Destroy()
        {
            m_pUserData = null;
            m_pUserData1 = null;
            m_onCallback = null;
            ClearStatus();
        }
    }
}