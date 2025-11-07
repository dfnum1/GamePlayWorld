#if UNITY_EDITOR
#define USE_SHOWFPS
#endif
/********************************************************************
生成日期:	1:11:2020 10:08
类    名: 	FpsStat
作    者:	HappLI
描    述:	帧率
*********************************************************************/
using Framework.Base;
using UnityEngine;

namespace Framework.Core
{
    public class FpsStat : Singleton<FpsStat>
    {
        private float m_fUpdateInterval = 0.25f;
        private float m_fLastInterval;
        private int m_nFrames = 0;
        public float fFps = 0;

        public static System.Action<float> OnFps;
        public void Update()
        {
          //  if (OnFps == null) return;
            ++m_nFrames;
            float timeNow = Time.unscaledTime;
            if (timeNow > m_fLastInterval + m_fUpdateInterval)
            {
                fFps = m_nFrames / (timeNow - m_fLastInterval);
                m_nFrames = 0;
                m_fLastInterval = timeNow;
#if USE_SHOWFPS
                if (OnFps!=null) OnFps(fFps);
#endif
            }
        }   
    }
}

