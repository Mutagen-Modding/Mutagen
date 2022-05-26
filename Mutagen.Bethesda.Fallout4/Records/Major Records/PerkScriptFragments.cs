using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class PerkScriptFragmentsBinaryCreateTranslation
{
    public static PerkScriptFragments ReadFragments(MutagenFrame frame, ushort objectFormat)
    {
        var ret = new PerkScriptFragments();
        FillFragments(frame, objectFormat, ret);
        return ret;
    }

    public static void FillFragments(MutagenFrame frame, ushort objectFormat, IPerkScriptFragments item)
    {
        item.ExtraBindDataVersion = frame.ReadUInt8();
        item.Script = AVirtualMachineAdapterBinaryCreateTranslation.ReadEntry(frame, objectFormat);
        item.Fragments.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<PerkScriptFragment>.Instance.Parse(
                amount: frame.ReadUInt16(),
                reader: frame,
                transl: PerkScriptFragment.TryCreateFromBinary));
    }
}

partial class PerkScriptFragmentsBinaryWriteTranslation
{
    public static void WriteFragments(MutagenWriter writer, IPerkScriptFragmentsGetter item, ushort objectFormat)
    {
        writer.Write(item.ExtraBindDataVersion);
        AVirtualMachineAdapterBinaryWriteTranslation.WriteEntry(writer, item.Script, objectFormat);
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IPerkScriptFragmentGetter>.Instance.Write(
            writer: writer,
            items: item.Fragments,
            countLengthLength: 2,
            transl: (MutagenWriter subWriter, IPerkScriptFragmentGetter subItem, TypedWriteParams? conv) =>
            {
                var Item = subItem;
                ((PerkScriptFragmentBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}

partial class PerkScriptFragmentsBinaryOverlay
{
    public IReadOnlyList<IPerkScriptFragmentGetter> Fragments { get; private set; } = null!;

    public byte ExtraBindDataVersion => throw new NotImplementedException();

    public IScriptEntryGetter Script => throw new NotImplementedException();

    public string FileName => throw new NotImplementedException();
}