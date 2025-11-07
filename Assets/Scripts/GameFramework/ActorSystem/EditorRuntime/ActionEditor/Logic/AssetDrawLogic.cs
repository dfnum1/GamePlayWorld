#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:06:2023
类    名: 	AssetDrawLogic
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Core;
using Framework.ED;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using static Framework.ED.TreeAssetView;

namespace ActorSystem.ED
{
    [EditorBinder(typeof(ActionEditorWindow), "AssetRect")]
    public class AssetDrawLogic : ActionEditorLogic
    {
        enum ETab
        {
            Info,
            AvatarMask,
            CommonAction,
            TimelineAction,
        }
        static string[] TABS = new string[] { "基本信息","AvatarMask", "常规动作", "Timeline动作" };
        ETab m_Tab = ETab.Info;
        public class GraphItem : TreeAssetView.ItemData
        {
            public ActorTimelineAction asset;
            public override Color itemColor()
            {
                return Color.white;
            }
        }
        TreeAssetView m_pActionTimelineTree;
        TreeAssetView m_pCommonTimlineTree;
        GraphItem m_pSelectData;

        uint m_addTimelineAction = 0;
        int m_nAddLayerMask = 0;

        GameObject m_pActorPrefab = null;
        Actor m_pActor = null;
        ActorComponent m_pActorComp = null;
        Vector2 m_CommonActionList = Vector2.zero;
        Vector2 m_TimelineActionList = Vector2.zero;
        Dictionary<int, bool> m_vCommonActionExpand = new Dictionary<int, bool>();
        //--------------------------------------------------------
        protected override void OnEnable()
        {
            m_pActionTimelineTree = new TreeAssetView(new string[] { "ID", "动作名", "类型组", "Tag" });
            m_pActionTimelineTree.OnCellDraw += OnDrawTimelineAction;
            m_pActionTimelineTree.OnSelectChange = OnTimlineTreeItemSelect;
            m_pActionTimelineTree.OnItemDoubleClick = OnTimlineTreeItemSelect;
            RefreshTimelineAction();
        }
        //--------------------------------------------------------
        void OnTimlineTreeItemSelect(TreeAssetView.ItemData item)
        {
            m_pSelectData = item as GraphItem;
#if USE_CUTSCENE
            GetOwner().OnChangeSelect(m_pSelectData.asset.GetCutsceneGraph(true));
#endif
        }
        //--------------------------------------------------------
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_pActor = null;
        }
        //--------------------------------------------------------
        public new Framework.Core.Actor GetActor()
        {
            return m_pActor;
        }
        //--------------------------------------------------------
        public override void OnSelectActor(Actor pActor)
        {
            if (m_pActor == pActor)
                return;

            //! 只有攻击方可编辑
            if (pActor.GetAttackGroup() != 0)
                return;

            m_pActor = pActor;
            m_pActor.GetAgent<ActorGraphicAgent>(true);
            m_pActorComp = m_pActor.GetComponent<ActorComponent>();
            if(m_pActorComp.Prefab) m_pActorPrefab = m_pActorComp.Prefab;

            RefreshTimelineAction();
        }
        //--------------------------------------------------------
        public override void OnSaveChanges()
        {
            base.OnSaveChanges();
            if (m_pActorPrefab != null)
                SaveData();
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            var viewRect = GetRect();
            if (viewRect.width <= 1 || viewRect.height <= 1)
                return; // 防止无效区域

            GUILayout.BeginArea(viewRect);
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(viewRect.width), GUILayout.Height(25) });
            Color backColor = GUI.color;
            for(int i =0; i < TABS.Length; ++i)
            {
                if (i == (int)m_Tab) GUI.color = Color.green;
                else GUI.color = backColor;
                if (GUILayout.Button(TABS[i]))
                {
                    m_Tab = (ETab)i;
                }
            }
            GUI.color = backColor;
            GUILayout.EndHorizontal();

            Rect infoView = new Rect(viewRect.x, viewRect.y + 25, viewRect.width, viewRect.height - 25);

            if (m_Tab == ETab.Info) DrawInfo(infoView);
            else if (m_Tab == ETab.AvatarMask) DrawAvatarMask(infoView);
            else if (m_Tab == ETab.CommonAction) DrawCommonAction(infoView);
            else if (m_Tab == ETab.TimelineAction) DrawTimelineAction(infoView);
            GUILayout.EndArea();
        }
        //--------------------------------------------------------
        void DrawInfo(Rect view)
        {
            if (m_pActor == null)
                return;
            var graphData = m_pActor.GetGraphData();
            if (graphData == null)
                return;
            InspectorDrawUtil.DrawProperty(graphData, null);
        }
        //--------------------------------------------------------
        void RefreshTimelineAction()
        {
            m_pActionTimelineTree.BeginTreeData();
            if(m_pActor!=null && m_pActor.GetGraphData()!=null)
            {
                var graphData = m_pActor.GetGraphData();
                if (graphData.timelineActions == null) graphData.timelineActions = new System.Collections.Generic.List<ActorTimelineAction>();
                for (int i = 0; i < graphData.timelineActions.Count; ++i)
                {
                    var timeline = graphData.timelineActions[i];
                    m_pActionTimelineTree.AddData(new GraphItem() { asset = timeline, id = (int)EditorUtil.BuildActionKey(timeline.type, timeline.actionTag), name = timeline.actioName });
                }
            }
            m_pActionTimelineTree.EndTreeData();
        }
        //--------------------------------------------------------
        void DrawAvatarMask(Rect view)
        {
            if (m_pActorComp == null)
                return;
 
            if (m_pActorComp.avatarMasks == null)
                m_pActorComp.avatarMasks = new List<ActorAvatarMask>(2);

            bool bHasAdd = false;
            for (int i =0; i < m_pActorComp.avatarMasks.Count; ++i )
            {
                var mask = m_pActorComp.avatarMasks[i];
                if (m_pActorComp.avatarMasks[i].layer == m_nAddLayerMask)
                    bHasAdd = true;
                EditorGUILayout.BeginVertical("box");
                EditorGUI.BeginDisabledGroup(true);
                mask.layer = EditorGUILayout.IntField("作用层", mask.layer);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();
                mask.avatarMask = (UnityEngine.AvatarMask)EditorGUILayout.ObjectField("AvatarMask", mask.avatarMask, typeof(UnityEngine.AvatarMask), false);
                if (EditorGUI.EndChangeCheck())
                {
                    m_pActorComp.avatarMasks[i] = mask;
                }

                // 删除按钮
                if (GUILayout.Button("删除"))
                {
                    if (EditorUtility.DisplayDialog("提示", "确定删除该AvatarMask吗？", "删除", "取消"))
                    {
                        m_pActorComp.avatarMasks.RemoveAt(i);
                        EditorGUILayout.EndVertical();
                        break;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            // 新增按钮
            EditorGUILayout.BeginHorizontal();
            m_nAddLayerMask = EditorGUILayout.IntField(m_nAddLayerMask);
            EditorGUI.BeginDisabledGroup(bHasAdd );
            if (GUILayout.Button("新增AvatarMask"))
            {
                if (m_nAddLayerMask >= 0)
                {
                    var newMask = new ActorAvatarMask();
                    newMask.layer = m_nAddLayerMask;
                    m_pActorComp.avatarMasks.Add(newMask);
                    m_nAddLayerMask = -1;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "作用层必须大于等于0", "确定");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
        //--------------------------------------------------------
        void DrawCommonAction(Rect view)
        {
            if (m_pActorComp == null)
                return;
            m_CommonActionList = GUILayout.BeginScrollView(m_CommonActionList, new GUILayoutOption[] { GUILayout.Width(view.width), GUILayout.Height(view.height - 30) });
            if (m_pActorComp.commonActions == null) m_pActorComp.commonActions = new System.Collections.Generic.List<ActorCommonAction>();


            ActorCommonAction preIdleClip = null;
            for (int i = 0; i < m_pActorComp.commonActions.Count; ++i)
            {
                var common = m_pActorComp.commonActions[i];
                if(common.type == EActionStateType.Idle)
                {
                    preIdleClip = common;
                    break;
                }
            }
            for (int i =0; i < m_pActorComp.commonActions.Count; ++i)
            {
                bool bExpand;
                m_vCommonActionExpand.TryGetValue(m_pActorComp.commonActions[i].GetHashCode(), out bExpand);

                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                bExpand = EditorGUILayout.Foldout(bExpand, m_pActorComp.commonActions[i].type.ToString());
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_pActorComp.commonActions[i].clip != null && bExpand)
                    {
                        m_pActor.SetIdleType(m_pActorComp.commonActions[i].type, m_pActorComp.commonActions[i].actionTag);
                        m_pActor.StartActionState(m_pActorComp.commonActions[i].type, m_pActorComp.commonActions[i].actionTag, true);
                    }
                }
                if(GUILayout.Button("-"))
                {
                    if(EditorUtility.DisplayDialog("提示", "确定删除该常规动作配置吗？", "删除", "再想想"))
                    {
                        m_pActorComp.commonActions.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                }
                
                GUILayout.EndHorizontal();
                if (bExpand)
                {
                    EditorGUI.indentLevel++;
                    InspectorDrawUtil.BeginChangeCheck();
                    EActionStateType lastType = m_pActorComp.commonActions[i].type;
                    uint lastTag = m_pActorComp.commonActions[i].actionTag;
                    m_pActorComp.commonActions[i] = (ActorCommonAction)InspectorDrawUtil.DrawProperty(m_pActorComp.commonActions[i], null);
                    if(InspectorDrawUtil.EndChangeCheck())
                    {
                        m_pActor.RemoveActionState(lastType, lastTag);
                        if (m_pActorComp.commonActions[i].clip != null)
                        {
                            m_pActor.SetIdleType(m_pActorComp.commonActions[i].type, m_pActorComp.commonActions[i].actionTag);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                m_vCommonActionExpand[m_pActorComp.commonActions[i].GetHashCode()] = bExpand;
                InspectorDrawUtil.EndChangeCheck();
            }
            GUILayout.EndScrollView();
            if(GUILayout.Button("添加常规动作"))
            {
                if (m_pActorComp.commonActions == null) m_pActorComp.commonActions = new List<ActorCommonAction>();
                ActorCommonAction common = new ActorCommonAction();
                common.type = EActionStateType.None;
                common.actionTag = 0;
                common.clip = null;
                m_pActorComp.commonActions.Add(common);
            }
            ActorCommonAction idleAction = default;
            for (int i = 0; i < m_pActorComp.commonActions.Count; ++i)
            {
                var common = m_pActorComp.commonActions[i];
                if (common.type == EActionStateType.Idle)
                {
                    idleAction = common;
                    break;
                }
            }
            if (preIdleClip!=null && !preIdleClip.Equal(idleAction))
            {
                //!更新playable
                var pAgent = m_pActor.GetAgent<ActorGraphicAgent>(true);
                if(pAgent!=null)
                {
                    pAgent.SetDefaultClip(idleAction.clip);
                }
            }
        }
        //--------------------------------------------------------
        void DrawTimelineAction(Rect infoView)
        {
            m_pActionTimelineTree.GetColumn(0).width = infoView.width / 4;
            m_pActionTimelineTree.GetColumn(1).width = infoView.width / 4;
            m_pActionTimelineTree.GetColumn(2).width = infoView.width / 4;
            m_pActionTimelineTree.GetColumn(3).width = infoView.width / 4;
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(infoView.width), GUILayout.Height(20) });
            EditorGUILayout.LabelField("搜索", new GUILayoutOption[] { GUILayout.Width(40) });
            m_pActionTimelineTree.searchString = EditorGUILayout.TextField(m_pActionTimelineTree.searchString);
            EditorGUILayout.EndHorizontal();

            m_pActionTimelineTree.OnGUI(new Rect(0, 20, infoView.width, infoView.height - 75));

            GUILayout.BeginArea(new Rect(infoView.x, infoView.yMax - 50, infoView.width, 25));

            EditorGUILayout.BeginHorizontal();
            m_addTimelineAction = EditorUtil.DrawActionAndTag(m_addTimelineAction, "", false, new GUILayoutOption[] { GUILayout.MaxWidth(infoView.width - 50) });
            EditorGUI.BeginDisabledGroup(m_pActor == null || m_pActor.GetGraphData() == null || m_pActionTimelineTree.GetItem((int)m_addTimelineAction) != null);
            if (GUILayout.Button("新增", new GUILayoutOption[] { GUILayout.Width(50) }))
            {
                EditorUtil.GetActionTypeAndTag(m_addTimelineAction, out var eType, out var tag);
                ActorTimelineAction timelineAction = new ActorTimelineAction();
                timelineAction.actioName = eType.ToString();
                timelineAction.type = eType;
                timelineAction.actionTag = tag;
                timelineAction.priority = 0;
                if (m_pActor.GetGraphData().timelineActions == null)
                    m_pActor.GetGraphData().timelineActions = new List<ActorTimelineAction>();
                m_pActor.GetGraphData().timelineActions.Add(timelineAction);
                RefreshTimelineAction();
                m_pActionTimelineTree.SetSelection(new List<int>(new int[] { (int)m_addTimelineAction }));
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        //--------------------------------------------------------
        bool OnDrawTimelineAction(RowArgvData rowData)
        {
            var data = rowData.itemData.data as GraphItem;
            if (rowData.column == 0)
            {
                string name = Framework.ED.EditorUtil.GetEnumDisplayName(data.asset.type);
                name += $"[{data.asset.actionTag}]";
                EditorGUI.LabelField(rowData.rowRect, name);
            }
            else if (rowData.column == 1)
            {
                data.asset.actioName = EditorGUI.DelayedTextField(rowData.rowRect, data.asset.actioName);
            }
            else if (rowData.column == 2)
            {
                string name = Framework.ED.EditorUtil.GetEnumDisplayName(data.asset.type);
                name += $"[{data.asset.actionTag}]";
                EditorGUI.LabelField(rowData.rowRect, name);
                GUILayout.BeginArea(rowData.rowRect);
                data.asset.type = (EActionStateType)Framework.ED.EditorEnumPop.PopEnum(string.Empty, data.asset.type);
                GUILayout.EndArea();
            }
            else if (rowData.column == 3)
            {
                data.asset.actionTag = (ushort)EditorGUI.IntField(rowData.rowRect, data.asset.actionTag);
            }
            return true;
        }
        //--------------------------------------------------------
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
        }
        //--------------------------------------------------------
        public void SaveData()
        {
            if (m_pActor == null)
                return;
            var able = m_pActor.GetObjectAble();
            if (able == null) return;
            if (m_pActorPrefab == null)
                return;

            var actorComp = able as ActorComponent;
            if (actorComp == null)
                return;

            var component = m_pActorPrefab.GetComponent<ActorComponent>();
            if (component == null)
                component = m_pActorPrefab.AddComponent<ActorComponent>();
            component.avatarMasks = actorComp.avatarMasks;
            component.commonActions = actorComp.commonActions;

            string file = m_pActor.GetGraphData().GetPathFile();
            if(string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                string saveFile = EditorUtility.SaveFilePanel("保存动作文件", "Datas/ActionGraphs", "New","json");
                saveFile = saveFile.Replace("\\", "/");
                saveFile = saveFile.Replace(Application.dataPath + "/", "Assets/");
                if (!string.IsNullOrEmpty(saveFile))
                {
                    m_pActor.GetGraphData().SetPathFile(saveFile);
                }
            }

            component.ActionGraphData = m_pActor.GetGraphData().Save();
            EditorUtility.SetDirty(m_pActorPrefab);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif