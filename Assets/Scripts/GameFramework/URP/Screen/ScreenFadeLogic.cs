#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ScreenFadeLogic
作    者:	HappLI
描    述:	Screen 颜色过渡 逻辑
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    public enum EScreenFadeState
    {
        Begin,
        Fading,
        End,
    }
    public interface IScreenFadeCallback
    {
        void OnScreenFadeState(EScreenFadeState state, IUserData userData);
    }

    [PostPass(EPostPassType.Screen, 0)]
    public class ScreenFadeLogic : APostLogic
    {
        Color m_Color = new Color(0.01f, 0.01f, 0.01f, 0.0f);
        Color m_FadeColor = new Color(0.01f, 0.01f, 0.01f, 0.0f);

        float m_fLerpTime = 0.5f;
        AnimationCurve m_FadeCurve;
        private bool m_bFadingThershold = false;
        private bool m_bFading = false;
        float m_fFactor = 0;
        float m_fMaxTime = 0;

        struct Callback
        {
            public IUserData userData;
            public IScreenFadeCallback callback;
            public Callback(IScreenFadeCallback callback, IUserData userData)
            {
                this.callback = callback;
                this.userData = userData;
            }
        }
        List<Callback> m_vCallbacks = null;
        //------------------------------------------------------
        public override void Awake()
        {
            base.Awake();
            m_bFadingThershold = false;
            m_bFading = false;
            m_FadeCurve = null;
            m_fFactor = 0;
            m_fMaxTime = 0;
            if (m_vCallbacks != null) m_vCallbacks.Clear();
        }
        //------------------------------------------------------
        public static bool IsFading()
        {
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic == null) return false;
            return logic.m_bFading;
        }
        //------------------------------------------------------
        public static void RegisterCallback(IScreenFadeCallback callback, IUserData userData)
        {
            if (callback == null) return;
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic == null) return;
            if (logic.m_vCallbacks == null) logic.m_vCallbacks = new List<Callback>(2);
            for(int i =0; i < logic.m_vCallbacks.Count; ++i)
            {
                if (logic.m_vCallbacks[i].callback == callback) return;
            }
            logic.m_vCallbacks.Add(new Callback(callback, userData));
            if(logic.m_bFading)
            {
                callback.OnScreenFadeState(EScreenFadeState.Begin, userData);
                if (logic.m_bFadingThershold)
                {
                    callback.OnScreenFadeState(EScreenFadeState.Fading, userData);
                }
            }
        }
        //------------------------------------------------------
        public static void UnRegisterCallback(IScreenFadeCallback callback)
        {
            if (callback == null) return;
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic == null) return;
            if (logic.m_vCallbacks == null) return;
            for (int i = 0; i < logic.m_vCallbacks.Count;)
            {
                if (logic.m_vCallbacks[i].callback == callback)
                {
                    logic.m_vCallbacks.RemoveAt(i);
                }
                else ++i;
            }
        }
        //------------------------------------------------------
        public static void Fade(Color fadeColor, float fLerpTime = 0.5f, float fFactor = 1)
        {
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic != null) logic.BeginFade(fadeColor, fLerpTime, fFactor);
        }
        //------------------------------------------------------
        void BeginFade(Color fadeColor, float fLerpTime = 0.5f, float fFactor = 1)
        {
            m_Color = m_FadeColor;
            m_FadeColor = fadeColor;
            m_fMaxTime = fLerpTime;
            m_fFactor = fFactor;
            m_bFading = true;
            m_bFadingThershold = false;
            if (m_vCallbacks!=null)
            {
                Callback callback;
                for(int i= 0; i < m_vCallbacks.Count; ++i)
                {
                    callback = m_vCallbacks[i];
                    callback.callback.OnScreenFadeState(EScreenFadeState.Begin, callback.userData);
                }
            }
        }
        //------------------------------------------------------
        public static void Fade(Color fadeColor, AnimationCurve curve, float fLerpTime = 0.5f)
        {
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic != null) logic.BeginFade(fadeColor, curve, fLerpTime);
        }
        //------------------------------------------------------
        void BeginFade(Color fadeColor, AnimationCurve curve, float fLerpTime = 0.5f)
        {
            m_FadeCurve = curve;
            m_fMaxTime = BaseUtil.GetCurveMaxTime(curve);
            m_fFactor = 0;
            m_Color = fadeColor;
            m_FadeColor = fadeColor;
            m_fLerpTime = fLerpTime;
            m_bFadingThershold = false;
            m_bFading = curve != null && m_fMaxTime > 0;
            if (m_RenderPass != null)
            {
                ScreenPassSettings setting = m_RenderPass.GetSetting() as ScreenPassSettings;
                if (setting.blitMat)
                    setting.blitMat.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
            }
            if (m_bFading && m_vCallbacks != null)
            {
                Callback callback;
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    callback = m_vCallbacks[i];
                    callback.callback.OnScreenFadeState(EScreenFadeState.Begin, callback.userData);
                }
            }
            Update(0);
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            ClearReset();
        }
        //------------------------------------------------------
        public static void Clear()
        {
            URP.ScreenFadeLogic logic = URP.URPPostWorker.CastPostPass<URP.ScreenFadeLogic>(EPostPassType.Screen);
            if (logic != null) logic.ClearReset();
        }
        //------------------------------------------------------
        public override void ClearReset()
        {
            base.ClearReset();
            m_Color = new Color(0.01f, 0.01f, 0.01f, 0.0f);
            m_FadeColor = new Color(0.01f, 0.01f, 0.01f, 0.0f);
            m_bFading = false;
            m_bFadingThershold = false;
            m_FadeCurve = null;
            m_fFactor = 0;
            m_fMaxTime = 0;
            if (m_vCallbacks != null) m_vCallbacks.Clear();
        }
        //------------------------------------------------------
        public override void Excude(APostRenderPass pass, CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_bFading)
            {
                ScreenPassSettings setting = pass.GetSetting() as ScreenPassSettings;
                if (setting != null)
                {
                    if (setting.blitMat)
                    {
                        Camera camera = renderingData.cameraData.camera;
                        setting.blitMat.color = m_Color;
                        cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, setting.blitMat);
                        cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Update(float fFrame)
        {
            if (m_bFading)
            {
                if (m_FadeCurve!=null)
                {
                    m_fFactor += fFrame;
                    float alpha = m_FadeCurve.Evaluate(m_fFactor);
                    m_Color.a = Mathf.Lerp(m_Color.a, alpha, m_fLerpTime);
                    if(alpha>=0.99f)
                    {
                        if(!m_bFadingThershold)
                        {
                            if (m_vCallbacks != null)
                            {
                                Callback callback;
                                for (int i = 0; i < m_vCallbacks.Count; ++i)
                                {
                                    callback = m_vCallbacks[i];
                                    callback.callback.OnScreenFadeState(EScreenFadeState.Fading, callback.userData);
                                }
                            }
                            m_bFadingThershold = true;
                        }

                    }
                    if (Mathf.Abs(m_Color.a - alpha) <= 0.1f && m_fFactor > m_fMaxTime)
                    {
                        m_bFading = false;
                        if (m_vCallbacks != null)
                        {
                            Callback callback;
                            for (int i = 0; i < m_vCallbacks.Count; ++i)
                            {
                                callback = m_vCallbacks[i];
                                callback.callback.OnScreenFadeState(EScreenFadeState.End, callback.userData);
                            }
                        }
                        ClearReset();
                    }
                }
                else
                {
                    m_Color = Color.Lerp(m_Color,m_FadeColor, fFrame* m_fFactor);
                    if (!m_bFadingThershold)
                    {
                        m_bFadingThershold = true;
                        if (m_vCallbacks != null && BaseUtil.Equal(m_Color, m_FadeColor, 0.01f))
                        {
                            Callback callback;
                            for (int i = 0; i < m_vCallbacks.Count; ++i)
                            {
                                callback = m_vCallbacks[i];
                                callback.callback.OnScreenFadeState(EScreenFadeState.Fading, callback.userData);
                            }
                        }
                    }

                    if (m_fMaxTime > 0 )
                    {
                        m_bFading = false;
                        if (m_vCallbacks != null)
                        {
                            Callback callback;
                            for (int i = 0; i < m_vCallbacks.Count; ++i)
                            {
                                callback = m_vCallbacks[i];
                                callback.callback.OnScreenFadeState(EScreenFadeState.End, callback.userData);
                            }
                            m_vCallbacks.Clear();
                        }

                        ClearReset();
                    }
                }
            }
        }
    }
}
#endif