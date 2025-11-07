#if !USE_SERVER
using Framework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin
{
    public class CGpuSkinMeshAgent : IUserData
    {
        public string gpuAssetPath;
        public string cpuAssetPath;

        public Texture gpuTexture;

        public Texture cpuTexture;

        private byte m_nSkinIndex = 0;
        ESkinType m_SkinType = ESkinType.CpuData;
        private SkinFrameData m_FrameData;
        private IUserData m_pOwner = null;
        private AWorldNode m_pNode = null;
        public SkinFrameData frameData
        {
            get { return m_FrameData; }
        }
        public ESkinType frameDataType
        {
            get { return m_SkinType; }
        }
        private float m_fPlayTime = 0f;
        private int m_nCurIndex = 0;
        public int curIndex
        {
            get { return m_nCurIndex; }
        }
        private float m_fSpeed = 1f;

        int m_nDefaultIndex = -1;
        List<Vector4> m_GpuSkinMatrix = null;

        MaterialPropertyBlock m_Blockproperties;
        public CGpuSkinMeshAgent(IUserData owner)
        {
            m_pOwner = owner;
            if(owner !=null)
            {
                m_pNode = owner as AWorldNode;
            }
            CGpuSharePropertyID.Init();
            m_nSkinIndex = 0;
        }
        //-----------------------------------------------------------------------------
        public bool IsCanDraw()
        {
            if (m_pNode != null) return m_pNode.IsVisible() && !m_pNode.IsDestroy();
            return m_pOwner != null;
        }
        //-----------------------------------------------------------------------------
        public void Destroy()
        {
            if (m_FrameData != null) m_FrameData.release();
            m_FrameData = null;
            m_pOwner = null;
            m_pNode = null;
            m_nDefaultIndex = -1;
        }
        //-----------------------------------------------------------------------------
        public IUserData GetOwner()
        {
            return m_pOwner;
        }
        //-----------------------------------------------------------------------------
        public Vector3 GetPosition()
        {
            if (m_pNode != null)
                return m_pNode.GetPosition();
            return Vector3.zero;
        }
        //-----------------------------------------------------------------------------
        public Vector3 GetEulerAngle()
        {
            if (m_pNode != null) return m_pNode.GetEulerAngle();
            return Vector3.zero;
        }
        //-----------------------------------------------------------------------------
        public Vector3 GetScale()
        {
            if (m_pNode != null) return m_pNode.GetScale();
            return Vector3.one;
        }
        //-----------------------------------------------------------------------------
        public void SetSpeed(float speed)
        {
            m_fSpeed = speed;
        }
        //-----------------------------------------------------------------------------
        public int GetSkin()
        {
            return m_nSkinIndex;
        }
        //-----------------------------------------------------------------------------
        public void SetSkin(byte skinIndex)
        {
            if (m_FrameData == null) return;
            if (m_nSkinIndex != skinIndex)
                m_nSkinIndex = skinIndex;
        }
        //-----------------------------------------------------------------------------
        public Mesh GetShareMesh()
        {
            return m_FrameData.getShareMesh();
        }
        //-----------------------------------------------------------------------------
        public Material GetShareMaterial()
        {
            return m_FrameData.getShareMat();
        }
        //-----------------------------------------------------------------------------
        public MaterialPropertyBlock GetPropertyBlock()
        {
            return m_Blockproperties;
        }
        //-----------------------------------------------------------------------------
        public void SetPlayNormalTime(float time)
        {
            if (m_FrameData == null) return;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

                m_fPlayTime = time * boneAnimation._length;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return;
                m_fPlayTime = time * boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount;
            }
        }
        //-----------------------------------------------------------------------------
        public void SetFixedTime(float time)
        {
            if (m_FrameData == null) return;
            m_fPlayTime = time;
        }
        //-----------------------------------------------------------------------------
        public int GetFrameIndex()
        {
            if (m_FrameData == null) return 0;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (boneFrameDatas.boneAnimations == null) return 0;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return 0;

                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

                int frameIndex = 0;
                if (boneAnimation._loop)
                    frameIndex = (int)(m_fPlayTime * boneAnimation._fps) % (int)(boneAnimation._length * boneAnimation._fps);
                else if (m_fPlayTime >= boneAnimation._length)
                    frameIndex = (int)(boneAnimation._length * boneAnimation._fps);
                else
                    frameIndex = (int)(m_fPlayTime * boneAnimation._fps);
                return frameIndex;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                boneFrameDatas.update();
                if (boneFrameDatas.animations == null || m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return 0;

                int frameIndex = (int)m_fPlayTime;
                if (frameIndex >= boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount)
                {
                    if (boneFrameDatas.animations.animMeshs[m_nCurIndex]._loop)
                    {
                        frameIndex = 0;
                    }
                    else
                    {
                        frameIndex = (int)m_fPlayTime - 1;
                    }
                }
                return frameIndex;
            }
            return 0;
        }
        //-----------------------------------------------------------------------------
        public void SetSkinFrameData(SkinFrameData frames, ESkinType type)
        {
            if(m_FrameData !=null && m_FrameData != frames)
            {
                if (m_FrameData != null) m_FrameData.release();
                m_FrameData = null;
            }
            m_SkinType = type;
            m_FrameData = frames;
            if (frames != null) frames.grab();
            UpdateSkinFrameData();
        }
        //-----------------------------------------------------------------------------
        public void UpdateSkinFrameData()
        {
            if (m_FrameData == null) return;
            if (m_SkinType == ESkinType.GpuArray)
            {
                m_Blockproperties = new MaterialPropertyBlock();
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;

                if (m_GpuSkinMatrix == null) m_GpuSkinMatrix = new List<Vector4>(5);
                m_GpuSkinMatrix.Clear();
                Vector2[] skins = m_FrameData.getSkins();
                if (skins != null)
                {
                    for (int i = 0; i < skins.Length; ++i)
                    {
                        m_GpuSkinMatrix.Add(skins[i]);
                    }
                }
                for (int i = m_GpuSkinMatrix.Count; i < 5; ++i)
                    m_GpuSkinMatrix.Add(Vector4.zero);


#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    //! comon uniform pro
                    Material material = m_FrameData.getShareMat();
                    if (gpuTexture) material.SetTexture(CGpuSharePropertyID.shaderPorpID_MainTex, gpuTexture);
                    material.SetVectorArray(CGpuSharePropertyID.shaderPropID_GPUSkinning_SKINS, m_GpuSkinMatrix);
                    material.SetTexture(CGpuSharePropertyID.shaderPropID_GPUSkinning_TextureMatrix, boneFrameDatas.getAniTex());
                    material.SetVector(CGpuSharePropertyID.shaderPropID_GPUSkinning_TextureSize_NumPixelsPerFrame,
                    new Vector4(boneFrameDatas.boneAnimations._textureWidth, boneFrameDatas.boneAnimations._textureHeight, boneFrameDatas.boneAnimations._skeletonLen * 3, 0f));
                }
                else
                {
                    //! comon uniform pro
                    Material material = m_FrameData.getShareMat();
                    if (gpuTexture) material.SetTexture("_DiffuseTex", gpuTexture);
                    material.SetVectorArray("_SKIN_UVS", m_GpuSkinMatrix);
                    material.SetTexture("_GPUSkinning_TextureMatrix", boneFrameDatas.getAniTex());
                    material.SetVector("_GPUSkinning_TextureSize_NumPixelsPerFrame",
                    new Vector4(boneFrameDatas.boneAnimations._textureWidth, boneFrameDatas.boneAnimations._textureHeight, boneFrameDatas.boneAnimations._skeletonLen * 3, 0f));
                }
#else
                 Material material = m_FrameData.getShareMat();
                if (gpuTexture) material.SetTexture(CGpuSharePropertyID.shaderPorpID_MainTex, gpuTexture);
                material.SetVectorArray(CGpuSharePropertyID.shaderPropID_GPUSkinning_SKINS, m_GpuSkinMatrix);
                material.SetTexture(CGpuSharePropertyID.shaderPropID_GPUSkinning_TextureMatrix, boneFrameDatas.getAniTex());
                material.SetVector(CGpuSharePropertyID.shaderPropID_GPUSkinning_TextureSize_NumPixelsPerFrame,
                new Vector4(boneFrameDatas.boneAnimations._textureWidth, boneFrameDatas.boneAnimations._textureHeight, boneFrameDatas.boneAnimations._skeletonLen * 3, 0f));
#endif
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (cpuTexture)
                    {
                        CGpuSharePropertyID.materialBlock.SetTexture(CGpuSharePropertyID.shaderPorpID_MainTex, cpuTexture);
                    }
                }
                else
                {
                    if (cpuTexture)
                        m_FrameData.getShareMat().SetTexture(CGpuSharePropertyID.shaderPorpID_MainTex, cpuTexture);
                    else
                        cpuTexture = m_FrameData.getShareMat().GetTexture(CGpuSharePropertyID.shaderPorpID_MainTex);
                }
#else
                     if (cpuTexture)
                        {
                            CGpuSharePropertyID.materialBlock.SetTexture(CGpuSharePropertyID.shaderPorpID_MainTex, cpuTexture);
                        }
#endif
            }
        }
        //-----------------------------------------------------------------------------
        public bool GetSlot(string name, out Vector3 vPos, out Quaternion vRot, out Vector3 vScale)
        {
            vPos = Vector3.zero;
            vRot = Quaternion.identity;
            vScale = Vector3.one;
            if (m_FrameData == null) return false;
            SkeletonSlot[] slots = m_FrameData.getSlots();
            for (int i = 0; i < slots.Length; ++i)
            {
                if (slots[i]._name.CompareTo(name) == 0)
                {
                    slots[i].GetFrameTRS(m_nCurIndex, IsLooping(), GetTime(), GetDuration(), GetFrameRate(), out vPos, out vRot, out vScale);
                    return true;
                }
            }
            return false;
        }
        //-----------------------------------------------------------------------------
        public int GetAnimsCnt()
        {
            if (m_FrameData == null) return 0;
            return m_FrameData.getAnimCnt();
        }
        //-----------------------------------------------------------------------------
        public ISkinAnimData GetAnimData(int index)
        {
            if (m_FrameData == null) return null;
            int arryIndex = index - 1;
            if (arryIndex < 0) return null;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (arryIndex < 0 && arryIndex >= boneFrameDatas.boneAnimations.animations.Length)
                    return null;
                return boneFrameDatas.boneAnimations.animations[arryIndex];
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                if (arryIndex < 0 && arryIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return null;
                return boneFrameDatas.animations.animMeshs[arryIndex];
            }
            return null;
        }
        //-----------------------------------------------------------------------------
        public int GetActionIndex(EActionStateType eType, uint tag = 0)
        {
            if (m_FrameData == null) return -1;
            if (m_SkinType == ESkinType.GpuArray)
            {
                uint actionTag = (uint)((int)eType << 16 | (int)tag);
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                for (int i = 0; i < boneFrameDatas.boneAnimations.animations.Length; ++i)
                {
                    if (boneFrameDatas.boneAnimations.animations[i]._actionTag == actionTag)
                    {
                        return i;
                    }
                }
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                uint actionTag = (uint)((int)eType << 16 | tag);

                for (int i = 0; i < boneFrameDatas.animations.animMeshs.Length; ++i)
                {
                    if (boneFrameDatas.animations.animMeshs[i].actionTag == actionTag)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        //-----------------------------------------------------------------------------
        public int GetActionIndex(string actionName)
        {
            if (m_FrameData == null || string.IsNullOrEmpty(actionName)) return -1;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                for (int i = 0; i < boneFrameDatas.boneAnimations.animations.Length; ++i)
                {
                    if (boneFrameDatas.boneAnimations.animations[i]._animName.CompareTo(actionName) ==0)
                    {
                        return i;
                    }
                }
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                for (int i = 0; i < boneFrameDatas.animations.animMeshs.Length; ++i)
                {
                    if (boneFrameDatas.animations.animMeshs[i]._name.CompareTo(actionName)==0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        //-----------------------------------------------------------------------------
        public void SetIdleType(EActionStateType eType, uint tag = 0)
        {
            m_nDefaultIndex = GetActionIndex(eType, tag);
        }
        //-----------------------------------------------------------------------------
        public bool PlayByIndex(int index)
        {
            if (m_FrameData == null) return false;
            int arryIndex = index - 1;
            if (arryIndex < 0) return false;
            if (m_nCurIndex == arryIndex) return true;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (arryIndex < 0 && arryIndex >= boneFrameDatas.boneAnimations.animations.Length)
                    return false;
                m_nCurIndex = arryIndex;
                return true;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                if (arryIndex < 0 && arryIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return false;
                m_nCurIndex = arryIndex;

                return true;
            }
            return false;
        }
        //-----------------------------------------------------------------------------
        public bool Play(EActionStateType eType, uint tag, bool bForce = false)
        {
            if (m_FrameData == null) return false;
            if (m_SkinType == ESkinType.GpuArray)
            {
                uint actionTag = (uint)((int)eType << 16 | (int)tag);
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                for (int i = 0; i < boneFrameDatas.boneAnimations.animations.Length; ++i)
                {
                    if (boneFrameDatas.boneAnimations.animations[i]._actionTag == actionTag)
                    {
                        if(m_nCurIndex != i)
                            m_fPlayTime = 0f;
                        m_nCurIndex = i;
                        return true;
                    }
                }
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                uint actionTag = (uint)((int)eType << 16 | tag);

                for (int i = 0; i < boneFrameDatas.animations.animMeshs.Length; ++i)
                {
                    if (boneFrameDatas.animations.animMeshs[i].actionTag == actionTag)
                    {
                        if (m_nCurIndex != i)
                            m_fPlayTime = 0f;
                        m_nCurIndex = i;
                        return true;
                    }
                }
            }

            return false;
        }
        //-----------------------------------------------------------------------------
        public bool Play(string name, bool bForce = false)
        {
            if (m_FrameData == null) return false;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.boneAnimations.animations.Length &&
                    boneFrameDatas.boneAnimations.animations[m_nCurIndex]._animName.CompareTo(name) == 0)
                    return true;

                for (int i = 0; i < boneFrameDatas.boneAnimations.animations.Length; ++i)
                {
                    if (boneFrameDatas.boneAnimations.animations[i]._animName.CompareTo(name) == 0)
                    {
                        m_fPlayTime = 0f;
                        m_nCurIndex = i;
                        return true;
                    }
                }
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.animations.animMeshs.Length &&
                    boneFrameDatas.animations.animMeshs[m_nCurIndex]._name.CompareTo(name) == 0)
                    return true;

                for (int i = 0; i < boneFrameDatas.animations.animMeshs.Length; ++i)
                {
                    if (boneFrameDatas.animations.animMeshs[i]._name.CompareTo(name) == 0)
                    {
                        m_fPlayTime = 0f;
                        m_nCurIndex = i;
                        return true;
                    }
                }
            }

            return false;
        }
        //-----------------------------------------------------------------------------
        public void Stop(EActionStateType eType, uint tag = 0)
        {
            int index = GetActionIndex(eType, tag);
            if(index == m_nCurIndex)
            {
                m_nCurIndex = m_nDefaultIndex;
                m_fPlayTime = 0.0f;
            }
        }
        //-----------------------------------------------------------------------------
        public void Stop(string actionName)
        {
            int index = GetActionIndex(actionName);
            if (index == m_nCurIndex)
            {
                m_nCurIndex = m_nDefaultIndex;
                m_fPlayTime = 0.0f;
            }
        }
        //-----------------------------------------------------------------------------
        public uint GetCurPlayActionTag()
        {
            if (m_nCurIndex < 0 || m_FrameData == null)
                return 0xffffffff;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.boneAnimations.animations.Length)
                    return boneFrameDatas.boneAnimations.animations[m_nCurIndex]._actionTag;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.animations.animMeshs.Length)
                    return boneFrameDatas.animations.animMeshs[m_nCurIndex].actionTag;
            }
            return 0xffffffff;
        }
        //-----------------------------------------------------------------------------
        public string GetCurPlayName()
        {
            if (m_nCurIndex < 0 || m_FrameData== null)
                return null;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.boneAnimations.animations.Length)
                    return boneFrameDatas.boneAnimations.animations[m_nCurIndex]._animName;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.animations.animMeshs.Length)
                    return boneFrameDatas.animations.animMeshs[m_nCurIndex]._name;
            }
            return null;
        }
        //-----------------------------------------------------------------------------
        public int NameToIndex(string name)
        {
            if (m_FrameData == null) return -1;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.boneAnimations.animations.Length &&
                    boneFrameDatas.boneAnimations.animations[m_nCurIndex]._animName.CompareTo(name) == 0)
                    return m_nCurIndex+1;

                for (int i = 0; i < boneFrameDatas.boneAnimations.animations.Length; ++i)
                {
                    if (boneFrameDatas.boneAnimations.animations[i]._animName.CompareTo(name) == 0)
                    {
                        return i+1;
                    }
                }
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= 0 && m_nCurIndex < boneFrameDatas.animations.animMeshs.Length &&
                    boneFrameDatas.animations.animMeshs[m_nCurIndex]._name.CompareTo(name) == 0)
                    return m_nCurIndex+1;

                for (int i = 0; i < boneFrameDatas.animations.animMeshs.Length; ++i)
                {
                    if (boneFrameDatas.animations.animMeshs[i]._name.CompareTo(name) == 0)
                    {
                        return i+1;
                    }
                }
            }

            return 0;
        }
        //-----------------------------------------------------------------------------
        public bool HasActionAnim(string name)
        {
            return NameToIndex(name) > 0;
        }
        //-----------------------------------------------------------------------------
        public bool IsEndAction()
        {
            if (m_FrameData == null) return false;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return false;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

                if (m_fPlayTime >= boneAnimation._length) return true;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return false;

                int frameIndex = (int)m_fPlayTime;
                if (frameIndex >= boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount)
                    return true;
            }
            return false;
        }
        //-----------------------------------------------------------------------------
        public float GetNormalizeTime()
        {
            if (m_FrameData == null) return 0f;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return 0f;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

                return m_fPlayTime / boneAnimation._length;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;

                if (m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return 0f;
                return m_fPlayTime / boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount;
            }
            return 0f;
        }
        //-----------------------------------------------------------------------------
        public float GetTime()
        {
            return m_fPlayTime;
        }
        //-----------------------------------------------------------------------------
        public float GetDuration()
        {
            if (m_FrameData == null) return 0f;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return 0f;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];
                return boneAnimation._length;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                if (m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return 0f;
                var frame = boneFrameDatas.animations.animMeshs[m_nCurIndex];
                return (float)frame._keyFrameCount/ 30.0f;
            }
            return 0f;
        }
        //-----------------------------------------------------------------------------
        public int GetFrameRate()
        {
            if (m_FrameData == null) return 30;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return 30;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];
                return boneAnimation._fps;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                return 30;
            }
            return 30;
        }
        //-----------------------------------------------------------------------------
        public bool IsLooping()
        {
            if (m_FrameData == null) return false;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (m_nCurIndex<0 || m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return false;
                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];
                return boneAnimation._loop;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                if (m_nCurIndex<0 || m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return false;
                return boneFrameDatas.animations.animMeshs[m_nCurIndex]._loop;
            }
            return false;
        }
        //-----------------------------------------------------------------------------
        public void ForceUpdate(float fFrameTime)
        {
            InnerUpdate(fFrameTime);
        }
        //-----------------------------------------------------------------------------
        public void FillInstanceParams(Vector4[] vParams, int index)
        {
            vParams[index] = Vector4.zero;
            if (m_FrameData == null || m_SkinType != ESkinType.GpuArray)
                return;

            SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
            if (boneFrameDatas.boneAnimations == null) return;
            if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return;

            SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

            int frameIndex = 0;
            if (boneAnimation._loop)
                frameIndex = (int)(m_fPlayTime * boneAnimation._fps) % (int)(boneAnimation._length * boneAnimation._fps);
            else if (m_fPlayTime >= boneAnimation._length)
                frameIndex = (int)(boneAnimation._length * boneAnimation._fps);
            else
                frameIndex = (int)(m_fPlayTime * boneAnimation._fps);
            vParams[index] = new Vector4(frameIndex, boneAnimation._pixelSegmentation, m_nSkinIndex);
        }
        //-----------------------------------------------------------------------------
        protected void InnerUpdate(float fFrameTime)
        {
            if (m_FrameData == null) return;
            if (m_SkinType == ESkinType.GpuArray)
            {
                SkinFrameBoneData boneFrameDatas = m_FrameData as SkinFrameBoneData;
                if (boneFrameDatas.boneAnimations == null) return;
                if (m_nCurIndex >= boneFrameDatas.boneAnimations.animations.Length) return;

                SkeletonBoneAnimation boneAnimation = boneFrameDatas.boneAnimations.animations[m_nCurIndex];

                int frameIndex = 0;
                if (boneAnimation._loop)
                    frameIndex = (int)(m_fPlayTime * boneAnimation._fps) % (int)(boneAnimation._length * boneAnimation._fps);
                else if (m_fPlayTime >= boneAnimation._length)
                {
                    frameIndex = (int)(boneAnimation._length * boneAnimation._fps);
                    if(m_nDefaultIndex != m_nCurIndex && m_nDefaultIndex>=0)
                    {
                        m_nCurIndex = m_nDefaultIndex;
                        m_fPlayTime = 0.0f;
                    }
                }
                else
                    frameIndex = (int)(m_fPlayTime * boneAnimation._fps);
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    //! instance pro
                    m_Blockproperties.SetVector(CGpuSharePropertyID.shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation, new Vector4(frameIndex, boneAnimation._pixelSegmentation, m_nSkinIndex));
                }
                else
                {
                    //! instance pro
                    m_Blockproperties.SetVector(CGpuSharePropertyID.shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation, new Vector4(frameIndex, boneAnimation._pixelSegmentation, m_nSkinIndex));

                }
#else
                //! instance pro
                m_blockproperties.SetVector(CGpuSharePropertyID.shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation, new Vector4(frameIndex, boneAnimation._pixelSegmentation, m_nSkinIndex));

#endif
                m_fPlayTime += fFrameTime * m_fSpeed * boneAnimation._speed;
            }
            else if (m_SkinType == ESkinType.CpuData)
            {
                SkinFrameMeshData boneFrameDatas = m_FrameData as SkinFrameMeshData;
                boneFrameDatas.update();
                if (boneFrameDatas.animations == null || m_nCurIndex >= boneFrameDatas.animations.animMeshs.Length)
                    return;

                int frameIndex = (int)m_fPlayTime;
                if (frameIndex >= boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount)
                {
                    if (boneFrameDatas.animations.animMeshs[m_nCurIndex]._loop)
                    {
                        m_fPlayTime = 0f;
                        frameIndex = 0;
                    }
                    else
                    {
                        m_fPlayTime = boneFrameDatas.animations.animMeshs[m_nCurIndex]._keyFrameCount;
                        frameIndex = (int)m_fPlayTime - 1;
                        if (m_nDefaultIndex != m_nCurIndex && m_nDefaultIndex >= 0)
                        {
                            m_nCurIndex = m_nDefaultIndex;
                            m_fPlayTime = 0.0f;
                        }
                    }
                }
                AnimFrameData frames = boneFrameDatas.animations.animMeshs[m_nCurIndex];
                //! ÏÈ·ÏÆú
                //  var sharedMesh = frames._frames[frameIndex].getMesh(boneFrameDatas.animations._trianges, boneFrameDatas.animations.getUV(m_nSkinIndex), m_nSkinIndex, boneFrameDatas.animations.getSkinCnt());

                m_fPlayTime += fFrameTime * m_fSpeed * 30 * frames._speed;
            }
        }
    }
}
#endif