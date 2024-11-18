using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

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
    public IFormLinkGetter<ISkyrimMajorRecordGetter> Reference => FormLinkBinaryTranslation.Instance.OverlayFactory<ISkyrimMajorRecordGetter>(_package, _structData.Span.Slice(0x10, 0x4));
    public Int32 RunOnTypeIndex => BinaryPrimitives.ReadInt32LittleEndian(_structData.Slice(0x14, 0x4));
}