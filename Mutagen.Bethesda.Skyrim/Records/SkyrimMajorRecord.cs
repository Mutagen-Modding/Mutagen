using System;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimMajorRecord
    {
        [Flags]
        public enum SkyrimMajorRecordFlag
        {
            ESM = Mutagen.Bethesda.Plugins.Internals.Constants.MasterFlag,
            NotPlayable = 0x0000_0004,
            Deleted = Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag,
            InitiallyDisabled = Mutagen.Bethesda.Plugins.Internals.Constants.InitiallyDisabled,
            Ignored = Mutagen.Bethesda.Plugins.Internals.Constants.Ignored,
            VisibleWhenDistant = 0x00008000,
            Dangerous_OffLimits_InteriorCell = 0x00020000,
            Compressed = Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag,
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
