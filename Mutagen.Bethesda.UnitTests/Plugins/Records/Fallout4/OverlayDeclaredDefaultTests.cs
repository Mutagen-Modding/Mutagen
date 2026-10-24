using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class OverlayDeclaredDefaultTests
{
    private const uint FormId = 0x00000001;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static BinaryOverlayFactoryPackage Package() => new(new ParsingMeta(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters)));

    private static byte[] Record(string type, params byte[][] subrecords)
    {
        var content = subrecords.SelectMany(x => x).ToArray();
        var record = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength + content.Length];
        Encoding.ASCII.GetBytes(type).CopyTo(record, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(4), (uint)content.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(12), FormId);
        BinaryPrimitives.WriteUInt16LittleEndian(record.AsSpan(20), FormVersion);
        content.CopyTo(record, GameConstants.Fallout4.MajorConstants.HeaderLength);
        return record;
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var subrecord = new byte[6 + content.Length];
        Encoding.ASCII.GetBytes(type).CopyTo(subrecord, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(subrecord.AsSpan(4), (ushort)content.Length);
        content.CopyTo(subrecord, 6);
        return subrecord;
    }

    [Fact]
    public void DialogTopic_WithoutPnam_PriorityIsTheDefault()
    {
        DialogTopicBinaryOverlay.DialogTopicFactory(Record("DIAL"), Package())
            .Priority.ShouldBe(DialogTopic.PriorityDefault);
    }

    [Fact]
    public void EffectShader_WithoutDnam_MembraneZTestIsTheDefault()
    {
        EffectShaderBinaryOverlay.EffectShaderFactory(Record("EFSH"), Package())
            .MembraneZTest.ShouldBe(EffectShader.ZTest.EqualTo);
    }

    [Fact]
    public void QuestAdapter_EndingAtTheBreak_ExtraBindDataVersionIsTheDefault()
    {
        var versionFormatAndNoScripts = new byte[] { 6, 0, 2, 0, 0, 0 };
        QuestBinaryOverlay.QuestFactory(Record("QUST", Subrecord("VMAD", versionFormatAndNoScripts)), Package())
            .VirtualMachineAdapter!.ExtraBindDataVersion.ShouldBe((byte)3);
    }
}
