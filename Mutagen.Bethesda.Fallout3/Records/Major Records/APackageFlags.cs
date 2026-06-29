using System.Buffers.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class APackageFlags
{
    // The polymorphic "Type Specific Flags" of PKDT. The (hidden) package Type byte at content
    // offset 4 picks the subclass; the 2-byte flag value at offset 8 is present only in 12-byte PKDT.
    public static APackageFlags Create(ReadOnlyMemorySlice<byte> content)
    {
        var item = Construct((Package.Types)content[4]);
        if (content.Length >= 12)
        {
            SetFlags(item, BinaryPrimitives.ReadUInt16LittleEndian(content.Slice(8)));
        }
        return item;
    }

    public static APackageFlags Construct(Package.Types type) => type switch
    {
        Package.Types.Find => new PackageFind(),
        Package.Types.Follow => new PackageFollow(),
        Package.Types.Escort => new PackageEscort(),
        Package.Types.Eat => new PackageEat(),
        Package.Types.Sleep => new PackageSleep(),
        Package.Types.Wander => new PackageWander(),
        Package.Types.Travel => new PackageTravel(),
        Package.Types.Accompany => new PackageAccompany(),
        Package.Types.UseItemAt => new PackageUseItemAt(),
        Package.Types.Ambush => new PackageAmbush(),
        Package.Types.FleeNotCombat => new PackageFleeNotCombat(),
        Package.Types.Sandbox => new PackageSandbox(),
        Package.Types.Patrol => new PackagePatrol(),
        Package.Types.Guard => new PackageGuard(),
        Package.Types.Dialogue => new PackageDialogue(),
        Package.Types.UseWeapon => new PackageUseWeapon(),
        _ => throw new Mutagen.Bethesda.Plugins.Exceptions.MalformedDataException($"Unknown package type: {(byte)type}"),
    };

    public static Package.Types TypeOf(IAPackageFlagsGetter item) => item switch
    {
        IPackageFindGetter => Package.Types.Find,
        IPackageFollowGetter => Package.Types.Follow,
        IPackageEscortGetter => Package.Types.Escort,
        IPackageEatGetter => Package.Types.Eat,
        IPackageSleepGetter => Package.Types.Sleep,
        IPackageWanderGetter => Package.Types.Wander,
        IPackageTravelGetter => Package.Types.Travel,
        IPackageAccompanyGetter => Package.Types.Accompany,
        IPackageUseItemAtGetter => Package.Types.UseItemAt,
        IPackageAmbushGetter => Package.Types.Ambush,
        IPackageFleeNotCombatGetter => Package.Types.FleeNotCombat,
        IPackageSandboxGetter => Package.Types.Sandbox,
        IPackagePatrolGetter => Package.Types.Patrol,
        IPackageGuardGetter => Package.Types.Guard,
        IPackageDialogueGetter => Package.Types.Dialogue,
        IPackageUseWeaponGetter => Package.Types.UseWeapon,
        _ => throw new NotImplementedException(),
    };

    private static void SetFlags(APackageFlags item, ushort raw)
    {
        switch (item)
        {
            case PackageFind f: f.Flags = (Package.ServiceFlag)raw; break;
            case PackageEscort f: f.Flags = (Package.ServiceFlag)raw; break;
            case PackageEat f: f.Flags = (Package.ServiceFlag)raw; break;
            case PackageUseItemAt f: f.Flags = (Package.UseItemAtFlag)raw; break;
            case PackageWander f: f.Flags = (Package.WanderFlag)raw; break;
            case PackageSandbox f: f.Flags = (Package.WanderFlag)raw; break;
            case PackageAmbush f: f.Flags = (Package.AmbushFlag)raw; break;
            case PackageGuard f: f.Flags = (Package.GuardFlag)raw; break;
            // Marker types have no named flags in xEdit; preserve the raw bits.
            case PackageFollow f: f.UnusedFlags = raw; break;
            case PackageSleep f: f.UnusedFlags = raw; break;
            case PackageTravel f: f.UnusedFlags = raw; break;
            case PackageAccompany f: f.UnusedFlags = raw; break;
            case PackageFleeNotCombat f: f.UnusedFlags = raw; break;
            case PackagePatrol f: f.UnusedFlags = raw; break;
            case PackageDialogue f: f.UnusedFlags = raw; break;
            case PackageUseWeapon f: f.UnusedFlags = raw; break;
            default: break;
        }
    }

    public static ushort GetFlags(IAPackageFlagsGetter item) => item switch
    {
        IPackageFindGetter f => (ushort)f.Flags,
        IPackageEscortGetter f => (ushort)f.Flags,
        IPackageEatGetter f => (ushort)f.Flags,
        IPackageUseItemAtGetter f => (ushort)f.Flags,
        IPackageWanderGetter f => (ushort)f.Flags,
        IPackageSandboxGetter f => (ushort)f.Flags,
        IPackageAmbushGetter f => (ushort)f.Flags,
        IPackageGuardGetter f => (ushort)f.Flags,
        IPackageFollowGetter f => f.UnusedFlags,
        IPackageSleepGetter f => f.UnusedFlags,
        IPackageTravelGetter f => f.UnusedFlags,
        IPackageAccompanyGetter f => f.UnusedFlags,
        IPackageFleeNotCombatGetter f => f.UnusedFlags,
        IPackagePatrolGetter f => f.UnusedFlags,
        IPackageDialogueGetter f => f.UnusedFlags,
        IPackageUseWeaponGetter f => f.UnusedFlags,
        _ => 0,
    };
}
