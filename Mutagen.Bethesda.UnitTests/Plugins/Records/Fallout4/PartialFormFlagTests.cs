using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class PartialFormFlagTests
{
    private const int PartialFormBit = 1 << 14;

    private static readonly FormKey Key = FormKey.Factory("123456:Test.esp");

    [Fact]
    public void Cell_PartialForm_IsRecordFlagBit14()
    {
        new Cell(Key, Fallout4Release.Fallout4) { MajorFlags = Cell.MajorFlag.PartialForm }
            .MajorRecordFlagsRaw.ShouldBe(PartialFormBit);
    }

    [Fact]
    public void Worldspace_PartialForm_IsRecordFlagBit14()
    {
        new Worldspace(Key, Fallout4Release.Fallout4) { MajorFlags = Worldspace.MajorFlag.PartialForm }
            .MajorRecordFlagsRaw.ShouldBe(PartialFormBit);
    }

    [Fact]
    public void DialogTopic_PartialForm_IsRecordFlagBit14()
    {
        new DialogTopic(Key, Fallout4Release.Fallout4) { MajorFlags = DialogTopic.MajorFlag.PartialForm }
            .MajorRecordFlagsRaw.ShouldBe(PartialFormBit);
    }
}
