using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class Utility
    {
        public static T GetGetterInterfaceReference<T>(object rhs)
            where T : class
        {
            if (rhs is T ret) return ret;
            if (rhs == null) return default;
            throw new ArgumentException($"An IGetter was directed to copy as reference but could not be cast to {typeof(T).Name}");
        }
    }
}
