#if UNITY_EDITOR
/********************************************************************
生成日期:	24:7:2019   11:12
类    名: 	MenuTools
作    者:	HappLI
描    述:	Csv 解析代码自动生成器
*********************************************************************/
using Framework.Base;
using Framework.Core;
using Framework.ED;
using System.IO;
using UnityEditor;

namespace TopGame.ED
{
    public class MenuTools
    {
        [MenuItem("Tools/RunGame _F5")]
        static void RunGame()
        {
            EditorUtil.OpenStartUpApplication("Assets/Scenes/GameHot.unity");
        }
        //------------------------------------------------------
        [MenuItem("Tools/Pause _F10")]
        static void PauseGame()
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
        //------------------------------------------------------
        [MenuItem("Tools/Step _F11")]
        static void NextStep()
        {
            EditorApplication.Step();
        }
        //------------------------------------------------------
        [MenuItem("Tools/同步磁盘")]
        static void ForceSyncSave()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        //------------------------------------------------------
        [MenuItem("Tools/更新Layer")]
        static void CheckTagAndLayer()
        {
            LayerUtil.CheckTagAndLayer();
        }
        //------------------------------------------------------
        [MenuItem("Tools/代码/代码自动化 _#F11")]
        public static void AutoCode()
        {
            ActionEventCoreGenerator.Build();
        }
        //------------------------------------------------------
        [MenuItem("Tools/代码/重新编译 _#F3")]
        public static void ReCompilation()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        //------------------------------------------------------
        [MenuItem("Tools/代码/非编辑态代码编辑检测")]
        public static void UnEditorCompilation()
        {
            string outputPath = "Library/TempBuild/Runtime.dll";
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            string[] scripts = Directory.GetFiles("Assets/Scritps", "*.cs", SearchOption.AllDirectories);

            var builder = new UnityEditor.Compilation.AssemblyBuilder(outputPath, scripts);

            builder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            builder.buildTargetGroup = BuildPipeline.GetBuildTargetGroup(builder.buildTarget);
            builder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(builder.buildTargetGroup);

            builder.buildFinished += (path, result) =>
            {
                bool bHasError = false;
                for(int i =0; i < result.Length; i++)
                {
                    var assembly = result[i];
                    if(assembly.type ==  UnityEditor.Compilation.CompilerMessageType.Error )
                    {
                        bHasError = true;
                        UnityEngine.Debug.Log("编译失败: " + assembly.message);
                        UnityEngine.Debug.Log(" Path: " + assembly.file + " Line: " + assembly.line);
                    }
                }
                if (!bHasError)
                    UnityEngine.Debug.Log("编译成功: " + path);
                else
                {
                    UnityEngine.Debug.Log("编译失败: " + path);
                }
            };

            // 6. 开始编译
            if (!builder.Build())
                UnityEngine.Debug.LogError("DLL编译启动失败");
        }
        //------------------------------------------------------
        [MenuItem("Assets/获取资源配置路径")]
        public static void GetAssetPathFile()
        {
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                UnityEngine.Debug.Log("请选择资源!");
                return;
            }
            string paths = "";
            for (int i = 0; i < Selection.objects.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                UnityEngine.Debug.Log(path);
                paths += path + "\r\n";
            }
            EditorGUIUtility.systemCopyBuffer = paths;
        }
    }
}

#endif