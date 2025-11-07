#if USE_CUTSCENE
#if UNITY_EDITOR
/********************************************************************
生成日期:	06:30:2025
类    名: 	AssetDrawLogic
作    者:	HappLI
描    述:	资源面板逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.Cutscene.Runtime;
using Framework.ED;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Cutscene.Editor
{
    [EditorBinder(typeof(CutsceneEditor), "AssetRect", -1)]
    public class AssetDrawLogic : ACutsceneLogic
    {
        string m_AddNewName = "";
        CutsceneObject m_pCurrent = null;
        CutsceneGraph m_pCurrentGraph = null;
        Vector2 m_ScrollPosition = Vector2.zero;
        Vector2 m_DebugScrollPosition = Vector2.zero;
        bool m_bDebugExpand = false;
        string m_strSearchDebug = "";
        // TreeAssetView m_pAssetTree;
        //--------------------------------------------------------
        protected override void OnEnable()
        {
//             m_pAssetTree = new TreeAssetView(new string[] { "ID", "Cutscene" });
//             m_pAssetTree.BeginTreeData();
//             string[] cutscenes = AssetDatabase.FindAssets("t:CutsceneObject");
//             for(int i =0; i < cutscenes.Length; ++i)
//             {
//                 string path = AssetDatabase.GUIDToAssetPath(cutscenes[i]);
//                 string name = System.IO.Path.GetFileNameWithoutExtension(path);
//                 m_pAssetTree.AddData(new TreeAssetView.ItemData() { id = i, name = name, path = path});
//             }
//             m_pAssetTree.EndTreeData();
        }
        //--------------------------------------------------------
        public override void OnChangeSelect(object pData)
        {
            base.OnChangeSelect(pData);
            if (pData is CutsceneObject)
            {
                m_pCurrent = pData as CutsceneObject;
               if(m_pCurrent!=null) m_pCurrentGraph = m_pCurrent.GetCutsceneGraph();
                if (m_pCurrentGraph != null && m_pCurrentGraph.GetEnterCutscene()!=null)
                {
                    SetCutsceneData(m_pCurrentGraph.GetEnterCutscene(), false);
                }
            }
            else if(pData is CutsceneGraph)
            {
                m_pCurrentGraph = (CutsceneGraph)pData;
                if (m_pCurrentGraph != null && m_pCurrentGraph.GetEnterCutscene() != null)
                {
                    SetCutsceneData(m_pCurrentGraph.GetEnterCutscene(), false);
                }
            }
        }
        //--------------------------------------------------------
        public override void OnSaveChanges()
        {
            if (IsRuntimePlayingCutscene())
                return;

            if(m_pCurrent != null)
            {
                m_pCurrent.Save();
                EditorUtility.SetDirty(m_pCurrent);
                AssetDatabase.SaveAssetIfDirty(m_pCurrent);
            }
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(IsRuntimePlayingCutscene());
            Rect rect = GetRect();
            float panelHeight = rect.height - 20;
            float cutsceneListHeight = panelHeight / 2;
            float debugHeight = panelHeight / 2;
            if (!m_bDebugExpand)
            {
                cutsceneListHeight = panelHeight - 30;
                debugHeight = 30;
            }

            GUILayout.BeginArea(new Rect(rect.x, rect.y+20, rect.width, cutsceneListHeight));
            if(m_pCurrentGraph != null)
            {
                if(m_pCurrentGraph.vCutscenes!=null)
                {
                    m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                    for (int i = 0; i < m_pCurrentGraph.vCutscenes.Count;)
                    {
                        var cutscene = m_pCurrentGraph.vCutscenes[i].cutSceneData;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(cutscene.id.ToString(), GUILayout.Width(50));
                        string name = EditorGUILayout.DelayedTextField(cutscene.name);
                        if(name != cutscene.name)
                        {
                            if(m_pCurrentGraph.FindCutscene(name) == null)
                            {
                                RegisterUndoData();
                                cutscene.name = name;
                            }
                        }
                        if (GUILayout.Button("编辑", new GUILayoutOption[] { GUILayout.Width(50) }))
                        {
                            SetCutsceneData(cutscene, false);
                        }

                        bool bRemove = false;
                        if (GUILayout.Button("移除", new GUILayoutOption[] { GUILayout.Width(50) }))
                        {
                            if(EditorUtility.DisplayDialog("提示","确认要移除[" + cutscene.name+"]?", "移除","再想想"))
                            {
                                m_pCurrentGraph.vCutscenes.RemoveAt(i);
                                bRemove = true;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (!bRemove) ++i;
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(GetRect().width-10));
                m_AddNewName = EditorGUILayout.TextField(m_AddNewName);
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_AddNewName) || m_pCurrentGraph.FindCutscene(m_AddNewName)!=null);
                if (GUILayout.Button("添加Cutscene"))
                {
                    CutsceneData data = new CutsceneData();
                    data.name = m_AddNewName;
                    data.id = GeneratorID();
                    m_pCurrentGraph.AddCutscene(data);
                    m_AddNewName = "";
                    SetCutsceneData(data, false);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            //             if (m_pAssetTree != null)
            //             {
            //                 m_pAssetTree.GetColumn(0).width = window.AssetRect.width / 3;
            //                 m_pAssetTree.GetColumn(1).width = window.AssetRect.width* 2/ 3;
            //                 m_pAssetTree.searchString = EditorGUILayout.TextField("搜索", m_pAssetTree.searchString);
            //                 m_pAssetTree.OnGUI(new Rect(0, 20, window.AssetRect.width, window.AssetRect.height-20));
            //             }
            GUILayout.EndArea();

            {
                UIDrawUtils.DrawColorLine(new Vector2(rect.xMin, rect.y + 20 + cutsceneListHeight), new Vector2(rect.xMax, rect.y + 20 + cutsceneListHeight), new Color(1, 0, 1, 0.5f));
                GUILayout.BeginArea(new Rect(rect.x, rect.y + 20 + cutsceneListHeight, rect.width, debugHeight));
                m_bDebugExpand = EditorGUILayout.Foldout(m_bDebugExpand, "调试设置");
                if (m_bDebugExpand)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("搜索", GUILayout.Width(30));
                    m_strSearchDebug = EditorGUILayout.TextField(m_strSearchDebug);
                    EditorGUILayout.EndHorizontal();
                    m_DebugScrollPosition = EditorGUILayout.BeginScrollView(m_DebugScrollPosition);
                    EditorGUI.indentLevel++;
                    DrawDebugPanel();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndArea();
            }


            UIDrawUtils.DrawColorLine(new Vector2(rect.xMin, rect.y + 20), new Vector2(rect.xMax, rect.y + 20), new Color(1, 1, 1, 0.5f));
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width,20));
            GUILayout.Label("数据面板", TextUtil.panelTitleStyle);
            GUILayout.EndArea();

            EditorGUI.EndDisabledGroup();
        }
        //-----------------------------------------------------
        void DrawDebugPanel()
        {
            ACutsceneEditor pEditor = GetOwner<ACutsceneEditor>();
            var cutsceneInstance = pEditor.GetCutsceneInstance();
            EditorGUI.BeginDisabledGroup(cutsceneInstance == null);
            EditorGUILayout.LabelField("剪辑:");
            var dataAttris = DataUtils.GetClipAttrs();
            if(dataAttris!=null)
            {
                ClipAttriData customClipAttri = null;
                foreach (var db in dataAttris)
                {
                    if (db.type == typeof(CutsceneCustomClip))
                    {
                        customClipAttri = db;
                        continue;
                    }
                    if(!string.IsNullOrEmpty(m_strSearchDebug) && !(db.pAttri.name.Contains(m_strSearchDebug) || m_strSearchDebug.Contains(db.pAttri.name)))
                    {
                        continue;
                    }

                    EditorGUI.BeginChangeCheck();
                    db.isPlayable = EditorGUILayout.Toggle(db.pAttri.name, db.isPlayable);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (cutsceneInstance != null)
                            cutsceneInstance.SetPlayableToggle(EDataType.eClip, db.typeId, db.isPlayable);
                    }
                }
                if (customClipAttri != null)
                {
                    var customEvents = CustomAgentUtil.GetClipList();
                    foreach (var db in customEvents)
                    {
                        if (!string.IsNullOrEmpty(m_strSearchDebug) && !(db.name.Contains(m_strSearchDebug) || m_strSearchDebug.Contains(db.name)))
                        {
                            continue;
                        }
                        EditorGUI.BeginChangeCheck();
                        db.playAble = EditorGUILayout.Toggle(db.name, db.playAble);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (cutsceneInstance != null)
                                cutsceneInstance.SetPlayableToggle(EDataType.eClip, customClipAttri.typeId, db.customType, db.playAble);
                        }
                    }
                }
            }
            EditorGUILayout.LabelField("事件:");
            var evtAttris = DataUtils.GetEventAttrs();
            if (evtAttris != null)
            {
                EventAttriData customEventAttri = null;
                foreach (var db in evtAttris)
                {
                    if (db.type == typeof(CutsceneCustomEvent))
                    {
                        customEventAttri = db;
                        continue;
                    }
                    if (!string.IsNullOrEmpty(m_strSearchDebug) && !(db.pAttri.name.Contains(m_strSearchDebug) || m_strSearchDebug.Contains(db.pAttri.name)))
                    {
                        continue;
                    }

                    EditorGUI.BeginChangeCheck();
                    db.isPlayable = EditorGUILayout.Toggle(db.pAttri.name, db.isPlayable);
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(cutsceneInstance!=null)
                            cutsceneInstance.SetPlayableToggle(EDataType.eEvent, db.typeId, db.isPlayable);
                    }
                }
                if (customEventAttri != null)
                {
                    var customEvents = CustomAgentUtil.GetEventList();
                    foreach (var db in customEvents)
                    {
                        if (!string.IsNullOrEmpty(m_strSearchDebug) && !(db.name.Contains(m_strSearchDebug) || m_strSearchDebug.Contains(db.name)))
                        {
                            continue;
                        }
                        EditorGUI.BeginChangeCheck();
                        db.playAble = EditorGUILayout.Toggle(db.name, db.playAble);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (cutsceneInstance != null)
                                cutsceneInstance.SetPlayableToggle(EDataType.eEvent, customEventAttri.typeId, db.customType, db.playAble);
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        //-----------------------------------------------------
        ushort GeneratorID()
        {
            ushort id = 1;
            HashSet<ushort> vSets = new HashSet<ushort>();
            if(m_pCurrentGraph!=null)
            {
                if (m_pCurrentGraph.vCutscenes != null)
                {
                    foreach (var db in m_pCurrentGraph.vCutscenes)
                    {
                        vSets.Add(db.cutSceneData.id);
                    }
                }
            }

            int stackCnt = 65535;
            while (vSets.Contains(id))
            {
                id++;
                stackCnt--;
                if (stackCnt <= 0) break;
            }
            return id;
        }
    }
}

#endif
#endif
#endif