
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Plugin
{
    internal class Points2Polygon
    {
        //-------------------------------------------------
        public static void Build(ref List<Vector3> Points)
        {
            if (Points == null || Points.Count <= 0)
                return;
            int MaxXPointIndex = 0;//选取x坐标最大的点
            for (int i = 0; i < Points.Count; i++)
            {
                Vector3 gp = Points[i];
                if (Points[MaxXPointIndex].x < gp.x)////选取x坐标最大的点
                {
                    MaxXPointIndex = i;
                }
                else if (Points[MaxXPointIndex].x < gp.x && Points[MaxXPointIndex].z > gp.z)//选取x坐标最大的点，如果最大x坐标点有多个，去y最小者
                {
                    MaxXPointIndex = i;
                }
            }
            //计算斜率
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == MaxXPointIndex)
                {
                    Vector3 pt = Points[MaxXPointIndex];
                    pt.y = float.MaxValue;
                    Points[MaxXPointIndex] = pt;
                }
                else
                {
                    if (Points[i].x == Points[MaxXPointIndex].x)//与最大x坐标的x相同的点,因为x坐标之差为零，所以取SLOPE最大值
                    {
                        Vector3 pt = Points[i];
                        pt.y = float.MaxValue;
                        Points[MaxXPointIndex] = pt;
                    }
                    else//计算斜率，注意正切函数在-0.5Pi和0.5Pi之间是单调递增的
                    {
                        Vector3 pt = Points[i];
                        pt.y = (Points[i].z - Points[MaxXPointIndex].z) / (Points[MaxXPointIndex].x - Points[i].x);
                        Points[i] = pt;
                    }
                }
            }
            QuickSortUp(ref Points);
        }
        //------------------------------------------------------
        static void Swap(ref List<Vector3> vList, int left, int right)
        {
            if (left == right) return;
            Vector3 temp = vList[left];
            vList[left] = vList[right];
            vList[right] = temp;
        }
        //-------------------------------------------------
        static void QuickSortUp(ref List<Vector3> vList, int start = -1, int end = -1)
        {
            if (vList == null || vList.Count <= 1) return;
            if (start == -1) start = 0;
            if (end == -1) end = vList.Count - 1;

            if (end <= start || start < 0 || end < 0)
                return;
            if (end - start == 1)
            {
                if (vList[start].y > vList[end].y)
                    Swap(ref vList, start, end);
                return;
            }

            int i;
            {
                i = start - 1;
                int j = end;
                Vector3 v = vList[end];
                for (; ; )
                {
                    while (i < end && vList[++i].y <v.y) ;
                    while (j > start && vList[--j].y > v.y) ;
                    if (i >= j) break;
                    Swap(ref vList, i, j);
                }
                Swap(ref vList, i, end);
            }
            if (i > start)
            {
                QuickSortUp(ref vList, start, i - 1);
            }

            if (i < end)
            {
                QuickSortUp(ref vList, i + 1, end);
            }
        }
    }
}
