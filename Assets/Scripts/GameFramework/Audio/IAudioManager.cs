/********************************************************************
生成日期:	23:3:2020   18:07
类    名: 	AudioManager
作    者:	HappLI
描    述:	音效管理
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public enum ESoundType : byte
    {
        Effect,
        BG,
        StopEffect,
        StopBG,
    }
    public interface ISoundCallback
    {
        void OnSoundChannelStop(uint pAudio);
    }
    public interface ISound
    {
        int GetID();

        void SetGroup(int group);
        float Get3DRatio();
        void SetVolumnRatio(float ratio);
        float GetVolumnRatio();
        void SetVolume(float fVolume);
        bool UpdateVolume(bool bForce = false, bool bClose = false);

        void Start(float fTime, float fStart, float fEnd, bool bOverClear);
        void Start(AnimationCurve curve, float fStart, float fEnd, bool bOverClear);
        void Stop();

        void Destroy(bool isClear = true);
        bool IsPlaying();
        bool IsPause();
        bool IsFadeingIn();
        bool IsFadeingOut();
    }
    public interface IAudioManager : IUserData
    {
        void StartUp();
        void StopAllEffect(bool isSetPermanent = false);
        void StopAllBG(bool isSetPermanent = false);

        ISound _PlayEffectSnd(string strFile);
        ISound _PlayEffectVolume(string strFile, float fVolume);
        ISound _PlayEffectVolume(string strFile, float fVolume, Vector3 position);
        ISound _Play3DEffectVolume(string strFile, float fVolume, Transform pTrans);

        void _StopEffect(string strFile);
        void _StopEffect(int nID);
        void _PauseEffect(int nID);
        void _ResumeEffect(int nID);

        void _FadeOutEffect(int nID, float fFadeTime);
        void _FadeOutALLEffects(float fFadeTime);

        ISound _PlayID(uint nId, Vector3 position, bool bMix = false);
        ISound _PlayID(uint nId, Transform pTrans = null, bool bMix = false);

        void _SetBGVolume(int sound, float fVolume);
        float _GetBGVolume(int sound);
        void _StopBG(int nID);
        void _PauseBG(int nID);
        void _ResumeBG(int nID);
        void _StopBG(string strMusic);
        ISound _MixBG(string strFile, bool bLoop = true, string fastName = null, int mixGroup = 0);
        ISound _PlayBG(string strFile, AnimationCurve fadeCurve, bool bAllBGFadeStop = true, bool bLoop = true, string fastName = null);
        ISound _MixBG(string strFile, AnimationCurve fadeCurve, bool bLoop = true, string fastName = null, int mixGroup = 0);
        ISound _PlayBG(string strFile, bool bStopAllBG = true, bool bLoop = true, string fastName = null);
        void _FadeOutBG(int nID, float fFadeTime);
        void _FadeOutBG(string strMusic, float fFadeTime);
        void _FadeOutBGByName(string strName, float fFadeTime);
        void _FadeOutALLBGs(float fadeOut);

        void _SetEffectVolume(float fVolume);
        void _SetBGVolume(float volumn);
    }
    public class AudioUtil
    {
        //------------------------------------------------------
        public static IAudioManager GetInstance()
        {
            if (AFramework.mainFramework == null) return null;
            return AFramework.mainFramework.audioManager;
        }
        //------------------------------------------------------
        public static void StopEffect(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._StopEffect(nID);
        }
        //------------------------------------------------------
        public static void StopEffect(string strFile)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._StopEffect(strFile);
        }
        //------------------------------------------------------
        public static void FadeOutEffect(int nID, float fFadeTime)
        {
            if (fFadeTime <= 0)
            {
                StopEffect(nID);
                return;
            }
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutEffect(nID, fFadeTime);
        }
        //------------------------------------------------------
        public static void FadeOutALLEffects(float fFadeTime)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutALLEffects(fFadeTime);
        }
        //------------------------------------------------------
        public static void PauseEffect(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._PauseEffect(nID);
        }
        //------------------------------------------------------
        public static void ResumeEffect(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._ResumeEffect(nID);
        }
        //------------------------------------------------------
        public static ISound PlayEffectSnd(string strFile)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null) return null;

            return mgr._PlayEffectSnd(strFile);
        }
        //------------------------------------------------------
        public static ISound PlayEffect(string strFile)
        {
            return PlayEffectVolume(strFile, 1);
        }
        //------------------------------------------------------
        public static ISound PlayEffectVolume(string strFile, float fVolume)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null ) return null;
            return mgr._PlayEffectVolume(strFile, fVolume);
        }
        //------------------------------------------------------
        public static ISound PlayEffectVolume(string strFile, float fVolume, Vector3 position)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
            IAudioManager mgr = GetInstance();
            if (mgr == null ) return null;
            return mgr._PlayEffectVolume(strFile, fVolume, position);
        }
        //------------------------------------------------------
        public static ISound Play3DEffectVolume(string strFile, float fVolume, Transform pTrans)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null) return null;
            return mgr._Play3DEffectVolume(strFile, fVolume, pTrans);
        }
        //------------------------------------------------------
        public static void StopAll(bool bBG = true, bool bEffect = true)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            if (bBG) mgr.StopAllBG();
            if (bEffect) mgr.StopAllEffect();
        }
        //------------------------------------------------------
        public static ISound PlayID(uint nId, Vector3 position, bool bMix = false)
        {
            if (nId <= 0 ) return null;
            IAudioManager mgr = GetInstance();
            if (mgr == null) return  null;
            return mgr._PlayID(nId, position, bMix);
        }
        //------------------------------------------------------
        public static ISound PlayID(uint nId, Transform pTrans = null, bool bMix = false)
        {
            if (nId <= 0 ) return null;
            IAudioManager mgr = GetInstance();
            if (mgr == null) return null;
            return mgr._PlayID(nId, pTrans, bMix);
        }
        //------------------------------------------------------
        public static void FadeOutAll(float fadeOutBG = 0.1f, float fadeOutEffect = 0.1f)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            if (fadeOutBG <= 0) mgr.StopAllBG();
            else mgr._FadeOutALLBGs(fadeOutBG);

            if (fadeOutEffect <= 0) mgr.StopAllEffect();
            else mgr._FadeOutALLEffects(fadeOutEffect);
        }
        //------------------------------------------------------
        public static void StopBG(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._StopBG(nID);
        }
        //------------------------------------------------------
        public static void FadeOutBG(int nID, float fFadeTime)
        {
            if (fFadeTime <= 0)
            {
                StopEffect(nID);
                return;
            }
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutBG(nID, fFadeTime);
        }
        //------------------------------------------------------
        public static void FadeOutBGByName(string strName, float fFadeTime)
        {
            if (string.IsNullOrEmpty(strName)) return;
            if (fFadeTime <= 0)
            {
                StopBG(strName);
                return;
            }
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutBGByName(strName, fFadeTime);
        }
        //------------------------------------------------------
        public static void StopBG(string strMusic)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._StopBG(strMusic);
        }
        //------------------------------------------------------
        public static void FadeOutBG(string strMusic, float fFadeTime)
        {
            if (fFadeTime <= 0)
            {
                StopBG(strMusic);
                return;
            }
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutBG(strMusic, fFadeTime);
        }
        //------------------------------------------------------
        public static void FadeOutALLBGs(float fFadeTime)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._FadeOutALLBGs(fFadeTime);
        }
        //------------------------------------------------------
        public static void SetEffectVolume(float fVolume)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._SetEffectVolume(fVolume);
        }
        //------------------------------------------------------
        public static void SetBGVolume(float fVolume)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._SetBGVolume(fVolume);
        }
        //------------------------------------------------------
        public static void SetBGVolume(int sound, float fVolume)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._SetBGVolume(sound, fVolume);
        }
        //------------------------------------------------------
        public static float GetBGVolume(int sound)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return 0;
            return mgr._GetBGVolume(sound);
        }
        //------------------------------------------------------
        public static void PauseBG(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._PauseBG(nID);
        }
        //------------------------------------------------------
        public static void ResumeBG(int nID)
        {
            IAudioManager mgr = GetInstance();
            if (mgr == null) return;
            mgr._ResumeBG(nID);
        }
        //------------------------------------------------------
        public static ISound PlayBG(string strFile, bool bStopAllBG = true, bool bLoop = true, string fastName = null)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null) return null;
            return mgr._PlayBG(strFile, bStopAllBG, bLoop, fastName);
        }
        //------------------------------------------------------
        public static ISound MixBG(string strFile, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            if (string.IsNullOrEmpty(strFile)) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null) return null;
            return mgr._MixBG(strFile, bLoop, fastName, mixGroup);
        }
        //------------------------------------------------------
        public static ISound PlayBG(string strFile, AnimationCurve fadeCurve, bool bAllBGFadeStop = true, bool bLoop = true, string fastName = null)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null || !Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null) return null;
            return mgr._PlayBG(strFile, fadeCurve, bAllBGFadeStop, bLoop, fastName);
        }
        //------------------------------------------------------
        public static ISound MixBG(string strFile, AnimationCurve fadeCurve, bool bLoop = true, string fastName = null, int mixGroup = 0)
        {
            if (string.IsNullOrEmpty(strFile) ) return null;
            IAudioManager mgr = GetInstance();
#if UNITY_EDITOR
            if (mgr == null ||!Application.isPlaying)
            {
                ED.EditorUtil.PlayClip(strFile);
                return null;
            }
#endif
            if (mgr == null ) return null;
            return mgr._MixBG(strFile, fadeCurve, bLoop, fastName, mixGroup);
        }
    }
}
