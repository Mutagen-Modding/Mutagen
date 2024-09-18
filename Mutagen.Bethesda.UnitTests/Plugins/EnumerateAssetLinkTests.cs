using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;
using AssetLinkQuery = Mutagen.Bethesda.Plugins.Assets.AssetLinkQuery;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class EnumerateAssetLinkTests
{
    [Theory, MutagenAutoData]
    public void Inferred(
        string scriptName)
    {
        var entry = new ScriptEntry()
        {
            Name = scriptName
        };
        entry.EnumerateAssetLinks(AssetLinkQuery.Inferred, null, null)
            .Should()
            .Equal(
                new AssetLink<SkyrimScriptCompiledAssetType>(Path.Combine("Scripts", $"{scriptName}.pex")),
                    new AssetLink<SkyrimScriptSourceAssetType>(Path.Combine("Scripts", "Source", $"{scriptName}.psc")));
    }
    
    [Theory, MutagenModAutoData]
    public void InferredUnderListed(
        SkyrimMod mod,
        Quest quest,
        string scriptName)
    {
        quest.VirtualMachineAdapter = new QuestAdapter()
        {
            Scripts = new ExtendedList<ScriptEntry>()
            {
                new ScriptEntry()
                {
                    Name = scriptName
                }
            }
        };
        quest.EnumerateAssetLinks(AssetLinkQuery.Inferred, null, null)
            .Should()
            .Equal(
                new AssetLink<SkyrimScriptCompiledAssetType>(Path.Combine("Scripts", $"{scriptName}.pex")),
                new AssetLink<SkyrimScriptSourceAssetType>(Path.Combine("Scripts", "Source", $"{scriptName}.psc")));
    }
    
    [Theory, MutagenModAutoData]
    public void InferredUnderListedFilteredOut(
        SkyrimMod mod,
        Quest quest,
        string scriptName)
    {
        quest.VirtualMachineAdapter = new QuestAdapter()
        {
            Scripts = new ExtendedList<ScriptEntry>()
            {
                new ScriptEntry()
                {
                    Name = scriptName
                }
            }
        };
        quest.EnumerateAssetLinks(AssetLinkQuery.Listed, null, null)
            .Should()
            .Equal();
    }
}