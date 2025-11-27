/********************************************************************
生成日期:	11:07:2025
类    名: 	AssetDrawLogic
作    者:	HappLI
描    述:	资源面板逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.War.Editor
{
    [EditorBinder(typeof(WarWorldEditor), "AssetRect", -1)]
    public class AssetDrawLogic : AWarWorldLogic
    {
        public enum ETab
        {
            WarEle,
            AgentLibrary,
        }
        static string[] TABS = new string[] { "战争元素", "Agent图书馆" };
        ETab m_eTab = ETab.WarEle;
        //--------------------------------------------------------
        protected override void OnEnable()
        {
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            Rect rect = GetRect();

            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, 20));

            GUILayout.BeginHorizontal();
            Color color = GUI.color;
            for (int i = 0; i < TABS.Length; ++i)
            {
                GUI.color = (m_eTab == (ETab)i) ? Color.yellow : color;
                if (GUILayout.Button(TABS[i]))
                {
                    m_eTab = (ETab)i;
                }
                GUI.color = color;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(rect.x, rect.y+20, rect.width, rect.height));
            GUILayout.EndArea();
            UIDrawUtils.DrawColorLine(new Vector2(rect.xMin, rect.y+20 ), new Vector2(rect.xMax, rect.y + 20), new Color(1, 1, 1, 0.5f));
        }
    }
}

#endif