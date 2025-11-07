/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	UserInterface
作    者:	HappLI
描    述:	UI 界面
*********************************************************************/

using System;
using UnityEngine;

namespace Framework.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UserInterface : UISerialized
    {
        [Framework.Base.PluginDisplay("图形脚本")]
        public Framework.Plugin.AT.AgentTreeCoreData ATData;

        [System.NonSerialized]
        public ushort uiType = 0;

        [System.NonSerialized]
        protected IUIEventLogic m_uLogic;

        [System.NonSerialized]
        public Action<bool> OnApplicationPauseCB;
        //------------------------------------------------------
        public void SetEventLogic(IUIEventLogic pLogic)
        {
            m_uLogic = pLogic;
        }
        //------------------------------------------------------
        public IUIEventLogic GetEventLogic()
        {
            return m_uLogic;
        }
        //------------------------------------------------------
        protected bool DoUIEvent(UIEventType type)
        {
            if ((int)type >= UIEventDatas.Length) return false;
            UIEventData data = UIEventDatas[(int)type];
            if (data == null) return false;
//#if !USE_FMOD
//            if (data.Audio != null) { Core.AudioManager.PlayEffect(data.Audio); }
//#endif
            return m_uLogic.ExcudeEvent(transform, type, data);
        }
        //------------------------------------------------------
        public bool OnShow()
        {
            bool bEvent = DoUIEvent(UIEventType.Show);
            return bEvent;
        }
        //------------------------------------------------------
        public bool OnHide()
        {
            bool bEvent = DoUIEvent(UIEventType.Hide);
            return bEvent;
        }
        //------------------------------------------------------
        public bool OnMoveOut()
        {
            bool bEvent = DoUIEvent(UIEventType.MoveOut);
            return bEvent;
        }
        //------------------------------------------------------
        public bool OnMoveIn()
        {
            bool bEvent = DoUIEvent(UIEventType.MoveIn);
            return bEvent;
        }
        //------------------------------------------------------
        private void OnApplicationPause(bool pause)
        {
            OnApplicationPauseCB?.Invoke(pause);
        }
        //------------------------------------------------------
        public void Visible()
        {
        }
        //------------------------------------------------------
        public void Hidden()
        {
        }
    }


}
