/********************************************************************
生成日期:	11:07:2025
类    名: 	WarWorldObject
作    者:	HappLI
描    述:	一个战争世界数据体
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.War.Runtime
{
    public class WarWorldObject : ScriptableObject
    {
        public WarVariables warVariables = new WarVariables();
        public List<WarAgentData> warAgents = new List<WarAgentData>();
    }
}

