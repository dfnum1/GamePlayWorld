/********************************************************************
生成日期:	2020-6-16
类    名: 	GridPathOperate
作    者:	HapplI
描    述:	格子寻路操作器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    internal enum EAStarLisType
    {
        Open,
        Close,
    }
    public enum ESearchDir
    {
        EightDir= 0,
        FourDir = 1,
    }

    internal class SearchMap
    {
        public AStarSearchMap searchResult;
        public TerrainGridMap gridMap = null;
        public Vector3Int start = Vector3Int.zero;
        public Vector3Int end = Vector3Int.zero;
        public EActionStateType startAction = EActionStateType.None;
        public EActionStateType endAction = EActionStateType.None;
        public uint f = 0;
        public uint g = 0;
        public uint h = 0;
        public SearchMap parent = null;
        public SearchMap next = null;
        public void Destroy()
        {
            Clear();
        }
        //------------------------------------------------------
        public void Clear()
        {
            start = Vector3Int.zero;
            end = Vector3Int.zero;
            f = g = h = 0;
            parent = null;
            next = null;
            gridMap = null;
            startAction = EActionStateType.None;
            endAction = EActionStateType.None;
            searchResult.Destroy();
        }
    }

    internal class SearchNode
    {
        public AFramework pFramework;
        public TerrainGridMap gridMap;
        public Vector3Int pos;
        public uint f;
        public uint g;
        public uint h;
        public SearchNode parent;
        public SearchNode next;
        public void Destroy()
        {
            Clear();
        }
        //------------------------------------------------------
        public void Clear()
        {
            pos = Vector3Int.zero;
            f = g = h = 0;
            parent = null;
            next = null;
            gridMap = null;
        }
    }
    internal class SearchCatch
    {
        static int m_CatchFreeMapCnt = 0;
        static SearchMap ms_FreeListMap = null;


        static int m_CatchFreeNodeCnt = 0;
        static SearchNode ms_FreeListNode = null;
        public static SearchMap MallocMap()
        {
            SearchMap pMap = null;
            if (ms_FreeListMap != null)
            {
                pMap = ms_FreeListMap;
                ms_FreeListMap = ms_FreeListMap.next;
                m_CatchFreeMapCnt--;
            }
            else
                pMap = new SearchMap();
            pMap.Clear();
            return pMap;
        }
        //------------------------------------------------------
        public static void FreeMap(SearchMap search)
        {
            SearchMap pSearch = ms_FreeListMap;
            search.Destroy();
            if (m_CatchFreeMapCnt >= 128) return;
            ms_FreeListMap = search;
            ms_FreeListMap.next = pSearch;
            m_CatchFreeMapCnt++;
        }
        //------------------------------------------------------
        public static SearchNode MallocNode(TerrainGridMap gridMap)
        {
            SearchNode pNode = null;
            if (ms_FreeListNode != null)
            {
                pNode = ms_FreeListNode;
                ms_FreeListNode = ms_FreeListNode.next;
                m_CatchFreeNodeCnt--;
            }
            else
                pNode = new SearchNode();
            pNode.Clear();
            pNode.gridMap = gridMap;
            return pNode;
        }
        //------------------------------------------------------
        public static void FreeNode(SearchNode search)
        {
            SearchNode pNode = ms_FreeListNode;
            search.Destroy();
            if (m_CatchFreeNodeCnt >= 128) return;
            ms_FreeListNode = search;
            ms_FreeListNode.next = pNode;
            m_CatchFreeNodeCnt++;
        }
    }
    //------------------------------------------------------
    //! AStarSearch
    //------------------------------------------------------
    public struct AStarSearch : IPathSearcher
    {
        public AFramework m_pFramework;
        public enum EStatus
        {
            None = 0,
            Finished =1,
            NoReach = 2,
            FinishedNoReach =3,
        }
        Vector3 m_vStart;
        Vector3 m_vEnd;

        Vector3Int m_vMapGridStart;
        Vector3Int m_vMapGridEnd;

        Vector3Int m_vWorldGridStart;
        Vector3Int m_vWorldGridEnd;


        SearchMap m_pOpenList;
        SearchMap m_pCloseList;


        uint m_nBlockFlags;
        ESearchDir m_eSearchDir;
        int m_IfEndBlockFindNearlyUnBlockExtern;
        IUserData m_pUserData;
        IUserData m_pUserData1;

        TerrainGridMap m_pStartGridMap;
        TerrainGridMap m_pEndGridMap;

        System.Action<IPathSearcher> m_onCallback;

        EStatus m_Status;
        //------------------------------------------------------
        public void SetFramework(AFramework framework)
        {
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
        public void SetBlockFlags(uint flags)
        {
            m_nBlockFlags = flags;
        }
        //------------------------------------------------------
        public void SetEndBlockFindNearlyExtern(int nExtern)
        {
            m_IfEndBlockFindNearlyUnBlockExtern = nExtern;
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
        public void SetSearchDir(ESearchDir searchDir)
        {
            m_eSearchDir = searchDir;
        }
        //------------------------------------------------------
        EStatus SearchGridMap(SearchMap gridMap)
        {
            gridMap.searchResult = new AStarSearchMap();

            Vector3 startPos = TerrainLayers.WorldGridToWorldPos(gridMap.start);
            Vector3 endPos = TerrainLayers.WorldGridToWorldPos(gridMap.end);
            if( gridMap.parent != null)
            {
                TerrainGridLinkOffline terrainOffline = TerrainLayers.GetTerrainGridLinkOffline(m_pFramework, gridMap.gridMap.GetID(), gridMap.parent.gridMap.GetID());
                if(terrainOffline.IsValid())
                {
                    float nearlyDist = float.MaxValue;
                    for (int i = 0; i <= terrainOffline.lineWidth; ++i)
                    {
                        Vector3Int temp = terrainOffline.GetLineGridStartFromWidth(i);
                        if (!gridMap.gridMap.GetGridByWorld(temp).IsState(m_nBlockFlags))
                        {
                            float dist = (temp - gridMap.end).sqrMagnitude + (temp - gridMap.end).sqrMagnitude;
                            if (dist <= nearlyDist)
                            {
                                nearlyDist = dist;
                                startPos = TerrainLayers.WorldGridToWorldPos(temp);
                            }
                        }
                    }
                    nearlyDist = float.MaxValue;
                    for (int i = 0; i <= terrainOffline.lineWidth; ++i)
                    {
                        Vector3Int temp = terrainOffline.GetLineGridEndFromWidth(i);
                        if (!gridMap.parent.gridMap.GetGridByWorld(temp).IsState(m_nBlockFlags))
                        {
                            float dist = (temp - gridMap.parent.start).sqrMagnitude + (temp - gridMap.parent.start).sqrMagnitude;
                            if (dist <= nearlyDist)
                            {
                                nearlyDist = dist;
                                gridMap.parent.end = temp;
                            }
                        }
                    }
                }
            }

            gridMap.searchResult.SetStart(startPos);
            gridMap.searchResult.SetEnd(endPos);
            gridMap.searchResult.SetBlockFlags(m_nBlockFlags);
            gridMap.searchResult.SetGridMap(gridMap.gridMap);
            gridMap.searchResult.SetSearchDir(m_eSearchDir);
            gridMap.searchResult.SetEndBlockFindNearlyExtern(0);
            gridMap.searchResult.StartSearch();
            return gridMap.searchResult.GetStatus();
        }
        //------------------------------------------------------
        SearchMap BuildCrossMapOffline(SearchMap searchMap, TerrainGridMap toMap)
        {
            if (searchMap == null || toMap == null) return null;
            if (searchMap.gridMap == toMap) return null;

            TerrainGridLinkOffline terrainOffline = TerrainLayers.GetTerrainGridLinkOffline(m_pFramework, searchMap.gridMap.GetID(), toMap.GetID());
            if (!terrainOffline.IsValid()) return null;

            float nearlyDist = float.MaxValue;
            int useStartWidth = -1;
            for (int i = 0; i <= terrainOffline.lineWidth; ++i)
            {
                Vector3Int startPos = terrainOffline.GetLineGridStartFromWidth(i);
                if (!searchMap.gridMap.GetGridByWorld(startPos).IsState(m_nBlockFlags))
                {
                    float dist = (startPos - m_vWorldGridStart).sqrMagnitude + (startPos - m_vWorldGridEnd).sqrMagnitude;
                    if (dist <= nearlyDist)
                    {
                        nearlyDist = dist;
                        useStartWidth = i;
                    }
                }
            }
            if (useStartWidth == -1)
                return null;

            int useEndWidth = -1;
            nearlyDist = float.MaxValue;
            for (int i = 0; i <= terrainOffline.lineWidth; ++i)
            {
                Vector3Int endPos = terrainOffline.GetLineGridEndFromWidth(i);
                if (!toMap.GetGridByWorld(endPos).IsState(m_nBlockFlags))
                {
                    float dist = (endPos - m_vWorldGridStart).sqrMagnitude + (endPos - m_vWorldGridEnd).sqrMagnitude;
                    if (dist <= nearlyDist)
                    {
                        nearlyDist = dist;
                        useEndWidth = i;
                    }
                }
            }
            if (useEndWidth == -1)
                return null;

            searchMap.end = terrainOffline.GetLineGridStartFromWidth(useStartWidth);
            searchMap.endAction = terrainOffline.endAction;

            SearchMap linkMap = SearchCatch.MallocMap();
            linkMap.start = terrainOffline.GetLineGridEndFromWidth(useEndWidth);
            linkMap.startAction = terrainOffline.startAction;
            linkMap.gridMap = toMap;
            if (toMap == m_pEndGridMap) linkMap.end = m_vWorldGridEnd;
            return linkMap;
        }
        //------------------------------------------------------
        public void StartSearch()
        {
            if(m_pFramework == null)
            {
                m_Status = EStatus.Finished;
                return;
            }
            ClearStatus();
            if (BaseUtil.Equal(m_vStart,m_vEnd))
            {
                m_Status = EStatus.Finished;
                return;
            }
            m_vWorldGridStart = TerrainLayers.WorldPosToWorldGrid3D(m_vStart);
            m_vWorldGridEnd = TerrainLayers.WorldPosToWorldGrid3D(m_vEnd);
            if (m_vWorldGridStart == m_vWorldGridEnd)
            {
                m_Status = EStatus.Finished;
                return;
            }
            m_pStartGridMap = TerrainLayers.GetGridMap(m_pFramework, m_vWorldGridStart,true);
            m_pEndGridMap = TerrainLayers.GetGridMap(m_pFramework, m_vWorldGridEnd, true);

            if (m_pStartGridMap == null || m_pEndGridMap == null)
            {
                m_Status = EStatus.NoReach;
                return;
            }

            Vector3Int[] offDirs = null;
            if (m_eSearchDir == ESearchDir.FourDir) offDirs = AStarSearchMap.ms_FourOff;
            else offDirs = AStarSearchMap.ms_EightOff;
            int dirLen = offDirs.Length;

            m_vMapGridStart = m_pStartGridMap.WorldGridToMapGrid3D(m_vWorldGridStart);
            m_vMapGridEnd = m_pEndGridMap.WorldGridToMapGrid3D(m_vWorldGridEnd);
            if (IsGridBlocked(m_pStartGridMap,m_vMapGridStart))
            {
                if (m_IfEndBlockFindNearlyUnBlockExtern > 0)
                {
                    int nearyDis = int.MaxValue;
                    Vector3Int srcEnd = m_vMapGridStart;
                    for (int d = 1; d <= m_IfEndBlockFindNearlyUnBlockExtern; ++d)
                    {
                        for (int i = 0; i < dirLen; ++i)
                        {
                            Vector3Int temp = srcEnd + offDirs[i] * d;
                            if (!IsGridBlocked(m_pStartGridMap,temp))
                            {
                                if ((temp - m_vMapGridEnd).sqrMagnitude <= nearyDis)
                                {
                                    m_vMapGridStart = temp;
                                    m_vWorldGridStart = m_pStartGridMap.MapGridToWorldGrid(temp);
                                    nearyDis = (temp - m_vMapGridEnd).sqrMagnitude;
                                }
                            }
                        }
                    }
                }
            }

            if (IsGridBlocked(m_pEndGridMap, m_vMapGridEnd))
            {
                if (m_IfEndBlockFindNearlyUnBlockExtern > 0)
                {
                    int nearyDis = int.MaxValue;
                    Vector3Int srcEnd = m_vMapGridEnd;
                    for (int d = 1; d <= m_IfEndBlockFindNearlyUnBlockExtern; ++d)
                    {
                        for (int i = 0; i < dirLen; ++i)
                        {
                            Vector3Int temp = srcEnd + offDirs[i] * d;
                            if (!IsGridBlocked(m_pEndGridMap, temp))
                            {
                                if ((temp - m_vMapGridStart).sqrMagnitude <= nearyDis)
                                {
                                    m_vMapGridEnd = temp;
                                    m_vWorldGridEnd = m_pEndGridMap.MapGridToWorldGrid(temp);
                                    nearyDis = (temp - m_vMapGridStart).sqrMagnitude;
                                }
                            }
                        }
                    }
                    if (IsGridBlocked(m_pEndGridMap,m_vMapGridEnd))
                    {
                        m_Status = AStarSearch.EStatus.NoReach;
                        return;
                    }
                }
                else
                {
                    m_Status = AStarSearch.EStatus.NoReach;
                    return;
                }
            }

            SearchMap bestNode = SearchCatch.MallocMap();
            bestNode.g = 0;
            bestNode.gridMap = m_pStartGridMap;
            bestNode.start = m_vWorldGridStart;
            SetHn(bestNode);
            SetFn(bestNode);
            bestNode.parent = null;
            bestNode.next = null;

            int strackLoop = 1000;
            SearchMap nearlyNode = null;
            while (!IsSearchFinished(bestNode))
            {
                List<TerrainGridMap> bridges = TerrainLayers.GetBridgeTerrainGridMaps(m_pFramework, bestNode.gridMap.GetID());
                if(bridges!=null)
                {
                    for(int i =0; i < bridges.Count; ++i)
                    {
                        SearchMap bridgeNode = BuildCrossMapOffline(bestNode, bridges[i]);
                        if (bridgeNode!=null)
                        {
                            nearlyNode = bridgeNode;
                            nearlyNode.parent = bestNode;
                            nearlyNode.next = null;
                            nearlyNode.gridMap = bridges[i];
                            ValidAddToOpenList(nearlyNode);
                        }
                    }
                }
                AddToCloseList(bestNode);
                bestNode = GetBestOpenNode();
                if (bestNode == null) break;
                strackLoop--;
                if (strackLoop <= 0) break;
            }
            if (strackLoop <= 0) m_Status = AStarSearch.EStatus.FinishedNoReach;
            else m_Status = AStarSearch.EStatus.Finished;

            if(m_Status == EStatus.Finished)
            {
                if (bestNode != null)
                    bestNode.end = m_vWorldGridEnd;
                SearchMap pGridMap = GetFormList(m_pEndGridMap, EAStarLisType.Close);
                while (pGridMap != null)
                {
                    EStatus status = SearchGridMap(pGridMap);
                    if(status != EStatus.Finished)
                    {
                        m_Status = status;
                        break;
                    }
                    pGridMap = pGridMap.parent;
                }
            }
        }
        //------------------------------------------------------
        public void GetRunPath(ref List<RunPoint> path)
        {
            if (path == null || m_pStartGridMap == null || m_pEndGridMap == null) return;

            SearchMap pNode = GetFormList(m_pEndGridMap, EAStarLisType.Close);
            TerrainGridLinkOffline lastLinkOffline = TerrainGridLinkOffline.DEF;
            while (pNode != null && pNode.searchResult.IsValid())
            {
                TerrainGridLinkOffline linkOffline = TerrainGridLinkOffline.DEF;
                if (pNode.parent != null)
                {
                    TerrainGridMap fromMap = pNode.gridMap;
                    TerrainGridMap toMap = pNode.parent.gridMap;

                    if (toMap != null && fromMap != null)
                    {
                        //! add offline link
                        linkOffline = TerrainLayers.GetTerrainGridLinkOffline(m_pFramework, fromMap.GetID(), toMap.GetID());
                    }
                }
                int curCnt = path.Count;
                pNode.searchResult.GetPathList(ref path, false);
                if(path.Count>0)
                {
                    if(lastLinkOffline.IsValid())
                    {
                        if(curCnt < path.Count)
                        {
                            RunPoint runPt = path[path.Count-curCnt-1];
                            runPt.actionState = lastLinkOffline.startAction;
                            runPt.speedScale = (float)(lastLinkOffline.speed * 0.01f);
                            path[path.Count - curCnt - 1] = runPt;
                        }
                    }
                    else if(linkOffline.IsValid())
                    {
                        RunPoint runPt = path[0];
                        runPt.actionState = linkOffline.endAction;
                        path[0] = runPt;
                    }
                }
                lastLinkOffline = linkOffline;
                //                 if (linkOffline.IsValid() && linkOffline.IsCurveLine())
                //                 {
                //                     for (int j = 1; j < 10; ++j)
                //                     {
                //                         path.Add(linkOffline.Sample(((float)j) / 10.0f));
                //                     }
                //                 }
                pNode = pNode.parent;
            }
            if (path.Count > 0)
            {
                if (!BaseUtil.Equal(m_vStart, path[0].position))
                {
                    path.Insert(0, new RunPoint(m_vStart));
                }
                if (!BaseUtil.Equal(m_vEnd, path[path.Count - 1].position))
                {
                    path.Add(new RunPoint(m_vEnd));
                }
            }
        }
        //------------------------------------------------------
        public void GetPathList(ref List<Vector3> path)
        {
            if (path == null || m_pStartGridMap == null || m_pEndGridMap == null) return;

            SearchMap pNode = GetFormList(m_pEndGridMap, EAStarLisType.Close);
            while (pNode != null && pNode.searchResult.IsValid())
            {
                pNode.searchResult.GetPathList(ref path,false);

//                 if(pNode.parent!=null)
//                 {
//                     TerrainGridMap fromMap = pNode.gridMap;
//                     TerrainGridMap toMap = pNode.parent.gridMap;
// 
//                     if(toMap!=null && fromMap!=null)
//                     {
//                         //! add offline link
//                         TerrainGridLinkOffline linkOffline = TerrainLayers.GetTerrainGridLinkOffline(fromMap.GetID(), toMap.GetID());
//                         if (linkOffline.IsValid() && linkOffline.IsCurveLine())
//                         {
//                             for (int j = 1; j < 10; ++j)
//                             {
//                                 path.Add(linkOffline.Sample(((float)j) / 10.0f));
//                             }
//                         }
//                     }
//                 }
                pNode = pNode.parent;
            }
            if (path.Count > 0)
            {
                if (!BaseUtil.Equal(m_vStart, path[0]))
                {
                    path.Insert(0, m_vStart);
                }
                if (!BaseUtil.Equal(m_vEnd, path[path.Count - 1]))
                {
                    path.Add(m_vEnd);
                }
            }
        }
        //------------------------------------------------------
        public void GetGridPathList(ref List<Vector3Int> path)
        {
            if (path == null || m_pStartGridMap == null || m_pEndGridMap == null) return;
            SearchMap pNode = GetFormList(m_pEndGridMap, EAStarLisType.Close);
            while (pNode != null && pNode.searchResult.IsValid())
            {
                pNode.searchResult.GetGridPathList(ref path, false);

                //                 if(pNode.parent!=null)
                //                 {
                //                     TerrainGridMap fromMap = pNode.gridMap;
                //                     TerrainGridMap toMap = pNode.parent.gridMap;
                // 
                //                     if(toMap!=null && fromMap!=null)
                //                     {
                //                         //! add offline link
                //                         TerrainGridLinkOffline linkOffline = TerrainLayers.GetTerrainGridLinkOffline(fromMap.GetID(), toMap.GetID());
                //                         if (linkOffline.IsValid() && linkOffline.IsCurveLine())
                //                         {
                //                             for (int j = 1; j < 10; ++j)
                //                             {
                //                                 path.Add(linkOffline.Sample(((float)j) / 10.0f));
                //                             }
                //                         }
                //                     }
                //                 }
                pNode = pNode.parent;
            }
            if ( path.Count > 0)
            {
                Vector3Int temp = TerrainLayers.WorldPosToWorldGrid3D(m_vStart);
                if (!BaseUtil.Equal(temp, path[0]))
                {
                    path.Insert(0, temp);
                }
                temp = TerrainLayers.WorldPosToWorldGrid3D(m_vEnd);
                if (!BaseUtil.Equal(temp, path[path.Count - 1]))
                {
                    path.Add(temp);
                }
            }
        }
        //------------------------------------------------------
        void ClearStatus()
        {
            SearchMap node;
            while (null != m_pOpenList)
            {
                node = m_pOpenList;
                m_pOpenList = m_pOpenList.next;
                node.Destroy();
                SearchCatch.FreeMap(node);
            }

            while (null != m_pCloseList)
            {
                node = m_pCloseList;
                m_pCloseList = m_pCloseList.next;
                node.Destroy();
                SearchCatch.FreeMap(node);
            }
            m_pCloseList = null;
            m_pOpenList = null;

            m_Status = EStatus.None;
        }
        //------------------------------------------------------
        void AddToCloseList(SearchMap node)
        {
            if (node == null)
                return;
            if (null == m_pCloseList)
            {
                m_pCloseList = node;
                m_pCloseList.next = null;
                return;
            }
            SearchMap pNode = m_pCloseList;
            m_pCloseList = node;
            m_pCloseList.next = pNode;
        }
        //------------------------------------------------------
        void AddToOpenList(SearchMap node)
        {
            if (null == m_pOpenList)
            {
                m_pOpenList = node;
                m_pOpenList.next = null;
                return;
            }

            SearchMap preNode = null;
            SearchMap listNode = m_pOpenList;
            while (null != listNode)
            {
                if (listNode.f >= node.f)
                {
                    if (null == preNode)
                    {
                        node.next = listNode;
                        m_pOpenList = node;
                        break;
                    }
                    preNode.next = node;
                    node.next = listNode;
                    break;
                }
                preNode = listNode;
                listNode = listNode.next;
            }
            if (null == listNode)
            {
                preNode.next = node;
                node.next = null;
            }
        }
        //------------------------------------------------------
        void ValidAddToOpenList(SearchMap node)
        {
            if (IsBlocked(node.gridMap))
            {
                SearchCatch.FreeMap(node);
                return;
            }
            SetGn(node);
            SetHn(node);
            SetFn(node);

            SearchMap cachNode = GetFormList(node.gridMap, EAStarLisType.Close);
            if (cachNode != null)
            {
                if (cachNode.g > node.g)
                {
                    cachNode.g = node.g;
                    cachNode.f = node.f;
                    cachNode.h = node.h;
                    cachNode.parent = node.parent;
                }
                SearchCatch.FreeMap(node);
                return;
            }
            cachNode = GetFormList(node.gridMap, EAStarLisType.Open);
            if (cachNode != null)
            {
                if (cachNode.g > node.g)
                {
                    cachNode.g = node.g;
                    cachNode.f = node.f;
                    cachNode.h = node.h;
                    cachNode.parent = node.parent;

                    RemoveFormOpenList(cachNode);
                    AddToOpenList(cachNode);
                }
                SearchCatch.FreeMap(node);
                return;
            }
            AddToOpenList(node);
        }
        //------------------------------------------------------
        void SetGn(SearchMap node)
        {
            if (node.gridMap != node.parent.gridMap)
            {
                node.g =AStarSearchMap.G_CROSS_FAC + node.parent.g;
            }
            else
                node.g = AStarSearchMap.G_FORWARD_FAC + node.parent.g;
        }
        //------------------------------------------------------
        void SetHn(SearchMap node)
        {
            Vector3Int center = node.gridMap.GetCenter();
            node.h = (uint)(Mathf.Abs(center.x - m_vWorldGridEnd.x) + Mathf.Abs(center.z - m_vWorldGridEnd.z) + Mathf.Abs(center.y - m_vMapGridEnd.y)) * AStarSearchMap.H_FAC;
        }
        //------------------------------------------------------
        void SetFn(SearchMap node)
        {
            node.f = node.g + node.h;
        }
        //------------------------------------------------------
        SearchMap GetBestOpenNode()
        {
            if (null == m_pOpenList) return null;
            SearchMap node = m_pOpenList;
            m_pOpenList = m_pOpenList.next;
            node.next = null;
            return node;
        }
        //------------------------------------------------------
        SearchMap GetFormList(TerrainGridMap point, EAStarLisType listType)
        {
            SearchMap listNode = null;
            if (EAStarLisType.Open == listType) listNode = m_pOpenList;
            else if (EAStarLisType.Close == listType) listNode = m_pCloseList;

            while (null != listNode)
            {
                if (listNode.gridMap == point)
                    return listNode;
                listNode = listNode.next;
            }

            return listNode;
        }
        //------------------------------------------------------
        bool IsSearchFinished(SearchMap node)
        {
            if (node != null && node.gridMap == m_pEndGridMap)
            {
                AddToCloseList(node);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        bool IsBlocked(TerrainGridMap gridMap)
        {
            //             if (null == m_pGridMap) return false;
            //             GridNode gridNode = m_pGridMap.GetGrid(point);
            //             if (gridNode.IsValid) return gridNode.IsState(m_nBlockFlags);
            //             return false;
            return false;
        }
        //------------------------------------------------------
        bool IsGridBlocked(TerrainGridMap gridMap, Vector3Int point)
        {
            if (null == gridMap) return false;
            GridNode gridNode = gridMap.GetGridByWorld(point);
            if (gridNode.IsValid) return gridNode.IsState(m_nBlockFlags);
            return false;
        }
        //------------------------------------------------------
        void RemoveFormOpenList(SearchMap node)
        {
            SearchMap preNode = null;
            SearchMap listNode = m_pOpenList;
            while (null != listNode)
            {
                if (listNode.gridMap == node.gridMap)
                {
                    if (null == preNode)
                    {
                        m_pOpenList = m_pOpenList.next;
                        m_pOpenList.next = null;
                        break;
                    }
                    preNode.next = listNode.next;
                    listNode.next = null;
                    break;
                }
                preNode = listNode;
                listNode = listNode.next;
            }
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
            m_pStartGridMap = null;
            m_pEndGridMap = null;
            m_pUserData = null;
            m_pUserData1 = null;
            m_onCallback = null;
            ClearStatus();
        }
    }
    //------------------------------------------------------
    //! AStarSearchMap
    //------------------------------------------------------
    internal struct AStarSearchMap
    {
        Vector3 m_vStart;
        Vector3 m_vEnd;

        Vector3Int m_vMapGridStart;
        Vector3Int m_vMapGridEnd;

        SearchNode m_pOpenList;
        SearchNode m_pCloseList;
        uint m_nBlockFlags;
        ESearchDir m_eSearchDir;
        int m_IfEndBlockFindNearlyUnBlockExtern;

        TerrainGridMap m_pGridMap;
        AStarSearch.EStatus m_Status;

        public static uint H_FAC = 10;
        public static uint G_FORWARD_FAC = 10;
        public static uint G_CROSS_FAC = 14;
        public static Vector3Int[] ms_EightOff =
        {
            new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, -1), new Vector3Int(-1, 0, 1)
        };
        public static Vector3Int[] ms_FourOff =
        {
            new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1), new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        };
        //------------------------------------------------------
        public void SetStart(Vector3 point)
        {
            m_vStart = point;
        }
        //------------------------------------------------------
        public void SetEnd(Vector3 point)
        {
            m_vEnd = point;
        }
        //------------------------------------------------------
        public void SetBlockFlags(uint flags)
        {
            m_nBlockFlags = flags;
        }
        //------------------------------------------------------
        public void SetEndBlockFindNearlyExtern(int nExtern)
        {
            m_IfEndBlockFindNearlyUnBlockExtern = nExtern;
        }
        //------------------------------------------------------
        public void SetSearchDir(ESearchDir searchDir)
        {
            m_eSearchDir = searchDir;
        }
        //------------------------------------------------------
        public void SetGridMap(TerrainGridMap gridMap)
        {
            m_pGridMap = gridMap;
        }
        //------------------------------------------------------
        public TerrainGridMap GetMap()
        {
            return m_pGridMap;
        }
        //------------------------------------------------------
        public bool IsValid()
        {
            return m_pGridMap != null;
        }
        //------------------------------------------------------
        public AStarSearch.EStatus GetStatus()
        {
            return m_Status;
        }
        //------------------------------------------------------
        public void StartSearch()
        {
            if(m_pGridMap == null)
            {
                m_Status = AStarSearch.EStatus.NoReach;
                return;
            }
            m_Status = AStarSearch.EStatus.None;
            if (BaseUtil.Equal(m_vStart, m_vEnd))
            {
                m_Status = AStarSearch.EStatus.Finished;
                return;
            }
            Vector3Int worldStartGrid = TerrainLayers.WorldPosToWorldGrid3D(m_vStart);
            Vector3Int worldEndGrid = TerrainLayers.WorldPosToWorldGrid3D(m_vEnd);
            if (worldStartGrid == worldEndGrid)
            {
                m_Status = AStarSearch.EStatus.Finished;
                return;
            }
            m_vMapGridStart = m_pGridMap.WorldGridToMapGrid3D(worldStartGrid);
            m_vMapGridEnd = m_pGridMap.WorldGridToMapGrid3D(worldEndGrid);

            Vector3Int[] offDirs = null;
            if (m_eSearchDir == ESearchDir.FourDir) offDirs = ms_FourOff;
            else offDirs = ms_EightOff;
            int dirLen = offDirs.Length;

            if (IsBlocked(m_vMapGridStart))
            {
                if (m_IfEndBlockFindNearlyUnBlockExtern > 0)
                {
                    int nearyDis = int.MaxValue;
                    Vector3Int srcEnd = m_vMapGridStart;
                    for (int d = 1; d <= m_IfEndBlockFindNearlyUnBlockExtern; ++d)
                    {
                        for (int i = 0; i < dirLen; ++i)
                        {
                            Vector3Int temp = srcEnd + offDirs[i] * d;
                            if (!IsBlocked(temp))
                            {
                                if ((temp - m_vMapGridEnd).sqrMagnitude <= nearyDis)
                                {
                                    m_vMapGridStart = temp;
                                    nearyDis = (temp - m_vMapGridEnd).sqrMagnitude;
                                }
                            }
                        }
                    }
                }
            }

            if (IsBlocked(m_vMapGridEnd))
            {
                if (m_IfEndBlockFindNearlyUnBlockExtern > 0)
                {
                    int nearyDis = int.MaxValue;
                    Vector3Int srcEnd = m_vMapGridEnd;
                    for (int d = 1; d <= m_IfEndBlockFindNearlyUnBlockExtern; ++d)
                    {
                        for (int i = 0; i < dirLen; ++i)
                        {
                            Vector3Int temp = srcEnd + offDirs[i] * d;
                            if (!IsBlocked(temp))
                            {
                                if ((temp - m_vMapGridStart).sqrMagnitude <= nearyDis)
                                {
                                    m_vMapGridEnd = temp;
                                    nearyDis = (temp - m_vMapGridStart).sqrMagnitude;
                                }
                            }
                        }
                    }
                    if (IsBlocked(m_vMapGridEnd))
                    {
                        m_Status = AStarSearch.EStatus.NoReach;
                        return;
                    }
                }
                else
                {
                    m_Status = AStarSearch.EStatus.NoReach;
                    return;
                }
            }
            if (m_vMapGridStart == m_vMapGridEnd)
            {
                m_Status = AStarSearch.EStatus.Finished;
                return;
            }

            ClearStatus();
            SearchNode bestNode = SearchCatch.MallocNode(m_pGridMap);
            bestNode.g = 0;
            SetHn(bestNode);
            SetFn(bestNode);
            bestNode.parent = null;
            bestNode.next = null;
            bestNode.pos = m_vMapGridStart;

            int strackLoop = 1000;
            SearchNode nearlyNode = null;
            while (!IsSearchFinished(bestNode))
            {
                for (int i = 0; i < dirLen; ++i)
                {
                    nearlyNode = SearchCatch.MallocNode(m_pGridMap);
                    nearlyNode.parent = bestNode;
                    nearlyNode.next = null;
                    nearlyNode.pos = bestNode.pos + offDirs[i];
                    ValidAddToOpenList(nearlyNode);
                }
                AddToCloseList(bestNode);
                bestNode = GetBestOpenNode();
                if (bestNode == null) break;
                strackLoop--;
                if (strackLoop <= 0) break;
            }
            if (strackLoop <= 0) m_Status = AStarSearch.EStatus.FinishedNoReach;
            else m_Status = AStarSearch.EStatus.Finished;
        }

        //------------------------------------------------------
        public void GetPathList(ref List<RunPoint> path, bool bCheckStartEnd)
        {
            if (path == null || m_pGridMap == null) return;
            SearchNode pNode = GetFormList(m_vMapGridEnd, EAStarLisType.Close);
            while (pNode != null)
            {
                RunPoint point = new RunPoint(TerrainLayers.PathWorldGridToWorldPos(pNode.gridMap.MapGridToWorldGrid(pNode.pos)));
                path.Insert(0, point);
                pNode = pNode.parent;
            }
            if (bCheckStartEnd && path.Count > 0)
            {
                if (!BaseUtil.Equal(m_vStart, path[0].position))
                {
                    path.Insert(0, new RunPoint(m_vStart));
                }
                if (!BaseUtil.Equal(m_vEnd, path[path.Count - 1].position))
                {
                    path.Add(new RunPoint(m_vEnd));
                }
            }
        }
        //------------------------------------------------------
        public void GetPathList(ref List<Vector3> path, bool bCheckStartEnd)
        {
            if (path == null || m_pGridMap == null) return;
            SearchNode pNode = GetFormList(m_vMapGridEnd, EAStarLisType.Close);
            while (pNode != null)
            {
                path.Insert(0, TerrainLayers.PathWorldGridToWorldPos(pNode.gridMap.MapGridToWorldGrid(pNode.pos)));
                pNode = pNode.parent;
            }
            if (bCheckStartEnd && path.Count > 0)
            {
                if (!BaseUtil.Equal(m_vStart, path[0]))
                {
                    path.Insert(0, m_vStart);
                }
                if (!BaseUtil.Equal(m_vEnd, path[path.Count - 1]))
                {
                    path.Add(m_vEnd);
                }
            }
        }
        //------------------------------------------------------
        public void GetGridPathList(ref List<Vector3Int> path, bool bCheckStartEnd)
        {
            if (path == null || m_pGridMap == null) return;
            SearchNode pNode = GetFormList(m_vMapGridEnd, EAStarLisType.Close);
            while (pNode != null)
            {
                path.Insert(0, pNode.gridMap.MapGridToWorldGrid(pNode.pos));
                pNode = pNode.parent;
            }
            if (bCheckStartEnd && path.Count > 0)
            {
                if (!BaseUtil.Equal(m_vMapGridStart, path[0]))
                {
                    path.Insert(0, m_pGridMap.MapGridToWorldGrid(m_vMapGridStart));
                }
                if (!BaseUtil.Equal(m_vMapGridEnd, path[path.Count - 1]))
                {
                    path.Add(m_pGridMap.MapGridToWorldGrid(m_vMapGridEnd));
                }
            }
        }
        //------------------------------------------------------
        internal void ClearStatus()
        {
            SearchNode node;
            while (null != m_pOpenList)
            {
                node = m_pOpenList;
                m_pOpenList = m_pOpenList.next;
                node.Destroy();
                SearchCatch.FreeNode(node);
            }

            while (null != m_pCloseList)
            {
                node = m_pCloseList;
                m_pCloseList = m_pCloseList.next;
                node.Destroy();
                SearchCatch.FreeNode(node);
            }
            m_pCloseList = null;
            m_pOpenList = null;
        }
        //------------------------------------------------------
        void AddToCloseList(SearchNode node)
        {
            if (node == null)
                return;
            if (null == m_pCloseList)
            {
                m_pCloseList = node;
                m_pCloseList.next = null;
                return;
            }
            SearchNode pNode = m_pCloseList;
            m_pCloseList = node;
            m_pCloseList.next = pNode;
        }
        //------------------------------------------------------
        void AddToOpenList(SearchNode node)
        {
            if (null == m_pOpenList)
            {
                m_pOpenList = node;
                m_pOpenList.next = null;
                return;
            }

            SearchNode preNode = null;
            SearchNode listNode = m_pOpenList;
            while (null != listNode)
            {
                if (listNode.f >= node.f)
                {
                    if (null == preNode)
                    {
                        node.next = listNode;
                        m_pOpenList = node;
                        break;
                    }
                    preNode.next = node;
                    node.next = listNode;
                    break;
                }
                preNode = listNode;
                listNode = listNode.next;
            }
            if (null == listNode)
            {
                preNode.next = node;
                node.next = null;
            }
        }
        //------------------------------------------------------
        void ValidAddToOpenList(SearchNode node)
        {
            if (IsBlocked(node.pos))
            {
                SearchCatch.FreeNode(node);
                return;
            }
            SetGn(node);
            SetHn(node);
            SetFn(node);

            SearchNode cachNode = GetFormList(node.pos, EAStarLisType.Close);
            if (cachNode != null)
            {
                if (cachNode.g > node.g)
                {
                    cachNode.g = node.g;
                    cachNode.f = node.f;
                    cachNode.h = node.h;
                    cachNode.parent = node.parent;
                }
                SearchCatch.FreeNode(node);
                return;
            }
            cachNode = GetFormList(node.pos, EAStarLisType.Open);
            if (cachNode != null)
            {
                if (cachNode.g > node.g)
                {
                    cachNode.g = node.g;
                    cachNode.f = node.f;
                    cachNode.h = node.h;
                    cachNode.parent = node.parent;

                    RemoveFormOpenList(cachNode);
                    AddToOpenList(cachNode);
                }
                SearchCatch.FreeNode(node);
                return;
            }
            AddToOpenList(node);
        }
        //------------------------------------------------------
        void SetGn(SearchNode node)
        {
            if (node.pos != node.parent.pos)
            {
                node.g = G_CROSS_FAC + node.parent.g;
            }
            else
                node.g = G_FORWARD_FAC + node.parent.g;
        }
        //------------------------------------------------------
        void SetHn(SearchNode node)
        {
            node.h = (uint)(Mathf.Abs(node.pos.x - m_vMapGridEnd.x) + Mathf.Abs(node.pos.z - m_vMapGridEnd.z) + Mathf.Abs(node.pos.y - m_vMapGridEnd.y)) * H_FAC;
        }
        //------------------------------------------------------
        void SetFn(SearchNode node)
        {
            node.f = node.g + node.h;
        }
        //------------------------------------------------------
        SearchNode GetBestOpenNode()
        {
            if (null == m_pOpenList) return null;
            SearchNode node = m_pOpenList;
            m_pOpenList = m_pOpenList.next;
            node.next = null;
            return node;
        }
        //------------------------------------------------------
        SearchNode GetFormList(Vector3Int point, EAStarLisType listType)
        {
            SearchNode listNode = null;
            if (EAStarLisType.Open == listType) listNode = m_pOpenList;
            else if (EAStarLisType.Close == listType) listNode = m_pCloseList;

            while (null != listNode)
            {
                if (listNode.pos == point)
                    return listNode;
                listNode = listNode.next;
            }

            return listNode;
        }
        //------------------------------------------------------
        bool IsSearchFinished(SearchNode node)
        {
            if (node != null && node.pos == m_vMapGridEnd)
            {
                AddToCloseList(node);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        bool IsBlocked(Vector3Int point)
        {
            if (null == m_pGridMap ) return false;
            GridNode gridNode = m_pGridMap.GetGrid(point);
            if (gridNode.IsValid) return gridNode.IsState(m_nBlockFlags);
            return false;
        }
        //------------------------------------------------------
        void RemoveFormOpenList(SearchNode node)
        {
            SearchNode preNode = null;
            SearchNode listNode = m_pOpenList;
            while (null != listNode)
            {
                if (listNode.pos == node.pos)
                {
                    if (null == preNode)
                    {
                        m_pOpenList = m_pOpenList.next;
                        m_pOpenList.next = null;
                        break;
                    }
                    preNode.next = listNode.next;
                    listNode.next = null;
                    break;
                }
                preNode = listNode;
                listNode = listNode.next;
            }
        }
        //------------------------------------------------------
        public void Destroy()
        {
            m_pGridMap = null;
            ClearStatus();
        }
    }
}

