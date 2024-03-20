using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IHasEnabledMarkersProvider
{
    bool HasEnabledMarkers { get; }
}

public sealed class HasEnabledMarkersProvider : IHasEnabledMarkersProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;

    public bool HasEnabledMarkers => GameConstants.Get(_gameReleaseContext.Release).HasEnabledMarkers;

    public HasEnabledMarkersProvider(IGameReleaseContext gameReleaseContext)
    {
        _gameReleaseContext = gameReleaseContext;
    }
}

public sealed record HasEnabledMarkersInjection(bool HasEnabledMarkers) : IHasEnabledMarkersProvider;