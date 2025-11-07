#if !USE_SERVER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Plugin
{
    public abstract class ASkeletonCpuData : ScriptableObject
    {
        public Vector2[] _shareUV;
        public Vector2[] _offsets;
        public int[] _trianges;
        [SerializeField]
        AnimFrameData[] _animMeshs;
        internal AnimFrameData[] animMeshs
        {
            get { return _animMeshs; }
            set { _animMeshs = value; }
        }
        [SerializeField]
        SkeletonSlot[] _slots;
        internal SkeletonSlot[] slots
        {
            get { return _slots; }
            set { _slots = value; }
        }

        private List<Vector2[]> m_uvs = null;

        public Vector2[] getUV(int uvindex = 0)
        {
            if (_offsets == null || uvindex >= _offsets.Length) return _shareUV;
            if (m_uvs == null) m_uvs = new List<Vector2[]>(_offsets.Length);
            if (m_uvs.Count != _offsets.Length)
            {
                for (int i = 0; i < _offsets.Length; ++i)
                    m_uvs.Add(null);
            }
            if (m_uvs[uvindex] == null)
            {
                m_uvs[uvindex] = new Vector2[_shareUV.Length];
                for (int i = 0; i < m_uvs[uvindex].Length; ++i)
                    m_uvs[uvindex][i] = _shareUV[i] + _offsets[uvindex];
            }
            return m_uvs[uvindex];
        }

        public byte getSkinCnt()
        {
            if (_offsets == null) return 1;
            return (byte)_offsets.Length;
        }

        public void clearMesh()
        {
            if (m_uvs != null) m_uvs.Clear();
            m_uvs = null;
            if (_animMeshs == null) return;
            for (int i = 0; i < _animMeshs.Length; ++i)
            {
                _animMeshs[i].clearMesh();
            }
        }
        public void dirtyMesh()
        {
            if (_animMeshs == null) return;
            for (int i = 0; i < _animMeshs.Length; ++i)
            {
                _animMeshs[i].dirtyMesh();
            }
        }
#if UNITY_EDITOR
        static System.Type ms_pInheritType = null;
        public static ASkeletonCpuData CreateInstance()
        {
            if (ms_pInheritType == null) ms_pInheritType = ED.EditorUtil.FindInheirtTypeType<ASkeletonCpuData>();
            if (ms_pInheritType == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("tips", "请定义一个类继承为ASkeletonCpuData", "Ok");
                return null;
            }
            return ScriptableObject.CreateInstance(ms_pInheritType) as ASkeletonCpuData;
        }
#endif
    }

    [System.Serializable]
    public class AnimMeshData
    {
        public Vector3[] _vertices;

#if UNITY_EDITOR
        [System.NonSerialized] public float time = 0f;
#endif
        [System.NonSerialized] public byte m_skin = 0;
        [System.NonSerialized] private Mesh[] m_mesh = null;
        [System.NonSerialized] private bool m_bDirty = false;

        public void setMesh(Mesh mesh)
        {
            //  m_mesh = mesh;
            _vertices = mesh.vertices;
        }

        public void clearMesh()
        {
            if (m_mesh == null) return;
            for (int i = 0; i < m_mesh.Length; ++i)
            {
                UnityEngine.Object.Destroy(m_mesh[i]);
                m_mesh[i] = null;
            }
        }

        public void dirtyMesh()
        {
            m_bDirty = true;
        }

        public Mesh getMesh(int[] trianges, Vector2[] uv, byte skin, byte maxSkin)
        {
            if (m_mesh == null)
            {
                if (maxSkin <= 0) maxSkin = 1;
                clearMesh();
                m_mesh = new Mesh[maxSkin];
                m_bDirty = true;
            }
            if (skin >= m_mesh.Length) skin = 0;
            if (m_mesh[skin] == null)
            {
                m_mesh[skin] = new Mesh();
                m_bDirty = true;
            }
            if (m_bDirty)
            {
                m_mesh[skin].vertices = _vertices;
                m_mesh[skin].triangles = trianges;
                m_mesh[skin].uv = uv;
                m_bDirty = false;
            }

            return m_mesh[skin];
        }
    }

    [System.Serializable]
    public class AnimFrameData : ISkinAnimData
    {
        public string _name;
        public uint actionTag;
        public int _keyFrameCount;
        public float _speed = 1;
        public bool _loop = false;
        public AnimMeshData[] _frames;

        public void clearMesh()
        {
            if (_frames == null) return;
            for (int i = 0; i < _frames.Length; ++i)
            {
                _frames[i].clearMesh();
            }
        }
        public void dirtyMesh()
        {
            if (_frames == null) return;
            for (int i = 0; i < _frames.Length; ++i)
            {
                _frames[i].dirtyMesh();
            }
        }
        public string GetName() { return _name; }
        public float GetSpeed() { return _speed; }
        public float GetDuration() { return _keyFrameCount/30; }
        public bool IsLoop() { return _loop; }
    }
}
#endif