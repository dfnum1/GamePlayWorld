/********************************************************************
生成日期:	3:10:2022  15:03
类    名: 	Atomic
作    者:	HappLI
描    述:	原子锁
*********************************************************************/
using System.Threading;
namespace Framework.Module
{
    public struct Atomic : System.IDisposable
    {
        private int m_Lock;

        public Atomic(int flag=0)
        {
            m_Lock = flag;
            Interlocked.Increment(ref m_Lock);
        }
        public void Dispose()
        {
            Interlocked.Decrement(ref m_Lock);
        }
    }
}
