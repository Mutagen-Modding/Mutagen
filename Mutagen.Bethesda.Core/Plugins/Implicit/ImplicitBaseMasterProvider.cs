using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
{
    public interface IImplicitBaseMasterProvider
    {
        IReadOnlyList<ModKey> Get();
    }

    public class ImplicitBaseMasterProvider : IImplicitBaseMasterProvider
    {
        private readonly GameReleaseContext _gameRelease;

        public ImplicitBaseMasterProvider(
            GameReleaseContext gameRelease)
        {
            _gameRelease = gameRelease;
        }
        
        public IReadOnlyList<ModKey> Get()
        {
            return Implicits.Get(_gameRelease.Release).BaseMasters;
        }
    }
}