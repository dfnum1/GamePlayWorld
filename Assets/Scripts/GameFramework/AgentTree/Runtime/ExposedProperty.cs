/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	ExposedProperty
作    者:	HappLI
描    述:	
*********************************************************************/
namespace Framework.Plugin.AT
{
    [System.Serializable]
    public class ExposedProperty
    {
        public static ExposedProperty CreateInstance()
        {
            return new ExposedProperty();
        }

        public string PropertyName = "New String";
        public string PropertyValue = "New Value";
    }
}