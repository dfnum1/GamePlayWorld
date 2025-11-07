/********************************************************************
生成日期:	24:7:2019   11:12
类    名: 	CsvBuilderTool
作    者:	HappLI
描    述:	Csv 解析代码自动生成器
*********************************************************************/
using Framework.Data;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TopGame.ED
{
    [CustomEditor(typeof(CsvConfig))]
    [CanEditMultipleObjects]
    public class CsvConfigEditor :UnityEditor.Editor
    {
        float[] inspectorTabWidth = new float[7];
        bool m_bExpandCommonAssets = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CsvConfig pCsv = target as CsvConfig;
            Color color = GUI.color;
            //   pCsv.TileLine = EditorGUILayout.IntField("开始行", pCsv.TileLine);
            //   pCsv.bBinary = EditorGUILayout.Toggle("二进制", pCsv.bBinary);

            pCsv.dllRead = EditorGUILayout.Toggle("c++读取", pCsv.dllRead);
            m_bExpandCommonAssets = EditorGUILayout.Foldout(m_bExpandCommonAssets, "通用配置资源");
            if (m_bExpandCommonAssets)
            {
                List<string> vFiles = null;
                if (pCsv.CommonAssets != null) vFiles = new List<string>(pCsv.CommonAssets);
                else vFiles = new List<string>();
                for (int i = 0; i < vFiles.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    if (vFiles[i] == null)
                    {
                        GUI.color = Color.red;
                    }
                    else
                        GUI.color = color;

                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(vFiles[i]);
                    asset = EditorGUILayout.ObjectField(asset, typeof(UnityEngine.Object), false);
                    if (GUILayout.Button("删除"))
                    {
                        vFiles.RemoveAt(i);
                        break;
                    }
                    if (asset)
                        vFiles[i] = AssetDatabase.GetAssetPath(asset);
                    else
                        vFiles[i] = null;
                    GUILayout.EndHorizontal();
                }
                if(GUILayout.Button("添加"))
                {
                    vFiles.Add(null);
                }
                pCsv.CommonAssets = vFiles.ToArray();
            }
            GUI.color = color;

            if (inspectorTabWidth.Length == 7)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("数据类型", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[0]) });
                GUILayout.Label("资源对象", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[1]) });
                GUILayout.Label("c#Export", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[2]) });
                GUILayout.Label("c++Export", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[3]) });
                GUILayout.Label("AT", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[3]) });
                GUILayout.Label("", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[4]) });
                GUILayout.Label("", new GUILayoutOption[] { GUILayout.MaxWidth(inspectorTabWidth[5]) });
                GUILayout.EndHorizontal();

            }

            HashSet<int> vAssets = new HashSet<int>();
            List<CsvAsset> vData = null;
            if (pCsv.Assets == null)
                vData = new List<CsvAsset>();
            else vData = new List<CsvAsset>(pCsv.Assets);
            for(int i = 0; i < vData.Count; ++i)
            {
                if (vData[i].Asset == null || vAssets.Contains(vData[i].nHash)) GUI.color = Color.red;
                else GUI.color = color;

                CsvAsset csvAsset = vData[i];
                vAssets.Add(csvAsset.nHash);
                GUILayout.BeginHorizontal();
                csvAsset.type = (EDataType)EditorGUILayout.EnumPopup(csvAsset.type);
                if (i == 0) inspectorTabWidth[0] = GUILayoutUtility.GetLastRect().width;
                csvAsset.Asset = EditorGUILayout.ObjectField(csvAsset.Asset, typeof(TextAsset),false) as TextAsset;
                string path = null;
                if(csvAsset.Asset) path = AssetDatabase.GetAssetPath(csvAsset.Asset);
                if (i == 0) inspectorTabWidth[1] = GUILayoutUtility.GetLastRect().width;
                if (csvAsset.type == EDataType.None)
                {
                    string suffix = path != null ? Path.GetExtension(path) : "";
                    if (suffix.CompareTo(".json") == 0) csvAsset.type = EDataType.Json;
                    else if (suffix.CompareTo(".bytes") == 0) csvAsset.type = EDataType.Binary;
                    else if (suffix.CompareTo(".csv") == 0) csvAsset.type = EDataType.Csv;
                }
                if (csvAsset.Asset != null)
                {
                    path = AssetDatabase.GetAssetPath(csvAsset.Asset);
                    csvAsset.nHash = Animator.StringToHash("csvdata_" + System.IO.Path.GetFileNameWithoutExtension(path).ToLower());
                }
                else
                    csvAsset.nHash = 0;

                csvAsset.csharpParse = EditorGUILayout.Toggle(csvAsset.csharpParse);
                if (i == 0) inspectorTabWidth[2] = GUILayoutUtility.GetLastRect().width;

                csvAsset.dllParse = EditorGUILayout.Toggle(csvAsset.dllParse);
                if (i == 0) inspectorTabWidth[3] = GUILayoutUtility.GetLastRect().width;

                csvAsset.exportAT = EditorGUILayout.Toggle(csvAsset.exportAT);
                if (i == 0) inspectorTabWidth[4] = GUILayoutUtility.GetLastRect().width;

                if (GUILayout.Button("删除", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    vData.RemoveAt(i);
                    break;
                }
                if (i == 0) inspectorTabWidth[5] = GUILayoutUtility.GetLastRect().width;

                if (GUILayout.Button("刷新", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    if (csvAsset.Asset != null)
                    {
                        path = AssetDatabase.GetAssetPath(csvAsset.Asset);
                        csvAsset.nHash = Animator.StringToHash("csvdata_"+System.IO.Path.GetFileNameWithoutExtension(path).ToLower());
                    }
                    else
                        csvAsset.nHash = 0;
                }
                if (i == 0) inspectorTabWidth[6] = GUILayoutUtility.GetLastRect().width;
                path = AssetDatabase.GetAssetPath(csvAsset.Asset);
                csvAsset.nHash = Animator.StringToHash("csvdata_" + System.IO.Path.GetFileNameWithoutExtension(path).ToLower());

                GUILayout.EndHorizontal();

                vData[i] = csvAsset;
            }
            GUI.color = color;
            if (GUILayout.Button("添加"))
            {
                vData.Add(new CsvAsset() { csharpParse = true, dllParse = false });
            }
            pCsv.Assets = vData.ToArray();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("刷新保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            if (GUILayout.Button("导出代码"))
            {
                if(EditorUtility.DisplayDialog("二次确认", "是否自动导出代码?", "导出", "取消"))
                {
                    CsvBuilderTool.BuildCode();
                }
            }
        }
    }
}

