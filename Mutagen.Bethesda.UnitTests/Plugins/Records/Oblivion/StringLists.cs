using FluentAssertions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Oblivion;

public class StringLists : ASpecificCaseTest<Creature, ICreatureGetter>
{
    public override ModPath Path => TestDataPathing.OblivionStringLists;
    public override GameRelease Release => GameRelease.Oblivion;
    public override bool TestPassthrough => false;
    
    public override void TestItem(ICreatureGetter item)
    {
        item.Models.Should().Equal(
            "NDAyleid.NIF",
            "NDAyleidLegs.NIF",
            "NDMinionHead.NIF");
    }
}