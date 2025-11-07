/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TransformRef
作    者:	HappLI
描    述:	动态绑点
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public static class DyncmicTransformCollects
    {
        public struct DynamicNode
        {
            public Transform transform;
            public Renderer renderer;
            public Core.AComSerialized serialzed;
            public DynamicNode(Transform trans, Renderer render, Core.AComSerialized serialzed)
            {
                this.transform = trans;
                this.renderer = render;
                this.serialzed = serialzed;
            }
            public static DynamicNode DEF = new DynamicNode() { transform = null, renderer = null };
        }
        static Dictionary<string, DynamicNode> ms_Refs = new Dictionary<string, DynamicNode>();
        static Dictionary<int, DynamicNode> ms_RefByGUIDs = new Dictionary<int, DynamicNode>();
        //------------------------------------------------------
        public static void Collect(string name, Transform trans, Renderer render = null, Core.AComSerialized serialzed = null)
        {
            ms_Refs[name] = new DynamicNode(trans, render, serialzed);
        }
        //------------------------------------------------------
        public static void UnCollect(string name)
        {
            ms_Refs.Remove(name);
        }
        //------------------------------------------------------
        public static void Collect(int guid, Transform trans, Renderer render = null, Core.AComSerialized serialzed = null)
        {
            ms_RefByGUIDs[guid] = new DynamicNode(trans, render, serialzed);
        }
        //------------------------------------------------------
        public static void UnCollect(int guid)
        {
            ms_RefByGUIDs.Remove(guid);
        }
        //------------------------------------------------------
        public static DynamicNode FindNode(string strName)
        {
            if (string.IsNullOrEmpty(strName)) return DynamicNode.DEF;
            DynamicNode node;
            if (ms_Refs.TryGetValue(strName, out node)) return node;
            return DynamicNode.DEF;
        }
        //------------------------------------------------------
        public static DynamicNode FindNode(int guid)
        {
            if (guid == 0) return DynamicNode.DEF;
            DynamicNode node;
            if (ms_RefByGUIDs.TryGetValue(guid, out node)) return node;
            return DynamicNode.DEF;
        }
        //------------------------------------------------------
        public static Transform FindTransformByName(string strName)
        {
            if (string.IsNullOrEmpty(strName)) return null;
            DynamicNode node;
            if (ms_Refs.TryGetValue(strName, out node)) return node.transform;
            return null;
        }
        //------------------------------------------------------
        public static Transform FindTransformByGUID(int Guid)
        {
            DynamicNode node;
            if (ms_RefByGUIDs.TryGetValue(Guid, out node)) return node.transform;
            return null;
        }
        //------------------------------------------------------
        public static void ActiveNodeByGUID(int Guid, bool bActive, bool bPosActive = true)
        {
            if (Guid==0) return;
            DynamicNode node;
            if (ms_RefByGUIDs.TryGetValue(Guid, out node))
            {
                if (node.renderer)
                {
                    node.renderer.enabled = bActive;
                }
                else
                {
#if !USE_SERVER
                    if (bPosActive) BaseUtil.SetActive(node.transform, bActive);
                    else if (node.transform) node.transform.gameObject.SetActive(bActive);
#endif
                }
            }
        }
        //------------------------------------------------------
        public static void ActiveNodeByName(string strName, bool bActive, bool bPosActive = true)
        {
            if (string.IsNullOrEmpty(strName)) return;
            DynamicNode node;
            if (ms_Refs.TryGetValue(strName, out node))
            {
                if (node.renderer)
                {
                    node.renderer.enabled = bActive;
                }
                else
                {
#if !USE_SERVER
                    if (bPosActive) BaseUtil.SetActive(node.transform, bActive);
                    else if(node.transform) node.transform.gameObject.SetActive(bActive);
#endif
                }
            }
        }
    }
    //------------------------------------------------------
    //TransformRef
    //------------------------------------------------------
    [ExecuteInEditMode]
    public class TransformRef : MonoBehaviour
    {
        public string   strSymbole = "";
        public int      GUID = 0;

        public Transform refTransform;
        public Renderer refRenderer;
        public Core.AComSerialized serialized;
        //------------------------------------------------------
        private void Awake()
        {
            RefreshRef();
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            RefreshRef();
        }
        //------------------------------------------------------
        void RefreshRef()
        {
            if (string.IsNullOrEmpty(strSymbole))
                DyncmicTransformCollects.Collect(name, refTransform ? refTransform : transform, refRenderer, serialized);
            else
                DyncmicTransformCollects.Collect(strSymbole, refTransform ? refTransform : transform, refRenderer, serialized);
            if (GUID != 0)
            {
                DyncmicTransformCollects.Collect(GUID, refTransform ? refTransform : transform, refRenderer, serialized);
            }
        }
        //------------------------------------------------------
        void OnDestroy()
        {
            if (string.IsNullOrEmpty(strSymbole))
                DyncmicTransformCollects.UnCollect(name);
            else
                DyncmicTransformCollects.UnCollect(strSymbole);
            if (GUID != 0)
            {
                DyncmicTransformCollects.UnCollect(GUID);
            }
        }
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransformRef), true)]
    public class ATransformRefEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TransformRef transRef = target as TransformRef;
            GUILayout.BeginHorizontal();
            transRef.strSymbole = EditorGUILayout.TextField("标识名", transRef.strSymbole);
            if(GUILayout.Button("缺省", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                transRef.strSymbole = target.name;
            }
            GUILayout.EndHorizontal();
            transRef.GUID = EditorGUILayout.IntField("标识ID", transRef.GUID);
            transRef.refTransform = EditorGUILayout.ObjectField("引用对象", transRef.refTransform, typeof(Transform), true) as Transform;
            transRef.refRenderer = EditorGUILayout.ObjectField("绑定渲染器", transRef.refRenderer, typeof(Renderer), true) as Renderer;
            transRef.serialized = EditorGUILayout.ObjectField("序列化器", transRef.serialized, typeof(AComSerialized), true) as AComSerialized;

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}