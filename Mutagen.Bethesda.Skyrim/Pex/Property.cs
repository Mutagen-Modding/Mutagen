using Mutagen.Bethesda.Pex.Binary.Translations;
using System;

namespace Mutagen.Bethesda.Skyrim.Pex.Internals
{
    public partial class PropertyPexCreateTranslation
    {
        public static partial void FillBinaryFlagsParseCustom(PexReader parse, IProperty item)
        {
            var flags = (byte)item.Flags;

            if ((flags & 4) != 0)
            {
                item.AutoVarName = parse.ReadString();
            }

            if ((flags & 5) == 1)
            {
                item.ReadHandler = Function.CreateFromBinary(parse);
            }

            if ((flags & 6) == 2)
            {
                item.WriteHandler = Function.CreateFromBinary(parse);
            }
        }
    }

    public partial class PropertyPexWriteTranslation
    {
        public static partial void WriteBinaryFlagsParseCustom(PexWriter writer, IPropertyGetter item)
        {
            var flags = (byte)item.Flags;
            if ((flags & 4) != 0)
            {
                writer.Write(item.AutoVarName);
            }

            if ((flags & 5) == 1)
            {
                item.ReadHandler?.WriteToBinary(writer);
            }

            if ((flags & 6) == 2)
            {
                item.WriteHandler?.WriteToBinary(writer);
            }
        }
    }
}
