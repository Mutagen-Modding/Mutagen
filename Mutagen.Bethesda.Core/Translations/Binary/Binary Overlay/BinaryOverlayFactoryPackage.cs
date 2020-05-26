using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryOverlayFactoryPackage
    {
        public RecordInfoCache RecordInfoCache;
        public IStringsFolderLookup? StringsLookup;
        public MasterReferenceReader MasterReferences;
        public GameConstants Meta;
        public Dictionary<RecordType, Dictionary<RecordType, object>> EdidLinkCache = new Dictionary<RecordType, Dictionary<RecordType, object>>();

        public BinaryOverlayFactoryPackage(ModKey modKey, GameMode gameMode, RecordInfoCache? infoCache, IStringsFolderLookup? stringsLookup)
        {
            this.Meta = GameConstants.Get(gameMode);
            this.MasterReferences = new MasterReferenceReader(modKey);
            this.RecordInfoCache = infoCache ?? throw new ArgumentNullException("infoCache");
            this.StringsLookup = stringsLookup;
        }
    }
}
