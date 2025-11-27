/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuidePanel
作    者:	
描    述:	引导界面面板
*********************************************************************/
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Guide
{
    public class GuidePanel
    {
        public static Vector3 INVAILD_POS = new Vector3(-9000, -9000, -9000);
        static Vector3[] ms_contersArray = new Vector3[4];
        Vector3[] ms_contersArray1 = new Vector3[4];
        int m_nDefaultOrder = 0;
        UIGuideSerialize m_Serialize = null;



        private bool m_bVisible = false;
        private bool m_bSlideFinger = false;
#if UNITY_EDITOR
        Vector4 m_SlidePosParam = Vector4.zero;
        Transform m_pClickZoomEditor = null;
#endif
        private int m_SlideStartWidgetGuid = 0;
        private Transform m_SlideStartWidget = null;
        private Vector3 m_SlideStarPos = Vector3.zero;
        private Vector3 m_SlideEndPos = Vector3.zero;
        private Transform m_pSlideFinger = null;
        private Vector3 m_SlideParam = Vector3.zero;
        private Vector3 m_SlideRuning = Vector3.zero;

        private EDescBGType m_TipsType = EDescBGType.None;
        private int m_nTipsDockWidgetGUID = 0;
        private string m_nTipsDockWidgetTag = null;
        private int m_nTipsDockListIndex = -1;
        private Transform m_pTipDockWidget = null;
        //这边的偏移是屏幕坐标的偏移,不是世界坐标的偏移
        private Vector3 m_TipDockOffset = Vector3.zero;
        private bool m_bTipsDockPosUse3D = false;

        public Vector3 SlideStartPos { get { return m_SlideStarPos; } }
        public Vector3 SlideEndPos { get { return m_SlideEndPos; } }

        private bool m_bTopCloneWidget = false;
        private Transform m_pGuideWidget;
        private EventTriggerListener m_pGuideTriggerListen;
        private Transform m_pOriGuideWidget;
        private bool m_bListenGuideWidget = false;
        private int m_ListenGuideGuid;
        string m_ListenGuideGuidTag = null;
        private bool m_bConvertUIPos = false;
        private bool m_bRayTest = false;
        private bool m_bMaskSelfWidget = false;
        private int m_nListIndex = -1;
        private int m_nListenLastFrame = 0;
        private Vector3 m_FingerOffset = Vector3.zero;
        private EFingerType m_fingerType = EFingerType.None;
        private bool m_bClickZoom = false;
        private Vector3 m_ClickZoomPosition = Vector3.zero;
        private Vector3 m_ClickAngle = Vector3.zero;
        private bool m_bClick3DPosition = false;

        string m_SearchListenName;
        string m_TipsSearchListenName;

        //    Framework.RtgTween.PositionTween m_tween;

        private bool m_bDoing = false;
        public bool bDoing
        {
            get { return m_bDoing; }
            set
            {
                m_bDoing = value;
            }
        }
#if UNITY_EDITOR
        public bool IsEditorPreview = false;
#endif

        private bool m_isShowingDialog = false;
        private Coroutine m_DialogCoroutine = null;
        private string m_DialogContent = null;
        public bool IsShowingDialog
        {
            get { return m_isShowingDialog; }
        }

        private EventTriggerListener m_SimulationClickListener = null;
        public EventTriggerListener SimulationClickListener
        {
            get
            {
                if (m_SimulationClickListener == null && m_Serialize && m_Serialize.SimulationClickImage)
                {
                    m_SimulationClickListener = m_Serialize.SimulationClickImage.GetComponent<EventTriggerListener>();
                }
                return m_SimulationClickListener;
            }
        }

        /// <summary>
        /// 对话框背景高度最低值
        /// </summary>
        private int m_DescBgMinHeight = 200;

        Dictionary<GameObject, Transform> m_MoveUIDic = new Dictionary<GameObject, Transform>();

        Regex m_Regex = new Regex("(<).+?(>)", RegexOptions.Singleline);//匹配字符中所有<>

        StepNode m_CurStepNode;

        CanvasScaler m_CanvasScaler = null;
        RectTransform m_pRootUI;
        Canvas m_pRootUICanvas;
        Camera m_pUICamera;
        Dictionary<int, WaitForSeconds> m_WaiteSendond = null;

        PointerEventData m_pTestEventData = null;
        List<RaycastResult> m_RayTestResults = null;
        //-------------------------------------------
#if UNITY_EDITOR
        internal Transform GetClickZoomEditorImage()
        {
            if (m_Serialize == null)
                return null;
            if(m_pClickZoomEditor == null)
            {
                GameObject pImage = new GameObject();
                pImage.hideFlags |= HideFlags.HideAndDontSave;
                pImage.transform.SetParent(m_Serialize.transform);
                m_pClickZoomEditor = pImage.transform;
            }
            return m_pClickZoomEditor;
        }
#endif
        //-------------------------------------------
        public void Awake(Canvas pUIRoot, Camera uiCamera, UIGuideSerialize serialzie)
        {
            m_Serialize = serialzie;
            m_pRootUICanvas = pUIRoot;
            m_CanvasScaler = m_pRootUICanvas.GetComponent<CanvasScaler>();
            m_pRootUI = pUIRoot.transform as RectTransform;
            m_pUICamera = uiCamera;
            if (serialzie.Canvas) m_nDefaultOrder = serialzie.Canvas.sortingOrder;
            else m_nDefaultOrder = 30000;
                ResetData();
        }
        //-------------------------------------------
        public void Show()
        {
            bool bVisible = m_bVisible;
            m_bVisible = true;
            if(bVisible != m_bVisible)
                ResetData();
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetRootCavas(m_pRootUICanvas, m_pUICamera);
            }
            if (m_Serialize) m_Serialize.gameObject.SetActive(m_bVisible);
        }
        //-------------------------------------------
        public void Hide()
        {
            m_bVisible = false;
            ResetData();
            SetDefaultOrder();
            bDoing = false;
            if(m_Serialize) m_Serialize.gameObject.SetActive(m_bVisible);
        }
        //-------------------------------------------
        public bool IsVisible()
        {
            return m_bVisible;
        }
        //-------------------------------------------
        public void ClearData()
        {
            ResetData();
            bDoing = false;
        }
        //-------------------------------------------
        public void ResetData()
        {
            //Framework.Plugin.Logger.Warning("Guide ResetData");
            m_SlideStartWidget = null;
            m_SlideStartWidgetGuid = 0;
            m_bSlideFinger = false;
            m_pSlideFinger = null;
            m_SlideParam = Vector3.zero;
            m_SlideRuning = Vector3.zero;
            if (m_Serialize == null) return;
            if (m_Serialize.Fingers != null)
            {
                for (int i = 0; i < m_Serialize.Fingers.Length; ++i)
                {
                    if (m_Serialize.Fingers[i])
                    {
                        m_Serialize.Fingers[i].gameObject.SetActive(false);
                        m_Serialize.Fingers[i].rotation = Quaternion.identity;
                    }
                }
            }


            if (m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetSpeed(0);
                m_Serialize.BgMask.SetShapeScale(Vector2.one);
                m_Serialize.BgMask.EnablePenetrate(false, 0);
                m_Serialize.BgMask.SetTarget(null);
                m_Serialize.BgMask.SetShape(GuideHighlightMask.EShape.None);
                m_Serialize.BgMask.SetClick(true);
                m_Serialize.BgMask.SetMaskZoomPenetrate(Vector3.zero, 0, false);
                m_Serialize.BgMask.gameObject.SetActive(false);
            }

            if (m_Serialize.TargetContainer)
            {
                m_Serialize.TargetContainer.DetachChildren();
            }

            if (m_Serialize.DescBgs != null)
            {
                for (int i = 0; i < m_Serialize.DescBgs.Length; ++i)
                {
                    if (m_Serialize.DescBgs[i]) m_Serialize.DescBgs[i].gameObject.SetActive(false);
                }
            }

            ClearWidget();
            ClearTipDock();

            m_isShowingDialog = false;
            m_DialogCoroutine = null;
            m_DialogContent = null;

            ResetImage();
            CleatDialogArrow();

            ClearMoveUI();

        //    if (m_tween != null)
        //    {
       //         m_tween.Stop();
        //    }

            ResetGuideText();
            ResetContinueImage();

            if (m_Serialize.SkipBtn)
            {
                m_Serialize.SkipBtn.gameObject.SetActive(false);
            }

            if (m_RayTestResults != null) m_RayTestResults.Clear();
        }
        //------------------------------------------------------
        void ClearMoveUI()
        {
            if (m_MoveUIDic == null || m_MoveUIDic.Count == 0)
            {
                return;
            }
            foreach (var item in m_MoveUIDic.Keys)
            {
                if (item == null)
                {
                    continue;
                }
                item.transform.SetParent(m_MoveUIDic[item], true);
            }
            m_MoveUIDic.Clear();
        }
        //------------------------------------------------------
        public void CleatDialogArrow()
        {
            if (DialogArrow == null)
            {
                return;
            }

            DialogArrow.anchoredPosition = new Vector2(0, -5.7f);
            DialogArrow.rotation = Quaternion.identity;
            DialogArrow.localScale = Vector3.one;
            SetDialogArrowEnable(true);
        }
        //------------------------------------------------------
        public Transform GetListenGuideOriWiget()
        {
            return m_pOriGuideWidget;
        }
        //------------------------------------------------------
        public void ClearWidget()
        {
            if(m_bTopCloneWidget)
            {
                if(m_pGuideWidget)
                {
                    GameObject.Destroy(m_pGuideWidget.gameObject);
                }
                if (m_pOriGuideWidget)
                    m_pOriGuideWidget.gameObject.SetActive(true);
            }
            m_bTopCloneWidget = false;
            m_pOriGuideWidget = null;
            m_pGuideWidget = null;
            m_pGuideTriggerListen = null;
            m_ListenGuideGuid = 0;
            m_ListenGuideGuidTag = null;
            m_bListenGuideWidget = false;
            m_bConvertUIPos = false;
            m_bMaskSelfWidget = false;
            m_bRayTest = false;
            m_nListIndex = -1;
            m_nListenLastFrame = 0;
            m_fingerType = EFingerType.None;
            m_FingerOffset = Vector3.zero;
            m_SearchListenName = null;
            SetSimulationClickImageActive(false);
            m_CurStepNode = null;
            m_bClickZoom = false;
            m_ClickZoomPosition = Vector3.zero;
            m_ClickAngle = Vector3.zero;
            m_bClick3DPosition = false;
        }
        //------------------------------------------------------
        public Transform TargetContainer
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.TargetContainer;
            }
        }
        //------------------------------------------------------
        public GameObject BgMask
        {
            get
            {
                if (m_Serialize == null || m_Serialize.BgMask == null) return null;
                return m_Serialize.BgMask.gameObject;
            }
        }
        //------------------------------------------------------
        public GuideHighlightMask BgHighlightMask
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.BgMask;
            }
        }
        //------------------------------------------------------
        public TMPro.TMP_Text AvatarTipLabel
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.AvatarTipLabel;
            }
        }
        //------------------------------------------------------
        public TMPro.TMP_Text AvatarTitleLabel
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.AvatarTitleLabel;
            }
        }
        //------------------------------------------------------
        public Transform GetFinger(EFingerType type)
        {
            if (m_Serialize == null || m_Serialize.Fingers == null) return null;
            for (int i = 0; i < m_Serialize.Fingers.Length; ++i)
            {
                bool bShow = i == (int)type;
                if (m_Serialize.Fingers[i] && m_Serialize.Fingers[i].gameObject.activeSelf != bShow)
                {
                    m_Serialize.Fingers[i].gameObject.SetActive(bShow);
                }
            }
            if (type >= 0 && (int)type < m_Serialize.Fingers.Length)
                return m_Serialize.Fingers[(int)type];
            return null;
        }
        //------------------------------------------------------
        public Transform GetDescBG(EDescBGType type)
        {
            if (m_Serialize == null || m_Serialize.DescBgs == null) return null;
            for (int i = 0; i < m_Serialize.DescBgs.Length; ++i)
            {
                if (m_Serialize.DescBgs[i])
                    m_Serialize.DescBgs[i].gameObject.SetActive(i == (int)type);
            }
            if (type >= 0 && (int)type < m_Serialize.DescBgs.Length)
            {
                return m_Serialize.DescBgs[(int)type];
            }
            return null;
        }
        //------------------------------------------------------
        public RectTransform DialogArrow
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.DialogArrow;
            }
        }
        //------------------------------------------------------
        public Image GuideImage
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.GuideImage;
            }
        }
        //------------------------------------------------------
        public Image ContinueImage
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.ContinueImage;
            }
        }
        //------------------------------------------------------
        public UnityEngine.UI.Text GuideText
        {
            get
            {
                if (m_Serialize == null) return null;
                return m_Serialize.GuideText;
            }
        }
        //------------------------------------------------------
        public void SetOrder(int order)
        {
            if (m_Serialize == null) return;
            if (m_Serialize.Canvas == null) return;
            m_Serialize.Canvas.sortingOrder = order;
        }
        //------------------------------------------------------
        public void SetDefaultOrder()
        {
            SetOrder(m_nDefaultOrder);
        }
        //-------------------------------------------
        public void OverClear(bool bReset = true)
        {
            if (bReset) ResetData();
        }
        //-------------------------------------------
        public void SlideArrow(Vector4 slidePos, Vector2 param)
        {

        }
        //------------------------------------------------------
        public void AddTipDock(EDescBGType tipType, string tipTitle, string tipsContext, Color tipColor, float speed, int autoHideTime,Vector3 offset, bool is3D, int dockWidget, string dockWidgetTag = null, int listIndex =-1,string listenerName="",bool enableArrow = true)
        {
            ClearTipDock();
            if (m_Serialize.GuideAvatarTip)
                m_Serialize.GuideAvatarTip.gameObject.SetActive(true);
            SetAvatarTipsActive(!string.IsNullOrEmpty(tipsContext));
            SetAvatarTitleActive(!string.IsNullOrEmpty(tipTitle));
            SetAvatarTipsLabel(tipsContext, speed);
            SetAvatarTipsTitle(tipTitle); ;
            SetAvatarTipsLabelColor(tipColor);
            CalcDescBgHeight(tipType, tipsContext);
            SetDialogArrowEnable(enableArrow);

            m_TipsType = tipType;
            m_nTipsDockWidgetGUID = dockWidget;
            m_nTipsDockWidgetTag = dockWidgetTag;
            m_nTipsDockListIndex = listIndex;
            //这边的偏移是屏幕坐标的偏移,不是世界坐标的偏移
            m_TipDockOffset = offset;
            m_bTipsDockPosUse3D = is3D;
            m_TipsSearchListenName = listenerName;
            ListenTipDockWidget();
            if(m_nTipsDockWidgetGUID<=0)
                GetDescBG(tipType);

            if (autoHideTime>0)
            {
                if(m_Serialize) m_Serialize.Invoke("AutoHideDescBG", autoHideTime / 1000.0f);
            }
        }
        //------------------------------------------------------
        void AutoHideDescBG()
        {
            GetDescBG(EDescBGType.None);
        }
        //------------------------------------------------------
        public void ClearTipDock()
        {
            GetDescBG(EDescBGType.None);

            SetAvatarTipsActive(false);
            m_pTipDockWidget = null;
            m_TipsType = EDescBGType.None;
            m_nTipsDockWidgetGUID = 0;
            m_nTipsDockWidgetTag = null;
            m_nTipsDockListIndex = -1;
            m_TipDockOffset = Vector3.zero;
            m_bTipsDockPosUse3D = false;
            m_TipsSearchListenName = "";
            SetAvatarEnable(false);
            if (m_Serialize && m_Serialize.GuideAvatarTip)
                m_Serialize.GuideAvatarTip.gameObject.SetActive(false);
        }
        //------------------------------------------------------
        void ListenTipDockWidget()
        {
            Vector3 pos =Vector3.zero;
            bool isFollowGuid = false;
            if (m_nTipsDockWidgetGUID>0)
            {
                GuideGuid guide = GuideGuidUtl.FindGuide(m_nTipsDockWidgetGUID, m_nTipsDockWidgetTag);
                if (guide == null) return;
                if (m_nTipsDockListIndex > 0)
                {
                    m_pTipDockWidget = GetListIndexTransform(guide, m_nTipsDockListIndex - 1);
                    if (!string.IsNullOrWhiteSpace(m_TipsSearchListenName))//如果有输入控件名字,就查找控件名字
                    {
                        var listeners = m_pTipDockWidget.GetComponentsInChildren<EventTriggerListener>();
                        foreach (var item in listeners)
                        {
                            if (item.name.Equals(m_TipsSearchListenName))
                            {
                                m_pTipDockWidget = item.transform;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    m_pTipDockWidget = guide.transform;
                }
                GetDescBG(m_TipsType);
                SetAvatarTipsActive(true);
            }

            //对话框累加目标坐标
            if (m_pTipDockWidget)
            {
                pos += m_pTipDockWidget.position;//这边使用目标的世界坐标,偏移用屏幕坐标添加
                isFollowGuid = true;
            }

            SetAvatarTipsPos(pos, m_bTipsDockPosUse3D, m_TipsType, isFollowGuid);
        }
        //------------------------------------------------------
        //        public void SlideFinger(Transform finger, StepData step)
        //        {
        //            m_bSlideFinger = false;
        ////             if (step == null || step.FingerOffset.x <= 0) return;
        ////             if (finger == null && step.Slide3DArrow == 0)
        ////                 return;
        ////             m_pSlideFinger = finger;
        ////             m_bSlideFinger = true;
        ////             m_SlideParam = step.FingerOffset;
        ////             m_SlideParam.y = step.ClickZoom.x;
        ////             m_SlideParam.z = step.ClickZoom.y;
        //// 
        ////             m_SlideStartWidget = null;
        ////             Vector3 start3DPos = Vector3.zero;
        ////             GuideSystem guideSystem = GameInstance.getInstance().guideSystem;
        ////             if (step.WidgetGuid!=0)
        ////             {
        ////                 m_SlideStartWidgetGuid = step.WidgetGuid;
        ////                 m_SlideStartWidget = guideSystem.FastFindWidet(step.WidgetGuid);
        ////                 if (m_SlideStartWidget == null)
        ////                 {
        ////                     float startposx = Screen.width * step.FingerSlidePos.x;
        ////                     float startposy = Screen.height * step.FingerSlidePos.y;
        ////                     m_SlideStarPos = guideSystem.ConverScreenToUIPos(new Vector3(startposx, startposy, 0));
        ////                     start3DPos = guideSystem.ConverScreenToWorldPos(new Vector3(startposx, startposy));
        ////                 }
        ////                 else
        ////                 {
        ////                     m_SlideStarPos = m_SlideStartWidget.position;
        ////                     Vector3 screenpos = guideSystem.ConverUIPosToScreen(m_SlideStarPos);
        ////                     start3DPos = guideSystem.ConverScreenToWorldPos(screenpos);
        ////                 }
        ////             }
        ////             else
        ////             {
        ////                 m_SlideStartWidgetGuid = 0;
        ////                 if (step.SlidePosUse3D != 0)
        ////                 {
        ////                     m_SlideStarPos = guideSystem.ConverWorldPosToUIPos(new Vector3(step.FingerSlidePos.x, 0, step.FingerSlidePos.y));
        ////                     start3DPos = new Vector3(step.FingerSlidePos.x, 0, step.FingerSlidePos.y);
        ////                 }
        ////                 else
        ////                 {
        ////                     float startposx = Screen.width * step.FingerSlidePos.x ;
        ////                     float startposy = Screen.height * step.FingerSlidePos.y;
        ////                     m_SlideStarPos = guideSystem.ConverScreenToUIPos(new Vector3(startposx, startposy, 0));
        ////                     start3DPos = guideSystem.ConverScreenToWorldPos(new Vector3(startposx, startposy, 0));
        ////                 }
        ////             }
        //// 
        ////             if (step.SlidePosUse3D == 0)
        ////             {
        ////                 float posx = Screen.width * step.FingerSlidePos.z ;
        ////                 float posy = Screen.height * step.FingerSlidePos.w;
        ////                 m_SlideEndPos = guideSystem.ConverScreenToUIPos(new Vector3(posx, posy, 0));
        ////             }
        ////             else
        ////             {
        ////                 m_SlideEndPos = guideSystem.ConverWorldPosToUIPos(new Vector3(step.FingerSlidePos.z, 0, step.FingerSlidePos.w));
        ////             }
        ////             m_SlideRuning = Vector3.zero;
        ////             InnerUpdate(0f);
        //// #if UNITY_EDITOR
        ////             m_SlidePosParam = step.FingerSlidePos;
        //// #endif
        //        }
        //-------------------------------------------
        public void Update(float fFrameTime)
        {
            if (!m_bVisible)
                return;
            bool bListenWidgetInView = true;
            if(m_bListenGuideWidget)
            {
                ListenWidget();
            }
            else
            {
                if(m_pOriGuideWidget!=null)
                {
                    bListenWidgetInView = IsCheckInViewAdnCanHit(m_pOriGuideWidget, m_bRayTest);
                    if (!bListenWidgetInView)
                    {
                        if (m_bMaskSelfWidget)
                        {
                            m_Serialize.BgMask?.gameObject.SetActive(false);
                        }

                        if(GuideSystem.getInstance().bNoForceDoing)
                        {
                            GuideSystem.getInstance().OverGuide(false);
                        }
                    }
                    else
                    {
                        if (m_bMaskSelfWidget)
                        {
                            m_Serialize.BgMask?.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    //! destroyed widget, so relinsten widget
                    m_bListenGuideWidget = true;
                    m_nListenLastFrame = Time.frameCount;
                    if (m_bMaskSelfWidget)
                    {
                        m_Serialize.BgMask?.gameObject.SetActive(false);
                    }
                    if (GuideSystem.getInstance().bNoForceDoing)
                    {
                        GuideSystem.getInstance().OverGuide(false);
                    }
                }
            }

            ListenTipDockWidget();
            if (m_fingerType == EFingerType.Click || m_fingerType == EFingerType.Effect)
            {
                if (m_pGuideWidget)
                {
                    if (m_Serialize &&m_Serialize.uiPenetrate)
                    {
                        if(m_pGuideTriggerListen) m_pGuideTriggerListen.SetParentrate(m_Serialize.uiPenetrate);
                        m_Serialize.uiPenetrate.SearchListenName = m_SearchListenName;

                        var widgetRect = m_pOriGuideWidget? m_pOriGuideWidget: m_pGuideWidget;
                        m_Serialize.uiPenetrate.TriggerGo = widgetRect?.gameObject;

                        if(widgetRect!=null && widgetRect is RectTransform)
                        {
                            RectTransform rectTransform = widgetRect as RectTransform;
                            RectTransform rect = m_Serialize.uiPenetrate.transform as RectTransform;
                            rect.anchoredPosition = rectTransform.anchoredPosition;
                            rect.anchoredPosition3D = rectTransform.anchoredPosition3D;
                            rect.anchorMax = rectTransform.anchorMax;
                            rect.anchorMin = rectTransform.anchorMin;
                            rect.offsetMax = rectTransform.offsetMax;
                            rect.offsetMin = rectTransform.offsetMin;
                            rect.pivot = rectTransform.pivot;
                            rect.sizeDelta = rectTransform.sizeDelta;
                            rect.position = rectTransform.position;
                            rect.rotation = rectTransform.rotation;
                        }
                    }

                    Transform finger = GetFinger(m_fingerType);
                    if (finger)
                    {
                        if (m_bConvertUIPos)
                        {
                            Vector3 uiPos = Vector3.zero;
                            if (WorldPosToUIPos(m_pGuideWidget.transform.position, false, ref uiPos))
                            {
                                finger.position = uiPos;
                                SetSimulationClickImagePosition(uiPos);
                            }
                        }
                        else
                            finger.position = m_pGuideWidget.transform.position;
                        finger.localPosition += m_FingerOffset;
                        if (!bListenWidgetInView)
                            finger.localPosition = INVAILD_POS;
                    }

                    if (m_Serialize)
                    {
                        if (m_Serialize.BgMask)
                        {
                            if (m_Serialize.BgMask.isActiveAndEnabled)
                            {
                                if (m_pGuideWidget is RectTransform)
                                {
                                    m_Serialize.BgMask.SetTarget(m_pGuideWidget as RectTransform);
                                    m_Serialize.BgMask.EnablePenetrate(true, m_ListenGuideGuid, m_nListIndex, m_ListenGuideGuidTag, false); 
                                }
                                else if (finger is RectTransform)//如果是3D建筑坐标,设置遮罩显示位置为手指的坐标
                                {
                                    m_Serialize.BgMask.Set3DTarget((finger as RectTransform).anchoredPosition);
                                }
                            }
                            else
                                m_Serialize.BgMask.SetTarget(null);
                        }
                        RectTransform widget = m_pOriGuideWidget as RectTransform;
                        if (widget && m_Serialize.TargetContainer)
                        {
                            RectTransform rect = m_Serialize.TargetContainer as RectTransform;
                            rect.anchoredPosition = widget.anchoredPosition;
                            rect.anchoredPosition3D = widget.anchoredPosition3D;
                            rect.anchorMax = widget.anchorMax;
                            rect.anchorMin = widget.anchorMin;
                            rect.offsetMax = widget.offsetMax;
                            rect.offsetMin = widget.offsetMin;
                            rect.pivot = widget.pivot;
                            rect.sizeDelta = widget.sizeDelta;
                            rect.position = widget.position;
                            rect.rotation = widget.rotation;
                            // rect.localScale = widget.localScale;

                            if (m_pGuideWidget != m_pOriGuideWidget)
                            {
                                rect = m_pGuideWidget as RectTransform;
                                rect.anchoredPosition = widget.anchoredPosition;
                                rect.anchoredPosition3D = widget.anchoredPosition3D;
                                rect.anchorMax = widget.anchorMax;
                                rect.anchorMin = widget.anchorMin;
                                rect.offsetMax = widget.offsetMax;
                                rect.offsetMin = widget.offsetMin;
                                rect.pivot = widget.pivot;
                                rect.sizeDelta = widget.sizeDelta;
                                rect.position = widget.position;
                                rect.rotation = widget.rotation;
                                if (rect.localScale == Vector3.zero)//克隆出来UI缩放为0时,默认设置为1
                                {
                                    rect.localScale = Vector3.one;
                                }
                                //rect.localScale = widget.localScale;
                            }

                        }
                    }
                }
                else if(m_ListenGuideGuid==0)
                {
                    ShowSkipBtn(GuideSystem.getInstance().DoingSeqNode);
                }
                else
                {
                    Transform finger = GetFinger(m_fingerType);
                    if(finger) finger.position = INVAILD_POS;
                }

                if(m_bClickZoom)
                {
                    Transform finger = GetFinger(m_fingerType);
                    if(finger!=null)
                    {
                        Vector3 pos = m_ClickZoomPosition;
                        finger.rotation = Quaternion.Euler(m_ClickAngle);
                        if (m_bClick3DPosition)
                        {
                            Vector3 uiPos = Vector3.zero;
                            if (WorldPosToUIPos(m_ClickZoomPosition, true, ref uiPos))
                                pos = uiPos;
                        }
                        finger.localPosition = pos;
#if UNITY_EDITOR
                        if (m_pClickZoomEditor)
                            m_pClickZoomEditor.transform.localPosition = pos;
#endif
                        m_Serialize.BgMask?.Set3DTarget((finger as RectTransform).anchoredPosition);
                    }
                }
            }
            if (!m_bSlideFinger) return;
        }
        //-------------------------------------------
        public void SetMaskActive(bool isActive)
        {
            if (BgMask) BgMask.SetActive(isActive);
        }
        //-------------------------------------------
        public void SetMaskColor(Color color)
        {
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.color = color;
            }
        }
        //-------------------------------------------
        public void SetMaskShape(EMaskType type)
        {
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetShape((GuideHighlightMask.EShape)type);
            }
        }
        //-------------------------------------------
        public void SetMaskSpeed(float speed)
        {
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetSpeed(speed);
            }
        }
        //-------------------------------------------
        public void SetMaskShapeScale(Vector2 scale)
        {
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetShapeScale(scale);
            }
        }
        //-------------------------------------------
        public void SetMaskZoomPenetrate(Vector3 position, float radius, bool b3D)
        {
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetMaskZoomPenetrate(position, radius*GetUGUIScaler(), b3D);
            }
        }
        //-------------------------------------------
        public void SetAvatarTipsLabel(string content)
        {
            if (AvatarTipLabel == null) return;
            AvatarTipLabel.text = content;
        }
        //-------------------------------------------
        public void SetAvatarTipsLabel(string content, float speed)
        {
            m_DialogContent = content;
            if (speed>0)
            {
                if (AvatarTipLabel != null)
                {
                    m_DialogCoroutine = TargetContainer.GetComponent<Image>().StartCoroutine(LabelTransition(content, speed));
                }
            }
            else
            {
                SetAvatarTipsLabel(content);
            }
        }
        //-------------------------------------------
        System.Collections.IEnumerator LabelTransition(string content, float speed)
        {
            m_isShowingDialog = true;
            WaitForSeconds wait = WaitForSecond(Time.deltaTime / (1f / speed));

            if (AvatarTipLabel)
            {
                for (int i = 0; i <= content.Length; i++)
                {
                    AvatarTipLabel.text = content.Substring(0, i);
                    yield return wait;
                }
            }

            m_isShowingDialog = false;
        }
        //-------------------------------------------
        System.Collections.IEnumerator GuideTextTransition(string content, float speed)
        {
            m_isShowingDialog = true;
            WaitForSeconds wait = WaitForSecond(1f / speed);

            if(GuideText)
            {
                for (int i = 1; i <= content.Length; i++)
                {
                    GuideText.text = content.Substring(0, i);
                    yield return wait;
                }
            }

            m_isShowingDialog = false;
        }
        //-------------------------------------------
        public void SetAvatarTipsLabelColor(Color color)
        {
            if (AvatarTipLabel == null)
            {
                return;
            }
            AvatarTipLabel.color = color;
        }
        //-------------------------------------------
        public void SetAvatarTipsTitle(string content)
        {
            if (AvatarTitleLabel == null)
            {
                return;
            }
            AvatarTitleLabel.text = content;
        }
        //-------------------------------------------
        public void SetAvatarTipsPos(Vector3 pos, EDescBGType type,bool isFollowGuid)
        {
            if (AvatarTipLabel == null)
            {
                return;
            }
            //AvatarTipLabel.transform.localPosition = pos;文字跟随背景框移动,没有背景框,不显示文字
            Transform bg = GetDescBG(type);
            if (bg != null)
            {
                if (isFollowGuid)
                {
                    bg.position = pos;//先设置背景框位置,用世界坐标
                    bg.localPosition += m_TipDockOffset;//再加上偏移,用屏幕坐标
                }
                else
                {
                    bg.localPosition = m_TipDockOffset;//不是跟随控件情况下,直接用屏幕坐标赋值
                }
            }
            LimitAvatarTipsPos(type);
        }
        //------------------------------------------------------
        /// <summary>
        /// 限制超出屏幕
        /// 目前只做左右屏幕判断
        /// </summary>
        /// <param name="type"></param>
        void LimitAvatarTipsPos(EDescBGType type)
        {
            RectTransform bg = GetDescBG(type) as RectTransform;
            if (bg == null)
            {
                return;
            }

            float screenWidth = bg.rect.size.x/2;

            Vector2 screen = GetReallyResolution();

            float offset = 0;
            float avatarWidth = 0;
            if (m_Serialize.Avatar != null)
            {
                avatarWidth = m_Serialize.Avatar.rectTransform.rect.x;
            }

            if (((bg.anchoredPosition.x + screenWidth) > screen.x /2))//超出右屏幕
            {
                offset = screen.x / 2 - (bg.anchoredPosition.x + screenWidth);
            }
            else if ((bg.anchoredPosition.x - screenWidth - avatarWidth / 2) < (-screen.x / 2))//超出左屏幕(左屏幕由于有角色图标,所以需要加上角色宽度一半)
            {
                offset = -screen.x / 2 - (bg.anchoredPosition.x - screenWidth - avatarWidth / 2);
            }

            bg.anchoredPosition += new Vector2(offset,0);
        }
        //-------------------------------------------
        public void SetAvatarTipsActive(bool isActive)
        {
            if (AvatarTipLabel == null)
            {
                return;
            }
            AvatarTipLabel.gameObject.SetActive(isActive);
        }
        //-------------------------------------------
        public void SetAvatarTitleActive(bool isActive)
        {
            if (AvatarTitleLabel == null)
            {
                return;
            }
            AvatarTitleLabel.gameObject.SetActive(isActive);
        }
        //-------------------------------------------
        public void SetAvatarTipsPos(Vector3 pos, bool is3D, EDescBGType type,bool isFollowGuid)
        {
            if (AvatarTipLabel == null)
            {
                return;
            }
            if (is3D)
            {
                pos = m_pUICamera.WorldToScreenPoint(pos);
            }
            SetAvatarTipsPos(pos, type, isFollowGuid);
        }
        //-------------------------------------------
        public void SetFinger(EFingerType type, Vector3 angle, Vector2 offset)
        {
            m_fingerType = type;
            Transform finger = GetFinger(type);
            if (finger == null)
            {
                UnityEngine.Debug.LogWarning("获取不到手势类型:" + type);
                return;
            }
            finger.rotation = Quaternion.Euler(angle);
            finger.localPosition += new Vector3(offset.x, offset.y, 0);
        }
        //-------------------------------------------
        public void SetFinger(EFingerType type, Vector3 angle, Vector3 offset)
        {
            m_fingerType = type;
            Transform finger = GetFinger(type);
            if (finger == null)
            {
                UnityEngine.Debug.LogWarning("获取不到手势类型:" + type);
                return;
            }
            m_FingerOffset = offset;
            finger.rotation = Quaternion.Euler(angle);
        }
        //-------------------------------------------
        public void ClickZoom(EFingerType type, Vector3 angle, Vector3 pos, bool is3D, float radius)
        {
            m_fingerType = type;
            Transform finger = GetFinger(type);
            if (finger == null)
            {
                UnityEngine.Debug.LogWarning("获取不到手势类型:" + type);
                return;
            }
            finger.rotation = Quaternion.Euler(angle);
            m_ClickZoomPosition = pos;
            if (is3D)
            {
                Vector3 uiPos = Vector3.zero;
                if (WorldPosToUIPos(pos, true, ref uiPos))
                    pos = uiPos;
            }
            else pos.z = 0;
                finger.localPosition = pos;

            m_bClickZoom = true;
            m_ClickAngle = angle;
            m_bClick3DPosition = is3D;

#if UNITY_EDITOR
            if (m_pClickZoomEditor)
                m_pClickZoomEditor.transform.localPosition = pos;
#endif
            //todo:radius 半径大小怎么表现?
        }
        //-------------------------------------------
        public void MoveUI(GameObject ui, bool isHighLight)
        {
            //CleatMoveUI();

            if (isHighLight)
            {
                if (!m_MoveUIDic.ContainsKey(ui))
                {
                    m_MoveUIDic.Add(ui, ui.transform.parent);

                    ui.transform.SetParent(TargetContainer, true);
                }
            }
            else
            {
                if (m_MoveUIDic.ContainsKey(ui))
                {
                    Transform parent = m_MoveUIDic[ui];
                    ui.transform.SetParent(parent);
                    m_MoveUIDic.Remove(ui);
                }
            }
        }
        //-------------------------------------------
        public void SlideFinger(Vector2 startPos, Vector2 endPos, int sliderSpeed)
        {
            Transform finger = GetFinger(EFingerType.Slide);

        //    if (m_tween == null)
        //    {
        //        m_tween = finger.GetComponentInChildren<Framework.RtgTween.PositionTween>();
         //   }

          //  if (m_tween == null)
            {
                return;
            }
            float time = sliderSpeed / 100f;
            float speed = 1f / time;

        //    m_tween.transform.localPosition = startPos;
        //    m_tween.isLoop = true;
        //    m_tween.Play(startPos, endPos, speed);

        }
        //-------------------------------------------
        public void WidgetListen(StepNode stepNode, EFingerType type, Vector3 angle, Vector2 offset, int clickUI, int clickIndex, string widgetTag = null, bool bTop = true,string searchName = "", bool bMaskSelf = true, bool bRayTest = true)
        {
            if(m_Serialize && m_Serialize.BgMask)
                m_Serialize.BgMask.SetTarget(null);
            ClearWidget();
            m_CurStepNode = stepNode;
            m_bTopCloneWidget = bTop;
            m_nListIndex = clickIndex;
            m_ListenGuideGuid = clickUI;
            m_ListenGuideGuidTag = widgetTag;
            m_bListenGuideWidget = true;
            m_nListenLastFrame = Time.frameCount;
            m_bConvertUIPos = false;
            m_bRayTest = bRayTest;
            m_nListenLastFrame = Time.frameCount;
            m_SearchListenName = searchName;
            m_bMaskSelfWidget = bMaskSelf;
            SetFinger(type, angle, new Vector3(offset.x, offset.y, 0));
            Transform trans = GetFinger(type);
            ListenWidget();
            if(m_Serialize) m_Serialize.gameObject.SetActive(true);
        }
        //------------------------------------------------------
        public void SetCurStepNode(StepNode stepNode)
        {
            m_CurStepNode = stepNode;
        }
        //-------------------------------------------
        void ListenWidget()
        {
            if (m_ListenGuideGuid == 0 || !m_bListenGuideWidget) return;
#if UNITY_EDITOR
            if(IsEditorPreview)
                m_nListenLastFrame = 0;
#endif
            if (m_nListenLastFrame > 0 && Time.frameCount - m_nListenLastFrame < 20)
                return;
            m_nListenLastFrame = Time.frameCount;

            GuideGuid widget = GuideGuidUtl.FindGuide(m_ListenGuideGuid, m_ListenGuideGuidTag);
            if (widget && IsCheckInViewAdnCanHit(widget.transform, m_bRayTest))
            {
                if (m_bMaskSelfWidget) SetMaskActive(true);
                //检测组件状态,如果强制引导,并且没有跳过步骤,则默认显示
                if (widget.gameObject.activeInHierarchy == false && m_CurStepNode != null && m_CurStepNode.bOption == false && m_CurStepNode.bSuccessedListenerBreak == false)
                {
                    widget.gameObject.SetActive(true);
                }
                m_bConvertUIPos = widget.ConvertUIPos;
                if (m_nListIndex >= 0)//动态列表加载情况
                {
                    m_pOriGuideWidget = GetListIndexTransform(widget, m_nListIndex);
                    if (m_pOriGuideWidget == null)
                    {
                        //滚动到英雄头像位置
                        SetGridBoxMgrScrollToIndex(widget, m_nListIndex);
                    }
                    if (!string.IsNullOrWhiteSpace(m_SearchListenName) && m_pOriGuideWidget != null)//如果有输入控件名字,就查找控件名字
                    {
                        Transform trs = m_pOriGuideWidget.Find(m_SearchListenName);//如果查找子物体那么格式为 父物体/子物体
                        if (trs)
                        {
                            m_pOriGuideWidget = trs;
                        }
                        else
                        {
                            //兼容旧得查找子物体
                            var listeners = m_pOriGuideWidget.GetComponentsInChildren<EventTriggerListener>();
                            foreach (var item in listeners)
                            {
                                if (item.name.Equals(m_SearchListenName))
                                {
                                    m_pOriGuideWidget = item.transform;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                    m_pOriGuideWidget = widget.transform;

                if(m_pOriGuideWidget)
                {
               //     if (/*!GuideUtility.IsCheckInViewAdnCanHit(m_pOriGuideWidget.transform) ||*/ GuideUtility.IsCheckTweening(m_pOriGuideWidget.transform) ||
               //         !m_pOriGuideWidget.gameObject.activeSelf) return;
                    if (!m_pOriGuideWidget.gameObject.activeSelf) return;
                }

                if (m_bTopCloneWidget && m_Serialize && m_Serialize.TargetContainer)
                {
                    if(m_pOriGuideWidget)
                    {
                        GameObject pClone = GameObject.Instantiate(m_pOriGuideWidget.gameObject, m_Serialize.TargetContainer, true);
                        m_pGuideWidget = pClone.transform;
                        m_pGuideTriggerListen = pClone.GetComponentInChildren<EventTriggerListener>();
                        pClone.AddComponent<UITouchIngore>();
                         GuideGuid guide = pClone.GetComponent<GuideGuid>();
                        if(guide) GameObject.DestroyImmediate(guide);
                     //   m_pOriGuideWidget.gameObject.SetActive(false);
                        GuideGuidUtl.OnAdd(widget);
                    }
                }
                else
                {
                    m_pGuideWidget = m_pOriGuideWidget;
                }
                //找到组件的情况下才清空
                if (m_pOriGuideWidget != null)
                {
                    m_bListenGuideWidget = false;
                   // m_ListenGuideGuid = 0;
                }

                //3d建筑坐标情况下,设置模拟点击得图片状态
                if (m_bConvertUIPos)
                {
                    SetSimulationClickImageActive(true);
                    SetSimulationClickImageEventTriggerListenerGuid(widget);
                }
            }
            else
            {
                if (m_bMaskSelfWidget) SetMaskActive(false);
            }
        }
        //------------------------------------------------------
        public void SetMaskHighLightRect(int clickUI,int clickIndex, string tag = "",bool click = true)
        {
            if(m_Serialize && m_Serialize.BgMask) m_Serialize.BgMask.SetTarget(null);
            ClearWidget();
            m_bListenGuideWidget = true;
            m_nListenLastFrame = Time.frameCount;
            m_ListenGuideGuid = clickUI;
            m_nListIndex = clickIndex;
            m_ListenGuideGuidTag = tag;
            ListenWidget();
            if (m_Serialize && m_Serialize.BgMask)
            {
                m_Serialize.BgMask.SetShape(GuideHighlightMask.EShape.Box);
                m_Serialize.BgMask.SetTarget(m_pGuideWidget as RectTransform);
                m_Serialize.BgMask.SetClick(click);
            }
        }
        //-------------------------------------------
        public Transform GetListIndexTransform(GuideGuid guide,int index)
        {
            if (guide == null || index < 0)
            {
                return null;
            }

            return guide.GetWidget(index);
        }
        //-------------------------------------------
        /// <summary>
        /// 对话点击
        /// </summary>
        /// <returns></returns>
        public bool OnDialogClick()
        {
            if (m_isShowingDialog)
            {
                if (m_DialogCoroutine != null)
                {
                    TargetContainer.GetComponent<Image>().StopCoroutine(m_DialogCoroutine);
                    m_DialogCoroutine = null;
                    SetAvatarTipsLabel(m_DialogContent);
                    SetGuideText(m_DialogContent);
                    m_isShowingDialog = false;
                }
                return false;
            }
            return true;
        }
        //-------------------------------------------
        public void SetDialogArrow(int widgetID,Vector2 offset,float angle,bool isReverse, string widgetTag= null)
        {
            if (DialogArrow == null)
            {
                return;
            }
            GuideGuid guide = null;
            if (widgetID > 0)
            {
                guide = GuideGuidUtl.FindGuide(widgetID, widgetTag);
            }
            if (guide != null)
            {
                //todo:这边的guide获取有问题,没有考虑到动态加载组件的情况,并且使用的坐标也是本地坐标,可能会出现位置不对的情况
                DialogArrow.localPosition = guide.transform.localPosition + new Vector3(offset.x,offset.y);
            }
            else
            {
                DialogArrow.localPosition += new Vector3(offset.x, offset.y);
            }
            DialogArrow.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (isReverse)
            {
                DialogArrow.localScale = new Vector3(-1, 1, 1);
            }
        }
        //------------------------------------------------------
        void SetDialogArrowEnable(bool enable)
        {
            if (DialogArrow)
            {
                DialogArrow.gameObject.SetActive(enable);
            }
        }
        //-------------------------------------------
        public void SetAvatarHero(string avatarFile)
        {
            if (m_Serialize == null || m_Serialize.Avatar == null)
                return;
            if (string.IsNullOrEmpty(avatarFile))
                avatarFile = m_Serialize.defaultAvatarFile;

            if (string.IsNullOrEmpty(avatarFile))
                return;
            GuideSystem.getInstance().LoadAsset(m_Serialize.Avatar, avatarFile,false);
        }
        //-------------------------------------------
        public void SetGuideImage(Sprite imageSpr,bool isSetNativeSize,Vector2 pos,float angleZ,bool isReverse,Vector2 size,bool isActive,Color color)
        {
            if (GuideImage == null)
            {
                return;
            }

            GuideImage.gameObject.SetActive(isActive);

            GuideImage.sprite = imageSpr;

            if (isSetNativeSize)
            {
                GuideImage.SetNativeSize();
            }
            else
            {
                GuideImage.rectTransform.sizeDelta = size;
            }

            GuideImage.transform.localPosition = pos;

            GuideImage.transform.localScale = isReverse ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            GuideImage.transform.rotation = Quaternion.Euler(0, 0, angleZ);

            GuideImage.color = color;
        }
        //-------------------------------------------
        void ResetImage()
        {
            if (GuideImage == null)
            {
                return;
            }

            GuideImage.gameObject.SetActive(false);
            GuideImage.transform.localPosition = Vector3.zero;
            GuideImage.transform.rotation = Quaternion.identity;
            GuideImage.transform.localScale = Vector3.one;
        }
        //-------------------------------------------
        public void CalcDescBgHeight(EDescBGType descBg,string content)
        {
            Transform trs = GetDescBG(descBg);
            if (trs == null)
            {
                Debug.LogWarning("descBg为null,不计算高度");
                return;
            }
            RectTransform rect = trs as RectTransform;
            if (content == null)
            {
                Debug.LogWarning("显示的文本内容为null,不计算高度");
                return;
            }

            float fontSize = AvatarTipLabel.fontSize;

            int row = Mathf.CeilToInt(rect.sizeDelta.x / fontSize);//每行文字个数,向上取整
            int showLabelCount = 0;
            var matchs = m_Regex.Matches(content);
            if (matchs.Count > 0)//考虑到富文本的使用,去掉富文本的字符长度
            {
                showLabelCount = content.Length;
                foreach (Match item in matchs)
                {
                    //Debug.Log(item);
                    showLabelCount -= item.Length;
                }
            }
            else
            {
                showLabelCount = content.Length;
            }
            
            int rowCount = Mathf.CeilToInt((float)showLabelCount / (float)row) + 2;//计算总共有几行,向上取整 + 上下两行留空
            float height = rowCount * (fontSize + AvatarTipLabel.lineSpacing *4) + Mathf.Abs(AvatarTipLabel.rectTransform.sizeDelta.y);//4是文字间隔 + 文本上下边距
            height = height > m_DescBgMinHeight ? height : m_DescBgMinHeight;//高度最少值
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
            //Framework.Plugin.Logger.Warning("row:" + row + ",column:" + rowCount + ",height:" + height);
        }
        //------------------------------------------------------
        public void SetGuideText(Vector2 size,Color color,Vector2 pos,int fontSize,bool isEnable,string text, bool isTransition,int speed)
        {
            if (GuideText == null)
            {
                Debug.LogError("找不到GuideText对象");
                return;
            }
            if(m_Serialize && m_Serialize.GuideDescWidget)
                m_Serialize.GuideDescWidget.gameObject.SetActive(isEnable);

            GuideText.enabled = isEnable;

            GuideText.color = color;
            GuideText.rectTransform.sizeDelta = size;
            GuideText.rectTransform.localPosition = pos;
            GuideText.fontSize = fontSize;

            if (isTransition && text != null)
            {
                m_DialogCoroutine = TargetContainer.GetComponent<Image>().StartCoroutine(GuideTextTransition(text, speed));
            }
            else
            {
                SetGuideText(text);
            }
            m_DialogContent = text;
        }
        //------------------------------------------------------
        public void SetGuideText(string text)
        {
            if (GuideText == null)
            {
                UnityEngine.Debug.LogError("找不到GuideText对象");
                return;
            }
            GuideText.text = text;
            m_DialogContent = text;
        }
        //-------------------------------------------
        void ResetGuideText()
        {
            if (m_Serialize && m_Serialize.GuideDescWidget)
                m_Serialize.GuideDescWidget.gameObject.SetActive(false);
            if (GuideText == null)
            {
                return;
            }

            GuideText.enabled = false;

            GuideText.color = Color.white;
            GuideText.rectTransform.sizeDelta = Vector2.zero;
            GuideText.rectTransform.localPosition = Vector2.zero;
            GuideText.fontSize = 23;
            GuideText.text = "";
        }
        //------------------------------------------------------
        public void SetContinueImageEnable(bool isEnable)
        {
            if (ContinueImage == null)
            {
                return;
            }

            ContinueImage.enabled = isEnable;
        }
        //------------------------------------------------------
        public void SetContinueImagePos(Vector2 pos)
        {
            if (ContinueImage == null)
            {
                return;
            }

            ContinueImage.rectTransform.anchoredPosition = pos;
        }
        //------------------------------------------------------
        public void SetContinueImageSize(Vector2 size)
        {
            if (ContinueImage == null)
            {
                return;
            }

            ContinueImage.rectTransform.sizeDelta = size;
        }
        //------------------------------------------------------
        public void ResetContinueImage()
        {
            if (ContinueImage == null)
            {
                return;
            }

            ContinueImage.SetNativeSize();
            Vector2 size = ContinueImage.rectTransform.sizeDelta;
            ContinueImage.rectTransform.anchoredPosition = new Vector2(-size.x/2f, size.y/2f);
            ContinueImage.enabled = false;
        }
        //------------------------------------------------------
        public void OnStuckSkipGuide(SeqNode pNode)
        {
            if (GuideSystem.getInstance().IsEnableStuckSkip == false)
            {
                return;
            }

            GuideSystem.getInstance().OnStuckSkipGuide(pNode.GetTag());
      //      Net.GuideHandler.Req_SetGuideRequest(pNode.GetTag());
        }
        //------------------------------------------------------
        public void ShowSkipBtn(SeqNode pNode)
        {
            if (pNode == null) return;
            if (m_Serialize.SkipBtn)
            {
                m_Serialize.SkipBtn.gameObject.SetActive(true);
                EventTriggerListener.Get(m_Serialize.SkipBtn.gameObject).onClick = (go,param) => {
                    OnStuckSkipGuide(pNode);
                };
            }
        }
        //------------------------------------------------------
        public void SetPenetrateEnable(bool enable,int targetGuid, string targetTag = null)
        {
            if (m_Serialize && m_Serialize.BgMask)
                m_Serialize.BgMask.EnablePenetrate(enable, targetGuid,-1, targetTag,true);
        }
        //-------------------------------------------
        public void SetGridBoxMgrScrollToIndex(GuideGuid guide, int index)
        {
            if (guide == null || index < 0)
            {
                return;
            }
            IGuideScroll listView = guide.GetComponent<IGuideScroll>();
            if (listView!=null)
            {
                listView.ScrollToIndex(index,0);
            }
        }
        //------------------------------------------------------
        void SetSimulationClickImageActive(bool active)
        {
            if (m_Serialize&& m_Serialize.SimulationClickImage)
            {
                m_Serialize.SimulationClickImage.gameObject.SetActive(active);
            }
        }
        //------------------------------------------------------
        void SetSimulationClickImagePosition(Vector3 pos)
        {
            if (m_Serialize && m_Serialize.SimulationClickImage)
            {
                m_Serialize.SimulationClickImage.position = pos;
            }
        }
        //------------------------------------------------------
        void SetSimulationClickImageEventTriggerListenerGuid(GuideGuid guid)
        {
            if (SimulationClickListener)
            {
                SimulationClickListener.SetGuideGuid(guid);
            }
        }
        //------------------------------------------------------
        public void SetAvatarEnable(bool enable, string avatarFile = null)
        {
            if (m_Serialize && m_Serialize.Avatar)
            {
                m_Serialize.Avatar.enabled = enable;

                if (enable)
                {
                    SetAvatarHero(avatarFile);
                }
            }
        }
        //------------------------------------------------------
        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return m_pUICamera.WorldToScreenPoint(worldPos);
        }
        //------------------------------------------------------
        public Vector3 ScreenToWorldPoint(Vector3 screenPos)
        {
            return m_pUICamera.ScreenToWorldPoint(screenPos);
        }
        //------------------------------------------------------
        public bool WorldPosToUIPos(Vector3 worldPos, bool bLocal, ref Vector3 point, Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null) return false;
            Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
            if (bLocal)
            {
                Vector2 local_temp = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_pRootUI, screenPos, m_pUICamera, out local_temp))
                {
                    point.x = local_temp.x;
                    point.y = local_temp.y;
                    return true;
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_pRootUI, screenPos, m_pUICamera, out point))
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public Vector3 UGUIPosToWorldPos(Vector3 uiguiPos, float distance = 0, Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(m_pUICamera, uiguiPos);
            Ray ray = cam.ScreenPointToRay(screenPoint);
            return ray.GetPoint(distance <= 0 ? cam.farClipPlane : distance);
        }
        //------------------------------------------------------
        public bool ScreenToUIPos(Vector3 screenPos, bool bLocal, ref Vector3 point)
        {
            if (bLocal)
            {
                Vector2 local_temp = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_pRootUI, screenPos, m_pUICamera, out local_temp))
                {
                    point.x = local_temp.x;
                    point.y = local_temp.y;
                    return true;
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_pRootUI, screenPos, m_pUICamera, out point))
                {
                    return true;
                }
            }
            return false;
        }
        //-------------------------------------------
        WaitForSeconds WaitForSecond(float second)
        {
            int miSecond = ((int)second * 1000);
            WaitForSeconds wait;
            if (m_WaiteSendond != null)
            {
                if (m_WaiteSendond.TryGetValue(miSecond, out wait))
                    return wait;
            }
            else
            {
                m_WaiteSendond = new Dictionary<int, WaitForSeconds>();
            }

            wait = new WaitForSeconds(second);
            m_WaiteSendond[miSecond] = wait;
            return wait;
        }
        //------------------------------------------------------
        public Vector2 GetReallyResolution()
        {
            if (m_pRootUI == null)
            {
                if (m_CanvasScaler != null)
                {
                    return m_CanvasScaler.referenceResolution;
                }
                return new Vector2(1080, 1920);
            }
            return m_pRootUI.sizeDelta;
        }     
        //------------------------------------------------------
        public float GetUGUIScaler()
        {
            if (m_CanvasScaler != null)
            {
                return m_CanvasScaler.scaleFactor;
            }
            return 1;
        }
        //------------------------------------------------------
        public float GetUGUIPixelUnit()
        {
            if (m_CanvasScaler != null)
            {
                return m_CanvasScaler.dynamicPixelsPerUnit;
            }
            return 1;
        }
        //-------------------------------------------
        public float UGUISizeToWorldSize(float uiSize)
        {
            if (m_pRootUICanvas == null) return uiSize;
            if (m_pRootUICanvas.renderMode != RenderMode.ScreenSpaceCamera)
                return uiSize;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(m_pUICamera, m_Serialize.transform.position);
            Vector3 screenPosWithRadius = screenPos + new Vector3(uiSize, 0, 0);
            screenPos.z = m_pRootUICanvas.planeDistance;
            screenPosWithRadius.z = screenPos.z;
            Vector3 worldPosWithRadius = m_pUICamera.ScreenToWorldPoint(screenPosWithRadius);
            Vector3 worldPosCenter = m_pUICamera.ScreenToWorldPoint(screenPos);
            return (worldPosWithRadius - worldPosCenter).magnitude;
        }
        //------------------------------------------------------
        public bool IsCheckInViewAdnCanHit(Transform pObj, bool bRayTest)
        {
            if (pObj is RectTransform)
            {
                RectTransform rectTrans = pObj as RectTransform;
                //lb,lt,rt,rb
                m_pRootUI.GetWorldCorners(ms_contersArray);
                rectTrans.GetWorldCorners(ms_contersArray1);
                Bounds rootBd = new Bounds();
                rootBd.min = new Vector3(ms_contersArray[0].x, ms_contersArray[0].y, 0);//忽略旋转
                rootBd.max = new Vector3(ms_contersArray[2].x, ms_contersArray[2].y, 0);

                Bounds tranBd = new Bounds();
                tranBd.min = new Vector3(ms_contersArray1[0].x, ms_contersArray1[0].y, 0);//忽略旋转
                tranBd.max = new Vector3(ms_contersArray1[2].x, ms_contersArray1[2].y, 0);
                bool bInView = tranBd.Intersects(rootBd);
                if(m_nListIndex >=0 || !bRayTest)
                    return bInView;
                if(bInView)
                {
                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_pUICamera, rectTrans.position);
                    if (m_pTestEventData == null) m_pTestEventData = new PointerEventData(EventSystem.current);
                    m_pTestEventData.position = screenPos;
                    if (m_RayTestResults == null) m_RayTestResults = new List<RaycastResult>(4);
                    m_RayTestResults.Clear();
                    EventSystem.current.RaycastAll(m_pTestEventData, m_RayTestResults);

                    if(m_RayTestResults.Count>0)
                    {
                        int checkIndex = 0;
                        if(this.BgMask!=null && this.BgMask.gameObject.activeInHierarchy)
                        {
                            if (m_RayTestResults[0].gameObject == this.BgMask)
                                checkIndex = 1;
                        }

                        if(checkIndex < m_RayTestResults.Count)
                        {
                            var result = m_RayTestResults[checkIndex];
                            if (result.gameObject != null)
                            {
                                // 判断是否为目标对象或其子对象
                                Transform t = result.gameObject.transform;
                                while (t != null)
                                {
                                    if (t == pObj)
                                        return true;
                                    t = t.parent;
                                }
                            }
                        }
                    }
   
                }
                return false;
            }
            else
            {
                if(Camera.main !=null)
                {
                    var mainCam = Camera.main;
                    var camTrans = mainCam.transform;
                    Vector2 viewPos = mainCam.WorldToViewportPoint(pObj.position);
                    Vector3 dir = (pObj.position - camTrans.position).normalized;
                    float dot = Vector3.Dot(camTrans.forward, dir);
                    if (dot > 0 && viewPos.x >= -0.1f && viewPos.x <= 1.1f && viewPos.y >= -0.1f && viewPos.y <= 1.1f)
                        return true;
                }
            }
            return false;
        }
    }
}