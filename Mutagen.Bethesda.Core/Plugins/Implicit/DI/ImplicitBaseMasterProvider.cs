using System.Collections.Generic;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Implicit.DI
{
    public interface IImplicitBaseMasterProvider
    {
        IReadOnlyList<ModKey> BaseMasters { get; }
    }

    public class ImplicitBaseMasterProvider : IImplicitBaseMasterProvider
    {
        private readonly GameReleaseInjection _gameRelease;

        public ImplicitBaseMasterProvider(
            GameReleaseInjection gameRelease)
        {
            _gameRelease = gameRelease;
        }
        
        public IReadOnlyList<ModKey> BaseMasters => Implicits.Get(_gameRelease.Release).BaseMasters;
    }
}