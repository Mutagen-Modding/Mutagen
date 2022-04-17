using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class DecimalSettingsVM : BasicSettingsVM<decimal>
{
    public DecimalSettingsVM(FieldMeta meta, object? defaultVal)
        : base(meta, defaultVal)
    {
    }

    public DecimalSettingsVM()
        : base(FieldMeta.Empty, default)
    {
    }

    public override decimal Get(JsonElement property) => property.GetDecimal();

    public override decimal GetDefault() => default(decimal);

    public override SettingsNodeVM Duplicate() => new DecimalSettingsVM(Meta, DefaultValue);
}