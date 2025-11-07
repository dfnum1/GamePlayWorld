/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	FontSwitch
作    者:	happli
描    述:	字体切换器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    [UIWidgetExport,RequireComponent(typeof(Text))]
    public class FontSwitch : MonoBehaviour
    {
      //  static Asset ms_pSwitchFontAsset = null;
        static Font ms_pSwitchFont = null;
        private static HashSet<FontSwitch> m_vFontSwitch = new HashSet<FontSwitch>();
       // FontUpdateTracker

        public bool bCheckFont = true;

       [SerializeField]
        Text text;
        //------------------------------------------------------
        private void Awake()
        {
            m_vFontSwitch.Add(this);
            UpdateFont(ms_pSwitchFont);
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            UpdateFont(ms_pSwitchFont);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            m_vFontSwitch.Remove(this);
        }
        //------------------------------------------------------
        void UpdateFont(Font font)
        {
            if (font == null) return;
            if (text && bCheckFont)
            {
                if (text.font != font)
                    text.font = font;
            }
        }
        //------------------------------------------------------
        public static void SwitchFont(string strFont)
        {
            //if (string.IsNullOrEmpty(strFont)) return;
            //if (ms_pSwitchFontAsset != null && ms_pSwitchFontAsset.Path.CompareTo(strFont) == 0)
            //    return;
            //ms_pSwitchFont = null;
            //if (ms_pSwitchFontAsset != null)
            //    ms_pSwitchFontAsset.Release();
            //ms_pSwitchFontAsset = FileSystemUtil.LoadAsset(strFont, false, true, false);
            //if (ms_pSwitchFontAsset == null)
            //    return;
            //ms_pSwitchFontAsset.Grab();
            //ms_pSwitchFont = ms_pSwitchFontAsset.GetOrigin<Font>();
            //if (ms_pSwitchFont == null) return;
            //foreach (var db in m_vFontSwitch)
            //{
            //    db.UpdateFont(ms_pSwitchFont);
            //}
        }
    }
}
