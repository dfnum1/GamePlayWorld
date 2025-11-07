/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TimerEvent
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.Core
{
    public interface IBaseTimerEvent
    {
        bool Update(long dwNow);
        bool Call(int nHashID);
        void SetFlag(uint nFlag);
        uint GetFlag();
        string GetEventName();

        bool IsTimeScale();
        void Clear();
    }
    //------------------------------------------------------
    public class TimerEvent : IBaseTimerEvent
    {
        ITimerTicker m_pTicker;
        IUserData m_pParam;
        TimerManager.EventFunction m_pFunc;
        uint m_nFlag = 0;

        public bool bDelta;
        public long deltaTime;
        public long callTime;
        public long interval;
        public long exeTimes;
        public string eventName;
        public int eventHash;
        public bool bTimerScale;
        //------------------------------------------------------
        public TimerEvent()
        {
            Clear();
        }
        //------------------------------------------------------
        public void Set(ITimerTicker ticker, TimerManager.EventFunction pFunc, IUserData param)
        {
            m_pTicker = ticker;
            m_pFunc = pFunc;
            m_pParam = param;
        }
        //------------------------------------------------------
        ~TimerEvent()
        {
            m_pTicker = null;
            m_pFunc = null;
            m_pParam = null;
        }
        //------------------------------------------------------
        public bool IsTimeScale()
        {
            return bTimerScale;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_pTicker = null;
            m_pFunc = null;
            m_pParam = null;

            bDelta = false;
            deltaTime = 0;
            callTime = 0;
            interval = 0;
            exeTimes = 0;
            eventName = "";
            eventHash = 0;
            bTimerScale = true;
        }
        //------------------------------------------------------
        public bool Update(long dwNow)
        {
            if(eventHash!=0)
            {
                if (m_pTicker != null && !m_pTicker.IsTimerValid()) return false;
                while (dwNow >= callTime)
                {
                    bool bResult = Call(eventHash);
                    if (bResult)
                    {
                        if (exeTimes > 0)
                            exeTimes--;
                        if (exeTimes != 0)
                        {
                            if (bDelta) deltaTime = dwNow - callTime;
                        }
                    }
                    else
                        exeTimes = 0;

                    if(exeTimes == 0) break;
                    else
                    {
                        callTime = dwNow + interval - (bDelta ? deltaTime : 0);
                    }
                }

            }
            return exeTimes!=0;
        }
        //------------------------------------------------------
        public bool Call(int nHashID)
        {
            int bOver = 0;
            if (m_pTicker != null && m_pTicker.OnTimerTick(this, m_pParam)) bOver++;
            if (m_pFunc != null)
            {
                if (m_pFunc(nHashID, m_pParam)) bOver++;
            }
            return bOver>=0;
        }
        //------------------------------------------------------
       public  void SetFlag(uint nFlag)
        {
            m_nFlag = nFlag;
        }
        //------------------------------------------------------
        public uint GetFlag() { return m_nFlag;}
        //------------------------------------------------------
        public string GetEventName() { return eventName; }
    }
}