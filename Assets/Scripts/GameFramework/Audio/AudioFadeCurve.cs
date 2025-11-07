/********************************************************************
生成日期:	23:3:2020   18:07
类    名: 	AudioFadeCurve
作    者:	HappLI
描    述:	音效过度曲线
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public struct AudioFadeCurve
    {
        AnimationCurve m_curve;
        float m_fRuntime;
        float m_fDuration;
        bool m_bOverClear;
        bool m_bFading;
        Vector2 m_FadeValue;
        //-------------------------------------------------
        public void Start(AnimationCurve curve, float fStart, float fEnd, bool bOverClear)
        {
            if (BaseUtil.IsValidCurve(curve))
            {
                m_curve = curve;
                m_fDuration = BaseUtil.GetCurveMaxTime(m_curve);
                if (m_curve == null) m_fDuration = 0.5f;
                m_fRuntime = 0;
                m_bOverClear = bOverClear;
                m_bFading = true;
                m_FadeValue.x = fStart;
                m_FadeValue.y = fEnd;
                return;
            }
            Start(1, fStart, fEnd, bOverClear);
        }
        //-------------------------------------------------
        public void Start(float fTime, float fStart, float fEnd, bool bOverClear)
        {
            if (fTime <= 0) fTime = 0.5f;
            m_FadeValue.x = fStart;
            m_FadeValue.y = fEnd;
            m_curve = null;
            m_fDuration = fTime;
            m_fRuntime = 0;
            m_bOverClear = bOverClear;
            m_bFading = true;
        }
        //-------------------------------------------------
        public bool IsFading()
        {
            return m_bFading;
        }
        //-------------------------------------------------
        public void SetOverClear(bool bClear)
        {
            m_bOverClear = bClear;
        }
        //-------------------------------------------------
        public bool IsOverClear()
        {
            return m_bOverClear;
        }
        //-------------------------------------------------
        public bool IsFadeingIn()
        {
            return m_bFading && m_FadeValue.x < m_FadeValue.y;
        }
        //-------------------------------------------------
        public bool IsFadeingOut()
        {
            return m_bFading && m_FadeValue.x > m_FadeValue.y;
        }
        //-------------------------------------------------
        public void Clear()
        {
            m_curve = null;
            m_fDuration = 0;
            m_fRuntime = 0;
            m_bOverClear = false;
            m_bFading = false;
            m_FadeValue = Vector2.up;
        }
        //-------------------------------------------------
        public bool Evaluate(ISound snd, float fDeltaTime)
        {
            if (!m_bFading) return false;
            if (m_fDuration <= 0) return false;
            if (snd.IsPause()) return true;
            m_fRuntime += fDeltaTime;
            float fFactor = 0;
            if (m_curve != null)
            {
                fFactor = m_curve.Evaluate(m_fRuntime);
            }
            else
            {
                fFactor = Mathf.Clamp01(m_fRuntime / m_fDuration);
            }
            snd.SetVolumnRatio(m_FadeValue.x * (1 - fFactor) + m_FadeValue.y * fFactor);

            if (m_fRuntime >= m_fDuration)
            {
                m_bFading = false;
                return false;
            }
            return true;
        }
    }
}
