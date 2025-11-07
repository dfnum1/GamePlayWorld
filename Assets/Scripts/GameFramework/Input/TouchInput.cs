/********************************************************************
生成日期:	23:3:2020   16:23
类    名: 	TouchInput
作    者:	HappLI
描    述:	输入模块
*********************************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Framework.Core
{
    public class TouchInput : ATouchInput
    {
        static TouchInput ms_TouchInput = null;
        public static TouchInput getInstance()
        {
            return ms_TouchInput;
        }
        //-------------------------------------------------
        protected override void OnAwake()
        {
            ms_TouchInput = this;
        }
        //-------------------------------------------------
        protected override void OnDestroy()
        {
            ms_TouchInput = null;
        }
        //-------------------------------------------------
        public static bool IsUITouching()
        {
            if (ms_TouchInput == null) return false;
            return ms_TouchInput.isUITouch;
        }
    }
}