/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	NumberModel
作    者:	HappLI
描    述:	数字模型
*********************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public class NumberModel : AInstanceAble
    {
        public Vector3 size = Vector3.one;
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NumberModel), true)]
    public class NumberModelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            NumberModel transRef = target as NumberModel;
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            transRef.size = EditorGUILayout.Vector3Field("大小", transRef.size);
            if(GUILayout.Button("SetModelSize", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                var filter = transRef.GetComponent<MeshFilter>();
                if (filter == null) filter = transRef.GetComponentInChildren<MeshFilter>();
                if (filter && filter.sharedMesh)
                {
                   var scale = filter.transform.rotation * filter.transform.localScale;
                    transRef.size = filter.transform.rotation * filter.sharedMesh.bounds.size;
                    transRef.size = Vector3.Scale(transRef.size, scale);
                }
                EditorUtility.SetDirty(transRef);
            }
            if (EditorGUI.EndChangeCheck())
                ED.EditorUtil.RepaintPlayModeView();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
        public void OnSceneGUI()
        {
            NumberModel number = target as NumberModel;
            Handles.DrawWireCube(number.transform.position, number.size);
        }
    }
#endif
}