using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class UnknownSettingsVM : SettingsNodeVM
{
    public UnknownSettingsVM(FieldMeta fieldMeta)
        : base(fieldMeta)
    {
    }

    public override void Import(JsonElement property, Action<string> logger)
    {
        logger($"Tried to import for unknown setting: {this.Meta}");
    }

    public override void Persist(JObject obj, Action<string> logger)
    {
    }

    public override SettingsNodeVM Duplicate() => this;
}