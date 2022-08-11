#if false

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rano
{
    public static class EnumExtensions
    {
        public static int Length(this Enum e)
        {
            return Enum.GetNames(e.GetType()).Length;
        }
    }
}

#endif