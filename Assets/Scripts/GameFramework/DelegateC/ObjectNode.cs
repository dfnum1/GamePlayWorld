/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ObjectNode
作    者:	HappLI
描    述:	Plus 代理
*********************************************************************/
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework.Core
{
    public class ObjectNode : IInstanceCallback, IInstanceSign
    {
        public int GUID;
        public string strName = "";
#if UNITY_EDITOR
        public string strFile = "";
#endif

        public IUserData pContextData = null;

        protected bool m_bVisible = true;
        protected bool m_bDestroy = false;
        protected AInstanceAble m_pObject;

        private bool m_bPauseAnimation = false;
        private float m_fAnimationSpeed = 1;
        private sTargetAnimation m_TargetAnimation = sTargetAnimation.Default;

        protected Vector3 m_Position = Vector3.zero;
        protected Vector3 m_EulerAngle = Vector3.zero;
        protected Vector3 m_Scale = Vector3.one;
        public ObjectNode()
        {
            Init();
        }
        //-------------------------------------------------
        protected virtual void Init()
        {
            m_bVisible = true;
            if (m_pObject != null) m_pObject.RecyleDestroy();
             m_pObject = null;
            GUID = 0;
            m_Position = Vector3.zero;
            m_EulerAngle = Vector3.zero;
            m_Scale = Vector3.one;
            m_fAnimationSpeed = 1;
            m_bPauseAnimation = false;
            m_TargetAnimation = sTargetAnimation.Default;
            pContextData = null;
        }
        //-------------------------------------------------
        public virtual void SetVisible(bool bVisible)
        {
            m_bVisible = bVisible;
            if (m_pObject)
            {
                if (bVisible) m_pObject.SetActive();
                else m_pObject.SetUnActive();
            }
        }
        //-------------------------------------------------
        public virtual void SetPosition(Vector3 pos)
        {
            m_Position = pos;
            if (m_pObject)
            {
                m_pObject.SetPosition(pos);
            }
        }
        //-------------------------------------------------
        public virtual void SetEulerAngle(Vector3 angle)
        {
            m_EulerAngle = angle;
            if (m_pObject)
            {
                m_pObject.SetEulerAngle(angle);
            }
        }
        //-------------------------------------------------
        public virtual void SetScale(Vector3 scale)
        {
            m_Scale = scale;
            if (m_pObject)
            {
                m_pObject.SetEulerAngle(m_Scale);
            }
        }
        //-------------------------------------------------
        public virtual void PauseAnimation(bool bPause)
        {
            m_bPauseAnimation = bPause;
            if (m_pObject != null)
            {
                Animator animator = m_pObject.GetBehaviour<Animator>();
                if (animator)
                {
                    animator.speed = bPause?0:m_fAnimationSpeed;
                }
            }
        }
        //-------------------------------------------------
        public virtual void PlayAnimation(int animation, float blendDuration, float blendOffset, int layer = 0)
        {
            m_TargetAnimation.animation = animation;
            m_TargetAnimation.blendOffset = blendOffset;
            m_TargetAnimation.blendDuration = blendDuration;
            m_TargetAnimation.layer = layer;
            if (m_pObject && animation!=0 )
            {
                Animator animator = m_pObject.GetBehaviour<Animator>();
                if(animator)
                {
                    animator.CrossFadeInFixedTime(animation, blendDuration, layer, 0);
                }
            }
        }
        //-------------------------------------------------
        public virtual void SetAnimSpeed(float speed)
        {
            m_fAnimationSpeed = speed;
            if (m_pObject != null)
            {
                Animator animator = m_pObject.GetBehaviour<Animator>();
                if (animator)
                {
                    animator.speed = m_bPauseAnimation ? 0 : m_fAnimationSpeed;
                }
            }
        }
        //-------------------------------------------------
        public virtual void Destroy()
        {
            m_bDestroy = true;
            if (m_pObject != null) m_pObject.RecyleDestroy();
            m_pObject = null;
        }
        //-------------------------------------------------
        public void OnSpawnCallback(InstanceOperiaon pICallback)
        {
            InstanceOperiaon pCallback = pICallback as InstanceOperiaon;
            if (m_pObject == pCallback.pPoolAble) return;
            if (m_pObject!=null)
            {
                m_pObject.RecyleDestroy();
            }
            m_pObject = pCallback.pPoolAble;
            if(m_pObject!=null)
            {
                SetPosition(m_Position);
                SetEulerAngle(m_EulerAngle);
                SetScale(m_Scale);
                SetVisible(m_bVisible);
                SetAnimSpeed(m_fAnimationSpeed);
                PauseAnimation(m_bPauseAnimation);
                if (m_TargetAnimation.animation!=0)
                {
                    PlayAnimation(m_TargetAnimation.animation, m_TargetAnimation.blendDuration, m_TargetAnimation.blendOffset, m_TargetAnimation.layer);
                }
            }
        }
        //-------------------------------------------------
        public void OnSpawnSign(InstanceOperiaon pCallback)
        {
            pCallback.SetUsed( GUID > 0 && !m_bDestroy);
        }
    }
}