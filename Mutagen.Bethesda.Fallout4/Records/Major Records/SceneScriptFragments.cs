using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class SceneScriptFragmentsBinaryCreateTranslation
{
    public static SceneScriptFragments ReadFragments(MutagenFrame frame, ushort objectFormat)
    {
        var ret = new SceneScriptFragments();
        FillFragments(frame, objectFormat, ret);
        ret.PhaseFragments.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ScenePhaseFragment>.Instance.Parse(
                amount: frame.ReadUInt16(),
                reader: frame,
                transl: ScenePhaseFragment.TryCreateFromBinary));
        return ret;
    }
}

partial class SceneScriptFragmentsBinaryWriteTranslation
{
    public static void WriteFragments(MutagenWriter writer, ushort objectFormat, ISceneScriptFragmentsGetter item)
    {
        ScriptFragmentsBinaryWriteTranslation.WriteFragments(writer, item, objectFormat);
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IScenePhaseFragmentGetter>.Instance.Write(
            writer: writer,
            items: item.PhaseFragments,
            countLengthLength: 2,
            transl: (MutagenWriter subWriter, IScenePhaseFragmentGetter subItem, TypedWriteParams? conv) =>
            {
                var Item = subItem;
                ((ScenePhaseFragmentBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}
