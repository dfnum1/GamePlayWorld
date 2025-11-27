/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideCustomAgent
作    者:	HappLI
描    述:	引导自定义步骤、执行器、触发器
*********************************************************************/
using UnityEngine;
namespace Framework.Guide
{
    public class GuideCustomAgent : ScriptableObject
    {
        [System.Serializable]
        public class AgentArgv
        {
            public string name = "";
            public string displayType;
            public EBitGuiType bBit = EBitGuiType.None;
            public EArgvFalg Flag = EArgvFalg.All;
            public bool canEdit = true;
        }
        [System.Serializable]
        public class AgentUnit
        {
            public uint customType = 0;
            public string name = "";
            public AgentArgv[] inputs;
            public AgentArgv[] outputs;
            public bool IsValid()
            {
                return !string.IsNullOrEmpty(name) && customType != 0;
            }
        }
        public AgentUnit[] vTriggerAgents;
        public AgentUnit[] vStepAgents;
        public AgentUnit[] vExecuteAgents;
    }
}
