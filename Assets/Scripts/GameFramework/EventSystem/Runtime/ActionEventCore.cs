/********************************************************************
生成日期:	02:28:2024
类    名: 	ActionEventCore
作    者:	HappLI
描    述:	事件数据创建
*********************************************************************/
using System.Collections.Generic;
namespace Framework.Core
{
    [System.Serializable]
    public struct ActionEventData
	{
		public int type;
		public string eventCmd;

		[System.NonSerialized]
		public BaseEvent runtimeEvent;
	}
	public interface IBaseEvent
	{
		void Reset(BaseEvent parameter);
	}
	[System.Serializable, EventCore]
	public class ActionEventCore
	{
		public List<ActionEventData> events = new List<ActionEventData>();
		//--------------------------------------------------------
		public void BuildEvent(AFramework pFramework, List<BaseEvent> vEvent)
		{
			if (pFramework == null)
				pFramework = AFramework.mainFramework;

            for (int i = 0; i < events.Count; ++i)
            {
                ActionEventData eventData = events[i];
				if(eventData.runtimeEvent == null)
				{
                    BaseEvent eventBase = BaseEvent.NewEvent(pFramework, eventData.eventCmd);
                    if (eventBase == null)
                        continue;

                    eventData.runtimeEvent = eventBase;
                    events[i] = eventData;
                }
				if(eventData.runtimeEvent !=null)
					vEvent.Add(eventData.runtimeEvent);
            }
		}
		//--------------------------------------------------------
		public void AddEvent(BaseEvent param)
		{
			if (events == null)
				events = new List<ActionEventData>();
            ActionEventData evnetData = new ActionEventData();
			evnetData.runtimeEvent = param;
			evnetData.type = (int)param.GetEventType();
			evnetData.eventCmd = param.ToString();
			events.Add(evnetData);
        }
		//--------------------------------------------------------
		public void DelEvent(BaseEvent param)
		{
			if (events == null) return;
			for(int i =0; i< events.Count; ++i)
			{
				ActionEventData eventData = events[i];
                if (eventData.runtimeEvent == param)
				{
					events.RemoveAt(i);
                    continue;
				}
			}
		}
		//--------------------------------------------------------
		public static void ReleaseEvent(BaseEvent eventParam)
		{
			if(eventParam == null ) return;
			AFramework framework = eventParam.GetFramework();
			if (framework == null || framework.eventSystem == null) return;
			framework.eventSystem.FreeEvent(eventParam);
		}
		//--------------------------------------------------------
		internal static BaseEvent NewEvent(ushort type)
		{
			if(type == (ushort)EEventType.UIEvent) return new Framework.Core.UIEventParameter();
			if(type == (ushort)EEventType.BornAble) return new Framework.Core.InstanceEventParameter();
			if (type == (ushort)EEventType.TimeScale) return new Framework.Core.TimeScaleEventParamenter();
			return null;
		}
		//--------------------------------------------------------
		public void Init(AFramework pFramework)
		{
			if (events == null)
				return;
			for(int i =0; i < events.Count; ++i)
			{
				ActionEventData eventData = events[i];
				if (eventData.runtimeEvent == null)
				{
                    BaseEvent eventBase = BaseEvent.NewEvent(pFramework, eventData.eventCmd);
                    if (eventBase == null)
                        continue;
                    eventData.runtimeEvent = eventBase;

                    events[i] = eventData;
                }
				eventData.runtimeEvent.FillParams();
            }
		}
#if UNITY_EDITOR
		public void Save()
        {
            if (events == null)
                return;
            for (int i = 0; i < events.Count; ++i)
            {
                ActionEventData eventData = events[i];
                if (eventData.runtimeEvent != null)
                {
                    eventData.type = (int)eventData.runtimeEvent.GetEventType();
                    eventData.eventCmd = eventData.runtimeEvent.ToString();
                    eventData.eventCmd = eventData.runtimeEvent.ToString();
					events[i] = eventData;

				}
            }
        }
#endif
	}
}
