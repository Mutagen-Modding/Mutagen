using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

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

        item.WalkForwardFootsteps.SetTo(ReadIn(counts[0]));
        item.RunForwardFootsteps.SetTo(ReadIn(counts[1]));
        item.WalkForwardAlternateFootsteps.SetTo(ReadIn(counts[2]));
        item.RunForwardAlternateFootsteps.SetTo(ReadIn(counts[3]));
        item.WalkForwardAlternateFootsteps2.SetTo(ReadIn(counts[4]));

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
        var walkForwardFootsteps = item.WalkForwardFootsteps;
        var runForwardFootsteps = item.RunForwardFootsteps;
        var walkAlternateFootsteps = item.WalkForwardAlternateFootsteps;
        var runAlternateFootsteps = item.RunForwardAlternateFootsteps;
        var walkAlternateFootsteps2 = item.WalkForwardAlternateFootsteps2;

        using (HeaderExport.Subrecord(writer, RecordTypes.XCNT))
        {
            writer.Write(walkForwardFootsteps.Count);
            writer.Write(runForwardFootsteps.Count);
            writer.Write(walkAlternateFootsteps.Count);
            writer.Write(runAlternateFootsteps.Count);
            writer.Write(walkAlternateFootsteps2.Count);
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
            WriteListOfFormKeys(walkForwardFootsteps);
            WriteListOfFormKeys(runForwardFootsteps);
            WriteListOfFormKeys(walkAlternateFootsteps);
            WriteListOfFormKeys(runAlternateFootsteps);
            WriteListOfFormKeys(walkAlternateFootsteps2);
        }
    }
}

partial class FootstepSetBinaryOverlay
{
    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> WalkForwardFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> RunForwardFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> WalkForwardAlternateFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> RunForwardAlternateFootsteps { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public IReadOnlyList<IFormLinkGetter<IFootstepGetter>> WalkForwardAlternateFootsteps2 { get; private set; } = Array.Empty<IFormLinkGetter<IFootstepGetter>>();

    public partial ParseResult CountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        int[] counts = FootstepSetBinaryCreateTranslation.GetListCounts(stream);

        IReadOnlyList<IFormLinkGetter<IFootstepGetter>> Get(int index)
        {
            var ret =  BinaryOverlayList.FactoryByCount<IFormLinkGetter<IFootstepGetter>>(
                _recordData.Slice(stream.Position - offset, 4 * counts[index]),
                _package,
                itemLength: 4,
                count: checked((uint)counts[index]),
                getter: (s, p) => new FormLink<IFootstepGetter>(FormKeyBinaryTranslation.Instance.Parse(s, p.MetaData.MasterReferences!)));
            stream.Position += 4 * counts[index];
            return ret;
        }

        WalkForwardFootsteps = Get(0);
        RunForwardFootsteps = Get(1);
        WalkForwardAlternateFootsteps = Get(2);
        RunForwardAlternateFootsteps = Get(3);
        WalkForwardAlternateFootsteps2 = Get(4);

        return null;
    }
}