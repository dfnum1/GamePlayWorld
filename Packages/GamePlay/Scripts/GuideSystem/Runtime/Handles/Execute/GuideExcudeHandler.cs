/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideStepHandler
作    者:	HappLI
描    述:	内置步骤操作句柄
*********************************************************************/
using UnityEngine;

namespace Framework.Guide
{
	public class GuideExecudeHandler
	{
		public static bool OnGuideExecuteNode(ExcudeNode pNode)
		{
			var guidePanel = GuideSystem.getInstance().GetGuidePanel();
			if (guidePanel == null)
				return false;

			switch((GuideExcudeType)pNode.type)
			{
				case GuideExcudeType.MaskAble:
                    {
                        ExcudeMaskNode(pNode, guidePanel);
                        guidePanel.bDoing = true;
                    }
					return true;
				case GuideExcudeType.Tips:
                    {
                        ExcudeTipsNode(pNode, guidePanel);
                        guidePanel.bDoing = true;
                    }
					return true;
				case GuideExcudeType.TipsByGUI:
                    {
                        ExcudeTipsNodeByGUI(pNode, guidePanel);
                        guidePanel.bDoing = true;
                    }
					return true;
                case GuideExcudeType.SetGuideOger:
                    {
                        guidePanel.SetOrder(pNode._Ports[0].fillValue);
                        guidePanel.bDoing = true;
                    }
                    return true;
                default:
                    return false;
            }

            return false;
		}
        //-------------------------------------------
        public static void ExcudeMaskNode(ExcudeNode pNode, GuidePanel guidePanel)
        {
            if (pNode._Ports == null || pNode._Ports.Count == 0)
            {
                Debug.LogError("引导组:" + pNode.guideGroup.Guid + ",节点:" + pNode.Guid + ",的端口数据为空!");
                return;
            }
            //显示引导界面
            //打开遮罩
            guidePanel.Show();
            int isMask = pNode._Ports[0].fillValue;
            Color maskColor = pNode._Ports[1].ToColor();
            bool penetrateEnable  = pNode._Ports[2].fillValue == 1;
            int shapeType =  pNode._Ports[3].fillValue;
            Vector2 scale = pNode._Ports[4].ToVec2();
            int TargetGuid  = pNode._Ports[5].fillValue;
            string TargetTag  = pNode._Ports[6].fillStrValue;
            float speed  = pNode._Ports[7].fillValue * 0.001f;
            if (guidePanel != null)
            {
                guidePanel.SetMaskSpeed(speed);
                guidePanel.SetMaskShapeScale(scale);
                guidePanel.SetMaskShape((EMaskType)shapeType);
                guidePanel.SetMaskActive(isMask == 1);
                guidePanel.SetMaskColor(maskColor);
                guidePanel.SetPenetrateEnable(penetrateEnable, TargetGuid, TargetTag);
#if UNITY_EDITOR
                if(!GuideSystem.getInstance().bDoing && guidePanel.IsEditorPreview && pNode.pNext!=null && pNode.pNext is StepNode)
                {
                    StepNode stepNode = pNode.pNext as StepNode;
                    if(stepNode.type ==(int)GuideStepType.ClickUI)
                    {
                        int guid = stepNode._Ports[0].fillValue;
                        Transform guideGuid = GuideGuidUtl.GetWidget(guid, stepNode._Ports[1].fillStrValue, stepNode._Ports[2].fillValue-1);
                        if(guideGuid) guidePanel.BgHighlightMask?.SetTarget(guideGuid as RectTransform);
                    }
                }
#endif
            }
        }
        //-------------------------------------------
        public static void ExcudeTipsNode(ExcudeNode pNode, GuidePanel guidePanel)
        {
            int bgType = pNode._Ports[0].fillValue;
            string titleID = pNode._Ports[1].fillStrValue;
            string contentID = pNode._Ports[2].fillStrValue;
            var color = pNode._Ports[3].ToColor();
            bool is3D = pNode._Ports[4].fillValue == 1;

            Vector3 pos = new Vector3(pNode._Ports[5].fillValue, pNode._Ports[6].fillValue, pNode._Ports[7].fillValue);
            bool isTransition = pNode._Ports[8].fillValue == 1;
            int speed = pNode._Ports[9].fillValue;
            bool enableArrow = pNode._Ports[10].fillValue == 1;
            bool enableAvatar = pNode._Ports[11].fillValue == 1;
            string avatarFile = pNode._Ports[12].fillStrValue;

            guidePanel.Show();
            guidePanel.AddTipDock((EDescBGType)bgType, GuideSystem.getInstance().ConvertLanguage(titleID), GuideSystem.getInstance().ConvertLanguage(contentID),
                color, isTransition ? speed : 0, 0, pos, is3D, 0, "",0, "", enableArrow);
            guidePanel.SetAvatarEnable(enableAvatar, avatarFile);
        }
        //-------------------------------------------
        public static void ExcudeTipsNodeByGUI(ExcudeNode pNode, GuidePanel guidePanel)
        {
            int bgType = pNode._Ports[0].fillValue;
            string tilteID = pNode._Ports[1].fillStrValue;
            string contentID = pNode._Ports[2].fillStrValue;
            Color color = pNode._Ports[3].ToColor();
            int guid = pNode._Ports[4].fillValue;
            string widgetTag = pNode._Ports[5].fillStrValue;
            int widgetIndex = pNode._Ports[6].fillValue;
            string searchListenerName = pNode._Ports[7].fillStrValue;
            float offsetX = pNode._Ports[8].fillValue;
            float offsetY = pNode._Ports[9].fillValue;
            bool isTransition = pNode._Ports[10].fillValue == 1;
            float speed = pNode._Ports[11].fillValue;
            int autoHideTime = pNode._Ports[12].fillValue;
            //设置对话箭头状态
            bool enableArrow = pNode._Ports[13].fillValue == 1;
            //人物背景
            bool enableAvatar = pNode._Ports[14].fillValue == 1;
            string avatarFile = pNode._Ports[15].fillStrValue;

            guidePanel.Show();
            guidePanel.AddTipDock((EDescBGType)bgType, GuideSystem.getInstance().ConvertLanguage(tilteID), GuideSystem.getInstance().ConvertLanguage(contentID), 
                color, isTransition ? speed : 0, autoHideTime, new Vector3(offsetX, offsetY, 0), false, guid, widgetTag, widgetIndex, searchListenerName, enableArrow);
            guidePanel.SetAvatarEnable(enableAvatar, avatarFile);
        }
    }
}
