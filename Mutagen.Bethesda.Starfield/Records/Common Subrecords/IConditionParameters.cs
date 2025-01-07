namespace Mutagen.Bethesda.Starfield;

public interface IConditionParametersGetter
{
    string? StringParameter1 { get; }
    string? StringParameter2 { get; }
    object? Parameter1 { get; }
    object? Parameter2 { get; }
    new Type? Parameter1Type { get; }
    new Type? Parameter2Type { get; }
}

public interface IConditionParameters : IConditionParametersGetter
{
    new string? StringParameter1 { get; set; }
    new string? StringParameter2 { get; set; }
    new object? Parameter1 { get; set; }
    new object? Parameter2 { get; set; }
}
