/********************************************************************
生成日期:	10:7:2019   16:47
类    名: 	PacketWorker
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.Net
{
    public abstract class PacketWorker// : Framework.Module.IJobUpdate
    {
        public delegate void PacketEvent(PacketBase pPacket);

        protected ANetWork m_pNetWork = null;
        protected object   m_pMuxLock = null;
        protected const int       MAX_NET_SEED_SIZE = 8;
        protected PacketEvent     m_PacketEvent = null;

        protected PacketBytes     m_pPacketBytes = null;
        protected byte[]          m_pBuff = null;

        protected bool m_bStartUp = false;

        protected bool            m_bEncrypt = true;
        protected byte[]          m_pSeed = new byte[MAX_NET_SEED_SIZE];
        protected int             m_nSeedLen = 0;
        //------------------------------------------------------
        public PacketWorker(ANetWork pNetwork, uint nMaxSize )
        {
            m_pNetWork = pNetwork;
            m_pMuxLock = new object();
            m_pPacketBytes = new PacketBytes((int)nMaxSize);
            m_pBuff = new byte[nMaxSize];
        }
        //------------------------------------------------------
        public void SetPacketEvent(PacketEvent pEvent)
        {
            m_PacketEvent = pEvent;
        }
        //------------------------------------------------------
        public void SetSeed(uint nSeed)
        {
            if(nSeed == 0)
            {
                m_bEncrypt = false;
                return;
            }
            //Tip: 这里暂时是这样，之后移到c++ 处理
            nSeed = ((((nSeed & 0xFFFF0000) >> 16) & 0x0000FFFF) | (((nSeed & 0x0000FFFF) << 16) & 0xFFFF0000));
            int i = 0;
            for (; i < MAX_NET_SEED_SIZE; i++)
            {
                m_pSeed[i] = (byte)((nSeed % 255) + 1);
                nSeed /= 19;
            }
            m_nSeedLen = i;
            m_bEncrypt = true;
        }
        //------------------------------------------------------
        public int UnCompress(ref byte[] compressBuff, ref byte[] buff, int lenth)
        {
            //uint hash = 5381;
            //for (uint i = 0; i < lenth; i++)
            //    hash += (hash << 5) + (datas[i]);

            return Core.GameDelegate.DecompressLZ4(ref compressBuff[0], ref buff[0], lenth, buff.Length);

     //       return true;
        }
        //------------------------------------------------------
        public bool CanEncrypt()
        {
            return m_bEncrypt;
        }
        //------------------------------------------------------
        public void EnableEncrypt(bool bEncrypt)
        {
            m_bEncrypt = bEncrypt;
        }
        //------------------------------------------------------
        protected int Encrypt(byte[] pData, int nOffset, int nLen, int nIgnore, int nCuto = 0)
        {
            //Tip: 这里暂时是这样，之后移到c++ 处理
            int i = nIgnore;
            int k = nCuto % m_nSeedLen;
            for (; i < nLen; i++, k++)
            {
                if (k == m_nSeedLen) k = 0;
                pData[i+ nOffset] ^= m_pSeed[k];
            }

            return k;
        }
        //------------------------------------------------------
        protected void Decrypt(byte[] pPacket, int nLenth)
        {
            //Tip: 这里暂时是这样，之后移到c++ 处理
            int i = 0;
            int k = 0;
            for (; i < nLenth; i++, k++)
            {                
                if (k == m_nSeedLen) k = 0;
                pPacket[i] ^= m_pSeed[k];
            }
        }
        //------------------------------------------------------
        public bool IsStartUp()
        {
            return m_bStartUp;
        }
        //------------------------------------------------------
        public void StartUp()
        {
            if (m_bStartUp) return;
            m_bStartUp = true;
          //  Framework.Module.ModuleManager.getInstance().AddJob(this, 0);
        }
        //------------------------------------------------------
        public void Shutdown()
        {
            if (!m_bStartUp) return;
            m_bStartUp = false;
          //  Framework.Module.ModuleManager.getInstance().RemoveJob(this);
        }
        //------------------------------------------------------
        public void MainUpdate(float fFrameTime)
        {
            if(GetJob()<0)
            {
                OnUpdate(fFrameTime);
                OnProgress();
            }
            else
            {
                lock (m_pMuxLock)
                {
                    OnUpdate(fFrameTime);
                }
            }

        }
        //------------------------------------------------------
        protected abstract void OnUpdate(float fFrameTime);
        //------------------------------------------------------
        public abstract int GetJob();
        //------------------------------------------------------
        public bool JobUpdate(float fFrame, IUserData userData = null)
        {
            lock (m_pMuxLock)
            {
                if(m_bStartUp)
                    OnProgress();
            }
            return m_bStartUp;
        }
        //------------------------------------------------------
        protected abstract bool OnProgress();
        //------------------------------------------------------
        public void SuspendedUpdate()
        {
            if (m_bStartUp) return;
            OnUpdate(0);
            OnProgress();
        }
    }
}
