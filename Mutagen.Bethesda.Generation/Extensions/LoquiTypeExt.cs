using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
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
                    data.SubLoquiTypes.TryCreateValue(subRec).Add(subObj);
                }
            }
        }
    }
}
