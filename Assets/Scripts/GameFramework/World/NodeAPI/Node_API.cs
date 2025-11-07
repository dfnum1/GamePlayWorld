/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
using System.Collections.Generic;
using UnityEngine;
using Framework.Plugin.AT;

namespace Framework.Core
{
    public abstract partial class AWorldNode
    {
        protected virtual void InnerCreated() { }
        protected virtual void OnContextDataDirty() { }
        //------------------------------------------------------
        protected abstract void OnDestroy();
        protected virtual void OnReleaseGC() { }
        //-------------------------------------------------
        [ATMethod]
        public virtual FFloat GetTurnTime()
        {
#if USE_FIXEDMATH
            return new FFloat(0.1f);
#else
            return 0.1f;
#endif
        }
        //-------------------------------------------------
        internal virtual void SetRVOVelocity(FVector3 speed)
        {

        }
        //-------------------------------------------------
        internal virtual FVector3 GetRVOVelocity()
        {
            return FVector3.zero;
        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetSpeed(FVector3 speed)
        {

        }
        //-------------------------------------------------
        [ATMethod]
        public virtual void SetSpeedXZ(FVector3 vSpeed)
        {

        }
        //-------------------------------------------------
        [ATMethod]
        public virtual FVector3 GetSpeed()
        {
            return FVector3.zero;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FFloat GetRunSpeed()
        {
            return 5;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FFloat GetFastRunSpeed()
        {
            return 10;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FFloat GetPhysicRadius()
        {
            return m_Transform.GetScaleMag()*0.5f;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual FFloat GetTimeSpeed()
        {
#if USE_FIXEDMATH
            if (IsFreezed()) return FFloat.zero;
            return FFloat.one;
#else
            if (IsFreezed()) return 0.0f;
            return 1.0f;
#endif
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual bool IsRunAlongPath()
        {
            return false;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual bool IsLocalRunAlongPath()
        {
            return false;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual void AppendRunAlongPathByTimeStep(FVector3 point, FFloat fTime, int nInsertIndex = -1)
        {

        }
        //------------------------------------------------------
        public virtual FFloat RunAlongPathPoint(List<FVector3> vPoints, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            return 0;
        }
        //------------------------------------------------------
        [ATMethod("RunAlongPathPoint-FormTo")]
        public virtual FFloat RunAlongPathPoint(FVector3 srcPos, FVector3 toPos, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            return 0;
        }
        //------------------------------------------------------
        [ATMethod("RunAlongPathPoint-To")]
        public virtual FFloat RunAlongPathPoint(FVector3 toPos, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            return 0;
        }
        //------------------------------------------------------
        [ATMethod]
        public virtual void StopRunAlongPathPoint()
        {

        }
        //------------------------------------------------------
        [ATMethod]
        public virtual void PauseRunAlongPathPoint()
        {

        }
        //------------------------------------------------------
        [ATMethod]
        public virtual void ResumeRunAlongPathPoint()
        {

        }
        //------------------------------------------------------
        [ATMethod]
        public virtual bool IsRunAlongPathPlaying()
        {
            return IsRunAlongPath();
        }
        //------------------------------------------------------
        public virtual bool OnWorldTrigger(AWorldNode pTrigger, WorldTriggerParamter parameter)
        {
            return true;
        }
#if USE_AISYSTEM
        //------------------------------------------------------
        public virtual AI.AIAgent GetAI()
        {
            return null;
        }
#endif
        //-------------------------------------------------
        protected virtual void InnerUpdate(ExternEngine.FFloat fFrame) { }
        //-------------------------------------------------
        protected virtual void OnInnerSpawnObject(IUserData userData) { }
        //------------------------------------------------------
        public AFramework GetGameModule()
        {
            return m_pGame;
        }
        //------------------------------------------------------
        public virtual StateParam GetStateParam()
        {
            return null;
        }
        //------------------------------------------------------
        protected virtual void OnFreezed(bool freeze) 
        {
            if (m_pServerSync != null) m_pServerSync.OutSyncData(new SvrSyncData((int)EDefaultSyncType.Freeze, freeze ));
        }
        //------------------------------------------------------
        public virtual void OnLogicCall(IUserData takeData)
        {

        }
        //------------------------------------------------------
        public virtual byte GetClassify() { return 0xff; }
        public virtual byte GetCollisionFilter() { return 0xff; }
        [Plugin.AT.ATMethod]
        public abstract uint GetConfigID();
        [Plugin.AT.ATMethod]
        public abstract uint GetElementFlags();
        public abstract void SetElementFlags(uint flags);
        //------------------------------------------------------
        public virtual bool RayHit(Ray ray)
        {
            if (ray.GetPoint((ray.origin - GetPosition()).magnitude).y < GetPosition().y)
                return false;
            m_BoundBox.SetTransform(GetMatrix());
            return GetBounds().RayHit(ray);
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        //------------------------------------------------------
        public virtual void DrawDebug()
        {
            UnityEditor.Handles.Label(GetPosition(), GetActorType().ToString() + ":" + GetInstanceID().ToString() + " config:" + GetConfigID());
            ED.EditorUtil.DrawBoundingBox(m_BoundBox.GetCenter(), m_BoundBox.GetHalf(), GetMatrix(), Color.green, true);
        }
#endif
    }
}
