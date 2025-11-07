#if USE_REPORTVIEW
namespace SRDebugger.Services
{
    using UnityEngine;

    public interface IDebugCameraService
    {
        Camera Camera { get; }
    }
}

#endif