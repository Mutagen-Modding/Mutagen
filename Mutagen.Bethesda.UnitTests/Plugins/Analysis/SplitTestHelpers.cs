using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.IO.Abstractions;
using FormList = Mutagen.Bethesda.Skyrim.FormList;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class SplitTestPayload
{
    private readonly Func<FormKey> _formKeyGen;
    private readonly Func<string> _edidFunc;
    public SkyrimMod Mod { get; }

    private int lastEdidIndex = 0;
    private readonly HashSet<string> expectedEdids = new();

    public SplitTestPayload(SkyrimMod mod, Func<ModKey> modKeyGen, Func<string> edidFunc)
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
        flst.EditorID = GetNewEdid("testFormList_" + _edidFunc());
        FillFormListWithRemoteRecords(flst, numFiles);
        return flst;
    }

    public IEnumerable<string> GetExpectedEdids() => expectedEdids;
}

public static class SplitTestUtil
{
    public static HashSet<ModKey> ExtractMasters(IModGetter mod, DirectoryPath outputDir, IFileSystem fileSystem)
    {
        var path = new ModPath(mod.ModKey, Path.Combine(outputDir.Path, mod.ModKey.FileName));
        mod.WriteToBinary(path, new BinaryWriteParameters
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck,
        });
        return MasterReferenceCollection
            .FromPath(path, mod.GameRelease, fileSystem)
            .Masters
            .Select(m => m.Master)
            .ToHashSet();
    }

    public static int WrittenMasterCount(IModGetter mod, DirectoryPath outputDir, IFileSystem fileSystem)
        => ExtractMasters(mod, outputDir, fileSystem).Count;
}
