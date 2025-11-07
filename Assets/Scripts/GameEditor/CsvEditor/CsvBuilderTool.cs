/********************************************************************
生成日期:	24:7:2019   11:12
类    名: 	CsvBuilderTool
作    者:	HappLI
描    述:	Csv 解析代码自动生成器
*********************************************************************/
using Framework.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.Data;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;
using Framework.ED;

namespace TopGame.ED
{
    public class CsvBuilderTool
    {
        private static string CPP_CODE_SVR_PROJECT_CSV_PATH = "/../../Server/work_spaces/BattleServer/";
        private static string CPP_CODE_PROJECT_CSV_PATH = "/../GamePlus/CorePlus/Source/Data/generator/";
        private static string[] SPEC_CSV_TML = new string[] { "projectile" };
        //------------------------------------------------------
        static string ms_strEditorPath = null;
        static string InitEditorPath()
        {
            if (ms_strEditorPath == null)
            {
                var scripts = AssetDatabase.FindAssets("t:Script CsvBuilderTool");
                if (scripts.Length > 0)
                {
                    ms_strEditorPath = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0])).Replace("\\", "/");
                    if (ms_strEditorPath.StartsWith("Assets/"))
                        ms_strEditorPath = ms_strEditorPath.Substring("Assets/".Length);
                }
            }
            return ms_strEditorPath;
        }
        //------------------------------------------------------
        class MappCodeTables
        {
            public string strCode;
            public List<string> tables = new List<string>();
        }
        struct MappData
        {
            public string PropName;
            public string ClassName;
            public string ClassType;
            public bool bArray;

            public string strMapFunc;
            public string strToMapField;
        }

        struct ClassSimply
        {
            public string className;
            public bool enabelAT;
        }
        struct SBinderData
        {
            public System.Type BindType;
            public System.Type BindFieldDataType;
            public List<FieldMapTableAttribute> FieldMapTable;
            public string MainKeyType;
            public string MainKeyName;
            public string DataFieldName;
        }
        struct sCppEnum
        {
            public string strClassName;
            public string strType;
            public string strHash;
        }
        struct ClassMappingPropety
        {
            public string className;
            public string propertyClassName;
            public string propertyKeyTypeName;
        }
        //------------------------------------------------------
        //     [MenuItem("Tools/Csv/表格转化CSV")]
        public static void BuildXlsxBuildCsv()
        {
            string exepath = Application.dataPath + "/../Tools/DataConverter/DataConverter.exe";

            string xlsx = Application.dataPath + "/../Tools/DataConverter/data/";
            string csv = Application.dataPath + "/DatasRef/Config/Csv/";
            int windowDeltyClose = 1000;
            string cmd = exepath + string.Format(" {0} {1} {2} {3}", xlsx, csv, "Csv", windowDeltyClose);

            System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CmdThread));
            newThread.Start(cmd);

            // BuildCode();
        }
        //------------------------------------------------------
        //   [MenuItem("Tools/Csv/表格转化二进制")]
        public static void BuildXlsxBuildBinary()
        {
            string exepath = Application.dataPath + "/../Tools/DataConverter/DataConverter.exe";

            string xlsx = Application.dataPath + "/../Tools/DataConverter/data/";
            string csv = Application.dataPath + "/DatasRef/Config/Csv/";
            int windowDeltyClose = 1000;
            string cmd = exepath + string.Format(" {0} {1} {2} {3}", xlsx, csv, "Binary", windowDeltyClose);

            System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CmdThread));
            newThread.Start(cmd);

            // BuildCode();
        }
        //-----------------------------------------------------
        private static void CmdThread(object obj)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c " + obj.ToString();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            p.Close();
        }
        //------------------------------------------------------
        class TmlData
        {
            public class Code
            {
                public string label;
                public List<string> vLine = new List<string>();
            }
            public Dictionary<string, Code> vCodes = new Dictionary<string, Code>();

            public List<string> GetCode(string label)
            {
                Code vod;
                if (vCodes.TryGetValue(label, out vod)) return vod.vLine;
                return null;
            }
        }
        static List<string> GetTmlCode(Dictionary<string, TmlData> vMaps, string strTml, string label)
        {
            TmlData tml;
            if (vMaps.TryGetValue(strTml, out tml))
            {
                return tml.GetCode(label);
            }
            return null;
        }
        class ClassDatas
        {
            public int hash;
            public bool maplist;
        }
        static Dictionary<string, ClassMappingPropety> ms_vClassMappingPropertys = new Dictionary<string, ClassMappingPropety>();
        //------------------------------------------------------
        public static void BuildBattleServerColde(string serverBinPath, bool bBuildCode = true, bool bCopyCsv = false)
        {
            HashSet<string> vServerTables = ParseServerTableConfig();
            if (bBuildCode) BuildCode(vServerTables, true, CPP_CODE_SVR_PROJECT_CSV_PATH);

            //copy csvs 
            //     if(bCopyCsv)
            {
                string[] assets = AssetDatabase.FindAssets("t:CsvConfig");
                if (assets == null || assets.Length <= 0) return;
                CsvConfig csvCfg = AssetDatabase.LoadAssetAtPath<CsvConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                if (csvCfg)
                {
                    string strCreateFileMapping = serverBinPath + "sos/CsvConfig.xml";
                    string strCode = "";
                    strCode += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
                    strCode += "<CsvConfig>\r\n";

                    string destSvrDir = serverBinPath + "csv/";
                    string destSvrJsonDir = serverBinPath + "json/";
                    if (bCopyCsv)
                    {
                        if (!Directory.Exists(destSvrDir))
                            Directory.CreateDirectory(destSvrDir);
                        if (!Directory.Exists(destSvrJsonDir))
                            Directory.CreateDirectory(destSvrJsonDir);
                        for (int i = 0; i < csvCfg.Assets.Length; ++i)
                        {
                            if (csvCfg.Assets[i].Asset == null) continue;
                            if (vServerTables.Contains(csvCfg.Assets[i].Asset.name.ToLower()))
                            {
                                string file = AssetDatabase.GetAssetPath(csvCfg.Assets[i].Asset);
                                string fileName = Path.GetFileName(file);
                                string strSrc = Application.dataPath.Replace("/Assets", "/") + file;

                                string strTo = "";
                                if (file.EndsWith("json")) strTo = destSvrJsonDir + fileName;
                                else strTo = destSvrDir + fileName;
                                File.Copy(strSrc, strTo, true);
                            }
                        }
                    }

                    for (int i = 0; i < csvCfg.Assets.Length; ++i)
                    {
                        if (csvCfg.Assets[i].Asset == null) continue;
                        if (vServerTables.Contains(csvCfg.Assets[i].Asset.name.ToLower()))
                        {
                            string file = AssetDatabase.GetAssetPath(csvCfg.Assets[i].Asset);
                            string fileName = Path.GetFileName(file);
                            if (file.EndsWith("json"))
                                strCode += "\t<item id= \"" + csvCfg.Assets[i].nHash + "\" type=\"" + (int)(csvCfg.Assets[i].type) + "\" file=\"json/" + fileName + "\"/>\r\n";
                            else
                                strCode += "\t<item id= \"" + csvCfg.Assets[i].nHash + "\" type=\"" + (int)(csvCfg.Assets[i].type) + "\" file=\"csv/" + fileName + "\"/>\r\n";
                        }
                    }
                    strCode += "</CsvConfig>\r\n";
                    FileStream fs = new FileStream(strCreateFileMapping, FileMode.OpenOrCreate);
                    StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    fs.SetLength(0);
                    fs.Position = 0;
                    writer.Write(strCode);
                    writer.Close();
                }
            }

            // 
            //             //! copy binary data
            //             {
            //                 string strCreateFileMapping = Application.dataPath + CPP_CODE_SVR_PROJECT_CSV_PATH + "Config/Binarys/";
            //                 if (Directory.Exists(strCreateFileMapping))
            //                     Directory.Delete(strCreateFileMapping, true);
            //                 Directory.CreateDirectory(strCreateFileMapping);
            //                 ED.EditorKits.CopyDir(Application.dataPath + "/../Binarys", strCreateFileMapping);
            //             }
        }
        //------------------------------------------------------
        public static void BuildCode(HashSet<string> vContainTables = null, bool bServerCode = false, string rootDir = "Scripts/GameMain/Generators/GameDatas")
        {
            string[] KEYS = new string[] { "DATE_TIME", "CLASS_NAME", "DATA_KEY", "PROP_FIELD", "PARSE_FIELD" };
            EditorUtility.DisplayProgressBar("代码生成", "Csv配置表解析代码生成", 0);
            string[] assets = AssetDatabase.FindAssets("t:CsvConfig");
            if (assets == null || assets.Length <= 0)
            {
                EditorUtility.DisplayDialog("提示", "没有转译的csv 文件", "ok");
                EditorUtility.ClearProgressBar();
                return;
            }

            ms_vClassMappingPropertys.Clear();
            Dictionary<string, ClassDatas> vMap = new Dictionary<string, ClassDatas>();
            Dictionary<string, TmlData> vTmlMaps = new Dictionary<string, TmlData>();
            List<sCppEnum> vCppDatatTypeEnum = new List<sCppEnum>();

            //! tml-csv
            {
                TmlData csvTml = new TmlData();
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "default_c#";
                    ParseTml(ref code.vLine, "CsvData_Tml");
                    csvTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "list_c#";
                    if (ParseTml(ref code.vLine, "CsvData_Tml_List"))
                        csvTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "maplist_c#";
                    if (ParseTml(ref code.vLine, "CsvData_Tml_MapList"))
                        csvTml.vCodes.Add(code.label, code);
                }
                //cpp-default
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "default_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_Tml_h"))
                            csvTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "default_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_Tml_cpp"))
                            csvTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }
                //cpp-maplist
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "maplist_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_Tml_h_MapList"))
                            csvTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "maplist_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_Tml_cpp_MapList"))
                            csvTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }

                vTmlMaps.Add("CsvData_Tml", csvTml);
            }
            //! binary tml
            {
                TmlData binaryTml = new TmlData();
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "default_c#";
                    ParseTml(ref code.vLine, "CsvData_BinaryTml");
                    binaryTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "list_c#";
                    if (ParseTml(ref code.vLine, "CsvData_BinaryTml_List"))
                        binaryTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "maplist_c#";
                    if (ParseTml(ref code.vLine, "CsvData_BinaryTml_MapList"))
                        binaryTml.vCodes.Add(code.label, code);
                }

                //cpp-default
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "default_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_BinaryTml_h"))
                            binaryTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "default_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_BinaryTml_cpp"))
                            binaryTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }
                //cpp-maplist
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "maplist_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_BinaryTml_h_MapList"))
                            binaryTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "maplist_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_BinaryTml_cpp_MapList"))
                            binaryTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }

                vTmlMaps.Add("CsvData_BinaryTml", binaryTml);
            }
            //! json tml
            {
                TmlData jsonTml = new TmlData();
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "default_c#";
                    ParseTml(ref code.vLine, "CsvData_JsonTml");
                    jsonTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "list_c#";
                    if (ParseTml(ref code.vLine, "CsvData_JsonTml_List"))
                        jsonTml.vCodes.Add(code.label, code);
                }
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "maplist_c#";
                    if (ParseTml(ref code.vLine, "CsvData_JsonTml_MapList"))
                        jsonTml.vCodes.Add(code.label, code);
                }

                //cpp-default
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "default_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_JsonTml_h"))
                            jsonTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "default_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_JsonTml_cpp"))
                            jsonTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }
                //cpp-maplist
                {
                    TmlData.Code code_h = new TmlData.Code();
                    TmlData.Code code_cpp = new TmlData.Code();
                    {
                        code_h.label = "maplist_c++_h";
                        if (ParseTml(ref code_h.vLine, "cpp/CsvData_JsonTml_h_MapList"))
                            jsonTml.vCodes.Add(code_h.label, code_h);

                        code_cpp.label = "maplist_c++_cpp";
                        if (ParseTml(ref code_cpp.vLine, "cpp/CsvData_JsonTml_cpp_MapList"))
                            jsonTml.vCodes.Add(code_cpp.label, code_cpp);
                    }
                }
                vTmlMaps.Add("CsvData_JsonTml", jsonTml);
            }

            for (int j = 0; j < SPEC_CSV_TML.Length; ++j)
            {
                TmlData specTml = new TmlData();
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "default_c#";
                    if (ParseTml(ref code.vLine, SPEC_CSV_TML[j] + "Tml"))
                        specTml.vCodes.Add(code.label, code);
                }
                vTmlMaps.Add(SPEC_CSV_TML[j] + "Tml", specTml);

                TmlData specBinaryTml = new TmlData();
                {
                    TmlData.Code code = new TmlData.Code();
                    code.label = "default_c#";
                    if (ParseTml(ref code.vLine, SPEC_CSV_TML[j] + "BinaryTml"))
                        specBinaryTml.vCodes.Add(code.label, code);
                }
                vTmlMaps.Add(SPEC_CSV_TML[j] + "Binary", specBinaryTml);
            }

            Dictionary<string, SBinderData> vBindTypes = new Dictionary<string, SBinderData>();
            Assembly assembly = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
             //   if (ass.GetName().Name == "MainScripts" || ass.GetName().Name.Contains("FrameworkPlus"))
                {
                    assembly = ass;
                    System.Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        System.Type tp = types[i];
                        if (tp.IsDefined(typeof(DataBinderTypeAttribute), false))
                        {
                            DataBinderTypeAttribute attr = (DataBinderTypeAttribute)tp.GetCustomAttribute(typeof(DataBinderTypeAttribute));
                            if (attr.strConfigName.Length > 0)
                            {
                                if (vContainTables != null && !vContainTables.Contains(attr.strConfigName.ToLower()))
                                    continue;

                                if (vBindTypes.ContainsKey(attr.strConfigName.ToLower()))
                                {
                                    Debug.LogError("重复绑定表名：" + tp.FullName);
                                }
                                SBinderData binder = new SBinderData();
                                binder.BindType = tp;
                                System.Reflection.FieldInfo fields = binder.BindType.GetField(attr.DataField, BindingFlags.Public | BindingFlags.Instance);
                                if (fields == null) continue;
                                binder.BindFieldDataType = fields.FieldType;
                                binder.MainKeyName = attr.strMainKeyField;
                                binder.MainKeyType = attr.strMainKeyType;
                                binder.DataFieldName = attr.DataField;
                                FieldMapTableAttribute[] attris = (FieldMapTableAttribute[])tp.GetCustomAttributes(typeof(FieldMapTableAttribute));
                                if (attris != null && attris.Length > 0)
                                {
                                    binder.FieldMapTable = new List<FieldMapTableAttribute>(attris);
                                }
                                else
                                    binder.FieldMapTable = null;
                                vBindTypes[attr.strConfigName.ToLower()] = binder;

                                FieldInfo propertyDataField = tp.GetField(attr.DataField, BindingFlags.Public | BindingFlags.Instance);
                                if (propertyDataField != null && propertyDataField.FieldType.IsArray)
                                {
                                    ClassMappingPropety classMapPropety = new ClassMappingPropety();
                                    classMapPropety.className = tp.FullName;
                                    classMapPropety.propertyClassName = propertyDataField.FieldType.FullName.Replace("[]", "");
                                    classMapPropety.propertyKeyTypeName = attr.strMainKeyType;
                                    ms_vClassMappingPropertys[attr.strConfigName.ToLower()] = classMapPropety;
                                }

                            }
                        }
                    }
                }
            }
            System.DateTime nowTime = System.DateTime.Now;
            //      for (int a = 0; a < assets.Length; ++a)
            {
                CsvConfig csvCfg = AssetDatabase.LoadAssetAtPath<CsvConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                if (csvCfg)
                {
                    for (int i = 0; i < csvCfg.Assets.Length; ++i)
                    {
                        if (csvCfg.Assets[i].type == EDataType.None)
                            continue;
                        string class_name = csvCfg.Assets[i].Asset.name;
                        if (class_name.Length <= 0)
                        {
                            EditorUtility.DisplayDialog("提示", "请输入一个有效名字", "好的");
                            return;
                        }
                        if (vContainTables != null && !vContainTables.Contains(class_name.ToLower()))
                            continue;
                        class_name = class_name.Substring(0, 1).ToUpper() + class_name.Substring(1, class_name.Length - 1);
                        if (vMap.ContainsKey(class_name.ToLower()))
                        {
                            EditorUtility.DisplayDialog("提示", "相同的文件配置[" + csvCfg.Assets[i].Asset.name + "]", "请确认");
                            return;
                        }
                        string tmlHead = "";
                        if (csvCfg.Assets[i].type == EDataType.Binary) tmlHead = ParseHeadTmlLabel(csvCfg.Assets[i].Asset.bytes);
                        else if (csvCfg.Assets[i].type == EDataType.Csv)
                        {
                            CsvParser csv = new CsvParser();
                            csv.SetName(AssetDatabase.GetAssetPath(csvCfg.Assets[i].Asset));
                            string textData = System.Text.Encoding.UTF8.GetString(csvCfg.Assets[i].Asset.bytes);
                            if (csv.LoadTableString(textData))
                            {
                                tmlHead = ParseHeadTmlLabel(csv);
                            }
                        }
                        vMap.Add(class_name.ToLower(), new ClassDatas() { hash = csvCfg.Assets[i].nHash, maplist = tmlHead.CompareTo("maplist") == 0 });
                    }
                }
            }
            //    for (int a = 0; a < assets.Length; ++a)
            {
                CsvConfig csvCfg = AssetDatabase.LoadAssetAtPath<CsvConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                if (csvCfg)
                {
                    bool bBinary = csvCfg.bBinary;
                    string srcDir = Application.dataPath + "/../Temp/CsvCodeTemp/";
                    if (Directory.Exists(srcDir))
                        Directory.Delete(srcDir, true);

                    bool bBuildOk = true;
                    #region 解析每张表
                    string date_time = string.Format("{0}:{1}:{2}   {3}:{4}", nowTime.Day, nowTime.Month, nowTime.Year, nowTime.Hour, nowTime.Minute);

                    Dictionary<string, bool> vEnableAT = new Dictionary<string, bool>();
                    Dictionary<string, List<MappData>> vMapping = new Dictionary<string, List<MappData>>();
                    List<string> vNewCodeLines = new List<string>();
                    int total = csvCfg.Assets.Length;
                    for (int i = 0; i < csvCfg.Assets.Length; ++i)
                    {
                        string class_name = csvCfg.Assets[i].Asset.name;
                        if (vContainTables != null && !vContainTables.Contains(class_name.ToLower()))
                            continue;
                        class_name = class_name.Substring(0, 1).ToUpper() + class_name.Substring(1, class_name.Length - 1);

                        vEnableAT["CsvData_" + class_name] = csvCfg.Assets[i].exportAT;

                        if (csvCfg.Assets[i].type == EDataType.Binary)
                        {
                            vNewCodeLines = new List<string>();

                            bool bMapping = true;

                            string strTmlHead = ParseHeadTmlLabel(csvCfg.Assets[i].Asset.bytes);

                            List<string> vDefaultLines = GetTmlCode(vTmlMaps, class_name.ToLower() + "BinaryTml", strTmlHead + "_c#");
                            if (vDefaultLines != null) bMapping = false;
                            if (vDefaultLines == null)
                                vDefaultLines = GetTmlCode(vTmlMaps, "CsvData_BinaryTml", strTmlHead + "_c#");
                            if (vDefaultLines == null)
                            {
                                EditorUtility.DisplayDialog("提示", "CsvData_BinaryTml[" + strTmlHead + "_c#]模板丢失！！！", "好的");
                                bBuildOk = false;
                                break;
                            }
                            for (int j = 0; j < vDefaultLines.Count; ++j)
                                vNewCodeLines.Add(vDefaultLines[j] + "\r\n");

                            if (csvCfg.Assets[i].csharpParse)
                            {
                                int ret = BuildCodeByBinary(csvCfg.Assets[i].Asset.bytes, vNewCodeLines, date_time, class_name, bMapping ? vMap : null, vMapping, csvCfg.Assets[i].exportAT);
                                if (ret != 1)
                                {
                                    bBuildOk = ret != 0;
                                    break;
                                }
                                else if (ret == 1)
                                {
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/CsvData_" + class_name + ".cs", vNewCodeLines);
                                }
                            }

                        }
                        else if (csvCfg.Assets[i].type == EDataType.Json)
                        {
                            if (csvCfg.Assets[i].csharpParse)
                            {
                                SBinderData ouBinder;
                                if (vBindTypes.TryGetValue(csvCfg.Assets[i].Asset.name.ToLower(), out ouBinder))
                                {
                                    vNewCodeLines = new List<string>();

                                    List<string> vDefaultLines = GetTmlCode(vTmlMaps, "CsvData_JsonTml", "default_c#");
                                    if (vDefaultLines == null)
                                    {
                                        EditorUtility.DisplayDialog("提示", "CsvData_JsonTml[default_c#]模板丢失！！！", "好的");
                                        bBuildOk = false;
                                        break;
                                    }
                                    for (int j = 0; j < vDefaultLines.Count; ++j)
                                        vNewCodeLines.Add(vDefaultLines[j] + "\r\n");
                                    int ret = BuildCodeByJson(ouBinder, vNewCodeLines, date_time, class_name, vMap, vMapping, csvCfg.Assets[i].exportAT);
                                    if (ret != 1)
                                    {
                                        bBuildOk = ret != 0;
                                        break;
                                    }
                                    else if (ret == 1)
                                    {
                                        BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/CsvData_" + class_name + ".cs", vNewCodeLines);
                                    }
                                }
                            }
                            if (csvCfg.Assets[i].dllParse)
                            {
                                SBinderData ouBinder;
                                if (vBindTypes.TryGetValue(csvCfg.Assets[i].Asset.name.ToLower(), out ouBinder))
                                {
                                    vNewCodeLines = new List<string>();

                                    List<string> vDefaultLines = GetTmlCode(vTmlMaps, "CsvData_JsonTml", "default_c++_h");
                                    if (vDefaultLines == null)
                                    {
                                        EditorUtility.DisplayDialog("提示", "CsvData_JsonTml[default_c++_h]模板丢失！！！", "好的");
                                        bBuildOk = false;
                                        break;
                                    }
                                    for (int j = 0; j < vDefaultLines.Count; ++j)
                                        vNewCodeLines.Add(vDefaultLines[j] + "\r\n");
                                    int ret = BuildCodeCppByJson(ouBinder, vNewCodeLines, date_time, class_name, true);
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/cpp/CsvData_" + class_name + ".h", vNewCodeLines);

                                    vNewCodeLines.Clear();
                                    vDefaultLines = GetTmlCode(vTmlMaps, "CsvData_JsonTml", "default_c++_cpp");
                                    if (vDefaultLines == null)
                                    {
                                        EditorUtility.DisplayDialog("提示", "CsvData_JsonTml[default_c++_cpp]模板丢失！！！", "好的");
                                        bBuildOk = false;
                                        break;
                                    }
                                    for (int j = 0; j < vDefaultLines.Count; ++j)
                                        vNewCodeLines.Add(vDefaultLines[j] + "\r\n");
                                    int ret_cpp = BuildCodeCppByJson(ouBinder, vNewCodeLines, date_time, class_name);
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/cpp/CsvData_" + class_name + ".cpp", vNewCodeLines);

                                    sCppEnum enumC = new sCppEnum();
                                    enumC.strClassName = "CsvData_" + class_name;
                                    enumC.strType = "DataType_" + class_name;
                                    enumC.strHash = csvCfg.Assets[i].nHash.ToString();
                                    vCppDatatTypeEnum.Add(enumC);
                                }
                            }
                        }
                        else if (csvCfg.Assets[i].type == EDataType.Csv)
                        {
                            CsvParser csv = new CsvParser();
                            csv.SetName(AssetDatabase.GetAssetPath(csvCfg.Assets[i].Asset));
                            string textData = System.Text.Encoding.UTF8.GetString(csvCfg.Assets[i].Asset.bytes);
                            if (!csv.LoadTableString(textData))
                            {
                                EditorUtility.DisplayDialog("提示", csvCfg.Assets[i].Asset.name + " -> 解析失败...", "好的");
                                bBuildOk = false;
                                break;
                            }

                            string headTml = ParseHeadTmlLabel(csv);

                            if (csvCfg.Assets[i].csharpParse)
                            {
                                vNewCodeLines = new List<string>();

                                bool bMapping = true;

                                List<string> vDefaultLines = GetTmlCode(vTmlMaps, class_name.ToLower() + "Tml", headTml + "_c#");
                                if (vDefaultLines != null) bMapping = false;
                                if (vDefaultLines == null)
                                    vDefaultLines = GetTmlCode(vTmlMaps, "CsvData_Tml", headTml + "_c#");
                                if (vDefaultLines == null)
                                {
                                    EditorUtility.DisplayDialog("提示", "CsvData_Tml[" + headTml + "_c#]模板丢失！！！", "好的");
                                    bBuildOk = false;
                                    break;
                                }
                                for (int j = 0; j < vDefaultLines.Count; ++j)
                                    vNewCodeLines.Add(vDefaultLines[j] + "\r\n");
                                if (Parse(csv, vNewCodeLines, date_time, class_name, bMapping ? vMap : null, vMapping, false, csvCfg.Assets[i].exportAT))
                                {
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/CsvData_" + class_name + ".cs", vNewCodeLines);
                                }
                                else
                                {
                                    bBuildOk = false;
                                    break;
                                }
                            }
                            if (csvCfg.Assets[i].dllParse)
                            {
                                vNewCodeLines = new List<string>();
                                List<string> vNewCodeCppLines = new List<string>();

                                List<string> vDefaultHLines = GetTmlCode(vTmlMaps, class_name.ToLower() + "Tml", headTml + "_c++_h");
                                if (vDefaultHLines == null)
                                    vDefaultHLines = GetTmlCode(vTmlMaps, "CsvData_Tml", headTml + "_c++_h");
                                if (vDefaultHLines == null)
                                {
                                    EditorUtility.DisplayDialog("提示", "CsvData_Tml[" + headTml + "_c++_h]模板丢失！！！", "好的");
                                    bBuildOk = false;
                                    break;
                                }
                                List<string> vDefaultCppLines = GetTmlCode(vTmlMaps, class_name.ToLower() + "Tml", headTml + "_c++_cpp");
                                if (vDefaultCppLines == null)
                                    vDefaultCppLines = GetTmlCode(vTmlMaps, "CsvData_Tml", headTml + "_c++_cpp");
                                if (vDefaultCppLines == null)
                                {
                                    EditorUtility.DisplayDialog("提示", "CsvData_Tml[" + headTml + "_c++_cpp]模板丢失！！！", "好的");
                                    bBuildOk = false;
                                    break;
                                }
                                for (int j = 0; j < vDefaultHLines.Count; ++j)
                                    vNewCodeLines.Add(vDefaultHLines[j] + "\r\n");
                                for (int j = 0; j < vDefaultCppLines.Count; ++j)
                                    vNewCodeCppLines.Add(vDefaultCppLines[j] + "\r\n");

                                if (Parse(csv, vNewCodeLines, date_time, class_name, null, null, true, csvCfg.Assets[i].exportAT) && Parse(csv, vNewCodeCppLines, date_time, class_name, null, null, true, csvCfg.Assets[i].exportAT))
                                {
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/cpp/CsvData_" + class_name + ".h", vNewCodeLines);
                                    BuildCode(Application.dataPath + "/../Temp/CsvCodeTemp/cpp/CsvData_" + class_name + ".cpp", vNewCodeCppLines);

                                    sCppEnum enumC = new sCppEnum();
                                    enumC.strClassName = "CsvData_" + class_name;
                                    enumC.strType = "DataType_" + class_name;
                                    enumC.strHash = csvCfg.Assets[i].nHash.ToString();
                                    vCppDatatTypeEnum.Add(enumC);
                                }
                                else
                                {
                                    bBuildOk = false;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    if (bBuildOk)
                    {
                        string strError = "";
                        //! check mapp data exist
                        foreach (var db in vMapping)
                        {
                            foreach (var sub in db.Value)
                            {
                                if (string.IsNullOrEmpty(sub.strMapFunc) && !vMap.ContainsKey(sub.ClassName.ToLower()))
                                {
                                    if (strError.Length <= 0)
                                        strError += "字段映射表格数据错误:" + sub.ClassName;
                                    else
                                        strError += db + ",";
                                }
                            }

                        }
                        if (strError.Length > 0)
                        {
                            Debug.LogError(strError);
                            //    bBuildOk = false;
                        }
                    }

                    if (bBuildOk)
                    {
                        //! 复制文件
                        if (Directory.Exists(srcDir))
                        {
                            List<ClassSimply> vClass = new List<ClassSimply>();
                            string[] csvs = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories);
                            for (int i = 0; i < csvs.Length; ++i)
                            {
                                ClassSimply simply = new ClassSimply();
                                simply.className = Path.GetFileName(csvs[i]).Replace(".cs", "");
                                if (!vEnableAT.TryGetValue(simply.className, out simply.enabelAT))
                                    simply.enabelAT = false;
                                vClass.Add(simply);
                            }

                            //! copy to sverver project
                            if (bServerCode)
                            {
                                string destSvrDir = Path.Combine(Application.dataPath, rootDir, "Source/Data/Generator/");
                                if (Directory.Exists(destSvrDir))
                                {
                                    EditorUtil.ClearDirectory(destSvrDir);
                                    for (int i = 0; i < csvs.Length; ++i)
                                    {
                                        string destFile = destSvrDir + Path.GetFileName(csvs[i]);
                                        File.Copy(csvs[i], destFile);
                                    }
                                    ParseManagerCsv(destSvrDir + "DataManager_Func.cs", vClass, vMapping, vMap, date_time, false, false);
                                }
                            }
                            else
                            {
                                //! copy to unity project
                                {
                                    string destDir = Path.Combine(Application.dataPath, rootDir);
                                    if (!Directory.Exists(destDir))
                                        Directory.CreateDirectory(destDir);
                                    EditorUtil.ClearDirectory(destDir);
                                    for (int i = 0; i < csvs.Length; ++i)
                                    {
                                        string destFile = Path.Combine(destDir, Path.GetFileName(csvs[i]));
                                        File.Copy(csvs[i], destFile);
                                    }

                                    ParseManagerCsv(Path.Combine(Application.dataPath, rootDir, "DataManager_Func.cs"), vClass, vMapping, vMap, date_time, false);
                                }
                                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                                // build cpp
                                {
                                    bool HasCppGenerator = false;
                                    string destDir = Application.dataPath + CPP_CODE_PROJECT_CSV_PATH;
                                    if (Directory.Exists(destDir))
                                        Directory.Delete(destDir, true);
                                    Directory.CreateDirectory(destDir);
                                    var csv_cpps = Directory.GetFiles(srcDir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".h") || s.EndsWith(".cpp"));
                                    foreach (var tmp in csv_cpps)
                                    {
                                        string destFile = destDir + Path.GetFileName(tmp);
                                        File.Copy(tmp, destFile);
                                        HasCppGenerator = true;
                                    }
                                    if (HasCppGenerator)
                                        BuildCppDataTypeEnum(vCppDatatTypeEnum);
                                }
                            }


                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
        //------------------------------------------------------
        static void BuildCppDataTypeEnum(List<sCppEnum> vEnums)
        {
            string strRoot = Application.dataPath + CPP_CODE_PROJECT_CSV_PATH + "/../";
            if (!Directory.Exists(strRoot))
                Directory.CreateDirectory(strRoot);
            {
                string destDir = strRoot + "DataTypeDef.h";
                if (File.Exists(destDir))
                    File.Delete(destDir);

                FileStream fs = new FileStream(destDir, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                writer.WriteLine("//autogeneraor");
                writer.WriteLine("#ifndef __TopPlus_DataTypeDef__");
                writer.WriteLine("#define __TopPlus_DataTypeDef__");
                writer.WriteLine("namespace TopPlus");
                writer.WriteLine("{");
                writer.WriteLine("\tenum EDataType");
                writer.WriteLine("\t{");
                for (int i = 0; i < vEnums.Count; ++i)
                {
                    writer.WriteLine("\t\t" + vEnums[i].strType + "=" + vEnums[i].strHash + ",");
                }
                writer.WriteLine("\t};");
                writer.WriteLine("\tclass Data_Base;");
                writer.WriteLine("\tclass DataMapUtil");
                writer.WriteLine("\t{");
                writer.WriteLine("\tpublic:");
                writer.WriteLine("\t\tstatic Data_Base* CreateType(EDataType type);");
                writer.WriteLine("\t};");
                writer.WriteLine("}");
                writer.WriteLine("#endif//__TopPlus_DataTypeDef__");
                writer.Close();
            }
            {
                string destDir = strRoot + "DataTypeDef.cpp";
                if (File.Exists(destDir))
                    File.Delete(destDir);

                FileStream fs = new FileStream(destDir, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                writer.WriteLine("//autogeneraor");
                writer.WriteLine("#include \"stdafx.h\"");
                writer.WriteLine("#include \"Data/DataTypeDef.h\"");
                for (int i = 0; i < vEnums.Count; ++i)
                {
                    writer.WriteLine("#include \"Data/generator/" + vEnums[i].strClassName + ".h\"");
                }
                writer.WriteLine("namespace TopPlus");
                writer.WriteLine("{");
                writer.WriteLine("\tData_Base* DataMapUtil::CreateType(EDataType type)");
                writer.WriteLine("\t{");
                writer.WriteLine("\t\tswitch((int)type)");
                writer.WriteLine("\t\t{");
                for (int i = 0; i < vEnums.Count; ++i)
                {
                    writer.WriteLine("\t\tcase " + vEnums[i].strHash + ": return new " + vEnums[i].strClassName + "();");
                }
                writer.WriteLine("\t\t}");
                writer.WriteLine("\t\treturn NULL;");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                writer.Close();
            }
        }
        //------------------------------------------------------
        static void ParseManagerCsv(string file, List<ClassSimply> vClasss, Dictionary<string, List<MappData>> vMapping, Dictionary<string, ClassDatas> vMap, string date_time, bool bCpp, bool bCreateMappingFormat = true)
        {
            List<string> tmlCode = new List<string>();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            string code = "";
            code += "/********************************************************************\r\n";
            //      code += "生成日期:	" + date_time + "\r\n";
            code += "作    者:	" + "自动生成" + "\r\n";
            code += "描    述:\r\n";
            code += "*********************************************************************/\r\n";

            int maxLent = 0;
            for (int i = 0; i < vClasss.Count; ++i)
            {
                maxLent = Mathf.Max(vClasss[i].className.Length, maxLent);
            }
            string space_gap = "\t";

            code += "#if USE_SERVER\r\n";
            code += "using ExternEngine;\r\n";
            code += "#else\r\n";
            code += "using UnityEngine;\r\n";
            code += "#endif\r\n";
            code += "using Framework.Core;\r\n";
            code += "using Framework.Data;\r\n";
            code += "namespace TopGame.Data\r\n";
            code += "{\r\n";
            code += "\tpublic partial class DataManager : ADataManager\r\n";
            code += "\t{\r\n";
            for (int i = 0; i < vClasss.Count; ++i)
            {
                ClassSimply simply = vClasss[i];
                string[] strSplit = simply.className.Split('_');
                if (bCpp)
                    code += "\t\t" + simply.className + space_gap + strSplit[1] + "{get;private set;}\n";
                else
                {
                    code += "\t\tprivate " + simply.className + space_gap + "m_p" + strSplit[1] + ";\r\n";
                    if (simply.enabelAT)
                        code += "\t\t//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]\n";
                    code += "\t\tpublic " + simply.className + space_gap + strSplit[1] + "{\n";
                    code += "\t\t\tget{\r\n";
                    code += "\t\t\t//#if UNITY_EDITOR\r\n";
                    code += "\t\t\t\t//if(m_p" + strSplit[1] + "== null) m_p" + strSplit[1] + "=DataEditorUtil.GetTable<" + simply.className + ">();\r\n";
                    code += "\t\t\t//#endif\r\n";
                    code += "\t\t\t\treturn m_p" + strSplit[1] + ";\r\n";
                    code += "\t\t\t}\r\n";
                    code += "\t\t\tprivate set{m_p" + strSplit[1] + "=value;}\r\n";
                    code += "\t\t}\r\n";
                }
            }
            code += "\t\t//-------------------------------------------\n";
            code += "\t\tprotected override Data_Base  Parser(CsvParser csvParser, int index, TextAsset pAsset, EDataType eType = EDataType.Binary)\n";
            code += "\t\t{\r\n";
            code += "\t\t\tData_Base pCsv = null;\r\n";
            code += "\t\t\tswitch(index)\r\n";
            code += "\t\t\t{\r\n";
            for (int i = 0; i < vClasss.Count; ++i)
            {
                ClassSimply simply = vClasss[i];
                string[] strSplit = simply.className.Split('_');
                if (!vMap.ContainsKey(strSplit[1].ToLower())) continue;
                code += "\t\t\t\tcase " + vMap[strSplit[1].ToLower()].hash + ":\r\n";
                code += "\t\t\t\t{\r\n";
                code += "\t\t\t\t\t " + strSplit[1] + " = new " + simply.className + "();\r\n";
                code += "\t\t\t\t\t pCsv = " + strSplit[1] + "; break;\r\n";
                code += "\t\t\t\t}\r\n";
            }
            code += "\t\t\t}\r\n";
            code += "\t\t\tif(pCsv!=null)\r\n";
            code += "\t\t\t{\r\n";
            code += "\t\t\t\tif(eType == EDataType.Binary)\r\n";
            code += "\t\t\t\t{\r\n";
            code += "\t\t\t\t\tif(!pCsv.LoadBinary(BeginBinary(pAsset.bytes)))\r\n";
            code += "\t\t\t\t\t\tDebug.LogWarning(pAsset.name + \".bytes: load failed ... \" );\r\n";
            code += "\t\t\t\t\telse\r\n";
            code += "\t\t\t\t\tm_nLoadCnt++;\r\n";
            code += "\t\t\t\t\tEndBinary();\r\n";
            code += "\t\t\t\t}\r\n";
            code += "\t\t\t\telse if(eType == EDataType.Json)\r\n";
            code += "\t\t\t\t{\r\n";
            code += "\t\t\t\t\tif( !pCsv.LoadJson(  pAsset.text ) )\r\n";
            code += "\t\t\t\t\t\tDebug.LogWarning(pAsset.name + \".json: load failed ... \" );\r\n";
            code += "\t\t\t\t\telse\r\n";
            code += "\t\t\t\t\t\tm_nLoadCnt++;\r\n";
            code += "\t\t\t\t}\r\n";
            code += "\t\t\t\telse\r\n";
            code += "\t\t\t\t{\r\n";
            code += "\t\t\t\t\tif( !pCsv.LoadData(  pAsset.text,csvParser ) )\r\n";
            code += "\t\t\t\t\t\tDebug.LogWarning(pAsset.name + \".csv: load failed ... \" );\r\n";
            code += "\t\t\t\t\telse\r\n";
            code += "\t\t\t\t\t\tm_nLoadCnt++;\r\n";
            code += "\t\t\t\t}\r\n";
            code += "#if UNITY_EDITOR\r\n";
            code += "\t\t\t\tpCsv.strFilePath = UnityEditor.AssetDatabase.GetAssetPath(pAsset);\r\n";
            code += "#endif\r\n";
            code += "\t\t\t}\r\n";
            code += "\t\t\t\t\treturn pCsv;\r\n";
            code += "\t\t}\r\n";

            Dictionary<string, MappCodeTables> vTableDataMappingCode = new Dictionary<string, MappCodeTables>();
            //! mapping func
            code += "\t\t//-------------------------------------------\r\n";
            code += "\t\tprotected override void Mapping()\r\n";
            code += "\t\t{\r\n";
            foreach (var db in vMapping)
            {
                if (db.Value.Count > 0)
                {
                    MappCodeTables mapCodeTables = new MappCodeTables();
                    bool bMapList = false;
                    if (vMap.ContainsKey(db.Key.ToLower()) && vMap[db.Key.ToLower()].maplist)
                        bMapList = true;

                    string strTmpCode = "";
                    if (bMapList)
                    {
                        foreach (var sub in db.Value)
                        {
                            if (sub.ClassName != null && vMap.ContainsKey(sub.ClassName.ToLower()) && vMap[sub.ClassName.ToLower()].maplist)
                                continue;

                            string strTableCallName = sub.ClassName;
                            if (!string.IsNullOrEmpty(sub.ClassName) && !mapCodeTables.tables.Contains(sub.ClassName)) mapCodeTables.tables.Add(sub.ClassName);
                            string strCastType = "";
                            ClassMappingPropety classProperty;
                            if (ms_vClassMappingPropertys.TryGetValue(sub.ClassName.ToLower(), out classProperty) && !string.IsNullOrEmpty(classProperty.propertyKeyTypeName))
                            {
                                strCastType = "(" + classProperty.propertyKeyTypeName + ")";
                            }

                            if (!string.IsNullOrEmpty(sub.strMapFunc))
                            {
                                strTmpCode += "\t\t\t\t\t\tdb.Value[a]." + sub.strMapFunc + ";\n";
                            }
                            else if (!string.IsNullOrEmpty(sub.strToMapField))
                            {
                                if (sub.bArray)
                                {
                                    strTmpCode += "\t\t\t\t\t\tif(db.Value[a]." + sub.PropName + "!=null)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\tdb.Value[a]." + sub.strToMapField + " = new " + sub.ClassType + "[db.Value[a]." + sub.PropName + ".Length];\n";
                                    strTmpCode += "\t\t\t\t\t\t\tfor(int i=0; i < db.Value[a]." + sub.PropName + ".Length; ++i)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\t\tdb.Value[a]." + sub.strToMapField + "[i] = " + strTableCallName + ".GetData(" + strCastType + "db.Value[a]." + sub.PropName + "[i]);\n";
                                    strTmpCode += "\t\t\t\t\t\t\t}\r\n";
                                    strTmpCode += "\t\t\t\t\t\t}\r\n";
                                }
                                else
                                    strTmpCode += "\t\t\t\t\t\tdb.Value[a]." + sub.strToMapField + " = " + strTableCallName + ".GetData(" + strCastType + "db.Value[a]." + sub.PropName + ");\n";
                            }
                            else
                            {
                                if (sub.bArray)
                                {
                                    strTmpCode += "\t\t\t\t\t\tif(db.Value[a]." + sub.PropName + "!=null)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\tdb.Value[a]." + sub.ClassName + "_" + sub.PropName + "_data = new " + sub.ClassType + "[db.Value[a]." + sub.PropName + ".Length];\n";
                                    strTmpCode += "\t\t\t\t\t\t\tfor(int i=0; i < db.Value[a]." + sub.PropName + ".Length; ++i)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\t\tdb.Value[a]." + sub.ClassName + "_" + sub.PropName + "_data[i] = " + strTableCallName + ".GetData(" + strCastType + "  db.Value[a]." + sub.PropName + "[i]);\n";
                                    strTmpCode += "\t\t\t\t\t\t\t}\r\n";
                                    strTmpCode += "\t\t\t\t\t\t}\r\n";
                                }
                                else
                                    strTmpCode += "\t\t\t\t\t\tdb.Value[a]." + sub.ClassName + "_" + sub.PropName + "_data = " + strTableCallName + ".GetData(" + strCastType + "db.Value[a]." + sub.PropName + ");\n";
                            }
                        }
                    }
                    else
                    {
                        foreach (var sub in db.Value)
                        {
                            if (sub.ClassName != null && vMap.ContainsKey(sub.ClassName.ToLower()) && vMap[sub.ClassName.ToLower()].maplist)
                                continue;

                            string strTableCallName = sub.ClassName;
                            if (!string.IsNullOrEmpty(sub.ClassName) && !mapCodeTables.tables.Contains(sub.ClassName)) mapCodeTables.tables.Add(sub.ClassName);
                            string strCastType = "";
                            if (sub.ClassName != null)
                            {
                                ClassMappingPropety classProperty;
                                if (ms_vClassMappingPropertys.TryGetValue(sub.ClassName.ToLower(), out classProperty) && !string.IsNullOrEmpty(classProperty.propertyKeyTypeName))
                                {
                                    strCastType = "(" + classProperty.propertyKeyTypeName + ")";
                                }
                            }


                            if (!string.IsNullOrEmpty(sub.strMapFunc))
                            {
                                strTmpCode += "\t\t\t\t\tdb.Value." + sub.strMapFunc + ";\n";
                            }
                            else if (!string.IsNullOrEmpty(sub.strToMapField))
                            {
                                if (sub.bArray)
                                {
                                    strTmpCode += "\t\t\t\t\tif(db.Value." + sub.PropName + "!=null)\r\n";
                                    strTmpCode += "\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\tdb.Value." + sub.strToMapField + " = new " + sub.ClassType + "[db.Value." + sub.PropName + ".Length];\n";
                                    strTmpCode += "\t\t\t\t\t\tfor(int i=0; i < db.Value." + sub.PropName + ".Length; ++i)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\tdb.Value." + sub.strToMapField + "[i] = " + strTableCallName + ".GetData(" + strCastType + "db.Value." + sub.PropName + "[i]);\n";
                                    strTmpCode += "\t\t\t\t\t\t}\r\n";
                                    strTmpCode += "\t\t\t\t\t}\r\n";
                                }
                                else
                                    strTmpCode += "\t\t\t\t\tdb.Value." + sub.strToMapField + " = " + strTableCallName + ".GetData(" + strCastType + "db.Value." + sub.PropName + ");\n";
                            }
                            else
                            {
                                if (sub.bArray)
                                {
                                    strTmpCode += "\t\t\t\t\tif(db.Value." + sub.PropName + "!=null)\r\n";
                                    strTmpCode += "\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\tdb.Value." + sub.ClassName + "_" + sub.PropName + "_data = new " + sub.ClassType + "[db.Value." + sub.PropName + ".Length];\n";
                                    strTmpCode += "\t\t\t\t\t\tfor(int i=0; i < db.Value." + sub.PropName + ".Length; ++i)\r\n";
                                    strTmpCode += "\t\t\t\t\t\t{\r\n";
                                    strTmpCode += "\t\t\t\t\t\t\tdb.Value." + sub.ClassName + "_" + sub.PropName + "_data[i] = " + strTableCallName + ".GetData(" + strCastType + "db.Value." + sub.PropName + "[i]);\n";
                                    strTmpCode += "\t\t\t\t\t\t}\r\n";
                                    strTmpCode += "\t\t\t\t\t}\r\n";
                                }
                                else
                                    strTmpCode += "\t\t\t\t\tdb.Value." + sub.ClassName + "_" + sub.PropName + "_data = " + strTableCallName + ".GetData(" + strCastType + "db.Value." + sub.PropName + ");\n";
                            }
                        }
                    }

                    if (strTmpCode.Length > 0)
                    {
                        string strMapingCode = "";
                        strMapingCode += "\t\t\t{\n";
                        strMapingCode += "\t\t\t\tforeach(var db in " + db.Key + ".datas)\n";
                        strMapingCode += "\t\t\t\t{\n";

                        if (bMapList)
                        {
                            strMapingCode += "\t\t\t\t\tfor(int a =0; a< db.Value.Count; ++a)\r\n";
                            strMapingCode += "\t\t\t\t\t{\r\n";
                        }

                        strMapingCode += strTmpCode;
                        if (bMapList)
                            strMapingCode += "\t\t\t\t\t}\r\n";

                        strMapingCode += "\t\t\t\t}\n";
                        strMapingCode += "\t\t\t}\n";

                        code += strMapingCode;
                        mapCodeTables.strCode = strMapingCode;
                        vTableDataMappingCode[db.Key] = mapCodeTables;
                    }
                }
            }
            code += "\t\t}\r\n";

            //! mapping func
            code += "\t\t//-------------------------------------------\r\n";


            //! mapping binary coder
            List<System.Type> vBinarys = new List<System.Type>();
            List<System.Type> vMarcoBinarys = new List<System.Type>();
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = ass.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    if (types[i].GetInterface("IUserData") == null)
                    {
                        continue;
                    }
                    if (types[i].IsDefined(typeof(BinaryCodeAttribute)))
                    {
                        if (types[i].IsDefined(typeof(BinaryCodeMarcosAttribute)))
                        {
                            vMarcoBinarys.Add(types[i]);
                        }
                        else
                            vBinarys.Add(types[i]);
                    }
                }
            }
            code += "\t\tprotected override IUserData OnReadBinary(System.Type classType, byte[] buffers, int dataSize)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif (buffers == null || dataSize <= 0) return null;\r\n";
            code += "\t\t\tBinaryUtil binary = new BinaryUtil();\r\n";
            code += "\t\t\tbinary.Set(buffers, dataSize);\r\n";
            for (int j = 0; j < vMarcoBinarys.Count; ++j)
            {
                code += "\t\t\t#if " + vMarcoBinarys[j].GetCustomAttribute<BinaryCodeMarcosAttribute>().marcos + "\r\n";
                code += "\t\t\tif(classType == typeof(" + vMarcoBinarys[j].FullName + "))\r\n";
                code += "\t\t\t\treturn " + vMarcoBinarys[j].FullName + "BinaryUtil.Read(ref binary);\r\n";
                code += "\t\t\t#endif\r\n";
            }
            for (int j = 0; j < vBinarys.Count; ++j)
            {
                if (j == 0)
                    code += "\t\t\tif(classType == typeof(" + vBinarys[j].FullName + "))\r\n";
                else
                    code += "\t\t\telse if(classType == typeof(" + vBinarys[j].FullName + "))\r\n";
                code += "\t\t\t\treturn " + vBinarys[j].FullName + "BinaryUtil.Read(ref binary);\r\n";
            }
            code += "\t\t\treturn null;\r\n";
            code += "\t\t}\r\n";

            code += "\t}\r\n";
            code += "}\r\n";

            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();

            //if (bCreateMappingFormat)
            //{
            //    //! create editor mapping code
            //    string editorMappingFile = Application.dataPath + CODE_PROJECT_CSV_EDITOR_MAPPING;
            //    if (File.Exists(editorMappingFile))
            //    {
            //        File.Delete(editorMappingFile);
            //    }
            //    code = "#if UNITY_EDITOR\r\n";
            //    code += "//auto generator code\r\n";
            //    code += "using Framework.Core;\r\n";
            //    code += "using System.Collections.Generic;\r\n";
            //    code += "using Framework.Data;\r\n";
            //    code += "using TopGame.Data;\r\n";
            //    code += "namespace TopGame.ED\r\n";
            //    code += "{\r\n";
            //    code += "\tpublic partial class CsvDataFormer\r\n";
            //    code += "\t{\r\n";
            //    code += "\t\tstatic Framework.Module.AFramework GetFramework(){ return null; }\r\n";
            //    code += "\t\t[TableMapping(\"mappingdata\")]\r\n";
            //    code += "\t\tpublic static void MappingData(Data_Base table)\r\n";
            //    code += "\t\t{\r\n";
            //    code += "\t\t\tif(table == null) return;\r\n";
            //    code += "\t\t\tint hash = UnityEngine.Animator.StringToHash(table.GetType().Name.ToLower());\r\n";
            //    code += "\t\t\tswitch(hash)\r\n";
            //    code += "\t\t\t{\r\n";
            //    foreach (var db in vTableDataMappingCode)
            //    {
            //        code += "\t\t\tcase " + UnityEngine.Animator.StringToHash("csvdata_" + db.Key.ToLower()) + ":\r\n";
            //        code += "\t\t\t{\r\n";
            //        code += "\t\t\t\tCsvData_" + db.Key + " " + db.Key + " = (CsvData_" + db.Key + ")table;\r\n";
            //        for (int i = 0; i < db.Value.tables.Count; ++i)
            //            code += "\t\t\t\tCsvData_" + db.Value.tables[i] + " " + db.Value.tables[i] + "=" + "DataEditorUtil.GetTable<CsvData_" + db.Value.tables[i] + ">();\r\n";
            //        if (db.Value.tables.Count > 0)
            //        {
            //            code += "\t\t\t\tif(";
            //            for (int i = 0; i < db.Value.tables.Count; ++i)
            //            {
            //                code += db.Value.tables[i] + "!=null";
            //                if (i < db.Value.tables.Count - 1) code += "&& ";
            //            }
            //            code += ")\r\n";
            //        }

            //        code += db.Value.strCode;
            //        code += "\t\t\t}\r\n";
            //        code += "\t\t\tbreak;\r\n";
            //    }
            //    code += "\t\t\t}\r\n";
            //    code += "\t\t}\r\n";
            //    code += "\t}\r\n";
            //    code += "}\r\n";
            //    code += "#endif\r\n";
            //    fs = new FileStream(editorMappingFile, FileMode.OpenOrCreate);
            //    writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            //    writer.Write(code);
            //    writer.Close();
            //}
        }
        //------------------------------------------------------
        static HashSet<string> ParseServerTableConfig()
        {
            HashSet<string> vServerConfig = new HashSet<string>();
            string file = Application.dataPath + "/Scripts/Editor/CsvEditor/serverConfig.txt";
            if (!File.Exists(file)) return vServerConfig;
            FileStream fs = new FileStream(file, FileMode.Open);
            StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8);
            string sStuName = string.Empty;
            while ((sStuName = reader.ReadLine()) != null)
            {
                vServerConfig.Add(sStuName.Trim().ToLower());
            }

            reader.Close();
            fs.Close();

            return vServerConfig;
        }
        //------------------------------------------------------
        static bool ParseTml(ref List<string> tmlCode, string tml = "CsvData_Tml")
        {
            string eidotPath = InitEditorPath();
            if (string.IsNullOrEmpty(eidotPath)) eidotPath = "Scripts/GameEditor/CsvEditor/";
            string file = Path.Combine(Application.dataPath , eidotPath, tml + ".txt");
            if (!File.Exists(file)) return false;
            FileStream fs = new FileStream(file, FileMode.Open);
            StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8);
            string sStuName = string.Empty;
            while ((sStuName = reader.ReadLine()) != null)
            {
                tmlCode.Add(sStuName);
            }

            reader.Close();
            fs.Close();

            return true;
        }
        //------------------------------------------------------
        struct sPropData
        {
            public string label;
            public string spec;
            public string enumType;
            public bool array;

            public string table_label;
            public string[] table_type_sepc;
        }
        //------------------------------------------------------
        static string ReadString(BinaryReader reader)
        {
            int lent = reader.ReadUInt16();
            if (lent <= 0) return "";
            return System.Text.Encoding.UTF8.GetString(reader.ReadBytes(lent));
        }
        //------------------------------------------------------
        static string ParseHeadTmlLabel(byte[] bytes)
        {
            BinaryReader read = new BinaryReader(new MemoryStream(bytes));
            int version = read.ReadInt32();
            int head = read.ReadUInt16();
            int row = read.ReadInt32();
            int col = read.ReadInt32();

            if (head <= 0)
                return "default";

            //! 数据类型
            List<sPropData> propTypes = new List<sPropData>();
            for (int i = 0; i < col; ++i)
                ReadString(read);

            //! 键值函数
            string keyFunc = ReadString(read);
            int tmlHead = keyFunc.IndexOf('@');
            if (tmlHead > 0 && tmlHead < keyFunc.Length)
            {
                return keyFunc.Substring(0, tmlHead);
            }
            return "default";
        }
        //------------------------------------------------------
        static int BuildCodeByBinary(byte[] bytes, List<string> vNewCode, string strDateTime, string strClassName, Dictionary<string, ClassDatas> vClasses, Dictionary<string, List<MappData>> vMapping, bool bEnableAT)
        {
            BinaryReader read = new BinaryReader(new MemoryStream(bytes));
            int version = read.ReadInt32();
            int head = read.ReadUInt16();
            int row = read.ReadInt32();
            int col = read.ReadInt32();

            if (head <= 0)
                return 2;

            //! 数据类型
            List<sPropData> propTypes = new List<sPropData>();
            for (int i = 0; i < col; ++i)
            {
                sPropData data = new sPropData();
                string tmp = ReadString(read);
                if (tmp.ToLower().Contains("table|"))
                {
                    string strSub = tmp.Substring("table|".Length);
                    if (strSub.Length <= 1)
                    {
                        Debug.LogError("Table 格式不对:" + tmp);
                        return 0;
                    }
                    data.label = "table";
                    string[] keyValue = strSub.Split('-');
                    if (keyValue.Length == 2 && keyValue[0].Length > 0 && keyValue[1].Length > 0)
                    {
                        data.table_type_sepc = new string[6];
                        string[] keyTypeSpec = keyValue[0].Replace("enum|", "").Split('|');
                        string[] valueTypeSpec = keyValue[1].Replace("enum|", "").Split('|');

                        string strKeyLabel = "";
                        string strSpec = "";
                        if (keyTypeSpec.Length == 2)
                        {
                            strKeyLabel = keyTypeSpec[0];
                            strSpec = keyTypeSpec[1];
                        }
                        else
                        {
                            strKeyLabel = keyTypeSpec[0];
                        }
                        ConvertType(ref strKeyLabel);
                        data.table_type_sepc[0] = strKeyLabel;
                        data.table_type_sepc[1] = strSpec;
                        data.table_type_sepc[2] = keyValue[0].Contains("enum|") ? strKeyLabel : null;

                        if (valueTypeSpec.Length == 2)
                        {
                            strKeyLabel = valueTypeSpec[0];
                            strSpec = valueTypeSpec[1];
                        }
                        else
                        {
                            strKeyLabel = valueTypeSpec[0];
                        }
                        ConvertType(ref strKeyLabel);
                        data.table_type_sepc[3] = strKeyLabel;
                        data.table_type_sepc[4] = strSpec;
                        data.table_type_sepc[5] = keyValue[1].Contains("enum|") ? strKeyLabel : null;
                        data.table_label = "Dictionary<" + data.table_type_sepc[0] + "," + data.table_type_sepc[3] + ">";
                    }
                    data.enumType = null;
                    data.spec = null;
                }
                else if (tmp.ToLower().Contains("enum|"))
                {
                    string[] assign_spec_type = tmp.Split('|');
                    data.label = assign_spec_type[0];
                    data.enumType = assign_spec_type[1];
                    data.spec = null;
                }
                else
                {
                    int pos = tmp.IndexOf('|');
                    if (pos > 0 && pos < tmp.Length)
                    {
                        data.label = tmp.Substring(0, pos);
                        data.spec = tmp.Substring(pos + 1);
                    }
                    else
                    {
                        data.spec = "";
                        data.enumType = null;
                        data.label = tmp;
                    }
                }

                if (data.label.ToLower().CompareTo("vector2") == 0) data.label = "Vector2";
                else if (data.label.ToLower().CompareTo("vector3") == 0) data.label = "Vector3";
                else if (data.label.ToLower().CompareTo("vector4") == 0) data.label = "Vector4";
                else if (data.label.ToLower().CompareTo("vector2int") == 0) data.label = "Vector2Int";
                else if (data.label.ToLower().CompareTo("vector3int") == 0) data.label = "Vector3Int";

                if (data.label.Contains("[]")) data.array = true;
                else data.array = false;
                propTypes.Add(data);
            }

            //! 键值函数
            string keyFunc = ReadString(read);
            int tmlHead = keyFunc.IndexOf('@');
            bool valueList = false;
            if (tmlHead >= 0 && tmlHead < keyFunc.Length)
            {
                keyFunc = keyFunc.Substring(tmlHead + 1);
                if (keyFunc.Substring(0, tmlHead).CompareTo("maplist") == 0)
                {
                    valueList = true;
                }
            }

            //! 字段表格映射
            List<string> maps = new List<string>();
            maps.Add("");
            for (int i = 1; i < col; ++i)
            {
                string map = ReadString(read);
                maps.Add(map.Trim());
            }

            //! 属性名称
            List<string> propTypeNames = new List<string>();
            for (int i = 0; i < col; ++i)
            {
                string name = ReadString(read);
                if (string.IsNullOrEmpty(name))
                    return 0;
                if (propTypeNames.Contains(name))
                {
                    Debug.LogError("表格中具有相同属性名称:" + name);
                    return 0;
                }
                propTypeNames.Add(name);
            }

            //! 解析字段类型
            string strPropField = "";
            string[] arCastName = new string[col];
            string[] arPropType = new string[col];
            for (int i = 0; i < col; ++i)
            {
                if (!string.IsNullOrEmpty(propTypes[i].table_label))
                {
                    if (bEnableAT)
                        strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + propTypes[i].table_label + "\t\t\t\t" + propTypeNames[i] + ";\r\n";
                    else
                        strPropField += "\t\t\t" + "public\t" + propTypes[i].table_label + "\t\t\t\t" + propTypeNames[i] + ";\r\n";

                    arCastName[i] = "";
                }
                else if (!string.IsNullOrEmpty(propTypes[i].enumType))
                {
                    if (bEnableAT)
                        strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + propTypes[i].enumType + "\t\t\t\t" + propTypeNames[i] + ";\r\n";
                    else
                        strPropField += "\t\t\t" + "public\t" + propTypes[i].enumType + "\t\t\t\t" + propTypeNames[i] + ";\r\n";
                    arCastName[i] += "(" + propTypes[i].enumType + ")";
                }
                else
                {
                    if (bEnableAT)
                        strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + propTypes[i].label + "\t\t\t\t" + propTypeNames[i] + ";\r\n";
                    else
                        strPropField += "\t\t\t" + "public\t" + propTypes[i].label + "\t\t\t\t" + propTypeNames[i] + ";\r\n";
                    arCastName[i] = "";
                }
            }
            string strSaveField = "";
            string strParseField = "";
            for (int i = 0; i < col; ++i)
            {
                string label = propTypes[i].label.ToLower();
                if (!string.IsNullOrEmpty(propTypes[i].enumType))
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadInt32();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write((int)db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "table")
                {

                }
                else if (label == "string")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "ReadString(reader);\r\n";
                    strSaveField += "\t\t\t\tWriterString(writer,db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "bool")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadBoolean();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "char")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadChar();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "byte")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadByte();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "short")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadInt16();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "ushort")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadUInt16();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "int")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadInt32();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "uint")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadUInt32();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "long")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadInt64();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "ulong")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadUInt64();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "float")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadSingle();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "double")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "reader.ReadDouble();\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ");\r\n";
                }
                else if (label == "vector2")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "new Vector2(reader.ReadSingle(),reader.ReadSingle());\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".x);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".y);\r\n";
                }
                else if (label == "vector3")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "new Vector3(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".x);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".y);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".z);\r\n";
                }
                else if (label == "vector4")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "new Vector4(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".x);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".y);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".z);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".w);\r\n";
                }
                else if (label == "vector2int")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "new Vector2Int(reader.ReadInt32(),reader.ReadInt32());\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".x);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".y);\r\n";
                }
                else if (label == "vector3int")
                {
                    strParseField += "\t\t\t\t" + "data." + propTypeNames[i] + " = " + arCastName[i] + "new Vector3Int(reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32());\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".x);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".y);\r\n";
                    strSaveField += "\t\t\t\t" + "writer.Write(db.Value." + propTypeNames[i] + ".z);\r\n";
                }
                else if (label == "byte[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadByte();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "int[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadInt32();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "uint[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadUInt32();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "float[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadSingle();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "short[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadInt16();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "ushort[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new " + propTypes[i].label.Substring(0, propTypes[i].label.Length - 2) + "[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "vector2[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new Vector2[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = new Vector2(reader.ReadSingle(),reader.ReadSingle());\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t{\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].x);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].y);\r\n";
                    strSaveField += "\t\t\t\t\t}\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "vector3[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new Vector3[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = new Vector3(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t{\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].x);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].y);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].z);\r\n";
                    strSaveField += "\t\t\t\t\t}\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "vector3int[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new Vector3Int[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = new Vector3Int(reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32());\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t{\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].x);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].y);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].z);\r\n";
                    strSaveField += "\t\t\t\t\t}\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "vector2int[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new Vector2Int[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = new Vector2Int(reader.ReadInt32(),reader.ReadInt32());\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t{\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].x);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].y);\r\n";
                    strSaveField += "\t\t\t\t\t\twriter.Write(db.Value." + propTypeNames[i] + "[i].z);\r\n";
                    strSaveField += "\t\t\t\t\t}\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else if (label == "string[]")
                {
                    strParseField += "\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\tint len=reader.ReadUInt16();\n";
                    strParseField += "\t\t\t\t\tif(len>0)\n";
                    strParseField += "\t\t\t\t\t{\n";
                    strParseField += "\t\t\t\t\t\t" + "data." + propTypeNames[i] + "=" + "new string[len];\n";
                    strParseField += "\t\t\t\t\t\t" + "for(int j =0; j < len; ++j)\n";
                    strParseField += "\t\t\t\t\t\t\t" + "data." + propTypeNames[i] + "[j] = ReadString(reader);\n";
                    strParseField += "\t\t\t\t\t}\n";
                    strParseField += "\t\t\t\t}\n";

                    strSaveField += "\t\t\t\t" + "if(db.Value." + propTypeNames[i] + "!=null)\r\n";
                    strSaveField += "\t\t\t\t" + "{\r\n";
                    strSaveField += "\t\t\t\t\t" + "writer.Write((ushort)db.Value." + propTypeNames[i] + ".Length);\r\n";
                    strSaveField += "\t\t\t\t\tfor(int i = 0; i < db.Value." + propTypeNames[i] + ".Length; ++i)\r\n";
                    strSaveField += "\t\t\t\t\t\tWriterString(writer, db.Value." + propTypeNames[i] + "[i]);\r\n";
                    strSaveField += "\t\t\t\t" + "}\r\n";
                    strSaveField += "\t\t\t\t" + "else";
                    strSaveField += " writer.Write((ushort)0);\r\n";
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "数据类型[" + label + "]不存在", "好的");
                    return 0;
                }
            }

            string buildFunc = "";

            //! 解析键值
            string keyType = propTypes[0].label;
            string keyName = "data." + propTypeNames[0];
            if (keyFunc.Length > 0)
            {
                string[] spilt = keyFunc.Split('=');
                if (spilt.Length == 2)
                {
                    string valueListCompareCode = "";
                    string funcAgvs = "";
                    string func = spilt[1];
                    bool bValidFunc = false;
                    for (int f = 0; f < propTypeNames.Count; ++f)
                    {
                        if (func.Contains(propTypeNames[f]))
                        {
                            func = func.Replace(propTypeNames[f], "data." + propTypeNames[f]);
                            bValidFunc = true;
                            funcAgvs += arPropType[f] + " " + propTypeNames[f] + ", ";

                            valueListCompareCode += "outData[i]." + propTypeNames[f] + "==" + propTypeNames[f] + "&& ";
                        }
                    }
                    if (bValidFunc && funcAgvs.Length > 0)
                    {
                        keyType = spilt[0];
                        keyName = "nTempCombineKey";

                        strParseField += "\t\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + func + ");";

                        if (funcAgvs[funcAgvs.Length - 1] == ' ')
                            funcAgvs = funcAgvs.Substring(0, funcAgvs.Length - 1);

                        if (funcAgvs[funcAgvs.Length - 1] == ',')
                            funcAgvs = funcAgvs.Substring(0, funcAgvs.Length - 1);

                        if (valueListCompareCode.Length > 0)
                        {
                            if (valueListCompareCode[valueListCompareCode.Length - 1] == ' ')
                                valueListCompareCode = valueListCompareCode.Substring(0, valueListCompareCode.Length - 1);

                            if (valueListCompareCode.Length > 0 && valueListCompareCode[valueListCompareCode.Length - 1] == '&')
                                valueListCompareCode = valueListCompareCode.Substring(0, valueListCompareCode.Length - 1);
                            if (valueListCompareCode.Length > 0 && valueListCompareCode[valueListCompareCode.Length - 1] == '&')
                                valueListCompareCode = valueListCompareCode.Substring(0, valueListCompareCode.Length - 1);
                        }

                        //! 有组合键值，提供组合键值函数获取
                        buildFunc = "public " + strClassName + "Data" + " GetData(" + funcAgvs + ")\r\n";
                        buildFunc += "\t\t{\r\n";
                        if (valueList)
                        {
                            buildFunc += "\t\t\t" + strClassName + "Data outData;\r\n";
                            buildFunc += "\t\t\tif (m_vData.TryGetValue(nTempCombineKey, out outData))\r\n";
                            buildFunc += "\t\t\t{\r\n";
                            buildFunc += "\t\t\t\tfor(int i =0; i< outData.Count;++i)\r\n";
                            buildFunc += "\t\t\t\tif(" + valueListCompareCode + ")\r\n";
                            buildFunc += "\t\t\t\t\treturn outData;\r\n";
                            buildFunc += "\t\t\t}\r\n";
                        }
                        else
                        {
                            buildFunc += "\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + spilt[1] + ");\r\n";
                            buildFunc += "\t\t\t" + strClassName + "Data outData;\r\n";

                            buildFunc += "\t\t\tif (m_vData.TryGetValue(nTempCombineKey, out outData))\r\n";
                            buildFunc += "\t\t\t{\r\n";
                            buildFunc += "\t\t\t\treturn outData;\r\n";
                            buildFunc += "\t\t\t}\r\n";
                        }

                        buildFunc += "\t\t\treturn null;\r\n";
                        buildFunc += "\t\t}";
                    }
                }
            }


            List<MappData> vDatas = new List<MappData>();
            //! csv mapping
            strPropField += "\n";
            strPropField += "\t\t\t//mapping data\n";
            if (vClasses != null)
            {
                ClassDatas classData;
                if (vClasses.TryGetValue(strClassName, out classData))
                    classData.maplist = valueList;

                for (int i = 1; i < col; ++i)
                {
                    string mappCsv = maps[i];

                    if (mappCsv.Length > 0)
                    {
                        mappCsv = mappCsv.Substring(0, 1).ToUpper() + mappCsv.Substring(1, mappCsv.Length - 1);
                        if (!vClasses.ContainsKey(mappCsv.ToLower()) || vClasses[mappCsv.ToLower()].maplist) continue;

                        MappData temp = new MappData();
                        temp.PropName = propTypeNames[i];
                        temp.ClassName = mappCsv;
                        temp.ClassType = "CsvData_" + mappCsv + "." + mappCsv + "Data";
                        temp.strToMapField = null;
                        temp.strMapFunc = null;
                        temp.bArray = propTypes[i].label.ToLower().Contains("[]");

                        vDatas.Add(temp);

                        ClassMappingPropety classProperty;
                        if (bEnableAT)
                        {
                            if (ms_vClassMappingPropertys.TryGetValue(mappCsv.ToLower(), out classProperty))
                            {
                                if (temp.bArray)
                                    strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + classProperty.propertyClassName + "[]" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                                else
                                    strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + classProperty.propertyClassName + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                            }
                            else
                            {
                                if (temp.bArray)
                                    strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data[]" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                                else
                                    strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                            }
                        }
                        else
                        {
                            if (ms_vClassMappingPropertys.TryGetValue(mappCsv.ToLower(), out classProperty))
                            {
                                if (temp.bArray)
                                    strPropField += "\t\t\t" + "public\t" + classProperty.propertyClassName + "[]" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                                else
                                    strPropField += "\t\t\t" + "public\t" + classProperty.propertyClassName + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                            }
                            else
                            {
                                if (temp.bArray)
                                    strPropField += "\t\t\t" + "public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data[]" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                                else
                                    strPropField += "\t\t\t" + "public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data" + " " + mappCsv + "_" + propTypeNames[i] + "_data" + ";\r\n";
                            }
                        }

                    }
                }
            }

            vMapping.Add(strClassName, vDatas);

            ParseDateTimeFiled(vNewCode, strDateTime);
            ParseClassNameFiled(vNewCode, strClassName);
            ParsePropFiled(vNewCode, strPropField);
            ParseDataKeyFiled(vNewCode, keyType);
            ParseUseKeyFiled(vNewCode, keyName);
            ParseFunc(vNewCode, buildFunc);
            ParseFiled(vNewCode, strParseField);
            SaveFiled(vNewCode, strSaveField);
            ParseATEnableFiled(vNewCode, bEnableAT);
            return 1;
        }
        class CppJsonClass
        {
            public string strName;
            public string strTypeName;
            public string strParseFuncName;
            public bool bArray = false;
            public bool bCanDec = false;
            public bool bClassParser = false;
            public bool bCast = false;
            public List<CppJsonClass> vFields = new List<CppJsonClass>();
        }
        //------------------------------------------------------
        static string CanExportToCppFieldType(System.Type type, out string JsonFuncName, bool bVec = false)
        {
            bVec = !bVec;
            JsonFuncName = "";
            if (type == typeof(byte)) { JsonFuncName = bVec ? "Byte" : "ByteArray"; return "byte"; }
            if (type == typeof(char)) { JsonFuncName = bVec ? "Char" : "CharArray"; return "char"; }
            if (type == typeof(bool)) { JsonFuncName = bVec ? "Bool" : "BoolArray"; return "bool"; }
            if (type == typeof(int)) { JsonFuncName = bVec ? "Int" : "IntArray"; return "int"; }
            if (type == typeof(uint)) { JsonFuncName = bVec ? "Uint" : "UintArray"; return "uint"; }
            if (type == typeof(short)) { JsonFuncName = bVec ? "Short" : "ShortArray"; return "short"; }
            if (type == typeof(ushort)) { JsonFuncName = bVec ? "Ushort" : "UshortArray"; return "ushort"; }
            if (type == typeof(long)) { JsonFuncName = bVec ? "Int64" : "IntArray"; return "tInt64"; }
            if (type == typeof(ulong)) { JsonFuncName = bVec ? "Uint64" : "IntArray"; return "tUint64"; }
            if (type == typeof(System.Int64)) { JsonFuncName = bVec ? "Int64" : "Int64Array"; return "tInt64"; }
            if (type == typeof(System.Int32)) { JsonFuncName = bVec ? "Int" : "IntArray"; return "int"; }
            if (type == typeof(System.UInt32)) { JsonFuncName = bVec ? "Uint" : "UintArray"; return "uint"; }
            if (type == typeof(float)) { JsonFuncName = bVec ? "Float" : "FloatArray"; return "float"; }
            if (type == typeof(double)) { JsonFuncName = bVec ? "Double" : "DoubleArray"; return "double"; }
            if (type == typeof(Vector2)) { JsonFuncName = bVec ? "Vec2" : "Vec2Array"; return "Vector2"; }
            if (type == typeof(Vector3)) { JsonFuncName = bVec ? "Vec3" : "Vec3Array"; return "Vector3"; }
            if (type == typeof(Vector4)) { JsonFuncName = bVec ? "Vec4" : "Vec4Array"; return "Vector4"; }
            if (type == typeof(Vector2Int)) { JsonFuncName = bVec ? "IVec2" : "IVec2Array"; return "IVector2"; }
            if (type == typeof(Vector3Int)) { JsonFuncName = bVec ? "IVec3" : "IVec3Array"; return "IVector3"; }
            if (type == typeof(string)) { JsonFuncName = bVec ? "String" : "StringArray"; return "string"; }
            if (type.IsEnum)
            {
                JsonFuncName = "Int";
                if (type.Name.Contains("."))
                    return type.Name.Substring(type.Name.IndexOf('.') + 1);
                return type.Name;
            }
            return null;
        }
        //------------------------------------------------------
        static bool CheckIsArrayOrList(ref bool bArray, ref bool bList, out System.Type datatType, System.Type type)
        {
            datatType = null;
            if (type.IsArray || type.IsGenericType)
            {
                if (type.IsArray)
                {
                    datatType = type.Assembly.GetType(type.FullName.Replace("[]", ""));
                    bArray = true;
                    return datatType != null;
                }
                else
                {
                    if (type.Name.Contains("List`1") && type.GenericTypeArguments != null && type.GenericTypeArguments.Length == 1)
                    {
                        datatType = type.GenericTypeArguments[0];
                        return true;
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------
        static void BuildJsonClassField(System.Type type, CppJsonClass classData, ref List<CppJsonClass> vDeclStruct)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsNotSerialized) continue;

                BuildJsonCppPropFiled(classData, ref vDeclStruct, fields[i]);
            }
        }
        //------------------------------------------------------
        static void BuildJsonCppPropFiled(CppJsonClass classData, ref List<CppJsonClass> vDeclStruct, FieldInfo field)
        {
            bool bArray = false, bList = false;
            System.Type dataType;
            if (CheckIsArrayOrList(ref bArray, ref bList, out dataType, field.FieldType))
            {
                string parsFuncName = "";
                string strField = CanExportToCppFieldType(dataType, out parsFuncName, true);
                if (string.IsNullOrEmpty(strField))
                {
                    CppJsonClass classD = new CppJsonClass();
                    classD.strName = field.Name;
                    classD.strTypeName = dataType.Name;
                    classD.bCanDec = true;
                    classD.bArray = true;
                    classD.bClassParser = true;
                    if (classData != null)
                        classData.vFields.Add(classD);
                    else
                        vDeclStruct.Add(classD);

                    BuildJsonClassField(dataType, classD, ref vDeclStruct);
                }
                else
                {
                    CppJsonClass classD = new CppJsonClass();
                    classD.strName = field.Name;
                    classD.strParseFuncName = parsFuncName;
                    classD.strTypeName = strField;
                    classD.bArray = true;
                    classD.bCanDec = false;
                    if (classData != null)
                        classData.vFields.Add(classD);
                    else
                        vDeclStruct.Add(classD);
                }
            }
            else
            {
                string parsFuncName = "";
                string strField = CanExportToCppFieldType(field.FieldType, out parsFuncName, false);
                if (!string.IsNullOrEmpty(strField))
                {
                    if (classData == null)
                    {
                        classData = new CppJsonClass();
                        classData.strName = field.Name;
                        classData.strTypeName = strField;
                        classData.strParseFuncName = parsFuncName;

                        if (field.FieldType.IsEnum)
                            classData.bCast = true;

                        vDeclStruct.Add(classData);
                    }
                    else
                    {
                        CppJsonClass fieldData = new CppJsonClass();
                        fieldData.strName = field.Name;
                        fieldData.strTypeName = strField;
                        fieldData.strParseFuncName = parsFuncName;
                        if (field.FieldType.IsEnum)
                            fieldData.bCast = true;
                        classData.vFields.Add(fieldData);
                    }
                }
                else if (field.FieldType.IsClass || field.FieldType.BaseType == typeof(System.ValueType))
                {
                    string structName = field.FieldType.Name;
                    CppJsonClass structData = new CppJsonClass();
                    structData.strName = field.Name;
                    structData.strTypeName = structName;
                    structData.bCanDec = true;
                    structData.bClassParser = true;
                    if (classData != null) classData.vFields.Add(structData);
                    else
                        vDeclStruct.Add(classData);

                    BuildJsonClassField(field.FieldType, structData, ref vDeclStruct);
                }
            }
        }
        //------------------------------------------------------
        static void BuildJsonCppPropFiled(ref List<CppJsonClass> vDeclStruct, FieldInfo[] fields)
        {
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].IsNotSerialized) continue;
                BuildJsonCppPropFiled(null, ref vDeclStruct, fields[i]);
            }
        }
        //------------------------------------------------------
        static void FindCanDecClass(List<CppJsonClass> vDecs, CppJsonClass pParent)
        {
            if (pParent == null) return;
            if (pParent.bCanDec)
            {
                bool bExist = false;
                for (int i = 0; i < vDecs.Count; ++i)
                {
                    if (vDecs[i].strTypeName.CompareTo(pParent.strTypeName) == 0)
                    {
                        bExist = true;
                    }
                }
                if (!bExist) vDecs.Add(pParent);
            }
            for (int i = 0; i < pParent.vFields.Count; ++i)
                FindCanDecClass(vDecs, pParent.vFields[i]);
        }
        //------------------------------------------------------
        static int BuildCodeCppByJson(SBinderData binderData, List<string> vNewCode, string strDateTime, string strClassName, bool bIncludeHead = false, bool bEnableAT = false)
        {
            FieldInfo fieldInfo = binderData.BindType.GetField("datas");
            if (fieldInfo == null || !fieldInfo.FieldType.IsArray) return 0;

            System.Type eleType = fieldInfo.FieldType.Assembly.GetType(fieldInfo.FieldType.FullName.Replace("[]", ""));
            if (eleType == null) return 0;

            string strParseField = "";
            string buildFunc = "";
            string CppFunc = "";
            string PropField = "";
            string JsonClassName = fieldInfo.FieldType.Name.Replace("[]", "");

            List<CppJsonClass> vJsons = new List<CppJsonClass>();
            FieldInfo[] fields = eleType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            BuildJsonCppPropFiled(ref vJsons, fields);

            List<CppJsonClass> vDecs = new List<CppJsonClass>();
            for (int i = 0; i < vJsons.Count; ++i)
            {
                FindCanDecClass(vDecs, vJsons[i]);
            }

            string strClassDec = "\r\n";
            for (int i = vDecs.Count - 1; i >= 0; --i)
            {
                if (vDecs[i].bCanDec)
                {
                    strClassDec += "\t\tstruct " + vDecs[i].strTypeName + "\r\n";
                    strClassDec += "\t\t{\r\n";
                    for (int j = 0; j < vDecs[i].vFields.Count; ++j)
                    {
                        if (vDecs[i].vFields[j].bArray)
                            strClassDec += "\t\t\tvector<" + vDecs[i].vFields[j].strTypeName + "> " + vDecs[i].vFields[j].strName + ";\r\n";
                        else
                            strClassDec += "\t\t\t" + vDecs[i].vFields[j].strTypeName + " " + vDecs[i].vFields[j].strName + ";\r\n";
                    }
                    //! parse func
                    strClassDec += "\t\t\tvoid Parse(rapidjson::Value* pEnter, int index)\r\n";
                    strClassDec += "\t\t\t{\r\n";
                    for (int j = 0; j < vDecs[i].vFields.Count; ++j)
                    {
                        if (vDecs[i].vFields[j].bClassParser && string.IsNullOrEmpty(vDecs[i].vFields[j].strParseFuncName))
                        {
                            if (vDecs[i].vFields[j].bArray)
                            {
                                strClassDec += "\t\t\t\t{\r\n";
                                strClassDec += "\t\t\t\t\trapidjson::Value* pSub = JsonParser::ValueArray(*pEnter,\"" + vDecs[i].vFields[j].strName + "\",index);\r\n";
                                strClassDec += "\t\t\t\t\tif(pSub!=NULL)\r\n";
                                strClassDec += "\t\t\t\t\t{\r\n";
                                strClassDec += "\t\t\t\t\t\tfor(int i = 0; i < (int)(*pSub).Size(); ++i)\r\n";
                                strClassDec += "\t\t\t\t\t\t{\r\n";
                                strClassDec += "\t\t\t\t\t\t\t" + vDecs[i].vFields[j].strTypeName + " tmp;\r\n";
                                strClassDec += "\t\t\t\t\t\t\ttmp.Parse(pSub, i);\r\n";
                                strClassDec += "\t\t\t\t\t\t\t" + vDecs[i].vFields[j].strName + ".push_back(tmp);\r\n";
                                strClassDec += "\t\t\t\t\t\t}\r\n";
                                strClassDec += "\t\t\t\t\t}\r\n";
                                strClassDec += "\t\t\t\t}\r\n";
                            }
                            else
                            {
                                string strObjectGet = "JsonParser::ValueObject(*pEnter," + "\"" + vDecs[i].vFields[j].strName + "\", index)";
                                strClassDec += "\t\t\t\t" + vDecs[i].vFields[j].strName + ".Parse(" + strObjectGet + ",-1);\r\n";
                            }
                        }
                        else
                        {
                            if (vDecs[i].vFields[j].bCast)
                                strClassDec += "\t\t\t\t" + vDecs[i].vFields[j].strName + " = (" + vDecs[i].vFields[j].strTypeName + ")JsonParser::" + vDecs[i].vFields[j].strParseFuncName + "(*pEnter, \"" + vDecs[i].vFields[j].strName + "\",index);\r\n";
                            else
                                strClassDec += "\t\t\t\t" + vDecs[i].vFields[j].strName + " = JsonParser::" + vDecs[i].vFields[j].strParseFuncName + "(*pEnter, \"" + vDecs[i].vFields[j].strName + "\", index);\r\n";
                        }
                    }
                    strClassDec += "\t\t\t}\r\n";
                    strClassDec += "\t\t};\r\n";

                    vDecs[i].strTypeName = "CsvData_" + strClassName + "::" + vDecs[i].strTypeName;
                }
            }
            for (int i = 0; i < vJsons.Count; ++i)
            {
                if (vJsons[i] == null) continue;
                if (vJsons[i].bArray)
                {
                    PropField += "\t\t\tvector<" + vJsons[i].strTypeName + ">\t\t\t" + vJsons[i].strName + ";\r\n";

                    if (string.IsNullOrEmpty(vJsons[i].strParseFuncName))
                    {
                        strParseField += "\t\t\t{\r\n";
                        strParseField += "\t\t\t\trapidjson::Value* pSub = JsonParser::ValueArray(datas,\"" + vJsons[i].strName + "\", i);\r\n";
                        strParseField += "\t\t\t\tif(pSub!=NULL)\r\n";
                        strParseField += "\t\t\t\t{\r\n";
                        strParseField += "\t\t\t\t\tfor(int j = 0; j < (int)(*pSub).Size(); ++j)\r\n";
                        strParseField += "\t\t\t\t\t{\r\n";
                        strParseField += "\t\t\t\t\t\t" + vJsons[i].strTypeName + " tmp;\r\n";
                        strParseField += "\t\t\t\t\t\ttmp.Parse(pSub, j);\r\n";
                        strParseField += "\t\t\t\t\t\tdata->" + vJsons[i].strName + ".push_back(tmp);\r\n";
                        strParseField += "\t\t\t\t\t}\r\n";
                        strParseField += "\t\t\t\t}\r\n";
                        strParseField += "\t\t\t}\r\n";
                    }
                    else
                    {
                        if (vJsons[i].bClassParser)
                        {
                            strParseField += "\t\t\tdata->" + vJsons[i].strName + ".Parse(&datas, i);\r\n";
                        }
                        else
                        {
                            if (vJsons[i].bCast)
                                strParseField += "\t\t\tdata->" + vJsons[i].strName + "= (" + vJsons[i].strTypeName + ")JsonParser::" + vJsons[i].strParseFuncName + "(datas,i," + "\"" + vJsons[i].strName + "\");\r\n";
                            else
                                strParseField += "\t\t\tdata->" + vJsons[i].strName + " = JsonParser::" + vJsons[i].strParseFuncName + "(datas,i," + "\"" + vJsons[i].strName + "\");\r\n";
                        }
                    }
                }
                else if (vJsons[i].bClassParser)
                {
                    PropField += "\t\t\tvector<" + vJsons[i].strTypeName + ">\t\t\t" + vJsons[i].strName + ";\r\n";
                    strParseField += "\t\t\tdata->" + vJsons[i].strName + ".Parse(&datas, i);\r\n";
                }
                else
                {
                    PropField += "\t\t\t" + vJsons[i].strTypeName + "\t\t\t" + vJsons[i].strName + ";\r\n";
                    if (vJsons[i].bCast)
                        strParseField += "\t\t\tdata->" + vJsons[i].strName + "= (" + vJsons[i].strTypeName + ")JsonParser::" + vJsons[i].strParseFuncName + "(datas," + "\"" + vJsons[i].strName + "\",i);\r\n";
                    else
                        strParseField += "\t\t\tdata->" + vJsons[i].strName + " = JsonParser::" + vJsons[i].strParseFuncName + "(datas," + "\"" + vJsons[i].strName + "\",i);\r\n";
                }
            }


            ParseDateTimeFiled(vNewCode, strDateTime);
            ParseClassNameFiled(vNewCode, strClassName);
            ParseDataKeyFiled(vNewCode, binderData.MainKeyType);
            ParseUseKeyFiled(vNewCode, "data->" + binderData.MainKeyName);
            ParsePropFiled(vNewCode, PropField);
            ParseClassDeclFiled(vNewCode, strClassDec);
            ParsePropJsonClassName(vNewCode, JsonClassName);
            ParseDecFunc(vNewCode, "");
            ParseCppFunc(vNewCode, CppFunc);
            ParseFunc(vNewCode, buildFunc);
            ParseFiled(vNewCode, strParseField);
            ParseBindData(vNewCode, binderData.BindFieldDataType.FullName.Replace("[", "").Replace("]", ""));
            ParseBindFieldData(vNewCode, binderData.BindType.FullName);
            ParseBindFieldPropData(vNewCode, binderData.DataFieldName);
            ParseATEnableFiled(vNewCode, bEnableAT);
            return 1;
        }
        //------------------------------------------------------
        static int BuildCodeByJson(SBinderData binderData, List<string> vNewCode, string strDateTime, string strClassName, Dictionary<string, ClassDatas> vClasses, Dictionary<string, List<MappData>> vMapping, bool bEnabelAT = false)
        {
            ParseDateTimeFiled(vNewCode, strDateTime);
            ParseClassNameFiled(vNewCode, strClassName);
            ParseDataKeyFiled(vNewCode, binderData.MainKeyType);
            ParseUseKeyFiled(vNewCode, binderData.MainKeyName);
            ParseBindData(vNewCode, binderData.BindFieldDataType.FullName.Replace("[", "").Replace("]", ""));
            ParseBindFieldData(vNewCode, binderData.BindType.FullName);
            ParseBindFieldPropData(vNewCode, binderData.DataFieldName);
            ParseATEnableFiled(vNewCode, bEnabelAT);
            if (binderData.FieldMapTable != null)
            {
                List<MappData> vDatas = new List<MappData>();
                for (int i = 0; i < binderData.FieldMapTable.Count; ++i)
                {
                    if (string.IsNullOrEmpty(binderData.FieldMapTable[i].strMapFunc) && (binderData.FieldMapTable[i].table == null ||
                        string.IsNullOrEmpty(binderData.FieldMapTable[i].strMapField) ||
                        string.IsNullOrEmpty(binderData.FieldMapTable[i].strMapToField) ||
                        !binderData.FieldMapTable[i].table.Name.Contains("CsvData_")))
                        continue;
                    MappData temp = new MappData();
                    temp.strMapFunc = null;
                    if (binderData.FieldMapTable[i].table != null)
                    {
                        temp.PropName = binderData.FieldMapTable[i].strMapField;
                        temp.strToMapField = binderData.FieldMapTable[i].strMapToField;
                        temp.ClassName = binderData.FieldMapTable[i].table.Name.Replace("CsvData_", "");
                    }
                    else
                    {
                        temp.strMapFunc = binderData.FieldMapTable[i].strMapFunc;
                    }

                    vDatas.Add(temp);
                }
                vMapping.Add(strClassName, vDatas);
            }

            return 1;
        }
        //------------------------------------------------------
        static void BuildCode(string strFile, List<string> vNewCode)
        {
            string dir = strFile.Replace(Path.GetFileName(strFile), "");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(strFile))
                File.Delete(strFile);

            StreamWriter writer = new StreamWriter(strFile, false, System.Text.Encoding.UTF8);
            for (int i = 0; i < vNewCode.Count; ++i)
                writer.Write(vNewCode[i]);
            writer.Close();
        }
        //------------------------------------------------------
        static string ParseHeadTmlLabel(CsvParser Csv)
        {
            string keyFunc = Csv[2][0].String().Trim();
            int tmlHead = keyFunc.IndexOf('@');
            if (tmlHead > 0)
                return keyFunc.Substring(0, tmlHead);
            return "default";
        }
        //------------------------------------------------------
        static bool Parse(CsvParser Csv, List<string> vNewCode, string strDateTime, string strClassName, Dictionary<string, ClassDatas> vMap, Dictionary<string, List<MappData>> vMapping, bool bCpp = false, bool bEnableAT = false)
        {
            try
            {
                string[,] tablPropMap = new string[,] {
                { "string", "String" },
                { "bool", "Bool" },
                { "char", "Char" },
                { "byte", "Byte" },
                { "short", "Short" },
                { "ushort", "Ushort" },
                { "int", "Int" },
                { "uint", "Uint" },
                { "long", "Long" },
                { "ulong", "Ulong" },
                { "float", "Float" },
                { "double", "Double" },
                { "Vector2", "Vec2" },
                { "Vector3", "Vec3" },
                { "Vector4", "Vec4" },
                { "Vector2Int", "Vec2int" },
                { "Vector3Int", "Vec3int" },
                { "byte[]", "ByteArray" },
                { "int[]", "IntArray" },
                { "uint[]", "UintArray" },
                { "float[]", "FloatArray" },
                { "short[]", "ShortArray" },
                { "ushort[]", "UshortArray" },
                { "string[]", "StringArray" },
                { "Vector2[]", "Vec2Array" },
                { "Vector3[]", "Vec3Array" },
                { "Vector2int[]", "Vec2IntArray" },
                { "Vector3int[]", "Vec3IntArray" },
            };
                //! 解析字段类型
                string strPropField = "";
                string[] arCastName = new string[Csv.GetMaxColumn()];
                string[] arPropName = new string[Csv.GetMaxColumn()];
                string[] arPropType = new string[Csv.GetMaxColumn()];
                string[] arParseFunc = new string[Csv.GetMaxColumn()];
                string[] arWriteFunc = new string[Csv.GetMaxColumn()];
                string[] arTableParserFunc = new string[Csv.GetMaxColumn()];
                string[] arTableWriteFunc = new string[Csv.GetMaxColumn()];
                HashSet<string> vPropNames = new HashSet<string>();
                int Validcount = 0;
                for (int i = 0; i < Csv.GetMaxColumn(); ++i)
                {
                    string propTypeName = Csv[1][i].String().Replace("\"", "").Replace("\r", "").Replace("\n", "");
                    if (string.IsNullOrEmpty(propTypeName) || string.IsNullOrEmpty(Csv[3][i].String()))
                    {
                        arPropType[i] = "";
                        arCastName[i] = "";
                        arPropName[i] = "";
                        arParseFunc[i] = "";
                        arWriteFunc[i] = "";
                        arTableWriteFunc[i] = "";
                        arTableParserFunc[i] = "";
                        continue;
                    }
                    string assign_type = "";
                    string[] type_sepc = null;
                    string[] table_type_spec = null;
                    if (propTypeName.ToLower().Contains("table|"))
                    {
                        string strSub = propTypeName.Substring("table|".Length);
                        if (strSub.Length <= 1)
                        {
                            arPropType[i] = "";
                            arCastName[i] = "";
                            arPropName[i] = "";
                            arParseFunc[i] = "";
                            arWriteFunc[i] = "";
                            arTableWriteFunc[i] = "";
                            arTableParserFunc[i] = "";
                            continue;
                        }
                        assign_type = "table";
                        string[] keyValue = strSub.Split('-');
                        if (keyValue.Length == 2 && keyValue[0].Length > 0 && keyValue[1].Length > 0)
                        {
                            table_type_spec = new string[6];
                            string[] keyTypeSpec = keyValue[0].Replace("enum|", "").Split('|');
                            string[] valueTypeSpec = keyValue[1].Replace("enum|", "").Split('|');
                            string spec = "";
                            if (keyTypeSpec.Length == 2)
                            {
                                table_type_spec[0] = keyTypeSpec[0];
                                spec = keyTypeSpec[1];
                            }
                            else
                            {
                                table_type_spec[0] = keyTypeSpec[0];
                            }
                            ConvertType(ref table_type_spec[0]);
                            string strFunc = ConvertTypeToFunc(table_type_spec[0], tablPropMap);
                            if (keyValue[0].Contains("enum|")) strFunc = "Int";
                            if (strFunc == "")
                            {
                                arPropType[i] = "";
                                arCastName[i] = "";
                                arPropName[i] = "";
                                arParseFunc[i] = "";
                                arWriteFunc[i] = "";
                                arTableWriteFunc[i] = "";
                                arTableParserFunc[i] = "";
                                continue;
                            }
                            string strWriteTemp = "";
                            if (keyValue[0].Contains("enum|"))
                            {
                                strWriteTemp = "CsvParser.TableItem.Int((int)tab.Key) + \"-\"";
                                table_type_spec[2] = "(" + table_type_spec[0] + ")" + "CsvParser.TableItem.DefaultTable({0}).Int();";
                            }
                            else
                            {
                                if (spec.Length > 0)
                                    strWriteTemp = "CsvParser.TableItem." + strFunc + "(tab.Value," + spec + ") + \"-\"";
                                else
                                    strWriteTemp = "CsvParser.TableItem." + strFunc + "(tab.Value) + \"-\"";
                                table_type_spec[2] = "CsvParser.TableItem.DefaultTable({0})." + strFunc + "(" + spec + ");";
                            }

                            if (valueTypeSpec.Length == 2)
                            {
                                table_type_spec[3] = valueTypeSpec[0];
                                spec = valueTypeSpec[1];
                            }
                            else
                            {
                                table_type_spec[3] = valueTypeSpec[0];
                            }
                            ConvertType(ref table_type_spec[3]);
                            strFunc = ConvertTypeToFunc(table_type_spec[3], tablPropMap);
                            if (keyValue[1].Contains("enum|")) strFunc = "Int";
                            if (strFunc == "")
                            {
                                arPropType[i] = "";
                                arCastName[i] = "";
                                arPropName[i] = "";
                                arParseFunc[i] = "";
                                arWriteFunc[i] = "";
                                arTableWriteFunc[i] = "";
                                arTableParserFunc[i] = "";
                                continue;
                            }
                            if (keyValue[1].Contains("enum|"))
                            {
                                strWriteTemp += "+ CsvParser.TableItem.Int((int)tab.Value);\r\n";
                                table_type_spec[5] = "(" + table_type_spec[3] + ")" + "CsvParser.TableItem.DefaultTable({0}).Int();";
                            }
                            else
                            {
                                if (spec.Length > 0)
                                    strWriteTemp += "+ CsvParser.TableItem." + strFunc + "(tab.Value," + spec + ");";
                                else
                                    strWriteTemp += "+ CsvParser.TableItem." + strFunc + "(tab.Value);";
                                table_type_spec[5] = "CsvParser.TableItem.DefaultTable({0})." + strFunc + "(" + spec + ");";
                            }
                            table_type_spec[1] = strWriteTemp;
                            type_sepc = new string[] { string.Format("Dictionary<{0},{1}>", keyValue[0].Replace("enum|", ""), keyValue[1].Replace("enum|", "")) };
                        }
                        else
                        {
                            arPropType[i] = "";
                            arCastName[i] = "";
                            arPropName[i] = "";
                            arParseFunc[i] = "";
                            arWriteFunc[i] = "";
                            arTableWriteFunc[i] = "";
                            arTableParserFunc[i] = "";
                            continue;
                        }
                    }
                    else if (propTypeName.ToLower().Contains("enum|"))
                    {
                        string[] assign_spec_type = propTypeName.Split('|');
                        if (assign_spec_type.Length <= 0)
                        {
                            arPropType[i] = "";
                            arCastName[i] = "";
                            arPropName[i] = "";
                            arParseFunc[i] = "";
                            arWriteFunc[i] = "";
                            arTableWriteFunc[i] = "";
                            arTableParserFunc[i] = "";
                            continue;
                        }
                        assign_type = assign_spec_type[0];
                        type_sepc = new string[] { assign_spec_type[1] };
                    }
                    else
                    {
                        int pos = propTypeName.IndexOf('|');
                        if (pos > 0 && pos < propTypeName.Length)
                        {
                            type_sepc = new string[]
                            {
                            propTypeName.Substring(0,pos),
                            propTypeName.Substring(pos+1),
                            };
                        }
                        else
                            type_sepc = new string[] { propTypeName };

                    }
                    string specSign = "";
                    if (type_sepc.Length > 1)
                        specSign = type_sepc[1];

                    ConvertType(ref type_sepc[0]);

                    arPropType[i] = type_sepc[0];
                    if (string.IsNullOrEmpty(arPropType[i]))
                    {
                        arPropType[i] = "";
                        arCastName[i] = "";
                        arPropName[i] = "";
                        arParseFunc[i] = "";
                        arWriteFunc[i] = "";
                        arTableWriteFunc[i] = "";
                        arTableParserFunc[i] = "";
                        continue;
                    }

                    arPropName[i] = Csv[3][i].String().Replace("\r", "").Replace("\n", "");
                    if (string.IsNullOrEmpty(arPropName[i])) continue;
                    if (bCpp)
                    {
                        string strPropType = arPropType[i];
                        if (strPropType.Contains("."))
                        {
                            strPropType = strPropType.Substring(strPropType.IndexOf('.') + 1);
                        }
                        if (strPropType.Contains("[]"))
                        {
                            strPropType = "vector<" + strPropType.Replace("[]", "") + ">";
                        }
                        strPropField += "\t\t\t" + strPropType + "\t\t\t\t\t" + arPropName[i] + ";\r\n";
                    }
                    else
                    {
                        if (bEnableAT)
                            strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + arPropType[i] + "\t\t\t\t" + arPropName[i] + ";\r\n";
                        else
                            strPropField += "\t\t\t" + "public\t" + arPropType[i] + "\t\t\t\t" + arPropName[i] + ";\r\n";
                    }
                    if (vPropNames.Contains(arPropName[i]))
                    {
                        Debug.LogError("表格" + Csv.GetName() + "中具有相同属性名称:" + arPropName[i]);
                        return false;
                    }
                    arCastName[i] = "";
                    vPropNames.Add(arPropName[i]);
                    arParseFunc[i] = "";
                    arWriteFunc[i] = "";
                    if (assign_type.Length > 0)
                    {
                        if (assign_type.ToLower() == "enum")
                        {
                            string propDelcTypeName = arPropType[i];
                            if (bCpp)
                            {
                                if (propDelcTypeName.Contains("."))
                                {
                                    propDelcTypeName = propDelcTypeName.Substring(propDelcTypeName.IndexOf('.') + 1);
                                }
                            }
                            if (arPropType[i].ToLower().Contains("[]"))
                            {
                                string strCast = arPropType[i].Replace("[]", "");
                                arTableParserFunc[i] += "\t\t\t\t{\r\n";
                                if (bCpp)
                                {
                                    arTableParserFunc[i] += "\t\t\t\t\tvector<int> temps = csv[i][\"" + arPropName[i] + "\"].IntArray();\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\tif(temps.size())\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t{\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t\tdata->" + arPropName[i] + ".resize(temps.size());\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t\tmemcpy(&data->" + arPropName[i] + "[0], &temps[0], sizeof(int)*temps.size());\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t}\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t}\r\n";

                                    arTableWriteFunc[i] += "\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\tstring strTableTemp=\"\";\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\tif(it->second->" + arPropName[i] + ".size())\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\tfor(size_t e = 0; e < it->second->" + arPropName[i] + ".size(); ++e)\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t\tstrTableTemp += StringKit::stringFormat(\"%d\",(int)it->second->" + arPropName[i] + "[e]);\r\n";
                                    if (string.IsNullOrEmpty(specSign))
                                        arTableWriteFunc[i] += "\t\t\t\t\t\t\tif(e < it->second->" + arPropName[i] + ".size()-1)strTableTemp +=" + "\"|\"" + ";\r\n";
                                    else
                                        arTableWriteFunc[i] += "\t\t\t\t\t\t\tif(e < it->second->" + arPropName[i] + ".size()-1)strTableTemp +=" + "\"" + specSign + "\"" + ";\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t}\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t}\r\n";
                                    //   arTableWriteFunc[i] += "\t\t\t\t\t\twriter.Write(CsvParser.TableItem.String(strTableTemp));\r\n";c++ writer code
                                    arTableWriteFunc[i] += "\t\t\t\t}\r\n";
                                }
                                else
                                {
                                    arTableParserFunc[i] += "\t\t\t\t\tint[] temps = csv[i][\"" + arPropName[i] + "\"].IntArray();\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\tif(temps !=null)\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t{\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t\tdata." + arPropName[i] + "= new " + strCast + "[temps.Length];\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t\tfor(int e = 0; e < temps.Length; ++e)\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t\t\tdata." + arPropName[i] + "[e] = (" + strCast + ")temps[e];\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t\t}\r\n";
                                    arTableParserFunc[i] += "\t\t\t\t}\r\n";

                                    arTableWriteFunc[i] += "\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\tstring strTableTemp=\"\";\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\tif(db.Value." + arPropName[i] + "!=null)\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\tfor(int e = 0; e < db.Value." + arPropName[i] + ".Length; ++e)\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t{\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t\tstrTableTemp +=((int)db.Value." + arPropName[i] + "[e]).ToString();\r\n";
                                    if (string.IsNullOrEmpty(specSign))
                                        arTableWriteFunc[i] += "\t\t\t\t\t\t\tif(e < db.Value." + arPropName[i] + ".Length-1)strTableTemp +=" + "\"|\"" + ";\r\n";
                                    else
                                        arTableWriteFunc[i] += "\t\t\t\t\t\t\tif(e < db.Value." + arPropName[i] + ".Length-1)strTableTemp +=" + "\"" + specSign + "\"" + ";\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\t}\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t\twriter.Write(CsvParser.TableItem.String(strTableTemp));\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t\t}\r\n";
                                    arTableWriteFunc[i] += "\t\t\t\t}\r\n";
                                }

                            }
                            else
                            {

                                arParseFunc[i] = "Int" + "();";
                                arWriteFunc[i] = "Int((int){0})";
                                arCastName[i] = "(" + propDelcTypeName + ")";
                            }
                        }
                        else if (assign_type.ToLower() == "table")
                        {
                            //                             arTableParserFunc[i] += "\t\t\t\t{\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\tstring[] keyValueGroup = " + "csv[i][\"" + arPropName[i] + "\"].StringArray();\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\tif(keyValueGroup != null && keyValueGroup.Length>0)\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t{\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\tdata." + arPropName[i] + "= new " + arPropType[i] + "(keyValueGroup.Length)" + ";\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\tfor(int t =0; t < keyValueGroup.Length; ++t)\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t{\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\tstring[] item_temps = keyValueGroup[t].Split('|');\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\tif(item_temps.Length==2)\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\t{\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\t\t" + table_type_spec[0] + " temp0 = " + string.Format(table_type_spec[2], "item_temps[0]") + ";\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\t\t" + table_type_spec[3] + " temp1 = " + string.Format(table_type_spec[5], "item_temps[0]") + ";\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\t\tdata." + arPropName[i] + ".Add(temp0, temp1);\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t\t}\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t\t}\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t\t}\r\n";
                            //                             arTableParserFunc[i] += "\t\t\t\t}\r\n";
                            // 
                            //                             arTableWriteFunc[i] += "\t\t\t\t{\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\tstring strTableTemp=\"\";\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\tforeach(var tab in db.Value." + arPropName[i] + ")\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\t{\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\t\tstrTableTemp += " + table_type_spec[1] + "\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\t\tstrTableTemp += \"|\";\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\t}\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\tif(strTableTemp.Length>0 && strTableTemp[strTableTemp.Length-1] == '|') strTableTemp = strTableTemp.Substring(0, strTableTemp.Length-1);\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t\twriter.Write(strTableTemp);\r\n";
                            //                             arTableWriteFunc[i] += "\t\t\t\t}\r\n";
                        }
                    }
                    else
                    {
                        for (int j = 0; j < tablPropMap.GetLength(0); ++j)
                        {
                            if (tablPropMap[j, 0].CompareTo(type_sepc[0]) == 0)
                            {
                                if (specSign.Length > 0)
                                {
                                    arWriteFunc[i] = tablPropMap[j, 1] + "({0}, '" + specSign + "')";
                                    arParseFunc[i] = tablPropMap[j, 1] + "('" + specSign + "');";
                                }
                                else
                                {
                                    arWriteFunc[i] = tablPropMap[j, 1] + "({0})";
                                    arParseFunc[i] = tablPropMap[j, 1] + "();";
                                }
                                break;
                            }
                        }
                    }


                    Validcount++;
                }

                string strSaveField = "";
                string strParseField = "";
                int index = 0;
                for (int i = 0; i < Csv.GetMaxColumn(); ++i)
                {
                    if (string.IsNullOrEmpty(arPropType[i])) continue;
                    if (string.IsNullOrEmpty(arTableParserFunc[i]))
                    {
                        if (bCpp)
                            strParseField += "\t\t\t\t" + "data->" + arPropName[i] + " = " + arCastName[i] + "csv[i][\"" + arPropName[i] + "\"]." + arParseFunc[i] + "\r\n";
                        else
                            strParseField += "\t\t\t\t" + "data." + arPropName[i] + " = " + arCastName[i] + "csv[i][\"" + arPropName[i] + "\"]." + arParseFunc[i] + "\r\n";
                    }
                    else
                    {
                        strParseField += arTableParserFunc[i];
                    }
                    if (string.IsNullOrEmpty(arTableWriteFunc[i]))
                    {
                        if (bCpp)
                            strSaveField += "\t\t\t\t" + ";\r\n";
                        else
                            strSaveField += "\t\t\t\t" + "writer.Write(CsvParser.TableItem." + string.Format(arWriteFunc[i], "db.Value." + arPropName[i]) + ");\r\n";
                    }
                    else
                    {
                        strSaveField += arTableWriteFunc[i];
                    }
                    if (index < Validcount - 1) strSaveField += "\t\t\t\t" + "writer.Write(\",\");\r\n";
                    index++;
                }

                string buildFunc = "";
                List<MappData> vDatas = new List<MappData>();

                string strCppFuncDec = "";

                //! 解析键值
                string keyType = Csv[1][0].String();
                string keyName = bCpp ? ("data->" + Csv[3][0].String()) : ("data." + Csv[3][0].String());
                string keyFunc = Csv[2][0].String().Trim();
                int tmlHead = keyFunc.IndexOf('@');
                bool bValueList = false;
                if (tmlHead >= 0 && tmlHead < keyFunc.Length)
                {
                    if (keyFunc.Substring(0, tmlHead).CompareTo("maplist") == 0) bValueList = true;
                    keyFunc = keyFunc.Substring(tmlHead + 1);
                }
                if (keyFunc.Length > 0)
                {
                    string[] spilt = keyFunc.Split('=');
                    if (spilt.Length == 2)
                    {
                        string strCompareGetCode = "";
                        string funcAgvs = "";
                        string func = spilt[1];
                        bool bValidFunc = false;
                        List<string> replaceKey = new List<string>();
                        for (int f = 0; f < arPropName.Length; ++f)
                        {
                            if (!string.IsNullOrEmpty(arPropName[f]) && func.Contains(arPropName[f]))
                            {
                                for (int s = 0; s < replaceKey.Count;)
                                {
                                    if (arPropName[f].Contains(replaceKey[s]))
                                    {
                                        replaceKey.RemoveAt(s);
                                    }
                                    else ++s;
                                }
                                replaceKey.Add(arPropName[f]);
                            }
                        }
                        for (int f = 0; f < replaceKey.Count; ++f)
                        {
                            string bestPropName = replaceKey[f];
                            if (bCpp)
                            {
                                func = func.Replace(bestPropName, "data->" + bestPropName);
                                bValidFunc = true;
                                funcAgvs += arPropType[f] + " " + bestPropName + ", ";
                                strCompareGetCode += "iter->second[i]." + bestPropName + "==" + bestPropName + ", ";
                            }
                            else
                            {
                                func = func.Replace(bestPropName, "data." + bestPropName);
                                bValidFunc = true;
                                funcAgvs += arPropType[f] + " " + bestPropName + ", ";
                                strCompareGetCode += "outData[i]." + bestPropName + "==" + bestPropName + "&& ";
                            }
                        }
                        if (bValidFunc && funcAgvs.Length > 0)
                        {
                            keyType = spilt[0];
                            keyName = "nTempCombineKey";

                            strParseField += "\t\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + func + ");";

                            if (funcAgvs[funcAgvs.Length - 1] == ' ')
                                funcAgvs = funcAgvs.Substring(0, funcAgvs.Length - 1);

                            if (funcAgvs[funcAgvs.Length - 1] == ',')
                                funcAgvs = funcAgvs.Substring(0, funcAgvs.Length - 1);

                            if (strCompareGetCode.Length > 0)
                            {
                                if (strCompareGetCode[strCompareGetCode.Length - 1] == ' ')
                                    strCompareGetCode = strCompareGetCode.Substring(0, strCompareGetCode.Length - 1);
                                if (strCompareGetCode[strCompareGetCode.Length - 1] == '&')
                                    strCompareGetCode = strCompareGetCode.Substring(0, strCompareGetCode.Length - 1);

                                if (strCompareGetCode[strCompareGetCode.Length - 1] == '&')
                                    strCompareGetCode = strCompareGetCode.Substring(0, strCompareGetCode.Length - 1);

                            }

                            //! 有组合键值，提供组合键值函数获取
                            if (bCpp)
                            {
                                strCppFuncDec = "virtual " + strClassName + "Data* " + " GetData(" + funcAgvs + ");\r\n";
                                buildFunc = "CsvData_" + strClassName + "::" + strClassName + "Data CsvData_" + strClassName + "::GetData(" + funcAgvs + ")\r\n";
                                buildFunc += "\t\t{\r\n";
                                if (bValueList)
                                {
                                    buildFunc += "\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + spilt[1] + ");\r\n";
                                    buildFunc += "\t\t\ttDataMap::iterator iter = m_mData.find( nTempCombineKey );\r\n";
                                    buildFunc += "\t\t\tif (iter!=m_mData.end())\r\n";
                                    buildFunc += "\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\tfor(size_t i = 0; i < iter->second.size(); ++i)\r\n";
                                    buildFunc += "\t\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\t\tif(" + strCompareGetCode + ") return iter->second[i];\r\n";
                                    buildFunc += "\t\t\t\t}\r\n";
                                    buildFunc += "\t\t\t}\r\n";
                                    buildFunc += "\t\t\treturn null;\r\n";
                                }
                                else
                                {
                                    buildFunc += "\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + spilt[1] + ");\r\n";
                                    buildFunc += "\t\t\ttDataMap::iterator iter = m_mData.find( nTempCombineKey );\r\n";
                                    buildFunc += "\t\t\tif (iter!=m_mData.end())\r\n";
                                    buildFunc += "\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\treturn iter->second;\r\n";
                                    buildFunc += "\t\t\t}\r\n";
                                    buildFunc += "\t\t\treturn null;\r\n";
                                }

                                buildFunc += "\t\t}";
                            }
                            else
                            {
                                buildFunc = "public " + strClassName + "Data" + " GetData(" + funcAgvs + ")\r\n";
                                buildFunc += "\t\t{\r\n";
                                buildFunc += "\t\t\t" + keyType + " nTempCombineKey = " + "(" + keyType + ")(" + spilt[1] + ");\r\n";
                                if (bValueList)
                                {
                                    buildFunc += "\t\t\tList<" + strClassName + "Data> outData;\r\n";
                                    buildFunc += "\t\t\tif (m_vData.TryGetValue(nTempCombineKey, out outData))\r\n";
                                    buildFunc += "\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\tfor(int i=0; i < outData.Count; ++i)\r\n";
                                    buildFunc += "\t\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\t\tif(" + strCompareGetCode + ") return outData[i];\r\n";
                                    buildFunc += "\t\t\t\t}\r\n";
                                    buildFunc += "\t\t\t}\r\n";
                                    buildFunc += "\t\t\treturn null;\r\n";
                                }
                                else
                                {
                                    buildFunc += "\t\t\t" + strClassName + "Data outData;\r\n";
                                    buildFunc += "\t\t\tif (m_vData.TryGetValue(nTempCombineKey, out outData))\r\n";
                                    buildFunc += "\t\t\t{\r\n";
                                    buildFunc += "\t\t\t\treturn outData;\r\n";
                                    buildFunc += "\t\t\t}\r\n";
                                    buildFunc += "\t\t\treturn null;\r\n";
                                }
                                buildFunc += "\t\t}";
                            }

                        }
                    }
                }

                //! csv mapping
                strPropField += "\n";
                strPropField += "\t\t\t//mapping data\n";
                if (vMap != null)
                {
                    ClassDatas classData;
                    if (vMap.TryGetValue(strClassName.ToLower(), out classData))
                        classData.maplist = bValueList;

                    for (int i = 1; i < Csv.GetMaxColumn(); ++i)
                    {
                        if (string.IsNullOrEmpty(arPropType[i])) continue;

                        string mappCsv = Csv[2][i].String().Trim();

                        if (mappCsv.Length > 0 && !string.IsNullOrEmpty(arPropName[i]))
                        {
                            mappCsv = mappCsv.Substring(0, 1).ToUpper() + mappCsv.Substring(1, mappCsv.Length - 1);
                            if (!vMap.ContainsKey(mappCsv.ToLower()) || vMap[mappCsv.ToLower()].maplist) continue;
                            MappData temp = new MappData();
                            temp.PropName = arPropName[i];
                            temp.ClassName = mappCsv;
                            temp.ClassType = "CsvData_" + mappCsv + "." + mappCsv + "Data";
                            temp.bArray = arPropType[i].ToLower().Contains("[]");
                            temp.strToMapField = null;
                            vDatas.Add(temp);
                            if (bCpp)
                            {
                                ClassMappingPropety classProperty;
                                if (ms_vClassMappingPropertys.TryGetValue(mappCsv.ToLower(), out classProperty))
                                {
                                    if (temp.bArray)
                                        strPropField += "\t\t\t" + classProperty.propertyClassName + "[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    else
                                        strPropField += "\t\t\t" + classProperty.propertyClassName + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                }
                                else
                                {
                                    if (temp.bArray)
                                        strPropField += "\t\t\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    else
                                        strPropField += "\t\t\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                }
                            }
                            else
                            {
                                ClassMappingPropety classProperty;
                                if (bEnableAT)
                                {
                                    if (ms_vClassMappingPropertys.TryGetValue(mappCsv.ToLower(), out classProperty))
                                    {
                                        if (temp.bArray)
                                            strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + classProperty.propertyClassName + "[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                        else
                                            strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + classProperty.propertyClassName + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    }
                                    else
                                    {
                                        if (temp.bArray)
                                            strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                        else
                                            strPropField += "\t\t\t" + "//[Framework.Plugin.AT.ATField(\"\",null,\"\",1)]public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    }
                                }
                                else
                                {
                                    if (ms_vClassMappingPropertys.TryGetValue(mappCsv.ToLower(), out classProperty))
                                    {
                                        if (temp.bArray)
                                            strPropField += "\t\t\t" + "public\t" + classProperty.propertyClassName + "[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                        else
                                            strPropField += "\t\t\t" + "public\t" + classProperty.propertyClassName + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    }
                                    else
                                    {
                                        if (temp.bArray)
                                            strPropField += "\t\t\t" + "public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data[]" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                        else
                                            strPropField += "\t\t\t" + "public\t" + "CsvData_" + mappCsv + "." + mappCsv + "Data" + " " + mappCsv + "_" + arPropName[i] + "_data" + ";\r\n";
                                    }
                                }
                            }
                        }
                    }
                }

                if (vMapping != null) vMapping.Add(strClassName, vDatas);

                ParseDateTimeFiled(vNewCode, strDateTime);
                ParseClassNameFiled(vNewCode, strClassName);
                ParsePropFiled(vNewCode, strPropField);
                ParseDataKeyFiled(vNewCode, keyType);
                ParsePropKeyFiled(vNewCode, Csv[3][0].String());
                ParseUseKeyFiled(vNewCode, keyName);
                ParseFunc(vNewCode, buildFunc);
                ParseClassDeclFiled(vNewCode, "");
                ParseDecFunc(vNewCode, strCppFuncDec);
                ParseFiled(vNewCode, strParseField);
                SaveFiled(vNewCode, strSaveField);
                ParseATEnableFiled(vNewCode, bEnableAT);
                if (bCpp)
                    ParseCppFunc(vNewCode, "//no auto func");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(strClassName + " parse failed!");
                Debug.LogError(ex.StackTrace);
                return false;
            }
            return true;
        }
        //------------------------------------------------------
        static string ConvertTypeToFunc(string strType, string[,] tablPropMap)
        {
            for (int j = 0; j < tablPropMap.GetLength(0); ++j)
            {
                if (tablPropMap[j, 0].CompareTo(strType) == 0)
                {
                    return tablPropMap[j, 1];
                }
            }
            return "";
        }
        //------------------------------------------------------
        static void ConvertType(ref string strType)
        {
            if (strType.ToLower().CompareTo("vector2") == 0) strType = "Vector2";
            else if (strType.ToLower().CompareTo("vector3") == 0) strType = "Vector3";
            else if (strType.ToLower().CompareTo("vector4") == 0) strType = "Vector4";
            else if (strType.ToLower().CompareTo("vector2int") == 0) strType = "Vector2Int";
            else if (strType.ToLower().CompareTo("vector3int") == 0) strType = "Vector3Int";
            else if (strType.ToLower().CompareTo("vector2int[]") == 0) strType = "Vector2Int[]";
            else if (strType.ToLower().CompareTo("vector3int[]") == 0) strType = "Vector3Int[]";
            else if (strType.ToLower().CompareTo("vector2[]") == 0) strType = "Vector2[]";
            else if (strType.ToLower().CompareTo("vector3[]") == 0) strType = "Vector3[]";
            else if (strType.ToLower().CompareTo("vec2") == 0) strType = "Vector2";
            else if (strType.ToLower().CompareTo("vec3") == 0) strType = "Vector3";
            else if (strType.ToLower().CompareTo("vec4") == 0) strType = "Vector4";
            else if (strType.ToLower().CompareTo("vec2int") == 0) strType = "Vector2Int";
            else if (strType.ToLower().CompareTo("vec3int") == 0) strType = "Vector3Int";
            else if (strType.ToLower().CompareTo("vec2int[]") == 0) strType = "Vector2Int[]";
            else if (strType.ToLower().CompareTo("vec3int[]") == 0) strType = "Vector3Int[]";
            else if (strType.ToLower().CompareTo("vec2[]") == 0) strType = "Vector2[]";
            else if (strType.ToLower().CompareTo("vec3[]") == 0) strType = "Vector3[]";
            else if (strType.ToLower().CompareTo("vec2i") == 0) strType = "Vector2Int";
            else if (strType.ToLower().CompareTo("vec3i") == 0) strType = "Vector3Int";
            else if (strType.ToLower().CompareTo("vec2i[]") == 0) strType = "Vector2Int[]";
            else if (strType.ToLower().CompareTo("vec3i[]") == 0) strType = "Vector3Int[]";
        }
        //------------------------------------------------------
        static void ParseDateTimeFiled(List<string> vLine, string dateTime)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#DATE_TIME#"))
                {
                    vLine[i] = vLine[i].Replace("#DATE_TIME#", dateTime);
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseClassNameFiled(List<string> vLine, string strClassName)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#CLASS_NAME#"))
                {
                    vLine[i] = vLine[i].Replace("#CLASS_NAME#", strClassName);
                }
            }
        }
        //------------------------------------------------------
        static void ParseDataKeyFiled(List<string> vLine, string strDateKey)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#DATA_KEY#"))
                {
                    vLine[i] = vLine[i].Replace("#DATA_KEY#", strDateKey);
                }
            }
        }
        //------------------------------------------------------
        static void ParsePropKeyFiled(List<string> vLine, string strPropKey)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#PROP_KEY#"))
                {
                    vLine[i] = vLine[i].Replace("#PROP_KEY#", strPropKey);
                }
            }
        }
        //------------------------------------------------------
        static void ParseUseKeyFiled(List<string> vLine, string strPropKey)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#USE_KEY#"))
                {
                    vLine[i] = vLine[i].Replace("#USE_KEY#", strPropKey);
                }
            }
        }
        //------------------------------------------------------
        static void ParseFunc(List<string> vLine, string strFuncs)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#GENE_FUNCS#") == 0)
                {
                    vLine[i] = vLine[i].Replace("#GENE_FUNCS#", strFuncs);
                    if (strFuncs.Length <= 0)
                    {
                        vLine[i] = "";
                        if (i > 0 && vLine[i - 1].Trim().CompareTo("//-------------------------------------------") == 0)
                        {
                            vLine[i - 1] = "";
                        }
                    }
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseDecFunc(List<string> vLine, string strFuncs)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#GENECPP_DEC_FUNCS#") == 0)
                {
                    vLine[i] = vLine[i].Replace("#GENECPP_DEC_FUNCS#", strFuncs);
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseCppFunc(List<string> vLine, string strFuncs)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#GENECPP_FUNCS#") == 0)
                {
                    vLine[i] = vLine[i].Replace("#GENECPP_FUNCS#", strFuncs);
                    if (strFuncs.Length <= 0)
                    {
                        vLine[i] = "";
                        if (i > 0 && vLine[i - 1].Trim().CompareTo("//-------------------------------------------") == 0)
                        {
                            vLine[i - 1] = "";
                        }
                    }
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParsePropFiled(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#PROP_FIELD#") == 0 ||
                    vLine[i].Trim().CompareTo("//#PROP_FIELD#") == 0)
                {
                    vLine[i] = strPropFiled;
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseClassDeclFiled(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#CLASS_DECL#"))
                {
                    vLine[i] = vLine[i].Replace("#CLASS_DECL#", strPropFiled);
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParsePropJsonClassName(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#JSON_CLASS_NAME#"))
                {
                    vLine[i] = vLine[i].Replace("#JSON_CLASS_NAME#", strPropFiled);
                }
            }
        }
        //------------------------------------------------------
        static void ParseFiled(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#PARSE_FIELD#") == 0)
                {
                    vLine[i] = strPropFiled;
                    //         break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseATEnableFiled(List<string> vLine, bool bEnable)
        {
            if (!bEnable)
            {
                for (int i = 0; i < vLine.Count;)
                {
                    if (vLine[i].Trim().Contains("[Framework.Plugin.AT.ATExportMono(") ||
                        vLine[i].Trim().Contains("[Framework.Plugin.AT.ATField(") ||
                        vLine[i].Trim().Contains("[Framework.Plugin.AT.ATMethod("))
                    {
                        vLine.RemoveAt(i);
                    }
                    else
                        ++i;
                }
            }

        }
        //------------------------------------------------------
        static void ParseBindData(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#BIND_DATA#"))
                {
                    vLine[i] = vLine[i].Replace("#BIND_DATA#", strPropFiled);
                }
            }
        }
        //------------------------------------------------------
        static void ParseBindFieldData(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#DATA_FIELD_TYPE#"))
                {
                    vLine[i] = vLine[i].Replace("#DATA_FIELD_TYPE#", strPropFiled);
                }
            }
        }
        //------------------------------------------------------
        static void ParseBindFieldPropData(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().Contains("#ARRAY_DATA_FIELD#"))
                {
                    vLine[i] = vLine[i].Replace("#ARRAY_DATA_FIELD#", strPropFiled);
                }
            }
        }
        //------------------------------------------------------
        static void SaveFiled(List<string> vLine, string strPropFiled)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#SAVE_FIELD#") == 0)
                {
                    vLine[i] = strPropFiled;
                    break;
                }
            }
        }
        //------------------------------------------------------
        static void ParseATExportData(List<string> vLine, string atLine)
        {
            for (int i = 0; i < vLine.Count; ++i)
            {
                if (vLine[i].Trim().CompareTo("#AT_EXPORT_LINE#") == 0)
                {
                    vLine[i] = atLine;
                    break;
                }
            }
        }
    }
}

