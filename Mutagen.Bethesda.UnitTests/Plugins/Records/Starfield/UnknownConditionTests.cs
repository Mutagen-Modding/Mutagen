using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class UnknownConditionTests : ASpecificCaseTest<LeveledItem, ILeveledItemGetter>
{
    public override ModPath Path => TestDataPathing.StarfieldUnknownCondition;
    public override GameRelease Release => GameRelease.Starfield;
    
    public override void TestItem(ILeveledItemGetter item)
    {
        
    }
}