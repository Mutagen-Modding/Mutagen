using System;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class PackageBranch
{
    [Flags]
    public enum Flag
    {
        SuccessCompletesPackage = 0x01
    }
}

public enum ProcedureType
{
    Acquire,
    Activate,
    Eat,
    Escort,
    Find,
    Flee,
    FlightGrab,
    Follow,
    FollowTo,
    ForceGreet,
    Guard,
    HoldPosition,
    Hover,
    KeepAnEyeOn,
    LockDoors,
    Orbit,
    Patrol,
    Sandbox,
    Say,
    Shout,
    Sit,
    Sleep,
    Travel,
    UnlockDoors,
    UseIdleMarker,
    UseMagic,
    UseWeapon,
    Wait,
    Wander
}

public enum BranchType
{
    Procedure,
    Random,
    Sequential,
    Simultaneous,
    Stacked
}

partial class PackageBranchBinaryCreateTranslation
{
    public static partial void FillBinaryFlagsOverrideCustom(MutagenFrame frame, IPackageBranch item, PreviousParse lastParsed)
    {
        item.FlagsOverride = PackageFlagsOverride.CreateFromBinary(frame);
        if (frame.Reader.TryGetSubrecordHeader(RecordTypes.PFO2, out var rec))
        {
            item.FlagsOverrideUnused = PackageFlagsOverride.CreateFromBinary(frame);
        }
    }
}

partial class PackageBranchBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsOverrideCustom(MutagenWriter writer, IPackageBranchGetter item)
    {
        item.FlagsOverride?.WriteToBinary(writer);
        item.FlagsOverrideUnused?.WriteToBinary(writer);
    }
}

partial class PackageBranchBinaryOverlay
{
    public IPackageFlagsOverrideGetter? FlagsOverrideUnused { get; private set; }

    private IPackageFlagsOverrideGetter? _flagsOverride;
    public partial IPackageFlagsOverrideGetter? GetFlagsOverrideCustom() => _flagsOverride;

    partial void FlagsOverrideCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _flagsOverride = PackageFlagsOverride.CreateFromBinary(new MutagenFrame(
            new MutagenInterfaceReadStream(stream, _package.MetaData)));
        if (stream.TryGetSubrecordHeader(RecordTypes.PFO2, out var rec))
        {
            FlagsOverrideUnused = PackageFlagsOverride.CreateFromBinary(new MutagenFrame(
                new MutagenInterfaceReadStream(stream, _package.MetaData)));
        }
    }
}
