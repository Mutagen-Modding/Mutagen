using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.IO;
using Xunit;
using Constants = Mutagen.Bethesda.Skyrim.Constants;
using Weapon = Mutagen.Bethesda.Skyrim.Weapon;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class WriteTests
{
    public static readonly ModKey WriteKey = new ModKey("Write", ModType.Plugin);
    public static readonly ModKey BadWriteKey = new ModKey("BadWrite", ModType.Plugin);

    public static TempFile GetFile()
    {
        return new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
    }

    [Fact]
    public async Task BasicWrite()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        var weap = mod.Weapons.AddNew();
        await mod.BeginWrite
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .NoMastersListContentCheck()
            .SingleThread()
            .WriteAsync();
    }

    [Fact]
    public async Task BasicParallelWrite()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        var weap = mod.Weapons.AddNew();
        await mod.BeginWrite
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .NoMastersListContentCheck()
            .WriteAsync();
    }

    [Fact]
    public async Task ParallelWrite_MasterFlagSync_Throw()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(BadWriteKey, SkyrimRelease.SkyrimLE);
        var weap = mod.Weapons.AddNew();
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(tmp.File.Path)
                .WithNoLoadOrder()
                .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                .NoMastersListContentCheck()
                .WriteAsync();
        });
    }

    [Fact]
    public async Task Write_MasterFlagSync_Throw()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(BadWriteKey, SkyrimRelease.SkyrimLE);
        var weap = mod.Weapons.AddNew();
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
            {
                await mod.BeginWrite
                    .ToPath(tmp.File.Path)
                    .WithNoLoadOrder()
                    .WithModKeySync(ModKeyOption.ThrowIfMisaligned)
                    .NoMastersListContentCheck()
                    .WriteAsync();
            });
    }

    [Fact]
    public async Task ParallelWrite_MasterListSync_Throw()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
        await Assert.ThrowsAsync<AggregateException>(
            async () =>
            {
                await mod.BeginWrite
                    .ToPath(tmp.File.Path)
                    .WithNoLoadOrder()
                    .NoModKeySync()
                    .NoMastersListContentCheck()
                    .WriteAsync();
            });
    }

    [Fact]
    public async Task Write_MasterListSync_Throw()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
        await Assert.ThrowsAsync<RecordException>(
            async () =>
            {
                try
                {
                    await mod.BeginWrite
                        .ToPath(tmp.File.Path)
                        .WithNoLoadOrder()
                        .NoModKeySync()
                        .NoMastersListContentCheck()
                        .SingleThread()
                        .WriteAsync();
                }
                catch (RecordException ex)
                {
                    Assert.IsType<UnmappableFormIDException>(ex.InnerException);
                    throw;
                }
            });
    }

    [Fact]
    public async Task ParallelWrite_MasterListSync()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
        await mod.BeginWrite
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WriteAsync();
    }

    [Fact]
    public async Task Write_MasterListSync()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
        await mod.BeginWrite
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate)
            .SingleThread()
            .WriteAsync();
    }

    [Fact]
    public async Task Write_ModNotOnLoadOrder()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
        mod.Weapons.RecordCache.Set(
            new Weapon(FormKey.Factory("012345:SomeOtherMod.esp"), SkyrimRelease.SkyrimLE));
        await Assert.ThrowsAsync<MissingModException>(async () =>
        {
            await mod.BeginWrite
                .ToPath(tmp.File)
                .WithNoLoadOrder()
                .NoModKeySync()
                .WithMastersListContent(MastersListContentOption.Iterate)
                .WithMastersListOrdering(Constants.Skyrim.AsEnumerable())
                .WriteAsync();
        });
    }

    [Fact]
    public async Task WriteWithCounterLists()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        var armor = mod.Armors.AddNew();
        var writer = SkyrimMod.WriteBuilder(SkyrimRelease.SkyrimLE)
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate);
        await writer.WriteAsync(mod);
        armor.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        await writer.WriteAsync(mod);
        armor.Keywords.Add(FormKey.Null);
        await writer.WriteAsync(mod);
        for (int i = 0; i < 20000; i++)
        {
            armor.Keywords.Add(FormKey.Null);
        }
        Assert.Throws<AggregateException>(() =>
        {
            mod.BeginWrite
                .ToPath(tmp.File.Path)
                .WithNoLoadOrder()
                .NoModKeySync()
                .WithMastersListContent(MastersListContentOption.Iterate)
                .Write();
        });
    }

    [Fact]
    public async Task WriteWithMisalignedGameRelease()
    {
        using var tmp = GetFile();
        var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
        var writer = SkyrimMod.WriteBuilder(SkyrimRelease.SkyrimSE)
            .ToPath(tmp.File.Path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithMastersListContent(MastersListContentOption.Iterate);
        Assert.Throws<ArgumentException>(() =>
        {
            writer.Write(mod);
        });
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public async Task WriteWithDifferentModKeyWithStrings(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        string npcName, 
        ModPath modPath)
    {
        npc.Name = npcName;
        mod.UsingLocalization = true;
        mod.ModKey.Should().NotBe(modPath.ModKey);
        await mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithModKeySync(ModKeyOption.CorrectToPath)
            .WithFileSystem(fileSystem)
            .WriteAsync();
        var stringsPath = Path.Combine(modPath.Path.Directory, "Strings");
        fileSystem.Directory.Exists(stringsPath).Should().BeTrue();
        fileSystem.File.Exists(Path.Combine(stringsPath, $"{modPath.ModKey}_English.STRINGS"));
        fileSystem.File.Exists(Path.Combine(stringsPath, $"{modPath.ModKey}_English.DLSTRINGS"));
        fileSystem.File.Exists(Path.Combine(stringsPath, $"{modPath.ModKey}_English.ILSTRINGS"));
    }
}