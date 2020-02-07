using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public class MasterReferences
    {
        public static MasterReferences Empty { get; } = new MasterReferences(ModKey.Null);

        public readonly IList<IMasterReferenceGetter> Masters = new List<IMasterReferenceGetter>();
        public readonly ModKey CurrentMod;

        public MasterReferences(ModKey modKey)
        {
            this.CurrentMod = modKey;
        }

        public MasterReferences(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            this.Masters.AddRange(masters);
        }

        public MasterReferences(ModKey modKey, params IMasterReferenceGetter[] masters)
        {
            this.CurrentMod = modKey;
            this.Masters.AddRange(masters);
        }
    }
}
