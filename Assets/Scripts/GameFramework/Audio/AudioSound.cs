/********************************************************************
生成日期:	23:3:2020   18:07
类    名: 	AudioManager
作    者:	HappLI
描    述:	音效句柄
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public struct IDSoundData
    {
        public uint id;
        public byte group;
        public byte type;
        public string location;
        public string channel;
        public Vector2 volume;
        public bool b3D;
        public static IDSoundData NULL = new IDSoundData()
        {
            id = 0,
            group = 0,
            location = null,
            channel = null,
        };
        public bool IsValid() { return id > 0; }
    }
#if USE_FMOD
    public class FmodSound : ISound
    {
        public int id;
        public string eventPath;
        public float volumn;
        public float volumnFactor;
        public float fadeVolumeRatio;
        public AudioFadeCurve Fade;
        public FMOD.Studio.EventInstance eventInstance;
        public FMOD.GUID fmodGUID;

        public int mixerGroup=0;

        public bool bUseFollow ;
        public Transform pFollow ;
        public bool use3DPosition;
        public Vector3 triggerPosition;

        public bool IsValid()
        {
            return (id>0 || !string.IsNullOrEmpty(eventPath)) &&  eventInstance.isValid();
        }
        public void Set(FMOD.Studio.EventInstance eventInst, int id, string eventPath="")
        {
            this.id = id;
            this.eventPath = eventPath;
            Fade = new AudioFadeCurve();
            mixerGroup = 0;
            volumn = 1;
            volumnFactor = 1;
            fadeVolumeRatio = 1;
            bUseFollow = false;
            pFollow = null;
            use3DPosition = false;
            triggerPosition = Vector3.zero;
            eventInstance = eventInst;
            fmodGUID.Clear();
            eventInst.setCallback(new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback), FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
            instance.release();
            return FMOD.RESULT.OK;
        }
        public void Destroy(bool isClear = true)
        {
            if (eventInstance.isValid())
            {
               eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventInstance.clearHandle();
            }
            eventInstance = FMOD.Studio.EventInstance.DEFAULT;
            Fade.Clear();

            volumn = 0;
            volumnFactor = 1;
            fadeVolumeRatio = 1;
            pFollow = null;
            bUseFollow = false;
            Fade.Clear();
            use3DPosition = false;
            triggerPosition = Vector3.zero;

            mixerGroup = 0;
        }

        public float Get3DRatio()
        {
            return 1;
        }
        public float GetVolume()
        {
            return volumn * volumnFactor * Get3DRatio() * fadeVolumeRatio * GetGlobalMixerFade();
        }
        //-------------------------------------------------
        public void SetGroup(int group)
        {
            mixerGroup = group;
        }
        //-------------------------------------------------
        float GetGlobalMixerFade()
        {
            if (mixerGroup == -1) return 1;
            AudioManager mgr = AudioManager.getInstance();
            if (mgr == null) return 1;
            return mgr.GetGroupMixer(mixerGroup);
        }

        public int GetID()
        {
            return id;
        }

        public bool IsFadeingIn()
        {
            return Fade.IsFadeingIn();
        }

        public bool IsFadeingOut()
        {
            return Fade.IsFadeingOut();
        }

        public bool IsPlaying()
        {
            if (eventInstance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE state;
                eventInstance.getPlaybackState(out state);
                return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
            }
            return false;
        }

        public bool IsPause()
        {
            if (eventInstance.isValid())
            {
                bool bPause;
                if (eventInstance.getPaused(out bPause) == FMOD.RESULT.OK) return bPause;
            }
            return false;
        }

        public void SetVolume(float fVolume)
        {
            volumn = Mathf.Clamp01(fVolume);
        }

        public void SetVolumnRatio(float ratio)
        {
            fadeVolumeRatio = ratio;
        }
        public float GetVolumnRatio()
        {
            return fadeVolumeRatio;
        }

        public void Start(float fTime =0, float fStart = 1, float fEnd = 1, bool bOverClear = false)
        {
            if (eventInstance.isValid())
            {
                if(!IsPlaying())
                {
                    eventInstance.start();
                }
            }
            if(fTime>0)
                Fade.Start(fTime, fStart, fEnd, bOverClear);
        }

        public void Start(AnimationCurve curve, float fStart = 1, float fEnd = 1, bool bOverClear = false)
        {
            if (eventInstance.isValid())
            {
                if (!IsPlaying())
                {
                    eventInstance.start();
                }
            }
            if(curve!=null)
                Fade.Start(curve, fStart, fEnd, bOverClear);
        }

        public void Pause(bool bPause)
        {
            if (eventInstance.isValid())
            {
                eventInstance.setPaused(bPause);
            }
        }
        public void Stop()
        {
            if (eventInstance.isValid())
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventInstance.clearHandle();
            }
            eventInstance = FMOD.Studio.EventInstance.DEFAULT;
        }
        //-------------------------------------------------
        public bool UpdateVolume(bool bForce = false, bool bClose = false)
        {
            if (!eventInstance.isValid()) return false;
            if (eventInstance.isValid())
            {
                if(bClose)
                    eventInstance.setVolume(0);
                else
                    eventInstance.setVolume(GetVolume());
            }

            return true;
        }
    }
#endif
    public struct ConfigAudio
    {
        public byte group;
        public byte type;
        public string location;
        public Vector2 volume;
        public bool b3D;
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(location);
        }
    }
    public class AudioSound : ISound
    {
        public int Guid;
        public Asset pAsset;
        public string strKey;
        public AudioSource audioer;
        public float volumn;
        public float volumnFactor = 1;
        public float fadeVolumeRatio = 1;

        public bool prepare = false;
        public bool isPause = false;

        public AudioFadeCurve Fade = new AudioFadeCurve();

        public bool bMixedFadeOutBGNoOver = false;

        public bool bUseFollow = false;
        public Transform pFollow = null;
        public bool use3DPosition = false;
        public Vector3 triggerPosition = Vector3.zero;

        public int mixerGroup = 0;
        //-------------------------------------------------
        public int GetID() { return Guid; }
        //-------------------------------------------------
        public void SetGroup(int group)
        {
            mixerGroup = group;
        }
        //-------------------------------------------------
        public void Destroy(bool isClear = true)
        {
            if (isClear)
            {
                if (pAsset != null) pAsset.Release();
                pAsset = null;
                if (audioer)
                {
                    audioer.enabled = false;
                    audioer.Stop();
                    audioer.clip = null;
                }
                audioer = null;
            }
            volumn = 0;
            volumnFactor = 1;
            pFollow = null;
            bUseFollow = false;
            Fade.Clear();
            use3DPosition = false;
            prepare = false;
            isPause = false;
            triggerPosition = Vector3.zero;
            mixerGroup = 0;
            bMixedFadeOutBGNoOver = false;
        }
        //-------------------------------------------------
        public bool IsPlaying()
        {
            if (audioer != null) return audioer.isPlaying;
            return false;
        }
        //-------------------------------------------------
        public bool IsPause()
        {
            return isPause;
        }
        //-------------------------------------------------
        public bool IsFadeingIn()
        {
            return Fade.IsFadeingIn();
        }
        //-------------------------------------------------
        public bool IsFadeingOut()
        {
            return Fade.IsFadeingOut();
        }
        //-------------------------------------------------
        public void Start(AnimationCurve curve, float fStart, float fEnd, bool bOverClear)
        {
            Fade.Start(curve, fStart, fEnd, bOverClear);
        }
        //-------------------------------------------------
        public void Start(float fTime, float fStart, float fEnd, bool bOverClear)
        {
            Fade.Start(fTime, fStart, fEnd, bOverClear);
        }
        //-------------------------------------------------
        public float GetVolume()
        {
            return volumn * volumnFactor * Get3DRatio() * fadeVolumeRatio* GetGlobalMixerFade();
        }
        //-------------------------------------------------
        public void SetVolume(float fVolume)
        {
            volumn = Mathf.Clamp01(fVolume);
            if (audioer) audioer.volume = GetVolume();
        }
        //-------------------------------------------------
        public void SetVolumnRatio(float ratio)
        {
            fadeVolumeRatio = ratio;
            if (audioer)
            {
                audioer.volume = GetVolume();
            }
        }
        //-------------------------------------------------
        public float GetVolumnRatio()
        {
            return fadeVolumeRatio;
        }
        //-------------------------------------------------
        public float Get3DRatio()
        {
            if (pFollow == null && !use3DPosition) return 1;
            AudioManager mgr = AudioManager.getInstance();
            if (mgr == null) return 1;
            float dist = 0;
            if (pFollow != null)
            {
                dist = (pFollow.position - CameraUtil.mainCameraPosition).sqrMagnitude;
            }
            else
                dist = (triggerPosition - CameraUtil.mainCameraPosition).sqrMagnitude;
            float fixedDist = mgr.volumeDistanceFade * mgr.volumeDistanceFade;
            float fixedMinDist = mgr.volumeMinDistanceFade * mgr.volumeMinDistanceFade;
            if (dist <= fixedMinDist) return 1;
            return 1 - Mathf.Clamp01(dist / fixedDist);
        }
        //-------------------------------------------------
        float GetGlobalMixerFade()
        {
            if (mixerGroup < 0) return 1;
            AudioManager mgr = AudioManager.getInstance();
            if (mgr == null) return 1;
            return mgr.GetGroupMixer(mixerGroup);
        }
        //-------------------------------------------------
        public bool UpdateVolume(bool bForce = false, bool bClose = false)
        {
            if (audioer == null || !audioer.isPlaying) return false;
            if (bClose)
                audioer.volume = 0;
            else
                audioer.volume = GetVolume();
            if (bUseFollow)
            {
                if (pFollow == null) return false;
            }
            return true;
        }
        //-------------------------------------------------
        public void Stop()
        {
            if (audioer) audioer.Stop();
        }
    }
}
