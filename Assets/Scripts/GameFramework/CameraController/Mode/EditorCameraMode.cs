/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	EditorCameraMode
作    者:	HappLI
描    述:	编辑模式相机
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public class EditorCameraMode : CameraMode
    {
        enum EDir
        {
            Foward = 1<<0,
            Back = 1<<1,
            Left = 1<<2,
            Right = 1<<3,
            Rotate = 1<<3,
            Shift = 1<<4,
        }
        int m_nPressDir = 0;
        float m_fMoveSpeed = 5;
        Vector3 m_vCurrentLookAt;
        protected FloatLerp m_fCurrentDistance = new FloatLerp();
        //------------------------------------------------------
        public EditorCameraMode()
        {
            m_pController = null;
            Reset();
        }
        //------------------------------------------------------
        public void SetMoveSpeed(float speed)
        {
            m_fMoveSpeed = speed;
        }
        //------------------------------------------------------
        public override void Start()
        {
            base.Start();
            m_vCurrentLookAt = Vector3.zero;
        }
        //------------------------------------------------------
        public override void Reset()
        {
            base.Reset();
            m_fCurrentDistance.Reset();
        }
        //------------------------------------------------------
        public override void End()
        {
            base.End();
        }
        //------------------------------------------------------
        public override Vector3 GetCurrentLookAt(bool bFinal = false)
        {
            return m_vCurrentLookAt + GetFinalLookAtOffset(bFinal);
        }
        //------------------------------------------------------
        public override float GetFollowDistance(bool bToFinal = false)
        {
            if (bToFinal) return m_fCurrentDistance.toValue;
            return m_fCurrentDistance.value;
        }

        //------------------------------------------------------   
        public void SetFollowDistance(float fCur, float fMin, float fMax)
        {
            m_fCurrentDistance.toValue = fCur;
            m_fCurrentDistance.Blance();
        }
        //------------------------------------------------------   
        public override void SetFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false)
        {
            if (bAmount)
                m_fCurrentDistance.toValue += fDistance;
            else m_fCurrentDistance.toValue = fDistance;
            m_fCurrentDistance.Blance();
        }
        //------------------------------------------------------   
        public override void AppendFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false, float fLerp = 0, bool pingpong = false, bool bClamp = true)
        {
            if (bAmount)
                m_fCurrentDistance.toValue += fDistance;
            else m_fCurrentDistance.toValue = fDistance;
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
        public override void Update(float fFrameTime)
        {
            base.Update(fFrameTime);
            if(Input.GetKeyDown(KeyCode.W)) m_nPressDir |= (int)EDir.Foward;
            else if (Input.GetKeyUp(KeyCode.W)) m_nPressDir &= ~(int)EDir.Foward;

            if (Input.GetKeyDown(KeyCode.S)) m_nPressDir |= (int)EDir.Back;
            else if (Input.GetKeyUp(KeyCode.S)) m_nPressDir &= ~(int)EDir.Back;

            if (Input.GetKeyDown(KeyCode.A)) m_nPressDir |= (int)EDir.Left;
            else if (Input.GetKeyUp(KeyCode.A)) m_nPressDir &= ~(int)EDir.Left;

            if (Input.GetKeyDown(KeyCode.D)) m_nPressDir |= (int)EDir.Right;
            else if (Input.GetKeyUp(KeyCode.D)) m_nPressDir &= ~(int)EDir.Right;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                m_nPressDir |= (int)EDir.Shift;
            else m_nPressDir &= ~(int)EDir.Shift;

            float fSpeed = m_fMoveSpeed;
            if ((m_nPressDir & (int)EDir.Shift) != 0) fSpeed *= 2;

            if ((m_nPressDir & (int)EDir.Foward) != 0)
            {
                m_vCurrentLookAt += m_pController.GetDir() * fSpeed * 10 * fFrameTime;
            }
            else if ((m_nPressDir & (int)EDir.Back) != 0)
            {
                m_vCurrentLookAt -= m_pController.GetDir() * fSpeed * 10 * fFrameTime;
            }

            if ((m_nPressDir & (int)EDir.Right) != 0)
            {
                m_vCurrentLookAt += m_pController.GetRight() * fSpeed * 10 * fFrameTime;
            }
            else if ((m_nPressDir & (int)EDir.Left) != 0)
            {
                m_vCurrentLookAt -= m_pController.GetRight() * fSpeed * 10* fFrameTime;
            }

            if (Input.GetMouseButton(1))
            {
                m_vCurrentEulerAngle.toValue.x -= Input.GetAxis("Mouse Y") * fSpeed;
                m_vCurrentEulerAngle.toValue.y += Input.GetAxis("Mouse X") * fSpeed;
                m_vCurrentEulerAngle.Blance();
            }
            if (Input.GetMouseButton(2))
            {
                m_vCurrentLookAt -= m_pController.GetUp() * Input.GetAxis("Mouse Y")  * fSpeed;
                m_vCurrentLookAt -= m_pController.GetRight() * Input.GetAxis("Mouse X") * fSpeed;
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                m_fCurrentDistance.toValue -= Input.GetAxis("Mouse ScrollWheel") * 100 * fSpeed;
                m_fCurrentDistance.Blance();
            }
            m_fCurrentDistance.Update(fFrameTime);
            Vector3 dir = BaseUtil.EulersAngleToDirection(m_vCurrentEulerAngle.value);
            m_vCurrentTrans = GetCurrentLookAt() - m_fCurrentDistance.value * dir;
        }
    }
}