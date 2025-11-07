#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	PostPassAttribute
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System;

namespace Framework.URP
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PostPassAttribute : Attribute
    {
        public EPostPassType posPassType;
        public int logicQueue;
        public PostPassAttribute(EPostPassType posPassType, int logicQueue)
        {
            this.logicQueue = logicQueue;
            this.posPassType = posPassType;
        }
    }
}
#endif