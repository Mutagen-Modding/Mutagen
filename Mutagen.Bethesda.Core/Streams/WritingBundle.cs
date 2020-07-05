using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Class containing all the extra meta bits for writing
    /// </summary>
    public class WritingBundle
    {
        /// <summary>
        /// Game constants meta object to reference for header length measurements
        /// </summary>
        public GameConstants Constants { get; }

        /// <summary>
        /// Optional master references for easy access during write operations
        /// </summary>
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <summary>
        /// Optional strings writer for easy access during write operations
        /// </summary>
        public StringsWriter? StringsWriter { get; set; }

        /// <summary>
        /// Optional RecordInfoCache to reference while reading
        /// </summary>
        public RecordInfoCache? RecordInfoCache { get; set; }

        /// <summary>
        /// Tracker of current major record version
        /// </summary>
        public ushort? FormVersion { get; set; }

        public WritingBundle(GameConstants constants)
        {
            this.Constants = constants;
        }
    }
}
