using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public class MasterReferenceReader
    {
        public static MasterReferenceReader Empty { get; } = new MasterReferenceReader(ModKey.Null);

        public IReadOnlyList<IMasterReferenceGetter> Masters;
        public ModKey CurrentMod;

        public MasterReferenceReader(ModKey modKey)
        {
            this.CurrentMod = modKey;
            this.Masters = new List<IMasterReferenceGetter>();
        }

        public MasterReferenceReader(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            this.Masters = new List<IMasterReferenceGetter>(masters);
        }

        public MasterReferenceReader(ModKey modKey, IReadOnlyList<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            this.Masters = masters;
        }
    }
}
