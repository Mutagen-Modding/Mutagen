using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimMajorRecord
    {
        [Flags]
        public enum SkyrimMajorRecordFlag
        {
            ESM = Mutagen.Bethesda.Constants.Constants.MasterFlag,
            NotPlayable = 0x0000_0004,
            Deleted = Mutagen.Bethesda.Constants.Constants.DeletedFlag,
            InitiallyDisabled = Mutagen.Bethesda.Constants.Constants.InitiallyDisabled,
            Ignored = Mutagen.Bethesda.Constants.Constants.Ignored,
            VisibleWhenDistant = 0x00008000,
            Dangerous_OffLimits_InteriorCell = 0x00020000,
            Compressed = Mutagen.Bethesda.Constants.Constants.CompressedFlag,
            CantWait = 0x00080000,
        }

        public SkyrimMajorRecordFlag SkyrimMajorRecordFlags
        {
            get => (SkyrimMajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        protected override ushort? FormVersionAbstract => this.FormVersion;
    }

    namespace Internals
    {
        public partial class SkyrimMajorRecordBinaryOverlay
        {
            protected override ushort? FormVersionAbstract => this.FormVersion;
        }
    }
}
