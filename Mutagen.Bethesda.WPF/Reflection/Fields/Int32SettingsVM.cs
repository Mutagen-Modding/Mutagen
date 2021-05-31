using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields
{
    public class Int32SettingsVM : BasicSettingsVM<int>
    {
        public Int32SettingsVM(FieldMeta fieldMeta, object? defaultVal)
            : base(fieldMeta, defaultVal)
        {
        }

        public Int32SettingsVM()
            : base(FieldMeta.Empty, default)
        {
        }

        public override int Get(JsonElement property) => property.GetInt32();

        public override int GetDefault() => default(int);

        public override SettingsNodeVM Duplicate() => new Int32SettingsVM(Meta, DefaultValue);
    }
}
