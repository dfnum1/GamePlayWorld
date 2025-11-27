/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuidePreferences
作    者:	
描    述:	引导编辑器偏好设置
*********************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Framework.Guide.Editor
{
    public class GuidePreferences
    {
        public enum NoodleType { Curve, Line, Angled, Count }

        /// <summary> The last key we checked. This should be the one we modify </summary>
        private static string lastKey => GetProjectKeyPrefix() + "Settings";

        private static Dictionary<string, Color> typeColors = new Dictionary<string, Color>();
        private static Dictionary<string, Settings> settings = new Dictionary<string, Settings>();

        private static string GetProjectKeyPrefix()
        {
            // 用项目路径的 hash 作为前缀，保证唯一性
            string projectPath = Application.dataPath; // 例如 D:/MyProject/Assets
            int hash = projectPath.GetHashCode();
            return $"GuideSystem.{hash}.";
        }
        [System.Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            public string dataSavePath = "Assets/Res/GuideDatas";
            public string generatorCodePath = "Assets/OpenScripts/GameApp/GuideSystem/Generated";
            public string commitRoots = "Res/UI;Res/UIPrefabTmp";
            [SerializeField] private Color32 _gridLineColor = new Color(0.45f, 0.45f, 0.45f);
            public Color32 gridLineColor { get { return _gridLineColor; } set { _gridLineColor = value; _gridTexture = null; _crossTexture = null; } }

            [SerializeField]
            private Color32 _gridBgColor = new Color(0.55f, 0.55f, 0.55f);
            [SerializeField]
            private Color _linkLineColor = Color.white;

            [SerializeField]
            private float _linkLineWidth = 5;
            public Color32 gridBgColor { get { return _gridBgColor; } set { _gridBgColor = value; _gridTexture = null; } }

            public Color linkLineColor { get { return _linkLineColor; } set { _linkLineColor = value; } }

            public float linkLineWidth { get { return _linkLineWidth; } set { _linkLineWidth = value; } }

            [Obsolete("Use maxZoom instead")]
            public float zoomOutLimit { get { return maxZoom; } set { maxZoom = value; } }

            [UnityEngine.Serialization.FormerlySerializedAs("zoomOutLimit")]
            public float maxZoom = 5f;
            public float minZoom = 1f;
            public Color32 highlightColor = new Color32(255, 255, 255, 255);
            public Color32 excudeColor = new Color32(255, 0, 0, 148);
            public Color32 nodeBgColor = new Color32(63, 63, 63, 148);
            public bool gridSnap = true;
            public bool autoSave = true;
            public bool zoomToMouse = true;
            public bool portTooltips = true;
            [SerializeField] private string typeColorsData = "";
            [NonSerialized] public Dictionary<string, Color> typeColors = new Dictionary<string, Color>();
            public NoodleType noodleType = NoodleType.Angled;

            private Texture2D _gridTexture;
            public Texture2D gridTexture
            {
                get
                {
                    if (_gridTexture == null) _gridTexture = GuideEditorResources.GenerateGridTexture(gridLineColor, gridBgColor);
                    return _gridTexture;
                }
            }
            private Texture2D _crossTexture;
            public Texture2D crossTexture
            {
                get
                {
                    if (_crossTexture == null) _crossTexture = GuideEditorResources.GenerateCrossTexture(gridLineColor);
                    return _crossTexture;
                }
            }

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
        [SettingsProvider]
        public static SettingsProvider CreateNodeSettingsProvider() {
            SettingsProvider provider = new SettingsProvider("Preferences/GuideSystemEditor", SettingsScope.User) {
                guiHandler = (searchContext) => { PreferencesGUI(); },
                keywords = new HashSet<string>(new [] { "enter", "node", "editor", "graph", "connections", "noodles", "ports" })
            };
            return provider;
        }
#endif

#if !UNITY_2019_1_OR_NEWER
        [PreferenceItem("AT Editor")]
#endif
        private static void PreferencesGUI()
        {
            VerifyLoaded();
            Settings settings = GuidePreferences.settings[lastKey];

            settings.generatorCodePath = EditorGUILayout.TextField(new GUIContent("Generator Code Path", "Path to generate code to"), settings.generatorCodePath);
            settings.dataSavePath = EditorGUILayout.TextField(new GUIContent("Data Save Path", "Path save"), settings.dataSavePath);
            settings.commitRoots = EditorGUILayout.TextField(new GUIContent("Commit Directorys", "Commit Check Directory, split by \';\'"), settings.commitRoots);

            NodeSettingsGUI(lastKey, settings);
            GridSettingsGUI(lastKey, settings);
            SystemSettingsGUI(lastKey, settings);
            TypeColorsGUI(lastKey, settings);
            if (GUILayout.Button(new GUIContent("Set Default", "Reset all values to default"), GUILayout.Width(120)))
            {
                ResetPrefs();
            }
        }

        private static void GridSettingsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            settings.gridSnap = EditorGUILayout.Toggle(new GUIContent("Snap", "Hold CTRL in editor to invert"), settings.gridSnap);
            settings.zoomToMouse = EditorGUILayout.Toggle(new GUIContent("Zoom to Mouse", "Zooms towards mouse position"), settings.zoomToMouse);
            EditorGUILayout.LabelField("Zoom");
            EditorGUI.indentLevel++;
            settings.maxZoom = EditorGUILayout.FloatField(new GUIContent("Max", "Upper limit to zoom"), settings.maxZoom);
            settings.minZoom = EditorGUILayout.FloatField(new GUIContent("Min", "Lower limit to zoom"), settings.minZoom);
            EditorGUI.indentLevel--;
            settings.gridLineColor = EditorGUILayout.ColorField("Color", settings.gridLineColor);
            settings.gridBgColor = EditorGUILayout.ColorField("Grid BG Color", settings.gridBgColor);
            settings.linkLineColor = EditorGUILayout.ColorField("Link Line Color", settings.linkLineColor);
            settings.linkLineWidth = EditorGUILayout.Slider("Link Line Width", settings.linkLineWidth,1,10);
            if (GUI.changed)
            {
                SavePrefs(key, settings);

                GuideSystemEditor.RepaintAll();
            }
            EditorGUILayout.Space();
        }

        private static void SystemSettingsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("System", EditorStyles.boldLabel);
            settings.autoSave = EditorGUILayout.Toggle(new GUIContent("Autosave", "Disable for better editor performance"), settings.autoSave);
            if (GUI.changed) SavePrefs(key, settings);
            EditorGUILayout.Space();
        }

        private static void NodeSettingsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
            settings.highlightColor = EditorGUILayout.ColorField("Selection", settings.highlightColor);
            settings.excudeColor = EditorGUILayout.ColorField("Excude", settings.excudeColor);
            settings.nodeBgColor = EditorGUILayout.ColorField("NodeBG", settings.nodeBgColor);
            settings.noodleType = (NoodleType)EditorGUILayout.EnumPopup("Noodle type", (Enum)settings.noodleType);
            settings.portTooltips = EditorGUILayout.Toggle("Port Tooltips", settings.portTooltips);
            if (GUI.changed)
            {
                SavePrefs(key, settings);
                GuideSystemEditor.RepaintAll();
            }
            EditorGUILayout.Space();
        }

        private static void TypeColorsGUI(string key, Settings settings)
        {
            //Label
            EditorGUILayout.LabelField("Types", EditorStyles.boldLabel);

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
                    GuideSystemEditor.RepaintAll();
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
            GuideSystemEditor.RepaintAll();
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
}
#endif