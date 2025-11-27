/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UISerialize
作    者:	
描    述:	UI 序列化对象容器，用于界面绑定操作对象
*********************************************************************/
using UnityEngine;

namespace Framework.Guide
{
    public class UIGuideSerialize : MonoBehaviour
    {
        public Transform[] Fingers = new Transform[4]; //EFingerType.None
        public Transform[] DescBgs = new Transform[1]; //EDescBGType.None
        public Framework.Guide.GuideHighlightMask BgMask;
        public UIPenetrate uiPenetrate;
        public Transform TargetContainer;

        public Canvas Canvas;

        public Transform GuideAvatarTip;
        public string defaultAvatarFile = "NoviceGuide_Img_King_Small_Texture";
        public UnityEngine.UI.RawImage Avatar;
        public TMPro.TMP_Text AvatarTitleLabel;
        public TMPro.TMP_Text AvatarTipLabel;
        public UnityEngine.RectTransform DialogArrow;

        public Transform GuideDescWidget;
        public UnityEngine.UI.Image GuideImage;
        public UnityEngine.UI.Text GuideText;

        public UnityEngine.UI.Image ContinueImage;
        public UnityEngine.UI.Image SkipBtn;
        public UnityEngine.RectTransform SimulationClickImage;
    }
}
