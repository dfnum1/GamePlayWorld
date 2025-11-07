#if UNITY_EDITOR && USE_GUIDESYSTEM
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.Guide
{

    public class GuideAutoCode
    {
      //  [MenuItem("Tools/引导系统/导出代码", false, -1)]
        public static void AutoCode(string root = "Scripts/GameMain/Generators/GuideRuntime")
        {
            string strCodeFile = Path.Combine(Application.dataPath, root, "GuideWrapper.cs");
            string code = "//auto generator\r\n";
            code += "#if USE_GUIDESYSTEM\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Plugin.Guide\r\n";
            code += "{\r\n";
            code += "\tpublic class GuideWrapper\r\n";
            code += "\t{\r\n";

            code += "\t\tpublic static bool bDoing\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tget{return GuideSystem.getInstance().bDoing;}\r\n";
            code += "\t\t}\r\n";
            code += "\t\tpublic static void OnOptionStepCheck()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OverOptionState();\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic static void OnUIWidgetTrigger(int widgetGuid, int listIndex, EUIWidgetTriggerType type, params IUserData[] argvs)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnUIWidgetTrigger(widgetGuid, listIndex, type, argvs);\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic static void OnCustomCallback(int customType, int userData, params IUserData[] argvs)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnCustomCallback(customType, userData, argvs);\r\n";
            code += "\t\t}\r\n";

            code += "\t\tpublic static void DoGuide(int guid, int state, BaseNode pStartNode = null, bool bForce = false)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().DoGuide(guid,state,pStartNode,bForce);\r\n";
            code += "\t\t}\r\n";

            try
            {
                string strTypArgvCount = "";
                string strTypeMustReqArgvCount = "";
                string strTypeMappingCode = "";
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types = assembly.GetTypes();
                    for (int t = 0; t < types.Length; ++t)
                    {
                        if (!types[t].IsEnum) continue;
                        System.Type enumType = types[t];
                        if (!enumType.IsDefined(typeof(GuideExportAttribute)))
                            continue;
                        foreach (var v in Enum.GetValues(enumType))
                        {
                            string strFunc = "";
                            string strName = Enum.GetName(enumType, v);
                            FieldInfo fi = enumType.GetField(strName);
                            int flagValue = (int)v;
                            GuideTriggerAttribute aiDeclar;
                            if (fi.IsDefined(typeof(GuideTriggerAttribute)))
                            {
                                aiDeclar = fi.GetCustomAttribute<GuideTriggerAttribute>();

                                GuideArgvAttribute[] argvs = (GuideArgvAttribute[])fi.GetCustomAttributes(typeof(GuideArgvAttribute));
                                string strArgv = "";
                                string strTransArgv = "";
                                if (argvs != null && argvs.Length > 0)
                                {
                                    int mustReq = 0;
                                    for (int i = 0; i < argvs.Length; ++i)
                                    {
                                        strArgv += "int " + argvs[i].argvName;
                                        strTransArgv += argvs[i].argvName;
                                        if (i < argvs.Length - 1)
                                        {
                                            strTransArgv += ", ";
                                            strArgv += ",";
                                        }
                                    }
                                    strTypeMustReqArgvCount += "\t\t\t\tcase " + flagValue + ": return " + mustReq + ";\r\n";
                                    strTypArgvCount += "\t\t\t\tcase " + flagValue + ": return " + argvs.Length + ";\r\n";
                                }
              //                  if (!fi.IsDefined(typeof(AIInTimerAttribute)))
                                {
                                    if (strArgv.Length > 0)
                                        strFunc += "\t\tpublic static bool On" + strName + "(" + strArgv + ")\r\n";
                                    else
                                        strFunc += "\t\tpublic static bool On" + strName + "()\r\n";
                                    strFunc += "\t\t{\r\n";
                                    strFunc += "\t\t//" + enumType.FullName + "." + v.ToString() + "\r\n";
                                    if (strArgv.Length > 0)
                                        strFunc += "\t\t\treturn GuideSystem.getInstance().OnTrigger(" + flagValue + ", null, false, " + strTransArgv+");\r\n";
                                    else
                                        strFunc += "\t\t\treturn GuideSystem.getInstance().OnTrigger(" + flagValue + ", null, false);\r\n";
                                    strFunc += "\t\t}\r\n";
                                }
                               

                                if(!string.IsNullOrEmpty(aiDeclar.DisplayName) )
                                    strTypeMappingCode += "\t\t\tcase " + flagValue + ": return \"" + aiDeclar.DisplayName + "\";\r\n";
                                else
                                    strTypeMappingCode += "\t\t\tcase " + flagValue + ": return \"" + strName + "\";\r\n";
                            }

                            if (strFunc.Length > 0)
                            {
                                code += strFunc;
                            }
                        }
                    }
                }


                code += "\t}\r\n";
                code += "}\r\n";
                code += "#endif\r\n";

                string dir = Path.GetDirectoryName(strCodeFile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                FileStream fs = new FileStream(strCodeFile, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                fs.SetLength(0);
                fs.Position = 0;
                writer.Write(code);
                writer.Close();
            }
            catch (System.Exception ex)
            {
            	
            }
        }
    }
}
#endif