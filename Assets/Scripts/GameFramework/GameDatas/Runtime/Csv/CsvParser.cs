/********************************************************************
生成日期:	23:7:2019   20:58
类    名: 	CsvParser
作    者:	HappLI
描    述:	Csv 解析器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

#if UNITY_5_1
	public struct Vector2Int
	{
		public int x;
		public int y;
		public Vector2Int(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
		public static Vector2Int zero = new Vector2Int(0,0);
	}
	public struct Vector3Int
	{
		public int x;
		public int y;
		public int z;
		public Vector3Int(int _x, int _y, int _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}
		public static Vector3Int zero = new Vector3Int(0,0, 0);
	}	
#endif

namespace Framework.Data
{
    public class CsvParser
    {
        //------------------------------------------------------
        //! TableItem
        //------------------------------------------------------
        public struct TableItem
        {
            string m_szItemString;
            public string str
            {
                get { return m_szItemString; }
            }

            static TableItem DEFAULT_TABLE = new TableItem() { m_szItemString = null };
            public static TableItem DefaultTable(string strStr)
            {
                DEFAULT_TABLE.m_szItemString = strStr;
                return DEFAULT_TABLE;
            }

            public TableItem(string szItemString) { m_szItemString = szItemString; }
            public bool IsValid() { return (m_szItemString!=null && m_szItemString.Length>0 && m_szItemString[0] != 0); }

            public string String() { return (IsValid() ? m_szItemString : string.Empty); }
            public static string String(string val) { return string.IsNullOrEmpty(val)?"":val; }
            //       string stringUTF8() { string szItemString;	if( IsValid()) System.Text.UTF8Encoding.Convert( m_szItemString, szItemString ); return szItemString; }
            public bool Bool() { int temp = 0; int.TryParse(m_szItemString, out temp); return temp!=0; }
            public static string Bool(bool val) { return val?"1":"0"; }
            public char Char() { return m_szItemString[0]; }
            public static string Char(char val) { return val.ToString(); }
            public byte Byte() { byte temp = 0; byte.TryParse(m_szItemString, out temp); return temp; }
            public static string Byte(byte val) { return val.ToString(); }
            public short Short() { short temp = 0; short.TryParse(m_szItemString, out temp); return temp; }
            public static string Short(short val) { return val.ToString(); }
            public ushort Ushort() { ushort temp = 0; ushort.TryParse(m_szItemString, out temp); return temp; }
            public static string Ushort(ushort val) { return val.ToString(); }
            public int Int() { int temp = 0; int.TryParse(m_szItemString, out temp); return temp; }
            public static string Int(int val) { return val.ToString(); }
            public uint Uint() { uint temp = 0; uint.TryParse(m_szItemString, out temp); return temp; }
            public static string Uint(uint val) { return val.ToString(); }
            public long Long() { long temp = 0;  long.TryParse(m_szItemString, out temp);  return temp; }
            public static string Long(long val) { return val.ToString(); }
            public ulong Ulong() { ulong temp = 0; ulong.TryParse(m_szItemString, out temp); return temp; }
            public static string Ulong(ulong val) { return val.ToString(); }
            public float Float() { float temp = 0; float.TryParse(m_szItemString, out temp); return temp; }
            public static string Float(float val) 
            {
                if(val.ToString().Contains("."))
                    return string.Format("{0:f2}", val); 
                return string.Format("{0:f0}", val);
            }
            public double Double() { double temp = 0; double.TryParse(m_szItemString, out temp); return temp; }
            public static string Double(double val)
            {
                if (val.ToString().Contains("."))
                    return string.Format("{0:f4}", val);
                return string.Format("{0:f0}", val);
            }
            public Vector2 Vec2(char spec = '|')
            {
                if (!IsValid()) return Vector2.zero;
                string[] split = m_szItemString.Split(spec);
                if(split.Length == 2)
                {
                    return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
                }
                return Vector2.zero;
            }
            public static string Vec2(Vector2 val, char spec = '|') { return string.Format("{0:f2}{1}{2:f2}", val.x, spec, val.y); }

            public Vector3 Vec3(char spec = '|')
            {
                if (!IsValid()) return Vector3.zero;
                string[] split = m_szItemString.Split(spec);
                if (split.Length == 3)
                {
                    return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
                }
                return Vector3.zero;
            }
            public static string Vec3(Vector3 val, char spec = '|') { return string.Format("{0:f2}{1}{2:f2}{3}{4:f2}", val.x, spec, val.y, spec, val.z); }

            public Vector4 Vec4(char spec = '|')
            {
                if (!IsValid()) return Vector4.zero;
                string[] split = m_szItemString.Split(spec);
                if (split.Length == 4)
                {
                    return new Vector4(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                }
                return Vector4.zero;
            }
            public static string Vec4(Vector4 val, char spec = '|') { return string.Format("{0:f2}{1}{2:f2}{3}{4:f2}{5}{6:f2}", val.x, spec, val.y, spec, val.z, spec, val.w); }

            public Vector2Int Vec2int(char spec = '|')
            {
                if (!IsValid()) return Vector2Int.zero;
                string[] split = m_szItemString.Split(spec);
                if (split.Length == 2)
                {
                    return new Vector2Int(int.Parse(split[0]), int.Parse(split[1]));
                }
                return Vector2Int.zero;
            }
            public static string Vec2int(Vector2Int val, char spec = '|') { return string.Format("{0}{1}{2}", val.x, spec, val.y); }

            public Vector3Int Vec3int(char spec= '|')
            {
                if (!IsValid()) return Vector3Int.zero;
                string[] split = m_szItemString.Split(spec);
                if (split.Length == 3)
                {
                    return new Vector3Int(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                }
                return Vector3Int.zero;
            }
            public static string Vec3int(Vector3Int val, char spec = '|') { return string.Format("{0}{1}{2}{3}{4}", val.x, spec, val.y, spec, val.z); }

            public byte[] ByteArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    byte[] ret = new byte[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = (byte)Mathf.Clamp(int.Parse(split[i]), 0, 255);
                    return ret;
                }
                return null;
            }
            public static string ByteArray(byte[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for(int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i].ToString();
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }

            public int[] IntArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    int[] ret = new int[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = int.Parse(split[i]);
                    return ret;
                }
                return null;
            }

            public static string IntArray(int[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i].ToString();
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
            public uint[] UintArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    uint[] ret = new uint[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = uint.Parse(split[i]);
                    return ret;
                }
                return null;
            }

            public static string UintArray(uint[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i].ToString();
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
            public float[] FloatArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    float[] ret = new float[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = float.Parse(split[i]);
                    return ret;
                }
                return null;
            }

            public static string FloatArray(float[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += string.Format("{0:f2}", val[i]);
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
            public short[] ShortArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    short[] ret = new short[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = short.Parse(split[i]);
                    return ret;
                }
                return null;
            }

            public static string ShortArray(short[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i].ToString();
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
            public ushort[] UshortArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0)
                {
                    ushort[] ret = new ushort[split.Length];
                    for (int i = 0; i < ret.Length; ++i)
                        ret[i] = ushort.Parse(split[i]);
                    return ret;
                }
                return null;
            }

            public static string UshortArray(ushort[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i].ToString();
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }

            public Vector2[] Vec2Array(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length >0 && split.Length%2 == 0)
                {
                    Vector2[] rets = new Vector2[split.Length/2];
                    for(int i = 0; i < split.Length; i+=2)
                    {
                        rets[i/2] = new Vector2(float.Parse(split[0+i]), float.Parse(split[1+i]));
                    }
                    return rets;
                }
                return null;
            }

            public static string Vec2Array(Vector2[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += string.Format("{0:f2}{1}{2:f2}", val[i].x, spec, val[i].y);
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }

            public Vector2Int[] Vec2IntArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0 && split.Length % 2 == 0)
                {
                    Vector2Int[] rets = new Vector2Int[split.Length / 2];
                    for (int i = 0; i < split.Length; i += 2)
                    {
                        rets[i/2] = new Vector2Int(int.Parse(split[0 + i]), int.Parse(split[1 + i]));
                    }
                    return rets;
                }
                return null;
            }

            public static string Vec2IntArray(Vector2Int[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += string.Format("{0}{1}{2}", val[i].x, spec, val[i].y);
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }

            public Vector3[] Vec3Array(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0 && split.Length % 3 == 0)
                {
                    Vector3[] rets = new Vector3[split.Length / 3];
                    for (int i = 0; i < split.Length; i += 3)
                    {
                        rets[i/3] = new Vector3(float.Parse(split[0 + i]), float.Parse(split[1 + i]), float.Parse(split[2 + i]));
                    }
                    return rets;
                }
                return null;
            }

            public static string Vec3Array(Vector3[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += string.Format("{0:f2}{1}{2:f2}{3}{4:f2}", val[i].x, spec, val[i].y, spec, val[i].z);
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
            public Vector3Int[] Vec3IntArray(char spec = '|')
            {
                if (!IsValid()) return null;
                string[] split = m_szItemString.Split(spec);
                if (split.Length > 0 && split.Length % 3 == 0)
                {
                    Vector3Int[] rets = new Vector3Int[split.Length / 3];
                    for (int i = 0; i < split.Length; i += 3)
                    {
                        rets[i/3] = new Vector3Int(int.Parse(split[0 + i]), int.Parse(split[1 + i]), int.Parse(split[2 + i]));
                    }
                    return rets;
                }
                return null;
            }
            public static string Vec3IntArray(Vector3Int[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += string.Format("{0}{1}{2}{3}{4}", val[i].x, spec, val[i].y, spec, val[i].z);
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }

            public string[] StringArray(char spec = '|')
            {
                if (!IsValid()) return null;
                return m_szItemString.Split(spec);
            }

            public static string StringArray(string[] val, char spec = '|')
            {
                if (val == null || val.Length <= 0) return "";
                string strTemp = "";
                for (int i = 0; i < val.Length; ++i)
                {
                    strTemp += val[i];
                    if (i < val.Length - 1) strTemp += spec;
                }
                return strTemp;
            }
        }
        //------------------------------------------------------
        //! TableLine
        //------------------------------------------------------
        public class TableLine
        {
            CsvParser m_pTable;
            int m_iLineIdx;
            internal TableLine() { }
            internal TableLine(CsvParser pTable, int iLineIdx) { m_pTable = pTable; m_iLineIdx = iLineIdx; }
            internal void SetLine(CsvParser pTable, int iLineIdx) { m_pTable = pTable; m_iLineIdx = iLineIdx; }


            public TableItem this[int index]
            {
                get
                {
                    return new TableItem(m_pTable.GetString(m_iLineIdx, index));
                }
            }
            public TableItem this[string index]
            {
                get
                {
                    return new TableItem(m_pTable.GetString(m_iLineIdx, index));
                }
            }
        }
        //------------------------------------------------------
        //! CsvParser
        //------------------------------------------------------

#if UNITY_EDITOR
        string m_strName = null;
#endif
        char m_cSeparator;
        string m_pContent;
        List<List<TableItem>> m_pItems;
        Dictionary<string, int> m_NameColumnIndex = null;
        TableLine m_ParseLine = new TableLine();

        int m_iLineCount;
        int m_iMaxColumn;

        int m_iTitleLine;
        int m_iTitleColumn;
        //------------------------------------------------------
        public CsvParser()
        {
            m_cSeparator = ',';
            m_pContent = null;
            m_pItems = null;
            m_NameColumnIndex = null;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void SetName(string name)
        {
            m_strName = name;
        }
        //------------------------------------------------------
        public string GetName()
        {
            return m_strName;
        }
#endif
        //------------------------------------------------------
        bool ParseTextTable()
        {
            int iLine;
            int iState; // 0 普通, 1 字符串内
            int pWord, pCur, pd;

            if (m_NameColumnIndex != null) m_NameColumnIndex.Clear();
            else m_NameColumnIndex = new Dictionary<string, int>();

            int npos = m_pContent.Length;
            iState = 0;
            iLine = 0;
            List<TableItem> vLines = null;
            System.Text.StringBuilder stringBuilder = BaseUtil.stringBuilder;
            pd = pWord = pCur = 0;
            while (pCur < npos)
            {
                if (iState == 0)
                {
                    if (m_pContent[pCur] == '"')
                    {
                        iState = 1;
                    }
                    else if (m_pContent[pCur] == m_cSeparator)
                    {
                        if (vLines == null)
                        {
                            vLines = new List<TableItem>();
                            m_pItems.Add(vLines);
                        }

                        string str = stringBuilder.ToString();
                        vLines.Add(new TableItem(str));

                        pd = pWord = pCur + 1;
                        stringBuilder.Clear();
                    }
                    else if (m_pContent[pCur] == 0x0A || m_pContent[pCur] == 0x0D)
                    {
                        if (pCur + 1 < m_pContent.Length && (m_pContent[pCur + 1] == 0x0A || m_pContent[pCur + 1] == 0x0D))
                        {
                            pCur++;
                        }
                        if(vLines!=null)
                        {
                            string str = stringBuilder.ToString();
                            vLines.Add(new TableItem(str));
                            if(m_pItems!=null && m_pItems.Count>m_iTitleLine)
                            {
                                if (vLines.Count < m_pItems[m_iTitleLine].Count)
                                {
#if UNITY_EDITOR
                                    if (!string.IsNullOrEmpty(m_strName))
                                        Debug.LogWarning(m_strName + " 第" + iLine + "行 列数不足" + m_pItems[m_iTitleLine].Count + "列");
#endif
                                    Clear();
                                    return false;
                                }
                            }
                        }
                        vLines = null;
                        iLine++;
                        pd = pWord = pCur + 1;
                        stringBuilder.Clear();
                    }
                    else if (m_pContent[pCur] == '\r')
                    {
                        if (pCur != pd) pd = pCur;
                        pd++;
                    }
                    else
                    {
                        if (pCur != pd) pd = pCur;
                        pd++;
                        stringBuilder.Append(m_pContent[pCur]);
                    }
                }
                else if (iState == 1)
                {
                    if (m_pContent[pCur] == '"')
                    {
                        if (m_pContent[pCur + 1] == '"')
                        {
                            // 还是双引号
                            pCur++;
                            if (pCur != pd) pd = pCur;
                            pd++;
                            stringBuilder.Append(m_pContent[pCur]);
                        }
                        else
                        {
                            // 结束
                            iState = 0;
                        }
                    }
                    else
                    {
                        if (pCur != pd) pd = pCur;
                        pd++;
                        stringBuilder.Append(m_pContent[pCur]);
                    }
                }
                pCur++;
            }

            stringBuilder.Clear();
            m_iLineCount = iLine;
            if (m_pItems.Count < m_iTitleLine) return false;
            m_iMaxColumn = m_pItems[0].Count;
            for (int i = 0; i < m_iMaxColumn; ++i)
            {
                string keyname = m_pItems[m_iTitleLine][i].str;
                if (string.IsNullOrEmpty(keyname)) continue;
                m_NameColumnIndex[keyname] = i;
            }
            return iLine==m_pItems.Count && m_NameColumnIndex.Count == m_iMaxColumn;
        }
        //------------------------------------------------------
        public bool LoadTableString(string strString)
        {
            Clear();

            int iFileSize = strString.Length;
            m_pContent = strString;
            if(m_pItems == null) m_pItems = new List<List<TableItem>>();

            ParseTextTable();
            m_pContent = null;
            return m_iMaxColumn>0 && m_iTitleLine>0;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_iTitleLine = 3;
            m_iTitleColumn = 0;
            m_iMaxColumn = 0;
            m_iLineCount = 0;
            m_pContent = null;
            if(m_pItems!=null)
            {
                for(int i = 0; i < m_pItems.Count; ++i)
                {
                    m_pItems[i].Clear();
                }
            }
            m_pItems = null;
            m_ParseLine.SetLine(this,-1);
        }
        //------------------------------------------------------
        public bool IsLoaded() { return !string.IsNullOrEmpty(m_pContent); }
        //------------------------------------------------------
        public TableLine this[int nIndex]
        {
            get 
            {
                m_ParseLine.SetLine(this, nIndex);
                return m_ParseLine; 
            }
        }
        //------------------------------------------------------
        public TableLine this[string szIdx]
        {
            get
            {
                m_ParseLine.SetLine(this, -1);
                for (int i = 0; i < m_iLineCount; i++)
                {
                    if (szIdx.CompareTo(m_pItems[i][m_iTitleColumn].str) == 0)
                    {
                        m_ParseLine.SetLine(this, i);
                        return m_ParseLine;
                    }
                }
                return m_ParseLine;
            }
        }
        //------------------------------------------------------
        public string GetString(int iLine, string szColumnIdx)
        {
            if (iLine < 0 || iLine >= m_pItems.Count) return string.Empty;
            int columnIndex = GetStringColumn(iLine, szColumnIdx);
            if(columnIndex<0) return string.Empty;
            string Str = m_pItems[iLine][columnIndex].str;
            if (Str == null) Str = string.Empty;
            return Str;
        }
        //------------------------------------------------------
        public string GetString(string szLineIdx, string szColumnIdx)
        {
            return (this)[szLineIdx][szColumnIdx].str;
        }
        //------------------------------------------------------
        public int GetStringColumn(int iLine, string szColumnIdx)
        {
            int iNull = -1;
            if (iLine < 0) return iNull;
            int columnIndex = -1;
            if (!m_NameColumnIndex.TryGetValue(szColumnIdx, out columnIndex)) return iNull;
            return columnIndex;
        }
        //------------------------------------------------------
        public string GetString(int iLine, int iRow)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return "";
            return temp;
        }
        //------------------------------------------------------
        public bool Char(int iLine, int iRow, ref char vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (char.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "char 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Byte(int iLine, int iRow, ref byte vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            int tempV = -1;
            if (int.TryParse(temp, out tempV))
            {
                vValue = (byte)Mathf.Clamp(tempV, 0, byte.MaxValue);
                return true;
            }
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "byte 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Short(int iLine, int iRow, ref short vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (short.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "ushort 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Word(int iLine, int iRow, ref ushort vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            short tempV = -1;
            vValue = 0;
            if (short.TryParse(temp, out tempV))
            {
                vValue = (ushort)Mathf.Clamp(tempV, ushort.MinValue, ushort.MaxValue);
                return true;
            }
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "ushort 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Int(int iLine, int iRow, ref int vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (int.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "int 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool UInt(int iLine, int iRow, ref uint vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            int tempV = -1;
            vValue = 0;
            if (int.TryParse(temp, out tempV))
            {
                vValue = (uint)tempV;
                return true;
            }
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "uint 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Long(int iLine, int iRow, ref long vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (long.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "long 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool DWord(int iLine, int iRow, ref ulong vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            long tempV = -1;
            vValue = 0;
            if (long.TryParse(temp, out tempV))
            {
                vValue = (ulong)tempV;
                return true;
            }
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "ulong 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Float(int iLine, int iRow, ref float vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (float.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "float 解析失败!");
#endif
            return false;
        }
        //------------------------------------------------------
        public bool Double(int iLine, int iRow, ref double vValue)
        {
            string temp = GetItem(iLine, iRow);
            if (temp == null) return false;
            if (double.TryParse(temp, out vValue))
                return true;
#if UNITY_EDITOR
            AssertPop(iLine, iRow, "double 解析失败!");
#endif
            return false;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        void AssertPop(int iLine, int iColumn, string tips)
        {
            if (!string.IsNullOrEmpty(m_strName))
                Debug.LogAssertionFormat("表{0} 第{1}行 第{2}列", m_strName, iLine, iColumn, tips);
        }
#endif
        //------------------------------------------------------
        string GetItem(int iLine, int iColumn)
        {
            if (iColumn < 0 || iLine < 0 || iLine >= m_pItems.Count || iColumn >= m_pItems[iLine].Count) return null;
            return m_pItems[iLine][iColumn].str;
        }
        //------------------------------------------------------
        public int GetLineCount() { return m_iLineCount; }
        //------------------------------------------------------
        public int GetMaxColumn() { return m_iMaxColumn; }
        //------------------------------------------------------
        // 设置索引行(0 base)，用于用字符串索引表格
        public void SetTitleLine(int iIdx) { m_iTitleLine = iIdx; }
        //------------------------------------------------------
        public int GetTitleLine() { return m_iTitleLine; }
        //------------------------------------------------------
        // 设置索引列(0 base)，用于用字符串索引表格
        void SetReadTitleColumn(int iIdx)
        {
            m_iTitleColumn = iIdx;
        }
        //------------------------------------------------------
        public int GetReadTitleColumn()
        {
            return m_iTitleColumn;
        }
        //------------------------------------------------------
        // 查找关键字szString第一次出现的位置，找到返回true并且返回行列到iLine, iRow否则返回false
        public bool FindPosByString(string szString, ref int iLine, ref int iRow)
        {
            return false;
        }
        //------------------------------------------------------
        // 查找szString第一次出现的行，找不到返回-1
        public int FindLineByString(string szString) 
        {
            return 0;
        }
        //------------------------------------------------------
        // 查找szString第一次出现的列，找不到返回-1
        public int FindRowByString(string szString)  
        {
            return 0;
        }
    }

}
