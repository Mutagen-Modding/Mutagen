using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public readonly struct TypedParseParams
{
    public readonly RecordTypeConverter? RecordTypeConverter;
    public readonly int? LengthOverride;
    private readonly bool _doNotShortCircuit;
    public bool ShortCircuit => !_doNotShortCircuit;

    public TypedParseParams(
        int? lengthOverride, 
        RecordTypeConverter? recordTypeConverter,
        bool doNotShortCircuit)
    {
        LengthOverride = lengthOverride;
        RecordTypeConverter = recordTypeConverter;
        _doNotShortCircuit = doNotShortCircuit;
    }

    public static implicit operator TypedParseParams(RecordTypeConverter? converter)
    {
        return new TypedParseParams(lengthOverride: null, converter, doNotShortCircuit: default);
    }

    public static TypedParseParams FromLengthOverride(int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: null,
            doNotShortCircuit: default);
    }
}

public static class TypedParseParamsExt
{
    public static RecordTypeConverter? Combine(this TypedParseParams lhs, RecordTypeConverter? rhs)
    {
        if (lhs.RecordTypeConverter == null) return rhs;
        if (rhs == null) return null;
        throw new NotImplementedException();
    }
        
    public static TypedParseParams With(this TypedParseParams param, RecordTypeConverter conv)
    {
        return new TypedParseParams(
            lengthOverride: param.LengthOverride,
            recordTypeConverter: conv, 
            doNotShortCircuit: default);
    }

    public static TypedParseParams With(this TypedParseParams param, RecordTypeConverter conv, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: conv, 
            doNotShortCircuit: default);
    }

    public static TypedParseParams With(this TypedParseParams param, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: param.RecordTypeConverter, 
            doNotShortCircuit: default);
    }

    public static TypedParseParams DoNotShortCircuit(this TypedParseParams param)
    {
        return new TypedParseParams(
            lengthOverride: param.LengthOverride,
            recordTypeConverter: param.RecordTypeConverter, 
            doNotShortCircuit: true);
    }

    public static TypedParseParams ShortCircuit(this TypedParseParams param)
    {
        return new TypedParseParams(
            lengthOverride: param.LengthOverride,
            recordTypeConverter: param.RecordTypeConverter, 
            doNotShortCircuit: false);
    }

    public static RecordType ConvertToStandard(this TypedParseParams converter, RecordType rec)
    {
        return converter.RecordTypeConverter.ConvertToStandard(rec);
    }
        
    public static RecordType ConvertToCustom(this TypedParseParams converter, RecordType rec)
    {
        return converter.RecordTypeConverter.ConvertToCustom(rec);
    }
}