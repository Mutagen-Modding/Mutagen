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
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

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
                if (item.FormVersion >= 44)
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
                if (item.FormVersion >= 44)
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
                if (this.FormVersion >= 44)
                {
                    return (MaterialObject.Flag)BinaryPrimitives.ReadUInt64LittleEndian(_data.Slice(_FlagsLocation));
                }
                else
                {
                    return (MaterialObject.Flag)BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_FlagsLocation));
                }
            }
        }
    }
}
