/********************************************************************
生成日期:	23:3:2020   16:12
类    名: 	AsyncJobSystem
作    者:	HappLI
描    述:	异步工作流
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Collections;
using System.Linq;

#if USE_UNITY_JOB
using Unity.Jobs;
#endif

namespace Framework.Core
{
    public class AsyncJobSystem : AJobSystem
    {
#if USE_UNITY_JOB
        public struct FrameJob
        {
            public AJobSystem jobSystem;
        }
        public struct JobTask : IJob
        {
            public int JobIndex;
            public void Execute()
            {
                if(AFramework.mainFramework!=null && AFramework.mainFramework.jobSystem!=null)
                    AFramework.mainFramework.jobSystem.DoJobUpdate(JobIndex,0.033f, null);
            }
        }
    //    Unity.Collections.NativeArray<JobHandle> m_JobHandleList;
   //     int                                      m_nJobCount = 0;
#endif
        class AsyncQueueItem
        {
            public int job = -1;
            public int millSecondSleep = 0;
            public IUserData userData = null;
        }
#if USE_UNITY_JOB
        private JobHandle[]                         m_jobHandles;
#endif
        private AsyncQueueItem[]                    m_arrAsyncJobs = null;
        //-----------------------------------------------------
        public AsyncJobSystem() : base()
        {
        }
        //-----------------------------------------------------
        protected override void OnStart()
        {
            base.OnStart();
            if (m_nMaxThreads <= 0)
                return;
#if USE_UNITY_JOB
            m_jobHandles = new JobHandle[m_nMaxThreads];
#endif
            m_arrAsyncJobs = new AsyncQueueItem[m_nMaxThreads];
            for (int i = 0; i < m_nMaxThreads; ++i)
            {
                m_arrAsyncJobs[i] = new AsyncQueueItem();
#if USE_UNITY_JOB
                m_jobHandles[i] = default;
#endif
            }
        }
        //-----------------------------------------------------
        protected override void OnDestroy()
        {
#if USE_UNITY_JOB
        //    if (m_JobHandleList.IsCreated)
      //          m_JobHandleList.Dispose();
      //      m_nJobCount = 0;
#endif
        }
        //-----------------------------------------------------
        protected override void InnerUpdate(float fFrame)
        {
#if USE_UNITY_JOB
            if (m_vAsyncJobs == null) return;
            int jobCount = 0;
            foreach (var jobList in m_vAsyncJobs)
            {
                if (jobList != null && jobList.Count > 0)
                    jobCount += jobList.Count;
            }

            if (jobCount == 0) return;

            for (int i = 0; i < m_vAsyncJobs.Length; ++i)
            {
                var jobList = m_vAsyncJobs[i];
                if (jobList == null) continue;
                foreach (var jobTask in jobList)
                {
                    if (m_jobHandles[i].IsCompleted)
                    {
                        var job = new JobTask { JobIndex = i };
                        m_jobHandles[i] = job.Schedule();
                    }
                }
            }
            /*
            if(m_nJobCount<=0)
            {
                int cnt = 0;
                for (int i = 0; i < m_vAsyncJobs.Length; ++i)
                {
                    if (m_vAsyncJobs[i] != null && m_vAsyncJobs[i].Count > 0)
                        cnt++;
                }
                if (cnt <= 0) 
                    return;
                if(!m_JobHandleList.IsCreated || m_JobHandleList.Length< cnt)
                {
                    if (m_JobHandleList.IsCreated) m_JobHandleList.Dispose();
                }
                if(!m_JobHandleList.IsCreated)
                    m_JobHandleList = new Unity.Collections.NativeArray<JobHandle>(cnt, Unity.Collections.Allocator.Persistent);
                int index = 0;
                for (int i = 0; i < m_vAsyncJobs.Length; ++i)
                {
                    if (m_vAsyncJobs[i] != null && m_vAsyncJobs[i].Count > 0)
                    {
                        JobTask task = new JobTask();
                        task.JobIndex = i;
                        m_JobHandleList[index++] = task.Schedule();
                    }
                }
                m_nJobCount = index;
            }
            else if(m_nJobCount>0)
            {
                bool bAllCompeleted = true;
                for(int i =0; i < m_nJobCount; ++i)
                {
                    if (!m_JobHandleList[i].IsCompleted)
                    {
                        bAllCompeleted = false;
                        m_JobHandleList[i].Complete();
                        break;
                    }
                }
                if (bAllCompeleted)
                {
                    m_nJobCount = 0;
                }
            }*/
#endif
        }
        //-----------------------------------------------------
        protected override void OnAddJob(int job, IUserData userData = null)
        {
        }
        //-----------------------------------------------------
        protected override void OnAddThread(int job, int millSleep = 500, IUserData userData = null)
        {
            if (m_arrAsyncJobs[job].job == -1)
            {
                m_arrAsyncJobs[job].job = job;

                m_arrAsyncJobs[job].millSecondSleep = millSleep;
                m_arrAsyncJobs[job].userData = userData;
#if UNITY_WEBGL
                GetFramework().BeginCoroutine(CoroutineRunActionJob(m_arrAsyncJobs[job]));
#else
                ThreadPool.QueueUserWorkItem(RunActionJob, m_arrAsyncJobs[job]);
#endif
            }
        }

#if UNITY_WEBGL
        //-----------------------------------------------------
        private System.Collections.IEnumerator CoroutineRunActionJob(AsyncQueueItem queueItem)
        {
            bool bDo = true;
            while (bDo)
            {
                try
                {
                    bDo = DoJobUpdate(queueItem.job, 0.0333f, queueItem.userData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                }
                if (queueItem.millSecondSleep > 0)
                    yield return new WaitForSeconds(queueItem.millSecondSleep / 1000.0f);
            }
            int job = queueItem.job;
            if (job >= 0)
            {
                LockJob(job);
                queueItem.job = -1;
                UnLockJob(job);
            }
        }
#endif
        //-----------------------------------------------------
        private void RunActionJob(object action)
        {
            try
            {
                AsyncQueueItem queueItem = (AsyncQueueItem)action;
                while (DoThreadUpdate(queueItem.job, 0.0333f, queueItem.userData))
                {
                    if (queueItem.millSecondSleep > 0)
                        System.Threading.Thread.Sleep(queueItem.millSecondSleep);
                }
                int job = queueItem.job;
                if (job >= 0)
                {
                    LockThreadJob(job);
                    queueItem.job = -1;
                    UnLockThreadJob(job);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
            finally
            {

            }
        }
    }
}
