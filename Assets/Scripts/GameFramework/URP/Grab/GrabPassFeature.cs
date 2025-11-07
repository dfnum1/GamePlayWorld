#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderFeature
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework.URP
{
    [System.Serializable]
    public class GrabPassSettings : PassSettings
    {
        public string textureID;
        public Material blurMat;
        public float renderScale;
        public override bool isEnabled()
        {
            return base.isEnabled() && !string.IsNullOrEmpty(textureID);
        }
    }
    public class GrabPassFeature : APostRenderFeature
    {
        GrabRenderPass m_ScriptablePass;
        public GrabPassSettings settings = new GrabPassSettings();

        private static int ms_bActiveRef = 0;
        private static int ms_nCullingMask = 0;
        private static int ms_nGrabStat = 0;
        private static int ms_nGrabStatMaskFlag = 0;
        private static float ms_fGrabRenderScale = -1;
        private static bool ms_bGrabAutoBlur = false;
        private static Material ms_pPassMaterial = null;
        static GrabPassFeature ms_instnace = null;
        //------------------------------------------------------
        public static GrabPassFeature getInstance()
        {
            return ms_instnace;
        }
        //------------------------------------------------------
        public static void OnTogglePass(bool bEnabled)
        {
            if(ms_bActiveRef>0)
            {
                ms_nGrabStatMaskFlag = ms_nCullingMask;
                if(ms_nGrabStat<0) ms_nGrabStat = 1;
            }
        }
        //------------------------------------------------------
        public static int CullingMask
        {
            get { return ms_nCullingMask; }
            set { ms_nCullingMask = value; }
        }
        //------------------------------------------------------
        public static int GrabStatMaskFlag
        {
            get { return ms_nGrabStatMaskFlag; }
            set { ms_nGrabStatMaskFlag = value; }
        }
        //------------------------------------------------------
        public static int GrabStat
        {
            get { return ms_nGrabStat; }
            set { ms_nGrabStat = value; }
        }
        //------------------------------------------------------
        public static Material PassMaterial
        {
            get { return ms_pPassMaterial; }
        }
        //------------------------------------------------------
        public static Material CurGrabMaterial
        {
            get
            {
                if (ms_pPassMaterial != null) return ms_pPassMaterial;
                if (ms_instnace == null) return null;
                return ms_instnace.settings.blurMat;
            }
        }
        //------------------------------------------------------
        public static void ActiveRef(bool bActive, float renderScale =-1, int nCullingMask=0, bool bGrabAlways = true, bool bAutoBlur = false, Material passMaterial = null, AnimationCurve blurCurve = null)
        {
            int activeRef = ms_bActiveRef;
            if (bActive) ms_bActiveRef++;
            else
            {
                ms_pPassMaterial = null;
                ms_bActiveRef--;
            }
            if (ms_instnace == null)
            {
                ms_pPassMaterial = null;
                ms_nCullingMask = 0;
                return;
            }
            if (!ms_instnace.isActive)
            {
                ms_pPassMaterial = null;
                ms_nCullingMask = 0;
                return;
            }
            ms_bGrabAutoBlur = bAutoBlur;
            ms_fGrabRenderScale = renderScale;

            ms_pPassMaterial = passMaterial;
            if(ms_bActiveRef>0 && bActive)
            {
                Core.MaterailBlockUtil.SetFloat(GrabPassFeature.CurGrabMaterial, "_Strength", 1);
                if (ms_nCullingMask != nCullingMask)
                {
                    ms_nGrabStat = bGrabAlways ? 0 : 1;
                    ms_nGrabStatMaskFlag = nCullingMask;
                    ms_nCullingMask = nCullingMask;
                }
                else
                {
                    if (bGrabAlways) ms_nGrabStat = 0;
                }
            }

            if (ms_instnace.m_ScriptablePass != null)
                ms_instnace.m_ScriptablePass.SetActive(ms_bActiveRef > 0, renderScale, bAutoBlur);

        }
        //------------------------------------------------------
        public override void Create()
        {
            ms_instnace = this;
            this.name = settings.passName;
            m_ScriptablePass = new GrabRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.renderEvent;
            OnCreatePass(m_ScriptablePass);
            m_ScriptablePass.SetActive(ms_bActiveRef>0);
        }
        //------------------------------------------------------
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (ms_bActiveRef <= 0 || !settings.isEnabled())
                return;

            if (renderingData.cameraData.isSceneViewCamera) return;
#if UNITY_2020_1_OR_NEWER
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
#endif
            int layerMask = ms_nCullingMask;
            if (layerMask <= 0) layerMask = settings.layerMask.value;
            if ((renderingData.cameraData.camera.cullingMask & layerMask) == 0) return;
            if (!URPPostWorker.IsEnabledPass(EPostPassType.Grab))
                return;
            if (renderingData.cameraData.isSceneViewCamera) return;
            var src = renderer.cameraColorTarget;
            m_ScriptablePass.SetActive(ms_bActiveRef>0, ms_fGrabRenderScale, ms_bGrabAutoBlur);
            m_ScriptablePass.Setup(src);
            //注入队列
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
#endif