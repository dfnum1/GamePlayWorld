/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	InstanceAbleHandler
作    者:	HappLI
描    述:	实例化对象句柄
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.Reflection;
#endif
namespace Framework.Core
{
    public class InstanceAbleHandler : AInstanceAble
    {
        [System.Serializable]
        public struct Widget
        {
            public Component widget;
            public string fastName;
#if UNITY_EDITOR
            [System.NonSerialized]
            public int assignType;

            [System.NonSerialized]
            public Component[] vComponents;
#endif
        }
        public Widget[] Widgets;
        private bool m_bSerialized = false;
        Dictionary<string, List<Component>> m_vWidgets = null;
        protected override void Awake()
        {
            base.Awake();
            Serialized();
        }
        //------------------------------------------------------
        protected void Serialized()
        {
            if (m_bSerialized) return;
            m_bSerialized = true;
            if (Widgets != null && Widgets.Length > 0)
            {
                m_vWidgets = new Dictionary<string, List<Component>>(Widgets.Length);
                for (int i = 0; i < Widgets.Length; ++i)
                {
                    if (Widgets[i].widget == null) continue;
                    List<Component> vmap;
                    if (!string.IsNullOrEmpty(Widgets[i].fastName))
                    {
                        if (!m_vWidgets.TryGetValue(Widgets[i].fastName, out vmap))
                        {
                            vmap = new List<Component>();
                            m_vWidgets.Add(Widgets[i].fastName, vmap);
                        }
                    }
                    else
                    {
                        if (!m_vWidgets.TryGetValue(Widgets[i].widget.name, out vmap))
                        {
                            vmap = new List<Component>();
                            m_vWidgets.Add(Widgets[i].widget.name, vmap);
                        }
                    }

                    vmap.Add(Widgets[i].widget);
                }
            }
        }
        //------------------------------------------------------
        public T GetWidget<T>(string strName) where T : Component
        {
            Serialized();
            if (m_vWidgets == null || m_vWidgets.Count <= 0) return null;

            List<Component> behavours;
            if (m_vWidgets.TryGetValue(strName, out behavours) && behavours.Count > 0)
            {
                Type type = typeof(T);
                if (behavours.Count == 1)
                {
                    return behavours[0] as T;
                }
                else
                {
                    for (int i = 0; i < behavours.Count; ++i)
                    {
                        if (behavours[i].GetType() == type)
                            return behavours[i] as T;
                    }

                    for (int i = 0; i < behavours.Count; ++i)
                    {
                        if (behavours[i] as T != null)
                            return behavours[i] as T;
                    }
                }
            }
            return null;
        }
#if UNITY_EDITOR
        public void AddWidget(Component component, string fastName= null)
        {
            if (component == null) return;
            List<Widget> vWidgets = null;
            if (Widgets!=null)
            {
                for (int i = 0; i < Widgets.Length; ++i)
                {
                    if (Widgets[i].widget == component)
                    {
                        Widgets[i].fastName = fastName;
                        return;
                    }
                }
                vWidgets = new List<Widget>(Widgets);
            }
           if(vWidgets == null) vWidgets = new List<Widget>();
            Widget widget = new Widget();
            widget.widget = component;
            widget.fastName = fastName;
            vWidgets.Add(widget);
            Widgets = vWidgets.ToArray();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InstanceAbleHandler))]
    [CanEditMultipleObjects]
    public class InstanceAbleHandlerEditor : Editor
    {
        static System.Type[] WidgetTypes = new System.Type[]
        {
            typeof(GameObject),
            typeof(MonoBehaviour),
            typeof(RectTransform),
            typeof(Transform),
            typeof(SpriteRenderer),
            typeof(DG.Tweening.DOTweenAnimation),
            typeof(AComSerialized),
            typeof(MonoBehaviour),
            typeof(RectTransform),
            typeof(Transform),
            typeof(CanvasRenderer),
            typeof(UnityEngine.EventSystems.UIBehaviour),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.Toggle),
            typeof(UnityEngine.UI.ToggleGroup),
            typeof(UnityEngine.UI.LayoutElement),
            typeof(UnityEngine.UI.LayoutGroup),
            typeof(UnityEngine.UI.GridLayoutGroup),
            typeof(UnityEngine.UI.CanvasScaler),
            typeof(UnityEngine.Canvas),
            typeof(UnityEngine.UI.Clipping),
            typeof(UnityEngine.UI.Mask),
            typeof(UnityEngine.UI.RawImage),
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.ScrollRect),
            typeof(UnityEngine.UI.Slider),
            typeof(UnityEngine.UI.RectMask2D),
            typeof(UnityEngine.UI.Outline),
            typeof(UnityEngine.UI.Shadow),
            typeof(UnityEngine.UI.InputField),
            typeof(UnityEngine.UI.Dropdown),
            typeof(UnityEngine.EventSystems.EventTrigger),
            typeof(UnityEngine.Playables.PlayableDirector),
            typeof(UnityEngine.Video.VideoPlayer),
            typeof(UnityEngine.Video.VideoClip),
            typeof(CanvasGroup),
        };
        Dictionary<int, System.Type> m_vTypes = new Dictionary<int, Type>();
        List<string> m_vTypeDisplay = new List<string>();
        HashSet<Object> m_vSets = new HashSet<Object>();
        bool m_bExpand = false;
        //    UnityEditorInternal.ReorderableList m_TextureReorderableList;
        int m_nAddType = 0;

        void CheckTypes()
        {
            if (m_vTypes.Count < WidgetTypes.Length)
            {
                for (int i = 0; i < WidgetTypes.Length; ++i)
                {
                    if (m_vTypes.ContainsKey(WidgetTypes[i].GetHashCode())) continue;
                    m_vTypes.Add(WidgetTypes[i].GetHashCode(), WidgetTypes[i]);
                    string name = WidgetTypes[i].ToString().Replace(".", "/");
                    m_vTypeDisplay.Add(name);
                }
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InstanceAbleHandler agb = target as InstanceAbleHandler;

            CheckTypes();
            Color color = GUI.color;
            m_bExpand = EditorGUILayout.Foldout(m_bExpand, "控件列表");
            if (m_bExpand)
            {
                EditorGUI.indentLevel++;
                m_vSets.Clear();
                if (agb.Widgets != null)
                {
                    for (int i = 0; i < agb.Widgets.Length; ++i)
                    {
                        if (m_vSets.Contains(agb.Widgets[i].widget))
                        {
                            GUI.color = Color.red;
                        }
                        else
                        {
                            GUI.color = color;

                            if (agb.Widgets[i].widget != null)
                                m_vSets.Add(agb.Widgets[i].widget);
                        }

                        System.Type type = typeof(MonoBehaviour);
                        int poip = -1;
                        int defaultpoip = -1;
                        for (int t = 0; t < m_vTypes.Count; ++t)
                        {
                            if (m_vTypes.ElementAt(t).Key == agb.Widgets[i].assignType)
                            {
                                type = m_vTypes.ElementAt(t).Value;
                                poip = t;
                                break;
                            }
                            if (defaultpoip == -1 && m_vTypes.ElementAt(t).Key == type.GetHashCode())
                            {
                                defaultpoip = t;
                            }
                        }
                        if (poip == -1) poip = defaultpoip;

                        int preType = agb.Widgets[i].assignType;
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("复制名字"))
                        {
                            GUIUtility.systemCopyBuffer = agb.Widgets[i].widget.name;
                        }
                        if (GUILayout.Button("复制代码"))
                        {
                            Component widget = agb.Widgets[i].widget;
                            string typeString = widget.GetType().ToString();
                            GUIUtility.systemCopyBuffer = typeString + " " + agb.Widgets[i].widget.name.ToLower() + " = ui.GetWidget<" + typeString + ">(\"" + agb.Widgets[i].widget.name + "\");";
                        }

                        agb.Widgets[i].widget = EditorGUILayout.ObjectField(agb.Widgets[i].widget, type, true) as Component;
                        agb.Widgets[i].fastName = EditorGUILayout.TextField(agb.Widgets[i].fastName);
                        poip = EditorGUILayout.Popup(poip, m_vTypeDisplay.ToArray());
                        if (poip >= 0 && poip < m_vTypes.Count)
                            agb.Widgets[i].assignType = m_vTypes.ElementAt(poip).Key;
                        if (agb.Widgets[i].assignType != preType)
                            m_nAddType = agb.Widgets[i].assignType;
                        if (GUILayout.Button("删除"))
                        {
                            List<InstanceAbleHandler.Widget> vData = new List<InstanceAbleHandler.Widget>(agb.Widgets);
                            vData.RemoveAt(i);
                            agb.Widgets = vData.ToArray();
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUI.color = color;
                if (GUILayout.Button("新建"))
                {
                    List<InstanceAbleHandler.Widget> vData = (agb.Widgets != null) ? new List<InstanceAbleHandler.Widget>(agb.Widgets) : new List<InstanceAbleHandler.Widget>();
                    vData.Add(new InstanceAbleHandler.Widget() { assignType = m_nAddType });
                    agb.Widgets = vData.ToArray();
                }
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}
