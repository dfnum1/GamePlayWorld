/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	BattleCameraMode
作    者:	HappLI
描    述:	战斗模式相机
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public class BattleCameraMode : CameraMode
    {
        protected Vector3 m_pActorPosition;

        protected FloatLerp m_fCurrentDistance = new FloatLerp();
        protected FloatLerp m_fLockOffsetDistance = new FloatLerp();
        Vector3 m_vCurrentLookAt;

        float m_fCameraFollowMinDistance;
        float m_fCameraFollowMaxDistance;
        Vector4 m_fCameraFollowSpeed;

        bool m_bFalling = false;
        Vector3 m_FollowAddTween = Vector3.zero;
        Vector3 m_FollowLerpAddTween = Vector3.zero;
        //------------------------------------------------------
        public BattleCameraMode()
        {
            m_pController = null;
            Reset();
        }
        //------------------------------------------------------
        public override void Reset()
        {
            base.Reset();
            m_pActorPosition = Vector3.zero;
            m_fCameraFollowMinDistance =1.5f;
            m_fCameraFollowMaxDistance = 3f;
            m_fCameraFollowSpeed = Vector4.one * 1.5f;
            m_fCurrentDistance.Reset();
            m_fLockOffsetDistance.Reset();
            m_bFalling = false;

            m_FollowAddTween = Vector3.zero;
            m_FollowLerpAddTween = Vector3.zero;
        }
        //------------------------------------------------------
        public float GetMinDistance()
        {
            return m_fCameraFollowMinDistance;
        }
        //------------------------------------------------------
        public float GetMaxDistance()
        {
            return m_fCameraFollowMaxDistance;
        }
        //------------------------------------------------------
        public override float GetFollowDistance(bool bFinal = false)
        {
            if(bFinal)
                return m_fCurrentDistance.toValue + m_fLockOffsetDistance.toValue;
            return m_fCurrentDistance.value + m_fLockOffsetDistance.value;
        }
        //------------------------------------------------------
        public override void SetFollowLookAtPosition(Vector3 toPos, bool bForce = false)
        {
            if (toPos.y > m_vCurrentLookAt.y)
            {
                m_bFalling = false;
            }
            else m_bFalling = true;
            m_pActorPosition = toPos;
            ClampLockZoom(ECameraLockZoomFlag.LookAtX, ref m_pActorPosition.x);
            ClampLockZoom(ECameraLockZoomFlag.LookAtY, ref m_pActorPosition.y);
            ClampLockZoom(ECameraLockZoomFlag.LookAtZ, ref m_pActorPosition.z);
            if (bForce) m_vCurrentLookAt = m_pActorPosition;
        }
        //------------------------------------------------------
        public override Vector3 GetFollowLookAtPosition()
        {
             return m_pActorPosition;
        }
        //------------------------------------------------------   
        public void SetFollowSpeed(Vector4 fSpeed)
        {
            m_fCameraFollowSpeed = fSpeed;
        }
        //------------------------------------------------------   
        public Vector4 GetFollowSpeed()
        {
            return m_fCameraFollowSpeed;
        }
        //------------------------------------------------------   
        public void SetFollowLimit(float fMin, float fMax)
        {
            m_fCameraFollowMinDistance = Mathf.Min(fMax, fMin);
            m_fCameraFollowMaxDistance = Mathf.Max(fMax, fMin);
            m_fCurrentDistance.toValue = Mathf.Clamp(m_fCurrentDistance.toValue, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
        }
        //------------------------------------------------------   
        public void SetFollowDistance(float fCur, float fMin, float fMax)
        {
            m_fCameraFollowMinDistance = Mathf.Min(fMax, fMin);
            m_fCameraFollowMaxDistance = Mathf.Max(fMax, fMin);
            m_fCurrentDistance.toValue = Mathf.Clamp(fCur, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
            m_fCurrentDistance.Blance();
        }
        //------------------------------------------------------   
        public override void SetFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false)
        {
            if (bAmount)
                m_fCurrentDistance.toValue += fDistance;
            else m_fCurrentDistance.toValue = fDistance;
            if (bReMinMax)
            {
                m_fCameraFollowMinDistance = Mathf.Min(m_fCameraFollowMinDistance, m_fCurrentDistance.toValue);
                m_fCameraFollowMaxDistance = Mathf.Max(m_fCameraFollowMaxDistance, m_fCurrentDistance.toValue);
            }
            m_fCurrentDistance.toValue = Mathf.Clamp(m_fCurrentDistance.toValue, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
            m_fCurrentDistance.Blance();
        }
        //------------------------------------------------------   
        public override void AppendFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false, float fLerp = 0, bool pingpong = false, bool bClamp = true)
        {
            if (bAmount)
                m_fCurrentDistance.toValue += fDistance;
            else m_fCurrentDistance.toValue = fDistance;
            if (bReMinMax)
            {
                m_fCameraFollowMinDistance = Mathf.Min(m_fCameraFollowMinDistance, m_fCurrentDistance.toValue);
                m_fCameraFollowMaxDistance = Mathf.Max(m_fCameraFollowMaxDistance, m_fCurrentDistance.toValue);
            }
            if(bClamp)
                m_fCurrentDistance.toValue = Mathf.Clamp(m_fCurrentDistance.toValue, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
            if (fLerp > 0)
            {
                m_fCurrentDistance.SetPingpong(pingpong);
                m_fCurrentDistance.fFactor = fLerp;
            }
            else
            {
                m_fCurrentDistance.Blance();
            }
        }
        //------------------------------------------------------
        public override void SetLockOffsetDistance(float fDistance, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_fLockOffsetDistance.toValue += fDistance;
            else m_fLockOffsetDistance.toValue = fDistance;
            if (fLerp > 0)
            {
                m_fLockOffsetDistance.SetPingpong(pingpong);
                m_fLockOffsetDistance.fFactor = fLerp;
            }
            else
            {
                m_fLockOffsetDistance.Blance();
            }
        }
        //------------------------------------------------------
        public override bool IsLockOffsetDistanceLerping(float factor = 0.1f)
        {
            return !m_fLockOffsetDistance.IsArrived(factor);
        }
        //------------------------------------------------------
        public override void Start()
        {
            m_vCurrentLookAt = m_pActorPosition;
            base.Start();
            m_pController.ForceUpdate(0);
        }
        //------------------------------------------------------
        public override void End()
        {
            base.End();
        }
        //------------------------------------------------------
        public override void Blance()
        {
            base.Blance();
            m_fCurrentDistance.Blance();
            m_fLockOffsetDistance.Blance();
        }
        //------------------------------------------------------
        public override Vector3 GetCurrentLookAt(bool bFinal = false)
        {
            return m_vCurrentLookAt + GetFinalLookAtOffset(bFinal);
        }
        //------------------------------------------------------
        public override void Update(float fFrameTime)
        {
            base.Update(fFrameTime);
            if (fFrameTime <= 0 || !m_bTweenEffect)
            {
                m_vCurrentLookAt = m_pActorPosition;
            }
            else
            {
                m_FollowLerpAddTween = Vector3.Lerp(m_FollowLerpAddTween, m_FollowAddTween, fFrameTime);

                if (m_fCameraFollowSpeed.x <= 0 ) m_vCurrentLookAt.x = m_pActorPosition.x;
                else m_vCurrentLookAt.x = Mathf.Lerp(m_vCurrentLookAt.x, m_pActorPosition.x, (m_fCameraFollowSpeed.x+ m_FollowLerpAddTween.x) * fFrameTime);
                if (m_bFalling )
                {
                    if ( m_fCameraFollowSpeed.w <= 0) m_vCurrentLookAt.y = m_pActorPosition.y;
                    else m_vCurrentLookAt.y = Mathf.Lerp(m_vCurrentLookAt.y, m_pActorPosition.y, (m_fCameraFollowSpeed.w+ m_FollowLerpAddTween.y) * fFrameTime);
                }
                else
                {
                    if (m_fCameraFollowSpeed.y <= 0) m_vCurrentLookAt.y = m_pActorPosition.y;
                    else m_vCurrentLookAt.y = Mathf.Lerp(m_vCurrentLookAt.y, m_pActorPosition.y, (m_fCameraFollowSpeed.y+ m_FollowLerpAddTween.y) * fFrameTime);
                }

                if (m_fCameraFollowSpeed.z <=0) m_vCurrentLookAt.z = m_pActorPosition.z;
                else m_vCurrentLookAt.z = Mathf.Lerp(m_vCurrentLookAt.z, m_pActorPosition.z, (m_fCameraFollowSpeed.z+ m_FollowLerpAddTween.z) * fFrameTime);
            }

            m_fCurrentDistance.Update(fFrameTime);
            m_fLockOffsetDistance.Update(fFrameTime);
            m_fCurrentDistance.value = Mathf.Clamp(m_fCurrentDistance.value, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
            Vector3 dir = BaseUtil.EulersAngleToDirection(GetCurrentEulerAngle(false));

            m_vCurrentTrans = m_vCurrentLookAt + GetFinalLookAtOffset() - (m_fCurrentDistance.value+ m_fLockOffsetDistance.value) * dir;
        }
    }
}