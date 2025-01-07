using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

partial class FootstepSetBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryCountCustom(MutagenFrame frame, IFootstepSetInternal item, PreviousParse lastParsed)
    {
        var counts = GetListCounts(frame);

        IEnumerable<IFormLinkGetter<Footstep>> ReadIn(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new FormLink<Footstep>(FormKeyBinaryTranslation.Instance.Parse(frame));
            }
        }

        item.WalkFootsteps.SetTo(ReadIn(counts[0]));
        item.RunFootsteps.SetTo(ReadIn(counts[1]));
        item.SprintFootsteps.SetTo(ReadIn(counts[2]));
        item.SneakFootsteps.SetTo(ReadIn(counts[3]));
        item.SwimFootsteps.SetTo(ReadIn(counts[4]));

        return null;
    }

    public static int[] GetListCounts(IMutagenReadStream frame)
    {
        var subFrame = frame.ReadSubrecordHeader(RecordTypes.XCNT);
        if (subFrame.ContentLength != 20)
        {
            throw new ArgumentException($"XCNT record had unexpected length {subFrame.ContentLength} != 20");
        }

        int[] ret = new int[5];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = checked((int)frame.ReadUInt32());
        }

        var formIDCount = ret.Sum();

        var dataHeader = frame.ReadSubrecordHeader(RecordTypes.DATA);
        var expectedLen = formIDCount * 4;
        if (dataHeader.ContentLength != expectedLen)
        {
            throw new ArgumentException($"DATA record had unexpected length that did not match previous counts {dataHeader.ContentLength} != {expectedLen}");
        }
        return ret;
    }
}

partial class FootstepSetBinaryWriteTranslation
{
    public static partial void WriteBinaryCountCustom(MutagenWriter writer, IFootstepSetGetter item)
    {
        var walkFootsteps = item.WalkFootsteps;
        var runFootsteps = item.RunFootsteps;
        var sprint = item.SprintFootsteps;
        var sneak = item.SneakFootsteps;
        var swim = item.SwimFootsteps;

        using (HeaderExport.Subrecord(writer, RecordTypes.XCNT))
        {
            writer.Write(walkFootsteps.Count);
            writer.Write(runFootsteps.Count);
            writer.Write(sprint.Count);
            writer.Write(sneak.Count);
            writer.Write(swim.Count);
        }

        void WriteListOfFormKeys(IReadOnlyList<IFormLinkGetter<IFootstepGetter>> formKeys)
        {
            foreach (var link in formKeys)
            {
                FormLinkBinaryTranslation.Instance.Write(writer, link);
            }
        }

        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            WriteListOfFormKeys(walkFootsteps);
            WriteListOfFormKeys(runFootsteps);
            WriteListOfFormKeys(sprint);
            WriteListOfFormKeys(sneak);
            WriteListOfFormKeys(swim);
        }
    }
}

partial class FootstepSetBinaryOverlay
{
    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> WalkFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> RunFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> SprintFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> SneakFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> SwimFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public partial ParseResult CountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        int[] counts = FootstepSetBinaryCreateTranslation.GetListCounts(stream);

        IReadOnlyList<IFormLinkGetter<IFootstepGetter>> Get(int index)
        {
            var ret = BinaryOverlayList.FactoryByCount<IFormLinkGetter<IFootstepGetter>>(
                _recordData.Slice(stream.Position - offset, 4 * counts[index]),
                _package,
                itemLength: 4,
                count: checked((uint)counts[index]),
                getter: (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IFootstepGetter>(p, s));
            stream.Position += 4 * counts[index];
            return ret;
        }

        WalkFootsteps = Get(0);
        RunFootsteps = Get(1);
        SprintFootsteps = Get(2);
        SneakFootsteps = Get(3);
        SwimFootsteps = Get(4);

        return null;
    }
}