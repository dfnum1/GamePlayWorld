#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    internal class SkinFrameVertexData : SkinFrameData
    {
        public SkeletonAnimationTex _animsTex = null;
        public override void clear()
        {
            base.clear();
            if (_animsTex != null) _animsTex._animMap = null;
            _animsTex = null;
        }
        public override SkeletonSlot[] getSlots()
        {
            return _animsTex._slots;
        }
        public override void setSlots(SkeletonSlot[] slots)
        {
            _animsTex._slots = slots;
        }

        public override Vector2[] getSkins() { return _animsTex._offsets; }


        public override int getAnimCnt()
        {
            return _animsTex._pixelSegmentations.Length;
        }

        public override string getAnimState(int index)
        {
            return index.ToString();
        }
    }
}
#endif