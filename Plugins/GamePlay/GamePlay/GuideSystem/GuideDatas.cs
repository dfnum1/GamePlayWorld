/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideData
作    者:	HappLI
描    述:	引导数据
*********************************************************************/
using UnityEngine;
namespace Framework.Guide
{
    [CreateAssetMenu(menuName = "GamePlay/引导数据集")]
    public class GuideDatas : AGuideDatas
    {
    }

#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(GuideDatas), true)]
    public class GuideDataEditor : AGuideDataEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}
