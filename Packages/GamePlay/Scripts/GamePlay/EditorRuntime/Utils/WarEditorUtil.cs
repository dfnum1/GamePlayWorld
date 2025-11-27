
using UnityEngine;

namespace Framework.War.Editor
{
    public class WarEditorUtil
    {
        static string ms_installPath = null;
        public static string BuildInstallPath()
        {
            if (string.IsNullOrEmpty(ms_installPath))
            {
                var scripts = UnityEditor.AssetDatabase.FindAssets("t:Script WarWorldEditor");
                if (scripts.Length > 0)
                {
                    ms_installPath = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0])).Replace("\\", "/");
                }
            }
            return ms_installPath;
        }
        //-----------------------------------------------------
        public static Texture2D GetFloorTexture()
        {
            string install = BuildInstallPath();
            if (string.IsNullOrEmpty(install)) return null;
            var ground = UnityEditor.AssetDatabase.FindAssets("t:Texture2D ground", new string[] { install });
            if (ground == null || ground.Length <= 0) return null;
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath(ground[0]));
        }
        //-----------------------------------------------------
        private static GUIStyle ms_PanelTileStyle = null;
        public static GUIStyle panelTitleStyle
        {
            get
            {
                if (ms_PanelTileStyle == null)
                {
                    ms_PanelTileStyle = new GUIStyle();
                    ms_PanelTileStyle.fontSize = 13;
                    ms_PanelTileStyle.normal.textColor = Color.white;
                    ms_PanelTileStyle.alignment = TextAnchor.MiddleCenter;

                }
                return ms_PanelTileStyle;
            }
        }
    }
}