using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class MajorRecordBuilderTests
{
    [Theory]
    [MutagenModAutoData]
    public void Typical(
        SkyrimMod mod,
        Npc npc)
    {
        npc.FormKey.ModKey.Should().Be(mod.ModKey);
        mod.Npcs.Records.Should().Contain(n => n == npc);
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void ConfigureMembers(
        SkyrimMod mod,
        Npc npc)
    {
        npc.FormKey.ModKey.Should().Be(mod.ModKey);
        mod.Npcs.Records.Should().Contain(n => n == npc);
    }
    
    [Theory]
    [MutagenModAutoData]
    public void TwoRequests(
        SkyrimMod mod,
        Npc npc1,
        Npc npc2)
    {
        npc1.Should().NotBeSameAs(npc2);
        npc1.FormKey.ModKey.Should().Be(mod.ModKey);
        npc2.FormKey.ModKey.Should().Be(mod.ModKey);
        npc1.FormKey.Should().NotBe(npc2.FormKey);
        mod.Npcs.Records.Should().Contain(n => n == npc1);
        mod.Npcs.Records.Should().Contain(n => n == npc2);
        mod.Npcs.Records.Should().HaveCount(2);
    }
    
    [Theory]
    [MutagenModAutoData]
    public void DeepRecord(
        SkyrimMod mod,
        DialogResponses responses)
    {
        responses.FormKey.ModKey.Should().Be(mod.ModKey);
    }
}