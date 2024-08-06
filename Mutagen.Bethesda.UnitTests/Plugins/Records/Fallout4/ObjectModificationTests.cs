using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class ObjectModificationCanImportNoDataTest : ASpecificCaseTest<AObjectModification, IAObjectModificationGetter>
{
    public static string NoDataPath = "Files/Fallout4/ObjectModificationNoData.esp";

    public override ModPath Path => NoDataPath;
    public override GameRelease Release => GameRelease.Fallout4;
    public override bool TestPassthrough => false;

    public override void TestItem(IAObjectModificationGetter item)
    {
    }
}