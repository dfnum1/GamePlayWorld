/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	RecycleAble
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.Core
{
    //-------------------------------------------------
    public  abstract class RecycleAble<T> where T : new()
    {
        protected ObjectPool<T> m_vRecycle = null;
        //------------------------------------------------------
        public RecycleAble()
        {
            m_vRecycle = new ObjectPool<T>(16);
        }
        //------------------------------------------------------
        public RecycleAble(int capacity)
        {
            m_vRecycle = new ObjectPool<T>(capacity);
        }
        //------------------------------------------------------
        public virtual void Clear()
        {
            m_vRecycle.Clear();
        }
        //------------------------------------------------------
        protected virtual T Malloc()
        {
            return m_vRecycle.Get();
        }
        //------------------------------------------------------
        protected virtual void Free(T obj)
        {
            m_vRecycle.Release(obj);
        }
    }
}
