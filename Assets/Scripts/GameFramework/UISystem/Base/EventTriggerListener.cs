/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	EventTriggerListener
作    者:	HappLI
描    述:	UI 公共监听
*********************************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework.Core;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
namespace Framework.UI
{
    [Framework.Plugin.AT.ATEvent("AT事件")]
    public enum EUIEventType
    {
        [Framework.Plugin.AT.ATEvent("ui-点击")] onClick = 1,
        [Framework.Plugin.AT.ATEvent("ui-按下")] onDown = 2,
        [Framework.Plugin.AT.ATEvent("ui-鼠标进入")] onEnter = 3,
        [Framework.Plugin.AT.ATEvent("ui-鼠标退出")] onExit = 4,
        [Framework.Plugin.AT.ATEvent("ui-鼠标弹起")] onUp = 5,
        [Framework.Plugin.AT.ATEvent("ui-选中")] onSelect = 6,
        [Framework.Plugin.AT.ATEvent("ui-选中更新")] onUpdateSelect = 7,
        [Framework.Plugin.AT.ATEvent("ui-拖拽")] onDrag = 8,
        [Framework.Plugin.AT.ATEvent("ui-拖拽释放")] onDrop = 9,
        [Framework.Plugin.AT.ATEvent("ui-取消选中")] onDeselect = 10,
        [Framework.Plugin.AT.ATEvent("ui-列表滚动")] onScroll = 11,
        [Framework.Plugin.AT.ATEvent("ui-开始拖拽")] onBeginDrag = 12,
        [Framework.Plugin.AT.ATEvent("ui-结束拖拽")] onEndDrag = 13,
        [Framework.Plugin.AT.ATEvent("ui-提交")] onSubmit = 14,
        [Framework.Plugin.AT.ATEvent("ui-取消")] onCancel = 15,
        [Framework.Plugin.AT.ATEvent("ui-打开界面", "Framework.UI.EUIType")] OnShowUI = 16,
        [Framework.Plugin.AT.ATEvent("ui-关闭界面", "Framework.UI.EUIType")] OnHideUI = 17,
        [Framework.Plugin.AT.ATEvent("ui-列表项更新")] OnListItem = 18,
        [Framework.Plugin.AT.ATEvent("ui-打开界面结束", "Framework.UI.EUIType")] OnShowUIEnd = 19,
        [Framework.Plugin.AT.ATEvent("ui-移动")] onMove = 20,
    }
    //------------------------------------------------------
    public enum EParamArgvFlag
    {
        String = 0,
        Int,
        Float,
        Obj,
    }
    //------------------------------------------------------
    [System.Serializable]
    public struct ATEventParam
    {
        public EUIEventType type;
        public string eventName;
    }
    //------------------------------------------------------
    [System.Serializable]
    public struct UIParamArgv
    {
        public MonoBehaviour listBehavour;
        public GameObject agentTrigger;
        public byte nFlag;
        public string strParam;
        public int intParam;
        public float floatParam;
        public UnityEngine.Object objParam;

        public bool IsFlag(EParamArgvFlag flag)
        {
            return (nFlag & (1 << (int)flag)) != 0;
        }

        public void SetFlag(EParamArgvFlag flag, bool bEnable)
        {
            if (bEnable) nFlag |= (byte)(1 << (int)flag);
            else nFlag &= (byte)(~((1 << (int)flag)));
        }
    }
    //------------------------------------------------------
    public struct UIRuntimeParamArgvs : IUserData
    {
        public IUserData param1;
        public IUserData param2;
        public IUserData param3;
        public IUserData param4;
        public void Destroy()
        {
            if (param1 != null) param1.Destroy();
            if (param2 != null) param2.Destroy();
            if (param3 != null) param3.Destroy();
            if (param4 != null) param4.Destroy();
        }
        public void FillParam(UIParamArgv paramArgvs)
        {
            if (paramArgvs.IsFlag(EParamArgvFlag.String) && !string.IsNullOrEmpty(paramArgvs.strParam))
            {
                if (param1 == null) param1 = new VariableString() { strValue = paramArgvs.strParam };
                else if (param1 == null) param2 = new VariableString() { strValue = paramArgvs.strParam };
                else if (param3 == null) param3 = new VariableString() { strValue = paramArgvs.strParam };
                else if (param4 == null) param4 = new VariableString() { strValue = paramArgvs.strParam };
            }
            if (paramArgvs.IsFlag(EParamArgvFlag.Int) && paramArgvs.intParam != 0)
            {
                if (param1 == null) param1 = new Variable1() { intVal = paramArgvs.intParam };
                else if (param1 == null) param2 = new Variable1() { intVal = paramArgvs.intParam };
                else if (param3 == null) param3 = new Variable1() { intVal = paramArgvs.intParam };
                else if (param4 == null) param4 = new Variable1() { intVal = paramArgvs.intParam };
            }
            if (paramArgvs.IsFlag(EParamArgvFlag.Obj) && paramArgvs.objParam != null)
            {
                if (param1 == null) param1 = new VariableObj() { pGO = paramArgvs.objParam };
                else if (param1 == null) param2 = new VariableObj() { pGO = paramArgvs.objParam };
                else if (param3 == null) param3 = new VariableObj() { pGO = paramArgvs.objParam };
                else if (param4 == null) param4 = new VariableObj() { pGO = paramArgvs.objParam };
            }
            if (paramArgvs.IsFlag(EParamArgvFlag.Float) && Mathf.Abs(paramArgvs.floatParam) > 0.01f)
            {
                if (param1 == null) param1 = new Variable1() { floatVal = paramArgvs.floatParam };
                else if (param1 == null) param2 = new Variable1() { floatVal = paramArgvs.floatParam };
                else if (param3 == null) param3 = new Variable1() { floatVal = paramArgvs.floatParam };
                else if (param4 == null) param4 = new Variable1() { floatVal = paramArgvs.floatParam };
            }
        }
        //------------------------------------------------------
        public bool GetInt(out int outValue)
        {
            outValue = 0;
            if (param1 != null)
            {
                if (param1 is Variable1)
                {
                    outValue = ((Variable1)param1).intVal;
                    return true;
                }
            }
            if (param2 != null)
            {
                if (param2 is Variable1)
                {
                    outValue = ((Variable1)param2).intVal;
                    return true;
                }
            }
            if (param3 != null)
            {
                if (param3 is Variable1)
                {
                    outValue = ((Variable1)param3).intVal;
                    return true;
                }
            }
            if (param4 != null)
            {
                if (param3 is Variable1)
                {
                    outValue = ((Variable1)param3).intVal;
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool GetFloat(out float outValue)
        {
            outValue = 0;
            if (param1 != null)
            {
                if (param1 is Variable1)
                {
                    outValue = ((Variable1)param1).floatVal;
                    return true;
                }
            }
            if (param2 != null)
            {
                if (param2 is Variable1)
                {
                    outValue = ((Variable1)param2).floatVal;
                    return true;
                }
            }
            if (param3 != null)
            {
                if (param3 is Variable1)
                {
                    outValue = ((Variable1)param3).floatVal;
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool GetString(out string outValue)
        {
            outValue = null;
            if (param1 != null)
            {
                if (param1 is VariableString)
                {
                    outValue = ((VariableString)param1).strValue;
                    return !string.IsNullOrEmpty(outValue);
                }
            }
            if (param2 != null)
            {
                if (param2 is VariableString)
                {
                    outValue = ((VariableString)param2).strValue;
                    return !string.IsNullOrEmpty(outValue);
                }
            }
            if (param3 != null)
            {
                if (param3 is VariableString)
                {
                    outValue = ((VariableString)param3).strValue;
                    return !string.IsNullOrEmpty(outValue);
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool GetObj(out UnityEngine.Object outValue)
        {
            outValue = null;
            if (param1 != null)
            {
                if (param1 is VariableObj)
                {
                    outValue = ((VariableObj)param1).pGO;
                    return outValue != null;
                }
                if (param1 is VariableGO)
                {
                    outValue = ((VariableGO)param1).pGO;
                    return outValue != null;
                }
            }
            if (param2 != null)
            {
                if (param2 is VariableObj)
                {
                    outValue = ((VariableObj)param2).pGO;
                    return outValue != null;
                }
                if (param2 is VariableGO)
                {
                    outValue = ((VariableGO)param2).pGO;
                    return outValue != null;
                }
            }
            if (param3 != null)
            {
                if (param3 is VariableObj)
                {
                    outValue = ((VariableObj)param3).pGO;
                    return outValue != null;
                }
                if (param3 is VariableGO)
                {
                    outValue = ((VariableGO)param3).pGO;
                    return outValue != null;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool GetUserData(out IUserData outValue)
        {
            outValue = null;
            if (param1 != null)
            {
                outValue = param1;
                return true;
            }
            if (param2 != null)
            {
                outValue = param2;
                return true;
            }
            if (param3 != null)
            {
                outValue = param3;
                return true;
            }
            return false;
        }
    }
    [UIWidgetExport]
    public class EventTriggerListener : EventTrigger
    {
        //按钮缩放
        public enum ButtonAnimation
        {
            None,
            Scale,
            ScaleHightlight,
            /// <summary>
            /// 缩放并且替换图标
            /// </summary>
            ScaleAndPicture,
        }

        public ButtonAnimation animationType = ButtonAnimation.Scale;
        public float scaleDelta = 0.9f;
        public float scaleDeltaTime = 0.05f;
        public Transform scalerTarget = null;
        public Graphic hightGraphic;
        public Color highlightColor = new Color(0.63f, 0.63f, 0.63f, 1);
        public Sprite hightlightSprite;

        Vector3 m_originalScale = Vector3.one;
        Color m_originalColor = Color.white;

        //是否播放默认点击音效
        public bool isPlayCommonClickSound = true;
        public int PlayClickSoundId = 0;

        UIPenetrate m_Parentrate = null;
#if USE_GUIDESYSTEM
        [SerializeField]
        private Framework.Plugin.Guide.GuideGuid m_GuideGuid = null;
#endif
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

        public ATEventParam atEventParam = new ATEventParam();
        public UIParamArgv paramArgvs = new UIParamArgv();
        [NonSerialized]
        public UIRuntimeParamArgvs param = new UIRuntimeParamArgvs();

        public int listIndex
        {
            get { return m_nListIndex; }
            set { m_nListIndex = value; }
        }
        public IUserData param1
        {
            get { return param.param1; }
            set { param.param1 = value; }
        }

        public IUserData param2
        {
            get { return param.param2; }
            set { param.param2 = value; }
        }
        public IUserData param3
        {
            get { return param.param3; }
            set { param.param3 = value; }
        }

        public IUserData param4
        {
            get { return param.param4; }
            set { param.param4 = value; }
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
#if USE_GUIDESYSTEM
        //------------------------------------------------------
        public void SetGuideGuid(Framework.Plugin.Guide.GuideGuid guideGuid)
        {
            m_GuideGuid = guideGuid;
        }
#endif
        //------------------------------------------------------
        public void SetParentrate(UIPenetrate parentrate)
        {
            m_Parentrate = parentrate;
        }
        //------------------------------------------------------
        void CheckParam()
        {
            param.FillParam(paramArgvs);
        }
        //------------------------------------------------------
        bool OnUIWidgetTrigger(BaseEventData eventData, EUIEventType eventType)
        {
            if (AFramework.mainFramework == null) return false;

#if USE_GUIDESYSTEM
            int guide =  m_GuideGuid ? m_GuideGuid.Guid : 0;
#else
            int guide = 0;
#endif
            return AFramework.mainFramework.OnUIWidgetTrigger(this, eventData, eventType, guide, m_nListIndex, param1, param1, param2, param3, param4);
        }
        //------------------------------------------------------
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData != null && eventData.dragging && !CanFire(eventData.position - eventData.pressPosition))
                return;

            if (OnUIWidgetTrigger(eventData, EUIEventType.onClick))
                return;

            if (onClickEvent != null) onClickEvent(GetTriggerObject(), eventData);
            OnATEvent(GetTriggerObject(), EUIEventType.onClick, param);

            if (onClick != null) onClick(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnPointerClick(eventData);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            DoPressAction();

            if (OnUIWidgetTrigger(eventData, EUIEventType.onDown))
                return;
            if (onDownEvent != null) onDownEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            if (onDown != null) onDown(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnPointerDown(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onEnter))
                return;
            if (onEnterEvent != null) onEnterEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onEnter, param);
            if (onEnter != null) onEnter(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnPointerEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onExit))
                return;
            if (onExitEvent != null) onExitEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onExit, param);
            if (onExit != null) onExit(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnPointerExit(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            DoUpAction();
            if (OnUIWidgetTrigger(eventData, EUIEventType.onUp))
                return;
            if (onUpEvent != null) onUpEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onUp, param);
            if (onUp != null) onUp(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnPointerUp(eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onSelect))
                return;
            if (onSelectEvent != null) onSelectEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onSelect, param);
            if (onSelect != null) onSelect(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnSelect(eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onUpdateSelect))
                return;
            if (onUpdateSelectEvent != null) onUpdateSelectEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onUpdateSelect, param);
            if (onUpdateSelect != null) onUpdateSelect(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnUpdateSelected(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onDrag))
                return;
            if (onDragEvent != null) onDragEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onDrag, param);
            if (onDrag != null) onDrag(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnDrag(eventData);
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onDrop))
                return;
            if (onDropEvent != null) onDropEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onDrop, param);
            if (onDrop != null) onDrop(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnDrop(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onDeselect))
                return;
            if (onDeselectEvent != null) onDeselectEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onDeselect, param);
            if (onDeselect != null) onDeselect(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnDeselect(eventData);
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onScroll))
                return;
            if (onScrollEvent != null) onScrollEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onScroll, param);
            if (onScroll != null) onScroll(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnScroll(eventData);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onMove))
                return;
            if (onMoveEvent != null) onMoveEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            if (onMove != null) onMove(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnMove(eventData);
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onInitializePotentialDragEvent != null) onInitializePotentialDragEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            if (onInitializePotentialDrag != null) onInitializePotentialDrag(GetTriggerObject(), param.param1, param.param2, param3, param4);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onBeginDrag))
                return;
            if (onBeginDragEvent != null) onBeginDragEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onBeginDrag, param);
            if (onBeginDrag != null) onBeginDrag(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onEndDrag))
                return;
            if (onEndDragEvent != null) onEndDragEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onEndDrag, param);
            if (onEndDrag != null) onEndDrag(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnEndDrag(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onSubmit))
                return;
            if (onSubmitEvent != null) onSubmitEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onSubmit, param);
            if (onSubmit != null) onSubmit(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnSubmit(eventData);
        }

        public override void OnCancel(BaseEventData eventData)
        {
            if (OnUIWidgetTrigger(eventData, EUIEventType.onCancel))
                return;
            if (onCancelEvent != null) onCancelEvent(GetTriggerObject(), eventData, param.param1, param.param2, param3, param4);
            OnATEvent(GetTriggerObject(), EUIEventType.onCancel, param);
            if (onCancel != null) onCancel(GetTriggerObject(), param.param1, param.param2, param3, param4);
            if (m_Parentrate != null) m_Parentrate.OnCancel(eventData);
        }

        public static EventTriggerListener Get(GameObject go)
        {
            if (go == null) return null;
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }


        public void RefreshScale()
        {
            m_originalScale = GetScaler().localScale;
        }

        public void ReplaceScale(Vector3 scale)
        {
            m_originalScale = scale;
        }

        public void ReplaceHightColor(Color color)
        {
            m_originalColor = color;
        }

        private void Awake()
        {
            CheckParam();
            m_originalScale = GetScaler().localScale;
            if (hightGraphic)
            {
                m_originalColor = hightGraphic.color;
            }
            m_nListIndex = -1;
#if USE_GUIDESYSTEM
            if (m_GuideGuid != null && m_GuideGuid.ScrollList!=null)
            {
                //这边为了能获取到正确得index,要在 scroll 得生成格子上添加EventTriggerListener进行点击触发,而不是在子物体底下加,不然找不到正确index
                m_nListIndex = m_GuideGuid.ScrollList.GetIndexByItem(this.gameObject);//从0开始得索引,
            }
#endif
        }

        private void OnDisable()
        {
            if (animationType == ButtonAnimation.ScaleAndPicture && hightGraphic is Image)
            {
                (hightGraphic as Image).overrideSprite = null;
            }
        }

#region 按钮点击表现
        //------------------------------------------------------
        public virtual Transform GetScaler()
        {
            if (scalerTarget) return scalerTarget;
            return this.transform;
        }
        //------------------------------------------------------
        string GetSymboleName()
        {
            return GetScaler().name;
        }
        //------------------------------------------------------
        public void DoPressAction()
        {
            if(animationType == ButtonAnimation.None)return;
            if (UICommonScaler.IsIngoreSet(GetSymboleName())) return;
               
            UICommonScaler.AddScaler(this.GetScaler(), m_originalScale, 1, scaleDelta, scaleDeltaTime);
            if (animationType == ButtonAnimation.ScaleHightlight && hightGraphic)
            {
                UICommonScaler.AddHightLight(this.hightGraphic, m_originalColor, m_originalColor, highlightColor, scaleDeltaTime);
            }
            if (animationType == ButtonAnimation.ScaleAndPicture && hightGraphic is Image)
            {
                (hightGraphic as Image).overrideSprite = hightlightSprite;
            }
        }
        //------------------------------------------------------
        public void DoUpAction()
        {
            if (animationType == ButtonAnimation.None) return;
            if (UICommonScaler.IsIngoreSet(GetSymboleName())) return;

            UICommonScaler.AddScaler(this.GetScaler(), m_originalScale, scaleDelta, 1, scaleDeltaTime);
            if (animationType == ButtonAnimation.ScaleHightlight && hightGraphic)
            {
                UICommonScaler.AddHightLight(this.hightGraphic, m_originalColor, highlightColor, m_originalColor, scaleDeltaTime);
            }
            if (animationType == ButtonAnimation.ScaleAndPicture && hightGraphic is Image)
            {
                (hightGraphic as Image).overrideSprite = null;
            }
        }
        //------------------------------------------------------
        bool OnATEvent(GameObject pTrans, EUIEventType evtType, UIRuntimeParamArgvs param)
        {
            if (evtType == atEventParam.type)
            {
                if (!string.IsNullOrEmpty(atEventParam.eventName))
                    Framework.Plugin.AT.AgentTreeManager.getInstance().ExecuteEvent((ushort)evtType, atEventParam.eventName, param);
            }
            int index = IndexListItem();
            if (index >= 0)
                return Framework.Plugin.AT.AgentTreeManager.getInstance().ExecuteEvent((ushort)evtType, pTrans, new Variable1() { intVal = index });
            else
                return Framework.Plugin.AT.AgentTreeManager.getInstance().ExecuteEvent((ushort)evtType, pTrans, param);
        }
        //------------------------------------------------------
        public int IndexListItem()
        {
            if (paramArgvs.listBehavour == null) return -1;
            if (paramArgvs.listBehavour is ListView)
            {
                ListView pGrid = paramArgvs.listBehavour as ListView;
                return pGrid.GetIndexByItem(this.gameObject);
            }
            return -1;
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            UICommonScaler.OnDestroy(this.transform);
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

#if UE_GUIDESYSTEM
                if(fields[i].Name.CompareTo("m_GuideGuid") == 0)
                {
                    var guidValue = fields[i].GetValue(evt);
                    if(guidValue == null)
                    {
                        fields[i].SetValue(evt, evt.gameObject.GetComponent<Framework.Plugin.Guide.GuideGuid>());
                    }
                }s
#endif
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
            }