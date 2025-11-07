#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:03:2023
类    名: 	ActionEditorWindow
作    者:	HappLI
描    述:	表现图编辑窗口
*********************************************************************/
using Framework.Core;
using Framework.Cutscene.Editor;
using Framework.Cutscene.Runtime;
using Framework.ED;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ActorSystem.ED
{
    public class ActionEditorWindow
#if USE_CUTSCENE
        : Framework.Cutscene.Editor.ACutsceneEditor, ICutsceneCallback
#else
        : EditorWindowBase
#endif
    {
        private const float EDGE_SNAP_OFFSET = 0.25f;
        enum EDragEdge
        {
            None = 0,
            Asset,
            Inspector,
            Timeline,
        }

        Rect                            m_PreviewRect;

        float                           m_fToolSize = 25;
        Rect                            m_InspectorRect;
        Vector2                         m_InspectorScoller;
        Rect                            m_TimelineRect;
        Vector2                         m_TimelineScoller;
        Rect                            m_AssetRect;
        Vector2                         m_AssetScoller;
        Rect                            m_ToolBarRect;

        EDragEdge m_eDragEdge = EDragEdge.None;

        float                           m_ViewLeftRate;
        float                           m_ViewRightRate;
        float                           m_ViewHeightRate;

        GUIStyle                        m_TileStyle;
#if USE_CUTSCENE
        private Rect                    m_CutsceneAssetRect;
        private Rect                    m_CutsceneTimelineRect;
        private Rect                    m_CutsceneInspectorRect;
        CutsceneInstance                m_pCutsceneInstance = null;
        private bool                    m_bCutscenInspectorExpand = true;
#endif

        Framework.Plugin.AT.AgentTreeEditor m_pSkillEditor = null;
        ActorComponent                  m_pActorComp = null;
        Framework.Core.Actor            m_pActor = null;
        Framework.Core.Actor            m_pTarget = null;
        //--------------------------------------------------------
        public Framework.Core.Actor Actor
        {
            get { return m_pActor; }
        }
        //--------------------------------------------------------
        public Framework.Core.Actor Target
        {
            get { return m_pTarget; }
        }
        //--------------------------------------------------------
        [MenuItem("Tools/动作编辑器")]
        public static void Open()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
                return;
            }
            ActionEditorWindow window = EditorWindow.GetWindow<ActionEditorWindow>();
            window.titleContent = new GUIContent("动作编辑器");
        }
        //--------------------------------------------------------
        public static void OpenTarget(Framework.Core.ActorComponent pActor)
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
                return;
            }
            ActionEditorWindow window = EditorWindow.GetWindow<ActionEditorWindow>();
            window.titleContent = new GUIContent("动作编辑器-" + pActor.gameObject.name);
            window.m_pActorComp = pActor;
        }
        //--------------------------------------------------------
        protected override void OnInnerEnable()
        {
            this.minSize = new Vector2(400, 300);
            this.wantsMouseMove = true;
            this.wantsMouseEnterLeaveWindow = true;
            this.autoRepaintOnSceneChange = true;
            this.wantsLessLayoutEvents = true;

            m_ViewLeftRate = 0.25f;
            m_ViewRightRate = 0.75f;
            m_ViewHeightRate = 0.65f;

            m_TileStyle = new GUIStyle();
            m_TileStyle.fontSize = 20;
            m_TileStyle.normal.textColor = Color.white;
            m_TileStyle.alignment = TextAnchor.MiddleCenter;
#if USE_CUTSCENE
            RegisterLogic<Framework.Cutscene.Editor.AssetDrawLogic>().InitRectMethod(this.GetType(), "m_CutsceneAssetRect");
            RegisterLogic<Framework.Cutscene.Editor.TimelineDrawLogic>().InitRectMethod(this.GetType(), "m_CutsceneTimelineRect");
            RegisterLogic<Framework.Cutscene.Editor.InspectorDrawLogic>().InitRectMethod(this.GetType(), "m_CutsceneInspectorRect");
            RegisterLogic<Framework.Cutscene.Editor.UndoLogic>();
#endif
        }
        //--------------------------------------------------------
        protected override void OnInnerDisable()
        {
            if (m_pSkillEditor != null) m_pSkillEditor.Close();
            m_pSkillEditor = null;
        }
        //--------------------------------------------------------
        protected override void OnInnerDestroy()
        {
            if (m_pActor != null) m_pActor.Destroy();
            m_pActor = null;
            if (m_pTarget != null) m_pTarget.Destroy();
            m_pTarget = null;
        }
        //--------------------------------------------------------
        protected override void OnStart()
        {
            m_pActor = null;
            if (m_pActorComp)
            {
                var pInstance = GameObject.Instantiate<GameObject>(m_pActorComp.gameObject);
                var pComp = pInstance.GetComponent<ActorComponent>();
                pComp.SetBindPrefab(m_pActorComp.gameObject);

                Framework.Core.Actor pActorInstance = null;
                if (pComp is CharacterComponent)
                {
                    pActorInstance = GetEditorGame().gameWorld.SyncCreateNode<Character>(EActorType.Character,null, -9999);
                }
                else
                {
                    pActorInstance = GetEditorGame().gameWorld.SyncCreateNode<Actor>(EActorType.Character, null, -9999);
                }
                pActorInstance.SetAttackGroup(0);
                pActorInstance.OnCreated();
                pActorInstance.SetObjectAble(pComp);
                pActorInstance.SetActived(true);
                pActorInstance.SetVisible(true);
                pActorInstance.EnableLogic(true);
                pActorInstance.EnableRVO(false);
                pActorInstance.SetAttackGroup(0);
                SelectActor(pActorInstance);
                m_pActor = pActorInstance;
#if USE_CUTSCENE
                m_pCutsceneInstance = pActorInstance.GetActorGraph().GetCutsceneInstance();
                m_pCutsceneInstance.RegisterCallback(this);
                pActorInstance.GetActorGraph().SetAutoUpdate(false);
#endif
            }

            if(m_pTarget==null)
            {
                m_pTarget = GetEditorGame().gameWorld.SyncCreateNode<Actor>(EActorType.Character, null, -9998);
                m_pTarget.OnCreated();
                m_pTarget.SetPosition(new Vector3(2, 0, 0));
                GameObject pInst = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_pTarget.SetObjectAble(pInst.AddComponent<ActorComponent>());
            }
            m_pTarget.SetActived(true);
            m_pTarget.SetVisible(true);
            m_pTarget.EnableLogic(true);
            m_pTarget.EnableRVO(false);
            m_pTarget.SetAttackGroup(1);
            m_pActor.GetSkillSystem().AddLockTarget(m_pTarget);
            SelectActor(m_pTarget);
        }
        //--------------------------------------------------------
        protected override void OnInnerUpdate()
        {
            RefreshLayout();
#if USE_CUTSCENE
            if (m_pCutsceneInstance != null)
                m_pCutsceneInstance.BindData(m_pActor);
#endif
       //     if (m_pActor != null) m_pActor.Update(m_pTimer.deltaTime);
       //     if (m_pTarget != null) m_pTarget.Update(m_pTimer.deltaTime);
        }
        //--------------------------------------------------------
        public Rect InspectorRect
        {
            get { return m_InspectorRect; }
        }
        //--------------------------------------------------------
        public Rect AssetRect
        {
            get { return m_AssetRect; }
        }
        //--------------------------------------------------------
        public Rect PreviewRect
        {
            get { return m_PreviewRect; }
        }
        //--------------------------------------------------------
        public Rect TimelineRect
        {
            get { return m_TimelineRect; }
        }
        //--------------------------------------------------------
        public Rect ToolBarRect
        {
            get { return m_ToolBarRect; }
        }
        //--------------------------------------------------------
        public GUIStyle TileStyle
        {
            get { return m_TileStyle; }
        }
        //--------------------------------------------------------
        public void OpenSkillEditor()
        {
            var actor = GetLogic<AssetDrawLogic>().GetActor();
            if (actor == null)
                return;
            var actorComp = actor.GetObjectAble();
            if (actorComp == null)
                return;
            ActorComponent component = actorComp as ActorComponent;
            if(m_pActorComp!=null)
            {
                component = m_pActorComp;
            }
            else
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(actorComp.gameObject);
                if (prefab != null)
                {
                    component = prefab.GetComponent<ActorComponent>();
                }
            }
            if (component == null)
                return;

            if (m_pSkillEditor != null)
            {
                if (m_pSkillEditor.GetATCoreData() == component.ATData)
                {
                    m_pSkillEditor.Focus();
                    return;
                }
                m_pSkillEditor.Close();
            }
             m_pSkillEditor = Framework.Plugin.AT.AgentTreeEditor.Editor(component.ATData, component);
        }
        //--------------------------------------------------------
        protected override void OnEvent(Event evt)
        {
            base.OnEvent(evt);
            if(evt.control && evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Z)
            {
                evt.Use();
            }
        }
        //--------------------------------------------------------
        protected override void OnInnerGUI()
        {
            base.OnInnerGUI();
            ProcessDragEdge(Event.current);
        }
        //--------------------------------------------------------
        protected override void OnInnerGUIEnd()
        {
            var processManipulators = Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout;
            bool bRepaint = Event.current.type == EventType.Repaint;
            //! top split toobar line
            if (bRepaint)
                UIDrawUtils.DrawColorLine(new Vector2(0, m_fToolSize), m_ToolBarRect.size, Color.grey);
            bool bDragOver = false;
            //! asset line
            {
                Color lineColor = Color.grey;
                var rect = new Rect(m_AssetRect.xMax-10, m_AssetRect.yMin, 10, m_AssetRect.height);
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
                if (CheckEdgeDrag(EDragEdge.Asset, rect))
                {
                    lineColor = Color.yellow;
                    bDragOver = true;
                }
                if (bRepaint)
                    UIDrawUtils.DrawColorLine(new Vector2(m_AssetRect.xMax, m_AssetRect.yMin), new Vector2(m_AssetRect.xMax, m_AssetRect.yMax), lineColor);
            }
            //! inspector
            {
                Color lineColor = Color.grey;
                var rect = new Rect(m_InspectorRect.xMin, m_InspectorRect.yMin, 10, m_InspectorRect.height);
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
                if (CheckEdgeDrag(EDragEdge.Inspector, rect))
                {
                    lineColor = Color.yellow;
                    bDragOver = true;
                }
                if (bRepaint)
                    UIDrawUtils.DrawColorLine(new Vector2(m_InspectorRect.xMin + 0.01f, m_InspectorRect.yMin), new Vector2(m_InspectorRect.xMin + 0.01f, m_InspectorRect.yMax), lineColor);
            }
            //! timeline split line
            {
                Color lineColor = Color.grey;
                var rect = new Rect(m_TimelineRect.xMin, m_TimelineRect.yMin, m_TimelineRect.width, 10);
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);
                if (CheckEdgeDrag(EDragEdge.Timeline, rect))
                {
                    lineColor = Color.yellow;
                    bDragOver = true;
                }
                if (bRepaint)
                {
#if USE_CUTSCENE
                    UIDrawUtils.DrawColorLine(new Vector2(m_TimelineRect.xMin, m_TimelineRect.yMin + 0.01f), new Vector2(m_bCutscenInspectorExpand?m_InspectorRect.xMin:m_TimelineRect.xMax, m_TimelineRect.yMin + 0.01f), lineColor);
#else
                    UIDrawUtils.DrawColorLine(new Vector2(m_TimelineRect.xMin, m_TimelineRect.yMin + 0.01f), new Vector2(m_TimelineRect.xMax, m_TimelineRect.yMin + 0.01f), lineColor);
#endif
                }
            }

#if USE_CUTSCENE
            if(GUI.Button(new Rect(m_CutsceneInspectorRect.xMax-20, m_CutsceneInspectorRect.yMin, 20, 20),"U"))
            {
                m_bCutscenInspectorExpand = !m_bCutscenInspectorExpand;
            }
#endif
        }
        //--------------------------------------------------------
        bool CheckEdgeDrag(EDragEdge type, Rect region)
        {
            if (m_eDragEdge == type) return true;
            if (region.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDrag)
                    m_eDragEdge = type;
                return true;
            }
            return false;
        }
        //--------------------------------------------------------
        void ProcessDragEdge(Event evt)
        {
            if (m_eDragEdge == EDragEdge.None) return;
            if (evt.type == EventType.MouseUp)
            {
                m_eDragEdge = EDragEdge.None;
                return;
            }
            if (evt.button == 0)
            {
                if (evt.type != EventType.MouseDrag)
                    return;

                switch (m_eDragEdge)
                {
                    case EDragEdge.Asset:
                        {
                            m_ViewLeftRate += Event.current.delta.x / this.position.width;
                            evt.Use();
                        }
                        break;
                    case EDragEdge.Inspector:
                        {
                            m_ViewRightRate += Event.current.delta.x / this.position.width;
                            evt.Use();
                        }
                        break;
                    case EDragEdge.Timeline:
                        {
                            m_ViewHeightRate += Event.current.delta.y / (this.position.height - m_fToolSize);
                            evt.Use();
                        }
                        break;
                }
            }
        }
        //--------------------------------------------------------
        void RefreshLayout()
        {
            m_ViewLeftRate = Mathf.Clamp(m_ViewLeftRate, 0.1f, 0.5f);
            m_ViewRightRate = Mathf.Clamp(m_ViewRightRate, 0.55f, 0.8f);
            m_ViewHeightRate = Mathf.Clamp(m_ViewHeightRate, 0.25f, 0.9f);
            m_PreviewRect.x = this.position.width * m_ViewLeftRate;
            m_PreviewRect.y = m_fToolSize;
            m_PreviewRect.width = this.position.width * m_ViewRightRate - m_PreviewRect.x;
            m_PreviewRect.height = (this.position.height - m_fToolSize) * m_ViewHeightRate;

            m_AssetRect.y = m_InspectorRect.y = m_PreviewRect.y;
            m_AssetRect.width = m_PreviewRect.xMin;
            m_InspectorRect.x = m_PreviewRect.xMax;
            m_InspectorRect.width = this.position.width - m_PreviewRect.xMax;
            m_InspectorRect.height = m_AssetRect.height = m_PreviewRect.height;

            m_TimelineRect.yMin = m_PreviewRect.yMax;
            m_TimelineRect.height = this.position.height - m_PreviewRect.yMax;
            m_TimelineRect.width = this.position.width;

            m_ToolBarRect.position = Vector2.zero;
            m_ToolBarRect.size = new Vector2(this.position.width, m_fToolSize);


            m_CutsceneAssetRect.x = m_TimelineRect.x;
            m_CutsceneAssetRect.y = m_TimelineRect.y;
            m_CutsceneAssetRect.width = 200;
            m_CutsceneAssetRect.height = m_TimelineRect.height;

            m_CutsceneTimelineRect.x = m_CutsceneAssetRect.xMax;
            m_CutsceneTimelineRect.y = m_TimelineRect.y;
            m_CutsceneTimelineRect.width = m_InspectorRect.xMin- m_CutsceneAssetRect.xMax;
            m_CutsceneTimelineRect.height = m_TimelineRect.height;

            if(m_bCutscenInspectorExpand)
            {
                m_CutsceneInspectorRect.x = m_InspectorRect.x;
                m_CutsceneInspectorRect.y = m_InspectorRect.y;
                m_CutsceneInspectorRect.width = m_InspectorRect.width;
                m_CutsceneInspectorRect.height = this.position.height- m_InspectorRect.y;
            }
            else
            {
                m_CutsceneInspectorRect.x = m_InspectorRect.x;
                m_CutsceneInspectorRect.y = m_TimelineRect.y;
                m_CutsceneInspectorRect.width = m_InspectorRect.width;
                m_CutsceneInspectorRect.height = m_TimelineRect.height;
            }
        }
        //--------------------------------------------------------
        public void SelectActor(Actor pActor)
        {
            var actors = GetLogics();
            for(int i =0; i < actors.Count; ++i)
            {
                if (actors[i] is ActionEditorLogic)
                {
                    (actors[i] as ActionEditorLogic).OnSelectActor(pActor);
                }
            }
        }
#if USE_CUTSCENE
        //--------------------------------------------------------
        public override bool IsRuntimeOpenPlayingCutscene()
        {
            return false;
        }
        //--------------------------------------------------------
        public override void OpenRuntimePlayingCutscene(CutsceneInstance pInstance)
        {
            m_pCutsceneInstance = pInstance;
        }
        //--------------------------------------------------------
        public override CutsceneInstance GetCutsceneInstance()
        {
            return m_pCutsceneInstance;
        }
        //--------------------------------------------------------
        public override void OpenAgentTreeEdit()
        {
        }
        //--------------------------------------------------------
        public override void SaveAgentTreeData()
        {
        }
        //--------------------------------------------------------
        public override Framework.Cutscene.Editor.AgentTreeWindow GetAgentTreeWindow()
        {
            return null;
        }
        //--------------------------------------------------------
        public void OnCutsceneStatus(int cutsceneInstanceId, EPlayableStatus status)
        {
            if(status == EPlayableStatus.Start)
            {
                if (m_pTarget != null)
                {
                    m_pTarget.ClearParts();
                    m_pTarget.SetFinalPosition(new Vector3(2, 0, 0));
                }
                if (m_pActor != null)
                {
                    m_pActor.SetPosition(Vector3.zero);
                    m_pActor.SetFinalPosition(Vector3.right);
                }
            }    
        }
        //--------------------------------------------------------
        public bool OnCutscenePlayableCreateClip(CutscenePlayable playable, CutsceneTrack track, IBaseClip clip)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnCutscenePlayableDestroyClip(CutscenePlayable playable, CutsceneTrack track, IBaseClip clip)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnCutscenePlayableFrameClip(CutscenePlayable playable, FrameData frameData)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnCutscenePlayableFrameClipEnter(CutscenePlayable playable, CutsceneTrack track, FrameData frameData)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnCutscenePlayableFrameClipLeave(CutscenePlayable playable, CutsceneTrack track, FrameData frameData)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnCutsceneEventTrigger(CutscenePlayable pPlayablle, CutsceneTrack pTrack, Framework.Cutscene.Runtime.IBaseEvent pEvent)
        {
            return false;
        }
        //--------------------------------------------------------
        public bool OnAgentTreeExecute(Framework.CutsceneAT.Runtime.AgentTree pAgentTree, Framework.CutsceneAT.Runtime.BaseNode pNode)
        {
            return false;
        }
#endif
        //--------------------------------------------------------
        public override void OnSpawnInstance(AInstanceAble pInstance)
        {
            PreviewLogic previewLogic = GetLogic<PreviewLogic>();
            if (previewLogic == null)
                return;
            previewLogic.AddInstance(pInstance);
        }
        //--------------------------------------------------------
        public override void OnSetTime(float time)
        {
            var logic = GetLogic<Framework.Cutscene.Editor.TimelineDrawLogic>();
            if (logic == null)
                return;
            logic.SetCurrentTime(time);
        }

    }
}

#endif