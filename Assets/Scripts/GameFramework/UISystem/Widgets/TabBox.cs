/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	TabDock
作    者:	HappLI
描    述:	元素位置布局
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    public class TabBox : MonoBehaviour
    {
        public class Item
        {
            public TabBox tabBox;
            //public AInstanceAble pAble;

            //public virtual void Start() { }
            //public virtual void OnAwake(InstanceOperiaon pCallback)
            //{
            //    pAble = pCallback.pPoolAble;
            //    pCallback.pPoolAble.SetParent(tabBox.transform);
            //    pAble.GetTransorm().localScale = Vector3.one;
            //    pCallback.pPoolAble.SetPosition(Vector3.zero, true);
            //    if (pAble) Start();
            //}

            public virtual void Update(float fFrame)
            {

            }

            public virtual void Destroy(bool isRealDestory = false)
            {
                //if (pAble != null)
                //{
                //    if (isRealDestory)
                //    {
                //        GameObject.Destroy(pAble.gameObject);
                //    }
                //    else
                //    {
                //        pAble.RecyleDestroy(10);
                //    }
                //}

                //pAble = null;
            }
        }
        public enum EType : byte
        {
            File,
            Prefab,
        }
        public EType type = EType.File;
        public string strPrefabFile = "";
        public GameObject refPrefab = null;

        public int nTabCount = 0;
        List<Item> m_vItems = null;
        //public T AddTab<T>() where T : Item, new()
        //{
        //    if (m_vItems == null) m_vItems = new List<Item>(nTabCount);
        //    InstanceOperiaon pCb = null;
        //    T item = new T();
        //    item.tabBox = this;
        //    if (type == EType.File)
        //    {
        //        pCb =FileSystemUtil.SpawnInstance(strPrefabFile, true);
        //    }
        //    else
        //        pCb =FileSystemUtil.SpawnInstance(refPrefab);
        //    if (pCb != null)
        //    {
        //        pCb.OnCallback = item.OnAwake;
        //    }

        //    m_vItems.Add(item);
        //    return item;
        //}
        //------------------------------------------------------
        void Update()
        {
            if (m_vItems == null) return;
            for (int i = 0; i < m_vItems.Count; ++i)
                m_vItems[i].Update(Time.deltaTime);
        }
        //------------------------------------------------------
        public void DelTab(int index)
        {
            if (m_vItems == null || index < 0 || index >= m_vItems.Count) return;
            m_vItems[index].Destroy();
            m_vItems.RemoveAt(index);
        }
        //------------------------------------------------------
        public void DelTab(Item pItem)
        {
            if (m_vItems == null || pItem == null) return;
            pItem.Destroy();
            m_vItems.Remove(pItem);
        }
        //------------------------------------------------------
        public void Clear(bool isRealDestory = false)
        {
            if (m_vItems == null) return;
            for (int i = 0; i < m_vItems.Count; ++i)
                m_vItems[i].Destroy(isRealDestory);
            m_vItems.Clear();
        }
        //------------------------------------------------------
        public Transform GetItemByIndex(int index)
        {
            if (m_vItems == null || m_vItems.Count <= index)
            {
                return null;
            }
            // return m_vItems[index].pAble.transform;
            return null;
        }
        //------------------------------------------------------
        public int GetIndexByItem(GameObject go)
        {
            if (m_vItems == null) return -1;
            //for(int i=0;i < m_vItems.Count;i++)
            //{
            //    Item item = m_vItems[i];
            //    if (item.pAble.gameObject == go)
            //    {
            //        return i;
            //    }
            //}
            return -1;
        }
        //------------------------------------------------------
        public bool GetIsLoadCompleted()
        {
            return true;
        }
    }
}
