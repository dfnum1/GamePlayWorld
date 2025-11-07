#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UISerializedEditor
作    者:	HappLI
描    述:	UI 序列化对象容器，用于界面绑定操作对象 编辑操作
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Reflection;

namespace Framework.UI
{
    [CustomEditor(typeof(UISerialized), true)]
    public class UISerializedEditor : Core.AComSerializedEditor
    {
        static System.Type[] WidgetTypes = new System.Type[]
        {
            //typeof(Joystick),
            //typeof(BubbleJoystick),
            //typeof(Core.RenderSortByDistance),
            typeof(SpriteRenderer),
            typeof(DG.Tweening.DOTweenAnimation),
            typeof(UISerialized),
            typeof(MonoBehaviour),
            typeof(RectTransform),
            typeof(Transform),
            typeof(CanvasRenderer),
            typeof(UIBehaviour),
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
        public bool bExpandEles = false;
        public bool bExpandEvent = false;
        public bool[] bExpandEventItem = new bool[(int)UIEventType.Count];
        public bool bExpandHide = false;
        public bool bExpandEnable = false;
        public bool bExpandDisable = false;
        public bool bExpandCreate = false;
        public bool bExpandClose = false;


        List<string> m_vAnimationPops = new List<string>();
        List<int> m_vAnimationIDPops = new List<int>();

        List<string> m_vTweenGroupPops = new List<string>();
        List<int> m_vTweenGroupIDPops = new List<int>();

        float m_fTestAnimClipTime = 0;
        protected override System.Type[] GetOtherTypes()
        {
            return WidgetTypes;
        }
        private void OnEnable()
        {
            m_vAnimationPops.Clear();
            m_vAnimationIDPops.Clear();
            m_vAnimationIDPops.Add(-1);
            m_vAnimationPops.Add("None");
            UIAnimatorFactory.getInstance().Init(UI.ED.UIAnimatorEditor.ASSET_FILE, false);
            foreach (var db in UIAnimatorFactory.getInstance().Groups)
            {
                m_vAnimationIDPops.Add(db.Value.nID);
                m_vAnimationPops.Add(db.Value.desc + "[" + db.Value.nID + "]");
            }
        }

        public void DrawEventSinglePart(UISerialized uis, int i)
        {
            if (uis.UIEventDatas[i] != null)
                uis.UIEventDatas[i].tweenGroup = EditorGUILayout.ObjectField("TweenGroup", uis.UIEventDatas[i].tweenGroup, typeof(RtgTween.TweenerGroup), true) as RtgTween.TweenerGroup;
            if (uis.UIEventDatas[i].tweenGroup != null)
            {
                m_vTweenGroupIDPops.Clear();
                m_vTweenGroupPops.Clear();
                for (int j = 0; j < uis.UIEventDatas[i].tweenGroup.Groups.Count; ++j)
                {
                    m_vTweenGroupIDPops.Add(j);
                    m_vTweenGroupPops.Add(uis.UIEventDatas[i].tweenGroup.Groups[j].Name + "[" + j + "]");
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("动画ID");

                int index = EditorGUILayout.Popup(m_vTweenGroupIDPops.IndexOf(uis.UIEventDatas[i].animationID), m_vTweenGroupPops.ToArray());
                if (index >= 0 && index < m_vTweenGroupIDPops.Count)
                {
                    uis.UIEventDatas[i].animationID = m_vTweenGroupIDPops[index];
                }
                GUILayout.EndHorizontal();

                if (uis.UIEventDatas[i].tweenGroup != null)
                {
                    uis.UIEventDatas[i].Animation = null;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("动画ID");

                int index = EditorGUILayout.Popup(m_vAnimationIDPops.IndexOf(uis.UIEventDatas[i].animationID), m_vAnimationPops.ToArray());
                if (index >= 0 && index < m_vAnimationIDPops.Count)
                {
                    uis.UIEventDatas[i].animationID = m_vAnimationIDPops[index];
                }
                GUILayout.EndHorizontal();

                if (uis.UIEventDatas[i].animationID <= 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("界面动画");
                    if (uis.UIEventDatas[i] != null)
                    {
                        uis.UIEventDatas[i].Animation = EditorGUILayout.ObjectField(uis.UIEventDatas[i].Animation, typeof(AnimationClip), true) as AnimationClip;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("反向播放");
            if (uis.UIEventDatas[i] != null)
                uis.UIEventDatas[i].bReverse = EditorGUILayout.Toggle(uis.UIEventDatas[i].bReverse);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("速度");
            if (uis.UIEventDatas[i] != null)
                uis.UIEventDatas[i].speedScale = EditorGUILayout.FloatField(uis.UIEventDatas[i].speedScale);
            GUILayout.EndHorizontal();

#if !USE_FMOD
            GUILayout.BeginHorizontal();
            GUILayout.Label("界面声音");
            if (uis.UIEventDatas[i] != null)
                uis.UIEventDatas[i].Audio = EditorGUILayout.ObjectField(uis.UIEventDatas[i].Audio, typeof(AudioClip), true) as AudioClip;
            GUILayout.EndHorizontal();
#endif

            GUILayout.BeginHorizontal();
            if (uis.UIEventDatas[i] != null)
            {
                SerializedProperty eventReference = null;
                SerializedProperty uiEventDatasPop = serializedObject.FindProperty("UIEventDatas");
                if (uiEventDatasPop != null)
                {
                    SerializedProperty elementAt = uiEventDatasPop.GetArrayElementAtIndex(i);
                    if (elementAt != null)
                    {
#if USE_FMOD					
                        eventReference = elementAt.FindPropertyRelative("fmodEvent");
#endif
                    }
                }
                if (eventReference != null)
                    EditorGUILayout.PropertyField(eventReference, new GUIContent("FMOD音效"));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("动画挂载节点");
            if (uis.UIEventDatas[i] != null)
                uis.UIEventDatas[i].ApplyRoot = EditorGUILayout.ObjectField(uis.UIEventDatas[i].ApplyRoot, typeof(Transform), true) as Transform;
            GUILayout.EndHorizontal();

            if (uis.UIEventDatas[i].Animation != null)
            {
                EditorGUI.BeginChangeCheck();
                m_fTestAnimClipTime = EditorGUILayout.Slider("预览", m_fTestAnimClipTime, 0, uis.UIEventDatas[i].Animation.length);
                if (EditorGUI.EndChangeCheck())
                    uis.UIEventDatas[i].Animation.SampleAnimation(uis.UIEventDatas[i].ApplyRoot ? uis.UIEventDatas[i].ApplyRoot.gameObject : uis.gameObject, m_fTestAnimClipTime);
            }
            //HandleUtilityWrapper.DrawPropertyByFieldName(uis.UIEventDatas[i], "EventId");
        }

        protected override void OnInnerInspectorGUI()
        {
            DrawTarget(target);
        }
        //------------------------------------------------------
        protected void DrawTarget(UnityEngine.Object target)
        {
            UISerialized uis = target as UISerialized;

            Color color = GUI.color;
            bExpandEles = EditorGUILayout.Foldout(bExpandEles, "引用对象");
            if (bExpandEles)
            {
                EditorGUI.indentLevel++;
                m_vSets.Clear();
                if (uis.Elements != null)
                {
                    for (int i = 0; i < uis.Elements.Length; ++i)
                    {
                        if (m_vSets.Contains(uis.Elements[i]))
                        {
                            GUI.color = Color.red;
                        }
                        else
                        {
                            GUI.color = color;

                            if (uis.Elements[i] != null)
                                m_vSets.Add(uis.Elements[i]);
                        }

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("复制名字"))
                        {
                            GUIUtility.systemCopyBuffer = uis.Elements[i].name;
                        }
                        if (GUILayout.Button("复制代码"))
                        {
                            GUIUtility.systemCopyBuffer = "GameObject " + char.ToLower(uis.Elements[i].name[0]) + uis.Elements[i].name.Substring(1, uis.Elements[i].name.Length - 1) + " = ui.Find(\"" + uis.Elements[i].name + "\");";
                        }
                        uis.Elements[i] = EditorGUILayout.ObjectField(uis.Elements[i], typeof(UnityEngine.Object), true) as UnityEngine.Object;
                        if (GUILayout.Button("删除"))
                        {
                            List<UnityEngine.Object> vData = new List<UnityEngine.Object>(uis.Elements);
                            vData.RemoveAt(i);
                            uis.Elements = vData.ToArray();
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUI.color = color;
                if (GUILayout.Button("新建"))
                {
                    List<UnityEngine.Object> vData = (uis.Elements != null) ? new List<UnityEngine.Object>(uis.Elements) : new List<UnityEngine.Object>();
                    vData.Add(null);
                    uis.Elements = vData.ToArray();
                }
                EditorGUI.indentLevel--;
            }

            if (uis.UIEventDatas == null || uis.UIEventDatas.Length != (int)UIEventType.Count)
            {
                if (uis.UIEventDatas != null)
                {
                    List<UIEventData> events = new List<UIEventData>(uis.UIEventDatas);
                    if (events.Count > (int)UIEventType.Count)
                        events.RemoveRange((int)UIEventType.Count, events.Count - (int)UIEventType.Count);
                    for (int i = events.Count; i < (int)UIEventType.Count; ++i)
                    {
                        events.Add(null);
                    }
                    uis.UIEventDatas = events.ToArray();
                }
                else
                {
                    uis.UIEventDatas = new UIEventData[(int)UIEventType.Count];
                }
            }

            bExpandEvent = EditorGUILayout.Foldout(bExpandEvent, "事件");
            if (bExpandEvent)
            {
                EditorGUI.indentLevel++;
                UIEventType curType;
                for (int i = 0; i < (int)UIEventType.Count; i++)
                {
                    curType = (UIEventType)i;
                    bExpandEventItem[i] = EditorGUILayout.Foldout(bExpandEventItem[i], Framework.ED.EditorUtil.GetDisplayName(curType));
                    if (bExpandEventItem[i])
                    {
                        DrawEventSinglePart(uis, i);
                    }
                }
                EditorGUI.indentLevel--;
            }

            GUI.color = color;
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsDefined(typeof(Base.CustomCUIAttribute)))
                    continue;

                if (fields[i].Name.CompareTo("Elements") == 0) continue;
                if (fields[i].Name.CompareTo("Widgets") == 0) continue;
                if (fields[i].Name.CompareTo("UIEventDatas") == 0) continue;
                if (fields[i].Name.CompareTo("ATData") == 0) continue;

                if (fields[i].Name.CompareTo("OnUIShow") == 0) continue;
                if (fields[i].Name.CompareTo("OnUIHide") == 0) continue;
                if (fields[i].Name.CompareTo("OnUIEnable") == 0) continue;
                if (fields[i].Name.CompareTo("OnUIDisable") == 0) continue;
                if (fields[i].Name.CompareTo("OnUICreate") == 0) continue;
                if (fields[i].Name.CompareTo("OnUIDestory") == 0) continue;

                if (fields[i].IsNotSerialized) continue;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fields[i].Name), true);
            }
            UserInterface user = uis as UserInterface;
            if (user != null)
            {
                Framework.ED.InspectorDrawUtil.DrawPropertyByFieldName(target, "ATData");
            }
        }
    }
}
#endif