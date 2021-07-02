using System.Collections.Generic;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.Plugins.Implicit
{
    public interface IImplicitRecordFormKeyProvider
    {
        IReadOnlyCollection<FormKey> RecordFormKeys { get; }
    }

    public class ImplicitRecordFormKeyProvider : IImplicitRecordFormKeyProvider
    {
        private readonly GameReleaseInjection _gameRelease;

        public ImplicitRecordFormKeyProvider(
            GameReleaseInjection gameRelease)
        {
            _gameRelease = gameRelease;
        }

        public IReadOnlyCollection<FormKey> RecordFormKeys => Implicits.Get(_gameRelease.Release).RecordFormKeys;
    }
}