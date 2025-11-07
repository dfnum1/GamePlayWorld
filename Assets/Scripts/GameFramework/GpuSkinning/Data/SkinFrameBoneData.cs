
#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    internal class SkinFrameBoneData : SkinFrameData
    {
        public ASkeletonGpuData boneAnimations;
        public Texture2D m_pTex;

        public override void clear()
        {
            base.clear();
#if UNITY_EDITOR
            if (m_pTex)
            {
                if(Application.isPlaying) UnityEngine.Object.Destroy(m_pTex);
                else UnityEngine.Object.DestroyImmediate(m_pTex);
            }
#else
            if(m_pTex) UnityEngine.Object.Destroy(m_pTex);
#endif
            m_pTex = null;
            boneAnimations = null;
        }
        public override Vector2[] getSkins() { return boneAnimations._offsets; }

        public override SkeletonSlot[] getSlots()
        {
            return boneAnimations.slots;
        }
        public override void setSlots(SkeletonSlot[] slots)
        {
            boneAnimations.slots = slots;
        }

        public override Mesh getShareMesh()
        {
            if (!m_shareMesh)
                m_shareMesh = boneAnimations.shareMeshData.getShareMesh();
            return m_shareMesh;
        }

        public override Material getShareMat()
        {
            if (!m_shareMat)
            {
                m_shareMat = new Material(Shader.Find("Hidden/HL/Role/GPUSkinningInstance"));
                m_shareMat.enableInstancing = true;
                m_shareMat.EnableKeyword("INSTANCING_ON");
            }
            return m_shareMat;
        }

        public override int getAnimCnt()
        {
            return boneAnimations.animations.Length;
        }

        public override string getAnimState(int index)
        {
            return boneAnimations.animations[index]._animName;
        }

        public Texture2D getAniTex()
        {
            if (!m_pTex)
            {
                m_pTex = new Texture2D(boneAnimations._textureWidth, boneAnimations._textureHeight, TextureFormat.RGBAHalf, false, true);
                m_pTex.name = "GPUSkinningTextureMatrix";
                m_pTex.filterMode = FilterMode.Point;
                m_pTex.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                m_pTex.LoadRawTextureData(boneAnimations._texDatas);
                m_pTex.Apply(false, true);
            }

            return m_pTex;
        }
#if UNITY_EDITOR
        public override void setShareMesh(Mesh mesh)
        {
            m_shareMesh = mesh;
            boneAnimations.shareMeshData.setShareMesh(mesh);
        }
        public void setAniTex(Texture2D tex)
        {
            m_pTex = tex;
            boneAnimations.setTex(tex);
        }

        public SkeletonBone GetBoneByTransform(Transform transform)
        {
            SkeletonBone[] bones = boneAnimations.skeleton;
            int numBones = bones.Length;
            for (int i = 0; i < numBones; ++i)
            {
                if (bones[i]._transform == transform)
                {
                    return bones[i];
                }
            }
            return null;
        }

        public int GetBoneIndex(SkeletonBone bone)
        {
            return System.Array.IndexOf(boneAnimations.skeleton, bone);
        }
#endif
    }

}
#endif