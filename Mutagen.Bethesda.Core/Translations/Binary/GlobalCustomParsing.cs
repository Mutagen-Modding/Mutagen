using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Static class with some common utility functions for parsing Global records
    /// </summary>
    public static class GlobalCustomParsing
    {
        public static readonly RecordType GLOB = new RecordType("GLOB");
        public static readonly RecordType FNAM = new RecordType("FNAM");
        public static readonly RecordType FLTV = new RecordType("FLTV");

        /// <summary>
        /// An interface for Global records
        /// </summary>
        public interface IGlobalCommon
        {
            float? RawFloat { get; set; }
        }

        /// <summary>
        /// Retrieves the character representing a Global's data type from a MajorRecordFrame
        /// </summary>
        /// <param name="frame">Frame to retrieve from</param>
        /// <returns>Character representing data type</returns>
        /// <exception cref="ArgumentException">If FNAM not present or malformed</exception>
        public static char GetGlobalChar(MajorRecordFrame frame)
        {
            var subrecordSpan = frame.Content;
            var fnamLocation = UtilityTranslation.FindFirstSubrecord(subrecordSpan, frame.Header.Meta, FNAM);
            if (fnamLocation == null)
            {
                throw new ArgumentException($"Could not find FNAM.");
            }
            var fnamMeta = frame.Header.Meta.SubrecordFrame(subrecordSpan.Slice(fnamLocation.Value));
            if (fnamMeta.Content.Length != 1)
            {
                throw new ArgumentException($"FNAM had non 1 length: {fnamMeta.Content.Length}");
            }
            return (char)fnamMeta.Content[0];
        }

        /// <summary>
        /// Global factory helper
        /// </summary>
        /// <param name="frame">Frame to read from</param>
        /// <param name="getter">Func factory to create Global given data type char</param>
        /// <returns>Constructed Global from getter</returns>
        /// <exception cref="ArgumentException">If frame aligned to a malformed Global record</exception>
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
            if (fltvLoc == null)
            {
                throw new ArgumentException($"Could not find FLTV.");
            }
            g.RawFloat = majorMeta.Content.Slice(fltvLoc.Value).GetFloat();

            // Skip to end
            frame.Reader.Position = initialPos + majorMeta.Header.TotalLength;
            return g;
        }

    }
}
