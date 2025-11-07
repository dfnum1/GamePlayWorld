#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;

namespace Framework.Plugin.AT
{
    public class AgentTreeAutoBuildCodeGUI
    {
        public struct ExportMethodInfo
        {
            public string memberName;
            public MemberInfo info;
            public System.Type DeclaringType
            {
                get { return info.DeclaringType; }
            }
            public T GetCustomAttribute<T>() where T : Attribute
            {
                return info.GetCustomAttribute(typeof(T)) as T;
            }
            public T GetCustomAttribute<T>(bool inherit) where T : Attribute
            {
                return info.GetCustomAttribute(typeof(T), inherit) as T;
            }
            public T[] GetCustomAttributes<T>() where T : Attribute
            {
                return info.GetCustomAttributes(typeof(T)) as T[];
            }
            public T[] GetCustomAttributes<T>(bool inherit) where T : Attribute
            {
                return info.GetCustomAttributes(typeof(T), inherit) as T[];
            }
            public bool IsDefined(System.Type attrType)
            {
                return info.IsDefined(attrType);
            }
        }
        static List<Type> vConvertBehaviourInterfaceTypes = new List<Type>();
        static Dictionary<string, Type> EXPORTCLASS = new Dictionary<string, Type>();
        [MenuItem("Tools/AT/导出")]
        public static void Build()
        {
            SetAutoCode.Build();

            vConvertBehaviourInterfaceTypes.Clear();
            EXPORTCLASS.Clear();
            Assembly assembly = null;
            Dictionary<EVariableType, System.Type> vVariables = new Dictionary<EVariableType, System.Type>();
            List<ExportMethodInfo> vAutos = new List<ExportMethodInfo>();
            Dictionary<System.Type, List<ExportMethodInfo>> vExportMonos = new Dictionary<System.Type, List<ExportMethodInfo>>();
            List<System.Type> vInheritUserDatas = new List<System.Type>();
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                assembly = ass;
                bool bContinue = true;
//                 if (assembly.GetName().Name == "MainScripts" || assembly.GetName().Name == "FrameworkPlus")
//                     bContinue = true;
                if (!bContinue)
                    continue;

                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type tp = types[i];
                    if (!CanExport(tp))
                        continue;
                    if(tp.IsInterface && tp.IsDefined(typeof(ATConvertBehaviourAttribute)) )
                    {
                        vConvertBehaviourInterfaceTypes.Add(tp);
                    }
                    if(!tp.IsInterface && tp.GetInterface("IUserData") !=null)
                    {
                        vInheritUserDatas.Add(tp);
                    }
                    if (tp.IsDefined(typeof(VariableTypeAttribute), false))
                    {
                        VariableTypeAttribute attr = (VariableTypeAttribute)tp.GetCustomAttribute(typeof(VariableTypeAttribute));
                        if (attr.valType != EVariableType.Null)
                        {
                            vVariables.Add(attr.valType, tp);
                        }
                    }
                    else if (tp.IsDefined(typeof(ATExportMonoAttribute), false))
                    {
                        EXPORTCLASS.Add(tp.FullName.ToLower(), tp);
                        ATExportMonoAttribute exportAttr = (ATExportMonoAttribute)tp.GetCustomAttribute(typeof(ATExportMonoAttribute));
                        if(exportAttr.bEnable)
                        {
                            Dictionary<string, int> vMethodNames = new Dictionary<string, int>();
                            MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                            for (int m = 0; m < meths.Length; ++m)
                            {
                                if (meths[m].IsDefined(typeof(ATMethodAttribute), false))
                                {
                                    ATMethodAttribute attr = (ATMethodAttribute)meths[m].GetCustomAttribute(typeof(ATMethodAttribute));
                                    List<ExportMethodInfo> vEpxots;
                                    if (!vExportMonos.TryGetValue(tp, out vEpxots))
                                    {
                                        vEpxots = new List<ExportMethodInfo>();
                                        vExportMonos.Add(tp, vEpxots);
                                    }
                                    int memberCnt = 0;
                                    if(!vMethodNames.TryGetValue(meths[m].Name, out memberCnt))
                                    {
                                        memberCnt = 1;
                                        vMethodNames[meths[m].Name] = memberCnt;
                                    }
                                    else
                                        vMethodNames[meths[m].Name] = ++memberCnt;

                                    ExportMethodInfo exportMth = new ExportMethodInfo();
                                    exportMth.info = meths[m];
                                    if (memberCnt <= 1)  exportMth.memberName = meths[m].Name;
                                    else exportMth.memberName = meths[m].Name + "_" + (memberCnt-1);
                                    vAutos.Add(exportMth);
                                    vEpxots.Add(exportMth);
                                }
                            }

                            FieldInfo[] fields = types[i].GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                            for (int m = 0; m < fields.Length; ++m)
                            {
                                if (fields[m].IsDefined(typeof(ATFieldAttribute), false))
                                {
                                    ATFieldAttribute attr = (ATFieldAttribute)fields[m].GetCustomAttribute(typeof(ATFieldAttribute));
                                    List<ExportMethodInfo> vEpxots;
                                    if (!vExportMonos.TryGetValue(tp, out vEpxots))
                                    {
                                        vEpxots = new List<ExportMethodInfo>();
                                        vExportMonos.Add(tp, vEpxots);
                                    }

                                    ExportMethodInfo exportMth = new ExportMethodInfo();
                                    exportMth.info = fields[m];
                                    exportMth.memberName = fields[m].Name;
                                    vAutos.Add(exportMth);
                                    vEpxots.Add(exportMth);
                                }
                            }
                            PropertyInfo[] pros = types[i].GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                            for (int m = 0; m < pros.Length; ++m)
                            {
                                if (pros[m].IsDefined(typeof(ATFieldAttribute), false))
                                {
                                    ATFieldAttribute attr = (ATFieldAttribute)pros[m].GetCustomAttribute(typeof(ATFieldAttribute));
                                    List<ExportMethodInfo> vEpxots;
                                    if (!vExportMonos.TryGetValue(tp, out vEpxots))
                                    {
                                        vEpxots = new List<ExportMethodInfo>();
                                        vExportMonos.Add(tp, vEpxots);
                                    }

                                    ExportMethodInfo exportMth = new ExportMethodInfo();
                                    exportMth.info = pros[m];
                                    exportMth.memberName = pros[m].Name;
                                    vAutos.Add(exportMth);
                                    vEpxots.Add(exportMth);
                                }
                            }
                        }
                    }
                    else if (tp.IsDefined(typeof(ATExportAttribute), false))
                    {
                        EXPORTCLASS.Add(tp.FullName.ToLower(), tp);
                        Dictionary<string, int> vMethodNames = new Dictionary<string, int>();
                        ATExportAttribute exportAttr = (ATExportAttribute)tp.GetCustomAttribute(typeof(ATExportAttribute));
                        MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        for (int m = 0; m < meths.Length; ++m)
                        {
                            if (meths[m].IsDefined(typeof(ATMethodAttribute), false))
                            {
                                ATMethodAttribute attr = (ATMethodAttribute)meths[m].GetCustomAttribute(typeof(ATMethodAttribute));
                                if (attr.bAutoGenerator && attr.ActionType != (int)EActionType.None)
                                {

                                    int memberCnt = 0;
                                    if (!vMethodNames.TryGetValue(meths[m].Name, out memberCnt))
                                    {
                                        memberCnt = 1;
                                        vMethodNames[meths[m].Name] = memberCnt;
                                    }
                                    else
                                        vMethodNames[meths[m].Name] = ++memberCnt;

                                    ExportMethodInfo exportMth = new ExportMethodInfo();
                                    exportMth.info = meths[m];
                                    if (memberCnt <= 1)
                                        exportMth.memberName = meths[m].Name;
                                    else exportMth.memberName = meths[m].Name + "_" + (memberCnt-1).ToString();
                                    vAutos.Add(exportMth);
                                }
                            }
                        }
                    }
                    else if(tp.IsInterface && tp != typeof(IUserData))
                    {
                        Type[]  intefaces = tp.GetInterfaces();
                        if(intefaces != null)
                        {
                            for(int k = 0; k < intefaces.Length; ++k)
                            {
                                if(intefaces[k] == typeof(IUserData))
                                {
                                    EXPORTCLASS.Add(tp.FullName.ToLower(), tp);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            EXPORTCLASS.Add("unityengine.renderer", typeof(UnityEngine.Renderer));
            EXPORTCLASS.Add("unityengine.monobehaviour", typeof(UnityEngine.MonoBehaviour));
            Dictionary<string, string> vAllFiles = new Dictionary<string, string>();
            //switch entry
            {
                string strAllCode = "#if UNITY_EDITOR\r\n";
                strAllCode += "//auto generator\r\n";
                strAllCode += "using System.Collections;\r\n";
                strAllCode += "using System.Collections.Generic;\r\n";
                strAllCode += "using UnityEngine;\r\n";
                strAllCode += "using System;\r\n";
                strAllCode += "using UnityEditor;\r\n";
                strAllCode += "using System.Linq;\r\n";
                strAllCode += "namespace Framework.Plugin.AT\r\n";
                strAllCode += "{\r\n";
                strAllCode += "\t[GraphNodeDrawProperty]\r\n";
                strAllCode += "\tpublic partial class GraphNodeLayoutGUI\r\n";
                strAllCode += "\t{\r\n";
                strAllCode += "\t\t[GraphNodeDrawProperty]\r\n";
                strAllCode += "\t\tpublic static bool DrawProperty(GraphNode pNode)\r\n";
                strAllCode += "\t\t{\r\n";
                EditorUtility.DisplayProgressBar("导出AT 图形绘制函数", "", 0);
                strAllCode += "\t\t\tswitch((int)pNode.BindNode.GetExcudeHash())\r\n";
                strAllCode += "\t\t\t{\r\n";
                for (int i = 0; i < vAutos.Count; ++i)
                {
                    Type type = vAutos[i].DeclaringType;
                    if (vAutos.Count > 0) EditorUtility.DisplayProgressBar("导出AT 图形绘制函数", type.ToString(), (float)((float)i / (float)vAutos.Count));
                    ATExportMonoAttribute exportAttr = GetExportAttr(type);

                    int ActionType = BuildMonoFuncID(type, vAutos[i]);
                    string tempDrawCode = "";
                    if (vAutos[i].info is FieldInfo)
                    {
                        ATFieldAttribute atFied = vAutos[i].GetCustomAttribute<ATFieldAttribute>();
                        if (atFied.CanGet)
                        {
                            string strFunc = BuildMonoDrawFuncName(type, vAutos[i], "get") + "(pNode)";
                            tempDrawCode += "\t\t\t\tcase " + BuildMonoFuncID(type, vAutos[i], "get") + ": return " + strFunc + ";\r\n";

                        }
                        if (atFied.CanSet)
                        {
                            string strFunc = BuildMonoDrawFuncName(type, vAutos[i], "set") + "(pNode)";
                            tempDrawCode += "\t\t\t\tcase " + BuildMonoFuncID(type, vAutos[i], "set") + ": return " + strFunc + ";\r\n";
                        }
                    }
                    else if (vAutos[i].info is PropertyInfo)
                    {
                        PropertyInfo prop = vAutos[i].info as PropertyInfo;
                        ATFieldAttribute atFied = vAutos[i].GetCustomAttribute<ATFieldAttribute>();
                        if (prop.CanRead && atFied.CanGet)
                        {
                            string strFunc = BuildMonoDrawFuncName(type, vAutos[i], "get") + "(pNode)";
                            tempDrawCode += "\t\t\t\tcase " + BuildMonoFuncID(type, vAutos[i], "get") + ": return " + strFunc + ";\r\n";
                        }
                        if (prop.CanWrite && atFied.CanSet)
                        {
                            string strFunc = BuildMonoDrawFuncName(type, vAutos[i], "set") + "(pNode)";
                            tempDrawCode += "\t\t\t\tcase " + BuildMonoFuncID(type, vAutos[i], "set") + ": return " + strFunc + ";\r\n";
                        }
                    }
                    else
                    {
                        string strFunc = BuildMonoDrawFuncName(type, vAutos[i]) + "(pNode)";
                        tempDrawCode += "\t\t\t\tcase " + ActionType + ": return " + strFunc + ";\r\n";
                    }
                    if(!string.IsNullOrEmpty(tempDrawCode))
                    {
                        if(!string.IsNullOrEmpty( exportAttr.marcoDefine))
                        {
                            strAllCode += "\t\t\t\t#if " + exportAttr.marcoDefine + "\r\n";
                        }
                        strAllCode += tempDrawCode;
                        if (!string.IsNullOrEmpty(exportAttr.marcoDefine))
                        {
                            strAllCode += "\t\t\t\t#endif\r\n";
                        }
                    }
                }
                strAllCode += "\t\t\t}\r\n";
                strAllCode += "\t\t\treturn false;\r\n";
                strAllCode += "\t\t}\r\n";

                strAllCode += "\t}\r\n";
                strAllCode += "}\r\n";
                strAllCode += "#endif\r\n";

                vAllFiles[Path.Combine(AgentTreeEditorPath.BuildGeneratorEditorPath(), "GraphNodeLayoutGUI.cs").Replace("\\", "/")] = strAllCode;
            }

            EditorUtility.ClearProgressBar();

            Dictionary<string, string> subCodes = new Dictionary<string, string>();
            Dictionary<string, ATExportMonoAttribute> vTypeExportAttrs = new Dictionary<string, ATExportMonoAttribute>();
            EditorUtility.DisplayProgressBar("导出AT 图形绘制", "", 0);
            for (int i = 0; i < vAutos.Count; ++i)
            {
                string file = Path.Combine(AgentTreeEditorPath.BuildGeneratorEditorPath(), "GraphNodeLayoutGUI_" + vAutos[i].DeclaringType.Name + ".cs").Replace("\\", "/");
                string strAllCode;
                if(!subCodes.TryGetValue(file, out strAllCode))
                {
                    strAllCode = "";
                }

                Type type = vAutos[i].DeclaringType;
                if (vAutos.Count > 0) EditorUtility.DisplayProgressBar("导出AT 图形绘制", type.ToString(), (float)((float)i / (float)vAutos.Count));

                ATExportMonoAttribute exportAttr = GetExportAttr(type);
                vTypeExportAttrs[file] = exportAttr;

                ATMethodReturnAttribute[] returnArgvs = null;
                ATMethodArgvAttribute[] argvs = null;

                ATMethodArgvAttribute pointerAttr = null;
                if (exportAttr.bMono)
                {
                    bool bMono = false;
                    bool bUserClass = false;
                    CheckClassOrMono(type, ref bMono, ref bUserClass);
                    if (bMono)
                        pointerAttr = new ATMethodArgvAttribute(typeof(VariableMonoScript), "pPointer", false, type, null, "", false, -1, true);
                    else if (bUserClass)
                        pointerAttr = new ATMethodArgvAttribute(typeof(VariableUser), "pPointer", false, type, null, "", false, -1, true);
                    if (pointerAttr == null)
                    {
                        if (IsStatic(vAutos[i]))
                        {
                            pointerAttr = new ATMethodArgvAttribute(typeof(VariableInt), "ClassHashCode", false, type, null, "", false, -1, true, false);
                        }
                    }

                }
                if(vAutos[i].info is FieldInfo)
                {
                    ATFieldAttribute pField = vAutos[i].GetCustomAttribute<ATFieldAttribute>();
                    if (pField.CanGet)
                    {
                        returnArgvs = (ATMethodReturnAttribute[])vAutos[i].GetCustomAttributes<ATMethodReturnAttribute>();
                        argvs = (ATMethodArgvAttribute[])vAutos[i].GetCustomAttributes<ATMethodArgvAttribute>();
                        if (argvs == null || argvs.Length <= 0) argvs = BuildMonoArgvAttr(type, vAutos[i], "get");
                        if (returnArgvs == null || returnArgvs.Length <= 0) returnArgvs = BuildMonoReturnAttr(type, vAutos[i],"get");

                        if (pointerAttr != null)
                        {
                            List<ATMethodArgvAttribute> tempagrs = argvs != null ? new List<ATMethodArgvAttribute>(argvs) : new List<ATMethodArgvAttribute>();
                            tempagrs.Insert(0, pointerAttr);
                            argvs = tempagrs.ToArray();
                        }
                        string strCode = BuildDrawFunc(type, vAutos[i], returnArgvs, argvs, pointerAttr, "get");
                        strAllCode += strCode;
                    }

                    if(pField.CanSet)
                    {
                        returnArgvs = (ATMethodReturnAttribute[])vAutos[i].GetCustomAttributes<ATMethodReturnAttribute>();
                        argvs = (ATMethodArgvAttribute[])vAutos[i].GetCustomAttributes<ATMethodArgvAttribute>();
                        if (argvs == null || argvs.Length <= 0) argvs = BuildMonoArgvAttr(type, vAutos[i], "set");
                        if (returnArgvs == null || returnArgvs.Length <= 0) returnArgvs = BuildMonoReturnAttr(type, vAutos[i], "");

                        if (pointerAttr != null)
                        {
                            List<ATMethodArgvAttribute> tempagrs = argvs != null ? new List<ATMethodArgvAttribute>(argvs) : new List<ATMethodArgvAttribute>();
                            tempagrs.Insert(0, pointerAttr);
                            argvs = tempagrs.ToArray();
                        }
                        string strCode = BuildDrawFunc(type, vAutos[i], returnArgvs, argvs, pointerAttr, "set");
                        strAllCode += strCode;
                    }

                }
                else if (vAutos[i].info is PropertyInfo)
                {
                    PropertyInfo prop = vAutos[i].info as PropertyInfo;
                    ATFieldAttribute atFied = vAutos[i].GetCustomAttribute<ATFieldAttribute>();
                    if (prop.CanRead && atFied.CanGet)
                    {
                        returnArgvs = (ATMethodReturnAttribute[])vAutos[i].GetCustomAttributes<ATMethodReturnAttribute>();
                        argvs = (ATMethodArgvAttribute[])vAutos[i].GetCustomAttributes<ATMethodArgvAttribute>();
                        if (argvs == null || argvs.Length <= 0) argvs = BuildMonoArgvAttr(type, vAutos[i], "get");
                        if (returnArgvs == null || returnArgvs.Length <= 0) returnArgvs = BuildMonoReturnAttr(type, vAutos[i], "get");

                        if (pointerAttr != null)
                        {
                            List<ATMethodArgvAttribute> tempagrs = argvs != null ? new List<ATMethodArgvAttribute>(argvs) : new List<ATMethodArgvAttribute>();
                            tempagrs.Insert(0, pointerAttr);
                            argvs = tempagrs.ToArray();
                        }

                        string strCode = BuildDrawFunc(type, vAutos[i], returnArgvs, argvs, pointerAttr, "get");
                        strAllCode += strCode;
                    }

                    if (prop.CanWrite && atFied.CanSet)
                    {
                        returnArgvs = (ATMethodReturnAttribute[])vAutos[i].GetCustomAttributes<ATMethodReturnAttribute>();
                        argvs = (ATMethodArgvAttribute[])vAutos[i].GetCustomAttributes<ATMethodArgvAttribute>();
                        if (argvs == null || argvs.Length <= 0) argvs = BuildMonoArgvAttr(type, vAutos[i], "set");
                        if (returnArgvs == null || returnArgvs.Length <= 0) returnArgvs = BuildMonoReturnAttr(type, vAutos[i], "");

                        if (pointerAttr != null)
                        {
                            List<ATMethodArgvAttribute> tempagrs = argvs != null ? new List<ATMethodArgvAttribute>(argvs) : new List<ATMethodArgvAttribute>();
                            tempagrs.Insert(0, pointerAttr);
                            argvs = tempagrs.ToArray();
                        }

                        string strCode = BuildDrawFunc(type, vAutos[i], returnArgvs, argvs, pointerAttr, "set");
                        strAllCode += strCode;
                    }
                }
                else
                {
                    returnArgvs = (ATMethodReturnAttribute[])vAutos[i].GetCustomAttributes<ATMethodReturnAttribute>();
                    argvs = (ATMethodArgvAttribute[])vAutos[i].GetCustomAttributes<ATMethodArgvAttribute>();

                    if (argvs == null || argvs.Length <= 0) argvs = BuildMonoArgvAttr(type, vAutos[i],"");
                    if (returnArgvs == null || returnArgvs.Length <= 0) returnArgvs = BuildMonoReturnAttr(type, vAutos[i],"");

                    if (pointerAttr != null)
                    {
                        List<ATMethodArgvAttribute> tempagrs = argvs != null ? new List<ATMethodArgvAttribute>(argvs) : new List<ATMethodArgvAttribute>();
                        tempagrs.Insert(0, pointerAttr);
                        argvs = tempagrs.ToArray();
                    }

                    string strCode = BuildDrawFunc(type, vAutos[i], returnArgvs, argvs, pointerAttr);
                    strAllCode += strCode;
                }

                subCodes[file] = strAllCode;
            }

            EditorUtility.ClearProgressBar();

            foreach (var db in subCodes)
            {
                string strAllCode = "#if UNITY_EDITOR\r\n";
                if (vTypeExportAttrs.TryGetValue(db.Key, out var epxortAttr))
                {
                    if(!string.IsNullOrEmpty(epxortAttr.marcoDefine))
                    {
                        strAllCode += "#if " + epxortAttr.marcoDefine + "\r\n";
                    }
                }

                strAllCode += "//auto generator\r\n";
                strAllCode += "using System.Collections;\r\n";
                strAllCode += "using System.Collections.Generic;\r\n";
                strAllCode += "using UnityEngine;\r\n";
                strAllCode += "using System;\r\n";
                strAllCode += "using UnityEditor;\r\n";
                strAllCode += "using System.Linq;\r\n";
                strAllCode += "namespace Framework.Plugin.AT\r\n";
                strAllCode += "{\r\n";
                strAllCode += "\tpublic partial class GraphNodeLayoutGUI\r\n";
                strAllCode += "\t{\r\n";
                strAllCode += db.Value;
                strAllCode += "\t}\r\n";
                strAllCode += "}\r\n";
                if (epxortAttr!=null)
                {
                    if (!string.IsNullOrEmpty(epxortAttr.marcoDefine))
                    {
                        strAllCode += "#endif\r\n";
                    }
                }
                strAllCode += "#endif\r\n";

                vAllFiles[db.Key] = strAllCode;
            }

            HashSet<string> vFiles = new HashSet<string>();
            string buildPath = AgentTreeEditorPath.BuildGeneratorEditorPath();
            if(!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            string[] files = Directory.GetFiles(buildPath);
            for(int i = 0; i < files.Length; ++i)
            {
                if(files[i].EndsWith(".cs"))
                {
                    vFiles.Add(files[i].Replace("\\", "/"));
                }
            }
            foreach (var db in vAllFiles)
            {
                string path = db.Key;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                fs.Position = 0;
                fs.SetLength(0);
                writer.Write(db.Value);
                writer.Close();

                vFiles.Remove(path);
            }
            foreach(var db in vFiles)
            {
                File.Delete(db);
            }

            {
                BuildMonoClass(vExportMonos, vInheritUserDatas);
            }

            AssetDatabase.Refresh();
        }
        //------------------------------------------------------
        static string BuildDrawFunc(System.Type type, ExportMethodInfo memberRef, ATMethodReturnAttribute[] returnArgvs, ATMethodArgvAttribute[] argvs, ATMethodArgvAttribute pointerAttr,
            string strGetSetLabel="")
        {
            ATExportMonoAttribute exportAttr = GetExportAttr(type);


            ATCanVarBetweenAttribute varBetween = null;
            ATCanVarsAttribute varVars = null;
            if (memberRef.IsDefined(typeof(ATCanVarBetweenAttribute)))
            {
                varBetween = (ATCanVarBetweenAttribute)memberRef.GetCustomAttribute<ATCanVarBetweenAttribute>();
            }
            if (memberRef.IsDefined(typeof(ATCanVarsAttribute)))
            {
                varVars = (ATCanVarsAttribute)memberRef.GetCustomAttribute<ATCanVarsAttribute>();
            }

            bool bMono = false;
            bool bUserClass = false;
            CheckClassOrMono(type, ref bMono, ref bUserClass);

            bool bValid = true;

            int ActionType = BuildMonoFuncID(type, memberRef);

            string strFunc = BuildMonoDrawFuncName(type, memberRef, strGetSetLabel);
            string strCode = "\t\t[GraphNodeDrawGUIMethod(" + ActionType + ")]\r\n";
            strCode += "\t\tpublic static bool " + strFunc + "(GraphNode pNode)\r\n";
            strCode += "\t\t{\r\n";
            strCode += "\t\t\tbool bChanged = false;\r\n";
            bool bPopVariable = false;
            if (varBetween != null)
            {
                if (!varBetween.bAll && (varBetween.end < varBetween.begin || varBetween.begin == EVariableType.Null))
                {
                    bValid = false;
                }
                else
                {
                    bPopVariable = true;
                    strCode += "\t\t\tfloat labelWidth = EditorGUIUtility.labelWidth;\r\n";
                    strCode += "\t\t\tEditorGUIUtility.labelWidth = 84;\r\n";
                    strCode += "\t\t\tlong preValue = pNode.BindNode.GetCustomValue();\r\n";
                    strCode += "\t\t\tATExportNodeAttrData exportData = null;\r\n";
                    strCode += "\t\t\tAgentTreeEditorUtils.vPopVariables.Clear();\r\n";
                    strCode += "\t\t\tif (pNode.Editor != null && AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)pNode.BindNode.GetExcudeHash(), out exportData) && exportData.betweenVar != null)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += "\t\t\t\tint index = -1;\r\n";
                    strCode += "\t\t\t\tif(!exportData.betweenVar.bAll && exportData.betweenVar.end > exportData.betweenVar.begin && exportData.betweenVar.begin> EVariableType.Null)\r\n";
                    strCode += "\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\tfor(EVariableType i = exportData.betweenVar.begin; i <= exportData.betweenVar.end; ++i)\r\n";
                    strCode += "\t\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\t\tif (preValue == (int)i) index = AgentTreeEditorUtils.vPopVariables.Count;\r\n";
                    strCode += "\t\t\t\t\t\tAgentTreeEditorUtils.vPopVariables.Add(i.ToString());\r\n";
                    strCode += "\t\t\t\t\t}\t\n";
                    strCode += "\t\t\t\t}\r\n";
                    strCode += "\t\t\t\telse if(exportData.betweenVar.bAll)\r\n";
                    strCode += "\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\tforeach (EVariableType v in Enum.GetValues(typeof(EVariableType)))\r\n";
                    strCode += "\t\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\t\tif (preValue == (int)v) index = AgentTreeEditorUtils.vPopVariables.Count;\r\n";
                    strCode += "\t\t\t\t\t\tAgentTreeEditorUtils.vPopVariables.Add(v.ToString());\r\n";
                    strCode += "\t\t\t\t\t}\r\n";
                    strCode += "\t\t\t\t}\r\n";
                    strCode += "\t\t\t\tindex = EditorGUILayout.Popup(\"类型\", index, AgentTreeEditorUtils.vPopVariables.ToArray());\r\n";
                    strCode += "\t\t\t\tif (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));\r\n";
                    strCode += "\t\t\t\telse pNode.BindNode.SetCustomValue((int)exportData.betweenVar.begin);\r\n";
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\telse\r\n";
                    strCode += "\t\t\t{\r\n";
                    if (varBetween.bAll)
                    {
                        strCode += "\t\t\t\tforeach (EVariableType v in Enum.GetValues(typeof(EVariableType)))\r\n";
                        strCode += "\t\t\t\t{\r\n";
                        strCode += "\t\t\t\t\tAgentTreeEditorUtils.vPopVariables.Add(v.ToString());\r\n";
                        strCode += "\t\t\t\t}\r\n";

                        strCode += "\t\t\t\tint index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());\r\n";
                        strCode += "\t\t\t\tindex = EditorGUILayout.Popup(\"类型\", index, AgentTreeEditorUtils.vPopVariables.ToArray());\r\n";
                        strCode += "\t\t\t\tif(index>=0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));\r\n";
                        strCode += "\t\t\t\telse pNode.BindNode.SetCustomValue(1);\r\n";
                    }
                    else
                    {
                        for (int t = (int)varBetween.begin; t <= (int)varBetween.end; ++t)
                        {
                            strCode += "AgentTreeEditorUtils.vPopVariables.Add(" + "\"" + ((EVariableType)t).ToString() + "\");";
                            if (t % 5 == 0) strCode += "\t\t\t\t\r\n";
                        }
                        strCode += "\t\t\t\r\n";
                        strCode += "\t\t\t\tint index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());\r\n";
                        strCode += "\t\t\t\tindex = EditorGUILayout.Popup(\"类型\", index, AgentTreeEditorUtils.vPopVariables.ToArray());\r\n";
                        strCode += "\t\t\t\tif(index>=0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));\r\n";
                        strCode += "\t\t\t\telse pNode.BindNode.SetCustomValue(" + (int)varBetween.begin + ");\r\n";
                    }
                    strCode += "\t\t\t}\r\n";

                    strCode += "\t\t\tbChanged = preValue != pNode.BindNode.GetCustomValue();\r\n";
                    strCode += "\t\t\tEditorGUIUtility.labelWidth = labelWidth;\r\n";
                    strCode += "\t\t\t\tAgentTreeEditorUtils.vPopVariables.Clear();\r\n";
                }
            }
            else if (varVars != null)
            {
                if (varVars.vars.Length <= 0)
                {
                    bValid = false;
                }
                else
                {
                    bPopVariable = true;
                    strCode += "\t\t\tfloat labelWidth = EditorGUIUtility.labelWidth;\r\n";
                    strCode += "\t\t\tEditorGUIUtility.labelWidth = 84;\r\n";
                    strCode += "\t\t\tlong preValue = pNode.BindNode.GetCustomValue();\r\n";
                    strCode += "\t\t\tATExportNodeAttrData exportData = null;\r\n";
                    strCode += "\t\t\tAgentTreeEditorUtils.vPopVariables.Clear();\r\n";
                    strCode += "\t\t\tif (pNode.Editor != null && AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)pNode.BindNode.GetExcudeHash(), out exportData) && exportData.canVars != null)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += "\t\t\t\tint index = -1;\r\n";
                    strCode += "\t\t\t\tif(exportData.canVars.Length>0)\r\n";
                    strCode += "\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\tfor(int i = 0; i < exportData.canVars.Length; ++i)\r\n";
                    strCode += "\t\t\t\t\t{\r\n";
                    strCode += "\t\t\t\t\t\tif (preValue == (int)exportData.canVars[i]) index = AgentTreeEditorUtils.vPopVariables.Count;\r\n";
                    strCode += "\t\t\t\t\t\tAgentTreeEditorUtils.vPopVariables.Add(exportData.canVars[i].ToString());\r\n";
                    strCode += "\t\t\t\t\t}\t\n";
                    strCode += "\t\t\t\t}\r\n";
                    strCode += "\t\t\t\tindex = EditorGUILayout.Popup(\"类型\", index, AgentTreeEditorUtils.vPopVariables.ToArray());\r\n";
                    strCode += "\t\t\t\tif (index >= 0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));\r\n";
                    strCode += "\t\t\t\telse pNode.BindNode.SetCustomValue((int)exportData.canVars[0]);\r\n";
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\telse\r\n";
                    strCode += "\t\t\t{\r\n";

                    for (int t = 0; t < varVars.vars.Length; ++t)
                    {
                        strCode += "AgentTreeEditorUtils.vPopVariables.Add(" + "\"" + varVars.vars[t].ToString() + "\");";
                        if (t % 5 == 0) strCode += "\t\t\t\t\r\n";
                    }
                    strCode += "\t\t\t\r\n";
                    strCode += "\t\t\t\tint index = AgentTreeEditorUtils.vPopVariables.IndexOf(((EVariableType)preValue).ToString());\r\n";
                    strCode += "\t\t\t\tindex = EditorGUILayout.Popup(\"类型\", index, AgentTreeEditorUtils.vPopVariables.ToArray());\r\n";
                    strCode += "\t\t\t\tif(index>=0 && index < AgentTreeEditorUtils.vPopVariables.Count) pNode.BindNode.SetCustomValue((int)Enum.Parse(typeof(EVariableType), AgentTreeEditorUtils.vPopVariables[index]));\r\n";
                    strCode += "\t\t\t\telse pNode.BindNode.SetCustomValue(" + (int)varVars.vars[0] + ");\r\n";

                    strCode += "\t\t\t}\r\n";

                    strCode += "\t\t\tbChanged = preValue != pNode.BindNode.GetCustomValue();\r\n";
                    strCode += "\t\t\tEditorGUIUtility.labelWidth = labelWidth;\r\n";
                    strCode += "\t\t\t\tAgentTreeEditorUtils.vPopVariables.Clear();\r\n";
                }
            }
            else
            {
                if (argvs != null)
                {
                    for (int a = 0; a < argvs.Length; ++a)
                    {
                        if (argvs[a].ArgvType == null)
                        {
                            bValid = false;
                            break;
                        }
                    }
                }
                if (returnArgvs != null && returnArgvs.Length > 0)
                {
                    for (int a = 0; a < returnArgvs.Length; ++a)
                    {
                        if (returnArgvs[a].ReturnType == null)
                        {
                            bValid = false;
                            break;
                        }
                    }
                }
            }
            if (bValid)
            {
                if (argvs != null && argvs.Length > 0)
                {
                    strCode += "\t\t\tif(!bChanged && (";
                    strCode += "pNode.BindNode.inArgvs == null || pNode.BindNode.inArgvs.Length != " + argvs.Length + ")) bChanged = true;\r\n";
                }
                if (returnArgvs != null && returnArgvs.Length > 0)
                {
                    strCode += "\t\t\tif(!bChanged && (";
                    strCode += "pNode.BindNode.outArgvs == null || pNode.BindNode.outArgvs.Length != " + returnArgvs.Length + ")) bChanged = true;\r\n";
                }

                if (exportAttr.bMono || !bPopVariable)
                    strCode += "\t\t\tif(bChanged)\r\n";
                else
                    strCode += "\t\t\tif(bChanged && pNode.BindNode.GetCustomValue()>0)\r\n";
                strCode += "\t\t\t{\r\n";
                strCode += "\t\t\t\tVariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();\n";
                strCode += "\t\t\t\tpNode.Editor?.AdjustMaxGuid();\n";
                strCode += "\t\t\t\tList<Port> vInTemp = (pNode.BindNode.inArgvs!=null)?new List<Port>(pNode.BindNode.inArgvs):null;\r\n";
                strCode += "\t\t\t\tList<Port> vOutTemp = (pNode.BindNode.outArgvs!=null)?new List<Port>(pNode.BindNode.outArgvs):null;\r\n";
                strCode += "\t\t\t\tpNode.BindNode.ClearArgv();\r\n";
                if (argvs != null)
                {
                    for (int a = 0; a < argvs.Length; ++a)
                    {
                        int argvIndex = a;
                        strCode += "\t\t\t\t{\r\n";
                        strCode += "\t\t\t\t\tPort pvar=null;\r\n";
                        if (argvs[a].ListElementByArgvIndex >= 0 && argvs[a].ListElementByArgvIndex < argvs.Length)
                        {
                            strCode += "\t\t\t\t\tif( pNode.BindNode.GetInArgvCount() >=" + argvs[a].ListElementByArgvIndex + ")\r\n";
                            strCode += "\t\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\t\t Variable temp = pNode.BindNode.GetInVariable(" + argvs[a].ListElementByArgvIndex + ");\r\n";
                            strCode += "\t\t\t\t\t\tif(temp !=null && temp.IsList())\r\n";
                            strCode += "\t\t\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew(temp.GetListElementType(), pVarsFactor, vInTemp, "+ argvIndex + ");\r\n";

                            strCode += "\t\t\t\t\t\t}\r\n";
                            strCode += "\t\t\t\t\t}\r\n";
                            strCode += "\t\t\t\t\tif(pvar==null)\r\n";
                            strCode += "\t\t\t\t\t{\r\n";
                            Type argvType = argvs[a].ArgvType;
                            if (argvs[a].ArgvType != null)
                                strCode += "\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew<" + argvs[a].ArgvType.FullName + ">(pVarsFactor,vInTemp, "+ argvIndex + ");\r\n";
                            else
                                strCode += "\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew((EVariableType)pNode.BindNode.GetCustomValue(), pVarsFactor, vInTemp, " + argvIndex + ");\r\n";
                            strCode += "\t\t\t\t\t}\r\n";
                        }
                        else
                        {
                            Type argvType = argvs[a].ArgvType;
                            if (argvs[a].ArgvType != null)
                            {
                                strCode += "\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew<" + argvs[a].ArgvType.FullName + ">(pVarsFactor, vInTemp, " + argvIndex + ");\r\n";
                                if (argvs[a].defaultValue != null)
                                {
                                    if (argvs[a].defaultValue.GetType() == typeof(System.DBNull))
                                    {

                                    }
                                    else if (argvs[a].defaultValue.GetType() == typeof(System.Boolean))
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + argvs[a].defaultValue.ToString().ToLower() + ";\r\n";
                                    else if (argvs[a].defaultValue.GetType() == typeof(System.Single) || argvs[a].defaultValue.GetType() == typeof(System.Double))
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + argvs[a].defaultValue.ToString() + "f;\r\n";
                                    else if (argvs[a].defaultValue.GetType().IsEnum)
                                    {
                                        Enum val = (Enum)Enum.ToObject(argvs[a].defaultValue.GetType(), argvs[a].defaultValue);
                                        int valInt = Convert.ToInt32(val);
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + valInt + ";//"+ argvs[a].defaultValue.GetType().FullName.Replace("+",".") + "." + argvs[a].defaultValue.ToString() + "\r\n";
                                    }
                                    else if (argvs[a].defaultValue.GetType() == typeof(System.UInt16) || argvs[a].defaultValue.GetType() == typeof(System.UInt32) || argvs[a].defaultValue.GetType() == typeof(System.Byte))
                                    {
                                        int toValue = (int)long.Parse(argvs[a].defaultValue.ToString());
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + toValue + ";\r\n";
                                    }
                                    else if (argvs[a].defaultValue.GetType() == typeof(System.UInt64))
                                    {
                                        long toValue = long.Parse(argvs[a].defaultValue.ToString());
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + toValue + ";\r\n";
                                    }
                                    else if (argvs[a].defaultValue.GetType() == typeof(string))
                                    {
                                        strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = \"" + argvs[a].defaultValue.ToString() + "\";\r\n";
                                    }
                                    else strCode += "\t\t\t\t\tpvar.GetVariable<" + argvs[a].ArgvType.FullName + ">().mValue = " + argvs[a].defaultValue.ToString() + ";\r\n";
                                }
                            }
                            else
                                strCode += "\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew((EVariableType)pNode.BindNode.GetCustomValue(),pVarsFactor, vInTemp, " + argvIndex + ");\r\n";
                        }
                        //                             if (argvs[a].AlignType != null)
                        //                             {
                        //                                 strCode += "\t\t\t\t\tif(pvar is Plugin.AT.VariableUserList) (pvar as Plugin.AT.VariableUserList).hashCode = " + argvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableUser) (pvar as Plugin.AT.VariableUser).hashCode = " + argvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableMonoScript) (pvar as Plugin.AT.VariableMonoScript).hashCode = " + argvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableMonoScriptList) (pvar as Plugin.AT.VariableMonoScriptList).hashCode = " + argvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                             }

                //        strCode += "\t\t\t\t\tpvar.strName = \"" + /*argvs[a].DisplayName +*/ "\";\r\n";
                        if (argvs[a].bAutoDestroy)
                            strCode += "\t\t\t\t\tpvar.SetFlag(EFlag.AutoDestroy, true);\r\n";
                        strCode += "\t\t\t\t\tpNode.BindNode.AddInPort(pvar);\r\n";
                        if (argvs[a].bReturn)
                            strCode += "\t\t\t\t\tpNode.BindNode.AddOutPort(pvar);\r\n";
                        //       if (argvs[a].bSeriable)
                        //            strCode += "\t\t\t\t\tif(pNode.ATData!=null && pvar != null) pNode.ATData.Data.Locals.AddVariable(pvar);\r\n";
                        strCode += "\t\t\t\t}\r\n";
                    }
                }
                if (returnArgvs != null)
                {
                    for (int a = 0; a < returnArgvs.Length; ++a)
                    {
                        strCode += "\t\t\t\t{\r\n";
                        strCode += "\t\t\t\t\tPort pvar=null;\r\n";
                        if (returnArgvs[a].ListElementByArgvIndex >= 0 && returnArgvs[a].ListElementByArgvIndex < argvs.Length)
                        {
                            strCode += "\t\t\t\t\tif( pNode.BindNode.GetInArgvCount() >=" + returnArgvs[a].ListElementByArgvIndex + ")\r\n";
                            strCode += "\t\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\t\tif(pNode.BindNode.getInArgvs()[" + returnArgvs[a].ListElementByArgvIndex + "].IsList())\r\n";
                            strCode += "\t\t\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew(pNode.BindNode.getInArgvs()[" + returnArgvs[a].ListElementByArgvIndex + "].GetListElementType(),pVarsFactor, vOutTemp,"+a+");\r\n";

                            strCode += "\t\t\t\t\t\t}\r\n";
                            strCode += "\t\t\t\t\t}\r\n";
                            strCode += "\t\t\t\t\tif(pvar==null)\r\n";
                            strCode += "\t\t\t\t\t{\r\n";
                            if (returnArgvs[a].ReturnType != null)
                                strCode += "\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew<" + returnArgvs[a].ReturnType.FullName + ">(pVarsFactor, vOutTemp," + a + ");\r\n";
                            else
                                strCode += "\t\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew((EVariableType)pNode.BindNode.GetCustomValue(),pVarsFactor, vOutTemp," + a + ");\r\n";
                            strCode += "\t\t\t\t\t}\r\n";
                        }
                        else
                        {
                            if (returnArgvs[a].ReturnType != null)
                                strCode += "\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew<" + returnArgvs[a].ReturnType.FullName + ">(pVarsFactor, vOutTemp," + a + ");\r\n";
                            else
                                strCode += "\t\t\t\t\tpvar = AgentTreeEditorUtils.BuildOriVariableCommonNew((EVariableType)pNode.BindNode.GetCustomValue(),pVarsFactor, vOutTemp," + a + ");\r\n";
                        }

                        //                             if (returnArgvs[a].AlignType != null)
                        //                             {
                        //                                 strCode += "\t\t\t\t\tif(pvar is Plugin.AT.VariableUserList) (pvar as Plugin.AT.VariableUserList).hashCode = " + returnArgvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableUser) (pvar as Plugin.AT.VariableUser).hashCode = " + returnArgvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableMonoScript) (pvar as Plugin.AT.VariableMonoScript).hashCode = " + returnArgvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                                 strCode += "\t\t\t\t\telse if(pvar is Plugin.AT.VariableMonoScriptList) (pvar as Plugin.AT.VariableMonoScriptList).hashCode = " + returnArgvs[a].AlignType.GetHashCode() + ";\r\n";
                        //                             }
                   //     strCode += "\t\t\t\t\tpvar.strName = \"" + /*returnArgvs[a].Name +*/ "\";\r\n";
                        if (returnArgvs[a].bAutoDestroy)
                            strCode += "\t\t\t\t\tpvar.SetFlag(EFlag.AutoDestroy, true);\r\n";
                        strCode += "\t\t\t\t\tpNode.BindNode.AddOutPort(pvar);\r\n";
                        //      if (returnArgvs[a].bSeriable)
                        //           strCode += "\t\t\t\t\tif(pNode.ATData!=null && pvar != null) pNode.ATData.Data.Locals.AddVariable(pvar);\r\n";
                        strCode += "\t\t\t\t}\r\n";
                    }
                }
                strCode += "\t\t\t\tpNode.BindNode.Save();\r\n";
                strCode += "\t\t\t}\r\n";

                strCode += "\t\t\tATExportNodeAttrData attrData = pNode.Editor?.GetActionNodeAttr((int)pNode.BindNode.GetExcudeHash());\r\n";
                strCode += "\t\t\tif (attrData.nolinkAttr == null)\r\n";
                strCode += "\t\t\t{\r\n";
                strCode += "\t\t\t\tpNode.bLink = true;\r\n";
                strCode += "\t\t\t\tRect rect = GUILayoutUtility.GetLastRect();\r\n";
                strCode += "\t\t\t\tpNode.InLink.baseNode = pNode;\r\n";
                strCode += "\t\t\t\tpNode.InLink.direction = EPortIO.In;\r\n";
                strCode += "\t\t\t\tGraphNode.LinkField(new Vector2(rect.x - 10, 8), pNode.InLink);\r\n";
                strCode += "\t\t\t\tpNode.OutLink.baseNode = pNode;\r\n";
                strCode += "\t\t\t\tpNode.OutLink.direction = EPortIO.Out;\r\n";
                strCode += "\t\t\t\tGraphNode.LinkField(new Vector2(rect.width + 10, 8), pNode.OutLink);\r\n";
                strCode += "\t\t\t}\r\n";
                strCode += "\t\t\telse pNode.bLink = false;\r\n";

                strCode += "\t\t\tfor(int i = 0; i < pNode.BindNode.GetInArgvCount(); ++i)\r\n";
                strCode += "\t\t\t{\r\n";
                strCode += "\t\t\t\tpNode.DrawArgvInPort(i);\r\n";
                /*
                          strCode += "\t\t\t\tArgvPort port = pNode.BindNode.GetInEditorPort<ArgvPort>(i);\r\n";
                          strCode += "\t\t\t\tport.baseNode = pNode;\r\n";
                          strCode += "\t\t\t\tport.port = pNode.BindNode.GetInPort(i);\r\n";
                       //   strCode += "\t\t\t\tport.variable = pNode.BindNode.GetInVariable(i);\r\n";
                          strCode += "\t\t\t\tport.direction = EPortIO.In;\r\n";
                          //   strCode += "\t\t\t\tif (pNode.BindNode.getOutArgvs().Contains(pNode.BindNode.GetInVariable(i))) port.direction |= EPortIO.Out;\r\n";
                          strCode += "\t\t\t\tport.index = i;\r\n";
                          strCode += "\t\t\t\tbool bShowEdit = true;\r\n";
                          strCode += "\t\t\t\tif (attrData != null && i < attrData.InArgvs.Count)\r\n";
                          strCode += "\t\t\t\t{\t\n";
                          strCode += "\t\t\t\t\tport.SetDefaultName(attrData.InArgvs[i].DisplayName);\r\n";
                          strCode += "\t\t\t\t\tport.alignType = attrData.InArgvs[i].AlignType;\r\n";
                          strCode += "\t\t\t\t\tport.displayType = attrData.InArgvs[i].DisplayType;\r\n";
                          strCode += "\t\t\t\t\tbShowEdit = attrData.InArgvs[i].bShowEdit;\r\n";
                          strCode += "\t\t\t\t\tif (port.alignType == null && port.port.dummyMap != null && port.port.dummyMap.Count > 0)\r\n";
                          strCode += "\t\t\t\t\t{\r\n";
                          strCode += "\t\t\t\t\t\tVariable var = port.port.dummyMap.ElementAt(0).Value;\r\n";
                          strCode += "\t\t\t\t\t\tif (var != null && var.GetClassHashCode() != 0)\r\n";
                          strCode += "\t\t\t\t\t\t\tAgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);\r\n";
                          strCode += "\t\t\t\t\t}\r\n";
                          strCode += "\t\t\t\t\tif(!bShowEdit && port.alignType!=null)\r\n";
                          strCode += "\t\t\t\t\t{\r\n";
                          strCode += "\t\t\t\t\t\tif(port.variable!=null)port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));\r\n";
                          strCode += "\t\t\t\t\t}\r\n";
                          strCode += "\t\t\t\t}\r\n";
                          strCode += "\t\t\t\tif(bShowEdit)DrawPropertyGUI.DrawVariable(pNode, port);\r\n";
                          strCode += "\t\t\t\tif(port.variable!=null)pNode.Inputs.Add(port);\r\n";
                          */
                strCode += "\t\t\t}\r\n";

                strCode += "\t\t\tfor(int i = 0; i < pNode.BindNode.GetOutArgvCount(); ++i)\r\n";
                strCode += "\t\t\t{\r\n";
                strCode += "\t\t\t\tpNode.DrawArgvOutPort(i);\r\n";
                /*
                strCode += "\t\t\t\tArgvPort port = pNode.BindNode.GetOutEditorPort<ArgvPort>(i);\r\n";
                strCode += "\t\t\t\tport.baseNode = pNode;\r\n";
                strCode += "\t\t\t\tport.port = pNode.BindNode.GetOutPort(i);\r\n";
           //     strCode += "\t\t\t\tport.variable = pNode.BindNode.GetOutVariable(i);\r\n";
                strCode += "\t\t\t\tport.direction = EPortIO.Out;\r\n";
                strCode += "\t\t\t\tport.index = i;\r\n";
                strCode += "\t\t\t\tbool bShowEdit = true;\r\n";
                strCode += "\t\t\t\tif (attrData != null && i < attrData.OutArgvs.Count)\r\n";
                strCode += "\t\t\t\t{\r\n";
                strCode += "\t\t\t\t\tport.SetDefaultName(attrData.OutArgvs[i].Name);\r\n";
                strCode += "\t\t\t\t\tport.alignType = attrData.OutArgvs[i].AlignType;\r\n";
                strCode += "\t\t\t\t\tport.displayType = attrData.OutArgvs[i].DisplayType;\r\n";
                strCode += "\t\t\t\t\tbShowEdit = attrData.OutArgvs[i].bShowEdit;\r\n";
                strCode += "\t\t\t\t\tif (port.alignType == null && port.port.dummyMap != null && port.port.dummyMap.Count > 0)\r\n";
                strCode += "\t\t\t\t\t{\r\n";
                strCode += "\t\t\t\t\t\tVariable var = port.port.dummyMap.ElementAt(0).Value;\r\n";
                strCode += "\t\t\t\t\t\tif (var != null && var.GetClassHashCode() != 0)\r\n";
                strCode += "\t\t\t\t\t\t\tAgentTreeUtl.ExportClasses.TryGetValue(var.GetClassHashCode(), out port.alignType);\r\n";
                strCode += "\t\t\t\t\t}\r\n";
                strCode += "\t\t\t\t\tif(!bShowEdit && port.alignType!=null)\r\n";
                strCode += "\t\t\t\t\t{\r\n";
                strCode += "\t\t\t\t\t\tif(port.variable!=null)port.variable.SetClassHashCode(AgentTreeUtl.TypeToHash(port.alignType));\r\n";
                strCode += "\t\t\t\t\t}\r\n";
                strCode += "\t\t\t\t}\r\n";
                strCode += "\t\t\t\tif(bShowEdit)\r\n";
                strCode += "\t\t\t\t{\r\n";
                strCode += "\t\t\t\t\tint checkIndex = pNode.BindNode.IndexofInArgv(port.variable);\r\n";
                strCode += "\t\t\t\t\tif (checkIndex!=-1 && checkIndex<pNode.Inputs.Count)\r\n";
                strCode += "\t\t\t\t\t\tDrawPropertyGUI.DrawVariable(pNode, port, pNode.Inputs[checkIndex]);\r\n";
                strCode += "\t\t\t\t\telse DrawPropertyGUI.DrawVariable(pNode, port);\r\n";
                strCode += "\t\t\t\t}\r\n";
                strCode += "\t\t\t\tif(port.variable!=null)pNode.Outputs.Add(port);\r\n";
                */
                strCode += "\t\t\t}\r\n";
            }
            strCode += "\t\t\treturn true;\r\n";
            strCode += "\t\t}\r\n";
            return strCode;
        }
        //------------------------------------------------------
        public static int BuildMonoFuncID(Type classType, ExportMethodInfo memthod, string label = "")
        {
            ATExportGUIDAttribute guide_attr = (ATExportGUIDAttribute)memthod.GetCustomAttribute<ATExportGUIDAttribute>(false);
            if (guide_attr != null && guide_attr.hashCode != 0) return guide_attr.hashCode;
            if (memthod.info is MethodInfo)
            {
                ATMethodAttribute attr = (ATMethodAttribute)memthod.GetCustomAttribute<ATMethodAttribute>();
                MethodInfo methodInfo = (MethodInfo)memthod.info;
                if (attr.ActionType == 0)
                {
                    string strParamName = "";
                    for (int i = 0; i < methodInfo.GetParameters().Length; ++i)
                        strParamName += methodInfo.GetParameters()[i].ParameterType.Name;

                    if (methodInfo.ReturnParameter != null)
                        strParamName += methodInfo.ReturnParameter.ParameterType.Name;
                    if (string.IsNullOrEmpty(label))
                        return Animator.StringToHash(classType.FullName + "_" + memthod.memberName + "_" + strParamName);
                    else
                        return Animator.StringToHash(classType.FullName + "_" + label + "_" + memthod.memberName + "_" + strParamName);
                }
                return attr.ActionType;
            }
            else
            {
                ATFieldAttribute attr = (ATFieldAttribute)memthod.GetCustomAttribute<ATFieldAttribute>();
                if (string.IsNullOrEmpty(label))
                    return Animator.StringToHash(classType.FullName + "_" + memthod.memberName);
                else
                    return Animator.StringToHash(classType.FullName + "_" + label + "_" + memthod.memberName);
            }
        }
        //------------------------------------------------------
        public static string BuildMonoDrawFuncName(Type classType, ExportMethodInfo memthod, string label = "")
        {
            if(memthod.info is MethodInfo)
            {
                MethodInfo methodInfo = memthod.info as MethodInfo;
                ATMethodAttribute attr = memthod.GetCustomAttribute<ATMethodAttribute>();
                if (attr.ActionType == 0)
                {
                    string strParamName = "";
                    for (int i = 0; i < methodInfo.GetParameters().Length; ++i)
                        strParamName += methodInfo.GetParameters()[i].ParameterType.Name;

                    if (methodInfo.ReturnParameter != null)
                        strParamName += methodInfo.ReturnParameter.ParameterType.Name;
                    string strFunName = "";
                    if (string.IsNullOrEmpty(label))
                        strFunName = classType.FullName + "_" + memthod.memberName + "_" + strParamName;
                    else
                        strFunName = classType.FullName + "_" + label + "_" + memthod.memberName + "_" + strParamName;

                    string strRes = "";
                    for (int i = 0; i < strFunName.Length; ++i)
                    {
                        char code = strFunName[i];
                        if (((code >= '0' && code <= '9') ||
                            (code >= 'A' && code <= 'Z') ||
                            (code >= 'a' && code <= 'z') || code == '_'))
                        {
                            strRes += code;
                        }
                        else
                            strRes += '_';
                    }
                    return strRes;
                }
                if (string.IsNullOrEmpty(label))
                    return classType.Name + "_Draw" + memthod.memberName;
                else
                    return classType.Name + "_Draw" + "_" + label + "_" + memthod.memberName;
            }
            else
            {
                ATFieldAttribute attr = (ATFieldAttribute)memthod.GetCustomAttribute<ATFieldAttribute>();
                if (string.IsNullOrEmpty(label))
                    return classType.Name + "_Draw" + memthod.memberName;
                else
                    return classType.Name + "_Draw" + "_" + label + "_" + memthod.memberName;
            }
        }
        //------------------------------------------------------
        class StructDelca
        {
            public string strName;
            public string strClassName;
            public string strCode;
            public string strCodeArray;
        }
        //------------------------------------------------------
        static void BuildMemberCode(ATExportMonoAttribute exportAttr, Type classType, ExportMethodInfo memberInfo,
            ref System.Collections.Generic.Dictionary<string, List<StructDelca>> vStaticDelca, 
            ref string strClassName, ref string strCode, ref string strClassMapping, ref string strSwitchDo, string strLabelFuncName="")
        {
            string strDisplayName = memberInfo.memberName;
            if(!string.IsNullOrEmpty(strLabelFuncName))
                strDisplayName = strLabelFuncName + "_" + memberInfo.memberName;
            ATMethodAttribute methodAttr = memberInfo.GetCustomAttribute<ATMethodAttribute>();
            if (methodAttr != null && !string.IsNullOrEmpty(methodAttr.DisplayName))
            {
                if (string.IsNullOrEmpty(methodAttr.DisplayName)) methodAttr.DisplayName = memberInfo.memberName;
                if (!string.IsNullOrEmpty(strLabelFuncName))
                    strDisplayName = strLabelFuncName + "_" + methodAttr.DisplayName;
                else
                    strDisplayName = methodAttr.DisplayName;
            }
            ATDefaultPointerAttribute defaultPointerAttr = memberInfo.GetCustomAttribute<ATDefaultPointerAttribute>();
            int funcID = BuildMonoFuncID(classType, memberInfo, strLabelFuncName);
            strCode += "#if UNITY_EDITOR\r\n";
            if (IsMember(memberInfo) && strLabelFuncName == "get")
                strCode += "\t\t[ATMonoFunc(" + funcID + ",\"" + strDisplayName + "\"" + ",typeof(" + classType.FullName.Replace("+", ".") + "), true)]\r\n";
            else
                strCode += "\t\t[ATMonoFunc(" + funcID + ",\"" + strDisplayName + "\"" + ",typeof(" + classType.FullName.Replace("+", ".") + "))]\r\n";
            ATMethodArgvAttribute[] argvAttrs = BuildMonoArgvAttr(classType, memberInfo, strLabelFuncName);
            ATMethodReturnAttribute[] returnAttrs = BuildMonoReturnAttr(classType, memberInfo, strLabelFuncName);

            bool bMono = false;
            bool bUserClass = false;
            CheckClassOrMono(classType, ref bMono, ref bUserClass);
            ATMethodArgvAttribute pointerAttr = null;

            if (bMono)
                pointerAttr = new ATMethodArgvAttribute(typeof(VariableMonoScript), "pPointer", false, classType, null, "", false, -1, true, exportAttr.bGlobalModule== EGlobalType.None);
            else if (bUserClass)
                pointerAttr = new ATMethodArgvAttribute(typeof(VariableUser), "pPointer", false, classType, null, "", false, -1, true, exportAttr.bGlobalModule == EGlobalType.None);
            if (pointerAttr == null)
            {
                if (IsStatic(memberInfo))
                {
                    pointerAttr = new ATMethodArgvAttribute(typeof(VariableInt), "ClassHashCode", false, classType, null, "", false, -1, true, false);
                }
            }
            if (pointerAttr != null)
            {
                string alignType = "null";
                string displayType = "null";
                if (pointerAttr.DisplayType != null)
                {
                    displayType = "typeof(" + pointerAttr.DisplayType.FullName.Replace("+", ".") + ")";
                }
                if (pointerAttr.AlignType != null)
                {
                    alignType = "typeof(" + pointerAttr.AlignType.FullName.Replace("+", ".") + ")";
                }
                if (IsStatic(memberInfo) || exportAttr.bGlobalModule != EGlobalType.None)
                    strCode += "\t\t[ATMethodArgv(typeof(" + pointerAttr.ArgvType.Name + "),\"" + pointerAttr.DisplayName + "\"" + ",false, " + alignType + "," + displayType + ", \"\", false, -1, true,false)]\r\n";
                else
                    strCode += "\t\t[ATMethodArgv(typeof(" + pointerAttr.ArgvType.Name + "),\"" + pointerAttr.DisplayName + "\"" + ",false,  " + alignType + "," + displayType + ")]\r\n";
            }
            if (argvAttrs != null)
            {
                for (int j = 0; j < argvAttrs.Length; ++j)
                {
                    string alignType = "null";
                    string displayType = "null";
                    if (argvAttrs[j].DisplayType != null)
                    {
                        displayType = "typeof(" + argvAttrs[j].DisplayType.FullName.Replace("+", ".") + ")";
                    }
                    if (argvAttrs[j].AlignType != null)
                    {
                        alignType = "typeof(" + argvAttrs[j].AlignType.FullName.Replace("+", ".") + ")";
                    }
                    strCode += "\t\t[ATMethodArgv(typeof(" + argvAttrs[j].ArgvType.Name + "),\"" + argvAttrs[j].DisplayName + "\"" + ",false, " + alignType + "," + displayType + ")]\r\n";
                }
            }


            if (returnAttrs != null && returnAttrs.Length > 0)
            {
                for (int j = 0; j < returnAttrs.Length; ++j)
                {
                    string alignType = "null";
                    string displayType = "null";
                    if (returnAttrs[j].DisplayType != null)
                    {
                        displayType = "typeof(" + returnAttrs[j].DisplayType.FullName.Replace("+", ".") + ")";
                    }
                    if (returnAttrs[j].AlignType != null)
                    {
                        alignType = "typeof(" + returnAttrs[j].AlignType.FullName.Replace("+", ".") + ")";
                    }
                    strCode += "\t\t[ATMethodReturn(typeof(" + returnAttrs[j].ReturnType.Name + "),\"" + returnAttrs[j].Name + "\"" + "," + alignType + "," + displayType + ")]\r\n";
                }
            }
            strCode += "#endif\r\n";

            string strFunc = "AT_" + memberInfo.memberName;
            if(!string.IsNullOrEmpty(strLabelFuncName))
                strFunc = "AT_" + strLabelFuncName + "_" + memberInfo.memberName;

            if (IsStatic(memberInfo))
            {
                strCode += "\t\tpublic static bool " + strFunc + "(";
                string strArgv = "";
                if (argvAttrs != null)
                {
                    for (int j = 0; j < argvAttrs.Length; ++j)
                    {
                        strArgv += argvAttrs[j].ArgvType.Name + " " + argvAttrs[j].DisplayName + ",";
                    }
                }
                if (returnAttrs != null)
                {
                    for (int j = 0; j < returnAttrs.Length; ++j)
                        strArgv += returnAttrs[j].ReturnType.Name + " " + returnAttrs[j].Name + "=null" + ",";
                }
                if (strArgv.Length > 1)
                {
                    strArgv = strArgv.Substring(0, strArgv.Length - 1);
                    strCode += strArgv;
                }
                strCode += ")\r\n";
            }
            else
            {
                strCode += "\t\tpublic static bool " + strFunc + "(" + classType.FullName.Replace("+", ".") + " pPointer";
                if (argvAttrs != null)
                {
                    for (int j = 0; j < argvAttrs.Length; ++j)
                    {
                        string strLabel = argvAttrs[j].DisplayName;
                        if (IsMember(memberInfo))
                        {
                            strLabel = memberInfo.memberName;
                        }
                        strCode += ", " + argvAttrs[j].ArgvType.Name + " " + strLabel;
                    }
                }
                if (returnAttrs != null)
                {
                    for (int j = 0; j < returnAttrs.Length; ++j)
                        strCode += ", " + returnAttrs[j].ReturnType.Name + " " + returnAttrs[j].Name + "=null";
                }
                strCode += ")\r\n";
            }

            strCode += "\t\t{\r\n";

            bool bValid = true;
            string strCheck = "";
            if (argvAttrs != null && argvAttrs.Length > 0)
            {
                bool bObjVar = false;
                strCheck += "\t\t\tif(";
                for (int j = 0; j < argvAttrs.Length; ++j)
                {
                    string strLabel = argvAttrs[j].DisplayName;
                    if (IsMember(memberInfo))
                    {
                        strLabel = memberInfo.memberName;
                    }
                    if (string.IsNullOrEmpty(strLabel))
                    {
                        bValid = false;
                        break;
                    }

                    if (argvAttrs[j].ArgvType == typeof(VariableObject) || argvAttrs[j].ArgvType == typeof(VariableUser) || argvAttrs[j].ArgvType == typeof(VariableMonoScript))
                        bObjVar = true;
                    strCheck += strLabel + "== null";
                    if (j < argvAttrs.Length - 1) strCheck += " || ";
                }
                strCheck += ")\r\n";
                strCheck += "\t\t\t{\r\n";
                strCheck += "\t\t\t\tAgentTreeUtl.LogWarning(\"argv is null...\");\r\n";
                strCheck += "\t\t\t\treturn true;\r\n";
                strCheck += "\t\t\t}\r\n";

                ParameterInfo[] argvParams = null;
                if(memberInfo.info is MethodInfo)
                    argvParams = (memberInfo.info as MethodInfo).GetParameters();

                if (bObjVar && argvAttrs != null && argvAttrs.Length > 0)
                {
                    string strTmep = "";
                    for (int j = 0; j < argvAttrs.Length; ++j)
                    {
                        string strLabel = argvAttrs[j].DisplayName;
                        if (IsMember(memberInfo))
                        {
                            strLabel = memberInfo.memberName;
                        }
                        if (string.IsNullOrEmpty(strLabel))
                        {
                            bValid = false;
                            break;
                        }
                        if (argvAttrs[j].isDelegateCall)
                        {
                            continue;
                        }

                        if (argvAttrs[j].isExternAttrThis)
                        {
                            if(argvParams!=null && j < argvParams.Length)
                            {
                                if (argvAttrs[j].ArgvType == typeof(VariableUser) || argvAttrs[j].ArgvType == typeof(VariableObject) || argvAttrs[j].ArgvType == typeof(VariableMonoScript))
                                {
                                    strTmep += "(" + strLabel + ".mValue == null || !(" + strLabel + ".mValue is " + argvParams[j].ParameterType.FullName.Replace("+", ".") + "))";
                                    strTmep += " || ";
                                }
                            }
                        }
                        else
                        {
//                             if (argvAttrs[j].ArgvType == typeof(VariableUser) || argvAttrs[j].ArgvType == typeof(VariableObject) || argvAttrs[j].ArgvType == typeof(VariableMonoScript))
//                             {
//                                 strTmep += strLabel + ".mValue == null";
//                                 strTmep += " || ";
//                             }
                        }
                    }
                    if (strTmep.Length > 0)
                    {
                        strTmep = strTmep.Substring(0, strTmep.Length - " || ".Length);
                        strCheck += "\t\t\tif(" + strTmep + ")\r\n";
                        strCheck += "\t\t\t{\r\n";
                        strCheck += "\t\t\t\tAgentTreeUtl.LogWarning(\"argv value is null...\");\r\n";
                        strCheck += "\t\t\t\treturn true;\r\n";
                        strCheck += "\t\t\t}\r\n";
                    }
                }

            }

            string strCatch = "";
            List<StructDelca> vUsedDelcs = new List<StructDelca>();
            string strTemp = "";
            string strTemp1 = "";
            if (IsMember(memberInfo))
            {
                if (IsStatic(memberInfo))
                    strTemp1 += classType.FullName.Replace("+", ".") + "." + memberInfo.info.Name;
                else
                    strTemp1 += "pPointer." + memberInfo.info.Name;
                strTemp = strTemp1 + ";\r\n";
            }
            else
            {
                if (IsStatic(memberInfo))
                    strTemp1 += classType.FullName.Replace("+", ".") + "." + memberInfo.info.Name + "(";
                else
                    strTemp1 += "pPointer." + memberInfo.info.Name + "(";
                if (argvAttrs != null)
                {
                    if (memberInfo.info is MethodInfo)
                    {
                        ParameterInfo[] argvParams = (memberInfo.info as MethodInfo).GetParameters();
                        if (argvParams.Length == argvAttrs.Length)
                        {
                            for (int j = 0; j < argvAttrs.Length; ++j)
                            {
                                if(argvAttrs[j].isDelegateCall)
                                {
                                    if(argvAttrs[j].isDelegateCallValid)
                                    {
                                        string strDelegateCode = "\t\t\t\t";

                                        bool isValidDelegate = true;
                                        for (int c = 0; c < argvAttrs[j].vDelegateArgvs.Count; ++c)
                                        {
                                            string varNew = "";
                                            if (c == 0) varNew = argvParams[j].Name + ".mValue";
                                            else varNew = argvParams[j].Name + ".param" + c;
                                            if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(bool)) varNew += "= new Framework.Core.VariableByte(){boolVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(byte)) varNew += "= new Framework.Core.VariableByte(){byteVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(short)) varNew += "= new Framework.Core.Variable1(){intVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(ushort)) varNew += "= new Framework.Core.Variable1(){shortVal0 = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(int)) varNew += "= new Framework.Core.Variable1(){intVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(uint)) varNew += "= new Framework.Core.Variable1(){uintVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(float)) varNew += "= new Framework.Core.Variable1(){floatVal = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(double)) varNew += "= new Framework.Core.Variable2(){doubleValue = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(long)) varNew += "= new Framework.Core.Variable2(){longValue = " + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (argvAttrs[j].vDelegateArgvs[c].DisplayType == typeof(ulong)) varNew += "= new Framework.Core.Variable2(){longValue = (long)" + argvAttrs[j].vDelegateArgvs[c].DisplayName + "}";
                                            else if (IsInterface(argvAttrs[j].vDelegateArgvs[c].DisplayType, typeof(IUserData))) varNew += "=" + argvAttrs[j].vDelegateArgvs[c].DisplayName;
                                            else
                                            {
                                                Debug.LogError(classType.Name + "::" + memberInfo.info.Name + "代理不支持!!!");
                                                isValidDelegate = false;
                                                break;
                                            }
                                            strDelegateCode += varNew + ";";
                                        }
                                        if(isValidDelegate)
                                        {
                                            strDelegateCode += "\r\n\t\t\t\t" + argvParams[j].Name + ".DoCall();";
                                            strTemp1 += "(";
                                            for (int c = 0; c < argvAttrs[j].vDelegateArgvs.Count; ++c)
                                            {
                                                strTemp1 += argvAttrs[j].vDelegateArgvs[c].DisplayName;
                                                if (c < argvAttrs[j].vDelegateArgvs.Count - 1) strTemp1 += ", ";
                                            }
                                            strTemp1 += ")=>{\r\n" + strDelegateCode + "}";
                                        }
                                        else
                                        {
                                            strTemp1 += "null";
                                        }
                                    }
                                    else
                                    {
                                        strTemp1 += "null";
                                    }
                                }
                                else
                                {
                                    string castLabel = GetVariableToArgvParamLabel(classType, argvParams[j], argvAttrs[j].AlignType);
                                    if (bList(argvParams[j].ParameterType))
                                    {
                                        Type classListObjType = argvParams[j].ParameterType.GenericTypeArguments[0];

                                        StructDelca delc = BuildStaticMemberDelcaByList(vStaticDelca, argvParams[j].ParameterType);
                                        if (!vUsedDelcs.Contains(delc)) vUsedDelcs.Add(delc);
                                        strCatch += "\t\t\t{//\"catch temp\"\r\n";
                                        strCatch += "\t\t\t\tif(" + delc.strName + " == null) " + delc.strName + "= new " + delc.strCode + "();\r\n";
                                        strCatch += "\t\t\t\tfor(int i = 0; i < " + argvParams[j].Name + ".GetList().Count; ++i)\r\n";
                                        strCatch += "\t\t\t\t{\r\n";
                                        if (classListObjType != null)
                                        {
                                            strCatch += "\t\t\t\t\tif(" + argvParams[j].Name + ".mValue[i]!=null)\r\n";
                                            strCatch += "\t\t\t\t\t\t" + delc.strName + ".Add( (" + classListObjType.FullName.Replace("+", ".") + ")" + argvParams[j].Name + ".mValue[i]);\r\n";
                                        }
                                        else
                                            strCatch += "\t\t\t\t\t" + delc.strName + ".Add(" + argvParams[j].Name + ".mValue[i]);\r\n";
                                        strCatch += "\t\t\t\t}\r\n";
                                        strCatch += "\t\t\t}\r\n";

                                        strTemp1 += delc.strName;
                                    }
                                    else if (bMatrix(argvParams[j].ParameterType) && argvAttrs[j].ArgvType == typeof(VariableFloatList))
                                    {
                                        strCatch += "UnityEngine.Matrix4x4 " + argvParams[j].Name + "_temp = UnityEngine.Matrix4x4.identity;\r\n";
                                        strCatch += "\t\t\tif(" + argvParams[j].Name + ".mValue !=null && " + argvParams[j].Name + ".mValue.Count == 16) \r\n";
                                        strCatch += "\t\t\t{\r\n";
                                        strCatch += "\t\t\t\tfor(int i =0; i < 16; ++i) " + argvParams[j].Name + "_temp[i] = " + argvParams[j].Name + ".mValue[i];\r\n";
                                        strCatch += "\t\t\t}\r\n";
                                        strTemp1 += argvParams[j].Name + "_temp";
                                    }
                                    else
                                    {
                                        if (argvAttrs[j].AlignType == typeof(System.Type))
                                        {
                                            strTemp1 += castLabel;
                                            if (argvAttrs[j].ArgvType != typeof(VariableInt))
                                            {
                                                bValid = false;
                                                Debug.LogError(classType.Name + "::" + memberInfo.info.Name + "导出函数参数不支持!!!");
                                            }
                                            else
                                                strTemp1 += "AgentTree_ClassTypes.HashToClassType(" + argvParams[j].Name + ".mValue" + ")";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(castLabel) && argvAttrs[j].ArgvType == typeof(VariableObject) || argvAttrs[j].ArgvType == typeof(VariableMonoScript))
                                            {
                                                strTemp1 += argvParams[j].Name + ".ToObject<" + castLabel.Replace("(", "").Replace(")", "") + ">()";
                                            }
                                            else
                                            {
                                                strTemp1 += castLabel;
                                                strTemp1 += argvParams[j].Name + ".mValue";
                                            }
                                        }
                                    }
                                }
                                
                                if (j < argvAttrs.Length - 1) strTemp1 += ",";
                            }
                        }
                        else
                        {
                            bValid = false;
                            Debug.LogError(classType.Name + "::" + memberInfo.info.Name + "导出函数参数不支持!!!");
                        }
                    }
                }
                strTemp = strTemp1 + ");\r\n";
            }


            if (bValid)
            {
                strCode += strCheck;
                strCode += strCatch;
                Type returnType = GetReturnType(memberInfo);
                if (returnType != null && returnAttrs != null && returnAttrs.Length > 0)
                {
                    strCode += "\t\t\tif(pReturn != null)\r\n";
                    strCode += "\t\t\t{\t\n";
                    if (bList(returnType))
                    {
                        StructDelca delc = BuildStaticMemberDelcaByList(vStaticDelca, returnType, 1);
                        if (!vUsedDelcs.Contains(delc)) vUsedDelcs.Add(delc);
                        if (IsMember(memberInfo) && strLabelFuncName == "set")
                        {
                            strCode += "\t\t\t\tif(pReturn.mValue!=null)\r\n";
                            strCode += "\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\tfor(int i=0;i <pReturn.mValue.Count; ++i)\r\n";
                            strCode += "\t\t\t\t\tpReturn.mValue.Add(pReturn.mValue[i]);\r\n";
                            strCode += "\t\t\t\t}\r\n";
                            strCode += "\t\t\t\telse if(" + strTemp1 + "!=null) " + strTemp1 + ".Clear();\r\n";

                            strCode += "\t\t\t\t\t" + strTemp1 + "= pReturn.mValue;\r\n";
                            strCode += "\t\t\t\t" + strTemp1 + "= null;\r\n";
                        }
                        else
                        {
                            strCode += "\t\t\t\t" + delc.strCode + "vReturnTemp =" + strTemp;
                            strCode += "\t\t\t\tif(vReturnTemp!=null)\r\n";
                            strCode += "\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\tfor(int i=0;i <vReturnTemp.Count; ++i)\r\n";
                            strCode += "\t\t\t\t\tpReturn.mValue.Add(vReturnTemp[i]);\r\n";
                            strCode += "\t\t\t\t}\r\n";
                        }
                    }
                    else if (bArray(returnType))
                    {
                        StructDelca delc = BuildStaticMemberDelcaByList(vStaticDelca, returnType, 1);
                        if (!vUsedDelcs.Contains(delc)) vUsedDelcs.Add(delc);
                        if (IsMember(memberInfo) && strLabelFuncName == "set")
                        {
                            strCode += "\t\t\t\tif(pReturn.mValue!=null)\r\n";
                            strCode += "\t\t\t\t\t" + strTemp1 + "= pReturn.mValue.ToArray();\r\n";
                            strCode += "\t\t\t\t" + strTemp1 + "= null;\r\n";
                        }
                        else
                        {
                            strCode += "\t\t\t\t" + delc.strCodeArray + "vReturnTemp =" + strTemp;
                            strCode += "\t\t\t\tif(vReturnTemp!=null)\r\n";
                            strCode += "\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\tfor(int i=0;i <vReturnTemp.Length; ++i)\r\n";
                            strCode += "\t\t\t\t\tpReturn.mValue.Add(vReturnTemp[i]);\r\n";
                            strCode += "\t\t\t\t}\r\n";
                        }

                    }
                    else if(bMatrix(returnType) && returnAttrs[0].ReturnType == typeof(VariableFloatList))
                    {
                        if (IsMember(memberInfo) && strLabelFuncName == "set")
                        {
                            strCode += "\t\t\t\tif(pReturn!=null && pReturn.mValue !=null && pReturn.mValue.Count == 16) \r\n";
                            strCode += "\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\tfor(int i =0; i < 16; ++i) matrix[i] = pReturn.mValue[i];\r\n";
                            strCode += "\t\t\t\t}\r\n";
                        }
                        else
                        {
                            strCode += "\t\t\t\tUnityEngine.Matrix4x4 matrix = " + GetArgvParamToVariableLabel(returnType) + strTemp;
                            strCode += "\t\t\t\tif(pReturn!=null) \r\n";
                            strCode += "\t\t\t\t{\r\n";
                            strCode += "\t\t\t\t\tif(pReturn.mValue == null)pReturn.mValue = new System.Collections.Generic.List<float>(16);\r\n";
                            strCode += "\t\t\t\t\telse pReturn.mValue.Clear();\r\n";
                            strCode += "\t\t\t\t\tfor(int i =0; i < 16; ++i) pReturn.mValue.Add(matrix[i]);\r\n";
                            strCode += "\t\t\t\t}\r\n";
                        }
                    }
                    else
                    {
                        if(IsMember(memberInfo) && strLabelFuncName == "set")
                        {
                            string label = GetVariableToArgvParamLabel(returnType);
                            strCode += "\t\t\t\t" + strTemp1 + " = " + label + "pReturn.mValue;\r\n";
                        }
                        else
                            strCode += "\t\t\t\tpReturn.mValue = " + GetArgvParamToVariableLabel(returnType) + strTemp;
                    }
                    strCode += "\t\t\t}\t\n";
               //     if (!IsMember(memberInfo))
               //         strCode += "\t\t\telse " + strTemp;
                }
                else
                {
                    if (!IsMember(memberInfo))
                        strCode += "\t\t\t" + strTemp;
                    else if (returnType != null && strLabelFuncName == "set" && argvAttrs.Length==1)
                    {
                        string label = GetVariableToArgvParamLabel(returnType);
                        strCode += "\t\t\t" + strTemp1 + " = " + label + memberInfo.memberName + ".mValue;\r\n";
                    }
                }

                for (int j = 0; j < vUsedDelcs.Count; ++j)
                {
                    strCode += "\t\t\tif(" + vUsedDelcs[j].strName + "!=null) " + vUsedDelcs[j].strName + ".Clear();\r\n";
                }

                if (string.IsNullOrEmpty(strClassMapping))
                {
                    strSwitchDo += "\t\t\t\tcase " + funcID + ":\r\n";
                    strSwitchDo += "\t\t\t\t{//" + classType.FullName.Replace("+", ".") + "->" + memberInfo.memberName + "\r\n";
                    if (!IsStatic(memberInfo))
                    {
                        strSwitchDo += "\t\t\t\t\tif(pUserClass == null) return true;\r\n";
                        if (exportAttr.bGlobalModule == EGlobalType.Single &&
                            (IsInterface(classType, typeof(IUserData)) || classType.IsSubclassOf(typeof(MonoBehaviour))))
                        {
                            strSwitchDo += "\t\t\t\t\tif(pUserClass.mValue == null)\r\n";
                            if(classType.IsSubclassOf(typeof(Behaviour)))
                                strSwitchDo += "\t\t\t\t\t\tpUserClass.mValue = " + classType.FullName.Replace("+", ".") + ".getInstance() as UnityEngine.Behaviour;\r\n";
                            else
                                strSwitchDo += "\t\t\t\t\t\tpUserClass.mValue = " + classType.FullName.Replace("+", ".") + ".getInstance();\r\n";
                        }
                        else if (exportAttr.bGlobalModule == EGlobalType.AlignLamda && !string.IsNullOrEmpty(exportAttr.strLamda) )
                        {
                            strSwitchDo += "\t\t\t\t\tif(pUserClass.mValue == null)\r\n";
                            strSwitchDo += "\t\t\t\t\t\tpUserClass.mValue = " + exportAttr.strLamda + ";\r\n";
                        }
                        if(defaultPointerAttr!=null && !string.IsNullOrEmpty(defaultPointerAttr.strLamda) )
                        {
                            strSwitchDo += "\t\t\t\t\tif(pUserClass.mValue == null)\r\n";
                            strSwitchDo += "\t\t\t\t\t\tpUserClass.mValue = " + defaultPointerAttr.strLamda + ";\r\n";
                        }
                        if (!bMono)
                        {
                            strSwitchDo += "\t\t\t\t\tif(pUserClass.mValue == null)\r\n";
                            strSwitchDo += "\t\t\t\t\t\tpUserClass.mValue = AgentTreeManager.getInstance().FindUserClass(pUserClass.hashCode, pTask.pAT);\r\n";
                        }
                        strSwitchDo += "\t\t\t\t\tif(pUserClass.mValue == null) return true;\r\n";
                    }

                    if (argvAttrs != null && argvAttrs.Length > 0)
                    {
                        strSwitchDo += "\t\t\t\t\tif(pAction.inArgvs.Length <=" + argvAttrs.Length + ") return true;\r\n";
                    }
                    if (returnAttrs != null && returnAttrs.Length > 0)
                    {
                        strSwitchDo += "\t\t\t\t\tif(pAction.outArgvs ==null || pAction.outArgvs.Length <" + returnAttrs.Length + ") return true;\r\n";
                    }
                    if (IsStatic(memberInfo))
                    {
                        strSwitchDo += "\t\t\t\t\treturn " + strClassName + "." + strFunc + "(";
                        string strArgv = "";
                        if (argvAttrs != null)
                        {
                            for (int j = 0; j < argvAttrs.Length; ++j)
                            {
                                strArgv += "pAction.GetInVariableByIndex<" + argvAttrs[j].ArgvType.FullName + ">(" + (j + 1) + ", pTask),";
                            }
                        }

                        if (returnAttrs != null && returnAttrs.Length > 0)
                        {
                            for (int j = 0; j < returnAttrs.Length; ++j)
                            {
                                strArgv += "pAction.GetOutVariableByIndex<" + returnAttrs[j].ReturnType.FullName + ">(" + j + ", pTask),";
                            }
                        }
                        if (strArgv.Length > 0)
                        {
                            strArgv = strArgv.Substring(0, strArgv.Length - 1);
                            strSwitchDo += strArgv;
                        }
                    }
                    else
                    {
                        if (classType.IsValueType)
                            strSwitchDo += "\t\t\t\t\treturn " + strClassName + "." + strFunc + "(" + "(" + classType.FullName.Replace("+", ".") + ")" + "pUserClass.mValue";
                        else
                        {
                            strSwitchDo += "\t\t\t\t\tif(!(pUserClass.mValue is " + classType.FullName.Replace("+", ".") + ")) return true;\r\n";
                            strSwitchDo += "\t\t\t\t\treturn " + strClassName + "." + strFunc + "(" + "pUserClass.mValue as " + classType.FullName.Replace("+", ".");
                        }
                        if (argvAttrs != null)
                        {
                            for (int j = 0; j < argvAttrs.Length; ++j)
                            {
                                strSwitchDo += ", pAction.GetInVariableByIndex<" + argvAttrs[j].ArgvType.FullName + ">(" + (j + 1) + ", pTask)";
                            }
                        }

                        if (returnAttrs != null && returnAttrs.Length > 0)
                        {
                            for (int j = 0; j < returnAttrs.Length; ++j)
                            {
                                strSwitchDo += ", pAction.GetOutVariableByIndex<" + returnAttrs[j].ReturnType.FullName + ">(" + j + ", pTask)";
                            }
                        }
                    }
                    strSwitchDo += ");\r\n";
                    strSwitchDo += "\t\t\t\t}\r\n";
                }
            }


            strCode += "\t\t\treturn true;\r\n";
            strCode += "\t\t}\r\n";
        }
        //------------------------------------------------------
        static bool CanExport(System.Type type)
        {
            if (type.IsDefined(typeof(DisiableExportAttribute), false))
                return false;
            return true;
        }
        //------------------------------------------------------
        static ATExportMonoAttribute GetExportAttr(System.Type type)
        {
            ATExportMonoAttribute exportAttr = (ATExportMonoAttribute)type.GetCustomAttribute(typeof(ATExportMonoAttribute));
            if (exportAttr == null) exportAttr = new ATExportMonoAttribute((ATExportAttribute)type.GetCustomAttribute(typeof(ATExportAttribute)));
            return exportAttr;
        }
        //------------------------------------------------------
        static int BuildHashCode(System.Type type)
        {
            int classId = AgentTreeUtl.TypeToHash(type);
//             ATExportGUIDAttribute exportGuid = (ATExportGUIDAttribute)typeof(AT.IUserData).GetCustomAttribute(typeof(ATExportGUIDAttribute));
//             if (exportGuid != null && exportGuid.hashCode != 0) classId = exportGuid.hashCode;
            return classId;
        }
        //------------------------------------------------------
        static void BuildMonoClass(Dictionary<System.Type, List<ExportMethodInfo>> vExportMonos, List<System.Type> vUserClass)
        {
            float curProcess = 0;
            EditorUtility.DisplayProgressBar("导出AT节点", "", 0);
            string strMappingCode = "";
            string strClassTypeToHashCode = "";
            string strHashCodeToClassType = "";
            string strParentHashCodeByHashCode = "";

            int userDataHashCode = BuildHashCode(typeof(IUserData));

            foreach (var db in vExportMonos)
            {
                curProcess++;
                if (vExportMonos.Count>0)  EditorUtility.DisplayProgressBar("导出AT节点", db.Key.ToString(), curProcess/(float)vExportMonos.Count);
                ATExportMonoAttribute exportAttr = GetExportAttr(db.Key);
                
                string exportDisplay = exportAttr.DisplayName;
                if (string.IsNullOrEmpty(exportDisplay)) exportDisplay = db.Key.Name;
                string strClassName = "AgentTree_" + db.Key.Name;
                string strClassMapping = "";
                string strCode = "//auto genreator\r\n";
                if (!string.IsNullOrEmpty(exportAttr.marcoDefine))
                {
                    strCode += "#if " + exportAttr.marcoDefine + "\r\n";
                }
                strCode += "namespace Framework.Plugin.AT\r\n";
                strCode += "{\r\n";
                strCode += "#if UNITY_EDITOR\r\n";
                strCode += "\t[ATExport(\""+ exportDisplay + "\",true)]\r\n";
                strCode += "#endif\r\n";
                strCode += "\tpublic static class " + strClassName + "\r\n";
                strCode += "\t{\r\n";

                bool bMono = false;
                bool bUserClass = false;
                CheckClassOrMono(db.Key, ref bMono, ref bUserClass);

                
                string strSwitchDo = "";
                System.Collections.Generic.Dictionary<string, List<StructDelca>> vStaticDelca = new System.Collections.Generic.Dictionary<string, List<StructDelca>>();
                for(int i =0; i < db.Value.Count; ++i)
                {
                    if(db.Value[i].info is MethodInfo)
                        BuildMemberCode(exportAttr, db.Key, db.Value[i], ref vStaticDelca, ref strClassName, ref strCode, ref strClassMapping, ref strSwitchDo);
                    else if(db.Value[i].info is FieldInfo)
                    {
                        ATFieldAttribute atField = db.Value[i].GetCustomAttribute<ATFieldAttribute>();
                        if(atField.CanGet) BuildMemberCode(exportAttr, db.Key, db.Value[i], ref vStaticDelca, ref strClassName, ref strCode, ref strClassMapping, ref strSwitchDo, "get");
                        if(atField.CanSet) BuildMemberCode(exportAttr, db.Key, db.Value[i], ref vStaticDelca, ref strClassName, ref strCode, ref strClassMapping, ref strSwitchDo, "set");
                    }
                    else if (db.Value[i].info is PropertyInfo)
                    {
                        PropertyInfo prop = db.Value[i].info as PropertyInfo;
                        ATFieldAttribute atField = db.Value[i].GetCustomAttribute<ATFieldAttribute>();
                        if (prop.CanRead && atField.CanGet) BuildMemberCode(exportAttr, db.Key, db.Value[i], ref vStaticDelca, ref strClassName, ref strCode, ref strClassMapping, ref strSwitchDo, "get");
                        if (prop.CanWrite && atField.CanSet) BuildMemberCode(exportAttr, db.Key, db.Value[i], ref vStaticDelca, ref strClassName, ref strCode, ref strClassMapping, ref strSwitchDo, "set");
                    }
                }

                if (!string.IsNullOrEmpty(strSwitchDo))
                {
                    if (!string.IsNullOrEmpty(exportAttr.marcoDefine))
                    {
                        strClassMapping += "\t\t\t\t#if " + exportAttr.marcoDefine + "\r\n";
                    }
                    strClassMapping += "\t\t\t\tcase " + BuildHashCode(db.Key) + ":\r\n";
                    strClassMapping += "\t\t\t\t{//" + db.Key.FullName.Replace("+", ".") + "\r\n";
                    if (bMono)
                    {
                        strClassMapping += "\t\t\t\t\treturn " + strClassName + ".DoAction(pUserPointer as VariableMonoScript, pTask, pAction, functionId)" + ";\r\n";
                    }
                    else
                        strClassMapping += "\t\t\t\t\treturn " + strClassName + ".DoAction(pUserPointer as VariableUser, pTask, pAction, functionId)" + ";\r\n";
                    strClassMapping += "\t\t\t\t}\r\n";

                    if (!string.IsNullOrEmpty(exportAttr.marcoDefine))
                    {
                        strClassMapping += "\t\t\t\t#endif\r\n";
                    }
                    strMappingCode += strClassMapping;
                }
                foreach (var dc in vStaticDelca)
                {
                    for (int i = 0; i < dc.Value.Count; ++i)
                    {
                        strCode += "\t\tprivate static " + dc.Value[i].strCode + " " + dc.Value[i].strName + ";\r\n";
                    }
                }
                if (bMono)
                    strCode += "\t\tpublic static bool DoAction(VariableMonoScript pUserClass, AgentTreeTask pTask, ActionNode pAction, int functionId = 0)\r\n";
                else
                    strCode += "\t\tpublic static bool DoAction(VariableUser pUserClass, AgentTreeTask pTask, ActionNode pAction, int functionId = 0)\r\n";
                strCode += "\t\t{\r\n";
                strCode += "\t\t\tint funcId = (functionId!=0)?functionId:(int)pAction.GetExcudeHash();\r\n";
                strCode += "\t\t\tswitch(funcId)\r\n";
                strCode += "\t\t\t{\r\n";
                strCode +=  strSwitchDo;
                strCode += "\t\t\t}\r\n";

                if(db.Key.BaseType !=null && vExportMonos.ContainsKey(db.Key.BaseType))
                {
                    strCode += "\t\t\treturn AgentTree_" + db.Key.BaseType.Name + ".DoAction(pUserClass,pTask,pAction,functionId);\r\n";
                }
                else 
                    strCode += "\t\t\treturn true;\r\n";
                strCode += "\t\t}\r\n";

                strCode += "\t}\r\n";
                strCode += "}\r\n";

                if (!string.IsNullOrEmpty(exportAttr.marcoDefine))
                {
                    strCode += "#endif\r\n";
                }
                string dir = AgentTreeEditorPath.BuildGeneratorRuntimePath();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string path = Path.Combine(dir, strClassName + ".cs").Replace("\\", "/");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                writer.Write(strCode);
                writer.Close();
            }

            strClassTypeToHashCode += "\t\t\t\tms_vClassTypeToHash[typeof(" + typeof(IUserData).FullName.Replace("+", ".") + ")]= " + userDataHashCode + ";\r\n";
            //  strHashCodeToClassType += "\t\t\tif( hashCode == " + userDataHashCode + ") return typeof(" + typeof(IUserData).FullName.Replace("+", ".") + ");\r\n";
            strHashCodeToClassType += "\t\t\t\tcase " + userDataHashCode + ": return " + "typeof(" + typeof(IUserData).FullName.Replace("+", ".") + ");\r\n";

            for(int i =0; i < vConvertBehaviourInterfaceTypes.Count; ++i)
            {
                if (!CanExport(vConvertBehaviourInterfaceTypes[i]))
                    continue;
                int hasCode = BuildHashCode(vConvertBehaviourInterfaceTypes[i]);

                strClassTypeToHashCode += "\t\t\t\tms_vClassTypeToHash[typeof(" + vConvertBehaviourInterfaceTypes[i].FullName.Replace("+", ".") + ")]= " + hasCode + ";\r\n";
                strHashCodeToClassType += "\t\t\t\tcase " + hasCode + ": return " + "typeof(" + vConvertBehaviourInterfaceTypes[i].FullName.Replace("+", ".") + ");\r\n";
            }
            for (int i = 0; i < vUserClass.Count; ++i)
            {
                System.Type parentType = GetParentClassTypeHashCode(vUserClass[i], vExportMonos);
                if (parentType != null)
                {
                    if (!CanExport(parentType))
                        continue;

                    var attri = GetExportAttr(vUserClass[i]);
                    if(attri!=null && !string.IsNullOrEmpty(attri.marcoDefine))
                    {
                        strClassTypeToHashCode += "\t\t\t\t#if " + attri.marcoDefine + "\r\n";
                        strHashCodeToClassType += "\t\t\t\t#if " + attri.marcoDefine + "\r\n";
                        strParentHashCodeByHashCode += "\t\t\t\t#if " + attri.marcoDefine + "\r\n";
                    }

                    strClassTypeToHashCode += "\t\t\t\tms_vClassTypeToHash[typeof(" + vUserClass[i].FullName.Replace("+", ".") + ")]= " + BuildHashCode(vUserClass[i]) + ";\r\n";
                    //    strHashCodeToClassType += "\t\t\tif( hashCode == " + BuildHashCode(db.Key) + ") return typeof(" + db.Key.FullName.Replace("+", ".") + ");\r\n";
                    strHashCodeToClassType += "\t\t\t\tcase " + BuildHashCode(vUserClass[i]) + ": return " + "typeof(" + vUserClass[i].FullName.Replace("+", ".") + ");\r\n";

                    strParentHashCodeByHashCode += "\t\t\t\tcase " + BuildHashCode(vUserClass[i]) + ": return " + BuildHashCode(parentType) + ";//" + vUserClass[i].FullName.Replace("+", ".") + "->" + parentType.FullName.Replace("+", ".") + "\r\n";
                    if (attri != null && !string.IsNullOrEmpty(attri.marcoDefine))
                    {
                        strClassTypeToHashCode += "\t\t\t\t#endif\r\n";
                        strHashCodeToClassType += "\t\t\t\t#endif\r\n";
                        strParentHashCodeByHashCode += "\t\t\t\t#endif\r\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(strMappingCode))
            {
                {
                    string dir = AgentTreeEditorPath.BuildGeneratorRuntimePath();
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string path = Path.Combine(dir, "AgentTreeUserClass_Func.cs").Replace("\\","/");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    string strCode = "//auto generator\r\n";
                    strCode += "using System;\r\n";
                    strCode += "namespace Framework.Plugin.AT\r\n";
                    strCode += "{\r\n";
                    strCode += "\tpublic static class AgentTreeUserClass_Func\r\n";
                    strCode += "\t{\r\n";
                    strCode += "\t\tpublic static bool DoInerAction(AgentTreeTask pTask, ActionNode pAction, int functionId = 0)\r\n";
                    strCode += "\t\t{\r\n";
                    strCode += "\t\t\tif(pAction.inArgvs == null || pAction.inArgvs.Length < 1) return true;\r\n";
                    strCode += "\t\t\tVariable pUserPointer = pAction.GetInVariableByIndex<Variable>(0, pTask);\r\n";
                    strCode += "\t\t\tVariable pOriUserPointer = pAction.GetInVariable(0); // get ori port\r\n";
                    strCode += "\t\t\tint classHashCode= 0;\r\n";
                    strCode += "\t\t\tif (pOriUserPointer != null) classHashCode = pOriUserPointer.GetClassHashCode();\r\n";
                    strCode += "\t\t\telse if(pUserPointer != null ) classHashCode = pUserPointer.GetClassHashCode();\r\n";
                    strCode += "\t\t\tif(classHashCode == 0 )\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += "\t\t\t\tVariableInt pUserHash = pAction.GetInVariableByIndex<VariableInt>(0, pTask);\r\n";
                    strCode += "\t\t\t\tif(pUserHash!=null)classHashCode = pUserHash.mValue;\r\n";
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\tif(classHashCode == 0) return true;\r\n";
                    strCode += "\t\t\tswitch(classHashCode)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += strMappingCode;
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\treturn true;\r\n";
                    strCode += "\t\t}\r\n";
                    strCode += "\t}\r\n";
                    strCode += "}\r\n";

                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                    StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    writer.Write(strCode);
                    writer.Close();
                }

                //! class hash mapping
                {
                    string dir = AgentTreeEditorPath.BuildGeneratorRuntimePath();
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string path = Path.Combine(dir, "../AgentTree_ClassTypes.cs").Replace("\\", "/");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    string strCode = "//auto generator\r\n";
                    strCode += "using System;\r\n";
                    strCode += "using System.Collections.Generic;\r\n";
                    strCode += "namespace Framework.Plugin.AT\r\n";
                    strCode += "{\r\n";
                    strCode += "\tpublic static class AgentTree_ClassTypes\r\n";
                    strCode += "\t{\r\n";
                    strCode += "\t\tstatic Dictionary<System.Type, int> ms_vClassTypeToHash = new Dictionary<System.Type, int>();\r\n";
                    strCode += "\t\tpublic static int ClassTypeToHash(System.Type classType)\r\n";
                    strCode += "\t\t{\r\n";
                    strCode += "\t\t\t#region MappingHash\r\n";
                    strCode += "\t\t\tif(ms_vClassTypeToHash.Count <=0)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += strClassTypeToHashCode;
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\t#endregion\r\n";
                    strCode += "\t\t\tint hash = 0;\r\n";
                    strCode += "\t\t\tif(ms_vClassTypeToHash.TryGetValue(classType, out hash)) return hash;\r\n";
                    strCode += "\t\t\treturn 0;\r\n";
                    strCode += "\t\t}\r\n";

                    strCode += "\t\tpublic static System.Type HashToClassType(int hashCode)\r\n";
                    strCode += "\t\t{\r\n";
                    strCode += "\t\t\tif(hashCode == 0) return null;\r\n";
                    strCode += "\t\t\tswitch(hashCode)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += strHashCodeToClassType;
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\treturn null;\r\n";
                    strCode += "\t\t}\r\n";

                    strCode += "\t\tpublic static int HashToParentHash(int hashCode)\r\n";
                    strCode += "\t\t{\r\n";
                    strCode += "\t\t\tif(hashCode == 0) return 0;\r\n";
                    strCode += "\t\t\tswitch(hashCode)\r\n";
                    strCode += "\t\t\t{\r\n";
                    strCode += strParentHashCodeByHashCode;
                    strCode += "\t\t\t}\r\n";
                    strCode += "\t\t\treturn 0;\r\n";
                    strCode += "\t\t}\r\n";

                    strCode += "\t}\r\n";
                    strCode += "}\r\n";

                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                    StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    writer.Write(strCode);
                    writer.Close();
                }
            }
            EditorUtility.ClearProgressBar();
        }
        //------------------------------------------------------
        static System.Type GetParentClassTypeHashCode(System.Type type, Dictionary<Type, List<ExportMethodInfo>> vExportMonos)
        {
            if (type == null) return null;
            System.Type baseType = type.BaseType;
            while(baseType!=null)
            {
                if(vExportMonos.ContainsKey(baseType))
                {
                    return baseType;
                }
                baseType = baseType.BaseType;
            }
            return null;
        }
        //------------------------------------------------------
        public static void BuildUserMapping(Dictionary<int, string> vMapping)
        {

        }
        //------------------------------------------------------
        static Type GetReturnType(ExportMethodInfo member)
        {
            if (member.info is MethodInfo)
            {
                return ((member.info as MethodInfo).ReturnParameter!=null)?(member.info as MethodInfo).ReturnParameter.ParameterType:null;
            }
            else if (member.info is FieldInfo)
            {
                return (member.info as FieldInfo).FieldType;
            }
            else if (member.info is PropertyInfo)
            {
                return (member.info as PropertyInfo).PropertyType;
            }
            return null;
        }
        //------------------------------------------------------
        static bool IsStatic(ExportMethodInfo member)
        {
            if(member.info is MethodInfo)
            {
                return (member.info as MethodInfo).IsStatic;
            }
            else if (member.info is FieldInfo)
            {
                return (member.info as FieldInfo).IsStatic;
            }
            else if (member.info is PropertyInfo)
            {
               var method = (member.info as PropertyInfo).GetGetMethod();
                return method.IsStatic;
            }
            return false;
        }
        //------------------------------------------------------
        static bool IsDelegate(Type type)
        {
            if (type == null) return false;
            Type tp = type;
            while(type!=null)
            {
                if(type == typeof(System.Delegate))
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
        //------------------------------------------------------
        static bool IsInterface(Type type, Type interfaceType)
        {
            if (type == null || interfaceType == null) return false;
            Type[] inter = type.GetInterfaces();
            for(int i = 0; i < inter.Length; ++i)
            {
                if (inter[i] == interfaceType) return true;
            }
            return false;
        }
        //------------------------------------------------------
        static bool IsConvertBehaviour(Type type)
        {
            if (type == null) return false;
            if (type.IsDefined(typeof(ATConvertBehaviourAttribute))) return true;
            return false;
        }
        //------------------------------------------------------
        static bool IsMember(ExportMethodInfo member)
        {
            if (member.info is MethodInfo)
            {
                return false;
            }
            else if (member.info is FieldInfo)
            {
                return true;
            }
            else if (member.info is PropertyInfo)
            {
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        static bool IsUnityObject(Type baseType)
        {
            if (baseType.IsSubclassOf(typeof(UnityEngine.Object))) return true;
            System.Type temp = baseType;
            while (temp != null)
            {
                if (temp.IsSubclassOf(typeof(UnityEngine.Object))) return true;
                temp = temp.BaseType;
            }
            return false;
        }
        //------------------------------------------------------
        static void CheckClassOrMono(Type baseType, ref bool bMono, ref bool bUserClass)
        {
            bMono = false;
            bUserClass = false;
            if (baseType.IsDefined(typeof(ATConvertBehaviourAttribute), true))
            {
                bMono = true;
                return;
            }
            Type temp = baseType;
            while (baseType != null)
            {
                if (baseType.IsSubclassOf(typeof(Behaviour))) bMono = true;
                baseType = baseType.BaseType;
                if (bMono) break;
            }
            Type[] types = temp.GetInterfaces();
            if (types == null) return;
            for(int i = 0; i < types.Length; ++i)
            {
                if (types[i] == typeof(IUserData)) bUserClass = true;
                if (bUserClass) break;
            }
        }
        //------------------------------------------------------
        static ATMethodArgvAttribute[] BuildMonoArgvAttr(Type classType, ExportMethodInfo pMethod, string strLabel)
        {
            List<ATMethodArgvAttribute> vargs = new List<ATMethodArgvAttribute>();
            ATExportMonoAttribute exportAttr = (ATExportMonoAttribute)classType.GetCustomAttribute(typeof(ATExportMonoAttribute));
            if (exportAttr == null) return vargs.ToArray();

            ParameterInfo[] parmas = null;
            if(pMethod.info is MethodInfo)
            {
                parmas = (pMethod.info as MethodInfo).GetParameters();
                if (parmas.Length <= 0)
                {
                    return null;
                }
            }
            if (parmas!=null)
            {
                ATMethodAttribute methodAttr = (ATMethodAttribute)pMethod.GetCustomAttribute<ATMethodAttribute>();
                for (int i = 0; i < parmas.Length; ++i)
                {
                    string param = parmas[i].Name;
                    ATMethodArgvAttribute agv = null;
                    Type paramClassType = null;
                    Type varType = GetVariableType(parmas[i].ParameterType, ref paramClassType);

                    Type displayType = null;
                    if (methodAttr.ArgvDisplays != null && i < methodAttr.ArgvDisplays.Length)
                    {
                        displayType = methodAttr.ArgvDisplays[i];
                    }
                    if (displayType ==null && parmas[i].ParameterType.IsEnum)
                        displayType = parmas[i].ParameterType;

                    if (varType != null)
                    {
                        if (paramClassType == null)
                        {
                            if (parmas[i].ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
                                paramClassType = parmas[i].ParameterType;
                        }
                        agv = new ATMethodArgvAttribute(varType, param, false, paramClassType, displayType, "", false, -1, true);
                        agv.defaultValue = parmas[i].DefaultValue;
                        if (i == 0)
                            agv.isExternAttrThis = pMethod.info.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute),false);

                        if(IsDelegate(parmas[i].ParameterType))
                        {
                            agv.isDelegateCall = true;
                            agv.isDelegateCallValid = true;
                            agv.vDelegateArgvs = null;
                            agv.CheckDelegateArgvs(parmas[i]);
                        }

                        vargs.Add(agv);
                    }
                }
            }
            if (pMethod.info is FieldInfo && strLabel.CompareTo("set") == 0)
            {
                FieldInfo field = pMethod.info as FieldInfo;

                string param = field.Name;
                ATMethodArgvAttribute agv = null;
                Type paramClassType = null;
                Type varType = GetVariableType(field.FieldType, ref paramClassType);
                if (field.FieldType.IsGenericType || field.FieldType.IsArray)
                {
                    if (field.FieldType.IsArray)
                    {

                    }
                }
                if (varType != null)
                {
                    ATFieldAttribute methodAttr = (ATFieldAttribute)pMethod.GetCustomAttribute<ATFieldAttribute>();
                    Type displayType = methodAttr.displayType;
                    if(!string.IsNullOrEmpty(methodAttr.DisplayName))param = methodAttr.DisplayName;
                    if (paramClassType == null)
                    {
                        if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                            paramClassType = field.FieldType;
                    }
                    agv = new ATMethodArgvAttribute(varType, param, false, paramClassType, displayType, "", false, -1, true);
                    vargs.Add(agv);
                }
            }
            else if (pMethod.info is PropertyInfo && strLabel.CompareTo("set") == 0)
            {
                PropertyInfo field = pMethod.info as PropertyInfo;
                if ( field.CanWrite)
                {
                    string param = field.Name;
                    ATMethodArgvAttribute agv = null;
                    Type paramClassType = null;
                    Type varType = GetVariableType(field.PropertyType, ref paramClassType);
                    if (field.PropertyType.IsGenericType || field.PropertyType.IsArray)
                    {
                        if (field.PropertyType.IsArray)
                        {

                        }
                    }
                    if (varType != null)
                    {
                        ATFieldAttribute methodAttr = (ATFieldAttribute)pMethod.GetCustomAttribute<ATFieldAttribute>();
                        Type displayType = methodAttr.displayType;
                        if (!string.IsNullOrEmpty(methodAttr.DisplayName)) param = methodAttr.DisplayName;
                        else param = pMethod.memberName;
                        if (paramClassType == null)
                        {
                            if (field.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
                                paramClassType = field.PropertyType;
                        }
                        if (displayType == null && field.PropertyType.IsEnum)
                            displayType = field.PropertyType;

                        bool bReturn = false;
                        if (field.CanRead) bReturn = true;
                        agv = new ATMethodArgvAttribute(varType, param, false, paramClassType, displayType, "", bReturn, -1, true);
                        vargs.Add(agv);
                    }
                }
            }
            return vargs.ToArray();
        }
        //------------------------------------------------------
        static ATMethodReturnAttribute[] BuildMonoReturnAttr(Type classType, ExportMethodInfo pMethod, string strLabel)
        {
            List<ATMethodReturnAttribute> vargs = new List<ATMethodReturnAttribute>();
            ATExportMonoAttribute exportAttr = (ATExportMonoAttribute)classType.GetCustomAttribute<ATExportMonoAttribute>();
            if (exportAttr == null) return vargs.ToArray();

            if (pMethod.info is MethodInfo)
            {
                ParameterInfo parmas = ((MethodInfo)pMethod.info).ReturnParameter;
                if (parmas == null)
                {
                    return null;
                }
                string param = parmas.Name;
                ATMethodReturnAttribute agv = null;
                Type paramClassType = null;
                Type varType = GetVariableType(parmas.ParameterType, ref paramClassType);
                if (parmas.ParameterType.IsGenericType || parmas.ParameterType.IsArray)
                {
                    if (parmas.ParameterType.IsArray)
                    {

                    }
                }
                if (varType != null)
                {
                    ATMethodAttribute methodAttr = (ATMethodAttribute)pMethod.GetCustomAttribute<ATMethodAttribute>();
                    Type displayType = null;
                    if (methodAttr.ReturnDisplays != null)
                    {
                        displayType = methodAttr.ReturnDisplays;
                    }
                    if (displayType == null && parmas.ParameterType.IsEnum)
                        displayType = parmas.ParameterType;
                    if (paramClassType == null)
                    {
                        if (parmas.ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
                            paramClassType = parmas.ParameterType;
                    }
                    agv = new ATMethodReturnAttribute(varType, "pReturn", paramClassType, displayType, "", -1, true);
                    vargs.Add(agv);
                }
            }
            else if(pMethod.info is FieldInfo && strLabel.CompareTo("get") == 0)
            {
                FieldInfo field = pMethod.info as FieldInfo;

                string param = field.Name;
                ATMethodReturnAttribute agv = null;
                Type paramClassType = null;
                Type varType = GetVariableType(field.FieldType, ref paramClassType);
                if (field.FieldType.IsGenericType || field.FieldType.IsArray)
                {
                    if (field.FieldType.IsArray)
                    {

                    }
                }
                if (varType != null)
                {
                    ATFieldAttribute methodAttr = (ATFieldAttribute)pMethod.GetCustomAttribute<ATFieldAttribute>();
                    Type displayType = methodAttr.displayType;
                    if (paramClassType == null)
                    {
                        if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                            paramClassType = field.FieldType;
                    }
                    if (displayType == null && field.FieldType.IsEnum)
                        displayType = field.FieldType;
                    agv = new ATMethodReturnAttribute(varType, "pReturn", paramClassType, displayType, "", -1, true, true, 3);
                    vargs.Add(agv);
                }
            }
            else if (pMethod.info is PropertyInfo && strLabel.CompareTo("get") == 0)
            {
                PropertyInfo field = pMethod.info as PropertyInfo;
                if(field.CanRead || field.CanWrite)
                {
                    string param = field.Name;
                    ATMethodReturnAttribute agv = null;
                    Type paramClassType = null;
                    Type varType = GetVariableType(field.PropertyType, ref paramClassType);
                    if (field.PropertyType.IsGenericType || field.PropertyType.IsArray)
                    {
                        if (field.PropertyType.IsArray)
                        {

                        }
                    }
                    if (varType != null)
                    {
                        ATFieldAttribute methodAttr = (ATFieldAttribute)pMethod.GetCustomAttribute<ATFieldAttribute>();
                        Type displayType = methodAttr.displayType;
                        if (paramClassType == null)
                        {
                            if (field.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
                                paramClassType = field.PropertyType;
                        }
                        if (displayType == null && field.PropertyType.IsEnum)
                            displayType = field.PropertyType;
                        byte protyFlag = 0;
                        if (field.CanRead && field.CanWrite) protyFlag = 3;
                        else if (field.CanRead) protyFlag = 1;
                        else if (field.CanWrite) protyFlag = 2;
                        agv = new ATMethodReturnAttribute(varType, "pReturn", paramClassType, displayType, "", -1, true, true, protyFlag);
                        vargs.Add(agv);
                    }
                }
            }

            return vargs.ToArray();
        }
        //------------------------------------------------------
        static bool bList(Type paramType)
        {
            if (paramType == null) return false;
            if (paramType.IsGenericType)
            {
                if (paramType.Name.Contains("List`1") && paramType.GenericTypeArguments != null && paramType.GenericTypeArguments.Length == 1)
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        static bool bArray(Type paramType)
        {
            if (paramType == null) return false;
            if (paramType.IsArray && paramType.FullName.Contains("[]"))
            {
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        static bool bMatrix(Type paramType)
        {
            if (paramType == null) return false;
            if (paramType == typeof(Matrix4x4))
            {
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        internal static Type GetVariableType(Type paramType, ref Type classType, string label = "")
        {
            classType = null;
            if (string.IsNullOrEmpty(label) && paramType != null)
                label = paramType.Name;
            label = label.Replace("&", "").ToLower();

            if(paramType!=null)
            {
                if (paramType.IsInterface && paramType.IsDefined(typeof(ATConvertBehaviourAttribute), true))
                {
                    classType = paramType;
                    return typeof(VariableMonoScript);
                }
                if (paramType == typeof(bool)) return typeof(VariableBool);
                if (paramType == typeof(byte)) return typeof(VariableByte);
                if (paramType == typeof(int)) return typeof(VariableInt);
                if (paramType == typeof(short)) return typeof(VariableInt);
                if (paramType == typeof(ushort)) return typeof(VariableInt);
                if (paramType == typeof(uint)) return typeof(VariableInt);
                if (paramType == typeof(float)) return typeof(VariableFloat);
                if (paramType == typeof(long)) return typeof(VariableLong);
                if (paramType == typeof(ulong)) return typeof(VariableLong);
                if (paramType == typeof(Vector2)) return typeof(VariableVector2);
#if USE_FIXEDMATH
                if (paramType == typeof(ExternEngine.FVector2)) return typeof(VariableVector2);
                if (paramType == typeof(ExternEngine.FVector3)) return typeof(VariableVector3);
                if (paramType == typeof(ExternEngine.FQuaternion)) return typeof(VariableQuaternion);
                if (paramType == typeof(ExternEngine.FMatrix4x4)) return typeof(VariableFloatList);
                if (paramType == typeof(ExternEngine.FFloat)) return typeof(VariableFloat);
#endif
                if (paramType == typeof(Vector3)) return typeof(VariableVector3);
                if (paramType == typeof(Vector2Int)) return typeof(VariableVector2Int);
                if (paramType == typeof(Vector3Int)) return typeof(VariableVector3Int);
                if (paramType == typeof(Vector4)) return typeof(VariableVector4);
                if (paramType == typeof(Quaternion)) return typeof(VariableQuaternion);
                if (paramType == typeof(Matrix4x4)) return typeof(VariableFloatList);
                if (paramType == typeof(Color)) return typeof(VariableColor);
                if (paramType == typeof(String)) return typeof(VariableString);
                if (paramType == typeof(AnimationCurve)) return typeof(VariableCurve);
                if (IsDelegate(paramType) ) return typeof(VariableDelegate);
                if (paramType == typeof(System.Type))
                {
                    classType = typeof(System.Type);
                    return typeof(VariableInt);
                }
                if (paramType.IsSubclassOf(typeof(UnityEngine.Behaviour)))
                {
                    classType = paramType;
                    return typeof(VariableMonoScript);
                }
                if (paramType == typeof(UnityEngine.Texture)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Texture2D)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Texture3D)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Material)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Shader)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Transform)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.GameObject)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Camera)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Renderer)) return typeof(VariableObject);
                if (paramType == typeof(UnityEngine.Object)) return typeof(VariableObject);
                if (paramType.IsSubclassOf(typeof(UnityEngine.Object))) return typeof(VariableObject);
                if (paramType.IsEnum) return typeof(VariableInt);
                if (paramType.IsGenericType)
                {
                    if (bList(paramType))
                    {
                        Type classListObjType = null;
                        Type varObj = GetVariableType(paramType.GenericTypeArguments[0], ref classListObjType);
                        if (varObj == null) return null;
                        classType = classListObjType;
                        if (varObj == typeof(VariableBool)) return typeof(VariableBoolList);
                        if (varObj == typeof(VariableByte)) return typeof(VariableByteList);
                        if (varObj == typeof(VariableInt)) return typeof(VariableIntList);
                        if (varObj == typeof(VariableFloat)) return typeof(VariableFloatList);
                        if (varObj == typeof(VariableLong)) return typeof(VariableLongList);
                        if (varObj == typeof(VariableVector2)) return typeof(VariableVector2List);
                        if (varObj == typeof(VariableVector3)) return typeof(VariableVector3List);
                        if (varObj == typeof(VariableVector2Int)) return typeof(VariableVector2IntList);
                        if (varObj == typeof(VariableVector3Int)) return typeof(VariableVector3IntList);
                        if (varObj == typeof(VariableVector4)) return typeof(VariableVector4List);
                        if (varObj == typeof(VariableQuaternion)) return typeof(VariableQuaternionList);
                        if (varObj == typeof(VariableColor)) return typeof(VariableColorList);
                        if (varObj == typeof(VariableString)) return typeof(VariableStringList);
                        if (varObj == typeof(VariableCurve)) return typeof(VariableCurveList);
                        if (varObj == typeof(VariableObject)) return typeof(VariableObjectList);
                        if (varObj == typeof(VariableMonoScript)) return typeof(VariableMonoScriptList);
                        if (varObj == typeof(VariableUser)) return typeof(VariableUserList);
                    }
                    return null;
                }
                else if (paramType.IsArray)
                {
                    if (bArray(paramType))
                    {
                        Type classListObjType = null;
                        Type varObj = GetVariableType(null, ref classListObjType,paramType.FullName.Replace("[]", ""));
                        if (varObj == null) return null;
                        classType = classListObjType;
                        if (varObj == typeof(VariableBool)) return typeof(VariableBoolList);
                        if (varObj == typeof(VariableByte)) return typeof(VariableByteList);
                        if (varObj == typeof(VariableInt)) return typeof(VariableIntList);
                        if (varObj == typeof(VariableLong)) return typeof(VariableLongList);
                        if (varObj == typeof(VariableFloat)) return typeof(VariableFloatList);
                        if (varObj == typeof(VariableVector2)) return typeof(VariableVector2List);
                        if (varObj == typeof(VariableVector3)) return typeof(VariableVector3List);
                        if (varObj == typeof(VariableVector2Int)) return typeof(VariableVector2IntList);
                        if (varObj == typeof(VariableVector3Int)) return typeof(VariableVector3IntList);
                        if (varObj == typeof(VariableVector4)) return typeof(VariableVector4List);
                        if (varObj == typeof(VariableQuaternion)) return typeof(VariableQuaternionList);
                        if (varObj == typeof(VariableColor)) return typeof(VariableColorList);
                        if (varObj == typeof(VariableString)) return typeof(VariableStringList);
                        if (varObj == typeof(VariableCurve)) return typeof(VariableCurveList);
                        if (varObj == typeof(VariableObject)) return typeof(VariableObjectList);
                        if (varObj == typeof(VariableMonoScript)) return typeof(VariableMonoScriptList);
                        if (varObj == typeof(VariableUser)) return typeof(VariableUserList);
                    }
                }
            }
            
            if (label == "byte") return typeof(VariableByte);
            if (label == "int") return typeof(VariableInt);
            if (label == "int32") return typeof(VariableInt);
            if (label == "int16") return typeof(VariableInt);
            if (label == "short") return typeof(VariableInt);
            if (label == "ushort") return typeof(VariableInt);
            if (label == "uint16") return typeof(VariableInt);
            if (label == "uint") return typeof(VariableInt);
            if (label == "uint32") return typeof(VariableInt);
            if (label == "long") return typeof(VariableLong);
            if (label == "ulong") return typeof(VariableLong);
            if (label == "uint64") return typeof(VariableLong);
            if (label == "int64") return typeof(VariableLong);
            if (label == "float") return typeof(VariableFloat);
            if (label == "ffloat") return typeof(VariableFloat);
            if (label == "vector2") return typeof(VariableVector2);
            if (label == "fvector2") return typeof(VariableVector2);
            if (label == "vector3") return typeof(VariableVector3);
            if (label == "fvector3") return typeof(VariableVector3);
            if (label == "vector2int") return typeof(VariableVector2Int);
            if (label == "vector3int") return typeof(VariableVector3Int);
            if (label == "vector4") return typeof(VariableVector4);
            if (label == "fvector4") return typeof(VariableVector4);
            if (label == "quaternion") return typeof(VariableQuaternion);
            if (label == "matrix4x4") return typeof(VariableFloatList);
            if (label == "fmatrix4x4") return typeof(VariableFloatList);
            if (label == "color") return typeof(VariableColor);
            if (label == "string") return typeof(VariableString);
            if (label == "animationcurve") return typeof(VariableCurve);
            if (label == "system.type")
            {
                classType = typeof(System.Type);
                return typeof(VariableInt);
            }
            else if (paramType != null && EXPORTCLASS.ContainsKey(paramType.FullName.ToLower()))
            {
                classType = EXPORTCLASS[paramType.FullName.ToLower()];
                bool bMono = false;
                bool bUserClass = false;
                CheckClassOrMono(classType, ref bMono, ref bUserClass);
                if (bUserClass)
                {
                    return typeof(VariableUser);
                }
                else if (bUserClass)
                    return typeof(VariableMonoScript);
            }
            else if (EXPORTCLASS.ContainsKey(label))
            {
                classType = EXPORTCLASS[label];
                bool bMono = false;
                bool bUserClass = false;
                CheckClassOrMono(classType, ref bMono, ref bUserClass);
                if (bUserClass)
                {
                    return typeof(VariableUser);
                }
                else if (bUserClass)
                    return typeof(VariableMonoScript);

                if (IsUnityObject(classType))
                {
                    return typeof(VariableObject);
                }
            }
            if (paramType!=null)
            {
                if(paramType == typeof(IUserData))
                {
                    classType = paramType;
                    return typeof(VariableUser);
                }
                else if(IsInterface(paramType, typeof(IUserData)))
                {
                    if(IsConvertBehaviour(paramType))
                    {
                        classType = paramType;
                        return typeof(VariableMonoScript);
                    }
                    else
                    {
                        classType = paramType;
                        return typeof(VariableUser);
                    }
                }
            }
            return null;
        }
        //------------------------------------------------------
        static string GetTypeFullName(System.Type type)
        {
            return type.FullName.Replace("+", ".");
        }
        //------------------------------------------------------
        static StructDelca BuildStaticMemberDelcaByList(System.Collections.Generic.Dictionary<string, List<StructDelca>> vDelca, Type paramType,int lesstCount = -1)
        {
            if (bList(paramType))
            {
               string strClassName = paramType.GenericTypeArguments[0].FullName;

                List<StructDelca> vDelc = null;
                if (!vDelca.TryGetValue(strClassName, out vDelc))
                {
                    vDelc = new List<StructDelca>();
                    vDelca.Add(strClassName, vDelc);
                }
                if (lesstCount > 0)
                {
                    if (vDelc.Count >= lesstCount)
                        return vDelc[0];
                }
                StructDelca delc = new StructDelca();
                delc.strClassName = strClassName.Replace("+", ".");
                delc.strName = "ms_" + paramType.GenericTypeArguments[0].Name + "_Catch_" + vDelc.Count;
                delc.strCode = "System.Collections.Generic.List<" + strClassName.Replace("+",".") + ">";
                delc.strCodeArray = strClassName.Replace("+", ".") + "[] ";
                vDelc.Add(delc);

                return delc;
            }
            if (bArray(paramType))
            {
                string strClassName = paramType.FullName.Replace("[]","");

                List<StructDelca> vDelc = null;
                if (!vDelca.TryGetValue(strClassName, out vDelc))
                {
                    vDelc = new List<StructDelca>();
                    vDelca.Add(strClassName, vDelc);
                }
                if (lesstCount > 0)
                {
                    if (vDelc.Count >= lesstCount)
                        return vDelc[0];
                }
                StructDelca delc = new StructDelca();
                delc.strClassName = strClassName;
                delc.strName = "ms_" + paramType.Name.Replace("[]","") + "_Catch_" + vDelc.Count;
                delc.strCode = "System.Collections.Generic.List<" + strClassName.Replace("+", ".") + ">";
                delc.strCodeArray = strClassName.Replace("+", ".") + "[] ";
                vDelc.Add(delc);

                return delc;
            }
            return null;
        }
        //------------------------------------------------------
        static string GetVariableToArgvParamLabel(Type paramType, ParameterInfo paramInfo, Type displayType = null)
        {
            Type paramTT = paramInfo.ParameterType;
            string label = "";
            if (paramInfo.IsOut) label = "out ";
            else if (paramInfo.IsIn || paramTT.IsByRef) label = "ref ";
            if (displayType != null)
                return label + "(" + displayType.FullName.Replace("+", ".") + ")";
            if (paramTT == typeof(ushort)) return label + "(ushort)";
            if (paramTT == typeof(System.UInt16)) return label + "(ushort)";
            if (paramTT == typeof(short)) return label + "(short)";
            if (paramTT == typeof(uint)) return label + "(uint)";
            if (paramTT == typeof(System.UInt32)) return label + "(uint)";
            if (paramTT == typeof(ulong)) return label + "(ulong)";
            if (paramTT == typeof(System.UInt64)) return label + "(ulong)";
            if (paramTT.IsEnum) return label + "(" + paramTT.FullName.Replace("+", ".") + ")";
        //    if (displayType != null)
            if (EXPORTCLASS.ContainsKey(paramTT.FullName.ToLower()))
            {
                Type classType = EXPORTCLASS[paramType.FullName.ToLower()];
                return label + "(" + classType.FullName.Replace("+", ".") + ")";
            }
            else if (paramTT.IsSubclassOf(typeof(UnityEngine.Object))) return label + "(" + paramTT.FullName.Replace("+", ".") + ")";
            else if (IsConvertBehaviour(paramTT)) return label + "(UnityEngine.Behaviour)";

            return label;
        }
        //------------------------------------------------------
        static string GetVariableToArgvParamLabel(Type paramType)
        {
            string label = "";
            if (paramType == typeof(ushort)) return label + "(ushort)";
            if (paramType == typeof(System.UInt16)) return label + "(ushort)";
            if (paramType == typeof(short)) return label + "(short)";
            if (paramType == typeof(uint)) return label + "(uint)";
            if (paramType == typeof(System.UInt32)) return label + "(uint)";
            if (paramType.IsEnum) return label + "(" + paramType.FullName.Replace("+", ".") + ")";
            //    if (displayType != null)
            if (EXPORTCLASS.ContainsKey(paramType.FullName.ToLower()))
            {
                Type classType = EXPORTCLASS[paramType.FullName.ToLower()];
                return label + "(" + classType.FullName.Replace("+", ".") + ")";
            }
            else if (paramType.IsSubclassOf(typeof(UnityEngine.Object))) return label + "(" + paramType.FullName.Replace("+", ".") + ")";
            else if(IsConvertBehaviour(paramType)) return label + "(UnityEngine.Behaviour)";
            return label;
        }
        //------------------------------------------------------
        static string GetArgvParamToVariableLabel(Type paramTT)
        {
            string label = "";
            if (paramTT == typeof(ushort)) return label + "(int)";
            if (paramTT == typeof(System.UInt16)) return label + "(int)";
            if (paramTT == typeof(short)) return label + "(int)";
            if (paramTT == typeof(uint)) return label + "(int)";
            if (paramTT == typeof(System.UInt32)) return label + "(int)";
            if (paramTT.IsEnum) return label + "(int)";
            if (IsConvertBehaviour(paramTT)) return label + "(UnityEngine.Behaviour)";
            return label;
        }
    }
}
#endif