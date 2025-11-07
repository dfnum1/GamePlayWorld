/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	Variable
作    者:	HappLI
描    述:	AT基础节点
*********************************************************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace Framework.Plugin.AT
{
#if UNITY_EDITOR
    public struct DrawUIParam
    {
        public Vector2 size;
        public float offsetWidth;
        public string strDefaultName;
        public Type asignType;
        public Type displayType;
        public bool bEdit;
        public void Clear()
        {
            size = Vector2.zero;
            offsetWidth = 0;
            asignType = null;
            displayType = null;
            strDefaultName = "";
            bEdit = true;
        }

        public static DrawUIParam Current = new DrawUIParam();
    }
#endif
    /*
    public interface IUserData
    {
        void Destroy();
    }*/
    public interface IVariableOp
    {
        //------------------------------------------------------
        bool isNull();
        int CompareTo(Variable oth);
        void EqualTo(Variable oth);
        void AddTo(Variable oth, Variable refTo=null);
        void SubTo(Variable oth, Variable refTo = null);
        void MulTo(Variable oth, Variable refTo = null);
        void DevTo(Variable oth, Variable refTo = null);

        void Min(Variable oth, Variable refTo = null);
        void Max(Variable oth, Variable refTo = null);
        void Clamp(Variable left, Variable right, Variable refTo = null);
        void Lerp(Variable oth, float fValue, Variable refTo = null);
    }

    //------------------------------------------------------
    public abstract class Variable : BaseNode, IVariableOp
    {
        public virtual int GetClassHashCode() { return 0; }
        public virtual void SetClassHashCode(int hashCode) { }
        public virtual bool IsList() { return false; }
        public virtual IList GetList() { return null; }
        public virtual int IndexofList(Variable item) { return -1; }
        public virtual void AddToList(Variable item) { if(IsList()) throw new NotImplementedException(); }
        public virtual void GetListItem(int index, Variable refTo = null) { if (IsList()) throw new NotImplementedException(); }
        public virtual EVariableType GetListElementType() { if (IsList()) throw new NotImplementedException(); return EVariableType.Null; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeID = true)
        {
            if (bIncludeID) GUID = pOther.GUID;
            flags = pOther.flags;
            strName = pOther.strName;
        }
        //------------------------------------------------------
        public virtual void EqualTo(Variable oth)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------
        public virtual int CompareTo(Variable oth)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------
        public virtual void AddTo(Variable oth, Variable refTo = null)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------
        public virtual void SubTo(Variable oth, Variable refTo = null)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------
        public virtual void MulTo(Variable oth, Variable refTo = null)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------
        public virtual void DevTo(Variable oth, Variable refTo = null)
        {
            throw new NotImplementedException();
        }
        public virtual void Min(Variable oth, Variable refTo = null) { }
        public virtual void Max(Variable oth, Variable refTo = null) { }
        public virtual void Reverse() { }
        public virtual void Clamp(Variable left, Variable right, Variable refTo = null) { }
        public virtual void Lerp(Variable oth, float fValue, Variable refTo = null) { }
        //------------------------------------------------------
        public override void Destroy()
        {
        }
        //------------------------------------------------------
        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
        }
        //------------------------------------------------------
        public virtual bool isNull() { return false; }
#if UNITY_EDITOR
        //------------------------------------------------------
        public virtual Rect OnGUI(DrawUIParam param ) { return Rect.zero; }
        public virtual string ToValueText() { return ""; }
#endif
        //------------------------------------------------------
        public virtual void DoFill()
        {
        }
    }
    //------------------------------------------------------
    [System.Serializable]
    public abstract class AbsVariable<T> : Variable
    {
        [SerializeField]
        public T mValueDef;

        [SerializeField]
        public int bindVarGuid = 0;

        [System.NonSerialized]
        public T mValue;


        [System.NonSerialized]
        public AbsVariable<T> pBindVariable;

#if UNITY_EDITOR
        public override string ToValueText()
        {
            string strText = "";
            if (mValueDef != null)
                strText += "src=" + mValueDef.ToString() + "\r\n";
            if(mValue != null)
                strText +=  "runtime =" + mValue.ToString();
            if (pBindVariable != null)
                strText += "   bind-" + pBindVariable.ToValueText();
            return strText;
        }
#endif
        public override void Init(AgentTree pTree)
        {
            if(m_bInited)return;
            base.Init(pTree);
            mValue = mValueDef;

        }
        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            if(!IsFlag(EFlag.Locked))
            {
                mValue = mValueDef;
                if (pBindVariable != null) mValue = pBindVariable.mValue;
            }
        }
        public override void Copy(BaseNode pOther, bool bIncludeID = true)
        {
            base.Copy(pOther, bIncludeID);
            if (pOther.GetType() != GetType()) return;

            AbsVariable<T> pVar = pOther as AbsVariable<T>;
            mValue = pVar.mValue;
            mValueDef = pVar.mValueDef;
            pBindVariable = pVar.pBindVariable;
            bindVarGuid = pVar.bindVarGuid;
#if UNITY_EDITOR

#endif
        }
#if UNITY_EDITOR
        public override void Save()
        {
            base.Save();
            if (pBindVariable != null)
            {
                bindVarGuid = pBindVariable.GUID;
            }
            else
            {
                mValueDef = mValue;
                bindVarGuid = 0;
            }
        }
#endif
        //------------------------------------------------------
        public override void DoFill()
        {
            if (pBindVariable != null) mValue = pBindVariable.mValue;
        }
    }
    //------------------------------------------------------
    public abstract class ListVariable<T> : AbsVariable<T>
    {
        public override bool IsList() { return true; }
        public override IList GetList() { return (IList)mValue; }
    }
}
