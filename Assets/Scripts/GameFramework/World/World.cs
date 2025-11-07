/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	World
作    者:	HappLI
描    述:	世界
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [Plugin.AT.ATExportMono("World世界", "Framework.Module.ModuleManager.mainModule.world")]
    public partial class World : AModule, IDrawGizmos
    {
        WorldNodePool m_Pool = new WorldNodePool();
        WorldTriggers m_WorldTriggers = null;
        WorldPhysic m_WorldPhysic = null;
        // WorldSpatial m_pWorldSpatial = null;
        KDTree.WorldKDTree m_pWorldKDTree = null;
        AWorldNode m_pTail = null;
        AWorldNode m_pRoot = null;
        List<AWorldNode> m_vDestroyList = new List<AWorldNode>(16);
        Dictionary<int, AWorldNode> m_vNodes = new Dictionary<int, AWorldNode>(128);
        int m_nAutoGUID = 0;

#if USE_FIXEDMATH
        FFloat m_fWorldSearchRadiusPlottingScale = FFloat.one;
        FVector3 m_vMinRegion = FVector3.min;
        FVector3 m_vMaxRegion = FVector3.max;
#else
        FFloat m_fWorldSearchRadiusPlottingScale = 1.0f;
        FVector3 m_vMinRegion = Vector3.one*float.MinValue;
        FVector3 m_vMaxRegion = Vector3.one * float.MaxValue;
#endif
        List<IWorldNodeCallback> m_vCallbacks = new List<IWorldNodeCallback>(4);
        //-------------------------------------------------
        protected override void OnAwake()
        {
            m_vDestroyList.Clear();
            m_vCallbacks.Clear();
            m_vNodes.Clear();
            m_nAutoGUID = 0;
#if USE_FIXEDMATH
            m_fWorldSearchRadiusPlottingScale = FFloat.one;
#else
            m_fWorldSearchRadiusPlottingScale = 1.0f;
#endif
            m_WorldTriggers = new WorldTriggers(this);
            m_WorldPhysic = new WorldPhysic(this);
            // m_pWorldSpatial = new WorldSpatial(this,3,10,5,4,20,10);
            m_pWorldKDTree = new KDTree.WorldKDTree(this);
        }
        //-------------------------------------------------
        public WorldPhysic GetPhysic()
        {
            return m_WorldPhysic;
        }
        //-------------------------------------------------
        public WorldTriggers GetWorldTriggers()
        {
            return m_WorldTriggers;
        }
        //-------------------------------------------------
        public void RegisterCallback(IWorldNodeCallback callback)
        {
            if(callback!=null && !m_vCallbacks.Contains(callback))
                m_vCallbacks.Add(callback);
            if(m_Pool.pMallocCallback == null)
            {
                IWorldMallolCallback mallockCB = callback as IWorldMallolCallback;
                if (mallockCB != null)
                {
                    m_Pool.pMallocCallback = mallockCB;
                }
            }
        }
        //-------------------------------------------------
        public void UnRegisterCallback(IWorldNodeCallback callback)
        {
            if (callback != null)
                m_vCallbacks.Remove(callback);
        }
        //-------------------------------------------------
        private AWorldNode InnerFindNode(int guid)
        {
            if (guid == 0) return null;
            AWorldNode worldNode;
            if (m_vNodes.TryGetValue(guid, out worldNode) && !worldNode.IsDestroy())
                return worldNode;
            return null;
        }
        //-------------------------------------------------
        void AddNode(AWorldNode pNode)
        {
            if (pNode == null) return;
            if (m_pRoot == null)
                m_pRoot = pNode;
            if (m_pTail == null)
                m_pTail = m_pRoot;
            else
            {
                m_pTail.SetNext(pNode);
                pNode.SetPrev(m_pTail);
            }

            m_pTail = pNode;

            m_vNodes.Add(pNode.GetInstanceID(), pNode);
        }
        //-------------------------------------------------
        public void RemoveNode(AWorldNode pNode, bool bRemoveMaps = true)
        {
            var prev = pNode.GetPrev();
            var next = pNode.GetNext();
            if (prev != null)
                prev.SetNext(next);
            if (next != null)
                next.SetPrev(prev);

            if (m_pRoot == pNode)
                m_pRoot = next;
            if (m_pTail == pNode)
                m_pTail = prev;

            if (bRemoveMaps)
                m_vNodes.Remove(pNode.GetInstanceID());
            pNode.FreeDestroy();
        }
        //-------------------------------------------------
        internal bool Recyle(AWorldNode pNode)
        {
            return m_Pool.Recyle(pNode);
        }
        //-------------------------------------------------
        public void OnWorldNodeCallback(AWorldNode pNode, EWorldNodeStatus status, IUserData userVariable = null)
        {
            for (int i = 0; i < m_vCallbacks.Count; ++i)
            {
                m_vCallbacks[i].OnWorldNodeStatus(pNode, status, userVariable);
            }
        }
        //-------------------------------------------------
        public long AddStaticObstacle(FVector3 vPos, List<Vector3> vPoints)
        {
            return m_WorldPhysic.AddStaticObstacle(vPos, vPoints);
        }
        //-------------------------------------------------
        public void RemoveStaticObstacle(long key)
        {
            m_WorldPhysic.RemoveStaticObstacle(key);
        }
        //-------------------------------------------------
        public int AddTrigger(AWorldNode pOwner, WorldTriggerParamter parameter)
        {
            return m_WorldTriggers.AddTrigger(pOwner,parameter);
        }
        //-------------------------------------------------
        public void RemoveTrigger(int id)
        {
            m_WorldTriggers.RemoveTrigger(id);
        }
        //-------------------------------------------------
        public void RemoveTrigger(AWorldNode pOwner)
        {
            m_WorldTriggers.RemoveTrigger(pOwner);
        }
        //-------------------------------------------------
        private void Collects(ref List<AWorldNode> vNodes, uint typeFilter = 0xffffffff, HashSet<int> vIngores = null)
        {
            if (m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode.IsCollectAble() && pNode.IsCanLogic() && (((1<<(int)pNode.GetActorType())&typeFilter)!=0) && (vIngores == null || !vIngores.Contains(pNode.GetInstanceID())))
                        vNodes.Add(pNode);
                    pNode = pNode.GetNext();
                }
            }
        }
        //-------------------------------------------------
        private int StatNodeCountByConfigID(int configID, uint typeFilter = 0xffffffff)
        {
            int cnt = 0;
            if (m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (!pNode.IsDestroy() && !pNode.IsKilled() && (((1 << (int)pNode.GetActorType()) & typeFilter) != 0) && configID == pNode.GetConfigID())
                        cnt++;
                    pNode = pNode.GetNext();
                }
            }
            return cnt;
        }
        //-------------------------------------------------
        private void Collects<T>(ref List<AWorldNode> vNodes, uint typeFilter = 0xffffffff, HashSet<int> vIngores = null) where T : AWorldNode
        {
            if (m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode is T && pNode.IsCollectAble() && pNode.IsCanLogic() && (((1 << (int)pNode.GetActorType()) & typeFilter) != 0) && (vIngores == null || !vIngores.Contains(pNode.GetInstanceID())))
                        vNodes.Add(pNode);
                    pNode = pNode.GetNext();
                }
            }
        }
        //-------------------------------------------------
        private void ComputeNeighbors(AWorldNode pNode, FFloat range, ref List<AWorldNode> vNodes)
        {
            m_pWorldKDTree.QueryRadius(pNode.GetPosition(), range* m_fWorldSearchRadiusPlottingScale, vNodes);
           // m_pWorldSpatial.ComputeNeighbors(pNode, range, ref vNodes);
        }
        //-------------------------------------------------
        private List<AWorldNode> ComputeNeighbors(AWorldNode pNode, FFloat range)
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            // m_pWorldSpatial.ComputeNeighbors(pNode, range, ref vNodes);
            m_pWorldKDTree.QueryRadius(pNode.GetPosition(), range* m_fWorldSearchRadiusPlottingScale, vNodes, pNode);
            return vNodes;
        }
        //-------------------------------------------------
        private void CalcAABB(AWorldNode pNode, FVector3 min, FVector3 max, ref List<AWorldNode> vNodes)
        {
            m_pWorldKDTree.QueryInterval(min+pNode.GetPosition(), max+pNode.GetPosition(), vNodes, pNode);
            // m_pWorldSpatial.ComputeNeighbors(pNode, range, ref vNodes);
        }
        //-------------------------------------------------
        private List<AWorldNode> CalcAABB(AWorldNode pNode, FVector3 min, FVector3 max)
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            // m_pWorldSpatial.ComputeNeighbors(pNode, range, ref vNodes);
            m_pWorldKDTree.QueryInterval(min + pNode.GetPosition(), max + pNode.GetPosition(), vNodes, pNode);
            return vNodes;
        }
        //-------------------------------------------------
        internal void ComputeCollisionByRadius(AWorldNode pNode, ref List<AWorldNode> vNodes, float rangeOffset = 0, bool b3D = true)
        {
            m_pWorldKDTree.QueryRadius(pNode.GetPosition(), (pNode.GetPhysicRadius()+rangeOffset)*2, vNodes, pNode);
        //    m_pWorldSpatial.ComputeCollisionByRadius(pNode, ref vNodes, rangeOffset, b3D);
        }
        //-------------------------------------------------
        private List<AWorldNode> ComputeCollisionByRadius(AWorldNode pNode, float rangeOffset = 0, bool b3D = true)
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            // m_pWorldSpatial.ComputeCollisionByRadius(pNode, ref vNodes, rangeOffset, b3D);
            m_pWorldKDTree.QueryRadius(pNode.GetPosition(), (pNode.GetPhysicRadius() + rangeOffset)*2, vNodes, pNode);
            return vNodes;
        }
        //-------------------------------------------------
        public FVector3 ComputerNewVelocity(AWorldNode pNode, FVector3 prefNormalVelocity, FFloat maxSpeed, FFloat timeHorizon, FFloat timeHorizonObs, out bool isCollisioned)
        {
            return m_WorldPhysic.ComputerNewVelocity(m_pWorldKDTree, pNode, prefNormalVelocity, maxSpeed, timeHorizon, timeHorizonObs, out isCollisioned);
        }
        //-------------------------------------------------
        private List<AWorldNode> Culling(FVector3 queryPosition, FMatrix4x4 culling)
        {
            List<AWorldNode> vNodes = m_Pool.CatchNodeList;
            m_pWorldKDTree.QueryCulling(queryPosition, culling, vNodes);
            return vNodes;
        }
        //-------------------------------------------------
        private void Culling(FVector3 queryPosition, FMatrix4x4 culling, ref List<AWorldNode> vNodes)
        {
            m_pWorldKDTree.QueryCulling(queryPosition, culling, vNodes);
        }
        //-------------------------------------------------
        AWorldNode InnerCreateNode(EActorType nodeType, int nodeID, IUserData pData, bool bAsync, bool immediately = false, IUserData userVariable = null)
        {
            AWorldNode pNode = null;
            if(pNode == null) pNode = m_Pool.Malloc(nodeType,this);
            if (pNode == null) return null;

            if (nodeID == 0)
            {
                m_nAutoGUID++;
                nodeID = m_nAutoGUID;
            }
            else
            {
                if (m_vNodes.ContainsKey(nodeID))
                {
                    m_nAutoGUID++;
                    nodeID = m_nAutoGUID;
                }
                else
                    m_nAutoGUID = Mathf.Max(m_nAutoGUID, nodeID);
            }
            pNode.SetContextData(pData);
            pNode.SetInstanceID(nodeID);
            AddNode(pNode);

            VariableMultiData mutiData = new VariableMultiData();
            mutiData.pData1 = new Variable1() { byteVal0 = (byte)(bAsync ? 1 : 0), byteVal1 = (byte)(immediately?1:0) };
            mutiData.pData2 = userVariable;

            OnWorldNodeCallback(pNode, EWorldNodeStatus.Create, mutiData);
            pNode.OnCreated();
            return pNode;
        }
        //-------------------------------------------------
        protected override void OnUpdate(float delta)
        {
            FFloat fFrame = delta;
            m_pWorldKDTree.Clear();
            //   m_pWorldSpatial.Clear();
            if (m_pRoot!=null)
            {
                int index = 0;
                AWorldNode pNode = m_pRoot;
                while(pNode!=null)
                {
                    if (pNode.IsFlag(EWorldNodeFlag.Destroy))
                    {
                        m_vDestroyList.Add(pNode);
                    }
                    else if (pNode.IsFlag(EWorldNodeFlag.Spatial))
                    {
                        m_pWorldKDTree.Set(pNode, index++, false);
                        //  m_pWorldSpatial.AddNode(pNode);
                    }
                    pNode = pNode.GetNext();
                }
                m_pWorldKDTree.Rebuild();

                AWorldNode destroyNode;
                for (int i = 0; i < m_vDestroyList.Count; ++i)
                {
                    destroyNode = m_vDestroyList[i];
                    m_vNodes.Remove(destroyNode.GetInstanceID());
                    RemoveNode(destroyNode, false);
                }
                m_vDestroyList.Clear();

                pNode = m_pRoot;
                while (pNode != null)
                {
                    pNode.Update(fFrame);
                    m_WorldTriggers.TriggerCheck(pNode);
                    pNode = pNode.GetNext();
                }
                m_WorldTriggers.Update();
            }
            else
            {
                AWorldNode destroyNode;
                for (int i = 0; i < m_vDestroyList.Count; ++i)
                {
                    destroyNode = m_vDestroyList[i];
                    m_vNodes.Remove(destroyNode.GetInstanceID());
                    RemoveNode(destroyNode, false);
                }
                m_vDestroyList.Clear();
            }

            m_Pool.ClearCatch();
        }
        //-------------------------------------------------
        public List<AWorldNode> CatchNodeList
        {
            get { return m_Pool.CatchNodeList; }
        }
        //-------------------------------------------------
        void SetLimitRegion(FVector3 vMin, FVector3 vMax)
        {
            m_vMinRegion = FVector3.Min(vMin, vMax);
            m_vMaxRegion = FVector3.Max(vMin, vMax);
        }
        //-------------------------------------------------
        public FVector3 GetMinLimitRegion()
        {
            return m_vMinRegion;
        }
        //-------------------------------------------------
        public FVector3 GetMaxLimitRegion()
        {
            return m_vMaxRegion;
        }
        //-------------------------------------------------
        void Clear()
        {
            if(m_pWorldKDTree!=null) m_pWorldKDTree.Clear();
            if(m_WorldTriggers!=null) m_WorldTriggers.Clear();
          //  m_pWorldSpatial.Clear();
            if (m_pRoot != null)
            {
                AWorldNode pNext = null;
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    pNext = pNode.GetNext();
                    pNode.FreeDestroy();
                    pNode = pNext;
                }
            }
            m_vNodes.Clear();
            m_vDestroyList.Clear();
            m_pRoot = null;
            m_pTail = null;
            m_nAutoGUID = 0;
#if USE_FIXEDMATH
            m_fWorldSearchRadiusPlottingScale = FFloat.one;
#else
            m_fWorldSearchRadiusPlottingScale = 1.0f;
#endif

            m_vMinRegion = -Vector3.one * 100000;
            m_vMaxRegion = Vector3.one * 100000;
        }
        //-------------------------------------------------
        protected override void OnDestroy()
        {
            Clear();
            m_vCallbacks.Clear();
        }
        //-------------------------------------------------
        public void DrawGizmos()
        {
#if UNITY_EDITOR
            bool bShowSpatial =Base.ConfigUtil.bShowSpatial;
            bool bShowNodeDebugFrame = Base.ConfigUtil.bShowNodeDebugFrame;
            Color color = UnityEditor.Handles.color;
            if(Mathf.Abs(m_vMaxRegion.x-m_vMinRegion.x)>0.01f && Mathf.Abs(m_vMaxRegion.z - m_vMinRegion.z) > 0.01f)
            {
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireCube((m_vMaxRegion + m_vMinRegion) / 2, m_vMaxRegion - m_vMinRegion);
                UnityEditor.Handles.color = color;
            }

            if (bShowSpatial)
            {
                //   m_pWorldSpatial.DrawDebug();
                m_pWorldKDTree.DrawDebug();
            }
            m_WorldTriggers.DrawDebug();
            if ( m_pRoot != null)
            {
                AWorldNode pNode = m_pRoot;
                while (pNode != null)
                {
                    if (pNode.IsDebug())
                    {
                        if (bShowNodeDebugFrame)
                            pNode.DrawDebug();
                        UnityEditor.Handles.color = Color.yellow;
                        UnityEditor.Handles.CircleHandleCap(0, pNode.GetPosition() + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), pNode.GetPhysicRadius(), EventType.Repaint);
                        UnityEditor.Handles.color = color;
                    }

                    pNode = pNode.GetNext();
                }
            }
#endif
        }
    }
}
