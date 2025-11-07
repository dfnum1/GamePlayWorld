/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	PluginManager
作    者:	HappLI
描    述:	插件管理面板
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TopGame.ED;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class PluginManager : EditorWindow
    {
        static int TOGGLE_WIDTH = 40; // 勾选开关的宽度
        [Serializable]
        public class PluginInfo
        {
            public string name;
            public string displayName; // 显示名称
            public string version;
            public string banner;
            public string marcoWord;
            public string description;
            public string installDir;
            public bool toggle = false;

            [System.NonSerialized]public Texture2D Texture;
        }
        [Serializable]
        public class PluginSetting
        {
            public List<PluginInfo> plugins = new List<PluginInfo>();
        }

        static PluginManager ms_Instance = null;
        [MenuItem("Tools/插件管理")]
        public static PluginManager Open()
        {
            if (ms_Instance == null)
                ms_Instance = EditorWindow.GetWindow<PluginManager>();
            if (ms_Instance != null)
            {
                ms_Instance.titleContent = new GUIContent("插件管理");
                ms_Instance.ShowUtility();
            }
            return ms_Instance;
        }
        //------------------------------------------------------
        public static Dictionary<string, bool> GetPluginMarcoWords()
        {
            Dictionary<string, bool> marcoWords = new Dictionary<string, bool>();
            try
            {
                string pluginCfg = Path.Combine(Application.dataPath, "../Plugins/PluginSetting.setting");
                if (File.Exists(pluginCfg))
                {
                    var plugins = JsonUtility.FromJson<PluginSetting>(File.ReadAllText(pluginCfg));
                    foreach (var plugin in plugins.plugins)
                    {
                        if (!string.IsNullOrEmpty(plugin.marcoWord))
                        {
                            var splits = plugin.marcoWord.Split(new char[] { '|', ';', ',', '/', ' ' });
                            for (int i = 0; i < splits.Length; i++)
                            {
                                var word = splits[i].Trim();
                                if (!string.IsNullOrEmpty(word) && !marcoWords.ContainsKey(word))
                                    marcoWords[word] = plugin.toggle;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return marcoWords;
        }
        //------------------------------------------------------
        private PluginSetting m_Setting = null;
        Vector2 m_Scroll = Vector2.zero;
        private string m_SearchText = ""; // 搜索关键字
        //------------------------------------------------------
        private void OnDisable()
        {
            ms_Instance = null;
            var windows = EditorWindow.FindObjectsOfType<PluginConfigWindow>();
            if(windows!=null)
            {
                foreach (var win in windows)
                {
                    win.Close();
                }
            }
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            minSize = maxSize = new Vector2(800, 600);
            try
            {
                string pluginCfg = Path.Combine(Application.dataPath, "../Plugins/PluginSetting.setting");
                if (File.Exists(pluginCfg))
                {
                    m_Setting = JsonUtility.FromJson<PluginSetting>(File.ReadAllText(pluginCfg));
                }
            }
            catch
            {
                m_Setting = null;
            }
            if (m_Setting == null) m_Setting = new PluginSetting();
        }
        //------------------------------------------------------
        private void OnGUI()
        {
            if (m_Setting == null)
            {
                GUILayout.Label("插件配置文件加载失败，请检查 Plugins/PluginSetting.setting 文件是否存在。", EditorStyles.boldLabel);
                return;
            }
            if (m_Setting.plugins == null)
                return;

            // 搜索输入框
            GUILayout.BeginHorizontal();
            GUILayout.Label("搜索：", GUILayout.Width(40));
            m_SearchText = GUILayout.TextField(m_SearchText, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            Color oldColor = GUI.color;
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            foreach (var plugin in m_Setting.plugins)
            {
                // 过滤：名称或描述包含搜索内容才显示（忽略大小写）
                if (!string.IsNullOrEmpty(m_SearchText))
                {
                    string search = m_SearchText.ToLower();
                    if ((plugin.name == null || !plugin.name.ToLower().Contains(search)) &&
                        (plugin.displayName == null || !plugin.displayName.ToLower().Contains(search)) &&
                        (plugin.description == null || !plugin.description.ToLower().Contains(search)))
                    {
                        continue;
                    }
                }

                if (plugin.Texture == null && !string.IsNullOrEmpty(plugin.banner))
                {
                    string imagePath = Path.Combine(Application.dataPath, "../Plugins", plugin.banner);
                    if(!File.Exists(imagePath))
                    {
                        imagePath = Path.Combine(Application.dataPath, "../Plugins", plugin.name, plugin.banner);
                    }
                    if (!File.Exists(imagePath))
                    {
                        imagePath = Path.Combine(Application.dataPath, "../Plugins", plugin.displayName, plugin.banner);
                    }
                    plugin.Texture = EditorUtil.LoadExternalImage(imagePath);
                }

                float width = position.width - 20; // 留出滚动条和边距
                float height = 100;
                if (plugin.Texture != null)
                {
                    height = plugin.Texture.height;
                }
                Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                rect.xMin += 10;
                rect.xMax -= 10;
                rect.yMin += 10;
                rect.yMax -= 10;
                GUI.color = plugin.toggle ? Color.green : Color.gray;
                GUI.DrawTexture(new Rect(rect.x, rect.y - 2, rect.width + 5, rect.height + 4), Texture2D.whiteTexture); // 边框
                GUI.color = oldColor;
                GUILayout.BeginHorizontal();
                // 左侧勾选开关
                plugin.toggle = GUI.Toggle(new Rect(rect.x + 10, rect.y + rect.height / 2 - 20, TOGGLE_WIDTH, TOGGLE_WIDTH), plugin.toggle, "");
                if(GUI.Button(new Rect(rect.x + 2, rect.y + rect.height-30, 35, 25), "编辑"))
                {
                    PluginConfigWindow.Show(plugin, (oriPlugin, newPlugin) =>
                    {
                        if(oriPlugin!=null)
                        {
                            int index = m_Setting.plugins.IndexOf(oriPlugin);
                            m_Setting.plugins.Insert(index, newPlugin);
                            m_Setting.plugins.Remove(oriPlugin);
                            Repaint();
                        }
                    });
                }

                Rect pulginRect = new Rect(rect.x + TOGGLE_WIDTH, rect.y, rect.width - TOGGLE_WIDTH, rect.height);

                if (plugin.Texture != null)
                {
                    // 画外边框
                    GUI.color = oldColor;
                    UIDrawUtils.DrawTextureNineSlice(plugin.Texture, pulginRect, 2);
                    var nameSize = EditorStyles.boldLabel.CalcSize(new GUIContent(plugin.displayName));
                    GUI.Label(new Rect(pulginRect.xMax - nameSize.x - 5, pulginRect.y, pulginRect.width - nameSize.x, 30), plugin.displayName, EditorStyles.boldLabel);
                }
                else
                {
                    if (!string.IsNullOrEmpty(plugin.description))
                        GUI.Label(new Rect(pulginRect.x, pulginRect.y, pulginRect.width, 30), plugin.description, EditorStyles.boldLabel);
                    else
                        GUI.Label(new Rect(pulginRect.x, pulginRect.y, pulginRect.width, 30), plugin.displayName, EditorStyles.boldLabel);
                }
                var versionSize = EditorStyles.boldLabel.CalcSize(new GUIContent(plugin.version));
                GUI.Label(new Rect(pulginRect.xMax - versionSize.x - 5, pulginRect.yMax - versionSize.y - 10, versionSize.x, 30), plugin.version, EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUI.color = oldColor;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新增插件"))
            {
                PluginConfigWindow.Show(null, (ori, newPlugin) =>
                {
                    // 查找是否已存在
                    var exist = m_Setting.plugins.Find(p => p.name == newPlugin.name);
                    if (exist != null)
                    {
                        // 更新
                        exist.version = newPlugin.version;
                        exist.banner = newPlugin.banner;
                        exist.marcoWord = newPlugin.marcoWord;
                        exist.description = newPlugin.description;
                    }
                    else
                    {
                        m_Setting.plugins.Add(newPlugin);
                    }
                    Repaint();
                });
            }
            if (GUILayout.Button("保存"))
            {
                Save();
            }
            if (GUILayout.Button("应用"))
            {
                Applay();
            }
            GUILayout.EndHorizontal();
        }
        //------------------------------------------------------
        void Applay()
        {
            Save(false);
            foreach (var plugin in m_Setting.plugins)
            {
                if (string.IsNullOrEmpty(plugin.installDir))
                    continue;

                string installDir = Path.Combine(Application.dataPath, plugin.installDir, plugin.name).Replace("\\","/");
                string pluginDir = Path.Combine(Application.dataPath, "../Plugins", plugin.name).Replace("\\", "/");
                if(!Directory.Exists(pluginDir))
                {
                    pluginDir = Path.Combine(Application.dataPath, "../Plugins", plugin.displayName).Replace("\\", "/");
                }
                if (!Directory.Exists(pluginDir))
                {
                    continue;
                }
                // 1. 清空安装目录
                if (Directory.Exists(installDir))
                {
                    try
                    {
                        Directory.Delete(installDir, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"清空目录失败: {installDir} {ex.Message}");
                    }
                }

                if (!plugin.toggle)
                {
                    // 未启用，清空后不做其它处理
                    string metaFile = installDir + ".meta";
                    if(File.Exists(metaFile))
                    {
                        try
                        {
                            File.Delete(metaFile);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"删除meta文件失败: {metaFile} {ex.Message}");
                        }
                    }
                    continue;
                }

                // 查找 zip/rar
                var archiveFiles = Directory.GetFiles(pluginDir, "*.zip", SearchOption.TopDirectoryOnly);
                if (archiveFiles!=null && archiveFiles.Length>0)
                {
                    try
                    {
                        ExtractArchive(archiveFiles[0], installDir);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"解压失败: {archiveFiles[0]} 到 {installDir} {ex.Message}");
                    }
                    continue;
                }
            }
            AutoMarcros.SetMacros(null);

            EditorUtility.DisplayDialog("提示", "插件已应用，等待编译！", "确定");
        }
        //------------------------------------------------------
        private static void CopyDirectory(string srcDir, string dstDir)
        {
            Directory.CreateDirectory(dstDir);
            foreach (var file in Directory.GetFiles(srcDir))
            {
                string dstFile = Path.Combine(dstDir, Path.GetFileName(file));
                File.Copy(file, dstFile, true);
            }
            foreach (var dir in Directory.GetDirectories(srcDir))
            {
                string dstSubDir = Path.Combine(dstDir, Path.GetFileName(dir));
                CopyDirectory(dir, dstSubDir);
            }
        }
        //------------------------------------------------------
        private static void ExtractArchive(string archivePath, string extractDir)
        {
            string ext = Path.GetExtension(archivePath).ToLower();
            if (ext == ".zip")
            {
                ZipFile.ExtractToDirectory(archivePath, extractDir, true);
            }
            else if (ext == ".rar")
            {
                /*
                using (var archive = RarArchive.Open(archivePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                            entry.WriteToDirectory(extractDir, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }*/
            }
        }
        //------------------------------------------------------
        void Save(bool bPop = true)
        {
            try
            {
                string pluginCfg = Path.Combine(Application.dataPath, "../Plugins/PluginSetting.setting");
                File.WriteAllText(pluginCfg, JsonUtility.ToJson(m_Setting, true));
                AssetDatabase.Refresh();
                if(bPop) EditorUtility.DisplayDialog("提示", "插件配置已保存", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("错误", "保存插件配置失败: " + ex.Message, "确定");
            }
        }
    }
}
