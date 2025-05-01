using Shouldly;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class TranslationMaskTests
{
    const string Name = "Test";

    [Fact]
    public void FalseLoquiSpecification()
    {
        ICell cell = new Cell(TestConstants.Form1, SkyrimRelease.SkyrimLE);
        cell.Landscape = new Landscape(TestConstants.Form2, SkyrimRelease.SkyrimLE);
        var copy = (Cell) cell.DeepCopy(new Cell.TranslationMask(true)
        {
            Landscape = false
        });
        copy.Landscape.ShouldBeNull();
    }

    private Npc GetNpc()
    {
        return new Npc(TestConstants.Form1, SkyrimRelease.SkyrimLE)
        {
            Name = Name,
            Destructible = new Destructible()
            {
                Stages = new ExtendedList<DestructionStage>()
                {
                    new()
                }
            },
        };
    }

    private void AssertHasDestructible(INpcGetter n)
    {
        n.Destructible.ShouldNotBeNull();
        n.Destructible!.Stages.ShouldHaveCount(1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DefaultOnBringsSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: true, onOverall: npcOnOverall)
        {
            Name = false
        });
        copy.Name.ShouldBeNull();
        AssertHasDestructible(copy);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DefaultOffExcludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: false, onOverall: npcOnOverall)
        {
            Name = true
        });
        copy.Name!.String.ShouldBe(Name);
        copy.Destructible.ShouldBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnOverallIncludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: false, onOverall: npcOnOverall)
        {
            Name = true,
            Destructible = new Destructible.TranslationMask(defaultOn: true, onOverall: true)
        });
        copy.Name!.String.ShouldBe(Name);
        AssertHasDestructible(copy);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OffOverallExcludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: false, onOverall: npcOnOverall)
        {
            Name = true,
            Destructible = new Destructible.TranslationMask(defaultOn: true, onOverall: false)
        });
        copy.Name!.String.ShouldBe(Name);
        copy.Destructible.ShouldBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnOverallWithDefaultOffIncludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: false, onOverall: npcOnOverall)
        {
            Name = true,
            Destructible = new Destructible.TranslationMask(defaultOn: false, onOverall: true)
            {
                Stages = true
            }
        });
        copy.Name!.String.ShouldBe(Name);
        AssertHasDestructible(copy);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OffOverallWithDefaultOnExcludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: true, onOverall: npcOnOverall)
        {
            Name = true,
            Destructible = new Destructible.TranslationMask(defaultOn: true, onOverall: false)
        });
        copy.Name!.String.ShouldBe(Name);
        copy.Destructible.ShouldBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnOverallWithDefaultOfIncludesSubobject(bool npcOnOverall)
    {
        var rec = GetNpc();
        var copy = rec.DeepCopy(new Npc.TranslationMask(defaultOn: true, onOverall: npcOnOverall)
        {
            Name = true,
            Destructible = new Destructible.TranslationMask(defaultOn: false, onOverall: true)
        });
        copy.Name!.String.ShouldBe(Name);
        copy.Destructible.ShouldNotBeNull();
        copy.Destructible!.Stages.ShouldBeEmpty();
    }
}