using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class SkyrimMultiModOverlayTests
{
    [Theory, MutagenModAutoData]
    public void BasicMerge_TwoModsWithDifferentFormLists(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Create FormLists in mod1
        var flst1 = mod1.FormLists.AddNew();
        flst1.EditorID = "FormList1";

        var flst2 = mod1.FormLists.AddNew();
        flst2.EditorID = "FormList2";

        // Create FormLists in mod2
        var flst3 = mod2.FormLists.AddNew();
        flst3.EditorID = "FormList3";

        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Verify merged view
        overlay.FormLists.Count.ShouldBe(3);
        overlay.FormLists.ShouldContain(f => f.EditorID == "FormList1");
        overlay.FormLists.ShouldContain(f => f.EditorID == "FormList2");
        overlay.FormLists.ShouldContain(f => f.EditorID == "FormList3");

        // Verify ModKey
        overlay.ModKey.ShouldBe(targetModKey);
    }

    [Theory, MutagenModAutoData]
    public void DuplicateFormKey_ThrowsException(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Create same FormKey in both mods
        var formKey = mod1.GetNextFormKey();
        var flst1 = mod1.FormLists.AddNew(formKey);
        flst1.EditorID = "FormList1";

        var flst2 = mod2.FormLists.AddNew(formKey);
        flst2.EditorID = "FormList2";

        // Create overlay - this should succeed
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Accessing the FormLists group should throw when it tries to cache and detects the duplicate
        Should.Throw<SplitModException>(() =>
        {
            var count = overlay.FormLists.Count;
        }).Message.ShouldContain(formKey.ToString());
    }

    [Theory, MutagenModAutoData]
    public void MasterListMerging_OrderedByLoadOrder(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Set up masters for mod1
        var master1 = new ModKey("Skyrim", ModType.Master);
        var master2 = new ModKey("Update", ModType.Master);
        mod1.ModHeader.MasterReferences.Add(new MasterReference { Master = master1 });
        mod1.ModHeader.MasterReferences.Add(new MasterReference { Master = master2 });

        // Set up masters for mod2 (different master)
        var master3 = new ModKey("Dawnguard", ModType.Master);
        mod2.ModHeader.MasterReferences.Add(new MasterReference { Master = master3 });

        // Define load order
        var loadOrder = new List<IModMasterStyledGetter>
        {
            new KeyedMasterStyle(master1, MasterStyle.Full),
            new KeyedMasterStyle(master2, MasterStyle.Full),
            new KeyedMasterStyle(master3, MasterStyle.Full)
        };

        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            loadOrder);

        // Verify masters are in load order
        overlay.Masters.Count.ShouldBe(3);
        overlay.Masters[0].ModKey.ShouldBe(master1);
        overlay.Masters[1].ModKey.ShouldBe(master2);
        overlay.Masters[2].ModKey.ShouldBe(master3);
    }

    [Theory, MutagenModAutoData]
    public void DeepNestedRecords_CellsAndPlacedObjects(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Cells property should now work (returns ISkyrimListGroupGetter<ICellBlockGetter>)
        var cells = overlay.Cells;
        cells.ShouldNotBeNull();

        // If mods are empty, cells should also be empty
        cells.Count.ShouldBe(0);
    }

    [Theory, MutagenModAutoData]
    public void NextFormID_ReturnsMaxFromAllMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Add some records to each mod so they have different NextFormIDs
        // AutoFixture creates mods with unique ModKeys, so adding records will give different FormIDs
        for (int i = 0; i < 10; i++)
        {
            mod1.FormLists.AddNew();
        }

        for (int i = 0; i < 20; i++)
        {
            mod2.FormLists.AddNew();
        }

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Should return the maximum NextFormID from all mods
        var expected = Math.Max(mod1.ModHeader.Stats.NextFormID, mod2.ModHeader.Stats.NextFormID);
        overlay.NextFormID.ShouldBe(expected);
    }

    [Theory, MutagenModAutoData]
    public void RecordAccessByFormKey_WorksAcrossMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Create records in both mods
        var flst1 = mod1.FormLists.AddNew();
        flst1.EditorID = "FromMod1";
        var formKey1 = flst1.FormKey;

        var flst2 = mod2.FormLists.AddNew();
        flst2.EditorID = "FromMod2";
        var formKey2 = flst2.FormKey;

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Should be able to access records from both mods by FormKey
        overlay.FormLists.TryGetValue(formKey1, out var retrieved1).ShouldBeTrue();
        retrieved1.EditorID.ShouldBe("FromMod1");

        overlay.FormLists.TryGetValue(formKey2, out var retrieved2).ShouldBeTrue();
        retrieved2.EditorID.ShouldBe("FromMod2");
    }

    [Theory, MutagenModAutoData]
    public void EnumerateMajorRecords_IncludesAllMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Create different types of records
        mod1.FormLists.AddNew().EditorID = "FormList1";
        mod1.Armors.AddNew().EditorID = "Armor1";

        mod2.FormLists.AddNew().EditorID = "FormList2";
        mod2.Weapons.AddNew().EditorID = "Weapon1";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Should enumerate all records from all mods
        var allRecords = overlay.EnumerateMajorRecords().ToList();
        allRecords.Count.ShouldBe(4);
        allRecords.ShouldContain(r => r.EditorID == "FormList1");
        allRecords.ShouldContain(r => r.EditorID == "Armor1");
        allRecords.ShouldContain(r => r.EditorID == "FormList2");
        allRecords.ShouldContain(r => r.EditorID == "Weapon1");
    }

    [Theory, MutagenModAutoData]
    public void GetRecordCount_SumsAllMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Get initial counts
        var initialCount1 = mod1.GetRecordCount();
        var initialCount2 = mod2.GetRecordCount();

        // Add records to both mods
        mod1.FormLists.AddNew();
        mod1.FormLists.AddNew();
        mod1.Armors.AddNew();

        mod2.FormLists.AddNew();
        mod2.Weapons.AddNew();

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Should sum all records from all mods (initial + newly added)
        var expectedCount = mod1.GetRecordCount() + mod2.GetRecordCount();
        overlay.GetRecordCount().ShouldBe(expectedCount);
    }

    [Theory, MutagenModAutoData]
    public void EmptyMods_CreatesEmptyOverlay(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // Empty mods should result in empty groups
        overlay.FormLists.Count.ShouldBe(0);
        overlay.Armors.Count.ShouldBe(0);
        overlay.EnumerateMajorRecords().ShouldBeEmpty();
        overlay.GetRecordCount().ShouldBe(0u);
    }

    [Theory, MutagenModAutoData]
    public void ModHeader_TakenFromFirstMod(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Set distinct properties on mod1's header
        mod1.ModHeader.Author = "TestAuthor";
        mod1.ModHeader.Description = "TestDescription";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            GameRelease.SkyrimSE,
            new[] { mod1, mod2 },
            Array.Empty<IModMasterStyledGetter>());

        // ModHeader should come from first mod
        overlay.ModHeader.Author.ShouldBe("TestAuthor");
        overlay.ModHeader.Description.ShouldBe("TestDescription");
    }
}
