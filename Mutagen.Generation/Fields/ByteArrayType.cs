using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class ByteArrayType : Loqui.Generation.ByteArrayType, IRecordTypeableFieldSetter
    {
        public RecordType? RecordType { get; set; }
        public bool Optional { get; set; }
        new public ulong? Length { get; set; }

        public override void Load(XElement node, bool requireName = true)
        {
            base.Load(node, requireName);
            this.LoadIRecordTypable(node);
            if (!this.Length.HasValue)
            {
                this.Length = 4;
            }
        }
    }
}
