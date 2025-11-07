using System.Runtime.InteropServices;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct WorldTransform
	{
#region Properties
        private FVector3 m_vLastPosition;
        private FVector3 m_vPosition;
		private FVector3 m_vEulerAngle;
		private FVector3 m_vScale;

        private FVector3 m_vDirection;
        private FVector3 m_vUp;
        private FVector3 m_vRight;

        private FFloat m_fScaleMag;

        private FMatrix4x4 m_Matrix;
#endregion

		public bool bDirtyPos { get; set; }
		public bool bDirtyEuler { get; set; }
		public bool bDirtyScale { get; set; }

        private bool m_bDirtyMatrix;

        public WorldTransform(FVector3 position)
        {
            m_vLastPosition = position;
            m_vPosition = position;
            m_vEulerAngle = FVector3.zero;
            m_vScale = FVector3.one;
            m_vDirection = FVector3.forward;
            m_vUp = FVector3.up;
            m_vRight = FVector3.right;
            m_Matrix = FMatrix4x4.identity;
            bDirtyPos = false;
            bDirtyEuler = false;
            bDirtyScale = false;
            m_bDirtyMatrix = true;
#if USE_FIXEDMATH
            m_fScaleMag = FFloat.one;
#else
            m_fScaleMag = 1.0f;
#endif
        }
		public void Clear()
        {
            bDirtyPos = false;
            bDirtyEuler = false;
            bDirtyScale = false;
            m_bDirtyMatrix = true;
            m_vLastPosition = FVector3.zero;
            m_vPosition = FVector3.zero;
            m_vEulerAngle = FVector3.zero;
            m_vScale = FVector3.one;
            m_vUp = FVector3.up;
            m_vDirection = FVector3.forward;
            m_vRight = FVector3.right;
            m_Matrix = FMatrix4x4.identity;
#if USE_FIXEDMATH
            m_fScaleMag = FFloat.one;
#else
            m_fScaleMag = 1.0f;
#endif
        }

        public void ClearDirty()
        {
            bDirtyPos = false;
            bDirtyEuler = false;
            bDirtyScale = false;
        }

        public void Translate(FVector3 vOffset)
        {
            if (vOffset.sqrMagnitude>0)
            {
                m_vLastPosition = m_vPosition;
                m_vPosition += vOffset;
                m_bDirtyMatrix = true;
                bDirtyPos = true;
            }
        }
		public void SetPosition(FVector3 vPosition)
        {
            if ((m_vPosition - vPosition).sqrMagnitude > 0)
            {
                m_vLastPosition = m_vPosition;
                m_vPosition = vPosition;
                m_bDirtyMatrix = true;
                bDirtyPos = true;
			}
		}

        public void SetLastPosition(FVector3 vPosition)
        {
            m_vLastPosition = m_vPosition;
        }

        public FVector3 GetPosition()
        {
            return m_vPosition;
        }
        public FVector3 GetLastPosition()
        {
            return m_vLastPosition;
        }

        public void SetEulerAngle(FVector3 vEulerAngle)
        {
            if ((m_vEulerAngle - vEulerAngle).sqrMagnitude > 0)
            {
                m_vEulerAngle = vEulerAngle;
                FQuaternion qRot = FQuaternion.Euler(m_vEulerAngle);
                m_vUp = qRot* FVector3.up;
                m_vDirection = qRot * FVector3.forward;
                m_vRight = qRot * FVector3.right;
                bDirtyEuler = true;
                m_bDirtyMatrix = true;
            }
        }

        public void SetDirection(FVector3 vDir)
        {
            if ((m_vDirection-vDir).sqrMagnitude>0 && vDir.sqrMagnitude>0)
            {
                if (vDir.sqrMagnitude > 1) vDir =vDir.normalized;
                m_vDirection = vDir;
                FQuaternion qRot  = FQuaternion.LookRotation(m_vDirection, m_vUp);
                m_vEulerAngle = qRot.eulerAngles;
                m_vRight = qRot * FVector3.right;
                bDirtyEuler = true;
                m_bDirtyMatrix = true;
            }
        }

        public void SetUp(FVector3 vUp)
        {
            if ((m_vUp - vUp).sqrMagnitude > 0 && vUp.sqrMagnitude > 0)
            {
                FQuaternion qRot = FQuaternion.LookRotation(m_vDirection, vUp);
                m_vEulerAngle = qRot.eulerAngles;
                m_vUp = vUp;
                m_vRight = qRot * FVector3.right;
                bDirtyEuler = true;
                m_bDirtyMatrix = true;
            }
        }

        public void TranslateEulerAngle(FVector3 vEulerAngle)
        {
            if(vEulerAngle.sqrMagnitude>0)
            {
                m_vEulerAngle += vEulerAngle;
                FQuaternion qRot = FQuaternion.Euler(m_vEulerAngle);
                m_vUp = FVector3.up;
                m_vDirection = qRot * FVector3.forward;
                m_vRight = qRot * FVector3.right;
                bDirtyEuler = true;
                m_bDirtyMatrix = true;
            }
        }

        public FVector3 GetEulerAngle()
        {
            return m_vEulerAngle;
        }

        public FVector3 GetDirection()
        {
            return m_vDirection;
        }

        public FVector3 GetUp()
        {
            return m_vUp;
        }

        public FVector3 GetRight()
        {
            return m_vRight;
        }
        public void SetScale(FVector3 vScale)
        {
            if ((m_vScale-vScale).sqrMagnitude > 0)
            {
                m_vScale = vScale;
                m_bDirtyMatrix = true;
                bDirtyScale = true;
                m_fScaleMag = m_vScale.magnitude;
            }
        }

        public void TranslateScale(FVector3 vScale)
        {
            if (vScale.sqrMagnitude>0)
            {
                m_vScale += vScale;
                m_fScaleMag = m_vScale.magnitude;
                m_bDirtyMatrix = true;
                bDirtyScale = true;
            }
        }

        public FVector3 GetScale()
        {
            return m_vScale;
        }
        public FFloat GetScaleMag()
        {
            return m_fScaleMag;
        }

        public void SetTransform(FVector3 vPos, FVector3 vEulerAngle, FVector3 vScale)
        {
            SetPosition(vPos);
            SetEulerAngle(vEulerAngle);
            SetScale(vScale);
        }

        public FMatrix4x4 GetMatrix()
        {
            if(m_bDirtyMatrix)
            {
                m_Matrix = FMatrix4x4.TRS(m_vPosition, FQuaternion.Euler(m_vEulerAngle), m_vScale);
                m_bDirtyMatrix = false;
            }
            return m_Matrix;
        }
    }
}