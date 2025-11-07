/********************************************************************
生成日期:	2020-06-23
类    名: 	RenderHudManager
作    者:	happli
描    述:	RenderTexture管理器
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Data;
using UnityEngine;
#if USE_URP
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
#endif
namespace Framework.Core
{
    public interface IHudRenderCallback
    {
        void OnDestroy();
    }
    public class RenderHudManager : Base.Singleton<RenderHudManager>
    {
        struct DelayDestroyHud
        {
            public float fTime;
            public HudCamera cameraHud;
        }
        struct HudRender
        {
            public HudCamera camera;
            public IUserData binderData;

            public void Destroy()
            {
                if (camera == null) return;
                camera.Clear();
                BaseUtil.Desytroy(camera.gameObject);
            }
            public static HudRender EMPTY = new HudRender() { camera = null, binderData = null };
        }
        static int MAX_POOL_CNT = 2;
        /// <summary>
        /// 渲染的Layer  1 << 12   UI_3D
        /// </summary>

        Vector3 m_OffsetPos = Vector3.zero;

        List<HudRender> m_RenderHuds = null;
        List<DelayDestroyHud> m_vDelayDestroyHuds = null;
        Stack<HudCamera> m_PoolRenders = new Stack<HudCamera>(MAX_POOL_CNT);
        //-----------------------------------------------------
        public RenderHudManager()
        {
            Awake();
        }
        //-----------------------------------------------------
        public void Awake()
        {
            m_OffsetPos = new Vector3(1000,1000,1000);
            m_RenderHuds = null;
        }
        //-----------------------------------------------------
        public void Shutdown()
        {
            if(m_RenderHuds != null)
            {
                for (int i = 0; i < m_RenderHuds.Count; ++i)
                {
                    m_RenderHuds[i].Destroy();
                }
                m_RenderHuds.Clear();
            }

            foreach(HudCamera var in m_PoolRenders)
            {
                BaseUtil.Desytroy(var.gameObject);
            }
            m_PoolRenders.Clear();
        }
        //-----------------------------------------------------
        public void Update(float fTime)
        {
            if (m_vDelayDestroyHuds == null) return;
            float time = Time.time;
            DelayDestroyHud delayHud;
            for (int i = 0; i < m_vDelayDestroyHuds.Count;)
            {
                delayHud = m_vDelayDestroyHuds[i];
                if(time >= delayHud.fTime)
                {
                    m_vDelayDestroyHuds.RemoveAt(i);
                    Destroy(delayHud.cameraHud,0);
                    continue;
                }
                ++i;
            }
        }
        //-----------------------------------------------------
        public void Destroy(HudCamera renderHud, float fDelay =0)
        {
            if (renderHud == null)
            {
                return;
            }
            if(fDelay >0)
            {
                if(m_vDelayDestroyHuds == null)
                {
                    m_vDelayDestroyHuds = new List<DelayDestroyHud>(MAX_POOL_CNT);
                }
                else
                {
                    DelayDestroyHud delayHud;
                    for(int i = 0; i< m_vDelayDestroyHuds.Count; ++i)
                    {
                        delayHud =   m_vDelayDestroyHuds[i];
                        if(delayHud.cameraHud == renderHud)
                        {
                            delayHud.fTime = Time.time + fDelay;
                            m_vDelayDestroyHuds[i] = delayHud;
                            return;
                        }
                    }
                }
                DelayDestroyHud newDelayHud = new DelayDestroyHud();
                newDelayHud.fTime = Time.time + fDelay;
                newDelayHud.cameraHud = renderHud;
                m_vDelayDestroyHuds.Add(newDelayHud);
                return;
            }
            if (m_RenderHuds != null)
            {
                for (int i = 0; i < m_RenderHuds.Count;)
                {
                    if (m_RenderHuds[i].camera == renderHud)
                    {
                        m_RenderHuds.RemoveAt(i);
                    }
                    else
                        ++i;
                }
            }
            RecycleHudCamera(renderHud);
        }
        //-----------------------------------------------------
        public void OnClearCallback(IUserData userData)
        {
            if (m_RenderHuds == null) return;
            HudRender hudRender;
            for(int i = 0; i < m_RenderHuds.Count;)
            {
                hudRender = m_RenderHuds[i];
                if (hudRender.binderData == userData)
                {
                    m_RenderHuds.RemoveAt(i);
                    RecycleHudCamera(hudRender.camera);
                }
                else
                    ++i;
            }
        }
        //------------------------------------------------------
        void RecycleHudCamera(HudCamera renderHud)
        {
            if (renderHud == null) return;

            renderHud.Clear();
            if (m_PoolRenders.Contains(renderHud))
                return;
            if (m_PoolRenders.Count < MAX_POOL_CNT)
            {
                renderHud.enabled = false;
                m_PoolRenders.Push(renderHud);
            }
            else
            {
                GameObject.Destroy(renderHud.gameObject);
            }
        }
        //------------------------------------------------------
        public static bool IsHUDLowerModel()
        {
       //     //! 临时处理
       //     if (SystemInfo.deviceName.Contains("HUAWEI") || SystemInfo.deviceModel.Contains("HUAWEI"))
       //         return true;
            return false;
        }
        //------------------------------------------------------
        public HudCamera CreateRenderHud(IUserData bindUser, UnityEngine.UI.RawImage rawImage, Vector3 vPos, int width = 256, int height = 256, float fov = 60)
        {
            HudCamera hudCamera = null;
            if (m_PoolRenders.Count>0)
            {
                hudCamera = m_PoolRenders.Pop();
                hudCamera.enabled =true;
            }
            if(hudCamera == null)
            {
                if (m_RenderHuds == null) m_RenderHuds = new List<HudRender>(2);
                GameObject cameraRenderGameObject = new GameObject("HudCamera_" + m_RenderHuds.Count);
                Transform hudTrans = cameraRenderGameObject.transform;

                hudTrans.SetParent(CameraUtil.cameraSystemRoot);

                //GameObject.DontDestroyOnLoad(cameraRenderGameObject);

                hudCamera = cameraRenderGameObject.AddComponent<HudCamera>();
            }
            if (width > 1024) width = 1024;
            if (height > 1024) height = 1024;
            // width = Mathf.NextPowerOfTwo(width);
            //  height = Mathf.NextPowerOfTwo(height);
            int depth = 0;// 24;
            RenderTexture renderTexture = null;
#if UNITY_EDITOR
                renderTexture = RenderTexture.GetTemporary(width, height, depth, RenderTextureFormat.ARGB32);
#else
            if (IsSupportRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                renderTexture = RenderTexture.GetTemporary(width, height, depth, RenderTextureFormat.ARGBHalf);
            else
                renderTexture = RenderTexture.GetTemporary(width, height, depth, RenderTextureFormat.ARGB32);
#endif

            if(renderTexture!=null)
            {
                renderTexture.useMipMap = false;
                renderTexture.autoGenerateMips = false;
            }
            Camera camera = hudCamera.GetCamera();
       //     camera.forceIntoRenderTexture = true;
            camera.useOcclusionCulling = false;
            camera.depthTextureMode = DepthTextureMode.None;
#if USE_URP
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
            if(cameraData!=null)
            {
                cameraData.renderShadows = true;
                cameraData.dithering = false;
            //    cameraData.allowXRRendering = false;
                cameraData.requiresColorTexture = false;
                cameraData.requiresDepthOption = CameraOverrideOption.Off;
                cameraData.requiresColorOption = CameraOverrideOption.Off;
                cameraData.requiresDepthTexture = false;
                cameraData.renderPostProcessing = false;
                cameraData.antialiasing = AntialiasingMode.None;
            }
#endif
            hudCamera.SetLightCullingMask(1 << LayerUtil.RenderUIModelLayer);

            hudCamera.SetPosition(vPos);
            camera.fieldOfView = fov;
            camera.targetTexture = renderTexture;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.cullingMask = 1 << LayerUtil.RenderUIModelLayer;

            rawImage.texture = renderTexture;

            HudRender hudRender = new HudRender();
            hudRender.camera = hudCamera;
            hudRender.binderData = bindUser;

            m_RenderHuds.Add(hudRender);
            return hudCamera;
        }
        //-----------------------------------------------------
        bool IsSupportARGB16()
        {
            return IsSupportRenderTextureFormat(RenderTextureFormat.ARGB4444);
        }
        //-----------------------------------------------------
        bool IsSupportRenderTextureFormat(RenderTextureFormat textureFormat)
        {
            return SystemInfo.SupportsRenderTextureFormat(textureFormat);
        }
    }
}
