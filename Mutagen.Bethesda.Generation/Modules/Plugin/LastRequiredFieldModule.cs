using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class LastRequiredFieldModule : GenerationModule
{
    public override async Task PostLoad(ObjectGeneration obj)
    {
        obj.GetObjectData().ShortCircuitToLastRequiredField = obj.Node.GetAttribute("shortCircuitToLastRequired", false);
        var lastField = obj.Fields
            .WithIndex()
            .Where(x => x.Item.GetFieldData().HasTrigger)
            .Where(x => !x.Item.Nullable)
            .LastOrDefault();
        if (lastField.Item == null) return;
        obj.GetObjectData().LastRequiredFieldIndex = lastField.Index;
    }
}