/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Vector3Track
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using Framework.Base;

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.Core
{
    public struct RunPoint
    {
        public FVector3 position;
        public FVector3 rotation;

        public EActionStateType actionState;
        public FFloat speedScale;
        public RunPoint(FVector3 pos, EActionStateType action = EActionStateType.None, float speedScale =1)
        {
            this.rotation = FVector3.zero;
            this.speedScale = (FFloat)speedScale;
            this.position = pos;
            this.actionState = action;
        }
    }
    public class Vector3Track
    {
        struct KeyPoint : IQuickSort<KeyPoint>
        {
            public FFloat time;
            public FVector3 position;
            public EActionStateType actionState;
            public int CompareTo(int type, KeyPoint other)
            {
                if (time == other.time) return 0;
                if (time < other.time) return -1;
                return 1;
            }
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }

        List<KeyPoint> m_Keys = new List<KeyPoint>();
        public void AddKeyPoint(FFloat fTime, FVector3 pos, EActionStateType actionType = EActionStateType.None, bool bSort = true)
        {
            m_Keys.Add(new KeyPoint() { time = fTime, position = pos, actionState = actionType });
            if(bSort) SortUtility.QuickSortUp<KeyPoint>(ref m_Keys, 0);
        }
        //------------------------------------------------------
        public void InsertKeyPoint(int index, FFloat fTime, FVector3 pos, EActionStateType actionType = EActionStateType.None)
        {
            if (index >= m_Keys.Count) AddKeyPoint(fTime, pos, actionType);
            else
            {
                if (index < 0) index = 0;
                for (int i =0; i < m_Keys.Count; ++i)
                {
                    if(i >= index)
                    {
                        KeyPoint kp = m_Keys[i];
                        kp.time += fTime;
                        m_Keys[i] = kp;
                    }
                }
                m_Keys.Insert(index, new KeyPoint() { time = 0, position = pos, actionState = actionType });
            }
        }
        //------------------------------------------------------
        public void AddKeyPoint(FFloat fTime, RunPoint runPoint)
        {
            m_Keys.Add(new KeyPoint() { time = fTime, position = runPoint.position, actionState = runPoint.actionState });
            SortUtility.QuickSortUp<KeyPoint>(ref m_Keys, 0);
        }
        //------------------------------------------------------
        public void DelKeyPoint(FFloat fTime)
        {
            for(int i = 0; i < m_Keys.Count; ++i)
            {
#if USE_FIXEDMATH
                if(FMath.Abs(m_Keys[i].time-fTime) <= 0.001f)
#else
                if (System.Math.Abs(m_Keys[i].time - fTime) <= 0.001f)
#endif
                {
                    m_Keys.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_Keys.Clear();
        }
        //------------------------------------------------------
        public void Offset(FVector3 offset)
        {
            if (m_Keys == null) return;
            KeyPoint key;
            for(int i =0; i < m_Keys.Count; ++i)
            {
                key = m_Keys[i];
                key.position += offset;
                m_Keys[i] = key;
            }
        }
        //------------------------------------------------------
        public bool Evaluate(FFloat fTime, out FVector3 retPos, out EActionStateType actionType)
        {
            actionType = EActionStateType.None;
            retPos = FVector3.zero;
            FVector3 toPos;
            return Evaluate(fTime, out retPos, out toPos, out actionType);
        }
        //------------------------------------------------------
        public bool Evaluate(FFloat fTime, out FVector3 retPos, out FVector3 toPos, out EActionStateType actionType)
        {
            actionType = EActionStateType.None;
            retPos = FVector3.zero;
            toPos = FVector3.zero;
            if (m_Keys.Count <= 0) return false;
            if (fTime <= m_Keys[0].time)
            {
                actionType = m_Keys[0].actionState;
                toPos = retPos = m_Keys[0].position;
                return true;
            }
            if (fTime >= m_Keys[m_Keys.Count - 1].time)
            {
                actionType = m_Keys[m_Keys.Count - 1].actionState;
                toPos = retPos = m_Keys[m_Keys.Count - 1].position;
                m_Keys.Clear();
                return true;
            }

            int __len = m_Keys.Count;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (fTime < m_Keys[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(m_Keys.Count - 1, __first);

            if (lhs < 0 || lhs >= m_Keys.Count || rhs < 0 || rhs >= m_Keys.Count)
                return false;

            KeyPoint lhsKey = m_Keys[lhs];
            KeyPoint rhsKey = m_Keys[rhs];

            FFloat dx = rhsKey.time - lhsKey.time;
            Vector3 m1 = Vector3.zero, m2 = Vector3.zero;
            FFloat t;
            if (dx != 0f)
            {
                t = (fTime - lhsKey.time) / dx;
            }
            else
                t = 0;

            retPos = lhsKey.position * (1 - t) + rhsKey.position * t;
            toPos = rhsKey.position;
            if (t <= 0.98f) actionType = lhsKey.actionState;
            else actionType = rhsKey.actionState;

            return true;
        }
#if UNITY_EDITOR
        public void DrawDebug()
        {
            if (m_Keys == null || m_Keys.Count <= 1) return;
            Vector3 position = m_Keys[0].position;
            Color color = Handles.color;
            Handles.color = Color.yellow;
            for (int i = 1; i < m_Keys.Count; ++i)
            {
                Handles.CubeHandleCap(0, m_Keys[i].position, Quaternion.identity, 0.1f, EventType.Repaint);
                Handles.DrawLine(position, m_Keys[i].position);
                position = m_Keys[i].position;
            }
            Handles.color = color;
        }
#endif
    }

    public class PosRotTrack
    {
        struct KeyPoint : IQuickSort<KeyPoint>
        {
            public FFloat time;
            public FVector3 position;
            public FVector3 rotation;
            public EActionStateType actionState;
            public int CompareTo(int type, KeyPoint other)
            {
                if (time == other.time) return 0;
                if (time < other.time) return -1;
                return 1;
            }
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }

        List<KeyPoint> m_Keys = new List<KeyPoint>();
        public void AddKeyPoint(FFloat fTime, FVector3 pos, FVector3 rot, EActionStateType actionType = EActionStateType.None, bool bSort = true)
        {
            m_Keys.Add(new KeyPoint() { time = fTime, position = pos, rotation = rot, actionState = actionType });
            if (bSort) SortUtility.QuickSortUp<KeyPoint>(ref m_Keys, 0);
        }
        //------------------------------------------------------
        public void InsertKeyPoint(int index, FFloat fTime, FVector3 pos, FVector3 rot, EActionStateType actionType = EActionStateType.None)
        {
            if (index >= m_Keys.Count) AddKeyPoint(fTime, pos, rot, actionType);
            else
            {
                if (index < 0) index = 0;
                for (int i = 0; i < m_Keys.Count; ++i)
                {
                    if (i >= index)
                    {
                        KeyPoint kp = m_Keys[i];
                        kp.time += fTime;
                        m_Keys[i] = kp;
                    }
                }
                m_Keys.Insert(index, new KeyPoint() { time = 0, position = pos, rotation = rot, actionState = actionType });
            }
        }
        //------------------------------------------------------
        public void AddKeyPoint(FFloat fTime, RunPoint runPoint)
        {
            m_Keys.Add(new KeyPoint() { time = fTime, position = runPoint.position, rotation = runPoint.rotation, actionState = runPoint.actionState });
            SortUtility.QuickSortUp<KeyPoint>(ref m_Keys, 0);
        }
        //------------------------------------------------------
        public void DelKeyPoint(FFloat fTime)
        {
            for (int i = 0; i < m_Keys.Count; ++i)
            {
#if USE_FIXEDMATH
                if (FMath.Abs(m_Keys[i].time - fTime) <= 0.001f)
#else
                if (System.Math.Abs(m_Keys[i].time - fTime) <= 0.001f)
#endif
                {
                    m_Keys.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_Keys.Clear();
        }
        //------------------------------------------------------
        public void Offset(FVector3 offset)
        {
            if (m_Keys == null) return;
            KeyPoint key;
            for (int i = 0; i < m_Keys.Count; ++i)
            {
                key = m_Keys[i];
                key.position += offset;
                m_Keys[i] = key;
            }
        }
        //------------------------------------------------------
        public bool Evaluate(FFloat fTime, out FVector3 retPos, out FVector3 retRotation, out EActionStateType actionType)
        {
            actionType = EActionStateType.None;
            retPos = FVector3.zero;
            retRotation = FVector3.zero;
            if (m_Keys.Count <= 0) return false;
            if (fTime <= m_Keys[0].time)
            {
                actionType = m_Keys[0].actionState;
                retPos = m_Keys[0].position;
                retRotation = m_Keys[0].rotation;
                return true;
            }
            if (fTime >= m_Keys[m_Keys.Count - 1].time)
            {
                actionType = m_Keys[m_Keys.Count - 1].actionState;
                retPos = m_Keys[m_Keys.Count - 1].position;
                retRotation = m_Keys[m_Keys.Count - 1].rotation;
                m_Keys.Clear();
                return true;
            }

            int __len = m_Keys.Count;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (fTime < m_Keys[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(m_Keys.Count - 1, __first);

            if (lhs < 0 || lhs >= m_Keys.Count || rhs < 0 || rhs >= m_Keys.Count)
                return false;

            KeyPoint lhsKey = m_Keys[lhs];
            KeyPoint rhsKey = m_Keys[rhs];

            FFloat dx = rhsKey.time - lhsKey.time;
            Vector3 m1 = Vector3.zero, m2 = Vector3.zero;
            FFloat t;
            if (dx != 0f)
            {
                t = (fTime - lhsKey.time) / dx;
            }
            else
                t = 0;

            retPos = lhsKey.position * (1 - t) + rhsKey.position * t;
            retRotation = lhsKey.rotation * (1 - t) + rhsKey.rotation * t;

            if (t <= 0.98f) actionType = lhsKey.actionState;
            else actionType = rhsKey.actionState;

            return true;
        }
#if UNITY_EDITOR
        public void DrawDebug()
        {
            if (m_Keys == null || m_Keys.Count <= 1) return;
            Vector3 position = m_Keys[0].position;
            Color color = Handles.color;
            Handles.color = Color.yellow;
            for (int i = 1; i < m_Keys.Count; ++i)
            {
                Handles.CubeHandleCap(0, m_Keys[i].position, Quaternion.identity, 0.1f, EventType.Repaint);
                Handles.DrawLine(position, m_Keys[i].position);
                Handles.ArrowHandleCap(0, position, Quaternion.Euler(m_Keys[i].rotation), 0.1f, EventType.Repaint);
                position = m_Keys[i].position;
            }
            Handles.color = color;
        }
#endif
    }
}

