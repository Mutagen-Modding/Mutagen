using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MaterialObject
    {
        [Flags]
        public enum Flag : ulong
        {
            SinglePass = 0x01,

            /// <summary>
            /// SSE only
            /// </summary>
            Snow = 0x1_0000_0000,
        }
    }

    namespace Internals
    {
        public partial class MaterialObjectBinaryCreateTranslation
        {
            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IMaterialObjectInternal item)
            {
                if (item.FormVersion >= 43)
                {
                    item.Flags = (MaterialObject.Flag)frame.ReadUInt64();
                }
                else
                {
                    item.Flags = (MaterialObject.Flag)frame.ReadUInt32();
                }
            }
        }

        public partial class MaterialObjectBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IMaterialObjectGetter item)
            {
                if (item.FormVersion >= 43)
                {
                    writer.Write((ulong)item.Flags);
                }
                else
                {
                    writer.Write((uint)item.Flags);
                }
            }
        }

        public partial class MaterialObjectBinaryOverlay
        {
            public MaterialObject.Flag GetFlagsCustom()
            {
                if (_data.Length < (_FlagsLocation + 4)) return default;
                var slice = _data.Slice(_FlagsLocation);
                if (this.FormVersion >= 43)
                {
                    return (MaterialObject.Flag)BinaryPrimitives.ReadUInt64LittleEndian(slice);
                }
                else
                {
                    return (MaterialObject.Flag)BinaryPrimitives.ReadUInt32LittleEndian(slice);
                }
            }
        }
    }
}
