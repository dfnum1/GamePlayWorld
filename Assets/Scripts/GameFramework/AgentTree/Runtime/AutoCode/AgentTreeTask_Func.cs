/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AgentTreeTask
作    者:	HappLI
描    述:	
*********************************************************************/
namespace Framework.Plugin.AT
{
	public partial class AgentTreeTask
	{
		bool DoInerAction(ActionNode pNode, int funcId=0)
		{
		#if UNITY_EDITOR
			if (AgentTreeManager.OnExcudeAction != null) AgentTreeManager.OnExcudeAction(this, pNode);
		#endif
			int caseId = (funcId!=0)?funcId:(int)pNode.actionType;
			switch(caseId)
			{
				case 4: return Framework.Plugin.AT.OpCommon.ATFunction(this, pNode);//ATFunction
				case 5: return Framework.Plugin.AT.OpCommon.DelegateCallback(this, pNode);//DelegateCallback
				case 70: return Framework.Plugin.AT.OpCondition.IfElse(this, pNode);//Condition_Begin
				case 77: return Framework.Plugin.AT.OpCondition.IfAnd(this, pNode);//Condition_IfAnd
				case 76: return Framework.Plugin.AT.OpCondition.IfOr(this, pNode);//Condition_IfOr
				case 71: return Framework.Plugin.AT.OpCondition.Switch(this, pNode);//Condition_Switch
				case 72: return Framework.Plugin.AT.OpCondition.Where(this, pNode);//Condition_Where
				case 73: return Framework.Plugin.AT.OpCondition.Parallel(this, pNode);//Condition_Parallel
				case 74: return Framework.Plugin.AT.OpCondition.Sync(this, pNode);//Condition_Sync
				case 75: return Framework.Plugin.AT.OpCondition.FrameDo(this, pNode);//Condition_FrameDo
				case 60: return Framework.Plugin.AT.OpSwitcher.Op_Clear(this, pNode);//Op_Clear
				case 61: return Framework.Plugin.AT.OpSwitcher.Op_Destroy(this, pNode);//Op_Destroy
				case 10: return Framework.Plugin.AT.OpSwitcher.Op_Add(this, pNode);//Op_Add
				case 11: return Framework.Plugin.AT.OpSwitcher.Op_Sub(this, pNode);//Op_Sub
				case 12: return Framework.Plugin.AT.OpSwitcher.Op_Mul(this, pNode);//Op_Mul
				case 13: return Framework.Plugin.AT.OpSwitcher.Op_Dev(this, pNode);//Op_Dev
				case 14: return Framework.Plugin.AT.OpSwitcher.Op_Reverse(this, pNode);//Op_Reverse
				case 30: return Framework.Plugin.AT.OpSwitcher.Op_EqualTo(this, pNode);//Op_EqualTo
				case 31: return Framework.Plugin.AT.OpSwitcher.Op_AddEqualTo(this, pNode);//Op_AddEqualTo
				case 32: return Framework.Plugin.AT.OpSwitcher.Op_SubEqualTo(this, pNode);//Op_SubEqualTo
				case 33: return Framework.Plugin.AT.OpSwitcher.Op_MulEqualTo(this, pNode);//Op_MulEqualTo
				case 34: return Framework.Plugin.AT.OpSwitcher.Op_DevEqualTo(this, pNode);//Op_DevEqualTo
				case 20: return Framework.Plugin.AT.OpSwitcher.Op_Dot(this, pNode);//Op_Dot
				case 21: return Framework.Plugin.AT.OpSwitcher.Op_Cross(this, pNode);//Op_Cross
				case 22: return Framework.Plugin.AT.OpSwitcher.Op_Distance(this, pNode);//Op_Distance
				case 23: return Framework.Plugin.AT.OpSwitcher.Op_CurveValue(this, pNode);//Op_CurveValue
				case 24: return Framework.Plugin.AT.OpSwitcher.Op_CurveLenth(this, pNode);//Op_CurveLenth
				case 25: return Framework.Plugin.AT.OpSwitcher.Op_CurveDuration(this, pNode);//Op_CurveDuration
				case 40: return Framework.Plugin.AT.OpSwitcher.Op_MinValue(this, pNode);//Op_MinValue
				case 41: return Framework.Plugin.AT.OpSwitcher.Op_MaxValue(this, pNode);//Op_MaxValue
				case 42: return Framework.Plugin.AT.OpSwitcher.Op_Clamp(this, pNode);//Op_Clamp
				case 43: return Framework.Plugin.AT.OpSwitcher.Op_ClampEqualTo(this, pNode);//Op_ClampEqualTo
				case 44: return Framework.Plugin.AT.OpSwitcher.Op_Lerp(this, pNode);//Op_Lerp
				case 45: return Framework.Plugin.AT.OpSwitcher.Op_LerpEqualTo(this, pNode);//Op_LerpEqualTo
				case 50: return Framework.Plugin.AT.OpSwitcher.Op_ListCount(this, pNode);//Op_ListCount
				case 51: return Framework.Plugin.AT.OpSwitcher.Op_ListUnion(this, pNode);//Op_ListUnion
				case 53: return Framework.Plugin.AT.OpSwitcher.Op_ListUnionTo(this, pNode);//Op_ListUnionTo
				case 52: return Framework.Plugin.AT.OpSwitcher.Op_ListCull(this, pNode);//Op_ListCull
				case 54: return Framework.Plugin.AT.OpSwitcher.Op_ListCullTo(this, pNode);//Op_ListCullTo
				case 55: return Framework.Plugin.AT.OpSwitcher.Op_RemoveListAt(this, pNode);//Op_RemoveListAt
				case 56: return Framework.Plugin.AT.OpSwitcher.Op_RemoveItem(this, pNode);//Op_RemoveItem
				case 57: return Framework.Plugin.AT.OpSwitcher.Op_AddList(this, pNode);//Op_AddList
				case 58: return Framework.Plugin.AT.OpSwitcher.Op_GetElementAt(this, pNode);//Op_GetElementAt
				case 59: return Framework.Plugin.AT.OpSwitcher.Op_IndexOfList(this, pNode);//Op_IndexOfList
				case 46: return Framework.Plugin.AT.OpSwitcher.Op_LerpColor(this, pNode);//Op_LerpColor
				case 47: return Framework.Plugin.AT.OpSwitcher.Op_LerpVector2(this, pNode);//Op_LerpVector2
				case 48: return Framework.Plugin.AT.OpSwitcher.Op_LerpVector3(this, pNode);//Op_LerpVector3
				case 49: return Framework.Plugin.AT.OpSwitcher.Op_LerpVector4(this, pNode);//Op_LerpVector4
				case 62: return Framework.Plugin.AT.OpSwitcher.Op_Delta(this, pNode);//Op_Delta
				case 63: return Framework.Plugin.AT.OpSwitcher.Op_Random(this, pNode);//Op_Random
				case 1: return Framework.Plugin.AT.OpVariable.NewVariable(this, pNode);//NewVariable
				case 3: return Framework.Plugin.AT.OpVariable.CastVariable(this, pNode);//CastVariable
			}
			AgentTreeManager.getInstance().OnActionCallback(this, pNode, funcId);
			return true;
		}
	}
}
