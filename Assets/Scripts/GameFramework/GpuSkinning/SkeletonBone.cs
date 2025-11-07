#if !USE_SERVER
using UnityEngine;
using System.Collections;

namespace Framework.Plugin
{
    [System.Serializable]
    internal class SkeletonBone
    {
        public Transform        _transform = null;
        public Matrix4x4        _bindpose;
        public int              _parentBoneIndex = -1;
        public int[]            _childrenBonesIndices = null;

        public string           _name = null;

        [System.NonSerialized]
        public Matrix4x4        m_animationMatrix;
        [System.NonSerialized]
        private bool            m_bindposeInvInit = false;
        [System.NonSerialized]
        private Matrix4x4       m_bindposeInv;
        public Matrix4x4 _BindposeInv
        {
            get
            {
                if (!m_bindposeInvInit)
                {
                    m_bindposeInv = _bindpose.inverse;
                    m_bindposeInvInit = true;
                }
                return m_bindposeInv;
            }
        }
    }
}
#endif