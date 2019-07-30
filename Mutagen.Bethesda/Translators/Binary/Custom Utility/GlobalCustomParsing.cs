using Loqui.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public static class GlobalCustomParsing
    {
        public static readonly RecordType GLOB = new RecordType("GLOB");
        public static readonly RecordType FNAM = new RecordType("FNAM");
        public static readonly RecordType FLTV = new RecordType("FLTV");
        
        public interface IGlobalCommon
        {
            float RawFloat { get; set; }
        }

        public static char GetGlobalChar(MajorRecordFrame frame)
        {
            var subrecordSpan = frame.ContentSpan;
            var fnamLocation = UtilityTranslation.FindFirstSubrecord(subrecordSpan, frame.Header.Meta, FNAM);
            if (fnamLocation == -1)
            {
                throw new ArgumentException($"Could not find FNAM.");
            }
            var fnamMeta = frame.Header.Meta.SubRecordFrame(subrecordSpan.Slice(fnamLocation));
            if (fnamMeta.ContentSpan.Length != 1)
            {
                throw new ArgumentException($"FNAM had non 1 length: {fnamMeta.ContentSpan.Length}");
            }
            return (char)fnamMeta.ContentSpan[0];
        }

        public static T Create<T>(
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            Func<char, T> getter)
            where T : MajorRecord, IGlobalCommon
        {
            var initialPos = frame.Position;
            var majorMeta = frame.MetaData.ReadMajorRecordFrame(frame);
            if (majorMeta.Header.RecordType != GLOB)
            {
                throw new ArgumentException();
            }
            
            T g = getter(GetGlobalChar(majorMeta));

            // Fill with major record fields
            frame.Reader.Position = initialPos + frame.MetaData.MajorConstants.TypeAndLengthLength;
            MajorRecord.FillBinary(
                frame,
                g,
                masterReferences,
                errorMask);

            // Read data
            var fltvLoc = UtilityTranslation.FindFirstSubrecord(majorMeta.ContentSpan, frame.MetaData, FLTV, navigateToContent: true);
            if (fltvLoc == -1)
            {
                throw new ArgumentException($"Could not find FLTV.");
            }
            g.RawFloat = majorMeta.ContentSpan.Slice(fltvLoc).GetFloat();

            // Skip to end
            frame.Reader.Position = initialPos + majorMeta.Header.TotalLength;
            return g;
        }

    }
}
