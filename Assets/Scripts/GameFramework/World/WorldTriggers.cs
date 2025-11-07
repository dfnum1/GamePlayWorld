/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldTriggers
作    者:	HappLI
描    述:	世界触发器
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector4 = UnityEngine.Vector4;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public struct WorldTriggerParamter
    {
        public string name;
        public int typeFlags;
        public short triggetCnt;
        public Vector3 position;
        public Vector3 eulerAngle;
        public Vector3 aabb_min;
        public Vector3 aabb_max;
        public List<string> events;

        [System.NonSerialized]
        public List<BaseEvent> runtimeEvents;
        public void Create(AFramework pFramework)
        {
            if (events == null)
            {
                if (runtimeEvents != null) runtimeEvents.Clear();
                return;
            }
            if(runtimeEvents == null)
            {
                if (runtimeEvents == null) runtimeEvents = new List<BaseEvent>(events.Count);
                for (int i = 0; i < events.Count; ++i)
                {
                    var eventP = BuildEventUtl.BuildEvent(pFramework, events[i]);
                    if (eventP == null) continue;
                    runtimeEvents.Add(eventP);
                }
            }
        }

        public static WorldTriggerParamter DEF = new WorldTriggerParamter() { events = null, typeFlags = 0 };
        public bool IsValid()
        {
            return events != null && events.Count > 0 && (aabb_min-aabb_max).sqrMagnitude>0;
        }
    }
    public class WorldTriggers
    {
        struct FrameTrigger
        {
            public AWorldNode pChecker;
            public WorldTriggerData data;
            public FrameTrigger(AWorldNode pChecker, WorldTriggerData triggertData)
            {
                this.pChecker = pChecker;
                this.data = triggertData;
            }
        }
        private ObjectStackPool<WorldTriggerData> m_vPools = null;
        class WorldTriggerData
        {
            public int id;
            public AWorldNode owner;
            public WorldTriggerParamter parameter;
            public int triggerCnt;
            public void Clear()
            {
                id = 0;
                owner = null;
                triggerCnt = 0;
            }
            public FVector3 GetPosition()
            {
                if (owner != null) return owner.GetPosition() + parameter.position;
                return parameter.position;
            }
            public FVector3 GetEulerAngle()
            {
                if (owner != null) return owner.GetEulerAngle() + parameter.eulerAngle;
                return parameter.eulerAngle;
            }
        }
        List<WorldTriggerData> m_vTriggers = null;
        int m_nTriggerAUTOID = 0;
        List<FrameTrigger> m_vFrameTriggereds = null;

        bool m_bDirtyList = false;
        World m_pWorld = null;
        public WorldTriggers(World pWorld)
        {
            m_pWorld = pWorld;
            m_vFrameTriggereds = new List<FrameTrigger>(4);
            m_vTriggers = new List<WorldTriggerData>(8);
            m_vPools = new ObjectStackPool<WorldTriggerData>(20);
        }
        //-------------------------------------------------
        public void Clear()
        {
            m_bDirtyList = false;
            if (m_vTriggers!=null)
            {
                for(int i =0; i < m_vTriggers.Count; ++i)
                {
                    m_vTriggers[i].Clear();
                    m_vPools.Release(m_vTriggers[i]);
                }
                m_vTriggers.Clear();
            }
            if (m_vFrameTriggereds != null) m_vFrameTriggereds.Clear();
        }
        //-------------------------------------------------
        internal void TriggerCheck(AWorldNode pNode)
        {
            if (pNode == null) return;
            if (m_vTriggers == null) return;

            AFramework pFramework = pNode.GetGameModule();
            if (pFramework == null) return;
            FVector3 vCenter = pNode.GetBounds().GetCenter();
            FVector4 vHalf = pNode.GetBounds().GetHalf();
            FMatrix4x4 vWorldMt = pNode.GetMatrix();

           FFloat nodeSizeSqr =  pNode.GetBounds().GetBoundSizeSqr();

            WorldTriggerData trigger;
            for (int i =0; i < m_vTriggers.Count;)
            {
                if (m_bDirtyList) break;
                trigger = m_vTriggers[i];
                if (trigger.owner != null)
                {
                    if (trigger.owner.IsKilled() || trigger.owner.IsDestroy())
                    {
                        m_vTriggers.RemoveAt(i);
                        continue;
                    }
                    if(!trigger.owner.IsActived() || !trigger.owner.IsVisible())
                    {
                        ++i;
                        continue;
                    }
                }

                if ((trigger.parameter.typeFlags & (1 << (int)pNode.GetActorType())) != 0)
                {
                    if ((trigger.GetPosition() - pNode.GetPosition()).sqrMagnitude >= (nodeSizeSqr + (trigger.parameter.aabb_max - trigger.parameter.aabb_min).sqrMagnitude))
                    {
                        ++i;
                        continue;
                    }
                    FVector3 vC1 = (trigger.parameter.aabb_min + trigger.parameter.aabb_max) * 0.5f;
                    FVector3 vH1 = trigger.parameter.aabb_max - vC1;
                    FMatrix4x4 mtTemp = FMatrix4x4.identity;
                    mtTemp.SetTRS(trigger.GetPosition(), FQuaternion.Euler(trigger.GetEulerAngle()), FVector3.one);
                    if (Base.IntersetionUtil.CU_LineOBBIntersection(pFramework.shareParams.intersetionParam, pNode.GetLastPosition(), pNode.GetPosition(), vC1, vH1, mtTemp) ||
                        Base.IntersetionUtil.CU_OBBOBBIntersection(pFramework.shareParams.intersetionParam, vCenter, vHalf, vWorldMt, vC1, vH1, mtTemp))
                    {
                        if (m_vFrameTriggereds == null) m_vFrameTriggereds = new List<FrameTrigger>();
                        m_vFrameTriggereds.Add(new FrameTrigger(pNode, trigger));
                    }
                }
                ++i;
            }
            m_bDirtyList = false;
        }
        //-------------------------------------------------
        internal void Update()
        {
            if (m_vFrameTriggereds == null) return;
            FrameTrigger frameTrigger;
            for (int i =0; i < m_vFrameTriggereds.Count; ++i)
            {
                frameTrigger =  m_vFrameTriggereds[i];
                if(frameTrigger.pChecker!=null)
                {
                    if(frameTrigger.pChecker.OnWorldTrigger(frameTrigger.data.owner, frameTrigger.data.parameter))
                    {
                        frameTrigger.data.triggerCnt++;

                        var eventTrigger = frameTrigger.pChecker.GetGameModule().eventSystem;
                        eventTrigger.Begin();
                        eventTrigger.ATuserData = frameTrigger.pChecker;
                        eventTrigger.TriggerEventPos = frameTrigger.pChecker.GetPosition();
                        eventTrigger.TriggerEventRealPos = frameTrigger.pChecker.GetPosition();
                        eventTrigger.TriggerActorDir = frameTrigger.pChecker.GetDirection();
                        for(int j = 0; j < frameTrigger.data.parameter.runtimeEvents.Count; ++j)
                            eventTrigger.OnTriggerEvent(frameTrigger.data.parameter.runtimeEvents[j]);
                        eventTrigger.End();
                    }
                }
                if(frameTrigger.data.parameter.triggetCnt>0 && frameTrigger.data.triggerCnt >= frameTrigger.data.parameter.triggetCnt)
                {
                    frameTrigger.data.Clear();
                    m_vPools.Release(frameTrigger.data);
                    m_vTriggers.Remove(frameTrigger.data);
                }
            }
            m_vFrameTriggereds.Clear();
        }
        //-------------------------------------------------
        public int AddTrigger(AWorldNode pOwner, WorldTriggerParamter parameter)
        {
            if (!parameter.IsValid()) return 0;
            if (m_vTriggers == null) m_vTriggers = new List<WorldTriggerData>();
            parameter.Create(m_pWorld.GetFramework());
            if (parameter.runtimeEvents == null || parameter.runtimeEvents.Count <= 0)
                return 0;

            WorldTriggerData data = m_vPools.Get();
            data.id = ++m_nTriggerAUTOID;
            data.owner = pOwner;
            data.parameter = parameter;
            m_vTriggers.Add(data);
            m_bDirtyList = true;
            return data.id;
        }
        //-------------------------------------------------
        public void RemoveTrigger(int id)
        {
            if (id == 0) return;
            if (m_vTriggers == null) return;
            m_bDirtyList = false;
            for (int i =0; i < m_vTriggers.Count;)
            {
                if (m_vTriggers[i].id == id)
                {
                    m_bDirtyList = true;
                    m_vTriggers.RemoveAt(i);
                }
                else ++i;
            }
        }
        //-------------------------------------------------
        public void RemoveTrigger(AWorldNode pOwner)
        {
            if (m_vTriggers == null) return;
            m_bDirtyList = false;
            for (int i = 0; i < m_vTriggers.Count;)
            {
                if (m_vTriggers[i].owner == pOwner)
                {
                    m_bDirtyList = true;
                    m_vTriggers.RemoveAt(i);
                }
                else ++i;
            }
        }
#if UNITY_EDITOR
        //-------------------------------------------------
        public void DrawDebug()
        {
            if (m_vTriggers == null) return;
            WorldTriggerData trigger;
            for (int i = 0; i < m_vTriggers.Count; ++i)
            {
                trigger = m_vTriggers[i];
                FVector3 vC1 = (trigger.parameter.aabb_min + trigger.parameter.aabb_max) * 0.5f;
                FVector3 vH1 = trigger.parameter.aabb_max - vC1;
                FMatrix4x4 mtTemp = FMatrix4x4.identity;
                mtTemp.SetTRS(trigger.GetPosition(), FQuaternion.Euler(trigger.GetEulerAngle()), FVector3.one);

                ED.EditorUtil.DrawBoundingBox(vC1, vH1, mtTemp, Color.cyan, true);
            }
        }
#endif
    }
}
