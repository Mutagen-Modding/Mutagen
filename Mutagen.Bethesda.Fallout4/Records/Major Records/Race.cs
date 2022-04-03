using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Noggog;
using System.Linq;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Race
    {
        internal static readonly RecordType NAM2 = new("NAM2");
        public bool ExportingExtraNam2 { get; set; }

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
            Unknown3 = 0x0000_0008_0000_0000,
            Unknown4 = 0x0000_0010_0000_0000,
            HeadAxisBit0 = 0x0000_0020_0000_0000,
            HeadAxisBit1 = 0x0000_0040_0000_0000,
            CanMeleeWhenKnockedDown = 0x0000_0080_0000_0000,
            UseIdleChatterDuringCombat = 0x0000_0100_0000_0000,
            Ungendered = 0x0000_0200_0000_0000,
            CanMoveWhenKnockedDown = 0x0000_0400_0000_0000,
            UseLargeActorPathing = 0x0000_0800_0000_0000,
            UseSubsegmentedDamage = 0x0000_1000_0000_0000,
            FlightDeferKill = 0x0000_2000_0000_0000,
            Unknown14 = 0x0000_4000_0000_0000,
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

    public partial interface IRace
    {
        new bool ExportingExtraNam2 { get; set; }
    }

    public partial interface IRaceGetter
    {
        bool ExportingExtraNam2 { get; }
    }

    namespace Internals
    {
        public partial class RaceBinaryCreateTranslation
        {
            public const int NumBipedObjectNames = 32;

            public static partial void FillBinaryExtraNAM2Custom(MutagenFrame frame, IRaceInternal item)
            {
                if (frame.Complete) return;
                if (frame.TryGetSubrecord(Race.NAM2, out var subHeader))
                {
                    item.ExportingExtraNam2 = true;
                    frame.Position += subHeader.TotalLength;
                }
            }

            public static partial void FillBinaryBipedObjectsCustom(MutagenFrame frame, IRaceInternal item)
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
                    var subFrame = frame.ReadSubrecordFrame();
                    if (subFrame.RecordType != RecordTypes.NAME)
                    {
                        throw new ArgumentException($"Unexpected record type: {subFrame.RecordType} != {RecordTypes.NAME}");
                    }

                    data.Name = subFrame.AsString(frame.MetaData.Encodings.NonTranslated);
                }

                var content = frame.ReadSubrecordFrame(RecordTypes.RBPC).Content;
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

            public static partial ParseResult FillBinaryFaceFxPhonemesListingParsingCustom(MutagenFrame frame, IRaceInternal item) => FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, item.FaceFxPhonemes);

            public static partial ParseResult FillBinaryFaceFxPhonemesRawParsingCustom(MutagenFrame frame, IRaceInternal item) => FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, item.FaceFxPhonemes);

            public static partial void FillBinaryMorphValuesCustom(MutagenFrame frame, IRaceInternal item)
            {
                item.MorphValues.SetTo(
                    Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<MorphValue>.Instance.Parse(
                        reader: frame.SpawnAll(),
                        triggeringRecord: MorphValue_Registration.TriggerSpecs,
                        transl: MorphValue.TryCreateFromBinary));

                // Read off last index subrecord
                frame.ReadSubrecordFrame(RecordTypes.MLSI);
            }

            public static partial ParseResult FillBinaryBoneDataParseCustom(MutagenFrame frame, IRaceInternal item)
            {
                var genderFrame = frame.ReadSubrecordFrame(RecordTypes.BSMP);
                
                ExtendedList<Bone> list = Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<Bone>.Instance.Parse(
                    reader: frame.SpawnAll(),
                    triggeringRecord: Bone_Registration.TriggerSpecs,
                    transl: Bone.TryCreateFromBinary);
                if (genderFrame.AsInt32() == 0)
                {
                    item.BoneData.Male = list;
                }
                else
                {
                    item.BoneData.Female = list;
                }
                return null;
            }

            public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IRaceInternal item)
            {
                item.Flags = EnumBinaryTranslation<Race.Flag, MutagenFrame, MutagenWriter>.Instance.Parse(
                    reader: frame,
                    length: 4);
            }

            public static partial void FillBinaryFlags2Custom(MutagenFrame frame, IRaceInternal item)
            {
                // Clear out upper flags
                item.Flags &= ((Race.Flag)0x00000000FFFFFFFF);

                // Set upper flags
                ulong flags2 = frame.ReadUInt32();
                flags2 <<= 32;
                item.Flags |= ((Race.Flag)flags2);
            }
        }

        public partial class RaceBinaryOverlay
        {
            private readonly ICollectionGetter<RecordType> BoneRecordTypes = new CollectionGetterWrapper<RecordType>(
                new HashSet<RecordType>(new[]{ RecordTypes.BSMB, RecordTypes.BSMS, RecordTypes.BMMP}));

            public bool ExportingExtraNam2 { get; private set; }

            private int? _faceFxPhonemesLoc;
            public IFaceFxPhonemesGetter FaceFxPhonemes => GetFaceFx();

            public IReadOnlyDictionary<BipedObject, IBipedObjectDataGetter> BipedObjects { get; private set; } =
                DictionaryExt.Empty<BipedObject, IBipedObjectDataGetter>();

            private GenderedItem<IReadOnlyList<IBoneGetter>?>? _boneData;
            public IGenderedItemGetter<IReadOnlyList<IBoneGetter>?> BoneData => _boneData ?? new GenderedItem<IReadOnlyList<IBoneGetter>?>(null, null);

            public IReadOnlyList<IMorphValueGetter> MorphValues { get; private set; } = ListExt.Empty<MorphValueBinaryOverlay>();

            partial void ExtraNAM2CustomParse(OverlayStream stream, int offset)
            {
            }

            public partial ParseResult BoneDataParseCustomParse(OverlayStream stream, int offset)
            {
                var genderFrame = stream.ReadSubrecordFrame(RecordTypes.BSMP);
                _boneData ??= new GenderedItem<IReadOnlyList<IBoneGetter>?>(null, null);
                IReadOnlyList<IBoneGetter> list = this.ParseRepeatedTypelessSubrecord(
                    stream: stream,
                    parseParams: null,
                    trigger: Bone_Registration.TriggerSpecs,
                    factory: BoneBinaryOverlay.BoneFactory);
                if (genderFrame.AsInt32() == 0)
                {
                    _boneData.Male = list;
                }
                else
                {
                    _boneData.Female = list;
                }
                return null;
            }

            public partial Race.Flag GetFlagsCustom()
            {
                if (!_DATALocation.HasValue) return default;
                var flag = (Race.Flag)BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(_FlagsLocation, 4));

                // Clear out upper flags
                flag &= ((Race.Flag)0x00000000FFFFFFFF);

                // Set upper flags
                ulong flags2 = BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(_Flags2Location, 4));
                flags2 <<= 32;
                flag |= ((Race.Flag)flags2);
                return flag;
            }

            private void BipedObjectsCustomParse(OverlayStream stream, int finalPos, int offset)
            {
                var dict = new Dictionary<BipedObject, BipedObjectData>();
                RaceBinaryCreateTranslation.FillBinaryBipedObjectsDictionary(stream, FormVersion, dict);
                BipedObjects = dict.Covariant<BipedObject, BipedObjectData, IBipedObjectDataGetter>();
            }

            public partial ParseResult FaceFxPhonemesListingParsingCustomParse(OverlayStream stream, int offset)
            {
                FaceFxPhonemesRawParsingCustomParse(stream, offset);
                return null;
            }

            public partial ParseResult FaceFxPhonemesRawParsingCustomParse(OverlayStream stream, int offset)
            {
                if (_faceFxPhonemesLoc == null)
                {
                    _faceFxPhonemesLoc = (ushort)(stream.Position - offset);
                }
                PluginUtilityTranslation.SkipPastAll(stream, _package.MetaData.Constants, RecordTypes.PHTN);
                PluginUtilityTranslation.SkipPastAll(stream, _package.MetaData.Constants, RecordTypes.PHWT);
                return null;
            }

            private FaceFxPhonemes GetFaceFx()
            {
                var ret = new FaceFxPhonemes();
                if (_faceFxPhonemesLoc == null) return ret;
                var frame = new MutagenFrame(new MutagenMemoryReadStream(_data.Slice(_faceFxPhonemesLoc.Value), _package.MetaData));
                FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, ret);
                return ret;
            }

            partial void MorphValuesCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
            {
                this.MorphValues = this.ParseRepeatedTypelessSubrecord<MorphValueBinaryOverlay>(
                    stream: stream,
                    parseParams: null,
                    trigger: MorphValue_Registration.TriggerSpecs,
                    factory: MorphValueBinaryOverlay.MorphValueFactory);

                // Read off last index subrecord
                stream.ReadSubrecordFrame(RecordTypes.MLSI);
            }
        }

        public partial class RaceBinaryWriteTranslation
        {
            public static partial void WriteBinaryExtraNAM2Custom(MutagenWriter writer, IRaceGetter item)
            {
                if (item.ExportingExtraNam2)
                {
                    using var header = HeaderExport.Subrecord(writer, Race.NAM2);
                }
            }

            public static partial void WriteBinaryBipedObjectsCustom(MutagenWriter writer, IRaceGetter item)
            {
                var bipedObjs = item.BipedObjects;
                for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
                {
                    using var name = HeaderExport.Subrecord(writer, RecordTypes.NAME);
                    StringBinaryTranslation.Instance.Write(writer, bipedObjs[(BipedObject)i].Name, StringBinaryType.NullTerminate);
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

            public static partial void WriteBinaryBoneDataParseCustom(MutagenWriter writer, IRaceGetter item)
            {
                var bones = item.BoneData;
                WriteBinaryBoneDataParseCustom(writer, bones.Male, 0);
                WriteBinaryBoneDataParseCustom(writer, bones.Female, 1);
            }

            private static void WriteBinaryBoneDataParseCustom(MutagenWriter writer, IReadOnlyList<IBoneGetter>? bones, int genderInt)
            {
                if (bones == null) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.BSMP))
                {
                    writer.Write(genderInt);
                }
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IBoneGetter>.Instance.Write(writer, bones,
                    transl: (MutagenWriter subWriter, IBoneGetter subItem, TypedWriteParams? conv) =>
                    {
                        var Item = subItem;
                        ((BoneBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                            item: Item,
                            writer: subWriter,
                            translationParams: conv);
                    });
            }

            public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IRaceGetter item)
            {
                EnumBinaryTranslation<Race.Flag, MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    item.Flags,
                    length: 4);
            }

            public static partial void WriteBinaryFlags2Custom(MutagenWriter writer, IRaceGetter item)
            {
                ulong flags = (ulong)item.Flags;
                flags >>= 32;
                writer.Write((uint)flags);
            }

            public static partial void WriteBinaryFaceFxPhonemesListingParsingCustom(MutagenWriter writer, IRaceGetter item) => FaceFxPhonemesBinaryWriteTranslation.WriteFaceFxPhonemes(writer, item.FaceFxPhonemes);

            public static partial void WriteBinaryFaceFxPhonemesRawParsingCustom(MutagenWriter writer, IRaceGetter item)
            {
                // Handled by Listing section
            }

            public static partial void WriteBinaryMorphValuesCustom(MutagenWriter writer, IRaceGetter item)
            {
                var morphs = item.MorphValues;
                if (morphs.Count == 0) return;
                
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IMorphValueGetter>.Instance.Write(
                    writer: writer,
                    items: morphs,
                    transl: (MutagenWriter subWriter, IMorphValueGetter subItem, TypedWriteParams? conv) =>
                    {
                        var Item = subItem;
                        ((MorphValueBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                            item: Item,
                            writer: subWriter,
                            translationParams: conv);
                    });
                using (HeaderExport.Subrecord(writer, RecordTypes.MLSI))
                {
                    if (morphs.Count == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(morphs.Max(x => x.Index));
                    }
                }
            }
        }
    }
}
