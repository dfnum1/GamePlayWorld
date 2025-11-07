/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	FbxAnimationCliper
作    者:	HappLI
描    述:	动画帧拆分
*********************************************************************/
using Framework.ED;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace TopGame.ED
{
    public class FbxAnimationCliper : EditorWindow
    {
        public class ImportData
        {
            public string strPath;
            public ModelImporter importer;
            public GameObject srcObject;

            public bool bExpand = false;
            public List<ModelImporterClipAnimation> clips;
        }
        public static FbxAnimationCliper Instance { protected set; get; }

        public string assetFileName = "";
        public ModelImporter m_Source;
        public ModelImporter m_Target;
        public GameObject m_SourceObj;
        public GameObject m_TargetObj;

        Vector2 m_SourceClipPos = Vector2.zero;
        Vector2 m_TargetClipPos = Vector2.zero;

        public List<AnimationClip> newTargetClips = new List<AnimationClip>();
        bool m_bReimportAfter = false;
        bool m_bCopyController = true;

        private List<ImportData> m_vDatas = new List<ImportData>();
        public bool reimportAfter
        {
            get { return m_bReimportAfter; }
        }
        [MenuItem("Assets/裁切动画")]
        static void ToolOpen()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return;
            List<ImportData> Fbx = null;
            if (Instance == null) Fbx = new List<ImportData>();
            else
                Fbx = Instance.m_vDatas;

            for (int i = 0; i < Selection.gameObjects.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(Selection.gameObjects[i]);
                ImportFbx(Selection.gameObjects[i], AssetImporter.GetAtPath(path) as ModelImporter, Fbx);
            }
            if (Fbx.Count <= 0) return;

            if (Instance == null)
            {
                FbxAnimationCliper window = EditorWindow.GetWindow<FbxAnimationCliper>();
            }
            Instance.m_vDatas = Fbx;
        }

        public static void Init()
        {
            if (Instance == null)
            {
                FbxAnimationCliper window = EditorWindow.GetWindow<FbxAnimationCliper>();
            }
        }
        //-----------------------------------------------------
        private void OnDisable()
        {
            assetFileName = "";
            if(m_vDatas.Count>0)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            m_vDatas.Clear();
        }
        //-----------------------------------------------------
        private void OnEnable()
        {
            Instance = this;
            base.minSize = new Vector2(850f, 320f);
        }
        //-----------------------------------------------------
        public void setSourceTarget(GameObject source, GameObject targetObj, ModelImporter target)
        {
            m_Source = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(source)) as ModelImporter;
            m_Target = target;
            assetFileName = target.assetPath;
            m_TargetObj = targetObj;
            m_SourceObj = source;
        }
        //-----------------------------------------------------
        public static void OptimizeFbx(ModelImporter target)
        {
            if (target != null)
            {
                ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.High;
                float animationScaleError = 0.4f;
                float animationPositionError = 0.4f;
                float animationRotationError = 0.4f;
                string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
                if (!string.IsNullOrEmpty(path))
                {
                    string datapath = path.Replace("Assets/", Application.dataPath + "/");
                    string srcPath = Path.ChangeExtension(datapath, ".txt");
                    if(File.Exists(srcPath))
                    {
                        string strContext = File.ReadAllText(srcPath);
                        Regex regexNum = new Regex(" *(?<error>[0-9]+)",
                                        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

                        {
                            Regex regex = new Regex("scaleerror", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            Match match = regex.Match(strContext);
                            if (match.Success)
                            {
                                match = regexNum.Match(strContext, match.Index + match.Length);
                                if (match.Success)
                                {
                                    int temp;
                                    if (int.TryParse(match.Groups["error"].Value.Trim(), out temp)) animationScaleError = temp * 0.01f;
                                }
                            }
                        }
                        {
                            Regex regex = new Regex("positionerror", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            Match match = regex.Match(strContext);
                            if (match.Success)
                            {
                                match = regexNum.Match(strContext, match.Index + match.Length);
                                if (match.Success)
                                {
                                    int temp;
                                    if (int.TryParse(match.Groups["error"].Value.Trim(), out temp)) animationPositionError = temp * 0.01f;
                                }
                            }
                        }
                        {
                            Regex regex = new Regex("rotationError", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            Match match = regex.Match(strContext);
                            if (match.Success)
                            {
                                match = regexNum.Match(strContext, match.Index + match.Length);
                                if (match.Success)
                                {
                                    int temp;
                                    if (int.TryParse(match.Groups["error"].Value.Trim(), out temp)) animationRotationError = temp * 0.01f;
                                }
                            }
                        }
                        {
                            Regex regex = new Regex("meshCompression", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            Match match = regex.Match(strContext);
                            if (match.Success)
                            {
                                match = regexNum.Match(strContext, match.Index + match.Length);
                                if (match.Success)
                                {
                                    int temp;
                                    if (int.TryParse(match.Groups["error"].Value.Trim(), out temp)) meshCompression = (ModelImporterMeshCompression)temp;
                                }
                            }
                            
                        }
                    }
                }

                bool bDirty = false;
                if (target.meshCompression != meshCompression)
                {
                    bDirty = true;
                    target.meshCompression = meshCompression;
                }
                if (target.importBlendShapes)
                {
                    bDirty = true;
                    target.importBlendShapes = false;
                }
                if (target.importCameras)
                {
                    bDirty = true;
                    target.importCameras = false;
                }

                if (target.animationCompression != ModelImporterAnimationCompression.KeyframeReduction ||
                    Mathf.Abs(target.animationScaleError - animationScaleError) >= 0.099f ||
                    Mathf.Abs(target.animationPositionError - animationPositionError) >= 0.099f ||
                    Mathf.Abs(target.animationRotationError - animationRotationError) >= 0.099f)
                {
                    bDirty = true;
                    target.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;
                    target.animationScaleError = animationScaleError;
                    target.animationPositionError = animationPositionError;
                    target.animationRotationError = animationRotationError;
                }
                if (bDirty) target.SaveAndReimport();
            }
        }
        //-----------------------------------------------------
        public static AnimationClip OptmizeAnimationFloat(AnimationClip clip)
        {
            //浮点数精度压缩到f3
            AnimationClipCurveData[] curves = null;
            curves = AnimationUtility.GetAllCurves(clip);
            Keyframe key;
            Keyframe[] keyFrames;
            for (int ii = 0; ii < curves.Length; ++ii)
            {
                AnimationClipCurveData curveDate = curves[ii];
                if (curveDate.curve == null || curveDate.curve.keys == null)
                {
                    //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
                    continue;
                }
                keyFrames = curveDate.curve.keys;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    key = keyFrames[i];
                    key.value = float.Parse(key.value.ToString("f3"));
                    key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                    key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                    keyFrames[i] = key;
                }
                curveDate.curve.keys = keyFrames;
                clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
            }

            return clip;
        }
        //-----------------------------------------------------
        public static AnimationClip OptmizeAnimationScaleCurve(AnimationClip clip, float scalerError = 0.01f)
        {
            //             //去除scale曲线
            //             foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(clip))
            //             {
            //                 string name = theCurveBinding.propertyName.ToLower();
            //                 if (name.Contains("scale"))
            //                 {
            //                     AnimationUtility.SetEditorCurve(clip, theCurveBinding, null);
            //                 }
            //             }
            // 
            //             return clip;

            AnimationClipCurveData[] curves = null;
            curves = AnimationUtility.GetAllCurves(clip);
            Keyframe key;
            Keyframe[] keyFrames;
            for (int ii = 0; ii < curves.Length; ++ii)
            {
                AnimationClipCurveData curveDate = curves[ii];
                if (curveDate.curve == null || curveDate.curve.keys == null || curveDate.curve.length<=0)
                {
                    //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
                    continue;
                }
                if(!curveDate.propertyName.Contains("scale"))
                {
                    continue;
                }
                keyFrames = curveDate.curve.keys;
                Keyframe frame0 = keyFrames[0];
                if(Mathf.Abs(frame0.value- 1) > 0.01f)
                {
                    bool bOptimze = true;
                    float maxTime = keyFrames[keyFrames.Length - 1].time;
                    float delta = 0;
                    while(delta <= maxTime)
                    {
                        if( Mathf.Abs(curveDate.curve.Evaluate(delta)- frame0.value) > 0.01f )
                        {
                            bOptimze = false;
                            break;
                        }
                        delta += 0.03333f;
                    }
                    if(bOptimze)
                    {
                        EditorCurveBinding binding = new EditorCurveBinding();
                        binding.propertyName = curveDate.propertyName;
                        binding.path = curveDate.path;
                        binding.type = curveDate.type;
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                    }
                }
            }

            return clip;
        }
        //-----------------------------------------------------
        public static bool ImportFbx(GameObject targetObj, ModelImporter target, List<ImportData> vData = null)
        {
            if (target == null || targetObj == null) return false;
            OptimizeFbx(target);
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            if (string.IsNullOrEmpty(path)) return false;
            string datapath = path.Replace("Assets/", Application.dataPath + "/");
            string srcPath = Path.ChangeExtension(datapath, ".txt");
            if (File.Exists(srcPath))
            {
                try
                {
                    if(vData == null)
                    {
                        if (FbxAnimationCliper.Instance == null)
                            FbxAnimationCliper.Init();
                        vData = Instance.m_vDatas;
                    }

                    for (int i = 0; i < vData.Count; ++i)
                    {
                        if (vData[i].strPath.CompareTo(vData[i].strPath) == 0)
                            return false;
                    }
                    List<ModelImporterClipAnimation> List = new List<ModelImporterClipAnimation>();
                    StreamReader reader = new StreamReader(File.Open(srcPath, FileMode.Open));
                    string sAnimList = reader.ReadToEnd();
                    ParseAnimFile(sAnimList, ref List);

                    vData.Add(new ImportData() { strPath = path, importer = target, srcObject = targetObj, clips = List });
                    reader.Close();
                    return true;
                }
                catch (System.Exception ex)
                {

                }
            }
            return false;
        }
        //-----------------------------------------------------
        static bool ms_bReImport = false;
        public static void setTarget(GameObject targetObj, ModelImporter target)
        {
            if (ms_bReImport) return;
            string path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            if (string.IsNullOrEmpty(path)) return;

            string fbxPath = Path.ChangeExtension(path, ".controller");
            string datapath = path.Replace("Assets/", Application.dataPath + "/");
            string srcPath = Path.ChangeExtension(datapath, ".txt");

            string strAnimRoot = path.Substring(0, path.Length- Path.GetFileName(path).Length);
     //       AnimatorController targetController = AnimatorController.CreateAnimatorControllerAtPath(fbxPath);

            if (File.Exists(srcPath))
            {
                try
                {
                    //          targetController.AddLayer("Base Layer");
                    //          AnimatorControllerLayer controllerLayer = targetController.layers[0];

                    List<ModelImporterClipAnimation> List = new List<ModelImporterClipAnimation>();
                    StreamReader reader = new StreamReader(File.Open(srcPath, FileMode.Open));
                    string sAnimList = reader.ReadToEnd();
                    ParseAnimFile(sAnimList, ref List);

                    target.clipAnimations = List.ToArray();

                    //for (int i = 0; i < List.Count; ++i)
                    //{
                    //    ModelImporterClipAnimation clip = (ModelImporterClipAnimation)List[i];

                    //    AnimatorState pState = controllerLayer.stateMachine.AddState(clip.name);
                    //    string anim = strAnimRoot + clip.name + ".anim";
                    //    AnimationClip src = AssetDatabase.LoadAssetAtPath<AnimationClip>(anim);
                    //    pState.motion = src;
                    //}

                    ms_bReImport = true;
                    AssetDatabase.ImportAsset(path);
                    ms_bReImport = false;
                }
                catch (System.Exception ex)
                {
                    
                }
            }
        }
        //-----------------------------------------------------
        static void ApplayClip(ImportData importData, bool bOver)
        {
            OptimizeFbx(importData.importer);
            CreateController(importData);
            FbxAnimationFilter.FilterClip(importData, false, false);
            if (bOver)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
        //------------------------------------------------------
        public static void ParseAnimFile(string sAnimList, ref List<ModelImporterClipAnimation> List)
        {
            Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            Match match = regexString.Match(sAnimList, 0);
            while (match.Success)
            {
                ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

                if (match.Groups["firstFrame"].Success)
                {
                    clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
                }
                if (match.Groups["lastFrame"].Success)
                {
                    clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
                }
                if (match.Groups["loop"].Success)
                {
                    clip.loopTime = match.Groups["loop"].Value == "loop";
                    clip.loop = clip.loopTime;
                }
                if (match.Groups["name"].Success)
                {
                    clip.name = match.Groups["name"].Value;
                }

                List.Add(clip);

                match = regexString.Match(sAnimList, match.Index + match.Length);
            }
        }
        //-----------------------------------------------------
        public static void CreateController(ImportData importData)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(importData.strPath);
            string path = System.IO.Path.GetDirectoryName(importData.strPath) + "/" + name + ".controller";
            AnimatorController controller = null;
            if (File.Exists(path))
            {
                controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
            }
            else
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path);// new AnimatorController();
            }
            if (controller == null)
            {
                return;
            }
            Dictionary<string, AnimatorState> vStates = new Dictionary<string, AnimatorState>();
            for (int i = 0; i < controller.layers.Length; ++i)
            {
                if (controller.layers[i].stateMachine == null) continue;

                List<AnimatorState> states = EditorUtil.GetStatesRecursive(controller.layers[i].stateMachine);
                foreach (var db in states)
                {
                    vStates.Add(db.name, db);
                }
            }
            if (controller.layers == null || controller.layers.Length <= 0)
            {
                controller.AddLayer("Base Layer");

            }
            for (int i = 0; i < importData.clips.Count; ++i)
            {
                if (vStates.ContainsKey(importData.clips[i].name)) continue;
                controller.layers[0].stateMachine.AddState(importData.clips[i].name);
            }
        }
        //-----------------------------------------------------
        public void CopyController(string targetpath)
        {
            if (m_SourceObj == null || !m_bReimportAfter)
            {
                m_bReimportAfter = false;
                return;
            }
            m_bReimportAfter = false;
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            string path = m_Source.assetPath;
            path = path.Replace("FBX", "controller");
            if (File.Exists(path))
            {
                List<AnimationClip> clips = new List<AnimationClip>();

                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(targetpath);
                for (int i = 0; i < objs.Length; ++i)
                {
                    if (objs[i] is AnimationClip)
                        clips.Add(objs[i] as AnimationClip);
                }

                Animator animator = m_SourceObj.GetComponent<Animator>();
                AnimatorController controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
                string animAssetPath = Path.GetPathRoot(m_Source.assetPath);
                string targetControllerName = Path.GetFileNameWithoutExtension(targetpath);
                AnimatorController targetController = AnimatorController.CreateAnimatorControllerAtPath(animAssetPath + targetControllerName + ".controller");// new AnimatorController();
                                                                                                                                                              //EditorUtility.CopySerialized(controller, targetController);
                targetController.name = targetControllerName;

                if (clips != null)
                {
                    for (int i = 0; i < controller.layers.Length; ++i)
                    {
                        if (controller.layers[i].stateMachine == null) continue;
                        targetController.RemoveLayer(i);
                        targetController.AddLayer(controller.layers[i].name);

                        AnimatorControllerLayer controllerLayer = targetController.layers[i];

                        List<AnimatorState> states = EditorUtil.GetStatesRecursive(controller.layers[i].stateMachine);
                        foreach (var db in states)
                        {
                            AnimatorState state = controllerLayer.stateMachine.AddState(db.name);
                            if (db.motion == null) continue;
                            for (int j = 0; j < clips.Count; ++j)
                            {
                                if (clips[j].name.CompareTo(db.motion.name) == 0)
                                {
                                    //    db.motion = clips[j];
                                    state.motion = clips[j];
                                }
                            }
                        }
                    }
                }


                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                //    targetController.name = Util.GetFileName(m_Target.assetPath);
                //     AssetDatabase.CreateAsset(targetController, animAssetPath + targetController.name + ".controller");
            }
        }
        //-----------------------------------------------------
        private void OnGUI()
        {
            if (GUILayout.Button("批量应用"))
            {
                FbxAnimationFilter.ms_vClearedDirs.Clear();
                for (int i = 0; i < m_vDatas.Count; ++i)
                    ApplayClip(m_vDatas[i], i == m_vDatas.Count-1);
                FbxAnimationFilter.ms_vClearedDirs.Clear();
            }
            m_SourceClipPos = GUILayout.BeginScrollView(m_SourceClipPos);
            for(int i = 0; i < m_vDatas.Count; ++i)
            {
                if (m_vDatas[i].importer == null) continue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(m_vDatas[i].srcObject.name, m_vDatas[i].importer, typeof(ModelImporter),false);
                if (GUILayout.Button("应用"))
                {
                    FbxAnimationFilter.ms_vClearedDirs.Clear();
                    ApplayClip(m_vDatas[i], true);
                    FbxAnimationFilter.ms_vClearedDirs.Clear();
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                for(int a = 0; a < m_vDatas[i].clips.Count; ++a)
                {
                    m_vDatas[i].clips[a].name = EditorGUILayout.TextField("名称",m_vDatas[i].clips[a].name);
                    EditorGUI.indentLevel++;
                    m_vDatas[i].clips[a].loop = EditorGUILayout.Toggle("循环",m_vDatas[i].clips[a].loop);
                    m_vDatas[i].clips[a].firstFrame = EditorGUILayout.IntField("开始帧", (int)m_vDatas[i].clips[a].firstFrame);
                    m_vDatas[i].clips[a].lastFrame = EditorGUILayout.IntField("结束帧", (int)m_vDatas[i].clips[a].lastFrame);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndScrollView();
            return;
            GUILayout.BeginArea(new Rect(0, 0, position.width / 2 - 60, position.height));
            GUILayout.Box("Source Clip Animation Datas", new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
            if (m_Source != null)
            {
                GUILayout.Box(AssetDatabase.GetAssetPath(m_Source), new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
                m_SourceClipPos = GUILayout.BeginScrollView(m_SourceClipPos);

                if (m_Source.clipAnimations != null)
                {
                    for (int i = 0; i < m_Source.clipAnimations.Length; ++i)
                    {
                        ModelImporterClipAnimation clipInfo = m_Source.clipAnimations[i];
                        GUILayout.BeginVertical();
                        GUILayout.Space(3);
                        GUILayout.Box("Clip " + clipInfo.name, new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
                        GUILayout.EndVertical();

                        clipInfo.name = EditorGUILayout.TextField("Aniamtion Name", clipInfo.name);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        GUILayout.EndHorizontal();
                        clipInfo.firstFrame = EditorGUILayout.FloatField("Start Frame", clipInfo.firstFrame);
                        clipInfo.lastFrame = EditorGUILayout.FloatField("End Frame", clipInfo.lastFrame);
                        clipInfo.loop = EditorGUILayout.Toggle("Loop", clipInfo.loop);
                        clipInfo.mirror = EditorGUILayout.Toggle("Mirror", clipInfo.mirror);
                        clipInfo.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", clipInfo.wrapMode);
                    }
                }
                else
                {
                    GUILayout.TextArea("Empty Clip");
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(position.width / 2 - 60, position.height / 2 - 5, 120, 120));
            m_bCopyController = GUILayout.Toggle(m_bCopyController, "Controller Applay");
            if (GUILayout.Button("Applay->"))
            {
                if (m_Target != null && m_Source != null)
                {
                    m_bReimportAfter = true;
                    Selection.activeObject = null;
                    m_Target.generateAnimations = ModelImporterGenerateAnimations.GenerateAnimations;
                    m_Target.clipAnimations = m_Source.clipAnimations;
                    m_Target.SaveAndReimport();
                    m_Target.name = m_Source.name;
                    m_Target.assetBundleName = m_Source.assetBundleName;
                    //    AssetDatabase.DeleteAsset(m_Source.assetPath);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                    if (m_bCopyController)
                        CopyController(assetFileName);
                    m_bReimportAfter = false;
                }
                else if (m_Target != null)
                {
                    m_Target.SaveAndReimport();
                }
                m_Target = null;
                Close();

                return;
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(position.width / 2 + 60, 0, position.width - position.width / 2 + 60, position.height));
            GUILayout.Box("Target Clip Animation Datas", new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
            if (m_Target != null)
            {
                GUILayout.Box(AssetDatabase.GetAssetPath(m_Target), new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
                m_TargetClipPos = GUILayout.BeginScrollView(m_TargetClipPos);
                if (m_Target.clipAnimations != null)
                {
                    for (int i = 0; i < m_Target.clipAnimations.Length; ++i)
                    {
                        ModelImporterClipAnimation clipInfo = m_Target.clipAnimations[i];
                        GUILayout.BeginVertical();
                        GUILayout.Space(3);
                        GUILayout.Box("Clip " + clipInfo.name, new GUILayoutOption[] { GUILayout.MaxWidth(position.width / 2) });
                        GUILayout.EndVertical();

                        clipInfo.name = EditorGUILayout.TextField("Aniamtion Name", clipInfo.name);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        GUILayout.EndHorizontal();
                        clipInfo.firstFrame = EditorGUILayout.FloatField("Start Frame", clipInfo.firstFrame);
                        clipInfo.lastFrame = EditorGUILayout.FloatField("End Frame", clipInfo.lastFrame);
                        clipInfo.loop = EditorGUILayout.Toggle("Loop", clipInfo.loop);
                        clipInfo.mirror = EditorGUILayout.Toggle("Mirror", clipInfo.mirror);
                        clipInfo.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", clipInfo.wrapMode);
                    }
                }
                else
                {
                    GUILayout.TextArea("Empty Clip");
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
    }
}