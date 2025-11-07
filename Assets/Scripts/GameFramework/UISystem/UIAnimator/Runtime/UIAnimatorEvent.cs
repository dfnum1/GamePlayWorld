/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    public enum UIAnimatorEventType
    {
        Sound,
        Shake,
        Active,
        None,
    }
    public struct UIAnimatorEvent
    {
        public UIAnimatorEventType _eType;
        public float _fTriggerTime;
        public string strParameter;

        public UIAnimatorEvent(UIAnimatorEventType type = UIAnimatorEventType.None)
        {
            _eType = type;
            _fTriggerTime = 0f;
            strParameter = null;
        }
    }
}

