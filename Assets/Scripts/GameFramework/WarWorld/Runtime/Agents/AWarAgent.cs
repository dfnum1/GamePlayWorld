/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	AWarAgent
作    者:	HappLI
描    述:	War行为体基类
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
    public abstract class AWarAgent
    {
        WarWorld m_pOwnerWar = null;
        List<AWorldNode> m_vNodes = null;
        WarAgentData m_pData;
        //-------------------------------------------------
        public void SetOwnerWar(WarWorld war)
        {
            m_pOwnerWar = war;
        }
        //-------------------------------------------------
        public void SetAgentData(WarAgentData data)
        {
            m_pData = data;
        }
        //-------------------------------------------------
        public void Awake()
        {
        }
        //-------------------------------------------------
        public void AddNode(AWorldNode pNode)
        {
            if (pNode == null)
                return;
            if (m_vNodes == null) m_vNodes = new List<AWorldNode>(4);
            if (m_vNodes.Contains(pNode))
                return;
            m_vNodes.Add(pNode);
        }
        //-------------------------------------------------
        public void RemoveNode(AWorldNode pNode)
        {
            if (pNode == null || m_vNodes == null)
                return;
            m_vNodes.Remove(pNode);
        }
        //-------------------------------------------------
        public void Update(FFloat delta)
        {
        }
        //-------------------------------------------------
        protected virtual void Clear()
        {
            m_pData = null;
        }
        //-------------------------------------------------
        public void Destroy()
        {
            Clear();
            m_pOwnerWar = null;
        }
        //-------------------------------------------------
        protected virtual void OnAwake() { }
        protected virtual void OnUpdate(FFloat delta) { }

    }
}
