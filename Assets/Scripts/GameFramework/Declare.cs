/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	数据帧定义
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Base;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
namespace Framework.Core
{
    public class DeclareKit
    {
        public static int Time_InTag_OutTag_Pos_LookAt
        {
            get
            {
                return (int)(1 << (int)(eDeclareType.Time)) |
                        (int)(1 << (int)(eDeclareType.InTag)) |
                        (int)(1 << (int)(eDeclareType.OutTag)) |
                        (int)(1 << (int)(eDeclareType.Pos)) |
                        (int)(1 << (int)(eDeclareType.LookAt));
            }
        }
        public static int Time_InTag_OutTag_Pos_Euler
        {
            get
            {
                return (int)(1 << (int)(eDeclareType.Time)) |
                        (int)(1 << (int)(eDeclareType.InTag)) |
                        (int)(1 << (int)(eDeclareType.OutTag)) |
                        (int)(1 << (int)(eDeclareType.Pos)) |
                        (int)(1 << (int)(eDeclareType.Euler));
            }
        }
        public static int Time_InTag_OutTag_Pos_LookAt_Fov
        {
            get
            {
                return  (int)(1<<(int)(eDeclareType.Time))   |
                        (int)(1 << (int)(eDeclareType.InTag)) |
                        (int)(1 << (int)(eDeclareType.OutTag)) |
                        (int)(1 << (int)(eDeclareType.Pos)) |
                        (int)(1 << (int)(eDeclareType.LookAt)) |
                        (int)(1 << (int)(eDeclareType.Fov));
            }
        }
        public static int Time_InTag_OutTag_Pos_Euler_Fov
        {
            get
            {
                return (int)(1 << (int)(eDeclareType.Time)) |
                        (int)(1 << (int)(eDeclareType.InTag)) |
                        (int)(1 << (int)(eDeclareType.OutTag)) |
                        (int)(1 << (int)(eDeclareType.Pos)) |
                        (int)(1 << (int)(eDeclareType.Euler)) |
                        (int)(1 << (int)(eDeclareType.Fov));
            }
        }

        public static int InTag_OutTag_Pos_Euler
        {
            get
            {
                return (int)(1 << (int)(eDeclareType.InTag)) |
                        (int)(1 << (int)(eDeclareType.OutTag)) |
                        (int)(1 << (int)(eDeclareType.Pos)) |
                        (int)(1 << (int)(eDeclareType.Euler));
            }
        }
        public static int GetTypeSize(eDeclareType type)
        {
            switch(type)
            {
                case eDeclareType.Fov:
                case eDeclareType.Time: return 1;
                case eDeclareType.InTag:
                case eDeclareType.OutTag:
                case eDeclareType.Pos:
                case eDeclareType.Euler:
                case eDeclareType.LookAt:
                case eDeclareType.Scale:
                    return 3;
            }
            return 0;
        }
        public static int CalcTotalSize(int bit)
        {
            int size = 0;
            for(int i = 0; i < (int)eDeclareType.Count; ++i)
            {
                if( (bit&(1<<i))!=0 )
                {
                    size += GetTypeSize((eDeclareType)i);
                }
            }
            return size;
        }

        public static int CalcBoundIndex(eDeclareType type, int bit)
        {
            int size = 0;
            for (int i = 0; i < (int)eDeclareType.Count; ++i)
            {
                if ((bit & (1 << i)) != 0)
                {
                    if (type == (eDeclareType)i) return size;
                    size += GetTypeSize((eDeclareType)i);
                }
            }
            return -1;
        }

        public static bool HasDeclare(eDeclareType type, int bit)
        {
            if ((bit & (1 << (int)type)) != 0) return true;
            return false;
        }

        //------------------------------------------------------
        public static bool GetFloat(Core.eDeclareType type, int flags, List<float> propertys, ref float value)
        {
            int index = Core.DeclareKit.CalcBoundIndex(type, flags);
            if (propertys == null || index < 0 || index > propertys.Count) return false;
            value = propertys[index];
            return true;
        }
        //------------------------------------------------------
        public static bool GetVector3(Core.eDeclareType type, int flags, List<float> propertys, ref Vector3 vec3)
        {
            int index = Core.DeclareKit.CalcBoundIndex(type, flags);
            if (propertys == null || index < 0 || index + 3 > propertys.Count) return false;
            vec3.x = propertys[index];
            vec3.y = propertys[index + 1];
            vec3.z = propertys[index + 2];
            return true;
        }
        //------------------------------------------------------
        public static void SetFloat(Core.eDeclareType type, int flags, List<float> propertys, float value)
        {
            int index = Core.DeclareKit.CalcBoundIndex(type, flags);
            if (propertys == null || index < 0 || index > propertys.Count) return;
            propertys[index] = value;
        }
        //------------------------------------------------------
        public static void SetVector3(Core.eDeclareType type, int flags, List<float> propertys, Vector3 vec3)
        {
            int index = Core.DeclareKit.CalcBoundIndex(type, flags);
            if (propertys == null || index < 0 || index + 3 > propertys.Count) return;
            propertys[index] = vec3.x;
            propertys[index + 1] = vec3.y;
            propertys[index + 2] = vec3.z;
        }
        //------------------------------------------------------
        public static void InitDeclare(int flag, List<float> resultPropertys, float defaultValue =0)
        {
            int newSize = CalcTotalSize(flag);
            if (newSize <= 0) return;
            if (resultPropertys == null) resultPropertys = new List<float>();
            resultPropertys.Clear();
            for (int i = 0; i < newSize; ++i) resultPropertys.Add(defaultValue);
        }
        //------------------------------------------------------
        public static void ReDeclare(int oldFalg, int newFlag, List<float> propertys, List<float> resultPropertys)
        {
            int newSize = CalcTotalSize(newFlag);
            if (newSize <= 0) return;
            if (resultPropertys == null) resultPropertys = new List<float>();
            resultPropertys.Clear();
            for (int i = 0; i < newSize; ++i) resultPropertys.Add(0);
            for (int i = 0; i < (int)eDeclareType.Count; ++i)
            {
                if ((oldFalg & (1 << i)) != 0 && (newFlag&(1<<i)) !=0)
                {
                    int size = GetTypeSize((eDeclareType)i);
                    int bound = CalcBoundIndex((eDeclareType)i, oldFalg);
                    int new_bound = CalcBoundIndex((eDeclareType)i, newFlag);
                    if (bound>= 0 && bound+ size <= propertys.Count && new_bound+size <= resultPropertys.Count)
                    {
                        for (int j = 0; j < size; ++j)
                            resultPropertys[j + new_bound] = propertys[j + bound];
                    }
                }
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public static void DrawProperty(int flags, List<float> propertys)
        {
            if (propertys == null) return;
            float labelWidth = EditorGUIUtility.labelWidth;
            System.Type type = typeof(eDeclareType);
            for (int i = 0; i < (int)eDeclareType.Count; ++i)
            {
                if ((flags & (1 << i)) == 0)
                {
                    continue;
                }

                string strName = System.Enum.GetName(type, (eDeclareType)i);
                System.Reflection.FieldInfo fi = type.GetField(strName);
                if (fi.IsDefined(typeof(DisableGUIAttribute)))
                    continue;
                  
                if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                {
                    strName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
                }
                if (!strName.EndsWith(":")) strName += ":";

                int size = GetTypeSize((eDeclareType)i);
                int bound = CalcBoundIndex((eDeclareType)i, flags);

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 70;
                GUILayout.Label(strName);
                EditorGUIUtility.labelWidth = labelWidth;
                if (bound >= 0 && bound + size <= propertys.Count)
                {
                    for (int j = 0; j < size; ++j)
                        propertys[j + bound] = EditorGUILayout.FloatField(propertys[j + bound]);
                }
                else
                {
                    EditorGUILayout.HelpBox("数据错误", MessageType.Error);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
    }
    public enum eDeclareType : int
    {
        [PluginDisplay("时  间")]Time = 0,
        [PluginDisplay("位  置")] Pos,
        [PluginDisplay("旋  转")] Euler,
        [PluginDisplay("视  点")] LookAt,
        [PluginDisplay("缩  放")] Scale,
        [PluginDisplay("入切角")] InTag,
        [PluginDisplay("出切角")] OutTag,
        [PluginDisplay("广  角")] Fov,
        [DisableGUI] Count,
    }
    [System.Serializable]
    public struct DeclareKeyFrame
    {
        public int bits;
        public List<int[]> frames;
        public static DeclareKeyFrame DEFAULT = new DeclareKeyFrame() { bits = 0, frames = null };

        public bool isValid
        {
            get
            {
                if (bits <= 0 || frames == null) return false;
                return frames.Count > 0;
            }
        }

        public int totalSize
        {
            get
            {
                return DeclareKit.CalcTotalSize(bits);
            }
        }

        public DeclareKeyFrame NewClone()
        {
            DeclareKeyFrame newF = new DeclareKeyFrame();

            newF.bits = bits;
            if (!isValid) return newF;

            newF.frames = new List<int[]>();
            for (int i = 0; i < frames.Count; ++i)
            {
                newF.frames.Add(new List<int>(frames[i]).ToArray());
            }

            return newF;
        }
    }
}