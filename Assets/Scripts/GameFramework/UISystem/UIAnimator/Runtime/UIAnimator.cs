/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.Core;
using Framework.RtgTween;
using UnityEngine;

namespace Framework.UI
{
    public struct UIAnimator : UIAnimatorTask
    {
        private UnityEngine.Object m_pController;
        private Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> m_vGraphics;

        private UIAnimatorParameter m_pParameter;

        bool m_bInited;
        bool m_bStarted;
        bool m_bEnding;
        long m_lLastRuntime;
        long m_lRuntime;
        int m_nRuntimeDelayTimes;
        int m_nRuntimeLoop;

       // bool m_bAniEndEvent;

        bool m_bReverse;

        RtgTween.RtgTweenerProperty m_tweeningValue;
        float m_finalValue;
        float m_initialValue;

        private RtgTween.RtgTweenerProperty m_Backup;
        private bool m_bBackuped;

        //------------------------------------------------------
        public bool IsValid()
        {
            return m_pController != null;
        }
        //------------------------------------------------------
        public void SetParameter(UIAnimatorParameter parameter)
        {
            Clear();
            m_pParameter = parameter;

            m_tweeningValue = parameter.from;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_bReverse = false;
            this.m_nRuntimeLoop = 0;
            this.m_lRuntime = 0;
            this.m_lLastRuntime = 0;
            this.m_bStarted = false;
            this.m_bEnding = true;
            this.m_bInited = false;
            this.m_bBackuped = false;
            m_pParameter = null;
            this.m_finalValue = 1;
            this.m_initialValue = 0;

           // m_bAniEndEvent = false;
            if (m_vGraphics != null) m_vGraphics.Clear();
        }
        //------------------------------------------------------
        public void SetReverse(bool bReverse)
        {
            m_bReverse = bReverse;
        }
        //------------------------------------------------------
        public void Start()
        {
            if(m_pParameter == null)
            {
                m_bStarted = false;
                return;
            }
            m_bEnding = false;
            this.m_bInited = false;
            m_bStarted = true;
            m_lRuntime = 0;
            m_lLastRuntime = 0;

            if(m_bReverse)
            {
                this.m_finalValue = m_pParameter.initialValue;
                this.m_initialValue = m_pParameter.finalValue;
            }
            else
            {
                this.m_finalValue = m_pParameter.finalValue;
                this.m_initialValue = m_pParameter.initialValue;
            }
            Init(true);
            Update(0, 0);
        }
        //------------------------------------------------------
        public void Stop()
        {
            m_bEnding = true;
            m_bStarted = false;
            if (m_pParameter == null) return;
            this.m_finalValue = m_pParameter.finalValue;
            this.m_initialValue = m_pParameter.initialValue;
            if(m_pParameter.bRecover)
            {
                Recover();
            }
        }
        //------------------------------------------------------
        public void StopRecover()
        {
            if (!m_bStarted) return;
            m_bStarted = false;
            m_bEnding = true;
            Recover();
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            if (GetController() == null) return null;
            Transform transform = GetController() as Transform;
            if (transform) return transform;
            GameObject go = GetController() as GameObject;
            if (go) return go.transform;

            Behaviour behavior = GetController() as Behaviour;
            if (behavior) return behavior.transform;
            return null;
        }
        //------------------------------------------------------
        public Camera GetCamera()
        {
            if (GetController() == null) return null;
            Camera transform = GetController() as Camera;
            if (transform) return transform;
            return null;
        }
        //------------------------------------------------------
        public RectTransform GetRectTransform()
        {
            return GetTransform() as RectTransform;
        }
        //------------------------------------------------------
        public CanvasGroup GetCanvasGroup()
        {
            return GetController() as CanvasGroup;
        }
        //------------------------------------------------------
        public Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> GetUIGraphics()
        {
            return m_vGraphics;
        }
        //------------------------------------------------------
        public RtgTweenerProperty GetBackup()
        {
            return m_Backup;
        }
        //------------------------------------------------------
        public void BackUp()
        {
            if (m_bBackuped) return;
            m_Backup = new RtgTweenerProperty();
            if (GetController() == null) return;
            if (m_vGraphics != null) m_vGraphics.Clear();
             m_bBackuped = true;
            if (m_pParameter.type == UIAnimatorElementType.POSITION)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(m_pParameter.bLocal ? transform.localPosition : transform.position);
            }
            else if (m_pParameter.type == UIAnimatorElementType.ROTATE)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(m_pParameter.bLocal ? transform.localEulerAngles : transform.eulerAngles);
            }
            else if (m_pParameter.type == UIAnimatorElementType.SCALE)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(transform.localScale);
            }
            else if (m_pParameter.type == UIAnimatorElementType.PIVOT)
            {
                RectTransform transform = GetRectTransform();
                if (transform)
                {
                    m_Backup.setVector2(transform.pivot);
                    Vector2 backup_pivot = transform.pivot;
                    transform.pivot = m_pParameter.to.toVector2();
                    Vector2 offset = (m_pParameter.to.toVector2() - backup_pivot);
                    offset.x *= transform.sizeDelta.x;
                    offset.y *= transform.sizeDelta.y;
                    transform.offsetMin += offset;
                    transform.offsetMax += offset;
                }
            }
            else if (m_pParameter.type == UIAnimatorElementType.COLOR)
            {
                m_vGraphics = UIGraphicUtil.BackupGraphic<UnityEngine.EventSystems.UIBehaviour>(m_vGraphics, GetTransform());
            }
            else if ( m_pParameter.type == UIAnimatorElementType.ALPAH)
            {
                CanvasGroup group = GetCanvasGroup();
                if (group != null)
                {
                    m_Backup.property4 = group.alpha;
                }
                else
                    m_vGraphics = UIGraphicUtil.BackupGraphic<UnityEngine.CanvasGroup>(m_vGraphics, GetTransform());
            }
        }
        //------------------------------------------------------
        public void Recover()
        {
            if (!m_bBackuped) return;
            if (GetController() == null) return;
            if (m_pParameter.type == UIAnimatorElementType.POSITION)
            {
                Transform transform = GetTransform();
                if (transform)
                {
                    if (m_pParameter.bLocal)
                        transform.localPosition = m_Backup.toVector3();
                    else
                        transform.position = m_Backup.toVector3();
                }
            }
            else if (m_pParameter.type == UIAnimatorElementType.ROTATE)
            {
                Transform transform = GetTransform();
                if (transform)
                {
                    if (m_pParameter.bLocal)
                        transform.localEulerAngles = m_Backup.toVector3();
                    else
                        transform.eulerAngles = m_Backup.toVector3();
                }
            }
            else if (m_pParameter.type == UIAnimatorElementType.SCALE)
            {
                Transform transform = GetTransform();
                if (transform) transform.localScale = m_Backup.toVector3();
            }
            else if (m_pParameter.type == UIAnimatorElementType.PIVOT)
            {
                RectTransform transform = GetRectTransform();
                if (transform)
                {
                    transform.pivot = m_Backup.toVector2();
                    Vector2 offset = (m_pParameter.to.toVector2() - m_Backup.toVector2());
                    offset.x *= transform.sizeDelta.x;
                    offset.y *= transform.sizeDelta.y;
                    transform.offsetMin -= offset;
                    transform.offsetMax -= offset;
                }
            }
            else if (m_pParameter.type == UIAnimatorElementType.COLOR)
            {
                UIGraphicUtil.RecoverGraphic(m_vGraphics);
            }
            else if (m_pParameter.type == UIAnimatorElementType.ALPAH)
            {
                CanvasGroup group = GetCanvasGroup();
                if (group) group.alpha = m_Backup.toAlpha();
                else
                    UIGraphicUtil.RecoverGraphic(m_vGraphics);
            }
            if (m_vGraphics != null) m_vGraphics.Clear();
        }
        //------------------------------------------------------
        void Init(bool bApplay = true)
        {
            if (!m_bInited)
            {
                m_tweeningValue.property1 = m_pParameter.from.property1;
                m_tweeningValue.property2 = m_pParameter.from.property2;
                m_tweeningValue.property3 = m_pParameter.from.property3;
                m_tweeningValue.property4 = m_pParameter.from.property4;

                if(bApplay) ApplayProperty();
                m_bInited = true;
            }
        }
        //------------------------------------------------------
        public void Update(int fMillisecondRuntime, int fFrameTime, bool bAutoStop = true)
        {
            if (!m_bStarted) return;
            if (m_pParameter == null) return;

            if (m_bStarted)
            {
                BackUp();
                m_lRuntime += fFrameTime;
                if (m_pParameter.delay > 0)
                {
                    if (m_lRuntime < (long)(m_pParameter.delay * 1000))
                    {
                        return;
                    }
                }
                Init(true);

                int t = (int)(m_lRuntime - (int)(m_pParameter.delay*1000));
                int d = (int)(m_pParameter.time*1000);
                if (t < d)
                {
                    float fFactor = 1;
                    if (BaseUtil.IsValidCurve(m_pParameter.lerpCurve)&&d>0) fFactor = m_pParameter.lerpCurve.Evaluate((float)t / (float)d);

                    float res = RtgTweenUtil.runEquation(m_pParameter.transition, m_pParameter.equation, (float)t, m_initialValue, (m_finalValue - m_initialValue), (float)d);

                    m_tweeningValue.property1 = ((m_pParameter.to.property1 - m_pParameter.from.property1) * res + m_pParameter.from.property1)* fFactor;
                    m_tweeningValue.property2 = ((m_pParameter.to.property2 - m_pParameter.from.property2) * res + m_pParameter.from.property2)* fFactor;
                    m_tweeningValue.property3 = ((m_pParameter.to.property3 - m_pParameter.from.property3) * res + m_pParameter.from.property3)* fFactor;
                    m_tweeningValue.property4 = ((m_pParameter.to.property4 - m_pParameter.from.property4) * res + m_pParameter.from.property4)* fFactor;

                    ApplayProperty();
                }
                else
                {
                    if (m_pParameter.loop >= 0)
                    {
                        int loop = m_pParameter.loop;
                        if (m_pParameter.pingpong) loop *= 2;
                        if (loop > 0)
                        {
                            m_nRuntimeLoop++;
                            if ( m_nRuntimeLoop < loop)
                            {
                                if (m_pParameter.pingpong) m_tweeningValue = m_pParameter.from;
                                else m_tweeningValue = m_pParameter.to;
                            }
                            else
                            {
                                if (m_pParameter.pingpong) m_tweeningValue = m_pParameter.from;
                                else m_tweeningValue = m_pParameter.to;
                                m_bEnding = true;
                                if (bAutoStop)
                                    Stop();
                            }
                            ApplayProperty();
                        }
                        else
                        {
                            if (m_pParameter.pingpong)
                            {
                                float fTem = m_initialValue;
                                m_initialValue = m_finalValue;
                                m_finalValue = fTem;
                            }
                            m_nRuntimeDelayTimes++;
                            if (m_nRuntimeDelayTimes >= m_pParameter.delay_times)
                            {
                                m_lRuntime = 0;
                                m_nRuntimeDelayTimes = 0;
                            }
                            else
                                m_lRuntime = (long)(m_pParameter.delay * 1000);
                        }
                    }
                    else
                    {
                        if (m_pParameter.pingpong) m_tweeningValue = m_pParameter.from;
                        else m_tweeningValue = m_pParameter.to;
                        ApplayProperty();
                        m_bEnding = true;
                        if (bAutoStop)
                            Stop();
                    }
                }


                if (m_pParameter.EventList != null)
                {
                    UIAnimatorEvent evt;
                    for (int i = 0; i < m_pParameter.EventList.Length; ++i)
                    {
                        evt = m_pParameter.EventList[i];
                        int time = (int)(evt._fTriggerTime * 1000);
                        if (time >= m_lLastRuntime && time < m_lRuntime)
                        {
                            //todo
                        }
                    }
                }

            }
        }
        //------------------------------------------------------
        private float GetCanvasScale()
        {
            if(AFramework.mainFramework == null) return 1.0f;
            var uiMgr = AFramework.mainFramework.uiManager;
            if (uiMgr == null || uiMgr.GetUICanvasScalerRoot() ==null) return 1.0f;
            return uiMgr.GetUICanvasScalerRoot().scaleFactor;
        }
        //------------------------------------------------------
        void ApplayProperty()
        {
            if (GetController() == null) return;
            switch (m_pParameter.type)
            {
                case UIAnimatorElementType.POSITION:
                    {
                        Transform transform = GetTransform();
                        if (transform)
                        {
                            if (m_pParameter.bOffset)
                            {
                                if (m_pParameter.bLocal)
                                    transform.localPosition = m_tweeningValue.toVector3() * GetCanvasScale() + m_Backup.toVector3();
                                else
                                    transform.position = m_tweeningValue.toVector3() * GetCanvasScale() + m_Backup.toVector3();
                            }
                            else
                            {
                                if (m_pParameter.bLocal)
                                    transform.localPosition = m_tweeningValue.toVector3() * GetCanvasScale();
                                else
                                    transform.position = m_tweeningValue.toVector3() * GetCanvasScale();
                            }
                        }
                    }
                    break;
                case UIAnimatorElementType.ROTATE:
                    {
                        Transform transform = GetRectTransform();
                        if (transform)
                        {
                            if (m_pParameter.bOffset)
                            {
                                if (m_pParameter.bLocal)
                                    transform.localEulerAngles = m_tweeningValue.toVector3() + m_Backup.toVector3();
                                else
                                    transform.eulerAngles = m_tweeningValue.toVector3() + m_Backup.toVector3();
                            }
                            else
                            {
                                if (m_pParameter.bLocal)
                                    transform.localEulerAngles = m_tweeningValue.toVector3();
                                else
                                    transform.eulerAngles = m_tweeningValue.toVector3();
                            }

                        }
                    }
                    break;
                case UIAnimatorElementType.SCALE:
                    {
                        Transform transform = GetTransform();
                        if (transform)
                        {
                            if (m_pParameter.bOffset)
                            {
                                transform.localScale = m_tweeningValue.toVector3() + m_Backup.toVector3();
                            }
                            else
                                transform.localScale = m_tweeningValue.toVector3();
                        }
                    }
                    break;
                case UIAnimatorElementType.COLOR:
                    {
                        if (m_pParameter.bOffset)
                            UIGraphicUtil.UpdateGraphicColor(m_vGraphics, m_tweeningValue.toColor() + m_Backup.toColor());
                        else
                            UIGraphicUtil.UpdateGraphicColor(m_vGraphics, m_tweeningValue.toColor());
                    }
                    break;
                case UIAnimatorElementType.ALPAH:
                    {
                        CanvasGroup group = GetCanvasGroup();
                        if (group) group.alpha = m_tweeningValue.property1 * m_Backup.toAlpha();
                        else UIGraphicUtil.UpdateGraphicAlpha(m_vGraphics, m_tweeningValue.property1);
                    }
                    break;
            }
        }
        //------------------------------------------------------
        public float GetCurrentDelta()
        {
            return m_lRuntime*0.001f;
        }
        //------------------------------------------------------
        public void SetCurrentDelta(float fDelta)
        {
            m_lLastRuntime = m_lRuntime;
            m_lRuntime = (long)(fDelta*1000);
            if(!m_bStarted)
            {

                int t = (int)(m_lRuntime - (int)(m_pParameter.delay * 1000));
                int d = (int)(m_pParameter.time * 1000);
                if (t < d)
                {
                    float fFactor = 1;
                    if(BaseUtil.IsValidCurve(m_pParameter.lerpCurve) && d > 0)  fFactor = m_pParameter.lerpCurve.Evaluate((float)t / (float)d);
                    float res = RtgTweenUtil.runEquation(m_pParameter.transition, m_pParameter.equation, (float)t, m_initialValue, (m_finalValue - m_initialValue), (float)d);
                    m_tweeningValue.property1 = ((m_pParameter.to.property1 - m_pParameter.from.property1) * res + m_pParameter.from.property1)* fFactor;
                    m_tweeningValue.property2 = ((m_pParameter.to.property2 - m_pParameter.from.property2) * res + m_pParameter.from.property2) * fFactor;
                    m_tweeningValue.property3 = ((m_pParameter.to.property3 - m_pParameter.from.property3) * res + m_pParameter.from.property3) * fFactor;
                    m_tweeningValue.property4 = ((m_pParameter.to.property4 - m_pParameter.from.property4) * res + m_pParameter.from.property4) * fFactor;

                    ApplayProperty();
                }
            }
        }
        //------------------------------------------------------
        public float GetTrackDuration()
        {
            if (m_pParameter == null) return 0;
            return m_pParameter.GetTrackDuration();
        }
        //------------------------------------------------------
        public bool IsEnd()
        {
            if (m_bStarted) return m_bEnding;
            return !m_bStarted;
        }
        //------------------------------------------------------
        public UIBaseParameter GetParameter()
        {
            return m_pParameter;
        }
        //------------------------------------------------------
        public ELogicController GetControllerType()
        {
            if (m_pParameter == null) return ELogicController.Widget;
            return m_pParameter.eControllerType;
        }
        //------------------------------------------------------
        public string GetControllerName()
        {
            if(m_pParameter == null)return null;
            return m_pParameter.strControllerName;
        }
        //------------------------------------------------------
        public int GetControllerTag()
        {
            if (m_pParameter == null) return 0;
            return m_pParameter.controllerTag;
        }
        //------------------------------------------------------
        public void SetController(UnityEngine.Object pController)
        {
            if (m_pController == pController)
                return;

            m_pController = pController;
            if(!m_bBackuped)
            {
                if (m_vGraphics != null) m_vGraphics.Clear();
            }
            BackUp();
        }
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            return m_pController;
        }
    }
}

