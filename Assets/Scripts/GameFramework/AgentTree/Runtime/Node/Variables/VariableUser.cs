/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableUser
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [VariableType(EVariableType.UserData)]
    [System.Serializable]
    public class VariableUser : AbsVariable<IUserData>
    {
        public int hashCode = 0;
        public bool bAutoDestroy = false;
        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableUser>(bindVarGuid);
            }
        }
        public override void SetClassHashCode(int hashCode) { this.hashCode = hashCode; }

        public override int GetClassHashCode() { return hashCode; }
        public override void Reset(System.Collections.Generic.HashSet<int> vLocks)
        {
            if (vLocks.Contains(this.GUID)) return;
            base.Reset(vLocks);
            if (IsFlag(EFlag.Const)) return;
            if (bAutoDestroy && mValue != null)
                mValue.Destroy();

            mValue = mValueDef;
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableUser)
                mValue = ((VariableUser)oth).mValue;
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (IsFlag(EFlag.Const)) return;
            if (bAutoDestroy)
            {
                base.Destroy();
                if (mValue != null) mValue.Destroy();
                mValue = null;
            }
        }
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableUser)) return 1;
            return mValue == (oth as VariableUser).mValue?0:1;
        }

        public static VariableUser DEFAULT = new VariableUser() { mValue = null };
#if UNITY_EDITOR
        //------------------------------------------------------
        public override Rect OnGUI(DrawUIParam param)
        {
            int index = -1;
            string strLabel = "";
            if (param.asignType != null) hashCode = AgentTreeUtl.TypeToHash(param.asignType);
            for (int i = 0; i < AgentTreeUtl.EXPORT_TYPE_MONOS.Count; ++i)
            {
                if (AgentTreeUtl.EXPORT_TYPE_MONOS[i].hashCode == hashCode)
                {
                    index = i;
                    strLabel += "C-(" + AgentTreeUtl.EXPORT_TYPE_MONOS[i].type.Name + ")";
                    break;
                }
            }
            if (string.IsNullOrEmpty(strLabel))
                strLabel = "类对象";

            param.offsetWidth = AgentTreeEditorResources.styles.nodeBody.CalcSize(new GUIContent(strLabel)).x;
            if (param.bEdit)
            {
                index = UnityEditor.EditorGUILayout.Popup(strLabel, index, AgentTreeUtl.POP_EXPORT_MONOS.ToArray());
                if (index >= 0 && index < AgentTreeUtl.EXPORT_TYPE_MONOS.Count)
                    hashCode = AgentTreeUtl.EXPORT_TYPE_MONOS[index].hashCode;
            }
            else
            {
                UnityEditor.EditorGUILayout.LabelField(strLabel);
            }
            
            return GUILayoutUtility.GetLastRect();
        }
#endif
    }
    //------------------------------------------------------
    [VariableType(EVariableType.UserDataList)]
    [System.Serializable]
    public class VariableUserList : ListVariable<List<IUserData>>
    {
        public int hashCode = 0;

        //------------------------------------------------------
        public override void Init(AgentTree pTree)
        {
            base.Init(pTree);
            if (bindVarGuid != 0 && bindVarGuid != this.GUID)
            {
                pBindVariable = pTree.GetVariable<VariableUserList>(bindVarGuid);
            }
        }
        public static implicit operator VariableUserList(List<IUserData> value) { return new VariableUserList { mValue = value }; }
        public override int GetClassHashCode() { return hashCode; }
        public override void SetClassHashCode(int hashCode) { this.hashCode = hashCode; }

        public override EVariableType GetListElementType() { return EVariableType.UserData; }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (IsFlag(EFlag.Const)) return;
            base.Destroy();
            if (isNull()) return;
            for (int i = 0; i < mValue.Count; ++i)
                mValue.Add(mValue[i]);
            mValue.Clear();
        }
        //------------------------------------------------------
        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther, bIncludeGuid);
            if (pOther.GetType() != GetType()) return;
            VariableUserList pVar = pOther as VariableUserList;
            hashCode = pVar.hashCode;
            if (pVar.mValue != null && pVar.mValue.Count > 0)
            {
                mValue = new List<IUserData>(pVar.mValue.Count);
                for (int i = 0; i < pVar.mValue.Count; ++i)
                    mValue.Add(pVar.mValue[i]);
            }
        }
        //------------------------------------------------------
        public override void EqualTo(Variable oth)
        {
            if (IsFlag(EFlag.Const)) return;
            if (oth is VariableUserList)
                mValue = ((VariableUserList)oth).mValue;
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
            if (item is VariableUser)
            {
                mValue.Add((item as VariableUser).mValue);
            }
        }
        //------------------------------------------------------
        public override int IndexofList(Variable item)
        {
            if (item is VariableUser)
            {
                return mValue.IndexOf((item as VariableUser).mValue);
            }
            return -1;
        }
        //------------------------------------------------------
        public override void GetListItem(int index, Variable refTo = null)
        {
            if (index < 0 || index >= mValue.Count || refTo == null) return;
            if (refTo is VariableUser)
            {
                (refTo as VariableUser).mValue = mValue[index];
            }
        }
        //------------------------------------------------------
        public override bool isNull() { return mValue == null; }
        //------------------------------------------------------
        public override int CompareTo(Variable oth)
        {
            if (oth == null || oth.GetType() != typeof(VariableUserList)) return 1;
            VariableUserList othList = oth as VariableUserList;
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
            if (mValue == null) mValue = new List<IUserData>();

            string strLabel = "";

            int index = -1;
            for (int i = 0; i < AgentTreeUtl.EXPORT_TYPE_MONOS.Count; ++i)
            {
                if (AgentTreeUtl.EXPORT_TYPE_MONOS[i].hashCode == hashCode)
                {
                    index = i;
                    strLabel += "C-(" + AgentTreeUtl.EXPORT_TYPE_MONOS[index].type.Name + ")";
                    break;
                }
            }
            if (string.IsNullOrEmpty(strLabel))
                strLabel = "类对象";
            if (param.bEdit)
            {
                index = UnityEditor.EditorGUILayout.Popup(strLabel, index, AgentTreeUtl.POP_EXPORT_MONOS.ToArray());
                if (index >= 0 && index < AgentTreeUtl.EXPORT_TYPE_MONOS.Count)
                    hashCode = AgentTreeUtl.EXPORT_TYPE_MONOS[index].hashCode;
            }
            else
                UnityEditor.EditorGUILayout.LabelField(strLabel);

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
                    UnityEditor.EditorGUILayout.LabelField("[" + i + "]");
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
