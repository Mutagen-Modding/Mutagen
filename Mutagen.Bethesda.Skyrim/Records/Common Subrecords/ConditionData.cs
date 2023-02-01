using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

partial class ConditionData : IFormLinkOrAliasFlagGetter, IConditionStringParameter
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
}

public partial interface IConditionDataGetter
{
    Condition.Function Function { get; }
}

partial class ConditionDataBinaryOverlay : IFormLinkOrAliasFlagGetter, IConditionStringParameterGetter
{
    public bool UseAliases { get; internal set; }
    string? IConditionStringParameterGetter.FirstStringParameter => throw new NotImplementedException();
    string? IConditionStringParameterGetter.SecondStringParameter => throw new NotImplementedException();
    Condition.Function IConditionDataGetter.Function => throw new NotImplementedException();
}

partial class ConditionDataBinaryOverlay
{
    public Condition.RunOnType RunOnType => (Condition.RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_structData.Span.Slice(0xC, 0x4));
    public IFormLinkGetter<ISkyrimMajorRecordGetter> Reference => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_structData.Span.Slice(0x10, 0x4))));
    public Int32 Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_structData.Slice(0x14, 0x4));
}