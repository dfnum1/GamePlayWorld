/********************************************************************
生成日期:	3:15:2022  11:59
类    名: 	Random
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace Framework.Base
{
    internal partial struct Random
    {
        public ulong randSeed;
        //-------------------------------------------------
        public Random(uint seed = 17)
        {
            randSeed = seed;
        }
        //-------------------------------------------------
        public uint Next()
        {
            randSeed = randSeed * 1103515245 + 36153;
            return (uint)(randSeed / 65536);
        }
        //-------------------------------------------------
        // range:[0 ~(max-1)]
        public uint Next(uint max)
        {
            return Next() % max;
        }
        //-------------------------------------------------
        public int Next(int max)
        {
            return (int)(Next() % max);
        }
        //-------------------------------------------------
        // range:[min~(max-1)]
        public uint Range(uint min, uint max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return this.Next(num) + min;
        }
        //-------------------------------------------------
        public int Range(int min, int max)
        {
            if (min >= max - 1)
                return min;
            int num = max - min;

            return this.Next(num) + min;
        }
    }
}
