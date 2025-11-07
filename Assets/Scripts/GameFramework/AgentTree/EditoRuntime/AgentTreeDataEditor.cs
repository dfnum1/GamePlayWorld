using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.AT
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AAgentTreeData),true)]
    public class AgentTreeDataEditor : UnityEditor.Editor
    {
        float[] inspectorTabWidth = new float[6];
        HashSet<ushort> m_vSets = new HashSet<ushort>();
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AAgentTreeData uis = target as AAgentTreeData;
            UnityEditor.EditorGUILayout.BeginHorizontal();
            uis.Data.bEnable = UnityEditor.EditorGUILayout.Toggle("启用", uis.Data.bEnable);
            if (GUILayout.Button("编辑"))
            {
                AgentTreeEditor.Editor(uis, null);
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
