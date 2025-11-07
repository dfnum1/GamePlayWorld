/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	Pools
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;

namespace Framework.Plugin.AT
{
    public class Pools<T> where T : new()
    {
        private List<T> m_vPools = null;
        int m_nCapacity = 0;
        //------------------------------------------------------
        public Pools(int nCapacity)
        {
            m_nCapacity = nCapacity;
            m_vPools = new List<T>(m_nCapacity);
        }
        //------------------------------------------------------
        ~Pools()
        {
            m_vPools = null;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_vPools.Clear();
        }
        //------------------------------------------------------
        public void Recycle(T pAT)
        {
            if (m_vPools.Count < m_nCapacity)
                m_vPools.Add(pAT);
        }
        //------------------------------------------------------
        public T New()
        {
            if(m_vPools.Count>0)
            {
                T pNode = m_vPools[0];
                m_vPools.RemoveAt(0);
                return pNode;
            }
            return new T();
        }
    }
}