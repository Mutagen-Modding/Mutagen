using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System.Collections;
using Noggog.StructuredStrings;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using RecordTypes = Mutagen.Bethesda.Fallout4.Internals.RecordTypes;

namespace Mutagen.Bethesda.Fallout4;

public partial class ArmorAddon
{
    [Flags]
    public enum MajorFlag
    {
        NoUnderarmorScaling = 0x0000_0040,
        HiResFirstPersonOnly = 0x4000_0000
    }

    public IGenderedItem<Boolean> WeightSliderEnabled { get; set; } = new GenderedItem<Boolean>(default, default);
    IGenderedItemGetter<Boolean> IArmorAddonGetter.WeightSliderEnabled => this.WeightSliderEnabled;
}


partial class ArmorAddonBinaryCreateTranslation
{
    public static bool IsEnabled(byte b) => Enums.HasFlag(b, (byte)2);

    public static partial void FillBinaryWeightSliderEnabledCustom(MutagenFrame frame, IArmorAddonInternal item)
    {
        item.WeightSliderEnabled = new GenderedItem<bool>(frame.ReadUInt8() >= 2, frame.ReadUInt8() >= 2);
    }

    public static partial ParseResult FillBinaryBoneDataParseCustom(MutagenFrame frame, IArmorAddonInternal item)
    {
        var genderFrame = frame.ReadSubrecord(RecordTypes.BSMP);

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
}

partial class ArmorAddonBinaryWriteTranslation
{
    public static partial void WriteBinaryWeightSliderEnabledCustom(MutagenWriter writer, IArmorAddonGetter item)
    {
        var weightSlider = item.WeightSliderEnabled;
        writer.Write(weightSlider.Male ? (byte)2 : default(byte));
        writer.Write(weightSlider.Female ? (byte)2 : default(byte));
    }

    public static partial void WriteBinaryBoneDataParseCustom(MutagenWriter writer, IArmorAddonGetter item)
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
            transl: (MutagenWriter subWriter, IBoneGetter subItem, TypedWriteParams conv) =>
            {
                var Item = subItem;
                ((BoneBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}

internal partial class ArmorAddonBinaryOverlay
{
    public partial IGenderedItemGetter<Boolean> GetWeightSliderEnabledCustom() => new GenderedItem<bool>(
        _recordData.Slice(_DNAMLocation!.Value.Min + 2)[0] >= 2,
        _recordData.Slice(_DNAMLocation!.Value.Min + 3)[0] >= 2);

    private GenderedItem<IReadOnlyList<IBoneGetter>?>? _boneData;
    public IGenderedItemGetter<IReadOnlyList<IBoneGetter>?> BoneData => _boneData ?? new GenderedItem<IReadOnlyList<IBoneGetter>?>(null, null);

    public partial ParseResult BoneDataParseCustomParse(OverlayStream stream, int offset)
    {
        var genderFrame = stream.ReadSubrecord(RecordTypes.BSMP);
        _boneData ??= new GenderedItem<IReadOnlyList<IBoneGetter>?>(null, null);
        IReadOnlyList<IBoneGetter> list = this.ParseRepeatedTypelessSubrecord(
            stream: stream,
            translationParams: null,
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
}