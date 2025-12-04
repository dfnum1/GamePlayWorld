/********************************************************************
生成日期:	11:03:2023
类    名: 	CutsceneObject
作    者:	HappLI
描    述:	过场unity 存储对象
*********************************************************************/
using System;
using UnityEngine;

namespace Framework.Cutscene.Runtime
{
    public class CutsceneObject : ACutsceneObject
    {
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CutsceneObject))]
    public class CutsceneObjectEditor : ACutsceneObjectEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}