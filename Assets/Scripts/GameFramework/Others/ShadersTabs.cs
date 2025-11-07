/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ShadersTabs
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Core
{
    [CreateAssetMenu(menuName = "GameData/ShaderTabs")]
    public class ShadersTabs : ScriptableObject
    {
        [System.Serializable]
        public class Item
        {
            public string name;
            public Shader shader;
            public string[] keyWorlds;
        }

        [System.Serializable]
        public struct Tabs
        {
            public string shaderName;
            public List<Item> items;
        }

        static ShadersTabs ms_Instance = null;

        public Tabs[] tabs;
        Dictionary<string, Dictionary<string, Item>> m_vTabs;
        private void OnEnable()
        {
            ms_Instance = this;
            if (tabs != null && tabs.Length > 0)
            {
                m_vTabs = new Dictionary<string, Dictionary<string, Item>>(tabs.Length);
                for (int i = 0; i < tabs.Length; ++i)
                {
                    if (string.IsNullOrEmpty(tabs[i].shaderName) || tabs[i].items == null || tabs[i].items.Count <= 0) continue;
                    Dictionary<string, Item> vsub;
                    if (!m_vTabs.TryGetValue(tabs[i].shaderName, out vsub))
                    {
                        vsub = new Dictionary<string, Item>(tabs[i].items.Count);
                        m_vTabs.Add(tabs[i].shaderName, vsub);
                    }

                    for (int j = 0; j < tabs[i].items.Count; ++j)
                    {
                        if (string.IsNullOrEmpty(tabs[i].items[j].name) || (tabs[i].items[j].shader == null && tabs[i].items[j].keyWorlds == null)) continue;
                        vsub[tabs[i].items[j].name] = tabs[i].items[j];
                    }
                }
            }
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            m_vTabs = null;
            ms_Instance = null;
        }
        //------------------------------------------------------
        public static bool IsValid()
        {
            return ms_Instance != null;
        }
        //------------------------------------------------------
        public static Item FindShader(string strShader, string tab)
        {
            if (ms_Instance == null || ms_Instance.m_vTabs == null) return null;
            if (string.IsNullOrEmpty(strShader) || string.IsNullOrEmpty(tab)) return null;

            Item shader = null;
            Dictionary<string, Item> vsub;
            if (ms_Instance.m_vTabs.TryGetValue(strShader, out vsub))
            {
                if (vsub.TryGetValue(tab, out shader))
                {
                    return shader;
                }
            }
            return null;
        }
    }
}
