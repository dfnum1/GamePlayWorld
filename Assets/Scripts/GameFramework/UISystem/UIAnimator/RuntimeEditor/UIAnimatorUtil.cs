#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;
using Framework.RtgTween;
using Framework.Base;

namespace Framework.UI.ED
{
    //------------------------------------------------------
    public class UIAnimatorUtil
    {
   //     static Texture2D ms_SceneGUIBG = null;
        private static Texture2D _gridTexture;
        public static Texture2D gridTexture
        {
            get
            {
                if (_gridTexture == null) _gridTexture = GenerateGridTexture(Color.gray, new Color(0.3f, 0.3f, 0.3f, 0.5f));
                return _gridTexture;
            }
        }
        private static Texture2D _crossTexture;
        public static Texture2D crossTexture
        {
            get
            {
                if (_crossTexture == null) _crossTexture = GenerateCrossTexture(Color.gray);
                return _crossTexture;
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
        //------------------------------------------------------
        public static void DrawGrid(Rect rect, float zoom, Vector2 panOffset)
        {
            rect.position = Vector2.zero;

            Vector2 center = rect.size / 2f;
            Texture2D gridTex = gridTexture;
            Texture2D crossTex = crossTexture;

            // Offset from origin in tile units
            float xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            float yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            Vector2 tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            float tileAmountX = Mathf.Round(rect.size.x) / gridTex.width;
            float tileAmountY = Mathf.Round(rect.size.y) / gridTex.height;

            Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }
        //------------------------------------------------------
        public static void Check()
        {
//             ms_vAnimationElementList.Clear();
//             foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
//             {
//                 Type[] types = assembly.GetTypes();
//                 for (int i = 0; i < types.Length; ++i)
//                 {
//                     Type enumType = types[i];
//                     if(enumType.GetInterface(typeof(UIAnimationElement).FullName)!=null)
//                     {
//                         ms_vAnimationElementList.Add(enumType);
//                     }
//                 }
//             }
        }
        //------------------------------------------------------
        public struct EaseMenuCatch
        {
            public object pointer;
            public EEaseType easeType;
        }
        public static void BuildEaseMenu(string partent, object pointer, GenericMenu.MenuFunction2 OnCallback, EEaseType current = EEaseType.RTG_NUM)
        {
            System.Type enumType = typeof(EEaseType);
            foreach (Enum v in Enum.GetValues(enumType))
            {
                string strName = Enum.GetName(enumType, v);
                System.Reflection.FieldInfo fi = enumType.GetField(strName);
                if (fi.IsDefined(typeof(DisableGUIAttribute)))
                {
                    continue;
                }
                if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                {
                    strName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
                }
                if (current == (EEaseType)v)
                {
                    strName += "(✔)";
                }
                if (string.IsNullOrEmpty(partent))
                    ContextMenu.AddItemWithArge(strName, false, OnCallback, new EaseMenuCatch() { pointer = pointer, easeType = (EEaseType)v });
                else
                    ContextMenu.AddItemWithArge(partent + "/" + strName, false, OnCallback, new EaseMenuCatch() { pointer = pointer, easeType = (EEaseType)v });
            }
        }
        //------------------------------------------------------
        public struct QuationMenuCatch
        {
            public object pointer;
            public EQuationType quationType;
        }
        public static void BuildQuationMenu(string partent,object pointer, GenericMenu.MenuFunction2 OnCallback, EQuationType current = EQuationType.RTG_EASE_OUT)
        {
            System.Type enumType = typeof(EQuationType);
            foreach (Enum v in Enum.GetValues(enumType))
            {
                string strName = Enum.GetName(enumType, v);
                System.Reflection.FieldInfo fi = enumType.GetField(strName);
                if (fi.IsDefined(typeof(DisableGUIAttribute)))
                {
                    continue;
                }
                if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                {
                    strName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
                }
                if(current == (EQuationType)v)
                {
                    strName += "(✔)";
                }
                if(string.IsNullOrEmpty(partent))
                    ContextMenu.AddItemWithArge(strName, false, OnCallback, new QuationMenuCatch() { pointer = pointer, quationType = (EQuationType)v });
                else
                    ContextMenu.AddItemWithArge(partent + "/" + strName, false, OnCallback, new QuationMenuCatch() { pointer = pointer, quationType = (EQuationType)v });
            }
        }
        //------------------------------------------------------
        public struct AnimatorElementMenuCatch
        {
            public UIAnimatorElementType type;
            public object pointer;
            public int useCtl;
        }
        public static void BuildElementTypeMenu(string partent, object pointer, GenericMenu.MenuFunction2 OnCallback)
        {
            System.Type enumType = typeof(UIAnimatorElementType);
            foreach (Enum v in Enum.GetValues(enumType))
            {
                string strName = Enum.GetName(enumType, v);
                System.Reflection.FieldInfo fi = enumType.GetField(strName);
                if (fi.IsDefined(typeof(DisableGUIAttribute)))
                {
                    continue;
                }
                if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                {
                    strName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
                }
                if (string.IsNullOrEmpty(partent))
                    ContextMenu.AddItemWithArge(strName, false, OnCallback, new AnimatorElementMenuCatch() { pointer = pointer, type = (UIAnimatorElementType)v, useCtl= -1 });
                else
                    ContextMenu.AddItemWithArge(partent + "/" + strName, false, OnCallback, new AnimatorElementMenuCatch() { pointer = pointer, type = (UIAnimatorElementType)v, useCtl = -1 });
            }
        }
        //------------------------------------------------------
        public struct MenuCtlCatch
        {
            public enum ECtlType
            {
                None,
                ResetThis,
                ResetAll,
                RemoveThis,
                DelTrack,
            }
            public object pointer;
            public int start;
            public int count;

            public int useCtl;

            public ECtlType ctlType;
            public MenuCtlCatch(object track, int start, int count, ECtlType ctlType)
            {
                this.pointer = track;
                this.start = start;
                this.count = count;
                this.ctlType = ctlType;
                this.useCtl = -1;
            }
        }
        //------------------------------------------------------
        public static void BuildControllerKeyFrameMenuNoDel(string partent, object pointer, GenericMenu.MenuFunction2 OnCallback, int start = 0, int count = -1)
        {
            if (string.IsNullOrEmpty(partent))
            {
                ContextMenu.AddItemWithArge("移除", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge("重置", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
            }
            else
            {
                ContextMenu.AddItemWithArge(partent + "/移除", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge(partent + "/重置", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
            }
        }
        //------------------------------------------------------
        public static void BuildControllerMenuNoDel(string partent, object pointer, GenericMenu.MenuFunction2 OnCallback, int start = 0, int count = -1)
        {
            if (string.IsNullOrEmpty(partent))
            {
                ContextMenu.AddItemWithArge("移除所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge("重置当前帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
                ContextMenu.AddItemWithArge("重置所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetAll));
            }
            else
            {
                ContextMenu.AddItemWithArge(partent + "/移除所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge(partent + "/重置当前帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
                ContextMenu.AddItemWithArge(partent + "/重置所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetAll));
            }
        }
        //------------------------------------------------------
        public static void BuildControllerMenu(string partent, object pointer, GenericMenu.MenuFunction2 OnCallback, int start = 0, int count = -1)
        {
            if(string.IsNullOrEmpty(partent))
            {
                ContextMenu.AddItemWithArge( "移除所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge( "重置当前帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
                ContextMenu.AddItemWithArge( "重置所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetAll));
                ContextMenu.AddItemWithArge("移除", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.DelTrack));
            }
            else
            {
                ContextMenu.AddItemWithArge(partent+"/移除所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.RemoveThis));
                ContextMenu.AddItemWithArge(partent + "/重置当前帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetThis));
                ContextMenu.AddItemWithArge(partent + "/重置所有帧", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.ResetAll));
                ContextMenu.AddItemWithArge(partent + "/移除", false, OnCallback, new MenuCtlCatch(pointer, start, count, MenuCtlCatch.ECtlType.DelTrack));
            }
        }
        //------------------------------------------------------
        public static int DrawBitEnum(System.Type type, string strDispalyName, int nValue, bool bEnumBitOffset = true, HashSet<string> vIngores = null)
        {
            EditorGUILayout.LabelField(strDispalyName);
            EditorGUI.indentLevel++;
            foreach (Enum v in Enum.GetValues(type))
            {
                string strName = Enum.GetName(type, v);
                if (vIngores!=null && vIngores.Contains(strName))
                    continue;
                FieldInfo fi = type.GetField(strName);
                if (fi.IsDefined(typeof(DisableGUIAttribute)))
                {
                    continue;
                }
                int flagValue = Convert.ToInt32(v);
                if (bEnumBitOffset) flagValue = 1 << flagValue;
                if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                {
                    strName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
                }

                bool bToggle = EditorGUILayout.Toggle(strName, (nValue & flagValue) != 0);
                if (bToggle)
                {
                    nValue |= (int)flagValue;
                }
                else nValue &= (int)(~flagValue);
            }

            EditorGUI.indentLevel--;
            return nValue;
        }
        //------------------------------------------------------
        public static string GetDispplayName(System.Object pPointer, string fieldName)
        {
            if (pPointer == null) return fieldName;
            FieldInfo fi = pPointer.GetType().GetField(fieldName);
            if (fi.IsDefined(typeof(DisableGUIAttribute)))
                return fieldName;
            if (fi != null && fi.IsDefined(typeof(PluginDisplayAttribute)))
            {
                fieldName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
            }
            return fieldName;
        }
        private static System.Type realType;
        private static System.Reflection.PropertyInfo s_property_handleWireMaterial;

        private static void InitType()
        {
            if (realType == null)
            {
                realType = System.Reflection.Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.HandleUtility");
                s_property_handleWireMaterial = realType.GetProperty("handleWireMaterial", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            }
        }

        public static Material handleWireMaterial
        {
            get
            {
                InitType();
                return (s_property_handleWireMaterial.GetValue(null, null) as Material);
            }
        }
        //-----------------------------------------------------
        private static int timelineHash = "timelinecontrol".GetHashCode();
        public static float DrawTimelinePanel(Rect timeRect, float time, float maxTime, float fGap = 1)
        {
            Rect position = timeRect;
            int controlID = GUIUtility.GetControlID(timelineHash, FocusType.Passive, position);
            Rect rect2 = new Rect((position.x + (position.width * time / maxTime)) - 5f, position.y + 2f, 10f, 20f);
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.MouseDown:
                    if (rect2.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseMove:
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float introduced10 = Mathf.Clamp(current.mousePosition.x, position.x, position.x + position.width);
                        time = (introduced10 - position.x) / position.width * maxTime;
                        current.Use();
                    }
                    break;

                default:
                    if (type == EventType.Repaint)
                    {
                        Rect rect = new Rect(position.x, position.y + 2f, position.width, timeRect.height);
                        DrawTimeline(rect, time, maxTime, fGap);
                        Color backupColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        GUI.skin.horizontalSliderThumb.Draw(rect2, new GUIContent(), controlID);
                        GUI.backgroundColor = backupColor;
                    }
                    break;
            }

            return time;
        }
        //------------------------------------------------------
        public static void DrawColorBox(Rect rect, Color color)
        {
            handleWireMaterial.SetPass(0);
            GL.Color(color);
            GL.Begin(1);
            GL.Vertex3(rect.xMin, rect.yMin, 0f);
            GL.Vertex3(rect.xMin, rect.yMax, 0f);
            GL.Vertex3(rect.xMax, rect.yMin, 0f);
            GL.Vertex3(rect.xMax, rect.yMax, 0f);
            GL.End();
        }
        //------------------------------------------------------
        public static void DrawTimeline(Rect rect, float curTime, float maxTime, float fGap)
        {
            if (fGap <= 0) fGap = Mathf.Max(0.01f, maxTime / 10);
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUI.RepeatButton(new Rect(rect.x, rect.y, rect.width, 25), Texture2D.blackTexture);
                EditorGUI.EndDisabledGroup();
                handleWireMaterial.SetPass(0);
                Color c = new Color(1f, 1f, 1f, 0.75f);
                GL.Begin(1);
                GL.Color(c);

                GL.Vertex3(rect.x, rect.y, 0f);
                GL.Vertex3(rect.x + rect.width, rect.y, 0f);
                GL.Vertex3(rect.x, rect.y + 25f, 0f);
                GL.Vertex3(rect.x + rect.width, rect.y + 25f, 0f);
                int gapCnt = 0;
                float fCur = 0;
                for (float i = 0; i <= maxTime; i += fGap)
                {
                    if (fCur >= maxTime) fCur = maxTime;
                    gapCnt++;
                    if ((gapCnt % 5) == 0)
                    {
                        GL.Vertex3(rect.x + ((rect.width * fCur) / maxTime), rect.y, 0f);
                        GL.Vertex3(rect.x + ((rect.width * fCur) / maxTime), rect.y + 10f, 0f);
                    }
                    else
                    {
                        GL.Vertex3(rect.x + ((rect.width * fCur) / maxTime), rect.y, 0f);
                        GL.Vertex3(rect.x + ((rect.width * fCur) / maxTime), rect.y + 5f, 0f);
                    }
                    if (fCur >= maxTime)
                    {
                        break;
                    }
                    fCur += fGap;
                }
                GL.End();
                c = new Color(1f, 0f, 0f, 1f);
                GL.Begin(1);
                GL.Color(c);
                GL.Vertex3(rect.x + (rect.width * curTime / maxTime), rect.y, 0f);
                GL.Vertex3(rect.x + (rect.width * curTime / maxTime), rect.y+ rect.height, 0f);
                GL.End();

                gapCnt = 0;
                for (float i = 0; i <= maxTime; i += fGap)
                {
                    fCur = i;
                    if (fCur > maxTime) fCur = maxTime;
                    gapCnt++;
                    if ((gapCnt % 5) == 0 || (gapCnt % 10) == 0 || fCur == maxTime)
                    {
                        if (fCur >= maxTime)
                            GUI.Label(new Rect(rect.x + ((rect.width * fCur) / maxTime) - 50, rect.y, 50, 20), i.ToString("f2"));
                        else
                            GUI.Label(new Rect(rect.x + ((rect.width * fCur) / maxTime), rect.y, 50, 20), i.ToString("f2"));
                    }
                }
            }
        }
    }
}
#endif
