/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SpatialNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public struct SpatialCell
    {
        private List<AWorldNode> m_vNodes;
        //------------------------------------------------------
        public void Clear()
        {
            if (m_vNodes == null) return;
            m_vNodes.Clear();
        }
        //------------------------------------------------------
        public void AddNode(AWorldNode pNode)
        {
            if (pNode == null) return;
            if (m_vNodes == null) m_vNodes = new List<AWorldNode>(16);
            m_vNodes.Add(pNode);
        }
        //------------------------------------------------------
        public void ScanRangeSq(Vector3 position, float rangSq, ref List<AWorldNode> vNodes)
        {
            if (m_vNodes == null || m_vNodes.Count<=0) return;
            AWorldNode node;
            for(int i =0; i< m_vNodes.Count; ++i)
            {
                node = m_vNodes[i];
                if (node.IsKilled() || node.IsDestroy()) continue;
                if ((node.GetPosition() - position).sqrMagnitude <= rangSq + node.GetBounds().GetBoundSizeSqr())
                    vNodes.Add(node);
            }
        }
        //------------------------------------------------------
        public void ScanRangeSq(AWorldNode pNode, float rangSq, ref List<AWorldNode> vNodes)
        {
            if (m_vNodes == null || m_vNodes.Count <= 0) return;
            AWorldNode node;
            Vector3 position = pNode.GetPosition();
            for (int i = 0; i < m_vNodes.Count; ++i)
            {
                node = m_vNodes[i];
                if (pNode == node) continue;
                if (node.IsKilled() || node.IsDestroy()) continue;
                if ((node.GetPosition() - position).sqrMagnitude <= rangSq+ node.GetBounds().GetBoundSizeSqr())
                    vNodes.Add(node);
            }
        }
        //------------------------------------------------------
        public void ScanRadiusCollision(AWorldNode pNode, ref List<AWorldNode> vNodes, float randOffset = 0)
        {
            if (m_vNodes == null || m_vNodes.Count <= 0) return;
            AWorldNode node;
            float rang = 0;
            Vector3 position = pNode.GetPosition();
            for (int i = 0; i < m_vNodes.Count; ++i)
            {
                node = m_vNodes[i];
                if (pNode == node) continue;
                if (node.IsKilled() || node.IsDestroy() || pNode.GetPhysicRadius()<=0) continue;
                rang = pNode.GetPhysicRadius() + randOffset + node.GetPhysicRadius();
                if ((node.GetPosition() - position).sqrMagnitude <= rang * rang)
                    vNodes.Add(node);
            }
        }
        //------------------------------------------------------
        public void IntersectionBound(AWorldNode pNode, ref List<AWorldNode> vNodes)
        {
            if (m_vNodes == null || m_vNodes.Count <= 0) return;
            AWorldNode node;
            WorldBoundBox boundBox = pNode.GetBounds();
            float boundSize = boundBox.GetBoundSizeSqr();
            Vector3 position = pNode.GetPosition();
            var shareParam = pNode.GetShareFrameParams();
            if (shareParam == null) return;
            for (int i = 0; i < m_vNodes.Count; ++i)
            {
                node = m_vNodes[i];
                if (pNode == node) continue;
                if (node.IsKilled() || node.IsDestroy()) continue;
                WorldBoundBox othBox = node.GetBounds();
                if ((node.GetPosition() - position).sqrMagnitude <= othBox.GetBoundSizeSqr() + boundSize )
                {
                    if(Base.IntersetionUtil.CU_WorldBoxIntersection(shareParam.intersetionParam, ref boundBox,ref othBox))
                        vNodes.Add(node);
                }
            }
        }
    }
}
