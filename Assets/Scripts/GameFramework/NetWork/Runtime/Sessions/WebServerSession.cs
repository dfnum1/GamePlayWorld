/********************************************************************
生成日期:	10:7:2019   15:29
类    名: 	WebServerSession
作    者:	HappLI
描    述:	
*********************************************************************/
/*
using UnityEngine;
using System;
using NativeWebSocket;
namespace Framework.Net
{
    public class WebServerSession : AServerSession
    {
        private WebSocket m_pWebSocket = null;
        private bool m_bConnected = false;
        private bool m_bConnecting = false;
        private byte[] m_RecvBuffer = new byte[1024 * 64];
        private int m_RecvSize = 0;
        public WebServerSession(ANetWork netWork, uint nReciveMaxSize, uint nSendMaxSize, uint nProcessCount, uint nTryConnectCount, int index = 0)
            : base(netWork, nReciveMaxSize, nSendMaxSize, nProcessCount, nTryConnectCount, index)
        {

        }
        //------------------------------------------------------
        protected override void Close()
        {
            if (m_pWebSocket != null)
            {
                m_pWebSocket.OnOpen -= OnWebSocketOpen;
                m_pWebSocket.OnError -= OnWebSocketError;
                m_pWebSocket.OnClose -= OnWebSocketClose;
                m_pWebSocket.OnMessage -= OnWebSocketMessage;
                m_pWebSocket.Close();
                m_pWebSocket = null;
            }
            m_bConnected = false;
            m_bConnecting = false;
        }
        //------------------------------------------------------
        void DoTimeoutListener(IAsyncResult pAsyncResult)
        {
            if (m_nTimeoutDelta > 0)
            {
            }
        }
        //------------------------------------------------------
        protected override void OnConnect()
        {
            if (m_pWebSocket != null)
            {
                m_pWebSocket.Close();
                m_pWebSocket = null;
            }
            m_bConnected = false;
            m_bConnecting = true;

            string url = $"ws://{m_Ip}:{m_nPort}/";
            m_pWebSocket = new WebSocket(url);

            m_pWebSocket.OnOpen += OnWebSocketOpen;
            m_pWebSocket.OnError += OnWebSocketError;
            m_pWebSocket.OnClose += OnWebSocketClose;
            m_pWebSocket.OnMessage += OnWebSocketMessage;

            // NativeWebSocket是异步连接
            ConnectAsync();
        }
        //------------------------------------------------------
        private async void ConnectAsync()
        {
            try
            {
                await m_pWebSocket.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError("WebSocket Connect Error: " + ex);
                m_bConnecting = false;
                ChangeState(ESessionState.TryConnect);
            }
        }
        //------------------------------------------------------
        void OnWebSocketOpen()
        {
            m_bConnected = true;
            m_bConnecting = false;
            ChangeState(ESessionState.Connected);
        }
        //------------------------------------------------------
        void OnWebSocketError(string error)
        {
            Debug.LogError("WebSocket Error: " + error);
            m_bConnected = false;
            m_bConnecting = false;
            ChangeState(ESessionState.TryConnect);
        }
        //------------------------------------------------------
        void OnWebSocketClose(WebSocketCloseCode code)
        {
            m_bConnected = false;
            m_bConnecting = false;
            ChangeState(ESessionState.Disconnected);
        }
        //------------------------------------------------------
        void OnWebSocketMessage(byte[] bytes)
        {
            // 收到消息，写入缓冲区
            int copySize = Mathf.Min(m_RecvBuffer.Length, bytes.Length);
            Buffer.BlockCopy(bytes, 0, m_RecvBuffer, 0, copySize);
            m_RecvSize = copySize;

            // 这里可直接解析为 PacketBase 并调用 OnPacketHander
            // PacketBase packet = new PacketBase();
            // packet.Fill(m_RecvBuffer, m_RecvSize);
            // OnPacketHander(packet);
        }
        //------------------------------------------------------
        void OnConnectThread()
        {
        }
        //------------------------------------------------------
        protected override void OnReConnect()
        {
            OnConnect();
        }
        //------------------------------------------------------
        protected override void OnDisconnect()
        {
            if (m_pWebSocket != null)
            {
                m_pWebSocket.Close();
                m_pWebSocket = null;
            }
            m_bConnected = false;
            m_bConnecting = false;
        }
        //------------------------------------------------------
        protected override void OnSendBuffer(byte[] pBuff, int nOffset, int nSize)
        {
            if (m_pWebSocket != null && m_bConnected)
            {
                byte[] sendData = new byte[nSize];
                Buffer.BlockCopy(pBuff, nOffset, sendData, 0, nSize);
                SendAsync(sendData);
            }
        }
        //------------------------------------------------------
        private async void SendAsync(byte[] data)
        {
            try
            {
                await m_pWebSocket.Send(data);
            }
            catch (Exception ex)
            {
                Debug.LogError("WebSocket Send Error: " + ex);
            }
        }
        //------------------------------------------------------
        protected override bool OnReadBuffer(ref byte[] reciveBuffer, int nOffset, ref int nSize)
        {
            if (m_RecvSize > 0)
            {
                int copySize = Mathf.Min(reciveBuffer.Length, m_RecvSize);
                Buffer.BlockCopy(m_RecvBuffer, 0, reciveBuffer, nOffset, copySize);
                nSize = copySize;
                m_RecvSize = 0;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        protected override void OnPacketHander(PacketBase pPacket)
        {
            m_vReviceMsgQueue.Add(pPacket);
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
        }
        //------------------------------------------------------
        private static void OnConnectCallback(IAsyncResult pAsynResult)
        {
        }
    }
}
*/