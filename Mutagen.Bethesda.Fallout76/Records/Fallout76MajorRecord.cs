using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

public partial class Fallout76MajorRecord
{
    [Flags]
    public enum Fallout76MajorRecordFlag
    {
        ESM = Plugins.Internals.Constants.MasterFlag,
        NotPlayable = 0x0000_0004,
        Deleted = Plugins.Internals.Constants.DeletedFlag,
        InitiallyDisabled = Plugins.Internals.Constants.InitiallyDisabled,
        Ignored = Plugins.Internals.Constants.Ignored,
        VisibleWhenDistant = 0x00008000,
        Dangerous_OffLimits_InteriorCell = 0x00020000,
        Compressed = Plugins.Internals.Constants.CompressedFlag,
        CantWait = 0x00080000,
    }

    public Fallout76MajorRecordFlag Fallout76MajorRecordFlags
    {
        get => (Fallout76MajorRecordFlag)this.MajorRecordFlagsRaw;
        set => this.MajorRecordFlagsRaw = (int)value;
    }

    protected override ushort? FormVersionAbstract => this.FormVersion;
}

public partial interface IFallout76MajorRecord : IFormVersionSetter
{
}

internal partial class Fallout76MajorRecordBinaryOverlay
{
    protected override ushort? FormVersionAbstract => this.FormVersion;

    public Fallout76MajorRecord.Fallout76MajorRecordFlag Fallout76MajorRecordFlags
    {
        get => (Fallout76MajorRecord.Fallout76MajorRecordFlag)this.MajorRecordFlagsRaw;
    }
}