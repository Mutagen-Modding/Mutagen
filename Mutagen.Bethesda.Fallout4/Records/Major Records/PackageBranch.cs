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

public static class ProcedureType
{
    public static string Acquire => "Acquire";
    public static string Activate => "Activate";
    public static string Eat => "Eat";
    public static string Escort => "Escort";
    public static string Find => "Find";
    public static string Flee => "Flee";
    public static string FlightGrab => "FlightGrab";
    public static string Follow => "Follow";
    public static string FollowTo => "FollowTo";
    public static string ForceGreet => "ForceGreet";
    public static string Guard => "Guard";
    public static string HoldPosition => "HoldPosition";
    public static string Hover => "Hover";
    public static string KeepAnEyeOn => "KeepAnEyeOn";
    public static string LockDoors => "LockDoors";
    public static string Orbit => "Orbit";
    public static string Patrol => "Patrol";
    public static string Sandbox => "Sandbox";
    public static string Say => "Say";
    public static string Shout => "Shout";
    public static string Sit => "Sit";
    public static string Sleep => "Sleep";
    public static string Travel => "Travel";
    public static string UnlockDoors => "UnlockDoors";
    public static string UseIdleMarker => "UseIdleMarker";
    public static string UseMagic => "UseMagic";
    public static string UseWeapon => "UseWeapon";
    public static string Wait => "Wait";
    public static string Wander => "Wander";
}

public static class BranchType
{
    public static string Procedure => "Procedure";
    public static string Random => "Random";
    public static string Sequential => "Sequential";
    public static string Simultaneous => "Simultaneous";
    public static string Stacked => "Stacked";
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
