#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Plugin.AT
{
	public class TreeIMGUI<T> where T : ITreeIMGUIData
	{
        private readonly TreeNode<T> _root;

		private Rect _controlRect;
		private float _drawY;
		private float _height;
		private TreeNode<T> _selected;
		private int _controlID;

        public System.Action<T, bool> onSelected;
        public System.Action<T> onDoubleClick;
        public System.Action<T> onDraw;

        public double lastClickTime;
        public TreeIMGUI(TreeNode<T> root)
		{
			_root = root;
		}

		public void DrawTreeLayout()
		{
			_height = 0;
			_drawY = 0;
			_root.Traverse(OnGetLayoutHeight);

			_controlRect = EditorGUILayout.GetControlRect(false,_height);
			_controlID = GUIUtility.GetControlID(FocusType.Passive,_controlRect);

            _root.Traverse(OnDrawRow);

            EventType eventType = Event.current.GetTypeForControl(_controlID);
            if (_selected != null)
            {
                if (eventType == EventType.KeyDown)
                {
                    if (Event.current.keyCode == KeyCode.D || Event.current.keyCode == KeyCode.RightArrow)
                    {
                        _selected.Data.isExpanded = true;
                    }
                    else if (Event.current.keyCode == KeyCode.A || Event.current.keyCode == KeyCode.LeftArrow)
                    {
                        _selected.Data.isExpanded = false;
                    }
                    //else if (Event.current.keyCode == KeyCode.W || Event.current.keyCode == KeyCode.UpArrow)
                    //{
                    //    _root.TraverseNeighborSelect(_selected, true);
                    //}
                    //else if (Event.current.keyCode == KeyCode.S || Event.current.keyCode == KeyCode.DownArrow)
                    //{
                    //    _root.TraverseNeighborSelect(_selected, false);
                    //}
                }
            }
        }

		protected virtual float GetRowHeight(TreeNode<T> node)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		protected virtual bool OnGetLayoutHeight(TreeNode<T> node)
		{
			if(node.Data==null) return true;

			_height += GetRowHeight(node);
			return node.Data.isExpanded;
		}

		protected virtual bool OnDrawRow(TreeNode<T> node)
		{
			if(node.Data==null) return true;

            if (onDraw != null) onDraw(node.Data);


            float rowIndent = 14*node.Level;
			float rowHeight = GetRowHeight(node);

			Rect rowRect = new Rect(0,_controlRect.y+_drawY,_controlRect.width,rowHeight);
			Rect indentRect = new Rect(rowIndent,_controlRect.y+_drawY,_controlRect.width-rowIndent,rowHeight);

			// render
			if(_selected==node){
				EditorGUI.DrawRect(rowRect,Color.gray);
			}

			OnDrawTreeNode(indentRect,node,_selected==node,false);

			// test for events
			EventType eventType = Event.current.GetTypeForControl(_controlID);
			if(eventType==EventType.MouseUp && rowRect.Contains(Event.current.mousePosition)){
                if (onSelected != null) onSelected(node.Data, _selected == node);
                _selected = node;

                if(Mathf.Abs((float)(lastClickTime - EditorApplication.timeSinceStartup)) <= 0.2f)
                {
                    if (onDoubleClick != null) onDoubleClick(node.Data);
                }
                lastClickTime = EditorApplication.timeSinceStartup;
                GUI.changed = true;
				Event.current.Use();
            }
            _drawY += rowHeight;

			return node.Data.isExpanded;
		}

		protected virtual void OnDrawTreeNode(Rect rect, TreeNode<T> node, bool selected, bool focus)
		{
			GUIContent labelContent = new GUIContent(node.Data.ToString());

			if(!node.IsLeaf){
				node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x-12,rect.y,12,rect.height),node.Data.isExpanded,GUIContent.none);
			}

			EditorGUI.LabelField(rect,labelContent,selected ? EditorStyles.whiteLabel : EditorStyles.label);
		}

	}

	public interface ITreeIMGUIData
	{

		bool isExpanded { get; set; }

	}

}
#endif