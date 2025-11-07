/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	AModule
作    者:	HappLI
描    述:	基础模块类
*********************************************************************/
using ExternEngine;

namespace Framework.Core
{
    public abstract class AModule : IUserData
    {
        protected AFramework m_pFramework;
        public void Init(AFramework pFramwork)
        {
            if (m_pFramework == pFramwork)
                return;
            m_pFramework = pFramwork;
            OnInit();
        }
        //-------------------------------------------------
        public void Awake()
        {
            OnAwake();
        }
        //-------------------------------------------------
        public void Start()
        {
            OnStart();
        }
        //-------------------------------------------------
        public void Update(float fFrame)
        {
            OnUpdate(fFrame);
        }
        //-------------------------------------------------
        protected virtual void OnUpdate(float fFrame) { }
        //-------------------------------------------------
        public AFramework GetFramework()
        {
            return m_pFramework;
        }
        //-------------------------------------------------
        protected virtual void OnStart() { }
        //-------------------------------------------------
        protected virtual void OnInit() { }
        protected virtual void OnAwake() { }
        //-------------------------------------------------
        public virtual void OnClearWorld() { }
        //-------------------------------------------------
        public void Destroy()
        {
            OnDestroy();
        }
        //-------------------------------------------------
        protected virtual void OnDestroy() { }
    }
}
