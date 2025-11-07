using System;
namespace Framework.Data
{
    [System.Serializable]
    public struct IntKeyValue
    {
        public int key;
        public int value;
    }

    [System.Serializable]
    public struct KeyValueParam
    {
        public string key;
        public string value;
        public static KeyValueParam DEF = new KeyValueParam() { key = null, value = null };
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value);
            }
        }
        //------------------------------------------------------
        public KeyValueParam(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
#if UNITY_EDITOR
        public KeyValueParam OnInspector(System.Object param = null)
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            float backup = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 40;
            this.key = UnityEditor.EditorGUILayout.TextField("key", this.key);
            UnityEditor.EditorGUIUtility.labelWidth = 80;
            this.value = UnityEditor.EditorGUILayout.TextField(" = value", this.value);
            UnityEditor.EditorGUIUtility.labelWidth = backup;
            UnityEditor.EditorGUILayout.EndHorizontal();
            return this;
        }
        //------------------------------------------------------
        public void Write(ref Framework.Data.BinaryUtil write)
        {
            write.WriteString(key);
            write.WriteString(value);
        }
#endif
        //------------------------------------------------------
        public void Read(ref Framework.Data.BinaryUtil write)
        {
            key = write.ToString();
            value = write.ToString();
        }
    }
}
