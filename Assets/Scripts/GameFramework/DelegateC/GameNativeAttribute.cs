/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GameNativeAttribute
作    者:	HappLI
描    述:	c++ 代码自动化属性
*********************************************************************/
using System;

namespace Framework.Core
{
    class CppNativeAttribute : Attribute
    {
        public string filePath;
        public string className;
        public bool isStruct;
        public CppNativeAttribute(string filePath, string className, bool isStruct = true)
        {
            this.className = className;
            this.filePath = filePath;
            this.isStruct = isStruct;
        }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    class CppNativeHeadAttribute : Attribute
    {
        public string filePath;
        public CppNativeHeadAttribute(string filePath)
        {
            this.filePath = filePath;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    class CppCodeInParentAttribute : Attribute
    {
        public CppCodeInParentAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    class CppDiscardAttribute : Attribute
    {
        public CppDiscardAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    class CppNoSerializerAttribute : Attribute
    {
        public CppNoSerializerAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    class CppTypeAttribute : Attribute
    {
        public string type;
        public CppTypeAttribute(string type)
        {
            this.type = type;
        }
    }
}