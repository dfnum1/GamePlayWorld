/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorComponent
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.ActorSystem.Runtime
{
    public class ActorComponent : AActorComponent
    {
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ActorComponent))]
    class ActorComponentEditor : AActorComponentEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}