using System.Collections.Generic;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
namespace Framework.Core
{
    public interface IServerSyncCallback
    {
        bool OnServerSyncCallback(AWorldNode pNode, SvrSyncData syncData, bool bIn);
    }

    public enum EDefaultSyncType
    {
        Position = 0,
        EulerAngle =1,
        Scale = 2,
        ActionType = 3,
        Freeze = 4,
        NodeFlag = 5,
        SkillAdd = 6,
        SkillDel = 7,
        SkillDo = 8,
    }
    public struct SvrSyncData
    {
        public int type;
        public int data0;
        public int data1;
        public int data2;
        public int data3;
        public SvrSyncData(int type, bool data0)
        {
            this.type = type;
            this.data0 = data0?1:0;
            this.data1 = 0;
            this.data2 = 0;
            this.data3 = 0;
        }
        public SvrSyncData(int type, int data0)
        {
            this.type = type;
            this.data0 = data0;
            this.data1 = 0;
            this.data2 = 0;
            this.data3 = 0;
        }
        public SvrSyncData(int type, int data0, int data1)
        {
            this.type = type;
            this.data0 = data0;
            this.data1 = data1;
            this.data2 = 0;
            this.data3 = 0;
        }
        public SvrSyncData(int type, int data0, int data1, int data2)
        {
            this.type = type;
            this.data0 = data0;
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = 0;
        }
        public SvrSyncData(int type, int data0, int data1, int data2, int data3)
        {
            this.type = type;
            this.data0 = data0;
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
        }
#if USE_FIXEDMATH
        public SvrSyncData(int type, FVector3 va)
        {
            this.type = type;
            this.data0 = (int)(va[0].ToFloat()*100);
            this.data1 = (int)(va[1].ToFloat() * 100);
            this.data2 = (int)(va[2].ToFloat() * 100);
            this.data3 = 0;
        }
#endif
        public SvrSyncData(int type, UnityEngine.Vector3 va)
        {
            this.type = type;
            this.data0 = (int)(va[0] * 100);
            this.data1 = (int)(va[1] * 100);
            this.data2 = (int)(va[2] * 100);
            this.data3 = 0;
        }
        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(data0*0.01f, data1 * 0.01f, data2 * 0.01f);
        }
#if USE_FIXEDMATH
        public FVector3 ToFVector3()
        {
            return new FVector3(FMath.ToFloat(data0 * 0.01f), FMath.ToFloat(data1 * 0.01f), ExternEngine.FMath.ToFloat(data2 * 0.01f));
        }
#endif
        public bool ToBool()
        {
            return data0 != 0;
        }
    }
    //-------------------------------------------------
    //! 服务器数据同步器
    //-------------------------------------------------
    public abstract class AServerSync : IUserData
    {
        protected uint m_nSyncFlags = 0xffffffff;
#if USE_FIXEDMATH
        protected FFloat m_fSyncSpeed = FFloat.one;
        protected FFloat m_fRunPointDuration = FFloat.zero;
        protected FFloat m_fRunPointTime = FFloat.zero;
#else
        protected FFloat m_fSyncSpeed = 1.0f;
        protected FFloat m_fRunPointDuration = 0.0f;
        protected FFloat m_fRunPointTime = 0.0f;
#endif
        bool m_bDisableSync = false;
        protected AWorldNode m_pNode;
        protected FVector3 m_FromPosition = FVector3.zero;
        protected FVector3 m_ToPosition = FVector3.zero;
        protected FVector3 m_vFinalEulerangle = FVector3.zero;
        protected List<FVector3> m_vRunPoints = new List<FVector3>();

        List<IServerSyncCallback> m_vCallback = null;
        //-------------------------------------------------
        internal void Awake(AWorldNode pNode)
        {
#if USE_FIXEDMATH
            m_fSyncSpeed = FFloat.one;
#else
            m_fSyncSpeed = 1.0f;
#endif
            m_pNode = pNode;
            OnAwake(pNode);
        }
        //-------------------------------------------------
        protected virtual void OnAwake(AWorldNode node) { }
        //-------------------------------------------------
        public void EnabeSyncFlag(EDefaultSyncType type, bool bEnable)
        {
            EnableSyncFlag((uint)type, bEnable);
        }
        //-------------------------------------------------
        public void EnableSyncFlag(uint type, bool bEnable)
        {
            if (bEnable) m_nSyncFlags |= (uint)(1 << (int)type);
            else m_nSyncFlags &= (uint)(~(1 << (int)type));
        }
        //-------------------------------------------------
        public void OutSyncData(SvrSyncData syncData)
        {
            if (m_bDisableSync || m_pNode == null || !m_pNode.IsSvrSyncOut()) return;
            DoCallback(syncData, false);
            OnInnerOutSyncData(syncData);
        }
        //-------------------------------------------------
        public void InSyncData(SvrSyncData syncData)
        {
            if (m_pNode == null || !m_pNode.IsSvrSyncIn()) return;
            m_bDisableSync = true;
            OnInSyncData(syncData);
            m_bDisableSync = false;
        }
        //-------------------------------------------------
        void OnInSyncData(SvrSyncData syncData)
        {
            if (m_pNode == null) return;
            DoCallback(syncData, true);
            m_bDisableSync = true;
            switch ((EDefaultSyncType)syncData.type)
            {
                case EDefaultSyncType.Position:
                    {
                        //   m_pNode.RunAlongPathPoint(m_pNode.GetPosition(), syncData.ToVector3(), m_pNode.GetRunSpeed(), true);
                        //         m_pNode.SetPosition(syncData.ToVector3());
#if USE_FIXEDMATH
                        if(m_fSyncSpeed <= FFloat.zero)
                            m_pNode.SetFinalPosition(syncData.ToVector3());
                        else
                            m_vRunPoints.Add(syncData.ToVector3());
#else
                        if (m_fSyncSpeed <= 0.0f)
                            m_pNode.SetFinalPosition(syncData.ToVector3());
                        else
                            m_vRunPoints.Add(syncData.ToVector3());
#endif
                    }
                    break;
                case EDefaultSyncType.EulerAngle:
                    {
                        m_vFinalEulerangle = syncData.ToVector3();
                        if(m_vRunPoints.Count<=0)
                            m_pNode.SetEulerAngle(m_vFinalEulerangle);
                    }
                    break;
                case EDefaultSyncType.Scale:
                    {
                        m_pNode.SetScale(syncData.ToVector3());
                    }
                    break;
                case EDefaultSyncType.ActionType:
                    {
                        ((Actor)m_pNode).StartActionState((EActionStateType)syncData.data0, (uint)syncData.data1);
                    }
                    break;
                case EDefaultSyncType.Freeze:
                    {
#if USE_FIXEDMATH
                        m_pNode.Freezed(syncData.ToBool(), FFloat.MaxValue);
#else
                        m_pNode.Freezed(syncData.ToBool(), float.MaxValue);
#endif
                    }
                    break;
                case EDefaultSyncType.NodeFlag:
                    {
                        if(syncData.data0 <= (int)EWorldNodeFlag.RVO)
                            m_pNode.SetFlag((EWorldNodeFlag)syncData.data0, syncData.data1 != 0);
                    }
                    break;
            }
            OnInnerInSyncData(syncData);
            m_bDisableSync = false;
        }
        //------------------------------------------------------
        protected virtual void OnInnerInSyncData(SvrSyncData syncData)
        {

        }
        //------------------------------------------------------
        protected virtual void OnInnerOutSyncData(SvrSyncData syncData)
        {

        }
        //------------------------------------------------------
        public void AddCallback(IServerSyncCallback callback)
        {
            if (callback == null) return;
            if (m_vCallback == null)
            {
                m_vCallback = new List<IServerSyncCallback>();
                m_vCallback.Add(callback);
            }
            else
            {
                if (m_vCallback.Contains(callback)) return;
                m_vCallback.Add(callback);
            }
        }
        //------------------------------------------------------
        public void RemoveCallback(IServerSyncCallback callback)
        {
            if (callback == null || m_vCallback == null) return;
            m_vCallback.Remove(callback);
        }
        //------------------------------------------------------
        void DoCallback(SvrSyncData syncData, bool bIn)
        {
            if (m_vCallback == null) return;
            for(int i =0; i < m_vCallback.Count;)
            {
                if(m_vCallback[i]!=null)
                {
                    if(!m_vCallback[i].OnServerSyncCallback(m_pNode, syncData, bIn))
                    {
                        m_vCallback.RemoveAt(i);
                        continue;
                    }
                }
                ++i;
            }
        }
        //------------------------------------------------------
        internal void Update(FFloat frameTime)
        {
            if (m_pNode == null) return;
            if(m_pNode.IsSvrSyncIn())
            {
#if USE_FIXEDMATH
                if (m_fRunPointTime > FFloat.zero)
#else
                if (m_fRunPointTime > 0.0f)
#endif
                {
                    m_fRunPointDuration += frameTime;
#if USE_FIXEDMATH
                    m_pNode.SetFinalPosition(FVector3.Lerp(m_FromPosition, m_ToPosition, FMath.Clamp01(m_fRunPointDuration / m_fRunPointTime)));
#else
                    m_pNode.SetFinalPosition(FVector3.Lerp(m_FromPosition, m_ToPosition, UnityEngine.Mathf.Clamp01(m_fRunPointDuration / m_fRunPointTime)));
#endif
                    FVector3 dir = m_ToPosition - m_FromPosition;
                    dir.y = 0;
                    m_pNode.SetDirection(dir);
                    Actor pActor = m_pNode as Actor;
                    if (pActor != null)
                    {
                        pActor.SetIdleType(EActionStateType.Run);
                        pActor.StartActionState(EActionStateType.Run, 0);
                    }
                    if (m_fRunPointDuration >= m_fRunPointTime)
                    {
                        m_fRunPointDuration = 0.0f;
                        m_fRunPointTime = 0.0f;

                        if (m_vRunPoints.Count <= 0)
                        {
                            if (pActor != null)
                            {
                                pActor.SetIdleType(EActionStateType.Idle);
                                pActor.StartActionState(EActionStateType.Idle, 0);
                            }
                            m_pNode.SetEulerAngle(m_vFinalEulerangle);
                        }
                    }
                }
                else
                {
#if USE_FIXEDMATH
                    FFloat fSpeed = FMath.Max(FFloat.one, m_pNode.GetRunSpeed() * m_fSyncSpeed);
                    FFloat speedSqr = FMath.Sqr(fSpeed);
                    FFloat gap = new FFloat(0.9f);
#else
                    FFloat fSpeed = System.Math.Max(1.0f, m_pNode.GetRunSpeed() * m_fSyncSpeed);
                    FFloat speedSqr = fSpeed* fSpeed;
                    FFloat gap = 0.9f;
#endif
                    FVector3 curPos = m_pNode.GetPosition();
                    FVector3 toPos = m_ToPosition;
                    while (m_vRunPoints.Count > 0)
                    {
                        if(m_vRunPoints.Count> 1)
                        {
                            if((m_vRunPoints[0]- toPos).sqrMagnitude<= speedSqr)
                            {
                                m_vRunPoints.RemoveAt(0);
                                continue;
                            }
                            if (FVector3.Dot(toPos - curPos, m_vRunPoints[0] - curPos) >= gap)
                            {
                                m_vRunPoints.RemoveAt(0);
                                continue;
                            }
                        }

                        m_fRunPointTime = (curPos - m_vRunPoints[0]).magnitude / fSpeed;
#if USE_FIXEDMATH
                        m_fRunPointDuration = FFloat.zero;
#else
                        m_fRunPointDuration = 0.0f;
#endif
                        m_FromPosition = m_pNode.GetPosition();
                        m_ToPosition = m_vRunPoints[0];
                        m_vRunPoints.RemoveAt(0);
                        break;
                    }
                }
            }
            
            
            OnUpdate(frameTime);
        }
        //------------------------------------------------------
        protected virtual void OnUpdate(FFloat fFrameTime) { }
        //------------------------------------------------------
        public void Destroy()
        {
            OnDestroy();
#if USE_FIXEDMATH
            m_fRunPointDuration = FFloat.zero;
            m_fRunPointTime = FFloat.zero;
#else
            m_fRunPointDuration = 0.0f;
            m_fRunPointTime = 0.0f;
#endif
            m_vRunPoints.Clear();
            m_ToPosition = FVector3.zero;
            m_FromPosition = FVector3.zero;
            m_pNode = null;
            if (m_vCallback != null) m_vCallback.Clear();
        }
        //------------------------------------------------------
        protected virtual void OnDestroy()
        {

        }
    }
}
