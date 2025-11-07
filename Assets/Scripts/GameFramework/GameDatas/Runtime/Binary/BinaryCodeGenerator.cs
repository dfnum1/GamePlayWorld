#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	BinaryCodeGenerator
作    者:	HappLI
描    述:	二进制序列代码自动生成器
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using Framework.Base;

namespace Framework.Data
{
    public class BinaryCodeGenerator
    {
        class ClassBinaryCode
        {
            public System.Type classType = null;
            public int verison = 0;
            public List<string> ReadCodes = new List<string>();
            public List<string> WriteCodes = new List<string>();
            public List<string> ServerReadCodes = new List<string>();
            public List<string> ServerWriteCodes = new List<string>();
            public string strReadFunc = "";
            public string strWriteFunc = "";
            public string GetFullName()
            {
                return classType.FullName.Replace("+", ".");
            }
        }
        private static Dictionary<string, ClassBinaryCode> ClassCodeMapping = new Dictionary<string, ClassBinaryCode>();
        public static string GeneratorCode = "/Scripts/GameDatas/Binary/Generator";
        public static string ServerGeneratorCode = "/../../Server/work_spaces/Game-Common/src/main/java/com/topgame/common/generate_binary_config";
        [MenuItem("Tools/代码/BinaryCoder")]
        public static void Build()
        {
            if (!Directory.Exists(Application.dataPath + GeneratorCode))
            {
                Directory.CreateDirectory(Application.dataPath + GeneratorCode);
            }
            Assembly assembly = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                assembly = ass;
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type tp = types[i];
                    if (tp.IsDefined(typeof(BinaryCodeAttribute), false))
                    {
                        ClassCodeMapping.Clear();
                        BuildCode(Application.dataPath + GeneratorCode, tp);
                        if (tp.IsDefined(typeof(BinaryServerCodeAttribute), false))
                        {
                            if (Directory.Exists(Application.dataPath + ServerGeneratorCode))
                            {
                                ClassCodeMapping.Clear();
                                BuildServerCode(Application.dataPath + ServerGeneratorCode, tp);
                            }
                        }
                    }
                }
            }
        }
        static bool BinaryerFunc(System.Type type, ref string strRead, ref string strWrite)
        {
            if (type == typeof(bool))
            {
                strRead = "ToBool";
                strWrite = "WriteBool";
                return true;
            }
            else if (type == typeof(byte))
            {
                strRead = "ToByte";
                strWrite = "WriteByte";
            }
            else if (type.IsEnum)
            {
                strRead = "ToUshort";
                strWrite = "WriteUshort";
            }
            else if (type == typeof(char))
            {
                strRead = "ToChar";
                strWrite = "WriteChar";
            }
            else if (type == typeof(short))
            {
                strRead = "ToShort";
                strWrite = "WriteShort";
            }
            else if (type == typeof(ushort))
            {
                strRead = "ToUshort";
                strWrite = "WriteUshort";
            }
            else if (type == typeof(int))
            {
                strRead = "ToInt32";
                strWrite = "WriteInt32";
            }
            else if (type == typeof(uint))
            {
                strRead = "ToUint32";
                strWrite = "WriteUint32";
            }
            else if (type == typeof(long))
            {
                strRead = "ToInt64";
                strWrite = "WriteInt64";
            }
            else if (type == typeof(ulong))
            {
                strRead = "ToUint64";
                strWrite = "WriteUint64";
            }
            else if (type == typeof(float))
            {
                strRead = "ToFloat";
                strWrite = "WriteFloat";
            }
            else if (type == typeof(double))
            {
                strRead = "ToDouble";
                strWrite = "WriteDouble";
            }
            else if (type == typeof(Vector2))
            {
                strRead = "ToVec2";
                strWrite = "WriteVector2";
            }
            else if (type == typeof(Vector3))
            {
                strRead = "ToVec3";
                strWrite = "WriteVector3";
            }
            else if (type == typeof(Vector4))
            {
                strRead = "ToVec4";
                strWrite = "WriteVector4";
            }
            else if (type == typeof(Vector2Int))
            {
                strRead = "ToVec2Int";
                strWrite = "WriteVector2Int";
            }
            else if (type == typeof(Vector3Int))
            {
                strRead = "ToVec3Int";
                strWrite = "WriteVector3Int";
            }
            else if (type == typeof(Color))
            {
                strRead = "ToColor";
                strWrite = "WriteColor";
            }
            else if (type == typeof(AnimationCurve))
            {
                strRead = "ToCurve";
                strWrite = "WriteCurve";
            }
            else if (type == typeof(string))
            {
                strRead = "ToString";
                strWrite = "WriteString";
            }
            return !string.IsNullOrEmpty(strRead);
        }

        #region binaryClient
        //------------------------------------------------------
        static void BuildSegmentFieldCode(System.Type type, string fieldName, ref List<string> strReadCode, ref List<string> strWriteCode)
        {
            string strName = fieldName;
            string readapi = "";
            string writeapi = "";
            if(BinaryerFunc(type,ref readapi, ref writeapi))
            {
                if (type.IsEnum)
                {
                    strReadCode.Add(strName + " = (" + type.FullName.Replace("+",".") + ")binaryer.ToUshort();\r\n");
                    strWriteCode.Add("binaryer.WriteUshort((ushort)" + strName + ");\r\n");
                }
                else
                {
                    strReadCode.Add(strName + " = binaryer."+ readapi + "();\r\n");
                    strWriteCode.Add("binaryer."+ writeapi + "(" + strName + ");\r\n");
                }
            }
            else if (type.IsArray || type.IsGenericType)
            {
                if (type.IsArray)
                {
                    System.Type datatType = type.Assembly.GetType(type.FullName.Replace("[]", ""));
                    strWriteCode.Add("binaryer.WriteShort((short)(" + strName + "!=null?" + strName + ".Length:0));\r\n");

                    strReadCode.Add("short " + strName.Replace(".", "") + "Cnt = binaryer.ToShort();\r\n");
                    strReadCode.Add("if("+ strName.Replace(".", "") + "Cnt>0) " + strName + " = new " + datatType.FullName.Replace("+", ".") + "[" + strName.Replace(".", "") + "Cnt];\r\n" );
                    strReadCode.Add("for(short i = 0; i < " + strName.Replace(".", "") + "Cnt; ++i)\r\n");
                    strReadCode.Add("{\r\n");

                    strWriteCode.Add("if("+ strName+"!=null)\r\n");
                    strWriteCode.Add("{\r\n");
                    strWriteCode.Add("\tfor(short i = 0; i < " + strName + ".Length; ++i)\r\n");
                    strWriteCode.Add("\t{\r\n");

                    string readapi1 = "";
                    string writeapi1 = "";

                    bool bApiDo = BinaryerFunc(datatType, ref readapi1, ref writeapi1);
                        ClassBinaryCode classCode = null;
                    if (!bApiDo &&(datatType.IsClass || datatType.BaseType== typeof(System.ValueType)))
                    {
                        if(!ClassCodeMapping.ContainsKey(datatType.FullName))
                        {
                            classCode = new ClassBinaryCode();
                            classCode.classType = datatType;
                            ClassCodeMapping[classCode.classType.FullName] = classCode;
                            BuildSegmentTypeCode(classCode);
                        }
                        strReadCode.Add("\t" + datatType.FullName.Replace("+", ".") + " temp = " + "Read" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(ref binaryer, version)" + ";\r\n");
                        strReadCode.Add("\t" + strName + "[i] = temp;\r\n");

                        strWriteCode.Add("\t\t" + "Write" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + "[i], " + "ref binaryer)" + ";\r\n");
                    }
                    else
                    {
                        List<string> SubRead = new List<string>();
                        List<string> SubWrite = new List<string>();
                        BuildSegmentFieldCode(datatType, strName + "[i]",ref SubRead, ref SubWrite);
                        for (int i = 0; i < SubRead.Count; ++i)
                        {
                            if(!string.IsNullOrEmpty(SubRead[i]))
                                strReadCode.Add("\t" + SubRead[i]);
                        }
                        for (int i = 0; i < SubWrite.Count; ++i)
                        {
                            if(!string.IsNullOrEmpty(SubWrite[i]))
                                strWriteCode.Add("\t\t" + SubWrite[i]);
                        }
                    }

                    strReadCode.Add("}\r\n");

                    strWriteCode.Add("\t}\r\n");
                    strWriteCode.Add("}\r\n");
                }
                else if (type.Name.Contains("List`1") && type.GenericTypeArguments != null && type.GenericTypeArguments.Length == 1)
                {
                    System.Type datatType = type.GenericTypeArguments[0];
                    strWriteCode.Add("binaryer.WriteShort((short)(" + strName + "!=null?" + strName + ".Count:0));\r\n");

                    
                    strReadCode.Add("short " + strName.Replace(".","") + "Cnt = binaryer.ToShort();\r\n");
                    strReadCode.Add("if(" + strName.Replace(".", "") + "Cnt>0)" + strName + " = new System.Collections.Generic.List<" + datatType.FullName.Replace("+", ".") + ">(" + strName.Replace(".", "") + "Cnt);\r\n");
                    strReadCode.Add("for(short i = 0; i < " + strName.Replace(".", "") + "Cnt; ++i)\r\n");
                    strReadCode.Add("{\r\n");

                    strWriteCode.Add("if(" + strName + "!=null)\r\n");
                    strWriteCode.Add("{\r\n");
                    strWriteCode.Add("\tfor(short i = 0; i < " + strName + ".Count; ++i)\r\n");
                    strWriteCode.Add("\t{\r\n");

                    string readapi1 = "";
                    string writeapi1 = "";

                    bool bApiDo = BinaryerFunc(datatType, ref readapi1, ref writeapi1);
                    ClassBinaryCode classCode = null;
                    if (!bApiDo && (datatType.IsClass || datatType.BaseType == typeof(System.ValueType)))
                    {
                        if(!ClassCodeMapping.ContainsKey(datatType.FullName))
                        {
                            classCode = new ClassBinaryCode();
                            classCode.classType = datatType;
                            ClassCodeMapping[classCode.classType.FullName] = classCode;
                            BuildSegmentTypeCode(classCode);
                        }
                        strReadCode.Add("\t" + datatType.FullName.Replace("+", ".") + " temp =" + "Read" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(ref binaryer, version)" + ";\r\n");
                        strReadCode.Add("\t" + strName + ".Add(temp);\r\n");

                        strWriteCode.Add("\t\t" + "Write" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + "[i], " + "ref binaryer)" + ";\r\n");
                    }
                    else
                    {
                        List<string> SubRead = new List<string>();
                        List<string> SubWrite = new List<string>();
                        BuildSegmentFieldCode(datatType, strName + "[i]", ref SubRead, ref SubWrite);
                        for (int i = 0; i < SubRead.Count; ++i)
                        {
                            strReadCode.Add("\t"+ strName + ".Add(" + SubRead[i].Replace("=", "").Replace(strName + "[i]", "").Replace(";\r\n", "") + ");\r\n");
                        }
                        for (int i = 0; i < SubWrite.Count; ++i)
                            strWriteCode.Add("\t\t" + SubWrite[i]);
                    }

                    strReadCode.Add("}\r\n");

                    strWriteCode.Add("\t}\r\n");
                    strWriteCode.Add("}\r\n");
                }
                else if (type.Name.Contains("Dictionary") && type.GenericTypeArguments != null && type.GenericTypeArguments.Length == 2)
                {
                    System.Type keyType = type.GenericTypeArguments[0];
                    System.Type datatType = type.GenericTypeArguments[1];

                    string keyRead = "";
                    string keyWrite = "";
                    if(BinaryerFunc(keyType, ref keyRead, ref keyWrite))
                    {
                        strWriteCode.Add("binaryer.WriteShort((short)(" + strName + "!=null?" + strName + ".Count:0));\r\n");
                        string dbKey = "db_" + strName.Replace(".", "_");

                        strReadCode.Add("short " + strName.Replace(".", "") + "Cnt = binaryer.ToShort();\r\n");
                        strReadCode.Add("if(" + strName.Replace(".", "") + "Cnt>0)" + strName + " = new System.Collections.Generic.Dictionary<" + keyType.FullName.Replace("+", ".") + ", " + datatType.FullName.Replace("+", ".") + ">(" + strName.Replace(".", "") + "Cnt);\r\n");
                        strReadCode.Add("for(short i = 0; i < " + strName.Replace(".", "") + "Cnt; ++i)\r\n");
                        strReadCode.Add("{\r\n");

                        strWriteCode.Add("if(" + strName + "!=null)\r\n");
                        strWriteCode.Add("{\r\n");
                        strWriteCode.Add("\tforeach(var " + dbKey + " in " + strName + ")\r\n");
                        strWriteCode.Add("\t{\r\n");

                        string readapi1 = "";
                        string writeapi1 = "";

                        bool bApiDo = BinaryerFunc(datatType, ref readapi1, ref writeapi1);
                        ClassBinaryCode classCode = null;
                        if (!bApiDo && (datatType.IsClass || datatType.BaseType == typeof(System.ValueType)))
                        {
                            if (!ClassCodeMapping.ContainsKey(datatType.FullName))
                            {
                                classCode = new ClassBinaryCode();
                                classCode.classType = datatType;
                                ClassCodeMapping[classCode.classType.FullName] = classCode;
                                BuildSegmentTypeCode(classCode);
                            }
                            strReadCode.Add("\t" + keyType.FullName.Replace("+", ".") + " tempkey = binaryer." + keyRead + "();\r\n");
                            strReadCode.Add("\t" + datatType.FullName.Replace("+", ".") + " temp =" + "Read" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(ref binaryer, version)" + ";\r\n");
                            strReadCode.Add("\t" + strName + "[tempkey] = temp;\r\n");

                            strWriteCode.Add("\t\t" + "binaryer." + keyWrite + "("+ dbKey + ".Key);\r\n");
                            strWriteCode.Add("\t\t" + "Write" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(" + dbKey + ".Value, " + "ref binaryer)" + ";\r\n");
                        }
                        else
                        {
                            List<string> KeySubRead = new List<string>();
                            List<string> KeySubWrite = new List<string>();
                            BuildSegmentFieldCode(keyType, dbKey + ".Key", ref KeySubRead, ref KeySubWrite);

                            List<string> ValueSubRead = new List<string>();
                            List<string> ValueSubWrite = new List<string>();
                            BuildSegmentFieldCode(datatType, dbKey + ".Value", ref ValueSubRead, ref ValueSubWrite);
                            for (int i = 0; i < KeySubRead.Count; ++i)
                            {
                                string code = strName + "[" + KeySubRead[i].Replace("=", "").Replace(strName + "[i]", "").Replace(dbKey + ".Key", "").Replace(";\r\n", "") + "] = ";
                                code += ValueSubRead[i].Replace("=", "").Replace(strName + "[i]", "").Replace(dbKey + ".Value", "").Replace(";\r\n", "");
                                strReadCode.Add("\t" + code + ";\r\n");
                            }
                            for (int i = 0; i < KeySubWrite.Count; ++i)
                            {
                                strWriteCode.Add("\t\t" + KeySubWrite[i]);
                                strWriteCode.Add("\t\t" + ValueSubWrite[i]);
                            }
                        }

                        strReadCode.Add("}\r\n");

                        strWriteCode.Add("\t}\r\n");
                        strWriteCode.Add("}\r\n");
                    }
                    else
                    {
                        Debug.LogError("Dictionary Key:'" + keyType.FullName+ "' isvalid serailizer");
                    }
                }
            }
            else if (type.IsClass || type.BaseType == typeof(System.ValueType))
            {
                ClassBinaryCode classCode;
                if (!ClassCodeMapping.TryGetValue(type.FullName, out classCode))
                {
                    classCode = new ClassBinaryCode();
                    classCode.classType = type;
                    ClassCodeMapping[classCode.classType.FullName] = classCode;
                    BuildSegmentTypeCode(classCode);
                }
                if (type.IsClass)
                {
                    strWriteCode.Add("binaryer.WriteBool(" + strName + " != null" + ");\r\n");
                    strWriteCode.Add("if(" + strName + " != null" + ")\r\n");
                    strWriteCode.Add("\t" + "Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + ", " + "ref binaryer);\r\n");

                    strReadCode.Add("if(binaryer.ToBool())\r\n");
                    strReadCode.Add("\t" + strName + " = Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(ref binaryer, version);\r\n");
                }
                else
                {
                    strReadCode.Add(strName + " = Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(ref binaryer, version);\r\n");
                    strWriteCode.Add("Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + ", " + "ref binaryer);\r\n");
                }
            }
            else
            {
                Debug.LogError(type.FullName + "   数据类型没序列化!");
            }
        }
        //------------------------------------------------------
        static void BuildSegmentTypeCode(ClassBinaryCode classCode)
        {
            if (classCode.classType.IsClass || classCode.classType.BaseType == typeof(System.ValueType))
            {
                classCode.strReadFunc = "Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_");
                classCode.strWriteFunc = "Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_");
            }
            FieldInfo[] fields = classCode.classType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<int, List<FieldInfo>> vVersions = new Dictionary<int, List<FieldInfo>>();
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsNotSerialized) continue;
                int version = 0;
                if (fields[i].IsDefined(typeof(BinaryFieldVersionAttribute)))
                {
                    version = fields[i].GetCustomAttribute<BinaryFieldVersionAttribute>().version;
                    classCode.verison = Mathf.Max(classCode.verison, version);
                }
                List<FieldInfo> vs;
                if (!vVersions.TryGetValue(version, out vs))
                {
                    vs = new List<FieldInfo>();
                    vVersions[version] = vs;
                }
                vs.Add(fields[i]);
            }

            foreach (var db in vVersions)
            {
                classCode.ServerReadCodes.Add("if(version >=" + db.Key + ")\r\n");
                classCode.ServerReadCodes.Add("{\r\n");
                int startServerR = classCode.ServerReadCodes.Count;
                int startServerW = classCode.ServerWriteCodes.Count;

                classCode.ReadCodes.Add("if(version >=" + db.Key + ")\r\n");
                classCode.ReadCodes.Add("{\r\n");
                int startR = classCode.ReadCodes.Count;
                int startW = classCode.WriteCodes.Count;
                for (int i = 0; i < db.Value.Count; ++i)
                {
                    if (db.Value[i].IsNotSerialized) continue;
                    if (db.Value[i].IsDefined(typeof(BinaryDiscardAttribute))) continue;

                    if (!db.Value[i].IsDefined(typeof(BinaryUnServerAttribute)))
                    {
                        BuildSegmentFieldCode(db.Value[i].FieldType, "pointer." + db.Value[i].Name, ref classCode.ServerReadCodes, ref classCode.ServerWriteCodes);
                    }
                    BuildSegmentFieldCode(db.Value[i].FieldType, "pointer." + db.Value[i].Name, ref classCode.ReadCodes, ref classCode.WriteCodes);
                }
                for (int i = startR; i < classCode.ReadCodes.Count; ++i)
                {
                    classCode.ReadCodes[i] = "\t" + classCode.ReadCodes[i];
                }
                for (int i = startW; i < classCode.WriteCodes.Count; ++i)
                {
                    classCode.WriteCodes[i] = "\t" + classCode.WriteCodes[i];
                }
                classCode.ReadCodes.Add("}\r\n");

                for (int i = startServerR; i < classCode.ServerReadCodes.Count; ++i)
                {
                    classCode.ServerReadCodes[i] = "\t" + classCode.ServerReadCodes[i];
                }
                for (int i = startServerW; i < classCode.ServerWriteCodes.Count; ++i)
                {
                    classCode.ServerWriteCodes[i] = "\t" + classCode.ServerWriteCodes[i];
                }
                classCode.ServerReadCodes.Add("}\r\n");
            }
        }
        //------------------------------------------------------
        static void BuildCode(string strPath, System.Type type)
        {
            BinaryCodeAttribute attr = (BinaryCodeAttribute)type.GetCustomAttribute(typeof(BinaryCodeAttribute));
            if(!string.IsNullOrEmpty(attr.savePath) )
            {
                strPath = Application.dataPath + "/" + attr.savePath;
            }

            string strMarco = null;
            if (type.IsDefined(typeof(BinaryCodeMarcosAttribute), false))
            {
                BinaryCodeMarcosAttribute marcoAttr = (BinaryCodeMarcosAttribute)type.GetCustomAttribute(typeof(BinaryCodeMarcosAttribute));
                strMarco = marcoAttr.marcos;
            }

            List<string> constructorsDec =new List<string>();
            List<string> constructorsCode = new List<string>();
            var constructors = type.GetConstructors( BindingFlags.Public| BindingFlags.Instance);
            if (constructors != null)
            {
                for(int i =0; i < constructors.Length; ++i)
                {
                   ParameterInfo[] parameters =  constructors[i].GetParameters();
                    if (parameters != null && parameters.Length > 0)
                    {
                        string dec = "";
                        string callCode = "";
                        for(int j =0; j < parameters.Length; ++j)
                        {
                            dec += parameters[j].ParameterType.FullName.Replace("+", ".") + " " + parameters[j].Name;
                            callCode += parameters[j].Name;
                            if (j < parameters.Length-1)
                            {
                                dec += ", ";
                                callCode += ", ";
                            }
                        }
                        constructorsDec.Add(dec);
                        constructorsCode.Add(callCode);
                    }
                    else
                    {
                        constructorsDec.Add("");
                        constructorsCode.Add("");
                    }
                }
            }


            ClassBinaryCode clasCode = new ClassBinaryCode();
            clasCode.classType = type;
            clasCode.verison = attr.version;
            ClassCodeMapping[clasCode.classType.FullName] = clasCode;
            BuildSegmentTypeCode(clasCode);

            foreach (var db in ClassCodeMapping)
                clasCode.verison = Mathf.Max(db.Value.verison, clasCode.verison);

            strPath += "/" + type.Name + "BinaryUtil.cs";
            if (File.Exists(strPath))
            {
                File.Delete(strPath);
            }
            string className = type.FullName.Replace("+", ".");

            string code = "//auto generator\r\n";
            if (!string.IsNullOrEmpty(strMarco))
            {
                code += "#if " + strMarco + "\r\n";
            }
            code += "using Framework.Core;\r\n";
            code += "using Framework.Data;\r\n";
            code += "using Framework.Logic;\r\n";
            code += "namespace " + type.Namespace + "\r\n";
            code += "{\r\n";
            code += "\tpublic class " + type.Name + "BinaryUtil \r\n";
            code += "\t{\r\n";

            if(type.IsAbstract)
            {
                code += "\t\tpublic static " + className + " Read("+ className + " pPointer, " + "ref BinaryUtil binaryer)\r\n";
                code += "\t\t{\r\n";
                code += "\t\t\tushort version = binaryer.ToUshort();\r\n\r\n";
                code += "\t\t\treturn " + clasCode.strReadFunc + "(ref binaryer, ref pPointer, version);\r\n";
                code += "\t\t}\r\n";
            }
            else
            {
                if(constructorsDec.Count>0)
                {
                    for(int i =0; i < constructorsDec.Count; ++i)
                    {
                        if (string.IsNullOrEmpty(constructorsDec[i]))
                            code += "\t\tpublic static " + className + " Read(ref BinaryUtil binaryer)\r\n";
                        else
                            code += "\t\tpublic static " + className + " Read(ref BinaryUtil binaryer," + constructorsDec[i] + ")\r\n";
                        code += "\t\t{\r\n";
                        code += "\t\t\tushort version = binaryer.ToUshort();\r\n";
                        code += "\t\t\t" + type.FullName.Replace("+", ".") + " pointer = new " + type.FullName.Replace("+", ".") + "(" + constructorsCode[i] + ");\r\n";
                        code += "\t\t\treturn " + clasCode.strReadFunc + "(ref binaryer, ref pointer, version);\r\n";
                        code += "\t\t}\r\n";
                    }
                }
                else
                {          
                    code += "\t\tpublic static " + className + " Read(ref BinaryUtil binaryer)\r\n";
                    code += "\t\t{\r\n";
                    code += "\t\t\tushort version = binaryer.ToUshort();\r\n";
                    code += "\t\t\t" + type.FullName.Replace("+", ".") + " pointer = new " + type.FullName.Replace("+", ".") + "();\r\n";
                    code += "\t\t\treturn " + clasCode.strReadFunc + "(ref binaryer, ref pointer, version);\r\n";
                    code += "\t\t}\r\n";
                }
            }

            code += "#if UNITY_EDITOR\r\n";
            code += "\t\tpublic static BinaryUtil " + "Write(" + className + " pointer, " + "string strFile)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tBinaryUtil binaryer = new BinaryUtil();\r\n";
            code += "\t\t\tbinaryer.WriteUshort(" + clasCode.verison + ");\r\n\r\n";
            code += "\t\t\t" + clasCode.strWriteFunc + "(pointer, ref binaryer);\r\n";
            code += "\t\t\tif(!string.IsNullOrEmpty(strFile)) binaryer.SaveTo(strFile);\r\n";
            code += "\t\t\treturn binaryer;\r\n";
            code += "\t\t}\r\n";
            code += "#endif\r\n";

            bool bHasServer = false;
            if (type.IsDefined(typeof(BinaryServerCodeAttribute)))
            {
                bHasServer = true;
                code += "#if UNITY_EDITOR\r\n";
                if(clasCode.ServerReadCodes.Count == clasCode.ReadCodes.Count)
                {
                    code += "\t\tpublic static " + className + " ReadServer(ref BinaryUtil binaryer)\r\n";
                    code += "\t\t{\r\n";
                    code += "\t\t\treturn Read(ref binaryer);\r\n";
                    code += "\t\t}\r\n";
                }
                else
                {
                    code += "\t\tpublic static " + className + " ReadServer(ref BinaryUtil binaryer)\r\n";
                    code += "\t\t{\r\n";
                    code += "\t\t\tushort version = binaryer.ToUshort();\r\n\r\n";
                    code += "\t\t\treturn " + clasCode.strReadFunc + "ToServer(ref binaryer, version);\r\n";
                    code += "\t\t}\r\n";
                }

                if (clasCode.ServerWriteCodes.Count == clasCode.WriteCodes.Count)
                {
                    code += "\t\tpublic static void " + "WriteServer(" + className + " pointer, " + "string strFile)\r\n";
                    code += "\t\t{\r\n";
                    code += "\t\t\tWrite(pointer, strFile);\r\n";
                    code += "\t\t}\r\n";
                }
                else
                {
                    code += "\t\tpublic static void " + "WriteServer(" + className + " pointer, " + "string strFile)\r\n";
                    code += "\t\t{\r\n";
                    code += "\t\t\tBinaryUtil binaryer = new BinaryUtil();\r\n";
                    code += "\t\t\tbinaryer.WriteUshort(" + clasCode.verison + ");\r\n\r\n";
                    code += "\t\t\t" + clasCode.strWriteFunc + "ToServer(pointer, ref binaryer);\r\n";
                    code += "\t\t\tbinaryer.SaveTo(strFile);\r\n";
                    code += "\t\t}\r\n";
                }

                code += "#endif\r\n";
            }

            Dictionary<string, string> vServerFuncMapping = new Dictionary<string, string>();
            foreach(var db in ClassCodeMapping)
            {
                if (db.Value.ServerReadCodes.Count != db.Value.ReadCodes.Count)
                {
                    vServerFuncMapping[db.Value.strReadFunc] = db.Value.strReadFunc + "ToServer";
                }
                if (db.Value.ServerWriteCodes.Count != db.Value.WriteCodes.Count)
                {
                    vServerFuncMapping[db.Value.strWriteFunc] = db.Value.strWriteFunc + "ToServer";
                }
            }

            foreach (var db in ClassCodeMapping)
            {
                if (db.Value.classType == type && db.Value.classType.IsAbstract)
                {
                    code += "\t\tstatic " + db.Value.GetFullName() + " " + db.Value.strReadFunc + "(ref BinaryUtil binaryer, " + "ref " + db.Value.GetFullName() + " pointer, ushort version)\r\n";
                    code += "\t\t{\r\n";
                }
                else
                {
                    if (db.Value.classType == type)
                    {
                        code += "\t\tstatic " + db.Value.GetFullName() + " " + db.Value.strReadFunc + "(ref BinaryUtil binaryer, " + "ref " + db.Value.GetFullName() + " pointer, ushort version)\r\n";
                        code += "\t\t{\r\n";
                    }
                    else
                    {
                        code += "\t\tstatic " + db.Value.GetFullName() + " " + db.Value.strReadFunc + "(ref BinaryUtil binaryer, ushort version)\r\n";
                        code += "\t\t{\r\n";
                        code += "\t\t\t" + db.Value.GetFullName() + " pointer = new " + db.Value.GetFullName() + "();\r\n";
                    }
                }

                for (int i = 0; i < db.Value.ReadCodes.Count; ++i)
                {
                    code += "\t\t\t" + db.Value.ReadCodes[i];
                }
                code += "\t\t\treturn pointer;\r\n";
                code += "\t\t}\r\n";

                code += "#if UNITY_EDITOR\r\n";
                code += "\t\tstatic void " + db.Value.strWriteFunc + "(" + db.Value.GetFullName() + " pointer, " + "ref BinaryUtil binaryer)\r\n";
                code += "\t\t{\r\n";
                for (int i = 0; i < db.Value.WriteCodes.Count; ++i)
                {
                    code += "\t\t\t" + db.Value.WriteCodes[i];
                }
                code += "\t\t}\r\n";
                code += "#endif\r\n";

                if(bHasServer)
                {
                    code += "#if UNITY_EDITOR\r\n";
                    if(db.Value.ServerReadCodes.Count != db.Value.ReadCodes.Count)
                    {
                        code += "\t\tstatic " + db.Value.GetFullName() + " " + db.Value.strReadFunc + "ToServer(ref BinaryUtil binaryer, ushort version)\r\n";
                        code += "\t\t{\r\n";
                        code += "\t\t\t" + db.Value.GetFullName() + " pointer = new " + db.Value.GetFullName() + "();\r\n";
                        for (int i = 0; i < db.Value.ServerReadCodes.Count; ++i)
                        {
                            string strCode = db.Value.ServerReadCodes[i];
                            foreach (var sub in vServerFuncMapping)
                                strCode = strCode.Replace(sub.Key, sub.Value);
                            code += "\t\t\t" + strCode;
                        }
                        code += "\t\t\treturn pointer;\r\n";
                        code += "\t\t}\r\n";
                    }

                    if (db.Value.WriteCodes.Count != db.Value.ServerWriteCodes.Count)
                    {
                        code += "\t\tstatic void " + db.Value.strWriteFunc + "ToServer(" + db.Value.GetFullName() + " pointer, " + "ref BinaryUtil binaryer)\r\n";
                        code += "\t\t{\r\n";
                        for (int i = 0; i < db.Value.ServerWriteCodes.Count; ++i)
                        {
                            string strCode = db.Value.ServerWriteCodes[i];
                            foreach (var sub in vServerFuncMapping)
                                strCode = strCode.Replace(sub.Key, sub.Value);
                            code += "\t\t\t" + strCode;
                        }
                        code += "\t\t}\r\n";
                    }

                    code += "#endif\r\n";
                }
            }

            code += "\t}\r\n";
            code += "}\r\n";
            if (!string.IsNullOrEmpty(strMarco))
            {
                code += "#endif\r\n";
            }

            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();
        }
        #endregion
        #region binaryServer
        static string BinaryerToServerType(System.Type type)
        {
            string typeName = null;
            if (type == typeof(bool))
            {
                typeName = "Boolean";
            }
            else if (type == typeof(byte))
            {
                typeName = "byte";
            }
            else if (type.IsEnum)
            {
                typeName = "short";
            }
            else if (type == typeof(char))
            {
                typeName = "char";
            }
            else if (type == typeof(short))
            {
                typeName = "short";
            }
            else if (type == typeof(ushort))
            {
                typeName = "short";
            }
            else if (type == typeof(int))
            {
                typeName = "int";
            }
            else if (type == typeof(uint))
            {
                typeName = "int";
            }
            else if (type == typeof(long))
            {
                typeName = "long";
            }
            else if (type == typeof(ulong))
            {
                typeName = "long";
            }
            else if (type == typeof(float))
            {
                typeName = "float";
            }
            else if (type == typeof(double))
            {
                typeName = "double";
            }
            else if (type == typeof(Vector2))
            {
                typeName = "Vector2";
            }
            else if (type == typeof(Vector3))
            {
                typeName = "Vector3";
            }
            else if (type == typeof(Vector4))
            {
                typeName = "Vector4";
            }
            else if (type == typeof(Vector2Int))
            {
                typeName = "Vector2Int";
            }
            else if (type == typeof(Vector3Int))
            {
                typeName = "Vector3Int";
            }
            else if (type == typeof(Color))
            {
                typeName = "Vector4";
            }
            else if (type == typeof(string))
            {
                typeName = "String";
            }
            else if(!type.IsArray && !type.IsGenericType)
            {
                typeName = type.Name;
            }

            return typeName;
        }
        //------------------------------------------------------
        static void BuildSegmentFieldServerCode(System.Type type, string fieldName, ref List<string> strReadCode, ref List<string> strWriteCode)
        {
            string strName = fieldName;
            string readapi = "";
            string writeapi = "";
            if (BinaryerFunc(type, ref readapi, ref writeapi))
            {
                if (type.IsEnum)
                {
                    strReadCode.Add(strName + " = binaryer.ToUshort();\r\n");
                    strWriteCode.Add("binaryer.WriteUshort((ushort)" + strName + ");\r\n");
                }
                else
                {
                    strReadCode.Add(strName + " = binaryer." + readapi + "();\r\n");
                    strWriteCode.Add("binaryer." + writeapi + "(" + strName + ");\r\n");
                }
            }
            else if (type.IsArray || type.IsGenericType)
            {
                if (type.IsArray)
                {
                    System.Type datatType = type.Assembly.GetType(type.FullName.Replace("[]", ""));
                    string convertType = BinaryerToServerType(datatType);
                    if (convertType == null) convertType = datatType.Name;
                    strWriteCode.Add("binaryer.WriteShort((short)(" + strName + "!=null?" + strName + ".length:0));\r\n");

                    strReadCode.Add("short " + strName.Replace(".", "") + "Cnt = binaryer.ToShort();\r\n");
                    strReadCode.Add("if(" + strName.Replace(".", "") + "Cnt>0) " + strName + " = new " + convertType.Replace("+", ".") + "[" + strName.Replace(".", "") + "Cnt];\r\n");
                    strReadCode.Add("for(short i = 0; i < " + strName.Replace(".", "") + "Cnt; ++i)\r\n");
                    strReadCode.Add("{\r\n");

                    strWriteCode.Add("if(" + strName + "!=null)\r\n");
                    strWriteCode.Add("{\r\n");
                    strWriteCode.Add("\tfor(short i = 0; i < " + strName + ".length; ++i)\r\n");
                    strWriteCode.Add("\t{\r\n");

                    string readapi1 = "";
                    string writeapi1 = "";

                    bool bApiDo = BinaryerFunc(datatType, ref readapi1, ref writeapi1);
                    ClassBinaryCode classCode = null;
                    if (!bApiDo && (datatType.IsClass || datatType.BaseType == typeof(System.ValueType)))
                    {
                        if(!ClassCodeMapping.ContainsKey(datatType.FullName))
                        {
                            classCode = new ClassBinaryCode();
                            classCode.classType = datatType;
                            ClassCodeMapping[classCode.classType.FullName] = classCode;
                            BuildSegmentTypeServerCode(classCode);
                        }
                        strReadCode.Add("\t" + convertType.Replace("+", ".") + " temp = " + "Read" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(binaryer, version)" + ";\r\n");
                        strReadCode.Add("\t" + strName + "[i] = temp;\r\n");

                        strWriteCode.Add("\t\t" + "Write" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + "[i], " + "binaryer)" + ";\r\n");

                    }
                    else
                    {
                        List<string> SubRead = new List<string>();
                        List<string> SubWrite = new List<string>();
                        BuildSegmentFieldServerCode(datatType, strName + "[i]", ref SubRead, ref SubWrite);
                        for (int i = 0; i < SubRead.Count; ++i)
                            strReadCode.Add("\t" + SubRead[i] );
                        for (int i = 0; i < SubWrite.Count; ++i)
                            strWriteCode.Add("\t\t" + SubWrite[i]);
                    }

                    strReadCode.Add("}\r\n");

                    strWriteCode.Add("\t}\r\n");
                    strWriteCode.Add("}\r\n");
                }
                else if (type.Name.Contains("List`1") && type.GenericTypeArguments != null && type.GenericTypeArguments.Length == 1)
                {
                    System.Type datatType = type.GenericTypeArguments[0];
                    string convertType = BinaryerToServerType(datatType);
                    if (convertType == null) convertType = datatType.Name;

                    strWriteCode.Add("binaryer.WriteShort((short)(" + strName + "!=null?" + strName + ".length:0));\r\n");


                    strReadCode.Add("short " + strName.Replace(".", "") + "Cnt = binaryer.ToShort();\r\n");
                    strReadCode.Add("if(" + strName.Replace(".", "") + "Cnt>0)" + strName + " = new " + convertType.Replace("+", ".") + "["+ strName.Replace(".", "") + "Cnt];\r\n");
                    strReadCode.Add("for(short i = 0; i < " + strName.Replace(".", "") + "Cnt; ++i)\r\n");
                    strReadCode.Add("{\r\n");

                    strWriteCode.Add("if(" + strName + "!=null)\r\n");
                    strWriteCode.Add("{\r\n");
                    strWriteCode.Add("\tfor(short i = 0; i < " + strName + ".Count; ++i)\r\n");
                    strWriteCode.Add("\t{\r\n");

                    string readapi1 = "";
                    string writeapi1 = "";

                    bool bApiDo = BinaryerFunc(datatType, ref readapi1, ref writeapi1);
                    ClassBinaryCode classCode = null;
                    if (!bApiDo && (datatType.IsClass || datatType.BaseType == typeof(System.ValueType)))
                    {
                        if (!ClassCodeMapping.ContainsKey(datatType.FullName))
                        {
                            classCode = new ClassBinaryCode();
                            classCode.classType = datatType;
                            ClassCodeMapping[classCode.classType.FullName] = classCode;
                            BuildSegmentTypeServerCode(classCode);
                        }
                        strReadCode.Add("\t" + convertType.Replace("+", ".") + " temp =" + "Read" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(binaryer, version)" + ";\r\n");
                        strReadCode.Add("\t" + strName + "[i] = temp;\r\n");

                        strWriteCode.Add("\t\t" + "Write" + datatType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + "[i], " + "binaryer)" + ";\r\n");
                    }
                    else
                    {
                        List<string> SubRead = new List<string>();
                        List<string> SubWrite = new List<string>();
                        BuildSegmentFieldServerCode(datatType, strName + "[i]", ref SubRead, ref SubWrite);
                        for (int i = 0; i < SubRead.Count; ++i)
                        {
                            strReadCode.Add("\t" + strName + "[i]=" + SubRead[i].Replace("=", "").Replace(strName + "[i]", "").Replace(";\r\n", "") + ";\r\n");
                        }
                        for (int i = 0; i < SubWrite.Count; ++i)
                            strWriteCode.Add("\t\t" + SubWrite[i]);
                    }

                    strReadCode.Add("}\r\n");

                    strWriteCode.Add("\t}\r\n");
                    strWriteCode.Add("}\r\n");
                }
            }
            else if (type.IsClass || type.BaseType == typeof(System.ValueType))
            {
                ClassBinaryCode classCode;
                if (!ClassCodeMapping.TryGetValue(type.FullName, out classCode))
                {
                    classCode = new ClassBinaryCode();
                    classCode.classType = type;
                    ClassCodeMapping[classCode.classType.FullName] = classCode;
                    BuildSegmentTypeServerCode(classCode);
                }
                if (type.IsClass)
                {
                    strWriteCode.Add("binaryer.WriteBool(" + strName + " != null" + ");\r\n");
                    strWriteCode.Add("if(" + strName + " != null" + ")\r\n");
                    strWriteCode.Add("\t" + "Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + ", " + "binaryer);\r\n");

                    strReadCode.Add("if(binaryer.ToBool())\r\n");
                    strReadCode.Add("\t" + strName + " = Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(binaryer, version);\r\n");
                }
                else
                {
                    strReadCode.Add(strName + " = Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(binaryer, version);\r\n");
                    strWriteCode.Add("Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_") + "(" + strName + ", " + "binaryer);\r\n");
                }
            }
            else
            {
                Debug.LogError(type.FullName + "   数据类型没序列化!");
            }
        }
        //------------------------------------------------------
        static void BuildSegmentTypeServerCode(ClassBinaryCode classCode)
        {
            if (classCode.classType.IsClass || classCode.classType.BaseType == typeof(System.ValueType))
            {
                classCode.strReadFunc = "Read" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_");
                classCode.strWriteFunc = "Write" + classCode.classType.FullName.Replace("+", "_").Replace(".", "_");
            }

            FieldInfo[] fields = classCode.classType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<int, List<FieldInfo>> vVersions = new Dictionary<int, List<FieldInfo>>();
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsNotSerialized) continue;
                int version = 0;
                if (fields[i].IsDefined(typeof(BinaryFieldVersionAttribute)))
                {
                    version = fields[i].GetCustomAttribute<BinaryFieldVersionAttribute>().version;
                }
                List<FieldInfo> vs;
                if (!vVersions.TryGetValue(version, out vs))
                {
                    vs = new List<FieldInfo>();
                    vVersions[version] = vs;
                }
                vs.Add(fields[i]);
            }

            foreach (var db in vVersions)
            {
                classCode.ReadCodes.Add("if(version >=" + db.Key + ")\r\n");
                classCode.ReadCodes.Add("{\r\n");
                int startR = classCode.ReadCodes.Count;
                int startW = classCode.WriteCodes.Count;
                for (int i = 0; i < db.Value.Count; ++i)
                {
                    if (db.Value[i].IsNotSerialized) continue;
                    if (db.Value[i].IsDefined(typeof(BinaryUnServerAttribute))) continue;
                    if (db.Value[i].IsDefined(typeof(BinaryDiscardAttribute))) continue;
                    BuildSegmentFieldServerCode(db.Value[i].FieldType, "pointer." + db.Value[i].Name, ref classCode.ReadCodes, ref classCode.WriteCodes);
                }
                for (int i = startR; i < classCode.ReadCodes.Count; ++i)
                {
                    classCode.ReadCodes[i] = "\t" + classCode.ReadCodes[i];
                }
                for (int i = startW; i < classCode.WriteCodes.Count; ++i)
                {
                    classCode.WriteCodes[i] = "\t" + classCode.WriteCodes[i];
                }
                classCode.ReadCodes.Add("}\r\n");
            }
        }
        //------------------------------------------------------
        static void BuildServerCode(string strPath, System.Type type)
        {
            BinaryCodeAttribute attr = (BinaryCodeAttribute)type.GetCustomAttribute(typeof(BinaryCodeAttribute));
            BinaryServerCodeAttribute serverAttr = (BinaryServerCodeAttribute)type.GetCustomAttribute(typeof(BinaryServerCodeAttribute));
            if (serverAttr!=null && !string.IsNullOrEmpty(serverAttr.savePath))
            {
                strPath = Application.dataPath + "/" +attr.savePath;
            }

            ClassBinaryCode clasCode = new ClassBinaryCode();
            clasCode.classType = type;
            clasCode.verison = attr.version;
            ClassCodeMapping[clasCode.classType.FullName] = clasCode;
            BuildSegmentTypeServerCode(clasCode);

            strPath += "/" + type.Name + ".java";
            if (File.Exists(strPath))
            {
                File.Delete(strPath);
            }
            string className = type.Name.Replace("+", ".");


            string code = "//auto generator\r\n";
            code += "package com.topgame.common.generate_binary_config;\r\n";
            code += "import com.topgame.common.config.*;\r\n";
            code += "import com.topgame.common.constant.config.*;\r\n";
            code += "import java.util.*;\r\n";

            code += "public class " + type.Name + " implements IData{\r\n";

            List<string> vSubCodes = new List<string>();
            bool bDeclare = true;
            foreach (var db in ClassCodeMapping)
            {
                string classCode = "";
                string strSp = "";
                if (db.Key != type.FullName)
                {
                    strSp += "\t";
                    classCode += "\tpublic class " + db.Value.classType.Name + "{\r\n";
                }
                FieldInfo[] fields = db.Value.classType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; ++i)
                {
                    if (!bDeclare) break;
                    if (fields[i].IsDefined(typeof(BinaryUnServerAttribute))) continue;
                    if (fields[i].IsNotSerialized) continue;
                    string convertType = BinaryerToServerType(fields[i].FieldType);
                    if(!string.IsNullOrEmpty(convertType))
                    {
                        classCode += strSp + "\tpublic " + convertType + " " + fields[i].Name + ";\r\n";
                        continue;
                    }

                    if (fields[i].FieldType.IsArray || fields[i].FieldType.IsGenericType)
                     { 
                        if (fields[i].FieldType.IsArray)
                        {
                            Type datatType = fields[i].FieldType.Assembly.GetType(fields[i].FieldType.FullName.Replace("[]", ""));
                            convertType = BinaryerToServerType(datatType);
                            if(string.IsNullOrEmpty(convertType))
                            {
                                if(ClassCodeMapping.ContainsKey(datatType.FullName))
                                {
                                    classCode += strSp + "\tpublic " + datatType.Name + "[] " + fields[i].Name + ";\r\n";
                                }
                                else
                                {
                                    Debug.LogError(fields[i].Name + "生成失败");
                                    bDeclare = false;
                                    break;
                                }
                            }
                            else
                            {
                                classCode += strSp + "\tpublic " + convertType + "[] " + fields[i].Name + ";\r\n";
                            }
                        }
                        else if (fields[i].FieldType.Name.Contains("List`1") && fields[i].FieldType.GenericTypeArguments != null && fields[i].FieldType.GenericTypeArguments.Length == 1)
                        {
                            System.Type datatType = fields[i].FieldType.GenericTypeArguments[0];
                            convertType = BinaryerToServerType(datatType);
                            if (string.IsNullOrEmpty(convertType))
                            {
                                if (ClassCodeMapping.ContainsKey(datatType.FullName))
                                {
                                    classCode += strSp + "\tpublic " + datatType.Name + "[] " + fields[i].Name + ";\r\n";
                                }
                                else
                                {
                                    Debug.LogError(fields[i].Name + "生成失败");
                                    bDeclare = false;
                                    break;
                                }
                            }
                            else
                            {
                                classCode += strSp + "\tpublic " + convertType + "[] " + fields[i].Name + ";\r\n";
                            }
                        }
                        else
                        {
                            Debug.LogError(fields[i].Name + "生成失败");
                        }
                    }
                    else
                    {
                        Debug.LogError(fields[i].Name + "生成失败");
                    }
                }
                if (db.Key != type.FullName)
                    classCode += "}\r\n";

                if (db.Key == type.FullName)
                {
                    code += classCode;
                    code += "\r\n";
                    continue;
                }
                else
                    vSubCodes.Add(classCode);
            }
            if (!bDeclare)
                return;

            foreach(var db in vSubCodes)
                code += db;

            code += "\tpublic void Read(BinaryUtil binaryer)\r\n";
            code += "\t{\r\n";
            code += "\t\tshort version = binaryer.ToUshort();\r\n\r\n";
            code += "\t\t" + clasCode.strReadFunc + "(binaryer, version);\r\n";
            code += "\t}\r\n";

            //             code += "\tpublic void " + "Write(" + className + " pointer, " + "string strFile)\r\n";
            //             code += "\t{\r\n";
            //             code += "\t\tBinaryUtil binaryer = new BinaryUtil();\r\n";
            //             code += "\t\tbinaryer.WriteUshort(" + attr.version + ");\r\n\r\n";
            //             code += "\t\t" + clasCode.strWriteFunc + "(pointer, ref binaryer);\r\n";
            //             code += "\t\tbinaryer.SaveTo(strFile);\r\n";
            //             code += "\t}\r\n";

            foreach (var db in ClassCodeMapping)
            {
                if(db.Key == type.FullName)
                {
                    code += "\t" + "void " + db.Value.strReadFunc + "(BinaryUtil binaryer, short version)\r\n";
                    code += "\t{\r\n";
                    code += "\t\t" + db.Value.classType.Name + " pointer = this;\r\n";
                    for (int i = 0; i < db.Value.ReadCodes.Count; ++i)
                    {
                        code += "\t\t" + db.Value.ReadCodes[i];
                    }
                    code += "\t}\r\n";
                }
                else
                {
                    code += "\t" + db.Value.classType.Name + " " + db.Value.strReadFunc + "(BinaryUtil binaryer, short version)\r\n";
                    code += "\t{\r\n";
                    code += "\t\t" + db.Value.classType.Name + " pointer = new " + db.Value.classType.Name + "();\r\n";
                    for (int i = 0; i < db.Value.ReadCodes.Count; ++i)
                    {
                        code += "\t\t" + db.Value.ReadCodes[i];
                    }
                    code += "\t\treturn pointer;\r\n";
                    code += "\t}\r\n";
                }


                //                 code += "\t void " + db.Value.strWriteFunc + "(" + db.Value.GetFullName() + " pointer, " + "BinaryUtil binaryer)\r\n";
                //                 code += "\t{\r\n";
                //                 for (int i = 0; i < db.Value.WriteCodes.Count; ++i)
                //                 {
                //                     code += "\t\t" + db.Value.WriteCodes[i];
                //                 }
                //                 code += "\t}\r\n";
            }

            code += "}\r\n";

            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();
        }
        #endregion
    }
}
#endif