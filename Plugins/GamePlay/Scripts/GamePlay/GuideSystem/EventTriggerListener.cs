/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	EventTriggerListener
作    者:	HappLI
描    述:	UI 公共监听
*********************************************************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif
namespace Framework.Guide
{
    public class EventTriggerListener : AEventTriggerListener
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EventTriggerListener))]
    [CanEditMultipleObjects]
    public class EventTriggerListenerEditor : AEventTriggerListenerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}