#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using Framework.Core;
using UnityEngine;

namespace Framework.ED
{
    [EventPopClass]
    public class EventPopDatas
    {
        static List<string> m_vEventPops = new List<string>();
        static List<ushort> m_vEventTypes = new List<ushort>();
        static Dictionary<int, System.Type> m_vEvents = new Dictionary<int, Type>();
        public static List<string> GetEventPops()
        {
            if(m_vEventPops.Count<=0)
                Check();
            return m_vEventPops;
        }
        //------------------------------------------------------
        public static string GetEventName(ushort type)
        {
            int index = m_vEventTypes.IndexOf(type);
            if (index < 0) return "";
            return m_vEventPops[index];
        }
        //------------------------------------------------------
        public static List<ushort> GetEventTypes()
        {
            if (m_vEventTypes.Count <= 0)
                Check();
            return m_vEventTypes;
        }
        //------------------------------------------------------
        public static BaseEvent NewEvent(int nType)
        {
            if(m_vEvents.TryGetValue(nType, out var eventType))
            {
                var evntObj = Activator.CreateInstance(eventType);
                if (evntObj == null) return null;
                return evntObj as BaseEvent;
            }
            return null;
        }
        //------------------------------------------------------
        public static ushort DrawEventPop(ushort curType, string label)
        {
            if (m_vEventTypes.Count <= 0)
                Check();
            int index=  m_vEventTypes.IndexOf(curType);
            if (string.IsNullOrEmpty(label))
                index = EditorGUILayout.Popup(index, m_vEventPops.ToArray());
            else
                index = EditorGUILayout.Popup(label, index, m_vEventPops.ToArray());
            if (index >= 0 && index < m_vEventTypes.Count)
                curType = m_vEventTypes[index];

            return curType;
        }
        //------------------------------------------------------
        static void Check()
        {
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = ass.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type tp = types[i];
                    if (tp.IsDefined(typeof(EventDeclarationAttribute), false))
                    {
                        EventDeclarationAttribute attr = (EventDeclarationAttribute)tp.GetCustomAttribute(typeof(EventDeclarationAttribute));
                        if (attr.bEnable)
                        {
                            m_vEventPops.Add(attr.EventName);
                            m_vEventTypes.Add(attr.eType);

                            m_vEvents[attr.eType] = tp;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        static int ms_nEditorIndex = -1;
        static List<BaseEvent> ms_EditorCmds = new List<BaseEvent>();
        static ushort m_nAddEventCmd = 0;
        //------------------------------------------------------
        public static string[] DrawEvents(string label, string[] cmds, GUILayoutOption[] ops =null)
        {
            ms_EditorCmds.Clear();
            if(ops!=null) GUILayout.Box(label, ops);
            else GUILayout.Box(label);
            if (cmds!=null)
            {
                for(int i =0; i < cmds.Length; ++i)
                {
                    var evt = BaseEvent.NewEvent(null, cmds[i]);
                    if(evt!=null) ms_EditorCmds.Add(evt);
                }
                for (int i = 0; i < ms_EditorCmds.Count; ++i)
                {
                    var evnt = ms_EditorCmds[i];
                    if (ops != null) GUILayout.BeginHorizontal(ops);
                    else GUILayout.BeginHorizontal();
                    evnt.bExpand = UnityEditor.EditorGUILayout.Foldout(ms_nEditorIndex == i, GetEventName(evnt.GetEventType()));
                    if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(30) }))
                    {
                        ms_EditorCmds.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();
                    if (evnt.bExpand)
                    {
                        ms_nEditorIndex = i;
                        Framework.ED.DrawEventCore.DrawUnAction(evnt);
                    }
                    else
                    {
                        if (ms_nEditorIndex == i)
                            ms_nEditorIndex = -1;
                    }
                }
            }
            
            GUILayout.BeginHorizontal();
            m_nAddEventCmd = Framework.ED.EventPopDatas.DrawEventPop(m_nAddEventCmd, null);
            if (GUILayout.Button("Ìí¼ÓÊÂ¼þ"))
            {
                var eventParam = Framework.Core.BuildEventUtl.BuildEventByType(null, m_nAddEventCmd);
                ms_EditorCmds.Add(eventParam);
                ms_nEditorIndex = ms_EditorCmds.Count - 1;
            }
            GUILayout.EndHorizontal();

            if (cmds == null || cmds.Length != ms_EditorCmds.Count)
                cmds = new string[ms_EditorCmds.Count];
            for (int i = 0; i < ms_EditorCmds.Count; ++i)
                cmds[i] = ms_EditorCmds[i].WriteCmd();
            ms_EditorCmds.Clear();
            return cmds;
        }
    }
}
#endif