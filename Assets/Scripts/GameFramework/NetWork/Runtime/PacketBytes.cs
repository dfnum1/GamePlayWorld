/********************************************************************
生成日期:	10:7:2019   17:27
类    名: 	PacketBytes
作    者:	HappLI
描    述:	包体字节流
*********************************************************************/

using System;
using UnityEngine;

namespace Framework.Net
{
    public class PacketBytes
    {
        private int m_nHead;
        private int m_nTail;
        private int m_nSize;
        private byte[] m_pBuffer;

        public int      Length { get { return m_nSize; } } //有效长度
        public int      Offset { get { return m_nHead; } } //偏移
        public byte[]   Buffers { get { return m_pBuffer; } }
        public int      TotalLength { get { return m_pBuffer.Length; } }

        //------------------------------------------------------
        public PacketBytes(int nSize = 2048)
        {
            m_pBuffer = new byte[nSize];
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_nHead = 0;
            m_nTail = 0;
            m_nSize = 0;
        }
        //------------------------------------------------------
        private void SetCapacity(int nCapacity)
        {
            if (m_pBuffer != null && m_pBuffer.Length == nCapacity)
                return;
            byte[] newBuffer = new byte[nCapacity];

            if (m_nSize > 0)
            {
                if (m_nHead < m_nTail)
                {
                    Buffer.BlockCopy(m_pBuffer, m_nHead, newBuffer, 0, m_nSize);
                }
                else
                {
                    Buffer.BlockCopy(m_pBuffer, m_nHead, newBuffer, 0, m_pBuffer.Length - m_nHead);
                    Buffer.BlockCopy(m_pBuffer, 0, newBuffer, m_pBuffer.Length - m_nHead, m_nTail);
                }
            }

            m_nHead = 0;
            m_nTail = m_nSize;
            m_pBuffer = newBuffer;
        }
        //------------------------------------------------------
        public int Pop(byte[] pBuffer, int nOffset, int nSize)
        {
            if (m_nSize <= 0) return 0;
            if(pBuffer!=null) nSize = ReadBytes(pBuffer, nOffset, nSize);

            m_nHead = (m_nHead + nSize) % m_pBuffer.Length;
            m_nSize -= nSize;

            if (m_nSize == 0)
            {
                m_nHead = 0;
                m_nTail = 0;
            }
            return nSize;
        }
        //------------------------------------------------------
        public bool Push(byte[] pBuffer, int nOffset, int nSize)
        {
            if (nSize <= 0) return false;
            if ((m_nSize + nSize) > m_pBuffer.Length)
            {
                nSize = m_pBuffer.Length - m_nSize;
                return false;
            }

            if (m_nHead < m_nTail)
            {
                int rightLength = (m_pBuffer.Length - m_nTail);

                if (rightLength >= nSize)
                {
                    Buffer.BlockCopy(pBuffer, nOffset, m_pBuffer, m_nTail, nSize);
                }
                else
                {
                    Buffer.BlockCopy(pBuffer, nOffset, m_pBuffer, m_nTail, rightLength);
                    Buffer.BlockCopy(pBuffer, nOffset + rightLength, m_pBuffer, 0, nSize - rightLength);
                }
            }
            else
            {
                Buffer.BlockCopy(pBuffer, nOffset, m_pBuffer, m_nTail, nSize);
            }

            m_nTail = (m_nTail + nSize) % m_pBuffer.Length;
            m_nSize += nSize;
            return true;
        }
        //------------------------------------------------------
        public int PeekBytes(byte[] pBytes, int nOffset, int nSize)
        {
           return ReadBytes(pBytes, nOffset, nSize);
        }
        //------------------------------------------------------
        private int ReadBytes(byte[] pBytes, int nOffset, int nSize)
        {
            if (nSize == 0)
                return 0;

            if (nSize > m_nSize)
                nSize = m_nSize;


            if (m_nHead < m_nTail)
            {
                Buffer.BlockCopy(m_pBuffer, m_nHead, pBytes, nOffset, nSize);
            }
            else
            {
                int rightLength = (m_pBuffer.Length - m_nHead);

                if (rightLength >= nSize)
                {
                    Buffer.BlockCopy(m_pBuffer, m_nHead, pBytes, nOffset, nSize);
                }
                else
                {
                    Buffer.BlockCopy(m_pBuffer, m_nHead, pBytes, nOffset, rightLength);
                    Buffer.BlockCopy(m_pBuffer, 0, pBytes, nOffset + rightLength, nSize - rightLength);
                }
            }

            return nSize;
        }
    }
}
