/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	RootsHandler
作    者:	HappLI
描    述:	根节点挂点
*********************************************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Framework.ED;
using System.Collections.Generic;
#endif
namespace Framework.Core
{
    public class RootsHandler : MonoBehaviour
    {
        [SerializeField]
        Transform actorsRoot = null;
        [SerializeField]
        Transform[] actorTypeRoots = null;
        [SerializeField]
        Transform particlesRoot = null;
        [SerializeField]
        Transform scenesRoot = null;

        [SerializeField]
        Transform themeRoot = null;

        static RootsHandler ms_Instance = null;

        private void Awake()
        {
            //GameObject.DontDestroyOnLoad(this);
            ms_Instance = this;
            DyncmicTransformCollects.Collect(gameObject.name, transform);
            if (actorsRoot == null)
            {
                actorsRoot = (new GameObject("ActorSystems")).transform;
                actorsRoot.SetParent(transform);
            }
            DyncmicTransformCollects.Collect(actorsRoot.name, actorsRoot);
            for (int i = 0; i < actorTypeRoots.Length; ++i)
            {
                if (!actorTypeRoots[i])
                {
                    actorTypeRoots[i] = (new GameObject(BaseUtil.stringBuilder.Append(i.ToString()).Append("s").ToString())).transform;
                    actorTypeRoots[i].SetParent(actorsRoot);
                }
                DyncmicTransformCollects.Collect(actorTypeRoots[i].name, actorTypeRoots[i]);
            }
            if (particlesRoot == null)
            {
                particlesRoot = (new GameObject("Particles")).transform;
                particlesRoot.SetParent(transform);
            }
            DyncmicTransformCollects.Collect(particlesRoot.name, particlesRoot);
            if (scenesRoot == null)
            {
                scenesRoot = (new GameObject("SceneRoots")).transform;
                scenesRoot.SetParent(transform);
            }
            DyncmicTransformCollects.Collect(scenesRoot.name, scenesRoot);

            if (themeRoot == null)
            {
                themeRoot = (new GameObject("ThemeRoot")).transform;
                themeRoot.SetParent(transform);
            }
            DyncmicTransformCollects.Collect(themeRoot.name, themeRoot);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            ms_Instance = null;
        }
        //------------------------------------------------------
        public static Transform ActorsRoot
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.actorsRoot;
            }
        }
        //------------------------------------------------------
        public static Transform[] ActorTypeRoots
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.actorTypeRoots;
            }
        }
        //------------------------------------------------------
        public static Transform FindActorRoot(int type)
        {
            if (ms_Instance == null || ms_Instance.actorTypeRoots == null || type >= ms_Instance.actorTypeRoots.Length) return null;
            return ms_Instance.actorTypeRoots[type];
        }
        //------------------------------------------------------
        public static Transform ParticlesRoot
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.particlesRoot;
            }
        }
        //------------------------------------------------------
        public static Transform ScenesRoot
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.scenesRoot;
            }
        }
        //------------------------------------------------------
        public static Transform ThemeRoot
        {
            get
            {
                if (ms_Instance == null) return null;
                return ms_Instance.themeRoot;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(RootsHandler), true)]
    public class RootsHandlerEditor : Editor
    {
        bool m_bEpxandActorTypeRoots = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RootsHandler rootHandler = target as RootsHandler;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actorsRoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("particlesRoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scenesRoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("themeRoot"));
            SerializedProperty actorTypeRoots = serializedObject.FindProperty("actorTypeRoots");
            if (actorTypeRoots != null && actorTypeRoots.isArray)
            {
                if(actorTypeRoots.arraySize == 0)
                {
                    actorTypeRoots.arraySize = (int)EActorType.Count;
                }
                else
                {
                    while (actorTypeRoots.arraySize > (int)EActorType.Count)
                    {
                        actorTypeRoots.DeleteArrayElementAtIndex(actorTypeRoots.arraySize - 1);
                    }
                    for (int i = actorTypeRoots.arraySize; i < (int)EActorType.Count; ++i)
                    {
                        actorTypeRoots.InsertArrayElementAtIndex(actorTypeRoots.arraySize);
                    }
                }
                m_bEpxandActorTypeRoots = EditorGUILayout.Foldout(m_bEpxandActorTypeRoots, "ActorTypeRoots");
                if(m_bEpxandActorTypeRoots)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < (int)EActorType.Count; ++i)
                    {
                        EditorGUILayout.PropertyField(actorTypeRoots.GetArrayElementAtIndex(i), new GUIContent(((EActorType)i).ToString()));
                    }
                    EditorGUI.indentLevel--;
                }
            }
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新"))
            {
                EditorUtility.SetDirty(rootHandler);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}

