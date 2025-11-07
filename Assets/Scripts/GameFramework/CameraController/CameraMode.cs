/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	CameraMode
作    者:	HappLI
描    述:	相机模式
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;
namespace Framework.Core
{
    public enum ECameraLockZoomFlag : byte
    {
        [PluginDisplay("视点X")] LookAtX = 0,
        [PluginDisplay("视点Y")] LookAtY = 1,
        [PluginDisplay("视点Z")] LookAtZ = 2,
        [PluginDisplay("位置X")] PosX = 3,
        [PluginDisplay("位置Y")] PosY = 4,
        [PluginDisplay("位置Z")] PosZ = 5,
        [PluginDisplay("旋转X")] RotX = 6,
        [PluginDisplay("旋转Y")] RotY = 7,
        [PluginDisplay("旋转Z")] RotZ = 8,
        [PluginDisplay("FOV")] Fov = 9,
        [DisableGUI]Count,
    }

    [Framework.Plugin.AT.ATExportMono("相机系统/基础模式")]
    public abstract class CameraMode : IUserData
    {
        protected CameraController m_pController;

        protected FloatLerp m_fLockFovOffset = new FloatLerp();
        protected Vector3Lerp m_vLockCameraOffset = new Vector3Lerp();
        protected Vector3Lerp m_vLockCameraLookAtOffset = new Vector3Lerp();
        protected Vector3Lerp m_vLockEulerAngleOffset = new Vector3Lerp();
        protected Vector3Lerp m_vLockUpOffset = new Vector3Lerp();

        protected Vector3Lerp m_vCurrentLookatOffset = new Vector3Lerp();
        protected Vector3Lerp m_vCurrentTransOffset = new Vector3Lerp();
        protected Vector3 m_vCurrentTrans = Vector3.zero;
        protected Vector3Lerp m_vCurrentEulerAngle = new Vector3Lerp();
        protected Vector3Lerp m_vCurrentUp = new Vector3Lerp();
        protected FloatLerp m_fCurrentFov = new FloatLerp();

        protected Vector3 m_ExternOffset = Vector3.zero;
        protected Vector3 m_ExternEulerAngleOffset = Vector3.zero;

        private Vector4 m_LookFocusScattingAxis = Vector4.zero;
        private float m_LookFocusScattingFrequency = 0;
        protected Vector3Lerp m_LookFocusScatter = new Vector3Lerp();
        protected List<Vector3> m_vLookScatterRivetPoint = null;

        protected bool m_bTweenEffect = true;

        private ushort m_LockZoomFlags = 0;
        protected Vector2[] m_arrLockZooms = new Vector2[(int)ECameraLockZoomFlag.Count];

        //------------------------------------------------------
        public CameraMode()
        {
            m_pController = null;
            Reset();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void Reset()
        {
            m_vCurrentTrans = Vector3.zero;
            m_vCurrentUp.Reset();
            m_fCurrentFov.Reset();
            m_fLockFovOffset.Reset();
            m_bTweenEffect = true;
            m_vCurrentTransOffset.Reset();
            m_vCurrentLookatOffset.Reset();

            m_vLockCameraOffset.Reset();
            m_vLockCameraLookAtOffset.Reset();
            m_vLockEulerAngleOffset.Reset();
            m_vLockUpOffset.Reset();

            m_fCurrentFov.toValue = 45;
            m_fCurrentFov.Blance();

            m_ExternOffset = Vector3.zero;
            m_ExternEulerAngleOffset = Vector3.zero;

            m_LookFocusScattingAxis = Vector4.zero;
            m_LookFocusScatter.Reset();

            ClearLockZooms();

            if (m_vLookScatterRivetPoint != null) m_vLookScatterRivetPoint.Clear();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ResetLockOffsets()
        {
            m_vLockCameraOffset.Reset();
            m_vLockCameraLookAtOffset.Reset();
            m_vLockEulerAngleOffset.Reset();
            m_vLockUpOffset.Reset();
            m_LookFocusScatter.Reset();
            if (m_vLookScatterRivetPoint != null) m_vLookScatterRivetPoint.Clear();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ClearLockZooms()
        {
            ClearAllLockZoomFlags();
            for (int i =0; i < m_arrLockZooms.Length; ++i)
            {
                m_arrLockZooms[i] = Vector2.zero;
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockZoom(ECameraLockZoomFlag flag, float min, float max)
        {
            int index = (int)flag;
            if (index<0 || index >= m_arrLockZooms.Length) return;
            m_arrLockZooms[index].x = Mathf.Min(min, max);
            m_arrLockZooms[index].y = Mathf.Max(min, max);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockZoom(ECameraLockZoomFlag flag, Vector2 limit)
        {
            int index = (int)flag;
            if (index < 0 || index >= m_arrLockZooms.Length) return;
            m_arrLockZooms[index].x = Mathf.Min(limit.x, limit.y);
            m_arrLockZooms[index].y = Mathf.Max(limit.x, limit.y);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void EnableLockZoom(ECameraLockZoomFlag flag, bool bEnable)
        {
            if (flag >= ECameraLockZoomFlag.Count) return;
            if (bEnable) m_LockZoomFlags |= (ushort)(1<<(int)flag);
            else m_LockZoomFlags &= (ushort)(~(1 << (int)flag));
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public bool IsEnableLockZoom(ECameraLockZoomFlag flag)
        {
            if (flag >= ECameraLockZoomFlag.Count) return false;
            return (m_LockZoomFlags & (ushort)(1 << (int)flag)) != 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ClearAllLockZoomFlags()
        {
            m_LockZoomFlags = 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ClampLockZoom(ECameraLockZoomFlag flag, ref float curValue)
        {
            curValue = ClampLockZoom(flag, curValue);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public float ClampLockZoom(ECameraLockZoomFlag flag, float curValue)
        {
            if (!IsEnableLockZoom(flag)) return curValue;
            int index = (int)flag;
            if (index < 0 || index >= m_arrLockZooms.Length) return curValue;
            return Mathf.Clamp(curValue, m_arrLockZooms[index].x, m_arrLockZooms[index].y);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetExternOffset(Vector3 offset)
        {
            m_ExternOffset = offset;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetExternEulerAngleOffset(Vector3 offset)
        {
            m_ExternEulerAngleOffset = offset;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void SetLookFocusScatter(Vector3 scatter, float fIntensity, float fFrequency)
        {
            m_LookFocusScattingAxis.x = scatter.x;
            m_LookFocusScattingAxis.y = scatter.y;
            m_LookFocusScattingAxis.z = scatter.z;
            m_LookFocusScattingFrequency = fFrequency;
            //    m_LookFocusScatter.toValue = scatter;
            m_LookFocusScatter.fFactor = fIntensity;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void AddScatterRivetPoint(Vector3 point, bool bClear = false)
        {
            if (m_vLookScatterRivetPoint == null) m_vLookScatterRivetPoint = new List<Vector3>(3);
            if (bClear) m_vLookScatterRivetPoint.Clear();
            m_vLookScatterRivetPoint.Add(point);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ClearScatterRivetPoint()
        {
            if (m_vLookScatterRivetPoint != null) m_vLookScatterRivetPoint.Clear();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void ReStartFocusScatter()
        {
            Vector3 toValue = m_LookFocusScatter.toValue;
            toValue.x = UnityEngine.Random.Range(-m_LookFocusScattingAxis.x, m_LookFocusScattingAxis.x);
            toValue.y = UnityEngine.Random.Range(-m_LookFocusScattingAxis.y, m_LookFocusScattingAxis.y);
            toValue.z = UnityEngine.Random.Range(-m_LookFocusScattingAxis.z, m_LookFocusScattingAxis.z);
            m_LookFocusScatter.toValue = toValue;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void EnableTween(bool bTween)
        {
            m_bTweenEffect = bTween;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetController(CameraController pController)
        {
            m_pController = pController;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetCurrentTrans(Vector3 vCurrentTrans)
        {
            m_vCurrentTrans = vCurrentTrans;
        }
        //------------------------------------------------------
        public void SetCurrentTransOffset(Vector3 vCurrentTrans, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vCurrentTransOffset.toValue += vCurrentTrans;
            else
                m_vCurrentTransOffset.toValue = vCurrentTrans;
            if (fLerp > 0)
            {
                m_vCurrentTransOffset.SetPingpong(pingpong);
                m_vCurrentTransOffset.fFactor = fLerp;
            }
            else
            {
                m_vCurrentTransOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetCurrentTransOffset(bool bFinal = false)
        {
            if (bFinal) return m_vCurrentTransOffset.toValue;
            return m_vCurrentTransOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetCurrentEulerAngle(Vector3 vEulerAngle, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            vEulerAngle = BaseUtil.ClampAngle(vEulerAngle);
            if (bAmount)
                m_vCurrentEulerAngle.toValue += vEulerAngle;
            else
                m_vCurrentEulerAngle.toValue = vEulerAngle;
            if (fLerp > 0)
            {
                m_vCurrentEulerAngle.SetPingpong(pingpong);
                m_vCurrentEulerAngle.fFactor = fLerp;
            }
            else
            {
                m_vCurrentEulerAngle.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetCurrentUp(Vector3 vUp, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vCurrentUp.toValue += vUp;
            else
                m_vCurrentUp.toValue = vUp;
            if (fLerp > 0)
            {
                m_vCurrentUp.SetPingpong(pingpong);
                m_vCurrentUp.fFactor = fLerp;
            }
            else
            {
                m_vCurrentUp.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetCurrentFov(float fFov, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_fCurrentFov.toValue += fFov;
            else
                m_fCurrentFov.toValue = fFov;
            if (fLerp > 0)
            {
                m_vCurrentUp.SetPingpong(pingpong);
                m_fCurrentFov.fFactor = fLerp;
            }
            else
            {
                m_fCurrentFov.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public float GetCurrentFov(bool bFinal = false)
        {
            if (bFinal)
                return ClampLockZoom( ECameraLockZoomFlag.Fov, m_fCurrentFov.toValue + m_fLockFovOffset.toValue);
            return ClampLockZoom(ECameraLockZoomFlag.Fov, m_fCurrentFov.value + m_fLockFovOffset.value);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetCurrentTrans(bool bFinal = false)
        {
            Vector3 trans;
            if (bFinal)
                trans = m_vCurrentTrans + m_vLockCameraOffset.toValue + m_vCurrentTransOffset.toValue + m_ExternOffset;
            else trans = m_vCurrentTrans + m_vLockCameraOffset.value + m_vCurrentTransOffset.value + m_ExternOffset;

            ClampLockZoom(ECameraLockZoomFlag.PosX, ref trans.x);
            ClampLockZoom(ECameraLockZoomFlag.PosY, ref trans.y);
            ClampLockZoom(ECameraLockZoomFlag.PosZ, ref trans.z);
            return trans;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual Vector3 GetCurrentLookAt(bool bFinal = false)
        {
            if (bFinal)
                return BaseUtil.RayHitPos(m_vCurrentTrans, BaseUtil.EulersAngleToDirection(m_vCurrentEulerAngle.toValue + m_vLockEulerAngleOffset.toValue)) + m_vLockCameraLookAtOffset.toValue + m_vCurrentLookatOffset.toValue;
            return BaseUtil.RayHitPos(m_vCurrentTrans, BaseUtil.EulersAngleToDirection(m_vCurrentEulerAngle.value + m_vLockEulerAngleOffset.value)) + m_vLockCameraLookAtOffset.value + m_vCurrentLookatOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetCurrentLookAtOffset(bool bFinal = false)
        {
            if (bFinal) return m_vCurrentLookatOffset.toValue;
            return m_vCurrentLookatOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetFinalLookAtOffset(bool bFinal = false)
        {
            return GetCurrentLookAtOffset(bFinal) + GetLockCameraLookAtOffset(bFinal);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetCurrentLookAtOffset(Vector3 offset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vCurrentLookatOffset.toValue += offset;
            else
                m_vCurrentLookatOffset.toValue = offset;
            if (fLerp > 0)
            {
                m_vCurrentLookatOffset.SetPingpong(pingpong);
                m_vCurrentLookatOffset.fFactor = fLerp;
            }
            else
            {
                m_vCurrentLookatOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual Vector3 GetCurrentEulerAngle(bool bExternAdd = true, bool bFinal = false)
        {
            if (bExternAdd)
            {
                if (bFinal) return m_vCurrentEulerAngle.toValue + m_vLockEulerAngleOffset.toValue + m_LookFocusScatter.toValue + m_ExternEulerAngleOffset;
                return m_vCurrentEulerAngle.value + m_vLockEulerAngleOffset.value + m_LookFocusScatter.value + m_ExternEulerAngleOffset;
            }
            if (bFinal)
                return m_vCurrentEulerAngle.toValue + m_vLockEulerAngleOffset.toValue + m_LookFocusScatter.toValue;
            return m_vCurrentEulerAngle.value + m_vLockEulerAngleOffset.value + m_LookFocusScatter.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetCurrentUp(bool bFinal = false)
        {
            if (bFinal) return m_vCurrentUp.toValue + m_vLockUpOffset.toValue;
            return m_vCurrentUp.value + m_vLockUpOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockCameraOffset(Vector3 vOffset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vLockCameraOffset.toValue += vOffset;
            else
                m_vLockCameraOffset.toValue = vOffset;
            if (fLerp > 0)
            {
                m_vLockCameraOffset.SetPingpong(pingpong);
                m_vLockCameraOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockCameraOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetLockCameraOffset(bool bFinal = false)
        {
            return bFinal ? m_vLockCameraOffset.toValue : m_vLockCameraOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3Lerp GetLockCameraLookAtOffset() { return m_vLockCameraLookAtOffset; }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockCameraLookAtOffset(Vector3 vOffset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if(bAmount)
                m_vLockCameraLookAtOffset.toValue += vOffset;
            else
                m_vLockCameraLookAtOffset.toValue = vOffset;
            if (fLerp > 0)
            {
                m_vLockCameraLookAtOffset.SetPingpong(pingpong);
                m_vLockCameraLookAtOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockCameraLookAtOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetLockCameraLookAtOffset(bool bFinal = false)
        {
            if(bFinal) return m_vLockCameraLookAtOffset.toValue;
            return m_vLockCameraLookAtOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockEulerAngleOffset(Vector3 vOffset, bool bAmount = false, float fLerp =0, bool pingpong = false)
        {
            if(bAmount)
                m_vLockEulerAngleOffset.toValue += vOffset;
            else
                m_vLockEulerAngleOffset.toValue = vOffset;
            if (fLerp > 0)
            {
                m_vLockEulerAngleOffset.SetPingpong(pingpong);
                m_vLockEulerAngleOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockEulerAngleOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockPitchOffset(float fOffset, bool bAmount = false, float fLerp =0, bool pingpong = false)
        {
            if(bAmount)
                m_vLockEulerAngleOffset.toValue.y += fOffset;
            else
                m_vLockEulerAngleOffset.toValue.y = fOffset;
            if (fLerp > 0)
            {
                m_vLockEulerAngleOffset.SetPingpong(pingpong);
                m_vLockEulerAngleOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockEulerAngleOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockYawOffset(float fOffset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vLockEulerAngleOffset.toValue.x -= fOffset;
            else
                m_vLockEulerAngleOffset.toValue.x = -fOffset;
            if (fLerp > 0)
            {
                m_vLockEulerAngleOffset.SetPingpong(pingpong);
                m_vLockEulerAngleOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockEulerAngleOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockRollOffset(float fOffset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if (bAmount)
                m_vLockEulerAngleOffset.toValue.z -= fOffset;
            else
                m_vLockEulerAngleOffset.toValue.z = -fOffset;
            if (fLerp > 0)
            {
                m_vLockEulerAngleOffset.SetPingpong(pingpong);
                m_vLockEulerAngleOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockEulerAngleOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3Lerp GetLockEulerAngleOffsetLerp() { return m_vLockEulerAngleOffset; }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetLockEulerAngleOffset()
        {
            return m_vLockEulerAngleOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockUpOffset(Vector3 vUp, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if(bAmount)
                m_vLockUpOffset.toValue += vUp;
            else
                m_vLockUpOffset.toValue = vUp;
            if (fLerp > 0)
            {
                m_vLockUpOffset.SetPingpong(pingpong);
                m_vLockUpOffset.fFactor = fLerp;
            }
            else
            {
                m_vLockUpOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public Vector3 GetLockUpOffset()
        {
            return m_vLockUpOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public void SetLockFovOffset(float fOffset, bool bAmount = false, float fLerp = 0, bool pingpong = false)
        {
            if(bAmount)
                m_fLockFovOffset.toValue += fOffset;
            else
                m_fLockFovOffset.toValue = fOffset;
            if (fLerp > 0)
            {
                m_fLockFovOffset.SetPingpong(pingpong);
                m_fLockFovOffset.fFactor = fLerp;
            }
            else
            {
                m_fLockFovOffset.Blance();
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public FloatLerp GetLockFovOffsetLerp() { return m_fLockFovOffset; }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public float GetLockFovOffset()
        {
            return m_fLockFovOffset.value;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void Start()
        {
            Blance();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void End()
        {
            Reset();
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void Blance()
        {
            m_vCurrentUp.Blance();
            m_fCurrentFov.Blance();
            m_fLockFovOffset.Blance();
            m_vCurrentTransOffset.Blance();
            m_vCurrentLookatOffset.Blance();

            m_vLockCameraOffset.Blance();
            m_vLockCameraLookAtOffset.Blance();
            m_vLockEulerAngleOffset.Blance();
            m_vLockUpOffset.Blance();
        }
        //------------------------------------------------------
        public virtual void Update(float fFrame)
        {
   //         Core.CommonUtility.AjustUpdatePostitionFrame(ref fFrame);

            m_fLockFovOffset.Update(fFrame);
            m_vLockCameraOffset.Update(fFrame);
            m_vLockCameraLookAtOffset.Update(fFrame);
            m_vLockEulerAngleOffset.Update(fFrame);
            m_vLockUpOffset.Update(fFrame);

            m_vCurrentLookatOffset.Update(fFrame);
            m_vCurrentTransOffset.Update(fFrame);
            m_vCurrentEulerAngle.Update(fFrame);
            m_vCurrentUp.Update(fFrame);
            m_fCurrentFov.Update(fFrame);

            if(m_LookFocusScattingAxis.w >= 0)
            {
                m_LookFocusScattingAxis.w -= fFrame;
                if (m_LookFocusScattingAxis.w <= 0)
                    ReStartFocusScatter();
            }
            else if( BaseUtil.Equal(m_LookFocusScatter.value, m_LookFocusScatter.toValue, Mathf.Max(m_LookFocusScatter.fFactor, 0.01f) ) )
            {
                m_LookFocusScattingAxis.w = m_LookFocusScattingFrequency;
            }
            m_LookFocusScatter.Update(fFrame);
            if( (m_LookFocusScatter.toValue-m_LookFocusScatter.value).sqrMagnitude>0 )
            {
                if(m_vLookScatterRivetPoint!=null)
                {
                    for(int i = 0; i < m_vLookScatterRivetPoint.Count; ++i)
                    {
                        if(!m_pController.IsInView(m_vLookScatterRivetPoint[i], -0.1f))
                        {
                            m_LookFocusScatter.toValue = Vector3.zero;
                            break;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual Vector3 GetFollowLookAtPosition() { return Vector3.zero; }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void SetFollowLookAtPosition(Vector3 pos, bool bForce=false) { }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void SetFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false) { }
        [Framework.Plugin.AT.ATMethod]
        public virtual float GetFollowDistance(bool bFinal = false) { return 0; }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void AppendFollowDistance(float fDistance, bool bReMinMax, bool bAmount = false, float fLerp=0, bool pingpong = false, bool bClamp = true) { }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual void SetLockOffsetDistance(float fDistance, bool bAmount = false, float fLerp = 0, bool pingpong = false) { }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod]
        public virtual bool IsLockOffsetDistanceLerping(float factor =0.1f ) { return false; }
        //------------------------------------------------------
        public void Destroy()
        {
            Reset();
        }
    }
}