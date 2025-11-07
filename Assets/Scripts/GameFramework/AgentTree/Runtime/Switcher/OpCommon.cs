using UnityEngine;

namespace Framework.Plugin.AT
{
    [ATExport("基础",true)]
    public class OpCommon
    {
        //------------------------------------------------------
        [ATMethod("调用AT函数", (int)EActionType.ATFunction, true, true,false)]
        public static bool ATFunction(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            int guid = (int)(pAction.GetCustomValue() >> 32);
            int owner = (int)(pAction.GetCustomValue() & 0xffffffff);
            APINode pAPINode = pTask.GetAPINode(guid, owner);
            if(pAPINode == null)
            {
                AgentTreeUtl.LogWarning("未找到AT 函数");
                return false;
            }
            if(pAPINode.inArgvs!=null)
            {
                if (pAction.inArgvs == null || pAction.inArgvs.Length != pAPINode.inArgvs.Length)
                {
                    AgentTreeUtl.LogWarning("AT 函数参数不匹配");
                    return false;
                }

                for (int i = 0; i < pAPINode.inArgvs.Length; ++i)
                {
                    Variable inputVar = pAction.GetInVariableByIndex<Variable>(i);
                    Variable variable = pAPINode.GetInVariableByIndex<Variable>(i);
                    if(variable!=null && inputVar!=null)
                    {
                        if(inputVar.GetType() == variable.GetType())
                        {
                            if(variable.IsFlag( EFlag.Override))
                                variable.EqualTo(inputVar);
                        }
                        else
                        {
                            AgentTreeUtl.LogWarning("AT 函数参数不匹配");
                            return false;
                        }
                    }
                }
            }

            pTask.AddDoAction(pAPINode);
            return true;
        }
        //------------------------------------------------------
        [ATMethod("AT回调", (int)EActionType.DelegateCallback, true, true, false)]
        public static bool DelegateCallback(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return true;
        }
    }
}