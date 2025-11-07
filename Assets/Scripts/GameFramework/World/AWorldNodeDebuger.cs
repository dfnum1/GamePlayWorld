#if UNITY_EDITOR
/********************************************************************
生成日期:	2:14:2020 10:29
类    名: 	AWorldNodeDebuger
作    者:	HappLI
描    述:	世界节点调试
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public abstract class AWorldNodeDebuger : MonoBehaviour
    {
        public AWorldNode pNode;
        static System.Type ms_pDebugerType = null;
        public static System.Type GetDebugerType()
        {
            if (ms_pDebugerType != null) return ms_pDebugerType;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    System.Type tp = types[i];
                    if ( tp.IsSubclassOf( typeof( AWorldNodeDebuger)))
                    {
                        ms_pDebugerType = tp;
                        return ms_pDebugerType;
                    }
                }
            }
            return ms_pDebugerType;
        }
    }
}

#endif