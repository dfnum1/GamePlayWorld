/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	PackageStream
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Core
{
    public class PackageStream : Stream
    {
        AFileSystem m_pFileSystem = null;
        string m_strFile = null;
        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return true; } }

        public override bool CanWrite { get { return false; } }

        private long m_nLength = -1;
        public override long Length 
        {
            get
            {
                if (m_nLength < 0)
                {
                    if(m_pFileSystem != null)
                    {
                        m_nLength = (long)m_pFileSystem.GetFileSize(m_strFile, true);
                    }

                }
                return m_nLength;
            } 
        }

        long m_nPosition = 0;
        public override long Position 
        {
            get
            {
                return m_nPosition;
            }
            set
            {
                m_nPosition = value;
            }
        }
        //------------------------------------------------- 
        public override void Flush()
        {

        }
        //------------------------------------------------- 
        public PackageStream(AFileSystem fileSystem, string strFile)
        {
            m_pFileSystem = fileSystem;
            m_strFile = strFile;
            m_nPosition = 0;
            m_nLength = -1;
        }
        //------------------------------------------------- 
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_pFileSystem == null) return 0;
            int num = (int)(Length - m_nPosition);
            if (num > count)
                num = count;
            if (num <= 0) return 0;
            m_pFileSystem.ReadBuffer(m_strFile, buffer, num, offset, (int)m_nPosition, true);
            m_nPosition += num;
            return num;
        }
        //------------------------------------------------- 
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {  
                        m_nPosition = offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        long num2 = (this.m_nPosition + offset);
                        if (num2 < Length)
                            this.m_nPosition = num2;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        long num3 = Length + offset;
                        if(num3 < Length)
                            this.m_nPosition = num3;
                        break;
                    }
            }
            return m_nPosition;
        }
        //------------------------------------------------- 
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------- 
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}

