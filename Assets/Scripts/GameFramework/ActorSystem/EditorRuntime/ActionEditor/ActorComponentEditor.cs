#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:03:2023
类    名: 	ActionEditorWindow
作    者:	HappLI
描    述:	表现图编辑窗口
*********************************************************************/
using Framework.Core;
using Framework.Cutscene.Runtime;
using Framework.ED;
using UnityEditor;
using UnityEngine;

namespace ActorSystem.ED
{
    [CustomEditor(typeof(ActorComponent))]
    //[CanEditMultipleObjects]
    class ActorComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("打开编辑器"))
            {
                ActionEditorWindow.OpenTarget(target as ActorComponent);
            }
            if(Application.isPlaying)
            {
                if (GUILayout.Button("动画调试"))
                    GraphPlayableUtil.DebugPlayable((target as ActorComponent).gameObject);
            }
        }
        //-----------------------------------------------------
        [MenuItem("Assets/打开动作编辑器", true)]
        private static bool ValidatePlayCutscene()
        {
            if (Selection.activeGameObject == null)
                return false;
            return Selection.activeGameObject.GetComponent<ActorComponent>() != null;
        }
        //-----------------------------------------------------

        [MenuItem("Assets/打开动作编辑器", false, 0)]
        private static void PlayCutscene()
        {
            var obj = Selection.activeGameObject ;
            if (obj != null)
            {
                ActionEditorWindow.OpenTarget(Selection.activeGameObject.GetComponent<ActorComponent>());
            }
        }
    }
}

#endif