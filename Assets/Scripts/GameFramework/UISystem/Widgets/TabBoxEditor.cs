#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UserInterfaceEditor
作    者:	HappLI
描    述:	UI 序列化对象容器，用于界面绑定操作对象 编辑操作
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Reflection;

namespace Framework.UI
{
    [CustomEditor(typeof(TabBox), true)]
    public class TabBoxEditor : Editor
    {
        static void DestroyChilds(UnityEngine.GameObject pObj)
        {
            if (pObj == null) return;
            UnityEngine.Transform pTransform = pObj.transform;
            int count = pTransform.childCount;
            while (pTransform.childCount > 0)
            {
                GameObject.DestroyImmediate(pTransform.GetChild(0).gameObject);
            }
            pTransform.DetachChildren();
        }
        //------------------------------------------------------
        public static void Preview(TabBox box, bool bAuto = false)
        {
            if(bAuto)
            {
                if (box.transform.childCount > 0)
                    CancelPreview(box);
                else
                    Preview(box, false);
                return;
            }

            GameObject pAset = null;
            if (box.type == TabBox.EType.File)
            {
                GameObject pAsset = AssetDatabase.LoadAssetAtPath<GameObject>(box.strPrefabFile);
            }
            else
                pAset = box.refPrefab;
            if (pAset == null)
            {
                EditorUtility.DisplayDialog("提示", "资源不存在!", "好的");
            }
            if (pAset)
            {
                for (int i = 0; i < box.nTabCount; ++i)
                {
                    GameObject pIns = GameObject.Instantiate<GameObject>(pAset);
                    pIns.transform.SetParent(box.transform);
                    pIns.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor;
                }
            }
        }
        //------------------------------------------------------
        static void CancelPreview(TabBox box)
        {
            DestroyChilds(box.gameObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TabBox box = target as TabBox;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nTabCount"), true);

            if (box.type == TabBox.EType.File)
            {
                GameObject pAsset = null;
                if (box.refPrefab != null && string.IsNullOrEmpty(box.strPrefabFile))
                    pAsset = box.refPrefab;
                else
                    pAsset = AssetDatabase.LoadAssetAtPath<GameObject>(box.strPrefabFile);

                pAsset = EditorGUILayout.ObjectField("Prefab", pAsset, typeof(GameObject), true) as GameObject;

                if (pAsset != null) box.strPrefabFile = AssetDatabase.GetAssetPath(pAsset);
                else
                    box.strPrefabFile = "";

                box.refPrefab = null;
            }
            else
            {
                box.refPrefab = EditorGUILayout.ObjectField("Prefab", box.refPrefab, typeof(GameObject), true) as GameObject;
                box.strPrefabFile = null;
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("预览"))
            {
                Preview(box, true);
            }
            if (GUILayout.Button("刷新"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
}
#endif