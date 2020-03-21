using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using Noggog;
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
            float? RawFloat { get; set; }
        }

        public static char GetGlobalChar(MajorRecordFrame frame)
        {
            var subrecordSpan = frame.Content;
            var fnamLocation = UtilityTranslation.FindFirstSubrecord(subrecordSpan, frame.Header.Meta, FNAM);
            if (fnamLocation == -1)
            {
                throw new ArgumentException($"Could not find FNAM.");
            }
            var fnamMeta = frame.Header.Meta.SubRecordFrame(subrecordSpan.Slice(fnamLocation));
            if (fnamMeta.Content.Length != 1)
            {
                throw new ArgumentException($"FNAM had non 1 length: {fnamMeta.Content.Length}");
            }
            return (char)fnamMeta.Content[0];
        }

        public static T Create<T>(
            MutagenFrame frame,
            Func<MutagenFrame, char, T> getter)
            where T : IMajorRecordCommon, IGlobalCommon
        {
            var initialPos = frame.Position;
            var majorMeta = frame.GetMajorRecordFrame();
            if (majorMeta.Header.RecordType != GLOB)
            {
                throw new ArgumentException();
            }
            
            T g = getter(frame, GetGlobalChar(majorMeta));

            frame.Reader.Position = initialPos + frame.MetaData.MajorConstants.TypeAndLengthLength;

            // Read data
            var fltvLoc = UtilityTranslation.FindFirstSubrecord(majorMeta.Content, frame.MetaData, FLTV, navigateToContent: true);
            if (fltvLoc == -1)
            {
                throw new ArgumentException($"Could not find FLTV.");
            }
            g.RawFloat = majorMeta.Content.Slice(fltvLoc).GetFloat();

            // Skip to end
            frame.Reader.Position = initialPos + majorMeta.Header.TotalLength;
            return g;
        }

    }
}
