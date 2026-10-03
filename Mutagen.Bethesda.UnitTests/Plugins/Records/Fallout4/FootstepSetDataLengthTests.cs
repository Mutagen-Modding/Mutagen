using System.Text;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Fallout4;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class FootstepSetDataLengthTests
{
    private static readonly ModKey TestModKey = new("Test", ModType.Plugin);

    private static ParsingMeta Meta()
    {
        var masters = SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(TestModKey));
        return new ParsingMeta(GameConstants.Fallout4, TestModKey, masters);
    }

    private static void WriteSubrecord(MemoryStream stream, string type, byte[] content)
    {
        stream.Write(Encoding.ASCII.GetBytes(type));
        stream.Write(BitConverter.GetBytes(checked((ushort)content.Length)));
        stream.Write(content);
    }

    private static byte[] Counts(params uint[] counts)
    {
        return counts.SelectMany(BitConverter.GetBytes).ToArray();
    }

    private static byte[] FormIds(params uint[] ids)
    {
        return ids.SelectMany(BitConverter.GetBytes).ToArray();
    }

    private static byte[] MakeFootstepSetBytes(byte[] xcnt, byte[] data)
    {
        var content = new MemoryStream();
        WriteSubrecord(content, "XCNT", xcnt);
        WriteSubrecord(content, "DATA", data);
        WriteSubrecord(content, "EDID", Encoding.ASCII.GetBytes("TestFootstepSet\0"));
        var body = content.ToArray();

        var record = new MemoryStream();
        record.Write(Encoding.ASCII.GetBytes("FSTS"));
        record.Write(BitConverter.GetBytes(body.Length));
        record.Write(BitConverter.GetBytes(0));
        record.Write(BitConverter.GetBytes(0x800));
        record.Write(BitConverter.GetBytes(0));
        record.Write(BitConverter.GetBytes((ushort)131));
        record.Write(BitConverter.GetBytes((ushort)0));
        record.Write(body);
        return record.ToArray();
    }

    private static IFootstepSetGetter ReadDirect(byte[] bytes)
    {
        return FootstepSet.CreateFromBinary(
            new MutagenFrame(new MutagenMemoryReadStream(bytes, Meta())));
    }

    private static IFootstepSetGetter ReadOverlay(byte[] bytes)
    {
        var meta = Meta();
        return FootstepSetBinaryOverlay.FootstepSetFactory(
            new OverlayStream(bytes, meta),
            new BinaryOverlayFactoryPackage(meta));
    }

    private static IEnumerable<IFootstepSetGetter> ReadBoth(byte[] bytes)
    {
        yield return ReadDirect(bytes);
        yield return ReadOverlay(bytes);
    }

    [Fact]
    public void PopulatedListWithSurplusData_ReadsListAndFollowingSubrecords()
    {
        var data = FormIds(0x801).Concat(new byte[4]).ToArray();
        var bytes = MakeFootstepSetBytes(Counts(1, 0, 0, 0, 0), data);

        foreach (var footstepSet in ReadBoth(bytes))
        {
            footstepSet.EditorID.ShouldBe("TestFootstepSet");
            footstepSet.WalkFootsteps.Select(x => x.FormKey)
                .ShouldBe(new[] { new FormKey(TestModKey, 0x801) });
            footstepSet.RunFootsteps.ShouldBeEmpty();
            footstepSet.SprintFootsteps.ShouldBeEmpty();
            footstepSet.SneakFootsteps.ShouldBeEmpty();
            footstepSet.SwimFootsteps.ShouldBeEmpty();
        }
    }
}
