using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EnumerateMajorRecordTests
{
    [Theory, MutagenModAutoData]
    public void EnumerateMajorRecordsOnMutableModSafe(
        SkyrimMod mod,
        Npc n)
    {
        foreach (var rec in mod.EnumerateMajorRecords())
        {
            mod.Npcs.AddNew();
        }
    }
    
    [Theory, MutagenModAutoData]
    public void EnumerateMajorRecordsTypedOnMutableModSafe(
        SkyrimMod mod,
        Npc n)
    {
        foreach (var rec in mod.EnumerateMajorRecords<INpcGetter>())
        {
            mod.Npcs.AddNew();
        }
    }
}