#if UNITY_EDITOR
//auto generator
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Framework.Data;
using Framework.Core;
using Framework.ED;

namespace Framework.ED
{
	[EventDraw]
	public class DrawEventCore
	{
		static List<BaseEvent> ms_CurrentEventSceneDraw = new List<BaseEvent>();
		static List<BaseEvent> ms_Events = new List<BaseEvent>();
		static List<string> ms_IngoreNoActionFiled = new List<string>();
		public static Dictionary<ushort, BaseEvent> vCopyParameters = new Dictionary<ushort, BaseEvent>();
		public static List<string> GetNoActionIngoreField()
		{
			if(ms_IngoreNoActionFiled == null || ms_IngoreNoActionFiled.Count<=0)
			{
				ms_IngoreNoActionFiled.Add("totalTriggertCount");
				ms_IngoreNoActionFiled.Add("actionWithBit");
				ms_IngoreNoActionFiled.Add("canTriggerAfterKilled");
				ms_IngoreNoActionFiled.Add("triggetTime");
			}
			return ms_IngoreNoActionFiled;
		}
		public static void CheckCopyByClipBoard()
		{
			if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer)) return;
				BaseEvent copyData = BaseEvent.NewEvent(null, GUIUtility.systemCopyBuffer);
			if(copyData != null)
			{
				AddCopyEvent(copyData);
				GUIUtility.systemCopyBuffer = null;
			}
		}
		public static void AddCopyEvent(BaseEvent evt)
		{
			if(evt == null) return;
				vCopyParameters[evt.GetEventType()] = evt;
		}
		public static BaseEvent GetCopyEvent(ushort type)
		{
			BaseEvent evet;
			if (vCopyParameters.TryGetValue(type, out evet))
				return evet;
			return null;
		}
		public static bool CopyEvent(BaseEvent evt, bool bRemove = true)
		{
			BaseEvent cpy;
			if (vCopyParameters.TryGetValue(evt.GetEventType(), out cpy))
			{
				evt.Copy(cpy);
				if (bRemove) vCopyParameters.Remove(evt.GetEventType());
				return true;
			}
			return false;
		}
		public static bool CanCopyEvent(BaseEvent evt)
		{
			if (evt == null) return false;
			BaseEvent cpy;
			if (vCopyParameters.TryGetValue(evt.GetEventType(), out cpy) && evt != cpy)
				return true;
			return false;
		}
		public static BaseEvent DrawUnAction(BaseEvent evt, List<string> Slots = null)
		{
			if (Slots != null)
                InspectorDrawUtil.BindSlots = Slots;
			if(!evt.OnInspectorGUI())			evt = (BaseEvent)InspectorDrawUtil.DrawProperty(evt, GetNoActionIngoreField());
            InspectorDrawUtil.BindSlots=null;
			return evt;
		}
		public static void Draw(ActionEventCore pCore, List<string> Slots = null, System.Action<BaseEvent, string> OnDrawEvent = null, int EventLayerMask = -1)
		{
			if(Slots != null)
                InspectorDrawUtil.BindSlots = Slots;
			ms_Events.Clear();
			pCore.BuildEvent(null, ms_Events);
			for(int i=0; i < ms_Events.Count; ++i)
			{
				if(EventLayerMask>0 &&!ms_Events[i].IsEventBit(EventLayerMask)) continue;
				GUILayout.BeginHorizontal();
				if (OnDrawEvent != null)
				{
				    OnDrawEvent(ms_Events[i], null);
				}
				if (ms_Events[i].OnEdit(true) && GUILayout.Button("编辑", new GUILayoutOption[] { GUILayout.Width(35) }))
				{
					ms_Events[i].OnEdit(false);
					if (ms_CurrentEventSceneDraw.Count > 0) ms_CurrentEventSceneDraw.Clear();
					else ms_CurrentEventSceneDraw.Add(ms_Events[i]);
					if (OnDrawEvent != null) OnDrawEvent(ms_Events[i], "OnEditor_Callback");
				}
				if (ms_Events[i].OnPreview(true) && GUILayout.Button("预览", new GUILayoutOption[] { GUILayout.Width(35) }))
				{
					ms_Events[i].OnPreview(false);
					if (OnDrawEvent != null) OnDrawEvent(ms_Events[i], "OnPreview_Callback");
				}
				if(GUILayout.Button("Del", new GUILayoutOption[] { GUILayout.Width(30) }))
				{
					if (EditorUtility.DisplayDialog("提示", "确定删除 ? ", "确定", "取消"))
					{
						pCore.DelEvent(ms_Events[i]);
						if (ms_CurrentEventSceneDraw.Contains(ms_Events[i])) ms_CurrentEventSceneDraw.Remove(ms_Events[i]);
						break;
					}
				}
				if (GUILayout.Button("Copy", new GUILayoutOption[] { GUILayout.Width(50) }))
				{
				    AddCopyEvent(ms_Events[i]);
				}
				if(CanCopyEvent(ms_Events[i]) && GUILayout.Button("Parse", new GUILayoutOption[] { GUILayout.Width(50) }))
				{
				    CopyEvent(ms_Events[i]);
				}
				ms_Events[i].bExpand = EditorGUILayout.Foldout(ms_Events[i].bExpand, "Event[" + ms_Events[i].GetEventType() + "]");
				GUILayout.EndHorizontal();
				if(ms_Events[i].bExpand)
				{
					EditorGUI.indentLevel++;
					string strTip = ms_Events[i].GetTips();
					if (!string.IsNullOrEmpty(strTip)) EditorGUILayout.HelpBox(strTip, MessageType.Warning);
					if(!ms_Events[i].OnInspectorGUI((string field) =>{if (OnDrawEvent != null) {OnDrawEvent(ms_Events[i], field);} }))
					{
						ms_Events[i]= (BaseEvent)InspectorDrawUtil.DrawProperty(ms_Events[i], null, (string field) =>
						{
							if(OnDrawEvent != null)
							{
								OnDrawEvent(ms_Events[i], field);
							}
						});
					}
					EditorGUI.indentLevel--;
				}
			}
            InspectorDrawUtil.BindSlots = null;
		}
		public static void OnSceneGUI(Rect rect, Transform transform)
		{
			if(ms_CurrentEventSceneDraw == null) return;
			for (int i = 0; i < ms_CurrentEventSceneDraw.Count; ++i)
				ms_CurrentEventSceneDraw[i].OnSceneGUI(rect, transform);
		}
		public static void PlayPreviews(bool bPlay)
		{
			if(ms_CurrentEventSceneDraw == null) return;
			for (int i = 0; i < ms_CurrentEventSceneDraw.Count; ++i)
				ms_CurrentEventSceneDraw[i].PlayPreview(bPlay);
		}
		public static void PreviewUpdates(float fFrame, Transform target)
		{
			if(ms_CurrentEventSceneDraw == null) return;
			for (int i = 0; i < ms_CurrentEventSceneDraw.Count; ++i)
				ms_CurrentEventSceneDraw[i].OnPreviewUpdate(fFrame, target);
		}
		public static void ClearEditingEvents()
		{
			if(ms_CurrentEventSceneDraw == null) return;
			ms_CurrentEventSceneDraw.Clear();
		}
		public static void AddEditEvent(BaseEvent pEvent)
		{
			if (pEvent == null || ms_CurrentEventSceneDraw.Contains(pEvent)) return;
			ms_CurrentEventSceneDraw.Add(pEvent);
		}
	}
}
#endif
