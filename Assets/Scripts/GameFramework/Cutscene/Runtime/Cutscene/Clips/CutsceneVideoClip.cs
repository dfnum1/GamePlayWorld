#if USE_CUTSCENE
/********************************************************************
生成日期:	08:05:2025
类    名: 	CutsceneVideoClip
作    者:	HappLI
描    述:	视频
*********************************************************************/
using Framework.Cutscene.Runtime;
using UnityEngine;
using Framework.Base;
using Framework.Core;


#if UNITY_EDITOR
using UnityEditor;
using Framework.Cutscene.Editor;
#endif
namespace GameApp.Cutscene
{
    [System.Serializable, CutsceneClip("音频/视频Clip")]
    public class CutsceneVideoClip : IBaseClip
    {
        [Display("基本属性")] public BaseClipProp baseProp;
        [Display("视频路径"), RowFieldInspector] public string videoName;
        [UnEdit, Display("StreamAsset目录")] public bool bStreamAsset = true;
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eVideoClip;
        }
        //-----------------------------------------------------
        public float GetTime()
        {
            return baseProp.time;
        }
        //-----------------------------------------------------
        public string GetName()
        {
            return baseProp.name;
        }
        //-----------------------------------------------------
        public string GetVideName()
        {
            return videoName;
        }
        public bool IsStreamAsset()
        {
            return bStreamAsset;
        }
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new CutsceneVideoClipDriver();
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
        public ushort GetRepeatCount()
        {
            return baseProp.repeatCnt;
        }
        //-----------------------------------------------------
        public float GetBlend(bool bIn)
        {
            return baseProp.GetBlend(bIn);
        }
#if UNITY_EDITOR
        [System.NonSerialized] float m_fOriVideoDuration = -1;
        //-----------------------------------------------------
        public CutsceneVideoClip OnDrawFieldLineRow(System.Object pOwner, System.Reflection.FieldInfo fieldInfo)
        {
            if (fieldInfo.Name == "videoName")
            {
                if (GUILayout.Button("选择视频"))
                {
                    var binder = baseProp.ownerTrackObject.GetBindLastCutsceneObject();
                    var provider = ScriptableObject.CreateInstance<VideoSearchProvider>();
                    provider.Init((vidoeName, streamRes) =>
                    {
                        this.videoName = vidoeName;
                        this.bStreamAsset = streamRes;
                        if(!string.IsNullOrEmpty(this.videoName))
                        {
                            this.baseProp.duration = GetVideoDuration();
                            m_fOriVideoDuration = this.baseProp.duration;
                        }
                    });
                    // 弹出搜索窗口，位置可根据需要调整
                    UnityEditor.Experimental.GraphView.SearchWindow.Open(new UnityEditor.Experimental.GraphView.SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
                }
            }
            return this;
        }
        //-----------------------------------------------------
        public float GetVideoDuration()
        {
            if (!string.IsNullOrEmpty(videoName))
            {
                var videoInfo = VideoSearchProvider.GetVideoByCodeName(this.videoName);
                return Framework.ED.EditorUtil.GetVideoDuration(videoInfo.videoFile);
            }
            return 0;
        }
        //-----------------------------------------------------
        [AddInspector]
        public void OnDrawInspector()
        {
            if (!string.IsNullOrEmpty(videoName))
            {
                if (GUILayout.Button("同步视频时长到轨道剪辑"))
                {
                    this.baseProp.duration = GetVideoDuration();
                    m_fOriVideoDuration = this.baseProp.duration;
                }
                if(m_fOriVideoDuration<0)
                    m_fOriVideoDuration = GetVideoDuration();
                GUILayout.Label("视频时长:" + m_fOriVideoDuration + "秒");
            }
        }
#endif
    }
    //-----------------------------------------------------
    //! CutsceneVideoClipDriver
    //-----------------------------------------------------
    public class CutsceneVideoClipDriver : ACutsceneDriver
    {   
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            CutsceneVideoClip clipData = clip.clip.Cast<CutsceneVideoClip>();
            var videCode = clipData.GetVideName();
            var bStreamAsset = clipData.IsStreamAsset();

            if(VideoSystem.Instance == null)
            {
                Debug.LogError("VideoSystem未初始化，无法播放视频:" + videCode);
                return false;
            }
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            return true;
        }
    }
}
#endif