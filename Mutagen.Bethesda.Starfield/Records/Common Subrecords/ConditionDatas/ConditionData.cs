using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

public static class ConditionDataExtension
{
    public static bool TryGetParameter1(this IConditionDataGetter conditionData, [MaybeNullWhen(false)] out Type type, [MaybeNullWhen(false)] out object value)
    {
        type = conditionData.Parameter1Type;
        if (type is null)
        {
            value = null;
            return false;
        }

        value = conditionData.Parameter1;
        return value is not null;
    }

    public static bool TrySetParameter1(this IConditionData conditionData, object? value)
    {
        try
        {
            conditionData.Parameter1 = value;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

partial class ConditionData : IFormLinkOrIndexFlagGetter, IConditionStringParameter
{
    string? IConditionStringParameter.FirstStringParameter
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    string? IConditionStringParameter.SecondStringParameter
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    string? IConditionStringParameterGetter.FirstStringParameter => throw new NotImplementedException();
    string? IConditionStringParameterGetter.SecondStringParameter => throw new NotImplementedException();
    Condition.Function IConditionDataGetter.Function => throw new NotImplementedException();
    public abstract object? Parameter1 { get; set; }
    public abstract Type? Parameter1Type { get; }
    public abstract object? Parameter2 { get; set; }
    public abstract Type? Parameter2Type { get; }
}

public partial interface IConditionDataGetter
{
    Condition.Function Function { get; }
    object? Parameter1 { get; }
    Type? Parameter1Type { get; }
    object? Parameter2 { get; }
    Type? Parameter2Type { get; }
}

public partial interface IConditionData
{
    object? Parameter1 { get; set; }
    object? Parameter2 { get; set; }
}

partial class ConditionDataBinaryOverlay : IFormLinkOrIndexFlagGetter, IConditionStringParameterGetter
{
    public bool UseAliases { get; internal set; }
    public bool UsePackageData { get; internal set; }
    string? IConditionStringParameterGetter.FirstStringParameter => throw new NotImplementedException();
    string? IConditionStringParameterGetter.SecondStringParameter => throw new NotImplementedException();
    Condition.Function IConditionDataGetter.Function => throw new NotImplementedException();
    public object? Parameter1 => throw new NotImplementedException();
    public Type? Parameter1Type => throw new NotImplementedException();
    public object? Parameter2 => throw new NotImplementedException();
    public Type? Parameter2Type => throw new NotImplementedException();
}

partial class ConditionDataBinaryOverlay
{
    public Condition.RunOnType RunOnType => (Condition.RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_structData.Span.Slice(0xC, 0x4));
    public IFormLinkGetter<IStarfieldMajorRecordGetter> Reference => FormLinkBinaryTranslation.Instance.OverlayFactory<IStarfieldMajorRecordGetter>(_package, _structData.Span.Slice(0x10, 0x4));
    public Int32 Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_structData.Slice(0x14, 0x4));
}