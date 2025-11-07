using UnityEngine;

namespace Framework.Plugin.AT
{
    [ATExport("运算",true)]
    public class OpSwitcher
    {
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("数据清理", (int)EActionType.Op_Clear, true, true, true)]
        [ATMethodArgv(null, "变量")]
        public static bool Op_Clear(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween]
        [ATMethod("数据删除", (int)EActionType.Op_Destroy, true, true, true)]
        [ATMethodArgv(null, "变量")]
        public static bool Op_Destroy(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.StringList)]
        [ATMethod("加", (int)EActionType.Op_Add, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Add(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("减", (int)EActionType.Op_Sub, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Sub(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("乘", (int)EActionType.Op_Mul, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Mul(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("除", (int)EActionType.Op_Dev, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Dev(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Bool, EVariableType.ColorList)]
        [ATMethod("取反", (int)EActionType.Op_Reverse, true, true, true)]
        [ATMethodArgv(null, "值0")]
        public static bool Op_Reverse(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Bool, EVariableType.StringList)]
        [ATMethod("赋值", (int)EActionType.Op_EqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        public static bool Op_EqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.StringList)]
        [ATMethod("加等于", (int)EActionType.Op_AddEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        public static bool Op_AddEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("减等于", (int)EActionType.Op_SubEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        public static bool Op_SubEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("乘等于", (int)EActionType.Op_MulEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        public static bool Op_MulEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("除等于", (int)EActionType.Op_DevEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        public static bool Op_DevEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(new EVariableType[] { EVariableType.Vector2, EVariableType.Vector3,EVariableType.Vector2Int, EVariableType.Vector3Int })]
        [ATMethod("点乘", (int)EActionType.Op_Dot, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(typeof(VariableFloat), "计算结果")]
        public static bool Op_Dot(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(new EVariableType[] { EVariableType.Vector2, EVariableType.Vector3, EVariableType.Vector2Int, EVariableType.Vector3Int })]
        [ATMethod("叉乘", (int)EActionType.Op_Cross, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Cross(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(new EVariableType[] { EVariableType.Vector2, EVariableType.Vector3, EVariableType.Vector2Int, EVariableType.Vector3Int })]
        [ATMethod("点距离", (int)EActionType.Op_Distance, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(typeof(VariableFloat), "计算结果")]
        public static bool Op_Distance(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATMethod("曲线插值", (int)EActionType.Op_CurveValue, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "曲线")]
        [ATMethodArgv(typeof(VariableFloat), "采样值")]
        [ATMethodReturn(typeof(VariableFloat), "计算结果")]
        public static bool Op_CurveValue(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATMethod("曲线帧数", (int)EActionType.Op_CurveLenth, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "曲线")]
        [ATMethodReturn(typeof(VariableInt), "计算结果")]
        public static bool Op_CurveLenth(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATMethod("曲线时长", (int)EActionType.Op_CurveDuration, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "曲线")]
        [ATMethodReturn(typeof(VariableFloat), "计算结果")]
        public static bool Op_CurveDuration(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("取最小值", (int)EActionType.Op_MinValue, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_MinValue(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("取最大值", (int)EActionType.Op_MaxValue, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_MaxValue(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("区间校正Clamp", (int)EActionType.Op_Clamp, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "左区间")]
        [ATMethodArgv(null, "右区间")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Clamp(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("区间校正Clamp(返回自身)", (int)EActionType.Op_ClampEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "左区间")]
        [ATMethodArgv(null, "右区间")]
        public static bool Op_ClampEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("插值", (int)EActionType.Op_Lerp, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodArgv(typeof(VariableFloat), "采样值")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Lerp(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVarBetween(EVariableType.Byte, EVariableType.ColorList)]
        [ATMethod("插值(返回自身)", (int)EActionType.Op_LerpEqualTo, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodArgv(typeof(VariableFloat), "采样值")]
        public static bool Op_LerpEqualTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("获取列表大小", (int)EActionType.Op_ListCount, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodReturn(typeof(VariableInt), "列表大小")]
        public static bool Op_ListCount(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("列表合并", (int)EActionType.Op_ListUnion, true, true, true)]
        [ATMethodArgv(null, "表1")]
        [ATMethodArgv(null, "表2")]
        [ATMethodReturn(null, "合并表")]
        public static bool Op_ListUnion(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("列表合并(返回表1)", (int)EActionType.Op_ListUnionTo, true, true, true)]
        [ATMethodArgv(null, "表1", false, null, null, "", true)]
        [ATMethodArgv(null, "表2")]
        public static bool Op_ListUnionTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("列表剔除(表1-表2)", (int)EActionType.Op_ListCull, true, true, true)]
        [ATMethodArgv(null, "表1")]
        [ATMethodArgv(null, "表2")]
        [ATMethodReturn(null, "剔除后表")]
        public static bool Op_ListCull(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("列表剔除(返回表1)", (int)EActionType.Op_ListCullTo, true, true, true)]
        [ATMethodArgv(null, "表1", false, null, null, "", true)]
        [ATMethodArgv(null, "表2")]
        public static bool Op_ListCullTo(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("删除列表元素(根据索引)", (int)EActionType.Op_RemoveListAt, true, true, true)]
        [ATMethodArgv(null, "表1", false, null, null, "", true)]
        [ATMethodArgv(typeof(VariableInt), "下表索引")]
        public static bool Op_RemoveListAt(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("删除列表元素", (int)EActionType.Op_RemoveItem, true, true, true)]
        [ATMethodArgv(null, "表1", false, null, null, "", true)]
        [ATMethodArgv(null, "元素", false, null, null, "",false, 0)]
        public static bool Op_RemoveItem(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("添加列表元素", (int)EActionType.Op_AddList, true, true, true)]
        [ATMethodArgv(null, "表1", false, null, null, "", true)]
        [ATMethodArgv(null, "元素", false, null, null, "", false, 0)]
        public static bool Op_AddList(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("获取列表元素", (int)EActionType.Op_GetElementAt, true, true, true)]
        [ATMethodArgv(null, "表1")]
        [ATMethodArgv(typeof(VariableInt), "下标索引")]
        [ATMethodArgv(null, "元素", false, null, null, "", true, 0)]
        public static bool Op_GetElementAt(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATCanVars(true)]
        [ATMethod("获取元素在列表中的索引", (int)EActionType.Op_IndexOfList, true, true, true)]
        [ATMethodArgv(null, "表1")]
        [ATMethodArgv(null, "元素", false, null, null, "", false, 0)]
        [ATMethodArgv(typeof(VariableInt), "元素索引")]
        public static bool Op_IndexOfList(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            return OpAction(pTask, pAction, nFunctionId);
        }
        //------------------------------------------------------
        [ATMethod("颜色插值", (int)EActionType.Op_LerpColor, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "Alpha")]
        [ATMethodArgv(typeof(VariableCurve), "Red")]
        [ATMethodArgv(typeof(VariableCurve), "Green")]
        [ATMethodArgv(typeof(VariableCurve), "Blue")]
        [ATMethodArgv(typeof(VariableFloat), "LerpTime")]
        [ATMethodReturn(typeof(VariableColor), "颜色")]
        public static bool Op_LerpColor(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 5 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            VariableCurve alpha = pAction.GetInVariableByIndex<VariableCurve>(0, pTask);
            VariableCurve red = pAction.GetInVariableByIndex<VariableCurve>(1, pTask);
            VariableCurve green = pAction.GetInVariableByIndex<VariableCurve>(2, pTask);
            VariableCurve blue = pAction.GetInVariableByIndex<VariableCurve>(3, pTask);
            VariableFloat lerpTime = pAction.GetInVariableByIndex<VariableFloat>(4, pTask);
            if (alpha == null || red == null || green == null || blue == null || lerpTime == null)
            {
                AgentTreeUtl.LogWarning("参数为空");
                return true;
            }
            VariableColor color = pAction.GetOutVariableByIndex<VariableColor>(0, pTask);
            if(color == null)
            {
                AgentTreeUtl.LogWarning("返回值为空");
                return true;
            }
            if(alpha.mValue!=null) color.mValue.a = alpha.mValue.Evaluate(lerpTime.mValue);
            if (red.mValue != null) color.mValue.r = red.mValue.Evaluate(lerpTime.mValue);
            if (green.mValue != null) color.mValue.g = green.mValue.Evaluate(lerpTime.mValue);
            if (blue.mValue != null) color.mValue.b = blue.mValue.Evaluate(lerpTime.mValue);
            return true;
        }
        //------------------------------------------------------
        [ATMethod("Vector2插值", (int)EActionType.Op_LerpVector2, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "X")]
        [ATMethodArgv(typeof(VariableCurve), "Y")]
        [ATMethodArgv(typeof(VariableFloat), "LerpTime")]
        [ATMethodReturn(typeof(VariableVector2), "Vector2")]
        public static bool Op_LerpVector2(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 3 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            VariableCurve x = pAction.GetInVariableByIndex<VariableCurve>(0, pTask);
            VariableCurve y = pAction.GetInVariableByIndex<VariableCurve>(1, pTask);
            VariableFloat lerpTime = pAction.GetInVariableByIndex<VariableFloat>(2, pTask);
            if (x == null || y == null || lerpTime == null)
            {
                AgentTreeUtl.LogWarning("参数为空");
                return true;
            }
            VariableVector2 vec = pAction.GetOutVariableByIndex<VariableVector2>(0, pTask);
            if (vec == null)
            {
                AgentTreeUtl.LogWarning("返回值为空");
                return true;
            }
            if (x.mValue != null) vec.mValue.x = x.mValue.Evaluate(lerpTime.mValue);
            if (y.mValue != null) vec.mValue.y = y.mValue.Evaluate(lerpTime.mValue);
            return true;
        }
        //------------------------------------------------------
        [ATMethod("Vector3插值", (int)EActionType.Op_LerpVector3, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "X")]
        [ATMethodArgv(typeof(VariableCurve), "Y")]
        [ATMethodArgv(typeof(VariableCurve), "Z")]
        [ATMethodArgv(typeof(VariableFloat), "LerpTime")]
        [ATMethodReturn(typeof(VariableVector3), "Vector3")]
        public static bool Op_LerpVector3(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 4 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            VariableCurve x = pAction.GetInVariableByIndex<VariableCurve>(0, pTask);
            VariableCurve y = pAction.GetInVariableByIndex<VariableCurve>(1, pTask);
            VariableCurve z = pAction.GetInVariableByIndex<VariableCurve>(2, pTask);
            VariableFloat lerpTime = pAction.GetInVariableByIndex<VariableFloat>(3, pTask);
            if (x == null || y == null || z == null || lerpTime == null)
            {
                AgentTreeUtl.LogWarning("参数为空");
                return true;
            }
            VariableVector3 vec = pAction.GetOutVariableByIndex<VariableVector3>(0, pTask);
            if (vec == null)
            {
                AgentTreeUtl.LogWarning("返回值为空");
                return true;
            }
            if (x.mValue != null) vec.mValue.x = x.mValue.Evaluate(lerpTime.mValue);
            if (y.mValue != null) vec.mValue.y = y.mValue.Evaluate(lerpTime.mValue);
            if (z.mValue != null) vec.mValue.z = z.mValue.Evaluate(lerpTime.mValue);
            return true;
        }
        //------------------------------------------------------
        [ATMethod("Vector4插值", (int)EActionType.Op_LerpVector4, true, true, true)]
        [ATMethodArgv(typeof(VariableCurve), "X")]
        [ATMethodArgv(typeof(VariableCurve), "Y")]
        [ATMethodArgv(typeof(VariableCurve), "Z")]
        [ATMethodArgv(typeof(VariableCurve), "W")]
        [ATMethodArgv(typeof(VariableFloat), "LerpTime")]
        [ATMethodReturn(typeof(VariableVector4), "Vector4")]
        public static bool Op_LerpVector4(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 5 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
            {
                AgentTreeUtl.LogWarning("参数个数不匹配");
                return true;
            }
            VariableCurve x = pAction.GetInVariableByIndex<VariableCurve>(0, pTask);
            VariableCurve y = pAction.GetInVariableByIndex<VariableCurve>(1, pTask);
            VariableCurve z = pAction.GetInVariableByIndex<VariableCurve>(2, pTask);
            VariableCurve w = pAction.GetInVariableByIndex<VariableCurve>(3, pTask);
            VariableFloat lerpTime = pAction.GetInVariableByIndex<VariableFloat>(4, pTask);
            if (x == null || y == null || z == null || w == null || lerpTime == null)
            {
                AgentTreeUtl.LogWarning("参数为空");
                return true;
            }
            VariableVector4 vec = pAction.GetOutVariableByIndex<VariableVector4>(0, pTask);
            if (vec == null)
            {
                AgentTreeUtl.LogWarning("返回值为空");
                return true;
            }
            if (x.mValue != null) vec.mValue.x = x.mValue.Evaluate(lerpTime.mValue);
            if (y.mValue != null) vec.mValue.y = y.mValue.Evaluate(lerpTime.mValue);
            if (z.mValue != null) vec.mValue.z = z.mValue.Evaluate(lerpTime.mValue);
            if (w.mValue != null) vec.mValue.w = w.mValue.Evaluate(lerpTime.mValue);
            return true;
        }
        //------------------------------------------------------
        [ATMethod("延时", (int)EActionType.Op_Delta, true, true, true)]
        [ATMethodArgv(typeof(VariableFloat), "时间(s)")]
        public static bool Op_Delta(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 1)
                return true;

            VariableFloat var = pAction.GetInVariableByIndex<VariableFloat>(0, pTask);
            if (var == null) return true;
            pAction.fDeltaTime = var.mValue;
            pAction.bDeltaing = var.mValue>0;
            return !pAction.bDeltaing;
        }
        //------------------------------------------------------
        [ATCanVars(new EVariableType[] { EVariableType.Byte, EVariableType.Int, EVariableType.Long, EVariableType.Float })]
        [ATMethod("随机", (int)EActionType.Op_Random, true, true, true)]
        [ATMethodArgv(null, "值0")]
        [ATMethodArgv(null, "值1")]
        [ATMethodReturn(null, "计算结果")]
        public static bool Op_Random(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length != 2 || pAction.outArgvs == null || pAction.outArgvs.Length != 1)
                return true;
            Variable argv0 = pAction.GetInVariableByIndex<Variable>(0, pTask);
            Variable argv1 = pAction.GetInVariableByIndex<Variable>(1, pTask);
            Variable ret = pAction.GetOutVariableByIndex<Variable>(0, pTask);

            if (argv0 == null || argv1 == null || ret == null) return true;
            if (argv0.GetType() != argv1.GetType() || argv1.GetType() != ret.GetType()) return true;

            if(argv0 is VariableByte)
            {
                (ret as VariableByte).mValue = (byte)UnityEngine.Random.Range((argv0 as VariableByte).mValue, (argv1 as VariableByte).mValue);
                return true;
            }
            if (argv0 is VariableInt)
            {
                (ret as VariableInt).mValue = UnityEngine.Random.Range((argv0 as VariableInt).mValue, (argv1 as VariableInt).mValue);
                return true;
            }
            if (argv0 is VariableLong)
            {
                (ret as VariableLong).mValue = (long)UnityEngine.Random.Range((argv0 as VariableLong).mValue, (argv1 as VariableLong).mValue);
                return true;
            }
            if (argv0 is VariableFloat)
            {
                (ret as VariableFloat).mValue = UnityEngine.Random.Range((argv0 as VariableFloat).mValue, (argv1 as VariableFloat).mValue);
                return true;
            }
            return true;
        }
        //------------------------------------------------------
        static bool OpAction(AgentTreeTask pTask, ActionNode pAction, int nFunctionId = 0)
        {
            if (pAction.inArgvs == null || pAction.inArgvs.Length < 1)
            {
                return true;
            }
            Variable argv0 = pAction.GetInVariableByIndex<Variable>(0, pTask);
            if (argv0 == null) return true;
            Variable pRet = pAction.outArgvs.Length > 0 ? pAction.GetOutVariableByIndex<Variable>(0, pTask) : null;
            switch (pAction.actionType)
            {
                case EActionType.Op_Reverse: argv0.Reverse(); return true;
                case EActionType.Op_ListCount:
                    {
                        if (pRet != null && pRet is VariableInt && argv0 != null && argv0.IsList())
                        {
                            (pRet as VariableInt).mValue = argv0.GetList().Count;
                        }
                        return true;
                    }
                case EActionType.Op_Clear:
                    {
                        for (int i = 0; i < pAction.inArgvs.Length; ++i)
                        {
                            Variable pVar = pAction.GetInVariableByIndex<Variable>(i, pTask);
                            if (pVar != null)
                            {
                                pVar.Reset(AgentTreeUtl.intSet);
                            }
                        }
                        for (int i = 0; i < pAction.outArgvs.Length; ++i)
                        {
                            Variable pVar = pAction.GetOutVariableByIndex<Variable>(i, pTask);
                            if (pVar != null)
                            {
                                pVar.Reset(AgentTreeUtl.intSet);
                            }
                        }
                        return true;
                    }
                case EActionType.Op_Destroy:
                    {
                        for (int i = 0; i < pAction.inArgvs.Length; ++i)
                        {
                            Variable pVar = pAction.GetInVariableByIndex<Variable>(i, pTask);
                            if (pVar != null) pVar.Destroy();
                        }
                        for (int i = 0; i < pAction.outArgvs.Length; ++i)
                        {
                            Variable pVar = pAction.GetOutVariableByIndex<Variable>(i, pTask);
                            if (pVar != null) pVar.Destroy();
                        }
                        return true;
                    }
            }
            if (pAction.inArgvs == null || pAction.inArgvs.Length < 2)
            {
                return true;
            }

            Variable argv1 = pAction.GetInVariableByIndex<Variable>(1, pTask);
            if (argv1 == null) return true;
            if (pAction.actionType == EActionType.Op_Clamp)
            {
                if (pAction.inArgvs == null || pAction.inArgvs.Length < 3)
                {
                    Variable argv2 = pAction.GetInVariableByIndex<Variable>(2, pTask);
                    argv0.Clamp(argv1, argv2, pRet);
                }
                return true;
            }
            if (pAction.actionType == EActionType.Op_IndexOfList)
            {
                if (pRet != null && pRet is VariableInt && argv0 != null && argv0.IsList())
                {
                    (pRet as VariableInt).mValue = argv0.IndexofList(argv1);
                }
                return true;
            }
            switch (pAction.actionType)
            {
                case EActionType.Op_Add: if(pRet!=null) argv0.AddTo(argv1, pRet); break;
                case EActionType.Op_Sub: if (pRet != null) argv0.SubTo(argv1, pRet); break;
                case EActionType.Op_Mul: if (pRet != null) argv0.MulTo(argv1, pRet); break;
                case EActionType.Op_Dev: if (pRet != null) argv0.DevTo(argv1, pRet); break;
                case EActionType.Op_EqualTo: argv0.EqualTo(argv1); break;
                case EActionType.Op_AddEqualTo: argv0.AddTo(argv1); break;
                case EActionType.Op_SubEqualTo: argv0.AddTo(argv1); break;
                case EActionType.Op_MulEqualTo: argv0.AddTo(argv1); break;
                case EActionType.Op_DevEqualTo: argv0.AddTo(argv1); break;
                case EActionType.Op_MinValue: argv0.Min(argv1, pRet); break;
                case EActionType.Op_MaxValue: argv0.Max(argv1, pRet); break;
                case EActionType.Op_Dot:
                    {
                        if(pRet != null && pRet is VariableFloat && argv0 is VariableVector2)
                        {
                            VariableVector2 v0 = (VariableVector2)argv0;
                            VariableVector2 v1 = (VariableVector2)argv1;
                            (pRet as VariableFloat).mValue = Vector2.Dot(v0.mValue, v1.mValue);
                        }
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableVector3)
                        {
                            VariableVector3 v0 = (VariableVector3)argv0;
                            VariableVector3 v1 = (VariableVector3)argv1;
                            (pRet as VariableFloat).mValue = Vector3.Dot(v0.mValue, v1.mValue);
                        }
                    }
                    break;
                case EActionType.Op_Cross:
                    {
                        if (pRet != null && pRet is VariableVector3 && argv0 is VariableVector3)
                        {
                            VariableVector3 v0 = (VariableVector3)argv0;
                            VariableVector3 v1 = (VariableVector3)argv1;
                            (pRet as VariableVector3).mValue = Vector3.Cross(v0.mValue, v1.mValue);
                        }
                    }
                    break;
                case EActionType.Op_Distance:
                    {
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableVector3)
                        {
                            VariableVector3 v0 = (VariableVector3)argv0;
                            VariableVector3 v1 = (VariableVector3)argv1;
                            (pRet as VariableFloat).mValue = Vector3.Distance(v0.mValue, v1.mValue);
                        }
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableVector2)
                        {
                            VariableVector2 v0 = (VariableVector2)argv0;
                            VariableVector2 v1 = (VariableVector2)argv1;
                            (pRet as VariableFloat).mValue = Vector2.Distance(v0.mValue, v1.mValue);
                        }
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableVector2)
                        {
                            VariableVector2 v0 = (VariableVector2)argv0;
                            VariableVector2 v1 = (VariableVector2)argv1;
                            (pRet as VariableFloat).mValue = Vector2.Distance(v0.mValue, v1.mValue);
                        }
                    }
                    break;
                case EActionType.Op_CurveValue:
                    {
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableCurve && argv1 is VariableFloat)
                        {
                            VariableCurve v0 = (VariableCurve)argv0;
                            VariableFloat v1 = (VariableFloat)argv1;
                            (pRet as VariableFloat).mValue = v0.mValue.Evaluate(v1.mValue);
                        }
                    }
                    break;
                case EActionType.Op_CurveLenth:
                    {
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableCurve)
                        {
                            VariableCurve v0 = (VariableCurve)argv0;
                            (pRet as VariableFloat).mValue = v0.mValue.length;
                        }
                    }
                    break;
                case EActionType.Op_CurveDuration:
                    {
                        if (pRet != null && pRet is VariableFloat && argv0 is VariableCurve)
                        {
                            VariableCurve v0 = (VariableCurve)argv0;
                            if(v0.mValue.length>0)
                            (pRet as VariableFloat).mValue = v0.mValue[v0.mValue.length-1].time;
                        }
                    }
                    break;
                case EActionType.Op_Lerp:
                    {
                        if (pAction.inArgvs.Length>=3 && pRet != null && pRet.GetType() == argv0.GetType() && argv1 != null)
                        {
                            VariableFloat speed = pAction.GetInVariableByIndex<VariableFloat>(2, pTask);
                            if(speed!=null)
                            {
                                argv0.Lerp(argv1, speed.mValue,pRet);
                            }
                        }
                    }
                    break;
                case EActionType.Op_LerpEqualTo:
                    {
                        if (!argv0.IsFlag(EFlag.Const) && pAction.inArgvs.Length >= 3 && pRet.GetType() == argv0.GetType() && argv1 != null)
                        {
                            VariableFloat speed = pAction.GetInVariableByIndex<VariableFloat>(2, pTask);
                            if (speed != null)
                            {
                                argv0.Lerp(argv1, speed.mValue, argv0);
                            }
                        }
                    }
                    break;
                case EActionType.Op_ListUnion:
                    {
                        if (!argv0.IsFlag(EFlag.Const) && argv0.IsList() && argv1.IsList())
                        {
                            for(int i = 0; i < argv1.GetList().Count; ++i)
                            argv0.GetList().Add(argv1.GetList()[i]);
                        }
                    }
                    break;
                case EActionType.Op_ListCull:
                    {
                        if (!argv0.IsFlag(EFlag.Const) && argv0.IsList() && argv1.IsList())
                        {
                            for (int i = 0; i < argv1.GetList().Count; ++i)
                                argv0.GetList().Remove(argv1.GetList()[i]);
                        }
                    }
                    break;
                case EActionType.Op_RemoveListAt:
                    {
                        if (!argv0.IsFlag(EFlag.Const) && argv0 != null && argv0.IsList() && argv1 is VariableInt)
                        {
                            int index = ((VariableInt)argv1).mValue;
                            if (index >= 0 && index < argv0.GetList().Count)
                                argv0.GetList().RemoveAt(index);
                        }
                    }
                    break;
                case EActionType.Op_AddList:
                    {
                        if (!argv0.IsFlag(EFlag.Const) && argv0 != null && argv0.IsList() && argv1 != null)
                        {
                            argv0.AddToList(argv1);
                        }
                    }
                    break;
                case EActionType.Op_GetElementAt:
                    {
                        if (argv0.IsList() && argv1 is VariableInt)
                        {
                            argv0.GetListItem(((VariableInt)argv1).mValue, pRet);
                        }
                    }
                    break;
            }
            return true;
        }
    }
}