/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	RtgTweenerManager
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.RtgTween
{
    public class RtgTweenUtil
    {
        protected static RtgEasing[] m_pErasingFuncs = null;
        static RtgEasing[] Easings
        {
            get
            {
                Init();
                return m_pErasingFuncs;
            }
        }
        //------------------------------------------------------
        static void Init()
        {
            if(m_pErasingFuncs == null)
            {
                m_pErasingFuncs = new RtgEasing[(int)EEaseType.RTG_NUM];
                m_pErasingFuncs[(int)EEaseType.RTG_LINEAR] = new RtgLinear();
                m_pErasingFuncs[(int)EEaseType.RTG_SINE] = new RtgSine();
                m_pErasingFuncs[(int)EEaseType.RTG_QUINT] = new RtgQuint();
                m_pErasingFuncs[(int)EEaseType.RTG_QUART] = new RtgQuart();
                m_pErasingFuncs[(int)EEaseType.RTG_QUAD] = new RtgQuad();
                m_pErasingFuncs[(int)EEaseType.RTG_EXPO] = new RtgExpo();
                m_pErasingFuncs[(int)EEaseType.RTG_ELASTIC] = new RtgElastic();
                m_pErasingFuncs[(int)EEaseType.RTG_CUBIC] = new RtgCubic();
                m_pErasingFuncs[(int)EEaseType.RTG_CIRC] = new RtgCirc();
                m_pErasingFuncs[(int)EEaseType.RTG_BOUNCE] = new RtgBounce();
                m_pErasingFuncs[(int)EEaseType.RTG_BACK] = new RtgBack();
            }
        }
        //------------------------------------------------------
        public static float runEquation(EEaseType transition, EQuationType equation, float t, float b, float c, float d)
        {
            if (d <= 0) return c;
            if (EEaseType.RTG_NUM == transition) return t;
            Init();
            float result;
            if (equation == EQuationType.RTG_EASE_IN)
            {
                result = m_pErasingFuncs[(int)transition].EaseIn(t, b, c, d);
            }
            else if (equation == EQuationType.RTG_EASE_OUT)
            {
                result = m_pErasingFuncs[(int)transition].EaseOut(t, b, c, d);
            }
            else
            {
                result = m_pErasingFuncs[(int)transition].EaseInOut(t, b, c, d);
            }
            return result;
        }
    }
    public class RtgTweenerManager : Framework.Base.Singleton<RtgTweenerManager>
    {
        public enum ECallBackType { ON_START, ON_STEP, ON_COMPLETE };


        protected int m_nTweenerID = 0;
        protected List<RtgTweenerParam> m_listTweens = new List<RtgTweenerParam>(64);

        //------------------------------------------------------
        public RtgTweenerManager()
        {

        }
        //------------------------------------------------------
        public void clear()
        {
            m_nTweenerID = 0;
            m_listTweens.Clear();
        }
        //------------------------------------------------------
        public int BuildTweeneID()
        {
            return m_nTweenerID++;
        }
        //------------------------------------------------------
        public int addTween(ref RtgTweenerParam param, float fDelay = 0)
        {
            param.Reset();
            param.tweenerID = BuildTweeneID();
            Init(ref param);
            m_listTweens.Add(param);
            if(fDelay>0)
            {
                RtgTweenerParam tween = m_listTweens[m_listTweens.Count - 1];
                tween.delay += fDelay;
                m_listTweens[m_listTweens.Count - 1] = tween;
            }
            return param.tweenerID;
        }
        //------------------------------------------------------
        public void UpdateTween(RtgTweenerParam param)
        {
            if (param.tweenerID < 0) return;
            for (int i = 0; i < m_listTweens.Count; ++i)
            {
                if (param.tweenerID == m_listTweens[i].tweenerID)
                {
                    m_listTweens[i] = param;
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void removeTween(RtgTweenerParam param)
        {
            if (param.tweenerID < 0) return;
            for (int i = 0; i < m_listTweens.Count; ++i)
            {
                if (param.tweenerID == m_listTweens[i].tweenerID)
                {
                    if(m_listTweens[i].backupReover)
                        m_listTweens[i].Recover();
                    m_listTweens.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public bool findTween(RtgTweenerParam param)
        {
            if (param.tweenerID < 0) return false;
            for (int i = 0; i < m_listTweens.Count; ++i)
            {
                if (param.tweenerID == m_listTweens[i].tweenerID)
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsTweening(Transform pObj)
        {
            for(int i = 0; i < m_listTweens.Count; ++i)
            {
                if (m_listTweens[i].pController == pObj) return true;
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsTweening()
        {
            return m_listTweens.Count > 0;
        }
        //------------------------------------------------------
        void Init(ref RtgTweenerParam param, bool bApply = true)
        {
            param.BackUp();
            param.tweeningValue.property1 = param.from.property1;
            param.tweeningValue.property2 = param.from.property2;
            param.tweeningValue.property3 = param.from.property3;
            param.tweeningValue.property4 = param.from.property4;

            if(bApply) ApplayProperty(ref param);
        }
        //------------------------------------------------------
        public static void DoMove(Transform pCtl, Vector3 toPosition, float fTime, EEaseType transition = EEaseType.RTG_LINEAR, EQuationType equation = EQuationType.RTG_EASE_OUT, int loop = 1)
        {
            RtgTweenerParam param = new RtgTweenerParam();
            param.pController = pCtl;
            param.property = ETweenPropertyType.Position;
            param.from = new RtgTweenerProperty(); param.from.setVector3(pCtl.position);
            param.to = new RtgTweenerProperty(); param.to.setVector3(toPosition);
            param.time = fTime;
        }
        //------------------------------------------------------
        public void update(long currentMillis)
        {
            int t = 0;
            int d = 0;
       //     currentMillis = Mathf.Min((int)currentMillis, GetMaxFrameTimeMillisecondStep());

            RtgTweenerParam param;
            for (int i = 0; i < m_listTweens.Count;)
            {
                param = m_listTweens[i];
                if (!param.started)
                {
                    Init(ref param);
                    param.BackUp();
                    if (param.listerner != null)
                    {
                        param.listerner(param, ETweenStatus.Start);
                        if (i >= m_listTweens.Count) break;
                    }
                    param.started = true;
                }
                param.runtime += currentMillis;
                if (param.delay>0)
                {
                    if(param.runtime < (long)(param.delay*1000) )
                    {
                        m_listTweens[i] = param;
                        ++i;
                        continue;
                    }
                }

                if (param.listerner != null)
                {
                    param.listerner(param, ETweenStatus.Steping);
                    if (i >= m_listTweens.Count) break;
                }

                t = (int)param.runtime - (int)(param.delay*1000);

                d = (int)( param.time * 1000);
                if (t <= d )
                {
                    float fFactor = 1;
                    if (BaseUtil.IsValidCurve(param.lerpCurve) && d>0) fFactor = param.lerpCurve.Evaluate((float)t / (float)d);
                    float res = RtgTweenUtil.runEquation(param.transition, param.equation, (float)t, param.runtime_initialValue, (param.runtime_finalValue - param.runtime_initialValue), (float)d);
                    param.tweeningValue.property1 = ((param.to.property1 - param.from.property1) * res + param.from.property1)* fFactor;
                    param.tweeningValue.property2 = ((param.to.property2 - param.from.property2) * res + param.from.property2) * fFactor;
                    param.tweeningValue.property3 = ((param.to.property3 - param.from.property3) * res + param.from.property3) * fFactor;
                    param.tweeningValue.property4 = ((param.to.property4 - param.from.property4) * res + param.from.property4) * fFactor;

                    ApplayProperty(ref param);
                    m_listTweens[i] = param;
                    ++i;
                }
                else
                {
                    t = d;
                    float fFactor = 1;
                    if (BaseUtil.IsValidCurve(param.lerpCurve) && d > 0) fFactor = param.lerpCurve.Evaluate((float)t / (float)d);
                    float res = RtgTweenUtil.runEquation(param.transition, param.equation, (float)t, param.runtime_initialValue, (param.runtime_finalValue - param.runtime_initialValue), (float)d);
                    param.tweeningValue.property1 = ((param.to.property1 - param.from.property1) * res + param.from.property1)* fFactor;
                    param.tweeningValue.property2 = ((param.to.property2 - param.from.property2) * res + param.from.property2) * fFactor;
                    param.tweeningValue.property3 = ((param.to.property3 - param.from.property3) * res + param.from.property3) * fFactor;
                    param.tweeningValue.property4 = ((param.to.property4 - param.from.property4) * res + param.from.property4) * fFactor;
                    ApplayProperty(ref param);

                    if (param.loop>=0)
                    {
                        param.runtimeLoop++;
                        int loop = param.loop;
                        if (param.pingpong) loop *= 2;
                        if (param.loop > 0 && param.runtimeLoop >= loop)
                        {
                            if (param.pingpong) param.tweeningValue = param.from;
                            else param.tweeningValue = param.to;
                            ApplayProperty(ref param);
                            m_listTweens.RemoveAt(i);
                            if (param.listerner != null)
                                param.listerner(param, ETweenStatus.Complete);
                        }
                        else
                        {
                            if (param.pingpong)
                            {
                                float fTem = param.runtime_initialValue;
                                param.runtime_initialValue = param.runtime_finalValue;
                                param.runtime_finalValue = fTem;
                            }
                            param.runtime_delay_times++;
                            if (param.runtime_delay_times >= param.delay_times)
                            {
                                param.runtime = 0;
                                param.runtime_delay_times = 0;
                            }
                            else
                                param.runtime = (long)(param.delay * 1000);

                            m_listTweens[i] = param;
                            ++i;
                        }
                    }
                    else
                    {
                        if(param.pingpong) param.tweeningValue = param.from;
                        else param.tweeningValue = param.to;
                        ApplayProperty(ref param);

                        m_listTweens.RemoveAt(i);
                        if (param.listerner != null)
                            param.listerner(param, ETweenStatus.Complete);
                    }
                }
            }
        }
        //------------------------------------------------------
        void ApplayProperty(ref RtgTweenerParam param)
        {
            if (param.GetController() == null) return;
            switch (param.property)
            {
                case ETweenPropertyType.Position:
                    {
                        Transform transform = param.GetTransform();
                        if (transform)
                        {
                            if (param.useBackup)
                            {
                                if (param.bLocal)
                                    transform.localPosition = param.tweeningValue.toVector3() + param.getBackup().toVector3() + param.GetPivotOffset();
                                else
                                    transform.position = param.tweeningValue.toVector3() + param.getBackup().toVector3() + param.GetPivotOffset();
                            }
                            else
                            {
                                if (param.bLocal)
                                    transform.localPosition = param.tweeningValue.toVector3() + param.GetPivotOffset();
                                else
                                    transform.position = param.tweeningValue.toVector3() + param.GetPivotOffset();
                            }
                        }
                    }
                    break;
                case ETweenPropertyType.Rotate:
                    {
                        Transform transform = param.GetTransform();
                        if (transform)
                        {
                            if (param.useBackup)
                            {
                                if (param.bLocal)
                                    transform.localEulerAngles = param.tweeningValue.toVector3() + param.getBackup().toVector3();
                                else
                                    transform.eulerAngles = param.tweeningValue.toVector3() + param.getBackup().toVector3();
                            }
                            else
                            {
                                if (param.bLocal)
                                    transform.localEulerAngles = param.tweeningValue.toVector3();
                                else
                                    transform.eulerAngles = param.tweeningValue.toVector3();
                            }

                        }
                    }
                    break;
                case ETweenPropertyType.Scale:
                    {
                        Transform transform = param.GetTransform();
                        if (transform)
                        {
                            if (param.useBackup)
                            {
                                transform.localScale = param.tweeningValue.toVector3() + param.getBackup().toVector3();
                            }
                            else
                                transform.localScale = param.tweeningValue.toVector3();
                        }
                    }
                    break;
                case ETweenPropertyType.Color:
                    {
                        if (param.useBackup)
                            UI.UIGraphicUtil.UpdateGraphicColor(param.GetUIGraphics(), param.tweeningValue.toColor() + param.getBackup().toColor());
                        else
                            UI.UIGraphicUtil.UpdateGraphicColor(param.GetUIGraphics(), param.tweeningValue.toColor());
                    }
                    break;
                case ETweenPropertyType.Alpha:
                    {
                        CanvasGroup group = param.GetCanvasGroup();
                        if (group) group.alpha = param.tweeningValue.property1 * param.getBackup().toAlpha();
                        else UI.UIGraphicUtil.UpdateGraphicAlpha(param.GetUIGraphics(), param.tweeningValue.property1);
                    }
                    break;
            }
        }
        //------------------------------------------------------
        public float GetMaxFrameTimeStep()
        {
            return 0.05f;
        }
        //------------------------------------------------------
        public int GetMaxFrameTimeMillisecondStep()
        {
            return 50;
        }
    }
}

