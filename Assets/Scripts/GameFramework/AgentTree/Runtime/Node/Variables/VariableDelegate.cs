/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableDelegate
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.DelegateV)]
    [System.Serializable]
    public class VariableDelegate : AbsVariable<IUserData>
    {
        [System.NonSerialized] public IUserData param1;
        [System.NonSerialized] public IUserData param2;
        [System.NonSerialized] public IUserData param3;
        [System.NonSerialized] public IUserData param4;
        public int[] Actions;
        public bool bAutoDestroy = false;

        AgentTreeTask m_pTreeTask = null;

#if UNITY_EDITOR
        public DelegateLinkPort OutLink = new DelegateLinkPort();
#endif
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableDelegate>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        internal void SetTreeTask(AgentTreeTask pTreeTask)
        {
            m_pTreeTask = pTreeTask;
        }
        //------------------------------------------------------
        public IUserData GetData(int index)
        {
            switch(index)
            {
                case 0: return mValue;
                case 2: return param1;
                case 3: return param2;
                case 4: return param3;
                case 5: return param4;
            }
            return null;
        }
        //------------------------------------------------------
        public void DoCall()
        {
            if (m_pTreeTask == null) return;
            if (Actions != null && Actions.Length > 0)
            {
                for (int i = 0; i < Actions.Length; ++i)
                {
                    var node = m_pTreeTask.GetExcudeNode(Actions[i]);
                    if (node != null)
                    {
                        if(node.GetExcudeHash() == (int)EActionType.DelegateCallback)
                        {
                            if(node.outArgvs!=null)
                            {
                                for(int j=0; j < node.outArgvs.Length;++j)
                                {
                                    if(node.outArgvs[j]!=null)
                                    {
                                        VariableUser pVar = node.outArgvs[j].GetVariable<VariableUser>(m_pTreeTask);
                                        if (pVar != null)
                                            pVar.mValue = this.GetData(j);
                                    }
                                }
                            }
                        }
                        m_pTreeTask.AddDoAction(node);
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void SetClassHashCode(int hashCode) {  }

        public override int GetClassHashCode() { return 0; }
        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            m_pTreeTask = null;
            param1 = param2 = param3 = param4 = null;
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            if (IsFlag(EFlag.Const)) return;
            if (bAutoDestroy && mValue != null)
                mValue.Destroy();

            mValue = mValueDef;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableDelegate)
                mValue = ((VariableDelegate)oth).mValue;
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            m_pTreeTask = null;
            param1 = param2 = param3 = param4 = null;
            if (IsFlag(EFlag.Const)) return;
            if (bAutoDestroy)
            {
                base.Destroy();
                if (mValue != null) mValue.Destroy();
                mValue = null;
                param1 = param2 = param3 = param4 = null;
            }
        }
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableDelegate)) return 1;
            return mValue == (oth as VariableDelegate).mValue?0:1;
        }

        public static VariableDelegate DEFAULT = new VariableDelegate() { mValue = null };
#if UNITY_EDITOR
        //------------------------------------------------------
        public override void Save()
        {
            base.Save();
            Actions = null;
            List<int> vActions = new List<int>();
            for(int i =0; i < OutLink.linkNodes.Count; ++i)
            {
                if (OutLink.linkNodes[i] != null)
                {
                    vActions.Add(OutLink.linkNodes[i].GetGUID());
                }
            }
            Actions = vActions.ToArray();
        }
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            string strLabel = "回调";
            UnityEditor.EditorGUILayout.LabelField(strLabel);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
}
