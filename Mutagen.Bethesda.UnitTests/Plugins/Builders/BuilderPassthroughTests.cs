using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Builders;

public class BuilderPassthroughTests
{
    public class Payload
    {
        public Payload(
            IFileSystem fileSystem,
            DirectoryPath existingDataFolder,
            ModKey normalMasterKey,
            ModKey smallMasterKey,
            ModKey mediumMasterKey,
            ModKey originatingKey)
        {
            NormalMasterKey = normalMasterKey;
            SmallMasterKey = smallMasterKey;
            MediumMasterKey = mediumMasterKey;
            OriginatingKey = originatingKey;
            FileSystem = fileSystem;
            DataFolder = existingDataFolder;
        }

        public ModKey NormalMasterKey { get; }
        public ModKey SmallMasterKey { get; }
        public ModKey MediumMasterKey { get; }
        public ModKey OriginatingKey { get; }
        public IFileSystem FileSystem { get; }
        public DirectoryPath DataFolder { get; }

        public async Task<string> WriteMod<TModGetter>(
            TModGetter mod,
            bool useDataFolder)
            where TModGetter : IModGetter
        {
            var modPath = Path.Combine(DataFolder, mod.ModKey.ToString());
            await mod.BeginWrite
                .ToPath(modPath)
                .WithLoadOrder(new ModKey[]
                {
                    NormalMasterKey,
                    MediumMasterKey,
                    SmallMasterKey
                })
                .WithDataFolder(DataFolder)
                .WithFileSystem(FileSystem)
                .WriteAsync();
            return modPath;
        }
    }

    [Theory, MutagenAutoData]
    public async Task NonSeparated(
        Payload payload)
    {
        var normalMaster = new SkyrimMod(payload.NormalMasterKey, SkyrimRelease.SkyrimSE)
        {
            IsMaster = true,
        };
        var normalNpc = normalMaster.Npcs.AddNew();
        normalNpc.EditorID = "Normal";
        var smallMaster = new SkyrimMod(payload.SmallMasterKey, SkyrimRelease.SkyrimSE)
        {
            IsSmallMaster = true,
        };
        var smallNpc = smallMaster.Npcs.AddNew();
        smallNpc.EditorID = "Small";
        var originating = new SkyrimMod(payload.OriginatingKey, SkyrimRelease.SkyrimSE);
        var originatingNpc = originating.Npcs.AddNew();
        originatingNpc.EditorID = "Originating";

        originating.Npcs.GetOrAddAsOverride(normalNpc);
        originating.Npcs.GetOrAddAsOverride(smallNpc);
            
        normalMaster.WriteToBinary(Path.Combine(payload.DataFolder, normalMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        smallMaster.WriteToBinary(Path.Combine(payload.DataFolder, smallMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        
        var modPath = await payload.WriteMod(
            originating,
            useDataFolder: false);
        using var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(modPath)
            .WithFileSystem(payload.FileSystem)
            .Construct();

        mod.Npcs.Should().HaveCount(3);
        mod.Npcs.TryGetValue(normalNpc.FormKey, out var normalNpcReimport)
            .Should().BeTrue();
        normalNpcReimport!.FormKey.Should().Be(normalNpc.FormKey);
        mod.Npcs.TryGetValue(smallNpc.FormKey, out var smallNpcReimport)
            .Should().BeTrue();
        smallNpcReimport!.FormKey.Should().Be(smallNpc.FormKey);
        mod.Npcs.TryGetValue(originatingNpc.FormKey, out var originatingNpcReimport)
            .Should().BeTrue();
        originatingNpcReimport!.FormKey.Should().Be(originatingNpc.FormKey);
    }

    [Theory, MutagenAutoData]
    public async Task SeparatedCollision(
        Payload payload)
    {
        var normalMaster = new StarfieldMod(payload.NormalMasterKey, StarfieldRelease.Starfield)
        {
            IsMaster = true,
        };
        var normalNpc = normalMaster.Npcs.AddNew();
        normalNpc.EditorID = "Normal";
        var smallMaster = new StarfieldMod(payload.SmallMasterKey, StarfieldRelease.Starfield)
        {
            IsSmallMaster = true,
        };
        var smallNpc = smallMaster.Npcs.AddNew();
        smallNpc.EditorID = "Small";
        var mediumMaster = new StarfieldMod(payload.MediumMasterKey, StarfieldRelease.Starfield)
        {
            IsMediumMaster = true,
        };
        var mediumNpc = mediumMaster.Npcs.AddNew();
        mediumNpc.EditorID = "Medium";
        var originating = new StarfieldMod(payload.OriginatingKey, StarfieldRelease.Starfield);
        var originatingNpc = originating.Npcs.AddNew();
        originatingNpc.EditorID = "Originating";

        originating.Npcs.GetOrAddAsOverride(normalNpc);
        originating.Npcs.GetOrAddAsOverride(smallNpc);
        originating.Npcs.GetOrAddAsOverride(mediumNpc);
        
        normalMaster.WriteToBinary(Path.Combine(payload.DataFolder, normalMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        smallMaster.WriteToBinary(Path.Combine(payload.DataFolder, smallMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        mediumMaster.WriteToBinary(Path.Combine(payload.DataFolder, mediumMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        
        var modPath = await payload.WriteMod(
            originating,
            useDataFolder: false);
        Assert.Throws<MissingModMappingException>(() =>
        {
            using var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(modPath)
                .WithNoLoadOrder()
                .WithFileSystem(payload.FileSystem)
                .Construct();
        });
    }

    [Theory, MutagenAutoData]
    public async Task SeparatedWithDataFolder(
        Payload payload)
    {
        var normalMaster = new StarfieldMod(payload.NormalMasterKey, StarfieldRelease.Starfield)
        {
            IsMaster = true,
        };
        var normalNpc = normalMaster.Npcs.AddNew();
        normalNpc.EditorID = "Normal";
        var smallMaster = new StarfieldMod(payload.SmallMasterKey, StarfieldRelease.Starfield)
        {
            IsSmallMaster = true,
        };
        var smallNpc = smallMaster.Npcs.AddNew();
        smallNpc.EditorID = "Small";
        var mediumMaster = new StarfieldMod(payload.MediumMasterKey, StarfieldRelease.Starfield)
        {
            IsMediumMaster = true,
        };
        var mediumNpc = mediumMaster.Npcs.AddNew();
        mediumNpc.EditorID = "Medium";
        var originating = new StarfieldMod(payload.OriginatingKey, StarfieldRelease.Starfield);
        var originatingNpc = originating.Npcs.AddNew();
        originatingNpc.EditorID = "Originating";

        originating.Npcs.GetOrAddAsOverride(normalNpc);
        originating.Npcs.GetOrAddAsOverride(smallNpc);
        originating.Npcs.GetOrAddAsOverride(mediumNpc);
            
        normalMaster.WriteToBinary(Path.Combine(payload.DataFolder, normalMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        smallMaster.WriteToBinary(Path.Combine(payload.DataFolder, smallMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        mediumMaster.WriteToBinary(Path.Combine(payload.DataFolder, mediumMaster.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = payload.FileSystem
        });
        
        var modPath = await payload.WriteMod(
            originating,
            useDataFolder: true);
        using var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(payload.DataFolder)
            .WithFileSystem(payload.FileSystem)
            .Construct();

        mod.Npcs.Should().HaveCount(4);
        mod.Npcs.TryGetValue(normalNpc.FormKey, out var normalNpcReimport)
            .Should().BeTrue();
        normalNpcReimport!.FormKey.Should().Be(normalNpc.FormKey);
        mod.Npcs.TryGetValue(smallNpc.FormKey, out var smallNpcReimport)
            .Should().BeTrue();
        smallNpcReimport!.FormKey.Should().Be(smallNpc.FormKey);
        mod.Npcs.TryGetValue(mediumNpc.FormKey, out var mediumNpcReimport)
            .Should().BeTrue();
        mediumNpcReimport!.FormKey.Should().Be(mediumNpc.FormKey);
        mod.Npcs.TryGetValue(originatingNpc.FormKey, out var originatingNpcReimport)
            .Should().BeTrue();
        originatingNpcReimport!.FormKey.Should().Be(originatingNpc.FormKey);
    }
}