/********************************************************************
生成日期:	2020-6-16
类    名: 	GridNode
作    者:	HapplI
描    述:	格子状态
*********************************************************************/

namespace Framework.Core
{
    public enum EGridState : byte
    {
        Wakeable  = 0,
        Road =1,
        Obstacle = 2,
    }
    public struct GridNode
    {
        public int gridState;
        public int posY;
        public bool IsValid
        {
            get
            {
                return gridState !=0;
            }
        }
        public static GridNode DEF = new GridNode() { gridState = 0 };

        public bool IsState(EGridState state)
        {
            return (gridState & (int)(1 << (int)state)) != 0;
        }
        public bool IsState(uint flags)
        {
            return (gridState & flags) != 0;
        }
    }
}

