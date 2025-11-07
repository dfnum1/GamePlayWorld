#if !USE_SERVER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Plugin
{
    public abstract class ASkeletonGpuData : ScriptableObject
    {
        [HideInInspector,SerializeField]
        ShareMeshData _shareMeshData;
        internal ShareMeshData shareMeshData
        {
            get { return _shareMeshData; }
            set { _shareMeshData = value; }
        }
        public int          _rootIndex = -1;
        [HideInInspector] public Vector3      _rootMotionPosition;
        [HideInInspector] public Quaternion   _rootMotionRotation;
        public int          _textureWidth = 0;
        public int          _textureHeight = 0;
        [SerializeField]
        SkeletonBoneAnimation[]  _animations;
        internal SkeletonBoneAnimation[] animations
        {
            get { return _animations; }
            set { _animations = value; }
        }

        [SerializeField]
        SkeletonSlot[] _slots;
        internal SkeletonSlot[] slots
        {
            get { return _slots; }
            set { _slots = value; }
        }
        public int          _skeletonLen = 0;
        public Vector2[]    _offsets;

        [HideInInspector]
        public byte[]       _texDatas;

        [System.NonSerialized]
        SkeletonBone[] _skeleton;
        internal SkeletonBone[] skeleton
        {
            get { return _skeleton; }
            set { _skeleton = value; }
        }
#if UNITY_EDITOR
        public void setTex(Texture2D pTex)
        {
            _texDatas = pTex.GetRawTextureData();
        }
        static System.Type ms_pInheritType =null;
        public static ASkeletonGpuData CreateInstance()
        {
            if (ms_pInheritType == null) ms_pInheritType = ED.EditorUtil.FindInheirtTypeType<ASkeletonGpuData>();
            if (ms_pInheritType == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("tips", "请定义一个类继承为ASkeletonGpuData", "Ok");
                return null;
            }
            return ScriptableObject.CreateInstance(ms_pInheritType) as ASkeletonGpuData;
        }
#endif

    }

    [System.Serializable]
    public class ShareMeshData
    {
        public Vector3[]    _vertices;
        public int[]        _triangles;
        public Vector2[]    _uv;
        public Vector2[]    _offsets;
        public List<Vector4>    _uv1 = new List<Vector4>();
        public List<Vector4>    _uv2 = new List<Vector4>();

#if UNITY_EDITOR
        public void setShareMesh(Mesh mesh)
        {
            _vertices = mesh.vertices;
            _triangles = mesh.triangles;
            _uv = mesh.uv;

            mesh.GetUVs(1, _uv1);
            mesh.GetUVs(2, _uv2);
        }
#endif

        public Mesh getShareMesh()
        {
            Mesh shareMesh = new Mesh();
            shareMesh.vertices = _vertices;
            shareMesh.triangles = _triangles;
            shareMesh.uv = _uv;
            shareMesh.SetUVs(1, _uv1);
            shareMesh.SetUVs(2, _uv2);
            return shareMesh;
        }
    }

    [System.Serializable]
    internal class SkeletonBoneAnimation : ISkinAnimData
    {
        public string                   _animName = null;
        public uint                      _actionTag = 0;
        public float                    _length = 0;/*seconds*/
        public int                      _fps = 0;
        public bool                     _loop = true;
        public float                    _speed  =1;

        public int                      _pixelSegmentation = 0;
        //     public bool                     _rootMotionEnabled = false;
        //   public bool                     _individualDifferenceEnabled = false;

        [System.NonSerialized]
        public SkeletonAnimationFrame[] _frames = null;

        public string GetName() { return _animName; }
        public float GetSpeed() { return _speed; }
        public bool IsLoop() { return _loop; }
        public float GetDuration() { return _length; }
    }
}


#endif