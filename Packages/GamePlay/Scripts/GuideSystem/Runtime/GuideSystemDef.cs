/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideSystem
作    者:	HappLI
描    述:	引导系统定义
*********************************************************************/

namespace Framework.Guide
{
    public enum ELogType
    {
        Info = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
        Break = 1 << 3,
        Asset = 1 << 4,
        Exception = 1 << 5,
    }

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

    public enum EFingerType
    {
        [GuideDisplay("点击手势")]
        Click = 0,
        [GuideDisplay("滑动手势")]
        Slide,
        [GuideDisplay("箭头手势")]
        Arrow,
        [GuideDisplay("无手势,有特效")]
        Effect,
        [GuideDisplay("无手势")]
        None,
    }

    public enum EDescBGType
    {
        [GuideDisplay("背景框")]
        Box = 0,
        [GuideDisplay("无背景")]
        None,
    }

    public enum EMaskType
    {
        [GuideDisplay("无")]
        None = 0,
        [GuideDisplay("方框")]
        Box,
        [GuideDisplay("圆形")]
        Circel,
        [GuideDisplay("菱形")]
        Diamond,
    }

    public enum EModeBit
    {
        [GuideDisplay("关键步骤(服务器记录)")]
        MasterStep = 1 << 0,
        [GuideDisplay("自动跳转到下一步骤")]
        AutoNext = 1 << 1,
        [GuideDisplay("自动执行")]
        AutoAction = 1 << 2,
    }
}

