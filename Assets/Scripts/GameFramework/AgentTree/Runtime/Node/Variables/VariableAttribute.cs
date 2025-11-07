/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	VariableTypeAttribute
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
namespace Framework.Plugin.AT
{
    public class VariableTypeAttribute : Attribute
    {
        public EVariableType valType;
        public string DisplayName;
        public VariableTypeAttribute(EVariableType type, string DisplayName="")
        {
            valType = type;
            if (string.IsNullOrEmpty(DisplayName))
                this.DisplayName = type.ToString();
            else
                this.DisplayName = DisplayName;
        }
    }
}
