/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	MapLevelEditor
作    者:	HappLI
描    述:	地图编辑器
*********************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class MapLevelEditor : EditorWindowBase
    {
        static MapLevelEditor ms_pInstance;
        [MenuItem("Tools/地图关卡编辑器")]
        public static void Open()
        {
            if (ms_pInstance != null)
            {
                ms_pInstance.Focus();
                return;
            }
            MapLevelEditor window = EditorWindow.GetWindow<MapLevelEditor>();
            window.titleContent = new GUIContent("地图关卡编辑器");
            window.minSize = new Vector2(800, 600);
            ms_pInstance = window;
        }
        //-----------------------------------------------------
    }
}