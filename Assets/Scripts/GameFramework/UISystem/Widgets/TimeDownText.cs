/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TimeDownText
作    者:	HappLI
描    述:	倒计时
*********************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    [UIWidgetExport]
    public class TimeDownText : Text
    {
        private float m_fEndTime = 0;
        int m_nTime = 0;
        System.Action<IUserData> m_pOnEnd = null;
        IUserData m_pVar = null;
        bool m_bForamt = true;
        //------------------------------------------------------
        private void LateUpdate()
        {
            if (m_fEndTime <= 0) return;
            float fTime = m_fEndTime - Time.realtimeSinceStartup;
            SetText((int)fTime);
            if (fTime < 0)
            {
                if (m_pOnEnd != null) m_pOnEnd(m_pVar);
                m_pVar = null;
                m_pOnEnd = null;
                 m_fEndTime = 0;
            }
        }
        //------------------------------------------------------
        public void SetParam(float endTime, bool bFormat = true, System.Action<IUserData> OnEnd = null, IUserData var = null)
        {
            m_fEndTime = endTime;
            m_pOnEnd = OnEnd;
            m_pVar = var;
            m_bForamt = bFormat;
        }
        //------------------------------------------------------
        void SetText(int time)
        {
            if (time < 0) time = 0;
            if (m_nTime == time) return;
            m_nTime = time;
            if(m_bForamt) this.text = Base.TimerUtil.GetTimeForString(m_nTime);
            else this.text = m_nTime.ToString();
        }
    }
}
