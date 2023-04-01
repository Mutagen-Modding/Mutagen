using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.UnitTests.Plugins.Records;

namespace Mutagen.Bethesda.UnitTests.Fallout4.Records;

public class ObjectModificationCanImportNoDataTest : ASpecificCaseTest<AObjectModification, IAObjectModificationGetter>
{
    public static string NoDataPath = "Files/Fallout4/ObjectModificationNoData.esp";

    public override ModPath Path => NoDataPath;
    public override GameRelease Release => GameRelease.Fallout4;
    
    public override void TestItem(IAObjectModificationGetter item)
    {
    }
}