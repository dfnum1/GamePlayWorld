/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableFloat
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.Float)]
    [System.Serializable]
    public class VariableFloat : AbsVariable<float>
    {
        public static implicit operator VariableFloat(float value) { return new VariableFloat { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableFloat>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        #region VAR_OP
        //------------------------------------------------------
        public void VarAdd(VariableVector2 refP)
        {
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector2 refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector2 refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector2 refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableVector2Int refP)
        {
            refP.mValue.x += (int)mValue;
            refP.mValue.y += (int)mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector2Int refP)
        {
            refP.mValue.x -= (int)mValue;
            refP.mValue.y -= (int)mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector2Int refP)
        {
            refP.mValue.x *= (int)mValue;
            refP.mValue.y *= (int)mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector2Int refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= (int)mValue;
            refP.mValue.y /= (int)mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableVector3Int refP)
        {
            refP.mValue.x += (int)mValue;
            refP.mValue.y += (int)mValue;
            refP.mValue.z += (int)mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector3Int refP)
        {
            refP.mValue.x -= (int)mValue;
            refP.mValue.y -= (int)mValue;
            refP.mValue.z -= (int)mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector3Int refP)
        {
            refP.mValue.x *= (int)mValue;
            refP.mValue.y *= (int)mValue;
            refP.mValue.z *= (int)mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector3Int refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= (int)mValue;
            refP.mValue.y /= (int)mValue;
            refP.mValue.z /= (int)mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableVector3 refP)
        {
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
            refP.mValue.z += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector3 refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
            refP.mValue.z -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector3 refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
            refP.mValue.z *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector3 refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
            refP.mValue.z /= mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableVector4 refP)
        {
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
            refP.mValue.z += mValue;
            refP.mValue.w += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector4 refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
            refP.mValue.z -= mValue;
            refP.mValue.w -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector4 refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
            refP.mValue.z *= mValue;
            refP.mValue.w *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector4 refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
            refP.mValue.z /= mValue;
            refP.mValue.w /= mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableQuaternion refP)
        {
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
            refP.mValue.z += mValue;
            refP.mValue.w += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableQuaternion refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
            refP.mValue.z -= mValue;
            refP.mValue.w -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableQuaternion refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
            refP.mValue.z *= mValue;
            refP.mValue.w *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableQuaternion refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
            refP.mValue.z /= mValue;
            refP.mValue.w /= mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableColor refP)
        {
            refP.mValue.a += mValue;
            refP.mValue.r += mValue;
            refP.mValue.g += mValue;
            refP.mValue.b += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableColor refP)
        {
            refP.mValue.a -= mValue;
            refP.mValue.r -= mValue;
            refP.mValue.g -= mValue;
            refP.mValue.b -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableColor refP)
        {
            refP.mValue.a *= mValue;
            refP.mValue.r *= mValue;
            refP.mValue.g *= mValue;
            refP.mValue.b *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableColor refP)
        {
            if (mValue == 0) return;
            refP.mValue.a /= mValue;
            refP.mValue.r /= mValue;
            refP.mValue.g /= mValue;
            refP.mValue.b /= mValue;
        }
        #endregion
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableFloat)) return 1;
            if (Mathf.Abs(((VariableFloat)oth).mValue - mValue) <= 0.01f) return 0;
            if (mValue > ((VariableFloat)oth).mValue) return 1;
            return -1;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue = -mValue;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableFloat)
                mValue = ((VariableFloat)oth).mValue;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableFloat)) return;
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableFloat)refTo).mValue = (mValue + ((VariableFloat)oth).mValue);
                return;
            }
            if (IsFlag(EFlag.Const)) return;
            mValue += ((VariableFloat)oth).mValue;
        }
        //------------------------------------------------------
        public override void SubTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableInt)) return;
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableFloat)refTo).mValue = (mValue - ((VariableFloat)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue -= ((VariableFloat)oth).mValue;
        }
        //------------------------------------------------------
        public override void MulTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableFloat)) return;
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableFloat)refTo).mValue = (mValue * ((VariableFloat)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue *= ((VariableFloat)oth).mValue;
        }
        //------------------------------------------------------
        public override void DevTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableFloat)) return;
            if ((oth as VariableFloat).mValue == 0) return;

            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableFloat)refTo).mValue = (mValue / ((VariableFloat)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue /= ((VariableFloat)oth).mValue;
        }
        //------------------------------------------------------
        public override void Min(Variable oth, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) (refTo as VariableFloat).mValue = Mathf.Min((oth as VariableByte).mValue, mValue);
                else if (oth is VariableInt) (refTo as VariableFloat).mValue = Mathf.Min((oth as VariableInt).mValue, mValue);
                else if (oth is VariableFloat) (refTo as VariableFloat).mValue = Mathf.Min((oth as VariableFloat).mValue, mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) mValue = Mathf.Min(mValue, (oth as VariableByte).mValue);
                else if (oth is VariableInt) mValue = Mathf.Min(mValue, (oth as VariableInt).mValue);
                else if (oth is VariableFloat) mValue = Mathf.Min(mValue, (oth as VariableFloat).mValue);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) (refTo as VariableFloat).mValue = Mathf.Max((oth as VariableByte).mValue, mValue);
                else if (oth is VariableInt) (refTo as VariableFloat).mValue = Mathf.Max((oth as VariableInt).mValue, mValue);
                else if (oth is VariableFloat) (refTo as VariableFloat).mValue = Mathf.Max((oth as VariableFloat).mValue, mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) mValue = Mathf.Max(mValue, (oth as VariableByte).mValue);
                else if (oth is VariableInt) mValue = Mathf.Max(mValue, (oth as VariableInt).mValue);
                else if (oth is VariableFloat) mValue = Mathf.Max(mValue, (oth as VariableFloat).mValue);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (left is VariableByte) (refTo as VariableFloat).mValue = Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) (refTo as VariableFloat).mValue = Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
                else if (left is VariableFloat) (refTo as VariableFloat).mValue = Mathf.Clamp(mValue, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableByte) mValue = Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) mValue = Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
                else if (left is VariableFloat) mValue = Mathf.Clamp(mValue, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableFloat)) return;
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableFloat) (refTo as VariableFloat).mValue = Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            mValue = UnityEditor.EditorGUILayout.FloatField(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName, mValue);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.FloatList)]
    [System.Serializable]
    public class VariableFloatList : ListVariable<List<float>>
    {
        public static implicit operator VariableFloatList(List<float> value) { return new VariableFloatList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableFloatList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.Float; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableFloatList pVar = pOther as VariableFloatList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<float>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            for(int i =0; i < mValue.Count; ++i)
                mValue[i] = -mValue[i];
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableFloatList)
                mValue = ((VariableFloatList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableFloat && right is VariableFloat)
            {
                if (refTo != null && refTo is VariableFloatList)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableFloatList temp = refTo as VariableFloatList;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        temp.mValue[i] = Mathf.Clamp(mValue[i], (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        mValue[i] = Mathf.Clamp(mValue[i], (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableFloatList)) return;
            VariableFloatList oth_ = (VariableFloatList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableFloatList refTo_ = refTo as VariableFloatList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableByte)
            {
                mValue.Add((float)(item as VariableByte).mValue);
            }
            else if (item is VariableInt)
            {
                mValue.Add((float)(item as VariableInt).mValue);
            }
            else if (item is VariableFloat)
            {
                mValue.Add((item as VariableFloat).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableFloat)
            {
                return mValue.IndexOf((item as VariableFloat).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableByte)
            {
                (refTo as VariableByte).mValue = (byte)mValue[index];
            }
            else if (refTo is VariableInt)
            {
                (refTo as VariableInt).mValue = (int)mValue[index];
            }
            else if (refTo is VariableFloat)
            {
                (refTo as VariableFloat).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableFloatList)) return 1;
            VariableFloatList othList = oth as VariableFloatList;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for(int i =0; i < mValue.Count; ++i)
            {
                if (Mathf.Abs(othList.mValue[i] - mValue[i]) > 0.01f)
                    return 1;
            }
            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<float>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(0);
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
                    mValue[i] = UnityEditor.EditorGUILayout.FloatField("[" + i + "]", mValue[i]);
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
