using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class NpcFaceMorphTailTests
{
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void TailRoundTripsByteForByte(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        Fallout4Mod mod,
        Npc npc)
    {
        byte[] tail = [1, 0, 0, 0, 1, 0, 0, 0];
        npc.FaceMorphs.Add(new NpcFaceMorph { Unknown = tail });
        var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        TailOf(overlay).ShouldBe(tail);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(rewritePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        TailOf(rewritten).ShouldBe(tail);
    }

    private static byte[] TailOf(IFallout4ModGetter mod) =>
        mod.Npcs.ShouldHaveSingleItem().FaceMorphs.ShouldHaveSingleItem().Unknown.ToArray();
}
