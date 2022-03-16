using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace Mutagen.Bethesda.Plugins.Internals
{
    public class TriggeringRecordCollection
    {
        private readonly IReadOnlyList<RecordType> _ordered;
        private readonly IReadOnlySet<RecordType> _set;

        public TriggeringRecordCollection(params RecordType[] types)
        {
            // ToDo
            // Perhaps optimize and only make set if above X number of types
            _ordered = types;
            _set = types.ToHashSet();
        }

        public bool Contains(RecordType type) => _set.Contains(type);

        public int IndexOf(RecordType type) => _ordered.IndexOf(type);
    }
}
