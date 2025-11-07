/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableString
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.String)]
    [System.Serializable]
    public class VariableString : AbsVariable<string>
    {
        public static implicit operator VariableString(string value) { return new VariableString { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableString>(bindVarGuid);
            }
        }  
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableString)) return 1;
            VariableString othList = (VariableString)oth;
            if (othList.mValue == null || mValue == null) return 1;
            return mValue.CompareTo(othList.mValue);
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableString)
                mValue = ((VariableString)oth).mValue;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableString)) return;
            if (refTo != null && refTo is VariableString)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableString)refTo).mValue = AgentTreeUtl.stringBuilder.Append(mValue).Append((((VariableString)oth).mValue)).ToString();
                return;
            }
            if (IsFlag(EFlag.Const)) return;
            mValue += ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override bool isNull() { return string.IsNullOrEmpty(mValue); }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            string label = !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName;
            if (mValue == null && AgentTreeUtl.IsUnityObject(param.displayType))
                mValue = "";
            object ret = AgentTreeUtl.DrawProperty(label, mValue, param.displayType);
            if (ret == null)
                mValue = UnityEditor.EditorGUILayout.TextField(label, mValue);
            else mValue = (string)ret;
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.StringList)]
    [System.Serializable]
    public class VariableStringList : ListVariable<List<string>>
    {
        public static implicit operator VariableStringList(List<string> value) { return new VariableStringList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableStringList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.String; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableStringList pVar = pOther as VariableStringList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<string>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableStringList)
                mValue = ((VariableStringList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {

        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {

        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableString)
            {
                mValue.Add((item as VariableString).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableString)
            {
                return mValue.IndexOf((item as VariableString).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableString)
            {
                (refTo as VariableString).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableStringList)) return 1;
            VariableStringList othList = (VariableStringList)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (mValue[i] == null || othList.mValue[i] == null || mValue[i].CompareTo(othList.mValue[i])!=0) return 1;
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<string>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add("");
            }
            AgentTreeUtl.EndHorizontal();
            bool bUnityObj = AgentTreeUtl.IsUnityObject(param.displayType);
            Rect rc = GUILayoutUtility.GetLastRect();
            if (IsFlag(EFlag.Expanded))
            {
                float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                UnityEditor.EditorGUIUtility.labelWidth = 50;

                for (int i = 0; i < mValue.Count; ++i)
                {
                    AgentTreeUtl.BeginHorizontal();
                    if (mValue[i] == null && bUnityObj)
                        mValue[i] = "";
                    object ret = AgentTreeUtl.DrawProperty("[" + i + "]", mValue[i], param.displayType);
                    if (ret == null)
                        mValue[i] = UnityEditor.EditorGUILayout.TextField("[" + i + "]", mValue[i]);
                    else mValue[i] = (string)ret;

                    if (GUILayout.Button("删除"))
                    {
                        if (UnityEditor.EditorUtility.DisplayDialog("提示", "是否确定删除", "确定", "取消"))
                        {
                            mValue.RemoveAt(i);
                            break;
                        }
                    }
                    AgentTreeUtl.EndHorizontal();
                }
                UnityEditor.EditorGUIUtility.labelWidth = labelWidth;

            }
            return rc;
        }
#endif
    }
}
