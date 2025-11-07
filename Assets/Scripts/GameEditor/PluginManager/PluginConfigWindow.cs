/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	PluginConfigWindow
作    者:	HappLI
描    述:	插件配置窗口
*********************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
    public class PluginConfigWindow : EditorWindow
    {
        public Action<PluginManager.PluginInfo, PluginManager.PluginInfo> OnSave;
        private PluginManager.PluginInfo m_EditingPlugin;
        private PluginManager.PluginInfo m_OriPlugin;
        private bool isEdit = false;

        public static void Show(PluginManager.PluginInfo plugin, Action<PluginManager.PluginInfo,PluginManager.PluginInfo> onSave)
        {
            var window = CreateInstance<PluginConfigWindow>();
            window.titleContent = new GUIContent(plugin == null ? "新增插件" : "编辑插件");
            window.m_OriPlugin = plugin;
            window.m_EditingPlugin = plugin != null ? new PluginManager.PluginInfo
            {
                name = plugin.name,
                displayName = plugin.displayName,
                version = plugin.version,
                banner = plugin.banner,
                marcoWord = plugin.marcoWord,
                installDir = plugin.installDir,
                toggle = plugin.toggle,
                description = plugin.description
            } : new PluginManager.PluginInfo();
            window.isEdit = plugin != null;
            window.OnSave = onSave;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.Label(isEdit ? "编辑插件" : "新增插件", EditorStyles.boldLabel);
            m_EditingPlugin.displayName = EditorGUILayout.TextField("插件名", m_EditingPlugin.displayName);
            m_EditingPlugin.name = EditorGUILayout.TextField("程序名", m_EditingPlugin.name);
            m_EditingPlugin.version = EditorGUILayout.TextField("版本", m_EditingPlugin.version);
            m_EditingPlugin.banner = EditorGUILayout.TextField("Banner图片名", m_EditingPlugin.banner);
            m_EditingPlugin.marcoWord = EditorGUILayout.TextField("宏定义", m_EditingPlugin.marcoWord);
            m_EditingPlugin.installDir = EditorGUILayout.TextField("安装目录", m_EditingPlugin.installDir);
            m_EditingPlugin.description = EditorGUILayout.TextField("描述", m_EditingPlugin.description, GUILayout.Height(40));

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(m_EditingPlugin.name))
                {
                    EditorUtility.DisplayDialog("提示", "插件名不能为空", "确定");
                    return;
                }
                OnSave?.Invoke(m_OriPlugin, m_EditingPlugin);
                Close();
            }
            if (GUILayout.Button("取消", GUILayout.Height(30)))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}
