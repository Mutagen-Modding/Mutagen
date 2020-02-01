using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryOverlayFactoryPackage
    {
        public MasterReferences MasterReferences;
        public MetaDataConstants Meta;
        public Dictionary<RecordType, Dictionary<RecordType, object>> EdidLinkCache = new Dictionary<RecordType, Dictionary<RecordType, object>>();

        public BinaryOverlayFactoryPackage(ModKey modKey, GameMode gameMode)
        {
            this.Meta = MetaDataConstants.Get(gameMode);
            this.MasterReferences = new MasterReferences(modKey);
        }
    }
}
