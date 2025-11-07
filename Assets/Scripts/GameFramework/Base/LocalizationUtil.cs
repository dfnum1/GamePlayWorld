using System;

namespace Framework.Base
{
    public class LocalizationUtil
    {
        public static string Convert(uint id, string defaultStr = null)
        {
            if (!string.IsNullOrEmpty(defaultStr)) return defaultStr;
            return id.ToString();
        }
    }
}
