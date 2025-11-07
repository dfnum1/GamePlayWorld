#if USE_CUTSCENE

/********************************************************************
生成日期:	06:30:2025
类    名: 	ACutsceneDriver
作    者:	HappLI
描    述:	基础驱动层,用于处理剪辑的创建、销毁、帧更新、进入和离开事件等。
*********************************************************************/
using Framework.CutsceneAT.Runtime;
using Framework.Core;
using UnityEngine;

namespace Framework.Cutscene.Runtime
{
    public abstract class ACutsceneDriver : IUserData
    {
        long m_nKey = 0;
        CutsceneInstance m_pCutscene = null;
        //-----------------------------------------------------
        internal void SetCutscene(CutsceneInstance pCutscene)
        {
            m_pCutscene = pCutscene;
        }
        //-----------------------------------------------------
        internal CutsceneInstance Getcutscne()
        {
            return m_pCutscene;
        }
        //-----------------------------------------------------
        public AFramework GetFramework()
        {
            if (m_pCutscene == null) return null;
            return m_pCutscene.GetFramework();
        }
        //-----------------------------------------------------
        public int GetCutsceneID()
        {
            if (m_pCutscene != null) return m_pCutscene.GetGUID();
            return 0;
        }
        //-----------------------------------------------------
        public string GetCutsceneName()
        {
            if (m_pCutscene != null) return m_pCutscene.GetName();
            return "";
        }
        //-----------------------------------------------------
        public void Pause()
        {
            if (m_pCutscene != null) m_pCutscene.Pause();
        }
        //-----------------------------------------------------
        public void Resume()
        {
            if (m_pCutscene != null) m_pCutscene.Resume();
        }
        //-----------------------------------------------------
        public void SetTime(float time)
        {
            if (m_pCutscene != null) m_pCutscene.SetTime(time);
        }
        //-----------------------------------------------------
        internal void SetKey(long key)
        {
            m_nKey = key;
        }
        //-----------------------------------------------------
        public long GetKey()
        {
            return m_nKey;
        }
        //-----------------------------------------------------
        public bool IsDestroyed()
        {
            return m_pCutscene == null || m_nKey == 0;
        }
        //-----------------------------------------------------
        public virtual bool IsValid()
        {
            return !IsDestroyed();
        }
        //-----------------------------------------------------
        public bool IsEditorMode()
        {
            return m_pCutscene != null && m_pCutscene.IsEditorMode();
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public UnityEditor.EditorWindow GetOwnerEditor()
        {
            return m_pCutscene!=null ? m_pCutscene.GetOwnerEditor() : null;
        }
#endif
        //-----------------------------------------------------
        public void LoadAsset(string file, bool bAsync = true)
        {
            if (m_pCutscene == null)
            {
                return;
            }
            m_pCutscene.LoadAsset(file, bAsync,this);
        }
        //-----------------------------------------------------
        public void SpawnInstance(string file, bool bAsync = true)
        {
            if (m_pCutscene == null)
            {
                return;
            }
            m_pCutscene.SpawnInstance(file, bAsync, this);
        }
        //-----------------------------------------------------
        public void DespawnInstance(AInstanceAble pInstance)
        {
            if (m_pCutscene == null)
            {
                return;
            }
            m_pCutscene.DespawnInstance(pInstance);
        }
        //-----------------------------------------------------
        public virtual void OnSpawnInstance(AInstanceAble pInstance)
        {
        }
        //-----------------------------------------------------
        public virtual void OnLoadAsset(Asset pAsset)
        {

        }
        //-----------------------------------------------------
        public void SetObject(ObjId objId, ICutsceneObject pObject, bool bAutoDestroy = false)
        {
            if (m_pCutscene == null)
                return;
            m_pCutscene.SetObject(objId, pObject, bAutoDestroy);
        }
        //-----------------------------------------------------
        public void RemoveObject(ObjId objId)
        {
            if (m_pCutscene == null)
                return;
            m_pCutscene.RemoveObject(objId);
        }
        //-----------------------------------------------------
        public void RemoveObject(ICutsceneObject pObj)
        {
            if (m_pCutscene == null)
                return;
            m_pCutscene.RemoveObject(pObj);
        }
        //-----------------------------------------------------
        public ICutsceneObject GetObject(ObjId objId)
        {
            if (m_pCutscene == null)
                return null;
            return m_pCutscene.GetObject(objId);
        }
        //-----------------------------------------------------
        public ICutsceneObject GetObject(VariableObjId objId)
        {
            return GetObject(objId.value);
        }
        //-----------------------------------------------------
        public virtual void OnDestroy()
        {
        }
        //-----------------------------------------------------
        public virtual bool OnCreateClip(CutsceneTrack pTrack, IBaseClip clip)
		{
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnDestroyClip(CutsceneTrack pTrack, IBaseClip clip)
		{
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnUpdateClip(CutsceneTrack pTrack, FrameData frameData)
        {
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnFrameClip(CutsceneTrack pTrack, FrameData frameData)
		{
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnClipEnter(CutsceneTrack pTrack, FrameData frameData)
		{
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnClipLeave(CutsceneTrack pTrack, FrameData frameData)
		{
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnEventTrigger(CutsceneTrack pTrack, IBaseEvent pEvt)
        {
            return false;
        }
        //-----------------------------------------------------
        public virtual bool OnAgentTreeExecute(AgentTree pAgentTree, BaseNode pNode)
        {
            return true;
        }
        //-----------------------------------------------------
        public void Destroy()
        {
        }
    }
}
#endif