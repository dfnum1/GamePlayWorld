/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	EPortType
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.Plugin.AT
{
    public enum EPortType : byte
    { 
        Input   = (1 << 0),
        Output  = (1 << 1),
        VariableSolt = (1<<2),
    }
}