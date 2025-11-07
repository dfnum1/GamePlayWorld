/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	ExcudeNode
作    者:	HappLI
描    述:	执行节点
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [Serializable]
    public class APINode : ExcudeNode
    {
        public int excudeType =0;

        public override int GetExcudeHash() { return (int)excudeType; }

        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            APINode pAT = pOther as APINode;
            excudeType = pAT.excudeType;
        }
        //------------------------------------------------------
        protected override void OnReset()
        {
        }
        //------------------------------------------------------
        protected override void OnInit(AgentTree pTree)
        {
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        protected override void OnSave()
        {
        }
        public override void SetExcudeHash(int hash)
        {
            excudeType = hash;
        }
        //------------------------------------------------------
        public override T GetEditAttrData<T>()
        {
            return null;
        }

        public override string ToTips(ArgvPort port)
        {
            return "";
        }

        public override string ToTitleTips()
        {
            return "";
        }
#endif
    }
}
