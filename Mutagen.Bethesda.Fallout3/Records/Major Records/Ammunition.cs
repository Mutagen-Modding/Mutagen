using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Fallout3.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Ammunition
{
    [Flags]
    public enum AmmoFlag : uint
    {
        IgnoresNormalWeaponResistance = 0x01,
        NonPlayable = 0x02,
    }
}

partial class AmmunitionBinaryCreateTranslation
{
    public static partial void FillBinaryExtraDataCustom(
        MutagenFrame frame,
        IAmmunitionInternal item,
        PreviousParse lastParsed)
    {
        var subHeader = frame.ReadSubrecordHeader(RecordTypes.DAT2);
        var dataFrame = frame.SpawnWithLength(subHeader.ContentLength);
        var extraData = new AmmunitionExtraData();
        extraData.ProjectilesPerShot = dataFrame.ReadUInt32();
        extraData.Projectile.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: dataFrame));
        extraData.Weight = dataFrame.ReadFloat();
        if (dataFrame.Remaining >= 8)
        {
            extraData.ConsumedAmmo.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: dataFrame));
            extraData.ConsumedPercentage = dataFrame.ReadFloat();
        }
        item.ExtraData = extraData;
    }
}

partial class AmmunitionBinaryWriteTranslation
{
    public static partial void WriteBinaryExtraDataCustom(
        MutagenWriter writer,
        IAmmunitionGetter item)
    {
        if (item.ExtraData is not { } extraData) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DAT2))
        {
            writer.Write(extraData.ProjectilesPerShot);
            FormLinkBinaryTranslation.Instance.Write(writer: writer, item: extraData.Projectile);
            writer.Write(extraData.Weight);
            FormLinkBinaryTranslation.Instance.Write(writer: writer, item: extraData.ConsumedAmmo);
            writer.Write(extraData.ConsumedPercentage);
        }
    }
}

partial class AmmunitionBinaryOverlay
{
    private IAmmunitionExtraDataGetter? _ExtraData;

    partial void ExtraDataCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var dat2Rec = stream.ReadSubrecord(RecordTypes.DAT2);
        var extraData = new AmmunitionExtraData();
        extraData.ProjectilesPerShot = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(dat2Rec.Content);
        extraData.Projectile.SetTo(FormKeyBinaryTranslation.Instance.Parse(dat2Rec.Content.Slice(4, 4), _package.MetaData.MasterReferences));
        extraData.Weight = dat2Rec.Content.Slice(8, 4).Float();
        if (dat2Rec.Content.Length >= 20)
        {
            extraData.ConsumedAmmo.SetTo(FormKeyBinaryTranslation.Instance.Parse(dat2Rec.Content.Slice(12, 4), _package.MetaData.MasterReferences));
            extraData.ConsumedPercentage = dat2Rec.Content.Slice(16, 4).Float();
        }
        _ExtraData = extraData;
    }

    public partial IAmmunitionExtraDataGetter? GetExtraDataCustom() => _ExtraData;
}
