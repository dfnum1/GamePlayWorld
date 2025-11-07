/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Terrain
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public interface IPathSearcher
    {
        void SetFramework(AFramework framework);
        Vector3 GetStart();
        Vector3 GetEnd();
        void SetCallback(System.Action<IPathSearcher> onCallback);
        void DoCallback();
        IUserData GetUserData();
        void SetUserData(IUserData userData);

        IUserData GetUserData1();
        void SetUserData1(IUserData userData);

        void StartSearch();
        void GetPathList(ref List<Vector3> vPaths);
        void GetGridPathList(ref List<Vector3Int> vPaths);
        void GetRunPath(ref List<RunPoint> vPaths);

        void Destroy();

        bool IsValid();
    }

    public struct NavMeshPathSearcher : IPathSearcher
    {
        public Vector3 start;
        public Vector3 end;
        public float edge_offset;

        private IUserData m_pUserData;
        private IUserData m_pUserData1;
        private Action<IPathSearcher> m_onCallback;
        //------------------------------------------------------
        public void SetFramework(AFramework framework)
        {
        }
        //------------------------------------------------------
        private static UnityEngine.AI.NavMeshPath ms_NavPath ;
        //------------------------------------------------------
        public Vector3 GetStart() { return start; }
        //------------------------------------------------------
        public Vector3 GetEnd() { return end; }
        //------------------------------------------------------
        public bool IsValid()
        {
            return m_onCallback != null && (start-end).sqrMagnitude>0;
        }
        //------------------------------------------------------
        public void GetPathList(ref List<Vector3Int> vPaths)
        {
            if (vPaths == null) return;
            if (ms_NavPath == null || ms_NavPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                return;
            UnityEngine.AI.NavMeshHit navMeshHit;
            Vector3[] corners = ms_NavPath.corners;
            for (int i = 0; i < corners.Length; ++i)
            {
                if (i > 0 && i < corners.Length - 2 && edge_offset > 0)
                {
                    bool result = UnityEngine.AI.NavMesh.FindClosestEdge(corners[i], out navMeshHit, UnityEngine.AI.NavMesh.AllAreas);
                    if (result && navMeshHit.distance < edge_offset * 2)
                    {
                        Vector3 ptF = navMeshHit.position + navMeshHit.normal * edge_offset * 2;
                        Vector3Int pt = Vector3Int.zero;
                        pt.x = (int)ptF.x;
                        pt.y = (int)ptF.y;
                        pt.z = (int)ptF.z;
                        vPaths.Add(pt);
                    }
                    else
                    {
                        Vector3Int pt = Vector3Int.zero;
                        pt.x = (int)corners[i].x;
                        pt.y = (int)corners[i].y;
                        pt.z = (int)corners[i].z;
                        vPaths.Add(pt);
                    }
                }
                else
                {
                    Vector3Int pt = Vector3Int.zero;
                    pt.x = (int)corners[i].x;
                    pt.y = (int)corners[i].y;
                    pt.z = (int)corners[i].z;
                    vPaths.Add(pt);
                }
            }
            Vector3Int tempInt = new Vector3Int((int)start.x, (int)start.y, (int)start.z);
            if (tempInt != vPaths[0])
                vPaths.Insert(0, tempInt);

            tempInt = new Vector3Int((int)end.x, (int)end.y, (int)end.z);
            if (tempInt != vPaths[vPaths.Count-1])
                vPaths.Add(tempInt);
        }
        //------------------------------------------------------
        public void GetRunPath(ref List<RunPoint> vPaths)
        {
            if (vPaths == null) return;
            if (ms_NavPath == null || ms_NavPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                return;

            UnityEngine.AI.NavMeshHit navMeshHit;
            Vector3[] corners = ms_NavPath.corners;
            for (int i = 0; i < corners.Length; ++i)
            {
                if (i > 0 && i < corners.Length - 2 && edge_offset > 0)
                {
                    bool result = UnityEngine.AI.NavMesh.FindClosestEdge(corners[i], out navMeshHit, UnityEngine.AI.NavMesh.AllAreas);
                    if (result && navMeshHit.distance < edge_offset * 2)
                    {
                        vPaths.Add(new RunPoint(navMeshHit.position + navMeshHit.normal * edge_offset * 2));
                    }
                    else
                    {
                        vPaths.Add(new RunPoint(corners[i]));
                    }
                }
                else
                {
                    vPaths.Add(new RunPoint(corners[i]));
                }
            }
            if (BaseUtil.Equal(start, vPaths[0].position))
                vPaths.Insert(0, new RunPoint(start));

            if (BaseUtil.Equal(end, vPaths[vPaths.Count-1].position))
                vPaths.Add(new RunPoint(end));
        }
        //------------------------------------------------------
        public void GetPathList(ref List<Vector3> vPaths)
        {
            if (vPaths == null) return;
            if (ms_NavPath == null || ms_NavPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                return;

            UnityEngine.AI.NavMeshHit navMeshHit;
            Vector3[] corners = ms_NavPath.corners;
            for (int i = 0; i < corners.Length; ++i)
            {
                if (i > 0 && i < corners.Length - 2 && edge_offset > 0)
                {
                    bool result = UnityEngine.AI.NavMesh.FindClosestEdge(corners[i], out navMeshHit, UnityEngine.AI.NavMesh.AllAreas);
                    if (result && navMeshHit.distance < edge_offset * 2)
                    {
                        vPaths.Add(navMeshHit.position + navMeshHit.normal * edge_offset * 2);
                    }
                    else
                    {
                        vPaths.Add(corners[i]);
                    }
                }
                else
                {
                    vPaths.Add(corners[i]);
                }
            }
            if (BaseUtil.Equal(start, vPaths[0]))
                vPaths.Insert(0, start);

            if (BaseUtil.Equal(end, vPaths[vPaths.Count - 1]))
                vPaths.Add(end);
        }
        //------------------------------------------------------
        public void GetGridPathList(ref List<Vector3Int> vPaths)
        {
            if (vPaths == null) return;
            if (ms_NavPath == null || ms_NavPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                return;

            UnityEngine.AI.NavMeshHit navMeshHit;
            Vector3[] corners = ms_NavPath.corners;
            for (int i = 0; i < corners.Length; ++i)
            {
                if (i > 0 && i < corners.Length - 2 && edge_offset > 0)
                {
                    bool result = UnityEngine.AI.NavMesh.FindClosestEdge(corners[i], out navMeshHit, UnityEngine.AI.NavMesh.AllAreas);
                    if (result && navMeshHit.distance < edge_offset * 2)
                    {
                        vPaths.Add(TerrainLayers.WorldPosToWorldGrid3D(navMeshHit.position + navMeshHit.normal * edge_offset * 2));
                    }
                    else
                    {
                        vPaths.Add(TerrainLayers.WorldPosToWorldGrid3D(corners[i]));
                    }
                }
                else
                {
                    vPaths.Add(TerrainLayers.WorldPosToWorldGrid3D(corners[i]));
                }
            }
            Vector3Int startGrid = TerrainLayers.WorldPosToWorldGrid3D(start);
            if (BaseUtil.Equal(startGrid, vPaths[0]))
                vPaths.Insert(0, startGrid);

            Vector3Int endGrid = TerrainLayers.WorldPosToWorldGrid3D(end);
            if (BaseUtil.Equal(endGrid, vPaths[vPaths.Count - 1]))
                vPaths.Add(endGrid);
        }
        //------------------------------------------------------
        public IUserData GetUserData()
        {
            return m_pUserData;
        }
        //------------------------------------------------------
        public void SetUserData(IUserData userData)
        {
            m_pUserData = userData;
        }
        //------------------------------------------------------
        public IUserData GetUserData1()
        {
            return m_pUserData1;
        }
        //------------------------------------------------------
        public void SetUserData1(IUserData userData)
        {
            m_pUserData1 = userData;
        }
        //------------------------------------------------------
        public void SetCallback(Action<IPathSearcher> onCallback)
        {
            m_onCallback = onCallback;
        }
        //------------------------------------------------------
        public void StartSearch()
        {
            if (ms_NavPath == null) ms_NavPath = new UnityEngine.AI.NavMeshPath();
            ms_NavPath.ClearCorners();
            if (!UnityEngine.AI.NavMesh.CalculatePath(start, end, UnityEngine.AI.NavMesh.AllAreas, ms_NavPath))
            {
                ms_NavPath.ClearCorners();
            }
        }
        //------------------------------------------------------
        public void DoCallback()
        {
            if (m_onCallback != null) m_onCallback(this);
            if(ms_NavPath!=null) ms_NavPath.ClearCorners();
            Destroy();
        }
        //------------------------------------------------------
        public void Destroy()
        {
            m_onCallback = null;
            m_pUserData = null;
            m_pUserData1 = null;
        }
    }
}