#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeStructInspector
    {
        enum EState
        {
            Stand,
            Open,
            Close,
        }
        Texture2D m_pBTest = null;
        public Rect inspectorRect = new Rect(1, 22, 120, 50);

        EState m_nState = EState.Close;
        bool m_bTweeing = false;
        float m_fTweenDelta = 0;
        float m_fTweenDuration = 0;

        GUIStyle m_pTileStyle = null;

        AgentTreeEditorLogic m_pLogic;

        Vector2 m_Scroll = Vector2.zero;
        private string m_AddStructName = "";
        private EVariableType m_AddVariableType = EVariableType.UserData;

        DrawUIParam m_DrawUIParam = new DrawUIParam();
        //------------------------------------------------------
        public void Open(AgentTreeEditorLogic logic, float fDuration = 0.1f )
        {
            m_pLogic = logic;
            if (m_nState == EState.Open) return;
            m_nState = EState.Open;
            m_bTweeing = true;
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public void Close(float fDuration = 0.1f)
        {
            if (m_nState == EState.Close) return;
            m_nState = EState.Close;
            m_bTweeing = true;
            m_fTweenDelta = 0;
            m_fTweenDuration = fDuration;
        }
        //------------------------------------------------------
        public bool IsMouseIn(Vector2 mousePos)
        {
            return m_nState == EState.Open && inspectorRect.Contains(mousePos);
        }
        //------------------------------------------------------
        public void OnDraw(Rect rect)
        {
            inspectorRect.height = rect.height;
            inspectorRect.width = 300;

            if (m_nState == EState.Close || Event.current.type == EventType.ScrollWheel) return;
      //      if (Event.current.type <= EventType.ScrollWheel) return;

            if (m_pBTest == null)
            {
                m_pBTest = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                for(int x =0; x < 2; ++x)
                {
                    for(int z =0;z <2;++z)
                    {
                        m_pBTest.SetPixel(x, z, new Color(0.25f, 0.25f, 0.25f, 0.95f));
                    }
                }
                m_pBTest.Apply();
            }

            AgentTreeUtl.BeginArea(inspectorRect, m_pBTest);
            GUI.DrawTexture(new Rect(0, 0, inspectorRect.width, inspectorRect.height), m_pBTest);
            AgentTreeUtl.BeginArea(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            OnGUI(new Rect(0, 0, inspectorRect.width, inspectorRect.height));
            AgentTreeUtl.EndArea();
            AgentTreeUtl.EndArea();
        }
        //------------------------------------------------------
        public void Update(Rect rect, float fTime)
        {
            if (!m_bTweeing) return;
            if (m_nState == EState.Open)
            {
                m_fTweenDelta += fTime;
                if(m_fTweenDuration>0)
                {
                    inspectorRect.x = rect.width - inspectorRect.width + (1 - Mathf.Clamp01(m_fTweenDelta / m_fTweenDuration)) * inspectorRect.width;
                }
                if (m_fTweenDelta >= m_fTweenDuration)
                {
                    m_bTweeing = false;
                }
            }
            else if (m_nState == EState.Close)
            {
                m_fTweenDelta += fTime;
                if (m_fTweenDuration > 0)
                {
                    inspectorRect.x = rect.width - inspectorRect.width + (Mathf.Clamp01(m_fTweenDelta / m_fTweenDuration)) * inspectorRect.width;
                }
                if (m_fTweenDelta >= m_fTweenDuration)
                {
                    m_bTweeing = false;
                }
            }
        }
        //------------------------------------------------------
        private void OnGUI(Rect rc)
        {
            if (m_nState == EState.Close) return;
            if(m_pTileStyle == null)
            {
                m_pTileStyle = new GUIStyle();
                m_pTileStyle.fontSize = 16;
            }
            GUILayout.Box("结构体列表", AgentTreeEditorResources.styles.nodeHeader);
            GUILayout.BeginHorizontal();
            m_AddStructName = EditorGUILayout.TextField("结构名", m_AddStructName);
            if (GUILayout.Button("添加", new GUILayoutOption[] { GUILayout.Width(40) }))
            {
                bool bHas = false;
                for(int i =0; i < m_pLogic.StrcutDatas.Count; ++i)
                {
                    if(m_pLogic.StrcutDatas[i].structName == m_AddStructName)
                    {
                        bHas = true;
                        break;
                    }
                }
                if(bHas)
                {
                    m_pLogic.ShowNotification("结构体变量已存在", 1);
                }
                else
                {
                    m_pLogic.StrcutDatas.Add(new StructData() { runtimeVars = new List<Variable>(), structName = m_AddStructName });
                    m_AddStructName = "";
                }
            }
            GUILayout.EndHorizontal();
            m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
            for(int i =0;i < m_pLogic.StrcutDatas.Count; ++i)
            {
                StructData stData = m_pLogic.StrcutDatas[i];
                GUILayout.BeginHorizontal();
                stData.Expand = EditorGUILayout.Foldout(stData.Expand, stData.structName);
                if (GUILayout.Button("移除", new GUILayoutOption[] { GUILayout.Width(40) }))
                {
                    if(EditorUtility.DisplayDialog("提示", "确认移除该结构", "移除", "取消"))
                    {
                        m_pLogic.StrcutDatas.RemoveAt(i);
                        break;
                    }
                }
                GUILayout.EndHorizontal();
                if (stData.Expand)
                {
                    EditorGUI.indentLevel++;
                    stData.structName = EditorGUILayout.TextField(stData.structName);
                    for(int j = 0; j < stData.runtimeVars.Count; ++j)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Member[" + j + "]");
                        if (GUILayout.Button("移除", new GUILayoutOption[] { GUILayout.Width(40) }))
                        {
                            stData.runtimeVars.RemoveAt(j);
                            break;
                        }
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        if (stData.runtimeVars[j] != null)
                        {
                            stData.runtimeVars[j].SetFlag(EFlag.Locked, true);
                            stData.runtimeVars[j].strName = EditorGUILayout.TextField("变量名", stData.runtimeVars[j].strName);
                            m_DrawUIParam.strDefaultName = "值";

                            stData.runtimeVars[j].OnGUI(m_DrawUIParam);
                        }
                        EditorGUI.indentLevel--;

                    }
                    GUILayout.BeginHorizontal();
                    m_AddVariableType = (EVariableType)EditorGUILayout.EnumPopup("变量", m_AddVariableType);
                    if (GUILayout.Button("添加", new GUILayoutOption[] { GUILayout.Width(40) }))
                    {
                        VariableFactory pVarsFactor = AgentTreeManager.getInstance().GetVariableFactory();
                        m_pLogic.Editor.AdjustMaxGuid();
                        Variable variable = pVarsFactor.NewVariableByType(VariableSerializes.GetVariableType(m_AddVariableType));
                        variable.SetFlag(EFlag.Locked, true);
                        stData.runtimeVars.Add(variable);
                        m_pLogic.UpdateStrcuts();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif