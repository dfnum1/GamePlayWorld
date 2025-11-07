/********************************************************************
生成日期:	23:3:2020   16:12
类    名: 	AsyncJobTask
作    者:	HappLI
描    述:	异步工作流
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework.Core
{
    //-----------------------------------------------------
    // JobTask
    //-----------------------------------------------------
    public struct JobTask
    {
        public IJobUpdate job;
        public int guid;
        public JobTask(IJobUpdate job, int guid)
        {
            this.job = job;
            this.guid = guid;
        }
        public bool OnJobUpdate(float fFrame, IUserData takeData)
        {
            return job.OnJobUpdate(fFrame, takeData);
        }
    }
    //-----------------------------------------------------
    // ThreadTask
    //-----------------------------------------------------
    struct ThreadTask
    {
        public IThreadJob job;
        public int guid;
        public ThreadTask(IThreadJob job, int guid)
        {
            this.job = job;
            this.guid = guid;
        }
        public bool OnThreadUpdate(float fFrame, IUserData takeData)
        {
            return job.OnThreadUpdate(fFrame, takeData);
        }
    }
    public abstract class AJobSystem : AModule
    {
        public struct NoDelayedQueueItem
        {
            public Action<IUserData> action;
            public Action action_0;
            public IUserData param;
        }
        public struct DelayedQueueItem
        {
            public int time;
            public Action<IUserData> action;
            public Action action_0;
            public IUserData param;
        }

        protected bool                      m_bActiveJob = false;
        protected int                       m_nMaxThreads = 4;
        LinkedList<ThreadTask>[]            m_vThreadAsyncJobs = null;
        private int[]                       m_arrThreadLocks = null;
        private short                       m_nThreadGuid = 0;

#if USE_UNITY_JOB
        protected LinkedList<JobTask>[]     m_vAsyncJobs = null;
        private int[]                       m_arrJobLocks = null;
        private short                       m_nJobGuid = 0;
#endif

        private List<NoDelayedQueueItem>    m_vActions = new List<NoDelayedQueueItem>(2);
        LinkedList<DelayedQueueItem>        m_vDelayed = new LinkedList<DelayedQueueItem>();
        LinkedList<DelayedQueueItem>        m_vCurrentDelayed = new LinkedList<DelayedQueueItem>();
        LinkedList<NoDelayedQueueItem>      m_vCurrentActions = new LinkedList<NoDelayedQueueItem>();

        //-----------------------------------------------------
        protected override void OnAwake()
        {
#if USE_UNITY_JOB
            m_arrJobLocks = null;
            m_vAsyncJobs = null;
            m_nJobGuid = 0;
#endif
            m_vThreadAsyncJobs = null;
            m_arrThreadLocks = null;
            m_bActiveJob = false;
            m_nThreadGuid = 0;

            int maxThread = 4;
            if (GetFramework().gameStartup != null)
                maxThread = GetFramework().gameStartup.GetMaxThread();
            m_nMaxThreads = maxThread;
            if (maxThread <= 0) return;
            m_bActiveJob = true;
#if USE_UNITY_JOB
            m_vAsyncJobs = new LinkedList<JobTask>[maxThread];
            m_arrJobLocks = new int[maxThread];
#endif
            m_vThreadAsyncJobs = new LinkedList<ThreadTask>[maxThread];
            m_arrThreadLocks = new int[maxThread];
        }
        //-----------------------------------------------------
        protected override void OnStart()
        {
            if (m_nMaxThreads <= 0) return;
            m_bActiveJob = true;
        }
        //-----------------------------------------------------
        public virtual void Clear()
        {
            m_bActiveJob = false;
            if (m_nMaxThreads > 0)
            {
#if USE_UNITY_JOB
                for (int i = 0; i < m_vAsyncJobs.Length; ++i)
                {
                    LockJob(i);
                    if (m_vAsyncJobs[i] != null) m_vAsyncJobs[i].Clear();
                    UnLockJob(i);
                }
#endif
                for (int i = 0; i < m_vThreadAsyncJobs.Length; ++i)
                {
                    LockThreadJob(i);
                    if (m_vThreadAsyncJobs[i] != null) m_vThreadAsyncJobs[i].Clear();
                    UnLockThreadJob(i);
                }
            }
            m_nMaxThreads = 0;
#if USE_UNITY_JOB
            m_nJobGuid = 0;
#endif
            m_nThreadGuid = 0;
            m_vActions.Clear();
            m_vDelayed.Clear();
            m_vCurrentDelayed.Clear();
            m_vCurrentActions.Clear();
        }
        //-----------------------------------------------------
        public void Active(bool bActive)
        {
            m_bActiveJob = bActive;
        }
        //-----------------------------------------------------
        int BuildGuid(int type, int job, int index)
        {
            return (type << 8 | job) << 16 | index;
        }
        //-----------------------------------------------------
        void GetJobIndex(int guid, out int type, out int job, out int index)
        {
            type = (guid >> 24) & 0xFF;
            job = (guid >> 16) & 0xFF;
            index = guid & 0xFFFF;
        }
        //-----------------------------------------------------
        public void AbortJob(int guid)
        {
            if (guid == 0)
                return;
            GetJobIndex(guid, out var type, out var job, out var index);
            if (type == 0)
            {
#if USE_UNITY_JOB
                RemoveJob(guid);
#endif
            }
            else if (type == 1)
            {
                RemoveThread(guid);
            }
        }
#if USE_UNITY_JOB
        //-----------------------------------------------------
        public int AddJob(IJobUpdate pJob, IUserData takeData = null)
        {
            int job = pJob.GetJob();
            if (job < 0 || job >= m_nMaxThreads) return -1;
            LockJob(job);
            if (m_vAsyncJobs[job] == null)
            {
                m_vAsyncJobs[job] = new LinkedList<JobTask>();
            }
            else
            {
                int findGuid = -1;
                var vLinkList = m_vAsyncJobs[job];
                for (var node = vLinkList.First; node != null; node = node.Next)
                {
                    if (node.Value.job == pJob)
                    {
                        findGuid = node.Value.guid;
                        break;
                    }
                }
                if (findGuid >= 0)
                {
                    UnLockJob(job);
                    return findGuid;
                }
            }
            if (m_nJobGuid + 1 >= ushort.MaxValue) m_nJobGuid = 0;

            int guid = BuildGuid(0, job, ++m_nJobGuid);
            m_vAsyncJobs[job].AddLast(new JobTask(pJob, guid));
            OnAddJob(job, takeData);
            UnLockJob(job);
            return guid;
        }
        //-----------------------------------------------------
        public void RemoveJob(IJobUpdate pJob)
        {
            int job = pJob.GetJob();
            if (job < 0 || job >= m_nMaxThreads) return;
            if (m_vAsyncJobs[job] == null) return;
            LockJob(job);
            {
                var vLinkList = m_vAsyncJobs[job];
                for (var node = vLinkList.First; node != null;)
                {
                    var next = node.Next;
                    var cur = node.Value;
                    if (node.Value.job == pJob)
                    {
                        vLinkList.Remove(node);
                        break;
                    }
                    node = next;
                }
            }
            UnLockJob(job);
        }
        //-----------------------------------------------------
        public void RemoveJob(int guid)
        {
            GetJobIndex(guid, out var type, out var job, out var index);
            if (type != 0)
                return;

            if (m_vAsyncJobs[job] == null) return;
            LockJob(job);
            {
                var vLinkList = m_vAsyncJobs[job];
                for (var node = vLinkList.First; node != null;)
                {
                    var next = node.Next;
                    var cur = node.Value;
                    if (node.Value.guid == guid)
                    {
                        vLinkList.Remove(node);
                        break;
                    }
                    node = next;
                }
            }
            UnLockJob(job);
        }
#endif
        //-----------------------------------------------------
        public int AddThread(IThreadJob pJob, int millSleep = 500, IUserData takeData = null)
        {
            int job = pJob.GetThread();
            if (job < 0 || job >= m_nMaxThreads) return -1;
            LockThreadJob(job);
            if (m_vThreadAsyncJobs[job] == null)
            {
                m_vThreadAsyncJobs[job] = new LinkedList<ThreadTask>();
            }
            else
            {
                int findGuid = -1;
                var vLinkList = m_vThreadAsyncJobs[job];
                for (var node = vLinkList.First; node != null; node = node.Next)
                {
                    if (node.Value.job == pJob)
                    {
                        findGuid = node.Value.guid;
                        break;
                    }
                }
                if (findGuid >= 0)
                {
                    UnLockThreadJob(job);
                    return findGuid;
                }
            }

            if (m_nThreadGuid + 1 >= ushort.MaxValue) m_nThreadGuid = 0;

            int guid = BuildGuid(1, job, ++m_nThreadGuid);
            m_vThreadAsyncJobs[job].AddLast(new ThreadTask(pJob, guid));

            OnAddThread(job, millSleep, takeData);
            UnLockThreadJob(job);
            return guid;
        }
        //-----------------------------------------------------
        public void RemoveThread(IThreadJob pJob)
        {
            int job = pJob.GetThread();
            if (job < 0 || job >= m_nMaxThreads) return;
            if (m_vThreadAsyncJobs[job] == null) return;
            LockThreadJob(job);
            {
                var vLinkList = m_vThreadAsyncJobs[job];
                for (var node = vLinkList.First; node != null;)
                {
                    var next = node.Next;
                    var cur = node.Value;
                    if (node.Value.job == pJob)
                    {
                        vLinkList.Remove(node);
                        break;
                    }
                    node = next;
                }
            }
            UnLockThreadJob(job);
        }
        //-----------------------------------------------------
        public void RemoveThread(int guid)
        {
            GetJobIndex(guid, out var type, out var job, out var index);
            if (type != 1)
                return;
            if (job < 0 || job >= m_nMaxThreads) return;
            if (m_vThreadAsyncJobs[job] == null) return;
            LockThreadJob(job);
            {
                var vLinkList = m_vThreadAsyncJobs[job];
                for (var node = vLinkList.First; node != null;)
                {
                    var next = node.Next;
                    var cur = node.Value;
                    if (node.Value.guid == guid)
                    {
                        vLinkList.Remove(node);
                        break;
                    }
                    node = next;
                }
            }
            UnLockThreadJob(job);
        }
#if USE_UNITY_JOB
        //-----------------------------------------------------
        protected void LockJob(int job)
        {
#if !UNITY_WEBGL
            Interlocked.Increment(ref m_arrJobLocks[job]);
#endif
        }
        //-----------------------------------------------------
        protected void UnLockJob(int job)
        {
#if !UNITY_WEBGL
            Interlocked.Decrement(ref m_arrJobLocks[job]);
#endif
        }
#endif
        //-----------------------------------------------------
        protected void LockThreadJob(int job)
        {
#if !UNITY_WEBGL
            Interlocked.Increment(ref m_arrThreadLocks[job]);
#endif
        }
        //-----------------------------------------------------
        protected void UnLockThreadJob(int job)
        {
#if !UNITY_WEBGL
            Interlocked.Decrement(ref m_arrThreadLocks[job]);
#endif
        }
        //-----------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            if (!m_bActiveJob) return;
            lock (m_vActions)
            {
                m_vCurrentActions.Clear();
                foreach (var db in m_vActions)
                    m_vCurrentActions.AddLast(db);
                m_vActions.Clear();
            }

            for (var node = m_vCurrentActions.First; node != null; node = node.Next)
            {
                if (node.Value.action_0 != null)
                    node.Value.action_0();
                if (node.Value.action != null)
                    node.Value.action(node.Value.param);
            }
            lock (m_vDelayed)
            {
                m_vCurrentDelayed.Clear();
                if (m_vDelayed.Count > 0)
                {
                    int runtime = (int)GetFramework().GetRunTime();
                    for (var node = m_vDelayed.First; node != null;)
                    {
                        var next = node.Next;
                        var cur = node.Value;
                        if (runtime >= cur.time)
                        {
                            m_vCurrentDelayed.AddLast(cur);
                            m_vDelayed.Remove(node);
                        }
                        node = next;
                    }
                }
            }

            for (var node = m_vCurrentDelayed.First; node != null; node = node.Next)
            {
                if (node.Value.action_0 != null)
                    node.Value.action_0();
                if (node.Value.action != null)
                    node.Value.action(node.Value.param);
            }
            InnerUpdate(fFrame);
        }
        //-----------------------------------------------------
        public void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }
        //-----------------------------------------------------
        public void QueueOnMainThread(Action at, float time)
        {
            if (time != 0)
            {
                lock (m_vDelayed)
                {
                    m_vDelayed.AddLast(new DelayedQueueItem { time = (int)GetFramework().GetRunTime() + (int)(time * 1000), action_0 = at, action = null });
                }
            }
            else
            {
                lock (m_vActions)
                {
                    m_vActions.Add(new NoDelayedQueueItem { action_0 = at, action = null });
                }
            }
        }
        //-----------------------------------------------------
        public void QueueOnMainThread(Action<IUserData> taction, IUserData tparam)
        {
            QueueOnMainThread(taction, tparam, 0f);
        }
        //-----------------------------------------------------
        public void QueueOnMainThread(Action<IUserData> taction, IUserData tparam, float time)
        {
            if (time != 0)
            {
                lock (m_vDelayed)
                {
                    m_vDelayed.AddLast(new DelayedQueueItem { time = (int)GetFramework().GetRunTime() + (int)(time * 1000), action = taction, param = tparam, action_0 = null });
                }
            }
            else
            {
                lock (m_vActions)
                {
                    m_vActions.Add(new NoDelayedQueueItem { action = taction, param = tparam, action_0 = null });
                }
            }
        }
        //-----------------------------------------------------
        protected abstract void InnerUpdate(float fFrame);
#if USE_UNITY_JOB
        //-----------------------------------------------------
        internal bool DoJobUpdate(int job, float fFrameTime, IUserData userData = null)
        {
            if (job < 0 || job >= m_nMaxThreads) return false;
            if (m_vAsyncJobs[job] == null) return false;
            LinkedList<JobTask> vAsync = m_vAsyncJobs[job];
            for (var node = vAsync.First; node != null;)
            {
                var next = node.Next;
                var jobTask = node.Value;
                if (!jobTask.OnJobUpdate(fFrameTime, userData))
                {
                    LockJob(job);
                    {
                        vAsync.Remove(node);
                    }
                    UnLockJob(job);
                    QueueOnMainThread(jobTask.job.OnJobComplete, userData);
                }
                node = next;
            }
            return vAsync.Count > 0;
        }
#endif
        //-----------------------------------------------------
        protected bool DoThreadUpdate(int job, float fFrameTime, IUserData takeData = null)
        {
            if (job < 0 || job >= m_nMaxThreads) return false;
            if (m_vThreadAsyncJobs[job] == null) return false;
            LinkedList<ThreadTask> vAsync = m_vThreadAsyncJobs[job];
            for (var node = vAsync.First; node != null;)
            {
                var next = node.Next;
                var jobTask = node.Value;
                if (!jobTask.OnThreadUpdate(fFrameTime, takeData))
                {
                    LockThreadJob(job);
                    {
                        vAsync.Remove(node);
                    }
                    UnLockThreadJob(job);
                    QueueOnMainThread(jobTask.job.OnThreadComplete, takeData);
                }
                node = next;
            }
            return vAsync.Count > 0;
        }
        //-----------------------------------------------------
        protected abstract void OnAddJob(int job, IUserData userData = null);
        protected abstract void OnAddThread(int job, int millSleep = 500, IUserData userData = null);
    }
}
