/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIView
作    者:	HappLI
描    述:	UI视图基类
*********************************************************************/

using UnityEngine;

namespace Framework.UI
{
    public abstract class UIView : Core.ADyanmicLoader, IUserData
    {
        protected UIBase m_pBaseUI = null;
        public virtual void Init(UIBase pBase) { m_pBaseUI = pBase; }
        public virtual void Awake() { }
        public virtual void Show() { }
        public virtual void Hide() { }
        //------------------------------------------------------
        public void Clear(bool bUnloadDynamic = true)
        {
            if(bUnloadDynamic)
                ClearLoaded();
            OnClear();
        }
        //------------------------------------------------------
        protected virtual void OnClear() { }
        //------------------------------------------------------
        public void Destroy()
        {
            ClearLoaded();
            OnDestroy();
        }
        //------------------------------------------------------
        protected virtual void OnDestroy() { }
        //------------------------------------------------------
        protected override bool LoadSignCheck()
        {
            return m_pBaseUI != null && m_pBaseUI.IsInstanced();// && m_pBaseUI.IsVisible();
        }
        //------------------------------------------------------
        protected override bool LoadInstanceSignCheck()
        {
            return m_pBaseUI != null && m_pBaseUI.IsInstanced() && m_pBaseUI.IsVisible();
        }
        //------------------------------------------------------
        protected override void OnSpawInstanced(Core.AInstanceAble pAble)
        {
            //var controller = pAble as Core.ParticleController;
            //if(controller)
            //{
            //    controller.bIsDisableCheck = true;
            //    controller.SetRenderOrder(m_pBaseUI.GetOrder(), true);
            //}
        }
        //------------------------------------------------------
        public T GetWidget<T>(string name) where T : Component
        {
            if (m_pBaseUI == null) return null;
            return m_pBaseUI.GetWidget<T>(name);
        }
        //------------------------------------------------------
        public virtual void Update(float fFrame) { }

    }
}
