/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIBase
作    者:	HappLI
描    述:	UI操作类
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System;
using Framework.Core;

namespace Framework.UI
{
    public abstract class UIBase  : IUserData
    {
        public static System.Action<UIBase> OnGlobalAwakeUI;
        public static System.Action<UIBase> OnGlobalShowUI;
        public static System.Action<UIBase> OnGlobalHideUI;
        public static System.Action<UIBase> OnGlobalDestroyUI;
        public static System.Action<UIBase> OnGlobalMoveOutUI;
        public static System.Action<UIBase> OnGlobalMoveInUI;

        protected Core.AInstanceAble m_pAble = null;
        protected UserInterface m_UI = null;
        protected Canvas m_Canvas = null;
        protected RectTransform m_RootRect = null;

        protected bool m_bVisible = false;
        protected bool m_bActive = false;
        protected bool m_bPermanent = false;
        protected bool m_bAlwayShow = false;
        private bool m_bDestroyed = false;
        protected bool m_bTackAble = false;
        protected byte m_nBackupAble = 0;

        protected float m_fDelayShow = 0;
        protected float m_fAutoHide = 0;

        private int m_nMoveCount = 0;

        protected bool m_bFullUI = false;
        protected int m_nOrder = 0;
        protected int m_nZValue = 0;
        protected int m_nUIType = 0;

        protected float m_openTime = 0;
        protected bool m_showLog = false;
        protected UIView m_pView = null;
        public System.Action<UIBase> OnCloseUI;
        public System.Action<UIBase> OnHideUI;
        public UIView view
        {
            get { return m_pView; }
        }

        public Dictionary<int, UILogic> m_vLogic;

        public UISerialized ui { get { return m_UI; } }
        protected Framework.Plugin.AT.AgentTree m_pAT = null;
        public Framework.Plugin.AT.AgentTree uiAT
        {
            get { return m_pAT; }
        }

        Transform m_pParent = null;
        public System.Action m_pUIPendingAction;

        private Vector3 m_vMoveBeforePos;

        private Dictionary<string, IUserData> m_vUserDatas = null;
        //------------------------------------------------------
        public virtual void Awake()
        {
            m_bDestroyed = false;
            OnAwake();
            if (m_pView != null) m_pView.Awake();
            if(m_vLogic!=null)
            {
                foreach (var db in m_vLogic)
                    db.Value.Awake(this);
            }

            if (m_UI != null)
            {
                UserInterface ui = m_UI as UserInterface;
                if (ui)
                    m_pAT = Framework.Plugin.AT.AgentTreeManager.getInstance().LoadAT(ui.ATData);
                if (m_pAT != null)
                {
                    m_pAT.AddOwnerClass(this);
                    if (m_pView != null) m_pAT.AddOwnerClass(m_pView);
                    if (m_vLogic != null)
                    {
                        foreach (var db in m_vLogic)
                            m_pAT.AddOwnerClass(db.Value);
                    }
                    if (m_bVisible)
                    {
                        m_pAT.Enable(true);
                        m_pAT.Enter();
                    }
                }
            }

            if (OnGlobalAwakeUI != null) OnGlobalAwakeUI(this);

            if(m_bVisible)
            {
                OnFrameShow();
            }
        }
        //------------------------------------------------------
        protected virtual void OnAwake() { }
        //------------------------------------------------------
        internal int GeOrder()
        {
            return m_nOrder;
        }
        //------------------------------------------------------
        public AFramework GetFramework()
        {
            if (AUIManager.getInstance() == null)
                return null;
            return AUIManager.getInstance().GetFramework();
        }
        //------------------------------------------------------
        public GameObject GetRoot()
        {
            if (m_Canvas == null || m_Canvas.gameObject == null) return null;
            return m_Canvas.gameObject;
        }
        //------------------------------------------------------
        public void SetUIType(int uiType)
        {
            m_nUIType = uiType;
        }
        //------------------------------------------------------
        public int GetUIType()
        {
            return m_nUIType;
        }
        //------------------------------------------------------
        public int GetOrder()
        {
            return m_nOrder;
        }
        //------------------------------------------------------
        public void SetOrder(int order)
        {
            m_nOrder = order;
            if(m_Canvas != null)
            {
                m_Canvas.overrideSorting = true;
                if (m_Canvas.sortingOrder != order)
                {
                    m_Canvas.sortingOrder = order;
                }
            }
        }
        //------------------------------------------------------
        public void SetZDeepth(int zValue)
        {
            m_nZValue = zValue;
            if (m_Canvas != null&& zValue != 0)
            {
                Vector3 pos = m_RootRect.anchoredPosition3D;
                m_RootRect.anchoredPosition3D = new Vector3(pos.x,pos.y,zValue);
            }
        }
        //------------------------------------------------------
        public void SetFullUI(bool bFull)
        {
            m_bFullUI = bFull;
        }
        //------------------------------------------------------
        public void SetPermanent(bool bPermanent)
        {
            m_bPermanent = bPermanent;
        }
        
        //------------------------------------------------------
        public void SetAlwayShow(bool bAlwayShow)
        {
            m_bAlwayShow = bAlwayShow;
        }
        //------------------------------------------------------
        public bool GetAlwayShow()
        {
            return m_bAlwayShow;
        }
        //------------------------------------------------------
        public bool GetPermanent()
        {
            return m_bPermanent;
        }
        //------------------------------------------------------
        public void SetParent(Transform pTrans)
        {
            if(m_pParent != pTrans)
            {
                if (m_pAble != null)
                {
                    m_pParent = pTrans;
                    RefreshInstanceAble();
                }
                else
                    m_pParent = pTrans;
            }
        }
        //------------------------------------------------------
        public void SetView(UIView pView)
        {
            m_pView = pView;
            if (m_pView != null)
                m_pView.Init(this);
        }

        //------------------------------------------------------
        public void ExecuteAT(EUIEventType type, int nCustomID, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteEvent((ushort)type, nCustomID, paramData);
        }
        //------------------------------------------------------
        public void ExecuteAT(EUIEventType type, string strCustom, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteEvent((ushort)type, strCustom, paramData);
        }
        //------------------------------------------------------
        public void ExecuteAT(EUIEventType type, GameObject pObj, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteEvent((ushort)type, pObj, paramData);
        }
        //------------------------------------------------------
        public void ExecuteCustom(GameObject pGo, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteCustom(pGo, paramData);
        }
        //------------------------------------------------------
        public void ExecuteCustom(string strName, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteCustom(strName, paramData);
        }
        //------------------------------------------------------
        public void ExecuteCustom(int nID, IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteCustom(nID, paramData);
        }
        //------------------------------------------------------
        public void ExecuteEvent(EUIEventType enterType,IUserData paramData = null)
        {
            if (m_pAT == null) return;
            m_pAT.ExecuteCustom((ushort)enterType, paramData);
        }
        //------------------------------------------------------
        public UILogic FindLogic(System.Type type)
        {
            if (m_vLogic == null) return null;
            if (type == null) return null;
            int hashCode = type.GetHashCode();

            UILogic logic = null;
            if (m_vLogic.TryGetValue(hashCode, out logic))
                return logic;
            return null;
        }
        //------------------------------------------------------
        public T GetLogic<T>(bool bAutoNew = false) where T : UILogic, new()
        {
            int hashCode = typeof(T).GetHashCode();
            UILogic logic = null;
            if (m_vLogic!=null && m_vLogic.TryGetValue(hashCode, out logic))
                return logic as T;
            if(bAutoNew)
                return AddLogic<T>(hashCode);
            return null;
        }
        //------------------------------------------------------
        public void AddLogic(UILogic pLogic, int hashCode = 0)
        {
            if (hashCode == 0) hashCode = pLogic.GetType().GetHashCode();
            if (m_vLogic == null) m_vLogic = new Dictionary<int, UILogic>(4);
            if (m_vLogic.ContainsKey(hashCode))
                return;

            pLogic.OnInit(this);
            pLogic.Active(true);
            m_vLogic[hashCode] = pLogic;
        }
        //------------------------------------------------------
        public T AddLogic<T>(int hashCode = 0) where T : UILogic, new()
        {
            if(hashCode == 0) hashCode = typeof(T).GetHashCode();
            if (m_vLogic == null) m_vLogic = new Dictionary<int, UILogic>(4);
            UILogic logic = null;
            if (m_vLogic.TryGetValue(hashCode, out logic))
                return (T)logic;

            T newT = new T();
            newT.OnInit(this);
            newT.Active(true);
            if (m_pAble != null) newT.Awake(this);
            if (IsVisible())
            {
                newT.OnShow();
            }
            m_vLogic[hashCode] = newT;
            return newT;
        }
        //------------------------------------------------------
        public void RemoveLogic(UILogic pLogic, int hashCode = 0)
        {
            if (m_vLogic == null) return;
            if (hashCode == 0) hashCode = pLogic.GetType().GetHashCode();
            m_vLogic.Remove(hashCode);
            pLogic.Destroy();
        }
        //------------------------------------------------------
        public void RemoveLogic<T>(int hashCode = 0) where T : UILogic
        {
            if (m_vLogic == null) return;
            if(hashCode ==0 ) hashCode = typeof(T).GetHashCode();
            UILogic logic = null;
            if(m_vLogic.TryGetValue(hashCode, out logic))
            {
                logic.Destroy();
                m_vLogic.Remove(hashCode);
            }
        }
        //------------------------------------------------------
        public void Show(float fDelta)
        {
            if (m_bVisible) return;
            m_fDelayShow = 0;
            if (fDelta <=0)
            {
                Show();
                return;
            }
            m_bVisible = true;
            m_fDelayShow = fDelta;
        }
        //------------------------------------------------------
        void OnFrameShow()
        {
            if (!m_bVisible) return;
            m_fDelayShow = 0;
            //Debug.Log("显示UI:" + (EUIType)m_nUIType);
            if (m_bVisible && m_pAble)
            {
                m_pAble.SetPosition(Vector3.zero, true);
            }
            if (m_pAT != null)
            {
                m_pAT.Enable(true);
                m_pAT.Enter();
            }
            SetZDeepth(m_nZValue);
            if (m_pView != null) m_pView.Show();
            DoShow();

            if (m_UI != null && m_pAble)
            {
                if(m_UI.OnShow())
                {
                    return;
                }
            }
            DoShowEnd();
        }
        //------------------------------------------------------
        public void Show()
        {
            if (m_bVisible) return;
            m_bVisible = true;

            if (m_pAble == null) return;
            OnFrameShow();
            m_openTime = Time.time;
        }
        //------------------------------------------------------
        protected virtual void DoShow()
        {
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                {
                    db.Value.OnShow();
                }
            }
        }
        //------------------------------------------------------
        protected void DoShowEnd()
        {
            if (m_bFullUI)
                CameraUtil.CloseCameraRef(true);
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                    db.Value.OnShowEnd();
            }

            if (OnGlobalShowUI != null) OnGlobalShowUI(this);
            OnShow();
        }
        //------------------------------------------------------
        protected virtual void OnShow()
        {
        }
        //------------------------------------------------------
        protected virtual void OnMoveInside()
        {

        }
        //------------------------------------------------------
        protected virtual void OnMoveOutside()
        {

        }
        //------------------------------------------------------
        public bool Close()
        {
            if (m_bPermanent)
            {
                Hide();
                return false;
            }
            Hide();
            UIEventLogic eventLogic = GetLogic<UIEventLogic>();
            if (eventLogic != null && eventLogic.IsEvnting())
            {
                m_bDestroyed = true;
                return true;
                //DoHide();
            }
            if(AFramework.mainFramework!=null) AFramework.mainFramework.UnRegisterFunction(this);
            DoClose();
            return true;
        }
        //------------------------------------------------------
        protected void DoClose()
        {
            Destroy();
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                    db.Value.OnClose();
            }
            OnClose();
        }
        //------------------------------------------------------
        protected virtual void OnClose()
        {
        }
        //------------------------------------------------------
        public void MoveOutside()
        {
            if (m_bAlwayShow)
            {
                return;
            }
            if (m_nMoveCount == 0) m_vMoveBeforePos = m_pAble.transform.localPosition;
            if (m_nMoveCount == 0 && m_pAble)
            {
                if (OnGlobalMoveOutUI != null) OnGlobalMoveOutUI(this);
                bool bTween = false;
                if (m_UI != null)
                {
                    bTween = m_UI.OnMoveOut();
                }
                if(!bTween) m_pAble.SetPosition(Vector3.one * 8888, true);

                if (m_bFullUI)
                    CameraUtil.CloseCameraRef(false);
                OnMoveOutside();
            }
            m_nMoveCount++;
        }
        //------------------------------------------------------
        public void MoveInside()
        {
            if (m_bAlwayShow)
            {
                return;
            }
            m_nMoveCount--;
            if (m_nMoveCount == 0 && m_pAble)
            {
                if (OnGlobalMoveInUI != null) OnGlobalMoveInUI(this);
                m_pAble.SetPosition(m_vMoveBeforePos, true);
                bool bTween = false;
                if (m_UI != null)
                {
                    bTween = m_UI.OnMoveIn();
                }

             //   if (m_bFullUI)
             //       GameInstance.getInstance().cameraController.CloseCameraRef(true);

                OnMoveInside();
            }
        }
        //------------------------------------------------------
        public bool CanTrack()
        {
            return m_bTackAble;
        }
        //------------------------------------------------------
        public void SetTrackAble(bool able)
        {
            m_bTackAble = able;
        }
        //------------------------------------------------------
        public byte GetBackupFlags()
        {
            return m_nBackupAble;
        }
        //------------------------------------------------------
        public bool CanBackup()
        {
            return (m_nBackupAble&(int)EUIBackupFlag.Toggle)!=0;
        }
        //------------------------------------------------------
        public bool IsMoveOutSideBackup()
        {
            return (m_nBackupAble & (int)EUIBackupFlag.MoveOutside) != 0;
        }
        //------------------------------------------------------
        public bool IsBackupAllShowed()
        {
            return (m_nBackupAble & (int)EUIBackupFlag.BackupAllShow) != 0;
        }
        //------------------------------------------------------
        public bool CanInheritCommonBackup()
        {
            return (m_nBackupAble & (int)EUIBackupFlag.InheritCommon) != 0;
        }
        //------------------------------------------------------
        public void SetBackupAble(byte able)
        {
            m_nBackupAble = able;
        }
        //------------------------------------------------------
        public virtual bool CanHide()
        {
            return true;
        }
        //------------------------------------------------------
        public void Hide()
        {
            m_fDelayShow = 0;
            m_fAutoHide = 0;
            if (m_bAlwayShow) return;
            if (!m_bVisible) return;
            m_bVisible = false;
            m_nMoveCount = 0;
            if (OnGlobalHideUI != null) OnGlobalHideUI(this);
            if (m_UI != null && m_pAble)
            {
                if(m_UI.OnHide())
                {
                    return;
                }
            }
            AFramework.mainFramework?.UnRegisterFunction(this);
            DoHide();
        }
        //------------------------------------------------------
        public void AutoHide(float fDelay)
        {
            m_fAutoHide = fDelay;
        }
        //------------------------------------------------------
        protected virtual void DoHide()
        {
            bool bHideEvent = false;

            if (!bHideEvent)
            {
                if (m_pAble) m_pAble.SetPosition(Vector3.one * 9999, true);
            }
            if (m_pView != null) m_pView.Hide();
            if (m_pView != null)
                m_pView.Clear(m_bPermanent);

            OnHideUI?.Invoke(this);
            OnHideUI = null;
            if (m_bFullUI)
                CameraUtil.CloseCameraRef(false);

            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                {
                    AFramework.mainFramework?.UnRegisterFunction(db.Value);
                    db.Value.OnHide();
                }
            }

            if (m_pAT != null)
            {
                m_pAT.Exit();
                if (m_pAT != null) m_pAT.Enable(false);
            }
            if (m_vUserDatas != null) m_vUserDatas.Clear();
            OnHide();
        }
        //------------------------------------------------------
        protected virtual void OnHide()
        {

        }
        //------------------------------------------------------
        public bool IsVisible()
        {
            return m_bVisible;
        }
        //------------------------------------------------------
        public bool IsMoveOut()
        {
            return m_nMoveCount > 0;
        }
        //------------------------------------------------------
        public bool CanBreakGuide()
        {
            return true;
        }
        //------------------------------------------------------
        public T FindOject<T>(string strName) where T : UnityEngine.Object
        {
            if (m_UI == null) return null;
            return m_UI.GetRefObject<T>(strName);
        }
        //------------------------------------------------------
        public void Destroy()
        {
         //   if (m_bFullUI && m_bVisible)
        //        Core.CameraKit.CloseCameraRef(false);
            Clear();
            OnDestroy();
            if (m_pView != null)
            {
                m_pView.Destroy();
            }
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                    db.Value.OnDestroy();
                m_vLogic.Clear();
            }
            if (m_pAble != null)
                m_pAble.RecyleDestroy(1);
            m_pAble = null;
            m_UI = null;
            m_bDestroyed = true;
            m_pParent = null;
            m_pView = null;

            if (m_pAT != null)
                Framework.Plugin.AT.AgentTreeManager.getInstance().UnloadAT(m_pAT);
            m_pAT = null;

            OnCloseUI?.Invoke(this);
            OnCloseUI = null;
            if (OnGlobalDestroyUI != null) OnGlobalDestroyUI(this);
            m_bVisible = false;
            AFramework.mainFramework?.UnRegisterFunction(this);
        }
        //------------------------------------------------------
        protected virtual void OnDestroy() { }
        //------------------------------------------------------
        public void Clear()
        {
            m_fDelayShow = 0;
            m_fAutoHide = 0;
            if (m_pView != null) m_pView.Clear();
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                    db.Value.OnClear();
            }
            if (m_vUserDatas != null) m_vUserDatas.Clear();
        }
        //------------------------------------------------------
        public bool IsInstanced()
        {
            return m_pAble != null;
        }
        //------------------------------------------------------
        public Core.AInstanceAble GetInstanceAble()
        {
            return m_pAble;
        }
        //------------------------------------------------------
        void SetInstanceAble(Core.AInstanceAble pAble)
        {
            m_pAble = pAble;
            if (m_pAble != null)
            {
                m_UI = m_pAble.GetComponent<UserInterface>();
                if (m_UI is UserInterface)
                {
                    UIEventLogic logic = new UIEventLogic();
                    m_UI.SetEventLogic(logic);
                    AddLogic(logic);
                }

                RefreshInstanceAble();
            }
            m_bActive = m_pAble != null;
            Awake();
        }
        //------------------------------------------------------
        void RefreshInstanceAble()
        {
            if (m_pAble == null) return;
            if(m_Canvas == null)
                m_Canvas = m_pAble.GetComponent<Canvas>();
            if (m_Canvas )
            {
                if (m_RootRect == null)
                    m_RootRect = m_Canvas.GetComponent<RectTransform>();
            }
            RectTransform rect = m_pAble.GetTransorm() as RectTransform;

            if (m_pParent != null)
                m_pAble.SetParent(m_pParent);
            if (rect == null) return;
            rect.localScale = Vector3.one;
            rect.anchoredPosition3D = Vector2.zero;
            rect.offsetMax = Vector3.zero;
            rect.offsetMin = Vector3.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            if (m_bVisible)
            {
                m_pAble.SetPosition(Vector3.zero, true);
            }
            else
                m_pAble.SetPosition(Base.ConstDef.INVAILD_POS, true);

            SetOrder(m_nOrder);
        }
        //------------------------------------------------------
        public void OnEventHandle(IUserData param)
        {
            if(param !=null && param is Variable1)
            {
                UIEventType type = (UIEventType)((Variable1)param).intVal;
                switch(type)
                {
                    case UIEventType.Hide:
                        {
                            DoHide();
                            if (m_bDestroyed) DoClose();
                        }
                        break;
                    case UIEventType.Show:
                        {
                            DoShowEnd();
                        }
                        break;
                    case UIEventType.MoveOut:
                        {
                            m_pAble.SetPosition(Vector3.one * 8888, true);
                        }
                        break;
                    case UIEventType.MoveIn:
                        {
                            m_pAble.SetPosition(m_vMoveBeforePos, true);
                        }
                        break;
                }
            }

        }
        //------------------------------------------------------
        public void OnLoaded(Core.InstanceOperiaon pOp)
        {
            AUIManager.RemoveLoadingInstance(m_nUIType);
            if (pOp == null) return;
            m_nUIType = pOp.GetUserData<Variable1>(0).intVal;
            SetInstanceAble(pOp.GetAble());
        }
        //------------------------------------------------------
        public void OnSign(Core.InstanceOperiaon pOp)
        {
            pOp.SetUsed(!m_bDestroyed);
            if (!pOp.IsUsed()) AUIManager.RemoveLoadingInstance(m_nUIType);
        }
        //------------------------------------------------------
        public void Update(float fFrame)
        {
            if (!m_bActive || !m_bVisible) return;

            if (m_fDelayShow > 0)
            {
                if (m_pAble)
                {
                    m_fDelayShow -= Time.deltaTime;
                    if (m_fDelayShow <= 0)
                    {
                        m_bVisible = false;
                        Show();
                    }
                }
                return;
            }
            if(m_bVisible)
            {
                if(m_fAutoHide>0)
                {
                    m_fAutoHide -= Time.deltaTime;
                    if (m_fAutoHide<=0)
                    {
                        Hide();
                    }
                }
            }

            InnerUpdate(fFrame);
            if (m_pView != null) m_pView.Update(fFrame);
            if (m_vLogic != null)
            {
                foreach (var db in m_vLogic)
                    db.Value.Update(fFrame);
            }
        }
        //------------------------------------------------------
        protected virtual void InnerUpdate(float fFrame)
        {

        }
        //------------------------------------------------------
        public Core.AssetOperiaon LoadObjectAsset(UnityEngine.Object pObj, string strPath, bool bPermanent = false, bool bAysnc = false, Sprite defaultSprite = null)
        {
            if (m_pView == null) return null;
            return m_pView.LoadObjectAsset(pObj, strPath, bPermanent, bAysnc, defaultSprite);
        }
        //------------------------------------------------------
        public InstanceOperiaon LoadInstance(string strFile, Transform pParent, bool bAsync = true, Action<InstanceOperiaon> OnCallback = null, IUserData userPtr = null)
        {
            if (m_pView == null) return null;
            return m_pView.LoadInstance(strFile, pParent, bAsync, OnCallback, userPtr);
        }
        //------------------------------------------------------
        public InstanceOperiaon LoadInstance(GameObject prefab, Transform pParent, bool bAsync = true, Action<InstanceOperiaon> OnCallback = null, IUserData userPtr = null)
        {
            if (m_pView == null) return null;
            return m_pView.LoadInstance(prefab, pParent, bAsync, OnCallback, userPtr);
        }
        //------------------------------------------------------
        public T GetWidget<T>(string name) where T : Component
        {
            if (m_UI == null) return null;
            return m_UI.GetWidget<T>(name);
        }
        //------------------------------------------------------
        public void SetShowLog(bool isShow)
        {
            m_showLog = isShow;
        }
        //------------------------------------------------------
        public void AddUserParam(string key, IUserData param)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (m_vUserDatas == null) m_vUserDatas = new Dictionary<string, IUserData>(2);
            m_vUserDatas[key.ToLower()] = param;
        }
        //------------------------------------------------------
        public void AddUserIntParam(string key, int param)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (m_vUserDatas == null) m_vUserDatas = new Dictionary<string, IUserData>(2);
            m_vUserDatas[key.ToLower()] = new Variable1() { intVal = param };
        }
        //------------------------------------------------------
        public void AddUserStrParam(string key, string param)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (m_vUserDatas == null) m_vUserDatas = new Dictionary<string, IUserData>(2);
            m_vUserDatas[key.ToLower()] = new VariableString() { strValue = param };
        }
        //------------------------------------------------------
        public IUserData GetUserParam(string key)
        {
            if (string.IsNullOrEmpty(key) || m_vUserDatas == null) return null;
            IUserData result;
            if (m_vUserDatas.TryGetValue(key.ToLower(), out result)) return result;
            return null;
        }
        //------------------------------------------------------
        public int GetUserIntParam(string key)
        {
            if (string.IsNullOrEmpty(key) || m_vUserDatas == null) return 0;
            IUserData result;
            if (m_vUserDatas.TryGetValue(key.ToLower(), out result))
            {
                if(result is Variable1)
                    return ((Variable1)result).intVal;
                else if(result is VariableString)
                {
                    int temp;
                    if (int.TryParse(((VariableString)result).strValue, out temp))
                        return temp;
                }
            }
            return 0;
        }
        //------------------------------------------------------
        public string GetUserStrParam(string key)
        {
            if (string.IsNullOrEmpty(key) || m_vUserDatas == null) return null;
            IUserData result;
            if (m_vUserDatas.TryGetValue(key.ToLower(), out result) && result is VariableString) return ((VariableString)result).strValue;
            return null;
        }
    }
}
