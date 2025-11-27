/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideHighlightMask
作    者:	HappLI
描    述:	引导蒙版Mask
*********************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Framework.Guide
{
    public class GuideHighlightMask : Image, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
        , IPointerEnterHandler, IPointerExitHandler
        , ISelectHandler, IDeselectHandler, IMoveHandler, IUpdateSelectedHandler
        , IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
        , IScrollHandler, ISubmitHandler, ICancelHandler
    {
        private static int _CenterID = 0;
        private static int _SliderXID = 0;
        private static int _SliderYID = 0;
        private static int _MaskTypeID = 0;
        static void CheckID()
        {
            if(_CenterID == 0)
            {
                _CenterID = Shader.PropertyToID("_Center");
                _SliderXID = Shader.PropertyToID("_SliderX");
                _SliderYID = Shader.PropertyToID("_SliderY");
                _MaskTypeID = Shader.PropertyToID("_MaskType");
            }
        }

        public enum EShape
        {
            None = 0,
            Box ,
            Circle,
            Diamond,
            SmallCircle
        }
        RectTransform m_target;
        Vector2 m_3DTargetPos;

        private EShape m_eShape = EShape.None;

        private Transform _cacheTrans = null;

        private bool m_bPenetrate = false;
        private int m_PenetrateGUID = 0;
        private int m_nListIndex = -1;
        private string m_PenetrateTag = null;
        private RectTransform m_PenetrateTarget = null;

        private Vector3[] m_corners = new Vector3[4];

        private Vector2 m_toSize = Vector2.zero;
        private float m_fSpeed = 0;
        private Vector2 m_ShapeScale = Vector2.one;
        private bool m_bClick = true;

        private float m_fClickRadius = 0;
        private Vector3 m_ClickPosition = Vector3.zero;
        private bool m_b3DPosition = false;

        private RectTransform m_RootRectTrans;
        //------------------------------------------------------
        public void EnablePenetrate(bool bPenetrate, int target = 0, int listIndex=-1, string targetTag = null, bool bFindTarget = true)
        {
            m_bPenetrate = bPenetrate;
            m_PenetrateGUID = target;
            m_PenetrateTag = targetTag;
            m_PenetrateTarget = null;
            m_nListIndex = listIndex;
            if (target>0 && bFindTarget)
            {
                GuideGuid guide = GuideGuidUtl.FindGuide(target, targetTag);
                if (guide)
                {
                    m_PenetrateTarget = guide.transform as RectTransform;
                    m_PenetrateGUID = 0;
                    m_PenetrateTag = null;
                }
            }
        }
        //-------------------------------------------
        public void SetMaskZoomPenetrate(Vector3 position, float radius, bool b3D)
        {
            m_fClickRadius = radius;
            m_ClickPosition = position;
            m_b3DPosition = b3D;
        }
        //------------------------------------------------------
        public void SetRootCavas(Canvas rootCanvas, Camera uiCamera)
        {
            if (rootCanvas == null) return;
            m_RootRectTrans = rootCanvas.transform as RectTransform;
        }
        //------------------------------------------------------
        public void SetSpeed(float fSpeed)
        {
            m_fSpeed = fSpeed;
        }
        //------------------------------------------------------
        public void SetShape(EShape type)
        {
            m_eShape = type;
        }
        //------------------------------------------------------
        public void SetShapeScale(Vector2 scale)
        {
            m_ShapeScale = scale;
        }
        //------------------------------------------------------
        public void SetTarget(RectTransform target)
        {
            if (m_target != target)
            {
                m_PenetrateTarget = target;
                m_target = target;
                m_toSize = new Vector2(Screen.width, Screen.height) / 2;
            }
            m_3DTargetPos = Vector2.zero;
        }
        //------------------------------------------------------
        public void Set3DTarget(Vector2 target)
        {
            m_3DTargetPos = target;
        }
        //------------------------------------------------------
        public RectTransform GetTarget()
        {
            if (m_PenetrateTarget) return m_PenetrateTarget;
            return m_target;
        }
        //------------------------------------------------------
        public override bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
        {
            if(m_fClickRadius>0)
            {
                if (m_b3DPosition)
                {
                    var mainCam = Camera.main;
                    if (mainCam == null)
                        return true;
                    var ray = mainCam.ScreenPointToRay(screenPos);
                    if (!GuideStepHandler.RayInsectionFloor(out var worldPos, ray.origin, ray.direction, m_ClickPosition.y))
                        return true;
                    if ((worldPos.x - m_ClickPosition.x) * (worldPos.x - m_ClickPosition.x) + (worldPos.y - m_ClickPosition.y) * (worldPos.y - m_ClickPosition.y) + (worldPos.z - m_ClickPosition.z) * (worldPos.z - m_ClickPosition.z) <= m_fClickRadius * m_fClickRadius)
                    {
                        return false;
                    }
                }
                else
                {
                    if (GuideSystem.getInstance().GetGuidePanel() == null)
                        return true;
                    var panel = GuideSystem.getInstance().GetGuidePanel();
                    Vector3 uguiPos = Vector3.zero;
                    if (!panel.ScreenToUIPos(screenPos, true, ref uguiPos))
                        return true;
                    if ((uguiPos.x - m_ClickPosition.x) * (uguiPos.x - m_ClickPosition.x) + (uguiPos.y - m_ClickPosition.y) * (uguiPos.y - m_ClickPosition.y) <= m_fClickRadius * m_fClickRadius)
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
            //return base.IsRaycastLocationValid(screenPos, eventCamera);
            var target = GetTarget();
            if (null == target) return true;//点击有效
            return !RectTransformUtility.RectangleContainsScreenPoint(target, screenPos, eventCamera);
            /*
            if (inTarget && m_bClick) return false;//点击无效
            if(m_bPenetrate)
            {
                if (m_PenetrateGUID > 0)
                {
                    GuideGuid guide = GuideGuidUtl.FindGuide(m_PenetrateGUID, m_PenetrateTag);
                    if (guide)
                    {
                        m_PenetrateTarget = guide.transform as RectTransform;
                        m_PenetrateGUID = 0;
                        m_PenetrateTag = null;
                    }
                }

                if (m_PenetrateTarget)
                {
                    inTarget = RectTransformUtility.RectangleContainsScreenPoint(m_PenetrateTarget, screenPos, eventCamera);
                    if (inTarget) return false;
                }
                else
                    return false;
            }
            */
            return true;
        }
        //------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
            _cacheTrans = this.transform as RectTransform;
        }
        //------------------------------------------------------
        void Update()
        {
            if(material)
            {
                CheckID();
                if (m_target)
                {
                    if (m_fSpeed > 0)
                    {
                        m_toSize = Vector2.Lerp(m_toSize, m_target.sizeDelta * 0.5f, m_fSpeed * Time.deltaTime);
                    }
                    else
                    {
                        m_toSize = m_target.rect.size * 0.5f;
                    }
                    material.SetVector(_CenterID, CenterWorldPos());
                    material.SetFloat(_SliderXID, m_toSize.x* m_ShapeScale.x);
                    material.SetFloat(_SliderYID, m_toSize.y* m_ShapeScale.y);
                    material.SetInt(_MaskTypeID, (int)m_eShape);
                }
                else
                {
                    if(m_fClickRadius>0)
                    {
                        material.SetVector(_CenterID, m_3DTargetPos);//如果是3D建筑坐标,设置遮罩显示位置为手指的形状,大小100
                        material.SetFloat(_SliderXID, m_fClickRadius * m_ShapeScale.x);
                        material.SetFloat(_SliderYID, m_fClickRadius * m_ShapeScale.y);

                        material.SetInt(_MaskTypeID, (int)m_eShape);
                    }
                    else if (m_3DTargetPos == Vector2.zero)
                    {
                        material.SetVector(_CenterID, Vector2.zero);
                        material.SetFloat(_SliderXID, 0);
                        material.SetFloat(_SliderYID, 0);

                        material.SetInt(_MaskTypeID, (int)EShape.None);
                    }
                    else
                    {
                     //   material.SetVector(_CenterID, m_3DTargetPos);//如果是3D建筑坐标,设置遮罩显示位置为手指的形状,大小100
                     //   material.SetFloat(_SliderXID, 100* m_ShapeScale.x);
                     //   material.SetFloat(_SliderYID, 100* m_ShapeScale.y);

                     //   material.SetInt(_MaskTypeID, (int)m_eShape);
                    }
                    
                }
            }
        }
        //------------------------------------------------------
        Vector4 CenterWorldPos()
        {
            //m_target.GetWorldCorners(m_corners);
            //float  targetOffsetX = Vector2.Distance(WorldToCanvasPos(m_corners[0]), WorldToCanvasPos(m_corners[3])) / 2f;
            //float targetOffsetY = Vector2.Distance(WorldToCanvasPos(m_corners[0]), WorldToCanvasPos(m_corners[1])) / 2f;

            //float x = m_corners[0].x +((m_corners[3].x - m_corners[0].x) / 2);
            //float y = m_corners[0].y + ((m_corners[1].y - m_corners[0].y) / 2);
            //Vector3 centerWorld = new Vector3(x, y, 0);
            
            if (m_RootRectTrans && m_target)
            {
                Vector2 privot = new Vector2(0.5f, 0.5f) - m_target.pivot;
                Vector2 center = m_RootRectTrans.InverseTransformPoint(m_target.position);
                return new Vector4(center.x + privot.x * m_target.sizeDelta.x, center.y + privot.y * m_target.sizeDelta.y, 0, 0);
            }

            return Vector4.zero;
        }
        //------------------------------------------------------
        public void SetClick(bool click)
        {
            m_bClick = click;
        }
        //------------------------------------------------------
        void OnUIWidgetTrigger(BaseEventData eventData, EUIWidgetTriggerType eventType)
        {
            if (!m_bPenetrate) return;
            if (eventType == EUIWidgetTriggerType.None) return;
            if (m_PenetrateGUID == 0) return;
            Guide.GuideSystem.getInstance().OnUIWidgetTrigger(m_PenetrateGUID, m_nListIndex, m_PenetrateTag, eventType);
        }
        //------------------------------------------------------
        public void OnPointerClick(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            if (eventData != null && eventData.dragging)
                return;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Click);
            ExecuteEvents.Execute<IPointerClickHandler>(pTarget, eventData, ExecuteEvents.pointerClickHandler);
        }
        //------------------------------------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Down);
            ExecuteEvents.Execute<IPointerDownHandler>(pTarget, eventData, ExecuteEvents.pointerDownHandler);
        }
        //------------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Enter);
            ExecuteEvents.Execute<IPointerEnterHandler>(pTarget, eventData, ExecuteEvents.pointerEnterHandler);
        }
        //------------------------------------------------------
        public void OnPointerExit(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Exit);
            ExecuteEvents.Execute<IPointerExitHandler>(pTarget, eventData, ExecuteEvents.pointerExitHandler);
        }
        //------------------------------------------------------
        public void OnPointerUp(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Up);
            ExecuteEvents.Execute<IPointerUpHandler>(m_target.gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }
        //------------------------------------------------------
        public void OnSelect(BaseEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Select);
            ExecuteEvents.Execute<ISelectHandler>(pTarget, eventData, ExecuteEvents.selectHandler);
        }
        //------------------------------------------------------
        public void OnUpdateSelected(BaseEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.UpdateSelect);
            ExecuteEvents.Execute<IUpdateSelectedHandler>(pTarget, eventData, ExecuteEvents.updateSelectedHandler);
        }
        //------------------------------------------------------
        public void OnDrag(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Drag);
            ExecuteEvents.Execute<IDragHandler>(pTarget, eventData, ExecuteEvents.dragHandler);
        }
        //------------------------------------------------------
        public void OnDrop(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Drop);
            ExecuteEvents.Execute<IDropHandler>(pTarget, eventData, ExecuteEvents.dropHandler);
        }
        //------------------------------------------------------
        public void OnDeselect(BaseEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Deselect);
            ExecuteEvents.Execute<IDeselectHandler>(pTarget, eventData, ExecuteEvents.deselectHandler);
        }
        //------------------------------------------------------
        public void OnScroll(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Scroll);
            ExecuteEvents.Execute<IScrollHandler>(pTarget, eventData, ExecuteEvents.scrollHandler);
        }
        //------------------------------------------------------
        public void OnMove(AxisEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Move);
            ExecuteEvents.Execute<IMoveHandler>(pTarget, eventData, ExecuteEvents.moveHandler);
        }
        //------------------------------------------------------
        public void OnBeginDrag(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.BeginDrag);
            ExecuteEvents.Execute<IBeginDragHandler>(pTarget, eventData, ExecuteEvents.beginDragHandler);
        }
        //------------------------------------------------------
        public void OnEndDrag(PointerEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(target, eventData.position, eventData.enterEventCamera))
                return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.EndDrag);
            ExecuteEvents.Execute<IEndDragHandler>(pTarget, eventData, ExecuteEvents.endDragHandler);
        }
        //------------------------------------------------------
        public void OnSubmit(BaseEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Submit);
            ExecuteEvents.Execute<ISubmitHandler>(pTarget, eventData, ExecuteEvents.submitHandler);
        }
        //------------------------------------------------------
        public void OnCancel(BaseEventData eventData)
        {
            var target = GetTarget();
            if (target == null) return;
            GameObject pTarget = target.gameObject;
            OnUIWidgetTrigger(eventData, EUIWidgetTriggerType.Cancel);
            ExecuteEvents.Execute<ICancelHandler>(pTarget, eventData, ExecuteEvents.cancelHandler);
        }
    }
}