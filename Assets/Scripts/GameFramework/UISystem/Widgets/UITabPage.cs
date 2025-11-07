/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UITabPage
作    者:	happli
描    述:	标签页
*********************************************************************/
using Framework.Core;
using UnityEngine;

namespace Framework.UI
{
    [UIWidgetExport]
    public class UITabPage : MonoBehaviour
    {
        [System.Serializable]
        struct Page
        {
            public EventTriggerListener tab;
            public UI.UISerialized page;
        }
        [SerializeField]
        Page[] pages;
        [SerializeField] int defaultPage = 0;
        private int m_nCurPage = 0;
        private void Awake()
        {
            if (pages == null) return;
            m_nCurPage = -100;
            for (int i =0; i < pages.Length; ++i)
            {
                pages[i].tab.param1 = new Variable1() { intVal = i };
                pages[i].tab.onClick += OnTabClick;
            }
            SetPage(defaultPage);
        }
        //------------------------------------------------------
        void OnTabClick(GameObject pGo, params IUserData[] param)
        {
            if (pages == null) return;
            if (param == null || param.Length <= 0) return;
            SetPage(((Variable1)param[0]).intVal);
        }
        //------------------------------------------------------
        void SetPage(int page)
        {
            if (m_nCurPage == page) return;
            m_nCurPage = page;
            if (pages == null) return;
            for (int i = 0; i < pages.Length; ++i)
            {
                if (pages[i].page == null) continue;
                if (i == m_nCurPage) pages[i].page.SetVisible(true);
                else pages[i].page.SetVisible(false);
            }
        }
    }
}
