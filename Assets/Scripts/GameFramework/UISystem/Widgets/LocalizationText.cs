/********************************************************************
生成日期:	2020-06-12
类    名: 	LocalizationText
作    者:	zdq
描    述:	多语言控制Text显示组件,使用方法,将此脚本挂载到显示的Text控件上
*********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using Framework.Data;

namespace Framework.Core
{
    [DisallowMultipleComponent]
    [UI.UIWidgetExport]
    [RequireComponent(typeof(Text))]
    public  class LocalizationText : LocalizationBase
    {
        public Text m_text;

        void Awake()
        {
            if (m_text == null)
            {
                m_text = GetComponent<Text>();
            }
        }

        //------------------------------------------------------
        public override void OnLanguageChangeCallback(SystemLanguage languageType)
        {
            if (m_text == null ||gameObject.activeInHierarchy == false)
            {
                return;
            }
            RefreshShow();
        }

        /// <summary>
        /// 提供一个设置id的方法,同时刷新显示
        /// </summary>
        /// <param name="id"></param>
        public void UpdateLocalization(uint id)
        {
            if (ID == id) return;
            ID = id;
            RefreshShow();
        }

        //------------------------------------------------------
        [ContextMenu("测试显示")]
        public override void RefreshShow()
        {
            if (m_text == null || ID == 0)
            {
                return;
            }
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif
            string text = Base.LocalizationUtil.Convert(ID);
            if (text != null)
            {
                m_text.text = text;
            }
        }
    }
}
