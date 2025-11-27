/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	IGuideScroll
作    者:	HappLI
描    述:	列表接口
*********************************************************************/
using UnityEngine;

namespace Framework.Guide
{
    public interface IGuideScroll
    {
        Transform GetItemByIndex(int index);
        /// <summary>
        /// 根据go查找索引
        /// 从0开始
        /// </summary>
        /// <param name="go"></param>
        /// <returns>找不到返回-1</returns>
        int GetIndexByItem(GameObject go);
        void ScrollToIndex(int index, float lerpTime = 0.0f);
    }
}
