using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Xunit;
using TempFile = Noggog.IO.TempFile;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class ShortSubrecordTests
{
    private const ushort SpecialEditionFormVersion = 44;
    private const float HeaderVersion = 1.71f;
    private const int RecordCount = 1;
    private const int InteriorCellCount = 7;
    private const uint AcousticSpaceId = 0x800;
    private const uint AmbientSoundId = 0x801;
    private const uint EnvironmentTypeId = 0x802;
    private const uint NextObjectId = 0x803;

    public static TheoryData<int, bool> SubrecordLengthsAndWhetherTheirFieldReads => new()
    {
        { 0, false },
        { 2, false },
        { 4, true },
    };

    private static byte[] PluginWithTypedFieldsOfLength(int length)
    {
        var header = Record("TES4", 0,
            Subrecord("HEDR", [.. BitConverter.GetBytes(HeaderVersion), .. BitConverter.GetBytes(RecordCount), .. BitConverter.GetBytes(NextObjectId)]),
            Subrecord("INCC", BitConverter.GetBytes(InteriorCellCount)[..length]));
        var acousticSpace = Record("ASPC", AcousticSpaceId,
            Subrecord("OBND", new byte[12]),
            Subrecord("SNAM", BitConverter.GetBytes(AmbientSoundId)[..length]),
            Subrecord("BNAM", BitConverter.GetBytes(EnvironmentTypeId)));
        return header.Concat(TopLevelGroup("ASPC", acousticSpace)).ToArray();
    }

    private static byte[] Record(string type, uint formId, params byte[][] subrecords)
    {
        var content = subrecords.SelectMany(x => x).ToArray();
        var header = new byte[GameConstants.SkyrimSE.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), formId);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), SpecialEditionFormVersion);
        return header.Concat(content).ToArray();
    }

    private static byte[] TopLevelGroup(string label, byte[] content)
    {
        var header = new byte[GameConstants.SkyrimSE.GroupConstants.HeaderLength];
        Encoding.ASCII.GetBytes("GRUP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)(header.Length + content.Length)));
        Encoding.ASCII.GetBytes(label).CopyTo(header, 8);
        return header.Concat(content).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[GameConstants.SkyrimSE.SubConstants.HeaderLength];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static TempFile PluginFile(int length)
    {
        var file = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        File.WriteAllBytes(file.File.Path, PluginWithTypedFieldsOfLength(length));
        return file;
    }

    private static void ShouldReadOnlyTheFieldsThatFit(ISkyrimModGetter mod, bool fieldsRead)
    {
        mod.ModHeader.INCC.ShouldBe(fieldsRead ? InteriorCellCount : null);
        var acousticSpace = mod.AcousticSpaces.Single();
        (acousticSpace.AmbientSound.FormKeyNullable?.ID).ShouldBe(fieldsRead ? AmbientSoundId : null);
        acousticSpace.EnvironmentType.FormKey.ID.ShouldBe(EnvironmentTypeId);
    }

    [Theory]
    [MemberData(nameof(SubrecordLengthsAndWhetherTheirFieldReads))]
    public void DeepParse_ReadsATypedFieldOnlyFromASubrecordLongEnoughToHoldIt(int length, bool fieldsRead)
    {
        using var file = PluginFile(length);
        ShouldReadOnlyTheFieldsThatFit(
            SkyrimMod.CreateFromBinary(new ModPath(file.File.Path), SkyrimRelease.SkyrimSE),
            fieldsRead);
    }

    [Theory]
    [MemberData(nameof(SubrecordLengthsAndWhetherTheirFieldReads))]
    public void Overlay_ReadsATypedFieldOnlyFromASubrecordLongEnoughToHoldIt(int length, bool fieldsRead)
    {
        using var file = PluginFile(length);
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(new ModPath(file.File.Path), SkyrimRelease.SkyrimSE);
        ShouldReadOnlyTheFieldsThatFit(overlay, fieldsRead);
    }

    [Theory]
    [MemberData(nameof(SubrecordLengthsAndWhetherTheirFieldReads))]
    public void Overlay_WritesBackOnlyTheFieldsItRead(int length, bool fieldsRead)
    {
        using var file = PluginFile(length);
        using var written = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        var writtenPath = new ModPath(written.File.Path);
        using (var overlay = SkyrimMod.CreateFromBinaryOverlay(new ModPath(file.File.Path), SkyrimRelease.SkyrimSE))
        {
            overlay.BeginWrite.ToPath(writtenPath).WithNoLoadOrder().NoModKeySync().Write();
        }
        ShouldReadOnlyTheFieldsThatFit(SkyrimMod.CreateFromBinary(writtenPath, SkyrimRelease.SkyrimSE), fieldsRead);
    }
}
