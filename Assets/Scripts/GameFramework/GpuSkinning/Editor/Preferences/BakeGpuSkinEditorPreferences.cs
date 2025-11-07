#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Framework.Plugin
{
    public class BakeGpuSkinEditorPreferences
    {
        /// <summary> The last key we checked. This should be the one we modify </summary>
        private static string lastKey = "BakeGpuSkin.Settings";

        private static Dictionary<string, Settings> settings = new Dictionary<string, Settings>();

        [System.Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            [SerializeField] public string exportRoot = "Assets/Datas/Roles/Bakes/";

            public void OnAfterDeserialize()
            {
            }

            public void OnBeforeSerialize()
            {
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
        public static SettingsProvider CreateNodeSettingsProvider() {
            SettingsProvider provider = new SettingsProvider("Preferences/GameFramework/BakeGpuSkinEditor", SettingsScope.User) {
                guiHandler = (searchContext) => { PreferencesGUI(); },
                keywords = new HashSet<string>(new [] { "exportRoot" })
            };
            return provider;
        }
//#endif

        private static void PreferencesGUI()
        {
            VerifyLoaded();
            Settings settings = BakeGpuSkinEditorPreferences.settings[lastKey];

            settings.exportRoot = EditorGUILayout.TextField("ºæ±ºÉú³ÉÄ¿Â¼", settings.exportRoot);
            if (GUILayout.Button(new GUIContent("Set Default", "Reset all values to default"), GUILayout.Width(120)))
            {
                ResetPrefs();
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
    }
}
#endif