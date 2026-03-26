using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class PackageIdles
{
    public enum Types
    {
        None = 0,
        Standard = 8,
        Patrol = 9,
        Guard = 12,
    }
}

partial class PackageIdlesBinaryCreateTranslation
{
    public static partial void FillBinaryAnimationsCustom(MutagenFrame frame, IPackageIdles item, PreviousParse lastParsed)
    {
        var subFrame = frame.ReadSubrecord();
        item.Animations.Clear();
        int pos = 0;
        while (pos < subFrame.Content.Length)
        {
            item.Animations.Add(
                FormLinkBinaryTranslation.Instance.Factory<IIdleAnimationGetter>(frame.MetaData, subFrame.Content.Slice(pos)));
            pos += 4;
        }
    }
}

partial class PackageIdlesBinaryWriteTranslation
{
    public static partial void WriteBinaryAnimationsCustom(MutagenWriter writer, IPackageIdlesGetter item)
    {
        var anims = item.Animations;
        if (anims.Count == 0) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.IDLA))
        {
            foreach (var anim in anims)
            {
                FormKeyBinaryTranslation.Instance.Write(writer, anim);
            }
        }
    }
}

partial class PackageIdlesBinaryOverlay
{
    public IReadOnlyList<IFormLinkGetter<IIdleAnimationGetter>> Animations { get; private set; } = [];

    partial void AnimationsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        var subHeader = stream.ReadSubrecordHeader();
        Animations = BinaryOverlayList.FactoryByStartIndex<IFormLinkGetter<IIdleAnimationGetter>>(
            mem: stream.RemainingMemory.Slice(0, subHeader.ContentLength),
            package: _package,
            itemLength: 4,
            getter: (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IIdleAnimationGetter>(p, s));
    }
}
