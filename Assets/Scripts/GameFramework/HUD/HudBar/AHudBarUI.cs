/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	HudBar
作    者:	HappLI
描    述:   头顶进度Bar UI控件
*********************************************************************/

using UnityEngine;

namespace Framework.UI
{
    public abstract class AHudBarUI : MonoBehaviour
    {
        public abstract void SetValue(byte attrType, float newValue, float oldValue = 1, bool isSetDefaultValue = false);
        public abstract void Clear();
    }
}
