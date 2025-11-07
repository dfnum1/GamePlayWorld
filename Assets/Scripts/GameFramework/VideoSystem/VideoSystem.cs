#if USE_VIDEOSYSTEM
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	VideoController
作    者:	HappLI
描    述:	视频播放控制器模块
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Framework.Core
{
    public class VideoSystem : AModule
    {
        GameObject m_pVideoRoot = null;

        List<IMediaPlayer> m_vVideos = null;
        //------------------------------------------------------
        static VideoSystem ms_pInstance= null;
        public static VideoSystem Instance
        {
            get 
            {
                return ms_pInstance;    
            }
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            ms_pInstance = this;
            if(GetFramework().IsEditorMode)
            {

            }
            else
            {
                m_pVideoRoot = new GameObject("VideoSystem");
                var gameRoot = GetFramework().gameStartup?.GetTransform();
                if (gameRoot)
                    m_pVideoRoot.transform.SetParent(gameRoot);
                else
                {
#if UNITY_EDITOR
                    if (Application.isPlaying) GameObject.DontDestroyOnLoad(m_pVideoRoot);
#else

                    GameObject.DontDestroyOnLoad(m_pVideoRoot);
#endif
                }
                m_pVideoRoot.hideFlags |= HideFlags.HideAndDontSave;
            }
            m_vVideos = new List<IMediaPlayer>(2);
        }
        //------------------------------------------------------
        public void PreLoadVideo(string strPath)
        {
            PlayVideo(strPath, false);
        }
        //------------------------------------------------------
        public IMediaPlayer PlayVideo(string strPath, bool bAutoPlay = true)
        {
            if (m_vVideos == null) m_vVideos = new List<IMediaPlayer>(2);
            for (int i = 0; i < m_vVideos.Count; ++i)
            {
                if(strPath.CompareTo(m_vVideos[i].GetVideoPath()) == 0)
                {
                    if (bAutoPlay && !m_vVideos[i].IsPlaying()) m_vVideos[i].Play();
                    return m_vVideos[i];
                }
            }
            IMediaPlayer media = NewMediaPlayer();
            if (media == null) return null;
            media.AddListener(OnMediaListener);
            if (media.OpenVideoFromFile(strPath))
            {
                m_vVideos.Add(media);
                return media;
            }
            return null;
        }
        //------------------------------------------------------
        public IMediaPlayer PlayVideo(UnityEngine.Video.VideoClip clip, bool bAutoPlay = true)
        {
            if (clip == null) return null;
            if (m_vVideos == null) m_vVideos = new List<IMediaPlayer>(2);
            string clipName = clip.name;
            for (int i = 0; i < m_vVideos.Count; ++i)
            {
                if (clipName.CompareTo(m_vVideos[i].GetVideoPath()) == 0)
                {
                    if (bAutoPlay && !m_vVideos[i].IsPlaying()) m_vVideos[i].Play();
                    return m_vVideos[i];
                }
            }

            IMediaPlayer media = NewMediaPlayer();
            if (media == null) return null;
            media.AddListener(OnMediaListener);
            if (media.OpenVideo(clip))
            {
                m_vVideos.Add(media);
                return media;
            }
            return null;
        }
        //------------------------------------------------------
        public void StopVideo(string strPath)
        {
            if (m_vVideos == null) return;
            for (int i = 0; i < m_vVideos.Count; ++i)
            {
                if (strPath.CompareTo(m_vVideos[i].GetVideoPath()) == 0)
                {
                    FreeMediaPlayer(m_vVideos[i]);
                    m_vVideos.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        public void StopVideo(IMediaPlayer mediaPlayer)
        {
            if (mediaPlayer == null) return;
            FreeMediaPlayer(mediaPlayer);
            m_vVideos.Remove(mediaPlayer);
        }
        //------------------------------------------------------
        public void ClearVideo()
        {
            if (m_vVideos == null) return;
            for (int i = 0; i < m_vVideos.Count; ++i)
            {
                FreeMediaPlayer(m_vVideos[i]);
            }
            m_vVideos.Clear();
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            ms_pInstance = null;
            if (m_pVideoRoot)
            {
#if UNITY_EDITOR
                if(Application.isPlaying) GameObject.Destroy(m_pVideoRoot);
                else GameObject.DestroyImmediate(m_pVideoRoot);
#else
                GameObject.Destroy(m_pVideoRoot);
#endif
            }
            m_pVideoRoot = null;
        }
        //------------------------------------------------------
        void OnMediaListener(IMediaPlayer player, MediaPlayerEvent.EventType type, ErrorCode code)
        {
            Debug.Log(player.GetVideoPath() + ":" + type.ToString());
            if(type == MediaPlayerEvent.EventType.Closing ||
                type == MediaPlayerEvent.EventType.Error || 
                type == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                FreeMediaPlayer(player);
                if (m_vVideos != null) m_vVideos.Remove(player);
            }
        }
        //------------------------------------------------------
        void FreeMediaPlayer(IMediaPlayer player)
        {
            MonoBehaviour behaviour = player as MonoBehaviour;
            if(behaviour)
            {
                GameObject.Destroy(behaviour.gameObject);
            }
        }
        //------------------------------------------------------
        IMediaPlayer NewMediaPlayer()
        {
            if (m_pVideoRoot == null) return null;
            GameObject video = new GameObject("video");
            video.transform.SetParent(m_pVideoRoot.transform);
#if USE_AVPRO1
            return video.AddComponent<AVProVideoPlayer>();
#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && !UNITY_EDITOR_OSX
            //return video.AddComponent<UnityMediaPlayer>();
            return video.AddComponent<WindowsMediaPlayer>();
#elif !UNITY_EDITOR && UNITY_ANDROID
             //return video.AddComponent<UnityMediaPlayer>();
             return video.AddComponent<AndroidMediaPlayer>();
#else
            return video.AddComponent<UnityMediaPlayer>();
#endif
        }
    }
}
#endif