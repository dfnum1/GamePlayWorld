/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SceneTheme
作    者:	HappLI
描    述:	场景主题
*********************************************************************/
#define USE_FMOD
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Framework.Core
{
    public abstract class ASceneTheme : IUserData
    {  
        protected bool m_bCleared = false;

        protected int m_nCurTheme = 0;

        protected string m_strBG = null;
        protected AInstanceAble m_pBGInstnace = null;

        protected string m_strNearEffect = null;
        protected AInstanceAble m_pNearEffectInstnace = null;

        protected string m_strSkyBoxMat = null;
        protected Asset m_pSkyBoxAsset = null;

        protected AnimationCurve m_FadeInCurve = null;
        protected ISound m_pSound = null;

        //! keep distance
        protected float m_fNearDistnace = 0;
        protected Vector3 m_NearOffset = Vector3.zero;
        protected Vector3 m_NearRotate = Vector3.zero;
        protected float m_fFarDistnace = 0;
        protected Vector3 m_FarOffset = Vector3.zero;
        protected Vector3 m_FarRotate = Vector3.zero;

        //! fog
        protected int m_nCloseFog = 0;
        protected ColorLerp m_FogColor = new ColorLerp();
        protected FloatLerp m_FogStart = new FloatLerp();
        protected FloatLerp m_FogEnd = new FloatLerp();
        protected FloatLerp m_FogDensity = new FloatLerp();

        //! light
        protected List<Light> m_vLights = new List<Light>(2);

        //! Environment color
        protected ColorLerp m_EnvironmentColor = new ColorLerp();
        //------------------------------------------------------
        public ASceneTheme()
        {
        }
        //------------------------------------------------------
        public void SetFarDistance(float fDistance, Vector3 offset, Vector3 rotateOffset)
        {
            m_fFarDistnace = fDistance;
            m_FarOffset = offset;
            m_FarRotate = rotateOffset;
        }
        //------------------------------------------------------
        public void SetNearDistance(float fDistance, Vector3 offset, Vector3 rotateOffset)
        {
            m_fNearDistnace = fDistance;
            m_NearOffset = offset;
            m_NearRotate = rotateOffset;
        }
        //------------------------------------------------------
        public int GetCurTheme()
        {
            return m_nCurTheme;
        }
        //------------------------------------------------------
        public abstract bool UseTheme(uint theme, bool bClear = false, bool bLerpTo = true);
        public abstract bool UseTheme(IUserData pData, bool bClear = false, bool bLerpTo = true);
        //------------------------------------------------------
        public void SetEnvironmentColor(Color color, float fLerp = -1)
        {
            m_EnvironmentColor.toValue = color;
            if (fLerp <= 0)
            {
                m_EnvironmentColor.Blance();
            }
            else
            {
                m_EnvironmentColor.fFactor = fLerp;
            }
        }
        //------------------------------------------------------
        public void UpdateBG(string strBG)
        {
            if (string.IsNullOrEmpty(strBG)) return;
            if (strBG.CompareTo(m_strBG) == 0) return;

            if (m_pBGInstnace != null)
                m_pBGInstnace.RecyleDestroy(1);
            m_pBGInstnace = null;
            m_strBG = strBG;
            InstanceOperiaon pCb = FileSystemUtil.SpawnInstance(strBG, true);
            if (pCb != null)
            {
                pCb.OnCallback = OnSpawnSceneBG;
                pCb.OnSign = OnSpawnSign;
                pCb.pByParent = RootsHandler.ThemeRoot;
            }
        }
        //------------------------------------------------------
        public void UpdateNearEffect(string strEffect)
        {
            if (string.IsNullOrEmpty(strEffect)) return;
            if (strEffect.CompareTo(m_strNearEffect) == 0) return;

            if (m_pNearEffectInstnace != null)
                m_pNearEffectInstnace.RecyleDestroy(1);
            m_pNearEffectInstnace = null;
            m_strNearEffect = strEffect;
            InstanceOperiaon pCb = FileSystemUtil.SpawnInstance(strEffect, true);
            if (pCb != null)
            {
                pCb.OnCallback = OnSpawnSceneNearEffect;
                pCb.OnSign = OnSpawnEffectSign;
                pCb.pByParent = RootsHandler.ThemeRoot;
            }
        }
        //------------------------------------------------------
        public void UpdateSkyBox(string skyBox)
        {
            if (skyBox == null) return;
            if (!string.IsNullOrEmpty(skyBox))
            {
                if (skyBox.CompareTo(m_strSkyBoxMat) == 0) return;
                m_strSkyBoxMat = skyBox;
                AssetOperiaon pAssetOp = FileSystemUtil.AsyncReadFile(skyBox);
                if (pAssetOp != null)
                {
                    pAssetOp.OnCallback = OnSkyBoxCallback;
                }
            }
        }
        //------------------------------------------------------
        public void FadeInMusic(string music, float fadeLerp)
        {
            if (fadeLerp <= 0)
            {
                UpdateMusic(music);
                return;
            }
            if (m_FadeInCurve == null) m_FadeInCurve = new AnimationCurve();
            m_FadeInCurve.keys = null;
            m_FadeInCurve.AddKey(0, 0);
            m_FadeInCurve.AddKey(fadeLerp, 1);
            UpdateMusic(music, m_FadeInCurve);
        }
        //------------------------------------------------------
        public void FadeInMusic(string music, AnimationCurve fadeLerp)
        {
#if !USE_FMOD
            if (fadeLerp == null)
            {
                FadeInMusic(music, 1);
                return;
            }
#endif
            UpdateMusic(music, fadeLerp);
        }
        //------------------------------------------------------
        public void UpdateMusic(string music, AnimationCurve fadeLerp = null)
        {
            if (string.IsNullOrEmpty(music))
            {
                AudioManager.FadeOutALLBGs(1);
                return;
            }
            if (m_pSound != null) m_pSound.Stop();
            m_pSound = AudioUtil.PlayBG(music, fadeLerp);
        }
        //------------------------------------------------------
        public void UpdateFog(bool bEnable, Color fogColor, float fogStart, float fogEnd, float fogDensity, float fLerp = -1)
        {
          //  if (!Data.GameQuality.enableFog) return;
            RenderSettings.fog = bEnable;
            if (bEnable)
            {
                m_FogColor.toValue = fogColor;
                m_FogStart.toValue = fogStart;
                m_FogEnd.toValue = fogEnd;
                m_FogDensity.toValue = fogDensity;

                if (fLerp <= 0)
                {
                    m_FogColor.Blance();
                    m_FogStart.Blance();
                    m_FogEnd.Blance();
                    m_FogDensity.Blance();
                }
                else
                {
                    m_FogColor.fFactor = fLerp;
                    m_FogStart.fFactor = fLerp;
                    m_FogEnd.fFactor = fLerp;
                    m_FogDensity.fFactor = fLerp;
                }
                ApplayFog(0);
            }
        }
        //------------------------------------------------------
        void OnSkyBoxCallback(AssetOperiaon pOp)
        {
            if (m_pSkyBoxAsset != null)
            {
                if (m_pSkyBoxAsset == pOp.pAsset)
                    return;
                m_pSkyBoxAsset.Release();
            }
            m_pSkyBoxAsset = null;

            if (m_bCleared)
                return;

            if (m_strSkyBoxMat.CompareTo(pOp.strFile) != 0)
                return;

            m_pSkyBoxAsset = pOp.pAsset;
            m_pSkyBoxAsset.Grab();
            SetSkybox(m_pSkyBoxAsset.GetOrigin<Material>());
        }
        //------------------------------------------------------
        protected void SetSkybox(Material material)
        {
            if (RenderSettings.skybox == material)
                return;
            RenderSettings.skybox = material;
        }
        //------------------------------------------------------
        public void SetCameraClearType(CameraClearFlags flag, Color color)
        {
          //  CameraUtil.SetCameraClearFlag(flag, color);
        }
        //------------------------------------------------------
        public virtual void Clear()
        {
            m_nCurTheme = 0;
            m_bCleared = true;
            if (m_pBGInstnace != null)
                m_pBGInstnace.RecyleDestroy(1);
            m_pBGInstnace = null;
            m_strBG = null;

            if (m_pNearEffectInstnace != null)
                m_pNearEffectInstnace.RecyleDestroy(1);
            m_pNearEffectInstnace = null;
            m_strNearEffect = null;

            m_fNearDistnace = 200;
            m_fFarDistnace = 10;
            m_FarOffset = Vector3.zero;
            m_FarRotate = Vector3.zero;
            m_NearOffset = Vector3.zero;
            m_NearRotate = Vector3.zero;

            if (m_pSkyBoxAsset != null)
            {
                m_pSkyBoxAsset.Release();
            }
            m_pSkyBoxAsset = null;
            m_strSkyBoxMat = null;

            if (m_pSound != null) m_pSound.Stop();
            m_pSound = null;

            m_nCloseFog = 0;
            m_FogColor.Reset();
            m_FogColor.toValue = Color.white;
            m_FogColor.Blance();
            m_FogStart.Reset(); m_FogStart.toValue = 40; m_FogStart.Blance();
            m_FogEnd.Reset(); m_FogEnd.toValue = 300; m_FogEnd.Blance();
            m_FogDensity.Reset(); m_FogDensity.toValue = 1; m_FogDensity.Blance();

            ApplayFog(0);
            //  RenderSettings.fog = false;

            m_EnvironmentColor.Reset();
            m_EnvironmentColor.toValue = Color.white;
            m_EnvironmentColor.Blance();
            ApplayEnvironmentColor(0);

            if(m_vLights!=null)
            {
                for(int i = 0; i < m_vLights.Count; ++i)
                {
                    if(m_vLights[i])
                        GameObject.Destroy(m_vLights[i].gameObject);
                }
                m_vLights.Clear();
            }
        }
        //------------------------------------------------------
        protected Light AddLight(string lightName)
        {
            if (string.IsNullOrEmpty(lightName)) return null;
            GameObject lightGO = new GameObject(lightName);
            lightGO.transform.SetParent(RootsHandler.ThemeRoot);
            BaseUtil.ResetGameObject(lightGO, EResetType.All);
            Light light = lightGO.AddComponent<Light>();
            m_vLights.Add(light);
            return light;
        }
        //------------------------------------------------------
        void ApplayFog(float fFrame)
        {
            m_FogColor.Update(fFrame);
            m_FogStart.Update(fFrame);
            m_FogEnd.Update(fFrame);
            m_FogDensity.Update(fFrame);

            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = m_FogColor.value;
            RenderSettings.fogStartDistance = m_FogStart.value;
            RenderSettings.fogEndDistance = m_FogEnd.value;
            RenderSettings.fogDensity = m_FogDensity.value;

            RenderSettings.fog = /*Data.GameQuality.enableFog && */m_nCloseFog<=0;
        }
        //------------------------------------------------------
        public void EnableFog(bool bEnable)
        {
            int pre = m_nCloseFog;
            if (bEnable)
            {
                m_nCloseFog--;
                if (m_nCloseFog < 0) m_nCloseFog = 0;
            }
            else
                m_nCloseFog++;
            RenderSettings.fog = /*Data.GameQuality.enableFog && */m_nCloseFog <= 0;
        }
        //------------------------------------------------------
        void ApplayEnvironmentColor(float fFrame)
        {
            m_EnvironmentColor.Update(fFrame);
            RenderSettings.ambientSkyColor = m_EnvironmentColor.value;
        }
        //------------------------------------------------------
        public void Update(float fFrame)
        {
            ApplayFog(fFrame);
            ApplayEnvironmentColor(fFrame);

            if (m_pSkyBoxAsset != null)
            {
                if (m_pSkyBoxAsset.RefCnt <= 0)
                    m_pSkyBoxAsset.Grab();
                SetSkybox(m_pSkyBoxAsset.GetOrigin<Material>());
            }



            Vector3 dir = CameraUtil.mainCameraDirection;
            if (m_pNearEffectInstnace)
            {
                Vector3 pos = CameraUtil.mainCameraPosition + dir.normalized * m_fNearDistnace;
                pos.x = 0;
                pos.y = 0;
                m_pNearEffectInstnace.SetPosition(pos+m_NearOffset);
                m_pNearEffectInstnace.SetEulerAngle(m_NearRotate);
            }
            if (m_pBGInstnace)
            {
                Vector3 pos = CameraUtil.mainCameraPosition + dir.normalized * m_fFarDistnace;
                pos.x = 0;
                pos.y = 0;
                m_pBGInstnace.SetEulerAngle(CameraUtil.mainCameraEulerAngle+m_FarRotate);
                m_pBGInstnace.SetPosition(pos+m_FarOffset);
            }
        }
        //------------------------------------------------------
        void OnSpawnSceneBG(InstanceOperiaon pCB)
        {
            if (m_pBGInstnace != null)
                m_pBGInstnace.RecyleDestroy(1);
            m_pBGInstnace = pCB.pPoolAble;
            Update(0);
        }
        //------------------------------------------------------
        void OnSpawnSign(InstanceOperiaon pCB)
        {
            if (pCB.strFile.CompareTo(m_strBG) != 0)
            {
                pCB.bUsed = false;
                return;
            }
            pCB.bUsed = !m_bCleared;
        }
        //------------------------------------------------------
        void OnSpawnSceneNearEffect(InstanceOperiaon pCB)
        {
            if (m_pNearEffectInstnace != null)
                m_pNearEffectInstnace.RecyleDestroy(1);
            m_pNearEffectInstnace = pCB.pPoolAble;
            Update(0);
        }
        //------------------------------------------------------
        void OnSpawnEffectSign(InstanceOperiaon pCB)
        {
            if (pCB.strFile.CompareTo(m_strNearEffect) != 0)
            {
                pCB.bUsed = false;
                return;
            }
            pCB.bUsed = !m_bCleared;
        }
        //------------------------------------------------------

        public void Destroy()
        {
            Clear();
        }
    }
}
