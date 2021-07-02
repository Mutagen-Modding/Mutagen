using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
{
    public interface IImplicitListingModKeyProvider
    {
        IReadOnlyCollection<ModKey> Listings { get; }
    }

    public class ImplicitListingModKeyProvider : IImplicitListingModKeyProvider
    {
        private readonly GameReleaseInjection _gameRelease;

        public ImplicitListingModKeyProvider(
            GameReleaseInjection gameRelease)
        {
            _gameRelease = gameRelease;
        }

        public IReadOnlyCollection<ModKey> Listings => Implicits.Get(_gameRelease.Release).Listings;
    }
}