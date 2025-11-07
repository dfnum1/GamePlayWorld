#if UNITY_EDITOR && USE_ACTORSYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace ActorSystem.ED
{
    public class EditorPreferences
    {
        /// <summary> The last key we checked. This should be the one we modify </summary>
        private static string lastKey = "ActorSystemEditor.Settings";

        private static Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        private static Dictionary<string, Settings> settings = new Dictionary<string, Settings>();

        [System.Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            public int FrameRate = 30;

            [SerializeField]
            public Color colorClipFont = new Color(0.569f, 0.580f, 0.588f, 1.0f);

            [SerializeField]
            public Color clipBckg = new Color(0.5f, 0.5f, 0.5f, 1.0f);

            [SerializeField]
            public Color clipSelectedBckg = new Color(0.7f, 0.7f, 0.7f, 1.0f);

            [SerializeField]
            public Color clipBorderColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

            [SerializeField]
            public Color clipEaseBckgColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

            [SerializeField]
            public Color clipBlendIn = new Color(0.286f, 0.306f, 0.329f, 1.0f);

            [SerializeField]
            public Color clipBlendInSelected = new Color(0.408f, 0.427f, 0.478f, 1.0f);

            [SerializeField]
            public Color clipBlendOut = new Color(0.286f, 0.306f, 0.329f, 1.0f);

            [SerializeField]
            public Color clipBlendOutSelected = new Color(0.408f, 0.427f, 0.478f, 1.0f);

            [SerializeField]
            public Color colorRecordingClipOutline = new Color(1, 0, 0, 0.9f);

            [SerializeField] private string typeColorsData = "";
            [NonSerialized] public Dictionary<string, Color> typeColors = new Dictionary<string, Color>();

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

//#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider CreateActorSystemSettingsProvider() {
            SettingsProvider provider = new SettingsProvider("Preferences/GameFramework/ActorSystemEditor", SettingsScope.User) {
                guiHandler = (searchContext) => { PreferencesGUI(); },
            };
            return provider;
        }
//#endif

//#if !UNITY_2019_1_OR_NEWER
//        [PreferenceItem("AT Editor")]
//#endif
        public static void PreferencesGUI()
        {
            VerifyLoaded();
            Settings settings = EditorPreferences.settings[lastKey];
            SystemSettingsGUI(lastKey, settings);
            TypeColorsGUI(lastKey, settings);
            if (GUILayout.Button(new GUIContent("Set Default", "Reset all values to default"), GUILayout.Width(120)))
            {
                ResetPrefs();
            }
        }

        static string[] ms_vFramePop = { "30", "45", "60" };
        private static void SystemSettingsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("System", EditorStyles.boldLabel);
            int index = 0;
            for(int i =0; i < ms_vFramePop.Length; ++i)
            {
                if (ms_vFramePop[i] == settings.FrameRate.ToString())
                {
                    index = i;
                    break;
                }
            }
            index = EditorGUILayout.Popup("FrameRate", index, ms_vFramePop);
            settings.FrameRate = int.Parse(ms_vFramePop[index]);
            bool bChange = false;
            if (GUI.changed) bChange = true;
            var fields = settings.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            for(int i =0; i < fields.Length; ++i)
            {
                if (fields[i].Name == "FrameRate")
                    continue;
                if (fields[i].FieldType == typeof(Color))
                {
                    Color col = (Color)fields[i].GetValue(settings);
                    EditorGUI.BeginChangeCheck();
                    col = EditorGUILayout.ColorField(fields[i].Name, col);
                    if (EditorGUI.EndChangeCheck())
                    {
                        fields[i].SetValue(settings, col);
                    }
                }
            }
            if (bChange) SavePrefs(key, settings);

            EditorGUILayout.Space();
        }

        private static void TypeColorsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("Types", EditorStyles.boldLabel);

            //Clone keys so we can enumerate the dictionary and make changes.
            var typeColorKeys = new List<Type>(typeColors.Keys);

            //Display type colors. Save them if they are edited by the user
            foreach (var type in typeColorKeys)
            {
                string typeColorKey = type.Name;
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
                    RepaintAll();
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
            typeColors = new Dictionary<Type, Color>();
            VerifyLoaded();

            RepaintAll();
        }

        static void RepaintAll()
        {
            var editors = Resources.FindObjectsOfTypeAll<ActionEditorWindow>();
            if (editors != null)
            {
                for (int i = 0; i < editors.Length; ++i)
                    editors[i].Repaint();
            }
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
        public static Color GetTypeColor(System.Type type)
        {
            VerifyLoaded();
            if (type == null) return Color.gray;
            Color col;
            if (!typeColors.TryGetValue(type, out col))
            {
                string typeName = type.Name;
                if (settings[lastKey].typeColors.ContainsKey(typeName)) typeColors.Add(type, settings[lastKey].typeColors[typeName]);
                else
                {
#if UNITY_5_4_OR_NEWER
                    UnityEngine.Random.InitState(typeName.GetHashCode());
#else
                    UnityEngine.Random.seed = typeName.GetHashCode();
#endif
                    col = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    typeColors.Add(type, col);
                }
            }
            return col;
        }
    }
}
#endif