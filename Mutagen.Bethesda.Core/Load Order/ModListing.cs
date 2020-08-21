using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Class associating a ModKey with a Mod object that may or may not exist.
    /// </summary>
    [DebuggerDisplay("ModListing {ToString()}")]
    public class ModListing<TMod> : IModListing<TMod>
        where TMod : class, IModGetter
    {
        /// <summary>
        /// Mod object
        /// </summary>
        public TMod? Mod { get; private set; }

        /// <summary>
        /// ModKey associated with listing
        /// </summary>
        public ModKey ModKey { get; private set; }

        private ModListing(ModKey key, TMod? mod)
        {
            this.ModKey = key;
            this.Mod = mod;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ModListing(TMod mod)
        {
            this.ModKey = mod.ModKey;
            this.Mod = mod;
        }

        /// <summary>
        /// Factory to create a ModListing which does not have a mod object
        /// </summary>
        /// <param name="key">ModKey to associate with listing</param>
        /// <returns>ModListing with no mod object</returns>
        public static ModListing<TMod> UnloadedModListing(ModKey key)
        {
            return new ModListing<TMod>(key, default);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{ModKey} => {(Mod == null ? "Missing" : "Present")}";
        }

        public static implicit operator ModListing<TMod>(TMod mod)
        {
            return new ModListing<TMod>(mod);
        }
    }

    public interface IModListing<out TMod> : IModKeyed
        where TMod : class, IModGetter
    {
        /// <summary>
        /// Mod object
        /// </summary>
        TMod? Mod { get; }
    }
}
