/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableQuaternion
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    [VariableType(EVariableType.Quaternion)]
    [System.Serializable]
    public class VariableQuaternion : AbsVariable<Quaternion>
    {
        public static implicit operator VariableQuaternion(Quaternion value) { return new VariableQuaternion { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableQuaternion>(bindVarGuid);
            }
        }  
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableQuaternion)) return 1;
            VariableQuaternion othList = (VariableQuaternion)oth;

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
            if (oth is VariableQuaternion)
                mValue = ((VariableQuaternion)oth).mValue;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue.eulerAngles = Vector3.one*180 + mValue.eulerAngles;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (oth is VariableQuaternion)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x + ((VariableQuaternion)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y + ((VariableQuaternion)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z + ((VariableQuaternion)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w + ((VariableQuaternion)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x + ((VariableQuaternion)oth).mValue.x);
                mValue.y = (mValue.y + ((VariableQuaternion)oth).mValue.y);
                mValue.z = (mValue.z + ((VariableQuaternion)oth).mValue.z);
                mValue.w = (mValue.w + ((VariableQuaternion)oth).mValue.w);
                return;
            }
            else if (oth is VariableVector4)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x + ((VariableVector4)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y + ((VariableVector4)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z + ((VariableVector4)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w + ((VariableVector4)oth).mValue.w);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x + ((VariableVector4)oth).mValue.x);
                mValue.y = (mValue.y + ((VariableVector4)oth).mValue.y);
                mValue.z = (mValue.z + ((VariableVector4)oth).mValue.z);
                mValue.w = (mValue.w + ((VariableVector4)oth).mValue.w);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableByte).VarAdd((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarAdd(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableInt).VarAdd((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarAdd(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableFloat).VarAdd((VariableQuaternion)refTo);
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x - ((VariableQuaternion)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y - ((VariableQuaternion)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z - ((VariableQuaternion)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w - ((VariableQuaternion)oth).mValue.w);
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x - ((VariableVector4)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y - ((VariableVector4)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z - ((VariableVector4)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w - ((VariableVector4)oth).mValue.w);
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableByte).VarSub((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarSub(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableInt).VarSub((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarSub(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableFloat).VarSub((VariableQuaternion)refTo);
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
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x * ((VariableQuaternion)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y * ((VariableQuaternion)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z * ((VariableQuaternion)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w * ((VariableQuaternion)oth).mValue.w);
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue.x = (mValue.x * ((VariableVector4)oth).mValue.x);
                    ((VariableQuaternion)refTo).mValue.y = (mValue.y * ((VariableVector4)oth).mValue.y);
                    ((VariableQuaternion)refTo).mValue.z = (mValue.z * ((VariableVector4)oth).mValue.z);
                    ((VariableQuaternion)refTo).mValue.w = (mValue.w * ((VariableVector4)oth).mValue.w);
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableByte).VarMul((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarMul(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableInt).VarMul((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarMul(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableFloat).VarMul((VariableQuaternion)refTo);
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
            if (oth is VariableQuaternion)
            {
                VariableQuaternion oth_ = oth as VariableQuaternion;
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableQuaternion refTo_ = refTo as VariableQuaternion;
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
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableByte).VarDev((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarDev(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableInt).VarDev((VariableQuaternion)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarDev(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableQuaternion)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableQuaternion)refTo).mValue = mValue;
                    (oth as VariableFloat).VarDev((VariableQuaternion)refTo);
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
            if (!(oth is VariableQuaternion)) return;
            VariableQuaternion oth_ = oth as VariableQuaternion;

            if (refTo != null && refTo is VariableQuaternion)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableQuaternion refTo_ = refTo as VariableQuaternion;
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
            if (!(oth is VariableQuaternion)) return;
            VariableQuaternion oth_ = oth as VariableQuaternion;

            if (refTo != null && refTo is VariableQuaternion)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableQuaternion refTo_ = refTo as VariableQuaternion;
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
            if (refTo != null && refTo is VariableQuaternion)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableQuaternion refTo_ = refTo as VariableQuaternion;
                if (left is VariableQuaternion)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableQuaternion).mValue.x, (right as VariableQuaternion).mValue.x);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableQuaternion).mValue.y, (right as VariableQuaternion).mValue.y);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableQuaternion).mValue.z, (right as VariableQuaternion).mValue.z);
                    refTo_.mValue.w = Mathf.Clamp(mValue.w, (left as VariableQuaternion).mValue.w, (right as VariableQuaternion).mValue.w);
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
                if (left is VariableQuaternion)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableQuaternion).mValue.x, (right as VariableQuaternion).mValue.x);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableQuaternion).mValue.y, (right as VariableQuaternion).mValue.y);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableQuaternion).mValue.z, (right as VariableQuaternion).mValue.z);
                    mValue.w = Mathf.Clamp(mValue.w, (left as VariableQuaternion).mValue.w, (right as VariableQuaternion).mValue.w);
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
            if (!(oth is VariableQuaternion)) return;
            if (refTo != null && refTo is VariableQuaternion)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableQuaternion).mValue = Quaternion.Lerp(mValue, (oth as VariableQuaternion).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Quaternion.Lerp(mValue, (oth as VariableQuaternion).mValue, fValue);
            }
        }
        //------------------------------------------------------
        public void Slerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableQuaternion)) return;
            if (refTo != null && refTo is VariableQuaternion)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableQuaternion).mValue = Quaternion.Slerp(mValue, (oth as VariableQuaternion).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Quaternion.Slerp(mValue, (oth as VariableQuaternion).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            Vector4 rot = UnityEditor.EditorGUILayout.Vector4Field(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName, new Vector4(mValue.x, mValue.y, mValue.z, mValue.w));
            mValue = new Quaternion(rot.x, rot.y, rot.z, rot.w);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.QuaternionList)]
    [System.Serializable]
    public class VariableQuaternionList : ListVariable<List<Quaternion>>
    {
        public static implicit operator VariableQuaternionList(List<Quaternion> value) { return new VariableQuaternionList { mValue = value }; }
        public override EVariableType GetListElementType() { return EVariableType.Quaternion; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableQuaternionList>(bindVarGuid);
            }
        }  
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableQuaternionList pVar = pOther as VariableQuaternionList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<Quaternion>(pVar.mValue.Count);
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
                Quaternion qt = mValue[i];
                qt.eulerAngles = Vector3.one*180 + mValue[i].eulerAngles;
                mValue[i] = qt;
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableQuaternionList)
                mValue = ((VariableQuaternionList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableQuaternionList && right is VariableQuaternionList)
            {
                if (refTo != null && refTo is VariableQuaternionList)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableQuaternionList temp = refTo as VariableQuaternionList;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Quaternion rot = temp.mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableQuaternionList).mValue[i].x, (right as VariableQuaternionList).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableQuaternionList).mValue[i].y, (right as VariableQuaternionList).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableQuaternionList).mValue[i].z, (right as VariableQuaternionList).mValue[i].z);
                        rot.w = Mathf.Clamp(mValue[i].w, (left as VariableQuaternionList).mValue[i].w, (right as VariableQuaternionList).mValue[i].w);
                        temp.mValue[i] = rot;
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Quaternion rot = mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableQuaternionList).mValue[i].x, (right as VariableQuaternionList).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableQuaternionList).mValue[i].y, (right as VariableQuaternionList).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableQuaternionList).mValue[i].z, (right as VariableQuaternionList).mValue[i].z);
                        rot.w = Mathf.Clamp(mValue[i].w, (left as VariableQuaternionList).mValue[i].w, (right as VariableQuaternionList).mValue[i].w);
                        mValue[i] = rot;
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableQuaternionList)) return;
            VariableQuaternionList oth_ = (VariableQuaternionList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableQuaternionList refTo_ = refTo as VariableQuaternionList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Quaternion.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Quaternion.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public void Slerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableQuaternionList)) return;
            VariableQuaternionList oth_ = (VariableQuaternionList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableQuaternionList refTo_ = refTo as VariableQuaternionList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Quaternion.Slerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Quaternion.Slerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableQuaternion)
            {
                mValue.Add((item as VariableQuaternion).mValue);
            }
            else if (item is VariableVector4)
            {
                mValue.Add(new Quaternion((item as VariableVector4).mValue.x, (item as VariableVector4).mValue.y, (item as VariableVector4).mValue.z, (item as VariableVector4).mValue.w));
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableQuaternion)
            {
                return mValue.IndexOf((item as VariableQuaternion).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableQuaternion)
            {
                (refTo as VariableQuaternion).mValue = mValue[index];
            }
            else if (refTo is VariableVector4)
            {
                (refTo as VariableVector4).mValue = new Vector4(mValue[index].x, mValue[index].y, mValue[index].z, mValue[index].w);
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableQuaternionList)) return 1;
            VariableQuaternionList othList = (VariableQuaternionList)oth;
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
            if (mValue == null) mValue = new List<Quaternion>();
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ?strName: param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(Quaternion.identity);
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
                    Vector4 rot = UnityEditor.EditorGUILayout.Vector4Field("[" + i + "]", new Vector4(mValue[i].x, mValue[i].y, mValue[i].z, mValue[i].w));
                    mValue[i] = new Quaternion(rot.x, rot.y, rot.z, rot.w);
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
