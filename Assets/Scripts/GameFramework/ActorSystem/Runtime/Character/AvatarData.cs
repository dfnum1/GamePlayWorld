/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	AvatarData
作    者:	HappLI
描    述:	Avatar数据
*********************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.Core
{
    [System.Serializable]
    public class AvatarData : ScriptableObject
    {
        public Mesh partMesh = null;
        public Material partMaterial = null;
        public string[] bones;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(AvatarData))]
    public class AvatarDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AvatarData avatarData = (AvatarData)target;
            DrawDefaultInspector();
        }
        //--------------------------------------------------------
        [MenuItem("Assets/转为Avatar部件")]
        public static void CreateAvatarData()
        {
            var selects = Selection.gameObjects;
            if (selects == null || selects.Length <= 0)
                return;

            string path = EditorUtility.SaveFolderPanel("Create Avatar Data", Application.dataPath + "/Datas/Characters", "Avatars");
            if (string.IsNullOrEmpty(path)) return;
            path = path.Replace("\\", "/").Replace(Application.dataPath.Replace("\\", "/") + "/", "Assets/");
            for (int i = 0; i < selects.Length; i++)
            {
                GameObject go = selects[i];
                if (go == null) continue;
                SkinnedMeshRenderer skinRender = go.GetComponent<SkinnedMeshRenderer>();
                if (skinRender == null)
                    continue;
                string file = System.IO.Path.Combine(path, go.name + ".asset");
                AvatarData avatarData = CreateInstance<AvatarData>();
                avatarData.partMesh = skinRender.sharedMesh;
                avatarData.partMaterial = skinRender.sharedMaterial;

                // 计算骨骼名列表
                if (skinRender.bones != null && skinRender.bones.Length > 0)
                {
                    avatarData.bones = new string[skinRender.bones.Length];
                    for (int j = 0; j < skinRender.bones.Length; j++)
                    {
                        avatarData.bones[j] = skinRender.bones[j] != null ? skinRender.bones[j].name : string.Empty;
                    }
                }

                AssetDatabase.CreateAsset(avatarData, file);
            }
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
