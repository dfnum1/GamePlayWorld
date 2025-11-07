//auto generator
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
	public partial class VariableFactory
	{
		public static System.Action<Variable> OnNewVariable;
		private int m_nPoolMax = 32;
		VariablePools m_Pool = new VariablePools();
		private Dictionary<int, Variable> m_vGlobalVariables = null;
		public VariableFactory()
		{
			m_vGlobalVariables = new Dictionary<int, Variable>(32);
			m_Pool = new VariablePools();
			m_Pool.Init(m_nPoolMax);
		}
		~VariableFactory()
		{
			m_vGlobalVariables = null;
			m_Pool = null;
		}
		public void Clear()
		{
			m_Pool.Clear();
		}
		public T GetGlobalVariable<T>(int guid, bool bAutoNew = false) where T : Variable
		{
			Variable variable;
			if (m_vGlobalVariables.TryGetValue(guid, out variable))
				return variable as T;
			if(bAutoNew)
			{
				Variable pVar = NewVariableByType( typeof(T), guid );
				if(pVar!=null) m_vGlobalVariables.Add(guid, pVar);
				return pVar as T;
			}
			return null;
		}
		public void Recycel(Variable pVar)
		{
			if (pVar == null) return;
			pVar.Destroy();
			if(typeof(Framework.Plugin.AT.VariableBool) == pVar.GetType())
			{
				if(m_Pool.varVariableBool.Count < m_nPoolMax)
				m_Pool.varVariableBool.Add(pVar as Framework.Plugin.AT.VariableBool);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableBoolList) == pVar.GetType())
			{
				if(m_Pool.varVariableBoolList.Count < m_nPoolMax)
				m_Pool.varVariableBoolList.Add(pVar as Framework.Plugin.AT.VariableBoolList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableByte) == pVar.GetType())
			{
				if(m_Pool.varVariableByte.Count < m_nPoolMax)
				m_Pool.varVariableByte.Add(pVar as Framework.Plugin.AT.VariableByte);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableByteList) == pVar.GetType())
			{
				if(m_Pool.varVariableByteList.Count < m_nPoolMax)
				m_Pool.varVariableByteList.Add(pVar as Framework.Plugin.AT.VariableByteList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableColor) == pVar.GetType())
			{
				if(m_Pool.varVariableColor.Count < m_nPoolMax)
				m_Pool.varVariableColor.Add(pVar as Framework.Plugin.AT.VariableColor);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableColorList) == pVar.GetType())
			{
				if(m_Pool.varVariableColorList.Count < m_nPoolMax)
				m_Pool.varVariableColorList.Add(pVar as Framework.Plugin.AT.VariableColorList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableCurve) == pVar.GetType())
			{
				if(m_Pool.varVariableCurve.Count < m_nPoolMax)
				m_Pool.varVariableCurve.Add(pVar as Framework.Plugin.AT.VariableCurve);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableCurveList) == pVar.GetType())
			{
				if(m_Pool.varVariableCurveList.Count < m_nPoolMax)
				m_Pool.varVariableCurveList.Add(pVar as Framework.Plugin.AT.VariableCurveList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableDelegate) == pVar.GetType())
			{
				if(m_Pool.varVariableDelegate.Count < m_nPoolMax)
				m_Pool.varVariableDelegate.Add(pVar as Framework.Plugin.AT.VariableDelegate);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableFloat) == pVar.GetType())
			{
				if(m_Pool.varVariableFloat.Count < m_nPoolMax)
				m_Pool.varVariableFloat.Add(pVar as Framework.Plugin.AT.VariableFloat);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableFloatList) == pVar.GetType())
			{
				if(m_Pool.varVariableFloatList.Count < m_nPoolMax)
				m_Pool.varVariableFloatList.Add(pVar as Framework.Plugin.AT.VariableFloatList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableInt) == pVar.GetType())
			{
				if(m_Pool.varVariableInt.Count < m_nPoolMax)
				m_Pool.varVariableInt.Add(pVar as Framework.Plugin.AT.VariableInt);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableIntList) == pVar.GetType())
			{
				if(m_Pool.varVariableIntList.Count < m_nPoolMax)
				m_Pool.varVariableIntList.Add(pVar as Framework.Plugin.AT.VariableIntList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableLong) == pVar.GetType())
			{
				if(m_Pool.varVariableLong.Count < m_nPoolMax)
				m_Pool.varVariableLong.Add(pVar as Framework.Plugin.AT.VariableLong);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableLongList) == pVar.GetType())
			{
				if(m_Pool.varVariableLongList.Count < m_nPoolMax)
				m_Pool.varVariableLongList.Add(pVar as Framework.Plugin.AT.VariableLongList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableMonoScript) == pVar.GetType())
			{
				if(m_Pool.varVariableMonoScript.Count < m_nPoolMax)
				m_Pool.varVariableMonoScript.Add(pVar as Framework.Plugin.AT.VariableMonoScript);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableMonoScriptList) == pVar.GetType())
			{
				if(m_Pool.varVariableMonoScriptList.Count < m_nPoolMax)
				m_Pool.varVariableMonoScriptList.Add(pVar as Framework.Plugin.AT.VariableMonoScriptList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableObject) == pVar.GetType())
			{
				if(m_Pool.varVariableObject.Count < m_nPoolMax)
				m_Pool.varVariableObject.Add(pVar as Framework.Plugin.AT.VariableObject);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableObjectList) == pVar.GetType())
			{
				if(m_Pool.varVariableObjectList.Count < m_nPoolMax)
				m_Pool.varVariableObjectList.Add(pVar as Framework.Plugin.AT.VariableObjectList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableQuaternion) == pVar.GetType())
			{
				if(m_Pool.varVariableQuaternion.Count < m_nPoolMax)
				m_Pool.varVariableQuaternion.Add(pVar as Framework.Plugin.AT.VariableQuaternion);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableQuaternionList) == pVar.GetType())
			{
				if(m_Pool.varVariableQuaternionList.Count < m_nPoolMax)
				m_Pool.varVariableQuaternionList.Add(pVar as Framework.Plugin.AT.VariableQuaternionList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableString) == pVar.GetType())
			{
				if(m_Pool.varVariableString.Count < m_nPoolMax)
				m_Pool.varVariableString.Add(pVar as Framework.Plugin.AT.VariableString);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableStringList) == pVar.GetType())
			{
				if(m_Pool.varVariableStringList.Count < m_nPoolMax)
				m_Pool.varVariableStringList.Add(pVar as Framework.Plugin.AT.VariableStringList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableUser) == pVar.GetType())
			{
				if(m_Pool.varVariableUser.Count < m_nPoolMax)
				m_Pool.varVariableUser.Add(pVar as Framework.Plugin.AT.VariableUser);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableUserList) == pVar.GetType())
			{
				if(m_Pool.varVariableUserList.Count < m_nPoolMax)
				m_Pool.varVariableUserList.Add(pVar as Framework.Plugin.AT.VariableUserList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector2) == pVar.GetType())
			{
				if(m_Pool.varVariableVector2.Count < m_nPoolMax)
				m_Pool.varVariableVector2.Add(pVar as Framework.Plugin.AT.VariableVector2);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector2List) == pVar.GetType())
			{
				if(m_Pool.varVariableVector2List.Count < m_nPoolMax)
				m_Pool.varVariableVector2List.Add(pVar as Framework.Plugin.AT.VariableVector2List);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector2Int) == pVar.GetType())
			{
				if(m_Pool.varVariableVector2Int.Count < m_nPoolMax)
				m_Pool.varVariableVector2Int.Add(pVar as Framework.Plugin.AT.VariableVector2Int);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector2IntList) == pVar.GetType())
			{
				if(m_Pool.varVariableVector2IntList.Count < m_nPoolMax)
				m_Pool.varVariableVector2IntList.Add(pVar as Framework.Plugin.AT.VariableVector2IntList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector3) == pVar.GetType())
			{
				if(m_Pool.varVariableVector3.Count < m_nPoolMax)
				m_Pool.varVariableVector3.Add(pVar as Framework.Plugin.AT.VariableVector3);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector3List) == pVar.GetType())
			{
				if(m_Pool.varVariableVector3List.Count < m_nPoolMax)
				m_Pool.varVariableVector3List.Add(pVar as Framework.Plugin.AT.VariableVector3List);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector3Int) == pVar.GetType())
			{
				if(m_Pool.varVariableVector3Int.Count < m_nPoolMax)
				m_Pool.varVariableVector3Int.Add(pVar as Framework.Plugin.AT.VariableVector3Int);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector3IntList) == pVar.GetType())
			{
				if(m_Pool.varVariableVector3IntList.Count < m_nPoolMax)
				m_Pool.varVariableVector3IntList.Add(pVar as Framework.Plugin.AT.VariableVector3IntList);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector4) == pVar.GetType())
			{
				if(m_Pool.varVariableVector4.Count < m_nPoolMax)
				m_Pool.varVariableVector4.Add(pVar as Framework.Plugin.AT.VariableVector4);
				return;
			}
			if(typeof(Framework.Plugin.AT.VariableVector4List) == pVar.GetType())
			{
				if(m_Pool.varVariableVector4List.Count < m_nPoolMax)
				m_Pool.varVariableVector4List.Add(pVar as Framework.Plugin.AT.VariableVector4List);
				return;
			}
		}
		public T NewVariable<T>(string strName, int nGUID = 0) where T : Variable, new()
		{
			T newT = NewVariableByType(typeof(T), nGUID) as T;
			#if UNITY_EDITOR
			if(newT!=null) newT.strName = strName;
			#endif
			return newT;
		}
		public T NewVariable<T>(int nGUID = 0) where T : Variable, new()
		{
			return NewVariableByType(typeof(T), nGUID) as T;
		}
		public Variable NewVariableByType(System.Type type, int nGUID = 0, bool bEditor = false)
		{
			Variable newT = null;
			if(typeof(Framework.Plugin.AT.VariableBool) == type)
			{
				if(!bEditor && m_Pool.varVariableBool.Count>0)
				{
					newT = m_Pool.varVariableBool[0];
					newT.Destroy();
					m_Pool.varVariableBool.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableBool();
			}
			else if(typeof(Framework.Plugin.AT.VariableBoolList) == type)
			{
				if(!bEditor && m_Pool.varVariableBoolList.Count>0)
				{
					newT = m_Pool.varVariableBoolList[0];
					newT.Destroy();
					m_Pool.varVariableBoolList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableBoolList();
			}
			else if(typeof(Framework.Plugin.AT.VariableByte) == type)
			{
				if(!bEditor && m_Pool.varVariableByte.Count>0)
				{
					newT = m_Pool.varVariableByte[0];
					newT.Destroy();
					m_Pool.varVariableByte.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableByte();
			}
			else if(typeof(Framework.Plugin.AT.VariableByteList) == type)
			{
				if(!bEditor && m_Pool.varVariableByteList.Count>0)
				{
					newT = m_Pool.varVariableByteList[0];
					newT.Destroy();
					m_Pool.varVariableByteList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableByteList();
			}
			else if(typeof(Framework.Plugin.AT.VariableColor) == type)
			{
				if(!bEditor && m_Pool.varVariableColor.Count>0)
				{
					newT = m_Pool.varVariableColor[0];
					newT.Destroy();
					m_Pool.varVariableColor.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableColor();
			}
			else if(typeof(Framework.Plugin.AT.VariableColorList) == type)
			{
				if(!bEditor && m_Pool.varVariableColorList.Count>0)
				{
					newT = m_Pool.varVariableColorList[0];
					newT.Destroy();
					m_Pool.varVariableColorList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableColorList();
			}
			else if(typeof(Framework.Plugin.AT.VariableCurve) == type)
			{
				if(!bEditor && m_Pool.varVariableCurve.Count>0)
				{
					newT = m_Pool.varVariableCurve[0];
					newT.Destroy();
					m_Pool.varVariableCurve.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableCurve();
			}
			else if(typeof(Framework.Plugin.AT.VariableCurveList) == type)
			{
				if(!bEditor && m_Pool.varVariableCurveList.Count>0)
				{
					newT = m_Pool.varVariableCurveList[0];
					newT.Destroy();
					m_Pool.varVariableCurveList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableCurveList();
			}
			else if(typeof(Framework.Plugin.AT.VariableDelegate) == type)
			{
				if(!bEditor && m_Pool.varVariableDelegate.Count>0)
				{
					newT = m_Pool.varVariableDelegate[0];
					newT.Destroy();
					m_Pool.varVariableDelegate.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableDelegate();
			}
			else if(typeof(Framework.Plugin.AT.VariableFloat) == type)
			{
				if(!bEditor && m_Pool.varVariableFloat.Count>0)
				{
					newT = m_Pool.varVariableFloat[0];
					newT.Destroy();
					m_Pool.varVariableFloat.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableFloat();
			}
			else if(typeof(Framework.Plugin.AT.VariableFloatList) == type)
			{
				if(!bEditor && m_Pool.varVariableFloatList.Count>0)
				{
					newT = m_Pool.varVariableFloatList[0];
					newT.Destroy();
					m_Pool.varVariableFloatList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableFloatList();
			}
			else if(typeof(Framework.Plugin.AT.VariableInt) == type)
			{
				if(!bEditor && m_Pool.varVariableInt.Count>0)
				{
					newT = m_Pool.varVariableInt[0];
					newT.Destroy();
					m_Pool.varVariableInt.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableInt();
			}
			else if(typeof(Framework.Plugin.AT.VariableIntList) == type)
			{
				if(!bEditor && m_Pool.varVariableIntList.Count>0)
				{
					newT = m_Pool.varVariableIntList[0];
					newT.Destroy();
					m_Pool.varVariableIntList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableIntList();
			}
			else if(typeof(Framework.Plugin.AT.VariableLong) == type)
			{
				if(!bEditor && m_Pool.varVariableLong.Count>0)
				{
					newT = m_Pool.varVariableLong[0];
					newT.Destroy();
					m_Pool.varVariableLong.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableLong();
			}
			else if(typeof(Framework.Plugin.AT.VariableLongList) == type)
			{
				if(!bEditor && m_Pool.varVariableLongList.Count>0)
				{
					newT = m_Pool.varVariableLongList[0];
					newT.Destroy();
					m_Pool.varVariableLongList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableLongList();
			}
			else if(typeof(Framework.Plugin.AT.VariableMonoScript) == type)
			{
				if(!bEditor && m_Pool.varVariableMonoScript.Count>0)
				{
					newT = m_Pool.varVariableMonoScript[0];
					newT.Destroy();
					m_Pool.varVariableMonoScript.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableMonoScript();
			}
			else if(typeof(Framework.Plugin.AT.VariableMonoScriptList) == type)
			{
				if(!bEditor && m_Pool.varVariableMonoScriptList.Count>0)
				{
					newT = m_Pool.varVariableMonoScriptList[0];
					newT.Destroy();
					m_Pool.varVariableMonoScriptList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableMonoScriptList();
			}
			else if(typeof(Framework.Plugin.AT.VariableObject) == type)
			{
				if(!bEditor && m_Pool.varVariableObject.Count>0)
				{
					newT = m_Pool.varVariableObject[0];
					newT.Destroy();
					m_Pool.varVariableObject.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableObject();
			}
			else if(typeof(Framework.Plugin.AT.VariableObjectList) == type)
			{
				if(!bEditor && m_Pool.varVariableObjectList.Count>0)
				{
					newT = m_Pool.varVariableObjectList[0];
					newT.Destroy();
					m_Pool.varVariableObjectList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableObjectList();
			}
			else if(typeof(Framework.Plugin.AT.VariableQuaternion) == type)
			{
				if(!bEditor && m_Pool.varVariableQuaternion.Count>0)
				{
					newT = m_Pool.varVariableQuaternion[0];
					newT.Destroy();
					m_Pool.varVariableQuaternion.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableQuaternion();
			}
			else if(typeof(Framework.Plugin.AT.VariableQuaternionList) == type)
			{
				if(!bEditor && m_Pool.varVariableQuaternionList.Count>0)
				{
					newT = m_Pool.varVariableQuaternionList[0];
					newT.Destroy();
					m_Pool.varVariableQuaternionList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableQuaternionList();
			}
			else if(typeof(Framework.Plugin.AT.VariableString) == type)
			{
				if(!bEditor && m_Pool.varVariableString.Count>0)
				{
					newT = m_Pool.varVariableString[0];
					newT.Destroy();
					m_Pool.varVariableString.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableString();
			}
			else if(typeof(Framework.Plugin.AT.VariableStringList) == type)
			{
				if(!bEditor && m_Pool.varVariableStringList.Count>0)
				{
					newT = m_Pool.varVariableStringList[0];
					newT.Destroy();
					m_Pool.varVariableStringList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableStringList();
			}
			else if(typeof(Framework.Plugin.AT.VariableUser) == type)
			{
				if(!bEditor && m_Pool.varVariableUser.Count>0)
				{
					newT = m_Pool.varVariableUser[0];
					newT.Destroy();
					m_Pool.varVariableUser.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableUser();
			}
			else if(typeof(Framework.Plugin.AT.VariableUserList) == type)
			{
				if(!bEditor && m_Pool.varVariableUserList.Count>0)
				{
					newT = m_Pool.varVariableUserList[0];
					newT.Destroy();
					m_Pool.varVariableUserList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableUserList();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector2) == type)
			{
				if(!bEditor && m_Pool.varVariableVector2.Count>0)
				{
					newT = m_Pool.varVariableVector2[0];
					newT.Destroy();
					m_Pool.varVariableVector2.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector2();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector2List) == type)
			{
				if(!bEditor && m_Pool.varVariableVector2List.Count>0)
				{
					newT = m_Pool.varVariableVector2List[0];
					newT.Destroy();
					m_Pool.varVariableVector2List.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector2List();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector2Int) == type)
			{
				if(!bEditor && m_Pool.varVariableVector2Int.Count>0)
				{
					newT = m_Pool.varVariableVector2Int[0];
					newT.Destroy();
					m_Pool.varVariableVector2Int.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector2Int();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector2IntList) == type)
			{
				if(!bEditor && m_Pool.varVariableVector2IntList.Count>0)
				{
					newT = m_Pool.varVariableVector2IntList[0];
					newT.Destroy();
					m_Pool.varVariableVector2IntList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector2IntList();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector3) == type)
			{
				if(!bEditor && m_Pool.varVariableVector3.Count>0)
				{
					newT = m_Pool.varVariableVector3[0];
					newT.Destroy();
					m_Pool.varVariableVector3.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector3();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector3List) == type)
			{
				if(!bEditor && m_Pool.varVariableVector3List.Count>0)
				{
					newT = m_Pool.varVariableVector3List[0];
					newT.Destroy();
					m_Pool.varVariableVector3List.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector3List();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector3Int) == type)
			{
				if(!bEditor && m_Pool.varVariableVector3Int.Count>0)
				{
					newT = m_Pool.varVariableVector3Int[0];
					newT.Destroy();
					m_Pool.varVariableVector3Int.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector3Int();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector3IntList) == type)
			{
				if(!bEditor && m_Pool.varVariableVector3IntList.Count>0)
				{
					newT = m_Pool.varVariableVector3IntList[0];
					newT.Destroy();
					m_Pool.varVariableVector3IntList.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector3IntList();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector4) == type)
			{
				if(!bEditor && m_Pool.varVariableVector4.Count>0)
				{
					newT = m_Pool.varVariableVector4[0];
					newT.Destroy();
					m_Pool.varVariableVector4.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector4();
			}
			else if(typeof(Framework.Plugin.AT.VariableVector4List) == type)
			{
				if(!bEditor && m_Pool.varVariableVector4List.Count>0)
				{
					newT = m_Pool.varVariableVector4List[0];
					newT.Destroy();
					m_Pool.varVariableVector4List.RemoveAt(0);
					if(OnNewVariable!=null) OnNewVariable(newT);
					return newT;
				}
				newT = new Framework.Plugin.AT.VariableVector4List();
			}
			if(newT == null) return null;
			if(nGUID == 0)newT.GUID = AgentTreeManager.AutoGUID();
			else newT.GUID = nGUID;
			if(OnNewVariable!=null) OnNewVariable(newT);
			return newT;
		}
	}
}
