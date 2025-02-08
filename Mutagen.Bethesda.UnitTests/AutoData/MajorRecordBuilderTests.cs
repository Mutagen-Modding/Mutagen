using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
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
        npc.FormKey.ModKey.ShouldBe(mod.ModKey);
        mod.Npcs.Records.ShouldContain(n => n == npc);
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void ConfigureMembers(
        SkyrimMod mod,
        Npc npc)
    {
        npc.FormKey.ModKey.ShouldBe(mod.ModKey);
        mod.Npcs.Records.ShouldContain(n => n == npc);
    }
    
    [Theory]
    [MutagenModAutoData]
    public void TwoRequests(
        SkyrimMod mod,
        Npc npc1,
        Npc npc2)
    {
        npc1.ShouldNotBeSameAs(npc2);
        npc1.FormKey.ModKey.ShouldBe(mod.ModKey);
        npc2.FormKey.ModKey.ShouldBe(mod.ModKey);
        npc1.FormKey.ShouldNotBe(npc2.FormKey);
        mod.Npcs.Records.ShouldContain(n => n == npc1);
        mod.Npcs.Records.ShouldContain(n => n == npc2);
        mod.Npcs.Records.ShouldHaveCount(2);
    }
    
    [Theory]
    [MutagenModAutoData]
    public void DeepRecord(
        SkyrimMod mod,
        DialogResponses responses)
    {
        responses.FormKey.ModKey.ShouldBe(mod.ModKey);
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void CellNestedMajorRecords(
        SkyrimMod mod,
        Cell cell)
    {
        cell.FormKey.ModKey.ShouldBe(mod.ModKey);
        cell.Temporary.ShouldNotBeEmpty();
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void WeaponDataSubArray(
        SkyrimMod mod,
        Weapon weapon)
    {
        weapon.Data.ShouldNotBeNull();
        weapon.Data!.Unknown3.Length.ShouldBe(12);
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void IPlacedWithMod(
        SkyrimMod mod,
        IPlaced placed)
    {
        placed.FormKey.ModKey.ShouldBe(mod.ModKey);
        placed.IsCompressed.ShouldBeFalse();
        placed.IsDeleted.ShouldBeFalse();
    }
    
    [Theory]
    [MutagenModAutoData(ConfigureMembers: true)]
    public void IPlacedNoMod(
        IPlaced placed)
    {
        placed.FormKey.ShouldNotBe(FormKey.Null);
        placed.IsCompressed.ShouldBeFalse();
        placed.IsDeleted.ShouldBeFalse();
    }
}