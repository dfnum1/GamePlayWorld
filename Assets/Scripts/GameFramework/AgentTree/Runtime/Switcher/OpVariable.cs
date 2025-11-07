namespace Framework.Plugin.AT
{
    [ATExport("变量",true)]
    public class OpVariable
    {
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATNoLink]
        [ATMethod("声明", (int)EActionType.NewVariable, true, true, false)]
        [ATMethodArgv(null, "变量",false, null, null, "", true)]
        public static bool NewVariable(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return true;
        }
        //------------------------------------------------------
        //[ATNoLink]
        //[ATMethod("AT成员变量", (int)EActionType.FieldVariable, true, true, false)]
        //[ATMethodArgv(typeof(VariableInt), "变量", false, null, null, "", true)]
        //[ATMethodArgv(typeof(VariableInt), "变量", false, null, null, "", true)]
        //[ATMethodReturn(null, "值", null, null, "", -1, true, true)]
        //public static bool FieldVariable(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        //{
        //    if (pAction.inArgvs == null || pAction.inArgvs.Length <= 0 || pAction.outArgvs == null || pAction.outArgvs.Length <= 0)
        //    {
        //        AgentTreeUtl.LogWarning("参数个数不匹配");
        //        return true;
        //    }

        //    if (pTask.pAT == null) return false;
        //    pTask.pAT.GetStruct();

        //    VariableUser pClass = pAction.GetInVariableByIndex<VariableUser>(0, pTask);
        //    if (pClass == null || pClass.hashCode == 0)
        //    {
        //        AgentTreeUtl.LogWarning("自定义类型为空");
        //        return true;
        //    }

        //    Variable pValue = pAction.GetOutVariableByIndex<Variable>(0, pTask);
        //    if (pValue == null)
        //    {
        //        AgentTreeUtl.LogWarning("数据承接变量为空!");
        //        return true;
        //    }

        //    return pTask.ExcudeAction(pAction, pAction.custumeValue);
        //}
        //------------------------------------------------------
        [ATMethod("变量转化", (int)EActionType.CastVariable, true, true, false)]
        [ATMethodArgv(typeof(VariableUser), "原对象", false, null, null, "", false)]
        [ATMethodReturn(typeof(VariableUser), "转化对象", null, null, "", -1, true, true)]
        public static bool CastVariable(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 1 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }

            VariableUser pClass = pAction.GetInVariableByIndex<VariableUser>(0, pTask);
            if (pClass == null || pClass.hashCode == 0)
            {
                AgentTreeUtl.LogWarning("自定义类型为空");
                return true;
            }

            VariableUser pToClass = pAction.GetOutVariableByIndex<VariableUser>(0, pTask);
            if (pToClass == null || pToClass.hashCode == 0)
            {
                AgentTreeUtl.LogWarning("转化对象类型为空!");
                return true;
            }
            pToClass.mValue = pClass.mValue;
            return true;
        }
    }
}