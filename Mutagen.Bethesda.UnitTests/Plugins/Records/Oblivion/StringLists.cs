using Shouldly;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Oblivion;

public class StringLists : ASpecificCaseTest<Creature, ICreatureGetter>
{
    public override ModPath Path => TestDataPathing.OblivionStringLists;
    public override GameRelease Release => GameRelease.Oblivion;
    public override bool TestPassthrough => false;
    
    public override void TestItem(ICreatureGetter item)
    {
        item.Models.ShouldEqualEnumerable(
            "NDAyleid.NIF",
            "NDAyleidLegs.NIF",
            "NDMinionHead.NIF");
    }
}

public class MalformedStringLists : ASpecificCaseTest<Creature, ICreatureGetter>
{
    public override ModPath Path => TestDataPathing.OblivionMalformedStringLists;
    public override GameRelease Release => GameRelease.Oblivion;
    public override bool TestPassthrough => false;
    
    public override void TestItem(ICreatureGetter item)
    {
        item.Models.ShouldEqualEnumerable(
            "NDAyleid.NIF",
            "NDAyleidLegs.NIF",
            "NDMinionHead.NIF");
    }
}