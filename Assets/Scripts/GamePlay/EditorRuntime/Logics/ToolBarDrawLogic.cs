/********************************************************************
生成日期:	11:07:2025
类    名: 	ToolBarDrawLogic
作    者:	HappLI
描    述:	工具栏绘制逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Framework.War.Editor
{
    [EditorBinder(typeof(WarWorldEditor), "ToolBarRect")]
    public class ToolBarDrawLogic : AEditorLogic
    {
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("创建", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
            }
            if(GUILayout.Button("保存", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                GetOwner().SaveChanges();
            }
            if (GUILayout.Button("文档说明", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                Application.OpenURL("https://docs.qq.com/doc/DTHNGdXpNaVdmT1dx");
            }
            GUILayout.EndHorizontal();
        }
    }
}

#endif