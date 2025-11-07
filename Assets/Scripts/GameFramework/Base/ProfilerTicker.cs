# if UNITY_EDITOR || USE_SERVER
/********************************************************************
生成日期:	3:10:2022  12:03
类    名: 	ProfilerTicker
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace Framework.Base
{
    public struct ProfilerTicker
    {
        private string m_strProfilerName;
        private System.DateTime m_lTicker;
        private long m_lPringThreshold;
        //------------------------------------------------------
        public ProfilerTicker(long lThreshold = 0)
        {
            m_lTicker = new DateTime(0);
            m_strProfilerName = null;
            m_lPringThreshold = lThreshold;
        }
        //------------------------------------------------------
        public void Start(string profilerName)
        {
            if (!ConfigUtil.bProfilerDebug)
                return;

            if (!string.IsNullOrEmpty(m_strProfilerName))
            {
                if (m_strProfilerName.CompareTo(profilerName) != 0)
                {
                    double tick = (System.DateTime.Now - m_lTicker).TotalMilliseconds;
                    if (tick >= m_lPringThreshold)
                        Base.Logger.Info(m_strProfilerName + ":" + tick * 0.001f + " 秒");
                }
            }

            m_strProfilerName = profilerName;
            m_lTicker = System.DateTime.Now;
        }
        //------------------------------------------------------
        public long Stop(string symbol=null)
        {
            if (!ConfigUtil.bProfilerDebug)
                return 0;
            long milliseconds = (long)(System.DateTime.Now - m_lTicker).TotalMilliseconds;
            m_lTicker = new DateTime(0);
            if (milliseconds >= m_lPringThreshold)
            {
                if (!string.IsNullOrEmpty(m_strProfilerName))
                {
                    if (string.IsNullOrEmpty(symbol))
                        Base.Logger.Info("Profiler[" + m_strProfilerName + "]:" + milliseconds * 0.001f + " 秒");
                    else
                        Base.Logger.Info("Profiler[" + m_strProfilerName　+ "@" + symbol + "]:" + milliseconds * 0.001f + " 秒");
                }
                else if(string.IsNullOrEmpty(symbol))
                    Base.Logger.Info("Profiler[" + symbol + "]:" + milliseconds * 0.001f + " 秒");
            }

            m_strProfilerName = null;
            return milliseconds; 
        }
    }
}
#endif