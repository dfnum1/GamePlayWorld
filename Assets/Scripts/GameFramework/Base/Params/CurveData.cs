/********************************************************************
生成日期:	10:7:2019   12:11
类    名: 	CurveData
作    者:	HappLI
描    述:	曲线结构数据
*********************************************************************/

using UnityEngine;

namespace Framework.Data
{
    [System.Serializable]
    public struct CurveData : Base.IQuickSort<CurveData>
    {
        public enum EUsedFlag
        {
            Position = 1 << 0,
            Rotation = 1 << 1,
            Fov = 1 << 2,
            LookAt = 1 << 3,
        }

        public float time;
        public Vector3 position;
        public Vector3 inTan;
        public Vector3 outTan;
        public Quaternion rotation;
        public Vector3 lookat;
        public float fov;
        //-----------------------------------------------------
        public int CompareTo(int type, CurveData other)
        {
            if (time < other.time) return -1;
            if (time > other.time) return 1;
            return 0;
        }
#if UNITY_EDITOR
            [System.NonSerialized]
            public bool bExpand;
            [System.NonSerialized]
            public bool bSyncToCamera;

            [System.NonSerialized]
            public Transform lookatTrans;

            [System.NonSerialized]
            public float editorTime;

            //------------------------------------------------------
            public void Write(ref Framework.Data.BinaryUtil write)
            {
                write.WriteFloat(time);
                write.WriteVector3(position);
                write.WriteVector3(inTan);
                write.WriteVector3(outTan);
                write.WriteQuaternion(rotation);
                write.WriteVector3(lookat);
                write.WriteFloat(fov);
            }
#endif
        //------------------------------------------------------
        public void Read(ref Framework.Data.BinaryUtil write)
        {
            time = write.ToFloat();
            position = write.ToVec3();
            inTan = write.ToVec3();
            outTan = write.ToVec3();
            rotation = write.ToQuaternion();
            lookat = write.ToVec3();
            fov = write.ToFloat();
        }
        //------------------------------------------------------
        public void Destroy()
        {
        }
    }
}