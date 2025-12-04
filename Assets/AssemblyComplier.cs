#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public class AssemblyComplier
{
    static HashSet<string> ms_vPackageDlls = new HashSet<string>();
    [MenuItem("Tools/AssemblyComplier")]
    public static void CompileAssembly()
    {
        ms_vPackageDlls.Clear();
        ms_vPackageDlls.Add("agenttree");
        ms_vPackageDlls.Add("guidesystem");
        ms_vPackageDlls.Add("gameframework");
        ms_vPackageDlls.Add("gamestate");
        ms_vPackageDlls.Add("cutscene");
        ms_vPackageDlls.Add("actorsystem");
        //ComplierDll((succeed4) =>
        //{
        //},
        //Application.dataPath + "/../Packages/GamePlay/Scripts/Cutscene", 
        //Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/CutsceneEditor.dll",
        //"UNITY_EDITOR",
        //                   Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/TagLibSharp.dll",
        //                   Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll",
        //                   Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/AgentTreeEditor.dll");
        //return;
        BuildEditorDll((result) => {
            if(result)
            {
                BuildDll((resutl1) => {
                    EditorUtility.DisplayDialog("提示", (result && resutl1) ? "DLL编译成功" : "DLL编译失败", "确定");
                });
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "DLL编译失败", "确定");
            }
        });
    }
    //--------------------------------------------------------
    static void BuildEditorDll(System.Action<bool> onCallback)
    {
        ComplierDll((bSucceed) => {
            if (bSucceed)
            {
                ComplierDll((succeed1) =>
                {
                    if (succeed1)
                    {
                        ComplierDll((succeed2) =>
                        {
                            if (succeed2)
                            {
                                ComplierDll((succeed3) =>
                                {
                                    if (succeed3)
                                    {
                                        ComplierDll((succeed4) =>
                                        {
                                            onCallback(succeed4);
                                        },
                                        Application.dataPath + "/../Packages/GamePlay/Scripts/GuideSystem", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GuideSystemEditor.dll", "UNITY_EDITOR",
                                        Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll");
                                    }
                                    else
                                        onCallback(false);
                                },
                                Application.dataPath + "/../Packages/GamePlay/Scripts/ActorSystem", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/ActorSystemEditor.dll", "UNITY_EDITOR",
                                Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll",
                                Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/AgentTreeEditor.dll",
                                Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/CutsceneEditor.dll");
                            }
                            else
                                onCallback(false);
                        },
                        Application.dataPath + "/../Packages/GamePlay/Scripts/Cutscene", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/CutsceneEditor.dll", "UNITY_EDITOR",
                           Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/TagLibSharp.dll",
                           Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll",
                           Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/AgentTreeEditor.dll");
                    }
                    else
                        onCallback(false);
                },
                Application.dataPath + "/../Packages/GamePlay/Scripts/AgentTree", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/AgentTreeEditor.dll", "UNITY_EDITOR",
                Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll");
            }
            else
                onCallback(false);
        }, Application.dataPath + "/../Packages/GamePlay/Scripts/GameFramework", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFrameworkEditor.dll", "UNITY_EDITOR");
    }
    //--------------------------------------------------------
    static void BuildDll(System.Action<bool> onCallback)
    {
        ComplierDll((bSucceed) => {
            if (bSucceed)
            {
                ComplierDll((succeed1) =>
                {
                    if (succeed1)
                    {
                        ComplierDll((succeed2) =>
                        {
                            if (succeed2)
                            {
                                ComplierDll((succeed3) =>
                                {
                                    if (succeed3)
                                    {
                                        ComplierDll((succeed4) =>
                                        {
                                            onCallback(succeed4);
                                        },
                                        Application.dataPath + "/../Packages/GamePlay/Scripts/GuideSystem", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GuideSystem.dll");
                                    }
                                    else
                                        onCallback(false);
                                },
                                Application.dataPath + "/../Packages/GamePlay/Scripts/ActorSystem", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/ActorSystem.dll");
                            }
                            else
                                onCallback(false);
                        },
                        Application.dataPath + "/../Packages/GamePlay/Scripts/Cutscene", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/Cutscene.dll");
                    }
                    else
                        onCallback(false);
                },
                Application.dataPath + "/../Packages/GamePlay/Scripts/AgentTree", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/AgentTree.dll");
            }
            else
                onCallback(false);
        }, Application.dataPath + "/../Packages/GamePlay/Scripts/GameFramework", Application.dataPath + "/../Plugins/GamePlay/Scripts/Framework/GameFramework.dll");
    }
    //--------------------------------------------------------
    public static void ComplierDll(System.Action<bool> onCallback, string sourceDir, string outputDll, string defineSymbols = "", params string[] referenceDlls)
    {
        var scripts = Directory.GetFiles(sourceDir, "*.cs", SearchOption.AllDirectories);
        if (scripts.Length == 0)
        {
            UnityEngine.Debug.LogError("未找到C#文件: " + sourceDir);
            return;
        }
        var references = new List<string>();
        var unityEnginePath = Path.Combine(EditorApplication.applicationContentsPath, "Managed");
        var unityEditorPath = Path.Combine(EditorApplication.applicationContentsPath, "Managed");
        if (defineSymbols.Contains("UNITY_EDITOR"))
        {
            references.Add(Path.Combine(unityEditorPath, "UnityEngine/UnityEditor.UIElementsModule.dll"));
            references.Add(Path.Combine(unityEditorPath, "UnityEngine/UnityEditor.CoreModule.dll"));
            references.Add(Path.Combine(unityEditorPath, "UnityEngine/UnityEditor.GraphViewModule.dll"));
            references.Add(Path.Combine(unityEditorPath, "UnityEngine/UnityEditor.dll"));
            references.Add(Path.Combine(unityEditorPath, "UnityEditor.dll"));
        }

        if (referenceDlls!=null)
        {
            references.AddRange(referenceDlls);
        }

        var builder = new AssemblyBuilder(outputDll, scripts)
        {
            buildTarget = EditorUserBuildSettings.activeBuildTarget,
            buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget),
            additionalDefines = defineSymbols.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray(),
            additionalReferences = references.ToArray()
        };
        builder.compilerOptions.AllowUnsafeCode = true;
        builder.referencesOptions = ReferencesOptions.UseEngineModules;
         builder.compilerOptions.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_Standard;

        List<string> excludeReferences = new List<string>();
        if (referenceDlls != null && referenceDlls.Length>0)
        {
            Dictionary<string, string> vRefDlls = new Dictionary<string, string>();
            for(int i =0; i < builder.defaultReferences.Length; ++i)
            {
                string name = Path.GetFileNameWithoutExtension(builder.defaultReferences[i]).ToLower();
                vRefDlls[name] = builder.defaultReferences[i];
              //  if(builder.defaultReferences[i].StartsWith("Library/ScriptAssemblies"))
                if(ms_vPackageDlls.Contains(name))
                {
                    excludeReferences.Add(builder.defaultReferences[i]);
                }
            }
            for(int i =0; i < referenceDlls.Length; ++i)
            {
                string name = Path.GetFileNameWithoutExtension(referenceDlls[i]);
                if (!name.EndsWith("Editor"))
                    continue;
                name = name.Substring(0, name.Length - "Editor".Length).ToLower();
                if (vRefDlls.TryGetValue(name, out var dllPath))
                {
                    if(!excludeReferences.Contains(dllPath))
                        excludeReferences.Add(dllPath);
                }
            }
        }
        if(excludeReferences.Count>0)
            builder.excludeReferences = excludeReferences.ToArray();


        string dllName = Path.GetFileNameWithoutExtension(outputDll);

        builder.buildFinished += (path, result) =>
        {
            bool bSucceed = true;
            for(int i =0; i < result.Length; ++i)
            {
                if (result[i].type == CompilerMessageType.Error)
                {
                    bSucceed = false;
                    try
                    {
                        UnityEngine.Debug.LogError(dllName + "  编译失败: " + path + "\n" + string.Join("\n", result[i].message));
                    }
                    catch
                    {

                    }
                }
            }
            if(bSucceed)
            {
                UnityEngine.Debug.Log(dllName + "  DLL编译成功");
            }
            onCallback(bSucceed);
        };

        if (!builder.Build())
        {
            UnityEngine.Debug.LogError(dllName + "  编译启动失败");
            onCallback(false);
        }
    }
}
#endif