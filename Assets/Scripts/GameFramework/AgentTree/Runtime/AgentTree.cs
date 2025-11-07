/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AgentTree
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public struct RuntimeStruct
    {
        public List<Variable> members;
        public bool IsValid(int index)
        {
            return members != null && index < members.Count && index >= 0;
        }
        public Variable GetIndex(int index)
        {
            if (index < 0 || members == null || index >= members.Count) return null;
            return members[index];
        }

        public static RuntimeStruct DEFAULT = new RuntimeStruct() { members = null };
    }
    public class AgentTree
    {
        public delegate Variable OnGetVariableFunc(int guid);
        public OnGetVariableFunc onGetVariable;

        AgentTreeCoreData m_pATCoreData = null;

        Dictionary<int, ExcudeNode> m_vNodes = new Dictionary<int, ExcudeNode>(16);
        Dictionary<int, Variable> m_Variables = new Dictionary<int, Variable>(16);
#if UNITY_EDITOR
        public AAgentTreeData pSoAT = null;
        public AgentTreeCoreData CoreData
        {
            get { return m_pATCoreData; }
        }
        public Dictionary<int, Variable> variables
        {
            get { return m_Variables; }
        }
        public bool IsContains(int guid)
        {
            for(int i = 0; i < m_vTask.Count; ++i)
            {
                if (m_vTask[i].IsContains(guid)) return true;
            }
            return false;
        }
#endif

        private bool m_bEnable = false;
        public int Guid { get; set; }

        private AgentTreeTask m_pStartTask = null;
        private List<AgentTreeTask> m_vTickTasks = null;
        private AgentTreeTask m_pExitTask = null;

        private Dictionary<int, IUserData> m_OwnerClass = null;
        private Dictionary<int, IUserData> m_OwnerParentClass = null;

        private AgentTreeTask m_pCurExcudingTask = null;
        private List<AgentTreeTask> m_vTasking = new List<AgentTreeTask>(1);
        private List<APINode> m_vAPINodes = new List<APINode>(2);

        private Dictionary<int, RefPort> m_vRefPorts = new Dictionary<int, RefPort>(2);

        private List<AgentTreeTask> m_vTask = new List<AgentTreeTask>(2);
        private Dictionary<string, RuntimeStruct> m_vStructData = new Dictionary<string, RuntimeStruct>(2);
        //------------------------------------------------------
        public IUserData GetOwnerClass(int hashCode)
        {
            if (m_OwnerClass == null) return null;
            IUserData userClass;
            if (m_OwnerClass.TryGetValue(hashCode, out userClass))
                return userClass;

            if(m_OwnerParentClass!=null)
            {
                if (m_OwnerParentClass.TryGetValue(hashCode, out userClass)) return userClass;
            }

            int parent = AgentTreeManager.getInstance().GetParentHashCode(hashCode);
            while(parent != 0)
            {
                if (m_OwnerClass.TryGetValue(parent, out userClass))
                    return userClass;
                parent = AgentTreeManager.getInstance().GetParentHashCode(parent);
            }
            return null;
        }
        //------------------------------------------------------
        public void AddOwnerClass(IUserData pOwner, int hashCode = 0)
        {
            if (pOwner == null) return;
            if (hashCode == 0) hashCode = AgentTreeManager.getInstance().GetClassHashCode(pOwner.GetType());
            if (m_OwnerClass == null) m_OwnerClass = new Dictionary<int, IUserData>(2);
            if (m_OwnerParentClass == null) m_OwnerParentClass = new Dictionary<int, IUserData>(2);
            m_OwnerClass[hashCode] = pOwner;

            int parent = AgentTreeManager.getInstance().GetParentHashCode(hashCode);
            while (parent != 0)
            {
                m_OwnerParentClass[parent] = pOwner;
                parent = AgentTreeManager.getInstance().GetParentHashCode(parent);
            }
        }
        //------------------------------------------------------
        public ExcudeNode GetExcudeNode(int guid)
        {
            ExcudeNode node;
            if (m_vNodes.TryGetValue(guid, out node)) return node;
            return null;
        }
        //------------------------------------------------------
        public T GetVariable<T>(int guid) where T : Variable
        {
            Variable tOut = null;
            if (m_Variables.TryGetValue(guid, out tOut))
            {
                return tOut as T;
            }
            T var = AgentTreeManager.getInstance().GetVariableFactory().GetGlobalVariable<T>(guid);
            if (var == null && onGetVariable != null)
                return onGetVariable(guid) as T;
            return null;
        }
        //------------------------------------------------------
        public RuntimeStruct GetStruct(string name)
        {
            RuntimeStruct retStruct;
            if (m_vStructData.TryGetValue(name, out retStruct))
                return retStruct;
            return RuntimeStruct.DEFAULT;
        }
        //------------------------------------------------------
        public void Init(AgentTreeCoreData pATData, AAgentTreeData pSoData = null)
        {
            Clear();
            if (pATData == null && pSoData == null) return;
            if (pATData == null) pATData = pSoData.Data;
#if UNITY_EDITOR
            pSoAT = pSoData;
#endif
            m_pATCoreData = pATData;

            if (m_pATCoreData == null) return;

            int maxGuid = 0;
            if (m_pATCoreData.Locals!=null)
                m_pATCoreData.Locals.InitRuntime(m_Variables, ref maxGuid);
            if(m_pATCoreData.vNodes!=null)
            {
                foreach (var db in m_pATCoreData.vNodes)
                    m_vNodes.Add(db.GUID, db);
            }
            if (m_pATCoreData.vATApis != null)
            {
                foreach (var db in m_pATCoreData.vATApis)
                {
                    m_vNodes.Add(db.GUID, db);
                    if(db.GetExcudeHash()!=0)
                        m_vAPINodes.Add(db);
                }
            }
            if (m_pATCoreData.RefPorts != null)
            {
                foreach (var db in m_pATCoreData.RefPorts)
                    m_vRefPorts.Add(db.id, db);
            }

            foreach (var db in m_Variables)
                db.Value.Init(this);

            foreach (var db in m_vNodes)
                db.Value.Init(this);

            foreach (var db in m_vRefPorts)
                db.Value.Init(this);

            AgentTreeManager.AdjustMaxGUID(maxGuid);

            if(m_pATCoreData.Tasks != null)
            {
                for (int i =0; i< m_pATCoreData.Tasks.Count; ++i)
                {
                    AddTask(m_pATCoreData.Tasks[i], i);
                }
            }
            if(m_pATCoreData.StructDatas!=null)
            {
                StructData structData;
                for(int i = 0; i < m_pATCoreData.StructDatas.Count; ++i)
                {
                    structData = m_pATCoreData.StructDatas[i];
                    if (structData.variables == null || structData.variables.Count <= 0) continue;
                    RuntimeStruct runtimeStruct = new RuntimeStruct();
                    runtimeStruct.members = new List<Variable>(structData.variables.Count);
                    for(int j = 0; j < structData.variables.Count; ++j)
                    {
                        Variable var = GetVariable<Variable>(structData.variables[j]);
                        if(var!=null) runtimeStruct.members.Add(var);
                    }
                    if(runtimeStruct.members.Count>0)
                        m_vStructData.Add(m_pATCoreData.StructDatas[i].structName, runtimeStruct);
                }
            }
            m_bEnable = pATData.bEnable;
        }
        //------------------------------------------------------
        public void Enable(bool bEnable)
        {
            m_bEnable = bEnable;
        }
        //------------------------------------------------------
        public bool IsEnable()
        {
            return m_bEnable;
        }
        //------------------------------------------------------
        public void AddTask(Task pTask, int index)
        {
            for(int i =0; i < m_vTask.Count; ++i)
            {
                if (m_vTask[i].taskData == pTask) return;
            }
            AgentTreeTask task = AgentTreeManager.getInstance().NewTask(this, pTask, index);
            m_vTask.Add(task);
            AgentTreeManager.getInstance().OnAddTask(task);
            if (pTask.type == ETaskType.Start)
                m_pStartTask = task;
            else if (pTask.type == ETaskType.Exit)
                m_pExitTask = task;
            else if (pTask.type == ETaskType.Tick)
            {
                if (m_vTickTasks == null) m_vTickTasks = new List<AgentTreeTask>(2);
                m_vTickTasks.Add(task);
            }
        }
        //------------------------------------------------------
        public AgentTreeTask GetTask(int index)
        {
            return m_vTask[index];
        }
        //------------------------------------------------------
        public RefPort GetRefPort(int id)
        {
            RefPort refPort;
            if (m_vRefPorts.TryGetValue(id, out refPort))
                return refPort;
            return null;
        }
        //------------------------------------------------------
        public bool ExecuteAPI(IUserData userData)
        {
            if (!m_bEnable) return false;
            int hashCode  = AgentTreeManager.getInstance().GetClassHashCode(userData.GetType());
            if (hashCode == 0)
                return false;
            for(int i =0; i < m_vAPINodes.Count; ++i)
            {
                var apiNode = m_vAPINodes[i];
                if (apiNode.GetExcudeHash() != hashCode)
                    continue;
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteCustom(int nID, IUserData pParam = null)
        {
            if (!m_bEnable || nID<=0) return false;
            if (m_vTask != null)
            {
                for(int i = 0; i < m_vTask.Count; ++i)
                {
                    if (m_vTask[i].taskData == null) continue;
                    if (m_vTask[i].taskData.EnterNode == null) continue;
                    if(m_vTask[i].taskData.EnterNode.CustomID == nID)
                    {
                        ExecuteCustom(m_vTask[i], pParam);
                        return true;
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteCustom(AgentTreeTask pTask, IUserData pParam = null)
        {
            if (!m_bEnable) return false;
            if (pTask != null)
            {
                pTask.Reset();
                if (!m_vTasking.Contains(pTask))
                {
                    m_vTasking.Add(pTask);
                    m_pCurExcudingTask = pTask;
                    pTask.Play(pParam);
                }
                else
                {
                    m_pCurExcudingTask = pTask;
                    pTask.Play(pParam);
                }
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteCustom(string strName, IUserData pParam = null)
        {
            if (!m_bEnable || string.IsNullOrEmpty(strName)) return false;
            if (m_vTask != null)
            {
                for (int i = 0; i < m_vTask.Count; ++i)
                {
                    if (m_vTask[i].taskData == null) continue;
                    if (m_vTask[i].taskData.EnterNode == null) continue;
                    if (strName.CompareTo(m_vTask[i].taskData.EnterNode.CustomName)==0)
                    {
                        ExecuteCustom(m_vTask[i], pParam);
                        return true;
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteCustom(GameObject pGo, IUserData pParam = null)
        {
            if (!m_bEnable || pGo == null) return false;
            if (m_vTask != null)
            {
                for (int i = 0; i < m_vTask.Count; ++i)
                {
                    if (m_vTask[i].taskData == null) continue;
                    if (m_vTask[i].taskData.EnterNode == null) continue;
                    if (pGo == m_vTask[i].taskData.EnterNode.CustomGO)
                    {
                        ExecuteCustom(m_vTask[i], pParam);
                        return true;
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteCustom(ushort enterType, IUserData pParam = null)
        {
            if (!m_bEnable) return false;
            bool bExcude = false;
            if (m_vTask != null)
            {
                for (int i = 0; i < m_vTask.Count; ++i)
                {
                    if (m_vTask[i].taskData == null) continue;
                    if (m_vTask[i].taskData.EnterNode == null) continue;
                    if (m_vTask[i].taskData.EnterNode.EnterType == enterType)
                    {
                        if (ExecuteCustom(m_vTask[i], pParam))
                            bExcude = true;
                    }
                }
            }
            return bExcude;
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort type, int nID, IUserData pParam = null)
        {
            if (!m_bEnable) return false;
            return AgentTreeManager.getInstance().ExecuteEvent(type, nID, pParam);
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort type, string strName, IUserData pParam = null)
        {
            if (!m_bEnable) return false;
            return AgentTreeManager.getInstance().ExecuteEvent(type, strName, pParam);
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort type, UnityEngine.GameObject handler, IUserData pParam = null)
        {
            if (!m_bEnable) return false;
            return AgentTreeManager.getInstance().ExecuteEvent(type, handler, pParam);
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_bEnable = false;
#if UNITY_EDITOR
            pSoAT = null;
#endif
            m_pATCoreData = null;
            foreach(var db in m_vTask)
            {
          //      AgentTreeManager.getInstance().OnRemoveTask(db);
                db.Clear();
            }
            m_vTask.Clear();
            foreach (var db in m_Variables)
            {
                db.Value.Destroy();
            }
            m_Variables.Clear();

            foreach (var db in m_vNodes)
            {
                db.Value.Destroy();
            }
            m_vNodes.Clear();
            m_vTasking.Clear();
            m_vAPINodes.Clear();
            m_pCurExcudingTask = null;
            m_pStartTask = null;
            m_pExitTask = null;
            if (m_vTickTasks != null) m_vTickTasks.Clear();

            m_vStructData.Clear();
        }
        //------------------------------------------------------
        public void Enter()
        {
            if (!m_bEnable) return;
            m_pCurExcudingTask = null;
            if (m_pStartTask != null && !m_pStartTask.IsPlaying && !m_vTasking.Contains(m_pStartTask))
            {
                foreach (var db in m_vTask)
                    db.Reset();
                m_vTasking.Clear();

                m_pStartTask.Play();
                m_vTasking.Add(m_pStartTask);
            }
        }
        //------------------------------------------------------
        public void Exit()
        {
            if (!m_bEnable) return;
            if (m_pCurExcudingTask != null && m_pCurExcudingTask.IsPlaying)
                m_pCurExcudingTask.ContinueNext();
            m_pCurExcudingTask = null;
            if (m_pExitTask != null && !m_pExitTask.IsPlaying)
            {
                m_pExitTask.Play();
           //     m_pExitTask.Update(0);
            }
            foreach (var db in m_vTask)
                db.Reset();
        }
        //------------------------------------------------------
        public void Destroy()
        {
            foreach (var db in m_vTask)
            {
                AgentTreeManager.getInstance().OnRemoveTask(db);
                db.Clear();
            }
            foreach (var db in m_Variables)
            {
                db.Value.Destroy();
            }
            if(m_OwnerClass!=null) m_OwnerClass.Clear();
            if (m_OwnerParentClass != null) m_OwnerParentClass.Clear();
        }
        //------------------------------------------------------
        public void Update(float fFrame)
        {
            if (!m_bEnable) return;

            if(m_vTickTasks != null)
            {
                AgentTreeTask pTask;
                for(int i =0; i < m_vTickTasks.Count; ++i)
                {
                    pTask = m_vTickTasks[i];
                    if(!pTask.Update(fFrame))
                    {
                        pTask.Play();
                    }
                }
            }

            m_pCurExcudingTask = null;
            for (int i =0; i < m_vTasking.Count;)
            {
                m_pCurExcudingTask = m_vTasking[i];
                if (!m_pCurExcudingTask.Update(fFrame))
                {
                    m_vTasking.RemoveAt(i);
                }
                else
                    ++i;
            }
            m_pCurExcudingTask = null;
        }
    }

}
