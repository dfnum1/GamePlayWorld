namespace Framework.Plugin.AT
{
    [ATExport("条件",true)]
    public class OpCondition
    {
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("是否-If Else", (int)EActionType.Condition_IfElse, true, true, true)]
        [ATMethodArgv(null, "变量",false, null, null, "", true)]
        public static bool IfElse(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return true;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }

            return pAction.condition.Excude(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("且-If And", (int)EActionType.Condition_IfAnd, true, true, true)]
        [ATMethodArgv(null, "变量", false, null, null, "", true)]
        public static bool IfAnd(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null || pAction.condition.portals == null || pAction.condition.portals.Count<=0)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return false;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length != pAction.condition.portals.Count)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return false;
            }

            return pAction.condition.Excude(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("或-If Or", (int)EActionType.Condition_IfOr, true, true, true)]
        [ATMethodArgv(null, "变量", false, null, null, "", true)]
        public static bool IfOr(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null || pAction.condition.portals == null || pAction.condition.portals.Count <= 0)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return false;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length != pAction.condition.portals.Count)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return false;
            }

            return pAction.condition.Excude(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("分流-Switch", (int)EActionType.Condition_Switch, true, true, true)]
        [ATMethodArgv(null, "变量", false, null, null, "", true)]
        public static bool Switch(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return true;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            pAction.condition.Excude(pTask, pAction, nFunctionId);
            return true;
        }
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("当-Where", (int)EActionType.Condition_Where, true, true, true)]
        [ATMethodArgv(null, "变量", false, null, null, "", true)]
        public static bool Where(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return true;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            return pAction.condition.Excude(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATMethod("并行", (int)EActionType.Condition_Parallel, true, true, true)]
        public static bool Parallel(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return true;
        }
        //------------------------------------------------------
        [ATMethod("同步", (int)EActionType.Condition_Sync, true, true, true)]
        public static bool Sync(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return true;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0 || pAction.outArgvs == null || pAction.outArgvs.Length <= 0)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            return pAction.condition.Excude(pTask, pAction, nFunctionId);

        }
        //------------------------------------------------------
        [ATMethod("帧执行", (int)EActionType.Condition_FrameDo, true, true, true)]
        [ATMethodArgv(typeof(VariableFloat), "秒", false, null, null, "", false)]
        [ATMethodArgv(typeof(VariableFloat), "当前秒", false, null, null, "", true)]
        public static bool FrameDo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.condition == null)
            {
                AgentTreeUtl.LogWarning("不是条件语句");
                return true;
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0 || pAction.outArgvs == null || pAction.outArgvs.Length <= 0)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            return pAction.condition.Excude(pTask, pAction, nFunctionId);
        }
    }
}