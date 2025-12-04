#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Framework.ED
{
    public class TransformSlotProvider : ScriptableObject, ISearchWindowProvider
    {
        // 回调：选中Transform后执行
        public Action<Transform,int> OnTransformSelected;
        public Action<Transform> OnTransformSelected0;

        private int m_nUserIndex;
        // 根节点
        private Transform m_Root;

        // 缓存所有子节点
        private List<Transform> m_AllSlots;

        // 初始化
        public void Init(Transform root)
        {
            m_Root = root;
            BuildSlotList();
        }

        // 构建所有子节点列表
        private void BuildSlotList()
        {
            m_AllSlots = new List<Transform>();
            if (m_Root == null)
                return;
            // 广度优先遍历所有子节点
            var queue = new Queue<Transform>();
            queue.Enqueue(m_Root);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current != m_Root) // 不包含根节点本身
                    m_AllSlots.Add(current);
                for (int i = 0; i < current.childCount; ++i)
                {
                    queue.Enqueue(current.GetChild(i));
                }
            }
        }

        // ISearchWindowProvider 实现
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
			var tree = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("子节点列表"), 0)
			};

			// 记录已添加的分组路径，避免重复
			var groupLevels = new Dictionary<string, int>();

			if (m_AllSlots != null)
			{
				foreach (var slot in m_AllSlots)
				{
					string path = GetTransformPath(slot, m_Root);
					var parts = path.Split('/');
					string groupPath = "";
					int level = 1;

					// 构建分组
					for (int i = 0; i < parts.Length - 1; ++i)
					{
						groupPath = groupPath == "" ? parts[i] : groupPath + "/" + parts[i];
						if (!groupLevels.ContainsKey(groupPath))
						{
							tree.Add(new SearchTreeGroupEntry(new GUIContent(parts[i]), level));
							groupLevels[groupPath] = level;
						}
						level++;
					}

					// 添加Transform条目
					tree.Add(new SearchTreeEntry(new GUIContent(parts.Last()))
					{
						level = level,
						userData = slot
					});
				}
			}
			return tree;
        }

        // 选中回调
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is Transform tf)
            {
                OnTransformSelected?.Invoke(tf, m_nUserIndex);
                OnTransformSelected0?.Invoke(tf);
                return true;
            }
            return false;
        }

        // 获取Transform的完整路径
        private string GetTransformPath(Transform tf, Transform root)
        {
            var names = new List<string>();
            var current = tf;
            while (current != null && current != root)
            {
                names.Add(current.name);
                current = current.parent;
            }
            names.Reverse();
            return string.Join("/", names);
        }

        // 工具方法：弹出窗口
        public static void Show(Transform root, Action<Transform,int> onSelected, int index)
        {
            var provider = CreateInstance<TransformSlotProvider>();
            provider.m_nUserIndex = index;
            provider.OnTransformSelected = onSelected;
            provider.Init(root);
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
        }
        public static void Show(Transform root, Action<Transform> onSelected)
        {
            var provider = CreateInstance<TransformSlotProvider>();
            provider.m_nUserIndex = -1;
            provider.OnTransformSelected0 = onSelected;
            provider.Init(root);
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
        }
    }
}
#endif