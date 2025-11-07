/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ADynamicHudBarManager
作    者:	HappLI
描    述:	对象头顶条管理模块
*********************************************************************/
using Framework.Core;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.UI
{
    public abstract class ADynamicHudBarManager : AModule
    {
        private readonly int POOL_MAX = 4;

        Dictionary<int, HudBar> m_vData;

        List<int> m_vDestroying = null;
        Dictionary<int, Queue<HudBar>> m_vRecycle = null;
        ObjectPool<HudBar> m_vPools = null;

        private bool m_bEnable = true;
        private uint m_nHudEnableFlags = 0xffffffff;
        private uint m_nHudEnableAttackGroupFlags = 0xffffffff;
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_vPools = new ObjectPool<HudBar>(16);
            m_vData = new Dictionary<int, HudBar>(16);
            m_vRecycle = new Dictionary<int, Queue<HudBar>>(2);
            m_vDestroying = new List<int>(16);
        }
        //------------------------------------------------------
        public void Enable(bool bEnable)
        {
            m_bEnable = bEnable;
            if (bEnable)
            {
                m_nHudEnableFlags = 0xffffffff;
                m_nHudEnableAttackGroupFlags = 0xffffffff;
            }
            if (!m_bEnable)
            {
                Destroy();
            }
        }
        //------------------------------------------------------
        public void EnableAttackGroupHud(byte attackGroup, bool bEnable)
        {
            if (bEnable) m_nHudEnableAttackGroupFlags |= (uint)((1 << (int)attackGroup));
            else m_nHudEnableAttackGroupFlags &= ~(uint)((1 << (int)attackGroup));
        }
        //------------------------------------------------------
        public bool IsEnableAttackGroupHud(byte attackGroup)
        {
            if (!m_bEnable) return false;
            return (m_nHudEnableAttackGroupFlags & (uint)((1 << (int)attackGroup))) != 0;
        }
        //------------------------------------------------------
        public void EnableHud(EActorType actorType, bool bEnable)
        {
            if (bEnable) m_nHudEnableFlags |= (uint)((1 << (int)actorType));
            else m_nHudEnableFlags &= ~(uint)((1 << (int)actorType));
        }
        //------------------------------------------------------
        public bool IsEnableHud(EActorType actorType)
        {
            if (!m_bEnable) return false;
            return (m_nHudEnableFlags & (uint)((1 << (int)actorType))) != 0;
        }
        //------------------------------------------------------
        protected abstract int GetHurBarAssetType(Actor pActor);
        protected abstract GameObject GetHudBarGameobject(int type);
        //------------------------------------------------------
        public void OnActorAttrChange(Actor pActor, byte attrType, float fValue, float oldValue)
        {
            if (!IsEnableHud(pActor.GetActorType())) return;
            if (!pActor.IsEnableHudBar())
                return;

            if (Mathf.Abs(fValue) <= 0.01f) return;

            HudBar hud;
            if (m_vData.TryGetValue(pActor.GetInstanceID(), out hud))
            {
                if (hud != null)
                {
                    hud.Show(attrType, fValue, oldValue);
                }
                return;
            }

            int assetType = GetHurBarAssetType(pActor);
            Queue<HudBar> vPools;
            if (m_vRecycle.TryGetValue(assetType, out vPools) && vPools.Count > 0)
            {
                hud = vPools.Dequeue();
                hud.SetHudType((byte)assetType);
                hud.SetActor(pActor);

                m_vData.Add(pActor.GetInstanceID(), hud);
                OnCreateHudBar(pActor, hud);
                hud.Show(attrType, fValue, oldValue);
            }
            else
            {
                GameObject pAsset = GetHudBarGameobject(assetType);
                if (pAsset == null) return;

                if (pActor == null) return;
                HudBar hudBar;
                if (!m_vData.TryGetValue(pActor.GetInstanceID(), out hudBar))
                {
                    hudBar = MallocHudBar();
                    m_vData.Add(pActor.GetInstanceID(), hudBar);
                }
                if (hudBar.pInstanceOperiaon != null)
                    hudBar.pInstanceOperiaon.Earse();

                hudBar.pInstanceOperiaon = FileSystemUtil.SpawnInstance(pAsset);
                if (hudBar.pInstanceOperiaon == null) return;
                hudBar.SetHudType((byte)assetType);
                hudBar.SetActor(pActor);
                hudBar.pInstanceOperiaon.userData0 = hudBar;
                hudBar.pInstanceOperiaon.OnSign = OnCallSign;
                hudBar.pInstanceOperiaon.OnCallback = OnSpawnCallback;
                OnCreateHudBar(pActor, hudBar);
                hudBar.Show(attrType, fValue, oldValue);
            }
        }
        //------------------------------------------------------
        protected abstract void OnCreateHudBar(Actor pActor, HudBar hudBar);
        //------------------------------------------------------
        void OnCallSign(InstanceOperiaon pCb)
        {
            pCb.bUsed = pCb.userData0 != null;

            HudBar pHudBar = pCb.userData0 as HudBar;
            if(pCb.bUsed)
            {
                Actor pActor = pHudBar.GetActor();
                if (pActor != null)
                {
                    if (pCb.bUsed)
                    {
                        pCb.bUsed = !pActor.IsDestroy() && !pActor.IsKilled() && pActor.IsActived();
                    }
                }
                else pCb.bUsed = false;
            }
            if (!m_bEnable)
                pCb.bUsed = false;
            if (!pCb.bUsed)
            {
                pHudBar.pInstanceOperiaon = null;
                m_vData.Remove(pHudBar.GetInstanceID());
                pHudBar.Destroy();
                FreeHudBar(pHudBar);
            }
        }
        //------------------------------------------------------
        void OnSpawnCallback(InstanceOperiaon pCb)
        {
            if(pCb.pPoolAble != null)
            {
                HudBar hudBar = pCb.userData0 as HudBar;
                ObjectHudBar hud = pCb.pPoolAble as ObjectHudBar;
                if(hudBar!=null)
                    hudBar.pInstanceOperiaon = null;
                if (hudBar == null || hudBar.GetActor() == null || hud == null || pCb.userData0 == null)
                {
                    if(hudBar!=null)
                    {
                        hudBar.pInstanceOperiaon = null;
                        m_vData.Remove(hudBar.GetInstanceID());
                        hudBar.Destroy();
                        FreeHudBar(hudBar);
                    }

                    pCb.pPoolAble.RecyleDestroy();
                    return;
                }
                hud.SetParent(AUIManager.GetAutoUIRoot());
                hud.SetScale(Vector3.one);
                hudBar.Awake(hud);
            }
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            foreach (var db in m_vData)
            {
                db.Value.Destroy();
                FreeHudBar(db.Value);
            }
            m_vData.Clear();

            foreach (var db in m_vRecycle)
            {
                foreach (var hud in db.Value) hud.Destroy();
            }
            m_vRecycle.Clear();
            m_vDestroying.Clear();
            m_nHudEnableFlags = 0xffffffff;
        }
        //------------------------------------------------------
        void FreeHudBar(HudBar hudBar)
        {
            hudBar.Destroy();
            m_vPools.Release(hudBar);
        }
        //------------------------------------------------------
        HudBar MallocHudBar()
        {
            return m_vPools.Get();
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            if (!m_bEnable) return;
            bool bRecyle = false;
            HudBar hudHp;
            foreach (var db in m_vData)
            {
                hudHp = db.Value;
                if (hudHp == null)
                {
                    continue;
                }
                if (!hudHp.IsInited())
                    continue;
                hudHp.Update();
                if (hudHp.IsEnd())
                {
                    bRecyle = false;
                    if(hudHp.GetHudType() != 0xff)
                    {
                        Queue<HudBar> vPools;
                        if (!m_vRecycle.TryGetValue(hudHp.GetHudType(), out vPools))
                        {
                            vPools = new Queue<HudBar>(POOL_MAX);
                            m_vRecycle.Add(hudHp.GetHudType(), vPools);
                        }
                        if (vPools.Count <= POOL_MAX)
                        {
                            vPools.Enqueue(hudHp);
                            bRecyle = true;
                        }
                    }

                    hudHp.Clear();
                    if(!bRecyle)
                    {
                        hudHp.Destroy();
                        FreeHudBar(hudHp);
                    }
                    m_vDestroying.Add(db.Key);
                }
            }
            for(int i = 0; i < m_vDestroying.Count; ++i)
            {
                m_vData.Remove(m_vDestroying[i]);
            }
            m_vDestroying.Clear();
        }
    }
}
