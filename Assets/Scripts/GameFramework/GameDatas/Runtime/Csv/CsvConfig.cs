/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	CsvConfig
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;

namespace Framework.Data
{
    public enum EDataType : byte
    {
        None,
        Binary,
        Csv,
        Json,
    }
    [System.Serializable]
    public struct CsvAsset
    {
        public int nHash;
        public bool csharpParse;
        public bool dllParse;
        public bool exportAT;
        public EDataType type;
        public TextAsset Asset;
    }
    [CreateAssetMenu(menuName = "GameData/CsvDatas")]
    public class CsvConfig : ScriptableObject
    {
        public int TileLine = 3;    //名称，数据类型，键值
        public bool bBinary = false;
        public bool dllRead = true;
        public string[] CommonAssets = null;
        public CsvAsset[] Assets;
    }
}
