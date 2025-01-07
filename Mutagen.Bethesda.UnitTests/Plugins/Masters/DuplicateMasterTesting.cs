using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class DuplicateMasterTesting
{
    [Fact]
    public void NotSeparated()
    {
        var mod = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimDuplicateMasterListing,
            SkyrimRelease.SkyrimSE);
        mod.Npcs.Count.Should().Be(4);
        mod.Npcs.Records.First(n => n.EditorID == "Originating")
            .FormKey.ModKey.FileName.String.Should().Be("DuplicateMasterListing.esp");
        mod.Npcs.Records.First(n => n.EditorID == "MasterAFirst")
            .FormKey.ModKey.FileName.String.Should().Be("MasterA.esm");
        mod.Npcs.Records.First(n => n.EditorID == "MasterBFirst")
            .FormKey.ModKey.FileName.String.Should().Be("MasterB.esm");
        mod.Npcs.Records.First(n => n.EditorID == "MasterASecond")
            .FormKey.ModKey.FileName.String.Should().Be("MasterA.esm");
    }
    
    [Fact]
    public void Separated()
    {
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(TestDataPathing.StarfieldDuplicateMasterListing)
            .WithKnownMasters(
                new KeyedMasterStyle(ModKey.FromFileName("DuplicateMasterListing.esp"), MasterStyle.Full),
                new KeyedMasterStyle(ModKey.FromFileName("MasterA.esm"), MasterStyle.Full),
                new KeyedMasterStyle(ModKey.FromFileName("MasterB.esm"), MasterStyle.Full),
                new KeyedMasterStyle(ModKey.FromFileName("MasterA.esm"), MasterStyle.Full))
            .Construct();
        mod.Npcs.Count.Should().Be(4);
        mod.Npcs.Records.First(n => n.EditorID == "Originating")
            .FormKey.ModKey.FileName.String.Should().Be("DuplicateMasterListing.esp");
        mod.Npcs.Records.First(n => n.EditorID == "MasterAFirst")
            .FormKey.ModKey.FileName.String.Should().Be("MasterA.esm");
        mod.Npcs.Records.First(n => n.EditorID == "MasterBFirst")
            .FormKey.ModKey.FileName.String.Should().Be("MasterB.esm");
        mod.Npcs.Records.First(n => n.EditorID == "MasterASecond")
            .FormKey.ModKey.FileName.String.Should().Be("MasterA.esm");
    }
}