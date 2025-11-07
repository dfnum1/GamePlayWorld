/********************************************************************
生成日期:	2020-06-23
类    名: 	HudCamera
作    者:	happli
描    述:	Hud 相机
*********************************************************************/

using System;
using UnityEngine;

namespace Framework.Core
{
    public class HudCamera : MonoBehaviour
    {
        private Transform m_pTransform = null;
        private Camera m_pCamera;
        //------------------------------------------------------
        private void Awake()
        {
            m_pTransform = this.transform;
        }
        //------------------------------------------------------
        void OnEnable()
        {
            if (m_pCamera) m_pCamera.enabled = true;
        }
        //------------------------------------------------------
        void OnDisable()
        {
            if (m_pCamera)  m_pCamera.enabled = false;
            Clear();
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            Clear();
        }
        //------------------------------------------------------
        public Camera GetCamera()
        {
            if (m_pCamera == null)
            {
                m_pCamera = gameObject.GetComponent<Camera>();
                if (m_pCamera == null) m_pCamera = gameObject.AddComponent<Camera>();
                m_pCamera.hideFlags |= HideFlags.DontSave;
            }
            return m_pCamera;
        }
        //------------------------------------------------------
        public void Clear()
        {
            if (m_pCamera)
            {
                if (m_pCamera.activeTexture)
                    RenderTexture.ReleaseTemporary(m_pCamera.activeTexture);
                m_pCamera.targetTexture = null;
                m_pCamera.orthographic = false;
            }
        }
        //------------------------------------------------------
        public void SetTargetTexture(RenderTexture rt)
        {
            if (m_pCamera)
            {
                if (m_pCamera.activeTexture)
                    RenderTexture.ReleaseTemporary(m_pCamera.activeTexture);
                m_pCamera.targetTexture = rt;
            }
        }
        //------------------------------------------------------
        public void SetPosition(Vector3 pos)
        {
            m_pTransform.position = pos;
        }
        //------------------------------------------------------
        public void SetEulerAngle(Vector3 eulerAngle)
        {
            m_pTransform.eulerAngles = eulerAngle;
        }
        //------------------------------------------------------
        public void SetRotation(Quaternion rotate)
        {
            m_pTransform.rotation = rotate;
        }
        //------------------------------------------------------
        public void SetFov(float fov)
        {
            if(m_pCamera) m_pCamera.fieldOfView = fov;
        }
        //------------------------------------------------------
        public void SetLightCullingMask(int mask)
        {
            //if (m_pLightSlot) m_pLightSlot.nCullingMask = mask;
        }
        //------------------------------------------------------
        public void SetProjectionOrthographic(float size)
        {
            if (m_pCamera)
            {
                m_pCamera.orthographic = true;
                m_pCamera.orthographicSize = size;
            }
        }
    }
}
