/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ObjectHudHp
作    者:	HappLI
描    述:	对象头顶血条信息
*********************************************************************/
using Framework.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.UI
{
    public class ObjectHudBar : AInstanceAble
    {
        [System.Serializable]
        public class UIAttriData
        {
            [Base.PluginDisplay("显示控件")]
            public AHudBarUI hudBar;

            [Data.FormViewBinder("AttributeType", "type"),Data.FormFieldDisplay("attrName"),Base.PluginDisplay("属性类型")]
            public byte baseAttriType;

            [Data.FormViewBinder("AttributeType", "type"), Data.FormFieldDisplay("attrName"), Base.StateGUIByField("showRate", "true"), Base.PluginDisplay("最大属性类型")]
            public byte maxAttriType;

            [Base.PluginDisplay("比例值")] public bool showRate;
        }
        public UIAttriData[] bindAttriDatas;
        public float fShowDuration = 9999;

        public bool IsValid()
        {
            return bindAttriDatas != null && bindAttriDatas.Length > 0;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectHudBar))]
    class ObjectHudBarEditor : Editor
    {
        bool m_bExpand = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ObjectHudBar hudBar = target as ObjectHudBar;

            hudBar.fShowDuration = EditorGUILayout.FloatField("显示时长", hudBar.fShowDuration);

            EditorGUILayout.BeginHorizontal();
            m_bExpand = EditorGUILayout.Foldout(m_bExpand, "控件绑定显示属性");
            if(UnityEngine.GUILayout.Button("新增"))
            {
                System.Collections.Generic.List<ObjectHudBar.UIAttriData> vList = new System.Collections.Generic.List<ObjectHudBar.UIAttriData>();
                if (hudBar.bindAttriDatas != null) vList.AddRange(hudBar.bindAttriDatas);

                ObjectHudBar.UIAttriData uiData = new ObjectHudBar.UIAttriData();
                vList.Add(uiData);
                hudBar.bindAttriDatas = vList.ToArray();
            }
            EditorGUILayout.EndHorizontal();
            if (m_bExpand && hudBar.bindAttriDatas!=null)
            {
                EditorGUI.indentLevel++;

                for(int i =0; i< hudBar.bindAttriDatas.Length; ++i)
                {
                    string labelName = "控件属性[" + (i + 1) + "]";
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(labelName);
                    if (UnityEngine.GUILayout.Button("-", new UnityEngine.GUILayoutOption[] { UnityEngine.GUILayout.Width(30) }))
                    {
                        System.Collections.Generic.List<ObjectHudBar.UIAttriData> vList = new System.Collections.Generic.List<ObjectHudBar.UIAttriData>(hudBar.bindAttriDatas);
                        vList.RemoveAt(i);
                        hudBar.bindAttriDatas = vList.ToArray();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;

                    var itemBar = hudBar.bindAttriDatas[i];
                    itemBar =(ObjectHudBar.UIAttriData)Framework.ED.InspectorDrawUtil.DrawProperty(itemBar, System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.Public);
                 //   hudBar.bindAttriDatas[i] = itemBar;
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            if (UnityEngine.GUILayout.Button("刷新保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}
