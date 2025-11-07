/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	CameraController
作    者:	HappLI
描    述:	相机控制
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.Rendering;

namespace Framework.Core
{
    public class CameraController : AModule
    {
        struct LerpFromToMode
        {
            public Vector3 tansformPos;
            public Vector3 eulerAngle;
            public float fov;
            public float time;
            public float duration;
            public void Clear()
            {
                time = 0;
                duration = 0;
            }
            public bool IsValid()
            {
                return duration>0 && time < duration;
            }
        }
        static CameraController ms_pInstnace = null;

        CameraSetting m_pSetting = null;
        Camera m_pMainCamera = null;

        Transform m_URPCameraTranfrom = null;
        private int m_CameraCullingMask;

        private Transform m_pCameaRoot = null;

        private LerpFromToMode m_LerpFromToMode = new LerpFromToMode();
        private CameraMode m_pCurrentMode = null;
        private Dictionary<string, CameraMode> m_vCameraModes;

        private Vector3 m_EffectPos = Vector3.zero;
        private Vector3 m_EffectEulerAngle = Vector3.zero;
        private Vector3 m_EffectLookAt = Vector3.zero;
        private float m_EffectFov = 0;

        Texture2D m_pCaptureScreenTexture = null;
#if UNITY_EDITOR
        EditorCameraController m_pEditor = new EditorCameraController();
        public bool IsEditorMode()
        {
            return m_pEditor.IsEditorMode();
        }
		//------------------------------------------------------
		public EditorCameraController EditorController
        {
            get { return m_pEditor; }
        }
#endif
        //------------------------------------------------------
        static Camera _GetMainCamera()
        {
            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera();

#if UNITY_EDITOR
            return Camera.main;
#else
            return null;
#endif
        }
        public static Camera MainCamera
        {
            get
            {
                return _GetMainCamera();
            }
        }
        //------------------------------------------------------
        static Vector3 _GetMainCameraPosition()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetPosition();
            }
            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera().transform.position;
#if UNITY_EDITOR
            if (Camera.main)
                return Camera.main.transform.position;
            return Vector3.zero;
#else
                return Vector3.zero;
#endif
        }
        public static Vector3 MainCameraPoition
        {
            get { return _GetMainCameraPosition(); }
        }
        //------------------------------------------------------
        static Vector3 _GetMainCameraLookAt()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetCurrentLookAt();
            }

            if (getInstance() != null)
                return getInstance().GetCurrentLookAt();
#if UNITY_EDITOR
            if (Camera.main)
            {
                Vector3 curLookAt = BaseUtil.RayHitPos(Camera.main.transform.position, Camera.main.transform.forward, 0);
                if (Vector3.Dot(Camera.main.transform.forward, curLookAt - Camera.main.transform.position) < 0)
                {
                    float dist = Vector3.Distance(curLookAt, Camera.main.transform.position);
                    curLookAt = Camera.main.transform.position + (Camera.main.transform.position - curLookAt).normalized * dist;
                }
                return curLookAt;
            }
            return Vector3.zero;
#else
                return Vector3.zero;
#endif
        }
        public static Vector3 MainCameraLookAt
        {
            get { return _GetMainCameraLookAt(); }
        }
        //------------------------------------------------------
        static Matrix4x4 _GetMainCameraCulling()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetCamera().cullingMatrix;
            }
            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera().cullingMatrix;

#if UNITY_EDITOR
            if (Camera.main)
                return Camera.main.cullingMatrix;
            return Matrix4x4.identity;
#else
                return Matrix4x4.identity;
#endif
        }
        public static Matrix4x4 MainCameraCulling
        {
            get { return _GetMainCameraCulling(); }
        }
        //------------------------------------------------------
        static Vector3 _GetMainCameraEulerAngle()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetEulerAngle();
            }

            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera().transform.eulerAngles;

#if UNITY_EDITOR
            if (Camera.main)
                return Camera.main.transform.eulerAngles;
            return Vector3.zero;
#else
                return Vector3.zero;
#endif
        }
        public static Vector3 MainCameraEulerAngle
        {
            get { return _GetMainCameraEulerAngle(); }
        }
        //------------------------------------------------------
        static Vector3 _GetMainCameraDirection()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetDir();
            }

            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera().transform.forward;

#if UNITY_EDITOR
            if (Camera.main)
                return Camera.main.transform.forward;
            return Vector3.forward;
#else
                return Vector3.forward;
#endif
        }
        public static Vector3 MainCameraDirection
        {
            get { return _GetMainCameraDirection(); }
        }
        //------------------------------------------------------
        static float _GetMainCameraFOV()
        {
            if (getInstance() != null && getInstance().IsEnable())
            {
                return getInstance().GetFov();
            }

            if (getInstance() != null && getInstance().GetCamera())
                return getInstance().GetCamera().fieldOfView;

#if UNITY_EDITOR
            if (Camera.main)
                return Camera.main.fieldOfView;
            return 45;
#else
                return 45;
#endif
        }
        public static float MainCameraFOV
        {
            get { return _GetMainCameraFOV(); }
        }
        //------------------------------------------------------
        static bool _GetMainCameraIsActived() 
        {
#if UNITY_EDITOR
            if (getInstance() != null && !(getInstance() is CameraController)) return false;
#endif
            if (getInstance() != null) return (getInstance() as CameraController).IsEnable();
            return false;
        }
        public static bool MainCameraIsActived
        {
            get { return _GetMainCameraIsActived(); }
        }
        //------------------------------------------------------
        public static CameraController getInstance()
        {
            return ms_pInstnace;
        }
        //------------------------------------------------------
        protected int m_nCloseCameraRef = 0;
        protected int m_bActiveRef = 1;
        protected int m_nActiveVolumeRef = 1;
        private bool m_bEnable = true;
        private bool m_bClosed = false;

#if USE_URP
        UnityEngine.Rendering.Universal.UniversalAdditionalCameraData m_BaseURPCameraData = null;
#endif
        public CameraController()
        {
            m_bEnable = true;
            m_bActiveRef = 1;
            m_bClosed = false;
            m_vCameraModes = new Dictionary<string, CameraMode>(4);
            m_pCurrentMode = null;
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            ms_pInstnace = this;
            m_pSetting = GetFramework().gameStartup.GetCameraSetting();
            if (m_pSetting == null)
                return;

            m_pMainCamera = m_pSetting.mainCamera;
            m_pCameaRoot = m_pMainCamera.transform;
            m_CameraCullingMask = m_pMainCamera.cullingMask;

#if USE_URP
            m_BaseURPCameraData = m_pMainCamera.GetUniversalAdditionalCameraData();
#endif

            RegisterCameraMode("hall", new HallCameraMode());
            RegisterCameraMode("free", new FreeCameraMode());
            RegisterCameraMode("battle", new BattleCameraMode());
        }
        //------------------------------------------------------
        public bool IsEnable()
        {
            return m_bEnable && m_bActiveRef>0 && !m_bClosed && m_nCloseCameraRef<=0;
        }
        //------------------------------------------------------
        public bool IsClosed()
        {
            return m_bClosed && m_bActiveRef>0 && m_nCloseCameraRef>0;
        }
        //------------------------------------------------------
        public void Enable(bool bEnable)
        {
            if (m_bEnable == bEnable) return;
            m_bEnable = bEnable;
            if (m_bEnable)
            {
                if (m_nCloseCameraRef > 0 || m_bClosed)
                    Close(false);
                m_nCloseCameraRef = 0;
                m_bActiveRef = 0;
                m_nActiveVolumeRef = 0;
                ActiveRoot(true);
            }
        }
        //------------------------------------------------------
        public void Close(bool bClose)
        {
            if (bClose) m_nCloseCameraRef++;
            else
            {
                m_nCloseCameraRef--;
                if (m_nCloseCameraRef <= 0) m_nCloseCameraRef = 0;
            }
            m_pMainCamera.cullingMask = (m_nCloseCameraRef <= 0) ? m_CameraCullingMask : 0;
        }
        //------------------------------------------------------
        public void ActiveRoot(bool bActive)
        {
            if (bActive) m_bActiveRef++;
            else
            {
                m_bActiveRef--;
                //if (m_bActiveRef < 0) m_bActiveRef = 0;
            }
            if (m_pCameaRoot)
            {
                m_pCameaRoot.gameObject.SetActive(m_bActiveRef>0);
            }
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            ms_pInstnace = null;
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            return m_pCameaRoot;
        }
#if USE_URP
        //------------------------------------------------------
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData GetURPCamera()
        {
            return m_BaseURPCameraData;
        }
#endif
        //------------------------------------------------------
        public void SetCameraClearFlag(CameraClearFlags flags, Color color)
        {
        }
        //------------------------------------------------------
        public void SetCameraNear(float fNear)
        {

        }
        //------------------------------------------------------
        public void SetCameraFar(float fFar)
        {

        }
#if USE_URP
        //------------------------------------------------------
        public UnityEngine.Rendering.Volume GetPostProcessVolume()
        {
            if (m_pSetting == null) return null;
            return m_pSetting.postProcessVolume;
        }
        //------------------------------------------------------
        public void SetPostProcess(UnityEngine.Rendering.VolumeProfile profiler)
        {
            if (m_pSetting == null) return;
            if (m_pSetting.postProcessVolume && profiler) m_pSetting.postProcessVolume.sharedProfile = profiler;
        }
        //------------------------------------------------------
        public void SetURPAsset(UnityEngine.Rendering.RenderPipelineAsset urpAsset)
        {
            if (m_pSetting == null) return;
            if (m_pSetting.mainCamera)
            {
                if(urpAsset!=null)
                {
                    QualitySettings.renderPipeline = urpAsset;
                    UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset = urpAsset;
                    if(urpAsset is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)
                    m_pSetting.mainCamera.GetUniversalAdditionalCameraData().renderShadows = ((UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)urpAsset).supportsMainLightShadows;
                }
            }
        }
        //------------------------------------------------------
        public void SetURPCamera(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camera)
        {
            m_BaseURPCameraData = camera;
        }
        //------------------------------------------------------
        public void AddCameraStack(Camera pCamera, bool bAfter = true)
        {
            if (m_BaseURPCameraData == null || pCamera == null) return;

            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData = pCamera.GetUniversalAdditionalCameraData();
            if(cameraData!=null)
            {
                if (cameraData.renderType != CameraRenderType.Overlay)
                    cameraData.renderType = CameraRenderType.Overlay;
            }

            List<Camera> vStacks = m_BaseURPCameraData.cameraStack;
            if (vStacks.Contains(pCamera)) return;
                vStacks.Add(pCamera);
        }
        //------------------------------------------------------
        public void RemoveCameraStack(Camera pCamera)
        {
            if (m_BaseURPCameraData == null || pCamera == null) return;
            m_BaseURPCameraData.cameraStack.Remove(pCamera);
        }
#endif
        //------------------------------------------------------
        public Camera GetCamera()
        {
            return m_pMainCamera;
        }
        //------------------------------------------------------
        public  void SetCameraCullingMask( int cullingMask)
        {
            if (m_pMainCamera)
                m_pMainCamera.cullingMask = cullingMask;
        }
        //------------------------------------------------------
        public void RestoreCameraCullingMask()
        {
            if (m_pMainCamera) m_pMainCamera.cullingMask = m_CameraCullingMask;
        }
        //------------------------------------------------------
        public int GetCameraCullingMask()
        {
            if (m_pMainCamera) return m_pMainCamera.cullingMask;
            return 0;
        }
        //------------------------------------------------------
        public void UpdateFov(float fFov)
        {
            if (m_pMainCamera) m_pMainCamera.fieldOfView = fFov;
        }
        //------------------------------------------------------
        public void RegisterCameraMode(string strMode, CameraMode pMode)
        {
            pMode.SetController(this);
            if (m_vCameraModes.ContainsKey(strMode)) return;
            m_vCameraModes.Add(strMode, pMode);
        }
        //------------------------------------------------------
        public CameraMode SwitchMode(string strMode, bool bEnd = true)
        {
            CameraMode pMode;
            if(m_vCameraModes.TryGetValue(strMode, out pMode))
            {
                if (m_pCurrentMode == pMode)
                {
                    return m_pCurrentMode;
                }
                if (bEnd && m_pCurrentMode != null) m_pCurrentMode.End();
                m_pCurrentMode = pMode;
                m_pCurrentMode.Start();
            }
            return m_pCurrentMode;
        }
        //------------------------------------------------------
        public void SwitchMode(CameraMode pMode, bool bEnd = true)
        {
            if (m_pCurrentMode == pMode)
            {
                return;
            }
            if (bEnd && m_pCurrentMode != null) m_pCurrentMode.End();
            m_pCurrentMode = pMode;
            m_pCurrentMode.Start();
        }
        //------------------------------------------------------
        public CameraMode GetCurrentMode()
        {
            return m_pCurrentMode;
        }
        //------------------------------------------------------
        public CameraMode GetMode(string strMode)
        {
            CameraMode pMode;
            if (m_vCameraModes.TryGetValue(strMode, out pMode))
            {
                return pMode;
            }
            return null;
        }
        //------------------------------------------------------
        public Ray ScreenPointToRay(Vector3 mousePosition)
        {
            Camera camera = GetCamera();
            if (camera == null) return new Ray();
            return camera.ScreenPointToRay(mousePosition);
        }
        //------------------------------------------------------
        public float GetFov()
        {
            if (m_pMainCamera == null) return 45;
            return m_pMainCamera.fieldOfView;
        }
        //------------------------------------------------------
        public Vector3 GetDir()
        {
            if (m_pCameaRoot == null)
                return Vector3.forward;
            return m_pCameaRoot.forward;
        }
        //------------------------------------------------------
        public Vector3 GetRight()
        {
            if (m_pCameaRoot == null)
                return Vector3.right;
            return m_pCameaRoot.right;
        }
        //------------------------------------------------------
        public Vector3 GetUp()
        {
            if (m_pCameaRoot == null)
                return Vector3.up;
            return m_pCameaRoot.up;
        }
        //------------------------------------------------------
        public Vector3 GetPosition()
        {
            if (m_pCameaRoot == null)
                return Vector3.zero;
            return m_pCameaRoot.position;
        }
        //------------------------------------------------------
        public Vector3 GetEulerAngle()
        {
            if (m_pCameaRoot == null)
                return Vector3.zero;
            return m_pCameaRoot.eulerAngles;
        }
        //------------------------------------------------------
        public Vector3 GetCurrentLookAt()
        {
            if (m_pCurrentMode == null) return Vector3.zero;
            return m_pCurrentMode.GetCurrentLookAt();
        }
        //------------------------------------------------------
        public Vector3 GetCurrentFollowLookAt()
        {
            if (m_pCurrentMode == null) return Vector3.zero;
            return m_pCurrentMode.GetFollowLookAtPosition();
        }
        //------------------------------------------------------
        public void StopAllEffect()
        {
        }
        //------------------------------------------------------
        public bool IsTweenEffecting(float fFactorError =0.1f)
        {
            if(m_bEnable && m_pCurrentMode!=null)
            {
                if (m_pCurrentMode.IsLockOffsetDistanceLerping(fFactorError)) return true;
                if (!m_pCurrentMode.GetLockFovOffsetLerp().IsArrived(fFactorError)) return true;
                if (!m_pCurrentMode.GetLockEulerAngleOffsetLerp().IsArrived(-1,fFactorError)) return true;
            }
            return false;
        }
        //-------------------------------------------
        public bool IsInView(Vector3 pos, float factor = 0.1f)
        {
#if UNITY_EDITOR
            if (IsEditorMode()) return false;
#endif
            Camera mainCam = MainCamera;
            if (mainCam == null) return false;
            Vector2 viewPos = mainCam.WorldToViewportPoint(pos);
            Vector3 dir = (pos - GetPosition()).normalized;
            float dot = Vector3.Dot(GetDir(), dir);
            if (dot > 0 && viewPos.x >= -factor && viewPos.x <= 1 + factor && viewPos.y >= -factor && viewPos.y <= 1 + factor)
                return true;
            return false;
        }
        //-------------------------------------------
        public bool IsInView(Vector3 pos, Rect viewRc)
        {
#if UNITY_EDITOR
            if (IsEditorMode()) return false;
#endif
            Camera mainCam = MainCamera;
            if (mainCam == null) return false;
            Vector2 viewPos = mainCam.WorldToViewportPoint(pos);
            Vector3 dir = (pos - GetPosition()).normalized;
            float dot = Vector3.Dot(GetDir(), dir);
            if (dot > 0 && viewPos.x >= viewRc.xMin && viewPos.x <= viewRc.xMax && viewPos.y >= viewRc.yMin && viewPos.y <= viewRc.yMax)
                return true;
            return false;
        }
        //------------------------------------------------------
        public void SetLerpFormToMode(Vector3 transPosition, Vector3 eulerAngle, float fov, float fTime)
        {
            m_LerpFromToMode.duration = fTime;
            m_LerpFromToMode.time = 0;
            m_LerpFromToMode.fov = fov;
            m_LerpFromToMode.tansformPos = transPosition;
            m_LerpFromToMode.eulerAngle = eulerAngle;
            ForceUpdate(0);
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            base.OnUpdate(fFrame);
            ForceUpdate(fFrame);
        }
        //------------------------------------------------------
        public void ForceUpdate(float fFrame)
        {
            fFrame = Time.deltaTime;
            if (m_pCameaRoot == null) return;
            m_EffectPos = Vector3.zero;
            m_EffectEulerAngle = Vector3.zero;
            m_EffectLookAt = Vector3.zero;
            m_EffectFov = 0;

            bool hasDirectEffect = false;
            if(hasDirectEffect)
            {
                m_pCameaRoot.position = m_EffectPos;
                m_pCameaRoot.eulerAngles = m_EffectEulerAngle;
                UpdateFov(m_EffectFov);

                if (m_URPCameraTranfrom)
                {
                    m_URPCameraTranfrom.position = m_pCameaRoot.position;
                    m_URPCameraTranfrom.eulerAngles = m_pCameaRoot.eulerAngles;
                }
                return;
            }
#if UNITY_EDITOR
            if (m_pEditor.IsEditorMode())
            {
                m_pEditor.Update();
                return;
            }
#endif

            if (IsEnable())
            {
                if(m_pCurrentMode!=null)
                {
             //       Framework.Core.CommonUtility.AjustUpdatePostitionFrame(ref fFrame);
                    m_pCurrentMode.Update(fFrame);
                    if(m_LerpFromToMode.IsValid())
                    {
                        m_LerpFromToMode.time += fFrame;
                        float fFactor = Mathf.Clamp01(m_LerpFromToMode.time / m_LerpFromToMode.duration);
                        m_pCameaRoot.position = Vector3.Lerp(m_LerpFromToMode.tansformPos, m_pCurrentMode.GetCurrentTrans() + m_EffectPos, fFactor) ;
                        m_pCameaRoot.rotation = Quaternion.Slerp(Quaternion.Euler(m_LerpFromToMode.eulerAngle), Quaternion.Euler(m_pCurrentMode.GetCurrentEulerAngle() + m_EffectEulerAngle), fFactor);
                        UpdateFov(Mathf.Lerp(m_LerpFromToMode.fov, m_pCurrentMode.GetCurrentFov() + m_EffectFov, fFactor));
                    }
                    else
                    {
                        m_pCameaRoot.position = m_pCurrentMode.GetCurrentTrans() + m_EffectPos;
                        m_pCameaRoot.eulerAngles = m_pCurrentMode.GetCurrentEulerAngle() + m_EffectEulerAngle;
                        UpdateFov(m_pCurrentMode.GetCurrentFov() + m_EffectFov);

                    }

                    if (m_URPCameraTranfrom)
                    {
                        m_URPCameraTranfrom.position = m_pCameaRoot.position;
                        m_URPCameraTranfrom.eulerAngles = m_pCameaRoot.eulerAngles;
                    }
                }
            }
        }
        //------------------------------------------------------
        public void OnEffectPosition(Vector3 pos)
        {
            m_EffectPos += pos;
        }
        //------------------------------------------------------
        public void OnEffectEulerAngle(Vector3 pos)
        {
            m_EffectEulerAngle += pos;
        }
        //------------------------------------------------------
        public void OnEffectLookAt(Vector3 pos)
        {
            m_EffectLookAt += pos;
        }
        //------------------------------------------------------
        public void OnEffectFov(float fov)
        {
            m_EffectFov += fov;
        }
        //------------------------------------------------------
        public void OnFollowPosition(Vector3 pos)
        {

        }
        //-------------------------------------------
        public Texture CaptureScreenTexture(int sw = -1, int sh = -1, RenderTextureFormat format = RenderTextureFormat.ARGB32)
        {
            if (sw < 0) sw = Screen.width;
            if (sh < 0) sh = Screen.height;
            if (!SystemInfo.SupportsRenderTextureFormat(format))
                format = RenderTextureFormat.ARGB32;
            RenderTexture RT = RenderTexture.GetTemporary(sw, sh, 24, format);
            if (m_pCaptureScreenTexture != null && (m_pCaptureScreenTexture.width != sw || m_pCaptureScreenTexture.height != sh))
            {
                UnityEngine.Object.Destroy(m_pCaptureScreenTexture);
                m_pCaptureScreenTexture = null;
            }
            if (m_pCaptureScreenTexture == null)
            {
                m_pCaptureScreenTexture = new Texture2D(sw, sh, TextureFormat.ARGB32, false);
            }

            RenderTexture rt1 = m_pMainCamera.targetTexture;

            m_pMainCamera.targetTexture = RT;
            m_pMainCamera.Render();
            m_pMainCamera.targetTexture = rt1;

            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = RT;
            m_pCaptureScreenTexture.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
            m_pCaptureScreenTexture.Apply();
            RenderTexture.active = rt;

            RenderTexture.ReleaseTemporary(RT);

            return m_pCaptureScreenTexture;
        }
    }
}