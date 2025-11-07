/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	CharacterComponent
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Plugin.AT;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Framework.Core
{
    public class CharacterComponent : ActorComponent
    {
        public Matrix4x4[] bindposes;
        public Transform[] skeletons;
    }

#if UNITY_EDITOR && USE_ACTORSYSTEM
    [CustomEditor(typeof(CharacterComponent))]
    //[CanEditMultipleObjects]
    class CharacterComponentEditor : ActorSystem.ED.ActorComponentEditor
    {
        public SkinnedMeshRenderer m_pTPoseMesh;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_pTPoseMesh = EditorGUILayout.ObjectField("T-pose Mesh", m_pTPoseMesh, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            if (m_pTPoseMesh!=null && GUILayout.Button("T-pose"))
            {
                CharacterComponent character = target as CharacterComponent;
                string path = AssetDatabase.GetAssetPath(m_pTPoseMesh);
                UnityEditor.ModelImporter import = AssetImporter.GetAtPath(path) as UnityEditor.ModelImporter;

                Transform roots = character.gameObject.transform.Find("root");

                Dictionary<string, Transform> vBones = new Dictionary<string, Transform>();
                Transform[] boneArray = null;
                if (roots != null)
                {
                    List<Transform> allBones = new List<Transform>();
                    CollectBones(roots, allBones);
                    boneArray = allBones.ToArray();
                }
                for(int i =0; i < boneArray.Length; ++i)
                {
                    vBones[boneArray[i].name] = boneArray[i];
                }
                List<Transform> bones = new List<Transform>();
                for(int i =0; i < m_pTPoseMesh.bones.Length; ++i)
                {
                    if (m_pTPoseMesh.bones[i] != null)
                    {
                        if(vBones.TryGetValue(m_pTPoseMesh.bones[i].name, out var bone))
                        {
                            bones.Add(bone);
                        }
                    }
                }
                character.skeletons = bones.ToArray();
                character.bindposes = m_pTPoseMesh.sharedMesh.bindposes;
                UnityEditor.EditorUtility.SetDirty(target);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(target);
            }
        }
        //--------------------------------------------------------
        private void CollectBones(Transform parent, List<Transform> bones)
        {
            if (parent == null) return;
            bones.Add(parent);
            for (int i = 0; i < parent.childCount; i++)
            {
                CollectBones(parent.GetChild(i), bones);
            }
        }
    }
#endif
}