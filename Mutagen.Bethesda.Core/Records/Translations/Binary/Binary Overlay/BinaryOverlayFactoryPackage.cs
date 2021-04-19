using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public struct BinaryOverlayFactoryPackage
    {
        public ParsingBundle MetaData;
        public IFormVersionGetter? FormVersion;

        public BinaryOverlayFactoryPackage(ParsingBundle metaData)
        {
            this.MetaData = metaData;
            this.FormVersion = null;
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
