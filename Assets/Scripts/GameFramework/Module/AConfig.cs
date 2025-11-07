/********************************************************************
生成日期:	3:10:2022  15:03
类    名: 	AConfig
作    者:	HappLI
描    述:	配置基础Config
*********************************************************************/
namespace Framework.Core
{
    public interface AConfig
    {
        void Apply();
        void Init();
#if UNITY_EDITOR
        void OnInspector(object param = null);
#endif
    }
}
