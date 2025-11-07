/********************************************************************
生成日期:	10:7:2019   16:47
类    名: 	PacketWorker
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
namespace Framework.Net
{
    public class PacketRecvWorker : PacketWorker
    {
        public delegate bool ReciveEvent(ref byte[] pReciveBuffer, int nOffset, ref int nSize);

        byte[]          m_pReciveCompressBuff = null;
        ReciveEvent m_pReciveEvent = null;
        //------------------------------------------------------
        public PacketRecvWorker(ANetWork pNetwork, uint nMaxSize ) : base(pNetwork, nMaxSize)
        {
            m_pReciveCompressBuff = new byte[nMaxSize];
        }
        //------------------------------------------------------
        public void SetReciveEvent(ReciveEvent pEvent)
        {
            m_pReciveEvent = pEvent;
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrameTime)
        {
            lock(m_pMuxLock)
            {
                if (m_PacketEvent != null)
                {
                    //! 包缓存中有多少数据就解析多少 
                    while (m_pPacketBytes.Length >= PacketBase.PACKETHEADLEN)
                    {
                        int combine = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(m_pPacketBytes.Buffers, m_pPacketBytes.Offset));
                        int protoLength = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(m_pPacketBytes.Buffers, m_pPacketBytes.Offset + 4));
                        if (protoLength < 0)
                        {
                            break;
                        }
                        int code = combine >> 16;
                        //    Framework.Plugin.Logger.Error("Code=" + code + "Length=" + m_pRecivePacket.Length + "protoLength=" + protoLength);
                        if (protoLength > m_pPacketBytes.Length)
                        {
#if UNITY_EDITOR
                            Framework.Base.Logger.Warning("解析包体太大,分包解析:Code=" + code);
#endif

                            //! 清理包
                            //       m_pRecivePacket.Clear();
                            break;
                        }

                        m_pPacketBytes.Pop(null, 0, PacketBase.PACKETHEADLEN);

                        int flag = (combine & 0xffff);

                        bool bCompress = (flag & 1) != 0;
                        bool bEncrypt = (flag & (1 << 1)) != 0;

                        if (bCompress)
                        {
                            m_pPacketBytes.Pop(m_pReciveCompressBuff, 0, protoLength);
                            protoLength = UnCompress(ref m_pReciveCompressBuff, ref m_pBuff, protoLength);
                            if (protoLength <= 0)
                            {
                                Framework.Base.Logger.Warning("un compress fail!!");
                                continue;
                            }
                        }
                        else
                        {
                            m_pPacketBytes.Pop(m_pBuff, 0, protoLength);
                        }
                        if (bEncrypt) Decrypt(m_pBuff, protoLength);

                        PacketBase packet = new PacketBase();
                        packet.SetCode(code);
                        if (packet.Paser(m_pNetWork, m_pBuff, protoLength))
                        {
                            m_PacketEvent(packet);
                        }
                        else
                            Framework.Base.Logger.Warning("消息解析失败:Code=" + code);
                    }
                }
            }
            
        }
        //------------------------------------------------------
        protected override bool OnProgress()
        {
            //! 收包
            if (m_pReciveEvent != null)
            {
                lock(m_pMuxLock)
                {
                    int buffLen = 0;
                    if (m_pReciveEvent(ref m_pBuff, 0, ref buffLen))
                    {
                        // if (m_bEncrypt)
                        //    Decrypt(m_pReciveBuff, buffLen);

                        m_pPacketBytes.Push(m_pBuff, 0, buffLen);
                    }
                }

            }
            return m_bStartUp;
        }
        //------------------------------------------------------
        public override int GetJob()
        {
            return 1;
        }
    }
}
