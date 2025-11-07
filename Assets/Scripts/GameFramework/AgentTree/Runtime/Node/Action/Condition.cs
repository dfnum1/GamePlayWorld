/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	Condition
作    者:	HappLI
描    述:	动作节点
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    public enum ECondOpType : byte
    {
        None,
        [ATField("视野外")]
        [ATCanVars(new EVariableType[]
        {
            EVariableType.Vector2, EVariableType.Vector2Int, EVariableType.Vector3, EVariableType.Vector3Int,
            EVariableType.Vector2List, EVariableType.Vector2IntList, EVariableType.Vector3List, EVariableType.Vector3IntList, EVariableType.Vector4List,
            EVariableType.Object, EVariableType.ObjectList
        })]
        OutView,
        [ATField("视野内")]
        [ATCanVars(new EVariableType[]
        {
          EVariableType.Vector2, EVariableType.Vector2Int, EVariableType.Vector3, EVariableType.Vector3Int,
          EVariableType.Vector2List, EVariableType.Vector2IntList, EVariableType.Vector3List, EVariableType.Vector3IntList, EVariableType.Vector4List,
          EVariableType.Object, EVariableType.ObjectList
        })]
        InView,
        [ATField("距离范围内")]
        [ATCanVars(new EVariableType[]
        {
          EVariableType.Vector2, EVariableType.Vector2Int, EVariableType.Vector3, EVariableType.Vector3Int, EVariableType.Object
        })]
        PositionDistance,
        [ATField(">")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        ValueGreater,
        [ATField("<")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        ValueLess,
        [ATField(">=")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        ValueGreaterEqual,
        [ATField("<=")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        ValueLessEqual,
        [ATField("==")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        ValueEqual,
        [ATField("!=")]
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Float })]
        UnEqual,
        [ATField("空")]
        [ATCanVars(new EVariableType[] 
        { EVariableType.BoolList, EVariableType.ByteList, EVariableType.IntList, EVariableType.FloatList,
          EVariableType.Vector2List, EVariableType.Vector2IntList, EVariableType.Vector3List, EVariableType.Vector3IntList, EVariableType.Vector4List,
          EVariableType.QuaternionList,EVariableType.ColorList,EVariableType.String,EVariableType.Curve,EVariableType.CurveList,
          EVariableType.UserData,EVariableType.UserDataList,
          EVariableType.MonoScript, EVariableType.MonoScriptList,
          EVariableType.Object, EVariableType.ObjectList
        })]
        IsNull,
    }

    [Serializable]
    public class PortalNode
    {
        public ECondOpType opType = ECondOpType.None;

        public Port argv = null;
        public Port compare = null;

        public int ActionGuid = -1;
        public int[] excudeActions = null;
        [System.NonSerialized]
        public List<ExcudeNode> Actions = null;

        [System.NonSerialized]
        public bool bExcuded = false;
        //------------------------------------------------------
        public bool Excude(Variable param0, AgentTreeTask pTask)
        {
            Variable param1 = (argv!=null)?argv.GetVariable<Variable>(pTask) :null;
            Variable compareParam = (compare!=null)?compare.GetVariable<Variable>(pTask) :null;
            switch (opType)
            {
                case ECondOpType.InView:
                    {

                    }
                    break;
                case ECondOpType.OutView:
                    {

                    }
                    break;
                case ECondOpType.PositionDistance:
                    {
                        if (param1==null || compareParam == null || !(compareParam is VariableFloat)) return true;
                        VariableFloat fValue = compareParam as VariableFloat;
                        if (param0 is VariableVector2)
                        {
                            return Vector2.SqrMagnitude(((VariableVector2)param0).mValue-((VariableVector2)param1).mValue) <= fValue.mValue * fValue.mValue;
                        }
                        if (param0 is VariableVector3)
                        {
                            return Vector2.SqrMagnitude(((VariableVector3)param0).mValue - ((VariableVector3)param1).mValue) <= fValue.mValue * fValue.mValue;
                        }
                    }
                    break;
                case ECondOpType.ValueGreater:
                    {
                        if (param1 == null || param0.CompareTo(param1) > 0) return true;
                        else return false;
                    }
                case ECondOpType.ValueGreaterEqual:
                    {
                        if (param1 == null || param0.CompareTo(param1) >= 0) return true;
                        else return false;
                    }
                case ECondOpType.ValueLess:
                    {
                        if (param1 == null || param0.CompareTo(param1) < 0) return true;
                        else return false;
                    }
                case ECondOpType.ValueLessEqual:
                    {
                        if (param1 == null || param0.CompareTo(param1) <= 0) return true;
                        else return false;
                    }
                case ECondOpType.ValueEqual:
                    {
                        if (param1 == null || param0.CompareTo(param1) == 0) return true;
                        else return false;
                    }
                case ECondOpType.UnEqual:
                    {
                        if (param1 == null || param0.CompareTo(param1) != 0) return true;
                        else return false;
                    }
                case ECondOpType.IsNull:
                    {
                        return param0.isNull();
                    }
            }
            return true;
        }
        //------------------------------------------------------
        public void Copy(PortalNode pCond)
        {
            ActionGuid = pCond.ActionGuid;
            if(pCond.excudeActions != null && pCond.excudeActions.Length>0)
            {
                excudeActions = new int[pCond.excudeActions.Length];
                for (int i = 0; i < excudeActions.Length; ++i)
                    excudeActions[i] = pCond.excudeActions[i];
            }
            if(pCond.Actions != null)
            {
                Actions = new List<ExcudeNode>(pCond.Actions.Count);
                for (int i=0; i < pCond.Actions.Count; ++i)
                {
                    Actions.Add(pCond.Actions[i]);
                }
            }
            else
                Actions = null;
            opType = pCond.opType;
            if (pCond.argv != null)
            {
                argv = new Port(null);
                argv.Copy(pCond.argv);
            }
            if (pCond.compare != null)
            {
                compare = new Port(null);
                compare.Copy(pCond.compare);
            }
        }
        //------------------------------------------------------
        public void Init(AgentTree pTree)
        {
            if(excudeActions != null)
            {
                for(int i = 0; i < excudeActions.Length; ++i)
                {
                    if (ActionGuid == excudeActions[i])
                        ActionGuid = -1;
                    ExcudeNode node = pTree.GetExcudeNode(excudeActions[i]);
                    if (node != null)
                    {
                        node.Init(pTree);

                        if (Actions == null) Actions = new List<ExcudeNode>();
                        if(!Actions.Contains(node))
                            Actions.Add(node);
                    }
                }
            }
            if (ActionGuid != -1)
            {
                ExcudeNode node = pTree.GetExcudeNode(ActionGuid);
                if (node != null)
                {
                    node.Init(pTree);
                    if (Actions == null) Actions = new List<ExcudeNode>();
                    if (!Actions.Contains(node))
                        Actions.Add(node);
                }
            }
            if(argv != null)
            {
                argv.Init(pTree);
            }
            if (compare != null)
            {
                compare.Init(pTree);
            }
            bExcuded = false;
        }
        //------------------------------------------------------
        public void Reset()
        {
            if (Actions != null)
            {
                for (int i = 0; i < Actions.Count;)
                {
                    if (Actions[i] != null)
                    {
                        Actions[i].Reset(AgentTreeUtl.intSet);
                        ++i;
                    }
                    else
                        Actions.RemoveAt(i);
                }
            }
            if (argv != null) argv.Reset();
            if (compare != null) compare.Reset();
            bExcuded = false;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            if (Actions != null)
            {
                for (int i = 0; i < Actions.Count;)
                {
                    if (Actions[i] != null)
                    {
                        Actions[i].Destroy();
                        ++i;
                    }
                    else
                        Actions.RemoveAt(i);
                }
            }
            if (argv != null) argv.Destroy();
            if (compare != null) compare.Destroy();
            bExcuded = false;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void Save()
        {
            if (Actions != null)
            {
                ActionGuid = -1;
                List<int> temps = new List<int>();
                for (int i = 0; i < Actions.Count;)
                {
                    if (Actions[i] != null)
                    {
                        Actions[i].Save();
                        if(!temps.Contains(Actions[i].GUID))
                            temps.Add(Actions[i].GUID);
                        ++i;
                    }
                    else
                        Actions.RemoveAt(i);
                }
                excudeActions = temps.ToArray();
            }
            if (argv != null)
            {
                argv.Save();
            }
            if (compare != null)
            {
                compare.Save();
            }
        }
#endif
    }

    [Serializable]
    public class Condition
    {
        public List<PortalNode> portals = null;

        public void Copy(Condition pCond)
        {
            if (pCond.portals != null && pCond.portals.Count>0)
            {
                portals = new List<PortalNode>(pCond.portals.Count);
                for(int i = 0; i < pCond.portals.Count; ++i)
                {
                    PortalNode pN = new PortalNode();
                    pN.Copy(pCond.portals[i]);
                    portals.Add(pN);
                }
            }
            else
                portals = null;
        }
        //------------------------------------------------------
        public void Init(AgentTree pTree)
        {
            if(portals != null)
            {
                for (int i = 0; i < portals.Count; ++i)
                    portals[i].Init(pTree);
            }
        }
        //------------------------------------------------------
        public void Reset()
        {
            if (portals != null)
            {
                for (int i = 0; i < portals.Count; ++i)
                    portals[i].Reset();
            }
        }
        //------------------------------------------------------
        public void Destroy()
        {
            if (portals != null)
            {
                for (int i = 0; i < portals.Count; ++i)
                    portals[i].Destroy();
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void Save()
        {
            if (portals != null)
            {
                for (int i = 0; i < portals.Count; ++i)
                    portals[i].Save();
            }
        }
#endif
        //------------------------------------------------------
        public bool Excude(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (portals == null || portals.Count<=0)
            {
                AgentTreeUtl.LogWarning("没有条件入口");
                return true;
            }
            Variable pArgv0 = pAction.GetInVariableByIndex<Variable>(0, pTask);
            if (pArgv0 == null)
            {
                AgentTreeUtl.LogWarning("参数为空");
                return true;
            }
            if (pAction.actionType == EActionType.Condition_IfElse)
            {
                bool bMatched = false;
                for (int i = 0; i < portals.Count; ++i)
                {
                    if (portals[i].Excude(pArgv0, pTask))
                    {
                        if (portals[i].Actions != null)
                        {
                            for (int j = 0; j < portals[i].Actions.Count; ++j)
                            {
                                pTask.AddDoAction(portals[i].Actions[j]);
                            }
                        }
                        bMatched = true;
                        break;
                    }
                }
                return !bMatched;
            }
            if (pAction.actionType == EActionType.Condition_IfAnd)
            {
                bool bMatched = true;
                for (int i = 0; i < portals.Count; ++i)
                {
                    pArgv0 = pAction.GetInVariableByIndex<Variable>(i, pTask);
                    if (pArgv0 == null) return false;
                    if (!portals[i].Excude(pArgv0, pTask))
                    {
                        bMatched = false;
                        break;
                    }
                }
                return bMatched;
            }
            if (pAction.actionType == EActionType.Condition_IfOr)
            {
                bool bMatched = false;
                for (int i = 0; i < portals.Count; ++i)
                {
                    pArgv0 = pAction.GetInVariableByIndex<Variable>(i, pTask);
                    if (pArgv0 == null) return false;

                    if (portals[i].Excude(pArgv0, pTask))
                    {
                        bMatched = true;
                        break;
                    }
                }
                return bMatched;
            }
            if (pAction.actionType == EActionType.Condition_FrameDo)
            {
                if(pArgv0 is VariableFloat)
                {
                    VariableFloat pArgv1 = pAction.GetOutVariableByIndex<VariableFloat>(0, pTask);
                    if (pArgv1 == null)
                    {
                        AgentTreeUtl.LogWarning("参数为空");
                        return true;
                    }

                    if (pArgv1.mValue < ((VariableFloat)pArgv0).mValue )
                    {
                        pArgv1.mValue += pTask.GetDeltaTime();
                        for (int i = 0; i < portals.Count; ++i)
                        {
                            if(portals[i].Actions!=null)
                            {
                                for (int j = 0; j < portals[i].Actions.Count; ++j)
                                {
                                    pTask.ExcudeAction(portals[i].Actions[j], nFunctionId);
                                }
                            }
                        }
                        return false;
                    }
                    pArgv1.mValue = 0;
                }

                return true;
            }
            if (pAction.actionType == EActionType.Condition_Parallel)
            {
                for (int i = 0; i < portals.Count; ++i)
                {
                    if (portals[i].Excude(pArgv0, pTask))
                    {
                        if(portals[i].Actions!=null)
                        {
                            for (int j = 0; j < portals[i].Actions.Count; ++j)
                            {
                                pTask.ExcudeAction(portals[i].Actions[j], nFunctionId);
                            }
                        }
                    }
                }
                return true;
            }
            if (pAction.actionType == EActionType.Condition_Sync)
            {
                bool bExcude = true;
                for (int i = 0; i < portals.Count; ++i)
                {
                    if (!portals[i].bExcuded && portals[i].Excude(pArgv0, pTask))
                    {
                        if (portals[i].Actions != null)
                        {
                            for(int j = 0; j < portals[i].Actions.Count; ++j)
                            {
                                if (pTask.ExcudeAction(portals[i].Actions[j], nFunctionId))
                                {
                                    portals[i].bExcuded = true;
                                    bExcude = false;
                                }
                            }
                        }
                    }
                }
                if(bExcude)
                {
                    for (int i = 0; i < portals.Count; ++i)
                    {
                        portals[i].bExcuded = false;
                    }
                }
                return bExcude;
            }
            if (pAction.actionType == EActionType.Condition_Switch)
            {
                for (int i = 0; i < portals.Count; ++i)
                {
                    if(portals[i].Excude(pArgv0, pTask))
                    {
                        if(portals[i].Actions!=null)
                        {
                            for(int j = 0; j < portals[i].Actions.Count; ++j)
                                pTask.ExcudeAction(portals[i].Actions[j], nFunctionId);
                        }
                        break;
                    }
                }
                return true;
            }
            if (pAction.actionType == EActionType.Condition_Where)
            {
                if(portals.Count == 1 && portals[0] != null)
                {
                    PortalNode pN = portals[0];
                    Variable pArgvGet = pN.argv.GetVariable(pTask);
                    if (pN.argv == null || pArgvGet == null)
                    {
                        AgentTreeUtl.LogWarning("比对参数为空");
                        return true;
                    }
                    if (pArgvGet.GetType() != pArgv0.GetType())
                    {
                        AgentTreeUtl.LogWarning("比对参数类型不一致");
                        return true;
                    }

                    if (pN.Excude(pArgv0, pTask))
                    {
                        if (pN.Actions != null)
                        {
                            for(int j =0; j < pN.Actions.Count; ++j)
                                pTask.ExcudeAction(pN.Actions[j], nFunctionId);
                        }
                        return false;
                    }
                    else
                        return true;
                }
                return true;
            }

            return true;
        }
    }
}
