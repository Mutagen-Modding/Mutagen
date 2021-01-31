using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Fallout4.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    public partial interface IGlobalGetter
    {
        char TypeChar { get; set; }
    }

    public partial class Global : GlobalCustomParsing.IGlobalCommon
    {
        protected static readonly RecordType FNAM = new RecordType("FNAM");

        public abstract float? RawFloat { get; set; }
        public abstract char TypeChar { get; set; }

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
                            var ret = GlobalFloat.CreateFromBinary(f);
                            ret.TypeChar = 'f';
                            return ret;
                        case null:
                            var ret2 = GlobalFloat.CreateFromBinary(f);
                            ret2.TypeChar = '0';
                            return ret2;
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
                IGlobalGetter item)
            {
                if (item.TypeChar != '0')
                    Mutagen.Bethesda.Binary.CharBinaryTranslation.Instance.Write(
                        writer,
                        item.TypeChar,
                        header: RecordTypes.FNAM);
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
                        var ret = GlobalFloatBinaryOverlay.GlobalFloatFactory(
                            stream,
                            package);
                        ret.TypeChar = 'f';
                        return ret;
                    case null:
                        var ret2 = GlobalFloatBinaryOverlay.GlobalFloatFactory(
                            stream,
                            package);
                        ret2.TypeChar = '0';
                        return ret2;
                    default:
                        throw new ArgumentException($"Unknown trigger char: {globalChar}");
                }
            }
        }
    }
}
