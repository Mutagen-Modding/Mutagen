using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Plugins.Masters;
using static Mutagen.Bethesda.Plugins.Meta.GameConstants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class OblivionMultiModOverlayTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void BasicMerge_TwoModsWithDifferentSpells(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Create Spells in mod1
        var spell1 = mod1.Spells.AddNew();
        spell1.EditorID = "Spell1";

        var spell2 = mod1.Spells.AddNew();
        spell2.EditorID = "Spell2";

        // Create Spells in mod2
        var spell3 = mod2.Spells.AddNew();
        spell3.EditorID = "Spell3";

        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Verify merged view
        overlay.Spells.Count.ShouldBe(3);
        overlay.Spells.ShouldContain(f => f.EditorID == "Spell1");
        overlay.Spells.ShouldContain(f => f.EditorID == "Spell2");
        overlay.Spells.ShouldContain(f => f.EditorID == "Spell3");

        // Verify ModKey
        overlay.ModKey.ShouldBe(targetModKey);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void DuplicateFormKey_ThrowsException(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Create same FormKey in both mods
        var formKey = mod1.GetNextFormKey();
        var spell1 = mod1.Spells.AddNew(formKey);
        spell1.EditorID = "Spell1";

        var spell2 = mod2.Spells.AddNew(formKey);
        spell2.EditorID = "Spell2";

        // Create overlay - this should succeed
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Accessing the Spells group should throw when it tries to cache and detects the duplicate
        Should.Throw<SplitModException>(() =>
        {
            var count = overlay.Spells.Count;
        }).Message.ShouldContain(formKey.ToString());
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void MasterListMerging_OrderedByLoadOrder(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Set up masters for mod1
        var master1 = new ModKey("Oblivion", ModType.Master);
        var master2 = new ModKey("Knights", ModType.Master);
        mod1.ModHeader.MasterReferences.Add(new MasterReference { Master = master1 });
        mod1.ModHeader.MasterReferences.Add(new MasterReference { Master = master2 });

        // Set up masters for mod2 (different master)
        var master3 = new ModKey("DLCShiveringIsles", ModType.Master);
        mod2.ModHeader.MasterReferences.Add(new MasterReference { Master = master3 });

        // Define merged masters
        var mergedMasters = new List<IMasterReferenceGetter>
        {
            new MasterReference { Master = master1 },
            new MasterReference { Master = master2 },
            new MasterReference { Master = master3 }
        };

        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            mergedMasters);

        // Verify masters are in load order
        overlay.MasterReferences.Count.ShouldBe(3);
        overlay.MasterReferences[0].Master.ShouldBe(master1);
        overlay.MasterReferences[1].Master.ShouldBe(master2);
        overlay.MasterReferences[2].Master.ShouldBe(master3);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void DeepNestedRecords_CellsAndPlacedObjects(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Cells property should now work (returns IOblivionListGroupGetter<ICellBlockGetter>)
        var cells = overlay.Cells;
        cells.ShouldNotBeNull();

        // If mods are empty, cells should also be empty
        cells.Count.ShouldBe(0);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void NextFormID_ReturnsMaxFromAllMods(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Add some records to each mod so they have different NextFormIDs
        // AutoFixture creates mods with unique ModKeys, so adding records will give different FormIDs
        for (int i = 0; i < 10; i++)
        {
            mod1.Spells.AddNew();
        }

        for (int i = 0; i < 20; i++)
        {
            mod2.Spells.AddNew();
        }

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should return the maximum NextFormID from all mods to avoid FormID collisions
        var expected = Math.Max(mod1.ModHeader.Stats.NextFormID, mod2.ModHeader.Stats.NextFormID);
        overlay.NextFormID.ShouldBe(expected);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void RecordAccessByFormKey_WorksAcrossMods(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Create records in both mods
        var spell1 = mod1.Spells.AddNew();
        spell1.EditorID = "FromMod1";
        var formKey1 = spell1.FormKey;

        var spell2 = mod2.Spells.AddNew();
        spell2.EditorID = "FromMod2";
        var formKey2 = spell2.FormKey;

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should be able to access records from both mods by FormKey
        overlay.Spells.TryGetValue(formKey1, out var retrieved1).ShouldBeTrue();
        retrieved1.EditorID.ShouldBe("FromMod1");

        overlay.Spells.TryGetValue(formKey2, out var retrieved2).ShouldBeTrue();
        retrieved2.EditorID.ShouldBe("FromMod2");
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void EnumerateMajorRecords_IncludesAllMods(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Create different types of records
        mod1.Spells.AddNew().EditorID = "Spell1";
        mod1.Armors.AddNew().EditorID = "Armor1";

        mod2.Spells.AddNew().EditorID = "Spell2";
        mod2.Weapons.AddNew().EditorID = "Weapon1";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should enumerate all records from all mods
        var allRecords = overlay.EnumerateMajorRecords().ToList();
        allRecords.Count.ShouldBe(4);
        allRecords.ShouldContain(r => r.EditorID == "Spell1");
        allRecords.ShouldContain(r => r.EditorID == "Armor1");
        allRecords.ShouldContain(r => r.EditorID == "Spell2");
        allRecords.ShouldContain(r => r.EditorID == "Weapon1");
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void GetRecordCount_SumsAllMods(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Get initial counts
        var initialCount1 = mod1.GetRecordCount();
        var initialCount2 = mod2.GetRecordCount();

        // Add records to both mods
        mod1.Spells.AddNew();
        mod1.Spells.AddNew();
        mod1.Armors.AddNew();

        mod2.Spells.AddNew();
        mod2.Weapons.AddNew();

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should sum all records from all mods (initial + newly added)
        var expectedCount = mod1.GetRecordCount() + mod2.GetRecordCount();
        overlay.GetRecordCount().ShouldBe(expectedCount);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void EmptyMods_CreatesEmptyOverlay(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Empty mods should result in empty groups
        overlay.Spells.Count.ShouldBe(0);
        overlay.Armors.Count.ShouldBe(0);
        overlay.EnumerateMajorRecords().ShouldBeEmpty();
        overlay.GetRecordCount().ShouldBe(0u);
    }

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void ModHeader_TakenFromFirstMod(
        OblivionMod mod1,
        OblivionMod mod2)
    {
        // Set distinct properties on mod1's header
        mod1.ModHeader.Author = "TestAuthor";
        mod1.ModHeader.Description = "TestDescription";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new OblivionMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // ModHeader should come from first mod
        overlay.ModHeader.Author.ShouldBe("TestAuthor");
        overlay.ModHeader.Description.ShouldBe("TestDescription");
    }
}
