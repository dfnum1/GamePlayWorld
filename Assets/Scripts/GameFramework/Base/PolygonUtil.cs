using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Base
{
    public class PolygonUtil
    {
        //------------------------------------------------------
        public static int getOrientation2D(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3) return 0;
            int i1, i2;
            i1 = i2 = 0;
            float area = 0f;
            for (; i1 < polygon.Count; ++i1)
            {
                i2 = i1 + 1;
                if (i2 == polygon.Count) i2 = 0;
                area += polygon[i1].x * polygon[i2].y - polygon[i1].y * polygon[i2].x;
            }
            if (area > 0f) return 1;   //CCW
            if (area < 0f) return -1;  //CW
            return 0;
        }
        //------------------------------------------------------
        public static bool isConvex2D(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3) return false;
            if (polygon.Count <= 3) return true;

            if (getOrientation2D(polygon) == -1)
            {
                polygon.Reverse();
            }

            for (int i = 0; i < polygon.Count; ++i)
            {
                if (!isConvexIndex2D(polygon, i)) return false;
            }

            return true;
        }
        //------------------------------------------------------
        static bool isConvexIndex2D(List<Vector2> polygon, int index)
        {
            int nCount = polygon.Count;
            if (index < 0 || index >= nCount) return false;
            if (nCount < 4)
            {
                return false;
            }

            Vector2 prev, mid, next;
            prev = polygon[(nCount + index - 1) % nCount];
            mid = polygon[(nCount + index + 0) % nCount];
            next = polygon[(nCount + index + 1) % nCount];
            return ((next.x - mid.x) * (prev.y - mid.y) - (prev.x - mid.x) * (next.y - mid.y)) > 0f;
        }
        //------------------------------------------------------
        public static int getOrientation(List<Vector3> polygon)
        {
            if (polygon == null || polygon.Count < 3) return 0;
            int i1, i2;
            i1 = i2 = 0;
            float area = 0f;
            for (; i1 < polygon.Count; ++i1)
            {
                i2 = i1 + 1;
                if (i2 == polygon.Count) i2 = 0;
                area += polygon[i1].x * polygon[i2].z - polygon[i1].z * polygon[i2].x;
            }
            if (area > 0f) return 1;   //CCW
            if (area < 0f) return -1;  //CW
            return 0;
        }
        //------------------------------------------------------
        public static bool isConvex(List<Vector3> polygon)
        {
            if (polygon == null || polygon.Count < 3) return false;
            if (polygon.Count <= 3) return true;

            if (getOrientation(polygon) == -1)
            {
                polygon.Reverse();
            }

            for (int i = 0; i < polygon.Count; ++i)
            {
                if (!isConvexIndex(polygon,i)) return false;
            }

            return true;
        }
        //------------------------------------------------------
        static bool isConvexIndex(List<Vector3> polygon, int index)
        {
            int nCount = polygon.Count;
            if (index < 0 || index >= nCount) return false;
            if (nCount < 4)
            {
                return false;
            }

            Vector3 prev, mid, next;
            prev = polygon[(nCount + index - 1) % nCount];
            mid = polygon[(nCount + index + 0) % nCount];
            next = polygon[(nCount + index + 1) % nCount];
            return ((next.x - mid.x) * (prev.z - mid.z) - (prev.x - mid.x) * (next.z - mid.z)) > 0f;
        }
        //------------------------------------------------------
        public static void ExternPolygon(List<Vector3> polygon, float fExtern, bool checkConvex = true)
        {
            if (polygon == null || polygon.Count < 3) return;
            if (Mathf.Abs(fExtern) <= 0.0001f)
                return;
            if (checkConvex)
            {
                if (isConvex(polygon))
                {
                    Vector3 center = Vector3.zero;
                    for (int i = 0; i < polygon.Count; ++i)
                    {
                        center += polygon[i];
                    }
                    center /= polygon.Count;
                    for (int i = 0; i < polygon.Count; ++i)
                        polygon[i] += (polygon[i] - center).normalized * fExtern;
                }
                else
                {
                    List<Vector3> vList = new List<Vector3>();
                    for (int i = 0; i < polygon.Count; ++i)
                        vList.Add(polygon[i]);
                    for (int i = 0; i < vList.Count; ++i)
                    {
                        int inext = (i + 1) % vList.Count;
                        int iprev = i - 1; if (iprev < 0) iprev = vList.Count - 1;
                        Vector3 next = (vList[inext] - vList[i]).normalized; next.y = 0;
                         Vector3 prev = (vList[iprev] - vList[i]).normalized; prev.y = 0;
                        float dd = prev.x * next.z - prev.z * next.x;
                        if (dd>0)
                            polygon[i] = vList[i] + (next + prev).normalized * fExtern;
                        else
                            polygon[i] = vList[i] - (next + prev).normalized * fExtern;
                    }
                    vList.Clear();
                }
            }
            else
            {
                Vector3 center = Vector3.zero;
                for (int i = 0; i < polygon.Count; ++i)
                {
                    center += polygon[i];
                }
                center /= polygon.Count;
                for (int i = 0; i < polygon.Count; ++i)
                    polygon[i] += (polygon[i] - center).normalized * fExtern;
            }
        }
        //-----------------------------------------------------
        public static Vector3 GetPolygonCenter(List<Vector3> polyPoints)
        {
            if (isConvex(polyPoints))
            {
                Vector3 center = Vector3.zero;
                for (int i = 0; i < polyPoints.Count; ++i)
                {
                    center += polyPoints[i];
                }
                center /= polyPoints.Count;
                return center;
            }
            else
            {
                Vector3 innerPt = Vector3.zero;
                for (int i = 0; i < polyPoints.Count; ++i)
                {
                    int inext = (i + 1) % polyPoints.Count;
                    int iprev = i - 1; if (iprev < 0) iprev = polyPoints.Count - 1;
                    Vector3 next = polyPoints[inext] - polyPoints[i];
                    Vector3 prev = polyPoints[iprev] - polyPoints[i];
                    if (Vector3.Dot(prev, next) > 0)
                    {
                        innerPt = polyPoints[i] + (next + prev).normalized * 0.1f;
                        break;
                    }
                }
                return innerPt;
            }
        }
        //------------------------------------------------------
        public static int SegLineInsectionPolygonHit(Vector3 p0, Vector3 p1, List<Vector3> polyPoints)
        {
            if (polyPoints == null || polyPoints.Count < 2) return 0;
            int inter = 0;
            for (int i =0; i < polyPoints.Count; ++i)
            {
                int inext = (i + 1)% polyPoints.Count;
                if (IntersetionUtil.SegLineLineIntersection(p0, p1, polyPoints[i], polyPoints[inext]))
                {
                    inter++;
                }
            }
            return inter;
        }
        //------------------------------------------------------
        public static bool ContainsConvexPolygonPoint(List<Vector3> polyPoints, Vector3 p, bool checkConvex = true)
        {
            if (polyPoints == null || polyPoints.Count < 3) return false;
            if(checkConvex)
            {
                if(isConvex(polyPoints))
                {
                    var j = polyPoints.Count - 1;
                    var inside = false;
                    Vector3 pi, pj;
                    for (int i = 0; i < polyPoints.Count; j = i++)
                    {
                        pi = polyPoints[i];
                        pj = polyPoints[j];
                        if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                            (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                            inside = !inside;
                    }
                    return inside;
                }
                else
                {
                    Vector3 vMin = float.MaxValue * Vector3.one;
                    Vector3 vMax = float.MinValue * Vector3.one;
                    for (int i =0; i < polyPoints.Count; ++i)
                    {
                        vMax = Vector3.Max(vMax, polyPoints[i]);
                        vMin = Vector3.Min(vMin, polyPoints[i]);
                    }
                    Vector3 outPt = vMin + (vMax - vMin) * 10;
                    if(BaseUtil.Equal(p, outPt))
                    {
                        outPt = vMin - (vMax - vMin) * 10;
                    }
                    int inter = 0;
                    for(int i =0; i < polyPoints.Count; ++i)
                    {
                        if (BaseUtil.Equal(p, polyPoints[i], 0.001f))
                        {
                            inter = 1;
                            break;
                        }
                        else
                        {
                            int inext = (i + 1) % polyPoints.Count;
                            if (IntersetionUtil.SegLineLineIntersection(p, outPt, polyPoints[i], polyPoints[inext]))
                            {
                                inter++;
                            }
                        }

                    }
                    return inter % 2 != 0;
                }
            }
            else
            {
                var j = polyPoints.Count - 1;
                var inside = false;
                Vector3 pi, pj;
                for (int i = 0; i < polyPoints.Count; j = i++)
                {
                    pi = polyPoints[i];
                    pj = polyPoints[j];
                    if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) &&
                        (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x))
                        inside = !inside;
                }
                return inside;
            }
        }
        //------------------------------------------------------
        public static bool ContainsConvexPolygonPoint(List<Vector2> polyPoints, Vector2 p, bool checkConvex = true)
        {
            if (polyPoints == null || polyPoints.Count < 3) return false;
            if (checkConvex)
            {
                if (isConvex2D(polyPoints))
                {
                    var j = polyPoints.Count - 1;
                    var inside = false;
                    Vector2 pi, pj;
                    for (int i = 0; i < polyPoints.Count; j = i++)
                    {
                        pi = polyPoints[i];
                        pj = polyPoints[j];
                        if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                            (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                            inside = !inside;
                    }
                    return inside;
                }
                else
                {
                    Vector2 vMin = float.MaxValue * Vector2.one;
                    Vector2 vMax = float.MinValue * Vector2.one;
                    for (int i = 0; i < polyPoints.Count; ++i)
                    {
                        vMax = Vector2.Max(vMax, polyPoints[i]);
                        vMin = Vector2.Min(vMin, polyPoints[i]);
                    }
                    Vector2 outPt = vMin + (vMax - vMin) * 10;
                    if (BaseUtil.Equal(p, outPt))
                    {
                        outPt = vMin - (vMax - vMin) * 10;
                    }
                    int inter = 0;
                    for (int i = 0; i < polyPoints.Count; ++i)
                    {
                        if (BaseUtil.Equal(p, polyPoints[i], 0.001f))
                        {
                            inter = 1;
                            break;
                        }
                        else
                        {
                            int inext = (i + 1) % polyPoints.Count;
                            if (IntersetionUtil.SegLineLineIntersection2D(p, outPt, polyPoints[i], polyPoints[inext], 0, 1))
                            {
                                inter++;
                            }
                        }

                    }
                    return inter % 2 != 0;
                }
            }
            else
            {
                var j = polyPoints.Count - 1;
                var inside = false;
                Vector2 pi, pj;
                for (int i = 0; i < polyPoints.Count; j = i++)
                {
                    pi = polyPoints[i];
                    pj = polyPoints[j];
                    if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                        (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                        inside = !inside;
                }
                return inside;
            }
        }
        //-----------------------------------------------------
        public static bool ContainsPolygonInPolygon(List<Vector3> checkPly, List<Vector3> polygon, bool checkConvex = false)
        {
            if (polygon == null || polygon.Count <= 3 || checkPly == null) return false;

            Vector3 vMin1 = float.MaxValue * Vector3.one;
            Vector3 vMax1 = float.MinValue * Vector3.one;
            for (int i = 0; i < checkPly.Count; ++i)
            {
                vMax1 = Vector3.Max(vMax1, checkPly[i]);
                vMin1 = Vector3.Min(vMin1, checkPly[i]);
            }

            Vector3 vMin2 = float.MaxValue * Vector3.one;
            Vector3 vMax2 = float.MinValue * Vector3.one;
            for (int i = 0; i < polygon.Count; ++i)
            {
                vMax2 = Vector3.Max(vMax2, polygon[i]);
                vMin2 = Vector3.Min(vMin2, polygon[i]);
            }
            if (vMin1.x < vMin2.x || vMin1.x > vMax2.x ||
                vMin1.z < vMin2.z || vMin1.z > vMax2.z)
                return false;

            if (checkConvex)
            {
                if (isConvex(polygon))
                {
                    for (int i = 0; i < checkPly.Count; ++i)
                    {
                        if (!ContainsConvexPolygonPoint(polygon, checkPly[i], false))
                            return false;
                    }
                }
                else
                {
                    Vector3 dir = (vMax2 - vMin2);
                    bool bAllIn = true;
                    for (int p = 0; p < checkPly.Count; ++p)
                    {
                        Vector3 outPt = vMin2 + (vMax2 - vMin2) * 10;
                        if (BaseUtil.Equal(checkPly[p], outPt))
                        {
                            outPt = vMin2 - (vMax2 - vMin2) * 10;
                        }

                        int inter = 0;
                        for (int i = 0; i < polygon.Count; ++i)
                        {
                            int inext = (i + 1) % polygon.Count;
                            if(BaseUtil.Equal(checkPly[p], polygon[i], 0.001f))
                            {
                                inter = 1;
                                break;
                            }
                            else if (IntersetionUtil.SegLineLineIntersection(checkPly[p], outPt, polygon[i], polygon[inext]))
                            {
                                inter++;
                            }
                        }
                        if(inter%2==0)
                        {
                            bAllIn = false;
                            break;
                        }
                    }
                    return bAllIn;
                }
            }
            else
            {
                for (int i = 0; i < checkPly.Count; ++i)
                {
                    if (!ContainsConvexPolygonPoint(polygon, checkPly[i], false))
                        return false;
                }
            }

            return true;
        }
    }
}
