/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TimeScaleEventParamenter
作    者:	HappLI
描    述:	游戏运行时间快慢事件
*********************************************************************/
using Framework.Base;
using System.Xml;
using UnityEngine;
#if USE_SERVER
using AnimationCurve = ExternEngine.AnimationCurve;
#endif
namespace Framework.Core
{
    //------------------------------------------------------
    [EventDeclaration((ushort)EEventType.TimeScale, "子弹时间")]
    [System.Serializable]
    public partial class TimeScaleEventParamenter : BaseEvent
    {
        [DisplayNameGUI("时间因子")]
        [StateGUIByField("bUsedCurve", "false")]
        public float timeScale = 1;

        [DisplayNameGUI("持续时间(s)")]
        [StateGUIByField("bUsedCurve", "false")]
        public float fDuration = 0;

        [DisplayNameGUI("使用曲线")]
        public bool bUsedCurve = false;

        [DisplayNameGUI("曲线轴")]
        [StateGUIByField("bUsedCurve", "true")]
        public AnimationCurve timeCurve = null;

        //-------------------------------------------
        public override void OnExecute(EventSystem pEventSystem)
        {
            var framework = pEventSystem.GetFramework();
            if (framework == null) return;
            if (bUsedCurve)
            {
                framework.ApplayTimeScaleByCurve(timeCurve);
            }
            else
            {
                framework.TimeScaleFactor = timeScale;
                framework.TimeScaleDuration = fDuration;
            }
        }
    }
}

