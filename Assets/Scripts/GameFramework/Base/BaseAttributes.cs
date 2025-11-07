using System;
using System.Reflection;

namespace Framework.Base
{
    public class PluginBinderTypeAttribute : Attribute
    {
#if UNITY_EDITOR
        public string bindName;
#endif
        public PluginBinderTypeAttribute(string bindName)
        {
#if UNITY_EDITOR
            this.bindName = bindName;
#endif
        }
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class DisplayAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strTips = "";
        public string displayName { get; set; }
#endif
        public DisplayAttribute()
        {
#if UNITY_EDITOR
            this.displayName = null;
#endif
        }
        public DisplayAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.displayName = displayName;
#endif
        }
        public DisplayAttribute(string displayName, string tip)
        {
#if UNITY_EDITOR
            this.strTips = tip;
            this.displayName = displayName;
#endif
        }
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class PluginDisplayAttribute : DisplayAttribute
    {
        public PluginDisplayAttribute()
        {
#if UNITY_EDITOR
            this.displayName = null;
#endif
        }
        public PluginDisplayAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.displayName = displayName;
#endif
        }
        public PluginDisplayAttribute(string displayName, string tip)
        {
#if UNITY_EDITOR
            this.strTips = tip;
            this.displayName = displayName;
#endif
        }
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class DisplayNameGUIAttribute : PluginDisplayAttribute
    {
        public DisplayNameGUIAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.displayName = displayName;
#endif
        }
        public DisplayNameGUIAttribute(string displayName, string tip)
        {
#if UNITY_EDITOR
            this.strTips = tip;
            this.displayName = displayName;
#endif
        }
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginExternAudioAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strMethod = "Play";
#endif
        public PluginExternAudioAttribute(string strMethod = null)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(strMethod)) return;
            this.strMethod = strMethod;
#endif
        }
    }
    public class PluginEditorWindowAttribute : Attribute
    {
#if UNITY_EDITOR
        public string widnowName;
        public string method;
#endif
        public PluginEditorWindowAttribute(string widnowName, string method = "OpenEditor")
        {
#if UNITY_EDITOR
            this.widnowName = widnowName;
            this.method = method;
#endif
        }
    }
    //-----------------------------------------------------
    public class DisableGUIAttribute : Attribute
    {
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class DisableFiledGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string[] fields;
#endif
        public DisableFiledGUIAttribute(params string[] fields)
        {
#if UNITY_EDITOR
            this.fields = fields;
#endif
        }
    }
    //-----------------------------------------------------
    public class UnEditAttribute : Attribute
    {
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginInspectorAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type classType;
        public string method = "DrawInspector";
#endif
        public PluginInspectorAttribute(System.Type classType, string method = "DrawInspector")
        {
#if UNITY_EDITOR
            this.classType = classType;
            this.method = method;
#endif
        }
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Method)]
    public class AddInspectorAttribute : Attribute
    {
    }
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field)]
    public class RowFieldInspectorAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type classType;
        public string method = "OnDrawFieldLineRow";
        MethodInfo m_method = null;

#endif
        public RowFieldInspectorAttribute(string method = "OnDrawFieldLineRow", System.Type classType = null)
        {
#if UNITY_EDITOR
            this.method = method;
            this.classType = classType;
#endif
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public System.Object OnDrawLineRow(System.Object ownerData, System.Reflection.FieldInfo fieldInfo, System.Object parentData, System.Reflection.FieldInfo parentFieldInfo)
        {
            if (m_method == null)
            {
                System.Type ownerType = classType;
                if (ownerType == null) ownerType = fieldInfo.DeclaringType;
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
                if (m_method.GetParameters().Length == 1)
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
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldInspectorAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type classType;
        public string method = "OnFieldInspector";
        MethodInfo m_method = null;

#endif
        public FieldInspectorAttribute(string method = "OnFieldInspector", System.Type classType = null)
        {
#if UNITY_EDITOR
            this.method = method;
            this.classType = classType;
#endif
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public System.Object OnInspector(System.Object ownerData, System.Reflection.FieldInfo fieldInfo, System.Object parentData, System.Reflection.FieldInfo parentFieldInfo)
        {
            if (m_method == null)
            {
                System.Type ownerType = classType;
                if (ownerType == null) ownerType = fieldInfo.DeclaringType;
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
                if (m_method.GetParameters().Length == 1)
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
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Field)]
    public class EditFloatAttribute : Attribute
    {
#if UNITY_EDITOR
        public int factor = 1000;
#endif
        public EditFloatAttribute(int factor =1000)
        {
#if UNITY_EDITOR
            this.factor = factor;
#endif
        }
    }
}
