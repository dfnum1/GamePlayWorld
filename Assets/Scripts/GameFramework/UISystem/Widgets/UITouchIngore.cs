/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UITouchIngore
作    者:	
描    述:	UI点击拦截
*********************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI
{
    public class UITouchIngore : MonoBehaviour, UnityEngine.ICanvasRaycastFilter
    {
		bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
		{
			return false;
		}
    }
}
