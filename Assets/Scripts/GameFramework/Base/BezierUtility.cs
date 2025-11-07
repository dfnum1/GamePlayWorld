#if USE_FIXEDMATH
using ExternEngine;
#endif
using UnityEngine;

namespace Framework.Base
{
    public static class BezierUtility
    {
        //-----------------------------------------------------
        public static Vector3 Bezier2(float t, Vector3 p1, Vector3 p2)
        {
            Vector3 val = p1 + t * (p2 - p1);

            return val;
        }
        //-----------------------------------------------------
        public static Vector3 Bezier3(float t, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 lbStart = Bezier2(t, p1, p2);
            Vector3 lbEnd = Bezier2(t, p2, p3);
            Vector3 val = lbStart + (lbEnd - lbStart) * t;
            return val;
        }
        //-----------------------------------------------------
        public static Vector3 Bezier4(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector3 lbStart = Bezier3(t, p1, p2, p3);
            Vector3 lbEnd = Bezier3(t, p2, p3, p4);
            Vector3 val = lbStart + (lbEnd - lbStart) * t;

            return val;
        }
#if USE_FIXEDMATH
        //-----------------------------------------------------
        public static ExternEngine.FVector3 FBezier2(ExternEngine.FFloat t, ExternEngine.FVector3 p1, ExternEngine.FVector3 p2)
        {
            FVector3 val = p1 + t * (p2 - p1);
            return val;
        }
        //-----------------------------------------------------
        public static ExternEngine.FVector3 FBezier3(ExternEngine.FFloat t, ExternEngine.FVector3 p1, ExternEngine.FVector3 p2, ExternEngine.FVector3 p3)
        {
            FVector3 lbStart = FBezier2(t, p1, p2);
            FVector3 lbEnd = FBezier2(t, p2, p3);
            FVector3 val = lbStart + (lbEnd - lbStart) * t;
            return val;
        }
        //-----------------------------------------------------
        public static ExternEngine.FVector3 FBezier4(ExternEngine.FFloat t, ExternEngine.FVector3 p1, ExternEngine.FVector3 p2, ExternEngine.FVector3 p3, ExternEngine.FVector3 p4)
        {
           ExternEngine.FVector3 lbStart = FBezier3(t, p1, p2, p3);
           ExternEngine.FVector3 lbEnd = FBezier3(t, p2, p3, p4);
           ExternEngine.FVector3 val = lbStart + (lbEnd - lbStart) * t;

            return val;
        }
#endif
    }
}
