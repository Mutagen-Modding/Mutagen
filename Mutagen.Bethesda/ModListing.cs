using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ModListing<M>
        where M : IMod<M>
    {
        public M Mod { get; private set; }
        public ModKey Key { get; private set; }
        public bool Loaded => Mod != null;

        public ModListing(ModKey key, M mod)
        {
            this.Key = key;
            this.Mod = mod;
        }

        public ModListing(ModKey key)
        {
            this.Key = key;
        }
    }
}
