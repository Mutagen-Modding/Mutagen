using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay
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
