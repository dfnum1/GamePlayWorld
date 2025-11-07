/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableMonoScript
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    [VariableType(EVariableType.MonoScript)]
    [System.Serializable]
    public class VariableMonoScript : AbsVariable<Behaviour>
    {
        public int hashCode = 0;
        public static implicit operator VariableMonoScript(Behaviour value) { return new VariableMonoScript { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableMonoScript>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override int GetClassHashCode() { return hashCode; }
        public override void SetClassHashCode(int hashCode) { this.hashCode = hashCode; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableMonoScript)) return 1;
            return (mValue == (oth as VariableMonoScript).mValue)?0:1;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableMonoScript)
                mValue = ((VariableMonoScript)oth).mValue;
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (IsFlag(EFlag.Const)) return;
            base.Destroy();
            if (IsFlag(EFlag.AutoDestroy))
            {
                if (mValue != null)
                    UnityEngine.Object.Destroy(mValue);
                mValue = null;
                hashCode = 0;
            }
        }
        //------------------------------------------------------
        public T ToObject<T>() where T : Behaviour
        {
            if (mValue == null) return null;
            return mValue as T;
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue!=null; }
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            if (param.asignType != null) hashCode = AgentTreeUtl.TypeToHash(param.asignType);
            string label = !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName;
            if (param.asignType != null)
            {
                mValue = UnityEditor.EditorGUILayout.ObjectField(label, mValue, param.asignType, true) as Behaviour;
                param.offsetWidth = AgentTreeEditorResources.styles.nodeBody.CalcSize(new GUIContent(param.asignType.Name)).x;
            }
            else
                mValue = UnityEditor.EditorGUILayout.ObjectField(label, mValue, typeof(Behaviour), true) as Behaviour;
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.MonoScriptList)]
    [System.Serializable]
    public class VariableMonoScriptList : ListVariable<List<Behaviour>>
    {
        public int hashCode = 0;
        public static implicit operator VariableMonoScriptList(List<Behaviour> value) { return new VariableMonoScriptList { mValue = value }; }
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableMonoScriptList>(bindVarGuid);
            }
        }
        //------------------------------------------------------
        public override int GetClassHashCode() { return hashCode; }
        public override void SetClassHashCode(int hashCode) { this.hashCode = hashCode; }

        //------------------------------------------------------
        public override EVariableType GetListElementType() { return EVariableType.MonoScript; }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableMonoScriptList pVar = pOther as VariableMonoScriptList;
            hashCode = pVar.hashCode;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<Behaviour>(pVar.mValue.Count);
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
            if (oth is VariableMonoScriptList)
                mValue = ((VariableMonoScriptList)oth).mValue;
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
            if (item is VariableMonoScript)
            {
                mValue.Add((item as VariableMonoScript).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableMonoScript)
            {
                return mValue.IndexOf((item as VariableMonoScript).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableMonoScript)
            {
                (refTo as VariableMonoScript).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableMonoScriptList)) return 1;
            VariableMonoScriptList othList = oth as VariableMonoScriptList;
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
            if (mValue == null) mValue = new List<Behaviour>();
            if (param.asignType != null) hashCode = AgentTreeUtl.TypeToHash(param.asignType);
            AgentTreeUtl.BeginHorizontal();
            SetFlag(EFlag.Expanded, UnityEditor.EditorGUILayout.Foldout(IsFlag(EFlag.Expanded), !string.IsNullOrEmpty(strName) ? strName : param.strDefaultName));
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
                    mValue[i] = UnityEditor.EditorGUILayout.ObjectField("[" + i + "]", mValue[i], typeof(Behaviour), true) as Behaviour;
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
