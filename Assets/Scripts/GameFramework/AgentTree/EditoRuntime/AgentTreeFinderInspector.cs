#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeFinderInspector
    {
        enum EState
        {
            Stand,
            Open,
            Close,
        }
        Texture2D m_pBTest = null;
        public Rect inspectorRect = new Rect(1, 22, 120, 50);

        EState m_nState = EState.Close;
        bool m_bTweeing = false;
        float m_fTweenDelta = 0;
        float m_fTweenDuration = 0;

        GUIStyle m_pTileStyle = null;

        AgentTreeEditorLogic m_pLogic;
        private string SeachFunc = "";
        private UnityEngine.Object ATSearchObject;

        Vector2 m_Scroll = Vector2.zero;
        private ReorderableList m_list;
        List<IGraphNode> m_vNodes = new List<IGraphNode>();
        //------------------------------------------------------
        public void Open(AgentTreeEditorLogic logic, float fDuration = 0.1f )
        {
            m_pLogic = logic;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            m_bTweeing = true;
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public void Close(float fDuration = 0.1f)
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
            m_bTweeing = true;
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public bool IsSearching(IGraphNode pNode)
        {
            return m_nState == EState.Open && m_vNodes.Contains(pNode);
        }
        //------------------------------------------------------
        void OnSelectItem(ReorderableList list)
        {
        }
        //------------------------------------------------------
        void DrawElement(Rect rect, int index, bool selected, bool focused)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width- rect.height, rect.height), m_vNodes[index].GetDesc());
            if(GUI.Button(new Rect(rect.xMax- rect.height, rect.y,rect.height, rect.height), "☛"))
            {
                m_pLogic.SelectNode(m_vNodes[index], false);
                m_pLogic.CenterView(m_vNodes[index]);
                m_list.index = index;
            }
        }
        //------------------------------------------------------
        public bool IsMouseIn(Vector2 mousePos)
        {
            return m_nState == EState.Open && inspectorRect.Contains(mousePos);
        }
        //------------------------------------------------------
        public void OnDraw(Rect rect)
        {
            inspectorRect.height = rect.height;
            inspectorRect.width = 300;

            if (m_nState == EState.Close || Event.current.type == EventType.ScrollWheel) return;
      //      if (Event.current.type <= EventType.ScrollWheel) return;

            if(m_list == null)
            {
                m_list = new ReorderableList(m_vNodes, typeof(GraphNode));
                m_list.displayAdd = false;
                m_list.displayRemove = false;
                m_list.draggable = false;
                m_list.onSelectCallback = OnSelectItem;
                m_list.drawElementCallback = DrawElement;
            }

            if (m_pBTest == null)
            {
                m_pBTest = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                for(int x =0; x < 2; ++x)
                {
                    for(int z =0;z <2;++z)
                    {
                        m_pBTest.SetPixel(x, z, new Color(0.25f, 0.25f, 0.25f, 0.95f));
                    }
                }
                m_pBTest.Apply();
            }

            AgentTreeUtl.BeginArea(inspectorRect, m_pBTest);
            GUI.DrawTexture(new Rect(0, 0, inspectorRect.width, inspectorRect.height), m_pBTest);
            AgentTreeUtl.BeginArea(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            OnGUI(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            AgentTreeUtl.EndArea();
            AgentTreeUtl.EndArea();
        }
        //------------------------------------------------------
        public void Update(Rect rect, float fTime)
        {
            if (!m_bTweeing) return;
            if (m_nState == EState.Open)
            {
                m_fTweenDelta += fTime;
                if(m_fTweenDuration>0)
                {
                    inspectorRect.x = -(1 - Mathf.Clamp01(m_fTweenDelta / m_fTweenDuration)) * inspectorRect.width;
                }
                if (m_fTweenDelta >= m_fTweenDuration)
                {
                    m_bTweeing = false;
                }
            }
            else if (m_nState == EState.Close)
            {
                m_fTweenDelta += fTime;
                if (m_fTweenDuration > 0)
                {
                    inspectorRect.x = -(Mathf.Clamp01(m_fTweenDelta / m_fTweenDuration)) * inspectorRect.width;
                }
                if (m_fTweenDelta >= m_fTweenDuration)
                {
                    m_bTweeing = false;
                }
            }
        }
        //------------------------------------------------------
        private void OnGUI(Rect rc)
        {
            if (m_nState == EState.Close) return;
            if(m_pTileStyle == null)
            {
                m_pTileStyle = new GUIStyle();
                m_pTileStyle.fontSize = 16;
            }
            GUILayout.Box("搜索面板", AgentTreeEditorResources.styles.nodeHeader);
            ATSearchObject = EditorGUILayout.ObjectField("GO对象", ATSearchObject, typeof(GameObject), true);
            SeachFunc = EditorGUILayout.TextField("函数名", SeachFunc);

            if (GUILayout.Button("搜索"))
            {
                ClearSearch();
                if (ATSearchObject)
                    FinderGameObject(ATSearchObject);
                if (!string.IsNullOrEmpty(SeachFunc))
                    FinderFuncName(SeachFunc);
            }
            m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
            if (m_list!=null) m_list.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
        //------------------------------------------------------
        void ClearSearch()
        {
            m_vNodes.Clear();
        }
        //------------------------------------------------------
        private void FinderGameObject(UnityEngine.Object go)
        {
            //遍历所有ActionTask
            foreach (var db in m_pLogic.ActionNodes)
            {
                if (db.Value.BindNode != null && db.Value.BindNode.inArgvs != null && db.Value.BindNode.inArgvs.Length > 0)
                {
                    foreach (var port in db.Value.BindNode.inArgvs)
                    {
                        if (port.variable is VariableObject)//单个物品
                        {
                            var variable = port.variable as VariableObject;
                            if (variable != null && variable.mValue != null && variable.mValue == go)
                            {
                                if(!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                            }
                        }
                        else if (port.variable is VariableObjectList)//多个物品
                        {
                            var variable = port.variable as VariableObjectList;
                            if (variable != null && variable.mValue != null)
                            {
                                foreach (var item in variable.mValue)
                                {
                                    if (item == go)
                                    {
                                        if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                                    }
                                }
                            }
                        }
                        else if (port.variable is VariableMonoScript)//UI组件,基础自Behaviour
                        {
                            var variable = port.variable as VariableMonoScript;
                            if (variable != null && variable.mValue != null && variable.mValue.gameObject == go)
                            {
                                if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                            }
                        }
                        else if (port.variable is VariableMonoScriptList)//脚本自定义组件拖拽的UI数组
                        {
                            var variable = port.variable as VariableMonoScriptList;
                            if (variable != null && variable.mValue != null)
                            {
                                foreach (var item in variable.mValue)
                                {
                                    if (item.gameObject == go)
                                    {
                                        if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
                else if (db.Value.BindNode == null)//入口节点
                {
                    int key = db.Key;
                    int index = -100 - key;

                    if (m_pLogic.ATData != null && m_pLogic.ATData.Data != null && m_pLogic.ATData.Data.Tasks != null && index < m_pLogic.ATData.Data.Tasks.Count && index >= 0)
                    {
                        EnterNode task = m_pLogic.ATData.Data.Tasks[index].EnterNode;
                        if (task != null && task.CustomGO != null && task.CustomGO == go)
                        {
                            if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        bool IsQuery(string query, string strContext)
        {
            if (string.IsNullOrEmpty(query)) return true;
            if (string.IsNullOrEmpty(strContext)) return false;
            if (strContext.Length > query.Length)
            {
                return strContext.ToLower().Contains(query.ToLower());
            }
            return query.ToLower().Contains(strContext.ToLower());
        }
        //------------------------------------------------------
        private void FinderFuncName(string query)
        {
            foreach (var db in m_pLogic.ActionNodes)
            {
                if (db.Value.BindNode != null)
                {
                    string name = db.Value.BindNode.GetExcudeHash() + ":" + db.Value.BindNode.strName;
                    ATEditorAttrData nodeAttr = db.Value.BindNode.GetEditAttrData<ATEditorAttrData>();
                    if(nodeAttr!=null)
                    {
                        name += nodeAttr.GetQueryName();
                    }
                    if(IsQuery(query, name))
                    {
                        if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                    }
                }
                else if (db.Value.BindNode == null)//入口节点
                {
                    int key = db.Key;
                    int index = -100 - key;

                    if (m_pLogic.ATData != null && m_pLogic.ATData.Data != null && m_pLogic.ATData.Data.Tasks != null && index < m_pLogic.ATData.Data.Tasks.Count && index >= 0)
                    {
                        EnterNode task = m_pLogic.ATData.Data.Tasks[index].EnterNode;
                        if (task != null && IsQuery(query, "custom:"+task.CustomName))
                        {
                            if (!m_vNodes.Contains(db.Value)) m_vNodes.Add(db.Value);
                        }
                    }
                }
            }
        }
    }
}
#endif