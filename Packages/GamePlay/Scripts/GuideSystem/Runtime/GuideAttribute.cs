/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideAttribute
作    者:	HappLI
描    述:	
*********************************************************************/
#if UNITY_EDITOR
using Framework.Guide.Editor;
#endif      
using System;
using System.IO;
using UnityEngine;
namespace Framework.Guide
{
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GuideEditorPreviewAttribute : Attribute
    {
#if UNITY_EDITOR
        public int type;
        public string CallMethod;
#endif
        public GuideEditorPreviewAttribute(int type,string CallMethod = null)
        {
#if UNITY_EDITOR
            this.type = type;
            this.CallMethod = CallMethod;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GuideCustomRecodeAttribute : Attribute
    {
#if UNITY_EDITOR
        public string callMethod { get; set; }
#endif
        public GuideCustomRecodeAttribute(string callMethod)
        {
#if UNITY_EDITOR
            this.callMethod = callMethod;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum)]
    public class GuideExportAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strDisplay = "";
#endif
        public GuideExportAttribute(string strDisplay)
        {
#if UNITY_EDITOR
            this.strDisplay = strDisplay;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideDisplayAttribute : Attribute
    {
#if UNITY_EDITOR
        public string displayName = "";
#endif
        public GuideDisplayAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.displayName = displayName;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideDisableAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type disableType = null;
#endif
        public GuideDisableAttribute()
        {
#if UNITY_EDITOR
            this.disableType = null;
#endif
        }
        public GuideDisableAttribute(System.Type disableType)
        {
#if UNITY_EDITOR
            this.disableType = disableType;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideStepAttribute : Attribute
    {
#if UNITY_EDITOR
        public bool bEditorPreview = false;
        public string DisplayName { get; set; }
#endif
        public GuideStepAttribute(string displayName, bool bEditorPreview= false)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            this.bEditorPreview = bEditorPreview;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideTriggerAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
#endif
        public GuideTriggerAttribute(string displayName)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideExcudeAttribute : Attribute
    {
#if UNITY_EDITOR
        public bool bEditorPreview = false;
        public string DisplayName { get; set; }
#endif
        public GuideExcudeAttribute(string displayName, bool bEditorPreview = false)
        {
#if UNITY_EDITOR
            this.bEditorPreview = bEditorPreview;
            DisplayName = displayName;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple =true)]
    public class GuideDisplayTypeAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type displayType;
        public System.Type callDisplayType;
        public string drawMethod = null;
        public string popDisplayName = "";
        public bool bStrValue = false;
#endif
        public GuideDisplayTypeAttribute(string drawMethod, string popDisplayName = null, bool bStrValue = false)
        {
#if UNITY_EDITOR
            this.drawMethod = drawMethod;
            this.displayType = null;
            this.popDisplayName = popDisplayName;
            this.bStrValue = bStrValue;
#endif
        }
        public GuideDisplayTypeAttribute(System.Type displayType, string drawMethod = null, string popDisplayName = null, bool bStrValue = false)
        {
#if UNITY_EDITOR
            this.drawMethod = drawMethod;
            this.displayType = displayType;
            this.popDisplayName = popDisplayName;
            this.bStrValue = bStrValue;
#endif
        }
#if UNITY_EDITOR
        System.Reflection.MethodInfo m_Method = null;
        public bool Draw(GraphNode graphNode, GUIContent strLabel, ref int value)
        {
            if (m_Method == null)
            {
                if (callDisplayType == null) callDisplayType = displayType;
                var method = callDisplayType.GetMethod(drawMethod, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if(method!=null && method.ReturnType == typeof(int) &&
                    method.GetParameters()!=null &&
                    method.GetParameters().Length == 2 &&
                    (method.GetParameters()[0].ParameterType == typeof(string) || method.GetParameters()[0].ParameterType == typeof(GUIContent))
                    && method.GetParameters()[1].ParameterType== typeof(int))
                {
                    m_Method = method;
                }
            }
            if (m_Method == null || m_Method.ReturnType != typeof(int)) return false;
            string lable = strLabel.text;
            if (m_Method.GetParameters()[0].ParameterType == typeof(string))
            {
                value = (int)m_Method.Invoke(null, new object[] { lable, value });
            }
            else
                value = (int)m_Method.Invoke(null, new object[] { strLabel, value });
            return true;
        }
        public bool Draw(GraphNode graphNode, GUIContent strLabel, ref string value)
        {
            if (m_Method == null)
            {
                if (callDisplayType == null) callDisplayType = displayType;
                var method = callDisplayType.GetMethod(drawMethod, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null && method.ReturnType == typeof(string) &&
                    method.GetParameters() != null &&
                    method.GetParameters().Length == 2 &&
                    (method.GetParameters()[0].ParameterType == typeof(string) || method.GetParameters()[0].ParameterType == typeof(GUIContent))
                    && method.GetParameters()[1].ParameterType == typeof(string))
                {
                    m_Method = method;
                }
            }
            if (m_Method == null || m_Method.ReturnType != typeof(string)) return false;
            string lable = strLabel.text;
            if(m_Method.GetParameters()[0].ParameterType == typeof(string))
            {
                value = (string)m_Method.Invoke(null, new object[] { lable, value });
            }
            else
                value = (string)m_Method.Invoke(null, new object[] { strLabel, value });
            return true;
        }
        public bool Draw(GraphNode graphNode, GUIContent strLabel, object data, string valueFieldName)
        {
            if (m_Method == null)
            {
                if (callDisplayType == null) callDisplayType = displayType;
                var method = callDisplayType.GetMethod(drawMethod, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null &&
                    method.GetParameters() != null &&
                    method.GetParameters().Length == 3 &&
                    (method.GetParameters()[0].ParameterType == typeof(string) || method.GetParameters()[0].ParameterType == typeof(GUIContent))
                    && method.GetParameters()[1].ParameterType == typeof(object)
                    && method.GetParameters()[2].ParameterType == typeof(System.Reflection.FieldInfo))
                {
                    m_Method = method;
                }
            }
            if (m_Method == null || m_Method.GetParameters().Length !=3 ||
                m_Method.GetParameters()[2].ParameterType != typeof(System.Reflection.FieldInfo)) 
                return false;

            var fieldInfo = data.GetType().GetField(valueFieldName, System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.NonPublic);
            if (fieldInfo == null)
                return false;

            string lable = strLabel.text;
            if (m_Method.GetParameters()[0].ParameterType == typeof(string))
            {
                m_Method.Invoke(null, new object[] { lable, data, fieldInfo });
            }
            else
                m_Method.Invoke(null, new object[] { strLabel, data, fieldInfo });
            return true;
        }
#endif
    }
    //------------------------------------------------------
    public enum EBitGuiType
    {
        None = 0,
        Normal,
        Offset,
    }
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true, Inherited =false)]
    public class GuideArgvAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type displayType = null;
        public EBitGuiType bBit = EBitGuiType.None;
        public EArgvFalg Flag = EArgvFalg.None;
        public string DisplayName { get; set; }
        public string argvName { get; set; }
        public string strTips = "";
        public string dispayTypeName;
        public object defaultValue = null;
#endif
        public GuideArgvAttribute(string displayName, string argvName, string strTips, System.Type displayType, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None, object defaultValue = null)
        {
#if UNITY_EDITOR
            this.dispayTypeName = null;
            this.displayType = displayType;
            this.argvName = argvName;
            this.DisplayName = displayName;
            this.bBit = bBit;
            this.Flag = Flag;
            this.strTips = strTips;
            this.defaultValue = defaultValue;
#endif
        }
        public GuideArgvAttribute(string displayName, string argvName, string strTips, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None, string displayTypeName = null, object defaultValue = null)
        {
#if UNITY_EDITOR
            this.displayType = null;
            this.argvName = argvName;
            this.DisplayName = displayName;
            this.bBit = bBit;
            this.Flag = Flag;
            this.strTips = strTips;
            this.dispayTypeName = displayTypeName;
            this.defaultValue = defaultValue;
#endif
        }
        public GuideArgvAttribute()
        {

        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class GuideStrArgvAttribute : GuideArgvAttribute
    {
        public GuideStrArgvAttribute(string displayName, string argvName, string strTips, System.Type displayType, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None)
            : base(displayName, argvName, strTips, displayType, Flag, EBitGuiType.Normal)
        {
        }
        public GuideStrArgvAttribute(string displayName, string argvName, string strTips, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None, string displayTypeName = null)
            : base(displayName, argvName, strTips, Flag, EBitGuiType.Normal, displayTypeName)
        {
        }
        public GuideStrArgvAttribute()
        {

        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class GuideBitAttribute : Attribute
    {
#if UNITY_EDITOR
        public int offset = 0;
        public int argvIndex = 0;
        public string DisplayName { get; set; }
#endif
        public GuideBitAttribute(string displayName, int offset, int argvIndex =0)
        {
#if UNITY_EDITOR
            this.argvIndex = argvIndex;
            this.offset = offset;
            this.DisplayName = displayName;
#endif
        }
    }
    //------------------------------------------------------
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field|AttributeTargets.Method, Inherited = false)]
    public class GuideNodeMenuAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
#endif
        public GuideNodeMenuAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.DisplayName = displayName;
#endif
        }
    }
}