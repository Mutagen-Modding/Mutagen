namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// Tracks custom mappings of one record type to another.
/// </summary>
public class RecordTypeConverter
{
    /// <summary>
    /// Tracks standard record types to their custom record type counterparts
    /// </summary>
    public Dictionary<RecordType, RecordType> FromConversions = new Dictionary<RecordType, RecordType>();
        
    /// <summary>
    /// Tracks custom record types to their standard record type counterparts
    /// </summary>
    public Dictionary<RecordType, RecordType> ToConversions = new Dictionary<RecordType, RecordType>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="conversions">Array of mappings, with standard as the key, and custom as the value</param>
    public RecordTypeConverter(params KeyValuePair<RecordType, RecordType>[] conversions)
    {
        foreach (var conv in conversions)
        {
            FromConversions[conv.Key] = conv.Value;
            ToConversions[conv.Value] = conv.Key;
        }
    }
}

public static class RecordTypeConverterExt
{
    /// <summary>
    /// Extension method to retrieve the record type to use in a custom context.
    /// If the converter is null, or a custom alternative is not registered, the input record type is returned.
    /// </summary>
    /// <param name="converter">Optional record type mapping</param>
    /// <param name="rec">Standard RecordType to query</param>
    /// <returns>Custom RecordType if one is registered in converter.  Otherwise the input RecordType.</returns>
    public static RecordType ConvertToCustom(this RecordTypeConverter? converter, RecordType rec)
    {
        if (converter == null) return rec;
        if (converter.FromConversions.TryGetValue(rec, out var converted))
        {
            rec = converted;
        }
        else if (converter.ToConversions.ContainsKey(rec))
        {
            return RecordType.Null;
        }
        return rec;
    }

    /// <summary>
    /// Extension method to retrieve the record type to use in a standard context.
    /// If the converter is null, or a standard alternative is not registered, the input record type is returned.
    /// </summary>
    /// <param name="converter">Optional record type mapping</param>
    /// <param name="rec">Custom RecordType to query</param>
    /// <returns>Standard RecordType if one is registered in converter.  Otherwise the input RecordType.</returns>
    public static RecordType ConvertToStandard(this RecordTypeConverter? converter, RecordType rec)
    {
        if (converter == null) return rec;
        if (converter.ToConversions.TryGetValue(rec, out var converted))
        {
            rec = converted;
        }
        else if (converter.FromConversions.ContainsKey(rec))
        {
            return RecordType.Null;
        }
        return rec;
    }

    /// <summary>
    /// Merges two converter mappings into a single one
    /// </summary>
    public static RecordTypeConverter? Combine(this RecordTypeConverter? lhs, RecordTypeConverter? rhs)
    {
        if (lhs == null) return rhs;
        if (rhs == null) return null;
        throw new NotImplementedException();
    }
}