/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	CameraUtil
作    者:	HappLI
描    述:	相机工具
*********************************************************************/
using Framework.Plugin.AT;
using UnityEngine;
namespace Framework.Core
{
    [ATExportMono("相机工具")]
    public static class CameraUtil
    {
        //------------------------------------------------------
        public static CameraController cameraController
        {
            get
            {
                if (AFramework.mainFramework == null) return null;
                return AFramework.mainFramework.cameraController;
            }
        }
        //------------------------------------------------------
        [ATField]
        public static Camera mainCamera
        {
            get
            {
                if (cameraController != null) return cameraController.GetCamera();
                return Camera.main;
            }
        }
        //------------------------------------------------------
        [ATField]
        public static Vector3 mainCameraPosition
        {
            get
            {
                if (cameraController != null) return cameraController.GetPosition();
                return Camera.main.transform.position;
            }
        }
        //------------------------------------------------------
        [ATField]
        public static Vector3 mainCameraEulerAngle
        {
            get
            {
                if (cameraController != null) return cameraController.GetEulerAngle();
                return Camera.main.transform.eulerAngles;
            }
        }
        //------------------------------------------------------
        [ATField]
        public static Vector3 mainCameraDirection
        {
            get
            {
                if (cameraController != null) return cameraController.GetDir();
                return Camera.main.transform.forward;
            }
        }
        //------------------------------------------------------
        [ATField]
        public static Transform cameraSystemRoot
        {
            get
            {
                if (cameraController != null) return cameraController.GetTransform();
                return Camera.main.transform;
            }
        }
        //------------------------------------------------------
        public static bool IsTweenEffecting(float fFactor)
        {
            return false;
        }
        //------------------------------------------------------
        public static void ForceUpdate()
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.ForceUpdate(0);
        }
        //------------------------------------------------------
        [ATMethod]
        public static bool IsInView(Vector3 position, float factor = 0.1f)
        {
            if (cameraController != null) return cameraController.IsInView(position, factor);
            return false;
        }
        //------------------------------------------------------
        [ATMethod]
        public static CameraMode GetCurrentMode()
        {
            if (cameraController == null) return null;
            return cameraController.GetCurrentMode();
        }
        //------------------------------------------------------
        [ATMethod]
        public static void Enable(bool bEnable)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.Enable(bEnable);
        }
        //------------------------------------------------------
        [ATMethod]
        public static void ActiveRoot(bool bActive)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.ActiveRoot(bActive);
        }
        //------------------------------------------------------
        [ATMethod]
        public static CameraMode SwitchMode(string mode, bool bEnd = true)
        {
            var ctl = cameraController;
            if (ctl == null)
                return null;
            return ctl.SwitchMode(mode, bEnd);
        }
        //------------------------------------------------------
        [ATMethod]
        public static void CloseCameraRef(bool bClose)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.Close(bClose);
        }
        //------------------------------------------------------
        public static bool ScreenPointToRay(Vector3 screenPos, out Ray ray)
        {
            ray = new Ray(Vector3.zero, Vector3.zero);
            if (cameraController != null)
            {
                ray = cameraController.ScreenPointToRay(screenPos);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool ScreenPointToRay(Vector2 screenPos, out Ray ray)
        {
            ray = new Ray(Vector3.zero, Vector3.zero);
            if (cameraController != null)
            {
                ray = cameraController.ScreenPointToRay(screenPos);
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        [ATMethod]
        static public Vector3 ScreenToWorldPos(Vector3 screen)
        {
            Camera camera = mainCamera;
            if (camera == null) camera = Camera.main;
            if (camera == null) return Vector3.zero;
            return camera.ScreenToWorldPoint(screen);
        }
        //------------------------------------------------------
        [ATMethod("Screen2DToWorldPos")]
        static public Vector3 ScreenToWorldPos(Vector2 screen)
        {
            Camera camera = mainCamera;
            if (camera == null) camera = Camera.main;
            if (camera == null) return Vector3.zero;
            return camera.ScreenToWorldPoint(screen);
        }
        //------------------------------------------------------
        [ATMethod]
        static public Vector3 ScreenRayWorldPos(Vector3 screen, float floorY = 0)
        {
            Camera camera = mainCamera;
            if (camera == null) camera = Camera.main;
            if (camera == null) return Vector3.zero;
            return BaseUtil.RayHitPos(camera.ScreenPointToRay(screen), floorY);
        }
        //------------------------------------------------------
        [ATMethod("Screen2DRayWorldPos")]
        static public Vector3 ScreenRayWorldPos(Vector2 screen, float floorY = 0)
        {
            Camera camera = mainCamera;
            if (camera == null) camera = Camera.main;
            if (camera == null) return Vector3.zero;
            return BaseUtil.RayHitPos(camera.ScreenPointToRay(screen), floorY);
        }
#if USE_URP
        //------------------------------------------------------
        [ATMethod]
        public static void AddCameraStack(Camera camera, bool bAfter = true)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.AddCameraStack(camera, bAfter);
        }
        //------------------------------------------------------
        [ATMethod]
        public static void RemoveCameraStack(Camera camera)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.RemoveCameraStack(camera);
        }
        //------------------------------------------------------
        public static void ActiveVolume(bool bActive)
        {

        }
        //------------------------------------------------------
        public static void SetPostProcess(UnityEngine.Rendering.VolumeProfile volume)
        {
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.SetPostProcess(volume);
        }
        //------------------------------------------------------
        public static void SetURPAsset(UnityEngine.Rendering.RenderPipelineAsset urpAsset)
        {
            if (urpAsset == null)
                return;
            var ctl = cameraController;
            if (ctl == null)
                return;
            ctl.SetURPAsset(urpAsset);
        }
#endif
    }
}
