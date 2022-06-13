using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public readonly struct TypedParseParams
{
    public readonly RecordTypeConverter? RecordTypeConverter;
    public readonly int? LengthOverride;
    public readonly bool ShortCircuit;

    public const bool DefaultShortCircuit = true;

    public TypedParseParams(
        int? lengthOverride, 
        RecordTypeConverter? recordTypeConverter,
        bool shortCircuit)
    {
        LengthOverride = lengthOverride;
        RecordTypeConverter = recordTypeConverter;
        ShortCircuit = shortCircuit;
    }

    public static implicit operator TypedParseParams(RecordTypeConverter? converter)
    {
        return new TypedParseParams(lengthOverride: null, converter, shortCircuit: DefaultShortCircuit);
    }

    public static TypedParseParams FromLengthOverride(int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: null,
            shortCircuit: DefaultShortCircuit);
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
            recordTypeConverter: conv, 
            shortCircuit: TypedParseParams.DefaultShortCircuit);
    }

    public static TypedParseParams With(this TypedParseParams? param, RecordTypeConverter conv, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: conv, 
            shortCircuit: TypedParseParams.DefaultShortCircuit);
    }

    public static TypedParseParams With(this TypedParseParams? param, int? lengthOverride)
    {
        return new TypedParseParams(
            lengthOverride: lengthOverride,
            recordTypeConverter: param?.RecordTypeConverter,
            shortCircuit: TypedParseParams.DefaultShortCircuit);
    }

    public static TypedParseParams DoNotShortCircuit(this TypedParseParams? param)
    {
        return new TypedParseParams(
            lengthOverride: param?.LengthOverride,
            recordTypeConverter: param?.RecordTypeConverter, 
            shortCircuit: false);
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