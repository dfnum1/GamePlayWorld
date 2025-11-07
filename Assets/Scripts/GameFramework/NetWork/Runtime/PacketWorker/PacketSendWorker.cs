/********************************************************************
生成日期:	10:7:2019   16:47
类    名: 	PacketWorker
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
namespace Framework.Net
{
    public class PacketSendWorker : PacketWorker
    {
        public delegate void SendEvent(byte[] pBuff, int nOffset, int nSize);

        SendEvent       m_pSendEvent = null;
        //------------------------------------------------------
        public PacketSendWorker(ANetWork pNetwork, uint nMaxSize) : base(pNetwork,nMaxSize)
        {
        }
        //------------------------------------------------------
        public void SetSendEvent(SendEvent pEvent)
        {
            m_pSendEvent = pEvent;
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrameTime)
        {
        }
        //------------------------------------------------------
        public int AddPacketToSend( byte[] pData, int nLenth )
        {
            if (nLenth <= 0) return 0;
            if(nLenth > m_pPacketBytes.TotalLength)
            {
                Debug.LogError( string.Format("数据包太大，不能超过{0}", BaseUtil.FormBytes(m_pPacketBytes.TotalLength)));
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("错误", "数据包太大，不能超过" + BaseUtil.FormBytes(m_pPacketBytes.TotalLength), "OK");
#endif
                return 0;
            }
            lock(m_pMuxLock)
            {
                if(nLenth > m_pPacketBytes.TotalLength - m_pPacketBytes.Length)
                {
                    return 1;
                }
                int offset = m_pPacketBytes.Length;
                if (m_bEncrypt && nLenth > PacketBase.PACKETHEADLEN)
                    Encrypt(pData, PacketBase.PACKETHEADLEN, nLenth - PacketBase.PACKETHEADLEN, 0);
                bool bSuccssed = m_pPacketBytes.Push(pData, 0, nLenth);
                return bSuccssed?2:1;
            }
        }
        //------------------------------------------------------
        protected override bool OnProgress()
        {
            //! 发包
            if (m_pSendEvent != null)
            {
                int bufferLen = m_pPacketBytes.Pop(m_pBuff, 0, m_pBuff.Length);
                if (bufferLen > 0)
                    m_pSendEvent(m_pBuff, 0, bufferLen);
            }
            return m_bStartUp;
        }
        //------------------------------------------------------
        public override int GetJob()
        {
            return 0;
        }
    }
}
