#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public interface IAgentTreeEditor
    {
        AgentTreeCoreData GetATCoreData();
        Dictionary<int, GraphNode> GetGraphNodes();
        GraphNode GetGraphNode(int guid);
        Dictionary<int, StructData> GetVariableStructs();
        ATExportNodeAttrData GetActionNodeAttr(int actionType);
        APINode GetAPINode(long guidKey);
        void ATFuncContextMenu(Vector2 mousePos, System.Object graphNode);
        void AdjustMaxGuid();
        AgentTree GetCurrentAgentTree();

        void PopEquivalence(ArgvPort port);
        void VaribleReName(Variable variable);
    }
}
#endif