/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if USE_SERVER
using Transform = ExternEngine.Transform;
#endif

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using Framework.Plugin.AT;
namespace Framework.Core
{
    public abstract partial class AWorldNode
    {
        protected WorldTransform m_Transform = new WorldTransform(FVector3.zero);

        protected WorldBoundBox m_BoundBox = new WorldBoundBox();
        //------------------------------------------------------
        [ATMethod]
        public virtual FVector3 GetPosition()
        {
            return m_Transform.GetPosition();
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FVector3 GetLastPosition()
        {
            return m_Transform.GetLastPosition();
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FVector3 GetEulerAngle()
        {
            return m_Transform.GetEulerAngle();
        }
        //------------------------------------------------------
        [ATMethod]
        public FVector3 GetScale()
        {
            return m_Transform.GetScale();
        }
        //-------------------------------------------------
        [ATMethod]
        public void SetScale(FVector3 scale)
        {
            m_Transform.SetScale(scale);
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetTransfrom(FVector3 vPosition, FVector3 vEulerAngle, FVector3 vScale)
        {
            m_Transform.SetTransform(vPosition, vEulerAngle, vScale);
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetDirection(FVector3 vDir)
        {
            if ((int)(vDir.sqrMagnitude * 100) <= 0) return;
            m_Transform.SetDirection(vDir);
        }
        //-------------------------------------------------
        [ATMethod]
        public FVector3 GetDirection()
        {
            return m_Transform.GetDirection();
        }
        //-------------------------------------------------
        [ATMethod]
        public FVector3 GetUp()
        {
            return m_Transform.GetUp();
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetUp(FVector3 up)
        {
            m_Transform.SetUp(up);
        }
        //-------------------------------------------------
        [ATMethod]
        public FVector3 GetRight()
        {
            return m_Transform.GetRight();
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetPosition(FVector3 vPos)
        {
            m_Transform.SetPosition(vPos);
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetFinalPosition(FVector3 vPos) { SetPosition(vPos); }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetEulerAngle(FVector3 vEulerAngle)
        {
            m_Transform.SetEulerAngle(vEulerAngle);
        }
        //-------------------------------------------------
        [ATMethod]
        public FMatrix4x4 GetMatrix()
        {
            return m_Transform.GetMatrix();
        }
        //-------------------------------------------------
        public WorldBoundBox GetBounds()
        {
            return m_BoundBox;
        }
        //-------------------------------------------------
        protected virtual void OnDirtyPosition() { }
        protected virtual void OnDirtyEulerAngle() { }
        protected virtual void OnDirtyScale() { }
        //-------------------------------------------------
        protected void UpdateTransform()
        {
            if (IsFlag(EWorldNodeFlag.Visible))
            {
                if (m_bDirtyTerrainY)
                {
#if USE_URP
                    if(URP.URPPostWorker.IsEnabledPass( URP.EPostPassType.PlaneShadow))
                    {
                        if (m_pObjectAble != null)
                        {
                            m_pObjectAble.SetFloat("_GroundHeight", m_vTerrain.y);
                        }
                    }
#endif
                    m_bDirtyTerrainY = false;
                }
                bool bDirty = false;
                if (m_Transform.bDirtyPos)
                {
                    if (m_pObjectAble != null)
                    {
                        m_pObjectAble.SetPosition(m_Transform.GetPosition());
                    }
                    m_Transform.bDirtyPos = false;
                    if (m_pServerSync != null) m_pServerSync.OutSyncData(new SvrSyncData((int)EDefaultSyncType.Position, m_Transform.GetPosition()));
                    OnDirtyPosition();
                    bDirty = true;
                }
                if (m_Transform.bDirtyEuler)
                {
                    if (m_pObjectAble!=null)
                    {
                        m_pObjectAble.SetEulerAngle(m_Transform.GetEulerAngle());
                    }
                    m_Transform.bDirtyEuler = false;
                    if (m_pServerSync != null) m_pServerSync.OutSyncData(new SvrSyncData((int)EDefaultSyncType.EulerAngle, m_Transform.GetEulerAngle()));
                    OnDirtyEulerAngle();
                    bDirty = true;
                }
                if (m_Transform.bDirtyScale)
                {
                    if (m_pObjectAble != null)
                    {
                        m_pObjectAble.SetScale(m_Transform.GetScale());
                    }
                    m_Transform.bDirtyScale = false;
                    if (m_pServerSync != null) m_pServerSync.OutSyncData(new SvrSyncData((int)EDefaultSyncType.Scale, m_Transform.GetScale()));
                    OnDirtyScale();
                    bDirty = true;
                }
                if (bDirty)
                {
                    if(Base.ConfigUtil.bProfilerDebug)
                    {
                        if (m_vTransformDebugs == null) m_vTransformDebugs = new List<TransformDebug>();
                        TransformDebug transformDebug = new TransformDebug();
                        transformDebug.frameCount = (int)m_pGame.GetRunTime();
                        transformDebug.position = m_Transform.GetPosition();
                        transformDebug.rotate = m_Transform.GetEulerAngle();
                        transformDebug.scale = m_Transform.GetScale();
                        m_vTransformDebugs.Add(transformDebug);
                    }
                    m_BoundBox.SetTransform(m_Transform.GetMatrix());
                }
            }
            else
            {
                if (m_pObjectAble!=null) m_pObjectAble.SetPosition(Base.ConstDef.INVAILD_POS);
            }
        }
        //------------------------------------------------------
        public virtual Transform FindBindSlot(string slot)
        {
            if (!IsObjected()) return null;
#if USE_SERVER
            return null;
#else
            return m_pObjectAble.GetTransorm();
#endif
        }
        //------------------------------------------------------
        public virtual FMatrix4x4 GetEventBindSlot(string strSlot, int bindSlot)
        {
            return m_Transform.GetMatrix();
        }
        //------------------------------------------------------
        public virtual bool IsIntersecition(AWorldNode pNode)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool IsIntersecition(FMatrix4x4 mtTrans, FVector3 vCenter, FVector3 vHalf)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool IsIntersecition(FMatrix4x4 mtTrans, FFloat radius)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool IsIntersecition(FMatrix4x4 mtTrans, FFloat innerRadius, FFloat outterRadius, FFloat sectorAngle)
        {
            return false;
        }
    }
}
