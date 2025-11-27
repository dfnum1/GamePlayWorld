/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	EventTriggerListener
作    者:	HappLI
描    述:	UI 公共监听
*********************************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
namespace Framework.Guide
{
    //------------------------------------------------------
    [System.Serializable]
    public struct UIParamArgv
    {
        public MonoBehaviour listBehavour;
        public GameObject agentTrigger;
    }
    public class EventTriggerListener : EventTrigger
    {
        public UIParamArgv paramArgvs;
        UIPenetrate m_Parentrate = null;
        [SerializeField]
        private Framework.Guide.GuideGuid m_GuideGuid = null;
        private int m_nListIndex = -1;

        public delegate void VoidDelegate(GameObject go, params IUserData[] param);
        [NonSerialized]public VoidDelegate onClick;
        [NonSerialized]public VoidDelegate onDown;
        [NonSerialized]public VoidDelegate onEnter;
        [NonSerialized]public VoidDelegate onExit;
        [NonSerialized]public VoidDelegate onUp;
        [NonSerialized]public VoidDelegate onSelect;
        [NonSerialized]public VoidDelegate onUpdateSelect;
        [NonSerialized]public VoidDelegate onDrag;
        [NonSerialized]public VoidDelegate onDrop;
        [NonSerialized]public VoidDelegate onDeselect;
        [NonSerialized]public VoidDelegate onScroll;
        [NonSerialized]public VoidDelegate onMove;
        [NonSerialized]public VoidDelegate onInitializePotentialDrag;
        [NonSerialized]public VoidDelegate onBeginDrag;
        [NonSerialized]public VoidDelegate onEndDrag;
        [NonSerialized]public VoidDelegate onSubmit;
        [NonSerialized]public VoidDelegate onCancel;
		
		public delegate void VoidEventDelegate(GameObject go, BaseEventData evtData, params IUserData[] param);
        [NonSerialized]public VoidEventDelegate onClickEvent;
        [NonSerialized]public VoidEventDelegate onDownEvent;
        [NonSerialized]public VoidEventDelegate onEnterEvent;
        [NonSerialized]public VoidEventDelegate onExitEvent;
        [NonSerialized]public VoidEventDelegate onUpEvent;
        [NonSerialized]public VoidEventDelegate onSelectEvent;
        [NonSerialized]public VoidEventDelegate onUpdateSelectEvent;
        [NonSerialized]public VoidEventDelegate onDragEvent;
        [NonSerialized]public VoidEventDelegate onDropEvent;
        [NonSerialized]public VoidEventDelegate onDeselectEvent;
        [NonSerialized]public VoidEventDelegate onScrollEvent;
        [NonSerialized]public VoidEventDelegate onMoveEvent;
        [NonSerialized]public VoidEventDelegate onInitializePotentialDragEvent;
        [NonSerialized]public VoidEventDelegate onBeginDragEvent;
        [NonSerialized]public VoidEventDelegate onEndDragEvent;
        [NonSerialized]public VoidEventDelegate onSubmitEvent;
        [NonSerialized]public VoidEventDelegate onCancelEvent;
        
        public bool IsCommonItem = false;
        public int listIndex
        {
            get { return m_nListIndex; }
            set { m_nListIndex = value; }
        }
        bool CanFire(Vector2 Delta)
        {
            float sensitivity = 0.1f;
            if (Delta.sqrMagnitude >= sensitivity* sensitivity) return false;
            return true;
        }
        //------------------------------------------------------
        GameObject GetTriggerObject()
        {
            if (paramArgvs.agentTrigger) return paramArgvs.agentTrigger;
            return this.gameObject;
        }
        //------------------------------------------------------
        public void SetGuideGuid(Framework.Guide.GuideGuid guideGuid)
        {
            m_GuideGuid = guideGuid;
        }
        //------------------------------------------------------
        public void SetParentrate(UIPenetrate parentrate)
        {
            m_Parentrate = parentrate;
        }
        //------------------------------------------------------
        bool OnUIWidgetTrigger(BaseEventData eventData, EUIWidgetTriggerType eventType)
        {
            int guide =  m_GuideGuid ? m_GuideGuid.guid : 0;
            string tag = m_GuideGuid ? m_GuideGuid.name : null;
            if (guide != 0 && eventType != EUIWidgetTriggerType.None)
                Guide.GuideSystem.getInstance().OnUIWidgetTrigger(guide, listIndex, tag, eventType);
            return false;
        }
        //------------------------------------------------------
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData != null && eventData.dragging && !CanFire(eventData.position - eventData.pressPosition))
                return;

            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Click))
                return;

            if (onClickEvent != null) onClickEvent(GetTriggerObject(), eventData);
            if (onClick != null) onClick(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnPointerClick(eventData);
        }
        //------------------------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
            DoPressAction();

            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Down))
                return;
            if (onDownEvent != null) onDownEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onDown != null) onDown(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnPointerDown(eventData);
        }
        //------------------------------------------------------
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Enter))
                return;
            if (onEnterEvent != null) onEnterEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onEnter != null) onEnter(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnPointerEnter(eventData);
        }
        //------------------------------------------------------
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Exit))
                return;
            if (onExitEvent != null) onExitEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onExit != null) onExit(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnPointerExit(eventData);
        }
        //------------------------------------------------------
        public override void OnPointerUp(PointerEventData eventData)
        {
            DoUpAction();
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Up))
                return;
            if (onUpEvent != null) onUpEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onUp != null) onUp(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnPointerUp(eventData);
        }
        //------------------------------------------------------
        public override void OnSelect(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Select))
                return;
            if (onSelectEvent != null) onSelectEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onSelect != null) onSelect(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnSelect(eventData);
        }
        //------------------------------------------------------
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.UpdateSelect))
                return;
            if (onUpdateSelectEvent != null) onUpdateSelectEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onUpdateSelect != null) onUpdateSelect(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnUpdateSelected(eventData);
        }
        //------------------------------------------------------
        public override void OnDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Drag))
                return;
            if (onDragEvent != null) onDragEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onDrag != null) onDrag(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnDrag(eventData);
        }
        //------------------------------------------------------
        public override void OnDrop(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Drop))
                return;
            if (onDropEvent != null) onDropEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onDrop != null) onDrop(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnDrop(eventData);
        }
        //------------------------------------------------------
        public override void OnDeselect(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Deselect))
                return;
            if (onDeselectEvent != null) onDeselectEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onDeselect != null) onDeselect(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnDeselect(eventData);
        }
        //------------------------------------------------------
        public override void OnScroll(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Scroll))
                return;
            if (onScrollEvent != null) onScrollEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onScroll != null) onScroll(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnScroll(eventData);
        }
        //------------------------------------------------------
        public override void OnMove(AxisEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Move))
                return;
            if (onMoveEvent != null) onMoveEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onMove != null) onMove(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnMove(eventData);
        }
        //------------------------------------------------------
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onInitializePotentialDragEvent != null) onInitializePotentialDragEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onInitializePotentialDrag != null) onInitializePotentialDrag(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
        }
        //------------------------------------------------------
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.BeginDrag))
                return;
            if (onBeginDragEvent != null) onBeginDragEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onBeginDrag != null) onBeginDrag(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnBeginDrag(eventData);
        }
        //------------------------------------------------------
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.EndDrag))
                return;
            if (onEndDragEvent != null) onEndDragEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onEndDrag != null) onEndDrag(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnEndDrag(eventData);
        }
        //------------------------------------------------------
        public override void OnSubmit(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Submit))
                return;
            if (onSubmitEvent != null) onSubmitEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onSubmit != null) onSubmit(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnSubmit(eventData);
        }
        //------------------------------------------------------
        public override void OnCancel(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Cancel))
                return;
            if (onCancelEvent != null) onCancelEvent(GetTriggerObject(), eventData/*, param.param1, param.param2, param3, param4*/);
            if (onCancel != null) onCancel(GetTriggerObject()/*, param.param1, param.param2, param3, param4*/);
            if (m_Parentrate != null) m_Parentrate.OnCancel(eventData);
        }
        //------------------------------------------------------
        public static EventTriggerListener Get(GameObject go)
        {
            if (go == null) return null;
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }
        private void Awake()
        {
            m_nListIndex = -1;
            if (m_GuideGuid) m_nListIndex = m_GuideGuid.listIndex;
         //   if (m_GuideGuid != null && m_GuideGuid.listViewComp!=null && m_GuideGuid.listViewComp is IGuideScroll)
         //  {
         //       //这边为了能获取到正确得index,要在 scroll 得生成格子上添加EventTriggerListener进行点击触发,而不是在子物体底下加,不然找不到正确index
         //       m_nListIndex = ((IGuideScroll)m_GuideGuid.listViewComp).GetIndexByItem(this.gameObject);//从0开始得索引,
         //   }
        }
#region 按钮点击表现
        //------------------------------------------------------
        public void DoPressAction()
        {
        }
        //------------------------------------------------------
        public void DoUpAction()
        {
        }
        //------------------------------------------------------
        public int IndexListItem()
        {
            if (paramArgvs.listBehavour == null) return -1;
            if (paramArgvs.listBehavour is IGuideScroll)
            {
                IGuideScroll pGrid = paramArgvs.listBehavour as IGuideScroll;
                return pGrid.GetIndexByItem(this.gameObject);
            }
            return -1;
        }
#endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EventTriggerListener))]
    [CanEditMultipleObjects]
    public class EventTriggerListenerEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            EventTriggerListener evt = target as EventTriggerListener;
            //if (evt.btnUnLockData.listener == null)
            //    evt.btnUnLockData.listener = evt.GetComponent<Logic.UnLockListener>();
            //if (evt.paramArgvs.listBehavour == null)
            //{
            //    evt.paramArgvs.listBehavour = evt.GetComponentInParent<ListView>();
            //    if (evt.paramArgvs.listBehavour == null)
            //        evt.paramArgvs.listBehavour = evt.GetComponentInParent<GridBoxMgr>();
            //    if (evt.paramArgvs.listBehavour && evt.paramArgvs.agentTrigger == null)
            //        evt.paramArgvs.agentTrigger = evt.paramArgvs.listBehavour.gameObject;
            //}
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EventTriggerListener evt = target as EventTriggerListener;

            GUILayout.Label("参数:");
            EditorGUI.indentLevel++;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            {
                /*
                evt.paramArgvs.agentTrigger = EditorGUILayout.ObjectField("回调GO", evt.paramArgvs.agentTrigger, typeof(UnityEngine.GameObject), true) as GameObject;
                if (evt.paramArgvs.agentTrigger)
                {
                    evt.paramArgvs.listBehavour = evt.paramArgvs.agentTrigger.GetComponent<ListView>();
                }
                GUILayout.BeginHorizontal();
                evt.paramArgvs.intParam = EditorGUILayout.IntField("Int", evt.paramArgvs.intParam);
                evt.paramArgvs.SetFlag(EParamArgvFlag.Int, EditorGUILayout.Toggle(evt.paramArgvs.IsFlag(EParamArgvFlag.Int)));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                evt.paramArgvs.floatParam = EditorGUILayout.FloatField("Float", evt.paramArgvs.floatParam);
                evt.paramArgvs.SetFlag(EParamArgvFlag.Float, EditorGUILayout.Toggle(evt.paramArgvs.IsFlag(EParamArgvFlag.Float)));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                evt.paramArgvs.strParam = EditorGUILayout.TextField("String", evt.paramArgvs.strParam);
                evt.paramArgvs.SetFlag(EParamArgvFlag.String, EditorGUILayout.Toggle(evt.paramArgvs.IsFlag(EParamArgvFlag.String)));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                evt.paramArgvs.objParam = EditorGUILayout.ObjectField("GO", evt.paramArgvs.objParam, typeof(UnityEngine.Object), true);
                evt.paramArgvs.SetFlag(EParamArgvFlag.Obj, EditorGUILayout.Toggle(evt.paramArgvs.IsFlag(EParamArgvFlag.Obj)));
                GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //evt.paramArgvs.objParam = EditorGUILayout.TextField("FMODEvent", evt.paramArgvs.strParam );
                //evt.paramArgvs.SetFlag(EParamArgvFlag.Obj, EditorGUILayout.Toggle(evt.paramArgvs.IsFlag(EParamArgvFlag.Obj)));
                //GUILayout.EndHorizontal();
                */
            }
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = labelWidth;

            FieldInfo[] fields = evt.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsNotSerialized) continue;
                if (fields[i].Name.CompareTo("paramArgvs") == 0) continue;
                if(serializedObject == null || fields[i]==null) continue; 
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fields[i].Name), true);
            }

            fields = evt.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (!fields[i].IsDefined(typeof(SerializeField))) continue;
                if (serializedObject == null || fields[i] == null) continue;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fields[i].Name), true);

                if(fields[i].Name.CompareTo("m_GuideGuid") == 0)
                {
                    var guidValue = fields[i].GetValue(evt);
                    if(guidValue == null)
                    {
                        fields[i].SetValue(evt, evt.gameObject.GetComponent<Framework.Guide.GuideGuid>());
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}