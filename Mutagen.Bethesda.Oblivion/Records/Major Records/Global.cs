using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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
        public static readonly RecordType FLTV = new RecordType("FLTV");

        public abstract float RawFloat { get; set; }
        public abstract char TypeChar { get; }

        public static Global CreateFromBinary(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            var initialPos = frame.Position;
            var majorMeta = frame.MetaData.ReadMajorRecord(frame);
            if (majorMeta.RecordType != Global_Registration.GLOB_HEADER)
            {
                throw new ArgumentException();
            }

            var subrecordSpan = frame.GetSpan(checked((int)majorMeta.RecordLength));

            // Find FNAM
            var locs = UtilityTranslation.FindFirstSubrecords(subrecordSpan, frame.MetaData, FNAM, FLTV);
            if (locs[0] < 0)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Could not find FNAM."));
                return null;
            }
            if (locs[1] < 0)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Could not find FLTV."));
                return null;
            }
            var fnamFrame = frame.MetaData.SubRecordFrame(subrecordSpan.Slice(locs[0]));
            if (fnamFrame.DataSpan.Length != 1)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"FNAM had non 1 length: {fnamFrame.DataSpan.Length}"));
            }

            // Create proper Global subclass
            var triggerChar = (char)fnamFrame.DataSpan[0];
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
            frame.Reader.Position = initialPos + frame.MetaData.MajorConstants.TypeAndLengthLength;
            OblivionMajorRecord.FillBinary(
                frame,
                g,
                masterReferences,
                errorMask);

            // Read data
            var fltvFrame = frame.MetaData.SubRecordFrame(subrecordSpan.Slice(locs[1]));
            g.RawFloat = fltvFrame.DataSpan.GetFloat();
            
            // Skip to end
            frame.Reader.Position = initialPos + majorMeta.TotalLength;
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
