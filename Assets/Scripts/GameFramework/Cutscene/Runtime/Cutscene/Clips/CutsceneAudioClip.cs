#if USE_CUTSCENE
/********************************************************************
生成日期:	08:05:2025
类    名: 	CutsceneAudioClip
作    者:	HappLI
描    述:	音效
*********************************************************************/
using Framework.Cutscene.Runtime;
using UnityEngine;
using Framework.Base;


#if UNITY_EDITOR
using Framework.Cutscene.Editor;
using UnityEditor;
#endif

namespace GameApp.Cutscene
{
    [System.Serializable, CutsceneClip("音频/AudioClip")]
    public class CutsceneAudioClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp baseProp;
        [Display("重复播放次数")] public int playTimes = 1;//播放次数
        [Display("重复播放间隔")] public float playGap = 0.0f;
#if USE_WWISE
        public AkCurveInterpolation blendInCurve;
        public AkCurveInterpolation blendOutCurve;
        //   [UnEdit, Display("WwiseEventId")] public uint akEventId = AkSoundEngine.AK_INVALID_UNIQUE_ID;
        //   [Display("声音"), StringViewPlugin("OnCutsceneWwiseEventPop"), DataBindSet("akEventId")] public string akEventGuid;
        [Display("声音"), StringViewPlugin("OnCutsceneWwiseEventNamePop")] public string akEventName;
#endif
        public ACutsceneDriver CreateDriver()
        {
#if USE_WWISE
            return new CutsceneWwiseEventDriver();
#else
            return null;
#endif
        }
        //-----------------------------------------------------
        public float GetBlend(bool bIn)
        {
            return baseProp.GetBlend(bIn);
        }
        //-----------------------------------------------------
        public float GetDuration()
        {
            return baseProp.duration;
        }
        //-----------------------------------------------------
        public EClipEdgeType GetEndEdgeType()
        {
            return baseProp.endEdgeType;
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eAudioClip;
        }
        //-----------------------------------------------------
        public string GetName()
        {
            return baseProp.name;
        }
        //-----------------------------------------------------
        public ushort GetRepeatCount()
        {
            return baseProp.repeatCnt;
        }
        //-----------------------------------------------------
        public float GetTime()
        {
            return baseProp.time;
        }
    }
#if USE_WWISE
    //-----------------------------------------------------
    //! CutsceneWwiseEventDriver
    //-----------------------------------------------------
    public class CutsceneWwiseEventDriver : ACutsceneDriver
    {
        private const uint CallbackFlags = (uint)(AkCallbackType.AK_EndOfEvent | AkCallbackType.AK_Duration);
        System.Collections.Generic.List<uint> m_vPlayingId = null;
        GameObject m_pTargetSound = null;
        float m_fLastPlayTime = 0;
        bool m_bPause = false;
        //-----------------------------------------------------
        public override void OnDestroy()
        {
            base.OnDestroy();
            StopPlayingId(0.5f, AkCurveInterpolation.AkCurveInterpolation_Linear);
            m_pTargetSound = null;
            if (m_vPlayingId != null)
            {
                UnityEngine.Pool.ListPool<uint>.Release(m_vPlayingId);
                m_vPlayingId = null;
            }
            m_bPause = false;
        }
        //-----------------------------------------------------
        public override bool OnCreateClip(CutsceneTrack pTrack, IBaseClip clip)
        {
#if UNITY_EDITOR && UNITY_EDITOR_WIN
            if(AkSoundEngine.IsInitialized())
            {
                string testPath = System.IO.Path.Combine(Application.dataPath, "../Data/Audio/GeneratedSoundBanks/Windows");
                if (System.IO.Directory.Exists(testPath))
                {
                    AkSoundEngine.AddBasePath(testPath);
                }
            }
#endif
            return true;
        }
        //-----------------------------------------------------
        public GameObject GetTargetSound(CutsceneTrack pTrack, FrameData clip)
        {
            if (m_pTargetSound != null) return m_pTargetSound;
            var pBind = pTrack.GetBindLastCutsceneObject();
            m_pTargetSound = pBind.GetUniyObject() as GameObject;
            if (m_pTargetSound == null)
                m_pTargetSound = AkSoundEngine.GetEmitterGameObject((int)WwiseAudioDefine.EAudioType.Sound);
            return m_pTargetSound;
        }
        //-----------------------------------------------------
        void StopPlayingId(float blendOutTime, AkCurveInterpolation curve )
        {
            if (m_vPlayingId == null)
                return;
            foreach(var db in m_vPlayingId)
            {
                if (db == AkSoundEngine.AK_INVALID_PLAYING_ID) continue;
                AkSoundEngine.StopPlayingID(db, (int)(blendOutTime * 1000), curve);
            }
            m_vPlayingId.Clear();
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            CutsceneWwiseEventClip clipData = clip.clip.Cast<CutsceneWwiseEventClip>();
            if (string.IsNullOrEmpty(clipData.akEventName))
                return true;
            m_bPause = false;
            var pBind = pTrack.GetBindLastCutsceneObject();
            m_pTargetSound = GetTargetSound(pTrack, clip);
            StopPlayingId(0.5f, clipData.blendOutCurve);

            if(m_vPlayingId == null)
                m_vPlayingId = UnityEngine.Pool.ListPool<uint>.Get();

            var playingId = AkSoundEngine.PostEventEx(clipData.akEventName, m_pTargetSound);
            if(playingId != AkSoundEngine.AK_INVALID_PLAYING_ID)
                m_vPlayingId.Add(playingId);
            m_fLastPlayTime = clipData.playGap;
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            CutsceneWwiseEventClip clipData = clip.clip.Cast<CutsceneWwiseEventClip>();
            if (string.IsNullOrEmpty(clipData.akEventName))
                return true;

            int blendOutTime = Mathf.Max(0, (int)(clipData.GetBlend(false) * 1000));
            StopPlayingId(clipData.GetBlend(false), clipData.blendOutCurve);
            m_pTargetSound = null;
            m_bPause = false;
            return true;
        }
        //-----------------------------------------------------
        public override bool OnFrameClip(CutsceneTrack pTrack, FrameData frameData)
        {
            m_pTargetSound = GetTargetSound(pTrack, frameData);
            CutsceneWwiseEventClip clipData = frameData.clip.Cast<CutsceneWwiseEventClip>();
            if (frameData.eStatus == EPlayableStatus.Pause)
            {
                m_bPause = true;
                if (m_vPlayingId != null && m_vPlayingId.Count > 0)
                {
                    int blendOutTime = Mathf.Max(0, (int)(clipData.GetBlend(false) * 1000));
                    AkSoundEngine.PauseEvent(clipData.akEventName, m_pTargetSound, blendOutTime, clipData.blendOutCurve);
                }
                return true;
            }
            else if (frameData.eStatus == EPlayableStatus.Start)
            {
                if (m_bPause)
                {
                    int blendInTime = Mathf.Max(0, (int)(clipData.GetBlend(true) * 1000));
                    AkSoundEngine.ResumeEvent(clipData.akEventName, m_pTargetSound, blendInTime, clipData.blendInCurve);
                }
                else
                {
                    if (m_fLastPlayTime > 0)
                    {
                        m_fLastPlayTime -= frameData.deltaTime;
                        if (m_fLastPlayTime <= 0.0f)
                        {
                            m_fLastPlayTime = clipData.playGap;
                            var nPlayingId = AkSoundEngine.PostEventEx(clipData.akEventName, m_pTargetSound);
                            if(nPlayingId != AkSoundEngine.AK_INVALID_PLAYING_ID)
                            {
                                if (m_vPlayingId == null)
                                    m_vPlayingId = UnityEngine.Pool.ListPool<uint>.Get();
                                m_vPlayingId.Add(nPlayingId);
                            }
                        }
                        return true;
                    }
                }
                return true;
            }
            float normalTime = frameData.curTime / clipData.GetDuration();
            return true;
        }
    }
#endif
}
#endif