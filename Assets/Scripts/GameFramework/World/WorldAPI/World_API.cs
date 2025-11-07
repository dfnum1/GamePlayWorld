/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	World
作    者:	HappLI
描    述:	世界
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Core
{
    public interface IWorldNodeCallback
    {
        void OnWorldNodeStatus(AWorldNode pNode, EWorldNodeStatus status, IUserData userVariable = null);
    }
    public partial class World
    {
        //-------------------------------------------------
        public T RayHitActors<T>(Ray ray) where T :AWorldNode
        {
            if (m_pRoot != null)
            {
                ShareFrameParams shareParam = GetFramework().shareParams; ;
                if (shareParam == null) return null;
                T rayActor = null;
                float fNearDist = float.MaxValue;
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode is T)
                    {
                        if(pNode.RayHit(ray))
                        {
                            Vector3 temp = (Vector3)pNode.GetPosition();
                            if ((temp - ray.origin).sqrMagnitude <= fNearDist)
                            {
                                fNearDist = (temp - ray.origin).sqrMagnitude;
                                rayActor = pNode as T;
                            }
                        }
                     }
                    pNode = pNode.GetNext();
                }
                return rayActor;
            }
            return null;
        }
        //-------------------------------------------------
        public T FindNode<T>(int guid) where T : AWorldNode
        {
            return this.InnerFindNode(guid) as T;
        }
        //-------------------------------------------------
        public T FindNodeBySvrID<T>(long roleGuid) where T : AWorldNode
        {
            if (roleGuid == 0) return null;
            if (m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode is T)
                    {
                        T actor = pNode as T;
                        if (actor.GetSvrGuid() == roleGuid) return actor;
                    }
                    pNode = pNode.GetNext();
                }
            }
            return null;
        }
        //-------------------------------------------------
        public T FindNodeByConfig<T>(EActorType type, uint config) where T : AWorldNode
        {
            if (config == 0) return null;
            if (m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode is T)
                    {
                        T actor = pNode as T;
                        if (actor.GetActorType() == type && actor.GetConfigID() == config) return actor;
                    }
                    pNode = pNode.GetNext();
                }
            }
            return null;
        }
        //-------------------------------------------------
        public T FindNearlyNodeByConfig<T>(Vector3 lookPos, EActorType type, uint config, HashSet<IUserData> vIngores = null) where T : AWorldNode
        {
            if (config == 0) return null;
            if (m_pRoot != null)
            {
                AWorldNode pActor = null;
                float nearlyDistance = float.MaxValue;
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode is T)
                    {
                        T actor = pNode as T;
                        if (actor.GetActorType() == type && actor.GetConfigID() == config)
                        {
                            if (vIngores != null && vIngores.Contains(actor)) continue;
                            float dist = (actor.GetPosition() - lookPos).sqrMagnitude;
                            if (dist <= nearlyDistance)
                            {
                                nearlyDistance = dist;
                                pActor = actor;
                            }
                        }
                    }
                    pNode = pNode.GetNext();
                }
                return pActor as T;
            }
            return null;
        }
        //-------------------------------------------------
        public int StatNodeByConfigID(int configID, uint typeFilter = 0xffffffff)
        {
            return this.StatNodeCountByConfigID(configID, typeFilter);
        }
        //-------------------------------------------------
        public void CollectNodes(ref List<AWorldNode> vNodes, uint typeFilter = 0xffffffff, HashSet<int> vIngores = null)
        {
            this.Collects(ref vNodes, typeFilter, vIngores);
        }
        //-------------------------------------------------
        public void CollectNodes<T>(ref List<AWorldNode> vNodes, uint typeFilter = 0xffffffff, HashSet<int> vIngores = null) where T : AWorldNode
        {
            this.Collects<T>(ref vNodes, typeFilter, vIngores);
        }
        //-------------------------------------------------
        public List<AWorldNode> CollectNodes<T>(uint typeFilter = 0xffffffff, HashSet<int> vIngores = null) where T : AWorldNode
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            this.Collects<T>(ref vNodes, typeFilter, vIngores);
            return vNodes;
        }
        //-------------------------------------------------
        public void CalcNeighbors(AWorldNode pNode, FFloat range, ref List<AWorldNode> vNodes)
        {
            this.ComputeNeighbors(pNode, range, ref vNodes);
        }
        //-------------------------------------------------
        public List<AWorldNode> CalcNeighbors(AWorldNode pNode, FFloat range)
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            this.ComputeNeighbors(pNode, range, ref vNodes);
            return vNodes;
        }
        //-------------------------------------------------
        public void CalcAABBBound(AWorldNode pNode, FVector3 min, FVector3 max, ref List<AWorldNode> vNodes)
        {
            this.CalcAABB(pNode, min, max, ref vNodes);
        }
        //-------------------------------------------------
        public List<AWorldNode> CalcAABBBound(AWorldNode pNode, FVector3 min, FVector3 max)
        {
            return this.CalcAABB(pNode, min, max);
        }
        //-------------------------------------------------
        public void CullingMatrix(FVector3 queryPosition, FMatrix4x4 culling, ref List<AWorldNode> vNodes)
        {
            this.Culling(queryPosition, culling, ref vNodes);
        }
        //-------------------------------------------------
        public List<AWorldNode> CullingMatrix(FVector3 queryPosition, FMatrix4x4 culling)
        {
            return this.Culling(queryPosition, culling);
        }
        //-------------------------------------------------
        public void CalcCollisionByRadius(AWorldNode pNode, ref List<AWorldNode> vNodes, float rangeOffset = 0, bool b3D = true)
        {
            this.ComputeCollisionByRadius(pNode, ref vNodes, rangeOffset, b3D);
        }
        //-------------------------------------------------
        public List<AWorldNode> CalcCollisionByRadius(AWorldNode pNode, float rangeOffset = 0, bool b3D = true)
        {
            return this.ComputeCollisionByRadius(pNode, rangeOffset, b3D);
        }
        //-------------------------------------------------
        public AWorldNode CreateNode(EActorType nodeType, IUserData pData, int nodeID = 0, IUserData userVariable = null, bool immediately = false)
        {
            return this.InnerCreateNode(nodeType, nodeID, pData, true, immediately, userVariable);
        }
        //-------------------------------------------------
        public T CreateNode<T>(EActorType nodeType, IUserData pData, int nodeID = 0, IUserData userVariable = null, bool immediately = false) where T : AWorldNode
        {
            return this.InnerCreateNode(nodeType, nodeID, pData, true, immediately, userVariable) as T;
        }
        //-------------------------------------------------
        public AWorldNode SyncCreateNode(EActorType nodeType, IUserData pData, int nodeID = 0, IUserData userVariable = null, bool immediately = false)
        {
            return this.InnerCreateNode(nodeType, nodeID, pData, false, immediately, userVariable);
        }
        //-------------------------------------------------
        public T SyncCreateNode<T>(EActorType nodeType, IUserData pData, int nodeID = 0, IUserData userVariable = null, bool immediately = false) where T : AWorldNode
        {
            return this.InnerCreateNode(nodeType, nodeID, pData, false, immediately, userVariable) as T;
        }
        //-------------------------------------------------
        public void ClearWorld()
        {
            if(m_pFramework!=null)
            {
                m_pFramework.OnClearWorld();
            }

            this.Clear();
        }
        //-------------------------------------------------
        public AWorldNode GetRootNode()
        {
            return this.m_pRoot;
        }
        //-------------------------------------------------
        public bool GetLimitZoom(ref FVector3 vMin, ref FVector3 vMax)
        {
            vMin = this.m_vMinRegion;
            vMax = this.m_vMaxRegion;
            return true;
        }
        //-------------------------------------------------
        public void SetLimitZoom(FVector3 vMin, FVector3 vMax)
        {
            this.SetLimitRegion(vMin, vMax);
        }
        //-------------------------------------------------
        public void SetWorldRadiusSearchPlottingScale(FFloat scale)
        {
            m_fWorldSearchRadiusPlottingScale = scale;
        }
    }
}
