using System;

namespace Mutagen.Bethesda.Plugins.Meta
{
    /// <summary>
    /// Reference for Major Record alignment and length constants
    /// </summary>
    public record MajorRecordConstants : RecordHeaderConstants
    {
        /// <summary>
        /// Offset in the header where flags are located
        /// </summary>
        public byte FlagLocationOffset { get; }

        /// <summary>
        /// Offset in the header where the FormID is located
        /// </summary>
        public byte FormIDLocationOffset { get; }

        /// <summary>
        /// Offset in the header where the Form Version is located
        /// </summary>
        public byte? FormVersionLocationOffset { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLength">Length of a Major Record header</param>
        /// <param name="lengthLength">Number of bytes representing the content length</param>
        /// <param name="flagsLoc">Offset in the header where flags are located</param>
        /// <param name="formIDloc">Offset in the header where the FormID is located</param>
        /// <param name="formVersionLoc">Offset in the header where the Form Version is located</param>
        public MajorRecordConstants(
            byte headerLength,
            byte lengthLength,
            byte flagsLoc,
            byte formIDloc,
            byte? formVersionLoc)
            : base(Records.Internals.ObjectType.Record, headerLength, lengthLength)
        {
            FlagLocationOffset = flagsLoc;
            FormIDLocationOffset = formIDloc;
            FormVersionLocationOffset = formVersionLoc;
        }
    }
}
