using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
{
    public interface IImplicitRecordFormKeyProvider
    {
        IReadOnlyCollection<FormKey> Get();
    }

    public class ImplicitRecordFormKeyProvider : IImplicitRecordFormKeyProvider
    {
        private readonly GameReleaseContext _gameRelease;

        public ImplicitRecordFormKeyProvider(
            GameReleaseContext gameRelease)
        {
            _gameRelease = gameRelease;
        }
        
        public IReadOnlyCollection<FormKey> Get()
        {
            return Implicits.Get(_gameRelease.Release).RecordFormKeys;
        }
    }
}