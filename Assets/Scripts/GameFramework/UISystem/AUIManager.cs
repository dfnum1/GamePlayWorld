/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	AUIManager
作    者:	HappLI
描    述:	UI管理器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Core;
using System;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Framework.UI
{
    public interface IUICallback
    {
        void OnUIAwake(UIBase pBase);
        void OnUIShow(UIBase pBase);
        void OnUIHide(UIBase pBase);
    }

    public abstract class AUIManager : AModule
    {
        public static Vector3[] ms_contersArray = new Vector3[4];
        static Vector3[] ms_contersArray1 = new Vector3[4];
        private readonly float DELTA_DESTROY_TIME = 30;
        struct DeltaDestroy
        {
            public int uiType;
            public float lastTime;
        }
        bool m_bInited = false;
        List<int> m_vTemp = null;

        bool m_bDirtyMaps = false;
        bool m_bLockRemoved = false;
        List<UIBase> m_vUIFraming = null;
        Dictionary<int, UIBase> m_vUIs = null;

        List<UIBase> m_vPreInits = null;
        UIConfig m_UIConfig = null;

        static HashSet<int> ms_vLoading = new HashSet<int>();

        Transform m_pRoot = null;
        Canvas m_UIRootCanvas = null;
        RectTransform[] m_DynamicUIRootRT = null;
        RectTransform m_UIRootRT = null;
        Transform m_UIRoot = null;
        CanvasScaler m_CanvasScaler = null;
        Camera m_pUICamera = null;
        GameObject m_EventSystem;

        protected Framework.Plugin.AT.AgentTree m_pAgentTree = null;

        List<Transform> m_Roots = new List<Transform>();

        private bool m_bLockDestroy = false;
        private List<DeltaDestroy> m_vDestroying = null;

        private List<IUICallback> m_vCallbacks = null;

        private UISignalTrackSystem m_SignalSlots = null;
        //-------------------------------------------
        public static AUIManager getInstance()
        {
            if (AFramework.mainFramework == null)
                return null;
            return AFramework.mainFramework.uiManager;
        }
        //------------------------------------------------------
        public RectTransform GetDynamicUIRoot(int order = 0)
        {
            if (m_DynamicUIRootRT == null || m_DynamicUIRootRT.Length <= 0) return m_UIRootRT;
            if (order < 0) order = 0;
            if (order >= m_DynamicUIRootRT.Length) order = m_DynamicUIRootRT.Length - 1;
            return m_DynamicUIRootRT[order];
        }
        //------------------------------------------------------
        public RectTransform GetStaticUIRoot() { return m_UIRootRT; }
        //------------------------------------------------------
        public Canvas GetUICanvasRoot() { return m_UIRootCanvas; }
        //------------------------------------------------------
        public CanvasScaler GetUICanvasScalerRoot() { return m_CanvasScaler; }
        //------------------------------------------------------
        public MonoBehaviour GetUICanvasScaler() { return m_CanvasScaler; }
        //------------------------------------------------------
        public Camera GetUICamera() { return m_pUICamera; }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_bInited = false;
            m_vPreInits = new List<UIBase>(4);
            m_vDestroying = new List<DeltaDestroy>(8);
            m_vUIs = new Dictionary<int, UIBase>(16);
            m_vUIFraming = new List<UIBase>(4);
            m_SignalSlots = new UISignalTrackSystem(this);
            m_bLockRemoved = false;
            m_bLockDestroy = false;
            Init(GetFramework().gameStartup);
        }
        //------------------------------------------------------
        void Init(IGame game)
        {
            if (m_bInited) return;
            if (game.GetUISystem() == null)
                return;
            var uiSystemPrefab = game.GetUISystem();
            m_bInited = true;
            //Asset pAsset = FileSystemUtil.LoadAsset(uiSystemPrefab.uiConfig, false, true, true);
            //if (pAsset == null || pAsset.GetOrigin() == null)
            //{
            //    Debug.LogError("请检查"+ uiSystem.uiConfig + ",是否存在!");
            //    return;
            //}
            // m_UIConfig = pAsset.GetOrigin() as UIConfig;

            if (m_UIConfig != null && m_UIConfig.uiAnimators)
                UIAnimatorFactory.getInstance().Init(m_UIConfig.uiAnimators, true);

            GameObject pUIRoot = GameObject.Instantiate(uiSystemPrefab.gameObject);
            pUIRoot.name = "UISystem";
            var uiSystem = pUIRoot.GetComponent<UISystem>();
            m_UIConfig = uiSystem.uiConfig;
            m_pRoot = pUIRoot.transform;
            m_pRoot.position = new Vector3(0, 10000, 0);
            m_pRoot.SetParent(game.GetTransform());
            m_UIRootCanvas = uiSystem.rootCanvas;
            m_UIRootRT = m_UIRootCanvas.transform as RectTransform;
            m_DynamicUIRootRT = uiSystem.dynamicRoots;
            m_UIRoot = uiSystem.staticRoot;
            m_CanvasScaler = uiSystem.canvasScaler;
            if (Screen.width > Screen.height)
            {
                m_CanvasScaler.referenceResolution = new Vector2(1334, 750);
            }
            if (m_DynamicUIRootRT != null)
            {
                for (int i = 0; i < m_DynamicUIRootRT.Length; ++i)
                {
                    m_Roots.Add(m_DynamicUIRootRT[i]);
                }
            }
            m_Roots.Add(m_UIRoot);

            SetUIOffset();

            //GameObject.DontDestroyOnLoad(pUIRoot);

            m_pUICamera = uiSystem.uiCamera;
            if (m_pUICamera == null) m_pUICamera = pUIRoot.GetComponentInChildren<Camera>();

#if USE_URP
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null)
            {
                m_pUICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
                CameraUtil.AddCameraStack(m_pUICamera);
            }
#endif
            m_bLockRemoved = false;
            m_bLockDestroy = false;

            UIBase.OnGlobalAwakeUI += OnAwakeUI;
            UIBase.OnGlobalShowUI += OnShowUI;
            UIBase.OnGlobalHideUI += OnHideUI;
            UIBase.OnGlobalMoveInUI += OnUIShow;
            UIBase.OnGlobalMoveOutUI += OnUIHide;
            UIBase.OnGlobalDestroyUI += OnDestroyUI;

            Font.textureRebuilt += OnFontRebuild;

            for (int i = 0; i < m_vPreInits.Count; ++i)
            {
                if (m_vPreInits[i].GetInstanceAble() != null)
                {
                    m_vPreInits[i].SetParent(m_UIRoot);
                }
            }
            m_vPreInits.Clear();
#if USE_VR
            m_UIRootCanvas.renderMode = RenderMode.WorldSpace;
            m_UIRootCanvas.worldCamera = m_pUICamera;
            m_pUICamera.cameraType = CameraType.VR;
            m_pUICamera.orthographic = false;
            m_pUICamera.fieldOfView = 80;
            m_pUICamera.nearClipPlane = 0.1f;
            m_pUICamera.farClipPlane = 1000.0f;
            m_pUICamera.transform.SetParent(RootsHandler.CameraSystemRoot);
            m_pRoot.transform.position += Vector3.forward * 30;
            m_pRoot.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
#endif

            if (m_CanvasScaler)
            {
                Debug.Log(BaseUtil.stringBuilder.Append("CanvasScaler:").Append(m_CanvasScaler.referenceResolution).ToString());
            }
            Debug.Log(BaseUtil.stringBuilder.Append("实际分辨率:").Append(GetReallyResolution()).ToString());
            Debug.Log(BaseUtil.stringBuilder.Append(Screen.width).Append(",").Append(Screen.height).ToString());
            OnInit(game);
        }
        //------------------------------------------------------
        protected abstract void OnInit(IGame game);
        //------------------------------------------------------
        public void Clear()
        {
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_bLockRemoved = true;
            foreach (var db in m_vUIs)
            {
                if (db.Value.GetPermanent()) continue;
                db.Value.Destroy();
                m_vTemp.Add(db.Key);
            }
            m_bLockRemoved = false;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                m_vUIs.Remove(m_vTemp[i]);
            }
            m_vTemp.Clear();
            if (m_SignalSlots != null) m_SignalSlots.Clear();
            if (ms_vLoading != null) ms_vLoading.Clear();

            m_bDirtyMaps = true;
            //   UIUtil.ResetDynamicMakerRef();
        }
        //------------------------------------------------------
        public void RegisterCallback(IUICallback callback)
        {
            if (m_vCallbacks == null) m_vCallbacks = new List<IUICallback>(2);
            if (m_vCallbacks.Contains(callback)) return;
            m_vCallbacks.Add(callback);
        }
        //------------------------------------------------------
        public void UnRegisterCallback(IUICallback callback)
        {
            if (m_vCallbacks == null) return;
            m_vCallbacks.Remove(callback);
        }
        //------------------------------------------------------
        public string GetPhoneType()
        {
            //Framework.Plugin.Logger.Info("设备型号:" + SystemInfo.deviceModel);
            return SystemInfo.deviceModel;
        }
        //------------------------------------------------------
        public static void SetCanvasBorderOffset(float left, float top, float right, float bottom, float posZ)
        {
            UIAdapter.AdapterLeft = left;
            UIAdapter.AdapterTop = top;
            UIAdapter.AdapterPosZ = posZ;
            UIAdapter.AdapterRight = right;
            UIAdapter.AdapterBottom = bottom;
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            m_bLockRemoved = true;
            foreach (var db in m_vUIs)
            {
                db.Value.Clear();
                db.Value.Destroy();
            }
            m_bLockRemoved = false;
            m_vUIs.Clear();
            m_bDirtyMaps = true;

            m_bLockDestroy = false;
            m_vDestroying.Clear();
            if (m_SignalSlots != null) m_SignalSlots.Clear();
            m_vCallbacks = null;

            UIBase.OnGlobalAwakeUI -= OnAwakeUI;
            UIBase.OnGlobalShowUI -= OnShowUI;
            UIBase.OnGlobalHideUI -= OnHideUI;
            UIBase.OnGlobalDestroyUI -= OnDestroyUI;
            Font.textureRebuilt -= OnFontRebuild;

            if (m_pAgentTree != null)
            {
                m_pAgentTree.Destroy();
                Framework.Plugin.AT.AgentTreeManager.getInstance().UnloadAT(m_pAgentTree);
                m_pAgentTree = null;
            }
        }
        //------------------------------------------------------
        void OnDestroyUI(UIBase ui)
        {
            if (ui == null) return;
            if (!m_bLockRemoved)
            {
                m_vUIs.Remove(ui.GetUIType());
                m_bDirtyMaps = true;
            }
            if (!m_bLockDestroy)
            {
                for (int i = 0; i < m_vDestroying.Count; ++i)
                {
                    if (m_vDestroying[i].uiType == ui.GetUIType())
                    {
                        m_vDestroying.RemoveAt(i);
                        break;
                    }
                }
            }
            if (m_SignalSlots != null) m_SignalSlots.RemoveSignalTrack(ui, m_UIConfig);
            OnUIDestroy(ui);
            ui = null;
        }
        //------------------------------------------------------
        protected virtual void OnUIDestroy(UIBase ui) { }
        //------------------------------------------------------
        void OnAwakeUI(UIBase ui)
        {
            if (ui == null) return;
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnUIAwake(ui);
                }
            }
            OnUIAwake(ui);
        }
        //------------------------------------------------------
        protected virtual void OnUIAwake(UIBase ui) { }
        //------------------------------------------------------
        protected abstract UIBase CreateUIHandle(int type);
        //------------------------------------------------------
        public UIBase AddUI(int type, UserInterface ui, bool bPermanent, int order)
        {
            if (ui == null) return null;

            UIBase pUI = GetUI(type, false);
            if (pUI != null) return pUI;

            pUI = CreateUIHandle(type);
            if (pUI == null) return null;

            if (m_UIRoot != null) pUI.SetParent(m_UIRoot);
            pUI.SetFullUI(true);
            pUI.SetOrder(order);
            pUI.SetTrackAble(false);
            pUI.SetAlwayShow(false);
            pUI.SetPermanent(bPermanent);
            pUI.Hide();

            //             if(bPermanent)
            //                 GameObject.DontDestroyOnLoad(ui.gameObject);
            InstanceOperiaon pInstCB = InstanceOperiaon.Malloc();
            pInstCB.userData0 = new Variable1() { intVal = (int)type };
            pInstCB.pPoolAble = ui.GetComponent<AInstanceAble>();
            CanvasScaler scaler = ui.GetComponent<CanvasScaler>();
            if (scaler)
                GameObject.Destroy(scaler);
            if (pInstCB.pPoolAble == null) pInstCB.pPoolAble = ui.gameObject.AddComponent<AInstanceAble>();
            pUI.OnLoaded(pInstCB);
            pUI.SetZDeepth(0);
            InstanceOperiaon.Free(pInstCB);
            m_vUIs.Add(type, pUI);
            //Debug.Log($"AddUI add {(EUIType)type}");
            m_bDirtyMaps = true;
            if (!m_bInited)
            {
                m_vPreInits.Add(pUI);
            }
            return pUI;
        }
        //------------------------------------------------------
        void OnShowUI(UIBase ui)
        {
            if (ui == null) return;
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnUIShow(ui);
                }
            }
            if (m_pAgentTree != null) m_pAgentTree.ExecuteEvent((ushort)EUIEventType.OnShowUI, ui.GetUIType());

            // backup restore ui
            if (m_SignalSlots != null)
                m_SignalSlots.AddSignalTrack(ui, m_UIConfig);
            OnUIShow(ui);
        }
        //------------------------------------------------------
        protected virtual void OnUIShow(UIBase ui) { }
        //------------------------------------------------------
        void OnHideUI(UIBase ui)
        {
            if (ui == null) return;
            RemoveLoadingInstance(ui.GetUIType());
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnUIHide(ui);
                }
            }
            if (m_pAgentTree != null) m_pAgentTree.ExecuteEvent((ushort)EUIEventType.OnHideUI, ui.GetUIType());

            // close when hide restore ui
            if (m_SignalSlots != null) m_SignalSlots.RemoveSignalTrack(ui, m_UIConfig);
            if (!ui.GetPermanent())
            {
                DeltaDestroy deltaUI;
                for (int i = 0; i < m_vDestroying.Count; ++i)
                {
                    deltaUI = m_vDestroying[i];
                    if (deltaUI.uiType == ui.GetUIType())
                    {
                        deltaUI.lastTime = Time.time;
                        m_vDestroying[i] = deltaUI;
                        return;
                    }
                }
                deltaUI = new DeltaDestroy();
                deltaUI.uiType = ui.GetUIType();
                deltaUI.lastTime = Time.time;
                m_vDestroying.Add(deltaUI);
            }
            OnUIHide(ui);
        }    
        //------------------------------------------------------
        protected virtual void OnUIHide(UIBase ui) { }
        //------------------------------------------------------
        public void PreferParent(Transform transNode)
        {
            if (transNode == null) return;
            RectTransform rect = transNode as RectTransform;

            if (m_UIRoot != null)
                transNode.SetParent(m_UIRoot);
            if (rect == null) return;
            rect.localScale = Vector3.one;
            rect.anchoredPosition3D = Vector2.zero;
            rect.offsetMax = Vector3.zero;
            rect.offsetMin = Vector3.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
        }
        //------------------------------------------------------
        protected override void OnStart()
        {
            base.OnStart();
            m_pAgentTree = Framework.Plugin.AT.AgentTreeManager.getInstance().LoadAT(Data.ATModuleSetting.UIMgrAT);
            if (m_pAgentTree != null)
            {
                m_pAgentTree.Enter();
            }
        }
        //------------------------------------------------------
        void OnFontRebuild(Font font)
        {
            //Framework.Plugin.Logger.Info(font.name + "---------------rebuild");
        }
        //------------------------------------------------------
        public Transform GetRoot()
        {
            return m_UIRoot;
        }
        //------------------------------------------------------
        public static Transform GetAutoUIRoot(int order = 0)
        {
            if (AFramework.mainFramework == null)
                return null;

            var uiMgr = AFramework.mainFramework.uiManager;
            if (uiMgr != null)
            {
                var root = uiMgr.GetDynamicUIRoot(order);
                if (root) return root;
                return uiMgr.GetRoot3D();
            }
            return null;
        }
        //------------------------------------------------------
        public Transform GetRoot3D()
        {
            return m_pRoot;
        }
        //------------------------------------------------------
        public Dictionary<int, UIBase> GetUIS()
        {
            return m_vUIs;
        }
        //------------------------------------------------------
        public bool IsWinShow(int type)
        {
            UIBase pUI = GetUI(type, false);
            if (pUI == null) return false;
            return pUI.IsVisible();
        }

        //------------------------------------------------------
        public void ShowAll()
        {
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
            {
                m_vTemp.Add(db.Key);
            }
            UIBase ui;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                if (m_vUIs.TryGetValue(m_vTemp[i], out ui))
                {
                    ui.Show();
                }
            }
            m_vTemp.Clear();
        }
        //------------------------------------------------------
        public UIBase ShowUI(int type)
        {
            //  Framework.Plugin.Logger.Info("ShowUI type:" + type);
            UIBase pUI = GetUI(type, true);
            if (pUI != null)
            {
                pUI.Show();
            }
            return pUI;
        }
        //------------------------------------------------------
        public T CastShowUI<T>(int type =0) where T : UIBase
        {
            if (type == 0) type = GetTypeToUIType(typeof(T));
            if (type == 0) return null;
            UIBase pUI = ShowUI(type);
            return pUI as T;
        }
        //------------------------------------------------------
        public abstract int GetTypeToUIType(System.Type type);
        //------------------------------------------------------
        public void HideUI(int type)
        {
            UIBase pUI = GetUI(type, false);
            if (pUI != null)
                pUI.Hide();
        }
        //------------------------------------------------------
        public void HideAll()
        {
            HideAllIngores(null);
        }
        //------------------------------------------------------
        public void HideAllIngores(HashSet<int> vIngores = null)
        {
            m_SignalSlots.Clear();
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
            {
                if (vIngores != null && vIngores.Contains(db.Key)) continue;
                if (db.Value.CanHide() && db.Value.IsVisible())
                    m_vTemp.Add(db.Key);
            }
            UIBase ui;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                if (m_vUIs.TryGetValue(m_vTemp[i], out ui))
                {
                    ui.Hide();
                }
            }
            m_vTemp.Clear();
        }
        //------------------------------------------------------
        public void HideAllIngore(int uiType)
        {
            m_SignalSlots.Clear();
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
            {
                if (uiType == db.Key) continue;
                if (db.Value.CanHide() && db.Value.IsVisible())
                    m_vTemp.Add(db.Key);
            }
            UIBase ui;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                if (m_vUIs.TryGetValue(m_vTemp[i], out ui))
                {
                    ui.Hide();
                }
            }
            m_vTemp.Clear();
        }
        //------------------------------------------------------
        public void ShowRoot(bool bShow)
        {
            if (bShow)
            {
                if (m_Roots != null)
                {
                    for (int i = 0; i < m_Roots.Count; ++i)
                    {
                        if (m_Roots[i] != null) m_Roots[i].localScale = Vector3.one;
                    }
                }
            }
            else
            {
                if (m_Roots != null)
                {
                    for (int i = 0; i < m_Roots.Count; ++i)
                    {
                        if (m_Roots[i] != null) m_Roots[i].localScale = Vector3.zero;
                    }
                }
            }
        }
        //------------------------------------------------------
        public void MoveOutside(int type)
        {
            UIBase ui = GetUI(type, false);
            if (ui == null) return;
            ui.MoveOutside();
        }
        //------------------------------------------------------
        public void MoveOutsideAll(int ingoreType = 0)
        {
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
            {
                if (db.Value.IsVisible() && ingoreType != db.Key)
                    m_vTemp.Add(db.Key);
            }
            UIBase ui;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                if (m_vUIs.TryGetValue(m_vTemp[i], out ui))
                {
                    ui.MoveOutside();
                }
            }
            m_vTemp.Clear();
        }
        //------------------------------------------------------
        public void MoveInside(int type)
        {
            UIBase ui = GetUI(type, false);
            if (ui == null) return;
            ui.MoveInside();
        }
        //------------------------------------------------------
        public void MoveInsideAll(int ingoreType =0)
        {
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
            {
                if (db.Value.IsVisible() && ingoreType != db.Key)
                    m_vTemp.Add(db.Key);
            }
            UIBase ui;
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                if (m_vUIs.TryGetValue(m_vTemp[i], out ui))
                {
                    ui.MoveInside();
                }
            }
            m_vTemp.Clear();
        }

        //------------------------------------------------------
        public void CloseUI(int type)
        {
            UIBase pUI = GetUI(type, false);
            if (pUI != null)
            {
                if (pUI.Close())
                {
                    if (!m_bLockRemoved)
                    {
                        m_vUIs.Remove(type);
                        m_bDirtyMaps = true;
                    }
                    if (!m_bLockDestroy)
                    {
                        for (int i = 0; i < m_vDestroying.Count; ++i)
                        {
                            if (m_vDestroying[i].uiType == type)
                            {
                                m_vDestroying.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void CloseAll()
        {
            if (m_vTemp == null) m_vTemp = new List<int>();
            m_vTemp.Clear();
            foreach (var db in m_vUIs)
                m_vTemp.Add(db.Key);
            for (int i = 0; i < m_vTemp.Count; ++i)
            {
                UIBase uiBase;
                if (m_vUIs.TryGetValue(m_vTemp[i], out uiBase) && uiBase.Close())
                    m_vUIs.Remove(m_vTemp[i]);
            }
            m_vTemp.Clear();
            if (m_SignalSlots != null) m_SignalSlots.Clear();
        }
        //------------------------------------------------------
        public UIBase GetUI(int type, bool bAuto = true)
        {
            if (m_vUIs == null) return null;
            UIBase pUI;
            if (m_vUIs.TryGetValue(type, out pUI))
                return pUI;
            if (bAuto)
            {
                return (UIBase)CreateUI(type, false);
            }
            return null;
        }
        //------------------------------------------------------
        public T CastGetUI<T>(bool bAuto = true, int type = 0) where T : UIBase
        {
            if (m_vUIs == null) return null;
            if (type == 0)
            {
                type = GetTypeToUIType(typeof(T));
                if (type == 0) return null;
            }
            UIBase pUI;
            if (m_vUIs.TryGetValue(type, out pUI))
                return pUI as T;
            if (bAuto)
            {
                return CreateUI(type, false) as T;
            }
            return null;
        }
        //------------------------------------------------------
        public UIBase CreateUI(int type, bool bShow = false)
        {
            UIBase pUI = GetUI(type, false);
            if (pUI != null) return pUI;
            if (m_UIConfig == null) return null;
            UIConfig.UI uiData = m_UIConfig.GetUI(type);
            if (uiData == null) return null;

            pUI = CreateUIHandle(type);
            if (pUI == null) return null;

            if (bShow) pUI.Show();
            pUI.SetUIType((int)type);
            pUI.SetParent(m_UIRoot);
            pUI.SetFullUI(uiData.fullUI);
            pUI.SetOrder(uiData.Order);

            pUI.SetAlwayShow(uiData.alwayShow);
            pUI.SetTrackAble(uiData.trackAble);
            pUI.SetBackupAble(uiData.canBackupFlag);
            pUI.SetPermanent(uiData.permanent);
            InstanceOperiaon pInstCB = FileSystemUtil.SpawnInstance(uiData.prefab, false);
            if (pInstCB != null)
            {
                ms_vLoading.Add(type);
                pInstCB.SetLimitCheckCnt(1);
                pInstCB.OnCallback = pUI.OnLoaded;
                pInstCB.OnSign = pUI.OnSign;
                pInstCB.pByParent = m_UIRootRT;
                pInstCB.SetUserData(0, new Variable1() { intVal = (int)type });
                pInstCB.Refresh();
            }
            pUI.SetZDeepth(uiData.uiZValue);
            m_vUIs.Add(type, pUI);
            //Debug.Log($"CreateUI add {(EUIType)type}");
            m_bDirtyMaps = true;

            if (!bShow && !uiData.permanent)
            {
                DeltaDestroy deltaUI = new DeltaDestroy();
                deltaUI.uiType = pUI.GetUIType();
                deltaUI.lastTime = Time.time;
                m_vDestroying.Add(deltaUI);
            }
            return pUI;
        }
        //------------------------------------------------------
        internal static void RemoveLoadingInstance(int uiType)
        {
            ms_vLoading.Remove(uiType);
        }
        //------------------------------------------------------
        public static bool HasLodingInstnaceUI()
        {
            return ms_vLoading.Count > 0;
        }
        //------------------------------------------------------
        public bool IsShow(int type)
        {
            if (type == 0) return false;
            UIBase pUI = GetUI(type, false);
            if (pUI != null)
                return pUI.IsVisible() && pUI.IsMoveOut() == false;
            return false;
        }
        //------------------------------------------------------
        public void SetScreenOrientation(ScreenOrientation orientation)
        {
            if (Screen.orientation == orientation)
                return;
            Screen.orientation = orientation;
            if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
            {
                // int width = Mathf.Min(Screen.width, Screen.height);
                // int height = Mathf.Max(Screen.width, Screen.height);
                //  Screen.SetResolution(width, height, true);

                if (m_CanvasScaler)
                {
                    m_CanvasScaler.referenceResolution = new Vector2(750, 1334);
                    m_CanvasScaler.matchWidthOrHeight = 0;
                }
#if UNITY_EDITOR
                Framework.ED.EditorUtil.SetGameViewTargetSize(750, 1334);
#endif
            }
            else
            {
                // int width = Mathf.Max(Screen.width, Screen.height);
                // int height = Mathf.Min(Screen.width, Screen.height);
                // Screen.SetResolution(width, height, true);

                if (m_CanvasScaler)
                {
                    m_CanvasScaler.referenceResolution = new Vector2(1334, 750);
                    m_CanvasScaler.matchWidthOrHeight = 1;
                }
#if UNITY_EDITOR
                Framework.ED.EditorUtil.SetGameViewTargetSize(1334, 750);
#endif
            }
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            fFrame = Time.deltaTime;
#if UNITY_EDITOR
            if (m_CanvasScaler)
            {
                if (Screen.width > Screen.height)
                {
                    m_CanvasScaler.referenceResolution = new Vector2(1334, 750);
                }
                else
                {
                    m_CanvasScaler.referenceResolution = new Vector2(750, 1334);
                }
            }

#endif
            m_bDirtyMaps = false;
            foreach (var db in m_vUIs)
            {
                db.Value.Update(fFrame);
                if (m_bDirtyMaps)
                    break;
            }
            m_bDirtyMaps = false;

            if (m_vDestroying.Count > 0)
            {
                DeltaDestroy uiDelta;
                float curTime = Time.time;
                for (int i = 0; i < m_vDestroying.Count;)
                {
                    uiDelta = m_vDestroying[i];
                    if (curTime - uiDelta.lastTime >= DELTA_DESTROY_TIME)
                    {
                        UIBase uiBase = GetUI(uiDelta.uiType, false);
                        if (uiBase == null || uiBase.IsVisible())
                        {
                            m_vDestroying.RemoveAt(i);
                            continue;
                        }
                        m_bLockDestroy = true;
                        uiBase.Destroy();
                        m_bLockDestroy = false;
                        m_vDestroying.RemoveAt(i);
                        m_vUIs.Remove(uiDelta.uiType);
                    }
                    else
                        ++i;
                }
            }
        }
        //------------------------------------------------------
        public static void Free()
        {
            if (AFramework.mainFramework == null)
                return;

            var uiMgr = AFramework.mainFramework.uiManager;
            if (uiMgr == null)
                return;

            DeltaDestroy uiDelta;
            float curTime = Time.time;
            for (int i = 0; i < uiMgr.m_vDestroying.Count; ++i)
            {
                uiDelta = uiMgr.m_vDestroying[i];
                {
                    UIBase uiBase = uiMgr.GetUI(uiDelta.uiType, false);
                    if (uiBase != null && !uiBase.IsVisible())
                    {
                        uiMgr.m_bLockDestroy = true;
                        uiBase.Destroy();
                        uiMgr.m_bLockDestroy = false;
                        uiMgr.m_vDestroying.RemoveAt(i);
                        uiMgr.m_vUIs.Remove(uiDelta.uiType);
                    }
                }
            }
            uiMgr.m_vDestroying.Clear();
        }
        //------------------------------------------------------
        public static bool IsInView(Transform trans)
        {
            if (AFramework.mainFramework == null)
                return false;

            var uiMgr = AFramework.mainFramework.uiManager;
            if (uiMgr == null)
                return false;
#if USE_FAIRYGUI
            if (uiMgr.m_pUICamera == null) return false;
            float factor = 0.1f;
            Vector3 viewPos = uiMgr.m_pUICamera.WorldToViewportPoint(trans.position);
            Vector3 dir = (trans.position - m_pRoot.position).normalized;
            float dot = Vector3.Dot(m_pRoot.forward, dir);
            if (dot > 0 && viewPos.x >= -factor && viewPos.x <= 1 + factor && viewPos.y >= -factor && viewPos.y <= 1 + factor)
                return true;
            return false;
#else
            if (!(trans is RectTransform))
                return BaseUtil.IsInView(trans.position);
            //lb,lt,rt,rb
            uiMgr.m_UIRootRT.GetWorldCorners(ms_contersArray);
            (trans as RectTransform).GetWorldCorners(ms_contersArray1);
            Bounds rootBd = new Bounds();
            rootBd.min = new Vector3(ms_contersArray[0].x, ms_contersArray[0].y, 0);//忽略旋转
            rootBd.max = new Vector3(ms_contersArray[2].x, ms_contersArray[2].y, 0);

            Bounds tranBd = new Bounds();
            tranBd.min = new Vector3(ms_contersArray1[0].x, ms_contersArray1[0].y, 0);//忽略旋转
            tranBd.max = new Vector3(ms_contersArray1[2].x, ms_contersArray1[2].y, 0);
            return tranBd.Intersects(rootBd);
#endif
        }
        //------------------------------------------------------
        public static void ScaleWithScreenScale(ref Vector3 scale)
        {
            AUIManager uiMgr = null;
            if (AFramework.mainFramework != null && AFramework.mainFramework.uiManager != null)
                uiMgr = AFramework.mainFramework.uiManager;

            Vector2 screenSize = new Vector2(750, 1334);// new Vector2(Screen.width, Screen.height);
            Vector2 ReferenceResolution = new Vector2(750, 1334);
            if (uiMgr != null)
            {
                screenSize = uiMgr.GetReallyResolution();
                UnityEngine.UI.CanvasScaler canvaScaler = uiMgr.GetUICanvasScalerRoot();
                if (canvaScaler) ReferenceResolution = canvaScaler.referenceResolution;
            }
            scale = Vector3.one * Mathf.Max(ReferenceResolution.x / screenSize.x, ReferenceResolution.y / screenSize.y);
        }
        //------------------------------------------------------
        public Vector3 ConvertUIPosToScreen(Vector3 uiWorldPos)
        {
            return RectTransformUtility.WorldToScreenPoint(m_pUICamera, uiWorldPos);
        }
        //------------------------------------------------------
        public bool ConvertScreenToUIPos(Vector3 screenPos, bool bLocal, ref Vector3 point)
        {
            if (bLocal)
            {
                Vector2 local_temp = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_UIRoot as RectTransform, screenPos, m_pUICamera, out local_temp))
                {
                    point.x = local_temp.x;
                    point.y = local_temp.y;
                    return true;
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_UIRoot as RectTransform, screenPos, m_pUICamera, out point))
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool ConvertWorldPosToUIPos(Vector3 worldPos, bool bLocal, ref Vector3 point, Camera cam = null)
        {
            if (cam == null) cam = CameraUtil.mainCamera;
            if (cam == null) return false;
            Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
            if (bLocal)
            {
                Vector2 local_temp = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_UIRoot as RectTransform, screenPos, m_pUICamera, out local_temp))
                {
                    point.x = local_temp.x;
                    point.y = local_temp.y;
                    return true;
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_UIRoot as RectTransform, screenPos, m_pUICamera, out point))
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public Vector3 UGUIPosToWorldPos(Camera cam, Vector3 uiguiPos, float distance = 0)
        {
            if (cam == null)
            {
                return Vector3.zero;
            }
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(m_pUICamera, uiguiPos);
            Ray ray = cam.ScreenPointToRay(screenPoint);
            return ray.GetPoint(distance <= 0 ? cam.farClipPlane : distance);
            //  return Base.Util.RayHitPos(ray.origin, ray.direction, 0);
        }
        //------------------------------------------------------
        public Vector3 ScreenPosToWorldPos(Vector3 screenPos)
        {
            if (m_pUICamera == null)
            {
                return Vector3.zero;
            }

            return m_pUICamera.ScreenToWorldPoint(screenPos);
        }
        //------------------------------------------------------
        public void SequenceOpenUI(List<ushort> uis)
        {
            if (uis == null || uis.Count < 2)
            {
                Debug.LogError("连续打开界面数据错误");
                return;
            }

            List<Variable1> used = new List<Variable1>();
            for (int i = 0; i < uis.Count; i++)
            {
                if (i != uis.Count - 1)
                {
                    UIBase baseUI = (UIBase)(i == 0 ? ShowUI(uis[i]) : GetUI(uis[i]));
                    Variable1 var1 = new Variable1();
                    var1.intVal = uis[i + 1];
                    used.Add(var1);

                    baseUI.OnCloseUI += (baseui) =>
                    {
                        ShowUI(var1.intVal);
                    };
                }
                else
                {
                    UIBase baseUI = GetUI(uis[i]);
                    List<Variable1> resetList = used;
                    baseUI.OnCloseUI += (baseui) =>
                    {
                        for (int j = 0; j < resetList.Count; j++)
                        {
                            UIBase subui = GetUI(resetList[j].intVal);
                            subui.OnCloseUI = null;
                        }
                    };
                }
            }
        }
        //------------------------------------------------------
        public string GetUIAssetFile(int type)
        {
            if (m_UIConfig == null) return null;
            UIConfig.UI uiData = m_UIConfig.GetUI(type);
            if (uiData == null) return null;
            return uiData.prefab;
        }
        //------------------------------------------------------
        public void PreSpawnUI(List<int> uis, bool bAsync = true, bool bFrontQueue = true)
        {
            if (uis == null) return;
            for (int i = 0; i < uis.Count; ++i)
            {
                if (m_vUIs.ContainsKey(uis[i]))
                    continue;
                UIConfig.UI uiData = m_UIConfig.GetUI(uis[i]);
                if (uiData == null) continue;
                if (FileSystemUtil.GetPreSpawnStats(uiData.prefab) <= 0)
                {
                    FileSystemUtil.PreSpawnInstance(uiData.prefab, bAsync, bFrontQueue);
                }
            }
        }
        //------------------------------------------------------
        public Vector2 GetReallyResolution()
        {
            if (m_UIRootRT == null)
            {
                if (m_CanvasScaler != null)
                {
                    return m_CanvasScaler.referenceResolution;
                }
                return new Vector2(1334, 750);
            }
            //Framework.Plugin.Logger.Info("canvas 分辨率:" + m_UIRootRT.sizeDelta);
            //Framework.Plugin.Logger.Info("手机分辨率:" + Screen.width + "," + Screen.height);
            return m_UIRootRT.sizeDelta;
        }
        //------------------------------------------------------
        public Vector2 GetReallyResolutionRatio()
        {
            if (m_UIRootRT == null)
            {
                return new Vector2(1, 1);
            }
            return new Vector2(m_UIRootRT.sizeDelta.x / 1334.0f, m_UIRootRT.sizeDelta.y / 750.0f);
        }
        //------------------------------------------------------
        public bool IsLandscape()
        {
            var resolution = GetReallyResolution();
            return resolution.x > resolution.y;
        }
        //------------------------------------------------------
        void SetUIOffset()
        {
            //string deviceModel = GetPhoneType();
            //float left = 0, top = 0, right = 0, bottom = 0, posZ = 0;
            ////获取配置表,是否能正确获取到配置表
            ////如果存在id跟这个一致的配置
            ////读取偏移
            //var cfg = Data.DataManager.getInstance().DeviceUIAdapter.GetData(deviceModel);
            //if (cfg != null)
            //{
            //    left = cfg.left;
            //    top = cfg.top;
            //    right = cfg.right;
            //    bottom = cfg.bottom;
            //    posZ = cfg.posZ;
            //}
            ////left = 20;
            //SetCanvasBorderOffset(left, top, right, bottom, posZ);
        }
        //------------------------------------------------------
        public void SetEventSystem(GameObject eventSystem)
        {
            m_EventSystem = eventSystem;
        }
    }
}
