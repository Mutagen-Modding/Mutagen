using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class MaterialSubstitutionTreeFolderTests
{
    private const ushort LegacyFormVersion = MaterialSwapBinaryCreateTranslation.NewFormVersion - 1;

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void SubstitutionKeepsItsTreeFolderExactlyFromTheNewFormVersion(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        Fallout4Mod mod,
        MaterialSwap current,
        MaterialSwap legacy,
        string treeFolder,
        string originalMaterial,
        float colorRemappingIndex)
    {
        current.FormVersion = GameConstants.Fallout4.DefaultFormVersion!.Value;
        current.TreeFolder = treeFolder;
        current.Substitutions.Add(new MaterialSubstitution
        {
            OriginalMaterial = originalMaterial,
            ObsoleteTreeFolder = string.Empty,
            ColorRemappingIndex = colorRemappingIndex,
        });
        current.Substitutions.Add(new MaterialSubstitution
        {
            OriginalMaterial = originalMaterial,
            ColorRemappingIndex = colorRemappingIndex,
        });
        legacy.FormVersion = LegacyFormVersion;
        legacy.TreeFolder = treeFolder;
        legacy.Substitutions.Add(new MaterialSubstitution
        {
            OriginalMaterial = originalMaterial,
            ObsoleteTreeFolder = string.Empty,
        });
        var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        ShouldHoldTheTreeFolders(overlay, current, legacy, treeFolder, colorRemappingIndex);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(rewritePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ShouldHoldTheTreeFolders(rewritten, current, legacy, treeFolder, colorRemappingIndex);
    }

    private static void ShouldHoldTheTreeFolders(
        IFallout4ModGetter mod,
        IMaterialSwapGetter current,
        IMaterialSwapGetter legacy,
        string treeFolder,
        float colorRemappingIndex)
    {
        var rereadCurrent = mod.MaterialSwaps[current.FormKey];
        rereadCurrent.TreeFolder.ShouldBe(treeFolder);
        rereadCurrent.Substitutions.Select(x => x.ObsoleteTreeFolder).ShouldBe(new string?[] { string.Empty, null });
        rereadCurrent.Substitutions.Select(x => x.ColorRemappingIndex).ShouldBe(new float?[] { colorRemappingIndex, colorRemappingIndex });

        var rereadLegacy = mod.MaterialSwaps[legacy.FormKey];
        rereadLegacy.TreeFolder.ShouldBe(treeFolder);
        rereadLegacy.Substitutions.ShouldHaveSingleItem().ObsoleteTreeFolder.ShouldBeNull();
    }
}
