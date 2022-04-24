using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Static class with some common utility functions for parsing Global records
/// </summary>
internal static class GlobalCustomParsing
{
    public static readonly RecordType GLOB = new("GLOB");
    public static readonly RecordType FNAM = new("FNAM");
    public static readonly RecordType FLTV = new("FLTV");

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
    public static char? GetGlobalChar(MajorRecordFrame frame)
    {
        var subrecordSpan = frame.Content;
        var fnamMeta = RecordSpanExtensions.FindFirstSubrecord(subrecordSpan, frame.Meta, FNAM);
        if (fnamMeta == null)
        {
            return null;
        }
        if (fnamMeta.Value.Content.Length != 1)
        {
            throw new ArgumentException($"FNAM had non 1 length: {fnamMeta.Value.Content.Length}");
        }
        return (char)fnamMeta.Value.Content[0];
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
        Func<MutagenFrame, char?, T> getter)
        where T : IMajorRecord, IGlobalCommon
    {
        var initialPos = frame.Position;
        var majorMeta = frame.GetMajorRecordFrame();
        if (majorMeta.RecordType != GLOB)
        {
            throw new ArgumentException();
        }
        
        T g = getter(frame, GetGlobalChar(majorMeta));

        frame.Reader.Position = initialPos + frame.MetaData.Constants.MajorConstants.TypeAndLengthLength;

        // Read data
        var fltvLoc = RecordSpanExtensions.FindFirstSubrecord(majorMeta.Content, frame.MetaData.Constants, FLTV);
        if (fltvLoc == null)
        {
            throw new ArgumentException($"Could not find FLTV.");
        }
        g.RawFloat = fltvLoc.Value.AsFloat();

        // Skip to end
        frame.Reader.Position = initialPos + majorMeta.TotalLength;
        return g;
    }

}
