using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public struct TypedParseParams
{
    public readonly RecordTypeConverter? RecordTypeConverter;
    public readonly int? LengthOverride;

    public TypedParseParams(
        int? lengthOverride, 
        RecordTypeConverter? recordTypeConverter)
    {
        LengthOverride = lengthOverride;
        RecordTypeConverter = recordTypeConverter;
    }

    public static implicit operator TypedParseParams(RecordTypeConverter? converter)
    {
        return new TypedParseParams(lengthOverride: null, converter);
    }
}

public static class TypedParseParamsExt
{
    public static RecordTypeConverter? Combine(this TypedParseParams? lhs, RecordTypeConverter? rhs)
    {
        if (lhs?.RecordTypeConverter == null) return rhs;
        if (rhs == null) return null;
        throw new NotImplementedException();
    }
        
    public static TypedParseParams With(this TypedParseParams? param, RecordTypeConverter conv)
    {
        return new TypedParseParams(
            lengthOverride: param?.LengthOverride,
            recordTypeConverter: conv);
    }

    public static TypedParseParams With(this TypedParseParams? param, RecordTypeConverter conv, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: conv);
    }

    public static TypedParseParams With(this TypedParseParams? param, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: param?.RecordTypeConverter);
    }

    public static RecordType ConvertToStandard(this TypedParseParams? converter, RecordType rec)
    {
        if (converter == null) return rec;
        return converter.Value.RecordTypeConverter.ConvertToStandard(rec);
    }
        
    public static RecordType ConvertToCustom(this TypedParseParams? converter, RecordType rec)
    {
        if (converter == null) return rec;
        return converter.Value.RecordTypeConverter.ConvertToCustom(rec);
    }
}