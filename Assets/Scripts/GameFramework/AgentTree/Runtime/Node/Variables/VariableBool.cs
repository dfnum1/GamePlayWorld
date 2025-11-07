/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableBool
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    [VariableType(EVariableType.Bool, "布尔")]
    [System.Serializable]
    public class VariableBool : AbsVariable<bool>
    {
        public static implicit operator VariableBool(bool value) { return new VariableBool { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid!= this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableBool>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null) return 1;
            if (oth is VariableBool) return (((VariableBool)oth).mValue == mValue)?0:1;
            if (oth is VariableByte) return ((((VariableByte)oth).mValue!=0) == mValue) ? 0 : 1;
            if (oth is VariableInt) return ((((VariableByte)oth).mValue != 0) == mValue) ? 0 : 1;
            return 1;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue = !mValue;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            mValue = ((VariableBool)oth).mValue;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            mValue = UnityEditor.EditorGUILayout.Toggle( !string.IsNullOrEmpty(strName) ?strName: param.strDefaultName, mValue);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.BoolList, "布尔组")]
    [System.Serializable]
    public class VariableBoolList : ListVariable<List<bool>>
    {
        public static implicit operator VariableBoolList(List<bool> value) { return new VariableBoolList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableBoolList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableBoolList pVar = pOther as VariableBoolList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<bool>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        public override EVariableType GetListElementType() { return EVariableType.Bool; }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            mValue = ((VariableBoolList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
           for(int i =0; i < mValue.Count;++i)
            {
                mValue[i] = !mValue[i];
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableBool)
            {
                mValue.Add((item as VariableBool).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableBool)
            {
                return mValue.IndexOf((item as VariableBool).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableBool)
            {
                (refTo as VariableBool).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableBoolList)) return 1;
            VariableBoolList othList = (VariableBoolList)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for(int i =0; i < mValue.Count; ++i)
            {
                if (mValue[i] != othList.mValue[i]) return 1;
            }

            return 0;
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<bool>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(false);
            }
            AgentTreeUtl.EndHorizontal();
            Rect rect = GUILayoutUtility.GetLastRect();
            if (IsFlag(EFlag.Expanded))
            {
                float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                UnityEditor.EditorGUIUtility.labelWidth = 50;

                for (int i = 0; i < mValue.Count; ++i)
                {
                    AgentTreeUtl.BeginHorizontal();
                    mValue[i] = UnityEditor.EditorGUILayout.Toggle("[" + i + "]", mValue[i]);
                    if (GUILayout.Button("删除"))
                    {
                        if(UnityEditor.EditorUtility.DisplayDialog("提示", "是否确定删除", "确定", "取消"))
                        {
                            mValue.RemoveAt(i);
                            break;
                        }
                    }
                    AgentTreeUtl.EndHorizontal();
                }
                UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
            }
            return rect;
        }
#endif
    }
}
