/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	MethodAttributes
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace Framework.Plugin.AT
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class DisiableExportAttribute : Attribute
    {
        public DisiableExportAttribute() { }
    }
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class ATDisplayNameAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
#endif
        public ATDisplayNameAttribute()
        {
#if UNITY_EDITOR
            this.DisplayName = null;
#endif
        }
        public ATDisplayNameAttribute(string displayName)
        {
#if UNITY_EDITOR
            this.DisplayName = displayName;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class ATDisableGUIAttribute : Attribute
    {
        public ATDisableGUIAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Enum|AttributeTargets.Field)]
    public class ATEventAttribute : ATDisplayNameAttribute
    {
#if UNITY_EDITOR
        private Type idType = null;
        private string typeName = null;
        public System.Type ownerType = null;
#endif
        public ATEventAttribute(string displayName, Type idType = null, System.Type ownerType = null)
        {
#if UNITY_EDITOR
            this.idType = idType;
            this.typeName = null;
            DisplayName = displayName;
            this.ownerType = ownerType;
#endif
        }
        public ATEventAttribute(string displayName, string typeName, System.Type ownerType = null)
        {
#if UNITY_EDITOR
            this.idType = null;
            this.typeName = typeName;
            DisplayName = displayName;
            this.ownerType = ownerType;
#endif
        }

        public System.Type GetDisplayType()
        {
#if UNITY_EDITOR
            if (this.idType != null) return this.idType;

            if(!string.IsNullOrEmpty(this.typeName))
            {
                this.idType = AgentTreeUtl.GetTypeByName(typeName);
            }
            return this.idType;
#else
            return null;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ATFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public string ToolTips;
        public Type displayType = null;
        public byte bGetSetFlag = 0;   // 0-all 1-get 2-set
#endif
        public ATFieldAttribute(string displayName="", Type displayType = null, string ToolTips = "", byte bOnlyGet = 0)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            this.ToolTips = ToolTips;
            this.displayType = null;
            this.bGetSetFlag = bOnlyGet;
#endif
        }
#if UNITY_EDITOR
        public bool CanGet
        {

            get { return bGetSetFlag == 0 || bGetSetFlag == 1; }
        }
        public bool CanSet
        {
            get { return bGetSetFlag == 0 || bGetSetFlag == 2; }
        }
#endif
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ATMethodAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public int ActionType = (int)EActionType.None;
        public string ToolTips;
        public bool bEnable = true;
        public bool bEditable = false;
        public bool bAutoGenerator = false;

        public Type DecleType = null;

        public Type[] ArgvDisplays = null;
        public Type ReturnDisplays = null;
#endif
        public ATMethodAttribute(string displayName, int actionType, bool bEnable = true, bool bEditable = false, bool bAutoGenerator = false, string ToolTips="")
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            ActionType = actionType;
            this.bEnable = bEnable;
            this.bEditable = bEditable;
            this.ToolTips = ToolTips;
            this.bAutoGenerator = bAutoGenerator;
            this.ArgvDisplays = null;
            this.ReturnDisplays = null;
#endif
        }

        public ATMethodAttribute(string displayName, Type[] argvDisplays = null, Type returnDisplays = null)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            ActionType = 0;
            this.bEnable = true;
            this.bEditable = false;
            this.ToolTips = "";
            this.bAutoGenerator = true;
            this.ArgvDisplays = argvDisplays;
            this.ReturnDisplays = returnDisplays;
#endif
        }
        public ATMethodAttribute()
        {
#if UNITY_EDITOR
            DisplayName = null;
            ActionType = 0;
            this.bEnable = true;
            this.bEditable = false;
            this.ToolTips = "";
            this.bAutoGenerator = true;
            this.ArgvDisplays = null;
            this.ReturnDisplays = null;
#endif
        }

        public ATMethodAttribute(ATMonoFuncAttribute func)
        {
#if UNITY_EDITOR
            DisplayName = func.DisplayName;
            ActionType = func.guid;
            ToolTips = func.ToolTips;
            bEnable = true;
            bEditable = false;
            bAutoGenerator = true;
            DecleType = func.DecleType;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
    public class ATAPIAttribute : Attribute
    {

#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public string ToolTips;

        public Type DecleType = null;
#endif
        public ATAPIAttribute(string displayName, string ToolTips = "", Type DecleType = null)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            this.ToolTips = ToolTips;
            this.DecleType = DecleType;
#endif
        }

        public ATAPIAttribute(string displayName, Type DecleType)
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            this.ToolTips = "";
            this.DecleType = DecleType;
#endif
        }
        public ATAPIAttribute()
        {
#if UNITY_EDITOR
            DisplayName = null;
            this.ToolTips = "";
            this.DecleType = null;
#endif
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class ATExportAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public bool bEnable = true;
        public string ToolTips;
        public bool bEditable = false;
        public string marcoDefine = "";
#endif
        public ATExportAttribute(string displayName="", bool bEnable = true, string ToolTips = "", string marcoDefine = "")
        {
#if UNITY_EDITOR
            this.DisplayName = displayName;
            this.ToolTips = ToolTips;
            this.bEnable = bEnable;
            this.marcoDefine = marcoDefine;
#endif
        }
#if UNITY_EDITOR
        public virtual bool IsMono() { return false; }
#endif
    }



    public enum EGlobalType
    {
        None = 0,
        Module,
        Single,
        AlignLamda,
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Method)]
    public class ATConvertBehaviourAttribute : Attribute
    {
        public ATConvertBehaviourAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Method)]
    public class ATExportGUIDAttribute : Attribute
    {
#if UNITY_EDITOR
        public int hashCode = 0;
#endif
        public ATExportGUIDAttribute(int hashCode)
        {
#if UNITY_EDITOR
            this.hashCode = hashCode;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Field)]
    public class ATDefaultPointerAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strLamda = "";
#endif
        public ATDefaultPointerAttribute(string strLamda)
        {
#if UNITY_EDITOR
            this.strLamda = strLamda;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface|AttributeTargets.Struct)]
    public class ATExportMonoAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public bool bEnable = true;
        public string ToolTips;
        public bool bEditable = false;
        public bool bMono = true;
        public string strLamda = "";
        public string marcoDefine = "";
        public EGlobalType bGlobalModule = EGlobalType.None;
#endif
        public ATExportMonoAttribute(string displayName = "", EGlobalType bGlobalModule = EGlobalType.None, bool bEnable = true, string ToolTips = "", string marcoDefine = "")
        {
#if UNITY_EDITOR
            this.DisplayName = displayName;
            this.ToolTips = ToolTips;
            this.bEnable = bEnable;
            this.bGlobalModule = bGlobalModule;
            this.strLamda = "";
            this.marcoDefine = marcoDefine;
            if (this.bGlobalModule == EGlobalType.AlignLamda)
                this.bGlobalModule = EGlobalType.None;
#endif
        }

        public ATExportMonoAttribute(string displayName, string strLamda, bool bEnable = true, string ToolTips = "", string marcoDefine = "")
        {
#if UNITY_EDITOR
            bGlobalModule = EGlobalType.AlignLamda;
            this.DisplayName = displayName;
            this.strLamda = strLamda;
            this.ToolTips = ToolTips;
            this.bEnable = bEnable;
            this.marcoDefine = marcoDefine;
#endif
        }

        public ATExportMonoAttribute(ATExportAttribute attr)
        {
#if UNITY_EDITOR
            this.DisplayName = attr.DisplayName;
            this.ToolTips = attr.ToolTips;
            this.bEnable = attr.bEnable;
            this.bEditable = false;
            this.bMono = false;
            this.marcoDefine = attr.marcoDefine;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ATMonoMethodExportAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public bool bEnable = true;
        public bool bEditable = false;
        public string ToolTips;
#endif
        public ATMonoMethodExportAttribute(string displayName, bool bEnable = true, string ToolTips ="")
        {
#if UNITY_EDITOR
            DisplayName = displayName;
            this.ToolTips = ToolTips;
            this.bEnable = bEnable;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ATMonoFuncAttribute : Attribute
    {
#if UNITY_EDITOR
        public string DisplayName { get; set; }
        public string ToolTips;
        public Type DecleType;
        public int guid;
        public bool bFieldGet = false;
#endif
        public ATMonoFuncAttribute(int guid, string displayName, Type type, bool bFieldGet = false, string ToolTips = "")
        {
#if UNITY_EDITOR
            this.guid = guid;
            this.ToolTips = ToolTips;
            DisplayName = displayName;
            this.DecleType = type;
            this.bFieldGet = bFieldGet;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ATMethodArgvAttribute : Attribute
    {
#if UNITY_EDITOR
        public Type DisplayType;
        public Type AlignType;
        public Type ArgvType;
        public string DisplayName;
        public string ToolTips;
        public bool bAutoDestroy = false;
        public bool bReturn = false;
        public bool bSeriable = true;
        public bool bShowEdit = true;
        public int ListElementByArgvIndex = -1;

        internal System.Object defaultValue = null;
        internal bool isExternAttrThis = false;
        internal bool isDelegateCall;
        internal bool isDelegateCallValid;
        internal System.Collections.Generic.List<ATMethodArgvAttribute> vDelegateArgvs;
#endif
        public ATMethodArgvAttribute(Type ArgvType, string DisplayName="", bool bAutoDestroy = false, Type AlignType = null, Type DisplayType = null, string ToolTips = "", bool bReturn= false, int ListElementByArgvIndex = -1, bool bSeriable= true, bool bShowEdit = true)
        {
#if UNITY_EDITOR
            this.ArgvType = ArgvType;
            this.DisplayName = DisplayName;
            this.ToolTips = ToolTips; 
            this.bAutoDestroy = bAutoDestroy;
            this.DisplayType = DisplayType;
            this.AlignType = AlignType;
            this.bReturn = bReturn;
            this.ListElementByArgvIndex = ListElementByArgvIndex;
            this.bSeriable = bSeriable;
            this.bShowEdit = bShowEdit;
            this.isExternAttrThis = false;
            this.isDelegateCall = false;
            this.isDelegateCallValid = false;

#endif
        }
        public ATMethodArgvAttribute(Type ArgvType, string DisplayName, object DefauleValue, Type AlignType, Type DisplayType=null, string ToolTips = "", bool bReturn = false, int ListElementByArgvIndex = -1, bool bSeriable = true, bool bShowEdit = true)
        {
#if UNITY_EDITOR
            this.ArgvType = ArgvType;
            this.DisplayType = DisplayType;
            this.DisplayName = DisplayName;
            this.AlignType = AlignType;
            this.ToolTips = ToolTips;
            this.bAutoDestroy = false;
            this.DisplayType = DisplayType;
            this.bReturn = bReturn;
            this.ListElementByArgvIndex = ListElementByArgvIndex;
            this.bSeriable = bSeriable;
            this.bShowEdit = bShowEdit;
            this.isExternAttrThis = false;
            this.isDelegateCall = false;
            this.isDelegateCallValid = false;
#endif
        }
#if UNITY_EDITOR
        internal void CheckDelegateArgvs(System.Reflection.ParameterInfo paramInfo)
        {
            if (vDelegateArgvs != null) return;
            vDelegateArgvs = new System.Collections.Generic.List<ATMethodArgvAttribute>();
            //! parse delegate param
            if (paramInfo.ParameterType.GenericTypeArguments != null && paramInfo.ParameterType.GenericTypeArguments.Length > 0)
            {
                for (int k = 0; k < paramInfo.ParameterType.GenericTypeArguments.Length; ++k)
                {
                    Type paramClassType = null;
                    Type varType1 = AgentTreeAutoBuildCodeGUI.GetVariableType(paramInfo.ParameterType.GenericTypeArguments[k], ref paramClassType);
                    if (varType1 != null)
                    {
                        var agv1 = new ATMethodArgvAttribute(varType1, paramInfo.Name, false, paramClassType, paramInfo.ParameterType.GenericTypeArguments[k], "", false, -1, true);
                        agv1.DisplayName = paramInfo.ParameterType.GenericTypeArguments[k].Name + "Var";
                        vDelegateArgvs.Add(agv1);
                    }
                    else
                    {
                        isDelegateCallValid = false;
                        break;
                    }
                }
            }
        }
#endif
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public class ATMethodReturnAttribute : Attribute
    {
#if UNITY_EDITOR
        public Type DisplayType;
        public Type AlignType;
        public Type ReturnType;
        public string Name;
        public string ToolTips="";
        public bool bAutoDestroy = false;
        public bool bSeriable = true;
        public bool bShowEdit = true;
        public byte bPropertySet = 0; //0=none, 1-get,2-set,3-getset
        public int ListElementByArgvIndex = -1;
#endif
        public ATMethodReturnAttribute(Type ReturnType, Type AlignType=null, string name="", bool bAutoDestroy = false, string ToolTips = "", int ListElementByArgvIndex = -1, bool bSeriable = true, bool bShowEdit = true, byte bPropertySet = 0)
        {
#if UNITY_EDITOR
            this.ReturnType = ReturnType;
            this.AlignType = AlignType;
            this.Name = name;
            this.ToolTips = ToolTips;
            this.bAutoDestroy = bAutoDestroy;
            this.ListElementByArgvIndex = ListElementByArgvIndex;
            this.bSeriable = bSeriable;
            this.bShowEdit = bShowEdit;
            this.bPropertySet = bPropertySet;
#endif
        }
        public ATMethodReturnAttribute(Type ReturnType, string name, Type AlignType=null, Type DisplayType=null, string ToolTips = "", int ListElementByArgvIndex = -1, bool lbSeriable = true, bool bShowEdit = true, byte bPropertySet = 0)
        {
#if UNITY_EDITOR
            this.ReturnType = ReturnType;
            this.AlignType = AlignType;
            this.Name = name;
            this.ToolTips = ToolTips;
            this.bAutoDestroy = false;
            this.DisplayType = DisplayType;
            this.ListElementByArgvIndex = ListElementByArgvIndex;
            this.bSeriable = lbSeriable;
            this.bShowEdit = bShowEdit;
            this.bPropertySet = bPropertySet;
#endif
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class ATCanVarAttribute : Attribute
    {
#if UNITY_EDITOR
        public EVariableType type;
#endif
        public ATCanVarAttribute(EVariableType type)
        {
#if UNITY_EDITOR
            this.type = type;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ATCanVarBetweenAttribute : Attribute
    {
#if UNITY_EDITOR
        public EVariableType begin;
        public EVariableType end;

        public bool bAll;
#endif
        public ATCanVarBetweenAttribute(EVariableType begin, EVariableType end)
        {
#if UNITY_EDITOR
            this.begin = begin;
            this.end = end;
            bAll = false;
#endif
        }

        public ATCanVarBetweenAttribute()
        {
#if UNITY_EDITOR
            this.bAll = true;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Field)]
    public class ATCanVarsAttribute : Attribute
    {
#if UNITY_EDITOR
        public EVariableType[] vars;
#endif
        public ATCanVarsAttribute(EVariableType[] vars)
        {
#if UNITY_EDITOR
            this.vars = vars;
#endif
        }
        public ATCanVarsAttribute(bool AllList)
        {
#if UNITY_EDITOR
            this.vars = new EVariableType[] { EVariableType.BoolList,EVariableType.ByteList, EVariableType.IntList, EVariableType.FloatList,
                EVariableType.Vector2List, EVariableType.Vector2IntList,
                EVariableType.Vector3List,EVariableType.Vector3IntList,
                EVariableType.Vector4List,EVariableType.QuaternionList,
                EVariableType.ColorList,EVariableType.StringList,
                EVariableType.CurveList,EVariableType.UserDataList,
                EVariableType.MonoScriptList,EVariableType.ObjectList};
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ATConditionEnableAttribute : Attribute
    {
        public ATConditionEnableAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ATNoLinkAttribute : Attribute
    {
        public ATNoLinkAttribute()
        {
        }
    }

}