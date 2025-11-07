/********************************************************************
生成日期:   1:11:2020 10:09
类    名: 	CameraStack
作    者:	HappLI
描    述:	相机栈
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    [RequireComponent(typeof(Camera))]
    public class CameraStack : MonoBehaviour
    {
        public Camera camera;
#if USE_URP
        //----------------------------------------------------------------------------------
        private void OnEnable()
        {
            CameraUtil.AddCameraStack(camera, true);
        }
        //----------------------------------------------------------------------------------
        private void OnDisable()
        {
            CameraUtil.RemoveCameraStack(camera);
        }
#endif
    }
    //----------------------------------------------------------------------------------
#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(CameraStack))]
    public class CameraStackEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CameraStack cameraStack = (CameraStack)target;
            cameraStack.camera = (Camera)UnityEditor.EditorGUILayout.ObjectField("Camera", cameraStack.camera, typeof(Camera), true);
            if (cameraStack.camera == null)
            {
                cameraStack.camera = cameraStack.GetComponent<Camera>();
            }
            UnityEditor.EditorUtility.SetDirty(cameraStack);
        }
    }
#endif
}