using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Plugins.Masters;
using Noggog;

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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

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

        // Define merged masters
        var mergedMasters = new List<IMasterReferenceGetter>
        {
            new MasterReference { Master = master1 },
            new MasterReference { Master = master2 },
            new MasterReference { Master = master3 }
        };

        // Create overlay
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            mergedMasters);

        // Verify masters are in load order
        overlay.MasterReferences.Count.ShouldBe(3);
        overlay.MasterReferences[0].Master.ShouldBe(master1);
        overlay.MasterReferences[1].Master.ShouldBe(master2);
        overlay.MasterReferences[2].Master.ShouldBe(master3);
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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should return the maximum NextFormID from all mods to avoid FormID collisions
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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

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
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Should enumerate all records from all mods
        var allRecords = overlay.EnumerateMajorRecords().ToList();
        allRecords.Count.ShouldBe(4);
        allRecords.ShouldContain(r => r.EditorID == "FormList1");
        allRecords.ShouldContain(r => r.EditorID == "Armor1");
        allRecords.ShouldContain(r => r.EditorID == "FormList2");
        allRecords.ShouldContain(r => r.EditorID == "Weapon1");
    }

    [Theory, MutagenModAutoData]
    public void GetRecordCount_DeduplicatesAcrossMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Add records to both mods
        mod1.FormLists.AddNew();
        mod1.FormLists.AddNew();
        mod1.Armors.AddNew();

        mod2.FormLists.AddNew();
        mod2.Weapons.AddNew();

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // GetRecordCount = distinct major records + one per non-empty top-level group.
        // The two mods collectively populate 3 groups: FormLists, Armors, Weapons.
        var distinctFormKeys = mod1.EnumerateMajorRecords()
            .Concat(mod2.EnumerateMajorRecords())
            .Select(r => r.FormKey)
            .Distinct()
            .Count();
        const uint nonEmptyTopLevelGroups = 3;
        overlay.GetRecordCount().ShouldBe((uint)distinctFormKeys + nonEmptyTopLevelGroups);
    }

    [Theory, MutagenModAutoData]
    public void EnumerateAssetLinks_YieldsAllForDisjointMods(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Each mod gets a unique Quest with a unique script → all inferred assets should show.
        mod1.Quests.Add(new Quest(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            VirtualMachineAdapter = new QuestAdapter
            {
                Scripts = new ExtendedList<ScriptEntry> { new() { Name = "ScriptA" } }
            }
        });
        mod2.Quests.Add(new Quest(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            VirtualMachineAdapter = new QuestAdapter
            {
                Scripts = new ExtendedList<ScriptEntry> { new() { Name = "ScriptB" } }
            }
        });

        var overlay = new SkyrimMultiModOverlay(
            new ModKey("TestMerged", ModType.Plugin),
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        var paths = overlay.EnumerateAssetLinks(AssetLinkQuery.Inferred, null, null)
            .Select(a => a.GivenPath)
            .ToList();
        paths.ShouldContain(p => p.Contains("ScriptA"));
        paths.ShouldContain(p => p.Contains("ScriptB"));
    }

    [Theory, MutagenModAutoData]
    public void EnumerateAssetLinks_PreservesNestedRecordsAcrossOverriddenParent(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // Same DialogTopic FormKey across mods, but each mod contributes a distinct
        // nested DialogResponse (different FormKeys). Both nested responses survive
        // the overlay, so their asset links must all be yielded.
        var sharedTopicKey = mod1.GetNextFormKey();
        var response1Key = mod1.GetNextFormKey();
        var response2Key = mod2.GetNextFormKey();

        mod1.DialogTopics.Add(new DialogTopic(sharedTopicKey, SkyrimRelease.SkyrimSE)
        {
            Responses =
            {
                new DialogResponses(response1Key, SkyrimRelease.SkyrimSE)
                {
                    VirtualMachineAdapter = new DialogResponsesAdapter
                    {
                        Scripts = new ExtendedList<ScriptEntry> { new() { Name = "ScriptResp1" } }
                    }
                }
            }
        });
        mod2.DialogTopics.Add(new DialogTopic(sharedTopicKey, SkyrimRelease.SkyrimSE)
        {
            Responses =
            {
                new DialogResponses(response2Key, SkyrimRelease.SkyrimSE)
                {
                    VirtualMachineAdapter = new DialogResponsesAdapter
                    {
                        Scripts = new ExtendedList<ScriptEntry> { new() { Name = "ScriptResp2" } }
                    }
                }
            }
        });

        var overlay = new SkyrimMultiModOverlay(
            new ModKey("TestMerged", ModType.Plugin),
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        var paths = overlay.EnumerateAssetLinks(AssetLinkQuery.Inferred, null, null)
            .Select(a => a.GivenPath)
            .ToList();
        paths.ShouldContain(p => p.Contains("ScriptResp1"));
        paths.ShouldContain(p => p.Contains("ScriptResp2"));
    }

    [Theory, MutagenModAutoData]
    public void EnumerateAssetLinks_DoesNotDuplicateSingleRecord(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        // A single record in one mod should yield each of its inferred asset links exactly once.
        mod1.Quests.Add(new Quest(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            VirtualMachineAdapter = new QuestAdapter
            {
                Scripts = new ExtendedList<ScriptEntry> { new() { Name = "UniqueScript" } }
            }
        });

        var overlay = new SkyrimMultiModOverlay(
            new ModKey("TestMerged", ModType.Plugin),
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        var paths = overlay.EnumerateAssetLinks(AssetLinkQuery.Inferred, null, null)
            .Select(a => a.GivenPath)
            .Where(p => p.Contains("UniqueScript"))
            .ToList();
        // Script produces one compiled (.pex) and one source (.psc) link. No more.
        paths.Count.ShouldBe(2);
    }

    [Theory, MutagenModAutoData]
    public void EmptyMods_CreatesEmptyOverlay(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        // Empty mods should result in empty groups
        overlay.FormLists.Count.ShouldBe(0);
        overlay.Armors.Count.ShouldBe(0);
        overlay.EnumerateMajorRecords().ShouldBeEmpty();
        overlay.GetRecordCount().ShouldBe(0u);
    }

    [Theory, MutagenModAutoData]
    public void ModHeader_MatchingFieldsExposed(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        mod1.ModHeader.Author = "TestAuthor";
        mod1.ModHeader.Description = "TestDescription";
        mod2.ModHeader.Author = "TestAuthor";
        mod2.ModHeader.Description = "TestDescription";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        var overlay = new SkyrimMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>());

        overlay.ModHeader.Author.ShouldBe("TestAuthor");
        overlay.ModHeader.Description.ShouldBe("TestDescription");
    }

    [Theory, MutagenModAutoData]
    public void ModHeader_MismatchingFieldsThrow(
        SkyrimMod mod1,
        SkyrimMod mod2)
    {
        mod1.ModHeader.Author = "AuthorA";
        mod2.ModHeader.Author = "AuthorB";

        var targetModKey = new ModKey("TestMerged", ModType.Plugin);
        Should.Throw<System.IO.InvalidDataException>(() => new SkyrimMultiModOverlay(
            targetModKey,
            new[] { mod1, mod2 },
            Array.Empty<IMasterReferenceGetter>()));
    }
}
