/********************************************************************
生成日期:	1:11:2020 10:05
类    名: 	DataManager
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
#if USE_SERVER
using ExternEngine;
#else
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;
using static UnityEngine.Application;
#endif

namespace Framework.Data
{
    public abstract class ADataManager : Core.AModule
    {
        private System.IO.MemoryStream m_pMemoryStream = null;
        private System.IO.BinaryReader m_pBinaryReader = null;

        protected Core.Asset m_pAssetRef = null;

        protected int m_nLoadCnt = 0;
        protected int m_nTotalCnt = 0;
        Dictionary<int, Data_Base> m_vDatas = new Dictionary<int, Data_Base>(64);
        Dictionary<string, IUserData> m_vCustomDatas = null;
        public static Action OnLoaded = null;
        public bool bInited { get; set; }
        //-------------------------------------------
        public ADataManager()
        {
            bInited = false;
        }
        //-------------------------------------------
        protected override void OnInit()
        {
            bInited = false;
            m_nLoadCnt = 0;
            m_nTotalCnt = 0;
            m_vDatas.Clear();
        }
        //-------------------------------------------
        protected override void OnAwake()
        {
            if (GetFramework() == null)
                return;
            if (bInited)
            {
                CheckLoaded();
                return;
            }
            bInited = false;
            var game = GetFramework().gameStartup;
            if (game == null)
            {
                return;
            }
            CsvConfig csv = GetFramework().GetBindData<CsvConfig>();
            InitCsv(csv);
            CheckLoaded();
        }
        //-------------------------------------------
        void InitCsv(CsvConfig pConfig)
        {
            if (pConfig == null) return;
            m_nLoadCnt = 0;
            m_nTotalCnt = 0;
            m_vDatas.Clear();
            m_nTotalCnt += pConfig.Assets.Length;
            CsvParser csvParser = new CsvParser();
            for (int i = 0; i < pConfig.Assets.Length; ++i)
            {
                csvParser.Clear();
#if UNITY_EDITOR
                CsvAsset curParseAsset = new CsvAsset() { Asset = null };
                try
                {
#endif
                    if (pConfig.Assets[i].nHash == 0)
                    {
#if UNITY_EDITOR
                        string strPath = pConfig.Assets[i].Asset != null ? UnityEditor.AssetDatabase.GetAssetPath(pConfig.Assets[i].Asset.GetInstanceID()) : "";
                        if (UnityEditor.EditorUtility.DisplayDialog("错误", "配置表[" + strPath + "]数据读取失败", "请确认"))
                        {
                            UnityEditor.Selection.activeObject = pConfig;
                            UnityEngine.Debug.Break();
                        }
#endif
                        Debug.LogError("load csv[" + (pConfig.Assets[i].Asset ? pConfig.Assets[i].Asset.name : i.ToString()) + "]");
                        continue;
                    }
                    Data_Base csvData = Parser(csvParser,pConfig.Assets[i].nHash, pConfig.Assets[i].Asset, pConfig.Assets[i].type);
                    if (csvData == null)
                    {
#if UNITY_EDITOR
                        string strPath = pConfig.Assets[i].Asset != null ? UnityEditor.AssetDatabase.GetAssetPath(pConfig.Assets[i].Asset.GetInstanceID()) : "";
                        if (UnityEditor.EditorUtility.DisplayDialog("错误", "配置表[" + strPath + "]数据读取失败", "请确认"))
                        {
                            UnityEditor.Selection.activeObject = pConfig;
                            UnityEngine.Debug.Break();
                        }
#endif
                    }
                    else
                    {
                        csvData.SetHashID(pConfig.Assets[i].nHash);
                        m_vDatas[csvData.GetType().GetHashCode()] = csvData;
                    }
#if UNITY_EDITOR
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    if (curParseAsset.Asset != null)
                    {
                        string strPath = curParseAsset.Asset != null ? UnityEditor.AssetDatabase.GetAssetPath(curParseAsset.Asset.GetInstanceID()) : "";
                        if (UnityEditor.EditorUtility.DisplayDialog("错误", "配置表[" + strPath + "]数据读取失败\r\n" + ex.ToString(), "请确认"))
                        {
                            UnityEditor.Selection.activeObject = pConfig;
                            UnityEngine.Debug.Break();
                        }
                    }
                }
#endif
            }
            csvParser.Clear();
        }
        //-------------------------------------------
        public float Progress
        {
            get
            {
                if (m_nTotalCnt <= 0) return 1;
                return (float)m_nLoadCnt / (float)m_nTotalCnt;
            }
        }
        //------------------------------------------------------
        private void CheckLoaded()
        {
            if (Progress >= 1)
            {
                Mapping();
                //if (m_pAssetRef != null) m_pAssetRef.Release(0);
                //m_pAssetRef = null;
                OnParserOver();
                if (OnLoaded != null) OnLoaded();
                OnLoaded = null;

                Debug.Log("config data loaded!");
            }
        }
        //-------------------------------------------
        public T GetCustomData<T>(string strFile) where T : IUserData
        {
            if (string.IsNullOrEmpty(strFile)) return default;
            if (m_vCustomDatas != null)
            {
                IUserData getData = null;
                if (m_vCustomDatas.TryGetValue(strFile, out getData))
                    return (T)getData;
            }
            return default;
        }
        //-------------------------------------------
        public void AddCustomData(string strFile, IUserData userData)
        {
            if (string.IsNullOrEmpty(strFile) || userData == null) return;
            if (m_vCustomDatas == null) m_vCustomDatas = new Dictionary<string, IUserData>(64);
            m_vCustomDatas[strFile] = userData;
        }
        //-------------------------------------------
        public void UnloadCustom(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return;
            if (m_vCustomDatas != null) m_vCustomDatas.Remove(strFile);
        }
        //-------------------------------------------
        public void LoadBinary<T>(string strFile, System.Action<IUserData,bool> onCallback, bool bCache = false, bool bAbsFile = false) where T : IUserData
        {
            //AFileSystem fileSystem = FileSystemUtil.GetFileSystem();
            //if(fileSystem == null)
            //{
            //    if (onCallback != null) onCallback(default);
            //    return;
            //}
            //if (string.IsNullOrEmpty(strFile))
            //{
            //    if (onCallback != null) onCallback(default);
            //    return;
            //}
            //IUserData getData = null;
            //if (m_vCustomDatas != null && m_vCustomDatas.TryGetValue(strFile, out getData))
            //{
            //    if (onCallback != null) onCallback((T)getData);
            //    return;
            //}

            //string fullFile = strFile;
            //if(!bAbsFile) fullFile = Path.Combine(FileSystemUtil.StreamBinaryPath, strFile).Replace("\\", "/");
            //var assetOp = FileSystemUtil.LoadCustomFile(fullFile, OnReadBinaryFile);
            //assetOp.userData = new VariableType() { type = typeof(T) };
            //assetOp.userData1 = new VariableCallback1() { callback = onCallback };
            //assetOp.userData2 = new Framework.Core.VariableByte() { boolVal = bCache };
            //assetOp.userData3 = new Framework.Core.VariableString() { strValue = strFile };
        }
        //-------------------------------------------
        void OnReadBinaryFile(AssetOperiaon assetOp)
        {
            VariableType userType = (VariableType)assetOp.userData;
            VariableCallback1 callback = (VariableCallback1)assetOp.userData1;
            Framework.Core.VariableByte cacheFlag = (Framework.Core.VariableByte)assetOp.userData2;
            Core.VariableString strFile = (Core.VariableString)assetOp.userData3;

            IUserData newData = OnReadBinary(userType.type, assetOp.pBufferAsset.buffers, assetOp.pBufferAsset.bufferSize);
            callback.Invoke(newData);
            if (cacheFlag.boolVal)
            {
                m_vCustomDatas[strFile.strValue] = newData;
            }
        }
        //-------------------------------------------
        public System.IO.BinaryReader BeginBinary(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) return null;
            if (m_pBinaryReader == null)
            {
                m_pMemoryStream = new System.IO.MemoryStream();
                m_pBinaryReader = new System.IO.BinaryReader(m_pMemoryStream);
            }
            m_pMemoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            m_pMemoryStream.SetLength(bytes.Length);
            m_pMemoryStream.Write(bytes, 0, bytes.Length);
            m_pMemoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return m_pBinaryReader;
        }
        //-------------------------------------------
        public void EndBinary()
        {
            if (m_pMemoryStream != null)
                m_pMemoryStream.SetLength(0);
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            //if (m_pAssetRef != null) m_pAssetRef.Release(0);
            //m_pAssetRef = null;
            OnLoaded = null;
            m_nLoadCnt = 0;
            m_nTotalCnt = 0;
            bInited = false;
            if (m_vCustomDatas != null)
            {
                foreach (var db in m_vCustomDatas)
                    db.Value.Destroy();
                m_vCustomDatas.Clear();
            }
        }
        //------------------------------------------------------
        public T GetCsv<T>() where T : Data_Base
        {
            int hashCode = typeof(T).GetHashCode();
            Data_Base csvData;
            if (m_vDatas.TryGetValue(hashCode, out csvData))
                return csvData as T;
            return null;
        }
        //-------------------------------------------
        protected abstract Data_Base Parser(CsvParser csvParser, int index, TextAsset pAsset, EDataType eType = EDataType.Binary);
        protected abstract void Mapping();
        //-------------------------------------------
        protected virtual IUserData OnReadBinary(System.Type classType, byte[] buffers, int dataSize)
        {
            return null;
        }
        //-------------------------------------------
        protected virtual void OnParserOver() { }
    }
}
