/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UICommonScaler
作    者:	HappLI
描    述:	UI 按钮缩放公共处理类
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
namespace Framework.UI
{
    public class UICommonScaler
    {
        static HashSet<string> ms_IngoreNames = null;
        public static bool IsIngoreSet(string name)
        {
            if(ms_IngoreNames == null)
            {
                ms_IngoreNames = new HashSet<string>();
                ms_IngoreNames.Add("bg");
                ms_IngoreNames.Add("BG");
                ms_IngoreNames.Add("mask");
                ms_IngoreNames.Add("Mask");
                ms_IngoreNames.Add("MaskBG");
                ms_IngoreNames.Add("Mask_bg");
                ms_IngoreNames.Add("mask_bg");
            }
            return ms_IngoreNames.Contains(name);
        }
        static RtgTween.RtgTweenerParam ms_ScaleAnimation = new RtgTween.RtgTweenerParam();
        static RtgTween.RtgTweenerParam ms_ColorAnimation = new RtgTween.RtgTweenerParam();

        private static UnityEngine.UI.Graphic ms_pGraphic = null;
        private static Color ms_OriginalGrphicColor = Color.white;

        private static Transform ms_TransScaler = null;
        private static Vector2 ms_ScaleFromTo = Vector2.one;
        private static Vector3 ms_OriginalScale = Vector3.one;
        //------------------------------------------------------
        public static void ReplaceOriginalScaler(Transform pTrans, Vector3 originalScale)
        {
            if (ms_TransScaler != pTrans)
                return;
            if (ms_ScaleAnimation.tweenerID == 0) return;
            ms_OriginalScale = originalScale;
            ms_ScaleAnimation.from.setVector3(ms_OriginalScale* ms_ScaleFromTo.x);
            ms_ScaleAnimation.to.setVector3(ms_OriginalScale * ms_ScaleFromTo.y);
            RtgTween.RtgTweenerManager.getInstance().UpdateTween(ms_ScaleAnimation);
        }
        //------------------------------------------------------
        public static void AddScaler(Transform pTrans, Vector3 originalScale, float from, float to, float fTime)
        {
            if(ms_TransScaler != pTrans)
            {
                if (ms_TransScaler) ms_TransScaler.localScale = ms_OriginalScale;
            }
            RtgTween.RtgTweenerManager.getInstance().removeTween(ms_ScaleAnimation);

            ms_ScaleFromTo.x = from;
            ms_ScaleFromTo.y = to;
            ms_OriginalScale = originalScale;
            ms_TransScaler = pTrans;

            ms_ScaleAnimation.property = RtgTween.ETweenPropertyType.Scale;
            ms_ScaleAnimation.from.setVector3(originalScale*from);
            ms_ScaleAnimation.to.setVector3(originalScale*to);
            ms_ScaleAnimation.bLocal = true;
            ms_ScaleAnimation.loop = 1;
            ms_ScaleAnimation.initialValue = 0;
            ms_ScaleAnimation.finalValue = 1;
            ms_ScaleAnimation.equation = RtgTween.EQuationType.RTG_EASE_IN_OUT;
            ms_ScaleAnimation.transition = RtgTween.EEaseType.RTG_BACK;
            ms_ScaleAnimation.pController = pTrans;
            ms_ScaleAnimation.time = fTime;
            ms_ScaleAnimation.listerner = OnScaleTweenerDelegate;
            ms_ScaleAnimation.tweenerID = RtgTween.RtgTweenerManager.getInstance().addTween(ref ms_ScaleAnimation);
        }
        //------------------------------------------------------
        static void OnScaleTweenerDelegate(RtgTween.RtgTweenerParam param, RtgTween.ETweenStatus status)
        {
            if (status == RtgTween.ETweenStatus.Complete)
                ms_ScaleAnimation.Reset();
        }
        //------------------------------------------------------
        public static void ReplaceOriginalHightLight(UnityEngine.UI.Graphic uiGraphic, Color originalColor)
        {
            if (ms_pGraphic != uiGraphic)
                return;
            if (ms_ColorAnimation.tweenerID == 0) return;
            ms_OriginalGrphicColor = originalColor;
            ms_ColorAnimation.to.setColor(originalColor);
            RtgTween.RtgTweenerManager.getInstance().UpdateTween(ms_ColorAnimation);
        }
        //------------------------------------------------------
        public static void AddHightLight(UnityEngine.UI.Graphic uiGraphic, Color originalColor, Color from, Color to, float fTime)
        {
            if (ms_pGraphic != uiGraphic)
            {
                if (ms_pGraphic) ms_pGraphic.color = ms_OriginalGrphicColor;
            }
            RtgTween.RtgTweenerManager.getInstance().removeTween(ms_ColorAnimation);

            ms_OriginalGrphicColor = originalColor;
            ms_pGraphic = uiGraphic;

            ms_ColorAnimation.property = RtgTween.ETweenPropertyType.Scale;
            ms_ColorAnimation.from.setColor( from);
            ms_ColorAnimation.to.setColor( to);
            ms_ColorAnimation.bLocal = true;
            ms_ColorAnimation.loop = 1;
            ms_ColorAnimation.initialValue = 0;
            ms_ColorAnimation.finalValue = 1;
            ms_ColorAnimation.pController = ms_pGraphic;
            ms_ColorAnimation.time = fTime;
            ms_ColorAnimation.listerner = OnColorTweenerDelegate;
            ms_ColorAnimation.tweenerID = RtgTween.RtgTweenerManager.getInstance().addTween(ref ms_ColorAnimation);
        }
        //------------------------------------------------------
        static void OnColorTweenerDelegate(RtgTween.RtgTweenerParam param, RtgTween.ETweenStatus status)
        {
            if (status == RtgTween.ETweenStatus.Complete)
                ms_ColorAnimation.Reset();
        }
        //------------------------------------------------------
        public static void Clear()
        {
            if (ms_TransScaler)
                ms_TransScaler.localScale = ms_OriginalScale;
            ms_TransScaler = null;
            RtgTween.RtgTweenerManager.getInstance().removeTween(ms_ScaleAnimation);
            ms_ScaleAnimation.Reset();

            if (ms_pGraphic) ms_pGraphic.color = ms_OriginalGrphicColor;
            ms_pGraphic = null;
            RtgTween.RtgTweenerManager.getInstance().removeTween(ms_ColorAnimation);
            ms_ColorAnimation.Reset();
        }
        //------------------------------------------------------
        public static void OnDestroy(Transform pTrans)
        {
            if (ms_TransScaler == pTrans)
            {
                Clear();
            }
        }
    }
}