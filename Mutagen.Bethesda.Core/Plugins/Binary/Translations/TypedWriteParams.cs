using System.Diagnostics;
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
    [DebuggerStepThrough]
    public static RecordTypeConverter? Combine(this TypedWriteParams lhs, RecordTypeConverter? rhs)
    {
        if (lhs.RecordTypeConverter == null) return rhs;
        if (rhs == null) return null;
        throw new NotImplementedException();
    }

    [DebuggerStepThrough]
    public static TypedWriteParams With(this TypedWriteParams converter, RecordTypeConverter conv)
    {
        return new TypedWriteParams(
            recordTypeConverter: conv,
            overflowRecordType: converter.OverflowRecordType);
    }

    [DebuggerStepThrough]
    public static TypedWriteParams With(this TypedWriteParams converter, RecordType overflow)
    {
        return new TypedWriteParams(
            recordTypeConverter: null,
            overflowRecordType: overflow);
    }

    [DebuggerStepThrough]
    public static RecordType ConvertToStandard(this TypedWriteParams converter, RecordType rec)
    {
        return converter.RecordTypeConverter.ConvertToStandard(rec);
    }

    [DebuggerStepThrough]
    public static RecordType ConvertToCustom(this TypedWriteParams converter, RecordType rec)
    {
        return converter.RecordTypeConverter.ConvertToCustom(rec);
    }
}