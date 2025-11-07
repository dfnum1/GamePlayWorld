/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableVector4
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.Vector4)]
    [System.Serializable]
    public class VariableVector4 : AbsVariable<Vector4>
    {
        public static implicit operator VariableVector4(Vector4 value) { return new VariableVector4 { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableVector4>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableVector4)) return 1;
            VariableVector4 othList = (VariableVector4)oth;

            if (Mathf.Abs(mValue.x - othList.mValue.x) > 0.01f ||
                Mathf.Abs(mValue.y - othList.mValue.y) > 0.01f ||
                Mathf.Abs(mValue.z - othList.mValue.z) > 0.01f ||
                Mathf.Abs(mValue.w - othList.mValue.w) > 0.01f) return 1;
            return 0;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableVector4)
                mValue = ((VariableVector4)oth).mValue;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue = -mValue;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (oth is VariableVector4)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x + ((VariableVector4)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y + ((VariableVector4)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z + ((VariableVector4)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w + ((VariableVector4)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x + ((VariableVector4)oth).mValue.x);
                mValue.y = (mValue.y + ((VariableVector4)oth).mValue.y);
                mValue.z = (mValue.z + ((VariableVector4)oth).mValue.z);
                mValue.w = (mValue.w + ((VariableVector4)oth).mValue.w);
                return;
            }
            else if (oth is VariableQuaternion)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x + ((VariableQuaternion)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y + ((VariableQuaternion)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z + ((VariableQuaternion)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w + ((VariableQuaternion)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x + ((VariableQuaternion)oth).mValue.x);
                mValue.y = (mValue.y + ((VariableQuaternion)oth).mValue.y);
                mValue.z = (mValue.z + ((VariableQuaternion)oth).mValue.z);
                mValue.w = (mValue.w + ((VariableQuaternion)oth).mValue.w);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableByte).VarAdd((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarAdd(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableInt).VarAdd((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarAdd(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableFloat).VarAdd((VariableVector4)refTo);
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
            if (oth is VariableQuaternion)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x - ((VariableQuaternion)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y - ((VariableQuaternion)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z - ((VariableQuaternion)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w - ((VariableQuaternion)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x - ((VariableQuaternion)oth).mValue.x);
                mValue.y = (mValue.y - ((VariableQuaternion)oth).mValue.y);
                mValue.z = (mValue.z - ((VariableQuaternion)oth).mValue.z);
                mValue.w = (mValue.w - ((VariableQuaternion)oth).mValue.w);
                return;
            }
            else if (oth is VariableVector4)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x - ((VariableVector4)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y - ((VariableVector4)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z - ((VariableVector4)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w - ((VariableVector4)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x - ((VariableVector4)oth).mValue.x);
                mValue.y = (mValue.y - ((VariableVector4)oth).mValue.y);
                mValue.z = (mValue.z - ((VariableVector4)oth).mValue.z);
                mValue.w = (mValue.w - ((VariableVector4)oth).mValue.w);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableByte).VarSub((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarSub(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableInt).VarSub((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarSub(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableFloat).VarSub((VariableVector4)refTo);
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
            if (oth is VariableQuaternion)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x * ((VariableQuaternion)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y * ((VariableQuaternion)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z * ((VariableQuaternion)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w * ((VariableQuaternion)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x * ((VariableQuaternion)oth).mValue.x);
                mValue.y = (mValue.y * ((VariableQuaternion)oth).mValue.y);
                mValue.z = (mValue.z * ((VariableQuaternion)oth).mValue.z);
                mValue.w = (mValue.w * ((VariableQuaternion)oth).mValue.w);
                return;
            }
            else if (oth is VariableVector4)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue.x = (mValue.x * ((VariableVector4)oth).mValue.x);
                    ((VariableVector4)refTo).mValue.y = (mValue.y * ((VariableVector4)oth).mValue.y);
                    ((VariableVector4)refTo).mValue.z = (mValue.z * ((VariableVector4)oth).mValue.z);
                    ((VariableVector4)refTo).mValue.w = (mValue.w * ((VariableVector4)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x * ((VariableVector4)oth).mValue.x);
                mValue.y = (mValue.y * ((VariableVector4)oth).mValue.y);
                mValue.z = (mValue.z * ((VariableVector4)oth).mValue.z);
                mValue.w = (mValue.w * ((VariableVector4)oth).mValue.w);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableByte).VarMul((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarMul(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableInt).VarMul((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarMul(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableFloat).VarMul((VariableVector4)refTo);
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
            if (oth is VariableVector4)
            {
                VariableVector4 oth_ = oth as VariableVector4;
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableVector4 refTo_ = refTo as VariableVector4;
                    if (oth_.mValue.x != 0) refTo_.mValue.x = mValue.x / oth_.mValue.x;
                    if (oth_.mValue.y != 0) refTo_.mValue.y = mValue.y / oth_.mValue.y;
                    if (oth_.mValue.z != 0) refTo_.mValue.z = mValue.z / oth_.mValue.z;
                    if (oth_.mValue.w != 0) refTo_.mValue.w = mValue.w / oth_.mValue.w;

                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                if (oth_.mValue.x != 0) mValue.x = mValue.x / oth_.mValue.x;
                if (oth_.mValue.y != 0) mValue.y = mValue.y / oth_.mValue.y;
                if (oth_.mValue.z != 0) mValue.z = mValue.z / oth_.mValue.z;
                if (oth_.mValue.w != 0) mValue.w = mValue.w / oth_.mValue.w;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableByte).VarDev((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarDev(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableInt).VarDev((VariableVector4)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarDev(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector4)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector4)refTo).mValue = mValue;
                    (oth as VariableFloat).VarDev((VariableVector4)refTo);
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
            if (!(oth is VariableVector4)) return;
            VariableVector4 oth_ = oth as VariableVector4;

            if (refTo != null && refTo is VariableVector4)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector4 refTo_ = refTo as VariableVector4;
                refTo_.mValue.x = Mathf.Min(oth_.mValue.x, mValue.x);
                refTo_.mValue.y = Mathf.Min(oth_.mValue.y, mValue.y);
                refTo_.mValue.z = Mathf.Min(oth_.mValue.z, mValue.z);
                refTo_.mValue.w = Mathf.Min(oth_.mValue.w, mValue.w);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.x = Mathf.Min(oth_.mValue.x, mValue.x);
                mValue.y = Mathf.Min(oth_.mValue.y, mValue.y);
                mValue.z = Mathf.Min(oth_.mValue.z, mValue.z);
                mValue.w = Mathf.Min(oth_.mValue.w, mValue.w);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableVector4)) return;
            VariableVector4 oth_ = oth as VariableVector4;

            if (refTo != null && refTo is VariableVector4)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector4 refTo_ = refTo as VariableVector4;
                refTo_.mValue.x = Mathf.Max(oth_.mValue.x, mValue.x);
                refTo_.mValue.y = Mathf.Max(oth_.mValue.y, mValue.y);
                refTo_.mValue.z = Mathf.Max(oth_.mValue.z, mValue.z);
                refTo_.mValue.w = Mathf.Max(oth_.mValue.w, mValue.w);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.x = Mathf.Max(oth_.mValue.x, mValue.x);
                mValue.y = Mathf.Max(oth_.mValue.y, mValue.y);
                mValue.z = Mathf.Max(oth_.mValue.z, mValue.z);
                mValue.w = Mathf.Max(oth_.mValue.w, mValue.w);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableVector4)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector4 refTo_ = refTo as VariableVector4;
                if (left is VariableVector4)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableVector4).mValue.x, (right as VariableVector4).mValue.x);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableVector4).mValue.y, (right as VariableVector4).mValue.y);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableVector4).mValue.z, (right as VariableVector4).mValue.z);
                    refTo_.mValue.w = Mathf.Clamp(mValue.w, (left as VariableVector4).mValue.w, (right as VariableVector4).mValue.w);
                }
                else if (left is VariableByte)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.w = Mathf.Clamp(mValue.w, (left as VariableByte).mValue, (right as VariableByte).mValue);
                }
                else if (left is VariableInt)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.w = Mathf.Clamp(mValue.w, (left as VariableInt).mValue, (right as VariableInt).mValue);
                }
                else if (left is VariableFloat)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.w = Mathf.Clamp(mValue.w, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableVector4)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableVector4).mValue.x, (right as VariableVector4).mValue.x);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableVector4).mValue.y, (right as VariableVector4).mValue.y);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableVector4).mValue.z, (right as VariableVector4).mValue.z);
                    mValue.w = Mathf.Clamp(mValue.w, (left as VariableVector4).mValue.w, (right as VariableVector4).mValue.w);
                }
                else if (left is VariableByte)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    mValue.w = Mathf.Clamp(mValue.w, (left as VariableByte).mValue, (right as VariableByte).mValue);
                }
                else if (left is VariableInt)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    mValue.w = Mathf.Clamp(mValue.w, (left as VariableInt).mValue, (right as VariableInt).mValue);
                }
                else if (left is VariableFloat)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    mValue.w = Mathf.Clamp(mValue.w, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableVector4)) return;
            if (refTo != null && refTo is VariableVector4)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableVector4).mValue = Vector4.Lerp(mValue, (oth as VariableVector4).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Vector4.Lerp(mValue, (oth as VariableVector4).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            mValue = UnityEditor.EditorGUILayout.Vector4Field(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName, mValue);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.Vector4List)]
    [System.Serializable]
    public class VariableVector4List : ListVariable<List<Vector4>>
    {
        public static implicit operator VariableVector4List(List<Vector4> value) { return new VariableVector4List { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableVector4List>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableVector4List pVar = pOther as VariableVector4List;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<Vector4>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            for (int i = 0; i < mValue.Count; ++i)
            {
                mValue[i] = -mValue[i];
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableVector4List)
                mValue = ((VariableVector4List)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableVector4List && right is VariableVector4List)
            {
                if (refTo != null && refTo is VariableVector4List)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableVector4List temp = refTo as VariableVector4List;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Vector4 rot = temp.mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableVector4List).mValue[i].x, (right as VariableVector4List).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableVector4List).mValue[i].y, (right as VariableVector4List).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableVector4List).mValue[i].z, (right as VariableVector4List).mValue[i].z);
                        rot.w = Mathf.Clamp(mValue[i].w, (left as VariableVector4List).mValue[i].w, (right as VariableVector4List).mValue[i].w);
                        temp.mValue[i] = rot;
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Vector4 rot = mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableVector4List).mValue[i].x, (right as VariableVector4List).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableVector4List).mValue[i].y, (right as VariableVector4List).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableVector4List).mValue[i].z, (right as VariableVector4List).mValue[i].z);
                        rot.w = Mathf.Clamp(mValue[i].w, (left as VariableVector4List).mValue[i].w, (right as VariableVector4List).mValue[i].w);
                        mValue[i] = rot;
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableVector4List)) return;
            VariableVector4List oth_ = (VariableVector4List)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector4List refTo_ = refTo as VariableVector4List;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Vector4.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Vector4.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public void Slerp(Variable oth, float fValue, Variable refTo = null)
        {

        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableVector4)
            {
                mValue.Add((item as VariableVector4).mValue);
            }
            else if (item is VariableQuaternion)
            {
                mValue.Add(new Vector4((item as VariableQuaternion).mValue.x, (item as VariableQuaternion).mValue.y, (item as VariableQuaternion).mValue.z, (item as VariableQuaternion).mValue.w));
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableVector4)
            {
                return mValue.IndexOf((item as VariableVector4).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableVector4)
            {
                (refTo as VariableVector4).mValue = mValue[index];
            }
            else if (refTo is VariableQuaternion)
            {
                (refTo as VariableQuaternion).mValue = new Quaternion(mValue[index].x, mValue[index].y, mValue[index].z, mValue[index].w);
            }
        }
        public override EVariableType GetListElementType() { return EVariableType.Vector4; }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableVector4List)) return 1;
            VariableVector4List othList = (VariableVector4List)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (Mathf.Abs(mValue[i].x - othList.mValue[i].x) > 0.01f ||
                    Mathf.Abs(mValue[i].y - othList.mValue[i].y) > 0.01f ||
                    Mathf.Abs(mValue[i].z - othList.mValue[i].z) > 0.01f ||
                    Mathf.Abs(mValue[i].w - othList.mValue[i].w) > 0.01f) return 1;
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<Vector4>();

            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(Vector4.zero);
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
                    mValue[i] = UnityEditor.EditorGUILayout.Vector4Field("[" + i + "]", mValue[i]);
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
            return rect;
        }
#endif
    }
}
