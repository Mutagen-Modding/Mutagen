using Loqui.Generation;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MapsToGetterModule : GenerationModule
{
    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        if (obj.Abstract || !await obj.IsMajorRecord()) return;
        obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, $"IMapsToGetter<{obj.Interface(getter: true, internalInterface: false)}>");
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        yield return "Loqui.Interfaces";
    }
}