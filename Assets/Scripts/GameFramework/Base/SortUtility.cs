/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SortUtility
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;

namespace Framework.Base
{
    //------------------------------------------------------
    public interface IQuickSort<T> : IUserData
    {
        int CompareTo(int type, T other);
    }
    //------------------------------------------------------
    public class SortUtility
    {
        public delegate int CompareFunction<T>(T left, T right) where T : IUserData;
        public delegate int CompareFunctionVar<T>(T left, T right) where T : System.IComparable;
        //------------------------------------------------------
        public static void Swap<T>(ref List<T> vList, int left, int right) where T : IUserData
        {
            if (left == right) return;
            T temp = vList[left];
            vList[left] = vList[right];
            vList[right] = temp;
        }
        //------------------------------------------------------
        public static void SwapVar<T>(ref List<T> vList, int left, int right) where T : System.IComparable
        {
            if (left == right) return;
            T temp = vList[left];
            vList[left] = vList[right];
            vList[right] = temp;
        }
        //------------------------------------------------------
        public static void QuickSort<T>(ref List<T> vList, CompareFunction<T> function, int start = -1, int end = -1) where T : IUserData
        {
            if (vList == null || vList.Count <= 1) return;
            if (start == -1) start = 0;
            if (end == -1) end = vList.Count - 1;

            if (end <= start || start < 0 || end < 0)
                return;
            if (end - start == 1)
            {
                if (function(vList[start], vList[end]) > 0)
                    Swap(ref vList, start, end);
                return;
            }

            int i;
            {
                i = start - 1;
                int j = end;
                T v = vList[end];
                for (;;)
                {
                    while (i < end && function(vList[++i], v) < 0) ;
                    while (j > start && function(vList[--j], v) > 0) ;
                    if (i >= j) break;
                    Swap(ref vList, i, j);
                }
                Swap(ref vList, i, end);
            }
            if (i > start)
            {
                QuickSort(ref vList, function, start, i - 1);
            }

            if (i < end)
            {
                QuickSort(ref vList, function, i + 1, end);
            }
        }
        //------------------------------------------------------
        public static void QuickSortVar<T>(ref List<T> vList, CompareFunctionVar<T> function, int start = -1, int end = -1) where T : System.IComparable
        {
            if (vList == null || vList.Count <= 1) return;
            if (start == -1) start = 0;
            if (end == -1) end = vList.Count - 1;

            if (end <= start || start < 0 || end < 0)
                return;
            if (end - start == 1)
            {
                if (function(vList[start], vList[end]) > 0)
                    SwapVar(ref vList, start, end);
                return;
            }

            int i;
            {
                i = start - 1;
                int j = end;
                T v = vList[end];
                for (;;)
                {
                    while (i < end && function(vList[++i], v) < 0) ;
                    while (j > start && function(vList[--j], v) > 0) ;
                    if (i >= j) break;
                    SwapVar(ref vList, i, j);
                }
                SwapVar(ref vList, i, end);
            }
            if (i > start)
            {
                QuickSortVar(ref vList, function, start, i - 1);
            }

            if (i < end)
            {
                QuickSortVar(ref vList, function, i + 1, end);
            }
        }
        //------------------------------------------------------
        public static void QuickSortUp<T>(ref List<T> vList, int useType=0, int start=-1, int end=-1) where T : IQuickSort<T>
        {
            if (vList == null || vList.Count<=1) return;
            if (start == -1) start = 0;
            if (end == -1) end = vList.Count-1;

            if (end <= start || start<0 || end <0)
                return;
            if (end - start == 1)
            {
                if (vList[start].CompareTo(useType, vList[end])>0)
                    Swap(ref vList, start, end);
                return;
            }

            int i;
            {
                i = start - 1;
                int j = end;
                T v = vList[end];
                for (;;)
                {
                    while (i < end && vList[++i].CompareTo(useType, v)<0) ;
                    while (j > start && vList[--j].CompareTo(useType, v)>0) ;
                    if (i >= j) break;
                    Swap(ref vList, i, j);
                }
                Swap(ref vList, i, end);
            }
            if (i > start)
            {
                QuickSortUp(ref vList, useType, start, i - 1);
            }

            if (i < end)
            {
                QuickSortUp(ref vList, useType, i + 1, end);
            }
        }
        //------------------------------------------------------
        public static void QuickSortDown<T>(ref List<T> vList, int useType = 0, int start = -1, int end = -1) where T : IQuickSort<T>
        {
            if (vList == null || vList.Count <= 1) return;
            if (start == -1) start = 0;
            if (end == -1) end = vList.Count - 1;

            if (end <= start || start < 0 || end < 0)
                return;

            if (end - start == 1)
            {
                if (vList[start].CompareTo(useType, vList[end]) < 0)
                    Swap(ref vList, start, end);
                return;
            }

            int i;
            {
                i = start - 1;
                int j = end;
                T v = vList[end];
                for (;;)
                {
                    while (i < end && vList[++i].CompareTo(useType, v) > 0) ;
                    while (j > start && vList[--j].CompareTo(useType, v) < 0) ;
                    if (i >= j) break;
                    Swap(ref vList, i, j);
                }
                Swap(ref vList, i, end);
            }
            if (i > start)
            {
                QuickSortDown(ref vList, useType, start, i - 1);
            }

            if (i < end)
            {
                QuickSortDown(ref vList, useType, i + 1, end);
            }
        }
    }
}

