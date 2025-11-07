/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	HallCameraMode
作    者:	HappLI
描    述:	主城模式相机
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public class HallCameraMode : CameraMode
    {
        protected Vector3 m_pActorPosition;

        protected FloatLerp m_fCurrentDistance = new FloatLerp();
        protected FloatLerp m_fLockOffsetDistance = new FloatLerp();
        Vector3 m_vCurrentLookAt;

        Vector2 m_ClampXRotate = new Vector2(10,80);
        Vector2 m_ClampYRotate = new Vector2(-45, 45);

        float m_fCameraFollowMinDistance;
        float m_fCameraFollowMaxDistance;
        Vector4 m_fCameraFollowSpeed;
        //------------------------------------------------------
        public HallCameraMode()
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
            m_ClampXRotate = new Vector2(10, 80);
            m_ClampYRotate = new Vector2(-45, 45);

            m_fCurrentDistance.Reset();
            m_fLockOffsetDistance.Reset();
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
        public override float GetFollowDistance(bool bToFinal = false)
        {
            if(bToFinal) return m_fCurrentDistance.toValue + m_fLockOffsetDistance.toValue;
            return m_fCurrentDistance.value + m_fLockOffsetDistance.value;
        }
        //------------------------------------------------------
        public override void SetFollowLookAtPosition(Vector3 toPos, bool bForce = false)
        {
            m_pActorPosition = toPos;
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
                m_fCurrentDistance.toValue = Mathf.Clamp(m_fCurrentDistance.toValue, m_fCameraFollowMinDistance-1, m_fCameraFollowMaxDistance+1);
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
        //-----------------------------------------------------------------------------
        public void SetAxisLerp(float fLerp)
        {
            m_vCurrentEulerAngle.fFactor = fLerp;
        }
        //-----------------------------------------------------------------------------
        public void SetAxisXClamp(Vector2 clamp)
        {
            m_ClampXRotate = clamp;
        }
        //-----------------------------------------------------------------------------
        public void SetAxisYClamp(Vector2 clamp)
        {
            m_ClampYRotate = clamp;
        }
        //-----------------------------------------------------------------------------
        public Vector2 GetAxisXClamp() { return m_ClampXRotate; }
        public Vector2 GetAxisYClamp() { return m_ClampYRotate; }
        //-----------------------------------------------------------------------------
        public void SetAxisXRotation(float fAxisRotation, bool bAmount, float fLerp)
        {
            if (fLerp <= 0)
            {
                if (bAmount)
                    m_vCurrentEulerAngle.toValue.x += fAxisRotation;
                else
                    m_vCurrentEulerAngle.toValue.x = fAxisRotation;
                m_vCurrentEulerAngle.value.x = m_vCurrentEulerAngle.toValue.x;
            }
            else
            {
                m_vCurrentEulerAngle.fFactor = fLerp;
                if (bAmount)
                    m_vCurrentEulerAngle.toValue.x += fAxisRotation;
                else
                    m_vCurrentEulerAngle.toValue.x = fAxisRotation;
            }
            m_vCurrentEulerAngle.toValue.x = Mathf.Clamp(m_vCurrentEulerAngle.toValue.x, m_ClampXRotate.x-1, m_ClampXRotate.y+1);
        }
        //-----------------------------------------------------------------------------
        public void SetAxisYRotation(float fAxisRotation, bool bAmount, float fLerp)
        {
            if (fLerp <= 0)
            {
                if (bAmount)
                    m_vCurrentEulerAngle.toValue.y += fAxisRotation;
                else
                    m_vCurrentEulerAngle.toValue.y = fAxisRotation;
                m_vCurrentEulerAngle.value.y = m_vCurrentEulerAngle.toValue.y;
            }
            else
            {
                m_vCurrentEulerAngle.fFactor = fLerp;
                if (bAmount)
                    m_vCurrentEulerAngle.toValue.y += fAxisRotation;
                else
                    m_vCurrentEulerAngle.toValue.y = fAxisRotation;
            }
            m_vCurrentEulerAngle.toValue.y = Mathf.Clamp(m_vCurrentEulerAngle.toValue.y, m_ClampYRotate.x - 1, m_ClampYRotate.y + 1);
        }
        //------------------------------------------------------
        public Vector3 GetInputEulerAngle()
        {
            return m_vCurrentEulerAngle.value;
        }
        //------------------------------------------------------
        public float GetInputDistance()
        {
            return m_fCurrentDistance.value;
        }
        //-----------------------------------------------------
        public static Vector3 RotatePoint(Vector3 Pt, Vector3 vAnchor, Quaternion rot)
        {
            return rot * (Pt - vAnchor) + vAnchor;
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
                if (m_fCameraFollowSpeed.x <= 0 ) m_vCurrentLookAt.x = m_pActorPosition.x;
                else m_vCurrentLookAt.x = Mathf.Lerp(m_vCurrentLookAt.x, m_pActorPosition.x, m_fCameraFollowSpeed.x * fFrameTime);

                if (m_fCameraFollowSpeed.y <= 0) m_vCurrentLookAt.y = m_pActorPosition.y;
                else m_vCurrentLookAt.y = Mathf.Lerp(m_vCurrentLookAt.y, m_pActorPosition.y, m_fCameraFollowSpeed.y * fFrameTime);

                if (m_fCameraFollowSpeed.z <=0) m_vCurrentLookAt.z = m_pActorPosition.z;
                else m_vCurrentLookAt.z = Mathf.Lerp(m_vCurrentLookAt.z, m_pActorPosition.z, m_fCameraFollowSpeed.z * fFrameTime);
            }

            m_vCurrentEulerAngle.toValue.x = Mathf.Lerp(m_vCurrentEulerAngle.toValue.x, Mathf.Clamp(m_vCurrentEulerAngle.toValue.x, m_ClampXRotate.x, m_ClampXRotate.y), fFrameTime*100);
            m_vCurrentEulerAngle.toValue.y = Mathf.Lerp(m_vCurrentEulerAngle.toValue.y, Mathf.Clamp(m_vCurrentEulerAngle.toValue.y, m_ClampYRotate.x, m_ClampYRotate.y), fFrameTime * 100);

            m_fCurrentDistance.Update(fFrameTime); 
            m_fLockOffsetDistance.Update(fFrameTime);

            m_fCurrentDistance.toValue = Mathf.Lerp(m_fCurrentDistance.toValue, Mathf.Clamp(m_fCurrentDistance.toValue, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance), fFrameTime* 100);
            //m_fCurrentDistance.value = Mathf.Clamp(m_fCurrentDistance.value, m_fCameraFollowMinDistance, m_fCameraFollowMaxDistance);
            float distance = (m_fCurrentDistance.value + m_fLockOffsetDistance.value);
            Vector3 dir = BaseUtil.EulersAngleToDirection(m_vCurrentEulerAngle.value);
            m_vCurrentTrans = GetCurrentLookAt() - distance * dir;
        }
    }
}