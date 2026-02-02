using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FormList = Mutagen.Bethesda.Skyrim.FormList;
using MiscItem = Mutagen.Bethesda.Skyrim.MiscItem;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class AutoSplitModWriterTests
{
    #region Util

    public class Payload
    {
        private readonly Func<FormKey> _formKeyGen;
        private readonly Func<string> _edidFunc;
        public SkyrimMod Mod { get; }

        // just to be able to create unique edids
        private int lastEdidIndex = 0;

        /// <summary>
        /// Stores EDIDs of generated forms in the input class, to be able to track them in the generated files
        /// </summary>
        private HashSet<string> expectedEdids = new();

        public Payload(SkyrimMod mod, Func<ModKey> modKeyGen, Func<string> edidFunc)
        {
            _formKeyGen = () => new FormKey(modKeyGen(), 0x800);
            _edidFunc = edidFunc;
            Mod = mod;
        }

        public string GetNewEdid(string baseEdid, bool addToExpected = true)
        {
            var newEdid = baseEdid + lastEdidIndex;
            lastEdidIndex++;
            if (addToExpected)
            {
                expectedEdids.Add(newEdid);
            }
            return newEdid;
        }

        /// <summary>
        /// Generates MISC items from NOT within the current file
        /// </summary>
        public void FillFormListWithRemoteRecords(FormList flst, int numFiles)
        {
            for (uint i = 0; i < numFiles; i++)
            {
                flst.Items.Add(_formKeyGen());
            }
        }

        public FormList CreateFormListWithContents(int numFiles)
        {
            var flst = Mod.FormLists.AddNew();
            // Set EditorID to FormKey so we can track it after split
            flst.EditorID = flst.FormKey.ToString();
            FillFormListWithRemoteRecords(flst, numFiles);
            expectedEdids.Add(flst.EditorID);
            return flst;
        }

        public IEnumerable<string> GetExpectedEdids() => expectedEdids;
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
    public void BasicAutoSplitTest(Payload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);

        // Create a mod that exceeds master limit
        var originalFormLists = new List<IFormListGetter>();
        for (uint i = 0; i < 5; i++)
        {
            var flst = payload.CreateFormListWithContents(70); // Each has 70 masters
            originalFormLists.Add(flst);
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        // This should trigger auto-split since we have way more than 254 masters
        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Verify split files were created (when splitting occurs, files get _1, _2, etc suffixes)
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        var splitFile3Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile3Path).ShouldBeFalse();

        // Re-import the split mods and verify content
        var splitMod1 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile1Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });
        var splitMod2 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile2Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });

        // Collect all FormLists from both split mods
        var reimportedFormLists = new List<IFormListGetter>();
        reimportedFormLists.AddRange(splitMod1.FormLists);
        reimportedFormLists.AddRange(splitMod2.FormLists);

        // Verify we have the same number of FormLists
        reimportedFormLists.Count.ShouldBe(originalFormLists.Count);

        // Verify each FormList by EditorID (which is set to FormKey)
        var expectedEdids = payload.GetExpectedEdids().ToHashSet();
        var reimportedEdids = reimportedFormLists.Select(f => f.EditorID).ToHashSet();

        reimportedEdids.ShouldBe(expectedEdids);

        // Verify the content of each FormList matches
        foreach (var originalFormList in originalFormLists)
        {
            var reimportedFormList = reimportedFormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            reimportedFormList.ShouldNotBeNull();

            // Verify the items are the same
            reimportedFormList.Items.Count.ShouldBe(originalFormList.Items.Count);
            for (int i = 0; i < originalFormList.Items.Count; i++)
            {
                reimportedFormList.Items[i].FormKey.ShouldBe(originalFormList.Items[i].FormKey);
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void NoSplitWhenNotNeededTest(SkyrimMod mod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        // Write should succeed without splitting
        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // File should be written
        fileSystem.File.Exists(outputPath).ShouldBeTrue();

        // No split files should exist - check for _1 and _2 suffixes
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);

        fileSystem.File.Exists(Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}")).ShouldBeFalse();
        fileSystem.File.Exists(Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}")).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void CleansUpOldSplitFilesWithGaps(Payload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);

        // Create stale split files with gaps to simulate a previous export with more files
        var staleFile3 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");
        var staleFile5 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_5{extension}");
        var staleFile7 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_7{extension}");

        fileSystem.File.WriteAllText(staleFile3, "stale");
        fileSystem.File.WriteAllText(staleFile5, "stale");
        fileSystem.File.WriteAllText(staleFile7, "stale");

        // Create a mod that will split into 2 files (need >254 masters to trigger split)
        for (uint i = 0; i < 5; i++)
        {
            payload.CreateFormListWithContents(70); // 5 x 70 = 350 masters
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Verify split files were created
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();

        // Verify stale files were cleaned up despite gaps in numbering
        fileSystem.File.Exists(staleFile3).ShouldBeFalse();
        fileSystem.File.Exists(staleFile5).ShouldBeFalse();
        fileSystem.File.Exists(staleFile7).ShouldBeFalse();
    }
}

