/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideStepType
作    者:	HappLI
描    述:	步骤节点类型
*********************************************************************/

namespace Framework.Guide
{
    [GuideExport("步骤")]
    public enum GuideStepType
    {
        [GuideStep("控件点击", true)]
        [GuideArgv("控件", "widgetGUID", "", typeof(GuideGuid), EArgvFalg.PortAll)]
        [GuideStrArgv("Tag", "widgetTag", "", EArgvFalg.PortAll)]
        [GuideArgv("控件索引", "index", "", null, EArgvFalg.PortAll)]
        [GuideStrArgv("控件名称", "listenerName", "动态加载查找的控件名称")]
        [GuideArgv("手势类型", "fingerType", "", typeof(EFingerType))]
        [GuideArgv("角度", "rotate", "")]
        [GuideArgv("偏移X", "offsetX", "")]
        [GuideArgv("偏移Y", "offsetY", "")]
        [GuideArgv("RayTest", "RayTest", "用于检测监听控件是否可点击", typeof(bool))]
        [GuideArgv("克隆到顶层", "MostTop", "", typeof(bool))]
        [GuideArgv("蒙版遮罩", "bMask", "", typeof(bool))]
        [GuideArgv("型状", "maskType", "", typeof(EMaskType), EArgvFalg.All, defaultValue:EMaskType.Circel)]
        [GuideArgv("蒙版颜色", "maskColor", "", typeof(UnityEngine.Color))]
        [GuideArgv("蒙版形状缩放", "maskScale", "", typeof(UnityEngine.Vector2), defaultValue:"1,1")]
        [GuideArgv("蒙版形状过渡", "tweenSpeed", "从无到有的过渡速度,值越大过渡的越快", typeof(float), EArgvFalg.All)]
        [GuideArgv("按任意键完成", "bPressAnyKey", "", typeof(bool), EArgvFalg.GetAndPort)]
        [GuideArgv("是否自动执行", "bAutoCallNext", "", typeof(bool), EArgvFalg.GetAndPort)]
        ClickUI = 100,

        [GuideStep("滑动(暂未支持)", true)]
        [GuideArgv("手势类型", "fingerType", "", typeof(EFingerType))]
        [GuideArgv("角度", "rotate", "")]
        [GuideArgv("起点3D", "IsStart3DPos", "是否为3D坐标,如果没勾选，则为屏幕比率", typeof(bool))]
        [GuideArgv("参数X", "startPosX", "")]
        [GuideArgv("参数Y", "startPosY", "")]
        [GuideArgv("起点大小", "startSize", "")]
        [GuideArgv("终点3D", "IsEnd3DPos", "是否为3D坐标,如果没勾选，则为屏幕比率", typeof(bool))]
        [GuideArgv("参数X", "endPosX", "")]
        [GuideArgv("参数Y", "endPosY", "")]
        [GuideArgv("终点大小", "endSize", "")]
        [GuideArgv("滑动速度(默认100)", "speed", "")]
        Slide = 101,

        [GuideStep("滑动(只判断方向)(暂未支持)", true)]
        [GuideArgv("起点X", "startPosX", "")]
        [GuideArgv("起点Y", "startPosY", "")]
        [GuideArgv("终点X", "endPosX", "")]
        [GuideArgv("终点Y", "endPosY", "")]
        [GuideArgv("速度(默认100)", "speed", "")]
        [GuideArgv("判断滑动成功角度临界值", "checkAngle", "")]
        SlideCheckDirection = 102,

        [GuideStep("滑动(立刻判断滑动方向不用松开滑动手指)(暂未支持)", true)]
        [GuideArgv("起点X", "startPosX", "")]
        [GuideArgv("起点Y", "startPosY", "")]
        [GuideArgv("终点X", "endPosX", "")]
        [GuideArgv("终点Y", "endPosY", "")]
        [GuideArgv("速度(默认100)", "speed", "")]
        [GuideArgv("判断滑动成功角度临界值", "checkAngle", "")]
        SlideCheckDirectionImmediately = 103,

        [GuideStep("点击区域", true)]
        [GuideArgv("手势类型", "fingerType", "", typeof(EFingerType))]
        [GuideArgv("角度", "rotate", "")]
        [GuideArgv("是否3D", "IsStart3DPos", "是否为3D坐标,如果没勾选，则为屏幕比率", typeof(bool))]
        [GuideArgv("参数X", "startPosX", "", typeof(float))]
        [GuideArgv("参数Y", "startPosY", "", typeof(float))]
        [GuideArgv("参数Z", "startPosZ", "", typeof(float))]
        [GuideArgv("区域大小", "Size", "", typeof(float))]
        [GuideArgv("蒙版遮罩", "bMask", "", typeof(bool))]
        [GuideArgv("型状", "maskType", "", typeof(EMaskType), EArgvFalg.All, defaultValue: EMaskType.Circel)]
        [GuideArgv("蒙版颜色", "maskColor", "", typeof(UnityEngine.Color))]
        [GuideArgv("蒙版形状缩放", "maskScale", "", typeof(UnityEngine.Vector2), defaultValue: "1,1")]
        [GuideArgv("蒙版形状过渡", "tweenSpeed", "从无到有的过渡速度,值越大过渡的越快", typeof(float), EArgvFalg.All)]
        ClickZoom = 104,

        [GuideStep("任意点击")]
        ClickAnywhere = 105,

        [GuideStep("等待Gameobject激活状态")]
        [GuideArgv("控件", "widgetID", "控件ID,需要在控件上绑定GuideGuid组件", typeof(GuideGuid), EArgvFalg.GetAndPort)]
        [GuideStrArgv("Tag", "widgetTag", "", EArgvFalg.PortAll)]
        [GuideArgv("是否显示", "isShow", "", null, EArgvFalg.GetAndPort)]
        WaitGameobjectActive = 106,

        [GuideStep("等待目标可以点击")]
        [GuideArgv("guid", "widgetID", "控件ID,需要在控件上绑定GuideGuid组件", typeof(GuideGuid), EArgvFalg.GetAndPort)]
        [GuideStrArgv("Tag", "widgetTag", "", EArgvFalg.PortAll)]
        WaitGameobjectCanClick = 107,

        [GuideDisable]
        CustomBegin = GuideTriggerDef.CustomEnd,
        [GuideDisable]
        CustomEnd = CustomBegin +2000,
    }
}

