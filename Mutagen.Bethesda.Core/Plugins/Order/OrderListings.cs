using System;
using System.Collections.Generic;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IOrderListings
    {
        IEnumerable<T> Order<T>(IEnumerable<T> e, Func<T, ModKey> selector);

        IEnumerable<T> Order<T>(
            IEnumerable<T> implicitListings,
            IEnumerable<T> pluginsListings,
            IEnumerable<T> creationClubListings,
            Func<T, ModKey> selector);
    }

    public class OrderListings : IOrderListings
    {
        /// <inheritdoc />
        public IEnumerable<T> Order<T>(IEnumerable<T> e, Func<T, ModKey> selector)
        {
            return e.OrderBy(e => selector(e).Type);
        }

        /// <inheritdoc />
        public IEnumerable<T> Order<T>(
            IEnumerable<T> implicitListings,
            IEnumerable<T> pluginsListings,
            IEnumerable<T> creationClubListings,
            Func<T, ModKey> selector)
        {
            var plugins = pluginsListings
                .Select(selector)
                .ToList();
            return implicitListings
                .Concat(
                    Order(creationClubListings
                        .Select(x =>
                        {
                            if (selector(x).Type == ModType.Plugin)
                            {
                                throw new NotImplementedException("Creation Club does not support esp plugins.");
                            }
                            return x;
                        })
                        // If CC mod is on plugins list, refer to its ordering
                        .OrderBy(selector, Comparer<ModKey>.Create((x, y) =>
                        {
                            var xIndex = plugins.IndexOf(x);
                            var yIndex = plugins.IndexOf(y);
                            if (xIndex == yIndex) return 0;
                            return xIndex - yIndex;
                        })), selector))
                .Concat(pluginsListings)
                .Distinct(selector);
        }
    }
}