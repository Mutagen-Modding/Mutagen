using System;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Fallout4MajorRecord
    {
        [Flags]
        public enum Fallout4MajorRecordFlag
        {
            ESM = Records.Constants.Constants.MasterFlag,
            NotPlayable = 0x0000_0004,
            Deleted = Records.Constants.Constants.DeletedFlag,
            InitiallyDisabled = Records.Constants.Constants.InitiallyDisabled,
            Ignored = Records.Constants.Constants.Ignored,
            VisibleWhenDistant = 0x00008000,
            Dangerous_OffLimits_InteriorCell = 0x00020000,
            Compressed = Records.Constants.Constants.CompressedFlag,
            CantWait = 0x00080000,
        }

        public Fallout4MajorRecordFlag Fallout4MajorRecordFlags
        {
            get => (Fallout4MajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        protected override ushort? FormVersionAbstract => this.FormVersion;
    }

    namespace Internals
    {
        public partial class Fallout4MajorRecordBinaryOverlay
        {
            protected override ushort? FormVersionAbstract => this.FormVersion;
        }
    }
}
