using System;
using System.Collections.Generic;
using System.Linq;

namespace GMTK2020
{
    public static class Utility
    {
        public static IEnumerable<TEnum> GetEnumValues<TEnum>() where TEnum : struct, Enum
            => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        public static TEnum ParseEnum<TEnum>(string enumStr, bool ignoreCase = false) where TEnum : struct, Enum
            => (TEnum)Enum.Parse(typeof(TEnum), enumStr, ignoreCase);
    } 
}
