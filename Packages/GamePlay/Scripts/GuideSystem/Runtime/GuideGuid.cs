/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideGuid
作    者:	HappLI
描    述:	引导GUID 生成器
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Guide
{
    [GuideDisplayType(typeof(GuideGuid), "OnDrawGUI", "引导组件")]
    public class GuideGuidUtl
    {
        struct GuideInfo
        {
            public GuideGuid guide;
            public string tag;
        }
        public static bool bGuideGuidLog;
        static Dictionary<int, List<GuideInfo>> ms_vGuids = new Dictionary<int, List<GuideInfo>>(16);
        //------------------------------------------------------
        public static void OnAdd(GuideGuid guid, bool bCheck = true)
        {
            if (guid.guid == 0)
                return;
            if (ms_vGuids.TryGetValue(guid.guid, out var infoList) == false)
            {
                infoList = new List<GuideInfo>(2);
                ms_vGuids[guid.guid] = infoList;
            }
            string findTag = "";
            bool bFindTag = false;
            for (int i =0; i < infoList.Count; )
            {
                var guideInfo = infoList[i];
                if(guideInfo.guide == null)
                {
                    infoList.RemoveAt(i);
                    continue;
                }
                if(guideInfo.guide.guid == guid.guid)
                {
                    if(!bFindTag)
                    {
                        bFindTag = true;
                        var guideTag = GuideTag.GetTag(guid);
                        if (guideTag != null) findTag = guideTag.GetTag();
                    }
                    if(findTag == guideInfo.tag)
                    {
                        if (bGuideGuidLog)
                        {
                            Debug.LogErrorFormat("[GuideGuid] {0}对应路径{1}下已存在相同PathTag的节点:{2}, 引导节点产生歧义, 请联系程序同学处理引导节点命名",
                                guid.guid, GetNodeFullPath(guideInfo.guide.transform), guideInfo.tag);
                        }
                        return;
                    }
                }
                ++i;
            }
            if (!bFindTag)
            {
                bFindTag = true;
                var guideTag = GuideTag.GetTag(guid);
                if (guideTag != null) findTag = guideTag.GetTag();
            }
            infoList.Add(new GuideInfo
            {
                guide = guid,
                tag = findTag
            });
            if (bGuideGuidLog)
            {
                if (findTag == null) findTag = "";
                Log("添加guid:" + guid.guid + ",组件:" + guid.name + "  tag:" + findTag);
            }
        }
        //------------------------------------------------------
        public static void OnRemove(GuideGuid guid)
        {
            if (guid == null || guid.guid == 0) return;

            if (ms_vGuids.TryGetValue(guid.guid, out var infoList))
            {
                for (int i = 0; i < infoList.Count;)
                {
                    var guideInfo = infoList[i];
                    if (guideInfo.guide == guid)
                    {
                        infoList.RemoveAt(i);
                    }
                    else ++i;
                }
            }
        }
        //------------------------------------------------------
        public static GuideGuid FindGuide(int guid, string tag = null)
        {
            if (guid == 0) return null;
            if (ms_vGuids.TryGetValue(guid, out var guideList) && guideList.Count>0)
            {
                if (guideList.Count == 1 || string.IsNullOrEmpty(tag))
                    return guideList[guideList.Count-1].guide;
                for (int i = 0; i < guideList.Count;++i)
                {
                    var guideInfo = guideList[i];
                    if (guideInfo.guide == null)
                    {
                        continue;
                    }
                    if (tag.CompareTo(guideInfo.tag) == 0)
                    {
                        return guideInfo.guide;
                    }
                }
            }
            return null;
        }
        //------------------------------------------------------
        public static Transform GetWidget(int guid, string pathTag = null, int listIndex = -1)
        {
            var compGuid = FindGuide(guid, pathTag);
            if (compGuid != null)
            {
                return compGuid.GetWidget(listIndex);
            }
            return null;
        }
        //------------------------------------------------------
        public static string GetNodeFullPath(Transform node)
        {
            var path = string.Empty;
            var find = node;
            while (find != null)
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = find.name;
                }
                else
                {
                    path = find.name + "/" + path;
                }
                find = find.parent;
            }
            return path;
        }
        //------------------------------------------------------
#if UNITY_EDITOR
        static int OnDrawGUI(string displayName, int value)
        {
            GuideGuid guidWidget = FindGuide(value);
            Color color = GUI.color;

            float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(displayName)).x+5;
            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (guidWidget == null) GUI.color = Color.red;
            GuideGuid guide = (GuideGuid)UnityEditor.EditorGUILayout.ObjectField(new GUIContent(displayName, "控件Id:" + value), guidWidget, typeof(GuideGuid), true);
            GUI.color = color;
            if (guide == null) UnityEditor.EditorGUILayout.HelpBox("引导控件为空", UnityEditor.MessageType.Error);
            UnityEditor.EditorGUILayout.EndHorizontal();
            if (guide != null)
                value = guide.guid;
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
            return value;
        }
        //------------------------------------------------------
        public static void PrintAllGuideGuid()
        {
            foreach (var item in ms_vGuids)
            {
                foreach(var db in item.Value)
                {
                    if (db.guide != null)
                    {
                        Log("guid:" + db.guide + ",组件:" + db.guide.name);
                    }
                    else
                    {
                        Log("guid:" + item.Key + ",组件:null");
                    }
                }
            }
        }
        //------------------------------------------------------
        public static int GeneratorGUID(GuideGuid guide)
        {
            if (guide == null) return 0;
            string path = guide.name;
            var transform = guide.transform.parent;
            while (transform/* && transform.GetComponent<UI.UserInterface>() == null*/)
            {
                path = transform.name + "/" + path;
                transform = transform.parent;
            }
            return Animator.StringToHash(path);
        }
        //------------------------------------------------------
        public static bool SetDirtyPrefab(GuideGuid guide)
        {
            /*
            if (guide == null) return false;
            string path = guide.name;
            var transform = guide.transform.parent;
            var instance = transform.GetComponent<Core.AInstanceAble>();
            while (instance == null && transform)
            {
                path = transform.name + "/" + path;
                transform = transform.parent;
            }
            if (instance)
            {
                GameObject prefab = instance.Prefab;
                if (prefab == null && !string.IsNullOrEmpty(instance.AssetFile))
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(instance.AssetFile);
                if (prefab != null)
                {
                    var childNode = BaseUtil.FindTransformByPath(prefab.transform, path);
                    if (childNode)
                    {
                        var temp = childNode.GetComponent<GuideGuid>();
                        if (temp == null) temp = childNode.gameObject.AddComponent<GuideGuid>();
                        temp.guid = guide.guid;

                        EventTriggerListener eventTrigger = childNode.gameObject.GetComponent<EventTriggerListener>();
                        if (eventTrigger == null) eventTrigger = childNode.gameObject.AddComponent<EventTriggerListener>();
                        eventTrigger.SetGuideGuid(temp);
                        EditorUtility.SetDirty(temp);
                        EditorUtility.SetDirty(eventTrigger);
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssetIfDirty(prefab);
                    }
                    else
                    {
                        Debug.LogError("无法找到对应组件预制体的子节点:" + path);
                    }
                }
                else
                {
                    Debug.LogError("无法找到对应组件的预制体");
                }
            }
            return instance != null;
            */
            return false;
        }
#endif
        //------------------------------------------------------
        public static void Log(string log)
        {
            if (bGuideGuidLog)
            {
                Debug.LogWarning("GuideLog: " + log);
            }
        }
    }
    [DisallowMultipleComponent]
    public class GuideGuid : MonoBehaviour 
	{
        public int guid = 0;
        public int listIndex = -1;
        public bool ConvertUIPos = false;
        public UnityEngine.Component listViewComp;

        public void Awake()
        {
            if (guid != 0 )
                GuideGuidUtl.OnAdd(this);
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            if (guid != 0)
                GuideGuidUtl.OnAdd(this);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            if (guid != 0)
                GuideGuidUtl.OnRemove(this);
        }
        //------------------------------------------------------
        private void OnDisable()
        {
//             if (guid != 0)
//                 GuideGuidUtl.OnRemove(this);
            //      if (guid != 0 && OnRemoveGuideGuid != null)
            //         OnRemoveGuideGuid(this);
        }
        //------------------------------------------------------
        public Transform GetWidget(int index =-1)
        {
            if (index < 0) index = listIndex;
            if (index >= 0)
            {
                IGuideScroll scoller = null;
                if (listViewComp == null)
                {
                    scoller = GetComponent<IGuideScroll>();
                    if(scoller == null)
                        listViewComp = GetComponent<ScrollRect>();
                    if (scoller !=null && scoller is UnityEngine.Component)
                    {
                        listViewComp = (UnityEngine.Component)scoller;
                    }
                }
                else
                {
                    scoller = listViewComp as IGuideScroll;
                }
                if (scoller != null) return scoller.GetItemByIndex(index);
                if (listViewComp != null)
                {
                    if (listViewComp is ScrollRect)
                    {
                        ScrollRect listView = (ScrollRect)listViewComp;
                        if (listView.content == null) return null;
                        if (index < listView.content.childCount) return listView.content.GetChild(index);
                    }
                    else
                    {
                        Debug.LogWarning("listViewComp is not IScrollListView or ScrollRect");
                    }
                }
                return null;
            }
            return transform;
        }
    }
}
