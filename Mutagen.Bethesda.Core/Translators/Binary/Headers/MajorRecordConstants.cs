using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MajorRecordConstants : RecordHeaderConstants
    {
        public sbyte FlagLocationOffset { get; }
        public sbyte FormIDLocationOffset { get; }

        public MajorRecordConstants(
            GameMode gameMode,
            ObjectType type,
            sbyte headerLength,
            sbyte lengthLength,
            sbyte flagsLoc,
            sbyte formIDloc)
            : base(gameMode, type, headerLength, lengthLength)
        {
            this.FlagLocationOffset = flagsLoc;
            this.FormIDLocationOffset = formIDloc;
        }
    }
}
