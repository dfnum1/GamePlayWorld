#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideAttribute
作    者:	HappLI
描    述:	
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Framework.Plugin.Guide
{
    public class GuideEditorPreviewAttribute : Attribute
    {
#if UNITY_EDITOR
        public string CallMethod;
#endif
        public GuideEditorPreviewAttribute(string CallMethod)
        {
#if UNITY_EDITOR
            this.CallMethod = CallMethod;
#endif
        }
    }

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

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class GuideDisplayAttribute : Framework.Base.PluginDisplayAttribute
    {
        public GuideDisplayAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.displayName = displayName;
#endif
        }
    }

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
#endif
        public GuideArgvAttribute(string displayName, string argvName="", string strTips ="", System.Type displayType =null, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None)
        {
#if UNITY_EDITOR
            this.displayType = displayType;
            this.argvName = argvName;
            this.DisplayName = displayName;
            this.bBit = bBit;
            this.Flag = Flag;
            this.strTips = strTips;
#endif
        }
        public GuideArgvAttribute()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class GuideStrArgvAttribute : GuideArgvAttribute
    {
        public GuideStrArgvAttribute(string displayName, string argvName = "", string strTips = "", System.Type displayType = null, EArgvFalg Flag = EArgvFalg.All, EBitGuiType bBit = EBitGuiType.None)
            : base(displayName, argvName, strTips, displayType, Flag, EBitGuiType.Normal)
        {
        }
        public GuideStrArgvAttribute()
        {

        }
    }
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
}
#endif