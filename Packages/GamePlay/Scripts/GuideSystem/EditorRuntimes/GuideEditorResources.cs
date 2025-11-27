/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideEditorResources
作    者:	
描    述:	引导相关资源
*********************************************************************/
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    public class GuideEditorResources
    {
        static string m_strInstallPath = null;
        public static Texture2D LoadTexture(string path)
        {
            if (m_strInstallPath == null)
            {
                string[] scripts = AssetDatabase.FindAssets("t:Script GuideSystemEditor");
                if (scripts.Length > 0)
                {
                    string installPath = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0])).Replace("\\", "/");

                    installPath = Path.Combine(installPath, "EditorResources").Replace("\\", "/");
                    if (System.IO.Directory.Exists(installPath))
                    {
                        m_strInstallPath = installPath;
                    }
                }
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(System.IO.Path.Combine(m_strInstallPath ,path));
        }
        public static Texture2D dot { get { return _dot != null ? _dot : _dot = LoadTexture("xnode_dot.png"); } }
        private static Texture2D _dot;
        public static Texture2D dotOuter { get { return _dotOuter != null ? _dotOuter : _dotOuter = LoadTexture("xnode_dot_outer.png"); } }
        private static Texture2D _dotOuter;
        public static Texture2D linkOuter { get { return _linkOuter != null ? _linkOuter : _linkOuter = LoadTexture("xnode_link.png"); } }
        private static Texture2D _linkOuter;
        public static Texture2D nodeBody { get { return _nodeBody != null ? _nodeBody : _nodeBody = LoadTexture("xnode_node.png"); } }
        private static Texture2D _nodeBody;
        public static Texture2D nodeHighlight { get { return _nodeHighlight != null ? _nodeHighlight : _nodeHighlight = LoadTexture("xnode_node_highlight.png"); } }
        private static Texture2D _nodeHighlight;
        
        // Styles
        public static Styles styles { get { return _styles != null ? _styles : _styles = new Styles(); } }
        public static Styles _styles = null;
        public static GUIStyle OutputPort { get { return new GUIStyle(EditorStyles.label) { alignment = TextAnchor.UpperRight }; } }
        public class Styles
        {
            public GUIStyle inputPort, nodeHeader, nodeHeaderDesc, nodeBody, tooltip, nodeHighlight;

            public Styles()
            {
                GUIStyle baseStyle = new GUIStyle("Label");
                baseStyle.fixedHeight = 18;

                inputPort = new GUIStyle(baseStyle);
                inputPort.alignment = TextAnchor.UpperLeft;
                inputPort.padding.left = 10;

                nodeHeader = new GUIStyle();
                nodeHeader.alignment = TextAnchor.MiddleCenter;
                nodeHeader.fontSize = 16;
                nodeHeader.fontStyle = FontStyle.Bold;
                nodeHeader.normal.textColor = Color.white;

                nodeBody = new GUIStyle();
                nodeBody.normal.background = GuideEditorResources.nodeBody;
                nodeBody.border = new RectOffset(32, 32, 32, 32);
                nodeBody.padding = new RectOffset(16, 16, 4, 16);

                nodeHighlight = new GUIStyle();
                nodeHighlight.normal.background = GuideEditorResources.nodeHighlight;
                nodeHighlight.border = new RectOffset(32, 32, 32, 32);

                tooltip = new GUIStyle("helpBox");
                tooltip.alignment = TextAnchor.MiddleCenter;
            }
        }

        public static Texture2D GenerateGridTexture(Color line, Color bg)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Color col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, bg, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.35f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCrossTexture(Color line)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Color col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }
    }
}
#endif