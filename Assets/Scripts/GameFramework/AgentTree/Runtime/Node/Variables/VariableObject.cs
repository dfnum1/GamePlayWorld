/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableObject
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Plugin.AT
{
    public enum EObjType : byte
    {
        Object,
        Instance,
        Prefab,
        Texture,
        Material,
        Shader,
        Renderer,
        Asset,
    }
    //------------------------------------------------------
    [VariableType(EVariableType.Object)]
    [System.Serializable]
    public class VariableObject : AbsVariable<UnityEngine.Object>
    {
        public EObjType type = EObjType.Object;

        [System.NonSerialized]
        Animator m_pAnimator = null;
        public Animator pAnimator
        {
            get
            {
                Check();
                return m_pAnimator;
            }
        }

        public static implicit operator VariableObject(UnityEngine.Object value) { return new VariableObject { mValue = value }; }
        [System.NonSerialized]
        bool m_bCheck = false;
        void Check()
        {
            if (m_bCheck) return;
            if (type == EObjType.Instance && mValue != null)
            {
                if (m_pAnimator == null) m_pAnimator = (mValue as GameObject).GetComponent<Animator>();
            }
            m_bCheck = true;
        }
        //------------------------------------------------------
        public T ToObject<T>() where T : UnityEngine.Object
        {
            if (mValue == null) return null;
            return mValue as T;
        }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableObject>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            if (IsFlag(EFlag.Const)) return;
            m_pAnimator = null;
            mValue = mValueDef;
            m_pTransfom = null;
            m_pRender = null;
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (IsFlag(EFlag.Const)) return;
            if (type == EObjType.Instance)
            {
                return;
            }
            if (mValue == null)
            {
                m_pAnimator = null;
                m_bCheck = false;
                return;
            }
            if (IsFlag(EFlag.AutoDestroy))
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(mValue,0.1f);
                else
                    GameObject.DestroyImmediate(mValue);
#else
            GameObject.Destroy(mValue,0.1f);
#endif
            }
            mValue = null;
            m_pTransfom = null;
            m_pRender = null;
            m_pAnimator = null;
            m_bCheck = false;
        }
        //------------------------------------------------------
        [System.NonSerialized]
        Renderer m_pRender;
        public Renderer render
        {
            get
            {
                if (type == EObjType.Instance && mValue != null && m_pRender == null)
                {
                    m_pRender = (mValue as GameObject).GetComponent<Renderer>();
                }
                return m_pRender;
            }
        }
        //------------------------------------------------------
        [System.NonSerialized]
        Transform m_pTransfom;
        public Transform pTransform
        {
            get
            {
                if (type == EObjType.Instance && mValue != null && m_pTransfom == null)
                {
                    m_pTransfom = (mValue as GameObject).transform;
                }
                return m_pTransfom;
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableObject && (oth as VariableObject).type == type)
                mValue = ((VariableObject)oth).mValue;
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableObject)) return 1;
            return mValue == (oth as VariableObject).mValue?0:1;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            System.Type tempType = param.asignType;
            if (tempType == null)
            {
                this.type = (EObjType)UnityEditor.EditorGUILayout.EnumPopup("资源类型", this.type);
                tempType = GetTypeDef(type);
            }
            string strLabel = !string.IsNullOrEmpty(strName) ? (strName+ ((IsFlag(EFlag.Const)) ? "(常量)" : "")) : param.strDefaultName;

            mValue = UnityEditor.EditorGUILayout.ObjectField(strLabel, mValue, tempType,true);
            return GUILayoutUtility.GetLastRect();
        }
        public static System.Type GetTypeDef(EObjType type)
        {
            Type typeDef = typeof(Object);
            if (type == EObjType.Instance) typeDef = typeof(GameObject);
            else if (type == EObjType.Prefab) typeDef = typeof(GameObject);
            else if (type == EObjType.Material) typeDef = typeof(Material);
            else if (type == EObjType.Texture) typeDef = typeof(Texture);
            else if (type == EObjType.Shader) typeDef = typeof(Shader);
            else if (type == EObjType.Asset) typeDef = typeof(ScriptableObject);
            return typeDef;
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.ObjectList)]
    [System.Serializable]
    public class VariableObjectList : ListVariable<List<UnityEngine.Object>>
    {
        public EObjType type = EObjType.Object;

        public static implicit operator VariableObjectList(List<UnityEngine.Object> value) { return new VariableObjectList { mValue = value }; }
        public override EVariableType GetListElementType() { return EVariableType.ObjectList; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableObjectList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableObjectList pVar = pOther as VariableObjectList;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<UnityEngine.Object>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (IsFlag(EFlag.Const)) return;
            base.Destroy();
            if (isNull()) return;
            for (int i = 0; i < mValue.Count; ++i)
            {
                if (mValue[i])
                {
                    GameObject.Destroy(mValue[i],0.1f);
                }
            }
            mValue.Clear();
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableObjectList && (oth as VariableObjectList).type == type)
                mValue = ((VariableObjectList)oth).mValue;
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
            if (item is VariableObject && (item as VariableObject).type == type)
            {
                mValue.Add((item as VariableObject).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableObject)
            {
                return mValue.IndexOf((item as VariableObject).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableObject && (refTo as VariableObject).type == type)
            {
                (refTo as VariableObject).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableObjectList)) return 1;
            VariableObjectList othList = oth as VariableObjectList;
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
            if (mValue == null) mValue = new List<Object>();
            System.Type tempType = param.asignType;
            if (tempType == null)
            {
                this.type = (EObjType)UnityEditor.EditorGUILayout.EnumPopup("资源类型", this.type);
                tempType = VariableObject.GetTypeDef(type);
            }

            string strLabel = !string.IsNullOrEmpty(strName) ? (strName+ ((IsFlag(EFlag.Const)) ? "(常量)" : "")) : param.strDefaultName;

            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), strLabel));
            if (GUILayout.Button("添加"))
            {
                SetFlag(EFlag.Expanded, true);
                mValue.Add(null);
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
                    mValue[i] = UnityEditor.EditorGUILayout.ObjectField("[" + i + "]", mValue[i], tempType,true);
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
