#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Plugin.AT
{
    public class RefPortNode : IGraphNode
    {
        RefPort m_BindNode;
        public RefPort BindNode
        {
            get { return m_BindNode; }
            set
            {
                m_BindNode = value;
            }
        }
        Vector2 offsetSize = Vector2.zero;
        public AAgentTreeData ATData;

        public RefPortNode(AAgentTreeData asset, RefPort pNode)
        {
            if (asset == null) return;
            ATData = asset;
            BindNode = pNode;

        }
        //------------------------------------------------------
        public string ToTitleTips()
        {
            return null;
        }
        //------------------------------------------------------
        public void SetPosition(Vector2 pos)
        {
            BindNode.rect.x = pos.x;
            BindNode.rect.y = pos.y;
        }
        //------------------------------------------------------
        public Vector2 GetPosition()
        {
            return new Vector2(BindNode.rect.x, BindNode.rect.y);
        }
        //------------------------------------------------------
        public Vector2 GetLinkPosition()
        {
            return new Vector2(BindNode.rect.x + GetWidth()-8.0f, BindNode.rect.y +24 );
        }
        //------------------------------------------------------
        public float GetWidth()
        {
            return Mathf.Max(120, BindNode.rect.width + offsetSize.x);
        }
        //------------------------------------------------------
        public bool IsExpand()
        {
            return true;
        }
        //------------------------------------------------------
        public void SetExpand(bool bexpand)
        {
        }
        //------------------------------------------------------
        public float GetHeight()
        {
            return 80;
        }
        //------------------------------------------------------
        public int GetGUID()
        {
            return BindNode.id;
        }
        //------------------------------------------------------
        public Color GetTint()
        {
            return AgentTreePreferences.GetSettings().nodeRefPortColor;
        }
        //------------------------------------------------------
        public string GetDesc()
        {
            if (BindNode.GetVariable() == null)
                return "无效";
            return BindNode.GetVariable().strName;
        }
        //------------------------------------------------------
        public void OnBodyGUI()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            DrawUIParam.Current.bEdit = false;
            DrawUIParam.Current.size = new Vector2(GetWidth(), GetHeight());
            if(BindNode.GetVariable()!=null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Space(10);
                Rect rect = BindNode.GetVariable().OnGUI(DrawUIParam.Current);
                EditorGUI.EndDisabledGroup();
                PortField(rect);
            }
            EditorGUIUtility.labelWidth = labelWidth;
        }
        //------------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {

        }
        //------------------------------------------------------
        public void PortField(Rect rect)
        {
            Vector2 position = rect.position + new Vector2(rect.width,0);
            Rect rectPort = new Rect(position, new Vector2(16, 16));

            Color backgroundColor = new Color32(90, 97, 105, 255);
            Color col = AgentTreePreferences.GetTypeColor(BindNode.GetVariable().GetType());
            DrawPortHandle(rectPort, backgroundColor, col);
        }
        //------------------------------------------------------
        void DrawPortHandle(Rect rect, Color backgroundColor, Color typeColor)
        {
            Color col = GUI.color;
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, AgentTreeEditorResources.dotOuter);
            GUI.color = typeColor;
            GUI.DrawTexture(rect, AgentTreeEditorResources.dot);
            GUI.color = col;
        }
    }
}
#endif