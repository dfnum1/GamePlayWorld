#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Framework.Plugin.AT
{
	public class AssetTreeIMGUI : TreeIMGUI<AssetData>
	{

		public AssetTreeIMGUI(TreeNode<AssetData> root) : base(root)
		{
			
		}

		protected override void OnDrawTreeNode(Rect rect, TreeNode<AssetData> node, bool selected, bool focus)
		{
            GUIContent labelContent = null;
            if(node.Data.icon != null) labelContent = new GUIContent(node.Data.path, node.Data.icon);
            else labelContent = new GUIContent(node.Data.path, AssetDatabase.GetCachedIcon(node.Data.fullPath));

            if (!node.IsLeaf){
				node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x-12,rect.y,12,rect.height),node.Data.isExpanded,GUIContent.none);
			}

			EditorGUI.LabelField(rect,labelContent,selected ? EditorStyles.whiteLabel : EditorStyles.label);
		}

	}

}
#endif