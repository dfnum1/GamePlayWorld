/********************************************************************
生成日期:	10:7:2019   12:06
类    名: 	NetHandler
作    者:	HappLI
描    述:	该类用于注册消息函数的回调,代码需要自动生成
*********************************************************************/

using System.Collections.Generic; 
using UnityEngine;

namespace Framework.Net
{
    public delegate void OnRevicePacketMsg(PacketBase msg);
    public class NetHandler
    {
        ANetWork m_pNetWork;
        Dictionary<int, OnRevicePacketMsg> m_vHandles;
        //------------------------------------------------------
        public NetHandler(ANetWork netWork)
        {
            m_pNetWork = netWork;
            m_vHandles = new Dictionary<int, OnRevicePacketMsg>();
        }
        //------------------------------------------------------
        public void Register(int code, OnRevicePacketMsg onHandler)
        {
            if (m_vHandles.ContainsKey(code)) return;
            m_vHandles.Add(code, onHandler);
        }
        //------------------------------------------------------
        public void UnRegister(int code)
        {
            m_vHandles.Remove(code);
        }
        //------------------------------------------------------
        public void OnPakckage(PacketBase msg)
        {
            OnRevicePacketMsg onHandler = null;

            m_pNetWork.ResetReceiveMessageTime(Framework.Base.TimerUtil.ClientNow());
            if (m_vHandles.TryGetValue(msg.GetCode(), out onHandler) && onHandler!=null)
            {
                onHandler(msg);
            }
        }
    }
}
