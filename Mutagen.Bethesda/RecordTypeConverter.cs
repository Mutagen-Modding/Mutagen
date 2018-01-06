using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class RecordTypeConverter
    {
        public Dictionary<RecordType, RecordType> Conversions = new Dictionary<RecordType, RecordType>();
    }

    public static class RecordTypeConverterExt
    {
        public static RecordType Convert(this RecordTypeConverter converter, RecordType rec)
        {
            if (converter == null) return rec;
            if (converter.Conversions.TryGetValue(rec, out var converted))
            {
                rec = converted;
            }
            return rec;
        }
    }
}
