/********************************************************************
生成日期:	2020-06-12
类    名: 	LocalizationImage
作    者:	zdq
描    述:	多语言控制Image显示组件,使用方法,将此脚本挂载到显示的Image控件上
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using Framework.Data;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class LocalizationImage : LocalizationAbsImage
    {

        public Image m_Image;
        
        void Awake()
        {
            if (m_Image == null)
            {
                m_Image = GetComponent<Image>();
            }
        }

        //------------------------------------------------------
        public override void OnLanguageChangeCallback(SystemLanguage languageType)
        {
            if (m_Image == null || gameObject.activeInHierarchy == false)
            {
                return;
            }
            RefreshShow();
        }
        //------------------------------------------------------
        [ContextMenu("测试显示")]
        public override void RefreshShow()
        {
            if (m_Image == null || ID == 0)
            {
                return;
            }
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif
            string strPath = Base.LocalizationUtil.Convert(ID);
            if (strPath != null)
            {
                m_pLoader.LoadObjectAsset(m_Image, strPath);
            }
        }
        //------------------------------------------------------
        public override bool IsEmpty()
        {
            if (m_Image == null)
            {
                m_Image = GetComponent<Image>();
            }
            if (m_Image is Image)
            {
                if (m_Image.sprite != null)
                {
                    return false;
                }
                return true;
            }
            else if (m_Image is ImageEx)
            {
                if (m_Image.overrideSprite != null || m_Image.sprite != null)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
