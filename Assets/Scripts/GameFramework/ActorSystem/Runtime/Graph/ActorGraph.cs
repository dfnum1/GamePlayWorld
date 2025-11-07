#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorGraph
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Cutscene.Runtime;
using Framework.Plugin.AT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public enum EDriverState
    {
        None,
        Stop,
        Play,
        Destroy,
    }
    public class ActorGraph : TypeObject
    {
        bool m_bInited = false;
        IUserData m_pOwner;
#if USE_CUTSCENE
        public Cutscene.Runtime.CutsceneInstance m_pCutsceneInstance = null;
#endif
        private bool m_bAutoUpdate = true;
        private ActorAction m_pPlayingAction = null;
        private ActorGraphData m_pGraphData = null;
        System.Action<ActorGraphData> m_pOnLoadedCallback = null;
        System.Action<ActorAction> m_pOnStartAction = null;
        System.Action<ActorAction> m_pOnEndAction = null;
        //--------------------------------------------------------
        public AFramework GetFramework()
        {
            if (m_pOwner == null) return null;
            if (m_pOwner is AWorldNode) return ((AWorldNode)m_pOwner).GetGameModule();
            return null;
        }
        //--------------------------------------------------------
        public ActorGraphData GetGraphData()
        {
            if (m_pGraphData == null) m_pGraphData = new ActorGraphData();
            return m_pGraphData;
        }
        //--------------------------------------------------------
        public void AddStartActionCallback(System.Action<ActorAction> onCallback)
        {
            m_pOnStartAction += onCallback;
        }
        //--------------------------------------------------------
        public void RemoveStartActionCallback(System.Action<ActorAction> onCallback)
        {
            m_pOnStartAction -= onCallback;
        }
        //--------------------------------------------------------
        public void AddEndActionCallback(System.Action<ActorAction> onCallback)
        {
            m_pOnStartAction += onCallback;
        }
        //--------------------------------------------------------
        public void RemoveEndActionCallback(System.Action<ActorAction> onCallback)
        {
            m_pOnStartAction -= onCallback;
        }
        //--------------------------------------------------------
        public bool LoadGraph(string graphName, System.Action<ActorGraphData> OnLoaded)
        {
            if (string.IsNullOrEmpty(graphName))
            {
                if (OnLoaded != null) OnLoaded(null);
                return false;
            }

            if (GetFramework() == null)
            {
                if (OnLoaded != null) OnLoaded(null);
                return false;
            }

            if(GetFramework().dataManager!=null)
            {
                var pGrahData = GetFramework().dataManager.GetCustomData<ActorGraphData>(graphName);
                if(pGrahData != null)
                {
                    m_pGraphData = pGrahData;
                    if (OnLoaded != null) OnLoaded(m_pGraphData);
                    return true;
                }
            }

            m_pOnLoadedCallback = OnLoaded;
            var assetOp = GetFramework().FileSystem.ReadFile(graphName);
            assetOp.OnCallback += OnLoadActorGraph;
            return true;
        }
        //--------------------------------------------------------
        void OnLoadActorGraph(AssetOperiaon pOp)
        {
            if (pOp.pAsset == null || m_pOwner == null || IsDestroy)
                return;
            if(m_pOwner is AWorldNode)
            {
                AWorldNode pNode = (AWorldNode)m_pOwner;
                if (pNode.IsDestroy() || pNode.IsKilled())
                    return;
            }
            LoadActorGraph(pOp.pAsset.GetOrigin<TextAsset>(), null);
        }
        //--------------------------------------------------------
        public void LoadActorGraph(TextAsset pText, System.Action<ActorGraphData> OnLoaded)
        {
            if (OnLoaded != null) m_pOnLoadedCallback = OnLoaded;
            if (pText != null)
            {
                string key = pText.GetInstanceID().ToString();
                if (GetFramework().dataManager != null)
                {
                    var pGrahData = GetFramework().dataManager.GetCustomData<ActorGraphData>(key);
                    if (pGrahData != null)
                    {
                        m_pGraphData = pGrahData;
                        if (m_pOnLoadedCallback != null) m_pOnLoadedCallback(m_pGraphData);
                        return;
                    }
                }

                if (m_pGraphData == null)
                    m_pGraphData = new ActorGraphData();
                m_pGraphData.Load(pText.text);
#if UNITY_EDITOR
                m_pGraphData.SetPathFile(UnityEditor.AssetDatabase.GetAssetPath(pText));
#endif
                GetFramework()?.dataManager?.AddCustomData(key, m_pGraphData);
                if (m_pOnLoadedCallback != null) m_pOnLoadedCallback(m_pGraphData);
            }
        }
        //--------------------------------------------------------
        public void Init(IUserData userPtr)
        {
            if (m_bInited)
                return;
            m_bInited = true;
            m_pOwner = userPtr;
            m_pPlayingAction = null;
#if USE_CUTSCENE
            if (m_pCutsceneInstance == null)
            {
                m_pCutsceneInstance = new Cutscene.Runtime.CutsceneInstance(GetFramework().cutsceneManager);
            }
#endif
        }
        //--------------------------------------------------------
        public void SetAutoUpdate(bool bAuto)
        {
            m_bAutoUpdate = bAuto;
        }
        //--------------------------------------------------------
#if USE_CUTSCENE
        public Cutscene.Runtime.CutsceneInstance GetCutsceneInstance()
        {
            return m_pCutsceneInstance;
        }
#endif
        //--------------------------------------------------------
        public IUserData GetOwner()
        {
            return m_pOwner;
        }
        //--------------------------------------------------------
        public void SetOwner(IUserData pOwner)
        {
            m_pOwner = pOwner;
        }
        //--------------------------------------------------------
        public int ToFrame(float time)
        {
            return (int)(time * GetFrameRate());
        }
        //--------------------------------------------------------
        public float ToTime(int frame)
        {
            return (int)(frame * GetInvFrameRate());
        }
        //--------------------------------------------------------
        public void SetFrameRate(int frameRate)
        {
        }
        //--------------------------------------------------------
        public int GetFrameRate()
        {
            return 0;
        }
        //--------------------------------------------------------
        public float GetInvFrameRate()
        {
            return 0.03f;
        }
        //--------------------------------------------------------
        public int GetPlayFrame()
        {
            return ToFrame(GetPlayTime());
        }
        //--------------------------------------------------------
        public float GetPlayTime()
        {
            return m_pCutsceneInstance.GetTime();
        }
        //--------------------------------------------------------
        public void Play(float starTime)
        {
            m_pCutsceneInstance.Play();
            m_pCutsceneInstance.SetTime(starTime);
        }
        //--------------------------------------------------------
        public void SetPlayTime(float time)
        {
            m_pCutsceneInstance.SetTime(time);
        }
#if USE_CUTSCENE
        //--------------------------------------------------------
        public void Play(CutsceneGraph cutsceneGraph, ActorAction pOwner, StateParam pStateParam = null)
        {
            if (m_pPlayingAction == pOwner)
                return;
            if (m_pCutsceneInstance.Create(cutsceneGraph))
            {
                m_pCutsceneInstance.BindData(m_pOwner);
                m_pCutsceneInstance.Enable(true);
                m_pCutsceneInstance.Play();
                m_pPlayingAction = pOwner;
                if(m_pOnStartAction!=null) m_pOnStartAction(m_pPlayingAction);
            }
            else
                m_pPlayingAction = null;
        }
#else
        public void Play()
        {
        }
#endif
        //--------------------------------------------------------
        public void Pause(bool bPause)
        {
            m_pCutsceneInstance.Pause();
        }
        //--------------------------------------------------------
        public void Stop(ActorAction pAction)
        {
            if(m_pPlayingAction == pAction)
            {
                m_pCutsceneInstance.Stop();
                if (m_pOnEndAction != null) m_pOnEndAction(m_pPlayingAction);
                m_pPlayingAction = null;
            }
        }
        //--------------------------------------------------------
        public bool IsInAction(EActionStateType eType)
        {
            if (m_pPlayingAction == null)
                return false;
            return m_pPlayingAction.type == eType;
        }
        //--------------------------------------------------------
        public bool IsPlaying
        {
            get 
            {
#if USE_CUTSCENE
                if (m_pCutsceneInstance != null) return m_pCutsceneInstance.GetStatus() == EPlayableStatus.Start;
#endif
                return false;
            }
        }
        //--------------------------------------------------------
        public bool IsStoped
        {
            get
            {
#if USE_CUTSCENE
                if (m_pCutsceneInstance != null) return m_pCutsceneInstance.GetStatus() == EPlayableStatus.Stop;
#endif
                return false;
            }
        }
        //--------------------------------------------------------
        public bool IsPause
        {
            get
            {
#if USE_CUTSCENE
                if (m_pCutsceneInstance != null) return m_pCutsceneInstance.GetStatus() == EPlayableStatus.Pause;
#endif
                return false;
            }
        }
        //--------------------------------------------------------
        public bool IsDestroy
        {
            get
            {
#if USE_CUTSCENE
                if (m_pCutsceneInstance != null) return m_pCutsceneInstance.GetStatus() == EPlayableStatus.Destroy;
#endif
                return false;
            }
        }
        //--------------------------------------------------------
        public void Update(float fDelta)
        {
            if (!m_bAutoUpdate)
                return;
#if USE_CUTSCENE
            if (m_pCutsceneInstance != null)
            {
                if(!m_pCutsceneInstance.Update(fDelta))
                {
                    if (m_pOnEndAction != null) m_pOnEndAction(m_pPlayingAction);
                    m_pPlayingAction = null;
                }
            }
#endif
        }
        //--------------------------------------------------------
#if UNITY_EDITOR
        public void EditUpdate(float fDelta)
        {
#if USE_CUTSCENE
            if (m_pCutsceneInstance != null) m_pCutsceneInstance.Update(fDelta);
#endif
        }
#endif
        //--------------------------------------------------------
        public override void Destroy()
        {
            if (m_pCutsceneInstance != null) m_pCutsceneInstance.Destroy();
            m_pOwner = null;
            m_bInited = false;
            m_bAutoUpdate = true;
            m_pPlayingAction = null;
        }
    }
}
#endif