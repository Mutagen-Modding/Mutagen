using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class AddonNodeBinaryCreateTranslation
        {
            public static partial void FillBinaryAlwaysLoadedCustom(MutagenFrame frame, IAddonNodeInternal item)
            {
                var flags = frame.ReadUInt16();
                item.AlwaysLoaded = flags switch 
                {
                    1 => false,
                    3 => true,
                    _ => throw new NotImplementedException()
                };
            }
        }

        public partial class AddonNodeBinaryWriteTranslation
        {
            public static partial void WriteBinaryAlwaysLoadedCustom(MutagenWriter writer, IAddonNodeGetter item)
            {
                if (item.AlwaysLoaded)
                {
                    writer.Write((short)3);
                }
                else
                {
                    writer.Write((short)1);
                }
            }
        }

        public partial class AddonNodeBinaryOverlay
        {
            public Boolean GetAlwaysLoadedCustom() => BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_AlwaysLoadedLocation)) switch
            {
                1 => false,
                3 => true,
                _ => throw new NotImplementedException()
            };
        }
    }
}
