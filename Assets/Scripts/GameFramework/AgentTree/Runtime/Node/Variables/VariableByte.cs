/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableByte
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    [VariableType(EVariableType.Byte)]
    [System.Serializable]
    public class VariableByte : AbsVariable<byte>
    {
        public static implicit operator VariableByte(byte value) { return new VariableByte { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableByte>(bindVarGuid);
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
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector2Int refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector2Int refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector2Int refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
        }
        //------------------------------------------------------
        public void VarAdd(VariableVector3Int refP)
        {
            refP.mValue.x += mValue;
            refP.mValue.y += mValue;
            refP.mValue.z += mValue;
        }
        //------------------------------------------------------
        public void VarSub(VariableVector3Int refP)
        {
            refP.mValue.x -= mValue;
            refP.mValue.y -= mValue;
            refP.mValue.z -= mValue;
        }
        //------------------------------------------------------
        public void VarMul(VariableVector3Int refP)
        {
            refP.mValue.x *= mValue;
            refP.mValue.y *= mValue;
            refP.mValue.z *= mValue;
        }
        //------------------------------------------------------
        public void VarDev(VariableVector3Int refP)
        {
            if (mValue == 0) return;
            refP.mValue.x /= mValue;
            refP.mValue.y /= mValue;
            refP.mValue.z /= mValue;
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
            if (oth is VariableByte) return mValue-((VariableByte)oth).mValue;
            if (oth is VariableInt) return mValue-((VariableByte)oth).mValue;
            return 1;
        }
        //------------------------------------------------------
        public override void Reverse()
        {
            if (IsFlag(EFlag.Const)) return;
            mValue = (byte)(255-mValue);
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableByte)
                mValue = ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void AddTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableByte)) return;
            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableByte)refTo).mValue = (byte)(mValue + ((VariableByte)oth).mValue);
                return;
            }
            if (IsFlag(EFlag.Const)) return;
            mValue += ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void SubTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableByte)) return;
            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableByte)refTo).mValue = (byte)(mValue - ((VariableByte)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue -= ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void MulTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableByte)) return;
            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableByte)refTo).mValue = (byte)(mValue * ((VariableByte)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue *= ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void DevTo(Variable oth, Variable refTo = null)
        {
            if (!(oth is VariableByte)) return;
            if ((oth as VariableByte).mValue == 0) return;

            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                ((VariableByte)refTo).mValue = (byte)(mValue / ((VariableByte)oth).mValue);
            }
            if (IsFlag(EFlag.Const)) return;
            mValue /= ((VariableByte)oth).mValue;
        }
        //------------------------------------------------------
        public override void Min(Variable oth, Variable refTo = null)
        {
            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) (refTo as VariableByte).mValue = (byte)Mathf.Min((oth as VariableByte).mValue, mValue);
                else if (oth is VariableInt) (refTo as VariableByte).mValue = (byte)Mathf.Min((oth as VariableInt).mValue, mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) mValue = (byte)Mathf.Min(mValue, (oth as VariableByte).mValue);
                else if (oth is VariableInt) mValue = (byte)Mathf.Min(mValue, (oth as VariableInt).mValue);
            }
        }
        //------------------------------------------------------
        public override void Max(Variable oth, Variable refTo = null)
        {
            if (refTo !=null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) (refTo as VariableByte).mValue = (byte)Mathf.Max((oth as VariableByte).mValue, mValue);
                else if (oth is VariableInt) (refTo as VariableByte).mValue = (byte)Mathf.Max((oth as VariableInt).mValue, mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) mValue = (byte)Mathf.Max(mValue, (oth as VariableByte).mValue);
                else if (oth is VariableInt) mValue = (byte)Mathf.Max(mValue, (oth as VariableInt).mValue);
            }
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (refTo !=null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (left is VariableByte) (refTo as VariableByte).mValue = (byte)Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) (refTo as VariableByte).mValue = (byte)Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                if (left is VariableByte) mValue = (byte)Mathf.Clamp(mValue, (left as VariableByte).mValue, (right as VariableByte).mValue);
                else if (left is VariableInt) mValue = (byte)Mathf.Clamp(mValue, (left as VariableInt).mValue, (right as VariableInt).mValue);
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableByte)) return;
            if (refTo != null && refTo is VariableByte)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                if (oth is VariableByte) (refTo as VariableByte).mValue = (byte)Mathf.Lerp(mValue, (oth as VariableByte).mValue, fValue);
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                mValue = (byte)Mathf.Lerp(mValue, (oth as VariableByte).mValue, fValue);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            string label = !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName;
            object ret = AgentTreeUtl.DrawProperty(label, mValue, param.displayType);
            if (ret == null)
                mValue = (byte)Mathf.Clamp(UnityEditor.EditorGUILayout.IntField(label, mValue), 0, 255);
            else
                mValue = System.Convert.ToByte(ret);
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.ByteList)]
    [System.Serializable]
    public class VariableByteList : ListVariable<List<byte>>
    {
        public static implicit operator VariableByteList(List<byte> value) { return new VariableByteList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableByteList>(bindVarGuid);
            }
        }    
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableByteList pVar = pOther as VariableByteList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<byte>(pVar.mValue.Count);
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
                mValue[i] = (byte)(255-mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableByteList)
                mValue = ((VariableByteList)oth).mValue;
        }
        //------------------------------------------------------
        public override void Clamp(Variable left, Variable right, Variable refTo = null)
        {
            if (left is VariableByte && right is VariableByte)
            {
                if(refTo != null && refTo is VariableByteList)
                {
                    if (refTo.IsFlag(EFlag.Const)) return;
                    VariableByteList temp = refTo as VariableByteList;
                    if (temp.mValue.Count != mValue.Count) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        temp.mValue[i] = (byte)Mathf.Clamp(mValue[i], (left as VariableByte).mValue, (right as VariableByte).mValue);
                    }
                }
                else
                {
                    if (IsFlag(EFlag.Const)) return;
                    for (int i = 0; i < mValue.Count; ++i)
                    {
                        mValue[i] = (byte)Mathf.Clamp(mValue[i], (left as VariableByte).mValue, (right as VariableByte).mValue);
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Lerp(Variable oth, float fValue, Variable refTo = null)
        {
            if (!(oth is VariableByteList)) return;
            VariableByteList oth_ = (VariableByteList)oth;
            if (oth_.mValue.Count != mValue.Count) return;
            if (refTo != null)
            {
                if (refTo.IsFlag(EFlag.Const)) return;
                VariableByteList refTo_ = refTo as VariableByteList;
                if (refTo_.mValue.Count != mValue.Count) return;
                for(int i = 0; i < mValue.Count; ++i)
                {
                    refTo_.mValue[i] = (byte)Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
            else
            {
                if (IsFlag(EFlag.Const)) return;
                for (int i = 0; i < mValue.Count; ++i)
                {
                    mValue[i] = (byte)Mathf.Lerp(mValue[i], oth_.mValue[i], fValue);
                }
            }
        }
        //------------------------------------------------------
        public override void AddToList(Variable item)
        {
            if (IsFlag(EFlag.Const)) return;
            if (item is VariableByte)
            {
                mValue.Add((item as VariableByte).mValue);
            }
            else if (item is VariableInt)
            {
                mValue.Add((byte)(item as VariableInt).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableByte)
            {
                return mValue.IndexOf((item as VariableByte).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if(refTo is VariableByte)
            {
                (refTo as VariableByte).mValue = mValue[index];
            }
            else if (refTo is VariableInt)
            {
                (refTo as VariableInt).mValue = mValue[index];
            }
        }
        public override EVariableType GetListElementType() { return EVariableType.Byte; }

        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableByteList)) return 1;
            VariableByteList othList = (VariableByteList)oth;
            if (othList.mValue == null || mValue == null) return 1;
            if (othList.mValue.Count != mValue.Count) return 1;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (mValue[i] != othList.mValue[i]) return mValue[i]- othList.mValue[i];
            }

            return 0;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (mValue == null) mValue = new List<byte>();
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
                        mValue[i] = (byte)Mathf.Clamp(UnityEditor.EditorGUILayout.IntField("[" + i + "]", mValue[i]), 0, 255);
                    else
                        mValue[i] = System.Convert.ToByte(ret);

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
