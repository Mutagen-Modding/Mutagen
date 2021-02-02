using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public abstract class LinkCachePreferences
    {
        public static LinkCachePreferences WholeRecord => LinkCachePreferenceWholeRecord.Instance;

        public static LinkCachePreferences Default => WholeRecord;
    }
}
