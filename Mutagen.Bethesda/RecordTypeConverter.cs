using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class RecordTypeConverter
    {
        public Dictionary<RecordType, RecordType> FromConversions = new Dictionary<RecordType, RecordType>();
        public Dictionary<RecordType, RecordType> ToConversions = new Dictionary<RecordType, RecordType>();

        public RecordTypeConverter(params KeyValuePair<RecordType, RecordType>[] conversions)
        {
            foreach (var conv in conversions)
            {
                this.FromConversions[conv.Key] = conv.Value;
                this.ToConversions[conv.Value] = conv.Key;
            }
        }
    }

    public static class RecordTypeConverterExt
    {
        public static RecordType ConvertToCustom(this RecordTypeConverter converter, RecordType rec)
        {
            if (converter == null) return rec;
            if (converter.FromConversions.TryGetValue(rec, out var converted))
            {
                rec = converted;
            }
            return rec;
        }

        public static RecordType ConvertToStandard(this RecordTypeConverter converter, RecordType rec)
        {
            if (converter == null) return rec;
            if (converter.ToConversions.TryGetValue(rec, out var converted))
            {
                rec = converted;
            }
            return rec;
        }
    }
}
