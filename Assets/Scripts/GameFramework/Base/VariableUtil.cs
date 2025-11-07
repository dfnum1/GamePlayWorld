/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	VariableUtil
作    者:	HappLI
描    述:	变量工具
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Framework.Core
{
    public static class VariableUtil
    {
        static public bool ToBool(this IUserData data)
        {
            if (data == null) return false;
            if (data is VariableByte) return ((VariableByte)data).boolVal;
            if (data is Variable1) return ((Variable1)data).boolVal;
            return false;
        }
        //------------------------------------------------------
        static public short ToShort(this IUserData data)
        {
            if (data == null) return 0;
            if (data is Variable1) return (short)((Variable1)data).shortVal0;
            else if (data is Variable2) return ((Variable2)data).shortVal0;
            return 0;
        }
        //------------------------------------------------------
        static public int ToInt(this IUserData data)
        {
            if (data == null) return 0;
            if (data is Variable1) return ((Variable1)data).intVal;
            else if (data is Variable2) return ((Variable2)data).intVal0;
            else if (data is Variable3) return ((Variable3)data).intVal0;
            return 0;
        }
        //------------------------------------------------------
        static public uint ToUInt(this IUserData data)
        {
            if (data == null) return 0xffffffff;
            if (data is Variable1) return ((Variable1)data).uintVal;
            else if (data is Variable2) return (uint)((Variable2)data).intVal0;
            else if (data is Variable3) return (uint)((Variable3)data).intVal0;
            return 0xffffffff;
        }
        //------------------------------------------------------
        static public float ToFloat(this IUserData data)
        {
            if (data == null) return 0;
            if (data is Variable1) return ((Variable1)data).floatVal;
            else if (data is Variable2) return ((Variable2)data).floatVal0;
            else if (data is Variable3) return ((Variable3)data).floatVal0;
            return 0;
        }
        //------------------------------------------------------
        static public float ToLong(this IUserData data)
        {
            if (data == null) return 0;
            if (data is Variable2) return ((Variable2)data).longValue;
            else if (data is Variable3) return ((Variable3)data).longValue;
            return 0;
        }
        //------------------------------------------------------
        static public Vector2 ToVec2(this IUserData data)
        {
            if (data == null) return Vector2.zero;
            if (data is Variable2)
            {
                Variable2 v3 = (Variable2)data;
                return new Vector2(v3.floatVal0, v3.floatVal1);
            }
            else if (data is Variable3)
            {
                Variable3 v3 = (Variable3)data;
                return new Vector2(v3.floatVal0, v3.floatVal1);
            }
            return Vector2.zero;
        }
        //------------------------------------------------------
        static public Vector2Int ToVec2Int(this IUserData data)
        {
            if (data == null) return Vector2Int.zero;
            if (data is Variable2)
            {
                Variable2 v3 = (Variable2)data;
                return new Vector2Int(v3.intVal0, v3.intVal1);
            }
            else if (data is Variable3)
            {
                Variable3 v3 = (Variable3)data;
                return new Vector2Int(v3.intVal0, v3.intVal1);
            }
            return Vector2Int.zero;
        }
        //------------------------------------------------------
        static public Vector3 ToVec3(this IUserData data)
        {
            if (data == null) return Vector3.zero;
            if (data is Variable3)
            {
                Variable3 v3 = (Variable3)data;
                return new Vector3(v3.floatVal0, v3.floatVal1, v3.floatVal2);
            }
            return Vector3.zero;
        }
        //------------------------------------------------------
        static public Vector3Int ToVec3Int(this IUserData data)
        {
            if (data == null) return Vector3Int.zero;
            if (data is Variable3)
            {
                Variable3 v3 = (Variable3)data;
                return new Vector3Int(v3.intVal0, v3.intVal1, v3.intVal2);
            }
            return Vector3Int.zero;
        }
        //------------------------------------------------------
        static public string ToString(this IUserData data)
        {
            if (data == null) return string.Empty;
            if (data is VariableString) return ((VariableString)data).strValue;
            return string.Empty;
        }
    }
}