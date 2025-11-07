/********************************************************************
生成日期:	10:7:2019   12:11
类    名: 	CircleLinkList
作    者:	HappLI
描    述:	自定义 循环链表
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
	public class CircleLinkList<T> : IUserData where T : IUserData
	{
        static CircleLinkList<T> ms_Header = null;
        static CircleLinkList<T> ms_Tailer = null;
        //------------------------------------------------------
        public static CircleLinkList<T> Header
        {
            get
            {
                return ms_Header;
            }
        }
        //------------------------------------------------------
        public static CircleLinkList<T> Tailer
        {
            get
            {
                return ms_Tailer;
            }
        }
        //------------------------------------------------------
        CircleLinkList<T> m_Prev = null;
        CircleLinkList<T> m_Next = null;
        bool m_IsInList = false;
        //------------------------------------------------------
        public CircleLinkList<T> Prev
        {
            get
            {
                return m_Prev;
            }
        }
        //------------------------------------------------------
        public CircleLinkList<T> Next
        {
            get
            {
                return m_Next;
            }
        }
        //------------------------------------------------------
        public bool IsInList
        {
            get
            {
                return m_IsInList;
            }
        }
        //------------------------------------------------------
        protected void Insert()
        {
            if (IsInList)
                return;

            if (ms_Header == null)
            {
                ms_Header = this;
            }

            if (ms_Tailer == null)
            {
                ms_Tailer = this;
            }
            else
            {
                ms_Tailer.m_Next = this;
                m_Prev = ms_Tailer;
                m_Next = ms_Header;
            }

            ms_Tailer = this;

            m_IsInList = true;

            OnInsert();
        }
        //------------------------------------------------------
        public static CircleLinkList<T> IndexAt(int index)
        {
            int count = 0;
            CircleLinkList<T> node = ms_Header;
            for (; node != null; node = node.Next, ++count)
            {
                if (count == index) return node;
            }
            return null;
        }
        //------------------------------------------------------
        public static CircleLinkList<T> IndexAt(int start, int stepCnt, bool bNext = true)
        {
            CircleLinkList<T> node = IndexAt(start);
            if (node == null) return null;

            if (bNext)
            {
                int count = 0;
                for (; node != null; node = node.Next, ++count)
                {
                    if (count == stepCnt) return node;
                }
            }
            else
            {
                int count = 0;
                for (; node != null; node = node.Prev, ++count)
                {
                    if (count == stepCnt) return node;
                }
            }
            return null;
        }
        //------------------------------------------------------
        void Remove()
        {
            if (!IsInList)
                return;

            OnRemove();

            if (Prev != null)
            {
                Prev.m_Next = Next;
            }

            if (Next != null)
            {
                Next.m_Prev = Prev;
            }

            if (ms_Header == this)
            {
                ms_Header = m_Next;
            }

            if (ms_Tailer == this)
            {
                ms_Tailer = m_Prev;
            }

            m_Prev = null;
            m_Next = null;
            m_IsInList = false;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            OnDestroy();
            Remove();
        }
        //------------------------------------------------------
        protected virtual void OnDestroy() { }
        protected virtual void OnRemove() { }
        protected virtual void OnInsert() { }
    }
}