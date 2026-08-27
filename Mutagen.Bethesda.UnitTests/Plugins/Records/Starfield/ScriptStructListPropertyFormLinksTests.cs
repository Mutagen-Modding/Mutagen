using Shouldly;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.UnitTests.Plugins.Masters;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

// Mutagen-Modding/Mutagen#688: same gap as the Fallout4 case (see
// Plugins/Records/Fallout4/ScriptStructListPropertyFormLinksTests.cs) — Starfield defines
// ScriptStructListProperty/ScriptEntryStructs with the same circular Members shape.
public class ScriptStructListPropertyFormLinksTests
{
    private static Quest BuildQuestWithStructListFormLink(StarfieldMod mod, FormKey linkedKey)
    {
        var quest = mod.Quests.AddNew();
        var adapter = new QuestAdapter();
        quest.VirtualMachineAdapter = adapter;
        var entry = new ScriptEntry { Name = "TestScript" };
        var structListProp = new ScriptStructListProperty { Name = "TestStructListProp" };
        var entryStruct = new ScriptEntryStructs();
        var objProp = new ScriptObjectProperty { Name = "TestObjProp" };
        objProp.Object.SetTo(linkedKey);
        entryStruct.Members.Add(objProp);
        structListProp.Structs.Add(entryStruct);
        entry.Properties.Add(structListProp);
        adapter.Scripts.Add(entry);
        return quest;
    }

    [Fact]
    public void EnumerateFormLinks_DescendsIntoStructListMembers()
    {
        var mod = new StarfieldMod(ModKey.FromNameAndExtension("Test.esp"), StarfieldRelease.Starfield);
        var otherKey = FormKey.Factory("123456:Other.esp");
        var quest = BuildQuestWithStructListFormLink(mod, otherKey);

        quest.EnumerateFormLinks().Select(x => x.FormKey).ShouldContain(otherKey);
    }

    [Theory, MutagenAutoData]
    public void Write_WithStructListFormLink_KeepsMasterAndDoesNotThrow(
        IFileSystem fileSystem,
        DirectoryPath existingFolder)
    {
        var modKey = ModKey.FromNameAndExtension("Test.esp");
        var otherKey = FormKey.Factory("123456:Other.esp");
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        BuildQuestWithStructListFormLink(mod, otherKey);

        // Starfield separates masters by MasterStyle (full/medium/small), so - unlike
        // Fallout4 - writing needs a load order that resolves each referenced master's
        // style, not WithNoLoadOrder().
        var lo = new[] { MastersTestUtil.GetFlags(otherKey.ModKey, MasterStyle.Full) };

        var modPath = Path.Combine(existingFolder, modKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(lo)
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(lo)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.ModHeader.MasterReferences.Select(m => m.Master).ShouldContain(otherKey.ModKey);
    }
}
