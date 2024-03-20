using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldMajorRecord
{
    [Flags]
    public enum StarfieldMajorRecordFlag
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

    public StarfieldMajorRecordFlag StarfieldMajorRecordFlags
    {
        get => (StarfieldMajorRecordFlag)this.MajorRecordFlagsRaw;
        set => this.MajorRecordFlagsRaw = (int)value;
    }

    protected override ushort? FormVersionAbstract => this.FormVersion;
}

public partial interface IStarfieldMajorRecord : IFormVersionSetter
{
}

partial class StarfieldMajorRecordBinaryOverlay
{
    protected override ushort? FormVersionAbstract => this.FormVersion;

    public StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags
    {
        get => (StarfieldMajorRecord.StarfieldMajorRecordFlag)this.MajorRecordFlagsRaw;
    }
}