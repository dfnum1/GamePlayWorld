#if UNITY_EDITOR && USE_GUIDESYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System;

namespace Framework.Plugin.Guide
{
    //------------------------------------------------------
    public partial class GuideSearcher
    {
        public class ItemEvent : AssetTree.ItemData
        {
            public object param;
            public System.Action<object> callback;
            public override Color itemColor()
            {
                return Color.white;
            }
            public override Texture2D itemIcon()
            {
                return Resources.Load<Texture2D>("at_tr_noder");
            }
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
        protected AssetTree m_assetTree;

        //------------------------------------------------------
        public void Open(Rect rect)
        {
            inspectorRect = rect;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            Init();
            Search("");
            OnOpen();
        }
        //------------------------------------------------------
        protected virtual void OnOpen() { }

        //------------------------------------------------------
        public void Close()
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
            OnClose();
        }
        //------------------------------------------------------
        protected virtual void OnClose() { }
        //------------------------------------------------------
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
                MultiColumnHeaderState.Column[] colums = new MultiColumnHeaderState.Column[] {
                        new MultiColumnHeaderState.Column(),
                };
                colums[0].headerContent = new GUIContent("Name", "");
                colums[0].minWidth = 100;
                colums[0].width = 100;
                colums[0].maxWidth = 100;
                colums[0].headerTextAlignment = TextAlignment.Left;
                colums[0].canSort = true;
                colums[0].autoResize = true;
                m_assetTree = new AssetTree(new UnityEditor.IMGUI.Controls.TreeViewState(), new MultiColumnHeaderState(colums));
                m_assetTree.Reload();

                m_assetTree.OnItemDoubleClick = OnSelected;
            //    m_assetTree.OnSelectChange = OnSelected;
                m_assetTree.OnCellDraw = OnDrawItem;
                m_assetTree.OnDragItem = OnDragItem;
                m_assetTree.OnDragDrop = OnDragDrop;
            }
        }
        //------------------------------------------------------
        void OnSelected(AssetTree.ItemData data)
        {
            ItemEvent evt = data as ItemEvent;
            if (evt.callback != null)
            {
                evt.callback(evt.param);
            }
            Close();
        }
        //------------------------------------------------------
        protected virtual bool OnDrawItem(Rect cellRect, AssetTree.TreeItemData item, int column, bool bSelected, bool focused)
        {
            AssetTree.ItemData itemData = item.data as AssetTree.ItemData;
            item.displayName = itemData.name;
            TreeView.DefaultGUI.Label(cellRect,item.displayName, bSelected, focused);
            return true;
        }
        //------------------------------------------------------
        protected virtual bool OnDragItem(AssetTree.ItemData item)
        {
            return false;
        }
        //------------------------------------------------------
        protected virtual void OnDragDrop(AssetTree.DragAndDropData drop)
        {
            if(drop.current!=null )
            {
                List<AssetTree.ItemData> vItems = m_assetTree.GetDatas();
                if (drop.insertAtIndex < 0)
                {
                    if(drop.parentItem!=null)
                    {
                        //swap
                        int cur = vItems.IndexOf(drop.current.data);
                        int index = vItems.IndexOf(drop.parentItem.data);
                        if(index!= cur)
                        {
                            AssetTree.ItemData temp = vItems[cur];
                            vItems[cur] = drop.parentItem.data;
                            vItems[index] = temp;
                            m_assetTree.Reload();
                            OnSawpDatas();
                        }
                    }
                }
                else
                {
                    if (drop.insertAtIndex <= vItems.Count)
                    {
                        int cur = vItems.IndexOf(drop.current.data);
                        if (cur == drop.insertAtIndex) return;
                        if (cur < drop.insertAtIndex)
                        {
                            vItems.Insert(drop.insertAtIndex, drop.current.data);
                            vItems.RemoveAt(cur);
                        }
                        else
                        {
                            vItems.Insert(drop.insertAtIndex, drop.current.data);
                            vItems.RemoveAt(cur + 1);
                        }
                        m_assetTree.Reload();
                        OnSawpDatas();
                    }
                }
   

            }
        }
        //------------------------------------------------------
        protected virtual void OnSawpDatas() { }
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
            GuideEditorLogic.BeginArea(inspectorRect, m_pBTest);
            GUI.Box(new Rect(0, 0, inspectorRect.width, inspectorRect.height), m_pBTest);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField("Search:", EditorStyles.miniLabel, GUILayout.Width(40));

            EditorGUI.BeginChangeCheck();

            //搜索栏聚焦
            GUI.SetNextControlName("search");
            m_assetTree.searchString = EditorGUILayout.TextField(m_assetTree.searchString, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
            if(new Rect(0, 0, inspectorRect.width, 30).Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                GUI.FocusControl("search");
            
            if (EditorGUI.EndChangeCheck())
            {
                Search(m_assetTree.searchString);
            }

            EditorGUILayout.EndHorizontal();

            m_assetTree.multiColumnHeader.GetColumn(0).width = inspectorRect.width - 10;
            // draw tree
            m_assetTree.OnGUI(new Rect(0, EditorStyles.toolbar.fixedHeight, inspectorRect.width, inspectorRect.height- EditorStyles.toolbar.fixedHeight));
            GuideEditorLogic.EndArea();
        }
        //------------------------------------------------------
       protected  bool IsQuery(string query, string strContext)
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
        protected virtual void OnSearch(string query) { }
        //------------------------------------------------------
        protected void Search(string query)
        {
            Init();
            m_assetTree.BeginTreeData();
            m_assetTree.buildMutiColumnDepth = true;
            OnSearch(query);
            m_assetTree.EndTreeData();
        }
    }
}
#endif