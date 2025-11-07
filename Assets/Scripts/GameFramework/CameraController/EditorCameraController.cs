/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	EditorCameraController
作    者:	HappLI
描    述:	相机控制
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    public class EditorCameraController
    {
        private bool m_bEditorMode = false;

        private bool    m_bRestoreCamera = false;
        private Vector3 m_RestoreCameraPos = Vector3.zero;
        private Vector3 m_RestoreCameraEulerAngle = Vector3.zero;
        private float   m_RestoreCameraFov = 0.0f;

        float m_MoveSpeed = 10f;
        float m_FastMoveMultiplier = 5f;
        float m_RotationSpeed = 1f;
        float m_PanSpeed = 0.5f;
        float m_ZoomSpeed = 10f;

        private Vector3 m_ToPosition =  Vector3.zero;
        private Vector3 m_LastMousePos;
        private bool m_IsRightMouseDown = false;
        private bool m_IsMiddleMouseDown = false;

        private CameraSetting m_CameraSetting;
        //-----------------------------------------------------
        public void AutoEditor()
        {
            SetEditorMode(!m_bEditorMode);
        }
		//-----------------------------------------------------
		public bool IsEditorMode()
		{
			return m_bEditorMode;
		}
        //-----------------------------------------------------
        public Camera GetMainCamera()
        {
            if(m_CameraSetting == null)
            m_CameraSetting = GameObject.FindAnyObjectByType<CameraSetting>();
            if (m_CameraSetting == null)
                return null;
            return m_CameraSetting.mainCamera;
        }
        //-----------------------------------------------------
        public void SetEditorMode(bool bEditorMode)
        {
            Camera cam = GetMainCamera();
            if (cam == null)
            {
                EditorUtility.DisplayDialog("提示", "没有主相机,无法进入相机编辑模式", "好的");
                return;
            }
            if (m_bEditorMode == bEditorMode)
                return;

            m_bEditorMode = bEditorMode;
            if(m_bEditorMode)
            {
                m_bRestoreCamera = true;
                m_RestoreCameraPos = cam.transform.position;
                m_RestoreCameraEulerAngle = cam.transform.eulerAngles;
                m_RestoreCameraFov = cam.fieldOfView;

                m_ToPosition = m_RestoreCameraPos;
            }
            else
            {
                if(m_bRestoreCamera)
                {
                    m_bRestoreCamera = false;
                    if(cam)
                    {
                        cam.transform.position = m_RestoreCameraPos;
                        cam.transform.eulerAngles = m_RestoreCameraEulerAngle;
                        cam.fieldOfView = m_RestoreCameraFov;
                    }
                }
            }
        }
        //-----------------------------------------------------
        public void Update()
        {
            if (!m_bEditorMode) return;
            Camera cam = GetMainCamera();
            if (cam == null) return;
            // 右键拖动旋转
            if (Input.GetMouseButtonDown(1))
            {
                m_IsRightMouseDown = true;
                m_LastMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1))
            {
                m_IsRightMouseDown = false;
            }
            if (m_IsRightMouseDown)
            {
                Vector3 delta = Input.mousePosition - m_LastMousePos;
                float yaw = delta.x * m_RotationSpeed * 0.2f;
                float pitch = -delta.y * m_RotationSpeed * 0.2f;
                Vector3 angles = cam.transform.eulerAngles;
                angles.x += pitch;
                angles.y += yaw;
                cam.transform.eulerAngles = angles;
                m_LastMousePos = Input.mousePosition;
            }

            // 中键拖动平移
            if (Input.GetMouseButtonDown(2))
            {
                m_IsMiddleMouseDown = true;
                m_LastMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(2))
            {
                m_IsMiddleMouseDown = false;
            }
            if (m_IsMiddleMouseDown)
            {
                Vector3 delta = Input.mousePosition - m_LastMousePos;
                Vector3 right = cam.transform.right;
                Vector3 up = cam.transform.up;
                Vector3 move = (-right * delta.x + -up * delta.y) * m_PanSpeed;
                m_ToPosition += move;
                m_LastMousePos = Input.mousePosition;
            }

            // 滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                m_ToPosition += cam.transform.forward * scroll * m_ZoomSpeed*2.0f;
            }

            // WASD QE移动
            if (m_IsRightMouseDown || m_IsMiddleMouseDown || Input.GetMouseButton(0)) // 仅在右键按下时响应
            {
                Vector3 move = Vector3.zero;
                float speed = m_MoveSpeed;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    speed *= m_FastMoveMultiplier;

                if (Input.GetKey(KeyCode.W)) move += cam.transform.forward;
                if (Input.GetKey(KeyCode.S)) move -= cam.transform.forward;
                if (Input.GetKey(KeyCode.A)) move -= cam.transform.right;
                if (Input.GetKey(KeyCode.D)) move += cam.transform.right;
                if (Input.GetKey(KeyCode.Q)) move -= cam.transform.up;
                if (Input.GetKey(KeyCode.E)) move += cam.transform.up;

                if (move.sqrMagnitude > 0.0001f)
                {
                    move.Normalize();
                    m_ToPosition += move * speed * Time.unscaledDeltaTime;
                }
            }
            cam.transform.position =  m_ToPosition;
        }
        //-----------------------------------------------------
        public void DrawSetting()
        {
            m_MoveSpeed = EditorGUILayout.Slider("移动速度", m_MoveSpeed, 10, 100);
            m_FastMoveMultiplier = EditorGUILayout.Slider("快速移速比率", m_FastMoveMultiplier, 1, 10);
            m_RotationSpeed = EditorGUILayout.Slider("旋转速度", m_RotationSpeed, 0.1f, 10);
            m_PanSpeed = EditorGUILayout.Slider("中间拖拽速度", m_PanSpeed, 0.1f, 5.0f);
            m_ZoomSpeed = EditorGUILayout.Slider("鼠标滚动速度", m_ZoomSpeed, 0.1f, 100.0f);
            Camera cam = CameraUtil.mainCamera;
            if (cam)
                cam.fieldOfView = EditorGUILayout.Slider("FOV", cam.fieldOfView, 0, 160.0f);
        }
        //-----------------------------------------------------
        [MenuItem("Tools/游戏相机编辑模式  _F2")]
        public static void EditorModeToggle()
        {
            if (CameraUtil.cameraController == null)
            {
                EditorUtil.ShowGameViewNotification("无法进入相机编辑器模式");
                return;
            }
            if (CameraUtil.cameraController.IsEditorMode()) EditorUtil.ShowGameViewNotification("退出相机编辑器模式");
            else EditorUtil.ShowGameViewNotification("进入相机编辑器模式");
            CameraUtil.cameraController.EditorController.AutoEditor();
            var editorCamera = GameObject.FindAnyObjectByType<CameraSetting>(FindObjectsInactive.Include);
            if(editorCamera) Selection.activeGameObject = editorCamera.gameObject;
        }
    }
}
#endif