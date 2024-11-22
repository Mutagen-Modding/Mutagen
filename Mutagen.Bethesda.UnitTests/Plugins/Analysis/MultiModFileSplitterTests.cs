using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;
using FormList = Mutagen.Bethesda.Skyrim.FormList;
using MiscItem = Mutagen.Bethesda.Skyrim.MiscItem;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class MultiModFileSplitterTests
{
    #region Util
    
    // just to be able to create unique edids
    private int lastEdidIndex = 0;

    /// <summary>
    /// Stores EDIDs of generated forms in the input class, to be able to track them in the generated files
    /// </summary>
    private HashSet<string> expectedEdids = new();
    
    /// <summary>
    /// Generates MISC items from NOT within the current file
    /// </summary>
    private void FillFormListWithRemoteRecords(FormList flst, int numFiles, string fileNameBase = "foobar_", uint startFormId = 0x666)
    {
        for (uint i = 0; i < numFiles; i++)
        {
            var curFileName = fileNameBase + i;
            var dummyItem = new MiscItem(new FormKey(new ModKey(curFileName, ModType.Plugin), startFormId + i), SkyrimRelease.SkyrimSE);
            dummyItem.EditorID = getNewEdid("TestMisc", false);
            dummyItem.Name = "Test Item from file " + curFileName;
    
            flst.Items.Add(dummyItem);
        }
    }
    
    private FormList CreateFormListWithContents(FormKey key, int numFiles, string fileNameBase, uint startFormId)
    {
        var result = new FormList(key, SkyrimRelease.SkyrimSE);
        result.EditorID = getNewEdid("testFormList_" + fileNameBase);
    
        FillFormListWithRemoteRecords(result, numFiles, fileNameBase, startFormId);
    
        return result;
    }
    private string getNewEdid(string baseEdid, bool addToExpected = true)
    {
        var newEdid = baseEdid + lastEdidIndex;
        lastEdidIndex++;
        if (addToExpected)
        {
            expectedEdids.Add(newEdid);
        }
        return newEdid;
    }
    
    private static HashSet<ModKey> GetAllMasters(IModGetter mod)
    {
        var recs = mod.EnumerateMajorRecords();
        var result = new HashSet<ModKey>();
    
        foreach (var majorRecord in recs)
        {
            if (mod.ModKey != majorRecord.FormKey.ModKey)
            {
                result.Add(majorRecord.FormKey.ModKey);
            }
            var formLinks = majorRecord.EnumerateFormLinks();
            foreach (var formLink in formLinks)
            {
                result.Add(formLink.FormKey.ModKey);
            }
        }
    
        return result;
    }
    #endregion

    [Theory, MutagenModAutoData]
    public void ClusterCachingTest(SkyrimMod inputMod)
    {
        // This doesn't actually test whenever anything is cached, but it creates a situation where it should, so it's debuggable.
        // To actually test this, we wouild need to inject a cache object, or so
        
        // create several forms with the same masterlists, and make sure this doesn't break anything
        List<SkyrimMod> testMods = new();
        for (var i = 0; i < 10; i++)
        {
            var curMod = new SkyrimMod(ModKey.FromNameAndExtension("dummy_" + i + ".esp"), SkyrimRelease.SkyrimSE);
            testMods.Add(curMod);
        }
    
        List<FormList> subset01 = new();
        for (var i = 0; i < 5; i++)
        {
            // the formlist should contain the first 10 files and go into the first cluster
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
    
        // make more formlists, with a smaller subset
        List<FormList> subset02 = new();
        for (var i = 0; i < 5; i++)
        {
            // the formlist should contain the first 10 files and go into the first cluster
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
        outputList.Count.Should().Be(1);
    }
    
    [Theory, MutagenModAutoData]
    public void GenerateClustersTest(SkyrimMod inputMod)
    {
        for (uint i = 0; i < 5; i++)
        {
            var flst = CreateFormListWithContents(new FormKey(inputMod.ModKey, 0x23 + i), 7, "dummyFile_" + i + "_", 0x666 + i);
    
            inputMod.FormLists.Add(flst);
        }
    
        for (uint i = 0; i < 5; i++)
        {
            var flst = CreateFormListWithContents(new FormKey(inputMod.ModKey, 0x523 + i), 3, "dummyFile2_" + i + "_", 0x777 + i);
    
            inputMod.FormLists.Add(flst);
        }

        var expectedEdids = inputMod.EnumerateMajorRecords()
            .Select(x => x.EditorID)
            .NotNull()
            .ToHashSet();
        
        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 10);

        // now, we expect 5 clusters, each containing one of the 7-sized FLSTs and one of the 3-sized FLSTs
        outputList.Count.Should().Be(5);
        
        foreach (var mod in outputList)
        {
            var modMasters = GetAllMasters(mod);
            modMasters.Count.Should().BeLessOrEqualTo(10);
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

        expectedEdids.Count.Should().Be(0, "Not all generated dummy records were found in the split files");
    }
    
    [Theory, MutagenModAutoData]
    public void OverridesArePreservedTest(SkyrimMod inputMod)
    {
        HashSet<string> edidsLocal = new();
        HashSet<string> edidsOverride = new();
        HashSet<string> edidsRemote = new();
    
        var localFlst = new FormList(inputMod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "LocalFlst"
        };
        edidsLocal.Add(localFlst.EditorID);
    
        inputMod.FormLists.Add(localFlst);
    
        for (var i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(inputMod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = "localMisc1_" + i
            };
            edidsLocal.Add(curMisc.EditorID);
            inputMod.MiscItems.Add(curMisc);
            localFlst.Items.Add(curMisc);
        }
    
        // now a local list, with some overrides
        var localWithOverridesFlst = new FormList(inputMod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
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
            inputMod.MiscItems.Add(curMisc);
            localWithOverridesFlst.Items.Add(curMisc);
        }
        inputMod.FormLists.Add(localWithOverridesFlst);
    
        // now a formlist which is an override
        var overrideFlst = new FormList(new FormKey(otherFileModKey, 0x900), SkyrimRelease.SkyrimSE)
        {
            EditorID = getNewEdid("overrideFlst")
        };
        edidsOverride.Add(overrideFlst.EditorID);
    
        // add some local records
        for (var i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(inputMod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = getNewEdid("localMisc")
            };
            edidsLocal.Add(curMisc.EditorID);
            inputMod.MiscItems.Add(curMisc);
            overrideFlst.Items.Add(curMisc);
        }
        // and some overrides
        for (uint i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(new FormKey(otherFileModKey, 0xa00 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = getNewEdid("ovrMisc")
            };
            edidsOverride.Add(curMisc.EditorID);
            inputMod.MiscItems.Add(curMisc);
            overrideFlst.Items.Add(curMisc);
        }
    
        // and some remove forms
        for (uint i = 0; i < 5; i++)
        {
            var curMisc = new MiscItem(new FormKey(otherFileModKey, 0xf00 + i), SkyrimRelease.SkyrimSE)
            {
                EditorID = getNewEdid("remoteMisc", false)
            };
            overrideFlst.Items.Add(curMisc);
            edidsRemote.Add(curMisc.EditorID);
        }
        inputMod.FormLists.Add(overrideFlst);

        var sut = new MultiModFileSplitter();
        var outputList = sut.Split<ISkyrimMod, ISkyrimModGetter>(inputMod, 255);
        // expecting one file exactly, everything in expectedEdids to be present, and overrides to have stayed overrides
        // essentially, we should recieve one file, which is pretty much identical to inputMod (besides the formIDs)
        outputList.Count().Should().Be(1);
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
        edidsLocal.Count.Should().Be(0, "Not all local forms were found in output file");
        edidsOverride.Count.Should().Be(0, "Not all overridden forms were found in output file");
    }
}