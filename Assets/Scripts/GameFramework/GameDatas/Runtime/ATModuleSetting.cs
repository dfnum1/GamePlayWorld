/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	图形脚本模块配置
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
namespace Framework.Data
{

    [CreateAssetMenu]
    public class ATModuleSetting : ScriptableObject
    {
        public Framework.Plugin.AT.AgentTreeData mainAT = null;
        public Framework.Plugin.AT.AgentTreeData uiMgrAT = null;
        public Framework.Plugin.AT.AgentTreeData soundAT = null;
        public Framework.Plugin.AT.AgentTreeData[] stateATs = null;

   //     [HideInInspector]
   //     public Framework.Plugin.AT.VariableSerializes Globals;

        static ATModuleSetting ms_Instance = null;
        public static ATModuleSetting Instance
        {
            get { return ms_Instance; }
        }
        private void OnEnable()
        {
            ms_Instance = this;
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            ms_Instance = null;
        }
        //------------------------------------------------------
        public static Framework.Plugin.AT.AgentTreeData MainAT
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.mainAT;
            }
        }
        //------------------------------------------------------
        public static Framework.Plugin.AT.AgentTreeData UIMgrAT
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.uiMgrAT;
            }
        }
        //------------------------------------------------------
        public static Framework.Plugin.AT.AgentTreeData SoundAT
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.soundAT;
            }
        }
        //------------------------------------------------------
        public static Framework.Plugin.AT.AgentTreeData GetStateAT(int state)
        {
            if (ms_Instance == null || ms_Instance.stateATs == null ||state >= ms_Instance.stateATs.Length) return null;
            return ms_Instance.stateATs[state];
        }
    }
}
