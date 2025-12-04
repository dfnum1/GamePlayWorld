/********************************************************************
生成日期:		1:11:2020 10:06
类    名: 	BindSlotAttribute
作    者:	HappLI
描    述:	数据类型绑定
*********************************************************************/
using System;
using System.Reflection;

namespace Framework.DrawProps
{
    public class BindSlotAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type classType;
        public string method = "OnDrawFieldLineRow";
        MethodInfo m_method = null;
#endif
        public BindSlotAttribute(string method = null, System.Type classType = null)
        {
#if UNITY_EDITOR
            this.method = method;
            this.classType = classType;
#endif
        }

#if UNITY_EDITOR
        //-----------------------------------------------------
        internal System.Object OnSlotCollect(System.Object ownerData, System.Reflection.FieldInfo fieldInfo, System.Object parentData, System.Reflection.FieldInfo parentFieldInfo)
        {
            if (string.IsNullOrEmpty(method))
                return ownerData;
            if (m_method == null)
            {
                System.Type ownerType = classType;
                if (ownerType == null) ownerType = fieldInfo.DeclaringType;
                if (ownerType == null)
                    return ownerData;
                m_method = ownerType.GetMethod(method, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (m_method == null)
                {
                    m_method = ownerType.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                }
                if (m_method == null)
                    m_method = ownerType.GetMethod(method, BindingFlags.Public | BindingFlags.Instance);
                if (m_method == null)
                    m_method = ownerType.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            if (m_method == null)
                return ownerData;

            System.Object pCall = ownerData;
            if (m_method.IsStatic) pCall = null;
            if (m_method.ReturnType == ownerData.GetType())
            {
                if (m_method.GetParameters() == null || m_method.GetParameters().Length == 0)
                    ownerData = m_method.Invoke(pCall, null);
                else if (m_method.GetParameters().Length == 1)
                    ownerData = m_method.Invoke(pCall, new object[] { fieldInfo });
                else if (m_method.GetParameters().Length == 2)
                    ownerData = m_method.Invoke(pCall, new object[] { ownerData, fieldInfo });
                else if (m_method.GetParameters().Length == 3)
                    ownerData = m_method.Invoke(pCall, new object[] { ownerData, fieldInfo, parentData });
                else if (m_method.GetParameters().Length == 4)
                    ownerData = m_method.Invoke(pCall, new object[] { ownerData, fieldInfo, parentData, parentFieldInfo });
            }
            else
            {
                if (m_method.GetParameters() == null || m_method.GetParameters().Length == 0)
                    m_method.Invoke(pCall, null);
                if (m_method.GetParameters().Length == 1)
                    m_method.Invoke(pCall, new object[] { fieldInfo });
                else if (m_method.GetParameters().Length == 2)
                    m_method.Invoke(pCall, new object[] { ownerData, fieldInfo });
                else if (m_method.GetParameters().Length == 3)
                    m_method.Invoke(pCall, new object[] { ownerData, fieldInfo, parentData });
                else if (m_method.GetParameters().Length == 4)
                    m_method.Invoke(pCall, new object[] { ownerData, fieldInfo, parentData, parentFieldInfo });
            }
            return ownerData;
        }
#endif
    }
}