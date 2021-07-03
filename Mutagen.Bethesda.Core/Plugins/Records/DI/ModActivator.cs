using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Records.DI
{
    public interface IModActivator
    {
        TMod Activate<TMod>(ModKey modKey)
            where TMod : IModGetter;
    }
    
    public interface IModActivator<TMod>
        where TMod : IModGetter
    {
        TMod Activate(ModKey modKey);
    }

    public class ModActivator : IModActivator
    {
        private readonly IGameReleaseContext _gameRelease;

        public ModActivator(
            IGameReleaseContext gameRelease)
        {
            _gameRelease = gameRelease;
        }

        public TMod Activate<TMod>(ModKey modKey)
            where TMod : IModGetter
        {
            return ModInstantiator<TMod>.Activator(modKey, _gameRelease.Release);
        }
    }

    public class ModActivator<TMod> : IModActivator<TMod>
        where TMod : IModGetter
    {
        private readonly IGameReleaseContext _gameRelease;

        public ModActivator(
            IGameReleaseContext gameRelease)
        {
            _gameRelease = gameRelease;
        }

        public TMod Activate(ModKey modKey)
        {
            return ModInstantiator<TMod>.Activator(modKey, _gameRelease.Release);
        }
    }
}