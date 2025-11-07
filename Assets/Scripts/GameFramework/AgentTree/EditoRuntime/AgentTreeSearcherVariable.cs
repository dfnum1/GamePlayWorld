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
    public partial class AgentTreeSearcherVariable
    {
        public struct ItemEvent
        {
            public AgentTreeEditor.BaseParam param;
            public System.Action<object> callback;
        }

        enum EState
        {
            Open,
            Close,
        }
        public Rect inspectorRect = new Rect(1, 22, 120, 50);
        Texture2D m_pBTest = null;

        EState m_nState = EState.Close;
        private Vector2 m_scrollPosition;
        string m_searchString = "";
        private AssetTree m_assetTree;
        private AssetTreeIMGUI m_assetTreeGUI;

        public System.Type filterVariable = null;
        public ArgvPort argvPort = null;
        public HashSet<Variable> vIngores = new HashSet<Variable>();
        public Action<ArgvPort,AgentTreeEditor.BaseParam> OnSelectedCall = null;

        AgentTreeEditorLogic m_pLogic;
        private Dictionary<string, ItemEvent> m_vEvents = new Dictionary<string, ItemEvent>();
        //------------------------------------------------------
        public void Open(AgentTreeEditorLogic logic, Rect rect)
        {
            m_pLogic = logic;
            inspectorRect = rect;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            Init();
            Search(m_searchString);
        }
        //------------------------------------------------------
        public void Close()
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
            vIngores.Clear();
            OnSelectedCall = null;
        }
        //--------------------------------------------------
        public bool IsOpen()
        {
            return m_nState == EState.Open;
        }
        //--------------------------------------------------
        public bool IsMouseIn(Vector2 mouse)
        {
            if (m_nState == EState.Open && inspectorRect.Contains(mouse)) return true;
            return false;
        }
        //------------------------------------------------------
        void Init()
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
        void OnSelected(AssetData data, bool bSelected)
        {
            if (data.guid == null || !bSelected) return;
            if (data.guid == null) return;

            ItemEvent evt;
            if (m_vEvents.TryGetValue(data.guid, out evt))
            {
                if (OnSelectedCall != null)
                {
                    OnSelectedCall(argvPort, evt.param);
                }
                else if(evt.callback != null) evt.callback(evt.param);
            }
            OnSelectedCall = null;
            Close();
        }
        //------------------------------------------------------
        void OnDoubleSelected(AssetData data)
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
        private void OnGUI()
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
            AgentTreeUtl.BeginArea(inspectorRect, m_pBTest);
            GUI.Box(new Rect(0, 0, inspectorRect.width, inspectorRect.height), m_pBTest);
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
        private void Search(string query)
        {
            m_vEvents.Clear();
            m_assetTree.Clear();

            HashSet<char> vQuery = new HashSet<char>();
            for (int i = 0; i < query.Length; ++i)
                vQuery.Add(query[i]);

            int id = 0;
            if(m_pLogic!=null)
            {
                foreach (var db in m_pLogic.ActionNodes)
                {
                    for(int i = 0; i < db.Value.Inputs.Count; ++i)
                    {
                        if(db.Value.Inputs[i].variable == null) continue;
                        if (vIngores.Contains(db.Value.Inputs[i].variable)) continue;
                        if (filterVariable != null && db.Value.Inputs[i].variable.GetType() != filterVariable) continue;
                        string name = db.Value.Inputs[i].GetDefaultName() + (db.Value.Inputs[i].variable.IsFlag(EFlag.Const) ? "(常量)" : "");
                        bool bQuerty = IsQuery(query, name);
                        if (!bQuerty) continue;

                        m_assetTree.AddUser(id.ToString(), name, vQuery.Count > 0);
                        AgentTreeEditor.VarialePop param = new AgentTreeEditor.VarialePop();
                        param.Data = db.Value.Inputs[i].variable;
                        param.Port = argvPort;
                        m_vEvents.Add(id.ToString(), new ItemEvent() { param = param, callback = m_pLogic.Editor.EquivalenceVariable });

                        id++;
                    }
                    for (int i = 0; i < db.Value.Outputs.Count; ++i)
                    {
                        if (db.Value.Outputs[i].variable == null) continue;
                        if (vIngores.Contains(db.Value.Outputs[i].variable)) continue;
                        if (filterVariable != null && db.Value.Outputs[i].variable.GetType() != filterVariable) continue;
                        string name = db.Value.Outputs[i].GetDefaultName() + (db.Value.Outputs[i].variable.IsFlag(EFlag.Const) ? "(常量)" : "");
                        bool bQuerty = IsQuery(query, name);
                        if (!bQuerty) continue;

                        if(db.Value.Outputs[i].variable.GetType() == typeof(VariableUser))
                        {
                            VariableUser user = db.Value.Outputs[i].variable as VariableUser;
                            for (int j = 0; j < AgentTreeUtl.EXPORT_TYPE_MONOS.Count; ++j)
                            {
                                if (AgentTreeUtl.EXPORT_TYPE_MONOS[j].hashCode == user.hashCode)
                                {
                                    name = name + "  C-(" + AgentTreeUtl.EXPORT_TYPE_MONOS[j].type.Name + ")  " + db.Value.GetDesc();
                                    break;
                                }
                            }
                        }

                        m_assetTree.AddUser(id.ToString(), name, vQuery.Count > 0);
                        AgentTreeEditor.VarialePop param = new AgentTreeEditor.VarialePop();
                        param.Data = db.Value.Outputs[i].variable;
                        param.Port = argvPort;
                        m_vEvents.Add(id.ToString(), new ItemEvent() { param = param, callback = m_pLogic.Editor.EquivalenceVariable });

                        id++;
                    }
                }

                foreach(var db in m_pLogic.StrcutDatas)
                {
                    for(int i = 0; i < db.runtimeVars.Count; ++i)
                    {
                        if (db.runtimeVars[i] == null) continue;
                        if (vIngores.Contains(db.runtimeVars[i])) continue;
                        if (filterVariable != null && db.runtimeVars[i].GetType() != filterVariable) continue;
                        string name = db.structName + "/"+ db.runtimeVars[i].strName + (db.runtimeVars[i].IsFlag(EFlag.Const) ? "(常量)" : "");
                        bool bQuerty = IsQuery(query, name);
                        if (!bQuerty) continue;

                        m_assetTree.AddUser(id.ToString(), name, vQuery.Count > 0);
                        AgentTreeEditor.VarialePop param = new AgentTreeEditor.VarialePop();
                        param.Data = db.runtimeVars[i];
                        param.Port = argvPort;
                        m_vEvents.Add(id.ToString(), new ItemEvent() { param = param, callback = m_pLogic.Editor.EquivalenceVariable });

                        id++;
                    }
                }
            }
        }
    }
}
#endif