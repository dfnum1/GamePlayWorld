/********************************************************************
生成日期:	10:7:2019   13:00
类    名: 	PacketBase
作    者:	HappLI
描    述:	消息包
*********************************************************************/
using System;

namespace Framework.Net
{
    //------------------------------------------------------
    //! 消息包类
    //------------------------------------------------------
    [Framework.Plugin.AT.ATExportMono("网络模块/消息包")]
    public struct PacketBase : IUserData
    {
        public static PacketBase DEF = new PacketBase() { m_pMessage = null, m_pDatas = null, m_nCode = 0, m_nDataSize = 0 };
        public const int PACKETHEADLEN = 8;
        int m_nCode;
        ESessionType m_eSessionType;
        int m_nSessionIndex ;

        byte[]  m_pDatas;
        int     m_nDataSize;
        Google.Protobuf.IMessage m_pMessage;
        //------------------------------------------------------
        public bool IsValid()
        {
            if (m_pMessage != null && m_nCode != 0) return true;
            return m_pDatas != null && m_nDataSize > PACKETHEADLEN;
        }
        //------------------------------------------------------
        public byte[] GetDatas()
        {
            return m_pDatas;
        }
        //------------------------------------------------------
        public int GetDataSize()
        {
            return m_nDataSize;
        }
        //------------------------------------------------------
        public bool Fill(byte[] pDatas, int nLenth)
        {
            if (nLenth < PACKETHEADLEN) return false;
            m_pDatas = pDatas;
            m_nDataSize = nLenth;

            return true;
        }
        //------------------------------------------------------
        public bool IsMessage<T>() where T : Google.Protobuf.IMessage
        {
            return m_pMessage != null && m_pMessage is T;
        }
        //------------------------------------------------------
        public Google.Protobuf.IMessage GetMsg()
        {
            return m_pMessage;
        }
        //------------------------------------------------------
        public T GetMsg<T>() where T : Google.Protobuf.IMessage
        {
            return (T)m_pMessage;
        }
        //------------------------------------------------------
        public void SetSessionType(ESessionType sessionType, int sessionIndex=0)
        {
            m_nSessionIndex = sessionIndex;
            m_eSessionType = sessionType;
        }
        //------------------------------------------------------
        public ESessionType GetSessionType()
        {
            return m_eSessionType;
        }
        //------------------------------------------------------
        public int GetSessionIndex()
        {
            return m_nSessionIndex;
        }
        //------------------------------------------------------
        public void SetCode(int code)
        {
            m_nCode = code;
        }
        //------------------------------------------------------
        public int GetCode()
        {
            return m_nCode;
        }
        //------------------------------------------------------
        public void SetMsg(int nCode, Google.Protobuf.IMessage pMsg)
        {
            m_nCode = nCode;
            m_pMessage = pMsg;
        }
        //------------------------------------------------------
        public bool Paser(ANetWork pNetwork, byte[] pDatas, int nLenth, int offset =0)
        {
            if (pDatas == null) return false;
            try
            {
                m_pMessage = pNetwork.CreateMessageByCode(m_nCode, pDatas, nLenth, offset);
            }
            catch (Exception e)
            {
                m_pMessage = null;
                UnityEngine.Debug.LogWarning(e);
            }
            return m_pMessage!=null;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            m_pMessage = null;
        }
    }
}
