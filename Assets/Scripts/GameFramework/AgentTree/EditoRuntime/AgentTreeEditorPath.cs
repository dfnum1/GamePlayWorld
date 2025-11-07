#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public class AgentTreeEditorPath
    {
        public string autoGeneratorBuildPath = "Scripts/GameMain/Generators/AgentTree/ATEditor";
        public string autoRuntimeMonoPath = "Scripts/GameMain/Generators/AgentTree/ATRuntime";
        public static string BuildGeneratorEditorPath()
        {
            return Path.Combine(Application.dataPath, AgentTreePreferences.GetSettings().editorPathConfig.autoGeneratorBuildPath);
        }
        public static string BuildGeneratorRuntimePath()
        {
            return Path.Combine(Application.dataPath, AgentTreePreferences.GetSettings().editorPathConfig.autoRuntimeMonoPath).Replace("\\", "/");
        }

        static string ms_installPath = null;
        static string ms_ProjectInstallPath = null;
        public static string BuildInstallProjectPath()
        {
            if (string.IsNullOrEmpty(ms_ProjectInstallPath))
            {
                var scripts = UnityEditor.AssetDatabase.FindAssets("t:Script AAgentTreeData");
                if (scripts.Length > 0)
                    ms_ProjectInstallPath = Path.GetDirectoryName(UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0])).Replace("\\", "/");
                if (!string.IsNullOrEmpty(ms_ProjectInstallPath))
                {
                    ms_ProjectInstallPath = Path.GetDirectoryName(ms_ProjectInstallPath).Replace("\\", "/");
                }
            }
            return ms_ProjectInstallPath;
        }
        public static string BuildInstallPath()
        {
            if(string.IsNullOrEmpty(ms_installPath))
            {
                var scripts = UnityEditor.AssetDatabase.FindAssets("t:Script AAgentTreeData");
                if(scripts.Length>0)
                    ms_installPath = Path.GetDirectoryName(UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0])).Replace("\\", "/");
                if (!string.IsNullOrEmpty(ms_installPath) )
                {
                    if(ms_installPath.StartsWith("Assets/"))
                        ms_installPath = ms_installPath.Substring("Assets/".Length);
                    ms_installPath = Path.GetDirectoryName(ms_installPath).Replace("\\", "/");
                }
            }
            string codePath = ms_installPath;
            if (string.IsNullOrEmpty(codePath))
                codePath = "Scripts/GameFramework/AgentTree";
            return Path.Combine(Application.dataPath, codePath).Replace("\\", "/");
        }
        public static string BuildATEditResPath()
        {
            return Path.Combine(BuildInstallProjectPath(), "EditoRuntime/Editor Default Resources").Replace("\\", "/");
        }
        public static string BuildVarSerializerCodePath()
        {
            return Path.Combine(BuildInstallPath(), "Runtime/AutoCode/VariableSerializes.cs").Replace("\\", "/");
        }
        public static string BuildVarFactoryCodePath()
        {
            return Path.Combine(BuildInstallPath(), "Runtime/AutoCode/VariableFactory.cs").Replace("\\", "/");
        }
        public static string BuildVarPoolCodePath()
        {
            return Path.Combine(BuildInstallPath(), "Runtime/AutoCode/VariablePools.cs").Replace("\\", "/");
        }
        public static string BuildATFunctionCodePath()
        {
            return Path.Combine(BuildInstallPath(), "Runtime/AutoCode/AgentTreeTask_Func").Replace("\\", "/");
        }
    }
}
#endif