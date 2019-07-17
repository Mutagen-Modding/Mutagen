using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial interface IGlobalInternalGetter
    {
        char TypeChar { get; }
    }

    public partial class Global
    {
        public static readonly RecordType FNAM = new RecordType("FNAM");

        public abstract float RawFloat { get; set; }
        public abstract char TypeChar { get; }

        public static Global CreateFromBinary(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            // Skip to FNAM
            var initialPos = frame.Position;
            if (HeaderTranslation.ReadNextRecordType(frame.Reader, out var recLen) != Global_Registration.GLOB_HEADER)
            {
                throw new ArgumentException();
            }
            frame.CheckUpcomingRead(18);
            frame.Reader.Position += 16;
            var edidLength = frame.Reader.ReadInt16();
            frame.Reader.Position += edidLength;

            // Confirm FNAM
            var type = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len);
            if (!type.Equals(FNAM))
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Could not find FNAM in its expected location: {frame.Position}"));
                return null;
            }
            if (len != 1)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"FNAM had non 1 length: {len}"));
            }

            // Create proper Global subclass
            var triggerChar = (char)frame.Reader.ReadUInt8();
            Global g;
            switch (triggerChar)
            {
                case GlobalInt.TRIGGER_CHAR:
                    g = GlobalInt.Factory();
                    break;
                case GlobalShort.TRIGGER_CHAR:
                    g = GlobalShort.Factory();
                    break;
                case GlobalFloat.TRIGGER_CHAR:
                    g = GlobalFloat.Factory();
                    break;
                default:
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"Unknown trigger char: {triggerChar}"));
                    return null;
            }

            // Fill with major record fields
            frame.Reader.Position = initialPos + 8;
            OblivionMajorRecord.FillBinary(
                frame,
                g,
                masterReferences,
                errorMask);

            // Skip to and read data
            frame.Reader.Position += 13;
            if (Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Parse(
                frame,
                out var rawFloat))
            {
                g.RawFloat = rawFloat;
            }
            return g;
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
                // Skip to FNAM
                var majorMeta = package.Meta.MajorRecord(stream.RemainingSpan);
                if (majorMeta.RecordType != Global_Registration.GLOB_HEADER)
                {
                    throw new ArgumentException();
                }
                int pos = majorMeta.HeaderLength;
                var edidMeta = package.Meta.SubRecord(stream.RemainingSpan.Slice(pos));
                if (edidMeta.RecordType != Mutagen.Bethesda.Constants.EditorID)
                {
                    throw new ArgumentException();
                }
                pos += edidMeta.TotalLength;

                // Confirm FNAM
                var fnamMeta = package.Meta.SubRecord(stream.RemainingSpan.Slice(pos));
                if (!fnamMeta.RecordType.Equals(Global.FNAM))
                {
                    throw new ArgumentException($"Could not find FNAM.");
                }
                if (fnamMeta.RecordLength != 1)
                {
                    throw new ArgumentException($"FNAM had non 1 length: {fnamMeta.RecordLength}");
                }
                pos += fnamMeta.HeaderLength;

                // Create proper Global subclass
                var triggerChar = (char)stream.RemainingSpan[pos];
                Global g;
                switch (triggerChar)
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
                        throw new ArgumentException($"Unknown trigger char: {triggerChar}");
                }
            }
        }
    }
}
