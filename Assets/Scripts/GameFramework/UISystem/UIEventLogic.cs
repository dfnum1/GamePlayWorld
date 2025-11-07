/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UILogic
作    者:	HappLI
描    述:	
*********************************************************************/

using UnityEngine;
using Framework.Core;

namespace Framework.UI
{
    public class UIEventLogic : UILogic, UIAnimatorCallback, IUIEventLogic
    {
        private UIEventData m_UIData;
        private bool m_isAnimationFinish = false;
        private float m_playTime = 0;
        UIEventType m_DoEvenType = UIEventType.Count;
        private UIBase m_pUI;

        UIAnimatorPlayable m_pUIAnimatoring = null;
        //------------------------------------------------------
        public bool IsEvnting()
        {
            return m_DoEvenType!= UIEventType.Count;
        }
        //------------------------------------------------------
        public override void OnInit(UIBase pBase)
        {
            base.OnInit(pBase);
            m_pUI = pBase;
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            if (m_pUI == null || !m_pUI.IsInstanced()) return;
            if (m_isAnimationFinish || m_UIData == null || m_UIData.Animation == null) return;

            GameObject pObj = m_UIData.ApplyRoot?m_UIData.ApplyRoot.gameObject:null;
            if(pObj == null)
                pObj = m_pUI.GetInstanceAble().gameObject;

            if(m_UIData.bReverse)
            {
                float length = m_UIData.Animation.length;
                m_UIData.Animation.SampleAnimation(pObj, length-m_playTime);
                m_playTime += fFrame;
                if (m_playTime >= length)
                {
                    m_UIData.Animation.SampleAnimation(pObj, 0);
                    m_isAnimationFinish = true;
                    m_UIData.Animation = null;
                    if (m_pUI != null) m_pUI.OnEventHandle(new Variable1() { intVal = (int)m_DoEvenType });
                    m_DoEvenType = UIEventType.Count;
                }
            }
            else
            {
                m_UIData.Animation.SampleAnimation(pObj, m_playTime);
                m_playTime += fFrame;
                if (m_playTime >= m_UIData.Animation.length)
                {
                    m_UIData.Animation.SampleAnimation(pObj, m_UIData.Animation.length);
                    m_isAnimationFinish = true;
                    m_UIData.Animation = null;
                    if (m_pUI != null) m_pUI.OnEventHandle(new Variable1() { intVal = (int)m_DoEvenType });
                    m_DoEvenType = UIEventType.Count;
                }
            }
        }
        //------------------------------------------------------
        public bool ExcudeEvent(Transform pTransform, UIEventType eventType, UIEventData uiEvent)
        {
            m_DoEvenType = UIEventType.Count;
#if USE_FMOD
            Framework.Core.AudioManager.PlayEvent(uiEvent.fmodEvent);
#endif
            if (m_pUIAnimatoring != null)
            {
                UIAnimatorFactory.getInstance().RemovePlayAble(m_pUIAnimatoring, false);
                m_pUIAnimatoring = null;
            }
            if (uiEvent.tweenGroup != null)
            {
                m_DoEvenType = eventType;
                return uiEvent.tweenGroup.Play((short)uiEvent.animationID, OnTweenerDelegate, true);
            }
            if (uiEvent.animationID!=0)
            {
                m_pUIAnimatoring = UIAnimatorFactory.getInstance().CreatePlayAble(uiEvent.animationID, uiEvent.ApplyRoot ? uiEvent.ApplyRoot : pTransform);
                if (m_pUIAnimatoring != null)
                {
                    m_DoEvenType = eventType;
                    m_pUIAnimatoring.SetUserData(new Variable1() { intVal = (int)eventType });
                    m_pUIAnimatoring.AddCallback(this);
                    m_pUIAnimatoring.SetSpeedScale(uiEvent.speedScale);
                    m_pUIAnimatoring.SetReverse(uiEvent.bReverse);
                    m_pUIAnimatoring.Start();
                    return true;
                }
            }
            if (uiEvent.Animation != null)
            {
                m_DoEvenType = eventType;
                m_isAnimationFinish = false;
                m_UIData = uiEvent;
                m_playTime = 0;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public override void OnHide()
        {
            m_isAnimationFinish = false;
            m_UIData = null;
            m_playTime = 0;
            if (m_pUIAnimatoring != null)
            {
                UIAnimatorFactory.getInstance().RemovePlayAble(m_pUIAnimatoring, false);
                m_pUIAnimatoring = null;
            }
            base.OnHide();
        }
        //------------------------------------------------------
        public void OnUIAnimatorPlayableBegin(IUserData userData)
        {

        }
        //------------------------------------------------------
        public void OnUIAnimatorPlayableEnd(IUserData userData)
        {
            m_pUIAnimatoring = null;
            if (m_pUI != null) m_pUI.OnEventHandle(userData);
            m_DoEvenType = UIEventType.Count;
        }
        //------------------------------------------------------
        void OnTweenerDelegate(RtgTween.RtgTweenerParam param, RtgTween.ETweenStatus status)
        {
            if(status == RtgTween.ETweenStatus.Complete)
            {
                if (m_pUI != null) m_pUI.OnEventHandle(new Variable1() { intVal = (int)m_DoEvenType });
                m_DoEvenType = UIEventType.Count;
            }
        }
    }
}
