using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class PlacedObject
{
    public enum ActionFlag
    {
        UseDefault,
        Activate,
        Open,
        OpenByDefault,
    }

    [Flags]
    public enum DefaultMajorFlag : uint
    {
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        Ground = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to:
    /// [Activator, Static, Tree]
    /// </summary>
    [Flags]
    public enum StaticMajorFlag : uint
    {
        HiddenFromLocalMap = 0x0000_0200,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        VisibleWhenDistant = 0x0000_8000,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to: [Container]
    /// </summary>
    [Flags]
    public enum ContainerMajorFlag : uint
    {
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        VisibleWhenDistant = 0x0000_8000,
        IsFullLod = 0x0001_0000,
        NoAiAcquire = 0x0200_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        Ground = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to: [Door]
    /// </summary>
    [Flags]
    public enum DoorMajorFlag : uint
    {
        HiddenFromLocalMap = 0x0000_0040,
        Inaccessible = 0x0000_0100,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to: [Light]
    /// </summary>
    [Flags]
    public enum LightMajorFlag : uint
    {
        DoesNotLightWater = 0x0000_0100,
        CastsShadows = 0x0000_0200,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        NeverFades = 0x0001_0000,
        DoesNotLightLandscape = 0x0002_0000,
        NoAiAcquire = 0x0200_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to: [MoveableStatic]
    /// </summary>
    [Flags]
    public enum MoveableStaticMajorFlag : uint
    {
        MotionBlur = 0x0000_0200,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to: [AddonNode]
    /// </summary>
    [Flags]
    public enum AddonNodeMajorFlag : uint
    {
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to:
    /// [Ingestible, Scroll, Ammunition, Ingredient, Key, MiscItem, SoulGem, Weapon]
    /// </summary>
    [Flags]
    public enum ItemMajorFlag : uint
    {
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        VisibleWhenDistant = 0x0000_8000,
        IsFullLod = 0x0001_0000,
        NoAiAcquire = 0x0200_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IPlacementGetter? IPlacedGetter.Placement => this.Placement;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnableParentGetter? IPlacedGetter.EnableParent => this.EnableParent;
}

partial class PlacedObjectBinaryCreateTranslation
{
    public const byte HasImageSpaceFlag = 0x40;
    public const byte HasLightingTemplateFlag = 0x80;

    public static partial ParseResult FillBinaryBoundDataCustom(MutagenFrame frame, IPlacedObjectInternal item, PreviousParse lastParsed)
    {
        var header = frame.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }
        item.Unknown = BinaryPrimitives.ReadInt16LittleEndian(header.Content.Slice(2));
        while (frame.Reader.TryReadSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.LNAM:
                    item.LightingTemplate.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                    break;
                case RecordTypeInts.INAM:
                    item.ImageSpace.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                    break;
                case RecordTypeInts.XLRM:
                    item.LinkedRooms.Add(new FormLink<PlacedObject>(FormKeyBinaryTranslation.Instance.Parse(frame)));
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
        var lightingTemplate = item.LightingTemplate;
        var imageSpace = item.ImageSpace;
        var linkedRooms = item.LinkedRooms;
        var unknown2 = item.Unknown;
        if (lightingTemplate.FormKeyNullable == null
            && imageSpace.FormKeyNullable == null
            && linkedRooms.Count == 0
            && unknown2 == 0)
        {
            return;
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.XRMR))
        {
            writer.Write((byte)item.LinkedRooms.Count);
            byte flags = 0;
            if (lightingTemplate.FormKeyNullable != null)
            {
                flags = Enums.SetFlag(flags, PlacedObjectBinaryCreateTranslation.HasLightingTemplateFlag, true);
            }
            if (imageSpace.FormKeyNullable != null)
            {
                flags = Enums.SetFlag(flags, PlacedObjectBinaryCreateTranslation.HasImageSpaceFlag, true);
            }
            writer.Write(flags);
            writer.Write(unknown2);
        }
        if (lightingTemplate.FormKeyNullable != null)
        {
            FormKeyBinaryTranslation.Instance.Write(writer, lightingTemplate.FormKeyNullable.Value, RecordTypes.LNAM);
        }
        if (imageSpace.FormKeyNullable != null)
        {
            FormKeyBinaryTranslation.Instance.Write(writer, imageSpace.FormKeyNullable.Value, RecordTypes.INAM);
        }
        foreach (var room in linkedRooms)
        {
            FormKeyBinaryTranslation.Instance.Write(writer, room.FormKey, RecordTypes.XLRM);
        }
    }
}
    
partial class PlacedObjectBinaryOverlay
{
    int? _boundDataLoc;

    public short Unknown => _boundDataLoc.HasValue ? BinaryPrimitives.ReadInt16LittleEndian(_recordData.Slice(_boundDataLoc.Value + 8)) : default(short);

    public IReadOnlyList<IFormLinkGetter<IPlacedObjectGetter>> LinkedRooms { get; private set; } = Array.Empty<IFormLinkGetter<IPlacedObjectGetter>>();

    int? _lightingTemplateLoc;
    public IFormLinkNullableGetter<ILightingTemplateGetter> LightingTemplate => _lightingTemplateLoc.HasValue ? new FormLinkNullable<ILightingTemplateGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, _lightingTemplateLoc.Value, _package.MetaData.Constants)))) : FormLinkNullable<ILightingTemplateGetter>.Null;

    int? _imageSpaceLoc;
    public IFormLinkNullableGetter<IImageSpaceGetter> ImageSpace => _imageSpaceLoc.HasValue ? new FormLinkNullable<IImageSpaceGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, _imageSpaceLoc.Value, _package.MetaData.Constants)))) : FormLinkNullable<IImageSpaceGetter>.Null;

    public partial ParseResult BoundDataCustomParse(OverlayStream stream, int offset)
    {
        _boundDataLoc = stream.Position - offset;
        var header = stream.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }
        var roomCount = header.Content[0];
        var flags = header.Content[1];
        while (stream.TryGetSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.LNAM:
                    _lightingTemplateLoc = stream.Position - offset;
                    stream.Position += subHeader.TotalLength;
                    break;
                case RecordTypeInts.INAM:
                    _imageSpaceLoc = stream.Position - offset;
                    stream.Position += subHeader.TotalLength;
                    break;
                case RecordTypeInts.XLRM:
                    LinkedRooms = BinaryOverlayList.FactoryByArray<IFormLinkGetter<IPlacedObjectGetter>>(
                        stream.RemainingMemory,
                        _package,
                        (s, p) => new FormLink<IPlacedObjectGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))),
                        locs: ParseRecordLocations(
                            stream: stream,
                            constants: _package.MetaData.Constants.SubConstants,
                            trigger: subHeader.RecordType,
                            skipHeader: true));
                    break;
                default:
                    return null;
            }
        }

        return null;
    }
}
