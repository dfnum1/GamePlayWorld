/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	UISystem
作    者:	HappLI
描    述:	UI系统配置模块
*********************************************************************/
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
    public class UISystem : MonoBehaviour
    {
        public UIConfig uiConfig;
        public Camera uiCamera;
        public Canvas rootCanvas;
        public CanvasScaler canvasScaler;
        public RectTransform[] dynamicRoots;
        public RectTransform staticRoot;
    }
}
