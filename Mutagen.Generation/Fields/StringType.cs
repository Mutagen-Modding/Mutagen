using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class StringType : Loqui.Generation.StringType, IRecordTypeableFieldSetter
    {
        public bool Optional { get; set; }
        public RecordType? RecordType { get; set; }
        public ulong? Length { get; set; }

        public override void Load(XElement node, bool requireName = true)
        {
            base.Load(node, requireName);
            this.LoadIRecordTypable(node);
        }
    }
}
