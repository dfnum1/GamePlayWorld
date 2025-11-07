/********************************************************************
生成日期:	10:7:2019   12:11
类    名: 	Singleton
作    者:	HappLI
描    述:	全局单例
*********************************************************************/

using System.Threading;

namespace Framework.Base
{
    public abstract class Singleton<T> where T : new()
    {
        protected static T sm_pInstance;

        static int sm_pLock = 0;
        //------------------------------------------------------
        public static T getInstance()
        {
            if (sm_pInstance == null)
            {
                Interlocked.Increment(ref sm_pLock);
                {
                    if(sm_pInstance == null)
                        sm_pInstance = new T();
                }
                Interlocked.Decrement(ref sm_pLock);
                return sm_pInstance;
            }

            return sm_pInstance;
        }
    }
}