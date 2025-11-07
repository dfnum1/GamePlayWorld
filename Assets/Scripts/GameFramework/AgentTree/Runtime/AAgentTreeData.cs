/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	AAgentTreeData
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    public enum ETaskType
    {
        Start =0,
        Exit = 1,
        Tick =2,
        KeyInput =3,
        MouseInput =4,
        Custom = 100,
        Count,
    }
    //------------------------------------------------------
    [System.Serializable]
    public class Task
    {
        public ETaskType type;
        // public VariableSerializes Locals;
        public EnterNode EnterNode;

        public void Copy(Task pOther, bool bIncludeGuid=true)
        {
            type = pOther.type;
            EnterNode = new EnterNode();
            EnterNode.Copy(pOther.EnterNode, bIncludeGuid);
        }
    }
    //------------------------------------------------------
    [System.Serializable]
    public class StructData
    {
        public string structName;
        public List<int> variables;
#if UNITY_EDITOR
        [System.NonSerialized]
        public List<Variable> runtimeVars;
        [System.NonSerialized]
        public bool Expand;
#endif
    }
    //------------------------------------------------------
    [System.Serializable]
    public class AgentTreeCoreData
    {
        public bool bEnable;
        [HideInInspector]
        public VariableSerializes Locals;
        [HideInInspector]
        public List<ActionNode> vNodes;
        [HideInInspector]
        public List<APINode> vATApis;
        [HideInInspector]
        public List<Task> Tasks;

        [HideInInspector]
        public List<StructData> StructDatas;

        [HideInInspector]
        public List<RefPort> RefPorts;

#if UNITY_EDITOR
        [HideInInspector]
        public List<TransferDot> transferDots;
#endif
        public void Clear()
        {
            if (Locals != null) Locals.Clear();
            if (vNodes != null) vNodes.Clear();
            if (vATApis != null) vATApis.Clear();
            if (Tasks!=null) Tasks.Clear();
            if (StructDatas != null) StructDatas.Clear();
            if (RefPorts != null) RefPorts.Clear();
#if UNITY_EDITOR
            if(transferDots!=null) transferDots.Clear();
#endif
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public AgentTreeCoreData Clone()
        {
            return (AgentTreeCoreData)this.MemberwiseClone();
        }
#endif
    }
    //------------------------------------------------------
    //[CreateAssetMenu]
    public abstract class AAgentTreeData : ScriptableObject
    {
        public AgentTreeCoreData Data = null;

#if UNITY_EDITOR
        static System.Type ms_pInheritType = null;
        public static AAgentTreeData CreateInstance()
        {
            if (ms_pInheritType == null) ms_pInheritType = AgentTreeEditorUtils.FindInheirtTypeType<AAgentTreeData>();
            if (ms_pInheritType == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("tips", "请定义一个类继承为AAgentTreeData", "Ok");
                return null;
            }
            return ScriptableObject.CreateInstance(ms_pInheritType) as AAgentTreeData;
        }
#endif
    }
}