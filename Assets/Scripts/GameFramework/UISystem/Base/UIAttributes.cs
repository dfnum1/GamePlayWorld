
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIWidgetExportAttribute
作    者:	HappLI
描    述:	UI 序列化对象容器，用于界面绑定操作对象 编辑操作
*********************************************************************/

using System;
using Framework.Core;

namespace Framework.UI
{
    public enum EUIAttr
    {
        UI,
        Logic,
        View,
    }
    public class UIAttribute : Attribute
    {
        public int uiType = 0;
        public EUIAttr attri = EUIAttr.View;
        public bool isAuto = false;
        public string marcoDefine = null;
        public UIAttribute(int type, EUIAttr attr, bool bAuto = true, string marcoDefine = null)
        {
            uiType = type;
            this.attri = attr;
            isAuto = bAuto;
            this.marcoDefine = marcoDefine;
        }
    }
    public class UIWidgetExportAttribute : ComSerializedExportAttribute
    {
        public UIWidgetExportAttribute()
        {

        }
    }
}