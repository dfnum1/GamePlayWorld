/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;

namespace Framework.UI
{
    public struct UITargetBindTrack : UIAnimatorTask
    {
        private UnityEngine.Object m_pController;

        private UITargetBindTrackParameter m_pParameter;

        bool m_bInited;
        bool m_bStarted;
        bool m_bEnding;
        long m_lLastRuntime;
        long m_lRuntime;

        bool m_bReverse;

        private Vector3 m_BackupPosition;
        private Vector3 m_BackupEulerAngle;
        private Vector3 m_BackupScale;
        private float m_BackupFov;
        private bool m_bBackuped;

        //------------------------------------------------------
        public bool IsValid()
        {
            return m_pController != null;
        }
        //------------------------------------------------------
        public void SetParameter(UITargetBindTrackParameter parameter)
        {
            Clear();
            m_pParameter = parameter;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_bReverse = false;
            this.m_lRuntime = 0;
            this.m_lLastRuntime = 0;
            this.m_bStarted = false;
            this.m_bEnding = true;
            this.m_bInited = false;
            this.m_bBackuped = false;
            m_pParameter = null;
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
            this.m_bInited = false;
            m_bStarted = true;
            m_bEnding = false;
            m_lRuntime = 0;
            m_lLastRuntime = 0;
        }
        //------------------------------------------------------
        public void Stop()
        {
            m_bStarted = false;
            m_bEnding = true;
            if (m_pParameter == null) return;
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
            Camera camera = GetController() as Camera;
            if (camera) return camera;
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
            return null;
        }
        //------------------------------------------------------
        public void BackUp()
        {
            m_bBackuped = true;
            if (GetController() == null) return;
            Transform transform = GetTransform();
            if(transform)
            {
                m_BackupPosition = transform.position;
                m_BackupEulerAngle = transform.eulerAngles;
                m_BackupScale = transform.localScale;
            }
            Camera camera = GetController() as Camera;
            if (camera)
                m_BackupFov = camera.fieldOfView;
        }
        //------------------------------------------------------
        public void Recover()
        {
            if (!m_bBackuped) return;
            if (GetController() == null) return;
            Transform transform = GetTransform();
            if(transform)
            {
                transform.position = m_BackupPosition;
                transform.eulerAngles = m_BackupEulerAngle;
                transform.localScale = m_BackupScale;
            }
            Camera camera = GetController() as Camera;
            if (camera)
                camera.fieldOfView = m_BackupFov;
        }
        //------------------------------------------------------
        void Init(bool bApplay = true)
        {
            if (!m_bInited)
            {
                if(bApplay) ApplayProperty(0);
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
                float fDelay = m_pParameter.GetTrackFirst();
                float fDuration = m_pParameter.GetTrackDuration();
                Init(fDelay > 0);
                m_lRuntime += fFrameTime;
                if (fDelay > 0)
                {
                    if (m_lRuntime < (long)(fDelay * 1000))
                    {
                        return;
                    }
                }

                if (!ApplayProperty(m_lRuntime * 0.001f))
                {
                    if(bAutoStop)
                        Stop();
                    m_bEnding = true;
                }
            }
        }
        //------------------------------------------------------
        bool ApplayProperty(float time)
        {
            if (m_pParameter.datas == null || m_pParameter.datas.Count<=0 || GetController() == null) return false;
            if (time <= m_pParameter.datas[0].time)
            {
                Vector3 retValue = Vector3.zero;
                Transform transform = GetTransform();
                Camera camera = GetCamera();
                int index = 0;
                if (transform)
                {
                    if (m_pParameter.GetVector3(Core.eDeclareType.Pos, index, ref retValue))
                    {
                        transform.position = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.Euler, index, ref retValue))
                    {
                        transform.eulerAngles = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.Scale, index, ref retValue))
                    {
                        transform.localPosition = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.LookAt, index, ref retValue))
                    {
                        transform.LookAt(retValue);
                    }
                }
                if (camera)
                {
                    float fov = 45;
                    if (m_pParameter.GetFloat(Core.eDeclareType.Fov, index, ref fov))
                    {
                        camera.fieldOfView = fov;
                    }
                }
                return true;
            }
            if (time >= m_pParameter.datas[m_pParameter.datas.Count - 1].time)
            {
                int index = m_pParameter.datas.Count - 1;
                Vector3 retValue = Vector3.zero;
                Transform transform = GetTransform();
                Camera camera = GetCamera();
                if (transform)
                {
                    if (m_pParameter.GetVector3(Core.eDeclareType.Pos,index, ref retValue))
                    {
                        transform.position = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.Euler, index, ref retValue))
                    {
                        transform.eulerAngles = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.Scale, index, ref retValue))
                    {
                        transform.localPosition = retValue;
                    }
                    if (m_pParameter.GetVector3(Core.eDeclareType.LookAt, index, ref retValue))
                    {
                        transform.LookAt(retValue);
                    }
                }
                if (camera)
                {
                    float fov = 45;
                    if (m_pParameter.GetFloat(Core.eDeclareType.Fov, index, ref fov))
                    {
                        camera.fieldOfView = fov;
                    }
                }
                return false;
            }

            int __len = m_pParameter.datas.Count;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (time < m_pParameter.datas[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(m_pParameter.datas.Count - 1, __first);

            if (lhs < 0 || lhs >= m_pParameter.datas.Count || rhs < 0 || rhs >= m_pParameter.datas.Count)
                return false;

            UITargetBindTrackParameter.SplineData lhsKey = m_pParameter.datas[lhs];
            UITargetBindTrackParameter.SplineData rhsKey = m_pParameter.datas[rhs];

            float dx = rhsKey.time - lhsKey.time;
            Vector3 m1 = Vector3.zero, m2 = Vector3.zero;
            float t;
            if (dx != 0f)
            {
                t = (time - lhsKey.time) / dx;
                //  m1 = 
            }
            else
                t = 0;

            if(m_pParameter.HasProperty(Core.eDeclareType.Pos))
            {
                Vector3 outTan = Vector3.zero;
                Vector3 inTan = Vector3.zero;
                m_pParameter.GetVector3(Core.eDeclareType.Euler, lhs, ref outTan);
                m_pParameter.GetVector3(Core.eDeclareType.Euler, rhs, ref inTan);

                Vector3 lpos=Vector3.zero, rpos = Vector3.zero;
                if(m_pParameter.GetVector3(Core.eDeclareType.Pos,lhs, ref lpos) && m_pParameter.GetVector3(Core.eDeclareType.Pos,rhs, ref rpos))
                {
                    Transform transform = GetTransform();
                    if(transform)
                        transform.position = BezierUtility.Bezier4(t, lpos, m1, m2, rpos);
                }
            }

            if (m_pParameter.HasProperty(Core.eDeclareType.Euler))
            {
                Vector3 leuler = Vector3.zero, reuler = Vector3.zero;
                if (m_pParameter.GetVector3(Core.eDeclareType.Euler,lhs, ref leuler) && m_pParameter.GetVector3(Core.eDeclareType.Euler, rhs, ref reuler))
                {
                    Transform transform = GetTransform();
                    if (transform)
                        transform.eulerAngles = Vector3.Lerp(leuler, reuler, t);
                }
            }
            if (m_pParameter.HasProperty(Core.eDeclareType.LookAt))
            {
                Vector3 llook = Vector3.zero, rlook = Vector3.zero;
                if (m_pParameter.GetVector3(Core.eDeclareType.LookAt, lhs, ref llook) && m_pParameter.GetVector3(Core.eDeclareType.LookAt, rhs, ref rlook))
                {
                    Transform transform = GetTransform();
                    if (transform)
                        transform.LookAt(Vector3.Lerp(llook, rlook, t));
                }
            }
            if (m_pParameter.HasProperty(Core.eDeclareType.Scale))
            {
                Vector3 lscale = Vector3.one, rscale = Vector3.one;
                if (m_pParameter.GetVector3(Core.eDeclareType.Scale, lhs, ref lscale) && m_pParameter.GetVector3(Core.eDeclareType.Scale, rhs, ref rscale))
                {
                    Transform transform = GetTransform();
                    if (transform)
                        transform.localScale = Vector3.Lerp(lscale, rscale, t);
                }
            }
            if (m_pParameter.HasProperty(Core.eDeclareType.Fov))
            {
                float lfov = 45, rfov = 45;
                if (m_pParameter.GetFloat(Core.eDeclareType.Fov, lhs, ref lfov) && m_pParameter.GetFloat(Core.eDeclareType.Fov, rhs, ref rfov))
                {
                    Camera camera = GetCamera();
                    if (camera)
                        camera.fieldOfView = Mathf.Lerp(lfov, rfov, t);
                }
            }
            return true;
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
                ApplayProperty(m_lRuntime * 0.001f);
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
            if (m_pParameter == null) return ELogicController.GameCamera;
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
            m_pController = pController;
        }
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            return m_pController;
        }
    }
}

