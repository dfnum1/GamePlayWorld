using System;
using UnityEngine;
using UnityEngine.UI;
using Framework.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.UI
{
    public class UIVideo : RawImage
    {
        const float                 ALPHA_TIME = 0.25f;
        public string               url;
        public bool                 bLoop = true;
        public Color                erasureColor = Color.green;  
        public float                delayPlay = 0.0f;
        public Graphic              defaultShow = null;

#if USE_VIDOESYSTEM
        private string                                      m_strUrl = null;
        private float                                       m_fDelayPlay = 0.0f; 
		private float               m_fAlphaTime = 0.0f;
        private IMediaPlayer                                m_VideoPlayer;
        private System.Action<MediaPlayerEvent.EventType>   m_pCallback = null;
        //------------------------------------------------------
        protected override void Awake()
        {
            erasureColor.a = 0.0f;
            this.color = erasureColor;
            base.Awake();
        }
        //------------------------------------------------------
        protected override void Start()
        {
            StartVideo();
            base.Start();
        }
        //------------------------------------------------------
        protected override void OnDisable()
        {
            Stop();
            base.OnDisable();
        }
        //------------------------------------------------------
        protected override void OnEnable()
        {
            StartVideo();
            base.OnEnable();
        }
        //------------------------------------------------------
        public void StartVideo()
        {
            m_fAlphaTime = ALPHA_TIME;
            erasureColor.a = 0.0f;
            this.color = erasureColor;
            if (defaultShow) defaultShow.CrossFadeAlpha(1, ALPHA_TIME, true);
            if (!string.IsNullOrEmpty(url))
            {
#if UNITY_EDITOR
                if (Application.isPlaying) Play(url, bLoop, delayPlay);
#else
                Play(url, bLoop, delayPlay);
#endif
            }
        }
        //------------------------------------------------------
        public void Stop()
        {
            VideoSystem.Instance?.StopVideo(m_VideoPlayer);
            m_VideoPlayer = null;
            m_pCallback = null;
            m_fDelayPlay = 0.0f;
            m_strUrl = null;
            m_fAlphaTime = 0;
            if (defaultShow)
            {
                var color = defaultShow.color;
                color.a = 1.0f;
                defaultShow.color = color;
            }
            erasureColor.a = 0.0f;
            this.color = erasureColor;
        }
        //------------------------------------------------------
        public bool IsPlaying()
        {
            if (m_VideoPlayer != null) return m_VideoPlayer.IsPlaying();
            return false;
        }
        //------------------------------------------------------
        public IMediaPlayer GetVideoPlayer()
        {
            return m_VideoPlayer;
        }
        //------------------------------------------------------
        public bool Play(UnityEngine.Video.VideoClip videoClip, bool bLoop = false, System.Action<MediaPlayerEvent.EventType> onCallback = null)
        {
            if(videoClip == null)
            {
                onCallback?.Invoke(MediaPlayerEvent.EventType.Error);
                return false;
            }
            delayPlay = 0.0f;
            this.bLoop = bLoop;
            m_pCallback = onCallback;
            if(m_VideoPlayer!=null)
            {
                if (m_VideoPlayer.GetVideoPath().CompareTo(videoClip.name) == 0)
                {
                    return true;
                }
                VideoSystem.Instance?.StopVideo(m_VideoPlayer);
            }
            m_VideoPlayer = VideoSystem.Instance?.PlayVideo(videoClip);
            if(m_VideoPlayer!=null)
            {
                m_VideoPlayer.SetLooping(bLoop);
                m_VideoPlayer.AddListener(OnMediaListener);
            }
            else
            {
                onCallback?.Invoke(MediaPlayerEvent.EventType.Error);
                m_pCallback = null;
                return false;
            }
            return true;
        }
        //------------------------------------------------------
        public bool Play(string video, bool bLoop = false, float fDelayPlay = 0, System.Action<MediaPlayerEvent.EventType> pCallback = null)
        {
            if (string.IsNullOrEmpty(video))
            {
                if (pCallback != null) pCallback(MediaPlayerEvent.EventType.Error);
                return false;
            }
            if (video.CompareTo(this.m_strUrl) == 0)
            {
                return true;
            }
            this.bLoop = bLoop;
            this.m_strUrl = video;
            this.m_fDelayPlay = fDelayPlay;
            m_pCallback = pCallback;

            if(fDelayPlay<0.0f)
            {
                DelayPlay();
            }
            return true;
        }
        //------------------------------------------------------
        bool DelayPlay()
        {
			 m_fAlphaTime = ALPHA_TIME;
            m_fDelayPlay = -1.0f;
            if (string.IsNullOrEmpty(this.m_strUrl)) return false;
            if (m_VideoPlayer != null)
            {
                if (m_VideoPlayer.GetVideoPath().CompareTo(this.m_strUrl) == 0) return true;
                VideoSystem.Instance?.StopVideo(m_VideoPlayer);
            }
#if UNITY_EDITOR
            if (Application.isPlaying)
                erasureColor.a = 0.0f;
#else
            erasureColor.a = 0.0f;
#endif
            this.color = erasureColor;
            if(defaultShow)
            {
                var color = defaultShow.color;
                color.a = 1.0f;
                defaultShow.color = color;
            }
            m_VideoPlayer = VideoSystem.Instance?.PlayVideo(this.m_strUrl);
            if (m_VideoPlayer != null)
            {
                m_VideoPlayer.SetLooping(this.bLoop);
                m_VideoPlayer.AddListener(OnMediaListener);
                return true;
            }
            else
            {
                if (m_pCallback != null) m_pCallback(MediaPlayerEvent.EventType.Error);
                m_pCallback = null;
            }
            return false;
        }
        //------------------------------------------------------
        void OnMediaListener(IMediaPlayer player, MediaPlayerEvent.EventType type, ErrorCode code)
        {
            if (m_VideoPlayer != player) return;
            if (m_pCallback != null) m_pCallback(type);
            if (type == MediaPlayerEvent.EventType.Closing ||
                type == MediaPlayerEvent.EventType.Error ||
                type == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                m_VideoPlayer = null;
            }
        }
        //------------------------------------------------------
        void Update()
        {
            if (this.m_fDelayPlay >= 0.0f)
            {
                this.m_fDelayPlay -= Time.deltaTime;
                if (this.m_fDelayPlay <= 0)
                {
                    DelayPlay();
                }
            }
            if (m_VideoPlayer == null) return;
            SyncTexture(m_VideoPlayer);
        //    if (m_VideoPlayer.IsFinished())
         //       Hide();
        }
        //------------------------------------------------------
        public void SyncTexture(IMediaPlayer player, float alphaFactor = 1)
        {
            this.texture = player.GetTexture();
            if (this.texture == null)
            {
                erasureColor.a = 0;
                this.color = erasureColor;
            }
            else
            {
                if (m_fAlphaTime>0)
                {
                    m_fAlphaTime -= Time.unscaledDeltaTime;
                    float factor = Mathf.Clamp01((1 - m_fAlphaTime / 0.25f));
                    if (defaultShow)
                    {
                        var color = defaultShow.color;
                        if (color.a > 0)
                        {
                            color.a = Mathf.Lerp(color.a, 0, factor);
                            defaultShow.color = color;
                        }
                    }
                    erasureColor.a = Mathf.Lerp(erasureColor.a, player.GetAlhpa(true) * alphaFactor, factor);
                    this.color = erasureColor;
                }
            }
        }
#endif
    }
#if UNITY_EDITOR && USE_VIDOESYSTEM
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(UIVideo))]
    public class UIVideoEditor : UnityEditor.Editor
    {
        bool m_bDestroy = false;
        VideoSystem m_PreviewPlayer = null;
        public void OnEnable()
        {
            m_bDestroy = false;
            if (Application.isPlaying)
            {
                m_PreviewPlayer = VideoSystem.Instance;
            }
            if(m_PreviewPlayer == null)
            {
                m_bDestroy = true;
                m_PreviewPlayer = new VideoSystem();
                m_PreviewPlayer.Awake();
            }
        }
        public void OnDisable()
        {
            if (m_bDestroy)
            {
                if (m_PreviewPlayer != null) m_PreviewPlayer.Destroy();
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UIVideo video = target as UIVideo;
            video.defaultShow = (Graphic)UnityEditor.EditorGUILayout.ObjectField(new GUIContent("缺省显示", "当视频播放失败，或者正在加载时，显示"), video.defaultShow, typeof(Graphic), true);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            video.url = UnityEditor.EditorGUILayout.TextField(new GUIContent("视频路径","视频放到StreamingAssets目录下,配置时不要包含StreamingAssets/"), video.url);
            if (GUILayout.Button("...", new GUILayoutOption[] { GUILayout.Width(20) }))
            {
                string path = UnityEditor.EditorUtility.OpenFilePanel("选择视频", Application.streamingAssetsPath, "mp4,avi,mov,wmv,flv,mkv,webm");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.streamingAssetsPath))
                    {
                        path = path.Substring(Application.streamingAssetsPath.Length + 1);
                        video.url = path;
                    }
                    else
                    {
                        if (SceneView.lastActiveSceneView)
                            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("视频放到StreamingAssets目录下,配置时不要包含StreamingAssets/"), 3.0f);
                    }
                }
            }
            if(!Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(video.url) && GUILayout.Button(video.IsPlaying() ? "停止" : "预览", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    if (video.IsPlaying())
                    {
                        video.Stop();
                    }
                    else
                    {
                        video.Stop();
                        video.Play(video.url, video.bLoop);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            video.bLoop = UnityEditor.EditorGUILayout.Toggle("循环", video.bLoop);
            video.erasureColor = UnityEditor.EditorGUILayout.ColorField("抠色", video.erasureColor);
            video.erasureColor.a =0;
            if (video.material)
            {
                if (video.material.shader.name != "UI/UI_Video")
                {
                    UnityEditor.EditorGUILayout.HelpBox("使用UI/Video shader 可进行抠色哦！", MessageType.Warning, true);
                }
            }
            video.material = (Material)UnityEditor.EditorGUILayout.ObjectField("材质", video.material, typeof(Material), false);
            video.delayPlay = UnityEditor.EditorGUILayout.FloatField("延迟播放", video.delayPlay);

            if (EditorGUI.EndChangeCheck())
            {
                video.SetVerticesDirty();
                video.SetMaterialDirty();
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
        //-----------------------------------------------------
        [MenuItem("GameObject/UI/Video", false, 0)]
        static public void AddVideo(MenuCommand menuCommand)
        {
            GameObject panelRoot = new GameObject("Video");
            var videoComp = panelRoot.AddComponent<UIVideo>();
            videoComp.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Res/UI/Material/UIVideo.mat");

            GameObject parent = menuCommand.context as GameObject;
            if (parent == null)
            {
                var canvasRoot = GameObject.FindAnyObjectByType<Canvas>();
                if (canvasRoot == null)
                {
                    var canvas = new GameObject("Canvas");
                    canvasRoot = canvas.AddComponent<Canvas>();
                    canvas.AddComponent<CanvasScaler>();
                    canvas.AddComponent<GraphicRaycaster>();
                    canvasRoot.gameObject.layer = LayerMask.NameToLayer("UI");
                    StageUtility.PlaceGameObjectInCurrentStage(canvas);
                    parent = canvas;
                }
                else
                    parent = canvasRoot.gameObject;
            }
            if(parent) panelRoot.transform.SetParent(parent.transform);
            RectTransform rectTransform = panelRoot.GetComponent<RectTransform>();
            if(rectTransform) rectTransform.sizeDelta = new Vector2(300,300);

            Selection.activeGameObject = panelRoot;
        }
    }
#endif
}
