/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	BinaryUtil
作    者:	HappLI
描    述:	二进制数据读写
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;
using UnityEngine.Windows;
#if USE_SERVER
using AnimationCurve = ExternEngine.AnimationCurve;
using Keyframe = ExternEngine.Keyframe;
#endif
namespace Framework.Data
{
    public  struct BinaryUtil
    {
        byte[] m_Buffer;
        int m_nBufferSize;
        int m_nCurPos;
        public void Set(byte[] buffer, int size, string key = null)
        {
            m_Buffer = buffer;
            m_nBufferSize = size;
            m_nCurPos = 0;
            if(!string.IsNullOrEmpty(key) && size>0)
            {
                var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);
                for (int i =0; i < size; ++i)
                {
                    m_Buffer[i] = (byte)(m_Buffer[i] ^ key_bytes[i % key_bytes.Length]);
                }
            }
        }
        //------------------------------------------------------
        public bool IsValid(int size =0)
        {
            return m_Buffer != null && m_nBufferSize > 0 && (m_nCurPos+ size) <= m_nBufferSize;
        }
        //------------------------------------------------------
        public byte[] GetBuffer()
        {
            return m_Buffer;
        }
        //------------------------------------------------------
        public int GetCur()
        {
            return m_nCurPos;
        }
        //------------------------------------------------------
        public void Seek(int cur)
        {
            if (cur >= m_nBufferSize) return;
            m_nCurPos = cur;
        }
        //------------------------------------------------------
        public void SeekBegin()
        {
            m_nCurPos = 0;
        }
        //------------------------------------------------------
        public void SeekEnd()
        {
            m_nCurPos = m_nBufferSize;
        }
        //------------------------------------------------------
        public bool ToBool(bool def = false)
        {
            if (!IsValid(1)) return def;
            def = m_Buffer[m_nCurPos] != 0;
            m_nCurPos += 1;
            return def;
        }
        //------------------------------------------------------
        public char ToChar(char def = ' ')
        {
            if (!IsValid(1)) return def;
            def = (char)BitConverter.ToChar(m_Buffer, m_nCurPos);
            m_nCurPos += 1;
            return def;
        }
        //------------------------------------------------------
        public byte ToByte(byte def = 0)
        {
            if (!IsValid(1)) return def;
            def = m_Buffer[m_nCurPos];
            m_nCurPos += 1;
            return def;
        }
        //------------------------------------------------------
        public short ToShort(short def = 0)
        {
            if (!IsValid(2)) return def;
            def = (short)BitConverter.ToInt16(m_Buffer, m_nCurPos);
            m_nCurPos += 2;
            return def;
        }
        //------------------------------------------------------
        public ushort ToUshort(ushort def = 0)
        {
            if (!IsValid(2)) return def;
            def = (ushort)BitConverter.ToUInt16(m_Buffer, m_nCurPos);
            m_nCurPos += 2;
            return def;
        }
        //------------------------------------------------------
        public int ToInt32(int def = 0)
        {
            if (!IsValid(4)) return def;
            def = BitConverter.ToInt32(m_Buffer, m_nCurPos);
            m_nCurPos += 4;
            return def;
        }
        //------------------------------------------------------
        public uint ToUint32(uint def = 0)
        {
            if (!IsValid(4)) return def;
            def = BitConverter.ToUInt32(m_Buffer, m_nCurPos);
            m_nCurPos += 4;
            return def;
        }
        //------------------------------------------------------
        public float ToFloat(float def = 0)
        {
            if (!IsValid(4)) return def;
            def = BitConverter.ToSingle(m_Buffer, m_nCurPos);
            m_nCurPos += 4;
            return def;
        }
        //------------------------------------------------------
        public double ToDouble(double def = 0)
        {
            if (!IsValid(8)) return def;
            def = BitConverter.ToDouble(m_Buffer, m_nCurPos);
            m_nCurPos += 8;
            return def;
        }
        //------------------------------------------------------
        public long ToInt64(long def = 0)
        {
            if (!IsValid(8)) return def;
            def = BitConverter.ToInt64(m_Buffer, m_nCurPos);
            m_nCurPos += 8;
            return def;
        }
        //------------------------------------------------------
        public ulong ToUint64(ulong def = 0)
        {
            if (!IsValid(8)) return def;
            def = BitConverter.ToUInt64(m_Buffer, m_nCurPos);
            m_nCurPos += 8;
            return def;
        }
        //------------------------------------------------------
        public string ToString(string def = null, int charLen = 2)
        {
            if (!IsValid(charLen)) return def;
            ushort len = 0;
            if(charLen == 2) len = BitConverter.ToUInt16(m_Buffer, m_nCurPos);
            else if(charLen == 4) len = (ushort)BitConverter.ToUInt32(m_Buffer, m_nCurPos);
            else len = (ushort)BitConverter.ToUInt64(m_Buffer, m_nCurPos);
            m_nCurPos += charLen;
            if (len <= 0 || !IsValid(len)) return def;
            def = System.Text.Encoding.UTF8.GetString(m_Buffer, m_nCurPos, len);
            m_nCurPos += len;
            return def;
        }
        //------------------------------------------------------
        public Color ToColor()
        {
            if (!IsValid(4)) return Color.white;
            uint val = ToUint32();
            return new Color(
                ((val & 0x00ff0000) >> 16) / 255f,
                ((val & 0x0000ff00) >> 8) / 255f,
                ((val & 0x000000ff)) / 255f,
                ((val & 0xff000000) >> 24) / 255f);
        }
        //------------------------------------------------------
        public Vector2 ToVec2()
        {
            if (!IsValid(8)) return Vector2.zero;
            return new Vector2(ToFloat(),ToFloat());
        }
        //------------------------------------------------------
        public Vector2Int ToVec2Int()
        {
            if (!IsValid(8)) return Vector2Int.zero;
            return new Vector2Int(ToInt32(), ToInt32());
        }
        //------------------------------------------------------
        public Vector3 ToVec3()
        {
            if (!IsValid(12)) return Vector3.zero;
            return new Vector3(ToFloat(), ToFloat(), ToFloat());
        }
        //------------------------------------------------------
        public Vector3Int ToVec3Int()
        {
            if (!IsValid(12)) return Vector3Int.zero;
            return new Vector3Int(ToInt32(), ToInt32(), ToInt32());
        }
        //------------------------------------------------------
        public Vector4 ToVec4()
        {
            if (!IsValid(16)) return Vector4.zero;
            return new Vector4(ToFloat(), ToFloat(), ToFloat(), ToFloat());
        }
        //------------------------------------------------------
        public Quaternion ToQuaternion()
        {
            if (!IsValid(16)) return Quaternion.identity;
            return new Quaternion(ToFloat(), ToFloat(), ToFloat(), ToFloat());
        }
        //------------------------------------------------------
        public Matrix4x4 ToMatrix4x4()
        {
            if (!IsValid(64)) return Matrix4x4.identity;
            Matrix4x4 mat = Matrix4x4.identity;
            for(int i =0; i< 16; ++i)
            {
                mat[i] = ToFloat();
            }
            return mat;
        }
        //------------------------------------------------------
        public AnimationCurve ToCurve()
        {
            if (!IsValid(2)) return null;
            int keys = (int)ToShort();
            if (keys <= 0) return null;
            AnimationCurve curve = new AnimationCurve();
            for(int i = 0; i < keys; ++i)
            {
                Keyframe frame = new Keyframe();
                frame.time = ToFloat();
                frame.value = ToFloat();
                frame.inTangent = ToFloat();
                frame.outTangent = ToFloat();
                frame.inWeight = ToFloat();
                frame.outWeight = ToFloat();
                frame.weightedMode = (WeightedMode)ToByte();
                curve.AddKey(frame);
            }
            return curve;
        }
        //------------------------------------------------------
        void WriteBuffer(byte[] buffer, int length =-1)
        {
            if (length <= 0) length = buffer.Length;
            if (m_nCurPos + length > m_nBufferSize)
            {
                int nNewSize = 1024;
                while (nNewSize < m_nCurPos + length)
                    nNewSize *= 2;
                byte[] temp = new byte[nNewSize];
                if(m_Buffer!=null && m_nBufferSize>0)
                    Array.Copy(m_Buffer, temp, m_Buffer.Length);

                m_Buffer = temp;
                m_nBufferSize = temp.Length;
            }
            Array.Copy(buffer,0, m_Buffer, m_nCurPos, length);
            m_nCurPos += length;
        }
        //------------------------------------------------------
        public void WriteBuffers(byte[] buffer, int size=-1)
        {
            if (size <= 0) size = buffer.Length;
            WriteBuffer(buffer, size);
        }
        //------------------------------------------------------
        public void WriteBool(bool def)
        {
            WriteBuffer(new byte[] { (byte)(def?1:0) });
        }
        //------------------------------------------------------
        public void WriteChar(char def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteByte(byte def)
        {
            WriteBuffer(new byte[] { def });
        }
        //------------------------------------------------------
        public void WriteShort(short def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteUshort(ushort def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteInt32(int def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteUint32(uint def )
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteFloat(float def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteInt64(long def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteUint64(ulong def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteDouble(double def)
        {
            WriteBuffer(BitConverter.GetBytes(def));
        }
        //------------------------------------------------------
        public void WriteString(string def)
        {
            if(def == null || def.Length<=0)
            {
                WriteBuffer(BitConverter.GetBytes((ushort)0));
                return;
            }
            byte[] text = System.Text.Encoding.UTF8.GetBytes(def);
            ushort textLen = (ushort)text.Length;
            WriteBuffer(BitConverter.GetBytes(textLen));
            WriteBuffer(text);
        }
        //------------------------------------------------------
        public void WriteColor(Color col)
        {
            uint val = (((uint)(col.a * 255f) << 24) | ((uint)(col.r * 255f) << 16) | ((uint)(col.g * 255f) << 8) | (uint)(col.b * 255f));
            WriteUint32(val);
        }
        //------------------------------------------------------
        public void WriteVector2(Vector2 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
        }
        //------------------------------------------------------
        public void WriteVector2Int(Vector2Int vec)
        {
            WriteInt32(vec.x);
            WriteInt32(vec.y);
        }
        //------------------------------------------------------
        public void WriteVector3(Vector3 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
        }
        //------------------------------------------------------
        public void WriteVector3Int(Vector3Int vec)
        {
            WriteInt32(vec.x);
            WriteInt32(vec.y);
            WriteInt32(vec.z);
        }
        //------------------------------------------------------
        public void WriteVector4(Vector4 vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
            WriteFloat(vec.w);
        }
        //------------------------------------------------------
        public void WriteQuaternion(Quaternion vec)
        {
            WriteFloat(vec.x);
            WriteFloat(vec.y);
            WriteFloat(vec.z);
            WriteFloat(vec.w);
        }
        //------------------------------------------------------
        public void WriteCurve(AnimationCurve curve)
        {
            if (curve == null || curve.length <= 0)
            {
                WriteShort((short)0);
                return;
            }
            WriteShort((short)curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                Keyframe frame = curve.keys[i];
                WriteFloat(frame.time);
                WriteFloat(frame.value);
                WriteFloat(frame.inTangent);
                WriteFloat(frame.outTangent);
                WriteFloat(frame.inWeight);
                WriteFloat(frame.outWeight);
                WriteByte((byte)frame.weightedMode);
            }
        }
        //------------------------------------------------------
        public void Encrypt(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < m_nCurPos;++i)
            {
                m_Buffer[i] = (byte)(m_Buffer[i] ^ key_bytes[i% key_bytes.Length]);
            }
        }
        //------------------------------------------------------
        public void SaveTo(string strFile)
        {
            if (string.IsNullOrEmpty(strFile))
                return;
            if(!Directory.Exists(System.IO.Path.GetDirectoryName(strFile)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(strFile));
            }
            System.IO.FileStream fs = new System.IO.FileStream(strFile, System.IO.FileMode.OpenOrCreate);
            System.IO.BinaryWriter sw = new System.IO.BinaryWriter(fs, System.Text.Encoding.UTF8);
            sw.BaseStream.SetLength(0);
            sw.BaseStream.Position = 0;
            sw.Write(m_Buffer, 0, m_nCurPos);
            sw.Close();
        }
    }
}

