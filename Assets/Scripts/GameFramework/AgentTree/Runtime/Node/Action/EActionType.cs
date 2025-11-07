/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	BaseNode
作    者:	HappLI
描    述:	动作节点
*********************************************************************/
using System;
using UnityEngine;
namespace Framework.Plugin.AT
{
    public enum EActionType : int
    {
        None = 0,

        NewVariable = 1,
        FieldVariable = 2,
        CastVariable = 3,
        ATFunction = 4,
        DelegateCallback = 5,

        Op_Add = 10,
        Op_Sub = 11,
        Op_Mul = 12,
        Op_Dev = 13,
        Op_Reverse = 14,

        Op_Dot = 20,
        Op_Cross = 21,
        Op_Distance = 22,
        Op_CurveValue = 23,
        Op_CurveLenth = 24,
        Op_CurveDuration = 25,

        Op_EqualTo = 30,
        Op_AddEqualTo = 31,
        Op_SubEqualTo = 32,
        Op_MulEqualTo = 33,
        Op_DevEqualTo = 34,

        
        Op_MinValue = 40,
        Op_MaxValue = 41,
        Op_Clamp = 42,
        Op_ClampEqualTo = 43,
        Op_Lerp = 44,
        Op_LerpEqualTo = 45,
        Op_LerpColor = 46,
        Op_LerpVector2 = 47,
        Op_LerpVector3 = 48,
        Op_LerpVector4 = 49,

        Op_ListCount = 50,
        Op_ListUnion = 51,
        Op_ListCull = 52,
        Op_ListUnionTo = 53,
        Op_ListCullTo = 54,
        Op_RemoveListAt = 55,
        Op_RemoveItem = 56,
        Op_AddList = 57,
        Op_GetElementAt = 58,
        Op_IndexOfList = 59,

        Op_Clear = 60,
        Op_Destroy =61,
        Op_Delta = 62,
        Op_Random = 63,

        Condition_Begin = 70,
        Condition_IfElse = Condition_Begin,
        Condition_Switch = 71,
        Condition_Where = 72,
        Condition_Parallel = 73,
        Condition_Sync = 74,
        Condition_FrameDo = 75,
        Condition_IfOr = 76,
        Condition_IfAnd = 77,
        Condition_End,

        CustomTypeBegin = Condition_End+1,
    }

    public enum EATMouseType
    {
        Begin,
        Move,
        Wheel,
        End,
    }

    [ATExportMono("系统/鼠标输入数据"), ATExportGUID(-1)]
    public struct ATMouseData : IUserData
    {
        [Plugin.AT.ATField("状态", null, "", 1)]
        public EATMouseType state;
        [Plugin.AT.ATField("当前点击坐标", null, "", 1)]
        public Vector2 position;
        [Plugin.AT.ATField("上次点击坐标", null, "", 1)]
        public Vector2 lastPosition;
        [Plugin.AT.ATField("差值坐标", null, "", 1)]
        public Vector2 deltaPosition;
        [Plugin.AT.ATField("是否点击UI", null, "", 1)]
        public bool isUITouched;

        public void Destroy()
        {
        }
    }
}
