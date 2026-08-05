using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using FormList = Mutagen.Bethesda.Skyrim.FormList;
using MiscItem = Mutagen.Bethesda.Skyrim.MiscItem;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class MultiModFileSplitterTests
{
    [Theory, MutagenModAutoData]
    public void ClusterCachingTest(SkyrimMod inputMod)
    {
        // This doesn't actually assert anything is cached; it just creates a situation where it should, so it's debuggable.
        List<SkyrimMod> testMods = new();
        for (var i = 0; i < 10; i++)
        {
            var curMod = new SkyrimMod(ModKey.FromNameAndExtension("dummy_" + i + ".esp"), SkyrimRelease.SkyrimSE);
            testMods.Add(curMod);
        }
    
        List<FormList> subset01 = new();
        for (var i = 0; i < 5; i++)
        {
            var firstMod = testMods[0];
            var curFst = new FormList(firstMod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subset01.Add(curFst);

            for (var j = 0; j < 10; j++)
            {
                var curMod = testMods[j];
                var curMisc = new MiscItem(curMod);

                curFst.Items.Add(curMisc);
            }
            inputMod.FormLists.Add(curFst);
        }
    
        List<FormList> subset02 = new();
        for (var i = 0; i < 5; i++)
        {
            var firstMod = testMods[0];
            var curFst = new FormList(firstMod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subset02.Add(curFst);
    
            for (var j = 2; j < 8; j++)
            {
                var curMod = testMods[j];
                var curMisc = new MiscItem(curMod);
    
                curFst.Items.Add(curMisc);
            }
            inputMod.FormLists.Add(curFst);
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 10);
        outputList.Count.ShouldBe(1);
    }
    
    [Theory, MutagenModAutoData]
    public void GenerateClustersTest(SplitTestPayload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        for (uint i = 0; i < 5; i++)
        {
            payload.CreateFormListWithContents(7);
        }
    
        for (uint i = 0; i < 5; i++)
        {
            payload.CreateFormListWithContents(3);
        }

        var expectedEdids = payload.Mod.EnumerateMajorRecords()
            .Select(x => x.EditorID)
            .WhereNotNull()
            .ToHashSet();
        
        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(payload.Mod, 10);

        // now, we expect 5 clusters, each containing one of the 7-sized FLSTs and one of the 3-sized FLSTs
        outputList.Count.ShouldBe(5);
        
        foreach (var mod in outputList)
        {
            var modMasters = SplitTestUtil.ExtractMasters(mod, existingOutputDirectory, fileSystem);
            modMasters.Count.ShouldBeLessThanOrEqualTo(10);
            var recs = mod.EnumerateMajorRecords();
    
            foreach (var rec in recs)
            {
                var edid = rec.EditorID;
                if (edid != null)
                {
                    expectedEdids.Remove(edid);
                }
            }
        }

        expectedEdids.Count.ShouldBe(0, "Not all generated dummy records were found in the split files");
    }
    
    [Theory, MutagenModAutoData]
    public void OverridesArePreservedTest(SplitTestPayload tracker)
    {
        HashSet<string> edidsLocal = new();
        HashSet<string> edidsOverride = new();
        HashSet<string> edidsRemote = new();
    
        var localFlst = new FormList(tracker.Mod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "LocalFlst"
        };
        edidsLocal.Add(localFlst.EditorID);
    
        tracker.Mod.FormLists.Add(localFlst);
    
        for (var i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(tracker.Mod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = "localMisc1_" + i
            };
            edidsLocal.Add(curMisc.EditorID);
            tracker.Mod.MiscItems.Add(curMisc);
            localFlst.Items.Add(curMisc);
        }
    
        var localWithOverridesFlst = new FormList(tracker.Mod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "LocalWithOverridesFlst"
        };
        edidsLocal.Add(localWithOverridesFlst.EditorID);
    
        var otherFileModKey = new ModKey("Fallout4", ModType.Master);
        for (uint i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(new FormKey(otherFileModKey, 0x800 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = "overrideMisc_" + i
            };
            edidsOverride.Add(curMisc.EditorID);
            tracker.Mod.MiscItems.Add(curMisc);
            localWithOverridesFlst.Items.Add(curMisc);
        }
        tracker.Mod.FormLists.Add(localWithOverridesFlst);
    
        var overrideFlst = new FormList(new FormKey(otherFileModKey, 0x900), SkyrimRelease.SkyrimSE)
        {
            EditorID = tracker.GetNewEdid("overrideFlst")
        };
        edidsOverride.Add(overrideFlst.EditorID);
    
        for (var i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(tracker.Mod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = tracker.GetNewEdid("localMisc")
            };
            edidsLocal.Add(curMisc.EditorID);
            tracker.Mod.MiscItems.Add(curMisc);
            overrideFlst.Items.Add(curMisc);
        }
        for (uint i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(new FormKey(otherFileModKey, 0xa00 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = tracker.GetNewEdid("ovrMisc")
            };
            edidsOverride.Add(curMisc.EditorID);
            tracker.Mod.MiscItems.Add(curMisc);
            overrideFlst.Items.Add(curMisc);
        }

        // Referenced-only forms: added to the list but never to the mod, so they must not appear in the output.
        for (uint i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(new FormKey(otherFileModKey, 0xf00 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = tracker.GetNewEdid("remoteMisc", false)
            };
            overrideFlst.Items.Add(curMisc);
            edidsRemote.Add(curMisc.EditorID);
        }
        tracker.Mod.FormLists.Add(overrideFlst);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(tracker.Mod, 255);
        outputList.Count().ShouldBe(1);
        var mod = outputList.First();
        var recs = mod.EnumerateMajorRecords();
    
        foreach (var rec in recs)
        {
            var edid = rec.EditorID;
            if (edid == null) continue;
    
            if (edidsRemote.Contains(edid))
            {
                Assert.Fail("Form which should have been referenced only was found within the file. Edid: " + edid);
            }
            else if (edidsOverride.Contains(edid))
            {
                if (rec.FormKey.ModKey == mod.ModKey)
                {
                    Assert.Fail("Overridden form turned into local form. Edid: " + edid);
                }
                edidsOverride.Remove(edid);
            }
            else if (edidsLocal.Contains(edid))
            {
                if (rec.FormKey.ModKey != mod.ModKey)
                {
                    Assert.Fail("Local form turned into overridden form. Edid: " + edid);
                }
                edidsLocal.Remove(edid);
            }
        }
        edidsLocal.Count.ShouldBe(0, "Not all local forms were found in output file");
        edidsOverride.Count.ShouldBe(0, "Not all overridden forms were found in output file");
    }

    [Theory, MutagenModAutoData]
    public void Split_ThrowsWhenSingleRecordExceedsMasterLimit(SkyrimMod inputMod)
    {
        // A single record referencing more masters than the limit can't be split, so this must throw.
        var formList = inputMod.FormLists.AddNew();
        formList.EditorID = "MassiveFormList";

        for (uint i = 0; i < 20; i++)
        {
            var masterKey = new ModKey($"Master_{i}", ModType.Plugin);
            formList.Items.Add(new FormKey(masterKey, 0x800));
        }

        var sut = new MultiModFileSplitter();

        Should.Throw<TooManyMastersException>(() =>
        {
            sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 10);
        });
    }

    [Theory, MutagenModAutoData]
    public void DialogResponsesFromManyMods_ShouldSplitSuccessfully(
        SkyrimMod inputMod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A DialogTopic's EnumerateFormLinks() aggregates all its child response FormLinks, so a topic whose
        // responses come from 20 different mods appears to need 20+ masters. The splitter must split the
        // responses across files rather than treating the topic as one unsplittable record.
        var topicModKey = new ModKey("Skyrim", ModType.Master);
        var topic = new DialogTopic(new FormKey(topicModKey, 0x100), SkyrimRelease.SkyrimSE)
        {
            EditorID = "TestTopic"
        };

        for (uint i = 0; i < 20; i++)
        {
            var responseModKey = new ModKey($"ResponseMod_{i}", ModType.Plugin);
            var response = new DialogResponses(new FormKey(responseModKey, 0x800 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = $"Response_{i}"
            };
            topic.Responses.Add(response);
        }

        inputMod.DialogTopics.Add(topic);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 10);

        var allResponses = outputList
            .SelectMany(m => m.EnumerateMajorRecords<IDialogResponsesGetter>())
            .ToList();
        allResponses.Count.ShouldBe(20);

        foreach (var mod in outputList)
        {
            var modMasters = SplitTestUtil.ExtractMasters(mod, existingOutputDirectory, fileSystem);
            modMasters.Count.ShouldBeLessThanOrEqualTo(10);
        }
    }

    [Theory, MutagenModAutoData]
    public void DialogResponsesOverrides_ShouldSplitAcrossFiles(
        SkyrimMod inputMod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var topicModKey = new ModKey("Skyrim", ModType.Master);
        var topic = new DialogTopic(new FormKey(topicModKey, 0x100), SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Quest = new FormKey(topicModKey, 0x200).ToNullableLink<IQuestGetter>()
        };

        for (uint i = 0; i < 15; i++)
        {
            var modKey = new ModKey($"Mod_{i}", ModType.Plugin);
            var response = new DialogResponses(new FormKey(modKey, 0x800 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = $"Override_{i}"
            };
            topic.Responses.Add(response);
        }

        inputMod.DialogTopics.Add(topic);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 5);

        outputList.Count.ShouldBeGreaterThan(1);

        var allResponses = outputList
            .SelectMany(m => m.EnumerateMajorRecords<IDialogResponsesGetter>())
            .ToList();
        allResponses.Count.ShouldBe(15);

        foreach (var mod in outputList)
        {
            var modMasters = SplitTestUtil.ExtractMasters(mod, existingOutputDirectory, fileSystem);
            modMasters.Count.ShouldBeLessThanOrEqualTo(5);
        }
    }

    [Theory, MutagenModAutoData]
    public void MultipleDialogTopics_ResponsesFromManyMods_ShouldSplit(
        SkyrimMod inputMod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        for (uint t = 0; t < 3; t++)
        {
            var topicModKey = new ModKey("Skyrim", ModType.Master);
            var topic = new DialogTopic(new FormKey(topicModKey, 0x100 + t), SkyrimRelease.SkyrimSE)
            {
                EditorID = $"Topic_{t}"
            };

            for (uint i = 0; i < 8; i++)
            {
                var modKey = new ModKey($"Mod_{t}_{i}", ModType.Plugin);
                var response = new DialogResponses(new FormKey(modKey, 0x800 + i), SkyrimRelease.SkyrimSE)
                {
                    EditorID = $"Response_{t}_{i}"
                };
                topic.Responses.Add(response);
            }

            inputMod.DialogTopics.Add(topic);
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 10);

        var allResponses = outputList
            .SelectMany(m => m.EnumerateMajorRecords<IDialogResponsesGetter>())
            .ToList();
        allResponses.Count.ShouldBe(24);

        foreach (var mod in outputList)
        {
            var modMasters = SplitTestUtil.ExtractMasters(mod, existingOutputDirectory, fileSystem);
            modMasters.Count.ShouldBeLessThanOrEqualTo(10);
        }
    }

    [Theory, MutagenModAutoData]
    public void NonFirstFragmentReferencingBase_StaysWithinMasterLimit(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A fragment that links to a record created in the base mod must carry the base
        // file as a real master, and that base master has to count against the fragment's budget. 
        const int limit = 5;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);

        var baseAnchor = mod.MiscItems.AddNew();
        baseAnchor.EditorID = "BaseAnchor";

        for (int i = 0; i < 12; i++)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = $"List_{i}";
            flst.Items.Add(new FormKey(new ModKey($"Ext{i:D2}", ModType.Plugin), 0x800));
            flst.Items.Add(baseAnchor.FormKey); // links back to a base-created record
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        outputList.Count.ShouldBeGreaterThan(1);
        foreach (var frag in outputList)
        {
            SplitTestUtil.WrittenMasterCount(frag, existingOutputDirectory, fileSystem).ShouldBeLessThanOrEqualTo(limit);
        }

        var lists = outputList.SelectMany(m => m.EnumerateMajorRecords<IFormListGetter>()).ToList();
        lists.Count.ShouldBe(12);
        outputList.SelectMany(m => m.EnumerateMajorRecords<IMiscItemGetter>()).Count().ShouldBe(1);
    }

    [Theory, MutagenModAutoData]
    public void BaseFragmentReferencingBase_UsesFullLimit(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // The first fragment keeps the base filename, so its records' base links resolve to its own key and
        // cost no masters. It must therefore be allowed to fill to the full limit even when every record
        // references the base.  This input fits in a single fragment and must not be split.
        const int limit = 5;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);

        var baseAnchor = mod.MiscItems.AddNew();
        baseAnchor.EditorID = "BaseAnchor";

        for (int i = 0; i < 3; i++)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = $"List_{i}";
            for (int e = 0; e < limit; e++)
            {
                flst.Items.Add(new FormKey(new ModKey($"Shared{e}", ModType.Plugin), 0x800));
            }
            flst.Items.Add(baseAnchor.FormKey);
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        outputList.Count.ShouldBe(1);
        SplitTestUtil.WrittenMasterCount(outputList[0], existingOutputDirectory, fileSystem).ShouldBe(limit);
    }

    /// <summary>
    /// Asserts the split output is internally consistent: within the master limit, every reference into the
    /// split family resolves to a record that actually exists, and no fragment references a later sibling
    /// (masters must load first).
    /// </summary>
    private static void AssertSplitValid(
        IReadOnlyList<ISkyrimMod> output, int limit, DirectoryPath outputDir, IFileSystem fileSystem)
    {
        var indexByKey = new Dictionary<ModKey, int>();
        for (int i = 0; i < output.Count; i++) indexByKey[output[i].ModKey] = i;
        var present = output
            .Select(m => new HashSet<FormKey>(m.EnumerateMajorRecords().Select(r => r.FormKey)))
            .ToList();

        for (int i = 0; i < output.Count; i++)
        {
            SplitTestUtil.WrittenMasterCount(output[i], outputDir, fileSystem).ShouldBeLessThanOrEqualTo(limit);
            foreach (var rec in output[i].EnumerateMajorRecords())
            {
                foreach (var fl in rec.EnumerateFormLinks())
                {
                    if (fl.FormKey.IsNull) continue;
                    if (!indexByKey.TryGetValue(fl.FormKey.ModKey, out var targetIdx)) continue;
                    present[targetIdx].ShouldContain(fl.FormKey,
                        $"dangling reference {fl.FormKey} from fragment {i} ({output[i].ModKey})");
                    targetIdx.ShouldBeLessThanOrEqualTo(i,
                        $"forward reference from fragment {i} to later fragment {targetIdx}");
                }
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void Split_ReferencesToCreatedRecord_ResolveAcrossFragments(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A single created record referenced by many records that must split across fragments. When the splitter
        // re-keys the referenced record into whichever fragment holds it, every referencing link must be remapped to
        // that new identity, so every reference still resolves after the split.
        const int limit = 4;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);

        var shared = mod.MiscItems.AddNew();
        shared.EditorID = "SharedTarget";

        for (int i = 0; i < 10; i++)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = $"List_{i}";
            for (int e = 0; e < 3; e++)
            {
                flst.Items.Add(new FormKey(new ModKey($"Ext{i:D2}_{e}", ModType.Plugin), 0x800));
            }
            flst.Items.Add(shared.FormKey); // reference the shared created record
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        outputList.Count.ShouldBeGreaterThan(1);
        AssertSplitValid(outputList, limit, existingOutputDirectory, fileSystem);

        // The shared record exists exactly once, and every list still references whatever it became.
        var sharedCopies = outputList.SelectMany(m => m.EnumerateMajorRecords<IMiscItemGetter>()).ToList();
        sharedCopies.Count.ShouldBe(1);
        var sharedKey = sharedCopies[0].FormKey;
        var listsReferencingShared = outputList
            .SelectMany(m => m.EnumerateMajorRecords<IFormListGetter>())
            .Count(l => l.Items.Any(item => item.FormKey == sharedKey));
        listsReferencingShared.ShouldBe(10);
    }

    [Theory, MutagenModAutoData]
    public void Split_DanglingSelfReference_BudgetsBaseFragmentAsMaster(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A link into the mod's own space that no record occupies is assumed to live in the first fragment. The
        // writer masters the base file for such a link like any other, so every later fragment carrying one must
        // budget a master slot for it. Two externals per list so several pack into a fragment; without the budget
        // a packed fragment fills to the limit on externals alone and then exceeds it once the base is added.
        const int limit = 4;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);
        var dangling = new FormKey(mod.ModKey, 0x999999);

        for (int i = 0; i < 8; i++)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = $"List_{i}";
            for (int e = 0; e < 2; e++)
            {
                flst.Items.Add(new FormKey(new ModKey($"Ext{i:D2}_{e}", ModType.Plugin), 0x800));
            }
            flst.Items.Add(dangling);
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        outputList.Count.ShouldBeGreaterThan(1);

        for (int i = 0; i < outputList.Count; i++)
        {
            var masters = SplitTestUtil.ExtractMasters(outputList[i], existingOutputDirectory, fileSystem);
            masters.Count.ShouldBeLessThanOrEqualTo(limit);
            if (i > 0)
            {
                masters.ShouldContain(mod.ModKey,
                    $"fragment {i} carries a dangling link to the base file, so it must master it");
            }
        }

        // The dangling link is left pointing at the base fragment rather than remapped onto a sibling.
        var lists = outputList.SelectMany(m => m.EnumerateMajorRecords<IFormListGetter>()).ToList();
        lists.Count.ShouldBe(8);
        lists.ShouldAllBe(l => l.Items.Any(x => x.FormKey == dangling));
    }

    [Theory, MutagenModAutoData]
    public void Split_LaterFragmentMastersEarlierSplitSibling(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A daisy chain of referencing records all in different fragments must point to their original references
        // properly after the split
        const int limit = 4;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);

        FormList MakeChainLink(string id, int extBucket, FormKey? childCreated)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = id;
            for (int e = 0; e < 3; e++)
            {
                flst.Items.Add(new FormKey(new ModKey($"Ext{extBucket}_{e}", ModType.Plugin), 0x800));
            }
            if (childCreated.HasValue) flst.Items.Add(childCreated.Value);
            return flst;
        }

        var a2 = MakeChainLink("A2", 2, null);
        var a1 = MakeChainLink("A1", 1, a2.FormKey);
        var a0 = MakeChainLink("A0", 0, a1.FormKey);
        var r = MakeChainLink("R", 9, a0.FormKey);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        AssertSplitValid(outputList, limit, existingOutputDirectory, fileSystem);

        // At least one non-base split file is used as a master by a later fragment.
        var indexByKey = new Dictionary<ModKey, int>();
        for (int i = 0; i < outputList.Count; i++) indexByKey[outputList[i].ModKey] = i;

        bool siblingMastering = false;
        for (int i = 0; i < outputList.Count; i++)
        {
            foreach (var master in SplitTestUtil.ExtractMasters(outputList[i], existingOutputDirectory, fileSystem))
            {
                if (indexByKey.TryGetValue(master, out var mi) && mi > 0 && mi < i)
                {
                    siblingMastering = true;
                }
            }
        }
        siblingMastering.ShouldBeTrue("expected a later fragment to master an earlier non-base split sibling");
    }

    [Theory, MutagenModAutoData]
    public void Split_CreatedRecordRef_NeverPointsToLaterFragment(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Enforce that referenced records come in the same or earlier fragments
        const int limit = 4;
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Synthesis.esp"), SkyrimRelease.SkyrimSE);

        // Create a chain of created records referencing each other.  Each list also references other records so that
        // each patch can only contain one of them
        FormKey? next = null;
        for (int k = 5; k >= 0; k--)
        {
            var flst = mod.FormLists.AddNew();
            flst.EditorID = $"Link_{k}";
            for (int e = 0; e < 3; e++)
            {
                flst.Items.Add(new FormKey(new ModKey($"Ext{k}_{e}", ModType.Plugin), 0x800));
            }
            if (next.HasValue) flst.Items.Add(next.Value);
            next = flst.FormKey;
        }

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(mod, limit);

        outputList.Count.ShouldBeGreaterThan(1);

        // Load position of each fragment, and the set of FormKeys each fragment actually defines.
        var loadPositionByKey = new Dictionary<ModKey, int>();
        for (int i = 0; i < outputList.Count; i++) loadPositionByKey[outputList[i].ModKey] = i;
        var definedIn = outputList
            .Select(m => new HashSet<FormKey>(m.EnumerateMajorRecords().Select(r => r.FormKey)))
            .ToList();

        for (int i = 0; i < outputList.Count; i++)
        {
            foreach (var rec in outputList[i].EnumerateMajorRecords())
            {
                foreach (var link in rec.EnumerateFormLinks())
                {
                    if (link.FormKey.IsNull) continue;
                    if (!loadPositionByKey.TryGetValue(link.FormKey.ModKey, out var targetFragment)) continue;

                    definedIn[targetFragment].ShouldContain(link.FormKey,
                        $"{rec.FormKey} in fragment {i} ({outputList[i].ModKey.FileName}) references {link.FormKey}, " +
                        $"which is absent from the fragment its key names — an unremapped, broken reference.");
                    targetFragment.ShouldBeLessThanOrEqualTo(i,
                        $"reverse-master: {rec.FormKey} in fragment {i} ({outputList[i].ModKey.FileName}) references " +
                        $"{link.FormKey} defined in later fragment {targetFragment} ({outputList[targetFragment].ModKey.FileName}).");
                }
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void Split_WorldspaceSharedParent_ReservesPulledInMasterPerFragment(
        SkyrimMod inputMod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // A shared Worldspace parent (override of Skyrim.esm) carries an external Water link. Its placed
        // objects split across fragments, and each fragment rebuilds Worldspace->Block->SubBlock->Cell around
        // its objects via GetOrAddAsOverride, so every fragment declares the worldspace's water master.
        // That master required by the water record must be taken into account
        const int limit = 5;
        var skyrimKey = new ModKey("Skyrim", ModType.Master);
        var waterModKey = new ModKey("WaterMod", ModType.Plugin);

        var worldspace = new Worldspace(new FormKey(skyrimKey, 0x3C), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Tamriel",
            Water = new FormKey(waterModKey, 0x800).ToNullableLink<IWaterGetter>()
        };
        var cell = new Cell(new FormKey(skyrimKey, 0x1000), SkyrimRelease.SkyrimSE)
        {
            EditorID = "ExtCell",
            Grid = new CellGrid { Point = new P2Int(0, 0) }
        };
        worldspace.AddCell(cell);

        for (uint i = 0; i < 15; i++)
        {
            var placedModKey = new ModKey($"Placed_{i:D2}", ModType.Plugin);
            cell.Persistent.Add(new PlacedObject(new FormKey(placedModKey, 0x800 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = $"Ref_{i}"
            });
        }
        inputMod.Worldspaces.Add(worldspace);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, limit);

        outputList.Count.ShouldBeGreaterThan(1);
        foreach (var frag in outputList)
        {
            var masters = SplitTestUtil.ExtractMasters(frag, existingOutputDirectory, fileSystem);
            masters.Count.ShouldBeLessThanOrEqualTo(limit);
            masters.ShouldContain(waterModKey,
                $"fragment {frag.ModKey.FileName} pulls in the shared worldspace but did not declare its water master");
        }

        outputList.SelectMany(m => m.EnumerateMajorRecords<IPlacedObjectGetter>()).Count().ShouldBe(15);
    }

    [Theory, MutagenModAutoData]
    public void Split_DialogTopicSharedParent_ReservesTopicMastersPerFragment(
        SkyrimMod inputMod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Companion to the worldspace case with a shallow (direct) parent. An override DialogTopic whose Quest
        // link targets a distinct external master is copied into every fragment that holds some of its
        // responses, so every fragment must reserve the quest master alongside the topic's own master.
        const int limit = 5;
        var topicModKey = new ModKey("Skyrim", ModType.Master);
        var questModKey = new ModKey("QuestMod", ModType.Plugin);

        var topic = new DialogTopic(new FormKey(topicModKey, 0x100), SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Quest = new FormKey(questModKey, 0x200).ToNullableLink<IQuestGetter>()
        };
        for (uint i = 0; i < 15; i++)
        {
            var modKey = new ModKey($"Resp_{i:D2}", ModType.Plugin);
            topic.Responses.Add(new DialogResponses(new FormKey(modKey, 0x800 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = $"Override_{i}"
            });
        }
        inputMod.DialogTopics.Add(topic);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, limit);

        outputList.Count.ShouldBeGreaterThan(1);
        foreach (var frag in outputList)
        {
            var masters = SplitTestUtil.ExtractMasters(frag, existingOutputDirectory, fileSystem);
            masters.Count.ShouldBeLessThanOrEqualTo(limit);
            masters.ShouldContain(questModKey);
        }

        outputList.SelectMany(m => m.EnumerateMajorRecords<IDialogResponsesGetter>()).Count().ShouldBe(15);
    }
}