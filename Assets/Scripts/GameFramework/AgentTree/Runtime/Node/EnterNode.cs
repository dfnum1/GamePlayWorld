/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	BaseNode
作    者:	HappLI
描    述:	动作节点
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [Serializable]
    public class EnterNode : BaseNode
    {
        public ushort EnterType = 0;
        public int CustomID=0;
        public string CustomName="";
        public GameObject CustomGO=null;

        public Port Param = null;

        public int ActionGUID=-1;
        [System.NonSerialized]
        public ExcudeNode Action;

        public Rect rect;

        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            if (Action != null)
                Action.Reset(vLocks);
            if (Param != null)
                Param.Reset();
        }

        public override void Init(AgentTree pTree)
        {
            if (m_bInited) return;
            base.Init(pTree);
            if (Action == null)
                Action = pTree.GetExcudeNode(ActionGUID);
            if (Action != null)
                Action.Init(pTree);
            if (Param != null)
                Param.Init(pTree);
        }
#if UNITY_EDITOR
        public override void Save()
        {
            if (Action != null)
            {
                ActionGUID = Action.GUID;
                Action.Save();
            }
            else
                ActionGUID = -1;
            if (Param != null)
            {
                Param.Save();
            }
        }
#endif
        public override void Destroy()
        {
            if (Action != null)
                Action.Destroy();
            if (Param != null)
                Param.Destroy();
        }

        public override void Copy(BaseNode pOther, bool bIncludeID = true)
        {
            base.Copy(pOther, bIncludeID);
            if (bIncludeID) GUID = pOther.GUID;
            flags = pOther.flags;
            strName = pOther.strName;

            EnterNode enter = pOther as EnterNode;
            rect = enter.rect;
            this.EnterType = enter.EnterType;
            this.CustomID = enter.CustomID;
            this.CustomName = enter.CustomName;
            this.CustomGO = enter.CustomGO;
            this.ActionGUID = enter.ActionGUID;
            this.Action = enter.Action;
            if (enter.Param != null)
            {
                this.Param = new Port(null);
                this.Param.Copy(enter.Param);
            }
            else
                this.Param = null;
        }
    }
}