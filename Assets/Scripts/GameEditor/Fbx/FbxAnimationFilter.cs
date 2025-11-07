/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	FbxAnimationFilter
作    者:	HappLI
描    述:	fbx 动画剥离器
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace TopGame.ED
{
    public class FbxAnimationFilter : EditorWindow
    {
        public static FbxAnimationFilter Instance { protected set; get; }

        public string assetFileName = "";
        public ModelImporter m_Source;
        public ModelImporter m_Target;
        public GameObject m_SourceObj;
        public GameObject m_TargetObj;

        Vector2 m_SourceClipPos = Vector2.zero;
        Vector2 m_TargetClipPos = Vector2.zero;

        public static HashSet<string> ms_vClearedDirs = new HashSet<string>();
        public List<AnimationClip> newTargetClips = new List<AnimationClip>();
        bool m_bReimportAfter = false;

        private List<FbxAnimationCliper.ImportData> m_vDatas = new List<FbxAnimationCliper.ImportData>();
        public bool reimportAfter
        {
            get { return m_bReimportAfter; }
        }
        [MenuItem("Assets/剥离动画")]
        static void ToolOpen()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0) return;
            List<FbxAnimationCliper.ImportData> Fbx = null;
            if (Instance == null) Fbx = new List<FbxAnimationCliper.ImportData>();
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
                FbxAnimationFilter window = EditorWindow.GetWindow<FbxAnimationFilter>();
            }
            Instance.m_vDatas = Fbx;
        }
        //-----------------------------------------------------
        public static void Init()
        {
            if (Instance == null)
            {
                FbxAnimationFilter window = EditorWindow.GetWindow<FbxAnimationFilter>();
            }
        }
        [MenuItem("Assets/批量剥离动画")]
        public static void BatchFilters()
        {
            string batchDir = "Assets/DatasRef/Role";
            UnityEngine.Object selct = Selection.activeObject;
            if(selct!=null )
            {
                string  dir = AssetDatabase.GetAssetPath(selct);
                if(!System.IO.Path.HasExtension(dir) && System.IO.Directory.Exists(dir))
                {
                    batchDir = dir;
                }
            }
            List<FbxAnimationCliper.ImportData> vDatas = new List<FbxAnimationCliper.ImportData>();
           
            string[] assets = AssetDatabase.FindAssets("t:GameObject", new string[]{ batchDir});
            for(int i = 0; i < assets.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[i]);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if(!string.IsNullOrEmpty(name) && System.IO.Path.GetExtension(path).ToLower() == ".fbx" && name[0] >= '0' && name[0] <= '9' )
                {
                    FbxAnimationCliper.ImportData import = new FbxAnimationCliper.ImportData();
                    ImportFbx(AssetDatabase.LoadAssetAtPath<GameObject>(path), AssetImporter.GetAtPath(path) as ModelImporter, vDatas);

                }
            }
            if (vDatas.Count <= 0) return;

            if (Instance == null)
            {
                FbxAnimationFilter window = EditorWindow.GetWindow<FbxAnimationFilter>();
            }
            Instance.m_vDatas = vDatas;
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
        public static bool ImportFbx(GameObject targetObj, ModelImporter target, List<FbxAnimationCliper.ImportData> vData = null)
        {
            if (target == null || targetObj == null) return false;
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
                        if (vData[i].strPath.CompareTo(path) == 0)
                            return false;
                    }
                    List<ModelImporterClipAnimation> List = new List<ModelImporterClipAnimation>();
                    StreamReader reader = new StreamReader(File.Open(srcPath, FileMode.Open));
                    string sAnimList = reader.ReadToEnd();
                    FbxAnimationCliper.ParseAnimFile(sAnimList, ref List);

                    vData.Add(new FbxAnimationCliper.ImportData() { strPath = path, importer = target, srcObject = targetObj, clips = List });
                    reader.Close();
                    return true;
                }
                catch (System.Exception ex)
                {

                }
            }
            else
            {
                if (vData == null)
                {
                    if (FbxAnimationCliper.Instance == null)
                        FbxAnimationCliper.Init();
                    vData = Instance.m_vDatas;
                }

                for (int i = 0; i < vData.Count; ++i)
                {
                    if (vData[i].strPath.CompareTo(path) == 0)
                        return false;
                }
                List<ModelImporterClipAnimation> List = new List<ModelImporterClipAnimation>(target.clipAnimations);
                StringBuilder strContext = new StringBuilder();
                for(int i = 0; i < List.Count; ++i)
                {
                    strContext.AppendFormat("{0}-{1}", (int)List[i].firstFrame, (int)List[i].lastFrame);
                    if(List[i].loopTime || List[i].loop)
                    strContext.Append(" loop");
                    strContext.Append(" " + List[i].name);
                    strContext.AppendLine();
                }
                File.WriteAllText(srcPath, strContext.ToString());
                vData.Add(new FbxAnimationCliper.ImportData() { strPath = path, importer = target, srcObject = targetObj, clips = List });
                return true;
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
                    FbxAnimationCliper.ParseAnimFile(sAnimList, ref List);

                    target.clipAnimations = List.ToArray();

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
        public static void FilterClip(FbxAnimationCliper.ImportData importData, bool bOver, bool bCheckDir = true)
        {
            Dictionary<AnimatorState, StateClip>  stateMapings = CollectStateClips(importData);
            importData.importer.clipAnimations = importData.clips.ToArray();
            importData.importer.SaveAndReimport();

            FbxAnimationCliper.OptimizeFbx(importData.importer);
            List<AnimationClip> clips = new List<AnimationClip>();
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(importData.strPath);
            for (int i = 0; i < objs.Length; ++i)
            {
                if (objs[i].ToString().Contains("PreviewAnimationClip"))
                {
                    continue;
                }
                if (objs[i] is AnimationClip)
                    clips.Add(objs[i] as AnimationClip);
            }
            string strDir = System.IO.Path.GetDirectoryName(importData.strPath).Replace("\\", "/") + "/animations/";
            //      if (strDir.Contains("/DatasRef/")) strDir = strDir.Replace("/DatasRef/", "/Datas/");
            if (bCheckDir )
            {
                if(!ms_vClearedDirs.Contains(strDir))
                {
                    if (System.IO.Directory.Exists(strDir))
                    {
                        System.IO.Directory.Delete(strDir, true);
                    }
                    ms_vClearedDirs.Add(strDir);
                }
            }

            if (clips.Count>0)
            {
                if(!System.IO.Directory.Exists(strDir))
                    System.IO.Directory.CreateDirectory(strDir);
            }

            Dictionary<string, AnimationClip> vMapping = new Dictionary<string, AnimationClip>();
            foreach (AnimationClip Asset in clips)
            {
                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(Asset, newClip);
                newClip.name = importData.srcObject.name + "_" + Asset.name;
                newClip.legacy = false;

                //optmize
                FbxAnimationCliper.OptmizeAnimationScaleCurve(newClip);
                FbxAnimationCliper.OptmizeAnimationFloat(newClip);

                string strFile = strDir + newClip.name + ".anim";
                AssetDatabase.CreateAsset(newClip, strFile);

                vMapping[Asset.name] = newClip;
                vMapping[newClip.name] = newClip;
            }
            if (stateMapings!=null && ConstrollerRefState(importData, stateMapings, vMapping))
            {
                //! remove fbx clip animations
                List<ModelImporterClipAnimation> model_clips = new List<ModelImporterClipAnimation>();
                importData.importer.clipAnimations = model_clips.ToArray();
                importData.importer.SaveAndReimport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            else
            {
                Debug.LogError(importData.strPath + " 的控制器找不到");
            }

            if (bOver)
                ms_vClearedDirs.Clear();
        }
        //-----------------------------------------------------
        private void OnGUI()
        {
            if (GUILayout.Button("批量剥离"))
            {
                EditorUtility.DisplayProgressBar("剥离", "批量剥离", 0);
                ms_vClearedDirs.Clear();
                for (int i = 0; i < m_vDatas.Count; ++i)
                {
                    EditorUtility.DisplayProgressBar("剥离", m_vDatas[i].strPath, (float)i/ (float)m_vDatas.Count);
                    FilterClip(m_vDatas[i], i == m_vDatas.Count - 1);
                }
                ms_vClearedDirs.Clear();
                EditorUtility.ClearProgressBar();
            }
            m_SourceClipPos = GUILayout.BeginScrollView(m_SourceClipPos);
            for(int i = 0; i < m_vDatas.Count; ++i)
            {
                if (m_vDatas[i].importer == null) continue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(m_vDatas[i].srcObject.name, m_vDatas[i].importer, typeof(ModelImporter));
                if (GUILayout.Button("剥离"))
                {
                    ms_vClearedDirs.Clear();
                    FilterClip(m_vDatas[i], true,false);
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                m_vDatas[i].bExpand = EditorGUILayout.Foldout(m_vDatas[i].bExpand, "动作列表");
                if(m_vDatas[i].bExpand)
                {
                    EditorGUI.indentLevel++;
                    for (int a = 0; a < m_vDatas[i].clips.Count; ++a)
                    {
                        m_vDatas[i].clips[a].name = EditorGUILayout.TextField("名称", m_vDatas[i].clips[a].name);
                        EditorGUI.indentLevel++;
                        m_vDatas[i].clips[a].loop = EditorGUILayout.Toggle("循环", m_vDatas[i].clips[a].loop);
                        m_vDatas[i].clips[a].firstFrame = EditorGUILayout.IntField("开始帧", (int)m_vDatas[i].clips[a].firstFrame);
                        m_vDatas[i].clips[a].lastFrame = EditorGUILayout.IntField("结束帧", (int)m_vDatas[i].clips[a].lastFrame);
                        EditorGUI.indentLevel--;
                    }
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

        struct StateClip
        {
            public string clipName;
            public string[] btMotionNames;
            public ChildMotion[] blendMotions;
        }

        static Dictionary<AnimatorState, StateClip> CollectStateClips(FbxAnimationCliper.ImportData data)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(data.strPath).Trim();
            string dir = System.IO.Path.GetDirectoryName(data.strPath).Replace("\\", "/");
            string path = dir + "/" + name + ".controller";

            AnimatorController controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
            if (controller == null && name.ToLower().EndsWith("_l"))
            {
                path = dir + "/" + name.Substring(0, name.Length - 2) + ".controller";
                controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
            }
            if (controller != null)
            {
                Dictionary<AnimatorState, StateClip> vMping = new Dictionary<AnimatorState, StateClip>();
                for (int i = 0; i < controller.layers.Length; ++i)
                {
                    if (controller.layers[i].stateMachine == null) continue;
                    List<AnimatorState> states = Framework.ED.EditorUtil.GetStatesRecursive(controller.layers[i].stateMachine);
                    foreach (var db in states)
                    {
                        if (db.motion is BlendTree)
                        {
                            BlendTree bt = db.motion as BlendTree;
                            StateClip btClips = new StateClip();
                            btClips.blendMotions = bt.children;
                            if (btClips.blendMotions.Length > 0)
                            {
                                btClips.btMotionNames = new string[btClips.blendMotions.Length];
                                for (int t = 0; t < bt.children.Length; ++t)
                                {
                                    if (bt.children[i].motion != null)
                                        btClips.btMotionNames[t] = bt.children[t].motion.name;
                                }
                            }
                            vMping.Add(db, btClips);
                        }
                        else
                        {
                            StateClip btClips = new StateClip();
                            if (db.motion != null)
                                btClips.clipName = db.motion.name;
                            else
                                btClips.clipName = db.name;
                            vMping.Add(db, btClips);
                        }

                    }
                }
                return vMping;
            }
            return null;
        }
        static bool ConstrollerRefState(FbxAnimationCliper.ImportData data, Dictionary<AnimatorState, StateClip> vMping, Dictionary<string, AnimationClip> clipMapping)
        {
            string root = Application.dataPath.Replace("/Assets", "");
          //  if (!File.Exists(root+path))
            {
              //  if (File.Exists(path))
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(data.strPath).Trim();
                    string dir = System.IO.Path.GetDirectoryName(data.strPath).Replace("\\", "/");
                    string path = dir + "/" + name + ".controller";

                    AnimatorController controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
                    if (controller == null && name.ToLower().EndsWith("_l"))
                    {
                        path = dir + "/" + name.Substring(0, name.Length - 2) + ".controller";
                        controller = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;
                    }
                    if (controller != null)
                    {
                        foreach (var sub in vMping)
                        {
                            AnimatorState state = sub.Key;
                            if (string.IsNullOrEmpty(sub.Value.clipName) && sub.Value.btMotionNames == null) continue;
                            if (state.motion is BlendTree)
                            {
                                BlendTree blend = state.motion as BlendTree;
                                if (sub.Value.btMotionNames != null && sub.Value.btMotionNames.Length == blend.children.Length)
                                {
                                    for (int t = 0; t < sub.Value.blendMotions.Length; ++t)
                                    {
                                        ChildMotion bt = sub.Value.blendMotions[t];
                                        if (string.IsNullOrEmpty(sub.Value.btMotionNames[t])) continue;
                                        AnimationClip clip;
                                        if(clipMapping.TryGetValue(sub.Value.btMotionNames[t], out clip))
                                            bt.motion = clip;
                                        sub.Value.blendMotions[t] = bt;
                                    }
                                    blend.children = sub.Value.blendMotions;
                                }
                            }
                            else if (!string.IsNullOrEmpty(sub.Value.clipName))
                            {
                                AnimationClip clip;
                                if (clipMapping.TryGetValue(sub.Value.clipName, out clip))
                                    state.motion = clip;
                            }
                        }

                        EditorUtility.SetDirty(controller);
                        AssetDatabase.SaveAssets();

                        return true;
                    }

                }
            }
            return false;
        }
    }
}