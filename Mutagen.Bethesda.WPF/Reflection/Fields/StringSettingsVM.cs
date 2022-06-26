using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class StringSettingsVM : BasicSettingsVM<string>
{
    public StringSettingsVM(FieldMeta fieldMeta, object? defaultVal)
        : base(fieldMeta, defaultVal)
    {
    }

    public StringSettingsVM()
        : base(FieldMeta.Empty, default)
    {
    }

    public override string Get(JsonElement property) => property.GetString() ?? string.Empty;

    public override string GetDefault() => string.Empty;

    public override SettingsNodeVM Duplicate() => new StringSettingsVM(Meta, DefaultValue);
}