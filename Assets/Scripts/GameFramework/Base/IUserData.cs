/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	IUserData
作    者:	HappLI
描    述:	用户数据
*********************************************************************/
using System;
using System.Runtime.InteropServices;
using UnityEditor.VersionControl;
using UnityEngine;

public interface IUserData
{
    void Destroy();
}

namespace Framework.Core
{
    //------------------------------------------------------
    public struct VariableBuffer : IUserData
    {
        public byte[] buffers;
        public int bufferSize;

        public VariableBuffer(byte[] buffers, int bufferSize)
        {
            this.buffers = buffers;
            this.bufferSize = bufferSize;
        }
        public void Destroy()
        {
            buffers = null;
            bufferSize = 0;
        }
        public bool IsValid()
        {
            return buffers != null && bufferSize > 0;
        }
    }
    //------------------------------------------------------
    public struct VariableType : IUserData
    {
        public System.Type type;

        public void Destroy() { }
    }
    //------------------------------------------------------
    public struct VariableCallback : IUserData
    {
        public System.Action callback;

        public void Destroy() { }
    }
    //------------------------------------------------------
    public struct VariableCallback1 : IUserData
    {
        public System.Delegate callback;
        public void Invoke<T>(T userData) where T : IUserData
        {
            if (callback != null && callback is System.Action<T>)
            {
                ((System.Action<T>)callback)(userData);
            }
        }
        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct VariableByte : IUserData
    {
        [FieldOffset(0)]
        public byte byteVal;
        [FieldOffset(0)]
        public bool boolVal;

        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct VariableUint : IUserData
    {
        [FieldOffset(0)]
        public uint uintVal;

        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct Variable1 : IUserData
    {
        [FieldOffset(0)]
        public int intVal;
        [FieldOffset(0)]
        public uint uintVal;
        [FieldOffset(0)]
        public float floatVal;
        [FieldOffset(0)]
        public bool boolVal;

        [FieldOffset(0)]
        public ushort shortVal0;
        [FieldOffset(2)]
        public ushort shortVal1;
        [FieldOffset(2)]
        public byte byteVal0;
        [FieldOffset(3)]
        public byte byteVal1;
        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct Variable2 : IUserData
    {
        [FieldOffset(0)]
        public int intVal0;
        [FieldOffset(0)]
        public float floatVal0;

        [FieldOffset(4)]
        public int intVal1;
        [FieldOffset(4)]
        public float floatVal1;

        [FieldOffset(0)]
        public short shortVal0;
        [FieldOffset(2)]
        public short shortVal1;
        [FieldOffset(4)]
        public short shortVal2;
        [FieldOffset(6)]
        public short shortVal3;

        [FieldOffset(0)]
        public long longValue;

        public Vector2 ToVector2()
        {
            return new Vector2(floatVal0, floatVal1);
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(intVal0, intVal1);
        }
        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct Variable3 : IUserData
    {
        [FieldOffset(0)]
        public int intVal0;
        [FieldOffset(0)]
        public float floatVal0;

        [FieldOffset(4)]
        public int intVal1;
        [FieldOffset(4)]
        public float floatVal1;

        [FieldOffset(8)]
        public int intVal2;
        [FieldOffset(8)]
        public float floatVal2;

        [FieldOffset(0)]
        public long longValue;

        public Vector3 ToVector3()
        {
            return new Vector3(floatVal0, floatVal1, floatVal2);
        }
        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(intVal0, intVal1, intVal2);
        }
        public Variable3(Vector3 val)
        {
            longValue = 0;
            intVal0 = intVal1 = intVal2 = 0;
            floatVal0 = val.x;
            floatVal1 = val.y;
            floatVal2 = val.z;
        }
        public void Destroy() { }
    }
    //------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct Variable4 : IUserData
    {
        [FieldOffset(0)]
        public int intVal0;
        [FieldOffset(0)]
        public float floatVal0;

        [FieldOffset(4)]
        public int intVal1;
        [FieldOffset(4)]
        public float floatVal1;

        [FieldOffset(8)]
        public int intVal2;
        [FieldOffset(8)]
        public float floatVal2;

        [FieldOffset(12)]
        public int intVal3;
        [FieldOffset(12)]
        public float floatVal3;

        [FieldOffset(0)]
        public long longValue0;

        [FieldOffset(8)]
        public long longValue1;

        public Vector3 ToVector3()
        {
            return new Vector3(floatVal0, floatVal1, floatVal2);
        }
        public Vector4 ToVector4()
        {
            return new Vector4(floatVal0, floatVal1, floatVal2, floatVal3);
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(floatVal0, floatVal1, floatVal2, floatVal3);
        }
        public Color ToColor()
        {
            return new Color(floatVal0, floatVal1, floatVal2, floatVal3);
        }

        public Vector2 ToVector2_0()
        {
            return new Vector2(floatVal0, floatVal1);
        }
        public Vector4 ToVector2_1()
        {
            return new Vector2(floatVal2, floatVal3);
        }
        public void Destroy() { }
    }
    //------------------------------------------------------
    public struct VariableString : IUserData
    {
        public string strValue;
        public void Destroy() { strValue = null; }
    }
    //------------------------------------------------------
    public struct VariableTransform : IUserData
    {
        public Transform pTransform;
        public void Destroy() { pTransform = null; }
    }
    //------------------------------------------------------
    public struct VariableGO : IUserData
    {
        public GameObject pGO;
        public void Destroy() { pGO = null; }
    }
    //------------------------------------------------------
    public struct VariableObj : IUserData
    {
        public UnityEngine.Object pGO;
        public void Destroy() { pGO = null; }
    }
    //------------------------------------------------------
    public struct VariableUserData : IUserData
    {
        public IUserData pUserData;
        public void Destroy() { pUserData = null; }
    }
    //------------------------------------------------------
    public struct VariableMultiData : IUserData
    {
        public IUserData pData1;
        public IUserData pData2;
        public IUserData pData3;
        public IUserData pData4;
        public void Destroy() { pData1 = pData2 = pData3 = pData4 = null; }
    }
}