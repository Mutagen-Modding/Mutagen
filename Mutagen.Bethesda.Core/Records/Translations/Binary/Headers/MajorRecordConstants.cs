using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Reference for Major Record alignment and length constants
    /// </summary>
    public record MajorRecordConstants : RecordHeaderConstants
    {
        /// <summary>
        /// Offset in the header where flags are located
        /// </summary>
        public sbyte FlagLocationOffset { get; }

        /// <summary>
        /// Offset in the header where the FormID is located
        /// </summary>
        public sbyte FormIDLocationOffset { get; }

        /// <summary>
        /// Offset in the header where the Form Version is located
        /// </summary>
        public sbyte? FormVersionLocationOffset { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headerLength">Length of a Major Record header</param>
        /// <param name="lengthLength">Number of bytes representing the content length</param>
        /// <param name="flagsLoc">Offset in the header where flags are located</param>
        /// <param name="formIDloc">Offset in the header where the FormID is located</param>
        /// <param name="formVersionLoc">Offset in the header where the Form Version is located</param>
        public MajorRecordConstants(
            sbyte headerLength,
            sbyte lengthLength,
            sbyte flagsLoc,
            sbyte formIDloc,
            sbyte? formVersionLoc)
            : base(ObjectType.Record, headerLength, lengthLength)
        {
            this.FlagLocationOffset = flagsLoc;
            this.FormIDLocationOffset = formIDloc;
            this.FormVersionLocationOffset = formVersionLoc;
        }
    }
}
