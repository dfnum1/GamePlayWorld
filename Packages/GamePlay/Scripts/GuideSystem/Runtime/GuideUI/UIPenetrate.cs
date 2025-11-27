/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIPenetrate
作    者:	HappLI
描    述:	UI穿透
*********************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
namespace Framework.Guide
{
    public class UIPenetrate : EventTriggerListener
    {
        GameObject m_TriggerGo;
        public GameObject TriggerGo
        {
            get { return m_TriggerGo; }
            set
            {
                if(m_TriggerGo != value)
                {
                    m_Listener = null;
                    m_TriggerGo = value;
                    CheckTargetListen();
                }
            }
        }
        EventTriggerListener m_Listener;

        public string SearchListenName;

        //------------------------------------------------------
        void CheckTargetListen()
        {
            if (m_Listener ==null && m_TriggerGo != null)
            {
                m_Listener = m_TriggerGo.GetComponent<EventTriggerListener>();
                if (m_Listener == null)
                {
                    //加上名字判断
                    
                    if (string.IsNullOrWhiteSpace(SearchListenName))
                    {
                        m_Listener = m_TriggerGo.GetComponentInChildren<EventTriggerListener>();
                    }
                    else
                    {
                        var listeners = m_TriggerGo.GetComponentsInChildren<EventTriggerListener>();
                        foreach (var item in listeners)
                        {
                            if (item.name.Equals(SearchListenName))
                            {
                                m_Listener = item;
                                break;
                            }
                        }
                    }
                    
                }
            }
        }
        //------------------------------------------------------
        private bool bCommonListener()
        {
            return m_Listener != null && m_Listener.gameObject == m_TriggerGo;
        }
        //------------------------------------------------------
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IBeginDragHandler>(m_Listener.gameObject, eventData, ExecuteEvents.beginDragHandler))
                    return;
                if (bCommon)  return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IBeginDragHandler>(TriggerGo, eventData, ExecuteEvents.beginDragHandler);
        }
        //------------------------------------------------------
        public override void OnDrag(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IDragHandler>(m_Listener.gameObject, eventData, ExecuteEvents.dragHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IDragHandler>(TriggerGo, eventData, ExecuteEvents.dragHandler);
        }
        //------------------------------------------------------
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IEndDragHandler>(m_Listener.gameObject, eventData, ExecuteEvents.endDragHandler))
                    return;
                    if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IEndDragHandler>(TriggerGo, eventData, ExecuteEvents.endDragHandler);
        }
        //------------------------------------------------------
        public override void OnPointerClick(PointerEventData eventData)
        {
            bool bActive = TriggerGo?TriggerGo.activeSelf:false;
            if (!bActive && TriggerGo) TriggerGo.SetActive(true);
            if (m_Listener!=null)
            {
                bool bCommon = bCommonListener();

                if (TriggerGo) TriggerGo.SetActive(bActive);
                if (ExecuteEvents.Execute<IPointerClickHandler>(m_Listener.gameObject, eventData, ExecuteEvents.pointerClickHandler))
                {
                    return;
                }
                if (bCommon)
                {
                    if (TriggerGo) TriggerGo.SetActive(bActive);
                    return;
                }
            }
            if (TriggerGo)
            {
                TriggerGo.SetActive(bActive);
                ExecuteEvents.Execute<IPointerClickHandler>(TriggerGo, eventData, ExecuteEvents.pointerClickHandler);
            }
        }
        //------------------------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
            DoPressAction();
            CheckTargetListen();
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IPointerDownHandler>(m_Listener.gameObject, eventData, ExecuteEvents.pointerDownHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IPointerDownHandler>(TriggerGo, eventData, ExecuteEvents.pointerDownHandler);
        }
        //------------------------------------------------------
        public override void OnPointerUp(PointerEventData eventData)
        {
            DoUpAction();
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IPointerUpHandler>(m_Listener.gameObject, eventData, ExecuteEvents.pointerUpHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IPointerUpHandler>(TriggerGo, eventData, ExecuteEvents.pointerUpHandler);
        }
        //------------------------------------------------------
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IPointerExitHandler>(m_Listener.gameObject, eventData, ExecuteEvents.pointerExitHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IPointerExitHandler>(TriggerGo, eventData, ExecuteEvents.pointerExitHandler);
        }
        //------------------------------------------------------
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IPointerEnterHandler>(m_Listener.gameObject, eventData, ExecuteEvents.pointerEnterHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IPointerEnterHandler>(TriggerGo, eventData, ExecuteEvents.pointerEnterHandler);
        }
        //------------------------------------------------------
        public override void OnDrop(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IDropHandler>(m_Listener.gameObject, eventData, ExecuteEvents.dropHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IDropHandler>(TriggerGo, eventData, ExecuteEvents.dropHandler);
        }
        //------------------------------------------------------
        public override void OnScroll(PointerEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IScrollHandler>(m_Listener.gameObject, eventData, ExecuteEvents.scrollHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IScrollHandler>(TriggerGo, eventData, ExecuteEvents.scrollHandler);
        }
        //------------------------------------------------------
        public override void OnMove(AxisEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IMoveHandler>(m_Listener.gameObject, eventData, ExecuteEvents.moveHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IMoveHandler>(TriggerGo, eventData, ExecuteEvents.moveHandler);
        }
        //------------------------------------------------------
        public override void OnSubmit(BaseEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<ISubmitHandler>(m_Listener.gameObject, eventData, ExecuteEvents.submitHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<ISubmitHandler>(TriggerGo, eventData, ExecuteEvents.submitHandler);
        }
        //------------------------------------------------------
        public override void OnCancel(BaseEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<ICancelHandler>(m_Listener.gameObject, eventData, ExecuteEvents.cancelHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<ICancelHandler>(TriggerGo, eventData, ExecuteEvents.cancelHandler);
        }
        //------------------------------------------------------
        public override void OnSelect(BaseEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<ISelectHandler>(m_Listener.gameObject, eventData, ExecuteEvents.selectHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<ISelectHandler>(TriggerGo, eventData, ExecuteEvents.selectHandler);
        }
        //------------------------------------------------------
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IUpdateSelectedHandler>(m_Listener.gameObject, eventData, ExecuteEvents.updateSelectedHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IUpdateSelectedHandler>(TriggerGo, eventData, ExecuteEvents.updateSelectedHandler);
        }
        //------------------------------------------------------
        public override void OnDeselect(BaseEventData eventData)
        {
            if (m_Listener != null)
            {
                bool bCommon = bCommonListener();
                if (ExecuteEvents.Execute<IDeselectHandler>(m_Listener.gameObject, eventData, ExecuteEvents.deselectHandler))
                    return;
                if (bCommon) return;
            }
            if (TriggerGo)
                ExecuteEvents.Execute<IDeselectHandler>(TriggerGo, eventData, ExecuteEvents.deselectHandler);

        }
    }
}
