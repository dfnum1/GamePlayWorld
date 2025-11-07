#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    public class MaterialRef
    {
        public Material material = null;
        public Texture texture = null;
        public int refIndex = 0;

        public delegate MaterialRef MallocMaterialDefEvent(Texture mainTexture);
        public delegate void FreeMaterialDefEvent(MaterialRef refMat);
    }
}
#endif