using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public struct TypedWriteParams
{
    public readonly RecordTypeConverter? RecordTypeConverter;
    public readonly RecordType? OverflowRecordType;

    public TypedWriteParams(
        RecordTypeConverter? recordTypeConverter,
        RecordType? overflowRecordType)
    {
        RecordTypeConverter = recordTypeConverter;
        OverflowRecordType = overflowRecordType;
    }

    public static implicit operator TypedWriteParams(RecordTypeConverter? converter)
    {
        return new TypedWriteParams(converter, overflowRecordType: null);
    }
}

public static class TypedWriteParamsExt
{
    public static RecordTypeConverter? Combine(this TypedWriteParams? lhs, RecordTypeConverter? rhs)
    {
        if (lhs?.RecordTypeConverter == null) return rhs;
        if (rhs == null) return null;
        throw new NotImplementedException();
    }
        
    public static TypedWriteParams With(this TypedWriteParams? converter, RecordTypeConverter conv)
    {
        return new TypedWriteParams(
            recordTypeConverter: conv,
            overflowRecordType: converter?.OverflowRecordType);
    }
        
    public static TypedWriteParams With(this TypedWriteParams? converter, RecordType overflow)
    {
        return new TypedWriteParams(
            recordTypeConverter: null,
            overflowRecordType: overflow);
    }

    public static RecordType ConvertToStandard(this TypedWriteParams? converter, RecordType rec)
    {
        if (converter == null) return rec;
        return converter.Value.RecordTypeConverter.ConvertToStandard(rec);
    }
        
    public static RecordType ConvertToCustom(this TypedWriteParams? converter, RecordType rec)
    {
        if (converter == null) return rec;
        return converter.Value.RecordTypeConverter.ConvertToCustom(rec);
    }
}