/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideStepHandler
作    者:	HappLI
描    述:	内置步骤操作句柄
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Framework.Guide
{
	public class GuideStepHandler
	{
        static Vector2 ms_TouchBeginPos;
        static Vector2 ms_BeginMousePos;
        static int ms_nowGuid = 0;
        //-------------------------------------------
        public static bool OnGuideExecuteNode(StepNode pNode)
		{
			var guidePanel = GuideSystem.getInstance().GetGuidePanel();
			if (guidePanel == null)
				return false;

			switch((GuideStepType)pNode.type)
			{
				case GuideStepType.ClickUI:
                    {
                        OnClickUIExcude(pNode, guidePanel);
                        //重置步骤节点是否自动跳转状态
                        if (pNode._Ports != null && pNode._Ports.Count > 16)
                        {
                            pNode._Ports[16].fillValue = 0;
                        }
                        guidePanel.bDoing = true;
                        guidePanel.SetCurStepNode(pNode);
                    }
                    return true;
				case GuideStepType.Slide:
                    {
                        OnSliderExcude(pNode, guidePanel);
                        guidePanel.SetCurStepNode(pNode);
                        guidePanel.bDoing = true;
                    }
					return true;
				case GuideStepType.ClickZoom:
                    {
                        OnSlickZoomExcude(pNode, guidePanel);
                        guidePanel.SetCurStepNode(pNode);
                        guidePanel.bDoing = true;
                    }
					return true;
				case GuideStepType.ClickAnywhere:
                    {
                        OnClickAnywhereExcude(pNode, guidePanel);
                        guidePanel.SetCurStepNode(pNode);
                        guidePanel.bDoing = true;
                    }
					return true;
                    case GuideStepType.SlideCheckDirection:
                    {
                        OnSliderCheckDirectionExcude(pNode, guidePanel);
                        guidePanel.SetCurStepNode(pNode);
                        guidePanel.bDoing = true;
                    }
                    return true;
                case GuideStepType.SlideCheckDirectionImmediately:
                    {
                        OnSliderCheckDirectionImmediatelyExcude(pNode, guidePanel);
                        guidePanel.bDoing = true;
                    }
                    return true;
                default:
                    return false;
            }

            return false;
		}
        //-------------------------------------------
        public static bool OnGuideNodeAutoNext(StepNode pNode)
        {
            var guidePanel = GuideSystem.getInstance().GetGuidePanel();
            if (guidePanel == null)
                return false;

            if (pNode._Ports == null || pNode._Ports.Count <= 0) return true;
            if (pNode.IsAutoNext() && pNode.GetAutoNextTime() > 0)
            {
                switch ((GuideStepType)pNode.type)
                {
                    case GuideStepType.ClickUI:
                        {
                            if (pNode._Ports.Count > 16)
                            {
                                pNode._Ports[16].fillValue = 1;//如果自动跳转了,那么设置节点状态为1
                            }
                        }
                        break;
                }

                //步骤节点完成后,要跳转到下一个节点时,都进行guide界面的清理
                guidePanel.ClearData();
            }
            return false;
        }
        //-------------------------------------------
        public static bool OnGuideCheckSign(StepNode pNode, CallbackParam param)
        {
            var guidePanel = GuideSystem.getInstance().GetGuidePanel();
            if (guidePanel == null)
                return false;

            if (param.touchType == ETouchType.Begin)
                ms_BeginMousePos = param.mousePos;
            if (ms_nowGuid != pNode.Guid)
            {
                // Framework.Plugin.Logger.Warning("OnGuideSign pNode.type= " + pNode.type);
                //打点
                ms_nowGuid = pNode.Guid;
            }
            switch ((GuideStepType)pNode.type)
            {
                case GuideStepType.ClickUI:
                    return OnClickUISign(pNode, param,guidePanel);
                case GuideStepType.Slide:
                    bool result = OnSlideSign(pNode, param, guidePanel);
                    if (result)
                    {
                        guidePanel.ClearData();
                    }
                    return result;
                case GuideStepType.ClickZoom:
                    result = OnClickZoomSign(pNode, param, guidePanel);
                    if (result)
                    {
                        guidePanel.ClearData();
                    }
                    return result;
                case GuideStepType.ClickAnywhere:
                    result = OnClickAnywhereSign(pNode, param, guidePanel);
                    if (guidePanel != null)
                    {
                        guidePanel.bDoing = false;
                    }
                    if (result)
                    {
                        guidePanel.ClearData();
                    }
                    return result;
                case GuideStepType.SlideCheckDirection:
                    result = OnSlideCheckDirectionSign(pNode, param, guidePanel);
                    if (result)
                    {
                        guidePanel.ClearData();
                    }
                    return result;
                case GuideStepType.SlideCheckDirectionImmediately:
                    result = OnSlideCheckDirectionImmediatelySign(pNode, param, guidePanel);
                    if (result)
                    {
                        guidePanel.ClearData();
                    }
                    return result;
                case GuideStepType.WaitGameobjectActive:
                    return WaitGameobjectActive(pNode);
                case GuideStepType.WaitGameobjectCanClick:
                    return WaitGameobjectCanClick(pNode);
                default:
                    break;
            }
            return false;
        }
        //-------------------------------------------
        public static void OnClickUIExcude(StepNode pNode, GuidePanel guidePanel)
        {
            if (pNode._Ports.Count == 0)
            {
                return;
            }
            int guid = pNode._Ports[0].fillValue;
            string tagName = pNode._Ports[1].fillStrValue;
            GuideGuid guideGuid = GuideGuidUtl.FindGuide(guid, tagName);
            int clickIndex = pNode._Ports[2].fillValue-1;
            string listenerName = pNode._Ports[3].fillStrValue;
            int touchType = pNode._Ports[4].fillValue;
            int angle = pNode._Ports[5].fillValue;
            int offsetX = pNode._Ports[6].fillValue;
            int offsetY = pNode._Ports[7].fillValue;

            bool bRayTest = pNode._Ports[8].fillValue != 0;
            bool bMostTop = pNode._Ports[9].fillValue != 0;

            bool bMask = pNode._Ports[10].fillValue != 0;
            EMaskType maskType = (EMaskType)pNode._Ports[11].fillValue;
            Color maskColor = pNode._Ports[12].ToColor();
            Vector2 maskScale = pNode._Ports[13].ToVec2();
            float maskSpeed = pNode._Ports[14].fillValue * 0.001f;

            //如果是3d,那么设置点击组件位置到手指位置,点击组件挂载 EventTriggerListener ,并且调用 SetGuideGuid 设置guid对象进行模拟点击guid的功能
            guidePanel.WidgetListen(pNode, (EFingerType)touchType, new Vector3(0, 0, angle), new Vector2(offsetX, offsetY), guid, clickIndex, tagName, bMostTop, listenerName, bMask,bRayTest);
            if(bMask)
            {
                guidePanel.SetMaskSpeed(maskSpeed);
                guidePanel.SetMaskShape(maskType);
                guidePanel.SetMaskShapeScale(maskScale);
                guidePanel.SetMaskActive(bMask);
                guidePanel.SetMaskColor(maskColor);
            }
            else
                guidePanel.SetMaskActive(bMask);
        }
        //-------------------------------------------
        private static bool OnClickUISign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            if (param.triggerType == EUIWidgetTriggerType.Click)
            {
                //按任意键功能
                if (pNode._Ports.Count > 16)
                {
                    bool isPressAnyKey = pNode._Ports[16].fillValue == 1;
                    if (isPressAnyKey)
                    {
                        guidePanel.ClearData();
                        return true;
                    }
                }
                //动态列表点击
                if (param.listIndex != -1)
                {
                    if(string.IsNullOrEmpty(param.widgetTag))
                    {
                        if (pNode._Ports[0].fillValue == param.widgetGuid && (pNode._Ports[2].fillValue - 1) == param.listIndex)
                        {
                            guidePanel.ClearData();
                            return true;
                        }
                    }
                    else
                    {
                        if (pNode._Ports[0].fillValue == param.widgetGuid && 
                            param.widgetTag.CompareTo(pNode._Ports[1].fillStrValue) ==0 && 
                            (pNode._Ports[2].fillValue - 1) == param.listIndex)
                        {
                            guidePanel.ClearData();
                            return true;
                        }
                    }

                }
                else
                {
                    if (string.IsNullOrEmpty(param.widgetTag))
                    {
                        if (pNode._Ports[0].fillValue == param.widgetGuid)//点击到指定guidUI
                        {
                            GuideGuid widget = GuideGuidUtl.FindGuide(param.widgetGuid);

                            guidePanel.ClearData();
                            return true;
                        }
                    }
                    else
                    {
                        if (pNode._Ports[0].fillValue == param.widgetGuid && param.widgetTag.CompareTo(pNode._Ports[1].fillStrValue) == 0)//点击到指定guidUI
                        {
                            GuideGuid widget = GuideGuidUtl.FindGuide(param.widgetGuid, param.widgetTag);

                            guidePanel.ClearData();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //-------------------------------------------
        public static void OnSliderExcude(StepNode pNode, GuidePanel guidePanel)
        {
            bool isStartPos3D = pNode._Ports[2].fillValue == 1;
            int startPosX = pNode._Ports[3].fillValue;
            int startPosY = pNode._Ports[4].fillValue;
            int startRadius = pNode._Ports[5].fillValue;

            bool isEndPos3D = pNode._Ports[6].fillValue == 1;
            int endPosX = pNode._Ports[7].fillValue;
            int endPosY = pNode._Ports[8].fillValue;
            int endRadius = pNode._Ports[9].fillValue;

            int sliderSpeed = 100;
            if (pNode._Ports.Count >= 11)
            {
                sliderSpeed = pNode._Ports[10].fillValue;
            }


            Vector2 startPos = new Vector2(startPosX, startPosY);
            Vector2 endPos = new Vector2(endPosX, endPosY);

            if (isStartPos3D)
            {
                Vector3 start3DPos = new Vector3(startPos.x, 0, startPos.y);
                start3DPos = guidePanel.WorldToScreenPoint(start3DPos);
                startPos = new Vector2(start3DPos.x, start3DPos.y);
            }

            if (isEndPos3D)
            {
                Vector3 end3DPos = new Vector3(endPos.x, 0, endPos.y);
                end3DPos = guidePanel.WorldToScreenPoint(end3DPos);
                endPos = new Vector2(end3DPos.x, end3DPos.y);
            }

            guidePanel.Show();
            guidePanel.SlideFinger(startPos, endPos, sliderSpeed);
        }
        //-------------------------------------------
        public static Vector2 MousePosToScreenPos(Vector2 mousePos)
        {
            return mousePos - new Vector2(Screen.width / 2f, Screen.height / 2f);
        }
        //-------------------------------------------
        public static bool OnSlideSign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            bool isStartPos3D = pNode._Ports[2].fillValue == 1;
            int startPosX = pNode._Ports[3].fillValue;
            int startPosY = pNode._Ports[4].fillValue;
            int startRadius = pNode._Ports[5].fillValue;

            bool isEndPos3D = pNode._Ports[6].fillValue == 1;
            int endPosX = pNode._Ports[7].fillValue;
            int endPosY = pNode._Ports[8].fillValue;
            int endRadius = pNode._Ports[9].fillValue;

            Vector2 mousePos = MousePosToScreenPos(param.mousePos);


            //Framework.Plugin.Logger.Warning("param.touchType=" + param.touchType);

            if (param.touchType == ETouchType.Begin)
            {
                ms_TouchBeginPos = MousePosToScreenPos(param.mousePos);
            }
            else if (param.touchType == ETouchType.End)
            {
                bool isStartTouchPass = false;

                if (isStartPos3D)
                {
                    int z = startPosY;
                    startPosY = 0;
                    Vector3 worldPos = guidePanel.ScreenToWorldPoint(ms_TouchBeginPos);
                    if ((worldPos.x - startPosX) * (worldPos.x - startPosX) + (worldPos.y - startPosY) * (worldPos.y - startPosY) + (worldPos.z - z) * (worldPos.z - z) <= startRadius * startRadius)
                    {
                        isStartTouchPass = true;
                    }
                }
                else
                {
                    if ((ms_TouchBeginPos.x - startPosX) * (ms_TouchBeginPos.x - startPosX) + (ms_TouchBeginPos.y - startPosY) * (ms_TouchBeginPos.y - startPosY) <= startRadius * startRadius)
                    {
                        isStartTouchPass = true;
                    }
                }
                if (isStartTouchPass)
                {
                    if (isEndPos3D)
                    {
                        int z = endPosY;
                        endPosY = 0;
                        Vector3 worldPos = guidePanel.ScreenToWorldPoint(mousePos);
                        if ((worldPos.x - endPosX) * (worldPos.x - endPosX) + (worldPos.y - endPosY) * (worldPos.y - endPosY) + (worldPos.z - z) * (worldPos.z - z) <= endRadius * endRadius)
                        {
                      //      GameInstance.getInstance().OnTouchBegin(new ATouchInput.TouchData() { touchID = 1, position = ms_BeginMousePos });
                       //     GameInstance.getInstance().OnTouchEnd(new ATouchInput.TouchData() { touchID = 1, position = param.mousePos });
                            return true;
                        }
                    }
                    else
                    {
                        //if (TopGame.Core.DebugConfig.bGuideLogEnable)
                        //Framework.Plugin.Logger.Warning("end click value =" + mousePos + "," + ((mousePos.x - endPosX) * (mousePos.x - endPosX) + (mousePos.y - endPosY) * (mousePos.y - endPosY)) + ",radius=" + endRadius * endRadius);
                        if ((mousePos.x - endPosX) * (mousePos.x - endPosX) + (mousePos.y - endPosY) * (mousePos.y - endPosY) <= endRadius * endRadius)
                        {
                       //     GameInstance.getInstance().OnTouchBegin(new ATouchInput.TouchData() { touchID = 1, position = ms_BeginMousePos });
                       //     GameInstance.getInstance().OnTouchEnd(new ATouchInput.TouchData() { touchID = 1, position = param.mousePos });
                            return true;
                        }
                    }

                }
                return false;
            }
            return false;
        }
        //-------------------------------------------
        public static void OnSlickZoomExcude(StepNode pNode, GuidePanel guidePanel)
        {
            int touchType = pNode._Ports[0].fillValue;
            int angle = pNode._Ports[1].fillValue;
            bool is3D = pNode._Ports[2].fillValue == 1;
            float posX = pNode._Ports[3].fillValue*0.001f;
            float posY = pNode._Ports[4].fillValue * 0.001f;
            float posZ = pNode._Ports[5].fillValue * 0.001f;
            float radius = pNode._Ports[6].fillValue*0.001f;

            guidePanel.Show();
            guidePanel.ClickZoom((EFingerType)touchType, new Vector3(0, 0, angle), new Vector3(posX, posY, posZ), is3D, radius);

            bool bMask = pNode._Ports[7].fillValue != 0;
            EMaskType maskType = (EMaskType)pNode._Ports[8].fillValue;
            Color maskColor = pNode._Ports[9].ToColor();
            Vector2 maskScale = pNode._Ports[10].ToVec2();
            float maskSpeed = pNode._Ports[11].fillValue * 0.001f;
            if (bMask)
            {
                guidePanel.SetMaskSpeed(maskSpeed);
                guidePanel.SetMaskShape(maskType);
                guidePanel.SetMaskShapeScale(maskScale);
                guidePanel.SetMaskActive(bMask);
                guidePanel.SetMaskColor(maskColor);
                guidePanel.SetMaskZoomPenetrate(new Vector3(posX, posY, posZ), radius, is3D);
            }
            else
                guidePanel.SetMaskActive(bMask);
        }
        //-------------------------------------------
        public static bool OnClickZoomSign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            if (param.touchType != ETouchType.End && param.triggerType != EUIWidgetTriggerType.Click)
                return false;
            bool is3D = pNode._Ports[2].fillValue == 1;
            float x = pNode._Ports[3].fillValue * 0.001f;
            float y = pNode._Ports[4].fillValue * 0.001f;
            float z = pNode._Ports[5].fillValue * 0.001f;
            float radius = pNode._Ports[6].fillValue*0.001f* guidePanel.GetUGUIScaler();
            if (is3D)
            {
                var mainCam = Camera.main;
                if (mainCam == null)
                    return false;
                var ray = mainCam.ScreenPointToRay(param.mousePos);
                if (!RayInsectionFloor(out var worldPos, ray.origin, ray.direction, y))
                    return false;
                if ((worldPos.x - x) * (worldPos.x - x) + (worldPos.y - y) * (worldPos.y - y) + (worldPos.z - z) * (worldPos.z - z) <= radius * radius)
                {
                    return true;
                }
            }
            else
            {
                Vector3 uguiPos = Vector3.zero;
                if (!guidePanel.ScreenToUIPos(param.mousePos, true, ref uguiPos))
                    return false;
                if ((uguiPos.x - x) * (uguiPos.x - x) + (uguiPos.y - y) * (uguiPos.y - y) <= radius * radius)
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public static bool RayInsectionFloor(out Vector3 retPos, Vector3 pos, Vector3 dir, float floorY = 0)
        {
            retPos = Vector3.zero;
            Vector3 vPlanePos = Vector3.zero;
            vPlanePos.y = floorY;

            Vector3 vPlaneNor = Vector3.up;

            float fdot = Vector3.Dot(dir, vPlaneNor);
            if (fdot == 0.0f)
                return false;

            float fRage = ((vPlanePos.x - pos.x) * vPlaneNor.x + (vPlanePos.y - pos.y) * vPlaneNor.y + (vPlanePos.z - pos.z) * vPlaneNor.z) / fdot;

            retPos = pos + dir * fRage;
            return true;
        }
        //-------------------------------------------
        public static void OnClickAnywhereExcude(StepNode pNode, GuidePanel guidePanel)
        {
            guidePanel.Show();
        }
        //-------------------------------------------
        public static bool OnClickAnywhereSign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            if (param.triggerType == EUIWidgetTriggerType.Click)
            {
                if (guidePanel.IsShowingDialog)
                {
                    guidePanel.OnDialogClick();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        //-------------------------------------------
        public static void OnSliderCheckDirectionExcude(StepNode pNode, GuidePanel guidePanel)
        {
            int startPosX = pNode._Ports[0].fillValue;
            int startPosY = pNode._Ports[1].fillValue;

            int endPosX = pNode._Ports[2].fillValue;
            int endPosY = pNode._Ports[3].fillValue;

            int checkAngle = pNode._Ports[5].fillValue;

            int sliderSpeed = 100;
            if (pNode._Ports.Count >= 5)
            {
                sliderSpeed = pNode._Ports[4].fillValue;
                if (sliderSpeed == 0)
                {
                    sliderSpeed = 100;
                }
            }

            Vector2 startPos = new Vector2(startPosX, startPosY);
            Vector2 endPos = new Vector2(endPosX, endPosY);

            guidePanel.Show();
            guidePanel.SlideFinger(startPos, endPos, sliderSpeed);
        }
        //-------------------------------------------
        public static bool OnSlideCheckDirectionSign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            //bool isStartPos3D = pNode._Ports[2].fillValue == 1;
            int startPosX = pNode._Ports[0].fillValue;
            int startPosY = pNode._Ports[1].fillValue;

            //bool isEndPos3D = pNode._Ports[6].fillValue == 1;
            int endPosX = pNode._Ports[2].fillValue;
            int endPosY = pNode._Ports[3].fillValue;

            int checkAngle = pNode._Ports[5].fillValue;

            Vector2 mousePos = MousePosToScreenPos(param.mousePos);//UI屏幕以中心点为0,0点,鼠标以左下角为0,0点,将鼠标位置转换到以屏幕坐标系为准

            if (param.touchType == ETouchType.Begin)
            {
                ms_TouchBeginPos = MousePosToScreenPos(param.mousePos);
            }
            else if (param.touchType == ETouchType.End)
            {
                Vector2 endPos = MousePosToScreenPos(param.mousePos);
                Vector2 sourceDirection = new Vector2(endPosX, endPosY) - new Vector2(startPosX, startPosY);
                Vector2 destinationDirection = endPos - ms_TouchBeginPos;

                float angle = Vector2.Angle(sourceDirection, destinationDirection);
                //float dot = Vector2.Dot(sourceDirection.normalized, destinationDirection.normalized);
                //float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                //if (TopGame.Core.DebugConfig.bGuideLogEnable)
                //Framework.Plugin.Logger.Warning("angle=" + angle + ",m_TouchBeginPos:" + m_TouchBeginPos + ",param.mousePos:" + endPos + ",setStart:" + new Vector2(startPosX, startPosY) + ",setEnd:" + new Vector2(endPosX, endPosY));
                if (angle <= checkAngle)
                {
                    return true;
                }

                return false;
            }
            return false;
        }
        //-------------------------------------------
        public static void OnSliderCheckDirectionImmediatelyExcude(StepNode pNode, GuidePanel guidePanel)
        {
            int startPosX = pNode._Ports[0].fillValue;
            int startPosY = pNode._Ports[1].fillValue;

            int endPosX = pNode._Ports[2].fillValue;
            int endPosY = pNode._Ports[3].fillValue;

            int checkAngle = pNode._Ports[5].fillValue;

            int sliderSpeed = 100;
            if (pNode._Ports.Count >= 5)
            {
                sliderSpeed = pNode._Ports[4].fillValue;
                if (sliderSpeed == 0)
                {
                    sliderSpeed = 100;
                }
            }

            Vector2 startPos = new Vector2(startPosX, startPosY);
            Vector2 endPos = new Vector2(endPosX, endPosY);

            guidePanel.Show();
            guidePanel.SlideFinger(startPos, endPos, sliderSpeed);
        }
        //-------------------------------------------
        public static bool OnSlideCheckDirectionImmediatelySign(StepNode pNode, CallbackParam param, GuidePanel guidePanel)
        {
            //bool isStartPos3D = pNode._Ports[2].fillValue == 1;
            int startPosX = pNode._Ports[0].fillValue;
            int startPosY = pNode._Ports[1].fillValue;

            //bool isEndPos3D = pNode._Ports[6].fillValue == 1;
            int endPosX = pNode._Ports[2].fillValue;
            int endPosY = pNode._Ports[3].fillValue;

            int checkAngle = pNode._Ports[5].fillValue;

            Vector2 mousePos = MousePosToScreenPos(param.mousePos);//UI屏幕以中心点为0,0点,鼠标以左下角为0,0点,将鼠标位置转换到以屏幕坐标系为准

            if (param.touchType == ETouchType.Begin)
            {
                ms_TouchBeginPos = MousePosToScreenPos(param.mousePos);
            }
            else if (param.touchType == ETouchType.Move)
            {
                Vector2 endPos = MousePosToScreenPos(param.mousePos);
                Vector2 sourceDirection = new Vector2(endPosX, endPosY) - new Vector2(startPosX, startPosY);
                Vector2 destinationDirection = endPos - ms_TouchBeginPos;

                float angle = Vector2.Angle(sourceDirection, destinationDirection);
                //float dot = Vector2.Dot(sourceDirection.normalized, destinationDirection.normalized);
                //float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                //if (TopGame.Core.DebugConfig.bGuideLogEnable)
                //Framework.Plugin.Logger.Warning("angle=" + angle + ",m_TouchBeginPos:" + m_TouchBeginPos + ",param.mousePos:" + endPos + ",setStart:" + new Vector2(startPosX, startPosY) + ",setEnd:" + new Vector2(endPosX, endPosY));
                if (angle <= checkAngle)
                {
                    return true;
                }

                return false;
            }
            return false;
        }
        //------------------------------------------------------
        private static bool WaitGameobjectActive(StepNode pNode)
        {
            GuideGuid guide = GuideGuidUtl.FindGuide(pNode._Ports[0].fillValue, pNode._Ports[1].fillStrValue);
            if (guide == null)//找不到情况,结束当前等待
            {
                return true;
            }
            bool bActive = guide.gameObject.activeInHierarchy;
            bool waitState = pNode._Ports[1].fillValue == 1;
            return bActive == waitState;
        }
        //------------------------------------------------------
        static PointerEventData ms_PointerEventData = null;
        private static bool WaitGameobjectCanClick(StepNode pNode)
        {
            GuideGuid guide = GuideGuidUtl.FindGuide(pNode._Ports[0].fillValue, pNode._Ports[1].fillStrValue);
            if (guide == null)//找不到情况,结束当前等待
            {
                return true;
            }

            Vector3 screenPos = Vector2.zero;
            Camera cam = Camera.main;
            if (cam != null)
            {
                screenPos = cam.WorldToScreenPoint(guide.transform.position);
            }

            if (ms_PointerEventData == null)
            {
                if (UnityEngine.EventSystems.EventSystem.current == null)
                {
                    return false;
                }
                ms_PointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            }

            ms_PointerEventData.position = screenPos;

            List<RaycastResult> results = ListPool<RaycastResult>.Get();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(ms_PointerEventData, results);
            int count = results.Count;
            ListPool<RaycastResult>.Release(results);
            return count > 0;
        }
    }
}
