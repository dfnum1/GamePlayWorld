/********************************************************************
生成日期:	06:30:2025
类    名: 	AIDrawLogic
作    者:	HappLI
描    述:	AI控制面板
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.War.Editor
{
    [EditorBinder(typeof(WarWorldEditor), "TimelineRect", 100)]
    public class AIDrawLogic : AWarWorldLogic
    {
        VisualElement m_pRoot;
        AILogicDrawView m_pGraphView;
        //--------------------------------------------------------
        protected override void OnEnable()
        {
            m_pRoot = new VisualElement();
            GetOwner().rootVisualElement.Add(m_pRoot);

            m_pGraphView = new AILogicDrawView(this);
            m_pGraphView.name = "AIDrawer";
            m_pGraphView.StretchToParentSize(); // 推荐
            m_pRoot.Add(m_pGraphView);
        }
        //--------------------------------------------------------
        protected override void OnDisable()
        {
            GetOwner().rootVisualElement.Remove(m_pRoot);
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            if (m_pGraphView == null)
                return;
            Rect rect = GetRect();

            Rect view = new Rect(rect.x, rect.y+5, rect.width, rect.height-6);
            DrawGrid(view, 1, m_pGraphView.viewTransform.position);

            m_pRoot.style.position = Position.Absolute;
            m_pRoot.style.left = view.x;
            m_pRoot.style.top = view.y;
            m_pRoot.style.width = view.width;
            m_pRoot.style.height = view.height;
            m_pGraphView.OnGUI(rect);
        }
        //------------------------------------------------------
        public void DrawGrid(Rect rect, float zoom, Vector2 panOffset)
        {
            Vector2 center = rect.size / 2f;
            Texture2D gridTex = EditorPreferences.GetSettings().gridTexture;
            Texture2D crossTex = EditorPreferences.GetSettings().crossTexture;

            // Offset from origin in tile units
            float xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            float yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            Vector2 tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            float tileAmountX = Mathf.Round(rect.size.x * zoom) / gridTex.width;
            float tileAmountY = Mathf.Round(rect.size.y * zoom) / gridTex.height;

            Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);


            Color color = GUI.color;

            // Draw tiled background
            GUI.color = Color.white;
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));

            // Draw tiled background
            GUI.color = Color.white;
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
            GUI.color = color;
        }
    }
}
#endif