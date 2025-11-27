#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Framework.Guide;
namespace Framework.Guide.Editor
{
    [InitializeOnLoad]
    public class GuideSystemEngineInit
    {
        static Texture2D s_CustomIcon;
        static GuideSystemEngineInit()
        {
            s_CustomIcon = GuideEditorResources.LoadTexture("GuideSystem");
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;

            //! auto code
            // 你可以自定义生成文件路径
            string path = GuidePreferences.GetSettings().generatorCodePath;
            if (!string.IsNullOrEmpty(path))
            {
                string outputPath = Path.Combine(path, "GuideWrapper.cs");
                GuideAutoCode.AutoCode(outputPath);
            }
        }
        //-----------------------------------------------------
        static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (s_CustomIcon == null) return;
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj is GuideDatas)
            {
                //      Rect iconRect = new Rect(selectionRect.x + 2, selectionRect.y + 2, 16, 16);
                //       GUI.DrawTexture(iconRect, s_CustomIcon, ScaleMode.ScaleToFit);
                if (EditorGUIUtility.GetIconForObject(obj) != s_CustomIcon)
                    EditorGUIUtility.SetIconForObject(obj, s_CustomIcon);
            }
        }
    }
}
#endif