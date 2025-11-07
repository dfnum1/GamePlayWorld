#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Framework.Plugin.AT
{
    public abstract class ATEditorAttrData
    {
        public int actionID = 0;
        public string DisplayName;
        public abstract string GetQueryName();
    }
    //------------------------------------------------------
    public class ATAPINodeAttrData : ATEditorAttrData
    {
        public override string GetQueryName() { return ""; }
    }
    //------------------------------------------------------
    public class ATExportNodeAttrData : ATEditorAttrData
    {
        public string strMenuItem;
        public Type classType = null;
        public int classHashCode = 0;
        public string funcName = "";
        public string classFullName = "";
        public ATMonoFuncAttribute monoFuncAttr;
        public ATExportAttribute exportAttr;
        public ATMethodAttribute methodAttr;
        public ATNoLinkAttribute nolinkAttr;
        public ATCanVarBetweenAttribute betweenVar;
        public EVariableType[] canVars;

        public List<ATMethodArgvAttribute> InArgvs;
        public List<ATMethodReturnAttribute> OutArgvs;

        public override string GetQueryName()
        {
            return funcName + strMenuItem;
        }
    }
    public class EventTypeAttr
    {
        public Type enumType;
        public object enumValue;
        public ushort enumValueInt;
        public string strName;
        public Type idCustomType;
    }
    public struct VariableAttrData
    {
        public VariableTypeAttribute attri;
        public System.Type varType;
    }
    public class AssemblyATData
    {
        public Dictionary<System.Type, VariableAttrData> VariableTypes = new Dictionary<System.Type, VariableAttrData>();
        public Dictionary<EVariableType, VariableAttrData> Variables = new Dictionary<EVariableType, VariableAttrData>();
        public Dictionary<int, ATExportNodeAttrData> ExportActions = new Dictionary<int, ATExportNodeAttrData>();
        public List<ATExportNodeAttrData> exportFieldMethods = new List<ATExportNodeAttrData>();
        public List<string> exportFieldMethodsNames = new List<string>();
        public List<string> ExportAPIPopNamess = new List<string>();

        public List<string> exportEventTypeNames = new List<string>();
        public List<EventTypeAttr> exportEventTypes = new List<EventTypeAttr>();

        public void Clear()
        {
            VariableTypes.Clear();
            Variables.Clear();
            ExportActions.Clear();
            ExportAPIPopNamess.Clear();
            exportFieldMethods.Clear();
            AgentTreeUtl.ExportClasses.Clear();
            AgentTreeUtl.POP_EXPORT_MONOS.Clear();
            AgentTreeUtl.EXPORT_CLASS_SHORTNAME_SET.Clear();
            AgentTreeUtl.EXPORT_TYPE_MONOS.Clear();

            exportEventTypeNames.Clear();
            exportEventTypes.Clear();
        }
    }
}
#endif