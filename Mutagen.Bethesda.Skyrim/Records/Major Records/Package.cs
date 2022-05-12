using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class Package
{
    public enum Types
    {
        Package = 18,
        PackageTemplate = 19
    }

    [Flags]
    public enum Flag : uint
    {
        OffersServices = 0x0000_0001,
        MustComplete = 0x0000_0004,
        MaintainSpeedAtGoal = 0x0000_0008,
        UnlockDoorsAtPackageStart = 0x0000_0040,
        UnlockDoorsAtPackageEnd = 0x0000_0080,
        ContinueIfPcNear = 0x0000_0200,
        OncePerDay = 0x0000_0400,
        PreferredSpeed = 0x0000_2000,
        AlwaysSneak = 0x0002_0000,
        AllowSwimming = 0x0004_0000,
        IgnoreCombat = 0x0010_0000,
        WeaponsUnequipped = 0x0020_0000,
        WeaponDrawn = 0x0080_0000,
        NoCombatAlert = 0x0800_0000,
        WearSleepOutfit = 2000_0000
    }

    public enum Interrupt
    {
        None,
        Spectator,
        ObserveDead,
        GuardWarn,
        Combat,
    }

    public enum Speed
    {
        Walk,
        Jog,
        Run,
        FastWalk,
    }

    [Flags]
    public enum InterruptFlag
    {
        HellosToPlayer = 0x0001,
        RandomConversations = 0x0002,
        ObserveCombatBehavior = 0x0004,
        GreetCorpseBehavior = 0x0008,
        ReactionToPlayerActions = 0x0010,
        FriendlyFireComments = 0x0020,
        AggroRadiusBehavior = 0x0040,
        AllowIdleChatter = 0x0080,
        WorldInteractions = 0x0200,
    }

    public enum DayOfWeek
    {
        Any = -1,
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Weekdays = 7,
        Weekends = 8,
        MondayWednesdayFriday = 9,
        TuesdayThursday = 10,
    }

    public enum TargetDataType
    {
        SpecificReference,
        ObjectId,
        ObjectType,
        LinkedReference,
        RefAlias,
        Unknown,
        Self,
    }
}

partial class PackageBinaryCreateTranslation
{
    public const string BoolKey = "Bool";
    public const string FloatKey = "Float";
    public const string IntKey = "Int";
    public const string LocationKey = "Location";
    public const string SingleRefKey = "SingleRef";
    public const string TargetSelectorKey = "TargetSelector";
    public const string TopicKey = "Topic";
    public const string ObjectListKey = "ObjectList";

    public static partial void FillBinaryPackageTemplateCustom(MutagenFrame frame, IPackageInternal item)
    {
        var pkcuRecord = frame.ReadSubrecord();
        if (pkcuRecord.Content.Length != 12)
        {
            throw new ArgumentException();
        }
        var dataCount = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(pkcuRecord.Content));
        item.PackageTemplate.FormKey = FormKeyBinaryTranslation.Instance.Parse(pkcuRecord.Content.Slice(4, 4), frame.MetaData.MasterReferences!);
        item.DataInputVersion = BinaryPrimitives.ReadInt32LittleEndian(pkcuRecord.Content.Slice(8));

        FillPackageData(frame.Reader, dataCount, item.Data);
    }

    public static void FillPackageData(IMutagenReadStream stream, int expectedCount, IDictionary<sbyte, APackageData> data)
    {
        // Retrieve the expected types, then skip rest of data
        var valuesPosition = stream.Position;
        var types = new List<string>(expectedCount);
        while (stream.TryReadSubrecord(out var subRecord))
        {
            switch (subRecord.RecordTypeInt)
            {
                case RecordTypeInts.ANAM:
                    types.Add(BinaryStringUtility.ProcessWholeToZString(subRecord.Content, stream.MetaData.Encodings.NonTranslated));
                    break;
                case RecordTypeInts.CNAM:
                case RecordTypeInts.BNAM:
                case RecordTypeInts.TPIC:
                case RecordTypeInts.PTDA:
                case RecordTypeInts.PDTO:
                case 0x54444C50: // PLDT
                    // Skip
                    break;
                default:
                    stream.Position -= subRecord.TotalLength;
                    goto done_types;
            }
        }
        done_types:

        // Parse package data
        APackageData? lastPackage = null;
        int itemIndex = 0;
        var packages = new List<APackageData>(expectedCount);
        while (stream.TryGetSubrecord(out var subRecord))
        {
            switch (subRecord.RecordTypeInt)
            {
                case RecordTypeInts.UNAM:
                    var index = (sbyte)subRecord.Content[0];
                    lastPackage = data.GetOrAdd<sbyte, APackageData>(
                        index,
                        () =>
                        {
                            return (types[itemIndex++]) switch
                            {
                                IntKey => new PackageDataInt(),
                                FloatKey => new PackageDataFloat(),
                                BoolKey => new PackageDataBool(),
                                LocationKey => new PackageDataLocation(),
                                SingleRefKey => new
                                    PackageDataTarget()
                                    {
                                        Type = PackageDataTarget.Types.SingleRef
                                    },
                                TargetSelectorKey => new PackageDataTarget()
                                {
                                    Type = PackageDataTarget.Types.Target
                                },
                                TopicKey => new PackageDataTopic(),
                                ObjectListKey => new PackageDataObjectList(),
                                _ => throw new NotImplementedException(),
                            };
                        });
                    packages.Add(lastPackage);
                    break;
                case RecordTypeInts.BNAM:
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package name came before index");
                    }
                    lastPackage.Name = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, stream.MetaData.Encodings.NonTranslated);
                    break;
                case RecordTypeInts.PNAM:
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package flags came before index");
                    }
                    lastPackage.Flags = (APackageData.Flag)BinaryPrimitives.ReadInt32LittleEndian(subRecord.Content);
                    break;
                default:
                    goto done_indexes;
            }
            stream.Position += subRecord.TotalLength;
        }
        done_indexes:
        if (expectedCount != data.Count)
        {
            throw new ArgumentException("Unexpected data count mismatch");
        }

        // Mark end for later
        var end = stream.Position;

        // Pase package data input values
        stream.Position = valuesPosition;
        itemIndex = -1;
        lastPackage = null;
        while (itemIndex < expectedCount)
        {
            if (!stream.TryGetSubrecord(out var subRecord)) break;
            switch (subRecord.RecordTypeInt)
            {
                case RecordTypeInts.ANAM:
                    lastPackage = packages[++itemIndex];
                    stream.Position += subRecord.TotalLength;
                    break;
                case RecordTypeInts.CNAM:
                    switch (lastPackage)
                    {
                        case PackageDataBool b:
                            b.Data = subRecord.Content[0] > 0;
                            break;
                        case PackageDataFloat f:
                            f.Data = subRecord.Content.Float();
                            break;
                        case PackageDataObjectList f:
                            f.Data = subRecord.Content.Float();
                            break;
                        case PackageDataInt i:
                            i.Data = BinaryPrimitives.ReadUInt32LittleEndian(subRecord.Content);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    stream.Position += subRecord.TotalLength;
                    break;
                case RecordTypeInts.TPIC:
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package data came before index");
                    }
                    if (lastPackage is PackageDataTopic tpic)
                    {
                        tpic.TPIC = subRecord.Content.ToArray();
                    }
                    else
                    {
                        throw new ArgumentException("Encountered TPIC for a non-target data package");
                    }
                    stream.Position += subRecord.TotalLength;
                    break;
                case RecordTypeInts.PTDA:
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package data came before index");
                    }
                    if (lastPackage is PackageDataTarget target)
                    {
                        stream.Position += subRecord.HeaderLength;
                        target.Target = APackageTarget.CreateFromBinary(new MutagenFrame(stream));
                    }
                    else
                    {
                        throw new ArgumentException("Encountered target for a non-target data package");
                    }
                    break;
                case RecordTypeInts.PDTO:
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package data came before index");
                    }
                    if (lastPackage is PackageDataTopic topic)
                    {
                        topic.Topics.SetTo(ATopicReferenceBinaryCreateTranslation.Factory(new MutagenFrame(stream)));
                    }
                    else
                    {
                        throw new ArgumentException("Encountered target for a non-target data package");
                    }
                    break;
                case 0x54444C50: // PLDT
                    if (lastPackage == null)
                    {
                        throw new ArgumentException("Package data came before index");
                    }
                    if (lastPackage is PackageDataLocation loc)
                    {
                        stream.Position += subRecord.HeaderLength;
                        loc.Location = Mutagen.Bethesda.Skyrim.LocationTargetRadius.CreateFromBinary(new MutagenFrame(stream));
                    }
                    else
                    {
                        throw new ArgumentException("Encountered location for a non-location data package");
                    }
                    break;
                default:
                    goto done;
            }
        }

        done:
        // Go back to end to continue
        stream.Position = end;
    }

    public static partial void FillBinaryXnamMarkerCustom(MutagenFrame frame, IPackageInternal item)
    {
        // Skip marker
        item.XnamMarker = frame.ReadSubrecord().Content.ToArray();
        item.ProcedureTree.SetTo(
            ListBinaryTranslation<PackageBranch>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: RecordTypes.ANAM,
                transl: (MutagenFrame r, [MaybeNullWhen(false)] out PackageBranch listSubItem, TypedParseParams? translationParams) =>
                {
                    listSubItem = PackageBranch.CreateFromBinary(r);
                    return true;
                }));
        AbsorbPackageData(frame.Reader, item.Data);
    }

    public static void AbsorbPackageData(IMutagenReadStream stream, IDictionary<sbyte, APackageData> dict)
    {
        APackageData? lastPackage = null;
        while (stream.TryGetSubrecord(out var subRecord))
        {
            switch (subRecord.RecordTypeInt)
            {
                case RecordTypeInts.UNAM:
                    lastPackage = dict.GetOrAdd((sbyte)subRecord.Content[0]);
                    break;
                case RecordTypeInts.BNAM:
                    lastPackage!.Name = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, stream.MetaData.Encodings.NonTranslated);
                    break;
                case RecordTypeInts.PNAM:
                    lastPackage!.Flags = (APackageData.Flag)BinaryPrimitives.ReadInt32LittleEndian(subRecord.Content);
                    break;
                default:
                    return;
            }
            stream.Position += subRecord.TotalLength;
        }
    }
}

partial class PackageBinaryWriteTranslation
{
    public static readonly RecordType PLDT = new("PLDT");

    public static partial void WriteBinaryPackageTemplateCustom(MutagenWriter writer, IPackageGetter item)
    {
        var data = item.Data;
        long jumpbackPos;
        using (HeaderExport.Subrecord(writer, RecordTypes.PKCU))
        {
            jumpbackPos = writer.Position;
            writer.Write(data.Count);
            FormKeyBinaryTranslation.Instance.Write(writer, item.PackageTemplate.FormKey);
            writer.Write(item.DataInputVersion);
        }

        HashSet<sbyte> addedKeys = new HashSet<sbyte>();
        foreach (var kv in data.OrderBy(k => k.Key))
        {
            var val = kv.Value;
            var typeStr = val switch
            {
                PackageDataBool => PackageBinaryCreateTranslation.BoolKey,
                PackageDataInt => PackageBinaryCreateTranslation.IntKey,
                PackageDataTarget { Type: PackageDataTarget.Types.Target } => PackageBinaryCreateTranslation.TargetSelectorKey,
                PackageDataTarget { Type: PackageDataTarget.Types.SingleRef } => PackageBinaryCreateTranslation.SingleRefKey,
                PackageDataFloat => PackageBinaryCreateTranslation.FloatKey,
                PackageDataObjectList => PackageBinaryCreateTranslation.ObjectListKey,
                PackageDataTopic => PackageBinaryCreateTranslation.TopicKey,
                PackageDataLocation => PackageBinaryCreateTranslation.LocationKey,
                _ => null,
            };
            if (typeStr == null) continue;
            addedKeys.Add(kv.Key);
            using (HeaderExport.Subrecord(writer, RecordTypes.ANAM))
            {
                writer.Write(typeStr, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
            }
            switch (val)
            {
                case PackageDataBool b:
                    using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
                    {
                        writer.Write((byte)(b.Data ? 1 : 0));
                    }
                    break;
                case PackageDataInt i:
                    using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
                    {
                        writer.Write(i.Data);
                    }
                    break;
                case PackageDataFloat f:
                    using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
                    {
                        writer.Write(f.Data);
                    }
                    break;
                case PackageDataObjectList list:
                    if (list.Data != null)
                    {
                        using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
                        {
                            writer.Write(list.Data);
                        }
                    }
                    break;
                case PackageDataTopic topic:
                    ATopicReferenceBinaryWriteTranslation.Write(writer, topic.Topics);
                    if (topic.TPIC is {} tpic)
                    {
                        using (HeaderExport.Subrecord(writer, RecordTypes.TPIC))
                        {
                            writer.Write(tpic);
                        }
                    }
                    break;
                case PackageDataTarget target:
                    using (HeaderExport.Subrecord(writer, RecordTypes.PTDA))
                    {
                        APackageTargetBinaryWriteTranslation.WriteCustom(writer, target.Target);
                    }
                    break;
                case PackageDataLocation loc:
                    using (HeaderExport.Subrecord(writer, PLDT))
                    {
                        loc.Location.WriteToBinary(writer);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        var jumpForwardPos = writer.Position;
        writer.Position = jumpbackPos;
        writer.Write(addedKeys.Count);
        writer.Position = jumpForwardPos;

        foreach (var k in addedKeys.OrderBy(k => k))
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.UNAM))
            {
                writer.Write(k);
            }
        }
    }

    public static partial void WriteBinaryXnamMarkerCustom(MutagenWriter writer, IPackageGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.XNAM)) 
        {
            writer.Write(item.XnamMarker);
        }

        foreach (var branch in item.ProcedureTree)
        {
            branch.WriteToBinary(writer);
        }

        var data = item.Data;
        foreach (var val in data.OrderBy(k => k.Key))
        {
            var name = val.Value.Name;
            var flags = val.Value.Flags;
            if (name == null && flags == null) continue;
            using (HeaderExport.Subrecord(writer, RecordTypes.UNAM))
            {
                writer.Write(val.Key);
            }
            if (name != null)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.BNAM))
                {
                    writer.Write(name, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                }
            }
            if (flags != null)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.PNAM))
                {
                    writer.Write((int)flags);
                }
            }
        }
    }
}

partial class PackageBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

    public IReadOnlyList<IPackageBranchGetter> ProcedureTree { get; private set; } = ListExt.Empty<IPackageBranchGetter>();

    private readonly Dictionary<sbyte, APackageData> _packageData = new Dictionary<sbyte, APackageData>();
    public IReadOnlyDictionary<sbyte, IAPackageDataGetter> Data => _packageData.Covariant<sbyte, APackageData, IAPackageDataGetter>();

    public int DataInputVersion { get; private set; }

    ReadOnlyMemorySlice<Byte> _xnam;
    public partial ReadOnlyMemorySlice<Byte> GetXnamMarkerCustom() => _xnam;

    FormLink<IPackageGetter> _packageTemplate = null!;
    public partial IFormLinkGetter<IPackageGetter> GetPackageTemplateCustom() => _packageTemplate;

    partial void PackageTemplateCustomParse(OverlayStream stream, long finalPos, int offset)
    {
    }

    private void PackageTemplateCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        var pkcu = stream.ReadSubrecord();
        var count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(pkcu.Content));
        _packageTemplate = FormKeyBinaryTranslation.Instance.Parse(pkcu.Content.Slice(4), _package.MetaData.MasterReferences!);
        DataInputVersion = BinaryPrimitives.ReadInt32LittleEndian(pkcu.Content.Slice(8));

        PackageBinaryCreateTranslation.FillPackageData(
            new MutagenInterfaceReadStream(stream, _package.MetaData), count, _packageData);
    }

    partial void XnamMarkerCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        var xnam = stream.ReadSubrecord();
        _xnam = xnam.Content;
        this.ProcedureTree = this.ParseRepeatedTypelessSubrecord<PackageBranchBinaryOverlay>(
            stream: stream,
            parseParams: null,
            trigger: RecordTypes.ANAM,
            factory: PackageBranchBinaryOverlay.PackageBranchFactory);
        PackageBinaryCreateTranslation.AbsorbPackageData(
            new MutagenInterfaceReadStream(stream, _package.MetaData), _packageData);
    }

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }
}