using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMajorRecord
    {
        [Flags]
        public enum OblivionMajorRecordFlag
        {
            ESM = 0x00000001,
            Deleted = 0x00000020,
            BorderRegion_ActorValue = 0x00000040,
            TurnOffFire_ActorValue = 0x00000080,
            CastsShadows = 0x00000200,
            QuestItemPersistentReference = 0x00000400,
            InitiallyDisabled = 0x00000800,
            Ignored = 0x00001000,
            VisibleWhenDistant = 0x00008000,
            Dangerous_OffLimits_InteriorCell = 0x00020000,
            Compressed = 0x00040000,
            CantWait = 0x00080000,
        }

        public OblivionMajorRecordFlag OblivionMajorRecordFlags
        {
            get => (OblivionMajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }
    }

    namespace Internals
    {
        public partial class OblivionMajorRecordBinaryOverlay
        {
            public OblivionMajorRecord.OblivionMajorRecordFlag OblivionMajorRecordFlags
            {
                get => (OblivionMajorRecord.OblivionMajorRecordFlag)this.MajorRecordFlagsRaw;
            }
        }
    }
}
