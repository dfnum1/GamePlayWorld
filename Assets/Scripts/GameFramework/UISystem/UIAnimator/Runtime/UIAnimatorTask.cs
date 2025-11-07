/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.RtgTween;
using UnityEngine;

namespace Framework.UI
{
    public interface UIAnimatorTask
    {
        bool IsValid();
        void Clear();
        void SetReverse(bool bReverse);
        void Start();
        void Stop();
        void StopRecover();
        Transform GetTransform();
        RectTransform GetRectTransform();
        CanvasGroup GetCanvasGroup();
        Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> GetUIGraphics();
        Camera GetCamera();
        void BackUp();
        void Recover();
        void Update(int fMillisecondRuntime, int fFrameTime, bool bAutoStop= true);
        float GetCurrentDelta();
        void SetCurrentDelta(float fDelta);
        float GetTrackDuration();
        bool IsEnd();
        UIBaseParameter GetParameter();
        ELogicController GetControllerType();
        string GetControllerName();
        int GetControllerTag();
        void SetController(UnityEngine.Object pController);
        UnityEngine.Object GetController();
    }
}

