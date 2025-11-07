/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	Port
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    [System.Serializable]
    public struct DummyPort
    {
        public int actionGuid;
        public int varGuid;
    }
    [System.Serializable]
    public class Port
    {
        public int guid = 0;
        public int refPortID = 0;
        public List<DummyPort> dummys = null;

        [System.NonSerialized]
        public Variable variable = null;

        [System.NonSerialized]
        public RefPort pRefPort = null;

        [System.NonSerialized]
        public Dictionary<int, Variable> dummyMap = null;

        [System.NonSerialized]
        public Dictionary<long, Variable> taskDummyCatch = null;

#if UNITY_EDITOR
        [System.NonSerialized]
        public IPortEditor Editorer;
        [System.NonSerialized]
        public IPortEditor Editorer1;
        //------------------------------------------------------
        public T GetEditorer<T>(int index=0) where T : IPortEditor, new()
        {
            if(index == 1)
            {
                if (Editorer1 == null) Editorer1 = new T();
                return (T)Editorer1;
            }
            if (Editorer == null) Editorer = new T();
            return (T)Editorer;
        }
        //------------------------------------------------------
        public void AddDummy(int actionGuid, Variable pVar)
        {
            if (actionGuid == -1) return;
            if (dummyMap == null) dummyMap = new Dictionary<int, Variable>();
            if (pVar == null) dummyMap.Remove(actionGuid);
            else
                dummyMap[actionGuid] = pVar;
        }
        //------------------------------------------------------
        public void RemoveDummy(int actionGuid)
        {
            if (dummyMap != null)
                dummyMap.Remove(actionGuid);
        }
        //------------------------------------------------------
        public bool HasDummy(int actionGuid)
        {
            if (dummyMap == null) return false;
            return dummyMap.ContainsKey(actionGuid);
        }
        //------------------------------------------------------
        public bool HasDummy(int actionGuid, int variable)
        {
            if (dummyMap == null) return false;
            Variable dumy = null;
            if(dummyMap.TryGetValue(actionGuid, out dumy) && dumy!=null && dumy.GUID == variable)
            {
                return true;
            }
            return false;
        }
#endif
        //------------------------------------------------------
        public Port(Variable var)
        {
            guid = var!=null?var.GUID:0;
            variable = var;
            dummys = null;
            dummyMap = null;
        }
        //------------------------------------------------------
        public void Init(AgentTree pTree)
        {
            if (taskDummyCatch != null) taskDummyCatch.Clear();
             variable = pTree.GetVariable<Variable>(guid);
            if (dummys != null)
            {
                if (dummyMap == null) dummyMap = new Dictionary<int, Variable>();
                for (int i = 0; i < dummys.Count; ++i)
                {
                    Variable dumVar = pTree.GetVariable<Variable>(dummys[i].varGuid);
                    if(dumVar == null) continue;
                    dummyMap[dummys[i].actionGuid] = dumVar;
                }
            }
            if(refPortID != 0)
                pRefPort = pTree.GetRefPort(refPortID);
        }
        //------------------------------------------------------
        public T GetVariable<T>(AgentTreeTask pTask = null) where T : Variable
        {
            Variable var = GetVariable(pTask);
            if(var!=null && var is VariableDelegate)
            {
                ((VariableDelegate)var).SetTreeTask(pTask);
            }
            return var as T;
        }
        //------------------------------------------------------
        public Variable GetVariable(AgentTreeTask pTask = null)
        {
            if (dummyMap == null || dummyMap.Count <= 0 || pTask == null)
            {
                if (pRefPort != null && pRefPort.IsValid()) return pRefPort.GetVariable();
                return variable;
            }
            List<int> excudeList = pTask.GetExcudedReturnList();
            if (excudeList.Count > 0)
            {
                Variable outVar;
                if (taskDummyCatch != null && taskDummyCatch.TryGetValue(pTask.GetID(), out outVar))
                {
                    if (outVar != null) outVar.DoFill();
                    return outVar;
                }
                for (int i = excudeList.Count - 1; i >= 0; i--)
                {
                    if (dummyMap.TryGetValue(excudeList[i], out outVar))
                    {
                        if(taskDummyCatch == null) taskDummyCatch = new Dictionary<long, Variable>(dummyMap.Count);
                        taskDummyCatch[pTask.GetID()] = outVar;
                        if (outVar != null) outVar.DoFill();
                        return outVar;
                    }
                }
            }
            else
            {
                foreach(var db in dummyMap)
                {
                    if (db.Value.IsFlag(EFlag.Declaration))
                    {
                        if (db.Value != null) db.Value.DoFill();
                        return db.Value;
                    }
                }
            }
            if (pRefPort != null && pRefPort.IsValid()) return pRefPort.GetVariable();
            if (variable != null) variable.DoFill();
            return variable;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            if (variable != null) variable.Destroy();
            if(dummyMap!=null)
            {
                foreach (var db in dummyMap)
                {
                    db.Value.Destroy();
                }
            }
        }
        //------------------------------------------------------
        public void Reset()
        {
            if (variable != null) variable.Reset(AgentTreeUtl.intSet);
            if (dummyMap != null)
            {
                foreach (var db in dummyMap)
                {
                    db.Value.Reset(AgentTreeUtl.intSet);
                }
            }
        }
        //------------------------------------------------------
        public void Copy(Port port, bool bIncludeGuid = true)
        {
            if(bIncludeGuid) guid = port.guid;
            variable = port.variable;
            refPortID = port.refPortID;
            pRefPort = port.pRefPort;
            if(port.dummys != null)
            {
                dummys = new List<DummyPort>(port.dummys.ToArray());
            }
            if(port.dummyMap != null)
            {
                dummyMap = new Dictionary<int, Variable>();
                foreach (var db in port.dummyMap)
                {
                    dummyMap[db.Key] = db.Value;
                }
            }
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        public void Save()
        {
            if (variable != null)
            {
                guid = variable.GUID;
            }
            else guid = 0;

            if (dummyMap != null)
            {
                dummys = new List<DummyPort>();
                foreach (var db in dummyMap)
                {
                    if (db.Value == null) continue;
                    dummys.Add(new DummyPort() { varGuid = db.Value.GUID, actionGuid = db.Key });
                }
            }
            else dummys = null;

            refPortID = 0;
            if (pRefPort != null)
                refPortID = pRefPort.id;
        }
#endif
    }

    [System.Serializable]
    public class RefPort
    {
        public int id;
        public int varGuid;
        public Rect rect;

        [System.NonSerialized]
        Variable m_pVariable = null;
        public Variable GetVariable()
        {
            return m_pVariable;
        }

        public bool IsValid()
        {
            return m_pVariable != null;
        }

        public void Init(AgentTree pTree)
        {
            if(pTree!=null && varGuid !=0)
            {
                m_pVariable = pTree.GetVariable<Variable>(varGuid);
            }
        }
#if UNITY_EDITOR
        public void SetVariable(Variable pVar)
        {
            m_pVariable = pVar;
        }
        public void Save(HashSet<Variable> vVarMaps )
        {
            varGuid = 0;
            if (m_pVariable!=null && vVarMaps.Contains(m_pVariable))
            {
                varGuid = m_pVariable.GUID;
            }
            else
                m_pVariable = null;
        }
#endif  
    }

    [System.Serializable]
    public class TransferDot
    {
        public long key;
        public List<Vector2> offsetDots;
    }

}