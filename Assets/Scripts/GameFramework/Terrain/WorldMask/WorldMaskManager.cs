/********************************************************************
生成日期:	08:04:2025
类    名: 	WorldMaskManager
作    者:	HappLI
描    述:	世界遮罩管理器，比如云雾遮罩、脚印等等，可以统一在这里管理
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using Framework.Base;
public class WorldMaskManager
{
    public static string CLOUD_UNLOCK_MASK = "CloudUnlockMask";
    private static WorldMaskManager ms_instance = null;
    static WorldMaskManager Instance
    {
        get
        {
            if (ms_instance == null)
            {
                ms_instance = new WorldMaskManager();
                ms_instance.Initialize();
            }
            return ms_instance;
        }
    }
    //-------------------------------------------------------
    public struct MaskWorldInfo
    {
        public Vector3 startPos;
        public Vector2Int size;
        public float cellSize;
        public Vector4 worldMinSize;
        public Material maskGeneMat;
        public int globalSizeShaderPropId;
#if UNITY_EDITOR
        public string displayName;
#endif
    }
    //-------------------------------------------------------
    public enum EMaskChannel
    {
        [Display("R通道")] Red = 0,
        [Display("透明通道")]Alpha,
    }
    //-------------------------------------------------------
    class MaskData
    {
        public string maskShaderPropName;
        public int maskShaderPropId;
        public RenderTexture maskTexture;
        public Material maskGeneratorMat;
        public Material maskBlur;
        public int useRef;
    }
    Dictionary<string, MaskData> m_vMaskDatas = null;
    Dictionary<string, MaskWorldInfo> m_vMaskMaterial = null;
    //-------------------------------------------------------
    void Initialize()
    {
        if(m_vMaskMaterial == null)
        {
            m_vMaskMaterial = new Dictionary<string, MaskWorldInfo>(2);
            {
                MaskWorldInfo info = new MaskWorldInfo();
                Shader shader = Shader.Find("EN/CloudUnlockMask");
                if(shader == null)
                {
                 //   GetLoader().LoadAsset("CloudUnlockMask", (obj) => {
                 //       shader = obj as Shader;
                 //   }, false);
                }
                if(shader !=null)
                {
                    info.maskGeneMat = new Material(shader);
                    info.maskGeneMat.hideFlags |= HideFlags.HideAndDontSave;
                    info.startPos = Vector3.zero;
                    info.size = new Vector2Int(1024, 1024);
                    info.cellSize = 7;
                    info.worldMinSize = new Vector4(info.startPos.x, info.startPos.z, info.size.x, info.size.y) * info.cellSize;
                    info.globalSizeShaderPropId = WorldMaskShaderPropId._CloudAreaWorldBounds;
#if UNITY_EDITOR
                    info.displayName = "云雾遮罩";
#endif
                    m_vMaskMaterial[CLOUD_UNLOCK_MASK] = info;
                }
                else
                {
                    Debug.LogError("WorldMaskManager: CloudUnlockMask shader not found, please check if the shader is included in the project.");
                }
            }
        }
    }
    //-------------------------------------------------------
    ~WorldMaskManager()
    {
        Shutdown();
    }
    //-------------------------------------------------------
    public void Clear()
    {
        if (m_vMaskDatas != null)
        {
            foreach (var db in m_vMaskDatas)
            {
                var maskData = db.Value;
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    if (maskData.maskBlur) Object.DestroyImmediate(maskData.maskBlur);
                    if (maskData.maskTexture) Object.DestroyImmediate(maskData.maskTexture);
                }
                else
                {
                    if (maskData.maskBlur) Object.Destroy(maskData.maskBlur);
                    if (maskData.maskTexture) RenderTexture.ReleaseTemporary(maskData.maskTexture);
                }
#else
                if (maskData.maskBlur) Object.Destroy(maskData.maskBlur);
                if (maskData.maskTexture) RenderTexture.ReleaseTemporary(maskData.maskTexture);
#endif
            }
            m_vMaskDatas.Clear();
        }
    }
    //-------------------------------------------------------
    public void Shutdown()
    {
        if (m_vMaskMaterial != null)
        {
//            foreach (var db in m_vMaskMaterial)
//            {
//                var maskData = db.Value;
//#if UNITY_EDITOR
//                if (Application.isEditor)
//                {
//                    if (maskData.maskGeneMat) Object.DestroyImmediate(maskData.maskGeneMat);
//                }
//                else
//                {
//                    if (maskData.maskGeneMat) Object.Destroy(maskData.maskGeneMat);
//                }
//#endif
//            }
            m_vMaskMaterial.Clear();
        }
        Clear();
    }
    //-------------------------------------------------------
    public void Update(float deltaTime)
    {
        if (m_vMaskDatas == null || m_vMaskDatas.Count<=0)
            return;
        if(m_vMaskMaterial!=null)
        {
            foreach (var db in m_vMaskMaterial)
            {
                Shader.SetGlobalVector(db.Value.globalSizeShaderPropId, db.Value.worldMinSize);
            }
        }
    }
    //-------------------------------------------------------
    public static bool ApplayMaskData(string maskName, string shaderPropName)
    {
        if (ms_instance == null)
            Instance.Initialize();

        if (ms_instance.m_vMaskMaterial == null)
            return false;
        if(!ms_instance.m_vMaskMaterial.TryGetValue(maskName, out var maskInfo))
        {
            return false;
        }
        if (maskInfo.maskGeneMat == null)
            return false;

        if (ms_instance.m_vMaskDatas == null)
            ms_instance.m_vMaskDatas = new Dictionary<string, MaskData>(2);

        if (ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            maskData.maskShaderPropId = Shader.PropertyToID(shaderPropName);
            maskData.maskShaderPropName = shaderPropName;
            maskData.useRef++;
        }
        else 
        {
            MaskData data = new MaskData();
            //! r=mask gb=flow direction
            RenderTextureFormat textureFormat = RenderTextureFormat.ARGB32;
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565)) textureFormat = RenderTextureFormat.RGB565;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB4444)) textureFormat = RenderTextureFormat.ARGB4444;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)) textureFormat = RenderTextureFormat.ARGBHalf;

            data.maskTexture = RenderTexture.GetTemporary(maskInfo.size.x, maskInfo.size.y, 0, textureFormat);
            data.maskTexture.wrapMode = TextureWrapMode.Clamp;
            data.maskTexture.filterMode = FilterMode.Bilinear;
            data.useRef = 1;
            data.maskShaderPropId = Shader.PropertyToID(shaderPropName);
            data.maskShaderPropName = shaderPropName;
            data.maskGeneratorMat = maskInfo.maskGeneMat;
            ms_instance.m_vMaskDatas.Add(maskName, data);
            return true;
        }
        return true;
    }
    //-------------------------------------------------------
    public static void ReleaseMask(string maskName, bool bForce = false)
    {
        if (ms_instance == null || ms_instance.m_vMaskDatas == null)
            return;
        if (ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            Graphics.Blit(null, maskData.maskTexture, maskData.maskGeneratorMat, 1);
            maskData.useRef--;
            if(maskData.useRef<=0 || bForce)
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    if(maskData.maskBlur) Object.DestroyImmediate(maskData.maskBlur);
                    if(maskData.maskTexture) RenderTexture.ReleaseTemporary(maskData.maskTexture);
                }
                else
                {
                    if (maskData.maskBlur) Object.Destroy(maskData.maskBlur);
                    if (maskData.maskTexture) RenderTexture.ReleaseTemporary(maskData.maskTexture);
                }
#else
                if (maskData.maskBlur) Object.Destroy(maskData.maskBlur);
                if (maskData.maskTexture) RenderTexture.ReleaseTemporary(maskData.maskTexture);
#endif
                ms_instance.m_vMaskDatas.Remove(maskName);
            }
        }
    }
    //-------------------------------------------------------
    public static void ClearMask(string maskName)
    {
        if (ms_instance == null || ms_instance.m_vMaskDatas == null)
            return;
        if (ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            Graphics.Blit(null, maskData.maskTexture, maskData.maskGeneratorMat, 1);
        }
    }
    //-------------------------------------------------------
    public static RenderTexture GetMaskTexture(string maskName)
    {
        if (ms_instance == null) return null;
        if (ms_instance.m_vMaskDatas == null)
            return null;
        if (ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            return maskData.maskTexture;
        }
        return null;
    }
    //-------------------------------------------------------
    public static Material GetMaskMaterial(string maskName)
    {
        if (ms_instance == null) return null;
        if (ms_instance.m_vMaskDatas == null)
            return null;
        if (ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            return maskData.maskGeneratorMat;
        }
        return null;
    }
    //-------------------------------------------------------
    public static void UpdateMask(string maskName, Vector3 position, float radius, float radiusFade, float angle = 360, bool bAdditive = true)
    {
        UpdateMask(maskName, position, Vector3.forward, radius, radius, angle, bAdditive);
    }
    //-------------------------------------------------------
    public static void UpdateMask(string maskName, Vector3 position, Vector3 direction, float radius, float radiusFade, float angle = 360, bool bAdditive = true)
    {
        UpdateMask(maskName, position, direction, radius, radius, radiusFade, angle, bAdditive);
    }
    //-------------------------------------------------------
    public static void UpdateMask(string maskName, Vector3 position, Vector3 dir, float radius, float maxRadius, float radiusFade, float angle = 360, bool bAdditive = true)
    {
        if (ms_instance == null)
            Instance.Initialize();
        if (ms_instance.m_vMaskDatas == null)
            return;
        if (!ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            return;
        }
        maskData.maskGeneratorMat.SetVector(WorldMaskShaderPropId._WorldCenter, new Vector4(position.x, position.y, position.z, 0));
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._Radius, radius);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._MaxRadius, maxRadius);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._InnerRadius, 0);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._RadiusFade, radiusFade);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._SectorAngle, angle);
        maskData.maskGeneratorMat.SetVector(WorldMaskShaderPropId._SectorDir, new Vector4(dir.x, dir.y, dir.z, 0));
        maskData.maskGeneratorMat.SetTexture(WorldMaskShaderPropId._ShapeTex, Texture2D.whiteTexture);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._ShapeExpand, 1);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._OnlyShapeSimple, 0);

        if (bAdditive)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(maskData.maskTexture.width, maskData.maskTexture.height, 0, maskData.maskTexture.format);
            Graphics.Blit(maskData.maskTexture, tempRT);
            Graphics.Blit(tempRT, maskData.maskTexture, maskData.maskGeneratorMat, 0);
            RenderTexture.ReleaseTemporary(tempRT);
        }
        else
        {
            Graphics.Blit(null, maskData.maskTexture, maskData.maskGeneratorMat, 0);
        }

        Shader.SetGlobalTexture(maskData.maskShaderPropId, maskData.maskTexture);
    }
    //-------------------------------------------------------
    public static void UpdateMaskTexture(string maskName, Texture regionMaskTexture, EMaskChannel eChannel, float shapeScale, Vector3 position, Vector3 dir, float radius, float maxRadius, float radiusFade, bool bAdditive = true, bool bExpand = false)
    {
        if (ms_instance == null)
            Instance.Initialize();
        if (ms_instance.m_vMaskDatas == null)
            return;
        if (!ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            return;
        }
        maskData.maskGeneratorMat.SetVector(WorldMaskShaderPropId._WorldCenter, new Vector4(position.x, position.y, position.z, 0));
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._Radius, radius);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._MaxRadius, maxRadius);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._InnerRadius, 0);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._RadiusFade, radiusFade);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._SectorAngle, 360);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._ShapeScale, shapeScale);
        maskData.maskGeneratorMat.SetVector(WorldMaskShaderPropId._SectorDir, new Vector4(dir.x, dir.y, dir.z, 0));
        maskData.maskGeneratorMat.SetTexture(WorldMaskShaderPropId._ShapeTex, regionMaskTexture);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._OnlyShapeSimple, 1);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._ShapeExpand, bExpand?0:1);
        maskData.maskGeneratorMat.SetFloat(WorldMaskShaderPropId._ShapeSimpleChanel, (float)(int)eChannel);

        if (bAdditive)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(maskData.maskTexture.width, maskData.maskTexture.height, 0, maskData.maskTexture.format);
            Graphics.Blit(maskData.maskTexture, tempRT);
            Graphics.Blit(tempRT, maskData.maskTexture, maskData.maskGeneratorMat, 0);
            RenderTexture.ReleaseTemporary(tempRT);
        }
        else
        {
            Graphics.Blit(null, maskData.maskTexture, maskData.maskGeneratorMat, 0);
        }

        Shader.SetGlobalTexture(maskData.maskShaderPropId, maskData.maskTexture);
    }
    //-------------------------------------------------------
    public static void BlurMask(string maskName, int blurStep, float blurStrength)
    {
        if (blurStep <= 0)
            return;
        if (ms_instance == null)
            Instance.Initialize();
        if (ms_instance.m_vMaskDatas == null)
            return;
        if (!ms_instance.m_vMaskDatas.TryGetValue(maskName, out var maskData))
        {
            return;
        }
        if(maskData.maskBlur == null)
        {
            var shader = Shader.Find("EN/TextureGaussianBlur");
            if(shader == null)
            {
            //    ms_instance.GetLoader().LoadAsset("TextureGaussianBlur", (obj) => {
           //             shader = obj as Shader;
            //        }, false);
            }
            if (shader == null)
            {
                Debug.LogError("WorldMaskManager: TextureGaussianBlur shader not found, please check if the shader is included in the project.");
                return;
            }
            maskData.maskBlur = new Material(shader);
            maskData.maskBlur.hideFlags |= HideFlags.HideAndDontSave;
        }
        if(maskData.maskBlur == null)
        {
            return;
        }

        if (blurStep > 0)
        {
            RenderTexture temp1 = RenderTexture.GetTemporary(maskData.maskTexture.width, maskData.maskTexture.height, 0, maskData.maskTexture.format);
            RenderTexture temp2 = RenderTexture.GetTemporary(maskData.maskTexture.width, maskData.maskTexture.height, 0, maskData.maskTexture.format);
            for (int i = 0; i < blurStep; ++i)
            {
                maskData.maskBlur.SetFloat(WorldMaskShaderPropId._BlurStrength, blurStrength);
                Graphics.Blit(maskData.maskTexture, temp1, maskData.maskBlur, 1); // Pass 1: Horizontal

                maskData.maskBlur.SetFloat(WorldMaskShaderPropId._BlurStrength, blurStrength);
                Graphics.Blit(temp1, temp2, maskData.maskBlur, 0); // Pass 0: Vertical
                Graphics.Blit(temp2, maskData.maskTexture);
            }

            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
            Shader.SetGlobalTexture(maskData.maskShaderPropId, maskData.maskTexture);
        }
    }
    //-------------------------------------------------------
    public static void EditorUpdate()
    {
        if (ms_instance == null) return;
        ms_instance.Update(0.033f);
    }
    //-------------------------------------------------------
    public static Dictionary<string, MaskWorldInfo> GetMaskWorldInfos()
    {
        if(ms_instance == null) Instance.Initialize();
        if (ms_instance == null) return null;
        return ms_instance.m_vMaskMaterial;
    }
}
