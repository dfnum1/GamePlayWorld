/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TimerManager
作    者:	HappLI
描    述:	
*********************************************************************/

using System.Collections.Generic;

namespace Framework.Core
{
    public interface ITimerTicker
    {
        bool OnTimerTick(IBaseTimerEvent hHandle, IUserData param);
        bool IsTimerValid();
    }
    //------------------------------------------------------
    public class TimerManager : AModule
    {
        public delegate bool EventFunction(int nEventHash, IUserData param);

        long m_dwStartGameTime;
        long m_dwCurrentSeverTime;
        uint m_nDiffSeconds;

        long m_dwLastCountTime;
        long m_dwServerTimeDelta;

        int m_nAutoHash = 0;
        
        Dictionary<string, IBaseTimerEvent> m_vTimerEvents;
        HashSet<IBaseTimerEvent> m_vDestroyed = null;

        static Stack<TimerEvent> ms_vPools = new Stack<TimerEvent>(32);
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_vTimerEvents = new Dictionary<string, IBaseTimerEvent>();
            m_vDestroyed = new HashSet<IBaseTimerEvent>();
            m_nAutoHash = 0;
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            if(m_vTimerEvents!=null)
                m_vTimerEvents.Clear();
            if(m_vDestroyed != null)
                m_vDestroyed.Clear();
            m_nAutoHash = 0;
        }
        //------------------------------------------------------
        public void GameStart(long StartTime)
        {
            m_dwStartGameTime = StartTime;
        }
        //------------------------------------------------------
        public long GetGameStart()
        {
            return m_dwStartGameTime;
        }
        //------------------------------------------------------
        public long GetGameTime()
        {
            return m_dwCurrentSeverTime +m_nDiffSeconds;
        }
        //------------------------------------------------------
        public void SynchronousTime(long SynTime, long localTime)
        {
            if(SynTime ==0)
            {
                //! req net
            }
            else
            {       
                m_dwCurrentSeverTime = SynTime;
                m_dwServerTimeDelta = 0;
                m_dwLastCountTime = localTime;
            }
        }
        //------------------------------------------------------
        public uint GetDiffSeconds()
        {
            return m_nDiffSeconds;
        }
        //------------------------------------------------------
        public void Update(long lRuntime, long lUnScaleRunTime, bool bProcess = true)
        {
            if(m_dwStartGameTime > 0 && m_dwCurrentSeverTime > 0)
            {
                long dwDelta = lRuntime;
                long dwFrameTime = (dwDelta - m_dwLastCountTime);
                m_dwLastCountTime = dwDelta;

                m_dwServerTimeDelta += dwFrameTime;
                m_dwCurrentSeverTime += m_dwServerTimeDelta / 1000;
                m_dwServerTimeDelta %= 1000;
            }

            if (bProcess)
            {
                long runTime = lRuntime;
                long runUnScaleTime = lUnScaleRunTime;
                IBaseTimerEvent timer;
                foreach (var db in m_vTimerEvents)
                {
                    timer = db.Value;
                    if (!timer.Update(timer.IsTimeScale()? runTime: runUnScaleTime))
                        m_vDestroyed.Add(db.Value);
                }
            }
            IBaseTimerEvent tiemr;
            foreach (var db in m_vDestroyed)
            {
                tiemr = db;
                m_vTimerEvents.Remove(tiemr.GetEventName());
                RecylePool(tiemr);
            }
            m_vDestroyed.Clear();
        }
        //------------------------------------------------------
        public bool FindTimer(string strEvent)
        {
            return IsTimerEvent(strEvent);
        }
        //------------------------------------------------------
        public void RemoveTimer(string strEvent)
        {
            IBaseTimerEvent pEvent;
            if (m_vTimerEvents.TryGetValue(strEvent, out pEvent))
            {
                RecylePool(pEvent);
                m_vTimerEvents.Remove(strEvent);
            }
        }
        //------------------------------------------------------
        public void ClearTimer()
        {
            ClearTimerEvent();
        }
        //------------------------------------------------------
        private void RecylePool(IBaseTimerEvent timer)
        {
            timer.Clear();
            if (timer is TimerEvent)
            {
                if (ms_vPools.Count < 32) ms_vPools.Push(timer as TimerEvent);
            }
        }
        //------------------------------------------------------
        private TimerEvent NewPool(ITimerTicker pTicker, TimerManager.EventFunction Function, IUserData param)
        {
            TimerEvent timer = null;
            if (ms_vPools.Count > 0) timer = ms_vPools.Pop();
            else timer = new TimerEvent();

            timer.Clear();
            timer.Set(pTicker, Function, param);
            return timer;
        }
        //------------------------------------------------------
        public IBaseTimerEvent AddTimer(string strEventName, ITimerTicker pTicker, TimerManager.EventFunction Function, long Interval, long ExeTimes =-1, bool bDelta = false, bool bTimerScale = true, IUserData param = null, bool bReplace = false)
        {
            IBaseTimerEvent pEvent = RegisterTimerEvent(strEventName, pTicker, Function, param);
            if (pEvent!=null)
            {
                TimerEvent timer = (TimerEvent)pEvent;
                timer.bDelta = bDelta;
                timer.deltaTime = 0;
                timer.interval = Interval;
                timer.eventName = strEventName;
                timer.exeTimes = ExeTimes;
                timer.bTimerScale = bTimerScale;
                if(bTimerScale)  timer.callTime =  GetFramework().GetRunTime() + Interval;
                else timer.callTime = GetFramework().GetRunUnScaleTime() + Interval;
                timer.eventHash = ++m_nAutoHash;
            }
            else if(bReplace)
            {
                pEvent = GetTimerEvent(strEventName);
                TimerEvent timer = (TimerEvent)pEvent;
                timer.bDelta = bDelta;
                timer.deltaTime = 0;
                timer.interval = Interval;
                timer.exeTimes = ExeTimes;
                timer.bTimerScale = bTimerScale;
                if (bTimerScale) timer.callTime = GetFramework().GetRunTime() + Interval;
                else timer.callTime = GetFramework().GetRunUnScaleTime() + Interval;
            }
            return pEvent;
        }
        //------------------------------------------------------
        IBaseTimerEvent RegisterTimerEvent(string strEventName, ITimerTicker pTicker, TimerManager.EventFunction Function, IUserData param)
        {
            if(!IsTimerEvent(strEventName))
            {
                IBaseTimerEvent timer_event = NewPool(pTicker, Function, param);
                m_vTimerEvents.Add(strEventName, timer_event);
                return timer_event;
            }
            return null;
        }
        //------------------------------------------------------
        bool IsTimerEvent(string strEventName)
        {
            return m_vTimerEvents.ContainsKey(strEventName);
        }
        //------------------------------------------------------
        IBaseTimerEvent GetTimerEvent(string strEventName)
        {
            IBaseTimerEvent pEvent;
            if (m_vTimerEvents.TryGetValue(strEventName, out pEvent))
                return pEvent;
            return null;
        }
        //------------------------------------------------------
        void RemoveTimerEvent(string strEventName)
        {
            if (string.IsNullOrEmpty(strEventName)) return;
            IBaseTimerEvent pEvent;
            if (m_vTimerEvents.TryGetValue(strEventName, out pEvent))
            {
                RecylePool(pEvent);
                m_vTimerEvents.Remove(strEventName);
            }
        }
        //------------------------------------------------------
        void ClearTimerEvent()
        {
            foreach (var db in m_vTimerEvents)
            {
                RecylePool(db.Value);
            }
            m_vTimerEvents.Clear();
        }
        //------------------------------------------------------
        public bool CallTimerEvent(int nHashID)
        {
            return false;
        }
        //------------------------------------------------------
        public void RemoveTimerEvent(int nHashID)
        {

        }
        //------------------------------------------------------
        /**********************************************************************************************************
        函数名:AddTimer
        参数说明:TimerEventName=事件名,Object=对像,Function=方法,Interval=间隔,ExeTimes=次数,bDelta=是否补回时间差
        输出说明:无
        功能描述:增加定时器
        注意:bDelta为true时,补回时间差是将延时或慢了的多用的时间,补回到下次计时中去.减少整个时间段的误差.
             如一秒定时,这次耗时1.5时,下次会在0.5秒时回调.若这次卡了用时3.5秒.将连续回调3次,之后0.5秒回调第四次.
             但,如果处理耗时间太长,或暂时时间过长,会出现连续回调多次或卡机状态. 慎用!!!
        **********************************************************************************************************/
        public static IBaseTimerEvent RegisterTimer(AFramework framework, string strEventName, ITimerTicker pTicker, long Interval, long ExeTimes = -1, bool bDelta = false, bool bTimeScale = true, IUserData param = null, bool bReplace = false)
        {
            if (framework == null || framework.timerManager == null) return null;
            return framework.timerManager.AddTimer(strEventName, pTicker, null, Interval, ExeTimes, bDelta, bTimeScale, param, bReplace);
        }
        //------------------------------------------------------
        public static IBaseTimerEvent RegisterTimer(AFramework framework, string strEventName,TimerManager.EventFunction Function, long Interval, long ExeTimes = -1, bool bDelta = false, bool bTimeScale = true, IUserData param = null, bool bReplace = false)
        {
            if (framework == null || framework.timerManager == null) return null;
            return framework.timerManager.AddTimer(strEventName, null, Function, Interval, ExeTimes, bDelta, bTimeScale, param, bReplace);
        }
        //------------------------------------------------------
        public static void UnRegisterTimer(AFramework framework, string strEventName)
        {
            if (framework == null || framework.timerManager == null) return;
            framework.timerManager.RemoveTimer(strEventName);
        }
        //------------------------------------------------------
        public static bool HasTimer(AFramework framework, string strEventName)
        {
            if (framework == null || framework.timerManager == null) return false;
            return framework.timerManager.FindTimer(strEventName);
        }
    }
}