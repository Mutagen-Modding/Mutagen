namespace Mutagen.Bethesda.Papyrus.Semantic;

public enum ConstantValueKind
{
    None,
    Bool,
    Int,
    Float,
    String,
}

/// <summary>The compile-time value of a literal expression.</summary>
public readonly struct ConstantValue
{
    public ConstantValueKind Kind { get; }

    private ConstantValue(ConstantValueKind kind)
    {
        Kind = kind;
    }

    public bool BoolValue { get; private init; }
    public long IntValue { get; private init; }
    public double FloatValue { get; private init; }
    public string? StringValue { get; private init; }

    public static ConstantValue None { get; } = new(ConstantValueKind.None);
    public static ConstantValue Bool(bool value) => new(ConstantValueKind.Bool) { BoolValue = value };
    public static ConstantValue Int(long value) => new(ConstantValueKind.Int) { IntValue = value };
    public static ConstantValue Float(double value) => new(ConstantValueKind.Float) { FloatValue = value };
    public static ConstantValue String(string value) => new(ConstantValueKind.String) { StringValue = value };

    public object? Value => Kind switch
    {
        ConstantValueKind.Bool => BoolValue,
        ConstantValueKind.Int => IntValue,
        ConstantValueKind.Float => FloatValue,
        ConstantValueKind.String => StringValue,
        _ => null,
    };

    public override string ToString() => Kind switch
    {
        ConstantValueKind.Bool => BoolValue ? "true" : "false",
        ConstantValueKind.Int => IntValue.ToString(),
        ConstantValueKind.Float => FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ConstantValueKind.String => $"\"{StringValue}\"",
        _ => "None",
    };
}
