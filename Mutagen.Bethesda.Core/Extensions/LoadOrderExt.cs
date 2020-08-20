using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class LoadOrderExt
    {
        public static IEnumerable<ModKey> OnlyEnabled(IEnumerable<(bool Enabled, ModKey ModKey)> loadOrder)
        {
            return loadOrder.Where(x => x.Enabled)
                .Select(x => x.ModKey);
        }
    }
}
