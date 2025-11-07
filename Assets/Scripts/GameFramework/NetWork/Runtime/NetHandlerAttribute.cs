/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	NetHandlerAttribute
作    者:	HappLI
描    述:	消息句柄
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework.Net
{
    public class NetHandlerAttribute : Attribute
    {
    }
    public class NetResponseAttribute : Attribute
    {
        public int mid;
        public NetResponseAttribute(int mid)
        {
            this.mid = mid;
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NetLockAttribute : Attribute
    {
        public int lockID;
        public int unlockID;
        public NetLockAttribute(int lockID, int unlockID)
        {
            this.lockID = lockID;
            this.unlockID = unlockID;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PacketBuilderCallAttribute : Attribute
    {
        public PacketBuilderCallAttribute()
        {
        }
    }
}
