/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideAutoCode
作    者:	
描    述:	引导系统自动代码生成
*********************************************************************/
#if UNITY_EDITOR
using Framework.Guide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TagLib.Asf;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    public class GuideAutoCode
    {
      //  [MenuItem("Tools/引导系统/导出代码", false, -1)]
        public static void AutoCode(string strCodeFile)
        {
            string code = "//auto generator\r\n";
            code += "using UnityEngine;\r\n";
            code += "namespace Framework.Guide\r\n";
            code += "{\r\n";
            code += "\tpublic class GuideWrapper\r\n";
            code += "\t{\r\n";

            code += "\t\tstatic VariablePort ms_vPortCaches = null;\r\n";
            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static bool bDoing\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tget{return GuideSystem.getInstance().bDoing;}\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void AddGuideGuid(int guid, GuideGuid widget)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif(widget== null) return;\r\n";
            code += "\t\t\twidget.guid = guid;\r\n"; 
            code += "\t\t\tFramework.Guide.GuideGuidUtl.OnAdd(widget);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void ClearCache()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif(ms_vPortCaches != null) ms_vPortCaches.Clear();\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void SetPortInt(byte index, int nValue)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif(ms_vPortCaches == null) ms_vPortCaches = new VariablePort();\r\n";
            code += "\t\t\tms_vPortCaches.SetInt(index,nValue);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void SetPortString(byte index, string strValue)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\t if(ms_vPortCaches == null) ms_vPortCaches = new VariablePort();\r\n";
            code += "\t\t\tms_vPortCaches.SetString(index, strValue);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tstatic VariableList BuildArgvList(bool bUsedClear = true)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tVariableList arvgList = VariableList.Get();\r\n";
            code += "\t\t\tif(ms_vPortCaches != null)\r\n";
            code += "\t\t\t{\r\n";
            code += "\t\t\t\tms_vPortCaches.Build(arvgList);\r\n";
            code += "\t\t\t\tif(bUsedClear) ms_vPortCaches.Clear();\r\n";
            code += "\t\t\t}\r\n";
            code += "\t\t\treturn arvgList;\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void FillNodePort(BaseNode pNode, bool bUsedClear = true)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tif (pNode == null || ms_vPortCaches == null) return;\r\n";
            code += "\t\t\tpNode.FillArgv(ms_vPortCaches);\r\n";
            code += "\t\t\tif(bUsedClear) ms_vPortCaches.Clear();\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void OnTouchBegin(int touchId, Vector2 position, Vector2 deltaPosition)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnTouchBegin(touchId, position, deltaPosition);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void OnTouchMove(int touchId, Vector2 position, Vector2 deltaPosition)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnTouchMove(touchId, position, deltaPosition);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void OnTouchEnd(int touchId, Vector2 position, Vector2 deltaPosition)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnTouchEnd(touchId, position, deltaPosition);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void OnOptionStepCheck()\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OverOptionState();\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void OnCustomCallback(int customType, int userData)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().OnCustomCallback(customType, userData,  BuildArgvList());\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static void DoGuide(int guid, int state, BaseNode pStartNode = null, bool bForce = false)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\tGuideSystem.getInstance().DoGuide(guid,state,pStartNode,bForce);\r\n";
            code += "\t\t}\r\n";

            code += "\t\t//--------------------------------------------------\r\n";
            code += "\t\tpublic static bool OnCustomTrigger(int customType, bool bForce = false)\r\n";
            code += "\t\t{\r\n";
            code += "\t\t\treturn GuideSystem.getInstance().OnTrigger(customType, null, bForce,  BuildArgvList());\r\n";
            code += "\t\t}\r\n";

            try
            {
                GuideSystemEditor.InitDisplayAttr();
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
                                string strCallArgvCode = "\t\t\tvar argvList = VariableList.Get();\r\n";
                                string strArgv = "";
                                string strTransArgv = "";
                                if (argvs != null && argvs.Length > 0)
                                {
                                    int mustReq = 0;
                                    for (int i = 0; i < argvs.Length; ++i)
                                    {
                                        bool bString = false;
                                        if (argvs[i].displayType == typeof(string))
                                            bString = true;
                                        if(!bString && argvs[i].displayType!=null && GuideSystemEditor.TypeDisplayTypes.TryGetValue(argvs[i].displayType, out var displayTypeType) && (displayTypeType.bStrValue || displayTypeType.displayType == typeof(string)))
                                        {
                                            bString = true;
                                        }
                                        if (!bString && !string.IsNullOrEmpty(argvs[i].dispayTypeName) && GuideSystemEditor.DisplayTypes.TryGetValue(argvs[i].dispayTypeName, out displayTypeType) && (displayTypeType.bStrValue || displayTypeType.displayType == typeof(string)))
                                        {
                                            bString = true;
                                        }
                                        if (bString)
                                        {
                                            strArgv += "string " + argvs[i].argvName;
                                            strCallArgvCode += "\t\t\targvList.AddString(" + argvs[i].argvName + ");\r\n";
                                        }
                                        else
                                        {
                                            strArgv += "int " + argvs[i].argvName;
                                            strCallArgvCode += "\t\t\targvList.AddInt(" + argvs[i].argvName + ");\r\n";
                                        }
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
                                    strFunc += "\t\t//--------------------------------------------------\r\n";
                                    if (strArgv.Length > 0)
                                        strFunc += "\t\tpublic static bool On" + strName + "(" + strArgv + ")\r\n";
                                    else
                                        strFunc += "\t\tpublic static bool On" + strName + "()\r\n";
                                    strFunc += "\t\t{\r\n";
                                    strFunc += "\t\t//" + enumType.FullName + "." + v.ToString() + "\r\n";
                                    if (strArgv.Length > 0)
                                    {
                                        strFunc += strCallArgvCode;
                                        strFunc += "\t\t\treturn GuideSystem.getInstance().OnTrigger(" + flagValue + ", null, false, argvList);\r\n";
                                    }
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

                string dir = Path.GetDirectoryName(strCodeFile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                FileStream fs = new FileStream(strCodeFile, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
                fs.Position = 0;
                fs.SetLength(0);
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