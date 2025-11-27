/********************************************************************
生成日期:	11:07:2025
类    名: 	InspectorDrawLogic
作    者:	HappLI
描    述:	数据面板逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.War.Editor
{
    [EditorBinder(typeof(WarWorldEditor), "InspectorRect")]
    public class InspectorDrawLogic : AWarWorldLogic
    {
        Vector2 m_Scoller;
        //--------------------------------------------------------
        protected override void OnEnable()
        {
        }
        //--------------------------------------------------------
        protected override void OnDisable()
        {
        }
        //--------------------------------------------------------
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
        }
        //--------------------------------------------------------
        public override void OnSceneView(SceneView sceneView)
        {
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            var window = GetOwner<WarWorldEditor>();
            Rect rect = GetRect();
            GUILayout.BeginArea(new Rect(rect.x, rect.y + 20, rect.width, rect.height - 20));

            m_Scoller = GUILayout.BeginScrollView(m_Scoller);
            
            GUILayout.EndScrollView();

            //    HandleUtilityWrapper.DrawProperty(m_Test);
            GUILayout.EndArea();
            UIDrawUtils.DrawColorLine(new Vector2(rect.xMin, rect.y + 20), new Vector2(rect.xMax, rect.y + 20), new Color(1,1,1,0.5f));
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, 20));
            GUILayout.Label("属性面板", WarEditorUtil.panelTitleStyle);
            GUILayout.EndArea();
        }
    }
}

#endif