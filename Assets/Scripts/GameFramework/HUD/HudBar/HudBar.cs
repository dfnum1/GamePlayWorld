/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	HudBar
作    者:	HappLI
描    述:	对象头顶血条
*********************************************************************/
using ExternEngine;
using Framework.Base;
using Framework.Core;
using UnityEngine;

namespace Framework.UI
{
    public class HudBar : IUserData, IActorAttrDirtyCallback
    {
        private bool m_bInited = false;

        private byte m_HudType = 0xff;
        private float m_fDuration = 0;
        Actor m_pActor;
        RectTransform m_pTrans;
        DynamicLoader m_Loader;

        uint m_nSetFlags = 0;
        ObjectHudBar m_pHudHp;
        public InstanceOperiaon pInstanceOperiaon = null;

        //------------------------------------------------------
        public void Awake(ObjectHudBar hudHp)
        {
            m_bInited = true;
            if (m_pHudHp == hudHp) return;
            if (m_pHudHp != null) m_pHudHp.RecyleDestroy();
             m_pHudHp = hudHp;
            m_Loader = new DynamicLoader();
            m_pTrans = null;
            m_nSetFlags = 0;
            if (m_pHudHp)
            {
                m_pTrans = m_pHudHp.GetTransorm() as RectTransform;
            }
            ClearHudBars();
        }
        //------------------------------------------------------
        public void SetActor(Actor pActor)
        {
            if (m_pActor == pActor) return;
            if (m_pActor != null)
            {
                m_pActor.RemoveCallback(this);
            }
            m_nSetFlags = 0;
            m_pActor = pActor;
            if (m_pActor != null)
                m_pActor.AddCallback(this);
        }
        //------------------------------------------------------
        public Actor GetActor()
        {
            return m_pActor;
        }
        //------------------------------------------------------
        public int GetInstanceID()
        {
            if (m_pActor == null) return 0;
            return m_pActor.GetInstanceID();
        }
        //------------------------------------------------------
        public bool IsInited()
        {
            return m_bInited;
        }
        //------------------------------------------------------
        public void SetHudType(byte type)
        {
            m_HudType = type;
        }
        //------------------------------------------------------
        public byte GetHudType()
        {
            return m_HudType;
        }
        //------------------------------------------------------
        public void Show(byte attriType, FFloat newValue, FFloat oldValue)
        {
            if (m_pHudHp == null) return;
            m_fDuration = m_pHudHp.fShowDuration;

            if (m_pActor == null)
                return;
            for (int i = 0; i < m_pHudHp.bindAttriDatas.Length; ++i)
            {
                var attri = m_pHudHp.bindAttriDatas[i];
                if (attri.hudBar == null)
                    continue;
                if (attri.baseAttriType == attriType || attri.maxAttriType == attriType)
                {
                    if (attri.showRate)
                    {
                        if (attriType == attri.maxAttriType)
                        {
                            newValue = m_pActor.GetAttr(attri.baseAttriType) / Mathf.Max(1,newValue);
                            oldValue = m_pActor.GetAttr(attri.baseAttriType) / Mathf.Max(1,oldValue);
                        }
                        else
                        {
                            newValue = newValue / Mathf.Max(1, m_pActor.GetAttr(attri.maxAttriType));
                            oldValue = oldValue / Mathf.Max(1, m_pActor.GetAttr(attri.maxAttriType));
                        }
                    }
                    attri.hudBar.SetValue(attriType, newValue, oldValue, IsFirstSet(i));
                }
            }
        }
        //------------------------------------------------------
        void ClearHudBars()
        {
            m_nSetFlags = 0;
            if (m_pHudHp == null || m_pHudHp.bindAttriDatas == null)
                return;
            for (int i = 0; i < m_pHudHp.bindAttriDatas.Length; ++i)
            {
                var attri = m_pHudHp.bindAttriDatas[i];
                if (attri.hudBar == null)
                    continue;
                attri.hudBar.Clear();
            }
        }
        //------------------------------------------------------
        bool IsFirstSet(int index)
        {
            return (m_nSetFlags & (1 << index)) == 0;
        }
        //------------------------------------------------------
        void UpdateSetFlags(int index)
        {
            m_nSetFlags |= (uint)(1 << index);
        }
        //------------------------------------------------------
        public void Destroy()
        {
            if (m_pActor != null)
            {
                m_pActor.RemoveCallback(this);
            }
            m_pActor = null;
            if (m_pHudHp != null) m_pHudHp.RecyleDestroy();
            m_pHudHp = null;
            m_bInited = false;
            m_nSetFlags = 0;
            if (pInstanceOperiaon != null) pInstanceOperiaon.Earse();
            pInstanceOperiaon = null;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_pActor = null;
            if (m_pTrans) m_pTrans.localPosition = Vector3.one * 1000;

            ClearHudBars();
            if (m_Loader!=null) m_Loader.ClearLoaded();

            if (pInstanceOperiaon != null) pInstanceOperiaon.Earse();
            pInstanceOperiaon = null;
        }
        //------------------------------------------------------
        public bool IsEnd()
        {
            return m_fDuration <= 0 || m_pActor == null || m_pActor.IsKilled() || !m_pActor.IsActived();
        }
        //------------------------------------------------------
        public void Update()
        {
            if (!m_bInited) return;
            if (m_pActor == null || CameraController.getInstance() == null) return;
            m_fDuration -= Time.deltaTime;
            if (m_fDuration>0)
            {
                if (m_pTrans && !m_pActor.IsKilled() && !m_pActor.IsDestroy())
                {
                    Vector3 worldPos = m_pActor.GetPosition();// + Vector3.up * m_pActor.GetModelHeight();
                    if(!CameraUtil.IsInView(worldPos, 0))
                    {
                        m_pTrans.localPosition = ConstDef.INVAILD_POS;
                        m_fDuration = 0;
                        m_pActor = null;
                        return;
                    }

                    if (!m_pActor.IsVisible() || !m_pActor.IsActived())
                    {
                        m_pTrans.localPosition = ConstDef.INVAILD_POS;
                    }
                    else
                    {
                        if (AFramework.mainFramework != null && AFramework.mainFramework.uiManager!=null)
                        {
                            Vector3 uiguiPos = Vector3.zero;
                            AFramework.mainFramework.uiManager.ConvertWorldPosToUIPos(worldPos, true, ref uiguiPos);
                            m_pTrans.localPosition = uiguiPos;
                        }
                    }
                }
                else
                {
                    if(m_pTrans)
                        m_pTrans.localPosition = ConstDef.INVAILD_POS;
                    m_fDuration = 0;
                    m_pActor = null;
                }
            }
            else
            {
                m_fDuration = 0;
                m_pActor = null;
                if(m_pTrans) m_pTrans.localPosition = Vector3.one * 1000;
            }
        }    
        //------------------------------------------------------
        public void OnActorAttrDirty(Actor pActor, byte type, float oldValue, float newValue)
        {
            Show(type, newValue, oldValue);
        }
    }
}
