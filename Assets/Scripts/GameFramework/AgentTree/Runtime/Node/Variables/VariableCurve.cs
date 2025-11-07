/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableCurve
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.Curve,"曲线")]
    [System.Serializable]
    public class VariableCurve : AbsVariable<AnimationCurve>
    {
        public static implicit operator VariableCurve(AnimationCurve value) { return new VariableCurve { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableCurve>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableCurve)
                mValue = ((VariableCurve)oth).mValue;
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableCurve)) return;
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableFloat).mValue = mValue.Evaluate(fValue);
            }
        }

        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || mValue == null || oth.GetType() != typeof(VariableCurve)) return 1;
            VariableCurve othList = (VariableCurve)oth;
            if (othList.mValue == null) return 1;
            for(int i = 0; i < mValue.length; ++i)
            {
                if (mValue[i].inTangent != othList.mValue[i].inTangent ||
                    mValue[i].inWeight != othList.mValue[i].inWeight ||
                    mValue[i].outTangent != othList.mValue[i].outTangent ||
                    mValue[i].outWeight != othList.mValue[i].outWeight ||
                    mValue[i].time != othList.mValue[i].time ||
                    mValue[i].value != othList.mValue[i].value ||
                    mValue[i].weightedMode != othList.mValue[i].weightedMode)
                {
                    return 1;
                }
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new AnimationCurve();
            GUILayout.BeginHorizontal();
            mValue = UnityEditor.EditorGUILayout.CurveField(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName, mValue);
            if(AgentTreeUtl.pCopyCurve == null)
            {
                if (GUILayout.Button(".", new GUILayoutOption[] { GUILayout.Width(10) }))
                {
                    AgentTreeUtl.pCopyCurve = mValue;
                }
            }
            else if (mValue != AgentTreeUtl.pCopyCurve && AgentTreeUtl.pCopyCurve != null && GUILayout.Button("P", new GUILayoutOption[] { GUILayout.Width(10) }))
            {
                mValue = AgentTreeUtl.FormCopy();
            }
            GUILayout.EndHorizontal();
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.CurveList,"曲线组")]
    [System.Serializable]
    public class VariableCurveList : ListVariable<List<AnimationCurve>>
    {
        public static implicit operator VariableCurveList(List<AnimationCurve> value) { return new VariableCurveList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableCurveList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.Curve; }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableCurveList)
                mValue = ((VariableCurveList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (refTo == null || !(refTo is VariableFloatList)) return;
            if (refTo.IsFlag(EFlag.Const)) return;
            VariableFloatList refTo_ = refTo as VariableFloatList;
            if (refTo_.mValue.Count != mValue.Count) return;
            for (int i = 0; i < mValue.Count; ++i)
            {
                refTo_.mValue[i] = mValue[i].Evaluate(fValue);
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableCurve)
            {
                mValue.Add((item as VariableCurve).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableCurve)
            {
                return mValue.IndexOf((item as VariableCurve).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableCurve)
            {
                (refTo as VariableCurve).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableCurveList pVar = pOther as VariableCurveList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<AnimationCurve>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableCurveList)) return 1;
            VariableCurveList othList = (VariableCurveList)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (mValue[i].length != othList.mValue[i].length) return 1;
                for(int j = 0; j < mValue[i].length; ++j)
                {
                    if(mValue[i][j].inTangent != othList.mValue[i][j].inTangent ||
                        mValue[i][j].inWeight != othList.mValue[i][j].inWeight ||
                        mValue[i][j].outTangent != othList.mValue[i][j].outTangent ||
                        mValue[i][j].outWeight != othList.mValue[i][j].outWeight ||
                        mValue[i][j].time != othList.mValue[i][j].time ||
                        mValue[i][j].value != othList.mValue[i][j].value ||
                        mValue[i][j].weightedMode != othList.mValue[i][j].weightedMode)
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<AnimationCurve>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(new AnimationCurve());
            }
            AgentTreeUtl.EndHorizontal();
            Rect rc = GUILayoutUtility.GetLastRect();
            if (IsFlag(EFlag.Expanded))
            {
                float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                UnityEditor.EditorGUIUtility.labelWidth = 50;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    AgentTreeUtl.BeginHorizontal();
                    if (mValue[i] == null) mValue[i] = new AnimationCurve();
                    mValue[i] = UnityEditor.EditorGUILayout.CurveField("[" + i + "]", mValue[i]);
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
