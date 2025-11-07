//auto generator
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
	public class VariablePools
	{
		public List<Framework.Plugin.AT.VariableBool> varVariableBool;
		public List<Framework.Plugin.AT.VariableBoolList> varVariableBoolList;
		public List<Framework.Plugin.AT.VariableByte> varVariableByte;
		public List<Framework.Plugin.AT.VariableByteList> varVariableByteList;
		public List<Framework.Plugin.AT.VariableColor> varVariableColor;
		public List<Framework.Plugin.AT.VariableColorList> varVariableColorList;
		public List<Framework.Plugin.AT.VariableCurve> varVariableCurve;
		public List<Framework.Plugin.AT.VariableCurveList> varVariableCurveList;
		public List<Framework.Plugin.AT.VariableDelegate> varVariableDelegate;
		public List<Framework.Plugin.AT.VariableFloat> varVariableFloat;
		public List<Framework.Plugin.AT.VariableFloatList> varVariableFloatList;
		public List<Framework.Plugin.AT.VariableInt> varVariableInt;
		public List<Framework.Plugin.AT.VariableIntList> varVariableIntList;
		public List<Framework.Plugin.AT.VariableLong> varVariableLong;
		public List<Framework.Plugin.AT.VariableLongList> varVariableLongList;
		public List<Framework.Plugin.AT.VariableMonoScript> varVariableMonoScript;
		public List<Framework.Plugin.AT.VariableMonoScriptList> varVariableMonoScriptList;
		public List<Framework.Plugin.AT.VariableObject> varVariableObject;
		public List<Framework.Plugin.AT.VariableObjectList> varVariableObjectList;
		public List<Framework.Plugin.AT.VariableQuaternion> varVariableQuaternion;
		public List<Framework.Plugin.AT.VariableQuaternionList> varVariableQuaternionList;
		public List<Framework.Plugin.AT.VariableString> varVariableString;
		public List<Framework.Plugin.AT.VariableStringList> varVariableStringList;
		public List<Framework.Plugin.AT.VariableUser> varVariableUser;
		public List<Framework.Plugin.AT.VariableUserList> varVariableUserList;
		public List<Framework.Plugin.AT.VariableVector2> varVariableVector2;
		public List<Framework.Plugin.AT.VariableVector2List> varVariableVector2List;
		public List<Framework.Plugin.AT.VariableVector2Int> varVariableVector2Int;
		public List<Framework.Plugin.AT.VariableVector2IntList> varVariableVector2IntList;
		public List<Framework.Plugin.AT.VariableVector3> varVariableVector3;
		public List<Framework.Plugin.AT.VariableVector3List> varVariableVector3List;
		public List<Framework.Plugin.AT.VariableVector3Int> varVariableVector3Int;
		public List<Framework.Plugin.AT.VariableVector3IntList> varVariableVector3IntList;
		public List<Framework.Plugin.AT.VariableVector4> varVariableVector4;
		public List<Framework.Plugin.AT.VariableVector4List> varVariableVector4List;
		public void Init(int max)
		{
			varVariableBool=new List<Framework.Plugin.AT.VariableBool>(max);
			varVariableBoolList=new List<Framework.Plugin.AT.VariableBoolList>(max);
			varVariableByte=new List<Framework.Plugin.AT.VariableByte>(max);
			varVariableByteList=new List<Framework.Plugin.AT.VariableByteList>(max);
			varVariableColor=new List<Framework.Plugin.AT.VariableColor>(max);
			varVariableColorList=new List<Framework.Plugin.AT.VariableColorList>(max);
			varVariableCurve=new List<Framework.Plugin.AT.VariableCurve>(max);
			varVariableCurveList=new List<Framework.Plugin.AT.VariableCurveList>(max);
			varVariableDelegate=new List<Framework.Plugin.AT.VariableDelegate>(max);
			varVariableFloat=new List<Framework.Plugin.AT.VariableFloat>(max);
			varVariableFloatList=new List<Framework.Plugin.AT.VariableFloatList>(max);
			varVariableInt=new List<Framework.Plugin.AT.VariableInt>(max);
			varVariableIntList=new List<Framework.Plugin.AT.VariableIntList>(max);
			varVariableLong=new List<Framework.Plugin.AT.VariableLong>(max);
			varVariableLongList=new List<Framework.Plugin.AT.VariableLongList>(max);
			varVariableMonoScript=new List<Framework.Plugin.AT.VariableMonoScript>(max);
			varVariableMonoScriptList=new List<Framework.Plugin.AT.VariableMonoScriptList>(max);
			varVariableObject=new List<Framework.Plugin.AT.VariableObject>(max);
			varVariableObjectList=new List<Framework.Plugin.AT.VariableObjectList>(max);
			varVariableQuaternion=new List<Framework.Plugin.AT.VariableQuaternion>(max);
			varVariableQuaternionList=new List<Framework.Plugin.AT.VariableQuaternionList>(max);
			varVariableString=new List<Framework.Plugin.AT.VariableString>(max);
			varVariableStringList=new List<Framework.Plugin.AT.VariableStringList>(max);
			varVariableUser=new List<Framework.Plugin.AT.VariableUser>(max);
			varVariableUserList=new List<Framework.Plugin.AT.VariableUserList>(max);
			varVariableVector2=new List<Framework.Plugin.AT.VariableVector2>(max);
			varVariableVector2List=new List<Framework.Plugin.AT.VariableVector2List>(max);
			varVariableVector2Int=new List<Framework.Plugin.AT.VariableVector2Int>(max);
			varVariableVector2IntList=new List<Framework.Plugin.AT.VariableVector2IntList>(max);
			varVariableVector3=new List<Framework.Plugin.AT.VariableVector3>(max);
			varVariableVector3List=new List<Framework.Plugin.AT.VariableVector3List>(max);
			varVariableVector3Int=new List<Framework.Plugin.AT.VariableVector3Int>(max);
			varVariableVector3IntList=new List<Framework.Plugin.AT.VariableVector3IntList>(max);
			varVariableVector4=new List<Framework.Plugin.AT.VariableVector4>(max);
			varVariableVector4List=new List<Framework.Plugin.AT.VariableVector4List>(max);
		}
		public void Clear()
		{
			for(int i = 0; i < varVariableBool.Count; ++i)
				varVariableBool[i].Destroy();
			for(int i = 0; i < varVariableBoolList.Count; ++i)
				varVariableBoolList[i].Destroy();
			for(int i = 0; i < varVariableByte.Count; ++i)
				varVariableByte[i].Destroy();
			for(int i = 0; i < varVariableByteList.Count; ++i)
				varVariableByteList[i].Destroy();
			for(int i = 0; i < varVariableColor.Count; ++i)
				varVariableColor[i].Destroy();
			for(int i = 0; i < varVariableColorList.Count; ++i)
				varVariableColorList[i].Destroy();
			for(int i = 0; i < varVariableCurve.Count; ++i)
				varVariableCurve[i].Destroy();
			for(int i = 0; i < varVariableCurveList.Count; ++i)
				varVariableCurveList[i].Destroy();
			for(int i = 0; i < varVariableDelegate.Count; ++i)
				varVariableDelegate[i].Destroy();
			for(int i = 0; i < varVariableFloat.Count; ++i)
				varVariableFloat[i].Destroy();
			for(int i = 0; i < varVariableFloatList.Count; ++i)
				varVariableFloatList[i].Destroy();
			for(int i = 0; i < varVariableInt.Count; ++i)
				varVariableInt[i].Destroy();
			for(int i = 0; i < varVariableIntList.Count; ++i)
				varVariableIntList[i].Destroy();
			for(int i = 0; i < varVariableLong.Count; ++i)
				varVariableLong[i].Destroy();
			for(int i = 0; i < varVariableLongList.Count; ++i)
				varVariableLongList[i].Destroy();
			for(int i = 0; i < varVariableMonoScript.Count; ++i)
				varVariableMonoScript[i].Destroy();
			for(int i = 0; i < varVariableMonoScriptList.Count; ++i)
				varVariableMonoScriptList[i].Destroy();
			for(int i = 0; i < varVariableObject.Count; ++i)
				varVariableObject[i].Destroy();
			for(int i = 0; i < varVariableObjectList.Count; ++i)
				varVariableObjectList[i].Destroy();
			for(int i = 0; i < varVariableQuaternion.Count; ++i)
				varVariableQuaternion[i].Destroy();
			for(int i = 0; i < varVariableQuaternionList.Count; ++i)
				varVariableQuaternionList[i].Destroy();
			for(int i = 0; i < varVariableString.Count; ++i)
				varVariableString[i].Destroy();
			for(int i = 0; i < varVariableStringList.Count; ++i)
				varVariableStringList[i].Destroy();
			for(int i = 0; i < varVariableUser.Count; ++i)
				varVariableUser[i].Destroy();
			for(int i = 0; i < varVariableUserList.Count; ++i)
				varVariableUserList[i].Destroy();
			for(int i = 0; i < varVariableVector2.Count; ++i)
				varVariableVector2[i].Destroy();
			for(int i = 0; i < varVariableVector2List.Count; ++i)
				varVariableVector2List[i].Destroy();
			for(int i = 0; i < varVariableVector2Int.Count; ++i)
				varVariableVector2Int[i].Destroy();
			for(int i = 0; i < varVariableVector2IntList.Count; ++i)
				varVariableVector2IntList[i].Destroy();
			for(int i = 0; i < varVariableVector3.Count; ++i)
				varVariableVector3[i].Destroy();
			for(int i = 0; i < varVariableVector3List.Count; ++i)
				varVariableVector3List[i].Destroy();
			for(int i = 0; i < varVariableVector3Int.Count; ++i)
				varVariableVector3Int[i].Destroy();
			for(int i = 0; i < varVariableVector3IntList.Count; ++i)
				varVariableVector3IntList[i].Destroy();
			for(int i = 0; i < varVariableVector4.Count; ++i)
				varVariableVector4[i].Destroy();
			for(int i = 0; i < varVariableVector4List.Count; ++i)
				varVariableVector4List[i].Destroy();
		}
		public void Destroy()
		{
			for(int i = 0; i < varVariableBool.Count; ++i)
				varVariableBool[i].Destroy();
				varVariableBool=null;
			for(int i = 0; i < varVariableBoolList.Count; ++i)
				varVariableBoolList[i].Destroy();
				varVariableBoolList=null;
			for(int i = 0; i < varVariableByte.Count; ++i)
				varVariableByte[i].Destroy();
				varVariableByte=null;
			for(int i = 0; i < varVariableByteList.Count; ++i)
				varVariableByteList[i].Destroy();
				varVariableByteList=null;
			for(int i = 0; i < varVariableColor.Count; ++i)
				varVariableColor[i].Destroy();
				varVariableColor=null;
			for(int i = 0; i < varVariableColorList.Count; ++i)
				varVariableColorList[i].Destroy();
				varVariableColorList=null;
			for(int i = 0; i < varVariableCurve.Count; ++i)
				varVariableCurve[i].Destroy();
				varVariableCurve=null;
			for(int i = 0; i < varVariableCurveList.Count; ++i)
				varVariableCurveList[i].Destroy();
				varVariableCurveList=null;
			for(int i = 0; i < varVariableDelegate.Count; ++i)
				varVariableDelegate[i].Destroy();
				varVariableDelegate=null;
			for(int i = 0; i < varVariableFloat.Count; ++i)
				varVariableFloat[i].Destroy();
				varVariableFloat=null;
			for(int i = 0; i < varVariableFloatList.Count; ++i)
				varVariableFloatList[i].Destroy();
				varVariableFloatList=null;
			for(int i = 0; i < varVariableInt.Count; ++i)
				varVariableInt[i].Destroy();
				varVariableInt=null;
			for(int i = 0; i < varVariableIntList.Count; ++i)
				varVariableIntList[i].Destroy();
				varVariableIntList=null;
			for(int i = 0; i < varVariableLong.Count; ++i)
				varVariableLong[i].Destroy();
				varVariableLong=null;
			for(int i = 0; i < varVariableLongList.Count; ++i)
				varVariableLongList[i].Destroy();
				varVariableLongList=null;
			for(int i = 0; i < varVariableMonoScript.Count; ++i)
				varVariableMonoScript[i].Destroy();
				varVariableMonoScript=null;
			for(int i = 0; i < varVariableMonoScriptList.Count; ++i)
				varVariableMonoScriptList[i].Destroy();
				varVariableMonoScriptList=null;
			for(int i = 0; i < varVariableObject.Count; ++i)
				varVariableObject[i].Destroy();
				varVariableObject=null;
			for(int i = 0; i < varVariableObjectList.Count; ++i)
				varVariableObjectList[i].Destroy();
				varVariableObjectList=null;
			for(int i = 0; i < varVariableQuaternion.Count; ++i)
				varVariableQuaternion[i].Destroy();
				varVariableQuaternion=null;
			for(int i = 0; i < varVariableQuaternionList.Count; ++i)
				varVariableQuaternionList[i].Destroy();
				varVariableQuaternionList=null;
			for(int i = 0; i < varVariableString.Count; ++i)
				varVariableString[i].Destroy();
				varVariableString=null;
			for(int i = 0; i < varVariableStringList.Count; ++i)
				varVariableStringList[i].Destroy();
				varVariableStringList=null;
			for(int i = 0; i < varVariableUser.Count; ++i)
				varVariableUser[i].Destroy();
				varVariableUser=null;
			for(int i = 0; i < varVariableUserList.Count; ++i)
				varVariableUserList[i].Destroy();
				varVariableUserList=null;
			for(int i = 0; i < varVariableVector2.Count; ++i)
				varVariableVector2[i].Destroy();
				varVariableVector2=null;
			for(int i = 0; i < varVariableVector2List.Count; ++i)
				varVariableVector2List[i].Destroy();
				varVariableVector2List=null;
			for(int i = 0; i < varVariableVector2Int.Count; ++i)
				varVariableVector2Int[i].Destroy();
				varVariableVector2Int=null;
			for(int i = 0; i < varVariableVector2IntList.Count; ++i)
				varVariableVector2IntList[i].Destroy();
				varVariableVector2IntList=null;
			for(int i = 0; i < varVariableVector3.Count; ++i)
				varVariableVector3[i].Destroy();
				varVariableVector3=null;
			for(int i = 0; i < varVariableVector3List.Count; ++i)
				varVariableVector3List[i].Destroy();
				varVariableVector3List=null;
			for(int i = 0; i < varVariableVector3Int.Count; ++i)
				varVariableVector3Int[i].Destroy();
				varVariableVector3Int=null;
			for(int i = 0; i < varVariableVector3IntList.Count; ++i)
				varVariableVector3IntList[i].Destroy();
				varVariableVector3IntList=null;
			for(int i = 0; i < varVariableVector4.Count; ++i)
				varVariableVector4[i].Destroy();
				varVariableVector4=null;
			for(int i = 0; i < varVariableVector4List.Count; ++i)
				varVariableVector4List[i].Destroy();
				varVariableVector4List=null;
		}
	}
}
