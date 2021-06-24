using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
{
    public interface IImplicitListingModKeyProvider
    {
        IReadOnlyCollection<ModKey> Get();
    }

    public class ImplicitListingModKeyProvider : IImplicitListingModKeyProvider
    {
        private readonly GameReleaseContext _gameRelease;

        public ImplicitListingModKeyProvider(
            GameReleaseContext gameRelease)
        {
            _gameRelease = gameRelease;
        }
        
        public IReadOnlyCollection<ModKey> Get()
        {
            return Implicits.Get(_gameRelease.Release).Listings;
        }
    }
}