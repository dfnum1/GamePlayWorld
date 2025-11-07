
#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    internal class SkinFrameMeshData : SkinFrameData
    {
        public ASkeletonCpuData animations = null;

        public override void clear()
        {
            base.clear();
            clearMesh();
            animations = null;
        }
        public override void dirty()
        {
            if (animations != null) animations.dirtyMesh();
        }
        public override void clearMesh()
        {
            if (animations != null) animations.clearMesh();
        }


        public override SkeletonSlot[] getSlots()
        {
            return animations.slots;
        }
        public override void setSlots(SkeletonSlot[] slots)
        {
            animations.slots = slots;
        }

        public override Material getShareMat()
        {
            if (!m_shareMat)
            {
                m_shareMat = new Material(Shader.Find("Hidden/HL/Role/CPUSkinning"));
            }
            return m_shareMat;
        }
        public override int getAnimCnt()
        {
            return animations.animMeshs.Length;
        }

        public override string getAnimState(int index)
        {
            return animations.animMeshs[index]._name;
        }
    }
}
#endif