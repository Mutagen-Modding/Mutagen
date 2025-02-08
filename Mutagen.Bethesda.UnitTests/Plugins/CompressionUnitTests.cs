using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class CompressionUnitTests
{
    [Theory, MutagenModAutoData]
    public void CompressedExport(
        IFileSystem fileSystem,
        ModPath filePath,
        SkyrimMod mod,
        Npc npc)
    {
        npc.EditorID = "Test123";
        npc.IsCompressed = true;
        mod.BeginWrite
            .ToPath(filePath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(filePath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.Npcs.Select(x => x.EditorID)
            .ShouldEqual("Test123");
    }
}