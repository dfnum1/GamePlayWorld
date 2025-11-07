
/********************************************************************
生成日期:	10:7:2019   15:29
类    名: 	TcpServerSession
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Net.Sockets;
using System.Threading;
namespace Framework.Net
{
    public class TcpServerSession : AServerSession
    {
        Thread m_pConnectThread = null;
        TcpClient m_pClient = null;
        public TcpServerSession(ANetWork netWork, uint nReciveMaxSize, uint nSendMaxSize, uint nProcessCount, uint nTryConnectCount, int index= 0)
            : base(netWork, nReciveMaxSize, nSendMaxSize, nProcessCount, nTryConnectCount, index)
        {

        }
        //------------------------------------------------------
        protected override void Close()
        {
            if(m_pClient!=null) m_pClient.Close();
            if (m_pConnectThread != null) m_pConnectThread.Abort();
            m_pConnectThread = null;
        }
        //------------------------------------------------------
        void DoTimeoutListener(IAsyncResult pAsyncResult)
        {
            if (m_nTimeoutDelta > 0)
            {
                if (!pAsyncResult.AsyncWaitHandle.WaitOne(m_nTimeoutDelta, false))
                {
                    if (m_eState == ESessionState.Connecting)
                    {
                        if (m_pClient == null || !m_pClient.Connected)
                        {
                       //     m_pClient.Close();
                            ChangeState(ESessionState.TryConnect);
                        }

                        Base.Logger.Warning("TimeOut Exception");
                    }
                }
            }
        }
        //------------------------------------------------------
        protected override void OnConnect()
        {
            if (m_pClient != null) m_pClient.Close();
            m_pClient = new TcpClient(m_IpAddress[0].AddressFamily);
            try
            {
                if (m_pConnectThread != null) m_pConnectThread.Abort();
                m_pConnectThread = new Thread(OnConnectThread);
                m_pConnectThread.Start();
            }
            catch (System.Exception ex)
            {
                Base.Logger.Info("connect error:" + ex.ToString());
            }
        }
        //------------------------------------------------------
        void OnConnectThread()
        {
            IAsyncResult pResult = m_pClient.BeginConnect(m_IpAddress, m_nPort, OnConnectCallback, this);
            DoTimeoutListener(pResult);
        }
        //------------------------------------------------------
        protected override void OnReConnect()
        {
            Base.Logger.Info("try reconect count:" + m_nReconnectTryCnt);
            if (m_pClient == null)
            {
                m_pClient = new TcpClient(m_IpAddress[0].AddressFamily);
            }
            m_pNetWork.OnReConnect();


            m_eState = ESessionState.Connecting;
            m_fConnectingBreakTime = 0;
            m_bDisconnected = false;
            try
            {
                if (m_pConnectThread != null) m_pConnectThread.Abort();
                m_pConnectThread = new Thread(OnConnectThread);
                m_pConnectThread.Start();
            }
            catch (System.Exception ex)
            {
                Base.Logger.Error("connect error:" + ex.ToString());
            }
        }
        //------------------------------------------------------
        protected override void OnDisconnect()
        {
            m_pClient = null;
            if (m_pConnectThread != null) m_pConnectThread.Abort();
            m_pConnectThread = null;
        }
        //------------------------------------------------------
        protected override void OnSendBuffer(byte[] pBuff, int nOffset, int nSize)
        {
            if(m_pClient!=null) m_pClient.GetStream().Write(pBuff, nOffset, nSize);
        }
        //------------------------------------------------------
        protected override bool OnReadBuffer(ref byte[] reciveBuffer, int nOffset, ref int nSize)
        {
            if (m_pClient.Connected && m_pClient.GetStream().DataAvailable)
            {
                int bytesRead = m_pClient.GetStream().Read(reciveBuffer, 0, reciveBuffer.Length);
                if (bytesRead > 0)
                {
                    nSize = bytesRead;
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        protected override void OnPacketHander(PacketBase pPacket)
        {
//             if (pPacket.GetCode() == (int)Proto3.MID.MessageSeedRes)
//             {
//                 Proto3.MessageSeedResponse response = pPacket.GetMsg() as Proto3.MessageSeedResponse;
//                 if (response != null)
//                 {
//                     m_pNetWork.SetSeed(response.Seed);
//                 }
//             }
//             else
            {
                m_vReviceMsgQueue.Add(pPacket);
            }
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
        }
        //------------------------------------------------------
        private static void OnConnectCallback(IAsyncResult pAsynResult)
        {
            TcpServerSession pSession = pAsynResult.AsyncState as TcpServerSession;
            lock(pSession.m_pMutex)
            {
                try
                {
                    pSession.m_pClient.EndConnect(pAsynResult);
                    if (pSession.m_eState == ESessionState.Connecting)
                    {
                        if (pSession.m_pClient.Connected)
                        {
                            pSession.ChangeState(ESessionState.Connected);
                        }
                        else
                        {
                            pSession.m_pClient.EndConnect(pAsynResult);
                            pSession.ChangeState(ESessionState.TryConnect);
                        }
                    }
                    else if (pSession.m_eState == ESessionState.Disconnecting)
                    {
                        pSession.m_pClient.EndConnect(pAsynResult);
                        pSession.m_pClient.Close();
                        pSession.ChangeState(ESessionState.Disconnected);
                    }
  
                }
                catch(Exception pException)
                {
                    pSession.Execption(pException);
                    if (pSession.m_eState == ESessionState.Connecting)
                    {
                        pSession.ChangeState(ESessionState.TryConnect);
                    }
                }
            }
        }
    }
}