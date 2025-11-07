#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActionStatePlayAble
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.Core
{
    public class ActionStatePlayAble
    {
        public void Initialize( AnimationClip clip, WrapMode wrapMode, string stateName)
        {
            m_bDestroy = false;
            m_Clip = clip;
            m_WrapMode = wrapMode;

            m_StateName = stateName;
        }

        public float GetTime()
        {
            if (m_TimeIsUpToDate)
                return m_Time;

            m_Time = (float)m_Playable.GetTime();
            m_TimeIsUpToDate = true;
            return m_Time;
        }

        public void SetTime(float newTime, bool overTimeDone = true)
        {
            m_Time = newTime;
            if (m_WrapMode == WrapMode.Clamp || m_WrapMode == WrapMode.ClampForever)
            {
                if (m_Time >= m_Playable.GetDuration())
                    m_Time = (float)m_Playable.GetDuration();
            }
            if(m_Playable.IsValid())
            {
                m_Playable.SetTime(m_Time);
                m_Playable.SetTime(m_Time);
            }
            if (overTimeDone)
            {
                if (m_WrapMode == WrapMode.Once)
                    m_Playable.SetDone(m_Time >= m_Playable.GetDuration());
            }
        }

        public void Enable()
        {
            if (m_Enabled)
                return;

            m_EnabledDirty = true;
            m_Enabled = true;
        }

        public void Disable()
        {
            if (m_Enabled == false)
                return;

            m_EnabledDirty = true;
            m_Enabled = false;
        }

        public void Pause()
        {
            m_Playable.Pause();
            // m_Playable.SetPlayState(PlayState.Paused);
        }

        public void Play()
        {
            if(m_Playable.IsValid()) m_Playable.Play();
            //       m_Playable.SetPlayState(PlayState.Playing);
        }

        public void Stop()
        {
            m_FadeSpeed = 0f;
            ForceWeight(0.0f);
            Disable();
            SetTime(0.0f);
            if(m_Playable.IsValid()) m_Playable.SetDone(false);
        }

        public void ForceWeight(float weight)
        {
            m_TargetWeight = weight;
            m_Fading = false;
            m_FadeSpeed = 0f;
            SetWeight(weight);
        }

        public void SetWeight(float weight)
        {
            m_Weight = weight;
            m_WeightDirty = true;
        }

        public void FadeTo(float weight, float speed)
        {
            m_Fading = Mathf.Abs(speed) > 0f;
            m_FadeSpeed = speed;
            m_TargetWeight = weight;
        }

        public void Destroy()
        {
            m_pOwner = null;
            m_bDestroy = true;
            if (m_Playable.IsValid())
            {
                m_Playable.GetGraph().DestroySubgraph(m_Playable);
            }
        }

        public bool enabled
        {
            get { return m_Enabled; }
        }

        private bool m_Enabled;

        public AnimationMixerPlayable mixerPlayable
        {
            get { return m_MixerPlayable; }
            set
            {
                m_MixerPlayable = value;
            }
        }

        private AnimationMixerPlayable m_MixerPlayable;

        private bool m_bDestroy;

        public bool isDestroy
        {
            get { return m_bDestroy; }
        }

        private IUserData m_pOwner;

        public IUserData owner
        {
            get { return m_pOwner; }
            set { m_pOwner = value; }
        }
        private int m_nIndex;

        public int index
        {
            get { return m_nIndex; }
            set { m_nIndex = value; }
        }

        private uint m_Layer;

        public uint layer
        {
            get { return m_Layer; }
            set { m_Layer = value; }
        }

        private bool m_ApplyAvatarMask;
        public bool applyAvatarMask
        {
            get { return m_ApplyAvatarMask; }
            set { m_ApplyAvatarMask = value; }
        }

        private AvatarMask m_AvatarMask;
        public AvatarMask avatarMask
        {
            get { return m_AvatarMask; }
            set { m_AvatarMask = value; }
        }

        private string m_StateName;

        public bool fading
        {
            get { return m_Fading; }
        }

        private bool m_Fading;


        private float m_Time;

        public float targetWeight
        {
            get { return m_TargetWeight; }
        }

        private float m_TargetWeight;

        public float weight
        {
            get { return m_Weight; }
        }

        float m_Weight;

        public float fadeSpeed
        {
            get { return m_FadeSpeed; }
        }

        float m_FadeSpeed;

        public float speed
        {
            get { return (float)m_Playable.GetSpeed(); }
            set { m_Playable.SetSpeed(value); }
        }

        public float playableDuration
        {
            get { return (float)m_Playable.GetDuration(); }
        }

        public AnimationClip clip
        {
            get { return m_Clip; }
        }

        private AnimationClip m_Clip;

        public void SetPlayable(Playable playable)
        {
            m_Playable = playable;
        }

        public bool isDone { get { return m_Playable.IsDone(); } }

        public PlayState playState
        {
            get
            {
                return m_Playable.GetPlayState();
            }
        }

        public bool isValid
        {
            get { return m_pOwner != null && !m_bDestroy && playable.IsValid(); }
        }

        public string stateName
        {
            get
            {
                return m_StateName;
            }
        }


        public Playable playable
        {
            get { return m_Playable; }
        }

        private Playable m_Playable;

        public WrapMode wrapMode
        {
            get { return m_WrapMode; }
        }

        private WrapMode m_WrapMode;

        public bool isReadyForCleanup
        {
            get { return m_ReadyForCleanup; }
        }

        private bool m_ReadyForCleanup;

        public bool enabledDirty { get { return m_EnabledDirty; } }
        public bool weightDirty { get { return m_WeightDirty; } }

        public void ResetDirtyFlags()
        {
            m_EnabledDirty = false;
            m_WeightDirty = false;
        }

        private bool m_WeightDirty;
        private bool m_EnabledDirty;

        public void InvalidateTime() { m_TimeIsUpToDate = false; }
        private bool m_TimeIsUpToDate;
    }
    public class GraphAnimationPlayer : PlayableBehaviour
    {
        class DefaultClip : IUserData
        {
            public void Destroy()
            {
            }
        }
        protected Playable m_ActualPlayable;
        AnimationMixerPlayable[] m_Mixers;
        AnimationLayerMixerPlayable m_MixerLayer;
        private int m_nClipNum = 1;
        private int m_nMixLayerCnt = 0;
        public Playable playable { get { return self; } }
        protected Playable self { get { return m_ActualPlayable; } }
        public PlayableGraph graph { get { return self.GetGraph(); } }

        DefaultClip m_DefaultClip = new DefaultClip();
        private IUserData m_DefaultOwner = null;
        private int m_StatesVersion = 0;
        private int m_Count =0;
        List<ActionStatePlayAble> m_vStates;
        Dictionary<IUserData, ActionStatePlayAble> m_vBindStates;
        Dictionary<string, ActionStatePlayAble> m_vBindNameStates;
        bool m_KeepStoppedPlayablesConnected = true;
        public bool keepStoppedPlayablesConnected
        {
            get { return m_KeepStoppedPlayablesConnected; }
            set
            {
                if (value != m_KeepStoppedPlayablesConnected)
                {
                    m_KeepStoppedPlayablesConnected = value;
                }
            }
        }
        public GraphAnimationPlayer()
        {
        }
        //--------------------------------------------------------
        public override void OnPlayableCreate(Playable playable)
        {
            m_ActualPlayable = playable;
            SetMixLayer(1);
        }
        //------------------------------------------------------
        public void SetAvatarMask(uint layer, AvatarMask mask)
        {
            if (mask == null)
            {
                return;
            }
            m_MixerLayer.SetLayerMaskFromAvatarMask(layer, mask);
        }
        //------------------------------------------------------
        public void SetMixLayer(int layerNum)
        {
            if (m_nMixLayerCnt == layerNum) return;
            m_nMixLayerCnt = layerNum;
            m_Mixers = new AnimationMixerPlayable[m_nMixLayerCnt];
            for (int i = 0; i < m_nMixLayerCnt; ++i)
                m_Mixers[i] = AnimationMixerPlayable.Create(graph, m_nClipNum);
            m_MixerLayer = AnimationLayerMixerPlayable.Create(graph, m_nMixLayerCnt);

            for (int i = 0; i < m_nMixLayerCnt; ++i)
            {
                m_Mixers[i].SetInputWeight(0, 1);
                m_MixerLayer.SetInputWeight(i, 1);
            }

            self.SetInputCount(1);
            self.SetInputWeight(0, 1);
            graph.Connect(m_MixerLayer, 0, self, 0);

            for (int i = 0; i < m_nMixLayerCnt; ++i)
                graph.Connect(m_Mixers[i], 0, m_MixerLayer, i);
        }
        //------------------------------------------------------
        public void SetClipNum(int layerNum)
        {
            if (m_nClipNum == layerNum) return;
            m_nClipNum = layerNum;
            for (int i = 0; i < m_Mixers.Length; ++i)
            {
                if (m_Mixers[i].GetInputCount() != layerNum)
                    m_Mixers[i].SetInputCount(layerNum);
            }
        }
        //------------------------------------------------------
        public override void OnPlayableDestroy(Playable playable)
        {
        }
        //--------------------------------------------------------
        public void SetDefaultClip(AnimationClip clip, uint layer = 0, IUserData pOwner = null, string stateName = null)
        {
            if (pOwner == null)
                pOwner = m_DefaultClip;
            else
                m_DefaultOwner = pOwner;
            if (m_vBindStates != null && m_vBindStates.TryGetValue(pOwner, out var defState))
            {
                if (defState.clip == clip)
                    return;

                RemoveMotion(pOwner);
            }
            if (null == stateName)
                stateName = clip.name;
            InnerAddMotion(pOwner, clip, stateName, layer);
        }
        //--------------------------------------------------------
        public bool PlayAction(string action, float fSpeed=1, bool bForce = false)
        {
            if (m_vBindNameStates == null)
                return false;
            if(m_vBindNameStates.TryGetValue(action, out var state))
            {
                if (!state.isValid)
                    return false;
                if (!bForce && !state.isDone && state.enabled)
                {
                    return false;
                }
                InnerPlay(state.index, fSpeed);
                return true;
            }
            return false;
        }
#if USE_CUTSCENE
        //--------------------------------------------------------
        //public void AddMotion(MotionClip motionClip, uint layer = 0)
        //{
        //    if (motionClip == null || motionClip.clip == null)
        //        return;
        //    InnerAddMotion(motionClip, motionClip.clip, motionClip.name, layer);
        //}
#endif
        //--------------------------------------------------------
        public void AddMotion(IUserData pOwner, AnimationClip clip, uint layer = 0, string stateName = null)
        {
            if (pOwner == null || clip == null)
                return;
            if(m_vBindStates!=null)
            {
                if (m_vBindStates.ContainsKey(pOwner))
                    return;
            }
            InnerAddMotion(pOwner, clip, stateName, layer);
        }
        //--------------------------------------------------------
        void InnerAddMotion(IUserData owner, AnimationClip clip, string stateName, uint layer = 0)
        {
            if (clip == null)
                return;
 
            if (m_vStates == null) m_vStates = new List<ActionStatePlayAble>(2);
            if (m_vBindStates == null) m_vBindStates = new Dictionary<IUserData, ActionStatePlayAble>(2);
            if (m_vBindNameStates == null) m_vBindNameStates = new Dictionary<string, ActionStatePlayAble>(2);

            if (layer >= m_nMixLayerCnt) layer = (uint)(m_nMixLayerCnt - 1);

            ActionStatePlayAble newState = InsertState(owner);
            newState.owner = owner;
            WrapMode wrapMode = WrapMode.Once;
            if (clip.isLooping) wrapMode = WrapMode.Loop;
            else wrapMode = WrapMode.ClampForever;
            newState.Initialize(clip, wrapMode, stateName);
            newState.layer = layer;
            newState.mixerPlayable = m_Mixers[layer];
            if (newState.index == newState.mixerPlayable.GetInputCount())
            {
                newState.mixerPlayable.SetInputCount(newState.index + 1);
            }
            var clipPlayable = AnimationClipPlayable.Create(graph, clip);
            clipPlayable.SetApplyFootIK(false);
            clipPlayable.SetApplyPlayableIK(false);
            if (!clip.isLooping || newState.wrapMode == WrapMode.Once)
            {
                clipPlayable.SetDuration(clip.length);
            }
            newState.mixerPlayable.SetInputWeight(newState.index, 0);
            newState.SetPlayable(clipPlayable);
            newState.Pause();

            m_vStates[newState.index] = newState;
            m_vBindStates[owner] = newState;
            m_vBindNameStates[clip.name] = newState;
            if (keepStoppedPlayablesConnected)
                ConnectInput(newState.index);

            UpdateDoneStatus();
            InvalidateStates();
        }
        //--------------------------------------------------------
        ActionStatePlayAble InsertState(IUserData motionClip)
        {
            ActionStatePlayAble state = new ActionStatePlayAble();
            state.owner = motionClip;

            int firstAvailable = m_vStates.FindIndex(s => s.isDestroy);
            if (firstAvailable == -1)
            {
                firstAvailable = m_vStates.Count;
                m_vStates.Add(state);
            }
            else
            {
                m_vStates.Insert(firstAvailable, state);
            }

            state.index = firstAvailable;
            m_Count++;
            return state;
        }
        //--------------------------------------------------------
        public void RemoveMotion(IUserData owner, uint layer = 0)
        {
            if (m_vBindStates == null)
                return;
            if(m_vBindStates.TryGetValue(owner, out var state))
            {
                m_vBindStates.Remove(owner);
                if (m_vBindNameStates != null) m_vBindNameStates.Remove(state.stateName);
                if (state.playable.IsValid())
                    state.Destroy();
             //   m_vStates[state.index] = state;
                m_Count = m_vStates.Count;
            }
        }
        //--------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (m_vStates == null)
                return;
            InvalidateStateTimes();

            UpdateStates(deltaTime);

            //Once everything is calculated, update done status
            UpdateDoneStatus();

            CleanClonedStates();
        }
        //------------------------------------------------------
        public override void PrepareFrame(Playable owner, FrameData data)
        {
            Update(data.deltaTime);
        }
        //------------------------------------------------------
        private void UpdateDoneStatus()
        {
            if (!AnyStatePlaying())
            {
                bool wasDone = playable.IsDone();
                playable.SetDone(true);
                //                 if (!wasDone && onDone != null)
                //                 {
                //                     onDone();
                //                 }
            }

        }
        private void InvalidateStates() { m_StatesVersion++; }
        //------------------------------------------------------
        bool AnyStatePlaying()
        {
            for(int i =0; i < m_vStates.Count; ++i)
            {
                if (m_vStates[i].playable.IsValid())
                {
                    if (m_vStates[i].enabled)
                        return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        private void CleanClonedStates()
        {
            for (int i = m_vStates.Count - 1; i >= 0; i--)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;

                if (state.isReadyForCleanup)
                {
                    Playable toDestroy = state.mixerPlayable.GetInput(state.index);
                    graph.Disconnect(state.mixerPlayable, state.index);
                    graph.DestroyPlayable(toDestroy);
                }
            }
        }
        //------------------------------------------------------
        void InvalidateStateTimes()
        {
            if (m_vStates == null) return;
            int count = m_vStates.Count;
            for (int i = 0; i < count; i++)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;

                state.InvalidateTime();
             //   m_vStates[i] = state;
            }
        }
        //------------------------------------------------------
        public bool IsStateExist(IUserData clipOwner)
        {
            if (m_vBindStates == null)
                return false;
            return m_vBindStates.ContainsKey(clipOwner);
        }
        //------------------------------------------------------
        internal Dictionary<IUserData, ActionStatePlayAble> GetBindStates()
        {
            return m_vBindStates;
        }
        //------------------------------------------------------
        public bool IsStateExist(AnimationClip clipOwner)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (clipOwner == state.clip)
                {
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsStateExist(string action)
        {
            if (m_vBindNameStates == null)
                return false;
            return m_vBindNameStates.ContainsKey(action);
        }
        //--------------------------------------------------------
        public bool Play(string action, float fSpeed = 1, bool bForce = false)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    if(!bForce && !state.isDone && state.enabled)
                    {
                        return true;
                    }
                    Play(state.owner, fSpeed);
                    return true;
                }
            }
            return false;
        }
        //--------------------------------------------------------
        public bool Play(AnimationClip action, float fSpeed = 1, bool bForce = false)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action ==state.clip)
                {
                    if (!bForce && !state.isDone && state.enabled)
                    {
                        return true;
                    }
                    Play(state.owner, fSpeed);
                    return true;
                }
            }
            return false;
        }
        //--------------------------------------------------------
        public void Play(IUserData motionClip, float fSpeed = 1, bool bForce= false)
        {
            if (m_vBindStates == null)
                return;
            if (m_vBindStates.TryGetValue(motionClip, out var state))
            {
                if (!bForce && !state.isDone && state.enabled)
                {
                    return;
                }
                InnerPlay(state.index, fSpeed);
            }
        }
        //--------------------------------------------------------
        public void Stop(IUserData motionClip, float stopFade = 0.1f)
        {
            if (m_vBindStates == null)
                return;
            if (m_vBindStates.TryGetValue(motionClip, out var state))
            {
                InnerStop(state.index, stopFade);
            }
        }
        //--------------------------------------------------------
        public void Stop(AnimationClip action, float stopFade = 0.1f)
        {
            if (m_vStates == null)
                return;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action == state.clip)
                {
                    InnerStop(state.index, stopFade);
                    break;
                }
            }
        }
        //--------------------------------------------------------
        public void Stop(string action, float stopFade = 0.1f)
        {
            if (m_vStates == null)
                return;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    InnerStop(state.index, stopFade);
                    break;
                }
            }
        }
        //--------------------------------------------------------
        public bool CrossFade(string action, float speed, float time, bool bForce = false)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    if (!bForce && !state.isDone && state.enabled)
                    {
                        return true;
                    }
                    CrossFade(state.owner, speed, time);
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool CrossFade(IUserData motionClip, float speed, float time, bool bForce = false)
        {
            if (m_vBindStates == null)
                return false;
            if(m_vBindStates.TryGetValue(motionClip, out var state))
            {
                if(state.isValid)
                {
                    if (!bForce && !state.isDone && state.enabled)
                    {
                        return true;
                    }
                    if (time == 0f)  return InnerPlay(state.index, speed);
                    return InnerCrossFade(state.index, speed, time, state.layer);
                }
            }
            return false;
        }
        //--------------------------------------------------------
        public bool Blend(string action, float speed, float targetWeight, float blendTime, bool bForce = false)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    Blend(state.owner, speed, targetWeight, blendTime, bForce);
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool Blend(IUserData motionClip, float speed, float targetWeight, float blendTime, bool bForce = false)
        {
            if (m_vBindStates == null)
                return false;
            if (m_vBindStates.TryGetValue(motionClip, out var state))
            {
                if (!bForce && !state.isDone && state.enabled)
                {
                    return true;
                }
                return InnerBlend(state.index, speed, targetWeight, blendTime);
            }
            return false;
        }
        //--------------------------------------------------------
        public void SetTime(float time, bool overTimeDone = true)
        {
            if (m_vStates == null)
                return;
            ActionStatePlayAble playable;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                playable = m_vStates[i];
                if (playable.isValid && playable.enabled && !playable.isDone)
                {
                    SetTime(playable.owner, time, overTimeDone);
               //     m_vStates[i] = playable;
                }
            }
        }
        //--------------------------------------------------------
        public bool SetTime(string action, float time, bool overTimeDone = true)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    SetTime(state.owner, time, overTimeDone);
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public void SetTime(IUserData motionClip, float time, bool overTimeDone = true)
        {
            if (m_vBindStates == null)
                return;
            if (m_vBindStates.TryGetValue(motionClip, out var playable))
            {
                if (playable.isValid)
                {
                    if (!playable.enabled)
                    {
                        playable.Enable();
                        playable.ForceWeight(1.0f);
                        playable.speed = 1;
                        playable.Play();
                    }
                    playable.SetTime(time, overTimeDone);
               //     m_vStates[playable.index] = playable;
                }
            }
        }
        //--------------------------------------------------------
        public float GetTime(string action)
        {
            if (m_vStates == null)
                return 0;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    return state.GetTime();
                }
            }
            return 0;
        }
        //------------------------------------------------------
        public float GetTime(IUserData motionClip)
        {
            if (m_vBindStates == null)
                return 0;
            if (m_vBindStates.TryGetValue(motionClip, out var playable))
            {
                if (playable.isValid)
                {
                    return playable.GetTime();
                }
            }
            return 0;
        }
        //--------------------------------------------------------
        public void SetSpeed(string action, float speed)
        {
            if (m_vStates == null)
                return;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    SetSpeed(state.owner, speed);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void SetSpeed(IUserData motionClip, float speed)
        {
            if (m_vBindStates == null)
                return;
            if (m_vBindStates.TryGetValue(motionClip, out var playable))
            {
                if (playable.isValid && playable.enabled && !playable.isDone)
                {
                    playable.speed = speed;
                  //  m_vStates[playable.index] = playable;
                }
            }
        }
        //--------------------------------------------------------
        public void SetBlendWeight(string action, float fWeight)
        {
            if (m_vStates == null)
                return;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    SetBlendWeight(state.owner, fWeight);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void SetBlendWeight(IUserData motionClip, float fWeight)
        {
            if (m_vBindStates == null)
                return;
            if (m_vBindStates.TryGetValue(motionClip, out var playable))
            {
                if (playable.isValid)
                {
                    playable.SetWeight(fWeight);
                 //   m_vStates[playable.index] = playable;
                }
            }
        }
        //------------------------------------------------------
        public void SetSpeed(float speed)
        {
            if (m_vStates == null)
                return;
              
            ActionStatePlayAble playable;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                playable = m_vStates[i];
                if (playable.isValid && playable.enabled && !playable.isDone)
                {
                    if (speed <= 0)
                    {
                        playable.Pause();
                    }
                    else
                    {
                        playable.Play();
                        playable.speed = speed;
                    }
                  //  m_vStates[i] = playable;
                }
            }
        }
        //------------------------------------------------------
        public bool IsPlaying(string action)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action.CompareTo(state.stateName) == 0)
                {
                    return state.enabled && !state.isDone;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsPlaying(AnimationClip action)
        {
            if (m_vStates == null)
                return false;
            for (int i = 0; i < m_vStates.Count; ++i)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;
                if (action ==state.clip)
                {
                    return state.enabled && !state.isDone;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public IUserData GetPlayingAction(uint layer)
        {
            if (m_vBindStates == null)
                return null;
            foreach(var db in m_vBindStates)
            { 
                if(!db.Value.isDone && db.Value.enabled && db.Value.layer == layer)
                {
                    return db.Key;
                }
            }
            return null;
        }
        //------------------------------------------------------
        public bool IsPlaying(IUserData motionClip)
        {
            if (m_vBindStates == null) return false;
            if(m_vBindStates.TryGetValue(motionClip, out var state))
            {
                return state.enabled && !state.isDone;
            }
            return false;
        }
        //------------------------------------------------------
        public bool StopAll()
        {
            if(m_vStates !=null)
            {
                for (int i = 0; i < m_vStates.Count; i++)
                {
                    var state = m_vStates[i];
                    m_vStates[i].Stop();
                 //   m_vStates[i] = state;
                }
            }
            playable.SetDone(true);
            return true;
        }
        //------------------------------------------------------
        public bool IsPlaying()
        {
            return AnyStatePlaying();
        }
        //------------------------------------------------------
        private void DoStop(int index)
        {
            if (m_vStates == null) return;
            ActionStatePlayAble state = m_vStates[index];
            state.Stop();
         //   m_vStates[index] = state;
        }
        //------------------------------------------------------
        private void SetupLerp(ref ActionStatePlayAble state, float targetWeight, float blendTime)
        {
            float travel = Mathf.Abs(state.weight - targetWeight);
            float newSpeed = blendTime != 0f ? travel / blendTime : Mathf.Infinity;

            // If we're fading to the same target as before but slower, assume CrossFade was called multiple times and ignore new speed
            if (state.fading && Mathf.Approximately(state.targetWeight, targetWeight) && newSpeed < state.fadeSpeed)
                return;

            state.FadeTo(targetWeight, newSpeed);
        }
        //--------------------------------------------------------
        bool InnerPlay(int index, float fSpeed = 1)
        {
            if (m_vStates == null)
                return false;
            bool bFined = false;
            for (int i = 0; i < m_vStates.Count; i++)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (state.index == index)
                {
                    DoStop(i);
                    state.Enable();
                    state.speed = fSpeed;
                    state.SetTime(0);
                    state.ForceWeight(1.0f);
                    state.Play();
                    //  m_vStates[i] = state;
                    bFined = true;
                }
                else
                {
                    state.Stop();
                }
            }
            return bFined;
        }
        //--------------------------------------------------------
        void InnerStop(int index, float stopFade = 0.0f)
        {
            if (m_vStates == null || index < 0 || index >= m_vStates.Count)
                return;
            ActionStatePlayAble state = m_vStates[index];
            if (state.isDone || !state.enabled)
                return;
            if (stopFade > 0)
            {
                SetupLerp(ref state, 0, stopFade);
            }
            else
            {
                state.ForceWeight(0.0f);
            }
        }
        //------------------------------------------------------
        private bool InnerCrossFade(int index, float speed, float blendTime, uint layer)
        {
            for (int i = 0; i < m_vStates.Count; i++)
            {
                ActionStatePlayAble state = m_vStates[i];
                if (!state.isValid)
                    continue;

                if (state.index == index)
                {
                    state.Enable();
                    state.speed = speed;
                    state.SetTime(0);
                    state.Play();
                 //   m_vStates[i] = state;
                }
                if (state.layer != layer)
                    continue;

                if (state.enabled == false)
                    continue;

                float targetWeight = state.index == index ? 1.0f : 0.0f;
                SetupLerp(ref state, targetWeight, blendTime);
              //  m_vStates[i] = state;
            }

            return true;
        }
        //------------------------------------------------------
        private bool InnerBlend(int index, float speed, float targetWeight, float blendTime)
        {
            ActionStatePlayAble state = m_vStates[index];
            if (!state.isValid)
                return false;
            if (state.enabled == false)
                state.Enable();
            state.SetTime(0);
            state.speed = speed;
            if (blendTime == 0f)
            {
                state.ForceWeight(targetWeight);
            }
            else
            {
                SetupLerp(ref state, targetWeight, blendTime);
            }
           // m_vStates[state.index] = state;
            return true;
        }
        //------------------------------------------------------
        private void UpdateStates(float deltaTime)
        {
            bool mustUpdateWeights = false;
            float layerTotalWeight0 = 0;
            float layerTotalWeight1 = 0;
            bool hasActionPlaying = false;
            for (int i = 0; i < m_vStates.Count; i++)
            {
                ActionStatePlayAble state = m_vStates[i];

                //Skip deleted states
                if (!state.isValid)
                {
                    continue;
                }

                bool fadingOut = false;
                //Update crossfade weight
                if (state.fading)
                {
                    if (state.targetWeight <= 0.0f)
                        fadingOut = true;
                    state.SetWeight(Mathf.MoveTowards(state.weight, state.targetWeight, state.fadeSpeed * deltaTime));
                    if (Mathf.Approximately(state.weight, state.targetWeight))
                    {
                        state.ForceWeight(state.targetWeight);
                        if (state.weight == 0f)
                        {
                            state.Stop();
                        }
                        else if (state.weight >= 1)
                            state.Play();
                    }
                }

                if (state.enabledDirty)
                {
                    if (state.enabled)
                        state.Play();
                    else
                        state.Pause();

                    if (!keepStoppedPlayablesConnected)
                    {
                        Playable input = state.mixerPlayable.GetInput(i);
                        //if state is disabled but the corresponding input is connected, disconnect it
                        if (input.IsValid() && !state.enabled)
                        {
                            DisconnectInput(state, i);
                        }
                        else if (state.enabled && !input.IsValid())
                        {
                            ConnectInput(state.index);
                        }
                    }
                }

                if (state.enabled && state.wrapMode == WrapMode.Once)
                {
                    bool stateIsDone = state.isDone;
                    float speed = state.speed;
                    float time = state.GetTime();
                    float duration = state.playableDuration;

                    stateIsDone |= speed < 0f && time < 0f;
                    stateIsDone |= speed >= 0f && time >= duration;
                    if (stateIsDone)
                    {
                        state.Stop();
                        state.Disable();
                        if (!keepStoppedPlayablesConnected)
                            DisconnectInput(state, state.index);

                    }
                }
                if (state.isDone)
                {
                    if (!state.fading && state.playState != PlayState.Playing)
                        state.SetWeight(0);
                }
                if (state.layer == 0) layerTotalWeight0 += state.weight;
                else layerTotalWeight1 += state.weight;
                if (state.weightDirty)
                {
                    mustUpdateWeights = true;
                }
                state.ResetDirtyFlags();

                if(!state.isDone && state.enabled && !fadingOut)
                {
                    hasActionPlaying = true;
                }

               // m_vStates[i] = state;
            }

            if (mustUpdateWeights)
            {
                bool hasAnyWeight0 = layerTotalWeight0 > 0.0f;
                bool hasAnyWeight1 = layerTotalWeight1 > 0.0f;
                for (int i = 0; i < m_vStates.Count; i++)
                {
                    ActionStatePlayAble state = m_vStates[i];
                    if (!state.isValid)
                        continue;

                    if (state.layer == 0)
                    {
                        float weight = hasAnyWeight0 ? state.weight / layerTotalWeight0 : 0.0f;
                        state.mixerPlayable.SetInputWeight(state.index, weight);
                    }
                    else if (state.layer == 1)
                    {
                        float weight = hasAnyWeight1 ? state.weight / layerTotalWeight1 : 0.0f;
                        state.mixerPlayable.SetInputWeight(state.index, weight);
                    }
                }
            }
            if(!hasActionPlaying)
            {
                IUserData defaltOwner = m_DefaultClip;
                if (m_DefaultOwner != null) defaltOwner = m_DefaultOwner;
                if (m_vBindStates.TryGetValue(defaltOwner, out var state) && (state.isDone || !state.enabled))
                {
                    InnerBlend(state.index, 1, 1, 0.1f);
                }
            }
        }
        //------------------------------------------------------
        private void DisconnectInput(ActionStatePlayAble state, int index)
        {
            if (keepStoppedPlayablesConnected)
            {
                m_vStates[index].Pause();
            }
            graph.Disconnect(state.mixerPlayable, index);
        }
        //------------------------------------------------------
        private void ConnectInput(int index)
        {
            ActionStatePlayAble state = m_vStates[index];
            graph.Connect(state.playable, 0, state.mixerPlayable, state.index);
        }
        //--------------------------------------------------------
        public void Destroy()
        {
            if(m_vStates!=null)
            {
                for (int i = m_vStates.Count - 1; i >= 0; i--)
                {
                    ActionStatePlayAble state = m_vStates[i];
                    if (state.isReadyForCleanup)
                    {
                        Playable toDestroy = state.mixerPlayable.GetInput(state.index);
                        graph.Disconnect(state.mixerPlayable, state.index);
                        graph.DestroyPlayable(toDestroy);
                    }
                }
                m_vStates.Clear();
            }
            if (m_vBindStates != null) m_vBindStates.Clear();
            if (m_vBindNameStates != null) m_vBindNameStates.Clear();
        }
    }
}
#endif