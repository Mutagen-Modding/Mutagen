using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public partial interface IGlobalInternalGetter
    {
        char TypeChar { get; }
    }

    public partial class Global : GlobalCustomParsing.IGlobalCommon
    {
        protected static readonly RecordType FNAM = new RecordType("FNAM");

        public abstract float RawFloat { get; set; }
        public abstract char TypeChar { get; }

        public static Global CreateFromBinary(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            return GlobalCustomParsing.Create<Global>(
                frame,
                masterReferences,
                errorMask,
                getter: (f, m, triggerChar) =>
                {
                    switch (triggerChar)
                    {
                        case GlobalInt.TRIGGER_CHAR:
                            return GlobalInt.CreateFromBinary(f, m);
                        case GlobalShort.TRIGGER_CHAR:
                            return GlobalShort.CreateFromBinary(f, m);
                        case GlobalFloat.TRIGGER_CHAR:
                            return GlobalFloat.CreateFromBinary(f, m);
                        default:
                            throw new ArgumentException($"Unknown trigger char: {triggerChar}");
                    }
                });
        }
    }

    namespace Internals
    {
        public partial class GlobalBinaryWriteTranslation
        {
            static partial void WriteBinaryTypeCharCustom(
                MutagenWriter writer,
                IGlobalInternalGetter item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                Mutagen.Bethesda.Binary.CharBinaryTranslation.Instance.Write(
                    writer,
                    item.TypeChar,
                    header: Global_Registration.FNAM_HEADER,
                    nullable: false);
            }
        }

        public abstract partial class GlobalBinaryWrapper
        {
            public abstract float RawFloat { get; }
            public abstract char TypeChar { get; }

            public static GlobalBinaryWrapper GlobalFactory(
                BinaryMemoryReadStream stream,
                BinaryWrapperFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var majorFrame = package.Meta.MajorRecordFrame(stream.RemainingSpan);
                var globalChar = GlobalCustomParsing.GetGlobalChar(majorFrame);
                switch (globalChar)
                {
                    case GlobalInt.TRIGGER_CHAR:
                        return GlobalIntBinaryWrapper.GlobalIntFactory(
                            stream,
                            package);
                    case GlobalShort.TRIGGER_CHAR:
                        return GlobalShortBinaryWrapper.GlobalShortFactory(
                            stream,
                            package);
                    case GlobalFloat.TRIGGER_CHAR:
                        return GlobalFloatBinaryWrapper.GlobalFloatFactory(
                            stream,
                            package);
                    default:
                        throw new ArgumentException($"Unknown trigger char: {globalChar}");
                }
            }
        }
    }
}
