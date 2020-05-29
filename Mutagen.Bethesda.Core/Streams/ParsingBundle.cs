using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Class containing all the extra meta bits for parsing
    /// </summary>
    public class ParsingBundle
    {
        /// <summary>
        /// Game constants meta object to reference for header length measurements
        /// </summary>
        public GameConstants Constants { get; }

        /// <summary>
        /// Optional MasterReferenceReader to reference while reading
        /// </summary>
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <summary>
        /// Optional RecordInfoCache to reference while reading
        /// </summary>
        public RecordInfoCache? RecordInfoCache { get; set; }

        /// <summary>
        /// Optional strings lookup to reference while reading
        /// </summary>
        public IStringsFolderLookup? StringsLookup { get; set; }

        public ParsingBundle(GameConstants constants)
        {
            this.Constants = constants;
        }

        public ParsingBundle Spawn(GameMode mode)
        {
            return new ParsingBundle(mode)
            {
                MasterReferences = this.MasterReferences,
                RecordInfoCache = this.RecordInfoCache,
                StringsLookup = this.StringsLookup,
            };
        }
    }
}
