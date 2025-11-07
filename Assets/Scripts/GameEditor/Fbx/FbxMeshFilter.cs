/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	FbxAnimationFilter
作    者:	HappLI
描    述:	fbx Mesh 剥离
*********************************************************************/
using UnityEditor;
using UnityEngine;

namespace TopGame.ED
{
    public class FbxMeshFilter
    {
        [MenuItem("Assets/剥离Mesh")]
        static void FilterMesh()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return;
            for (int i = 0; i < Selection.gameObjects.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(Selection.gameObjects[i]);
                ImportFbx(Selection.gameObjects[i], AssetImporter.GetAtPath(path) as ModelImporter);
            }
        }
        //------------------------------------------------------
        static void ImportFbx(GameObject targetObj, ModelImporter target)
        {
            string path = AssetDatabase.GetAssetPath(targetObj.GetInstanceID());
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
            Matrix4x4 world = targetObj.transform.localToWorldMatrix;
            world *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 2);
            for (int i = 0; i < objs.Length; ++i)
            {
                if(objs[i] is Mesh)
                {
                    Mesh mesh = objs[i] as Mesh;

                    Mesh createmesh = new Mesh();
                    Vector3[] vers = new Vector3[mesh.vertices.Length];
                    for (int j = 0; j < mesh.vertices.Length; ++j)
                    {
                        vers[j] = world.MultiplyPoint(mesh.vertices[j]);
                    }
                    createmesh.vertices = vers;
                    createmesh.uv = mesh.uv;
                    createmesh.triangles = mesh.triangles;
                    createmesh.name = mesh.name;
                    AssetDatabase.CreateAsset(createmesh, System.IO.Path.GetDirectoryName(path).Replace("\\", "/") + "/" + mesh.name + ".mesh");
                }
            }
        }
    }
}