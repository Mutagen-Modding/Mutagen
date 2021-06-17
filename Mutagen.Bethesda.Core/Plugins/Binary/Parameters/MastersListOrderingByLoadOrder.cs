using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters
{
    /// <summary>
    /// A class that instructs the export to make masters listings match a given load order.
    /// </summary>
    public class MastersListOrderingByLoadOrder : AMastersListOrderingOption
    {
        private readonly List<ModKey> _modKeys;

        public IReadOnlyList<ModKey> LoadOrder => _modKeys;

        /// <summary>
        /// Whether to throw an exception if an unknown mod during export that is not on the given load order
        /// </summary>
        public bool Strict { get; set; }

        public MastersListOrderingByLoadOrder(IEnumerable<ModKey> modKeys)
        {
            _modKeys = modKeys.ToList();
        }

        public static MastersListOrderingByLoadOrder Factory(IEnumerable<ModKey> modKeys) => new MastersListOrderingByLoadOrder(modKeys);
            
        public static MastersListOrderingByLoadOrder Factory<T>(LoadOrder<T> loadOrder)
            where T : IModKeyed
        {
            return Factory(loadOrder.Select(listing => listing.Key));
        }
    }
}