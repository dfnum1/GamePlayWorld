/********************************************************************
生成日期:	23:3:2020   18:07
类    名: 	AudioGroupMixer
作    者:	HappLI
描    述:	声音组混合
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public struct GroupMixerData
    {
        public int group;
        public int order;
        public AnimationCurve mixer;
    }
    public class AudioGroupMixer
    {
        enum EMixerStatus : byte
        {
            None,
            MixerIn,
            MixerOut,
        }
        private float m_fDuration = 0;
        private int m_nOrder = 0;
        private AnimationCurve m_Mixer;

        private float m_fTime = 0;
        private float m_fMixerValue = 0;
        EMixerStatus m_eMixerState = EMixerStatus.None;

        private int m_nCounter = 0;
        public void SetCurve(AnimationCurve mixer)
        {
            m_Mixer = mixer;
            m_fDuration = BaseUtil.GetCurveMaxTime(mixer);
        }
        public void SetOrder(int order)
        {
            m_nOrder = order;
        }
        //-------------------------------------------------
        public int GetOrder()
        {
            return m_nOrder;
        }
        //-------------------------------------------------
        public int GetCounter()
        {
            return m_nCounter;
        }
        //-------------------------------------------------
        public void SetCounter(int count)
        {
            m_nCounter = count;
            if (count <= 0) MixerOut();
            else MixerIn();
        }
        //-------------------------------------------------
        public void AddCounter(int cnt)
        {
            m_nCounter += cnt;
            if (m_nCounter <= 0) MixerOut();
            else MixerIn();
        }
        //-------------------------------------------------
        public void MixerIn()
        {
            m_eMixerState = EMixerStatus.MixerIn;
        }
        //-------------------------------------------------
        public void MixerOut()
        {
            m_eMixerState = EMixerStatus.MixerOut;
        }
        //-------------------------------------------------
        public float GetMixer()
        {
            if (m_fDuration <= 0 || m_eMixerState == EMixerStatus.None) return 1;
            return m_fMixerValue;
        }
        //-------------------------------------------------
        public void Update(float fDelta)
        {
            if (m_fDuration <= 0) return;
            switch(m_eMixerState)
            {
                case EMixerStatus.MixerIn:
                    {
                        m_fTime += fDelta;
                        if (m_fTime > m_fDuration) m_fTime = m_fDuration;
                        m_fMixerValue = Mathf.Lerp(m_fMixerValue, m_Mixer.Evaluate(Mathf.Clamp(m_fTime, 0, m_fDuration)), fDelta * 10);
                    }
                    break;
                case EMixerStatus.MixerOut:
                    {
                        m_fTime -= fDelta;
                        if (m_fTime < 0) m_fTime = 0;
                        m_fMixerValue = Mathf.Lerp(m_fMixerValue, m_Mixer.Evaluate(Mathf.Clamp(m_fTime, 0, m_fDuration)), fDelta * 10);
                    }
                    break;
            }
        }
    }
}
