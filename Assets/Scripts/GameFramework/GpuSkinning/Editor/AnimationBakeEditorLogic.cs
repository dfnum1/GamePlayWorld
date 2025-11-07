#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Reflection;
using Framework.ED;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif
namespace Framework.Plugin
{
    [System.Serializable]
    public class AnimSkinConfig
    {
        [System.Serializable]
        public class Item
        {
            public string displayName = "";
            public string bakeDir = "";
        }
        public Item defaultItem = new Item();
        public List<Item> vItems = new List<Item>();
    }
    public class AnimationBakeEditorLogic
    {
        private List<string> m_vConfigPop = new List<string>();
        private AnimSkinConfig m_Configs = new AnimSkinConfig();
        private SkinCombineData m_pSelectCombineData = null;
        private GameObject m_pSkinObject = null;

        private Vector3 m_BakeRolePosition = Vector3.zero;

        private bool m_bInspectorOpen = false;

        AnimSkinConfig.Item m_pCurSelect = null;

        CGpuSkinMeshAgent m_pAgent = new CGpuSkinMeshAgent(null);

        private ESkinType m_SkinType = ESkinType.GpuArray;
        private int m_BakeFrame = 30;
        private float m_BakeAnimSpeed = 1;
        private float m_fPlaySpeed = 1f;
        private Vector2 m_layerPanelScrollPos;
        private bool[] m_bExpandTexs = null;
        private int m_nCurPlayId = 0;
        private int m_nAniMapShaderKey = 0;
        private int m_nAnimLenShaderKey = 0;
        private int m_nAnimTotalLenShaderKey = 0;
        private int m_nAnimOffsetShaderKey = 0;
        private int m_nTimeEditorKey = 0;
        private Vector4 m_fTimeEditor = Vector4.zero;
        private bool m_bExpandSlots = false;
        private SkeletonSlot m_NewSlot = new SkeletonSlot();
        private List<SkeletonSlot> m_vSlots = new List<SkeletonSlot>();

        Vector2 m_CombineScrollPos = Vector2.zero;
        private bool m_bCombineSetting = false;
        private string m_strAddFile = "";
        private string m_strAddItemDir = "";
        private List<SkinCombineData> m_CombineObjects = new List<SkinCombineData>();

        TargetPreview m_preview;
        GUIStyle m_previewStyle;



        private int BakeWidht = 1024;
        private int BakeHeight = 1024;

        System.Type m_GPUDataType = null;
        System.Type m_CPUDataType = null;
        //-----------------------------------------------------
        void LoadConfig()
        {
            m_vConfigPop.Clear();
            string strTempFile = Application.dataPath + "/../EditorData/AnimSkinBakerConfig.json";
            if (!Directory.Exists(Application.dataPath + "/../EditorData/"))
                Directory.CreateDirectory(Application.dataPath + "/../EditorData");
            if (File.Exists(strTempFile))
            {
                try
                {
                    m_vConfigPop.Add("None");
                    m_Configs = JsonUtility.FromJson<AnimSkinConfig>(File.ReadAllText(strTempFile));
                    for (int i = 0; i < m_Configs.vItems.Count; ++i)
                    {
                        m_vConfigPop.Add(m_Configs.vItems[i].displayName);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
        //-----------------------------------------------------
        void SaveConfig()
        {
            string strTempFile = Application.dataPath + "/../EditorData/AnimSkinBakerConfig.json";
            if (!Directory.Exists(Application.dataPath + "/../EditorData/"))
                Directory.CreateDirectory(Application.dataPath + "/../EditorData");
            if (File.Exists(strTempFile))
                File.Delete(strTempFile);

            FileStream fs = new FileStream(strTempFile, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(JsonUtility.ToJson(m_Configs, true));
            writer.Close();
        }
        //-----------------------------------------------------
        void RefreshPop()
        {
            m_vConfigPop.Clear();
            m_vConfigPop.Add("None");
            for (int i = 0; i < m_Configs.vItems.Count; ++i)
            {
                m_vConfigPop.Add(m_Configs.vItems[i].displayName + "[" + i + "]");
            }
        }
        //-----------------------------------------------------
        public void OnDraw(int controllerId, Camera camera, Event evt)
        {
            var skinData = m_pAgent.frameData;
            if (skinData != null)
            {
                Quaternion qt = Quaternion.identity;
                for (int i = 0; i < m_vSlots.Count; ++i)
                {
                    qt.eulerAngles = m_vSlots[i]._rot;
                    Vector3 pos = m_vSlots[i]._pos;
                    Vector3 scale = Vector3.one;
                    float handleSize = 0.1f;
                    if (m_vSlots[i].slotFrames!=null)
                    {
                        m_pAgent.GetSlot(m_vSlots[i]._name, out pos, out qt, out scale);
                        handleSize *= scale.magnitude;

                    }
                    Handles.CubeHandleCap(controllerId, pos + m_pSkinObject.transform.position, qt, handleSize, EventType.Repaint);
                }

                if (m_NewSlot.expand)
                {
                    qt.eulerAngles = m_NewSlot._rot;
                    Color bk = Handles.color;
                    Handles.color = Color.red;
                    Handles.CubeHandleCap(controllerId, m_NewSlot._pos + m_pSkinObject.transform.position, qt, 0.1f, EventType.Repaint);
                    Handles.color = bk;
                }
                if (m_SkinType == ESkinType.GpuArray)
                    Graphics.DrawMesh(skinData.getShareMesh(), Matrix4x4.identity, skinData.getShareMat(), 0, camera,0, m_pAgent.GetPropertyBlock());
            }
        }
        //-----------------------------------------------------
        public void Update(float fFrameTime)
        {
            if (m_pAgent != null)
                m_pAgent.ForceUpdate(fFrameTime);
            if(m_pSelectCombineData!=null && m_pSelectCombineData.target!=null)
            {
                m_pSelectCombineData.target.transform.position = m_BakeRolePosition;

                if(m_pAgent!=null)
                {
                    string actioName = m_pAgent.GetCurPlayName();
                    float time = m_pAgent.GetTime();
                    if (m_pSelectCombineData.animatorStates ==null && m_pSelectCombineData.controller != null)
                    {
                        m_pSelectCombineData.animatorStates = BakeSkinUtil.GetStatesRecursive(m_pSelectCombineData.controller);
                    }
                    if(!string.IsNullOrEmpty(actioName) && m_pSelectCombineData.animatorStates!=null && m_pSelectCombineData.animatorStates.TryGetValue(actioName, out var animState))
                    {
                        if(m_pSelectCombineData.playAnimator == null)
                        {
                            m_pSelectCombineData.playAnimator = m_pSelectCombineData.target.GetComponent<Animator>();
                        }
                        if (m_pSelectCombineData.playAnimator.runtimeAnimatorController == null)
                            m_pSelectCombineData.playAnimator.runtimeAnimatorController = null;

                        if(animState.motion!=null)
                        {
                            var clip = animState.motion as AnimationClip;
                            if (clip)
                            {
                                if (animState.motion.isLooping && time >= animState.motion.averageDuration && animState.motion.averageDuration>0.0f)
                                    time = time % animState.motion.averageDuration;
                                clip.SampleAnimation(m_pSelectCombineData.target, time);
                            }
                        }
                        //if (m_pSelectCombineData.playAnimator)
                        //{
                        //    if (m_pSelectCombineData.playAnimator.HasState(0, animState.nameHash))
                        //    {
                        //        m_pSelectCombineData.playAnimator.PlayInFixedTime(animState.name, 0, time);
                        //    }
                        //    m_pSelectCombineData.playAnimator.Update(0);
                        //}
                    }
                }
            }
        }
        //-----------------------------------------------------
        string popSelPanel(string path)
        {
            if (path.StartsWith("Assets/")) path = path.Substring("Assets".Length);
            int nAssetLength = ("Assets/").Length;
            string strPath = Application.dataPath + "/DatasRef/";
            if (path.Length > 0)
                strPath = Application.dataPath + path.Substring(0, path.LastIndexOf('/') + 1);
            string selFile = EditorUtility.OpenFilePanel("Select File", strPath, "prefab");
            if (selFile.Length > 0)
                selFile = selFile.Substring(Application.dataPath.Length - nAssetLength, selFile.Length - Application.dataPath.Length + nAssetLength);
            if (path != selFile && selFile.Length > 0)
            {
                return selFile;
            }
            return "";
        }
        //-----------------------------------------------------
        public void OnDrawLayerPanel()
        {
            if (m_bCombineSetting)
            {
                bool bHas = m_strAddFile.Length <= 0;
                m_CombineScrollPos = EditorGUILayout.BeginScrollView(m_CombineScrollPos);
                for (int i = 0; i < m_CombineObjects.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(m_CombineObjects[i].filename);
                    if (GUILayout.Button("...", new GUILayoutOption[] { GUILayout.Width(50) }))
                    {
                        string sel = popSelPanel(m_CombineObjects[i].filename);
                        if (sel.Length > 0)
                        {
                            m_CombineObjects[i].filename = sel;
                            BakeSkinUtil.DestroyObject(m_CombineObjects[i].target);
                            if (m_CombineObjects[i].target != null)
                            {
                                m_CombineObjects[i].target = GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(sel));
                                m_CombineObjects[i].target.transform.position = m_BakeRolePosition;
                                m_preview.AddPreview(m_CombineObjects[i].target);
                            }
                        }
                    }
                    if (m_CombineObjects[i].target != null && GUILayout.Button(m_CombineObjects[i].show ? "S" : "H", new GUILayoutOption[] { GUILayout.Width(20) }))
                    {
                        m_CombineObjects[i].target.SetActive(m_CombineObjects[i].show);
                    }
                    GUILayout.EndHorizontal();
                    if (!bHas && m_strAddFile == m_CombineObjects[i].filename)
                        bHas = true;

                    EditorGUI.indentLevel++;
                    m_CombineObjects[i].controller = EditorGUILayout.ObjectField("Controllor", m_CombineObjects[i].controller, typeof(AnimatorController), false) as AnimatorController;
                    for (int k = 0; k < m_CombineObjects[i].materials.Count; ++k)
                    {
                        EditorGUILayout.ObjectField(k.ToString(), m_CombineObjects[i].materials[k], typeof(Material), false);
                    }
                    m_CombineObjects[i].bExpandAnimacitons = EditorGUILayout.Foldout(m_CombineObjects[i].bExpandAnimacitons, "动作文件");
                    if (m_CombineObjects[i].bExpandAnimacitons)
                    {
                        EditorGUI.indentLevel++;
                        for (int k = 0; k < m_CombineObjects[i].animations.Count; ++k)
                        {
                            EditorGUILayout.ObjectField(k.ToString(), m_CombineObjects[i].animations[k], typeof(AnimationClip), false);
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndScrollView();
                GUILayout.BeginHorizontal();
                GUILayout.TextField(m_strAddFile, new GUILayoutOption[] { GUILayout.Width(AnimationBakeSkinEditor.LeftWidth - 100) });
                if (GUILayout.Button("...", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    m_strAddFile = popSelPanel(m_strAddFile);
                }

                if (!bHas && GUILayout.Button("Add", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    if (AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(m_strAddFile) == null)
                    {
                        EditorUtility.DisplayDialog("Tips", "load failed", "ok");
                    }
                    else
                    {
                        SkinCombineData data = new SkinCombineData();
                        data.filename = m_strAddFile;
                        m_CombineObjects.Add(data);
                    }

                    m_strAddFile = "";

                }
                GUILayout.EndHorizontal();
                return;
            }

            int gp = (m_pCurSelect == null) ? -1 : (m_vConfigPop.IndexOf(m_pCurSelect.displayName));
            int preGp = gp;
            gp = EditorGUILayout.Popup("角色组", gp, m_vConfigPop.ToArray());
            if (preGp != gp)
            {
                if (gp > 0 && gp <= m_Configs.vItems.Count && m_Configs.vItems[gp - 1] != m_pCurSelect)
                {
                    Clear();
                    m_pCurSelect = m_Configs.vItems[gp - 1];
                    if (m_pCurSelect != null)
                    {
                        CollectPrefabCombineData(m_pCurSelect.bakeDir);
                    }
                }
                else
                {
                    m_pCurSelect = null;
                }
            }


            GUILayout.BeginHorizontal();
            m_strAddItemDir = EditorGUILayout.TextField("添加角色组", m_strAddItemDir);
            if (GUILayout.Button("...", new GUILayoutOption[] { GUILayout.Width(30) }))
            {
                int nAssetLength = ("Asset/").Length;
                string strPath = Application.dataPath + m_strAddItemDir.Substring(0, m_strAddItemDir.LastIndexOf('/') + 1);
                string selFile = EditorUtility.OpenFolderPanel("Select File", strPath, "prefab");
                if (selFile.Length > 0)
                    m_strAddItemDir = selFile.Substring(Application.dataPath.Length - nAssetLength, selFile.Length - Application.dataPath.Length + nAssetLength);
                m_strAddItemDir = m_strAddItemDir.Replace("\\", "/");
            }
            if (!string.IsNullOrEmpty(m_strAddItemDir) && GUILayout.Button("添加", new GUILayoutOption[] { GUILayout.Width(40) }))
            {
                AnimSkinConfig.Item item = new AnimSkinConfig.Item();
                item.displayName = System.IO.Path.GetFileNameWithoutExtension(m_strAddItemDir);
                item.bakeDir = m_strAddItemDir;

                bool bExists = false;
                for (int i = 0; i < m_Configs.vItems.Count; ++i)
                {
                    if (m_Configs.vItems[i].bakeDir.CompareTo(item.bakeDir) == 0)
                    {
                        bExists = true;
                        break;
                    }
                }
                if (!bExists)
                {
                    m_Configs.vItems.Add(item);
                    RefreshPop();
                }
                m_strAddItemDir = "";
            }
            GUILayout.EndHorizontal();
            if (m_pCurSelect == null)
            {
                m_pCurSelect = m_Configs.defaultItem;
            }

            GUILayout.BeginHorizontal();
            List<string> vPop = new List<string>();
            for (int i = 0; i < m_CombineObjects.Count; ++i)
            {
                vPop.Add(Path.GetFileName(m_CombineObjects[i].filename));
            }
            int index = m_CombineObjects.IndexOf(m_pSelectCombineData);
            index = EditorGUILayout.Popup(index, vPop.ToArray());
            if (index >= 0 && index < vPop.Count)
            {
                SelectCreateTarget(m_CombineObjects[index]);
            }

            GUILayout.EndHorizontal();

            if (m_pCurSelect != null)
            {
                int frame = EditorGUILayout.IntField("烘焙帧率", m_BakeFrame);
                if (frame != m_BakeFrame)
                {
                    m_BakeFrame = frame;
                    m_BakeAnimSpeed = (float)frame / 30f;
                }
                m_BakeAnimSpeed = EditorGUILayout.FloatField("烘焙速度", m_BakeAnimSpeed);
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(AnimationBakeSkinEditor.LeftWidth-15) });
                ESkinType preType = m_SkinType;
                m_SkinType = (ESkinType)EditorGUILayout.EnumPopup("SkinType", m_SkinType, new GUILayoutOption[] { GUILayout.Width(AnimationBakeSkinEditor.LeftWidth-60) });
                if (m_bInspectorOpen)
                {
                    if (preType != m_SkinType)
                    {
                        UpdateSkinTarget();
                    }
                }
                else
                {
                    if (preType != m_SkinType)
                    {
                        UpdateSkinTarget();
                    }
                    if (GUILayout.Button("烘焙", new GUILayoutOption[] { GUILayout.Width(50) }))
                    {
                        if (m_pSelectCombineData != null)
                            Bake(m_pSelectCombineData, m_SkinType, m_BakeFrame, m_BakeAnimSpeed);
                    }
                   // if (GUILayout.Button("批量烘焙", new GUILayoutOption[] { GUILayout.Width(80f) }))
                   // {
                   //     BakeBatch(m_SkinType, m_BakeFrame);
                   // }
                }

                GUILayout.EndHorizontal();
            }

            if (!m_bInspectorOpen)
            {
                if (m_pSelectCombineData != null)
                {
                   // if (GUILayout.Button("烘焙选定帧"))
                   // {
                   //     AnimationBakeFrameEditor.Show(m_pSelectCombineData.combineData.fbxAsset, m_pSelectCombineData.combineData.controller as AnimatorController, m_pSelectCombineData.combineData);
                   // }
                    if (m_pAgent.frameData!=null && GUILayout.Button("保存"))
                    {
                        var assetData= m_pSelectCombineData.combineData.GetBakeAsset(m_SkinType);
                        if(assetData!=null)
                        {
                            EditorUtility.SetDirty(assetData);
                            AssetDatabase.SaveAssetIfDirty(assetData);
                            m_pAgent.UpdateSkinFrameData();
                        }
                    }
                }
            }

            //base slots
            OnDrawSlot();
            var skinData = m_pAgent.frameData;
            if (skinData != null)
            {
                this.m_layerPanelScrollPos = GUILayout.BeginScrollView(this.m_layerPanelScrollPos);

                m_fPlaySpeed = Mathf.Clamp(EditorGUILayout.FloatField("播放速度", m_fPlaySpeed), 0, 10);
                m_pAgent.SetSpeed(m_fPlaySpeed);

                if (skinData is SkinFrameMeshData)
                {
                    SkinFrameMeshData meshSkin = skinData as SkinFrameMeshData;
                    if (meshSkin.animations == null) return;
                    if (meshSkin.animations._offsets != null && meshSkin.animations._offsets.Length > 0)
                    {
                        int[] pop = new int[meshSkin.animations._offsets.Length];
                        string[] strPop = new string[pop.Length];
                        for (int i = 0; i < pop.Length; ++i)
                        {
                            pop[i] = i;
                            strPop[i] = i.ToString();
                        }
                        byte preSel = (byte)EditorGUILayout.IntPopup(m_pAgent.GetSkin(), strPop, pop);
                        if (m_pAgent.GetSkin() != preSel)
                        {
                            m_pAgent.SetSkin(preSel);
                            meshSkin.dirty();
                        }
                    }


                    if (m_bExpandTexs == null)
                        m_bExpandTexs = new bool[meshSkin.animations.animMeshs.Length];
                    for (int i = 0; i < m_bExpandTexs.Length; ++i)
                    {
                        if (m_nCurPlayId == i)
                            GUI.color = Color.red;

                        GUILayout.BeginHorizontal();
                        m_bExpandTexs[i] = EditorGUILayout.Foldout(m_bExpandTexs[i], meshSkin.animations.animMeshs[i]._name);
                        if (GUILayout.Button("Sel", new GUILayoutOption[] { GUILayout.MaxWidth(40) }))
                        {
                            m_nCurPlayId = i;
                            m_pAgent.SetFixedTime(0);
                            m_pAgent.Play(meshSkin.animations.animMeshs[i]._name);
                        }
                        GUILayout.EndHorizontal();

                        GUI.color = Color.white;

                        if (m_bExpandTexs[i])
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            GUILayout.BeginVertical();

                            meshSkin.animations.animMeshs[i].actionTag = ActorSystem.ED.EditorUtil.DrawActionAndTag(meshSkin.animations.animMeshs[i].actionTag, "Action", true, null);
                            meshSkin.animations.animMeshs[i]._name = EditorGUILayout.TextField("Name", meshSkin.animations.animMeshs[i]._name);
                            meshSkin.animations.animMeshs[i]._speed = EditorGUILayout.FloatField("Speed", meshSkin.animations.animMeshs[i]._speed);
                            meshSkin.animations.animMeshs[i]._loop = EditorGUILayout.Toggle("Loop", meshSkin.animations.animMeshs[i]._loop);
                            EditorGUILayout.LabelField("FrameCount", meshSkin.animations.animMeshs[i]._keyFrameCount.ToString());

                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
                    }

                }
                else if (skinData is SkinFrameBoneData)
                {
                    SkinFrameBoneData bonSkins = skinData as SkinFrameBoneData;
                    if (m_bExpandTexs == null)
                        m_bExpandTexs = new bool[bonSkins.boneAnimations.animations.Length];

                    if (bonSkins.boneAnimations != null)
                    {
                        for (int i = 0; i < m_bExpandTexs.Length; ++i)
                        {
                            if (m_nCurPlayId == i)
                                GUI.color = Color.red;

                            GUILayout.BeginHorizontal();
                            m_bExpandTexs[i] = EditorGUILayout.Foldout(m_bExpandTexs[i], bonSkins.boneAnimations.animations[i]._animName);
                            if (GUILayout.Button("Sel", new GUILayoutOption[] { GUILayout.MaxWidth(40) }))
                            {
                                if(!bonSkins.boneAnimations.animations[i]._loop)
                                    m_pAgent.SetFixedTime(0);
                                m_pAgent.Play(bonSkins.boneAnimations.animations[i]._animName);
                                m_nCurPlayId = i;
                            }
                            GUILayout.EndHorizontal();

                            GUI.color = Color.white;

                            if (m_bExpandTexs[i])
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(10);
                                GUILayout.BeginVertical();

                                bonSkins.boneAnimations.animations[i]._animName = EditorGUILayout.TextField("Name", bonSkins.boneAnimations.animations[i]._animName);
                                bonSkins.boneAnimations.animations[i]._actionTag = ActorSystem.ED.EditorUtil.DrawActionAndTag(bonSkins.boneAnimations.animations[i]._actionTag, "Action", true, null);
                                EditorGUILayout.LabelField("Length", bonSkins.boneAnimations.animations[i]._length.ToString());
                                bonSkins.boneAnimations.animations[i]._loop = EditorGUILayout.Toggle("Loop", bonSkins.boneAnimations.animations[i]._loop);
                                bonSkins.boneAnimations.animations[i]._speed = EditorGUILayout.FloatField("Speed", bonSkins.boneAnimations.animations[i]._speed);
                                if (bonSkins.boneAnimations.animations[i]._frames != null)
                                    EditorGUILayout.LabelField("FrameCnt", bonSkins.boneAnimations.animations[i]._frames.Length.ToString());
                                EditorGUILayout.LabelField("Fps", bonSkins.boneAnimations.animations[i]._fps.ToString());

                                GUILayout.EndVertical();
                                GUILayout.EndHorizontal();
                            }
                        }
                    }

                }
                GUILayout.EndScrollView();
            }
        }
        //-----------------------------------------------------
        private void OnDrawSlot()
        {
            if (m_pAgent == null) return;
            var skinData = m_pAgent.frameData;

            m_bExpandSlots = EditorGUILayout.Foldout(m_bExpandSlots, "Slots");
            if (m_bExpandSlots)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                bool bHaded = false;
                for (int i = 0; i < m_vSlots.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    m_vSlots[i].expand = EditorGUILayout.Foldout(m_vSlots[i].expand, m_vSlots[i]._name);
                    if (GUILayout.Button("del", new GUILayoutOption[] { GUILayout.MaxWidth(40) }))
                    {
                        m_vSlots.RemoveAt(i);
                        if(skinData!=null) skinData.setSlots(m_vSlots.ToArray());
                        return;
                    }
                    GUILayout.EndHorizontal();
                    if (m_vSlots[i].expand)
                    {
                        m_vSlots[i]._pos = EditorGUILayout.Vector3Field("pos", m_vSlots[i]._pos);
                        m_vSlots[i]._rot = EditorGUILayout.Vector3Field("rot", m_vSlots[i]._rot);
                    }

                    if (m_NewSlot._name == m_vSlots[i]._name) bHaded = true;
                }
                GUILayout.BeginHorizontal();
                m_NewSlot._name = EditorGUILayout.TextField("name", m_NewSlot._name);
                EditorGUI.BeginDisabledGroup(bHaded);
                if (GUILayout.Button("Add"))
                {
                    m_vSlots.Add(new SkeletonSlot() { _name = m_NewSlot._name, _pos = m_NewSlot._pos, _rot = m_NewSlot._rot });
                    if (skinData != null) skinData.setSlots(m_vSlots.ToArray());
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
                //m_NewSlot.expand = EditorGUILayout.Foldout(m_NewSlot.expand, "NewSolt");
                //if (m_NewSlot.expand)
                //{
                //    GUILayout.Box("New", new GUILayoutOption[] { GUILayout.MaxHeight(2) });
                //    GUILayout.BeginHorizontal();
                //    GUILayout.Space(10);
                //    GUILayout.BeginVertical();

                //    m_NewSlot._name = EditorGUILayout.TextField("name", m_NewSlot._name);
                //    m_NewSlot._pos = EditorGUILayout.Vector3Field("pos", m_NewSlot._pos);
                //    m_NewSlot._rot = EditorGUILayout.Vector3Field("rot", m_NewSlot._rot);
                //    EditorGUI.BeginDisabledGroup(bHaded);
                //    if (GUILayout.Button("Add"))
                //    {
                //        m_vSlots.Add(new SkeletonSlot() { _name = m_NewSlot._name, _pos = m_NewSlot._pos, _rot = m_NewSlot._rot });
                //        m_NewSlot.expand = false;
                //        if (skinData != null) skinData.setSlots(m_vSlots.ToArray());
                //    }
                //    EditorGUI.EndDisabledGroup();
                //    GUILayout.EndVertical();
                //    GUILayout.EndHorizontal();
                //}


                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

        }
        //-----------------------------------------------------
        public void OnDrawInspectorPanel()
        {
            if (m_bCombineSetting)
            {
                BakeWidht = EditorGUILayout.IntField("烘焙贴图Width", BakeWidht);
                BakeHeight = EditorGUILayout.IntField("烘焙贴图Height", BakeHeight);
                if (m_pCurSelect != null)
                {
                    m_pCurSelect.bakeDir = EditorGUILayout.TextField("烘焙根目录", m_pCurSelect.bakeDir);

                    if (GUILayout.Button("刷新"))
                    {
                        CollectPrefabCombineData(m_pCurSelect.bakeDir);
                    }
                    if (m_CombineObjects.Count > 0 && GUILayout.Button("Bake"))
                    {
                        for (int i = 0; i < m_CombineObjects.Count; ++i)
                            SkinMeshPacker.CombineSingle(m_CombineObjects[i], BakeWidht, BakeHeight);
                    }
                }
                return;
            }

            Texture gpuSkinTexture = null;
            Texture cpuSkinTexture = null;
            if (m_pSelectCombineData != null && m_pSelectCombineData.combineData != null)
            {
                cpuSkinTexture = m_pSelectCombineData.combineData.useTexture;
                gpuSkinTexture = m_pSelectCombineData.combineData.gpuUseTexture;
            }

            float lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.ObjectField("GpuSkinTexture", gpuSkinTexture, typeof(Texture), false);
            EditorGUILayout.ObjectField("CpuSkinTexture", cpuSkinTexture, typeof(Texture), false);
            EditorGUILayout.EndHorizontal();

            if (m_pAgent != null)
            {
                EditorGUILayout.LabelField("GpuAsset", m_pAgent.gpuAssetPath);
                EditorGUILayout.LabelField("CpuAsset", m_pAgent.cpuAssetPath);
            }
            EditorGUIUtility.labelWidth = lastLabelWidth;

            Rect windowRect = GUILayoutUtility.GetLastRect();
            if (m_preview != null)
            {
                var window = AnimationBakeSkinEditor.Instance;
                if (window) m_preview.DrawPreview(new Rect(0, windowRect.yMax, window.position.width - AnimationBakeSkinEditor.LeftWidth, window.position.height - windowRect.yMax - AnimationBakeSkinEditor.GapTop - AnimationBakeSkinEditor.GapBottom));
            }
        }
        //-----------------------------------------------------
        public void SetTarget(UnityEngine.Object asset, bool bInspectorOpen = true)
        {
            string file = AssetDatabase.GetAssetPath(asset);
            if (m_pSelectCombineData != null && m_pSelectCombineData.filename == file)
                return;

            var combineData = CreateCombineData(file);
            if (combineData == null) return;

            SelectCreateTarget(combineData);
            m_bInspectorOpen = bInspectorOpen;
        }
        //-----------------------------------------------------
        private void SelectCreateTarget(SkinCombineData combineData)
        {
            if (m_pSelectCombineData == combineData) return;
            if (m_pSelectCombineData != null)
            {
                if (m_pSelectCombineData.target)
                    BakeSkinUtil.DestroyObject(m_pSelectCombineData.target);
                m_pSelectCombineData.target = null;
            }
            m_pSelectCombineData = combineData;
            if (m_pSelectCombineData == null)
                return;
            string bakeFile = combineData.GetBakeFile();
            if (!File.Exists(bakeFile)) return;

            SkinMeshBakerData srcuvData = JsonUtility.FromJson<SkinMeshBakerData>(File.ReadAllText(bakeFile));
            if (srcuvData != null)
            {
                combineData.combineData = srcuvData;
                if (srcuvData.fbxAsset)
                {
                    combineData.target = GameObject.Instantiate(srcuvData.fbxAsset) as GameObject;
                    combineData.target.transform.position = m_BakeRolePosition;
                    m_preview.AddPreview(combineData.target);
                }
                UpdateSkinTarget();
            }
            m_bInspectorOpen = false;
        }
        //-----------------------------------------------------
        public void BakeBatch(ESkinType type, float frameRate)
        {
            //if (m_bInspectorOpen) return;
            //string strPath = Application.dataPath.Replace("/Assets", "/") + m_pCurSelect.saveTo;
            //{
            //    if (false == System.IO.Directory.Exists(strPath))
            //    {
            //        System.IO.Directory.CreateDirectory(strPath);
            //    }
            //}

            //for (int i = 0; i < m_CombineObjects.Count; ++i)
            //{
            //    SkinMeshBakerData uvData = m_CombineObjects[i].combineData;
            //    if (uvData == null) continue;

            //    if (m_pSkinObject == null)
            //        return;

            //    string filename = System.IO.Path.GetFileNameWithoutExtension(m_CombineObjects[i].combineBakeFile);

            //    GameObject role = new GameObject(filename);
            //    MeshFilter filer = role.AddComponent<MeshFilter>();
            //    MeshRenderer render = role.AddComponent<MeshRenderer>();
            //    ASkinner ske = role.AddComponent<ASkinner>();

            //    render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            //    render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            //    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //    render.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            //    render.receiveShadows = false;

            //    string texPath = m_pCurSelect.saveTo + filename;

            //    if (m_pSkinData != null) m_pSkinData.clear();
            //    m_pSkinData = BakeSkinUtil.BakeSkin(m_CombineObjects[i].target, type, uvData, ((type == ESkinType.CpuData) ? uvData.useTexture : uvData.gpuUseTexture), frameRate);
            //    if (m_pSkinData is SkinFrameMeshData) texPath += "_AniMesh.asset";
            //    else if (m_pSkinData is SkinFrameVertexData) texPath += "_AniMap.asset";
            //    else if (m_pSkinData is SkinFrameBoneData) texPath += "_textureSkin.asset";
            //    if (File.Exists(texPath))
            //        File.Delete(texPath);

            //    if (m_pSkinData is SkinFrameMeshData)
            //    {
            //        SkinFrameMeshData skinMesh = m_pSkinData as SkinFrameMeshData;
            //        AssetDatabase.CreateAsset(skinMesh.animations, texPath);
            //        UnityEditor.AssetDatabase.Refresh();
            //        ske.cpuAssetPath = texPath;
            //    }
            //    else if (m_pSkinData is SkinFrameVertexData)
            //    {
            //        SkinFrameVertexData skinTex = m_pSkinData as SkinFrameVertexData;
            //        AssetDatabase.CreateAsset(skinTex._animsTex._animMap, texPath);
            //        UnityEditor.AssetDatabase.Refresh();
            //    }
            //    else if (m_pSkinData is SkinFrameBoneData)
            //    {
            //        SkinFrameBoneData boneDatas = m_pSkinData as SkinFrameBoneData;

            //        UnityEditor.AssetDatabase.CreateAsset(boneDatas.boneAnimations, texPath);

            //        UnityEditor.AssetDatabase.Refresh();
            //        ske.gpuAssetPath = texPath;
            //    }
            //    ske.cpuTexture = uvData.useTexture;
            //    ske.gpuTexture = uvData.gpuUseTexture;
            //    string prefab = m_pCurSelect.saveTo + filename + ".prefab";
            //    if (File.Exists(prefab))
            //    {
            //        GameObject oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            //        if (oldPrefab != null)
            //        {
            //            ASkinner agent = oldPrefab.GetComponent<ASkinner>();
            //            if (agent != null)
            //            {
            //                if (type == ESkinType.CpuData)
            //                    ske.gpuAssetPath = agent.gpuAssetPath;
            //                else if (type == ESkinType.GpuArray)
            //                    ske.cpuAssetPath = agent.cpuAssetPath;
            //            }

            //            for (int k = 0; k < oldPrefab.transform.childCount; ++k)
            //            {
            //                GameObject pChild = GameObject.Instantiate(oldPrefab.transform.GetChild(k).gameObject);
            //                pChild.name = oldPrefab.transform.GetChild(k).gameObject.name;
            //                pChild.transform.SetParent(role.transform);
            //            }
            //        }
            //        File.Delete(prefab);
            //    }
            //    role.layer = LayerMask.NameToLayer(GlobalDef.ms_foregroundLayerName);
            //    bool success;
            //    uvData.bakePrefab = PrefabUtility.SaveAsPrefabAsset(role, prefab, out success);
            //    if(!success)
            //    {
            //        EditorUtility.DisplayDialog("提示", "预制体保存失败:" + prefab, "确认");
            //    }
            //    GameObject sourceCombinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(m_CombineObjects[i].filename);
            //    if (sourceCombinePrefab != null)
            //    {
            //        SkinMeshBakerData meshUV = JsonUtility.FromJson<SkinMeshBakerData>(File.ReadAllText(m_CombineObjects[i].combinePrefab + ".json"));
            //        meshUV.bakePrefab = uvData.bakePrefab;
            //        EditorUtility.SetDirty(sourceCombinePrefab);
            //    }
            //    GameObject.DestroyImmediate(role);
            //}

            //if (m_pSkinData != null)
            //    m_pSkinData.clear();
            //m_pSkinData = null;
            //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        //-----------------------------------------------------
        public void Bake(SkinCombineData combineData, ESkinType type, float frameRate, float animSpeed = 1f)
        {
            if (combineData == null || m_bInspectorOpen) return;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(combineData.filename);
            if (prefab == null) return;
            if (combineData.target == null)
            {
                combineData.target = GameObject.Instantiate<GameObject>(prefab);
                combineData.target.transform.position = m_BakeRolePosition;
                m_preview.AddPreview(combineData.target);
            }

            ModelImporter model = AssetImporter.GetAtPath(combineData.filename) as ModelImporter;
            if (model != null)
            {
                model.optimizeGameObjects = false;
                model.isReadable = true;
                AssetDatabase.ImportAsset(combineData.filename);
            }

            string path = combineData.GetBakeFile();
            if (!File.Exists(path))
            {
                SkinMeshPacker.CombineSingle(combineData, BakeWidht, BakeHeight);
            }
            else
            {
                if(EditorUtility.DisplayDialog("提示", "已经烘焙过，需要重新烘焙？", "确定", "取消"))
                {
                    SkinMeshPacker.CombineSingle(combineData, BakeWidht, BakeHeight);
                }
            }
            if (!File.Exists(path))
                return;

            SkinMeshBakerData uvData = JsonUtility.FromJson<SkinMeshBakerData>(File.ReadAllText(path));
            Animator animator = combineData.target.GetComponent<Animator>();
            if (animator && animator.runtimeAnimatorController)
                uvData.controller = animator.runtimeAnimatorController;

            combineData.combineData = uvData;
            if (uvData.gpuUseTexture == null && uvData.useTexture)
                uvData.gpuUseTexture = uvData.useTexture;

            var skinData = BakeSkinUtil.BakeSkin(combineData, m_vSlots.ToArray(), type, ((type == ESkinType.CpuData) ? uvData.useTexture : uvData.gpuUseTexture), frameRate, animSpeed);
            m_pAgent.cpuTexture = uvData.useTexture;
            m_pAgent.gpuTexture = uvData.gpuUseTexture;

            SaveSkinData(combineData.GetBakeExport(), skinData);
            UpdateSkinTarget();
        }
        //-----------------------------------------------------
        private void UpdateSkinTarget()
        {
            m_BakeRolePosition = Vector3.zero;
            if (m_pSelectCombineData == null || m_pSelectCombineData.combineData == null)
                return;
            var scriptObj = m_pSelectCombineData.combineData.GetBakeAsset(m_SkinType);
            m_bExpandTexs = null;
            if (scriptObj == null)
            {
                return;
            }
            SkinFrameData skinData = null;
            if (scriptObj is ASkeletonGpuData)
            {
                SkinFrameBoneData boneData = new SkinFrameBoneData();
                boneData.boneAnimations = scriptObj as ASkeletonGpuData;
                boneData.getAniTex();
                boneData.assetFile = AssetDatabase.GetAssetPath(scriptObj);
                m_SkinType = ESkinType.GpuArray;
                skinData = boneData;
            }
            else if (scriptObj is ASkeletonCpuData)
            {
                SkinFrameMeshData boneData = new SkinFrameMeshData();
                boneData.animations = scriptObj as ASkeletonCpuData;
                skinData.assetFile = AssetDatabase.GetAssetPath(scriptObj);
                m_SkinType = ESkinType.CpuData;
                skinData = boneData;
            }
            //! get skindata
            if (skinData == null)
                return;

            m_pAgent.cpuTexture = m_pSelectCombineData.combineData.useTexture;
            m_pAgent.gpuTexture = m_pSelectCombineData.combineData.gpuUseTexture;

            m_BakeRolePosition = Vector3.right*3;
            skinData.grab();
            m_pAgent.SetSkinFrameData(skinData, m_SkinType);

            if (skinData is SkinFrameVertexData)
            {
                m_nCurPlayId = 0;
                SkinFrameVertexData texAllSkin = skinData as SkinFrameVertexData;
                if (skinData.getShareMat() != null)
                {
                    skinData.getShareMat().SetTexture(m_nAniMapShaderKey, texAllSkin._animsTex._animMap);
                }
            }
            SetBindSlots();
        }
        //-----------------------------------------------------
        public void SetBindSlots()
        {
            m_vSlots.Clear();
            var frameSkin = m_pAgent.frameData;
            if (frameSkin == null)
                return;
            SkeletonSlot[] slots = frameSkin.getSlots();
            if (slots == null || slots.Length <= 0)
                m_vSlots.Clear();
            else
                m_vSlots = new List<SkeletonSlot>(slots);
        }
        //-----------------------------------------------------
        public void OnEvent(Event evt)
        {
            if (!evt.isKey) return;

            if (evt.type == EventType.KeyDown)
            {
                if (evt.keyCode == KeyCode.F1)
                {
                    if (!m_bCombineSetting)
                    {
                        if (m_pSelectCombineData != null && m_pSelectCombineData.target)
                            m_pSelectCombineData.target.SetActive(!m_pSelectCombineData.target.activeSelf);
                    }
                }
                else if (evt.keyCode == KeyCode.F2)
                {
                    m_bCombineSetting = !m_bCombineSetting;
                    for (int i = 0; i < m_CombineObjects.Count; ++i)
                    {
                        if (m_CombineObjects[i].target) m_CombineObjects[i].target.SetActive(m_bCombineSetting);
                    }
                }
            }
        }
        //-----------------------------------------------------
        public void OnMouseUp(int button)
        {

        }
        //-----------------------------------------------------
        public void OnMouseMove(Vector3 vWorldPos, Ray ray)
        {

        }
        //-----------------------------------------------------
        public void OnMouseHit(Vector3 vWorldPos, ref Ray ray)
        {

        }
        //-----------------------------------------------------
        void SaveSkinData(string path, SkinFrameData skinFrameData)
        {
            string name = Path.GetFileNameWithoutExtension(m_pSelectCombineData.filename);
            if (skinFrameData is SkinFrameMeshData) path += name+ "_CpuData.asset";
            else if (skinFrameData is SkinFrameVertexData) path += name + "_GpuData.asset";
            else if (skinFrameData is SkinFrameBoneData) path += name + "_GpuArray.asset";
            if (File.Exists(path))
                File.Delete(path);

            string dirPath = System.IO.Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if (skinFrameData is SkinFrameMeshData)
            {
                SkinFrameMeshData skinMesh = skinFrameData as SkinFrameMeshData;
                AssetDatabase.CreateAsset(skinMesh.animations, path);
                UnityEditor.AssetDatabase.Refresh();
                m_pAgent.cpuAssetPath = path;
                m_pSelectCombineData.combineData.SetBakeAsset(ESkinType.CpuData, skinMesh.animations);
                m_pSelectCombineData.combineData.Save();
            }
            else if (skinFrameData is SkinFrameVertexData)
            {
                SkinFrameVertexData skinTex = skinFrameData as SkinFrameVertexData;
                AssetDatabase.CreateAsset(skinTex._animsTex._animMap, path);
                UnityEditor.AssetDatabase.Refresh();
                //     m_pSelectCombineData.SetBakeAsset(type, skinTex._animsTex._animMap);
            }
            else if (skinFrameData is SkinFrameBoneData)
            {
                SkinFrameBoneData boneDatas = skinFrameData as SkinFrameBoneData;

                UnityEditor.AssetDatabase.CreateAsset(boneDatas.boneAnimations, path);

                UnityEditor.AssetDatabase.Refresh();
                m_pAgent.gpuAssetPath = path;
                m_pSelectCombineData.combineData.SetBakeAsset(ESkinType.GpuArray, boneDatas.boneAnimations);
                m_pSelectCombineData.combineData.Save();
            }

            //create prefab
        }
        //-----------------------------------------------------
        public void SaveData()
        {
            if (m_pAgent == null)
            {
                LoadConfig();
                return;
            }
            UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_pAgent.gpuAssetPath);
            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
            }
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_pAgent.cpuAssetPath);
            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            LoadConfig();
        }
        //-----------------------------------------------------
        static void CheckFiles(string path, List<string> FileList, string extend, bool bInclude = true)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                if (f.Extension.ToLower().CompareTo(extend) != 0)
                    continue;
                long size = f.Length;
                FileList.Add(f.FullName.Replace("\\", "/"));
            }
            if (!bInclude) return;

            foreach (DirectoryInfo d in dii)
            {
                CheckFiles(d.FullName, FileList, extend, bInclude);
            }
        }
        //-----------------------------------------------------
        class sUintData
        {
            public string file;
            public string animator;
            public List<string> mateirals = new List<string>();
            public List<string> animations = new List<string>();
        }
        //-----------------------------------------------------
        public void OnEnable()
        {
            m_GPUDataType = null;
            m_CPUDataType = null;
            Assembly assembly = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                assembly = ass;
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    if (types[i].IsSubclassOf(typeof(ASkeletonGpuData)))
                    {
                        m_GPUDataType = types[i];
                    }
                    else if (types[i].IsSubclassOf(typeof(ASkeletonCpuData)))
                    {
                        m_CPUDataType = types[i];
                    }
                    if (m_GPUDataType != null && m_CPUDataType != null) break;
                }
                if (m_GPUDataType != null && m_CPUDataType != null) break;
            }
            AnimationBakeFrameEditor.OnEditorEnd = OnBakeFrameEditorEnd;
            m_nAniMapShaderKey = Shader.PropertyToID("_AnimMap");
            m_nAnimLenShaderKey = Shader.PropertyToID("_AnimLen");
            m_nAnimTotalLenShaderKey = Shader.PropertyToID("_AnimTotalLen");
            m_nTimeEditorKey = Shader.PropertyToID("_TimeEditor");
            m_nAnimOffsetShaderKey = Shader.PropertyToID("_AnimOffset");

            LoadConfig();

            setUpPreview();
        }
        //-----------------------------------------------------
        public void setUpPreview()
        {
            GameObject[] roots = new GameObject[1];
            roots[0] = new GameObject("BakerSkinEditorRoot");
            m_pSkinObject = roots[0];

            if (m_preview == null)
                m_preview = new TargetPreview(AnimationBakeSkinEditor.Instance);
            m_preview.AddPreview(roots[0]);

            TargetPreview.PreviewCullingLayer = 0;
            m_preview.SetCamera(0.01f, 10000f, 60f);
            m_preview.Initialize(roots);
            m_preview.SetPreviewInstance(roots[0]);
            m_preview.SetLightEulerAngle(new Vector3(90, 90, 0));

            m_preview.OnDrawAfterCB = OnDraw;
            //   m_preview.OnMouseDownCB = OnPreviewMouseHit;
        }
        //-----------------------------------------------------
        void OnBakeFrameEditorEnd()
        {
            //              for (int i = 0; i < m_CombineObjects.Count; ++i)
            //              {
            //                  GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(m_CombineObjects[i].combinePrefab);
            //                  if (asset)
            //                  {
            //                      SkinMeshBakerData data = asset.GetComponent<SkinMeshBakerData>();
            //                      data.bakeFrames = m_CombineObjects[i].combineData.bakeFrames;
            //                      EditorUtility.SetDirty(data);
            //                  }
            //              }
        }
        //-----------------------------------------------------
        public void OnDisable()
        {
            if (m_preview != null) m_preview.Destroy();
            m_preview = null;
            SaveConfig();
            AnimationBakeFrameEditor.OnEditorEnd = null;
            Clear();
        }
        //-----------------------------------------------------
        public void Clear()
        {
            if (m_pAgent != null)
            {
                if (m_pAgent.frameData!=null)
                    m_pAgent.frameData.clear();
            }
            for (int i = 0; i < m_CombineObjects.Count; ++i)
            {
                BakeSkinUtil.DestroyObject(m_CombineObjects[i].target);
            }
            m_CombineObjects.Clear();
            m_pSelectCombineData = null;
        }
        //-----------------------------------------------------
        void CollectPrefabCombineData(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath)) return;
            for (int i = 0; i < m_CombineObjects.Count; ++i)
            {
                BakeSkinUtil.DestroyObject(m_CombineObjects[i].target);
            }
            m_CombineObjects.Clear();

            if (rootPath.Contains("Assets/"))
                rootPath = rootPath.Substring("Assets/".Length);


            int nAssetLength = ("Asset/").Length;
            if (rootPath.Length > 1)
            {
                if (rootPath[0] != '/') rootPath = "/" + rootPath;
                if (rootPath[rootPath.Length - 1] != '/') rootPath = rootPath + "/";
            }

            List<sUintData> files = new List<sUintData>();
            string strPath = Application.dataPath + rootPath;

            //load combine data
            DirectoryInfo dir = new DirectoryInfo(strPath);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            //foreach (FileInfo f in fil)
            //{

            //}

            foreach (DirectoryInfo d in dii)
            {
                string fbx = "";
                for (int i = 0; i < d.GetFiles().Length; ++i)
                {
                    if (d.GetFiles()[i].Extension.ToLower().Contains("fbx"))
                    {
                        fbx = d.GetFiles()[i].FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/");
                        break;
                    }
                }
                if (fbx.Length <= 0) continue;
                string anima = "";
                for (int i = 0; i < d.GetFiles().Length; ++i)
                {
                    if (d.GetFiles()[i].Extension.ToLower().Contains("controller"))
                    {
                        anima = d.GetFiles()[i].FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/");
                        break;
                    }
                }
                if (anima.Length <= 0) continue;
                sUintData uintData = new sUintData();
                uintData.file = fbx;
                uintData.animator = anima;

                {
                    for (int i = 0; i < d.GetFiles().Length; ++i)
                    {
                        if (d.GetFiles()[i].Extension.ToLower().Contains(".mat"))
                        {
                            uintData.mateirals.Add(d.GetFiles()[i].FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                        }
                    }
                    if (uintData.mateirals.Count <= 0)
                    {
                        var matFiles = d.Parent.GetFiles("*.mat", SearchOption.AllDirectories);
                        for (int i = 0; i < matFiles.Length; ++i)
                        {
                            uintData.mateirals.Add(matFiles[i].FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                        }
                    }

                    {
                        if (uintData.mateirals.Count <= 0)
                        {
                            DirectoryInfo matdir = new DirectoryInfo(d.FullName + "/Material");
                            if (matdir.Exists)
                            {
                                FileInfo[] matfil = matdir.GetFiles();
                                foreach (FileInfo f in matfil)
                                {
                                    if (f.Extension.ToLower().CompareTo(".mat") != 0)
                                        continue;
                                    long size = f.Length;
                                    uintData.mateirals.Add(f.FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                                }
                            }

                        }
                    }
                }

                if (uintData.mateirals.Count > 0) files.Add(uintData);
                {
                    var animFiles = Directory.GetFiles( d.FullName, "*.anim");
                    DirectoryInfo matdir = new DirectoryInfo(d.FullName + "/animations");
                    foreach (var f in animFiles)
                    {
                        string Extension=  Path.GetExtension(f);
                        if (Extension.ToLower().CompareTo(".anim") != 0)
                            continue;
                        long size = f.Length;
                        uintData.animations.Add(f.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                    }
                }
            }


            // CheckFiles(strPath, files);
            int sizeGrid = 20;
            int gap = 2;
            Vector3 pos = -new Vector3(sizeGrid, 0, sizeGrid);
            for (int i = 0; i < files.Count; ++i)
            {
                var uintData = files[i];
                SkinCombineData data = new SkinCombineData();
                data.filename = uintData.file;
                data.controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(uintData.animator) as AnimatorController;
                data.animatorStates = BakeSkinUtil.GetStatesRecursive(data.controller);
                for (int j = 0; j < uintData.mateirals.Count; ++j)
                {
                    string selFile = uintData.mateirals[j];
                    Material mt = AssetDatabase.LoadAssetAtPath<Material>(selFile) as Material;
                    if (mt == null) continue;
                    if (mt.shader == null) continue;
                    if (mt.shader.name.Contains("GPUSkinningInstance"))
                        continue;
                    data.materials.Add(mt);
                }

                if (data.materials.Count <= 0) continue;

                for (int j = 0; j < uintData.animations.Count; ++j)
                {
                    string selFile = uintData.animations[j];
                    AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(selFile) as AnimationClip;
                    if (animClip == null) continue;
                    data.animations.Add(animClip);
                }

                m_CombineObjects.Add(data);
            }
        }
        //-----------------------------------------------------
        SkinCombineData CreateCombineData(string file)
        {
            string path = System.IO.Path.GetDirectoryName(file);
            var d = System.IO.Directory.CreateDirectory(path);


            string fbxFile = "";
            string anima = "";
            for (int i = 0; i < d.GetFiles().Length; ++i)
            {
                if (d.GetFiles()[i].Extension.ToLower().Contains("controller"))
                {
                    anima = d.GetFiles()[i].FullName.Replace("\\", "/");
                }
                else if (d.GetFiles()[i].Extension.ToLower().Contains("fbx"))
                {
                    fbxFile = d.GetFiles()[i].FullName.Replace("\\", "/");
                }
                if (!string.IsNullOrEmpty(fbxFile) && !string.IsNullOrEmpty(anima)) break;
            }
            if (string.IsNullOrEmpty(fbxFile))
            {
                Debug.LogError("目录[" + path + "]没有fbx 文件");
                return null;
            }
            sUintData uintData = new sUintData();

            uintData.animator = anima.Replace("\\", "/");
            uintData.file = fbxFile.Replace("\\", "/");
            if (uintData.file.Contains(Application.dataPath)) uintData.file = uintData.file.Replace(Application.dataPath + "/", "Assets/");
            if (uintData.animator.Contains(Application.dataPath)) uintData.animator = uintData.animator.Replace(Application.dataPath + "/", "Assets/");

            {
                {
                    DirectoryInfo matdir = new DirectoryInfo(d.FullName + "/Materials");
                    if (matdir.Exists)
                    {
                        FileInfo[] matfil = matdir.GetFiles();
                        foreach (FileInfo f in matfil)
                        {
                            if (f.Extension.ToLower().CompareTo(".mat") != 0)
                                continue;
                            long size = f.Length;
                            uintData.mateirals.Add(f.FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                        }
                    }

                }

                {
                    if (uintData.mateirals.Count <= 0)
                    {
                        DirectoryInfo matdir = new DirectoryInfo(d.FullName + "/Material");
                        if (matdir.Exists)
                        {
                            FileInfo[] matfil = matdir.GetFiles();
                            foreach (FileInfo f in matfil)
                            {
                                if (f.Extension.ToLower().CompareTo(".mat") != 0)
                                    continue;
                                long size = f.Length;
                                uintData.mateirals.Add(f.FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                            }
                        }

                    }
                }
            }
            if (uintData.mateirals.Count <= 0)
            {
                Debug.LogError("目录[" + path + "]没有材质文件");
                return null;
            }

            {
                List<FileInfo> vFiles = new List<FileInfo>();
                DirectoryInfo matdir = new DirectoryInfo(d.FullName + "/animations");
                if (matdir.Exists)
                    vFiles.AddRange(matdir.GetFiles());

                matdir = new DirectoryInfo(d.FullName + "/Animations");
                if (matdir.Exists) vFiles.AddRange(matdir.GetFiles());
                foreach (FileInfo f in vFiles)
                {
                    if (f.Extension.ToLower().CompareTo(".anim") != 0)
                        continue;
                    long size = f.Length;
                    uintData.animations.Add(f.FullName.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/"));
                }
            }
            if (uintData.animations.Count <= 0)
            {
                Debug.LogError("目录[" + path + "]动作文件");
                return null;
            }
            ModelImporter model = AssetImporter.GetAtPath(uintData.file) as ModelImporter;
            if (model != null)
            {
                model.optimizeGameObjects = false;
                model.isReadable = true;
                AssetDatabase.ImportAsset(uintData.file);
            }
            SkinCombineData data = new SkinCombineData();
            data.filename = uintData.file;

            data.controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(uintData.animator) as AnimatorController;
            for (int j = 0; j < uintData.mateirals.Count; ++j)
            {
                string selFile = uintData.mateirals[j];
                Material mt = AssetDatabase.LoadAssetAtPath<Material>(selFile) as Material;
                if (mt == null) continue;
                data.materials.Add(mt);
            }

            if (data.materials.Count <= 0) return null;

            for (int j = 0; j < uintData.animations.Count; ++j)
            {
                string selFile = uintData.animations[j];
                AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(selFile) as AnimationClip;
                if (animClip == null) continue;
                data.animations.Add(animClip);
            }
            return data;
        }
    }
}
#endif