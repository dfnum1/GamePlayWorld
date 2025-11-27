/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideGuid
作    者:	HappLI
描    述:	引导条件节点
*********************************************************************/
using System.Collections.Generic;
namespace Framework.Guide
{
    public enum ERelationType
    {
        [GuideDisplay("且")]
        And = 0,
        [GuideDisplay("或")]
        Or = 1,
    }
    public enum EOpType
    {
        [GuideDisplay("无")]
        None,
        [GuideDisplay(">"), GuideDisable(typeof(string))]
        ValueGreater,
        [GuideDisplay("<"), GuideDisable(typeof(string))]
        ValueLess,
        [GuideDisplay(">="), GuideDisable(typeof(string))]
        ValueGreaterEqual,
        [GuideDisplay("<="), GuideDisable(typeof(string))]
        ValueLessEqual,
        [GuideDisplay("==")]
        ValueEqual,
        [GuideDisplay("!=")]
        UnEqual,
        [GuideDisplay("与"), GuideDisable(typeof(string))]
        Xor,
    }

    [System.Serializable]
    public class GuideOperate : BaseNode
    {
        public VarPort[] Vars = null;  // max 8
        public byte Relation = 0;

        public int NextNode = 0;
        [System.NonSerialized]
        public BaseNode pNext = null;

#if UNITY_EDITOR
        [System.NonSerialized]
        public List<VarPort> _Ports = new List<VarPort>();

#endif
        //-----------------------------------------------------
        public override int GetEnumType()
        {
            return -1;
        }
        //-----------------------------------------------------
        public override void Init(GuideGroup pGroup)
        {
            base.Init(pGroup);
            if (Vars != null)
            {
                for (int i = 0; i < Vars.Length; ++i)
                {
                    Vars[i].Init(pGroup);
                }
            }
            pNext = null;
            if (NextNode != 0)
            {
                pNext = pGroup.GetNode<BaseNode>(NextNode);
            }
#if UNITY_EDITOR
            _Ports.Clear();
            if (Vars != null)
            {
                for (int i = 0; i < Vars.Length; ++i)
                {
                    _Ports.Add(Vars[i]);
                }
            }
#endif
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public override void Save()
        {
            base.Save();
            for (int i = 0; i < _Ports.Count; ++i)
                _Ports[i].Save();
            Vars = _Ports.ToArray();

            if(pNext!=null && !(pNext is GuideOperate) )
            {
                NextNode = pNext.Guid;
            }
            else
                NextNode = 0;
        }
#endif
        //------------------------------------------------------
        public bool Operate(List<BaseNode> vTrackNode)
        {
            if (vTrackNode.Count <=0) return false;
            if (Vars != null && Vars.Length > 0)
            {
                bool bOrOp = false;
                bool bOk = false;
                byte bSuccsseCnt = 0;
                byte bFailedCnt = 0;
                for (int i = 0; i < Vars.Length; ++i)
                {
                    bool bSuccssed = false;
                    ArgvPort port = Vars[i].GetDummyArgv(vTrackNode);
                    if (port != null)
                    {
                        switch (Vars[i].type)
                        {
                            case EOpType.ValueGreater:
                                {
                                    bSuccssed = port.fillValue > Vars[i].value;
                                }
                                break;
                            case EOpType.ValueLess:
                                {
                                    bSuccssed = port.fillValue < Vars[i].value;
                                }
                                break;
                            case EOpType.ValueGreaterEqual:
                                {
                                    bSuccssed = port.fillValue >= Vars[i].value;
                                }
                                break;
                            case EOpType.ValueLessEqual:
                                {
                                    bSuccssed = port.fillValue <= Vars[i].value;
                                }
                                break;
                            case EOpType.ValueEqual:
                                {
                                    if(!string.IsNullOrEmpty(Vars[i].strValue))
                                    {
                                        bSuccssed = Vars[i].strValue.CompareTo(port.fillStrValue) == 0;
                                    }
                                    if (!string.IsNullOrEmpty(port.fillStrValue))
                                    {
                                        bSuccssed = port.fillStrValue.CompareTo(Vars[i].strValue) == 0;
                                    }
                                    else
                                        bSuccssed = Vars[i].value == port.fillValue;
                                }
                                break;
                            case EOpType.UnEqual:
                                {
                                    if (!string.IsNullOrEmpty(Vars[i].strValue))
                                    {
                                        bSuccssed = Vars[i].strValue.CompareTo(port.fillStrValue) != 0;
                                    }
                                    else if (!string.IsNullOrEmpty(port.fillStrValue))
                                    {
                                        bSuccssed = port.fillStrValue.CompareTo(Vars[i].strValue) != 0;
                                    }
                                    else
                                        bSuccssed = Vars[i].value != port.fillValue;
                                }
                                break;
                            case EOpType.Xor:
                                {
                                    bSuccssed = (port.fillValue & Vars[i].value) != 0;
                                }
                                break;
                            case EOpType.None:
                                {
                                    bSuccssed = true;
                                }
                                break;
                        }
                    }
                    else
                        bSuccssed = true;

                    if (bSuccssed) bSuccsseCnt++;
                    else bFailedCnt++;

                    if (i + 1 < Vars.Length)
                    {
                        if ((Relation & (byte)1 << i) != 0) bOrOp = true;
                    }
                }
                if(bOrOp)
                {
                    if(bSuccsseCnt >0) bOk = true;
                }
                else
                {
                    if (bFailedCnt <= 0 && bSuccsseCnt>0) bOk = true;
                }
                if (!bOk) return false;
            }
            return true;
        }
    }
}
