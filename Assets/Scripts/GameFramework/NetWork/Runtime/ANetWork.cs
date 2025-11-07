/********************************************************************
生成日期:	10:7:2019   11:49
类    名: 	NetWork
作    者:	HappLI
描    述:	网络消息
*********************************************************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using Framework.Core;

namespace Framework.Net
{
    public abstract partial class ANetWork : Core.AModule, Core.ILateUpdate, ISessionCallback
    {
        struct TimerLockMid
        {
            public int mid;
            public int lastTime;
        }
        const int REC_MAXBUFFERSIZE = 1024 * 1024*4;//4 Mb
        const int SND_MAXBUFFERSIZE = 1024 * 1024 * 2;//2 Mb

        private bool m_bMutiThread = true;
        System.Threading.Thread m_pReciveThread = null;
        System.Threading.Thread m_pSendThread = null;

        Coroutine m_pReciveCoroutine = null;
        Coroutine m_pSendCoroutine = null;

        protected NetHandler m_pHandlers = null;
        List<AServerSession>[] m_Sessions = new List<AServerSession>[(int)ESessionType.Count];
    
        float m_fLastSendMessageTime = 0;
        int m_nHeartReqType = 0;
        Google.Protobuf.IMessage m_HeartRequest = null;

        protected HashSet<int> m_vLockMsgs = new HashSet<int>();
        protected Dictionary<int, long> m_vTimerLockMsgs = new Dictionary<int, long>();
        //------------------------------------------------------
        public static ANetWork getInstance()
        {
            if (AFramework.mainFramework == null) return null;
            return AFramework.mainFramework.netWork;
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            for (int i = 0; i < m_Sessions.Length; ++i)
                m_Sessions[i] = new List<AServerSession>();
            m_pHandlers = new NetHandler(this);
            WebHandler.SetPosCallback(OnTextPackage);
            m_fLastSendMessageTime = 0;
            m_vTimerLockMsgs.Clear();
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                m_bMutiThread = false;
        }
        //------------------------------------------------------
        protected override void OnStart()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                m_bMutiThread = false;
            m_fLastSendMessageTime = 0;
            m_vLockMsgs.Clear();
            m_vTimerLockMsgs.Clear();
            m_nHeartReqType = 0;
            if(m_HeartRequest!=null)
            {
                m_nHeartReqType = GetNetProtoMessageCode(m_HeartRequest.GetType());
            }
        }
        //------------------------------------------------------
        public void EnableMutiThread(bool bMutiThread)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                bMutiThread = false;
            m_bMutiThread = bMutiThread;
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            Disconnect(ESessionType.Count);
            m_vLockMsgs.Clear();
        }
        //------------------------------------------------------
        AServerSession CreateSession(ESessionType type)
        {
            switch (type)
            {
                case ESessionType.Kcp:
                    {
                        KcpServerSession pKcpSession = new KcpServerSession(this, REC_MAXBUFFERSIZE, SND_MAXBUFFERSIZE, 64, 10, m_Sessions[(int)type].Count);
                        pKcpSession.Register(this);
                        pKcpSession.SetConnectTimeout(10000);
                        m_Sessions[(int)type].Add(pKcpSession);
                        return pKcpSession;
                    }
                case ESessionType.Tcp:
                    {

                        TcpServerSession pTcpSession = new TcpServerSession(this, REC_MAXBUFFERSIZE, SND_MAXBUFFERSIZE, 64, 10, m_Sessions[(int)type].Count);
                        pTcpSession.Register(this);
                        pTcpSession.SetConnectTimeout(10000);
                        m_Sessions[(int)type].Add(pTcpSession);
                        return pTcpSession;
                    }
            }
            return null;
        }
        //------------------------------------------------------
        public AServerSession GetSession(string ip, int port)
        {
            for (int i = 0; i < m_Sessions.Length; ++i)
            {
                for (int j = 0; j < m_Sessions[i].Count; ++j)
                {
                    var session = m_Sessions[i][j];
                    if (session.GetPort() == port && session.GetIp().CompareTo(ip) == 0)
                        return session;
                }
            }
            return null;
        }
        //------------------------------------------------------
        public AServerSession GetSession(string strAddress)
        {
            for (int i = 0; i < m_Sessions.Length; ++i)
            {
                for (int j = 0; j < m_Sessions[i].Count; ++j)
                {
                    var session = m_Sessions[i][j];
                    if (session.GetAddress().CompareTo(strAddress) == 0)
                        return session;
                }
            }
            return null;
        }
        //------------------------------------------------------
        public AServerSession GetSession(ESessionType type, int index=0)
        {
            int tpIndex = (int)type;
            if (tpIndex < 0 || tpIndex >= m_Sessions.Length) return null;
            if (index < 0 || m_Sessions[tpIndex] ==null || index >= m_Sessions[tpIndex].Count) return null;
            return m_Sessions[tpIndex][index];
        }
        //------------------------------------------------------
        public void RegisterSessionCallback(ISessionCallback pCallback, int index=-1)
        {
            if(index < 0)
            {
                List<AServerSession> sessions;
                for (int i = 0; i < m_Sessions.Length; ++i)
                {
                    sessions = m_Sessions[i];
                    if (sessions == null) continue;
                    for(int j =0; j < sessions.Count; ++j)
                        sessions[j].Register(pCallback);
                }
            }
            else
            {
                for (int i = 0; i < m_Sessions.Length; ++i)
                {
                    if (index >= 0 && index < m_Sessions[i].Count)
                    {
                        m_Sessions[i][index].Register(pCallback);
                    }
                }
            }

        }
        //------------------------------------------------------
        public void UnRegisterSessionCallback(ISessionCallback pCallback, int index = -1)
        {
            if (index < 0)
            {
                List<AServerSession> sessions;
                for (int i = 0; i < m_Sessions.Length; ++i)
                {
                    sessions = m_Sessions[i];
                    if (sessions == null) continue;
                    for (int j = 0; j < sessions.Count; ++j)
                        sessions[j].UnRegister(pCallback);
                }
            }
            else
            {
                for (int i = 0; i < m_Sessions.Length; ++i)
                {
                    if (index >= 0 && index < m_Sessions[i].Count)
                    {
                        m_Sessions[i][index].UnRegister(pCallback);
                    }
                }
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public AServerSession Connect(ESessionType type,string strIp, int nPort, System.Action<AServerSession> onCallback = null, long localConnID =0)
        {
            if (string.IsNullOrEmpty(strIp) || strIp.CompareTo("0.0.0.0") == 0 || strIp.CompareTo("127.0.0.1") == 0)
            {
                var ipAdress = NetworkHelper.GetHostAddress(System.Net.Dns.GetHostName());
                strIp = ipAdress.ToString();
            }

            m_vLockMsgs.Clear();
            var session = GetSession(strIp, nPort);
            if (session == null) session = CreateSession(type);
            session.SetLocalConnID(localConnID);
            if (session != null) session.Connect(strIp, nPort, onCallback);
            return session;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("ConnectAddress")]
        public AServerSession Connect(ESessionType type, string strAddress, System.Action<AServerSession> onCallback = null, long localConnID = 0)
        {
            if (string.IsNullOrEmpty(strAddress))
            {
                if (onCallback != null) onCallback(null);
                Debug.LogError("address is null !");
                return null;
            }
            int ipPort = strAddress.LastIndexOf(":");
            if (ipPort <= 0)
            {
                if (onCallback != null) onCallback(null);
                Debug.LogError(strAddress + " address parser error, not port!");
                return null;
            }
            int port = 0;
            if (!int.TryParse(strAddress.Substring(ipPort + 1, strAddress.Length - ipPort - 1), out port))
            {
                if (onCallback != null) onCallback(null);
                Debug.LogError(strAddress + " address port invalid");
                return null;
            }
            string ip = strAddress.Substring(0, ipPort);
            return Connect(type, ip, port, onCallback, localConnID);
        }
        //------------------------------------------------------
        public void Disconnect(ESessionType type,int index =0)
        {
            if(type == ESessionType.Count)
            {
                for(int i =0; i < m_Sessions.Length; ++i)
                {
                    var session = m_Sessions[i];
                    if (session == null) continue;
                    for (int j = 0; j < session.Count; ++j)
                        session[j].Disconnect();
                    session.Clear();
                }
            }
            else
            {
                var session = m_Sessions[(int)type];
                if (session!=null)
                {
                    if (index < 0)
                    {
                        for (int i = 0; i < session.Count; ++i)
                        {
                            session[i].Disconnect();
                        }
                        session.Clear();
                    }
                    else if (index >= 0 && index < session.Count)
                    {
                        session[index].Disconnect();
                    }
                }
            }
            m_vLockMsgs.Clear();
            m_vTimerLockMsgs.Clear();
        }
        //------------------------------------------------------
        public void ReqHttp(string url, System.Action<string> onCallback, byte nRepeat = 6, int timeout = 3)
        {
            WebHandler.ReqHttp(url, onCallback, nRepeat, timeout);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("发送消息包")]
        public bool SendMessage(PacketBase pMsg)
        {
            if (pMsg.GetCode() <= 0) return false;
            if (IsTimerLock(pMsg.GetCode())) return false;
            var session = GetSession(pMsg.GetSessionType(), pMsg.GetSessionIndex());
            if (session == null) return false;
            bool bSend = session.Send(pMsg);
            if (bSend)
            {
                m_fLastSendMessageTime = Framework.Base.TimerUtil.ClientNow();
            }
            LockPacket(pMsg.GetCode());
            if (GetFramework() != null)
            {
                OnCheckNetPackage(pMsg.GetCode(), false);
            }
            return bSend;
        }
        //------------------------------------------------------
        public bool SendMessage(ESessionType type, int nCode, Google.Protobuf.IMessage pMsg,bool isCheckNet = true)
        {
            var session = GetSession(type);
            if (session == null) return false;
            if (IsTimerLock(nCode)) return false;
            bool bSend = session.Send(nCode, pMsg);
            if (bSend && isCheckNet)
            {
                m_fLastSendMessageTime = Framework.Base.TimerUtil.ClientNow();
            }
            LockPacket(nCode);
            if (GetFramework() != null)
            {
                 OnCheckNetPackage(nCode, false);
            }
            return bSend;
        }
        //------------------------------------------------------
        public void SendMessage(int nCode, Google.Protobuf.IMessage pMsg, bool isCheckNet = true)
        {
            if (IsTimerLock(nCode)) return;
            var session = GetSession(ESessionType.Tcp);
            if (session == null) session = GetSession(ESessionType.Kcp);
            if (session == null) return;
            if (session.Send(nCode, pMsg) && isCheckNet)
            {
                m_fLastSendMessageTime = Framework.Base.TimerUtil.ClientNow();
            }
            LockPacket(nCode);
            if (GetFramework() != null)
            {
                OnCheckNetPackage(nCode, false);
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("发送心跳包")]
        public void SendHeart(float fHeartGap)
        {
            //if (GetMessageInterval() < fHeartGap) return;
            if (m_HeartRequest == null)
                return;
            var session = GetSession(ESessionType.Tcp);
            if (session != null)
            {
                if (session.GetState() == ESessionState.Connected)
                {
                    SendMessage(ESessionType.Tcp, m_nHeartReqType, m_HeartRequest);
                }
            }
            session = GetSession(ESessionType.Kcp);
            if (session != null)
            {
                if (session.GetState() == ESessionState.Connected)
                {
                    SendMessage(ESessionType.Kcp, m_nHeartReqType, m_HeartRequest);
                }
            }
        }
        //------------------------------------------------------
        public void OnSessionState(AServerSession session, ESessionState eState)
        {
            Framework.Base.Logger.Info(eState.ToString());
            if (GetFramework() != null)
            {
                GetFramework().OnNetSessionState(session, eState);
            }
            if (eState == ESessionState.Connected)
            {
                if(m_bMutiThread)
                {
                    if (m_pSendThread == null || !m_pSendThread.IsAlive)
                    {
                        m_pSendThread = new System.Threading.Thread(OnThreadProgressSend);
                        m_pSendThread.Start();
                    }
                    if (m_pReciveThread == null || !m_pReciveThread.IsAlive)
                    {
                        m_pReciveThread = new System.Threading.Thread(OnThreadProgressRecive);
                        m_pReciveThread.Start();
                    }
                }
                else
                {
                    if (m_pSendCoroutine != null) m_pFramework.EndCoroutine(m_pSendCoroutine);
                    m_pSendCoroutine = null;
                    if (m_pReciveCoroutine != null) m_pFramework.EndCoroutine(m_pReciveCoroutine);
                    m_pReciveCoroutine = null;

                    m_pSendCoroutine = m_pFramework.BeginCoroutine(OnCoroutineProgressSend());
                    m_pReciveCoroutine = m_pFramework.BeginCoroutine(OnCoroutineProgressRecive());
                }
            }
            else if (eState == ESessionState.Disconnected)
            {
                if(!HasSessionConnected())
                {
                    if (m_pSendThread != null) m_pSendThread.Abort();
                    m_pSendThread = null;
                    if (m_pReciveThread != null) m_pReciveThread.Abort();
                    m_pReciveThread = null;

                    if (m_pSendCoroutine != null) m_pFramework.EndCoroutine(m_pSendCoroutine);
                    m_pSendCoroutine = null;
                    if (m_pReciveCoroutine != null) m_pFramework.EndCoroutine(m_pReciveCoroutine);
                    m_pReciveCoroutine = null;
                }
            }
        }
        //------------------------------------------------------
        bool HasSessionConnected()
        {
            for (int i = 0; i < m_Sessions.Length; ++i)
            {
                for (int j = 0; j < m_Sessions[i].Count; ++j)
                {
                    if (m_Sessions[i][j] != null && m_Sessions[i][j].GetState() == ESessionState.Connected)
                        return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public ESessionState GetSessionState(ESessionType type,int index=0)
        {
            var session = GetSession(type,index);
            if (session == null) return ESessionState.None;
            return session.GetState();
        }
        //------------------------------------------------------
        public bool IsRegisterHandler(int nCode)
        {
            return true;
        }
        //------------------------------------------------------
        public void Register(int code, OnRevicePacketMsg onHandler)
        {
            m_pHandlers.Register(code, onHandler);
        }
        //------------------------------------------------------
        public void UnRegister(int code)
        {
            m_pHandlers.UnRegister(code);
        }
        //------------------------------------------------------
        public void OnPackage(PacketBase pMsg)
        {
            UnLockPacket(pMsg.GetCode());
            OnCheckNetPackage(pMsg.GetCode(), true, pMsg);
            m_pHandlers.OnPakckage(pMsg);
        }
        //------------------------------------------------------
        void OnTextPackage(string msgJson)
        {
            PacketBase pkg = MessagePool.ToPackage(this,msgJson);
            if (pkg.GetCode() > 0) OnPackage(pkg);
            else Debug.LogWarning("pb code is zero!");
        }
        //------------------------------------------------------
        public void LateUpdate(float fFrame)
        {
            for(int i =0; i < m_Sessions.Length; ++i)
            {
                var session = m_Sessions[i];
                if (session == null) continue;
                for (int j =0; j < session.Count; ++j)
                {
                    session[j].OnMainUpdate(fFrame);
                }
            } 
        }
        //------------------------------------------------------
        public void ResetReceiveMessageTime(float fReceiveMessageTime)
        { 
            m_fLastSendMessageTime = fReceiveMessageTime;
        }
        //------------------------------------------------------
        public float GetMessageInterval()
        {
            if (m_fLastSendMessageTime == 0)
                return 0;
            else
            {
                return Framework.Base.TimerUtil.ClientNow() - m_fLastSendMessageTime;
            }
        }
        //------------------------------------------------------
        public void SetSeed(uint nSeed)
        {
            for (int i = 0; i < m_Sessions.Length; ++i)
            {
                for(int j =0; j < m_Sessions[i].Count; ++j)
                    m_Sessions[i][j].SetSeed(nSeed);
            }
        }
        //------------------------------------------------------
        public void OnReConnect(IUserData userData = null)
        {
            if (GetFramework() != null)
            {
                GetFramework().OnNetReConnect(userData);
            }
            m_vLockMsgs.Clear();
        }
        //------------------------------------------------------
        private int ProcessNetUpdate(int type)
        {
            int job = 0;
            try
            {
                for (int i = 0; i < m_Sessions.Length; ++i)
                {
                    for (int j = 0; j < m_Sessions[i].Count; ++j)
                    {
                        if (m_Sessions[i][j]!=null && m_Sessions[i][j].OnNetThreadUpdate(type))
                            job++;
                    }
                }
            }
            catch/* (Exception ex)*/
            {
                job = 1;
            }
            return job;
        }
        //------------------------------------------------------
        private void OnThreadProgressRecive()
        {
            int job = 1;
            while (job > 0)
            {
                job = ProcessNetUpdate(1);
                if (job <= 0) break;
                System.Threading.Thread.Sleep(1);
            }
        }
        //------------------------------------------------------
        private void OnThreadProgressSend()
        {
            int job = 1;
            while(job>0)
            {
                job = ProcessNetUpdate(0);
                if (job <= 0) break;
                System.Threading.Thread.Sleep(1);
            }
        }
        //------------------------------------------------------
        private System.Collections.IEnumerator OnCoroutineProgressRecive()
        {
            int job = 1;
            while (job > 0)
            {
                job = ProcessNetUpdate(1);
                if (job <= 0) break;
                yield return Framework.Core.CoroutineSystem.WaitForSecond(0.1f);
            }
            yield break;
        }
        //------------------------------------------------------
        private System.Collections.IEnumerator OnCoroutineProgressSend()
        {
            int job = 1;
            while (job > 0)
            {
                job = ProcessNetUpdate(0);
                if (job <= 0) break;
                yield return Framework.Core.CoroutineSystem.WaitForSecond(0.1f);
            }
            yield break;
        }
        //------------------------------------------------------
        public bool IsMsgLock(int mid)
        {
            return m_vLockMsgs.Contains(mid);
        }
        //------------------------------------------------------
        public int GetLockSize()
        {
            return m_vLockMsgs.Count;
        }
        //------------------------------------------------------
        public bool IsTimerLock(int mid, long time = 30000000)
        {
            long lastTime;
            if (m_vTimerLockMsgs.TryGetValue(mid, out lastTime) && System.DateTime.Now.Ticks - lastTime <= time) return true;
            return false;
        }
        //------------------------------------------------------
        public virtual void OnCheckNetPackage(int msgCode, bool bRevice, IUserData pParam = null)
        {
        }
        //------------------------------------------------------
        public virtual void LockPacket(int mid)
        {
        }
        //------------------------------------------------------
        public virtual void UnLockPacket(int mid)
        {
        }
        //------------------------------------------------------
        public abstract int GetNetProtoMessageCode(System.Type type);
        public abstract Google.Protobuf.IMessage CreateMessageByCode(int code);
        public abstract Google.Protobuf.IMessage CreateMessageByCode(int code, byte[] pDatas, int nLenth, int offset = 0);
    }
}
