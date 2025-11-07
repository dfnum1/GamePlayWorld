/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SelectToggleGroup
作    者:	happli
描    述:	选择组
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport]
    public class SelectToggleGroup : MonoBehaviour
    {
        public enum EToggleType
        {
            Active,
            Color,
            Scale,
        }
        [System.Serializable]
        public struct Item
        {
            public EventTriggerListener listener;
            public UnityEngine.Object pCtl;
            public EToggleType toggleType;
            public int toggleParam0;
            public int toggleParam1;
        }
        [System.Serializable]
        public struct Toogle
        {
            public int toggleIndex;
            public List<Item> items;
#if UNITY_EDITOR
            [System.NonSerialized]
            public bool bEpxand;
#endif
        }
        public List<Toogle> ToggleGroups;
        [SerializeField]
        int defaultIndex = 0;

        int m_nSelectIndex = -1;
        void Start()
        {
            if (m_nSelectIndex != -1 || defaultIndex < 0) return;
            SetSelect(defaultIndex);
        }
        //-------------------------------------------------
        private void OnEnable()
        {
            if (m_nSelectIndex != -1 || defaultIndex < 0) return;
            SetSelect(defaultIndex);
        }
        //-------------------------------------------------
        public void SetSelect(int index)
        {
            if (m_nSelectIndex == index) return;
            m_nSelectIndex = index;
            Toogle toggle;
            for (int i =0; i < ToggleGroups.Count; ++i)
            {
                toggle = ToggleGroups[i];
                if (toggle.items != null)
                {
                    for (int j = 0; j < toggle.items.Count; ++j)
                    {
                        UpdateItem(toggle.items[j], toggle.toggleIndex == index);
                    }
                }
            }
        }
        //-------------------------------------------------
        void UpdateItem(Item item, bool bToggled)
        {
            if (item.pCtl == null) return;
            switch (item.toggleType)
            {
                case EToggleType.Active:
                    {
                        GameObject pObj = item.pCtl as GameObject;
                        if(pObj)
                        {
                            if (bToggled)
                            {
                                pObj.SetActive(item.toggleParam0 != 0);
                            }
                            else
                            {
                                pObj.SetActive(item.toggleParam0 == 0);
                            }
                        }
                    }
                    break;
                case EToggleType.Color:
                    {
                        UnityEngine.UI.Graphic grphaic = item.pCtl as UnityEngine.UI.Graphic;
                        if(grphaic)
                        {
                            if (bToggled)
                            {
                                Color color = Color.white;
                                color[0] = ((item.toggleParam0 & 0x00ff0000) >> 16) / 255f;
                                color[1] = ((item.toggleParam0 & 0x0000ff00) >> 8) / 255f;
                                color[2] = ((item.toggleParam0 & 0x000000ff)) / 255f;
                                color[3] = ((item.toggleParam0 & 0xff000000) >> 24) / 255f;
                                grphaic.color = color;
                                UICommonScaler.ReplaceOriginalHightLight(grphaic, color);
                                if (item.listener != null) item.listener.ReplaceHightColor(color);
                            }
                            else
                            {
                                Color color = Color.white;
                                color[0] = ((item.toggleParam1 & 0x00ff0000) >> 16) / 255f;
                                color[1] = ((item.toggleParam1 & 0x0000ff00) >> 8) / 255f;
                                color[2] = ((item.toggleParam1 & 0x000000ff)) / 255f;
                                color[3] = ((item.toggleParam1 & 0xff000000) >> 24) / 255f;
                                grphaic.color = color;
                                UICommonScaler.ReplaceOriginalHightLight(grphaic, color);
                                if (item.listener != null) item.listener.ReplaceHightColor(color);
                            }
                        }
                    }
                    break;
                case EToggleType.Scale:
                    {
                        Transform transform = item.pCtl as Transform;
                        if(transform)
                        {
                            if (bToggled)
                            {
                                transform.localScale = Vector3.one * item.toggleParam0 * 0.001f;
                                UICommonScaler.ReplaceOriginalScaler(transform, transform.localScale);
                                if (item.listener != null) item.listener.ReplaceScale(transform.localScale);
                            }
                            else
                            {
                                transform.localScale = Vector3.one * item.toggleParam1 * 0.001f;
                                UICommonScaler.ReplaceOriginalScaler(transform, transform.localScale);
                                if (item.listener != null) item.listener.ReplaceScale(transform.localScale);
                            }
                        }
          
                    }
                    break;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(SelectToggleGroup), true)]
    public class SelectToggleGroupEditor : Editor
    {
        SerializedProperty m_DefaultIndex;
        public override void OnInspectorGUI()
        {
            SelectToggleGroup selectGroup = target as SelectToggleGroup;

            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            if (m_DefaultIndex == null) m_DefaultIndex = serializedObject.FindProperty("defaultIndex");
            if (m_DefaultIndex!=null) EditorGUILayout.PropertyField(m_DefaultIndex, true);
            EditorGUILayout.HelpBox("<0则不选择", MessageType.Info);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("添加组"))
            {
                if (selectGroup.ToggleGroups == null) selectGroup.ToggleGroups = new List<SelectToggleGroup.Toogle>();
                selectGroup.ToggleGroups.Add( new SelectToggleGroup.Toogle());
            }
            Color color = GUI.color;
            if (selectGroup.ToggleGroups!=null)
            {
                SelectToggleGroup.Toogle toggle;
                for (int i = 0; i < selectGroup.ToggleGroups.Count; ++i)
                {
                    toggle = selectGroup.ToggleGroups[i];
                    EditorGUILayout.BeginHorizontal();
                    toggle.bEpxand = EditorGUILayout.Foldout(toggle.bEpxand, "组[" + toggle.toggleIndex + "]");
                    toggle.toggleIndex = EditorGUILayout.IntField(toggle.toggleIndex, new GUILayoutOption[] { GUILayout.Width(80) });
                    if (GUILayout.Button("添加状态", new GUILayoutOption[] { GUILayout.Width(60) }))
                    {
                        if (toggle.items == null) toggle.items = new List<SelectToggleGroup.Item>();
                        toggle.items.Add(new SelectToggleGroup.Item());
                    }
                    if (GUILayout.Button("移除", new GUILayoutOption[] { GUILayout.Width(40) }))
                    {
                        selectGroup.ToggleGroups.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (toggle.bEpxand  && toggle.items!=null)
                    {
                        EditorGUI.indentLevel++;
                        for (int j =0; j < toggle.items.Count; ++j)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUI.color = Color.green;
                            EditorGUILayout.LabelField("组件状态[" + j + "]");
                            if (GUILayout.Button("移除", new GUILayoutOption[] { GUILayout.Width(40) }))
                            {
                                toggle.items.RemoveAt(j);
                                break;
                            }
                            GUI.color = color;
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel++;
                            SelectToggleGroup.Item item = toggle.items[j];
                            item.toggleType = (SelectToggleGroup.EToggleType)EditorGUILayout.EnumPopup("状态类型", item.toggleType);
                            if (item.toggleType == SelectToggleGroup.EToggleType.Active)
                            {
                                item.pCtl = EditorGUILayout.ObjectField("组件", item.pCtl, typeof(GameObject), true) as GameObject;
                                item.toggleParam0 = EditorGUILayout.Toggle("选中时显示", item.toggleParam0!=0)?1:0;
                            }
                            else if (item.toggleType == SelectToggleGroup.EToggleType.Color)
                            {
                                item.listener = EditorGUILayout.ObjectField("监听组件", item.listener, typeof(EventTriggerListener), true) as EventTriggerListener;
                                item.pCtl = EditorGUILayout.ObjectField("组件", item.pCtl, typeof(UnityEngine.UI.Graphic), true) as UnityEngine.UI.Graphic;
                                Color selectColor = new Color(((item.toggleParam0 & 0x00ff0000) >> 16)/ 255.0f, ((item.toggleParam0 & 0x0000ff00) >> 8) / 255.0f, ((item.toggleParam0 & 0x000000ff)) / 255.0f, ((item.toggleParam0 & 0xff000000) >> 24) / 255.0f);
                                Color unSelectColor = new Color(((item.toggleParam1 & 0x00ff0000) >> 16) / 255.0f, ((item.toggleParam1 & 0x0000ff00) >> 8) / 255.0f, ((item.toggleParam1 & 0x000000ff)) / 255.0f, ((item.toggleParam1 & 0xff000000) >> 24) / 255.0f);
                                selectColor = EditorGUILayout.ColorField("选中时颜色", selectColor);
                                unSelectColor = EditorGUILayout.ColorField("未选中时颜色", unSelectColor);
                                item.toggleParam0 = ((byte)(selectColor.a * 255.0f) << 24) | ((byte)(selectColor.r * 255.0f) << 16) | ((byte)(selectColor.g * 255.0f) << 8) | (byte)(selectColor.b * 255.0f);
                                item.toggleParam1 = ((byte)(unSelectColor.a * 255.0f) << 24) | ((byte)(unSelectColor.r * 255.0f) << 16) | ((byte)(unSelectColor.g * 255.0f) << 8) | (byte)(unSelectColor.b * 255.0f);
                            }
                            else if (item.toggleType == SelectToggleGroup.EToggleType.Scale)
                            {
                                item.listener = EditorGUILayout.ObjectField("监听组件", item.listener, typeof(EventTriggerListener), true) as EventTriggerListener;
                                item.pCtl = EditorGUILayout.ObjectField("组件", item.pCtl, typeof(Transform), true) as Transform;
                                float scale0 = EditorGUILayout.FloatField("选中时缩放", item.toggleParam0*0.001f);
                                float scale1 = EditorGUILayout.FloatField("未选中时缩放", item.toggleParam1 * 0.001f);
                                item.toggleParam0 = (int)(scale0 * 1000);
                                item.toggleParam1 = (int)(scale1 * 1000);
                            }
                            EditorGUI.indentLevel--;
                            toggle.items[j] = item;
                        }
                        EditorGUI.indentLevel--;
                    }
                    selectGroup.ToggleGroups[i] = toggle;
                }
            }
            GUI.color = color;

            serializedObject.ApplyModifiedProperties();
            if(GUILayout.Button("刷新"))
            {
                if(m_DefaultIndex!=null) selectGroup.SetSelect(m_DefaultIndex.intValue);
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}

