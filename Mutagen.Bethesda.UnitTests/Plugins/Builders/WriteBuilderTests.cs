using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Builders;

public class WriteBuilderTests
{
    [Theory, MutagenAutoData]
    public void NoDataFolderWithHeaderMasters(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        Assert.Throws<MissingModMappingException>(() =>
        {
            mod.BeginWrite
                .ToPath(modPath)
                .WithLoadOrderFromHeaderMasters()
                .WithNoDataFolder()
                .WithFileSystem(fileSystem)
                .Write();
        });
    }
    
    [Theory, MutagenAutoData]
    public void ExtraIncludedMasters(
        IFileSystem fileSystem,
        ModKey masterModKey,
        ModKey modKey,
        ModKey extraMasterKey,
        DirectoryPath existingDir)
    {
        var extraMaster = new StarfieldMod(extraMasterKey, StarfieldRelease.Starfield);
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(master, extraMaster)
            .WithFileSystem(fileSystem)
            .WithExtraIncludedMasters(extraMasterKey)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(master, extraMaster)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(masterModKey, extraMasterKey);
    }
    
    [Theory, MutagenAutoData]
    public void OverrideMasters(
        IFileSystem fileSystem,
        ModKey masterModKey,
        ModKey modKey,
        ModKey overrideMasterKey,
        DirectoryPath existingDir)
    {
        var overrideMod = new StarfieldMod(overrideMasterKey, StarfieldRelease.Starfield);
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(master, overrideMod)
            .WithFileSystem(fileSystem)
            .WithExplicitOverridingMasterList(overrideMasterKey, masterModKey)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(master, overrideMod)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(masterModKey, overrideMasterKey);
    }
    
    [Theory, MutagenAutoData]
    public void WithAllParentMasters(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey masterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        var transientMasterModPath = Path.Combine(existingDataDir, transientMasterMod.ModKey.FileName);
        transientMasterMod.BeginWrite
            .ToPath(transientMasterModPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();
        
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        master.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        var masterModPath = Path.Combine(existingDataDir, master.ModKey.FileName);
        master.BeginWrite
            .ToPath(masterModPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .WithAllParentMasters()
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, master)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(transientMasterModKey, masterModKey);
    }
    
    [Theory, MutagenAutoData]
    public void WithAllParentMastersNoDataFolder(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey masterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        master.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = transientMasterModKey
        });
        var masterWeapon = master.Weapons.AddNew();
        master.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(transientMasterMod, master)
            .WithFileSystem(fileSystem)
            .WithDataFolder(existingDataDir)
            .WithAllParentMasters()
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, master)
            .WithFileSystem(fileSystem)
            .WithDataFolder(existingDataDir)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(transientMasterModKey, masterModKey);
    }
    
    [Theory, MutagenAutoData]
    public void WithAllParentMastersCircular(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey masterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        transientMasterMod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = modKey
        });
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        master.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = transientMasterModKey
        });
        var masterWeapon = master.Weapons.AddNew();
        master.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(transientMasterMod, master)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .WithAllParentMasters()
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, master)
            .WithFileSystem(fileSystem)
            .WithDataFolder(existingDataDir)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(transientMasterModKey, masterModKey);
    }

    /// <summary>
    /// A mod lists masters where one of them is not present on disk in the data folder,
    /// but its master style is supplied via
    /// <see cref="IBinaryModdedWriteBuilderDataFolderChoice.WithKnownMasters(KeyedMasterStyle[])"/>.
    /// Combined with <see cref="WithLoadOrderFromHeaderMasters"/>, the existing-on-disk master
    /// (which itself has its own master) must still parse successfully — the master flag
    /// lookup must fall back to the supplied known-masters when a listing is missing on disk
    /// rather than short-circuiting to null and surfacing
    /// MissingModMappingException("Master flag lookup was not provided.").
    /// </summary>
    [Theory, MutagenAutoData]
    public void WithLoadOrderFromHeaderMastersKnownMasterMissingFromDisk(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey existingMasterModKey,
        ModKey missingMasterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        // A leaf master, on disk in the data folder.
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        var transientMasterModPath = Path.Combine(existingDataDir, transientMasterMod.ModKey.FileName);
        transientMasterMod.BeginWrite
            .ToPath(transientMasterModPath)
            .WithLoadOrder(transientMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        // A master that itself has a master, on disk in the data folder.
        var existingMasterMod = new StarfieldMod(existingMasterModKey, StarfieldRelease.Starfield);
        existingMasterMod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = transientMasterModKey
        });
        var existingMasterWeapon = existingMasterMod.Weapons.AddNew();
        existingMasterMod.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        var existingMasterModPath = Path.Combine(existingDataDir, existingMasterMod.ModKey.FileName);
        existingMasterMod.BeginWrite
            .ToPath(existingMasterModPath)
            .WithLoadOrder(transientMasterModKey, existingMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        // A master whose file is NOT on disk; only its style is supplied via WithKnownMasters.
        var missingMasterMod = new StarfieldMod(missingMasterModKey, StarfieldRelease.Starfield);
        var missingMasterWeapon = missingMasterMod.Weapons.AddNew();

        // Mod references both as direct masters.
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = existingMasterModKey
        });
        mod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = missingMasterModKey
        });
        mod.Weapons.GetOrAddAsOverride(existingMasterWeapon);
        mod.Weapons.GetOrAddAsOverride(missingMasterWeapon);

        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(existingDataDir)
            .WithKnownMasters(missingMasterMod)
            .WithFileSystem(fileSystem)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, existingMasterMod, missingMasterMod)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(existingMasterModKey, missingMasterModKey);
    }

    /// <summary>
    /// Same scenario as <see cref="WithLoadOrderFromHeaderMastersKnownMasterMissingFromDisk"/>,
    /// but exercising the non-modded <see cref="StarfieldMod.WriteBuilder"/> entry point so the
    /// fix to the sibling <c>BinaryWriteBuilderLoadOrderChoice.WithLoadOrderFromHeaderMasters</c>
    /// is also covered.
    /// </summary>
    [Theory, MutagenAutoData]
    public void NonModdedWithLoadOrderFromHeaderMastersKnownMasterMissingFromDisk(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey existingMasterModKey,
        ModKey missingMasterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        transientMasterMod.BeginWrite
            .ToPath(Path.Combine(existingDataDir, transientMasterMod.ModKey.FileName))
            .WithLoadOrder(transientMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        var existingMasterMod = new StarfieldMod(existingMasterModKey, StarfieldRelease.Starfield);
        existingMasterMod.ModHeader.MasterReferences.Add(new MasterReference()
        {
            Master = transientMasterModKey
        });
        var existingMasterWeapon = existingMasterMod.Weapons.AddNew();
        existingMasterMod.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        existingMasterMod.BeginWrite
            .ToPath(Path.Combine(existingDataDir, existingMasterMod.ModKey.FileName))
            .WithLoadOrder(transientMasterModKey, existingMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        var missingMasterMod = new StarfieldMod(missingMasterModKey, StarfieldRelease.Starfield);
        var missingMasterWeapon = missingMasterMod.Weapons.AddNew();

        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = existingMasterModKey });
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = missingMasterModKey });
        mod.Weapons.GetOrAddAsOverride(existingMasterWeapon);
        mod.Weapons.GetOrAddAsOverride(missingMasterWeapon);

        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        StarfieldMod.WriteBuilder(StarfieldRelease.Starfield)
            .ToPath(modPath, fileSystem)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(existingDataDir)
            .WithKnownMasters(missingMasterMod)
            .Write(mod);

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, existingMasterMod, missingMasterMod)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(existingMasterModKey, missingMasterModKey);
    }

    /// <summary>
    /// Exercises the non-modded <c>WithLoadOrder(IEnumerable&lt;ModKey&gt;)</c> deep-import path
    /// (BinaryWriteBuilder.cs site #2) with a master missing from disk but supplied via
    /// <c>WithKnownMasters</c>.
    /// </summary>
    [Theory, MutagenAutoData]
    public void NonModdedWithLoadOrderKnownMasterMissingFromDisk(
        IFileSystem fileSystem,
        ModKey existingMasterModKey,
        ModKey missingMasterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var existingMasterMod = new StarfieldMod(existingMasterModKey, StarfieldRelease.Starfield);
        var existingMasterWeapon = existingMasterMod.Weapons.AddNew();
        existingMasterMod.BeginWrite
            .ToPath(Path.Combine(existingDataDir, existingMasterMod.ModKey.FileName))
            .WithLoadOrder(existingMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        var missingMasterMod = new StarfieldMod(missingMasterModKey, StarfieldRelease.Starfield);
        var missingMasterWeapon = missingMasterMod.Weapons.AddNew();

        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = existingMasterModKey });
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = missingMasterModKey });
        mod.Weapons.GetOrAddAsOverride(existingMasterWeapon);
        mod.Weapons.GetOrAddAsOverride(missingMasterWeapon);

        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        StarfieldMod.WriteBuilder(StarfieldRelease.Starfield)
            .ToPath(modPath, fileSystem)
            .WithLoadOrder(existingMasterModKey, missingMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithKnownMasters(missingMasterMod)
            .Write(mod);

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(existingMasterMod, missingMasterMod)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master)
            .ShouldEqualEnumerable(existingMasterModKey, missingMasterModKey);
    }

    /// <summary>
    /// Negative case: a referenced master is neither on disk nor supplied via
    /// <c>WithKnownMasters</c>. The write should fail loudly with a clear, actionable
    /// error — not the lower-level "Master flag lookup was not provided." which indicates
    /// the lookup itself failed to construct.
    /// </summary>
    [Theory, MutagenAutoData]
    public void WithLoadOrderFromHeaderMastersUnknownMasterMissingFromDisk(
        IFileSystem fileSystem,
        ModKey existingMasterModKey,
        ModKey missingMasterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var existingMasterMod = new StarfieldMod(existingMasterModKey, StarfieldRelease.Starfield);
        var existingMasterWeapon = existingMasterMod.Weapons.AddNew();
        existingMasterMod.BeginWrite
            .ToPath(Path.Combine(existingDataDir, existingMasterMod.ModKey.FileName))
            .WithLoadOrder(existingMasterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();

        // missingMasterMod is in-memory only and NOT supplied via WithKnownMasters.
        var missingMasterMod = new StarfieldMod(missingMasterModKey, StarfieldRelease.Starfield);
        var missingMasterWeapon = missingMasterMod.Weapons.AddNew();

        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = existingMasterModKey });
        mod.ModHeader.MasterReferences.Add(new MasterReference() { Master = missingMasterModKey });
        mod.Weapons.GetOrAddAsOverride(existingMasterWeapon);
        mod.Weapons.GetOrAddAsOverride(missingMasterWeapon);

        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        Assert.ThrowsAny<Exception>(() =>
        {
            mod.BeginWrite
                .ToPath(modPath)
                .WithLoadOrderFromHeaderMasters()
                .WithDataFolder(existingDataDir)
                .WithFileSystem(fileSystem)
                .Write();
        }).ShouldNotBeOfType<MissingModMappingException>(
            "Negative case should surface a higher-level error (e.g. UnmappableFormIDException or MissingModException), not the lookup-construction failure.");
    }
}