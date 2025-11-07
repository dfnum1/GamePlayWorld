/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	BaseNode
作    者:	HappLI
描    述:	AT基础节点
*********************************************************************/
using System;
using UnityEngine;
namespace Framework.Plugin.AT
{
    public enum EFlag : short
    {
        Expanded = 1<<0,
        Debug = 1<<1,
        Locked = 1<<2,
        Global = 1 << 3,
        Const = 1 << 4,
        AutoDestroy = 1<<5,
        Declaration = 1<<6,
        Local = 1 << 7,
        Override = 1 << 8,
    }

    [Serializable]
    public abstract class BaseNode
    {
        public int GUID;
        public string strName;
        public ushort flags = 0;

        [System.NonSerialized]
        protected bool m_bInited = false;
#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bBreakPoint = false;
        [System.NonSerialized]
        public IGraphNode bindGraphNode = null;
        [System.NonSerialized]
        public IAgentTreeEditor pEditor = null;
#endif

        public bool IsFlag(EFlag flag)
        {
            return (flags & ((short)flag)) != 0;
        }
        //------------------------------------------------------
        public void SetFlag(EFlag flag, bool bEnable)
        {
            if (bEnable)
            {
                flags |= (ushort)flag;
            }
            else
                flags &= (ushort)(~(ushort)flag);
        }
        //------------------------------------------------------
        public virtual void Copy(BaseNode pOther, bool bIncludeID = true)
        {
            if (bIncludeID) GUID = pOther.GUID;
            flags = pOther.flags;
            strName = pOther.strName;
        }
        //------------------------------------------------------
        public virtual void Destroy()
        {
        }
        //------------------------------------------------------
        public virtual void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            vLocks.Add(GUID);
        }
        //------------------------------------------------------
        public virtual void Init(AgentTree pTree)
        {
            if (m_bInited) return;
            m_bInited = true;
        }
#if UNITY_EDITOR
        public virtual void Save() { }
#endif
    }
}
