#if USE_GUIDESYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin.Guide
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
        bool GetIsLoadCompleted();
        T GetComponent<T>();
    }
}
#endif