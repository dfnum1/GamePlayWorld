/********************************************************************
生成日期:	1:11:2020 13:07
类    名: 	ASceneMgr
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public abstract class ASceneMgr : AModule
    {
        internal class SceneNode : IUserData
        {
            public float fDestroyTime = 0;
            public SceneParam sceneParam = SceneParam.DEF;
            public Asset pSceneAsset;
            public AInstanceAble pSubSceneAble = null;
            public InstanceOperiaon pSubSceneOp = null;
            public float Progress
            {
                get
                {
                    if (sceneParam.isCompled) return 1;
                   
                    float process= pSceneAsset != null ? pSceneAsset.GetProgress() : 0;
                    if (pSubSceneOp != null) return process * 0.6f;
                    return process;
                }
            }

            public void Unload(bool bUnloadScene =true)
            {
                if (pSubSceneAble != null)
                    pSubSceneAble.Destroy();
                pSubSceneAble = null;

                if (pSubSceneOp != null) pSubSceneOp.Earse();
                pSubSceneOp = null;

                if (pSceneAsset != null) pSceneAsset.Release(0);
                pSceneAsset = null;
                if (bUnloadScene && !string.IsNullOrEmpty(sceneParam.sceneName))
                {
                    UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneParam.sceneName);
                    if(scene.IsValid())
                        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneParam.sceneName);
                }
                sceneParam.isCompled = false;
                sceneParam.Clear();
                fDestroyTime = 0;
            }

            public void OnSceneLoaded(Asset pAsset)
            {
                if (pAsset != null)
                {
                    if(pSceneAsset!= pAsset)
                    {
                        if (pSceneAsset != null) pSceneAsset.Release();
                    }
                    pAsset.Grab();
                    pSceneAsset = pAsset;
                }
                sceneParam.isCompled = true;
                pSubSceneOp = null;
            }

            public void Destroy()
            {
                //Unload();
                //sceneParam.Clear();
            }
        }
        Stack<SceneNode> m_vSceneList;
        List<SceneNode> m_vDestroyList;
        List<SceneNode> m_vLoading = null;

        //     private ASceneTheme m_pSceneTheme = null;

        List<ISceneCallback> m_vCallback = new List<ISceneCallback>(2);
        private SceneParam m_SceneCheckParam = SceneParam.DEF;
        //-------------------------------------------
        public float Progress
        {
            get
            {
                if (m_vLoading == null || m_vLoading.Count <= 0) return 1;
                float progress = 0;
                for (int i = 0; i < m_vLoading.Count; ++i) progress += m_vLoading[i].Progress;
                return progress / (float)m_vLoading.Count;
            }
        }
        //------------------------------------------------------
        protected override void OnInit()
        {
            m_vLoading = new List<SceneNode>(4);
            m_vSceneList = new Stack<SceneNode>(4);
            m_vDestroyList = new List<SceneNode>(4);
        }
        //------------------------------------------------------
        public void Register(ISceneCallback callback)
        {
            if (callback == null || m_vCallback.Contains(callback)) return;
            m_vCallback.Add(callback);
        }
        //------------------------------------------------------
        public void UnRegister(ISceneCallback callback)
        {
            if (callback == null) return;
            m_vCallback.Remove(callback);
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            if(m_vSceneList!=null)
            {
                foreach (var db in m_vSceneList)
                {
                    db.Unload();
                }
                m_vSceneList.Clear();
            }

            if(m_vDestroyList!=null)
            {
                for (int i = 0; i < m_vDestroyList.Count; ++i)
                {
                    m_vDestroyList[i].Unload();
                }
                m_vDestroyList.Clear();
            }

            if(m_vLoading!=null) m_vLoading.Clear();
            m_SceneCheckParam = SceneParam.DEF;
        }
        //------------------------------------------------------
        public void Free(float fDelay=0)
        {
            if(fDelay>0)
            {
                for (int i = 0; i < m_vDestroyList.Count; ++i)
                {
                    m_vDestroyList[i].fDestroyTime = Time.time + fDelay;
                }
            }
            else
            {
                for (int i = 0; i < m_vDestroyList.Count; ++i)
                {
                    m_vDestroyList[i].Unload();
                }
                m_vDestroyList.Clear();
            }
        }
        //------------------------------------------------------
        protected override void OnStart()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneActived;
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            if (m_SceneCheckParam.IsValid())
            {
                m_SceneCheckParam.isCompled = IsCompleted();
                DoCallback(m_SceneCheckParam);
                m_SceneCheckParam.Clear();
            }
            InnerUpdate(fFrame);

            if(m_vDestroyList!=null && m_vDestroyList.Count>0)
            {
                float curTime = Time.time;
                SceneNode sceneNode;
                for (int i = 0; i < m_vDestroyList.Count;)
                {
                    sceneNode = m_vDestroyList[i];
                    if(sceneNode.fDestroyTime>0 && curTime >= sceneNode.fDestroyTime)
                    {
                        sceneNode.Unload();
                        m_vDestroyList.RemoveAt(i);
                        continue;
                    }
                    ++i;
                }
            }
        }
        //------------------------------------------------------
        protected abstract void InnerUpdate(float fFrame);
        //------------------------------------------------------
        public bool IsCompleted()
        {
            return Progress >= 0.999f;
        }
        //------------------------------------------------------
        public bool IsInLoading(string sceneName)
        {
            for (int i = m_vLoading.Count - 1; i >= 0; --i)
            {
                SceneNode sceneNode = m_vLoading[i];
                if (sceneNode.sceneParam.sceneName.CompareTo(sceneName)==0)
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        SceneNode GetScene(string sceneName)
        {
            SceneNode sceneNode = null;
            foreach (var db in m_vSceneList)
            {
                if (db.sceneParam.sceneName.CompareTo(sceneName)==0)
                {
                    sceneNode = db;
                    return sceneNode;
                }
            }
            for (int i =0; i < m_vDestroyList.Count; ++i)
            {
                if (m_vDestroyList[i].sceneParam.sceneName.CompareTo(sceneName) == 0)
                {
                    sceneNode = m_vDestroyList[i];
                    sceneNode.fDestroyTime = 0;
                    m_vSceneList.Push(sceneNode);
                    m_vDestroyList.RemoveAt(i);
                    break;
                }
            }
            return sceneNode;
        }
        //------------------------------------------------------
        protected void DoCallback(SceneParam sceParam)
        {
            for (int i = 0; i < m_vCallback.Count; ++i)
            {
                if (m_vCallback[i] != null)
                    m_vCallback[i].OnSceneCallback(sceParam);
            }
        }
        //------------------------------------------------------
        public bool LoadScene(SceneParam sceParam, float fDelayPop = 0)
        {
            if (!sceParam.IsValid())
                return false;
            if(string.IsNullOrEmpty(sceParam.sceneName))
            {
                sceParam.isCompled = true;
                DoCallback(sceParam);
                return true;
            }
            if (IsInLoading(sceParam.sceneName)) return true;
            SceneNode pFinder = GetScene(sceParam.sceneName);
            if (pFinder!=null)
            {
                bool bDoCall = true;
                if (pFinder.sceneParam.subScene != null)
                {
                    if(pFinder.sceneParam.subScene.CompareTo(sceParam.subScene)!=0)
                    {
                        pFinder.pSubSceneOp = GetFramework().FileSystem.SpawnInstance(sceParam.subScene, true);
                        pFinder.pSubSceneOp.userData0 = pFinder;
                        pFinder.pSubSceneOp.OnCallback = OnSubSceneLoaded;
                        pFinder.pSubSceneOp.OnSign = OnSubSceneLoadSign;
                        pFinder.pSubSceneOp.pByParent = RootsHandler.ScenesRoot;
                        bDoCall = false;
                    }
                    else
                    {
                        if (pFinder.pSubSceneAble is LevelScene)
                        {
                            LevelScene levelScene = pFinder.pSubSceneAble as LevelScene;
                            GetFramework().shareParams.AddRuntimeData("playerStart", new Variable3(levelScene.playerStart));
                        }
                    }
                }
                if(bDoCall) DoCallback(pFinder.sceneParam);
                return true;
            }
#if !USE_SERVER

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(sceParam.sceneName))
            {
                bool bHas = false;
                string scenePath = "Assets/Scenes/" + sceParam.sceneName + ".unity";
                List<UnityEditor.EditorBuildSettingsScene> editorBuildSettingsScenes = UnityEditor.EditorBuildSettings.scenes != null ? new List<UnityEditor.EditorBuildSettingsScene>(UnityEditor.EditorBuildSettings.scenes) : new List<UnityEditor.EditorBuildSettingsScene>();
                for (int i = 0; i < editorBuildSettingsScenes.Count; ++i)
                {
                    if (editorBuildSettingsScenes[i].path.ToLower().CompareTo(scenePath.ToLower()) == 0)//转为小写进行对比
                    {
                        bHas = true;
                        break;
                    }
                }

                //添加过滤path为空的build scene,并且过滤掉重复
                Dictionary<UnityEditor.GUID, UnityEditor.EditorBuildSettingsScene> actuallySceneList = new Dictionary<UnityEditor.GUID, UnityEditor.EditorBuildSettingsScene>();
                foreach (var item in editorBuildSettingsScenes)
                {
                    if (!string.IsNullOrWhiteSpace(item.path) && actuallySceneList.ContainsKey(item.guid) == false)
                    {
                        actuallySceneList.Add(item.guid, item);
                    }
                }

                //添加到Scene列表中
                editorBuildSettingsScenes.Clear();
                foreach (var item in actuallySceneList)
                {
                    editorBuildSettingsScenes.Add(item.Value);
                }

                UnityEditor.EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

                //如果需要添加新的场景
                if (!bHas && System.IO.File.Exists(scenePath))
                {
                    var newScene = new UnityEditor.EditorBuildSettingsScene(scenePath, true);
                    actuallySceneList.Add(newScene.guid, newScene);

                    editorBuildSettingsScenes.Clear();
                    foreach (var item in actuallySceneList)
                    {
                        editorBuildSettingsScenes.Add(item.Value);
                    }

                    UnityEditor.EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                }
            }
#endif
            sceParam.isCompled = false;
            SceneNode pNode = new SceneNode();
            pNode.sceneParam = sceParam;
            ELoadSceneMode mode = ELoadSceneMode.Single;
            if (sceParam.load == ESceneSignType.PopAll)
            {
                if(fDelayPop>0)
                {
                    mode = ELoadSceneMode.Additive;
                    foreach (var db in m_vSceneList)
                    {
                        db.fDestroyTime = Time.time + fDelayPop;
                        m_vDestroyList.Add(db);
                    }
                }
                else
                {
                    foreach (var db in m_vSceneList)
                    {
                        db.Unload(mode != ELoadSceneMode.Single);
                    }
                }

                m_vSceneList.Clear();
            }
            else if (sceParam.load == ESceneSignType.Pop)
            {
                mode = ELoadSceneMode.Additive;
                SceneNode scene = m_vSceneList.Pop();
                if (scene.sceneParam.unload == ESceneSignType.PopAll)
                {
                    mode = ELoadSceneMode.Single;
                    if (fDelayPop>0)
                    {
                        foreach (var db in m_vSceneList)
                        {
                            db.fDestroyTime = Time.time + fDelayPop;
                            m_vDestroyList.Add(db);
                        }
                    }
                    else
                    {
                        foreach (var db in m_vSceneList)
                        {
                            db.Unload();
                        }
                    }
                    m_vSceneList.Clear();
                }
                if (fDelayPop > 0)
                {
                    mode = ELoadSceneMode.Additive;
                    scene.fDestroyTime = Time.time + fDelayPop;
                    m_vDestroyList.Add(scene);
                }
                else
                    scene.Unload(mode != ELoadSceneMode.Single);
            }
            else if (sceParam.load == ESceneSignType.Add)
                mode = ELoadSceneMode.Additive;
            if (mode == ELoadSceneMode.Single)
                m_vDestroyList.Clear();

            if (!string.IsNullOrEmpty(sceParam.subScene))
            {
                pNode.pSubSceneOp = InstanceOperiaon.Malloc();
                pNode.pSubSceneOp.strFile = sceParam.subScene;
                pNode.pSubSceneOp.userData0 = pNode;
                pNode.pSubSceneOp.OnCallback = OnSubSceneLoaded;
                pNode.pSubSceneOp.OnSign = OnSubSceneLoadSign;
                pNode.pSubSceneOp.pByParent = RootsHandler.ScenesRoot;
            }
            AssetOperiaon pAssetOp = FileSystemUtil.LoadScene(sceParam.sceneFile, sceParam.sceneName, mode, true);
            if (pAssetOp != null)
            {
                pAssetOp.userData = pNode;
                pAssetOp.OnCallback = OnSceneLoaded;
                m_vLoading.Add(pNode);
            }
            return pAssetOp != null;
#else
            SceneNode pNode = new SceneNode();
            pNode.sceneParam = sceParam;
            pNode.OnSceneLoaded(null);
            m_vSceneList.Push(pNode);
            m_SceneCheckParam = pNode.sceneParam;
            return true;
#endif
        }
        //------------------------------------------------------
        void OnSceneActived(UnityEngine.SceneManagement.Scene oldSce, UnityEngine.SceneManagement.Scene newSce)
        {

        }
        //------------------------------------------------------
        public void OnSceneLoaded(AssetOperiaon pOp)
        {
            SceneNode pNode = pOp.userData as SceneNode;
            bool bLoaded = false;
            if (pNode.pSubSceneOp != null)
            {
                pNode.pSubSceneOp.userData1 = pOp.pAsset;
                if (!GetFramework().FileSystem.SpawnInstance(pNode.pSubSceneOp))
                    bLoaded = true;
            }
            else bLoaded = true;
            if(bLoaded)
            {
                pNode.OnSceneLoaded(pOp.pAsset);

                m_vSceneList.Push(pNode);
                m_vLoading.Remove(pNode);
                m_SceneCheckParam = pNode.sceneParam;
            }
        }
        //------------------------------------------------------
        void OnSubSceneLoadSign(InstanceOperiaon pOp)
        {
            SceneNode pNode = pOp.userData0 as SceneNode;
            pOp.bUsed = !m_vDestroyList.Contains(pNode) && pOp.strFile.CompareTo(pNode.sceneParam.subScene) ==0;
        }
        //------------------------------------------------------
        public void OnSubSceneLoaded(InstanceOperiaon pOp)
        {
            SceneNode pNode = pOp.userData0 as SceneNode;
            if (pNode.pSubSceneAble != pOp.pPoolAble)
            {
                if (pNode.pSubSceneAble != null) pNode.pSubSceneAble.Destroy();
            }
            pNode.pSubSceneAble = pOp.pPoolAble;
            if (pOp.pPoolAble)
            {
                BaseUtil.ResetGameObject(pOp.pPoolAble.gameObject, EResetType.All);
                if(pNode.pSubSceneAble is LevelScene)
                {
                    LevelScene levelScene = pNode.pSubSceneAble as LevelScene;
                    GetFramework().shareParams.AddRuntimeData("playerStart", new Variable3(levelScene.playerStart));
                }
            }


            if(pOp.userData1!=null)
            {
                Asset pAssetOp = pOp.userData1 as Asset;
                pNode.OnSceneLoaded(pAssetOp);
            }
            else
                pNode.OnSceneLoaded(null);

            m_vSceneList.Push(pNode);
            m_vLoading.Remove(pNode);
            m_SceneCheckParam = pNode.sceneParam;
        }
    }
}