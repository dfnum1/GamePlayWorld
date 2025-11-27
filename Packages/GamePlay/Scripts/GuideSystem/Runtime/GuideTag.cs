/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideGuid
作    者:	HappLI
描    述:	引导GUID 生成器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Guide
{

    public class GuideTag : MonoBehaviour
    {
        static HashSet<GuideTag> ms_Tags = null;
        //------------------------------------------------------
        public string GetTag()
        {
            return this.name;
        }
        //------------------------------------------------------
        private void Awake()
        {
            if (ms_Tags == null)
                ms_Tags = new HashSet<GuideTag>(8);
            ms_Tags.Add(this);
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            if (ms_Tags == null)
                ms_Tags = new HashSet<GuideTag>(8);
            ms_Tags.Add(this);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            if (ms_Tags == null)
                ms_Tags = new HashSet<GuideTag>(8);
            ms_Tags.Remove(this);
        }
        //------------------------------------------------------
        public static GuideTag GetTag(GuideGuid guide)
        {
            if (guide == null)
                return null;

            if (ms_Tags == null)
                return null;

            Transform guideTrans = guide.transform;

            GuideTag findTag = null;
            int minLayer = int.MaxValue;
            foreach (var tagNode in ms_Tags)
            {
                int layerCnt = GetParentLayerCnt(guideTrans, tagNode.transform);
                if (layerCnt >= 0)
                {
                    if (findTag == null || layerCnt < minLayer)
                    {
                        minLayer = layerCnt;
                        findTag = tagNode;
                    }
                    if (layerCnt == 0)
                    {
                        break;
                    }
                }
            }
            return findTag;
        }
        //------------------------------------------------------
        public static int GetParentLayerCnt(Transform src, Transform dst)
        {
            int layerCnt = 0;
            Transform findNode = src;
            while (findNode != null)
            {
                if (findNode == dst)
                {
                    return layerCnt;
                }
                findNode = findNode.parent;
                layerCnt++;
            }
            return -1;
        }
    }
}
