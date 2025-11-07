/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Tweener
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
namespace Framework.RtgTween
{
    [UI.UIWidgetExport]
    [ExecuteInEditMode]
    public class Tweener : MonoBehaviour
    {
        public bool autoPlay = false;
        public bool checkView = false;
        public RtgTweenerParam[] Propertys;
        public string fmodEvent;

        private Transform m_pTransform;
        private Vector3 m_LockPoistion = Vector3.zero;
        private bool m_bStatInview = false;
        //------------------------------------------------------
        private void Awake()
        {
            m_pTransform = this.transform;
            m_LockPoistion = m_pTransform.position;
            m_bStatInview = true;
        }
        //------------------------------------------------------
        private void Start()
        {
            m_bStatInview = false;
            if (autoPlay) PlayTween();
        }
        //------------------------------------------------------
        void Backup()
        {
            if (Propertys != null)
            {
                for (int i = 0; i < Propertys.Length; ++i)
                {
                    Propertys[i].pController = this;
                    Propertys[i].BackUp();
                }
            }
        }
        //------------------------------------------------------
        public bool IsPlaying()
        {
            if (Propertys == null) return false;
            for(int i = 0; i < Propertys.Length; ++i)
            {
                if (Propertys[i].tweenerID == 0) continue;
                if (RtgTweenerManager.getInstance().findTween(Propertys[i]))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void PlayTween(float fDelay = 0)
        {
            m_bStatInview = true;
            if (m_pTransform == null) m_pTransform = this.transform;
            m_LockPoistion = m_pTransform.position;
            if (Propertys == null)return;
            if (IsPlaying()) return;
            Backup();
            for (int i = 0; i < Propertys.Length; ++i)
            {
                Propertys[i].bLocal = true;
                Propertys[i].pController = this;
                Propertys[i].tweenerID = RtgTweenerManager.getInstance().addTween(ref Propertys[i], fDelay);
            }
        }
        //------------------------------------------------------
        public void ForcePlayTween(float fDelay = 0)
        {
            m_bStatInview = true;
            if (m_pTransform == null) m_pTransform = this.transform;
            m_LockPoistion = m_pTransform.position;
            if (Propertys == null) return;
            Backup();
            for (int i = 0; i < Propertys.Length; ++i)
            {
                Propertys[i].bLocal = true;
                Propertys[i].pController = this;
                Propertys[i].tweenerID = RtgTweenerManager.getInstance().addTween(ref Propertys[i], fDelay);
            }
        }
        //------------------------------------------------------
        public void StopTween()
        {
            m_bStatInview = false;
            if (m_pTransform == null) m_pTransform = this.transform;
            m_LockPoistion = m_pTransform.position;
            if (Propertys == null) return;
            for (int i = 0; i < Propertys.Length; ++i)
            {
                if(Propertys[i].tweenerID!=0)
                    RtgTweenerManager.getInstance().removeTween(Propertys[i]);
                if(Propertys[i].backupReover)
                    Propertys[i].Recover();
                Propertys[i].tweenerID = 0;
            }
        }
        //------------------------------------------------------
        public void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (Propertys == null) return;
            if (m_pTransform == null) m_pTransform = this.transform;
            m_LockPoistion = m_pTransform.position;
            if(autoPlay && !m_bStatInview) PlayTween();
        }
        //------------------------------------------------------
        public void OnDisable()
        {
            if (Propertys == null) return;
            StopTween();
        }
        //------------------------------------------------------
        private void Update()
        {
            if(checkView)
            {
                if (m_pTransform == null) return;
                Camera camera = Camera.main;
                if (camera == null) return;
                if (!BaseUtil.Equal(m_LockPoistion, m_pTransform.position))
                {
                    m_LockPoistion = m_pTransform.position;
                    if (BaseUtil.PositionInView(camera.cullingMatrix, m_pTransform.position, 1))
                    {
                        if (!m_bStatInview) PlayTween();
                    }
                    else
                    {
                        if (m_bStatInview) StopTween();
                    }
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Tweener), true)]
    public class UITweenerEditor : Editor
    {
        UI.Backuper m_pBackuper = new UI.Backuper();
        TweenerEditorDrawer.ParamData[] m_vDatas = null;
        string m_InputName = "";
        int m_TemplateIdx = 0;
        //------------------------------------------------------
        private void OnEnable()
        {
            TweenerEditorDrawer.OnEnable();
            Tweener tweener = target as Tweener;
            m_vDatas = TweenerEditorDrawer.BuildCheck(tweener.Propertys);
            m_pBackuper.SetController(target);
        }
        //------------------------------------------------------
        private void OnDisable()
        {
            Tweener tweener = target as Tweener;
            if(tweener!=null)
            {
                TweenerEditorDrawer.OnDisable(tweener.gameObject, tweener.Propertys);
            }
            m_pBackuper.Recovert();
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Tweener tweener = target as Tweener;
            GUILayout.BeginHorizontal();
            string subpath = "EditorData/TweenerTemplate";
            string path = Application.dataPath.Replace("Assets", subpath);
            List<string> templistname = BaseUtil.GetDirectoryFilesName(path);

            m_TemplateIdx = EditorGUILayout.Popup(m_TemplateIdx, templistname.ToArray());
            if (GUILayout.Button("使用模板"))
            {
                string filepath = path + "/" + templistname[m_TemplateIdx];
                string content = System.IO.File.ReadAllText(filepath);
                TweenerJson p2 = (TweenerJson)JsonUtility.FromJson(content, typeof(TweenerJson));
                tweener.Propertys = p2.Propertys;
                tweener.checkView = p2.checkView;
                m_vDatas = TweenerEditorDrawer.BuildCheck(tweener.Propertys);
            }
            GUILayout.EndHorizontal();
            tweener.checkView = EditorGUILayout.Toggle("视野检测", tweener.checkView);
            tweener.autoPlay = EditorGUILayout.Toggle("自动播放", tweener.autoPlay);
            tweener.fmodEvent = EditorGUILayout.TextField("FMODEvent", tweener.fmodEvent);
            List<RtgTweenerParam> vTemp = TweenerEditorDrawer.DrawTween(tweener, ref m_vDatas, m_pBackuper);
            tweener.Propertys = vTemp.ToArray();
            serializedObject.ApplyModifiedProperties();

            GUILayout.BeginHorizontal();
            m_InputName = EditorGUILayout.TextField("模板名字", m_InputName, GUILayout.Width(300));
            if (GUILayout.Button("导出模板"))
            {
                ExportTemplateToJson(m_InputName);
            }
            GUILayout.EndHorizontal();
        }

        public void ExportTemplateToJson(string name)
        {
            Tweener p = target as Tweener;
            RtgTweenerParam[] Propertys = p.Propertys;

            string json = JsonUtility.ToJson(p);
            string subpath = "EditorData/TweenerTemplate";
            string path = Application.dataPath.Replace("Assets",subpath);
            System.IO.File.WriteAllText(path + "/" + name + ".json", json);
        }
    }

    public class TweenerJson
    {
        public bool checkView = false;
        public RtgTweenerParam[] Propertys;
    }
    //------------------------------------------------------
    public class TweenerEditorDrawer
    {
        class EditorTimer
        {
            public float m_PreviousTime;
            public float deltaTime = 0.02f;
            public float fixedDeltaTime = 0.02f;
            public float m_fDeltaTime = 0f;
            public float m_currentSnap = 1f;

            //-----------------------------------------------------
            public void Update()
            {
                if (Application.isPlaying)
                {
                    Application.targetFrameRate = 30;
                    deltaTime = Time.deltaTime;
                    m_fDeltaTime = (float)(deltaTime * m_currentSnap);
                }
                else
                {
                    float curTime = Time.realtimeSinceStartup;
                    m_PreviousTime = Mathf.Min(m_PreviousTime, curTime);//very important!!!

                    deltaTime = curTime - m_PreviousTime;
                    m_fDeltaTime = (float)(deltaTime * m_currentSnap);
                }

                m_PreviousTime = Time.realtimeSinceStartup;
            }
        }
        static  EditorTimer m_pTimer = new EditorTimer();
        static RtgTweenerManager m_pManager = new RtgTweenerManager();
        static List<RtgTweenerParam> m_vParams = new List<RtgTweenerParam>();
        static List<RtgTweenerParam> m_vTemps = new List<RtgTweenerParam>();

        static ParamData[] m_vCopyDatas = null;
        public class ParamData
        {
            public ETweenPropertyType type;
            public bool bUsed = false;
            public bool bExpand = false;
            public RtgTweenerProperty backup = new RtgTweenerProperty();
            public RtgTweenerParam param;
            public void Backup()
            {
                backup = param.BackUp();
            }
            //------------------------------------------------------
            public void Recover()
            {
                param.Recover();
            }
            //------------------------------------------------------
            public void Copy(ParamData other)
            {
                type = other.type;
                bUsed = other.bUsed;
                bExpand = other.bExpand;
                backup = other.backup;
                param = other.param;
            }
        }
        //-----------------------------------------------------
        public static void OnEnable()
        {
            EditorApplication.update += OnTweenerUpdate;
        }
        //------------------------------------------------------
        public static void OnDisable(GameObject pGo = null, RtgTweenerParam[] param = null)
        {
            for (int i = 0; i < m_vParams.Count; ++i)
            {
                m_vParams[i].Recover();
            }
            m_vParams.Clear();
            m_pManager.clear();
            if (pGo != null && param == null)
                CheckTweenAlphaParam(pGo, param);
            EditorApplication.update -= OnTweenerUpdate;
        }
        //------------------------------------------------------
        public static bool TestTweenAlphaParam(GameObject target, RtgTweenerParam[] param)
        {
            bool bDirty = false;
            for (int i = 0; i < param.Length; ++i)
            {
                if (param[i].property == ETweenPropertyType.Alpha)
                {

                    UnityEngine.EventSystems.UIBehaviour[] behavs = target.GetComponentsInChildren<UnityEngine.EventSystems.UIBehaviour>();
                    for (int j = 0; j < behavs.Length; ++j)
                    {
                        UnityEngine.UI.Graphic gap = behavs[j] as UnityEngine.UI.Graphic;
                        if (gap != null)
                        {
                            if (gap.color.a <= 0)
                            {
                                gap.color = new Color(gap.color.r, gap.color.g, gap.color.b, 1);
                                EditorUtility.SetDirty(gap);
                                bDirty = true;
                            }
                        }
                        UnityEngine.UI.Shadow shadow = behavs[j] as UnityEngine.UI.Shadow;
                        if (shadow != null)
                        {
                            if (shadow.effectColor.a <= 0)
                            {
                                shadow.effectColor = new Color(shadow.effectColor.r, shadow.effectColor.g, shadow.effectColor.b, 1);
                                bDirty = true;
                                EditorUtility.SetDirty(shadow);
                            }
                        }
                    }
                    if (param[i].GetCanvasGroup())
                    {
                        break;
                    }
                    Transform pBy = param[i].GetTransform();
                    if (pBy == null) pBy = target.transform;
                    CanvasGroup group = pBy.gameObject.GetComponent<CanvasGroup>();
                    if (group == null)
                    {
                        param[i].pBy = pBy.gameObject.AddComponent<CanvasGroup>();
                        EditorUtility.SetDirty(target);
                        bDirty = true;
                    }
                    break;
                }
            }
            return bDirty;
        }
        //------------------------------------------------------
        public static void CheckTweenAlphaParam(GameObject target, RtgTweenerParam[] param)
        {
            if (target == null) return;
            if (param == null)
            {
                CanvasGroup group = target.GetComponent<CanvasGroup>();
                if (group) GameObject.DestroyImmediate(group);
                return;
            }
            for (int i = 0; i < param.Length; ++i)
            {
                if (param[i].property == ETweenPropertyType.Alpha)
                {
                    if(param[i].pBy !=null && param[i].pBy is CanvasGroup)
                    {
                        break;
                    }
                    CanvasGroup group = target.GetComponent<CanvasGroup>();
                    if (group == null)
                    {
                        param[i].pBy = target.AddComponent<CanvasGroup>();
                        EditorUtility.SetDirty(target);
                    }
                    break;
                }
            }
        }
        //------------------------------------------------------
        public static ParamData[] BuildCheck(RtgTweenerParam[] Propertys)
        {
            int cnt = System.Enum.GetNames(typeof(ETweenPropertyType)).Length;
            if (cnt > 0)
            {
                ParamData[] vDatas = new TweenerEditorDrawer.ParamData[cnt];
                if (Propertys != null)
                {
                    for (int i = 0; i < Propertys.Length; ++i)
                    {
                        TweenerEditorDrawer.ParamData paramData = new TweenerEditorDrawer.ParamData();
                        paramData.bUsed = true;
                        paramData.type = Propertys[i].property;
                        paramData.param = Propertys[i];
                        vDatas[(int)Propertys[i].property] = paramData;
                    }
                }

                foreach (Enum v in Enum.GetValues(typeof(ETweenPropertyType)))
                {
                    ETweenPropertyType type = (ETweenPropertyType)v;
                    if (vDatas[(int)type] != null) continue;
                    TweenerEditorDrawer.ParamData paramData = new TweenerEditorDrawer.ParamData();
                    paramData.bUsed = false;
                    paramData.type = type;
                    paramData.param = new RtgTweenerParam();
                    paramData.param.finalValue = 1;
                    paramData.param.time = 1;
                    paramData.param.loop = 1;
                    vDatas[(int)type] = paramData;
                }
                return vDatas;
            }
            return null;
        }
        //------------------------------------------------------
        static void OnTweenerUpdate()
        {
            m_pTimer.Update();
            m_pManager.update((long)(m_pTimer.deltaTime * 1000));
        }
        //-----------------------------------------------------
        public static List<RtgTweenerParam> DrawTween(UnityEngine.Object target, ref ParamData[] vDatas, UI.Backuper backuper)
        {
            m_vTemps.Clear();
            if (vDatas != null)
            {
                if(vDatas.Length >0)
                {
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("复制"))
                    {
                        m_vCopyDatas = vDatas;
                    }
                    if (m_vCopyDatas!=null && m_vCopyDatas != vDatas && m_vCopyDatas.Length>0 && GUILayout.Button("黏贴"))
                    {
                        if(vDatas.Length != m_vCopyDatas.Length)
                        {
                            vDatas = new ParamData[m_vCopyDatas.Length];
                            for(int i = 0; i < m_vCopyDatas.Length; ++i)
                            {
                                vDatas[i] = new ParamData();
                                vDatas[i].Copy(m_vCopyDatas[i]);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                for (int i = 0; i < vDatas.Length; ++i)
                {
                    if (vDatas[i].type == ETweenPropertyType.UserDef) continue;

                    ParamData paramData = vDatas[i];
                    paramData.param.property = paramData.type;

                    if (paramData.type == ETweenPropertyType.Alpha && paramData.param.pBy == null)
                    {
                        Behaviour beha = target as Behaviour;
                        if (beha != null) paramData.param.pBy = beha.GetComponent<CanvasGroup>();
                    }

                    GUILayout.BeginHorizontal();
                    paramData.bExpand = EditorGUILayout.Foldout(paramData.bExpand, paramData.type.ToString());
                    paramData.bUsed = EditorGUILayout.Toggle(paramData.bUsed);
                    GUILayout.EndHorizontal();
                    if(paramData.bExpand)
                    {
                        EditorGUI.indentLevel++;
                        if (vDatas[i].bUsed)
                        {
                            bool bHasBackup = true;

                            paramData.param.pBy = EditorGUILayout.ObjectField("作用对象", paramData.param.pBy, typeof(UnityEngine.Object), true) as UnityEngine.Object;
                            if (vDatas[i].type == ETweenPropertyType.Position)
                            {
                                Vector3 from = paramData.param.from.toVector3();
                                Vector3 to = paramData.param.to.toVector3();
                                from = EditorGUILayout.Vector3Field("from pos", from);
                                to = EditorGUILayout.Vector3Field("to pos", to);
                                paramData.param.from.setVector3(from);
                                paramData.param.to.setVector3(to);
                            }
                            else if (vDatas[i].type == ETweenPropertyType.Rotate)
                            {
                                Vector3 from = paramData.param.from.toVector3();
                                Vector3 to = paramData.param.to.toVector3();
                                from = EditorGUILayout.Vector3Field("from rotate", from);
                                to = EditorGUILayout.Vector3Field("to rotate", to);
                                paramData.param.from.setVector3(from);
                                paramData.param.to.setVector3(to);
                            }
                            else if (vDatas[i].type == ETweenPropertyType.Scale)
                            {
                                Vector3 from = paramData.param.from.toVector3();
                                Vector3 to = paramData.param.to.toVector3();
                                from = EditorGUILayout.Vector3Field("from scale", from);
                                to = EditorGUILayout.Vector3Field("to scale", to);
                                paramData.param.from.setVector3(from);
                                paramData.param.to.setVector3(to);
                            }
                            else if (vDatas[i].type == ETweenPropertyType.Pivot)
                            {
                                bHasBackup = false;
                                Vector2 to = paramData.param.to.toVector2();
                                to = EditorGUILayout.Vector2Field("锚点", to);
                                paramData.param.to.setVector2(to);
                            }
                            else if (vDatas[i].type == ETweenPropertyType.Color)
                            {
                          //      bHasBackup = false;
                                Color from = paramData.param.from.toColor();
                                Color to = paramData.param.to.toColor();
                                from = EditorGUILayout.ColorField("from color", from);
                                to = EditorGUILayout.ColorField("to color", to);
                                paramData.param.from.setColor(from);
                                paramData.param.to.setColor(to);
                            }
                            else if (vDatas[i].type == ETweenPropertyType.Alpha)
                            {
                          //      bHasBackup = false;
                                float from = paramData.param.from.property1;
                                float to = paramData.param.to.property1;
                                from = EditorGUILayout.Slider("from color", from,0,1);
                                to = EditorGUILayout.Slider("to color", to, 0, 1);
                                paramData.param.from.property1 = from;
                                paramData.param.to.property1  = to;
                            }
                            paramData.param.backupReover = EditorGUILayout.Toggle("结束还原", paramData.param.backupReover);
                            if (vDatas[i].type != ETweenPropertyType.Pivot)
                            {
                                if (bHasBackup)
                                    paramData.param.useBackup = EditorGUILayout.Toggle("相对模式", paramData.param.useBackup);
                                //     paramData.param.bLocal = EditorGUILayout.Toggle("局部", paramData.param.bLocal);
                                paramData.param.bLocal = true;

                                paramData.param.time = EditorGUILayout.FloatField("时长", paramData.param.time);
                                paramData.param.delay = EditorGUILayout.FloatField("延迟", paramData.param.delay);
                                paramData.param.delay_times = EditorGUILayout.IntField("延迟次数间隔", paramData.param.delay_times);
                                paramData.param.loop = EditorGUILayout.IntField("循环(0为无限)", paramData.param.loop);
                                paramData.param.lerpCurve = EditorGUILayout.CurveField("过渡曲线横轴(0-1)", paramData.param.lerpCurve);
                                paramData.param.transition = (EEaseType)PopEnum("晃动类型", paramData.param.transition);
                                paramData.param.equation = (EQuationType)EditorGUILayout.EnumPopup("晃动效果", paramData.param.equation);
                                paramData.param.pingpong = EditorGUILayout.Toggle("Pingpong", paramData.param.pingpong);

                                paramData.param.initialValue = 0;
                                paramData.param.finalValue = 1;
                                //     paramData.param.initialValue = EditorGUILayout.FloatField("初始阀值", paramData.param.initialValue);
                                //      paramData.param.finalValue = EditorGUILayout.FloatField("最终阀值", paramData.param.finalValue);
                                if (paramData.param.finalValue < paramData.param.initialValue) paramData.param.finalValue = paramData.param.initialValue + 0.1f;
                            }
                            
                            m_vTemps.Add(vDatas[i].param);
                        }
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        if (vDatas[i].bUsed)
                            m_vTemps.Add(vDatas[i].param);
                    }
                    vDatas[i] = paramData;
                }
            }
            if (GUILayout.Button("刷新"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            if (m_vTemps.Count > 0 && GUILayout.Button("预览"))
            {
                m_pManager.clear();
                for (int i = 0; i < m_vParams.Count; ++i)
                {
                    m_vParams[i].Recover();
                }
                m_vParams.Clear();
                if (backuper != null) backuper.Recovert();
                for (int i = 0; i < m_vTemps.Count; ++i)
                {
                    m_vParams.Add(m_vTemps[i]);
                }
                RtgTweenerParam para;
                for (int i = 0; i < m_vParams.Count; ++i)
                {
                    para = m_vParams[i];
                    para.pController = target;
                    para.BackUp();
                    para.tweenerID = m_pManager.addTween(ref para);
                    m_vParams[i] = para;
                }
            }
            if (GUILayout.Button("停止"))
            {
                for (int i = 0; i < m_vParams.Count; ++i)
                {
                    m_vParams[i].Recover();
                }
                m_vParams.Clear();
                m_pManager.clear();
            }

            return m_vTemps;
        }
        //-----------------------------------------------------
        static List<string> EnumPops = new List<string>();
        static List<Enum> EnumValuePops = new List<Enum>();
        public static Enum PopEnum(string strDisplayName, Enum curVar, System.Type enumType = null, GUILayoutOption[] layOps = null)
        {
            if (enumType == null)
                enumType = curVar.GetType();
            EnumPops.Clear();
            EnumValuePops.Clear();
            int index = -1;
            foreach (Enum v in Enum.GetValues(enumType))
            {
                System.Reflection.FieldInfo fi = enumType.GetField(v.ToString());
                string strTemName = v.ToString();
                if (fi != null && fi.IsDefined(typeof(Base.DisableGUIAttribute)))
                    continue;
                if (fi != null && fi.IsDefined(typeof(Base.PluginDisplayAttribute)))
                {
                    strTemName = fi.GetCustomAttribute<Base.PluginDisplayAttribute>().displayName;
                }
                EnumPops.Add(strTemName);
                EnumValuePops.Add(v);
                if (v.ToString().CompareTo(curVar.ToString()) == 0)
                    index = EnumPops.Count - 1;
            }

            if (string.IsNullOrEmpty(strDisplayName))
                index = EditorGUILayout.Popup(index, EnumPops.ToArray(), layOps);
            else
                index = EditorGUILayout.Popup(strDisplayName, index, EnumPops.ToArray(), layOps);
            if (index >= 0 && index < EnumValuePops.Count)
            {
                curVar = EnumValuePops[index];
            }
            EnumPops.Clear();
            EnumValuePops.Clear();

            return curVar;
        }
    }
#endif
}

