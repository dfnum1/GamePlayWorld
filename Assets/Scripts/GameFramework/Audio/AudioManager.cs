/********************************************************************
生成日期:	23:3:2020   18:07
类    名: 	AudioManager
作    者:	HappLI
描    述:	音效管理
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Plugin.AT;
using UnityEngine;

namespace Framework.Core
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        static AudioManager ms_pInstnace = null;

        static float m_BGVolume = 1f;
        static float m_SoundEffectVolume = 1f;

        private Transform m_pTransform;
        static int ms_nGUID = 0;
#if USE_FMOD
        struct FMODTrigger
        {
            public FMOD.Studio.EventInstance evtInstnance;
            public FMODUnity.EventReference eventRef;
            public bool isBG;
            public bool IsValid()
            {
                return evtInstnance.isValid() && !eventRef.IsNull;
            }
        }
        List<FMODTrigger> m_vFmodTriggers = null;
        public enum EBusType
        {
            Ambient =0,
            BGM,
            UI = 20,
            Effect = 50,
        }
        [System.Serializable]
        public struct BusData
        {
            public EBusType type;
            public string BusPath;
            public bool IsBGM() { return type <= EBusType.BGM; }
            public bool IsEffect() { return type >= EBusType.Effect; }
            public bool IsValid() { return !string.IsNullOrEmpty(BusPath); }
        }
        public BusData[] FModBus;

        private FMOD.Studio.Bus[] m_arrFmodBus;

        Stack<FmodSound> m_vFModPools = null;
        List<FmodSound> m_vFModBGSounds;
        List<FmodSound> m_vFModEffectSounds;
#endif
        public GameObject BG;
        public GameObject Effecter;

        public int MaxBGCnt = 4;
        public int MaxEffectCount = 50;
        [Min(0)]
        public float volumeDistanceFade = 300;
        [Min(0)]
        public float volumeMinDistanceFade = 10;

        public GroupMixerData[] mixerGroups = null;

        [PluginDisplay("全局背景音乐过渡")]
        public AnimationCurve BGFade;

        Dictionary<int, AudioGroupMixer> m_vGroupMixer = null;
        protected Dictionary<int, AudioSound> m_vBGSounds;
        protected Dictionary<int, AudioSound> m_vEffectSounds;
        List<int> m_vDestroyList = null;
        Queue<AudioSource> m_vEffeterPool = null;
        Queue<AudioSource> m_vBGPool = null;
        ObjectPool<AudioSound> m_vPools = null;
        bool m_bCloseAllBGAudio = false;
        bool m_bCloseEffectAudio = false;
        AgentTree m_pAgentTree = null;
        //------------------------------------------------------
        public static AudioManager getInstance()
        {
            return ms_pInstnace;
            //return null;
        }
        //------------------------------------------------------
        public void Awake()
        {
            ms_pInstnace = this;
            m_pTransform = transform;
            gameObject.name = "SoundSystems";
          //  DontDestroyOnLoad(gameObject);
            ms_nGUID = 0;
#if USE_FMOD
            FMODUnity.RuntimeManager.OnFmodEventTrigger += OnFmodEventTrigger;
            if (FModBus!=null && FModBus.Length>0)
            {
                m_arrFmodBus = new FMOD.Studio.Bus[FModBus.Length];
                for (int i = 0; i < FModBus.Length; ++i)
                {
                    if(FModBus[i].IsValid())
                        m_arrFmodBus[i] = FMODUnity.RuntimeManager.GetBus(FModBus[i].BusPath);
                }
            }
            m_vFModBGSounds = new List<FmodSound>(4);
            m_vFModEffectSounds = new List<FmodSound>(32);
            m_vFModPools = new Stack<FmodSound>(32);
#endif

            m_vPools = new ObjectPool<AudioSound>(32);
            m_vBGSounds = new Dictionary<int, AudioSound>(4);
            m_vEffectSounds = new Dictionary<int, AudioSound>(32);
            m_vEffeterPool = new Queue<AudioSource>(MaxEffectCount);
            m_vBGPool = new Queue<AudioSource>(MaxBGCnt);
            m_vDestroyList = new List<int>(MaxEffectCount);

            if (mixerGroups!=null && mixerGroups.Length>0)
            {
                GroupMixerData mixerGpData;
                m_vGroupMixer = new Dictionary<int, AudioGroupMixer>(mixerGroups.Length);
                for (int i = 0; i < mixerGroups.Length; ++i)
                {
                    mixerGpData = mixerGroups[i];
                    if (BaseUtil.IsValidCurve(mixerGpData.mixer))
                    {
                        AudioGroupMixer mixerGp = new AudioGroupMixer();
                        mixerGp.SetCurve(mixerGroups[i].mixer);
                        mixerGp.SetOrder(mixerGroups[i].order);
                        m_vGroupMixer[mixerGroups[i].group] = mixerGp;
                    }
                }
            }
        }
        //------------------------------------------------------
        public void StartUp()
        {
            if (m_pAgentTree != null)
                AgentTreeManager.getInstance().UnloadAT(m_pAgentTree);
            m_pAgentTree = AgentTreeManager.getInstance().LoadAT(Data.ATModuleSetting.SoundAT);
            if (m_pAgentTree != null)
            {
                m_pAgentTree.AddOwnerClass(this);
                m_pAgentTree.Enable(true);
                m_pAgentTree.Enter();
            }
        }
        //------------------------------------------------------
        public void SyncConfig(int BGM , int SoundEffect, float BGVolumn, float SoundEffectVolumn)
        {
            m_bCloseAllBGAudio = BGM == 0;
            m_bCloseEffectAudio = SoundEffect == 0;
            _SetBGVolume(BGVolumn);
            _SetEffectVolume(SoundEffectVolumn);
        }
        //------------------------------------------------------
        public float GetGroupMixer(int group)
        {
            if (m_vGroupMixer == null) return 1;
            AudioGroupMixer mixer;
            if(m_vGroupMixer.TryGetValue(group,out mixer))
            {
                return mixer.GetMixer();
            }
            return 1;
        }
        //------------------------------------------------------
        void OnDestroy()
        {
            ms_pInstnace = null;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            ms_nGUID = 0;

            m_bCloseAllBGAudio = false;
            m_bCloseEffectAudio = false;

            StopAllEffect();
            StopAllBG();
            if (m_pAgentTree != null)
            {
                m_pAgentTree.Exit();
                m_pAgentTree.Enable(false);
                AgentTreeManager.getInstance().UnloadAT(m_pAgentTree);
                m_pAgentTree = null;
            }
        }
        //------------------------------------------------------
        void FreeSnd(ISound snd, bool bDestroy =true)
        {
            if(bDestroy) snd.Destroy();
#if USE_FMOD
            if (snd is FmodSound)
            {
                if (m_vFModPools.Count < 64) m_vFModPools.Push(snd as FmodSound);
            }
            else
#endif
            {
                m_vPools.Release(snd as AudioSound);
            }
        }
#if USE_FMOD
        //------------------------------------------------------
        FmodSound MallocFmodSnd()
        {
            FmodSound snd = null;
            if (m_vFModPools.Count > 0) snd = m_vFModPools.Pop();
            else snd = new FmodSound();
            snd.volumnFactor = 1;
            snd.Fade.Clear();
            return snd;
        }
#endif
        //------------------------------------------------------
        AudioSound MallocAudioSnd()
        {
            AudioSound snd = m_vPools.Get();
            if (snd == null) snd = new AudioSound();
            snd.volumnFactor = 1;
            snd.Fade.Clear();
            snd.prepare = false;
            return snd;
        }
        //------------------------------------------------------
        public void StopAllEffect(bool isSetPermanent = false)
        {
#if USE_FMOD
            if(m_vFModEffectSounds!=null)
            {
                foreach (var db in m_vFModEffectSounds)
                {
                    db.Destroy();
                    FreeSnd(db, false);
                }
                m_vFModEffectSounds.Clear();
            }
#endif
            foreach (var db in m_vEffectSounds)
            {
                db.Value.Destroy();
                if (db.Value.audioer)
                {
                    if (m_vEffeterPool.Count < MaxEffectCount)
                    {
                        m_vEffeterPool.Enqueue(db.Value.audioer);
                    }
                    else
                        GameObject.Destroy(db.Value.audioer);
                }
                FreeSnd(db.Value, false);
            }
            m_vEffectSounds.Clear();

            if (isSetPermanent)
            {
                m_bCloseEffectAudio = true;
            }
        }
        //------------------------------------------------------
        public void StopAllBG(bool isSetPermanent = false)
        {
#if USE_FMOD
            foreach (var db in m_vFModBGSounds)
            {
                db.Destroy();
                FreeSnd(db, false);
            }
            m_vFModBGSounds.Clear();
#endif
            foreach (var db in m_vBGSounds)
            {
                db.Value.Destroy(!isSetPermanent);
                if (db.Value.audioer)
                {
                    if (m_vBGPool.Count < MaxBGCnt)
                    {
                        m_vBGPool.Enqueue(db.Value.audioer);
                    }
                    else
                        GameObject.Destroy(db.Value.audioer);
                }
                FreeSnd(db.Value, false);
            }

            m_vBGSounds.Clear();

            if (isSetPermanent)
                m_bCloseAllBGAudio = true;
        }
        //------------------------------------------------------
        void FadeOutALLBG(float fadeOut)
        {
#if USE_FMOD
            if(m_vFModBGSounds!=null)
            {
                foreach (var db in m_vFModBGSounds)
                {
                    db.Start(fadeOut, 1, 0, true);
                }
            }

#endif
            if(m_vBGSounds!=null)
            {
                foreach (var db in m_vBGSounds)
                {
                    db.Value.Start(fadeOut, 1, 0, true);
                }
            }
        }
        //------------------------------------------------------
        void FadeOutALLEffect(float fadeOut)
        {
            if (m_vEffectSounds == null) return;
#if USE_FMOD
            if(m_vFModEffectSounds!=null)
            {
                foreach (var db in m_vFModEffectSounds)
                {
                    db.Start(fadeOut, 1, 0, true);
                }
            }

#endif
            if(m_vEffectSounds!=null)
            {
                foreach (var db in m_vEffectSounds)
                {
                    db.Value.Start(fadeOut, 1, 0, true);
                }
            }
        }
        //------------------------------------------------------
        public void _SetBGVolume(float volumn)
        {
            m_BGVolume = Mathf.Clamp01(volumn);
#if USE_FMOD
            if (FModBus != null)
            {
                BusData bus;
                for (int i = 0; i < FModBus.Length; ++i)
                {
                    bus = FModBus[i];
                    if (bus.IsValid() && bus.IsBGM())
                    {
                        if (m_arrFmodBus[i].isValid())
                        {
                            m_arrFmodBus[i].setVolume(m_BGVolume);
                        }
                    }
                }
            }
#endif
            if (BG == null) return;
            foreach (var item in m_vBGSounds)
            {
                item.Value.SetVolume(m_BGVolume);
            }
        }
        //------------------------------------------------------
        public void _SetEffectVolume(float volumn)
        {
            m_SoundEffectVolume = Mathf.Clamp01(volumn);
#if USE_FMOD
            if (FModBus != null)
            {
                BusData bus;
                for (int i = 0; i < FModBus.Length; ++i)
                {
                    bus = FModBus[i];
                    if (bus.IsValid() && bus.IsEffect())
                    {
                        if (m_arrFmodBus[i].isValid())
                        {
                            m_arrFmodBus[i].setVolume(m_SoundEffectVolume);
                        }
                    }
                }
            }
#endif
            foreach (var item in m_vEffectSounds)
            {
                item.Value.SetVolume(m_SoundEffectVolume);
            }
        }
        //------------------------------------------------------
        public void SetGroupMixCurve(int group, AnimationCurve curve)
        {

        }
        //------------------------------------------------------
        void Update()
        {
            if (m_pTransform == null) return;
            m_pTransform.position = CameraUtil.mainCameraPosition;
            float fFrame = Time.deltaTime;

            if (m_vGroupMixer != null)
            {
                foreach (var db in m_vGroupMixer)
                {
                    db.Value.SetCounter(0);
                }
            }

#if USE_FMOD
            if (m_vFModEffectSounds != null)
            {
                FmodSound snd;
                for (int i =0; i < m_vFModEffectSounds.Count;)
                {
                    snd = m_vFModEffectSounds[i];
                    if (snd.IsPause())
                    {
                        ++i;
                        continue;
                    }
                    bool bOver = false;
                    bool bFade = false;
                    if (snd.Fade.IsFading())
                    {
                        bFade = snd.Fade.Evaluate(snd, fFrame);
                        if (!bFade && snd.Fade.IsOverClear())
                        {
                            bOver = true;
                        }
                    }
                    if (!snd.UpdateVolume(bFade, m_bCloseEffectAudio)) bOver = true;
                    if (bOver || !snd.IsPlaying())
                    {
                        snd.Destroy();
                        FreeSnd(snd,false);
                        m_vFModEffectSounds.RemoveAt(i);
                        continue;
                    }
                    ++i;
                }
            }

            if (m_vFModBGSounds != null)
            {
                FmodSound snd;
                for (int i = 0; i < m_vFModBGSounds.Count;)
                {
                    snd = m_vFModBGSounds[i];
                    if (snd.IsPause())
                    {
                        ++i;
                        continue;
                    }
                    bool bOver = false;
                    bool bFade = false;
                    if (snd.Fade.IsFading())
                    {
                        bFade = snd.Fade.Evaluate(snd, fFrame);
                        if (!bFade && snd.Fade.IsOverClear())
                        {
                            bOver = true;
                        }
                    }
                    if (!snd.UpdateVolume(bFade, m_bCloseAllBGAudio)) bOver = true;
                    if (bOver || !snd.IsPlaying())//关闭背景音乐的时候,不进行检测,在 StopAllBG 时,就已经释放回对象池中
                    {
                        snd.Destroy();
                        FreeSnd(snd, false);
                        m_vFModBGSounds.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        snd.SetVolumnRatio(m_BGVolume);
                        if (snd.mixerGroup >= 0)
                        {
                            AudioGroupMixer gpMixer;
                            if (m_vGroupMixer != null && m_vGroupMixer.TryGetValue(snd.mixerGroup, out gpMixer))
                                gpMixer.AddCounter(1);
                        }
                    }
                    ++i;
                }
            }
#endif

            if (m_vEffectSounds != null)
            {
                AudioSound snd;
                m_vDestroyList.Clear();
                foreach (var db in m_vEffectSounds)
                {
                    snd = db.Value;
                    if (!snd.prepare) continue;
                    if (snd.isPause) continue;
                    bool bOver = false;
                    if (snd.audioer == null) continue;
                    bool bFade = false;
                    if (snd.Fade.IsFading())
                    {
                        bFade = snd.Fade.Evaluate(snd, fFrame);
                        if (!bFade && snd.Fade.IsOverClear())
                        {
                            bOver = true;
                        }
                    }
                    if (!snd.UpdateVolume(bFade, m_bCloseEffectAudio)) bOver = true;
                    if (bOver || !snd.audioer.isPlaying)
                    {
                        RecycleAudioSource(snd.audioer);
                        snd.Destroy();
                        m_vDestroyList.Add(db.Key);
                        FreeSnd(snd, false);
                    }
                }
                foreach (var db in m_vDestroyList)
                {
                    m_vEffectSounds.Remove(db);
                }
                m_vDestroyList.Clear();
            }

            if (m_vBGSounds != null)
            {
                AudioGroupMixer gpMixer;
                AudioSound snd;
                m_vDestroyList.Clear();
                foreach (var db in m_vBGSounds)
                {
                    snd = db.Value;
                    if (!snd.prepare) continue;
                    if (snd.isPause) continue;
                    bool bOver = false;
                    if (snd.audioer == null) continue;
                    bool bFade = false;
                    if (snd.Fade.IsFading())
                    {
                        bFade = snd.Fade.Evaluate(snd, fFrame);
                        if (!bFade && snd.Fade.IsOverClear())
                        {
                            bOver = true;
                        }
                    }
                    if (!snd.UpdateVolume(bFade,m_bCloseAllBGAudio)) bOver = true;
                    if (bOver || (!snd.audioer.isPlaying && !m_bCloseAllBGAudio))//关闭背景音乐的时候,不进行检测,在 StopAllBG 时,就已经释放回对象池中
                    {
                        snd.Destroy();
                        RecycleBGAudioSource(snd.audioer);
                        m_vDestroyList.Add(db.Key);
                        FreeSnd(snd, false);
                    }
                    else
                    {
                        snd.SetVolumnRatio(m_BGVolume);
                        if (snd.mixerGroup >= 0)
                        {
                            if (m_vGroupMixer != null && m_vGroupMixer.TryGetValue(snd.mixerGroup, out gpMixer))
                                gpMixer.AddCounter(1);
                        }
                    }
                }
                foreach (var db in m_vDestroyList)
                {
                    if (!m_bCloseAllBGAudio)
                    {
                        m_vBGSounds.Remove(db);
                    }
                }
                m_vDestroyList.Clear();
            }

            if (m_vGroupMixer != null)
            {
                AudioGroupMixer gpMixer;
#if USE_FMOD

                if (m_vFmodTriggers != null)
                {
                    for (int i = 0; i < m_vFmodTriggers.Count;)
                    {
                        var fmod = m_vFmodTriggers[i];
                        if (!fmod.IsValid())
                        {
                            m_vFmodTriggers.RemoveAt(i);
                            continue;
                        }
                        if (m_vGroupMixer != null && m_vGroupMixer.TryGetValue(0, out gpMixer))
                            gpMixer.AddCounter(1);
                        ++i;
                    }
                }
#endif
                int maxOrder = -1;
                foreach (var db in m_vGroupMixer)
                {
                    gpMixer = db.Value;
                    if(gpMixer.GetCounter() >0)
                    {
                        if (maxOrder < gpMixer.GetOrder())
                        {
                            maxOrder = gpMixer.GetOrder();
                        }
                    }
                }
                foreach (var db in m_vGroupMixer)
                {
                    gpMixer = db.Value;
                    if(gpMixer.GetOrder() < maxOrder)
                    {
                        gpMixer.SetCounter(0);
                    }
                    gpMixer.Update(fFrame);
                }
            }
#if USE_FMOD
            if(m_vFmodTriggers!=null)
            {
                float fVolume = 1;
                AudioGroupMixer mixer;
                if(m_vGroupMixer!=null)
                {
                    if (m_vGroupMixer.TryGetValue(0, out mixer))
                    {
                        fVolume = mixer.GetMixer();
                    }
                }

                for (int i = 0; i < m_vFmodTriggers.Count;)
                {
                    var fmod = m_vFmodTriggers[i];
                    if(!fmod.IsValid())
                    {
                        m_vFmodTriggers.RemoveAt(i);
                        continue;
                    }
                    if (m_bCloseAllBGAudio) fmod.evtInstnance.setVolume(0);
                    else fmod.evtInstnance.setVolume(fVolume * m_BGVolume);
                    ++i;
                }
            }
#endif
        }
        //------------------------------------------------------
        public void _StopEffect(int nID)
        {
#if USE_FMOD
            if(m_vFModEffectSounds != null)
            {
                FmodSound fmdSnd;
                for(int i =0; i < m_vFModEffectSounds.Count; ++i)
                {
                    fmdSnd = m_vFModEffectSounds[i];
                    if (fmdSnd.id == nID)
                    {
                        fmdSnd.Destroy();
                        FreeSnd(fmdSnd, false);
                        m_vFModEffectSounds.RemoveAt(i);
                        break;
                    }
                }
            }
#endif
            if(m_vEffectSounds!=null)
            {
                AudioSound snd;
                if (m_vEffectSounds.TryGetValue(nID, out snd))
                {
                    FreeSnd(snd);
                    RecycleAudioSource(snd.audioer);
                    m_vEffectSounds.Remove(nID);
                }
            }

        }
        //------------------------------------------------------
        public void _StopEffect(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return;
#if USE_FMOD
            if (m_vFModEffectSounds != null)
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModEffectSounds.Count; ++i)
                {
                    fmdSnd = m_vFModEffectSounds[i];
                    if (strFile.CompareTo(fmdSnd.eventPath) == 0)
                    {
                        fmdSnd.Stop();
                        break;
                    }
                }
            }
#endif
            if (m_vEffectSounds != null)
            {
                AudioSound snd;
                for (int i = 0; i < m_vEffectSounds.Count; ++i)
                {
                    snd = m_vEffectSounds[i];
                    if (strFile.CompareTo(snd.strKey) == 0)
                    {
                        snd.Stop();
                        break;
                    }
                }
            }
        }
        //------------------------------------------------------
        public void _PauseEffect(int nID)
        {
#if USE_FMOD
            if (m_vFModEffectSounds != null)
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModEffectSounds.Count; ++i)
                {
                    fmdSnd = m_vFModEffectSounds[i];
                    if (fmdSnd.id == nID)
                    {
                        fmdSnd.Pause(true);
                        break;
                    }
                }
            }
#endif
            if (m_vEffectSounds == null) return;
            AudioSound snd;
            if (m_vEffectSounds.TryGetValue(nID, out snd))
            {
                if (snd.audioer) snd.audioer.Pause();
            }
        }
        //------------------------------------------------------
        public void _ResumeEffect(int nID)
        {
#if USE_FMOD
            if (m_vFModEffectSounds != null)
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModEffectSounds.Count; ++i)
                {
                    fmdSnd = m_vFModEffectSounds[i];
                    if (fmdSnd.id == nID)
                    {
                        fmdSnd.Pause(false);
                        break;
                    }
                }
            }
#endif
            if (m_vEffectSounds == null) return;
            AudioSound snd;
            if (m_vEffectSounds.TryGetValue(nID, out snd))
            {
                if (snd.audioer) snd.audioer.UnPause();
            }
        }
        //------------------------------------------------------
        public void _FadeOutEffect(int nID, float fFadeTime)
        {
#if USE_FMOD
            if(fFadeTime<=0)
            {
                _StopEffect(nID);
                return;
            }
            if (m_vFModEffectSounds != null)
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModEffectSounds.Count; ++i)
                {
                    fmdSnd = m_vFModEffectSounds[i];
                    if (fmdSnd.id == nID)
                    {
                        fmdSnd.Start(fFadeTime,1,0, true);
                        break;
                    }
                }
            }
#endif
            if (fFadeTime <= 0)
            {
                _StopEffect(nID);
                return;
            }
            if (m_vEffectSounds == null) return;
            AudioSound snd;
            if (m_vEffectSounds.TryGetValue(nID, out snd))
            {
                snd.Fade.Start(fFadeTime, 1, 0, true);
            }
        }
        //------------------------------------------------------
        public void _FadeOutALLEffects(float fFadeTime)
        {
#if USE_FMOD
            if (m_vFModEffectSounds != null)
            {
                for (int i = 0; i < m_vFModEffectSounds.Count; ++i)
                {
                    m_vFModEffectSounds[i].Start(fFadeTime, 1, 0, true);
                }
            }
#endif
            if(m_vEffectSounds!=null)
            {
                foreach (var db in m_vEffectSounds)
                {
                    db.Value.Fade.Start(fFadeTime, 1, 0, true);
                }
            }
        }
        //------------------------------------------------------
        public void _FadeOutALLBGs(float fFadeTime)
        {
#if USE_FMOD
            if (m_vFModBGSounds != null)
            {
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    m_vFModBGSounds[i].Start(fFadeTime, 1, 0, true);
                }
            }
#endif
            if (m_vBGSounds == null) return;
            foreach (var db in m_vBGSounds)
            {
                db.Value.Fade.Start(fFadeTime, 1, 0, true);
            }
        }
        //------------------------------------------------------
        public ISound _PlayEffectSnd(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
#if USE_FMOD
            if (strFile.StartsWith("event:/"))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID);
                    fmdSnd.Start();
                    fmdSnd.SetVolumnRatio(m_SoundEffectVolume);
                    fmdSnd.eventPath = strFile;
                    fmdSnd.SetGroup(-1);
                    m_vFModEffectSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
            }
#endif
              
            AssetOperiaon pOp = FileSystemUtil.AsyncReadFile(strFile, OnLoad);
            if (pOp != null)
            {
                AudioSound snd = MallocAudioSnd();
                snd.Guid = ++ms_nGUID;
                snd.pFollow = null;
                snd.bUseFollow = false;
                snd.use3DPosition = false;
                snd.triggerPosition = Vector3.zero;
                snd.SetVolumnRatio(m_SoundEffectVolume);
                snd.strKey = strFile;
                snd.SetGroup(-1);
                pOp.userData = new Variable1() { intVal = snd.Guid };
                pOp.OnCallback = OnLoad;
                m_vEffectSounds.Add(snd.Guid, snd);
                return snd;
            }
            return null;

        }
        //------------------------------------------------------
        public ISound _PlayEffectVolume(string strFile, float fVolume)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            if (Effecter == null) return null;
            if (m_vEffectSounds.Count >= MaxEffectCount) return null;

            ISound snd = _PlayEffectSnd(strFile);
            if (snd != null)
            {
                snd.SetVolume(m_SoundEffectVolume * fVolume);
                return snd;
            }
            return null;
        }
        //------------------------------------------------------
        public ISound _PlayEffectVolume(string strFile, float fVolume, Vector3 position)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
#if USE_FMOD
            if (strFile.StartsWith("event:/"))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID, strFile);
                    fmdSnd.Start();
                    fmdSnd.SetVolume(fVolume);
                    fmdSnd.eventPath = strFile;
                    fmdSnd.use3DPosition = true;
                    fmdSnd.triggerPosition = position;
                    fmdSnd.SetGroup(-1);
                    fmdSnd.SetVolumnRatio(m_SoundEffectVolume);
                    m_vFModEffectSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
            }
#endif
            if (Effecter == null) return null;

            if (m_vEffectSounds.Count >= MaxEffectCount) return null;

            AudioSound snd = (AudioSound)_PlayEffectSnd(strFile);
            if (snd != null)
            {
                snd.use3DPosition = true;
                snd.triggerPosition = position;
                snd.SetVolume(fVolume);
                snd.SetVolumnRatio(m_SoundEffectVolume);
                snd.SetGroup(-1);
                return snd;
            }
            return null;
        }
        //------------------------------------------------------
        public ISound _Play3DEffectVolume(string strFile, float fVolume, Transform pTrans)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
#if USE_FMOD
            if (strFile.StartsWith("event:/"))
            {
                FmodSound snd = (FmodSound)_PlayEffectSnd(strFile);
                if (snd.IsValid())
                {
                    snd.pFollow = pTrans;
                    snd.bUseFollow = true;
                    snd.SetVolume( fVolume);
                    snd.SetVolumnRatio(m_SoundEffectVolume);
                    return snd;
                }
                return null;
            }
#endif
            {
                if (Effecter == null) return null;

                if (m_vEffectSounds.Count >= MaxEffectCount) return null;

                AudioSound snd = (AudioSound)_PlayEffectSnd(strFile);
                if (snd != null)
                {
                    snd.pFollow = pTrans;
                    snd.bUseFollow = true;
                    snd.SetVolume(fVolume);
                    snd.SetVolumnRatio(m_SoundEffectVolume);
                    return snd;
                }
            }

            return null;
        }
        //------------------------------------------------------
        public ISound _PlayID(uint nId, Transform pTrans = null, bool bMix = false)
        {
            if (nId <= 0 || AFramework.mainFramework == null) return null;
            if (m_vEffectSounds.Count >= MaxEffectCount) return null;
            if (Effecter == null) return null;
            var sndData = AFramework.mainFramework.GetSoundByID(nId);
            if (!sndData.IsValid()) return null;

            float volume = Mathf.Max(0, UnityEngine.Random.Range(Mathf.Min(sndData.volume.x, sndData.volume.y), Mathf.Max(sndData.volume.x, sndData.volume.y))) * 0.01f;
            if (volume <= 0) return null;

            ISound snd = null;
            if ((ESoundType)sndData.type == ESoundType.Effect)
            {
                if (sndData.b3D && pTrans) snd = _Play3DEffectVolume(sndData.location, volume, pTrans);
                else snd = _PlayEffectVolume(sndData.location, volume);
            }
            else if ((ESoundType)sndData.type == ESoundType.BG)
            {
                if (bMix) snd = _MixBG(sndData.location);
                snd = _PlayBG(sndData.location);
            }
            if (snd != null) snd.SetGroup(sndData.group);
            return snd;
        }
        //------------------------------------------------------
        public ISound _PlayID(uint nId, Vector3 position, bool bMix = false)
        {
            if (nId <= 0 || AFramework.mainFramework == null) return null;

            if (m_vEffectSounds.Count >= MaxEffectCount) return null;
            if (Effecter == null) return null;
            var sndData = AFramework.mainFramework.GetSoundByID(nId);
            if (!sndData.IsValid()) return null;

            float volume = Mathf.Max(0, UnityEngine.Random.Range(Mathf.Min(sndData.volume.x, sndData.volume.y), Mathf.Max(sndData.volume.x, sndData.volume.y))) * 0.01f;
            if (volume <= 0) return null;

            ISound snd = null;
            if ((ESoundType)sndData.type == ESoundType.Effect)
            {
                snd = _PlayEffectVolume(sndData.location, volume, position);
            }
            else if ((ESoundType)sndData.type == ESoundType.BG)
            {
                if (bMix) snd = _MixBG(sndData.location);
                snd = _PlayBG(sndData.location);
            }
            if (snd != null)
                snd.SetGroup(sndData.group);
            return snd;
        }
        //------------------------------------------------------
        public void _StopBG(int nID)
        {
#if USE_FMOD
            if(m_vFModBGSounds!=null)
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fmdSnd = m_vFModBGSounds[i];
                    if (fmdSnd.id == nID)
                    {
                        fmdSnd.Destroy();
                        FreeSnd(fmdSnd, false);
                        m_vFModBGSounds.RemoveAt(i);
                        break;
                    }
                }
            }
#endif
            if(m_vBGSounds!=null)
            {
                AudioSound snd;
                if (m_vBGSounds.TryGetValue(nID, out snd))
                {
                    FreeSnd(snd);
                    RecycleBGAudioSource(snd.audioer);
                    m_vBGSounds.Remove(nID);
                }
            }
        }
        //------------------------------------------------------
        public void _StopBG(string strMusic)
        {
            if (string.IsNullOrEmpty(strMusic)) return;
            if(strMusic.StartsWith("event:/"))
            {
#if USE_FMOD
            #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strMusic);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.PreviewStop(eventRef);
                    }
                    return;
                }
            #endif
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModBGSounds.Count;)
                {
                    fmdSnd = m_vFModBGSounds[i];
                    if (strMusic.CompareTo(fmdSnd.eventPath) == 0)
                    {
                        fmdSnd.Fade.Start(1, 1, 0, true);
                        break;
//                         fmdSnd.Destroy();
//                         FreeSnd(fmdSnd, false);
//                         m_vFModBGSounds.RemoveAt(i);
//                        continue;
                    }
                    ++i;
                }
#endif
            }
            else
            {
                if(m_vBGSounds!=null)
                {
                    foreach (var db in m_vBGSounds)
                    {
                        if (db.Value.strKey.CompareTo(strMusic) == 0)
                        {
                            _StopBG(db.Key);
                            break;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void _FadeOutBG(int nID, float fFadeTime)
        {
            if (fFadeTime <= 0)
            {
                _StopEffect(nID);
                return;
            }
#if USE_FMOD
            FmodSound fmdSnd;
            for (int i = 0; i < m_vFModBGSounds.Count; ++i)
            {
                fmdSnd = m_vFModBGSounds[i];
                if (fmdSnd.id == nID)
                {
                    fmdSnd.Start(fFadeTime, 1,0,true);
                    m_vFModBGSounds[i] = fmdSnd;
                    break;
                }
            }
#endif
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(nID, out snd))
            {
                snd.Fade.Start(fFadeTime, 1, 0, true);
            }
        }
        //------------------------------------------------------
        public void _FadeOutBGByName(string strName, float fFadeTime)
        {
            //throw new System.Exception("AudioManager void _FadeOutBGByName(string strName, float fFadeTime");
            if (string.IsNullOrEmpty(strName)) return;
            if (fFadeTime <= 0)
            {
                _StopBG(strName);
                return;
            }
#if USE_FMOD
            FmodSound fmdSnd;
            for (int i = 0; i < m_vFModBGSounds.Count; ++i)
            {
                fmdSnd = m_vFModBGSounds[i];
                if (strName.CompareTo(fmdSnd.eventPath) == 0)
                {
                    fmdSnd.Start(fFadeTime, 1, 0, true);
                }
            }
#endif
            foreach (var db in m_vBGSounds)
            {
                if (strName.CompareTo(db.Value.strKey) == 0)
                {
                    db.Value.Start(fFadeTime, 1, 0, true);
                }
            }
        }
        //------------------------------------------------------
        public void _FadeOutBG(string strMusic, float fFadeTime)
        {
            if (string.IsNullOrEmpty(strMusic)) return;
            //throw new System.Exception("AudioManager void _FadeOutBG(string strMusic, float fFadeTime)");
            if (fFadeTime <= 0)
            {
                _StopBG(strMusic);
                return;
            }
#if USE_FMOD
            if (strMusic.StartsWith("event:/"))
            {
                FmodSound fmdSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fmdSnd = m_vFModBGSounds[i];
                    if (strMusic.CompareTo(fmdSnd.eventPath) == 0)
                    {
                        fmdSnd.Start(fFadeTime, 1, 0, true);
                    }
                }
                return;
            }
#endif
            foreach (var db in m_vBGSounds)
            {
                if (db.Value.strKey.CompareTo(strMusic) == 0)
                {
                    db.Value.Fade.Start(fFadeTime, 1, 0, true);
                }
            }
        }
        //------------------------------------------------------
        public ISound _PlayBG(UnityEngine.AudioClip clip, bool bStopAllBG = true, bool bLoop = true, string fastName = null)
        {
            if (clip == null) return null;
            if (string.IsNullOrEmpty(fastName))
                fastName = clip.name;
            if (!bStopAllBG && m_vBGSounds.Count >= MaxBGCnt) return null;

            if (!bStopAllBG)
            {
                foreach (var db in m_vBGSounds)
                {
                    if (db.Value.strKey.CompareTo(fastName) == 0)
                    {
                        return db.Value;
                    }
                }
            }

            AudioSource audioer = NewBGAudioSource();
            if (audioer)
            {
                AudioSound snd = MallocAudioSnd();
                snd.Guid = ++ms_nGUID;
                snd.strKey = fastName;
                snd.pAsset = null;
                snd.audioer = audioer;
                snd.audioer.loop = bLoop;
                snd.audioer.enabled = true;
                snd.audioer.clip = clip;
                snd.SetVolumnRatio(m_BGVolume);
                if (!m_bCloseAllBGAudio)
                {
                    snd.audioer.Play();
                }
                if (bStopAllBG)
                {
                    foreach (var db in m_vBGSounds)
                        db.Value.Fade.Start(BGFade, 1, 0, true);
                }
                snd.Fade.Start(BGFade, 0, 1, false);
                m_vBGSounds[snd.Guid] = snd;
                return snd;
            }

            return null;
        }
        //------------------------------------------------------
        public ISound _PlayBG(string strFile, bool bStopAllBG = true, bool bLoop = true, string fastName = null)
        {
            //throw new System.Exception("AudioManager ISound _PlayBG(string strFile, bool bStopAllBG = true, bool bLoop = true, string fastName = null)");
            if (string.IsNullOrEmpty(strFile)) return null;
            if (string.IsNullOrEmpty(fastName))
                fastName = strFile;
#if USE_FMOD
            if (strFile.StartsWith("event:/"))
            {
                FmodSound fadeSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fadeSnd = m_vFModBGSounds[i];
                    if (fastName.CompareTo(fadeSnd.eventPath) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Start(0.5f, 0, 1, false);
                        }
                        if (bStopAllBG)
                        {
                            for (int j = 0; j < m_vFModBGSounds.Count; ++j)
                            {
                                if (i == j) continue;
                                m_vFModBGSounds[j].Start(0.5f, 1, 0, true);
                            }
                        }
                        return fadeSnd;
                    }
                }
                if (bStopAllBG)
                {
                    for (int j = 0; j < m_vFModBGSounds.Count; ++j)
                    {
                        m_vFModBGSounds[j].Start(0.5f, 1, 0, true);
                    }

                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID);
                    fmdSnd.Start();
                    fmdSnd.eventPath = strFile;
                    m_vFModBGSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
            }
#endif
            {
                if (BG == null) return null;

                if (!bStopAllBG && m_vBGSounds.Count >= MaxBGCnt) return null;

                AudioSound fadeSnd;
                foreach (var db in m_vBGSounds)
                {
                    fadeSnd = db.Value;
                    if (fastName.CompareTo(fadeSnd.strKey) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Fade.Start(BGFade, 0, 1, false);
                        }
                        if (bStopAllBG)
                        {
                            foreach (var tmp in m_vBGSounds)
                            {
                                if (tmp.Value != fadeSnd)
                                    tmp.Value.Fade.Start(BGFade, 1, 0, true);
                            }
                        }
                        return db.Value;
                    }
                }

                AssetOperiaon pOp = FileSystemUtil.AsyncReadFile(strFile, OnLoadBG);
                if (pOp != null)
                {
                    AudioSound snd = MallocAudioSnd();
                    snd.Guid = ++ms_nGUID;
                    snd.strKey = fastName;
                    pOp.userData = new Variable2() { intVal0 = snd.Guid, intVal1 = bLoop ? 1 : 0 };

                    if (bStopAllBG)
                    {
                        foreach (var tmp in m_vBGSounds)
                        {
                            tmp.Value.Fade.Start(BGFade, 1, 0, true);
                        }
                    }
                    snd.Fade.Start(BGFade, 0, 1, false);
                    m_vBGSounds.Add(snd.Guid, snd);
                    return snd;
                }
            }
            
            return null;
        }
        //------------------------------------------------------
        public ISound _MixBG(string strFile, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            //throw new System.Exception("AudioManager ISound _MixBG(string strFile, bool bLoop = true, string fastName = null)");
            if (string.IsNullOrEmpty(strFile)) return null;
            if (string.IsNullOrEmpty(fastName))
                fastName = strFile;
#if USE_FMOD
            if (strFile.StartsWith("event:/"))
            {
                FmodSound fadeSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fadeSnd = m_vFModBGSounds[i];
                    if (fastName.CompareTo(fadeSnd.eventPath) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Start(0.5f, 0, 1, false);
                        }
                        fadeSnd.SetGroup(mixGroup);
                        return fadeSnd;
                    }
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID);
                    fmdSnd.Start();
                    fmdSnd.SetGroup(mixGroup);
                    fmdSnd.eventPath = strFile;
                    m_vFModBGSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
            }
#endif
            {
                if (BG == null) return null;

                if (m_vBGSounds.Count >= MaxBGCnt)
                {
                    Debug.LogWarning("已达背景音乐最大可播放数");
                    return null;
                }
                AudioSound fadeSnd;
                foreach (var db in m_vBGSounds)
                {
                    fadeSnd = db.Value;
                    if (fastName.CompareTo(fadeSnd.strKey) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Fade.Start(BGFade, 0, 1, false);
                        }
                        fadeSnd.SetGroup(mixGroup);
                        return db.Value;
                    }
                }

                AssetOperiaon pOp = FileSystemUtil.AsyncReadFile(strFile, OnLoadBG);
                if (pOp != null)
                {
                    AudioSound snd = MallocAudioSnd();
                    snd.Guid = ++ms_nGUID;
                    snd.strKey = fastName;
                    pOp.userData = new Variable2() { intVal0 = snd.Guid, intVal1 = bLoop ? 1 : 0 };

                    snd.Fade.Start(BGFade, 0, 1, false);
                    snd.SetVolumnRatio(m_BGVolume);
                    snd.SetGroup(mixGroup);
                    m_vBGSounds.Add(snd.Guid, snd);
                    return snd;
                }
            }
           
            return null;
        }
        //------------------------------------------------------
        public ISound _PlayBG(string strFile, AnimationCurve fadeCurve, bool bAllBGFadeStop = true, bool bLoop = true, string fastName = null)
        {
            //throw new System.Exception("AudioManager ISound _PlayBG(string strFile, AnimationCurve fadeCurve, bool bAllBGFadeStop = true, bool bLoop = true, string fastName = null)");

            if (string.IsNullOrEmpty(strFile)) return null;
            if (string.IsNullOrEmpty(fastName))
                fastName = strFile;

#if USE_FMOD
            if(strFile.StartsWith("event:/"))
            {
                FmodSound fadeSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fadeSnd = m_vFModBGSounds[i];
                    if (fastName.CompareTo(fadeSnd.eventPath) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Start(fadeCurve, 0, 1, false);
                        }
                        if (bAllBGFadeStop)
                        {
                            for (int j = 0; j < m_vFModBGSounds.Count; ++j)
                            {
                                if (i == j) continue;
                                m_vFModBGSounds[j].Start(fadeCurve, 1, 0, true);
                            }
                        }
                        return fadeSnd;
                    }
                }
                if (bAllBGFadeStop)
                {
                    for (int j = 0; j < m_vFModBGSounds.Count; ++j)
                    {
                        m_vFModBGSounds[j].Start(fadeCurve, 1, 0, true);
                    }
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID);
                    fmdSnd.Start();
                    fmdSnd.eventPath = strFile;
                    m_vFModBGSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
            }
#endif
            {
                if (BG == null) return null;
                AudioSound fadeSnd;
                foreach (var db in m_vBGSounds)
                {
                    fadeSnd = db.Value;
                    if (fastName.CompareTo(fadeSnd.strKey) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Fade.Start(fadeCurve, 0, 1, false);
                        }
                        if (bAllBGFadeStop)
                        {
                            foreach (var tmp in m_vBGSounds)
                            {
                                if (tmp.Value != fadeSnd)
                                    tmp.Value.Fade.Start(fadeCurve, 1, 0, true);
                            }
                        }
                        return db.Value;
                    }
                }

                AssetOperiaon pOp = FileSystemUtil.AsyncReadFile(strFile, OnLoadBG);
                if (pOp != null)
                {
                    AudioSound snd = MallocAudioSnd();
                    snd.Guid = ++ms_nGUID;
                    snd.strKey = fastName;
                    pOp.userData = new Variable2() { intVal0 = snd.Guid, intVal1 = bLoop ? 1 : 0 };

                    if (bAllBGFadeStop)
                    {
                        foreach (var tmp in m_vBGSounds)
                        {
                            tmp.Value.Fade.Start(fadeCurve, 1, 0, true);
                        }
                    }
                    snd.Fade.Start(fadeCurve, 0, 1, false);
                    snd.SetVolumnRatio(m_BGVolume);
                    m_vBGSounds.Add(snd.Guid, snd);
                    return snd;
                }
            }
            
            return null;
        }
        //------------------------------------------------------
        public ISound _MixBG(string strFile, AnimationCurve fadeCurve, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            //throw new System.Exception("AudioManager ISound _MixBG(string strFile, AnimationCurve fadeCurve, bool bLoop = true, string fastName = null)");
            if (string.IsNullOrEmpty(strFile)) return null;
            if (string.IsNullOrEmpty(fastName))
                fastName = strFile;
            if(strFile.StartsWith("event:/"))
            {
#if USE_FMOD
                FmodSound fadeSnd;
                for (int i = 0; i < m_vFModBGSounds.Count; ++i)
                {
                    fadeSnd = m_vFModBGSounds[i];
                    if (fastName.CompareTo(fadeSnd.eventPath) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Start(fadeCurve, 0, 1, false);
                        }
                        fadeSnd.eventPath = strFile;
                        fadeSnd.SetGroup(mixGroup);
                        return fadeSnd;
                    }
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FMODUnity.EditorEventRef eventRef = FMODUnity.EventCache.GetEventRef(strFile);
                    if (eventRef != null)
                    {
                        FMODUnity.EditorUtils.LoadPreviewBanks();
                        FMODUnity.EditorUtils.PreviewEvent(eventRef, new Dictionary<string, float>());
                    }
                    return null;
                }
#endif
                FMOD.Studio.EventInstance eventIns = FMODUnity.RuntimeManager.CreateInstance(strFile);
                if (eventIns.isValid())
                {
                    FmodSound fmdSnd = MallocFmodSnd();
                    fmdSnd.Set(eventIns, ++ms_nGUID);
                    fmdSnd.SetGroup(mixGroup);
                    fmdSnd.eventPath = strFile;
                    fmdSnd.Start();
                    m_vFModBGSounds.Add(fmdSnd);
                    return fmdSnd;
                }
                return null;
#endif
            }
            else
            {
                if (BG == null) return null;

                if (m_vBGSounds.Count >= MaxBGCnt)
                {
                    Debug.LogWarning("已达背景音乐最大可播放数");
                    return null;
                }

                AudioSound fadeSnd;
                foreach (var db in m_vBGSounds)
                {
                    fadeSnd = db.Value;
                    if (fastName.CompareTo(fadeSnd.strKey) == 0)
                    {
                        if (fadeSnd.IsFadeingOut())
                        {
                            fadeSnd.Fade.Start(fadeCurve, 0, 1, false);
                        }
                        fadeSnd.SetGroup(mixGroup);
                        return db.Value;
                    }
                }

                if (!BaseUtil.IsValidCurve(fadeCurve))
                {
                    return _MixBG(strFile, bLoop, fastName,mixGroup);
                }

                AssetOperiaon pOp = FileSystemUtil.AsyncReadFile(strFile, OnLoadBG);
                if (pOp != null)
                {
                    AudioSound snd = MallocAudioSnd();
                    snd.Guid = ++ms_nGUID;
                    snd.SetGroup(mixGroup);
                    snd.strKey = fastName;
                    pOp.userData = new Variable2() { intVal0 = snd.Guid, intVal1 = bLoop ? 1 : 0 };
                    snd.Fade.Start(fadeCurve, 0, 1, false);
                    snd.SetVolumnRatio(m_BGVolume);
                    m_vBGSounds.Add(snd.Guid, snd);
                    return snd;
                }
            }
            
            return null;
        }
        //------------------------------------------------------
        public void _SetBGVolume(int sound, float fVolume)
        {
#if USE_FMOD
            FmodSound fmodSnd;
            for(int i =0; i < m_vFModBGSounds.Count; ++i)
            {
                fmodSnd = m_vFModBGSounds[i];
                if (fmodSnd.IsValid() && fmodSnd.id == sound)
                {
                    fmodSnd.SetVolume(fVolume);
                    break;
                }
            }
#endif
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(sound, out snd))
            {
                if (snd.audioer) snd.audioer.volume = Mathf.Clamp01(fVolume);
            }
        }
        //------------------------------------------------------
        public float _GetBGVolume(int sound)
        {
#if USE_FMOD
            FmodSound fmodSnd;
            for (int i = 0; i < m_vFModBGSounds.Count; ++i)
            {
                fmodSnd = m_vFModBGSounds[i];
                if (fmodSnd.IsValid() && fmodSnd.id == sound)
                {
                    return fmodSnd.GetVolume();
                }
            }
#endif
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(sound, out snd))
            {
                if (snd.audioer) return snd.audioer.volume;
            }
            return 0;
        }
        //------------------------------------------------------
        public void _PauseBG(int nID)
        {
#if USE_FMOD
            FmodSound fmodSnd;
            for (int i = 0; i < m_vFModBGSounds.Count; ++i)
            {
                fmodSnd = m_vFModBGSounds[i];
                if (fmodSnd.IsValid() && fmodSnd.id == nID)
                {
                    fmodSnd.Pause(true);
                }
            }
#endif
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(nID, out snd))
            {
                snd.isPause = true;
                if (snd.audioer) snd.audioer.Pause();
            }
        }
        //------------------------------------------------------
        public void _ResumeBG(int nID)
        {
#if USE_FMOD
            FmodSound fmodSnd;
            for (int i = 0; i < m_vFModBGSounds.Count; ++i)
            {
                fmodSnd = m_vFModBGSounds[i];
                if (fmodSnd.IsValid() && fmodSnd.id == nID)
                {
                    fmodSnd.Pause(false);
                }
            }
#endif
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(nID, out snd))
            {
                snd.isPause = false;
                if (snd.audioer) snd.audioer.UnPause();
            }
        }
        //------------------------------------------------------
        void OnLoadBG(AssetOperiaon pAo)
        {
            if (pAo.userData == null) return;
            int guid = ((Variable2)pAo.userData).intVal0;
            bool bLoop = ((Variable2)pAo.userData).intVal1 != 0;
            if (!pAo.isValid())
            {
                m_vBGSounds.Remove(guid);
                return;
            }
            AudioSound snd;
            if (m_vBGSounds.TryGetValue(guid, out snd))
            {
                snd.pAsset = pAo.pAsset;
                snd.prepare = true;
                if (snd.pAsset.GetOrigin() as UnityEngine.AudioClip)
                {
                    snd.audioer = NewBGAudioSource();
                    if (snd.audioer)
                    {
                        snd.pAsset.Grab();
                        snd.audioer.loop = bLoop;
                        snd.audioer.enabled = true;
                        snd.audioer.clip = snd.pAsset.GetOrigin() as UnityEngine.AudioClip;
                        snd.SetVolume(m_BGVolume);
                        if (!m_bCloseAllBGAudio)
                        {
                            snd.audioer.Play();
                        }
                    }
                    else
                    {
                        FreeSnd(snd);
                    }
                }
            }
        }
        //------------------------------------------------------
        void OnLoad(AssetOperiaon pAo)
        {
            if (pAo.userData == null) return;
            int guid = ((Variable1)pAo.userData).intVal;
            if (!pAo.isValid())
            {
                m_vEffectSounds.Remove(guid);
                return;
            }
            AudioSound snd;
            if (m_vEffectSounds.TryGetValue(guid, out snd))
            {
                snd.pAsset = pAo.pAsset;
                snd.prepare = true;
                if (snd.pAsset.GetOrigin() as UnityEngine.AudioClip)
                {
                    snd.audioer = NewAudioSource();
                    if (snd.audioer)
                    {
                        snd.pAsset.Grab();
                        snd.audioer.clip = snd.pAsset.GetOrigin() as UnityEngine.AudioClip;
                        snd.audioer.enabled = !m_bCloseEffectAudio;
                        snd.SetVolume(snd.volumn);
                        snd.SetVolumnRatio(m_SoundEffectVolume);
                        if (!m_bCloseEffectAudio)
                        {
                            snd.audioer.Play();
                        }
                        m_vEffectSounds[guid] = snd;
                    }
                    else
                    {
                        FreeSnd(snd);
                        m_vEffectSounds.Remove(guid);
                    }
                }
            }
        }
        //------------------------------------------------------
        AudioSource NewAudioSource()
        {
            if (m_vEffeterPool.Count > 0)
                return m_vEffeterPool.Dequeue();
            if (m_vEffeterPool.Count + m_vEffectSounds.Count < MaxEffectCount)
            {
                return Effecter.AddComponent<AudioSource>();
            }
            return null;
        }
        //------------------------------------------------------
        void RecycleAudioSource(AudioSource audio)
        {
            if (audio)
            {
                audio.enabled = false;
                audio.loop = false;
                if (m_vEffeterPool.Count < MaxEffectCount)
                {
                    m_vEffeterPool.Enqueue(audio);
                }
                else
                    GameObject.Destroy(audio);
            }
        }
        //------------------------------------------------------
        void RecycleBGAudioSource(AudioSource audio)
        {
            if (audio)
            {
                audio.enabled = false;
                audio.loop = false;
                if (m_vBGPool.Count < MaxBGCnt)
                {
                    m_vBGPool.Enqueue(audio);
                }
                else
                    GameObject.Destroy(audio);
            }
        }
        //------------------------------------------------------
        AudioSource NewBGAudioSource()
        {
            if (m_vBGPool.Count > 0)
                return m_vBGPool.Dequeue();
            if (BG == null) return null;
            if (m_vBGPool.Count + m_vBGPool.Count < MaxBGCnt)
            {
                return BG.AddComponent<AudioSource>();
            }
            return null;
        }
        //------------------------------------------------------
#if USE_FMOD
        void OnFmodEventTrigger(string mixGroup, FMOD.Studio.EventInstance evtInstnance, FMODUnity.EventReference eventRef)
        {
            if (string.IsNullOrEmpty(mixGroup) || eventRef.IsNull || !evtInstnance.isValid()) return;
            if(mixGroup.CompareTo("BG") ==0)
            {
                if (m_vFmodTriggers == null) m_vFmodTriggers = new List<FMODTrigger>();
                 FMODTrigger trigger = new FMODTrigger();
                trigger.eventRef = eventRef;
                trigger.evtInstnance = evtInstnance;
                trigger.isBG = true;
                m_vFmodTriggers.Add(trigger);
            }
        }
#endif
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("停止音效")]
        public static void StopEffect(int nID)
        {
            AudioUtil.StopEffect(nID);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("FadeOut音效")]
        public static void FadeOutEffect(int nID, float fFadeTime)
        {
            AudioUtil.FadeOutEffect(nID, fFadeTime);
        }
        //------------------------------------------------------
        public static void FadeOutALLEffects(float fFadeTime)
        {
            AudioUtil.FadeOutALLEffects(fFadeTime);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("暂停音效")]
        public static void PauseEffect(int nID)
        {
            AudioUtil.PauseEffect(nID);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("恢复音效")]
        public static void ResumeEffect(int nID)
        {
            AudioUtil.ResumeEffect(nID);
        }
        //------------------------------------------------------
        protected static int PlayEffectSnd(string strFile)
        {
            ISound snd = AudioUtil.PlayEffectSnd(strFile);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放FMOD声音-3D")]
        public static void PlayFMOD3D(string strEvent, GameObject pGo)
        {
#if USE_FMOD
            if (string.IsNullOrEmpty(strEvent)) return;
            FMODUnity.RuntimeManager.PlayOneShotAttached(strEvent, pGo);
#endif
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效", new System.Type[] { typeof(AudioClip) })]
        public static int PlayEffect(string strFile)
        {
            ISound snd = AudioUtil.PlayEffectVolume(strFile,1);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效", new System.Type[] { typeof(AudioClip) })]
        public static int Play3DEffect(string strFile, Transform pTrans)
        {
            ISound snd = AudioUtil.Play3DEffectVolume(strFile, 1, pTrans);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效-Volume", new System.Type[] { typeof(AudioClip) })]
        public static int PlayEffectVolume(string strFile, float fVolume)
        {
            ISound snd = AudioUtil.PlayEffectSnd(strFile);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放3D音效-Transform", new System.Type[] { typeof(AudioClip) })]
        public static int Play3DEffectVolume(string strFile, float fVolume, Transform pTrans)
        {
            ISound snd = AudioUtil.Play3DEffectVolume(strFile, fVolume, pTrans);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放3D音效-Position", new System.Type[] { typeof(AudioClip) })]
        public static int PlayEffectVolume(string strFile, float fVolume, Vector3 position)
        {
            ISound snd = AudioUtil.PlayEffectVolume(strFile, fVolume, position);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效-Transform", new System.Type[] { typeof(AudioClip) })]
        public static int PlayID(uint nId, Transform pTrans = null, bool bMix = false)
        {
            ISound snd = AudioUtil.PlayID(nId, pTrans, bMix);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效-Position", new System.Type[] { typeof(AudioClip) })]
        public static int PlayID(uint nId, Vector3 position, bool bMix = false)
        {
            ISound snd = AudioUtil.PlayID(nId, position, bMix);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("播放音效(clip)")]
        public static int PlayEffect(UnityEngine.AudioClip clip)
        {
            if (clip == null) return 0;
            AudioManager mgr = ms_pInstnace;
            if (mgr == null || mgr.Effecter == null) return 0;

            if (mgr.m_vEffectSounds.Count >= mgr.MaxEffectCount) return 0;

            AudioSource audioer = mgr.NewAudioSource();
            if (audioer)
            {
                AudioSound snd = mgr.MallocAudioSnd();
                snd.Guid = ++ms_nGUID;
                snd.strKey = clip.name;
                snd.pAsset = null;
                snd.audioer = audioer;
                snd.audioer.clip = clip;
                snd.audioer.enabled = true;
                //snd.SetVolume(m_SoundEffectVolume);
                if (!mgr.m_bCloseEffectAudio)
                {
                    snd.audioer.Play();
                }
                mgr.m_vEffectSounds.Add(snd.Guid, snd);
                return snd.Guid;
            }
            return 0;
        }
        //------------------------------------------------------
        public static float GetEffectVolume(int sound)
        {
            if (sound == 0) return 0;

            AudioManager mgr = ms_pInstnace;
            if (mgr == null) return 0;
#if USE_FMOD
            for(int i =0; i < mgr.m_vFModEffectSounds.Count; ++i)
            {
                if (mgr.m_vFModEffectSounds[i].id == sound)
                    return mgr.m_vFModEffectSounds[i].GetVolume();
            }
#endif
            AudioSound snd;
            if (mgr.m_vEffectSounds.TryGetValue(sound, out snd))
            {
                if (snd.audioer) return snd.audioer.volume;
            }
            return 0;
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("停止所有")]
        public static void StopAll(bool bBG= true, bool bEffect=true)
        {
            AudioUtil.StopAll(bBG, bEffect);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("FadeOut所有")]
        public static void FadeOutAll(float fadeOutBG = 0.1f, float fadeOutEffect = 0.1f)
        {
            AudioUtil.FadeOutAll(fadeOutBG, fadeOutEffect);
        }
        //------------------------------------------------------
        public static void OnSoundATEvent(string evtName, IUserData pParam = null)
        {
            AudioManager mgr = ms_pInstnace;
            if (mgr == null) return;
            if (mgr.m_pAgentTree == null) return;
            mgr.m_pAgentTree.ExecuteCustom(evtName, pParam);
        }
        //------------------------------------------------------
        public static void OnSoundATEvent(int evtID, IUserData pParam = null)
        {
            AudioManager mgr = ms_pInstnace;
            if (mgr == null) return;
            if (mgr.m_pAgentTree == null) return;
            mgr.m_pAgentTree.ExecuteCustom(evtID, pParam);
        }
        //------------------------------------------------------
        public static void OnSoundATEvent(GameObject pGO, IUserData pParam = null)
        {
            AudioManager mgr = ms_pInstnace;
            if (mgr == null) return;
            if (mgr.m_pAgentTree == null) return;
            mgr.m_pAgentTree.ExecuteCustom(pGO, pParam);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("停止背景音乐")]
        public static void StopBG(int nID)
        {
            AudioUtil.StopBG(nID);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("FadeOut背景音乐")]
        public static void FadeOutBG(int nID, float fFadeTime)
        {
            AudioUtil.FadeOutBG(nID, fFadeTime);
        }
        //------------------------------------------------------
        [Framework.Plugin.AT.ATMethod("FadeOut(Name)背景音乐")]
        public static void FadeOutBGByName(string strName, float fFadeTime)
        {
            AudioUtil.FadeOutBGByName(strName, fFadeTime);
        }
        //------------------------------------------------------
        public static void StopBG(string strMusic)
        {
            AudioUtil.StopBG(strMusic);
        }
        //------------------------------------------------------
        public static void FadeOutBG(string strMusic, float fFadeTime)
        {
            AudioUtil.FadeOutBG(strMusic, fFadeTime);
        }
        //------------------------------------------------------
        public static void FadeOutALLBGs(float fFadeTime)
        {
            AudioUtil.FadeOutALLBGs(fFadeTime);
        }
        //------------------------------------------------------
        public static void SetBGVolume(int sound, float fVolume)
        {
            AudioUtil.SetBGVolume(sound, fVolume);
        }
        //------------------------------------------------------
        public static float GetBGVolume(int sound)
        {
            return AudioUtil.GetBGVolume(sound);
        }
        //------------------------------------------------------
        public static void PauseBG(int nID)
        {
            AudioUtil.PauseBG(nID);
        }
        //------------------------------------------------------
        public static void ResumeBG(int nID)
        {
            AudioUtil.ResumeBG(nID);
        }
        //------------------------------------------------------
        public static int PlayBG(UnityEngine.AudioClip clip, bool bStopAllBG = true, bool bLoop = true, string fastName = null)
        {
            if (clip == null) return 0;
            AudioManager mgr = ms_pInstnace;
            if (string.IsNullOrEmpty(fastName))
                fastName = clip.name;
            if (!bStopAllBG && mgr.m_vBGSounds.Count >= mgr.MaxBGCnt) return 0;

            if (!bStopAllBG)
            {
                foreach (var db in mgr.m_vBGSounds)
                {
                    if (db.Value.strKey.CompareTo(fastName) == 0)
                    {
                        return db.Key;
                    }
                }
            }

            AudioSource audioer = mgr.NewBGAudioSource();
            if (audioer)
            {
                AudioSound snd = mgr.MallocAudioSnd();
                snd.Guid = ++ms_nGUID;
                snd.strKey = fastName;
                snd.pAsset = null;
                snd.audioer = audioer;
                snd.audioer.loop = bLoop;
                snd.audioer.enabled = true;
                snd.audioer.clip = clip;
                //snd.SetVolume(m_BGVolume);
                if (!mgr.m_bCloseAllBGAudio)
                {
                    snd.audioer.Play();
                }
                if (bStopAllBG)
                {
                    foreach (var db in mgr.m_vBGSounds)
                        db.Value.Fade.Start(mgr.BGFade, 1, 0, true);
                }
                snd.fadeVolumeRatio = 0;
                snd.Fade.Start(mgr.BGFade, 0, 1, false);
                mgr.m_vBGSounds[snd.Guid] = snd;
                return snd.Guid;
            }

            return 0;
        }
        //------------------------------------------------------
        public static int PlayBG(string strFile, bool bStopAllBG = true, bool bLoop = true, string fastName=null)
        {
            ISound snd = AudioUtil.PlayBG(strFile, bStopAllBG, bLoop, fastName);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        public static int MixBG(string strFile, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            ISound snd = AudioUtil.MixBG(strFile, bLoop, fastName, mixGroup);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        public static int PlayBG(string strFile, AnimationCurve fadeCurve, bool bAllBGFadeStop = true, bool bLoop = true, string fastName = null)
        {
            ISound snd = AudioUtil.PlayBG(strFile, fadeCurve, bAllBGFadeStop, bLoop, fastName);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        public static int MixBG(string strFile, AnimationCurve fadeCurve, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            ISound snd = AudioUtil.MixBG(strFile, fadeCurve, bLoop, fastName, mixGroup);
            if (snd != null) return snd.GetID();
            return 0;
        }
        //------------------------------------------------------
        public static void SetBGParameter(int parameter)
        {
#if USE_FMOD
            if (ModuleManager.mainModule == null) return;
            AudioManager mgr = ms_pInstnace;
            if (mgr.m_vFModBGSounds == null) return;
            for(int i =0; i < mgr.m_vFModBGSounds.Count; ++i)
            {
                mgr.m_vFModBGSounds[i].eventInstance.setParameterByName("Parameter", parameter);
            }
#endif
        }
        //------------------------------------------------------
        public static void SetBGParameterLabel(string parameter)
        {
#if USE_FMOD
            if (ModuleManager.mainModule == null) return;
            AudioManager mgr = ms_pInstnace;
            if (mgr.m_vFModBGSounds == null) return;
            for (int i = 0; i < mgr.m_vFModBGSounds.Count; ++i)
            {
                mgr.m_vFModBGSounds[i].eventInstance.setParameterByNameWithLabel("Parameter", parameter);
            }
#endif
        }
#if USE_FMOD
        //------------------------------------------------------
        public static ISound PlayEvent(FMODUnity.EventReference eventRef, float fDelay =0,string mixChangel = null)
        {
            if (eventRef.IsNull)
                return null;
            if (ModuleManager.mainModule == null) return null;
            AudioManager mgr = ms_pInstnace;

            FMOD.Studio.EventInstance evtInst = FMODUnity.RuntimeManager.CreateInstance(eventRef);
            if(evtInst.isValid())
            {
                FmodSound fmdSnd = mgr.MallocFmodSnd();
                fmdSnd.Set(evtInst, ++ms_nGUID);
                fmdSnd.fmodGUID = eventRef.Guid;
                if (fDelay > 0) evtInst.setProperty(FMOD.Studio.EVENT_PROPERTY.SCHEDULE_DELAY, fDelay * 30000);//fps 30
                fmdSnd.Start();
                fmdSnd.SetGroup(-1);
                mgr.m_vFModEffectSounds.Add(fmdSnd);
                if(!string.IsNullOrEmpty(mixChangel))
                {
                    fmdSnd.SetVolume(0);
                    FMODUnity.RuntimeManager.FadeMixer(mixChangel, evtInst, 1);
                }
                return fmdSnd;
            }
            return null;
        }
        //------------------------------------------------------
        public static void StopEvent(FMODUnity.EventReference eventRef, string mixChangel = null)
        {
            if (eventRef.IsNull)
                return;
            if (ModuleManager.mainModule == null) return;
            AudioManager mgr = ms_pInstnace;
            if (mgr.m_vFModEffectSounds == null) return;
            for(int i =0; i < mgr.m_vFModEffectSounds.Count; ++i)
            {
                if(eventRef.Guid == mgr.m_vFModEffectSounds[i].fmodGUID)
                {
                    if(string.IsNullOrEmpty(mixChangel))
                        mgr.m_vFModEffectSounds[i].Stop();
                    else
                        FMODUnity.RuntimeManager.FadeMixer(mixChangel, mgr.m_vFModEffectSounds[i].eventInstance, 0);
                    break;
                }
            }
        }
#endif
        //------------------------------------------------------
        public static bool BGMToggle
        {
            get
            {
                if (ms_pInstnace == null) return true;
                return !ms_pInstnace.m_bCloseAllBGAudio;
            }
            set
            {
                if (ms_pInstnace == null) return;
                ms_pInstnace.m_bCloseAllBGAudio = value == false;
            }
        }
        //------------------------------------------------------
        public static bool EffectToggle
        {
            get
            {
                if (ms_pInstnace == null) return true;
                return !ms_pInstnace.m_bCloseEffectAudio;
            }
            set
            {
                if (ms_pInstnace == null) return;
                ms_pInstnace.m_bCloseEffectAudio = value == false;
            }
        }
        //------------------------------------------------------
        public static float BGMVolume
        {
            get
            {
                return m_BGVolume;
            }
            set
            {
                if(ms_pInstnace!=null)
                    ms_pInstnace._SetBGVolume(value);
            }
        }
        //------------------------------------------------------
        public static float EffectVolume
        {
            get
            {
                return m_SoundEffectVolume;
            }
            set
            {
                if (ms_pInstnace != null)
                    ms_pInstnace._SetEffectVolume(value);
            }
        }
    }
}