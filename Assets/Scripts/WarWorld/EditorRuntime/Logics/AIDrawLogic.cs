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
            m_pGraphView.name = "AgentTree";
            m_pGraphView.StretchToParentSize(); // 推荐
            m_pRoot.Add(m_pGraphView);
        }
        //--------------------------------------------------------
        protected override void OnDisable()
        {
            GetOwner().rootVisualElement.Remove(m_pRoot);
        }
        //--------------------------------------------------------
        public void OnGUI(Rect rect)
        {
            if (m_pGraphView == null)
                return;
     //       DrawGrid(rect, 1, m_pGraphView.viewTransform.position);
            m_pRoot.style.position = Position.Absolute;
            m_pRoot.style.left = rect.x;
            m_pRoot.style.top = rect.y;
            m_pRoot.style.width = rect.width;
            m_pRoot.style.height = rect.height;
            m_pGraphView.OnGUI(rect);
        }
    }
}
#endif