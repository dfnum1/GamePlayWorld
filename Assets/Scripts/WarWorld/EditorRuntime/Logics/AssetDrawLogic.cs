/********************************************************************
生成日期:	11:07:2025
类    名: 	AssetDrawLogic
作    者:	HappLI
描    述:	资源面板逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.War.Editor
{
    [EditorBinder(typeof(WarWorldEditor), "AssetRect", -1)]
    public class AssetDrawLogic : AWarWorldLogic
    {
        //--------------------------------------------------------
        protected override void OnEnable()
        {
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            Rect rect = GetRect();
            GUILayout.BeginArea(new Rect(rect.x, rect.y+20, rect.width, rect.height));
            GUILayout.EndArea();
            UIDrawUtils.DrawColorLine(new Vector2(rect.xMin, rect.y+20 ), new Vector2(rect.xMax, rect.y + 20), new Color(1, 1, 1, 0.5f));
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width,20));
            GUILayout.Label("数据面板");
            GUILayout.EndArea();
        }
    }
}

#endif