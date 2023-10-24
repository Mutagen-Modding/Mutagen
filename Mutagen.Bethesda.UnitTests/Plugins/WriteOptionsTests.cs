using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class WriteOptionsTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void NextFormID(
        OblivionMod mod, 
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        var nextId = mod.ModHeader.Stats.NextFormID;
        var npc = mod.Npcs.AddNew();
        npc.FormKey.ID.Should().Be(nextId);
        mod.WriteToBinary(existingModPath, new BinaryWriteParameters()
        {
            ModKey = ModKeyOption.NoCheck
        }, fileSystem: fileSystem);
        using var reimport = OblivionMod.CreateFromBinaryOverlay(existingModPath, fileSystem: fileSystem);
        reimport.ModHeader.Stats.NextFormID.Should().Be(nextId + 1);
    }
}