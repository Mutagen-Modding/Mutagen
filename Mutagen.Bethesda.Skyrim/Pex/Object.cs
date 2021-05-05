using System.IO;
using Mutagen.Bethesda.Pex.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim.Pex
{
    namespace Internals
    {
        public partial class ObjectPexCreateTranslation
        {
            public static partial void FillBinarySizeMarkerCustom(PexReader parse, IObject item)
            {
                /*
                 * This is the size of the entire object in bytes not some count variable for a loop. This also includes
                 * the size of itself thus the - sizeof(uint)
                 */
                var size = parse.ReadUInt32() - sizeof(uint);
                parse.ExpectedPos = parse.Position + size;
            }

            public static partial void FillBinarySizeConfirmationCustom(PexReader parse, IObject item)
            {
                var newPos = parse.Position;
                if (newPos != parse.ExpectedPos)
                    throw new InvalidDataException("Current position in Stream does not match expected position: " +
                                                  $"Current: {newPos} Expected: {parse.ExpectedPos}");
            }
        }

        public partial class ObjectPexWriteTranslation
        {
            public static partial void WriteBinarySizeMarkerCustom(PexWriter writer, IObjectGetter item)
            {
                //needed for later changing
                writer.SizePosition = writer.Writer.BaseStream.Position;
                writer.Write(sizeof(uint));
            }

            public static partial void WriteBinarySizeConfirmationCustom(PexWriter writer, IObjectGetter item)
            {
                //calculate object size, go back, change it and return to the current position
                var newPos = writer.Writer.BaseStream.Position;
                writer.Writer.BaseStream.Position = writer.SizePosition;

                var objectSize = newPos - writer.SizePosition;
                writer.Writer.Write((uint)objectSize);

                writer.Writer.BaseStream.Position = newPos;
            }
        }
    }
}
