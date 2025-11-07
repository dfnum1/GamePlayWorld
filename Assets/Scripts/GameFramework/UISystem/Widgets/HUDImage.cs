/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	HUDImage
作    者:	Happli
描    述:	3d 模型RT 
*********************************************************************/

using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport,]
    public class HUDImage : RawImage
    {
        public string playAction = "idle";
        public Vector3 ModelPos;
        public Vector3 ModelScale = Vector3.one;
        public Vector3 ModelRotate;

        public bool hasTransparentShadow = true;

        public Vector3  CameraPos;
        public Vector3  CameraRotate;
        public float    CameraFOV = 45;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HUDImage), true)]
    [CanEditMultipleObjects]
    public class HUDImageEditor : UnityEditor.UI.RawImageEditor
    {
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            HUDImage image = target as HUDImage;
            image.playAction = EditorGUILayout.TextField("动作名", image.playAction);
            image.hasTransparentShadow = EditorGUILayout.Toggle("阴影", image.hasTransparentShadow);
            image.ModelPos = EditorGUILayout.Vector3Field("模型位置", image.ModelPos);
            image.ModelRotate = EditorGUILayout.Vector3Field("模型角度", image.ModelRotate);
            image.ModelScale = EditorGUILayout.Vector3Field("模型缩放", image.ModelScale);
            image.CameraPos = EditorGUILayout.Vector3Field("相机位置", image.CameraPos);
            image.CameraRotate = EditorGUILayout.Vector3Field("相机角度", image.CameraRotate);
            image.CameraFOV = EditorGUILayout.Slider("相机FOV", image.CameraFOV, 20, 160);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
#endif
}