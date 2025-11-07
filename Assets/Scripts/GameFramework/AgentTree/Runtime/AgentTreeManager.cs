/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AgentTreeManager
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public interface AgentTreeDoActionCallback
    {
        bool OnATAction(AgentTreeTask pTask, ActionNode pNode, int nFuncId);
        bool OnATFillCustomVariable(AgentTreeTask pTask, Variable pCustomVariable, IUserData pData);
        int GetATClassHashCode(System.Type type);
        System.Type GetATHashCodeClass(int hasCode);
        int GetATParentHashCode(int hasCode);
    }
    public partial class AgentTreeManager
    {
        protected struct KeyListen
        {
            public KeyCode keyCode;
            public ushort useRef;
        }
        protected class EventData
        {
            public Dictionary<int, HashSet<AgentTreeTask>> Events;
            public Dictionary<string, HashSet<AgentTreeTask>> EventByNames;
            public Dictionary<GameObject, HashSet<AgentTreeTask>> EventByGOs;
        }
        class ObjectPool<T> where T : new()
        {
            private readonly Stack<T> m_Stack = new Stack<T>();

            public int countAll { get; private set; }
            public int countActive { get { return countAll - countInactive; } }
            public int countInactive { get { return m_Stack.Count; } }

            public ObjectPool()
            {
            }

            public T Get()
            {
                T element;
                if (m_Stack.Count == 0)
                {
                    element = new T();
                    countAll++;
                }
                else
                {
                    element = m_Stack.Pop();
                }
                return element;
            }

            public void Release(T element)
            {
                if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                    AgentTreeUtl.LogError("Internal error. Trying to destroy object that is already released to pool.");
                m_Stack.Push(element);
            }
        }

        private static AgentTreeManager ms_pInstance = null;
        public static AgentTreeManager getInstance()
        {
            if (ms_pInstance == null)
                ms_pInstance = new AgentTreeManager();
            return ms_pInstance;
        }

        private static int m_nATGUID = 100;
        private static int m_nAutoGUID = 5000;
        private static int MAX_TASK_POOL_NUM = 64;

        ObjectPool<AgentTree> m_Pools = new ObjectPool<AgentTree>();
        Stack<AgentTreeTask> m_PoolsTasks = new Stack<AgentTreeTask>(MAX_TASK_POOL_NUM);
        Dictionary<int, AgentTree> m_vAgents = null;

        List<AgentTree> m_vLoadingAgent = null;
        List<int> m_vUnloadingAgent = null;

        VariableFactory m_pVariableFactory = null;
        Pools<AgentTree> m_vRecycle = null;

        List<AgentTreeDoActionCallback> m_vCallback = null;

        private List<AgentTreeTask> m_vDoingTask = new List<AgentTreeTask>(8);
        private Dictionary<ushort, EventData> m_vCustomTask = new Dictionary<ushort, EventData>(2);
        private HashSet<AgentTreeTask> m_vKeyInputTask = null;
        private HashSet<AgentTreeTask> m_vMouseInputTask = null;

        private List<KeyListen> m_vKeyListens = null;

        List<AgentTreeTask> m_vDestroyingTask = new List<AgentTreeTask>();

        Dictionary<int, IUserData> m_ModuleMaps = new Dictionary<int, IUserData>();
#if UNITY_EDITOR
        public static System.Action<AgentTreeTask, ActionNode> OnExcudeAction;
        public Dictionary<int, AgentTree> Agents
        {
            get { return m_vAgents; }
        }
#endif
        //------------------------------------------------------
        public static int AutoGUID()
        {
            return ++m_nAutoGUID;
        }
        //------------------------------------------------------
        public static void AdjustMaxGUID(int guid, bool bSet = false)
        {
            if (bSet) m_nAutoGUID = guid;
            else
                m_nAutoGUID = UnityEngine.Mathf.Max(m_nAutoGUID, guid);
        }
        //------------------------------------------------------
        public static void ClearAutoGUID()
        {
            m_nAutoGUID = 5000;
            m_nATGUID = 100;
        }
        //------------------------------------------------------
        AgentTreeManager()
        {
            m_vRecycle = new Pools<AgentTree>(MAX_TASK_POOL_NUM);
            m_vAgents = new Dictionary<int, AgentTree>(MAX_TASK_POOL_NUM);
            m_pVariableFactory = new VariableFactory();
            //  m_pActionFactory = new AT_Pools<AT_Action>(32);
            m_vCallback = new List<AgentTreeDoActionCallback>(2);

            m_vLoadingAgent = new List<AgentTree>(4);
            m_vUnloadingAgent = new List<int>(4);

            m_vCustomTask.Clear();
            m_vDoingTask.Clear();
            ClearAutoGUID();
        }
        //------------------------------------------------------
        ~AgentTreeManager()
        {
            m_vAgents = null;
            m_pVariableFactory = null;
            ClearAutoGUID();
            m_vCustomTask = null;
            m_vRecycle = null;
            m_vDoingTask = null;

            m_vKeyInputTask = null;
            m_vMouseInputTask = null;
            m_vKeyListens = null;

            m_vLoadingAgent = null;
            m_vUnloadingAgent = null;
        }
        //------------------------------------------------------
        public IUserData FindUserClass(int hashCode, AgentTree pAT = null)
        {
            if (hashCode == 0) return null;
            
            IUserData classData;
            if (pAT != null)
            {
                classData = pAT.GetOwnerClass(hashCode);
                if (classData != null) return classData;
            }
            if (m_ModuleMaps.TryGetValue(hashCode, out classData))
                return classData;
            return null;
        }
        //------------------------------------------------------
        public void RegisterClass(IUserData classData, int hashCode = 0)
        {
            if (classData == null) return;
            if (hashCode == 0) hashCode = GetClassHashCode(classData.GetType());
            if (m_ModuleMaps.ContainsKey(hashCode)) return;
            m_ModuleMaps[hashCode] = classData;
        }
        //------------------------------------------------------
        public void UnRegisterClass(IUserData classData, int hashCode = 0)
        {
            if (classData == null) return;
            if (hashCode == 0) hashCode = GetClassHashCode(classData.GetType());
            m_ModuleMaps.Remove(hashCode);
        }
        //------------------------------------------------------
        public VariableFactory GetVariableFactory()
        {
            return m_pVariableFactory;
        }
        //------------------------------------------------------
        public void RecycelVars(Variable pVar)
        {
            m_pVariableFactory.Recycel(pVar);
        }
        //------------------------------------------------------
        public Variable NewVariableByType(System.Type type, int nGUID = 0, bool bEditor = false)
        {
            return m_pVariableFactory.NewVariableByType(type, nGUID, bEditor);
        }
        //------------------------------------------------------
        public void Update(float fFrameTime)
        {
            if (m_vAgents == null) return;

            if (m_vLoadingAgent != null)
            {
                for (int i = 0; i < m_vLoadingAgent.Count; ++i)
                {
                    m_vAgents[m_vLoadingAgent[i].Guid] = m_vLoadingAgent[i];
                }
                m_vLoadingAgent.Clear();
            }
            if (m_vUnloadingAgent != null)
            {
                for (int i = 0; i < m_vUnloadingAgent.Count; ++i)
                    m_vAgents.Remove(m_vUnloadingAgent[i]);
                m_vUnloadingAgent.Clear();
            }

            foreach (var db in m_vAgents)
            {
                db.Value.Update(fFrameTime);
            }

            if(m_vKeyListens != null)
            {
                KeyListen keyListen;
                for (int i =0; i < m_vKeyListens.Count; ++i)
                {
                    keyListen = m_vKeyListens[i];
                    if (keyListen.useRef <= 0)
                        continue;
                    if (Input.GetKeyDown(keyListen.keyCode)) KeyInputEvent(keyListen.keyCode, true);
                    else if (Input.GetKeyUp(keyListen.keyCode)) KeyInputEvent(keyListen.keyCode, false);
                }
            }

            if(m_vDestroyingTask.Count>0)
            {
                AgentTreeTask task;
                for (int i = 0; i < m_vDestroyingTask.Count; ++i)
                {
                    task = m_vDestroyingTask[i];
                    DelTask(task);
                    if(m_PoolsTasks.Count< MAX_TASK_POOL_NUM) m_PoolsTasks.Push(task);
                }
                m_vDestroyingTask.Clear();
            }
        }
        //------------------------------------------------------
        public AgentTree LoadAT(AAgentTreeData pAT)
        {
            if (pAT == null) return null;
            return LoadAT(pAT.Data, pAT);
        }
        //------------------------------------------------------
        public AgentTree LoadAT(AgentTreeCoreData pAT)
        {
            if (pAT == null) return null;

            return LoadAT(pAT, null);
        }
        //------------------------------------------------------
        public AgentTree LoadAT(AgentTreeCoreData pAT, AAgentTreeData pSoAT)
        {
            if (pAT == null || !pAT.bEnable) return null;
            AgentTree pAgent = m_Pools.Get();
            pAgent.Guid = m_nATGUID++;
            pAgent.Init(pAT, pSoAT);
           if(m_vLoadingAgent!=null)
                m_vLoadingAgent.Add(pAgent);
           else
                m_vAgents.Add(pAgent.Guid, pAgent);
            return pAgent;
        }
        //------------------------------------------------------
        public void UnloadAT(AgentTree pAT)
        {
            if (pAT == null) return;

            if(m_vUnloadingAgent!=null)
                m_vUnloadingAgent.Add(pAT.Guid);
            else
            {
                m_vAgents.Remove(pAT.Guid);
            }
            pAT.Destroy();
            m_Pools.Release(pAT);
        }
        //------------------------------------------------------
        public AgentTree GetAT(int guid)
        {
            AgentTree pAT;
            if (m_vAgents.TryGetValue(guid, out pAT)) return pAT;

            if(m_vLoadingAgent!=null)
            {
                for (int i = 0; i < m_vLoadingAgent.Count; ++i)
                {
                    if (m_vLoadingAgent[i].Guid == guid) return m_vLoadingAgent[i];
                }
            }

            return null;
        }
        //------------------------------------------------------
        public void Shutdown()
        {
            if(m_vAgents!=null)
            {
                foreach (var db in m_vAgents)
                {
                    db.Value.Destroy();
                }

                m_vAgents.Clear();
            }
            ClearAutoGUID();
            m_vDestroyingTask.Clear();
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort evenType, UnityEngine.GameObject handler, IUserData pParam = null)
        {
            if (handler == null) return false;
            EventData vData;
            if (!m_vCustomTask.TryGetValue(evenType, out vData) || vData.EventByGOs == null) return false;

            HashSet<AgentTreeTask> vTask;
            if (vData.EventByGOs.TryGetValue(handler, out vTask))
            {
                m_vDoingTask.Clear();
                bool bDo = false;
                foreach (var db in vTask)
                {
                    if (db.IsEnabled)
                        m_vDoingTask.Add(db);
                }
                for(int i = 0; i < m_vDoingTask.Count; ++i)
                {
                    if (m_vDoingTask[i].pAT.ExecuteCustom(m_vDoingTask[i], pParam))
                    {
                        bDo = true;
                    }
                }
                m_vDoingTask.Clear();
                return bDo;
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort evenType, int handler, IUserData pParam = null)
        {
            if (handler == 0) return false;
            EventData vData;
            if (!m_vCustomTask.TryGetValue(evenType, out vData) || vData.Events == null) return false;

            HashSet<AgentTreeTask> vTask;
            if (vData.Events.TryGetValue(handler, out vTask))
            {
                m_vDoingTask.Clear();
                bool bDo = false;
                foreach (var db in vTask)
                {
                    if (db.IsEnabled)
                    {
                        m_vDoingTask.Add(db);
                    }
                }
                for (int i = 0; i < m_vDoingTask.Count; ++i)
                {
                    if (m_vDoingTask[i].pAT.ExecuteCustom(m_vDoingTask[i], pParam))
                    {
                        bDo = true;
                    }
                }
                m_vDoingTask.Clear();
                return bDo;
            }
            return false;
        }
        //------------------------------------------------------
        public bool ExecuteEvent(ushort evenType, string handler, IUserData pParam = null)
        {
            if (string.IsNullOrEmpty(handler)) return false;
            EventData vData;
            if (!m_vCustomTask.TryGetValue(evenType, out vData) || vData.EventByNames == null) return false;

            HashSet<AgentTreeTask> vTask;
            if (vData.EventByNames.TryGetValue(handler, out vTask))
            {
                m_vDoingTask.Clear();
                bool bDo = false;
                foreach (var db in vTask)
                {
                    if (db.IsEnabled)
                    {
                        m_vDoingTask.Add(db);
                    }
                }
                for (int i = 0; i < m_vDoingTask.Count; ++i)
                {
                    if (m_vDoingTask[i].pAT.ExecuteCustom(m_vDoingTask[i], pParam))
                    {
                        bDo = true;
                    }
                }
                m_vDoingTask.Clear();
                return bDo;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool Execute(ushort evenType, UnityEngine.GameObject handler, IUserData pParam = null)
        {
            if (getInstance() == null) return false;
            return getInstance().ExecuteEvent(evenType, handler, pParam);
        }
        //------------------------------------------------------
        public static bool Execute(ushort evenType, int handler, IUserData pParam = null)
        {
            if (getInstance() == null) return false;
            return getInstance().ExecuteEvent(evenType, handler, pParam);
        }
        //------------------------------------------------------
        public static bool Execute(ushort evenType, string handler, IUserData pParam = null)
        {
            if (getInstance() == null) return false;
            return getInstance().ExecuteEvent(evenType, handler, pParam);
        }
        //------------------------------------------------------
        public bool KeyInputEvent(KeyCode keyCode, bool bState)
        {
            if (m_vKeyInputTask == null)
                return false;
            m_vDoingTask.Clear();
            bool bDo = false;
            foreach (var db in m_vKeyInputTask)
            {
                if (!db.IsEnabled) continue;
                if (db.taskData.EnterNode.EnterType != (int)keyCode)
                    continue;
                m_vDoingTask.Add(db);
            }
            if (m_vDoingTask.Count <= 0)
                return false;
            Framework.Core.Variable1 variable = new Framework.Core.Variable1();
            variable.boolVal = bState;
            for (int i = 0; i < m_vDoingTask.Count; ++i)
            {
                if (m_vDoingTask[i].pAT.ExecuteCustom(m_vDoingTask[i], variable))
                {
                    bDo = true;
                }
            }
            m_vDoingTask.Clear();
            return bDo;
        }
        //------------------------------------------------------
        public bool MouseInputEvent(int touchId, ATMouseData mouseData)
        {
            if (m_vMouseInputTask == null)
                return false;
            m_vDoingTask.Clear();
            bool bDo = false;
            foreach (var db in m_vMouseInputTask)
            {
                if (!db.IsEnabled) continue;

                if (mouseData.state != EATMouseType.Wheel)
                {
                    if(db.taskData.EnterNode.EnterType != touchId)
                        continue;
                }
                m_vDoingTask.Add(db);
            }
            for (int i = 0; i < m_vDoingTask.Count; ++i)
            {
                if (m_vDoingTask[i].pAT.ExecuteCustom(m_vDoingTask[i], mouseData))
                {
                    bDo = true;
                }
            }
            m_vDoingTask.Clear();
            return bDo;
        }
        //------------------------------------------------------
        public void Register(AgentTreeDoActionCallback pCallback)
        {
            m_vCallback.Add(pCallback);
        }
        //------------------------------------------------------
        public void UnRegister(AgentTreeDoActionCallback pCallback)
        {
            m_vCallback.Remove(pCallback);
        }
        //------------------------------------------------------
        public AgentTreeTask NewTask(AgentTree pAT, Task pData, int index)
        {
            if (m_PoolsTasks.Count > 0)
            {
                AgentTreeTask task = m_PoolsTasks.Pop();
                task.SetData(pAT, pData, index);
                return task;
            }
            return new AgentTreeTask(pAT, pData, index);
        }
        //------------------------------------------------------
        public void OnAddTask(AgentTreeTask pTask)
        {
            if (pTask.taskData == null || pTask.taskData.EnterNode == null) return;
            if (pTask.taskData.type == ETaskType.Custom)
            {
                if (pTask.taskData.EnterNode.EnterType == 0)
                    return;

                EventData vData;
                if (!m_vCustomTask.TryGetValue(pTask.taskData.EnterNode.EnterType, out vData))
                {
                    vData = new EventData();
                    m_vCustomTask.Add(pTask.taskData.EnterNode.EnterType, vData);
                }

                if (pTask.taskData.EnterNode != null)
                {
                    if (pTask.taskData.EnterNode.CustomGO)
                    {
                        HashSet<AgentTreeTask> vTask;
                        if (vData.EventByGOs == null)
                        {
                            vData.EventByGOs = new Dictionary<GameObject, HashSet<AgentTreeTask>>();
                            vTask = new HashSet<AgentTreeTask>();
                            vData.EventByGOs.Add(pTask.taskData.EnterNode.CustomGO, vTask);
                        }
                        else if(!vData.EventByGOs.TryGetValue(pTask.taskData.EnterNode.CustomGO, out vTask))
                        {
                            vTask = new HashSet<AgentTreeTask>();
                            vData.EventByGOs.Add(pTask.taskData.EnterNode.CustomGO, vTask);
                        }

                        vTask.Add(pTask);
                    }
                    if (!string.IsNullOrEmpty(pTask.taskData.EnterNode.CustomName))
                    {
                        HashSet<AgentTreeTask> vTask;
                        if (vData.EventByNames == null)
                        {
                            vData.EventByNames = new Dictionary<string, HashSet<AgentTreeTask>>();
                            vTask = new HashSet<AgentTreeTask>();
                            vData.EventByNames.Add(pTask.taskData.EnterNode.CustomName, vTask);
                        }
                        else if (!vData.EventByNames.TryGetValue(pTask.taskData.EnterNode.CustomName, out vTask))
                        {
                            vTask = new HashSet<AgentTreeTask>();
                            vData.EventByNames.Add(pTask.taskData.EnterNode.CustomName, vTask);
                        }

                        vTask.Add(pTask);
                    }
                    if (pTask.taskData.EnterNode.CustomID != 0)
                    {
                        HashSet<AgentTreeTask> vTask;
                        if (vData.Events == null)
                        {
                            vData.Events = new Dictionary<int, HashSet<AgentTreeTask>>();
                            vTask = new HashSet<AgentTreeTask>();
                            vData.Events.Add(pTask.taskData.EnterNode.CustomID, vTask);
                        }
                        else if (!vData.Events.TryGetValue(pTask.taskData.EnterNode.CustomID, out vTask))
                        {
                            vTask = new HashSet<AgentTreeTask>();
                            vData.Events.Add(pTask.taskData.EnterNode.CustomID, vTask);
                        }

                        vTask.Add(pTask);
                    }
                }
            }
            else if(pTask.taskData.type == ETaskType.KeyInput)
            {
                if (pTask.taskData.EnterNode.EnterType == 0)
                    return;

                if (m_vKeyInputTask == null) m_vKeyInputTask = new HashSet<AgentTreeTask>(32);
                m_vKeyInputTask.Add(pTask);
                if (m_vKeyListens == null) m_vKeyListens = new List<KeyListen>(4);
                KeyCode keyCode = (KeyCode)pTask.taskData.EnterNode.EnterType;
                KeyListen keyListen = new KeyListen();
                keyListen.keyCode = keyCode;
                keyListen.useRef = 1;
                for(int i =0; i < m_vKeyListens.Count;++i)
                {
                    keyListen = m_vKeyListens[i];
                    if(keyListen.keyCode == keyCode)
                    {
                        keyListen.useRef++;
                        m_vKeyListens[i] = keyListen;
                        return;
                    }
                }
                keyListen.keyCode = keyCode;
                keyListen.useRef = 1;
                m_vKeyListens.Add(keyListen);
            }
            else if (pTask.taskData.type == ETaskType.MouseInput)
            {
                if (m_vMouseInputTask == null) m_vMouseInputTask = new HashSet<AgentTreeTask>(32);
                m_vMouseInputTask.Add(pTask);
            }
        }
        //------------------------------------------------------
        public void OnRemoveTask(AgentTreeTask pTask)
        {
            if (pTask == null || m_vDestroyingTask.Contains(pTask)) return;
            m_vDestroyingTask.Add(pTask);
        }
        //------------------------------------------------------
        void DelTask(AgentTreeTask pTask)
        {
            if (pTask.taskData == null || pTask.taskData.EnterNode == null) return;

            if(pTask.taskData.type == ETaskType.Custom)
            {
                if (pTask.taskData.EnterNode.EnterType == 0)
                    return;
                EventData vData;
                if (m_vCustomTask.TryGetValue(pTask.taskData.EnterNode.EnterType, out vData))
                {
                    if (pTask.taskData.EnterNode != null)
                    {
                        if (pTask.taskData.EnterNode.CustomGO)
                        {
                            if (vData.EventByGOs != null)
                            {
                                HashSet<AgentTreeTask> vTask;
                                if (vData.EventByGOs.TryGetValue(pTask.taskData.EnterNode.CustomGO, out vTask))
                                    vTask.Remove(pTask);
                            }

                        }
                        if (!string.IsNullOrEmpty(pTask.taskData.EnterNode.CustomName))
                        {

                            if (vData.EventByNames != null)
                            {
                                HashSet<AgentTreeTask> vTask;
                                if (vData.EventByNames.TryGetValue(pTask.taskData.EnterNode.CustomName, out vTask))
                                    vTask.Remove(pTask);
                            }
                        }
                        if (pTask.taskData.EnterNode.CustomID != 0)
                        {
                            if (vData.Events != null)
                            {
                                HashSet<AgentTreeTask> vTask;
                                if (vData.Events.TryGetValue(pTask.taskData.EnterNode.CustomID, out vTask))
                                    vTask.Remove(pTask);
                            }
                        }
                    }
                }
            }
            else if (pTask.taskData.type == ETaskType.KeyInput)
            {
                if (m_vKeyInputTask != null)
                    m_vKeyInputTask.Remove(pTask);
                if(m_vKeyListens!=null)
                {
                    for (int i = 0; i < m_vKeyListens.Count; ++i)
                    {
                        var keyListen = m_vKeyListens[i];
                        if (keyListen.keyCode == (KeyCode)pTask.taskData.EnterNode.EnterType)
                        {
                            keyListen.useRef--;
                            if (keyListen.useRef <= 0)
                                m_vKeyListens.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
            else if (pTask.taskData.type == ETaskType.MouseInput)
            {
                if (m_vMouseInputTask != null)
                    m_vMouseInputTask.Remove(pTask);
            }
        }
        //------------------------------------------------------
        public void OnActionCallback(AgentTreeTask pTask, ActionNode pNode, int nFuncId)
        {
            for(int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnATAction(pTask, pNode, nFuncId);
            }
        }
        //------------------------------------------------------
        public bool OnFillCustomVariable(AgentTreeTask pTask, Variable pCustomVariable, IUserData pData)
        {
            if (pData == null)
            {
                pCustomVariable.Destroy();
                return false;
            }
            OnInnerFillCustomVariable(pTask, pCustomVariable, pData);
            if (pCustomVariable is VariableUser)
            {
                VariableUser variable = pCustomVariable as VariableUser;
                variable.mValue = pData;
                if(variable.hashCode == GetClassHashCode( typeof(IUserData) ) )
                    variable.SetClassHashCode(GetClassHashCode(pData.GetType()) );
                return true;
            }
            if (pCustomVariable is VariableMonoScript)
            {
                (pCustomVariable as VariableMonoScript).mValue = pData as Behaviour;
                return true;
            }
            if (pCustomVariable is VariableObject)
            {
                (pCustomVariable as VariableObject).mValue = pData as UnityEngine.Object;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public int GetClassHashCode(System.Type type)
        {
            int hash = 0;
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                hash = m_vCallback[i].GetATClassHashCode(type);
                if (hash != 0) return hash;
            }
            return AgentTreeUtl.StringToHash(type.FullName);
        }
        //------------------------------------------------------
        public System.Type GetHashCodeClassType(int hashCode)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                System.Type type = m_vCallback[i].GetATHashCodeClass(hashCode);
                if (type != null) return type;
            }
            return null;
        }
        //------------------------------------------------------
        public int GetParentHashCode(int hashCode)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                int parentHash = m_vCallback[i].GetATParentHashCode(hashCode);
                if (parentHash != 0) return parentHash;
            }
            return 0;
        }
        //------------------------------------------------------
        bool OnInnerFillCustomVariable(AgentTreeTask pTask, Variable pCustomVariable, IUserData pData)
        {
            if (pCustomVariable is Framework.Plugin.AT.VariableBool)
            {
                if (pData is Variable1) (pCustomVariable as Framework.Plugin.AT.VariableBool).mValue = ((Variable1)pData).intVal != 0;
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableBoolList)
            {
                Framework.Plugin.AT.VariableBoolList list = pCustomVariable as Framework.Plugin.AT.VariableBoolList;
                if (pData is Variable2)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<bool>(2);
                    list.mValue.Add(((Variable2)pData).intVal0 != 0);
                    list.mValue.Add(((Variable2)pData).intVal1 != 0);
                }
                else if (pData is Variable3)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<bool>(3);
                    list.mValue.Add(((Variable3)pData).intVal0 != 0);
                    list.mValue.Add(((Variable3)pData).intVal1 != 0);
                    list.mValue.Add(((Variable3)pData).intVal2 != 0);
                }
                else if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<bool>(4);
                    list.mValue.Add(((Variable4)pData).intVal0 != 0);
                    list.mValue.Add(((Variable4)pData).intVal1 != 0);
                    list.mValue.Add(((Variable4)pData).intVal2 != 0);
                    list.mValue.Add(((Variable4)pData).intVal3 != 0);
                }
                return true;
            }

            //byte
            if (pCustomVariable is Framework.Plugin.AT.VariableByte)
            {
                if (pData is Variable1) (pCustomVariable as Framework.Plugin.AT.VariableByte).mValue = (byte)((Variable1)pData).intVal;
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    int outValue;
                    if (uiArgv.GetInt(out outValue)) (pCustomVariable as Framework.Plugin.AT.VariableByte).mValue = (byte)outValue;
                }
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableByteList)
            {
                Framework.Plugin.AT.VariableByteList list = pCustomVariable as Framework.Plugin.AT.VariableByteList;
                if (pData is Variable2)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<byte>(2);
                    list.mValue.Add((byte)((Variable2)pData).intVal0);
                    list.mValue.Add((byte)((Variable2)pData).intVal1);
                }
                else if (pData is Variable3)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<byte>(3);
                    list.mValue.Add((byte)((Variable3)pData).intVal0);
                    list.mValue.Add((byte)((Variable3)pData).intVal1);
                    list.mValue.Add((byte)((Variable3)pData).intVal2);
                }
                else if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<byte>(4);
                    list.mValue.Add((byte)((Variable4)pData).intVal0);
                    list.mValue.Add((byte)((Variable4)pData).intVal1);
                    list.mValue.Add((byte)((Variable4)pData).intVal2);
                    list.mValue.Add((byte)((Variable4)pData).intVal3);
                }
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    int outValue;
                    if (uiArgv.GetInt(out outValue))
                    {
                        if (list.mValue == null) list.mValue = new System.Collections.Generic.List<byte>(1);
                        list.mValue.Add((byte)outValue);
                    }
                }
                return true;
            }

            //int
            if (pCustomVariable is Framework.Plugin.AT.VariableInt)
            {
                if (pData is Variable1) (pCustomVariable as Framework.Plugin.AT.VariableInt).mValue = ((Variable1)pData).intVal;
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    int outValue;
                    if (uiArgv.GetInt(out outValue)) (pCustomVariable as Framework.Plugin.AT.VariableInt).mValue = outValue;
                }
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableIntList)
            {
                Framework.Plugin.AT.VariableIntList list = pCustomVariable as Framework.Plugin.AT.VariableIntList;
                if (pData is Variable2)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<int>(2);
                    list.mValue.Add(((Variable2)pData).intVal0);
                    list.mValue.Add(((Variable2)pData).intVal1);
                }
                else if (pData is Variable3)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<int>(3);
                    list.mValue.Add(((Variable3)pData).intVal0);
                    list.mValue.Add(((Variable3)pData).intVal1);
                    list.mValue.Add(((Variable3)pData).intVal2);
                }
                else if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<int>(4);
                    list.mValue.Add(((Variable4)pData).intVal0);
                    list.mValue.Add(((Variable4)pData).intVal1);
                    list.mValue.Add(((Variable4)pData).intVal2);
                    list.mValue.Add(((Variable4)pData).intVal3);
                }
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    int outValue;
                    if (uiArgv.GetInt(out outValue))
                    {
                        if (list.mValue == null) list.mValue = new System.Collections.Generic.List<int>(1);
                        list.mValue.Add(outValue);
                    }
                }
                return true;
            }

            //long
            if (pCustomVariable is Framework.Plugin.AT.VariableLong)
            {
                if (pData is Variable2) (pCustomVariable as Framework.Plugin.AT.VariableLong).mValue = ((Variable2)pData).longValue;
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableLongList)
            {
                Framework.Plugin.AT.VariableLongList list = pCustomVariable as Framework.Plugin.AT.VariableLongList;
                if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<long>(2);
                    list.mValue.Add(((Variable4)pData).longValue0);
                    list.mValue.Add(((Variable4)pData).longValue1);
                }
                return true;
            }

            //float
            if (pCustomVariable is Framework.Plugin.AT.VariableFloat)
            {
                if (pData is Variable1) (pCustomVariable as Framework.Plugin.AT.VariableFloat).mValue = ((Variable1)pData).floatVal;
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    float outValue;
                    if (uiArgv.GetFloat(out outValue)) (pCustomVariable as Framework.Plugin.AT.VariableFloat).mValue = outValue;
                }
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableFloatList)
            {
                Framework.Plugin.AT.VariableFloatList list = pCustomVariable as Framework.Plugin.AT.VariableFloatList;
                if (pData is Variable2)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<float>(2);
                    list.mValue.Add(((Variable2)pData).floatVal0);
                    list.mValue.Add(((Variable2)pData).floatVal1);
                }
                else if (pData is Variable3)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<float>(3);
                    list.mValue.Add(((Variable3)pData).floatVal0);
                    list.mValue.Add(((Variable3)pData).floatVal1);
                    list.mValue.Add(((Variable3)pData).floatVal2);
                }
                else if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<float>(4);
                    list.mValue.Add(((Variable4)pData).floatVal0);
                    list.mValue.Add(((Variable4)pData).floatVal1);
                    list.mValue.Add(((Variable4)pData).floatVal2);
                    list.mValue.Add(((Variable4)pData).floatVal3);
                }
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    float outValue;
                    if (uiArgv.GetFloat(out outValue))
                    {
                        if (list.mValue == null) list.mValue = new System.Collections.Generic.List<float>(1);
                        list.mValue.Add(outValue);
                    }
                }
                return true;
            }

            //Vector2
            if (pCustomVariable is Framework.Plugin.AT.VariableVector2)
            {
                if (pData is Variable2) (pCustomVariable as Framework.Plugin.AT.VariableVector2).mValue = ((Variable2)pData).ToVector2();
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableVector2Int)
            {
                if (pData is Variable2) (pCustomVariable as Framework.Plugin.AT.VariableVector2Int).mValue = ((Variable2)pData).ToVector2Int();
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableVector2List)
            {
                Framework.Plugin.AT.VariableVector2List list = pCustomVariable as Framework.Plugin.AT.VariableVector2List;
                if (pData is Variable4)
                {
                    if (list.mValue == null) list.mValue = new System.Collections.Generic.List<UnityEngine.Vector2>(4);
                    list.mValue.Add(((Variable4)pData).ToVector2_0());
                    list.mValue.Add(((Variable4)pData).ToVector2_1());
                }
                return true;
            }
            //Vector3
            if (pCustomVariable is Framework.Plugin.AT.VariableVector3)
            {
                if (pData is Variable3) (pCustomVariable as Framework.Plugin.AT.VariableVector3).mValue = ((Variable3)pData).ToVector3();
                return true;
            }
            if (pCustomVariable is Framework.Plugin.AT.VariableVector3Int)
            {
                if (pData is Variable3) (pCustomVariable as Framework.Plugin.AT.VariableVector3Int).mValue = ((Variable3)pData).ToVector3Int();
                return true;
            }
            //Vector4
            if (pCustomVariable is Framework.Plugin.AT.VariableVector4)
            {
                if (pData is Variable4) (pCustomVariable as Framework.Plugin.AT.VariableVector3).mValue = ((Variable4)pData).ToVector4();
                return true;
            }
            //Quaternion
            if (pCustomVariable is Framework.Plugin.AT.VariableQuaternion)
            {
                if (pData is Variable4) (pCustomVariable as Framework.Plugin.AT.VariableQuaternion).mValue = ((Variable4)pData).ToQuaternion();
                return true;
            }
            //Color
            if (pCustomVariable is Framework.Plugin.AT.VariableColor)
            {
                if (pData is Variable4) (pCustomVariable as Framework.Plugin.AT.VariableColor).mValue = ((Variable4)pData).ToColor();
                return true;
            }
            //string
            if (pCustomVariable is Framework.Plugin.AT.VariableString)
            {
                if (pData is VariableString)
                    (pCustomVariable as Framework.Plugin.AT.VariableString).mValue = ((Framework.Core.VariableString)pData).strValue;
                else if (pData is Framework.UI.UIRuntimeParamArgvs)
                {
                    Framework.UI.UIRuntimeParamArgvs uiArgv = (Framework.UI.UIRuntimeParamArgvs)pData;
                    string outValue;
                    if (uiArgv.GetString(out outValue)) (pCustomVariable as Framework.Plugin.AT.VariableString).mValue = outValue;
                }
                return true;
            }

            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                if (m_vCallback[i].OnATFillCustomVariable(pTask, pCustomVariable, pData))
                    return true;
            }
            return false;
        }
    }
}