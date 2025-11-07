/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableVector3
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.Vector3)]
    [System.Serializable]
    public class VariableVector3 : AbsVariable<Vector3>
    {
        public static implicit operator VariableVector3(Vector3 value) { return new VariableVector3 { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableVector3>(bindVarGuid);
            }
        }    
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableVector3)) return 1;
            VariableVector3 othList = (VariableVector3)oth;

            if (Mathf.Abs(mValue.x - othList.mValue.x) > 0.01f ||
                Mathf.Abs(mValue.y - othList.mValue.y) > 0.01f ||
                Mathf.Abs(mValue.z - othList.mValue.z) > 0.01f) return 1;
            return 0;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableVector3)
                mValue = ((VariableVector3)oth).mValue;
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
            if (oth is VariableVector3)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = (mValue + ((VariableVector3)oth).mValue);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue = (mValue + ((VariableVector3)oth).mValue);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableByte).VarAdd((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarAdd(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableInt).VarAdd((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarAdd(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableFloat).VarAdd((VariableVector3)refTo);
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
            if (oth is VariableVector3)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = (mValue - ((VariableVector3)oth).mValue);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue = (mValue - ((VariableVector3)oth).mValue);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableByte).VarSub((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarSub(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableInt).VarSub((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarSub(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableFloat).VarSub((VariableVector3)refTo);
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
            if (oth is VariableVector3)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue.x = (mValue.x * ((VariableVector3)oth).mValue.x);
                    ((VariableVector3)refTo).mValue.y = (mValue.y * ((VariableVector3)oth).mValue.y);
                    ((VariableVector3)refTo).mValue.z = (mValue.z * ((VariableVector3)oth).mValue.z);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                mValue.x = (mValue.x * ((VariableVector3)oth).mValue.x);
                mValue.y = (mValue.y * ((VariableVector3)oth).mValue.y);
                mValue.z = (mValue.z * ((VariableVector3)oth).mValue.z);
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableByte).VarMul((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarMul(this);
                return;
            }
            else if (oth is VariableVector3)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableInt).VarMul((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarMul(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableFloat).VarMul((VariableVector3)refTo);
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
            if (oth is VariableVector3)
            {
                VariableVector3 oth_ = oth as VariableVector3;
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableVector3 refTo_ = refTo as VariableVector3;
                    if (oth_.mValue.x != 0) refTo_.mValue.x = mValue.x / oth_.mValue.x;
                    if (oth_.mValue.y != 0) refTo_.mValue.y = mValue.y / oth_.mValue.y;
                    if (oth_.mValue.z != 0) refTo_.mValue.z = mValue.z / oth_.mValue.z;
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                if (oth_.mValue.x != 0) mValue.x = mValue.x / oth_.mValue.x;
                if (oth_.mValue.y != 0) mValue.y = mValue.y / oth_.mValue.y;
                if (oth_.mValue.z != 0) mValue.z = mValue.z / oth_.mValue.z;
                return;
            }
            else if (oth is VariableByte)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableByte).VarDev((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableByte).VarDev(this);
                return;
            }
            else if (oth is VariableInt)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableInt).VarDev((VariableVector3)refTo);
                    return;
                }
                if (IsFlag(EFlag.Const)) return;
                (oth as VariableInt).VarDev(this);
                return;
            }
            else if (oth is VariableFloat)
            {
                if (refTo != null && refTo is VariableVector3)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    ((VariableVector3)refTo).mValue = mValue;
                    (oth as VariableFloat).VarDev((VariableVector3)refTo);
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
            if (!(oth is VariableVector3)) return;
            VariableVector3 oth_ = oth as VariableVector3;

            if (refTo != null && refTo is VariableVector3)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector3 refTo_ = refTo as VariableVector3;
                refTo_.mValue.x = Mathf.Min(oth_.mValue.x, mValue.x);
                refTo_.mValue.y = Mathf.Min(oth_.mValue.y, mValue.y);
                refTo_.mValue.z = Mathf.Min(oth_.mValue.z, mValue.z);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.x = Mathf.Min(oth_.mValue.x, mValue.x);
                mValue.y = Mathf.Min(oth_.mValue.y, mValue.y);
                mValue.z = Mathf.Min(oth_.mValue.z, mValue.z);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableVector3)) return;
            VariableVector3 oth_ = oth as VariableVector3;

            if (refTo != null && refTo is VariableVector3)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector3 refTo_ = refTo as VariableVector3;
                refTo_.mValue.x = Mathf.Max(oth_.mValue.x, mValue.x);
                refTo_.mValue.y = Mathf.Max(oth_.mValue.y, mValue.y);
                refTo_.mValue.z = Mathf.Max(oth_.mValue.z, mValue.z);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue.x = Mathf.Max(oth_.mValue.x, mValue.x);
                mValue.y = Mathf.Max(oth_.mValue.y, mValue.y);
                mValue.z = Mathf.Max(oth_.mValue.z, mValue.z);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableVector3)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector3 refTo_ = refTo as VariableVector3;
                if (left is VariableVector3)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableVector3).mValue.x, (right as VariableVector3).mValue.x);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableVector3).mValue.y, (right as VariableVector3).mValue.y);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableVector3).mValue.z, (right as VariableVector3).mValue.z);
                }
                else if (left is VariableByte)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableByte).mValue, (right as VariableByte).mValue);
                }
                else if (left is VariableInt)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableInt).mValue, (right as VariableInt).mValue);
                }
                else if (left is VariableFloat)
                {
                    refTo_.mValue.x = Mathf.Clamp(mValue.x, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.y = Mathf.Clamp(mValue.y, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    refTo_.mValue.z = Mathf.Clamp(mValue.z, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableVector3)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableVector3).mValue.x, (right as VariableVector3).mValue.x);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableVector3).mValue.y, (right as VariableVector3).mValue.y);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableVector3).mValue.z, (right as VariableVector3).mValue.z);
                }
                else if (left is VariableByte)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableByte).mValue, (right as VariableByte).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableByte).mValue, (right as VariableByte).mValue);
                }
                else if (left is VariableInt)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableInt).mValue, (right as VariableInt).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableInt).mValue, (right as VariableInt).mValue);
                }
                else if (left is VariableFloat)
                {
                    mValue.x = Mathf.Clamp(mValue.x, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    mValue.y = Mathf.Clamp(mValue.y, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                    mValue.z = Mathf.Clamp(mValue.z, (left as VariableFloat).mValue, (right as VariableFloat).mValue);
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableVector3)) return;
            if (refTo != null && refTo is VariableVector3)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                (refTo as VariableVector3).mValue = Vector3.Lerp(mValue, (oth as VariableVector3).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = Vector3.Lerp(mValue, (oth as VariableVector3).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            mValue = UnityEditor.EditorGUILayout.Vector3Field(!string.IsNullOrEmpty(strName) ? strName : param.strDefaultName, mValue);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.Vector3List)]
    [System.Serializable]
    public class VariableVector3List : ListVariable<List<Vector3>>
    {
        public static implicit operator VariableVector3List(List<Vector3> value) { return new VariableVector3List { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableVector3List>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.Vector3; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableVector3List pVar = pOther as VariableVector3List;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<Vector3>(pVar.mValue.Count);
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
            if (oth is VariableVector3List)
                mValue = ((VariableVector3List)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableVector3List && right is VariableVector3List)
            {
                if (refTo != null && refTo is VariableVector3List)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableVector3List temp = refTo as VariableVector3List;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Vector3 rot = temp.mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableVector3List).mValue[i].x, (right as VariableVector3List).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableVector3List).mValue[i].y, (right as VariableVector3List).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableVector3List).mValue[i].z, (right as VariableVector3List).mValue[i].z);
                        temp.mValue[i] = rot;
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        Vector3 rot = mValue[i];
                        rot.x = Mathf.Clamp(mValue[i].x, (left as VariableVector3List).mValue[i].x, (right as VariableVector3List).mValue[i].x);
                        rot.y = Mathf.Clamp(mValue[i].y, (left as VariableVector3List).mValue[i].y, (right as VariableVector3List).mValue[i].y);
                        rot.z = Mathf.Clamp(mValue[i].z, (left as VariableVector3List).mValue[i].z, (right as VariableVector3List).mValue[i].z);
                        mValue[i] = rot;
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableVector3List)) return;
            VariableVector3List oth_ = (VariableVector3List)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableVector3List refTo_ = refTo as VariableVector3List;
                if (refTo_.mValue.Count != mValue.Count) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = Vector3.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = Vector3.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableVector3)
            {
                mValue.Add((item as VariableVector3).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableVector3)
            {
                return mValue.IndexOf((item as VariableVector3).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableVector3)
            {
                (refTo as VariableVector3).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableVector3List)) return 1;
            VariableVector3List othList = (VariableVector3List)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (Mathf.Abs(mValue[i].x - othList.mValue[i].x) > 0.01f ||
                    Mathf.Abs(mValue[i].y - othList.mValue[i].y) > 0.01f ||
                    Mathf.Abs(mValue[i].z - othList.mValue[i].z) > 0.01f) return 1;
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<Vector3>();

            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(Vector3.zero);
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
                    mValue[i] = UnityEditor.EditorGUILayout.Vector3Field("[" + i + "]", mValue[i]);
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
