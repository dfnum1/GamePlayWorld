/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	Event Attributes
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
namespace Framework.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventDrawAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strMethod = "DrawUnAction";
#endif
        public EventDrawAttribute(string strMethod = null)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(strMethod)) return;
            this.strMethod = strMethod;
#endif
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class EventPopClassAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strMethod = "DrawEventPop";
        public string strEventName = "GetEventName";
#endif
        public EventPopClassAttribute(string strMethod = null, string strEventName = null)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(strMethod))
                this.strMethod = strMethod;
            if (!string.IsNullOrEmpty(strEventName))
                this.strEventName = strEventName;
#endif
        }
    }
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class EventCoreAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strMethod = "NewEvent";
#endif
        public EventCoreAttribute(string strMethod = null)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(strMethod)) return;
            this.strMethod = strMethod;
#endif
        }
    }
    public class EventDeclarationAttribute : Attribute
    {
        public ushort eType;
        public string EventName;
        public int nProprity;
        public bool bEnable = true;
        public bool marshBuff = false;
        public EventDeclarationAttribute(ushort eType, string EventName = "", bool marshBuff = false, bool bEnable = true, int nProprity = 0)
        {
            this.eType = eType;
            if (string.IsNullOrEmpty(EventName))
                this.EventName = this.eType.ToString();
            else
                this.EventName = EventName;
            this.nProprity = nProprity;
            this.bEnable = bEnable;
            this.marshBuff = marshBuff;
        }
    }
    public class EventNativeIncludeAttribute : Attribute
    {
        public string includePath;
        public EventNativeIncludeAttribute(string includePath)
        {
            this.includePath = includePath;
        }
    }
    //------------------------------------------------------
#if UNITY_EDITOR
    public class EventPreferences
    {
        public enum NoodleType { Curve, Line, Angled, Count }

        /// <summary> The last key we checked. This should be the one we modify </summary>
        private static string lastKey = "AI.Settings";

        private static Dictionary<string, Color> typeColors = new Dictionary<string, Color>();
        private static Dictionary<string, Settings> settings = new Dictionary<string, Settings>();

        [System.Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            [SerializeField]
            public float rowHeight = 20;

            [SerializeField]
            private string typeColorsData = "";
            [NonSerialized]
            public Dictionary<string, Color> typeColors = new Dictionary<string, Color>();
            public void OnAfterDeserialize()
            {
                // Deserialize typeColorsData
                typeColors = new Dictionary<string, Color>();
                string[] data = typeColorsData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < data.Length; i += 2)
                {
                    Color col;
                    if (ColorUtility.TryParseHtmlString("#" + data[i + 1], out col))
                    {
                        typeColors.Add(data[i], col);
                    }
                }
            }

            public void OnBeforeSerialize()
            {
                // Serialize typeColors
                typeColorsData = "";
                foreach (var item in typeColors)
                {
                    typeColorsData += item.Key + "," + ColorUtility.ToHtmlStringRGB(item.Value) + ",";
                }
            }
        }

        /// <summary> Get settings of current active editor </summary>
        public static Settings GetSettings()
        {
            if (!settings.ContainsKey(lastKey)) VerifyLoaded();
            return settings[lastKey];
        }

#if UNITY_2019_1_OR_NEWER
        [UnityEditor.SettingsProvider]
        public static UnityEditor.SettingsProvider CreateNodeSettingsProvider()
        {
            UnityEditor.SettingsProvider provider = new UnityEditor.SettingsProvider("Preferences/EventSetting", UnityEditor.SettingsScope.User)
            {
                guiHandler = (searchContext) => { PreferencesGUI(); },
            };
            return provider;
        }
#endif

#if !UNITY_2019_1_OR_NEWER
        [PreferenceItem("Event Editor")]
#endif
        private static void PreferencesGUI()
        {
            VerifyLoaded();
            Settings settings = EventPreferences.settings[lastKey];

            SettingsGUI(lastKey, settings);
            TypeColorsGUI(lastKey, settings);
            if (GUILayout.Button(new GUIContent("Set Default", "Reset all values to default"), GUILayout.Width(120)))
            {
                ResetPrefs();
            }
        }

        private static void SettingsGUI(string key, Settings settings)
        {
            //Label
            settings.rowHeight = EditorGUILayout.FloatField(new GUIContent("行高", "行高"), settings.rowHeight);
            if (GUI.changed)
            {
                SavePrefs(key, settings);
            }
            EditorGUILayout.Space();
        }

        private static void TypeColorsGUI(string key, Settings settings)
        {
            //Clone keys so we can enumerate the dictionary and make changes.
            var typeColorKeys = new List<String>(typeColors.Keys);

            //Display type colors. Save them if they are edited by the user
            foreach (var type in typeColorKeys)
            {
                string typeColorKey = type;
                Color col = typeColors[type];
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                col = EditorGUILayout.ColorField(typeColorKey, col);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    typeColors[type] = col;
                    if (settings.typeColors.ContainsKey(typeColorKey)) settings.typeColors[typeColorKey] = col;
                    else settings.typeColors.Add(typeColorKey, col);
                    SavePrefs(key, settings);
                }
            }
        }

        /// <summary> Load prefs if they exist. Create if they don't </summary>
        private static Settings LoadPrefs()
        {
            // Create settings if it doesn't exist
            if (!EditorPrefs.HasKey(lastKey))
            {
                EditorPrefs.SetString(lastKey, JsonUtility.ToJson(new Settings()));
            }
            return JsonUtility.FromJson<Settings>(EditorPrefs.GetString(lastKey));
        }

        /// <summary> Delete all prefs </summary>
        public static void ResetPrefs()
        {
            if (EditorPrefs.HasKey(lastKey)) EditorPrefs.DeleteKey(lastKey);
            if (settings.ContainsKey(lastKey)) settings.Remove(lastKey);
            typeColors = new Dictionary<String, Color>();
            VerifyLoaded();
        }

        /// <summary> Save preferences in EditorPrefs </summary>
        private static void SavePrefs(string key, Settings settings)
        {
            EditorPrefs.SetString(key, JsonUtility.ToJson(settings));
        }

        /// <summary> Check if we have loaded settings for given key. If not, load them </summary>
        private static void VerifyLoaded()
        {
            if (!settings.ContainsKey(lastKey)) settings.Add(lastKey, LoadPrefs());
        }

        /// <summary> Return color based on type </summary>
        public static Color GetTypeColor(string typeName)
        {
            VerifyLoaded();
            if (string.IsNullOrEmpty(typeName)) return Color.gray;
            Color col;
            if (!typeColors.TryGetValue(typeName, out col))
            {
                if (settings[lastKey].typeColors.ContainsKey(typeName)) typeColors.Add(typeName, settings[lastKey].typeColors[typeName]);
                else
                {
#if UNITY_5_4_OR_NEWER
                    UnityEngine.Random.InitState(typeName.GetHashCode());
#else
                    UnityEngine.Random.seed = typeName.GetHashCode();
#endif
                    col = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    typeColors.Add(typeName, col);
                }
            }
            return col;
        }
    }
#endif
}