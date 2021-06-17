using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Streams
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
        public IMasterReferenceReader? MasterReferences { get; set; }

        /// <summary>
        /// Optional strings writer for easy access during write operations
        /// </summary>
        public StringsWriter? StringsWriter { get; set; }

        /// <summary>
        /// Optional RecordInfoCache to reference while reading
        /// </summary>
        public RecordTypeInfoCacheReader? RecordInfoCache { get; set; }

        /// <summary>
        /// Tracker of current major record version
        /// </summary>
        public ushort? FormVersion { get; set; }

        /// <summary>
        /// If a FormID has all zeros for the ID, but a non-zero mod index, then set mod index to zero as well.
        /// </summary>
        public bool CleanNulls { get; set; } = true;

        public WritingBundle(GameConstants constants)
        {
            this.Constants = constants;
        }
    }
}
