/********************************************************************
生成日期:	1:11:2020 10:08
类    名: 	Logger
作    者:	HappLI
描    述:	日志系统
*********************************************************************/
#if !USE_SERVER
using UnityEngine;
#endif
namespace Framework.Base
{
    public enum ELogType
    {
        Info        = 1 << 0,
        Warning     = 1 << 1,
        Error       = 1 << 2,
        Break       = 1 << 3,
        Asset       = 1 << 4,
        Exception   = 1 << 5,
    }

    public class Logger
    {
        private uint    m_nFlag = 0;
        bool            m_bFile = false;
        string          m_strFile = null;
#if USE_SERVER
        System.Text.StringBuilder m_pStringBuilder = new System.Text.StringBuilder();
#endif

        private static Logger sm_pInstance;

        //------------------------------------------------------
        public static Logger getInstance()
        {
            if (sm_pInstance == null)
            {
                    if (sm_pInstance == null)
                        sm_pInstance = new Logger();
                return sm_pInstance;
            }

            return sm_pInstance;
        }
        //------------------------------------------------------
        public Logger()
        {
            EnableFlag((uint)(ELogType.Info| ELogType.Error| ELogType.Warning | ELogType.Break));
        }
        //------------------------------------------------------
        public void EnableToFile(bool bFile, string strFile = null)
        {
            m_bFile = bFile;
            m_strFile = strFile;
        }
        //------------------------------------------------------
        public void EnableFlag(uint nFlag)
        {
            m_nFlag |= nFlag;
        }      
        //------------------------------------------------------
        public void SetFlag(uint nFlag)
        {
            m_nFlag = nFlag;
        }
        //------------------------------------------------------
        public uint GetFlag()
        {
            return m_nFlag;
        }
        //------------------------------------------------------
        public void LogType(ELogType eType, string pMsg)
        {
            if(eType == ELogType.Asset || eType == ELogType.Exception)
                return;

            Log(eType, pMsg);
        }
        //------------------------------------------------------
        void LogAsset(bool bCondition, string strMsg = null)
        {
            if ((((uint)ELogType.Asset) & m_nFlag) == 0) return;
#if !USE_SERVER
            if (strMsg != null)
                Debug.Assert(bCondition, strMsg);
            else
                Debug.Assert(bCondition);
#else
            m_pStringBuilder.Clear();
            System.Console.WriteLine(m_pStringBuilder.Append("Assert:").Append(strMsg).ToString());
#endif
        }
        //------------------------------------------------------
        void LogException(System.Exception pException)
        {
            if ((((uint)ELogType.Exception) & m_nFlag) == 0) return;
#if USE_SERVER
            m_pStringBuilder.Clear();
            System.Console.WriteLine(m_pStringBuilder.Append("Exception:").Append(pException.ToString()).ToString());
#else
            Debug.LogException(pException);
#endif
        }
        //------------------------------------------------------
        void Log(ELogType nLevel, string pMsg)
        {
            if ((((uint)nLevel) & m_nFlag) == 0) return;
#if USE_SERVER
            m_pStringBuilder.Clear();
            if (nLevel == ELogType.Info) System.Console.WriteLine(m_pStringBuilder.Append("Info:").Append(pMsg).ToString());
            else if (nLevel == ELogType.Warning) System.Console.WriteLine(m_pStringBuilder.Append("Warning:").Append(pMsg).ToString());
            else if (nLevel == ELogType.Error) System.Console.WriteLine(m_pStringBuilder.Append("Error:").Append(pMsg).ToString());
            else if (nLevel == ELogType.Break) System.Console.WriteLine(m_pStringBuilder.Append("Break:").Append(pMsg).ToString());
#else
            if (nLevel == ELogType.Info) Debug.Log(pMsg);
            else if (nLevel == ELogType.Warning) Debug.LogWarning(pMsg);
            else if (nLevel == ELogType.Error) Debug.LogError(pMsg);
            else if (nLevel == ELogType.Break) Debug.Break();
#endif
            if (m_bFile)
            {
                //! TODO....
            }
        }
        //------------------------------------------------------
        public static void Info(string pMsg)
        {
            Logger.getInstance().Log(ELogType.Info, pMsg);
        }
        //------------------------------------------------------
        public static void Warning(string pMsg)
        {
            Logger.getInstance().Log(ELogType.Warning, pMsg);
        }
        //------------------------------------------------------
        public static void Error(string pMsg)
        {
            Logger.getInstance().Log(ELogType.Error, pMsg);
        }
        //------------------------------------------------------
        public static void Break(string pMsg)
        {
            Logger.getInstance().Log(ELogType.Break, pMsg);
        }
        //------------------------------------------------------
        public static void Asset(bool bCondition, string strMsg=null)
        {
            Logger.getInstance().LogAsset(bCondition, strMsg);
        }
        //------------------------------------------------------
        public static void Asset(System.Exception pException)
        {
            Logger.getInstance().LogException(pException);
        }
    }
}

