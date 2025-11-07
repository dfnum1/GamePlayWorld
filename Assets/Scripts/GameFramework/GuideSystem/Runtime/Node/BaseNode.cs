#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideGuid
作    者:	HappLI
描    述:	引导基础节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    [System.Serializable]
    public class BaseNode
    {
        public int Guid;

        [System.NonSerialized]
        public GuideGroup guideGroup = null;
        public int guideGroupGUID
        {
            get { return guideGroup!=null?guideGroup.Guid:0; }
        }
        public ushort GetTag()
        {
            if (guideGroup == null) return 0xffff;
            return guideGroup.Tag;
        }
        public virtual List<ArgvPort> GetArgvPorts()
        {
            return null;
        }
        public virtual List<IUserData> GetBeginEvents()
        {
            return null;
        }

        public virtual List<IUserData> GetEndEvents()
        {
            return null;
        }
#if UNITY_EDITOR
        public string Name;
        public bool bExpand = false;
        public int posX;
        public int posY;
        public virtual void SetArgvPorts(List<ArgvPort> vPorts) { }
#endif
        public virtual void Init(GuideGroup pGroup)
        {
            guideGroup = pGroup;
        }
#if UNITY_EDITOR
        public virtual void Save()
        {

        }
        internal virtual void CheckPorts()
        {

        }
#endif
    }

    public abstract class SeqNode : BaseNode
    {
        public int[] Ops = null;
        public int nextGuid = 0;
        public bool bFireCheck = false;

        public override List<ArgvPort> GetArgvPorts()
        {
            return null;
        }

        [System.NonSerialized]
        public SeqNode pNext = null;

        public virtual float GetDeltaTime()
        {
            return 0;
        }

        public virtual float GetAutoNextTime()
        {
            return 0;
        }
        public virtual ExcudeNode GetAutoExcudeNode()
        {
            return null;
        }

        public virtual bool IsAutoNext()
        {
            return true;
        }

        public virtual bool IsAutoSignCheck()
        {
            return false;
        }

        public virtual bool IsSuccessedListenerBreak()
        {
            return false;
        }

        public virtual bool IsOption()
        {
            return false;
        }

        public virtual float GetDeltaSignTime()
        {
            return 0;
        }

        public abstract int GetEnumType();

        [System.NonSerialized]
        public List<GuideOperate> vOps = null;
        public override void Init(GuideGroup pGroup)
        {
            base.Init(pGroup);
            if (Ops != null && Ops.Length>0)
            {
                vOps = new List<GuideOperate>(Ops.Length);
                for (int i = 0; i < Ops.Length; ++i)
                {
                    GuideOperate pNode = pGroup.GetNode<GuideOperate>(Ops[i]);
                    if (pNode == null) continue;
                    vOps.Add(pNode);
                }
            }
            else
            {
#if UNITY_EDITOR
                vOps = new List<GuideOperate>();
#endif
            }

            if(Ops==null || Ops.Length<=0)
            {
                pNext = pGroup.GetNode<SeqNode>(nextGuid);
            }
        }
#if UNITY_EDITOR
        public override void Save()
        {
            base.Save();
            if (vOps.Count > 0)
            {
                Ops = new int[vOps.Count];
                for (int i = 0; i < Ops.Length; ++i)
                    Ops[i] = vOps[i].Guid;
            }
            else
                Ops = null;
            if (pNext != null)
            {
                nextGuid = pNext.Guid;
                Ops = null;
            }
            else
                nextGuid = 0;
        }
#endif
    }
}
#endif