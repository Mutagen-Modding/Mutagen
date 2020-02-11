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

        public IReadOnlyList<IMasterReferenceGetter> Masters;
        public ModKey CurrentMod;

        public MasterReferences(ModKey modKey)
        {
            this.CurrentMod = modKey;
            this.Masters = new List<IMasterReferenceGetter>();
        }

        public MasterReferences(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            this.Masters = new List<IMasterReferenceGetter>(masters);
        }

        public MasterReferences(ModKey modKey, IReadOnlyList<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            this.Masters = masters;
        }
    }
}
