/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	MeshPivotCenter
作    者:	HappLI
描    述:	mesh 某点中心归0
*********************************************************************/
using UnityEditor;
using UnityEngine;

namespace TopGame.ED
{
    public class MeshPivotCenter
    {
		[MenuItem("Assets/Mesh Center Pivot")]
		static void CenterSelectedMeshPivot()
		{
			var selected = Selection.activeGameObject;
			if (selected == null)
			{
				Debug.LogWarning("请先选中一个带 MeshFilter 的物体。");
				return;
			}

			var meshFilter = selected.GetComponent<MeshFilter>();
			if (meshFilter == null || meshFilter.sharedMesh == null)
			{
				Debug.LogWarning("选中的物体没有 MeshFilter 或 Mesh。");
				return;
			}

			Mesh mesh = meshFilter.sharedMesh;
			Vector3[] vertices = mesh.vertices;

			// 计算中心点
			Vector3 center = Vector3.zero;
			foreach (var v in vertices)
				center += v;
			center /= vertices.Length;

			// 顶点归零中心
			for (int i = 0; i < vertices.Length; i++)
				vertices[i] -= center;
			mesh.vertices = vertices;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();

			// 让物体位置补偿中心偏移
			Undo.RecordObject(selected.transform, "Center Mesh Pivot");
			selected.transform.position += selected.transform.TransformVector(center);

			// 标记 mesh 变更
			EditorUtility.SetDirty(mesh);
			Debug.Log("锚点已归到中心。");
		}        
    }
}