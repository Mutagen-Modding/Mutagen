namespace Mutagen.Bethesda.Fallout4;

public partial class Fallout4MajorRecord
{
    [Flags]
    public enum Fallout4MajorRecordFlag
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

    public Fallout4MajorRecordFlag Fallout4MajorRecordFlags
    {
        get => (Fallout4MajorRecordFlag)this.MajorRecordFlagsRaw;
        set => this.MajorRecordFlagsRaw = (int)value;
    }

    protected override ushort? FormVersionAbstract => this.FormVersion;
}

internal partial class Fallout4MajorRecordBinaryOverlay
{
    protected override ushort? FormVersionAbstract => this.FormVersion;
}