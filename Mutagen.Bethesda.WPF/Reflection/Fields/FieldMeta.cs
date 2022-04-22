namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public record FieldMeta(
    string DisplayName,
    string DiskName,
    string? Tooltip,
    ReflectionSettingsVM MainVM,
    SettingsNodeVM? Parent,
    bool IsPassthrough)
{
    public static readonly FieldMeta Empty = new(string.Empty, string.Empty, null, null!, null, false);
}