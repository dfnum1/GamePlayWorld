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
    public partial class AgentTreeSearcherAPI
    {
        public struct ItemEvent
        {
            public System.Object bindData;
            public APINode apiNode;
            public Vector2 gridPos;
            public Vector2 mousePos;
            public int atGUID;
            public System.Action<System.Object> callback;
        }

        protected enum EState
        {
            Open,
            Close,
        }
        public Rect inspectorRect = new Rect(1, 22, 120, 50);
        protected Texture2D m_pBTest = null;

        protected EState m_nState = EState.Close;
        protected Vector2 m_scrollPosition;
        protected string m_searchString = "";
        protected AssetTree m_assetTree;
        protected AssetTreeIMGUI m_assetTreeGUI;

        AgentTreeEditorLogic m_pLogic;

        System.Object m_pBindData = null;
        protected Dictionary<string, ItemEvent> m_vEvents = new Dictionary<string, ItemEvent>();
        //------------------------------------------------------
        public void Open(AgentTreeEditorLogic pLogic, Rect rect, System.Object bindData = null)
        {
            m_pLogic = pLogic;
            m_pBindData = bindData;
            inspectorRect = rect;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            Init();
            Search(m_searchString);
        }
        //------------------------------------------------------
        public virtual void Close()
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
        }
        //--------------------------------------------------
        public bool IsMouseIn(Vector2 mouse)
        {
            if (m_nState == EState.Open && inspectorRect.Contains(mouse)) return true;
            return false;
        }
        //------------------------------------------------------
        protected  void Init()
        {
            if(m_assetTree == null)
            {
                m_assetTree = new AssetTree();
                m_assetTreeGUI = new AssetTreeIMGUI(m_assetTree.Root);
                m_assetTreeGUI.onSelected = OnSelected;
                m_assetTreeGUI.onDoubleClick = OnDoubleSelected;
                m_assetTreeGUI.onDraw = OnDrawItem;
            }
        }
        //------------------------------------------------------
        protected virtual void OnSelected(AssetData data, bool bSelected)
        {
            if (data.guid == null || !bSelected) return;
            if (data.guid == null) return;

            ItemEvent evt;
            if (m_vEvents.TryGetValue(data.guid, out evt) && evt.callback!=null)
            {
                evt.callback(evt);
            }
            Close();
        }
        //------------------------------------------------------
        protected virtual void OnDoubleSelected(AssetData data)
        {
            if (data.guid == null) return;

//             ItemEvent evt;
//             if (m_vEvents.TryGetValue(data.guid, out evt) && evt.callback != null)
//             {
//                 evt.callback(evt.param);
//             }
//             Close();
        }
        //------------------------------------------------------
        void OnDrawItem(AssetData data)
        {
            if (data.guid == null)
            {
                if (AgentTreeUtl.EXPORT_CLASS_SHORTNAME_SET.Contains(data.path))
                {
                    if (data.icon == null)
                        data.icon = Resources.Load<Texture2D>("at_tr_class");
                }
                else
                {
                    if (data.icon == null)
                        data.icon = Resources.Load<Texture2D>("at_tr_noder");
                }
                return;
            }
            if(data.icon == null)
                data.icon = Resources.Load<Texture2D>("at_tr_function");
        }
        //------------------------------------------------------
        public void OnDraw()
        {
            if (m_nState != EState.Open) return;
            Init();

            OnGUI();
        }
        //------------------------------------------------------
        public void Update(float fTime)
        {
            
        }
        //------------------------------------------------------
        protected virtual void OnGUI()
        {
            DrawList(inspectorRect);
        }
        //------------------------------------------------------
        protected void DrawList(Rect rect)
        {
            if (m_pBTest == null)
            {
                m_pBTest = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                for (int x = 0; x < 2; ++x)
                {
                    for (int z = 0; z < 2; ++z)
                    {
                        m_pBTest.SetPixel(x, z, new Color(1, 1, 1, 0.8f));
                    }
                }
                m_pBTest.Apply();
            }
            // draw search
            AgentTreeUtl.BeginArea(rect, m_pBTest);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), m_pBTest);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField("Search:", EditorStyles.miniLabel, GUILayout.Width(40));

            EditorGUI.BeginChangeCheck();

            //搜索栏聚焦
            GUI.SetNextControlName("search");
            m_searchString = EditorGUILayout.TextField(m_searchString, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
            GUI.FocusControl("search");

            if (EditorGUI.EndChangeCheck())
            {
                Search(m_searchString);
            }

            EditorGUILayout.EndHorizontal();

            // draw tree
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            m_assetTreeGUI.DrawTreeLayout();

            EditorGUILayout.EndScrollView();

            AgentTreeUtl.EndArea();
        }
        //------------------------------------------------------
        bool IsQuery(string query, string strContext)
        {
            if (string.IsNullOrEmpty(query)) return true;
            if (string.IsNullOrEmpty(strContext)) return false;
            if(strContext.Length > query.Length)
            {
                return strContext.ToLower().Contains(query.ToLower());
            }
            return query.ToLower().Contains(strContext.ToLower());
        }
        //------------------------------------------------------
        protected void Search(string query)
        {
            m_vEvents.Clear();
            m_assetTree.Clear();

            HashSet<char> vQuery = new HashSet<char>();
            for (int i = 0; i < query.Length; ++i)
                vQuery.Add(query[i]);

            int id = 0;
            foreach (var item in AgentTreeEditor.GlobalAPILists)
            {
                bool bQuerty = IsQuery(query, item.strName);
                if (!bQuerty ) continue;
                m_assetTree.AddUser(id.ToString(), "全局/" + item.strName, vQuery.Count > 0);
                ItemEvent evt =  new ItemEvent() { bindData = m_pBindData, apiNode = item, atGUID = 0, callback = m_pLogic.Editor.OnAPINodeSelect };
                evt.mousePos = inspectorRect.position;
                evt.gridPos = m_pLogic.WindowToGridPosition(inspectorRect.position);
                m_vEvents.Add(id.ToString(), evt);
                id++;
            }

            foreach(var db in m_pLogic.ActionNodes)
            {
                if(db.Value.BindNode is APINode)
                {
                    APINode item = db.Value.BindNode as APINode;
                    bool bQuerty = IsQuery(query, item.strName);
                    if (!bQuerty) continue;
                    m_assetTree.AddUser(id.ToString(), "局部/" + item.strName, vQuery.Count > 0);
                    ItemEvent evt = new ItemEvent() { bindData = m_pBindData, apiNode = item, atGUID = 1, callback = m_pLogic.Editor.OnAPINodeSelect };
                    evt.mousePos = inspectorRect.position;
                    evt.gridPos = m_pLogic.WindowToGridPosition(inspectorRect.position);
                    m_vEvents.Add(id.ToString(), evt);
                    id++;
                }
            }

        }
    }
}
#endif