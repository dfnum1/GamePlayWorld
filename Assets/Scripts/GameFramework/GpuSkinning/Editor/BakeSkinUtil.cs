#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.Plugin
{
    internal class BakeSkinUtil
    {
        //-----------------------------------------------------------------------------
        public static void ClearMesh(Mesh mesh)
        {
            if (mesh == null) return;
            mesh.Clear();
        }
        //-----------------------------------------------------------------------------
        public static void DestroyObject(UnityEngine.Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) GameObject.Destroy(obj);
            GameObject.DestroyImmediate(obj);
        }
        //-----------------------------------------------------------------------------
        private class BoneWeightSortData : System.IComparable<BoneWeightSortData>
        {
            public int index = 0;

            public float weight = 0;

            public int CompareTo(BoneWeightSortData b)
            {
                return weight > b.weight ? -1 : 1;
            }
        }
        //-----------------------------------------------------
        static void GetStatesRecursive(UnityEditor.Animations.AnimatorController controller, List<UnityEditor.Animations.AnimatorState> mapping, int layer)
        {
            UnityEditor.Animations.AnimatorStateMachine sm = controller.layers[layer].stateMachine;
            for (int i = 0; i < sm.states.Length; i++)
            {
                if (sm.states[i].state.motion != null) mapping.Add(sm.states[i].state);
            }
        }
        //-----------------------------------------------------------------------------
        public static SkinFrameData BakeSkin(SkinCombineData skinCombine, SkeletonSlot[] slots, ESkinType type, Texture skinTex = null, float frameRate = -1, float animSpeed = 1)
        {
            if (skinCombine == null || skinCombine.target == null || skinCombine.combineData == null) return null;
            if (skinTex == null) skinTex = skinCombine.combineData.useTexture;
            GameObject target = skinCombine.target as GameObject;
            if (target == null) return null;

            SkinFrameData skinData;
            if (type == ESkinType.GpuArray)
                skinData = new SkinFrameBoneData();
            else if (type == ESkinType.CpuData)
                skinData = new SkinFrameMeshData();
            //else if (type == ESkinType.GpuData)
            //    skinData = new SkinFrameVertexData();
            else
                return null;

            if(slots!=null)
            {
                for(int i =0; i < slots.Length; ++i)
                {
                    slots[i].slotFrames = null;
                }
            }

            Vector3 pos = target.transform.position;
            Vector3 dir = target.transform.forward;

            target.transform.position = Vector3.zero;
            target.transform.forward = Vector3.forward;

            SkinnedMeshRenderer skinrender = target.GetComponentInChildren<SkinnedMeshRenderer>();
            SkinnedMeshRenderer prefabSkin = null;

            if (type == ESkinType.CpuData)
            {
                Material mat = new Material(Shader.Find("Hidden/HL/Role/CPUSkinning"));
                if (prefabSkin)
                    mat.CopyPropertiesFromMaterial(prefabSkin.sharedMaterial);
                else
                    mat.CopyPropertiesFromMaterial(skinrender.sharedMaterial);

                skinData.setShareMat(mat);
                BakeMeshFrameData(target, skinData as SkinFrameMeshData, skinrender, skinCombine.combineData, frameRate, animSpeed);
                mat.SetTexture("_DiffuseTex", skinTex);
            }
            //else if (type == ESkinType.GpuData)
            //{
            //    skinData._shareMesh = new Mesh();
            //    skinData._shareMesh.vertices = skinrender.sharedMesh.vertices;
            //    skinData._shareMesh.uv = skinrender.sharedMesh.uv;
            //    skinData._shareMesh.triangles = skinrender.sharedMesh.triangles;

            //    skinData._shareMat = new Material(Shader.Find("SD/GPUSkinningData"));
            //    skinData._shareMat.CopyPropertiesFromMaterial(skinrender.sharedMaterial);

            //    BakeVertexFrameData(target, skinData as SkinFrameVertexData, skinrender);
            //}
            else
            {
                Material shareMat = new Material(Shader.Find("Hidden/HL/Role/GPUSkinningInstance"));
                //    shareMat.EnableKeyword("USE_MATRICES");
                shareMat.enableInstancing = true;
                if (prefabSkin)
                    shareMat.CopyPropertiesFromMaterial(prefabSkin.sharedMaterial);
                else
                    shareMat.CopyPropertiesFromMaterial(skinrender.sharedMaterial);
                if (skinCombine.materials != null && skinCombine.materials.Count > 0 && skinCombine.materials[0])
                {
                    shareMat.CopyPropertiesFromMaterial(skinCombine.materials[0]);
                }
                shareMat.name = skinCombine.combineData.assetName;
                string materalPath = skinCombine.combineData.bakerRootPath + "gpu_" + shareMat.name + ".mat";

                skinData.setShareMat(shareMat);
                BakeBoneFrameData(target,slots, skinData as SkinFrameBoneData, skinrender, skinCombine.combineData, frameRate);
                if (skinTex != null)
                {
                    shareMat.SetTexture("_DiffuseTex", skinTex);
                }
                AssetDatabase.CreateAsset(shareMat, materalPath);
                skinCombine.combineData.useMaterial = AssetDatabase.LoadAssetAtPath<Material>(materalPath);
            }

            target.transform.position = pos;
            target.transform.forward = dir;

            return skinData;
        }

        //-----------------------------------------------------------------------------
        Matrix4x4[] CalculateSkinMatrix(Transform[] bonePose,
            Matrix4x4[] bindPose,
            Matrix4x4 rootMatrix1stFrame,
            bool haveRootMotion = false)
        {
            if (bonePose.Length == 0)
                return null;

            Transform root = bonePose[0];
            while (root.parent != null)
            {
                root = root.parent;
            }
            Matrix4x4 rootMat = root.worldToLocalMatrix;

            Matrix4x4[] matrix = new Matrix4x4[bonePose.Length];
            for (int i = 0; i != bonePose.Length; ++i)
            {
                matrix[i] = rootMat * bonePose[i].localToWorldMatrix * bindPose[i];
            }
            return matrix;
        }
        //-----------------------------------------------------------------------------
        private static void BakeBoneFrameData(GameObject target, SkeletonSlot[] arrSlots, SkinFrameBoneData skinBoneData, SkinnedMeshRenderer skinRender, SkinMeshBakerData meshData = null, float frameRate = -1)
        {
            if (!skinRender || skinBoneData == null) return;
            skinBoneData.boneAnimations = ASkeletonGpuData.CreateInstance();

            string path = "";
            if (meshData.fbxAsset) path = AssetDatabase.GetAssetPath(meshData.fbxAsset);
            string bakePath = System.IO.Path.GetDirectoryName(path).Replace("\\", "/") + "/baker/";


            Mesh shareMesh = new Mesh();
            shareMesh.vertices = skinRender.sharedMesh.vertices;
            if (meshData != null)
            {
                shareMesh.uv = meshData.UV;
                //  shareMesh.normals = meshData.offsets;
            }
            else
                shareMesh.uv = skinRender.sharedMesh.uv;
            shareMesh.triangles = skinRender.sharedMesh.triangles;

            int numBones = skinRender.bones.Length;
            Animator animator = target.GetComponent<Animator>();
            animator.runtimeAnimatorController = meshData.controller;


            List<SkeletonBone> boneTmp = new List<SkeletonBone>();
            List<Matrix4x4> matricesTmp = new List<Matrix4x4>();
            List<string> bonesHierarchyNames = new List<string>();

            List<SkeletonBone> skeletonRet = new List<SkeletonBone>();
            CollectBones(skeletonRet, skinRender.bones, skinRender.sharedMesh.bindposes, null, skinRender.rootBone, 0);

            if (skinRender.rootBone.parent)
            {
                for (int k = 0; k < skinRender.rootBone.parent.childCount; ++k)
                {
                    Transform curBone = skinRender.rootBone.parent.transform.GetChild(k);
                    if (skinRender.rootBone == curBone ||
                        curBone.GetComponent<Renderer>() != null)
                        continue;

                    bool bHas = false;
                    for (int i = 0; i < skeletonRet.Count; ++i)
                    {
                        if (skeletonRet[i]._transform == curBone)
                        {
                            bHas = true;
                        }
                    }
                    if (bHas)
                        continue;
                    CollectBones(skeletonRet, skinRender.bones, skinRender.sharedMesh.bindposes, null, curBone, 0);
                }
            }
            for (int j = 0; j < target.transform.childCount; ++j)
            {
                Transform curBone = target.transform.GetChild(j);
                if (skinRender.rootBone == curBone ||
                    skinRender.rootBone.parent == curBone ||
                    curBone.GetComponent<Renderer>() != null)
                    continue;
                bool bHas = false;
                for (int i = 0; i < skeletonRet.Count; ++i)
                {
                    if (skeletonRet[i]._transform == curBone)
                    {
                        bHas = true;
                    }
                }
                if (bHas)
                    continue;

                CollectBones(skeletonRet, skinRender.bones, skinRender.sharedMesh.bindposes, null, curBone, 0);

            }

            SkeletonBone[] skeleton = skeletonRet.ToArray();
            skinBoneData.boneAnimations.skeleton = skeleton;
            skinBoneData.boneAnimations._skeletonLen = skeleton.Length;
            skinBoneData.boneAnimations._rootIndex = 0;
            skinBoneData.boneAnimations._offsets = meshData.offsets;

            Vector4[] uv2 = new Vector4[skinRender.sharedMesh.vertexCount];
            Vector4[] uv3 = new Vector4[skinRender.sharedMesh.vertexCount];
            for (int i = 0; i < skinRender.sharedMesh.vertexCount; ++i)
            {
                BoneWeight boneWeight = skinRender.sharedMesh.boneWeights[i];

                BoneWeightSortData[] weights = new BoneWeightSortData[4];
                weights[0] = new BoneWeightSortData() { index = boneWeight.boneIndex0, weight = boneWeight.weight0 };
                weights[1] = new BoneWeightSortData() { index = boneWeight.boneIndex1, weight = boneWeight.weight1 };
                weights[2] = new BoneWeightSortData() { index = boneWeight.boneIndex2, weight = boneWeight.weight2 };
                weights[3] = new BoneWeightSortData() { index = boneWeight.boneIndex3, weight = boneWeight.weight3 };

                System.Array.Sort(weights);

                SkeletonBone bone0 = skinBoneData.GetBoneByTransform(skinRender.bones[weights[0].index]);
                SkeletonBone bone1 = skinBoneData.GetBoneByTransform(skinRender.bones[weights[1].index]);
                SkeletonBone bone2 = skinBoneData.GetBoneByTransform(skinRender.bones[weights[2].index]);
                SkeletonBone bone3 = skinBoneData.GetBoneByTransform(skinRender.bones[weights[3].index]);

                uv2[i].x = skinBoneData.GetBoneIndex(bone0);
                uv2[i].y = weights[0].weight;
                uv2[i].z = skinBoneData.GetBoneIndex(bone1);
                uv2[i].w = weights[1].weight;

                uv3[i].x = skinBoneData.GetBoneIndex(bone2);
                uv3[i].y = weights[2].weight;

                uv3[i].z = skinBoneData.GetBoneIndex(bone3);
                uv3[i].w = weights[3].weight;
            }

            shareMesh.SetUVs(1, new List<Vector4>(uv2));
            shareMesh.SetUVs(2, new List<Vector4>(uv3));

            skinBoneData.boneAnimations.shareMeshData = new ShareMeshData();
            skinBoneData.setShareMesh(shareMesh);
            AnimationClip[] animationClips = null;
            if (animator.runtimeAnimatorController != null) animationClips = animator.runtimeAnimatorController.animationClips;
            if (meshData != null && meshData.animations != null && meshData.animations.Length > 0) animationClips = meshData.animations;

            List<AnimationClip> vClips = new List<AnimationClip>();
            for (int i = 0; i < animationClips.Length; ++i)
            {
                AnimationClip animClip = animationClips[i];
                if (vClips.Contains(animClip))
                    continue;
                vClips.Add(animClip);
            }
            animationClips = vClips.ToArray();
            skinBoneData.boneAnimations.animations = new SkeletonBoneAnimation[animationClips.Length];
            if (arrSlots != null)
            {
                for (int j = 0; j < arrSlots.Length; ++j)
                {
                    arrSlots[j].slotFrames = new SkeletonSlot.SlotFrames[skinBoneData.boneAnimations.animations.Length];
                }
            }
            Dictionary<string, SkeletonSlot.SlotFrameMatrix[]> vAnimSlotFrames = new Dictionary<string, SkeletonSlot.SlotFrameMatrix[]>();
            int index = 0;
            for (int i = 0; i < animationClips.Length; ++i)
            {
                AnimationClip animClip = animationClips[i];
                int rate = 30;
                if (frameRate > 0) rate = (int)frameRate;

                SkeletonBoneAnimation boneAnimation = new SkeletonBoneAnimation();
                boneAnimation._fps = rate;
                boneAnimation._animName = animClip.name;
                boneAnimation._length = animClip.length;
                boneAnimation._loop = animClip.isLooping;
                skinBoneData.boneAnimations.animations[index++] = boneAnimation;

                int keyFrameCount = (int)(boneAnimation._length * rate);
                vAnimSlotFrames.Clear();
                if (arrSlots != null)
                {
                    for (int k = 0; k < arrSlots.Length; ++k)
                    {
                        SkeletonSlot.SlotFrames slotAniFrames = arrSlots[k].slotFrames[i];
                        if (slotAniFrames.frames == null)
                        {
                            slotAniFrames.frames = new SkeletonSlot.SlotFrameMatrix[keyFrameCount];
                            arrSlots[k].slotFrames[i] = slotAniFrames;
                        }
                        vAnimSlotFrames[arrSlots[k]._name] = slotAniFrames.frames;
                    }
                }
                boneAnimation._frames = new SkeletonAnimationFrame[keyFrameCount];
                for (int frameIndex = 0; frameIndex < keyFrameCount; ++frameIndex)
                {
                    SkeletonAnimationFrame frame = new SkeletonAnimationFrame();
                    boneAnimation._frames[frameIndex] = frame;
                    float second = frameIndex <= 0 ? boneAnimation._length : (frameIndex * 1f / rate);
                    animClip.SampleAnimation(target, second);

                    boneTmp.Clear();
                    matricesTmp.Clear();
                    bonesHierarchyNames.Clear();

                    frame._matrices = new Matrix4x4[skeleton.Length];
                    for (int j = 0; j < skeleton.Length; ++j)
                    {
                        Transform boneTransform = skeleton[j]._transform;
                        SkeletonBone currentBone = skinBoneData.GetBoneByTransform(boneTransform);
                        frame._matrices[j] = currentBone._bindpose;
                        //frame._matrices[j] = Matrix4x4.TRS(currentBone._transform.position, currentBone._transform.rotation, currentBone._transform.localScale) * currentBone._bindpose;
                        frame._matrices[j] = currentBone._transform.localToWorldMatrix * currentBone._bindpose;
                        if (vAnimSlotFrames.TryGetValue(boneTransform.name, out var slotBoneFrames))
                        {
                            SkeletonSlot.SlotFrameMatrix slotMatrix = new SkeletonSlot.SlotFrameMatrix();
                            slotMatrix.pos = BaseUtil.GetPosition(currentBone._transform.localToWorldMatrix);
                            slotMatrix.rot = currentBone._transform.localToWorldMatrix.rotation;
                            slotMatrix.scale = 1.0f;
                            slotBoneFrames[frameIndex] = slotMatrix;
                        }
                    }
                    //frame._matrices = new Matrix4x4[skeleton.Length];
                    //for (int j = 0; j < skeleton.Length; ++j)
                    //{
                    //    Transform boneTransform = skeleton[j]._transform;
                    //    SkeletonBone currentBone = skinBoneData.GetBoneByTransform(boneTransform);
                    //    frame._matrices[j] = currentBone._bindpose;
                    //    do
                    //    {
                    //        Matrix4x4 mat = Matrix4x4.TRS(currentBone._transform.localPosition, currentBone._transform.localRotation, currentBone._transform.localScale);
                    //        frame._matrices[j] = mat * frame._matrices[j];
                    //        if (currentBone._parentBoneIndex == -1)
                    //        {
                    //            break;
                    //        }
                    //        else
                    //        {
                    //            currentBone = skeleton[currentBone._parentBoneIndex];
                    //        }
                    //    }
                    //    while (true);
                    //}

                    //if (frameIndex == 0)
                    //{
                    //    skinBoneData.boneAnimations._rootMotionPosition = skeleton[skinBoneData.boneAnimations._rootIndex]._transform.localPosition;
                    //    skinBoneData.boneAnimations._rootMotionRotation = skeleton[skinBoneData.boneAnimations._rootIndex]._transform.localRotation;
                    //}
                    //else
                    //{
                    //    Vector3 newPosition = skeleton[skinBoneData.boneAnimations._rootIndex]._transform.localPosition;
                    //    Quaternion newRotation = skeleton[skinBoneData.boneAnimations._rootIndex]._transform.localRotation;
                    //    Vector3 deltaPosition = newPosition - skinBoneData.boneAnimations._rootMotionPosition;
                    //    frame._rootMotionDeltaPositionQ = Quaternion.Inverse(Quaternion.Euler(target.transform.forward.normalized)) * Quaternion.Euler(deltaPosition.normalized);
                    //    frame._rootMotionDeltaPositionL = deltaPosition.magnitude;
                    //    frame._rootMotionDeltaRotation = Quaternion.Inverse(skinBoneData.boneAnimations._rootMotionRotation) * newRotation;
                    //    skinBoneData.boneAnimations._rootMotionPosition = newPosition;
                    //    skinBoneData.boneAnimations._rootMotionRotation = newRotation;

                    //    if (frameIndex == 1)
                    //    {
                    //        boneAnimation._frames[0]._rootMotionDeltaPositionQ = boneAnimation._frames[1]._rootMotionDeltaPositionQ;
                    //        boneAnimation._frames[0]._rootMotionDeltaPositionL = boneAnimation._frames[1]._rootMotionDeltaPositionL;
                    //        boneAnimation._frames[0]._rootMotionDeltaRotation = boneAnimation._frames[1]._rootMotionDeltaRotation;
                    //    }
                    //}
                }
                animClip.SampleAnimation(target, 0);
            }

            //create texture

            int numPixels = 0;
            SkeletonBoneAnimation[] clips = skinBoneData.boneAnimations.animations;
            int numClips = clips.Length;
            for (int clipIndex = 0; clipIndex < numClips; ++clipIndex)
            {
                SkeletonBoneAnimation clip = clips[clipIndex];
                clip._pixelSegmentation = numPixels;

                SkeletonAnimationFrame[] frames = clip._frames;
                int numFrames = frames.Length;
                numPixels += skinBoneData.boneAnimations.skeleton.Length * 3 * numFrames;
            }

            int texWidth = 1;
            int texHeight = 1;
            while (true)
            {
                if (texWidth * texHeight >= numPixels) break;
                texWidth *= 2;
                if (texWidth * texHeight >= numPixels) break;
                texHeight *= 2;
            }

            skinBoneData.boneAnimations._textureWidth = texWidth;
            skinBoneData.boneAnimations._textureHeight = texHeight;
            skinBoneData.boneAnimations.slots = arrSlots;
           Texture2D texture = new Texture2D(texWidth, texHeight, TextureFormat.RGBAHalf, false, true);
            Color[] pixels = texture.GetPixels();
            int pixelIndex = 0;
            for (int clipIndex = 0; clipIndex < clips.Length; ++clipIndex)
            {
                SkeletonBoneAnimation clip = clips[clipIndex];
                SkeletonAnimationFrame[] frames = clip._frames;
                int numFrames = frames.Length;
                for (int frameIndex = 0; frameIndex < numFrames; ++frameIndex)
                {
                    SkeletonAnimationFrame frame = frames[frameIndex];
                    Matrix4x4[] matrices = frame._matrices;
                    int numMatrices = matrices.Length;
                    for (int matrixIndex = 0; matrixIndex < numMatrices; ++matrixIndex)
                    {
                        Matrix4x4 matrix = matrices[matrixIndex];
                        pixels[pixelIndex++] = new Color(matrix.m00, matrix.m01, matrix.m02, matrix.m03);
                        pixels[pixelIndex++] = new Color(matrix.m10, matrix.m11, matrix.m12, matrix.m13);
                        pixels[pixelIndex++] = new Color(matrix.m20, matrix.m21, matrix.m22, matrix.m23);
                    }
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(pixels);
            texture.Apply(false, false);
            texture.name = target.name.Replace("(Clone)", "");

            var pngData = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(bakePath + "/" + texture.name + "_anim.png", pngData);

            skinBoneData.setAniTex(texture);

            //  UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
        }
        //-----------------------------------------------------------------------------
        private void BakeVertexFrameData(GameObject target, SkinFrameVertexData skinData, SkinnedMeshRenderer skinRender, Mesh shareMesh)
        {
            if (!skinRender || skinData == null) return;
            Animator animator = target.GetComponent<Animator>();

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            int numPixels = 0;
            int numClips = clips.Length;
            Vector2[] pixelSegmentations = new Vector2[clips.Length];
            for (int clipIndex = 0; clipIndex < clips.Length; ++clipIndex)
            {
                AnimationClip clip = clips[clipIndex];
                pixelSegmentations[clipIndex].x = numPixels;

                int numFrames = Mathf.ClosestPowerOfTwo((int)(clip.frameRate * clip.length));
                numPixels += skinRender.sharedMesh.vertexCount * numFrames;
                pixelSegmentations[clipIndex].y = numPixels;
            }

            int texWidth = 1;
            int texHeight = 1;
            while (true)
            {
                if (texWidth * texHeight >= numPixels) break;
                texWidth *= 2;
                if (texWidth * texHeight >= numPixels) break;
                texHeight *= 2;
            }


            Texture2D animsTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBAHalf, false);
            animsTex.filterMode = FilterMode.Point;


            int curClipFrame = 0;
            float sampleTime = 0;
            float perFrameTime = 0;

            int offsetY = 0;
            for (int n = 0; n < clips.Length; ++n)
            {
                AnimationClip curAnim = clips[n];

                sampleTime = 0;
                curClipFrame = Mathf.ClosestPowerOfTwo((int)(curAnim.frameRate * curAnim.length));
                perFrameTime = curAnim.length / curClipFrame;

                for (int i = 0; i < curClipFrame; i++)
                {
                    curAnim.SampleAnimation(target, sampleTime);

                    Mesh mesh = new Mesh();
                    skinRender.BakeMesh(mesh);

                    for (int j = 0; j < mesh.vertexCount; j++)
                    {
                        Vector3 vertex = mesh.vertices[j];
                        animsTex.SetPixel(j, i + offsetY, new Color(vertex.x, vertex.y, vertex.z));
                    }

                    sampleTime += perFrameTime;
                }
                offsetY += curClipFrame;
            }
            animsTex.Apply();

            skinData._animsTex = new SkeletonAnimationTex(target.name.Replace("(Clone)", ""), offsetY, animsTex, pixelSegmentations, null);
        }
        //-----------------------------------------------------------------------------
        private static void BakeMeshFrameData(GameObject target, SkinFrameMeshData skinData, SkinnedMeshRenderer skinRender, SkinMeshBakerData meshData = null, float frameRate = -1, float animSpeed = 1f)
        {
            if (skinRender == null || skinData == null) return;
            Animator animator = target.GetComponent<Animator>();
            AnimationClip[] clips = null;
            if (meshData.controller != null)
            {
                animator.runtimeAnimatorController = meshData.controller;
                clips = animator.runtimeAnimatorController.animationClips;
            }
            if (meshData.animations != null && meshData.animations.Length > 0)
                clips = meshData.animations;

            int curClipFrame = 0;
            float sampleTime = 0;
            float perFrameTime = 0;
            skinData.animations = ASkeletonCpuData.CreateInstance();
            skinData.animations.animMeshs = new AnimFrameData[clips.Length];

            if (meshData != null)
            {
                skinData.animations._shareUV = meshData.UV;
                skinData.animations._offsets = meshData.offsets;
            }
            else
            {
                skinData.animations._shareUV = skinRender.sharedMesh.uv;
            }

            skinData.animations._trianges = skinRender.sharedMesh.triangles;
            for (int n = 0; n < clips.Length; ++n)
            {
                AnimationClip curAnim = clips[n];

                sampleTime = 0;
                float rate = curAnim.frameRate;
                if (frameRate > 0) rate = frameRate;

                curClipFrame = Mathf.ClosestPowerOfTwo((int)(rate * curAnim.length));
                perFrameTime = curAnim.length / curClipFrame;

                AnimFrameData frame = new AnimFrameData();
                skinData.animations.animMeshs[n] = frame;

                frame._name = curAnim.name;
                frame._speed = animSpeed;
                frame._loop = curAnim.isLooping;

                List<AnimMeshData> animFrames = new List<AnimMeshData>();
                //frame._frames = new AnimMeshData[curClipFrame];

                List<int> bakeFramIndexs = new List<int>();
                List<int> bakeFrams = new List<int>();
                SkinMeshBakerData.BakeFrame bakeFrame = meshData.getFrame(curAnim.name);
                if (bakeFrame != null)
                {
                    List<int> sets = new List<int>();
                    for (int i = 0; i < bakeFrame.frams.Count; ++i)
                    {
                        if (sets.Contains(bakeFrame.frams[i]))
                            continue;

                        bakeFrams.Add(bakeFrame.frams[i]);
                        sets.Add(bakeFrame.frams[i]);
                    }
                }

                Mesh mesh = new Mesh();
                for (int i = 0; i < curClipFrame; i++)
                {
                    if (bakeFrams.Contains(i))
                        continue;

                    curAnim.SampleAnimation(target, sampleTime);

                    mesh.Clear();
                    skinRender.BakeMesh(mesh);

                    AnimMeshData animData = new AnimMeshData();
                    animData.setMesh(mesh);
                    animData.time = sampleTime;

                    // frame._frames[i] = animData;
                    animFrames.Add(animData);
                    sampleTime += perFrameTime;
                }

                for (int i = 0; i < bakeFrams.Count; ++i)
                {
                    curAnim.SampleAnimation(target, bakeFrams[i] * perFrameTime);

                    mesh.Clear();
                    skinRender.BakeMesh(mesh);
                    AnimMeshData animData = new AnimMeshData();
                    animData.setMesh(mesh);
                    animData.time = bakeFrams[i] * perFrameTime;
                    animFrames.Add(animData);
                }

                animFrames.Sort(delegate (AnimMeshData a1, AnimMeshData a2)
                {
                    if (a1.time < a2.time) return -1;
                    if (a1.time > a2.time) return 1;
                    return 0;
                });
                curAnim.SampleAnimation(target,0);
                frame._frames = animFrames.ToArray();
                frame._keyFrameCount = frame._frames.Length;
                DestroyObject(mesh);
            }
        }
        //-----------------------------------------------------------------------------
        private static void CollectBones(List<SkeletonBone> bonesRet, Transform[] bones_smr, Matrix4x4[] bindposes, SkeletonBone parentBone, Transform curBoneTrans, int curBoneIndex)
        {
            //SkeletonBone currentBone = new SkeletonBone();
            //bonesRet.Add(currentBone);

            //int indexOfSmrBones = System.Array.IndexOf(bones_smr, curBoneTrans);
            //currentBone._transform = curBoneTrans;
            //currentBone._name = currentBone._transform.gameObject.name;
            //currentBone._bindpose = indexOfSmrBones == -1 ? Matrix4x4.identity : bindposes[indexOfSmrBones];
            //currentBone._parentBoneIndex = parentBone == null ? -1 : bonesRet.IndexOf(parentBone);

            //if (parentBone != null)
            //{
            //    parentBone._childrenBonesIndices[curBoneIndex] = bonesRet.IndexOf(currentBone);
            //}
            SkeletonBone currentBone = BuildBones(bonesRet, bones_smr, bindposes, parentBone, curBoneTrans, curBoneIndex);

            int numChildren = currentBone._transform.childCount;
            if (numChildren > 0)
            {
                currentBone._childrenBonesIndices = new int[numChildren];
                for (int i = 0; i < numChildren; ++i)
                {
                    CollectBones(bonesRet, bones_smr, bindposes, currentBone, currentBone._transform.GetChild(i), i);
                }
            }
        }
        //-----------------------------------------------------------------------------
        private static SkeletonBone BuildBones(List<SkeletonBone> bonesRet, Transform[] bones_smr, Matrix4x4[] bindposes, SkeletonBone parentBone, Transform curBoneTrans, int curBoneIndex)
        {
            SkeletonBone currentBone = new SkeletonBone();
            bonesRet.Add(currentBone);

            int indexOfSmrBones = System.Array.IndexOf(bones_smr, curBoneTrans);
            currentBone._transform = curBoneTrans;
            currentBone._name = currentBone._transform.gameObject.name;
            currentBone._bindpose = indexOfSmrBones == -1 ? Matrix4x4.identity : bindposes[indexOfSmrBones];
            currentBone._parentBoneIndex = parentBone == null ? -1 : bonesRet.IndexOf(parentBone);

            if (parentBone != null)
            {
                parentBone._childrenBonesIndices[curBoneIndex] = bonesRet.IndexOf(currentBone);
            }
            return currentBone;
        }
        //-----------------------------------------------------------------------------
        static UnityEditor.Animations.AnimatorState GetAnimationState(Animator pAnimator, AnimationClip clip, int layer = 0)
        {
            if (pAnimator == null || clip == null) return null;
            if (pAnimator.runtimeAnimatorController == null) return null;

            UnityEditor.Animations.AnimatorController targetController = (UnityEditor.Animations.AnimatorController)pAnimator.runtimeAnimatorController;
            List<UnityEditor.Animations.AnimatorState> list = GetStatesRecursive(targetController.layers[layer].stateMachine, true);
            for (int i = 0; i < list.Count; ++i)
            {
                if (clip == list[i].motion) return list[i];
            }
            return null;
        }
        //-----------------------------------------------------
        static public Dictionary<string, UnityEditor.Animations.AnimatorState> GetStatesRecursive(AnimatorController targetController, bool bChild = true)
        {
            if (targetController == null ) return null;

            Dictionary<string, UnityEditor.Animations.AnimatorState> vAnimator = new Dictionary<string, AnimatorState>();

            if (targetController.layers.Length <= 0)
                return null;
            List<UnityEditor.Animations.AnimatorState> list = GetStatesRecursive(targetController.layers[0].stateMachine, true);
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] == null || list[i].motion == null)
                    continue;
                vAnimator[list[i].name] = list[i];
            }
            return vAnimator;
        }
        //-----------------------------------------------------
        static public List<UnityEditor.Animations.AnimatorState> GetStatesRecursive(UnityEditor.Animations.AnimatorStateMachine sm, bool bChild = true)
        {
            List<UnityEditor.Animations.AnimatorState> list = new List<UnityEditor.Animations.AnimatorState>();

            for (int i = 0; i < sm.states.Length; i++)
            {
                list.Add(sm.states[i].state);
            }
            if (bChild)
            {
                for (int i = 0; i < sm.stateMachines.Length; i++)
                {
                    list.AddRange(GetStatesRecursive(sm.stateMachines[i].stateMachine));
                }
            }

            return list;
        }
    }
}
#endif