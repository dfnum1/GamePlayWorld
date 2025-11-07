/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AgentTreeTask
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeTask
    {
        Task m_pTaskData;
        AgentTree m_pTree;

        EnterNode m_pEnterNode;
        List<int> m_vExcudedAndHasReturnStacks = new List<int>(4);
        List<ExcudeNode> m_vQueueActions = new List<ExcudeNode>(4);
        public Task taskData
        {
            get { return m_pTaskData; }
        }
#if UNITY_EDITOR
        private bool m_bContinueBreakPointing = true;
        private ExcudeNode m_BreakPointingAction = null;
        public float delayActionTime = 0;
        private HashSet<int> m_vExcudedNodes = new HashSet<int>();
        private HashSet<int> m_vRemovExcudeNodes = new HashSet<int>();

        public bool IsContains(int guid)
        {
            return m_vExcudedNodes.Contains(guid);
        }

        public HashSet<int> ExcudedNodes
        {
            get { return m_vExcudedNodes; }
        }

        public void AddExcudeNodes(int guid)
        {
            m_vExcudedNodes.Add(guid);
        }

        public void ContinueBreakPoint(int guid)
        {
            if (m_BreakPointingAction!=null && m_BreakPointingAction.GUID == guid)
            {
                if (UnityEditor.EditorApplication.isPaused)
                    UnityEditor.EditorApplication.isPaused = false;
                m_bContinueBreakPointing = true;
            }
        }
#endif
        bool m_bInstancing = false;
        long m_lTaskID = 0;

        bool m_bPlay = false;
        bool m_bInited = false;
        //------------------------------------------------------
        public AgentTreeTask(AgentTree pAT, Task pData, int index)
        {
            SetData(pAT, pData, index);
        }
        //------------------------------------------------------
        ~AgentTreeTask()
        {
            m_vExcudedAndHasReturnStacks = null;
            m_vQueueActions = null;
            m_pEnterNode = null;
        }
        //------------------------------------------------------
        public void SetData(AgentTree pAT, Task pData, int index)
        {
            m_bInstancing = false;
            m_bInited = false;
            Init(pAT, pData);
            m_lTaskID = pAT.Guid << 32 | index;
        }
        //------------------------------------------------------
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
        //------------------------------------------------------
        public long GetID()
        {
            return m_lTaskID;
        }
        //------------------------------------------------------
        public void Init(AgentTree pAT, Task pData)
        {
            Clear();

            m_bInited = pAT != null && pData != null;
            if (!m_bInited) return;

            m_pTree = pAT;
            m_pTaskData = pData;
            m_pEnterNode = pData.EnterNode;
            if (m_pEnterNode == null) return;
            m_pEnterNode.Init(pAT);
            if (m_pEnterNode.Action != null)
                m_pEnterNode.Action.Reset(AgentTreeUtl.intSet);
        }
        //------------------------------------------------------
        public void Reset()
        {
            if (m_pEnterNode != null)
            {
                m_pEnterNode.Destroy();
                m_pEnterNode.Reset(AgentTreeUtl.intSet);
            }
            m_vExcudedAndHasReturnStacks.Clear();
            m_vQueueActions.Clear();
            m_bInstancing = false;
            m_bPlay = false;
#if UNITY_EDITOR
            m_BreakPointingAction = null;
            m_bContinueBreakPointing = true;
            delayActionTime = 0;
            m_vExcudedNodes.Clear();
            m_vRemovExcudeNodes.Clear();
#endif
        }
        //------------------------------------------------------
        public void Clear()
        {
            Reset();
            m_lTaskID = 0;
            m_pEnterNode = null;
        }
        //------------------------------------------------------
        public bool IsPlaying
        {
            get { return m_bPlay; }
        }
        //------------------------------------------------------
        public bool IsEnabled
        {
            get { return m_pTree != null && m_pTree.IsEnable(); }
        }
        //------------------------------------------------------
        public AgentTree pAT
        {
            get { return m_pTree; }
        }
        //------------------------------------------------------
        public List<int> GetExcudedReturnList()
        {
            return m_vExcudedAndHasReturnStacks;
        }
        //------------------------------------------------------
        public ExcudeNode GetExcudeNode(int guid)
        {
            if (m_pTree == null) return null;
            return m_pTree.GetExcudeNode(guid);
        }
        //------------------------------------------------------
        public APINode GetAPINode(int guid, int ownerAT = 0)
        {
            return null;
        }
        //------------------------------------------------------
        public T GetVariable<T>(int guid) where T : Variable
        {
            return m_pTree.GetVariable<T>(guid);
        }
        //------------------------------------------------------
        public bool DoAction(ExcudeNode pNode, float fFrame)
        {
            if (pNode == null) return true;

            if(pNode.bDeltaing)
            {
                if (pNode.fDeltaTime > 0)
                    pNode.fDeltaTime -= fFrame;
                else
                {
                    pNode.fDeltaTime = 0;
                }
                return pNode.fDeltaTime <= 0;
            }
            pNode.OnFillInArgv(this);

#if UNITY_EDITOR
            if (m_BreakPointingAction == pNode)
            {
                m_BreakPointingAction = null;
                m_bContinueBreakPointing = true;
            }
            else if (pNode.bBreakPoint)
            {
                m_bContinueBreakPointing = false;
                m_BreakPointingAction = pNode;
            }
            m_vExcudedNodes.Add(pNode.GUID);
            if (!m_bContinueBreakPointing) return false;
#endif
            return ExcudeDO(pNode, 0);
        }
        //------------------------------------------------------
        public bool ExcudeAction(ExcudeNode pNode, int functionId = 0)
        {
#if UNITY_EDITOR
            m_vExcudedNodes.Add(pNode.GUID);
#endif
            if (ExcudeDO(pNode, functionId))
            {
                if (pNode.nextActions != null)
                {
                    for (int i = 0; i < pNode.nextActions.Length; ++i)
                        AddDoAction(pNode.nextActions[i]);
                }
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        bool ExcudeDO(ExcudeNode pNode, int functionId)
        {
            if(pNode is ActionNode)
            {
                return DoInerAction(pNode as ActionNode, functionId);
            }
            else if (pNode is APINode)
            {
                APINode apiNode = pNode as APINode;
            //    apiNode.excudeType)
                return DoInerAction(pNode as ActionNode, functionId);
            }
            return false;
        }
        //------------------------------------------------------
        public void AddDoAction(ExcudeNode pAction)
        {
            if (pAction == null) return;
            if (m_vQueueActions.Contains(pAction)) return;
            m_vQueueActions.Add(pAction);
#if UNITY_EDITOR
            m_vExcudedNodes.Add(pAction.GUID);
#endif
        }
        //------------------------------------------------------
        public void Play(IUserData pData = null)
        {
            if (m_pEnterNode == null || m_pEnterNode.Action == null) return;
            Reset();
            if(m_pEnterNode.Param != null && m_pEnterNode.Param.variable!=null)
                AgentTreeManager.getInstance().OnFillCustomVariable(this, m_pEnterNode.Param.variable, pData);

#if UNITY_EDITOR
            delayActionTime = 0;
            m_vExcudedNodes.Clear();
            m_vRemovExcudeNodes.Clear();
#endif
            AddDoAction(m_pEnterNode.Action);

            m_bPlay = true;
            Update(0);
        }
        //------------------------------------------------------
        public void ContinueNext()
        {
            if (!m_bInited || !m_bPlay || m_vQueueActions == null || m_vQueueActions.Count<=0) return;
            ExcudeNode pNode = m_vQueueActions[0];
            if (pNode.outArgvs != null && pNode.outArgvs.Length > 0)
                m_vExcudedAndHasReturnStacks.Add(pNode.GUID);

            //     pNode.Reset();
            m_vQueueActions.RemoveAt(0);
#if UNITY_EDITOR
            m_vRemovExcudeNodes.Add(pNode.GUID);
            delayActionTime = 1f;
#endif
            if (pNode.nextActions != null)
            {
                for (int j = 0; j < pNode.nextActions.Length; ++j)
                    AddDoAction(pNode.nextActions[j]);
            }
            Update(0);
        }
        //------------------------------------------------------
        public bool Update(float fFrame)
        {
            if (!m_bInited)
            {
                return false;
            }

#if UNITY_EDITOR
            if(delayActionTime >0)
            {
                delayActionTime -= fFrame;
                if (delayActionTime <= 0)
                {
                    foreach(var db in m_vRemovExcudeNodes)
                        m_vExcudedNodes.Remove(db);
                    m_vRemovExcudeNodes.Clear();
                }
            }
#endif

            if (!m_bPlay) return false;
            if (m_pEnterNode == null)
            {
                m_bPlay = false;
                if (m_pEnterNode.Param != null) m_pEnterNode.Param.Destroy();
#if UNITY_EDITOR
                m_vRemovExcudeNodes.Clear();
                m_vExcudedNodes.Clear();
#endif
                return false;
            }

            if (m_bInstancing) return true;
            if (m_vQueueActions == null || m_vQueueActions.Count <= 0)
            {
                if (m_pEnterNode.Param != null) m_pEnterNode.Param.Destroy();
#if UNITY_EDITOR
                m_vRemovExcudeNodes.Clear();
                m_vExcudedNodes.Clear();
#endif
                return false;
            }

#if UNITY_EDITOR
            if (!m_bContinueBreakPointing) return true;
#endif
            for (int i = 0; i < m_vQueueActions.Count;)
            {
                if (DoAction(m_vQueueActions[i], fFrame))
                {
                    if (m_vQueueActions.Count > i)
                    {
                        ExcudeNode pNode = m_vQueueActions[i];
                        if (pNode.outArgvs != null && pNode.outArgvs.Length > 0)
                            m_vExcudedAndHasReturnStacks.Add(pNode.GUID);

                        //     pNode.Reset();
                        m_vQueueActions.RemoveAt(i);
#if UNITY_EDITOR
                        m_vRemovExcudeNodes.Add(pNode.GUID);
                        delayActionTime = 1;
#endif
                        if (pNode.nextActions != null)
                        {
                            for (int j = 0; j < pNode.nextActions.Length; ++j)
                                AddDoAction(pNode.nextActions[j]);
                        }
                    }
                    else ++i;
                }
                else
                {
#if UNITY_EDITOR
                    if (!m_bContinueBreakPointing)
                    {
                        if (UnityEditor.EditorApplication.isPaused)
                            UnityEditor.EditorApplication.isPaused = true;
                        break;
                    }
#endif
                    ++i;
                }
            }
            return true;
        }
    }

}
