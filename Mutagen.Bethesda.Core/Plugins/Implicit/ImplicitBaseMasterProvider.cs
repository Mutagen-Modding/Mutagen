using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
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