namespace Mutagen.Bethesda.Fallout3;

public partial class Fallout3MajorRecord
{
    [Flags]
    public enum Fallout3MajorRecordFlag
    {
        ESM = Mutagen.Bethesda.Plugins.Internals.Constants.MasterFlag,
        Deleted = Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag,
        BorderRegion_ActorValue = 0x00000040,
        TurnOffFire_ActorValue = 0x00000080,
        CastsShadows = 0x00000200,
        QuestItemPersistentReference = 0x00000400,
        InitiallyDisabled = Mutagen.Bethesda.Plugins.Internals.Constants.InitiallyDisabled,
        Ignored = Mutagen.Bethesda.Plugins.Internals.Constants.Ignored,
        VisibleWhenDistant = 0x00008000,
        Dangerous_OffLimits_InteriorCell = 0x00020000,
        Compressed = Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag,
        CantWait = 0x00080000,
    }

    public Fallout3MajorRecordFlag Fallout3MajorRecordFlags
    {
        get => (Fallout3MajorRecordFlag)this.MajorRecordFlagsRaw;
        set => this.MajorRecordFlagsRaw = (int)value;
    }

    protected override ushort? FormVersionAbstract => null;
}

internal partial class Fallout3MajorRecordBinaryOverlay
{
    protected override ushort? FormVersionAbstract => null;

    public Fallout3MajorRecord.Fallout3MajorRecordFlag Fallout3MajorRecordFlags
    {
        get => (Fallout3MajorRecord.Fallout3MajorRecordFlag)this.MajorRecordFlagsRaw;
    }
}