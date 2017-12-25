using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod
    {
        public INotifyingListGetter<MasterReference> MasterReferences => this.TES4.MasterReferences;

        public bool TryGetRecord<T>(uint id, out T record)
        {
            throw new NotImplementedException();
        }
    }
}
