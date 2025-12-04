/********************************************************************
生成日期:	06:30:2025
类    名: 	AgentTreeData
作    者:	HappLI
描    述:	过场动画行为树
*********************************************************************/
#if UNITY_EDITOR
using Framework.AT.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace Framework.AT.Runtime
{
    //-----------------------------------------------------
    //! AgentTreeObject 
    //-----------------------------------------------------
    [CreateAssetMenu(menuName = "GamePlay/蓝图脚本")]
    public class AgentTreeObject : AAgentTreeObject
    {
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(AgentTreeObject))]
    public class AgentTreeObjectEditor : AAgentTreeObjectEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}