using System;
using System.Collections.Generic;
using System.Linq;

namespace GMTK2020
{
    public static class Utility
    {
        public static IEnumerable<TEnum> GetEnumValues<TEnum>() where TEnum : struct, Enum
            => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    } 
}
