#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	RunAlongPathAgent
作    者:	HappLI
描    述:	路径播放表现类
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using System.Collections.Generic;
#endif

namespace Framework.Core
{
    public class RunAlongPathAgent : AActorAgent
    {
        protected bool              m_bRunningAlongPathPoint = false;
        protected bool              m_bRunningAlongPathPlay = false;
        protected bool              m_bLocalRunAlongPath = false;
        protected bool              m_bRunAlongPathPointEnsureSucceed = false;
        protected bool              m_bRunAlongPathPointUpdateDirection = false;
        protected EActionStateType  m_eActionStateType = EActionStateType.None;
        protected uint               m_nActionStateTag = 0;
        protected FFloat            m_fRunAlongPathPointSpeed = 0.0f;
        protected FFloat            m_fRunAlongPathPointTime = 0.0f;
        protected FFloat            m_fRunAlongPathPointDelta = 0.0f;
        protected FVector3          m_LocalRunAlongPoisiotn = FVector3.zero;
        protected Vector3Track      m_pPathPointTrack = null;
        //--------------------------------------------------------
        protected override void OnInit()
        {
            m_pPathPointTrack = new Vector3Track();
        }
        //--------------------------------------------------------
        protected override void OnClear()
        {
            if (m_pPathPointTrack != null) m_pPathPointTrack.Clear();
            m_bRunningAlongPathPoint = false;
            m_bRunningAlongPathPlay = false;
            m_bLocalRunAlongPath = false;
            m_bRunAlongPathPointEnsureSucceed = false;
            m_bRunAlongPathPointUpdateDirection = false;
            m_fRunAlongPathPointSpeed = 0;
            m_fRunAlongPathPointTime = 0;
            m_fRunAlongPathPointDelta = 0;
            m_LocalRunAlongPoisiotn = FVector3.zero;
            m_eActionStateType = EActionStateType.None;
            m_nActionStateTag = 0;
        }
        //------------------------------------------------------
        public void AppendRunAlongPathByTimeStep(FVector3 point, FFloat fTime, int nInsertIndex = -1)
        {
            if (m_bRunningAlongPathPoint)
            {
                m_fRunAlongPathPointTime += fTime;
                if (nInsertIndex >= 0)
                {
                    m_pPathPointTrack.InsertKeyPoint(nInsertIndex, fTime, point);
                }
                else
                    m_pPathPointTrack.AddKeyPoint(m_fRunAlongPathPointTime, point);
            }
        }
        //------------------------------------------------------
        public FFloat RunAlongPathPoint(FVector3 srcPos, FVector3 toPos, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            FFloat fPathLength = (toPos - srcPos).magnitude;
            if (fPathLength > 0)
            {
                m_bRunningAlongPathPoint = true;
                m_bRunningAlongPathPlay = true;
                m_eActionStateType = EActionStateType.Run;
                m_nActionStateTag = 0;
                m_bLocalRunAlongPath = bLocalRun;
                m_bRunAlongPathPointEnsureSucceed = bEnsureSucceed;
                m_bRunAlongPathPointUpdateDirection = bUpdateDirection;
                m_fRunAlongPathPointSpeed = fSpeed;
                m_fRunAlongPathPointDelta = 0.0f;
                m_LocalRunAlongPoisiotn = FVector3.zero;
                m_pPathPointTrack.Clear();
                m_pPathPointTrack.AddKeyPoint(0.0f, srcPos);

                FFloat fTime = fPathLength / m_fRunAlongPathPointSpeed;
                m_pPathPointTrack.AddKeyPoint(fTime, toPos);
                m_fRunAlongPathPointTime = fTime;
                OnRunAlongPathPoint(srcPos, toPos);
            }
            else
            {
                StopRunAlongPathPoint();
                m_fRunAlongPathPointTime = -1.0f;
            }

            return m_fRunAlongPathPointTime;
        }
        //------------------------------------------------------
        public FFloat RunAlongPathPoint(System.Collections.Generic.List<FVector3> vPathPoint, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            if (vPathPoint.Count > 1)
            {
                m_bRunningAlongPathPoint = true;
                m_bRunningAlongPathPlay = true;
                m_eActionStateType = EActionStateType.Run;
                m_nActionStateTag = 0;
                m_bLocalRunAlongPath = bLocalRun;
                m_bRunAlongPathPointEnsureSucceed = bEnsureSucceed;
                m_bRunAlongPathPointUpdateDirection = bUpdateDirection;
                m_fRunAlongPathPointSpeed = fSpeed;
                m_fRunAlongPathPointDelta = 0.0f;
                m_LocalRunAlongPoisiotn = FVector3.zero;
                m_pPathPointTrack.Clear();
                m_pPathPointTrack.AddKeyPoint(0.0f, vPathPoint[0]);

                FFloat fPathLength = 0.0f;
                for (int i = 1; i < vPathPoint.Count; i++)
                {
                    FVector3 vStart = vPathPoint[i - 1];
                    FVector3 vEnd = vPathPoint[i];
                    FFloat fPathSegmentLength = (vEnd - vStart).magnitude;
                    if (fPathSegmentLength <= 0) continue;
                    fPathLength += fPathSegmentLength;
                    FFloat fTime = fPathLength / m_fRunAlongPathPointSpeed;
                    m_pPathPointTrack.AddKeyPoint(fTime, vEnd);
                }

                m_fRunAlongPathPointTime = fPathLength / m_fRunAlongPathPointSpeed;
                OnRunAlongPathPoint(vPathPoint);
            }
            else
            {
                StopRunAlongPathPoint();
                m_fRunAlongPathPointTime = -1.0f;
            }

            return m_fRunAlongPathPointTime;
        }
        //------------------------------------------------------
        public FFloat RunAlongPathPoint(System.Collections.Generic.List<RunPoint> vPathPoint, FFloat fSpeed, bool bEnsureSucceed = false, bool bUpdateDirection = true, bool bLocalRun = false)
        {
            if (vPathPoint.Count > 1)
            {
                m_bRunningAlongPathPoint = true;
                m_bRunningAlongPathPlay = true;
                m_bLocalRunAlongPath = bLocalRun;
                m_eActionStateType = EActionStateType.Run;
                m_nActionStateTag = 0;
                m_bRunAlongPathPointEnsureSucceed = bEnsureSucceed;
                m_bRunAlongPathPointUpdateDirection = bUpdateDirection;
                m_fRunAlongPathPointSpeed = fSpeed;
                m_fRunAlongPathPointDelta = 0.0f;
                m_LocalRunAlongPoisiotn = FVector3.zero;
                m_pPathPointTrack.Clear();
                m_pPathPointTrack.AddKeyPoint(0.0f, vPathPoint[0]);

                FFloat fPathLength = 0.0f;
                for (int i = 1; i < vPathPoint.Count; i++)
                {
                    RunPoint vStart = vPathPoint[i - 1];
                    RunPoint vEnd = vPathPoint[i];
                    FFloat fPathSegmentLength = (vEnd.position - vStart.position).magnitude;
                    if (fPathSegmentLength <= 0) continue;
                    fPathLength += fPathSegmentLength;
                    FFloat fTime = fPathLength / (m_fRunAlongPathPointSpeed * System.Math.Max(0.1f, vStart.speedScale));
                    m_pPathPointTrack.AddKeyPoint(fTime, vEnd);
                }

                m_fRunAlongPathPointTime = fPathLength / m_fRunAlongPathPointSpeed;
                OnRunAlongPathPoint(vPathPoint);
            }
            else
            {
                StopRunAlongPathPoint();
                m_fRunAlongPathPointTime = -1.0f;
            }

            return m_fRunAlongPathPointTime;
        }
        //------------------------------------------------------
        public void StopRunAlongPathPoint()
        {
            if (m_bRunningAlongPathPoint)
            {
                OnStopAlongPathPoint();

                m_bRunningAlongPathPlay = false;
                m_bRunningAlongPathPoint = false;
                m_bLocalRunAlongPath = false;
                m_bRunAlongPathPointEnsureSucceed = false;
                m_bRunAlongPathPointUpdateDirection = false;
                m_fRunAlongPathPointSpeed = 0.0f;
                m_fRunAlongPathPointDelta = 0.0f;
                m_fRunAlongPathPointTime = 0.0f;
                m_LocalRunAlongPoisiotn = FVector3.zero;
                if(m_eActionStateType != EActionStateType.None)
                    m_pActor.StopActionState(m_eActionStateType, m_nActionStateTag);
                m_eActionStateType = EActionStateType.Run;
                m_nActionStateTag = 0;
                m_pPathPointTrack.Clear();
                m_pActor.SetSpeedXZ(FVector3.zero);
                m_pActor.SetRVOTest(50);
            }
        }
        //------------------------------------------------------
        public bool IsLocalRunAlongPath()
        {
            return m_bLocalRunAlongPath;
        }
        //------------------------------------------------------
        public void PauseRunAlongPathPoint()
        {
            if (m_bRunningAlongPathPoint)
                m_bRunningAlongPathPlay = false;
        }
        //------------------------------------------------------
        public void ResumeRunAlongPathPoint()
        {
            if (m_bRunningAlongPathPoint)
                m_bRunningAlongPathPlay = true;
        }
        //------------------------------------------------------
        public Vector3Track GetRuningPath()
        {
            return m_pPathPointTrack;
        }
        public bool IsRunningAlongPathPlaying() { return m_bRunningAlongPathPlay && m_bRunningAlongPathPoint; }
        //------------------------------------------------------
        public bool IsRunningAlongPathPoint() { return m_bRunningAlongPathPoint; }
        //--------------------------------------------------------
        protected virtual void OnRunAlongPathPoint(System.Collections.Generic.List<FVector3> vPathPoint) { }
        //--------------------------------------------------------
        protected virtual void OnRunAlongPathPoint(System.Collections.Generic.List<RunPoint> vPathPoint) { }
        //--------------------------------------------------------
        protected virtual void OnRunAlongPathPoint(FVector3 srcPos, FVector3 toPos) { }
        //--------------------------------------------------------
        protected virtual void OnStopAlongPathPoint() { }
        //--------------------------------------------------------
        protected override void OnUpdate(ExternEngine.FFloat fDelta)
        {
            if (m_pActor.IsFlag(EWorldNodeFlag.Killed))
                return;
                UpdateRunAlongPathPoint(fDelta);
        }
        //--------------------------------------------------------
        protected void UpdateRunAlongPathPoint(FFloat fFrame)
        {
            if (m_bRunningAlongPathPoint)
            {
                if (m_bRunningAlongPathPlay) m_fRunAlongPathPointDelta += fFrame;
                FVector3 vTargetPosition, vToPosition;
                EActionStateType actionType = EActionStateType.None;
                bool bOk = m_pPathPointTrack.Evaluate(m_fRunAlongPathPointDelta, out vTargetPosition, out vToPosition, out actionType);
                if (!bOk)
                {
                    StopRunAlongPathPoint();
                    return;
                }
                FVector3 vPostionOffset = vTargetPosition - m_pActor.GetPosition();
                if (m_bLocalRunAlongPath)
                {
                    m_pActor.SetPositionOffset(vTargetPosition);
                }
                else if (m_bRunAlongPathPointEnsureSucceed)
                {
                    m_pActor.SetPosition(vTargetPosition);
                }
                else
                {
                    FFloat fOffset = vPostionOffset.magnitude;
                    if (fOffset > 0.01f)
                    {
                        FVector3 vSpeedVector = vPostionOffset / fOffset;
                        vSpeedVector.y = 0.0f;
                        m_pActor.SetSpeedXZ(vSpeedVector * m_fRunAlongPathPointSpeed);
                    }
                }
                if (m_bRunAlongPathPointUpdateDirection)
                {
                    vPostionOffset.y = 0;
                    bool bForward = vPostionOffset.x > 0f;
                    if (m_pActor.IsFlag(EWorldNodeFlag.Facing2D))
                        m_pActor.SetDirection(bForward ? FVector3.right : (-FVector3.right), 0.1f, false);
                    else
                    {
                        vPostionOffset = vToPosition - m_pActor.GetPosition();
                        vPostionOffset.y = 0;
                        m_pActor.SetDirection(vPostionOffset.normalized, 0.1f, false);
                    }
                }
                if (m_bRunningAlongPathPlay && actionType != EActionStateType.None && actionType != EActionStateType.Idle)
                    m_pActor.StartActionState(actionType);
                if (m_fRunAlongPathPointDelta >= m_fRunAlongPathPointTime)
                    StopRunAlongPathPoint();
            }
        }
        //--------------------------------------------------------
        protected override void OnDestroy()
        {
        }
	}
}
#endif