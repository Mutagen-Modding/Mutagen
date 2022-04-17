using System.Collections.Generic;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Implicit.DI;

public interface IImplicitListingModKeyProvider
{
    IReadOnlyCollection<ModKey> Listings { get; }
}

public class ImplicitListingModKeyProvider : IImplicitListingModKeyProvider
{
    private readonly IGameReleaseContext _gameRelease;

    public ImplicitListingModKeyProvider(
        IGameReleaseContext gameRelease)
    {
        _gameRelease = gameRelease;
    }

    public IReadOnlyCollection<ModKey> Listings => Implicits.Get(_gameRelease.Release).Listings;
}