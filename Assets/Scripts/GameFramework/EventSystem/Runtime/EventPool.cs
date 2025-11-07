/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	EventPool
作    者:	HappLI
描    述:	实践池
*********************************************************************/
using System.Collections.Generic;

namespace Framework.Core
{
    internal class EventPool
    {
        Dictionary<ushort, Stack<BaseEvent>> m_vEventPool = null;
        //------------------------------------------------------
        internal EventPool()
        {
            m_vEventPool = new Dictionary<ushort, Stack<BaseEvent>>(64);
        }
        //------------------------------------------------------
        internal BaseEvent MallocEvent(int eventType)
        {
            if(m_vEventPool.TryGetValue((ushort)eventType, out var vPools))
            {
                if(vPools.Count>0)
                {
                    BaseEvent pEvent = vPools.Pop();
                    return pEvent;
                }
            }
            return ActionEventCore.NewEvent((ushort)eventType);
        }
        //------------------------------------------------------
        internal void FreeEvent(BaseEvent evt)
        {
            ushort evtType= (ushort)evt.GetEventType();
            if (!m_vEventPool.TryGetValue(evtType, out var vPools))
            {
                vPools = new Stack<BaseEvent>(16);
                m_vEventPool[evtType] = vPools;
            }
            evt.Destroy();
            vPools.Push(evt);
        }
    }
}

