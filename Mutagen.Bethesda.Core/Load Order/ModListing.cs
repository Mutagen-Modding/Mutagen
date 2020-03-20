using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ModListing<TMod>
        where TMod : class, IModGetter
    {
        public TMod? Mod { get; private set; }
        public ModKey Key { get; private set; }

        private ModListing(ModKey key, TMod? mod)
        {
            this.Key = key;
            this.Mod = mod;
        }

        public ModListing(TMod mod)
        {
            this.Key = mod.ModKey;
            this.Mod = mod;
        }

        public static ModListing<TMod> UnloadedModListing(ModKey key)
        {
            return new ModListing<TMod>(key, default);
        }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
