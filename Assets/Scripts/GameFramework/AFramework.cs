using System.Collections;
using System.Collections.Generic;
using Framework.RtgTween;
using Framework.UI;
#if USE_URP
using Framework.URP;
#endif

using Framework.Data;
using Framework.Net;
using Framework.Plugin.AT;
using Framework.Plugin;
#if USE_CUTSCENE
using Framework.Cutscene.Runtime;
using ExternEngine;

#endif

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif

using UnityEngine;

namespace Framework.Core
{
    public abstract class AFramework : IWorldNodeCallback, IWorldMallolCallback, IUserData, ProjectileManagerCB
#if USE_CUTSCENE
        , Framework.Cutscene.Runtime.ICutsceneCallback
#endif
#if USE_URP
        , IURPPassWorkerCallback
#endif
    {
        private static AFramework ms_pMainFramework;
        public static AFramework mainFramework { get { return ms_pMainFramework; } }
#if UNITY_EDITOR
        private static AFramework ms_pEditorFramework;
        public static AFramework editorFramework { get { return ms_pEditorFramework; } }
#endif
        public bool IsEditorMode { get { return m_pGame == null || m_pGame.IsEditor(); } }
        public static bool isStartup { get { return ms_pMainFramework!=null && ms_pMainFramework.m_bStarted; } }

        private IGame m_pGame;
        public IGame gameStartup { get { return m_pGame; } }

        private bool m_bInited = false;
        private bool m_bAwaked = false;
        private bool m_bStarted = false;
        protected long m_lRuntime = 0;
        protected long m_lRuntimeUnScale = 0;

        protected short m_nLogicLock = 0;
        protected short m_nTouchLock = 0;
        protected short m_nPause = 0;

        ShareFrameParams m_pShareParams = null;
        public ShareFrameParams shareParams { get { return m_pShareParams; } }
        private CoroutineSystem m_pCoroutineSystem;
        private FileSystem m_pFileSystem;
        private TimerManager m_pTimerManger;
        private AudioManager m_pAudioManager;
        private SceneMgr m_pSceneMgr;
        private CameraController m_pCameraController;
        private TouchInput m_pTouchInput;
        private World m_pWorld;
        private EventSystem m_pEventSystem;
        private AJobSystem m_pJobSystem;
        private TerrainManager m_pTerrainManager = null;
        private AUIManager m_pUIManager = null;
        private ADataManager m_pDataManager;
        private ANetWork m_pNetwork = null;
        private ProjectileManager m_pProjectileManager = null;
        private BakerSkinningManager m_pBakerSkinMgr = null;

        protected ADynamicTextManager m_pDynamicTextMgr = null;
#if USE_CUTSCENE
        private CutsceneManager m_pCutsceneManger = null;
        public CutsceneManager cutsceneManager { get { return m_pCutsceneManger; } }
#endif
        private int m_nTargetFrame = 30;

        private AnimationCurve m_TimeScaleCurve = null;
        private float m_fTimeScaleCurveDelta = 0;
        private float m_fTimeScaleCurveDuration = 0;

        Base.Random m_pRandom = new Base.Random();
#if USE_FIXEDMATH
        private FFloat m_fLogicAccumoulator = 0.0f;
        private FFloat m_fInvTargetFrame = 0.03333f;
        private FVector3 m_TimeScale = new FVector3(1.0f,0.0f,1.0f);
        public FFloat TimeScale
        {
            get { return m_TimeScale.x * m_TimeScale.z; }
            set { m_TimeScale.x = value; }
        }
        public FFloat TimeScaleDuration
        {
            get { return m_TimeScale.y; }
            set { m_TimeScale.y = value; }
        }
        public FFloat TimeScaleFactor
        {
            get { return m_TimeScale.z; }
            set { m_TimeScale.z = value; }
        }
        public int TargetFrameRate
        {
            get { return m_nTargetFrame; }
            set
            {
#if !USE_SERVER
                Application.targetFrameRate = value;
#endif
                m_nTargetFrame = value;
                if (value <= 0) m_nTargetFrame = 30;
                m_fInvTargetFrame._val = FMath.ToRawFloat(1.0f / (float)m_nTargetFrame);
            }
        }
        public FFloat InvTargetFrameRate
        {
            get { return m_fInvTargetFrame; }
        }
        public FFloat TimeDelta
        {
            get
            {
#if USE_SERVER
                return m_pExterTimer.deltaTime;
#else
                return (FFloat)(Time.deltaTime);
#endif
            }
        }
#else
        private float m_fLogicAccumoulator = 0.0f;
        private float m_fInvTargetFrame = 0.03333f;
        private Vector3 m_TimeScale = new Vector3(1,0,1);
        public float TimeScale
        {
            get { return m_TimeScale.x * m_TimeScale.z; }
            set { m_TimeScale.x = value; }
        }
        public float TimeScaleDuration
        {
            get { return m_TimeScale.y; }
            set { m_TimeScale.y = value; }
        }
        public float TimeScaleFactor
        {
            get { return m_TimeScale.z; }
            set { m_TimeScale.z = value; }
        }
        public int TargetFrameRate
        {
            get { return m_nTargetFrame; }
            set
            {
#if !USE_SERVER
                Application.targetFrameRate = value;
#endif
                m_nTargetFrame = value;
                if (value <= 0) m_nTargetFrame = 30;
                m_fInvTargetFrame =  Mathf.Floor(1.0f / (float)m_nTargetFrame);
            }
        }
        public float InvTargetFrameRate
        {
            get { return m_fInvTargetFrame; }
        }
        public float TimeDelta
        {
            get
            {
#if USE_SERVER
                return m_pExterTimer.deltaTime;
#else
                return Time.deltaTime;
#endif
            }
        }
#endif

#if USE_URP
        URPPostWorker m_UrpWorker = null;
#endif

        private List<IUpdate> m_vAllUpdates = new List<IUpdate>();
        private Dictionary<int, IFixedUpdate> m_vAllFixedUpdates = new Dictionary<int, IFixedUpdate>();
        private Dictionary<int, ILateUpdate> m_vAllLateUpdates = new Dictionary<int, ILateUpdate>();
        private Dictionary<int, ITouchInput> m_vAllTouchInputs = new Dictionary<int, ITouchInput>();
        private Dictionary<int, IKeyInput> m_vAllKeyInputs = new Dictionary<int, IKeyInput>();
        private Dictionary<int, IPause> m_vAllPauses = new Dictionary<int, IPause>();
#if UNITY_EDITOR
        private List<IDrawGizmos> m_vAllDrawGizmos = new List<IDrawGizmos>();
#endif
        public ANetWork netWork { get { return m_pNetwork; } }
        public ADataManager dataManager { get { return m_pDataManager; } }
        public CameraController cameraController { get { return m_pCameraController; } }
        public AudioManager audioManager { get { return m_pAudioManager; } }
        public SceneMgr sceneManager { get { return m_pSceneMgr; } }
        public TerrainManager terrainManager { get { return m_pTerrainManager; } }
        public TimerManager timerManager { get { return m_pTimerManger; } }
        public FileSystem FileSystem { get { return m_pFileSystem; } }
        public World gameWorld { get { return m_pWorld; } }
        public EventSystem eventSystem { get { return m_pEventSystem; } }
        public CoroutineSystem coroutineSystem { get{ return m_pCoroutineSystem; }}
        public AJobSystem jobSystem { get { return m_pJobSystem; } }
        public ProjectileManager projectileManager { get { return m_pProjectileManager; } }
        public AUIManager uiManager { get { return m_pUIManager; } }
        private List<AModule> m_vModdules = new List<AModule>();
        protected AgentTree m_pAgentTree = null;
        public AgentTree mainAT{get { return m_pAgentTree; }}
        public TouchInput touchInput { get { return m_pTouchInput; } }
        public BakerSkinningManager bakerSkinManager { get { return m_pBakerSkinMgr; } }
        public ADynamicTextManager dynamicTextManager { get { return m_pDynamicTextMgr; } }
        public void Init(IGame game)
        {
            m_pShareParams = new ShareFrameParams(this);
#if UNITY_EDITOR
            if (game.IsEditor())
                ms_pEditorFramework = this;
#endif
            ms_pMainFramework = this;
            if (m_bInited)
                return;
            if (m_pGame != null)
                return;

            m_pAudioManager = game.GetAudioMgr();

            m_bAwaked = false;
            m_bStarted = false;
            m_pFileSystem = AddModule<FileSystem>();
            m_pTouchInput = AddModule<TouchInput>();
            m_pCoroutineSystem = AddModule<CoroutineSystem>();
            m_pJobSystem = AddModule<AsyncJobSystem>();
            m_pTimerManger = AddModule<TimerManager>();
            m_pSceneMgr = AddModule<SceneMgr>();
            m_pCameraController = AddModule<CameraController>();
            m_pWorld = AddModule<World>();
            m_pEventSystem = AddModule<EventSystem>();
            m_pProjectileManager = AddModule<ProjectileManager>();
            m_pTerrainManager = AddModule<TerrainManager>();
#if USE_URP
            m_UrpWorker = AddModule<URPPostWorker>();
#endif
#if USE_VIDEOSYSTEM
            AddModule<VideoSystem>();
#endif
#if USE_CUTSCENE
            m_pCutsceneManger = AddModule<CutsceneManager>();
#endif
            m_pBakerSkinMgr = AddModule<BakerSkinningManager>();

            m_pGame = game;

            if (m_pAgentTree != null)
                AgentTreeManager.getInstance().UnloadAT(m_pAgentTree);
            m_pAgentTree = AgentTreeManager.getInstance().LoadAT(ATModuleSetting.MainAT);
            if (m_pAgentTree != null)
            {
                m_pAgentTree.AddOwnerClass(this);
                m_pAgentTree.Enable(true);
            }
            AgentTreeManager.getInstance().RegisterClass(this);
            if(this is AgentTreeDoActionCallback)
                AgentTreeManager.getInstance().Register(this as AgentTreeDoActionCallback);

            VersionData.Parser(game.GetFileStreamType(), "version");

            OnInit();
            m_bInited = true;
            for (int i =0; i < m_vModdules.Count; ++i)
            {
                m_vModdules[i].Init(this);
            }

            m_pFileSystem.Build(game.GetFileStreamType(), "base_pkg", VersionData.version, VersionData.base_pack_cnt, VersionData.assetbundleEncryptKey, VersionData.sceneStreamAB);
        }
        //------------------------------------------------------
        public void Awake()
        {
            if (m_bAwaked)
                return;
            m_bAwaked = true;
            for (int i = 0; i < m_vModdules.Count; ++i)
            {
                m_vModdules[i].Awake();
            }
#if USE_URP
            CameraUtil.SetURPAsset(this.gameStartup.GetURPAsset());
#endif
            for (int i = 0; i < m_vModdules.Count; ++i)
            {
                RegisterFunction(m_vModdules[i]);
            }
            OnAwake();
            m_pWorld.RegisterCallback(this);
        }
        //------------------------------------------------------
        protected abstract void OnInit();
        //------------------------------------------------------
        protected abstract void OnAwake();
        //------------------------------------------------------
        public void Start()
        {
            if (m_bStarted)
                return;
            m_bStarted = true;
            GameQuality.Init();
            for (int i = 0; i < m_vModdules.Count; ++i)
            {
                m_vModdules[i].Start();
            }
            OnStart();

            if (m_pAgentTree != null)
            {
                m_pAgentTree.Enter();
            }
        }
        protected abstract void OnStart();
        //------------------------------------------------------
        public virtual void ResetRuntime(uint nResetFlags = 0xffffffff)
        {
            m_pTouchInput.ResetRuntime();
            if(m_pShareParams!=null) m_pShareParams.ClearRuntimeDatas();
            ClearTimeScaleCurve();
            m_nPause = 0;
            ClearLock();
        }
        //------------------------------------------------------
        public virtual void ClearLock()
        {
            m_nLogicLock = 0;
            m_nTouchLock = 0;
        }
        //------------------------------------------------------
        public bool IsLogicLock()
        {
            return m_nLogicLock > 0;
        }
        //------------------------------------------------------
        public void LogicLock(bool bLock)
        {
            if (bLock) m_nLogicLock++;
            else
            {
                m_nLogicLock--;
                if (m_nLogicLock < 0) m_nLogicLock = 0;
            }
        }
        //------------------------------------------------------
        public short LogicLockCount { get { return m_nLogicLock; } set { m_nLogicLock = value; } }
        //------------------------------------------------------
        public bool IsTouchLock()
        {
            return m_nTouchLock > 0;
        }
        //------------------------------------------------------
        public short TouchLockCount { get { return m_nTouchLock; } set { m_nTouchLock = value; } }
        //------------------------------------------------------
        public void TouchLock(bool bLock)
        {
            if (bLock) m_nTouchLock++;
            else
            {
                m_nTouchLock--;
                if (m_nTouchLock < 0) m_nTouchLock = 0;
            }
        }
        //------------------------------------------------------    
        public virtual void Resume()
        {
            if (m_nPause > 0)
            {
                m_nPause--;
                OnPause(m_nPause > 0);
            }
        }
        //------------------------------------------------------    
        public virtual void Pause()
        {
            m_nPause++;
            OnPause(m_nPause > 0);
        }
        //------------------------------------------------------
        public bool IsPause()
        {
            return m_nPause > 0;
        }
        //------------------------------------------------------    
        protected virtual void OnPause(bool bPause)
        {
            foreach (var db in m_vAllPauses)
                db.Value.OnPause(bPause);
        }
        //------------------------------------------------------
        public void Update(float fFrameTime)
        {
            if (!m_bInited || !m_bStarted || !m_bAwaked)
                return;

            FpsStat.getInstance().Update();
#if USE_SERVER
            m_pExterTimer.DoUpdate(fFrameTime);
            m_lRuntime += (int)(fFrameTime * 1000);
            m_lRuntimeUnScale = m_pExterTimer.realtimeSinceStartup;
#else
            m_lRuntime = (int)(Time.time * 1000);
            m_lRuntimeUnScale = (int)(Time.unscaledTime * 1000);
#endif
            bool bLockFrame = IsPause() || IsLogicLock();

            if(bLockFrame)
            {
                //for (int i = 0; i < m_vModdules.Count; ++i)
                //{
                //    m_vModdules[i].Update(0.0f);
                //}
                //for (int i = 0; i < m_vAllUpdates.Count; i++)
                //    m_vAllUpdates[i].Update(0.0f);

                //AgentTreeManager.getInstance().Update(0.0f);
                //RtgTweenerManager.getInstance().update(0);
                //OnUpdate(0.0f);
            }
            else
            {
                m_fLogicAccumoulator += Time.deltaTime;
                while (m_fLogicAccumoulator >= m_fInvTargetFrame)
                {
                    m_fLogicAccumoulator -= m_fInvTargetFrame;
                    for (int i = 0; i < m_vModdules.Count; ++i)
                    {
                        m_vModdules[i].Update(m_fInvTargetFrame);
                    }
                    for (int i = 0; i < m_vAllUpdates.Count; i++)
                        m_vAllUpdates[i].Update(m_fInvTargetFrame);

                    AgentTreeManager.getInstance().Update(m_fInvTargetFrame);
                    RtgTweenerManager.getInstance().update((long)(m_fInvTargetFrame * 1000));
                    OnUpdate(m_fInvTargetFrame);
                }
            }

            if (m_TimeScale.y >= 0)
            {
                m_TimeScale.y -= m_fInvTargetFrame;
                if (m_TimeScale.y <= 0)
                {
                    m_TimeScale.z = 1.0f;
                }
            }
            UpdateTimeScaleCurve(Time.unscaledDeltaTime);
#if !USE_SERVER
            Time.timeScale = m_TimeScale.x * m_TimeScale.z;
#endif
            if (m_pDynamicTextMgr != null) m_pDynamicTextMgr.Update(fFrameTime);
        }
        //------------------------------------------------------
        protected abstract void OnUpdate(float fTime);
        //------------------------------------------------------
        public void LateUpdate()
        {
            if (!m_bInited || !m_bStarted || !m_bAwaked)
                return;
            float fDelta = Time.deltaTime;
            foreach (var db in m_vAllLateUpdates)
                db.Value.LateUpdate(fDelta);

            OnLateUpdate(fDelta);

            GameQuality.Update(fDelta);
            if (m_pShareParams != null) m_pShareParams.ClearLogicTemp();
        }
        //------------------------------------------------------
        protected virtual void OnLateUpdate(float fFrameTime) { }
        //------------------------------------------------------
        public void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            foreach (var db in m_vAllFixedUpdates)
                db.Value.FixedUpdate(fixedDeltaTime);
            OnFixedUpdate(fixedDeltaTime);
        }
        //------------------------------------------------------
        protected virtual void OnFixedUpdate(float fFrameTime) { }
        //-------------------------------------------------
        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            for (int i = 0; i < m_vAllDrawGizmos.Count; i++)
                m_vAllDrawGizmos[i].DrawGizmos();
#endif
        }
        //-------------------------------------------------
        public void OnTouchBegin(ATouchInput.TouchData touch)
        {
          //  AgentTreeManager.getInstance().ExecuteEvent
            foreach (var db in m_vAllTouchInputs)
                db.Value.OnTouchBegin(touch);
            AgentTreeManager.getInstance().MouseInputEvent(touch.touchID, touch.ToATData(EATMouseType.Begin));
#if USE_GUIDESYSTEM
            Plugin.Guide.GuideSystem.getInstance().OnTouchBegin(touch.touchID, touch.position, touch.deltaPosition);
#endif
        }
        //-------------------------------------------------
        public void OnTouchMove(ATouchInput.TouchData touch)
        {
            foreach (var db in m_vAllTouchInputs)
                db.Value.OnTouchMove(touch);

            AgentTreeManager.getInstance().MouseInputEvent(touch.touchID, touch.ToATData(EATMouseType.Move));
#if USE_GUIDESYSTEM
            Plugin.Guide.GuideSystem.getInstance().OnTouchMove(touch.touchID, touch.position, touch.deltaPosition);
#endif
        }
        //-------------------------------------------------
        public void OnTouchWheel(float wheel, Vector2 mouse)
        {
            foreach (var db in m_vAllTouchInputs)
                db.Value.OnTouchWheel(wheel, mouse);

            ATMouseData atMouse = new ATMouseData();
            atMouse.state = EATMouseType.Wheel;
            atMouse.position = mouse;
            atMouse.lastPosition = mouse;
            atMouse.deltaPosition = new Vector2(wheel, wheel);
            AgentTreeManager.getInstance().MouseInputEvent(-1, atMouse);
        }
        //-------------------------------------------------
        public void OnTouchEnd(ATouchInput.TouchData touch)
        {
            foreach (var db in m_vAllTouchInputs)
                db.Value.OnTouchEnd(touch);
            AgentTreeManager.getInstance().MouseInputEvent(touch.touchID, touch.ToATData(EATMouseType.End));
#if USE_GUIDESYSTEM
            Plugin.Guide.GuideSystem.getInstance().OnTouchEnd(touch.touchID, touch.position, touch.deltaPosition);
#endif
        }
        //-------------------------------------------------
        internal void OnKeyDown(KeyCode code)
        {
            foreach (var db in m_vAllKeyInputs)
                db.Value.OnKeyDown(code);
         //   AgentTreeManager.getInstance().KeyInputEvent(code, true);
        }
        //-------------------------------------------------
        internal void OnKeyUp(KeyCode code)
        {
            foreach (var db in m_vAllKeyInputs)
                db.Value.OnKeyUp(code);
        //    AgentTreeManager.getInstance().KeyInputEvent(code, false);
        }
        //------------------------------------------------------
        public void AddFixedUpdate(int hashCode, IFixedUpdate update)
        {
            if (update == null || m_vAllFixedUpdates.ContainsKey(hashCode)) return;
            m_vAllFixedUpdates.Add(hashCode, update);
        }
        //------------------------------------------------------
        public void RemoveFixedUpdate(int hashCode)
        {
            m_vAllFixedUpdates.Remove(hashCode);
        }
        //------------------------------------------------------
        public void AddLateUpdate(int hashCode, ILateUpdate update)
        {
            if (update == null || m_vAllLateUpdates.ContainsKey(hashCode)) return;
            m_vAllLateUpdates.Add(hashCode, update);
        }
        //------------------------------------------------------
        public void RemoveLateUpdate(int hashCode)
        {
            m_vAllLateUpdates.Remove(hashCode);
        }
        //------------------------------------------------------
        public void AddTouchInput(int hashCode, ITouchInput update)
        {
            if (update == null || m_vAllTouchInputs.ContainsKey(hashCode)) return;
            m_vAllTouchInputs.Add(hashCode, update);
        }
        //------------------------------------------------------
        public void RemoveTouchInput(int hashCode)
        {
            if (m_vAllTouchInputs.ContainsKey(hashCode))
                m_vAllTouchInputs.Remove(hashCode);
        }
        //------------------------------------------------------
        public void AddKeyInput(int hashCode, IKeyInput update)
        {
            if (update == null || m_vAllLateUpdates.ContainsKey(hashCode)) return;
            m_vAllKeyInputs.Add(hashCode, update);
        }
        //------------------------------------------------------
        public void RemoveKeyInput(int hashCode)
        {
            m_vAllKeyInputs.Remove(hashCode);
        }
        //------------------------------------------------------
        public void AddOnPause(int hashCode, IPause pause)
        {
            if (pause == null || m_vAllPauses.ContainsKey(hashCode)) return;
            m_vAllPauses.Add(hashCode, pause);
        }
        //------------------------------------------------------
        public void RemoveOnPause(int hashCode)
        {
            m_vAllPauses.Remove(hashCode);
        }
#if USE_UNITY_JOB
        //------------------------------------------------------
        public void AddJob(IJobUpdate async, IUserData userData = null)
        {
            if(m_pJobSystem!=null)
                m_pJobSystem.AddJob(async, userData);
        }
        //------------------------------------------------------
        public void RemoveJob(IJobUpdate async)
        {
            if (async == null) return;
            if (m_pJobSystem != null)
                m_pJobSystem.RemoveJob(async);
        }
#endif
        //------------------------------------------------------
        public void AddThreadJob(IThreadJob async, int millSleep = 500, IUserData userData = null)
        {
            if (m_pJobSystem != null)
                m_pJobSystem.AddThread(async, millSleep, userData);
        }
        //------------------------------------------------------
        public void RemoveThreadJob(IThreadJob async)
        {
            if (async == null) return;
            if (m_pJobSystem != null)
                m_pJobSystem.RemoveThread(async);
        }
        //------------------------------------------------------
        public void RegisterFunction(IUserData pointer, int hashCode = 0)
        {
            if (pointer == null) return;
            if (hashCode == 0) hashCode = pointer.GetType().GetHashCode();

            AModule modulePtr = pointer as AModule;
            if(modulePtr!=null)
            {
                var update = pointer as IUpdate;
                if (update != null) m_vAllUpdates.Add(update);
            }

            IFixedUpdate fixedUpdate = pointer as IFixedUpdate;
            if (fixedUpdate != null)
                AddFixedUpdate(hashCode, fixedUpdate);


            ILateUpdate lateUpdate = pointer as ILateUpdate;
            if (lateUpdate != null)
                AddLateUpdate(hashCode, lateUpdate);

            ISceneCallback sceneCb = pointer as ISceneCallback;
            if(sceneCb!=null && m_pSceneMgr!=null)
            {
                m_pSceneMgr.Register(sceneCb);
            }

            IWorldNodeCallback worldNodeCallback = pointer as IWorldNodeCallback;
            if (worldNodeCallback != null && m_pWorld != null)
                m_pWorld.RegisterCallback(worldNodeCallback);

            AddTouchInput(hashCode, pointer as ITouchInput);
            AddKeyInput(hashCode, pointer as IKeyInput);

            IUICallback uiCallback = pointer as IUICallback;
            if (uiCallback !=null && m_pUIManager!=null)
            {
                m_pUIManager.RegisterCallback(uiCallback);
            }

            IPause pauseCB = pointer as IPause;
            if (pauseCB != null)
                AddOnPause(hashCode, pauseCB);

#if !USE_SERVER && USE_UNITY_JOB
            var async = pointer as IJobUpdate;
            if (async != null) AddJob(async);
#endif
            var threadJob = pointer as IThreadJob;
            if (threadJob != null) AddThreadJob(threadJob);
#if UNITY_EDITOR
            var gizmos = pointer as IDrawGizmos;
            if (gizmos != null) m_vAllDrawGizmos.Add(gizmos);
#endif
            OnRegisterFunction(pointer, hashCode);
        }
        //------------------------------------------------------
        protected virtual void OnRegisterFunction(IUserData pointer, int hashCode = 0) { }
        //------------------------------------------------------
        public void UnRegisterFunction(IUserData pointer, int hashCode = 0)
        {
            if (pointer == null) return;
            if (hashCode == 0) hashCode = pointer.GetType().GetHashCode();

            var update = pointer as IUpdate;
            if (update != null) m_vAllUpdates.Remove(update);

            IFixedUpdate fixedUpdate = pointer as IFixedUpdate;
            if (fixedUpdate != null)
                RemoveFixedUpdate(hashCode);


            ILateUpdate lateUpdate = pointer as ILateUpdate;
            if (lateUpdate != null)
                RemoveLateUpdate(hashCode);

            ISceneCallback sceneCb = pointer as ISceneCallback;
            if (sceneCb != null && m_pSceneMgr != null)
            {
                m_pSceneMgr.UnRegister(sceneCb);
            }
            IWorldNodeCallback worldNodeCallback = pointer as IWorldNodeCallback;
            if (worldNodeCallback != null && m_pWorld != null)
                m_pWorld.UnRegisterCallback(worldNodeCallback);

            RemoveTouchInput(hashCode);
            RemoveKeyInput(hashCode);

            IUICallback uiCallback = pointer as IUICallback;
            if (uiCallback != null && m_pUIManager != null)
            {
                m_pUIManager.UnRegisterCallback(uiCallback);
            }

            IPause pauseCB = pointer as IPause;
            if (pauseCB != null)
                RemoveOnPause(hashCode);

#if !USE_SERVER && USE_UNITY_JOB
            var async = pointer as IJobUpdate;
            if (async != null) RemoveJob(async);
#endif
            var threadJob = pointer as IThreadJob;
            if (threadJob != null) RemoveThreadJob(threadJob);
#if UNITY_EDITOR
            var gizmos = pointer as IDrawGizmos;
            if (gizmos != null) m_vAllDrawGizmos.Remove(gizmos);
#endif

            OnUnRegisterFunction(pointer, hashCode);
        }
        //-------------------------------------------------
        protected virtual void OnUnRegisterFunction(IUserData pointer, int hashCode = 0) { }

        //------------------------------------------------------
        public void Destroy()
        {
            m_vAllLateUpdates.Clear();
            m_vAllFixedUpdates.Clear();
            m_vAllUpdates.Clear();

            m_vAllTouchInputs.Clear();
            m_vAllKeyInputs.Clear();

            m_vAllPauses.Clear();
#if UNITY_EDITOR
            m_vAllDrawGizmos.Clear();
#endif
            if (m_pAgentTree != null)
            {
                m_pAgentTree.Exit();
                m_pAgentTree.Enable(false);
                AgentTreeManager.getInstance().UnloadAT(m_pAgentTree);
                m_pAgentTree = null;
            }
            AgentTreeManager.getInstance().Shutdown();
            OnDestroy();
            if (m_pDynamicTextMgr != null) m_pDynamicTextMgr.Destroy();
            m_pDynamicTextMgr = null;

            for (int i = 0; i < m_vModdules.Count; ++i)
            {
                if (m_pFileSystem == m_vModdules[i]) continue;
                m_vModdules[i].Destroy();
            }
            m_vModdules.Clear();

            m_pFileSystem.Destroy();
        }
        //------------------------------------------------------
        protected abstract void OnDestroy();
        //------------------------------------------------------
        public T GetBindData<T>(string name = null) where T : UnityEngine.Object
        {
            if (m_pGame == null || m_pGame.GetDatas() == null) return null;
            var gameDatas = m_pGame.GetDatas();
            for (int i = 0; i < gameDatas.Length; ++i)
            {
                if (gameDatas[i] == null) continue;
                if (!string.IsNullOrEmpty(name) && gameDatas[i].name.CompareTo(name) != 0)
                    continue;

                if (gameDatas[i] is T)
                    return gameDatas[i] as T;
            }
            return null;
        }
        //------------------------------------------------------
        protected T AddModule<T>() where T : AModule, new()
        {
            for(int i =0; i < m_vModdules.Count; ++i)
            {
                if (m_vModdules[i] is T)
                    return m_vModdules[i] as T;
            }
            T moduel = new T();

            if(m_bInited)
                moduel.Init(this);
            if (m_bAwaked)
                moduel.Awake();
            if (m_bStarted)
                moduel.Start();

            OnAddModule(moduel);

            if(m_bStarted)
                RegisterFunction(moduel);
            m_vModdules.Add(moduel);
            return moduel;
        }
        //------------------------------------------------------
        public T GetModule<T>() where T : AModule
        {
            for (int i = 0; i < m_vModdules.Count; ++i)
            {
                if (m_vModdules[i] is T)
                    return m_vModdules[i] as T;
            }
            return null;
        }
        //------------------------------------------------------
        public void RegisterModule(AModule moduel)
        {
            //! is inited,do init
            if(m_pGame != null)
                moduel.Init(this);
            if (m_vModdules.Contains(moduel))
                return;
            OnAddModule(moduel);
            RegisterFunction(moduel);
            m_vModdules.Add(moduel);
        }
        //------------------------------------------------------
        public void UnRegisterModule(AModule module)
        {
            UnRegisterFunction(module);
            m_vModdules.Remove(module);
            OnRemoveModule(module);
        }
        //------------------------------------------------------
        void OnAddModule(AModule module)
        {
            if (module is TerrainManager)
                m_pTerrainManager = module as TerrainManager;
            else if (module is AUIManager)
                m_pUIManager = module as AUIManager;
            else if (module is ADataManager)
                m_pDataManager = module as ADataManager;
            else if (module is ANetWork)
                m_pNetwork = module as ANetWork;
        }
        //------------------------------------------------------
        void OnRemoveModule(AModule module)
        {
            if (module is TerrainManager)
                m_pTerrainManager = null;
            else if (module is AUIManager)
                m_pUIManager = null;
            else if (module is ADataManager)
                m_pDataManager = null;
            else if (module is ANetWork)
                m_pNetwork = null;
        }
        //------------------------------------------------------
        public Coroutine BeginCoroutine(IEnumerator enumerator)
        {
            if (this.m_pGame == null)
                return null;
            return this.m_pGame.BeginCoroutine(enumerator);
        }
        //------------------------------------------------------
        public void EndCoroutine(Coroutine enumerator)
        {
            if (this.m_pGame == null) return;
            this.m_pGame.EndCoroutine(enumerator);
        }
        //------------------------------------------------------
        public void EndCoroutine(IEnumerator enumerator)
        {
            if (this.m_pGame == null) return;
            this.m_pGame.EndCoroutine(enumerator);
        }
        //------------------------------------------------------
        public virtual void PreloadAsset(string file, bool bAsync = true)
        {

        }
        //------------------------------------------------------
        public void PreloadAssets(HashSet<string> vPreLoad, bool bAsync = true)
        {
            if (vPreLoad.Count <= 0) return;
            foreach (var db in vPreLoad)
            {
                PreloadAsset(db, bAsync);
            }
        }
        //------------------------------------------------------
        public void PreloadInstances(List<string> vPreLoad)
        {
            if (vPreLoad.Count <= 0) return;
            Dictionary<string, int> vStats = shareParams.stringCatchStatsMap;
            vStats.Clear();
            for (int i = 0; i < vPreLoad.Count; ++i)
            {
                int stat = 0;
                if (!vStats.TryGetValue(vPreLoad[i], out stat))
                {
                    vStats.Add(vPreLoad[i], 1);
                }
                else if (stat < 3)
                {
                    vStats[vPreLoad[i]] = stat + 1;
                }
            }
            foreach (var db in vStats)
            {
                int cnt = db.Value - FileSystemUtil.StatsInstanceCount(db.Key);
                if (cnt < 0) continue;
                PreloadInstance(db.Key, cnt);
            }
            vStats.Clear();
        }
        //------------------------------------------------------
        public virtual void PreloadInstance(string file, int cnt)
        {
            for (int i = 0; i < cnt; ++i)
                FileSystemUtil.PreSpawnInstance(file, true, false);
        }
        //------------------------------------------------------
        public virtual IDSoundData GetSoundByID(uint nId)
        {
            return IDSoundData.NULL;
        }
        //------------------------------------------------------
        public virtual SceneParam GetSceneByID(uint nId)
        {
            return SceneParam.DEF;
        }
        //------------------------------------------------------
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
        //------------------------------------------------------
        public long GetRunTime()
        {
            return m_lRuntime;
        }
        //------------------------------------------------------
        public long GetRunUnScaleTime()
        {
            return m_lRuntimeUnScale;
        }
        //-------------------------------------------------
        public void ApplayTimeScaleByCurve(AnimationCurve curve)
        {
            if (curve == null)
                return;
            m_fTimeScaleCurveDuration = BaseUtil.GetCurveMaxTime(curve);
            if (m_fTimeScaleCurveDuration <= 0.0f) return;
            m_fTimeScaleCurveDelta = 0.0f;
            m_TimeScaleCurve = curve;
            TimeScaleDuration = 0.0f;
        }
        //------------------------------------------------------
        void UpdateTimeScaleCurve(float fFrame)
        {
            if (m_fTimeScaleCurveDuration > 0 && m_TimeScaleCurve != null)
            {
                m_fTimeScaleCurveDelta += fFrame;
                TimeScaleFactor = m_TimeScaleCurve.Evaluate(m_fTimeScaleCurveDelta / m_fTimeScaleCurveDuration);
                if (m_fTimeScaleCurveDelta >= m_fTimeScaleCurveDuration)
                {
                    m_fTimeScaleCurveDuration = 0.0f;
                    m_TimeScaleCurve = null;
                }
            }
        }
        //------------------------------------------------------
        public void ClearTimeScaleCurve()
        {
            m_fTimeScaleCurveDelta = 0.0f;
            m_fTimeScaleCurveDuration = 0.0f;
            m_TimeScaleCurve = null;
#if USE_FIXEDMATH
            m_TimeScale = FVector3.zZero;
#else
            m_TimeScale =  Vector3.right;
#endif
        }
        //-------------------------------------------------
        public void SetRandomSeed(uint seed)
        {
            m_pRandom.randSeed = seed;
        }
        //-------------------------------------------------
        public ulong GetRandomSeed()
        {
            return m_pRandom.randSeed;
        }
        //-------------------------------------------------
        public float GetRamdom(float min, float max)
        {
            float result = m_pRandom.Range((int)(min * 1000), (int)(max * 1000)) * 0.001f;
            return result;
        }
        //-------------------------------------------------
        public int GetRamdom(int min, int max)
        {
            int result = m_pRandom.Range(min, max);
            return result;
        }
        //-------------------------------------------------
        public bool CheckerRandom(int cur, int min = 0, int max = 1000)
        {
            if (cur <= 0) return false;
            if (cur >= max) return true;
            return cur >= GetRamdom(min, max);
        }
        //-------------------------------------------------
        public bool CheckerRandom(float cur)
        {
            return CheckerRandom((int)(cur * 1000), 0, 1000);
        }
        //------------------------------------------------------
        internal virtual void OnClearWorld()
        {
            foreach(var db in m_vModdules)
            {
                db.OnClearWorld();
            }
        }
        //------------------------------------------------------
        public virtual void OnActorAttrDirty(Actor pActor, byte attrType, FFloat oldValue, FFloat newValue)
        {
            if(m_pDynamicTextMgr!=null)
                m_pDynamicTextMgr.OnActorAttrChange(pActor, attrType, oldValue, newValue);
        }
        //------------------------------------------------------
        public abstract BaseEvent OnMallocEvent(int evntType);
        //------------------------------------------------------
        public abstract AWorldNode OnExcudeWorldNodeMalloc(EActorType type);
        //------------------------------------------------------
        public virtual void OnWorldNodeStatus(AWorldNode pNode, EWorldNodeStatus status, IUserData userVariable = null)
        {
        }
        //------------------------------------------------------
        public virtual void OnNetReConnect(IUserData userData = null)
        {

        }
        //------------------------------------------------------
        public virtual void OnNetSessionState(Net.AServerSession pSession, Net.ESessionState eState)
        {

        }
        //------------------------------------------------------
        public virtual FVector3 GetExternLogicAppendSpeed()
        {
            return FVector3.zero;
        }
        //------------------------------------------------------
        public virtual void OnGameQuality(int quality, AConfig config)
        {
            if (config == null || !(config is QualityConfig)) return;
            FileSystem fileSystem = m_pFileSystem as FileSystem;
            QualityConfig qualityCfg = (QualityConfig)config;
            if (fileSystem != null) fileSystem.SetCapability(qualityCfg.OneFrameCost, qualityCfg.MaxInstanceCount, qualityCfg.DestroyDelayTime);
           // if (m_pLodMgr != null)
           //     m_pLodMgr.ForceLowerLOD(qualityCfg.bForceLOWLOD);

#if USE_URP
            CameraUtil.SetPostProcess(qualityCfg.postProcess);
            CameraUtil.SetURPAsset(qualityCfg.urpAsset);
            URP.URPPostWorker.SetPassFlags(qualityCfg.nURPPassFlags);
#endif
            m_pFileSystem.SetSreamReadBufferSize(qualityCfg.ReadAbStreamBuffSize);

            TargetFrameRate = qualityCfg.TargetFrameRate;
        }
        //-------------------------------------------------
        public virtual void TriggertEvent(int nEventID, IUserData pUseData) { }

        public virtual void TriggertEvent(int nEventID, IUserData pTrigger, IUserData pTarget) { }
        //------------------------------------------------------
        public void OnTriggerEvent(BaseEvent pEvent, bool bAutoClear = true)
        {
            OnTriggerEvent(pEvent, null, bAutoClear);
        }
        public virtual void OnTriggerEvent(BaseEvent pEvent, IUserData pBinderData, bool bAutoClear = true) { }
        //------------------------------------------------------
        public void OnTriggerEvent(System.Collections.Generic.List<BaseEvent> vEvent, bool bAutoClear = true)
        {
            OnTriggerEvent(vEvent, null, bAutoClear);
        }
        //------------------------------------------------------
        public virtual void OnTriggerEvent(System.Collections.Generic.List<BaseEvent> vEvent, IUserData pBinderData, bool bAutoClear = true) { }
        //------------------------------------------------------

        public virtual void OnTriggerEvent(int nEventID, bool bAutoClear = true)
        {
        }
        //------------------------------------------------------
        public virtual bool OnUIWidgetTrigger(UI.EventTriggerListener pTrigger, UnityEngine.EventSystems.BaseEventData eventData, EUIEventType triggerType, int guid, int listIndex, params IUserData[] argvs)
        {
#if USE_GUIDESYSTEM
            var guideTriggerType = Plugin.Guide.GuideGuidUtl.GetTriggerType(triggerType);
            if (guid != 0 && guideTriggerType != Plugin.Guide.EUIWidgetTriggerType.None)
                Plugin.Guide.GuideSystem.getInstance().OnUIWidgetTrigger(guid, listIndex, guideTriggerType, argvs);
#endif
            return false;
        }
#if USE_URP
        //------------------------------------------------------
        public virtual void OnURPBuildPassLogic(APostRenderPass pass, List<APostLogic> vLogics)
        {
        }
#endif
#if USE_CUTSCENE
#region Cutscene Playable Callbacks
        //------------------------------------------------------
        public virtual void OnCutsceneStatus(int cutsceneId, EPlayableStatus eStatus)
        {
        }
        //------------------------------------------------------
        public virtual bool OnCutscenePlayableCreateClip(CutscenePlayable playable, CutsceneTrack track, IBaseClip clip)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnCutscenePlayableDestroyClip(CutscenePlayable playable, CutsceneTrack track, IBaseClip clip)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnCutscenePlayableFrameClip(CutscenePlayable playable, FrameData frameData)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnCutscenePlayableFrameClipEnter(CutscenePlayable playable, CutsceneTrack track, FrameData frameData)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnCutscenePlayableFrameClipLeave(CutscenePlayable playable, CutsceneTrack track, FrameData frameData)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnCutsceneEventTrigger(CutscenePlayable pPlayablle, CutsceneTrack pTrack, Cutscene.Runtime.IBaseEvent pEvent)
        {
            return false;
        }
        //------------------------------------------------------
        public virtual bool OnAgentTreeExecute(CutsceneAT.Runtime.AgentTree pAgentTree, CutsceneAT.Runtime.BaseNode pNode)
        {
            return false;
        }
        #endregion
#endif
#if USE_ACTORSYSTEM
        //------------------------------------------------------
        public virtual FFloat CalcAttrByType(int formulaType, Actor attacker, Actor target, out byte applayAttrType)
        {
            applayAttrType = byte.MaxValue;
            return 0;
        }
#endif
#region Projectile Callbacks
        //------------------------------------------------------
        public virtual void OnStopProjectile(ProjectileNode pProjectile)
        {
        }
        //------------------------------------------------------
        public virtual void OnDelayStopProjectile(ProjectileNode pProjectile)
        {
        }
        //------------------------------------------------------
        public virtual void OnProjectileUpdate(ProjectileNode pProjectile)
        {
        }
        //------------------------------------------------------
        public virtual void OnLaunchProjectile(ProjectileNode pProjectile)
        {
        }
        //------------------------------------------------------
        public virtual void OnProjectileHit(ProjectileNode pProjectile, HitFrameActor attackData, bool bHitScene, bool bExplode)
        {
        }
        #endregion
        //------------------------------------------------------
        public virtual FFloat OnHitFrameDamage(HitFrameActor hitFrameData)
        {
            return 0;
        }
    }
}
