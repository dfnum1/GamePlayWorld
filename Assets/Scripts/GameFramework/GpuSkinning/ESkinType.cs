#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    public enum ESkinType
    {
        GpuArray = 0,
        //    GpuData,
        CpuData,
        None,
        Count = None,
    }
}
#endif