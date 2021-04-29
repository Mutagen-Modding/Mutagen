using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PcLevelMultBinaryCreateTranslation
        {
            public static partial void FillBinaryLevelMultCustom(MutagenFrame frame, IPcLevelMult item)
            {
                throw new NotImplementedException();
            }
        }

        public partial class PcLevelMultBinaryWriteTranslation
        {
            public static partial void WriteBinaryLevelMultCustom(MutagenWriter writer, IPcLevelMultGetter item)
            {
                throw new NotImplementedException();
            }
        }

        public partial class PcLevelMultBinaryOverlay
        {
            float GetLevelMultCustom(int location) => throw new NotImplementedException();
        }
    }
}
