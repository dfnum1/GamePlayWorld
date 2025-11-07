#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public class SetAutoCode
    {
        struct TypeData
        {
            public Plugin.AT.VariableTypeAttribute attri;
            public Type type;
        }
        struct ActionData
        {
            public Plugin.AT.ATMethodAttribute attri;
            public string funcName;
        }

        [MenuItem("Tools/AT/映射")]
        public static void Build()
        {
            string runtimePath = AgentTreeEditorPath.BuildInstallPath();
            if (!Directory.Exists(runtimePath))
                return;
            Assembly assembly = null;
            List<TypeData> vTypes = new List<TypeData>();
            Dictionary<EActionType, ActionData> vActions = new Dictionary<EActionType, ActionData>();
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
              //  if (ass.GetName().Name == "ScriptsHot" || ass.GetName().Name == "GameEditor") continue;
                {
                    assembly = ass;
                    Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        Type tp = types[i];
                        if (tp.IsDefined(typeof(Plugin.AT.VariableTypeAttribute), false))
                        {
                            Plugin.AT.VariableTypeAttribute attr = (Plugin.AT.VariableTypeAttribute)tp.GetCustomAttribute(typeof(Plugin.AT.VariableTypeAttribute));
                            TypeData type = new TypeData();
                            type.attri = attr;
                            type.type = tp;
                            vTypes.Add(type);
                        }
                        else if (tp.IsDefined(typeof(Plugin.AT.ATExportAttribute), false))
                        {
                            MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                            for (int m = 0; m < meths.Length; ++m)
                            {
                                if (meths[m].IsDefined(typeof(ATMethodAttribute), false))
                                {
                                    ATMethodAttribute attr = (ATMethodAttribute)meths[m].GetCustomAttribute(typeof(ATMethodAttribute));
                                    if (attr.ActionType != (int)EActionType.None)
                                    {
                                        ActionData action = new ActionData();
                                        action.funcName = tp.FullName + "." + meths[m].Name;
                                        action.attri = attr;
                                        vActions[(EActionType)attr.ActionType] = action;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            string path = AgentTreeEditorPath.BuildVarSerializerCodePath();
            string atmgr_path = AgentTreeEditorPath.BuildVarFactoryCodePath();
            string vars_pools = AgentTreeEditorPath.BuildVarPoolCodePath();
            string ats_funcs = AgentTreeEditorPath.BuildATFunctionCodePath();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string code = "//auto generator\r\n";
            code += "using System;\r\n";
            code += "using System.Collections.Generic;\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Plugin.AT\r\n";
            code += "{\r\n";
            code += "\t[Serializable]\r\n";
            code += "\tpublic class VariableSerializes\r\n";
            code += "\t{\r\n";
            for(int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\tpublic "+ vTypes[i].type.FullName + "[] var" + vTypes[i].type.Name + ";\r\n";
            }

            code += "\tpublic bool isValid()\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\tif( var" + vTypes[i].type.Name + " != null && var" + vTypes[i].type.Name + ".Length>0) return true" + ";\r\n";
            }
            code += "\t\treturn false;\r\n";
            code += "\t}\r\n";

            code += "\tpublic void Clear()\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\tvar" + vTypes[i].type.Name + " = null;\r\n";
            }
            code += "\t}\r\n";

            code += "\tpublic void InitRuntime(Dictionary<int, Variable> vMap, ref int maxInstance, bool bClone= false, bool bIncludeGuid=false)\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\tif(bClone) CloneNew(var"+ vTypes[i].type.Name + ", vMap, ref maxInstance, bIncludeGuid);\r\n";
                code += "\t\telse InitVaribles(var" + vTypes[i].type.Name + ", vMap, ref maxInstance);\r\n";
                code += "\r\n";
            }

            code += "\t}\r\n";

            code += "\tvoid InitVaribles<T>(T[] Vars, Dictionary<int, Variable> vMap, ref int maxInstance) where T : Variable\r\n";
            code += "\t{\r\n";
            code += "\t\tif (Vars == null) return;\r\n";
            code += "\t\tfor (int i = 0; i < Vars.Length; ++i)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tvMap.Add(Vars[i].GUID, Vars[i]);\r\n";
            code += "\t\t\tmaxInstance = Mathf.Max(Vars[i].GUID, maxInstance);\r\n";
            code += "\t\t}\r\n";
            code += "\t}\r\n";

            code += "\tvoid CloneNew<T>(T[] Vars, Dictionary<int, Variable> vMap, ref int maxInstance, bool bIncludeGuid) where T : Variable\r\n";
            code += "\t{\r\n";
            code += "\t\tif (Vars == null) return;\r\n";
            code += "\t\tfor (int i = 0; i < Vars.Length; ++i)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tVariable pClone = AgentTreeManager.getInstance().NewVariableByType(typeof(T), bIncludeGuid ? Vars[i].GUID : 0);\r\n";
            code += "\t\t\tpClone.Copy(Vars[i], false);\r\n";
            code += "\t\t\tvMap.Add(pClone.GUID, pClone);\r\n";
            code += "\t\t\tmaxInstance = Mathf.Max(pClone.GUID, maxInstance);;\r\n";
            code += "\t\t}\r\n";
            code += "\t}\r\n";


            code += "#if UNITY_EDITOR\r\n";

            code += "\tpublic Variable GetVariable(int guid)\r\n";
            code += "\t{\r\n";
            code += "\t\tSystem.Reflection.FieldInfo[] fiels = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);\r\n";
            code += "\t\tfor (int i = 0; i < fiels.Length; ++i)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tArray list = (Array)fiels[i].GetValue(this);\r\n";
            code += "\t\t\tif (list == null) continue;\r\n";
            code += "\t\t\tfor (int j = 0; j < list.Length; ++j)\r\n";
            code += "\t\t\t{\r\n";
            code += "\t\t\t\tVariable temp = list.GetValue(j) as Variable;\r\n";
            code += "\t\t\t\tif(temp.GUID == guid)\r\n";
            code += "\t\t\t\t\treturn temp;\r\n";
            code += "\t\t\t}\r\n";
            code += "\t\t}\r\n";
            code += "\t\treturn null;\r\n";
            code += "\t}\r\n";

            code += "\tpublic static System.Type GetVariableType(EVariableType type)\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\tif(type == EVariableType." + vTypes[i].attri.valType + ")\r\n";
                code += "\t\treturn typeof(" + vTypes[i].type.FullName + ");\r\n";
            }
            code += "\t\treturn null;\r\n";
            code += "\t}\r\n";

            code += "\tpublic static EVariableType GetVariableType(System.Type type)\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\tif(type == typeof(" + vTypes[i].type.FullName + "))" + "return EVariableType." + vTypes[i].attri.valType + ";\r\n";
            }
            code += "\t\treturn EVariableType.Null;\r\n";
            code += "\t}\r\n";

            code += "\tpublic bool AddVariable<T>(T pVar) where T : Variable\r\n";
            code += "\t{\r\n";
            code += "\t\tstring strName = \"var\" + pVar.GetType().Name;\r\n";
            code += "\t\tSystem.Reflection.FieldInfo[] fiels = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);\r\n";
            code += "\t\tfor (int i = 0; i < fiels.Length; ++i)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif(fiels[i].Name.CompareTo(strName)==0)\r\n";
            code += "\t\t\t{\r\n";
            for(int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\t\t\tif(pVar is " + vTypes[i].type.FullName + ")\r\n";
                code += "\t\t\t\t{\r\n";
                code += "\t\t\t\t\t" + vTypes[i].type.FullName + "[]" + " list = " + "(" + vTypes[i].type.FullName + "[])fiels[i].GetValue(this);" + "\r\n";
                code += "\t\t\t\t\tAddVaribles<" + vTypes[i].type.FullName + ">(ref list, pVar as " + vTypes[i].type.FullName + ");\r\n";
                code += "\t\t\t\t\tfiels[i].SetValue(this, list);\r\n";
                code += "\t\t\t\t\treturn true;\r\n";
                code += "\t\t\t\t}\r\n";
            }
            code += "\t\t\t\tbreak;\r\n";
            code += "\t\t\t}\r\n";
            code += "\t\t}\r\n";
            code += "\t\tUnityEditor.EditorUtility.DisplayDialog(\"提示\", \"变量\" + pVar.strName + \"添加失败!\", \"请确认\");\r\n";
            code += "\t\treturn false;\r\n";
            code += "\t}\r\n";

            code += "\tvoid AddVaribles<T>(ref T[] Vars, T pVar) where T : Variable\r\n";
            code += "\t{\r\n";
            code += "\t\tList<T> Lists = null;\r\n";
            code += "\t\tif (Vars != null) Lists = new List<T>(Vars);\r\n";
            code += "\t\telse Lists = new List<T>();\r\n";
            code += "\t\tLists.Add(pVar);\r\n";
            code += "\t\tVars = Lists.ToArray();\r\n";
            code += "\t}\r\n";

            code += "#endif\r\n";
            code += "\t}\r\n";
            code += "}\r\n";
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();

            //! build pools
            if (File.Exists(vars_pools))
            {
                File.Delete(vars_pools);
            }
            code = "//auto generator\r\n";
            code += "using System;\r\n";
            code += "using System.Collections.Generic;\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Plugin.AT\r\n";
            code += "{\r\n";
            code += "\tpublic class VariablePools\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\tpublic List<" + vTypes[i].type.FullName + "> var" + vTypes[i].type.Name + ";\r\n";
            }
            code += "\t\tpublic void Init(int max)\r\n";
            code += "\t\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\t\tvar" + vTypes[i].type.Name + "=" + "new List<" + vTypes[i].type.FullName + ">" + "(max)" + ";\r\n";
            }
            code += "\t\t}\r\n";

            code += "\t\tpublic void Clear()\r\n";
            code += "\t\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\t\tfor(int i = 0; i < var" + vTypes[i].type.Name + ".Count; ++i)\r\n";
                code += "\t\t\t\tvar" + vTypes[i].type.Name + "[i].Destroy();\r\n";
            }
            code += "\t\t}\r\n";

            code += "\t\tpublic void Destroy()\r\n";
            code += "\t\t{\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\t\tfor(int i = 0; i < var" + vTypes[i].type.Name + ".Count; ++i)\r\n";
                code += "\t\t\t\tvar" + vTypes[i].type.Name + "[i].Destroy();\r\n";
                code += "\t\t\t\tvar" + vTypes[i].type.Name + "=null;\r\n";
            }
            code += "\t\t}\r\n";

            code += "\t}\r\n";
            code += "}\r\n";

            fs = new FileStream(vars_pools, FileMode.OpenOrCreate);
            writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();

            //! build at action do
            if (File.Exists(ats_funcs))
            {
                File.Delete(ats_funcs);
            }
            code = "//auto generator\r\n";
            code += "using System;\r\n";
            code += "using System.Collections.Generic;\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Plugin.AT\r\n";
            code += "{\r\n";
            code += "\tpublic partial class AgentTreeTask\r\n";
            code += "\t{\r\n";
            code += "\t\tbool DoInerAction(ActionNode pNode, int funcId=0)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t#if UNITY_EDITOR\r\n";
            code += "\t\t\tif (AgentTreeManager.OnExcudeAction != null) AgentTreeManager.OnExcudeAction(this, pNode);\r\n";
            code += "\t\t#endif\r\n";
            code += "\t\t\tint caseId = (funcId!=0)?funcId:(int)pNode.actionType;\r\n";
            code += "\t\t\tswitch(caseId)\r\n";
            code += "\t\t\t{\r\n";
            foreach (var act in vActions)
            {
                code += "\t\t\t\tcase " + ((int)act.Key).ToString() + ": return " + act.Value.funcName + "(this, pNode);";
                code += "//"+ act.Key.ToString() + "\r\n";
            }
            code += "\t\t\t}\r\n";
            code += "\t\t\tAgentTreeManager.getInstance().OnActionCallback(this, pNode, funcId);\r\n";
            code += "\t\t\treturn true;\r\n";
            code += "\t\t}\r\n";
            code += "\t}\r\n";
            code += "}\r\n";

            fs = new FileStream(ats_funcs, FileMode.OpenOrCreate);
            writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();

            //! building mgr
            if (File.Exists(atmgr_path))
            {
                File.Delete(atmgr_path);
            }
            code = "//auto generator\r\n";
            code += "using System;\r\n";
            code += "using System.Collections.Generic;\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Plugin.AT\r\n";
            code += "{\r\n";
            code += "\tpublic partial class VariableFactory\r\n";
            code += "\t{\r\n";
            code += "\t\tpublic static System.Action<Variable> OnNewVariable;\r\n";
            code += "\t\tprivate int m_nPoolMax = 32;\r\n";
            code += "\t\tVariablePools m_Pool = new VariablePools();\r\n";
            code += "\t\tprivate Dictionary<int, Variable> m_vGlobalVariables = null;\r\n";
            code += "\t\tpublic VariableFactory()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tm_vGlobalVariables = new Dictionary<int, Variable>(32);\r\n";
            code += "\t\t\tm_Pool = new VariablePools();\r\n";
            code += "\t\t\tm_Pool.Init(m_nPoolMax);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t~VariableFactory()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tm_vGlobalVariables = null;\r\n";
            code += "\t\t\tm_Pool = null;\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic void Clear()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tm_Pool.Clear();\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic T GetGlobalVariable<T>(int guid, bool bAutoNew = false) where T : Variable\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tVariable variable;\r\n";
            code += "\t\t\tif (m_vGlobalVariables.TryGetValue(guid, out variable))\r\n";
            code += "\t\t\t\treturn variable as T;\r\n";
            code += "\t\t\tif(bAutoNew)\r\n";
            code += "\t\t\t{\r\n";
            code += "\t\t\t\tVariable pVar = NewVariableByType( typeof(T), guid );\r\n";
            code += "\t\t\t\tif(pVar!=null) m_vGlobalVariables.Add(guid, pVar);\r\n";
            code += "\t\t\t\treturn pVar as T;\r\n";
            code += "\t\t\t}\r\n";
            code += "\t\t\treturn null;\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic void Recycel(Variable pVar)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif (pVar == null) return;\r\n";
            code += "\t\t\tpVar.Destroy();\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                code += "\t\t\tif(typeof(" + vTypes[i].type.FullName + ") == pVar.GetType())\r\n";
                code += "\t\t\t{\r\n";
                code += "\t\t\t\tif(m_Pool.var" + vTypes[i].type.Name + ".Count < m_nPoolMax)\r\n";
                code += "\t\t\t\tm_Pool.var" + vTypes[i].type.Name + ".Add(pVar as " + vTypes[i].type.FullName + ");\r\n";
                code += "\t\t\t\treturn;\r\n";
                code += "\t\t\t}\r\n";
            }
            code += "\t\t}\r\n";

            code += "\t\tpublic T NewVariable<T>(string strName, int nGUID = 0) where T : Variable, new()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tT newT = NewVariableByType(typeof(T), nGUID) as T;\r\n";
            code += "\t\t\t#if UNITY_EDITOR\r\n";
            code += "\t\t\tif(newT!=null) newT.strName = strName;\r\n";
            code += "\t\t\t#endif\r\n";
            code += "\t\t\treturn newT;\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic T NewVariable<T>(int nGUID = 0) where T : Variable, new()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\treturn NewVariableByType(typeof(T), nGUID) as T;\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic Variable NewVariableByType(System.Type type, int nGUID = 0, bool bEditor = false)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tVariable newT = null;\r\n";
            for (int i = 0; i < vTypes.Count; ++i)
            {
                if(i == 0)
                    code += "\t\t\tif(typeof(" + vTypes[i].type.FullName + ") == type)\r\n";
                else
                    code += "\t\t\telse if(typeof(" + vTypes[i].type.FullName + ") == type)\r\n";

                code += "\t\t\t{\r\n";
                code += "\t\t\t\tif(!bEditor && m_Pool.var" + vTypes[i].type.Name + ".Count>0)\r\n";
                code += "\t\t\t\t{\r\n";
                code += "\t\t\t\t\tnewT = m_Pool.var" + vTypes[i].type.Name  + "[0];\r\n";
                code += "\t\t\t\t\tnewT.Destroy();\r\n";
                code += "\t\t\t\t\tm_Pool.var" + vTypes[i].type.Name + ".RemoveAt(0);\r\n";
                code += "\t\t\t\t\tif(OnNewVariable!=null) OnNewVariable(newT);\r\n";
                code += "\t\t\t\t\treturn newT;\r\n";
                code += "\t\t\t\t}\r\n";
                code += "\t\t\t\tnewT = new " + vTypes[i].type.FullName + "();\r\n";
                code += "\t\t\t}\r\n";
            }
            code += "\t\t\tif(newT == null) return null;\r\n";
            code += "\t\t\tif(nGUID == 0)newT.GUID = AgentTreeManager.AutoGUID();\r\n";
            code += "\t\t\telse newT.GUID = nGUID;\r\n";
            code += "\t\t\tif(OnNewVariable!=null) OnNewVariable(newT);\r\n";
            code += "\t\t\treturn newT;\r\n";
            code += "\t\t}\r\n";

            code += "\t}\r\n";
            code += "}\r\n";

            fs = new FileStream(atmgr_path, FileMode.OpenOrCreate);
            writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();
        }
    }
}
#endif