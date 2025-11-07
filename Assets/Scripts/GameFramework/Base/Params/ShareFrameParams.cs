
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class ShareFrameParams
    {
        AFramework m_pFramework;
        List<byte> m_CatchByteList;
        List<long> m_CatchLongList;
        Dictionary<int, int> m_CatchIntMapStats;
        HashSet<IUserData> m_CatchUserDataSet;
        Dictionary<string, int> m_CatchStringMapStats;
        List<IUserData> m_TempCatchUserDatas = null;
        List<UnityEngine.Transform> m_TempCatchTrans = null;
        List<UnityEngine.Object> m_vEngineObjectNodes = null;
        List<int> m_TempCatchInts = null;
        List<UnityEngine.Vector3> m_TempCatchPoints = null;
        List<UnityEngine.Vector2Int> m_TempCatchPointInt2Ds = null;
        List<UnityEngine.Vector3Int> m_TempCatchPointInt3Ds = null;
        HashSet<UnityEngine.Vector3Int> m_TempCatchPointInt3DSets = null;
        List<int> m_LogicArgvs = null;
        HashSet<int> m_CatchIntSet;
        HashSet<long> m_CatchLongSet;
        HashSet<string> m_CatchStringSet;
        List<string> m_CatchStringList;
        HashSet<HitFrameActor> m_vHitFrameCaches;

        List<AWorldNode> m_CatchNodeList;

        Dictionary<string, IUserData> m_vRuntimeDatas = null;
        Base.IntersetionParam m_IntersetionParam = null;
        public ShareFrameParams(AFramework pFramework)
        {
            m_pFramework = pFramework;
        }
        //------------------------------------------------------
        public void AddRuntimeData(string strKey, IUserData pUserData)
        {
            if (m_vRuntimeDatas == null) m_vRuntimeDatas = new Dictionary<string, IUserData>();
            m_vRuntimeDatas[strKey.ToLower()] = pUserData;
        }
        //------------------------------------------------------
        public void RemoveRuntimeData(string strKey)
        {
            if (m_vRuntimeDatas == null) return;
            m_vRuntimeDatas.Remove(strKey.ToLower());
        }
        //------------------------------------------------------
        public IUserData GetRuntimeData(string strKey)
        {
            if (m_vRuntimeDatas == null) return null;
            IUserData pUserData;
            if (m_vRuntimeDatas.TryGetValue(strKey.ToLower(), out pUserData)) return pUserData;
            return null;
        }
        //------------------------------------------------------
        public T GetRuntimeData<T>(string strKey) where T :IUserData
        {
            if (m_vRuntimeDatas == null) return default;
            IUserData pUserData;
            if (m_vRuntimeDatas.TryGetValue(strKey.ToLower(), out pUserData) && pUserData is T) return (T)pUserData;
            return default;
        }
        //------------------------------------------------------
        public List<int> LogicArgvs
        {
            get
            {
                if (m_LogicArgvs == null) m_LogicArgvs = new List<int>();
                return m_LogicArgvs;
            }
        }
        //------------------------------------------------------
        internal void ClearRuntimeDatas()
        {
            if (m_vRuntimeDatas != null) m_vRuntimeDatas.Clear();
        }
        //------------------------------------------------------
        internal void ClearLogicTemp()
        {
            if (m_LogicArgvs != null) m_LogicArgvs.Clear();
            if (m_TempCatchUserDatas != null) m_TempCatchUserDatas.Clear();
            if (m_TempCatchInts != null) m_TempCatchInts.Clear();
            if (m_CatchIntSet != null) m_CatchIntSet.Clear();
            if (m_CatchLongSet != null) m_CatchLongSet.Clear();
            if (m_CatchLongList != null) m_CatchLongList.Clear();
            if (m_CatchUserDataSet != null) m_CatchUserDataSet.Clear();
            if (m_CatchStringMapStats != null) m_CatchStringMapStats.Clear();
            if (m_CatchIntMapStats != null) m_CatchIntMapStats.Clear();
            if (m_CatchStringSet != null) m_CatchStringSet.Clear();
            if (m_CatchStringList != null) m_CatchStringList.Clear();
            if (m_CatchByteList != null) m_CatchByteList.Clear();
            if (m_TempCatchPoints != null) m_TempCatchPoints.Clear();
            if (m_TempCatchTrans != null) m_TempCatchTrans.Clear();
            if (m_TempCatchPointInt2Ds != null) m_TempCatchPointInt2Ds.Clear();
            if (m_TempCatchPointInt3Ds != null) m_TempCatchPointInt3Ds.Clear();
            if (m_TempCatchPointInt3DSets != null) m_TempCatchPointInt3DSets.Clear();
            if (m_vEngineObjectNodes != null) m_vEngineObjectNodes.Clear();
            if (m_vHitFrameCaches != null) m_vHitFrameCaches.Clear();
            if (m_CatchNodeList != null) m_CatchNodeList.Clear();
            OnClearLogicTemp();
        }
        //------------------------------------------------------
        protected virtual void OnClearLogicTemp() { }
        //------------------------------------------------------
        public List<byte> catchByteList
        {
            get
            {
                if (m_CatchByteList == null) m_CatchByteList = new List<byte>(4);
                return m_CatchByteList;
            }
        }
        //------------------------------------------------------
        public HashSet<HitFrameActor> hitFrameActorCaches
        {
            get
            {
                if (m_vHitFrameCaches == null) m_vHitFrameCaches = new HashSet<HitFrameActor>(2);
                return m_vHitFrameCaches;
            }
        }
        //------------------------------------------------------
        public List<AWorldNode> catchNodeList
        {
            get
            {
                if (m_CatchNodeList == null) m_CatchNodeList = new List<AWorldNode>(2);
                return m_CatchNodeList;
            }
        }
        //------------------------------------------------------
        public List<IUserData> catchUserDataList
        {
            get
            {
                if (m_TempCatchUserDatas == null) m_TempCatchUserDatas = new List<IUserData>(4);
                return m_TempCatchUserDatas;
            }
        }
        //------------------------------------------------------
        public HashSet<IUserData> catchUserDataSet
        {
            get
            {
                if (m_CatchUserDataSet == null) m_CatchUserDataSet = new HashSet<IUserData>(4);
                return m_CatchUserDataSet;
            }
        }
        //------------------------------------------------------
        public List<int> catchIntList
        {
            get
            {
                if (m_TempCatchInts == null) m_TempCatchInts = new List<int>(4);
                return m_TempCatchInts;
            }
        }
        //------------------------------------------------------
        public HashSet<int> intCatchSet
        {
            get
            {
                if (m_CatchIntSet == null) m_CatchIntSet = new HashSet<int>(4);
                return m_CatchIntSet;
            }
        }
        //------------------------------------------------------
        public HashSet<long> longCatchSet
        {
            get
            {
                if (m_CatchLongSet == null) m_CatchLongSet = new HashSet<long>(4);
                return m_CatchLongSet;
            }
        }
        //------------------------------------------------------
        public List<long> longCatchList
        {
            get
            {
                if (m_CatchLongList == null) m_CatchLongList = new List<long>(4);
                return m_CatchLongList;
            }
        }
        //------------------------------------------------------
        public HashSet<string> stringCatchSet
        {
            get
            {
                if (m_CatchStringSet == null) m_CatchStringSet = new HashSet<string>(4);
                return m_CatchStringSet;
            }
        }
        //------------------------------------------------------
        public List<string> stringCatchList
        {
            get
            {

                if (m_CatchStringList == null) m_CatchStringList = new List<string>(4);
                return m_CatchStringList;
            }
        }
        //------------------------------------------------------
        public Dictionary<string, int> stringCatchStatsMap
        {
            get
            {
                if (m_CatchStringMapStats == null) m_CatchStringMapStats = new Dictionary<string, int>(4);
                return m_CatchStringMapStats;
            }
        }
        //------------------------------------------------------
        public Dictionary<int, int> catchIntMapStats
        {
            get
            {
                if (m_CatchIntMapStats == null) m_CatchIntMapStats = new Dictionary<int, int>(4);
                return m_CatchIntMapStats;
            }
        }
        //------------------------------------------------------
        public List<UnityEngine.Transform> catchTransforms
        {
            get
            {
                if (m_TempCatchTrans == null) m_TempCatchTrans = new List<UnityEngine.Transform>(2);
                return m_TempCatchTrans;
            }
        }
        //------------------------------------------------------
        public List<UnityEngine.Object> catchEngineObjs
        {
            get
            {
                if (m_vEngineObjectNodes == null) m_vEngineObjectNodes = new List<UnityEngine.Object>(2);
                return m_vEngineObjectNodes;
            }
        }
        //------------------------------------------------------
        public List<UnityEngine.Vector3> catchPoints
        {
            get
            {
                if (m_TempCatchPoints == null) m_TempCatchPoints = new List<UnityEngine.Vector3>(2);
                return m_TempCatchPoints;
            }
        }
        //------------------------------------------------------
        public List<UnityEngine.Vector2Int> catchPointInt2Ds
        {
            get
            {
                if (m_TempCatchPointInt2Ds == null) m_TempCatchPointInt2Ds = new List<UnityEngine.Vector2Int>(2);
                return m_TempCatchPointInt2Ds;
            }
        }
        //------------------------------------------------------
        public List<UnityEngine.Vector3Int> catchPointInt3Ds
        {
            get
            {
                if (m_TempCatchPointInt3Ds == null) m_TempCatchPointInt3Ds = new List<UnityEngine.Vector3Int>(2);
                return m_TempCatchPointInt3Ds;
            }
        }
        //------------------------------------------------------
        public HashSet<UnityEngine.Vector3Int> catchPointInt3DSets
        {
            get
            {
                if (m_TempCatchPointInt3DSets == null) m_TempCatchPointInt3DSets = new HashSet<UnityEngine.Vector3Int>(2);
                return m_TempCatchPointInt3DSets;
            }
        }
        //------------------------------------------------------
        public Base.IntersetionParam intersetionParam
        {
            get
            {
                if (m_IntersetionParam == null)
                {
                    m_IntersetionParam = new Base.IntersetionParam();
                    m_IntersetionParam.Check();
                }
                return m_IntersetionParam;
            }
        }
    }
}
