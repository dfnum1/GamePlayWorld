/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SpawnSplineManager
作    者:	HappLI
描    述:	出生曲线管理器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if USE_SERVER
using AnimationCurve = ExternEngine.AnimationCurve;
#endif
namespace Framework.Core
{
    public class SplineManager : AModule
    {
        struct RuntimeData
        {
            public Core.AWorldNode pNode;
            public SplineData splineData;
            public Vector3 spawnPosition;

            public bool lockedState;
            public float fDuration;
            public float time;

            public Vector3 offsetPosition;
            //------------------------------------------------------
            public bool Evaluate(float fTime, ref Vector3 retPos, ref Vector3 retEuler, ref Vector3 retScale)
            {
                if(splineData.Evaluate(fTime, ref retPos, ref retEuler,ref retScale))
                {
                    retPos += offsetPosition;
                    return true;
                }
                return false;
            }
        }
        List<RuntimeData> m_Runtimes;
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_Runtimes = new List<RuntimeData>(16);
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            m_Runtimes.Clear();
        }
        //------------------------------------------------------
        public static void OffsetSpline(AFramework framework, Vector3 offset)
        {
            SplineManager module = framework.GetModule<SplineManager>();
            if (module == null || module.m_Runtimes == null) return;
            RuntimeData runtime;
            for (int i =0; i < module.m_Runtimes.Count; ++i)
            {
                runtime = module.m_Runtimes[i];
                runtime.offsetPosition = offset;
                module.m_Runtimes[i] = runtime;
            }
        }
        //------------------------------------------------------
        public static bool AddSpawnSpline(Core.AWorldNode pNode, SplineData data)
        {
            if (pNode == null) return false;
            if (data.Frames == null || data.Frames.Length <= 0) return false;
            SplineManager module = pNode.GetGameModule().GetModule<SplineManager>();
            if (module == null) return false;

            float fMaxTime = 0;
            if(data.Frames != null)
            {
                for (int i = 0; i < data.Frames.Length; ++i)
                    fMaxTime = Mathf.Max(data.Frames[i].time, fMaxTime);
            }
            if (fMaxTime <= 0) return false;

            RuntimeData runtimeData;
            for (int i =0; i < module.m_Runtimes.Count; ++i)
            {
                runtimeData = module.m_Runtimes[i];
                if (runtimeData.pNode == pNode)
                {
                    runtimeData.splineData = data;
                    runtimeData.fDuration = fMaxTime;
                    module.m_Runtimes[i] = runtimeData;
                    runtimeData.spawnPosition = pNode.GetPosition();
                    return true;
                }
            }
            runtimeData = new RuntimeData();
            runtimeData.splineData = data;
            runtimeData.fDuration = fMaxTime;
            runtimeData.time = module.GetFramework().GetRunTime()*0.001f;
            runtimeData.pNode = pNode;
            runtimeData.lockedState = pNode.IsFlag(Core.EWorldNodeFlag.Logic);
            runtimeData.spawnPosition = pNode.GetPosition();
            pNode.SetFlag(Core.EWorldNodeFlag.Logic, false);
            Vector3 fPos = Vector3.zero;
            Vector3 fEuler = Vector3.zero;
            Vector3 fScale = Vector3.one;
            if (data.GetLastFrame(ref fPos, ref fEuler, ref fScale))
            {
                pNode.SetPosition(fPos + runtimeData.spawnPosition);
                pNode.SetEulerAngle(fEuler);
                pNode.SetScale(fScale);
            }
            module.m_Runtimes.Add(runtimeData);
            return true;

        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            if (m_Runtimes.Count <= 0) return;
            float fTime = GetFramework().GetRunTime()*0.001f;
            float delta = 0;
            Vector3 position = Vector3.zero;
            Vector3 eulerAngle = Vector3.zero;
            Vector3 scale = Vector3.one;
            RuntimeData runtime;
            for (int i = 0; i < m_Runtimes.Count;)
            {
                runtime = m_Runtimes[i];
                if (runtime.pNode == null || runtime.pNode.IsKilled() || runtime.pNode.IsDestroy())
                {
                    m_Runtimes.RemoveAt(i);
                    continue;
                }
                if (!runtime.pNode.IsActived())
                {
                    runtime.time = fTime;
                    m_Runtimes[i] = runtime;
                    ++i;
                    continue;
                }
                if (runtime.pNode.GetObjectAble() == null)
                {
                    runtime.time = fTime;
                    m_Runtimes[i] = runtime;
                    ++i;
                    continue;
                }
                runtime.pNode.EnableLogic(false);
                delta = fTime - runtime.time;
                if(runtime.Evaluate(delta, ref position, ref eulerAngle, ref scale))
                {
                    runtime.pNode.SetPosition(position+runtime.spawnPosition);
                    runtime.pNode.SetEulerAngle(eulerAngle);
                    runtime.pNode.SetScale(scale);
                }
                if(delta >= runtime.fDuration)
                {
                    runtime.pNode.EnableLogic(runtime.lockedState);
                    m_Runtimes.RemoveAt(i);
                    continue;
                }
                ++i;
            }
        }
    }
}
