/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableLong
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    [VariableType(EVariableType.Long)]
    [System.Serializable]
    public class VariableLong : AbsVariable<long>
    {
        public static implicit operator VariableLong(int value) { return new VariableLong { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableLong>(bindVarGuid);
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
            if (oth == null) return 1;
            if (oth is VariableByte) return (int)(mValue-((VariableByte)oth).mValue);
            if (oth is VariableInt) return (int)(mValue -((VariableByte)oth).mValue);
            if (oth is VariableLong) return (int)(mValue - ((VariableLong)oth).mValue);
            return 1;
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
            if (oth is VariableLong)
                mValue = ((VariableLong)oth).mValue;
            else if (oth is VariableInt)
                mValue = ((VariableInt)oth).mValue;
            else if (oth is VariableByte)
                mValue = ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableLong)) return;
            if(refTo != null)
            {
                if (refTo is VariableInt)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableInt)refTo).mValue = (int)(mValue + ((VariableLong)oth).mValue);
                    return;
                }
                else if (refTo is VariableLong)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableLong)refTo).mValue = (int)(mValue + ((VariableLong)oth).mValue);
                    return;
                }
            }

            if (IsFlag(EFlag.Const)) return;
            mValue += ((VariableLong)oth).mValue;
        }
        //------------------------------------------------------
        public override void SubTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableLong)) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if(refTo is VariableInt)
                {
                    ((VariableInt)refTo).mValue = (int)(mValue - ((VariableLong)oth).mValue);
                    return;
                }
                if (refTo is VariableLong)
                {
                    ((VariableLong)refTo).mValue = (mValue - ((VariableLong)oth).mValue);
                    return;
                }
            }
            if (IsFlag(EFlag.Const)) return;
            mValue -= ((VariableLong)oth).mValue;
        }
        //------------------------------------------------------
        public override void MulTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableLong)) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if(refTo is VariableInt)
                {
                    ((VariableInt)refTo).mValue = (int)(mValue * ((VariableLong)oth).mValue);
                    return;
                }
                if (refTo is VariableLong)
                {
                    ((VariableLong)refTo).mValue = (mValue * ((VariableLong)oth).mValue);
                    return;
                }
            }
            if (IsFlag(EFlag.Const)) return;
            mValue *= ((VariableLong)oth).mValue;
        }
        //------------------------------------------------------
        public override void DevTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableLong)) return;
            if ((oth as VariableLong).mValue == 0) return;

            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (refTo is VariableInt)
                {
                    ((VariableInt)refTo).mValue = (int)(mValue / ((VariableLong)oth).mValue);
                    return;
                }
                if (refTo is VariableLong)
                {
                    ((VariableLong)refTo).mValue = (mValue / ((VariableLong)oth).mValue);
                    return;
                }
            }
            if (IsFlag(EFlag.Const)) return;
            mValue /= ((VariableLong)oth).mValue;
        }
        //------------------------------------------------------
        public override void Min(Variable oth, Variable refTo = null)
        {
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableInt)
                {
                    if(refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Min((oth as VariableInt).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Min((oth as VariableInt).mValue, mValue);
                    return;
                }
                else if (oth is VariableByte)
                {
                    if (refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Min((oth as VariableByte).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Min((oth as VariableByte).mValue, mValue);
                    return;
                }
                else if (oth is VariableLong)
                {
                    if (refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Min((oth as VariableLong).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Min((oth as VariableLong).mValue, mValue);
                    return;
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableLong) mValue = (long)Mathf.Min(mValue, (oth as VariableLong).mValue);
                else if (oth is VariableInt) mValue = (long)Mathf.Min(mValue, (oth as VariableInt).mValue);
                else if (oth is VariableByte) mValue = (long)Mathf.Min(mValue, (oth as VariableByte).mValue);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableInt)
                {
                    if (refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Max((oth as VariableInt).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Max((oth as VariableInt).mValue, mValue);
                    return;
                }
                else if (oth is VariableByte)
                {
                    if (refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Max((oth as VariableByte).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Max((oth as VariableByte).mValue, mValue);
                    return;
                }
                else if (oth is VariableLong)
                {
                    if (refTo is VariableInt)
                        (refTo as VariableInt).mValue = (int)Mathf.Max((oth as VariableLong).mValue, mValue);
                    else if (refTo is VariableLong)
                        (refTo as VariableLong).mValue = (long)Mathf.Max((oth as VariableLong).mValue, mValue);
                    return;
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableLong) mValue = (long)Mathf.Max(mValue, (oth as VariableLong).mValue);
                else if (oth is VariableInt) mValue = (long)Mathf.Max(mValue, (oth as VariableInt).mValue);
                else if (oth is VariableByte) mValue = (long)Mathf.Max(mValue, (oth as VariableByte).mValue);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableInt)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (left is VariableByte) (refTo as VariableInt).mValue = (int)Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) (refTo as VariableInt).mValue = (int)Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
                else if (left is VariableLong) (refTo as VariableInt).mValue = (int)Mathf.Clamp(mValue, (left as VariableLong).mValue, (right as VariableLong).mValue);
                return;
            }
            if (refTo != null && refTo is VariableLong)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (left is VariableByte) (refTo as VariableLong).mValue = (long)Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) (refTo as VariableLong).mValue = (long)Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
                else if (left is VariableLong) (refTo as VariableLong).mValue = (long)Mathf.Clamp(mValue, (left as VariableLong).mValue, (right as VariableLong).mValue);
                return;
            }

            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableByte) mValue = (long)Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) mValue = (long)Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
                else if (left is VariableLong) mValue = (long)Mathf.Clamp(mValue, (left as VariableLong).mValue, (right as VariableLong).mValue);
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableFloat)) return;
            if (refTo != null && refTo is VariableInt)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableInt).mValue = (int)Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
                return;
            }
            if (refTo != null && refTo is VariableLong)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableLong).mValue = (int)Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
                return;
            }
            if (refTo != null && refTo is VariableFloat)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableFloat).mValue = Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
                return;
            }
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = (int)Mathf.Lerp(mValue, (oth as VariableFloat).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            string label = !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName;
            object ret = AgentTreeUtl.DrawProperty(label, mValue, param.displayType);
            if (ret == null)
                mValue = UnityEditor.EditorGUILayout.LongField(label, mValue);
            else
            {
                mValue = System.Convert.ToInt64(ret);
            }

            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.LongList)]
    [System.Serializable]
    public class VariableLongList : ListVariable<List<long>>
    {
        public static implicit operator VariableLongList(List<long> value) { return new VariableLongList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableLongList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.Int; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableLongList pVar = pOther as VariableLongList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<long>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            for (int i = 0; i < mValue.Count; ++i)
                mValue[i] = -mValue[i];
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableLongList)
                mValue = ((VariableLongList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableLong && right is VariableLong)
            {
                if (refTo != null && refTo is VariableLongList)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableLongList temp = refTo as VariableLongList;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        temp.mValue[i] = (long)Mathf.Clamp(mValue[i], (left as VariableLong).mValue, (right as VariableLong).mValue);
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        mValue[i] = (long)Mathf.Clamp(mValue[i], (left as VariableLong).mValue, (right as VariableLong).mValue);
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableLongList)) return;
            VariableLongList oth_ = (VariableLongList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableLongList refTo_ = refTo as VariableLongList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = (long)Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = (long)Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableInt)
            {
                mValue.Add((item as VariableInt).mValue);
            }
            else if (item is VariableByte)
            {
                mValue.Add((item as VariableByte).mValue);
            }
            else if (item is VariableLong)
            {
                mValue.Add((item as VariableLong).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableLong)
            {
                return mValue.IndexOf((item as VariableLong).mValue);
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
            else if (refTo is VariableLong)
            {
                (refTo as VariableLong).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableLongList)) return 1;
            VariableLongList othList = oth as VariableLongList;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (othList.mValue[i] != mValue[i])
                    return 1;
            }
            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<long>();
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
                    object ret = AgentTreeUtl.DrawProperty("[" + i + "]", mValue[i], param.displayType);
                    if (ret == null)
                        mValue[i] = UnityEditor.EditorGUILayout.LongField("[" + i + "]", mValue[i]);
                    else
                        mValue[i] = System.Convert.ToInt64(ret);

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
