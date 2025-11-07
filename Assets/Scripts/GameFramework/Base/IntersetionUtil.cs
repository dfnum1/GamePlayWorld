/********************************************************************
生成日期:	3:14:2022  16:20
类    名: 	IntersetionUtil
作    者:	HappLI
描    述:	
        //    1-----0
        //   /|    /|
        //  3-----2 |
        //  | 5-----4
        //  |/    |/
        //  7-----6   
*********************************************************************/
using Framework.Core;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Base
{
    public class IntersetionParam
    {
        public FVector3[] vDirArray1 = null;
        public FVector3[] vDirArray2 = null;
        public FVector3[] vVertexArray1 = null;
        public FVector3[] vVertexArray2 = null;
        //------------------------------------------------------
        public void Check()
        {
            if (vDirArray1 == null)
            {
                vDirArray1 = new FVector3[3];
                vDirArray2 = new FVector3[3];
                vVertexArray1 = new FVector3[8];
                vVertexArray2 = new FVector3[8];
            }
        }
    }

    public class IntersetionUtil
    {
        //------------------------------------------------------
        public static bool CU_AABBIntersection(FVector3 vMin0, FVector3 vMax0, FVector3 vMin1, FVector3 vMax1)
        {
            if ((vMin0.x > vMax1.x) || (vMin1.x > vMax0.x)) return (vMin0.x <= vMin1.x && vMin0.y <= vMin1.y && vMin0.z <= vMin1.z && vMax0.x >= vMax1.x && vMax0.y >= vMax1.y && vMax0.z >= vMax1.z);
            if ((vMin0.y > vMax1.y) || (vMin1.y > vMax0.y)) return (vMin0.x <= vMin1.x && vMin0.y <= vMin1.y && vMin0.z <= vMin1.z && vMax0.x >= vMax1.x && vMax0.y >= vMax1.y && vMax0.z >= vMax1.z);
            if ((vMin0.z > vMax1.z) || (vMin1.z > vMax0.z)) return (vMin0.x <= vMin1.x && vMin0.y <= vMin1.y && vMin0.z <= vMin1.z && vMax0.x >= vMax1.x && vMax0.y >= vMax1.y && vMax0.z >= vMax1.z);
            return true;
        }
        //------------------------------------------------------
        public static bool CU_SphereAABBInstersection(FVector3 Center, FFloat fRadius, FVector3 vMin1, FVector3 vMax1)
        {
            if (vMax1.x < Center.x - fRadius)
                return false;
            if (vMin1.x > Center.x + fRadius)
                return false;
            if (vMax1.y < Center.y - fRadius)
                return false;
            if (vMin1.y > Center.y + fRadius)
                return false;
            if (vMax1.z < Center.z - fRadius)
                return false;
            if (vMin1.z > Center.z + fRadius)
                return false;

            return true;
        }
        //------------------------------------------------------
        public static bool CU_SphereSphereInstersection(FVector3 Center, FFloat fRadius, FVector3 Center1, FFloat fRadius1)
        {
            FFloat radius = fRadius + fRadius1;
            return (Center - Center1).sqrMagnitude <= radius * radius;
        }
        //------------------------------------------------------
        public static bool CU_OBBOBBIntersection(Base.IntersetionParam frameParams, FVector3 vCenter1, FVector3 vHalf1, FMatrix4x4 mWorld1, FVector3 vCenter2, FVector3 vHalf2, FMatrix4x4 mWorld2)
        {
            FVector3 vTransCenter1 = mWorld1.MultiplyPoint(vCenter1);
            FVector3 vTransCenter2 = mWorld2.MultiplyPoint(vCenter2);

            frameParams.vDirArray1[0] = mWorld1.GetColumn(2); //dir
            frameParams.vDirArray1[1] = mWorld1.GetColumn(1); // up
            frameParams.vDirArray1[2] = mWorld1.GetColumn(0); //right
            frameParams.vVertexArray1[0] = vTransCenter1 + frameParams.vDirArray1[2] * vHalf1.x + frameParams.vDirArray1[1] * vHalf1.y + frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[1] = vTransCenter1 - frameParams.vDirArray1[2] * vHalf1.x + frameParams.vDirArray1[1] * vHalf1.y + frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[2] = vTransCenter1 + frameParams.vDirArray1[2] * vHalf1.x + frameParams.vDirArray1[1] * vHalf1.y - frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[3] = vTransCenter1 - frameParams.vDirArray1[2] * vHalf1.x + frameParams.vDirArray1[1] * vHalf1.y - frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[4] = vTransCenter1 + frameParams.vDirArray1[2] * vHalf1.x - frameParams.vDirArray1[1] * vHalf1.y + frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[5] = vTransCenter1 - frameParams.vDirArray1[2] * vHalf1.x - frameParams.vDirArray1[1] * vHalf1.y + frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[6] = vTransCenter1 + frameParams.vDirArray1[2] * vHalf1.x - frameParams.vDirArray1[1] * vHalf1.y - frameParams.vDirArray1[0] * vHalf1.z;
            frameParams.vVertexArray1[7] = vTransCenter1 - frameParams.vDirArray1[2] * vHalf1.x - frameParams.vDirArray1[1] * vHalf1.y - frameParams.vDirArray1[0] * vHalf1.z;

            frameParams.vDirArray2[0] = mWorld2.GetColumn(2); //dir
            frameParams.vDirArray2[1] = mWorld2.GetColumn(1); // up
            frameParams.vDirArray2[2] = mWorld2.GetColumn(0); //right

            frameParams.vVertexArray2[0] = vTransCenter2 + frameParams.vDirArray2[2] * vHalf2.x + frameParams.vDirArray2[1] * vHalf2.y + frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[1] = vTransCenter2 - frameParams.vDirArray2[2] * vHalf2.x + frameParams.vDirArray2[1] * vHalf2.y + frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[2] = vTransCenter2 + frameParams.vDirArray2[2] * vHalf2.x + frameParams.vDirArray2[1] * vHalf2.y - frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[3] = vTransCenter2 - frameParams.vDirArray2[2] * vHalf2.x + frameParams.vDirArray2[1] * vHalf2.y - frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[4] = vTransCenter2 + frameParams.vDirArray2[2] * vHalf2.x - frameParams.vDirArray2[1] * vHalf2.y + frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[5] = vTransCenter2 - frameParams.vDirArray2[2] * vHalf2.x - frameParams.vDirArray2[1] * vHalf2.y + frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[6] = vTransCenter2 + frameParams.vDirArray2[2] * vHalf2.x - frameParams.vDirArray2[1] * vHalf2.y - frameParams.vDirArray2[0] * vHalf2.z;
            frameParams.vVertexArray2[7] = vTransCenter2 - frameParams.vDirArray2[2] * vHalf2.x - frameParams.vDirArray2[1] * vHalf2.y - frameParams.vDirArray2[0] * vHalf2.z;

            bool bTest = TestIntersection(frameParams.vDirArray1, frameParams.vDirArray1, frameParams.vVertexArray1, frameParams.vDirArray2, frameParams.vDirArray2, frameParams.vVertexArray2);

            return bTest;
        }
        //------------------------------------------------------
        public static bool CU_WorldBoxIntersection(Base.IntersetionParam frameParams, ref WorldBoundBox box1, ref WorldBoundBox box2)
        {
            box1.GetPoints(ref frameParams.vDirArray1, ref frameParams.vVertexArray1);
            box2.GetPoints(ref frameParams.vDirArray2, ref frameParams.vVertexArray2);

            bool bTest = TestIntersection(frameParams.vDirArray1, frameParams.vDirArray1, frameParams.vVertexArray1, frameParams.vDirArray2, frameParams.vDirArray2, frameParams.vVertexArray2);
            return bTest;
        }
        //------------------------------------------------------
        public static bool CU_LineOBBIntersection(Base.IntersetionParam frameParams, FVector3 lineStart, FVector3 lineEnd, FVector3 vCenter, FVector3 vHalf, FMatrix4x4 mWorld)
        {
#if USE_FIXEDMATH
            FFloat result = FFloat.zero;
#else
            FFloat result = 0.0f;
#endif
            return CU_LineOBBIntersection(frameParams,out result, lineStart, lineEnd, vCenter, vHalf, mWorld);
        }
        //------------------------------------------------------
        public static bool CU_LineSphereIntersection(Base.IntersetionParam frameParams, FVector3 lineStart, FVector3 lineEnd, FVector3 vCenter, FFloat fRadius)
        {
            FFloat radius = fRadius * fRadius;
            return ((lineStart - vCenter).sqrMagnitude <= radius ||
                (lineEnd - vCenter).sqrMagnitude <= radius);
        }
        //------------------------------------------------------
        public static bool CU_WorldBoxIntersection(Base.IntersetionParam frameParams, WorldBoundBox box, FVector3 point)
        {
            FVector3 vMin = box.GetMin(true);
            FVector3 vMax = box.GetMax(true);

            FVector3 tempMin = FVector3.Min(vMin, vMax);
            FVector3 tempMax = FVector3.Max(vMin, vMax);
            if (point.x < tempMin.x || tempMin.x > tempMax.x ||
                point.y < tempMin.y || tempMin.y > tempMax.y ||
                point.z < tempMin.z || tempMin.z > tempMax.z)
                return false;

            return CU_OBBOBBIntersection(frameParams, box.GetCenter(false), box.GetHalf(), box.GetTransform(), point, FVector3.zero, FMatrix4x4.identity);
        }
        //------------------------------------------------------
        public static bool CU_LineOBBIntersection(Base.IntersetionParam frameParams, out FFloat result, FVector3 lineStart, FVector3 lineEnd, FVector3 vCenter, FVector3 vHalf, FMatrix4x4 mWorld)
        {
            result = 0;
#if USE_FIXEDMATH
            if ((lineStart - lineEnd).sqrMagnitude <= FFloat.one) return false;
#else
            if ((lineStart - lineEnd).sqrMagnitude <= 1.0f) return false;
#endif
            FVector3 vTransCenter = mWorld.MultiplyPoint(vCenter);
            if (FVector3.Dot(lineStart - vTransCenter, lineEnd - vTransCenter) >= 0)
            {
                return false;
            }
           frameParams.vDirArray1[0] = mWorld.GetColumn(2); //dir
           frameParams.vDirArray1[1] = mWorld.GetColumn(1); // up
           frameParams.vDirArray1[2] = mWorld.GetColumn(0); //right
           frameParams.vVertexArray1[0] = vTransCenter + frameParams.vDirArray1[2] * vHalf.x + frameParams.vDirArray1[1] * vHalf.y + frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[1] = vTransCenter - frameParams.vDirArray1[2] * vHalf.x + frameParams.vDirArray1[1] * vHalf.y + frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[2] = vTransCenter + frameParams.vDirArray1[2] * vHalf.x + frameParams.vDirArray1[1] * vHalf.y - frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[3] = vTransCenter - frameParams.vDirArray1[2] * vHalf.x + frameParams.vDirArray1[1] * vHalf.y - frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[4] = vTransCenter + frameParams.vDirArray1[2] * vHalf.x - frameParams.vDirArray1[1] * vHalf.y + frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[5] = vTransCenter - frameParams.vDirArray1[2] * vHalf.x - frameParams.vDirArray1[1] * vHalf.y + frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[6] = vTransCenter + frameParams.vDirArray1[2] * vHalf.x - frameParams.vDirArray1[1] * vHalf.y - frameParams.vDirArray1[0] * vHalf.z;
           frameParams.vVertexArray1[7] = vTransCenter - frameParams.vDirArray1[2] * vHalf.x - frameParams.vDirArray1[1] * vHalf.y - frameParams.vDirArray1[0] * vHalf.z;

            //back
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[0], frameParams.vVertexArray1[1], frameParams.vVertexArray1[4]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[1], frameParams.vVertexArray1[4], frameParams.vVertexArray1[5]))
            {
                return true;
            }

            //front
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[2], frameParams.vVertexArray1[3], frameParams.vVertexArray1[6]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[3], frameParams.vVertexArray1[6], frameParams.vVertexArray1[7]))
            {
                return true;
            }

            //top
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[0], frameParams.vVertexArray1[1], frameParams.vVertexArray1[2]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[1], frameParams.vVertexArray1[2], frameParams.vVertexArray1[3]))
            {
                return true;
            }

            //bottom
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[4], frameParams.vVertexArray1[5], frameParams.vVertexArray1[6]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[5], frameParams.vVertexArray1[6], frameParams.vVertexArray1[7]))
            {
                return true;
            }

            //left
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[1], frameParams.vVertexArray1[5], frameParams.vVertexArray1[3]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[5], frameParams.vVertexArray1[3], frameParams.vVertexArray1[7]))
            {
                return true;
            }

            //right
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[0], frameParams.vVertexArray1[4], frameParams.vVertexArray1[2]))
            {
                return true;
            }
            if (LineTriangleIntersectionInside(out result, lineStart, lineEnd, frameParams.vVertexArray1[4], frameParams.vVertexArray1[2], frameParams.vVertexArray1[6]))
            {
                return true;
            }
            return false;
        }
        //-----------------------------------------------------------------------
        static void GetInterval(ref FVector3[] v, ref FVector3 axis, ref FFloat min_interval, ref FFloat max_interval)
        {
            min_interval = max_interval = FVector3.Dot(axis, v[0]);
            for (int i = 1; i < 8; i++)
            {
                FFloat value = FVector3.Dot(axis, v[i]);
#if USE_FIXEDMATH
                min_interval = FMath.Min(min_interval, value);
                max_interval = FMath.Max(max_interval, value);
#else
                min_interval = System.Math.Min(min_interval, value);
                max_interval = System.Math.Max(max_interval, value);
#endif
            }
        }
        //-----------------------------------------------------------------------
        static bool TestIntersection(FVector3[] obb1_face_dir, FVector3[] obb1_edge_dir, FVector3[] obb1_vert,
                              FVector3[] obb2_face_dir, FVector3[] obb2_edge_dir, FVector3[] obb2_vert)
        {
#if USE_FIXEDMATH
            FFloat min1 = FFloat.zero, max1 = FFloat.zero, min2 = FFloat.zero, max2 = FFloat.zero;
#else
            FFloat min1 = 0.0f, max1 = 0.0f, min2 = 0.0f, max2 = 0.0f;
#endif
            for (int i = 0; i < 3; i++)
            {
                GetInterval(ref obb1_vert, ref obb1_face_dir[i], ref min1, ref max1);
                GetInterval(ref obb2_vert, ref obb1_face_dir[i], ref min2, ref max2);
                if (max1 < min2 || max2 < min1)
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                GetInterval(ref obb1_vert, ref obb2_face_dir[i], ref min1, ref max1);
                GetInterval(ref obb2_vert, ref obb2_face_dir[i], ref min2, ref max2);
                if (max1 < min2 || max2 < min1)
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    FVector3 axis = FVector3.Cross(obb1_edge_dir[i], obb2_edge_dir[j]);
                    GetInterval(ref obb1_vert, ref axis, ref min1, ref max1);
                    GetInterval(ref obb2_vert, ref axis, ref min2, ref max2);
                    if (max1 < min2 || max2 < min1)
                        return false;
                }
            }
            return true;
        }
        //-----------------------------------------------------------------------
        static bool LineTriangleIntersectionInside(out FFloat result, FVector3 lineStart, FVector3 lineEnd, FVector3 v0, FVector3 v1, FVector3 v2)
        {
            result = 0;
            FVector3 lineDir = (lineEnd - lineStart).normalized;
            FVector3 edge1 = v1 - v0;
            FVector3 edge2 = v2 - v0;

            FVector3 pvec = FVector3.Cross(lineDir, edge2);

            FFloat det = FVector3.Dot(edge1, pvec);

            FVector3 tvec;
            if (det > 0)
            {
                tvec = lineStart - v0;
            }
            else
            {
                tvec = v0 - lineStart;
                det = -det;
            }

            if (det < 0.0001f)
                return false;

            FFloat u = FVector3.Dot(tvec, pvec);
            if (u < 0.0f || u > det)
                return false;


            FVector3 qvec = FVector3.Cross(tvec, edge1);
            FFloat v = FVector3.Dot(lineDir, qvec);
            if (v < 0.0f || u + v > det)
                return false;

            FFloat t = FVector3.Dot(edge2, qvec) / det;
            if (t >= 0 && t * t <= (lineEnd - lineStart).sqrMagnitude)
            {
                result = t;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static void CU_GetBoundPoints(Base.IntersetionParam frameParams, WorldBoundBox box, out FVector3[] vDirArray, out FVector3[] vVertexArray)
        {
            box.GetPoints(ref frameParams.vDirArray1, ref frameParams.vVertexArray1);
            vDirArray = frameParams.vDirArray1;
            vVertexArray = frameParams.vVertexArray1; 
        }
        //-----------------------------------------------------
        public static bool BoundInView(Base.IntersetionParam frameParams, FMatrix4x4 clipMatrix, WorldBoundBox worldBounds)
        {
            worldBounds.GetPoints(ref frameParams.vDirArray1, ref frameParams.vVertexArray1);
            for (int i = 0; i < frameParams.vVertexArray1.Length; ++i)
            {
                if (PositionInView(clipMatrix, frameParams.vVertexArray1[i]))
                {
                    return true;
                }
            }
            return false;
        }
        //-----------------------------------------------------
        public static bool BoundInView(Matrix4x4 clipMatrix, UnityEngine.Bounds worldBounds)
        {
            if (PositionInView(clipMatrix, new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z))) return true;
            return false;
        }
#if USE_FIXEDMATH
        //-----------------------------------------------------
        public static bool BoundInView(FMatrix4x4 clipMatrix, ExternEngine.Bounds worldBounds)
        {
            if (PositionInView(clipMatrix, new FVector3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.max.x, worldBounds.min.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z))) return true;
            if (PositionInView(clipMatrix, new FVector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z))) return true;
            return false;
        }
        //-----------------------------------------------------
        public static bool PositionInView(FMatrix4x4 clipMatrix, FVector3 worldPos, float factor = 1f)
        {
            worldPos = clipMatrix.MultiplyPoint(worldPos);

            if (FMath.Abs(worldPos.x) < factor
             && FMath.Abs(worldPos.y) < factor
             && worldPos.z <= factor)
            {
                return true;
            }
            return false;
        }
#endif
        //-----------------------------------------------------
        public static bool PositionInView(Matrix4x4 clipMatrix, Vector3 worldPos, float factor = 1f)
        {
            return BaseUtil.PositionInView(clipMatrix, worldPos, factor);
        }

        //------------------------------------------------------
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
        {
            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);

            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;

                vector = lineVec.normalized * length;

                intersection = linePoint + vector;

                return true;
            }
            else
            {
                return false;
            }
        }
        //------------------------------------------------------
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }
#if USE_FIXEDMATH
    //------------------------------------------------------
    public static FVector3 ProjectPointOnPlane(FVector3 planeNormal, FVector3 planePoint, FVector3 point)
    {
        if (planeNormal.sqrMagnitude > FFloat.one) planeNormal = planeNormal.normalized;
        FFloat temp = FVector3.Dot(planeNormal, (point - planePoint));
        return point - planeNormal * temp;
    }
    //------------------------------------------------------
    public static FVector3 ProjectPointOnPlane(ref FFloat distance, FVector3 planeNormal, FVector3 planePoint, FVector3 point)
    {
        if (planeNormal.sqrMagnitude > FFloat.one) planeNormal = planeNormal.normalized;
        distance = FVector3.Dot(planeNormal, (point - planePoint));
        return point - planeNormal * distance;
    }
    //------------------------------------------------------
    public static FFloat PointDistancePlane(FVector3 planeNormal, FVector3 planePoint, FVector3 point)
    {
        if (planeNormal.sqrMagnitude > FFloat.one) planeNormal = planeNormal.normalized;
        return FVector3.Dot(planeNormal, (point - planePoint));
    }
    //------------------------------------------------------
    public static bool LineLineIntersectionF(out FVector3 intersection, FVector3 linePoint1, FVector3 linePoint2, FVector3 linePoint3, FVector3 linePoint4)
    {
        linePoint2.y = linePoint3.y = linePoint4.y = linePoint1.y;
        FVector3 lineVec1 = linePoint2 - linePoint1; lineVec1.Normalize();
        FVector3 lineVec2 = linePoint4 - linePoint3; lineVec2.Normalize();
        FVector3 lineVec3 = linePoint3 - linePoint1;
        FVector3 crossVec1and2 = FVector3.Cross(lineVec1, lineVec2);
        FVector3 crossVec3and2 = FVector3.Cross(lineVec3, lineVec2);

        FFloat planarFactor = FVector3.Dot(lineVec3, crossVec1and2);
        intersection = FVector3.zero;
        if (FMath.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            FFloat s = FVector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            if (s >= FFloat.zero && s < FFloat.one)
            {
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
                return false;
        }
        else
        {
            intersection = FVector3.zero;
            return false;
        }
    }
#endif
        //------------------------------------------------------
        public static bool SegLineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 linePoint2, Vector3 linePoint3, Vector3 linePoint4)
        {
            intersection = Vector3.zero;

            Vector3 ab = linePoint2 - linePoint1;
            Vector3 ca = linePoint1 - linePoint3;
            Vector3 cd = linePoint4 - linePoint3;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            Vector3 ad = linePoint4 - linePoint1;
            Vector3 cb = linePoint2 - linePoint3;
            // 快速排斥
            if (Mathf.Min(linePoint1.x, linePoint2.x) > Mathf.Max(linePoint3.x, linePoint4.x) || Mathf.Max(linePoint1.x, linePoint2.x) < Mathf.Min(linePoint3.x, linePoint4.x)
               || Mathf.Min(linePoint1.y, linePoint2.y) > Mathf.Max(linePoint3.y, linePoint4.y) || Mathf.Max(linePoint1.y, linePoint2.y) < Mathf.Min(linePoint3.y, linePoint4.y)
               || Mathf.Min(linePoint1.z, linePoint2.z) > Mathf.Max(linePoint3.z, linePoint4.z) || Mathf.Max(linePoint1.z, linePoint2.z) < Mathf.Min(linePoint3.z, linePoint4.z)
            )
                return false;

            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersection = linePoint1 + ab * ratio;
                return true;
            }

            return false;
        }
        //------------------------------------------------------
        public static bool SegLineLineIntersection2D(Vector2 linePoint1, Vector2 linePoint2, Vector2 linePoint3, Vector2 linePoint4, float factorFrom = 0, float factorTo = 1)
        {
            Vector2 intersection = Vector2.zero;
            return SegLineLineIntersection2D(out intersection, linePoint1, linePoint2, linePoint3, linePoint4, factorFrom, factorTo);
        }
        //------------------------------------------------------
        public static bool SegLineLineIntersection2D(out Vector2 intersection, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float factorFrom = 0, float factorTo = 1)
        {
            intersection = Vector2.zero;
            Vector2 dir1 = p2 - p1;
            Vector2 dir2 = p4 - p3;

            float den = dir2.y * dir1.x - dir2.x * dir1.y;

            if (Mathf.Abs(den) <= 0.01f)
            {
                return false;
            }

            float nom = dir2.x * (p1.y - p3.y) - dir2.y * (p1.x - p3.x);
            float nom2 = dir1.x * (p1.y - p3.y) - dir1.y * (p1.x - p3.x);

            float factor1 = (float)nom / den;
            float factor2 = (float)nom2 / den;

            if (factor1 >= factorFrom && factor1 <= factorTo && factor2 >= factorFrom && factor2 <= factorTo)
            {
                intersection = p1 + dir1.normalized * factor1;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool SegLineLineIntersection(Vector3 linePoint1, Vector3 linePoint2, Vector3 linePoint3, Vector3 linePoint4)
        {
            Vector3 intersection = Vector3.zero;
            return SegLineLineIntersection(out intersection, linePoint1, linePoint2, linePoint3, linePoint4);
        }
    }
}
