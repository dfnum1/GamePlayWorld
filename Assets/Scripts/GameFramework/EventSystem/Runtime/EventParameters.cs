/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	EventSystemTrigger
作    者:	HappLI
描    述:	
*********************************************************************/
namespace Framework.Core
{
    public enum EEventParamFlag : byte
    {
        UsedActor = 1<<0,
        UsedInstance = 1<<1,
    }
    //------------------------------------------------------
    public enum EEventType : ushort
    {
        [Base.DisableGUI]Base = 0,
        [Base.DisplayNameGUI("UI事件")] UIEvent = 1,
        [Base.DisplayNameGUI("实例化对象")] BornAble = 2,
        [Base.DisplayNameGUI("子弹时间")] TimeScale = 3,
        [Base.DisableGUI] Count,
    }
}

