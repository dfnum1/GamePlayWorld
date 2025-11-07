#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideSystem
作    者:	HappLI
描    述:	引导系统定义
*********************************************************************/

namespace Framework.Plugin.Guide
{
    public enum EUIWidgetTriggerType
    {
        [GuideDisplay("无")]
        None = 0,
        [GuideDisplay("点击")]
        Click,
        [GuideDisplay("按下")]
        Down,
        [GuideDisplay("进入")]
        Enter,
        [GuideDisplay("退出")]
        Exit,
        [GuideDisplay("弹起")]
        Up,
        [GuideDisplay("选中")]
        Select,
        [GuideDisplay("选中更新")]
        UpdateSelect,
        [GuideDisplay("拖动")]
        Drag,
        [GuideDisplay("放置")]
        Drop,
        [GuideDisplay("取消选中")]
        Deselect,
        [GuideDisplay("滚动")]
        Scroll,
        [GuideDisplay("移动")]
        Move,
        [GuideDisplay("开始拖动")]
        BeginDrag,
        [GuideDisplay("结束拖动")]
        EndDrag,
        [GuideDisplay("提交操作")]
        Submit,
        [GuideDisplay("取消")]
        Cancel,
    }

    public enum ETouchType
    {
        None,
        Begin,
        Move,
        End,
    }
}
#endif