using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PcLevelMultBinaryCreateTranslation
        {
            static partial void FillBinaryLevelMultCustom(MutagenFrame frame, IPcLevelMult item)
            {
                throw new NotImplementedException();
            }
        }

        public partial class PcLevelMultBinaryWriteTranslation
        {
            static partial void WriteBinaryLevelMultCustom(MutagenWriter writer, IPcLevelMultGetter item)
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
