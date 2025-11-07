#if !USE_SERVER
using UnityEngine;

using System.Collections;

namespace Framework.Plugin
{
    [System.Serializable]
    internal class SkeletonAnimationFrame
    {
        public Matrix4x4[]    _matrices = null;

        public Quaternion     _rootMotionDeltaPositionQ;

        public float          _rootMotionDeltaPositionL;

        public Quaternion     _rootMotionDeltaRotation;

        [System.NonSerialized]
        private bool            m_rootMotionInvInit = false;
        [System.NonSerialized]
        private Matrix4x4       m_rootMotionInv;
        public Matrix4x4 RootMotionInv(int rootBoneIndex)
        {
            if (!m_rootMotionInvInit)
            {
                m_rootMotionInv = _matrices[rootBoneIndex].inverse;
                m_rootMotionInvInit = true;
            }
            return m_rootMotionInv;
        }
    }
}
#endif