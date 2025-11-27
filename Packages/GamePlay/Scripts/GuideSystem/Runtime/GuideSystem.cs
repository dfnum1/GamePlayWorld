/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideSystem
作    者:	HappLI
描    述:	引导系统
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Guide
{
    public struct CallbackParam
    {
        /// <summary>
        /// 点击的UI控件上面挂载的 GuideGuid 组件上的 guid
        /// </summary>
        public int widgetGuid;
        public string widgetTag;
        /// <summary>
        /// 点击动态加载的UI时,点击的物体索引
        /// </summary>
        public int listIndex;
        public EUIWidgetTriggerType triggerType;

        public ETouchType touchType;
        public int touchId;
        public Vector2 mousePos;
        public Vector2 deltaMose;

        public int customType;
        public int userData;

        public VariableList Argvs;

        public void Clear()
        {
            widgetGuid = 0;
            widgetTag = null;
             listIndex = -1;
            triggerType = EUIWidgetTriggerType.None;
            if(Argvs!=null) Argvs.Clear();

            touchId = -1;
            touchType = ETouchType.None;
            mousePos = Vector2.zero;
            deltaMose = Vector2.zero;

            userData = 0;
            customType = 0;
        }
    }
    public interface IGuideSystemCallback
    {
        void OnGuideGroupStatus(BaseNode triggerNode, bool bDoing, bool bCheckRecord);
        IUserData OnGuideBuildEvent(string strEvent);
        bool OnTriggerGuideEvent(IUserData pEvent, IUserData pTrigger = null);
        void OnGuideExecuteNode(BaseNode pNode);
        bool OnGuideCheckSign(BaseNode pNode, CallbackParam param);
        void OnGuideNodeAutoNext(BaseNode pNode);
        bool OnGuideSuccssedListener(BaseNode pNode);
        string OnGuideLanguageContent(string codeName);
        bool OnGuideLoadAsset(UnityEngine.Object pCall, string file, bool bAsync);
        public bool OnGuideUnLoadAsset(string file, UnityEngine.Object obj);

    }
    public class GuideSystem
    {
        public static Action OnGuideNoForce;
#if UNITY_EDITOR
        public static System.Reflection.MethodInfo BUILD_EVENT_METHOD = null;
        public List<BaseNode> vTrackingNodes { get { return m_vTracking; } }

        public bool bTestFlag = false;
#endif
        static GuideSystem ms_Instance;
        public static GuideSystem getInstance()
        {
            if (ms_Instance == null) ms_Instance = new GuideSystem();
            return ms_Instance;
        }
        List<IGuideSystemCallback> m_vCallbacks = new List<IGuideSystemCallback>(2);
        Dictionary<int, GuideGroup> m_vDatas = null;
        Dictionary<int, List<TriggerNode>> m_vTriggers = new Dictionary<int, List<TriggerNode>>(128);

        List<BaseNode> m_vTracking = new List<BaseNode>(16);
        TriggerNode m_pDoingTriggerNode = null;
        SeqNode m_pDoingNode = null;
        float m_fAutoNextDelta = 0;
        float m_fDeltaDelta = 0;
        /// <summary>
        /// 延迟交互检测
        /// </summary>
        float m_fDeltaSign = 0;
        int m_nTouchID = -1;
        CallbackParam m_CallbackParam = new CallbackParam();

        private Dictionary<int,System.UInt64> m_vGuideFlags = new Dictionary<int, ulong>();
//         private System.UInt64 m_GuideFlag1 = 0;
//         private System.UInt64 m_GuideFlag2 = 0;
//         private System.UInt64 m_GuideFlag3 = 0;
//         private System.UInt64 m_GuideFlag4 = 0;

        /// <summary>
        /// 最后一次执行的引导组id,包括非强制引导组
        /// </summary>
        public int LastGroupGuid = 0;
        public bool bGuideLogEnable = false;

        private bool m_bIgnoreTouchInput = false;
        public bool bIgnoreTouchInput
        {
            get
            {
                return m_bIgnoreTouchInput;
            }
            set
            {
                //Debug.LogError("set bIgnoreTouchInput:" + value);
                m_bIgnoreTouchInput = value;
            }
        }

        private GuidePanel m_GuidePanel = null;
        private bool m_bEnableSystem = false;
        private bool m_bEnableStuckSkip = false;
        public bool IsEnableStuckSkip
        {
            get
            {
                return m_bEnableStuckSkip;
            }
        }
        public Dictionary<int, GuideGroup> datas
        {
            get { return m_vDatas; }
            set
            {
                if (m_vDatas != value)
                {
                    m_vDatas = value;
                    //ResetRefresh(0,0,0,0);//本地测试引导时使用
                }
            }
        }
        public bool bDoing
        {
            get
            {
                return m_pDoingTriggerNode != null;
            }
        }
        public bool bNoForceDoing
        {
            get
            {
                if (m_pDoingNode != null && m_pDoingNode is StepNode)
                {
                    return ((StepNode)m_pDoingNode).bOption;
                }
                return false;
            }
        }
        public TriggerNode DoingTriggerNode
        {
            get { return m_pDoingTriggerNode; }
        }
        public SeqNode DoingSeqNode
        {
            get { return m_pDoingNode; }
        }
        //------------------------------------------------------
        public bool IsEnable
        {
            get { return m_bEnableSystem; }
        }
        //------------------------------------------------------
        public void Enable(bool bEnable)
        {
            if (m_bEnableSystem == bEnable) return;
            m_bEnableSystem = bEnable;
            if (!m_bEnableSystem)
                OverGuide(false);
        }
        //------------------------------------------------------
        public void EnableSkip(bool bEnableSkip)
        {
            if (m_bEnableStuckSkip == bEnableSkip) return;
            m_bEnableStuckSkip = bEnableSkip;
        }
        //------------------------------------------------------
        public GuidePanel GetGuidePanel()
        {
            return m_GuidePanel;
        }
        //------------------------------------------------------
        public void SetGuidePanel(GuidePanel panel)
        {
            m_GuidePanel = panel;
        }
        //------------------------------------------------------
        public void Clear()
        {
            OverGuide(false);
            m_vTriggers.Clear();
            m_nTouchID = -1;
            m_fAutoNextDelta = 0;
            m_fDeltaDelta = 0;
            m_fDeltaSign = 0;
            m_CallbackParam.Clear();

            m_vGuideFlags.Clear();
            //             m_GuideFlag1 = 0;
            //             m_GuideFlag2 = 0;
            //             m_GuideFlag3 = 0;
            //             m_GuideFlag4 = 0;

            m_bIgnoreTouchInput = false;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            Clear();
        }
        //------------------------------------------------------
        void GetKeyIndex(int tag, out int key, out int index)
        {
            key = Mathf.FloorToInt(tag / 64f);
            index = tag % 64;
            if (index == 0)
            {
                index = 64;
                key = key - 1;
            }
        }
        //------------------------------------------------------
        public bool IsTrigged(int tag)
        {
#if UNITY_EDITOR
            if (bTestFlag)
	        {
                 return false;
	        }
#endif
            GetKeyIndex(tag, out var key, out var index);
            if (index < 0)
            {
                UnityEngine.Debug.LogWarning("GuideSystem IsTrigged index<0 and tag > 0:" + tag);
                return false;
            }
            if (m_vGuideFlags.TryGetValue(key, out var maskFlag))
            {
                ulong bit = 1;
                if ((maskFlag & (bit << (index-1))) != 0)
                    return true;
            }
            return false;
//             if (tag >= 256)
//                 return false;
//             ulong bit = 1;
//             if (tag < 64) return (m_GuideFlag1 & (bit << tag)) != 0;
//             if (tag < 128) return (m_GuideFlag2 & (bit << (tag - 64))) != 0;
//             if (tag < 192) return (m_GuideFlag3 & (bit << (tag - 128))) != 0;
//             if (tag < 256) return (m_GuideFlag4 & (bit << (tag - 192))) != 0;
//             return false;
        }
        //------------------------------------------------------
        public void SetFlagValue(int key, UInt64 maskValue)
        {
            m_vGuideFlags[key] = maskValue;
        }
        //------------------------------------------------------
        public void UpdateFlag(int tag)
        {
            GetKeyIndex(tag, out var key, out var index);
            if (index < 0)
            {
                UnityEngine.Debug.LogWarning("GuideSystem IsTrigged index<0 and tag > 0:" + tag);
                return;
            }
            if (m_vGuideFlags.TryGetValue(key, out var maskFlag))
            {
                ulong bit = 1;
                maskFlag |= (bit << (index-1));
                m_vGuideFlags[key] = maskFlag;
            }
            else
            {
                ulong bit = 1;
                maskFlag = (bit << (index-1));
                m_vGuideFlags.Add(key, maskFlag);
            }
            //   m_vGuideFlags.Add(tag);
            //             if (tag >= 256)
            //                 return;
            //             ulong bit = 1;
            //             if (tag < 64) m_GuideFlag1 |= (bit << tag);
            //             else if (tag < 128) m_GuideFlag2 |= (bit << (tag - 64));
            //             else if (tag < 192) m_GuideFlag3 |= (bit << (tag - 128));
            //             else if (tag < 256) m_GuideFlag4 |= (bit << (tag - 192));
        }
        //------------------------------------------------------
        public void ResetRefresh(IList<UInt64> flags, bool bMaskFlag=true)
        {
            if (m_vDatas == null) return;
            Clear();
            if(flags!=null)
            {
                if(bMaskFlag)
                {
                    for (int i = 0; i < flags.Count; ++i)
                        SetFlagValue(i, flags[i]);
                }
                else
                {
                    for (int i = 0; i < flags.Count; ++i)
                        UpdateFlag((int)flags[i]);
                }
            }
            RefreshTriggers();
        }
        //------------------------------------------------------
        public void RefreshTriggers()
        {
            if (m_vTriggers != null) m_vTriggers.Clear();
            foreach (var db in m_vDatas)
            {
                if (db.Value.vTriggers == null) continue;

                for (int i = 0; i < db.Value.vTriggers.Count; ++i)
                {
                    if (IsTrigged(db.Value.vTriggers[i].GetTag()))
                    {
                        if(m_pDoingTriggerNode!=null && m_pDoingTriggerNode.Guid == db.Value.vTriggers[i].Guid)
                        {
                            OverGuide(false);
                        }
                        continue;
                    }
                    List<TriggerNode> vTriggers;
                    if (!m_vTriggers.TryGetValue(db.Value.vTriggers[i].type, out vTriggers))
                    {
                        vTriggers = new List<TriggerNode>(64);
                        m_vTriggers.Add(db.Value.vTriggers[i].type, vTriggers);
                    }
                    vTriggers.Add(db.Value.vTriggers[i]);
                }
            }

            //! sort
            List<TriggerNode> vTri;
            foreach (var db in m_vTriggers)
            {
                vTri = db.Value;
                SortUtility.QuickSortUp<TriggerNode>(ref vTri);
            }
        }
        //------------------------------------------------------
        public void AddGuide(GuideGroup guidGroup, bool bIngoreFlag = false )
        {
            if (guidGroup == null || m_vDatas == null) return;
            m_vDatas.Remove(guidGroup.Guid);
            foreach (var db in m_vTriggers)
            {
                if(db.Value == null) continue;
                for(int i = 0; i < db.Value.Count; )
                {
                    if (db.Value[i].guideGroup == guidGroup)
                    {
                        db.Value.RemoveAt(i);
                    }
                    else
                        ++i;
                }
            }

            if (guidGroup.vTriggers == null) return;
            m_vDatas.Add(guidGroup.Guid, guidGroup);

            for (int i = 0; i < guidGroup.vTriggers.Count; ++i)
            {
                if (!bIngoreFlag && IsTrigged(guidGroup.vTriggers[i].GetTag())) continue;
                List<TriggerNode> vTriggers;
                if (!m_vTriggers.TryGetValue(guidGroup.vTriggers[i].type, out vTriggers))
                {
                    vTriggers = new List<TriggerNode>(64);
                    m_vTriggers.Add(guidGroup.vTriggers[i].type, vTriggers);
                }
                vTriggers.Add(guidGroup.vTriggers[i]);
            }
        }
        //------------------------------------------------------
        public void OverOptionState()
        {
            if (m_pDoingNode == null) return;
            if (m_pDoingNode is StepNode && m_pDoingNode.IsOption())
            {
                OverGuide(false);
            }
        }
        //------------------------------------------------------
        public bool IsCanChangeStateBreak()
        {
            if (m_pDoingNode == null) return false;
            if (m_pDoingNode.guideGroup == null) return true;
            return m_pDoingNode.guideGroup.bChangeStateBreak;
        }
        //------------------------------------------------------
        void OnTriggerNodeEnd(TriggerNode triggerNode, bool bCheckRecord)
        {
            if (bCheckRecord)
            {
                if (triggerNode != null)
                {
                    OnEvent(triggerNode.GetEndEvents());
                    OnGuideGroupStatus(triggerNode, false, bCheckRecord);

                    if (triggerNode.guideGroup.bRemoveOver)
                    {
                        List<TriggerNode> vTriggers;
                        if (m_vTriggers.TryGetValue(triggerNode.type, out vTriggers) && vTriggers.Count > 0)
                        {
                            if (bGuideLogEnable)
                            {
                                Log("移除引导组:" + triggerNode.guideGroup.Guid);
                            }
                            vTriggers.Remove(triggerNode);
                        }
                    }
                }
            }
            else
            {
                if (triggerNode != null)
                {
                    OnGuideGroupStatus(triggerNode, false, bCheckRecord);
                }
            }
        }
        //------------------------------------------------------
        public void OverGuide(bool bCheckRecord = false)
        {
            OnTriggerNodeEnd(m_pDoingTriggerNode, bCheckRecord);

            m_pDoingTriggerNode = null;
            SetDoingNod(null);
            m_fAutoNextDelta = 0;
            m_fDeltaDelta = 0;
            m_fDeltaSign = 0;
            m_vTracking.Clear();
            if (bGuideLogEnable)
                Log("结束当前引导");
#if UNITY_EDITOR
            bTestFlag = false;
#endif
            if (m_GuidePanel != null) m_GuidePanel.Hide();
        }
        //------------------------------------------------------
        public bool DoGuide(int guide, int curState, BaseNode pStartNode = null, bool bForce =false)
        {
            if (m_vDatas == null)
                return false;
            GuideGroup pGroup;
            if(m_vDatas.TryGetValue(guide, out pGroup) && pGroup.vTriggers.Count>0)
            {
                var argv = VariableList.Get();
                argv.AddInt(curState);
                int startIndex = 0;
                if(pStartNode!=null)
                {
                    for(int i =0; i < pGroup.vTriggers.Count; ++i)
                    {
                        if(pGroup.vTriggers[i].Guid == pStartNode.Guid)
                        {
                            startIndex = i;
                            break;
                        }
                    }
                }
                return OnTrigger(pGroup.vTriggers[startIndex], pStartNode, bForce, argv);
            }
            return false;
        }
        //------------------------------------------------------
        public bool OnTrigger(int triggerType, BaseNode pStartNode, bool bForce, VariableList pArgvs = null)
        {
            if (!m_bEnableSystem) return false;
            List<TriggerNode> vTriggers;
            if (m_vTriggers.TryGetValue(triggerType, out vTriggers) && vTriggers.Count>0)
            {
                for (int i = vTriggers.Count -1;i >= 0;i--)
                {
                    if (bGuideLogEnable && vTriggers[i] != null && vTriggers[i].guideGroup != null)
                        Log("检测引导组:" + vTriggers[i].guideGroup.Guid + ",触发类型:" + triggerType);

                    if (OnTrigger(vTriggers[i], pStartNode, bForce, pArgvs))
                    {
                        if (bGuideLogEnable && vTriggers[i] != null && vTriggers[i].guideGroup != null)
                        {
                            Log("引导组:" + vTriggers[i].guideGroup.Guid + ",满足触发条件,不执行以下引导id:");
                            for (int j = i - 1; j >= 0; j--)
                            {
                                Log("不检测的引导组id:" + vTriggers[j].guideGroup.Guid);
                            }
                        }

                        //       if (vTriggers.Count > i)
                        //      {
                        //           vTriggers.RemoveAt(i);
                        //       }
                        return true;
                    }
                }
                
            }
            return false;
        }
        //------------------------------------------------------
        bool OnTrigger(TriggerNode pTrigger, BaseNode pStartNode, bool bForce, VariableList pArgvs = null)
        {
            if (!m_bEnableSystem) return false;

            if (m_pDoingTriggerNode == pTrigger)
            {
                if (bGuideLogEnable && pTrigger != null)
                    Log("当前正在执行的引导组和触发的引导组一致:" + pTrigger.GetTag());
                return true;
            }

            if (!bForce && IsTrigged(pTrigger.GetTag()))
            {
                if (bGuideLogEnable && pTrigger != null && pTrigger.guideGroup != null)
                    Log("当前引导组已经触发过:" + pTrigger.guideGroup.Guid);
                return false;
            }

            if (!bForce)
            {
                //! 当前为非强制的,要执行的引导组为强制的,则强制执行
                if (m_pDoingTriggerNode!=null && m_pDoingTriggerNode.IsOption() && !pTrigger.IsOption())
                {
                    bForce = true;
                }
            }

            if (!bForce)
            {
                if (m_pDoingTriggerNode != null && pTrigger.priority < m_pDoingTriggerNode.priority)
                {
                    if (bGuideLogEnable && pTrigger != null && pTrigger.guideGroup != null && m_pDoingTriggerNode != null && m_pDoingTriggerNode.guideGroup != null)
                        Log("触发引导组id:" + pTrigger.guideGroup.Guid + ",优先级:" + pTrigger.priority + ",低于当前正在执行的引导组:" + m_pDoingTriggerNode.guideGroup.Guid + ",优先级:" + m_pDoingTriggerNode.priority + ",不执行");
                    return false;
                }
            }

            if (pTrigger != null)
            {
                m_vTracking.Clear();
                AddTracking(pTrigger);
                pTrigger.FillArgv(pArgvs);
                SeqNode pNext = null;
                if (pStartNode != null && !(pStartNode is TriggerNode))
                {
                    if (pStartNode is SeqNode)
                        pNext = pStartNode as SeqNode;
                    else
                    {
                        pNext = CheckStart(pStartNode);
                    }
                }
                else
                    pNext = CheckStart(pTrigger);
                if (pNext == null)
                {
                    return false;
                }

                if (bGuideLogEnable)
                    Log("开始引导节点:" + pNext.guideGroup.Guid);

                if (m_pDoingTriggerNode != null)
                {
                    if (bGuideLogEnable)
                        Log("触发引导组id:" + pTrigger.guideGroup.Guid + ",优先级::" + pTrigger.priority + ",大于当前正在执行的引导组:" + pTrigger.guideGroup.Guid + ",优先级:" + m_pDoingTriggerNode.priority + ",覆盖当前正在执行的引导组");
                    OverGuide(false);
                }

                m_pDoingTriggerNode = pTrigger;
                SetDoingNod(pNext);
                AddTracking(m_pDoingNode);
                m_fDeltaDelta = m_pDoingNode.GetDeltaTime();
                m_fDeltaSign = m_pDoingNode.GetDeltaSignTime();
                m_fAutoNextDelta = m_pDoingNode.GetAutoNextTime();

                OnGuideGroupStatus(pTrigger, true, false);

                //! trigger node
                OnEvent(pTrigger.GetBeginEvents());
                OnNodeCall(pTrigger);

                //! doing node
                if(m_fDeltaDelta<=0)
                {
                    OnEvent(m_pDoingNode.GetBeginEvents());
                    OnNodeCall(m_pDoingNode);
                }
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        void AddTracking(BaseNode pNode)
        {
            if (m_vTracking.Contains(pNode)) return;
            m_vTracking.Add(pNode);
        }
        //------------------------------------------------------
        SeqNode CheckStart(BaseNode pNode)
        {
            SeqNode pNext = CheckNext(pNode); 
            if(pNext != null)
            {
                if (pNext is ExcudeNode)
                {
                    AddTracking(pNext);
                    ExcudeNode excude = pNext as ExcudeNode;
                    if (excude.bFireCheck)
                    {
                        if((excude.vOps != null && excude.vOps.Count > 0) || excude.pNext is ExcudeNode )
                        {
                     //       OnEvent(excude.GetBeginEvents());
                    //        OnNodeCall(excude);
                    //        OnEvent(excude.GetEndEvents());
                            pNext = CheckStart(excude);
                        }
                    }
                }
            }
            return pNext;
        }
        //------------------------------------------------------
        SeqNode CheckNext(BaseNode pNode)
        {
            bool IsExecuted = m_vTracking!=null && m_vTracking.Contains(pNode);
            AddTracking(pNode);
            if (pNode is SeqNode)
            {
                SeqNode seq = pNode as SeqNode;

                if(!IsExecuted)
                {
                    //! if excude node, trigger events
                    ExcudeNode excude = pNode as ExcudeNode;
                    if (excude != null)
                    {
                        OnEvent(excude.GetBeginEvents());
                        OnNodeCall(excude);
                        OnEvent(excude.GetEndEvents());
                    }
                }


                if (seq.vOps != null && seq.vOps.Count > 0)
                {
                    for (int i = 0; i < seq.vOps.Count; ++i)
                    {
                        if (seq.vOps[i].Operate(m_vTracking))
                        {
                            if(seq.vOps[i].pNext is GuideOperate)
                            {
                                return CheckNext(seq.vOps[i].pNext);
                            }
                            else
                                return seq.vOps[i].pNext as SeqNode;
                        }
                    }
                }
                else
                {
                    return seq.pNext;
                }
            }
            else if( pNode is GuideOperate )
            {
                GuideOperate ops = pNode as GuideOperate;
                if (ops.Operate(m_vTracking))
                {
                    return CheckNext(ops.pNext);
                }
                else
                    return null;
            }
            return null;
        }
        //------------------------------------------------------
        bool IsValid()
        {
            if (m_pDoingTriggerNode == null) return false;
            if (m_pDoingNode == null) return false;
            return true;
        }
        //------------------------------------------------------
        public void Register(IGuideSystemCallback pCallback)
        {
            if (m_vCallbacks.Contains(pCallback)) return;
            m_vCallbacks.Add(pCallback);
        }
        //------------------------------------------------------
        public void UnRegister(IGuideSystemCallback pCallback)
        {
            m_vCallbacks.Remove(pCallback);
        }
        //------------------------------------------------------
        public void Update(float fFrame)
        {
            if (!m_bEnableSystem) return;
            if (m_GuidePanel != null) m_GuidePanel.Update(fFrame);
            if (m_pDoingTriggerNode == null) return;
            if(m_fDeltaDelta >0)
            {
                m_fDeltaDelta -= fFrame;
                if (m_fDeltaDelta <= 0)
                {
                    m_fDeltaDelta = 0;
                    if(m_pDoingNode!=null)
                    {
                        OnEvent(m_pDoingNode.GetBeginEvents());
                        OnNodeCall(m_pDoingNode);
                    }
                }
                else
                    return;
            }

            //更新延迟交互计时器
            if (m_fDeltaSign > 0)
            {
                m_fDeltaSign -= fFrame;
                if (m_fDeltaSign <0)
                {
                    m_fDeltaSign = 0;
                }
            }

            if (m_pDoingNode != null)
            {
                if (m_pDoingNode.GetAutoNextTime() > 0)
                {
                    m_fAutoNextDelta -= fFrame;
                    if (m_fAutoNextDelta <= 0)
                    {
                        if(m_pDoingNode != null)
                        {
                            int loopTrack = 0;
                            ExcudeNode pNode = m_pDoingNode.GetAutoExcudeNode();
                            while(pNode!=null && loopTrack < 100)
                            {
                                OnEvent(pNode.GetBeginEvents());
                                OnNodeCall(pNode);
                                OnEvent(pNode.GetEndEvents());
                                SeqNode pNext = CheckNext(pNode);
                                if (pNext is ExcudeNode)
                                    pNode = pNext as ExcudeNode;
                                else
                                    pNode = null;
                                loopTrack++;
                            }
                        }
                        DoNext();
                    }
                }
                else if (m_pDoingNode.IsAutoNext())
                {
                    DoNext();
                }
                else if(m_pDoingNode !=null )
                {
                    if(m_pDoingNode.IsAutoSignCheck() && OnNodeSign(m_pDoingNode))
                    {
                        OnNodeSignCompleted();
                        DoNext();
                    }
                    if(m_pDoingNode!=null && m_pDoingNode.IsSuccessedListenerBreak() && !OnGuideSuccssedListener(m_pDoingNode))
                    {
                        OnNodeSignCompleted();
                        StepNode stepNode = m_pDoingNode as StepNode;
                        if(stepNode!=null && stepNode.pSuccessedListenerBreakNode!=null)
                        {
                            GotoNode(stepNode.pSuccessedListenerBreakNode,true);
                        }
                        else
                            DoNext();
                    }
                }
            }
        }
        //------------------------------------------------------
        void DoNext()
        {
            GotoNode(null);
        }
        //------------------------------------------------------
        void GotoNode(BaseNode pGo, bool bCallAutoNext= false)
        {
            if (m_pDoingNode != null && (m_pDoingNode.IsAutoNext() || m_pDoingNode.GetAutoNextTime() > 0 || bCallAutoNext))
            {
                OnNodeAutoNext(m_pDoingNode);
            }

            if (m_pDoingNode != null) OnEvent(m_pDoingNode.GetEndEvents());

            TriggerNode preTriggerNode = m_pDoingTriggerNode;
            SeqNode pNext = null;
            if (pGo != null) pNext = CheckNext(pGo);
            else pNext = CheckNext(m_pDoingNode);
            if (pNext == null)
            {
                if (bGuideLogEnable && m_pDoingNode != null)
                    Log("当前引导节点没有满足触发条件,或者没有下一个节点,,开始结束当前引导:" + m_pDoingNode.guideGroup.Guid);
                if(preTriggerNode== m_pDoingTriggerNode)
                    OverGuide(true);
                else
                {
                    OnTriggerNodeEnd(preTriggerNode,true);
                }
                return;
            }
            SeqNode pCurrent = m_pDoingNode;
            if (pCurrent == m_pDoingNode)
            {
                SetDoingNod(pNext);
                AddTracking(m_pDoingNode);
                m_fDeltaDelta = m_pDoingNode.GetDeltaTime();
                m_fDeltaSign = m_pDoingNode.GetDeltaSignTime();
                m_fAutoNextDelta = m_pDoingNode.GetAutoNextTime();

                if (m_fDeltaDelta <= 0)
                {
                    OnEvent(m_pDoingNode.GetBeginEvents());
                    OnNodeCall(m_pDoingNode);
                }
            }
        }

        //------------------------------------------------------
        private void SetDoingNod(SeqNode pNext)
        {
            m_pDoingNode = pNext;
            if (bNoForceDoing)
            {
                OnGuideNoForce?.Invoke();
            }
        }
        //------------------------------------------------------
        /// <summary>
        /// 当节点自动跳转时
        /// </summary>
        void OnNodeAutoNext(BaseNode pNode)
        {
            //1.如果是跳转前是step节点,那么要Reset 引导界面
            //2.标记节点状态是自动跳转状态,
            if (pNode is Framework.Guide.StepNode)
            {
                if (m_GuidePanel != null)
                    m_GuidePanel.ClearWidget();
                if (GuideStepHandler.OnGuideNodeAutoNext(pNode as Framework.Guide.StepNode))
                    return;
            }

            if (m_vCallbacks.Count <= 0) return;
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                m_vCallbacks[j].OnGuideNodeAutoNext(pNode);
            }
        }
        //------------------------------------------------------
        /// <summary>
        /// 当节点sign完成时
        /// </summary>
        void OnNodeSignCompleted()
        {
            //标记节点是操作完成状态,
            //if (m_pDoingNode is StepNode)
            //{
            //    StepNode stepNode = m_pDoingNode as StepNode;
            //}
        }
        //------------------------------------------------------
        public void OnCustomCallback(int customType, int userData, VariableList pArgvs = null)
        {
            if (m_pDoingNode == null || m_fDeltaDelta > 0) return;
            m_CallbackParam.Clear();
            m_CallbackParam.customType = customType;
            m_CallbackParam.userData = userData;
            if (pArgvs != null)
            {
                if (m_CallbackParam.Argvs == null)
                    m_CallbackParam.Argvs = new VariableList();

                m_CallbackParam.Argvs.Copy(pArgvs);
            }
            if (/*!m_pDoingNode.IsAutoNext() &&*/ OnNodeSign(m_pDoingNode))
            {
                OnNodeSignCompleted();
                DoNext();
            }  
        }
        //------------------------------------------------------
        public void OnUIWidgetTrigger(int widgetGuid, int listIndex, string widgetTag, EUIWidgetTriggerType type, VariableList pArgvs = null)
        {
            if (m_pDoingNode == null || m_fDeltaDelta > 0) return;
            m_CallbackParam.Clear();
            if (pArgvs != null)
            {
                if (m_CallbackParam.Argvs == null)
                    m_CallbackParam.Argvs = new VariableList();

                m_CallbackParam.Argvs.Copy(pArgvs);
            }
            m_CallbackParam.widgetGuid = widgetGuid;
            m_CallbackParam.listIndex = listIndex;
            m_CallbackParam.triggerType = type;
            m_CallbackParam.widgetTag = widgetTag;

            SeqNode pPreNode = m_pDoingNode;
            if (OnNodeSign(m_pDoingNode))
            {
                OnNodeSignCompleted();
                DoNext();
            }
            else
            {//如果点击的不是目标UI,才进行非强制引导检测
                if (pPreNode == m_pDoingNode && type == EUIWidgetTriggerType.Click)
                {
                    OverOptionState();
                }
            }
        }
        //------------------------------------------------------
        public void OnTouchBegin(int touchId, Vector2 position, Vector2 deltaPosition)
        {
            if (m_pDoingNode == null || m_fDeltaDelta > 0) return;
            m_nTouchID = touchId;
            if (!IsValid() || m_fDeltaDelta > 0) return;

            m_CallbackParam.Clear();
            m_CallbackParam.touchType = ETouchType.Begin;
            m_CallbackParam.touchId = touchId;
            m_CallbackParam.mousePos = position;
            m_CallbackParam.deltaMose = deltaPosition;
            if (/*!m_pDoingNode.IsAutoNext() && */OnNodeSign(m_pDoingNode))
            {
                OnNodeSignCompleted();
                DoNext();
            }
        }
        //------------------------------------------------------
        public void OnTouchMove(int touchId, Vector2 position, Vector2 deltaPosition)
        {
            if (m_pDoingNode == null || m_fDeltaDelta > 0) return;
            if (m_nTouchID == touchId)
            {
                m_CallbackParam.Clear();
                m_CallbackParam.touchType = ETouchType.Move;
                m_CallbackParam.touchId = touchId;
                m_CallbackParam.mousePos = position;
                m_CallbackParam.deltaMose = deltaPosition;
                if (/* !m_pDoingNode.IsAutoNext() && */ OnNodeSign(m_pDoingNode))
                {
                    OnNodeSignCompleted();
                    DoNext();
                }
            }
        }
        //------------------------------------------------------
        public void OnTouchEnd(int touchId, Vector2 position, Vector2 deltaPosition)
        {
            if (m_pDoingNode == null || m_fDeltaDelta > 0) return;
            if (m_nTouchID == touchId)
            {
                m_CallbackParam.Clear();
                m_CallbackParam.touchType = ETouchType.End;
                m_CallbackParam.touchId = touchId;
                m_CallbackParam.mousePos = position;
                m_CallbackParam.deltaMose = deltaPosition;
                if (OnNodeSign(m_pDoingNode))//!m_pDoingNode.IsAutoNext() &&  如果步骤节点在勾上自动跳转后,手动触发,这边就不能通过
                {
                    OnNodeSignCompleted();
                    DoNext();
                }

                m_nTouchID = -1;
            }
        }
        //------------------------------------------------------
        public IUserData BuildEvent(string strCmd)
        {
            if (m_vCallbacks == null || m_vCallbacks.Count <= 0) return null;
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                IUserData eventData = m_vCallbacks[j].OnGuideBuildEvent(strCmd);
                if (eventData != null)
                    return eventData;
            }
            return null;
        }
        //------------------------------------------------------
        void OnEvent(List<IUserData> vEvents)
        {
            if (m_vCallbacks.Count <= 0 || vEvents == null) return;
            for(int i = 0; i < vEvents.Count; ++i)
            {
                for (int j = 0; j < m_vCallbacks.Count; ++j)
                {
                    if (m_vCallbacks[j].OnTriggerGuideEvent(vEvents[i], null))
                        break;
                }
            }
        }
        //------------------------------------------------------
        void OnNodeCall(BaseNode pNode)
        {
            if (m_vCallbacks.Count <= 0 || pNode == null) return;


            if (pNode is Framework.Guide.StepNode)
            {
                if (m_GuidePanel != null)
                    m_GuidePanel.ClearWidget();
                GuideStepHandler.OnGuideExecuteNode(pNode as Framework.Guide.StepNode);
            }
            if (pNode is Framework.Guide.ExcudeNode)
            {
                Framework.Guide.GuideExecudeHandler.OnGuideExecuteNode(pNode as Framework.Guide.ExcudeNode);
            }

            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                m_vCallbacks[j].OnGuideExecuteNode(pNode);
            }
        }
        //------------------------------------------------------
        bool OnNodeSign(BaseNode pNode)
        {
            if (m_vCallbacks.Count <= 0 || pNode == null) return false;

            //延迟交互检测时间
            if (m_fDeltaSign > 0)
            {
                return false;
            }

            if (pNode is Framework.Guide.StepNode)
            {
                if (GuideStepHandler.OnGuideCheckSign(pNode as Framework.Guide.StepNode, m_CallbackParam))
                {
                    //! 说明改节点已经完成，清理状态
                    if (m_GuidePanel != null)
                    {
                        m_GuidePanel.bDoing = false;
                        m_GuidePanel.ClearWidget();
                    }
                    return true;
                }
            }

            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                if (m_vCallbacks[j].OnGuideCheckSign(pNode, m_CallbackParam))
                {
                    //! 说明改节点已经完成，清理状态
                    if (m_GuidePanel != null)
                    {
                        m_GuidePanel.bDoing = false;
                        m_GuidePanel.ClearWidget();
                    }
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        void OnGuideGroupStatus(TriggerNode triggerNode, bool bDoing, bool bCheckRecord)
        {
            if (bDoing)
            {
                if(m_GuidePanel!=null)
                {
                    m_GuidePanel.ClearData();//引导结束时清空缓存
                    m_GuidePanel.Show();
                }
            }
            else
            {
                if (m_GuidePanel != null)
                {
                    m_GuidePanel.bDoing = false;
                    m_GuidePanel.Hide();
                }
            }
            if (m_vCallbacks.Count <= 0) return;
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                m_vCallbacks[j].OnGuideGroupStatus(triggerNode, bDoing, bCheckRecord);
            }
        }
        //------------------------------------------------------
        bool OnGuideSuccssedListener(BaseNode pNode)
        {
            if (m_vCallbacks.Count <= 0) return true;
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                if (!m_vCallbacks[j].OnGuideSuccssedListener(pNode))
                    return false;
            }
            return true;
        }
        //------------------------------------------------------
        public string ConvertLanguage(string id)
        {
            if (m_vCallbacks.Count <= 0) return id.ToString();
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                string str = m_vCallbacks[j].OnGuideLanguageContent(id);
                if (!string.IsNullOrEmpty(str))
                    return str;
            }
            return id.ToString();
        }
        //------------------------------------------------------
        public void OnGUI()
        {

        }
        //------------------------------------------------------
        public void OnStuckSkipGuide(ushort tag)
        {
            if (m_bEnableStuckSkip == false)
            {
                return;
            }
            if (DoingSeqNode != null)
            {
                UpdateFlag(tag);
            }
            OverGuide(false);
        }
        //------------------------------------------------------
        public bool LoadAsset(UnityEngine.Object pCall, string file, bool bAsync)
        {
            if (m_vCallbacks == null)
                return false;

            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                if (m_vCallbacks[j].OnGuideLoadAsset(pCall, file, bAsync))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool UnloadAsset(string file, UnityEngine.Object pObj)
        {
            if (m_vCallbacks == null)
                return false;
            for (int j = 0; j < m_vCallbacks.Count; ++j)
            {
                if (m_vCallbacks[j].OnGuideUnLoadAsset(file, pObj))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static void Log(string log,ELogType logType = ELogType.Warning)
        {
            if (ms_Instance != null && ms_Instance.m_bEnableSystem && ms_Instance.bGuideLogEnable)
            {
                switch (logType)
                {
                    case ELogType.Info:
                        UnityEngine.Debug.Log("GuideLog: " + log);
                        break;
                    case ELogType.Warning:
                        UnityEngine.Debug.LogWarning("GuideLog: " + log);
                        break;
                    case ELogType.Error:
                        UnityEngine.Debug.LogError("GuideLog: " + log);
                        break;
                    default:
                        UnityEngine.Debug.Log("GuideLog: " + log);
                        break;
                }
                
            }
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        public void SetTestFlag(bool flag)
        {
            bTestFlag = flag;
        }
#endif
    }
}

