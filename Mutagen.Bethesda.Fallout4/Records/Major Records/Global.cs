using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial interface IGlobalGetter
    {
        char TypeChar { get; }
    }

    public partial class Global : GlobalCustomParsing.IGlobalCommon
    {
        protected static readonly RecordType FNAM = new RecordType("FNAM");

        public abstract float? RawFloat { get; set; }
        public abstract char TypeChar { get; }

        [Flags]
        public enum MajorFlag
        {
            Constant = 0x0000_0040
        }

        public static Global CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter)
        {
            return GlobalCustomParsing.Create<Global>(
                frame,
                getter: (f, triggerChar) =>
                {
                    switch (triggerChar)
                    {
                        case GlobalInt.TRIGGER_CHAR:
                            return GlobalInt.CreateFromBinary(f);
                        case GlobalShort.TRIGGER_CHAR:
                            return GlobalShort.CreateFromBinary(f);
                        case GlobalFloat.TRIGGER_CHAR:
                            return GlobalFloat.CreateFromBinary(f);
                        case null:
                            var ret = GlobalFloat.CreateFromBinary(f);
                            ret.NoTypeDeclaration = true;
                            return ret;
                        default:
                            throw new ArgumentException($"Unknown trigger char: {triggerChar}");
                    }
                });
        }
    }

    namespace Internals
    {
        public partial class GlobalBinaryCreateTranslation
        {
            public static partial void FillBinaryTypeCharCustom(MutagenFrame frame, IGlobalInternal item)
            {
            }
        }

        public partial class GlobalBinaryWriteTranslation
        {
            public static partial void WriteBinaryTypeCharCustom(
                MutagenWriter writer,
                IGlobalGetter item)
            {
                if (item is not IGlobalFloatGetter f
                    || !f.NoTypeDeclaration)
                {
                    CharBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                        writer,
                        item.TypeChar,
                        header: RecordTypes.FNAM);
                }
            }
        }

        public abstract partial class GlobalBinaryOverlay
        {
            public abstract float? RawFloat { get; }
            public virtual char TypeChar { get; set; }

            public static GlobalBinaryOverlay GlobalFactory(
                OverlayStream stream,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var majorFrame = package.MetaData.Constants.MajorRecordFrame(stream.RemainingMemory);
                var globalChar = GlobalCustomParsing.GetGlobalChar(majorFrame);
                switch (globalChar)
                {
                    case GlobalInt.TRIGGER_CHAR:
                        return GlobalIntBinaryOverlay.GlobalIntFactory(
                            stream,
                            package);
                    case GlobalShort.TRIGGER_CHAR:
                        return GlobalShortBinaryOverlay.GlobalShortFactory(
                            stream,
                            package);
                    case GlobalFloat.TRIGGER_CHAR:
                        return GlobalFloatBinaryOverlay.GlobalFloatFactory(
                            stream,
                            package);
                    case null:
                        var ret = GlobalFloatBinaryOverlay.GlobalFloatFactory(
                            stream,
                            package);
                        ret.NoTypeDeclaration = true;
                        return ret;
                    default:
                        throw new ArgumentException($"Unknown trigger char: {globalChar}");
                }
            }
        }
    }
}
