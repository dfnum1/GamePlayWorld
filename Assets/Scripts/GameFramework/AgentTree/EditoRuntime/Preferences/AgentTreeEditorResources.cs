#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Framework.Plugin.AT
{
    public class AgentTreeEditorResources
    {
        public static UnityEngine.Object LoadRes(string name)
        {
            UnityEngine.Object pObj = EditorGUIUtility.Load(name);
            if (pObj != null) return pObj;

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine(AgentTreeEditorPath.BuildATEditResPath(), name));
        }
        public static Texture2D dot { get { return _dot != null ? _dot : _dot = LoadRes("xnode_dot.png") as Texture2D; } }
        private static Texture2D _dot;
        public static Texture2D dotOuter { get { return _dotOuter != null ? _dotOuter : _dotOuter = LoadRes("xnode_dot_outer.png") as Texture2D; } }
        private static Texture2D _dotOuter;
        public static Texture2D linkOuter { get { return _linkOuter != null ? _linkOuter : _linkOuter = LoadRes("xnode_link.png") as Texture2D; } }
        private static Texture2D _linkOuter;
        public static Texture2D nodeBody { get { return _nodeBody != null ? _nodeBody : _nodeBody = LoadRes("xnode_node.png") as Texture2D; } }
        private static Texture2D _nodeBody;
        public static Texture2D nodeHighlight { get { return _nodeHighlight != null ? _nodeHighlight : _nodeHighlight = LoadRes("xnode_node_highlight.png") as Texture2D; } }
        private static Texture2D _nodeHighlight;

        public static Texture2D enterTick { get { return _enterTick != null ? _enterTick : _enterTick = LoadRes("at_enter_tick.png") as Texture2D; } }
        private static Texture2D _enterTick;

        public static Texture2D enterStart { get { return _enterStart != null ? _enterStart : _enterStart = LoadRes("at_enter_start.png") as Texture2D; } }
        private static Texture2D _enterStart;

        public static Texture2D enterExit { get { return _enterExit != null ? _enterExit : _enterExit = LoadRes("at_enter_exit.png") as Texture2D; } }
        private static Texture2D _enterExit;

        public static Texture2D keyInput { get { return _keyInput != null ? _keyInput : _keyInput = LoadRes("at_key_input.png") as Texture2D; } }
        private static Texture2D _keyInput;

        public static Texture2D mouseInput { get { return _moueInput != null ? _moueInput : _moueInput = LoadRes("at_mouse_input.png") as Texture2D; } }
        private static Texture2D _moueInput;

        public static Texture2D enterCustom { get { return _enterCustom != null ? _enterCustom : _enterCustom = LoadRes("at_enter_custom.png") as Texture2D; } }
        private static Texture2D _enterCustom;

        public static Texture2D breakPoint { get { return _breakPoint != null ? _breakPoint : _breakPoint = LoadRes("at_break_point.png") as Texture2D; } }
        private static Texture2D _breakPoint;

        public static Texture2D function { get { return _function != null ? _function : _function = LoadRes("at_tr_function.png") as Texture2D; } }
        private static Texture2D _function;

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
                nodeBody.normal.background = AgentTreeEditorResources.nodeBody;
                nodeBody.border = new RectOffset(32, 32, 32, 32);
                nodeBody.padding = new RectOffset(16, 16, 4, 16);

                nodeHighlight = new GUIStyle();
                nodeHighlight.normal.background = AgentTreeEditorResources.nodeHighlight;
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