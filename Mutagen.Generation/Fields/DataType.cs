using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Mutagen.Generation;

namespace Mutagen
{
    public class DataType : SetMarkerType
    {
        public override async Task Load(XElement node, bool requireName = true)
        {
            var data = this.CustomData.TryCreateValue(Mutagen.Generation.Constants.DATA_KEY, () => new MutagenFieldData(this)) as MutagenFieldData;
            data.RecordType = new RecordType("DATA");
            await base.Load(node, requireName: false);
        }
    }
}
