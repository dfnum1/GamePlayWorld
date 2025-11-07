/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AgentTreeUtl
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
namespace Framework.Plugin.AT
{
    public class AgentTreeUtl
    {
#if UNITY_EDITOR
        public struct sMonoTypeInfo
        {
            public System.Type type;
            public int hashCode;
        }
        public static AnimationCurve pCopyCurve = null;
        public static AnimationCurve FormCopy()
        {
            AnimationCurve curve = new AnimationCurve();
            if (pCopyCurve == null) return curve;
            curve.keys = new List<Keyframe>(pCopyCurve.keys).ToArray();
            return curve;
        }

        public static HashSet<string> EXPORT_CLASS_SHORTNAME_SET = new HashSet<string>();
        public static List<string> POP_EXPORT_MONOS = new List<string>();
        public static List<sMonoTypeInfo> EXPORT_TYPE_MONOS = new List<sMonoTypeInfo>();
        public static Dictionary<int, System.Type> ExportClasses = new Dictionary<int, System.Type>();
        public static GUIStyle msCurrentStyle = null;
        static int m_nAreaClip = 0;
        static int m_nGroupClip = 0;
        static int m_nVerticalClip = 0;
        static int m_nHorizontalClip = 0;
        //------------------------------------------------------
        public static void BeginClip()
        {
            m_nAreaClip = 0;
            m_nGroupClip = 0;
            m_nVerticalClip = 0;
            m_nHorizontalClip = 0;
            msCurrentStyle = null;
        }
        //------------------------------------------------------
        public static void EndClip()
        {
            for (int i = 0; i < m_nAreaClip; ++i) EndArea();
            for (int i = 0; i < m_nGroupClip; ++i) EndGroup();
            for (int i = 0; i < m_nVerticalClip; ++i) EndVertical();
            for (int i = 0; i < m_nHorizontalClip; ++i) EndHorizontal();
            m_nAreaClip = 0;
            m_nGroupClip = 0;
            m_nVerticalClip = 0;
            m_nHorizontalClip = 0;

            msCurrentStyle = null;
        }
        //------------------------------------------------------
        public static void BeginArea(Rect rect)
        {
            m_nAreaClip++;
            GUILayout.BeginArea(rect);
        }   
        //------------------------------------------------------
        public static void BeginArea(Rect rect, Texture image)
        {
            m_nAreaClip++;
            if (image != null)
                GUILayout.BeginArea(rect, image);
            else GUILayout.BeginArea(rect);
        }
        //------------------------------------------------------
        public static void EndArea()
        {
            GUILayout.EndArea();
            m_nAreaClip--;
        }
        //------------------------------------------------------
        public static void BeginGroup(Rect rect)
        {
            m_nGroupClip++;
            GUI.BeginGroup(rect);
        }
        //------------------------------------------------------
        public static void BeginGroup(Rect rect, Texture image)
        {
            m_nGroupClip++;
            if (image) GUI.BeginGroup(rect, image);
            else GUI.BeginGroup(rect);
        }
        //------------------------------------------------------
        public static void EndGroup()
        {
            GUI.EndGroup();
            m_nGroupClip--;
        }
        //------------------------------------------------------
        public static void BeginVertical(GUIStyle style = null)
        {
            m_nVerticalClip++;
            msCurrentStyle = style;
            if (style != null) GUILayout.BeginVertical(style);
            else GUILayout.BeginVertical();
        }
        //------------------------------------------------------
        public static void EndVertical()
        {
            msCurrentStyle = null;
            GUILayout.EndVertical();
            m_nVerticalClip--;
        }
        //------------------------------------------------------
        public static void BeginHorizontal(GUIStyle style = null)
        {
            m_nHorizontalClip++;
            msCurrentStyle = style;
            if (style!=null) GUILayout.BeginHorizontal(style);
            else GUILayout.BeginHorizontal();
        }
        //------------------------------------------------------
        public static void EndHorizontal()
        {
            msCurrentStyle = null;
            GUILayout.EndHorizontal();
            m_nHorizontalClip--;
        }
        //-----------------------------------------------------
        static List<string> EnumPops = new List<string>();
        static List<System.Enum> EnumValuePops = new List<System.Enum>();
        public static System.Enum PopEnum(string strDisplayName, System.Enum curVar, System.Type enumType = null)
        {
            if(enumType == null) enumType = curVar.GetType();
            EnumPops.Clear();
            EnumValuePops.Clear();
            int index = -1;
            foreach (System.Enum v in System.Enum.GetValues(enumType))
            {
                System.Reflection.FieldInfo fi = enumType.GetField(v.ToString());
                string strTemName = v.ToString();
                if (fi != null && fi.IsDefined(typeof(ATDisableGUIAttribute)))
                    continue;
                if (fi != null && fi.IsDefined(typeof(ATDisplayNameAttribute)))
                {
                    strTemName = fi.GetCustomAttribute<ATDisplayNameAttribute>().DisplayName;
                }
                EnumPops.Add(strTemName);
                EnumValuePops.Add(v);
                if (v.ToString().CompareTo(curVar.ToString()) == 0)
                    index = EnumPops.Count - 1;
            }

            if (EnumPops.Count > 10)
            {
                //filter
                for (int i = 0; i < EnumPops.Count; ++i)
                {
                    string filter = EnumValuePops[i].ToString().Substring(0, 1).ToUpper();
                    EnumPops[i] = filter + "/" + EnumPops[i];
                }
            }

            if (string.IsNullOrEmpty(strDisplayName))
                index = EditorGUILayout.Popup(index, EnumPops.ToArray());
            else
                index = EditorGUILayout.Popup(strDisplayName, index, EnumPops.ToArray());
            if (index >= 0 && index < EnumValuePops.Count)
            {
                curVar = EnumValuePops[index];
            }
            EnumPops.Clear();
            EnumValuePops.Clear();

            return curVar;
        }
        //------------------------------------------------------
        public static object DrawProperty(string strLabel, object pValue, System.Type displayType)
        {
            if (displayType == null) return null;
            if(displayType.IsEnum)
            {
                if (pValue.GetType() == typeof(System.Int32))
                {
                    int val = (int)pValue;
                    return PopEnum(strLabel, (System.Enum)System.Enum.ToObject(displayType, val), displayType);
                }
                if (pValue.GetType() == typeof(System.Byte))
                {
                    int val = (byte)pValue;
                    return PopEnum(strLabel, (System.Enum)System.Enum.ToObject(displayType, val), displayType);
                }
            }
            if(IsUnityObject(displayType))
            {
                if(pValue !=null && pValue.GetType() == typeof(string))
                {
                    string strFile = (string)pValue;
                    UnityEngine.Object pRet = EditorGUILayout.ObjectField(strLabel, AssetDatabase.LoadMainAssetAtPath(strFile), displayType, true);
                    if (pRet != null) return AssetDatabase.GetAssetPath(pRet);
                    return "";
                }
            }
            if(displayType == typeof(System.Type))
            {
                if (pValue.GetType() == typeof(System.Int32))
                {
                    int val = (int)pValue;
                    System.Type classType = null;
                    int index = -1;
                    if (ExportClasses.TryGetValue(val, out classType))
                    {
                        if (classType != null)
                        {
                            for (int i = 0; i < AgentTreeUtl.EXPORT_TYPE_MONOS.Count; ++i)
                            {
                                if (EXPORT_TYPE_MONOS[i].type == classType)
                                {
                                    index = i;
                                    strLabel += "C-(" + AgentTreeUtl.EXPORT_TYPE_MONOS[i].type.Name + ")";
                                    break;
                                }
                            }
                        }
                    }
                    index = UnityEditor.EditorGUILayout.Popup(strLabel, index, AgentTreeUtl.POP_EXPORT_MONOS.ToArray());
                    if (index >= 0 && index < AgentTreeUtl.EXPORT_TYPE_MONOS.Count)
                        val = AgentTreeUtl.EXPORT_TYPE_MONOS[index].hashCode;
                    return val;
                }
            }

            return null;
        }
        //------------------------------------------------------
        public static bool IsUnityObject(System.Type baseType)
        {
            if (baseType == null) return false;
            if (baseType.IsSubclassOf(typeof(UnityEngine.Object))) return true;
            System.Type temp = baseType;
            while (temp != null)
            {
                if (temp.IsSubclassOf(typeof(UnityEngine.Object))) return true;
                temp = temp.BaseType;
            }
            return false;
        }
#endif
        //------------------------------------------------------
        public static void LogError(string msg)
        {
#if !UNITY_EDITOR
            return;
#endif
            Debug.LogError(msg);
        }
        //------------------------------------------------------
        public static void LogInfo(string msg)
        {
#if !UNITY_EDITOR
            return;
#endif
            Debug.Log(msg);
        }
        //------------------------------------------------------
        public static void LogWarning(string msg)
        {
#if !UNITY_EDITOR
            return;
#endif
            Debug.LogWarning(msg);
        }
        //-----------------------------------------------------
        static System.Text.StringBuilder ms_pBuider = null;
        public  static System.Text.StringBuilder stringBuilder
        {
            get
            {
                if (ms_pBuider == null) ms_pBuider = new System.Text.StringBuilder(128);
                ms_pBuider.Clear();
                return ms_pBuider;
            }
        }
        //-----------------------------------------------------
        static HashSet<int> ms_vIntSet = null;
        public static HashSet<int> intSet
        {
            get
            {
                if (ms_vIntSet == null) ms_vIntSet = new HashSet<int>();
                ms_vIntSet.Clear();
                return ms_vIntSet;
            }
        }
        //------------------------------------------------------
        public static int StringToHash(string name, int hashGUID = 0)
        {
#if USE_EXTERN
        return 0;
#endif
#if !USE_SERVER
            if (hashGUID == 0)
                return Animator.StringToHash(name);
#endif
            return hashGUID;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public static int TypeToHash(System.Type type, string name = null)
        {
            if (type == null) return 0;
            if(type.IsDefined( typeof(ATExportGUIDAttribute), false))
            {
                int hashGuid = type.GetCustomAttribute<ATExportGUIDAttribute>().hashCode;
                if (hashGuid != 0) return hashGuid;
            }
            if(!string.IsNullOrEmpty(name)) return AgentTreeUtl.StringToHash(name);
            return AgentTreeUtl.StringToHash(type.FullName);
        }
        //------------------------------------------------------
        static Dictionary<string, System.Type> ms_vBindTypes = null;
        public static System.Type GetTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            if (ms_vBindTypes == null)
            {
                ms_vBindTypes = new Dictionary<string, System.Type>();
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        System.Type tp = types[i];
                        if (tp.IsDefined(typeof(Framework.Base.PluginBinderTypeAttribute), false))
                        {
                            Framework.Base.PluginBinderTypeAttribute attr = (Framework.Base.PluginBinderTypeAttribute)tp.GetCustomAttribute(typeof(Framework.Base.PluginBinderTypeAttribute));
                            ms_vBindTypes[attr.bindName] = tp;
                        }
                    }
                }
            }
            System.Type returnType;
            if (ms_vBindTypes.TryGetValue(typeName, out returnType))
                return returnType;
            returnType = System.Type.GetType(typeName);
            if (returnType != null) return returnType;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                returnType = assembly.GetType(typeName, false, true);
                if (returnType != null) return returnType;
            }
            int index = typeName.LastIndexOf(".");
            if (index > 0 && index + 1 < typeName.Length)
            {
                string strTypeName = typeName.Substring(0, index) + "+" + typeName.Substring(index + 1, typeName.Length - index - 1);
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    returnType = assembly.GetType(strTypeName, false, true);
                    if (returnType != null) return returnType;
                }
            }

            return null;
        }
#endif
    }
}
