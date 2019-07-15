using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryWrapperFactoryPackage
    {
        public MasterReferences MasterReferences;
        public MetaDataConstants Meta;
        public IModGetter Mod;
        public Dictionary<RecordType, Dictionary<RecordType, object>> EdidLinkCache = new Dictionary<RecordType, Dictionary<RecordType, object>>();
    }
}
