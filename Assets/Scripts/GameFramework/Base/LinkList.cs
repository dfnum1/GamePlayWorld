/********************************************************************
生成日期:	10:7:2019   12:11
类    名: 	LinkList
作    者:	HappLI
描    述:	自定义 链表
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
	public class LinkList<T> : IUserData where T : IUserData
	{
		static LinkList<T>			ms_Header = null;
		static LinkList<T>			ms_Tailer = null;
        //------------------------------------------------------
        public static LinkList<T>	Header
		{
			get
			{
				return ms_Header;
			}
		}
        //------------------------------------------------------
        public static LinkList<T>	Tailer
		{
			get
			{
				return ms_Tailer;
			}
		}
		//------------------------------------------------------
		LinkList<T>				m_Prev		= null;
		LinkList<T>				m_Next		= null;
		bool								m_IsInList	= false;
        //------------------------------------------------------
        public LinkList<T>			Prev
		{
			get
			{
				return m_Prev;
			}
		}
        //------------------------------------------------------
        public LinkList<T>			Next
		{
			get
			{
				return m_Next;
			}
		}
        //------------------------------------------------------
        public bool							IsInList
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
			}
			
			ms_Tailer = this;

			m_IsInList = true;

			OnInsert();
		}
        //------------------------------------------------------
		public static LinkList<T> IndexAt(int index)
		{
			int count = 0;
			LinkList<T> node = ms_Header;
            for (; node != null; node = node.Next,++count)
            {
				if (count == index) return node;
            }
			return null;
        }
        //------------------------------------------------------
        public static LinkList<T> IndexAt(int start, int stepCnt, bool bNext = true)
        {
			LinkList<T>  node = IndexAt(start);
			if (node == null) return null;

			if(bNext)
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