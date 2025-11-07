#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	AActorAgent
作    者:	HappLI
描    述:	Actor 逻辑行为基类
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public abstract class AActorAgent : TypeObject
    {
        protected Actor m_pActor;
        //--------------------------------------------------------
        public AActorAgent()
        {
            m_pActor = null;
        }
        //--------------------------------------------------------
        public AActorAgent(Actor pActor)
        {
            m_pActor = pActor;
        }
        //--------------------------------------------------------
        public Actor GetActor()
        {
            return m_pActor;
        }
        //--------------------------------------------------------
        public void SetActor(Actor pActor)
        {
            m_pActor = pActor;
        }
        //--------------------------------------------------------
        public AInstanceAble GetAble()
        {
            if (m_pActor == null) return null;
            return m_pActor.GetObjectAble();
        }
        //--------------------------------------------------------
        public T GetAgent<T>(bool bAutoCreate = false) where T : AActorAgent, new()
        {
            if (m_pActor == null) return null;
            return m_pActor.GetAgent<T>(bAutoCreate);
        }
        //--------------------------------------------------------
        public T GetComponent<T>(bool bFindChild = false) where T : Component
        {
            if (m_pActor == null) return null;
            return m_pActor.GetComponent<T>(bFindChild);
        }
        //--------------------------------------------------------
        public void Init()
        {
            OnInit();
        }
        //--------------------------------------------------------
        protected virtual void OnInit() { }
        //--------------------------------------------------------
        public void LoadedAble(IUserData component)
        {
            OnLoadedAble(component);
        }
        //--------------------------------------------------------
        protected virtual void OnLoadedAble(IUserData component) { }
        //--------------------------------------------------------
        public void Update(ExternEngine.FFloat fDelta)
        {
            OnUpdate(fDelta);
        }
        //--------------------------------------------------------
        protected virtual void OnUpdate(ExternEngine.FFloat fDelta) { }
        //--------------------------------------------------------
        public void Clear()
        {
            OnClear();
        }
        //--------------------------------------------------------
        protected virtual void OnClear()
        {

        }
        //--------------------------------------------------------
        public override void Destroy()
        {
            OnDestroy();
            m_pActor = null;
        }
        //--------------------------------------------------------
        protected virtual void OnDestroy()
        {

        }
	}
}
#endif