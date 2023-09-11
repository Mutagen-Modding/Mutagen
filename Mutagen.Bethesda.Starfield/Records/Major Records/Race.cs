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
        else if (lastParsed.ParsedIndex < (int)Race_FieldIndex.BehaviorGraph)
        {
            return ParseBehaviorGraph(item, frame.SpawnAll());
        }
        else if (lastParsed.ParsedIndex < (int)Race_FieldIndex.HeadData)
        {
            return ParseHeadData(item, frame.SpawnAll());
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

    private static ParseResult ParseHeadData(
        IRaceInternal item,
        MutagenFrame frame)
    {
        item.HeadData = Mutagen.Bethesda.Plugins.Binary.Translations.GenderedItemBinaryTranslation.Parse<HeadData>(
            frame: frame,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            femaleRecordConverter: Race_Registration.HeadDataFemaleConverter,
            transl: HeadData.TryCreateFromBinary);
        return (int)Race_FieldIndex.HeadData;
    }

    private static ParseResult ParseBehaviorGraph(
        IRaceInternal item,
        MutagenFrame frame)
    {
        item.BehaviorGraph = Mutagen.Bethesda.Plugins.Binary.Translations.GenderedItemBinaryTranslation.Parse<Model>(
            frame: frame,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            transl: Model.TryCreateFromBinary);
        return (int)Race_FieldIndex.BehaviorGraph;
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
                link = FormKeyBinaryTranslation.Instance.Parse(content, frame.MetaData.MasterReferences);
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
    public IGenderedItemGetter<IHeadDataGetter?>? HeadData { get; private set; }

    public IGenderedItemGetter<IModelGetter?> BehaviorGraph { get; private set; } = new GenderedItem<IModelGetter?>(null, null);

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
        if (lastParsed.ParsedIndex < (int)Race_FieldIndex.SkeletalModel)
        {
            SkeletalModel = GenderedItemBinaryOverlay.FactorySkipMarkersPreRead<ISkeletalModelGetter>(
                package: _package,
                male: RecordTypes.MNAM,
                female: RecordTypes.FNAM,
                stream: stream,
                creator: SkeletalModelBinaryOverlay.SkeletalModelFactory);
            return (int)Race_FieldIndex.SkeletalModel;
        }
        else if (lastParsed.ParsedIndex < (int)Race_FieldIndex.BehaviorGraph)
        {
            BehaviorGraph = GenderedItemBinaryOverlay.FactorySkipMarkersPreRead<IModelGetter>(
                package: _package,
                male: RecordTypes.MNAM,
                female: RecordTypes.FNAM,
                stream: stream,
                creator: static (s, p, r) => ModelBinaryOverlay.ModelFactory(s, p, r));
            return (int)Race_FieldIndex.BehaviorGraph;
        }
        else
        {
            HeadData = GenderedItemBinaryOverlay.FactorySkipMarkersPreRead<IHeadDataGetter>(
                package: _package,
                male: RecordTypes.MNAM,
                female: RecordTypes.FNAM,
                stream: stream,
                femaleRecordConverter: Race_Registration.HeadDataFemaleConverter,
                creator: HeadDataBinaryOverlay.HeadDataFactory);
            return (int)Race_FieldIndex.HeadData;
        }
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

    public static partial void WriteBinaryBehaviorGraphCustom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        GenderedItemBinaryTranslation.Write(
            writer: writer,
            item: item.BehaviorGraph,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            markerWrap: false,
            transl: (MutagenWriter subWriter, IModelGetter? subItem, TypedWriteParams conv) =>
            {
                if (subItem is {} Item)
                {
                    ((ModelBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                        item: Item,
                        writer: subWriter,
                        translationParams: conv);
                }
            });
    }
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

    public static partial void WriteBinaryHeadDataCustom(
        MutagenWriter writer,
        IRaceGetter item)
    {
        GenderedItemBinaryTranslation.Write(
            writer: writer,
            item: item.HeadData,
            maleMarker: RecordTypes.MNAM,
            femaleMarker: RecordTypes.FNAM,
            markerWrap: false,
            transl: static (MutagenWriter subWriter, IHeadDataGetter? subItem, TypedWriteParams conv) =>
            {
                if (subItem is {} Item)
                {
                    ((HeadDataBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                        item: Item,
                        writer: subWriter,
                        translationParams: conv);
                }
            });
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
