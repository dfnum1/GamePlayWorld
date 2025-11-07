/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ADynamicTextManager
作    者:	HappLI
描    述:	飘字管理器
*********************************************************************/
using Framework.Base;
using Framework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public abstract class ADynamicTextManager : RecycleAble<ADynamicTextManager.DynamicText>
    {
        public class DynamicText
        {
            public AInstanceAble pInstance;
            public AWorldNode pActor;
            public Vector3 popPosition;
            public Vector3 endPosition;
            public string strText;
            public string imgPath;
            public float Vaule;
            public int CreateTime;
            public int assetType;
            public Color color;
            Framework.RtgTween.TextPopEffecter m_pEffector;

            bool bLoaded = false;

            public void OnSign(InstanceOperiaon pOp)
            {
                pOp.bUsed = true;
            }
            public void OnSpawnCallback(InstanceOperiaon pOp)
            {
                bLoaded = true;
                pInstance = pOp.pPoolAble;
                if (pInstance != null)
                {
                    UI.AUIManager mgr = AUIManager.getInstance();
                    Vector3 spawnPos = Vector3.zero;
                    mgr.ConvertWorldPosToUIPos(popPosition, true, ref spawnPos);
                    m_pEffector = pInstance.GetComponent<Framework.RtgTween.TextPopEffecter>();

                    if (!pInstance.gameObject.activeSelf)
                        pInstance.gameObject.SetActive(true);

                    if (m_pEffector != null)
                    {
                        Vector3 uiScaler = Vector3.one;
                        AUIManager.ScaleWithScreenScale(ref uiScaler);

                        //m_pEffector.SetColor(color);
                        m_pEffector.SetText(strText);
                        m_pEffector.SetImg(imgPath);
                        Vector3 startOffset = Vector3.zero;
                        if (m_pEffector.bRandomStart)
                        {
                            startOffset = new Vector3(UnityEngine.Random.Range(-m_pEffector.nStartXRandomRange, m_pEffector.nStartXRandomRange) * uiScaler.x,
                               UnityEngine.Random.Range(0, m_pEffector.nStartYRandomRange) * uiScaler.y, 0);
                            if (m_pEffector.bLocal)
                            {
                                m_pEffector.SetRuntimeStartPos(startOffset);
                            }
                            else
                            {
                                m_pEffector.SetRuntimeStartPos(new Vector3(spawnPos.x + startOffset.x, spawnPos.y + startOffset.y, spawnPos.z + startOffset.z));
                            }
                        }
                        if (m_pEffector.bRandomEnd)
                        {
                            if(m_pEffector.bLocal)
                               m_pEffector.SetRuntimeEndPos(startOffset + new Vector3(UnityEngine.Random.Range(-m_pEffector.nRandomRange, m_pEffector.nRandomRange) * uiScaler.x, UnityEngine.Random.Range(0, m_pEffector.nRandomRange) * uiScaler.y ,0));
                            else
                               m_pEffector.SetRuntimeEndPos(startOffset + new Vector3(spawnPos.x + UnityEngine.Random.Range(-m_pEffector.nRandomRange, m_pEffector.nRandomRange) * uiScaler.x, spawnPos.y + UnityEngine.Random.Range(0, m_pEffector.nRandomRange) * uiScaler.y , spawnPos.z));
                        }
                        if (endPosition != Vector3.zero)
                        {
                            Vector3 endPos = Vector3.zero;
                            Vector3 screenPos = mgr.ConvertUIPosToScreen(endPosition);//世界坐标转屏幕坐标

                            Vector2 local_temp = Vector2.zero;
                            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mgr.GetRoot() as RectTransform, screenPos, mgr.GetUICamera(), out local_temp))
                            {
                                endPos.x = local_temp.x;//屏幕坐标转本地坐标
                                endPos.y = local_temp.y;
                            }

                            m_pEffector.SetRuntimeEndPos(endPos);
                        }
                        m_pEffector.Play(spawnPos);
                    }


                    
                }
                else
                    m_pEffector = null;

            }

            public bool Update(float fFrame)
            {
                if (!bLoaded) return false;
                if (m_pEffector == null) return true;
                return m_pEffector.bEnd();
            }

            public void Destroy()
            {
                if (pInstance)
                {
                    Core.AComSerialized serize = pInstance.GetComponent<Core.AComSerialized>();
                    if (serize)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            RectTransform rect = serize.GetWidget<RectTransform>("element" + i);
                            if (rect)
                                rect.gameObject.SetActive(false);
                        }
                    }
                }

                if (pInstance)
                    FileSystemUtil.DeSpawnInstance(pInstance, 10);
                pInstance = null;
                pActor = null;
                strText = null;
                imgPath = null;
                m_pEffector = null;
                bLoaded = false;
                endPosition = Vector3.zero;
            }
        }
        LinkedList<DynamicText> m_vDynamics = null;
        public ADynamicTextManager()
        {
            m_vDynamics = new LinkedList<DynamicText>();
        }
        //------------------------------------------------------
        public void Destroy()
        {
            for(var node = m_vDynamics.First; node != null; )
            {
                var next = node.Next;
                node.Value.Destroy();
                m_vRecycle.Release(node.Value);
                node = next;
            }
            m_vDynamics.Clear();
        }
        //------------------------------------------------------
        public virtual void PopText(AWorldNode pActor, uint nContent, IUserData externVarial = null)
        {
            uint strId = nContent;
            string iconPath = null;

            GameObject pAsset = GetTypeAsset<GameObject>(-1);
            InstanceOperiaon pCb = FileSystemUtil.SpawnInstance(pAsset);
            if (pCb == null) return;
            pCb.pByParent = AUIManager.GetAutoUIRoot();

            DynamicText pDt = m_vRecycle.Get();
            pDt.Destroy();
            pDt.pActor = pActor;
            pDt.popPosition = pActor.GetPosition() + Vector3.up * pActor.GetBounds().GetSize().y;
         
            pDt.strText = LocalizationUtil.Convert(strId);
            pDt.imgPath = iconPath;
            pCb.OnCallback = pDt.OnSpawnCallback;
            pCb.OnSign = pDt.OnSign;

            m_vDynamics.AddLast(pDt);
        }
        //------------------------------------------------------
        protected abstract int GetAttrAssetType(byte attrType = byte.MaxValue);
        //------------------------------------------------------
        protected abstract UnityEngine.Object GetTypeAssetObject(int assetType);
        //------------------------------------------------------
        T GetTypeAsset<T>(int assetType) where T : UnityEngine.Object
        {
            var assetObj = GetTypeAssetObject(assetType);
            if (assetObj == null) return null;
            return assetObj as T;
        }
        //------------------------------------------------------
        public virtual void OnActorAttrChange(Actor pActor, byte attrType, float oldValue, float newValue, IUserData externVarial = null)
        {
            if (Mathf.Abs(newValue-oldValue) <= 0.01f) return;

            int assetType = GetAttrAssetType(attrType);
            GameObject pAsset = GetTypeAsset<GameObject>(assetType);
            if (pAsset == null) return;

            InstanceOperiaon pCb = FileSystemUtil.SpawnInstance(pAsset);
            if (pCb == null) return;
            pCb.pByParent = AUIManager.GetAutoUIRoot();
            int fValue = (int)(newValue - oldValue);

            string tempStrValue = fValue.ToString();

            DynamicText pDt = m_vRecycle.Get();
            pDt.Destroy();
            //pDt.color = Color.white;
            pDt.popPosition = pActor.GetPosition() + Vector3.up * (pActor.GetBounds().GetSize()).y;

            if (!CameraUtil.IsInView(pDt.popPosition,-0.1f))
            {
                if (CameraUtil.mainCamera)
                {
                    Vector3 screen = CameraUtil.mainCamera.WorldToScreenPoint(pDt.popPosition);
                    pDt.popPosition = CameraUtil.mainCamera.ScreenToWorldPoint(new Vector3(screen.x * 0.9f, screen.y * 0.9f, screen.z * 0.9f));
                }
            }

            pDt.pActor = pActor;
            pDt.strText = tempStrValue;
            pDt.Vaule = fValue;
            pDt.assetType = assetType;
            pDt.CreateTime = DateTime.Now.Second;
            pCb.OnCallback = pDt.OnSpawnCallback;
            pCb.OnSign = pDt.OnSign;

            m_vDynamics.AddLast(pDt);
        }
        //------------------------------------------------------
        public void Update(float fFrameTime)
        {
            for(var node = m_vDynamics.First; node != null; )
            {
                var next = node.Next;
                if (node.Value.Update(fFrameTime))
                {
                    node.Value.Destroy();
                    m_vRecycle.Release(node.Value);
                    m_vDynamics.Remove(node);
                }
                node = next;
            }
        }
    }
}
