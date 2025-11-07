/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using UnityEngine;
#if USE_SERVER
using CharacterController = ExternEngine.CharacterController;
#endif
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
#endif
namespace Framework.Core
{
    public abstract partial class AWorldNode
    {
        protected ushort m_nFlags = (ushort)EWorldNodeFlag.Default;
        private int m_nFreezeCounter = 0;
        private FFloat m_fFreezeDuration = 0;
        //------------------------------------------------------
        public bool IsFlag(EWorldNodeFlag flag)
        {
            return (m_nFlags & (ushort)flag) != 0;
        }
        //------------------------------------------------------
        public void SetFlag(EWorldNodeFlag flag, bool bSet)
        {
            if (bSet)
            {
                if (!IsFlag(flag))
                {
                    m_nFlags |= (ushort)flag;
                    OnFlagDirty(flag, true);
                }
            }
            else
            {
                if (IsFlag(flag))
                {
                    m_nFlags &= (ushort)(~(ushort)flag);
                    OnFlagDirty(flag, false);
                }
            }
        }
        //------------------------------------------------------
        protected virtual void OnFlagDirty(EWorldNodeFlag flag, bool IsUsed)
        {
            if (m_pWorld == null) return;
            if (m_pServerSync != null) m_pServerSync.OutSyncData(new SvrSyncData((int)EDefaultSyncType.NodeFlag, (int)flag, IsUsed ? 1 : 0 ));
            switch (flag)
            {
                case EWorldNodeFlag.Active: if (IsUsed) m_pWorld.OnWorldNodeCallback(this, EWorldNodeStatus.Active); return;
                case EWorldNodeFlag.Visible:
                    {
                        m_Transform.bDirtyPos = true;
                        m_pWorld.OnWorldNodeCallback(this, IsUsed ? EWorldNodeStatus.Visible : EWorldNodeStatus.Hide);
                        return;
                    }
                case EWorldNodeFlag.Killed:
                    {
                        ResetFreeze();
                        if (IsUsed) m_pWorld.OnWorldNodeCallback(this, EWorldNodeStatus.Killed);
                        else m_pWorld.OnWorldNodeCallback(this, EWorldNodeStatus.Revive);
                        return;
                    }
                case EWorldNodeFlag.Destroy: if (IsUsed) m_pWorld.OnWorldNodeCallback(this, EWorldNodeStatus.Destroy); return;
            }
        }
        //-------------------------------------------------
        public virtual bool IsCanLogic()
        {
            return !IsFlag(EWorldNodeFlag.Killed) && IsFlag(EWorldNodeFlag.Logic) && !IsFlag(EWorldNodeFlag.Destroy) && IsFlag(EWorldNodeFlag.Active);
        }
        //------------------------------------------------------
        public void SetDelayDestroy(FFloat fTime)
        {
            m_fDestoryDelta = fTime;
        }
        //------------------------------------------------------
        public float GetDelayDestroy()
        {
            return m_fDestoryDelta;
        }
        //------------------------------------------------------
        public void SetDestroy()
        {
            m_fDestoryDelta = 0;
            SetFlag(EWorldNodeFlag.Destroy, true);
        }
        //------------------------------------------------------
        public bool IsDestroy()
        {
            return IsFlag(EWorldNodeFlag.Destroy) || m_nInstanceID==0;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            SetDestroy();
#if UNITY_EDITOR
            if(!Application.isPlaying || !AFramework.isStartup)
            {
                if (m_pObjectAble != null)
                    m_pObjectAble.Destroy();
                m_pObjectAble = null;
            }
#endif
        }
        //------------------------------------------------------
        public void EnableHudBar(bool bHudBar)
        {
            SetFlag(EWorldNodeFlag.HudBar, bHudBar);
        }
        //------------------------------------------------------
        public bool IsEnableHudBar()
        {
            return IsFlag(EWorldNodeFlag.HudBar);
        }
        //------------------------------------------------------
        public void SetColliderAble<T>(bool bAble, bool isTrigger = false) where T : Collider
        {
#if !USE_SERVER
            if (IsColliderAble() != bAble)
            {
                if (m_pObjectAble)
                {
                    if (bAble)
                    {
                        T collider = m_pObjectAble.AddBehaviour<T>();
                        if (collider) collider.isTrigger = isTrigger;
                    }
                    m_pObjectAble.EnableBehaviour<T>(bAble);
                }
            }
#endif
            SetFlag(EWorldNodeFlag.ColliderAble, bAble);
        }
        //------------------------------------------------------
        public bool IsColliderAble()
        {
            return IsFlag(EWorldNodeFlag.ColliderAble);
        }
        //------------------------------------------------------
        public void SetKilled(bool bVisible)
        {
            SetFlag(EWorldNodeFlag.Killed, bVisible);
        }
        //------------------------------------------------------
        public bool IsKilled()
        {
            return IsFlag(EWorldNodeFlag.Killed);
        }
        //------------------------------------------------------
        public bool IsDebug()
        {
            return IsFlag(EWorldNodeFlag.Debug);
        }
        //------------------------------------------------------
        public void SetDebug(bool bDebug)
        {
            SetFlag(EWorldNodeFlag.Debug, bDebug);
        }
        //------------------------------------------------------
        public void SetCollectAble(bool bVisible)
        {
            SetFlag(EWorldNodeFlag.CollectAble, bVisible);
        }
        //------------------------------------------------------
        public bool IsCollectAble()
        {
            return IsFlag(EWorldNodeFlag.CollectAble);
        }
        //------------------------------------------------------
        public void SetSpatial(bool bVisible)
        {
            SetFlag(EWorldNodeFlag.Spatial, bVisible);
        }
        //------------------------------------------------------
        public bool IsSpatial()
        {
            return IsFlag(EWorldNodeFlag.Spatial);
        }
        //------------------------------------------------------
        public void SetVisible(bool bVisible)
        {
            SetFlag(EWorldNodeFlag.Visible, bVisible);
        }
        //------------------------------------------------------
        public bool IsVisible()
        {
            return IsFlag(EWorldNodeFlag.Visible);
        }
        //------------------------------------------------------
        public void SetActived(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.Active, bToggle);
        }
        //------------------------------------------------------
        public bool IsActived()
        {
            return IsFlag(EWorldNodeFlag.Active);
        }
        //------------------------------------------------------
        public void EnableLogic(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.Logic, bToggle);
        }
        //------------------------------------------------------
        public bool IsLogicEnable()
        {
            return IsFlag(EWorldNodeFlag.Logic);
        }
        //------------------------------------------------------
        public void EnableAI(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.AI, bToggle);
        }
        //------------------------------------------------------
        public bool IsEnableAI()
        {
            return IsFlag(EWorldNodeFlag.AI);
        }
        //------------------------------------------------------
        public void EnableRVO(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.RVO, bToggle);
        }
        //------------------------------------------------------
        public bool IsEnableRVO()
        {
            return IsFlag(EWorldNodeFlag.RVO);
        }
        //------------------------------------------------------
        public bool IsLoadComplete()
        {
            return m_bLoadCompleted;
        }
        //------------------------------------------------------
        public bool IsObjected()
        {
            return m_pObjectAble != null;
        }
        //------------------------------------------------------
        private void UpdateFreeze(FFloat fFrameTime)
        {
            if (m_nFreezeCounter <= 0) return;
            if(m_fFreezeDuration>0)
            {
                m_fFreezeDuration -= fFrameTime;
                if (m_fFreezeDuration <= 0)
                    ResetFreeze();
            }
        }
        //------------------------------------------------------
        public void ResetFreeze()
        {
            bool isFreezeCall = IsFreezed();
            m_nFreezeCounter = 0;
            m_fFreezeDuration = 0;
            if (isFreezeCall)
                OnFreezed(IsFreezed());
        }
        //------------------------------------------------------
        public void Freezed(bool bToggle, FFloat fDuration)
        {
            bool isFreezeCall = IsFreezed();
            if (bToggle)
            {
                if (m_fFreezeDuration>0)
                {
                    m_nFreezeCounter = 1;
                    if (fDuration > 0) m_fFreezeDuration = fDuration;
                }
                else
                {
                    m_fFreezeDuration = fDuration;
                    if (fDuration > 0) m_nFreezeCounter = 1;
                    else m_nFreezeCounter++;
                }
            }
            else
            {
                m_nFreezeCounter--;
                if (m_nFreezeCounter <= 0)
                {
                    m_nFreezeCounter = 0;
                    m_fFreezeDuration = 0;
                }
            }
            if (isFreezeCall != IsFreezed())
                OnFreezed(IsFreezed());
        }
        //------------------------------------------------------
        public bool IsFreezed()
        {
            return m_nFreezeCounter > 0;
        }
        //------------------------------------------------------
        public virtual bool HasBuffEffect(int buffEffect)
        {
            return false;
        }
        //------------------------------------------------------
        public void EnableSvrSyncIn(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.SvrSyncIn, bToggle);
        }
        //------------------------------------------------------
        public bool IsSvrSyncIn()
        {
            return IsFlag(EWorldNodeFlag.SvrSyncIn);
        }
        //------------------------------------------------------
        public void EnableSvrSyncOut(bool bToggle)
        {
            SetFlag(EWorldNodeFlag.SvrSyncOut, bToggle);
        }
        //------------------------------------------------------
        public bool IsSvrSyncOut()
        {
            return IsFlag(EWorldNodeFlag.SvrSyncOut);
        }
    }
}
