/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideExcudeType
作    者:	HappLI
描    述:	内置执行节点类型
*********************************************************************/
using UnityEngine;

namespace Framework.Guide
{
    [GuideExport("执行器")]
    public enum GuideExcudeType
    {
        [GuideExcude("启用Mask", true)]
        [GuideArgv("蒙版", "mask", "", typeof(bool), EArgvFalg.All,defaultValue:1)]
        [GuideArgv("蒙版颜色", "color", "", typeof(Color), EArgvFalg.All)]
        [GuideArgv("启动拖拽穿透", "penetrateEnable", "拖拽穿透,默认关闭", typeof(bool), EArgvFalg.All)]
        [GuideArgv("型状", "maskType", "", typeof(EMaskType), EArgvFalg.All)]
        [GuideArgv("蒙版形状缩放", "maskScale", "", typeof(UnityEngine.Vector2))]
        [GuideArgv("穿透的控件ID", "widgetID", "控件ID,需要在控件上绑定GuideGuid组件", typeof(GuideGuid), EArgvFalg.All)]
        [GuideStrArgv("穿透的控件Tag", "widgetTag", "控件Tag,需要在控件上绑定GuideGuid组件", EArgvFalg.All)]
        [GuideArgv("过渡", "tweenSpeed", "从无到有的过渡速度,值越大过渡的越快", typeof(float), EArgvFalg.All)]
        MaskAble = 1,

        [GuideExcude("提示框",true)]
        [GuideArgv("底框类型", "BgType", "", typeof(EDescBGType), EArgvFalg.All)]
        [GuideStrArgv("NPC名称", "title", "", null, EArgvFalg.All)]
        [GuideStrArgv("文本内容", "context", "", null, EArgvFalg.All)]
        [GuideArgv("文本颜色", "color", "", typeof(Color), EArgvFalg.All)]
        [GuideArgv("是否3D", "Is3DPos", "是否为3D坐标,如果没勾选，则为屏幕比率", typeof(bool), EArgvFalg.All)]
        [GuideArgv("PositionX", "posx", "百分位", null, EArgvFalg.All)]
        [GuideArgv("PositionY", "posy", "百分位", null, EArgvFalg.All)]
        [GuideArgv("PositionZ", "posz", "百分位", null, EArgvFalg.All)]
        [GuideArgv("逐字播放", "isTransition", "是否逐字播放", typeof(bool), EArgvFalg.All)]
        [GuideArgv("播放速度", "speed", "", null, EArgvFalg.All)]
        [GuideArgv("显示对话箭头", "enableArrow", "", typeof(bool), EArgvFalg.All)]
        [GuideArgv("形象", "avatarEnable", "", typeof(bool), EArgvFalg.All)]
        [GuideStrArgv("人物形象", "avatarFile", "", null, EArgvFalg.All)]
        Tips = 2,

        [GuideExcude("提示框-跟随控件",true)]
        [GuideArgv("底框类型", "BgType", "", typeof(EDescBGType), EArgvFalg.All)]
        [GuideStrArgv("NPC名称", "title", "", null, EArgvFalg.All)]
        [GuideStrArgv("文本内容", "context", "", null, EArgvFalg.All)]
        [GuideArgv("文本颜色", "color", "", typeof(Color), EArgvFalg.All)]
        [GuideArgv("跟随控件", "widgetID", "控件ID,需要在控件上绑定GuideGuid组件", typeof(GuideGuid), EArgvFalg.PortAll)]
        [GuideStrArgv("跟随控件Tag", "widgetTag", "控件Tag,需要在控件上绑定GuideTag组件", EArgvFalg.PortAll)]
        [GuideArgv("控件索引", "widgetIndex", "控件索引", null, EArgvFalg.PortAll)]
        [GuideStrArgv("控件名称", "searchName", "", null, EArgvFalg.All)]
        [GuideArgv("偏移X", "offsetX", "百分位", null, EArgvFalg.All)]
        [GuideArgv("偏移Y", "offsetY", "百分位", null, EArgvFalg.All)]
        [GuideArgv("逐字播放", "isTransition", "是否逐字播放", typeof(bool), EArgvFalg.All)]
        [GuideArgv("播放速度", "speed", "", null, EArgvFalg.All)]
        [GuideArgv("自动隐藏时间", "autoHideTime", "毫秒", null, EArgvFalg.All)]
        [GuideArgv("显示对话箭头", "enableArrow", "", typeof(bool), EArgvFalg.All)]
        [GuideArgv("形象", "avatarEnable", "", typeof(bool), EArgvFalg.All)]
        [GuideStrArgv("人物形象", "avatarFile", "", null, EArgvFalg.All)]
        TipsByGUI = 3,

        [GuideExcude("设置引导层级", true)]
        [GuideArgv("层级", "order", "值越大，代表渲染上层", null, EArgvFalg.All,defaultValue:30000)]
        SetGuideOger = 4,

        [GuideDisable]
        CustomBegin = GuideStepType.CustomBegin+1000,
        [GuideDisable]
        CustomEnd = CustomBegin+2000,
    }
}

