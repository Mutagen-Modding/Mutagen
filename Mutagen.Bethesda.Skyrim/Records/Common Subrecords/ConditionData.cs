using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

public static class ConditionDataExtension
{
    public static bool TryGetParameter1(this IConditionParametersGetter conditionData, [MaybeNullWhen(false)] out Type type, [MaybeNullWhen(false)] out object value)
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

    public static bool TrySetParameter1(this IConditionParameters conditionData, object? value)
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

partial class ConditionData : IFormLinkOrIndexFlagGetter, IConditionParameters
{
    string? IConditionParameters.StringParameter1
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    string? IConditionParameters.StringParameter2
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    object? IConditionParameters.Parameter1
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    object? IConditionParameters.Parameter2
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    string? IConditionParametersGetter.StringParameter1 => throw new NotImplementedException();
    string? IConditionParametersGetter.StringParameter2 => throw new NotImplementedException();
    object? IConditionParametersGetter.Parameter1 => throw new NotImplementedException();
    object? IConditionParametersGetter.Parameter2 => throw new NotImplementedException();
    Type? IConditionParametersGetter.Parameter1Type => throw new NotImplementedException();
    Type? IConditionParametersGetter.Parameter2Type => throw new NotImplementedException();
    Condition.Function IConditionDataGetter.Function => throw new NotImplementedException();
}

public partial interface IConditionDataGetter
{
    Condition.Function Function { get; }
}

partial class ConditionDataBinaryOverlay : IFormLinkOrIndexFlagGetter, IConditionParametersGetter
{
    public bool UseAliases { get; internal set; }
    public bool UsePackageData { get; internal set; }
    string? IConditionParametersGetter.StringParameter1 => throw new NotImplementedException();
    string? IConditionParametersGetter.StringParameter2 => throw new NotImplementedException();
    Condition.Function IConditionDataGetter.Function => throw new NotImplementedException();
    object? IConditionParametersGetter.Parameter1 => throw new NotImplementedException();
    object? IConditionParametersGetter.Parameter2 => throw new NotImplementedException();
    Type? IConditionParametersGetter.Parameter1Type => throw new NotImplementedException();
    Type? IConditionParametersGetter.Parameter2Type => throw new NotImplementedException();
}

partial class ConditionDataBinaryOverlay
{
    public Condition.RunOnType RunOnType => (Condition.RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_structData.Span.Slice(0xC, 0x4));
    public IFormLinkGetter<ISkyrimMajorRecordGetter> Reference => FormLinkBinaryTranslation.Instance.OverlayFactory<ISkyrimMajorRecordGetter>(_package, _structData.Span.Slice(0x10, 0x4));
    public Int32 RunOnTypeIndex => BinaryPrimitives.ReadInt32LittleEndian(_structData.Slice(0x14, 0x4));
}