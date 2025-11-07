#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorCommonAction\ActorTimelineAction
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Cutscene.Runtime;
using Framework.Plugin.AT;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public abstract class ActorAction : IUserData
    {
        public string actioName;
        public EActionStateType type;
        public ushort actionTag;
        public uint priority = 0;
        public uint layer = 0;

        public uint GetActionKey()
        {
            return(uint)((int)type << 16 | actionTag);
        }
#if USE_CUTSCENE
        public virtual Framework.Cutscene.Runtime.CutsceneGraph GetPlayCutscene()
        {
            return null;
        }
#endif
        public void Destroy()
        {
        }
    }

    [System.Serializable]
    public class ActorCommonAction : ActorAction
    {

        public AnimationClip clip;

        public bool Equal(ActorCommonAction obj)
        {
            if (obj.type == type && actionTag == obj.actionTag && obj.clip == clip)
                return true;
            return false;
        }
    }
    [System.Serializable]
    public class ActorTimelineAction : ActorAction
    {
#if USE_CUTSCENE
        [SerializeField] string cutsceneGraphData;
        [System.NonSerialized]private Framework.Cutscene.Runtime.CutsceneGraph m_cutsceneGraph;
        public Framework.Cutscene.Runtime.CutsceneGraph GetCutsceneGraph(bool bAutoNew=false)
        {
            if (m_cutsceneGraph == null && !string.IsNullOrEmpty(cutsceneGraphData))
            {
                m_cutsceneGraph = new Cutscene.Runtime.CutsceneGraph();
                m_cutsceneGraph.OnDeserialize(cutsceneGraphData);
            }
            if (bAutoNew && m_cutsceneGraph == null)
                m_cutsceneGraph = new Cutscene.Runtime.CutsceneGraph();
            return m_cutsceneGraph;
        }
        public override Framework.Cutscene.Runtime.CutsceneGraph GetPlayCutscene()
        {
            return GetCutsceneGraph();
        }
#else
        public string timelineName;
#endif
#if UNITY_EDITOR
        public void Save()
        {
#if USE_CUTSCENE
            if (m_cutsceneGraph != null)
                cutsceneGraphData = m_cutsceneGraph.OnSerialize();
            else cutsceneGraphData = "";
#endif
        }
#endif
    }
    [System.Serializable]
    public class ActorAvatarMask : IUserData
    {
        public int layer;
        public UnityEngine.AvatarMask avatarMask;
        public void Destroy()
        {
        }
    }
    [System.Serializable]
    public class ActorGraphData : IUserData
    {
        public BoundsInt boundBox;
        [DisableGUI] public List<ActorTimelineAction> timelineActions = new List<ActorTimelineAction>(4);

        private bool m_bInited = false;
        //-----------------------------------------------------
        public void Destroy()
        {
            m_bInited = false;
        }
        //-----------------------------------------------------
        public void Init(bool bForce = false)
        {
            if (!bForce && m_bInited) return;
            m_bInited = true;
            if (timelineActions != null)
            {
                for (int i = 0; i < timelineActions.Count; ++i)
                {
                    timelineActions[i].GetCutsceneGraph(false);
                }
            }
        }
        //-----------------------------------------------------
        public void Load(string content)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(content, this);
                Init();
            }
            catch(System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
#if UNITY_EDITOR
        [System.NonSerialized] string m_strFilePath = null;
        public void SetPathFile(string file)
        {
            m_strFilePath = file;
        }
        //-----------------------------------------------------
        public string GetPathFile()
        {
            return m_strFilePath;
        }
        //-----------------------------------------------------
        public TextAsset Save()
        {
            if (timelineActions != null)
            {
                for (int i = 0; i < timelineActions.Count; ++i)
                {
                    timelineActions[i].Save();
                }
            }
            string content = JsonUtility.ToJson(this, true);
            if (string.IsNullOrEmpty(m_strFilePath))
            {
                return null;
            }
            System.IO.File.WriteAllText(m_strFilePath, content, System.Text.Encoding.UTF8);
            AssetDatabase.ImportAsset(m_strFilePath);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(m_strFilePath);
        }
#endif
    }
}
#endif