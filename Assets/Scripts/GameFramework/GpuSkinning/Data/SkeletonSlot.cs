#if !USE_SERVER
using Codice.Client.Common;
using UnityEngine;


namespace Framework.Plugin
{

    [System.Serializable]
    public class SkeletonSlot
    {
        [System.Serializable]
        public struct SlotFrameMatrix
        {
            public Vector3 pos;
            public Quaternion rot;
            public float scale;
        }

        [System.Serializable]
        public struct SlotFrames
        {
            public SlotFrameMatrix[] frames;
        }

        public string _name;
        public Vector3 _pos;
        public Vector3 _rot;

        public SlotFrames[] slotFrames;
#if UNITY_EDITOR
        [System.NonSerialized]
        public bool expand;
#endif
        public bool GetFrameTRS(int nAnimIndex, bool IsLoop, float fTime, float fDuration, int nFrameRate, out Vector3 vPos, out Quaternion vRot, out Vector3 vScale)
        {
            vPos = _pos;
            vRot = Quaternion.Euler(_rot);
            vScale = Vector3.one;
            if ( nAnimIndex < 0 || slotFrames == null || nAnimIndex >= slotFrames.Length)
                return false;

            var frames = slotFrames[nAnimIndex].frames;
            if (frames == null)
                return false;

            int frameCount = frames.Length;
            float percent = IsLoop ? Mathf.Repeat(fTime / fDuration, 1.0f) : Mathf.Min(fTime / fDuration, 1.0f);

            float cur = percent * (frameCount - 1);
            int leftFrame = Mathf.FloorToInt(cur);
            int rightFrame = Mathf.Min(leftFrame + 1, frameCount - 1);
            float t = cur - leftFrame;

            if(leftFrame >=0 && leftFrame < frames.Length &&
                rightFrame >= 0 && rightFrame < frames.Length)
            {
                vPos = Vector3.Lerp(frames[leftFrame].pos, frames[rightFrame].pos, t) + _pos;
                vRot = Quaternion.Slerp(frames[leftFrame].rot, frames[rightFrame].rot, t) * Quaternion.Euler(_rot);
                vScale = Vector3.one * Mathf.Lerp(frames[leftFrame].scale, frames[rightFrame].scale, t);
            }
            else if(leftFrame >= 0 && leftFrame < frames.Length)
            {
                vPos = frames[leftFrame].pos + _pos;
                vRot = frames[leftFrame].rot * Quaternion.Euler(_rot);
                vScale = Vector3.one * frames[leftFrame].scale;
            }
            else if(rightFrame >= 0 && rightFrame < frames.Length)
            {
                vPos = frames[rightFrame].pos + _pos;
                vRot = frames[rightFrame].rot * Quaternion.Euler(_rot);
                vScale = Vector3.one * frames[rightFrame].scale;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
#endif
