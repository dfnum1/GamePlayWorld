#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:06:2023
类    名: 	TimelineDrawLogic
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.ED;
using UnityEditor;
using UnityEngine;

namespace ActorSystem.ED
{
    [EditorBinder(typeof(ActionEditorWindow), "ToolBarRect")]
    public class ToolBarDrawLogic : ActionEditorLogic
    {
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导入", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                string file = EditorUtility.OpenFilePanel("表现Timeline", Application.dataPath, ".skill");
            }
            GUILayout.Button("批量导出", new GUILayoutOption[] { GUILayout.Width(80) });
            if(GUILayout.Button("保存", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                GetOwner().SaveChanges();
            }
            if (GUILayout.Button("技能编辑器", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                ActionEditorWindow editor = GetOwner<ActionEditorWindow>();
                editor.OpenSkillEditor();
            }
            GUILayout.Button("文档说明", new GUILayoutOption[] { GUILayout.Width(80) });
            GUILayout.EndHorizontal();
        }
    }
}

#endif