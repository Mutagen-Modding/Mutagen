namespace Mutagen.Bethesda.Starfield;

internal interface IConditionStringParameterGetter
{
    string? FirstStringParameter { get; }
    string? SecondStringParameter { get; }
}

internal interface IConditionStringParameter : IConditionStringParameterGetter
{
    new string? FirstStringParameter { get; set; }
    new string? SecondStringParameter { get; set; }
}