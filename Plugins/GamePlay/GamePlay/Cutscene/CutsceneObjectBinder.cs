/********************************************************************
生成日期:	06:30:2025
类    名: 	CutsceneObjectBinder
作    者:	HappLI
描    述:	cutscene对象绑定器
*********************************************************************/
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Framework.Cutscene.Runtime
{
    //-----------------------------------------------------
    [ExecuteAlways,ExecuteInEditMode]
    public class CutsceneObjectBinder : ACutsceneObjectBinder
    {
    }
    //-----------------------------------------------------
#if UNITY_EDITOR
    [CustomEditor(typeof(CutsceneObjectBinder))]
    public class CutsceneObjectBinderEditor : ACutsceneObjectBinderEditor
    {
        //-----------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}