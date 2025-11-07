/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	ComSerialized
作    者:	HappLI
描    述:	组件序列化
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.Reflection;
#endif

namespace Framework.Core
{
    public class ComSerializedExportAttribute : Attribute
    {
        public ComSerializedExportAttribute()
        {

        }
    }
    public abstract class AComSerialized : MonoBehaviour
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

        Dictionary<string, List<Component>> m_vWidgets = null;
        protected bool m_bSerialized = false;
        private void Awake()
        {
            Serialized();
        }
        //------------------------------------------------------
        protected virtual void OnSerialized() { }
        //------------------------------------------------------
        protected void Serialized()
        {
            if (m_bSerialized) return;
            m_bSerialized = true;
            OnSerialized();
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
                if(behavours.Count == 1)
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
        //------------------------------------------------------
        public void SetWidget<T>(int index, Component component) where T : Component
        {
            if (Widgets == null || Widgets.Length <= 0 || index >= Widgets.Length) return;

            Widgets[index].widget = (component as T);
        }
        //------------------------------------------------------
        public bool IsExist<T>(Component component) where T : Component
        {
            Serialized();
            if (Widgets == null)
            {
                return false;
            }
            foreach (var item in Widgets)
            {
                if (item.widget == (component as T))
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public int GetIndex<T>(Component component) where T : Component
        {
            Serialized();
            int index = -1;
            foreach (var item in Widgets)
            {
                index++;
                if (item.widget == (component as T))
                {
                    return index;
                }
            }
            return index;
        }
        //------------------------------------------------------
        public void Destroy()
        {

        }
#if UNITY_EDITOR
        class SelectSerializedPop : EditorWindow
        {
            public static SelectSerializedPop ms_pInst;
            public static void Pop(List<AComSerialized> vList)
            {

            }
        }
        //------------------------------------------------------
        bool IsWidgeted(Component component)
        {
            if (component == null || Widgets == null) return false;
            for (int i = 0; i < Widgets.Length; ++i)
            {
                if (Widgets[i].widget == component)
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        void RemoveWidget(Component component)
        {
            if (component == null) return;
            List<Widget> widgets = null;
            if (Widgets != null) widgets = new List<Widget>(Widgets);
            else widgets = new List<Widget>();
            for (int i = 0; i < widgets.Count; ++i)
            {
                if (widgets[i].widget == component)
                {
                    widgets.RemoveAt(i);
                    Widgets = widgets.ToArray();
                    EditorUtility.SetDirty(this.gameObject);
                    if(this.gameObject.scene.IsValid())
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
                    }
                    return;
                }
            }
        }
        //------------------------------------------------------
        void AddWidget(Component component, string name = null)
        {
            if (component == null) return;
            List<Widget> widgets = null;
            if (Widgets != null) widgets = new List<Widget>(Widgets);
            else widgets = new List<Widget>();
            for(int i = 0; i < widgets.Count;++i)
            {
                if(widgets[i].widget == component)
                {
                    return;
                }
            }
            Widget newW = new Widget();
            newW.widget = component;
            newW.fastName = name;
            widgets.Add(newW);
            Widgets = widgets.ToArray();
            EditorUtility.SetDirty(this.gameObject);
            if (this.gameObject.scene.IsValid())
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
            }
        }
        //------------------------------------------------------
        static List<AComSerialized> GetComSerializeds(GameObject pGo)
        {
            List<AComSerialized> vLists = null;
            AComSerialized serialized = pGo.GetComponent<AComSerialized>();
            if(serialized!=null && serialized.gameObject != pGo)
            {
                if (vLists == null) vLists = new List<AComSerialized>();
                vLists.Add(serialized);
            }
            AComSerialized[] pars = pGo.GetComponentsInParent<AComSerialized>();
            if(pars!=null)
            {
                for(int i = 0; i < pars.Length; ++i)
                {
                    if (pars[i].gameObject == pGo) continue;
                    if (vLists == null) vLists = new List<AComSerialized>();
                    vLists.Add(pars[i]);
                }
            }
            return vLists;
        }
        //------------------------------------------------------
        [MenuItem("CONTEXT/Component/加入UI序列化", true)]
        static bool OnAddSerializeControllerCheck(MenuCommand cmd)
        {
            Component comp = cmd.context as Component;
            if (comp == null) return false;
            List<AComSerialized> serializeds = GetComSerializeds(comp.gameObject);
            if (serializeds == null || serializeds.Count <= 0) return false;
            if (serializeds.Count == 1) return !serializeds[0].IsWidgeted(comp);
            bool bCan = false;
            for(int i = 0; i < serializeds.Count; ++i)
            {
                if (!serializeds[i].IsWidgeted(comp))
                {
                    bCan = true;
                    break;
                }
            }
            return bCan;
        }
        //------------------------------------------------------
        [MenuItem("CONTEXT/Component/加入UI序列化")]
        static void OnAddSerializeController(MenuCommand cmd)
        {
            Component comp = cmd.context as Component;
            if (comp == null) return;
            List<AComSerialized> serializeds = GetComSerializeds(comp.gameObject);
            if (serializeds == null || serializeds.Count <= 0) return;
            for (int i = 0; i < serializeds.Count;)
            {
                if (serializeds[i].IsWidgeted(comp))
                {
                    serializeds.RemoveAt(i);
                }
                else ++i;
            }
            if (serializeds.Count == 1) serializeds[0].AddWidget(comp);
            else
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < serializeds.Count; ++i)
                {
                    string name = serializeds[i].name;
                    if (serializeds[i].transform.parent != null) name = serializeds[i].transform.parent + "->" + name;
                    menu.AddItem(new GUIContent(name), false, (object call) =>
                    {
                        ((AComSerialized)call).AddWidget(comp);
                    }, serializeds[i]);
                }
                menu.DropDown(new Rect(GetCurrentEventPosition(), new Vector2(200, 200)));
            }
        }
        //------------------------------------------------------
        public static Vector2 GetCurrentEventPosition()
        {
            if (Event.current != null) return Event.current.mousePosition;
            Event current = null;
            FieldInfo masterEvent = typeof(Event).GetField("s_MasterEvent", BindingFlags.NonPublic | BindingFlags.Static);
            if (masterEvent != null)
            {
                current = (Event)masterEvent.GetValue(null);
            }
            if (current != null) return current.mousePosition;
            return Input.mousePosition;
        }
        //------------------------------------------------------
        [MenuItem("CONTEXT/Component/移除UI序列化", true)]
        static bool OnRemoveSerializeControllerCheck(MenuCommand cmd)
        {
            Component comp = cmd.context as Component;
            if (comp == null) return false;
            List<AComSerialized> serializeds = GetComSerializeds(comp.gameObject);
            if (serializeds == null || serializeds.Count <= 0) return false;
            if (serializeds.Count == 1) return serializeds[0].IsWidgeted(comp);
            bool bCan = false;
            for (int i = 0; i < serializeds.Count; ++i)
            {
                if (serializeds[i].IsWidgeted(comp))
                {
                    bCan = true;
                    break;
                }
            }
            return bCan;
        }
        //------------------------------------------------------
        [MenuItem("CONTEXT/Component/移除UI序列化")]
        static void OnRemoveSerializeController(MenuCommand cmd)
        {
            Component comp = cmd.context as Component;
            if (comp == null) return;
            List<AComSerialized> serializeds = GetComSerializeds(comp.gameObject);
            if (serializeds == null || serializeds.Count <= 0) return;
            for (int i = 0; i < serializeds.Count;)
            {
                if (!serializeds[i].IsWidgeted(comp))
                {
                    serializeds.RemoveAt(i);
                }
                else ++i;
            }
            if (serializeds.Count == 1) serializeds[0].RemoveWidget(comp);
            else
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < serializeds.Count; ++i)
                {
                    string name = serializeds[i].name;
                    if (serializeds[i].transform.parent != null) name = serializeds[i].transform.parent + "->" + name;
                    menu.AddItem(new GUIContent(name), false, (object call) =>
                    {
                        ((AComSerialized)call).RemoveWidget(comp);
                    }, serializeds[i]);
                }
                menu.DropDown(new Rect(GetCurrentEventPosition(), new Vector2(200, 200)));
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AComSerialized),true)]
    [CanEditMultipleObjects]
    public class AComSerializedEditor : Editor
    {
        //------------------------------------------------------
        static System.Type[] BaseWidgetTypes = new System.Type[]
        {
            typeof(GameObject),
            typeof(MonoBehaviour),
            typeof(RectTransform),
            typeof(Transform),
            typeof(ParticleSystem),
            typeof(ParticleSystemRenderer),
             typeof(UnityEngine.Playables.PlayableDirector),
        };
        protected Dictionary<int, System.Type> m_vTypes = new Dictionary<int, Type>();
        protected List<string> m_vTypeDisplay = new List<string>();
        protected HashSet<UnityEngine.Object> m_vSets = new HashSet<UnityEngine.Object>();
        protected bool m_bExpand = false;
        protected int m_nAddType = 0;
        void CheckTypes()
        {
            System.Type[] types = GetOtherTypes();
            int cnt = BaseWidgetTypes.Length;
            if (types != null) cnt += types.Length;
            if (m_vTypes.Count < cnt)
            {
                m_vTypes.Clear();
                m_vTypeDisplay.Clear();
                for (int i = 0; i < BaseWidgetTypes.Length; ++i)
                {
                    int hashCode = BaseWidgetTypes[i].GetHashCode();
                    if (m_vTypes.ContainsKey(hashCode)) continue;
                    m_vTypes.Add(hashCode, BaseWidgetTypes[i]);
                    string name = BaseWidgetTypes[i].ToString().Replace(".", "/");
                    m_vTypeDisplay.Add(name);
                }
                if(types!=null)
                {
                    for(int i = 0; i < types.Length; ++i)
                    {
                        int hashCode = types[i].GetHashCode();
                        if (m_vTypes.ContainsKey(hashCode)) continue;
                        m_vTypes.Add(hashCode, types[i]);
                        string name = types[i].ToString().Replace(".", "/");
                        m_vTypeDisplay.Add(name);
                    }
                }

                Assembly assembly = null;
                foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ass.GetName().Name == "MainScripts")
                    {
                        assembly = ass;
                        Type[] assTypes = assembly.GetTypes();
                        for (int i = 0; i < assTypes.Length; ++i)
                        {
                            Type tp = assTypes[i];
                            if (tp.IsDefined(typeof(ComSerializedExportAttribute), true))
                            {
                                if (m_vTypes.ContainsKey(tp.GetHashCode())) continue;
                                m_vTypes.Add(tp.GetHashCode(), tp);
                                string name = tp.ToString().Replace(".", "/");
                                m_vTypeDisplay.Add(name);
                            }
                        }
                        break;
                    }
                }
            }
        }
        protected virtual System.Type[] GetOtherTypes()
        {
            return null;
        }
        protected virtual void OnInnerInspectorGUI()
        {

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AComSerialized agb = target as AComSerialized;

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
                        if (GUILayout.Button("复制局部代码"))
                        {
                            Component widget = agb.Widgets[i].widget;
                            string typeString = widget.GetType().ToString();
                            GUIUtility.systemCopyBuffer = typeString + " " + char.ToLower(agb.Widgets[i].widget.name[0]) + agb.Widgets[i].widget.name.Substring(1, agb.Widgets[i].widget.name.Length - 1) + " = ui.GetWidget<" + typeString + ">(\"" + agb.Widgets[i].widget.name + "\");";
                        }
                        if (GUILayout.Button("复制全局代码"))
                        {
                            Component widget = agb.Widgets[i].widget;
                            string typeString = widget.GetType().ToString();
                            GUIUtility.systemCopyBuffer = "m_" + agb.Widgets[i].widget.name + " = ui.GetWidget<" + typeString + ">(\"" + agb.Widgets[i].widget.name + "\");";
                        }
                        agb.Widgets[i].widget = EditorGUILayout.ObjectField(agb.Widgets[i].widget, type, true, GUILayout.MinWidth(180)) as Component;
                        agb.Widgets[i].fastName = EditorGUILayout.TextField(agb.Widgets[i].fastName, GUILayout.MinWidth(60));
                        poip = EditorGUILayout.Popup(poip, m_vTypeDisplay.ToArray());
                        if (poip >= 0 && poip < m_vTypes.Count)
                            agb.Widgets[i].assignType = m_vTypes.ElementAt(poip).Key;
                        if (agb.Widgets[i].assignType != preType)
                            m_nAddType = agb.Widgets[i].assignType;
                        if (GUILayout.Button("删除"))
                        {
                            List<AComSerialized.Widget> vData = new List<AComSerialized.Widget>(agb.Widgets);
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
                    List<AComSerialized.Widget> vData = (agb.Widgets != null) ? new List<AComSerialized.Widget>(agb.Widgets) : new List<AComSerialized.Widget>();
                    vData.Add(new AComSerialized.Widget() { assignType = m_nAddType });
                    agb.Widgets = vData.ToArray();
                }
                EditorGUI.indentLevel--;
            }

            OnInnerInspectorGUI();
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
