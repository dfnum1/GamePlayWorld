#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Plugin
{
    internal class MeshUVData : MonoBehaviour
    {
        [System.Serializable]
        public class BakeFrame
        {
            [SerializeField] public string state = "";
            [SerializeField] public List<int> frams = new List<int>();
        }
        public Texture useTexture;
        public Texture gpuUseTexture;
        public Vector2[] offsets;
        public Vector2[] UV;

        public UnityEngine.Object fbxAsset;
        public RuntimeAnimatorController controller;

        public GameObject bakePrefab;

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

        public void Copy(MeshUVData outData)
        {
            outData.useTexture = useTexture;
            outData.gpuUseTexture = gpuUseTexture;
            outData.UV = UV;
            outData.offsets = offsets;
            outData.fbxAsset = fbxAsset;
            outData.bakeFrames = bakeFrames;
            outData.bakePrefab = bakePrefab;
            outData.controller = controller;
        }
    }
}
#endif
