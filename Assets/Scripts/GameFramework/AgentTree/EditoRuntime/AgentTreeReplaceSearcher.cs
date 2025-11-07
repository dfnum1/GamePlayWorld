#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System;

namespace Framework.Plugin.AT
{
    //------------------------------------------------------
    public partial class AgentTreeReplaceSearcher : AgentTreeSearcher
    {
        GraphNode m_pSelectGraphNode = null;

        protected Texture2D m_pTitleBG = null;
        //------------------------------------------------------
        public void Open(GraphNode graphNode, Rect rect)
        {
            if (graphNode == null || graphNode.BindNode == null)
                return;

            m_pSelectGraphNode = graphNode;
            inspectorRect = rect;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            Init();
            m_searchString = graphNode.BindNode.strName;
            Search(m_searchString);
        }
        //------------------------------------------------------
        public override void Close()
        {
            base.Close();
            m_pSelectGraphNode = null;
        }
        //------------------------------------------------------
        protected override void OnGUI()
        {
            if(m_pSelectGraphNode != null)
            {
                if (m_pTitleBG == null)
                {
                    m_pTitleBG = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    for (int x = 0; x < 2; ++x)
                    {
                        for (int z = 0; z < 2; ++z)
                        {
                            m_pTitleBG.SetPixel(x, z, new Color(1, 0, 1, 1));
                        }
                    }
                    m_pTitleBG.Apply();
                }

                AgentTreeUtl.BeginArea(new Rect(inspectorRect.x, inspectorRect.y, inspectorRect.width, 25), m_pTitleBG);
                GUI.Box(new Rect(0, 0, inspectorRect.width, 25), "替换");
                AgentTreeUtl.EndArea();

                DrawList(new Rect(inspectorRect.x, inspectorRect.y+25, inspectorRect.width, inspectorRect.height-25));
                return;
            }
            base.OnGUI();
        }
        //------------------------------------------------------
        protected override void OnSelected(AssetData data, bool bSelected)
        {
            if(m_pSelectGraphNode != null)
            {
                if (data.guid == null || !bSelected) return;
                if (data.guid == null) return;

                ItemEvent evt;
                if (m_vEvents.TryGetValue(data.guid, out evt))
                {
                    if(evt.param is AgentTreeEditor.EnterParam)
                    {
                        if(m_pSelectGraphNode.EnterTask!=null)
                        {
                            m_pSelectGraphNode.EnterTask.type = ((AgentTreeEditor.EnterParam)evt.param).type;
                        }
                    }
                    else if(evt.param is AgentTreeEditor.ActionParam)
                    {
                        AgentTreeEditor.ActionParam param = (AgentTreeEditor.ActionParam)evt.param;
                        if (m_pSelectGraphNode.BindNode != null && m_pSelectGraphNode.EnterTask == null)
                        {
                            m_pSelectGraphNode.BindNode.strName = param.Data.DisplayName;
                            m_pSelectGraphNode.BindNode.SetExcudeHash(param.Data.actionID);
                        }
                    }
                }
                Close();
                return;
            }
            base.OnSelected(data, bSelected);
        }
    }
}
#endif