/********************************************************************
生成日期:	2020-6-16
类    名: 	TerrainGridMap
作    者:	Happli
描    述:	格子地表
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework.Base;
namespace Framework.Core
{
    public struct TerrainGridLinkOffline
    {
        public Vector3Int lineStart;
        public Vector3Int lineEnd;
        public Vector3 lineStartTan;
        public Vector3 lineEndTan;
        public int lineWidth;
        public Core.EActionStateType startAction;
        public Core.EActionStateType endAction;
        public int speed;
        internal Vector3 expandDir;
        public bool IsValid()
        {
            return (lineEnd - lineStart).sqrMagnitude > 0;
        }
        public bool IsCurveLine()
        {
            return lineStartTan.sqrMagnitude > 0 || lineEndTan.sqrMagnitude > 0;
        }
        public Vector3 Sample(float t)
        {
            Vector3 worldStart = TerrainLayers.PathWorldGridToWorldPos(lineStart);
            Vector3 worldStartPt = worldStart;
            Vector3 worldEnd = TerrainLayers.PathWorldGridToWorldPos(lineEnd);
            return BezierUtility.Bezier4(t, worldStart, worldStart + lineStartTan, worldEnd + lineEndTan, worldEnd);
        }
        public Vector3 GetLineStartFromWidth(int offset)
        {
            Vector3 worldStart = TerrainLayers.WorldGridToWorldPos(lineStart);
            if (lineWidth <= 0) return worldStart;
            return worldStart + expandDir * TerrainLayers.GridPosYToWorldPosY(Mathf.Clamp(offset, 0, lineWidth));
        }
        public Vector3 GetLineEndFromWidth(int offset)
        {
            Vector3 worldEnd = TerrainLayers.WorldGridToWorldPos(lineEnd);
            if (lineWidth <= 0) return worldEnd;
            Vector3 dir = lineEnd - lineStart;
            if (dir.sqrMagnitude <= 0) return worldEnd;
            Vector3 right = Quaternion.LookRotation(dir.normalized, Vector3.up) * Vector3.right;
            return worldEnd + expandDir * TerrainLayers.GridPosYToWorldPosY(Mathf.Clamp(offset, 0, lineWidth));
        }
        public Vector3Int GetLineGridStartFromWidth(int offset)
        {
            return TerrainLayers.WorldPosToWorldGrid3D(GetLineStartFromWidth(offset));
        }
        public Vector3Int GetLineGridEndFromWidth(int offset)
        {
            return TerrainLayers.WorldPosToWorldGrid3D(GetLineEndFromWidth(offset));
        }
        public static TerrainGridLinkOffline DEF = new TerrainGridLinkOffline() { lineStart = Vector3Int.zero, lineEnd = Vector3Int.zero };
    }
    public class TerrainGridMap : IDisposable
    {
        struct StoreState
        {
            public Vector2Int worldGrid;
            public GridNode node;
        }
        private Vector2Int m_StandMax = Vector2Int.zero;
        private Vector2Int m_StandMin = Vector2Int.zero;
        private Vector2Int m_StandCellSize  = Vector2Int.zero;

        private int         m_nID = -1;

        private int         m_PositionY = 0;
        private Vector2Int m_Max = Vector2Int.zero;
        private Vector2Int m_Min = Vector2Int.zero;
        private Vector2Int m_Size = Vector2Int.zero;
        private Vector2Int m_CellSize = Vector2Int.zero;

        private List<StoreState> m_vBackupStoreStates = null;
        private List<StoreState> m_vStoreStates = null;
        //   private Unity.Collections.NativeArray<GridNode> m_vGrids;
        private GridNode[] m_vGrids;
        public TerrainGridMap(int nID, Vector2Int max, Vector2Int min, Vector2Int cellSize)
        {
            m_nID = nID;
            m_StandMin = min;
            m_StandMax = max;
            m_StandCellSize = cellSize;
            m_vGrids = null;
        }
        //------------------------------------------------------
        ~TerrainGridMap()
        {
            Destroy();
        }
        //------------------------------------------------------
        public void Destroy()
        {
            //if (m_vGrids.IsCreated) m_vGrids.Dispose();
            m_vGrids = null;
            m_nID = -1;
        }
        //------------------------------------------------------
        public int GetID()
        {
            return m_nID;
        }
        //------------------------------------------------------
        public Vector2Int GetCellSize()
        {
            return m_CellSize;
        }
        //------------------------------------------------------
        public GridNode[] GetGrids()
        {
            return m_vGrids;
        }
        ////------------------------------------------------------
        //public Unity.Collections.NativeArray<GridNode> GetGrids()
        //{
        //    return m_vGrids;
        //}
        //------------------------------------------------------
        public int GetPositionY()
        {
            return m_PositionY;
        }
        //------------------------------------------------------
        public void SetPositionY(int posY)
        {
            m_PositionY = posY;
        }
        //------------------------------------------------------
        public Vector3Int GetCenter()
        {
            return new Vector3Int((m_Max.x - m_Min.x)/2, m_PositionY, (m_Max.y - m_Min.y)/2);
        }
        //------------------------------------------------------
        public Vector3Int GetWorldCenter()
        {
            return new Vector3Int((m_Max.x - m_Min.x) / 2 + m_Min.x, m_PositionY, (m_Max.y - m_Min.y) / 2 + m_Min.y);
        }
        //------------------------------------------------------
        internal bool GetProjectionClosely(Vector3Int worldGrid, out int projectDist)
        {
            projectDist = 0;
            if (IsContain(new Vector2Int(worldGrid.x, worldGrid.z)))
            {
                projectDist = Mathf.Abs(worldGrid.y - m_PositionY);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool SampleHeight(Vector3Int worldGrid, out int height)
        {
            height = 0;
            if (IsContain(new Vector2Int(worldGrid.x, worldGrid.z)))
            {
                height = m_PositionY;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsContain(Vector3Int worldGrid)
        {
            if (m_StandMin.x <= worldGrid.x && worldGrid.x <= m_StandMax.x &&
               m_StandMin.y <= worldGrid.z && worldGrid.z <= m_StandMax.y &&
               m_PositionY == worldGrid.y)
                return true;
            return false;
        }
        //------------------------------------------------------
        public bool IsContain(Vector2Int worldGrid)
        {
            if (m_StandMin.x <= worldGrid.x && worldGrid.x <= m_StandMax.x &&
               m_StandMin.y <= worldGrid.y && worldGrid.y <= m_StandMax.y)
                return true;
            return false;
        }
        //------------------------------------------------------
        public bool Extern(Vector3Int vMin, Vector3Int vMax, bool bReBuild, bool bKeepOldStates = true)
        {
            if (vMin.y != m_PositionY) return false;
            if (vMin.x <= m_StandMin.x && m_StandMin.x <= vMax.x &&
                vMin.z <= m_StandMin.y && m_StandMin.y <= vMax.z &&
                vMin.x <= m_StandMax.x && m_StandMax.x <= vMax.x &&
                vMin.z <= m_StandMax.y && m_StandMax.y <= vMax.z)
            {
                m_StandMin.x = vMin.x;
                m_StandMin.y = vMin.z;

                m_StandMax.x = vMax.x;
                m_StandMax.y = vMax.z;
                m_PositionY = vMin.y;

                if(bReBuild) Build(true, bKeepOldStates);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void AddStoreGridState(Vector3Int worldGrid, int stateFlag)
        {
            if (m_vStoreStates == null) m_vStoreStates = new List<StoreState>();
            StoreState storeNode = new StoreState();
            storeNode.worldGrid = new Vector2Int(worldGrid.x, worldGrid.z);
            storeNode.node = new GridNode() { gridState = stateFlag, posY = worldGrid.y };
            m_vStoreStates.Add(storeNode);
        }
        //------------------------------------------------------
        public void Build(bool bReBuild = false, bool bKeepOldStates = true)
        {
            if(bReBuild || m_vGrids == null)
            {
                BuildGrid(m_StandMax, m_StandMin, m_StandCellSize, bKeepOldStates);
            }
        }
        //------------------------------------------------------
        public void BuildGrid(Vector2Int max, Vector2Int min, Vector2Int cellSize, bool bKeepOldStates = true)
        {
            Vector2Int size = max - min;
            if (size.x <= 0 && size.y <= 0)
                return;
            if (cellSize.x == 0 || cellSize.y == 0)
                return;

            m_StandMin = min;
            m_StandMax = max;
            m_StandCellSize = cellSize;

            if (m_Max == min && m_Max == max && m_CellSize == size)
                return;

            if(bKeepOldStates && m_vGrids != null)
            {
                if (m_vBackupStoreStates == null) m_vBackupStoreStates = new List<StoreState>();
                 for (int i = 0; i < m_vGrids.Length; ++i)
                 {
                     if (!m_vGrids[i].IsState(EGridState.Wakeable) && m_vGrids[i].gridState != 0)
                     {
                        StoreState storeNode = new StoreState();
                        storeNode.worldGrid = MapGridToWorldGrid2D(IndexToMapGrid(i));
                        storeNode.node = m_vGrids[i];
                        m_vBackupStoreStates.Add(storeNode);
                     }
                 }
            }

            m_CellSize = cellSize;
            m_Min = min;
            m_Max = max;
            Vector2Int old = m_Size;
            m_Size.x = size.x / cellSize.x;
            m_Size.y = size.y / cellSize.y;

            //             Unity.Collections.NativeArray<GridNode> vTemps = new Unity.Collections.NativeArray<GridNode>(m_vGrids.Length, Unity.Collections.Allocator.TempJob);
            //             if (m_vGrids.IsCreated && m_vGrids.Length > 0)
            //                 Unity.Collections.NativeArray<GridNode>.Copy(m_vGrids, vTemps);
            //             m_vGrids = new Unity.Collections.NativeArray<GridNode>(m_Size.x* m_Size.y, Unity.Collections.Allocator.Persistent);

            int lenth = m_Size.x * m_Size.y;
            m_vGrids = new GridNode[lenth];
            for (int x = 0; x < m_Size.x; ++x)
            {
                for(int z = 0; z < m_Size.y; ++z)
                {
                    int index = z * m_Size.x + x;
                    m_vGrids[index] = new GridNode() {  gridState = 1<<(int)EGridState.Wakeable };
                }
            }
            if(m_vBackupStoreStates!=null)
            {
                for (int i = 0; i < m_vBackupStoreStates.Count; ++i)
                {
                    SetGridStateByWorld(m_vBackupStoreStates[i].worldGrid, m_vBackupStoreStates[i].node.gridState);
                }
                m_vBackupStoreStates.Clear();
            }
            if(m_vStoreStates != null)
            {
                for(int i =0; i < m_vStoreStates.Count; ++i)
                {
                    SetGridStateByWorld(m_vStoreStates[i].worldGrid, m_vStoreStates[i].node.gridState);
                }
                m_vStoreStates.Clear();
            }
      //      vTemps.Dispose();
        }
        //------------------------------------------------------
        public void SetGridStateByWorld(Vector3Int worldPoint, EGridState state)
        {
            if (!IsContain(worldPoint)) return;
            SetGridState(WorldGridToMapGrid(worldPoint), state);
        }
        //------------------------------------------------------
        public void SetGridStateByWorld(Vector3Int worldPoint, int stateFlag)
        {
            if (!IsContain(worldPoint)) return;
            SetGridState(WorldGridToMapGrid(worldPoint), stateFlag);
        }
        //------------------------------------------------------
        public void SetGridStateByWorld(Vector2Int wirldPoint, EGridState state)
        {
            if (!IsContain(wirldPoint)) return;
            SetGridState(WorldGridToMapGrid(wirldPoint), state);
        }
        //------------------------------------------------------
        public void SetGridStateByWorld(Vector2Int wirldPoint, int stateFlag)
        {
            if (!IsContain(wirldPoint)) return;
            SetGridState(WorldGridToMapGrid(wirldPoint), stateFlag);
        }
        //------------------------------------------------------
        public void SetGridState(Vector2Int mapGrid, EGridState state)
        {
            SetGridState(mapGrid, 1<<(int)state);
        }
        //------------------------------------------------------
        public void SetGridState(Vector2Int mapGrid, int stateFlag)
        {
            int index = MapGridToIndex(mapGrid);
            if (index < 0 || m_vGrids == null || index >= m_vGrids.Length) return;
            GridNode grid = m_vGrids[index];
            grid.gridState = stateFlag;
            m_vGrids[index] = grid;
        }
        //------------------------------------------------------
        public GridNode GetGridByWorld(Vector2Int worldGrid)
        {
            return GetGrid(WorldGridToMapGrid(worldGrid));
        }
        //------------------------------------------------------
        public GridNode GetGridByWorld(Vector3Int worldGrid)
        {
            if (worldGrid.y != m_PositionY) return GridNode.DEF;
            return GetGrid(WorldGridToMapGrid(worldGrid));
        }
        //------------------------------------------------------
        public GridNode GetGrid(Vector2Int point)
        {
            int index = MapGridToIndex(point);
            if (index < 0 || m_vGrids == null || index >= m_vGrids.Length) return GridNode.DEF;
            return m_vGrids[index];
        }
        //------------------------------------------------------
        public GridNode GetGrid(Vector3Int point)
        {
            int index = MapGridToIndex(new Vector2Int(point.x, point.z));
            if (index < 0 || m_vGrids == null || index >= m_vGrids.Length) return GridNode.DEF;
            return m_vGrids[index];
        }
        //------------------------------------------------------
        public bool CheckGridByWorld(Vector2Int worldGrid, uint gridState)
        {
            GridNode grid = GetGrid(WorldGridToMapGrid(worldGrid));
            if (grid.IsValid && grid.IsState(gridState)) return true;
            return false;
        }
        //------------------------------------------------------
        public bool CheckGridByWorld(Vector3Int worldGrid, uint gridState)
        {
            if (worldGrid.y != m_PositionY) return false;
            GridNode grid = GetGrid(WorldGridToMapGrid(worldGrid));
            if (grid.IsValid && grid.IsState(gridState)) return true;
            return false;
        }
        //------------------------------------------------------
        public bool CheckRegionGrid(Vector3Int vMin, Vector3Int vMax, uint gridState, HashSet<Vector3Int> vIngoreGrids = null)
        {
            if(IsContain(vMin) || IsContain(vMax))
            {
                Vector2Int mapMin = WorldGridToMapGrid(vMin);
                Vector2Int mapMax = WorldGridToMapGrid(vMax);
                Vector2Int pt = Vector2Int.zero;
                for (int x = mapMin.x; x < mapMax.x; ++x)
                {
                    for (int z = mapMin.y; z < mapMax.y; ++z)
                    {
                        pt.x = x;
                        pt.y = z;
                        if (vIngoreGrids != null && vIngoreGrids.Count > 0 && vIngoreGrids.Contains(MapGridToWorldGrid(pt)))
                            continue;
                        GridNode grid = GetGrid(pt);
                        if (grid.IsValid && grid.IsState(gridState)) return true;
                    }
                }
            }
            
            return false;
        }
        //------------------------------------------------------
        public bool CheckRegionGrid(Vector2Int worldGrid, Vector2Int worldSize, uint gridState, HashSet<Vector3Int> vIngoreGrids = null)
        {
            if (IsContain(worldGrid) || IsContain(worldGrid + worldSize))
            {
                worldGrid = WorldGridToMapGrid(worldGrid);
                worldSize = WorldGridToMapGrid(worldSize);
                Vector2Int pt = Vector2Int.zero;
                for (int x = worldGrid.x; x < worldGrid.x + worldSize.x; ++x)
                {
                    for (int z = worldGrid.y; z < worldGrid.y + worldSize.y; ++z)
                    {
                        pt.x = x;
                        pt.y = z;
                        if (vIngoreGrids != null && vIngoreGrids.Count>0 && vIngoreGrids.Contains(MapGridToWorldGrid(pt)))
                            continue;
                        GridNode grid = GetGrid(pt);
                        if (grid.IsValid && grid.IsState(gridState)) return true;
                    }
                }
            }

            return false;
        }
        //------------------------------------------------------
        public void SetRegionGridState(Vector3Int worldGrid, Vector2Int worldSize, EGridState gridState)
        {
            if (worldGrid.y != m_PositionY) return;
            SetRegionGridState(new Vector2Int(worldGrid.x, worldGrid.z), worldSize, gridState);
        }
        //------------------------------------------------------
        public void SetRegionGridState(Vector2Int worldGrid, Vector2Int worldSize, EGridState gridState)
        {
            if (worldSize.x <= 0 || worldSize.y <= 0) return;
            if(IsContain(worldGrid) || IsContain(worldGrid + worldSize))
            {
                worldGrid = WorldGridToMapGrid(worldGrid);
                Vector2Int pt = Vector2Int.zero;
                for (int x = worldGrid.x; x < worldGrid.x + worldSize.x; ++x)
                {
                    for (int z = worldGrid.y; z < worldGrid.y + worldSize.y; ++z)
                    {
                        pt.x = x;
                        pt.y = z;
                        SetGridState(pt, gridState);
                    }
                }
            }
        }
        //------------------------------------------------------
        public Vector3 MapGridToWorldPos(Vector2Int mapGrid)
        {
            Vector3Int worldGrid = MapGridToWorldGrid(mapGrid);
            return TerrainLayers.WorldGridToWorldPos(worldGrid);
        }
        //------------------------------------------------------
        public Vector2Int WorldPosToMapGrid(Vector3 worldPos)
        {
            return WorldGridToMapGrid(TerrainLayers.WorldPosToWorldGrid(worldPos));
        }
        //------------------------------------------------------
        public Vector2Int WorldGridToMapGrid(Vector2Int worldGrid)
        {
            worldGrid -= m_Min;
            if (m_CellSize.x != 0) worldGrid.x = worldGrid.x / m_CellSize.x;
            if (m_CellSize.y != 0) worldGrid.y = worldGrid.y / m_CellSize.y;
            return worldGrid;
        }
        //------------------------------------------------------
        public Vector2Int WorldGridToMapGrid(Vector3Int worldGrid)
        {
            worldGrid.x -= m_Min.x;
            worldGrid.z -= m_Min.y;
            if (m_CellSize.x != 0) worldGrid.x = worldGrid.x / m_CellSize.x;
            if (m_CellSize.y != 0) worldGrid.z = worldGrid.z / m_CellSize.y;
            return new Vector2Int(worldGrid.x, worldGrid.z);
        }
        //------------------------------------------------------
        public Vector3Int WorldGridToMapGrid3D(Vector3Int worldGrid)
        {
            worldGrid.x -= m_Min.x;
            worldGrid.z -= m_Min.y;
            if (m_CellSize.x != 0) worldGrid.x = worldGrid.x / m_CellSize.x;
            if (m_CellSize.y != 0) worldGrid.z = worldGrid.z / m_CellSize.y;
            return new Vector3Int(worldGrid.x, m_PositionY, worldGrid.z);
        }
        //------------------------------------------------------
        public Vector3Int MapGridToWorldGrid(Vector2Int mapGrid)
        {
            Vector3Int worldGrid = Vector3Int.zero;
            worldGrid.x = mapGrid.x * m_CellSize.x + m_Min.x;
            worldGrid.z = mapGrid.y * m_CellSize.y + m_Min.y;
            worldGrid.y = m_PositionY;

            GridNode gridNode = GetGrid(mapGrid);
            if (gridNode.IsValid) worldGrid.y += gridNode.posY;
            return worldGrid;
        }
        //------------------------------------------------------
        public Vector3Int MapGridToWorldGrid(Vector3Int mapGrid)
        {
            Vector3Int worldGrid = Vector3Int.zero;
            worldGrid.x = mapGrid.x * m_CellSize.x + m_Min.x;
            worldGrid.z = mapGrid.z * m_CellSize.y + m_Min.y;
            worldGrid.y = m_PositionY;

            GridNode gridNode = GetGrid(mapGrid);
            if (gridNode.IsValid) worldGrid.y += gridNode.posY;
            return worldGrid;
        }
        //------------------------------------------------------
        public Vector2Int MapGridToWorldGrid2D(Vector2Int mapGrid)
        {
            Vector2Int worldGrid = Vector2Int.zero;
            worldGrid.x = mapGrid.x * m_CellSize.x + m_Min.x;
            worldGrid.y = mapGrid.y * m_CellSize.y + m_Min.y;
            return worldGrid;
        }
        //------------------------------------------------------
        public Vector2Int IndexToMapGrid(int index)
        {
            return new Vector2Int(index%m_Size.x, index/m_Size.x);
        }
        //------------------------------------------------------
        public int MapGridToIndex(Vector2Int grid)
        {
            return grid.y * m_Size.x + grid.x;
        }
        //------------------------------------------------------
        public int MapGridToIndex(Vector3Int grid)
        {
            return grid.z * m_Size.x + grid.x;
        }
        //------------------------------------------------------
        public void Update()
        {
        }
        //------------------------------------------------------
        public void Dispose()
        {
            Destroy();
        }
    }
}

