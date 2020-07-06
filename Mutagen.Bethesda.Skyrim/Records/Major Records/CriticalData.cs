using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CriticalData
    {
        [Flags]
        public enum Flag
        {
            OnDeath = 0x01
        }
    }

    namespace Internals
    {
        public partial class CriticalDataBinaryOverlay
        {
            public Int32 GetUnused3Custom(int location)
            {
                if (_package.MajorRecord!.FormVersion!.Value < 44) return default;
                return BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(0xC));
            }

            public IFormLink<ISpellGetter> GetEffectCustom(int location)
            {
                location = _package.MajorRecord!.FormVersion!.Value < 44 ? 0xC : 0x10;
                return new FormLink<ISpellGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(location, 0x4))));
            }

            public Int32 GetUnused4Custom(int location)
            {
                if (_package.MajorRecord!.FormVersion!.Value < 44) return default;
                return BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(0x14));
            }
        }
    }
}
