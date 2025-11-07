/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableColor
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.Color, "颜色")]
    [System.Serializable]
    public class VariableColor : AbsVariable<Color>
    {
        public static implicit operator VariableColor(Color value) { return new VariableColor { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableColor>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableColor)) return 1;
            VariableColor othList = (VariableColor)oth;

            if (Mathf.Abs(mValue.a - othList.mValue.a) <=0.01f &&
                Mathf.Abs(mValue.r - othList.mValue.r) <=0.01f &&
                Mathf.Abs(mValue.g - othList.mValue.g) <= 0.01f &&
                Mathf.Abs(mValue.b - othList.mValue.b) <= 0.01f) return 0;
            return 1;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableColor)
                mValue = ((VariableColor)oth).mValue;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue.a = 1 - mValue.a;
            mValue.r = 1 - mValue.r;
            mValue.g = 1 - mValue.g;
            mValue.b = 1 - mValue.b;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (oth is VariableColor)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = (mValue + ((VariableColor)oth).mValue);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue += ((VariableColor)oth).mValue;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableByte).VarAdd((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarAdd(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableInt).VarAdd((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarAdd(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableFloat).VarAdd((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableFloat).VarAdd(this);
                return;
            }
        }
        //------------------------------------------------------
        public override void SubTo(Variable oth, Variable refTo = null)
        {
            if (oth is VariableColor)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue - ((VariableColor)oth).mValue;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue -= ((VariableColor)oth).mValue;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableByte).VarSub((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarSub(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableInt).VarSub((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarSub(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableFloat).VarSub((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableFloat).VarSub(this);
                return;
            }
        }
        //------------------------------------------------------
        public override void MulTo(Variable oth, Variable refTo = null)
        {
            if ((oth is VariableColor))
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = (mValue * ((VariableColor)oth).mValue);
                }
                if (IsFlag(EFlag.Const)) return;
                mValue *= ((VariableColor)oth).mValue;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableByte).VarMul((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarMul(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableInt).VarMul((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarMul(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableFloat).VarMul((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableFloat).VarMul(this);
                return;
            }
        }
        //------------------------------------------------------
        public override void DevTo(Variable oth, Variable refTo = null)
        {
            if(oth is VariableColor)
            {
                VariableColor oth_ = oth as VariableColor;
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableColor refTo_ = refTo as VariableColor;
                    if (oth_.mValue.a != 0) refTo_.mValue.a = mValue.a / oth_.mValue.a;
                    if (oth_.mValue.r != 0) refTo_.mValue.r = mValue.r / oth_.mValue.r;
                    if (oth_.mValue.g != 0) refTo_.mValue.g = mValue.g / oth_.mValue.g;
                    if (oth_.mValue.b != 0) refTo_.mValue.b = mValue.b / oth_.mValue.b;

                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                if (oth_.mValue.a != 0) mValue.a /= oth_.mValue.a;
                if (oth_.mValue.r != 0) mValue.r /= oth_.mValue.r;
                if (oth_.mValue.g != 0) mValue.g /= oth_.mValue.g;
                if (oth_.mValue.b != 0) mValue.b /= oth_.mValue.b;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableByte).VarDev((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarDev(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableInt).VarDev((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarDev(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableColor)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableColor)refTo).mValue = mValue;
                    (oth as VariableFloat).VarDev((VariableColor)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableFloat).VarDev(this);
                return;
            }
        }
        //------------------------------------------------------
        public override void Min(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableColor)) return;
            VariableColor oth_ = oth as VariableColor;

            if (refTo != null && refTo is VariableColor)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableColor refTo_ = refTo as VariableColor;
                refTo_.mValue.a = Mathf.Min(oth_.mValue.a, mValue.a);
                refTo_.mValue.r = Mathf.Min(oth_.mValue.r, mValue.r);
                refTo_.mValue.g = Mathf.Min(oth_.mValue.g, mValue.g);
                refTo_.mValue.b = Mathf.Min(oth_.mValue.b, mValue.b);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.a = Mathf.Min(oth_.mValue.a, mValue.a);
                mValue.r = Mathf.Min(oth_.mValue.r, mValue.r);
                mValue.g = Mathf.Min(oth_.mValue.g, mValue.g);
                mValue.b = Mathf.Min(oth_.mValue.b, mValue.b);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableColor)) return;
            VariableColor oth_ = oth as VariableColor;

            if (refTo != null && refTo is VariableColor)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableColor refTo_ = refTo as VariableColor;
                refTo_.mValue.a = Mathf.Max(oth_.mValue.a, mValue.a);
                refTo_.mValue.r = Mathf.Max(oth_.mValue.r, mValue.r);
                refTo_.mValue.g = Mathf.Max(oth_.mValue.g, mValue.g);
                refTo_.mValue.b = Mathf.Max(oth_.mValue.b, mValue.b);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.a = Mathf.Max(oth_.mValue.a, mValue.a);
                mValue.r = Mathf.Max(oth_.mValue.r, mValue.r);
                mValue.g = Mathf.Max(oth_.mValue.g, mValue.g);
                mValue.b = Mathf.Max(oth_.mValue.b, mValue.b);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableColor)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableColor refTo_ = refTo as VariableColor;
                if (left is VariableColor)
                {
                    refTo_.mValue.a = Mathf.Clamp(mValue.a, (left as VariableColor).mValue.a, (right as VariableColor).mValue.a);
                    refTo_.mValue.r = Mathf.Clamp(mValue.r, (left as VariableColor).mValue.r, (right as VariableColor).mValue.r);
                    refTo_.mValue.g = Mathf.Clamp(mValue.g, (left as VariableColor).mValue.g, (right as VariableColor).mValue.g);
                    refTo_.mValue.b = Mathf.Clamp(mValue.b, (left as VariableColor).mValue.b, (right as VariableColor).mValue.b);
                }
                else if (left is VariableByte)
                {
                    refTo_.mValue.a = Mathf.Clamp(mValue.a, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.r = Mathf.Clamp(mValue.r, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.g = Mathf.Clamp(mValue.g, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.b = Mathf.Clamp(mValue.b, (left as VariableByte).mValue, (right as VariableByte).mValue);
                }
                else if (left is VariableInt)
                {
                    refTo_.mValue.a = Mathf.Clamp(mValue.a, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.r = Mathf.Clamp(mValue.r, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.g = Mathf.Clamp(mValue.g, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.b = Mathf.Clamp(mValue.b, (left as VariableInt).mValue, (right as VariableInt).mValue);
                }
                else if (left is VariableFloat)
                {
                    refTo_.mValue.a = Mathf.Clamp(mValue.a, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.r = Mathf.Clamp(mValue.r, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.g = Mathf.Clamp(mValue.g, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.b = Mathf.Clamp(mValue.b, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableColor)
                {
                    mValue.a = Mathf.Clamp(mValue.a, (left as VariableColor).mValue.a, (right as VariableColor).mValue.a);
                    mValue.r = Mathf.Clamp(mValue.r, (left as VariableColor).mValue.r, (right as VariableColor).mValue.r);
                    mValue.g = Mathf.Clamp(mValue.g, (left as VariableColor).mValue.g, (right as VariableColor).mValue.g);
                    mValue.b = Mathf.Clamp(mValue.b, (left as VariableColor).mValue.b, (right as VariableColor).mValue.b);
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableColor)) return;
            if (refTo != null && refTo is VariableColor)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableColor).mValue = Color.Lerp(mValue, (oth as VariableColor).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Color.Lerp(mValue, (oth as VariableColor).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            mValue = UnityEditor.EditorGUILayout.ColorField(new GUIContent(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName), mValue,true, true, true);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.ColorList, "颜色组")]
    [System.Serializable]
    public class VariableColorList : ListVariable<List<Color>>
    {
        public static implicit operator VariableColorList(List<Color> value) { return new VariableColorList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableColorList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            for(int i =0; i < mValue.Count; ++i)
            {
                Color color = mValue[i];
                color.a = 1 - color.a;
                color.r = 1 - color.r;
                color.g = 1 - color.g;
                color.b = 1 - color.b;
                mValue[i] = color;
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableColorList)
                mValue = ((VariableColorList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableColorList && right is VariableColorList)
            {
                if (refTo != null && refTo is VariableColorList)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableColorList temp = refTo as VariableColorList;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Color color = temp.mValue[i];
                        color.a = Mathf.Clamp(mValue[i].a, (left as VariableColorList).mValue[i].a, (right as VariableColorList).mValue[i].a);
                        color.r = Mathf.Clamp(mValue[i].r, (left as VariableColorList).mValue[i].r, (right as VariableColorList).mValue[i].r);
                        color.g = Mathf.Clamp(mValue[i].g, (left as VariableColorList).mValue[i].g, (right as VariableColorList).mValue[i].g);
                        color.b = Mathf.Clamp(mValue[i].b, (left as VariableColorList).mValue[i].b, (right as VariableColorList).mValue[i].b);
                        temp.mValue[i] = color;
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Color color = mValue[i];
                        color.a = Mathf.Clamp(mValue[i].a, (left as VariableColorList).mValue[i].a, (right as VariableColorList).mValue[i].a);
                        color.r = Mathf.Clamp(mValue[i].r, (left as VariableColorList).mValue[i].r, (right as VariableColorList).mValue[i].r);
                        color.g = Mathf.Clamp(mValue[i].g, (left as VariableColorList).mValue[i].g, (right as VariableColorList).mValue[i].g);
                        color.b = Mathf.Clamp(mValue[i].b, (left as VariableColorList).mValue[i].b, (right as VariableColorList).mValue[i].b);
                        mValue[i] = color;
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableColorList)) return;
            VariableColorList oth_ = (VariableColorList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableColorList refTo_ = refTo as VariableColorList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Color.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Color.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableColor)
            {
                mValue.Add((item as VariableColor).mValue);
            }
            else if (item is VariableVector4)
            {
                mValue.Add(new Color((item as VariableVector4).mValue.x, (item as VariableVector4).mValue.y, (item as VariableVector4).mValue.z, (item as VariableVector4).mValue.w));
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableColor)
            {
                return mValue.IndexOf((item as VariableColor).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableColor)
            {
                (refTo as VariableColor).mValue = mValue[index];
            }
            else if (refTo is VariableVector4)
            {
                (refTo as VariableVector4).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableColorList pVar = pOther as VariableColorList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<Color>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        public override EVariableType GetListElementType() { return EVariableType.Color; }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableColorList)) return 1;
            VariableColorList othList = (VariableColorList)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (Mathf.Abs(mValue[i].a- othList.mValue[i].a) > 0.01f ||
                    Mathf.Abs(mValue[i].r - othList.mValue[i].r) > 0.01f||
                    Mathf.Abs(mValue[i].g - othList.mValue[i].g) > 0.01f||
                    Mathf.Abs(mValue[i].b - othList.mValue[i].b) > 0.01f) return 1;
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<Color>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(Color.black);
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
                    mValue[i] = UnityEditor.EditorGUILayout.ColorField(new GUIContent("[" + i + "]"), mValue[i], true, true, true);
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
