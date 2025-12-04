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
    [MenuItem("Tools/AssemblyComplier")]
    public static void CompileAssembly()
    {
        BuildEditorDll((result) => {
        BuildDll((resutl1) => {
            EditorUtility.DisplayDialog("Ã· æ", (result && resutl1) ? "DLL±‡“Î≥…π¶" : "DLL±‡“Î ß∞‹", "»∑∂®");
        });
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
                                        Application.dataPath + "/../Packages/GamePlay/Scripts/GuideSystem", Application.dataPath + "/../Plugins/GamePlay/GuideSystemEditor.dll", "UNITY_EDITOR");
                                    }
                                    else
                                        onCallback(false);
                                },
                                Application.dataPath + "/../Packages/GamePlay/Scripts/ActorSystem", Application.dataPath + "/../Plugins/GamePlay/ActorSystemEditor.dll", "UNITY_EDITOR");
                            }
                            else
                                onCallback(false);
                        },
                        Application.dataPath + "/../Packages/GamePlay/Scripts/Cutscene", Application.dataPath + "/../Plugins/GamePlay/CutsceneEditor.dll", "UNITY_EDITOR",
                           Application.dataPath + "/../Plugins/GamePlay/TagLibSharp.dll");
                    }
                    else
                        onCallback(false);
                },
                Application.dataPath + "/../Packages/GamePlay/Scripts/AgentTree", Application.dataPath + "/../Plugins/GamePlay/AgentTreeEditor.dll", "UNITY_EDITOR");
            }
            else
                onCallback(false);
        }, Application.dataPath + "/../Packages/GamePlay/Scripts/GameFramework", Application.dataPath + "/../Plugins/GamePlay/GameFrameworkEditor.dll", "UNITY_EDITOR");
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
                                        Application.dataPath + "/../Packages/GamePlay/Scripts/GuideSystem", Application.dataPath + "/../Plugins/GamePlay/GuideSystem.dll");
                                    }
                                    else
                                        onCallback(false);
                                },
                                Application.dataPath + "/../Packages/GamePlay/Scripts/ActorSystem", Application.dataPath + "/../Plugins/GamePlay/ActorSystem.dll");
                            }
                            else
                                onCallback(false);
                        },
                        Application.dataPath + "/../Packages/GamePlay/Scripts/Cutscene", Application.dataPath + "/../Plugins/GamePlay/Cutscene.dll",
                           Application.dataPath + "/../Plugins/GamePlay/TagLibSharp.dll");
                    }
                    else
                        onCallback(false);
                },
                Application.dataPath + "/../Packages/GamePlay/Scripts/AgentTree", Application.dataPath + "/../Plugins/GamePlay/AgentTree.dll");
            }
            else
                onCallback(false);
        }, Application.dataPath + "/../Packages/GamePlay/Scripts/GameFramework", Application.dataPath + "/../Plugins/GamePlay/GameFramework.dll");
    }
    //--------------------------------------------------------
    public static void ComplierDll(System.Action<bool> onCallback, string sourceDir, string outputDll, string defineSymbols = "", params string[] referenceDlls)
    {
        var scripts = Directory.GetFiles(sourceDir, "*.cs", SearchOption.AllDirectories);
        if (scripts.Length == 0)
        {
            UnityEngine.Debug.LogError("Œ¥’“µΩC#Œƒº˛: " + sourceDir);
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
                        UnityEngine.Debug.LogError(dllName + "  ±‡“Î ß∞‹: " + path + "\n" + string.Join("\n", result[i].message));
                    }
                    catch
                    {

                    }
                }
            }
            if(bSucceed)
            {
                UnityEngine.Debug.Log(dllName + "  DLL±‡“Î≥…π¶");
            }
            onCallback(bSucceed);
        };

        if (!builder.Build())
        {
            UnityEngine.Debug.LogError(dllName + "  ±‡“Î∆Ù∂Ø ß∞‹");
            onCallback(false);
        }
    }
}
#endif