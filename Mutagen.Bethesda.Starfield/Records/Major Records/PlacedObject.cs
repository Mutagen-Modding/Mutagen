using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

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
        GroundPiece = 0x0000_0010,
        LodRespectsEnableState = 0x0000_0100,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        Ground = 0x4000_0000,
        Multibound = 0x8000_0000,
    }

    /// <summary>
    /// Used when Placed Object refers to:
    /// [Activator, Static, StaticCollection, Tree]
    /// </summary>
    [Flags]
    public enum StaticMajorFlag : uint
    {
        GroundPiece = 0x0000_0010,
        LodRespectsEnableState = 0x0000_0100,
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
    /// Used when Placed Object refers to:
    /// [Container, Terminal]
    /// </summary>
    [Flags]
    public enum ContainerMajorFlag : uint
    {
        GroundPiece = 0x0000_0010,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        NoAiAcquire = 0x0200_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
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
        MinimalUseDoor = 0x0000_0004,
        HiddenFromLocalMap = 0x0000_0040,
        Inaccessible = 0x0000_0100,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
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
        GroundPiece = 0x0000_0010,
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
    /// [Ingestible, Book, Scroll, Ammunition, Armor, Ingredient, Key, MiscItem, Furniture, Weapon]
    /// </summary>
    [Flags]
    public enum ItemMajorFlag : uint
    {
        GroundPiece = 0x0000_0010,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        NoAiAcquire = 0x0200_0000,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
        Multibound = 0x8000_0000,
    }
}

partial class PlacedObjectBinaryWriteTranslation
{
    public static partial void WriteBinaryTraversalsCustom(
        MutagenWriter writer,
        IPlacedObjectGetter item)
    {
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ITraversalReferenceGetter>.Instance.Write(
            writer: writer,
            items: item.Traversals,
            recordType: RecordTypes.XTV2,
            overflowRecord: RecordTypes.XXXX,
            transl: (MutagenWriter subWriter, ITraversalReferenceGetter subItem, TypedWriteParams conv) =>
            {
                var Item = subItem;
                ((TraversalReferenceBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}

partial class PlacedObjectBinaryCreateTranslation
{
    public static partial void FillBinaryTraversalsCustom(
        MutagenFrame frame,
        IPlacedObjectInternal item,
        PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader();
        int len;
        if (lastParsed.LengthOverride.HasValue)
        {
            len = lastParsed.LengthOverride.Value;
        }
        else
        {
            len = sub.ContentLength;
        }
        item.Traversals = TraversalReferenceBinaryCreateTranslation.Parse(frame.SpawnWithLength(len));
    }
}

partial class PlacedObjectBinaryOverlay
{
    public IReadOnlyList<ITraversalReferenceGetter>? Traversals { get; private set; }
    
    partial void TraversalsCustomParse(
        OverlayStream stream,
        long finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed)
    {
        Traversals = TraversalReferenceBinaryOverlay.Factory(stream, _package, finalPos, offset, lastParsed);
    }
}