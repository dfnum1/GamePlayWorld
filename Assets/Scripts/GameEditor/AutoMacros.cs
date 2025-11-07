/********************************************************************
生成日期:	24:7:2019   11:12
类    名: 	AutoMarcros
作    者:	HappLI
描    述:	自动定义
*********************************************************************/
using Framework.ED;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TopGame.ED
{
    public static class AutoMarcros
    {
        public static string[] VersionMacros = new string[]
        {
           // "USE_ACTORSYSTEM",
             "USE_TIMELINE",
             "USE_URP",
             "USE_UNITY_JOB",
             "USE_REPORTVIEW",
             "USE_TMPRO",
           // "USE_FMOD",
           // "UNITY_TIMELINE_EXIST",
            //"USE_PICO",
            //"USE_VR",
          //  "USE_DIYLEVEL"
        //   "USE_HOTCODE",
       //     "UNITY_POST_PROCESSING_STACK_V2",
        //    "USE_LITMOTION",
        };
        //------------------------------------------------------
        public static void SetMacros(string[] macros)
        {
            string defineCommand = "";

            HashSet<string> vSets = new HashSet<string>();
            for (int i = 0; i < AutoMarcros.VersionMacros.Length; i++)
            {
                defineCommand += AutoMarcros.VersionMacros[i] + ";";
                vSets.Add(AutoMarcros.VersionMacros[i].ToLower());
            }
            var pluginMarcos = PluginManager.GetPluginMarcoWords();
            foreach(var db in pluginMarcos)
            {
                if (!db.Value) continue;
                if(vSets.Contains(db.Key))
                {
                    continue;
                }
                defineCommand += db.Key + ";";
                vSets.Add(db.Key.ToLower());
            }
            if(macros!=null)
            {
                for(int i= 0; i < macros.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(macros[i]))
                    {
                        defineCommand += macros[i] + ";";
                        vSets.Add(macros[i].ToLower());
                    }
                }
            }
            bool bRefresh = false;
            string curDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if(curDefine.CompareTo(defineCommand) != 0)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineCommand);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineCommand);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineCommand);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineCommand);

                bRefresh = true;
            }

            if(EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.iOS)
            {
                //var assets = AssetDatabase.FindAssets("t:GameQuality");
                //if (assets != null && assets.Length > 0)
                //{
                //    Data.GameQuality quality = AssetDatabase.LoadAssetAtPath<Data.GameQuality>(AssetDatabase.GUIDToAssetPath(assets[0]));
                //    int index = (int)Data.EGameQulity.High;
                //    if (quality.Configs != null && index < quality.Configs.Length)
                //    {
                //        Data.QualityConfig cfg = quality.Configs[index];
                //        cfg.MSAA = false;
                //        cfg.AntiAliasing = Data.QualityConfig.EAntiSamplingType.Disable;
                //        if (cfg.urpAsset)
                //        {
                //            if (cfg.urpAsset.msaaSampleCount != 0 || cfg.urpAsset.supportsCameraDepthTexture == true)
                //            {
                //                cfg.urpAsset.supportsCameraDepthTexture = false;
                //                cfg.urpAsset.msaaSampleCount = 0;
                //                EditorUtility.SetDirty(cfg.urpAsset);
                //            }
                //        }
                //        EditorUtility.SetDirty(quality);
                //    }
                //    bRefresh = true;
                //}
            }
            if(bRefresh)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
        public static void ApplayMarco(string marco)
        {
            if (marco.CompareTo("USE_VR") == 0)
            {
                string asmdefFile = Application.dataPath + "/Scripts/GameMain/GameMain.asmdef";
                if (File.Exists(asmdefFile))
                {
                    string strContext = File.ReadAllText(asmdefFile, System.Text.Encoding.UTF8);
                    if (strContext.Contains("\"Framework\","))
                    {
                        bool bDirty = false;
                        string strRefs = "\"Framework\",\r\n";
                        if (!strContext.Contains("Unity.XR.Interaction.Toolkit"))
                        {
                            strRefs += "\"Unity.XR.Interaction.Toolkit\",\r\n";
                            bDirty = true;
                        }
                        if (bDirty)
                        {
                            strContext = strContext.Replace("\"Framework\",", strRefs);
                            File.WriteAllText(asmdefFile, strContext, System.Text.Encoding.UTF8);
                        }
                    }
                }
            }
            if(marco.CompareTo("USE_PICO") == 0)
            {
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var PackageManagerWindow = assembly.GetType("UnityEditor.PackageManager.UI.PackageManagerWindow");
                    if (PackageManagerWindow != null)
                    {
                        var realType = assembly.GetType("UnityEditor.PackageManager.UI.PackageDatabase");
                        if(realType!=null)
                        {
                            var propertyN = realType.GetMethod("get_instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                            if (propertyN != null)
                            {
                                var obj = propertyN.Invoke(null, null);
                                var AddByPath = realType.GetMethod("InstallFromPath", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                if (AddByPath != null)
                                {
                                    //  AddByPath.Invoke(obj, );
                                }

                                var PackageManagerWindowAnalytics = assembly.GetType("UnityEditor.PackageManager.UI.PackageManagerWindowAnalytics");
                                if (PackageManagerWindowAnalytics != null)
                                {
                                    var SendEvent = PackageManagerWindowAnalytics.GetMethod("SendEvent", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                                    //      if (SendEvent != null) SendEvent.Invoke(null, new System.Object[] { "addFromDisk", null });
                                }
                            }
                        }
                        
                        break;
                    }
                }
            }
        }
    }
}

