using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

public partial class IdleMarker
{
    [Flags]
    public enum MajorFlag
    {
        ChildCanUse = 0x2000_0000,
    }

    [Flags]
    public enum Flag
    {
        RunInSequence = 0x0001,
        DoOnce = 0x0004,
        IgnoredBySandbox = 0x0010
    }
}

partial class IdleMarkerBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryAnimationCountCustom(MutagenFrame frame, IIdleMarkerInternal item, PreviousParse lastParsed)
    {
        // Skip. Don't care
        frame.ReadSubrecord();
        return null;
    }

    public static ExtendedList<IFormLinkGetter<IIdleAnimationGetter>> ParseAnimations(IMutagenReadStream stream)
    {
        var subFrame = stream.ReadSubrecord();
        var ret = new ExtendedList<IFormLinkGetter<IIdleAnimationGetter>>();
        int pos = 0;
        while (pos < subFrame.Content.Length)
        {
            ret.Add(
                new FormLink<IIdleAnimationGetter>(
                    FormKeyBinaryTranslation.Instance.Parse(subFrame.Content.Slice(pos), stream.MetaData.MasterReferences!)));
            pos += 4;
        }
        return ret;
    }

    public static partial void FillBinaryAnimationsCustom(MutagenFrame frame, IIdleMarkerInternal item, PreviousParse lastParsed)
    {
        item.Animations = ParseAnimations(frame);
    }
}

partial class IdleMarkerBinaryWriteTranslation
{
    public static partial void WriteBinaryAnimationCountCustom(MutagenWriter writer, IIdleMarkerGetter item)
    {
        if (item.Animations is not { } anims) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.IDLC))
        {
            writer.Write(checked((byte)anims.Count));
        }
    }

    public static partial void WriteBinaryAnimationsCustom(MutagenWriter writer, IIdleMarkerGetter item)
    {
        if (item.Animations is not { } anims) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.IDLA))
        {
            foreach (var anim in anims)
            {
                FormKeyBinaryTranslation.Instance.Write(writer, anim.FormKey);
            }
        }
    }
}

partial class IdleMarkerBinaryOverlay
{
    public partial ParseResult AnimationCountCustomParse(OverlayStream stream, int offset)
    {
        // Skip. Don't care
        stream.ReadSubrecord();
        return null;
    }

    public IReadOnlyList<IFormLinkGetter<IIdleAnimationGetter>>? Animations { get; private set; }

    partial void AnimationsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        var subHeader = stream.ReadSubrecordHeader();
        Animations = BinaryOverlayList.FactoryByStartIndex<IFormLinkGetter<IIdleAnimationGetter>>(
            mem: stream.RemainingMemory.Slice(0, subHeader.ContentLength),
            package: _package,
            itemLength: 4,
            getter: (s, p) => new FormLink<IIdleAnimationGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))));
    }
}