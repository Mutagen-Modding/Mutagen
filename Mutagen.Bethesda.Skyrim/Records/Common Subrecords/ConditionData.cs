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
}

partial class ConditionDataBinaryOverlay : IFormLinkOrAliasFlagGetter, IConditionStringParameterGetter
{
    public bool UseAliases { get; internal set; }
    string? IConditionStringParameterGetter.FirstStringParameter => throw new NotImplementedException();
    string? IConditionStringParameterGetter.SecondStringParameter => throw new NotImplementedException();
}