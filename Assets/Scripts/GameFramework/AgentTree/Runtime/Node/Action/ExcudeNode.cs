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
    public abstract class ExcudeNode : BaseNode
    {
        public Port[] inArgvs;
        public Port[] outArgvs;
        public int[] nextActionsID;

        [System.NonSerialized]
        public ExcudeNode[] nextActions;

        public Rect rect;

        public abstract int GetExcudeHash();
        public virtual void SetCustomValue(long value) { }
        public virtual long GetCustomValue() { return 0; }

        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;

            ExcudeNode pAT = pOther as ExcudeNode;
            if (pAT.inArgvs != null)
            {
                inArgvs = new Port[pAT.inArgvs.Length];
                for (int i = 0; i < pAT.inArgvs.Length; ++i)
                {
                    inArgvs[i] = new Port(null);
                    inArgvs[i].Copy(pAT.inArgvs[i]);
                }
            }
            if (pAT.outArgvs != null)
            {
                outArgvs = new Port[pAT.outArgvs.Length];
                for (int i = 0; i < pAT.outArgvs.Length; ++i)
                {
                    outArgvs[i] = new Port(null);
                    outArgvs[i].Copy(pAT.outArgvs[i]);
                }
            }

            if (pAT.nextActions != null)
            {
                nextActions = new ExcudeNode[pAT.nextActions.Length];
                System.Array.Copy(pAT.nextActions, nextActions, pAT.nextActions.Length);
            }

            rect = pAT.rect;
        }
        //------------------------------------------------------
        [System.NonSerialized]
        Variable m_vTempVar = null;
        [System.NonSerialized]
        public float fDeltaTime = 0;
        [System.NonSerialized]
        public bool bDeltaing = false;
        public override void Destroy()
        {
            if (m_vTempVar != null)
            {
                m_vTempVar.Destroy();
                AgentTreeManager.getInstance().RecycelVars(m_vTempVar);
                m_vTempVar = null;
            }
            if (nextActions != null)
            {
                for (int i = 0; i < nextActions.Length; ++i)
                {
                    nextActions[i].Destroy();
                }
            }
            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                    inArgvs[i].Destroy();
            }
            if (outArgvs != null)
            {
                for (int i = 0; i < outArgvs.Length; ++i)
                    outArgvs[i].Destroy();
            }
        }
        //------------------------------------------------------
        public override void Reset(HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            fDeltaTime = 0;
            bDeltaing = false;
            if (m_vTempVar != null) m_vTempVar.Reset(AgentTreeUtl.intSet);
            if (nextActions != null)
            {
                for (int i = 0; i < nextActions.Length; ++i)
                {
                    nextActions[i].Reset(vLocks);
                }
            }

            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                    inArgvs[i].Reset();
            }
            if (outArgvs != null)
            {
                for (int i = 0; i < outArgvs.Length; ++i)
                    outArgvs[i].Reset();
            }
            OnReset();
        }
        //------------------------------------------------------
        protected abstract void OnReset();
        //------------------------------------------------------
        public T GetInVariableByIndex<T>(int index, AgentTreeTask pTask = null) where T : Variable
        {
            if (inArgvs == null || index >= inArgvs.Length) return null;
            return inArgvs[index].GetVariable<T>(pTask);
        }
        //------------------------------------------------------
        public T GetOutVariableByIndex<T>(int index, AgentTreeTask pTask = null) where T : Variable
        {
            if (outArgvs == null || index >= outArgvs.Length) return null;
            return outArgvs[index].GetVariable<T>(pTask);
        }
        //------------------------------------------------------
        public Variable GetInVariable(int index)
        {
            if (inArgvs == null || index >= inArgvs.Length) return null;
            return inArgvs[index].variable;
        }
        //------------------------------------------------------
        public Variable GetOutVariable(int index)
        {
            if (outArgvs == null || index >= outArgvs.Length) return null;
            return outArgvs[index].variable;
        }
        //------------------------------------------------------
        public Port GetInPort(int index)
        {
            if (inArgvs == null || index >= inArgvs.Length) return null;
            return inArgvs[index];
        }
        //------------------------------------------------------
        public Port GetOutPort(int index)
        {
            if (outArgvs == null || index >= outArgvs.Length) return null;
            return outArgvs[index];
        }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            if (m_bInited) return;
            base.Init(pTree);
            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                {
                    inArgvs[i].Init(pTree);
                }
            }
            if (outArgvs != null)
            {
                for (int i = 0; i < outArgvs.Length; ++i)
                {
                    outArgvs[i].Init(pTree);
                }
            }

            if (nextActionsID != null)
            {
                nextActions = new ExcudeNode[nextActionsID.Length];
                for (int i = 0; i < nextActionsID.Length; ++i)
                {
                    nextActions[i] = pTree.GetExcudeNode(nextActionsID[i]);
                    if (nextActions[i] != null)
                        nextActions[i].Init(pTree);
                }
            }
            OnInit(pTree);
        }
        //------------------------------------------------------
        protected abstract void OnInit(AgentTree pTree);

        public virtual void OnFillInArgv(AgentTreeTask task ) { }

#if UNITY_EDITOR
        //------------------------------------------------------
        public T GetInEditorPort<T>(int index, int offset=0) where T : IPortEditor, new()
        {
            if (inArgvs == null || index >= inArgvs.Length) return default(T);
            if (inArgvs[index].Editorer == null)
                inArgvs[index].Editorer = new T();
            if(offset == 1)
            {
                if (inArgvs[index].Editorer1 == null)
                    inArgvs[index].Editorer1 = new T();
            }
            return inArgvs[index].GetEditorer<T>(offset);
        }
        //------------------------------------------------------
        public T GetOutEditorPort<T>(int index) where T : IPortEditor, new()
        {
            if (outArgvs == null || index >= outArgvs.Length) return default(T);
            return outArgvs[index].GetEditorer<T>();
        }
        //------------------------------------------------------
        public List<Variable> GetAllVariable()
        {
            List<Variable> varibales = new List<Variable>();
            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                {
                    if (inArgvs[i].variable != null)
                        varibales.Add(inArgvs[i].variable);
                    if (inArgvs[i].dummyMap == null) continue;
                    foreach (var db in inArgvs[i].dummyMap)
                    {
                        if (db.Value != null)
                            varibales.Add(db.Value);
                    }
                }
            }
            if (outArgvs != null)
            {
                for (int i = 0; i < outArgvs.Length; ++i)
                {
                    if (outArgvs[i].variable != null)
                        varibales.Add(outArgvs[i].variable);
                    if (outArgvs[i].dummyMap == null) continue;
                    foreach (var db in outArgvs[i].dummyMap)
                    {
                        if (db.Value != null)
                            varibales.Add(db.Value);
                    }
                }
            }
            OnGetAllVariable(varibales);
            return varibales;
        }
        //------------------------------------------------------
        protected virtual void OnGetAllVariable(List<Variable> variables) { }
        //------------------------------------------------------
        public int GetInArgvCount()
        {
            return inArgvs != null ? inArgvs.Length : 0;
        }
        //------------------------------------------------------
        public int GetOutArgvCount()
        {
            return outArgvs != null ? outArgvs.Length : 0;
        }
        //------------------------------------------------------
        public void ClearArgv()
        {
            inArgvs = null;
            outArgvs = null;
        }
        //------------------------------------------------------
        public void ClearInArgv()
        {
            inArgvs = null;
        }
        //------------------------------------------------------
        public void ClearOutArgv()
        {
            outArgvs = null;
        }
        //------------------------------------------------------
        public void SwapInArgv(int i0, int i1)
        {
            if (i0 == i1) return;
            if (inArgvs == null || i0 <0 || i1 <0 || i0 >= inArgvs.Length || i1>= inArgvs.Length) return;
            Port temp = inArgvs[i0];
            inArgvs[i0] = inArgvs[i1];
            inArgvs[i1] = temp;
        }
        //------------------------------------------------------
        public void SwapOutArgv(int i0, int i1)
        {
            if (i0 == i1) return;
            if (outArgvs == null || i0 < 0 || i1 < 0 || i0 >= outArgvs.Length || i1 >= outArgvs.Length) return;
            Port temp = outArgvs[i0];
            outArgvs[i0] = outArgvs[i1];
            outArgvs[i1] = temp;
        }
        //------------------------------------------------------
        public void AddInPort(Port pPort)
        {
            List<Port> vars = inArgvs != null ? new List<Port>(inArgvs) : new List<Port>();
            vars.Add(pPort);
            inArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public void DelInPort(Port pPort)
        {
            if (inArgvs == null) return;
            for(int i =0; i < inArgvs.Length; ++i)
            {
                if(inArgvs[i] == pPort)
                {
                    DelInPort(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void DelInPort(int index)
        {
            if (inArgvs == null || index < 0 || index >= inArgvs.Length) return;
            List<Port> vars = inArgvs != null ? new List<Port>(inArgvs) : new List<Port>();
            vars.RemoveAt(index);
            inArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public void AddOutPort(Port pPort)
        {
            List<Port> vars = outArgvs != null ? new List<Port>(outArgvs) : new List<Port>();
            vars.Add(pPort);
            outArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public void DelOutPort(Port pPort)
        {
            if (outArgvs == null) return;
            for (int i = 0; i < outArgvs.Length; ++i)
            {
                if (outArgvs[i] == pPort)
                {
                    DelOutPort(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void DelOutPort(int index)
        {
            if (outArgvs == null || index < 0 || index >= outArgvs.Length) return;
            List<Port> vars = outArgvs != null ? new List<Port>(outArgvs) : new List<Port>();
            vars.RemoveAt(index);
            outArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public void AddInArgv(Variable pVar)
        {
            List<Port> vars = inArgvs != null ? new List<Port>(inArgvs) : new List<Port>();
            vars.Add(new Port(pVar));
            inArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public void AddOutArgv(Variable pVar)
        {
            List<Port> vars = outArgvs != null ? new List<Port>(outArgvs) : new List<Port>();
            vars.Add(new Port(pVar));
            outArgvs = vars.ToArray();
        }
        //------------------------------------------------------
        public bool HasInArgv(Variable pVar)
        {
            if (inArgvs == null) return false;
            for (int i = 0; i < inArgvs.Length; ++i)
            {
                if (inArgvs[i].variable == pVar) return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool HasOutArgv(Variable pVar)
        {
            if (outArgvs == null) return false;
            for (int i = 0; i < outArgvs.Length; ++i)
            {
                if (outArgvs[i].variable == pVar) return true;
            }
            return false;
        }
        //------------------------------------------------------
        public int IndexofInArgv(Variable pVar)
        {
            if (inArgvs == null) return -1;
            for (int i = 0; i < inArgvs.Length; ++i)
            {
                if (inArgvs[i].variable == pVar) return i;
            }
            return -1;
        }
        //------------------------------------------------------
        public int IndexofInArgvLink(ExcudeNode pNode)
        {
            if (inArgvs == null) return -1;
            for (int i = 0; i < inArgvs.Length; ++i)
            {
                VariableDelegate deleageVar = inArgvs[i].variable as VariableDelegate;
                if(deleageVar!=null )
                {
                    for(int j =0; j < deleageVar.OutLink.linkNodes.Count; ++j)
                    {
                        if (deleageVar.OutLink.linkNodes[j].BindNode == pNode)
                            return i;
                    }
                }
            }
            return -1;
        }
        //------------------------------------------------------
        public int IndexofOutArgv(Variable pVar)
        {
            if (outArgvs == null) return -1;
            for (int i = 0; i < outArgvs.Length; ++i)
            {
                if (outArgvs[i].variable == pVar) return i;
            }
            return -1;
        }
        //------------------------------------------------------
        public int IndexofOutArgvLink(ExcudeNode pNode)
        {
            if (outArgvs == null) return -1;
            for (int i = 0; i < outArgvs.Length; ++i)
            {
                VariableDelegate deleageVar = outArgvs[i].variable as VariableDelegate;
                if (deleageVar != null)
                {
                    for (int j = 0; j < deleageVar.OutLink.linkNodes.Count; ++j)
                    {
                        if (deleageVar.OutLink.linkNodes[j].BindNode == pNode)
                            return i;
                    }
                }
            }
            return -1;
        }
        //------------------------------------------------------
        public void ReplaceInArgv(AgentTree pTask, Variable oldVar, Variable newVar, int solt)
        {
            if (inArgvs == null || solt < 0 || solt >= inArgvs.Length) return;
            //int i = solt;
            //if (inArgvs[i].guid == oldVar.GUID)
            //{
            //    if (inArgvs[i].guid == newVar.GUID)
            //    {
            //        Variable pLocalNewVar = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(newVar.GetType());
            //        pLocalNewVar.Copy(newVar, false);
            //        inArgvs[i].guid = pLocalNewVar.GUID;
            //        if (i < m_InArgvs.Count) m_InArgvs[i] = pLocalNewVar;
            //    }
            //    else
            //    {
            //        inArgvs[i].guid = newVar.GUID;
            //        if (i < m_InArgvs.Count) m_InArgvs[i] = newVar;
            //    }

            //}
        }
        //------------------------------------------------------
        public void ReplaceOutArgv(AgentTree pTask, Variable oldVar, Variable newVar, int solt)
        {
            if (outArgvs == null || solt >= outArgvs.Length) return;
            //int i = solt;
            //if (outArgvs[i] == oldVar.GUID)
            //{
            //    if (outArgvs[i] == newVar.GUID)
            //    {
            //        Variable pLocalNewVar = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(newVar.GetType());
            //        pLocalNewVar.Copy(newVar, false);
            //        outArgvs[i] = pLocalNewVar.GUID;
            //        if (i < m_OutArgvs.Count) m_OutArgvs[i] = pLocalNewVar;
            //    }
            //    else
            //    {
            //        outArgvs[i] = newVar.GUID;
            //        if (i < m_OutArgvs.Count) m_OutArgvs[i] = newVar;
            //    }
            //}
        }
        //------------------------------------------------------
        [System.NonSerialized]
        public bool bSaved = false;
        public override void Save()
        {
            if (bSaved) return;
            bSaved = true;
            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                    inArgvs[i].Save();
            }
            if (outArgvs != null)
            {
                for (int i = 0; i < outArgvs.Length; ++i)
                    outArgvs[i].Save();
            }

            if (nextActions != null && nextActions.Length > 0)
            {
                List<int> nextGuids = new List<int>();
                for (int i = 0; i < nextActions.Length; ++i)
                {
                    if (nextActions[i].GUID != this.GUID)
                    {
                        nextActions[i].Save();
                        nextGuids.Add(nextActions[i].GUID);
                    }
                }
                nextActionsID = nextGuids.ToArray();
            }
            else nextActionsID = null;

            OnSave();
        }
        //------------------------------------------------------
        protected abstract void OnSave();
        public virtual void SetExcudeHash(int hash) { }


        [System.NonSerialized]
        protected ATEditorAttrData m_pEditAttrData = null;
        public abstract T GetEditAttrData<T>() where T : ATEditorAttrData;
        public abstract string ToTips(ArgvPort port);
        public abstract string ToTitleTips();

#endif
    }
}
