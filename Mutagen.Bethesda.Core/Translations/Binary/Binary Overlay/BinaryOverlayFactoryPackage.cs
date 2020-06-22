using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryOverlayFactoryPackage
    {
        public ParsingBundle MetaData;
        public Dictionary<RecordType, Dictionary<RecordType, object>> EdidLinkCache = new Dictionary<RecordType, Dictionary<RecordType, object>>();

        public BinaryOverlayFactoryPackage(ParsingBundle metaData)
        {
            this.MetaData = metaData;
        }

        public static implicit operator ParsingBundle(BinaryOverlayFactoryPackage package)
        {
            return package.MetaData;
        }

        public static implicit operator GameConstants(BinaryOverlayFactoryPackage package)
        {
            return package.MetaData.Constants;
        }
    }
}
