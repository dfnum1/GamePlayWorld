#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Framework.Plugin
{
    public class SkinCombineData
    {
        public bool show = true;
        public string filename = "";
        public Texture combineTexture = null;
        public SkinMeshBakerData combineData;
        public GameObject target = null;
        public AnimatorController controller;
        public List<AnimationClip> animations = new List<AnimationClip>();
        public List<Material> materials = new List<Material>();
        public bool bExpandAnimacitons = false;
        public Animator playAnimator;
        public Dictionary<string, UnityEditor.Animations.AnimatorState> animatorStates;

        public string GetBakeExport()
        {
            string root = BakeGpuSkinEditorPreferences.GetSettings().exportRoot;
            if (!string.IsNullOrEmpty(root))
            {
                root = Path.Combine(root, Path.GetFileNameWithoutExtension(filename)).Replace("\\", "/");
                if (!root.EndsWith("/")) root += "/";
                return root;
            }
            return GetBakePath();
        }
        public string GetBakePath()
        {
            string bakeFile = System.IO.Path.GetDirectoryName(filename).Replace("\\", "/") + "/baker/";
            return bakeFile;
        }
        public string GetBakeFile()
        {
            string bakeFile = GetBakePath() + Path.GetFileNameWithoutExtension(filename) + ".baker";
            return bakeFile;
        }
    }
    public class SkinMeshPacker
    {
        class sTagUVData
        {
            public string tag;
            public Vector2[] uv;
        }
        class sUVData
        {
            public string tag;
            public Transform root;
            public Transform target;
            public Vector2[] uvList;
        }
        class STextureUVData
        {
            public List<sUVData> data = new List<sUVData>();
            public string texture;
        }
        class sSkinData
        {
            public Transform trans;
            public UnityEngine.Object assetFbx;
            public UnityEditor.Animations.AnimatorController controller;
            public AnimationClip[] animations;
            public SkinnedMeshRenderer[] skins;
            public Material[] materials;
            public int uvindex = 0;
            public bool bCollect = false;
        }
        class PackTextureData
        {
            public List<Texture2D> textures = new List<Texture2D>();
            public int packW;
            public int packH;
        }
        struct SizeEx
        {
            public int w;
            public int h;
        }

        static bool CheckCanPackOneTexture(sSkinData bakeData, int curSize, int totalSize)
        {
            for (int i = 0; i < bakeData.materials.Length; ++i)
            {
                if (bakeData.materials[i].mainTexture == null) continue;
                curSize += bakeData.materials[i].mainTexture.width * bakeData.materials[i].mainTexture.height;
            }
            return curSize <= totalSize;
        }

        public static Texture CombineSingle(SkinCombineData combine, int combineTexW = 1024, int combineTexH = 1024, string strShader = "Hidden/HL/Role/GPUSkinningInstance")
        {
            string bakerPath = combine.GetBakePath();
            if (!System.IO.Directory.Exists(bakerPath))
                System.IO.Directory.CreateDirectory(bakerPath);

            if (combine.combineData == null)
                combine.combineData = new SkinMeshBakerData();

            List<CombineInstance> combineInstances = new List<CombineInstance>();
            Material material = null;
            List<Transform> bones = new List<Transform>();
            List<PackTextureData> textures = new List<PackTextureData>();
            List<STextureUVData> uvList = new List<STextureUVData>();
            int uvCount = 0;

            int bakeTotalCnt = 0;
            List<sSkinData> vSkins = new List<sSkinData>();
            Dictionary<Transform, sSkinData> vBakes = new Dictionary<Transform, sSkinData>();

            SkinnedMeshRenderer[] skins = combine.target.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skins == null || skins.Length <= 0)
            {
                Debug.LogError(combine.filename + "没有skinmesh");
                return null;
            }
            if (skins.Length > 1)
            {
                Debug.LogError(combine.filename + " multiply render skin");
                return null;
            }
            sSkinData skin = new sSkinData();
            skin.assetFbx = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(combine.filename);
            skin.skins = skins;
            skin.animations = combine.animations.ToArray();
            skin.materials = combine.materials.ToArray();
            skin.trans = combine.target.transform;
            skin.controller = combine.controller;
            vSkins.Add(skin);
            if (skin.materials.Length <=0)
            {
                return null;
            }
            for (int i = 0; i < vSkins.Count; ++i)
            {
                vBakes.Add(vSkins[i].trans, vSkins[i]);
            }

            int backCnt = 0;
            int texIndex = 1;
            int curPixelSize = 0;
            int totalPixelSize = combineTexW * combineTexH;
            List<SizeEx> packs = new List<SizeEx>();
            EditorUtility.DisplayProgressBar("Bake Skin Mesh", "Bake...", 0);

            int totalTexture = 0;
            while (true)
            {
                int index = 0;
                sSkinData curSkin = vSkins[index];
                if (!CheckCanPackOneTexture(curSkin, curPixelSize, totalPixelSize))
                {
                    bool bFind = false;
                    for (int i = 0; i < vSkins.Count; ++i)
                    {
                        if (CheckCanPackOneTexture(vSkins[i], curPixelSize, totalPixelSize))
                        {
                            curSkin = vSkins[i];
                            index = i;
                            bFind = true;
                            break;
                        }
                    }
                    if (!bFind)
                    {
                        curPixelSize = 0;
                        texIndex++;
                    }
                }
                vSkins.RemoveAt(index);

                for (int k = 0; k < 1/*curSkin.skins.Length*/; ++k)
                {
                    List<Material> sortMaterial = new List<Material>(curSkin.materials);
                    //! check materials texture pack into one texture
                    for (int i = 0; i < sortMaterial.Count; ++i)
                    {
                        material = sortMaterial[i];

                        CombineInstance ci = new CombineInstance();
                        ci.mesh = curSkin.skins[k].sharedMesh;
                        ci.transform = curSkin.skins[k].transform.localToWorldMatrix;
                        combineInstances.Add(ci);

                        if (material.mainTexture != null)
                        {
                            Texture2D tex = material.mainTexture as Texture2D;
                            int uvindex = -1;
                            for (int t = 0; t < textures.Count; ++t)
                            {
                                if (textures[t].textures.Contains(tex))
                                {
                                    uvindex = t;
                                    break;
                                }
                            }
                            if (uvindex >= 0)
                            {
                                curSkin.uvindex = uvindex;
                                continue;
                            }
                            curPixelSize += tex.width * tex.height;
                            if (curPixelSize > totalPixelSize)
                            {
                                curPixelSize = 0;
                                texIndex++;
                            }
                            if (texIndex > textures.Count)
                            {
                                textures.Add(new PackTextureData());
                                uvList.Add(new STextureUVData());
                            }
                            curSkin.uvindex = texIndex - 1;

                            textures[texIndex - 1].textures.Add(tex);
                            totalTexture++;

                            uvList[texIndex - 1].data.Add(new sUVData() { tag = material.name, root = curSkin.trans, target = curSkin.skins[k].transform, uvList = curSkin.skins[k].sharedMesh.uv });
                            uvCount += curSkin.skins[k].sharedMesh.uv.Length;
                            backCnt++;
                        }
                        else
                            backCnt++;
                        EditorUtility.DisplayProgressBar("Bake Skin Mesh", "Back...", (float)backCnt / bakeTotalCnt);

                    }

                }

                if (vSkins.Count <= 0)
                    break;
            }


            EditorUtility.ClearProgressBar();


            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            EditorUtility.DisplayProgressBar("Pack Texture", "Pack...", 0);
            backCnt = 0;
            bakeTotalCnt = textures.Count;
            if(totalTexture >1)
            {
                for (int t = 0; t < textures.Count; ++t)
                {
                    for (int k = 0; k < textures[t].textures.Count; ++k)
                    {
                        if (textures[t].textures[k] == null) continue;
                        string texturePath1 = AssetDatabase.GetAssetPath(textures[t].textures[k]);
                        TextureImporter textureImp = AssetImporter.GetAtPath(texturePath1) as TextureImporter;
                        if (!textureImp.isReadable)
                        {
                            textureImp.isReadable = true;
                            AssetDatabase.ImportAsset(texturePath1);
                        }
                    }


                    Texture2D skinnedMeshAtlas = new Texture2D(combineTexW, combineTexH);
                    Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures[t].textures.ToArray(), 0);


                    if (skinnedMeshAtlas.format != TextureFormat.ARGB32 && skinnedMeshAtlas.format != TextureFormat.RGB24)
                    {
                        Texture2D newTexture = new Texture2D(skinnedMeshAtlas.width, skinnedMeshAtlas.height);
                        newTexture.SetPixels(skinnedMeshAtlas.GetPixels(0), 0);
                        skinnedMeshAtlas = newTexture;
                    }

                    var pngData = skinnedMeshAtlas.EncodeToPNG();
                    string texturePath = bakerPath + Path.GetFileNameWithoutExtension(combine.filename) + "_combine_" + t.ToString() + ".PNG";
                    string savePath = Application.dataPath.Replace("\\", "/").Replace("/Assets", "/") + texturePath;
                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                    File.WriteAllBytes(savePath, pngData);
                    savePath = "Assets" + savePath.Replace("\\", "/").Substring(Application.dataPath.Length);

                    //网格纹理坐标合并
                    Vector2[] atlasUVs = new Vector2[uvCount];
                    int j = 0;
                    for (int i = 0; i < uvList[t].data.Count; i++)
                    {
                        for (int k = 0; k < uvList[t].data[i].uvList.Length; ++k)
                        {
                            Vector2 newuv = uvList[t].data[i].uvList[k];
                            newuv.x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, newuv.x);
                            newuv.y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, newuv.y);
                            uvList[t].data[i].uvList[k] = newuv;
                        }

                        uvList[t].texture = texturePath;
                    }

                    Texture saveedSkinneMeshAtlas = AssetDatabase.LoadAssetAtPath<Texture>(savePath);

                    Material mat = new Material(Shader.Find(strShader));
                    mat.name = Path.GetFileNameWithoutExtension(combine.filename) + "_combine_" + t.ToString();
                    mat.SetTexture("_MainTex", saveedSkinneMeshAtlas);
                    mat.SetTexture("_DiffuseTex", saveedSkinneMeshAtlas);
                    mat.mainTexture = saveedSkinneMeshAtlas;
                    AssetDatabase.CreateAsset(mat, bakerPath + mat.name + ".mat");

                    for (int k = 0; k < textures[t].textures.Count; ++k)
                    {
                        if (textures[t].textures[k] == null) continue;
                        string texturePath1 = AssetDatabase.GetAssetPath(textures[t].textures[k]);
                        TextureImporter textureImp = AssetImporter.GetAtPath(texturePath1) as TextureImporter;
                        if (textureImp.isReadable)
                        {
                            textureImp.isReadable = false;
                            AssetDatabase.ImportAsset(texturePath1);
                        }
                    }

                    backCnt++;
                    EditorUtility.DisplayProgressBar("Bake Skin Mesh", "Back...", (float)backCnt / bakeTotalCnt);
                }
            }
            else
            {
                if(textures.Count>0 && textures[0].textures.Count > 0)
                {
                    combine.combineTexture = textures[0].textures[0];
                    combine.combineData.useTexture = textures[0].textures[0];
                    combine.combineData.gpuUseTexture = textures[0].textures[0];
                }
            }
           
            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayProgressBar("Mesk Prefab Create", "Create...", 0);
            backCnt = 0;
            bakeTotalCnt = vBakes.Count;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            Dictionary<Transform, List<sTagUVData>> vus = new Dictionary<Transform, List<sTagUVData>>();
            foreach (var db in vBakes)
            {
                if (db.Value.uvindex < 0 || db.Value.uvindex >= uvList.Count)
                    continue;
                for (int i = 0; i < uvList[db.Value.uvindex].data.Count; ++i)
                {
                    if (uvList[db.Value.uvindex].data[i].root == db.Key)
                    {
                        if (!vus.ContainsKey(uvList[db.Value.uvindex].data[i].target))
                        {
                            vus.Add(uvList[db.Value.uvindex].data[i].target, new List<sTagUVData>());
                        }
                        sTagUVData tagUV = new sTagUVData();
                        tagUV.tag = uvList[db.Value.uvindex].data[i].tag;
                        tagUV.uv = uvList[db.Value.uvindex].data[i].uvList;
                        vus[uvList[db.Value.uvindex].data[i].target].Add(tagUV);
                    }
                }
                if (vus.Count <= 0 || vus.Count > 2)
                {
                    backCnt++;
                    EditorUtility.DisplayProgressBar("Mesk Prefab Create", "Create...", (float)backCnt / bakeTotalCnt);

                    continue;
                }

                Animator animtor = db.Key.gameObject.GetComponent<Animator>();
                if (animtor != null)
                    animtor.runtimeAnimatorController = db.Value.controller;

                SkinMeshBakerData meshUVDb = new SkinMeshBakerData();
                meshUVDb.fbxAsset = db.Value.assetFbx;
                meshUVDb.controller = db.Value.controller;
                meshUVDb.animations = db.Value.animations;

                meshUVDb.useTexture = AssetDatabase.LoadAssetAtPath<Texture>(uvList[db.Value.uvindex].texture);
                meshUVDb.gpuUseTexture = meshUVDb.useTexture;
                List<sTagUVData> temp_vus = vus[vus.Keys.First()];
                temp_vus.Sort(delegate (sTagUVData left, sTagUVData right)
                {
                    return left.tag.CompareTo(right.tag);
                });

                meshUVDb.UV = temp_vus[0].uv;
                if (temp_vus.Count > 1)
                {
                    meshUVDb.offsets = new Vector2[temp_vus.Count];
                    meshUVDb.offsets[0] = Vector2.zero;
                    for (int i = 0; i < temp_vus.Count; ++i)
                    {
                        bool bHasDiff = false;
                        Vector2 offset = temp_vus[i].uv[0] - temp_vus[0].uv[0];
                        //   bool btest = false;
                        //  Vector2 offset = temp_vus[i][0] - temp_vus[0][0];
                        //for(int k = 0; k < temp_vus[i].Length; ++k)
                        //{
                        //    if(!btest)
                        //    {
                        //        offset = temp_vus[i][k] - temp_vus[0][k];
                        //    }
                        //    else
                        //    {
                        //        btest = true;
                        //        if (!Util.Vector3EqualInFailover(offset, temp_vus[i][k] - temp_vus[0][k]))
                        //        {
                        //            Debug.LogError("has uv offset different!");
                        //            bHasDiff = true;
                        //            break;
                        //        }
                        //    }
                        //}
                        if (!bHasDiff)
                        {
                            meshUVDb.offsets[i] = offset;
                        }
                    }
                }


                vus.Clear();

                string name = db.Key.name;
                // db.Key.gameObject.hideFlags = HideFlags.None;
                meshUVDb.Save();

                backCnt++;
                EditorUtility.DisplayProgressBar("Mesk Prefab Create", "Create...[" + name + "]", (float)backCnt / bakeTotalCnt);
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            return null;
        }
    }
}
#endif