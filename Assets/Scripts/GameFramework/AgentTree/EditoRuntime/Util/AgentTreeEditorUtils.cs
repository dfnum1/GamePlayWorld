#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Reflection;

namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    struct MousePosAndData
    {
        public Vector2 mousePos;
        public System.Object bindData;
    }
    //------------------------------------------------------
    struct StrcutPopMenuData
    {
        public System.Object port;
        public StructData structData;
        public Variable structMemeber;
    }
    //------------------------------------------------------
    public class GraphNodeDrawPropertyAttribute : Attribute
    {
        public GraphNodeDrawPropertyAttribute() { }
    }
    public class GraphNodeDrawGUIAttribute : Attribute
    {
        public GraphNodeDrawGUIAttribute() { }
    }
    public class GraphNodeDrawGUIMethodAttribute : Attribute
    {
        public int actionType;
        public GraphNodeDrawGUIMethodAttribute(int actionType) { this.actionType = actionType; }
    }
    public class AgentTreeEditorUtils
    {
        //------------------------------------------------------
        public static void RepaintAll()
        {
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            for (int i = 0; i < windows.Length; i++)
            {
                if(windows[i] is IAgentTreeEditor)
                    windows[i].Repaint();
            }
        }
        //------------------------------------------------------
        public static void SetAssetIcon(UnityEngine.Object asset, Texture2D icon)
        {
            if (asset == null || icon == null) return;
            MethodInfo method = typeof(UnityEditor.EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new object[] { asset, icon });
            }
        }
        //------------------------------------------------------
        public static List<string> vPopVariables = new List<string>();
        public static string PrettyName(Type type)
        {
            if (type == null) return "null";
            if (type == typeof(System.Object)) return "object";
            if (type == typeof(float)) return "float";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(bool)) return "bool";
            //else if (type.IsGenericType)
            //{
            //    string s = "";
            //    Type genericType = type.GetGenericTypeDefinition();
            //    if (genericType == typeof(List<>)) s = "List";
            //    else s = type.GetGenericTypeDefinition().ToString();

            //    Type[] types = type.GetGenericArguments();
            //    string[] stypes = new string[types.Length];
            //    for (int i = 0; i < types.Length; i++)
            //    {
            //        stypes[i] = types[i].PrettyName();
            //    }
            //    return s + "<" + string.Join(", ", stypes) + ">";
            //}
            //else if (type.IsArray)
            //{
            //    string rank = "";
            //    for (int i = 1; i < type.GetArrayRank(); i++)
            //    {
            //        rank += ",";
            //    }
            //    Type elementType = type.GetElementType();
            //    if (!elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";
            //    else
            //    {
            //        string s = elementType.PrettyName();
            //        int i = s.IndexOf('[');
            //        return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
            //    }
            //}
            //else return type.ToString();
            return type.Name.Replace("Variable", "").ToString();
        }
        //------------------------------------------------------
        public static Port BuildOriVariableCommonNew<T>(VariableFactory pVarsFactor, List<Port> vPort, int index) where T : Variable
        {
            if(vPort!=null && index < vPort.Count)
            {
                if(vPort[index].variable != null && vPort[index].variable.GetType() == typeof(T))
                {
                    return vPort[index];
                }
            }
            Variable pvar = pVarsFactor.NewVariableByType(typeof(T));
            pvar.strName = "";
            return new Port(pvar);
        }
        //------------------------------------------------------
        public static Port BuildOriVariableCommonNew(EVariableType varType, VariableFactory pVarsFactor, List<Port> vPort, int index)
        {
            System.Type varClassType = VariableSerializes.GetVariableType(varType);
            if (vPort != null && index < vPort.Count)
            {
                if (vPort[index].variable != null && vPort[index].variable.GetType() == varClassType)
                {
                    return vPort[index];
                }
            }
            Variable pvar = pVarsFactor.NewVariableByType(varClassType);
            pvar.strName = "";
            return new Port(pvar);
        }
        //------------------------------------------------------
        public static Port BuildOriVariableCommonNew(System.Type varType, VariableFactory pVarsFactor, List<Port> vPort, int index)
        {
            return BuildOriVariableCommonNew(VariableSerializes.GetVariableType(varType), pVarsFactor, vPort, index);
        }
        //------------------------------------------------------
        public static bool HasCommonInheritBase(Type srcType, Type dstType)
        {
            if (srcType == null || dstType == null) return false;

            if (srcType.IsInterface && dstType.IsInterface && dstType == srcType)
                return true;

            if(srcType.IsInterface && dstType.GetInterface(srcType.FullName)!=null)
            {
                return true;
            }
            if (dstType.IsInterface && srcType.GetInterface(dstType.FullName) != null)
            {
                return true;
            }

            Type src = srcType;
            while(src!=null)
            {
                bool bHasCommon = false;
                Type dst = dstType;
                while(dst !=null)
                {
                    if(dst == src && dst != typeof(IUserData))
                    {
                        bHasCommon = true;
                        break;
                    }
                    dst = dst.BaseType;
                }
                if (bHasCommon) return true;
                src = src.BaseType;
            }
            return false;
        }
        //------------------------------------------------------
        public static Type FindInheirtTypeType<T>()
        {
            System.Type parentType = typeof(T);
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    if (types[i].IsSubclassOf(parentType))
                    {
                        return types[i];
                    }
                }
            }
            return null;
        }
        //------------------------------------------------------
        static System.Reflection.MethodInfo ms_DrawPropertyMethod = null;
        public static System.Reflection.MethodInfo GetDrawProperyMethod()
        {
            return ms_DrawPropertyMethod;
        }
        //------------------------------------------------------
        public static void SetDrawProperyMethod(System.Reflection.MethodInfo method)
        {
            ms_DrawPropertyMethod = method;

        }
        //------------------------------------------------------
        public static long BuildTransferKey(int guid0, int guid1)
        {
            long minV = Math.Min(guid0, guid1);
            long maxV = Math.Max(guid0, guid1);
            return minV << 32 | maxV;
        }
        //------------------------------------------------------
        static AssemblyATData ms_AssemblyATData = new AssemblyATData();
        public static AssemblyATData AssemblyATData
        {
            get { return ms_AssemblyATData; }
        }
        //------------------------------------------------------
        public static void CheckAssemblyAT()
        {
            ms_AssemblyATData.Clear();
            {
                int classId = AgentTreeUtl.TypeToHash(typeof(IUserData));
                AgentTreeUtl.ExportClasses[classId] = typeof(IUserData);
            }

            bool bDrawPropetyCheck = false;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    System.Type tp = types[i];
                    if (!bDrawPropetyCheck && tp.IsDefined(typeof(GraphNodeDrawPropertyAttribute)))
                    {
                        MethodInfo[] methods = tp.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
                        for (int k = 0; k < methods.Length; ++k)
                        {
                            if (methods[k].IsDefined(typeof(GraphNodeDrawPropertyAttribute), false))
                            {
                                AgentTreeEditorUtils.SetDrawProperyMethod(methods[k]);
                                bDrawPropetyCheck = true;
                                break;
                            }
                        }
                    }
                    //  GraphNodeLayoutGUI.CheckDrawPropetyFunction(tp);
                    if (tp.IsEnum && tp.IsDefined(typeof(ATEventAttribute), false))
                    {
                        ATEventAttribute attr = (ATEventAttribute)tp.GetCustomAttribute(typeof(ATEventAttribute));
                        if (string.IsNullOrEmpty(attr.DisplayName))
                            attr.DisplayName = tp.Name;
                        foreach (var v in System.Enum.GetValues(tp))
                        {
                            string strName = System.Enum.GetName(tp, v);
                            FieldInfo fi = tp.GetField(strName);
                            int flagValue = (int)v;
                            System.Type customIDType = null;
                            if (fi.IsDefined(typeof(ATEventAttribute)))
                            {
                                customIDType = fi.GetCustomAttribute<ATEventAttribute>().GetDisplayType();
                                strName = fi.GetCustomAttribute<ATEventAttribute>().DisplayName;
                            }
                            ushort enumInt = System.Convert.ToUInt16(v);
                            if (enumInt > 0)
                            {
                                ms_AssemblyATData.exportEventTypeNames.Add(strName);
                                ms_AssemblyATData.exportEventTypes.Add(new EventTypeAttr() { enumType = tp, idCustomType = customIDType, enumValue = v, enumValueInt = enumInt, strName = System.Enum.GetName(tp, v) });
                            }
                        }
                    }
                    if (tp.IsDefined(typeof(VariableTypeAttribute), false))
                    {
                        VariableTypeAttribute attr = (VariableTypeAttribute)tp.GetCustomAttribute(typeof(VariableTypeAttribute));
                        if (attr.valType != EVariableType.Null)
                        {
                            ms_AssemblyATData.Variables.Add(attr.valType, new VariableAttrData() { attri = attr, varType = tp });
                            ms_AssemblyATData.VariableTypes.Add(tp, new VariableAttrData() { attri = attr, varType = tp });
                        }
                    }
                    if (tp.IsDefined(typeof(ATExportMonoAttribute), false))
                    {
                        int classId = AgentTreeUtl.TypeToHash(tp);
                        string popName = tp.FullName.Replace(".", "/");
                        int index = popName.LastIndexOf('/');
                        if (index > 0 && index < popName.Length)
                        {
                            string starPopName = popName.Substring(0, index);
                            string realName = popName.Substring(index + 1);
                            if (realName.Length > 4)
                            {
                                popName = starPopName + "/" + realName.Substring(0, 3) + "/" + realName;
                            }
                            else if (realName.Length > 1)
                                popName = starPopName + "/" + realName.Substring(0, 1) + "/" + realName;
                        }
                        AgentTreeUtl.POP_EXPORT_MONOS.Add(popName);
                        AgentTreeUtl.EXPORT_TYPE_MONOS.Add(new AgentTreeUtl.sMonoTypeInfo() { type = tp, hashCode = classId });
                        AgentTreeUtl.ExportClasses[classId] = tp;
                        AgentTreeUtl.EXPORT_CLASS_SHORTNAME_SET.Add(tp.Name);
                    }
                    else if (tp.IsDefined(typeof(ATExportAttribute), false))
                    {
                        ATExportAttribute exportAttr = tp.GetCustomAttribute<ATExportAttribute>();
                        MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        for (int m = 0; m < meths.Length; ++m)
                        {
                            if (meths[m].IsDefined(typeof(ATMethodAttribute), false) || meths[m].IsDefined(typeof(ATMonoFuncAttribute), false))
                            {
                                ATMonoFuncAttribute funcAttr = (ATMonoFuncAttribute)meths[m].GetCustomAttribute(typeof(ATMonoFuncAttribute));
                                ATMethodAttribute attr = (ATMethodAttribute)meths[m].GetCustomAttribute(typeof(ATMethodAttribute));
                                if (attr == null) attr = new ATMethodAttribute(funcAttr);
                                int actionType = attr.ActionType;
                                if (actionType != (int)EActionType.None)
                                {
                                    ATExportNodeAttrData node = new ATExportNodeAttrData();
                                    if (funcAttr != null && funcAttr.DecleType != null)
                                    {
                                        node.classHashCode = AgentTreeUtl.TypeToHash(funcAttr.DecleType);
                                        node.classType = funcAttr.DecleType;
                                    }
                                    if (funcAttr != null)
                                        node.DisplayName = funcAttr.DisplayName;
                                    else node.DisplayName = attr.DisplayName;
                                    node.actionID = actionType;
                                    node.classFullName = tp.FullName;
                                    node.funcName = meths[m].Name;
                                    node.exportAttr = exportAttr;
                                    node.monoFuncAttr = funcAttr;
                                    node.betweenVar = (ATCanVarBetweenAttribute)meths[m].GetCustomAttribute(typeof(ATCanVarBetweenAttribute));
                                    ATCanVarsAttribute vars = (ATCanVarsAttribute)meths[m].GetCustomAttribute(typeof(ATCanVarsAttribute));
                                    if (vars != null && vars.vars.Length > 0)
                                        node.canVars = vars.vars;
                                    else node.canVars = null;
                                    node.nolinkAttr = (ATNoLinkAttribute)meths[m].GetCustomAttribute(typeof(ATNoLinkAttribute));

                                    node.strMenuItem = exportAttr.DisplayName + "/" + attr.DisplayName;
                                    node.methodAttr = attr;
                                    ATMethodArgvAttribute[] inArgv = (ATMethodArgvAttribute[])meths[m].GetCustomAttributes(typeof(ATMethodArgvAttribute));
                                    if (inArgv != null)
                                        node.InArgvs = new List<ATMethodArgvAttribute>(inArgv);
                                    else
                                        node.InArgvs = new List<ATMethodArgvAttribute>();

                                    ATMethodReturnAttribute[] outArgv = (ATMethodReturnAttribute[])meths[m].GetCustomAttributes(typeof(ATMethodReturnAttribute));
                                    if (outArgv != null)
                                        node.OutArgvs = new List<ATMethodReturnAttribute>(outArgv);
                                    else
                                        node.OutArgvs = new List<ATMethodReturnAttribute>();

                                    //      if(funcAttr != null && funcAttr.bFieldGet && node.classType != null)
                                    //       {
                                    //          exportFieldMethods.Add(node);
                                    //          exportFieldMethodsNames.Add(exportAttr.DisplayName + "/" + funcAttr.DisplayName);
                                    //     }

                                    ms_AssemblyATData.ExportActions.Add(actionType, node);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif