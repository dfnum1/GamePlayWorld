
/********************************************************************
生成日期:	10:7:2019   15:29
类    名: 	TcpServerSession
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
namespace Framework.Net
{
    public enum ESessionType
    {
        Tcp = 0,
        Kcp,
        WebSocket,
        Count,
    }
    public interface ISessionCallback
    {
        void OnSessionState(AServerSession session, ESessionState eState);
    }

    public enum ESessionState
    {
        None = 0,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        TryConnect,
        TryConnectFailed,
    }
    [Framework.Plugin.AT.ATExportMono("网络模块/AServerSession")]
    public abstract class AServerSession : IUserData
    {
        public delegate void ChangeStateEvent(ESessionState eState);

        protected object m_pMutex = new object();

        protected int m_nIndex = 0;

        protected string m_Ip = null;
        protected int m_nPort;
        private string m_strAddress = null;
        protected System.Net.IPAddress[] m_IpAddress;

        protected int m_nTimeoutDelta = 0;
        protected uint m_nProcessCount;

        protected bool m_bDisconnected = false;
        protected ESessionState m_eState = ESessionState.None;

        protected float m_fReconnectStepTime = 0;
        protected uint m_nReconnectTryCnt = 0;
        protected uint m_nMaxReconnectTryCnt = 0;
        protected PacketSendWorker m_pSendWorker;
        protected PacketRecvWorker m_pRecvWorker;

        protected Queue<PacketBase> m_vCachePacket;
        protected List<PacketBase> m_vSendPacket;

        protected MemoryStream m_OutStream = new MemoryStream(1024);
        protected Google.Protobuf.CodedOutputStream m_pCodeOutput = null;

        protected List<ISessionCallback> m_vCallback = new List<ISessionCallback>(2);

        protected List<PacketBase> m_vReviceMsgQueue;

        protected float m_fConnectingBreakTime = 0;
        bool m_bDirtyState = false;
        protected System.Action<AServerSession> m_onCallback = null;

        protected ANetWork m_pNetWork = null;
        //------------------------------------------------------
        public AServerSession(ANetWork netWork, uint nReciveMaxSize, uint nSendMaxSize, uint nProcessCount, uint nTryConnectCount, int index =0)
        {
            m_fConnectingBreakTime = 0;
            m_bDirtyState = false;
            m_nIndex = index;
            m_pNetWork = netWork;
            m_nReconnectTryCnt = 0;
            m_nMaxReconnectTryCnt = nTryConnectCount;

            m_nProcessCount = nProcessCount;

            m_bDisconnected = false;
            m_pSendWorker = new PacketSendWorker(netWork, nSendMaxSize);
            m_pSendWorker.SetSendEvent(OnSend);
            m_pSendWorker.SetPacketEvent(OnPacketHander);

            m_pRecvWorker = new PacketRecvWorker(netWork,nReciveMaxSize);
            m_pRecvWorker.SetReciveEvent(OnRecive);
            m_pRecvWorker.SetPacketEvent(OnPacketHander);

            ChangeState(ESessionState.None);

            m_vCachePacket = new Queue<PacketBase>((int)nProcessCount);
            m_vSendPacket = new List<PacketBase>((int)nProcessCount);

            m_pCodeOutput = new Google.Protobuf.CodedOutputStream(m_OutStream);

            m_vReviceMsgQueue = new List<PacketBase>();
        }
        //------------------------------------------------------
        public ANetWork GetNetwork()
        {
            return m_pNetWork;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public int GetIndex()
        {
            return m_nIndex;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public string GetIp()
        {
            return m_Ip;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public int GetPort()
        {
            return m_nPort;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public string GetAddress()
        {
            return m_strAddress;
        }
        //------------------------------------------------------
        public virtual void SetLocalConnID(long id)
        {

        }
        //------------------------------------------------------
        protected virtual int PushProtocalHeads(MemoryStream inputStream) { return 0; }
        //------------------------------------------------------
        public void Register(ISessionCallback pCallback)
        {
            if (m_vCallback.Contains(pCallback)) return;
            m_vCallback.Add(pCallback);
        }
        //------------------------------------------------------
        public void UnRegister(ISessionCallback pCallback )
        {
            m_vCallback.Remove(pCallback);
        }
        //------------------------------------------------------
        public void AddCallback(System.Action<AServerSession> onCallback)
        {
            m_onCallback += onCallback;
        }
        //------------------------------------------------------
        public void RemoveCallback(System.Action<AServerSession> onCallback)
        {
            m_onCallback -= onCallback;
        }
        //------------------------------------------------------
        public void SetConnectTimeout(int nTimeout)
        {
            m_nTimeoutDelta = nTimeout;
        }
        //------------------------------------------------------
        protected abstract void Close();
        //------------------------------------------------------
        public void Connect(string strHost, int nPort, System.Action<AServerSession> onCallback = null)
        {
            if(onCallback!=null) m_onCallback += onCallback;
            //切换服务器的情况下,重置状态,
            if (m_eState == ESessionState.TryConnect && (m_Ip != strHost || m_nPort != nPort))
            {
                m_eState = ESessionState.None;
            }

            if (m_eState == ESessionState.Connected || m_eState == ESessionState.Connecting || m_eState == ESessionState.TryConnect)
            {
                if (m_onCallback != null) m_onCallback(this);
                return;
            }
            m_bDisconnected = false;

            if (string.IsNullOrEmpty(strHost) || strHost.CompareTo("0.0.0.0") == 0 || strHost.CompareTo("127.0.0.1") == 0)
            {
                var ipAdress = NetworkHelper.GetHostAddress(System.Net.Dns.GetHostName());
                strHost = ipAdress.ToString();
            }
            m_strAddress = strHost + ":" + nPort;

            m_nPort = nPort;
            m_Ip = strHost;
            m_IpAddress = System.Net.Dns.GetHostAddresses(m_Ip);


            ChangeState(ESessionState.Connecting);
            OnConnect();
        }
        //------------------------------------------------------
        protected abstract void OnConnect();
        //------------------------------------------------------
        public void ReConnect()
        {
            lock (m_pMutex)
            {
                ConnectingTimeBreak(0);
                m_eState = ESessionState.Connecting;
                m_bDisconnected = false;
                OnReConnect();
            }
        }
        //------------------------------------------------------
        protected abstract void OnReConnect();
        //------------------------------------------------------
        public void Disconnect()
        {
            if (m_eState == ESessionState.None || m_eState == ESessionState.Disconnected || m_eState == ESessionState.Disconnecting)
                return;
            if (m_eState == ESessionState.Connecting || m_eState == ESessionState.TryConnect)
            {
                m_eState = ESessionState.Disconnecting;
                return;
            }

            ChangeState(ESessionState.Disconnecting);
            Close();
            OnDisconnect();
            ChangeState(ESessionState.Disconnected);
        }
        //------------------------------------------------------
        protected abstract void OnDisconnect();
        //------------------------------------------------------
        public bool Send(PacketBase pPacket, bool bCache = true)
        {
            if (!pPacket.IsValid()) return false;
            if(bCache && (m_eState == ESessionState.Connecting ||
                m_eState == ESessionState.TryConnect ||
                m_eState == ESessionState.TryConnectFailed ||
                m_eState == ESessionState.Disconnecting ||
                m_eState == ESessionState.Disconnected))
            {
                //! 进行包缓存
                m_vCachePacket.Enqueue(pPacket);
                if (m_vCachePacket.Count > m_nProcessCount)
                    m_vCachePacket.Dequeue();

                if (m_eState == ESessionState.Disconnecting ||
                m_eState == ESessionState.Disconnected)
                {
                    if(m_nReconnectTryCnt >= m_nMaxReconnectTryCnt)
                        m_nReconnectTryCnt = 0;
                    m_fReconnectStepTime = 0;
                    m_eState = ESessionState.TryConnect;
                    //ReConnect();
                }
                return false;
            }
            if (m_eState == ESessionState.None)
                return false;

            int bResult = SendPacket(pPacket);
            if (bResult == 1)
                m_vSendPacket.Add(pPacket);
            return bResult>0;
        }
        //------------------------------------------------------
        public bool Send(int nCode, Google.Protobuf.IMessage pMsg, bool bCache = true)
        {
            if (pMsg == null) return false;
            PacketBase msg = new PacketBase();
            msg.SetMsg(nCode, pMsg);
            return Send(msg, bCache);
        }
        //------------------------------------------------------
        public void SendCachePacket()
        {
            foreach(var db in m_vCachePacket)
            {
                int result = SendPacket(db);
                if (result == 1)
                {
                    m_vSendPacket.Add(db);
                }
            }
            m_vCachePacket.Clear();
        }
        //------------------------------------------------------
        int SendPacket(PacketBase packet)
        {
            if (packet.GetMsg() != null)
            {
                return SendProtoMessage(packet.GetCode(), packet.GetMsg());
            }
            else
            {
                return m_pSendWorker.AddPacketToSend(packet.GetDatas(), packet.GetDataSize());
            }
        }
        //------------------------------------------------------
        int SendProtoMessage(int nCode, Google.Protobuf.IMessage pMsg)
        {
            int bodyLen = pMsg.CalculateSize();
            int flag = 0;
            if (m_pSendWorker.CanEncrypt()) flag |= 1 << 1;
           // if (compress) flag |= 1 << 0;

            m_OutStream.Position = 0;
            int offsetLen  = PushProtocalHeads(m_OutStream);
            m_OutStream.Write(System.BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(nCode << 16 | flag)), 0, 4);
            m_OutStream.Write(System.BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(bodyLen)), 0, 4);
            pMsg.WriteTo(m_pCodeOutput);
            m_pCodeOutput.Flush();
            m_OutStream.Position = 0;
            return m_pSendWorker.AddPacketToSend(m_OutStream.ToArray(), (int)(bodyLen + PacketBase.PACKETHEADLEN+ offsetLen));
        }
        //------------------------------------------------------
        protected void OnSend(byte[] pBuff, int nOffset, int nSize)
        {
       //     lock(m_pMutex)
            {
                if (m_eState != ESessionState.Connected) return;
                try
                {
                    OnSendBuffer(pBuff, nOffset, nSize);
                }
                catch(Exception pExeception)
                {
                    lock (m_pMutex)
                    {
                        m_bDisconnected = true;
                    }
                    Execption(pExeception);
                }
            }
        }
        //------------------------------------------------------
        protected abstract void OnSendBuffer(byte[] pBuff, int nOffset, int nSize);
        //------------------------------------------------------
        private bool OnRecive(ref byte[] reciveBuffer, int nOffset, ref int nSize)
        {
         //   lock (m_pMutex)
            {
                try
                {
                    if (OnReadBuffer(ref reciveBuffer, nOffset, ref nSize))
                    {
                        return true;
                    }
                }
                catch (Exception pException)
                {
                    lock (m_pMutex)
                    {
                        m_bDisconnected = true;
                    }
                    Execption(pException);
                }
                return false;
            }
        }
        //------------------------------------------------------
        protected abstract bool OnReadBuffer(ref byte[] reciveBuffer, int nOffset, ref int nSize);
        //------------------------------------------------------
        //! 在主线程上进行更新
        public void OnMainUpdate(float fFrameTime)
        {
            if (m_eState == ESessionState.Connecting)
            {
                if (m_fConnectingBreakTime > 0)
                {
                    m_fConnectingBreakTime -= Time.fixedDeltaTime;
                    if (m_fConnectingBreakTime <= 0)
                    {
                        ConnectingTimeBreak(0);
                        ChangeState(ESessionState.Disconnected);
                        m_bDisconnected = true;
                    }
                }
            }

            lock (m_pMutex)
            {
                if (m_bDisconnected)
                {
                    Disconnect();
                    m_bDisconnected = false;
                }
                if (m_eState == ESessionState.TryConnect)
                {
                    if (m_nReconnectTryCnt >= m_nMaxReconnectTryCnt)
                    {
                        if (m_bDirtyState)
                        {
                            OnConnectState(m_eState);
                            m_bDirtyState = false;
                        }
                        m_eState = ESessionState.TryConnectFailed;
                        OnConnectState(m_eState);
                        return;
                    }

                    m_fReconnectStepTime -= fFrameTime;
                    if (m_fReconnectStepTime > 0) return;
                    m_fReconnectStepTime = 5;
                    m_nReconnectTryCnt++;
                    ReConnect();

                    OnUpdate(fFrameTime);

                    if (m_bDirtyState)
                    {
                        OnConnectState(m_eState);
                        m_bDirtyState = false;
                    }
                    return;
                }
                if (m_eState != ESessionState.Connected)
                {
                    OnUpdate(fFrameTime);

                    if (m_bDirtyState)
                    {
                        OnConnectState(m_eState);
                        m_bDirtyState = false;
                    }
                    return;
                }
            }
            m_pSendWorker.MainUpdate(fFrameTime);
            m_pRecvWorker.MainUpdate(fFrameTime);


            for (int i =0; i < m_vReviceMsgQueue.Count; ++i)
            {
                m_pNetWork.OnPackage(m_vReviceMsgQueue[i]);
            }
            m_vReviceMsgQueue.Clear();

            for(int i =0; i < m_vSendPacket.Count; )
            {
                int result = SendPacket(m_vSendPacket[i]);
                if (result == 1) break;//full buffer
                if (result ==0 || result > 0)
                    m_vSendPacket.RemoveAt(i);
                else break;
            }

            OnUpdate(fFrameTime);

            if(m_bDirtyState)
            {
                OnConnectState(m_eState);
                m_bDirtyState = false;
            }
        }
        //------------------------------------------------------
        protected abstract void OnUpdate(float fFrame);
        //------------------------------------------------------
        public bool OnNetThreadUpdate(int job)
        {
            int jobCnt = 0;
            if (job == 0)
            {
                if (OnInnerNetThreadUpdate()) jobCnt++;
                if (m_pSendWorker.JobUpdate(0.033f)) jobCnt++;
            }
            else
            {
                if (m_pRecvWorker.JobUpdate(0.033f)) jobCnt++;
            }
            return jobCnt > 0;
        }
        //------------------------------------------------------
        protected virtual bool OnInnerNetThreadUpdate() { return false; }
        //------------------------------------------------------
        protected virtual void OnPacketHander(PacketBase pPacket)
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
        protected void ChangeState(ESessionState eState)
        {
            if (m_eState == eState) return;
            m_eState = eState;
            if (eState == ESessionState.Connecting)
                ConnectingTimeBreak(10);

            if (eState == ESessionState.Connected)
            {
                m_pSendWorker.SetSeed(0);
                m_pRecvWorker.SetSeed(0);
                m_nReconnectTryCnt = 0;
                m_pSendWorker.StartUp();
                m_pRecvWorker.StartUp();

                ConnectingTimeBreak(0);
                //! 发送缓存包
                SendCachePacket();
            }
            else
            {
                m_pSendWorker.Shutdown();
                m_pRecvWorker.Shutdown();
            }
            m_bDirtyState = true;
        }
        //------------------------------------------------------
        protected void ConnectingTimeBreak(float time)
        {
            m_fConnectingBreakTime = time;
        }
        //------------------------------------------------------
        protected void OnConnectState(ESessionState eState)
        {
            if (m_onCallback != null) m_onCallback(this);
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                m_vCallback[i].OnSessionState(this, eState);
            }
        }
        //------------------------------------------------------
        protected void Execption(Exception pException)
        {
            if (pException is IOException)
            {
                IOException ioException = pException as IOException;

                if (ioException.InnerException is SocketException)
                {
                    Framework.Base.Logger.Error("ioException.InnerException is SocketException ");
                    SocketException sockExcep = ioException.InnerException as SocketException;

                    Framework.Base.Logger.Error("Socket Error Type = " + sockExcep.SocketErrorCode.ToString());
                    Framework.Base.Logger.Error("Socket Message = " + sockExcep.Message);
                }
                else
                {
                    Framework.Base.Logger.Error("ioException.InnerException is not SocketException ");
                }

            }
            else if (pException is SocketException)
            {
                Framework.Base.Logger.Error( "Socket Error Type = " + (pException as SocketException).SocketErrorCode.ToString());
                Framework.Base.Logger.Error("Socket Message = " + (pException as SocketException).Message);
            }
            else
            {
                Framework.Base.Logger.Error("UNKNOW Exception..." + pException.ToString());
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public ESessionState GetState()
        {
            return m_eState;
        }
        //------------------------------------------------------
        public void SetSeed(uint nSeed)
        {
            m_pSendWorker.SetSeed(nSeed);
            m_pRecvWorker.SetSeed(nSeed);
        }
        //------------------------------------------------------
        public void Destroy()
        {
            Close();
        }
    }
}