using Shouldly;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

// Mutagen-Modding/Mutagen#688: ScriptStructListProperty.EnumerateFormLinks did not
// descend into Structs[*].Members, so a master referenced only from a VMAD struct-array
// property was pruned by MastersListContentOption.Iterate and the write threw
// UnmappableFormIDException.
public class ScriptStructListPropertyFormLinksTests
{
    private static Quest BuildQuestWithStructListFormLink(Fallout4Mod mod, FormKey linkedKey)
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
        var mod = new Fallout4Mod(ModKey.FromNameAndExtension("Test.esp"), Fallout4Release.Fallout4);
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
        var mod = new Fallout4Mod(modKey, Fallout4Release.Fallout4);
        BuildQuestWithStructListFormLink(mod, otherKey);

        var modPath = Path.Combine(existingFolder, modKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .WithFileSystem(fileSystem)
            .Write();

        using var reimport = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.ModHeader.MasterReferences.Select(m => m.Master).ShouldContain(otherKey.ModKey);
    }
}
