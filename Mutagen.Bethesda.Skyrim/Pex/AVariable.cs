using System;
using Mutagen.Bethesda.Pex.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim.Pex
{
    public partial class AVariable
    {
        public abstract VariableType VariableType { get; }

        public static bool TryCreateFromBinary(PexReader reader, out AVariable item)
        {
            item = CreateFromBinary(reader);
            return true;
        }

        public static AVariable CreateFromBinary(PexReader reader)
        {
            switch ((VariableType)reader.ReadUInt8())
            {
                case VariableType.Identifier:
                    return IdentifierVariable.CreateFromBinary(reader);
                case VariableType.String:
                    return StringVariable.CreateFromBinary(reader);
                case VariableType.Integer:
                    return IntVariable.CreateFromBinary(reader);
                case VariableType.Float:
                    return FloatVariable.CreateFromBinary(reader);
                case VariableType.Bool:
                    return BoolVariable.CreateFromBinary(reader);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public partial interface IAVariableGetter
    {
        VariableType VariableType { get; }
    }

    namespace Internals
    {
        public partial class AVariablePexCreateTranslation
        {
            public static partial void FillBinaryTypeTranslationCustom(PexReader reader, IAVariable item)
            {
            }
        }

        public partial class AVariablePexWriteTranslation
        {
            public static partial void WriteBinaryTypeTranslationCustom(PexWriter writer, IAVariableGetter item)
            {
                writer.Write((byte)item.VariableType);
            }
        }
    }

    public partial class StringVariable
    {
        public override VariableType VariableType => VariableType.String;
    }

    public partial class IdentifierVariable
    {
        public override VariableType VariableType => VariableType.Identifier;
    }

    public partial class IntVariable
    {
        public override VariableType VariableType => VariableType.Integer;
    }

    public partial class FloatVariable
    {
        public override VariableType VariableType => VariableType.Float;
    }

    public partial class BoolVariable
    {
        public override VariableType VariableType => VariableType.Bool;
    }
}
