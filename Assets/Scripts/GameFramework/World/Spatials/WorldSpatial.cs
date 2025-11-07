/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SpatialNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FVector3 = UnityEngine.Vector3;
#endif

namespace Framework.Core
{
    public class WorldSpatial
    {
        static int[] Neighbor_Indexs = new int[26];//8+9+9
        World m_pWorld;
        SpatialCell[] m_Cells = null;
        int m_nSaptialCount = 0;
        int m_nWidthNum =0;
        int m_nDepthNum = 0;
        int m_nHeightNum = 0;
        int m_nCellDepth = 0;
        int m_nCellWidth = 0;
        int m_nCellHeight = 0;

        Vector3Int m_StartPos = Vector3Int.zero;
        //------------------------------------------------------
        public WorldSpatial(World world, int col, int depth, int height, int cellWidth, int cellDepth, int cellHeight)
        {
            m_pWorld = world;
            m_nWidthNum = col;
            m_nDepthNum = depth;
            m_nHeightNum = height;
            m_nCellWidth = cellWidth;
            m_nCellDepth = cellDepth;
            m_nCellHeight = cellHeight;
            m_nSaptialCount = m_nWidthNum * depth * height;
            m_Cells = new SpatialCell[m_nSaptialCount];
            m_StartPos = new Vector3Int(-col* cellWidth/2, 0,0);
        }
        //------------------------------------------------------
        public void Clear()
        {
            if (m_Cells == null) return;
            for(int i =0; i < m_Cells.Length; ++i)
                m_Cells[i].Clear();
        }
        //------------------------------------------------------
        public void SetPosition(int x, int y, int z)
        {
            m_StartPos.x = x;
            m_StartPos.y = y;
            m_StartPos.z = z;
        }
        //------------------------------------------------------
        public void SetPositionX(int x)
        {
            m_StartPos.x = x;
        }
        //------------------------------------------------------
        public void SetPositionY(int y)
        {
            m_StartPos.y = y;
        }
        //------------------------------------------------------
        public void SetPositionZ(int z)
        {
            m_StartPos.z = z;
        }
        //------------------------------------------------------
        public int GetCellIndex(Vector3 position)
        {
            int x = ((int)position.x - m_StartPos.x)/ m_nCellWidth;
            int y = ((int)position.y - m_StartPos.y)/ m_nCellHeight;
            int z = ((int)position.z - m_StartPos.z)/m_nCellDepth;
            if (x < 0) return -1;
            if (y < 0) return -1;
            if (z < 0) return -1;
            int index = y * m_nDepthNum * m_nWidthNum + z* m_nWidthNum + x;
            if (index >= m_nSaptialCount) return -1;
            return index;
        }
        //------------------------------------------------------
        public int GetCellNeighborIndexs(Vector3 position, bool b3D = true)
        {
            int x = ((int)position.x - m_StartPos.x) / m_nCellWidth;
            int y = ((int)position.y - m_StartPos.y) / m_nCellHeight;
            int z = ((int)position.z - m_StartPos.z) / m_nCellDepth;
            if (x < 0) return -1;
            if (y < 0) return -1;
            if (z < 0) return -1;
            int index = y * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x;
            if (index >= m_nSaptialCount) return -1;

            Neighbor_Indexs[0] = y * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x-1;
            Neighbor_Indexs[1] = y * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x+1;
            Neighbor_Indexs[2] = y * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x;
            Neighbor_Indexs[3] = y * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x;
            Neighbor_Indexs[4] = y * m_nDepthNum * m_nWidthNum + (z-1) * m_nWidthNum + x - 1;
            Neighbor_Indexs[5] = y * m_nDepthNum * m_nWidthNum + (z+1) * m_nWidthNum + x - 1;
            Neighbor_Indexs[6] = y * m_nDepthNum * m_nWidthNum + (z-1) * m_nWidthNum + x + 1;
            Neighbor_Indexs[7] = y * m_nDepthNum * m_nWidthNum + (z+1) * m_nWidthNum + x + 1;

            if(b3D)
            {
                Neighbor_Indexs[8] = (y + 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x;
                Neighbor_Indexs[9] = (y + 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x - 1;
                Neighbor_Indexs[10] = (y + 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x + 1;
                Neighbor_Indexs[11] = (y + 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x;
                Neighbor_Indexs[12] = (y + 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x;
                Neighbor_Indexs[13] = (y + 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x - 1;
                Neighbor_Indexs[14] = (y + 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x - 1;
                Neighbor_Indexs[15] = (y + 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x + 1;
                Neighbor_Indexs[16] = (y + 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x + 1;

                Neighbor_Indexs[17] = (y - 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x;
                Neighbor_Indexs[18] = (y - 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x - 1;
                Neighbor_Indexs[19] = (y - 1) * m_nDepthNum * m_nWidthNum + z * m_nWidthNum + x + 1;
                Neighbor_Indexs[20] = (y - 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x;
                Neighbor_Indexs[21] = (y - 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x;
                Neighbor_Indexs[22] = (y - 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x - 1;
                Neighbor_Indexs[23] = (y - 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x - 1;
                Neighbor_Indexs[24] = (y - 1) * m_nDepthNum * m_nWidthNum + (z - 1) * m_nWidthNum + x + 1;
                Neighbor_Indexs[25] = (y - 1) * m_nDepthNum * m_nWidthNum + (z + 1) * m_nWidthNum + x + 1;
            }

            return index;
        }
        //------------------------------------------------------
        public int GetCellNeighborIndexs(Base.IntersetionParam param, WorldBoundBox bounds)
        {
            int statCnt = 0;
            FVector3[] vDirArray;
            FVector3[] vVertexArray;
            Base.IntersetionUtil.CU_GetBoundPoints(param,bounds, out vDirArray, out vVertexArray);
            for(int i =0; i < vVertexArray.Length; ++i)
            {
                int index = GetCellIndex(vVertexArray[i]);
                if (index >= 0)
                {
                    Neighbor_Indexs[statCnt++] = index;
                }
            }
            return statCnt;
        }
        //------------------------------------------------------
        public void AddNode(AWorldNode pNode)
        {
            int index = GetCellIndex(pNode.GetPosition());
#if UNITY_EDITOR 
            pNode.nSpatialCellIndex = index;
#endif
            if (index < 0 || index >= m_nSaptialCount)
                return;
            m_Cells[index].AddNode(pNode);
        }
        //------------------------------------------------------
        public bool ComputerIntersection(AWorldNode pNode, List<AWorldNode> vNodes)
        {
            if (pNode == null) return false;

            int cnt = GetCellNeighborIndexs(pNode.GetShareFrameParams().intersetionParam, pNode.GetBounds());
            if (cnt <= 0) return false;
            for(int i =0; i < cnt; ++i)
            {
                ComputeNeighbors(Neighbor_Indexs[i], pNode, ref vNodes);
                Neighbor_Indexs[i] = -1;
            }

            return vNodes.Count > 0;
        }
        //------------------------------------------------------
        public void ComputeNeighbors(AWorldNode pNode, float range, ref List<AWorldNode> vNodes, bool b3D = true)
        {
            float rangeSq = range * range;
            int index = GetCellNeighborIndexs(pNode.GetPosition(), b3D);
            if (index < 0) return;
            ComputeNeighbors(index, pNode, rangeSq, ref vNodes);
            for (int i = 0; i < Neighbor_Indexs.Length; ++i)
            {
                ComputeNeighbors(Neighbor_Indexs[i], pNode, rangeSq, ref vNodes);
                Neighbor_Indexs[i] = -1;
            }
        }
        //------------------------------------------------------
        public void ComputeCollisionByRadius(AWorldNode pNode, ref List<AWorldNode> vNodes, float rangeOffset = 0, bool b3D = true)
        {
            int index = GetCellNeighborIndexs(pNode.GetPosition(), b3D);
            if (index < 0) return;
            ComputeCollisionByRadius(index, pNode, ref vNodes, rangeOffset);
            for (int i = 0; i < Neighbor_Indexs.Length; ++i)
            {
                ComputeCollisionByRadius(Neighbor_Indexs[i], pNode, ref vNodes, rangeOffset);
                Neighbor_Indexs[i] = -1;
            }
        }
        //------------------------------------------------------
        void ComputeNeighbors(int cellIndex, Vector3 vPosition, float rangSq, ref List<AWorldNode> vNodes)
        {
            if (cellIndex < 0 || cellIndex >= m_nSaptialCount) return;
            m_Cells[cellIndex].ScanRangeSq(vPosition, rangSq, ref vNodes);
        }
        //------------------------------------------------------
        void ComputeNeighbors(int cellIndex, AWorldNode pNode, float rangSq, ref List<AWorldNode> vNodes)
        {
            if (cellIndex < 0 || cellIndex >= m_nSaptialCount) return;
            m_Cells[cellIndex].ScanRangeSq(pNode, rangSq, ref vNodes);
        }
        //------------------------------------------------------
        void ComputeNeighbors(int cellIndex, AWorldNode pNode, ref List<AWorldNode> vNodes)
        {
            if (cellIndex < 0 || cellIndex >= m_nSaptialCount) return;
            m_Cells[cellIndex].IntersectionBound(pNode, ref vNodes);
        }
        //------------------------------------------------------
        void ComputeCollisionByRadius(int cellIndex, AWorldNode pNode, ref List<AWorldNode> vNodes, float randOffset =0)
        {
            if (cellIndex < 0 || cellIndex >= m_nSaptialCount) return;
            m_Cells[cellIndex].ScanRadiusCollision(pNode, ref vNodes, randOffset);
        }
        //------------------------------------------------------
        public void DrawDebug()
        {
#if UNITY_EDITOR
            for(int x =0; x <= m_nWidthNum; ++x)
            {
                for (int y = 0; y <= m_nHeightNum; ++y)
                {
                    Gizmos.DrawLine(new Vector3(m_StartPos.x + x*m_nCellWidth, m_StartPos.y + y*m_nCellHeight, m_StartPos.z), new Vector3(m_StartPos.x + x * m_nCellWidth, m_StartPos.y + y * m_nCellHeight, m_StartPos.z + m_nCellDepth*m_nDepthNum));
                }
                for (int z = 0; z <= m_nDepthNum; ++z)
                {
                    Gizmos.DrawLine(new Vector3(m_StartPos.x + x * m_nCellWidth, m_StartPos.y, m_StartPos.z + z * m_nCellDepth), new Vector3(m_StartPos.x + x * m_nCellWidth, m_StartPos.y + m_nHeightNum * m_nCellHeight, m_StartPos.z + z * m_nCellDepth));
                }
            }
            for (int z = 0; z <= m_nDepthNum; ++z)
            {
                for (int y = 0; y <= m_nHeightNum; ++y)
                {
                    Gizmos.DrawLine(new Vector3(m_StartPos.x, m_StartPos.y + y * m_nCellHeight, m_StartPos.z + z*m_nCellDepth), new Vector3(m_StartPos.x + m_nWidthNum * m_nCellWidth, m_StartPos.y + y * m_nCellHeight, m_StartPos.z + z * m_nCellDepth));
                }
            }
#endif
        }
    }
}
