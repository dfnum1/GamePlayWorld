/********************************************************************
生成日期:		06:30:2025
类    名: 	VariableList
作    者:	HappLI
描    述:	变量列表存储类
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;


namespace Framework.Guide
{
    public enum EVariableType : byte
    {
        eInt,
        eString,
    }
    //-----------------------------------------------------
    //VariablePort
    //-----------------------------------------------------
    public class VariablePort
    {
        public struct PortData
        {
            public EVariableType type;
            public int value;
            public string strValue;
            public PortData(byte index, int value)
            {
                type = EVariableType.eInt;
                this.value = value;
                strValue = null;
            }
            public PortData(byte index, string value)
            {
                type = EVariableType.eString;
                this.strValue = value;
                this.value = 0;
            }
        }
        private Dictionary<byte, PortData> m_vPorts = null;
        private byte m_nMaxPortIndex = 0;
        public VariablePort() { }

        public void Clear()
        {
            m_nMaxPortIndex = 0;
            if (m_vPorts != null) m_vPorts.Clear();
        }

        public Dictionary<byte, PortData> GetPorts()
        {
            return m_vPorts;
        }

        public void Build(VariableList list)
        {
            if (list == null) return;
            if (m_vPorts == null || m_vPorts.Count == 0) return;
            for(int i =0; i < m_nMaxPortIndex; ++i)
            {
                if (m_vPorts.TryGetValue((byte)i, out var portData))
                {
                    if (portData.type == EVariableType.eInt)
                    {
                        list.AddInt(portData.value);
                    }
                    else if (portData.type == EVariableType.eString)
                    {
                        list.AddString(portData.strValue);
                    }
                }
                else
                    list.AddInt(0);
            }
        }

        public void SetInt(byte portIndex, int value)
        {
            m_nMaxPortIndex = (byte)Mathf.Max(m_nMaxPortIndex, portIndex);
            if (m_vPorts == null) m_vPorts = new Dictionary<byte, PortData>(2);
            if(m_vPorts.TryGetValue(portIndex, out var portData))
            {
                portData.value = value;
            }
            else
            {
                portData = new PortData();
                portData.type = EVariableType.eInt;
                portData.value = value;
            }
            m_vPorts[portIndex] = portData;
        }

        public void SetString(byte portIndex, string value)
        {
            m_nMaxPortIndex = (byte)Mathf.Max(m_nMaxPortIndex, portIndex);
            if (m_vPorts == null) m_vPorts = new Dictionary<byte, PortData>(2);
            if (m_vPorts.TryGetValue(portIndex, out var portData))
            {
                portData.strValue = value;
            }
            else
            {
                portData = new PortData();
                portData.strValue = value;
            }
            portData.type = EVariableType.eInt;
            m_vPorts[portIndex] = portData;
        }
    }
    //-----------------------------------------------------
    //VariableList
    //-----------------------------------------------------
    public class VariableList
    {
        struct TypeIndex
        {
            public EVariableType type;
            public byte index;
            public TypeIndex(EVariableType type, byte index)
            {
                this.type = type;
                this.index = index;
            }
        }
        List<int>           m_vInts = null;
        List<string>        m_vStrings = null;
        List<TypeIndex>     m_vTypes = null;
        byte                m_nCapacity = 2;
		static 	VariableList ms_TempCache = null;
        //-----------------------------------------------------
        internal VariableList()
        {

        }
        //-----------------------------------------------------
        public static VariableList Get(int capacity =2)
        {
			if(ms_TempCache == null) ms_TempCache = new VariableList();
            ms_TempCache.m_nCapacity = (byte)Mathf.Clamp(capacity, 1, 255);
			ms_TempCache.Clear();
            return ms_TempCache;
        }
        //-----------------------------------------------------
        public void Clear()
        {
            m_vInts?.Clear();
            m_vStrings?.Clear();
            m_vTypes?.Clear();
        }
        //-----------------------------------------------------
        public int GetVarCount()
        {
            if (m_vTypes == null) return 0;
            return m_vTypes.Count;
        }
        //-----------------------------------------------------
        public EVariableType GetVarType(int index)
        {
            if (index < 0 || m_vTypes == null || m_vTypes.Count == 0 || index >= m_vTypes.Count) return EVariableType.eInt;
            return m_vTypes[index].type;
        }
        //-----------------------------------------------------
        public void AddInt(int value)
        {
            if (m_vInts == null) m_vInts = new List<int>(m_nCapacity);
            if (m_vTypes == null) m_vTypes = new List<TypeIndex>(m_nCapacity);
            m_vTypes.Add(new TypeIndex(EVariableType.eInt, (byte)m_vInts.Count));
            m_vInts.Add(value);
        }
        //-----------------------------------------------------
        public void SetInt(int index, int value)
        {
            if (index >= 0 && m_vInts != null && m_vInts.Count > 0 && m_vTypes != null && m_vTypes.Count > 0 && index < m_vTypes.Count)
            {
                var type = m_vTypes[index];
                if (type.type != EVariableType.eInt)
                {
                    Debug.LogError($"VariableList: SetInt type mismatch, expected {EVariableType.eInt}, got {type}");
                }
                if (type.index < 0 || type.index >= m_vInts.Count)
                {
                    Debug.LogError($"VariableList: SetInt index out of range, index={type.index}, count={m_vInts.Count}");
                    return;
                }
                m_vInts[type.index] = value;
            }
        }
        //-----------------------------------------------------
        public int GetInt(int index, int defaultValue = 0)
        {
            if (index >= 0 && m_vInts != null && m_vInts.Count > 0 && m_vTypes != null && m_vTypes.Count > 0 && index < m_vTypes.Count)
            {
                var type = m_vTypes[index];
                if (type.type != EVariableType.eInt)
                {
                    Debug.LogError($"VariableList: GetInt type mismatch, expected {EVariableType.eInt}, got {type}");
                }
                if (type.index < 0 || type.index >= m_vInts.Count)
                {
                    Debug.LogError($"VariableList: GetInt index out of range, index={type.index}, count={m_vInts.Count}");
                    return defaultValue;
                }
                return m_vInts[type.index];
            }
            return defaultValue;
        }
        //-----------------------------------------------------
        public List<int> GetInts()
        {
            return m_vInts;
        }
        //-----------------------------------------------------
        public void AddString(string value)
        {
            if (m_vStrings == null) m_vStrings = new List<string>(m_nCapacity);
            if (m_vTypes == null) m_vTypes = new List<TypeIndex>(m_nCapacity);
            m_vTypes.Add(new TypeIndex(EVariableType.eString, (byte)m_vStrings.Count));
            m_vStrings.Add(value);
        }
        //-----------------------------------------------------
        public void SetString(int index, string value)
        {
            if (index >= 0 && m_vStrings != null && m_vStrings.Count > 0 && m_vTypes != null && m_vTypes.Count > 0 && index < m_vTypes.Count)
            {
                var type = m_vTypes[index];
                if (type.type != EVariableType.eString)
                {
                    Debug.LogError($"VariableList: SetString type mismatch, expected {EVariableType.eString}, got {type}");
                }
                if (type.index < 0 || type.index >= m_vStrings.Count)
                {
                    Debug.LogError($"VariableList: SetString index out of range, index={type.index}, count={m_vStrings.Count}");
                    return;
                }
                m_vStrings[type.index] = value;
            }
        }
        //-----------------------------------------------------
        public string GetString(int index, string defaultValue = null)
        {
            if (index >= 0 && m_vStrings != null && m_vStrings.Count > 0 && m_vTypes != null && m_vTypes.Count > 0 && index < m_vTypes.Count)
            {
                var type = m_vTypes[index];
                if (type.type != EVariableType.eString)
                {
                    Debug.LogError($"VariableList: GetString type mismatch, expected {EVariableType.eString}, got {type}");
                }
                if (type.index < 0 || type.index >= m_vStrings.Count)
                {
                    Debug.LogError($"VariableList: GetString index out of range, index={type.index}, count={m_vStrings.Count}");
                    return defaultValue;
                }
                return m_vStrings[type.index];
            }
            return defaultValue;
        }
        //-----------------------------------------------------
        public List<string> GetStrings()
        {
            return m_vStrings;
        }
        //-----------------------------------------------------
        internal void SwapIndex(int index0, int index1)
        {
            if (m_vTypes == null || index0 < 0 || index1 < 0 || index0 >= m_vTypes.Count || index1 >= m_vTypes.Count || index0 == index1)
                return;

            var typeIndex0 = m_vTypes[index0];
            var typeIndex1 = m_vTypes[index1];

            if (typeIndex0.type == typeIndex1.type)
            {
                // 同类型，交换数据和TypeIndex.index
                switch (typeIndex0.type)
                {
                    case EVariableType.eInt:
                        if (m_vInts != null)
                        {
                            int tmp = m_vInts[typeIndex0.index];
                            m_vInts[typeIndex0.index] = m_vInts[typeIndex1.index];
                            m_vInts[typeIndex1.index] = tmp;
                        }
                        break;
                    case EVariableType.eString:
                        if (m_vStrings != null)
                        {
                            string tmp = m_vStrings[typeIndex0.index];
                            m_vStrings[typeIndex0.index] = m_vStrings[typeIndex1.index];
                            m_vStrings[typeIndex1.index] = tmp;
                        }
                        break;
                    default:
                        break;
                }
                // 交换TypeIndex.index
                m_vTypes[index0] = new TypeIndex(typeIndex0.type, typeIndex1.index);
                m_vTypes[index1] = new TypeIndex(typeIndex1.type, typeIndex0.index);
            }
            else
            {
                // 不同类型，只交换TypeIndex，不动数据
                m_vTypes[index0] = typeIndex1;
                m_vTypes[index1] = typeIndex0;
            }
        }
        //-----------------------------------------------------
        public void Copy(VariableList list)
        {
            if (list == null) return;
            Clear();
            if (list.m_vInts != null && list.m_vInts.Count > 0)
            {
                if (m_vInts == null) m_vInts = new List<int>(list.m_vInts.Count);
                m_vInts.AddRange(list.m_vInts);
            }
            if (list.m_vStrings != null && list.m_vStrings.Count > 0)
            {
                if (m_vStrings == null) m_vStrings = new List<string>(list.m_vStrings.Count);
                m_vStrings.AddRange(list.m_vStrings);
            }
            if (list.m_vTypes != null && list.m_vTypes.Count > 0)
            {
                if (m_vTypes == null) m_vTypes = new List<TypeIndex>(list.m_vTypes.Count);
                m_vTypes.AddRange(list.m_vTypes);
            }
        }
    }
}