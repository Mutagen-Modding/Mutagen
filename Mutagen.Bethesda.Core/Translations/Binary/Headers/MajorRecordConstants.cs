using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Reference for Major Record alignment and length constants
    /// </summary>
    public class MajorRecordConstants : RecordHeaderConstants
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
        /// Constructor
        /// </summary>
        /// <param name="gameMode">GameMode to associate with the constants</param>
        /// <param name="headerLength">Length of a Major Record header</param>
        /// <param name="lengthLength">Number of bytes representing the content length</param>
        /// <param name="flagsLoc">Offset in the header where flags are located</param>
        /// <param name="formIDloc">Offset in the header where the FormID is located</param>
        public MajorRecordConstants(
            GameMode gameMode,
            sbyte headerLength,
            sbyte lengthLength,
            sbyte flagsLoc,
            sbyte formIDloc)
            : base(gameMode, ObjectType.Record, headerLength, lengthLength)
        {
            this.FlagLocationOffset = flagsLoc;
            this.FormIDLocationOffset = formIDloc;
        }
    }
}
