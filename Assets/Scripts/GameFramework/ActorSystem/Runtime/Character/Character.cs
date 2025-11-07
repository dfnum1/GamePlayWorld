/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	Character
作    者:	HappLI
描    述:	
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
#endif
using System.Collections.Generic;
using UnityEngine;
using Framework.Plugin.AT;
using UnityEngine.Pool;

namespace Framework.Core
{
    //--------------------------------------------------------
    class Avatar : IUserData
    {
        public string avatarFile = null;
        public bool IsDestroyed = false;
        public Asset pAsset;
        public AvatarData avatarData;

        public Avatar()
        {
            pAsset = null;
            avatarData = null;
            IsDestroyed = false;
        }

        public void Destroy()
        {
            if (IsDestroyed)
                return;
            IsDestroyed = true;
            avatarFile = null;
            if (pAsset != null) pAsset.Release();
            pAsset = null;
            avatarData = null;
        }
    }
    //--------------------------------------------------------
    public enum EControllerDirFlag
    {
        [ATField("向前")]Forward = 1 << 0,
        [ATField("向后")] Back = 1 << 1,
        [ATField("向左")] Left = 1 << 2,
        [ATField("向右")] Right = 1 << 3,
    };
    //--------------------------------------------------------
    [ATExportMono("World/Character", EGlobalType.None, true, "", marcoDefine = "USE_ACTORSYSTEM")]
    public class Character : Actor
    {
#if USE_ACTORSYSTEM
        int m_nControllerDir = 0;
        bool m_bLastControllPressing = false;
        bool m_bControllPressing = false;
        FVector3 m_ControllerDirection = FVector3.forward;
        FVector3 m_PressDirection = FVector3.zero;

        private Dictionary<byte, Avatar> m_vAvatars = null;
        private bool m_bDirtyAvatar = false;
        //--------------------------------------------------------
        public Character(AFramework pGame) : base(pGame)
        {
            Reset();
        }
        //--------------------------------------------------------
        protected override void Reset()
        {
            base.Reset();
            m_bControllPressing = false;
            m_bLastControllPressing = false;
            m_nControllerDir = 0;
            m_ControllerDirection = FVector3.forward;
            m_PressDirection = FVector3.zero;
        }
        //--------------------------------------------------------
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearAvatars();
        }
        //--------------------------------------------------------
        void ClearAvatars()
        {
            if (m_vAvatars != null)
            {
                foreach (var avatar in m_vAvatars)
                {
                    avatar.Value.Destroy();
                }
                m_vAvatars.Clear();
            }
            m_bDirtyAvatar = false;
        }
        //--------------------------------------------------------
        public void AddAvatar(byte type, string avatarFile)
        {
            if (m_vAvatars == null) m_vAvatars = new Dictionary<byte, Avatar>();
            if (!m_vAvatars.TryGetValue(type, out var avatar))
            {
                avatar = new Avatar();
                m_vAvatars[type] = avatar;
            }
            avatar.IsDestroyed = false;
            avatar.avatarFile = avatarFile;
            var assetOp = GetGameModule().FileSystem.ReadFile(avatarFile);
            assetOp.OnCallback += OnLoadAvatar;
            assetOp.userData1 = avatar;
        }
        //--------------------------------------------------------
        public void RemoveAvatar(byte type)
        {
            if (m_vAvatars != null && m_vAvatars.TryGetValue(type, out var avatar))
            {
                if (avatar.pAsset != null)
                {
                    avatar.Destroy();
                    m_bDirtyAvatar = true;
                }
            }
        }
        //--------------------------------------------------------
        void OnLoadAvatar(AssetOperiaon assetOp)
        {
            if (assetOp.pAsset == null)
                return;
            Avatar avatar = assetOp.userData1 as Avatar;
            assetOp.pAsset.Grab();
            if (avatar.IsDestroyed)
            {
                assetOp.Release();
                return;
            }
            if (assetOp.pAsset.Path != avatar.avatarFile)
            {
                assetOp.Release();
                return;
            }
            avatar.pAsset = assetOp.pAsset;
            avatar.avatarData = assetOp.pAsset.GetOrigin<AvatarData>();
            m_bDirtyAvatar = true;
        }
        //--------------------------------------------------------
        [ATMethod]
        public void EnableController(EControllerDirFlag flag, bool bEnable)
        {
            if (bEnable) m_nControllerDir |= (int)flag;
            else m_nControllerDir = (m_nControllerDir | (int)flag) - (int)flag;

            if (m_nControllerDir == 0)
            {
                m_bControllPressing = false;
                SetSpeedXZ(0.0f, 0.0f);
            }
            else
            {
                m_bControllPressing = true;
                m_PressDirection.y = 0;
                m_PressDirection.z = (((m_nControllerDir & (int)EControllerDirFlag.Forward) != 0) ? 1f : (((m_nControllerDir & (int)EControllerDirFlag.Back) != 0) ? -1f : 0f));
                m_PressDirection.x = (((m_nControllerDir & (int)EControllerDirFlag.Left) != 0) ? -1f : (((m_nControllerDir & (int)EControllerDirFlag.Right) != 0) ? 1f : 0f));
                m_ControllerDirection = m_PressDirection;
                m_ControllerDirection.Normalize();
            }
        }
        //--------------------------------------------------------
        protected override void InnerUpdate(ExternEngine.FFloat fFrame)
        {
            base.InnerUpdate(fFrame);
            if (m_bControllPressing)
            {
                SetSpeedXZ(m_PressDirection);
                SetDirection(m_ControllerDirection);
            }
            if (m_bLastControllPressing != m_bControllPressing)
            {
                if (m_bControllPressing)
                    StartActionState(EActionStateType.Run);
                else
                    StopActionState(EActionStateType.Run);
                m_bLastControllPressing = m_bControllPressing;
            }

            if (m_bDirtyAvatar)
            {
                if (IsLoadComplete())
                {
                    m_bDirtyAvatar = false;
                    RefreshAvatars();
                }
            }
        }
        //--------------------------------------------------------
        void RefreshAvatars()
        {
            CharacterComponent characterComp = this.GetObjectAble() as CharacterComponent;
            if (characterComp == null) return;

            // 1. 收集骨骼
            Dictionary<string, int> vBones = new Dictionary<string, int>();
            Transform roots = this.GetObjectAble().gameObject.transform.Find("root");
            Transform[] boneArray = characterComp.skeletons;
            for (int i = 0; i < boneArray.Length; ++i)
            {
                vBones[boneArray[i].name] = i;
            }

            // 2. 收集所有贴图
            var baseMaps = ListPool<Texture2D>.Get();
            var metallicMaps = ListPool<Texture2D>.Get();
            var occlusionMaps = ListPool<Texture2D>.Get();
            var avatarMaterials = ListPool<Material>.Get();
            var avatarMeshes = ListPool<Mesh>.Get();
            List<int> baseMapIndices = ListPool<int>.Get();
            foreach (var kv in m_vAvatars)
            {
                var avatar = kv.Value;
                if (avatar?.avatarData?.partMaterial == null || avatar.avatarData.partMesh == null) continue;
                var mat = avatar.avatarData.partMaterial;
                var texture = mat.GetTexture("_BaseMap") as Texture2D;
                int baseMapIdx = baseMaps.IndexOf(texture);
                if (baseMapIdx < 0)
                {
                    baseMaps.Add(texture);
                    baseMapIdx = baseMaps.Count - 1;
                }
                baseMapIndices.Add(baseMapIdx);

                texture = mat.GetTexture("_MetallicGlossMap") as Texture2D;
                if (!metallicMaps.Contains(texture)) metallicMaps.Add(texture);

                texture = mat.GetTexture("_OcclusionMap") as Texture2D;
                if (!occlusionMaps.Contains(texture)) occlusionMaps.Add(texture);



                avatarMaterials.Add(mat);
                avatarMeshes.Add(avatar.avatarData.partMesh);
            }

            bool sameBase = baseMaps.Count <= 1;
            bool sameMetallic = metallicMaps.Count <= 1;
            bool sameOcclusion = occlusionMaps.Count <= 1;

            // 4. pack 或直接使用
            Rect[] baseRects = null, metallicRects = null, occlusionRects = null;
            Texture2D packedBase = null, packedMetallic = null, packedOcclusion = null;

            if (!sameBase && baseMaps.Count > 0)
            {
                packedBase = new Texture2D(1024, 1024);
                baseRects = packedBase.PackTextures(baseMaps.ToArray(), 1, 1024);
            }
            else packedBase = baseMaps.Count > 0 ? baseMaps[0] : null;
            if (!sameMetallic && metallicMaps.Count > 0)
            {
                packedMetallic = new Texture2D(1024, 1024);
                metallicRects = packedMetallic.PackTextures(metallicMaps.ToArray(), 1, 1024);
            }
            else packedMetallic = metallicMaps.Count > 0 ? metallicMaps[0] : null;
            if (!sameOcclusion && occlusionMaps.Count > 0)
            {
                packedOcclusion = new Texture2D(1024, 1024);
                occlusionRects = packedOcclusion.PackTextures(occlusionMaps.ToArray(), 1, 1024);
            }
            else packedOcclusion = occlusionMaps.Count > 0 ? occlusionMaps[0] : null;

            // 5. 合并Mesh数据
            var vertices = ListPool<Vector3>.Get();
            var normals = ListPool<Vector3>.Get();
            var tangents = ListPool<Vector4>.Get();
            var uvs = ListPool<Vector2>.Get();
            var boneWeights = ListPool<BoneWeight>.Get();
            var allTriangles = ListPool<int>.Get();

            int vertexOffset = 0;
            int avatarIndex = 0;

            foreach (var kv in m_vAvatars)
            {
                var avatar = kv.Value;
                if (avatar == null || avatar.avatarData == null || avatar.avatarData.partMesh == null)
                    continue;

                Mesh mesh = avatar.avatarData.partMesh;
                Vector2[] meshUVs = mesh.uv;

                // 只对 pack 过的贴图做 UV 偏移
                if (!sameBase && baseRects != null)
                {
                    int baseMapIdx = baseMapIndices[avatarIndex];
                    Rect uvRect = baseRects[baseMapIdx];
                    Vector2[] newUVs = new Vector2[meshUVs.Length];
                    for (int i = 0; i < meshUVs.Length; i++)
                    {
                        newUVs[i] = new Vector2(
                            uvRect.x + meshUVs[i].x * uvRect.width,
                            uvRect.y + meshUVs[i].y * uvRect.height
                        );
                    }
                    meshUVs = newUVs;
                }
                // metallic/occlusion同理，如需支持多UV通道可扩展

                vertices.AddRange(mesh.vertices);
                normals.AddRange(mesh.normals);
                tangents.AddRange(mesh.tangents);
                uvs.AddRange(meshUVs);

                // 合并所有subMesh的三角面到一个数组
                for (int s = 0; s < mesh.subMeshCount; s++)
                {
                    var tris = mesh.GetTriangles(s);
                    for (int i = 0; i < tris.Length; i++)
                        tris[i] += vertexOffset;
                    allTriangles.AddRange(tris);
                }

                // 1. 建立bindpose索引映射
                int[] remap = new int[mesh.bindposes.Length];
                for (int i = 0; i < avatar.avatarData.bones.Length; i++)
                {
                    remap[i] = -1;
                    if (vBones.TryGetValue(avatar.avatarData.bones[i], out var boneIndex))
                    {
                        remap[i] = boneIndex;
                    }
                    if (remap[i] == -1)
                        Debug.LogWarning($"未找到bindpose映射: mesh[{i}]");
                }
                // 2. 重映射boneWeight
                foreach (var bw in mesh.boneWeights)
                {
                    BoneWeight newBW = bw;
                    newBW.boneIndex0 = remap[bw.boneIndex0];
                    newBW.boneIndex1 = remap[bw.boneIndex1];
                    newBW.boneIndex2 = remap[bw.boneIndex2];
                    newBW.boneIndex3 = remap[bw.boneIndex3];
                    boneWeights.Add(newBW);
                }

                vertexOffset += mesh.vertexCount;
                avatarIndex++;
            }

            // 6. 构建新Mesh
            Mesh combinedMesh = new Mesh();
            combinedMesh.name = "CombinedAvatarMesh";
            combinedMesh.SetVertices(vertices);
            combinedMesh.SetNormals(normals);
            combinedMesh.SetTangents(tangents);
            combinedMesh.SetUVs(0, uvs);
            combinedMesh.boneWeights = boneWeights.ToArray();
            combinedMesh.bindposes = characterComp.bindposes;

            // 只设置一个subMesh
            combinedMesh.subMeshCount = 1;
            combinedMesh.SetTriangles(allTriangles, 0);

            // 7. 合并材质并设置贴图
            var combinedMaterial = new Material(avatarMaterials.Count > 0 ? avatarMaterials[0].shader : Shader.Find("Standard"));
            if (!sameBase && packedBase != null)
                combinedMaterial.SetTexture("_BaseMap", packedBase);
            else if (baseMaps.Count > 0)
                combinedMaterial.SetTexture("_BaseMap", baseMaps[0]);
            if (!sameMetallic && packedMetallic != null)
                combinedMaterial.SetTexture("_MetallicGlossMap", packedMetallic);
            else if (metallicMaps.Count > 0)
                combinedMaterial.SetTexture("_MetallicGlossMap", metallicMaps[0]);
            if (!sameOcclusion && packedOcclusion != null)
                combinedMaterial.SetTexture("_OcclusionMap", packedOcclusion);
            else if (occlusionMaps.Count > 0)
                combinedMaterial.SetTexture("_OcclusionMap", occlusionMaps[0]);

            // 8. 应用到SkinnedMeshRenderer
            SkinnedMeshRenderer renderer = this.GetComponent<SkinnedMeshRenderer>(true);
            if (renderer == null) renderer = this.AddComponent<SkinnedMeshRenderer>(true);
            renderer.sharedMesh = combinedMesh;
            renderer.bones = boneArray;
            renderer.sharedMaterial = combinedMaterial;

            ListPool<Texture2D>.Release(baseMaps);
            ListPool<Texture2D>.Release(metallicMaps);
            ListPool<Texture2D>.Release(occlusionMaps);
            ListPool<Material>.Release(avatarMaterials);
            ListPool<Mesh>.Release(avatarMeshes);
            ListPool<Vector3>.Release(vertices);
            ListPool<Vector3>.Release(normals);
            ListPool<Vector4>.Release(tangents);
            ListPool<Vector2>.Release(uvs);
            ListPool<BoneWeight>.Release(boneWeights);
            ListPool<int>.Release(allTriangles);
        }
        //--------------------------------------------------------
        private void CollectBones(Transform parent, List<Transform> bones)
        {
            if (parent == null) return;
            bones.Add(parent);
            for (int i = 0; i < parent.childCount; i++)
            {
                CollectBones(parent.GetChild(i), bones);
            }
        }
        //--------------------------------------------------------
        // 工具函数：判断两个bindpose是否近似相等
        bool ApproximatelyEqualBindpose(Matrix4x4 a, Matrix4x4 b)
        {
            for (int i = 0; i < 16; i++)
                if (Mathf.Abs(a[i] - b[i]) > 1e-5f) return false;
            return true;
        }
#else
        public Character(AFramework pGame) : base(pGame)
        {
        }
        //--------------------------------------------------------
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearAvatars();
        }
        void ClearAvatars() { }
        public void AddAvatar(byte type, string avatarFile) { }
        public void RemoveAvatar(byte type) { }
#endif
    }
}