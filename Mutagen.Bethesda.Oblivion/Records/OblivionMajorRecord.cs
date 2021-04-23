using System;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMajorRecord
    {
        [Flags]
        public enum OblivionMajorRecordFlag
        {
            ESM = Records.Constants.Constants.MasterFlag,
            Deleted = Records.Constants.Constants.DeletedFlag,
            BorderRegion_ActorValue = 0x00000040,
            TurnOffFire_ActorValue = 0x00000080,
            CastsShadows = 0x00000200,
            QuestItemPersistentReference = 0x00000400,
            InitiallyDisabled = Records.Constants.Constants.InitiallyDisabled,
            Ignored = Records.Constants.Constants.Ignored,
            VisibleWhenDistant = 0x00008000,
            Dangerous_OffLimits_InteriorCell = 0x00020000,
            Compressed = Records.Constants.Constants.CompressedFlag,
            CantWait = 0x00080000,
        }

        public OblivionMajorRecordFlag OblivionMajorRecordFlags
        {
            get => (OblivionMajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        protected override ushort? FormVersionAbstract => null;
    }

    namespace Internals
    {
        public partial class OblivionMajorRecordBinaryOverlay
        {
            protected override ushort? FormVersionAbstract => null;

            public OblivionMajorRecord.OblivionMajorRecordFlag OblivionMajorRecordFlags
            {
                get => (OblivionMajorRecord.OblivionMajorRecordFlag)this.MajorRecordFlagsRaw;
            }
        }
    }
}
