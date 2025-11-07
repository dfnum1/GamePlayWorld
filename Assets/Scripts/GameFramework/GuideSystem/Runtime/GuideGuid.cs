#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideGuid
作    者:	HappLI
描    述:	引导GUID 生成器
*********************************************************************/
using Framework.UI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    public class GuideGuidUtl
    {
        public static bool bGuideGuidLog;
        static Dictionary<int, GuideGuid> ms_vGuids = new Dictionary<int, GuideGuid>(128);
        public static void OnAdd(GuideGuid guid, bool bCheck = true)
        {
            if (guid == null || guid.Guid == 0) return;
#if UNITY_EDITOR
            if(bCheck)
            {
                GuideGuid src;
                if (ms_vGuids.TryGetValue(guid.Guid, out src) && src != guid && !guid.name.Contains("Clone") && src.name != guid.name)
                {
                    UnityEditor.EditorUtility.DisplayDialog("警告", src.name + "  和 " + guid.name + " 重GUID" + guid.Guid, "检查");
                }
            }

#endif
            ms_vGuids[guid.Guid] = guid;
            if (bGuideGuidLog)
            {
                Log("添加guid:" + guid.Guid + ",组件:" + guid.name);
            }
        }
        public static void OnRemove(GuideGuid guid)
        {
            if (guid == null || guid.Guid == 0) return;

            GuideGuid oldGuid = FindGuide(guid.Guid);
            if(oldGuid == guid)
            {
                ms_vGuids.Remove(guid.Guid);
                if (bGuideGuidLog)
                {
                    Log("移除guid:" + guid.Guid + ",组件:" + guid.name);
                }
            }
        }
        public static GuideGuid FindGuide(int guid)
        {
            if (guid == 0) return null;
            GuideGuid guide;
            if (ms_vGuids.TryGetValue(guid, out guide))
                return guide;
            return null;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public static void PrintAllGuideGuid()
        {
            foreach (var item in ms_vGuids)
            {
                if (item.Value != null)
                {
                    Log("guid:" + item.Value.Guid + ",组件:" + item.Value.name);
                }
                else
                {
                    Log("guid:" + item.Key + ",组件:null");
                }
            }
        }
        //------------------------------------------------------
        public static int GeneratorGUID(GuideGuid guide)
        {
            if (guide == null) return 0;
            string path = guide.name;
            var transform = guide.transform.parent;
            while (transform && transform.GetComponent<UI.UserInterface>() == null)
            {
                path = transform.name + "/" + path;
                transform = transform.parent;
            }
            return Animator.StringToHash(path);
        }
        //------------------------------------------------------
        public static bool SetDirtyPrefab(GuideGuid guide)
        {
            if (guide == null) return false;
            string path = guide.name;
            var transform = guide.transform.parent;
            var instance =  transform.GetComponent<Core.AInstanceAble>();
            while (instance == null && transform)
            {
                path = transform.name + "/" + path;
                transform = transform.parent;
            }
            if(instance)
            {
                GameObject prefab = instance.Prefab;
                if(prefab == null && !string.IsNullOrEmpty(instance.AssetFile))
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(instance.AssetFile);
                if (prefab != null)
                {
                    var childNode = BaseUtil.FindTransformByPath(prefab.transform, path);
                    if(childNode)
                    {
                        var temp = childNode.GetComponent<GuideGuid>();
                        if (temp == null) temp = childNode.gameObject.AddComponent<GuideGuid>();
                        temp.Guid = guide.Guid;

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
        }
#endif
        //------------------------------------------------------
        public static EUIWidgetTriggerType GetTriggerType(UI.EUIEventType uiType)
        {
            switch (uiType)
            {
                case UI.EUIEventType.onClick:
                    {
                        return Framework.Plugin.Guide.EUIWidgetTriggerType.Click;
                    }
                case UI.EUIEventType.onDown: return Framework.Plugin.Guide.EUIWidgetTriggerType.Down;
                case UI.EUIEventType.onEnter: return Framework.Plugin.Guide.EUIWidgetTriggerType.Enter;
                case UI.EUIEventType.onExit: return Framework.Plugin.Guide.EUIWidgetTriggerType.Exit;
                case UI.EUIEventType.onUp: return Framework.Plugin.Guide.EUIWidgetTriggerType.Select;
                case UI.EUIEventType.onSelect: return Framework.Plugin.Guide.EUIWidgetTriggerType.Select;
                case UI.EUIEventType.onUpdateSelect: return Framework.Plugin.Guide.EUIWidgetTriggerType.UpdateSelect;
                case UI.EUIEventType.onDrag: return Framework.Plugin.Guide.EUIWidgetTriggerType.Drag;
                case UI.EUIEventType.onDrop: return Framework.Plugin.Guide.EUIWidgetTriggerType.Drop;
                case UI.EUIEventType.onDeselect: return Framework.Plugin.Guide.EUIWidgetTriggerType.Deselect;
                case UI.EUIEventType.onScroll: return Framework.Plugin.Guide.EUIWidgetTriggerType.Scroll;
                case UI.EUIEventType.onBeginDrag: return Framework.Plugin.Guide.EUIWidgetTriggerType.BeginDrag;
                case UI.EUIEventType.onEndDrag: return Framework.Plugin.Guide.EUIWidgetTriggerType.EndDrag;
                case UI.EUIEventType.onSubmit: return Framework.Plugin.Guide.EUIWidgetTriggerType.Submit;
                case UI.EUIEventType.onCancel: return Framework.Plugin.Guide.EUIWidgetTriggerType.Cancel;
                case UI.EUIEventType.onMove: return Framework.Plugin.Guide.EUIWidgetTriggerType.Move;
                default: return EUIWidgetTriggerType.None;
            }
        }
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
        public int Guid = 0;
        public bool ConvertUIPos = false;
        public Plugin.Guide.IGuideScroll ScrollList;
        public void Awake()
        {
            if (Guid != 0 )
                GuideGuidUtl.OnAdd(this);
        }
        //------------------------------------------------------
        private void OnEnable()
        {
            if (Guid != 0)
                GuideGuidUtl.OnAdd(this);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            if (Guid != 0)
                GuideGuidUtl.OnRemove(this);
        }
        //------------------------------------------------------
        private void OnDisable()
        {
//             if (Guid != 0)
//                 GuideGuidUtl.OnRemove(this);
            //      if (Guid != 0 && OnRemoveGuideGuid != null)
            //         OnRemoveGuideGuid(this);
        }
    }
}
#endif