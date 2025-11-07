#if !USE_SERVER
using UnityEngine;
using System.Collections;

namespace Framework.Plugin
{
    //-----------------------------------------------------------------------------
    // SkinFrameData
    //-----------------------------------------------------------------------------
    internal class SkeletonAnimationTex
    {
        public string       _name;
        public float        _animLen;
        public byte[]       _rawAnimMap;
        public int          _animMapWidth;
        public int          _animMapHeight;
        public Texture2D    _animMap;
        public Vector2[]    _offsets;
        public Vector2[]    _pixelSegmentations;
        public SkeletonSlot[] _slots;

        public SkeletonAnimationTex(string name, float animLen, Texture2D animMap, Vector2[] pixelSeg, Vector2[] _skin)
        {
            this._name = name;
            this._animLen = animLen;
            this._animMapHeight = animMap.height;
            this._animMapWidth = animMap.width;
            this._rawAnimMap = animMap.GetRawTextureData();
            this._animMap = animMap;
            this._pixelSegmentations = pixelSeg;
            this._offsets = _skin;
        }
    }
}
#endif