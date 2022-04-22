using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation;

public static class LoquiTypeExt
{
    public static async Task AddAsSubLoquiType(this LoquiType loqui, IEnumerable<ObjectGeneration> objGens)
    {
        var data = loqui.GetFieldData();
        foreach (var subObj in objGens)
        {
            var subRecs = await subObj.TryGetTriggeringRecordTypes();
            if (subRecs.Failed) continue;
            foreach (var subRec in subRecs.Value)
            {
                data.SubLoquiTypes.GetOrAdd(subRec).Add(subObj);
            }
        }
    }
}