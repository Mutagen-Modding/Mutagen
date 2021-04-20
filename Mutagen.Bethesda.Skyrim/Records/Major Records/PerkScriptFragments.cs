using Mutagen.Bethesda.Records.Binary.Overlay;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PerkScriptFragmentsBinaryOverlay
        {
            public IReadOnlyList<IIndexedScriptFragmentGetter> Fragments { get; private set; } = null!;

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position = FileNameEndingPos;
                Fragments = BinaryOverlayList.FactoryByCount<IIndexedScriptFragmentGetter>(
                    stream,
                    _package,
                    stream.ReadUInt16(),
                    (s, p) => IndexedScriptFragmentBinaryOverlay.IndexedScriptFragmentFactory(s, p));
            }
        }
    }
}
