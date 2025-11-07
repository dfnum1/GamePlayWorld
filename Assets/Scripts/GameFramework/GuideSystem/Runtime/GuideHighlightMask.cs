#if USE_GUIDESYSTEM
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Collections.Generic;
using UnityEngine.Serialization;
using System;

namespace Framework.Plugin.Guide
{
    public class GuideHighlightMask : Image, UnityEngine.ICanvasRaycastFilter
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
        [SerializeField]
        public RectTransform _target;
        Vector2 m_3DTargetPos;

        private EShape m_eShape = EShape.None;

        private Transform _cacheTrans = null;

        private bool m_bPenetrate = false;
        private int m_PenetrateGUID = 0;
        private RectTransform m_PenetrateTarget = null;

        private Vector3[] m_corners = new Vector3[4];

        private Vector2 m_toSize = Vector2.zero;
        private float m_fSpeed = 0;
        private bool m_bClick = true;

        private Canvas m_RootCanvas;
        private Camera m_UICamera;
        private RectTransform m_RootRectTrans;
        //------------------------------------------------------
        public void EnablePenetrate(bool bPenetrate, int target = 0)
        {
            m_bPenetrate = bPenetrate;
            m_PenetrateGUID = target;
            m_PenetrateTarget = null;
            if (target>0)
            {
                GuideGuid guide = GuideGuidUtl.FindGuide(target);
                if (guide)
                {
                    m_PenetrateTarget = guide.transform as RectTransform;
                    m_PenetrateGUID = 0;
                }
            }
        }
        //------------------------------------------------------
        public void SetRootCavas(Canvas rootCanvas, Camera uiCamera)
        {
            if (rootCanvas == null) return;
            m_RootCanvas = rootCanvas;
            m_UICamera = uiCamera;
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
        public void SetTarget(RectTransform target)
        {
            if (_target == target) return;
            _target = target;
            m_toSize = new Vector2(Screen.width, Screen.height)/2;
            m_3DTargetPos = Vector2.zero;
        }
        //------------------------------------------------------
        public void Set3DTarget(Vector2 target)
        {
            m_3DTargetPos = target;
        }
        //------------------------------------------------------
        bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
        {
            if (null == _target) return true;//点击有效
            bool inTarget = RectTransformUtility.RectangleContainsScreenPoint(_target, screenPos, eventCamera);
            if (inTarget && m_bClick) return false;//点击无效
            if(m_bPenetrate)
            {
                if (m_PenetrateGUID > 0)
                {
                    GuideGuid guide = GuideGuidUtl.FindGuide(m_PenetrateGUID);
                    if (guide)
                    {
                        m_PenetrateTarget = guide.transform as RectTransform;
                        m_PenetrateGUID = 0;
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
                if (_target)
                {
                    if (m_fSpeed > 0)
                    {
                        m_toSize = Vector2.Lerp(m_toSize, _target.sizeDelta * 0.5f, m_fSpeed * Time.deltaTime);
                    }
                    else
                    {
                        m_toSize = _target.rect.size * 0.5f;
                    }
                    material.SetVector(_CenterID, CenterWorldPos());
                    material.SetFloat(_SliderXID, m_toSize.x);
                    material.SetFloat(_SliderYID, m_toSize.y);
                    material.SetInt(_MaskTypeID, (int)m_eShape);
                }
                else
                {
                    if (m_3DTargetPos == Vector2.zero)
                    {
                        material.SetVector(_CenterID, Vector2.zero);
                        material.SetFloat(_SliderXID, 0);
                        material.SetFloat(_SliderYID, 0);

                        material.SetInt(_MaskTypeID, (int)EShape.None);
                    }
                    else
                    {
                        material.SetVector(_CenterID, m_3DTargetPos);//如果是3D建筑坐标,设置遮罩显示位置为手指的形状,大小100
                        material.SetFloat(_SliderXID, 100);
                        material.SetFloat(_SliderYID, 100);

                        material.SetInt(_MaskTypeID, (int)m_eShape);
                    }
                    
                }
            }
        }
        //------------------------------------------------------
        Vector4 CenterWorldPos()
        {
            //_target.GetWorldCorners(m_corners);
            //float  targetOffsetX = Vector2.Distance(WorldToCanvasPos(m_corners[0]), WorldToCanvasPos(m_corners[3])) / 2f;
            //float targetOffsetY = Vector2.Distance(WorldToCanvasPos(m_corners[0]), WorldToCanvasPos(m_corners[1])) / 2f;

            //float x = m_corners[0].x +((m_corners[3].x - m_corners[0].x) / 2);
            //float y = m_corners[0].y + ((m_corners[1].y - m_corners[0].y) / 2);
            //Vector3 centerWorld = new Vector3(x, y, 0);
            
            if (m_RootRectTrans && _target)
            {
                Vector2 privot = new Vector2(0.5f, 0.5f) - _target.pivot;
                Vector2 center = m_RootRectTrans.InverseTransformPoint(_target.position);
                return new Vector4(center.x + privot.x * _target.sizeDelta.x, center.y + privot.y * _target.sizeDelta.y, 0, 0);
            }

            return Vector4.zero;
        }
        //------------------------------------------------------
        Vector3 WorldToCanvasPos(Vector3 world)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RootCanvas.transform as RectTransform, world, m_UICamera, out position);
            return position;
        }
        //------------------------------------------------------
        public void SetClick(bool click)
        {
            m_bClick = click;
        }
    }
}
#endif