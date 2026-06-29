using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class PlacedObject
{
    [Flags]
    public enum MajorFlag : uint
    {
        HiddenFromLocalMap = 0x0000_0040,
        TurnOffFire = 0x0000_0080,
        Inaccessible = 0x0000_0100,
        CastsShadowsOrMotionBlur = 0x0000_0200,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        VisibleWhenDistant = 0x0000_8000,
        HighPriorityLOD = 0x0001_0000,
        NoAIAcquire = 0x0200_0000,
        NavMeshFilter = 0x0400_0000,
        NavMeshBoundingBox = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        RefractedByAutoWater = 0x2000_0000,
        NavMeshGround = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    public enum CollisionLayerEnum : uint
    {
        Unidentified = 0,
        Static = 1,
        AnimStatic = 2,
        Transparent = 3,
        Clutter = 4,
        Weapon = 5,
        Projectile = 6,
        Spell = 7,
        Biped = 8,
        Trees = 9,
        Props = 10,
        Water = 11,
        Trigger = 12,
        Terrain = 13,
        Trap = 14,
        NonCollidable = 15,
        CloudTrap = 16,
        Ground = 17,
        Portal = 18,
        DebrisSmall = 19,
        DebrisLarge = 20,
        AcousticSpace = 21,
        ActorZone = 22,
        ProjectileZone = 23,
        GasTrap = 24,
        ShellCasing = 25,
        TransparentSmall = 26,
        InvisibleWall = 27,
        TransparentSmallAnim = 28,
        DeadBip = 29,
        CharController = 30,
        AvoidBox = 31,
        CollisionBox = 32,
        CameraSphere = 33,
        DoorDetection = 34,
        CameraPick = 35,
        ItemPick = 36,
        LineOfSight = 37,
        PathPick = 38,
        CustomPick1 = 39,
        CustomPick2 = 40,
        SpellExplosion = 41,
        DroppingPick = 42,
    }

    [Flags]
    public enum SpecialRenderingFlag : uint
    {
        Imposter = 0x2,
        UseFullShaderInLOD = 0x4,
    }

    [Flags]
    public enum ActionFlagEnum : uint
    {
        UseDefault = 0x1,
        Activate = 0x2,
        Open = 0x4,
        OpenByDefault = 0x8,
    }
}

partial class PlacedObjectBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryBoundDataCustom(MutagenFrame frame, IPlacedObjectInternal item, PreviousParse lastParsed)
    {
        var header = frame.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }
        // First 2 bytes = linked rooms count, next 2 bytes = unknown
        item.Unknown = BinaryPrimitives.ReadInt16LittleEndian(header.Content.Slice(2));

        // Read XLRM linked rooms
        while (frame.Reader.TryReadSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.XLRM:
                    item.LinkedRooms.Add(new FormLink<IPlacedObjectGetter>(FormKeyBinaryTranslation.Instance.Parse(frame)));
                    break;
                default:
                    frame.Reader.Position -= subHeader.HeaderLength;
                    return null;
            }
        }
        return null;
    }
}

partial class PlacedObjectBinaryWriteTranslation
{
    public static partial void WriteBinaryBoundDataCustom(MutagenWriter writer, IPlacedObjectGetter item)
    {
        var linkedRooms = item.LinkedRooms;
        var unknown = item.Unknown;
        if (linkedRooms.Count == 0 && unknown == 0)
        {
            return;
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.XRMR))
        {
            writer.Write((short)item.LinkedRooms.Count);
            writer.Write(item.Unknown);
        }
        foreach (var room in linkedRooms)
        {
            FormLinkBinaryTranslation.Instance.Write(writer, room, RecordTypes.XLRM);
        }
    }
}

partial class PlacedObjectBinaryOverlay
{
    int? _boundDataLoc;

    public short Unknown => _boundDataLoc.HasValue ? BinaryPrimitives.ReadInt16LittleEndian(_recordData.Slice(_boundDataLoc.Value + 8)) : default(short);

    public IReadOnlyList<IFormLinkGetter<IPlacedObjectGetter>> LinkedRooms { get; private set; } = Array.Empty<IFormLinkGetter<IPlacedObjectGetter>>();

    public partial ParseResult BoundDataCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _boundDataLoc = stream.Position - offset;
        var header = stream.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }

        // Read XLRM linked rooms
        while (stream.TryGetSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.XLRM:
                    LinkedRooms = BinaryOverlayList.FactoryByArray<IFormLinkGetter<IPlacedObjectGetter>>(
                        stream.RemainingMemory,
                        _package,
                        (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IPlacedObjectGetter>(p, s),
                        locs: ParseRecordLocations(
                            stream: stream,
                            trigger: RecordTypes.XLRM,
                            constants: _package.MetaData.Constants.SubConstants,
                            skipHeader: true));
                    return null;
                default:
                    return null;
            }
        }
        return null;
    }
}
