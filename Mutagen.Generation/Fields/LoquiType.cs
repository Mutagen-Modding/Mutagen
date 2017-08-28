using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class LoquiType : Loqui.Generation.LoquiType, IRecordTypeableField
    {
        public bool Optional { get; set; }
        public RecordType? RecordType { get; private set; }
        public ulong? Length => null;

        public override void Load(XElement node, bool requireName = true)
        {
            base.Load(node, requireName);
            var recordAttr = node.GetAttribute("recordType");
            if (recordAttr != null)
            {
                this.RecordType = new RecordType(recordAttr);
            }
            else if (this.RefGen.Obj.CustomData.TryGetValue(nameof(RecordType), out var obj)
                && obj != null)
            { 
                this.RecordType = (RecordType)obj;
            }
            this.Optional = node.GetAttribute<bool>("optional", false);
            if (this.Optional && !this.RecordType.HasValue)
            {
                throw new ArgumentException("Cannot have an optional field if it is not a record typed field.");
            }
        }
    }
}
