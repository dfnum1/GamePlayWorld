/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideExcudeType
作    者:	HappLI
描    述:	内置执行节点类型
*********************************************************************/
namespace Framework.Guide
{
    [GuideTrigger("触发器")]
    public enum GuideTriggerDef
    {
        [GuideDisable]
        CustomBegin = 1000,
        [GuideDisable]
        CustomEnd = CustomBegin+2000,
    }
}

