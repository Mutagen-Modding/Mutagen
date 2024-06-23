using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class Race
{
    [Flags]
    public enum Flag : ulong
    {
        Playable = 0x0000_0001,
        FaceGenHead = 0x0000_0002,
        Child = 0x0000_0004,
        TiltFrontBack = 0x0000_0008,
        TiltLeftRight = 0x0000_0010,
        NoShadow = 0x0000_0020,
        Swims = 0x0000_0040,
        Flies = 0x0000_0080,
        Walks = 0x0000_0100,
        Immobile = 0x0000_0200,
        NotPunishable = 0x0000_0400,
        NoCombatInWater = 0x0000_0800,
        NoRotatingToHeadTrack = 0x0000_1000,
        DontShowBloodSpray = 0x0000_2000,
        DontShowBloodDecal = 0x0000_4000,
        UsesHeadTrackAnims = 0x0000_8000,
        SpellsAlignWithMagicNode = 0x0001_0000,
        UseWorldRaycastsForFootIK = 0x0002_0000,
        AllowRagdollCollision = 0x0004_0000,
        RegenHpInCombat = 0x0008_0000,
        CantOpenDoors = 0x0010_0000,
        AllowPcDialog = 0x0020_0000,
        NoKnockdowns = 0x0040_0000,
        AllowPickpocket = 0x0080_0000,
        AlwaysUseProxyController = 0x0100_0000,
        DontShowWeaponBlood = 0x0200_0000,
        OverlayHeadPartList = 0x0400_0000,
        OverrideHeadPartList = 0x0800_0000,
        CanPickupItems = 0x1000_0000,
        AllowMultipleMembraneShaders = 0x2000_0000,
        CanDualWield = 0x4000_0000,
        AvoidsRoads = 0x8000_0000,
        UseAdvancedAvoidance = 0x0000_0001_0000_0000,
        NonHostile = 0x0000_0002_0000_0000,
        Floats = 0x0000_0004_0000_0000,
        HeadAxisBit0 = 0x0000_0020_0000_0000,
        HeadAxisBit1 = 0x0000_0040_0000_0000,
        CanMeleeWhenKnockedDown = 0x0000_0080_0000_0000,
        UseIdleChatterDuringCombat = 0x0000_0100_0000_0000,
        Ungendered = 0x0000_0200_0000_0000,
        CanMoveWhenKnockedDown = 0x0000_0400_0000_0000,
        UseLargeActorPathing = 0x0000_0800_0000_0000,
        UseSubsegmentedDamage = 0x0000_1000_0000_0000,
        FlightDeferKill = 0x0000_2000_0000_0000,
        FlightAllowProceduralCrashLand = 0x0000_8000_0000_0000,
        DisableWeaponCulling = 0x0001_0000_0000_0000,
        UseOptimalSpeeds = 0x0002_0000_0000_0000,
        HasFacialRig = 0x0004_0000_0000_0000,
        CanUseCrippledLimbs = 0x0008_0000_0000_0000,
        UseQuadrupedController = 0x0010_0000_0000_0000,
        LowPriorityPushable = 0x0020_0000_0000_0000,
        CannotUsePlayableItems = 0x0040_0000_0000_0000
    }
}

partial class RaceBinaryCreateTranslation
{
    public const int NumBipedObjectNames = 64;

    public static partial ParseResult FillBinaryMNAMLogicCustom(
        MutagenFrame frame,
        IRaceInternal item,
        PreviousParse lastParsed)
    {
        return ParseMnamFnam(frame, item, lastParsed);
    }

    public static partial ParseResult FillBinaryFNAMLogicCustom(
        MutagenFrame frame,
        IRaceInternal item,
        PreviousParse lastParsed)
    {
        return ParseMnamFnam(frame, item, lastParsed);
    }

    private static ParseResult ParseMnamFnam(
        MutagenFrame frame,
        IRaceInternal item,
        PreviousParse lastParsed)
    {
        if (lastParsed.ParsedIndex < (int)Race_FieldIndex.SkeletalModel)
        {
            return ParseSkeletalModel(item, frame.SpawnAll());
        }

        return null;
    }

    private static ParseResult ParseSkeletalModel(
        IRaceInternal item,
        MutagenFrame frame)
    {
        item.SkeletalModel = Mutagen.Bethesda.Plugins.Binary.Translations.GenderedItemBinaryTranslation.Parse<SkeletalModel>(
            frame: frame,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            transl: SkeletalModel.TryCreateFromBinary);
        return (int)Race_FieldIndex.SkeletalModel;
    }

    public static partial void FillBinaryBipedObjectsCustom(MutagenFrame frame, IRaceInternal item, PreviousParse lastParsed)
    {
        FillBinaryBipedObjectsDictionary(frame, item.FormVersion, item.BipedObjects);
    }

    public static void FillBinaryBipedObjectsDictionary(IMutagenReadStream frame,
        int formVersion,
        IDictionary<BipedObject, BipedObjectData> dict)
    {
        for (int i = 0; i < NumBipedObjectNames; i++)
        {
            var data = new BipedObjectData();
            dict[(BipedObject)i] = data;
            var subFrame = frame.ReadSubrecord();
            if (subFrame.RecordType != RecordTypes.NAME)
            {
                throw new ArgumentException($"Unexpected record type: {subFrame.RecordType} != {RecordTypes.NAME}");
            }

            data.Name = subFrame.AsString(frame.MetaData.Encodings.NonTranslated);
        }

        var content = frame.ReadSubrecord(RecordTypes.RBPC).Content;
        for (int i = 0; i < NumBipedObjectNames; i++)
        {
            FormLink<IActorValueInformationGetter> link;
            if (formVersion < 78)
            {
                link = frame.MetaData.RecordInfoCache!.GetNthFormKey<IActorValueInformationGetter>(BinaryPrimitives.ReadInt32LittleEndian(content));
            }
            else
            {
                link = FormKeyBinaryTranslation.Instance.Parse(content, frame.MetaData.MasterReferences.Raw);
            }

            dict[(BipedObject)i].Conditions = link;
            content = content.Slice(4);
        }
    }

    public static partial ParseResult FillBinaryNAM3Custom(
        MutagenFrame frame,
        IRaceInternal item,
        PreviousParse lastParsed)
    {
        return (int)Race_FieldIndex.AimAssistPose;
    }
}

partial class RaceBinaryOverlay
{
    public IGenderedItemGetter<ISkeletalModelGetter?>? SkeletalModel { get; private set; }

    public IReadOnlyDictionary<BipedObject, IBipedObjectDataGetter> BipedObjects { get; private set; } =
        DictionaryExt.Empty<BipedObject, IBipedObjectDataGetter>();

    private void BipedObjectsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var dict = new Dictionary<BipedObject, BipedObjectData>();
        RaceBinaryCreateTranslation.FillBinaryBipedObjectsDictionary(stream, FormVersion, dict);
        BipedObjects = dict.Covariant<BipedObject, BipedObjectData, IBipedObjectDataGetter>();
    }
    
    public partial ParseResult MNAMLogicCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        return ParseMnamFnam(stream, offset, lastParsed);
    }
    
    public partial ParseResult FNAMLogicCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        return ParseMnamFnam(stream, offset, lastParsed);
    }

    private ParseResult ParseMnamFnam(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        SkeletalModel = GenderedItemBinaryOverlay.FactorySkipMarkersPreRead<ISkeletalModelGetter>(
            package: _package,
            male: RecordTypes.MNAM,
            female: RecordTypes.FNAM,
            stream: stream,
            creator: SkeletalModelBinaryOverlay.SkeletalModelFactory);
        return (int)Race_FieldIndex.SkeletalModel;
    }

    public partial ParseResult NAM3CustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        return (int)Race_FieldIndex.AimAssistPose;
    }
}

partial class RaceBinaryWriteTranslation
{
    public static partial void WriteBinaryBipedObjectsCustom(MutagenWriter writer, IRaceGetter item)
    {
        var bipedObjs = item.BipedObjects;
        for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
        {
            using var name = HeaderExport.Subrecord(writer, RecordTypes.NAME);
            StringBinaryTranslation.Instance.Write(writer, bipedObjs[(BipedObject)i].Name,
                StringBinaryType.NullTerminate);
        }

        using var rbpc = HeaderExport.Subrecord(writer, RecordTypes.RBPC);
        if (item.FormVersion < 78)
        {
            var dict = new Dictionary<FormKey, int>();
            for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
            {
                dict[writer.MetaData.RecordInfoCache!.GetNthFormKey<IActorValueInformationGetter>(i)] = i;
            }

            for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
            {
                var cond = bipedObjs[(BipedObject)i].Conditions;
                writer.Write(dict[cond.FormKey]);
            }
        }
        else
        {
            for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
            {
                FormLinkBinaryTranslation.Instance.Write(writer, bipedObjs[(BipedObject)i].Conditions);
            }
        }
    }

    public static partial void WriteBinaryMNAMLogicCustom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        // Nothing to do
    }

    public static partial void WriteBinaryFNAMLogicCustom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        // Nothing to do
    }

    public static partial void WriteBinaryNAM3Custom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.NAM3))
        {
        }
    }

    public static partial void WriteBinarySkeletalModelCustom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        GenderedItemBinaryTranslation.Write(
            writer: writer,
            item: item.SkeletalModel,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            markerWrap: false,
            transl: static (MutagenWriter subWriter, ISkeletalModelGetter? subItem, TypedWriteParams conv) =>
            {
                if (subItem is {} Item)
                {
                    ((SkeletalModelBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                        item: Item,
                        writer: subWriter,
                        translationParams: conv);
                }
            });
    }
}
