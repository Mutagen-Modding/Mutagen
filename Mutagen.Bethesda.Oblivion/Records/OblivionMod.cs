using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod, ILinkContainer
    {
        public INotifyingListGetter<MasterReference> MasterReferences => this.TES4.MasterReferences;

        public static IReadOnlyCollection<RecordType> NonTypeGroups { get; } = new HashSet<RecordType>(
            new RecordType[]
            {
                new RecordType("CELL"),
                new RecordType("WRLD"),
                new RecordType("DIAL"),
            });

        public bool TryGetRecord<T>(uint id, out T record)
        {
            throw new NotImplementedException();
        }
    }
}
