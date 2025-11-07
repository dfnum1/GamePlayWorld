/********************************************************************
生成日期:	10:7:2019   15:29
类    名: 	TcpServerSession
作    者:	HappLI
描    述:	
*********************************************************************/
//#define USE_CSKCP
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Framework.Net
{
    public class KcpClient
    {
        public const int ERR_Success = 0;
        public const int ERR_SocketCantSend = 102009;
        public const int ERR_SocketError = 102010;
        public const int ERR_KcpWaitSendSizeTooLarge = 102011;

        public const byte SYN = 101;
        public const byte ACK = 102;
        public const byte FIN = 103;
        public const byte MSG = 104;
        public const byte ReconnectSYN = 105;
#if USE_CSKCP
        private KCPCS.OutputDelegate m_pKcpOutput;
#else
        private Core.KcpOutput m_pKcpOutput;
#endif

        public int Error { get; set; }

        private Socket m_pSocket;
#if USE_CSKCP
        private KCPCS m_pKcpPtr;
#else
        private IntPtr m_pKcpPtr;
        static Dictionary<IntPtr, ANetWork> ms_KcpNetwork = new Dictionary<IntPtr, ANetWork>(1);
#endif

        private System.Action<int> m_OnCallback = null;

        private byte[] m_arrSendTempBuffer = null;
        private PacketBytes m_pSendPacket = null;

        private bool m_isConnected;
        public bool isConnected
        {
            get { return m_isConnected; }
        }

        private readonly IPEndPoint m_remoteEndPoint;

        private uint m_nLastRecvTime;
        private uint m_nNextUpdateTime;

        private readonly uint m_nCreateTime;

        public long ClientConn { get; private set; }
        public uint ServerConn { get; private set; }

        private readonly MemoryStream m_MemoryStream;

        PacketWorker.PacketEvent m_onPacketEvent;
        private KcpServerSession m_pSession;
        // connect
        //------------------------------------------------------
        public KcpClient(KcpServerSession kService, PacketWorker.PacketEvent onPacketEvent, long localConn, Socket socket, IPEndPoint remoteEndPoint, System.Action<int> OnCallback = null)
        {
            m_OnCallback = OnCallback;
            m_pSession = kService;
            this.m_MemoryStream = new MemoryStream(ushort.MaxValue);

            m_pSendPacket = new PacketBytes();

            this.ClientConn = localConn;
            this.m_pSocket = socket;
            this.m_remoteEndPoint = remoteEndPoint;
            this.m_nLastRecvTime = kService.TimeNow;
            m_nNextUpdateTime = 0;
            this.m_nCreateTime = kService.TimeNow;

            m_onPacketEvent = onPacketEvent;
            this.HandleConnnect();
        }
        //------------------------------------------------------
        public void Dispose()
        {
            try
            {
                if (this.Error == ERR_Success)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this.Disconnect();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
#if USE_CSKCP
            if (m_pKcpPtr != null)
            {
                m_pKcpPtr.Release();
                m_pKcpPtr = null;
            }
#else
            if (this.m_pKcpPtr != IntPtr.Zero)
            {
                ms_KcpNetwork.Remove(this.m_pKcpPtr);
                Core.KCPUtil.KcpRelease(this.m_pKcpPtr);
                this.m_pKcpPtr = IntPtr.Zero;
            }
#endif
            this.m_pSocket = null;
            this.m_MemoryStream.Dispose();
            if (m_OnCallback != null) m_OnCallback(-1);
             m_OnCallback = null;
        }
        //------------------------------------------------------
        public MemoryStream Stream
        {
            get
            {
                return this.m_MemoryStream;
            }
        }
        //------------------------------------------------------
        public void Disconnect(int error)
        {
            OnError(error);
        }
        //------------------------------------------------------
        void OnError(int error)
        {
            this.Error = error;
        }
        //------------------------------------------------------
        private KcpServerSession GetSession()
        {
            return m_pSession;
        }
        //------------------------------------------------------
        public ANetWork GetNetwork()
        {
            if (m_pSession == null) return null;
            return m_pSession.GetNetwork();
        }
        //------------------------------------------------------
        void HandleConnnect()
        {
            if (this.m_isConnected)
            {
                return;
            }

            this.ServerConn = 0;
#if USE_CSKCP
            m_pKcpPtr = new KCPCS(this.RemoteConn, new IntPtr(this.LocalConn));
            m_pKcpPtr.NoDelay(1, 20, 2, 1);
            m_pKcpPtr.WndSize(ushort.MaxValue, ushort.MaxValue);//拥塞窗口，表示发送方可发送多少个KCP数据包.与接收方窗口有关，与网络状况（拥塞控制）有关，与发送窗口大小有关。
            m_pKcpPtr.SetMTU(1400); // 默认1400 ,最大传输单元。即每次发送的最大数据c
            m_pKcpPtr.SetMinRTO(30);//重传超时时间。
            SetOutput();
#else
            this.m_pKcpPtr = Core.KCPUtil.KcpCreate(this.ServerConn, new IntPtr(m_pSession.GetIndex()));
            SetOutput();
            Core.KCPUtil.KcpNodelay(this.m_pKcpPtr, 1, 10, 1, 1);
            Core.KCPUtil.KcpWndsize(this.m_pKcpPtr, 32, 32);
            Core.KCPUtil.KcpSetmtu(this.m_pKcpPtr, 470);
            ms_KcpNetwork[this.m_pKcpPtr] = GetNetwork();
#endif

            this.m_isConnected = false;
            this.m_nLastRecvTime = m_pSession.TimeNow;

            Connect();
        }
        //------------------------------------------------------
        public void Accept()
        {
            if (this.m_pSocket == null)
            {
                return;
            }

            uint timeNow = m_pSession.TimeNow;

            try
            {
                byte[] buffer = this.m_MemoryStream.GetBuffer();
                m_pSendPacket.Push(BitConverter.GetBytes(ACK), 0, 1);
                m_pSendPacket.Push(BitConverter.GetBytes(ClientConn), 0, 8);
                m_pSendPacket.Push(BitConverter.GetBytes(ServerConn), 0, 4);
                int result = this.m_pSocket.SendTo(m_pSendPacket.Buffers, 0, m_pSendPacket.Length, SocketFlags.None, m_remoteEndPoint);
                if (m_OnCallback != null) m_OnCallback(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketCantSend);
            }
        }
        //------------------------------------------------------
        private void Connect()
        {
            try
            {
                uint timeNow = m_pSession.TimeNow;

                this.m_nLastRecvTime = timeNow;

                m_pSendPacket.Clear();
                m_pSendPacket.Push(BitConverter.GetBytes(SYN), 0,1);
                m_pSendPacket.Push(BitConverter.GetBytes(this.ClientConn),0, 8);
                m_pSendPacket.Push(BitConverter.GetBytes(this.ServerConn), 0, 4);
                byte[] ipBytes = System.Text.Encoding.Default.GetBytes(m_pSession.GetIp());
                m_pSendPacket.Push(BitConverter.GetBytes((byte)ipBytes.Length), 0, 1);
                m_pSendPacket.Push(ipBytes, 0, ipBytes.Length);
                int result = this.m_pSocket.SendTo(m_pSendPacket.Buffers, 0, m_pSendPacket.Length, SocketFlags.None, m_remoteEndPoint);
                if (m_OnCallback != null) m_OnCallback(result);
                m_pSendPacket.Clear();

                HandleSend();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketCantSend);
            }
        }
        //------------------------------------------------------
        /// <summary>
        /// 发送请求断开消息
        /// </summary>
        private void Disconnect()
        {
            if (this.m_pSocket == null)
            {
                return;
            }
            try
            {
                if(m_pSocket.Connected)
                {
                    m_pSendPacket.Clear();
                    m_pSendPacket.Push(BitConverter.GetBytes(FIN), 0, 1);
                    m_pSendPacket.Push(BitConverter.GetBytes(ClientConn), 0, 8);
                    m_pSendPacket.Push(BitConverter.GetBytes(ServerConn), 0, 4);
                    m_pSendPacket.Push(BitConverter.GetBytes(Error), 0, 4);
                    int result = this.m_pSocket.SendTo(m_pSendPacket.Buffers, 0, m_pSendPacket.Length, SocketFlags.None, m_remoteEndPoint);
                    if (m_OnCallback != null) m_OnCallback(result);
                    m_pSendPacket.Clear();

                    HandleSend();
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketCantSend);
            }
        }

        public void OnReciveSocket(byte[] reciveBuffer, int size =0)
        {
            if (reciveBuffer == null || size <= 1) return;
            byte flag = reciveBuffer[0];
            switch (flag)
            {
                case ACK:
                    {
                        this.m_isConnected = true;
                        ServerConn = BitConverter.ToUInt32(reciveBuffer, 1);

                        if (m_OnCallback != null) m_OnCallback(0);
                        //! send catch
                        HandleSend();
                    }
                    break;
                case FIN:
                    {
                        //Disconnet
                        if (m_OnCallback != null) m_OnCallback(-1);
                    }
                    break;
            }
        }

        public void Update()
        {
            uint timeNow = m_pSession.TimeNow;

            // 如果还没连接上，发送连接请求
            if (!this.m_isConnected)
            {
                //				// 10秒没连接上则报错
                //				if (timeNow - this.createTime > 10 * 1000)
                //				{
                //					this.OnError(ErrorCode.ERR_KcpCantConnect);
                //					return;
                //				}
                //				
                //				if (timeNow - this.lastRecvTime < 500)
                //				{
                //					return;
                //				}
            }

            if(timeNow < m_nNextUpdateTime)
            {
                return;
            }

            try
            {
#if USE_CSKCP
                if(m_pKcpPtr!=null) m_pKcpPtr.Update(timeNow);
#else
                if(this.m_pKcpPtr!= System.IntPtr.Zero) Core.KCPUtil.KcpUpdate(this.m_pKcpPtr, timeNow);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketError);
                return;
            }
#if USE_CSKCP
            if(m_pKcpPtr!=null)
            {
                m_nNextUpdateTime = m_pKcpPtr.Check(timeNow);
            }
#else
            if (this.m_pKcpPtr != IntPtr.Zero)
            {
                m_nNextUpdateTime = Core.KCPUtil.KcpCheck(this.m_pKcpPtr, timeNow);
            }
#endif
        }

        private void HandleSend()
        {
            while (true)
            {
                if (this.m_pSendPacket.Length <= 0)
                {
                    break;
                }

                this.KcpSend(m_pSendPacket.Buffers,0, m_pSendPacket.Length);
                m_pSendPacket.Clear();
            }
        }

        public void HandleRecv(byte[] date, int offset, int length)
        {
            this.m_isConnected = true;
#if USE_CSKCP
            m_pKcpPtr.Input(date, offset, length);
#else
            Core.KCPUtil.KcpInput(this.m_pKcpPtr, date, offset, length);
#endif
            while (true)
            {
#if USE_CSKCP
                int n = m_pKcpPtr.PeekSize();
#else
                int n = Core.KCPUtil.KcpPeeksize(this.m_pKcpPtr);
#endif
                if (n < 0)
                {
                    return;
                }
                if (n == 0)
                {
                    this.OnError((int)SocketError.NetworkReset);
                    return;
                }

                byte[] buffer = this.m_MemoryStream.GetBuffer();
                this.m_MemoryStream.SetLength(n);
                this.m_MemoryStream.Seek(0, SeekOrigin.Begin);
#if USE_CSKCP
                int count = m_pKcpPtr.Recv(buffer,0, ushort.MaxValue);
#else
                int count = Core.KCPUtil.KcpRecv(this.m_pKcpPtr, buffer,0, ushort.MaxValue);
#endif
                if (n != count)
                {
                    return;
                }
                if (count <= 0)
                {
                    return;
                }

                this.m_nLastRecvTime = m_pSession.TimeNow;

                try
                {
                    int opcodeAndFlag = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(m_MemoryStream.GetBuffer(), 0));
                    int bodyLen = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(m_MemoryStream.GetBuffer(), 4));
                    if (bodyLen < 0)
                    {
                        return;
                    }
                    if (bodyLen > m_MemoryStream.Length)
                    {
                        //解析包体太大,分包解析
                        break;
                    }
                        int opcode = opcodeAndFlag >> 16;
                    int flag = opcodeAndFlag & 0xffff;

                    bool bCompress = (flag & 1) != 0;
                    bool bEncrypt = (flag & (1 << 1)) != 0;

                    PacketBase pkg = new PacketBase();
                    pkg.SetCode(opcode);
                    if (pkg.Paser(GetNetwork(),m_MemoryStream.GetBuffer(), bodyLen, PacketBase.PACKETHEADLEN))
                    {
                        if(m_onPacketEvent!=null) m_onPacketEvent(pkg);
                    }
                    else
                        Framework.Base.Logger.Warning("消息解析失败:Code=" + opcode);
                }
                catch (System.Exception e)
                {
                    Framework.Base.Logger.Error($"{m_remoteEndPoint} {m_MemoryStream.Length} {e}");
                    // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                 // OnError((int)PacketParserError);
                }
            }
        }
#if USE_CSKCP
        public void Output(byte[] bytes, int count)
        {
            try
            {
                if (count == 0)
                {
                    Debug.LogError($"output 0");
                    return;
                }
                m_pSendPacket.Clear();
                m_pSendPacket.Push(System.BitConverter.GetBytes(KcpClient.MSG), 0, 1);
                m_pSendPacket.Push(System.BitConverter.GetBytes(ClientConn), 0, 8);
                m_pSendPacket.Push(System.BitConverter.GetBytes(ServerConn), 0, 4);
                m_pSendPacket.Push(bytes, 0, count);
                int result = this.m_pSocket.SendTo(m_pSendPacket.Buffers, 0, m_pSendPacket.Length, SocketFlags.None, this.m_remoteEndPoint);
                if (m_OnCallback != null) m_OnCallback(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketCantSend);
            }
        }
#else
        public void Output(IntPtr bytes, int count)
        {
            try
            {
                if (!m_isConnected || ServerConn == 0)
                    return;
                if (count == 0)
                {
                    Debug.LogError($"output 0");
                    return;
                }
                m_pSendPacket.Clear();
                if (m_arrSendTempBuffer == null || m_arrSendTempBuffer.Length < count)
                    m_arrSendTempBuffer = new byte[count];
                System.Runtime.InteropServices.Marshal.Copy(bytes, m_arrSendTempBuffer, 0, count);
                m_pSendPacket.Push(System.BitConverter.GetBytes(KcpClient.MSG), 0, 1);
                m_pSendPacket.Push(System.BitConverter.GetBytes(ClientConn), 0, 8);
                m_pSendPacket.Push(System.BitConverter.GetBytes(ServerConn), 0, 4);
                m_pSendPacket.Push(m_arrSendTempBuffer, 0, count);
           //     Debug.Log("ClientConn:" + ClientConn + "   ServerConn:" + ServerConn);
                int result = this.m_pSocket.SendTo(m_pSendPacket.Buffers, 0, m_pSendPacket.Length, SocketFlags.None, this.m_remoteEndPoint);
                if (m_OnCallback != null) m_OnCallback(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.OnError(ERR_SocketCantSend);
            }
        }
#endif

        public void SetOutput()
        {
            // 跟上一行一样写法，pc跟linux会出错, 保存防止被GC
#if USE_CSKCP
            m_pKcpOutput = KcpOutput;
            m_pKcpPtr.SetOutput(KcpOutput);
#else
            m_pKcpOutput = KcpOutput;
            Core.KCPUtil.KcpSetoutput(m_pKcpPtr,m_pKcpOutput);
#endif
        }

#if USE_CSKCP
        void KcpOutput(byte[] bytes, int len, object user)
        {
            Output(bytes, len);
        }
#else
        [AOT.MonoPInvokeCallback(typeof(Core.KcpOutput))]
        public static int KcpOutput(IntPtr bytes, int len, IntPtr kcp, IntPtr user)
        {
            if (!ms_KcpNetwork.TryGetValue(kcp, out var netWork) || netWork == null)
                return len;
            if (netWork.GetSession(ESessionType.Kcp) == null)
                return len;
            var session = netWork.GetSession(ESessionType.Kcp, user.ToInt32()) as KcpServerSession;
            session.GetClient().Output(bytes, len);
            return len;
        }
#endif

        private void KcpSend(byte[] buffers, int offset, int length)
        {
#if USE_CSKCP
            m_pKcpPtr.Send(buffers, offset, length);
#else
            Core.KCPUtil.KcpSend(this.m_pKcpPtr, buffers, offset, length);
#endif
        }

        public void Send(byte[] buffer, int index, int length)
        {
#if USE_CSKCP
            if (m_pKcpPtr == null) return;
#else
            if (this.m_pKcpPtr == IntPtr.Zero)
            {
                return;
            }
#endif
            if (m_isConnected)
            {
                this.KcpSend(buffer, index, length);
                return;
            }
            m_pSendPacket.Push(buffer, index, length);
        }

        public void Send(MemoryStream stream)
        {
#if USE_CSKCP
            if(m_pKcpPtr!=null)
            {
                // 检查等待发送的消息，如果超出两倍窗口大小，应该断开连接
                if (m_pKcpPtr.WaitSnd() > 256 * 2)
                {
                    this.OnError(ERR_KcpWaitSendSizeTooLarge);
                    return;
                }
            }
#else
            if (this.m_pKcpPtr != IntPtr.Zero)
            {
                // 检查等待发送的消息，如果超出两倍窗口大小，应该断开连接
                if (Core.KCPUtil.KcpWaitsnd(this.m_pKcpPtr) > 256 * 2)
                {
                    this.OnError(ERR_KcpWaitSendSizeTooLarge);
                    return;
                }
            }
#endif

            ushort size = (ushort)(stream.Length - stream.Position);
            byte[] bytes = stream.GetBuffer();
            Send(bytes, 0, size);
        }
    }
    public class KcpServerSession : AServerSession
    {
        System.Random m_pRandom = new System.Random();
        private Socket m_pSocket;
        KcpClient m_pClient = null;

        private long m_nLocalConnID = 0;

        private EndPoint m_ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        long m_lStartTime = 0;
        public uint TimeNow { get; private set; }

        public KcpServerSession(ANetWork netWork, uint nReciveMaxSize, uint nSendMaxSize, uint nProcessCount, uint nTryConnectCount,int index=0)
            : base(netWork, nReciveMaxSize, nSendMaxSize, nProcessCount, nTryConnectCount, index)
        {
            m_lStartTime = Framework.Base.TimerUtil.ClientNow();
            TimeNow = (uint)(Framework.Base.TimerUtil.ClientNow() - this.m_lStartTime);
        }
        //------------------------------------------------------
        protected override void Close()
        {
            if (m_pClient != null) m_pClient.Dispose();
            if (m_pSocket != null) m_pSocket.Close();
        }
        //------------------------------------------------------
        public KcpClient GetClient()
        {
            return m_pClient;
        }
        //------------------------------------------------------
        public override void SetLocalConnID(long localConnID)
        {
            m_nLocalConnID = localConnID;
        }
        //------------------------------------------------------
        protected override void OnConnect()
        {
            if (m_pClient != null && m_pClient.isConnected) m_pClient.Dispose();
            if (m_pSocket != null) m_pSocket.Close();
            m_pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_pSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            m_eState = ESessionState.Connecting;
            ConnectingTimeBreak(10);
            long localConn = m_nLocalConnID;
            if(localConn == 0) localConn = m_pRandom.Next(1000, int.MaxValue);
            m_pClient = new KcpClient(this, OnPacketHander, localConn, m_pSocket, new IPEndPoint(m_IpAddress[0], m_nPort), OnKcpCallback);
        }
        //------------------------------------------------------
        void OnKcpCallback(int error)
        {
            if(error == 0)
                ChangeState(ESessionState.Connected);
            else if (error == -1)
                ChangeState(ESessionState.Disconnected);
        }
        //------------------------------------------------------
        protected override void OnReConnect()
        {
            Framework.Base.Logger.Info("try reconect count:" + m_nReconnectTryCnt);
            if (m_pClient == null)
            {
                m_eState = ESessionState.Connecting;
                m_fConnectingBreakTime = 0;
                uint localConn = (uint)m_pRandom.Next(1000, int.MaxValue);
                m_pClient = new KcpClient(this, OnPacketHander, localConn, m_pSocket, new IPEndPoint(m_IpAddress[0], m_nPort), OnKcpCallback);
            }
            else
                m_eState = ESessionState.Connecting;
            m_pNetWork.OnReConnect();


            m_bDisconnected = false;
        }
        //------------------------------------------------------
        protected override void OnDisconnect()
        {
            m_pClient = null;
            m_pSocket = null;
        }
        //------------------------------------------------------
        protected override void OnSendBuffer(byte[] pBuff, int nOffset, int nSize)
        {
            if (m_pClient == null) return;
            m_pClient.Send(pBuff, nOffset, nSize);
        }
        //------------------------------------------------------
        protected override bool OnReadBuffer(ref byte[] reciveBuffer, int nOffset, ref int nSize)
        {
            if (m_pSocket == null) return false;
            if(m_pSocket!=null && m_pSocket.Available>0)
            {
                try
                {
                    int bytesRead = m_pSocket.ReceiveFrom(reciveBuffer, ref m_ipEndPoint);
                    if (bytesRead > 0)
                    {
                        byte flag = reciveBuffer[0];
                        switch(flag)
                        {
                            case KcpClient.MSG:
                                this.m_pClient.HandleRecv(reciveBuffer, 9, bytesRead-9);
                                break;
                            default:
                                {
                                    this.m_pClient.OnReciveSocket(reciveBuffer, bytesRead);
                                    return false;
                                }
                        }
                        nSize = bytesRead;
                        return true;
                    }

                }
                catch (Exception e)
                {
                    m_pSocket.Close();
                    m_pSocket = null;
                    m_pClient.Dispose();
                    m_pClient = null;
                    Debug.LogError(e);
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
            if(m_eState == ESessionState.Connecting)
            {
                if (m_pClient != null && !m_pClient.isConnected)
                {
                    if (!m_pRecvWorker.IsStartUp()) m_pRecvWorker.SuspendedUpdate();
                }
            }
        }
        //------------------------------------------------------
        protected override bool OnInnerNetThreadUpdate()
        {
            TimeNow = (uint)(Framework.Base.TimerUtil.ClientNow() - m_lStartTime);
            if (m_pClient != null) m_pClient.Update();
            return true;
        }
    }
}