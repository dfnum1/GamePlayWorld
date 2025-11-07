#if UNITY_EDITOR && USE_GUIDESYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Framework.Plugin.Guide
{
    public partial class EventInspector
    {
        enum EState
        {
            Stand,
            Open,
            Close,
        }
        Texture2D m_pBTest = null;
        public Rect inspectorRect = new Rect(1, 22, 120, 50);

        EState m_nState = EState.Stand;
        float m_fTweenDelta = 0;
        float m_fTweenDuration = 0;

        GUIStyle m_pTileStyle = null;

        private int m_nAddEventType = 0;
        private Vector2 m_Scroll = Vector2.zero;

        List<IUserData> m_vEvents = null;
        //------------------------------------------------------
        public void Open(List<IUserData> vNode, float fDuration = 0.1f )
        {
            m_vEvents = vNode;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public void Close(float fDuration = 0.1f)
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public void OnDraw(Rect rect)
        {
            if (m_vEvents == null) return;
            if(m_pBTest == null)
            {
                m_pBTest = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                for(int x =0; x < 2; ++x)
                {
                    for(int z =0;z <2;++z)
                    {
                        m_pBTest.SetPixel(x, z, new Color(0.5f, 0.5f, 0.5f, 0.8f));
                    }
                }
                m_pBTest.Apply();
            }
            inspectorRect.height = rect.height;
            inspectorRect.width = 300;
            GuideEditorLogic.BeginArea(inspectorRect, m_pBTest);
            GUI.DrawTexture(new Rect(0,0, inspectorRect.width, inspectorRect.height), m_pBTest);
            GuideEditorLogic.BeginArea(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            OnGUI(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            GuideEditorLogic.EndArea();
            GuideEditorLogic.EndArea();
        }
        //------------------------------------------------------
        public void Update(float fTime)
        {
            if (m_nState == EState.Open)
            {
                m_fTweenDelta += fTime;
                if(m_fTweenDuration>0)
                {
                    inspectorRect.x = -(1 - Mathf.Clamp01(m_fTweenDelta / m_fTweenDuration)) * inspectorRect.width;
                }
                if (m_fTweenDelta >= m_fTweenDuration)
                {
                    m_nState = EState.Stand;
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
                    m_vEvents = null;
                    m_nState = EState.Stand;
                }
            }
        }
        //------------------------------------------------------
        private void OnGUI(Rect rc)
        {
            if(m_pTileStyle == null)
            {
                m_pTileStyle = new GUIStyle();
                m_pTileStyle.fontSize = 16;
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            if (GuideEditor.EventPopMethod != null)
            {
                System.Type paramType = GuideEditor.EventPopMethod.ReturnType;
                if (paramType.IsEnum)
                {
                    System.Enum evtType = (System.Enum)System.Enum.ToObject(paramType, m_nAddEventType);
                    GUILayout.BeginHorizontal();
                    object retType = GuideEditor.EventPopMethod.Invoke(null, new object[] { evtType, "事件类型" });

                    m_nAddEventType = Convert.ToInt32(retType);
                    if (GuideEditor.EventBuildByTypeMethod != null && GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) }))
                    {
                        object ret = GuideEditor.EventBuildByTypeMethod.Invoke(null, new object[] { System.Enum.ToObject(paramType, m_nAddEventType) });
                        if (ret != null && ret is IUserData)
                        {
                            m_vEvents.Add(ret as IUserData);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, new GUILayoutOption[] { GUILayout.Width(inspectorRect.width-10) });

            if (GuideEditor.EventDrawMethod != null)
            {
                for(int i=0; i < m_vEvents.Count; ++i)
                {
                    System.Reflection.MethodInfo pMethod = m_vEvents[i].GetType().GetMethod("GetEventType");
                    string strTypeName = "";
                    if (pMethod != null && GuideEditor.EventNameGet != null)
                    {
                        object temp = GuideEditor.EventNameGet.Invoke(null, new object[] { pMethod.Invoke(m_vEvents[i], new object[] { }) });
                        if(temp!=null)
                            strTypeName = (string)temp;
                    }
                    if (string.IsNullOrEmpty(strTypeName))
                        strTypeName = "[" + i + "]";
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(inspectorRect.width - 50) });
                    GUILayout.Box(strTypeName, new GUILayoutOption[] { GUILayout.Width(inspectorRect.width - 70) });
                    if(GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(20) }))
                    {
                        if(EditorUtility.DisplayDialog("提示", "是否确认删除事件？", "删除", "取消"))
                        {
                            m_vEvents.RemoveAt(i);
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                    m_vEvents[i] = GuideEditor.EventDrawMethod.Invoke(null, new object[] { m_vEvents[i], null }) as IUserData;

                }
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif