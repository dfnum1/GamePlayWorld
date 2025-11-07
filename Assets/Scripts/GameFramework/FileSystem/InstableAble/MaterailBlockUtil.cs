using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class MaterailBlockUtil
    {
        static int ms_ProjectorColorID = 0;
        public static int ProjectorColorID
        {
            get
            {
                if (ms_ProjectorColorID == 0)
                    ms_ProjectorColorID = BuildPropertyID("_ProjectorColor");
                return ms_ProjectorColorID;
            }
        }
        static MaterialPropertyBlock m_propEmptyBlock;
        static MaterialPropertyBlock m_propBlock;
        static MaterialPropertyBlock m_bindPropBlock;
        static Dictionary<string, int> m_dictPropBlock = new Dictionary<string, int>();
        //------------------------------------------------------
        public static void BindBlock(MaterialPropertyBlock propertyBlock)
        {
            m_bindPropBlock = propertyBlock;
        }
        //------------------------------------------------------
        public static void UnBindBlock()
        {
            m_bindPropBlock = null;
        }
        //------------------------------------------------------
        public static MaterialPropertyBlock GetBlockProp()
        {
            if (m_bindPropBlock != null) return m_bindPropBlock;
            if (m_propBlock == null) m_propBlock = new MaterialPropertyBlock();
            return m_propBlock;
        }
        //------------------------------------------------------
        public static MaterialPropertyBlock GetEmptyBlockProp()
        {
            if (m_propEmptyBlock == null) m_propEmptyBlock = new MaterialPropertyBlock();
            return m_propEmptyBlock;
        }
        //------------------------------------------------------
        public static int BuildPropertyID(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return 0;
            int id = 0;
            if (m_dictPropBlock.TryGetValue(propertyName, out id)) return id;
            id = Shader.PropertyToID(propertyName);
            m_dictPropBlock.Add(propertyName, id);
            return id;
        }
        //------------------------------------------------------
        public static Material[] GetMaterials(Renderer render, bool bShare)
        {
            if (render == null) return null;
            if (bShare) return render.sharedMaterials;
            return render.materials;
        }
        //------------------------------------------------------
        public static void SetGlobalFloat(string propertyName, float fValue)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalFloat(propertyName, fValue);
        }
        //------------------------------------------------------
        public static void SetGoFloat(GameObject go, string propertyName, float param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!go) return;

            Renderer render = go.GetComponent<Renderer>();
            if (!render)
                render = go.GetComponent<SkinnedMeshRenderer>();
            SetRenderFloat(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetTransformFloat(Transform trans, string propertyName, float param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!trans) return;

            Renderer render = trans.GetComponent<Renderer>();
            if (!render)
                render = trans.GetComponent<SkinnedMeshRenderer>();
            SetRenderFloat(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRenderFloat(Renderer render, string propertyName, float param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!render || string.IsNullOrEmpty(propertyName)) return;

            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);
                if (index >= 0)
                {
                    if (index < materials.Length && materials[index])
                        materials[index].SetFloat(propertyId, param);
                }
                else
                {
                    for (int i = 0; i < materials.Length; ++i)
                    {
                        if (materials[i]) materials[i].SetFloat(propertyId, param);
                    }
                }
                return;
            }
            MaterialPropertyBlock block = GetBlockProp();
            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetFloat(propertyId, param);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetFloat(propertyId, param);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetRendersFloat(Renderer[] renderers, string propertyName, float param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers) return;
            for (int i = 0; i < renderers.Length; ++i)
                SetRenderFloat(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRendersFloat(List<Renderer> renderers, string propertyName, float param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers) return;
            for (int i = 0; i < renderers.Count; ++i)
                SetRenderFloat(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetGlobalInt(string propertyName, int nValue)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalInt(propertyName, nValue);
        }
        //------------------------------------------------------
        public static void SetGoInt(GameObject go, string propertyName, int param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!go || string.IsNullOrEmpty(propertyName)) return;

            Renderer render = go.GetComponent<Renderer>();
            if (!render)
                render = go.GetComponent<SkinnedMeshRenderer>();
            SetRenderInt(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetTransformInt(Transform trans, string propertyName, int param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!trans || string.IsNullOrEmpty(propertyName)) return;

            Renderer render = trans.GetComponent<Renderer>();
            if (!render)
                render = trans.GetComponent<SkinnedMeshRenderer>();
            SetRenderInt(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRenderInt(Renderer render, string propertyName, int param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!render || string.IsNullOrEmpty(propertyName)) return;

            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);
                if (index >= 0)
                {
                    if (index < materials.Length && materials[index])
                    {
                        materials[index].SetInt(propertyId, param);
                    }
                }
                else
                {
                    for (int i = 0; i < materials.Length; ++i)
                    {
                        if (materials[i]) materials[i].SetInt(propertyId, param);
                    }
                }
                return;
            }
            MaterialPropertyBlock block = GetBlockProp();

            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetInt(propertyId, param);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetInt(propertyId, param);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetRendersInt(Renderer[] renderers, string propertyName, int param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers) return;
            for (int i = 0; i < renderers.Length; ++i)
                SetRenderInt(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRendersInt(List<Renderer> renderers, string propertyName, int param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers) return;
            for (int i = 0; i < renderers.Count; ++i)
                SetRenderInt(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetGlobalVector(string propertyName, Vector4 param)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalVector(propertyName, param);
        }
        //------------------------------------------------------
        public static void SetGoVector(GameObject go, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (go == null || string.IsNullOrEmpty(propertyName)) return;

            Renderer render = go.GetComponent<Renderer>();
            if (!render)
                render = go.GetComponent<SkinnedMeshRenderer>();
            SetRenderVector(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetTransformVector(Transform transform, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (transform == null || string.IsNullOrEmpty(propertyName)) return;

            Renderer render = transform.GetComponent<Renderer>();
            if (!render)
                render = transform.GetComponent<SkinnedMeshRenderer>();
            SetRenderVector(render, propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRenderVector(Renderer render, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);
            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);
                if (index >= 0)
                {
                    if (materials != null && index < materials.Length && materials[index] != null)
                    {
                        materials[index].SetVector(propertyId, param);
                    }
                }
                else
                {
                    if (materials != null)
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i])
                                materials[i].SetVector(propertyId, param);
                        }
                    }
                }
                return;
            }
            MaterialPropertyBlock block = GetBlockProp();

            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetVector(propertyId, param);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetVector(propertyId, param);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetRendersVector(Renderer[] renderers, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers || string.IsNullOrEmpty(propertyName)) return;
            for (int i = 0; i < renderers.Length; ++i)
                SetRenderVector(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRendersVector(List<Renderer> renderers, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (null == renderers || string.IsNullOrEmpty(propertyName)) return;
            for (int i = 0; i < renderers.Count; ++i)
                SetRenderVector(renderers[i], propertyName, param, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static Vector4 GetGoVector(GameObject target, string propertyName)
        {
            if (target == null || string.IsNullOrEmpty(propertyName)) return Vector4.zero;
            return GetRenderVector(target.GetComponent<MeshRenderer>(), propertyName);
        }
        //------------------------------------------------------
        public static Vector4 GetTransformVector(Transform target, string propertyName)
        {
            if (target == null || string.IsNullOrEmpty(propertyName)) return Vector4.zero;
            return GetRenderVector(target.GetComponent<MeshRenderer>(), propertyName);
        }
        //------------------------------------------------------
        public static Vector4 GetRenderVector(Renderer render, string propertyName)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return Vector4.zero;
            MaterialPropertyBlock block = GetBlockProp();

            int propertyId = BuildPropertyID(propertyName);
            render.GetPropertyBlock(block);
            return block.GetVector(propertyId);
        }
        //------------------------------------------------------
        public static void SetGlobalVectorArray(string propertyName, List<Vector4> values)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalVectorArray(propertyName, values);
        }
        //------------------------------------------------------
        public static void SetGoVectorArray(GameObject target, string propertyName, List<Vector4> values, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!target) return;
            SetRenderVectorArray(target.GetComponent<Renderer>(), propertyName, values, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetRenderVectorArray(Renderer render, string propertyName, List<Vector4> values, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);
            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);
                if (index >= 0)
                {
                    if (materials != null && index < materials.Length)
                    {
                        if (materials[index])
                            materials[index].SetVectorArray(propertyId, values);
                    }
                }
                else
                {
                    if (materials != null)
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i])
                                materials[i].SetVectorArray(propertyId, values);
                        }
                    }
                }
                return;
            }

            MaterialPropertyBlock block = GetBlockProp();

            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetVectorArray(propertyId, values);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetVectorArray(propertyId, values);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetGlobalVectorArray(string propertyName, Matrix4x4 param)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalMatrix(propertyName, param);
        }
        //------------------------------------------------------
        public static void SetMatrix(Renderer render, string propertyName, Matrix4x4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);
            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);
                if (materials != null)
                {
                    if (index >= 0)
                    {
                        if (index < materials.Length && materials[index])
                        {
                            materials[index].SetMatrix(propertyId, param);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i]) materials[i].SetMatrix(propertyId, param);
                        }
                    }
                }
                return;
            }
            MaterialPropertyBlock block = GetBlockProp();

            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetMatrix(propertyId, param);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetMatrix(propertyId, param);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetGlobalColor(string propertyName, Color param)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalColor(propertyName, param);
        }
        //------------------------------------------------------
        public static void SetTextureScale(Renderer render, string propertyName, Vector4 param, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (!render || string.IsNullOrEmpty(propertyName)) return;


            int propertyId = BuildPropertyID(propertyName);

            Material[] materials = GetMaterials(render, bShare);

            if (materials != null)
            {
                if (index >= 0)
                {
                    if (index < materials.Length)
                    {
                        if (materials[index])
                            materials[index].SetTextureScale(propertyId, param);
                    }
                }
                else
                {
                    for (int i = 0; i < materials.Length; ++i)
                    {
                        if (materials[i])
                            materials[i].SetTextureScale(propertyId, param);
                    }
                }
            }
        }
        //------------------------------------------------------
        public static void SetGoColor(GameObject go, string propertyName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (go == null || string.IsNullOrEmpty(propertyName)) return;

            MaterialPropertyBlock block = GetBlockProp();

            int propertyId = BuildPropertyID(propertyName);

            Renderer render = go.GetComponent<Renderer>();
            if (render == null) return;

            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);

                if (materials != null)
                {
                    if (index >= 0)
                    {
                        if (index < materials.Length)
                        {
                            if (materials[index]) materials[index].SetColor(propertyId, color);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i]) materials[i].SetColor(propertyId, color);
                        }
                    }
                }
                return;
            }
            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetColor(propertyId, color);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetColor(propertyId, color);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetTransformColor(Transform transform, string propertyName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (transform == null) return;
            SetGoColor(transform.gameObject, propertyName, color, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public static void SetGlobalTexture(string propertyName, Texture param)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            int propId = BuildPropertyID(propertyName);

            Shader.SetGlobalTexture(propertyName, param);
        }
        //------------------------------------------------------
        public static void SetTexture(Renderer render, string propertyName, Texture tex, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);

                if (materials != null)
                {
                    if (index >= 0)
                    {
                        if (index < materials.Length)
                        {
                            if (materials[index]) materials[index].SetTexture(propertyId, tex);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i]) materials[i].SetTexture(propertyId, tex);
                        }
                    }
                }
                return;
            }

            MaterialPropertyBlock block = GetBlockProp();
            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetTexture(propertyId, tex);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetTexture(propertyId, tex);
                render.SetPropertyBlock(block);
            }
        }
        //------------------------------------------------------
        public static void SetRenderColor(Renderer render, string propertyName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (render == null || string.IsNullOrEmpty(propertyName)) return;

            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials = GetMaterials(render, bShare);

                if (materials != null)
                {
                    if (index >= 0)
                    {
                        if (index < materials.Length)
                        {
                            if (materials[index]) materials[index].SetColor(propertyId, color);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            if (materials[i]) materials[i].SetColor(propertyId, color);
                        }
                    }
                }
                return;
            }
            MaterialPropertyBlock block = GetBlockProp();

            if (index >= 0)
            {
                render.GetPropertyBlock(block, index);
                block.SetColor(propertyId, color);
                render.SetPropertyBlock(block, index);
            }
            else
            {
                render.GetPropertyBlock(block);
                block.SetColor(propertyId, color);
                render.SetPropertyBlock(block);
            }

        }
        //------------------------------------------------------
        public static void SetRendersColor(Renderer[] renders, string propertyName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (renders == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials;
                for (int i = 0; i < renders.Length; ++i)
                {
                    if (bShare)
                    {
                        materials = renders[i].sharedMaterials;
                    }
                    else
                    {
                        materials = renders[i].materials;
                    }
                    if (materials != null)
                    {
                        if (index >= 0)
                        {
                            if (index < materials.Length)
                            {
                                if (materials[index]) materials[index].SetColor(propertyId, color);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < materials.Length; ++j)
                            {
                                if (materials[j]) materials[j].SetColor(propertyId, color);
                            }
                        }
                    }
                }
                return;
            }

            MaterialPropertyBlock block = GetBlockProp();

            for (int i = 0; i < renders.Length; ++i)
            {
                if (renders[i])
                {
                    if (index >= 0)
                    {
                        renders[i].GetPropertyBlock(block, index);
                        block.SetColor(propertyId, color);
                        renders[i].SetPropertyBlock(block, index);
                    }
                    else
                    {
                        renders[i].GetPropertyBlock(block);
                        block.SetColor(propertyId, color);
                        renders[i].SetPropertyBlock(block);
                    }
                }
            }
        }
        //------------------------------------------------------
        public static void SetRendersColor(List<Renderer> renders, string propertyName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            if (renders == null || string.IsNullOrEmpty(propertyName)) return;
            int propertyId = BuildPropertyID(propertyName);

            if (!bBlock)
            {
                Material[] materials;
                for (int i = 0; i < renders.Count; ++i)
                {
                    materials = GetMaterials(renders[i], bShare);

                    if (materials != null)
                    {
                        if (index >= 0)
                        {
                            if (index < materials.Length)
                            {
                                if (materials[index]) materials[index].SetColor(propertyId, color);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < materials.Length; ++j)
                            {
                                if (materials[j]) materials[j].SetColor(propertyId, color);
                            }
                        }
                    }
                }
                return;
            }

            MaterialPropertyBlock block = GetBlockProp();

            for (int i = 0; i < renders.Count; ++i)
            {
                if (renders[i])
                {
                    if (index >= 0)
                    {
                        renders[i].GetPropertyBlock(block, index);
                        block.SetColor(propertyId, color);
                        renders[i].SetPropertyBlock(block, index);
                    }
                    else
                    {
                        renders[i].GetPropertyBlock(block);
                        block.SetColor(propertyId, color);
                        renders[i].SetPropertyBlock(block);
                    }
                }
            }
        }
        //------------------------------------------------------
        public static void ClearBlock(Renderer render)
        {
            if (render != null)
            {
                if (render.HasPropertyBlock())
                {
                    int cnt = render.sharedMaterials.Length;
                    for (int i = 0; i < cnt; ++i)
                        render.SetPropertyBlock(null, i);
                }
            }
        }
        //------------------------------------------------------
        public static void ClearBlock(Renderer[] renders)
        {
            if (renders == null) return;
            for (int i = 0; i < renders.Length; ++i)
                ClearBlock(renders[i]);
        }
        //------------------------------------------------------
        public static void ClearBlock(List<Renderer> renders)
        {
            if (renders == null) return;
            for (int i = 0; i < renders.Count; ++i)
                ClearBlock(renders[i]);
        }
        //------------------------------------------------------
        public static void ClearBlock(GameObject go)
        {
            if (go != null)
            {
                Renderer render = go.GetComponent<Renderer>();
                ClearBlock(render);
            }
        }
        //------------------------------------------------------
        public static void ClearRendersBlock(List<Renderer> renders)
        {
            if (renders == null) return;
            for (int i = 0; i < renders.Count; ++i)
                ClearBlock(renders[i]);
        }
        //------------------------------------------------------
        public static void ClearaGOBlock(GameObject go)
        {
            ClearBlock(go);
        }
        //------------------------------------------------------
        public static void SetGlobalLOD(int lod)
        {
            Shader.globalMaximumLOD = lod;
        }
        //------------------------------------------------------
        public static void SetFloat(Material material, string strProperty, float fValue)
        {
            if (string.IsNullOrEmpty(strProperty) || material ==null) return;
            int propertyId = BuildPropertyID(strProperty);
            if (!material.HasProperty(propertyId)) return;
            material.SetFloat(propertyId, fValue);
        }
        //------------------------------------------------------
        public static void SetVector4(Material material, string strProperty, Vector4 fValue)
        {
            if (string.IsNullOrEmpty(strProperty) || material == null) return;
            int propertyId = BuildPropertyID(strProperty);
            if (!material.HasProperty(propertyId)) return;
            material.SetVector(propertyId, fValue);
        }
        //------------------------------------------------------
        public static void SetColor(Material material, string strProperty, Color fValue)
        {
            if (string.IsNullOrEmpty(strProperty) || material == null) return;
            int propertyId = BuildPropertyID(strProperty);
            if (!material.HasProperty(propertyId)) return;
            material.SetColor(propertyId, fValue);
        }
        //------------------------------------------------------
        public static void SetTexture(Material material, string strProperty, Texture fValue)
        {
            if (string.IsNullOrEmpty(strProperty) || material == null) return;
            int propertyId = BuildPropertyID(strProperty);
            if (!material.HasProperty(propertyId)) return;
            material.SetTexture(propertyId, fValue);
        }
    }
}
