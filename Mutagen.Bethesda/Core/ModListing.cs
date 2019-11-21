using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ModListing<TMod>
        where TMod : IModGetter
    {
        public TMod Mod { get; private set; }
        public ModKey Key { get; private set; }
        public bool Loaded => Mod != null;

        public ModListing(ModKey key, TMod mod)
        {
            this.Key = key;
            this.Mod = mod;
        }

        public ModListing(ModKey key)
        {
            this.Key = key;
        }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
