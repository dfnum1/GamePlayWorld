
namespace Framework.Data
{
    public class FieldMapTableAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public System.Type table;
        public string strMapFunc;
        public string strMapField;
        public string strMapToField;
#endif
        public FieldMapTableAttribute(string strMapFunc = "", System.Type table = null, string strMapField = "", string strMapToField = "")
        {
#if UNITY_EDITOR
            this.strMapFunc = strMapFunc;
            this.table = table;
            this.strMapField = strMapField;
            this.strMapToField = strMapToField;
#endif
        }
    }
    public class DataBinderTypeAttribute : System.Attribute
    {
#if UNITY_EDITOR

        public string strConfigName = "";
        public string strMainKeyField = "nID";
        public string strMainKeyType = "";
        public string DataField = "datas";
#endif
        public DataBinderTypeAttribute(string strConfigName, string strMainKeyType = "ushort", string strMainKeyField = "nID", string DataField = "datas")
        {
#if UNITY_EDITOR
            this.strConfigName = strConfigName;
            this.strMainKeyField = strMainKeyField;
            this.strMainKeyType = strMainKeyType;
            this.DataField = DataField;
#endif
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum, AllowMultiple = true)]
    public class FormViewBinderAttribute : System.Attribute
    {
#if UNITY_EDITOR
        private System.Type bindTable;
        public string tableName = "";
        public string Field = "";
        public string KeyValue = "";
#endif
        public FormViewBinderAttribute(System.Type type, string field = "nID", string KeyValue = "")
        {
#if UNITY_EDITOR
            bindTable = type;
            tableName = "";
            this.Field = field;
            this.KeyValue = KeyValue;
#endif
        }

        public FormViewBinderAttribute(string tableName, string field = "nID", string KeyValue = "")
        {
#if UNITY_EDITOR
            this.tableName = tableName;
            this.Field = field;
            this.KeyValue = KeyValue;
#endif
        }
#if UNITY_EDITOR
        public System.Type GetTableType()
        {
            if (bindTable != null) return bindTable;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(tableName))
            {
                bindTable = ED.DataEditorUtil.GetTableType(tableName);
            }
#endif
            return bindTable;
        }
#endif
    }

    public class FormFieldDisplayAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
#endif
        public FormFieldDisplayAttribute(string fieldName)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
#endif
        }
    }
    public class FormSelectFieldAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
#endif
        public FormSelectFieldAttribute(string fieldName)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
#endif
        }
    }
    public class BinaryDiscardAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public int version;
#endif
        public BinaryDiscardAttribute(int version)
        {
#if UNITY_EDITOR
            this.version = version;
#endif
        }
    }

    public class BinaryCodeMarcosAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string marcos;
#endif
        public BinaryCodeMarcosAttribute(string marcos)
        {
#if UNITY_EDITOR
            this.marcos = marcos;
#endif
        }
    }

    public class BinaryCodeAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public int version;
        public string savePath = "";
#endif
        public BinaryCodeAttribute(int version, string savePath = "")
        {
#if UNITY_EDITOR
            this.version = version;
            this.savePath = savePath;
#endif
        }
    }

    public class BinaryFieldVersionAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public int version;
#endif
        public BinaryFieldVersionAttribute(int version)
        {
#if UNITY_EDITOR
            this.version = version;
#endif
        }
    }

    public class BinaryUnServerAttribute : System.Attribute
    {
        public BinaryUnServerAttribute()
        {

        }
    }

    public class BinaryServerCodeAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string savePath = "";
#endif
        public BinaryServerCodeAttribute(string savePath = "")
        {
#if UNITY_EDITOR
            this.savePath = savePath;
#endif
        }
    }

    public class TableMappingAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string strSymbol = "";
#endif
        public TableMappingAttribute(string symbol = "")
        {
#if UNITY_EDITOR
            this.strSymbol = symbol;
#endif
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PublishRefreshAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string refreshMethod;
#endif
        public PublishRefreshAttribute(string refreshMethod)
        {
#if UNITY_EDITOR
            this.refreshMethod = refreshMethod;
#endif
        }
    }
}