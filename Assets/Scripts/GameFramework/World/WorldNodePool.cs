/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    internal interface IWorldMallolCallback
    {
        AWorldNode OnExcudeWorldNodeMalloc(EActorType type);
    }
    internal class WorldNodePool
    {
        public IWorldMallolCallback pMallocCallback = null;
        List<AWorldNode> m_vNodeList = new List<AWorldNode>();
        Dictionary<EActorType, Stack<AWorldNode>> m_Pools = new Dictionary<EActorType, Stack<AWorldNode>>(64);

        public List<AWorldNode> CatchNodeList
        {
            get
            {
                m_vNodeList.Clear();
                return m_vNodeList;
            }
        }

        public void ClearCatch()
        {
            m_vNodeList.Clear();
        }

        public bool Recyle(AWorldNode node)
        {
            if (node == null) return false;
            EActorType type = node.GetActorType();
            if (type == EActorType.None) return false;
            Stack<AWorldNode> pool;
            if(!m_Pools.TryGetValue(type, out pool))
            {
                pool = new Stack<AWorldNode>(64);
                m_Pools[type] = pool;
            }
            if (pool.Count < 64)
            {
                pool.Push(node);
                return true;
            }
            return false;
        }
        public AWorldNode Malloc(EActorType type, World world)
        {
            Stack<AWorldNode> pool;
            AWorldNode pNode = null;
            if (m_Pools.TryGetValue(type, out pool))
            {
                if (pool.Count > 0)
                    pNode = pool.Pop();
            }
            if (type == EActorType.Actor)
            {
                pNode = new Actor(world.GetFramework());
            }
            else if (type == EActorType.Character)
            {
#if USE_ACTORSYSTEM
                pNode = new Character(world.GetFramework());
#endif
            }
            else if (type == EActorType.Projectile)
            {
#if USE_ACTORSYSTEM
                pNode = new ProjectileNode(world.GetFramework());
#endif
            }
            if (pNode == null)
            {
                if (pMallocCallback != null)
                    pNode = pMallocCallback.OnExcudeWorldNodeMalloc(type);
            }
            if (pNode == null)
            {
                Base.Logger.Warning("type:" + type + "  create error, please set OnMallocEvent");
                return null;
            }

            AWorldNode.OnConstruction(pNode, world, type);
            return pNode;
        }
    }
}
