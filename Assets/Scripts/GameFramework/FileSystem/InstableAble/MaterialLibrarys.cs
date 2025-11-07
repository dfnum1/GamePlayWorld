/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	MaterialLibrarys
作    者:	HappLI
描    述:	材质图书馆
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public struct LerpRender
    {
        public bool lerping;
        public int index;
        public bool bAutoDestroy;
        public Material toMaterial;
        public float lerpDuration;
        public float invLerp;
        public float lerp;
        public float keepDuration;

        public string propertyName;
    }
    public class LibRender
    {
        public enum EPropertyType : byte
        {
            None,
            Int,
            Float,
            Vector4,
            Color,
        }
        Renderer m_renderer;
        public Renderer renderer
        {
            get { return m_renderer; }
        }
        public struct MatPropery
        {
            public int properyName;
            public EPropertyType property;
            public Variable4 value;
            public static MatPropery DEF = new MatPropery() { properyName = 0, property = EPropertyType.None };
            public MatPropery(string name, Vector4 value)
            {
                properyName = MaterailBlockUtil.BuildPropertyID(name);
                property = EPropertyType.Vector4;
                this.value = new Variable4() { floatVal0 = value.x, floatVal1 = value.y, floatVal2 = value.z, floatVal3 = value.w };
            }
            public MatPropery(string name, float value)
            {
                properyName = MaterailBlockUtil.BuildPropertyID(name);
                property = EPropertyType.Float;
                this.value = new Variable4() { floatVal0 = value };
            }
            public MatPropery(string name, int value)
            {
                properyName = MaterailBlockUtil.BuildPropertyID(name);
                property = EPropertyType.Int;
                this.value = new Variable4() { intVal0 = value };
            }
            public MatPropery(string name, Color value)
            {
                properyName = MaterailBlockUtil.BuildPropertyID(name);
                property = EPropertyType.Color;
                this.value = new Variable4() { floatVal0 = value.a, floatVal1 = value.r, floatVal2 = value.g, floatVal3 = value.b };
            }
            public void Set(Material material)
            {
                if (material == null || properyName == 0 || property == EPropertyType.None) return;
                if (!material.HasProperty(properyName))
                    return;
                switch(property)
                {
                    case EPropertyType.Float: material.SetFloat(properyName, value.floatVal0); break;
                    case EPropertyType.Int: material.SetInt(properyName, value.intVal0); break;
                    case EPropertyType.Vector4: material.SetVector(properyName, value.ToVector4()); break;
                    case EPropertyType.Color: material.SetColor(properyName, value.ToColor()); break;
                }
            }
            public void Clear()
            {
                properyName = 0;
                property = EPropertyType.None;
            }
        }

        public struct Mat
        {
            public Material material;
            public bool instanceNew;

            public Material fromMaterial;
            public Material toMaterial;
            public float lerpDuration;
            public float keepTime;
            public float lerpStartTime;
            public int fadeMode ; // 1- in 2- out 
            public bool autoDestroyToMaterial;

            public void Destroy()
            {
                DelToMaterial();
                DelFromMaterial();
                if(instanceNew) MaterialLibrarys.RecyleCloneMaterial(material);
                material = null;
                instanceNew = false;
            }
            public void DelToMaterial()
            {
                if (autoDestroyToMaterial)
                {
                    MaterialLibrarys.RecyleCloneMaterial(toMaterial);
                }
                if (fromMaterial && toMaterial)
                    material.Lerp(fromMaterial, toMaterial, 0);
                toMaterial = null;
                lerpDuration = 0;
                lerpStartTime = 0;
                fadeMode = 0;
                keepTime = 0;
                autoDestroyToMaterial = false;
            }
            public void DelFromMaterial()
            {
                MaterialLibrarys.RecyleCloneMaterial(fromMaterial);
                fromMaterial = null;
            }
        }
        public struct KeyWorld
        {
            public int index;
            public string keyWorld;
            public static KeyWorld DEF = new KeyWorld() { index = -1, keyWorld = null };
        }
        private int m_initCnt;
        private List<Mat> m_runtimes;
        private Material[] m_shares;

        public List<KeyWorld> keyWorlds;

        //------------------------------------------------------
        internal int ContainKey(string key, int index)
        {
            if (keyWorlds == null) return -1;
            KeyWorld ky;
            for (int i = 0; i < keyWorlds.Count; ++i)
            {
                ky = keyWorlds[i];
                if (!string.IsNullOrEmpty(ky.keyWorld) && ky.keyWorld.CompareTo(key) == 0 && ky.index == index) return 0;
            }

            return -1;
        }
        //------------------------------------------------------
        internal void Delkey(string key, int index)
        {
            if (keyWorlds == null) return;
            KeyWorld ky;
            for (int i = 0; i < keyWorlds.Count; ++i)
            {
                ky = keyWorlds[i];
                if (!string.IsNullOrEmpty(ky.keyWorld) && ky.keyWorld.CompareTo(key) == 0)
                {
                    keyWorlds.RemoveAt(i);
                    break;
                }
            }
        }
        //------------------------------------------------------
        internal void Addkey(string key, int index)
        {
            if (keyWorlds == null) keyWorlds = new List<KeyWorld>(1);
            KeyWorld ky = new KeyWorld();
            ky.keyWorld = key;
            ky.index = index;
            keyWorlds.Add(ky);
        }
        //------------------------------------------------------
        internal Material GetRuntime(int index, bool bAutoInstance)
        {
            if (m_runtimes != null)
            {
                if(bAutoInstance)
                {
                    bool bDirty = false;
                    if (m_runtimes.Count == 1)
                    {
                        Mat mat = m_runtimes[0];
                        if (!mat.instanceNew)
                        {
                            if(m_renderer.sharedMaterial)
                            {
                                mat.material = MaterialLibrarys.GetInstanceMaterial(m_renderer.sharedMaterial);
                                mat.instanceNew = true;
                                m_runtimes[0] = mat;
                                bDirty = true;
                            }
                        }
                    }
                    else
                    {
                        var materials = m_renderer.sharedMaterials;
                        for (int i = 0; i < m_runtimes.Count; ++i)
                        {
                            Mat mat = m_runtimes[i];
                            if (!mat.instanceNew && i < materials.Length)
                            {
                                if(materials[i])
                                {
                                    mat.material = MaterialLibrarys.GetInstanceMaterial(materials[i]);
                                    mat.instanceNew = true;
                                    m_runtimes[i] = mat;
                                    bDirty = true;
                                }
                            }
                        }
                    }
                    if (bDirty) Applay();
                }
                if (index >= 0 && index < m_runtimes.Count)
                {
                    Mat mat = m_runtimes[index];
                    return mat.material;
                }
                return null;
            }
            return null;
        }
        //------------------------------------------------------
        internal int GetCount()
        {
            if (m_runtimes != null) return m_runtimes.Count;
            return m_initCnt;
        }
        //------------------------------------------------------
        internal bool CheckRuntime()
        {
            if (m_renderer == null) return false;
            if (m_initCnt < 0)
            {
                if (m_runtimes == null || m_runtimes.Count<=0)
                {
                    m_shares = m_renderer.sharedMaterials;
                    if(m_runtimes == null) m_runtimes = new List<Mat>(m_shares.Length);
                    m_runtimes.Clear();
                    for (int i = 0; i < m_shares.Length; ++i)
                    {
                        m_runtimes.Add(new Mat() { material = m_shares[i], instanceNew = false });
                    }
                }
                m_initCnt = m_runtimes != null ? m_runtimes.Count : 0;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        internal bool SetRuntime(int index, Material material, bool bAutoDestroy = true)
        {
            bool bDirty = false;
            if (m_runtimes != null)
            {
                if (index >= 0 && index < m_runtimes.Count)
                {
                    Mat mat = m_runtimes[index];
                    if (mat.material != material)
                    {
                        bDirty = true;
                        if (mat.instanceNew)
                        {
                            MaterialLibrarys.RecyleCloneMaterial(mat.material);
                        }

                        mat.material = material;
                        mat.instanceNew = bAutoDestroy;
                        m_runtimes[index] = mat;
                    }
                }
                else if (index < 0)
                {
                    for (int i = 0; i < m_runtimes.Count; ++i)
                    {
                        Mat mat = m_runtimes[i];
                        if (mat.material != material)
                        {
                            bDirty = true;
                            if (mat.instanceNew)
                            {
                                MaterialLibrarys.RecyleCloneMaterial(mat.material);
                            }
                            mat.instanceNew = bAutoDestroy;
                            mat.material = material;
                            m_runtimes[i] = mat;
                        }
                    }
                }
                else if (index >= m_runtimes.Count)
                {
                    for (int i = 0; i < m_runtimes.Count; ++i)
                    {
                        if (m_runtimes[i].material == material)
                        {
                            Mat mat = m_runtimes[i];
                            if (mat.instanceNew != bAutoDestroy)
                            {
                                mat.instanceNew = bAutoDestroy;
                                m_runtimes[i] = mat;
                            }
                            return false;
                        }
                    }
                    m_runtimes.Add(new Mat() { material = material, instanceNew = bAutoDestroy }); ;
                    bDirty = true;
                }
                if (bDirty)
                    Applay(true);
            }
            return bDirty;
        }
        //------------------------------------------------------
        void Applay(bool bShare = true, Material[] restoreMaterials = null)
        {
            if (m_runtimes == null || m_runtimes.Count <= 0)
                return;
            if(restoreMaterials!=null)
            {
                if (m_renderer)
                {
                    if (restoreMaterials.Length == 1)
                    {
                        if (bShare)
                        {
                            m_renderer.sharedMaterial = restoreMaterials[0];
                        }
                        else
                        {
                            m_renderer.material = restoreMaterials[0];
                        }
                    }
                    else
                    {
                        if (bShare)
                        {
                            m_renderer.sharedMaterials = restoreMaterials;
                        }
                        else
                        {
                            m_renderer.materials = restoreMaterials;
                        }
                    }
                }
                return;
            }
            if(m_runtimes.Count <= 1)
            {
                if (m_renderer)
                {
                    if (bShare)
                    {
                        m_renderer.sharedMaterial = m_runtimes[0].material;
                    }
                    else
                    {
                        m_renderer.material = m_runtimes[0].material;
                    }
                }
            }
            else
            {
                restoreMaterials = MaterialLibrarys.ConvertListToArray(m_runtimes);
                if (restoreMaterials != null)
                {
                    if (m_renderer)
                    {
                        if (bShare)
                        {
                            m_renderer.sharedMaterials = restoreMaterials;
                        }
                        else
                        {
                            m_renderer.materials = restoreMaterials;
                        }
                    }
                    MaterialLibrarys.ClearConvertTempArray(restoreMaterials);
                }
            }
        }
        //------------------------------------------------------
        internal void Restore(bool bDestroy = false)
        {
            MaterailBlockUtil.ClearBlock(m_renderer);
            if (m_initCnt < 0) return;
            bool bDirty = false;

            Material[] restoreMaterials = null;
            if (bDestroy)
            {
                if (m_runtimes != null)
                {
                    Mat mat;
                    for (int i = 0; i < m_runtimes.Count; ++i)
                    {
                        mat = m_runtimes[i];
                        mat.Destroy();
                    }
                    bDirty = true;
                }
                restoreMaterials = m_shares;
            }
            else
            {
                if (m_runtimes != null)
                {
                    Mat mat;
                    for (int i = 0; i < m_runtimes.Count;)
                    {
                        mat = m_runtimes[i];
                        if (keyWorlds != null)
                        {
                            for (int j = 0; j < keyWorlds.Count; ++j)
                            {
                                if (!string.IsNullOrEmpty(keyWorlds[j].keyWorld))
                                {
                                    if (mat.material) mat.material.DisableKeyword(keyWorlds[j].keyWorld);
                                }
                            }
                        }
                        if (i >= m_initCnt)
                        {
                            if (mat.instanceNew)
                            {
                                MaterialLibrarys.RecyleCloneMaterial(mat.material);
                            }
                            mat.DelToMaterial();
                            mat.DelFromMaterial();
                            m_runtimes.RemoveAt(i);
                            bDirty = true;
                        }
                        else
                        {
                            if (m_shares!=null && mat.material && i < m_shares.Length &&  mat.material.shader != m_shares[i].shader)
                            {
                                MaterialLibrarys.RecyleCloneMaterial(mat.material);
                                mat.material = m_shares[i];
                                mat.instanceNew = false;
                            }
                            mat.DelToMaterial();
                            m_runtimes[i] = mat;
                            bDirty = true;
                            ++i;
                        }
                    }
                }
                if(m_shares!=null)
                {
                    Mat mat;
                    for (int i = 0; i < m_runtimes.Count; ++i)
                    {
                        mat = m_runtimes[i];
                        if (mat.instanceNew && mat.material && i < m_shares.Length) mat.material.CopyPropertiesFromMaterial(m_shares[i]);
                    }
                }
            }

            if (bDirty)
                Applay(true, restoreMaterials);
            if (keyWorlds != null) keyWorlds.Clear();
            if(bDestroy)
            {
                m_runtimes.Clear();
                m_initCnt = 0;
            }
        }
        //------------------------------------------------------
        internal void Backup(Renderer render)
        {
            m_renderer = render;
            m_initCnt = -1;
            if (m_runtimes != null)
            {
                for (int i = 0; i < m_runtimes.Count; ++i)
                    m_runtimes[i].Destroy();
                m_runtimes.Clear();
            }
            if (keyWorlds != null) keyWorlds.Clear();
        }
        //------------------------------------------------------
        internal void Clear()
        {
            m_renderer = null;
            m_initCnt = -1;
            if (m_runtimes != null)
            {
                for (int i = 0; i < m_runtimes.Count; ++i)
                    m_runtimes[i].Destroy();
                m_runtimes.Clear();
            }
            if (keyWorlds != null) keyWorlds.Clear();
        }
        //------------------------------------------------------
        public void LerpToMaterial(Material material, float fTime, int index = -1, float fKeepTime = 0, string fPropertyName = null, bool bAutoDestroy = true)
        {
            if (material == null) return;
            if (m_renderer == null) return;
            CheckRuntime();
            if (m_runtimes == null) return;

            if (index < 0)
            {
                Mat mat;
                for (int j = 0; j < GetCount(); ++j)
                {
                    GetRuntime(j, true);
                    mat = m_runtimes[j];

                    if (mat.toMaterial != material)
                    {
                        mat.DelToMaterial();
                    }
                    mat.toMaterial = material;
                    if(mat.material)
                    {
                        if (mat.fromMaterial == null)
                            mat.fromMaterial = MaterialLibrarys.GetInstanceMaterial(mat.material);
                        else
                            mat.fromMaterial.CopyPropertiesFromMaterial(mat.material);
                    }

                    mat.lerpDuration = fTime;
                    if(fKeepTime>0) mat.keepTime = fKeepTime + Time.time;
                    else mat.keepTime = 0;
                    mat.lerpStartTime = Time.time;
                    mat.fadeMode = 1;
                    m_runtimes[j] = mat;
                }
                Applay();
            }
            else if (index >= 0 && index < GetCount())
            {
                GetRuntime(index, true);
                Mat mat = m_runtimes[index];
                if (mat.toMaterial != material)
                {
                    mat.DelToMaterial();
                }
                mat.toMaterial = material;
                if(mat.material)
                {
                    if (mat.fromMaterial == null)
                        mat.fromMaterial = MaterialLibrarys.GetInstanceMaterial(mat.material);
                    else
                        mat.fromMaterial.CopyPropertiesFromMaterial(mat.material);
                }

                mat.lerpDuration = fTime;
                mat.lerpStartTime = Time.time;
                mat.fadeMode = 1;
                if (fKeepTime > 0) mat.keepTime = fKeepTime + Time.time;
                else mat.keepTime = 0;
                m_runtimes[index] = mat;
                Applay();
            }
        }
        //------------------------------------------------------
        bool UpdateLerpMaterial()
        {
            if (m_renderer == null)
            {
                return false;
            }
            if (m_runtimes == null) return false;

            float fTime = Time.time;

            for(int i =0; i < m_runtimes.Count; ++i)
            {
                Mat mat = m_runtimes[i];
                if (mat.fromMaterial && mat.toMaterial)
                {
                    float factor = 1;
                    if(mat.lerpDuration>0) factor = Mathf.Clamp01((fTime-mat.lerpStartTime) /mat.lerpDuration);
                    if(mat.material)
                    {
                        if (mat.fadeMode == 1)
                            mat.material.Lerp(mat.fromMaterial, mat.toMaterial, factor);
                        else
                            mat.material.Lerp(mat.toMaterial, mat.fromMaterial, factor);
                    }

                    if (mat.keepTime>0 || mat.fadeMode >= 2)
                    {
                        if(factor>=1)
                        {
                            if(fTime >= mat.keepTime)
                            {
                                if(mat.fadeMode <= 1)
                                    mat.fadeMode = 2;
                                else
                                {
                                    //! over
                                    mat.DelToMaterial();
                                }
                                m_runtimes[i] = mat;
                            }
                        }
                    }
                }
            }
            return true;
        }
        //------------------------------------------------------
        public void FadeoutLerp()
        {
            if (m_runtimes == null) return;
            float fTime = Time.time;
            for(int i =0; i < m_runtimes.Count; ++i)
            {
                Mat mat =  m_runtimes[i];
                if(mat.fadeMode != 2)
                {
                    mat.fadeMode = 2;
                    mat.lerpStartTime = fTime;
                    m_runtimes[i] = mat;
                }
            }
        }
        //------------------------------------------------------
        public void Update()
        {
            UpdateLerpMaterial();
        }
    }
    public class MaterialLibrarys
    {
        static int MAX_STACK_POOL = 32;
        static Stack<LibRender> ms_vLibrayPools = new Stack<LibRender>(32);
        static Dictionary<int, Material[]> ms_ConvertTemp = new Dictionary<int, Material[]>(2);
        static Material ms_UIMatrial = null;
        //------------------------------------------------------
        public static Material[] ConvertListToArray(List<Material> vMats)
        {
            int validCnt = 0;
            for (int i = 0; i < vMats.Count; ++i)
            {
                if (vMats[i] != null) validCnt++;
            }
            if (validCnt <= 0) return null;
            Material[] mats;
            if (!ms_ConvertTemp.TryGetValue(validCnt, out mats))
            {
                mats = new Material[validCnt];
                ms_ConvertTemp[validCnt] = mats;
            }
            validCnt = 0;
            for (int i = 0; i < vMats.Count; ++i)
            {
                if (vMats[i] != null) mats[validCnt++] = vMats[i];
            }
            return mats;
        }
        //------------------------------------------------------
        public static Material[] ConvertListToArray(List<LibRender.Mat> vMats)
        {
            int validCnt = 0;
            for (int i = 0; i < vMats.Count; ++i)
            {
                if (vMats[i].material != null) validCnt++;
            }
            if (validCnt <= 0) return null;
            Material[] mats;
            if (!ms_ConvertTemp.TryGetValue(validCnt, out mats))
            {
                mats = new Material[validCnt];
                ms_ConvertTemp[validCnt] = mats;
            }
            validCnt = 0;
            for (int i = 0; i < vMats.Count; ++i)
            {
                if (vMats[i].material != null) mats[validCnt++] = vMats[i].material;
            }
            return mats;
        }
        //------------------------------------------------------
        public static void ClearConvertTempArray(Material[] mats)
        {
            if (mats == null) return;
            for (int i = 0; i < mats.Length; ++i)
                mats[i] = null;
        }
        //------------------------------------------------------
        public static Material GetInstanceMaterial(Material material)
        {
            Material instance = null;
            //             List<Material> cloneList;
            //             if (m_vInstanceMaterial.TryGetValue(material, out cloneList) && cloneList.Count>0)
            //             {
            //                 instance = cloneList[0];
            //                 cloneList.RemoveAt(0);
            //             }
            instance = new Material(material);
            instance.hideFlags |= HideFlags.DontSave;
#if UNITY_EDITOR
            instance.name = material.name + "_instance";
#endif
            return instance;
        }
        //------------------------------------------------------
        public static void RecyleCloneMaterial(Material clone)
        {
            if (clone)
                GameObject.DestroyImmediate(clone);
        }
        //------------------------------------------------------
        public static void RecycleLibray(LibRender lib)
        {
            lib.Clear();
            if (ms_vLibrayPools.Count < MAX_STACK_POOL)
                ms_vLibrayPools.Push(lib);
        }
        //------------------------------------------------------
        public static LibRender NewLibray()
        {
            if (ms_vLibrayPools.Count > 0) return ms_vLibrayPools.Pop();
            return new LibRender();
        }
        //------------------------------------------------------
        public static Material CloneUIMaterial()
        {
            if (ms_UIMatrial == null)
            {
                ms_UIMatrial = new Material(Shader.Find("UI/Default"));
                ms_UIMatrial.hideFlags |= HideFlags.DontSave;
#if UNITY_EDITOR
                ms_UIMatrial.name = "UIDefault_instance";
#endif
            }
            return ms_UIMatrial;
        }
        //------------------------------------------------------
        public static void EnableKeyWorld(List<LibRender> libRenders, string keyWorld, bool bEnable, int index = -1)
        {
            if (libRenders == null) return;
            if (string.IsNullOrEmpty(keyWorld)) return;
            LibRender library;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                library = libRenders[i];
                if (index >= 0)
                {
                    Material material = library.GetRuntime(index, true);
                    if (material)
                    {
                        if (bEnable)
                        {
                            library.Addkey(keyWorld, index);
                            material.EnableKeyword(keyWorld);
                        }
                        else
                        {
                            library.Delkey(keyWorld, index);
                            material.DisableKeyword(keyWorld);
                        }
                    }
                }
                else
                {
                    for (int mtIndex = 0; mtIndex < library.GetCount(); ++mtIndex)
                    {
                        Material material = library.GetRuntime(mtIndex, true);
                        if (material)
                        {
                            if (bEnable)
                            {
                                library.Addkey(keyWorld, mtIndex);
                                material.EnableKeyword(keyWorld);
                            }
                            else
                            {
                                library.Delkey(keyWorld, mtIndex);
                                material.DisableKeyword(keyWorld);
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public static void SetMaterial(List<LibRender> libRenders, Material pMaterial, int index = -1, bool bAutoDestroy = true)
        {
            LerpToMaterial(libRenders, pMaterial, 0, index, 0, null, bAutoDestroy);
        }
        //------------------------------------------------------
        public static void LerpToMaterial(List<LibRender> libRenders, Material material, float fTime, int index = -1, float fKeepTime = 0, string fPropertyName = null, bool bAutoDestroy = true)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                libRenders[i].LerpToMaterial(material, fTime, index, fKeepTime, fPropertyName, bAutoDestroy);
            }
        }
        //------------------------------------------------------
        public static void FadeoutLerpMaterial(List<LibRender> libRenders)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                libRenders[i].FadeoutLerp();
            }
        }
        //------------------------------------------------------
        public static void SetRenderInt(LibRender render, string propName, int propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null) return;
            int propKey = MaterailBlockUtil.BuildPropertyID(propName);
            if (propKey == 0) return;
            if (index < 0)
            {
                for (int i = 0; i < render.GetCount(); ++i)
                {
                    Material material = render.GetRuntime(i, true);
                    if (material) material.SetInt(propKey, propValue);
                    // MaterailBlockUtil.SetRenderInt(render.renderer, propName, propValue, bBlock, bShare, i);
                }
            }
            else
            {
                Material material = render.GetRuntime(index, true);
                if (material) material.SetInt(propKey, propValue);
                //MaterailBlockUtil.SetRenderInt(render.renderer, propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderInt(List<LibRender> libRenders, string propName, int propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                SetRenderInt(libRenders[i], propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderFloat(LibRender render, string propName, float propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null) return;
            int propKey = MaterailBlockUtil.BuildPropertyID(propName);
            if (propKey == 0) return;

            if (index < 0)
            {
                for (int i = 0; i < render.GetCount(); ++i)
                {
                    Material material = render.GetRuntime(i, true);
                    if (material) material.SetFloat(propKey, propValue);
                    // MaterailBlockUtil.SetRenderFloat(render.renderer, propName, propValue, bBlock, bShare, i);
                }
            }
            else
            {
                Material material = render.GetRuntime(index, true);
                if (material) material.SetFloat(propKey, propValue);
                //  MaterailBlockUtil.SetRenderFloat(render.renderer, propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderFloat(List<LibRender> libRenders, string propName, float propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                SetRenderFloat(libRenders[i], propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderColor(LibRender render, string propName, Color propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null) return;
            int propKey = MaterailBlockUtil.BuildPropertyID(propName);
            if (propKey == 0) return;

            if (index < 0)
            {
                for (int i = 0; i < render.GetCount(); ++i)
                {
                    Material material = render.GetRuntime(i, true);
                    if (material) material.SetColor(propKey, propValue);
                  //      MaterailBlockUtil.SetRenderColor(render.renderer, propName, propValue, bBlock, bShare, i);
                }
            }
            else
            {
                Material material = render.GetRuntime(index, true);
                if (material) material.SetColor(propKey, propValue);
                    //MaterailBlockUtil.SetRenderColor(render.renderer, propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderColor(List<LibRender> libRenders, string propName, Color propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                SetRenderColor(libRenders[i], propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderVector(LibRender render, string propName, Vector4 propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null) return;
            int propKey = MaterailBlockUtil.BuildPropertyID(propName);
            if (propKey == 0) return;
            if (index < 0)
            {
                for (int i = 0; i < render.GetCount(); ++i)
                {
                    Material material = render.GetRuntime(i, true);
                    if (material) material.SetVector(propKey, propValue);// MaterailBlockUtil.SetRenderVector(render.renderer, propName, propValue, bBlock, bShare, i);
                }
            }
            else
            {
                Material material = render.GetRuntime(index, true);
                if (material) material.SetVector(propKey, propValue);
               // MaterailBlockUtil.SetRenderVector(render.renderer, propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderVector(List<LibRender> libRenders, string propName, Vector4 propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (libRenders == null) return;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                SetRenderVector(libRenders[i], propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderTexture(LibRender render, string propName, Texture propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null) return;
            int propKey = MaterailBlockUtil.BuildPropertyID(propName);
            if (propKey == 0) return;
            if (index < 0)
            {
                for (int i = 0; i < render.GetCount(); ++i)
                {
                    Material material = render.GetRuntime(i, true);
                    if (material) material.SetTexture(propKey, propValue);
                    //MaterailBlockUtil.SetTexture(render.renderer, propName, propValue, bBlock, bShare, i);
                }
            }
            else
            {
                Material material = render.GetRuntime(index, true);
                if (material) material.SetTexture(propKey, propValue);
                //   MaterailBlockUtil.SetTexture(render.renderer, propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void SetRenderTexture(List<LibRender> libRenders, string propName, Texture propValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (libRenders == null) return;
            bBlock = false;
            bShare = true;
            for (int i = 0; i < libRenders.Count; ++i)
            {
                SetRenderTexture(libRenders[i], propName, propValue, bBlock, bShare, index);
            }
        }
        //------------------------------------------------------
        public static void ReplaceShader(List<LibRender> Libs, string name, int index = -1)
        {
            if (Libs == null) return;
            if (string.IsNullOrEmpty(name)) return;
            if (!AShadersTabs.IsValid()) return;
            LibRender library;
            for (int i = 0; i < Libs.Count; ++i)
            {
                library = Libs[i];
                if (index >= 0)
                {
                    Material material = library.GetRuntime(index, true);
                    if (material != null)
                    {
                        if (material.shader == null) continue;
                        AShadersTabs.Item shader = AShadersTabs.FindShader(material.shader.name, name);
                        if (shader == null) continue;
                        if (shader.shader != null || shader.keyWorlds != null)
                        {
                            if (shader.keyWorlds != null)
                            {
                                for (int j = 0; j < shader.keyWorlds.Length; ++j)
                                {
                                    if (string.IsNullOrEmpty(shader.keyWorlds[j])) continue;
                                    library.Addkey(shader.keyWorlds[j], j);
                                    material.EnableKeyword(shader.keyWorlds[j]);
                                }
                            }

                            if (shader.shader != null && material.shader != shader.shader)
                            {
                                material.shader = shader.shader;
                            }
                        }
                    }
                }
                else
                {
                    for (int mtIndex = 0; mtIndex < library.GetCount(); ++mtIndex)
                    {
                        Material material = library.GetRuntime(mtIndex, true);
                        if (material != null)
                        {
                            if (material.shader == null) continue;
                            AShadersTabs.Item shader = AShadersTabs.FindShader(material.shader.name, name);
                            if (shader == null) continue;
                            if (shader.shader != null || shader.keyWorlds != null)
                            {
                                if (shader.keyWorlds != null)
                                {
                                    for (int j = 0; j < shader.keyWorlds.Length; ++j)
                                    {
                                        if (string.IsNullOrEmpty(shader.keyWorlds[j])) continue;
                                        library.Addkey(shader.keyWorlds[j], j);
                                        material.EnableKeyword(shader.keyWorlds[j]);
                                    }
                                }

                                if (shader.shader != null && material.shader != shader.shader)
                                {
                                    material.shader = shader.shader;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
