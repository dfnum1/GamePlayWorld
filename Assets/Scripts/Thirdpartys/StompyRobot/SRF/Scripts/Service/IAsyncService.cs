#if USE_REPORTVIEW
namespace SRF.Service
{
    public interface IAsyncService
    {
        bool IsLoaded { get; }
    }
}

#endif