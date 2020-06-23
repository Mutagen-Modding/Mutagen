using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageDataTopicBinaryOverlay
        {
            public IReadOnlyList<IATopicReferenceGetter> Topics => throw new NotImplementedException();

            public ReadOnlyMemorySlice<byte>? TPIC => throw new NotImplementedException();
        }
    }
}
