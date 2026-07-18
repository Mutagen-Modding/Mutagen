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
}