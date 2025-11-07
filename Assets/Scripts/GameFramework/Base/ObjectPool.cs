/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Pool
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Framework.Core
{
    public enum EPoolType
    {
        Discard,
        GrowUp,
    }
    //-------------------------------------------------
    public class ObjectSetPool<T> where T : new()
    {
        private readonly HashSet<T> m_Stack = null;
        private int m_nCapcity = 0;
        private EPoolType m_eDiscard = EPoolType.Discard;

        public ObjectSetPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new HashSet<T>();
        }

        public T Get()
        {
            if (m_Stack.Count == 0)
            {
                return new T();
            }
            else
            {
                //var iter = m_Stack.GetEnumerator();
                //iter.MoveNext();
                //element = iter.Current;
                //m_Stack.Remove(element);
                foreach (var db in m_Stack)
                {
                    T element = db;
                    m_Stack.Remove(db);
                    return element;
                }
            }
            // never call this
            return default(T);
        }

        public void Release(T element)
        {
            if (m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity) return;
            }
            if (m_Stack.Contains(element)) return;
            m_Stack.Add(element);
        }
    }
    //-------------------------------------------------
    public class AtomicObjectSetPool<T> where T : new()
    {
        private readonly HashSet<T> m_Stack = null;
        private int m_nCapcity = 0;
        private EPoolType m_eDiscard = EPoolType.Discard;
        int m_nLock = 0;
        public AtomicObjectSetPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nLock = 0;
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new HashSet<T>();
        }

        public T Get()
        {
            Interlocked.Increment(ref m_nLock);
            if (m_Stack.Count == 0)
            {
                return new T();
            }
            else
            {
                //var iter = m_Stack.GetEnumerator();
                //iter.MoveNext();
                //element = iter.Current;
                //m_Stack.Remove(element);
                foreach (var db in m_Stack)
                {
                    T element = db;
                    m_Stack.Remove(db);
                    return element;
                }
            }
            Interlocked.Decrement(ref m_nLock);
            // never call this
            return default(T);
        }

        public void Release(T element)
        {
            Interlocked.Increment(ref m_nLock);
            if (m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity)
                {
                    Interlocked.Decrement(ref m_nLock);
                    return;
                }
            }
            if (m_Stack.Contains(element))
            {
                Interlocked.Decrement(ref m_nLock);
                return;
            }
            m_Stack.Add(element);
            Interlocked.Decrement(ref m_nLock);
        }
    }
    //-------------------------------------------------
    public class ObjectStackPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = null;
        private int m_nCapcity = 0;
        private EPoolType m_eDiscard = EPoolType.Discard;

        public ObjectStackPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new Stack<T>(capacity);
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element, bool bCheckContain = false)
        {
            if (m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity) return;
            }
            if (bCheckContain && m_Stack.Contains(element)) return;
            m_Stack.Push(element);
        }
    }
    //-------------------------------------------------
    public class AtomicObjectStackPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = null;
        private int m_nCapcity = 0;
        private EPoolType m_eDiscard = EPoolType.Discard;

        private int m_nLock = 0;
        public AtomicObjectStackPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nLock = 0;
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new Stack<T>(capacity);
        }

        public T Get()
        {
            Interlocked.Increment(ref m_nLock);
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack.Pop();
            }
            Interlocked.Decrement(ref m_nLock);
            return element;
        }

        public void Release(T element, bool bCheckContain = false)
        {
            Interlocked.Increment(ref m_nLock);
            if (m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity)
                {
                    Interlocked.Decrement(ref m_nLock);
                    return;
                }
            }
            else if (m_eDiscard == EPoolType.GrowUp)
            {
                m_nCapcity *= 2;
            }
            if (bCheckContain && m_Stack.Contains(element))
            {
                Interlocked.Decrement(ref m_nLock);
                return;
            }
            m_Stack.Push(element);
            Interlocked.Decrement(ref m_nLock);
        }
    }
    //-------------------------------------------------
    public  class ObjectPool<T> where T :  new()
    {
        private readonly List<T> m_Stack = null;
        private int m_nCapcity  =0;
        private EPoolType m_eDiscard = EPoolType.Discard;

        public ObjectPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new List<T>(capacity);
        }

        public ObjectPool()
        {
        }

        public int poolCount
        {
            get { return m_Stack.Count; }
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack[0];
                m_Stack.RemoveAt(0);
            }
            return element;
        }

        public void Release(T element, bool bCheckContain = false)
        {
            if(m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity) return;
            }
            else if (m_eDiscard == EPoolType.GrowUp)
            {
                if (m_Stack.Count >= m_nCapcity)
                {
                    m_nCapcity *= 2;
                    m_Stack.Capacity = m_nCapcity;
                }
            }
            if (bCheckContain && m_Stack.Contains(element)) return;
            m_Stack.Add(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
        }
    }
    //-------------------------------------------------
    public class AtomicObjectPool<T> where T : new()
    {
        private readonly List<T> m_Stack = null;
        private int m_nCapcity = 0;
        private EPoolType m_eDiscard = EPoolType.Discard;
        int m_nLock = 0;
        public AtomicObjectPool(int capacity, EPoolType bDiscard = EPoolType.Discard)
        {
            m_nCapcity = capacity;
            m_eDiscard = bDiscard;
            m_Stack = new List<T>(capacity);
            m_nLock = 0;
        }

        public AtomicObjectPool()
        {
        }

        public T Get()
        {
            Interlocked.Increment(ref m_nLock);
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack[0];
                m_Stack.RemoveAt(0);
            }
            Interlocked.Decrement(ref m_nLock);
            return element;
        }

        public void Release(T element, bool bCheckContain = false)
        {
            Interlocked.Increment(ref m_nLock);
            if (m_eDiscard == EPoolType.Discard)
            {
                if (m_Stack.Count >= m_nCapcity)
                {
                    Interlocked.Decrement(ref m_nLock);
                    return;
                }
            }
            else if (m_eDiscard == EPoolType.GrowUp)
            {
                if (m_Stack.Count >= m_nCapcity)
                {
                    m_nCapcity *= 2;
                    m_Stack.Capacity = m_nCapcity;
                }
            }
            if (bCheckContain && m_Stack.Contains(element))
            {
                Interlocked.Decrement(ref m_nLock);
                return;
            }
            m_Stack.Add(element);
            Interlocked.Decrement(ref m_nLock);
        }

        public void Clear()
        {
            Interlocked.Increment(ref m_nLock);
            m_Stack.Clear();
            Interlocked.Decrement(ref m_nLock);
        }
    }
}
