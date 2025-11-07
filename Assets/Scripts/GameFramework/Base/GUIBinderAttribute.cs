using System;

namespace Framework.Base
{
    public class BindSlotGUIAttribute : Attribute
    {
        public BindSlotGUIAttribute()
        {

        }
    }
    public class ObjPathFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
#endif
        public ObjPathFieldAttribute(string fieldName)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
#endif
        }
    }
    public class SelectFileGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string root;
        public string subRoot;
        public string extend="*.*";
        public bool includeExtend = true;
#endif
        public SelectFileGUIAttribute()
        {
#if UNITY_EDITOR
            this.root = null;
#endif
        }
        public SelectFileGUIAttribute(string root, string subRoot = "", string extend = null, bool includeExtend = true)
        {
#if UNITY_EDITOR
            this.root = root;
            this.subRoot = subRoot;
            this.extend = extend;
            this.includeExtend = includeExtend;
#endif
        }
    }

    public class SelectDirGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string root;
#endif
        public SelectDirGUIAttribute()
        {
#if UNITY_EDITOR
            this.root = null;
#endif
        }
        public SelectDirGUIAttribute(string root)
        {
#if UNITY_EDITOR
            this.root = root;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = true)]
    public class BitViewGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string displayName;
        public int offset;
#endif
        public BitViewGUIAttribute(string displayName, int offset )
        {
#if UNITY_EDITOR
            this.displayName = displayName;
            this.offset = offset;
#endif
        }
    }

    public class StringViewPluginAttribute : Attribute
    {
#if UNITY_EDITOR
        public string userPlugin;
        public System.Type alignType;
#endif
        public StringViewPluginAttribute(string userPlugin, System.Type alignType = null)
        {
#if UNITY_EDITOR
            this.userPlugin = userPlugin;
            this.alignType = alignType;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StringViewGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        private System.Type m_bindType;
        public string bindTypeName;
        public int order = 0;
#endif
        public StringViewGUIAttribute(System.Type type, int order =0)
        {
#if UNITY_EDITOR
            m_bindType = type;
            bindTypeName = null;
            this.order = order;
#endif
        }
        public StringViewGUIAttribute(string typeName, int order =0)
        {
#if UNITY_EDITOR
            m_bindType = null;
            bindTypeName = typeName;
            this.order = order;
#endif
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public System.Type GetBindType()
        {
            if (m_bindType != null) return m_bindType;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(bindTypeName))
                this.m_bindType = ED.EditorUtil.GetTypeByName(bindTypeName);
#endif
            return m_bindType;
        }
#endif
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StateStringViewGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strField;
        public string strValue;
        public string strType;
        System.Type bindType;
#endif
        public StateStringViewGUIAttribute(string strField, string strValue, System.Type type)
        {
#if UNITY_EDITOR
            this.strField = strField;
            this.strValue = strValue;
            bindType = type;
            this.strType = null;
#endif
        }
        //------------------------------------------------------
        public StateStringViewGUIAttribute(string strField, string strValue, string type)
        {
#if UNITY_EDITOR
            this.strField = strField;
            this.strValue = strValue;
            bindType = null;
            this.strType = type;
#endif
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public System.Type GetBindType()
        {
            if (bindType != null) return bindType;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(strType))
                this.bindType = ED.EditorUtil.GetTypeByName(strType);
#endif
            return bindType;
        }
#endif
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class NoListHeaderGUIAttribute : Attribute
    {
        public NoListHeaderGUIAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class DefaultValueAttribute : Attribute
    {
#if UNITY_EDITOR
        public string strValue;
#endif
        public DefaultValueAttribute(System.Object defValue)
        {
#if UNITY_EDITOR
            this.strValue = defValue.ToString();
#endif
        }

#if UNITY_EDITOR
        //-----------------------------------------------------
        public T ToValue<T>(T defVal = default)
        {
            try
            {
                if (string.IsNullOrEmpty(strValue))
                    return defVal;

                Type targetType = typeof(T);

                // 先特殊处理枚举
                if (targetType.IsEnum)
                {
                    // 支持名称和数字
                    if (Enum.TryParse(targetType, strValue, true, out object enumVal))
                        return (T)enumVal;
                    // 尝试数字转枚举
                    if (int.TryParse(strValue, out int intVal))
                        return (T)Enum.ToObject(targetType, intVal);
                    return defVal;
                }

                // 支持所有常见数值类型
                if (targetType == typeof(byte))
                    return (T)(object)byte.Parse(strValue);
                if (targetType == typeof(bool))
                    return (T)(object)strValue.Equals("true", StringComparison.OrdinalIgnoreCase);
                if (targetType == typeof(short))
                    return (T)(object)short.Parse(strValue);
                if (targetType == typeof(ushort))
                    return (T)(object)ushort.Parse(strValue);
                if (targetType == typeof(int))
                    return (T)(object)int.Parse(strValue);
                if (targetType == typeof(uint))
                    return (T)(object)uint.Parse(strValue);
                if (targetType == typeof(long))
                    return (T)(object)long.Parse(strValue);
                if (targetType == typeof(ulong))
                    return (T)(object)ulong.Parse(strValue);
                if (targetType == typeof(float))
                    return (T)(object)float.Parse(strValue);
                if (targetType == typeof(double))
                    return (T)(object)double.Parse(strValue);
                if (targetType == typeof(decimal))
                    return (T)(object)decimal.Parse(strValue);

                // 其它类型尝试通用转换
                return (T)Convert.ChangeType(strValue, targetType);
            }
            catch
            {
                return defVal;
            }
        }
#endif
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayNameByFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
        public string strDisplayName;
        public System.Collections.Generic.List<string> fieldValue = new System.Collections.Generic.List<string>();
#endif
        public DisplayNameByFieldAttribute(string fieldName, string fieldValue, string strDisplayName=null)
        {
#if UNITY_EDITOR
            this.strDisplayName = strDisplayName;
            this.fieldName = fieldName;
            this.fieldValue.Add(fieldValue.ToLower());
#endif
        }
        public DisplayNameByFieldAttribute(string fieldName, string[] fieldValue, string strDisplayName = null)
        {
#if UNITY_EDITOR
            this.strDisplayName = strDisplayName;
            this.fieldName = fieldName;
            if (fieldValue == null) return;
            for (int i = 0; i < fieldValue.Length; ++i)
                this.fieldValue.Add(fieldValue[i].ToLower());
#endif
        }
    }

    public class DisplayEnumGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string enumTypeName="";
        private System.Type enumType;

        public string strField = "";
        public string[] groups = null;
#endif
        public DisplayEnumGUIAttribute(System.Type enumType)
        {
#if UNITY_EDITOR
            this.enumType = enumType;
            this.enumTypeName = "";
#endif
        }
        public DisplayEnumGUIAttribute(System.Type enumType, string field, string strGroup)
        {
#if UNITY_EDITOR
            this.enumType = enumType;
            this.enumTypeName = "";
            this.strField = field;
            if (!string.IsNullOrEmpty(strGroup))
                this.groups = new string[1] { strGroup };
#endif
        }
        public DisplayEnumGUIAttribute(System.Type enumType, string field, string[] groups)
        {
#if UNITY_EDITOR
            this.enumType = enumType;
            this.strField = field;
            this.groups = groups;
            this.enumTypeName = "";
#endif
        }
        public DisplayEnumGUIAttribute(string enumType)
        {
#if UNITY_EDITOR
            this.enumTypeName = enumType;
#endif

        }
        public DisplayEnumGUIAttribute(string enumType, string field, string strGroup)
        {
#if UNITY_EDITOR
            this.enumTypeName = enumType;
            this.strField = field;
            if (!string.IsNullOrEmpty(strGroup))
                this.groups = new string[1] { strGroup };
#endif
        }
        public DisplayEnumGUIAttribute(string enumType, string field, string[] groups)
        {
#if UNITY_EDITOR
            this.enumTypeName = enumType;
            this.strField = field;
            this.groups = groups;
#endif
        }
#if UNITY_EDITOR
        public System.Type GetEnumType()
        {
            if (enumType != null) return enumType;
#if UNITY_EDITOR
            if(!string.IsNullOrEmpty(enumTypeName))
                this.enumType = ED.EditorUtil.GetTypeByName(enumTypeName);
#endif
            return enumType;
        }
#endif
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayTypeByFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string byTypeName;
        System.Type byType;
        public string fieldName;
        public string fieldValue;
        public bool bBit = false;
        public bool bEnumBitOffset = false;
#endif
        public DisplayTypeByFieldAttribute(string fieldName, string fieldValue, System.Type byType, bool bBit = false, bool bEnumBitOffset = false)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            this.fieldValue = fieldValue.ToLower();
            this.byType = byType;
            this.bBit = bBit;
            this.bEnumBitOffset = bEnumBitOffset;
#endif
        }
        public DisplayTypeByFieldAttribute(string fieldName, string fieldValue, string byType, bool bBit = false, bool bEnumBitOffset = false)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            this.fieldValue = fieldValue.ToLower();
            this.byTypeName = byType;
            this.bBit = bBit;
            this.bEnumBitOffset = bEnumBitOffset;
#endif
        }
#if UNITY_EDITOR
        //-------------------------------------------------
        public System.Type GetDisType()
        {
            if (byType != null) return byType;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(byTypeName))
                this.byType = ED.EditorUtil.GetTypeByName(byTypeName);
#endif
            return byType;
        }
#endif
    }

    public class DisplayEnumBitGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public string enumTypeName = "";
        private System.Type enumType;
        public bool bBitOffset = false;
#endif
        public DisplayEnumBitGUIAttribute(System.Type enumType, bool bBitOffset = false)
        {
#if UNITY_EDITOR
            this.enumTypeName = "";
            this.enumType = enumType;
            this.bBitOffset = bBitOffset;
#endif
        }
        public DisplayEnumBitGUIAttribute(string enumType, bool bBitOffset = false)
        {
#if UNITY_EDITOR
            this.enumTypeName = enumType;
            this.bBitOffset = bBitOffset;
#endif
        }
#if UNITY_EDITOR
        public System.Type GetEnumType()
        {
            if (enumType != null) return enumType;
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(enumTypeName))
                this.enumType = ED.EditorUtil.GetTypeByName(enumTypeName);
#endif
            return enumType;
        }
#endif
    }

    public class ObjectTypeGUIAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Type objType;
        public bool bBitOffset = false;
#endif
        public ObjectTypeGUIAttribute(System.Type objType = null)
        {
#if UNITY_EDITOR
            this.objType = objType==null? typeof(UnityEngine.Object): objType;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayRenderLayerByFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
        public string fieldValue;
#endif
        public DisplayRenderLayerByFieldAttribute(string fieldName, string fieldValue)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            this.fieldValue = fieldValue.ToLower();
#endif
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StateGUIByFieldAttribute : Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
        public System.Collections.Generic.List<string> fieldValue = new System.Collections.Generic.List<string>();
        public bool IsContain;
#endif
        public StateGUIByFieldAttribute(string fieldName, string fieldValue, bool IsContain = true)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            this.fieldValue.Add(fieldValue.ToLower());
            this.IsContain = IsContain;
#endif
        }
        public StateGUIByFieldAttribute(string fieldName, string[] fieldValue, bool IsContain = true)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            if (fieldValue == null) return;
            for (int i = 0; i < fieldValue.Length; ++i)
            this.fieldValue.Add(fieldValue[i].ToLower());
            this.IsContain = IsContain;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StateGUIByBitAttribute : Attribute
    {
#if UNITY_EDITOR
        public string fieldName;
        public System.Collections.Generic.List<int> fieldValue = new System.Collections.Generic.List<int>();
#endif
        public StateGUIByBitAttribute(string fieldName, int fieldValue)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            this.fieldValue.Add(fieldValue);
#endif
        }
        public StateGUIByBitAttribute(string fieldName, int[] fieldValue)
        {
#if UNITY_EDITOR
            this.fieldName = fieldName;
            if (fieldValue == null) return;
            for (int i = 0; i < fieldValue.Length; ++i)
                this.fieldValue.Add(fieldValue[i]);
#endif
        }
    }


    public class StateGUIByTypeAttribute : Attribute
    {
#if UNITY_EDITOR
        public System.Collections.Generic.HashSet<Type> typeSets = new System.Collections.Generic.HashSet<Type>();
#endif
        public StateGUIByTypeAttribute(Type type)
        {
#if UNITY_EDITOR
            typeSets.Add(type);
#endif
        }
        public StateGUIByTypeAttribute(Type[] types)
        {
#if UNITY_EDITOR
            if (types == null) return;
            for (int i = 0; i < types.Length; ++i)
                this.typeSets.Add(types[i]);
#endif
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomCUIAttribute : Attribute
    {
        public CustomCUIAttribute()
        {

        }
    }
    [AttributeUsage(AttributeTargets.Enum)]
    public class PluginUnFilterAttribute : Attribute
    {
        public PluginUnFilterAttribute()
        {
        }
    }
}