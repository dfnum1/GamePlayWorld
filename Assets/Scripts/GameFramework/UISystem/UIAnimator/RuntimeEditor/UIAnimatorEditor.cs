#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Framework.UI.ED
{
    public class UIAnimatorEditor : EditorWindow
    {
        public static string ASSET_FILE = "Assets/Datas/Config/UIAnimations.asset";

        [System.Serializable]
        class TrackBindStack
        {
            public int dataDeclare = Core.DeclareKit.InTag_OutTag_Pos_Euler;
            public List<UITargetBindTrackParameter.SplineData> datas = new List<UITargetBindTrackParameter.SplineData>();
        }
        Stack<string> m_BindTacksStacks = new Stack<string>();
        class DragTrackData
        {
            public UIBaseParameter parameter;
            public AnimationCurve track;
            public int keyFrame;
            public bool left = false;
            public Rect rect;
            public float maxTime = 0;
            public float lastTime = -1;
        }

        static int BASE_GROUP_WIDTH = 350;
        static int STRIPT_TIME_TITLE_WIDTH = 60;
        static float TIMELINE_HEIGHT = 20;
     //   static float TIMELINE_KEYFRAME_WIDTH = 4;
        static int TOP_TOOL_HEIGHT = 30;
        static int LEFT_CONTROLLER = 300;
        public static Color TRACK_COLOR = new Color(1, 0, 0, 1);
        public static Color TRACK_CTL_COLOR = new Color(1, 1, 0, 1);
        public static Color PARAMETER_COLOR = new Color(1, 0, 1, 1);
        public static Color BINDTRACK_PARAMETER_COLOR = new Color(1, 1, 0, 1);
        static UIAnimatorEditor ms_Instance;
        //------------------------------------------------------
        [MenuItem("Tools/UI/动效")]
        public static void Open()
        {
            if (ms_Instance == null)
                ms_Instance = EditorWindow.GetWindow<UIAnimatorEditor>();
            ms_Instance.titleContent = new GUIContent("UI 动效");
            ms_Instance.Show();
        }
        //------------------------------------------------------
        List<string> m_vAnimatorsPop = new List<string>();
        List<UIAnimatorGroupData> m_vAnimatorGroups = new List<UIAnimatorGroupData>();

        UIAnimatorElementDrawer m_pDrawer = new UIAnimatorElementDrawer();

        bool m_bLockParent = false;
        UIAnimatorGroupData m_pCurGroup = null;

        HashSet<string> m_IngoreFields = new HashSet<string>();
        UIBaseParameter m_pCurrentAnimatorParameter = null;
        UISplineEditor m_pSplineEditor = new UISplineEditor();

        bool m_bRecoding = false;

        GUIStyle m_pStripTitle = null;

        List<UIAnimationElement> m_vAnimationElementList = new List<UIAnimationElement>();

        Vector2 m_Scroll = Vector2.zero;
        public float m_fCurTime = 0;

        public bool m_bPlaying = false;
        UIAnimatorPlayable m_pPlayerGroup = new UIAnimatorPlayable();

        Framework.ED.EditorTimer m_pTimer = new Framework.ED.EditorTimer();

        Vector2 m_TimelineMouse = Vector2.zero;
        Rect m_TimelineRect = new Rect();

        float m_fControllOffsetY = 0;

        DragTrackData m_DragTrack = null;

        Backuper m_Backuper = new Backuper();
        //------------------------------------------------------
        void OnEnable()
        {
            ms_Instance = this;
            UIAnimatorUtil.Check();
            UIAnimatorFactory.getInstance().Init(ASSET_FILE);
            m_bRecoding = false;

            m_pStripTitle = new GUIStyle();
            m_pStripTitle.normal.textColor = Color.white;

            m_vAnimatorGroups.Clear();
            m_vAnimatorsPop.Clear();
            foreach (var db in UIAnimatorFactory.getInstance().Groups)
            {
                m_vAnimatorsPop.Add(db.Value.desc + "[" + db.Key + "]");
                m_vAnimatorGroups.Add(db.Value);
            }
            OnSelectionChange();
            SceneView.duringSceneGui += OnSceneFunc;
        }
        //------------------------------------------------------
        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneFunc;
            m_bRecoding = false;
            if (m_pCurGroup != null) m_pCurGroup.Recovert();
            m_Backuper.Recovert();
            for (int i = 0; i < m_vAnimatorGroups.Count; ++i)
            {
                if(!UIAnimatorFactory.getInstance().Groups.ContainsValue(m_vAnimatorGroups[i]))
                {
                    if(EditorUtility.DisplayDialog("提示", "数据有修改，还未进行保存，是保存", "保存", "不保存"))
                    {
                        Save(false);
                        break;
                    }
                }
            }
            ms_Instance = null;
        }
        //------------------------------------------------------
        void Save(bool bCheckNullName = true)
        {
            if(bCheckNullName)
            {
                for (int i = 0; i < m_vAnimatorGroups.Count; ++i)
                {
                    if (m_vAnimatorGroups[i].nID <=0)
                    {
                        if (EditorUtility.DisplayDialog("提示", "组名ID不能<=0，请设置", "好的"))
                        {
                            m_pCurGroup = m_vAnimatorGroups[i];
                        }
                        return;
                    }
                }
            }

            UIAnimatorFactory.getInstance().Groups.Clear();
            foreach (var db in m_vAnimatorGroups)
            {
                UIAnimatorFactory.getInstance().Groups[db.nID] = db;
            }
            UIAnimatorFactory.getInstance().Save(ASSET_FILE);
        }
        //------------------------------------------------------
        void RefreshPop()
        {
            m_vAnimatorsPop.Clear();
            foreach (var db in m_vAnimatorGroups)
            {
                m_vAnimatorsPop.Add( db.desc+ "[" + db.nID + "]");
            }
        }
        //------------------------------------------------------
        public int NewID()
        {
            int id = 0;
            foreach (var db in m_vAnimatorGroups)
            {
                id = Mathf.Max(id, db.nID);
            }
            ++id;
            return id;
        }
        //------------------------------------------------------
        public int IndexOfGroups(UIAnimatorGroupData group)
        {
            return m_vAnimatorGroups.IndexOf(group);
        }
        //------------------------------------------------------
        private void OnSelectionChange()
        {
            if (m_bLockParent) return;
            if (m_Backuper.GetController() != Selection.activeTransform)
                m_bRecoding = false;
            m_Backuper.SetController( Selection.activeTransform);
            if (m_pPlayerGroup != null) m_pPlayerGroup.RefreshControlledWidget();
            if (ms_Instance!=null) ms_Instance.Repaint();
        }
        //------------------------------------------------------
        float GetMaxTime()
        {
            float maxTime = 1;
            if(m_pCurGroup!=null)
                maxTime = Mathf.Max(1, m_pCurGroup.GetTimelineLength());
            return maxTime;
        }
        //------------------------------------------------------
        private void OnGUI()
        {
            m_pDrawer.SetEditor(this);
            DrawTools();

            float maxTime = GetMaxTime();
            m_TimelineRect = new Rect(0,0,position.width, 30);
            if (m_pCurGroup != null)
            {
                float offset = 0;
                //base group
                {
                    GUILayout.BeginArea(new Rect(0, TOP_TOOL_HEIGHT,BASE_GROUP_WIDTH,position.height - TOP_TOOL_HEIGHT));
                    GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.MaxWidth(BASE_GROUP_WIDTH) };
                    {
                        DrawGroup(ref m_pCurGroup, layOp);
                    }
                    GUILayout.EndArea();
                    offset += BASE_GROUP_WIDTH;
                }
                UIAnimatorUtil.DrawColorBox(new Rect(offset, TOP_TOOL_HEIGHT, 2, position.height - TOP_TOOL_HEIGHT), Color.green);
                offset += 2;

                GUILayout.BeginVertical();
                //! timeline base info
                {
                    GUILayout.BeginArea(new Rect(offset, TOP_TOOL_HEIGHT+10, LEFT_CONTROLLER, 30));
                    GUILayout.BeginHorizontal();
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.LabelField("PlayTime：");
                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUI.BeginChangeCheck();
                    m_fCurTime = EditorGUILayout.FloatField(m_fCurTime, new GUILayoutOption[] { GUILayout.Height(25) });
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(!m_bPlaying)
                        {
                            m_pSplineEditor.Update(m_fCurTime);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }


                //parameters
                m_fControllOffsetY = 40;
                m_TimelineMouse = new Vector2(offset, m_fControllOffsetY);
                {
                    GUILayout.BeginArea(new Rect(offset, TOP_TOOL_HEIGHT+ m_fControllOffsetY, LEFT_CONTROLLER, position.height - TOP_TOOL_HEIGHT-10));
                    GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.Width(LEFT_CONTROLLER) };
                    {
                        m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
                        DrawParameters(ref m_pCurGroup, layOp);
                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndArea();
                    offset += LEFT_CONTROLLER;
                }

                GUILayout.EndVertical();

                //strips
                UIAnimatorUtil.DrawColorBox(new Rect(offset, TOP_TOOL_HEIGHT, 2, position.height- TOP_TOOL_HEIGHT), Color.green);
                offset += 2;
                m_TimelineRect = new Rect(offset, TOP_TOOL_HEIGHT, position.width - offset, position.height- TOP_TOOL_HEIGHT);
                {
                    if (m_bRecoding)
                    {
                        Color color = GUI.color;
                        GUI.color = new Color(1, 0, 0, 1);
                        GUI.Box(new Rect(m_TimelineRect.x, m_TimelineRect.y+5, m_TimelineRect.width, 35), "");
                        GUI.color = color;
                    }
                    else
                    {
                        Color color = GUI.color;
                        GUI.color = new Color(1, 1, 0, 0.8f);
                        GUI.Box(new Rect(m_TimelineRect.x, m_TimelineRect.y+5, m_TimelineRect.width, 35), "");
                        GUI.color = color;
                    }
                    GUILayout.BeginArea(m_TimelineRect);
                    UIAnimatorUtil.DrawGrid(m_TimelineRect,1f, Vector2.zero);
                    GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.Width(m_TimelineRect.width) };
                    {
                        EditorGUILayout.BeginScrollView(m_Scroll);
                        DrawTimerTracks(ref m_pCurGroup, layOp);
                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndArea();
                }
                int preTime = (int)(m_fCurTime*100);
                m_fCurTime = UIAnimatorUtil.DrawTimelinePanel(new Rect(m_TimelineRect.x, m_TimelineRect.y, m_TimelineRect.width - 5, m_TimelineRect.height), m_fCurTime, maxTime, maxTime * 0.01f);
                if(preTime != (int)(m_fCurTime * 100))
                {
                    if (!m_bPlaying)
                        m_pSplineEditor.Update(m_fCurTime);
                }
            }
            OnController();
        }
        //------------------------------------------------------
        void OnController()
        {
            wantsMouseMove = true;
            Event e = Event.current;
            if (e.type == EventType.MouseDrag)
            {
                if (e.button == 0)
                {
                    if (m_DragTrack != null)
                    {
                        if (m_DragTrack.track != null)
                        {
                            Keyframe[] keys = m_DragTrack.track.keys;
                            Keyframe keyF = keys[m_DragTrack.keyFrame];
                            m_DragTrack.rect.x = Mathf.Max(0, e.mousePosition.x - m_TimelineRect.x);
                            keyF.time = GetMaxTime() * m_DragTrack.rect.x / m_TimelineRect.width;
                            keys[m_DragTrack.keyFrame] = keyF;
                            m_DragTrack.track.keys = keys;
                            m_fCurTime = keyF.time;
                        }
                        else
                        {
                            if(m_DragTrack.parameter !=null)
                            {
                                float fTime = GetMaxTime() * e.delta.x / m_TimelineRect.width;
                                float start = m_DragTrack.parameter.GetTrackFirst()+fTime;
                                if (start >= 0)
                                {
                                    m_DragTrack.parameter.MoveGapTime(fTime);
                                    m_fCurTime += fTime;
                                }
                            }
                        }
                    }
                }
            }
            else if (e.type == EventType.MouseDown)
            {
                m_DragTrack = null;
            }
            else if (e.type == EventType.MouseUp)
            {
                m_DragTrack = null;
            }
        }
        //------------------------------------------------------
        void CordUIParameter(UIAnimatorParameter parameter, bool bFrom = true)
        {
            if (parameter == null) return;
            Transform trans = parameter.GetTransform();
            RectTransform rectTrans = parameter.GetRectTransform();
            Vector3 pos = trans ? trans.position : Vector3.zero;
            Vector3 rot = trans ? trans.eulerAngles : Vector3.zero;
            Vector3 localpos = trans ? trans.localPosition : Vector3.zero;
            Vector3 localrot = trans ? trans.localEulerAngles : Vector3.zero;
            Vector3 scale = trans ? trans.localScale : Vector3.one;
            Vector2 pivot = rectTrans ? rectTrans.pivot : Vector2.one * 0.5f;
            if (trans)
            {
                if (parameter.type == UIAnimatorElementType.POSITION)
                {
                    if (bFrom) parameter.from.setVector3(parameter.bLocal ? localpos : pos);
                    else parameter.to.setVector3(parameter.bLocal ? localpos : pos);
                }
                else if (parameter.type == UIAnimatorElementType.ROTATE)
                {
                    if (bFrom) parameter.from.setVector3(parameter.bLocal ? localrot : rot);
                    else parameter.to.setVector3(parameter.bLocal ? localrot : rot);
                }
                else if (parameter.type == UIAnimatorElementType.SCALE)
                {
                    if (bFrom) parameter.from.setVector3(scale);
                    else parameter.to.setVector3(scale);
                }
            }

            if (parameter.type == UIAnimatorElementType.PIVOT)
            {
                if (rectTrans)
                {
                    if (bFrom) parameter.from.setVector2(rectTrans.pivot);
                    else parameter.to.setVector2(rectTrans.pivot);
                }
            }
            else if (parameter.type == UIAnimatorElementType.COLOR)
            {
                UnityEngine.UI.Graphic grphic = parameter.GetUIGraphic();
                if (grphic)
                {
                    if (bFrom) parameter.from.setColor(grphic.color);
                    else parameter.to.setColor(grphic.color);
                }
            }
            else if (parameter.type == UIAnimatorElementType.ALPAH)
            {
                UnityEngine.CanvasGroup grphic = parameter.GetCanvasGroup();
                if (grphic)
                {
                    if (bFrom) parameter.from.setAlpha(grphic.alpha);
                    else parameter.to.setAlpha(grphic.alpha);
                }
            }
        }
        //------------------------------------------------------
        void CordParam(bool bFrom = true)
        {
            if (m_pCurrentAnimatorParameter != null)
            {
                if (m_pCurrentAnimatorParameter.pEditController != null)
                {
                    CordUIParameter(m_pCurrentAnimatorParameter as UIAnimatorParameter, bFrom);
                }
            }
        }
        //------------------------------------------------------
        private void Update()
        {
            if(!m_bPlaying )
            {
                if(m_pPlayerGroup!=null)
                {
                    m_pPlayerGroup.SetCurrentDelta(m_fCurTime);
                }
            }

            m_pTimer.Update();
            if(m_bPlaying)
            {
                m_pSplineEditor.Update(m_fCurTime);
                if (m_pPlayerGroup != null)
                {
                    m_pPlayerGroup.Update(m_pTimer.deltaTime);
                    m_fCurTime = m_pPlayerGroup.GetCurrentDelta();
                    float trackLen = m_pPlayerGroup.GetTrackDuration();
                    if (m_pPlayerGroup.IsEnd())
                    {
                        DoStop(false);
                    }
                    else
                    {
                        if (SceneView.lastActiveSceneView)
                            SceneView.lastActiveSceneView.Repaint();
                    }
                }
            }
            this.Repaint();
        }
        //------------------------------------------------------
        void DrawTools()
        {
            GUILayout.BeginArea(new Rect(0,0,position.width, TOP_TOOL_HEIGHT));

            UIAnimatorGroupData preGrap = m_pCurGroup;
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) });
            int curIndex = IndexOfGroups(m_pCurGroup);
            int index = EditorGUILayout.Popup(curIndex, m_vAnimatorsPop.ToArray(), new GUILayoutOption[] { GUILayout.Width(180), GUILayout.Height(TOP_TOOL_HEIGHT) });
            if(index!= curIndex && index>=0 && index < m_vAnimatorsPop.Count)
            {
                m_pCurGroup = m_vAnimatorGroups[index];
            }
            if (GUILayout.Button("新建", new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) }))
            {
                m_pCurGroup = new UIAnimatorGroupData();
                m_pCurGroup.nID = NewID();
                m_vAnimatorGroups.Add(m_pCurGroup);
                RefreshPop();
            }
            if(preGrap != m_pCurGroup)
            {
                if (preGrap != null) preGrap.Recovert();
            }
            if(m_pCurGroup != null && m_Backuper.GetController())
            {
                if (GUILayout.Button(m_bPlaying?"播放中":"播放", new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) }))
                {
                    DoPlay();
                }
                if (GUILayout.Button("停止", new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) }))
                {
                    DoStop(true);
                    m_fCurTime = 0;
                }
            }
            if (GUILayout.Button("保存", new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) }))
            {
                Save();
            }
            if (GUILayout.Button("关联文件", new GUILayoutOption[] { GUILayout.Height(TOP_TOOL_HEIGHT) }))
            {
                System.Diagnostics.Process[] prpgress = System.Diagnostics.Process.GetProcesses();

                string args = "";
                string path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + ASSET_FILE;

                args = path.Replace(":/", ":\\");
                args = args.Replace("/", "\\");
                if (path.Contains("."))
                {
                    args = string.Format("/Select, \"{0}\"", args);
                }
                else
                {
                    if (args[args.Length - 1] != '\\')
                    {
                        args += "\\";
                    }
                }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("Explorer.exe", args);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            Debug.Log("IOS 打包路径: " + path);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("open", path));
#endif
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        //------------------------------------------------------
        void DoPlay()
        {
            m_bPlaying = true;
            if(m_pCurGroup!=null && m_Backuper.GetController())
            {
                m_Backuper.Recovert();
                m_pCurGroup.Recovert();
                if(m_pCurGroup.BindTracks!=null)
                {
                    for(int i = 0; i < m_pCurGroup.BindTracks.Length;++i)
                    {
                        UITargetBindTrackParameter param = m_pCurGroup.BindTracks[i];
                        param.datas.Sort((UITargetBindTrackParameter.SplineData s1, UITargetBindTrackParameter.SplineData s2) =>
                        {
                            if (s1.time < s2.time) return -1;
                            return 1;
                        });
                    }
                }
                m_pPlayerGroup.SetGroupData(m_pCurGroup);
                m_pPlayerGroup.SetController(m_Backuper.GetController());
                m_pPlayerGroup.Start(true);
            }
        }
        //------------------------------------------------------
        void DoStop(bool bRecover)
        {
            m_bPlaying = false;
            if(m_pPlayerGroup!=null)
            {
                if (bRecover) m_pPlayerGroup.StopRecover();
                else m_pPlayerGroup.Stop();
            }
        }
        //------------------------------------------------------
        void DrawGroup(ref UIAnimatorGroupData group, GUILayoutOption[] layOp)
        {
            GUILayout.BeginHorizontal();
            Transform parent  = EditorGUILayout.ObjectField("父节点", m_Backuper.GetTransform(), typeof(Transform), true) as Transform;
            m_Backuper.SetController(parent);
            m_bLockParent = EditorGUILayout.Toggle(m_bLockParent);
            EditorGUILayout.LabelField("锁定");
            GUILayout.EndHorizontal();
            int id = EditorGUILayout.IntField("动画id", group.nID);
            if (group.nID != id)
            {
                bool bHas = false;
                for (int i = 0; i < m_vAnimatorGroups.Count; ++i)
                {
                    if(m_vAnimatorGroups[i].nID == id)
                    {
                        bHas = true;
                        break;
                    }
                }
                if(!bHas)
                {
                    group.nID = id;
                    RefreshPop();
                }
            }
            group.desc = EditorGUILayout.TextField("描述", group.desc);
            group.fSpeed = EditorGUILayout.FloatField("速度", group.fSpeed);
            if(GUILayout.Button("添加"))
            {
                List<UIAnimatorParameter> vParams = group.Parameters!=null? new List<UIAnimatorParameter>(group.Parameters):new List<UIAnimatorParameter>();
                UIAnimatorParameter newParam = new UIAnimatorParameter();
                newParam.type= UIAnimatorElementType.COUNT;
                newParam.eControllerType = ELogicController.Widget;
                newParam.isFullPath = false;
                newParam.strControllerName = "";
                newParam.controllerTag = 0;
                newParam.bLocal = true;
                newParam.bOffset = true;
                newParam.delay = 0;
                newParam.delay_times = 0;
                newParam.equation = RtgTween.EQuationType.RTG_EASE_OUT;
                newParam.transition = RtgTween.EEaseType.RTG_LINEAR;
                newParam.time = 1f;
                newParam.pingpong = false;
                newParam.loop = 1;
                newParam.finalValue = 1;
                newParam.initialValue = 0;
                m_pCurrentAnimatorParameter = newParam;
                m_pSplineEditor.SetTarget(null);
                vParams.Add(newParam);
                group.Parameters = vParams.ToArray();
            }
            if (GUILayout.Button("添加路径曲线"))
            {
                List<UITargetBindTrackParameter> vParams = group.BindTracks != null ? new List<UITargetBindTrackParameter>(group.BindTracks) : new List<UITargetBindTrackParameter>();
                UITargetBindTrackParameter newParam = new UITargetBindTrackParameter();
                newParam.eControllerType = ELogicController.GameCamera;
                newParam.isFullPath = false;
                newParam.strControllerName = "";
                newParam.controllerTag = 0;
                newParam.dataDeclare = Core.DeclareKit.InTag_OutTag_Pos_Euler;
                m_pCurrentAnimatorParameter = newParam;
                m_pSplineEditor.SetTarget(newParam);
                vParams.Add(newParam);
                group.BindTracks = vParams.ToArray();
            }
            if (GUILayout.Button("删除"))
            {
                if(EditorUtility.DisplayDialog("提示", "确定删除?", "删除", "取消"))
                {
                    m_vAnimatorGroups.Remove(m_pCurGroup);
                    if (m_pCurGroup != null) m_pCurGroup.Recovert();
                    m_pCurGroup = null;
                    RefreshPop();
                }
            }
        }
        //------------------------------------------------------
        void DrawParameters(ref UIAnimatorGroupData group, GUILayoutOption[] layOp)
        {
            if (group == null) return;
            if(group.Parameters!=null)
            {
                for (int i = 0; i < group.Parameters.Length; ++i)
                {
                    UIAnimatorParameter returnParameter = DrawParameter(group.Parameters[i], layOp);
                    if (returnParameter == null || returnParameter.bDeling)
                    {
                        List<UIAnimatorParameter> vParams = new List<UI.UIAnimatorParameter>(group.Parameters);
                        vParams.RemoveAt(i);
                        if (m_pCurrentAnimatorParameter == returnParameter)
                        {
                            m_pSplineEditor.SetTarget(null);
                            m_pCurrentAnimatorParameter = null;
                        }
                        group.Parameters = vParams.ToArray();
                        break;
                    }
                }
            }
            if(group.BindTracks!=null)
            {
                for (int i = 0; i < group.BindTracks.Length; ++i)
                {
                    UITargetBindTrackParameter returnParameter = DrawBindTrackParameter(group.BindTracks[i], layOp);
                    if (returnParameter == null || returnParameter.bDeling)
                    {
                        List<UITargetBindTrackParameter> vParams = new List<UI.UITargetBindTrackParameter>(group.BindTracks);
                        vParams.RemoveAt(i);
                        if (m_pCurrentAnimatorParameter == returnParameter)
                        {
                            m_pSplineEditor.SetTarget(null);
                            m_pCurrentAnimatorParameter = null;
                        }
                        group.BindTracks = vParams.ToArray();
                        break;
                    }
                }
            }
        }
        //------------------------------------------------------
        UITargetBindTrackParameter DrawBindTrackParameter(UITargetBindTrackParameter parameter, GUILayoutOption[] layOp)
        {
            GUILayout.BeginHorizontal(layOp);

            Color color = GUI.color;
            //left
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(LEFT_CONTROLLER) });
            {
                GUILayout.BeginHorizontal();
                if (m_pCurrentAnimatorParameter == parameter) GUI.color = Color.cyan;
                parameter.bExpand = EditorGUILayout.Foldout(parameter.bExpand, "路径曲线");
                GUI.color = color;
                if (GUILayout.Button("菜单", new GUILayoutOption[] { GUILayout.Width(40) }))
                {
                    ContextMenu.AddItemWithArge("移除", false, OnAnimatorElementMenu, new UIAnimatorUtil.AnimatorElementMenuCatch() { pointer = parameter, useCtl = 1 });
                    if(m_BindTacksStacks.Count>0)
                        ContextMenu.AddItemWithArge("回退", false, OnAnimatorElementMenu, new UIAnimatorUtil.AnimatorElementMenuCatch() { pointer = parameter, useCtl = 2 });
                    ContextMenu.Show();
                }
                GUILayout.EndHorizontal();

                EditorData editorData = parameter.controllRect;
                editorData.rect = GUILayoutUtility.GetLastRect();
                editorData.color = BINDTRACK_PARAMETER_COLOR;
                editorData.start = parameter.GetTrackFirst();
                editorData.length = parameter.GetTimelineLength();
                parameter.controllRect = editorData;

                if (parameter.bExpand)
                {
                    m_pCurrentAnimatorParameter = parameter;
                    UnityEngine.Object preController = parameter.pEditController;
                    System.Type transType = typeof(Transform);
                    if (parameter.eControllerType == ELogicController.GameCamera || parameter.eControllerType == ELogicController.UICamera)
                        transType = typeof(Camera);
                    parameter.pEditController = EditorGUILayout.ObjectField(parameter.pEditController, transType, true) as UnityEngine.Object;
                    if(!parameter.bFirstParent)
                    {
                        parameter.strControllerName = EditorGUILayout.TextField("节点名", parameter.strControllerName);
                        if(parameter.controllerTag!=0)
                            EditorGUILayout.IntField("节点Tag", parameter.controllerTag);
                    }
                    parameter.eControllerType = (ELogicController)EditorGUILayout.EnumPopup(parameter.eControllerType);
                    parameter.bFirstParent = EditorGUILayout.Toggle("根节点", parameter.bFirstParent);
                    m_IngoreFields.Clear();
                    m_IngoreFields.Add("Time");
                    m_IngoreFields.Add("OutTag");
                    m_IngoreFields.Add("InTag");
                    if (Core.DeclareKit.HasDeclare(Core.eDeclareType.Euler, parameter.dataDeclare)) m_IngoreFields.Add("LookAt");
                    if (Core.DeclareKit.HasDeclare(Core.eDeclareType.LookAt, parameter.dataDeclare)) m_IngoreFields.Add("Euler");
                    int delac = parameter.dataDeclare;
                    parameter.dataDeclare = UIAnimatorUtil.DrawBitEnum(typeof(Core.eDeclareType), "数据声明", parameter.dataDeclare, true, m_IngoreFields);
                    if(delac != parameter.dataDeclare)
                    {
                        if (parameter.datas!=null && parameter.datas.Count>0)
                        {
                            TrackBindStack bindStack = new TrackBindStack();
                            bindStack.datas = parameter.datas;
                            bindStack.dataDeclare = delac;
                            m_BindTacksStacks.Push(JsonUtility.ToJson(bindStack));

                            //! replace
                            for (int i = 0; i < parameter.datas.Count; ++i)
                            {
                                UITargetBindTrackParameter.SplineData splineData = parameter.datas[i];
                                List<float> propertys = new List<float>();
                                Core.DeclareKit.ReDeclare(delac, parameter.dataDeclare, bindStack.datas[i].propertys, propertys);
                                splineData.propertys = propertys;
                                parameter.datas[i] = splineData;
                            }
                        }
                    }

                    GUILayout.BeginHorizontal();
                    parameter.expandPropertys = EditorGUILayout.Foldout(parameter.expandPropertys, "属性");
                    GUI.color = color;
                    if (parameter.pEditController!=null && m_pSplineEditor.GetTarget() == parameter && GUILayout.Button("K帧", new GUILayoutOption[] { GUILayout.Width(40) }))
                    {
                        m_pSplineEditor.SetTarget(parameter);
                        bool bNew = true;
                        if (parameter.datas == null) parameter.datas = new List<UITargetBindTrackParameter.SplineData>();
                        for (int i = 0; i < parameter.datas.Count; ++i)
                        {
                            UITargetBindTrackParameter.SplineData spline = parameter.datas[i];
                            if (Mathf.Abs(spline.time - m_fCurTime) <=0.05f)
                            {
                                m_pSplineEditor.SetObjectToProperty(parameter.dataDeclare, spline.propertys);
                                parameter.datas[i] = spline;
                                bNew = false;
                                break;
                            }
                        }
                        if(bNew)
                        {
                            UITargetBindTrackParameter.SplineData spline = new UITargetBindTrackParameter.SplineData();
                            spline.time = m_fCurTime;
                            spline.propertys = new List<float>();
                            Core.DeclareKit.InitDeclare(parameter.dataDeclare, spline.propertys);
                            m_pSplineEditor.SetObjectToProperty(parameter.dataDeclare, spline.propertys);
                            parameter.datas.Add(spline);
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (parameter.expandPropertys && parameter.datas!=null)
                    {
                        bool bSortTime = false;
                        for (int i = 0; i < parameter.datas.Count; ++i)
                        {
                            UITargetBindTrackParameter.SplineData splineData = parameter.datas[i];
                            GUILayout.Label("P[" + i + "]");
                            if (splineData.editorTime <= 0)
                                splineData.editorTime = splineData.time;
                            GUILayout.BeginHorizontal();
                            splineData.editorTime = EditorGUILayout.FloatField("时间", splineData.editorTime);
                            if(splineData.editorTime!= splineData.time && GUILayout.Button("应用", new GUILayoutOption[] { GUILayout.Width(40) }))
                            {
                                GUI.FocusControl("");
                                splineData.time = splineData.editorTime;
                                bSortTime = true;
                            }
                            if(GUILayout.Button("☝", new GUILayoutOption[] { GUILayout.Width(25) }))
                            {
                                GUI.FocusControl("");
                                m_fCurTime = splineData.editorTime;
                            }
                            GUILayout.EndHorizontal();
                            Core.DeclareKit.DrawProperty(delac, splineData.propertys);
                            parameter.datas[i] = splineData;
                        }
                        if(bSortTime)
                        {
                            parameter.datas.Sort((UITargetBindTrackParameter.SplineData s1, UITargetBindTrackParameter.SplineData s2) =>
                            {
                                if (s1.time < s2.time) return -1;
                                return 1;
                            });
                        }
                    }
                    m_pSplineEditor.SetTarget(parameter);
                }
                else
                {
                    if (m_pCurrentAnimatorParameter == parameter)
                    {
                        m_pCurrentAnimatorParameter = null;
                        m_pSplineEditor.SetTarget(null);
                    }
                }
            }
            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
            return parameter;
        }
        //------------------------------------------------------
        UIAnimatorParameter DrawParameter( UIAnimatorParameter parameter, GUILayoutOption[] layOp)
        {
            GUILayout.BeginHorizontal(layOp);

            Color color = GUI.color;
            //left
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(LEFT_CONTROLLER) });
            {
                GUILayout.BeginHorizontal();
                if (m_pCurrentAnimatorParameter == parameter) GUI.color = Color.cyan;

                string parameName = UIAnimatorUtil.GetDispplayName(parameter.type, parameter.type.ToString());
                if (!parameter.bFirstParent)
                {
                    if(!string.IsNullOrEmpty(parameter.strControllerName))
                        parameName += "[节点=" + parameter.strControllerName + "]";
                    if (parameter.controllerTag != 0)
                        parameName += "[Tag=" + parameter.controllerTag + "]";
                }
                parameter.bExpand = EditorGUILayout.Foldout(parameter.bExpand, parameName);
                GUI.color = color;
                if (GUILayout.Button("菜单", new GUILayoutOption[] { GUILayout.Width(40) }))
                {
                    UIAnimatorUtil.BuildElementTypeMenu(null, parameter, OnAnimatorElementMenu);
                    ContextMenu.AddItemWithArge("移除", false, OnAnimatorElementMenu, new UIAnimatorUtil.AnimatorElementMenuCatch() { pointer = parameter, useCtl = 1 });
                    ContextMenu.Show();
                }
                GUILayout.EndHorizontal();

                EditorData editorData = parameter.controllRect;
                editorData.rect = GUILayoutUtility.GetLastRect();
                editorData.color = PARAMETER_COLOR;
                editorData.start = parameter.GetTrackFirst();
                editorData.length = parameter.GetTrackDuration();
                parameter.controllRect = editorData;

                if (parameter.bExpand)
                {
                    UnityEngine.Object preController = parameter.pEditController;
                    System.Type transType = typeof(Transform);
                    if (parameter.eControllerType == ELogicController.GameCamera || parameter.eControllerType == ELogicController.UICamera)
                        transType = typeof(Camera);
                    parameter.pEditController = EditorGUILayout.ObjectField(parameter.pEditController, transType, true) as UnityEngine.Object;
                    if (!parameter.bFirstParent)
                    {
                        parameter.strControllerName = EditorGUILayout.TextField("节点名", parameter.strControllerName);
                        if (parameter.controllerTag != 0)
                            EditorGUILayout.IntField("节点Tag", parameter.controllerTag);
                    }
                    parameter.eControllerType = (ELogicController)EditorGUILayout.EnumPopup(parameter.eControllerType);
                    parameter.bFirstParent = EditorGUILayout.Toggle("根节点", parameter.bFirstParent);
                    if(parameter.type == UIAnimatorElementType.ALPAH)
                    {
                        if(parameter.GetCanvasGroup() == null)
                            EditorGUILayout.HelpBox("请在作用的节点挂CanvasGroup组件", MessageType.Error);
                    }
                    if (parameter.type != UIAnimatorElementType.PIVOT)
                    {
                        parameter.bLocal = true;

                        parameter.time = EditorGUILayout.FloatField("时长", parameter.time);
                        parameter.delay = EditorGUILayout.FloatField("延迟", parameter.delay);
                        parameter.delay_times = EditorGUILayout.IntField("延迟次数间隔", parameter.delay_times);
                        parameter.loop = EditorGUILayout.IntField("循环(0为无限)", parameter.loop);
                        parameter.lerpCurve = EditorGUILayout.CurveField("过渡曲线", parameter.lerpCurve);
                        parameter.transition = (RtgTween.EEaseType)RtgTween.TweenerEditorDrawer.PopEnum("晃动类型", parameter.transition);
                        parameter.equation = (RtgTween.EQuationType)RtgTween.TweenerEditorDrawer.PopEnum("晃动效果", parameter.equation);
                        parameter.pingpong = EditorGUILayout.Toggle("Pingpong", parameter.pingpong);
                  //      parameter.initialValue = EditorGUILayout.FloatField("初始阀值", parameter.initialValue);
                  //     parameter.finalValue = EditorGUILayout.FloatField("最终阀值", parameter.finalValue);
                        if (parameter.finalValue < parameter.initialValue) parameter.finalValue = parameter.initialValue + 0.1f;
                    }
                  //  parameter.bOffset = true;
                    parameter.bOffset = EditorGUILayout.Toggle("相对值", parameter.bOffset);
                    parameter.bRecover = EditorGUILayout.Toggle("结束还原", parameter.bRecover);

                    m_pCurrentAnimatorParameter = parameter;
                    m_pSplineEditor.SetTarget(null);

                    switch (parameter.type)
                    {
                        case UIAnimatorElementType.POSITION:
                            m_pDrawer.DrawPositionTrackValue(parameter);
                            break;
                        case UIAnimatorElementType.ROTATE:
                            m_pDrawer.DrawRotateTrackValue(parameter);
                            break;
                        case UIAnimatorElementType.SCALE:
                            m_pDrawer.DrawScaleTrackValue(parameter);
                            break;
                        case UIAnimatorElementType.PIVOT:
                            m_pDrawer.DrawPivotTrackValue(parameter);
                            break;
                        case UIAnimatorElementType.ALPAH:
                            m_pDrawer.DrawAlphaTrackValue(parameter);
                            break;
                        case UIAnimatorElementType.COLOR:
                            m_pDrawer.DrawColorTrackValue(parameter);
                            break;
                    }
                }
                else
                {
                    if (m_pCurrentAnimatorParameter == parameter)
                    {
                        m_pCurrentAnimatorParameter = null;
                        m_pSplineEditor.SetTarget(null);
                    }
                }
            }
            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
            return parameter;
        }
        //------------------------------------------------------
        public void DrawTimerTracks(ref UIAnimatorGroupData group, GUILayoutOption[] layOp)
        {
            if(group.Parameters!=null)
            {
                for (int i = 0; i < group.Parameters.Length; ++i)
                {
                    UIAnimatorParameter returnParameter = group.Parameters[i];
                    DrawTimelineParameterCtl(returnParameter);
                }
            }
            if (group.BindTracks != null)
            {
                for (int i = 0; i < group.BindTracks.Length; ++i)
                {
                    UITargetBindTrackParameter returnParameter = group.BindTracks[i];
                    DrawTimelineParameterCtl(returnParameter);
                }
            }
        }
        //------------------------------------------------------
        public void DrawTimelineParameterCtl(UIBaseParameter parameter)
        {
            if (parameter.controllRect.rect.width <= 0) return;
            Color color = GUI.color;

            Rect rect = parameter.controllRect.rect;
            rect.y += m_fControllOffsetY;
            rect.xMin = m_TimelineRect.width * parameter.GetTrackFirst() / GetMaxTime();
            rect.xMax = m_TimelineRect.width *(parameter.GetTimelineLength()) / GetMaxTime();
            GUI.color = parameter.controllRect.color;
         //   GUI.Button(rect, parameter.strControllerName);
        //    GUI.Box(rect, Texture2D.blackTexture);
            if (GUI.RepeatButton(rect, Texture2D.blackTexture))
            {
                if (!m_bPlaying)
                    m_fCurTime = parameter.GetTrackFirst();
                if (Event.current.button == 1)
                {
                    m_DragTrack = null;
                    ContextMenu.Show();
                }
                else
                {
                    m_DragTrack = new DragTrackData();
                    m_DragTrack.parameter = parameter;
                    m_DragTrack.track = null;
                    m_DragTrack.keyFrame = -1;
                    m_DragTrack.rect = rect;
                    m_DragTrack.maxTime = GetMaxTime();
                    m_DragTrack.lastTime = -1;
                }
            }
            GUI.color = color;
        }
        public  //------------------------------------------------------
          float DrawTimeStripValue(string label, float vec, System.Action OnLabel = null)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(TIMELINE_HEIGHT) });
            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = STRIPT_TIME_TITLE_WIDTH;
            EditorGUILayout.LabelField(label);
            EditorGUIUtility.labelWidth = width;
            vec = EditorGUILayout.FloatField(vec);
            if (OnLabel != null) OnLabel();
            GUILayout.EndHorizontal();
            return vec;
        }
        //------------------------------------------------------
        public Vector2 DrawTimeStripValue(string label, Vector2 vec, System.Action OnLabel = null)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(TIMELINE_HEIGHT) });
            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = STRIPT_TIME_TITLE_WIDTH;
            EditorGUILayout.LabelField(label);
            EditorGUIUtility.labelWidth = width;
            vec.x = EditorGUILayout.FloatField(vec.x);
            vec.y = EditorGUILayout.FloatField(vec.y);
            if (OnLabel != null) OnLabel();
            GUILayout.EndHorizontal();
            return vec;
        }
        //------------------------------------------------------
        public Vector3 DrawTimeStripValue(string label, Vector3 vec, System.Action OnLabel = null)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(TIMELINE_HEIGHT) });
            float width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = STRIPT_TIME_TITLE_WIDTH;
            EditorGUILayout.LabelField(label);
            EditorGUIUtility.labelWidth = width;
            vec.x = EditorGUILayout.FloatField(vec.x);
            vec.y = EditorGUILayout.FloatField(vec.y);
            vec.z = EditorGUILayout.FloatField(vec.z);
            if (OnLabel != null) OnLabel();
            GUILayout.EndHorizontal();
            return vec;
        }
        //-----------------------------------------------------
        static public void OnSceneFunc(SceneView sceneView)
        {
            if(ms_Instance!=null) ms_Instance.OnSceneGUI(sceneView);
        }
        //-----------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
            if (m_pSplineEditor != null)
                m_pSplineEditor.OnSceneGUI(sceneView);
        }
        //------------------------------------------------------
        public void OnAnimatorElementMenu(System.Object argv)
        {
            if (argv == null) return;
            UIAnimatorUtil.AnimatorElementMenuCatch para = (UIAnimatorUtil.AnimatorElementMenuCatch)argv;
            if(para.pointer != null)
            {
                if(para.pointer is UIBaseParameter)
                {
                    UIBaseParameter parameter = para.pointer as UIBaseParameter;
                    if (parameter != null)
                    {
                        if(para.useCtl == 1)
                        {
                            if (EditorUtility.DisplayDialog("提示", "删除改组", "删除", "取消"))
                            {
                                parameter.bDeling = true;
                                parameter.m_Backuper.Recovert();
                            }
                            return;
                        }
                        if (para.useCtl == 2)
                        {
                            if (EditorUtility.DisplayDialog("提示", "确定回退", "回退", "取消"))
                            {
                                UITargetBindTrackParameter bindParamer = parameter as UITargetBindTrackParameter;
                                if(bindParamer!=null)
                                {
                                    string strData= m_BindTacksStacks.Pop();
                                    try
                                    {
                                        TrackBindStack track = JsonUtility.FromJson<TrackBindStack>(m_BindTacksStacks.Pop());
                                        bindParamer.dataDeclare = track.dataDeclare;
                                        bindParamer.datas = track.datas;
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            return;
                        }
                    }

                    UIAnimatorParameter uiparam = para.pointer as UIAnimatorParameter;
                    if (uiparam != null)
                    {
                        if (para.type != UIAnimatorElementType.COUNT)
                        {
                            uiparam.type = para.type;
                            uiparam.from.Reset();
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void OnEleTrackKeyFrameController(System.Object argv)
        {
            UIAnimatorUtil.MenuCtlCatch para = (UIAnimatorUtil.MenuCtlCatch)argv;
            if (para.pointer != null)
            {
                if (para.pointer is AnimationCurve)
                {
                    AnimationCurve track = para.pointer as AnimationCurve;
                    if (para.ctlType == UIAnimatorUtil.MenuCtlCatch.ECtlType.ResetThis)
                    {
                        Keyframe[] keys = track.keys;

                        Keyframe frame = keys[para.start];
                        frame.value = 0;
                        track.keys = keys;
                    }
                    else if (para.ctlType == UIAnimatorUtil.MenuCtlCatch.ECtlType.RemoveThis)
                    {
                        track.RemoveKey(para.start);
                    }
                }
            }
        }
        //------------------------------------------------------
        public  void OnEaseTypeCallback(System.Object argv)
        {
            UIAnimatorUtil.EaseMenuCatch para = (UIAnimatorUtil.EaseMenuCatch)argv;
            if (para.pointer == null) return;
            if (para.pointer is UIAnimatorParameter)
            {
                (para.pointer as UIAnimatorParameter).transition = para.easeType;
            }
        }
        //------------------------------------------------------
        public void OnQuationTypeCallback(System.Object argv)
        {
            UIAnimatorUtil.QuationMenuCatch para = (UIAnimatorUtil.QuationMenuCatch)argv;
            if (para.pointer == null) return;
            if (para.pointer is UIAnimatorParameter)
            {
                (para.pointer as UIAnimatorParameter).equation = para.quationType;
            }
        }
    }
}
#endif
