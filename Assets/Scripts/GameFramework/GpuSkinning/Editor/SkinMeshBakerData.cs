#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Framework.Plugin
{
    [System.Serializable]
    public class SkinMeshBakerData
    {
        public string bakerRootPath
        {
            get
            {
                if(fbxAsset)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(fbxAsset);
                    string dir = System.IO.Path.GetDirectoryName(path).Replace("\\", "/") + "/baker/";
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    return dir;
                }
                return "";
            }
        }
        public string assetName
        {
            get
            {
                if (fbxAsset)
                {
                    return fbxAsset.name;
                }
                return "";
            }
        }
        [System.Serializable]
        public class BakeFrame
        {
            [SerializeField] public string state = "";
            [SerializeField] public List<int> frams = new List<int>();
        }
        [SerializeField] string m_strUseTexture = null;
        [SerializeField] string m_strGPUUseTexture = null;
        private Texture m_pUseTexture = null;
        private Texture m_pGPUUseTexture = null;


        public Texture useTexture
        {
            get
            {
                if (m_pUseTexture == null && !string.IsNullOrEmpty(m_strUseTexture))
                {
                    m_pUseTexture = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture>(AssetDatabase.GUIDToAssetPath(m_strUseTexture));
                }
                return m_pUseTexture;
            }
            set
            {
                m_pUseTexture = value;
                m_strUseTexture = null;
                if (m_pUseTexture) m_strUseTexture = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            }
        }
        public Texture gpuUseTexture
        {
            get
            {
                if (m_pGPUUseTexture == null && !string.IsNullOrEmpty(m_strGPUUseTexture))
                {
                    m_pGPUUseTexture = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture>(AssetDatabase.GUIDToAssetPath(m_strGPUUseTexture));
                }
                return m_pGPUUseTexture;
            }
            set
            {
                m_pGPUUseTexture = value;
                m_strGPUUseTexture = null;
                if (m_pGPUUseTexture) m_strGPUUseTexture = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            }
        }
        [SerializeField] string m_strGPUMaterialGuid= null;
        private Material m_pGPUMaterial = null;
        public Material useMaterial
        {
            get
            {
                if (m_FbxAsset == null && !string.IsNullOrEmpty(m_strGPUMaterialGuid))
                {
                    m_pGPUMaterial = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(AssetDatabase.GUIDToAssetPath(m_strGPUMaterialGuid));
                }
                return m_pGPUMaterial;
            }
            set
            {
                m_pGPUMaterial = value;
                m_strGPUMaterialGuid = null;
                if (m_pGPUMaterial) m_strGPUMaterialGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            }
        }
        public Vector2[] offsets;
        public Vector2[] UV;

        [SerializeField] private string m_FbxAssetGuid = null;
        private UnityEngine.Object m_FbxAsset = null;
        public UnityEngine.Object fbxAsset
        {
            get
            {
                if(m_FbxAsset == null && !string.IsNullOrEmpty(m_FbxAssetGuid))
                {
                    m_FbxAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(m_FbxAssetGuid));
                }
                return m_FbxAsset;
            }
            set
            {
                m_FbxAsset = value;
                m_FbxAssetGuid = null;
                if (m_FbxAsset) m_FbxAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            }
        }
        [SerializeField] private string m_ControllerGuid = null;
        RuntimeAnimatorController m_Controller;
        public RuntimeAnimatorController controller
        {
            get
            {
                if (m_Controller == null && !string.IsNullOrEmpty(m_ControllerGuid))
                {
                    m_Controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetDatabase.GUIDToAssetPath(m_ControllerGuid));
                }
                return m_Controller;
            }
            set
            {
                m_Controller = value;
                m_ControllerGuid = null;
                if (m_Controller) m_ControllerGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            }
        }

        [SerializeField] private string[] m_AnimClipGuids = null;
        private AnimationClip[] m_Animations;

        public AnimationClip[] animations
        {
            get
            {
                if(m_Animations == null && m_AnimClipGuids!=null)
                {
                    List<AnimationClip> vAnims = new List<AnimationClip>();
                    for (int i =0; i < m_AnimClipGuids.Length; ++i)
                    {
                        var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(m_AnimClipGuids[i]));
                        if (anim == null)
                            continue;
                        vAnims.Add(anim);
                    }
                    m_Animations = vAnims.ToArray();
                }
                return m_Animations;
            }
            set
            {
                m_Animations = value;
                if(value!=null)
                {
                    m_AnimClipGuids = new string[value.Length];
                    for (int i =0; i < value.Length; ++i)
                    {
                        m_AnimClipGuids[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value[i]));
                    }
                }
            }
        }
        public BakeFrame[] bakeFrames;
        public BakeFrame getFrame(string name, bool bCreate = false)
        {
            if (bCreate)
            {
                if (bakeFrames != null)
                {
                    for (int i = 0; i < bakeFrames.Length; ++i)
                    {
                        if (bakeFrames[i].state == name)
                        {
                            return bakeFrames[i];
                        }
                    }
                }

                List<BakeFrame> list = null;
                if (bakeFrames != null) list = new List<BakeFrame>(bakeFrames);
                else list = new List<BakeFrame>();
                list.Add(new BakeFrame() { state = name });
                bakeFrames = list.ToArray();
                return bakeFrames[bakeFrames.Length - 1];
            }

            if (bakeFrames == null) return null;
            for (int i = 0; i < bakeFrames.Length; ++i)
            {
                if (bakeFrames[i].state == name)
                {
                    return bakeFrames[i];
                }
            }
            return null;
        }

        [SerializeField] public string[] bakerAssets;
        private ScriptableObject[] m_arrSkinTypeDatas;
        public void SetBakeAsset(ESkinType type, ScriptableObject frameData)
        {
            if (type >= ESkinType.None)
                return;

            if (m_arrSkinTypeDatas == null)
                m_arrSkinTypeDatas = new ScriptableObject[(int)ESkinType.Count];
            m_arrSkinTypeDatas[(int)type] = frameData;

            if (bakerAssets == null || bakerAssets.Length<=0)
                bakerAssets = new string[(int)ESkinType.Count];
            bakerAssets[(int)type] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(frameData));
        }

        public ScriptableObject GetBakeAsset(ESkinType type)
        {
            if (type >= ESkinType.None)
                return null;

            if (m_arrSkinTypeDatas == null)
                m_arrSkinTypeDatas = new ScriptableObject[(int)ESkinType.Count];
            if ((int)type < m_arrSkinTypeDatas.Length && m_arrSkinTypeDatas[(int)type] != null)
                return m_arrSkinTypeDatas[(int)type];
            if (bakerAssets != null && (int)type < bakerAssets.Length && !string.IsNullOrEmpty(bakerAssets[(int)type]))
            {
                m_arrSkinTypeDatas[(int)type] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(bakerAssets[(int)type]));
            }
            return m_arrSkinTypeDatas[(int)type];
        }
        public void Save()
        {
            if (fbxAsset == null)
                return;
            
            string bakerPath = bakerRootPath + assetName + ".baker";
            System.IO.File.WriteAllText(bakerPath, JsonUtility.ToJson(this, true));
        }
        public void CopyTo(SkinMeshBakerData outData)
        {
            outData.useTexture = useTexture;
            outData.gpuUseTexture = gpuUseTexture;
            outData.UV = UV;
            outData.offsets = offsets;
            outData.fbxAsset = fbxAsset;
            outData.bakeFrames = bakeFrames;
            outData.controller = controller;
            outData.bakerAssets = bakerAssets;
            outData.m_arrSkinTypeDatas = m_arrSkinTypeDatas;
        }
    }
}
#endif
