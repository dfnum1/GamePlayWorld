/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SplineData
作    者:	HappLI
描    述:	出生曲线管理器
*********************************************************************/
using Framework.Base;
using System.Collections.Generic;
using UnityEngine;
#if USE_SERVER
using AnimationCurve = ExternEngine.AnimationCurve;
#endif
namespace Framework.Core
{
    [System.Serializable]
    public struct SplineData
    {
        public struct KeyFrame : IQuickSort<KeyFrame>
        {
            public float time;
            public Vector3 position;
            public Vector3 eulerAngle;
            public Vector3 scale;
            public Vector3 inTan;
            public Vector3 outTan;

#if UNITY_EDITOR
            [System.NonSerialized]
            public bool bExpand;
            [System.NonSerialized]
            public float editorTime;
#endif
            public int CompareTo(int type, KeyFrame other)
            {
                if (time == other.time) return 0;
                if (time < other.time) return -1;
                return 1;
            }
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }

        public KeyFrame[] Frames;
        public AnimationCurve speedCurve;
        public string startAction;
        public string loopAction;
        public string endAction;
        //------------------------------------------------------
        public bool SetFrames(KeyFrame[] frames, AWorldNode pTrigger = null)
        {
            if (frames == null || frames.Length <= 1) return false;
            Frames = frames;
            Quaternion rotation = Quaternion.identity;
            if (pTrigger != null) rotation = Quaternion.Euler(pTrigger.GetEulerAngle());
            for (int i = 0; i < frames.Length; ++i)
            {
                KeyFrame kF = frames[i];
                if (pTrigger != null)
                {
                    Vector3 pos = BaseUtil.RoateAround(Vector3.zero, kF.position, rotation);
                    if (kF.outTan.sqrMagnitude > 0)
                    {
                        Vector3 outTan = BaseUtil.RoateAround(Vector3.zero, kF.outTan + kF.position, rotation);
                        kF.outTan = outTan - pos;
                    }
                    if (kF.inTan.sqrMagnitude > 0)
                    {
                        Vector3 inTan = BaseUtil.RoateAround(Vector3.zero, kF.inTan + kF.position, rotation);
                        kF.inTan = inTan - pos;
                    }
                    kF.position = pos;
                }
            }
            return true;
        }
        //------------------------------------------------------
        public bool GetLastFrame(ref Vector3 retPos, ref Vector3 retEuler, ref Vector3 retScale)
        {
            if (Frames == null || Frames.Length <= 0) return false;
            retPos = Frames[Frames.Length - 1].position;
            retEuler = Frames[Frames.Length - 1].eulerAngle;
            retScale = Frames[Frames.Length - 1].scale;
            return true;
        }
        //------------------------------------------------------
        public bool Evaluate(float fTime, ref Vector3 retPos, ref Vector3 retEuler, ref Vector3 retScale)
        {
            if (Frames == null || Frames.Length <= 0) return false;
            if (fTime <= Frames[0].time)
            {
                retPos = Frames[0].position;
                retEuler = Frames[0].eulerAngle;
                return true;
            }
            if (fTime >= Frames[Frames.Length - 1].time)
            {
                retPos = Frames[Frames.Length - 1].position;
                retEuler = Frames[Frames.Length - 1].eulerAngle;
                return true;
            }
            int __len = Frames.Length;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (fTime < Frames[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(Frames.Length - 1, __first);

            if (lhs < 0 || lhs >= Frames.Length || rhs < 0 || rhs >= Frames.Length)
                return false;

            KeyFrame lhsKey = Frames[lhs];
            KeyFrame rhsKey = Frames[rhs];

            float dx = rhsKey.time - lhsKey.time;
            Vector3 m1 = Vector3.zero, m2 = Vector3.zero;
            float t;
            if (dx != 0f)
            {
                t = (fTime - lhsKey.time) / dx;
            }
            else
                t = 0;

            m1 = lhsKey.position + lhsKey.outTan;
            m2 = rhsKey.position + rhsKey.inTan;
            retPos = BezierUtility.Bezier4(t, lhsKey.position, m1, m2, rhsKey.position);
            retEuler = Vector3.Slerp(lhsKey.eulerAngle, rhsKey.eulerAngle, t);
            retScale = Vector3.Slerp(lhsKey.scale, rhsKey.scale, t);
            return true;
        }

        public bool Read(ref Data.BinaryUtil reader)
        {
            int cnt = (int)reader.ToUshort();
            if (cnt <= 0) return false ;
            Frames = new KeyFrame[cnt];
            for(int i = 0; i < cnt; ++i)
            {
                KeyFrame key = new KeyFrame();
                key.time = reader.ToFloat();
                key.position = reader.ToVec3();
                key.eulerAngle = reader.ToVec3();
                key.scale = reader.ToVec3();
                key.inTan = reader.ToVec3();
                key.outTan = reader.ToVec3();
            }
            speedCurve = reader.ToCurve();
            startAction = reader.ToString();
            loopAction = reader.ToString();
            endAction = reader.ToString();
            return true;
        }
        public void Write(ref Data.BinaryUtil writer)
        {
            if (Frames == null || Frames.Length <= 0)
            {
                writer.WriteUshort(0);
                return;
            }
            writer.WriteUshort((ushort)Frames.Length);
            for(int i =0; i < Frames.Length; ++i)
            {
                writer.WriteFloat(Frames[i].time);
                writer.WriteVector3(Frames[i].position);
                writer.WriteVector3(Frames[i].eulerAngle);
                writer.WriteVector3(Frames[i].scale);
                writer.WriteVector3(Frames[i].inTan);
                writer.WriteVector3(Frames[i].outTan);
            }
            writer.WriteCurve(speedCurve);
            writer.WriteString(startAction);
            writer.WriteString(loopAction);
            writer.WriteString(endAction);
        }

        public void Copy(SplineData oth)
        {
            if(oth.Frames != null)
            {
                this.Frames = (new List<KeyFrame>(oth.Frames)).ToArray();
            }
            this.startAction = oth.startAction;
            this.loopAction = oth.loopAction;
            this.endAction = oth.endAction;
            this.speedCurve = oth.speedCurve;
        }
    }
}
