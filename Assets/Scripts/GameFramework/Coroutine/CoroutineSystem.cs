/********************************************************************
生成日期:	5:11:2022  14:36
类    名: 	CoroutineSystem
作    者:	HappLI
描    述:	
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
#if USE_SERVER
using UnityEngine;
#else
using UnityEngine;
#endif

namespace Framework.Core
{
    public class CoroutineSystem : AModule
    {
        struct Task
        {
            public CoroutineSystem m_pSystem;
            public IEnumerator coroutine;
            public bool Running { get; set; }
            public bool Paused { get; set; }
            public IEnumerator DoCoroutine()
            {
                IEnumerator task = coroutine;
                while (Running)
                {
                    if (Paused)
                        yield return null;
                    else
                    {
                        if (task != null && task.MoveNext())
                            yield return task.Current;
                        else
                            Running = false;
                    }
                }
                m_pSystem.m_vCortuines.Remove(coroutine);
            }
        }

        HashSet<IEnumerator> m_vCortuines = new HashSet<IEnumerator>();
        public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        static Dictionary<int, WaitForSeconds> ms_WaiteSendond = null;
        public static WaitForSeconds WaitForSecond(float second)
        {
            int miSecond = ((int)second * 1000);
            WaitForSeconds wait;
            if(ms_WaiteSendond!=null)
            {
                if (ms_WaiteSendond.TryGetValue(miSecond, out wait))
                    return wait;
            }
            else
            {
                ms_WaiteSendond = new Dictionary<int, WaitForSeconds>();
            }

            wait = new WaitForSeconds(second);
            ms_WaiteSendond[miSecond] = wait;
            return wait;
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            foreach (var db in m_vCortuines)
            {
                m_pFramework.EndCoroutine(db);
            }
            m_vCortuines.Clear();
        }
        //------------------------------------------------------
        public bool Start(IEnumerator cortuine)
        {
            if (m_vCortuines.Contains(cortuine)) return true;
            Task task = new Task();
            task.Running = true;
            task.Paused = false;
            task.coroutine = cortuine;
            task.m_pSystem = this;
            m_pFramework.BeginCoroutine(cortuine);
            m_vCortuines.Add(cortuine);
            return true;
        }
        //------------------------------------------------------
        public static bool StartCoroutine(IEnumerator cortuine, AFramework pFrameWork=null)
        {
            if (pFrameWork == null)
                pFrameWork = AFramework.mainFramework;
            if (pFrameWork == null) return false;
            CoroutineSystem system = pFrameWork.coroutineSystem;
            if (system == null) return false;
            return system.Start(cortuine);
        }
    }
}
