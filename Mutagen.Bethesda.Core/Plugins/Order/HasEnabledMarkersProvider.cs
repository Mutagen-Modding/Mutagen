using System;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IHasEnabledMarkersProvider
    {
        bool HasEnabledMarkers { get; }
    }

    public class HasEnabledMarkersProvider : IHasEnabledMarkersProvider
    {
        private readonly IGameReleaseContext _gameReleaseContext;

        public bool HasEnabledMarkers
        {
            get
            {
                return _gameReleaseContext.Release switch
                {
                    GameRelease.Fallout4 => true,
                    GameRelease.SkyrimSE => true,
                    GameRelease.EnderalSE => true,
                    GameRelease.SkyrimVR => true,
                    GameRelease.SkyrimLE => false,
                    GameRelease.EnderalLE => false,
                    GameRelease.Oblivion => false,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public HasEnabledMarkersProvider(IGameReleaseContext gameReleaseContext)
        {
            _gameReleaseContext = gameReleaseContext;
        }
    }

    public record HasEnabledMarkersInjector(bool HasEnabledMarkers) : IHasEnabledMarkersProvider;
}