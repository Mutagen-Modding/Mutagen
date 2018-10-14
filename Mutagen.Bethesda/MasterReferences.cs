using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class MasterReferences
    {
        public static MasterReferences Empty { get; } = new MasterReferences();
        public readonly IReadOnlyList<IMasterReferenceGetter> Masters;
        public readonly ModKey CurrentMod;

        private MasterReferences()
        {
        }

        public MasterReferences(
            IReadOnlyList<IMasterReferenceGetter> masters,
            ModKey modKey)
        {
            this.Masters = masters;
            this.CurrentMod = modKey;
        }
    }
}
